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
	using System.IO;
	using System.Net;
	using System.Diagnostics;
	public struct Parameter : IComparable<Parameter>, IEquatable<Parameter> {
		public static readonly Parameter Empty = new Parameter();
		public static Parameter Parse(string token) {
			return Parse(token, true, false);
		}
		public static Parameter Parse(string token, bool unescape, bool unquote) {
			if (String.IsNullOrEmpty(token)) {
				return new Parameter();
			}
			return Parse(token, 0, token.Length, unescape, unquote);
		}
		public static Parameter Parse(string token, int index, int count, bool unescape, bool unquote) {
			if (token == null) {
				throw new ArgumentNullException("token");
			}
			if (count == 0) {
				return new Parameter();
			}
			int length = token.Length;
			if (index < 0) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (index > length) {
				throw new ArgumentOutOfRangeException("index");
			}
			if (length < 0) {
				throw new ArgumentOutOfRangeException("count");
			}
			if (index > (length - count)) {
				throw new ArgumentOutOfRangeException("count");
			}
			int eq = token.IndexOf('=', index, count);
			if (eq == 0 || index == eq) {
				throw new FormatException(
						String.Format("The specified token is not valid. {0}",
								token.Substring(index, count)));
			}
			if (eq < 0) {
				return new Parameter(
					Escaping.Unescape(token, index, count, unescape, unquote),
					String.Empty);
			} else {
				return new Parameter(
					Escaping.Unescape(token, index, eq - index, unescape, unquote),
					Escaping.Unescape(token, eq + 1, count - ((eq + 1) - index), unescape, unquote));
			}
		}
		public static Parameter Version(string version) {
			if (String.IsNullOrEmpty(version)) {
				throw new ArgumentNullException("version");
			}
			return new Parameter("oauth_version", version);
		}
		public static Parameter Version() {
			return new Parameter("oauth_version", "1.0");
		}
		public static Parameter Nonce() {
			return new Parameter("oauth_nonce", DevExpress.Utils.OAuth.Nonce.CreateNonce().Value);
		}
		public static Parameter Timestamp() {
			DateTime TimeOffset = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			long seconds
					= (long)(DateTime.UtcNow - TimeOffset).TotalSeconds;
			return new Parameter("oauth_timestamp", seconds.ToString());
		}
		public bool IsTimestamp() {
			if (IsEmpty
					|| !string.Equals(Name, "oauth_timestamp", StringComparison.OrdinalIgnoreCase)) {
				return false;
			}
			DateTime TimeOffset = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			long timestamp;
			if (!long.TryParse(Value, out timestamp)) {
				return false;
			}
			long now = (long)(DateTime.UtcNow - TimeOffset).TotalSeconds;
			if (timestamp < now - 60
								|| timestamp > now + 60) {
				return false;
			}
			return true;
		}
		public bool IsNonce() {
			if (IsEmpty || !string.Equals(Name, "oauth_nonce", StringComparison.OrdinalIgnoreCase)
							|| string.IsNullOrEmpty(Value)) {
				return false;
			}
			return true;
		}
		public static Parameter Callback(string callback) {
			if (String.IsNullOrEmpty(callback)) {
				throw new ArgumentNullException("callback");
			}
			return new Parameter("oauth_callback", callback);
		}
		public static Parameter Verifier(string verifier) {
			if (String.IsNullOrEmpty(verifier)) {
				throw new ArgumentNullException("verifier");
			}
			return new Parameter("oauth_verifier", verifier);
		}
		public static Parameter SignatureMethod(Signature sig) {
			switch (sig) {
				case Signature.HMACSHA1:
					return new Parameter("oauth_signature_method", "HMAC-SHA1");
				case Signature.PLAINTEXT:
					return new Parameter("oauth_signature_method", "PLAINTEXT");
				case Signature.RSASHA1:
					return new Parameter("oauth_signature_method", "RSA-SHA1");
				default:
					throw new NotSupportedException();
			}
		}
		public static Parameter Token(string token) {
			if (String.IsNullOrEmpty(token)) {
				throw new ArgumentNullException("token");
			}
			return new Parameter("oauth_token", token);
		}
		public static Parameter ConsumerKey(string key) {
			if (String.IsNullOrEmpty(key)) {
				throw new ArgumentNullException("key");
			}
			return new Parameter("oauth_consumer_key", key);
		}
		public Parameter(string name, string value) {
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			_name = name;
			_value = value ?? String.Empty;
		}
		public Signature ToSignatureMethod() {
			if (!String.Equals(Name, "oauth_signature_method", StringComparison.InvariantCultureIgnoreCase)) {
				throw new InvalidOperationException(String.Format("Parameter \"oauth_signature_method\" expected but \"{0}\" found.", Name));
			}
			switch (Value) {
				case "HMAC-SHA1":
					return Signature.HMACSHA1;
				case "PLAINTEXT":
					return Signature.PLAINTEXT;
				case "RSASHA1":
					return Signature.RSASHA1;
				default:
					throw new InvalidOperationException(
						String.Format("oauth_signature_method: \"{0}\" is not supported.", Value));
			}
		}
		public bool IsEmpty {
			get {
				return String.IsNullOrEmpty(_name);
			}
		}
		string _name;
		public string Name {
			get {
				if (_name == null) {
					return String.Empty;
				}
				return _name;
			}
		}
		string _value;
		public string Value {
			get {
				if (_value == null) {
					return String.Empty;
				}
				return _value;
			}
		}
		public static bool operator !=(Parameter left, Parameter right) {
			return !(left == right);
		}
		public override bool Equals(object source) {
			if ((source == null)
					|| !(source is Parameter)) {
				return false;
			}
			return Equals(this, (Parameter)source);
		}
		public bool Equals(Parameter value) {
			return Equals(this, value);
		}
		public static bool operator ==(Parameter left, Parameter right) {
			return Parameter.Equals(left, right);
		}
		private static bool Equals(Parameter left, Parameter right) {
			if (left.IsEmpty) {
				return right.IsEmpty;
			}
			return ((left.Name == right.Name)
				 && (left.Value == right.Value));
		}
		public int CompareTo(Parameter other) {
			int compare
				= String.Compare(Name, other.Name, StringComparison.Ordinal);
			if (compare == 0) {
				compare = String.Compare(Value, other.Value, StringComparison.Ordinal);
			}
			return compare;
		}
		public override int GetHashCode() {
			if (IsEmpty) {
				return 0;
			}
			return (Name.GetHashCode() ^ Value.GetHashCode());
		}
	}
	public class RequiredParameters : IEnumerable<Parameter> {
		public RequiredParameters(Url url) {
			_Url = url;
		}
		Dictionary<string, Parameter> _HashTable
			= new Dictionary<string, Parameter>(StringComparer.InvariantCultureIgnoreCase);
		public IEnumerator<Parameter> GetEnumerator() {
			return _HashTable.Values.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		Url _Url;
		public Url Url {
			get {
				return _Url;
			}
		}
		IList<ValidationError> _Errors = new List<ValidationError>();
		public IEnumerable<ValidationError> Errors {
			get {
				return _Errors;
			}
		}
		public Parameter this[string name] {
			get {
				if (string.IsNullOrEmpty(name)) {
					return Parameter.Empty;
				}
				Parameter found;
				if (!_HashTable.TryGetValue(name, out found)) {
					found = Parameter.Empty;
				}
				return found;
			}
		}
		public void Require(params string[] names) {
			if (names != null && names.Length > 0) {
				foreach (string name in names)
					Match(name, true, null);
			}
		}
		public void Match(string name, bool required, string defaultValue) {
			Parameter param;
			ValidationError error;
			if (DoMatch(Url, name, required, out param, out error)) {
				if (!param.IsEmpty) {
					_HashTable[name] = new Parameter(
						param.Name, 
						param.Value);
				}
			} else if (!required) {
				_HashTable[name] = new Parameter(name, defaultValue);
			}
			if (error != null) {
				_Errors.Add(error);
			}
		}
		static bool DoMatch(Url url, string name, bool required,
			out Parameter found,
			out ValidationError error) {
			found = Parameter.Empty;
			error = null;
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentException("name is null or empty.", "name");
			}
			IEnumerable<Parameter> args = url.GetQueryParams(name);
			if (args != null) {
				foreach (Parameter i in args) {
					if (!found.IsEmpty) {
						error = (new ValidationError(
							(int)HttpStatusCode.BadRequest,
								"Duplicated OAuth Protocol Parameter: " + name));
						found = Parameter.Empty;
						return false;
					}
					found = i;
				}
			}
			if (!found.IsEmpty) {
				error = null;
				return true;
			}
			if (required) {
				error = (new ValidationError(
						(int)HttpStatusCode.BadRequest,
						"Missing required parameter: " + name));
			}
			return false;
		}
	}
	public class Parameters : IList<Parameter>, ICollection<Parameter>, IEnumerable<Parameter> {
		public static Parameters Parse(StreamReader reader, bool unescape) {
			if (reader == null) {
				throw new ArgumentNullException("reader");
			}
			return ParseTokens(reader.ReadToEnd(), unescape);
		}
		public static Parameters Parse(Stream stream, bool unescape) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8)) {
				return Parse(reader, unescape);
			}
		}
		public static Parameters Parse(byte[] bytes) { return Parse(bytes, true); }
		public static Parameters Parse(byte[] bytes, bool unescape) {
			using (Stream stream = new MemoryStream(bytes)) {
				return Parse(stream, unescape);
			}
		}
		public static Parameters Parse(WebResponse response) { return Parse(response, true); }
		public static Parameters Parse(WebResponse response, bool unescape) {
			if (response == null) {
				throw new ArgumentNullException("response");
			}
			using (Stream stream = response.GetResponseStream()) {
				return Parse(stream, unescape);
			}
		}
		public static Parameters ParseTokens(string tokens) { return ParseTokens(tokens, true); }
		public static Parameters ParseTokens(string tokens, bool unescape) {
			Parameters list = new Parameters();
			if (tokens == null
						|| tokens.Length <= 0) {
				return list;
			}
			int i = 0;
			int e = 0;
			while (i < tokens.Length) {			   
				if (tokens[i] == '&') {
					list.Add(Parameter.Parse(tokens, e, i - e, unescape, false));
					e = ++i;
				} else {
					i++;
				}
			}
			if (e <= tokens.Length) {
				list.Add(Parameter.Parse(tokens, e, tokens.Length - e, unescape, false));
			}
			return list;
		}
		public static Parameters FromUri(Uri uri, string authorizationHeader) {
			Parameters list = new Parameters();
			ParseQueryString(uri, list);
			ParseAuthorizationHeader(authorizationHeader, list);
			return list;
		}
		static void ParseAuthorizationHeader(string authorizationHeader, Parameters list) {
			Debug.Assert(list != null, "list is null.");
			if (authorizationHeader == null 
									|| authorizationHeader.Length <= 0) {
				return;
			}
			if (!authorizationHeader.StartsWith("OAuth ",
								StringComparison.InvariantCultureIgnoreCase)) {
									throw new ArgumentException("The specified authorization header must start with 'OAuth'", "authorizationHeader");
			}
			authorizationHeader = authorizationHeader.Substring("OAuth ".Length);
			int i = 0;
			int br = i;
			while (i < authorizationHeader.Length) {
				if (authorizationHeader[i] == ',') {
					list.Add(Parameter.Parse(authorizationHeader, br, i - br, true, true));
					br = ++i;
				} else {
					i++;
				}
			}
			if (br <= authorizationHeader.Length) {
				list.Add(Parameter.Parse(authorizationHeader, br, authorizationHeader.Length - br, true, true));
			}
		}
		static void ParseQueryString(Uri uri, Parameters list) {
			Debug.Assert(list != null, "list is null.");
			if (uri == null
						|| !uri.IsAbsoluteUri) {
				return;
			}
			string queryString = uri.Query;
			if (String.IsNullOrEmpty(queryString)) {
				return;
			}
			int i = 0;
			while (char.IsWhiteSpace(queryString[i])
							|| queryString[i] == '?') {
				i++;
			}
			int br = i;
			while (i < queryString.Length) {
				if (queryString[i] == '&') {
					list.Add(Parameter.Parse(queryString, br, i - br, true, false));
					br = ++i;
				} else {
					i++;
				}
			}
			if (br <= queryString.Length) {
				list.Add(Parameter.Parse(queryString, br, queryString.Length - br, true, false));
			}
		}
		static int CompareParams(Parameter x, Parameter y) {
			return x.CompareTo(y);
		}
		public static IList<Parameter> Sort(params IEnumerable<Parameter>[] args) {
			List<Parameter> sorted = new List<Parameter>();
			if (args != null && args.Length > 0) {
				foreach (IEnumerable<Parameter> l in args) {
					if (l != null) {
						foreach (Parameter p in l) {
							if (!p.IsEmpty) {
								sorted.Add(p);
							}
						}
					}
				}
				sorted.Sort(CompareParams);
			}
			return sorted;
		}
		IList<Parameter> _items;
		private Parameters(IList<Parameter> items) {
			if (items == null) {
				throw new ArgumentNullException("items");
			}
			_items = items;
		}
		public Parameters() {
			_items = new List<Parameter>();
		}
		public void Add(Parameter item) {
			_items.Add(item);
		}
		public void Clear() {
			_items.Clear();
		}
		public bool Contains(Parameter item) {
			return _items.Contains(item);
		}
		public void CopyTo(Parameter[] array, int index) {
			_items.CopyTo(array, index);
		}
		public int Count {
			get {
				return _items.Count;
			}
		}
		public bool IsReadOnly {
			get {
				return false;
			}
		}
		public bool Remove(Parameter item) {
			return _items.Remove(item);
		}
		public IEnumerator<Parameter> GetEnumerator() {
			return _items.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _items.GetEnumerator();
		}
		public int IndexOf(Parameter item) {
			return _items.IndexOf(item);
		}
		public void Insert(int index, Parameter item) {
			_items.Insert(index, item);
		}
		public void RemoveAt(int index) {
			_items.RemoveAt(index);
		}
		public Parameter this[string name] {
			get {
				foreach (Parameter p in _items) {
					if (String.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)) {
						return p;
					}
				}
				return new Parameter();
			}
		}
		public Parameter this[int index] {
			get {
				return _items[index];
			}
			set {
				_items[index] = value;
			}
		}
	}
}
