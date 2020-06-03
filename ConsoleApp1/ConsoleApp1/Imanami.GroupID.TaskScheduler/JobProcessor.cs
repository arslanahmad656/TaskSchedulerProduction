using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.TaskScheduler.Glm;
using Imanami.GroupID.TaskScheduler.GUS;
using Imanami.GroupID.UserLifeCycleManagment.Helpers;
using Imanami.GroupID.UserLifeCycleManagment.Job;
using log4net;
using log4net.Attributes;
using PostSharp.Aspects;
using PostSharp.Aspects.Internals;
using PostSharp.ImplementationDetails_bda91a0d;
using PostSharp.Reflection;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			<>z__a_5.Initialize();
			Imanami.GroupID.TaskScheduler.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessJob(long jobId)
		{
			Arguments<long> argument = new Arguments<long>()
			{
				Arg0 = jobId
			};
			MethodExecutionArgs methodExecutionArg = new MethodExecutionArgs(this, argument)
			{
				DeclarationIdentifier = new DeclarationIdentifier(-4780260886339584000L),
				Method = <>z__a_5._1
			};
			<>z__a_5.a36.OnEntry(methodExecutionArg);
			if (methodExecutionArg.FlowBehavior != FlowBehavior.Return)
			{
				try
				{
					try
					{
						TaskScheduling scheduledJob = (new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId);
						Imanami.GroupID.TaskScheduler.Helper.CurrentTask = scheduledJob;
						if (Imanami.GroupID.TaskScheduler.Helper.CurrentTask != null)
						{
							if (Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 6 && Imanami.GroupID.TaskScheduler.Helper.IsSystemSecurityContext && Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 8 && Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 13)
							{
								throw new Exception(string.Format("Unable to proceed. Authentication information has been expired for job {0} - {1}.", scheduledJob.get_JobId(), scheduledJob.get_Name()));
							}
							ServicesSearchServiceClient servicesSearchServiceClient = new ServicesSearchServiceClient(false);
							Imanami.GroupID.TaskScheduler.Helper.AppConfiguration = servicesSearchServiceClient.GetAppConfiguration(Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_IdentityStoreId());
							if (Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 6)
							{
								Imanami.GroupID.TaskScheduler.Helper.KnownProviderAttributes = servicesSearchServiceClient.GetKnownAttributes(Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_IdentityStoreId());
							}
							Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobId(), Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobName());
							if (scheduledJob.get_JobType() == 9)
							{
								IUserLifeCycleJob userLifeCycleJobProcessor = Imanami.GroupID.UserLifeCycleManagment.Helpers.Helper.GetUserLifeCycleJobProcessor();
								Imanami.GroupID.UserLifeCycleManagment.Helpers.Helper.set_KnownProviderAttributes(Imanami.GroupID.TaskScheduler.Helper.KnownProviderAttributes);
								userLifeCycleJobProcessor.Process(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 1)
							{
								(new SmartGroupJobProcessor()).ProcessSmartGroupUpdate(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 5)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								MembershipJob.RunMembershipLifeCycle(scheduledJob.get_IdentityStoreId());
							}
							else if (scheduledJob.get_JobType() == 11)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								ManagedByJobs.RunManagedByLifeCycle(scheduledJob.get_IdentityStoreId());
							}
							else if (scheduledJob.get_JobType() == 4)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new Imanami.GroupID.TaskScheduler.Glm.JobProcessor()).ProcessJob(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 6)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new SchemaReplicationProcessor()).ProcessJob(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 7)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new Imanami.GroupID.TaskScheduler.GUS.JobProcessor()).ProcessGroupUsage();
							}
							else if (scheduledJob.get_JobType() == 8)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new ServicesSchedulingServiceClient(false)).HistoryRetention(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 10)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new OrphanGroupProcessor()).ProcessJob(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 12)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new WorkflowApproverAccelerationProcessor()).ProcessJob(scheduledJob);
							}
							else if (scheduledJob.get_JobType() == 13)
							{
								Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", scheduledJob.get_JobId(), scheduledJob.get_JobName());
								(new PermissionAnalyzer()).ProcessJob(scheduledJob);
							}
						}
					}
					catch (Exception exception)
					{
						methodExecutionArg.Exception = exception;
						<>z__a_5.a36.OnException(methodExecutionArg);
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
					<>z__a_5.a36.OnExit(methodExecutionArg);
				}
			}
		}
	}
}