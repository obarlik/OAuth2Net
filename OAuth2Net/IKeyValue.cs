namespace OAuth2Net
{
    public interface IKeyValue<TKey, TValue>    
    {
        TValue this[TKey state] { get; set; }
    }
}