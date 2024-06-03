namespace Tool08
{
    internal class FileItem
    {
        public FileInfo FileInfo { get; set; } = new("DummyFileName.txt");

        public string FileContentsAsText { get; set; } = string.Empty;

        public List<string> FileContentsAsLines { get; set; } = new();

        public string OutputFileName { get; set; } = string.Empty;
    }
}