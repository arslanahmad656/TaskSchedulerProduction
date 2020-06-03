using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration;
using Imanami.GroupID.DataTransferObjects.Enums;
using Imanami.GroupID.DataTransferObjects.UtilityObjects;
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
	public abstract class GroupsProcessor
	{
		protected static ILog logger;

		static GroupsProcessor()
		{
			<>z__a_a.Initialize();
			GroupsProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		protected GroupsProcessor()
		{
		}

		public virtual IdentityStoreObject CloneObjectForUpdate(List<string> attributeNamesToClone, IdentityStoreObject objectToClone, IdentityStoreObject objectOrignal)
		{
			IdentityStoreObject returnValue = null;
			Arguments<List<string>, IdentityStoreObject, IdentityStoreObject> argument = new Arguments<List<string>, IdentityStoreObject, IdentityStoreObject>()
			{
				Arg0 = attributeNamesToClone,
				Arg1 = objectToClone,
				Arg2 = objectOrignal
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928622L),
				Method = <>z__a_a._25
			};
			<>z__a_a.a60.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						IdentityStoreObject identityStoreObject = new IdentityStoreObject();
						identityStoreObject.set_ObjectIdFromIdentityStore(objectToClone.get_ObjectIdFromIdentityStore());
						identityStoreObject.set_ObjectName(objectToClone.get_ObjectName());
						identityStoreObject.set_ObjectDisplayName(objectToClone.get_ObjectDisplayName());
						identityStoreObject.set_ObjectType(objectToClone.get_ObjectType());
						identityStoreObject.set_AttributesBusinessObject(new AttributeCollection());
						IdentityStoreObject identityStoreObject1 = identityStoreObject;
						foreach (string str in attributeNamesToClone)
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributeValues = this.GetAttributeValues(str, objectToClone.get_AttributesBusinessObject());
							if (attributeValues == null || !attributeValues.Any<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>())
							{
								continue;
							}
							if (objectOrignal != null)
							{
								List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attributes = this.GetAttributeValues(str, objectOrignal.get_AttributesBusinessObject());
								if (attributes != null && attributes.Count == 1 && StringUtility.EqualsIgnoreCase(attributes[0].get_Value(), attributeValues[0].get_Value()))
								{
									continue;
								}
							}
							identityStoreObject1.get_AttributesBusinessObject().Add(str, attributeValues);
						}
						returnValue = identityStoreObject1;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a60.OnException(methodExecutionArg);
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
								returnValue = (IdentityStoreObject)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a60.OnExit(methodExecutionArg);
					returnValue = (IdentityStoreObject)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (IdentityStoreObject)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual List<IdentityStoreObject> CloneObjectsForUpdate(List<string> attributeNamesToClone, List<IdentityStoreObject> objectsToClone, List<IdentityStoreObject> unchangedList)
		{
			List<IdentityStoreObject> returnValue = null;
			Arguments<List<string>, List<IdentityStoreObject>, List<IdentityStoreObject>> argument = new Arguments<List<string>, List<IdentityStoreObject>, List<IdentityStoreObject>>()
			{
				Arg0 = attributeNamesToClone,
				Arg1 = objectsToClone,
				Arg2 = unchangedList
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928623L),
				Method = <>z__a_a._23
			};
			<>z__a_a.a59.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						if (unchangedList == null || !unchangedList.Any<IdentityStoreObject>())
						{
							objectsToClone.ForEach((IdentityStoreObject o) => identityStoreObjects.Add(this.CloneObjectForUpdate(attributeNamesToClone, o, null)));
						}
						else
						{
							objectsToClone.ForEach((IdentityStoreObject o) => identityStoreObjects.Add(this.CloneObjectForUpdate(attributeNamesToClone, o, unchangedList.FirstOrDefault<IdentityStoreObject>((IdentityStoreObject k) => k.get_ObjectIdFromIdentityStore() == o.get_ObjectIdFromIdentityStore()))));
						}
						identityStoreObjects.RemoveAll((IdentityStoreObject z) => {
							if (z.get_AttributesBusinessObject() == null)
							{
								return true;
							}
							return !z.get_AttributesBusinessObject().get_AttributesCollection().Any<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>();
						});
						returnValue = identityStoreObjects;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a59.OnException(methodExecutionArg);
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
					<>z__a_a.a59.OnExit(methodExecutionArg);
					returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool ExtendGroupLife(int days, IdentityStoreObject group)
		{
			DateTime date;
			bool returnValue = false;
			Arguments<int, IdentityStoreObject> argument = new Arguments<int, IdentityStoreObject>()
			{
				Arg0 = days,
				Arg1 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928625L),
				Method = <>z__a_a._1f
			};
			<>z__a_a.a57.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attributeValue = this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject());
						if (StringUtility.IsBlank(attributeValue.get_Value()))
						{
							returnValue = false;
						}
						else if (DateTime.TryParse(attributeValue.get_Value(), out date))
						{
							date = date.AddDays((double)days).Date;
							attributeValue.set_Value(date.ToString("yyyy MMMM dd HH:mm:ss"));
							returnValue = true;
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a57.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a57.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool ExtendGroupLife(DateTime newDate, IdentityStoreObject group)
		{
			bool returnValue = false;
			Arguments<DateTime, IdentityStoreObject> argument = new Arguments<DateTime, IdentityStoreObject>()
			{
				Arg0 = newDate,
				Arg1 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928624L),
				Method = <>z__a_a._21
			};
			<>z__a_a.a58.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).set_Value(newDate.Date.ToString("yyyy MMMM dd HH:mm:ss"));
						returnValue = true;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a58.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a58.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		protected virtual bool ExtendLifeForGUS(IdentityStoreObject grp)
		{
			bool flag;
			DateTime today;
			if (!Helper.AppConfiguration.get_GUSIsLifecycleEnabled() || !Helper.AppConfiguration.get_GUSExtendGroupsLife() || !grp.get_AttributesBusinessObject().HasValue(Helper.KnownProviderAttributes.get_Alias()))
			{
				return false;
			}
			if (Helper.AppConfiguration.get_IsGroupAttestationEnabled())
			{
				return false;
			}
			try
			{
				string value = this.GetAttributeValue("IMGLastUsed", grp.get_AttributesBusinessObject()).get_Value();
				if (string.IsNullOrEmpty(value))
				{
					flag = false;
				}
				else
				{
					DateTime dateTime = Helper.ParseDateTime(value);
					if (dateTime == DateTime.MinValue)
					{
						GroupsProcessor.logger.ErrorFormat("ExtendLifeForGUS: Invalid date format {0}", dateTime);
						flag = false;
					}
					else if ((DateTime.Now - dateTime).Days > Helper.AppConfiguration.get_GUSUsedGroupsTime())
					{
						flag = false;
					}
					else
					{
						Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attributeValue = this.GetAttributeValue("XGroupExpirationPolicy", grp.get_AttributesBusinessObject());
						int defaultExpirationPolicy = -1;
						DateTime date = DateTime.MaxValue.Date;
						if (!int.TryParse(attributeValue.get_Value() ?? string.Empty, out defaultExpirationPolicy))
						{
							defaultExpirationPolicy = Helper.AppConfiguration.get_DefaultExpirationPolicy();
							if (defaultExpirationPolicy != 0)
							{
								today = DateTime.Today;
								today = today.AddDays((double)defaultExpirationPolicy);
								date = today.Date;
							}
						}
						else if (defaultExpirationPolicy != 0)
						{
							today = DateTime.Today;
							today = today.AddDays((double)defaultExpirationPolicy);
							date = today.Date;
						}
						this.SetAttributeValue("XGroupExpirationDate", date.ToString("yyyy MMMM dd HH:mm:ss"), grp.get_AttributesBusinessObject());
						today = DateTime.Now;
						this.SetAttributeValue("IMGLastRenewedDate", today.ToString(), grp.get_AttributesBusinessObject());
						flag = true;
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				LogExtension.LogException(GroupsProcessor.logger, string.Format("An Error occured while performing GLM Extend life operation on group: {0} Reason: {1}", this.GetAttributeValue(Helper.KnownProviderAttributes.get_DisplayName(), grp.get_AttributesBusinessObject()).get_Value() ?? string.Empty, exception.Message), exception);
				return false;
			}
			return flag;
		}

		public virtual string ExtractContainerName(string distinguishedName)
		{
			string empty = null;
			Arguments<string> argument = new Arguments<string>()
			{
				Arg0 = distinguishedName
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928628L),
				Method = <>z__a_a._19
			};
			<>z__a_a.a54.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (!StringUtility.IsBlank(distinguishedName))
						{
							string upper = distinguishedName.ToUpper();
							int num = upper.IndexOf(",CN=");
							if (num <= -1)
							{
								num = upper.IndexOf(",OU=");
								if (num <= -1)
								{
									num = upper.IndexOf(",DC=");
									empty = (num <= -1 ? string.Empty : distinguishedName.Substring(num + 1));
								}
								else
								{
									empty = distinguishedName.Substring(num + 1);
								}
							}
							else
							{
								empty = distinguishedName.Substring(num + 1);
							}
						}
						else
						{
							empty = string.Empty;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a54.OnException(methodExecutionArg);
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
								empty = (string)methodExecutionArg.ReturnValue;
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
					methodExecutionArg.ReturnValue = empty;
					<>z__a_a.a54.OnExit(methodExecutionArg);
					empty = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				empty = (string)methodExecutionArg.ReturnValue;
			}
			return empty;
		}

		public virtual List<string> GetAttributesToLoad()
		{
			List<string> strs = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928640L),
				Method = <>z__a_a._1
			};
			<>z__a_a.a42.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						strs = new List<string>()
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
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a42.OnException(methodExecutionArg);
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
								strs = (List<string>)methodExecutionArg.ReturnValue;
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
					methodExecutionArg.ReturnValue = strs;
					<>z__a_a.a42.OnExit(methodExecutionArg);
					strs = (List<string>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				strs = (List<string>)methodExecutionArg.ReturnValue;
			}
			return strs;
		}

		public virtual Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute GetAttributeValue(string attributeName, AttributeCollection attributes)
		{
			Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928638L),
				Method = <>z__a_a._5
			};
			<>z__a_a.a44.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes == null || attributes.get_AttributesCollection() == null)
						{
							attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
						}
						else if (attributes.IsIn(attributeName))
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item = attributes.get_AttributesCollection()[attributeName];
							attribute = (item.Count == 0 ? new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute() : item[0]);
						}
						else
						{
							attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a44.OnException(methodExecutionArg);
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
								attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
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
					methodExecutionArg.ReturnValue = attribute;
					<>z__a_a.a44.OnExit(methodExecutionArg);
					attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				attribute = (Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute)methodExecutionArg.ReturnValue;
			}
			return attribute;
		}

		public virtual List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> GetAttributeValues(string attributeName, AttributeCollection attributes)
		{
			List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> returnValue = null;
			Arguments<string, AttributeCollection> argument = new Arguments<string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928639L),
				Method = <>z__a_a._3
			};
			<>z__a_a.a43.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes == null || attributes.get_AttributesCollection() == null)
						{
							returnValue = null;
						}
						else
						{
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> item = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>();
							if (attributes.IsIn(attributeName))
							{
								item = attributes.get_AttributesCollection()[attributeName];
								returnValue = item;
							}
							else
							{
								returnValue = item;
							}
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a43.OnException(methodExecutionArg);
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
								returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a43.OnExit(methodExecutionArg);
					returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsGlmBlankGroup(IdentityStoreObject group)
		{
			bool returnValue = false;
			Arguments<IdentityStoreObject> argument = new Arguments<IdentityStoreObject>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928626L),
				Method = <>z__a_a._1d
			};
			<>z__a_a.a56.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						returnValue = (group.get_AttributesBusinessObject() == null ? true : !group.get_AttributesBusinessObject().IsIn("XGroupExpirationDate"));
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a56.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a56.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsGroup(IdentityStoreObject group)
		{
			// 
			// Current member / type: System.Boolean Imanami.GroupID.TaskScheduler.Glm.GroupsProcessor::IsGroup(Imanami.GroupID.DataTransferObjects.DataContracts.Services.IdentityStoreObject)
			// File path: C:\Users\Administrator.ERISED\Desktop\Production\original\Imanami.GroupID.TaskScheduler.exe
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
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 483
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 83
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
			//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 49
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 483
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 83
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
			bool excludeGlmContainers = false;
			Arguments<IdentityStoreObject> argument = new Arguments<IdentityStoreObject>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928630L),
				Method = <>z__a_a._15
			};
			<>z__a_a.a52.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (group != null)
						{
							string value = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty;
							if (Helper.AppConfiguration.get_GlmContainers().Count != 0)
							{
								string str = this.ExtractContainerName(value);
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
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a52.OnException(methodExecutionArg);
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
								excludeGlmContainers = (bool)methodExecutionArg.ReturnValue;
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
					methodExecutionArg.ReturnValue = excludeGlmContainers;
					<>z__a_a.a52.OnExit(methodExecutionArg);
					excludeGlmContainers = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				excludeGlmContainers = (bool)methodExecutionArg.ReturnValue;
			}
			return excludeGlmContainers;
		}

		public virtual bool IsNotificationDue(IdentityStoreObject group, out int dueDays)
		{
			DateTime dateTime;
			DateTime dateTime1;
			bool returnValue = false;
			Arguments<IdentityStoreObject, int> argument = new Arguments<IdentityStoreObject, int>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928635L),
				Method = <>z__a_a._b
			};
			<>z__a_a.a47.OnEntry(methodExecutionArg);
			dueDays = argument.Arg1;
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						TimeSpan timeSpan = new TimeSpan((long)0);
						if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime))
						{
							timeSpan = dateTime.Date.Subtract(DateTime.Now.Date);
						}
						dueDays = timeSpan.Days;
						bool flag = false;
						if (timeSpan.Days == 1)
						{
							if (Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
							{
								flag = true;
							}
						}
						else if (timeSpan.Days == 8)
						{
							if (Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport())
							{
								flag = true;
							}
						}
						else if (timeSpan.Days == 30 && Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport())
						{
							flag = true;
						}
						if (flag)
						{
							if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime1) && StringUtility.EqualsIgnoreCase(dateTime1.Date.ToString(), DateTime.Now.Date.ToString()))
							{
								flag = false;
							}
						}
						returnValue = flag;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						argument.Arg1 = dueDays;
						<>z__a_a.a47.OnException(methodExecutionArg);
						dueDays = argument.Arg1;
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					argument.Arg1 = dueDays;
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_a.a47.OnExit(methodExecutionArg);
					dueDays = argument.Arg1;
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsNotificationDueOneDay(IdentityStoreObject group, out int dueDays)
		{
			DateTime dateTime;
			DateTime dateTime1;
			bool returnValue = false;
			Arguments<IdentityStoreObject, int> argument = new Arguments<IdentityStoreObject, int>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928634L),
				Method = <>z__a_a._d
			};
			<>z__a_a.a48.OnEntry(methodExecutionArg);
			dueDays = argument.Arg1;
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						TimeSpan timeSpan = new TimeSpan((long)0);
						if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime))
						{
							timeSpan = dateTime.Date.Subtract(DateTime.Now.Date);
						}
						dueDays = timeSpan.Days;
						bool flag = false;
						if (timeSpan.Days == 1 && Helper.AppConfiguration.get_GenerateOnedayToExpiryReport())
						{
							flag = true;
						}
						if (flag)
						{
							if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime1) && StringUtility.EqualsIgnoreCase(dateTime1.Date.ToString(), DateTime.Now.Date.ToString()))
							{
								flag = false;
							}
						}
						returnValue = flag;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						argument.Arg1 = dueDays;
						<>z__a_a.a48.OnException(methodExecutionArg);
						dueDays = argument.Arg1;
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					argument.Arg1 = dueDays;
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_a.a48.OnExit(methodExecutionArg);
					dueDays = argument.Arg1;
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsNotificationDueSevenDays(IdentityStoreObject group, out int dueDays)
		{
			DateTime dateTime;
			DateTime dateTime1;
			bool returnValue = false;
			Arguments<IdentityStoreObject, int> argument = new Arguments<IdentityStoreObject, int>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928633L),
				Method = <>z__a_a._f
			};
			<>z__a_a.a49.OnEntry(methodExecutionArg);
			dueDays = argument.Arg1;
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						TimeSpan timeSpan = new TimeSpan((long)0);
						if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime))
						{
							timeSpan = dateTime.Date.Subtract(DateTime.Now.Date);
						}
						dueDays = timeSpan.Days;
						bool flag = false;
						if (timeSpan.Days == 7 && Helper.AppConfiguration.get_GenerateSevenDaysToExpiryReport())
						{
							flag = true;
						}
						if (flag)
						{
							if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime1) && StringUtility.EqualsIgnoreCase(dateTime1.Date.ToString(), DateTime.Now.Date.ToString()))
							{
								flag = false;
							}
						}
						returnValue = flag;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						argument.Arg1 = dueDays;
						<>z__a_a.a49.OnException(methodExecutionArg);
						dueDays = argument.Arg1;
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					argument.Arg1 = dueDays;
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_a.a49.OnExit(methodExecutionArg);
					dueDays = argument.Arg1;
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsNotificationDueThirtyDays(IdentityStoreObject group, out int dueDays)
		{
			DateTime dateTime;
			DateTime dateTime1;
			bool returnValue = false;
			Arguments<IdentityStoreObject, int> argument = new Arguments<IdentityStoreObject, int>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928632L),
				Method = <>z__a_a._11
			};
			<>z__a_a.a50.OnEntry(methodExecutionArg);
			dueDays = argument.Arg1;
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						TimeSpan timeSpan = new TimeSpan((long)0);
						if (DateTime.TryParse(this.GetAttributeValue("XGroupExpirationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime))
						{
							timeSpan = dateTime.Date.Subtract(DateTime.Now.Date);
						}
						dueDays = timeSpan.Days;
						bool flag = false;
						if (timeSpan.Days == 30 && Helper.AppConfiguration.get_GenerateThirtyDaysToExpiryReport())
						{
							flag = true;
						}
						if (flag)
						{
							if (DateTime.TryParse(this.GetAttributeValue("IMGLastSentExpireNotificationDate", group.get_AttributesBusinessObject()).get_Value() ?? string.Empty, out dateTime1) && StringUtility.EqualsIgnoreCase(dateTime1.Date.ToString(), DateTime.Now.Date.ToString()))
							{
								flag = false;
							}
						}
						returnValue = flag;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						argument.Arg1 = dueDays;
						<>z__a_a.a50.OnException(methodExecutionArg);
						dueDays = argument.Arg1;
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					argument.Arg1 = dueDays;
					methodExecutionArg.ReturnValue = returnValue;
					<>z__a_a.a50.OnExit(methodExecutionArg);
					dueDays = argument.Arg1;
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool IsSystemGroup(IdentityStoreObject group)
		{
			bool returnValue = false;
			Arguments<IdentityStoreObject> argument = new Arguments<IdentityStoreObject>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928629L),
				Method = <>z__a_a._17
			};
			<>z__a_a.a53.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (group != null)
						{
							string value = this.GetAttributeValue(Helper.KnownProviderAttributes.get_DistinguishedName(), group.get_AttributesBusinessObject()).get_Value() ?? string.Empty;
							if (value.IndexOf("OU=Microsoft Exchange Security Groups", StringComparison.InvariantCultureIgnoreCase) > -1 || value.IndexOf("CN=Microsoft Exchange System Objects", StringComparison.InvariantCultureIgnoreCase) > -1)
							{
								returnValue = true;
							}
							else
							{
								returnValue = (!value.ToUpper().Contains("CN=BUILTIN") ? false : true);
							}
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a53.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a53.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual void LogResults(ActionResult result, string action)
		{
			Arguments<ActionResult, string> argument = new Arguments<ActionResult, string>()
			{
				Arg0 = result,
				Arg1 = action
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928631L),
				Method = <>z__a_a._13
			};
			<>z__a_a.a51.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
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
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a51.OnException(methodExecutionArg);
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
					<>z__a_a.a51.OnExit(methodExecutionArg);
				}
			}
		}

		public virtual bool SetAttributeValue(string attributeName, string value, AttributeCollection attributes)
		{
			bool returnValue = false;
			Arguments<string, string, AttributeCollection> argument = new Arguments<string, string, AttributeCollection>()
			{
				Arg0 = attributeName,
				Arg1 = value,
				Arg2 = attributes
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928637L),
				Method = <>z__a_a._7
			};
			<>z__a_a.a45.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (attributes != null)
						{
							if (!attributes.HasValue(attributeName))
							{
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute = new Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute();
								attribute.set_Value(value);
								attribute.set_Action(3);
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attribute1 = attribute;
								if (!attributes.IsIn(attributeName))
								{
									attributes.Add(attributeName, new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
									{
										attribute1
									});
								}
								else
								{
									attributes.get_AttributesCollection()[attributeName] = new List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>()
									{
										attribute1
									};
								}
							}
							else
							{
								Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attributeValue = this.GetAttributeValue(attributeName, attributes);
								attributeValue.set_Value(value);
								attributeValue.set_Action(3);
							}
							returnValue = true;
						}
						else
						{
							returnValue = false;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a45.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a45.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public virtual bool ShouldExpireSecurityGroup(IdentityStoreObject group)
		{
			bool returnValue = false;
			Arguments<IdentityStoreObject> argument = new Arguments<IdentityStoreObject>()
			{
				Arg0 = group
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886338928636L),
				Method = <>z__a_a._9
			};
			<>z__a_a.a46.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (!Helper.AppConfiguration.get_IsSecurityGroupExpirationEnabled())
						{
							Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute attributeValue = this.GetAttributeValue("groupType", group.get_AttributesBusinessObject());
							attributeValue.set_Value(attributeValue.get_Value() ?? string.Empty);
							returnValue = (attributeValue.get_Value() == "4" || attributeValue.get_Value() == "2" ? true : attributeValue.get_Value() == "8");
						}
						else
						{
							returnValue = true;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_a.a46.OnException(methodExecutionArg);
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
								returnValue = (bool)methodExecutionArg.ReturnValue;
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
					<>z__a_a.a46.OnExit(methodExecutionArg);
					returnValue = (bool)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (bool)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}
	}
}