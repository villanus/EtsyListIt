﻿using EtsyWrapper.DomainObjects;
using EtsyWrapper.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace EtsyWrapper
{
    public class RestServiceWrapper : IRestServiceWrapper
    {
        private string _baseUrl;

        public RestServiceWrapper()
        {
            _baseUrl = "https://openapi.etsy.com/v2/";
        }
        public RestClient GetRestClient()
        {
            return new RestClient(_baseUrl);
        }

        public IAuthenticator GetAuthenticatorForRequestToken(string apiKey, string sharedSecret)
        {
            return OAuth1Authenticator.ForRequestToken(apiKey, sharedSecret, "oob");
        }

        public RestRequest GetRestRequest(string request, Method method)
        {
            return new RestRequest(request, method);
        }

        public IAuthenticator GetAuthenticatorForAccessToken(string apiKey, string sharedSecret, TemporaryToken tempToken, string verifier)
        {
            return OAuth1Authenticator.ForAccessToken(apiKey, sharedSecret, tempToken.OAuthToken, tempToken.OAuthTokenSecret, verifier);
        }

        public IAuthenticator GetAuthenticatorForProtectedResource(PermanentToken authToken)
        {
            return OAuth1Authenticator.ForProtectedResource(authToken.APIKey, authToken.SharedSecret, authToken.TokenID,
                authToken.TokenSecret);
        }
    }
}