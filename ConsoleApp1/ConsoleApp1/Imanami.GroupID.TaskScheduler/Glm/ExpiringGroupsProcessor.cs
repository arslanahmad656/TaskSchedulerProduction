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
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
			foreach (IdentityStoreObject managedGroup in managedGroups)
			{
				try
				{
					List<IdentityStoreObject> allLevelCurrentChildGroups = groupClient.GetAllLevelCurrentChildGroups(identityStoreId, managedGroup.get_ObjectIdFromIdentityStore(), null, this.GetAttributesToLoad());
					List<IdentityStoreObject> identityStoreObjects1 = identityStoreObjects;
					List<IdentityStoreObject> identityStoreObjects2 = allLevelCurrentChildGroups;
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
				catch (Exception exception1)
				{
					Exception exception = exception1;
					GroupsProcessor.logger.Error(string.Concat("Error occurred in getting child groups after smart group update. ", exception.Message));
				}
			}
			return identityStoreObjects;
		}

		public virtual void ExpireTheGroups(List<IdentityStoreObject> groups)
		{
			Arguments<List<IdentityStoreObject>> argument = new Arguments<List<IdentityStoreObject>>()
			{
				Arg0 = groups
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994173L),
				Method = <>z__a_d._3
			};
			<>z__a_d.a73.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						List<string> strs = new List<string>();
						List<string> strs1 = new List<string>();
						List<IdentityStoreObject> identityStoreObjects1 = new List<IdentityStoreObject>();
						List<IdentityStoreObject> identityStoreObjects2 = DeepCopyExtensionMethods.DeepCopy<List<IdentityStoreObject>>(groups);
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
								identityStoreObjects.Add(group);
								strs1.Add(group.get_ObjectIdFromIdentityStore());
							}
							else if (this.PrepareGroupForLifeExtension(group))
							{
								GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Extending life of object {0}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()));
								identityStoreObjects.Add(group);
							}
							else if (!this.HasReceivedNotificationInLast7Days(group))
							{
								strs.Add(group.get_ObjectIdFromIdentityStore());
								identityStoreObjects1.Add(group);
							}
							else
							{
								GroupsProcessor.logger.DebugFormat("ExpireTheGroups. Object {0} has received notification within last 7 days. Not expiring.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
							}
						}
						ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
						if (identityStoreObjects.Count > 0)
						{
							List<string> strs2 = new List<string>()
							{
								"XGroupExpirationDate"
							};
							string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(this.CloneObjectsForUpdate(strs2, identityStoreObjects, identityStoreObjects2));
							ActionResult actionResult = servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str, typeof(IdentityStoreObject).FullName);
							this.LogResults(actionResult, "ExtendGroupsLife");
						}
						if (strs.Count > 0)
						{
							ActionResult actionResult1 = servicesGroupServiceClient.Expire(Helper.CurrentTask.get_IdentityStoreId(), strs);
							this.LogResults(actionResult1, "ExpireGroups");
							this.GetExcludedNestedGroups((
								from grp in groups
								where strs.Contains(grp.get_ObjectIdFromIdentityStore())
								select grp).ToList<IdentityStoreObject>(), strs);
							actionResult1 = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 28, strs);
							this.LogResults(actionResult1, "ExpireGroups-Notifications");
							identityStoreObjects1.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
							List<IdentityStoreObject> identityStoreObjects3 = this.CloneObjectsForUpdate(new List<string>()
							{
								"IMGLastSentExpireNotificationDate"
							}, identityStoreObjects1, null);
							identityStoreObjects3.ForEach((IdentityStoreObject g) => g.set_StopNotification(true));
							string str1 = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(identityStoreObjects3);
							actionResult1 = servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str1, typeof(IdentityStoreObject).FullName);
							this.LogResults(actionResult1, "UpdateExpireGroupsNotificationData");
						}
						if (strs1.Count > 0)
						{
							ActionResult actionResult2 = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 35, strs1);
							this.LogResults(actionResult2, "ExtendedGroups-Notifications");
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a73.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					<>z__a_d.a73.OnExit(methodExecutionArg);
				}
			}
		}

		public virtual void ExpireTheGroupsWhichAreDueForExpiry()
		{
			int num = 0;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994174L),
				Method = <>z__a_d._1
			};
			<>z__a_d.a72.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
						FilterCriteria expiringGroupsFilter = this.GetExpiringGroupsFilter();
						Dictionary<string, bool> dictionary = null;
						if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
						{
							dictionary = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
						}
						SearchFilter searchFilter = new SearchFilter();
						searchFilter.set_ExtensionDataCriteria(expiringGroupsFilter);
						searchFilter.set_ProviderCriteria(new FilterCriteria());
						SearchFilter searchFilter1 = searchFilter;
						List<IdentityStoreObject> identityStoreObjects = servicesSearchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, dictionary, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
						this.ExpireTheGroups(identityStoreObjects);
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a72.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					<>z__a_d.a72.OnExit(methodExecutionArg);
				}
			}
		}

		private void FilterGroups(List<IdentityStoreObject> expiringGroups, List<IdentityStoreObject> groups, int type)
		{
			expiringGroups.ForEach((IdentityStoreObject obj) => {
				ManagedGroupTypes managedGroupType;
				if (obj.get_AttributesBusinessObject() != null)
				{
					string attributeValue = this.GetAttributeValue(obj.get_AttributesBusinessObject(), "IMSGManagedGroupType");
					if (!string.IsNullOrEmpty(attributeValue))
					{
						Enum.TryParse<ManagedGroupTypes>(attributeValue, true, out managedGroupType);
						if (type == 1 && (managedGroupType == 2 || managedGroupType == 6))
						{
							groups.Add(obj);
						}
						if (type == 2 && managedGroupType == 3)
						{
							groups.Add(obj);
						}
					}
				}
			});
		}

		private string GetAttributeValue(AttributeCollection collection, string attribute)
		{
			if (collection == null || collection.get_AttributesCollection() == null || collection.get_AttributesCollection().Count <= 0 || !collection.IsIn(attribute) || collection.get_AttributesCollection()[attribute] == null || collection.get_AttributesCollection()[attribute].Count <= 0)
			{
				return string.Empty;
			}
			return collection.get_AttributesCollection()[attribute][0].get_Value();
		}

		public virtual FilterCriteria GetCriteriaForExpiringNotification()
		{
			FilterCriteria returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994160L),
				Method = <>z__a_d._b
			};
			<>z__a_d.a77.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						FilterCriteria filterCriterium = new FilterCriteria();
						filterCriterium.set_Child(new List<FilterCriteria>());
						filterCriterium.set_Operator("and");
						FilterCriteria filterCriterium1 = new FilterCriteria();
						filterCriterium1.set_Attribute("IMGIsExpired");
						filterCriterium1.set_Operator("is exactly");
						filterCriterium1.set_Value("false");
						filterCriterium1.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium1);
						FilterCriteria filterCriterium2 = new FilterCriteria();
						filterCriterium2.set_Attribute("IMGIsDeleted");
						filterCriterium2.set_Operator("is exactly");
						filterCriterium2.set_Value("false");
						filterCriterium2.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium2);
						FilterCriteria filterCriterium3 = new FilterCriteria();
						filterCriterium3.set_Attribute("XGroupExpirationDate");
						filterCriterium3.set_Operator("less or equal");
						DateTime date = DateTime.Now.AddDays(30);
						date = date.Date;
						filterCriterium3.set_Value(date.ToString("yyyy MMMM dd HH:mm:ss"));
						filterCriterium3.set_ValueType(4);
						filterCriterium.get_Child().Add(filterCriterium3);
						returnValue = filterCriterium;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a77.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_d.a77.OnExit(methodExecutionArg);
					returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		private void GetExcludedNestedGroups(List<IdentityStoreObject> expiringGroups)
		{
			if (expiringGroups != null && expiringGroups.Count > 0)
			{
				List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
				List<IdentityStoreObject> identityStoreObjects1 = new List<IdentityStoreObject>();
				this.FilterGroups(expiringGroups, identityStoreObjects, 1);
				this.FilterGroups(expiringGroups, identityStoreObjects1, 2);
				FilterCriteria filterCriterium = this.PrepareNestChildsCriteria(identityStoreObjects);
				if (filterCriterium.get_Child() != null && filterCriterium.get_Child().Count > 0)
				{
					SearchFilter searchFilter = new SearchFilter();
					searchFilter.set_ExtensionDataCriteria(filterCriterium);
					searchFilter.set_ProviderCriteria(new FilterCriteria());
					SearchFilter searchFilter1 = searchFilter;
					int num = 0;
					List<IdentityStoreObject> identityStoreObjects2 = (new ServicesSearchServiceClient(false)).SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, new Dictionary<string, bool>(), string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
					if (identityStoreObjects2 != null && identityStoreObjects2.Count > 0)
					{
						expiringGroups.AddRange(
							from grp in identityStoreObjects2
							where !expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase))
							select grp);
					}
				}
				if (identityStoreObjects1 != null && identityStoreObjects1.Count > 0)
				{
					List<IdentityStoreObject> identityStoreObjects3 = this.EnsureChildGroups(Helper.CurrentTask.get_IdentityStoreId(), new ServicesGroupServiceClient(false), identityStoreObjects1);
					if (identityStoreObjects3 != null && identityStoreObjects3.Count > 0)
					{
						expiringGroups.AddRange(
							from grp in identityStoreObjects3
							where !expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase))
							select grp);
					}
				}
			}
		}

		private void GetExcludedNestedGroups(List<IdentityStoreObject> expiringGroups, List<string> identities)
		{
			if (expiringGroups != null && expiringGroups.Count > 0)
			{
				List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
				List<IdentityStoreObject> identityStoreObjects1 = new List<IdentityStoreObject>();
				this.FilterGroups(expiringGroups, identityStoreObjects, 1);
				this.FilterGroups(expiringGroups, identityStoreObjects1, 2);
				FilterCriteria filterCriterium = this.PrepareNestChildsCriteria(identityStoreObjects);
				if (filterCriterium.get_Child() != null && filterCriterium.get_Child().Count > 0)
				{
					SearchFilter searchFilter = new SearchFilter();
					searchFilter.set_ExtensionDataCriteria(filterCriterium);
					searchFilter.set_ProviderCriteria(new FilterCriteria());
					SearchFilter searchFilter1 = searchFilter;
					int num = 0;
					List<IdentityStoreObject> identityStoreObjects2 = (new ServicesSearchServiceClient(false)).SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, new Dictionary<string, bool>(), string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
					if (identityStoreObjects2 != null && identityStoreObjects2.Count > 0)
					{
						List<string> strs = new List<string>();
						identityStoreObjects2.ForEach((IdentityStoreObject grp) => {
							if (!expiringGroups.Any<IdentityStoreObject>((IdentityStoreObject expGrp) => expGrp.get_ObjectIdFromIdentityStore().Equals(grp.get_ObjectIdFromIdentityStore(), StringComparison.InvariantCultureIgnoreCase)))
							{
								strs.Add(grp.get_ObjectIdFromIdentityStore());
							}
						});
						identities.AddRange(strs);
					}
				}
				if (identityStoreObjects1 != null && identityStoreObjects1.Count > 0)
				{
					List<IdentityStoreObject> identityStoreObjects3 = this.EnsureChildGroups(Helper.CurrentTask.get_IdentityStoreId(), new ServicesGroupServiceClient(false), identityStoreObjects1);
					if (identityStoreObjects3 != null && identityStoreObjects3.Count > 0)
					{
						List<string> strs1 = new List<string>();
						identityStoreObjects3.ForEach((IdentityStoreObject grp) => {
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
			FilterCriteria returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994170L),
				Method = <>z__a_d._5
			};
			<>z__a_d.a74.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
						FilterCriteria filterCriterium = new FilterCriteria();
						filterCriterium.set_Child(new List<FilterCriteria>());
						filterCriterium.set_Operator("and");
						FilterCriteria filterCriterium1 = new FilterCriteria();
						filterCriterium1.set_Attribute("IMGIsExpired");
						filterCriterium1.set_Operator("is exactly");
						filterCriterium1.set_Value("false");
						filterCriterium1.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium1);
						FilterCriteria filterCriterium2 = new FilterCriteria();
						filterCriterium2.set_Attribute("IMGIsDeleted");
						filterCriterium2.set_Operator("is exactly");
						filterCriterium2.set_Value("false");
						filterCriterium2.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium2);
						FilterCriteria filterCriterium3 = new FilterCriteria();
						filterCriterium3.set_Attribute("XGroupExpirationDate");
						filterCriterium3.set_Operator("less or equal");
						DateTime date = Utility.GetCurrentDate().Date;
						filterCriterium3.set_Value(date.ToString("yyyy MMMM dd HH:mm:ss"));
						filterCriterium3.set_ValueType(4);
						filterCriterium.get_Child().Add(filterCriterium3);
						returnValue = filterCriterium;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a74.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_d.a74.OnExit(methodExecutionArg);
					returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
			}
			return returnValue;
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
			if (!Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport() && !Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport() && !Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
			{
				return false;
			}
			TimeSpan timeSpan = new TimeSpan();
			string value = this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value();
			if (!string.IsNullOrEmpty(value))
			{
				DateTime dateTime = Helper.ParseDateTime(value);
				if (dateTime.Date == DateTime.MinValue.Date)
				{
					GroupsProcessor.logger.ErrorFormat("HasReceivedNotificationInLast7Days: Invalid date format {0}", value);
					return false;
				}
				timeSpan = DateTime.Now.Subtract(dateTime);
			}
			if (timeSpan.Days <= 0)
			{
				return false;
			}
			return timeSpan.Days < 8;
		}

		public virtual void NotifyTheExpiringGroups(List<IdentityStoreObject> expiringGroups)
		{
			Arguments<List<IdentityStoreObject>> argument = new Arguments<List<IdentityStoreObject>>()
			{
				Arg0 = expiringGroups
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994162L),
				Method = <>z__a_d._9
			};
			<>z__a_d.a76.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						List<string> strs = new List<string>();
						List<string> strs1 = new List<string>();
						List<string> strs2 = new List<string>();
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						foreach (IdentityStoreObject expiringGroup in expiringGroups)
						{
							if (expiringGroup == null || !this.IsGroup(expiringGroup))
							{
								continue;
							}
							IdentityStoreObject parentGroup = this.GetParentGroup(expiringGroups, expiringGroup);
							if (parentGroup != null)
							{
								if (this.IsGroupInExcludedContainer(parentGroup))
								{
									continue;
								}
							}
							else if (this.IsGroupInExcludedContainer(expiringGroup))
							{
								continue;
							}
							if (!this.ShouldExpireSecurityGroup(expiringGroup) || this.ExtendedGroups != null && this.ExtendedGroups.Count > 0 && this.ExtendedGroups.Contains(expiringGroup.get_ObjectIdFromIdentityStore()))
							{
								continue;
							}
							int num = 0;
							if (this.IsNotificationDueOneDay(expiringGroup, out num))
							{
								strs.Add(expiringGroup.get_ObjectIdFromIdentityStore());
								identityStoreObjects.Add(expiringGroup);
							}
							num = 0;
							if (this.IsNotificationDueSevenDays(expiringGroup, out num))
							{
								strs1.Add(expiringGroup.get_ObjectIdFromIdentityStore());
								identityStoreObjects.Add(expiringGroup);
							}
							num = 0;
							if (!this.IsNotificationDueThirtyDays(expiringGroup, out num))
							{
								continue;
							}
							strs2.Add(expiringGroup.get_ObjectIdFromIdentityStore());
							identityStoreObjects.Add(expiringGroup);
						}
						if (strs.Count > 0 || strs1.Count > 0 || strs2.Count > 0)
						{
							ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
							ActionResult actionResult = null;
							if (strs.Count > 0)
							{
								actionResult = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 38, strs);
								this.LogResults(actionResult, "NotifyTheExpiringGroups-OneDay-Notifications");
							}
							if (strs1.Count > 0)
							{
								actionResult = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 39, strs1);
								this.LogResults(actionResult, "NotifyTheExpiringGroups-SevenDays-Notifications");
							}
							if (strs2.Count > 0)
							{
								actionResult = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 40, strs2);
								this.LogResults(actionResult, "NotifyTheExpiringGroups-ThirtyDays-Notifications");
							}
							identityStoreObjects.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
							List<IdentityStoreObject> identityStoreObjects1 = this.CloneObjectsForUpdate(new List<string>()
							{
								"IMGLastSentExpireNotificationDate"
							}, identityStoreObjects, null);
							identityStoreObjects1.ForEach((IdentityStoreObject g) => g.set_StopNotification(true));
							string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(identityStoreObjects1);
							actionResult = servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str, typeof(IdentityStoreObject).FullName);
							this.LogResults(actionResult, "NotifyTheExpiringGroups-AfterNotificationsUpdate");
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a76.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					<>z__a_d.a76.OnExit(methodExecutionArg);
				}
			}
		}

		protected virtual bool PrepareGroupForLifeExtension(IdentityStoreObject grp)
		{
			bool flag;
			try
			{
				if (!Helper.AppConfiguration.get_IsGroupAttestationEnabled())
				{
					string value = this.GetAttributeValue("IMGLastSentExpireNotificationDate", grp.get_AttributesBusinessObject()).get_Value();
					if (!string.IsNullOrEmpty(value))
					{
						DateTime dateTime = Helper.ParseDateTime(value);
						if (dateTime.Date == DateTime.MinValue.Date)
						{
							GroupsProcessor.logger.ErrorFormat("PrepareGroupForLifeExtension: Invalid date format {0}", value);
						}
						DateTime.Now.Subtract(dateTime);
					}
					else if (Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport() || Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
					{
						DateTime date = DateTime.Now.AddDays(7);
						date = date.Date;
						this.SetAttributeValue("XGroupExpirationDate", date.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
						flag = true;
						return flag;
					}
					return false;
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Expiry operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, exception.Message), exception);
				return false;
			}
			return flag;
		}

		private FilterCriteria PrepareNestChildsCriteria(List<IdentityStoreObject> expiringGroups)
		{
			FilterCriteria filterCriterium = new FilterCriteria();
			filterCriterium.set_Child(new List<FilterCriteria>());
			FilterCriteria filterCriterium1 = filterCriterium;
			filterCriterium1.set_Operator("or");
			foreach (IdentityStoreObject expiringGroup in expiringGroups)
			{
				FilterCriteria filterCriterium2 = new FilterCriteria();
				filterCriterium2.set_Attribute("IMSGObjectParentKey");
				filterCriterium2.set_Operator("is exactly");
				filterCriterium2.set_Value(expiringGroup.get_ObjectIdFromIdentityStore());
				filterCriterium2.set_ValueType(2);
				filterCriterium1.get_Child().Add(filterCriterium2);
			}
			return filterCriterium1;
		}

		public virtual void SendNotificationToExpiringGroups()
		{
			int num = 0;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338994169L),
				Method = <>z__a_d._7
			};
			<>z__a_d.a75.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
						FilterCriteria criteriaForExpiringNotification = this.GetCriteriaForExpiringNotification();
						Dictionary<string, bool> dictionary = null;
						if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
						{
							dictionary = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => true);
						}
						SearchFilter searchFilter = new SearchFilter();
						searchFilter.set_ExtensionDataCriteria(criteriaForExpiringNotification);
						searchFilter.set_ProviderCriteria(new FilterCriteria());
						SearchFilter searchFilter1 = searchFilter;
						List<IdentityStoreObject> identityStoreObjects = servicesSearchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, dictionary, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
						this.GetExcludedNestedGroups(identityStoreObjects);
						this.NotifyTheExpiringGroups(identityStoreObjects);
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_d.a75.OnException(methodExecutionArg);
						switch (methodExecutionArg.FlowBehavior)
						{
							case FlowBehavior.Continue:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.Return:
							{
								methodExecutionArg.Exception = null;
								break;
							}
							case FlowBehavior.ThrowException:
							{
								throw methodExecutionArg.Exception;
							}
							default:
							{
								throw;
							}
						}
					}
				}
				finally
				{
					<>z__a_d.a75.OnExit(methodExecutionArg);
				}
			}
		}
	}
}