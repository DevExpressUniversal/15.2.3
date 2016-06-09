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
	public struct Url {		
		public Url(Uri requestUri, string authorizationHeader) {
			if (requestUri == null) {
				throw new ArgumentNullException("requestUri");
			}
			if (!requestUri.IsAbsoluteUri) {
				throw new ArgumentException("Request URI must be absolute.", "requestUri");
			}
			string uriHost = requestUri.Host;
			string uriScheme = requestUri.Scheme;
			string uriPort = String.Empty;
			string uriPath = requestUri.AbsolutePath;
			if ((String.Equals(uriScheme, "http", StringComparison.InvariantCultureIgnoreCase) && requestUri.Port == 80) ||
					(String.Equals(uriScheme, "https", StringComparison.InvariantCultureIgnoreCase) && requestUri.Port == 443)) {
				uriPort = String.Empty;
			} else {
				uriPort = ":" + requestUri.Port;
			}
			_domain = String.Format("{0}://{1}{2}", uriScheme, uriHost, uriPort);
			_uri = _domain + uriPath;
			_queryParams = Parameters.FromUri(requestUri, authorizationHeader);
		}
		public static explicit operator Url(string value) {
			if (String.IsNullOrEmpty(value)) {
				return new Url();
			}
			return new Url(
				new Uri(value, UriKind.RelativeOrAbsolute), String.Empty);
		}
		string _uri;
		public string Uri {
			get {
				return _uri;
			}
		}
		string _domain;
		public string Domain {
			get {
				return _domain;
			}
		}
		IEnumerable<Parameter> _queryParams;
		public IEnumerable<Parameter> QueryParams {
			get {
				return _queryParams;
			}
		}
		public IEnumerable<Parameter> GetQueryParams(string name) {
			List<Parameter> list = new List<Parameter>();
			if (string.IsNullOrEmpty(name)) return list;
			foreach (Parameter i in _queryParams) {
				if (!i.IsEmpty
									&& String.Equals(name, i.Name, StringComparison.InvariantCultureIgnoreCase)) {
					list.Add(i);
				}
			}
			return list;
		}
		public override string ToString() {
			return String.IsNullOrEmpty(_uri) ?
				"{}" : String.Format("{0}", _uri);
		}
		public Uri ToUri(params Parameter[] additional) {
			if (String.IsNullOrEmpty(_uri)) {
				throw new InvalidOperationException();
			}
			return ToUri((IEnumerable<Parameter>)additional);
		}
		public Uri ToUri(IEnumerable<Parameter> additional) {
			if (String.IsNullOrEmpty(_uri)) {
				throw new InvalidOperationException();
			}
			StringBuilder uriBuilder = new StringBuilder();
			uriBuilder.Append(_uri.ToLowerInvariant());
			bool bIsFirst = true;
			foreach (Parameter p in Parameters.Sort(_queryParams, additional)) {
				if (!bIsFirst) {
					uriBuilder.Append("&");
				} else {
					uriBuilder.Append("?");
					bIsFirst = false;
				}
				uriBuilder.Append(Escaping.Escape(p.Name));
				if (!String.IsNullOrEmpty(p.Value)) {
					uriBuilder.Append("=");
					uriBuilder.Append(Escaping.Escape(p.Value));
				}
			}
			return new Uri(
				uriBuilder.ToString(),
				UriKind.Absolute);
		}
		public static string ToDomain(string uriString) {
			return new Url(new Uri(uriString), String.Empty).Domain;
		}
		public Url ToUrl() {
			return ToUrl(this.QueryParams);
		}
		public Url ToUrl(IEnumerable<Parameter> replace) {
			if (replace == null) {
				replace = new Parameter[] {};
			} 
			Url url = new Url();
			url._uri = _uri;
			url._queryParams = replace;
			return url;
		}
	}
}
