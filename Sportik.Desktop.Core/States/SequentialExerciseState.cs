namespace Sportik.Desktop.Core.States
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
