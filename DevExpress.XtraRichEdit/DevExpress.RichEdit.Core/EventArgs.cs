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
using DevExpress.Office.Export;
using DevExpress.Office.Import;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Import;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Windows.Forms;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.API.Layout;
#else
using DevExpress.Data;
using DevExpress.XtraRichEdit.Layout;
#endif
namespace DevExpress.XtraRichEdit {
	#region BeforeExportEventHandler
	[ComVisible(true)]
	public delegate void BeforeExportEventHandler(object sender, BeforeExportEventArgs e);
	#endregion
	#region BeforeExportEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class BeforeExportEventArgs : EventArgs {
		#region Fields
		readonly IExporterOptions options;
		readonly DocumentFormat documentFormat;
		#endregion
		public BeforeExportEventArgs(DocumentFormat documentFormat, IExporterOptions options) {
			if (documentFormat == DocumentFormat.Undefined)
				Exceptions.ThrowArgumentException("documentFormat", documentFormat);
			Guard.ArgumentNotNull(options, "options");
			this.documentFormat = documentFormat;
			this.options = options;
		}
		#region Properties
		public IExporterOptions Options { get { return options; } }
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		#endregion
	}
	#endregion
	#region BeforeImportEventHandler
	[ComVisible(true)]
	public delegate void BeforeImportEventHandler(object sender, BeforeImportEventArgs e);
	#endregion
	#region BeforeImportEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class BeforeImportEventArgs : EventArgs {
		#region Fields
		readonly IImporterOptions options;
		readonly DocumentFormat documentFormat;
		#endregion
		public BeforeImportEventArgs(DocumentFormat documentFormat, IImporterOptions options) {
			if (documentFormat == DocumentFormat.Undefined)
				Exceptions.ThrowArgumentException("documentFormat", documentFormat);
			Guard.ArgumentNotNull(options, "options");
			this.documentFormat = documentFormat;
			this.options = options;
		}
		#region Properties
		public IImporterOptions Options { get { return options; } }
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		#endregion
	}
	#endregion
	#region RichEditUnhandledExceptionEventHandler
	[ComVisible(true)]
	public delegate void RichEditUnhandledExceptionEventHandler(object sender, RichEditUnhandledExceptionEventArgs e);
	#endregion
	#region RichEditUnhandledExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class RichEditUnhandledExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		bool handled;
		#endregion
		public RichEditUnhandledExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	#region RichEditInvalidFormatExceptionEventHandler
	[ComVisible(true)]
	public delegate void RichEditInvalidFormatExceptionEventHandler(object sender, RichEditInvalidFormatExceptionEventArgs e);
	#endregion
	#region RichEditInvalidFormatExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class RichEditInvalidFormatExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		#endregion
		public RichEditInvalidFormatExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		#endregion
	}
	#endregion
	#region RichEditClipboardSetDataExceptionEventHandler
	[ComVisible(true)]
	public delegate void RichEditClipboardSetDataExceptionEventHandler(object sender, RichEditClipboardSetDataExceptionEventArgs e);
	#endregion
	#region RichEditClipboardSetDataExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class RichEditClipboardSetDataExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		#endregion
		public RichEditClipboardSetDataExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		#endregion
	}
	#endregion
	#region HyperlinkClickEventHandler
	[ComVisible(true)]
	public delegate void HyperlinkClickEventHandler(object sender, HyperlinkClickEventArgs e);
	#endregion
	#region HyperlinkClickEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class HyperlinkClickEventArgs : EventArgs {
		bool handled;
		readonly Hyperlink hyperlink;
		readonly Keys modifierKeys;
		public HyperlinkClickEventArgs(Hyperlink hyperlink, Keys modifierKeys) {
			Guard.ArgumentNotNull(hyperlink, "hyperlink");
			this.hyperlink = hyperlink;
			this.modifierKeys = modifierKeys;
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public Hyperlink Hyperlink { get { return hyperlink; } }
		public Keys ModifierKeys { get { return modifierKeys; } }
		public bool Control { get { return (modifierKeys & Keys.Control) == Keys.Control; } }
		public bool Alt { get { return (modifierKeys & Keys.Alt) == Keys.Alt; } }
		public bool Shift { get { return (modifierKeys & Keys.Shift) == Keys.Shift; } }
	}
	#endregion
	#region HeaderFooterEditingEventHandler
	[ComVisible(true)]
	public delegate void HeaderFooterEditingEventHandler(object sender, HeaderFooterEditingEventArgs e);
	#endregion
	#region HeaderFooterEditingEventArgs
	[ComVisible(true)]
	public class HeaderFooterEditingEventArgs : EventArgs {
	}
	#endregion
	#region AutoCorrectEventHandler
	[ComVisible(true)]
	public delegate void AutoCorrectEventHandler(object sender, AutoCorrectEventArgs e);
	#endregion
	#region AutoCorrectEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class AutoCorrectEventArgs : EventArgs {
		readonly InnerRichEditDocumentServer documentServer;
		AutoCorrectInfo autoCorrectInfo;
		protected internal AutoCorrectEventArgs(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
			this.autoCorrectInfo = CreateAutoCorrectInfo();
		}
		public AutoCorrectInfo AutoCorrectInfo { get { return autoCorrectInfo; } set { autoCorrectInfo = value; } }
		public AutoCorrectInfo CreateAutoCorrectInfo() {
			return new AutoCorrectInfo(documentServer);
		}
	}
	#endregion
	#region CustomizeMergeFieldsEventHandler
	[ComVisible(true)]
	public delegate void CustomizeMergeFieldsEventHandler(object sender, CustomizeMergeFieldsEventArgs e);
	#endregion
	#region CustomizeMergeFieldsEventArgs
	[ComVisible(true)]
	public class CustomizeMergeFieldsEventArgs : EventArgs {
		#region Fields
		MergeFieldName[] mergeFieldsNames;
		#endregion
		public CustomizeMergeFieldsEventArgs() {
		}
		public CustomizeMergeFieldsEventArgs(MergeFieldName[] mergeFieldsNames) {
			this.mergeFieldsNames = mergeFieldsNames;
		}
		#region Properties
		public MergeFieldName[] MergeFieldsNames { get { return mergeFieldsNames; } set { mergeFieldsNames = value; } }
		#endregion
	}
	#endregion
	#region CalculateDocumentVariableEventHandler
	[ComVisible(true)]
	public delegate void CalculateDocumentVariableEventHandler(object sender, CalculateDocumentVariableEventArgs e);
	#endregion
	#region CalculateDocumentVariableEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class CalculateDocumentVariableEventArgs : EventArgs {
		#region Fields
		readonly string variableName;
		readonly ArgumentCollection arguments;
		object result;
		bool handled;
		bool keepLastParagraph;		
		readonly DevExpress.XtraRichEdit.Model.Field field;
		readonly object unhandledValue;
		#endregion
		protected internal CalculateDocumentVariableEventArgs(string variableName, ArgumentCollection arguments, DevExpress.XtraRichEdit.Model.Field field, object unhandledValue) {
			Guard.ArgumentNotNull(arguments, "arguments");
			this.variableName = variableName;
			this.arguments = arguments;
			this.field = field;
			this.FieldLocked = field != null ? field.Locked : false;
			this.unhandledValue = unhandledValue;
		}
		#region Properties
		public string VariableName { get { return variableName; } }
		public ArgumentCollection Arguments { get { return arguments; } }
		public object Value { get { return result; } set { result = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		public bool KeepLastParagraph { get { return keepLastParagraph; } set { keepLastParagraph = value; } }
		public bool FieldLocked { get; set;}
		protected internal DevExpress.XtraRichEdit.Model.Field Field { get { return field; } }
		protected internal object UnhandledValue { get { return unhandledValue; } }
		#endregion
	}
	#endregion
	#region MailMergeStartedEventHandler
	[ComVisible(true)]
	public delegate void MailMergeStartedEventHandler(object sender, MailMergeStartedEventArgs e);
	#endregion
	#region MailMergeStartedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class MailMergeStartedEventArgs : CancelEventArgs {
		readonly DocumentModel targetDocumentModel;
		InternalRichEditDocumentServer targetServer;
		internal MailMergeStartedEventArgs(DocumentModel targetDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
		}
		string operationDescription = String.Empty;
		public string OperationDescription { get { return operationDescription; } set { operationDescription = value; } }
		public Document Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		internal static InternalRichEditDocumentServer CreateDocumentServerForExistingDocumentModel(DocumentModel documentModel) {
			return new InternalRichEditDocumentServer(documentModel);
		}
		internal void Clear() {
			targetServer.Dispose();
			targetServer = null;
		}
	}
	#endregion
	#region MailMergeRecordStartedEventHandler
	[ComVisible(true)]
	public delegate void MailMergeRecordStartedEventHandler(object sender, MailMergeRecordStartedEventArgs e);
	#endregion
	#region MailMergeRecordStartedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class MailMergeRecordStartedEventArgs : CancelEventArgs {
		readonly DocumentModel targetDocumentModel;
		readonly DocumentModel recordDocumentModel;
		InternalRichEditDocumentServer targetServer;
		InternalRichEditDocumentServer recordServer;
		int recordIndex;
		internal MailMergeRecordStartedEventArgs(DocumentModel targetDocumentModel, DocumentModel recordDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
			this.recordDocumentModel = recordDocumentModel;
		}
		public int RecordIndex { get { return recordIndex; } protected internal set { recordIndex = value; } }
		public Document Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = MailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		public Document RecordDocument {
			get {
				if (recordDocumentModel == null)
					return null;
				if (recordServer == null)
					recordServer = MailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(recordDocumentModel);
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
	#region MailMergeRecordFinishedEventHandler
	[ComVisible(true)]
	public delegate void MailMergeRecordFinishedEventHandler(object sender, MailMergeRecordFinishedEventArgs e);
	#endregion
	#region MailMergeRecordFinishedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class MailMergeRecordFinishedEventArgs : CancelEventArgs {
		readonly DocumentModel targetDocumentModel;
		readonly DocumentModel recordDocumentModel;
		InternalRichEditDocumentServer targetServer;
		InternalRichEditDocumentServer recordServer;
		int recordIndex;
		internal MailMergeRecordFinishedEventArgs(DocumentModel targetDocumentModel, DocumentModel recordDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
			this.recordDocumentModel = recordDocumentModel;
		}
		public int RecordIndex { get { return recordIndex; } protected internal set { recordIndex = value; } }
		public Document Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = MailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		public Document RecordDocument {
			get {
				if (recordDocumentModel == null)
					return null;
				if (recordServer == null)
					recordServer = MailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(recordDocumentModel);
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
	#region MailMergeFinishedEventHandler
	[ComVisible(true)]
	public delegate void MailMergeFinishedEventHandler(object sender, MailMergeFinishedEventArgs e);
	#endregion
	#region MailMergeFinishedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class MailMergeFinishedEventArgs : EventArgs {
		readonly DocumentModel targetDocumentModel;
		InternalRichEditDocumentServer targetServer;
		internal MailMergeFinishedEventArgs(DocumentModel targetDocumentModel) {
			this.targetDocumentModel = targetDocumentModel;
		}
		public Document Document {
			get {
				if (targetDocumentModel == null)
					return null;
				if (targetServer == null)
					targetServer = MailMergeStartedEventArgs.CreateDocumentServerForExistingDocumentModel(targetDocumentModel);
				return targetServer.Document;
			}
		}
		internal void Clear() {
			targetServer.Dispose();
			targetServer = null;
		}
	}
	#endregion
	#region ShowReviewingPaneEventHandler
	[ComVisible(true)]
	public delegate void ShowReviewingPaneEventHandler(object sender, ShowReviewingPaneEventArg e);
	#endregion
	#region ShowReviewingPaneEventArg
	[ComVisible(false)]
	public class ShowReviewingPaneEventArg : EventArgs {
		CommentViewInfo commentViewInfo;
		bool selectParagraph;
		DocumentLogPosition start;
		DocumentLogPosition end;
		bool setFocus;
		bool synchronizeSelection;
		public ShowReviewingPaneEventArg(CommentViewInfo commentViewInfo, bool selectParagraph, DocumentLogPosition start, DocumentLogPosition end, bool setFocus, bool synchronizeSelection) {
			this.commentViewInfo = commentViewInfo;
			this.selectParagraph = selectParagraph;
			this.start = start;
			this.end = end;
			this.setFocus = setFocus;
			this.synchronizeSelection = synchronizeSelection;
		}
		public CommentViewInfo CommentViewInfo { get { return commentViewInfo; } set { commentViewInfo = value; } }
		public bool SelectParagraph { get { return selectParagraph; } set { selectParagraph = value; } }
		public DocumentLogPosition Start { get { return start; } set { start = value; } }
		public DocumentLogPosition End { get { return end; } set { end = value; } }
		public bool SetFocus { get { return setFocus; } set { setFocus = value; } }
		public bool SynchronizeSelection { get { return synchronizeSelection; } set { synchronizeSelection = value; } }
	}
	#endregion
	#region ObtainReviewingPaneVisibleEventHandler
	[ComVisible(false)]
	public delegate void ObtainReviewingPaneVisibleEventHandler(object sender, ObtainReviewingPaneVisibleEventArg e);
	#endregion
	#region ObtainReviewingPaneVisibleEventArg
	[ComVisible(false)]
	public class ObtainReviewingPaneVisibleEventArg : EventArgs {
		public ObtainReviewingPaneVisibleEventArg() {
		}
		public bool ReviewingPaneVisible { get; set; }
	}
	#endregion
	#region UpdateReviewingPaneEventHandler
	[ComVisible(true)]
	public delegate void UpdateReviewingPaneEventHandler(object sender, UpdateReviewingPaneEventArg e);
	#endregion
	#region UpdateReviewingPaneEventArg
	[ComVisible(false)]
	public class UpdateReviewingPaneEventArg : EventArgs {
		DocumentLogPosition start;
		DocumentLogPosition end;
		public UpdateReviewingPaneEventArg(DocumentLogPosition start, DocumentLogPosition end) {
			this.start = start;
			this.end = end;
		}
		public DocumentLogPosition Start { get { return start; } set { start = value; } }
		public DocumentLogPosition End { get { return end; } set { end = value; } }
	}
	#endregion
	#region BeforePagePaintEventHandler
	public delegate void BeforePagePaintEventHandler(object sender, BeforePagePaintEventArgs e);
	#endregion
	#region BeforePagePaintEventArgs
	public class BeforePagePaintEventArgs : EventArgs {
		PagePainter painter;
		LayoutPage page;
		DevExpress.XtraRichEdit.Layout.Export.DocumentLayoutExporter exporter;
		CanvasOwnerType canvasOwnerType;
		internal BeforePagePaintEventArgs(PagePainter painter, LayoutPage page, DevExpress.XtraRichEdit.Layout.Export.DocumentLayoutExporter exporter, CanvasOwnerType canvasOwnerType) {
			this.painter = painter;
			this.page = page;
			this.exporter = exporter;
			this.canvasOwnerType = canvasOwnerType;
		}
		public PagePainter Painter {
			get { return painter; }
			set {
				if (Object.ReferenceEquals(painter, value))
					return;
				PagePainter oldPainter = painter;
				painter = value;
				painter.Initialize(exporter, Page);
				painter.Canvas.Synchronize(oldPainter.Canvas);
			}
		}
		public PageCanvas Canvas { get { return painter.Canvas; } }
		public LayoutPage Page { get { return page; } }
		public CanvasOwnerType CanvasOwnerType { get { return canvasOwnerType; } }
	}
	#endregion
}
