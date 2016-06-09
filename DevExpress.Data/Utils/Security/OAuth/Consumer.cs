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

namespace DevExpress.Utils.OAuth {
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Net;
	using System.IO;
	using System.Threading;
	public class ConsumerBase : IConsumer {
		Uri _callbackUri;
		public Uri CallbackUri {
			get {
				return _callbackUri;
			}
			set {
				_callbackUri = value;
			}
		}
		string _consumerKey;
		public string ConsumerKey {
			get {
				return _consumerKey;
			}
			set {
				_consumerKey = value;
			}
		}
		string _consumerSecret;
		public string ConsumerSecret {
			get {
				return _consumerSecret;
			}
			set {
				_consumerSecret = value;
			}
		}
	}
	public class ConsumerOperation : IAsyncResult {
		public static void GetResponseBytes(Stream source, ref byte[] buffer) {
			const int BufferSize = 0x1000;
			byte[] bytes = new byte[BufferSize];
			int bytesRead;
			while ((bytesRead = source.Read(bytes, 0, bytes.Length)) > 0) {
				byte[] responseBytes2 = new byte[bytesRead +
					(buffer != null && buffer.Length > 0 ? buffer.Length : 0)];
				if (buffer != null && buffer.Length > 0) {
					System.Buffer.BlockCopy(
						buffer,
						0,
						responseBytes2,
						0,
						buffer.Length);
				}
				System.Buffer.BlockCopy(
					bytes,
					0,
					responseBytes2,
					responseBytes2.Length - bytesRead,
					bytesRead);
				buffer = responseBytes2;
			}
		}
		public static byte[] GetResponseBytes(WebResponse response) {
			using (Stream source = response.GetResponseStream()) {
				byte[] buffer = null;
				GetResponseBytes(source, ref buffer);
				return buffer;
			}
		}
		public ConsumerOperation(HttpWebRequest request, Consumer owner, string version, bool unescape, AsyncCallback callback, object state) {
			if (request == null) {
				throw new ArgumentNullException("request", "request is null.");
			}
			_request = request;
			_asyncCallback = callback;
			_asyncState = state;
			_owner = owner;
			_version = version;
			_unescape = unescape;
		}
		Exception _errorInfo;
		public Exception ErrorInfo {
			get {
				return _errorInfo;
			}
		}
		HttpWebRequest _request;
		public HttpWebRequest Request {
			get {
				return _request;
			}
		}
		AsyncCallback _asyncCallback;
		public AsyncCallback AsyncCallback {
			get {
				return _asyncCallback;
			}
		}
		Consumer _owner;
		public Consumer Owner {
			get {
				return _owner;
			}
		}
		bool _unescape;
		public bool Unescape {
			get {
				return _unescape;
			}
		}
		string _version;
		public string Version {
			get {
				return _version;
			}
		}
		bool _endCalled;
		public bool EndCalled {
			get {
				return _endCalled;
			}
		}
		byte[] _response;
		public byte[] Response {
			get {
				return _response;
			}
		}
		object _asyncState;
		public object AsyncState {
			get {
				return _asyncState;
			}
		}
		public virtual WaitHandle AsyncWaitHandle {
			get {
				throw new NotSupportedException();
			}
		}
		bool _synchronously;
		public bool CompletedSynchronously {
			get {
				return _synchronously;
			}
		}
		int _isCompleted;
		public bool IsCompleted {
			get {
				return _isCompleted != 0;
			}
		}
		public byte[] End(object owner) {
			bool bOwner = true;
			if (owner == null) {
				if (_owner != null) {
					bOwner = false;
				}
			} else {
				if (_owner == null) {
					bOwner = false;
				} else {
					if (!object.ReferenceEquals(owner, _owner)) {
						bOwner = false;
					}
				}
			}
			if (!bOwner) {
				throw new InvalidOperationException("The IAsyncResult object was not returned from the corresponding asynchronous method on this class.");
			}
			if (!IsCompleted) {
				throw new InvalidOperationException("Method can only be called when an asynchronous operation is completed.");
			}
			if (EndCalled) {
				throw new InvalidOperationException("Method can only be called once for each asynchronous operation.");
			}
			_endCalled = true;
			Exception error = ErrorInfo;
			if (error != null) {
				throw error;
			}
			return _response;
		}
		protected virtual void Finish() {
		}
		void Complete(bool synchronously) {
			try {
				_synchronously = synchronously;
				AsyncCallback asyncCallback = AsyncCallback;
				if (asyncCallback != null) {
					asyncCallback(this);
				}
			} finally {
				Finish();
			}
		}
		public void CompleteWithError(Exception exception, bool synchronously) {
			if (((exception is OutOfMemoryException)
						|| (exception is StackOverflowException)) || (exception is ThreadAbortException)) {
							throw exception;
			}
			if (Interlocked.Exchange(ref _isCompleted, 0x7fffffff) == 0) {
				_errorInfo = exception;
				Complete(synchronously);
			}
		}
		public void Complete(byte[] response, bool synchronously) {
			if (Interlocked.Exchange(ref _isCompleted, 0x7fffffff) == 0) {
				_response = response;
				Complete(synchronously);
			}
		}
	}
	public class Consumer : ConsumerBase {		
		public static void AbortHttpRequest(WebRequest request) {
			try {
				if (request != null) {
					request.Abort();
				}
			} catch (Exception exception) {
				if (((exception is OutOfMemoryException) || (exception is StackOverflowException)) || (exception is ThreadAbortException)) {
					throw;
				}
			}
		}
		public static HttpWebRequest CreateHttpRequest(Uri requestUri, string httpMethod, CookieContainer cookies) {
			if (requestUri == null) {
				throw new ArgumentNullException("requestUri");
			}
			if (httpMethod == null || httpMethod.Length == 0) {
				throw new ArgumentNullException("httpMethod");
			}
			if (!requestUri.IsAbsoluteUri) {
				throw new InvalidOperationException("Request URI be absolute.");
			}
			HttpWebRequest httpRequest
				= (HttpWebRequest)HttpWebRequest.Create(requestUri);
			httpRequest.Method = httpMethod;
			if (String.Equals(httpRequest.Method, "POST", StringComparison.InvariantCultureIgnoreCase)) {
				httpRequest.ContentLength = 0;
				httpRequest.ContentType = "application/x-www-form-urlencoded";
			}
#if !SL
			if (cookies != null) {
				httpRequest.CookieContainer = cookies;
			}
			httpRequest.KeepAlive = false;
#endif
			return httpRequest;
		}
		public static HttpWebRequest CreateHttpRequest(Uri requestUri, string httpMethod, CookieContainer cookies, 
			IToken token, Signature signature, string version) {
			if (version == null || version.Length == 0) {
				version = "1.0";
			}
			if (!String.Equals(version, "1.0") &&
						!String.Equals(version, "2.0")) {
				throw new ArgumentException("The specified OAuth version is not supported.", "version");
			}
			if (token == null || token.IsEmpty) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token'");
			}
			if (requestUri == null) {
				throw new ArgumentNullException("requestUri");
			}
			if (httpMethod == null || httpMethod.Length == 0) {
				throw new ArgumentNullException("httpMethod");
			}
			if (!requestUri.IsAbsoluteUri) {
				throw new InvalidOperationException("Request URI be absolute.");
			}
			Url url = new Url(requestUri, String.Empty);
			HttpWebRequest httpRequest = null;
			switch (version) {
				case "1.0":
					httpRequest = (HttpWebRequest)HttpWebRequest.Create(requestUri.ToString());
					httpRequest.Headers[HttpRequestHeader.Authorization] = GetAuthorizationHeader(
					   requestUri,
					   token,
					   httpMethod,
					   signature
						#if DEBUGTEST
						, null
						, null
						#endif
					   );
					break;
				case "2.0":
					httpRequest = (HttpWebRequest)HttpWebRequest.Create(url.ToUri(
						new Parameter("access_token", token.Value)));
					break;
				default:
					throw new NotImplementedException();
			}
			httpRequest.Method = httpMethod;
#if !SL
			httpRequest.PreAuthenticate = true;
			if (cookies != null) {
				httpRequest.CookieContainer = cookies;
			}
#endif
			httpRequest.AllowAutoRedirect = false;
			return httpRequest;
		}
		public static string ParseResource(StreamReader reader) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			return reader.ReadToEnd();
		}
		public static string ParseResource(Stream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			using (StreamReader reader = new StreamReader(stream)) {
				return ParseResource(reader);
			}
		}
		public static string ParseResource(WebResponse response) {
			if (response == null) {
				throw new ArgumentNullException("response");
			}
			using (Stream stream = response.GetResponseStream()) {
				return ParseResource(stream);
			}
		}
