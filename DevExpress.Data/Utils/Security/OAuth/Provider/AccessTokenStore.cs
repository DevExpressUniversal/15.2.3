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
	public class AccessTokenStore : TokenStore, IAccessTokenStore {
		static Dictionary<string, IToken> s_cache = new Dictionary<string, IToken>();
		public override IToken GetToken(string token) {
			if (String.IsNullOrEmpty(token)) {
				return null;
			}
			IToken accessToken;
			lock (s_cache) {
				if (!s_cache.TryGetValue(token, out accessToken)) {
					accessToken = null;
				}
			}
			return accessToken;
		}
		public virtual void RevokeToken(string token) {
			if (String.IsNullOrEmpty(token)) {
				return;
			}
			lock (s_cache) {
				s_cache.Remove(token);
			}
		}
		public virtual IToken CreateToken(IToken requestToken) {
			if (requestToken == null || requestToken.IsEmpty) {
				throw new ArgumentException("requestToken is null or empty.", "requestToken");
			}
			Token token = new Token(
				requestToken.ConsumerKey,
				requestToken.ConsumerSecret,
				Token.NewToken(TokenLength.Long),
				Token.NewToken(TokenLength.Long),
				requestToken.AuthenticationTicket,
				requestToken.Verifier,
				requestToken.Callback
			);
			lock (s_cache) {
				s_cache[token.Value] = token;
			}
			return token;
		}
	}
}
#endif
