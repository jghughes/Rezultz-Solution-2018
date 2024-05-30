namespace Tool09;

internal class MyLapsFileItem
{
    public FileInfo MyLapsFileInfo { get; set; } = new("DummyFileName.txt");

    public string MyLapsFileContentsAsText { get; set; } = string.Empty;

    public List<MyLapsResultItem> MyLapsFileContentsAsMyLapsResultObjects { get; set; } = new();
}