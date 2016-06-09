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

using DevExpress.DataAccess.Native.Sql;
using DevExpress.Xpo.Helpers;
namespace DevExpress.DataAccess.ConnectionParameters {
	public class BigQueryConnectionParameters : DataConnectionParametersBase, IConnectionPageBigQuery {
		public string ProjectID { get { return ((IConnectionPage)this).ServerName; } set { ((IConnectionPage)this).ServerName = value; } }
		public string DataSetID { get { return ((IConnectionPage)this).DatabaseName; } set { ((IConnectionPage)this).DatabaseName = value; } }
		public string PrivateKeyFileName { get; set; }
		public string ServiceAccountEmail { get; set; }
		public string OAuthClientID { get; set; }
		public string OAuthClientSecret { get; set; }
		public string OAuthRefreshToken { get; set; }
		public BigQueryAuthorizationType AuthorizationType {
			get { return ((IConnectionPage)this).AuthType ? BigQueryAuthorizationType.PrivateKeyFile : BigQueryAuthorizationType.OAuth; } 
			set { ((IConnectionPage)this).AuthType = value == BigQueryAuthorizationType.PrivateKeyFile; }
		}
		public BigQueryConnectionParameters(string projectId, string datasetId, string oAuthClientID, string oAuthClientSecret, string oAuthRefreshToken)
			: this(projectId, datasetId) {
			AuthorizationType = BigQueryAuthorizationType.OAuth;
			OAuthClientID = oAuthClientID;
			OAuthClientSecret = oAuthClientSecret;
			OAuthRefreshToken = oAuthRefreshToken;
		}
		public BigQueryConnectionParameters() {
		}
		public BigQueryConnectionParameters(string projectId, string datasetId, string serviceAccountEmail, string privateKeyFileName)
			: this(projectId, datasetId) {
			AuthorizationType = BigQueryAuthorizationType.PrivateKeyFile;
			ServiceAccountEmail = serviceAccountEmail;
			PrivateKeyFileName = privateKeyFileName;
		}
		BigQueryConnectionParameters(string projectId, string datasetId) {
			ProjectID = projectId;
			DataSetID = datasetId;
		}
		internal override void Assign(IConnectionPage source) {
			IConnectionPageBigQuery sourceBigQuery = source as IConnectionPageBigQuery;
			if(sourceBigQuery == null)
				return;
			BigQueryAuthorizationType authorizationType = sourceBigQuery.AuthorizationType;
			AuthorizationType = authorizationType;
			if(authorizationType == BigQueryAuthorizationType.OAuth) {
				OAuthClientID = sourceBigQuery.OAuthClientID;
				OAuthClientSecret = sourceBigQuery.OAuthClientSecret;
				OAuthRefreshToken = sourceBigQuery.OAuthRefreshToken;
			}
			if(authorizationType == BigQueryAuthorizationType.PrivateKeyFile) {
				PrivateKeyFileName = sourceBigQuery.PrivateKeyFileName;
				ServiceAccountEmail = sourceBigQuery.ServiceAccountEmail;
			}
		}
	}
	public enum BigQueryAuthorizationType {
		PrivateKeyFile,
		OAuth
	}
}
