using Imanami.Common;
using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.ExtensionData.MembershipLifeCycleData;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.PublicInterfaces.Providers;
using Imanami.STS.Core.Tokens;
using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;

namespace Imanami.GroupID.TaskScheduler
{
	public class Helper
	{
		private static ILog logger;

		public static Imanami.GroupID.DataTransferObjects.DataContracts.Services.Configuration.AppConfiguration AppConfiguration
		{
			get;
			set;
		}

		public static TaskScheduling CurrentTask
		{
			get;
			set;
		}

		public static IdentityStoreObject CurrentUser
		{
			get;
			set;
		}

		public static bool IsSystemSecurityContext
		{
			get;
			set;
		}

		public static KnownAttributes KnownProviderAttributes
		{
			get;
			set;
		}

		static Helper()
		{
			<>z__a_2.Initialize();
			Helper.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public Helper()
		{
		}

		public static SecurityToken DeSerializeSecurityToken(string tokenString, string issuerThumbPrint, string issuerThumbprintName)
		{
			SecurityToken returnValue = null;
			Arguments<string, string, string> argument = new Arguments<string, string, string>()
			{
				Arg0 = tokenString,
				Arg1 = issuerThumbPrint,
				Arg2 = issuerThumbprintName
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977204L),
				Method = <>z__a_2._5
			};
			<>z__a_2.a23.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(tokenString));
						SecurityTokenHandlerConfiguration securityTokenHandlerConfiguration = new SecurityTokenHandlerConfiguration()
						{
							SaveBootstrapContext = true
						};
						securityTokenHandlerConfiguration.AudienceRestriction.AudienceMode = AudienceUriMode.Never;
						ConfigurationBasedIssuerNameRegistry configurationBasedIssuerNameRegistry = new ConfigurationBasedIssuerNameRegistry();
						configurationBasedIssuerNameRegistry.AddTrustedIssuer(issuerThumbPrint, issuerThumbPrint);
						securityTokenHandlerConfiguration.IssuerNameRegistry = configurationBasedIssuerNameRegistry;
						securityTokenHandlerConfiguration.IssuerTokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new ReadOnlyCollection<SecurityToken>(new List<SecurityToken>()
						{
							new X509SecurityToken(Constants.DefaultCertificate)
						}), true);
						returnValue = (new SecurityTokenHandlerCollection(securityTokenHandlerConfiguration)
						{
							new X509CertificateSessionSecurityTokenHandler(Constants.DefaultCertificate),
							new UserNameTokenHandler(),
							new SamlTokenHandler(),
							new EncryptedSecurityTokenHandler()
						}).ReadToken(xmlTextReader);
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a23.OnException(methodExecutionArg);
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
								returnValue = (SecurityToken)methodExecutionArg.ReturnValue;
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
					<>z__a_2.a23.OnExit(methodExecutionArg);
					returnValue = (SecurityToken)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (SecurityToken)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static string GetAttributeValue(IdentityStoreObject directoryObject, string attrName)
		{
			string empty = null;
			Arguments<IdentityStoreObject, string> argument = new Arguments<IdentityStoreObject, string>()
			{
				Arg0 = directoryObject,
				Arg1 = attrName
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977199L),
				Method = <>z__a_2._f
			};
			<>z__a_2.a28.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (directoryObject == null || directoryObject.get_AttributesBusinessObject().get_AttributesCollection().Count <= 0)
						{
							empty = string.Empty;
						}
						else if (!directoryObject.get_AttributesBusinessObject().get_AttributesCollection().ContainsKey(attrName))
						{
							empty = string.Empty;
						}
						else
						{
							KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> keyValuePair = directoryObject.get_AttributesBusinessObject().get_AttributesCollection().FirstOrDefault<KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>>>((KeyValuePair<string, List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>> y) => StringUtility.EqualsIgnoreCase(y.Key, attrName));
							List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> value = keyValuePair.Value;
							empty = (value == null || value.Count <= 0 ? string.Empty : value.FirstOrDefault<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>().get_Value());
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a28.OnException(methodExecutionArg);
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
					<>z__a_2.a28.OnExit(methodExecutionArg);
					empty = (string)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				empty = (string)methodExecutionArg.ReturnValue;
			}
			return empty;
		}

		public static IStoreTypeHelper GetStoreTypeHelper(int identityStoreId)
		{
			IStoreTypeHelper returnValue = null;
			Arguments<int> argument = new Arguments<int>()
			{
				Arg0 = identityStoreId
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977206L),
				Method = <>z__a_2._1
			};
			<>z__a_2.a21.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						try
						{
							StoreType storeType = (new ServicesAdministrationServiceClient(false)).GetIdentityStoreById(identityStoreId, false).get_StoreType();
							if (storeType != null)
							{
								IStoreTypeHelper storeTypeHelper = StoreTypeHelperFactory.get_Instance().GetStoreTypeHelper(storeType.get_StoreTypeName());
								returnValue = storeTypeHelper;
								return returnValue;
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							Helper.logger.Error(string.Concat("Error occurred while getting store type helper. ", exception.Message), exception);
						}
						returnValue = null;
						return returnValue;
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						<>z__a_2.a21.OnException(methodExecutionArg);
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
								returnValue = (IStoreTypeHelper)methodExecutionArg.ReturnValue;
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
					<>z__a_2.a21.OnExit(methodExecutionArg);
					returnValue = (IStoreTypeHelper)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (IStoreTypeHelper)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static void LogDebugResults(ILog logger, ActionResult result, string action)
		{
			Arguments<ILog, ActionResult, string> argument = new Arguments<ILog, ActionResult, string>()
			{
				Arg0 = logger,
				Arg1 = result,
				Arg2 = action
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977198L),
				Method = <>z__a_2._11
			};
			<>z__a_2.a29.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (result != null)
						{
							logger.DebugFormat("Action: {0}, message: {1}", result.get_Message() ?? string.Empty, action);
							ILog log = logger;
							MessageType status = result.get_Status();
							log.DebugFormat("Action: {0}, status:", status.ToString(), action);
							ILog log1 = logger;
							object data = result.get_Data();
							if (data == null)
							{
								data = string.Empty;
							}
							log1.DebugFormat("Action: {0}, data:", data, action);
							if (result.get_Details() != null)
							{
								result.get_Details().ForEach((ActionResult x) => Helper.LogDebugResults(logger, x, action));
							}
						}
						else
						{
							logger.DebugFormat("Action: {0}, result is null", action);
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a29.OnException(methodExecutionArg);
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
					<>z__a_2.a29.OnExit(methodExecutionArg);
				}
			}
		}

		public static void LogFailedResults(ILog logger, ActionResult result, string action)
		{
			Arguments<ILog, ActionResult, string> argument = new Arguments<ILog, ActionResult, string>()
			{
				Arg0 = logger,
				Arg1 = result,
				Arg2 = action
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977200L),
				Method = <>z__a_2._d
			};
			<>z__a_2.a27.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (result == null)
						{
							logger.ErrorFormat("Action: {0}, result is null", action);
						}
						else if (result.get_Status() != null && result.get_Status() != 3)
						{
							logger.ErrorFormat("Action: {0}, message: {1}", result.get_Message() ?? string.Empty, action);
							ILog log = logger;
							MessageType status = result.get_Status();
							log.ErrorFormat("Action: {0}, status:", status.ToString(), action);
							ILog log1 = logger;
							object data = result.get_Data();
							if (data == null)
							{
								data = string.Empty;
							}
							log1.ErrorFormat("Action: {0}, data:", data, action);
							if (result.get_Details() != null)
							{
								result.get_Details().ForEach((ActionResult x) => Helper.LogFailedResults(logger, x, action));
							}
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a27.OnException(methodExecutionArg);
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
					<>z__a_2.a27.OnExit(methodExecutionArg);
				}
			}
		}

		public static DateTime ParseDateTime(string date)
		{
			DateTime dateTime;
			DateTime minValue = new DateTime();
			Arguments<string> argument = new Arguments<string>()
			{
				Arg0 = date
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977205L),
				Method = <>z__a_2._3
			};
			<>z__a_2.a22.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						if (!StringUtility.IsBlank(date))
						{
							minValue = (DateTime.TryParse(date, out dateTime) ? dateTime : DateTime.MinValue);
						}
						else
						{
							minValue = DateTime.MinValue;
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a22.OnException(methodExecutionArg);
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
								minValue = (DateTime)methodExecutionArg.ReturnValue;
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
					methodExecutionArg.ReturnValue = minValue;
					<>z__a_2.a22.OnExit(methodExecutionArg);
					minValue = (DateTime)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				minValue = (DateTime)methodExecutionArg.ReturnValue;
			}
			return minValue;
		}

		public static List<IdentityStoreObject> PrepareCompressedData(List<IdentityStoreObject> list)
		{
			List<IdentityStoreObject> returnValue = null;
			Arguments<List<IdentityStoreObject>> argument = new Arguments<List<IdentityStoreObject>>()
			{
				Arg0 = list
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977203L),
				Method = <>z__a_2._7
			};
			<>z__a_2.a24.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						string str = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(list);
						List<IdentityStoreObject> identityStoreObjects = new List<IdentityStoreObject>();
						IdentityStoreObject identityStoreObject = new IdentityStoreObject();
						AttributesHelper.SetAttributeValue("CompressedData", str, identityStoreObject.get_AttributesBusinessObject(), 3);
						identityStoreObjects.Add(identityStoreObject);
						returnValue = identityStoreObjects;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a24.OnException(methodExecutionArg);
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
					<>z__a_2.a24.OnExit(methodExecutionArg);
					returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<IdentityStoreObject>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static List<MembershipLifecycleGroup> PrepareCompressedData(List<MembershipLifecycleGroup> list)
		{
			List<MembershipLifecycleGroup> returnValue = null;
			Arguments<List<MembershipLifecycleGroup>> argument = new Arguments<List<MembershipLifecycleGroup>>()
			{
				Arg0 = list
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977202L),
				Method = <>z__a_2._9
			};
			<>z__a_2.a25.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						string str = DataCompressionHelper.CompressObjects<List<MembershipLifecycleGroup>>(list);
						List<MembershipLifecycleGroup> membershipLifecycleGroups = new List<MembershipLifecycleGroup>();
						MembershipLifecycleGroup membershipLifecycleGroup = new MembershipLifecycleGroup();
						membershipLifecycleGroup.set_CompressedData(str);
						membershipLifecycleGroups.Add(membershipLifecycleGroup);
						returnValue = membershipLifecycleGroups;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a25.OnException(methodExecutionArg);
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
								returnValue = (List<MembershipLifecycleGroup>)methodExecutionArg.ReturnValue;
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
					<>z__a_2.a25.OnExit(methodExecutionArg);
					returnValue = (List<MembershipLifecycleGroup>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<MembershipLifecycleGroup>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public static List<Group> PrepareCompressedData(List<Group> list)
		{
			List<Group> returnValue = null;
			Arguments<List<Group>> argument = new Arguments<List<Group>>()
			{
				Arg0 = list
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(null, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339977201L),
				Method = <>z__a_2._b
			};
			<>z__a_2.a26.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						string str = DataCompressionHelper.CompressGroupMembership(list);
						List<Group> groups = new List<Group>();
						Group group = new Group();
						AttributesHelper.SetAttributeValue("CompressedData", str, group.get_AttributesBusinessObject(), 3);
						groups.Add(group);
						returnValue = groups;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_2.a26.OnException(methodExecutionArg);
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
								returnValue = (List<Group>)methodExecutionArg.ReturnValue;
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
					<>z__a_2.a26.OnExit(methodExecutionArg);
					returnValue = (List<Group>)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (List<Group>)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}
	}
}