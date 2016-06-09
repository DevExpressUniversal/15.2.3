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
	public enum AsaConnectionType {
		ServerBased,
		File
	}
	public class AsaConnectionParameters : DataConnectionParametersBase, IConnectionPageHostname {
		AsaConnectionType connectionType = AsaConnectionType.ServerBased;
		public AsaConnectionType ConnectionType { get { return this.connectionType; } set { this.connectionType = value; } }
		public string FileName { get { return ((IConnectionPage)this).FileName; } set { ((IConnectionPage)this).FileName = value; } }
		public string UserName { get { return ((IConnectionPage)this).UserName; } set { ((IConnectionPage)this).UserName = value; } }
		public string ServerName { get { return ((IConnectionPage)this).ServerName; } set { ((IConnectionPage)this).ServerName = value; } }
		public string Password { get { return ((IConnectionPage)this).Password; } set { ((IConnectionPage)this).Password = value; } }
		public string DatabaseName { get { return ((IConnectionPage)this).DatabaseName; } set { ((IConnectionPage)this).DatabaseName = value; } }
		public string Hostname { get; set; }
		protected override bool IsServerBasedInternal { get { return this.connectionType == AsaConnectionType.ServerBased; } }
		public AsaConnectionParameters() {
		}
		public AsaConnectionParameters(string serverName, string hostname, string databaseName, string userName, string password) 
			: this(serverName, databaseName, userName, password) {
			Hostname = hostname;
		}
		public AsaConnectionParameters(string serverName, string databaseName, string userName, string password) {
			FileName = string.Empty;
			ServerName = serverName;
			DatabaseName = databaseName;
			UserName = userName;
			Password = password;
		}
		public AsaConnectionParameters(string fileName, string userName, string password) {
			FileName = fileName;
			ServerName = string.Empty;
			DatabaseName = string.Empty;
			UserName = userName;
			Password = password;
			ConnectionType = AsaConnectionType.File;
		}
		bool ShouldSerializePassword() {
			return !string.IsNullOrEmpty(Password);
		}
		bool ShouldSerializeUserName() {
			return !string.IsNullOrEmpty(UserName);
		}
		bool ShouldSerializeHostname() {
			return !string.IsNullOrEmpty(Hostname);
		}
		internal override void Assign(IConnectionPage source) {
			UserName = source.UserName;
			FileName = source.FileName;
			ServerName = source.ServerName;
			Password = source.Password;
			DatabaseName = source.DatabaseName;
			AsaConnectionParameters asaParameters = source as AsaConnectionParameters;
			if(asaParameters != null)
				Hostname = asaParameters.Hostname;
			this.connectionType = source.IsServerbased ? AsaConnectionType.ServerBased : AsaConnectionType.File;
		}
	}
}
