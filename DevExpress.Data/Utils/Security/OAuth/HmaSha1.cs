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
	public static partial class Signing {
		public static class HmaSha1 {
			public static string Base(
				string httpMethod,
				Url url,
				IEnumerable<Parameter> parameters) {
				return Base(httpMethod,
						url,
						true,
						parameters);
			}
			public static string Base(
				string httpMethod,
				Url url,
				bool lowerCase,
				IEnumerable<Parameter> parameters) {
				StringBuilder sig = new StringBuilder();
				if (!String.IsNullOrEmpty(httpMethod)) {
					sig.Append(Escaping.Escape(httpMethod.ToUpperInvariant()));
				}
				if (!String.IsNullOrEmpty(url.Uri)) {
					if (sig.Length > 0) {
						sig.Append("&");
					}
					if (lowerCase) {
						sig.Append(Escaping.Escape(url.Uri.ToLowerInvariant()));
					} else {
						sig.Append(Escaping.Escape(url.Uri));
					}
				}
				bool first = true;
				foreach (Parameter p in Parameters.Sort(url.QueryParams, parameters)) {
					if (String.IsNullOrEmpty(p.Name)) {
						continue;
					}
					if (String.Equals(p.Name, "oauth_signature", StringComparison.InvariantCultureIgnoreCase) ||
						String.Equals(p.Name, "oauth_token_secret", StringComparison.InvariantCultureIgnoreCase) ||
						String.Equals(p.Name, "oauth_consumer_secret", StringComparison.InvariantCultureIgnoreCase)) {
						continue;
					}
					if (sig.Length > 0) {
						sig.Append(first ? "&" : Escaping.Escape("&"));
					}
					first = false;
					sig.Append(Escaping.Escape(p.Name));
					if (!String.IsNullOrEmpty(p.Value)) {
						sig.Append(Escaping.Escape("="));
						sig.Append((Escaping.Escape(Escaping.Escape(p.Value)))); 
					}
				}
#if DEBUGTEST && DEBUG
				System.Diagnostics.Debug.WriteLine("SigBase: " + sig.ToString());
#endif
				return sig.ToString();
			}
			public static string Hash(string sigBase, string key) {
				using (System.Security.Cryptography.HMACSHA1 hash
							= new System.Security.Cryptography.HMACSHA1(Encoding.UTF8.GetBytes(key))) {
					return Convert.ToBase64String(
						hash.ComputeHash(Encoding.UTF8.GetBytes(sigBase)));
				}
			}
			public static string Hash(string sigBase, string consumerSecret, string tokenSecret) {
				return Hash(sigBase, String.Format("{0}&{1}",
						Escaping.Escape(consumerSecret),
						Escaping.Escape(tokenSecret)));
			}
			public static Parameter Sign(string sigBase, string consumerSecret, string tokenSecret) {
				return new Parameter(
								"oauth_signature",
								Hash(sigBase, consumerSecret, tokenSecret));
			}
			public static void Sign(string httpMethod, Url url, string consumerSecret, string tokenSecret,
							ICollection<Parameter> parameters) {
				parameters.Add(
					Sign(Base(httpMethod, url, parameters),
							consumerSecret,
							tokenSecret));
			}
		}
	}
}