#if !SL
		public string GetResource(Uri requestUri) { return GetResource(requestUri, AccessToken, "1.0"); }
		public string GetResource(Uri requestUri, IToken token) { return GetResource(requestUri, token, "1.0"); }
		public string GetResource(Uri requestUri, IToken token, string version) {
			if (version == null
						|| version.Length == 0) {
				version = "1.0";
			}
			HttpWebRequest request = CreateHttpRequest(requestUri, 
				HttpMethod, 
				Cookies,
				token,
				Signature,
				version);
			try {
				HttpStatusCode statusCode = HttpStatusCode.OK;
				string statusDescription = String.Empty;
				string redirectUri = String.Empty;
#if TRACE
				System.Diagnostics.Trace.TraceInformation(request.RequestUri.ToString());
#endif
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
					statusCode = response.StatusCode;
					statusDescription = response.StatusDescription;
					redirectUri = response.Headers["Location"];
					if (statusCode == HttpStatusCode.OK) {
						return ParseResource(response);
					}
				}
				System.Diagnostics.Debug.Assert(statusCode != HttpStatusCode.OK, "statusCode != HttpStatusCode.OK");
				if (statusCode == HttpStatusCode.Redirect
								&& !String.IsNullOrEmpty(redirectUri)) {
					request = CreateHttpRequest(new Uri(redirectUri, UriKind.Absolute),
							HttpMethod,
							Cookies,
							token,
							Signature,
							version);
					using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
						statusCode = response.StatusCode;
						statusDescription = response.StatusDescription;
						if (statusCode == HttpStatusCode.OK) {
							return ParseResource(response);
						}
					}
				}
				throw new WebException(statusDescription);
			} catch {
				throw;
			}
		}
