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
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
#if !DXPORTABLE
using DevExpress.Pdf;
#endif
#if !SL
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit {
	[ComVisible(true)]
	public partial class RichEditDocumentServer : IRichEditDocumentServer, IBatchUpdateable, IServiceContainer, IServiceProvider, IDisposable, IInternalRichEditDocumentServerOwner {
		InternalRichEditDocumentServer innerServer;
		RichEditDocumentServerEventRouter eventRouter;
		public RichEditDocumentServer() {
			innerServer = CreateInternalDocumentServer();
			this.eventRouter = CreateEventRouter();
		}
		protected InternalRichEditDocumentServer InnerServer { get { return innerServer; } }
		protected virtual RichEditDocumentServerEventRouter CreateEventRouter() {
			return new RichEditDocumentServerEventRouter(innerServer, this);
		}
		protected virtual InternalRichEditDocumentServer CreateInternalDocumentServer() {
			return new InternalRichEditDocumentServer(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies());
		}
		[Browsable(false)]
		public DevExpress.XtraRichEdit.API.Native.Document Document { get { return innerServer.Document; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerDpiX")]
#endif
		public float DpiX { get { return innerServer.DpiX; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerDpiY")]
#endif
		public float DpiY { get { return innerServer.DpiY; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerHtmlText")]
#endif
		public string HtmlText { get { return innerServer.HtmlText; } set { innerServer.HtmlText = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerIsPrintingAvailable")]
#endif
		public bool IsPrintingAvailable { get { return innerServer.IsPrintingAvailable; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerIsUpdateLocked")]
#endif
		public bool IsUpdateLocked { get { return innerServer.IsUpdateLocked; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerLayoutUnit")]
#endif
		public DevExpress.XtraRichEdit.DocumentLayoutUnit LayoutUnit { get { return innerServer.LayoutUnit; } set { innerServer.LayoutUnit = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMhtText")]
#endif
		public string MhtText { get { return innerServer.MhtText; } set { innerServer.MhtText = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerModel")]
#endif
		public DevExpress.XtraRichEdit.Model.DocumentModelAccessor Model { get { return innerServer.Model; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerModified")]
#endif
		public bool Modified { get { return innerServer.Modified; } set { innerServer.Modified = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerOpenDocumentBytes")]
#endif
		public byte[] OpenDocumentBytes { get { return innerServer.OpenDocumentBytes; } set { innerServer.OpenDocumentBytes = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerOpenXmlBytes")]
#endif
		public byte[] OpenXmlBytes { get { return innerServer.OpenXmlBytes; } set { innerServer.OpenXmlBytes = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerOptions")]
#endif
		public RichEditControlOptionsBase Options { get { return innerServer.Options; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerRtfText")]
#endif
		public string RtfText { get { return innerServer.RtfText; } set { innerServer.RtfText = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerText")]
#endif
		public string Text { get { return innerServer.Text; } set { innerServer.Text = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerUnit")]
#endif
		public DocumentUnit Unit { get { return innerServer.Unit; } set { innerServer.Unit = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerWordMLText")]
#endif
		public string WordMLText { get { return innerServer.WordMLText; } set { innerServer.WordMLText = value; } }
		bool IRichEditDocumentServer.IsDisposed { get { return ((IRichEditDocumentServer)innerServer).IsDisposed; } }
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return ((IBatchUpdateable)innerServer).BatchUpdateHelper; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerDocumentLayout")]
#endif
		public DocumentLayout DocumentLayout { get { return this.InnerServer.InnerServer.DocumentLayout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerLayoutCalculationMode")]
#endif
		public CalculationModeType LayoutCalculationMode { get { return this.InnerServer.InnerServer.LayoutCalculationMode; } set { this.InnerServer.InnerServer.LayoutCalculationMode = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerBeforeExport")]
#endif
		public event BeforeExportEventHandler BeforeExport { add { eventRouter.BeforeExport += value; } remove { eventRouter.BeforeExport -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerAfterExport")]
#endif
		public event EventHandler AfterExport { add { eventRouter.AfterExport += value; } remove { eventRouter.AfterExport -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerBeforeImport")]
#endif
		public event BeforeImportEventHandler BeforeImport { add { eventRouter.BeforeImport += value; } remove { eventRouter.BeforeImport -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerCalculateDocumentVariable")]
#endif
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable { add { eventRouter.CalculateDocumentVariable += value; } remove { eventRouter.CalculateDocumentVariable -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerContentChanged")]
#endif
		public event EventHandler ContentChanged { add { eventRouter.ContentChanged += value; } remove { eventRouter.ContentChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerDocumentClosing")]
#endif
		public event CancelEventHandler DocumentClosing { add { eventRouter.DocumentClosing += value; } remove { eventRouter.DocumentClosing -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerDocumentLoaded")]
#endif
		public event EventHandler DocumentLoaded { add { eventRouter.DocumentLoaded += value; } remove { eventRouter.DocumentLoaded -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerEmptyDocumentCreated")]
#endif
		public event EventHandler EmptyDocumentCreated { add { eventRouter.EmptyDocumentCreated += value; } remove { eventRouter.EmptyDocumentCreated -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerHtmlTextChanged")]
#endif
		public event EventHandler HtmlTextChanged { add { eventRouter.HtmlTextChanged += value; } remove { eventRouter.HtmlTextChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerInitializeDocument")]
#endif
		public event EventHandler InitializeDocument { add { eventRouter.InitializeDocument += value; } remove { eventRouter.InitializeDocument -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerInvalidFormatException")]
#endif
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException { add { eventRouter.InvalidFormatException += value; } remove { eventRouter.InvalidFormatException -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMailMergeFinished")]
#endif
		public event MailMergeFinishedEventHandler MailMergeFinished { add { eventRouter.MailMergeFinished += value; } remove { eventRouter.MailMergeFinished -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMailMergeRecordFinished")]
#endif
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished { add { eventRouter.MailMergeRecordFinished += value; } remove { eventRouter.MailMergeRecordFinished -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMailMergeRecordStarted")]
#endif
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted { add { eventRouter.MailMergeRecordStarted += value; } remove { eventRouter.MailMergeRecordStarted -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMailMergeStarted")]
#endif
		public event MailMergeStartedEventHandler MailMergeStarted { add { eventRouter.MailMergeStarted += value; } remove { eventRouter.MailMergeStarted -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerMhtTextChanged")]
#endif
		public event EventHandler MhtTextChanged { add { eventRouter.MhtTextChanged += value; } remove { eventRouter.MhtTextChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerModifiedChanged")]
#endif
		public event EventHandler ModifiedChanged { add { eventRouter.ModifiedChanged += value; } remove { eventRouter.ModifiedChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerOpenDocumentBytesChanged")]
#endif
		public event EventHandler OpenDocumentBytesChanged { add { eventRouter.OpenDocumentBytesChanged += value; } remove { eventRouter.OpenDocumentBytesChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerOpenXmlBytesChanged")]
#endif
		public event EventHandler OpenXmlBytesChanged { add { eventRouter.OpenXmlBytesChanged += value; } remove { eventRouter.OpenXmlBytesChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerRtfTextChanged")]
#endif
		public event EventHandler RtfTextChanged { add { eventRouter.RtfTextChanged += value; } remove { eventRouter.RtfTextChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerSelectionChanged")]
#endif
		public event EventHandler SelectionChanged { add { eventRouter.SelectionChanged += value; } remove { eventRouter.SelectionChanged -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerUnhandledException")]
#endif
		public event RichEditUnhandledExceptionEventHandler UnhandledException { add { eventRouter.UnhandledException += value; } remove { eventRouter.UnhandledException -= value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditDocumentServerWordMLTextChanged")]
#endif
		public event EventHandler WordMLTextChanged { add { eventRouter.WordMLTextChanged += value; } remove { eventRouter.WordMLTextChanged -= value; } }
		public event BeforePagePaintEventHandler BeforePagePaint { add { eventRouter.BeforePagePaint += value; } remove { eventRouter.BeforePagePaint -= value; } }
		public event CommentEditingEventHandler CommentInserted { add { eventRouter.CommentInserted += value; } remove { eventRouter.CommentInserted -= value; } }
		event EventHandler IRichEditDocumentServer.UnitChanging { add { eventRouter.UnitChanging += value; } remove { eventRouter.UnitChanging -= value; } }
		event EventHandler IRichEditDocumentServer.UnitChanged { add { eventRouter.UnitChanged += value; } remove { eventRouter.UnitChanged -= value; } }
		public void AddService(Type serviceType, object serviceInstance) {
			innerServer.AddService(serviceType, serviceInstance);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			innerServer.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			innerServer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			innerServer.AddService(serviceType, callback, promote);
		}
		public virtual void BeginInitialize() {
			innerServer.BeginInitialize();
		}
		public void BeginUpdate() {
			innerServer.BeginUpdate();
		}
		public void CancelUpdate() {
			innerServer.CancelUpdate();
		}
		public DevExpress.XtraRichEdit.API.Native.MailMergeOptions CreateMailMergeOptions() {
			return innerServer.CreateMailMergeOptions();
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026")]
		public virtual bool CreateNewDocument(bool raiseDocumentClosing = false) {
			return innerServer.CreateNewDocument(raiseDocumentClosing);
		}
		public void EndUpdate() {
			innerServer.EndUpdate();
		}
#if !SL && !DXPORTABLE
		public void ExportToPdf(Stream stream) {
			innerServer.ExportToPdf(stream);
		}
		public void ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			innerServer.ExportToPdf(stream, creationOptions, saveOptions);
		}
		[Obsolete("Use the 'RichEditDocumentServer.ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions)' method instead.", false)]
		public void ExportToPdf(Stream stream, DevExpress.XtraPrinting.PdfExportOptions pdfExportOptions) {
			innerServer.ExportToPdf(stream, pdfExportOptions);
		}
#endif
		public T GetService<T>() where T : class {
			return innerServer.GetService<T>();
		}
		public virtual object GetService(Type serviceType) {
			return innerServer.GetService(serviceType);
		}
		public virtual void LoadDocument(Stream stream, DocumentFormat documentFormat) {
			innerServer.LoadDocument(stream, documentFormat);
		}
#if!SL
		public virtual void LoadDocument(string fileName) {
			innerServer.LoadDocument(fileName);
		}
		public virtual void LoadDocument(string fileName, DocumentFormat documentFormat) {
			innerServer.LoadDocument(fileName, documentFormat);
		}
		public virtual void LoadDocumentTemplate(string fileName) {
			innerServer.LoadDocumentTemplate(fileName);
		}
		public virtual void LoadDocumentTemplate(string fileName, DocumentFormat documentFormat) {
			innerServer.LoadDocumentTemplate(fileName, documentFormat);
		}
		public virtual void SaveDocument(string fileName, DocumentFormat documentFormat) {
			innerServer.SaveDocument(fileName, documentFormat);
		}
#endif
		public virtual void LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat) {
			innerServer.LoadDocumentTemplate(stream, documentFormat);
		}
		public void MailMerge(DevExpress.XtraRichEdit.API.Native.Document document) {
			innerServer.MailMerge(document);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(IRichEditDocumentServer documentServer) {
			innerServer.MailMerge(documentServer);
		}
		public void MailMerge(DevExpress.XtraRichEdit.API.Native.MailMergeOptions options, DevExpress.XtraRichEdit.API.Native.Document targetDocument) {
			innerServer.MailMerge(options, targetDocument);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete("This method has become obsolete. Use another MailMerge method overload with appropriate parameters instead.")]
		public void MailMerge(DevExpress.XtraRichEdit.API.Native.MailMergeOptions options, IRichEditDocumentServer targetDocumentServer) {
			innerServer.MailMerge(options, targetDocumentServer);
		}
		public void MailMerge(Stream stream, DocumentFormat format) {
			innerServer.MailMerge(stream, format);
		}
		public void MailMerge(string fileName, DocumentFormat format) {
			innerServer.MailMerge(fileName, format);
		}
		public void MailMerge(DevExpress.XtraRichEdit.API.Native.MailMergeOptions options, Stream stream, DocumentFormat format) {
			innerServer.MailMerge(options, stream, format);
		}
		public void MailMerge(DevExpress.XtraRichEdit.API.Native.MailMergeOptions options, string fileName, DocumentFormat format) {
			innerServer.MailMerge(options, fileName, format);
		}
		public void RemoveService(Type serviceType) {
			innerServer.RemoveService(serviceType);
		}
		public void RemoveService(Type serviceType, bool promote) {
			innerServer.RemoveService(serviceType, promote);
		}
		public T ReplaceService<T>(T newService) where T : class {
			return innerServer.ReplaceService<T>(newService);
		}
		public virtual void SaveDocument(Stream stream, DocumentFormat documentFormat) {
			innerServer.SaveDocument(stream, documentFormat);
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (innerServer != null)
					innerServer.Dispose();
				if (eventRouter != null)
					eventRouter.Dispose();
			}
			GC.SuppressFinalize(this);
		}
		#endregion
		InternalRichEditDocumentServer IInternalRichEditDocumentServerOwner.InternalServer { get { return innerServer; } }
	}
	public partial class RichEditDocumentServer : IPrintable {
		IPrintable InnerIPrintable { get { return (IPrintable)innerServer; } }
		bool IPrintable.CreatesIntersectedBricks {
			get { return InnerIPrintable.CreatesIntersectedBricks; }
		}
		UserControl IPrintable.PropertyEditorControl {
			get { return InnerIPrintable.PropertyEditorControl; }
		}
		void IPrintable.AcceptChanges() {
			InnerIPrintable.AcceptChanges();
		}
		bool IPrintable.HasPropertyEditor() {
			return InnerIPrintable.HasPropertyEditor();
		}
		void IPrintable.RejectChanges() {
			InnerIPrintable.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			InnerIPrintable.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return InnerIPrintable.SupportsHelp();
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			InnerIPrintable.CreateArea(areaName, graph);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			InnerIPrintable.Finalize(ps, link);
		}
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			InnerIPrintable.Initialize(ps, link);
		}
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	#region RichEditDocumentServerEventRouter
	public class RichEditDocumentServerEventRouter : IDisposable {
		RichEditDocumentServer server;
		InternalRichEditDocumentServer innerServer;
		public RichEditDocumentServerEventRouter(InternalRichEditDocumentServer innerServer, RichEditDocumentServer server) {
			this.server = server;
			this.innerServer = innerServer;
			SubscribeInnerServerEvents();
		}
		void SubscribeInnerServerEvents() {
			innerServer.BeforeExport += RaiseBeforeExport;
			innerServer.AfterExport += RaiseAfterExport;
			innerServer.BeforeImport += RaiseBeforeImport;
			innerServer.CalculateDocumentVariable += RaiseCalculateDocumentVariable;
			innerServer.ContentChanged += RaiseContentChanged;
			innerServer.DocumentClosing += RaiseDocumentClosing;
			innerServer.DocumentLoaded += RaiseDocumentLoaded;
			innerServer.EmptyDocumentCreated += RaiseEmptyDocumentCreated;
			innerServer.HtmlTextChanged += RaiseHtmlTextChanged;
			innerServer.InitializeDocument += RaiseInitializeDocument;
			innerServer.InvalidFormatException += RaiseInvalidFormatException;
			innerServer.MailMergeFinished += RaiseMailMergeFinished;
			innerServer.MailMergeRecordFinished += RaiseMailMergeRecordFinished;
			innerServer.MailMergeRecordStarted += RaiseMailMergeRecordStarted;
			innerServer.MailMergeStarted += RaiseMailMergeStarted;
			innerServer.MhtTextChanged += RaiseMhtTextChanged;
			innerServer.ModifiedChanged += RaiseModifiedChanged;
			innerServer.OpenDocumentBytesChanged += RaiseOpenDocumentBytesChanged;
			innerServer.OpenXmlBytesChanged += RaiseOpenXmlBytesChanged;
			innerServer.RtfTextChanged += RaiseRtfTextChanged;
			innerServer.SelectionChanged += RaiseSelectionChanged;
			innerServer.UnhandledException += RaiseUnhandledException;
			innerServer.WordMLTextChanged += RaiseWordMLTextChanged;
			innerServer.BeforePagePaint += RaiseBeforePagePaint;
			((IRichEditDocumentServer)innerServer).UnitChanging += RaiseUnitChanging;
			((IRichEditDocumentServer)innerServer).UnitChanged += RaiseUnitChanged;
		}
		void UnsubscribeInnerServerEvents() {
			if (innerServer == null)
				return;
			innerServer.BeforeExport -= RaiseBeforeExport;
			innerServer.AfterExport -= RaiseAfterExport;
			innerServer.BeforeImport -= RaiseBeforeImport;
			innerServer.CalculateDocumentVariable -= RaiseCalculateDocumentVariable;
			innerServer.ContentChanged -= RaiseContentChanged;
			innerServer.DocumentClosing -= RaiseDocumentClosing;
			innerServer.DocumentLoaded -= RaiseDocumentLoaded;
			innerServer.EmptyDocumentCreated -= RaiseEmptyDocumentCreated;
			innerServer.HtmlTextChanged -= RaiseHtmlTextChanged;
			innerServer.InitializeDocument -= RaiseInitializeDocument;
			innerServer.InvalidFormatException -= RaiseInvalidFormatException;
			innerServer.MailMergeFinished -= RaiseMailMergeFinished;
			innerServer.MailMergeRecordFinished -= RaiseMailMergeRecordFinished;
			innerServer.MailMergeRecordStarted -= RaiseMailMergeRecordStarted;
			innerServer.MailMergeStarted -= RaiseMailMergeStarted;
			innerServer.MhtTextChanged -= RaiseMhtTextChanged;
			innerServer.ModifiedChanged -= RaiseModifiedChanged;
			innerServer.OpenDocumentBytesChanged -= RaiseOpenDocumentBytesChanged;
			innerServer.OpenXmlBytesChanged -= RaiseOpenXmlBytesChanged;
			innerServer.RtfTextChanged -= RaiseRtfTextChanged;
			innerServer.SelectionChanged -= RaiseSelectionChanged;
			innerServer.UnhandledException -= RaiseUnhandledException;
			innerServer.WordMLTextChanged -= RaiseWordMLTextChanged;
			innerServer.BeforePagePaint -= RaiseBeforePagePaint;
			((IRichEditDocumentServer)innerServer).UnitChanging -= RaiseUnitChanging;
			((IRichEditDocumentServer)innerServer).UnitChanged -= RaiseUnitChanged;
		}
		public event BeforeExportEventHandler BeforeExport;
		void RaiseBeforeExport(object sender, BeforeExportEventArgs e) {
			if (BeforeExport == null)
				return;
			BeforeExport(sender == innerServer ? server : sender, e);
		}
		public event EventHandler AfterExport;
		void RaiseAfterExport(object sender, EventArgs e) {
			if (AfterExport == null)
				return;
			AfterExport(sender == innerServer ? server : sender, e);
		}
		public event BeforeImportEventHandler BeforeImport;
		void RaiseBeforeImport(object sender, BeforeImportEventArgs e) {
			if (BeforeImport == null)
				return;
			BeforeImport(sender == innerServer ? server : sender, e);
		}
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable;
		void RaiseCalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e) {
			if (CalculateDocumentVariable == null)
				return;
			CalculateDocumentVariable(sender == innerServer ? server : sender, e);
		}
		public event CancelEventHandler DocumentClosing;
		void RaiseDocumentClosing(object sender, CancelEventArgs e) {
			if (DocumentClosing == null)
				return;
			DocumentClosing(sender == innerServer ? server : sender, e);
		}
		public event EventHandler ContentChanged;
		void RaiseContentChanged(object sender, EventArgs e) {
			if (ContentChanged == null)
				return;
			ContentChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler DocumentLoaded;
		void RaiseDocumentLoaded(object sender, EventArgs e) {
			if (DocumentLoaded == null)
				return;
			DocumentLoaded(sender == innerServer ? server : sender, e);
		}
		public event EventHandler EmptyDocumentCreated;
		void RaiseEmptyDocumentCreated(object sender, EventArgs e) {
			if (EmptyDocumentCreated == null)
				return;
			EmptyDocumentCreated(sender == innerServer ? server : sender, e);
		}
		public event EventHandler HtmlTextChanged;
		void RaiseHtmlTextChanged(object sender, EventArgs e) {
			if (HtmlTextChanged == null)
				return;
			HtmlTextChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler InitializeDocument;
		void RaiseInitializeDocument(object sender, EventArgs e) {
			if (InitializeDocument == null)
				return;
			InitializeDocument(sender == innerServer ? server : sender, e);
		}
		public event RichEditInvalidFormatExceptionEventHandler InvalidFormatException;
		void RaiseInvalidFormatException(object sender, RichEditInvalidFormatExceptionEventArgs e) {
			if (InvalidFormatException == null)
				return;
			InvalidFormatException(sender == innerServer ? server : sender, e);
		}
		public event MailMergeFinishedEventHandler MailMergeFinished;
		void RaiseMailMergeFinished(object sender, MailMergeFinishedEventArgs e) {
			if (MailMergeFinished == null)
				return;
			MailMergeFinished(sender == innerServer ? server : sender, e);
		}
		public event MailMergeRecordFinishedEventHandler MailMergeRecordFinished;
		void RaiseMailMergeRecordFinished(object sender, MailMergeRecordFinishedEventArgs e) {
			if (MailMergeRecordFinished == null)
				return;
			MailMergeRecordFinished(sender == innerServer ? server : sender, e);
		}
		public event MailMergeRecordStartedEventHandler MailMergeRecordStarted;
		void RaiseMailMergeRecordStarted(object sender, MailMergeRecordStartedEventArgs e) {
			if (MailMergeRecordStarted == null)
				return;
			MailMergeRecordStarted(sender == innerServer ? server : sender, e);
		}
		public event MailMergeStartedEventHandler MailMergeStarted;
		void RaiseMailMergeStarted(object sender, MailMergeStartedEventArgs e) {
			if (MailMergeStarted == null)
				return;
			MailMergeStarted(sender == innerServer ? server : sender, e);
		}
		public event EventHandler MhtTextChanged;
		void RaiseMhtTextChanged(object sender, EventArgs e) {
			if (MhtTextChanged == null)
				return;
			MhtTextChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler ModifiedChanged;
		void RaiseModifiedChanged(object sender, EventArgs e) {
			if (ModifiedChanged == null)
				return;
			ModifiedChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler OpenDocumentBytesChanged;
		void RaiseOpenDocumentBytesChanged(object sender, EventArgs e) {
			if (OpenDocumentBytesChanged == null)
				return;
			OpenDocumentBytesChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler OpenXmlBytesChanged;
		void RaiseOpenXmlBytesChanged(object sender, EventArgs e) {
			if (OpenXmlBytesChanged == null)
				return;
			OpenXmlBytesChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler RtfTextChanged;
		void RaiseRtfTextChanged(object sender, EventArgs e) {
			if (RtfTextChanged == null)
				return;
			RtfTextChanged(sender == innerServer ? server : sender, e);
		}
		public event EventHandler SelectionChanged;
		void RaiseSelectionChanged(object sender, EventArgs e) {
			if (SelectionChanged == null)
				return;
			SelectionChanged(sender == innerServer ? server : sender, e);
		}
		public event RichEditUnhandledExceptionEventHandler UnhandledException;
		void RaiseUnhandledException(object sender, RichEditUnhandledExceptionEventArgs e) {
			if (UnhandledException == null)
				return;
			UnhandledException(sender == innerServer ? server : sender, e);
		}
		public event EventHandler WordMLTextChanged;
		void RaiseWordMLTextChanged(object sender, EventArgs e) {
			if (WordMLTextChanged == null)
				return;
			WordMLTextChanged(sender == innerServer ? server : sender, e);
		}
		internal event BeforePagePaintEventHandler BeforePagePaint;
		void RaiseBeforePagePaint(object sender, BeforePagePaintEventArgs e) {
			if (BeforePagePaint == null)
				return;
			BeforePagePaint(sender == innerServer ? server : sender, e);
		}
		public event CommentEditingEventHandler CommentInserted;
		void RaiseCommentInserted(object sender, CommentEditingEventArgs e) {
			if (CommentInserted != null)
				CommentInserted(sender == innerServer ? server : sender, e);
		}
		public event EventHandler UnitChanging;
		void RaiseUnitChanging(object sender, EventArgs e) {
			if (UnitChanging == null)
				return;
			UnitChanging(sender == innerServer ? server : sender, e);
		}
		public event EventHandler UnitChanged;
		void RaiseUnitChanged(object sender, EventArgs e) {
			if (UnitChanged == null)
				return;
			UnitChanged(sender == innerServer ? server : sender, e);
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeInnerServerEvents();
				innerServer = null;
				server = null;
			}
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	#endregion
}
