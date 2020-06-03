using Imanami.Common;
using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.ExtensionData.MembershipLifeCycleData;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.DataTransferObjects.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler
{
	internal class MembershipJob
	{
		public MembershipJob()
		{
		}

		private static void AddInNotification(List<MembershipLifecycleGroup> AddMemberGroups, IdentityStoreObject lGroup, MembershipLifecycleMember _notifyMember)
		{
			if (!AddMemberGroups.Any<MembershipLifecycleGroup>((MembershipLifecycleGroup z) => z.get_GUID().Equals(lGroup.get_ObjectIdFromIdentityStore())))
			{
				MembershipLifecycleGroup membershipLifecycleGroup = new MembershipLifecycleGroup();
				membershipLifecycleGroup.set_GUID(lGroup.get_ObjectIdFromIdentityStore());
				membershipLifecycleGroup.set_DisplayName(lGroup.get_ObjectDisplayName());
				membershipLifecycleGroup.set_ObjectType(lGroup.get_ObjectType());
				membershipLifecycleGroup.set_MembershiplifeCycleMembers(new List<MembershipLifecycleMember>());
				AddMemberGroups.Add(membershipLifecycleGroup);
			}
			AddMemberGroups.First<MembershipLifecycleGroup>((MembershipLifecycleGroup z) => z.get_GUID().Equals(lGroup.get_ObjectIdFromIdentityStore())).get_MembershiplifeCycleMembers().Add(_notifyMember);
		}

		private static void GetAndNotifyBeforeAction(int identityStoreID, MembershipLifecycleClient membershipClient, Dictionary<string, bool> containers)
		{
			foreach (IdentityStoreObject removalPendingObjectsFromContainer in membershipClient.GetRemovalPendingObjectsFromContainer(identityStoreID, Utility.GetCurrentDate(), containers))
			{
				if (StringUtility.EqualsIgnoreCase(Helper.GetAttributeValue(removalPendingObjectsFromContainer, "IMGIsExpired"), bool.TrueString) || StringUtility.EqualsIgnoreCase(Helper.GetAttributeValue(removalPendingObjectsFromContainer, "IMGIsDeleted"), bool.TrueString))
				{
					continue;
				}
				if (!removalPendingObjectsFromContainer.get_AttributesBusinessObject().get_AttributesCollection().Any<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attr) => {
					if (!attr.Key.Equals("XMember", StringComparison.OrdinalIgnoreCase))
					{
						return false;
					}
					return attr.Value.Count > 0;
				}))
				{
					continue;
				}
				List<MembershipLifecycleGroup> membershipLifecycleGroups = new List<MembershipLifecycleGroup>();
				KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> keyValuePair = removalPendingObjectsFromContainer.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> e) => e.Key.Equals("XMember", StringComparison.OrdinalIgnoreCase));
				keyValuePair.Value.ForEach((Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute lMember) => {
					MembershipType membershipType;
					MembershipLifecycleMember membershipLifecycleMember = new MembershipLifecycleMember();
					membershipLifecycleMember.set_GUID(lMember.get_Value());
					membershipLifecycleMember.set_FromDate(Convert.ToDateTime(lMember.get_AttributeCollection()["XBeginningDate"], CultureInfo.InvariantCulture));
					membershipLifecycleMember.set_ToDate(Convert.ToDateTime(lMember.get_AttributeCollection()["XEndingDate"], CultureInfo.InvariantCulture));
					MembershipLifecycleMember membershipLifecycleMember1 = membershipLifecycleMember;
					Enum.TryParse<MembershipType>(lMember.get_AttributeCollection()["XMembershipType"], out membershipType);
					membershipLifecycleMember1.set_MembershipType(membershipType);
					MembershipJob.AddInNotification(membershipLifecycleGroups, removalPendingObjectsFromContainer, membershipLifecycleMember1);
				});
				membershipClient.SendRemovalRemindernotification(Helper.PrepareCompressedData(membershipLifecycleGroups), identityStoreID);
			}
		}

		private static void RemoveExtraAttributes(IdentityStoreObject lGroup)
		{
			if (lGroup.get_AttributesBusinessObject().IsIn("IMSGManagedGroupType"))
			{
				lGroup.get_AttributesBusinessObject().Remove("IMSGManagedGroupType");
			}
			if (lGroup.get_AttributesBusinessObject().IsIn("IMSGIncludes"))
			{
				lGroup.get_AttributesBusinessObject().Remove("IMSGIncludes");
			}
			if (lGroup.get_AttributesBusinessObject().IsIn("IMSGExcludes"))
			{
				lGroup.get_AttributesBusinessObject().Remove("IMSGExcludes");
			}
		}

		public static void RunMembershipLifeCycle(int identityStoreID)
		{
			// 
			// Current member / type: System.Void Imanami.GroupID.TaskScheduler.MembershipJob::RunMembershipLifeCycle(System.Int32)
			// File path: C:\Users\Administrator.ERISED\Desktop\Production\original\Imanami.GroupID.TaskScheduler.exe
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void RunMembershipLifeCycle(System.Int32)
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
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 523
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 95
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(IfStatement ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 360
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 55
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RenameEnumValues.cs:line 48
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 72
			//    at ...( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 317
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 127
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 499
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 383
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 59
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ...Match( , Int32 ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 112
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 28
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 437
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 90
			//    at ..Visit(IEnumerable ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 383
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 388
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 33
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 507
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 120
			//    at ..Visit(IEnumerable ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 383
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 388
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 33
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildAnonymousDelegatesStep.cs:line 21
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private static void SmartGroupExclude(List<string> smartGrpIncludes, List<string> smartGrpExcludes, IdentityStoreObject lGroup, Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute lMember)
		{
			KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> keyValuePair;
			if (smartGrpIncludes != null && smartGrpIncludes.Count > 0 && smartGrpIncludes.Contains(lMember.get_Value()))
			{
				keyValuePair = lGroup.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> e) => StringUtility.EqualsIgnoreCase(e.Key, "IMSGIncludes"));
				keyValuePair.Value.FirstOrDefault<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>((Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute z) => z.get_Value().Equals(lMember.get_Value())).set_Action(2);
			}
			if ((smartGrpExcludes == null || smartGrpExcludes.Count <= 0) && !lGroup.get_AttributesBusinessObject().get_AttributesCollection().ContainsKey("IMSGExcludes"))
			{
				Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = lGroup.get_AttributesBusinessObject().get_AttributesCollection();
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
				attribute.set_Action(1);
				attribute.set_Value(lMember.get_Value());
				attributes.Add(attribute);
				attributesCollection.Add("IMSGExcludes", attributes);
				return;
			}
			keyValuePair = lGroup.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> e) => StringUtility.EqualsIgnoreCase(e.Key, "IMSGExcludes"));
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> value = keyValuePair.Value;
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			attribute1.set_Action(1);
			attribute1.set_Value(lMember.get_Value());
			value.Add(attribute1);
		}

		private static void SmartGroupInclude(List<string> smartGrpIncludes, List<string> smartGrpExcludes, IdentityStoreObject lGroup, Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute lMember)
		{
			KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> keyValuePair;
			if (smartGrpExcludes != null && smartGrpExcludes.Count > 0 && smartGrpExcludes.Contains(lMember.get_Value()))
			{
				keyValuePair = lGroup.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> e) => StringUtility.EqualsIgnoreCase(e.Key, "IMSGExcludes"));
				keyValuePair.Value.FirstOrDefault<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>((Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute z) => z.get_Value().Equals(lMember.get_Value())).set_Action(2);
			}
			if ((smartGrpIncludes == null || smartGrpIncludes.Count <= 0) && !lGroup.get_AttributesBusinessObject().get_AttributesCollection().ContainsKey("IMSGIncludes"))
			{
				Dictionary<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> attributesCollection = lGroup.get_AttributesBusinessObject().get_AttributesCollection();
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
				attribute.set_Action(1);
				attribute.set_Value(lMember.get_Value());
				attributes.Add(attribute);
				attributesCollection.Add("IMSGIncludes", attributes);
				return;
			}
			keyValuePair = lGroup.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> e) => StringUtility.EqualsIgnoreCase(e.Key, "IMSGIncludes"));
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> value = keyValuePair.Value;
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			attribute1.set_Action(1);
			attribute1.set_Value(lMember.get_Value());
			value.Add(attribute1);
		}
	}
}