#endif
		public string GetAuthorizationHeader(Uri requestUri) {
			return GetAuthorizationHeader(
				requestUri,
				AccessToken,
				HttpMethod,
				Signature
#if DEBUGTEST   
				, null
				, null
#endif
				);
		}
		public static string GetAuthorizationHeader(
			Uri requestUri,
			IToken token,
			string httpMethod,
			Signature signature
#if DEBUGTEST
			, Nullable<Parameter> nonce
			, Nullable<Parameter> timestamp
#endif
			) {
			if (requestUri == null) {
				throw new ArgumentNullException("requestUri");
			}
			if (!requestUri.IsAbsoluteUri) {
				throw new InvalidOperationException("Request URI be absolute.");
			}
			if (token == null) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token'");
			}
			if (String.IsNullOrEmpty(token.Value)) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token'");
			}
			if (String.IsNullOrEmpty(token.Secret)) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token_secret'");
			}
			if (String.IsNullOrEmpty(httpMethod)) {
				throw new ArgumentNullException("httpMethod");
			}
			if (String.IsNullOrEmpty(token.ConsumerKey)) {
				throw new ArgumentNullException("consumerKey");
			}
			if (String.IsNullOrEmpty(token.ConsumerSecret)) {
				throw new ArgumentNullException("consumerSecret");
			}
			List<Parameter> parameters = new List<Parameter>();
#if DEBUGTEST
			if (nonce.HasValue) {
				parameters.Add(nonce.Value);
			} else {
				parameters.Add(Parameter.Nonce());
			}
#else
			parameters.Add(Parameter.Nonce());
#endif
#if DEBUGTEST
			if (timestamp.HasValue) {
				parameters.Add(timestamp.Value);
			} else {
				parameters.Add(Parameter.Timestamp());
			}
#else
			parameters.Add(Parameter.Timestamp());
