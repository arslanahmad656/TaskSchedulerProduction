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
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339125245L),
				Method = <>z__a_b._1
			};
			<>z__a_b.a63.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
						FilterCriteria eligibleGroupsFilter = this.GetEligibleGroupsFilter();
						int num = 0;
						Dictionary<string, bool> dictionary = null;
						if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
						{
							dictionary = Helper.CurrentTask.get_Targets().ToDictionary<SchedulingTarget, string, bool>((SchedulingTarget target) => target.get_Target(), (SchedulingTarget target) => false);
						}
						SearchFilter searchFilter = new SearchFilter();
						searchFilter.set_ExtensionDataCriteria(eligibleGroupsFilter);
						searchFilter.set_ProviderCriteria(new FilterCriteria());
						SearchFilter searchFilter1 = searchFilter;
						List<IdentityStoreObject> identityStoreObjects = servicesSearchServiceClient.SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, dictionary, string.Empty, 1, -1, 20000, this.GetAttributesToLoad(), false);
						try
						{
							List<IdentityStoreObject> identityStoreObjects1 = DeepCopyExtensionMethods.DeepCopy<List<IdentityStoreObject>>(identityStoreObjects);
							List<string> strs = new List<string>();
							List<IdentityStoreObject> identityStoreObjects2 = this.PrepareGroupsForExtensions(identityStoreObjects, ref strs);
							ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
							List<IdentityStoreObject> identityStoreObjects3 = this.CloneObjectsForUpdate(new List<string>()
							{
								"XGroupExpirationPolicy",
								"XGroupExpirationDate"
							}, identityStoreObjects2, identityStoreObjects1);
							if (identityStoreObjects3 != null && identityStoreObjects3.Any<IdentityStoreObject>())
							{
								string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(identityStoreObjects3);
								ActionResult actionResult = servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str, typeof(IdentityStoreObject).FullName);
								this.LogResults(actionResult, "ExtendEligibleGroupsLife");
							}
							if (strs.Count > 0)
							{
								servicesGroupServiceClient.SendGlmNotification(Helper.CurrentTask.get_IdentityStoreId(), 37, strs);
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while expiring groups: {0}", exception.Message), exception);
						}
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						<>z__a_b.a63.OnException(methodExecutionArg);
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
					<>z__a_b.a63.OnExit(methodExecutionArg);
				}
			}
		}

		public virtual FilterCriteria GetEligibleGroupsFilter()
		{
			FilterCriteria returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339125242L),
				Method = <>z__a_b._5
			};
			<>z__a_b.a65.OnEntry(methodExecutionArg);
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
						returnValue = filterCriterium;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_b.a65.OnException(methodExecutionArg);
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
					<>z__a_b.a65.OnExit(methodExecutionArg);
					returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (FilterCriteria)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual List<IdentityStoreObject> PrepareGroupsForExtensions(List<IdentityStoreObject> groupsToExtend, ref List<string> reducedGroupsToNotify)
		{
			DateTime date;
			DateTime dateTime;
			List<IdentityStoreObject> returnValue = null;
			Arguments<List<IdentityStoreObject>, List<string>> argument = new Arguments<List<IdentityStoreObject>, List<string>>()
			{
				Arg0 = groupsToExtend,
				Arg1 = reducedGroupsToNotify
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339125244L),
				Method = <>z__a_b._3
			};
			<>z__a_b.a64.OnEntry(methodExecutionArg);
			reducedGroupsToNotify = argument.Arg1;
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						foreach (IdentityStoreObject identityStoreObject in groupsToExtend)
						{
							try
							{
								if (!this.IsGroup(identityStoreObject))
								{
									GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is not a group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
								}
								else if (this.IsSystemGroup(identityStoreObject))
								{
									GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is a system group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
								}
								else if (this.IsGlmBlankGroup(identityStoreObject))
								{
									GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is glm blank group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
								}
								else if (!this.ShouldExpireSecurityGroup(identityStoreObject))
								{
									GroupsProcessor.logger.DebugFormat("PrepareGroupsForExtensions. Object {0} is a security group.", this.GetAttributeValue(Helper.KnownProviderAttributes.get_Name(), identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty);
								}
								else if (!this.IsGroupInExcludedContainer(identityStoreObject))
								{
									if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime))
									{
										date = DateTime.Now.Date;
										TimeSpan timeSpan = date.Subtract(dateTime.Date);
										int days = timeSpan.Days;
									}
									if (this.ReduceLifeForGUS(identityStoreObject))
									{
										identityStoreObjects.Add(identityStoreObject);
										reducedGroupsToNotify.Add(identityStoreObject.get_ObjectIdFromIdentityStore());
									}
									if (this.ExtendLifeForGUS(identityStoreObject))
									{
										identityStoreObjects.Add(identityStoreObject);
										this.ExtendedGroups.Add(identityStoreObject.get_ObjectIdFromIdentityStore());
									}
								}
								else
								{
									int num = -1;
									if (int.TryParse(this.GetAttributeValue("XGroupExpirationPolicy", identityStoreObject.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out num) && num != 0)
									{
										int num1 = 0;
										this.SetAttributeValue("XGroupExpirationPolicy", num1.ToString(), identityStoreObject.get_AttributesBusinessObject());
										date = DateTime.MaxValue.Date;
										this.SetAttributeValue("XGroupExpirationDate", date.ToString(), identityStoreObject.get_AttributesBusinessObject());
										identityStoreObjects.Add(identityStoreObject);
									}
								}
							}
							catch (Exception exception1)
							{
								Exception exception = exception1;
								LogExtension.LogException(GroupsProcessor.logger, string.Format("An error ocurred while expiring groups: {0}", exception.Message), exception);
							}
						}
						returnValue = identityStoreObjects;
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						argument.Arg1 = reducedGroupsToNotify;
						<>z__a_b.a64.OnException(methodExecutionArg);
						reducedGroupsToNotify = argument.Arg1;
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
								returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
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
					argument.Arg1 = reducedGroupsToNotify;
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_b.a64.OnExit(methodExecutionArg);
					reducedGroupsToNotify = argument.Arg1;
					returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		protected virtual bool ReduceLifeForGUS(IdentityStoreObject grp)
		{
			bool flag;
			DateTime date;
			if (!Helper.AppConfiguration.get_GUSIsLifecycleEnabled() || !Helper.AppConfiguration.get_GUSReduceGroupsLife() || !grp.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Alias()))
			{
				return false;
			}
			if (Helper.AppConfiguration.get_IsGroupAttestationEnabled())
			{
				return false;
			}
			try
			{
				string value = this.GetAttributeValue("IMGLastProcessedDate", grp.get_AttributesBusinessObject()).get_Value();
				string str = this.GetAttributeValue("IMGLastUsed", grp.get_AttributesBusinessObject()).get_Value();
				if (!string.IsNullOrEmpty(value))
				{
					DateTime dateTime = Helper.ParseDateTime(value);
					if (dateTime == DateTime.MinValue)
					{
						GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", dateTime);
						flag = false;
						return flag;
					}
					else if ((DateTime.Now - dateTime).Days > 30)
					{
						flag = false;
						return flag;
					}
					else
					{
						DateTime minValue = DateTime.MinValue;
						if (!string.IsNullOrEmpty(str))
						{
							minValue = Helper.ParseDateTime(str);
						}
						if (minValue == DateTime.MinValue)
						{
							string value1 = this.GetAttributeValue("XGroupExpirationDate", grp.get_AttributesBusinessObject()).get_Value();
							if (string.IsNullOrEmpty(value1))
							{
								flag = false;
								return flag;
							}
							else
							{
								DateTime dateTime1 = Helper.ParseDateTime(value1);
								if (dateTime1.Date == DateTime.MinValue.Date)
								{
									GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", dateTime1);
									flag = false;
									return flag;
								}
								else if ((dateTime1.Date - DateTime.Now.Date).Days <= 7)
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
						else if ((DateTime.Now - minValue).Days >= Helper.AppConfiguration.get_GUSUnusedGroupsTime())
						{
							string str1 = this.GetAttributeValue("XGroupExpirationDate", grp.get_AttributesBusinessObject()).get_Value();
							if (string.IsNullOrEmpty(str1))
							{
								flag = false;
								return flag;
							}
							else
							{
								DateTime dateTime2 = Helper.ParseDateTime(str1);
								if (dateTime2.Date == DateTime.MinValue.Date)
								{
									GroupsProcessor.logger.ErrorFormat("ReduceLifeForGUS: Invalid date format {0}", dateTime2);
									flag = false;
									return flag;
								}
								else if ((dateTime2.Date - DateTime.Now.Date).Days <= 7)
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
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Expiry operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, exception.Message), exception);
				return false;
			}
			return flag;
		}
	}
}