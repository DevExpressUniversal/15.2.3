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
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap {
	#region ReportStructureEditorFormShowingEventHandler
	public delegate void ReportStructureEditorFormShowingEventHandler(object sender, ReportStructureEditorFormShowingEventArgs e);
	#endregion
	#region ReportStructureEditorFormShowingEventArgs
	public class ReportStructureEditorFormShowingEventArgs : FormShowingEventArgs {
		readonly ReportStructureEditorFormControllerParameters controllerParameters;
		public ReportStructureEditorFormShowingEventArgs(ReportStructureEditorFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public ReportStructureEditorFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region TableCellStyleFormShowingEventHandler
	public delegate void TableCellStyleFormShowingEventHandler(object sender, TableCellStyleFormShowingEventArgs e);
	#endregion
	#region TableCellStyleFormShowingEventArgs
	public class TableCellStyleFormShowingEventArgs : FormShowingEventArgs {
		readonly TableCellStyleFormControllerParameters controllerParameters;
		public TableCellStyleFormShowingEventArgs(TableCellStyleFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public TableCellStyleFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region MailMergeExportFormShowing
	public delegate void MailMergeExportFormShowingEventHandler(object sender, MailMergeExportFormShowingEventArgs e);
	public class MailMergeExportFormShowingEventArgs : FormShowingEventArgs {
		readonly MailMergeExportFormControllerParameters controllerParameters;
		public MailMergeExportFormShowingEventArgs(MailMergeExportFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			this.controllerParameters = controllerParameters;
		}
		public SnapMailMergeExportOptions Options { get { return controllerParameters.Properties; } }
	}
	#endregion
	#region BeforeDataSourceExport
	public delegate void BeforeDataSourceExportEventHandler(object sender, BeforeDataSourceExportEventArgs e);
	public class BeforeDataSourceExportEventArgs : EventArgs {
		readonly string dataSourceName;
		readonly object dataSource;
		public BeforeDataSourceExportEventArgs(object dataSource, string dataSourceName) {
			this.dataSource = dataSource;
			this.dataSourceName = dataSourceName;
		}
		public string DataSourceName { get { return dataSourceName; } }
		public object DataSource { get { return dataSource; } }
		public byte[] Data { get; set; }
	}
	#endregion
	#region AfterDataSourceImport
	public delegate void AfterDataSourceImportEventHandler(object sender, AfterDataSourceImportEventArgs e);
	public class AfterDataSourceImportEventArgs : EventArgs {
		readonly byte[] data;
		readonly string dataSourceName;
		public AfterDataSourceImportEventArgs(string dataSourceName, byte[] data) {
			this.data = data;
			this.dataSourceName = dataSourceName;
		}
		public string DataSourceName { get { return dataSourceName; } }
		public byte[] Data { get { return data; } }
	}
	#endregion
	#region DataSourceInfoChanged
	public enum DataSourceOwner {
		Control,
		Document
	}
	public class DataSourceInfoChangedEventArgs : EventArgs {
		readonly DataSourceOwner dataSourceOwner;
		public DataSourceInfoChangedEventArgs(DataSourceOwner dataSourceOwner) {
			this.dataSourceOwner = dataSourceOwner;
		}
		public DataSourceOwner DataSourceOwner { get { return dataSourceOwner; } }
	}
	public delegate void DataSourceInfoChangedEventHandler(object sender, DataSourceInfoChangedEventArgs e);
	#endregion
	#region DocumentClosingEventArgs
	public class DocumentClosingEventArgs : CancelEventArgs {
		DocumentDataSources interimDataSources;
		public DocumentClosingEventArgs(DocumentDataSources interimDataSources) {
			Guard.ArgumentNotNull(interimDataSources, "interimDataSources");
			this.interimDataSources = interimDataSources;
		}
		public DocumentDataSources InterimDataSources { get { return interimDataSources; } }
		public bool Handled { get; set; }
	}
	public delegate void DocumentClosingEventHandler(object sender, DocumentClosingEventArgs e);
	#endregion
	#region DocumentImportedEventArgs
	public class DocumentImportedEventArgs : EventArgs {
		DocumentDataSources interimDataSources;
		public DocumentImportedEventArgs(DocumentDataSources interimDataSources) {
			this.interimDataSources = interimDataSources;
		}
		public DocumentDataSources InterimDataSources { get { return interimDataSources; } }
		public bool Handled { get; set; }
	}
	public delegate void DocumentImportedEventHandler(object sender, DocumentImportedEventArgs e);
	#endregion
	#region DocumentDataSources
	public class DocumentDataSources {
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DocumentDataSourcesDataSources")]
#endif
		public DataSourceInfoCollection DataSources { get; set; }
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DocumentDataSourcesDefaultDataSourceInfo")]
#endif
		public DataSourceInfo DefaultDataSourceInfo {
			get { return DataSources.DefaultDataSourceInfo; }
			set { DataSources.SetDefaultDataSource(value); }
		}
		protected internal DocumentDataSources() {
			this.DataSources = new DataSourceInfoCollection();
		}
		protected internal DocumentDataSources(SnapDocumentModel documentModel) {
			this.DataSources = documentModel.DataSources;
		}
	}
	#endregion
	#region SnapMailMergeStartedEventHandler
	[ComVisible(true)]
	public delegate void SnapMailMergeStartedEventHandler(object sender, SnapMailMergeStartedEventArgs e);
	#endregion
	#region SnapMailMergeStartedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SnapMailMergeStartedEventArgs : CancelEventArgs {
		readonly SnapDocumentModel targetDocumentModel;
		InternalSnapDocumentServer targetServer;
		internal SnapMailMergeStartedEventArgs(SnapDocumentModel targetDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
		}
		string operationDescription = string.Empty;
		public string OperationDescription { get { return operationDescription; } set { operationDescription = value; } }
		public SnapDocument Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		internal static InternalSnapDocumentServer CreateDocumentServerForExistingDocumentModel(SnapDocumentModel documentModel) {
			return new InternalSnapDocumentServer(documentModel);
		}
		internal void Clear() {
			targetServer.Dispose();
			targetServer = null;
		}
	}
	#endregion
	#region SnapMailMergeRecordStartedEventHandler
	[ComVisible(true)]
	public delegate void SnapMailMergeRecordStartedEventHandler(object sender, SnapMailMergeRecordStartedEventArgs e);
	#endregion
	#region SnapMailMergeRecordStartedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SnapMailMergeRecordStartedEventArgs : CancelEventArgs {
		readonly SnapDocumentModel targetDocumentModel;
		readonly SnapDocumentModel recordDocumentModel;
		InternalSnapDocumentServer targetServer;
		InternalSnapDocumentServer recordServer;
		int recordIndex;
		internal SnapMailMergeRecordStartedEventArgs(SnapDocumentModel targetDocumentModel, SnapDocumentModel recordDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
			this.recordDocumentModel = recordDocumentModel;
		}
		public int RecordIndex { get { return recordIndex; } protected internal set { recordIndex = value; } }
		public SnapDocument Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = SnapMailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		public SnapDocument RecordDocument {
			get {
				if (recordDocumentModel == null)
					return null;
				if (recordServer == null)
					recordServer = SnapMailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(recordDocumentModel);
				return recordServer.Document;
			}
		}
		internal void Clear() {
			targetServer.Dispose();
			recordServer.Dispose();
			targetServer = null;
			recordServer = null;
		}
	}
	#endregion
	#region SnapMailMergeRecordFinishedEventHandler
	[ComVisible(true)]
	public delegate void SnapMailMergeRecordFinishedEventHandler(object sender, SnapMailMergeRecordFinishedEventArgs e);
	#endregion
	#region SnapMailMergeRecordFinishedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SnapMailMergeRecordFinishedEventArgs : CancelEventArgs {
		readonly SnapDocumentModel targetDocumentModel;
		readonly SnapDocumentModel recordDocumentModel;
		InternalSnapDocumentServer targetServer;
		InternalSnapDocumentServer recordServer;
		int recordIndex;
		internal SnapMailMergeRecordFinishedEventArgs(SnapDocumentModel targetDocumentModel, SnapDocumentModel recordDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
			this.recordDocumentModel = recordDocumentModel;
		}
		public int RecordIndex { get { return recordIndex; } protected internal set { recordIndex = value; } }
		public SnapDocument Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = SnapMailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		public SnapDocument RecordDocument {
			get {
				if (recordDocumentModel == null)
					return null;
				if (recordServer == null)
					recordServer = SnapMailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(recordDocumentModel);
				return recordServer.Document;
			}
		}
		internal void Clear() {
			targetServer.Dispose();
			recordServer.Dispose();
			targetServer = null;
			recordServer = null;
		}
	}
	#endregion
	#region SnapMailMergeFinishedEventHandler
	[ComVisible(true)]
	public delegate void SnapMailMergeFinishedEventHandler(object sender, SnapMailMergeFinishedEventArgs e);
	#endregion
	#region SnapMailMergeFinishedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SnapMailMergeFinishedEventArgs : EventArgs {
		readonly SnapDocumentModel targetDocumentModel;
		InternalSnapDocumentServer targetServer;
		internal SnapMailMergeFinishedEventArgs(SnapDocumentModel targetDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
		}
		public SnapDocument Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = SnapMailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		internal void Clear() {
			targetServer.Dispose();
			targetServer = null;
		}
	}
	#endregion
	#region BeforeConversionEventHandler
	[ComVisible(true)]
	public delegate void BeforeConversionEventHandler(object sender, BeforeConversionEventArgs e);
	#endregion
	#region BeforeConversionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class BeforeConversionEventArgs : EventArgs {
		SnapDocument document;
		public BeforeConversionEventArgs(SnapDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		public SnapDocument Document { get { return document; } }
	}
	#endregion
}
