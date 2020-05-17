using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth2Net
{
    internal class OAuth2NetStaticStateProvider : IOAuth2NetStateProvider
    {
        static readonly Dictionary<string, OAuth2NetState> Authentications = new Dictionary<string, OAuth2NetState>();

        public OAuth2NetState this[string state]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(state))
                    return null;

                return Authentications.TryGetValue(state, out var result)
                    ? result
                    : null;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(state))
                    return;

                lock (Authentications)
                {
                    Authentications[state] = value;
                }
            }
        }
                
        public object RemoveState(string state)
        {
            var result = this[state];

            if (result != null)
                lock (Authentications)
                {
                    Authentications.Remove(state);
                }

            return result;
        }

    }
}
