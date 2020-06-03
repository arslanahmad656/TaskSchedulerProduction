using Imanami.Common;
using Imanami.Common.DeepCopy;
using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Filter;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.DataTransferObjects.Enums;
using Imanami.GroupID.TaskScheduler;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public class ExpiringGroupsProcessor : GroupsProcessor
	{
		public HashSet<string> ExtendedGroups
		{
			get;
			set;
		}

		public ExpiringGroupsProcessor()
		{
		}

		private List<IdentityStoreObject> EnsureChildGroups(int identityStoreId, ServicesGroupServiceClient groupClient, List<IdentityStoreObject> managedGroups)
		{
			Func<IdentityStoreObject, bool> func = null;
			List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
			foreach (IdentityStoreObject pair in managedGroups)
			{
				try
				{
					List<IdentityStoreObject> childGroups = groupClient.GetAllLevelCurrentChildGroups(identityStoreId, pair.get_ObjectIdFromIdentityStore(), null, this.GetAttributesToLoad());
					List<IdentityStoreObject> identityStoreObjects1 = identityStoreObjects;
					List<IdentityStoreObject> identityStoreObjects2 = childGroups;
					Func<IdentityStoreObject, bool> func1 = func;
					if (func1 == null)
					{
						Func<IdentityStoreObject, bool> func2 = (IdentityStoreObject grp) => !identityStoreObjects.Any<IdentityStoreObject>((IdentityStoreObject obj1) => obj1.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase));
						Func<IdentityStoreObject, bool> func3 = func2;
						func = func2;
						func1 = func3;
					}
					identityStoreObjects1.AddRange(identityStoreObjects2.Where<IdentityStoreObject>(func1));
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					GroupsProcessor.logger.Error(string.Concat("Error occurred in getting child groups after smart group update. ", ex.Message));
					continue;
				}
			}
			return identityStoreObjects;
		}

		public virtual void ExpireTheGroups(List<IdentityStoreObject> groups)
		{
			List<IdentityStoreObject> groupsForExtension = new List<IdentityStoreObject>();
			List<string> strs = new List<string>();
			List<string> groupsToNotifyForExtension = new List<string>();
			List<IdentityStoreObject> groupsToNotifyUpdate = new List<IdentityStoreObject>();
			List<IdentityStoreObject> groupsOld = DeepCopyExtensionMethods.DeepCopy<List<IdentityStoreObject>>(groups);
			foreach (IdentityStoreObject group in groups)
			{
				if (!this.IsGroup(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} is not a group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
				}
				else if (this.IsSystemGroup(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} is a system group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
				}
				else if (this.IsGroupInExcludedContainer(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} is in excluded containers.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
				}
				else if (!this.ShouldExpireSecurityGroup(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} is a security group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
				}
				else if (this.ExtendLifeForGUS(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Extending life of object {0}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()));
					groupsForExtension.Add(group);
					groupsToNotifyForExtension.Add(group.get_ObjectIdFromIdentityStore());
				}
				else if (this.PrepareGroupForLifeExtension(group))
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Extending life of object {0}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()));
					groupsForExtension.Add(group);
				}
				else if (!this.HasReceivedNotificationInLast7Days(group))
				{
					strs.Add(group.get_ObjectIdFromIdentityStore());
					groupsToNotifyUpdate.Add(group);
				}
				else
				{
					GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} has received notification within last 7 days. Not expiring.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
				}
			}
			ServicesGroupServiceClient groupServiceClient = new ServicesGroupServiceClient(false);
			if (groupsForExtension.Count > 0)
			{
				List<IdentityStoreObject> groupsToUpdate = this.CloneObjectsForUpdate(new List<string>()
				{
					"XGroupExpirationDate"
				}, groupsForExtension, groupsOld);
				string compressedString = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(groupsToUpdate);
				ActionResult result = groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), compressedString, typeof(IdentityStoreObject).FullName);
				this.LogResults(result, "ExtendGroupsLife");
			}
			if (strs.Count > 0)
			{
				ActionResult result = groupServiceClient.Expire(Helper.CurrentTask.get_IdentityStoreId(), strs);
				this.LogResults(result, "ExpireGroups");
				this.GetExcludedNestedGroups((
					from grp in groups
					where strs.Contains(grp.get_ObjectIdFromIdentityStore())
					select grp).ToList<IdentityStoreObject>(), strs);
				result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 28, strs);
				this.LogResults(result, "ExpireGroups-Notifications");
				groupsToNotifyUpdate.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
				List<IdentityStoreObject> groupsToUpdate = this.CloneObjectsForUpdate(new List<string>()
				{
					"IMGLastSentExpireNotificationDate"
				}, groupsToNotifyUpdate, null);
				groupsToUpdate.ForEach((IdentityStoreObject g) => g.set_StopNotification(true));
				string compressedString = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(groupsToUpdate);
				result = groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), compressedString, typeof(IdentityStoreObject).FullName);
				this.LogResults(result, "UpdateExpireGroupsNotificationData");
			}
			if (groupsToNotifyForExtension.Count > 0)
			{
				ActionResult result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 35, groupsToNotifyForExtension);
				this.LogResults(result, "ExtendedGroups-Notifications");
			}
		}

		public virtual void ExpireTheGroupsWhichAreDueForExpiry()
		{
			int totalFound = 0;
			ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
			FilterCriteria filterCriteria = this.GetExpiringGroupsFilter();
			Dictionary<string, bool> containers = null;
			if ((Helper.CurrentTask.get_Targets() == null ? false : Helper.CurrentTask.get_Targets().Count > 0))
			{
				containers = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
			}
			SearchFilter searchFilter1 = new SearchFilter();
			searchFilter1.set_ExtensionDataCriteria(filterCriteria);
			searchFilter1.set_ProviderCriteria(new FilterCriteria());
			SearchFilter searchFilter = searchFilter1;
			List<IdentityStoreObject> groupsToExpire = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, containers, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
			this.ExpireTheGroups(groupsToExpire);
			if (totalFound > 20000)
			{
			}
		}

		private void FilterGroups(List<IdentityStoreObject> expiringGroups, List<IdentityStoreObject> groups, int type)
		{
			expiringGroups.ForEach((IdentityStoreObject obj) => {
				ManagedGroupTypes groupType;
				if (obj.get_AttributesBusinessObject() != null)
				{
					string grpType = this.GetAttributeValue(obj.get_AttributesBusinessObject(), "IMSGManagedGroupType");
					if (!string.IsNullOrEmpty(grpType))
					{
						Enum.TryParse<ManagedGroupTypes>(grpType, true, out groupType);
						if ((type != 1 ? false : (groupType == 2 ? true : groupType == 6)))
						{
							groups.Add(obj);
						}
						if ((type != 2 ? false : groupType == 3))
						{
							groups.Add(obj);
						}
					}
				}
			});
		}

		private string GetAttributeValue(AttributeCollection collection, string attribute)
		{
			string value;
			if ((collection == null || collection.get_AttributesCollection() == null ? false : collection.get_AttributesCollection().Count > 0))
			{
				if (collection.IsIn(attribute))
				{
					if ((collection.get_AttributesCollection()[attribute] == null ? false : collection.get_AttributesCollection()[attribute].Count > 0))
					{
						value = collection.get_AttributesCollection()[attribute][0].get_Value();
						return value;
					}
				}
			}
			value = string.Empty;
			return value;
		}

		public virtual FilterCriteria GetCriteriaForExpiringNotification()
		{
			FilterCriteria filterCriterium = new FilterCriteria();
			filterCriterium.set_Child(new List<FilterCriteria>());
			FilterCriteria filterCriteria = filterCriterium;
			filterCriteria.set_Operator("and");
			FilterCriteria filterCriterium1 = new FilterCriteria();
			filterCriterium1.set_Attribute("IMGIsExpired");
			filterCriterium1.set_Operator("is exactly");
			filterCriterium1.set_Value("false");
			filterCriterium1.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium1);
			FilterCriteria filterCriterium2 = new FilterCriteria();
			filterCriterium2.set_Attribute("IMGIsDeleted");
			filterCriterium2.set_Operator("is exactly");
			filterCriterium2.set_Value("false");
			filterCriterium2.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium2);
			FilterCriteria filterCriterium3 = new FilterCriteria();
			filterCriterium3.set_Attribute("XGroupExpirationDate");
			filterCriterium3.set_Operator("less or equal");
			DateTime date = DateTime.Now.AddDays(30);
			date = date.Date;
			filterCriterium3.set_Value(date.ToString("yyyy MMMM dd HH:mm:ss"));
			filterCriterium3.set_ValueType(4);
			filterCriteria.get_Child().Add(filterCriterium3);
			return filterCriteria;
		}

		private void GetExcludedNestedGroups(List<IdentityStoreObject> expiringGroups)
		{
			if ((expiringGroups == null ? false : expiringGroups.Count > 0))
			{
				List<IdentityStoreObject> smartGroups = new List<IdentityStoreObject>();
				List<IdentityStoreObject> parentDynasties = new List<IdentityStoreObject>();
				this.FilterGroups(expiringGroups, smartGroups, 1);
				this.FilterGroups(expiringGroups, parentDynasties, 2);
				FilterCriteria nestGroupsFilterCriteria = this.PrepareNestChildsCriteria(smartGroups);
				if ((nestGroupsFilterCriteria.get_Child() == null ? false : nestGroupsFilterCriteria.get_Child().Count > 0))
				{
					SearchFilter searchFilter1 = new SearchFilter();
					searchFilter1.set_ExtensionDataCriteria(nestGroupsFilterCriteria);
					searchFilter1.set_ProviderCriteria(new FilterCriteria());
					SearchFilter searchFilter = searchFilter1;
					int totalFound = 0;
					ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
					List<IdentityStoreObject> expiringNestedGroups = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, new Dictionary<string, bool>(), string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
					if ((expiringNestedGroups == null ? false : expiringNestedGroups.Count > 0))
					{
						expiringGroups.AddRange(
							from grp in expiringNestedGroups
							where !expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase))
							select grp);
					}
				}
				if ((parentDynasties == null ? false : parentDynasties.Count > 0))
				{
					List<IdentityStoreObject> allLevelCurrentChildGroups = this.EnsureChildGroups(Helper.CurrentTask.get_IdentityStoreId(), new ServicesGroupServiceClient(false), parentDynasties);
					if ((allLevelCurrentChildGroups == null ? false : allLevelCurrentChildGroups.Count > 0))
					{
						expiringGroups.AddRange(
							from grp in allLevelCurrentChildGroups
							where !expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase))
							select grp);
					}
				}
			}
		}

		private void GetExcludedNestedGroups(List<IdentityStoreObject> expiringGroups, List<string> identities)
		{
			if ((expiringGroups == null ? false : expiringGroups.Count > 0))
			{
				List<IdentityStoreObject> smartGroups = new List<IdentityStoreObject>();
				List<IdentityStoreObject> parentDynasties = new List<IdentityStoreObject>();
				this.FilterGroups(expiringGroups, smartGroups, 1);
				this.FilterGroups(expiringGroups, parentDynasties, 2);
				FilterCriteria nestGroupsFilterCriteria = this.PrepareNestChildsCriteria(smartGroups);
				if ((nestGroupsFilterCriteria.get_Child() == null ? false : nestGroupsFilterCriteria.get_Child().Count > 0))
				{
					SearchFilter searchFilter1 = new SearchFilter();
					searchFilter1.set_ExtensionDataCriteria(nestGroupsFilterCriteria);
					searchFilter1.set_ProviderCriteria(new FilterCriteria());
					SearchFilter searchFilter = searchFilter1;
					int totalFound = 0;
					ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
					List<IdentityStoreObject> expiringNestedGroups = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, new Dictionary<string, bool>(), string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
					if ((expiringNestedGroups == null ? false : expiringNestedGroups.Count > 0))
					{
						List<string> strs = new List<string>();
						expiringNestedGroups.ForEach((IdentityStoreObject grp) => {
							if (!expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase)))
							{
								strs.Add(grp.get_ObjectIdFromIdentityStore());
							}
						});
						identities.AddRange(strs);
					}
				}
				if ((parentDynasties == null ? false : parentDynasties.Count > 0))
				{
					List<IdentityStoreObject> allLevelCurrentChildGroups = this.EnsureChildGroups(Helper.CurrentTask.get_IdentityStoreId(), new ServicesGroupServiceClient(false), parentDynasties);
					if ((allLevelCurrentChildGroups == null ? false : allLevelCurrentChildGroups.Count > 0))
					{
						List<string> strs1 = new List<string>();
						allLevelCurrentChildGroups.ForEach((IdentityStoreObject grp) => {
							if (!expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase)))
							{
								strs1.Add(grp.get_ObjectIdFromIdentityStore());
							}
						});
						identities.AddRange(strs1);
					}
				}
			}
		}

		public virtual FilterCriteria GetExpiringGroupsFilter()
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
			FilterCriteria filterCriterium = new FilterCriteria();
			filterCriterium.set_Child(new List<FilterCriteria>());
			FilterCriteria filterCriteria = filterCriterium;
			filterCriteria.set_Operator("and");
			FilterCriteria filterCriterium1 = new FilterCriteria();
			filterCriterium1.set_Attribute("IMGIsExpired");
			filterCriterium1.set_Operator("is exactly");
			filterCriterium1.set_Value("false");
			filterCriterium1.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium1);
			FilterCriteria filterCriterium2 = new FilterCriteria();
			filterCriterium2.set_Attribute("IMGIsDeleted");
			filterCriterium2.set_Operator("is exactly");
			filterCriterium2.set_Value("false");
			filterCriterium2.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium2);
			FilterCriteria filterCriterium3 = new FilterCriteria();
			filterCriterium3.set_Attribute("XGroupExpirationDate");
			filterCriterium3.set_Operator("less or equal");
			DateTime date = Utility.GetCurrentDate().Date;
			filterCriterium3.set_Value(date.ToString("yyyy MMMM dd HH:mm:ss"));
			filterCriterium3.set_ValueType(4);
			filterCriteria.get_Child().Add(filterCriterium3);
			return filterCriteria;
		}

		private IdentityStoreObject GetParentGroup(List<IdentityStoreObject> expiringGroups, IdentityStoreObject group)
		{
			IdentityStoreObject parentGroup = null;
			string value = this.GetAttributeValue("IMSGObjectParentKey", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty;
			if (!string.IsNullOrEmpty(value))
			{
				parentGroup = expiringGroups.FirstOrDefault<IdentityStoreObject>((IdentityStoreObject grp) => grp.get_ObjectIdFromIdentityStore().Equals(value, StringComparison.InvariantCultureIgnoreCase));
				if (parentGroup != null)
				{
					parentGroup = this.GetParentGroup(expiringGroups, parentGroup) ?? parentGroup;
				}
			}
			return parentGroup;
		}

		protected virtual bool HasReceivedNotificationInLast7Days(IdentityStoreObject group)
		{
			bool flag;
			if ((Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport() || Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport() ? true : Helper.AppConfiguration.get_GenerateOnedayToExpiryReport()))
			{
				TimeSpan lastNotificationSpan = new TimeSpan();
				string lastSentValue = this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value();
				if (!string.IsNullOrEmpty(lastSentValue))
				{
					DateTime lastSentDate = Helper.ParseDateTime(lastSentValue);
					if (lastSentDate.Date == DateTime.MinValue.Date)
					{
						GroupsProcessor.logger.ErrorFormat("HasReceivedNotificationInLast7Days: Invalid date format {0}", lastSentValue);
						flag = false;
						return flag;
					}
					lastNotificationSpan = DateTime.Now.Subtract(lastSentDate);
				}
				flag = (lastNotificationSpan.Days <= 0 ? false : lastNotificationSpan.Days < 8);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public virtual void NotifyTheExpiringGroups(List<IdentityStoreObject> expiringGroups)
		{
			List<string> groupsToNotifyOneDay = new List<string>();
			List<string> groupsToNotifySevenDays = new List<string>();
			List<string> groupsToNotifyThirtyDays = new List<string>();
			List<IdentityStoreObject> groupsToNotifyUpdate = new List<IdentityStoreObject>();
			foreach (IdentityStoreObject group in expiringGroups)
			{
				if (group != null)
				{
					if (this.IsGroup(group))
					{
						IdentityStoreObject parentGroup = this.GetParentGroup(expiringGroups, group);
						if (parentGroup != null)
						{
							if (this.IsGroupInExcludedContainer(parentGroup))
							{
								continue;
							}
						}
						else if (this.IsGroupInExcludedContainer(group))
						{
							continue;
						}
						if (this.ShouldExpireSecurityGroup(group))
						{
							if ((this.ExtendedGroups == null || this.ExtendedGroups.Count <= 0 ? true : !this.ExtendedGroups.Contains(group.get_ObjectIdFromIdentityStore())))
							{
								int dueDays = 0;
								if (this.IsNotificationDueOneDay(group, out dueDays))
								{
									groupsToNotifyOneDay.Add(group.get_ObjectIdFromIdentityStore());
									groupsToNotifyUpdate.Add(group);
								}
								dueDays = 0;
								if (this.IsNotificationDueSevenDays(group, out dueDays))
								{
									groupsToNotifySevenDays.Add(group.get_ObjectIdFromIdentityStore());
									groupsToNotifyUpdate.Add(group);
								}
								dueDays = 0;
								if (this.IsNotificationDueThirtyDays(group, out dueDays))
								{
									groupsToNotifyThirtyDays.Add(group.get_ObjectIdFromIdentityStore());
									groupsToNotifyUpdate.Add(group);
								}
							}
						}
					}
				}
			}
			if ((groupsToNotifyOneDay.Count > 0 || groupsToNotifySevenDays.Count > 0 ? true : groupsToNotifyThirtyDays.Count > 0))
			{
				ServicesGroupServiceClient groupServiceClient = new ServicesGroupServiceClient(false);
				ActionResult result = null;
				if (groupsToNotifyOneDay.Count > 0)
				{
					result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 38, groupsToNotifyOneDay);
					this.LogResults(result, "NotifyTheExpiringGroups-OneDay-Notifications");
				}
				if (groupsToNotifySevenDays.Count > 0)
				{
					result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 39, groupsToNotifySevenDays);
					this.LogResults(result, "NotifyTheExpiringGroups-SevenDays-Notifications");
				}
				if (groupsToNotifyThirtyDays.Count > 0)
				{
					result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 40, groupsToNotifyThirtyDays);
					this.LogResults(result, "NotifyTheExpiringGroups-ThirtyDays-Notifications");
				}
				groupsToNotifyUpdate.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
				List<IdentityStoreObject> groupsToUpdate = this.CloneObjectsForUpdate(new List<string>()
				{
					"IMGLastSentExpireNotificationDate"
				}, groupsToNotifyUpdate, null);
				groupsToUpdate.ForEach((IdentityStoreObject g) => g.set_StopNotification(true));
				string compressedString = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(groupsToUpdate);
				result = groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), compressedString, typeof(IdentityStoreObject).FullName);
				this.LogResults(result, "NotifyTheExpiringGroups-AfterNotificationsUpdate");
			}
		}

		protected virtual bool PrepareGroupForLifeExtension(IdentityStoreObject grp)
		{
			bool flag;
			try
			{
				if (!Helper.AppConfiguration.get_IsGroupAttestationEnabled())
				{
					TimeSpan lastNotificationSpan = new TimeSpan();
					string lastSentValue = this.GetAttributeValue("IMGLastSentExpireNotificationDate", grp.get_AttributesBusinessObject()).get_Value();
					if (!string.IsNullOrEmpty(lastSentValue))
					{
						DateTime lastSentDate = Helper.ParseDateTime(lastSentValue);
						if (lastSentDate.Date == DateTime.MinValue.Date)
						{
							GroupsProcessor.logger.ErrorFormat("PrepareGroupForLifeExtension: Invalid date format {0}", lastSentValue);
						}
						lastNotificationSpan = DateTime.Now.Subtract(lastSentDate);
					}
					else if ((Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport() ? true : Helper.AppConfiguration.get_GenerateOnedayToExpiryReport()))
					{
						DateTime date = DateTime.Now.AddDays(7);
						date = date.Date;
						this.SetAttributeValue("XGroupExpirationDate", date.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
						flag = true;
						return flag;
					}
				}
				else
				{
					flag = false;
					return flag;
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Expiry operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, ex.Message), ex);
			}
			flag = false;
			return flag;
		}

		private FilterCriteria PrepareNestChildsCriteria(List<IdentityStoreObject> expiringGroups)
		{
			FilterCriteria filterCriterium = new FilterCriteria();
			filterCriterium.set_Child(new List<FilterCriteria>());
			FilterCriteria filterCriteria = filterCriterium;
			filterCriteria.set_Operator("or");
			foreach (IdentityStoreObject obj in expiringGroups)
			{
				FilterCriteria filterCriterium1 = new FilterCriteria();
				filterCriterium1.set_Attribute("IMSGObjectParentKey");
				filterCriterium1.set_Operator("is exactly");
				filterCriterium1.set_Value(obj.get_ObjectIdFromIdentityStore());
				filterCriterium1.set_ValueType(2);
				filterCriteria.get_Child().Add(filterCriterium1);
			}
			return filterCriteria;
		}

		public virtual void SendNotificationToExpiringGroups()
		{
			int totalFound = 0;
			ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
			FilterCriteria filterCriteria = this.GetCriteriaForExpiringNotification();
			Dictionary<string, bool> containers = null;
			if ((Helper.CurrentTask.get_Targets() == null ? false : Helper.CurrentTask.get_Targets().Count > 0))
			{
				containers = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => true);
			}
			SearchFilter searchFilter1 = new SearchFilter();
			searchFilter1.set_ExtensionDataCriteria(filterCriteria);
			searchFilter1.set_ProviderCriteria(new FilterCriteria());
			SearchFilter searchFilter = searchFilter1;
			List<IdentityStoreObject> expiringGroups = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, containers, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
			this.GetExcludedNestedGroups(expiringGroups);
			this.NotifyTheExpiringGroups(expiringGroups);
		}
	}
}