#endif
			parameters.Add(Parameter.Version());
			parameters.Add(new Parameter("oauth_consumer_key", token.ConsumerKey));
			parameters.Add(new Parameter("oauth_token", token.Value));
			Url url = new Url(requestUri, String.Empty);
			switch (signature) {
				case Signature.PLAINTEXT:
					parameters.Add(new Parameter("oauth_signature_method", "PLAINTEXT"));
					Signing.PlainText.Sign(
						token.ConsumerSecret,
						token.Secret,
						parameters);
					break;
				case Signature.HMACSHA1:
					parameters.Add(new Parameter("oauth_signature_method", "HMAC-SHA1"));
					Signing.HmaSha1.Sign(
						httpMethod,
						url,
						token.ConsumerSecret,
						token.Secret,
						parameters);
					break;
				default:
					throw new NotImplementedException();
			}
			StringBuilder authorizationHeader = new StringBuilder();
			authorizationHeader.Append("OAuth");
			IList<Parameter> sorted = Parameters.Sort(parameters);
			for (int i = 0; i < sorted.Count; i++) {
				if (sorted[i].IsEmpty) {
					continue;
				}
				if (i == 0) {
					authorizationHeader.Append(" ");
				} else {
					authorizationHeader.Append(",");
				}
				authorizationHeader.AppendFormat("{0}=\"{1}\"",
					Escaping.Escape(sorted[i].Name),
					Escaping.Escape(sorted[i].Value));
			}
			return authorizationHeader.ToString();
		}
		string _httpMethod = "GET";
		public string HttpMethod {
			get { 
				return _httpMethod; 
			}
			set {
				_httpMethod = value;
			}
		}		
		Uri _requestUri;
		public Uri RequestUri {
			get {
				return _requestUri;
			}
			set {
				_requestUri = value;
			}
		}
		Uri _authorizeUri;
		public Uri AuthorizeUri {
			get {
				return _authorizeUri;
			}
			set {
				_authorizeUri = value;
			}
		}
		Uri _accessUri;
		public Uri AccessUri {
			get {
				return _accessUri;
			}
			set {
				_accessUri = value;
			}
		}		
		Signature _signature;
		public Signature Signature {
			get {
				return _signature;
			}
			set {
				_signature = value;
			}
		}
		IToken _requestToken;
		public IToken RequestToken {
			get {
				return _requestToken;
			}
			set {
				_requestToken = value;
			}
		}
		public Uri GetRequestTokenUrl() {
			if (RequestUri == null) {
				throw new InvalidOperationException("RequestUri is not specified.");
			}
			if (!RequestUri.IsAbsoluteUri) {
				throw new InvalidOperationException("RequestUri be an absolute uri.");
			}
			if (String.IsNullOrEmpty(ConsumerKey)) {
				throw new InvalidOperationException("ConsumerKey is not specified. 'oauth_consumer_key' is a required attribute.");
			}
			if (String.IsNullOrEmpty(ConsumerSecret)) {
				throw new InvalidOperationException("ConsumerSecret is not specified. 'oauth_consumer_secret' is a required attribute.");
			}
			if (CallbackUri == null) {
				throw new InvalidOperationException("CallbackUri is not specified. 'oauth_callback' is a required attribute.");
			}
			List<Parameter> parameters = new List<Parameter>();
			parameters.Add(Parameter.Version());
			parameters.Add(Parameter.Nonce());
			parameters.Add(Parameter.Timestamp());
			parameters.Add(Parameter.ConsumerKey(ConsumerKey));
			parameters.Add(Parameter.Callback(CallbackUri.ToString()));
			Url url = new Url(RequestUri, String.Empty);
			switch (Signature) {
				case Signature.PLAINTEXT:
					parameters.Add(Parameter.SignatureMethod(Signature.PLAINTEXT));
					Signing.PlainText.Sign(
						ConsumerSecret,
						String.Empty,
						parameters);
					break;
				case Signature.HMACSHA1:
					parameters.Add(Parameter.SignatureMethod(Signature.HMACSHA1));
					Signing.HmaSha1.Sign(
						HttpMethod,
						url,
						ConsumerSecret,
						String.Empty,
						parameters);
					break;
				default:
					throw new NotImplementedException();
			}
			return url.ToUri(parameters);
		}
		CookieContainer _cookies = new CookieContainer();
		public CookieContainer Cookies {
			get {
				return _cookies;
			}
			set {
				_cookies = value;
			}
		}
