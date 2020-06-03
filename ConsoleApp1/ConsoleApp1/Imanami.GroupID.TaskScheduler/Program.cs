using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.Foundation.Licensing;
using Imanami.Foundation.Security;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.DataTransferObjects.UtilityObjects;
using Imanami.STS.Core.Configuration;
using Imanami.STS.Core.Models;
using Imanami.STS.ServiceClient;
using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Imanami.GroupID.TaskScheduler
{
	public sealed class Program
	{
		private static ILog logger;

		static Program()
		{
			Program.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public Program()
		{
		}

		private static void InitializeSecurityContext(string tokenString)
		{
			tokenString = CryptographyHelper.DecryptFromLocalMachine(tokenString);
			SettingsSecureFileStore settingsSecureFileStore = new SettingsSecureFileStore();
			ServiceConfiguration configurations = settingsSecureFileStore.GetConfigurations();
			if (configurations == null)
			{
				Program.logger.Error("Client configurations not found");
				return;
			}
			SecurityToken securityToken = Helper.DeSerializeSecurityToken(tokenString, configurations.get_STSThumbprint(), configurations.get_STSThumbprintName());
			ServiceHelperFactory.get_Instance().set_BaseUri(configurations.get_ServiceBaseUrl());
			ClientConfiguration clientSettingsFromConfigDll = settingsSecureFileStore.GetClientSettingsFromConfigDll();
			if (clientSettingsFromConfigDll == null)
			{
				Program.logger.Error("Client configurations for security service not found");
			}
			if (clientSettingsFromConfigDll != null)
			{
				string empty = string.Empty;
				List<RequestClaim> requestClaims = new List<RequestClaim>();
				SamlSecurityToken samlSecurityToken = securityToken as SamlSecurityToken;
				if (samlSecurityToken != null && samlSecurityToken.Assertion != null && samlSecurityToken.Assertion.Statements != null && samlSecurityToken.Assertion.Statements.Count > 0)
				{
					SamlAttributeStatement item = samlSecurityToken.Assertion.Statements[0] as SamlAttributeStatement;
					if (item != null && item.Attributes != null && item.Attributes.Count > 0)
					{
						SamlAttribute samlAttribute = item.Attributes.FirstOrDefault<SamlAttribute>((SamlAttribute z) => StringUtility.EqualsIgnoreCase(z.Name, "clientid"));
						if (samlAttribute != null && samlAttribute.AttributeValues != null && samlAttribute.AttributeValues.Count > 0)
						{
							empty = samlAttribute.AttributeValues[0];
							requestClaims.Add(new RequestClaim("http://schemas.imanami.com/ws/2014/06/identity/claims/clientId", true, empty));
						}
					}
				}
				clientSettingsFromConfigDll.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
				NetworkCredential defaultCredentials = CredentialCache.DefaultCredentials as NetworkCredential;
				ClaimsPrincipalSelector claimsPrincipalSelector = (empty == string.Empty ? new ClaimsPrincipalSelector(defaultCredentials, clientSettingsFromConfigDll, null) : new ClaimsPrincipalSelector(defaultCredentials, clientSettingsFromConfigDll, requestClaims));
				claimsPrincipalSelector.GetClaimsPrincipal();
				ClaimsPrincipal.ClaimsPrincipalSelector = new Func<ClaimsPrincipal>(claimsPrincipalSelector.GetClaimsPrincipal);
				if (claimsPrincipalSelector.get_ClaimsPrincipal() != null)
				{
					claimsPrincipalSelector.get_ClaimsPrincipal().set_ActAsToken(securityToken);
				}
				ServiceHelperFactory.get_Instance().set_StsClientConfiguration(clientSettingsFromConfigDll);
				ServiceHelperFactory.get_Instance().set_StsUserCredentials(defaultCredentials);
			}
		}

		private static void InitializeSystemSecurityContext()
		{
			SettingsSecureFileStore settingsSecureFileStore = new SettingsSecureFileStore();
			ServiceConfiguration configurations = settingsSecureFileStore.GetConfigurations();
			if (configurations == null)
			{
				Program.logger.Error("Client configurations not found");
				return;
			}
			ServiceHelperFactory.get_Instance().set_BaseUri(configurations.get_ServiceBaseUrl());
			ClientConfiguration clientSettingsFromConfigDll = settingsSecureFileStore.GetClientSettingsFromConfigDll();
			if (clientSettingsFromConfigDll == null)
			{
				Program.logger.Error("Client configurations for security service not found");
			}
			if (clientSettingsFromConfigDll != null)
			{
				clientSettingsFromConfigDll.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
				NetworkCredential defaultCredentials = CredentialCache.DefaultCredentials as NetworkCredential;
				ClaimsPrincipalSelector claimsPrincipalSelector = new ClaimsPrincipalSelector(defaultCredentials, clientSettingsFromConfigDll, null);
				claimsPrincipalSelector.GetClaimsPrincipal();
				ClaimsPrincipal.ClaimsPrincipalSelector = new Func<ClaimsPrincipal>(claimsPrincipalSelector.GetClaimsPrincipal);
				if (claimsPrincipalSelector.get_ClaimsPrincipal() != null)
				{
					claimsPrincipalSelector.get_ClaimsPrincipal().set_ActAsToken(ActiveClient.GetActAsToken(defaultCredentials, clientSettingsFromConfigDll, claimsPrincipalSelector.get_ClaimsPrincipal()));
				}
				ServiceHelperFactory.get_Instance().set_StsClientConfiguration(clientSettingsFromConfigDll);
				ServiceHelperFactory.get_Instance().set_StsUserCredentials(defaultCredentials);
			}
		}

		private static void Main(string[] args)
		{
			try
			{
				try
				{
					LogExtension.RegisterCustomLogLevels();
					XmlConfigurator.Configure();
					LogExtension.EnterMethod(Program.logger, MethodBase.GetCurrentMethod(), args);
					string str = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Schedules");
					int num = Convert.ToInt32(CryptographyHelper.DecryptFromLocalMachine(args[0]));
					str = Path.Combine(str, string.Concat("task", num, ".txt"));
					if (!File.Exists(str))
					{
						Program.InitializeSystemSecurityContext();
						Helper.IsSystemSecurityContext = true;
					}
					else
					{
						using (StreamReader streamReader = File.OpenText(str))
						{
							string[] strArrays = streamReader.ReadToEnd().Split(new string[] { "<#!#>" }, StringSplitOptions.None);
							num = Convert.ToInt32(CryptographyHelper.DecryptFromLocalMachine(strArrays[0]));
							Program.InitializeSecurityContext(CryptographyHelper.DecryptFromLocalMachine(strArrays[2]));
							Helper.IsSystemSecurityContext = false;
						}
					}
					LicensingProvider licensingProvider = new LicensingProvider();
					if (!licensingProvider.HasValidProductLicense(1) && !licensingProvider.HasValidProductLicense(3))
					{
						if (!licensingProvider.HasValidProductLicense(8))
						{
							Program.logger.Error("Invalid License");
							return;
						}
						else
						{
							TaskScheduling scheduledJob = (new ServicesSchedulingServiceClient(true)).GetScheduledJob((long)num);
							if (scheduledJob == null)
							{
								return;
							}
							else if (scheduledJob.get_JobType() != 13)
							{
								Program.logger.Error("Invalid License");
								return;
							}
						}
					}
					(new JobProcessor()).ProcessJob((long)num);
				}
				catch (CryptographicException cryptographicException1)
				{
					CryptographicException cryptographicException = cryptographicException1;
					string str1 = string.Concat(cryptographicException.Message, " Error in initializing security context for the Scheduled job. Possible reason may be, that a required windows service is not running. Please make sure that 'CNG Key Isolation' windows service is running.");
					LogExtension.LogException(Program.logger, str1, cryptographicException, 1, "Logging.Const.LoggingConstants.jobUpdate");
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					LogExtension.LogException(Program.logger, exception.Message, exception, 1, "Logging.Const.LoggingConstants.jobUpdate");
				}
			}
			finally
			{
				LogExtension.ExitMethod(Program.logger, MethodBase.GetCurrentMethod(), args);
			}
		}
	}
}