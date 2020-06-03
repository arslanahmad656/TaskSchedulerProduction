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
			Helper.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public Helper()
		{
		}

		public static SecurityToken DeSerializeSecurityToken(string tokenString, string issuerThumbPrint, string issuerThumbprintName)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader(tokenString));
			SecurityTokenHandlerConfiguration conf = new SecurityTokenHandlerConfiguration()
			{
				SaveBootstrapContext = true
			};
			conf.AudienceRestriction.AudienceMode = AudienceUriMode.Never;
			ConfigurationBasedIssuerNameRegistry actAsRegistry = new ConfigurationBasedIssuerNameRegistry();
			actAsRegistry.AddTrustedIssuer(issuerThumbPrint, issuerThumbPrint);
			conf.IssuerNameRegistry = actAsRegistry;
			List<SecurityToken> tokens = new List<SecurityToken>()
			{
				new X509SecurityToken(Constants.DefaultCertificate)
			};
			conf.IssuerTokenResolver = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(new ReadOnlyCollection<SecurityToken>(tokens), true);
			SecurityTokenHandlerCollection handlers = new SecurityTokenHandlerCollection(conf)
			{
				new X509CertificateSessionSecurityTokenHandler(Constants.DefaultCertificate),
				new UserNameTokenHandler(),
				new SamlTokenHandler(),
				new EncryptedSecurityTokenHandler()
			};
			return handlers.ReadToken(xmlTextReader);
		}

		public static string GetAttributeValue(IdentityStoreObject directoryObject, string attrName)
		{
			string empty;
			if ((directoryObject == null ? true : directoryObject.get_AttributesBusinessObject().get_AttributesCollection().Count <= 0))
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
				List<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute> attrObject = keyValuePair.Value;
				empty = ((attrObject == null ? true : attrObject.Count <= 0) ? string.Empty : attrObject.FirstOrDefault<Imanami.GroupID.DataTransferObjects.DataContracts.Services.Attribute>().get_Value());
			}
			return empty;
		}

		public static IStoreTypeHelper GetStoreTypeHelper(int identityStoreId)
		{
			IStoreTypeHelper storeTypeHelper;
			try
			{
				ServicesAdministrationServiceClient adminClient = new ServicesAdministrationServiceClient(false);
				StoreType storeType = adminClient.GetIdentityStoreById(identityStoreId, false).get_StoreType();
				if (storeType != null)
				{
					storeTypeHelper = StoreTypeHelperFactory.get_Instance().GetStoreTypeHelper(storeType.get_StoreTypeName());
					return storeTypeHelper;
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Helper.logger.Error(string.Concat("Error occurred while getting store type helper. ", ex.Message), ex);
			}
			storeTypeHelper = null;
			return storeTypeHelper;
		}

		public static void LogDebugResults(ILog logger, ActionResult result, string action)
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

		public static void LogFailedResults(ILog logger, ActionResult result, string action)
		{
			if (result == null)
			{
				logger.ErrorFormat("Action: {0}, result is null", action);
			}
			else if ((result.get_Status() == null ? false : result.get_Status() != 3))
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

		public static DateTime ParseDateTime(string date)
		{
			DateTime dateTime;
			DateTime minValue;
			if (!StringUtility.IsBlank(date))
			{
				minValue = (DateTime.TryParse(date, out dateTime) ? dateTime : DateTime.MinValue);
			}
			else
			{
				minValue = DateTime.MinValue;
			}
			return minValue;
		}

		public static List<IdentityStoreObject> PrepareCompressedData(List<IdentityStoreObject> list)
		{
			string cData = DataCompressionHelper.CompressObjects<List<IdentityStoreObject>>(list);
			List<IdentityStoreObject> compressedList = new List<IdentityStoreObject>();
			IdentityStoreObject dummyGroup = new IdentityStoreObject();
			AttributesHelper.SetAttributeValue("CompressedData", cData, dummyGroup.get_AttributesBusinessObject(), 3);
			compressedList.Add(dummyGroup);
			return compressedList;
		}

		public static List<MembershipLifecycleGroup> PrepareCompressedData(List<MembershipLifecycleGroup> list)
		{
			string cData = DataCompressionHelper.CompressObjects<List<MembershipLifecycleGroup>>(list);
			List<MembershipLifecycleGroup> compressedList = new List<MembershipLifecycleGroup>();
			MembershipLifecycleGroup membershipLifecycleGroup = new MembershipLifecycleGroup();
			membershipLifecycleGroup.set_CompressedData(cData);
			compressedList.Add(membershipLifecycleGroup);
			return compressedList;
		}

		public static List<Group> PrepareCompressedData(List<Group> list)
		{
			string cData = DataCompressionHelper.CompressGroupMembership(list);
			List<Group> compressedList = new List<Group>();
			Group dummyGroup = new Group();
			AttributesHelper.SetAttributeValue("CompressedData", cData, dummyGroup.get_AttributesBusinessObject(), 3);
			compressedList.Add(dummyGroup);
			return compressedList;
		}
	}
}