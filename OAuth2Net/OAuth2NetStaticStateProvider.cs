using System;
using System.Collections.Generic;
using System.Text;

namespace OAuth2Net
{
    public class OAuth2NetStaticStateProvider : IOAuth2NetStateProvider, IKeyValue<string, OAuth2App>
    {
        static readonly Dictionary<string, OAuth2App> Authentications = new Dictionary<string, OAuth2App>();


        public OAuth2App this[string state]
        {
            get => (OAuth2App)GetState(state);
            set => SetState(state, value);
        }

        public object GetState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return null;

            return Authentications.TryGetValue(state, out var result) 
                ? result 
                : null;
        }

        public object RemoveState(string state)
        {
            var result = GetState(state);

            if (result != null)
                lock (Authentications)
                {
                    Authentications.Remove(state);
                }

            return result;
        }

        public void SetState(string state, object stateValue)
        {
            if (string.IsNullOrWhiteSpace(state))
                return;

            lock (Authentications)
            {
                Authentications[state] = (OAuth2App)stateValue;
            }
        }
    }
}
