using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace OAuth2Net
{
    public interface IOAuth2NetStateProvider : IKeyValue<string, OAuth2NetState>
    {
        object RemoveState(string state);
    }
}
