using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace OAuth2Net
{
    public interface IOAuth2NetStateProvider
    {
        object GetState(string state);

        void SetState(string state, object stateValue);

        object RemoveState(string state);
    }
}
