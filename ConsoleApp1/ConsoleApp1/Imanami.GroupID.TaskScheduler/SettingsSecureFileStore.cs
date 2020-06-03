using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.Foundation.Security;
using Imanami.GroupID.DataTransferObjects.UtilityObjects;
using Imanami.Serialization.Helpers;
using Imanami.STS.Core.Configuration;
using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.IO;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler
{
	public class SettingsSecureFileStore
	{
		private readonly static ILog logger;

		private static string SecuritySettingsFileName;

		private static string filePath;

		public static string FilePath
		{
			get
			{
				if (StringUtility.IsBlank(SettingsSecureFileStore.filePath))
				{
					string str = "Software\\Imanami\\GroupID\\";
					string str1 = "Version 10.0";
					try
					{
						RegObject regObject = new RegObject(str1);
						regObject.LoadFromLocalMachine(str);
						SettingsSecureFileStore.filePath = regObject.GetAttributeAsString("Path");
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str2 = string.Format("Trying by getting path of executing assembly because installation path was not found in the registry. Reason: {0}", exception.Message);
						SettingsSecureFileStore.logger.Error(str2, exception);
					}
					if (StringUtility.IsBlank(SettingsSecureFileStore.filePath))
					{
						SettingsSecureFileStore.filePath = SerializationHelper.NormalizePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase));
					}
					SettingsSecureFileStore.filePath = Path.Combine(SettingsSecureFileStore.filePath, SettingsSecureFileStore.SecuritySettingsFileName);
				}
				return SettingsSecureFileStore.filePath;
			}
			set
			{
				SettingsSecureFileStore.filePath = value;
			}
		}

		static SettingsSecureFileStore()
		{
			<>z__a_4.Initialize();
			SettingsSecureFileStore.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			SettingsSecureFileStore.SecuritySettingsFileName = "ImanamiConfig.config";
			SettingsSecureFileStore.filePath = string.Empty;
		}

		public SettingsSecureFileStore()
		{
		}

		public ClientConfiguration GetClientSettingsFromConfigDll()
		{
			ClientConfiguration returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339649533L),
				Method = <>z__a_4._3
			};
			<>z__a_4.a35.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						ServiceConfiguration configurations = this.GetConfigurations();
						ClientConfiguration clientConfiguration = new ClientConfiguration();
						clientConfiguration.set_TrustedIssuerName(configurations.get_STSThumbprintName());
						clientConfiguration.set_TrustedIssuerThumbprint(configurations.get_STSThumbprint());
						clientConfiguration.set_STSActiveModeUrl(configurations.get_ActAsServiceUrl());
						clientConfiguration.set_ClientUrl(string.Format("http://{0}/groupidglm", Environment.MachineName));
						clientConfiguration.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
						returnValue = clientConfiguration;
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_4.a35.OnException(methodExecutionArg);
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
								returnValue = (ClientConfiguration)methodExecutionArg.ReturnValue;
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
					<>z__a_4.a35.OnExit(methodExecutionArg);
					returnValue = (ClientConfiguration)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (ClientConfiguration)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}

		public ServiceConfiguration GetConfigurations()
		{
			ServiceConfiguration serviceConfiguration;
			ServiceConfiguration returnValue = null;
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, Arguments.Empty)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339649534L),
				Method = <>z__a_4._1
			};
			<>z__a_4.a34.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						LogExtension.EnterMethod(SettingsSecureFileStore.logger, MethodBase.GetCurrentMethod());
						try
						{
							serviceConfiguration = ServiceConfiguration.FromXml(CryptographyHelper.DecryptFromLocalMachine(SerializationHelper.ReadFromFile(SettingsSecureFileStore.FilePath)));
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							SettingsSecureFileStore.logger.Error(exception.Message, exception);
							LogExtension.EnterMethod(SettingsSecureFileStore.logger, MethodBase.GetCurrentMethod());
							returnValue = null;
							return returnValue;
						}
						returnValue = serviceConfiguration;
					}
					catch (Exception exception2)
					{
						methodExecutionArg.Exception = exception2;
						<>z__a_4.a34.OnException(methodExecutionArg);
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
								returnValue = (ServiceConfiguration)methodExecutionArg.ReturnValue;
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
					<>z__a_4.a34.OnExit(methodExecutionArg);
					returnValue = (ServiceConfiguration)methodExecutionArg.ReturnValue;
				}
			}
			else
			{
				returnValue = (ServiceConfiguration)methodExecutionArg.ReturnValue;
			}
			return returnValue;
		}
	}
}