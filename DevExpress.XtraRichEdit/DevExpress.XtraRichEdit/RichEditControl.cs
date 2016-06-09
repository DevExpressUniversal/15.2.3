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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Serializing;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using ApiMailMergeOptions = DevExpress.XtraRichEdit.API.Native.MailMergeOptions;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.Office.Internal;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraRichEdit.API.Layout;
#if !WPF
#endif
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
#if SL || WPF
namespace DevExpress.Xpf.RichEdit {
#else
namespace DevExpress.XtraRichEdit {
#endif
	#region RichEditControl
	public partial class RichEditControl : IRichEditDocumentServer, IRichEditControl, IInnerRichEditControlOwner, INotifyPropertyChanged, ICommandAwareControl<RichEditCommandId>, IRichEditDocumentLayoutProvider {
		#region Fields
		InnerRichEditControl innerControl;
		TextColors skinTextColors;
		DocumentFormatsDependencies documentFormatsDependencies = RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies();
		DocumentFormatsDependencies IInnerRichEditDocumentServerOwner.DocumentFormatsDependencies { get { return documentFormatsDependencies; } }
		#endregion
		#region Properties
		protected internal BackgroundThreadUIUpdater BackgroundThreadUIUpdater { get { return InnerControl != null ? InnerControl.BackgroundThreadUIUpdater : null; } }
		protected internal MeasurementAndDrawingStrategy MeasurementAndDrawingStrategy { get { return InnerControl != null ? InnerControl.MeasurementAndDrawingStrategy : null; } }
		[Browsable(false)]
		public DocumentModelAccessor Model { get { return InnerControl.Model; } }
		protected internal DocumentModel DocumentModel { get { return InnerControl != null ? InnerControl.DocumentModel : null; } }
		[Browsable(false)]
		public float DpiX { get { return DocumentModelDpi.DpiX; } }
		[Browsable(false)]
		public float DpiY { get { return DocumentModelDpi.DpiY; } }
		#region SpellChecker
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlSpellChecker"),
#endif
 DefaultValue(null)]
		public ISpellChecker SpellChecker {
			get { return InnerControl != null ? InnerControl.SpellChecker : null; }
			set {
				if (InnerControl != null)
					InnerControl.SpellChecker = value;
			}
		}
		#endregion
		#region Modified
		[
		Browsable(false),
		DefaultValue(false)
		]
		public bool Modified {
			get { return InnerControl != null ? InnerControl.Modified : false; }
			set { if (InnerControl != null) InnerControl.Modified = value; }
		}
		#endregion
		#region Overtype
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlOvertype"),
#endif
		DefaultValue(false)
		]
		public bool Overtype {
			get { return InnerControl != null ? InnerControl.Overtype : false; }
			set { if (InnerControl != null) InnerControl.Overtype = value; }
		}
		bool IRichEditControl.OvertypeAllowed { get { return Options != null ? Options.Behavior.OvertypeAllowed : false; } }
		#endregion
		#region Views
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlViews"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public RichEditViewRepository Views { get { return InnerControl != null ? InnerControl.Views : null; } }
		#endregion
		#region ActiveView
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RichEditView ActiveView { get { return InnerControl != null ? InnerControl.ActiveView : null; } }
		#endregion
		#region ActiveViewType
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlActiveViewType")]
#endif
		public RichEditViewType ActiveViewType {
			get {
				if (InnerControl != null)
					return InnerControl.ActiveViewType;
				else
					return DefaultViewType;
			}
			set {
				if (InnerControl != null)
					InnerControl.ActiveViewType = value;
			}
		}
		protected internal virtual bool ShouldSerializeActiveViewType() {
			return ActiveViewType != DefaultViewType;
		}
		protected internal virtual void ResetActiveViewType() {
			ActiveViewType = DefaultViewType;
		}
		#endregion
		[Browsable(false)]
		public virtual RichEditViewType DefaultViewType { get { return InnerControl != null ? InnerControl.DefaultViewType : RichEditViewType.PrintLayout; } }
		#region ReadOnly
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlReadOnly"),
#endif
DefaultValue(false)]
		public bool ReadOnly {
			get { return InnerControl != null ? InnerControl.ReadOnly : true; }
			set {
				if (InnerControl != null)
					InnerControl.ReadOnly = value;
			}
		}
		#endregion
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		protected internal RichEditControlDeferredChanges ControlDeferredChanges { get { return InnerControl != null ? InnerControl.ControlDeferredChanges : null; } }
		#region Text
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlText")]
#endif
		public
#if !SL && !WPF
 override
#endif
 string Text {
			get {
#if !SL && !WPF
				if (insideHandleDestroyed)
					return String.Empty;
#endif
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.Text;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.Text = (value == null) ? String.Empty : value;
			}
		}
		protected internal virtual bool ShouldSerializeText() {
			return !String.IsNullOrEmpty(this.Text);
		}
#if !SL && !WPF
		public override
#else
		protected internal virtual
