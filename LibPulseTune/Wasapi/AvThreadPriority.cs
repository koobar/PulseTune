namespace LibPulseTune.Wasapi
{
    public enum AvThreadPriority
    {
        Low = AvrtInterop.AVRT_PRIORITY_LOW,
        Normal = AvrtInterop.AVRT_PRIORITY_NORMAL,
        High = AvrtInterop.AVRT_PRIORITY_HIGH,
        Critical = AvrtInterop.AVRT_PRIORITY_CRITICAL
    }
}
