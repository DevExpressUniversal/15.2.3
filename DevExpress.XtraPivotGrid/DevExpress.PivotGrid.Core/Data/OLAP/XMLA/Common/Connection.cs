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
using DevExpress.PivotGrid.OLAP;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.Xmla {
	class XmlaConnection : IOLAPConnection, IOLAPEntity {
		IOLAPMetadata data;
		IOLAPConnectionSettings connectionSettings;
		string serverVersion;
		string sessionId;
		string database;
		internal XmlaConnection(IOLAPMetadata data, IOLAPConnectionSettings connectionSettings) {
			this.connectionSettings = connectionSettings;
			this.data = data;
			this.serverVersion = string.Empty;
			this.sessionId = null;
		}
		public string CatalogName {
			get { return this.connectionSettings.CatalogName; }
		}
		public string CubeName {
			get { return this.connectionSettings.CubeName; }
		}
		public int ConnectionTimeout {
			get { return this.connectionSettings.ConnectionTimeout; }
		}
		public int LocaleIdentifier {
			get { return this.connectionSettings.LocaleIdentifier; }
		}
		public string Password {
			get { return this.connectionSettings.Password; }
		}
		public string ServerVersion {
			get { return this.serverVersion; }
			internal set { this.serverVersion = value; }
		}
		public string UserId {
			get { return this.connectionSettings.UserId; }
		}
		public string Roles {
			get { return this.connectionSettings.Roles; }
		}
		public string CustomData {
			get { return this.connectionSettings.CustomData; }
		}
		public string Database {
			get { return this.database; }
			internal set { this.database = value; }
		}
		public string ServerName {
			get { return this.connectionSettings.ServerName; }
		}
		public string SessionId {
			get { return this.sessionId; } 
			set { this.sessionId = value; } 
		}
		public void Open() {
#if !SL
			XmlaSchemaLoader schemaLoader = XmlaSchemaLoader.Create(this);
			schemaLoader.BeginSession();
#else
			this.database = "fake";
#endif
		}
		public void Close() {
#if !SL
			if(string.IsNullOrEmpty(this.sessionId)) return;
			XmlaSchemaLoader schemaLoader = XmlaSchemaLoader.Create(this);
			schemaLoader.EndSession();
			this.sessionId = null;
#endif
		}
		public IOLAPMetadata Data {
			get { return data; }
			internal set { data = value; }
		}
		public bool IsAsync { 
			get {
				if(data == null) return true;
				return data.IsLocked;
			} 
		}
		public IOLAPCommand CreateCommand(string mdx) {
			return new XmlaCommand(this, mdx);
		}
		#region IOLAPConnection Members
		IOLAPCommand IOLAPConnection.CreateCommand(string mdx) {
			return this.CreateCommand(mdx);
		}
		public void Close(bool endSession) {
			if(endSession) Close();
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			Close(false);
		}
		#endregion
	}
}
