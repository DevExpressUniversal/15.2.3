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

using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using DevExpress.Utils.Serializing;
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils;
using System.ComponentModel.Design;
using System.Xml;
using System.Globalization;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
#if SL
using DevExpress.Xpf.ComponentModel;
using DevExpress.Xpf.ComponentModel.Design;
using System.Windows.Printing;
using System.Windows.Input;
using DevExpress.Xpf.Drawing.Printing;
using DevExpress.Utils.Zip;
using DevExpress.Xpf.Collections;
using System.Windows.Media;
using System.Security;
using System.Windows.Controls;
#else
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Rtf;
using System.IO.Compression;
using DevExpress.XtraPrinting.Export.XLS;
using DevExpress.XtraPrinting.Export.Imaging;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
	using DevExpress.DocumentView;
	using DevExpress.XtraPrinting.Preview;
	using System.Net.Mail;
	[
	System.ComponentModel.ToolboxItem(false)
	]
	public class PrintingSystemBase : Component, ISupportInitialize, IPrintingSystem, IPrintingSystemContext, IDocument {
		protected internal virtual void EnsureBrickOnPage(BrickPagePair pair, Action<BrickPagePair> callback) {
			callback(pair);
		}
		protected virtual void BeforeDrawPagesCore(object syncObj) { }
		protected virtual void AfterDrawPagesCore(object syncObj, int[] pageIndices) { }
		void IDocument.BeforeDrawPages(object syncObj) {
			BeforeDrawPagesCore(syncObj);
		}
		void IDocument.AfterDrawPages(object syncObj, int[] pageIndices) {
			AfterDrawPagesCore(syncObj, pageIndices);
		}
		IList<IPage> innerPages;
		IList<IPage> IDocument.Pages {
			get {
				if(innerPages == null)
					innerPages = new ListWrapper<IPage, Page>(Document.Pages);
				return innerPages;
			}
		}
		bool IDocument.IsCreating {
			get {
				return Document.IsCreating;
			}
		}
		bool IDocument.IsEmpty {
			get {
				return Document.IsEmpty;
			}
		}
		IPageSettings IDocument.PageSettings {
			get { return PageSettings; }
		}
		event EventHandler IDocument.DocumentChanged {
			add { DocumentChanged += value; }
			remove { DocumentChanged -= value; }
		}
		event EventHandler pageBackgrChanged;
		event EventHandler IDocument.PageBackgrChanged {
			add { pageBackgrChanged += value; }
			remove { pageBackgrChanged -= value; }
		}
		internal void RaisePageBackgrChanged(EventArgs e) {
			if(pageBackgrChanged != null) pageBackgrChanged(this, e); 
		}
		#region inner classes
		class IntersectedBricksHelper {
			ICollection intersectedBricks;
			public ICollection IntersectedBricks {
				get {
					if(intersectedBricks == null)
						intersectedBricks = GetIntersectedBricks();
					return intersectedBricks;
				}
			}
			PrintingSystemBase ps;
			public IntersectedBricksHelper(PrintingSystemBase ps) {
				this.ps = ps;
			}
			public void HighlightIntersectedBricks(Color backColor) {
				foreach(Brick brick in IntersectedBricks) {
					VisualBrick visualBrick = brick as VisualBrick;
					if(visualBrick != null)
						visualBrick.BackColor = backColor;
				}
			}
			ICollection GetIntersectedBricks() {
				ArrayList bricks = new ArrayList();
				foreach(Page page in ps.Document.Pages) {
					page.PerformLayout(ps);
					bricks.AddRange(GetIntersectedBricks(page));
				}
				return bricks;
			}
			ICollection GetIntersectedBricks(Page page) {
				ArrayList outBricks = new ArrayList();
				ArrayList bricks = new ArrayList();
				ArrayList rects = new ArrayList();
				BrickEnumerator en = new BrickEnumerator(page);
				while(en.MoveNext()) {
					if(en.CurrentBrick.IsVisible) {
						bricks.Add(en.CurrentBrick);
						rects.Add(en.GetCurrentBrickBounds());
					}
				}
				for(int i = 0; i < bricks.Count; i++) {
					Brick brick1 = (Brick)bricks[i];
					RectangleF rect1 = (RectangleF)rects[i];
					if(brick1 is PanelBrick)
						outBricks.AddRange(GetIntersectedBricks(((PanelBrick)brick1).Bricks, brick1.Size));
					for(int j = 0; j < bricks.Count; j++) {
						Brick brick2 = (Brick)bricks[j];
						RectangleF rect2 = (RectangleF)rects[j];
						if(brick1 != brick2 && !FloatsComparer.Default.RectangleIsEmpty(RectangleF.Intersect(rect1, rect2)))
							outBricks.Add(brick1);
					}
				}
				return outBricks;
			}
			ICollection GetIntersectedBricks(BrickCollectionBase bricks, SizeF parentSize) {
				ArrayList outBricks = new ArrayList();
				for(int i = 0; i < bricks.Count; i++) {
					Brick brick1 = bricks[i];
					RectangleF rect1 = brick1.Rect;
					if(rect1.Left < 0 || rect1.Top < 0 || FloatsComparer.Default.FirstGreaterSecond(rect1.Right, parentSize.Width) || FloatsComparer.Default.FirstGreaterSecond(rect1.Bottom, parentSize.Height)) {
						outBricks.Add(brick1);
						continue;
					}
					if(brick1 is PanelBrick)
						outBricks.AddRange(GetIntersectedBricks(((PanelBrick)brick1).Bricks, brick1.Size));
					for(int j = 0; j < bricks.Count; j++) {
						Brick brick2 = bricks[j];
						if(brick1 != brick2 && !FloatsComparer.Default.RectangleIsEmpty(RectangleF.Intersect(rect1, brick2.Rect)))
							outBricks.Add(brick1);
					}
				}
				return outBricks;
			}
		}
		#endregion
		#region static
		internal const bool ContinuousPageNumberingDefaultValue = true;
		static string userName = string.Empty;
		public static string UserName {
			get {
#if SL
				return userName;
#else
				return !string.IsNullOrEmpty(userName) ? userName : SystemInformation.UserName;
#endif
			}
			set { userName = value; }
		}
		internal static ChangeEventArgs CreateEventArgs(string eventName, object[,] parameters) {
			ChangeEventArgs args = new ChangeEventArgs(eventName);
			if(parameters != null) {
				int length = parameters.GetLength(0);
				for(int i = 0; i < length; i++)
					args.Add((string)parameters[i, 0], parameters[i, 1]);
			}
			return args;
		}
		static void CheckExportMode(object currentMode, object prohibitedMode, PreviewStringId id) {
			if(Object.Equals(currentMode, prohibitedMode)) {
				string message = PreviewLocalizer.GetString(id);
				if(message != string.Empty)
					ExceptionHelper.ThrowInvalidOperationException(message);
			}
		}
		static XmlXtraSerializer CreateNormalSerializer() {
			return new PrintingSystemXmlSerializer();
		}
		internal static XmlXtraSerializer CreateIndependentPagesSerializer() {
			return new IndependentPagesPrintingSystemSerializer();
		}
		static void CloseStream(bool compressed, Stream stream) {
			if(compressed)
				stream.Close();
		}
#if !SL
		public static void SetBrickExporter(Type brickType, Type exporterType) {
			ExportersFactory.SetExporter(brickType, exporterType);
		}
		public static Type GetBrickExporter(Type brickType) {
			return ExportersFactory.GetExporterType(brickType);
		}
#endif
		#endregion
		#region Fields
		protected XtraPageSettingsBase xtraSettings;
		Document fDocument;
		BrickGraphics brickGraphics;
		GdiHashtable gdi;
		int initializingCounter;
		bool cancelPending = false;
		bool locked;
		bool continuousPageNumbering = ContinuousPageNumberingDefaultValue;
		LinkBase activeLink;
		BrickPagePairCollection markedPairs = new BrickPagePairCollection();
		Watermark watermark;
		bool showMarginsWarning = true;
		bool showPrintStatusDialog = true;
		ExportOptions exportOptions;
		bool raisingAfterChange;
		IObjectContainer[] containers;
		protected ProgressReflector fProgressReflector;
		ProgressReflector subscribedProgressReflector;
		protected ProgressReflector fFakedReflector = new ProgressReflector();
		bool isDisposed;
		int disposeCount;
		BrickPaintBase brickPainter;
		IPrintingSystemExtenderBase extender;
		ServiceContainer defaultServiceContainer;
		ServiceContainer serviceContainer;
		#endregion
		protected PrintingSystemBase(IContainer container, ExportOptions exportOptions) {
			if(exportOptions == null)
				throw new ArgumentNullException("exportOptions");
			this.exportOptions = exportOptions;
			if(container != null)
				container.Add(this);
			brickGraphics = new BrickGraphics(this);
			gdi = new GdiHashtable();
			containers = new IObjectContainer[] { new StylesContainer(), new ImagesContainer(), new ImagesContainer() };
			defaultServiceContainer = new ServiceContainer();
			defaultServiceContainer.AddService(typeof(Measurer), (c, t) => new GdiPlusMeasurer(this));
			defaultServiceContainer.AddService(typeof(GraphicsModifier), (c, t) => new GdiPlusGraphicsModifier());
			defaultServiceContainer.AddService(typeof(IPageContentService), (c, t) => new PageContentService());
			defaultServiceContainer.AddService(typeof(MergeBrickHelper), (c, t) => new MergeBrickHelper());
			serviceContainer = new ServiceContainer(defaultServiceContainer);
		}
		public PrintingSystemBase(System.ComponentModel.IContainer container)
			: this(container, new ExportOptions()) {
		}
		public PrintingSystemBase()
			: this(null) {
		}
		#region Properties
		internal PrintingSystemAction DelayedAction { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CancelPending { get { return cancelPending; } }
		protected internal void ResetCancelPending() { this.cancelPending = false; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Cancel() { this.cancelPending = true; } 
		internal IPrintingSystemExtenderBase Extender {
			get {
				if(extender == null)
					extender = CreateExtender();
				return extender;
			}
			set { extender = value; }
		}
		internal BrickPaintBase BrickPainter {
			get {
				if(brickPainter == null)
					brickPainter = CreateBrickPaint();
				return brickPainter;
			}
			set {
				brickPainter = value;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		]
		public ProgressReflector ProgressReflector {
			get {
				ProgressReflector result = fProgressReflector != null ? fProgressReflector :
					Extender.ActiveProgressReflector != null ? Extender.ActiveProgressReflector :
					fFakedReflector;
				if(result != subscribedProgressReflector) {
					UnsubscribeEvents(subscribedProgressReflector);
					SubscribeProgressEvents(result);
					subscribedProgressReflector = result;
				}
				return result;
			}
			set {
				fProgressReflector = value;
			}
		}
		[
		Obsolete("Use the ExportOptions.PrintPreview.DefaultSendFormat property instead"),
		DefaultValue(PrintingSystemCommand.SendPdf),
		Browsable(false)
		]
		public PrintingSystemCommand SendDefault {
			get {
				return ExportOptions.PrintPreview.DefaultSendFormat;
			}
			set {
				ExportOptions.PrintPreview.DefaultSendFormat = value;
			}
		}
		[
		Obsolete("Use the ExportOptions.PrintPreview.DefaultExportFormat property instead"),
		DefaultValue(PrintingSystemCommand.ExportPdf),
		Browsable(false)
		]
		public PrintingSystemCommand ExportDefault {
			get {
				return ExportOptions.PrintPreview.DefaultExportFormat;
			}
			set {
				ExportOptions.PrintPreview.DefaultExportFormat = value;
			}
		}
		bool Initializing { get { return initializingCounter != 0; } }
		string IPrintingSystem.Version {
			get {
#if SL
				return new System.Reflection.AssemblyName(typeof(PrintingSystemBase).Assembly.FullName).Version.ToString();
#else
				return typeof(PrintingSystemBase).Assembly.GetName().Version.ToString();
#endif
			}
		}
		IImagesContainer IPrintingSystem.Images { get { return Images; } }
		internal bool RaisingAfterChange { get { return raisingAfterChange; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintingSystemBaseContinuousPageNumbering"),
#endif
		DefaultValue(ContinuousPageNumberingDefaultValue)
		]
		public bool ContinuousPageNumbering { get { return continuousPageNumbering; } set { continuousPageNumbering = value; } }
		internal StylesContainer Styles { get { return (StylesContainer)containers[0]; } }
		internal ImagesContainer Images { get { return (ImagesContainer)containers[1]; } }
		internal ImagesContainer GarbageImages { get { return (ImagesContainer)containers[2]; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintingSystemBaseExportOptions"),
#endif
		Category(NativeSR.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public ExportOptions ExportOptions { get { return exportOptions; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintingSystemBaseShowMarginsWarning"),
#endif
		Category(NativeSR.CatBehavior),
		DefaultValue(true)
		]
		public bool ShowMarginsWarning { get { return showMarginsWarning; } set { showMarginsWarning = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintingSystemBaseShowPrintStatusDialog"),
#endif
		Category(NativeSR.CatBehavior),
		DefaultValue(true)
		]
		public bool ShowPrintStatusDialog { get { return showPrintStatusDialog; } set { showPrintStatusDialog = value; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public XtraPageSettingsBase PageSettings {
			get {
				if(xtraSettings == null)
					xtraSettings = CreatePageSettings();
				return xtraSettings;
			}
		}
		[Browsable(false)]
		public BrickGraphics Graph {
			get { return brickGraphics; }
		}
		[Browsable(false)]
		public Document Document {
			get {
				if(DocumentIsDisposed(fDocument))
					fDocument = CreateDocument();
				return fDocument;
			}
		}
		[Browsable(false)]
		public PrintingDocument PrintingDocument {
			get {
				return (PrintingDocument)Document;
			}
		}
		public int PageCount {
			get { return Document.PageCount; }
		}
		[Browsable(false)]
		public Margins PageMargins { get { return PageSettings.Margins; } }
		[Browsable(false)]
		public Rectangle PageBounds { get { return PageSettings.Bounds; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PrintingSystemBaseWatermark"),
#endif
		Category(NativeSR.CatAppearance)
		]
		public virtual Watermark Watermark {
			get {
				if(watermark == null)
					watermark = new Watermark();
				return watermark;
			}
		}
		[Browsable(false)]
		public PageList Pages {
			get { return Document.Pages; }
		}
		internal GdiHashtable Gdi { get { return gdi; } }
		internal bool Locked { get { return locked; } }
		internal LinkBase ActiveLink { get { return activeLink; } }
		Page IPrintingSystemContext.DrawingPage { get { return null; } }
		PrintingSystemBase IPrintingSystemContext.PrintingSystem { get { return this; } }
		Measurer IPrintingSystemContext.Measurer { get { return ((IServiceProvider)this).GetService(typeof(Measurer)) as Measurer; } }
		bool IPrintingSystemContext.CanPublish(Brick brick) {
			throw new NotSupportedException();
		}
		#endregion
		#region Events
		static readonly object AfterMarginsChangeEvent = new object();
		static readonly object BeforeMarginsChangeEvent = new object();
		static readonly object BeforePagePaintEvent = new object();
		static readonly object AfterPagePaintEvent = new object();
		static readonly object AfterPagePrintEvent = new object();
		static readonly object AfterBandPrintEvent = new object();
		static readonly object BeforeChangeEvent = new object();
		static readonly object AfterChangeEvent = new object();
		static readonly object PageSettingsChangedEvent = new object();
		static readonly object ScaleFactorChangedEvent = new object();
		static readonly object StartPrintEvent = new object();
		static readonly object EndPrintEvent = new object();
		static readonly object PrintProgressEvent = new object();
		static readonly object BeforeBuildPagesEvent = new object();
		static readonly object DocumentChangedEvent = new object();
		static readonly object FillEmptySpaceEvent = new object();
		static readonly object AfterBuildPagesEvent = new object();
		static readonly object CreateDocumentExceptionEvent = new object();
		static readonly object PageInsertCompleteEvent = new object();
		static readonly object XlsxDocumentCreatedEvent = new object();
		static readonly object XlSheetCreatedEvent = new object();
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public event PageInsertCompleteEventHandler PageInsertComplete {
			add { Events.AddHandler(PageInsertCompleteEvent, value); }
			remove { Events.RemoveHandler(PageInsertCompleteEvent, value); }
		}
		public event EventHandler AfterBuildPages {
			add { Events.AddHandler(AfterBuildPagesEvent, value); }
			remove { Events.RemoveHandler(AfterBuildPagesEvent, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public event EmptySpaceEventHandler FillEmptySpace {
			add { Events.AddHandler(FillEmptySpaceEvent, value); }
			remove { Events.RemoveHandler(FillEmptySpaceEvent, value); }
		}
		public event EventHandler BeforeBuildPages {
			add { Events.AddHandler(BeforeBuildPagesEvent, value); }
			remove { Events.RemoveHandler(BeforeBuildPagesEvent, value); }
		}
		internal event EventHandler DocumentChanged {
			add { Events.AddHandler(DocumentChangedEvent, value); }
			remove { Events.RemoveHandler(DocumentChangedEvent, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public event PageEventHandler AfterPagePrint {
			add { Events.AddHandler(AfterPagePrintEvent, value); }
			remove { Events.RemoveHandler(AfterPagePrintEvent, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public event PageEventHandler AfterBandPrint {
			add { Events.AddHandler(AfterBandPrintEvent, value); }
			remove { Events.RemoveHandler(AfterBandPrintEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event MarginsChangeEventHandler AfterMarginsChange {
			add { Events.AddHandler(AfterMarginsChangeEvent, value); }
			remove { Events.RemoveHandler(AfterMarginsChangeEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event MarginsChangeEventHandler BeforeMarginsChange {
			add { Events.AddHandler(BeforeMarginsChangeEvent, value); }
			remove { Events.RemoveHandler(BeforeMarginsChangeEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event PagePaintEventHandler BeforePagePaint {
			add { Events.AddHandler(BeforePagePaintEvent, value); }
			remove { Events.RemoveHandler(BeforePagePaintEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event PagePaintEventHandler AfterPagePaint {
			add { Events.AddHandler(AfterPagePaintEvent, value); }
			remove { Events.RemoveHandler(AfterPagePaintEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event ChangeEventHandler BeforeChange {
			add { Events.AddHandler(BeforeChangeEvent, value); }
			remove { Events.RemoveHandler(BeforeChangeEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event ChangeEventHandler AfterChange {
			add { Events.AddHandler(AfterChangeEvent, value); }
			remove { Events.RemoveHandler(AfterChangeEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event EventHandler PageSettingsChanged {
			add { Events.AddHandler(PageSettingsChangedEvent, value); }
			remove { Events.RemoveHandler(PageSettingsChangedEvent, value); }
		}
		[Category(NativeSR.CatPropertyChanged)]
		public event EventHandler ScaleFactorChanged {
			add { Events.AddHandler(ScaleFactorChangedEvent, value); }
			remove { Events.RemoveHandler(ScaleFactorChangedEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event PrintDocumentEventHandler StartPrint {
			add { Events.AddHandler(StartPrintEvent, value); }
			remove { Events.RemoveHandler(StartPrintEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event EventHandler EndPrint {
			add { Events.AddHandler(EndPrintEvent, value); }
			remove { Events.RemoveHandler(EndPrintEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event PrintProgressEventHandler PrintProgress {
			add { Events.AddHandler(PrintProgressEvent, value); }
			remove { Events.RemoveHandler(PrintProgressEvent, value); }
		}
		[Category(NativeSR.CatDocumentCreation)]
		public event ExceptionEventHandler CreateDocumentException {
			add { Events.AddHandler(CreateDocumentExceptionEvent, value); }
			remove { Events.RemoveHandler(CreateDocumentExceptionEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event XlsxDocumentCreatedEventHandler XlsxDocumentCreated {
			add { Events.AddHandler(XlsxDocumentCreatedEvent, value); }
			remove { Events.RemoveHandler(XlsxDocumentCreatedEvent, value); }
		}
		[Category(NativeSR.CatPrinting)]
		public event XlSheetCreatedEventHandler XlSheetCreated {
			add { Events.AddHandler(XlSheetCreatedEvent, value); }
			remove { Events.RemoveHandler(XlSheetCreatedEvent, value); }
		}
		protected internal virtual void OnCreateDocumentException(ExceptionEventArgs args) {
			Document.OnCreateException();
			ExceptionEventHandler handler = Events[CreateDocumentExceptionEvent] as ExceptionEventHandler;
			if(handler != null)
				handler(this, args);
			if(!args.Handled) 
				throw new Exception(args.Exception.Message, args.Exception);
		}
		protected internal void OnPageInsertComplete(PageInsertCompleteEventArgs e) {
			PageInsertCompleteEventHandler handler = Events[PageInsertCompleteEvent] as PageInsertCompleteEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void OnFillEmptySpace(EmptySpaceEventArgs e) {
			EmptySpaceEventHandler handler = Events[FillEmptySpaceEvent] as EmptySpaceEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void OnDocumentChanged(EventArgs e) {
			EventHandler handler = Events[DocumentChangedEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void OnAfterBuildPages(EventArgs e) {
			EventHandler handler = Events[AfterBuildPagesEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal void OnBeforeBuildPages(EventArgs e) {
			EventHandler handler = Events[BeforeBuildPagesEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		internal void OnStartPrint(PrintDocumentEventArgs e) {
			if(Initializing)
				return;
			PrintDocumentEventHandler eventDelegate = Events[StartPrintEvent] as PrintDocumentEventHandler;
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void OnEndPrint(EventArgs e) {
			EventHandler eventDelegate = Events[EndPrintEvent] as EventHandler;
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void OnPrintProgress(PrintProgressEventArgs e) {
			PrintProgressEventHandler eventDelegate = Events[PrintProgressEvent] as PrintProgressEventHandler;
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void OnBeforeChange(ChangeEventArgs e) {
			if(Initializing)
				return;
			ChangeEventHandler eventDelegate = (ChangeEventHandler)Events[BeforeChangeEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
#if DEBUGTEST
		public void Test_OnAfterChange(ChangeEventArgs e) {
			OnAfterChange(e);
		}
#endif
		internal void OnAfterChange(ChangeEventArgs e) {
			if(Initializing)
				return;
			ChangeEventHandler eventDelegate = (ChangeEventHandler)Events[AfterChangeEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void OnBeforePagePaint(PagePaintEventArgs e) {
			if(Initializing)
				return;
			PagePaintEventHandler eventDelegate = (PagePaintEventHandler)Events[BeforePagePaintEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		protected internal virtual void OnAfterPagePaint(PagePaintEventArgs e) {
			if(Initializing)
				return;
			PagePaintEventHandler eventDelegate = (PagePaintEventHandler)Events[AfterPagePaintEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		protected internal virtual void OnAfterPagePrint(PageEventArgs e) {
			PageEventHandler eventDelegate = (PageEventHandler)Events[AfterPagePrintEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		protected internal virtual void OnAfterBandPrint(PageEventArgs e) {
			PageEventHandler eventDelegate = (PageEventHandler)Events[AfterBandPrintEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal float OnBeforeMarginsChange(MarginSide side, float value) {
			if(Initializing)
				return value;
			MarginsChangeEventArgs args = new MarginsChangeEventArgs(side, value);
			MarginsChangeEventHandler eventDelegate = (MarginsChangeEventHandler)Events[BeforeMarginsChangeEvent];
			if(eventDelegate != null)
				eventDelegate(this, args);
			ChangeEventArgs changeArgs = CreateEventArgs(SR.BeforeMarginsChange, new object[,] { { SR.Side, Convert.ToString(side) }, { SR.Value, args.Value } });
			OnBeforeChange(changeArgs);
			return (float)changeArgs.ValueOf(SR.Value);
		}
		internal void OnAfterMarginsChange(MarginSide side, float value) {
			if(Initializing)
				return;
			if(activeLink != null)
				activeLink.UpdatePageSettingsInternal();
			try {
				raisingAfterChange = true;
				MarginsChangeEventHandler eventDelegate = (MarginsChangeEventHandler)Events[AfterMarginsChangeEvent];
				if(eventDelegate != null)
					eventDelegate(this, new MarginsChangeEventArgs(side, value));
				ChangeEventArgs changeArgs = CreateEventArgs(SR.AfterMarginsChange, new object[,] { { SR.Side, Convert.ToString(side) }, { SR.Value, value } });
				OnAfterChange(changeArgs);
			} finally {
				raisingAfterChange = false;
				ExecDelayedAction();
			}
		}
		internal void OnPageSettingsChanged() {
			if(Initializing)
				return;
			if(activeLink != null)
				activeLink.UpdatePageSettingsInternal();
			try {
				raisingAfterChange = true;
				EventHandler eventDelegate = (EventHandler)Events[PageSettingsChangedEvent];
				if(eventDelegate != null)
					eventDelegate(this, EventArgs.Empty);
				OnAfterChange(CreateEventArgs(SR.PageSettingsChanged, null));
			} finally {
				raisingAfterChange = false;
				ExecDelayedAction();
			}
		}
		internal void OnScaleFactorChanged() {
			if(Initializing)
				return;
			try {
				raisingAfterChange = true;
				EventHandler eventDelegate = (EventHandler)Events[ScaleFactorChangedEvent];
				if(eventDelegate != null)
					eventDelegate(this, EventArgs.Empty);
				OnAfterChange(CreateEventArgs(SR.ScaleFactorChanged, null));
			} finally {
				raisingAfterChange = false;
				ExecDelayedAction();
			}
		}
		internal void OnXlsxDocumentCreated(XlsxExportProvider provider, XlsxDocumentCreatedEventArgs e) {
			XlsxDocumentCreatedEventHandler handler = Events[XlsxDocumentCreatedEvent] as XlsxDocumentCreatedEventHandler;
			if(handler != null)
				handler(provider, e);
		}
		internal void OnXlSheetCreated(NewExcelExportProvider provider, XlSheetCreatedEventArgs e) {
			XlSheetCreatedEventHandler handler = Events[XlSheetCreatedEvent] as XlSheetCreatedEventHandler;
			if(handler != null)
				handler(provider, e);
		}
		void ExecDelayedAction() {
			if((DelayedAction & PrintingSystemAction.CreateDocument) > 0) {
				if(activeLink != null)
					activeLink.CreateDocument();
			} else if((DelayedAction & PrintingSystemAction.HandleNewPageSettings) > 0) {
				Document.HandleNewPageSettings();
			} else if((DelayedAction & PrintingSystemAction.HandleNewScaleFactor) > 0) {
				Document.HandleNewScaleFactor();
			}
			DelayedAction = PrintingSystemAction.None;
		}
		#endregion
		protected virtual XtraPageSettingsBase CreatePageSettings() {
			return new XtraPageSettingsBase(this);
		}
		bool ShouldSerializeExportOptions() {
			return exportOptions.ShouldSerialize();
		}
		public void ResetProgressReflector() {
			fProgressReflector = null;
		}
		protected virtual BrickPaintBase CreateBrickPaint() {
			return new BrickPaint();
		}
		protected virtual IPrintingSystemExtenderBase CreateExtender() {
			return new PrintingSystemExtenderPrint(this);
		}
		protected virtual PrintingDocument CreateDocument() {
			return new PSLinkDocument(this, null);
		}
		void ClearObjectContainers() {
			foreach(IObjectContainer container in containers)
				container.Clear();
		}
		internal void SetDocument(Document doc) {
			if(!ReferenceEquals(doc, fDocument)) {
				DisposeDocucmentCore();
				fDocument = doc;
			}
		}
		void DisposeDocucmentCore() {
			if(!DocumentIsDisposed(fDocument)) {
				fDocument.Dispose();
				fDocument = null;
			}
			if(innerPages != null) {
				innerPages.Clear();
				innerPages = null;
			}
		}
		public void MarkBrick(Brick brick, Page page) {
			if(page != null && brick != null)
				markedPairs.Add(BrickPagePair.Create(brick, page));
		}
		public void UnmarkBrick(Brick brick, Page page) {
			if(page != null && brick != null)
				markedPairs.Remove(BrickPagePair.Create(brick, page));
		}
		internal void ClearMarkedBricks() {
			markedPairs.Clear();
		}
		void ClearWaterMark() {
			if(watermark != null) {
				watermark.Dispose();
				watermark = null;
			}
		}
		internal Brick[] GetMarkedBricks(Page page) {
			return markedPairs.GetBricks(Pages, page.Index);
		}
		internal bool IsDisposed {
			get {
				return isDisposed;
			}
		}
		internal bool IsDisposing {
			get {
				return disposeCount > 0;
			}
		}
		protected void StartDispose() {
			disposeCount++;
		}
		protected void EndDispose() {
			disposeCount--;
			isDisposed = disposeCount <= 0;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				UnsubscribeEvents(subscribedProgressReflector);
				subscribedProgressReflector = null;
				StartDispose();
				if(gdi != null) {
					gdi.Dispose();
					gdi = null;
				}
				DisposeDocument();
				markedPairs.Clear();
				DisposeRichTextBoxForDrawing();
				ClearWaterMark();
				base.Dispose(disposing);
				if(brickGraphics != null) {
					((IDisposable)brickGraphics).Dispose();
					brickGraphics = null;
				}
				if(xtraSettings != null) {
					xtraSettings.Dispose();
					xtraSettings = null;
				}
				if(serviceContainer != null) {
					serviceContainer.Dispose();
					serviceContainer = null;
				}
				if(defaultServiceContainer != null) {
					defaultServiceContainer.Dispose();
					defaultServiceContainer = null;
				}
				if(extender != null) {
					extender.Clear();
					extender = null;
				}
				EndDispose();
			} else
				base.Dispose(disposing);
		}
		void DisposeDocument() {
			DisposeDocucmentCore();
			ClearObjectContainers();
		}
		public void InsertPageBreak(float pos) {
			Document.InsertPageBreak(GraphicsUnitConverter.Convert(pos, brickGraphics.Dpi, GraphicsDpi.Document));
		}
		public void InsertPageBreak(float pos, Margins margins, PaperKind? paperKind, Size pageSize, bool? landscape) {
			float convertedPos = GraphicsUnitConverter.Convert(pos, brickGraphics.Dpi, GraphicsDpi.Document);
			CustomPageData nextPageData = new CustomPageData();
			nextPageData.Margins = margins;
			nextPageData.PaperKind = paperKind;
			nextPageData.PageSize = pageSize;
			nextPageData.Landscape = landscape;
			Document.InsertPageBreak(convertedPos, nextPageData);
		}
		public void ClearContent() {
			if(fDocument != null)
				fDocument.ClearContent();
			ClearObjectContainers();
		}
		public void Lock() {
			locked = true;
		}
		public void Unlock() {
			locked = false;
		}
		public void Begin() {
			((ISupportInitialize)this).BeginInit();
			ClearContent();
			FinalizeLink(activeLink);
			brickGraphics.Init();
			brickGraphics.DefaultBrickStyle = BrickStyle.CreateDefault();
			Document.Begin();
			Extender.OnBeginCreateDocument();
		}
		public void End() {
			End(false);
		}
		public void End(bool buildPagesInBackground) {
			brickGraphics.Clear();
			Document.End(buildPagesInBackground);
			((ISupportInitialize)this).EndInit();
			ResetDocumentIfCancel();
		}
		protected internal void ResetDocumentIfCancel() {
			if(CancelPending) {
				ClearContent();
			}
		}
		internal void End(LinkBase link) {
			End(link, false);
		}
		internal void End(LinkBase link, bool buildPagesInBackground) {
			System.Diagnostics.Debug.Assert(link != null);
			activeLink = link;
			End(buildPagesInBackground);
		}
		internal virtual void FinalizeLink(LinkBase link) {
			if(activeLink != null && Comparer.Equals(activeLink, link)) {
				activeLink.BeforeDestroy();
				activeLink = null;
			}
		}
		public void BeginSubreport(PointF offset) {
			this.BeginSubreportInternal(null, offset);
		}
		internal void BeginSubreportInternal(DocumentBand docBand, PointF offset) {
			Document.BeginReport(docBand, GraphicsUnitConverter.Convert(offset, this.Graph.Dpi, GraphicsDpi.Document));
		}
		public void EndSubreport() {
			Document.EndReport();
		}
		public Page CreatePage() {
			return CreatePSPage();
		}
		internal PSPage CreatePSPage() {
			return new PSPage(new ReadonlyPageData(PageSettings.Data));
		}
		bool DocumentIsDisposed(Document doc) {
			return doc == null || doc.IsDisposed;
		}
		protected TextLayoutBuilder CreateTextLayoutBuilder(TextExportContext exportContext, TextExportOptionsBase options) {
			return new TextLayoutBuilder(Document, exportContext, options.Separator, options.QuoteStringsWithSeparators, options.TextExportMode, !(options is CsvExportOptions));
		}
		protected void SubscribeProgressEvents(ProgressReflector progressReflector) {
			if(progressReflector != null)
				progressReflector.PositionChanged += new EventHandler(OnProgressPositionChanged);
		}
		protected void UnsubscribeEvents(ProgressReflector progressReflector) {
			if(progressReflector != null)
				progressReflector.PositionChanged -= new EventHandler(OnProgressPositionChanged);
		}
		void OnProgressPositionChanged(object sender, EventArgs e) {
			ChangeEventArgs args = new ChangeEventArgs(SR.ProgressPositionChanged);
			args.Add(SR.ProgressPosition, ((ProgressReflector)sender).PositionCore);
			args.Add(SR.ProgressMaximum, ((ProgressReflector)sender).MaximumCore);
			OnAfterChange(args);
		}
		protected internal virtual void OnClearPages() {
			ClearMarkedBricks();
		}
		public virtual CommandVisibility GetCommandVisibility(PrintingSystemCommand command) {
			return Extender.GetCommandVisibility(command);
		}
		internal bool IsNoneVisibleCommand(PrintingSystemCommand command) {
			return GetCommandVisibility(command) == CommandVisibility.None;
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			serviceContainer.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			serviceContainer.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceContainer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if(serviceContainer != null)
				serviceContainer.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			serviceContainer.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			serviceContainer.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return serviceType == typeof(IServiceContainer) ? this : 
				serviceContainer.GetService(serviceType);
		}
		#endregion
		#region IPrintingSystem Members
		int IPrintingSystem.AutoFitToPagesWidth { get { return this.Document.AutoFitToPagesWidth; } set { this.Document.AutoFitToPagesWidth = value; } }
		ITextBrick IPrintingSystem.CreateTextBrick() {
			return new TextBrick();
		}
		IRichTextBrick IPrintingSystem.CreateRichTextBrick() {
			return BrickFactory.CreateBrick(BrickTypes.RichText) as IRichTextBrick;
		}
		IImageBrick IPrintingSystem.CreateImageBrick() {
			return new ImageBrick();
		}
		IPanelBrick IPrintingSystem.CreatePanelBrick() {
			return new PanelBrick();
		}
		IProgressBarBrick IPrintingSystem.CreateProgressBarBrick() {
			return new ProgressBarBrick();
		}
		ITrackBarBrick IPrintingSystem.CreateTrackBarBrick() {
			return new TrackBarBrick();
		}
		internal PrintingSystemCommand GetFirstVisibleCommand(PrintingSystemCommand[] commands) {
			foreach(PrintingSystemCommand command in commands)
				if(!IsNoneVisibleCommand(command))
					return command;
			return PrintingSystemCommand.None;
		}
		#endregion
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			initializingCounter++;
		}
		void ISupportInitialize.EndInit() {
			if(initializingCounter != 0)
				initializingCounter--;
		}
		#endregion
		public void HighlightIntersectedBricks(Color backColor) {
			new IntersectedBricksHelper(this).HighlightIntersectedBricks(backColor);
		}
		public ICollection GetIntersectedBricks() {
			return new IntersectedBricksHelper(this).IntersectedBricks;
		}
		#region DEBUG
#if DEBUGTEST
		public void AssertIntersectedBricks() {
			IntersectedBricksHelper helper = new IntersectedBricksHelper(this);
			helper.HighlightIntersectedBricks(DXColor.Red);
			System.Diagnostics.Debug.Assert(helper.IntersectedBricks.Count == 0, "There are " + helper.IntersectedBricks.Count + " intersected bricks in the document");
		}
#endif // DEBUG
		#endregion
		void BeforeExport() {
			RemoveService(typeof(IBrickPublisher), true);
			AddService(typeof(IBrickPublisher), new ExportBrickPublisher(), true);
		}
		void AfterExport() {
			RemoveService(typeof(IBrickPublisher), true);
			this.GarbageImages.Clear();
		}
		#region Export to XPS
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ExportToXps(string filePath, XpsExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			using(FileStream stream = new FileStream(filePath, FileMode.Create)) {
				ExportToXps(stream, options);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ExportToXps(Stream stream, XpsExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			XpsExportServiceBase xpsExportService = ((IServiceProvider)this).GetService(typeof(XpsExportServiceBase)) as XpsExportServiceBase;
			if(xpsExportService == null)
				throw new NotSupportedException();
			BeforeExport();
			try {
				xpsExportService.Export(stream, options, ProgressReflector);
			} finally {
				AfterExport();
			}
		}
		#endregion
#if SL
		public void ExportToText(Stream stream, TextExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToText(string filename, TextExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToCsv(Stream stream, CsvExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToCsv(string filename, CsvExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToRtf(Stream stream, RtfExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToRtf(string filename, RtfExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			throw new NotImplementedException();
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			throw new NotImplementedException();
		}
		internal string[] ExportToHtmlInternal(string filePath, HtmlExportOptions options) {
			throw new NotImplementedException();
		}
		internal string[] ExportToMhtInternal(string filePath, MhtExportOptions options) {
			throw new NotImplementedException();
		}
		public void SaveDocument(string filename) {
			throw new NotImplementedException();
		}
		internal string[] ExportToXlsInternal(string filePath, XlsExportOptions options) {
			throw new NotImplementedException();
		}
		internal string[] ExportToXlsxInternal(string filePath, XlsxExportOptions options) {
			throw new NotImplementedException();
		}
		internal string[] ExportToImageInternal(string filePath, ImageExportOptions options) {
			throw new NotImplementedException();
		}
		void DisposeRichTextBoxForDrawing() {
		}
		protected internal virtual void PrintDocument(PrintDocument pd) {
			try {
				try {
					SetCursor(CursorHelper.Wait);
					pd.Print(string.Empty);
				}
				finally {
					RestoreCursor();
				}
			} catch (SecurityException ex) {
				throw ex;
			}
		}
		public virtual IBrick CreateBrick(string typeName) {
			switch (typeName) {
				case "TextBrick":
					return new TextBrick();
				default:
					object obj = Activator.CreateInstance(null, "DevExpress.XtraPrinting." + typeName);
					return (IBrick)obj;
			}
		}
#else
		internal static bool IsVisible(Control control) {
			return control != null && control.Visible;
		}
		ExportersFactory exportersFactory;
		RichTextBox richTextBoxForDrawing;
		internal RichTextBox RichTextBoxForDrawing {
			get {
				if(richTextBoxForDrawing == null) {
					richTextBoxForDrawing = new RichTextBoxEx();
					PSNativeMethods.ForceCreateHandle(richTextBoxForDrawing);
				}
				return richTextBoxForDrawing;
			}
		}
		[Browsable(false)]
		internal ExportersFactory ExportersFactory {
			get {
				if(exportersFactory == null)
					exportersFactory = new ExportersFactory();
				return exportersFactory;
			}
		}
#endif
		public ExportersFactory TmpExportersFactory {
			get {
				return ExportersFactory;
			}
		}
		public void AddCommandHandler(ICommandHandler handler) {
			Extender.AddCommandHandler(handler);
		}
		internal void EnableCommandInternal(PrintingSystemCommand command, bool enabled) {
			Extender.EnableCommand(command, enabled);
		}
		public void SetCommandVisibility(PrintingSystemCommand command, CommandVisibility visibility) {
			SetCommandVisibility(command, visibility, Priority.High);
		}
		public void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility) {
			SetCommandVisibility(commands, visibility, Priority.High);
		}
		internal void SetCommandVisibility(PrintingSystemCommand command, CommandVisibility visibility, Priority priority) {
			SetCommandVisibility(new PrintingSystemCommand[] { command }, visibility, priority);
		}
		internal void SetCommandVisibility(PrintingSystemCommand[] commands, CommandVisibility visibility, Priority priority) {
			Extender.SetCommandVisibility(commands, visibility, priority);
		}
#if !SL
		void DisposeRichTextBoxForDrawing() {
			if(richTextBoxForDrawing != null) {
				richTextBoxForDrawing.Dispose();
				richTextBoxForDrawing = null;
			}
		}
		protected internal virtual void PrintDocument(PrintDocument pd) {
			BeforeExport();
			try {
				pd.Print();
			} finally {
				AfterExport();
			}
		}
		public void ExecCommand(PrintingSystemCommand command, object[] args) {
			Extender.ExecCommand(command, args);
		}
		public void ExecCommand(PrintingSystemCommand command) {
			ExecCommand(command, new object[] { });
		}
		protected void SetCommandVisibility(PrintingSystemCommand command, bool visible) {
			CommandVisibility visibility = visible ? CommandVisibility.All : CommandVisibility.None;
			SetCommandVisibility(command, visibility, Priority.Low);
		}
		void IPrintingSystem.SetCommandVisibility(PrintingSystemCommand command, bool visible) {
			SetCommandVisibility(command, visible);
		}
		public void RemoveCommandHandler(ICommandHandler handler) {
			Extender.RemoveCommandHandler(handler);
		}
		[Obsolete("Use the SetCommandVisibility(PrintingSystemCommand command, CommandVisibility visibility) method instead")]
		public void EnableCommand(PrintingSystemCommand command, bool enabled) {
			throw new MethodAccessException();
		}
		public virtual IBrick CreateBrick(string typeName) {
			switch(typeName) {
				case "TextBrick":
					return new TextBrick();
				default:
					ObjectHandle objH = Activator.CreateInstance(null, "DevExpress.XtraPrinting." + typeName);
					object obj = objH.Unwrap();
					return (IBrick)obj;
			}
		}
#endif
#if !SL
		#region save load preview
		public void SaveDocument(string filePath) {
			SaveDocument(filePath, ExportOptions.NativeFormat);
		}
		public void SaveDocument(string filePath, NativeFormatOptions options) {
			FileExportHelper.Do(filePath, stream => SaveDocument(stream, options));
		}
		public void LoadDocument(string filePath) {
			using(Stream stream = CreateFileStreamForRead(filePath)) {
				LoadDocument(stream);
			}
		}
		static Stream CreateFileStreamForRead(string filePath) {
			return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}
		internal void LoadVirtualDocument(string filePath) {
			LoadVirtualDocumentCore(CreateFileStreamForRead(filePath), true);
		}
		internal void LoadVirtualDocument(Stream stream) {
			LoadVirtualDocumentCore(stream, false);
		}
		void LoadVirtualDocumentCore(Stream stream, bool disposeStream) {
			PrepareLoading();
			SetDocument(new VirtualDocument(this, stream, disposeStream));
		}
		#endregion
		#region Export to Xls
		public void ExportToXls(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToXls(filePath, exportOptions.Xls);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExportToXlsInternal(filePath, options);
		}
		public void ExportToXls(Stream stream) {
			ExportToXls(stream, exportOptions.Xls);
		}
		public virtual void ExportToXls(Stream stream, XlsExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, XlsExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			BeforeExport();
			try {
				if(PrintingSettings.NewExcelExport) {
					XlsExportContext exportContext = new XlsExportContext(this, options);
					ProgressMaster progressMaster = (ProgressMaster)serviceContainer.GetService(typeof(ProgressMaster)) ?? new ProgressMaster(ProgressReflector, options.ExportMode);
					NewExcelExportProvider newProvider = new NewExcelExportProvider(stream, exportContext, NewExcelExportProvider.ExportFileType.Xls, progressMaster);
					newProvider.CreateDocument(Document);
				} else
					using(XlsExportProviderBase provider = CreateXlsExportProvider(stream, options))
						provider.CreateDocument(Document);
			} finally {
				AfterExport();
			}
		}
		protected XlsExportProviderBase CreateXlsExportProvider(Stream stream, XlsExportOptions options) {
			if(options.ExportMode == XlsExportMode.SingleFile && options.SinglePageForDifferentFiles)
				return new XlsFilePageExportProvider(stream, new XlsExportContext(this, options));
			return new XlsExportProvider(stream, new XlsExportContext(this, options));
		}
		internal string[] ExportToXlsInternal(string filePath, XlsExportOptions options) {
			exportOptions.Xls.ExportMode = options.ExportMode;
			try {
				AddService(typeof(ProgressMaster), new ProgressMaster(ProgressReflector, options.ExportMode));
				if(options.ExportMode == XlsExportMode.DifferentFiles) {
					XlsExportOptions tempOptions = (XlsExportOptions)ExportOptionsHelper.CloneOptions(options);
					tempOptions.ExportMode = XlsExportMode.SingleFile;
					tempOptions.SinglePageForDifferentFiles = true;
					ProgressReflector.SetProgressRanges(new float[] { 1 });
					PagesExportHelper exportHleper = new PagesExportHelper(this, tempOptions);
					return exportHleper.Execute(exportHleper.PageCount, filePath, stream => {
						ExportToXls(stream, tempOptions);
						ProgressReflector.RangeValue++;
					});
				} else {
					ProgressReflector.SetProgressRanges(new float[] { 1, 4 });
					FileExportHelper.Do(filePath, stream => ExportToXls(stream, options));
					return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
				}
			} finally {
				RemoveService(typeof(ProgressMaster));
			}
		}
		#endregion
		#region Export to Xlsx
		public void ExportToXlsx(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToXlsx(filePath, exportOptions.Xlsx);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExportToXlsxInternal(filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToXlsx(stream, exportOptions.Xlsx);
		}
		public virtual void ExportToXlsx(Stream stream, XlsxExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, XlsxExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			BeforeExport();
			try {
				if(PrintingSettings.NewExcelExport) {
					XlsExportContext exportContext = new XlsExportContext(this, options);
					ProgressMaster progressMaster = (ProgressMaster)serviceContainer.GetService(typeof(ProgressMaster)) ?? new ProgressMaster(ProgressReflector, options.ExportMode);
					NewExcelExportProvider newProvider = new NewExcelExportProvider(stream, exportContext, NewExcelExportProvider.ExportFileType.Xlsx, progressMaster);
					newProvider.CreateDocument(Document);
				}
				else			 
					using(XlsExportProviderBase provider = CreateXlsxExportProvider(stream, options))
						provider.CreateDocument(Document);
			} finally {				
				AfterExport();
			}
		}
		protected XlsxExportProvider CreateXlsxExportProvider(Stream stream, XlsxExportOptions options) {
			if(options.ExportMode == XlsxExportMode.SingleFilePageByPage)
				return new XlsxPageExportProvider(stream, new XlsExportContext(this, options));
			else
				return new XlsxExportProvider(stream, new XlsExportContext(this, options));
		}
		internal string[] ExportToXlsxInternal(string filePath, XlsxExportOptions options) {
			Guard.ArgumentNotNull(options, "options");
			exportOptions.Xlsx.ExportMode = options.ExportMode;
			try {
				AddService(typeof(ProgressMaster), new ProgressMaster(ProgressReflector, options.ExportMode));
				if(options.ExportMode == XlsxExportMode.DifferentFiles) {
					XlsxExportOptions tempOptions = (XlsxExportOptions)ExportOptionsHelper.CloneOptions(options);
					tempOptions.ExportMode = XlsxExportMode.SingleFilePageByPage;
					ProgressReflector.SetProgressRanges(new float[] { 1 });
					PagesExportHelper exportHelper = new PagesExportHelper(this, tempOptions);
					return exportHelper.Execute(exportHelper.PageCount, filePath, stream =>
					{
						ExportToXlsx(stream, tempOptions);
						ProgressReflector.RangeValue++;
					});
				} else if(options.ExportMode == XlsxExportMode.SingleFile) {
					ProgressReflector.SetProgressRanges(new float[] { 1, 4 });
					FileExportHelper.Do(filePath, stream => ExportToXlsx(stream, options));
					return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
				} else {
					FileExportHelper.Do(filePath, stream => ExportToXlsx(stream, options));
					return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
				}
			} finally {
				RemoveService(typeof(ProgressMaster));
			}
		}
		#endregion
		#region Export to Rtf
		public void ExportToRtf(string filePath, RtfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			FileExportHelper.Do(filePath, stream => ExportToRtf(stream, options));
		}
		public void ExportToRtf(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToRtf(filePath, exportOptions.Rtf);
		}
		public void ExportToRtf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToRtf(stream, exportOptions.Rtf);
		}
		public virtual void ExportToRtf(Stream stream, RtfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			BeforeExport();
			try {
				CreateRtfExportProvider(stream, options).CreateDocument();
			} finally {
				AfterExport();
			}
		}
		protected RtfDocumentProviderBase CreateRtfExportProvider(Stream stream, RtfExportOptions options) {
			if(options.ExportMode == RtfExportMode.SingleFilePageByPage)
				return new RtfPageExportProvider(stream, Document, options);
			return new RtfExportProvider(stream, Document, options);
		}
		#endregion
		#region Export to Html
		protected internal HtmlTableBuilder CreateHtmlTableBuilder() {
			return new HtmlTableBuilder();
		}
		public void ExportToHtml(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToHtml(filePath, exportOptions.Html);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExportToHtmlInternal(filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToHtml(stream, exportOptions.Html);
		}
		public virtual void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, HtmlExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			HtmlDocumentBuilder builder = CreateHtmlDocumentBuilder(options);
			IImageRepository imageRepository = builder.CreateImageRepository(stream);
			BeforeExport();
			try {
				builder.CreateDocument(Document, stream, imageRepository, options.ExportMode);
			} finally {
				imageRepository.Dispose();
				AfterExport();
			}
		}
		HtmlDocumentBuilder CreateHtmlDocumentBuilder(HtmlExportOptions options) {
			return new HtmlDocumentBuilder(options, brickGraphics.PageBackColor);
		}
		internal string[] ExportToHtmlInternal(string filePath, HtmlExportOptions options) {
			exportOptions.Html.ExportMode = options.ExportMode;
			if(options.ExportMode == HtmlExportMode.DifferentFiles) {
				IImageRepository commonRepository = HtmlDocumentBuilder.CreatePSImageRepository(filePath, options.EmbedImagesInHTML);
				HtmlExportOptions tempOptions = (HtmlExportOptions)ExportOptionsHelper.CloneOptions(options);
				tempOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
				PagesExportHelper exportHelper = new PagesExportHelper(this, tempOptions);
				BeforeExport();
				try {
					return exportHelper.Execute(2 * exportHelper.PageCount, filePath, stream => {
						tempOptions.Title = PagesExportHelper.GetStringWithPageIndex(options.Title, tempOptions.PageRange, PageCount);
						HtmlDocumentBuilder builder = CreateHtmlDocumentBuilder(tempOptions);
						builder.CreateDocument(Document, stream, commonRepository, options.ExportMode);
					});
				} finally {
					commonRepository.Dispose();
					AfterExport();
				}
			} else if(options.ExportMode == HtmlExportMode.SingleFilePageByPage) {
				int[] pageIndices = options.GetPageIndices(PageCount);
				ProgressReflector.Do(2 * pageIndices.Length, () => {
					FileExportHelper.Do(filePath, stream => ExportToHtml(stream, options));
				});
			} else {
				ProgressReflector.SetProgressRanges(new float[] { 1, 1 });
				ProgressReflector.Do(2, () =>
				{
					FileExportHelper.Do(filePath, stream => ExportToHtml(stream, options));
				});
			}
			return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
		}
		#endregion
		#region Export to Mht
		public void ExportToMht(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToMht(filePath, exportOptions.Mht);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExportToMhtInternal(filePath, options);
		}
		public virtual void ExportToMht(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToMht(stream, exportOptions.Mht);
		}
		public virtual void ExportToMht(Stream stream, MhtExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, HtmlExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			MhtDocumentBuilder builder = new MhtDocumentBuilder(options, brickGraphics.PageBackColor);
			IImageRepository imageRepository = builder.CreateImageRepository(stream);
			BeforeExport();
			try {
				builder.CreateDocument(Document, stream, imageRepository, options.ExportMode);
			} finally {
				imageRepository.Dispose();
				AfterExport();
			}
		}
		internal string[] ExportToMhtInternal(string filePath, MhtExportOptions options) {
			exportOptions.Mht.ExportMode = options.ExportMode;
			if(options.ExportMode == HtmlExportMode.DifferentFiles) {
				MhtExportOptions tempOptions = (MhtExportOptions)ExportOptionsHelper.CloneOptions(options);
				tempOptions.ExportMode = HtmlExportMode.SingleFilePageByPage;
				PagesExportHelper exportHleper = new PagesExportHelper(this, tempOptions);
				return exportHleper.Execute(2 * exportHleper.PageCount, filePath, stream => {
					tempOptions.Title = PagesExportHelper.GetStringWithPageIndex(options.Title, tempOptions.PageRange, PageCount);
					ExportToMht(stream, tempOptions);
				});
			} else if(options.ExportMode == HtmlExportMode.SingleFilePageByPage) {
				int[] pageIndices = options.GetPageIndices(PageCount);
				ProgressReflector.Do(2 * pageIndices.Length, () => {
					FileExportHelper.Do(filePath, stream => ExportToMht(stream, options));
				});
			} else {
				ProgressReflector.SetProgressRanges(new float[] { 1, 1 });
				ProgressReflector.Do(2, () =>
				{
					FileExportHelper.Do(filePath, stream => ExportToMht(stream, options));
				});
			}
			return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
		}	   
		#endregion
		#region Export to Mail
		public AlternateView ExportToMail(MailMessageExportOptions options) {
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, HtmlExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			MailDocumentBuilder builder = new MailDocumentBuilder(options);
			IImageRepository imageRepository = builder.CreateImageRepository(null);
			BeforeExport();
			try {
				return builder.CreateDocument(Document, imageRepository, options.ExportMode);
			} finally {
				imageRepository.Dispose();
				AfterExport();
			}
		}
		public AlternateView ExportToMail() {
			return ExportToMail(exportOptions.MailMessage);
		}
		public MailMessage ExportToMail(MailMessageExportOptions options, string from, string to, string subject) {
			MailMessage message = new MailMessage(from, to, subject, string.Empty);
			message.AlternateViews.Add(ExportToMail(options));
			return message;
		}
		public MailMessage ExportToMail(string from, string to, string subject) {
			MailMessage message = new MailMessage(from, to, subject, string.Empty);
			message.AlternateViews.Add(ExportToMail(exportOptions.MailMessage));
			return message;
		}
		#endregion
		#region Export to Text
		public void ExportToText(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToText(filePath, exportOptions.Text);
		}
		public void ExportToText(string filePath, TextExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			FileExportHelper.Do(filePath, stream => ExportToText(stream, options));
		}
		public void ExportToText(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToText(stream, exportOptions.Text);
		}
		public virtual void ExportToText(Stream stream, TextExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CreateTextDocument(stream, options, true);
		}
		void CreateTextDocument(Stream stream, TextExportOptionsBase options, bool insertSpaces) {
			ProgressReflector.SetProgressRanges(new float[] { 1, 1 });
			StreamWriter writer = new StreamWriter(stream, options.Encoding);
			BeforeExport();
			try {
				ProgressReflector.InitializeRange(5);
				TextExportContext exportContext = new TextExportContext(this);
				LayoutControlCollection layoutControls = null;
				ProgressReflector.EnsureRangeDecrement(() => layoutControls = CreateTextLayoutBuilder(exportContext, options).BuildLayoutControls());
				ProgressReflector.RangeValue++;
				Export.Text.TextDocument.CreateDocument(layoutControls, Document, writer, options, insertSpaces);
			} finally {
				ProgressReflector.MaximizeRange();
				writer.Flush();
				AfterExport();
			}
		}
		#endregion
		#region Export to Csv
		public void ExportToCsv(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToCsv(filePath, exportOptions.Csv);
		}
		public void ExportToCsv(string filePath, CsvExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			FileExportHelper.Do(filePath, stream => ExportToCsv(stream, options));
		}
		public void ExportToCsv(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToCsv(stream, exportOptions.Csv);
		}
		public virtual void ExportToCsv(Stream stream, CsvExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			BeforeExport();
			try {
				CreateTextDocument(stream, options, false);
			} finally {
				AfterExport();
			}
		}
		#endregion
		#region Export to Image
		public void ExportToImage(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToImage(filePath, exportOptions.Image);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			ExportToImageInternal(filePath, options);
		}
		public void ExportToImage(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToImage(stream, exportOptions.Image);
		}
		public virtual void ExportToImage(Stream stream, ImageExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			CheckExportMode(options.ExportMode, ImageExportMode.DifferentFiles, PreviewStringId.Msg_NoDifferentFilesInStream);
			if(exportOptions.Image.ExportMode == ImageExportMode.SingleFilePageByPage)
				ProgressReflector.InitializeRange(ExportOptionsHelper.GetPageIndices(options, Document.PageCount).Length);
			ImageDocumentBuilderBase builder = options.ExportMode == ImageExportMode.SingleFile ?
				(ImageDocumentBuilderBase)new ImageSinglePageDocumentBuilder(this, options) :
				new ImageSinglePageByPageDocumentBuilder(this, options);
			BeforeExport();
			try {
				builder.CreateDocument(stream);
			} finally {
				AfterExport();
			}
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(format, "format");
			ExportToImage(filePath, ExportOptionsHelper.ChangeOldImageProperties(exportOptions.Image, format));
		}
		public virtual void ExportToImage(Stream stream, ImageFormat format) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(format, "format");
			ExportToImage(stream, ExportOptionsHelper.ChangeOldImageProperties(exportOptions.Image, format));
		}
		internal string[] ExportToImageInternal(string filePath, ImageExportOptions options) {
			exportOptions.Image.ExportMode = options.ExportMode;
			if(options.ExportMode == ImageExportMode.DifferentFiles) {
				ImageExportOptions tempOptions = (ImageExportOptions)ExportOptionsHelper.CloneOptions(options);
				tempOptions.ExportMode = ImageExportMode.SingleFilePageByPage;
				PagesExportHelper exportHelper = new PagesExportHelper(this, tempOptions);
				return exportHelper.Execute(exportHelper.PageCount, filePath, stream => ExportToImage(stream, tempOptions));
			} else {
				try {
					FileExportHelper.Do(filePath, stream => ExportToImage(stream, options));
				} finally {
					ProgressReflector.MaximizeRange();
				}
				return File.Exists(filePath) ? new string[] { filePath } : new string[] { };
			}
		}
		#endregion
		#region Export to Pdf
		public void ExportToPdf(string filePath) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			ExportToPdf(filePath, ExportOptions.Pdf);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Guard.ArgumentIsNotNullOrEmpty(filePath, "filePath");
			Guard.ArgumentNotNull(options, "options");
			FileExportHelper.Do(filePath, stream => ExportToPdf(stream, options));
		}
		public virtual void ExportToPdf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			ExportToPdf(stream, ExportOptions.Pdf);
		}
		public virtual void ExportToPdf(Stream stream, PdfExportOptions options) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(options, "options");
			BeforeExport();
			try {
				PdfDocumentBuilder.CreateDocument(stream, Document, options);
			} finally {
				AfterExport();
			}
		}
		#endregion
#endif
		#region XPlatform
		static PrintingSystemBase() {
			PrintingSystemXmlSerializer.RegisterConverter(PaddingInfoConverter.Instance);
			PrintingSystemXmlSerializer.RegisterConverter(BrickStringFormatConverter.Instance);
			PrintingSystemXmlSerializer.RegisterConverter(RectangleDFConverter.Instance);
			DevExpress.XtraPrinting.Native.DrillDown.DrillDownKey.EnsureStaticConstructor();
		}
		static Stream GetStreamInstance(bool compressed, Stream baseStream, CompressionMode compressionMode) {
			return compressed
#if !SL
				? new GZipStream(baseStream, compressionMode, true)
#else
				? new DeflateStream(baseStream, compressionMode, true, true)
#endif
				: baseStream;
		}
		public void LoadDocument(Stream stream) {
			try {
				PrepareLoading();
				SetDocument(new DeserializedDocument(this));
				stream.Seek(0, SeekOrigin.Begin);
				if(DeflateStreamsArchiveReader.IsValidStream(stream)) {
					Document.Deserialize(stream, CreateIndependentPagesSerializer());
					return;
				}
				stream.Seek(0, SeekOrigin.Begin);
				const string name = "?xml";
				byte[] bytes = new byte[name.Length + 4];
				stream.Read(bytes, 0, bytes.Length);
				StringBuilder builder = new StringBuilder();
				for(int i = 0; i < bytes.Length; i++) {
					builder.Append((char)bytes[i]);
				}
				stream.Seek(0, SeekOrigin.Begin);
				bool compressed = !builder.ToString().Contains(name);
				stream = GetStreamInstance(compressed, stream, CompressionMode.Decompress);
				try {
					Document.Deserialize(stream, CreateNormalSerializer());
				} finally {
					CloseStream(compressed, stream);
				}
			} catch {
				throw new InvalidDataException(PreviewLocalizer.GetString(PreviewStringId.Msg_CannotLoadDocument));
			}
		}
		void PrepareLoading() {
			FinalizeLink(activeLink);
			DisposeDocument();
			ClearWaterMark();
			PageSettings.AssignDefaultPageSettings();
		}
		public void SaveDocument(Stream stream) {
			SaveDocument(stream, ExportOptions.NativeFormat);
		}
		public void SaveDocument(Stream stream, NativeFormatOptions options) {
			stream = GetStreamInstance(options.Compressed, stream, CompressionMode.Compress);
			Document.Serialize(stream, CreateNormalSerializer());
			CloseStream(options.Compressed, stream);
		}
		internal void SaveIndependentPages(Stream stream) {
			Document.Serialize(stream, CreateIndependentPagesSerializer());
		}
		#endregion
	}
}