#endif
 void ResetText() {
			this.Text = String.Empty;
		}
		#endregion
		#region RtfText
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string RtfText {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.RtfText;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.RtfText = (value == null) ? String.Empty : value;
			}
		}
		#endregion
		#region HtmlText
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string HtmlText {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.HtmlText;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.HtmlText = (value == null) ? String.Empty : value;
			}
		}
		#endregion
		#region MhtText
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string MhtText {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.MhtText;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.MhtText = (value == null) ? String.Empty : value;
			}
		}
		#endregion
		#region WordMLText
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string WordMLText {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.WordMLText;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.WordMLText = (value == null) ? String.Empty : value;
			}
		}
		#endregion
		#region XamlText
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal string XamlText {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.XamlText;
				else
					return String.Empty;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.XamlText = (value == null) ? String.Empty : value;
			}
		}
		#endregion
		#region OpenXmlBytes
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public byte[] OpenXmlBytes {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.OpenXmlBytes;
				else
					return null;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.OpenXmlBytes = value;
			}
		}
		#endregion
		#region OpenDocumentBytes
		[Bindable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public byte[] OpenDocumentBytes {
			get {
				if (DocumentModel != null)
					return DocumentModel.InternalAPI.OpenDocumentBytes;
				else
					return null;
			}
			set {
				if (DocumentModel != null)
					DocumentModel.InternalAPI.OpenDocumentBytes = value;
			}
		}
		#endregion
		#region Document
		[Browsable(false)]
		public Document Document { get { return InnerControl != null ? InnerControl.NativeDocument : null; } }
		#endregion
		#region Document
		[Browsable(false)]
		public DocumentLayout DocumentLayout { get { return InnerControl != null ? InnerControl.DocumentLayout : null; } }
		#endregion
		#region Unit
		[
#if !SL
	DevExpressXtraRichEditLocalizedDescription("RichEditControlUnit"),
#endif
DefaultValue(DocumentUnit.Document)]
		public DocumentUnit Unit {
			get { return InnerControl != null ? InnerControl.Unit : DocumentUnit.Document; }
			set {
				if (InnerControl != null)
					InnerControl.Unit = value;
			}
		}
		#endregion
		#region LayoutUnit
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlLayoutUnit")]
#endif
		public DevExpress.XtraRichEdit.DocumentLayoutUnit LayoutUnit {
			get {
				if (InnerControl != null)
					return InnerControl.LayoutUnit;
				else
					return DefaultLayoutUnit;
			}
			set {
				if (InnerControl != null)
					InnerControl.LayoutUnit = value;
			}
		}
		protected internal virtual bool ShouldSerializeLayoutUnit() {
			return LayoutUnit != DefaultLayoutUnit;
		}
		protected internal virtual void ResetLayoutUnit() {
			LayoutUnit = DefaultLayoutUnit;
		}
		protected virtual DevExpress.XtraRichEdit.DocumentLayoutUnit DefaultLayoutUnit {
			get {
#if !SL && !WPF
				return DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
#else
				return (DevExpress.XtraRichEdit.DocumentLayoutUnit)DocumentModel.DefaultLayoutUnit;
#endif
			}
		}
		#endregion
		[Browsable(false)]
		public bool CanUndo { get { return InnerControl != null ? InnerControl.CanUndo : false; } }
		[Browsable(false)]
		public bool CanRedo { get { return InnerControl != null ? InnerControl.CanRedo : false; } }
		protected internal InnerRichEditDocumentServer InnerDocumentServer { get { return InnerControl; } }
		InnerRichEditDocumentServer IRichEditControl.InnerDocumentServer { get { return this.InnerDocumentServer; } }
		protected internal InnerRichEditControl InnerControl { get { return innerControl; } }
		InnerRichEditControl IRichEditControl.InnerControl { get { return this.InnerControl; } }
		CommandBasedKeyboardHandler<RichEditCommandId> ICommandAwareControl<RichEditCommandId>.KeyboardHandler { get { return InnerControl != null ? InnerControl.KeyboardHandler as CommandBasedKeyboardHandler<RichEditCommandId> : null; } }
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlVerticalScrollPosition")]
#endif
		[Obsolete("This property has become obsolete. Use the 'DevExpress.XtraRichEdit.RichEditControl.VerticalScrollValue' property instead.")]
		public long VerticalScrollPosition {
			get {
				if (InnerControl != null)
					return InnerControl.VerticalScrollPosition;
				else
					return 0;
			}
			set {
				if (InnerControl != null)
					InnerControl.VerticalScrollPosition = value;
			}
		}
		protected internal bool ShouldSerializeVerticalScrollPosition() {
			return false;
		}
		protected internal void ResetVerticalScrollPosition() {
#pragma warning disable 0618
			VerticalScrollPosition = 0;
#pragma warning restore 0618
		}
		public long VerticalScrollValue {
			get {
				if (InnerControl != null)
					return InnerControl.VerticalScrollValue;
				else
					return 0;
			}
			set {
				if (InnerControl != null)
					InnerControl.VerticalScrollValue = value;
			}
		}
		protected internal bool ShouldSerializeVerticalScrollValue() {
			return false;
		}
		protected internal void ResetVerticalScrollValue() {
			VerticalScrollValue = 0;
		}
		RichEditControlOptionsBase IRichEditDocumentServer.Options { get { return this.Options; } }
		TextColors IRichEditControl.SkinTextColors { get { return skinTextColors; } }
		#endregion
		#region Events
		#region ActiveViewChanged
		EventHandler onActiveViewChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlActiveViewChanged")]
#endif
		public event EventHandler ActiveViewChanged { add { onActiveViewChanged += value; } remove { onActiveViewChanged -= value; } }
		protected internal virtual void RaiseActiveViewChanged() {
			if (onActiveViewChanged != null)
				onActiveViewChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ActiveRecordChanged
		EventHandler onActiveRecordChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlActiveRecordChanged")]
#endif
		public event EventHandler ActiveRecordChanged { add { onActiveRecordChanged += value; } remove { onActiveRecordChanged -= value; } }
		protected internal virtual void RaiseActiveRecordChanged() {
			if (onActiveRecordChanged != null)
				onActiveRecordChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ActiveRecordChanging
		EventHandler onActiveRecordChanging;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlActiveRecordChanging")]
#endif
		public event EventHandler ActiveRecordChanging { add { onActiveRecordChanging += value; } remove { onActiveRecordChanging -= value; } }
		protected internal virtual void RaiseActiveRecordChanging() {
			if (onActiveRecordChanging != null)
				onActiveRecordChanging(this, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlSelectionChanged")]
#endif
		public event EventHandler SelectionChanged {
			add { if (InnerControl != null) InnerControl.SelectionChanged += value; }
			remove { if (InnerControl != null) InnerControl.SelectionChanged -= value; }
		}
		#endregion
		#region DocumentLoaded
		EventHandler onDocumentLoaded;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDocumentLoaded")]
#endif
		public event EventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal virtual void RaiseDocumentLoaded() {
			if (onDocumentLoaded != null)
				onDocumentLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region EmptyDocumentCreated
		EventHandler onEmptyDocumentCreated;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlEmptyDocumentCreated")]
#endif
		public event EventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal virtual void RaiseEmptyDocumentCreated() {
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(this, EventArgs.Empty);
		}
		#endregion
		#region CommentInserted
		public event CommentEditingEventHandler CommentInserted;
		protected internal virtual void RaiseCommentInserted(CommentEditingEventArgs e) {
			if (CommentInserted != null)
				CommentInserted(this, e);
		}
		#endregion
		#region ShowReviewingPane
		protected internal event ShowReviewingPaneEventHandler ShowReviewingPane;
		void RaiseShowReviewingPane(ShowReviewingPaneEventArg e) {
			if (ShowReviewingPane == null)
				return;
			ShowReviewingPane(this, e);
		}
		#endregion
		#region ObtainReviewingPaneVisible
		protected internal event ObtainReviewingPaneVisibleEventHandler ObtainReviewingPaneVisible;
		void RaiseObtainReviewingPaneVisible(ObtainReviewingPaneVisibleEventArg e) {
			if (ObtainReviewingPaneVisible == null)
				return;
			ObtainReviewingPaneVisible(this, e);
		}
		#endregion
		#region CloseReviewingPane
		EventHandler onCloseReviewingPane;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlCloseReviewingPane")]
#endif
		public event EventHandler CloseReviewingPane { add { onCloseReviewingPane += value; } remove { onCloseReviewingPane -= value; } }
		protected internal virtual void RaiseCloseReviewingPane() {
			if (onCloseReviewingPane != null)
				onCloseReviewingPane(this, EventArgs.Empty);
		}
		#endregion
		#region UpdateReviewingPaneRuntimeUndoRedo
		protected internal event UpdateReviewingPaneEventHandler UpdateReviewingPaneRuntimeUndoRedo;
		void RaiseUpdateReviewingPane(UpdateReviewingPaneEventArg e) {
			if (UpdateReviewingPaneRuntimeUndoRedo == null)
				return;
			UpdateReviewingPaneRuntimeUndoRedo(this, e);
		}
		#endregion
		#region UpdateReviewingPane
		EventHandler onUpdateReviewingPane;
		protected internal event EventHandler UpdateReviewingPane { add { onUpdateReviewingPane += value; } remove { onUpdateReviewingPane -= value; } }
		protected internal virtual void RaiseUpdateReviewingPane() {
			if (onUpdateReviewingPane != null)
				onUpdateReviewingPane(this, EventArgs.Empty);
		}
		#endregion
		#region DocumentClosing
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDocumentClosing")]
#endif
		public event CancelEventHandler DocumentClosing {
			add { if (InnerControl != null) InnerControl.DocumentClosing += value; }
			remove { if (InnerControl != null) InnerControl.DocumentClosing -= value; }
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlContentChanged")]
#endif
		public event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if (onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentChangedCore
		EventHandler onContentChangedCore;
		protected internal event EventHandler ContentChangedCore { add { onContentChangedCore += value; } remove { onContentChangedCore -= value; } }
		protected internal virtual void RaiseContentChangedCore() {
			if (onContentChangedCore != null)
				onContentChangedCore(this, EventArgs.Empty);
		}
		#endregion
		#region RtfTextChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlRtfTextChanged")]
#endif
		public event EventHandler RtfTextChanged {
			add { if (InnerControl != null) InnerControl.RtfTextChanged += value; }
			remove { if (InnerControl != null) InnerControl.RtfTextChanged -= value; }
		}
		#endregion
		#region HtmlTextChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlHtmlTextChanged")]
#endif
		public event EventHandler HtmlTextChanged {
			add { if (InnerControl != null) InnerControl.HtmlTextChanged += value; }
			remove { if (InnerControl != null) InnerControl.HtmlTextChanged -= value; }
		}
		#endregion
		#region MhtTextChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlMhtTextChanged")]
#endif
		public event EventHandler MhtTextChanged {
			add { if (InnerControl != null) InnerControl.MhtTextChanged += value; }
			remove { if (InnerControl != null) InnerControl.MhtTextChanged -= value; }
		}
		#endregion
		#region WordMLTextChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlWordMLTextChanged")]
#endif
		public event EventHandler WordMLTextChanged {
			add { if (InnerControl != null) InnerControl.WordMLTextChanged += value; }
			remove { if (InnerControl != null) InnerControl.WordMLTextChanged -= value; }
		}
		#endregion
		#region XamlTextChanged
		internal event EventHandler XamlTextChanged {
			add { if (InnerControl != null) InnerControl.XamlTextChanged += value; }
			remove { if (InnerControl != null) InnerControl.XamlTextChanged -= value; }
		}
		#endregion
		#region OpenXmlBytesChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlOpenXmlBytesChanged")]
#endif
		public event EventHandler OpenXmlBytesChanged {
			add { if (InnerControl != null) InnerControl.OpenXmlBytesChanged += value; }
			remove { if (InnerControl != null) InnerControl.OpenXmlBytesChanged -= value; }
		}
		#endregion
		#region OpenDocumentBytesChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlOpenDocumentBytesChanged")]
#endif
		public event EventHandler OpenDocumentBytesChanged {
			add { if (InnerControl != null) InnerControl.OpenDocumentBytesChanged += value; }
			remove { if (InnerControl != null) InnerControl.OpenDocumentBytesChanged -= value; }
		}
		#endregion
		#region ReadOnlyChanged
		EventHandler onReadOnlyChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlReadOnlyChanged")]
#endif
		public event EventHandler ReadOnlyChanged { add { onReadOnlyChanged += value; } remove { onReadOnlyChanged -= value; } }
		protected internal virtual void RaiseReadOnlyChanged() {
			if (onReadOnlyChanged != null)
				onReadOnlyChanged(this, EventArgs.Empty);
		}
		#endregion
		#region OvertypeChanged
		EventHandler onOvertypeChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlOvertypeChanged")]
#endif
		public event EventHandler OvertypeChanged { add { onOvertypeChanged += value; } remove { onOvertypeChanged -= value; } }
		protected internal virtual void RaiseOvertypeChanged() {
			if (onOvertypeChanged != null)
				onOvertypeChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlModifiedChanged")]
#endif
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region UnitChanging
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlUnitChanging")]
#endif
		public event EventHandler UnitChanging {
			add {
				IRichEditDocumentServer server = InnerControl;
				if (server != null)
					server.UnitChanging += value;
			}
			remove {
				IRichEditDocumentServer server = InnerControl;
				if (server != null)
					server.UnitChanging -= value;
			}
		}
		#endregion
		#region UnitChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlUnitChanged")]
#endif
		public event EventHandler UnitChanged {
			add {
				IRichEditDocumentServer server = InnerControl;
				if (server != null)
					server.UnitChanged += value;
			}
			remove {
				IRichEditDocumentServer server = InnerControl;
				if (server != null)
					server.UnitChanged -= value;
			}
		}
		#endregion
		#region CalculateDocumentVariable
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlCalculateDocumentVariable")]
#endif
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable {
			add { if (InnerControl != null) InnerControl.CalculateDocumentVariable += value; }
			remove { if (InnerControl != null) InnerControl.CalculateDocumentVariable -= value; }
		}
		#endregion
		#region UpdateUI
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlUpdateUI")]
#endif
		public event EventHandler UpdateUI {
			add { if (InnerControl != null) InnerControl.UpdateUI += value; }
			remove { if (InnerControl != null) InnerControl.UpdateUI -= value; }
		}
		#endregion
		#region ZoomChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlZoomChanged")]
#endif
		public event EventHandler ZoomChanged {
			add { if (InnerControl != null) InnerControl.ZoomChanged += value; }
			remove { if (InnerControl != null) InnerControl.ZoomChanged -= value; }
		}
		#endregion
		#region BeforeImport
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlBeforeImport")]
#endif
		public event BeforeImportEventHandler BeforeImport {
			add { if (InnerControl != null) InnerControl.BeforeImport += value; }
			remove { if (InnerControl != null) InnerControl.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlBeforeExport")]
#endif
		public event BeforeExportEventHandler BeforeExport {
			add { if (InnerControl != null) InnerControl.BeforeExport += value; }
			remove { if (InnerControl != null) InnerControl.BeforeExport -= value; }
		}
		#endregion
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlAfterExport")]
#endif
		public event EventHandler AfterExport {
			add { if (InnerControl != null) InnerControl.AfterExport += value; }
			remove { if (InnerControl != null) InnerControl.AfterExport -= value; }
		}
		#region InitializeDocument
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlInitializeDocument")]
#endif
		public event EventHandler InitializeDocument {
			add { if (InnerControl != null) InnerControl.InitializeDocument += value; }
			remove { if (InnerControl != null) InnerControl.InitializeDocument -= value; }
		}
		#endregion
		#region UnhandledException
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlUnhandledException")]
#endif
		public event RichEditUnhandledExceptionEventHandler UnhandledException {
			add { if (InnerControl != null) InnerControl.UnhandledException += value; }
			remove { if (InnerControl != null) InnerControl.UnhandledException -= value; }
		}
		#endregion
		#region HyperlinkClick
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { if (InnerControl != null) InnerControl.HyperlinkClick += value; }
			remove { if (InnerControl != null) InnerControl.HyperlinkClick -= value; }
		}
		#endregion
		#region StartHeaderFooterEditing
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlStartHeaderFooterEditing")]
#endif
		public event HeaderFooterEditingEventHandler StartHeaderFooterEditing {
			add { if (InnerControl != null) InnerControl.StartHeaderFooterEditing += value; }
			remove { if (InnerControl != null) InnerControl.StartHeaderFooterEditing -= value; }
		}
		#endregion
		#region FinishHeaderFooterEditing
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlFinishHeaderFooterEditing")]
#endif
		public event HeaderFooterEditingEventHandler FinishHeaderFooterEditing {
			add { if (InnerControl != null) InnerControl.FinishHeaderFooterEditing += value; }
			remove { if (InnerControl != null) InnerControl.FinishHeaderFooterEditing -= value; }
		}
		#endregion
		#region DocumentProtectionChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlDocumentProtectionChanged")]
#endif
		public event EventHandler DocumentProtectionChanged {
			add { if (InnerControl != null) InnerControl.DocumentProtectionChanged += value; }
			remove { if (InnerControl != null) InnerControl.DocumentProtectionChanged -= value; }
		}
		#endregion
		#region AutoCorrect
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlAutoCorrect")]
#endif
		public event AutoCorrectEventHandler AutoCorrect {
			add { if (InnerControl != null) InnerControl.AutoCorrect += value; }
			remove { if (InnerControl != null) InnerControl.AutoCorrect -= value; }
		}
		#endregion
		#region CustomizeMergeFields
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlCustomizeMergeFields")]
#endif
		public event CustomizeMergeFieldsEventHandler CustomizeMergeFields {
			add { if (InnerControl != null) InnerControl.CustomizeMergeFields += value; }
			remove { if (InnerControl != null) InnerControl.CustomizeMergeFields -= value; }
		}
		#endregion
		#region MailMergeStarted
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlMailMergeStarted")]
#endif
		public event MailMergeStartedEventHandler MailMergeStarted {
			add { if (InnerControl != null) InnerControl.MailMergeStarted += value; }
			remove { if (InnerControl != null) InnerControl.MailMergeStarted -= value; }
		}
		#endregion
		#region MailMergeRecordStarted
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlMailMergeRecordStarted")]
#endif
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted {
			add { if (InnerControl != null) InnerControl.MailMergeRecordStarted += value; }
			remove { if (InnerControl != null) InnerControl.MailMergeRecordStarted -= value; }
		}
		#endregion
		#region MailMergeRecordFinished
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlMailMergeRecordFinished")]
#endif
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished {
			add { if (InnerControl != null) InnerControl.MailMergeRecordFinished += value; }
			remove { if (InnerControl != null) InnerControl.MailMergeRecordFinished -= value; }
		}
		#endregion
		#region MailMergeFinished
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlMailMergeFinished")]
#endif
		public event MailMergeFinishedEventHandler MailMergeFinished {
			add { if (InnerControl != null) InnerControl.MailMergeFinished += value; }
			remove { if (InnerControl != null) InnerControl.MailMergeFinished -= value; }
		}
		#endregion
		#region INotifyPropertyChanged Members
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlPropertyChanged")]
#endif
		public event PropertyChangedEventHandler PropertyChanged;
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region InvalidFormatException
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlInvalidFormatException")]
#endif
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException {
			add { if (InnerControl != null) InnerControl.InvalidFormatException += value; }
			remove { if (InnerControl != null) InnerControl.InvalidFormatException -= value; }
		}
		#endregion
		#region ClipboardSetDataException
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlClipboardSetDataException")]
#endif
		public event RichEditClipboardSetDataExceptionEventHandler ClipboardSetDataException {
			add { if (InnerControl != null) InnerControl.ClipboardSetDataException += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardSetDataException -= value; }
		}
		#endregion
		#region BeforePagePaint
		public event BeforePagePaintEventHandler BeforePagePaint {
			add { if (InnerControl != null) InnerControl.BeforePagePaint += value; }
			remove { if (InnerControl != null) InnerControl.BeforePagePaint -= value; }
		}
		#endregion
		#region VisiblePagesChanged
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("RichEditControlVisiblePagesChanged")]
#endif
		public event EventHandler VisiblePagesChanged {
			add { if (InnerControl != null) InnerControl.VisiblePagesChanged += value; }
			remove { if (InnerControl != null) InnerControl.VisiblePagesChanged -= value; }
		}
		#endregion
		#endregion
		protected internal virtual void BeginInitialize() {
			InnerControl.BeginInitialize();
		}
		protected internal virtual void EndInitializeCommon() {
			SubscribeInnerControlEvents();
			InnerControl.EndInitialize();
			AddServices();
		}
		protected internal virtual void SubscribeInnerControlEvents() {
			InnerControl.PlainTextChanged += OnPlainTextChanged;
			InnerControl.ContentChanged += OnInnerControlContentChanged;
			InnerControl.ModifiedChanged += OnModifiedChanged;
			InnerControl.ReadOnlyChanged += OnReadOnlyChanged;
			InnerControl.OvertypeChanged += OnOvertypeChanged;
			InnerControl.ActiveViewChanged += OnActiveViewChanged;
			InnerControl.PageBackgroundChanged += OnPageBackgroundChanged;
			InnerControl.EmptyDocumentCreated += OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded += OnDocumentLoaded;
			InnerControl.ActiveRecordChanged += OnActiveRecordChanged;
			InnerControl.ActiveRecordChanging += OnActiveRecordChanging;
			InnerControl.LayoutUnitChanged += OnLayoutUnitChanged;
			InnerControl.CommentInserted += OnCommentInserted;
		}
		void OnCommentInserted(object sender, CommentEventArgs e) {			
			CommentEditingEventArgs controlArgs = new CommentEditingEventArgs(this.Document.Comments[e.CommentIndex], e.CommentIndex);
			this.RaiseCommentInserted(controlArgs);
		}
		protected internal virtual void UnsubscribeInnerControlEvents() {
			InnerControl.PlainTextChanged -= OnPlainTextChanged;
			InnerControl.ContentChanged -= OnInnerControlContentChanged;
			InnerControl.ModifiedChanged -= OnModifiedChanged;
			InnerControl.ReadOnlyChanged -= OnReadOnlyChanged;
			InnerControl.OvertypeChanged -= OnOvertypeChanged;
			InnerControl.ActiveViewChanged -= OnActiveViewChanged;
			InnerControl.PageBackgroundChanged -= OnPageBackgroundChanged;
			InnerControl.EmptyDocumentCreated -= OnEmptyDocumentCreated;
			InnerControl.DocumentLoaded -= OnDocumentLoaded;
			InnerControl.ActiveRecordChanged -= OnActiveRecordChanged;
			InnerControl.ActiveRecordChanging -= OnActiveRecordChanging;
			InnerControl.LayoutUnitChanged -= OnLayoutUnitChanged;
		}
		protected internal virtual void OnPlainTextChanged(object sender, EventArgs e) {
			OnTextChanged(e);
		}
		void OnInnerControlContentChanged(object sender, EventArgs e) {
			DocumentContentChangedEventArgs args = e as DocumentContentChangedEventArgs;
			bool suppressBindingNotifications = (args == null ? false : args.SuppressBindingNotifications);
			OnInnerControlContentChangedPlatformSpecific(suppressBindingNotifications);
			RaisePropertyChanged("Text");
			RaisePropertyChanged("RtfText");
			RaisePropertyChanged("HtmlText");
			RaisePropertyChanged("MhtText");
			RaisePropertyChanged("WordMLText");
			RaisePropertyChanged("OpenXmlBytes");
			RaisePropertyChanged("OpenDocumentBytes");
			RaiseContentChanged();
		}
		protected internal virtual void OnModifiedChanged(object sender, EventArgs e) {
			RaisePropertyChanged("Modified");
			RaiseModifiedChanged();
		}
		protected internal virtual void OnReadOnlyChanged(object sender, EventArgs e) {
			RaisePropertyChanged("ReadOnly");
			RaiseReadOnlyChanged();
			OnReadOnlyChangedPlatformSpecific();
		}
		protected internal virtual void OnOvertypeChanged(object sender, EventArgs e) {
			RaisePropertyChanged("Overtype");
			RaiseOvertypeChanged();
		}
		protected internal virtual void OnActiveViewChanged(object sender, EventArgs e) {
			RaiseActiveViewChanged();
			OnActiveViewChangedPlatformSpecific();
		}
		protected internal virtual void OnActiveRecordChanged(object sender, EventArgs e) {
			RaiseActiveRecordChanged();
		}
		protected internal virtual void OnActiveRecordChanging(object sender, EventArgs e) {
			RaiseActiveRecordChanging();
		}
		protected internal virtual void OnPageBackgroundChanged(object sender, EventArgs e) {
			OnPageBackgroundChangedPlatformSpecific();
		}
		protected internal virtual void OnEmptyDocumentCreated(object sender, EventArgs e) {
			OnEmptyDocumentCreatedPlatformSpecific();
			RaiseEmptyDocumentCreated();
		}
		protected internal virtual void OnDocumentLoaded(object sender, EventArgs e) {
			OnDocumentLoadedPlatformSpecific();
			RaiseDocumentLoaded();
		}
		#region IDisposable implementation
		protected internal virtual void DisposeCommon() {
			if (innerControl != null) {
				UnsubscribeInnerControlEvents();
				innerControl.Dispose();
				innerControl = null;
			}
		}
		#endregion
		protected internal virtual void AddServices() {
			AddServicesPlatformSpecific();
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (InnerControl != null)
				InnerControl.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (InnerControl != null)
				InnerControl.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (InnerControl != null)
				InnerControl.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (InnerControl != null)
				InnerControl.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (InnerControl != null)
				InnerControl.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (InnerControl != null)
				InnerControl.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public
#if !SL && !WPF
 new
#endif
 virtual object GetService(Type serviceType) {
			if (InnerControl != null)
				return InnerControl.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T GetService<T>() where T : class {
			if (InnerControl != null)
				return InnerControl.GetService<T>();
			else
				return default(T);
		}
		public T ReplaceService<T>(T newService) where T : class {
			if (InnerControl != null)
				return InnerControl.ReplaceService<T>(newService);
			else
				return default(T);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026")]
		public virtual bool CreateNewDocument(bool raiseDocumentClosing = false) {
			if (InnerControl != null)
				return InnerControl.CreateNewDocument(raiseDocumentClosing);
			return true;
		}
		public virtual void LoadDocument() {
			if (InnerControl != null)
				InnerControl.LoadDocument();
		}
		public virtual void LoadDocument(Stream stream, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.LoadDocument(stream, documentFormat);
		}
		public virtual void LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.LoadDocumentTemplate(stream, documentFormat);
		}
		public virtual void SaveDocumentAs() {
			if (InnerControl != null)
				InnerControl.SaveDocumentAs();
		}
		public virtual void SaveDocumentAs(IWin32Window parent) {
			if (InnerControl != null)
				InnerControl.SaveDocumentAs(parent);
		}
		public virtual void SaveDocument(Stream stream, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.SaveDocument(stream, documentFormat);
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (InnerControl != null)
				InnerControl.BeginUpdate();
		}
		public void EndUpdate() {
			if (InnerControl != null)
				InnerControl.EndUpdate();
		}
		public void CancelUpdate() {
			if (InnerControl != null)
				InnerControl.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper {
			get {
				IBatchUpdateable updateable = InnerControl;
				if (updateable != null)
					return updateable.BatchUpdateHelper;
				else
					return null;
			}
		}
		[Browsable(false)]
		public bool IsUpdateLocked { get { return InnerControl != null ? InnerControl.IsUpdateLocked : false; } }
		#endregion
		protected internal virtual InnerRichEditControl CreateInnerControl() {
			return new InnerRichEditControl(this, DpiX, DpiY);
		}
		protected internal virtual void ApplyFontAndForeColor() {
			if (InnerControl != null)
				InnerControl.ApplyFontAndForeColor();
		}
		void IRichEditControl.UpdateUIFromBackgroundThread(Action method) {
			this.UpdateUIFromBackgroundThread(method);
		}
		protected internal virtual void UpdateUIFromBackgroundThread(Action method) {
			BackgroundThreadUIUpdater.UpdateUI(method);
		}
		protected internal virtual void PerformDeferredUIUpdates(DeferredBackgroundThreadUIUpdater deferredUpdater) {
			List<Action> deferredUpdates = deferredUpdater.Updates;
			int count = deferredUpdates.Count;
			for (int i = 0; i < count; i++)
				BackgroundThreadUIUpdater.UpdateUI(deferredUpdates[i]);
		}
		protected internal virtual void UpdateVerticalScrollBar(bool avoidJump) {
			if (InnerControl != null)
				InnerControl.UpdateVerticalScrollBar(avoidJump);
		}
		Command ICommandAwareControl<RichEditCommandId>.CreateCommand(RichEditCommandId commandId) {
			return this.CreateCommand(commandId);
		}
		public virtual RichEditCommand CreateCommand(RichEditCommandId commandId) {
			if (InnerControl != null)
				return InnerControl.CreateCommand(commandId);
			else
				return null;
		}
		public DocumentPosition GetPositionFromPoint(PointF clientPoint) {
			if (InnerControl != null) {
#if SL || WPF
				if (Surface != null) {
					try {
						System.Windows.Point relativeOffset = this.TransformToVisual(Surface).Transform(new System.Windows.Point(0, 0));
						System.Windows.Point pt = this.GetPositionSafe();
						Size layoutUnitOffset = this.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(new Size((int)(relativeOffset.X + pt.X), (int)(relativeOffset.Y + pt.Y)), DpiX, DpiY);
						SizeF unitOffset = InnerControl.LayoutUnitsToUnits(layoutUnitOffset);
						clientPoint.X += unitOffset.Width;
						clientPoint.Y += unitOffset.Height;
					}
					catch {
					}
				}
#endif
				return InnerControl.GetPositionFromPoint(clientPoint);
			}
			else
				return null;
		}
		public Rectangle GetBoundsFromPosition(DocumentPosition pos) {
			return Rectangle.Round(GetBoundsFromPositionF(pos));
		}
		public RectangleF GetBoundsFromPositionF(DocumentPosition pos) {
			if (InnerControl != null)
				return InnerControl.GetBoundsFromPosition(pos);
			else
				return RectangleF.Empty;
		}
		public Rectangle GetLayoutPhysicalBoundsFromPosition(DocumentPosition pos) {
			if (InnerControl != null)
				return InnerControl.GetLayoutPhysicalBoundsFromPosition(pos);
			else
				return Rectangle.Empty;
		}
		protected internal Rectangle GetLayoutLogicalBoundsFromPosition(DocumentPosition pos) {
			if (InnerControl != null)
				return InnerControl.GetLayoutLogicalBoundsFromPosition(pos);
			else
				return Rectangle.Empty;
		}
		protected internal Rectangle GetPixelPhysicalBounds(PageViewInfo pageViewInfo, Rectangle logicalBounds) {
			if (pageViewInfo == null)
				return Rectangle.Empty;
			Rectangle physicalBounds = ActiveView.CreatePhysicalRectangle(pageViewInfo, logicalBounds);
			physicalBounds.X += ViewBounds.Left;
			physicalBounds.Y += ViewBounds.Top;
			return ActiveView.DocumentLayout.UnitConverter.LayoutUnitsToPixels(physicalBounds, DpiX, DpiY);
		}
		bool ICommandAwareControl<RichEditCommandId>.HandleException(Exception e) {
			return this.HandleException(e);
		}
		protected internal virtual bool HandleException(Exception e) {
			if (InnerControl != null)
				return InnerControl.RaiseUnhandledException(e);
			else
				return false;
		}
		public void ScrollToCaret() {
			if (InnerControl != null)
				InnerControl.ScrollToCaret(-1);
		}
		public void ScrollToCaret(float relativeVerticalPosition) {
			if (InnerControl != null)
				InnerControl.ScrollToCaret(relativeVerticalPosition);
		}
		public void Undo() {
			if (InnerControl != null)
				InnerControl.Undo();
		}
		public void Redo() {
			if (InnerControl != null)
				InnerControl.Redo();
		}
		public void ClearUndo() {
			if (InnerControl != null)
				InnerControl.ClearUndo();
		}
		public void Cut() {
			if (InnerControl != null)
				InnerControl.Cut();
		}
		public void Copy() {
			if (InnerControl != null)
				InnerControl.Copy();
		}
		public void Paste() {
			if (InnerControl != null)
				InnerControl.Paste();
		}
		public void SelectAll() {
			if (InnerControl != null)
				InnerControl.SelectAll();
		}
		public void DeselectAll() {
			if (InnerControl != null)
				InnerControl.DeselectAll();
		}
		public bool IsSelectionInTable() {
			return DocumentModel.Selection.IsWholeSelectionInOneTable();
		}
		[Browsable(false)]
		public bool IsFloatingObjectSelected { get { return DocumentModel.Selection.IsFloatingObjectSelected(); } }
		[Browsable(false)]
		public bool IsSelectionInHeader { get { return DocumentModel.Selection.PieceTable.IsHeader; } }
		[Browsable(false)]
		public bool IsSelectionInFooter { get { return DocumentModel.Selection.PieceTable.IsFooter; } }
		[Browsable(false)]
		public bool IsSelectionInHeaderOrFooter { get { return DocumentModel.Selection.PieceTable.IsHeaderFooter; } }
		[Browsable(false)]
		public bool IsSelectionInComment { get { return DocumentModel.Selection.PieceTable.IsComment; } }
		[Browsable(false)]
		public bool IsSelectionInTextBox { get { return DocumentModel.Selection.PieceTable.IsTextBox; } }
		public void UpdateCommandUI() {
			if (InnerControl != null)
				InnerControl.RaiseUpdateUI();
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, RichEditCommandId commandId) {
			if (InnerControl != null)
				InnerControl.AssignShortcutKeyToCommand(key, modifier, commandId);
		}
		public void AssignShortcutKeyToCommand(Keys key, Keys modifier, RichEditCommandId commandId, RichEditViewType viewType) {
			if (InnerControl != null)
				InnerControl.AssignShortcutKeyToCommand(key, modifier, commandId, viewType);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier) {
			if (InnerControl != null)
				InnerControl.RemoveShortcutKey(key, modifier);
		}
		public void RemoveShortcutKey(Keys key, Keys modifier, RichEditViewType viewType) {
			if (InnerControl != null)
				InnerControl.RemoveShortcutKey(key, modifier, viewType);
		}
		public Keys GetShortcutKey(RichEditCommandId commandId, RichEditViewType viewType) {
			if (InnerControl != null)
				return InnerControl.GetShortcut(commandId, viewType);
			return Keys.None;
		}
		public void ForceSyntaxHighlight() {
			DocumentModel.ForceSyntaxHighlight();
		}
		#region Mail Merge
		public ApiMailMergeOptions CreateMailMergeOptions() {
			if (InnerControl != null)
				return InnerControl.CreateMailMergeOptions();
			else
				return new NativeMailMergeOptions();
		}
		public void MailMerge(Document document) {
			if (InnerControl != null)
				InnerControl.MailMerge(document);
		}
		public void MailMerge(ApiMailMergeOptions options, Document targetDocument) {
			if (InnerControl != null)
				InnerControl.MailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(IRichEditDocumentServer documentServer) {
			if (InnerControl != null)
				InnerControl.MailMerge(documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			if (InnerControl != null)
				InnerControl.MailMerge(options, targetDocumentServer);
		}
		public void MailMerge(string fileName, DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.MailMerge(fileName, format);
		}
		public void MailMerge(Stream stream, DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.MailMerge(stream, format);
		}
		public void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.MailMerge(options, fileName, format);
		}
		public void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format) {
			if (InnerControl != null)
				InnerControl.MailMerge(options, stream, format);
		}
		#endregion
		public virtual IRichEditDocumentServer CreateDocumentServer() {
			return new InternalRichEditDocumentServer(documentFormatsDependencies);
		}
		public SizeF MeasureSingleLineString(string text, CharacterPropertiesBase properties) {
			string fontName = properties.FontName;
			if (String.IsNullOrEmpty(fontName))
				return Size.Empty;
			float? fontSize = properties.FontSize;
			if (!fontSize.HasValue)
				return Size.Empty;
			float? doubleFontSize = properties.FontSize * 2;
			if (!doubleFontSize.HasValue)
				return Size.Empty;
			bool fontBold = properties.Bold.HasValue ? properties.Bold.Value : false;
			bool fontItalic = properties.Italic.HasValue ? properties.Italic.Value : false;
			CharacterFormattingScript fontScript;
			if (properties.Subscript.HasValue && properties.Subscript.Value)
				fontScript = CharacterFormattingScript.Subscript;
			else if (properties.Superscript.HasValue && properties.Superscript.Value)
				fontScript = CharacterFormattingScript.Superscript;
			else
				fontScript = CharacterFormattingScript.Normal;
			Size result;
			InnerControl.BeginDocumentRendering();
			try {
				FontCache cache = DocumentModel.FontCache;
				int index = cache.CalcFontIndex(fontName, (int)Math.Round(doubleFontSize.Value), fontBold, fontItalic, fontScript, false, false);
				if (index < 0)
					return Size.Empty;
				result = cache.Measurer.MeasureString(text, cache[index]);
			}
			finally {
				InnerControl.EndDocumentRendering();
			}
			DevExpress.Office.API.Internal.UnitConverter unitConverter = DocumentModel.InternalAPI.UnitConverters[Unit];
			float width = unitConverter.FromUnits(DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(result.Width));
			float height = unitConverter.FromUnits(DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(result.Height));
			return new SizeF(width, height);
		}
		#region IRichEditDocumentLayoutProvider Members
		CalculationModeType IRichEditDocumentLayoutProvider.LayoutCalculationMode { get { return CalculationModeType.Automatic; } }
		DevExpress.XtraRichEdit.Layout.DocumentLayout IRichEditDocumentLayoutProvider.GetDocumentLayout() {
			return ((IRichEditDocumentLayoutProvider)this).GetDocumentLayoutAsync();
		}
		event DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventHandler IRichEditDocumentLayoutProvider.DocumentLayoutInvalidated {
			add { this.InnerControl.DocumentLayoutInvalidated += value; }
			remove { this.InnerControl.DocumentLayoutInvalidated -= value; }
		}
		event EventHandler IRichEditDocumentLayoutProvider.DocumentFormatted {
			add { this.InnerControl.DocumentFormatted += value; }
			remove { this.InnerControl.DocumentFormatted -= value; }
		}
		XtraRichEdit.Layout.DocumentLayout IRichEditDocumentLayoutProvider.GetDocumentLayoutAsync() {
			return ActiveView.DocumentLayout;
		}
		event PageFormattedEventHandler IRichEditDocumentLayoutProvider.PageFormatted {
			add { this.InnerControl.PageFormatted += value; }
			remove { this.InnerControl.PageFormatted -= value; }
		}
		void IRichEditDocumentLayoutProvider.PerformPageSecondaryFormatting(DevExpress.XtraRichEdit.Layout.Page page) {
			this.InnerControl.Formatter.PerformPageSecondaryFormatting(page);
		}
		#endregion
	}
	#endregion
}
