using Imanami.Common;
using Imanami.Common.DeepCopy;
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
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public class EligibleGroupsForLifeExtensionProcessor : GroupsProcessor
	{
		public HashSet<string> ExtendedGroups
		{
			get;
			set;
		}

		public EligibleGroupsForLifeExtensionProcessor()
		{
			this.ExtendedGroups = new HashSet<string>();
		}

		public virtual void ExtendEligibleGroupsLife()
		{
			ServicesSearchServiceClient searchServiceClient = new ServicesSearchServiceClient(false);
			FilterCriteria filterCriteria = this.GetEligibleGroupsFilter();
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
			List<IdentityStoreObject> groupsToExtend = searchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref totalFound, searchFilter, containers, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
			try
			{
				List<IdentityStoreObject> groupsOld = DeepCopyExtensionMethods.DeepCopy<List<IdentityStoreObject>>(groupsToExtend);
				List<string> reducedGroupsToNotify = new List<string>();
				List<IdentityStoreObject> groupsToUpdate = this.PrepareGroupsForExtensions(groupsToExtend, ref reducedGroupsToNotify);
				ServicesGroupServiceClient groupServiceClient = new ServicesGroupServiceClient(false);
				List<IdentityStoreObject> groupsToUpdate1 = this.CloneObjectsForUpdate(new List<string>()
				{
					"XGroupExpirationPolicy",
					"XGroupExpirationDate"
				}, groupsToUpdate, groupsOld);
				if ((groupsToUpdate1 == null ? false : groupsToUpdate1.Any<IdentityStoreObject>()))
				{
					string compressedString = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(groupsToUpdate1);
					ActionResult result = groupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), compressedString, typeof(IdentityStoreObject).FullName);
					this.LogResults(result, "ExtendEligibleGroupsLife");
				}
				if (reducedGroupsToNotify.Count > 0)
				{
					groupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 37, reducedGroupsToNotify);
				}
			}
			catch (Exception exception)
			{
				Exception Ex = exception;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while expiring groups: {0}", Ex.Message), Ex);
			}
		}

		public virtual FilterCriteria GetEligibleGroupsFilter()
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
			return filterCriteria;
		}

		public virtual List<IdentityStoreObject> PrepareGroupsForExtensions(List<IdentityStoreObject> groupsToExtend, ref List<string> reducedGroupsToNotify)
		{
			DateTime expirationDate;
			List<IdentityStoreObject> groupsToUpdate = new List<IdentityStoreObject>();
			foreach (IdentityStoreObject group in groupsToExtend)
			{
				try
				{
					if (!this.IsGroup(group))
					{
						GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is not a group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
						continue;
					}
					else if (this.IsSystemGroup(group))
					{
						GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is a system group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
						continue;
					}
					else if (this.IsGlmBlankGroup(group))
					{
						GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is glm blank group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
						continue;
					}
					else if (!this.ShouldExpireSecurityGroup(group))
					{
						GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is a security group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
						continue;
					}
					else if (!this.IsGroupInExcludedContainer(group))
					{
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute expirationDateDto = this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject());
						if (DateTime.TryParse(expirationDateDto.get_Value() ?? string.Empty, out expirationDate))
						{
							if (DateTime.Now.Date.Subtract(expirationDate.Date).Days > 0)
							{
							}
						}
						if (this.ReduceLifeForGUS(group))
						{
							groupsToUpdate.Add(group);
							reducedGroupsToNotify.Add(group.get_ObjectIdFromIdentityStore());
						}
						if (this.ExtendLifeForGUS(group))
						{
							groupsToUpdate.Add(group);
							this.ExtendedGroups.Add(group.get_ObjectIdFromIdentityStore());
						}
					}
					else
					{
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute policyDto = this.GetAttributeValue("XGroupExpirationPolicy", group.get_AttributesBusinessObject());
						int policy = -1;
						if (int.TryParse(policyDto.get_Value() ?? string.Empty, out policy))
						{
							if (policy != 0)
							{
								int num = 0;
								this.SetAttributeValue("XGroupExpirationPolicy", num.ToString(), group.get_AttributesBusinessObject());
								DateTime date = DateTime.MaxValue.Date;
								this.SetAttributeValue("XGroupExpirationDate", date.ToString(), group.get_AttributesBusinessObject());
								groupsToUpdate.Add(group);
							}
						}
					}
				}
				catch (Exception exception)
				{
					Exception Ex = exception;
					LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while expiring groups: {0}", Ex.Message), Ex);
				}
			}
			return groupsToUpdate;
		}

		protected virtual bool ReduceLifeForGUS(IdentityStoreObject grp)
		{
			bool flag;
			DateTime date;
			if ((!Helper.AppConfiguration.get_GUSIsLifecycleEnabled() || !Helper.AppConfiguration.get_GUSReduceGroupsLife() ? true : !grp.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Alias())))
			{
				flag = false;
			}
			else if (!Helper.AppConfiguration.get_IsGroupAttestationEnabled())
			{
				try
				{
					string lastProcessed = this.GetAttributeValue("IMGLastProcessedDate", grp.get_AttributesBusinessObject()).get_Value();
					string lastUsed = this.GetAttributeValue("IMGLastUsed", grp.get_AttributesBusinessObject()).get_Value();
					if (!string.IsNullOrEmpty(lastProcessed))
					{
						DateTime lastProcessedDate = Helper.ParseDateTime(lastProcessed);
						if (lastProcessedDate == DateTime.MinValue)
						{
							GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", lastProcessedDate);
							flag = false;
							return flag;
						}
						else if ((DateTime.Now - lastProcessedDate).Days > 30)
						{
							flag = false;
							return flag;
						}
						else
						{
							DateTime lastUsedDate = DateTime.MinValue;
							if (!string.IsNullOrEmpty(lastUsed))
							{
								lastUsedDate = Helper.ParseDateTime(lastUsed);
							}
							if (lastUsedDate == DateTime.MinValue)
							{
								string expiration = this.GetAttributeValue("XGroupExpirationDate", grp.get_AttributesBusinessObject()).get_Value();
								if (string.IsNullOrEmpty(expiration))
								{
									flag = false;
									return flag;
								}
								else
								{
									DateTime expirationdDate = Helper.ParseDateTime(expiration);
									if (expirationdDate.Date == DateTime.MinValue.Date)
									{
										GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", expirationdDate);
										flag = false;
										return flag;
									}
									else if ((expirationdDate.Date - DateTime.Now.Date).Days <= 7)
									{
										flag = false;
										return flag;
									}
									else
									{
										date = DateTime.Now.AddDays(7);
										date = date.Date;
										this.SetAttributeValue("XGroupExpirationDate", date.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
										date = DateTime.Now;
										this.SetAttributeValue("IMGLastRenewedDate", date.ToString(), grp.get_AttributesBusinessObject());
										flag = true;
										return flag;
									}
								}
							}
							else if ((DateTime.Now - lastUsedDate).Days >= Helper.AppConfiguration.get_GUSUnusedGroupsTime())
							{
								string expiration = this.GetAttributeValue("XGroupExpirationDate", grp.get_AttributesBusinessObject()).get_Value();
								if (string.IsNullOrEmpty(expiration))
								{
									flag = false;
									return flag;
								}
								else
								{
									DateTime expirationdDate = Helper.ParseDateTime(expiration);
									if (expirationdDate.Date == DateTime.MinValue.Date)
									{
										GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", expirationdDate);
										flag = false;
										return flag;
									}
									else if ((expirationdDate.Date - DateTime.Now.Date).Days <= 7)
									{
										flag = false;
										return flag;
									}
									else
									{
										date = DateTime.Now.AddDays(7);
										date = date.Date;
										this.SetAttributeValue("XGroupExpirationDate", date.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
										date = DateTime.Now;
										this.SetAttributeValue("IMGLastRenewedDate", date.ToString(), grp.get_AttributesBusinessObject());
										flag = true;
										return flag;
									}
								}
							}
						}
					}
					flag = false;
					return flag;
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Expiry operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, ex.Message), ex);
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}