#if DEBUGTEST && !SL
		public IToken GetRequestToken(DevExpress.Utils.OAuth.Provider.ServiceProvider serviceProvider) {
			if (_requestToken != null
						&& !_requestToken.IsEmpty) {
				return _requestToken;
			}
			DevExpress.Utils.OAuth.Provider.Response response = DevExpress.Utils.OAuth.Provider.ServiceProvider.GetRequestToken(
				serviceProvider,
				HttpMethod,
				GetRequestTokenUrl());
			if (response.StatusCode != 200) {
				throw new WebException(String.Format("Http Error {0}. {1}", response.StatusCode, response.Content));
			}
			_requestToken = new Token(
				Parameters.ParseTokens(response.Content, true),
				ConsumerKey,
				ConsumerSecret, 
				CallbackUri.ToString(),
				"1.0");
			return _requestToken;
		}
#endif
#if !SL
		public IToken GetRequestToken() {
			if (_requestToken != null 
						&& !_requestToken.IsEmpty) {				
				return _requestToken;
			}
			using (WebResponse httpResponse 
						= CreateHttpRequest(
								GetRequestTokenUrl(), 
								HttpMethod, 
								Cookies).GetResponse()) {
				_requestToken = new Token(
					Parameters.Parse(httpResponse),
					ConsumerKey,
					ConsumerSecret,
					CallbackUri.ToString(),
					"1.0");				
				return _requestToken;
			}
		}
