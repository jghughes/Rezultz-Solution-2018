namespace NetStd.HttpRequestService.Feb2019;

public readonly struct ObjectResponseResult<T>(T deserialisedObject, string originatingJsonString)
{
    public T Object { get; } = deserialisedObject;
    public string Text { get; } = originatingJsonString;
}