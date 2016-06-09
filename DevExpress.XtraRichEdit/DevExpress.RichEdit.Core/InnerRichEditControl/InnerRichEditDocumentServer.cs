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
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Utils.Controls;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.Office.Services.Implementation;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using ApiMailMergeOptions = DevExpress.XtraRichEdit.API.Native.MailMergeOptions;
using DevExpress.Office.Internal;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if !SL
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit {
}
namespace DevExpress.XtraRichEdit {
	public enum CalculationModeType {
		Automatic,
		Manual
	}
	public interface IRichEditDocumentServer : IBatchUpdateable, IServiceContainer, IDisposable {
		bool IsDisposed { get; }
		DocumentModelAccessor Model { get; }
		float DpiX { get; }
		float DpiY { get; }
		bool Modified { get; set; }
		string Text { get; set; }
		string RtfText { get; set; }
		string HtmlText { get; set; }
		string MhtText { get; set; }
		string WordMLText { get; set; }
		byte[] OpenXmlBytes { get; set; }
		byte[] OpenDocumentBytes { get; set; }
		Document Document { get; }
		DocumentUnit Unit { get; set; }
		DocumentLayoutUnit LayoutUnit { get; set; }
		RichEditControlOptionsBase Options { get; }
		event EventHandler SelectionChanged;
		event EventHandler DocumentLoaded;
		event EventHandler EmptyDocumentCreated;
		event CancelEventHandler DocumentClosing;
		event EventHandler ContentChanged;
		event EventHandler RtfTextChanged;
		event EventHandler HtmlTextChanged;
		event EventHandler MhtTextChanged;
		event EventHandler WordMLTextChanged;
		event EventHandler OpenXmlBytesChanged;
		event EventHandler OpenDocumentBytesChanged;
		event EventHandler ModifiedChanged;
		event EventHandler UnitChanging;
		event EventHandler UnitChanged;
		event CalculateDocumentVariableEventHandler CalculateDocumentVariable;
		event BeforeImportEventHandler BeforeImport;
		event BeforeExportEventHandler BeforeExport;
		event EventHandler AfterExport;
		event EventHandler InitializeDocument;
		event MailMergeStartedEventHandler MailMergeStarted;
		event MailMergeRecordStartedEventHandler MailMergeRecordStarted;
		event MailMergeRecordFinishedEventHandler MailMergeRecordFinished;
		event MailMergeFinishedEventHandler MailMergeFinished;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026")]
		bool CreateNewDocument(bool raiseDocumentClosing = false);
		void LoadDocument(Stream stream, DocumentFormat documentFormat);
		void SaveDocument(Stream stream, DocumentFormat documentFormat);
		void LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat);
		#region Mail Merge
		ApiMailMergeOptions CreateMailMergeOptions();
		void MailMerge(Document document);
		void MailMerge(ApiMailMergeOptions options, Document targetDocument);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		void MailMerge(IRichEditDocumentServer documentServer);
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer);
		void MailMerge(string fileName, DocumentFormat format);
		void MailMerge(Stream stream, DocumentFormat format);
		void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format);
		void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format);
		#endregion
		T GetService<T>() where T : class;
		T ReplaceService<T>(T newService) where T : class;
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	#region InternalRichEditDocumentServer
	public partial class InternalRichEditDocumentServer : IRichEditDocumentServer, IInnerRichEditDocumentServerOwner, IRichEditDocumentLayoutProvider {
		protected internal static InternalRichEditDocumentServer TryConvertInternalRichEditDocumentServer(object obj) {
			DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer server = obj as DevExpress.XtraRichEdit.Internal.InternalRichEditDocumentServer;
			if (server != null)
				return server;
			DevExpress.XtraRichEdit.Internal.IInternalRichEditDocumentServerOwner owner = obj as DevExpress.XtraRichEdit.Internal.IInternalRichEditDocumentServerOwner;
			if (owner != null)
				return owner.InternalServer;
			return null;
		}
		#region Fields
		bool isDisposing;
		bool isDisposed;
		InnerRichEditDocumentServer innerServer;
		#endregion
		public InternalRichEditDocumentServer(DocumentFormatsDependencies documentFormatsDependencies) {
			DocumentFormatsDependencies = documentFormatsDependencies;
			this.innerServer = CreateInnerServer(null);
			BeginInitialize();
			EndInitialize();
			SubscribeToDocumentModelEvents();
		}
		protected internal InternalRichEditDocumentServer(DocumentModel documentModel) {
			this.DocumentFormatsDependencies = documentModel.DocumentFormatsDependencies;
			this.innerServer = CreateInnerServer(documentModel);
			BeginInitialize();
			EndInitialize();
			SubscribeToDocumentModelEvents();
		}
		#region Properties
		protected internal bool InnerIsDisposed { get { return isDisposed; } }
		protected internal bool InnerIsDisposing { get { return isDisposing; } }
		public InnerRichEditDocumentServer InnerServer { get { return innerServer; } }
		public DocumentModelAccessor Model { get { return InnerServer.Model; } }
		public float DpiX { get { return InnerServer.DpiX; } }
		public float DpiY { get { return InnerServer.DpiY; } }
		protected internal DocumentModel DocumentModel { get { return InnerServer != null ? InnerServer.DocumentModel : null; } }
		public DocumentFormatsDependencies DocumentFormatsDependencies { get; private set; }
		public CalculationModeType LayoutCalculationMode { get { return InnerServer.LayoutCalculationMode; } set { InnerServer.LayoutCalculationMode = value; } }
		#region Modified
		public bool Modified {
			get { return InnerServer != null ? InnerServer.Modified : false; }
			set { if (InnerServer != null) InnerServer.Modified = value; }
		}
		#endregion
		#region Text
		public string Text {
			get {
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
		#endregion
		#region RtfText
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
		#region OpenXmlBytes
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
		public Document Document { get { return InnerServer != null ? InnerServer.NativeDocument : null; } }
		#endregion
		#region Unit
		public DocumentUnit Unit {
			get { return InnerServer != null ? InnerServer.Unit : DocumentUnit.Document; }
			set {
				if (InnerServer != null)
					InnerServer.Unit = value;
			}
		}
		#endregion
		#region LayoutUnit
		public DocumentLayoutUnit LayoutUnit {
			get {
				if (InnerServer != null)
					return InnerServer.LayoutUnit;
				else
					return (DocumentLayoutUnit)DocumentModel.DefaultLayoutUnit;
			}
			set {
				if (InnerServer != null)
					InnerServer.LayoutUnit = value;
			}
		}
		#endregion
		public RichEditControlOptionsBase Options {
			get {
				if (InnerServer != null)
					return InnerServer.Options;
				else
					return null;
			}
		}
		internal BackgroundFormatter Formatter { get { return InnerServer.BackgroundFormatter; } }
		#endregion
		#region Events
		#region SelectionChanged
		public event EventHandler SelectionChanged {
			add { if (InnerServer != null) InnerServer.SelectionChanged += value; }
			remove { if (InnerServer != null) InnerServer.SelectionChanged -= value; }
		}
		#endregion
		#region DocumentLoaded
		public event EventHandler DocumentLoaded {
			add { if (InnerServer != null) InnerServer.DocumentLoaded += value; }
			remove { if (InnerServer != null) InnerServer.DocumentLoaded -= value; }
		}
		#endregion
		#region EmptyDocumentCreated
		public event EventHandler EmptyDocumentCreated {
			add { if (InnerServer != null) InnerServer.EmptyDocumentCreated += value; }
			remove { if (InnerServer != null) InnerServer.EmptyDocumentCreated -= value; }
		}
		#endregion
		#region DocumentClosing
		public event CancelEventHandler DocumentClosing {
			add { if (InnerServer != null) InnerServer.DocumentClosing += value; }
			remove { if (InnerServer != null) InnerServer.DocumentClosing -= value; }
		}
		#endregion
		#region AutoCorrect
		internal event AutoCorrectEventHandler AutoCorrect {
			add { if (InnerServer != null) InnerServer.AutoCorrect += value; }
			remove { if (InnerServer != null) InnerServer.AutoCorrect -= value; }
		}
		#endregion
		#region ContentChanged
		public event EventHandler ContentChanged {
			add { if (InnerServer != null) InnerServer.ContentChanged += value; }
			remove { if (InnerServer != null) InnerServer.ContentChanged -= value; }
		}
		#endregion
		#region RtfTextChanged
		public event EventHandler RtfTextChanged {
			add { if (InnerServer != null) InnerServer.RtfTextChanged += value; }
			remove { if (InnerServer != null) InnerServer.RtfTextChanged -= value; }
		}
		#endregion
		#region HtmlTextChanged
		public event EventHandler HtmlTextChanged {
			add { if (InnerServer != null) InnerServer.HtmlTextChanged += value; }
			remove { if (InnerServer != null) InnerServer.HtmlTextChanged -= value; }
		}
		#endregion
		#region MhtTextChanged
		public event EventHandler MhtTextChanged {
			add { if (InnerServer != null) InnerServer.MhtTextChanged += value; }
			remove { if (InnerServer != null) InnerServer.MhtTextChanged -= value; }
		}
		#endregion
		#region WordMLTextChanged
		public event EventHandler WordMLTextChanged {
			add { if (InnerServer != null) InnerServer.WordMLTextChanged += value; }
			remove { if (InnerServer != null) InnerServer.WordMLTextChanged -= value; }
		}
		#endregion
		#region XamlTextChanged
		internal event EventHandler XamlTextChanged {
			add { if (InnerServer != null) InnerServer.XamlTextChanged += value; }
			remove { if (InnerServer != null) InnerServer.XamlTextChanged -= value; }
		}
		#endregion
		#region OpenXmlBytesChanged
		public event EventHandler OpenXmlBytesChanged {
			add { if (InnerServer != null) InnerServer.OpenXmlBytesChanged += value; }
			remove { if (InnerServer != null) InnerServer.OpenXmlBytesChanged -= value; }
		}
		#endregion
		#region OpenDocumentBytesChanged
		public event EventHandler OpenDocumentBytesChanged {
			add { if (InnerServer != null) InnerServer.OpenDocumentBytesChanged += value; }
			remove { if (InnerServer != null) InnerServer.OpenDocumentBytesChanged -= value; }
		}
		#endregion
		#region ModifiedChanged
		public event EventHandler ModifiedChanged {
			add { if (InnerServer != null) InnerServer.ModifiedChanged += value; }
			remove { if (InnerServer != null) InnerServer.ModifiedChanged -= value; }
		}
		#endregion
		#region UnitChanging
		event EventHandler IRichEditDocumentServer.UnitChanging {
			add {
				IRichEditDocumentServer server = InnerServer;
				if (server != null)
					server.UnitChanging += value;
			}
			remove {
				IRichEditDocumentServer server = InnerServer;
				if (server != null)
					server.UnitChanging -= value;
			}
		}
		#endregion
		#region UnitChanged
		event EventHandler IRichEditDocumentServer.UnitChanged {
			add {
				IRichEditDocumentServer server = InnerServer;
				if (server != null)
					server.UnitChanged += value;
			}
			remove {
				IRichEditDocumentServer server = InnerServer;
				if (server != null)
					server.UnitChanged -= value;
			}
		}
		#endregion
		#region CalculateDocumentVariable
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable {
			add { if (InnerServer != null) InnerServer.CalculateDocumentVariable += value; }
			remove { if (InnerServer != null) InnerServer.CalculateDocumentVariable -= value; }
		}
		#endregion
		#region BeforeImport
		public event BeforeImportEventHandler BeforeImport {
			add { if (InnerServer != null) InnerServer.BeforeImport += value; }
			remove { if (InnerServer != null) InnerServer.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
		public event BeforeExportEventHandler BeforeExport {
			add { if (InnerServer != null) InnerServer.BeforeExport += value; }
			remove { if (InnerServer != null) InnerServer.BeforeExport -= value; }
		}
		#endregion
		public event EventHandler AfterExport {
			add { if (InnerServer != null) InnerServer.AfterExport += value; }
			remove { if (InnerServer != null) InnerServer.AfterExport -= value; }
		}
		#region InitializeDocument
		public event EventHandler InitializeDocument {
			add { if (InnerServer != null) InnerServer.InitializeDocument += value; }
			remove { if (InnerServer != null) InnerServer.InitializeDocument -= value; }
		}
		#endregion
		#region MailMergeStarted
		public event MailMergeStartedEventHandler MailMergeStarted {
			add { if (InnerServer != null) InnerServer.MailMergeStarted += value; }
			remove { if (InnerServer != null) InnerServer.MailMergeStarted -= value; }
		}
		#endregion
		#region MailMergeRecordStarted
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted {
			add { if (InnerServer != null) InnerServer.MailMergeRecordStarted += value; }
			remove { if (InnerServer != null) InnerServer.MailMergeRecordStarted -= value; }
		}
		#endregion
		#region MailMergeRecordFinished
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished {
			add { if (InnerServer != null) InnerServer.MailMergeRecordFinished += value; }
			remove { if (InnerServer != null) InnerServer.MailMergeRecordFinished -= value; }
		}
		#endregion
		#region MailMergeFinished
		public event MailMergeFinishedEventHandler MailMergeFinished {
			add { if (InnerServer != null) InnerServer.MailMergeFinished += value; }
			remove { if (InnerServer != null) InnerServer.MailMergeFinished -= value; }
		}
		#endregion
		#region InvalidFormatException
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException {
			add { if (InnerServer != null) InnerServer.InvalidFormatException += value; }
			remove { if (InnerServer != null) InnerServer.InvalidFormatException -= value; }
		}
		#endregion
		#region UnhandledException
		public event RichEditUnhandledExceptionEventHandler UnhandledException {
			add { if (InnerServer != null) InnerServer.UnhandledException += value; }
			remove { if (InnerServer != null) InnerServer.UnhandledException -= value; }
		}
		#endregion
		#region BeforePagePaint
		internal event BeforePagePaintEventHandler BeforePagePaint {
			add { if (InnerServer != null) InnerServer.BeforePagePaint += value; }
			remove { if (InnerServer != null) InnerServer.BeforePagePaint -= value; }
		}
		#endregion
		#region CommentInserted
		public event CommentEventHandler CommentInserted {
			add { if (InnerServer != null) InnerServer.CommentInserted += value; }
			remove { if (InnerServer != null) InnerServer.CommentInserted -= value; }
		}
		#endregion
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		protected internal virtual void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				this.isDisposed = true;
				this.isDisposing = false;
			}
		}
		#endregion
		protected internal virtual void DisposeCore() {
			UnsubscribeFromDocumentModelEvents();
			if (innerServer != null) {
				innerServer.Dispose();
				innerServer = null;
			}
		}
		void SubscribeToDocumentModelEvents() {
			if (DocumentModel != null) {
				DocumentModel.BeginDocumentUpdate += OnBeginDocumentUpdate;
				DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
			}
		}
		void UnsubscribeFromDocumentModelEvents() {
			if (DocumentModel != null) {
				DocumentModel.BeginDocumentUpdate -= OnBeginDocumentUpdate;
				DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
			}
		}
		void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			if (Formatter != null)
				Formatter.EndDocumentUpdate();
		}
		void OnBeginDocumentUpdate(object sender, EventArgs e) {
			if (Formatter != null)
				Formatter.BeginDocumentUpdate();
		}
		protected internal virtual InnerRichEditDocumentServer CreateInnerServer(DocumentModel documentModel) {
			if (documentModel == null)
				return new InnerRichEditDocumentServer(this);
			else
				return new InnerRichEditDocumentServer(this, documentModel);
		}
		protected internal virtual void BeginInitialize() {
			InnerServer.BeginInitialize();
		}
		protected internal virtual void EndInitialize() {
			InnerServer.EndInitialize();
		}
		#region IInnerRichEditDocumentServerOwner Members
		MeasurementAndDrawingStrategy IInnerRichEditDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
#if !SL && !DXPORTABLE
			if (PrecalculatedMetricsFontCacheManager.ShouldUse() || (documentModel.PrintingOptions.DrawLayoutFromSilverlightRendering && documentModel.ModelForExport))
				return new ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(documentModel);
			else
				return new ServerGdiMeasurementAndDrawingStrategy(documentModel);
#else
			return new ServerPrecalculatedMetricsMeasurementAndDrawingStrategy(documentModel);
#endif
		}
		RichEditControlOptionsBase IInnerRichEditDocumentServerOwner.CreateOptions(InnerRichEditDocumentServer documentServer) {
			return CreateOptionsCore(documentServer);
		}
		protected internal virtual RichEditControlOptionsBase CreateOptionsCore(InnerRichEditDocumentServer documentServer) {
			return new RichEditDocumentServerOptions(documentServer);
		}
		void IInnerRichEditDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			IThreadSyncService service = InnerServer.GetService<IThreadSyncService>();
			if (service != null)
				service.EnqueueInvokeInUIThread(new Action(delegate() { InnerServer.RaiseDeferredEventsCore(changeActions); }));
			else
				InnerServer.RaiseDeferredEventsCore(changeActions);
		}
		#endregion
		#region IBatchUpdateable Members
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper {
			get {
				IBatchUpdateable updateable = InnerServer;
				return updateable.BatchUpdateHelper;
			}
		}
		public void BeginUpdate() {
			if (InnerServer != null)
				InnerServer.BeginUpdate();
		}
		public void CancelUpdate() {
			if (InnerServer != null)
				InnerServer.CancelUpdate();
		}
		public void EndUpdate() {
			if (InnerServer != null)
				InnerServer.EndUpdate();
		}
		public bool IsUpdateLocked {
			get {
				if (InnerServer != null)
					return InnerServer.IsUpdateLocked;
				else
					return false;
			}
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (InnerServer != null)
				InnerServer.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (InnerServer != null)
				InnerServer.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (InnerServer != null)
				InnerServer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (InnerServer != null)
				InnerServer.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (InnerServer != null)
				InnerServer.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (InnerServer != null)
				InnerServer.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if (InnerServer != null)
				return InnerServer.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T GetService<T>() where T : class {
			if (InnerServer != null)
				return InnerServer.GetService<T>();
			else
				return default(T);
		}
		public T ReplaceService<T>(T newService) where T : class {
			if (InnerServer != null)
				return InnerServer.ReplaceService<T>(newService);
			else
				return default(T);
		}
		#region IRichEditDocumentServer Members
		bool IRichEditDocumentServer.IsDisposed { get { return InnerIsDisposed; } }
		#endregion
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026")]
		public virtual bool CreateNewDocument(bool raiseDocumentClosing = false) {
			if (InnerServer != null)
				return InnerServer.CreateNewDocument(raiseDocumentClosing);
			else
				return true;
		}
		public virtual void LoadDocument(Stream stream, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.LoadDocument(stream, documentFormat);
		}
		public virtual void SaveDocument(Stream stream, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.SaveDocument(stream, documentFormat);
		}
		public virtual void LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.LoadDocumentTemplate(stream, documentFormat);
		}
