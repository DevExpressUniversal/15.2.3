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

using System.Collections.Generic;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.Native {
	public class DataConnectionParametersComparer : IEqualityComparer<DataConnectionParametersBase> {
		bool IEqualityComparer<DataConnectionParametersBase>.Equals(DataConnectionParametersBase x, DataConnectionParametersBase y) {
			return Equals(x, y);
		}
		public static bool Equals(DataConnectionParametersBase x, DataConnectionParametersBase y) {
			if(ReferenceEquals(x, y))
				return true;
			if(y == null || x == null)
				return false;
			if(y.GetType() != x.GetType())
				return false;
			IConnectionPage pageX = x;
			IConnectionPage pageY = y;
			if(!Equals(pageX.FileName, pageY.FileName))
				return false;
			if(!Equals(pageX.Password, pageY.Password))
				return false;
			if(!Equals(pageX.UserName, pageY.UserName))
				return false;
			if(!Equals(pageX.ServerName, pageY.ServerName))
				return false;
			if(!Equals(pageX.DatabaseName, pageY.DatabaseName))
				return false;
			if(!Equals(pageX.CustomConStr, pageY.CustomConStr))
				return false;
			if(pageX.AuthType != pageY.AuthType)
				return false;
			IConnectionPageEx pageExX = x as IConnectionPageEx;
			IConnectionPageEx pageExY = y as IConnectionPageEx;
			if(!ReferenceEquals(pageExX, null) || !ReferenceEquals(pageExY, null))
				return Equals(pageExX, pageExY);
#if !DXPORTABLE
			IConnectionPageBigQuery pageKeyBigQueryX = x as IConnectionPageBigQuery;
			IConnectionPageBigQuery pageKeyBigQueryY = y as IConnectionPageBigQuery;
			if(!ReferenceEquals(pageKeyBigQueryX, null) || !ReferenceEquals(pageKeyBigQueryY, null))
				return Equals(pageKeyBigQueryX, pageKeyBigQueryY);
			IConnectionPageHostname pageHostnameX = x as IConnectionPageHostname;
			IConnectionPageHostname pageHostnameY = y as IConnectionPageHostname;
			if(!ReferenceEquals(pageHostnameX, null) || !ReferenceEquals(pageHostnameY, null))
				return Equals(pageHostnameX, pageHostnameY);
			IConnectionPageAdvantage pageAdvantageX = x as IConnectionPageAdvantage;
			IConnectionPageAdvantage pageAdvantageY = y as IConnectionPageAdvantage;
			if(!ReferenceEquals(pageAdvantageX, null) || !ReferenceEquals(pageAdvantageY, null))
				return Equals(pageAdvantageX, pageAdvantageY);
#endif
			return true;
		}
		static bool Equals(IConnectionPageEx x, IConnectionPageEx y) {
			if(ReferenceEquals(x, y))
				return true;
			if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			if(!Equals(x.Port, y.Port))
				return false;
			return true;
		}
#if !DXPORTABLE
		static bool Equals(IConnectionPageAdvantage x, IConnectionPageAdvantage y) {
			if(ReferenceEquals(x, y))
				return true;
			if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			if(!Equals(x.ServerType, y.ServerType))
				return false;
			return true;
		}
		static bool Equals(IConnectionPageHostname x, IConnectionPageHostname y) {
			if(ReferenceEquals(x, y))
				return true;
			if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			if(!Equals(x.Hostname, y.Hostname))
				return false;
			return true;
		}
		static bool Equals(IConnectionPageBigQuery x, IConnectionPageBigQuery y) {
			if(ReferenceEquals(x, y))
				return true;
			if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			if(!Equals(x.AuthorizationType, y.AuthorizationType))
				return false;
			if(x.AuthorizationType == BigQueryAuthorizationType.PrivateKeyFile) {
				if(!Equals(x.PrivateKeyFileName, y.PrivateKeyFileName))
					return false;
				if(!Equals(x.ServiceAccountEmail, y.ServiceAccountEmail))
					return false;
			}
			if(x.AuthorizationType == BigQueryAuthorizationType.OAuth) {
				if(!Equals(x.OAuthClientID, y.OAuthClientID))
					return false;
				if(!Equals(x.OAuthClientSecret, y.OAuthClientSecret))
					return false;
				if(!Equals(x.OAuthRefreshToken, y.OAuthRefreshToken))
					return false;
			}
			return true;
		}
#endif
		public static bool Equals(SqlDataConnection x, SqlDataConnection y) {
			if(ReferenceEquals(x, y))
				return true;
			if(ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;
			return Equals(x.ConnectionParameters, y.ConnectionParameters);
		}
		public int GetHashCode(DataConnectionParametersBase obj) {
			return 0;
		}
	}
}
