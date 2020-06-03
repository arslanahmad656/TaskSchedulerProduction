using Imanami.Data.ServiceClient;
using Imanami.GroupID.DataTransferObjects.DataContracts;
using Imanami.GroupID.DataTransferObjects.DataContracts.Services.Scheduling;
using log4net;
using System;
using System.Reflection;

namespace Imanami.GroupID.TaskScheduler
{
	public class WorkflowApproverAccelerationProcessor
	{
		private static ILog logger;

		static WorkflowApproverAccelerationProcessor()
		{
			WorkflowApproverAccelerationProcessor.logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public WorkflowApproverAccelerationProcessor()
		{
		}

		public void ProcessJob(TaskScheduling task)
		{
			try
			{
				ActionResult result = (new WorkflowManagerServiceClient(false)).ProcessApproverAcceleration(task.get_IdentityStoreId());
				WorkflowApproverAccelerationProcessor.logger.Debug(1023, 10334, string.Format("Workflow approver acceleration result: {0}. {1}", result.get_Status(), result.get_Message()), null, null);
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				string msg = string.Concat("Unable to accelerate the workflow approvers. ", ex.Message);
				WorkflowApproverAccelerationProcessor.logger.Error(1023, 10334, msg, ex, null);
			}
		}
	}
}