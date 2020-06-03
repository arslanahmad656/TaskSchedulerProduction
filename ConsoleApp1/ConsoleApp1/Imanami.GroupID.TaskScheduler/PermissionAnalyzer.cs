using Imanami.Data.ServiceClient;
using Imanami.Foundation;
using Imanami.Foundation.Security;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Replication;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.PermissionAnalyzer.Engine.Services;
using Imanami.PermissionReplicationService;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Imanami.GroupID.TaskScheduler
{
	internal class PermissionAnalyzer
	{
		private static ILog logger;

		static PermissionAnalyzer()
		{
			PermissionAnalyzer.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public PermissionAnalyzer()
		{
		}

		public Dictionary<string, Dictionary<int, string>> LoadConfigurations(IdentityStore store, ServicesAdministrationServiceClient _client, List<PermissionAnalyzerServer> servers, KnownAttributes knownAttributes)
		{
			Dictionary<string, Dictionary<int, string>> strs;
			try
			{
				try
				{
					PermissionAnalyzerConfigurationService permissionAnalyzerConfigurationService = new PermissionAnalyzerConfigurationService(store.get_IdentityStoreId());
					Dictionary<string, Dictionary<int, string>> strs1 = new Dictionary<string, Dictionary<int, string>>();
					IdentityStorePermissionAnalyzerConfiguration permissionConfigurations = permissionAnalyzerConfigurationService.GetPermissionConfigurations();
					if ((permissionConfigurations == null || permissionConfigurations.get_Servers() == null ? false : permissionConfigurations.get_Servers().Count > 0))
					{
						if (permissionConfigurations.get_IncludeFutureServers())
						{
							List<PermissionAnalyzerServer> allServers = permissionAnalyzerConfigurationService.GetCriteriaBasedServers(permissionConfigurations);
							IEnumerable<string> strs2 = (
								from x in allServers
								select x.get_ServerID()).Except<string>(
								from y in permissionConfigurations.get_Servers()
								select y.get_ServerID());
							IEnumerable<PermissionAnalyzerServer> latestServers = 
								from server in allServers
								where strs2.Any<string>((string id) => server.get_ServerID().Equals(id, StringComparison.InvariantCultureIgnoreCase))
								select server;
							if ((latestServers == null ? false : latestServers.Count<PermissionAnalyzerServer>() > 0))
							{
								permissionConfigurations.get_Servers().AddRange(latestServers);
							}
						}
						if ((Helper.CurrentTask.get_Targets() == null ? false : Helper.CurrentTask.get_Targets().Count > 0))
						{
							permissionConfigurations.set_Servers((
								from server in permissionConfigurations.get_Servers()
								where Helper.CurrentTask.get_Targets().Any<SchedulingTarget>((SchedulingTarget target) => target.get_Target().Equals(server.get_ServerID(), StringComparison.InvariantCultureIgnoreCase))
								select server).ToList<PermissionAnalyzerServer>());
							Helper.CurrentTask.get_Targets().Clear();
						}
						permissionConfigurations.get_Servers().ForEach((PermissionAnalyzerServer server) => {
							if (permissionConfigurations.get_ExcludedServers() != null)
							{
								if (permissionConfigurations.get_ExcludedServers().Any<string>((string excludedServer) => excludedServer.Equals(server.get_ServerID(), StringComparison.InvariantCultureIgnoreCase)))
								{
									return;
								}
							}
							if (!string.IsNullOrEmpty(server.get_ScheduleJob()))
							{
								if (!server.get_ScheduleJob().Equals(PermissionAnalyzerConfigurationService.GetScheduleName()))
								{
									return;
								}
							}
							if (server.get_Credentials() == null)
							{
								server.set_Credentials(new PermissionAnalyzerServerCredentials());
							}
							server.get_Credentials();
							if (server.get_IsServiceAccountConfigured())
							{
								server.get_Credentials().set_UserName(store.get_IdentityStoreConfigurationValues()["UserName"]);
								server.get_Credentials().set_Password(store.get_IdentityStoreConfigurationValues()["Password"]);
							}
							server.get_Credentials().set_ServerName(server.get_Server());
							if ((server.get_FileShare() == null ? false : server.get_FileShare().get_Shares() != null))
							{
								List<string> serverShares = new List<string>();
								if (!server.get_IsServiceAccountConfigured())
								{
									server.get_Credentials().set_Password(CryptographyHelper.DecryptFromLocalMachine(server.get_Credentials().get_Password()));
								}
								try
								{
									serverShares = permissionAnalyzerConfigurationService.GetNetworkShareResourcesList(server.get_Credentials());
								}
								catch (Exception exception)
								{
								}
								if (!server.get_IsServiceAccountConfigured())
								{
									server.get_Credentials().set_Password(CryptographyHelper.EncryptForLocalMachine(server.get_Credentials().get_Password()));
								}
								serverShares.ForEach((string latestShare) => {
									if (!server.get_FileShare().get_Shares().Any<PermissionAnalyzerServerShare>((PermissionAnalyzerServerShare x) => x.get_ShareID().Equals(latestShare, StringComparison.InvariantCultureIgnoreCase)))
									{
										List<PermissionAnalyzerServerShare> shares = server.get_FileShare().get_Shares();
										PermissionAnalyzerServerShare permissionAnalyzerServerShare = new PermissionAnalyzerServerShare();
										permissionAnalyzerServerShare.set_IsSelected(true);
										permissionAnalyzerServerShare.set_Share(latestShare);
										permissionAnalyzerServerShare.set_ShareID(latestShare);
										shares.Add(permissionAnalyzerServerShare);
									}
								});
							}
							this.SetConfigurations(store, server, strs1);
							servers.Add(server);
						});
					}
					strs = strs1;
					return strs;
				}
				catch (Exception exception1)
				{
					Exception ex = exception1;
					LogExtension.LogException(PermissionAnalyzer.logger, ex.Message, ex);
				}
			}
			finally
			{
				(new ServicesSchedulingServiceClient(false)).Update(Helper.CurrentTask);
			}
			strs = new Dictionary<string, Dictionary<int, string>>();
			return strs;
		}

		public void ProcessJob(long jobId)
		{
			this.ProcessJob((new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId));
		}

		public void ProcessJob(TaskScheduling task)
		{
			try
			{
				ServicesAdministrationServiceClient adminClient = new ServicesAdministrationServiceClient(true);
				ServicesSearchServiceClient searchClient = new ServicesSearchServiceClient(false);
				IdentityStore store = adminClient.GetIdentityStoreById(task.get_IdentityStoreId(), true);
				KnownAttributes knownAttributes = searchClient.GetKnownAttributes(task.get_IdentityStoreId());
				List<PermissionAnalyzerServer> servers = new List<PermissionAnalyzerServer>();
				Dictionary<string, Dictionary<int, string>> configurations = this.LoadConfigurations(store, adminClient, servers, knownAttributes);
				List<Schema> schema = adminClient.GetIdentityStoreSchema(task.get_IdentityStoreId());
				if (servers.Count > 0)
				{
					(new Imanami.PermissionReplicationService.PermissionReplicationService(store, configurations, schema, knownAttributes)).ReplicatePermissions(1, servers);
				}
			}
			catch (Exception exception)
			{
				LogExtension.LogException(PermissionAnalyzer.logger, "Error While Replicating Permissions.", exception);
			}
			PermissionAnalyzer.logger.InfoFormat("Job processed successfully.", Array.Empty<object>());
		}

		private void SetConfigurations(IdentityStore _store, PermissionAnalyzerServer server, Dictionary<string, Dictionary<int, string>> Configurations)
		{
			Dictionary<int, string> config = new Dictionary<int, string>()
			{
				{ 3, server.get_Credentials().get_UserName() },
				{ 4, server.get_Credentials().get_Password() },
				{ 5, _store.get_ConnectionString() }
			};
			Configurations.Add(server.get_Server().ToLower(), config);
		}
	}
}