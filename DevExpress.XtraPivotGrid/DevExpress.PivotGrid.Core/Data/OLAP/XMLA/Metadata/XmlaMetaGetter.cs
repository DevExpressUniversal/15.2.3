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
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.Xmla;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid {
	public class XmlaMetaGetter : IDisposable {
		internal class AsyncMetaGetterOperationResult : AsyncOperationResult {
			protected internal AsyncMetaGetterOperationResult(object value, Exception exception)
				: base(value, exception) {
			}
		}
		internal static AsyncMetaGetterOperationResult CreateAsyncOperationResult(IList<string> result, Exception error) {
			return new AsyncMetaGetterOperationResult(result, error);
		}
		XmlaConnection olapConnection;
		IOLAPConnectionSettings connectionSettings;
		string connectionString;
		public delegate void MetadataAccessedDelegate(List<string> result);
		public XmlaMetaGetter() {
		}
		public bool Connected {
			get { return olapConnection != null; }
			set {
				if(Connected == value) return;
				if(value) Connect();
				else Disconnect();
			}
		}
		public string ConnectionString {
			get { return connectionString; }
			set {
				if(connectionString != value || !Connected) {
					connectionSettings = null;
					connectionString = value;
					Connect();
				}
			}
		}
		public void GetCatalogsAsync(AsyncCompletedHandler completed) {
			if(!Connected) {
				completed(null);
				return;
			}
			XmlaSchemaLoader schemaLoader = XmlaSchemaLoader.Create(olapConnection);
			schemaLoader.GetCatalogsAsync(completed);
		}
		public void GetCubesAsync(string catalogName, AsyncCompletedHandler completed) {
			if(!Connected || string.IsNullOrEmpty(catalogName)) {
				completed(null);
				return;
			}
			ConnectionSettings.CatalogName = catalogName;
			XmlaSchemaLoader schemaLoader = XmlaSchemaLoader.Create(olapConnection);
			schemaLoader.GetCubesAsync(completed);
		}
		void Connect() {
			if(Connected) Disconnect();
			if(string.IsNullOrEmpty(ConnectionString)) return;
			olapConnection = new XmlaConnection(null, ConnectionSettings);
			try {
				olapConnection.Open();
				if(string.IsNullOrEmpty(olapConnection.Database))
					Disconnect();
			} catch(Exception) {
				Disconnect();
			}
		}
		void Disconnect() {
			if(!Connected) return;
			olapConnection.Close(false);
			olapConnection = null;
		}
		IOLAPConnectionSettings ConnectionSettings {
			get {
				if(string.IsNullOrEmpty(ConnectionString)) return null;
				if(connectionSettings == null)
					connectionSettings = new OLAPConnectionStringBuilder(ConnectionString);
				return connectionSettings;
			}
		}
		#region IDisposable Members
		bool disposed;
		public void Dispose() {
			if(!disposed) {
				disposed = true;
				Dispose(true);
				GC.SuppressFinalize(this);
			}
		}
		protected virtual void Dispose(bool disposing) {
			Disconnect();
		}
		#endregion
	}
}
