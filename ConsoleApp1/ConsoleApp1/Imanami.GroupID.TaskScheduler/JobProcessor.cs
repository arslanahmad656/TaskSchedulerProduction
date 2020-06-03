using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using Imanami.GroupID.TaskScheduler.Glm;
using Imanami.GroupID.TaskScheduler.GUS;
using Imanami.GroupID.UserLifeCycleManagment.Helpers;
using Imanami.GroupID.UserLifeCycleManagment.Job;
using log4net;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			Imanami.GroupID.TaskScheduler.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessJob(long jobId)
		{
			TaskScheduling task = (new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId);
			Console.WriteLine(string.Concat("Task ", task.get_TaskSchedulerJobName(), " obtained using scheduling client."));
			Console.WriteLine(string.Format("Job ID: {0}", jobId));
			Imanami.GroupID.TaskScheduler.Helper.CurrentTask = task;
			if (Imanami.GroupID.TaskScheduler.Helper.CurrentTask != null)
			{
				if ((Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() == 6 || !Imanami.GroupID.TaskScheduler.Helper.IsSystemSecurityContext || Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() == 8 ? false : Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 13))
				{
					string exMsg = string.Format("Unable to proceed. Authentication information has been expired for job {0} - {1}.", task.get_JobId(), task.get_Name());
					Console.WriteLine(string.Concat("Throwing exception: ", exMsg));
					throw new Exception(exMsg);
				}
				ServicesSearchServiceClient configurationService = new ServicesSearchServiceClient(false);
				Imanami.GroupID.TaskScheduler.Helper.AppConfiguration = configurationService.GetAppConfiguration(Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_IdentityStoreId());
				if (Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobType() != 6)
				{
					Imanami.GroupID.TaskScheduler.Helper.KnownProviderAttributes = configurationService.GetKnownAttributes(Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_IdentityStoreId());
				}
				Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobId(), Imanami.GroupID.TaskScheduler.Helper.CurrentTask.get_JobName());
				if (task.get_JobType() == 9)
				{
					IUserLifeCycleJob userLifeCycleProcessor = Imanami.GroupID.UserLifeCycleManagment.Helpers.Helper.GetUserLifeCycleJobProcessor();
					Imanami.GroupID.UserLifeCycleManagment.Helpers.Helper.set_KnownProviderAttributes(Imanami.GroupID.TaskScheduler.Helper.KnownProviderAttributes);
					userLifeCycleProcessor.Process(task);
				}
				else if (task.get_JobType() == 1)
				{
					Console.WriteLine(string.Format("Job type is {0}", task.get_JobType()));
					SmartGroupJobProcessor processor = new SmartGroupJobProcessor();
					Console.WriteLine("Ready to process smart group job");
					processor.ProcessSmartGroupUpdate(task);
				}
				else if (task.get_JobType() == 5)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					MembershipJob.RunMembershipLifeCycle(task.get_IdentityStoreId());
				}
				else if (task.get_JobType() == 11)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					ManagedByJobs.RunManagedByLifeCycle(task.get_IdentityStoreId());
				}
				else if (task.get_JobType() == 4)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new Imanami.GroupID.TaskScheduler.Glm.JobProcessor()).ProcessJob(task);
				}
				else if (task.get_JobType() == 6)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new SchemaReplicationProcessor()).ProcessJob(task);
				}
				else if (task.get_JobType() == 7)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new Imanami.GroupID.TaskScheduler.GUS.JobProcessor()).ProcessGroupUsage();
				}
				else if (task.get_JobType() == 8)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new ServicesSchedulingServiceClient(false)).HistoryRetention(task);
				}
				else if (task.get_JobType() == 10)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new OrphanGroupProcessor()).ProcessJob(task);
				}
				else if (task.get_JobType() == 12)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new WorkflowApproverAccelerationProcessor()).ProcessJob(task);
				}
				else if (task.get_JobType() == 13)
				{
					Imanami.GroupID.TaskScheduler.JobProcessor.logger.InfoFormat("Processing job: {0}, Name: {1}", task.get_JobId(), task.get_JobName());
					(new PermissionAnalyzer()).ProcessJob(task);
				}
			}
		}
	}
}