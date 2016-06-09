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
	using System.Collections.Generic;
	using System.Net;
	public class ValidationScope {
		public ValidationScope(string httpMethod, Uri uri, string authorizationHeader, ServiceProvider serviceProvider) {
			if (String.IsNullOrEmpty(httpMethod)) {
				throw new ArgumentException("httpMethod is null or empty.", "httpMethod");
			}
			if (!(httpMethod == "GET" || httpMethod == "POST" || httpMethod == "DELETE"
							|| httpMethod == "PUT" || httpMethod == "UPDATE")) {
				throw new ArgumentException("The specified http method is not supported.", "httpMethod");
			}
			if (uri == null) {
				throw new ArgumentNullException("uri");
			}
			if (serviceProvider == null) {
				throw new ArgumentNullException("serviceProvider", "serviceProvider is null.");
			}
			_httpMethod = httpMethod;
			_serviceProvider = serviceProvider;
			_parameters = new RequiredParameters(new Url(uri, authorizationHeader));
		}
		RequiredParameters _parameters;
		public RequiredParameters Parameters {
			get {
				return _parameters;
			}
		}
		string _httpMethod;
		public string HttpMethod {
			get {
				return _httpMethod;
			}
		}
		ServiceProvider _serviceProvider;
		public ServiceProvider ServiceProvider {
			get {
				return _serviceProvider;
			}
		}
		IList<ValidationError> _errors = new List<ValidationError>();
		public IEnumerable<ValidationError> Errors {
			get {
				return _errors;
			}
		}
		public void AddError(ValidationError error) {
			if (error == null) {
				throw new ArgumentNullException("error", "error is null.");
			}
			_errors.Add(error);
		}
		public int ErrorCount {
			get { 
				return _errors.Count; 
			}
		}
		public bool ValidateParameters() {
			foreach (ValidationError error in Parameters.Errors) {
				AddError(new ValidationError(
					error.StatusCode,
					error.Message));
				return false;
			}
			return true;
		}
		public bool ValidateTimestamp() {
			if (!Parameters["oauth_timestamp"].IsTimestamp()) {
				AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid timestamp: " + Parameters["oauth_timestamp"].Value));
				return false;
			}
			return true;
		}
		public bool ValidateNonce() {
			if (!Parameters["oauth_nonce"].IsNonce()) {
				AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid / used nonce: " + Parameters["oauth_nonce"].Value));
				return false;
			}
			return true;
		}
		public IConsumer ValidateConsumer() {
			IConsumerStore store = ServiceProvider.ConsumerStore;
			if (store == null) {
				throw new InvalidOperationException("Consumer store is not configured.");
			}
			IConsumer consumer = store.GetConsumer(_parameters["oauth_consumer_key"].Value);
			if (consumer == null
						|| _parameters["oauth_consumer_key"].Value != consumer.ConsumerKey) {
				AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid Consumer Key"));
				return null;
			}
			return consumer;
		}
		public IToken ValidateUnauthorizedToken() {
			IRequestTokenStore store = ServiceProvider.RequestTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Request token store is not configured.");
			}
			IToken unauthorizedToken = store.GetToken(_parameters["oauth_token"].Value);
			if (unauthorizedToken == null || unauthorizedToken.IsEmpty ||
					unauthorizedToken.Value != _parameters["oauth_token"].Value ||
					String.IsNullOrEmpty(unauthorizedToken.Verifier)) {
				AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid / expired Token: " + _parameters["oauth_token"].Value));
				return null;
			}
			return unauthorizedToken;
		}
		public IToken ValidateAuthorizedToken() {
			IRequestTokenStore store = ServiceProvider.RequestTokenStore;
			if (store == null) {
				throw new InvalidOperationException("Request token store is not configured.");
			}
			IToken authorizedToken = store.GetToken(_parameters["oauth_token"].Value);
			if (authorizedToken == null 
				|| authorizedToken.IsEmpty 
				|| authorizedToken.Value != _parameters["oauth_token"].Value
				|| String.IsNullOrEmpty(authorizedToken.AuthenticationTicket)
				|| String.IsNullOrEmpty(authorizedToken.Verifier)
				|| authorizedToken.Verifier != _parameters["oauth_verifier"].Value) {
				AddError(new ValidationError(
							(int)HttpStatusCode.Unauthorized,
							"Invalid / expired Token: " + _parameters["oauth_token"].Value));
				return null;
			}
			return authorizedToken;
		}
		public bool ValidateSignature(string consumerSecret, string tokenSecret) {
			if (String.IsNullOrEmpty(consumerSecret)) {
				throw new ArgumentException("consumerSecret is null or empty.", "consumerSecret");
			}
			if (tokenSecret == null) {
				tokenSecret = String.Empty;
			}
			Parameter sig = Parameter.Empty;
			switch (_parameters["oauth_signature_method"].Value) {
				case "PLAINTEXT":
					sig = Signing.PlainText.Sign(
						consumerSecret,
						tokenSecret);
					if (!String.Equals(Escaping.Escape(sig.Value),
									_parameters["oauth_signature"].Value, StringComparison.Ordinal)) {
						AddError(new ValidationError(
									(int)HttpStatusCode.Unauthorized,
									"Invalid signature. Signature Base: " + sig.Value));
						return false;
					}
					return true;
				case "HMAC-SHA1":
					sig = Signing.HmaSha1.Sign(
							Signing.HmaSha1.Base(
										HttpMethod,
										_parameters.Url.ToUrl(),
										null),
								consumerSecret,
								tokenSecret);
					if (!String.Equals(sig.Value,
								_parameters["oauth_signature"].Value, StringComparison.Ordinal)) {
						AddError(new ValidationError(
									(int)HttpStatusCode.Unauthorized,
									"Invalid signature. Signature Base: " + sig.Value));
						return false;
					}
					return true;
				default:
					AddError(new ValidationError(
								(int)HttpStatusCode.BadRequest,
								"Unsupported signature method: " + _parameters["oauth_signature_method"].Value));
					return false;
			}
		}
	}
}
#endif
