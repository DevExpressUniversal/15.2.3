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
	using System.Collections.Generic;
	public class RequestTokenStore : TokenStore, IRequestTokenStore {
		static Dictionary<string, IToken> s_cache = new Dictionary<string, IToken>();
		public override IToken GetToken(string token) {
			if (String.IsNullOrEmpty(token)) {
				return null;
			}
			IToken t;
			lock (s_cache) {
				if (!s_cache.TryGetValue(token, out t)) {
					t = null;
				}
			}
			return t;
		}
		public virtual IToken AuthorizeToken(string token, string authenticationTicket) {
			if (String.IsNullOrEmpty(token)) {
				throw new ArgumentException("token is null or empty.", "token");
			}
			if (String.IsNullOrEmpty(authenticationTicket)) {
				throw new ArgumentException("authenticationTicket is null or empty.", "authenticationTicket");
			}
			IToken unauthorizeToken = GetToken(token);
			if (unauthorizeToken == null
							|| unauthorizeToken.IsEmpty) {
				return null;
			} 
			IToken authorizeToken = new Token(
				unauthorizeToken.ConsumerKey,
				unauthorizeToken.ConsumerSecret,
				unauthorizeToken.Value,
				unauthorizeToken.Secret,
				authenticationTicket,
				unauthorizeToken.Verifier,
				unauthorizeToken.Callback);
			lock (s_cache) {
				s_cache[authorizeToken.Value] = authorizeToken;
			}
			return authorizeToken;
		}		
		public virtual IToken CreateUnauthorizeToken(string consumerKey, string consumerSecret, string callback) {
			if (String.IsNullOrEmpty(consumerKey))
				throw new ArgumentException("consumerKey is null or empty.", "consumerKey");
			if (String.IsNullOrEmpty(consumerSecret))
				throw new ArgumentException("consumerSecret is null or empty.", "consumerSecret");
			if (String.IsNullOrEmpty(callback))
				throw new ArgumentException("callback is null or empty.", "callback");
			IToken unauthorizedToken = new Token(
					consumerKey,
					consumerSecret,
					Token.NewToken(TokenLength.Long),
					Token.NewToken(TokenLength.Long),
					String.Empty, 
					Token.NewToken(TokenLength.Short),
					callback);
			lock (s_cache) {
				s_cache[unauthorizedToken.Value] = unauthorizedToken;
			}
			return unauthorizedToken;
		}
	}
}
#endif
