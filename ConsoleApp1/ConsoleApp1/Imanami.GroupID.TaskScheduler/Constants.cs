using Imanami.STS.Core.Utilities;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Imanami.GroupID.TaskScheduler
{
	public class Constants
	{
		public const int DefaultPageSize = 20000;

		public static X509Certificate2 DefaultCertificate;

		public const string ExchangeSecurityGroupContainerName = "Microsoft Exchange Security Groups";

		public const string ExchangeSystemObjectsContainerName = "Microsoft Exchange System Objects";

		static Constants()
		{
			Constants.DefaultCertificate = CertificateUtil.GetCertificate(StoreName.My, StoreLocation.LocalMachine, "CN=GroupIDSecurityService");
		}

		public Constants()
		{
		}
	}
}