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

using System;
using System.Collections.Generic;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Native.Sql {
	public static class DataAccessConnectionParameter {
		public const string PortParamID = "Port";
		public const string HostnameParamID = "hostname";
		public const string AdvantageServerTypeParamID = "servertype";
		public const string PrivateKeyFileNameParamID = "PrivateKeyFileName";
		public const string ServiceAccountEmailParamID = "ServiceAccountEmail";
		public const string OAuthClientIDParamID = "OAuthClientID";
		public const string OAuthClientSecretParamID = "OAuthClientSecret";
		public const string OAuthRefreshTokenParamID = "OAuthRefreshToken";
		public static Dictionary<string, string> GetParamsDict(IConnectionPage connPage) {
			Dictionary<string, string> paramDict = ConnectionParameter.GetParamsDict(connPage);
			IConnectionPageEx connectionPageEx = connPage as IConnectionPageEx;
			if(connectionPageEx != null)
				AddKey(paramDict, PortParamID, connectionPageEx.Port, true);
#if !DXPORTABLE
			IConnectionPageHostname connectionPageHostname = connPage as IConnectionPageHostname;
			if(connectionPageHostname != null)
				AddKey(paramDict, HostnameParamID, connectionPageHostname.Hostname, true);
			IConnectionPageAdvantage connectionPageAdvantage = connPage as IConnectionPageAdvantage;
			if(connectionPageAdvantage != null)
				AddKey(paramDict, AdvantageServerTypeParamID, connectionPageAdvantage.ServerType.ToString(), true);
			IConnectionPageBigQuery pageBigQuery = connPage as IConnectionPageBigQuery;
			if(pageBigQuery != null) {
				if(pageBigQuery.AuthorizationType == BigQueryAuthorizationType.PrivateKeyFile) {
					AddKey(paramDict, PrivateKeyFileNameParamID, pageBigQuery.PrivateKeyFileName, false);
					AddKey(paramDict, ServiceAccountEmailParamID, pageBigQuery.ServiceAccountEmail, false);
				}
				if(pageBigQuery.AuthorizationType == BigQueryAuthorizationType.OAuth) {
					AddKey(paramDict, OAuthClientIDParamID, pageBigQuery.OAuthClientID, false);
					AddKey(paramDict, OAuthClientSecretParamID, pageBigQuery.OAuthClientSecret, false);
					AddKey(paramDict, OAuthRefreshTokenParamID, pageBigQuery.OAuthRefreshToken, false);
				}
			}
#endif
			return paramDict;
		}
		public static void SetParamsDict(string providerKey, Dictionary<string, string> paramDict, IConnectionPage connPage) {
			ConnectionParameter.SetParamsDict(providerKey, paramDict, connPage);
			IConnectionPageEx connectionPageEx = connPage as IConnectionPageEx;
			if(connectionPageEx != null)
				ReadKey(paramDict, PortParamID, s => { connectionPageEx.Port = s; });
#if !DXPORTABLE
			IConnectionPageHostname connectionPageHost = connPage as IConnectionPageHostname;
			if(connectionPageHost != null)
				ReadKey(paramDict, HostnameParamID, s => { connectionPageHost.Hostname = s; });
			IConnectionPageBigQuery pageBigQuery = connPage as IConnectionPageBigQuery;
			if(pageBigQuery != null) {
				ReadKey(paramDict, PrivateKeyFileNameParamID, s => {
					pageBigQuery.PrivateKeyFileName = s;
				});
				ReadKey(paramDict, ServiceAccountEmailParamID, s => {
					pageBigQuery.ServiceAccountEmail = s;
				});
				ReadKey(paramDict, OAuthClientIDParamID, s => {
					pageBigQuery.OAuthClientID = s;
				});
				ReadKey(paramDict, OAuthClientSecretParamID, s => {
					pageBigQuery.OAuthClientSecret = s;
				});
				ReadKey(paramDict, OAuthRefreshTokenParamID, s => {
					pageBigQuery.OAuthRefreshToken = s;
				});
			}
			IConnectionPageAdvantage pageAdvantage = connPage as IConnectionPageAdvantage;
			if(pageAdvantage != null) {
				ReadKey(paramDict, AdvantageServerTypeParamID, s => {
					switch(s.ToLowerInvariant()) {
						case "remote":
							pageAdvantage.ServerType = AdvantageServerType.Remote;
							break;
						case "internet":
							pageAdvantage.ServerType = AdvantageServerType.Internet;
							break;
						default:
							pageAdvantage.ServerType = AdvantageServerType.Local;
							break;
					}
				});
			}
#endif
		}
		static void AddKey(IDictionary<string, string> destination, string id, string value, bool skipEmptyValues) {
			if(skipEmptyValues ? !string.IsNullOrWhiteSpace(value) : value != null)
				destination.Add(id, value);
		}
		static void ReadKey(IDictionary<string, string> source, string key, Action<string> apply) {
			if(source.ContainsKey(key))
				apply(source[key]);
		}
	}
}