#endif   
		static void OnHttpResponse(IAsyncResult asyncResult) {
			ConsumerOperation ar = (ConsumerOperation)asyncResult.AsyncState;
			try {
				using (WebResponse response = ar.Request.EndGetResponse(asyncResult)) {					
					ar.Complete(ConsumerOperation.GetResponseBytes(response), false);
					AbortHttpRequest(ar.Request);
				}
			} catch (Exception exception) {				
				ar.CompleteWithError(exception, false);
			}
		}
		public IAsyncResult BeginGetRequestToken(AsyncCallback callback, object state) {
			HttpWebRequest request = CreateHttpRequest(
				GetRequestTokenUrl(),
				HttpMethod,
				Cookies);			
			ConsumerOperation outerAsyncResult = new ConsumerOperation(
				request,
				this,
				"1.0",
				true,
				callback,
				state);
			request.BeginGetResponse(
				OnHttpResponse,
				outerAsyncResult);
			return outerAsyncResult;
		}
		public IToken EndGetRequestToken(IAsyncResult ar) {
			if (ar == null) {
				throw new ArgumentNullException("ar", "ar is null.");
			}
			ConsumerOperation operation = ((ConsumerOperation)ar);
			byte[] responseBytes = operation.End(this);
			_requestToken = new Token(
					Parameters.Parse(
						responseBytes,
						operation.Unescape),
					ConsumerKey,
					ConsumerSecret,
					CallbackUri.ToString(),
					"1.0");				
			return _requestToken;
		}
		bool _requireSsl;
		public bool RequireSsl {
			get {
				return _requireSsl;
			}
			set {
				_requireSsl = value;
			}
		}
		public Uri GetAuthorizeTokenUrl() { return GetAuthorizeTokenUrl(String.Empty); }
		public Uri GetAuthorizeTokenUrl(string version) {
			if (version == null || version.Length == 0) {
				version = "1.0";
			}
			if (!String.Equals(version, "1.0") &&
						!String.Equals(version, "2.0")) {
				throw new ArgumentException("The specified OAuth version is not supported.", "version");
			}
			if (AuthorizeUri == null) {
				throw new InvalidOperationException("AuthorizeUri is not specified.");
			}
			if (!AuthorizeUri.IsAbsoluteUri) {
				throw new InvalidOperationException("AuthorizeUri be an absolute uri.");
			}
			if (!String.Equals(AuthorizeUri.Scheme, "HTTPS", StringComparison.OrdinalIgnoreCase)
							&& RequireSsl) {								
				throw new InvalidOperationException("AuthorizeUri must use the HTTPS protocol.");
			}
			if (String.IsNullOrEmpty(ConsumerKey)) {
				throw new InvalidOperationException("ConsumerKey is not specified. 'client_id' is a required attribute.");
			}
			if (CallbackUri == null) {
				throw new InvalidOperationException("CallbackUri is not specified. 'redirect_uri' is a required attribute.");
			}
			if (!CallbackUri.IsAbsoluteUri) {
				throw new InvalidOperationException("CallbackUri be an absolute uri.");
			}
			if (version == "1.0") {				
				if (RequestToken == null || RequestToken.IsEmpty) {
					throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token'");
				}
				return new Url(AuthorizeUri, String.Empty).ToUri(
						Parameter.Token(RequestToken.Value));
			} else {				
				return new Url(AuthorizeUri, String.Empty).ToUri(
					new Parameter("client_id", ConsumerKey),
					new Parameter("redirect_uri", CallbackUri.ToString()));
			}
		}
		IToken _accessToken;
		public IToken AccessToken {
			get {
				return _accessToken;
			}
			set {
				_accessToken = value;
			}
		}
		public Uri GetAccessTokenUrl(IToken token, string verifier) {
			if (token == null) {
				throw new ArgumentNullException("token");
			}
			if (CallbackUri == null) {
				throw new InvalidOperationException("CallbackUri is not specified. 'redirect_uri' is a required attribute.");
			}
			if (AccessUri == null) {
				throw new InvalidOperationException("AccessUri is not specified.");
			}
			if (!AccessUri.IsAbsoluteUri) {
				throw new InvalidOperationException("AccessUri be an absolute uri.");
			}
			if (String.IsNullOrEmpty(ConsumerKey)) {
				throw new InvalidOperationException("ConsumerKey is not specified. 'oauth_consumer_key' is a required attribute.");
			}
			if (String.IsNullOrEmpty(ConsumerSecret)) {
				throw new InvalidOperationException("ConsumerSecret is not specified. 'oauth_consumer_secret' is a required attribute.");
			}
			if (String.IsNullOrEmpty(token.Value)) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token'");
			}
			if (String.IsNullOrEmpty(token.Secret)) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'oauth_token_secret'");
			}
			if (String.IsNullOrEmpty(verifier)) {
				throw new InvalidOperationException("The specified request is invalid. Paramter: 'oauth_verifier' is missing.");
			}
			List<Parameter> parameters = new List<Parameter>();
			parameters.Add(Parameter.Version());
			parameters.Add(Parameter.Nonce());
			parameters.Add(Parameter.Timestamp());
			parameters.Add(Parameter.ConsumerKey(ConsumerKey));
			parameters.Add(Parameter.Token(token.Value));
			parameters.Add(Parameter.Verifier(verifier));
			Url url = new Url(AccessUri, String.Empty);
			switch (Signature) {
				case Signature.PLAINTEXT:
					parameters.Add(Parameter.SignatureMethod(Signature.PLAINTEXT));
					Signing.PlainText.Sign(
						ConsumerSecret,
						token.Secret,
						parameters);
					break;
				case Signature.HMACSHA1:
					parameters.Add(Parameter.SignatureMethod(Signature.HMACSHA1));
					Signing.HmaSha1.Sign(
						HttpMethod,
						url,
						ConsumerSecret,
						token.Secret,
						parameters);
					break;
				default:
					throw new NotImplementedException();
			}
			return url.ToUri(parameters);
		}
		public Uri GetAccessTokenUrl(string code) {
			if (AccessUri == null) {
				throw new InvalidOperationException("AccessUri is not specified.");
			}
			if (!AccessUri.IsAbsoluteUri) {
				throw new InvalidOperationException("AccessUri be an absolute uri.");
			}
			if (!String.Equals(AccessUri.Scheme, "HTTPS", StringComparison.OrdinalIgnoreCase) 
							&& RequireSsl) {
				throw new InvalidOperationException("AccessUri must use the HTTPS protocol.");
			}
			if (CallbackUri == null) {
				throw new InvalidOperationException("CallbackUri is not specified. 'redirect_uri' is a required attribute.");
			}
			if (!CallbackUri.IsAbsoluteUri) {
				throw new InvalidOperationException("CallbackUri be an absolute uri.");
			}
			if (String.IsNullOrEmpty(ConsumerKey)) {
				throw new InvalidOperationException("ConsumerKey is not specified. 'client_id' is a required attribute.");
			}
			if (String.IsNullOrEmpty(ConsumerSecret)) {
				throw new InvalidOperationException("ConsumerSecret is not specified. 'client_secret' is a required attribute.");
			}
			if (String.IsNullOrEmpty(code)) {
				throw new InvalidOperationException("The specified request token is invalid. Paramter: 'code'");
			}
			List<Parameter> parameters = new List<Parameter>();
			parameters.Add(new Parameter("client_id", ConsumerKey));
			parameters.Add(new Parameter("client_secret", ConsumerSecret));
			parameters.Add(new Parameter("code", code));
			parameters.Add(new Parameter("redirect_uri", CallbackUri.ToString()));
			Url url = new Url(AccessUri, String.Empty);
			return url.ToUri(parameters);
		}
