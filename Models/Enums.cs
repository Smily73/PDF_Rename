namespace PDFRename.Models
{
    public enum ProcessingMode
    {
        AutomaticRename,
        SimulationMode,
        EditBeforeRename
    }

    public enum FileStatus
    {
        Pending,
        Processing,
        ReadingMetadata,
        Ready,
        Success,
        Error,
        Skipped
    }
}
