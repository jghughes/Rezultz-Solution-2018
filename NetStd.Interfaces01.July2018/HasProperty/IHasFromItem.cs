namespace NetStd.Interfaces01.July2018.HasProperty
{
    public interface IHasFromItem<out T, in TU>
    {
        T FromItem(TU item);

    }
}