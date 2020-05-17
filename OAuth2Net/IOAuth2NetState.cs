using System.Collections;
using System.Runtime.CompilerServices;

namespace OAuth2Net
{
    public interface IOAuth2NetState
    {
        object this[string key] { get; set; }
    }
}