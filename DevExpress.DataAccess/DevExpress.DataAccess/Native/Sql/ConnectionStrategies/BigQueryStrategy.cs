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
using DevExpress.DataAccess.Native.Sql.ConnectionProviders;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	public class BigQueryStrategy : ServerDbStrategyBase {
		const ConnectionParameterEdits editsKeyFile = ConnectionParameterEdits.KeyFileName | ConnectionParameterEdits.ServiceEmail;
		const ConnectionParameterEdits editsOAuth = ConnectionParameterEdits.OAuthClientID | ConnectionParameterEdits.OAuthClientSecret | ConnectionParameterEdits.OAuthRefreshToken;
		DataAccessBigQueryProviderFactory factory;
		#region Overrides of ServerDbStrategyBase
		public override ConnectionParameterEdits GetEditsSet(IConnectionParametersControl control) {
			return ConnectionParameterEdits.ProjectID | ConnectionParameterEdits.DataSetID |
				   ConnectionParameterEdits.AuthTypeBigQuery |
				   (control.AuthTypeBigQuery == BigQueryAuthorizationType.PrivateKeyFile ? editsKeyFile : editsOAuth);
		}
		public override ConnectionParameterEdits Subscriptions { get { return ConnectionParameterEdits.AuthTypeBigQuery; } }
		protected override ProviderFactory Factory { get { return ProviderFactory; } }
		DataAccessBigQueryProviderFactory ProviderFactory { get { return this.factory ?? (this.factory = new DataAccessBigQueryProviderFactory()); } }
		public override int GetDefaultIndex(ConnectionParameterEdits edit) {
			return edit == ConnectionParameterEdits.AuthTypeBigQuery ? 0 : 1;
		}
		public override string GetDefaultText(ConnectionParameterEdits edit) {
			switch(edit) {
				case ConnectionParameterEdits.ProjectID:
				case ConnectionParameterEdits.DataSetID:
				case ConnectionParameterEdits.KeyFileName:
				case ConnectionParameterEdits.ServiceEmail:
				case ConnectionParameterEdits.OAuthClientSecret:
				case ConnectionParameterEdits.OAuthRefreshToken:
					return string.Empty;
				case ConnectionParameterEdits.OAuthClientID:
					return "***.apps.googleusercontent.com";
				default:
					return base.GetDefaultText(edit);
			}
		}
		#endregion
		#region Implementation of IConnectionParametersStrategy
		public override DataConnectionParametersBase GetConnectionParameters(IConnectionParametersControl control) {
			switch(control.AuthTypeBigQuery) {
				case BigQueryAuthorizationType.OAuth:
					return new BigQueryConnectionParameters(control.ProjectID, control.DataSetID, control.OAuthClientID, control.OAuthClientSecret, control.OAuthRefreshToken);
				case BigQueryAuthorizationType.PrivateKeyFile:
					return new BigQueryConnectionParameters(control.ProjectID, control.DataSetID, control.ServiceEmail, control.KeyFileName);
				default:
					throw new ArgumentException();
			}
		}
		public override void InitializeControl(IConnectionParametersControl control, DataConnectionParametersBase value) {
			BigQueryConnectionParameters p = (BigQueryConnectionParameters)value;
			control.ProjectID = p.ProjectID;
			control.DataSetID = p.DataSetID;
			control.AuthTypeBigQuery = p.AuthorizationType;
			control.KeyFileName = p.AuthorizationType == BigQueryAuthorizationType.PrivateKeyFile ? p.PrivateKeyFileName : string.Empty;
			control.ServiceEmail = p.AuthorizationType == BigQueryAuthorizationType.PrivateKeyFile ? p.ServiceAccountEmail : string.Empty;
			control.OAuthClientID = p.AuthorizationType == BigQueryAuthorizationType.OAuth ? p.OAuthClientID : string.Empty;
			control.OAuthClientSecret = p.AuthorizationType == BigQueryAuthorizationType.OAuth ? p.OAuthClientSecret : string.Empty;
			control.OAuthRefreshToken = p.AuthorizationType == BigQueryAuthorizationType.OAuth ? p.OAuthRefreshToken : string.Empty;
		}
		#endregion
		#region Overrides of DbStrategyBase
		public override bool CanGetDatabases { get { return true; } }
		public override IEnumerable<string> GetDatabases(IConnectionParametersControl control) {
			switch(control.AuthTypeBigQuery) {
				case BigQueryAuthorizationType.OAuth:
					return ProviderFactory.GetDatabases(control.ProjectID, control.OAuthClientID, control.OAuthClientSecret, control.OAuthRefreshToken);
				case BigQueryAuthorizationType.PrivateKeyFile:
					return ProviderFactory.GetDatabases(control.ProjectID, control.KeyFileName, control.ServiceEmail);
				default:
					return new string[0];
			}
		}
		#endregion
	}
}
