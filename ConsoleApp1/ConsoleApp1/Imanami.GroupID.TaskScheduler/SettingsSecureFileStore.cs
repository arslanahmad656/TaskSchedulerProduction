using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.Foundation.Security;
using Imanami.GroupID.DataTransferObjects.UtilityObjects;
using Imanami.Serialization.Helpers;
using Imanami.STS.Core.Configuration;
using log4net;
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
					string REG_PATH = "Software\\Imanami\\GroupID\\";
					string REG_KEY = "Version 10.0";
					try
					{
						RegObject regObject = new RegObject(REG_KEY);
						regObject.LoadFromLocalMachine(REG_PATH);
						SettingsSecureFileStore.filePath = regObject.GetAttributeAsString("Path");
					}
					catch (Exception exception)
					{
						Exception ex = exception;
						string message = string.Format("Trying by getting path of executing assembly because installation path was not found in the registry. Reason: {0}", ex.Message);
						SettingsSecureFileStore.logger.Error(message, ex);
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
			SettingsSecureFileStore.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			SettingsSecureFileStore.SecuritySettingsFileName = "ImanamiConfig.config";
			SettingsSecureFileStore.filePath = string.Empty;
		}

		public SettingsSecureFileStore()
		{
		}

		public ClientConfiguration GetClientSettingsFromConfigDll()
		{
			ServiceConfiguration config = this.GetConfigurations();
			ClientConfiguration clientConfiguration = new ClientConfiguration();
			clientConfiguration.set_TrustedIssuerName(config.get_STSThumbprintName());
			clientConfiguration.set_TrustedIssuerThumbprint(config.get_STSThumbprint());
			clientConfiguration.set_STSActiveModeUrl(config.get_ActAsServiceUrl());
			clientConfiguration.set_ClientUrl(string.Format("http://{0}/groupidglm", Environment.MachineName));
			clientConfiguration.set_ActAsClientUrl(ServiceHelperFactory.get_Instance().get_BaseUri());
			return clientConfiguration;
		}

		public ServiceConfiguration GetConfigurations()
		{
			ServiceConfiguration serviceConfiguration;
			LogExtension.EnterMethod(SettingsSecureFileStore.logger, MethodBase.GetCurrentMethod());
			try
			{
				string encXml = SerializationHelper.ReadFromFile(SettingsSecureFileStore.FilePath);
				serviceConfiguration = ServiceConfiguration.FromXml(CryptographyHelper.DecryptFromLocalMachine(encXml));
				return serviceConfiguration;
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				SettingsSecureFileStore.logger.Error(ex.Message, ex);
			}
			LogExtension.EnterMethod(SettingsSecureFileStore.logger, MethodBase.GetCurrentMethod());
			serviceConfiguration = null;
			return serviceConfiguration;
		}
	}
}