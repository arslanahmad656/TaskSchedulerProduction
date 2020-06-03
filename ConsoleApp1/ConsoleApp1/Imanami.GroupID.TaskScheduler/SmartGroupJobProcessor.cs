using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.TaskProgressReporting;
using Imanami.GroupID.DataTransferObjects.Enums;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler
{
	public class SmartGroupJobProcessor
	{
		private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public SmartGroupJobProcessor()
		{
		}

		private Group ConvertToGroup(IdentityStoreObject identityStoreObject)
		{
			Group group1;
			if (!(identityStoreObject is Group))
			{
				Group group = new Group();
				group.set_ObjectIdFromIdentityStore(identityStoreObject.get_ObjectIdFromIdentityStore());
				group.set_ObjectName(identityStoreObject.get_ObjectName());
				group.set_ObjectDisplayName(identityStoreObject.get_ObjectDisplayName());
				group.set_DisplayName(identityStoreObject.get_ObjectDisplayName());
				group.set_ObjectType(identityStoreObject.get_ObjectType());
				group.set_AttributesBusinessObject(identityStoreObject.get_AttributesBusinessObject());
				group.set_StopNotification(identityStoreObject.get_StopNotification());
				group1 = group;
			}
			else
			{
				group1 = identityStoreObject as Group;
			}
			return group1;
		}

		private Dictionary<string, Dictionary<string, IdentityStoreObject>> EnsureChildGroups(int identityStoreId, ServicesGroupServiceClient groupClient, Dictionary<string, Dictionary<string, IdentityStoreObject>> managedGroups)
		{
			foreach (KeyValuePair<string, Dictionary<string, IdentityStoreObject>> pair in managedGroups)
			{
				try
				{
					List<IdentityStoreObject> childGroups = groupClient.GetAllLevelCurrentChildGroups(identityStoreId, pair.Key, null, new List<string>()
					{
						"IMSGManagedGroupType",
						"IMSGObjectParentKey",
						Helper.KnownProviderAttributes.get_DistinguishedName()
					});
					Dictionary<string, IdentityStoreObject> childDict = pair.Value;
					IdentityStoreObject[] array = childGroups.ToArray();
					for (int i = 0; i < (int)array.Length; i++)
					{
						IdentityStoreObject g = array[i];
						if (!childDict.ContainsKey(g.get_ObjectIdFromIdentityStore()))
						{
							childDict.Add(g.get_ObjectIdFromIdentityStore(), g);
						}
					}
					ILookup<string, IdentityStoreObject> childGroupsLookup = childGroups.ToLookup<IdentityStoreObject, string>((IdentityStoreObject c) => c.get_ObjectIdFromIdentityStore());
					KeyValuePair<string, IdentityStoreObject>[] keyValuePairArray = childDict.ToArray<KeyValuePair<string, IdentityStoreObject>>();
					for (int j = 0; j < (int)keyValuePairArray.Length; j++)
					{
						KeyValuePair<string, IdentityStoreObject> childPair = keyValuePairArray[j];
						if (!childGroupsLookup[childPair.Key].Any<IdentityStoreObject>())
						{
							childDict.Remove(childPair.Key);
						}
					}
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					this.logger.Error(string.Concat("Error occurred in getting child groups after smart group update. ", ex.Message));
					continue;
				}
			}
			return managedGroups;
		}

		private bool NeedNotify(TaskScheduling task, int processedFailed, int processedSuccessFully)
		{
			bool flag;
			if (!task.get_SendReport())
			{
				flag = false;
			}
			else if ((!task.get_SendOnFailure() ? false : processedFailed > 0))
			{
				flag = true;
			}
			else if ((!task.get_SendOnSuccess() ? true : processedSuccessFully <= 0))
			{
				flag = ((!task.get_SendOnUpdate() ? true : processedSuccessFully <= 0) ? false : true);
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		public void ProcessSmartGroupUpdate(TaskScheduling task)
		{
			// 
			// Current member / type: System.Void Imanami.GroupID.TaskScheduler.SmartGroupJobProcessor::ProcessSmartGroupUpdate(Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling.TaskScheduling)
			// File path: C:\Users\Administrator.ERISED\Desktop\Production\Imanami.GroupID.TaskScheduler.exe
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void ProcessSmartGroupUpdate(Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling.TaskScheduling)
			// 
			// Object reference not set to an instance of an object.
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RenameEnumValues.cs:line 77
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 499
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RenameEnumValues.cs:line 68
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 383
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 59
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(IfStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 361
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 55
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(IfStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 361
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 55
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(IfStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 361
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 55
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(IfStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 361
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 55
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(ForEachStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 442
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 73
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RenameEnumValues.cs:line 48
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private bool SendNotification(ServicesGroupServiceClient groupClient, TaskScheduling task, List<Group> groups, int processedFailed, int processedSuccessFully, string clientID = "")
		{
			bool flag;
			if (this.NeedNotify(task, processedFailed, processedSuccessFully))
			{
				try
				{
					List<Group> compressedList = Helper.PrepareCompressedData(groups);
					Console.WriteLine(string.Concat(Environment.NewLine, "SENDING NOTIFICATION THROUGH GROUP CLIENT FOR CLIENT ", clientID, Environment.NewLine));
					groupClient.SendUpdateNotification(task.get_IdentityStoreId(), compressedList, task.get_SendToSpecified(), task.get_SendToOwner(), task.get_SendOnUpdate(), clientID);
					flag = true;
				}
				catch (Exception exception)
				{
					this.logger.Error("Error occurred while sending smart group update notifications. ", exception);
					flag = false;
				}
			}
			else
			{
				flag = true;
			}
			return flag;
		}

		private void SortByParent(string parentId, Dictionary<string, IdentityStoreObject> childDict, ref Dictionary<string, IdentityStoreObject> sortedDict)
		{
			IEnumerable<KeyValuePair<string, IdentityStoreObject>> childList = 
				from c in childDict
				where (!StringUtility.EqualsIgnoreCase(AttributesHelper.GetValueAsString("IMSGObjectParentKey", c.Value.get_AttributesBusinessObject(), ""), parentId) ? false : true)
				select c;
			foreach (KeyValuePair<string, IdentityStoreObject> pair in childList)
			{
				if (!sortedDict.ContainsKey(pair.Key))
				{
					sortedDict.Add(pair.Key, pair.Value);
				}
				this.SortByParent(pair.Key, childDict, ref sortedDict);
			}
		}

		private Dictionary<int, List<Group>> SplitToDictionary(List<Group> list, int chunks)
		{
			Dictionary<int, List<Group>> nums;
			Dictionary<int, List<Group>> items = new Dictionary<int, List<Group>>();
			if (list.Count > chunks)
			{
				int index = 0;
				int count = 0;
				List<Group> childList = new List<Group>();
				foreach (Group val in list)
				{
					count++;
					childList.Add(val);
					if (count >= chunks)
					{
						items.Add(index, childList);
						index++;
						count = 0;
						childList = new List<Group>();
					}
				}
				if (childList.Count > 0)
				{
					items.Add(index, childList);
				}
				if (this.logger.get_IsDebugEnabled())
				{
					this.logger.DebugFormat("Splitting list of groups into Dictionary with page size {0} and list {1}", chunks, list.Count);
					foreach (KeyValuePair<int, List<Group>> item in items)
					{
						this.logger.DebugFormat("Dictinary item: {0}", item.Value.Count);
					}
				}
				nums = items;
			}
			else
			{
				items.Add(0, list);
				nums = items;
			}
			return nums;
		}
	}
}