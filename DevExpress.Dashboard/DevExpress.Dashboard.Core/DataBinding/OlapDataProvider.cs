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

using System;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils.Controls;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	[Obsolete("The OlapDataProvider class is obsolete now. Use the DashboardOlapDataSource classe instead.")]
	public class OlapDataProvider : IDisposable, IDataProvider {
		public const string XmlNameString = "OlapDataProvider";
		public const string XmlDataConnection = "DataConnection";
		OlapDataConnection connection;
		PivotCustomizationFieldsTreeBase dataSchema;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		internal string CubeName { get { return connection != null ? connection.OlapDataSource.CubeName : null; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		internal string CubeCaption { get { return connection != null ? connection.OlapDataSource.CubeCaption : null; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public OlapDataConnection DataConnection { get { return connection; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("OlapDataProviderOlapConnectionString"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string OlapConnectionString {
			get { return connection != null ? connection.ConnectionString : null; }
			internal set {
				if(connection == null || connection.ConnectionString == value)
					return;
				connection.SetConnectionString((string)value);
				dataSchema = null;
			}
		}
		bool IDataProvider.IsDisposed { get { return connection == null; } }
		bool IDataProvider.IsConnected { get { return connection != null && connection.Connected; } }
		object IDataProvider.Data { get { return null; } }
		DataConnectionBase IDataProvider.Connection { get { return DataConnection; } set { SetDataConnection((OlapDataConnection)value); } }
		object IDataProvider.DataSchema { get { return dataSchema; } }
		internal PivotGridAdomdDataSource PivotDataSource { get { return connection != null ? connection.DataSource : null; } }
		[Browsable(false)]
		public bool IsDisposed { get; private set; }
		[Browsable(false)]
		public event EventHandler Disposed;
		event EventHandler<DataSchemaChangedEventArgs> IDataProvider.DataSchemaChanged { add { } remove { } }
		public OlapDataProvider(string connectionName, OlapConnectionParameters connectionParameters)
			: this(new OlapDataConnection(connectionName, connectionParameters)) {
		}
		public OlapDataProvider(OlapDataConnection dataConnection) {
			SetDataConnection(dataConnection);
		}
		public OlapDataProvider() {
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			IsDisposed = true;
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		protected virtual PivotGridAdomdDataSource CreatePivotDataSource() {
			return new PivotGridAdomdDataSource();
		}
		void SetDataConnection(OlapDataConnection value) {
			if(value != null && value != connection) {
				connection = value;
			}
		}
   		internal void LoadDataSchema() {
			if(connection == null || !connection.Connected)
				return;
			IPivotOLAPDataSource ds = connection.OlapDataSource;
			if(ds != null)
				ds.ReloadData();
			PivotGridData pivotData = new PivotGridData() { PivotDataSource = connection.OlapDataSource };
			pivotData.RetrieveFields(PivotArea.FilterArea, false);
			dataSchema = new PivotCustomizationFieldsTreeBase(new CustomizationFormFields(pivotData), new PivotCustomizationTreeNodeFactoryBase(pivotData));
			dataSchema.Update(true);
		}
   		void IDataProvider.EndLoading(IDBSchemaProvider dbSchemaProvider) {
			((IDataProvider)this).EndLoading(dbSchemaProvider, null);			
		}
   		void IDataProvider.Dispose() {
			if(connection != null) {				
				connection.DataSource.Dispose();
			}
		}
		void IDataProvider.SaveToXml(XElement element) {
			element.Name = XmlNameString;
			if(connection != null)
				element.Add(new XAttribute(XmlDataConnection, connection.Name));
		}
		void IDataProvider.LoadFromXml(XElement dataProviderElement) {
		}
		bool IDataProvider.CancelExecute() {
			return false;
		}
		void IDataProvider.EndLoading(IDBSchemaProvider dbSchemaProvider, System.Collections.Generic.List<DataLoaderError> errors) {
			dataSchema = null;
			LoadDataSchema();
		}
	}
}
