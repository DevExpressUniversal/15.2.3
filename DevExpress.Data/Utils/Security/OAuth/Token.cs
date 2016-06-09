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
	public enum TokenLength {
		Short,
		Long,
	}
	[Serializable]
	public struct Token : IToken {
		public static readonly Token Empty = new Token();
		public Token(Parameters paramters, string consumerKey, string consumerSecret, string callback, string version) {
			if (paramters == null) {
				throw new ArgumentNullException("paramters");
			}
			if (String.IsNullOrEmpty(version)) {
				version = "1.0";
			}
			if (String.IsNullOrEmpty(consumerKey)) {
				throw new ArgumentNullException("consumerKey");
			}
			if (String.IsNullOrEmpty(consumerSecret)) {
				throw new ArgumentNullException("consumerSecret");
			}
			_callback = callback;
			_verifier = String.Empty;
			_authenticationTicket = String.Empty;
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			switch (version) {
				case "1.0":
					_value = paramters["oauth_token"].Value;
					_secret = paramters["oauth_token_secret"].Value;
					_isCallbackConfirmed =
						String.Equals("true", paramters["oauth_callback_confirmed"].Value, StringComparison.OrdinalIgnoreCase);
					break;
				case "2.0":
					_value = paramters["access_token"].Value;
					_secret = "n/a";
					_isCallbackConfirmed = true;
					break;
				default:
					throw new ArgumentException(String.Format("The specified OAuth version '{0}' is not supported.",
						version), "version");
			}
		}
		public Token(string consumerKey,
			string consumerSecret,
			string value,
			string secret,
			string authenticationTicket,
			string verifier,
			string callback) {
			if (String.IsNullOrEmpty(value)) {
				throw new ArgumentNullException("value");
			}
			if (String.IsNullOrEmpty(secret)) {
				throw new ArgumentNullException("secret");
			}
			if (String.IsNullOrEmpty(consumerKey)) {
				throw new ArgumentNullException("consumerKey");
			}
			if (String.IsNullOrEmpty(consumerSecret)) {
				throw new ArgumentNullException("consumerSecret");
			}
			_value = value;
			_secret = secret;
			_authenticationTicket = authenticationTicket;
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_verifier = verifier;
			_callback = callback;
			_isCallbackConfirmed = true;
		}
		public Token(
			string consumerKey,
			string consumerSecret,
			string value,
			string secret)
			: this(consumerKey,
				consumerSecret,
				value,
				secret,
				String.Empty,
				String.Empty,
				String.Empty) {
		}
		public static string NewToken(TokenLength length) {
			string token =
				Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
			int count = token.Length;
			while (token[count - 1] == '=') {
				count--;
			}
			if (count == 0) throw new InvalidOperationException();
			if (length == TokenLength.Short) {
				count = Math.Min(6, count);
			}
			return token.Substring(0, count);
		}
		public bool IsEmpty {
			get {
				return String.IsNullOrEmpty(Value) ||
					String.IsNullOrEmpty(Secret) ||
					String.IsNullOrEmpty(ConsumerKey) ||
					String.IsNullOrEmpty(ConsumerSecret);
			}
		}
		string _value;
		public string Value {
			get {
				return _value;
			}
		}
		bool _isCallbackConfirmed;
		public bool IsCallbackConfirmed {
			get {
				return _isCallbackConfirmed;
			}
		}
		string _secret;
		public string Secret {
			get {
				return _secret;
			}
		}
		string _callback;
		public string Callback {
			get {
				return _callback;
			}
		}
		string _authenticationTicket;
		public string AuthenticationTicket {
			get {
				return _authenticationTicket;
			}
		}
		string _consumerKey;
		public string ConsumerKey {
			get {
				return _consumerKey;
			}
		}
		string _consumerSecret;
		public string ConsumerSecret {
			get {
				return _consumerSecret;
			}
		}
		string _verifier;
		public string Verifier {
			get {
				return _verifier;
			}
		}
	}
}
