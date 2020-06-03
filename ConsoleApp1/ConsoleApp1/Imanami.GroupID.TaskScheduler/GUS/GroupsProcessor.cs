using Imanami.Common;
using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Filter;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.GUS;
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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler.GUS
{
	public class GroupsProcessor
	{
		private static ILog logger;

		static GroupsProcessor()
		{
			<>z__a_8.Initialize();
			GroupsProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public GroupsProcessor()
		{
		}

		private FilterCriteria GetEligibleGroupsFilter()
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
			return filterCriterium;
		}

		public List<IdentityStoreObject> GetGroups(string container)
		{
			int num = 0;
			List<IdentityStoreObject> returnValue = null;
			Arguments<string> argument = new Arguments<string>()
			{
				Arg0 = container
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339256318L),
				Method = <>z__a_8._3
			};
			<>z__a_8.a40.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						SearchFilter searchFilter = new SearchFilter();
						searchFilter.set_ExtensionDataCriteria(new FilterCriteria());
						FilterCriteria filterCriterium = new FilterCriteria();
						filterCriterium.set_Attribute(Helper.KnownProviderAttributes.get_Alias());
						filterCriterium.set_Operator("present");
						searchFilter.set_ProviderCriteria(filterCriterium);
						SearchFilter searchFilter1 = searchFilter;
						Dictionary<string, bool> strs = null;
						if (!string.IsNullOrEmpty(container))
						{
							strs = new Dictionary<string, bool>()
							{
								{ container, false }
							};
						}
						returnValue = (new ServicesSearchServiceClient(false)).SearchEx(Helper.CurrentTask.get_IdentityStoreId(), 2, ref num, searchFilter1, strs, string.Empty, 1, -1, 20000, new List<string>()
						{
							Helper.KnownProviderAttributes.get_EmailAddress(),
							"IMGFirstUsed",
							"IMGLastUsed",
							"IMGUsedCount",
							"IMGLastProcessedDate"
						}, false);
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_8.a40.OnException(methodExecutionArg);
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
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_8.a40.OnExit(methodExecutionArg);
					returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		private List<MessagingProviderLog> GetGroupUsage(DateTime? fromDate, DateTime toDate)
		{
			ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
			List<MessagingProviderLog> messagingProviderLogs = new List<MessagingProviderLog>();
			bool? includeAllMessageSystems = Helper.CurrentTask.get_IncludeAllMessageSystems();
			if ((includeAllMessageSystems.HasValue ? includeAllMessageSystems.GetValueOrDefault() : false))
			{
				messagingProviderLogs = servicesSearchServiceClient.GetMessagingProviderLog(Helper.CurrentTask.get_IdentityStoreId(), new List<string>(), true, fromDate, toDate);
			}
			else if (Helper.CurrentTask.get_MessagingSystems() != null && Helper.CurrentTask.get_MessagingSystems().Count > 0)
			{
				List<string> strs = new List<string>();
				Helper.CurrentTask.get_MessagingSystems().ForEach((SchedulingMessagingSystems server) => strs.Add(server.get_DisplayName()));
				messagingProviderLogs = servicesSearchServiceClient.GetMessagingProviderLog(Helper.CurrentTask.get_IdentityStoreId(), strs, false, fromDate, toDate);
			}
			return messagingProviderLogs;
		}

		private bool ProcessContainer(string container, DateTime? fromDate, DateTime toDate)
		{
			bool flag;
			object obj;
			try
			{
				List<MessagingProviderLog> groupUsage = this.GetGroupUsage(fromDate, toDate);
				ILog log = GroupsProcessor.logger;
				int count = groupUsage.Count;
				log.InfoFormat("Populated log from messaging server(s):{0}", count.ToString());
				if (groupUsage.Count <= 0)
				{
					flag = false;
				}
				else
				{
					List<IdentityStoreObject> groups = this.GetGroups(container);
					ILog log1 = GroupsProcessor.logger;
					obj = (string.IsNullOrEmpty(container) ? " all identity store" : container);
					count = groups.Count;
					log1.InfoFormat("Populated groups from {0}:{1}", obj, count.ToString());
					groups = this.SetGroupUsage(groups, groupUsage, fromDate, toDate);
					GroupsProcessor.logger.InfoFormat("Group usage has been processed for selected container", Array.Empty<object>());
					if (this.SaveGroupUsage(groups, toDate).get_Status() != null)
					{
						GroupsProcessor.logger.InfoFormat("Group usage has not been saved for selected container", Array.Empty<object>());
						flag = false;
					}
					else
					{
						GroupsProcessor.logger.InfoFormat("Group usage has been saved for selected container", Array.Empty<object>());
						flag = true;
					}
				}
			}
			catch (Exception exception)
			{
				GroupsProcessor.logger.ErrorFormat(exception.Message, Array.Empty<object>());
				flag = false;
			}
			return flag;
		}

		public ActionResult ProcessGroupUsage()
		{
			ActionResult returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339256320L),
				Method = <>z__a_8._1
			};
			<>z__a_8.a39.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						GroupsProcessor.logger.InfoFormat("Processing groups usage", Array.Empty<object>());
						ActionResult actionResult = new ActionResult();
						actionResult.set_Status(0);
						ActionResult actionResult1 = actionResult;
						try
						{
							ActionResult actionResult2 = this.ProcessTask();
							GroupsProcessor.logger.InfoFormat("Group usage stamped", Array.Empty<object>());
							if (actionResult2.get_Status() == null)
							{
								(new ServicesSchedulingServiceClient(false)).Update(Helper.CurrentTask);
								GroupsProcessor.logger.InfoFormat("Saved group usage", Array.Empty<object>());
							}
							GroupsProcessor.logger.InfoFormat("Processed groups usage", Array.Empty<object>());
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							actionResult1.set_Status(2);
							GroupsProcessor.logger.ErrorFormat(exception.Message, Array.Empty<object>());
						}
						returnValue = actionResult1;
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						<>z__a_8.a39.OnException(methodExecutionArg);
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
								returnValue = (ActionResult)methodExecutionArg.ReturnValue;
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
					<>z__a_8.a39.OnExit(methodExecutionArg);
					returnValue = (ActionResult)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (ActionResult)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		private ActionResult ProcessTask()
		{
			DateTime now = DateTime.Now;
			ActionResult actionResult = new ActionResult();
			actionResult.set_Status(0);
			ActionResult actionResult1 = actionResult;
			try
			{
				bool? includeAllContainers = Helper.CurrentTask.get_IncludeAllContainers();
				if ((includeAllContainers.HasValue ? includeAllContainers.GetValueOrDefault() : false))
				{
					try
					{
						GroupsProcessor.logger.InfoFormat("Processing roup usage for all containers", Array.Empty<object>());
						if (this.ProcessContainer(null, Helper.CurrentTask.get_LastProcessedTime(), now))
						{
							Helper.CurrentTask.set_LastProcessedTime(new DateTime?(now));
						}
					}
					catch (Exception exception3)
					{
						Exception exception2 = exception3;
						GroupsProcessor.logger.ErrorFormat(string.Format("An error ocurred while processing group usage for all containers: {0}", exception2.Message), exception2);
					}
				}
				else if (Helper.CurrentTask.get_Targets() != null)
				{
					Helper.CurrentTask.get_Targets().ForEach((SchedulingTarget container) => {
						DateTime? lastProcessedTime;
						try
						{
							DateTime value = DateTime.Now.AddDays(-29);
							if (container.get_LastProcessedTime().HasValue)
							{
								value = container.get_LastProcessedTime().Value;
							}
							else if (Helper.CurrentTask.get_LastProcessedTime().HasValue)
							{
								lastProcessedTime = Helper.CurrentTask.get_LastProcessedTime();
								value = lastProcessedTime.Value;
							}
							else if (Helper.CurrentTask.get_LastRunTime().HasValue)
							{
								lastProcessedTime = Helper.CurrentTask.get_LastRunTime();
								value = lastProcessedTime.Value;
							}
							GroupsProcessor.logger.InfoFormat("Processing group usage for {0} container", container.get_Target());
							if (this.ProcessContainer(container.get_Target(), new DateTime?(value), now))
							{
								container.set_LastProcessedTime(new DateTime?(now));
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							GroupsProcessor.logger.ErrorFormat(string.Format("An error ocurred while processing group usage for {0} container: {1}", container, exception.Message), exception);
						}
					});
					Helper.CurrentTask.set_LastProcessedTime(new DateTime?(now));
				}
			}
			catch (Exception exception4)
			{
				GroupsProcessor.logger.ErrorFormat(exception4.Message, Array.Empty<object>());
				actionResult1.set_Status(2);
			}
			return actionResult1;
		}

		private ActionResult SaveGroupUsage(List<IdentityStoreObject> providerResult, DateTime toDate)
		{
			providerResult.ForEach((IdentityStoreObject identityStoreObject) => {
				DateTime dateTime;
				identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection().Keys.ToList<string>().ForEach((string key) => {
					if (!key.Equals("IMGFirstUsed", StringComparison.InvariantCultureIgnoreCase) && !key.Equals("IMGLastUsed", StringComparison.InvariantCultureIgnoreCase) && !key.Equals("IMGUsedCount", StringComparison.InvariantCultureIgnoreCase) && !key.Equals("IMGLastProcessedDate", StringComparison.InvariantCultureIgnoreCase))
					{
						identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection().Remove(key);
						return;
					}
					if (!identityStoreObject.get_AttributesBusinessObject().HasValue(key) && !key.Equals("IMGLastProcessedDate", StringComparison.InvariantCultureIgnoreCase))
					{
						identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection().Remove(key);
					}
				});
				if (identityStoreObject.get_AttributesBusinessObject().HasValue("IMGLastProcessedDate"))
				{
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute item = identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection()["IMGLastProcessedDate"][0];
					dateTime = Convert.ToDateTime(toDate);
					item.set_Value(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
					identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection()["IMGLastProcessedDate"][0].set_Action(3);
				}
				else if (!identityStoreObject.get_AttributesBusinessObject().IsIn("IMGLastProcessedDate"))
				{
					Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection();
					List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute.set_Action(1);
					dateTime = Convert.ToDateTime(toDate);
					attribute.set_Value(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
					attributes.Add(attribute);
					attributesCollection.Add("IMGLastProcessedDate", attributes);
				}
				else
				{
					List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item1 = identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection()["IMGLastProcessedDate"];
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute1.set_Action(1);
					dateTime = Convert.ToDateTime(toDate);
					attribute1.set_Value(dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
					item1.Add(attribute1);
				}
				GroupsProcessor.logger.InfoFormat("Stamping {0} with last processed date {1} with action {2}", identityStoreObject.get_ObjectIdFromIdentityStore(), identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection()["IMGLastProcessedDate"][0].get_Value(), identityStoreObject.get_AttributesBusinessObject().get_AttributesCollection()["IMGLastProcessedDate"][0].get_Action().ToString());
			});
			ServicesGroupServiceClient servicesGroupServiceClient = new ServicesGroupServiceClient(false);
			string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(providerResult);
			return servicesGroupServiceClient.UpdateManyWithCompression(Helper.CurrentTask.get_IdentityStoreId(), str, typeof(IdentityStoreObject).FullName);
		}

		private List<IdentityStoreObject> SetGroupUsage(List<IdentityStoreObject> providerResult, List<MessagingProviderLog> messagingProvidersLogs, DateTime? fromDate, DateTime toDate)
		{
			providerResult.ForEach((IdentityStoreObject x) => x.set_StopNotification(true));
			messagingProvidersLogs.ForEach((MessagingProviderLog messagingProviderLog) => {
				try
				{
					if (messagingProviderLog != null && messagingProviderLog.get_GroupsLog() != null)
					{
						GroupsProcessor.logger.InfoFormat("Processing log of server {0} of groups {1}", messagingProviderLog.get_ServerIdentity(), messagingProviderLog.get_GroupsLog().Count.ToString());
						messagingProviderLog.get_GroupsLog().ForEach((GroupUsage groupLog) => {
							DateTime dateTime;
							try
							{
								ILog log = GroupsProcessor.logger;
								string groupIdentity = groupLog.get_GroupIdentity();
								int count = groupLog.get_Usage().Count;
								log.InfoFormat("Processing group {0} with usage found {1}", groupIdentity, count.ToString());
								List<IdentityStoreObject> list = providerResult.Where<IdentityStoreObject>((IdentityStoreObject x) => {
									if (!x.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_EmailAddress()))
									{
										return false;
									}
									return x.get_AttributesBusinessObject().get_AttributesCollection()[Helper.KnownProviderAttributes.get_EmailAddress()][0].get_Value().Equals(this.groupLog.get_GroupIdentity(), StringComparison.InvariantCultureIgnoreCase);
								}).ToList<IdentityStoreObject>();
								if (list.Count <= 0)
								{
									GroupsProcessor.logger.InfoFormat("Group {0} not found in container", groupLog.get_GroupIdentity());
								}
								else
								{
									GroupsProcessor.logger.InfoFormat("Group {0} found in container, usage will be calculated and stamped", groupLog.get_GroupIdentity());
									if (list[0].get_AttributesBusinessObject().HasValue("IMGUsedCount") && list[0].get_AttributesBusinessObject().HasValue("IMGLastUsed"))
									{
										groupLog.get_Usage().ForEach((DateTime useDate) => {
											int num;
											GroupsProcessor.logger.InfoFormat("Use date {0}", useDate.ToString());
											if (useDate > Convert.ToDateTime(list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGLastUsed"][0].get_Value()))
											{
												if (!fromDate.HasValue)
												{
													num = Convert.ToInt32(list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].get_Value()) + 1;
													list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].set_Value(num.ToString());
													list[0].set_StopNotification(false);
												}
												else if (useDate > fromDate.Value && useDate < toDate)
												{
													num = Convert.ToInt32(list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].get_Value()) + 1;
													list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].set_Value(num.ToString());
													list[0].set_StopNotification(false);
													return;
												}
											}
										});
									}
									else if (list[0].get_AttributesBusinessObject().HasValue("IMGUsedCount"))
									{
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute item = list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0];
										count = Convert.ToInt32(list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].get_Value()) + groupLog.get_Usage().Count;
										item.set_Value(count.ToString());
									}
									else if (!list[0].get_AttributesBusinessObject().IsIn("IMGUsedCount"))
									{
										Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = list[0].get_AttributesBusinessObject().get_AttributesCollection();
										List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
										attribute.set_Action(1);
										count = groupLog.get_Usage().Count;
										attribute.set_Value(count.ToString());
										attributes.Add(attribute);
										attributesCollection.Add("IMGUsedCount", attributes);
										list[0].set_StopNotification(false);
									}
									else
									{
										List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item1 = list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"];
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
										attribute1.set_Action(1);
										count = groupLog.get_Usage().Count;
										attribute1.set_Value(count.ToString());
										item1.Add(attribute1);
									}
									if (!list[0].get_AttributesBusinessObject().HasValue("IMGFirstUsed"))
									{
										if (!list[0].get_AttributesBusinessObject().IsIn("IMGFirstUsed"))
										{
											Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> strs = list[0].get_AttributesBusinessObject().get_AttributesCollection();
											List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
											Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute2 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
											attribute2.set_Action(1);
											dateTime = Convert.ToDateTime(groupLog.get_FirstUsed());
											string str = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
											if (str == null)
											{
												dateTime = DateTime.Now;
												str = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
											}
											attribute2.set_Value(str);
											attributes1.Add(attribute2);
											strs.Add("IMGFirstUsed", attributes1);
										}
										else
										{
											List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item2 = list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGFirstUsed"];
											Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute3 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
											attribute3.set_Action(1);
											dateTime = Convert.ToDateTime(groupLog.get_FirstUsed());
											string str1 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
											if (str1 == null)
											{
												dateTime = DateTime.Now;
												str1 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
											}
											attribute3.set_Value(str1);
											item2.Add(attribute3);
										}
										list[0].set_StopNotification(false);
									}
									if (list[0].get_AttributesBusinessObject().HasValue("IMGLastUsed"))
									{
										DateTime? lastUsed = groupLog.get_LastUsed();
										dateTime = Convert.ToDateTime(list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGLastUsed"][0].get_Value());
										if ((lastUsed.HasValue ? lastUsed.GetValueOrDefault() <= dateTime : true))
										{
											goto Label1;
										}
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute item3 = list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGLastUsed"][0];
										dateTime = Convert.ToDateTime(groupLog.get_LastUsed());
										string str2 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										if (str2 == null)
										{
											dateTime = DateTime.Now;
											str2 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										}
										item3.set_Value(str2);
										list[0].set_StopNotification(false);
										goto Label0;
									}
								Label1:
									if (!list[0].get_AttributesBusinessObject().IsIn("IMGLastUsed"))
									{
										Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection1 = list[0].get_AttributesBusinessObject().get_AttributesCollection();
										List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes2 = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute4 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
										attribute4.set_Action(1);
										dateTime = Convert.ToDateTime(groupLog.get_LastUsed());
										string str3 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										if (str3 == null)
										{
											dateTime = DateTime.Now;
											str3 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										}
										attribute4.set_Value(str3);
										attributes2.Add(attribute4);
										attributesCollection1.Add("IMGLastUsed", attributes2);
										list[0].set_StopNotification(false);
									}
									else
									{
										List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes3 = list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGLastUsed"];
										Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute5 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
										attribute5.set_Action(1);
										dateTime = Convert.ToDateTime(groupLog.get_LastUsed());
										string str4 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										if (str4 == null)
										{
											dateTime = DateTime.Now;
											str4 = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
										}
										attribute5.set_Value(str4);
										attributes3.Add(attribute5);
									}
								Label0:
									GroupsProcessor.logger.InfoFormat("Use count {0}", list[0].get_AttributesBusinessObject().get_AttributesCollection()["IMGUsedCount"][0].get_Value());
								}
							}
							catch (Exception exception)
							{
								GroupsProcessor.logger.ErrorFormat("Exception {0} :{1}", messagingProviderLog.get_ServerIdentity(), exception.Message);
							}
						});
					}
				}
				catch (Exception exception1)
				{
					GroupsProcessor.logger.ErrorFormat("Exception {0} :{1}", messagingProviderLog.get_ServerIdentity(), exception1.Message);
				}
			});
			return providerResult;
		}
	}
}