#if DEBUGTEST && !SL
		public IToken GetAccessToken(DevExpress.Utils.OAuth.Provider.ServiceProvider serviceProvider, IToken requestToken, string verifier) {
			if (_accessToken != null
						&& !_accessToken.IsEmpty) {
				return _accessToken;
			}
			DevExpress.Utils.OAuth.Provider.Response response = DevExpress.Utils.OAuth.Provider.ServiceProvider.GetAccessToken(
				serviceProvider,
				HttpMethod,
				GetAccessTokenUrl(requestToken, verifier));
			if (response.StatusCode != 200) {
				throw new WebException(String.Format("Http Error {0}. {1}", response.StatusCode, response.Content));
			}
			_accessToken = new Token(
				Parameters.ParseTokens(response.Content, true),
				ConsumerKey,
				ConsumerSecret,
				CallbackUri.ToString(),
				"1.0");
			return _accessToken;
		}
#endif
#if !SL
		public IToken GetAccessToken(IToken requestToken, string verifier) {
			if (_accessToken != null
						&& !_accessToken.IsEmpty) {
				return _accessToken;
			}
			using (WebResponse httpResponse
					= CreateHttpRequest(
						GetAccessTokenUrl(requestToken, verifier), 
						HttpMethod, 
						Cookies).GetResponse()) {
				_accessToken = new Token(
					Parameters.Parse(httpResponse),
					ConsumerKey,
					ConsumerSecret,
					CallbackUri.ToString(),
					"1.0");
				return _accessToken;
			}
		}
#endif
#if !SL
		public IToken GetAccessToken(string code) {
			if (_accessToken != null
						&& !_accessToken.IsEmpty) {			
				return _accessToken;
			}
			using (WebResponse httpResponse 
						= CreateHttpRequest(
									GetAccessTokenUrl(code), 
									HttpMethod, 
									Cookies).GetResponse()) {
				_accessToken = new Token(
					Parameters.Parse(
						httpResponse, 
						false  ),
					ConsumerKey,
					ConsumerSecret,
					CallbackUri.ToString(),
					"2.0");
				return _accessToken;
			}
		}
#endif
		public IAsyncResult BeginGetAccessToken(IToken requestToken, string verifier, AsyncCallback callback, object state) {
			HttpWebRequest request = CreateHttpRequest(
						 GetAccessTokenUrl(requestToken, verifier),
						 HttpMethod,
						 Cookies);
			ConsumerOperation outerAsyncResult = new ConsumerOperation(
				request,
				this,
				"1.0",
				true,
				callback,
				state);
			request.BeginGetResponse(
				OnHttpResponse,
				outerAsyncResult);
			return outerAsyncResult;
		}
		public IAsyncResult BeginGetAccessToken(string code, AsyncCallback callback, object state) {
			HttpWebRequest request = CreateHttpRequest(
						 GetAccessTokenUrl(code),
						 HttpMethod,
						 Cookies);
			ConsumerOperation outerAsyncResult = new ConsumerOperation(
				request,
				this,
				"2.0",
				false, 
				callback,
				state);
			request.BeginGetResponse(
				OnHttpResponse,
				outerAsyncResult);
			return outerAsyncResult;
		}
		public IToken EndGetAccessToken(IAsyncResult ar) {
			if (ar == null) {
				throw new ArgumentNullException("ar", "ar is null.");
			}
			ConsumerOperation operation = ((ConsumerOperation)ar);
			byte[] responseBytes = operation.End(this);
			_requestToken = new Token(
					Parameters.Parse(
						responseBytes,
						operation.Unescape),
					ConsumerKey,
					ConsumerSecret,
					CallbackUri.ToString(),
					operation.Version);
			return _requestToken;
		}
	}
}
