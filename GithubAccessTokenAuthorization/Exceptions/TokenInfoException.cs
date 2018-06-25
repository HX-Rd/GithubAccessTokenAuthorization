using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Github.GithubAccessTokenAuthorization
{

    [Serializable]
    public class TokenInfoException : Exception
    {
        public TokenInfoException() { }
        public TokenInfoException(string message) : base(message) { }
        public TokenInfoException(string message, Exception inner) : base(message, inner) { }
        protected TokenInfoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
