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
using System.Security.Permissions;
using System.Windows.Forms;
using DevExpress.Office;
using DevExpress.Office.Services.Implementation;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.Office.PInvoke;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet {
	[
	System.Runtime.InteropServices.ComVisible(false),
	ToolboxBitmap(typeof(SpreadsheetControl), DevExpress.Utils.ControlConstants.BitmapPath + "SpreadsheetControl.bmp"),
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabSpreadsheet),
	Designer("DevExpress.XtraSpreadsheet.Design.XtraSpreadsheetDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign),
	Docking(DockingBehavior.Ask),
	Description("A control to create, load, modify, print, save and convert spreadsheet documents in different formats."),
]
	public partial class SpreadsheetControl : Control, IToolTipControlClient {
		#region Fields
		bool isDisposing;
		bool isDisposed;
		bool useGdiPlus;
		IDXMenuManager menuManager;
		readonly LeakSafeEventRouter leakSafeEventRouter;
		ToolTipController toolTipController;
		SpreadsheetClipboardWithDelayedRendering clipboardWithDelayedRendering;
		#endregion
		static SpreadsheetControl() {
		}
		public static void About() {
		}
		public SpreadsheetControl()
			: this(false) {
		}
		internal SpreadsheetControl(bool useGdiPlus) {
			this.mouseHelper = new MouseWheelScrollHelper(this);
			this.useGdiPlus = useGdiPlus;
			this.leakSafeEventRouter = new LeakSafeEventRouter(this);
			this.AllowDrop = true;
			this.innerControl = CreateInnerControl();
			this.gestureHelper = new GestureHelper(this);
			clipboardWithDelayedRendering = null;
			BeginInitialize();
			innerControl.SetInitialDocumentModelLayoutUnit(DefaultLayoutUnit);
			EndInitialize();
			RegisterToolTipClient();
		}
		#region Properties
		internal bool InnerIsDisposed { get { return isDisposed; } }
		internal bool InnerIsDisposing { get { return isDisposing; } }
		protected override Size DefaultSize { get { return new Size(400, 200); } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Font Font { get { return base.Font; } set { base.Font = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override RightToLeft RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		#region UseGdiPlus
		[DefaultValue(false)]
		internal bool UseGdiPlus {
			get { return useGdiPlus; }
			set {
				if (useGdiPlus == value)
					return;
				useGdiPlus = value;
				OnUseGdiPlusChanged();
			}
		}
		bool ISpreadsheetControl.UseGdiPlus { get { return this.UseGdiPlus; } }
		bool IInnerSpreadsheetControlOwner.IsEnabled { get { return Enabled; } }
		#endregion
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlMenuManager"),
#endif
			DefaultValue(null)]
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlOptions"),
#endif
 NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetControlOptions Options { get { return InnerControl != null ? (SpreadsheetControlOptions)InnerControl.Options : null; } }
		internal ToolTipController ActualToolTipController { get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; } }
		#region ToolTipController
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlToolTipController"),
#endif
 Category(CategoryName.Appearance), DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if (value == ToolTipController.DefaultController)
					value = null;
				if (ToolTipController == value)
					return;
				UnsubscribeToolTipControllerEvents(ActualToolTipController);
				UnregisterToolTipClientControl(ActualToolTipController);
				this.toolTipController = value;
				RegisterToolTipClientControl(ActualToolTipController);
				SubscribeToolTipControllerEvents(ActualToolTipController);
			}
		}
		#endregion
		#region AllowDrop
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlAllowDrop"),
#endif
 NotifyParentProperty(true), DefaultValue(true)]
		public override bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		#endregion
		protected internal DevExpress.XtraSpreadsheet.UI.SpreadsheetBarController BarController { get; set; }
		#endregion
		#region IInnerDocumentServerOwner properties
		#region Unit
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlUnit"),
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
		#region ReadOnly
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlReadOnly"),
#endif
		DefaultValue(false)]
		public bool ReadOnly {
			get { return InnerControl != null ? InnerControl.IsReadOnly : true; }
			set {
				if (InnerControl != null)
					InnerControl.IsReadOnly = value;
			}
		}
		#endregion
		#region Modified
		[Browsable(false), DefaultValue(false)]
		public bool Modified {
			get { return InnerControl != null ? InnerControl.Modified : false; }
			set { if (InnerControl != null) InnerControl.Modified = value; }
		}
		#endregion
		#region LayoutUnit
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlLayoutUnit")]
#endif
		public DocumentLayoutUnit LayoutUnit {
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
		#endregion
		#region Selection
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.Spreadsheet.Range SelectedCell {
			get { return InnerControl != null ? InnerControl.SelectedApiCell : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiCell = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.Spreadsheet.Range Selection {
			get { return InnerControl != null ? InnerControl.SelectedApiRange : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiRange = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.Spreadsheet.Shape SelectedShape {
			get { return InnerControl != null ? InnerControl.SelectedApiShape : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiShape = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DevExpress.Spreadsheet.Picture SelectedPicture {
			get { return InnerControl != null ? InnerControl.SelectedApiPicture : null; }
			set {
				if (InnerControl != null)
					InnerControl.SelectedApiPicture = value;
			}
		}
		#endregion
		#region Clipboard
		[Browsable(false)]
		public IClipboardManager Clipboard { get { return InnerControl != null ? InnerControl.Clipboard : null; } }
		#endregion
		#endregion
		public Rectangle GetCellBounds(int rowIndex, int columnIndex) {
			if (InnerControl == null)
				return Rectangle.Empty;
			return InnerControl.GetCellBounds(columnIndex, rowIndex);
		}
		bool ISpreadsheetControl.CaptureMouse() {
			this.Capture = true;
			return this.Capture;
		}
		bool ISpreadsheetControl.ReleaseMouse() {
			this.Capture = false;
			return !this.Capture;
		}
		protected internal virtual void EndInitialize() {
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			SubscribeLookAndFeelEvents();
			this.painter = CreatePainter();
			EndInitializeCommon();
		}
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			this.isDisposing = true;
			try {
				if (disposing)
					DisposeCore();
			}
			finally {
				base.Dispose(disposing);
				this.isDisposed = true;
				this.isDisposing = false;
			}
		}
		#endregion
		protected internal virtual void DisposeCore() {
			lock (this) {
				if (!IsDisposed)
					RaiseBeforeDispose();
				if (GetService<IFormulaBarControl>() != null)
					RemoveService(typeof(IFormulaBarControl));
				if (GetService<INameBoxControl>() != null)
					RemoveService(typeof(INameBoxControl));
				DisposeTabSelector();
				DisposeCommon();
				if (printer != null) {
					printer.Dispose();
					printer = null;
				}
				DisposeScrollbars();
				DisposeToolTipService();
				DisposeLookAndFeel();
				DisposeBackgroundPainter();
				DisposeViewPainter();
				if (painter != null) {
					painter.Dispose();
					painter = null;
				}
				this.menuManager = null;
			}
		}
		void DisposeToolTipService() {
			if (toolTipController != null) {
				ToolTipController = null;
				UnsubscribeToolTipControllerEvents(ToolTipController.DefaultController);
				UnregisterToolTipClientControl(ToolTipController.DefaultController);
			}
		}
		protected internal virtual void AddServicesPlatformSpecific() {
			AddService(typeof(ISpreadsheetCommandFactoryService), CreateSpreadsheetCommandFactoryService());
			AddService(typeof(IToolTipService), new ToolTipService(this));
			AddService(typeof(IChartControllerFactoryService), new ChartControllerFactoryService());
			AddService(typeof(IChartImageService), new ChartImageService());
			AddService(typeof(IMessageBoxService), new MessageBoxService(this, LookAndFeel));
		}
		protected internal void SubscribeInnerEventsPlatformSpecific() {
			this.EnabledChanged += OnEnabledChanged;
		}
		protected internal void UnsubscribeInnerEventsPlatformSpecific() {
			this.EnabledChanged -= OnEnabledChanged;
		}
		void OnEnabledChanged(object sender, EventArgs e) {
			OnEnabledChanged();
		}
		#region IToolTipControlClient Members
		bool IToolTipControlClient.ShowToolTips { get { return InnerControl.MouseHandler.State.CanShowToolTip; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			IToolTipService service = GetService<IToolTipService>();
			return (service != null) ? service.CalculateToolTipInfo(point) : null;
		}
		#endregion
		protected internal virtual void RegisterToolTipClientControl(ToolTipController controller) {
			controller.AddClientControl(this);
		}
		protected internal virtual void UnregisterToolTipClientControl(ToolTipController controller) {
			controller.RemoveClientControl(this);
		}
		protected internal virtual void SubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed += new EventHandler(OnToolTipControllerDisposed);
		}
		protected internal virtual void UnsubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed -= new EventHandler(OnToolTipControllerDisposed);
		}
		protected internal virtual void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		protected virtual void RegisterToolTipClient() {
			RegisterToolTipClientControl(ToolTipController.DefaultController);
		}
		protected virtual ISpreadsheetCommandFactoryService CreateSpreadsheetCommandFactoryService() {
			return new WinFormsSpreadsheetCommandFactoryService(this);
		}
		SpreadsheetMouseHandlerStrategyFactory ISpreadsheetControl.CreateMouseHandlerStrategyFactory() {
			return new WinFormsSpreadsheetMouseHandlerStrategyFactory();
		}
		protected internal virtual void OnUseGdiPlusChanged() {
			if (InnerControl != null)
				InnerControl.RecreateMeasurementAndDrawingStrategy();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			ICopiedRangeDataForClipboard dataProvider = this.DocumentModel.GetService<ICopiedRangeDataForClipboard>();
			clipboardWithDelayedRendering = new SpreadsheetClipboardWithDelayedRendering(Handle, dataProvider);
			this.DocumentModel.Clipboard = clipboardWithDelayedRendering;
			ForceHandleLookAndFeelChanged();
			HorizontalSplitContainer.SplitterPosition = (int)(HorizontalSplitContainer.Width * 0.6f);
			DeferredBackgroundThreadUIUpdater deferredUpdater = (DeferredBackgroundThreadUIUpdater)BackgroundThreadUIUpdater;
			InnerControl.BackgroundThreadUIUpdater = new BeginInvokeBackgroundThreadUIUpdater(GetService<IThreadSyncService>());
			PerformDeferredUIUpdates(deferredUpdater);
			Application.Idle += leakSafeEventRouter.OnApplicationIdle;
			if (InnerControl != null)
				InnerControl.UpdateUIOnIdle = true;
			OnUpdateUI();
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			Application.Idle -= leakSafeEventRouter.OnApplicationIdle;
			if (InnerControl != null)
				InnerControl.UpdateUIOnIdle = false;
			OnUpdateUI();
			if (InnerControl != null)
				InnerControl.BackgroundThreadUIUpdater = new DeferredBackgroundThreadUIUpdater();
			base.OnHandleDestroyed(e);
		}
		protected internal virtual void OnApplicationIdle(object sender, EventArgs e) {
			if (InnerControl != null)
				InnerControl.OnApplicationIdle();
		}
		protected internal virtual void OnUpdateUI() {
			if (InnerControl != null)
				InnerControl.OnUpdateUI();
		}
		bool ISpreadsheetControl.IsHyperlinkActive() {
			return this.IsHyperlinkActive();
		}
		protected internal virtual bool IsHyperlinkActive() {
			return false;
		}
		DocumentOptions IInnerSpreadsheetDocumentServerOwner.CreateOptions(InnerSpreadsheetDocumentServer documentServer) {
			return new SpreadsheetControlOptions(documentServer);
		}
		protected internal virtual void OnReadOnlyChangedPlatformSpecific() {
		}
		void OnEmptyDocumentCreatedPlatformSpecific() {
		}
		void OnDocumentLoadedPlatformSpecific() {
		}
		protected internal virtual void OnInnerControlContentChangedPlatformSpecific(bool suppressBindingNotifications) {
		}
		void IInnerSpreadsheetControlOwner.Redraw() {
			Redraw();
		}
		void IInnerSpreadsheetControlOwner.Redraw(RefreshAction action) {
			Redraw();
		}
		MeasurementAndDrawingStrategy IInnerSpreadsheetDocumentServerOwner.CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			return this.CreateMeasurementAndDrawingStrategy(documentModel);
		}
		protected internal virtual MeasurementAndDrawingStrategy CreateMeasurementAndDrawingStrategy(DocumentModel documentModel) {
			if (useGdiPlus)
				return new GdiPlusMeasurementAndDrawingStrategy(documentModel);
			else
				return new GdiMeasurementAndDrawingStrategy(documentModel);
		}
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override void WndProc(ref Message m) {
			try {
				if (gestureHelper.WndProc(ref m))
					return;
				const int WM_CONTEXTMENU = 0x7B;
				const int WM_GETDLGCODE = 0x87;
				const int DLGC_WANTTAB = 0x02;
				const int DLGC_WANTALLKEYS = 0x04;
				switch (m.Msg) {
					case WM_CONTEXTMENU:
						if (OnWmContextMenu(ref m))
							return;
						else
							break;
					case WM_GETDLGCODE:
						if (AcceptsTab)
							m.Result = (IntPtr)(DLGC_WANTTAB | DLGC_WANTALLKEYS);
						else
							m.Result = (IntPtr)(DLGC_WANTALLKEYS);
						return;
					default:
						break;
				}
				if (ImeHelper.IsImeMessage(ref m)) {
					if (!InnerControl.IsInplaceEditorActive)
						return;
					IntPtr handle = Win32.GetFocus();
					if (handle != IntPtr.Zero) {
						Control control = FromChildHandle(handle);
						if (control != null && control != this)
							Win32.SendMessage(handle, m.Msg, (int)m.WParam, m.LParam);
					}
					return;
				}
				if (clipboardWithDelayedRendering != null && clipboardWithDelayedRendering.IsClipboardMessange(ref m)) {
					clipboardWithDelayedRendering.HandleWndProc(m.Msg, (int)m.WParam);
					return;
				}
				base.WndProc(ref m);
			}
			catch (Exception e) {
				if (!HandleException(e))
					throw;
			}
		}
		protected internal virtual Point GetPhysicalPoint(Point point) {
			int x = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.X, DpiX) - ViewBounds.Left;
			int y = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(point.Y, DpiY) - ViewBounds.Top;
			return new Point(x, y);
		}
		void IInnerSpreadsheetDocumentServerOwner.RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			this.RaiseDeferredEvents(changeActions);
		}
		protected internal virtual void RaiseDeferredEvents(DocumentModelChangeActions changeActions) {
			InnerSpreadsheetControl innerControl = InnerControl;
			IThreadSyncService service = GetService<IThreadSyncService>();
			if (innerControl != null)
				service.EnqueueInvokeInUIThread(new Action(delegate() { innerControl.RaiseDeferredEventsCore(changeActions); }));
		}
		void ApplyChangesCorePlatformSpecific(DocumentModelChangeActions changeActions) {
			if ((changeActions & DocumentModelChangeActions.ResetHeaderLayout) != 0 || (changeActions & DocumentModelChangeActions.ResetControlsLayout) != 0)
				OnDeferredResizeCore();
			if ((changeActions & DocumentModelChangeActions.Redraw) != 0)
				Redraw();
		}
		void IInnerSpreadsheetControlOwner.OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			this.OnOptionsChangedPlatformSpecific(e);
		}
		protected internal virtual void OnOptionsChangedPlatformSpecific(BaseOptionChangedEventArgs e) {
			string propertyName = e.Name;
			if (propertyName == SpreadsheetTabSelectorOptions.PropertyName_Visibility || propertyName == SpreadsheetScrollbarOptions.PropertyName_Visibility)
				OnDeferredResizeCore();
			if (propertyName == "UseSkinColors")
				Redraw();
		}
		protected internal virtual void OnDeferredResizeCore() {
			if (IsUpdateLocked)
				ControlDeferredChanges.Resize = true;
			else
				OnResizeCore();
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			if (Options == null)
				return;
			if (Options.Behavior.Selection.ShowSelectionMode != ShowSelectionMode.Always)
				Refresh();
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			if (Options == null)
				return;
			if (Options.Behavior.Selection.ShowSelectionMode != ShowSelectionMode.Always)
				Refresh();
		}
		protected internal bool ShouldDrawSelection() {
			if (!Enabled)
				return false;
			if (Options.Behavior.Selection.ShowSelectionMode == ShowSelectionMode.Always)
				return true;
			if (Focused)
				return true;
			if (InnerControl.IsInplaceEditorActive)
				return true;
			RibbonControl focusedControl = ObtainFocusedControl() as RibbonControl;
			if (focusedControl != null) {
				if (Object.ReferenceEquals(focusedControl.Manager, ObtainActiveBarManager()))
					return true;
			}
			return false;
		}
		Control ObtainFocusedControl() {
			IntPtr handle = Win32.GetFocus();
			if (handle != IntPtr.Zero)
				return Control.FromHandle(handle);
			return null;
		}
		BarManager ObtainActiveBarManager() {
			if (BarController == null)
				return null;
			List<BarItem> barItems = BarController.BarItems;
			if (barItems != null && barItems.Count > 0) {
				if (barItems[0] != null)
					return barItems[0].Manager;
			}
			return null;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			BeginUpdate();
			try {
				InnerControl.ResetAndRedrawHeader();
				InnerControl.HideHoveredComment();
			}
			finally {
				EndUpdate();
			}
		}
	}
}
