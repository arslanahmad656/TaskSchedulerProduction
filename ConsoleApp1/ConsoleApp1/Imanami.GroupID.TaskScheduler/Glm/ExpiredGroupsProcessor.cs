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
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
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
			Arguments<List<IdentityStoreObject>> argument = new Arguments<List<IdentityStoreObject>>()
			{
				Arg0 = groups
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339059711L),
				Method = <>z__a_c._3
			};
			<>z__a_c.a67.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						LogExtension.EnterMethod(GroupsProcessor.logger, MethodBase.GetCurrentMethod(), new object[0]);
						List<string> strs = new List<string>();
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						try
						{
							foreach (IdentityStoreObject group in groups)
							{
								try
								{
									if (!this.IsGroup(group))
									{
										GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is not a group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
									}
									else if (this.IsSystemGroup(group))
									{
										GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is a system group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
									}
									else if (this.IsGroupInExcludedContainer(group))
									{
										GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is in excluded containers.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
									}
									else if (this.ShouldExpireSecurityGroup(group))
									{
										DateTime dateTime = Helper.ParseDateTime(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value());
										if (dateTime.Date != DateTime.MinValue.Date)
										{
											if (DateTime.Now.Date.Subtract(dateTime.Date).Days >= Helper.AppConfiguration.get_DeletionDaysAfterExpiry())
											{
												this.SetAttributeValue("IMGIsDeleted", "true", group.get_AttributesBusinessObject());
												string updatedDisplayName = this.GetUpdatedDisplayName(group);
												group.set_ObjectDisplayName(updatedDisplayName);
												this.SetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), updatedDisplayName, group.get_AttributesBusinessObject());
												strs.Add(group.get_ObjectIdFromIdentityStore());
												identityStoreObjects.Add(group);
												GroupsProcessor.logger.InfoFormat("Group Deleted: {0}", group.get_ObjectDisplayName());
											}
										}
									}
									else
									{
										GroupsProcessor.logger.DebugFormat("DeleteExpiredGroups. Object {0} is a security group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
									}
								}
								catch (Exception exception1)
								{
									Exception exception = exception1;
									GroupsProcessor.logger.Error(string.Format("An Error occured while performing deletion operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, exception.Message), exception);
								}
							}
							ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
							if (identityStoreObjects.Count > 0)
							{
								identityStoreObjects.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGLastSentExpireNotificationDate", DateTime.Now.Date.ToString("yyyy MMMM dd HH:mm:ss"), g.get_AttributesBusinessObject()));
								identityStoreObjects.ForEach((IdentityStoreObject g) => this.SetAttributeValue("IMGIsDeleted", "TRUE", g.get_AttributesBusinessObject()));
								List<string> strs1 = new List<string>()
								{
									"IMGLastSentExpireNotificationDate",
									"IMGIsDeleted",
									Helper.KnownProviderAttributes.get_DisplayName()
								};
								string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(this.CloneObjectsForUpdate(strs1, identityStoreObjects, null));
								ActionResult actionResult = servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str, typeof(IdentityStoreObject).FullName);
								this.LogResults(actionResult, "DeleteExpiredGroups");
							}
							if (strs.Count > 0)
							{
								ActionResult actionResult1 = servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 29, strs);
								this.LogResults(actionResult1, "DeleteExpiredGroups-Notifications");
							}
							LogExtension.ExitMethod(GroupsProcessor.logger, MethodBase.GetCurrentMethod(), new object[0]);
						}
						catch (Exception exception3)
						{
							Exception exception2 = exception3;
							LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while deleting 30 days old expired groups: {0}", exception2.Message), exception2);
						}
					}
					catch (Exception exception4)
					{
						methodExecutionArg.Exception = exception4;
						<>z__a_c.a67.OnException(methodExecutionArg);
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
					<>z__a_c.a67.OnExit(methodExecutionArg);
				}
			}
		}

		public virtual void DeleteTheExpiredGroupsWhichAreDueForDeletion()
		{
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339059712L),
				Method = <>z__a_c._1
			};
			<>z__a_c.a66.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (Helper.AppConfiguration.get_ShouldDeleteExpiredGroups())
						{
							if (Helper.AppConfiguration.get_DeletionDaysAfterExpiry() >= 1)
							{
								ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
								FilterCriteria expiredGroupsFilter = this.GetExpiredGroupsFilter();
								int num = 0;
								Dictionary<string, bool> dictionary = null;
								if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
								{
									dictionary = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
								}
								SearchFilter searchFilter = new SearchFilter();
								searchFilter.set_ExtensionDataCriteria(expiredGroupsFilter);
								searchFilter.set_ProviderCriteria(new FilterCriteria());
								SearchFilter searchFilter1 = searchFilter;
								List<IdentityStoreObject> identityStoreObjects = servicesSearchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, dictionary, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
								this.DeleteExpiredGroups(identityStoreObjects);
							}
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_c.a66.OnException(methodExecutionArg);
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
					<>z__a_c.a66.OnExit(methodExecutionArg);
				}
			}
		}

		public virtual FilterCriteria GetExpiredGroupsFilter()
		{
			FilterCriteria returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339059709L),
				Method = <>z__a_c._7
			};
			<>z__a_c.a69.OnEntry(methodExecutionArg);
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
						filterCriterium1.set_Value("true");
						filterCriterium1.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium1);
						FilterCriteria filterCriterium2 = new FilterCriteria();
						filterCriterium2.set_Attribute("IMGIsDeleted");
						filterCriterium2.set_Operator("is exactly");
						filterCriterium2.set_Value("false");
						filterCriterium2.set_ValueType(5);
						filterCriterium.get_Child().Add(filterCriterium2);
						DateTime date = Utility.GetCurrentDate().Date;
						DateTime dateTime = date.AddDays((double)(Helper.AppConfiguration.get_DeletionDaysAfterExpiry() * -1));
						FilterCriteria filterCriterium3 = new FilterCriteria();
						filterCriterium3.set_Attribute("XGroupExpirationDate");
						filterCriterium3.set_Operator("less or equal");
						filterCriterium3.set_Value(dateTime.ToString("yyyy MMMM dd HH:mm:ss"));
						filterCriterium3.set_ValueType(4);
						filterCriterium.get_Child().Add(filterCriterium3);
						returnValue = filterCriterium;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_c.a69.OnException(methodExecutionArg);
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
					<>z__a_c.a69.OnExit(methodExecutionArg);
					returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual string GetUpdatedDisplayName(IdentityStoreObject group)
		{
			string returnValue = null;
			Arguments<IdentityStoreObject> argument = new Arguments<IdentityStoreObject>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339059710L),
				Method = <>z__a_c._5
			};
			<>z__a_c.a68.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attributeValue = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), group.get_AttributesBusinessObject());
						if (StringUtility.IsBlank(attributeValue.get_Value()))
						{
							attributeValue = this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), group.get_AttributesBusinessObject());
						}
						attributeValue.set_Value(attributeValue.get_Value() ?? string.Empty);
						string value = attributeValue.get_Value();
						if (value.StartsWith("Expired_"))
						{
							value = value.Remove(0, "Expired_".Length);
						}
						if (!value.StartsWith("Deleted_"))
						{
							value = string.Concat("Deleted_", value);
						}
						returnValue = value;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_c.a68.OnException(methodExecutionArg);
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
								returnValue = (string)methodExecutionArg.ReturnValue;
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
					<>z__a_c.a68.OnExit(methodExecutionArg);
					returnValue = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (string)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}
	}
}