#if !SL
		public virtual void LoadDocument(string fileName) {
			if (InnerServer != null)
				InnerServer.LoadDocument(fileName);
		}
		public virtual void LoadDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.LoadDocument(fileName, documentFormat);
		}
		public virtual void LoadDocumentTemplate(string fileName) {
			if (InnerServer != null)
				InnerServer.LoadDocumentTemplate(fileName);
		}
		public virtual void LoadDocumentTemplate(string fileName, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.LoadDocumentTemplate(fileName, documentFormat);
		}
		public virtual void SaveDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerServer != null)
				InnerServer.SaveDocument(fileName, documentFormat);
		}
#endif
		#region Mail Merge
		public ApiMailMergeOptions CreateMailMergeOptions() {
			if (InnerServer != null)
				return InnerServer.CreateMailMergeOptions();
			else
				return new NativeMailMergeOptions();
		}
		public void MailMerge(Document document) {
			if (InnerServer != null)
				InnerServer.MailMerge(document);
		}
		public void MailMerge(ApiMailMergeOptions options, Document targetDocument) {
			if (InnerServer != null)
				InnerServer.MailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(IRichEditDocumentServer documentServer) {
			if (InnerServer != null)
				InnerServer.MailMerge(documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			if (InnerServer != null)
				InnerServer.MailMerge(options, targetDocumentServer);
		}
		public void MailMerge(string fileName, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.MailMerge(fileName, format);
		}
		public void MailMerge(Stream stream, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.MailMerge(stream, format);
		}
		public void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.MailMerge(options, fileName, format);
		}
		public void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format) {
			if (InnerServer != null)
				InnerServer.MailMerge(options, stream, format);
		}
		#endregion
		#region IRichEditDocumentLayoutProvider Members
		Layout.DocumentLayout IRichEditDocumentLayoutProvider.GetDocumentLayout() {
			return this.innerServer.CalculatePrintDocumentLayout(this);
		}
		Layout.DocumentLayout IRichEditDocumentLayoutProvider.GetDocumentLayoutAsync() {
			return this.innerServer.ModelDocumentLayout;
		}
		event DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventHandler IRichEditDocumentLayoutProvider.DocumentLayoutInvalidated {
			add { this.innerServer.DocumentLayoutInvalidated += value; }
			remove { this.innerServer.DocumentLayoutInvalidated -= value; }
		}
		event DevExpress.XtraRichEdit.API.Layout.PageFormattedEventHandler IRichEditDocumentLayoutProvider.PageFormatted {
			add { this.innerServer.PageFormatted += value; }
			remove { this.innerServer.PageFormatted -= value; }
		}
		event EventHandler IRichEditDocumentLayoutProvider.DocumentFormatted {
			add { this.innerServer.DocumentFormatted += value; }
			remove { this.innerServer.DocumentFormatted -= value; }
		}
		void IRichEditDocumentLayoutProvider.PerformPageSecondaryFormatting(Page page) {
			this.Formatter.PerformPageSecondaryFormatting(page);
		}
		#endregion
	}
	#endregion
	#region RichEditDocumentServerOptions
	public class RichEditDocumentServerOptions : RichEditControlOptionsBase {
		public RichEditDocumentServerOptions(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
			this.Import.Html.DefaultAsyncImageLoading = false;
			this.Import.Html.AsyncImageLoading = false;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region InnerRichEditDocumentServer
	public partial class InnerRichEditDocumentServer : IBatchUpdateable, IBatchUpdateHandler, IDisposable, IBoxMeasurerProvider, IServiceContainer, IRichEditDocumentServer {
		#region Fields
		readonly IInnerRichEditDocumentServerOwner owner;
		bool isDisposed;
		int threadId;
		BatchUpdateHelper batchUpdateHelper;
		PredefinedFontSizeCollection predefinedFontSizeCollection;
		DocumentModel existingDocumentModel;
		DocumentModel documentModel;
		DocumentModelAccessor documentModelAccessor;
		DocumentModel documentModelTemplate;
		RichEditControlOptionsBase options;
		MeasurementAndDrawingStrategy measurementAndDrawingStrategy;
		DocumentDeferredChanges documentDeferredChanges;
		DocumentDeferredChanges documentDeferredChangesOnIdle;
		DocumentUnit unit;
		NativeDocument nativeDocument;
		DevExpress.XtraRichEdit.API.Layout.DocumentLayout documentLayout;
		float dpiX;
		float dpiY;
		CalculationModeType layoutCalculationMode;
		BackgroundFormatter formatter;
		DocumentFormattingController formattingController;
		DocumentLayout modelDocumentLayout;
		Graphics graphics;
		BoxMeasurer measurer;
		#endregion
		public InnerRichEditDocumentServer(IInnerRichEditDocumentServerOwner owner)
			: this(owner, DocumentModelDpi.DpiX, DocumentModelDpi.DpiY) {
		}
		internal InnerRichEditDocumentServer(IInnerRichEditDocumentServerOwner owner, float dpiX, float dpiY) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
			this.dpiX = dpiX;
			this.dpiY = dpiY;
			this.layoutCalculationMode = GetDefaultLayoutCalculationMode();
		}
		public InnerRichEditDocumentServer(IInnerRichEditDocumentServerOwner owner, DocumentModel documentModel) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(owner, "documentModel");
			this.owner = owner;
			this.existingDocumentModel = documentModel;
			this.dpiX = documentModel.ScreenDpiX;
			this.dpiY = documentModel.ScreenDpiY;
			this.layoutCalculationMode = GetDefaultLayoutCalculationMode();
		}
		#region Properties
		public IInnerRichEditDocumentServerOwner Owner { get { return owner; } }
		internal bool IsDisposed { get { return isDisposed; } }
		bool IRichEditDocumentServer.IsDisposed { get { return isDisposed; } }
		public float DpiX { get { return dpiX; } }
		public float DpiY { get { return dpiY; } }
		internal BackgroundFormatter BackgroundFormatter { get { return formatter; } set { formatter = value; } }
		public virtual DocumentFormattingController FormattingController { get { return formattingController; } }
		public virtual DocumentLayout ModelDocumentLayout { get { return modelDocumentLayout; } }
		public CalculationModeType LayoutCalculationMode {
			get {
				return layoutCalculationMode;
			}
			set {
				if (layoutCalculationMode != value) {
					layoutCalculationMode = value;
					OnLayoutCalculationModeChanged();
				}
			}
		}
		private void OnLayoutCalculationModeChanged() {
			if (layoutCalculationMode == CalculationModeType.Automatic)
				InitializeBackgroundFormatter();
			else
				DisposeBackgroundFormatter();
		}
		protected virtual CalculationModeType GetDefaultLayoutCalculationMode() {
			return CalculationModeType.Manual;
		}
		internal virtual void InitializeBackgroundFormatter() {
#if !DXPORTABLE
			graphics = DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphicsWithoutAspCheck();
			if (DevExpress.Office.Drawing.PrecalculatedMetricsFontCacheManager.ShouldUse() || (DocumentModel.PrintingOptions.DrawLayoutFromSilverlightRendering && DocumentModel.ModelForExport))
				measurer = new PrecalculatedMetricsBoxMeasurer(DocumentModel);
			else
				measurer = new GdiBoxMeasurer(DocumentModel, graphics);
			modelDocumentLayout = new Layout.DocumentLayout(DocumentModel, new ExplicitBoxMeasurerProvider(measurer));
			formattingController = new PrintLayoutViewDocumentFormattingController(ModelDocumentLayout, DocumentModel.MainPieceTable);
			formatter = new BackgroundFormatter(formattingController, CommentPadding.GetDefaultCommentPadding(DocumentModel));
			formatter.PageFormattingComplete += OnPageFormattingComplete;
			formatter.Start();
#endif
		}
		protected internal virtual void InitializeEmptyDocumentModel(DocumentModel documentModel) {
		}
		internal DocumentLayout CalculatePrintDocumentLayout(InternalRichEditDocumentServer server) {
			if (ModelDocumentLayout == null)
				this.modelDocumentLayout = CalculatePrintDocumentLayoutCore();
			return ModelDocumentLayout;
		}
		protected virtual DocumentLayout CalculatePrintDocumentLayoutCore() {
			using (LayoutDocumentPrinter printer = new LayoutDocumentPrinter(documentModel, false)) {
				printer.PageFormattingComplete += OnPageFormattingComplete;
				printer.Format();
				printer.PageFormattingComplete -= OnPageFormattingComplete;
				return printer.DocumentLayout;
			}
		}
		protected void OnPageFormattingComplete(object sender, PageFormattingCompleteEventArgs e) {
			RaisePageFormatted(new API.Layout.PageFormattedEventArgs(e.Page.PageIndex));
			if (e.DocumentFormattingComplete)
				RaiseDocumentFormatted();
		}
		internal virtual void DisposeBackgroundFormatter() {
			if (formatter != null) {
				formatter.PageFormattingComplete -= OnPageFormattingComplete;
				formatter.Dispose();
				formatter = null;
			}
			formattingController = null;
			modelDocumentLayout = null;
			if (measurer != null) {
				measurer.Dispose();
				measurer = null;
			}
			if (graphics != null) {
				graphics.Dispose();
				graphics = null;
			}
		}
		protected virtual void OnDocumentLayoutChanged(object sender, DocumentUpdateCompleteEventArgs e) {
			if (ModelDocumentLayout == null)
				return;
			DocumentLayoutResetType documentLayoutResetType = CalculateDocumentLayoutResetType(e.DeferredChanges);
			if (LayoutCalculationMode == CalculationModeType.Automatic) {
				PieceTable pieceTable = e.DeferredChanges.ChangeStart.PieceTable;
				DocumentModelPosition invalidateFrom = NotifyDocumentLayoutChanged(pieceTable, e.DeferredChanges, documentLayoutResetType);
				if (invalidateFrom == null)
					return;
				int changedPageIndex = ModelDocumentLayout.Pages.BinarySearchBoxIndex(pieceTable, invalidateFrom.LogPosition);
				if (changedPageIndex < 0)
					changedPageIndex = ~changedPageIndex;
				if (documentLayoutResetType != DocumentLayoutResetType.None) {
					DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventArgs args = new API.Layout.DocumentLayoutInvalidatedEventArgs(changedPageIndex);
					RaiseDocumentLayoutInvalidated(args);
				}
			}
			else {
				if (documentLayoutResetType != DocumentLayoutResetType.None) {
					this.modelDocumentLayout = null;
					DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventArgs args = new API.Layout.DocumentLayoutInvalidatedEventArgs(0);
					RaiseDocumentLayoutInvalidated(args);
				}
			}
		}
		internal virtual DocumentModelPosition NotifyDocumentLayoutChanged(PieceTable pieceTable, DocumentModelDeferredChanges changes, DocumentLayoutResetType documentLayoutResetType) {
			ServerDocumentChangesHandler handler = new ServerDocumentChangesHandler(BackgroundFormatter);
			return handler.NotifyDocumentChanged(pieceTable, changes, false, documentLayoutResetType);
		}
		public static DocumentLayoutResetType CalculateDocumentLayoutResetType(DocumentModelDeferredChanges changes) {
			DocumentModelChangeActions changeActions = changes.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0)
				return DocumentLayoutResetType.AllPrimaryLayout;
			if ((changeActions & DocumentModelChangeActions.ResetPrimaryLayout) != 0)
				return DocumentLayoutResetType.PrimaryLayoutFormPosition;
			if ((changeActions & DocumentModelChangeActions.ResetSecondaryLayout) != 0)
				return DocumentLayoutResetType.PrimaryLayoutFormPosition;
			else
				return DocumentLayoutResetType.None;
		}
		DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventHandler documentLayoutInvalidated;
		public event DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventHandler DocumentLayoutInvalidated {
			add { documentLayoutInvalidated += value; }
			remove { documentLayoutInvalidated -= value; }
		}
		void RaiseDocumentLayoutInvalidated(DevExpress.XtraRichEdit.API.Layout.DocumentLayoutInvalidatedEventArgs args) {
			if (documentLayoutInvalidated != null)
				documentLayoutInvalidated(this, args);
		}
		EventHandler documentFormatted;
		public event EventHandler DocumentFormatted {
			add { documentFormatted += value; }
			remove { documentFormatted -= value; }
		}
		void RaiseDocumentFormatted() {
			if (documentFormatted != null)
				documentFormatted(this, EventArgs.Empty);
		}
		DevExpress.XtraRichEdit.API.Layout.PageFormattedEventHandler pageFormatted;
		public event DevExpress.XtraRichEdit.API.Layout.PageFormattedEventHandler PageFormatted {
			add { pageFormatted += value; }
			remove { pageFormatted -= value; }
		}
		void RaisePageFormatted(DevExpress.XtraRichEdit.API.Layout.PageFormattedEventArgs args) {
			if (pageFormatted != null)
				pageFormatted(this, args);
		}
		protected internal int ThreadId { get { return threadId; } }
		protected internal PredefinedFontSizeCollection PredefinedFontSizeCollection { get { return predefinedFontSizeCollection; } }
		[Browsable(false)]
		public DocumentModelAccessor Model { get { return documentModelAccessor; } }
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		protected internal DocumentModel DocumentModelTemplate {
			get {
				if (documentModelTemplate == null)
					CreateDocumentModelTemplate();
				return documentModelTemplate;
			}
		}
		protected internal DevExpress.XtraRichEdit.Model.AbstractNumberingListCollection NumberingListsTemplate { get { return DocumentModelTemplate.AbstractNumberingLists; } }
		public RichEditControlOptionsBase Options { get { return options; } }
		protected internal MeasurementAndDrawingStrategy MeasurementAndDrawingStrategy { get { return measurementAndDrawingStrategy; } }
		protected internal BoxMeasurer Measurer { get { return MeasurementAndDrawingStrategy != null ? MeasurementAndDrawingStrategy.Measurer : null; } }
		BoxMeasurer IBoxMeasurerProvider.Measurer { get { return this.Measurer; } }
		#region LayoutUnit
		public DocumentLayoutUnit LayoutUnit {
			get { return (DocumentLayoutUnit)DocumentModel.LayoutUnit; }
			set {
				if (value == LayoutUnit)
					return;
				SetLayoutUnit(value);
			}
		}
		#endregion
		#region Modified
		public bool Modified {
			get { return DocumentModel.InternalAPI.Modified; }
			set { DocumentModel.InternalAPI.Modified = value; }
		}
		#endregion
		#region ReadOnly
		public virtual bool ReadOnly { get { return false; } set { } }
		#endregion
		public virtual bool Enabled { get { return true; } }
		internal bool IsEditable { get { return !ReadOnly && Enabled; } }
		protected internal DocumentDeferredChanges DocumentDeferredChanges { get { return documentDeferredChanges; } }
		#region Unit
		public DocumentUnit Unit {
			get { return unit; }
			set {
				if (unit == value)
					return;
				SetUnit(value);
			}
		}
		#endregion
		protected internal DocumentUnit UIUnit {
			get {
				if (Unit == DocumentUnit.Document)
					return DocumentUnit.Inch;
				else
					return Unit;
			}
		}
		protected internal NativeDocument NativeDocument {
			get {
				if (nativeDocument == null)
					nativeDocument = CreateNativeDocument();
				return nativeDocument;
			}
		}
		protected internal virtual NativeDocument CreateNativeDocument() {
			return new NativeDocument(DocumentModel.MainPieceTable, this);
		}
		public virtual ISpellChecker SpellChecker { get { return null; } set { } }
		protected internal virtual bool ActualReadOnly { get { return false; } }
		#region Text
		public string Text {
			get {
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
		#endregion
		#region RtfText
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
		#region OpenXmlBytes
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
		public Document Document { get { return NativeDocument; } }
		#endregion
		#region DocumentLayout
		[Browsable(false)]
		public DevExpress.XtraRichEdit.API.Layout.DocumentLayout DocumentLayout {
			get {
				InitializeDocumentLayout();
				return documentLayout;
			}
		}
		protected void InitializeDocumentLayout() {
			if (documentLayout == null)
				documentLayout = CreateDocumentLayout();
		}
		protected virtual DevExpress.XtraRichEdit.API.Layout.DocumentLayout CreateDocumentLayout() {
			IRichEditDocumentLayoutProvider layoutProvider = Owner as IRichEditDocumentLayoutProvider;
			return layoutProvider != null ? new DevExpress.XtraRichEdit.API.Layout.DocumentLayout(layoutProvider) : null;
		}
		#endregion
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static InternalRichEditDocumentServer CreateServer(DocumentFormatsDependencies documentFormatsDependencies) {
			return new InternalRichEditDocumentServer(documentFormatsDependencies);
		}
		#region Events
		#region ContentChanged
		EventHandler onContentChanged;
		public event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged(bool suppressBindingNotifications) {
			if (onContentChanged != null)
				onContentChanged(Owner, new DocumentContentChangedEventArgs(suppressBindingNotifications));
		}
		#endregion
		#region PlainTextChanged
		EventHandler onPlainTextChanged;
		public event EventHandler PlainTextChanged { add { onPlainTextChanged += value; } remove { onPlainTextChanged -= value; } }
		protected internal virtual void RaisePlainTextChanged() {
			if (onPlainTextChanged != null)
				onPlainTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region RtfTextChanged
		EventHandler onRtfTextChanged;
		public event EventHandler RtfTextChanged { add { onRtfTextChanged += value; } remove { onRtfTextChanged -= value; } }
		protected internal virtual void RaiseRtfTextChanged() {
			if (onRtfTextChanged != null)
				onRtfTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region HtmlTextChanged
		EventHandler onHtmlTextChanged;
		public event EventHandler HtmlTextChanged { add { onHtmlTextChanged += value; } remove { onHtmlTextChanged -= value; } }
		protected internal virtual void RaiseHtmlTextChanged() {
			if (onHtmlTextChanged != null)
				onHtmlTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region MhtTextChanged
		EventHandler onMhtTextChanged;
		public event EventHandler MhtTextChanged { add { onMhtTextChanged += value; } remove { onMhtTextChanged -= value; } }
		protected internal virtual void RaiseMhtTextChanged() {
			if (onMhtTextChanged != null)
				onMhtTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region WordMLTextChanged
		EventHandler onWordMLTextChanged;
		public event EventHandler WordMLTextChanged { add { onWordMLTextChanged += value; } remove { onWordMLTextChanged -= value; } }
		protected internal virtual void RaiseWordMLTextChanged() {
			if (onWordMLTextChanged != null)
				onWordMLTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region XamlTextChanged
		EventHandler onXamlTextChanged;
		public event EventHandler XamlTextChanged { add { onXamlTextChanged += value; } remove { onXamlTextChanged -= value; } }
		protected internal virtual void RaiseXamlTextChanged() {
			if (onXamlTextChanged != null)
				onXamlTextChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region OpenXmlBytesChanged
		EventHandler onOpenXmlBytesChanged;
		public event EventHandler OpenXmlBytesChanged { add { onOpenXmlBytesChanged += value; } remove { onOpenXmlBytesChanged -= value; } }
		protected internal virtual void RaiseOpenXmlBytesChanged() {
			if (onOpenXmlBytesChanged != null)
				onOpenXmlBytesChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region OpenDocumentBytesChanged
		EventHandler onOpenDocumentBytesChanged;
		public event EventHandler OpenDocumentBytesChanged { add { onOpenDocumentBytesChanged += value; } remove { onOpenDocumentBytesChanged -= value; } }
		protected internal virtual void RaiseOpenDocumentBytesChanged() {
			if (onOpenDocumentBytesChanged != null)
				onOpenDocumentBytesChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region UnitChanging
		EventHandler onUnitChanging;
		event EventHandler IRichEditDocumentServer.UnitChanging { add { onUnitChanging += value; } remove { onUnitChanging -= value; } }
		protected internal virtual void RaiseUnitChanging() {
			if (onUnitChanging != null)
				onUnitChanging(Owner, EventArgs.Empty);
		}
		#endregion
		#region UnitChanged
		EventHandler onUnitChanged;
		event EventHandler IRichEditDocumentServer.UnitChanged { add { onUnitChanged += value; } remove { onUnitChanged -= value; } }
		protected internal virtual void RaiseUnitChanged() {
			if (onUnitChanged != null)
				onUnitChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region CalculateDocumentVariable
		CalculateDocumentVariableEventHandler onCalculateDocumentVariable;
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable { add { onCalculateDocumentVariable += value; } remove { onCalculateDocumentVariable -= value; } }
		protected internal virtual bool RaiseCalculateDocumentVariable(CalculateDocumentVariableEventArgs args) {
			if (onCalculateDocumentVariable != null) {
				onCalculateDocumentVariable(Owner, args);
				return args.Handled;
			}
			else
				return false;
		}
		#endregion
		#region BeforeImport
		BeforeImportEventHandler onBeforeImport;
		public event BeforeImportEventHandler BeforeImport { add { onBeforeImport += value; } remove { onBeforeImport -= value; } }
		protected internal virtual void RaiseBeforeImport(BeforeImportEventArgs args) {
			if (onBeforeImport != null)
				onBeforeImport(Owner, args);
		}
		#endregion
		#region BeforeExport
		BeforeExportEventHandler onBeforeExport;
		public event BeforeExportEventHandler BeforeExport { add { onBeforeExport += value; } remove { onBeforeExport -= value; } }
		protected internal virtual void RaiseBeforeExport(BeforeExportEventArgs args) {
			if (onBeforeExport != null)
				onBeforeExport(Owner, args);
		}
		#endregion
		EventHandler onAfterExport;
		public event EventHandler AfterExport { add { onAfterExport += value; } remove { onAfterExport -= value; } }
		protected internal virtual void RaiseAfterExport() {
			if (onAfterExport != null)
				onAfterExport(Owner, EventArgs.Empty);
		}
		#region InitializeDocument
		EventHandler onInitializeDocument;
		public event EventHandler InitializeDocument { add { onInitializeDocument += value; } remove { onInitializeDocument -= value; } }
		protected internal virtual void RaiseInitializeDocument(EventArgs args) {
			if (onInitializeDocument != null)
				onInitializeDocument(Owner, args);
		}
		#endregion
		#region InvalidFormatException
		RichEditInvalidFormatExceptionEventHandler onInvalidFormatException;
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException { add { onInvalidFormatException += value; } remove { onInvalidFormatException -= value; } }
		protected internal virtual void RaiseInvalidFormatException(Exception e) {
			if (onInvalidFormatException != null) {
				RichEditInvalidFormatExceptionEventArgs args = new RichEditInvalidFormatExceptionEventArgs(e);
				onInvalidFormatException(Owner, args);
			}
		}
		#endregion
		#region UnhandledException
		RichEditUnhandledExceptionEventHandler onUnhandledException;
		public event RichEditUnhandledExceptionEventHandler UnhandledException { add { onUnhandledException += value; } remove { onUnhandledException -= value; } }
		protected internal virtual bool RaiseUnhandledException(Exception e) {
			try {
				if (onUnhandledException != null) {
					RichEditUnhandledExceptionEventArgs args = new RichEditUnhandledExceptionEventArgs(e);
					onUnhandledException(Owner, args);
					return args.Handled;
				}
				else
					return false;
			}
			catch {
				return false;
			}
		}
		#endregion
		#region SelectionChanged
		EventHandler onSelectionChanged;
		public event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region StartHeaderFooterEditing
		HeaderFooterEditingEventHandler onStartHeaderFooterEditing;
		public event HeaderFooterEditingEventHandler StartHeaderFooterEditing { add { onStartHeaderFooterEditing += value; } remove { onStartHeaderFooterEditing -= value; } }
		protected internal virtual void RaiseStartHeaderFooterEditing() {
			if (onStartHeaderFooterEditing != null)
				onStartHeaderFooterEditing(Owner, new HeaderFooterEditingEventArgs());
		}
		#endregion
		#region FinishHeaderFooterEditing
		HeaderFooterEditingEventHandler onFinishHeaderFooterEditing;
		public event HeaderFooterEditingEventHandler FinishHeaderFooterEditing { add { onFinishHeaderFooterEditing += value; } remove { onFinishHeaderFooterEditing -= value; } }
		protected internal virtual void RaiseFinishHeaderFooterEditing() {
			if (onFinishHeaderFooterEditing != null)
				onFinishHeaderFooterEditing(Owner, new HeaderFooterEditingEventArgs());
		}
		#endregion
		#region DocumentProtectionChanged
		EventHandler onDocumentProtectionChanged;
		public event EventHandler DocumentProtectionChanged { add { onDocumentProtectionChanged += value; } remove { onDocumentProtectionChanged -= value; } }
		protected internal virtual void RaiseDocumentProtectionChanged() {
			if (onDocumentProtectionChanged != null)
				onDocumentProtectionChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ActiveViewChanged
		EventHandler onActiveViewChanged;
		public event EventHandler ActiveViewChanged { add { onActiveViewChanged += value; } remove { onActiveViewChanged -= value; } }
		protected internal virtual void RaiseActiveViewChanged() {
			if (onActiveViewChanged != null)
				onActiveViewChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ActiveRecordChanged
		EventHandler onActiveRecordChanged;
		public event EventHandler ActiveRecordChanged { add { onActiveRecordChanged += value; } remove { onActiveRecordChanged -= value; } }
		protected internal virtual void RaiseActiveRecordChanged() {
			if (onActiveRecordChanged != null)
				onActiveRecordChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ActiveRecordChanging
		EventHandler onActiveRecordChanging;
		public event EventHandler ActiveRecordChanging { add { onActiveRecordChanging += value; } remove { onActiveRecordChanging -= value; } }
		protected internal virtual void RaiseActiveRecordChanging() {
			if (onActiveRecordChanging != null)
				onActiveRecordChanging(Owner, EventArgs.Empty);
		}
		#endregion
		#region DocumentLoaded
		EventHandler onDocumentLoaded;
		public event EventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal virtual void RaiseDocumentLoaded() {
			if (onDocumentLoaded != null)
				onDocumentLoaded(Owner, EventArgs.Empty);
		}
		#endregion
		#region EmptyDocumentCreated
		EventHandler onEmptyDocumentCreated;
		public event EventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal virtual void RaiseEmptyDocumentCreated() {
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(Owner, EventArgs.Empty);
		}
		#endregion
		#region ClipboardSetDataException
		RichEditClipboardSetDataExceptionEventHandler onClipboardSetDataException;
		public event RichEditClipboardSetDataExceptionEventHandler ClipboardSetDataException { add { onClipboardSetDataException += value; } remove { onClipboardSetDataException -= value; } }
		protected internal virtual void RaiseClipboardSetDataException(Exception e) {
			if (onClipboardSetDataException != null) {
				RichEditClipboardSetDataExceptionEventArgs args = new RichEditClipboardSetDataExceptionEventArgs(e);
				onClipboardSetDataException(Owner, args);
			}
		}
		#endregion
		#region UpdateUI
		EventHandler onUpdateUI;
		public event EventHandler UpdateUI { add { onUpdateUI += value; } remove { onUpdateUI -= value; } }
		protected internal virtual void RaiseUpdateUI() {
			if (onUpdateUI != null)
				onUpdateUI(Owner, EventArgs.Empty);
		}
		#endregion
		#region DocumentClosing
		CancelEventHandler onDocumentClosing;
		public event CancelEventHandler DocumentClosing { add { onDocumentClosing += value; } remove { onDocumentClosing -= value; } }
		protected internal virtual bool RaiseDocumentClosing() {
			if (onDocumentClosing != null) {
				CancelEventArgs args = new CancelEventArgs();
				onDocumentClosing(Owner, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region AutoCorrect
		AutoCorrectEventHandler onAutoCorrect;
		public event AutoCorrectEventHandler AutoCorrect { add { onAutoCorrect += value; } remove { onAutoCorrect -= value; } }
		protected internal virtual AutoCorrectInfo RaiseAutoCorrect() {
			if (onAutoCorrect != null) {
				AutoCorrectEventArgs args = new AutoCorrectEventArgs(this);
				onAutoCorrect(Owner, args);
				return args.AutoCorrectInfo;
			}
			else
				return null;
		}
		#endregion
		#region CustomizeMergeFields
		CustomizeMergeFieldsEventHandler onCustomizeMergeFields;
		public event CustomizeMergeFieldsEventHandler CustomizeMergeFields { add { onCustomizeMergeFields += value; } remove { onCustomizeMergeFields -= value; } }
		protected internal virtual MergeFieldName[] RaiseCustomizeMergeFields(CustomizeMergeFieldsEventArgs args) {
			if (onCustomizeMergeFields != null)
				onCustomizeMergeFields(Owner, args);
			return args.MergeFieldsNames;
		}
		#endregion
		#region MailMergeStarted
		MailMergeStartedEventHandler onMailMergeStarted;
		public event MailMergeStartedEventHandler MailMergeStarted { add { onMailMergeStarted += value; } remove { onMailMergeStarted -= value; } }
		protected internal virtual void RaiseMailMergeStarted(MailMergeStartedEventArgs args) {
			if (onMailMergeStarted != null)
				onMailMergeStarted(Owner, args);
		}
		#endregion
		#region MailMergeRecordStarted
		MailMergeRecordStartedEventHandler onMailMergeRecordStarted;
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted { add { onMailMergeRecordStarted += value; } remove { onMailMergeRecordStarted -= value; } }
		protected internal virtual void RaiseMailMergeRecordStarted(MailMergeRecordStartedEventArgs args) {
			if (onMailMergeRecordStarted != null)
				onMailMergeRecordStarted(Owner, args);
		}
		#endregion
		#region MailMergeRecordFinished
		MailMergeRecordFinishedEventHandler onMailMergeRecordFinished;
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished { add { onMailMergeRecordFinished += value; } remove { onMailMergeRecordFinished -= value; } }
		protected internal virtual void RaiseMailMergeRecordFinished(MailMergeRecordFinishedEventArgs args) {
			if (onMailMergeRecordFinished != null)
				onMailMergeRecordFinished(Owner, args);
		}
		#endregion
		#region MailMergeFinished
		MailMergeFinishedEventHandler onMailMergeFinished;
		public event MailMergeFinishedEventHandler MailMergeFinished { add { onMailMergeFinished += value; } remove { onMailMergeFinished -= value; } }
		protected internal virtual void RaiseMailMergeFinished(MailMergeFinishedEventArgs args) {
			if (onMailMergeFinished != null)
				onMailMergeFinished(Owner, args);
		}
		#endregion
		#region PageBackgroundChanged
		EventHandler onPageBackgroundChanged;
		internal event EventHandler PageBackgroundChanged { add { onPageBackgroundChanged += value; } remove { onPageBackgroundChanged -= value; } }
		protected internal virtual void RaisePageBackgroundChanged() {
			if (onPageBackgroundChanged != null)
				onPageBackgroundChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region BeforePagePaint
		BeforePagePaintEventHandler onBeforePagePaint;
		internal event BeforePagePaintEventHandler BeforePagePaint { add { onBeforePagePaint += value; } remove { onBeforePagePaint -= value; } }
		internal virtual void RaiseBeforePagePaint(BeforePagePaintEventArgs args) {
			if (onBeforePagePaint != null)
				onBeforePagePaint(Owner, args);
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				DisposeBackgroundFormatter();
				if (documentLayout != null) {
					documentLayout.Dispose();
					documentLayout = null;
				}
				if (options != null) {
					UnsubscribeOptionsEvents();
					options.Dispose();
					options = null;
				}
				if (documentModel != null) {
					UnsubscribeDocumentModelEvents();
					if (documentModel != existingDocumentModel)
						documentModel.Dispose();
					documentModel = null;
					documentModelAccessor = null;
				}
				if (measurementAndDrawingStrategy != null) {
					measurementAndDrawingStrategy.Dispose();
					measurementAndDrawingStrategy = null;
				}
				this.predefinedFontSizeCollection = null;
				this.isDisposed = true;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		public virtual void BeginInitialize() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
			this.predefinedFontSizeCollection = new PredefinedFontSizeCollection();
			this.documentModel = GetDocumentModel();
			this.documentModelAccessor = new DocumentModelAccessor(documentModel);
			SubscribeDocumentModelEvents();
			this.options = CreateOptions();
		}
		public virtual void EndInitialize() {
			UnsubscribeOptionsEvents();
			SubscribeOptionsEvents();
			CreateNewMeasurementAndDrawingStrategy();
			AddServices();
		}
		void AddServices() {
			AddService(typeof(IDocumentLayoutService), new DocumentServerLayoutService(this));
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdateCore();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastEndUpdateCore();
		}
		#endregion
		protected internal virtual void CreateDocumentModelTemplate() {
			this.documentModelTemplate = GetDocumentModel();
			documentModelTemplate.BeginSetContent();
			try {
				DefaultNumberingListHelper.InsertNumberingLists(documentModelTemplate, DocumentModel.UnitConverter, DocumentModel.DocumentProperties.DefaultTabWidth);
			}
			finally {
				documentModelTemplate.EndSetContent(DocumentModelChangeType.None, false, null);
			}
		}
		protected internal virtual void OnFirstBeginUpdateCore() {
		}
		protected internal virtual void OnLastEndUpdateCore() {
		}
		#region DocumentModel creation and event handling
		protected internal virtual DocumentModel GetDocumentModel() {
			if (existingDocumentModel != null)
				return existingDocumentModel;
			return CreateDocumentModelCore();
		}
		protected virtual DocumentModel CreateDocumentModelCore() {
			return new DocumentModel(owner.DocumentFormatsDependencies, DpiX, DpiY);
		}
		protected internal virtual void SubscribeDocumentModelEvents() {
			DocumentModel.BeginDocumentUpdate += OnBeginDocumentUpdate;
			DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
			DocumentModel.InnerSelectionChanged += OnInnerSelectionChanged;
			DocumentModel.SelectionChanged += OnSelectionChanged;
			DocumentModel.InnerContentChanged += OnInnerContentChanged;
			DocumentModel.ContentChanged += OnContentChanged;
			DocumentModel.ModifiedChanged += OnModifiedChanged;
			DocumentModel.BeforeExport += OnBeforeExport;
			DocumentModel.AfterExport += OnAfterExport;
			DocumentModel.BeforeImport += OnBeforeImport;
			DocumentModel.DocumentCleared += OnDocumentCleared;
			DocumentModel.InvalidFormatException += OnInvalidFormatException;
			DocumentModel.CalculateDocumentVariable += OnCalculateDocumentVariable;
			DocumentModel.MailMergeStarted += OnMailMergeStarted;
			DocumentModel.MailMergeRecordStarted += OnMailMergeRecordStarted;
			DocumentModel.MailMergeRecordFinished += OnMailMergeRecordFinished;
			DocumentModel.MailMergeFinished += OnMailMergeFinished;
			DocumentModel.PageBackgroundChanged += OnPageBackgroundChanged;
			DocumentModel.CommentInserted += OnCommentInserted;
		}
		private void OnCommentInserted(object sender, CommentEventArgs e) {
			RaiseCommentInserted(e);
		}
		public event CommentEventHandler CommentInserted;
		void RaiseCommentInserted(CommentEventArgs e) {
			if (CommentInserted != null)
				CommentInserted(this, e);
		}
		protected internal virtual void UnsubscribeDocumentModelEvents() {
			DocumentModel.BeginDocumentUpdate -= OnBeginDocumentUpdate;
			DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
			DocumentModel.InnerSelectionChanged -= OnInnerSelectionChanged;
			DocumentModel.SelectionChanged -= OnSelectionChanged;
			DocumentModel.InnerContentChanged -= OnInnerContentChanged;
			DocumentModel.ContentChanged -= OnContentChanged;
			DocumentModel.ModifiedChanged -= OnModifiedChanged;
			DocumentModel.BeforeExport -= OnBeforeExport;
			DocumentModel.AfterExport -= OnAfterExport;
			DocumentModel.BeforeImport -= OnBeforeImport;
			DocumentModel.DocumentCleared -= OnDocumentCleared;
			DocumentModel.InvalidFormatException -= OnInvalidFormatException;
			DocumentModel.CalculateDocumentVariable -= OnCalculateDocumentVariable;
			DocumentModel.MailMergeStarted -= OnMailMergeStarted;
			DocumentModel.MailMergeRecordStarted -= OnMailMergeRecordStarted;
			DocumentModel.MailMergeRecordFinished -= OnMailMergeRecordFinished;
			DocumentModel.MailMergeFinished -= OnMailMergeFinished;
			DocumentModel.PageBackgroundChanged -= OnPageBackgroundChanged;
			DocumentModel.CommentInserted -= OnCommentInserted;
		}
		protected internal virtual void OnSelectionChanged(object sender, EventArgs e) {
			RaiseSelectionChanged();
			OnUpdateUI();
		}
		protected internal virtual void OnInnerSelectionChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnBeginDocumentUpdate(object sender, EventArgs e) {
			this.documentDeferredChanges = new DocumentDeferredChanges();
			BeginUpdate();
			OnBeginDocumentUpdateCore();
		}
		protected internal virtual void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			OnEndDocumentUpdateCore(sender, e);
			EndUpdate();
			this.documentDeferredChanges = null;
		}
		protected internal virtual void OnBeginDocumentUpdateCore() {
		}
		protected internal virtual DocumentModelChangeActions ProcessEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = e.DeferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.PerformActionsOnIdle) != 0) {
				if (documentDeferredChangesOnIdle == null)
					this.documentDeferredChangesOnIdle = new DocumentDeferredChanges();
				documentDeferredChangesOnIdle.ChangeActions |= changeActions;
				documentDeferredChangesOnIdle.ChangeActions &= ~DocumentModelChangeActions.PerformActionsOnIdle;
				documentDeferredChangesOnIdle.StartRunIndex = Algorithms.Min(e.DeferredChanges.ChangeStart.RunIndex, documentDeferredChangesOnIdle.StartRunIndex);
				documentDeferredChangesOnIdle.EndRunIndex = Algorithms.Max(e.DeferredChanges.ChangeEnd.RunIndex, documentDeferredChangesOnIdle.EndRunIndex);
#if !SL
				changeActions = DocumentModelChangeActions.None;
#endif
			}
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0 ||
				(changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0)
				ApplyFontAndForeColor();
			return changeActions;
		}
		protected internal virtual DocumentModelChangeActions OnEndDocumentUpdateCore(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = ProcessEndDocumentUpdateCore(sender, e);
			OnDocumentLayoutChanged(this, e);
			ApplyChangesCore(changeActions);
			return changeActions;
		}
		protected internal virtual void OnInnerContentChanged(object sender, EventArgs e) {
		}
		protected internal virtual void OnContentChanged(object sender, EventArgs e) {
			OnContentChangedCore(false, false);
		}
		protected internal virtual void OnContentChangedCore(bool suppressBindingNotifications, bool suppressUpdateUI) {
			RaiseContentChanged(suppressBindingNotifications);
			if (!suppressBindingNotifications)
				RaiseBindingNotifications();
			if (!suppressUpdateUI)
				OnUpdateUI();
		}
		protected internal virtual void RaiseBindingNotifications() {
			RaiseRtfTextChanged();
			RaiseHtmlTextChanged();
			RaiseMhtTextChanged();
			RaiseWordMLTextChanged();
			RaiseXamlTextChanged();
			RaiseOpenXmlBytesChanged();
			RaiseOpenDocumentBytesChanged();
			RaisePlainTextChanged();
		}
		protected internal virtual void OnModifiedChanged(object sender, EventArgs e) {
			RaiseModifiedChanged();
			OnUpdateUI();
		}
		protected internal virtual void OnCalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e) {
			RaiseCalculateDocumentVariable(e);
		}
		protected internal virtual void OnMailMergeStarted(object sender, MailMergeStartedEventArgs e) {
			RaiseMailMergeStarted(e);
		}
		protected internal virtual void OnMailMergeRecordStarted(object sender, MailMergeRecordStartedEventArgs e) {
			RaiseMailMergeRecordStarted(e);
		}
		protected internal virtual void OnMailMergeRecordFinished(object sender, MailMergeRecordFinishedEventArgs e) {
			RaiseMailMergeRecordFinished(e);
		}
		protected internal virtual void OnMailMergeFinished(object sender, MailMergeFinishedEventArgs e) {
			RaiseMailMergeFinished(e);
		}
		protected internal virtual void OnPageBackgroundChanged(object sender, EventArgs e) {
			RaisePageBackgroundChanged();
		}
		protected internal virtual void OnBeforeExport(object sender, BeforeExportEventArgs e) {
			RaiseBeforeExport(e);
		}
		protected internal virtual void OnAfterExport(object sender, EventArgs e) {
			RaiseAfterExport();
		}
		protected internal virtual void OnBeforeImport(object sender, BeforeImportEventArgs e) {
			RaiseBeforeImport(e);
		}
		protected internal virtual void OnDocumentCleared(object sender, EventArgs e) {
			RaiseInitializeDocument(e);
		}
		protected internal virtual void OnInvalidFormatException(object sender, RichEditInvalidFormatExceptionEventArgs e) {
			RaiseInvalidFormatException(e.Exception);
		}
		protected internal virtual void OnEmptyDocumentCreated() {
			DocumentModel.SwitchToEmptyHistory(false);
			try {
				ApplyFontAndForeColor();
				RaiseEmptyDocumentCreated();
			}
			finally {
				DocumentModel.SwitchToNormalHistory(false);
			}
		}
		protected internal virtual void OnDocumentLoaded() {
			DocumentModel.SwitchToEmptyHistory(false);
			try {
				ApplyFontAndForeColor();
				RaiseDocumentLoaded();
			}
			finally {
				DocumentModel.SwitchToNormalHistory(false);
			}
		}
		protected internal virtual void OnActivePieceTableChanged(object sender, EventArgs e) {
			if (DocumentModel.ActivePieceTable.IsHeaderFooter)
				RaiseStartHeaderFooterEditing();
			else
				RaiseFinishHeaderFooterEditing();
			OnUpdateUI();
		}
		protected internal virtual void ApplyFontAndForeColor() {
		}
		protected internal virtual void ApplyChangesCore(DocumentModelChangeActions changeActions) {
			if (DocumentModel.IsUpdateLocked)
				DocumentDeferredChanges.ChangeActions |= changeActions;
			else
				RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void PerformRaiseDeferredEventsCore(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0)
				OnEmptyDocumentCreated();
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentLoaded) != 0)
				OnDocumentLoaded();
			if ((changeActions & DocumentModelChangeActions.RaiseContentChanged) != 0) {
				bool suppressBindingNotifications = (changeActions & DocumentModelChangeActions.SuppressBindingsNotifications) != 0;
				bool suppressUpdateUI = (changeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0;
				OnContentChangedCore(suppressBindingNotifications, suppressUpdateUI);
			}
			if ((changeActions & DocumentModelChangeActions.RaiseModifiedChanged) != 0)
				OnModifiedChanged(this, EventArgs.Empty);
			if ((changeActions & DocumentModelChangeActions.ActivePieceTableChanged) != 0)
				OnActivePieceTableChanged(this, EventArgs.Empty);
			if ((changeActions & DocumentModelChangeActions.RaiseSelectionChanged) != 0)
				OnSelectionChanged(this, EventArgs.Empty);
			if ((changeActions & DocumentModelChangeActions.RaiseDocumentProtectionChanged) != 0)
				RaiseDocumentProtectionChanged();
		}
		protected internal virtual void RaiseDeferredEventsCore(DocumentModelChangeActions changeActions) {
			lock (this) {
				if (IsDisposed)
					return;
				PerformRaiseDeferredEventsCore(changeActions);
			}
		}
		#endregion
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			Owner.RaiseDeferredEvents(changeActions);
		}
		protected internal virtual MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy() {
			return Owner.CreateMeasurementAndDrawingStrategy(DocumentModel);
		}
		protected internal virtual void CreateNewMeasurementAndDrawingStrategy() {
			this.measurementAndDrawingStrategy = CreateMeasurementAndDrawingStrategy();
			this.measurementAndDrawingStrategy.Initialize();
			DocumentModel.SetFontCacheManager(measurementAndDrawingStrategy.CreateFontCacheManager());
		}
		protected internal virtual void RecreateMeasurementAndDrawingStrategy() {
			BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					if (measurementAndDrawingStrategy != null)
						measurementAndDrawingStrategy.Dispose();
					CreateNewMeasurementAndDrawingStrategy();
					PieceTable pieceTable = DocumentModel.MainPieceTable;
					DocumentModelChangeActions changeActions = DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetAllPrimaryLayout;
					DocumentModel.ApplyChangesCore(pieceTable, changeActions, RunIndex.Zero, RunIndex.MaxValue);
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				EndUpdate();
			}
		}
		#region Options creation and event handling
		protected internal virtual RichEditControlOptionsBase CreateOptions() {
			return Owner.CreateOptions(this);
		}
		protected internal virtual void SubscribeOptionsEvents() {
			Options.Changed += OnOptionsChanged;
		}
		protected internal virtual void UnsubscribeOptionsEvents() {
			Options.Changed -= OnOptionsChanged;
		}
		protected internal virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "ActiveRecord")
				RaiseActiveRecordChanging();
			if (e.Name == "DataSource" || e.Name == "DataMember" || e.Name == "ViewMergedData" || e.Name == "ActiveRecord")
				OnOptionsMailMergeChanged();
			if (e.Name == "ActiveRecord")
				RaiseActiveRecordChanged();
#if !SL
			if (e.Name == "DrawLayoutFromSilverlightRendering") {
				DocumentModel.PrintingOptions.DrawLayoutFromSilverlightRendering = Options.Printing.DrawLayoutFromSilverlightRendering;
				if (DocumentModel.ModelForExport)
					RecreateMeasurementAndDrawingStrategy();
			}
#endif
		}
		#endregion
		protected virtual void OnOptionsMailMergeChanged() {
			BeginUpdate();
			try {
				DocumentModel.BeginUpdate();
				try {
					RichEditMailMergeOptions options = Options.MailMerge;
					DocumentModel.MailMergeProperties.ActiveRecord = options.ActiveRecord;
					DocumentModel.MailMergeProperties.ViewMergedData = options.ViewMergedData;
					DocumentModel.MailMergeDataController.DataSource = options.DataSource;
					DocumentModel.MailMergeDataController.DataMember = options.DataMember;
				}
				finally {
					DocumentModel.EndUpdate();
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetUnit(DocumentUnit value) {
			RaiseUnitChanging();
			this.unit = value;
			if (this.nativeDocument != null)
				this.nativeDocument.OnUnitsChanged();
			RaiseUnitChanged();
		}
		#region SetLayoutUnit
		protected internal virtual void SetLayoutUnit(DocumentLayoutUnit unit) {
			BeginUpdate();
			try {
				SetLayoutUnitCore(unit);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void SetLayoutUnitCore(DocumentLayoutUnit unit) {
			SetDocumentModelLayoutUnit(unit);
		}
		protected internal virtual void SetDocumentModelLayoutUnit(DocumentLayoutUnit unit) {
			DocumentModel.BeginUpdate();
			try {
				SetDocumentModelLayoutUnitCore(unit);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SetDocumentModelLayoutUnitCore(DocumentLayoutUnit unit) {
			DocumentModel.LayoutUnit = (DevExpress.Office.DocumentLayoutUnit)unit;
			RecreateMeasurementAndDrawingStrategy();
			MeasurementAndDrawingStrategy.OnLayoutUnitChanged();
		}
		#endregion
		protected internal virtual bool CanCloseExistingDocument() {
			if (CanCloseExistingDocumentCore())
				return true;
			return RaiseDocumentClosing();
		}
		protected internal virtual bool CanCloseExistingDocumentCore() {
			return !Modified;
		}
		protected internal virtual void OnApplicationIdle() {
			if (documentDeferredChangesOnIdle != null) {
				DocumentModel.BeginUpdate();
				try {
					DocumentModel.MainPieceTable.ApplyChangesCore(documentDeferredChangesOnIdle.ChangeActions, documentDeferredChangesOnIdle.StartRunIndex, documentDeferredChangesOnIdle.EndRunIndex);
				}
				finally {
					DocumentModel.EndUpdate();
				}
				documentDeferredChangesOnIdle = null;
			}
		}
		#region Load/Save content
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026")]
		public virtual bool CreateNewDocument(bool raiseDocumentClosing = false) {
			if (Modified)
				if (raiseDocumentClosing && !RaiseDocumentClosing())
					return false;
			DocumentModel.InternalAPI.CreateNewDocument();
			return true;
		}
		public virtual void LoadDocument(Stream stream, DocumentFormat documentFormat) {
			LoadDocumentCore(stream, documentFormat, String.Empty);
		}
		public virtual void LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat) {
			LoadDocumentCore(stream, documentFormat, String.Empty);
		}
		protected internal virtual void LoadDocumentCore(Stream stream, DocumentFormat documentFormat, string sourceUri) {
			DocumentModel.LoadDocument(stream, documentFormat, sourceUri);
		}
		public virtual void LoadDocument(string fileName) {
			LoadDocument(fileName, DocumentFormat.Undefined);
		}
		public virtual void LoadDocument(string fileName, DocumentFormat documentFormat) {
			LoadDocument(fileName, documentFormat, false);
		}
		public virtual void LoadDocumentTemplate(string fileName) {
			LoadDocumentTemplate(fileName, DocumentFormat.Undefined);
		}
		public virtual void LoadDocumentTemplate(string fileName, DocumentFormat documentFormat) {
			LoadDocument(fileName, documentFormat, true);
		}
		protected internal virtual void LoadDocument(string fileName, DocumentFormat documentFormat, bool loadAsTemplate) {
			using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				if (documentFormat == DocumentFormat.Undefined)
					documentFormat = DocumentModel.AutodetectDocumentFormat(fileName);
				DocumentSaveOptions saveOptions = DocumentModel.DocumentSaveOptions;
				string previousFileName = saveOptions.CurrentFileName;
				DocumentFormat previousDocumentFormat = saveOptions.CurrentFormat;
				try {
					if (!loadAsTemplate) {
						saveOptions.CurrentFileName = fileName;
						saveOptions.CurrentFormat = documentFormat;
					}
					LoadDocumentCore(stream, documentFormat, fileName);
				}
				catch {
					saveOptions.CurrentFileName = previousFileName;
					saveOptions.CurrentFormat = previousDocumentFormat;
					throw;
				}
			}
		}
		public virtual void SaveDocument(Stream stream, DocumentFormat documentFormat) {
			SaveDocumentCore(stream, documentFormat, String.Empty);
		}
		public virtual void SaveDocument(string fileName, DocumentFormat documentFormat) {
			SaveDocumentCore(fileName, documentFormat, DocumentModel.DocumentSaveOptions);
		}
		internal void SaveDocument(string fileName, DocumentFormat documentFormat, IDocumentSaveOptions<DocumentFormat> options) {
			SaveDocumentCore(fileName, documentFormat, options);
		}
		void SaveDocumentCore(string fileName, DocumentFormat documentFormat, IDocumentSaveOptions<DocumentFormat> options) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				string fullName = GetFullName(stream, fileName);
				SaveDocumentCore(stream, documentFormat, fullName);
				options.CurrentFileName = fileName;
				options.CurrentFormat = documentFormat;
			}
		}
		protected virtual string GetFullName(FileStream stream, string fileName) {
			try {
				string result = stream.Name;
				if (String.IsNullOrEmpty(result))
					return fileName;
				else
					return result;
			}
			catch {
				return fileName;
			}
		}
		protected internal virtual void SaveDocumentCore(Stream stream, DocumentFormat documentFormat, string targetUri) {
			DocumentModel.SaveDocument(stream, documentFormat, targetUri);
		}
		#endregion
		#region Mail Merge
		public ApiMailMergeOptions CreateMailMergeOptions() {
			return new NativeMailMergeOptions();
		}
		public void MailMerge(Document document) {
			MailMerge(CreateMailMergeOptions(), document);
		}
		public void MailMerge(ApiMailMergeOptions options, Document targetDocument) {
			Guard.ArgumentNotNull(targetDocument, "targetDocument");
			MailMerge(options, ((NativeDocument)targetDocument).DocumentModel);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(IRichEditDocumentServer documentServer) {
			MailMerge(CreateMailMergeOptions(), documentServer);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(ApiMailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			Guard.ArgumentNotNull(targetDocumentServer, "targetDocumentServer");
			MailMerge(options, targetDocumentServer.Model);
		}
		public void MailMerge(string fileName, DocumentFormat format) {
			MailMerge(CreateMailMergeOptions(), fileName, format);
		}
		public void MailMerge(Stream stream, DocumentFormat format) {
			MailMerge(CreateMailMergeOptions(), stream, format);
		}
		public void MailMerge(ApiMailMergeOptions options, string fileName, DocumentFormat format) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read)) {
				using (DocumentModel resultDocumentModel = DocumentModel.CreateNew()) {
					CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
					resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
					resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
					MailMerge(options, resultDocumentModel);
					SaveDocumentCore(resultDocumentModel, stream, format, fileName);
					resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
				}
			}
		}
		public void MailMerge(ApiMailMergeOptions options, Stream stream, DocumentFormat format) {
			using (DocumentModel resultDocumentModel = DocumentModel.CreateNew()) {
				CalculateDocumentVariableEventRouter eventRouter = new CalculateDocumentVariableEventRouter(DocumentModel);
				resultDocumentModel.CalculateDocumentVariable += eventRouter.OnCalculateDocumentVariable;
				resultDocumentModel.DocumentExportOptions.CopyFrom(DocumentModel.DocumentExportOptions);
				MailMerge(options, resultDocumentModel);
				SaveDocumentCore(resultDocumentModel, stream, format, String.Empty);
				resultDocumentModel.CalculateDocumentVariable -= eventRouter.OnCalculateDocumentVariable;
			}
		}
		protected internal void MailMerge(ApiMailMergeOptions options, DocumentModel targetModel) {
			Guard.ArgumentNotNull(targetModel, "targetModel");
			Guard.ArgumentNotNull(options, "options");
			targetModel.BeginUpdate();
			targetModel.InternalAPI.CreateNewDocument();
			try {
				targetModel.DocumentSaveOptions.CurrentFileName = DocumentModel.DocumentSaveOptions.CurrentFileName;
				MailMergeHelper mergeHelper = new MailMergeHelper(DocumentModel, ((NativeMailMergeOptions)options).GetInternalMailMergeOptions());
				mergeHelper.ExecuteMailMerge(targetModel);
				targetModel.DocumentSaveOptions.CurrentFileName = String.Empty;
			}
			finally {
				targetModel.EndUpdate();
			}
		}
		protected internal static void SaveDocumentCore(DocumentModel documentModel, Stream stream, DocumentFormat format, string targetUri) {
			IDocumentExportManagerService exportManagerService = documentModel.GetService<IDocumentExportManagerService>();
			if (exportManagerService == null)
				throw new InvalidOperationException("Could not find service: IDocumentExportManagerService");
			ExportHelper<DocumentFormat, bool> exportHelper = documentModel.CreateDocumentExportHelper(format);
			exportHelper.Export(stream, format, targetUri, exportManagerService);
		}
		#endregion
		protected internal virtual void OnUpdateUI() {
		}
		protected internal virtual CopySelectionManager CreateCopySelectionManager() {
			return new CopySelectionManager(this);
		}
	}
	#endregion
	public abstract class DocumentChangesHandler {
		readonly DocumentFormattingController formattingController;
		readonly BackgroundFormatter formatter;
		public DocumentChangesHandler(BackgroundFormatter formatter) {
			this.formattingController = formatter.DocumentFormatter.Controller;
			this.formatter = formatter;
		}
		public DocumentFormattingController FormattingController { get { return formattingController; } }
		public BackgroundFormatter Formatter { get { return formatter; } }
		public virtual DocumentModelPosition NotifyDocumentChanged(PieceTable pieceTable, DocumentModelDeferredChanges changes, bool debugSuppressControllerReset, DocumentLayoutResetType documentLayoutResetType) {
			Debug.Assert(pieceTable.DocumentModel.IsUpdateLockedOrOverlapped);
			if (documentLayoutResetType == DocumentLayoutResetType.AllPrimaryLayout) {
				DocumentModelPosition.SetParagraphStart(changes.ChangeStart, new ParagraphIndex(0));
				DocumentModelPosition.SetParagraphEnd(changes.ChangeEnd, new ParagraphIndex(pieceTable.Paragraphs.Count - 1));
			}
			ProcessSelectionChanges(changes.ChangeActions);
			DocumentModelPosition from = CalculateResetFromPosition(FormattingController, pieceTable, changes, documentLayoutResetType);
			DocumentModelPosition to = changes.ChangeEnd;
			if (to < from)
				to = from;
			ProcessSpellChanges(changes.ChangeActions);
			if (documentLayoutResetType == DocumentLayoutResetType.AllPrimaryLayout && !debugSuppressControllerReset) {
				ResetPages();
				FormattingController.Reset(false);
				Formatter.UpdateSecondaryPositions(from, to);
				Formatter.NotifyDocumentChanged(from, to, documentLayoutResetType);
				return new DocumentModelPosition(pieceTable);
			}
			DocumentModelPosition secondaryFrom;
			if (documentLayoutResetType == DocumentLayoutResetType.PrimaryLayoutFormPosition) {
				pieceTable.ResetParagraphs(from.ParagraphIndex, to.ParagraphIndex);
				ResetPages();
				UnsubscribePageFormattingComplete();
				try {
					secondaryFrom = FormattingController.ResetFrom(from, false);
				}
				finally {
					SubscribePageFormattingComplete();
				}
				Formatter.UpdateSecondaryPositions(secondaryFrom, to);
				Formatter.NotifyDocumentChanged(from, to, documentLayoutResetType);
				GeneratePages();
			}
			else {
				secondaryFrom = from;
				Formatter.UpdateSecondaryPositions(secondaryFrom, to);
				Formatter.NotifyDocumentChanged(from, to, documentLayoutResetType);
			}
			return secondaryFrom;
		}
		public static DocumentModelPosition SetNewPosition(DocumentModelPosition pos, DevExpress.XtraRichEdit.Model.Paragraph paragraph) {
			pos.RunIndex = paragraph.FirstRunIndex;
			pos.ParagraphIndex = paragraph.Index;
			pos.LogPosition = paragraph.LogPosition;
			pos.RunStartLogPosition = paragraph.LogPosition;
			return pos;
		}
		public static DocumentModelPosition CalculateResetFromPosition(DocumentFormattingController formattingController, PieceTable pieceTable, DocumentModelDeferredChanges changes, DocumentLayoutResetType resetType) {
			DocumentModelPosition changeStart = changes.ChangeStart;
			if (resetType == DocumentLayoutResetType.PrimaryLayoutFormPosition) {
				ParagraphIndex paragraphIndex = changeStart.ParagraphIndex;
				if (pieceTable.Paragraphs[paragraphIndex].ContextualSpacing && paragraphIndex > new ParagraphIndex(0))
					paragraphIndex -= 1;
				if (formattingController.RowsController.TablesController.IsInsideTable) {
					TablesControllerTableState state = formattingController.RowsController.TablesController.State as TablesControllerTableState;
					DevExpress.XtraRichEdit.Model.Table table = state.TableViewInfoManager.GetTopLevelTableViewInfoManager().ColumnController.CurrentCell.TableViewInfo.Table;
					if (table.FirstRow.FirstCell.StartParagraphIndex < paragraphIndex) {
						paragraphIndex = table.FirstRow.FirstCell.StartParagraphIndex;
					}
				}
				DocumentModelPosition modelPosition = DocumentModelPosition.FromParagraphStart(pieceTable, paragraphIndex);
				modelPosition = EnsurePositionVisibleWhenHiddenTextNotShown(pieceTable.DocumentModel, modelPosition);
				modelPosition = EnsurePositionNotBeforeSectionBreakAfterParagraphBreak(modelPosition);
				return EnsureTopLevelParagraph(modelPosition);
			}
			return changeStart;
		}
		public static DocumentModelPosition EnsurePositionVisibleWhenHiddenTextNotShown(DocumentModel documentModel, DocumentModelPosition modelPosition) { 
			PieceTable pieceTable = modelPosition.PieceTable;
			IVisibleTextFilter visibleTextFilter = pieceTable.VisibleTextFilter;
			ParagraphIndex paragraphIndex = modelPosition.ParagraphIndex;
			for (; paragraphIndex > ParagraphIndex.Zero; paragraphIndex--)
				if (visibleTextFilter.IsRunVisible(pieceTable.Paragraphs[paragraphIndex - 1].LastRunIndex))
					break;
			if (paragraphIndex == modelPosition.ParagraphIndex)
				return modelPosition;
			else
				return DocumentModelPosition.FromParagraphStart(pieceTable, paragraphIndex);
		}
		public static DocumentModelPosition EnsurePositionNotBeforeSectionBreakAfterParagraphBreak(DocumentModelPosition modelPosition) {
			DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs = modelPosition.PieceTable.Paragraphs;
			TextRunCollection runs = modelPosition.PieceTable.Runs;
			DevExpress.XtraRichEdit.Model.Paragraph paragraph = paragraphs[modelPosition.ParagraphIndex];
			while (modelPosition.RunIndex > RunIndex.Zero && paragraph.Length == 1 && runs[modelPosition.RunIndex] is SectionRun) {
				modelPosition.ParagraphIndex--;
				paragraph = paragraphs[modelPosition.ParagraphIndex];
				modelPosition.RunIndex = paragraph.FirstRunIndex;
				modelPosition.LogPosition = paragraph.LogPosition;
				modelPosition.RunStartLogPosition = modelPosition.LogPosition;
			}
			return modelPosition;
		}
		public static DocumentModelPosition EnsureTopLevelParagraph(DocumentModelPosition pos) {
			DevExpress.XtraRichEdit.Model.ParagraphCollection paragraphs = pos.PieceTable.Paragraphs;
			ParagraphIndex paragraphIndex = pos.ParagraphIndex;
			DevExpress.XtraRichEdit.Model.TableCell cell = paragraphs[paragraphIndex].GetCell();
			if (cell == null) {
				if (paragraphIndex != new ParagraphIndex(0)) {
					for (ParagraphIndex i = paragraphIndex - 1; i >= new ParagraphIndex(0); i--) {
						DevExpress.XtraRichEdit.Model.Paragraph paragraph = paragraphs[i];
						if (paragraph.FrameProperties != null && !paragraph.IsInCell())
							paragraphIndex = i;
						else
							break;
					}
					return SetNewPosition(pos, paragraphs[paragraphIndex]);
				}
				return pos;
			}
			while (cell.Table.ParentCell != null) {
				cell = cell.Table.ParentCell;
			}
			cell = cell.Table.FirstRow.FirstCell;
			paragraphIndex = cell.StartParagraphIndex;
			return SetNewPosition(pos, paragraphs[paragraphIndex]);
		}
		protected abstract void SubscribePageFormattingComplete();
		protected abstract void UnsubscribePageFormattingComplete();
		protected abstract void ProcessSelectionChanges(DocumentModelChangeActions changeActions);
		protected abstract void ProcessSpellChanges(DocumentModelChangeActions changeActions);
		protected abstract void ResetPages();
		protected abstract void GeneratePages();
	}
	public class ControlDocumentChangesHandler : DocumentChangesHandler {
		readonly RichEditView view;
		public ControlDocumentChangesHandler(RichEditView view)
			: base(view.Formatter) {
			this.view = view;
		}
		public RichEditView View { get { return view; } }
		public override DocumentModelPosition NotifyDocumentChanged(PieceTable pieceTable, DocumentModelDeferredChanges changes, bool debugSuppressControllerReset, DocumentLayoutResetType documentLayoutResetType) {
			if ((changes.ChangeActions & DocumentModelChangeActions.ActivePieceTableChanged) != 0)
				View.OnActivePieceTableChanged();
			return base.NotifyDocumentChanged(pieceTable, changes, debugSuppressControllerReset, documentLayoutResetType);
		}
		protected override void ProcessSelectionChanges(DocumentModelChangeActions changeActions) {
			View.ProcessSelectionChanges(changeActions);
		}
		protected override void ProcessSpellChanges(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.ResetUncheckedIntervals) != 0)
				View.DocumentModel.ResetUncheckedSpellIntervals();
		}
		protected override void ResetPages() {
			View.ResetPages(DevExpress.XtraRichEdit.Internal.PrintLayout.PageGenerationStrategyType.RunningHeight);
		}
		protected override void GeneratePages() {
			View.GeneratePages();
		}
		protected override void SubscribePageFormattingComplete() {
			View.SubscribePageFormattingComplete();
		}
		protected override void UnsubscribePageFormattingComplete() {
			View.UnsubscribePageFormattingComplete();
		}
	}
	public class ServerDocumentChangesHandler : DocumentChangesHandler {
		public ServerDocumentChangesHandler(BackgroundFormatter formatter)
			: base(formatter) {
		}
		public override DocumentModelPosition NotifyDocumentChanged(PieceTable pieceTable, DocumentModelDeferredChanges changes, bool debugSuppressControllerReset, DocumentLayoutResetType documentLayoutResetType) {
			if (documentLayoutResetType == DocumentLayoutResetType.None)
				return null;
			return base.NotifyDocumentChanged(pieceTable, changes, debugSuppressControllerReset, documentLayoutResetType);
		}
		protected override void ProcessSelectionChanges(DocumentModelChangeActions changeActions) {
		}
		protected override void ProcessSpellChanges(DocumentModelChangeActions changeActions) {
		}
		protected override void ResetPages() {
		}
		protected override void GeneratePages() {
		}
		protected override void SubscribePageFormattingComplete() {
		}
		protected override void UnsubscribePageFormattingComplete() {
		}
	}
}
