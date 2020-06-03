using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration;
using Imanami.GroupID.DataTransferObjects.Enums;
using Imanami.GroupID.DataTransferObjects.UtilityObjects;
using Imanami.GroupID.TaskScheduler;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public abstract class GroupsProcessor
	{
		protected static ILog logger;

		static GroupsProcessor()
		{
			GroupsProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		protected GroupsProcessor()
		{
		}

		public virtual IdentityStoreObject CloneObjectForUpdate(List<string> attributeNamesToClone, IdentityStoreObject objectToClone, IdentityStoreObject objectOrignal)
		{
			IdentityStoreObject identityStoreObject = new IdentityStoreObject();
			identityStoreObject.set_ObjectIdFromIdentityStore(objectToClone.get_ObjectIdFromIdentityStore());
			identityStoreObject.set_ObjectName(objectToClone.get_ObjectName());
			identityStoreObject.set_ObjectDisplayName(objectToClone.get_ObjectDisplayName());
			identityStoreObject.set_ObjectType(objectToClone.get_ObjectType());
			identityStoreObject.set_AttributesBusinessObject(new AttributeCollection());
			IdentityStoreObject newObject = identityStoreObject;
			foreach (string attributeName in attributeNamesToClone)
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = this.GetAttributeValues(attributeName, objectToClone.get_AttributesBusinessObject());
				if ((values == null ? false : values.Any<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()))
				{
					if (objectOrignal != null)
					{
						List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> origValues = this.GetAttributeValues(attributeName, objectOrignal.get_AttributesBusinessObject());
						if ((origValues == null || origValues.Count != 1 ? false : StringUtility.EqualsIgnoreCase(origValues[0].get_Value(), values[0].get_Value())))
						{
							continue;
						}
					}
					newObject.get_AttributesBusinessObject().Add(attributeName, values);
				}
			}
			return newObject;
		}

		public virtual List<IdentityStoreObject> CloneObjectsForUpdate(List<string> attributeNamesToClone, List<IdentityStoreObject> objectsToClone, List<IdentityStoreObject> unchangedList)
		{
			List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
			if ((unchangedList == null ? true : !unchangedList.Any<IdentityStoreObject>()))
			{
				objectsToClone.ForEach((IdentityStoreObject o) => identityStoreObjects.Add(this.CloneObjectForUpdate(attributeNamesToClone, o, null)));
			}
			else
			{
				objectsToClone.ForEach((IdentityStoreObject o) => identityStoreObjects.Add(this.CloneObjectForUpdate(attributeNamesToClone, o, unchangedList.FirstOrDefault<IdentityStoreObject>((IdentityStoreObject k) => k.get_ObjectIdFromIdentityStore() == o.get_ObjectIdFromIdentityStore()))));
			}
			identityStoreObjects.RemoveAll((IdentityStoreObject z) => (z.get_AttributesBusinessObject() == null ? true : !z.get_AttributesBusinessObject().get_AttributesCollection().Any<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>()));
			return identityStoreObjects;
		}

		public virtual bool ExtendGroupLife(int days, IdentityStoreObject group)
		{
			DateTime expirationDate;
			bool flag;
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute expirationDateDto = this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject());
			if (StringUtility.IsBlank(expirationDateDto.get_Value()))
			{
				flag = false;
			}
			else if (DateTime.TryParse(expirationDateDto.get_Value(), out expirationDate))
			{
				expirationDate = expirationDate.AddDays((double)days).Date;
				expirationDateDto.set_Value(expirationDate.ToString("yyyy MMMM dd HH:mm:ss"));
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public virtual bool ExtendGroupLife(DateTime newDate, IdentityStoreObject group)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute expirationDateDto = this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject());
			expirationDateDto.set_Value(newDate.Date.ToString("yyyy MMMM dd HH:mm:ss"));
			return true;
		}

		protected virtual bool ExtendLifeForGUS(IdentityStoreObject grp)
		{
			bool flag;
			DateTime today;
			if ((!Helper.AppConfiguration.get_GUSIsLifecycleEnabled() || !Helper.AppConfiguration.get_GUSExtendGroupsLife() ? true : !grp.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Alias())))
			{
				flag = false;
			}
			else if (!Helper.AppConfiguration.get_IsGroupAttestationEnabled())
			{
				try
				{
					string lastUsed = this.GetAttributeValue("IMGLastUsed", grp.get_AttributesBusinessObject()).get_Value();
					if (string.IsNullOrEmpty(lastUsed))
					{
						flag = false;
						return flag;
					}
					else
					{
						DateTime lastUsedDate = Helper.ParseDateTime(lastUsed);
						if (lastUsedDate == DateTime.MinValue)
						{
							GroupsProcessor.logger.ErrorFormat("ExtendLifeForGUS: Invalid date format {0}", lastUsedDate);
							flag = false;
							return flag;
						}
						else if ((DateTime.Now - lastUsedDate).Days > Helper.AppConfiguration.get_GUSUsedGroupsTime())
						{
							flag = false;
							return flag;
						}
						else
						{
							Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute policyDto = this.GetAttributeValue("XGroupExpirationPolicy", grp.get_AttributesBusinessObject());
							int policy = -1;
							DateTime extendedDate = DateTime.MaxValue.Date;
							if (!int.TryParse(policyDto.get_Value() ?? string.Empty, out policy))
							{
								policy = Helper.AppConfiguration.get_DefaultExpirationPolicy();
								if (policy != 0)
								{
									today = DateTime.Today;
									today = today.AddDays((double)policy);
									extendedDate = today.Date;
								}
							}
							else if (policy != 0)
							{
								today = DateTime.Today;
								today = today.AddDays((double)policy);
								extendedDate = today.Date;
							}
							this.SetAttributeValue("XGroupExpirationDate", extendedDate.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
							today = DateTime.Now;
							this.SetAttributeValue("IMGLastRenewedDate", today.ToString(), grp.get_AttributesBusinessObject());
							flag = true;
							return flag;
						}
					}
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Extend life operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, ex.Message), ex);
				}
				flag = false;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public virtual string ExtractContainerName(string distinguishedName)
		{
			string empty;
			if (!StringUtility.IsBlank(distinguishedName))
			{
				string upperCaseDistinguishedName = distinguishedName.ToUpper();
				int index = upperCaseDistinguishedName.IndexOf(",CN=");
				if (index <= -1)
				{
					index = upperCaseDistinguishedName.IndexOf(",OU=");
					if (index <= -1)
					{
						index = upperCaseDistinguishedName.IndexOf(",DC=");
						empty = (index <= -1 ? string.Empty : distinguishedName.Substring(index + 1));
					}
					else
					{
						empty = distinguishedName.Substring(index + 1);
					}
				}
				else
				{
					empty = distinguishedName.Substring(index + 1);
				}
			}
			else
			{
				empty = string.Empty;
			}
			return empty;
		}

		public virtual List<string> GetAttributesToLoad()
		{
			return new List<string>()
			{
				"XGroupExpirationDate",
				"XGroupExpirationPolicy",
				"IMGLastSentExpireNotificationDate",
				"XAdditionalOwner",
				"IMGFirstUsed",
				"IMGLastUsed",
				"IMGUsedCount",
				"IMGLastProcessedDate",
				"IMGLastRenewedDate",
				Helper.KnownProviderAttributes.get_DistinguishedName(),
				Helper.KnownProviderAttributes.get_Name(),
				Helper.KnownProviderAttributes.get_DisplayName(),
				Helper.KnownProviderAttributes.get_GroupType(),
				Helper.KnownProviderAttributes.get_ObjectClass(),
				Helper.KnownProviderAttributes.get_Alias(),
				"IMSGManagedGroupType",
				"IMSGObjectParentKey"
			};
		}

		public virtual Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute GetAttributeValue(string attributeName, AttributeCollection attributes)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute;
			if ((attributes == null ? true : attributes.get_AttributesCollection() == null))
			{
				attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			}
			else if (attributes.IsIn(attributeName))
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = attributes.get_AttributesCollection()[attributeName];
				attribute = (values.Count == 0 ? new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute() : values[0]);
			}
			else
			{
				attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
			}
			return attribute;
		}

		public virtual List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> GetAttributeValues(string attributeName, AttributeCollection attributes)
		{
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes1;
			if ((attributes == null ? false : attributes.get_AttributesCollection() != null))
			{
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> values = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
				if (attributes.IsIn(attributeName))
				{
					values = attributes.get_AttributesCollection()[attributeName];
					attributes1 = values;
				}
				else
				{
					attributes1 = values;
				}
			}
			else
			{
				attributes1 = null;
			}
			return attributes1;
		}

		public virtual bool IsGlmBlankGroup(IdentityStoreObject group)
		{
			return (group.get_AttributesBusinessObject() == null ? true : !group.get_AttributesBusinessObject().IsIn("XGroupExpirationDate"));
		}

		public virtual bool IsGroup(IdentityStoreObject group)
		{
			// 
			// Current member / type: System.Boolean Imanami.GroupID.TaskScheduler.Glm.GroupsProcessor::IsGroup(Imanami.GroupID.DataTransferObjects.DataContracts.Services.IdentityStoreObject)
			// File path: C:\Users\Administrator.ERISED\Desktop\Production\Imanami.GroupID.TaskScheduler.exe
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean IsGroup(Imanami.GroupID.DataTransferObjects.DataContracts.Services.IdentityStoreObject)
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
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public virtual bool IsGroupInExcludedContainer(IdentityStoreObject group)
		{
			bool excludeGlmContainers;
			if (group != null)
			{
				string groupDn = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty;
				if (Helper.AppConfiguration.get_GlmContainers().Count != 0)
				{
					string str = this.ExtractContainerName(groupDn);
					excludeGlmContainers = (!Helper.AppConfiguration.get_ExcludeGlmContainers() ? !Helper.AppConfiguration.get_GlmContainers().Any<SelectedContainer>((SelectedContainer c) => StringUtility.EqualsIgnoreCase(c.get_Name(), str)) : Helper.AppConfiguration.get_GlmContainers().Any<SelectedContainer>((SelectedContainer c) => StringUtility.EqualsIgnoreCase(c.get_Name(), str)));
				}
				else
				{
					excludeGlmContainers = !Helper.AppConfiguration.get_ExcludeGlmContainers();
				}
			}
			else
			{
				excludeGlmContainers = false;
			}
			return excludeGlmContainers;
		}

		public virtual bool IsNotificationDue(IdentityStoreObject group, out int dueDays)
		{
			DateTime expirationDate;
			DateTime lastSentDate;
			TimeSpan span = new TimeSpan((long)0);
			if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out expirationDate))
			{
				span = expirationDate.Date.Subtract(DateTime.Now.Date);
			}
			dueDays = span.Days;
			bool notificationdue = false;
			if (span.Days == 1)
			{
				if (Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
				{
					notificationdue = true;
				}
			}
			else if (span.Days == 8)
			{
				if (Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport())
				{
					notificationdue = true;
				}
			}
			else if (span.Days == 30)
			{
				if (Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport())
				{
					notificationdue = true;
				}
			}
			if (notificationdue)
			{
				if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out lastSentDate))
				{
					if (StringUtility.EqualsIgnoreCase(lastSentDate.Date.ToString(), DateTime.Now.Date.ToString()))
					{
						notificationdue = false;
					}
				}
			}
			return notificationdue;
		}

		public virtual bool IsNotificationDueOneDay(IdentityStoreObject group, out int dueDays)
		{
			DateTime expirationDate;
			DateTime lastSentDate;
			TimeSpan span = new TimeSpan((long)0);
			if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out expirationDate))
			{
				span = expirationDate.Date.Subtract(DateTime.Now.Date);
			}
			dueDays = span.Days;
			bool notificationdue = false;
			if (span.Days == 1)
			{
				if (Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
				{
					notificationdue = true;
				}
			}
			if (notificationdue)
			{
				if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out lastSentDate))
				{
					if (StringUtility.EqualsIgnoreCase(lastSentDate.Date.ToString(), DateTime.Now.Date.ToString()))
					{
						notificationdue = false;
					}
				}
			}
			return notificationdue;
		}

		public virtual bool IsNotificationDueSevenDays(IdentityStoreObject group, out int dueDays)
		{
			DateTime expirationDate;
			DateTime lastSentDate;
			TimeSpan span = new TimeSpan((long)0);
			if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out expirationDate))
			{
				span = expirationDate.Date.Subtract(DateTime.Now.Date);
			}
			dueDays = span.Days;
			bool notificationdue = false;
			if (span.Days == 7)
			{
				if (Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport())
				{
					notificationdue = true;
				}
			}
			if (notificationdue)
			{
				if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out lastSentDate))
				{
					if (StringUtility.EqualsIgnoreCase(lastSentDate.Date.ToString(), DateTime.Now.Date.ToString()))
					{
						notificationdue = false;
					}
				}
			}
			return notificationdue;
		}

		public virtual bool IsNotificationDueThirtyDays(IdentityStoreObject group, out int dueDays)
		{
			DateTime expirationDate;
			DateTime lastSentDate;
			TimeSpan span = new TimeSpan((long)0);
			if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out expirationDate))
			{
				span = expirationDate.Date.Subtract(DateTime.Now.Date);
			}
			dueDays = span.Days;
			bool notificationdue = false;
			if (span.Days == 30)
			{
				if (Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport())
				{
					notificationdue = true;
				}
			}
			if (notificationdue)
			{
				if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out lastSentDate))
				{
					if (StringUtility.EqualsIgnoreCase(lastSentDate.Date.ToString(), DateTime.Now.Date.ToString()))
					{
						notificationdue = false;
					}
				}
			}
			return notificationdue;
		}

		public virtual bool IsSystemGroup(IdentityStoreObject group)
		{
			bool flag;
			if (group != null)
			{
				string groupDn = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty;
				if ((groupDn.IndexOf("OU=Microsoft Exchange Security Groups", StringComparison.InvariantCultureIgnoreCase) > -1 ? false : groupDn.IndexOf("CN=Microsoft Exchange System Objects", StringComparison.InvariantCultureIgnoreCase) <= -1))
				{
					flag = (!groupDn.ToUpper().Contains("CN=BUILTIN") ? false : true);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public virtual void LogResults(ActionResult result, string action)
		{
			if (result != null)
			{
				GroupsProcessor.logger.DebugFormat("Action: {0}, message: {1}", result.get_Message() ?? string.Empty, action);
				ILog log = GroupsProcessor.logger;
				MessageType status = result.get_Status();
				log.DebugFormat("Action: {0}, status:", status.ToString(), action);
				ILog log1 = GroupsProcessor.logger;
				object data = result.get_Data();
				if (data == null)
				{
					data = string.Empty;
				}
				log1.DebugFormat("Action: {0}, data:", data, action);
				if (result.get_Details() != null)
				{
					result.get_Details().ForEach((ActionResult x) => this.LogResults(x, action));
				}
			}
			else
			{
				GroupsProcessor.logger.DebugFormat("Action: {0}, result is null", action);
			}
		}

		public virtual bool SetAttributeValue(string attributeName, string value, AttributeCollection attributes)
		{
			bool flag;
			if (attributes != null)
			{
				if (!attributes.HasValue(attributeName))
				{
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
					attribute1.set_Value(value);
					attribute1.set_Action(3);
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = attribute1;
					if (!attributes.IsIn(attributeName))
					{
						attributes.Add(attributeName, new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
						{
							attribute
						});
					}
					else
					{
						attributes.get_AttributesCollection()[attributeName] = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
						{
							attribute
						};
					}
				}
				else
				{
					Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = this.GetAttributeValue(attributeName, attributes);
					attribute.set_Value(value);
					attribute.set_Action(3);
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public virtual bool ShouldExpireSecurityGroup(IdentityStoreObject group)
		{
			bool flag;
			if (!Helper.AppConfiguration.get_IsSecurityGroupExpirationEnabled())
			{
				Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = this.GetAttributeValue("groupType", group.get_AttributesBusinessObject());
				attribute.set_Value(attribute.get_Value() ?? string.Empty);
				flag = (attribute.get_Value() == "4" || attribute.get_Value() == "2" ? true : attribute.get_Value() == "8");
			}
			else
			{
				flag = true;
			}
			return flag;
		}
	}
}