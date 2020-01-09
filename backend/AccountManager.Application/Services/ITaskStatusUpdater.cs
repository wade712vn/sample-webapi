using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AccountManager.Domain.Entities;
using AccountManager.Application.Common.Interfaces;

namespace AccountManager.Application.Services
{
    public interface ITaskStatusUpdater
    {
        Task UpdateStatus();
        void SetTask(SaasTaskBase task);
    }

    public interface ITaskStatusUpdater<T> : ITaskStatusUpdater where T : SaasTaskBase
    {
        void SetTask(T task);
    }

    public class CreateSiteTaskStatusUpdater : ITaskStatusUpdater<CreateSiteTask>
    {
        private static readonly string[] OperationList = new[]
        {
            "STRT",
            "CDNS",
            "USER",
            "UDEP",
            "USIT",
            "UCLT",
            "UREL",
            "UPOP",
            "CONV",
            "MNT-",
            "BIND",
            "PSIT",

        };

        private readonly IAccountManagerDbContext _context;
        private CreateSiteTask _task;

        public CreateSiteTaskStatusUpdater(IAccountManagerDbContext context)
        {
            _context = context;
        }

        public void SetTask(CreateSiteTask task)
        {
            _task = task;
        }

        public void SetTask(SaasTaskBase task)
        {
            SetTask(task as CreateSiteTask);
        }

        public async Task UpdateStatus()
        {
            Machine machine;
            if (_task?.MachineId == null || 
                (machine = await _context.Set<Machine>().FirstOrDefaultAsync(x => x.Id == _task.MachineId.Value)) == null)
            {
                return;
            }

            var machineId = _task.MachineId.Value;

            var operations = await _context.Set<Operation>().Include(x => x.Type).Where(x => x.MachineId == machineId)
                .OrderBy(x => x.Timestamp).ToListAsync();

            if (!operations.Any())
            {
                _task.Status = SaasTaskStatus.Queued;
                _task.Progress = 0;
                return;
            }

            var terminalOperation = operations.FirstOrDefault(x => x.TypeName == OperationList.Last());
            if (terminalOperation != null && terminalOperation.Status == "SUCCESS")
            {
                _task.Progress = 100;
                _task.Status = SaasTaskStatus.Completed;
                return;
            }

            var activeOperation = operations.FindLast(x => x.Active);

            var lastOperation = operations.Last();

            if (activeOperation != null)
            {
                _task.Status = SaasTaskStatus.Running;
                _task.StatusDetail = activeOperation.Type.Description;
            }

            if (lastOperation != null)
            {
                var success = lastOperation.Status == "SUCCESS";
                var operationIndex = Array.IndexOf(OperationList, lastOperation.TypeName);
                if (operationIndex > -1)
                {
                    _task.Progress = (operationIndex + (success ? 1 : 0)) * 100 / OperationList.Length;
                }

                if (lastOperation.Status == "FAILURE" && machine.NeedsAdmin)
                {
                    _task.Status = SaasTaskStatus.Failed;
                    return;
                }
            }
        }


    }
}