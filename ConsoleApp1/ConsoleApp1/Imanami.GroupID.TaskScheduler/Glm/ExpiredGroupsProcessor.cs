using Imanami.Common;
using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Filter;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.TaskScheduler;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public class ExpiredGroupsProcessor : GroupsProcessor
	{
		public ExpiredGroupsProcessor()
		{
		}

		public virtual void DeleteExpiredGroups(List<IdentityStoreObject> groups)
		{
			LogExtension.EnterMethod(GroupsProcessor.logger, MethodBase.GetCurrentMethod(), new object[0]);
			List<string> groupsToNotify = new List<string>();
			List<IdentityStoreObject> groupsToDelete = new List<IdentityStoreObject>();
			try
			{
				foreach (IdentityStoreObject group in groups)
				{
					try
					{
						if (!this.IsGroup(group))
						{
							GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is not a group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
							continue;
						}
						else if (this.IsSystemGroup(group))
						{
							GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is a system group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
							continue;
						}
						else if (this.IsGroupInExcludedContainer(group))
						{
							GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is in excluded containers.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
							continue;
						}
						else if (this.ShouldExpireSecurityGroup(group))
						{
							string expirationDateString = this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value();
							DateTime expirationDate = Helper.ParseDateTime(expirationDateString);
							if (expirationDate.Date == DateTime.MinValue.Date)
							{
								continue;
							}
							else if (DateTime.Now.Date.Subtract(expirationDate.Date).Days >= Helper.AppConfiguration.get_DeletionDaysAfterExpiry())
							{
								this.SetAttributeValue("IMGIsDeleted", "true", group.get_AttributesBusinessObject());
								string updatedDisplayName = this.GetUpdatedDisplayName(group);
								group.set_ObjectDisplayName(updatedDisplayName);
								this.SetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), updatedDisplayName, group.get_AttributesBusinessObject());
								groupsToNotify.Add(group.get_ObjectIdFromIdentityStore());
								groupsToDelete.Add(group);
								GroupsProcessor.logger.InfoFormat("Group Deleted: {0}", group.get_ObjectDisplayName());
							}
							else
							{
								continue;
							}
						}
						else
						{
							GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is a security group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
							continue;
						}
					}
					catch (Exception exception)
					{
						Exception ex = exception;
						GroupsProcessor.logger.Error(string.Format("An Error occured while performing deletion operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, ex.Message), ex);
					}
				}
				ServicesGroupServiceClient groupServiceClient = new ServicesGroupServiceClient(false);
				if (groupsToDelete.Count > 0)
				{
					groupsToDelete.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
					groupsToDelete.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGIsDeleted", "TRUE", g.get_AttributesBusinessObject()));
					List<IdentityStoreObject> groupsToUpdate = this.CloneObjectsForUpdate(new List<string>()
					{
						"IMGLastSentExpireNotificationDate",
						"IMGIsDeleted",
						Helper.KnownProviderAttributes.get_DisplayName()
					}, groupsToDelete, null);
					string compressedString = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(groupsToUpdate);
					ActionResult result = groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), compressedString, typeof(IdentityStoreObject).FullName);
					this.LogResults(result, "DeleteExpiredGroups");
				}
				if (groupsToNotify.Count > 0)
				{
					ActionResult result = groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 29, groupsToNotify);
					this.LogResults(result, "DeleteExpiredGroups-Notifications");
				}
				LogExtension.ExitMethod(GroupsProcessor.logger, MethodBase.GetCurrentMethod(), new object[0]);
			}
			catch (Exception exception1)
			{
				Exception Ex = exception1;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while deleting 30 days old expired groups: {0}", Ex.Message), Ex);
			}
		}

		public virtual void DeleteTheExpiredGroupsWhichAreDueForDeletion()
		{
			if (Helper.AppConfiguration.get_ShouldDeleteExpiredGroups())
			{
				if (Helper.AppConfiguration.get_DeletionDaysAfterExpiry() >= 1)
				{
					ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
					FilterCriteria filterCriteria = this.GetExpiredGroupsFilter();
					int totalFound = 0;
					Dictionary<string, bool> containers = null;
					if ((Helper.CurrentTask.get_Targets() == null ? false : Helper.CurrentTask.get_Targets().Count > 0))
					{
						containers = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
					}
					SearchFilter searchFilter1 = new SearchFilter();
					searchFilter1.set_ExtensionDataCriteria(filterCriteria);
					searchFilter1.set_ProviderCriteria(new FilterCriteria());
					SearchFilter searchFilter = searchFilter1;
					List<IdentityStoreObject> groupsToDelete = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, containers, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
					this.DeleteExpiredGroups(groupsToDelete);
				}
			}
		}

		public virtual FilterCriteria GetExpiredGroupsFilter()
		{
			FilterCriteria filterCriterium = new FilterCriteria();
			filterCriterium.set_Child(new List<FilterCriteria>());
			FilterCriteria filterCriteria = filterCriterium;
			filterCriteria.set_Operator("and");
			FilterCriteria filterCriterium1 = new FilterCriteria();
			filterCriterium1.set_Attribute("IMGIsExpired");
			filterCriterium1.set_Operator("is exactly");
			filterCriterium1.set_Value("true");
			filterCriterium1.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium1);
			FilterCriteria filterCriterium2 = new FilterCriteria();
			filterCriterium2.set_Attribute("IMGIsDeleted");
			filterCriterium2.set_Operator("is exactly");
			filterCriterium2.set_Value("false");
			filterCriterium2.set_ValueType(5);
			filterCriteria.get_Child().Add(filterCriterium2);
			DateTime date = Utility.GetCurrentDate().Date;
			DateTime deletionSpan = date.AddDays((double)(Helper.AppConfiguration.get_DeletionDaysAfterExpiry() * -1));
			FilterCriteria filterCriterium3 = new FilterCriteria();
			filterCriterium3.set_Attribute("XGroupExpirationDate");
			filterCriterium3.set_Operator("less or equal");
			filterCriterium3.set_Value(deletionSpan.ToString("yyyy MMMM dd HH:mm:ss"));
			filterCriterium3.set_ValueType(4);
			filterCriteria.get_Child().Add(filterCriterium3);
			return filterCriteria;
		}

		public virtual string GetUpdatedDisplayName(IdentityStoreObject group)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute displayName = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), group.get_AttributesBusinessObject());
			if (StringUtility.IsBlank(displayName.get_Value()))
			{
				displayName = this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject());
			}
			displayName.set_Value(displayName.get_Value() ?? string.Empty);
			string updatedDisplayName = displayName.get_Value();
			if (updatedDisplayName.StartsWith("Expired_"))
			{
				updatedDisplayName = updatedDisplayName.Remove(0, "Expired_".Length);
			}
			if (!updatedDisplayName.StartsWith("Deleted_"))
			{
				updatedDisplayName = string.Concat("Deleted_", updatedDisplayName);
			}
			return updatedDisplayName;
		}
	}
}