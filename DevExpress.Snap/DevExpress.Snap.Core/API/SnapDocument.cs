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
using System.ComponentModel;
using System.IO;
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.Snap.Core.Options;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
namespace DevExpress.Snap.Core.API {
	public interface ISnapFieldOwner {
		SnapEntity ActiveEntity { get; }
		SnapList FindListByName(string name);
		SnapList CreateSnList(DocumentPosition position, string name);
		void RemoveSnList(string name);
		SnapEntity ParseField(Field field);
		SnapEntity ParseField(int index);
		void RemoveField(int index);
		SnapText CreateSnText(DocumentPosition position, string dataFieldName);
		SnapImage CreateSnImage(DocumentPosition position, string dataFieldName);
		SnapCheckBox CreateSnCheckBox(DocumentPosition position, string dataFieldName);
		SnapBarCode CreateSnBarCode(DocumentPosition position);
		SnapSparkline CreateSnSparkline(DocumentPosition position, string dataFieldName);
		SnapHyperlink CreateSnHyperlink(DocumentPosition position, string dataFieldName);
	}
	public interface SnapDocument : Document, SnapSubDocument, IDataSourceOwner {
		DataSourceInfoCollection DataSources { get; }
		IDataSourceOwner GetDataSourceOwner(object dataSource);
		ParameterCollection Parameters { get; }
		ThemeCollection Themes { get; }
		string ActiveThemeName { get; }
		event EventHandler DataSourceChanged;
		void BeginUpdateDataSource();
		void EndUpdateDataSource();
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method overload has become obsolete. Use the Save method with the \"fileName\" parameter to save your documents in the native Snap format (.snx), and the Export method - to store your documents in other formats.")] 
		new void SaveDocument(string fileName, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method overload has become obsolete. Use the Save method with the \"fileName\" parameter to save your documents in the native Snap format (.snx), and the Export method - to store your documents in other formats.")] 
		new void SaveDocument(Stream stream, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new MailMergeOptions CreateMailMergeOptions();
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(string fileName, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(Stream stream, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(Document targetDocument);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(MailMergeOptions options, string fileName, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(MailMergeOptions options, Stream stream, DocumentFormat format);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method is not appropriate in Snap and has been rendered obsolete.")]
		new void MailMerge(MailMergeOptions options, Document targetDocument);
		SnapMailMergeExportOptions CreateSnapMailMergeExportOptions();
		void SnapMailMerge(string fileName, DocumentFormat format);
		void SnapMailMerge(Stream stream, DocumentFormat format);
		void SnapMailMerge(SnapDocument document);
		void SnapMailMerge(SnapMailMergeExportOptions options, string fileName, DocumentFormat format);
		void SnapMailMerge(SnapMailMergeExportOptions options, Stream stream, DocumentFormat format);
		void SnapMailMerge(SnapMailMergeExportOptions options, SnapDocument targetDocument);
		void SaveDocument(string fileName);
		void SaveDocument(Stream stream);
		void ExportDocument(string fileName, DocumentFormat format);
		void ExportDocument(Stream stream, DocumentFormat format);
		void SaveCurrentTheme(Stream stream);
		void SaveCurrentTheme(string fileName);
		void SaveCurrentTheme(Stream stream, string newName);
		Theme LoadTheme(Stream stream);
		Theme LoadTheme(string fileName);
		void ApplyTheme(String themeName);
		event ConfigureDataConnectionEventHandler ConfigureDataConnection;
		event CustomFilterExpressionEventHandler CustomFilterExpression;
		event ConnectionErrorEventHandler ConnectionError;
		TableCellStyleCollection TableCellStyles { get; }
		new SnapSectionCollection Sections { get; }
		new SnapSection InsertSection(DocumentPosition pos);
		new SnapSection AppendSection();
		new SnapSection GetSection(DocumentPosition pos);
		event BeforeInsertSnListEventHandler BeforeInsertSnList;
		event PrepareSnListEventHandler PrepareSnList;
		event AfterInsertSnListEventHandler AfterInsertSnList;
		event BeforeInsertSnListColumnsEventHandler BeforeInsertSnListColumns;
		event PrepareSnListColumnsEventHandler PrepareSnListColumns;
		event AfterInsertSnListColumnsEventHandler AfterInsertSnListColumns;
		event BeforeInsertSnListDetailEventHandler BeforeInsertSnListDetail;
		event PrepareSnListDetailEventHandler PrepareSnListDetail;
		event AfterInsertSnListDetailEventHandler AfterInsertSnListDetail;
		event BeforeInsertSnListRecordDataEventHandler BeforeInsertSnListRecordData;
		event AfterInsertSnListRecordDataEventHandler AfterInsertSnListRecordData;
	}
	public delegate void BeforeInsertSnListEventHandler(object sender, BeforeInsertSnListEventArgs e);
	public delegate void PrepareSnListEventHandler(object sender, PrepareSnListEventArgs e);
	public delegate void AfterInsertSnListEventHandler(object sender, AfterInsertSnListEventArgs e);
	public delegate void BeforeInsertSnListColumnsEventHandler(object sender, BeforeInsertSnListColumnsEventArgs e);
	public delegate void PrepareSnListColumnsEventHandler(object sender, PrepareSnListColumnsEventArgs e);
	public delegate void AfterInsertSnListColumnsEventHandler(object sender, AfterInsertSnListColumnsEventArgs e);
	public delegate void BeforeInsertSnListDetailEventHandler(object sender, BeforeInsertSnListDetailEventArgs e);
	public delegate void PrepareSnListDetailEventHandler(object sender, PrepareSnListDetailEventArgs e);
	public delegate void AfterInsertSnListDetailEventHandler(object sender, AfterInsertSnListDetailEventArgs e);
	public delegate void BeforeInsertSnListRecordDataEventHandler(object sender, BeforeInsertSnListRecordDataEventArgs e);
	public delegate void AfterInsertSnListRecordDataEventHandler(object sender, AfterInsertSnListRecordDataEventArgs e);
	public class SnapDocumentPosition : NativeDocumentPosition {
		internal SnapDocumentPosition(NativeSubDocument document, ModelPosition pos) : base(document, pos) { }
		public new SnapSubDocument BeginUpdateDocument() {
			return (SnapSubDocument)base.BeginUpdateDocument();
		}
	}
	public class SnapDocumentRange : NativeDocumentRange {
		internal SnapDocumentRange(NativeSubDocument document, ModelPosition start, ModelPosition end)
			: this(new SnapDocumentPosition(document, start.Clone()), new SnapDocumentPosition(document, end.Clone())) { }
		internal SnapDocumentRange(SnapDocumentPosition start, SnapDocumentPosition end) : base(start, end) { }
		public new SnapDocumentPosition Start { get { return (SnapDocumentPosition)base.Start; } }
		public new SnapDocumentPosition End { get { return (SnapDocumentPosition)base.End; } }
		public new SnapSubDocument BeginUpdateDocument { get { return (SnapSubDocument)base.BeginUpdateDocument(); } }
	}
	public abstract class SnListBasedEventArgs : EventArgs {
		SnapSubDocument subDocument;
		Field targetField;
		SnapList targetList;
		protected SnListBasedEventArgs(SnapSubDocument subDocument, Field targetField) {
			this.subDocument = subDocument;
			this.targetField = targetField;
		}
		protected SnapList GetTargetList() {
			if (targetList == null)
				targetList = subDocument.ParseField(targetField) as SnapList;
			return targetList;
		}
	}
	public class BeforeInsertSnListEventArgs : EventArgs {
		SnapDocumentPosition position;
		List<DataFieldInfo> dataFields;
		internal BeforeInsertSnListEventArgs(SnapDocumentPosition position, List<DataFieldInfo> dataFields) {
			this.position = position;
			this.dataFields = dataFields;
		}
		public SnapDocumentPosition Position { get { return position; } set { position = value; } }
		public List<DataFieldInfo> DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class DataFieldInfo {
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataFieldInfoDataSource")]
#endif
		public object DataSource { get; set; }
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataFieldInfoDataPaths")]
#endif
		public string[] DataPaths { get; set; }
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataFieldInfoDisplayName")]
#endif
		public string DisplayName { get; set; }
		public DataFieldInfo(object dataSource, string[] dataPaths, string displayName) {
			DataSource = dataSource;
			DataPaths = dataPaths;
			DisplayName = displayName;
		}
		public DataFieldInfo(object dataSource, string[] dataPaths) : this(dataSource, dataPaths, String.Empty) { }
		public DataFieldInfo(object dataSource, string dataMember) : this(dataSource, dataMember, dataMember) { }
		public DataFieldInfo(object dataSource, string dataMember, string displayName) : this(dataSource, new[] { dataMember }, displayName) { }
		internal DataFieldInfo(Native.Data.SNDataInfo dataInfo) {
			DataSource = dataInfo.Source;
			DataPaths = (string[])dataInfo.DataPaths.Clone();
			DisplayName = dataInfo.DisplayName;
		}
		internal Native.Data.SNDataInfo ToSNDataInfo() {
			return new Native.Data.SNDataInfo(DataSource, DataPaths, DisplayName);
		}
	}
	public class PrepareSnListEventArgs : EventArgs {
		SnapDocument template;
		DevExpress.Snap.Core.Native.SnapPieceTable pieceTable;
		InnerRichEditDocumentServer server;
		internal PrepareSnListEventArgs(DevExpress.Snap.Core.Native.SnapPieceTable pieceTable, InnerRichEditDocumentServer server) {
			this.pieceTable = pieceTable;
			this.server = server;
		}
		public SnapDocument Template {
			get {
				if (template == null)
					template = new DevExpress.Snap.API.Native.SnapNativeDocument(pieceTable, server);
				return template;
			}
		}
	}
	public class AfterInsertSnListEventArgs : EventArgs {
		SnapDocumentRange range;
		internal AfterInsertSnListEventArgs(SnapDocumentRange range) {
			this.range = range;
		}
		public SnapDocumentRange Range { get { return range; } }
	}
	public class BeforeInsertSnListColumnsEventArgs : SnListBasedEventArgs {
		int targetColumnIndex;
		List<DataFieldInfo> dataFields;
		internal BeforeInsertSnListColumnsEventArgs(SnapSubDocument subDocument, Field targetField, int targetColumnIndex, List<DataFieldInfo> dataFields)
			: base(subDocument, targetField) {
			this.targetColumnIndex = targetColumnIndex;
			this.dataFields = dataFields;
		}
		public SnapList Target { get { return GetTargetList(); } }
		public int TargetColumnIndex { get { return targetColumnIndex; } set { targetColumnIndex = value; } }
		public List<DataFieldInfo> DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class PrepareSnListColumnsEventArgs : EventArgs {
		Core.Native.PrepareSnListColumnsEventArgs innerE;
		InnerRichEditDocumentServer server;
		SnapDocument header;
		SnapDocument body;
		internal PrepareSnListColumnsEventArgs(Core.Native.PrepareSnListColumnsEventArgs e, InnerRichEditDocumentServer server) {
			this.innerE = e;
			this.server = server;
		}
		public SnapDocument Header {
			get {
				if (header == null) {
					if (innerE.Header == null)
						return null;
					header = new SnapNativeDocument((SnapPieceTable)innerE.Header.MainPieceTable, server);
				}
				return header;
			}
		}
		public SnapDocument Body {
			get {
				if (body == null) {
					if (innerE.Body == null)
						return null;
					body = new SnapNativeDocument((SnapPieceTable)innerE.Body.MainPieceTable, server);
				}
				return body;
			}
		}
	}
	public class AfterInsertSnListColumnsEventArgs : SnListBasedEventArgs {
		internal AfterInsertSnListColumnsEventArgs(SnapSubDocument document, Field field)
			: base(document, field) { }
		public SnapList SnList { get { return GetTargetList(); } }
	}
	public class BeforeInsertSnListDetailEventArgs : SnListBasedEventArgs {
		List<DataFieldInfo> dataFields;
		internal BeforeInsertSnListDetailEventArgs(SnapSubDocument subDocument, Field field, List<DataFieldInfo> dataFields)
			: base(subDocument, field) {
			this.dataFields = dataFields;
		}
		public SnapList Master { get { return GetTargetList(); } }
		public List<DataFieldInfo> DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class PrepareSnListDetailEventArgs : EventArgs {
		SnapDocument template;
		SnapPieceTable pieceTable;
		InnerRichEditDocumentServer server;
		internal PrepareSnListDetailEventArgs(SnapPieceTable pieceTable, InnerRichEditDocumentServer server) {
			this.pieceTable = pieceTable;
			this.server = server;
		}
		public SnapDocument Template {
			get {
				if (template == null)
					template = new SnapNativeDocument(pieceTable, server);
				return template;
			}
		}
	}
	public class AfterInsertSnListDetailEventArgs : SnListBasedEventArgs {
		internal AfterInsertSnListDetailEventArgs(SnapSubDocument subDocument, Field field)
			: base(subDocument, field) {
		}
		public SnapList Master { get { return GetTargetList(); } }
	}
	public class BeforeInsertSnListRecordDataEventArgs : SnListBasedEventArgs {
		int targetColumnIndex;
		List<DataFieldInfo> dataFields;
		internal BeforeInsertSnListRecordDataEventArgs(SnapSubDocument subDocument, Field field, int targetColumnIndex, List<DataFieldInfo> dataFields)
			: base(subDocument, field) {
			this.targetColumnIndex = targetColumnIndex;
			this.dataFields = dataFields;
		}
		public SnapList Target { get { return GetTargetList(); } }
		public int TargetColumnIndex { get { return targetColumnIndex; } }
		public List<DataFieldInfo> DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class AfterInsertSnListRecordDataEventArgs : EventArgs {
		SnapDocumentRange range;
		internal AfterInsertSnListRecordDataEventArgs(SnapDocumentRange range) {
			this.range = range;
		}
		public SnapDocumentRange Range { get { return range; } }
	}
}
