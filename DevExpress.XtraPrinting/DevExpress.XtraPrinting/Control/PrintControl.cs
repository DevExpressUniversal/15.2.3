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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DocumentView;
using DevExpress.DocumentView.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraPrinting.Control.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Preview.Native;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.Utils.Drawing.Helpers;
using System.Runtime.InteropServices;
namespace DevExpress.XtraPrinting.Control {
	[
	ToolboxItem(false)
	]
	public class PrintControl : DocumentViewerBase, IPrintControl {
		ProgressReflector IPrintControl.ProgressReflector {
			get { return ProgressReflector; }
		}
		CommandSetBase IPrintControl.CommandSet {
			get { return CommandSet; }
		}
#if DEBUGTEST
		[Browsable(false)]
		public DockPanelController Test_GetController(PrintingSystemCommand command) {
			return GetControllerByCommand(command);
		}
#endif
		#region events
		private static readonly object BrickMouseDownEvent = new object();
		private static readonly object BrickMouseUpEvent = new object();
		private static readonly object BrickMouseMoveEvent = new object();
		private static readonly object BrickClickEvent = new object();
		private static readonly object BrickDoubleClickEvent = new object();
		private static readonly object DockManagerCreatedEvent = new object();
		private static readonly object CommandExecuteEvent = new object();
		private static readonly object CommandChangedEvent = new object();
		private static readonly object PaintMarkedBricksAreaEvent = new object();
		[Category(NativeSR.CatAppearance)]
		[Obsolete("The PaintBookmarkArea event is obsolete now. Use the PaintMarkedBricksArea event instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler<AreaPaintEventArgs> PaintBookmarkArea {
			add { Events.AddHandler(PaintMarkedBricksAreaEvent, value); }
			remove { Events.RemoveHandler(PaintMarkedBricksAreaEvent, value); }
		}
		[Category(NativeSR.CatAppearance)]
		public event EventHandler<AreaPaintEventArgs> PaintMarkedBricksArea {
			add { Events.AddHandler(PaintMarkedBricksAreaEvent, value); }
			remove { Events.RemoveHandler(PaintMarkedBricksAreaEvent, value); }
		}
		bool RaisePaintMarkedBricksArea(Graphics gr, RectangleF rect) {
			EventHandler<AreaPaintEventArgs> eventDelegate = (EventHandler<AreaPaintEventArgs>)Events[PaintMarkedBricksAreaEvent];
			if(eventDelegate != null) {
				GraphicsState state = gr.Save();
				gr.PageUnit = GraphicsUnit.Display;
				try {
					eventDelegate(this, new AreaPaintEventArgs(gr, XRConvert.Convert(rect, GraphicsDpi.Document, GraphicsDpi.Pixel)));
				} finally {
					gr.Restore(state);
				}
				return true;
			}
			return false;
		}
		public event EventHandler CommandChanged {
			add { Events.AddHandler(CommandChangedEvent, value); }
			remove { Events.RemoveHandler(CommandChangedEvent, value); }
		}
		internal event CommandExecuteEventHandler CommandExecute {
			add { Events.AddHandler(CommandExecuteEvent, value); }
			remove { Events.RemoveHandler(CommandExecuteEvent, value); }
		}
		internal event EventHandler DockManagerCreated {
			add { Events.AddHandler(DockManagerCreatedEvent, value); }
			remove { Events.RemoveHandler(DockManagerCreatedEvent, value); }
		}
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickMouseDown {
			add { Events.AddHandler(BrickMouseDownEvent, value); }
			remove { Events.RemoveHandler(BrickMouseDownEvent, value); }
		}
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickMouseUp {
			add { Events.AddHandler(BrickMouseUpEvent, value); }
			remove { Events.RemoveHandler(BrickMouseUpEvent, value); }
		}
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickMouseMove {
			add { Events.AddHandler(BrickMouseMoveEvent, value); }
			remove { Events.RemoveHandler(BrickMouseMoveEvent, value); }
		}
		[Obsolete("The BrickDown event is obsolete now. Use the BrickMouseDown event instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickDown {
			add { Events.AddHandler(BrickMouseDownEvent, value); }
			remove { Events.RemoveHandler(BrickMouseDownEvent, value); }
		}
		[Obsolete("The BrickUp event is obsolete now. Use the BrickMouseUp event instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickUp {
			add { Events.AddHandler(BrickMouseUpEvent, value); }
			remove { Events.RemoveHandler(BrickMouseUpEvent, value); }
		}
		[Obsolete("The BrickMove event is obsolete now. Use the BrickMouseMove event instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickMove {
			add { Events.AddHandler(BrickMouseMoveEvent, value); }
			remove { Events.RemoveHandler(BrickMouseMoveEvent, value); }
		}
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickClick {
			add { Events.AddHandler(BrickClickEvent, value); }
			remove { Events.RemoveHandler(BrickClickEvent, value); }
		}
		[Category(NativeSR.CatAction)]
		public event BrickEventHandler BrickDoubleClick {
			add { Events.AddHandler(BrickDoubleClickEvent, value); }
			remove { Events.RemoveHandler(BrickDoubleClickEvent, value); }
		}
		void OnCommandChanged(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[CommandChangedEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void OnCommandExecute(CommandExecuteEventArgs e) {
			CommandExecuteEventHandler eventDelegate = (CommandExecuteEventHandler)Events[CommandExecuteEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		void OnDockManagerCreated(EventArgs e) {
			EventHandler eventDelegate = (EventHandler)Events[DockManagerCreatedEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		protected override void OnBrickMouseDown(MouseEventArgs e) {
			BrickEventHandler eventDelegate = (BrickEventHandler)Events[BrickMouseDownEvent];
			BrickEventArgs be = CreateBrickEventArgs(e);
			if(eventDelegate != null)
				eventDelegate(this, be);
			PrintingSystem.OnAfterChange(CreateEventArgs(SR.BrickMouseDown, be));
		}
		protected override void OnBrickMouseUp(MouseEventArgs e) {
			BrickEventHandler eventDelegate = (BrickEventHandler)Events[BrickMouseUpEvent];
			BrickEventArgs be = CreateBrickEventArgs(e);
			if(eventDelegate != null)
				eventDelegate(this, be);
			PrintingSystem.OnAfterChange(CreateEventArgs(SR.BrickMouseUp, be));
		}
		protected override void OnBrickMouseMove(MouseEventArgs e) {
			BrickEventHandler eventDelegate = (BrickEventHandler)Events[BrickMouseMoveEvent];
			BrickEventArgs be = CreateBrickEventArgs(e);
			if(eventDelegate != null)
				eventDelegate(this, be);
			PrintingSystem.OnAfterChange(CreateEventArgs(SR.BrickMouseMove, be));
		}
		protected override void OnBrickClick(EventArgs e) {
			BrickEventHandler eventDelegate = (BrickEventHandler)Events[BrickClickEvent];
			BrickEventArgs be = CreateBrickEventArgs(e);
			if(eventDelegate != null)
				eventDelegate(this, be);
			PrintingSystem.OnAfterChange(CreateEventArgs(SR.BrickClick, be));
		}
		protected override void OnBrickDoubleClick(EventArgs e) {
			BrickEventHandler eventDelegate = (BrickEventHandler)Events[BrickDoubleClickEvent];
			BrickEventArgs be = CreateBrickEventArgs(e);
			if(eventDelegate != null)
				eventDelegate(this, be);
			PrintingSystem.OnAfterChange(CreateEventArgs(SR.BrickDoubleClick, be));
		}
		protected ChangeEventArgs CreateEventArgs(string eventName, BrickEventArgs e) {
			return PrintingSystemBase.CreateEventArgs(eventName, new object[,] { 
				{SR.Brick, e.Brick}, {SR.X, e.X}, {SR.Y, e.Y}, {SR.MouseButtons, (e.Args is MouseEventArgs) ? ((MouseEventArgs)e.Args).Button : MouseButtons.None},
				{SR.PrintControl, this}, {SR.PreviewControl, ViewControl}, {SR.Page, e.Page}, {SR.BrickScreenBounds, e.BrickScreenBounds} });
		}
		#endregion
		IPrintingSystemExtender psExtender;
		BrickHandler brickHandler;
		PSCommandHandler commandHandler;
		Dictionary<Type, object[]> disabledExportModes;
		ParametersController parametersController;
		NavigationController navigationController;
		ThumbnailsController thumbnailsController;
		PanelHelper panelHelper = new PanelHelper();
		BarAndDockingController controller = new BarAndDockingController();
		DockManager dockManager;
		DevExpress.XtraBars.Docking2010.DocumentManager documentManager;
		CommandSet commandSet = new CommandSet();
		ProgressReflector reflectorBar;
		DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility verticalScrollBarVisibility;
		DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility horizontalScrollBarVisibility;
		SelectionService selectionService;
		SelectionMessageHandler selectionMessageHandler;
		Pen markPen;
		ImageCollection imageCollection;
		FindPanel findPanel;
		FindPanel FindPanel {
			get {
				if(findPanel == null) {
					findPanel = new FindPanel(this, selectionService) { Dock = DockStyle.Top, Height = 0, Enabled = false, BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder, Padding = new Padding(6), Margin = new Padding(0) };
					Controls.Add(findPanel);
					findPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					findPanel.RequestClose += findPanel_Close;
				}
				return findPanel;
			}
		}
		internal Dictionary<Type, object[]> DisabledExportModes { 
			get { return disabledExportModes; }
		}
		Pen MarkPen {
			get {
				if(markPen == null)
					markPen = CreateMarkPen();
				return markPen;
			}
		}
		static Pen CreateMarkPen() {
			using(Bitmap image = ResourceImageHelper.CreateBitmapFromResources("Core.Images.MarkBrush.bmp", typeof(DevExpress.Printing.ResFinder))) {
				image.MakeTransparent(DXColor.Magenta);
				using(Brush brush = new TextureBrush(image)) {
					return new Pen(brush, System.Math.Max(image.Width, image.Height));
				}
			}
		}
		[Browsable(false)]
		public bool DocumentHasBookmarks {
			get { return PrintingSystem != null && PrintingSystem.Document.BookmarkNodes.Count > 0; }
		}
		protected internal ProgressReflector ProgressReflector {
			get { return reflectorBar; }
			set { reflectorBar = value; }
		}
		protected override bool CanChangePageMargins {
			get {
				CommandSetItem item = CommandSet[PrintingSystemCommand.PageMargins];
				return item != null && item.Enabled && item.Visibility != CommandVisibility.None;
			}
		}
		[Browsable(false)]
		public bool DocumentMapVisible {
			get { return navigationController.PanelVisible; }
		}
		[Browsable(false)]
		public bool ThumbnailsVisible {
			get { return thumbnailsController.PanelVisible; }
		}
		internal CommandSet CommandSet {
			get { return commandSet; }
		}
		[Browsable(false)]
		public DockManager DockManager {
			get { return dockManager; }
		}
		internal SelectionService SelectionService {
			get { return selectionService; }
		}
		internal bool HasParameters {
			set {
				parametersController.HasParameters = value;
				UpdateCommands();
			}
		}
		[DefaultValue(XtraEditors.ViewInfo.ScrollBarVisibility.Visible)]
		public DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility VerticalScrollBarVisibility {
			get { return verticalScrollBarVisibility; }
			set {
				this.vScrollBar.Visible = this.Visible && value == DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility.Visible;
				verticalScrollBarVisibility = value;
			}
		}
		[DefaultValue(XtraEditors.ViewInfo.ScrollBarVisibility.Visible)]
		public DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility HorizontalScrollBarVisibility {
			get { return horizontalScrollBarVisibility; }
			set {
				this.hScrollBar.Parent.Visible = this.Visible && value == DevExpress.XtraEditors.ViewInfo.ScrollBarVisibility.Visible;
				horizontalScrollBarVisibility = value;
			}
		}
		[
		Category(NativeSR.CatPrinting),
		DefaultValue(null)
		]
		public virtual PrintingSystemBase PrintingSystem {
			get {
				return (PrintingSystemBase)Document;
			}
			set {
				Document = value;
				var serviceContainer = Document as System.ComponentModel.Design.IServiceContainer;
				if(serviceContainer != null && serviceContainer.GetService<IPrintControlActionInvoker>() == null)
					serviceContainer.AddService(typeof(IPrintControlActionInvoker), new PrintControlActionInvoker(this));
			}
		}
		protected override void BeforeSetDocument() {
			base.BeforeSetDocument();
			psExtender = null;
		}
		[Browsable(false)]
		public bool FindPanelVisible { get { return FindPanel.Enabled; } }
		protected override void AfterSetDocument() {
			if(Document != null) {
				UpdatePanelsVisibility();
				CommandSet.CopyFrom(PrintingSystemExtender.CommandSet);
				PrintingSystemExtender.ActiveViewer = this;
				IReport report = Document.GetService<IReport>();
				if(report != null && report.PrintTool == null)
					report.PrintTool = new ReportPrintTool(report);
			}
			base.AfterSetDocument();
			navigationController.UpdateDocumentMap();
			thumbnailsController.UpdateDocument();
		}
		IPrintingSystemExtender PrintingSystemExtender {
			get {
				if(psExtender == null && PrintingSystem != null)
					psExtender = PrintingSystem.Extend();
				return psExtender;
			}
		}
		protected override bool CanHandleClick {
			get {
				return base.CanHandleClick && !selectionService.HasSelection;
			}
		}
		public PrintControl() {
			CommandSet.CommandVisibilityChanged += new EventHandler(OnCommandVisibilityChanged);
			selectionService = new SelectionService(ViewControl);
			selectionMessageHandler = new SelectionMessageHandler(ViewControl, selectionService);
			commandHandler = new PSCommandHandler(this, selectionService);
			brickHandler = new BrickHandler(this);
			disabledExportModes = new Dictionary<Type, object[]>(){
				 {typeof(RtfExportMode), new object[] {}},
				 {typeof(HtmlExportMode), new object[] {}},
				 {typeof(XlsExportMode), new object[] {}},
				 {typeof(XlsxExportMode), new object[] {}},
				 {typeof(ImageExportMode), new object[] {}}
			};
			navigationController = new NavigationController(this);
			thumbnailsController = new ThumbnailsController(this, navigationController.DockPanel);
			parametersController = new ParametersController(this, navigationController.DockPanel);
			verticalScrollBarVisibility = XtraEditors.ViewInfo.ScrollBarVisibility.Visible;
			horizontalScrollBarVisibility = XtraEditors.ViewInfo.ScrollBarVisibility.Visible;
		}
		void findPanel_Close(object sender, EventArgs e) {
			ExecCommand(PrintingSystemCommand.Find, new object[] { false });
		}
		public void CloseFindControl() {
			FindPanel.CloseAnimation();
			Invalidate(true);
			ViewControl.Focus();
		}
		public void ShowFindControl() {
			FindPanel.ShowAnimation();
		}
		internal void FocusFindControl() {
			FindPanel.FocusFindControl();
		}
		protected override void ShowException(Exception ex) {
			if(PrintingSystem != null)
				PrintingSystem.Document.StopPageBuilding();
			NotificationService.ShowException<PrintingSystemBase>(LookAndFeel, FindForm(), ex);
		}
		void InitializeDockManager() {
			if(dockManager == null)
				dockManager = new DockManager(components);
			if(documentManager == null)
				documentManager = new DevExpress.XtraBars.Docking2010.DocumentManager(components);
			((System.ComponentModel.ISupportInitialize)(dockManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(documentManager)).BeginInit();
			imageCollection = ImageCollectionHelper.CreateVoidImageCollection();
			dockManager.Images = imageCollection;
			documentManager.ClientControl = ViewControl;
			documentManager.BarAndDockingController = controller;
			DockManager.Form = this;
			DockManager.Controller = controller;
			DockManager.TopZIndexControls.AddRange(new string[] {
				"DevExpress.XtraBars.BarDockControl",
				"System.Windows.Forms.StatusBar",
				"DevExpress.XtraBars.Ribbon.RibbonStatusBar",
				"DevExpress.XtraBars.Ribbon.RibbonControl"
				}
			);
			((System.ComponentModel.ISupportInitialize)(documentManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(DockManager)).EndInit();
			OnDockManagerCreated(EventArgs.Empty);
			dockManager.ForceInitialize();
		}
		internal DockPanel GetDockPanel(PrintingSystemCommand command) {
			DockPanelController panelController = GetControllerByCommand(command);
			return panelController != null ? panelController.DockPanel : null;
		}
		internal bool GetPanelVisibility(PrintingSystemCommand command) {
			DockPanelController panelController = GetControllerByCommand(command);
			return panelController != null ? panelController.PanelVisible : false;
		}
		internal void SetPanelVisibility(PrintingSystemCommand command, bool value) {
			DockPanelController panelController = GetControllerByCommand(command);
			if(panelController != null)
				panelController.PanelVisible = value;
		}
		DockPanelController GetControllerByCommand(PrintingSystemCommand command) {
			if(command == PrintingSystemCommand.Parameters)
				return parametersController;
			else if(command == PrintingSystemCommand.DocumentMap)
				return navigationController;
			else if(command == PrintingSystemCommand.Thumbnails)
				return thumbnailsController;
			else
				throw new ArgumentException("command");
		}
		public void SetDocumentMapVisibility(bool value) {
			SetPanelVisibility(PrintingSystemCommand.DocumentMap, value);
		}
		public void SetThumbnailsVisibility(bool value) {
			SetPanelVisibility(PrintingSystemCommand.Thumbnails, value);
		}
		protected override void UpdateEverything() {
			base.UpdateEverything();
			navigationController.UpdateDocumentMap();
			thumbnailsController.UpdateDocument();
			UpdatePanelsVisibility();
		}
		protected override void OnPageBackgrChanged(Object sender, EventArgs e) {
			base.OnPageBackgrChanged(sender, e);
			thumbnailsController.InvalidateViewer();
		}
		public void DisableExportModeValues(object[] disableValues) {
			foreach(var item in disableValues) {
				if(disabledExportModes.ContainsKey(item.GetType())) {
					object[] currentValues = disabledExportModes[item.GetType()];
					if(Array.IndexOf(currentValues, item) < 0) {
						int index = currentValues.Length;
						Array.Resize<object>(ref currentValues, index + 1);
						currentValues.SetValue(item, index);
						disabledExportModes[item.GetType()] = currentValues;
					}
				}
			}
		}
		void UpdatePanelsVisibility() {
			navigationController.UpdateVisibility();
			parametersController.UpdateVisibility();
			thumbnailsController.UpdateVisibility();
		}
		internal int[] GetBrickPath(Brick brick, Page page) {
			if(page == null || brick == null)
				return null;
			int[] path = page.GetIndicesByBrick(brick);
			int[] result = new int[path.Length + 1];
			result[0] = page.Index;
			Array.Copy(path, 0, result, 1, path.Length);
			return result;
		}
		internal virtual void ShowBrick(BrickPagePair pair) {
			Brick brick = pair.GetBrick(PrintingSystem.Pages);
			if(brick != null) {
				ShowBrick(brick, pair.GetPage(PrintingSystem.Pages));
			} else {
				PrintingSystem.EnsureBrickOnPage(pair, EnsureBrickOnPage);
			}
		}
		void EnsureBrickOnPage(BrickPagePair pair) {
			if(pair != null && pair.PageIndex < PrintingSystem.Pages.Count) {
				Brick brick = PrintingSystem.Pages[pair.PageIndex].GetBrickByIndices(pair.BrickIndices) as Brick;
				if(brick != null) {
					ShowBrick(brick, PrintingSystem.Pages[pair.PageIndex]);
					PrintingSystem.ClearMarkedBricks();
					PrintingSystem.MarkBrick(brick, pair.GetPage(PrintingSystem.Pages));
				}
			}
		}
		public void ShowBrick(Brick brick, Page page) {
			ShowBrickTop(brick, page);
		}
		public void ShowBrickCenter(Brick brick, Page page) {
			if(page == null || brick == null)
				return;
			SelectedPageIndex = page.Index;
			PointF pt = RectangleF.Intersect(page.GetBrickBounds(brick), new RectangleF(new PointF(0, 0), page.PageSize)).Location;
			pt = ViewControl.PointToClient(Point.Round(pt));
			ViewManager.ShowPagePoint(page, pt);
			InvalidateViewControl();
			UpdateScrollBars();
		}
		void ShowBrickTop(Brick brick, Page page) {
			if(page == null || brick == null)
				return;
			SelectedPageIndex = page.Index;
			PointF pt = RectangleF.Intersect(page.GetBrickBounds(brick), new RectangleF(new PointF(0, 0), page.PageSize)).Location;
			ViewManager.ShowPagePointTop(page, pt);
			InvalidateViewControl();
			UpdateScrollBars();
		}
		public void InvalidateBrick(Brick brick) {
			PageEnumerator pe = ViewManager.CreatePageEnumerator();
			while(pe.MoveNext()) {
				if(pe.Current == null)
					continue;
				RectangleF bounds = ((Page)pe.Current).GetBrickBounds(brick);
				if(bounds.IsEmpty)
					continue;
				bounds.Offset(pe.CurrentPlace.Location);
				bounds = PSUnitConverter.DocToPixel(bounds, Zoom, ViewManager.ScrollPos);
				ViewControl.InvalidateRect(bounds, false);
			}
		}
		protected override bool HandleKey(Keys key) {
			if(base.HandleKey(key))
				return true;
			switch(key) {
				case Keys.Home:
					ExecCommand(PrintingSystemCommand.ShowFirstPage);
					return true;
				case Keys.End:
					ExecCommand(PrintingSystemCommand.ShowLastPage);
					return true;
				case Keys.PageDown:
				case Keys.Right:
					ExecCommand(PrintingSystemCommand.ScrollPageDown);
					return true;
				case Keys.PageUp:
				case Keys.Left:
					ExecCommand(PrintingSystemCommand.ScrollPageUp);
					return true;
				case Keys.Add:
					ExecCommand(PrintingSystemCommand.ZoomIn);
					return true;
				case Keys.Subtract:
					ExecCommand(PrintingSystemCommand.ZoomOut);
					return true;
				case Keys.Divide:
					ExecCommand(PrintingSystemCommand.Zoom, new object[] { 1 });
					return true;
				case Keys.Multiply:
					ExecCommand(PrintingSystemCommand.ZoomToWholePage);
					return true;
				case Keys.P:
					return ExecCtrlCommand(PrintingSystemCommand.Print);
				case Keys.F:
					return ExecCtrlCommand(PrintingSystemCommand.Find, new object[] { true });
				case Keys.O:
					return ExecCtrlCommand(PrintingSystemCommand.Open);
				case Keys.S:
					return ExecCtrlCommand(PrintingSystemCommand.Save);
				case Keys.C:
					return ExecCtrlCommand(PrintingSystemCommand.Copy);
				case Keys.G:
					return ExecCtrlCommand(PrintingSystemCommand.GoToPage);
				case Keys.Escape: {
						ExecCommand(PrintingSystemCommand.Find, new object[] { false });
						return true;
					}
			}
			return false;
		}
		bool ExecCtrlCommand(PrintingSystemCommand command) {
			return ExecCtrlCommand(command, new object[] { });
		}
		bool ExecCtrlCommand(PrintingSystemCommand command, object[] args) {
			if(ModifierKeys == Keys.Control) {
				ExecCommand(command, args);
				return true;
			}
			return false;
		}
		public bool IsCommandEnabled(PrintingSystemCommand command) {
			return CommandSet.IsCommandEnabled(command);
		}
		public void ExecCommand(PrintingSystemCommand command) {
			ExecCommand(command, new object[] { });
		}
		public bool CanExecCommand(PrintingSystemCommand command) {
			return (PrintingSystemExtender != null && PrintingSystemExtender.CanExecCommand(command, this)) ? true :
				commandHandler.CanHandleCommand(command, this);
		}
		public void ExecCommand(PrintingSystemCommand command, object[] args) {
			if(PrintingSystemExtender != null)
				PrintingSystemExtender.ExecCommand(command, args, this);
		}
		internal void ExecCommandCore(PrintingSystemCommand command, object[] args) {
			CommandExecuteEventArgs e = new CommandExecuteEventArgs(command, args);
			OnCommandExecute(e);
			if(!e.Handled) {
				bool handled = false;
				commandHandler.HandleCommand(command, args, this, ref handled);
			}
		}
		protected override void BindPrintingSystem() {
			base.BindPrintingSystem();
			Document.ReplaceService<IWaitIndicator>(new WaitFormService(ViewControl, LookAndFeel));
			ICancellationService serv = Document.GetService<ICancellationService>();
			if(serv != null)
				serv.StateChanged += OnCancellationChanged;
		}
		void OnCancellationChanged(object sender, EventArgs e) {
			if(((ICancellationService)sender).IsCancellationRequested())
				HideWaitIndicator();
			else if(((ICancellationService)sender).CanBeCanceled() && DocumentIsEmpty)
				ShowWaitIndicator();
			UpdateCommands();
		}
		protected override void UnbindPrintingSystem() {
			base.UnbindPrintingSystem();
			if(psExtender != null)
				psExtender.RemoveViewer(this);
			ICancellationService serv = Document.GetService<ICancellationService>();
			if(serv != null)
				serv.StateChanged -= OnCancellationChanged;
		}
		protected override void DrawPage(IPage page, Graphics graph, PointF position) {
			base.DrawPage(page, graph, position);
			if(selectionService != null)
				selectionService.OnDrawPage((Page)page, graph, position);
			DrawMarkedBricks((Page)page, graph, position);
		}
		void DrawMarkedBricks(Page page, Graphics gr, PointF position) {
			if(page.PageSize.IsEmpty)
				return;
			List<Brick> markedBricks = new List<Brick>(PrintingSystem.GetMarkedBricks(page));
			if(markedBricks.Count == 0)
				return;
			Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
				if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
					((CompositeBrick)item).ForceBrickMap();
					PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
					return new MappedIndexedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = gr.ClipBounds, ViewOrigin = viewOrigin};
				}
				return new IndexedEnumerator(item.InnerBrickList);
			};
			new BrickNavigator(page, method) { BrickPosition = position }.IterateBricks((brick, brickRect, brickClipRect) => {
				if(markedBricks.Remove(brick)) {
					if(!RaisePaintMarkedBricksArea(gr, brickRect))
						DrawMarkedBrick(gr, brickRect);
				}
				return markedBricks.Count == 0;
			});
		}
		void DrawMarkedBrick(Graphics gr, RectangleF rect) {
			float inflateValue = PSUnitConverter.PixelToDoc(3);
			rect = RectangleF.Inflate(rect, inflateValue, inflateValue);
			gr.DrawRectangle(MarkPen, Rectangle.Round(rect));
		}
		protected override void UpdateCommands() {
			UpdateCommands(commandSet => {
				if(PrintingSystem != null) {
					commandSet.UpdateCommands(SelectedPageIndex, Zoom, fMinZoom, fMaxZoom, parametersController.HasParameters, PrintingSystem.Document);
					UpdateStopPageBuildingCommand(PrintingSystem.Document);
				}
				commandSet.UpdateOpenAndClosePreviewCommands(PrintingSystem != null && !Document.IsCreating);
			});
		}
		internal void UpdateCommands(Action<CommandSet> callback) {
			if(PrintingSystem != null) {
				CommandSet.CommandVisibilityChanged -= new EventHandler(OnCommandVisibilityChanged);
				callback(CommandSet);
				if(CommandSet.Dirty) {
					CommandSet.Dirty = false;
					OnCommandChanged(EventArgs.Empty);
				}
				this.CommandSet.CommandVisibilityChanged += new EventHandler(OnCommandVisibilityChanged);
			} else
				callback(CommandSet);
		}
		void UpdateStopPageBuildingCommand(Document document) {
			ReflectorBarItem reflectorBarItem = this.ProgressReflector as ReflectorBarItem;
			ICancellationService serv = document.PrintingSystem.GetService<ICancellationService>();
			bool value = document.IsCreating && reflectorBarItem != null && reflectorBarItem.Visible;
			CommandSet.UpdateStopPageBuildingCommand(value || serv.CanBeCanceled(), document.PrintingSystem);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(CommandSet != null) {
					CommandSet.Clear();
					CommandSet.CommandVisibilityChanged -= new EventHandler(OnCommandVisibilityChanged);
				}
				if(commandHandler != null) {
					commandHandler.Dispose();
					commandHandler = null;
				}
				if(brickHandler != null) {
					brickHandler.Dispose();
					brickHandler = null;
				}
				if(parametersController != null) {
					parametersController.Dispose();
					parametersController = null;
				}
				if(navigationController != null) {
					navigationController.Dispose();
					navigationController = null;
				}
				if(thumbnailsController != null) {
					thumbnailsController.Dispose();
					thumbnailsController = null;
				}
				if(controller != null) {
					controller.Dispose();
					controller = null;
				}
				if(dockManager != null) {
					dockManager.Controller = null;
					dockManager = null;
				}
				if(imageCollection != null) {
					imageCollection.Dispose();
					imageCollection = null;
				}
				if(documentManager != null) {
					documentManager.BarAndDockingController = null;
					documentManager = null;
				}
				if(markPen != null) {
					markPen.Dispose();
					markPen = null;
				}
				if(selectionMessageHandler != null) {
					selectionMessageHandler.Dispose();
					selectionMessageHandler = null;
				}
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			if(disposeRunning || DesignMode || dockManager != null)
				return;
			InitializeDockManager();
		}
		internal void HidePanels() {
			if(DockManager != null)
				panelHelper.HidePanels(DockManager.Panels);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible)
				panelHelper.RestorePanelsVisibility();
		}
		protected override void SetLookAndFeel() {
			base.SetLookAndFeel();
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected override bool TryGetCursor(IPage page, ref Cursor value) {
			if(base.TryGetCursor(page, ref value))
				return true;
			BrickEventArgs args = CreateBrickEventArgs(null);
			if(args.Brick != null && args.Brick.Url.Length > 0) {
				value = Cursors.Hand;
				return true;
			}
			return false;
		}
		protected override void OnCommandVisibilityChanged(object sender, EventArgs e) {
			base.OnCommandVisibilityChanged(sender, e);
			selectionMessageHandler.CanHandle = PrintingSystem.GetCommandVisibility(PrintingSystemCommand.Copy) != CommandVisibility.None;
		}
		internal BrickEventArgs CreateBrickEventArgs(EventArgs e) {
			return CreateBrickEventArgs(e, MousePosition);
		}
		BrickEventArgs CreateBrickEventArgs(EventArgs e, Point screenPoint) {
			Page page = null;
			RectangleF brickBounds = RectangleF.Empty;
			Brick brick = FindBrickBy(screenPoint, ref page, ref brickBounds);
			if(page != null && brick != null) {
				brickBounds.Offset(ViewManager.GetPageRect(page).Location);
				PointF pt = ViewControl.PointToClient(screenPoint);
				pt = PSUnitConverter.PixelToDoc(pt, Zoom, ViewManager.ScrollPos);
				float x = pt.X - brickBounds.X;
				float y = pt.Y - brickBounds.Y;
				return new BrickEventArgs(e, brick, page, BrickBoundsToScreen(brickBounds), x, y);
			}
			return new BrickEventArgs(e, null, page, Rectangle.Empty, 0.0f, 0.0f);
		}
		public Brick FindBrickBy(Point screenPoint, ref Page page, ref RectangleF brickBounds) {
			page = (Page)ViewManager.FindPage(screenPoint);
			if(page == null)
				return null;
			RectangleF pageRect = ViewManager.GetPageRect(page);
			if(pageRect.IsEmpty)
				return null;
			PointF pos = PSUnitConverter.PixelToDoc(ViewControl.PointToClient(screenPoint), Zoom, ViewManager.ScrollPos);
			pos.X -= pageRect.X;
			pos.Y -= pageRect.Y;
			Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
				if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
					((CompositeBrick)item).ForceBrickMap();
					PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
					return new ReversedMappedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = new RectangleF(pos, new SizeF(1, 1)), ViewOrigin = viewOrigin };
				}
				return new ReversedEnumerator(item.InnerBrickList);
			};
			Tuple<Brick, RectangleF> result = new BrickNavigator(page, method) { BrickPosition = PointF.Empty }.FindBrick(pos);
			if(result != null) {
				brickBounds = result.Item2;
				return result.Item1;
			}
			return null;
		}
		public Rectangle GetBrickScreenBounds(Brick brick, Page page) {
			RectangleF bounds = page.GetBrickBounds(brick);
			if(bounds.IsEmpty)
				return Rectangle.Empty;
			bounds.Offset(ViewManager.GetPageRect(page).Location);
			return BrickBoundsToScreen(bounds);
		}
		Rectangle BrickBoundsToScreen(RectangleF bounds) {
			RectangleF value = PSUnitConverter.DocToPixel(bounds, Zoom, ViewManager.ScrollPos);
			return ViewControl.RectangleToScreen(Rectangle.Round(value));
		}
		public Brick FindBrick(Point screenPoint) {
			Page page = null;
			RectangleF brickBounds = RectangleF.Empty;
			return FindBrickBy(screenPoint, ref page, ref brickBounds);
		}
	}
	internal class FindPanel : PanelControl {
		SelectionService selectionService;
		IPrintControl printControl;
		FindControl findControl;
		FindControl FindControl {
			get {
				if(findControl == null) {
					findControl = new FindControl() { Dock = DockStyle.Fill, RaiseRequestClose = RaiseRequestClose, SelectionService = selectionService, PrintingSystem = () => printControl.PrintingSystem };
					findControl.LookAndFeel.ParentLookAndFeel = LookAndFeel;
					Controls.Add(findControl);
				}
				return findControl;
			}
		}
		public FindPanel(IPrintControl printControl, SelectionService selectionService) {
			if(selectionService == null)
				throw new ArgumentNullException();
			this.selectionService = selectionService;
			this.printControl = printControl;
		}
		public event EventHandler RequestClose;
		void RaiseRequestClose() {
			if(RequestClose != null)
				RequestClose(this, EventArgs.Empty);
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				DrawBackground(cache);
			}
		}
		protected virtual void DrawBackground(GraphicsCache cache) {
			DevExpress.Skins.SkinElementInfo elementInfo = new DevExpress.Skins.SkinElementInfo(GetBackgroundElement(), ClientRectangle);
			ObjectPainter.DrawObject(cache, DevExpress.Skins.SkinElementPainter.Default, elementInfo);
		}
		protected internal DevExpress.Skins.SkinElement GetBackgroundElement() {
			if(SkinProvider == null) return null;
			return GetBackgroundElementCore();
		}
		protected virtual DevExpress.Skins.SkinElement GetBackgroundElementCore() {
			return PdfViewerSkins.GetSkin(SkinProvider)[PdfViewerSkins.SearchPanel];
		}
		protected ISkinProvider SkinProvider {
			get {
				if(this.LookAndFeel == null) return null;
				return this.LookAndFeel;
			}
		}
		public void FocusFindControl() {
			FindControl.FocusTextEditor();
			FindControl.SetTextSelection();
		}
		public void ShowAnimation() {
			const int borderWidth = 1;
			Rectangle setRect = Bounds;
			int findControlHeight = FindControl.GetValidHeight();
			setRect.Height = findControlHeight + Padding.Top + Padding.Bottom + Margin.Top + Margin.Bottom + borderWidth;
			XtraAnimator.Current.AddBoundsAnimation(
				new CallbackSupportAnimation(Parent, item => true),
				new CallbackObjectWithBounds(() => Bounds, rect => Bounds = rect),
				this, true, Bounds, setRect, 5);
			Enabled = true;
			FindControl.Reset();
			FindControl.FocusTextEditor();
			FindControl.SetTextSelection();
		}
		public void CloseAnimation() {
			Rectangle setRect = Bounds;
			setRect.Height = 0;
			XtraAnimator.Current.AddBoundsAnimation(
				new CallbackSupportAnimation(Parent, item => true),
				new CallbackObjectWithBounds(() => Bounds, rect => Bounds = rect),
				this, true, Bounds, setRect, 5);
			Enabled = false;
			FindControl.Reset();
		}
	}
}
