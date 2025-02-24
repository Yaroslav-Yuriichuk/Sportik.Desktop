namespace Sportik.Automation.States
{
    public enum SequentialExerciseState
    {
        Unknown,
        Disabled,
        WaitingBeforeForceExecution,
        WaitingWithForceExecution,
        Queued,
        Executing,
    }
}
