#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

#if !SL
namespace DevExpress.Utils.OAuth.Provider {
	using System;
	using System.Configuration;
	using System.Net;
	using System.Collections.Specialized;
	public class ServiceProvider : ConfigurationSection {
		#region Default
		public static ServiceProvider Default {
			get {
				ServiceProvider procider = ConfigurationManager.GetSection("oauth.serviceProvider") as ServiceProvider;
				if (procider == null) {
					procider = new ServiceProvider();
				}
				return procider;
			}
		}
		#endregion
		#region ConsumerStore
		IConsumerStore _consumerStore;
		public virtual IConsumerStore ConsumerStore {
			get {
				if (_consumerStore == null) {
					_consumerStore = ConfigurationManager.GetSection("oauth.consumerStore") as IConsumerStore;
					if (_consumerStore == null) {
						_consumerStore = new ConsumerStore();
					}
				}
				return _consumerStore;
			}
		}
		#endregion
		#region AccessTokenStore
		IAccessTokenStore _accessTokenStore;
		public virtual IAccessTokenStore AccessTokenStore {
			get {
				if (_accessTokenStore == null) {
					_accessTokenStore = ConfigurationManager.GetSection("oauth.accessTokenStore") as IAccessTokenStore;
					if (_accessTokenStore == null) {
						_accessTokenStore = new AccessTokenStore();
					}
				}
				return _accessTokenStore;
			}
		}
		#endregion
		#region RequestTokenStore
		IRequestTokenStore _requestTokenStore;
		public virtual IRequestTokenStore RequestTokenStore {
			get {
				if (_requestTokenStore == null) {
					_requestTokenStore = ConfigurationManager.GetSection("oauth.requestTokenStore") as IRequestTokenStore;
					if (_requestTokenStore == null) {
						_requestTokenStore = new RequestTokenStore();
					}
				}
				return _requestTokenStore;
			}
		}
		#endregion
		#region GetRequestToken
		public static Response GetRequestToken(string httpMethod, Uri uri) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return GetRequestToken(serviceProvider, httpMethod, uri);
		}
		public static Response GetRequestToken(ServiceProvider serviceProvider, string httpMethod, Uri uri) {
			ValidationScope scope;
			IToken token = GetRequestToken(serviceProvider, httpMethod, uri, out scope);
			if (token == null || token.IsEmpty) {
				if (scope != null) {
					foreach (ValidationError error in scope.Errors) {
						return new Response(error.StatusCode, error.Message, "text/plain");
					}
				}
				return new Response((int)HttpStatusCode.InternalServerError, "Internal server error", "text/plain");
			}
			return new Response((int)HttpStatusCode.OK, String.Format("oauth_token={0}&oauth_token_secret={1}&oauth_callback_confirmed=true",
					Escaping.Escape(token.Value),
					Escaping.Escape(token.Secret)), "text/plain");
		}
		public static IToken GetRequestToken(string httpMethod, Uri uri, out ValidationScope scope) {
			ServiceProvider serviceProvider = ServiceProvider.Default;			
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return GetRequestToken(serviceProvider,
					   httpMethod,
					   uri,
					   out scope);
		}
		public static IToken GetRequestToken(
			ServiceProvider serviceProvider,
			string httpMethod,
			Uri uri,
			out ValidationScope scope) {
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			scope = new ValidationScope(httpMethod,
										uri,
										String.Empty,
										serviceProvider);
			scope.Parameters.Require(
				"oauth_consumer_key",
				"oauth_nonce",
				"oauth_timestamp",
				"oauth_signature_method",
				"oauth_signature",
				"oauth_callback");
			scope.Parameters.Match("oauth_version", false, "1.0");
			if (!scope.ValidateParameters()) {
				return null;
			}
			if (!scope.ValidateTimestamp()) {
				return null;
			}
			if (!scope.ValidateNonce()) {
				return null;
			}
			IConsumer consumer = scope.ValidateConsumer();
			if (consumer == null) {
				return null;
			}
			if (!scope.ValidateSignature(consumer.ConsumerSecret, String.Empty)) {
				return null;
			}
			IRequestTokenStore store = serviceProvider.RequestTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Request token store is not configured.");
			}
			return store.CreateUnauthorizeToken(
				consumer.ConsumerKey,
				consumer.ConsumerSecret,
				scope.Parameters["oauth_callback"].Value);
		}
		#endregion
		#region VerifyRequestToken
		public static IToken VerifyRequestToken(string httpMethod, Uri uri, out ValidationScope scope) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return VerifyRequestToken(
				serviceProvider,
				httpMethod,
				uri,
				out scope);
		}
		public static IToken VerifyRequestToken(
			 ServiceProvider serviceProvider,
			 string httpMethod,
			 Uri uri, 
			 out ValidationScope scope) {
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			scope = new ValidationScope(httpMethod,
										uri,
										String.Empty,
										serviceProvider);
			scope.Parameters.Require(
				"oauth_token");
			scope.Parameters.Match("oauth_version", false, "1.0");
			if (!scope.ValidateParameters()) {
				return null;
			}
			IToken unauthorizedToken = scope.ValidateUnauthorizedToken();
			if (unauthorizedToken == null) {
				return null;
			}
			return unauthorizedToken;
		}
		#endregion
		#region AuthorizeRequestToken
		public static IToken AuthorizeRequestToken(string httpMethod, Uri uri, string authenticationTicket, out ValidationScope scope) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return AuthorizeRequestToken(
				serviceProvider,
				httpMethod,
				uri,
				authenticationTicket,
				out scope);
		}
		public static IToken AuthorizeRequestToken(
			 ServiceProvider serviceProvider,
			 string httpMethod,
			 Uri uri, 
			 string authenticationTicket,
			 out ValidationScope scope) {
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			scope = new ValidationScope(httpMethod,
										uri,
										String.Empty,
										serviceProvider);
			scope.Parameters.Require(
				"oauth_token");
			scope.Parameters.Match("oauth_version", false, "1.0");
			if (!scope.ValidateParameters()) {
				return null;
			}
			IRequestTokenStore store = serviceProvider.RequestTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Request token store is not configured.");
			}
			IToken authorizedToken = store.AuthorizeToken(
				scope.Parameters["oauth_token"].Value,
				authenticationTicket);
			if (authorizedToken == null 
					|| authorizedToken.IsEmpty 
					|| authorizedToken.Value != scope.Parameters["oauth_token"].Value
					|| String.IsNullOrEmpty(authorizedToken.AuthenticationTicket)
					|| authorizedToken.AuthenticationTicket != authenticationTicket 
					|| String.IsNullOrEmpty(authorizedToken.Verifier)) {
				scope.AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid / expired Token: " + scope.Parameters["oauth_token"].Value));
				return null;
			}
			scope = null;
			return authorizedToken;
		}
		#endregion
		#region GetAccessToken
		public static Response GetAccessToken(string httpMethod, Uri uri) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return GetAccessToken(serviceProvider, httpMethod, uri);
		}
		public static Response GetAccessToken(ServiceProvider serviceProvider, string httpMethod, Uri uri) {
			ValidationScope scope;
			IToken token = GetAccessToken(serviceProvider, httpMethod, uri, out scope);
			if (token == null || token.IsEmpty) {
				if (scope != null) {
					foreach (ValidationError error in scope.Errors) {
						return new Response(error.StatusCode, error.Message, "text/plain");
					}
				}
				return new Response((int)HttpStatusCode.InternalServerError, "Internal server error", "text/plain");
			}
			return new Response((int)HttpStatusCode.OK, String.Format("oauth_token={0}&oauth_token_secret={1}&oauth_callback_confirmed=true",
					Escaping.Escape(token.Value),
					Escaping.Escape(token.Secret)), "text/plain");
		}
		public static IToken GetAccessToken(string httpMethod, Uri uri, out ValidationScope scope) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return GetAccessToken(
				serviceProvider,
				httpMethod,
				uri,
				out scope);
		}
		public static IToken GetAccessToken(
			ServiceProvider serviceProvider,
			string httpMethod,
			Uri uri,
			out ValidationScope scope) {
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			scope = new ValidationScope(httpMethod,
										uri,
										String.Empty,
										serviceProvider);
			scope.Parameters.Require(
				"oauth_verifier",
				"oauth_token",
				"oauth_consumer_key",
				"oauth_nonce",
				"oauth_timestamp",
				"oauth_signature_method",
				"oauth_signature");
			scope.Parameters.Match("oauth_version", false, "1.0");
			if (!scope.ValidateParameters()) { return null; }
			if (!scope.ValidateTimestamp()) { return null; }
			if (!scope.ValidateNonce()) { return null; }
			IToken authorizedToken = scope.ValidateAuthorizedToken();
			if (authorizedToken == null) {
				return null;
			}
			if (!scope.ValidateSignature(authorizedToken.ConsumerSecret, authorizedToken.Secret)) {
				return null;
			}
			IAccessTokenStore store = serviceProvider.AccessTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Access token store is not configured.");
			}
			return store.CreateToken(authorizedToken);
		}
		#endregion
		#region GetTokenIdentity
		public static TokenIdentity GetTokenIdentity(
			string httpMethod,
			Uri uri,
			NameValueCollection headers) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			if (headers == null) return null;
			string authorizationHeader = headers["Authorization"]; 
			if (String.IsNullOrEmpty(authorizationHeader)) {
				return null;
			}
			ValidationScope scope;
			TokenIdentity identity = GetTokenIdentity(
				serviceProvider,
				httpMethod,
				uri,
				authorizationHeader,
				out scope);
			if (scope != null 
						&& scope.ErrorCount > 0) {
				return null;
			}
			return identity;
		}
		public static TokenIdentity GetTokenIdentity(
			string httpMethod,
			Uri uri,
			string authorizationHeader,
			out ValidationScope scope) {
			ServiceProvider serviceProvider = ServiceProvider.Default;
			if (serviceProvider == null) {
				throw new InvalidOperationException("Service provider not configured.");
			}
			return GetTokenIdentity(
				serviceProvider,
				httpMethod,
				uri,
				authorizationHeader,
				out scope);
		}
		public static TokenIdentity GetTokenIdentity(
			ServiceProvider serviceProvider,
			string httpMethod,
			Uri uri,
			string authorizationHeader,
			out ValidationScope scope) {
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider");
			}
			if (String.IsNullOrEmpty(authorizationHeader))
				throw new ArgumentException("authorizationHeader is null or empty.", "authorizationHeader");
			if (!authorizationHeader.StartsWith("OAuth",
						StringComparison.InvariantCultureIgnoreCase)) {
				scope = null;
				return null;
			}
			scope = new ValidationScope(httpMethod,
										uri,
										authorizationHeader,
										serviceProvider);
			scope.Parameters.Require(
				"oauth_token",
				"oauth_consumer_key",
				"oauth_nonce",
				"oauth_timestamp",
				"oauth_signature_method",
				"oauth_signature");
			scope.Parameters.Match("oauth_version", false, "1.0");
			if (!scope.ValidateParameters()) { return null; }
			if (!scope.ValidateTimestamp()) { return null; }
			if (!scope.ValidateNonce()) { return null; }
			IAccessTokenStore store = serviceProvider.AccessTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Access token store is not configured.");
			}
			IToken accessToken = store.GetToken(scope.Parameters["oauth_token"].Value);
			if (accessToken == null || accessToken.IsEmpty ||
					accessToken.Value != scope.Parameters["oauth_token"].Value
					|| String.IsNullOrEmpty(accessToken.AuthenticationTicket)) {
				scope.AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid / expired Token: " + scope.Parameters["oauth_token"].Value));
				return null;
			}
			if (!scope.ValidateSignature(accessToken.ConsumerSecret, accessToken.Secret)) {
				return null;
			}
			scope = null;
			return new TokenIdentity(
				accessToken.Value,
				accessToken.AuthenticationTicket);
		}
		#endregion
	}
}
#endif
