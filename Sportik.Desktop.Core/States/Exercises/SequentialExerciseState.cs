namespace Sportik.Desktop.Core.States.Exercises
{
    public enum SequentialExerciseState
    {
        Unknown,
        Disabled,
        WaitingBeforeForceExecution,
        WaitingWithForceExecution,
        Queued,
        Executing,
        Snoozed,
    }
}
