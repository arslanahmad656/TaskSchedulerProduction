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
			Dictionary<string, Dictionary<int, string>> strs1;
			try
			{
				try
				{
					PermissionAnalyzerConfigurationService permissionAnalyzerConfigurationService = new PermissionAnalyzerConfigurationService(store.get_IdentityStoreId());
					Dictionary<string, Dictionary<int, string>> strs2 = new Dictionary<string, Dictionary<int, string>>();
					IdentityStorePermissionAnalyzerConfiguration permissionConfigurations = permissionAnalyzerConfigurationService.GetPermissionConfigurations();
					if (permissionConfigurations != null && permissionConfigurations.get_Servers() != null && permissionConfigurations.get_Servers().Count > 0)
					{
						if (permissionConfigurations.get_IncludeFutureServers())
						{
							List<PermissionAnalyzerServer> criteriaBasedServers = permissionAnalyzerConfigurationService.GetCriteriaBasedServers(permissionConfigurations);
							IEnumerable<string> strs3 = (
								from x in criteriaBasedServers
								select x.get_ServerID()).Except<string>(
								from y in permissionConfigurations.get_Servers()
								select y.get_ServerID());
							IEnumerable<PermissionAnalyzerServer> permissionAnalyzerServers = 
								from server in criteriaBasedServers
								where strs3.Any<string>((string id) => server.get_ServerID().Equals(id, StringComparison.InvariantCultureIgnoreCase))
								select server;
							if (permissionAnalyzerServers != null && permissionAnalyzerServers.Count<PermissionAnalyzerServer>() > 0)
							{
								permissionConfigurations.get_Servers().AddRange(permissionAnalyzerServers);
							}
						}
						if (Helper.CurrentTask.get_Targets() != null && Helper.CurrentTask.get_Targets().Count > 0)
						{
							permissionConfigurations.set_Servers((
								from server in permissionConfigurations.get_Servers()
								where Helper.CurrentTask.get_Targets().Any<SchedulingTarget>((SchedulingTarget target) => target.get_Target().Equals(server.get_ServerID(), StringComparison.InvariantCultureIgnoreCase))
								select server).ToList<PermissionAnalyzerServer>());
							Helper.CurrentTask.get_Targets().Clear();
						}
						permissionConfigurations.get_Servers().ForEach((PermissionAnalyzerServer server) => {
							if (permissionConfigurations.get_ExcludedServers() != null && permissionConfigurations.get_ExcludedServers().Any<string>((string excludedServer) => excludedServer.Equals(server.get_ServerID(), StringComparison.InvariantCultureIgnoreCase)))
							{
								return;
							}
							if (!string.IsNullOrEmpty(server.get_ScheduleJob()) && !server.get_ScheduleJob().Equals(PermissionAnalyzerConfigurationService.GetScheduleName()))
							{
								return;
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
							if (server.get_FileShare() != null && server.get_FileShare().get_Shares() != null)
							{
								List<string> strs = new List<string>();
								if (!server.get_IsServiceAccountConfigured())
								{
									server.get_Credentials().set_Password(CryptographyHelper.DecryptFromLocalMachine(server.get_Credentials().get_Password()));
								}
								try
								{
									strs = permissionAnalyzerConfigurationService.GetNetworkShareResourcesList(server.get_Credentials());
								}
								catch (Exception exception)
								{
								}
								if (!server.get_IsServiceAccountConfigured())
								{
									server.get_Credentials().set_Password(CryptographyHelper.EncryptForLocalMachine(server.get_Credentials().get_Password()));
								}
								strs.ForEach((string latestShare) => {
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
							this.SetConfigurations(store, server, strs2);
							servers.Add(server);
						});
					}
					strs1 = strs2;
				}
				catch (Exception exception2)
				{
					Exception exception1 = exception2;
					LogExtension.LogException(PermissionAnalyzer.logger, exception1.Message, exception1);
					return new Dictionary<string, Dictionary<int, string>>();
				}
			}
			finally
			{
				(new ServicesSchedulingServiceClient(false)).Update(Helper.CurrentTask);
			}
			return strs1;
		}

		public void ProcessJob(long jobId)
		{
			this.ProcessJob((new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId));
		}

		public void ProcessJob(TaskScheduling task)
		{
			try
			{
				ServicesAdministrationServiceClient servicesAdministrationServiceClient = new ServicesAdministrationServiceClient(true);
				ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
				IdentityStore identityStoreById = servicesAdministrationServiceClient.GetIdentityStoreById(task.get_IdentityStoreId(), true);
				KnownAttributes knownAttributes = servicesSearchServiceClient.GetKnownAttributes(task.get_IdentityStoreId());
				List<PermissionAnalyzerServer> permissionAnalyzerServers = new List<PermissionAnalyzerServer>();
				Dictionary<string, Dictionary<int, string>> strs = this.LoadConfigurations(identityStoreById, servicesAdministrationServiceClient, permissionAnalyzerServers, knownAttributes);
				List<Schema> identityStoreSchema = servicesAdministrationServiceClient.GetIdentityStoreSchema(task.get_IdentityStoreId());
				if (permissionAnalyzerServers.Count > 0)
				{
					(new Imanami.PermissionReplicationService.PermissionReplicationService(identityStoreById, strs, identityStoreSchema, knownAttributes)).ReplicatePermissions(1, permissionAnalyzerServers);
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
			Dictionary<int, string> nums = new Dictionary<int, string>()
			{
				{ 3, server.get_Credentials().get_UserName() },
				{ 4, server.get_Credentials().get_Password() },
				{ 5, _store.get_ConnectionString() }
			};
			Configurations.Add(server.get_Server().ToLower(), nums);
		}
	}
}