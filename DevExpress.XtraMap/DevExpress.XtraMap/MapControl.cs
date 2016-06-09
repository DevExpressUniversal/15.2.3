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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Map;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Gesture;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using DevExpress.XtraMap.Printing;
using DevExpress.XtraPrinting;
using DevExpress.Map.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraMap {
	public enum RenderMode { Auto, GdiPlus, DirectX }
	public enum ElementSelectionMode { None, Single, Multiple, Extended }
	[
System.Runtime.InteropServices.ComVisible(true),
DXToolboxItem(true),
ToolboxTabName(AssemblyInfo.DXTabData),
ToolboxBitmap(typeof(MapControl), DevExpress.Utils.ControlConstants.BitmapPath + "MapControl.bmp"),
Designer("DevExpress.XtraMap.Design.MapControlDesigner," + AssemblyInfo.SRAssemblyMapDesign),
Docking(DockingBehavior.Ask)
]
	public class MapControl : Control, IMapControl, ISupportLookAndFeel, ISupportInitialize, IToolTipControlClient, IGestureClient,
		IMouseWheelSupport, IMouseWheelScrollClient, IMapAnimatableItem, IPrintable, IServiceContainer, IMapControlEventsListener {
		static MapControl() {
		}
		public static void About() {
		}
		int initializeCounter = 0;
		bool loadCompleted;
		bool isPainted;
		InnerMap map;
		UserLookAndFeel lookAndFeel;
		ToolTipController toolTipController;
		GestureHelper gestureHelper;
		Color backColor = InnerMap.DefaultBackColor;
		IntPtr mapHandle;
		MouseWheelScrollHelper mouseHelper;
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return lookAndFeel; } }
		protected MapPrinter Printer { get { return Map != null ? Map.Printer : null; } }
		protected bool IsPainted { get { return isPainted; } }
		protected virtual bool IsLoading { get { return this.initializeCounter != 0; } }
		protected virtual bool IsLoaded { get { return loadCompleted; } }
		protected virtual bool IsDesignMode { get { return DesignMode; } }
		protected virtual bool IsDesigntimeProcess { get { return DesignTimeTools.IsDesignMode; }}
		protected override Size DefaultSize { get { return InnerMap.DefaultMapControlSize; } }
		internal ToolTipController ActualToolTipController {
			get { return toolTipController != null ? toolTipController : ToolTipController.DefaultController; }
		}
		protected internal InnerMap Map { get { return map; } }
		protected MapUnitConverter UnitConverter { get { return Map.UnitConverter; } }
		protected internal IntPtr MapHandle { get { return mapHandle; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlBorderStyle"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(BorderStyles.Default)]
		public BorderStyles BorderStyle {
			get {
				return Map != null ? Map.BorderStyle : BorderStyles.Default;
			}
			set {
				if(Map != null) {
					Map.BorderStyle = value;
				}
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlBackColor"),
#endif
		Category(SRCategoryNames.Appearance)]
		public new Color BackColor {
			get { return backColor; }
			set {
				if(backColor == value)
					return;
				backColor = MapUtils.IsColorEmpty(value) ? InnerMap.DefaultBackColor : value;
				SetInnerBackColor(backColor);
			}
		}
		new void ResetBackColor() { BackColor = InnerMap.DefaultBackColor; }
		bool ShouldSerializeBackColor() { return BackColor != InnerMap.DefaultBackColor; }
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlToolTipController"),
#endif
		Category(SRCategoryNames.Appearance), DefaultValue(null)]
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(value == ToolTipController.DefaultController)
					value = null;
				if(ToolTipController == value)
					return;
				UnsubscribeToolTipControllerEvents(ActualToolTipController);
				UnregisterToolTipClientControl(ActualToolTipController);
				this.toolTipController = value;
				RegisterToolTipClientControl(ActualToolTipController);
				SubscribeToolTipControllerEvents(ActualToolTipController);
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlLayers"),
#endif
		Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.LayerCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))]
		public LayerCollection Layers { get { return Map != null ? Map.Layers : null; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlLegends"),
#endif
		Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.LegendCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))
		]
		public LegendCollection Legends { get { return Map != null ? Map.Legends : null; } }
		[Category(SRCategoryNames.Map), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraMap.Design.MapOverlayCollectionEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor))
		]
		public MapOverlayCollection Overlays { get { return Map != null ? Map.Overlays : null; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlCenterPoint"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category(SRCategoryNames.Map),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		RefreshProperties(RefreshProperties.All)]
		public CoordPoint CenterPoint { get { return Map != null ? Map.CenterPoint : null; } set { if(Map != null) Map.CenterPoint = value; } }
		void ResetCenterPoint() { CenterPoint = Map.CreateDefaultCenterPoint(); }
		bool ShouldSerializeCenterPoint() { return CenterPoint != Map.CreateDefaultCenterPoint(); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlZoomLevel"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultZoomLevel)]
		public double ZoomLevel { get { return Map != null ? Map.ZoomLevel : InnerMap.DefaultZoomLevel; } set { if(Map != null) Map.ZoomLevel = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlMinZoomLevel"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultMinZoomLevel)]
		public double MinZoomLevel { get { return Map != null ? Map.MinZoomLevel : InnerMap.DefaultMinZoomLevel; } set { if(Map != null) Map.MinZoomLevel = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlMaxZoomLevel"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultMaxZoomLevel)]
		public double MaxZoomLevel { get { return Map != null ? Map.MaxZoomLevel : InnerMap.DefaultMaxZoomLevel; } set { if(Map != null) Map.MaxZoomLevel = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlEnableAnimation"),
#endif
		Category(SRCategoryNames.Options), DefaultValue(InnerMap.DefaultEnableAnimation)]
		public bool EnableAnimation { get { return Map != null ? Map.EnableAnimation : InnerMap.DefaultEnableAnimation; } set { if(Map != null) Map.EnableAnimation = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlEnableZooming"),
#endif
		Category(SRCategoryNames.Options), DefaultValue(InnerMap.DefaultEnableZooming)]
		public bool EnableZooming { get { return Map != null ? Map.EnableZooming : InnerMap.DefaultEnableZooming; } set { if(Map != null) Map.EnableZooming = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlEnableScrolling"),
#endif
		Category(SRCategoryNames.Options), DefaultValue(InnerMap.DefaultEnableScrolling)]
		public bool EnableScrolling { get { return Map != null ? Map.EnableScrolling : InnerMap.DefaultEnableScrolling; } set { if(Map != null) Map.EnableScrolling = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlShowToolTips"),
#endif
		Category(SRCategoryNames.Options), DefaultValue(InnerMap.DefaultShowToolTips)]
		public bool ShowToolTips { get { return Map != null ? Map.ShowToolTips : InnerMap.DefaultShowToolTips; } set { if(Map != null) Map.ShowToolTips = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlRenderMode"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultRenderMode)]
		public RenderMode RenderMode { get { return Map != null ? Map.RenderMode : InnerMap.DefaultRenderMode; } set { if(Map != null) Map.RenderMode = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlSelectionMode"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(InnerMap.DefaultSelectionMode)]
		public ElementSelectionMode SelectionMode { get { return Map != null ? Map.SelectionMode : InnerMap.DefaultSelectionMode; } set { if(Map != null) Map.SelectionMode = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlImageList"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public object ImageList { get { return Map != null ? Map.ImageList : null; } set { if(Map != null) Map.ImageList = value; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlNavigationPanelOptions"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public NavigationPanelOptions NavigationPanelOptions { get { return Map != null ? Map.NavigationPanelOptions : null; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlPrintOptions"),
#endif
		Category(SRCategoryNames.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PrintOptions PrintOptions { get { return Map != null ? Map.PrintOptions : null; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlCoordinateSystem"),
#endif
		Category(SRCategoryNames.Behavior),
		Editor("DevExpress.XtraMap.Design.MapCoordinateSystemPickerEditor," + AssemblyInfo.SRAssemblyMapDesign, typeof(UITypeEditor)),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableObjectConverterShowsValueTypeNameInParentheses," + AssemblyInfo.SRAssemblyMapDesign),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public MapCoordinateSystem CoordinateSystem {
			get { return Map != null ? Map.CoordinateSystem : InnerMap.DefaultCoordinateSystem; }
			set { if(Map != null) Map.CoordinateSystem = value; }
		}
		void ResetCoordinateSystem() { Map.CoordinateSystem = CreateDefaultCoordinateSystem(); }
		bool ShouldSerializeCoordinateSystem() { return !(InnerMap.DefaultCoordinateSystem.IsEqual(Map.CoordinateSystem)); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlLookAndFeel"),
#endif
		Category(SRCategoryNames.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlInitialMapSize"),
#endif
		Category(SRCategoryNames.Behavior)]
		public Size InitialMapSize {
			get { return Map.InitialMapSize; }
			set { Map.InitialMapSize = value; }
		}
		void ResetInitialMapSize() { InitialMapSize = InnerMap.DefaultInitialSize; }
		bool ShouldSerializeInitialMapSize() { return InitialMapSize != InnerMap.DefaultInitialSize; }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapControlMiniMap"),
#endif
		Category(SRCategoryNames.Map), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraMap.Design.ExpandableNoneStringSupportedTypeConverter," + AssemblyInfo.SRAssemblyMapDesign),
		Editor("DevExpress.XtraMap.Design.MiniMapPickerEditor," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign, typeof(UITypeEditor))]
		public MiniMap MiniMap {
			get { return Map.MiniMap; }
			set { Map.MiniMap = value; }
		}
		#region Events
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlDrawMapItem")]
#endif
		public event DrawMapItemEventHandler DrawMapItem;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlLegendItemCreating")]
#endif
		public event LegendItemCreatingEventHandler LegendItemCreating;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlSelectionChanging")]
#endif
		public event MapSelectionChangingEventHandler SelectionChanging;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlSelectionChanged")]
#endif
		public event MapSelectionChangedEventHandler SelectionChanged;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlMapItemClick")]
#endif
		public event MapItemClickEventHandler MapItemClick;
		public event MapItemClickEventHandler MapItemDoubleClick;
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlHyperlinkClick")]
#endif
		public event HyperlinkClickEventHandler HyperlinkClick;
		public event OverlaysArrangedEventHandler OverlaysArranged;
		internal void RaiseSelectionChanged(List<object> selection) {
			MapSelectionChangedEventArgs args = new MapSelectionChangedEventArgs(selection);
			SelectionChanged(this, args);
		}
		internal void RaiseSelectionChanging(MapSelectionChangingEventArgs args) {
			SelectionChanging(this, args);
		}
		internal void RaiseMapItemClick(MapItemClickEventArgs args) {
			MapItemClick(this, args);
		}
		internal void RaiseMapItemDoubleClick(MapItemClickEventArgs args) {
			MapItemDoubleClick(this, args);
		}
		internal void RaiseHyperlinkClick(HyperlinkClickEventArgs args) {
			HyperlinkClick(this, args);
		}
#if !SL
	[DevExpressXtraMapLocalizedDescription("MapControlExportMapItem")]
#endif
		public event ExportMapItemEventHandler ExportMapItem;
		internal ExportMapItemEventArgs RaiseExportMapItem(MapItem item) {
			if(ExportMapItem == null)
				return null;
			ExportMapItemEventArgs args = new ExportMapItemEventArgs(item);
			ExportMapItem(this, args);
			return args;
		}
		#endregion
		public MapControl() {
			SetStyle(ControlConstants.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.UserMouse, true);
			mouseHelper = new MouseWheelScrollHelper(this);
			this.map = CreateInnerMap();
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			SubscribeLookAndFeelEvents();
			RegisterToolTipClientControl(ToolTipController.DefaultController);
		}
		#region ISupportInitialize implementation
		void ISupportInitialize.BeginInit() {
			BeginInit();
		}
		void ISupportInitialize.EndInit() {
			EndInit();
		}
		#endregion
		#region IMapControl members
		ISkinProvider IMapControl.SkinProvider { get { return LookAndFeel; } }
		bool IMapControl.IsSkinActive {
			get {
				if(LookAndFeel == null)
					return false;
				return LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin;
			}
		}
		ToolTipController IMapControl.ToolTipController { get { return ActualToolTipController; } }
		bool IMapControl.IsDesignMode { get { return IsDesignMode; } }
		bool IMapControl.IsDesigntimeProcess { get { return IsDesigntimeProcess; } }
		Graphics IMapControl.CreateGraphics() {
			return CreateGraphics();
		}
		void IMapControl.ShowToolTip(string s, Point point) {
			ActualToolTipController.ShowHint(s, PointToScreen(point));
		}
		void IMapControl.HideToolTip() {
			ActualToolTipController.HideHint();
		}
		void IMapControl.AddChildControl(Control child) {
			if(!Controls.Contains(child))
				Controls.Add(child);
		}
		void IMapControl.RemoveChildControl(Control child) {
			if(Controls.Contains(child))
				Controls.Remove(child);
		}
		#endregion
		#region IMapControlEventsListener members
		void IMapControlEventsListener.NotifySelectionChanged() {
			if(SelectionChanged != null) RaiseSelectionChanged(Map.CollectAllSelectedItems());
		}
		void IMapControlEventsListener.NotifySelectionChanging(MapSelectionChangingEventArgs args) {
			if(SelectionChanging != null) RaiseSelectionChanging(args);
		}
		IRenderItemStyle IMapControlEventsListener.NotifyDrawMapItem(MapItem item) {
			if(DrawMapItem == null)
				return null;
			DrawMapItemEventArgs args = item.CreateDrawEventArgs();
			args.HasUpdate = false;
			DrawMapItem(this, args);
			return args.HasUpdate ? args : null;
		}
		void IMapControlEventsListener.NotifyExportMapItem(ExportMapItemEventArgs args) {
			if(ExportMapItem != null) ExportMapItem(this, args);
		}
		void IMapControlEventsListener.NotifyMapItemClick(MapItemClickEventArgs args) {
			if(MapItemClick != null) MapItemClick(this, args);
		}
		void IMapControlEventsListener.NotifyMapItemDoubleClick(MapItemClickEventArgs args) {
			if(MapItemDoubleClick != null) MapItemDoubleClick(this, args);
		}
		void IMapControlEventsListener.NotifyHyperlinkClick(HyperlinkClickEventArgs args) {
			if(HyperlinkClick != null && args != null) HyperlinkClick(this, args);
		}
		void IMapControlEventsListener.NotifyLegendItemCreating(LegendItemCreatingEventArgs e) {
			if(LegendItemCreating != null) LegendItemCreating(this, e);
		}
		void IMapControlEventsListener.NotifyOverlaysArranged(OverlaysArrangedEventArgs e) {
			if(OverlaysArranged != null) OverlaysArranged(this, e);
		}
		#endregion
		#region IGestureClient Members
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(!Map.OperationHelper.CanUseGestures())
				return GestureAllowArgs.None;
			return CreateAllowedGestures();
		}
		GestureAllowArgs[] CreateAllowedGestures() {
			List<GestureAllowArgs> result = new List<GestureAllowArgs>();
			if(Map.OperationHelper.CanZoom())
				result.Add(GestureAllowArgs.Zoom);
			if(Map.OperationHelper.CanScroll()) {
				result.Add(GestureAllowArgs.PressAndTap);
				result.Add(GestureAllowArgs.Pan);
			}
			return result.ToArray();
		}
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			overPan = Point.Empty;
			if(info.IsBegin) {
				return;
			}
			Map.HandleGesturePan(delta, ref overPan);
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			Map.HandleGestureZoom(center, zoomDelta);
		}
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		#endregion
		#region IMapAnimatableItem Members
		bool IMapAnimatableItem.EnableAnimation { get { return ((IMapAnimatableItem)Map).EnableAnimation; } }
		void IMapAnimatableItem.FrameChanged(object sender, AnimationAction action, double progress) {
			((IMapAnimatableItem)Map).FrameChanged(sender, action, progress);
		}
		#endregion
		#region IToolTipControlClient implementation
		bool IToolTipControlClient.ShowToolTips { get { return !DesignMode && Map.OperationHelper.CanShowToolTips(); } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			MapToolTipInfo info = Map.CalculateToolTipInfo(point);
			return (info != null && !string.IsNullOrEmpty(info.Text)) ? new ToolTipControlInfo(info.HitItem, info.Text) : null;
		}
		#endregion
		#region IServiceContainer Members
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			((IServiceContainer)Map).AddService(serviceType, callback, promote);
		}
		void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			((IServiceContainer)Map).AddService(serviceType, callback);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			((IServiceContainer)Map).AddService(serviceType, serviceInstance, promote);
		}
		void IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			((IServiceContainer)Map).AddService(serviceType, serviceInstance);
		}
		void IServiceContainer.RemoveService(Type serviceType, bool promote) {
			((IServiceContainer)Map).RemoveService(serviceType, promote);
		}
		void IServiceContainer.RemoveService(Type serviceType) {
			((IServiceContainer)Map).RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return ((IServiceContainer)Map).GetService(serviceType);
		}
		#endregion
		#region IPrintable implementation
		void IBasePrintable.Initialize(IPrintingSystem ps, ILink link) {
			Printer.Initialize(ps, link);
		}
		void IBasePrintable.Finalize(IPrintingSystem ps, ILink link) {
			Printer.Finalize(ps, link);
		}
		void IBasePrintable.CreateArea(string areaName, IBrickGraphics graph) {
			Printer.CreateArea(areaName, graph);
		}
		bool IPrintable.CreatesIntersectedBricks {
			get { return Printer.CreatesIntersectedBricks; }
		}
		void IPrintable.AcceptChanges() {
			Printer.AcceptChanges();
		}
		void IPrintable.RejectChanges() {
			Printer.RejectChanges();
		}
		void IPrintable.ShowHelp() {
			Printer.ShowHelp();
		}
		bool IPrintable.SupportsHelp() {
			return Printer.SupportsHelp();
		}
		bool IPrintable.HasPropertyEditor() {
			return Printer.HasPropertyEditor();
		}
		UserControl IPrintable.PropertyEditorControl { get { return Printer.PropertyEditorControl; } }
		#endregion
		#region IMouseWheelScrollClient Members
		bool IMouseWheelScrollClient.PixelModeHorz { get { return false; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return false; } }
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			if(e.Horizontal)
				return;
			Point pt = e.Location;
			int delta = SystemInformation.MouseWheelScrollDelta;
			MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Distance * (-delta));
			Map.OnMouseWheel(args);
		}
		#endregion
		#region IMouseWheelSupport
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			Point pt = PointToClient(Control.MousePosition);
			MouseEventArgs args = new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
			mouseHelper.OnMouseWheel(args);
		}
		#endregion
		void UpdateLookAndFeel() {
			Color color = CanApplySkinBackColor() ? InnerMap.DefaultBackColor : BackColor;
			SetInnerBackColor(color);
			Map.HandleLookAndFeelChanged(LookAndFeel.ActiveSkinName);
		}
		bool CanApplySkinBackColor() {
			return BackColor == InnerMap.DefaultBackColor && ((IMapControl)this).IsSkinActive;
		}
		void OnLookAndFeelChanged(object sender, EventArgs e) {
			UpdateLookAndFeel();
		}
		void SetInnerBackColor(Color color) {
			if(Map == null)
				return;
			Color defaultColor = InnerMap.DefaultBackColor;
			Map.BackColor = color != defaultColor ? color : ObtainSkinMapBackColor(defaultColor);
		}
		Color ObtainSkinMapBackColor(Color defaultColor) {
			return ((IMapControl)this).IsSkinActive ? SkinPainterHelper.GetSkinColorProperty(LookAndFeel, MapSkins.PropMapBackColor, defaultColor) : defaultColor;
		}
		void RegisterToolTipClientControl(ToolTipController controller) {
			controller.AddClientControl(this);
		}
		void UnregisterToolTipClientControl(ToolTipController controller) {
			controller.RemoveClientControl(this);
		}
		void SubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed += new EventHandler(OnToolTipControllerDisposed);
		}
		void UnsubscribeToolTipControllerEvents(ToolTipController controller) {
			controller.Disposed -= new EventHandler(OnToolTipControllerDisposed);
		}
		void UpdateIsLoaded() {
			if(IsLoading || IsLoaded) return;
			this.loadCompleted = true;
		}
		[SuppressMessage("Microsoft.Security", "CA2123:Override link demands should be identical to base"),
		SuppressMessage("Microsoft.Security", "CA2141:Transparent methods must not satisfy LinkDemands")]
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			if(GestureHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ToolTipController = null;
				UnsubscribeToolTipControllerEvents(ToolTipController.DefaultController);
				UnregisterToolTipClientControl(ToolTipController.DefaultController);
				if(Map != null) {
					Map.Dispose();
					map = null;
				}
				if(this.lookAndFeel != null) {
					UnsubscribeLookAndFeelEvents();
					this.lookAndFeel.Dispose();
					this.lookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override bool IsInputKey(Keys keyData) {
			return Map.IsInputKey();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			Map.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			Map.OnKeyUp(e);
			base.OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			Map.OnKeyPress(e);
			base.OnKeyPress(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			if(XtraForm.ProcessSmartMouseWheel(this, e))
				return;
			mouseHelper.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Map.OnMouseDown(e);
			base.OnMouseDown(e);
			Focus();
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Map.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnDoubleClick(EventArgs e) {
			Map.OnMouseUp(e as MouseEventArgs);
			base.OnDoubleClick(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Map.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			base.OnHandleDestroyed(e);
			if(Map != null) Map.OnHandleDestroyed();
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(!IsLoading && !DesignMode) {
				if(!IsPainted) return;
				OnLoadComplete();
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(!IsPainted) {
				isPainted = true;
				OnLoadComplete();
			}
			if(IsLoaded)
				Map.OnPaint(e);
		}
		protected override void OnSizeChanged(EventArgs e) {
			Map.SetClientRectangle(ClientRectangle);
			base.OnSizeChanged(e);
		}
		protected virtual void OnEndInit() {
			if(!DesignMode) {
				OnLoadComplete();
			}
		}
		protected virtual void OnLoadComplete() {
			UpdateIsLoaded();
			if(IsLoaded && Map != null)
				InitializeRendering();
		}
		protected internal virtual InnerMap CreateInnerMap() {
			this.mapHandle = Handle;
			return new InnerMap(this);
		}
		protected MapCoordinateSystem CreateDefaultCoordinateSystem() {
			return new GeoMapCoordinateSystem();
		}
		protected internal virtual void SubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged += OnLookAndFeelChanged;
		}
		protected internal virtual void UnsubscribeLookAndFeelEvents() {
			lookAndFeel.StyleChanged -= OnLookAndFeelChanged;
		}
		protected internal virtual void OnToolTipControllerDisposed(object sender, EventArgs e) {
			ToolTipController = null;
		}
		internal GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		internal void InitializeRendering() {
			Map.PrepareRenderController();
			UpdateLookAndFeel();
			Map.InitializeRendering();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds) {
			base.DrawToBitmap(bitmap, targetBounds);
		}
		public void SetMapItemFactory(IMapItemFactory factory) {
			Map.SetMapItemFactory(factory);
		}
		public virtual void BeginInit() {
			this.loadCompleted = false;
			if (IsDesignMode)
				this.isPainted = false;
			this.initializeCounter++;
			((ISupportInitialize)Map).BeginInit();
		}
		public virtual void EndInit() {
			if(--this.initializeCounter == 0) OnEndInit();
			((ISupportInitialize)Map).EndInit();
		}
		public void ZoomOut() {
			Map.ZoomOut();
		}
		public void ZoomIn() {
			Map.ZoomIn();
		}
		public void Zoom(double zoomLevel) {
			Map.Zoom(zoomLevel);
		}
		public void Zoom(double newZoomLevel, MapPoint anchorPoint) {
			Map.Zoom(newZoomLevel, anchorPoint);
		}
		public void Zoom(double zoomLevel, bool animated) {
			Map.Zoom(zoomLevel, animated);
		}
		public void Zoom(double newZoomLevel, MapPoint anchorPoint, bool animated) {
			Map.Zoom(newZoomLevel, anchorPoint, animated);
		}
		public void SetCenterPoint(CoordPoint centerPoint, bool animated) {
			Map.SetCenterPoint(centerPoint, animated);
		}
		public MapHitInfo CalcHitInfo(Point hitPoint) {
			return Map.CalcHitInfo(hitPoint);
		}
		public void SuspendRender() {
			Map.SuspendRender();
		}
		public void ResumeRender() {
			Map.ResumeRender();
		}
		public void ForceRender() {
			Map.ForceRender();
		}
		public void ZoomToRegion(CoordPoint topLeft, CoordPoint bottomRight, double paddingFactor) {
			Map.ZoomToRegion(topLeft, bottomRight, paddingFactor);
		}
		public void ZoomToFit(IEnumerable<MapItem> items) {
			ZoomToFit(items, InnerMap.DefaultZoomPadding);
		}
		public void ZoomToFit(IEnumerable<MapItem> items, double paddingFactor) {
			Map.ZoomToFit(items, paddingFactor);
		}
		public void ZoomToFitLayerItems() {
			ZoomToFitLayerItems(InnerMap.DefaultZoomPadding);
		}
		public void ZoomToFitLayerItems(double paddingFactor) {
			ZoomToFitLayerItems(Map.ActualLayers, paddingFactor);
		}
		public void ZoomToFitLayerItems(IEnumerable<LayerBase> layers) {
			ZoomToFitLayerItems(layers, InnerMap.DefaultZoomPadding);
		}
		public void ZoomToFitLayerItems(IEnumerable<LayerBase> layers, double paddingFactor) {
			CoordBounds bounds = Map.CalculateLayersItemsBounds(layers);
			Map.ZoomToFit(bounds, paddingFactor);
		}
		public void SelectItemsByRegion(CoordPoint leftTop, CoordPoint rightBottom) {
			Map.SelectItemsByRegion(leftTop, rightBottom);
		}
		public void SelectItemsByRegion(Rectangle screenRegion) {
			Map.SelectItemsByRegion(screenRegion);
		}
		#region Units Converter methods
		public MapUnit CoordPointToMapUnit(CoordPoint point) {
			return CoordinateSystem.CoordPointToMapUnit(point, true);
		}
		public MapPoint CoordPointToScreenPoint(CoordPoint point) {
			return UnitConverter.CoordPointToScreenPoint(point);
		}
		public CoordPoint ScreenPointToCoordPoint(Point point) {
			return UnitConverter.ScreenPointToCoordPoint(new MapPoint(point.X, point.Y));
		}
		public CoordPoint MapUnitToCoordPoint(MapUnit unit) {
			return CoordinateSystem.MapUnitToCoordPoint(unit);
		}
		public CoordPoint ScreenPointToCoordPoint(MapPoint point) {
			return UnitConverter.ScreenPointToCoordPoint(point);
		}
		public MapPoint MapUnitToScreenPoint(MapUnit mapUnit) {
			return UnitConverter.MapUnitToScreenPoint(mapUnit, ZoomLevel, CenterPoint, Map.CurrentViewport.ViewportInPixels);
		}
		public MapUnit ScreenPointToMapUnit(MapPoint point) {
			return Map.ScreenPointToMapUnit(point);
		}
		public MapSize CoordToMeasureUnitSize(CoordPoint anchorPoint, MapSize size) {
			return CoordinateSystem.GeoToKilometersSize(anchorPoint, size);
		}
		public MapSize MeasureUnitToCoordSize(CoordPoint anchorPoint, MapSize size) {
			return CoordinateSystem.KilometersToGeoSize(anchorPoint, size);
		}
		#endregion
		#region printing methods
		public void ShowPrintPreview() {
			Map.ShowPrintPreview();
		}
		public void ShowRibbonPrintPreview() {
			Map.ShowRibbonPrintPreview();
		}
		public void Print() {
			Map.Print();
		}
		public void ShowPrintDialog() {
			Map.ShowPrintDialog();
		}
		public void ExportToPdf(string filePath) {
			Map.ExportToPdf(filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Map.ExportToPdf(filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			Map.ExportToPdf(stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Map.ExportToPdf(stream, options);
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			Map.ExportToImage(filePath, format);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			Map.ExportToImage(stream, format);
		}
		public void ExportToRtf(string filePath) {
			Map.ExportToRtf(filePath);
		}
		public void ExportToRtf(Stream stream) {
			Map.ExportToRtf(stream);
		}
		public void ExportToMht(string filePath) {
			Map.ExportToMht(filePath);
		}
		public void ExportToXls(string filePath) {
			Map.ExportToXls(filePath);
		}
		public void ExportToXls(Stream stream) {
			Map.ExportToXls(stream);
		}
		public void ExportToXlsx(string filePath) {
			Map.ExportToXlsx(filePath);
		}
		public void ExportToXlsx(Stream stream) {
			Map.ExportToXlsx(stream);
		}
		#endregion
		#region obsolete methods
		[
		Obsolete("The CalculateKilometersScale method is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public double CalculateKilometersScale() {
			return 0.0;
		}
		[
		Obsolete("The CalculateZoomLevelByScale method is obsolete now."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public double CalculateZoomLevelByScale(double scale) {
			return 0.0;
		}
		#endregion
	}
}
