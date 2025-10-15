namespace WindowsBackgroundService.WorkerObjects
{
    public interface I_Worker
    {
        bool ServiceStarted { get; set; }
        void ExecuteTask();
    }
}
