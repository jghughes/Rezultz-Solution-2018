namespace Tool04;

internal class MyLapsFileObject
{
    public FileInfo MyLapsFileInfo { get; set; } = new("DummyFileName.txt");

    public string MyLapsFileContentsAsText { get; set; } = string.Empty;

    public List<MyLapsResultObject> MyLapsFileContentsAsMyLapsResultObjects { get; set; } = new();
}