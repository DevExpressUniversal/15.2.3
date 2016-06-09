#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Xml;
using System.Xml.Linq;
using System.ComponentModel;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class OlapDataConnection : DataConnectionBase, IDataConnection {
		class DashboardPivotData : PivotGridData {
			bool handleException = true;
			public DashboardPivotData() { }
			protected override void DoRefreshCore() {
			}
			public override bool DelayFieldsGroupingByHierarchies { get { return true; } }
			protected override bool OnQueryException(IQueryDataSource dataSource, Exception ex) {
				OLAPConnectionException connExc = ex as OLAPConnectionException;
				return handleException && connExc != null;
			}
			public void TryConnect(IPivotOLAPDataSource ds) {
				PivotDataSource = ds;
				handleException = false;
				try {
					OLAPDataSource.Connect();
				} finally {
					handleException = true;
				}
			}
		}	
		IPivotOLAPDataSource data;
		DashboardPivotData ownerData;
		[
		Browsable(false)
		]
		public OlapConnectionParameters ConnectionParameters { get; private set; }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OlapDataConnectionConnectionString"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override string ConnectionString { 
			get { return data.FullConnectionString; } 
			set { try { data.FullConnectionString = value; } catch { } } 
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OlapDataConnectionConnectionStringSerializable"),
#endif
		Browsable(false)
		]
		public override string ConnectionStringSerializable {
			get {
				OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder(ConnectionString);
				builder.UserId = null;
				builder.Password = null;
				if(string.IsNullOrEmpty(builder.CubeName))
					return builder.ConnectionString + ";";
				else
					return string.Format("{0};Cube Name={1};", builder.ConnectionString, builder.CubeName);
			}
			set { base.ConnectionStringSerializable = value; }
		}
		internal bool Connected { get { return ((IPivotOLAPDataSource)data).Connected; } }
		internal PivotGridAdomdDataSource DataSource { get { return (PivotGridAdomdDataSource)data; } }
		protected internal IPivotOLAPDataSource OlapDataSource { get { return data; }  set { data = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OlapDataConnectionIsConnected")
#else
	Description("")
#endif
		]
		public override bool IsConnected { get { return Connected; } }
		public OlapDataConnection(string name, OlapConnectionParameters parameters)
			: this(name) {
			Initialize();
			ConnectionParameters = parameters;
			ConnectionString = parameters.ConnectionString;
		}
		public OlapDataConnection() {
			Initialize();
		}
		public override void Close() {
			if(this.data == null)
				return;
			this.data.Disconnect();
		}	
		protected internal OlapDataConnection(string name)
			: base(name) {
		}
		internal void SetConnectionString(string connectionString) {
			ConnectionString = connectionString;
			StoreConnectionNameOnly = false;
		}
		protected override void Dispose(bool disposing) {
			data.Disconnect();
			DataSource.Dispose();
			base.Dispose(disposing);
		}		
		protected override void CreateDataStoreCore() {
			if(ownerData == null)
				ownerData = new DashboardPivotData();
			ownerData.TryConnect(data);
		}
		public override DataConnectionParametersBase CreateDataConnectionParameters() {
			return new OlapConnectionParameters(ConnectionString);
		}
		void Initialize() {
			data = new PivotGridAdomdDataSource();
		}
		#region Implementation of IDataConnection
		string IDataConnection.CreateConnectionString(bool blackoutCredentials) {
			if(blackoutCredentials)
				throw new NotSupportedException();
			return CreateConnectionString();
		}
		void IDataConnection.BlackoutCredentials() { throw new NotSupportedException(); }
		#endregion
	}
}
