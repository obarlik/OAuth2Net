namespace OAuth2Net
{
    internal interface IKeyValue<TKey, TValue>    
    {
        TValue this[TKey state] { get; set; }
    }
}