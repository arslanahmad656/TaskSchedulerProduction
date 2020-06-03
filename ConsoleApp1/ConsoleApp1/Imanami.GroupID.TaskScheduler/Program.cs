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
			SettingsSecureFileStore securitySettings = new SettingsSecureFileStore();
			ServiceConfiguration configs = securitySettings.GetConfigurations();
			if (configs != null)
			{
				SecurityToken token = Helper.DeSerializeSecurityToken(tokenString, configs.get_STSThumbprint(), configs.get_STSThumbprintName());
				ServiceHelperFactory.get_Instance().set_BaseUri(configs.get_ServiceBaseUrl());
				ClientConfiguration settings = securitySettings.GetClientSettingsFromConfigDll();
				if (settings == null)
				{
					Program.logger.Error("Client configurations for security service not found");
				}
				if (settings != null)
				{
					string clientIDFromToken = string.Empty;
					List<RequestClaim> customClaims = new List<RequestClaim>();
					SamlSecurityToken samlToken = token as SamlSecurityToken;
					if ((samlToken == null || samlToken.Assertion == null || samlToken.Assertion.Statements == null ? false : samlToken.Assertion.Statements.Count > 0))
					{
						SamlAttributeStatement statements = samlToken.Assertion.Statements[0] as SamlAttributeStatement;
						if ((statements == null || statements.Attributes == null ? false : statements.Attributes.Count > 0))
						{
							SamlAttribute ClientId = statements.Attributes.FirstOrDefault<SamlAttribute>((SamlAttribute z) => StringUtility.EqualsIgnoreCase(z.Name, "clientid"));
							if ((ClientId == null || ClientId.AttributeValues == null ? false : ClientId.AttributeValues.Count > 0))
							{
								clientIDFromToken = ClientId.AttributeValues[0];
								customClaims.Add(new RequestClaim("http://schemas.imanami.com/ws/2014/06/identity/claims/clientId", true, clientIDFromToken));
							}
						}
					}
					settings.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
					NetworkCredential creds = CredentialCache.DefaultCredentials as NetworkCredential;
					ClaimsPrincipalSelector claimsPrincipalSelector = (clientIDFromToken == string.Empty ? new ClaimsPrincipalSelector(creds, settings, null) : new ClaimsPrincipalSelector(creds, settings, customClaims));
					claimsPrincipalSelector.GetClaimsPrincipal();
					ClaimsPrincipal.ClaimsPrincipalSelector = new Func<ClaimsPrincipal>(claimsPrincipalSelector.GetClaimsPrincipal);
					if (claimsPrincipalSelector.get_ClaimsPrincipal() != null)
					{
						claimsPrincipalSelector.get_ClaimsPrincipal().set_ActAsToken(token);
					}
					ServiceHelperFactory.get_Instance().set_StsClientConfiguration(settings);
					ServiceHelperFactory.get_Instance().set_StsUserCredentials(creds);
				}
			}
			else
			{
				Program.logger.Error("Client configurations not found");
			}
		}

		private static void InitializeSystemSecurityContext()
		{
			SettingsSecureFileStore securitySettings = new SettingsSecureFileStore();
			ServiceConfiguration configs = securitySettings.GetConfigurations();
			if (configs != null)
			{
				ServiceHelperFactory.get_Instance().set_BaseUri(configs.get_ServiceBaseUrl());
				ClientConfiguration settings = securitySettings.GetClientSettingsFromConfigDll();
				if (settings == null)
				{
					Program.logger.Error("Client configurations for security service not found");
				}
				if (settings != null)
				{
					settings.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
					NetworkCredential creds = CredentialCache.DefaultCredentials as NetworkCredential;
					ClaimsPrincipalSelector claimsPrincipalSelector = new ClaimsPrincipalSelector(creds, settings, null);
					claimsPrincipalSelector.GetClaimsPrincipal();
					ClaimsPrincipal.ClaimsPrincipalSelector = new Func<ClaimsPrincipal>(claimsPrincipalSelector.GetClaimsPrincipal);
					if (claimsPrincipalSelector.get_ClaimsPrincipal() != null)
					{
						claimsPrincipalSelector.get_ClaimsPrincipal().set_ActAsToken(ActiveClient.GetActAsToken(creds, settings, claimsPrincipalSelector.get_ClaimsPrincipal()));
					}
					ServiceHelperFactory.get_Instance().set_StsClientConfiguration(settings);
					ServiceHelperFactory.get_Instance().set_StsUserCredentials(creds);
				}
			}
			else
			{
				Program.logger.Error("Client configurations not found");
			}
		}

		private static void Main(string[] args)
		{
			Console.Title = "Task Scheduler Runner";
			args = new string[] { "0AA##0PV7M#AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAABOimvGWmYk+OwElU3xgGewQAAAACAAAAAAADZgAAwAAAABAAAAA+yg5rxV+6laCj+NmjyzUUAAAAAASAAACgAAAAEAAAALBk/IjhAvXRv7AKRVRSFxIIAAAA0wsOxFN3pAAUAAAALS5bfTs3WJzAPK4Tn+V3EK5mLC8=Ukx5N0AlazM=" };
			Console.WriteLine("Press any key to start...");
			Console.ReadKey(false);
			try
			{
				try
				{
					Console.WriteLine("Program Started");
					LogExtension.RegisterCustomLogLevels();
					XmlConfigurator.Configure();
					LogExtension.EnterMethod(Program.logger, MethodBase.GetCurrentMethod(), args);
					string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Schedules");
					Console.WriteLine(string.Concat("Schedules Path: ", path));
					int jobId = Convert.ToInt32(CryptographyHelper.DecryptFromLocalMachine(args[0]));
					Console.WriteLine(string.Format("Job ID: {0}", jobId));
					path = Path.Combine(path, string.Concat("task", jobId.ToString(), ".txt"));
					if (!File.Exists(path))
					{
						Console.WriteLine(string.Concat("Path ", path, " does not exists."));
						Program.InitializeSystemSecurityContext();
						Helper.IsSystemSecurityContext = true;
					}
					else
					{
						Console.WriteLine(string.Concat("Path ", path, " exists."));
						using (StreamReader data = File.OpenText(path))
						{
							string[] container = data.ReadToEnd().Split(new string[] { "<#!#>" }, StringSplitOptions.None);
							jobId = Convert.ToInt32(CryptographyHelper.DecryptFromLocalMachine(container[0]));
							Console.WriteLine(string.Format("Job ID from job file: {0}", jobId));
							Program.InitializeSecurityContext(CryptographyHelper.DecryptFromLocalMachine(container[2]));
							Helper.IsSystemSecurityContext = false;
						}
					}
					Console.WriteLine("System security context initialized");
					LicensingProvider licensingProvider = new LicensingProvider();
					if ((licensingProvider.HasValidProductLicense(1) ? false : !licensingProvider.HasValidProductLicense(3)))
					{
						if (!licensingProvider.HasValidProductLicense(8))
						{
							Console.WriteLine("License error. Returning...");
							Program.logger.Error("Invalid License");
							return;
						}
						else
						{
							TaskScheduling task = (new ServicesSchedulingServiceClient(true)).GetScheduledJob((long)jobId);
							if (task == null)
							{
								return;
							}
							else if (task.get_JobType() != 13)
							{
								Program.logger.Error("Invalid License");
								Console.WriteLine("License error. Returning...");
								return;
							}
						}
					}
					JobProcessor jobProcessor = new JobProcessor();
					Console.WriteLine(string.Format("Ready to process job {0}", jobId));
					jobProcessor.ProcessJob((long)jobId);
					Console.WriteLine(string.Format("Job {0} processed.", jobId));
				}
				catch (CryptographicException cryptographicException1)
				{
					CryptographicException cryptographicException = cryptographicException1;
					string message = string.Concat(cryptographicException.Message, " Error in initializing security context for the Scheduled job. Possible reason may be, that a required windows service is not running. Please make sure that 'CNG Key Isolation' windows service is running.");
					Console.WriteLine(string.Format("Exception {0}: {1}. Details: {2}. Trace:", cryptographicException.GetType(), cryptographicException.Message, message));
					Console.WriteLine(cryptographicException.StackTrace);
					LogExtension.LogException(Program.logger, message, cryptographicException, 1, "Logging.Const.LoggingConstants.jobUpdate");
				}
				catch (Exception exception)
				{
					Exception ex = exception;
					Console.WriteLine(string.Format("Exception {0}: {1}. Trace:", ex.GetType(), ex.Message));
					Console.WriteLine(ex.StackTrace);
					LogExtension.LogException(Program.logger, ex.Message, ex, 1, "Logging.Const.LoggingConstants.jobUpdate");
				}
			}
			finally
			{
				LogExtension.ExitMethod(Program.logger, MethodBase.GetCurrentMethod(), args);
			}
		}
	}
}