using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler.Glm
{
	public class JobProcessor
	{
		private static ILog logger;

		static JobProcessor()
		{
			Imanami.GroupID.TaskScheduler.Glm.JobProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public JobProcessor()
		{
		}

		public void ProcessJob(long jobId)
		{
			this.ProcessJob((new ServicesSchedulingServiceClient(true)).GetScheduledJob(jobId));
		}

		public void ProcessJob(TaskScheduling task)
		{
			(new ExpiredGroupsProcessor()).DeleteTheExpiredGroupsWhichAreDueForDeletion();
			ExpiringGroupsProcessor expiringGroupsProcessor = new ExpiringGroupsProcessor();
			expiringGroupsProcessor.ExpireTheGroupsWhichAreDueForExpiry();
			EligibleGroupsForLifeExtensionProcessor eligibleGroupsForLifeExtensionProcessor = new EligibleGroupsForLifeExtensionProcessor();
			eligibleGroupsForLifeExtensionProcessor.ExtendEligibleGroupsLife();
			if (eligibleGroupsForLifeExtensionProcessor.ExtendedGroups.Count > 0)
			{
				expiringGroupsProcessor.ExtendedGroups = eligibleGroupsForLifeExtensionProcessor.ExtendedGroups;
			}
			expiringGroupsProcessor.SendNotificationToExpiringGroups();
			Imanami.GroupID.TaskScheduler.Glm.JobProcessor.logger.InfoFormat("Processed job.", Array.Empty<object>());
		}
	}
}