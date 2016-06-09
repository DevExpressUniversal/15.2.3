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
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.ViewInfo;
using Editors = DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.Utils.NonclientArea;
using DevExpress.Accessibility;
using System.ComponentModel.Design;
using DevExpress.Utils.Drawing.Helpers;
using System.Reflection.Emit;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.CodedUISupport;
using DevExpress.Utils.Gesture;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.Utils.Win;
using DevExpress.Utils.Controls;
namespace DevExpress.XtraEditors {
	[Designer("System.Windows.Forms.Design.PanelDesigner, System.Design"), DefaultProperty("BorderStyle"), Docking(DockingBehavior.Ask), ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), DefaultEvent("Paint")]
	[ToolboxItem(false)]																			   
	public class XtraPanel : XtraScrollableControl {
		private BorderStyle borderStyle = BorderStyle.None;
		[RefreshProperties(System.ComponentModel.RefreshProperties.All), DefaultValue(false)]
		public override bool AutoScroll {
			get { return autoScrollCore; }
			set { autoScrollCore = value; PerformLayout(); }
		}
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public new event EventHandler AutoSizeChanged {
			add { base.AutoSizeChanged += value; }
			remove { base.AutoSizeChanged -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyEventHandler KeyDown {
			add { base.KeyDown += value; }
			remove { base.KeyDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyPressEventHandler KeyPress {
			add { base.KeyPress += value; }
			remove { base.KeyPress -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyEventHandler KeyUp {
			add { base.KeyUp += value; }
			remove { base.KeyUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler TextChanged {
			add { base.TextChanged += value; }
			remove { base.TextChanged -= value; }
		}
		protected void SetState2(int flag, bool etc) {
			System.Reflection.MethodInfo mi = typeof(System.Windows.Forms.Control).GetMethod("SetState2", BindingFlags.NonPublic | BindingFlags.Instance);
			if (mi != null) mi.Invoke(this, new object[] { flag, etc });
		}
		public XtraPanel() {
			SetState2(2048, true);
			TabStop = false;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Selectable, false);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			autoScrollCore = false;
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public override Size GetPreferredSize(Size proposedSize) {
			proposedSize = ApplySizeConstraints(proposedSize);
			Size preferredSize = GetPreferredSizeCore(proposedSize);
			return ApplySizeConstraints(preferredSize);
		}
		Size ApplySizeConstraints(Size size) {
			if(MaximumSize == Size.Empty && MinimumSize == Size.Empty)
				return size;
			Size maxSize = ConvertZeroToUnbounded(MaximumSize);
			size = IntersectSizes(size, maxSize);
			size = UnionSizes(size, MinimumSize);
			return size;
		}
		static Size IntersectSizes(Size a, Size b) {
			return new Size(Math.Min(a.Width, b.Width), Math.Min(a.Height, b.Height));
		}
		static Size UnionSizes(Size a, Size b) {
			return new Size(Math.Max(a.Width, b.Width), Math.Max(a.Height, b.Height));
		}
		static Size ConvertZeroToUnbounded(Size size) {
			if(size.Width == 0)
				size.Width = int.MaxValue;
			if(size.Height == 0)
				size.Height = int.MaxValue;
			return size;
		}
		protected virtual Size GetPreferredSizeCore(Size proposedSize) {
			Size bordersSize = this.SizeFromClientSize(Size.Empty) + base.Padding.Size;
			Size clientAreaMinSize = LayoutEngineGetPreferredSize(proposedSize - bordersSize);
			if(ShouldAddBorderSize(proposedSize))
				return clientAreaMinSize + bordersSize;
			return clientAreaMinSize + base.Padding.Size;
		}
		protected virtual bool ShouldAddBorderSize(Size proposedSize) {
			return true;
		}
		static Func<System.Windows.Forms.Layout.LayoutEngine, Control, Size, Size> getPreferredSize = null;
		protected Size LayoutEngineGetPreferredSize(Size psize) {
			if(getPreferredSize == null) {
				Type layoutEngineType = typeof(System.Windows.Forms.Layout.LayoutEngine);
				var mInfo = layoutEngineType.GetMethod("GetPreferredSize", BindingFlags.Instance | BindingFlags.NonPublic);
				var instance = System.Linq.Expressions.Expression.Parameter(layoutEngineType, "instance");
				var container = System.Linq.Expressions.Expression.Parameter(typeof(Control), "container");
				var proposedConstraints = System.Linq.Expressions.Expression.Parameter(typeof(Size), "proposedConstraints");
				var call = System.Linq.Expressions.Expression.Call(instance, mInfo, container, proposedConstraints);
				getPreferredSize = System.Linq.Expressions.Expression.Lambda<Func<System.Windows.Forms.Layout.LayoutEngine, Control, Size, Size>>(
					call, instance, container, proposedConstraints).Compile();
			}
			return getPreferredSize(LayoutEngine, this, psize);
		}
		protected override void OnResize(EventArgs eventargs) {
			if (base.DesignMode && (this.borderStyle == BorderStyle.None)) {
				base.Invalidate();
			}
			base.OnResize(eventargs);
		}
		static string StringFromBorderStyle(BorderStyle value) {
			Type type = typeof(BorderStyle);
			return (type.ToString() + "." + value.ToString());
		}
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), EditorBrowsable(EditorBrowsableState.Always), SmartTagProperty("Auto Size", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public override bool AutoSize {
			get { return base.AutoSize; }
			set { base.AutoSize = value; }
		}
		public virtual bool ShouldSerializeAutoSizeMode() {
			return AutoSizeMode != AutoSizeMode.GrowOnly;
		}
		public virtual void ResetAutoSizeMode() {
			AutoSizeMode = AutoSizeMode.GrowOnly;
		}
		[Localizable(true), Browsable(true), SmartTagProperty("Auto Size Mode", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual AutoSizeMode AutoSizeMode {
			get {
				return base.GetAutoSizeMode();
			}
			set {
				if (base.GetAutoSizeMode() != value) {
					base.SetAutoSizeMode(value);
				}
			}
		}
		[DefaultValue(BorderStyle.None)]
		public BorderStyle BorderStyle {
			get {
				return this.borderStyle;
			}
			set {
				if (this.borderStyle != value) {
					this.borderStyle = value;
					base.UpdateStyles();
				}
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 65536;
				createParams.ExStyle &= -513;
				createParams.Style &= -8388609;
				switch (this.borderStyle) {
					case BorderStyle.FixedSingle:
						createParams.Style |= 8388608;
						return createParams;
					case BorderStyle.Fixed3D:
						createParams.ExStyle |= 512;
						return createParams;
				}
				return createParams;
			}
		}
		protected override Size DefaultSize { get { return new Size(200, 100); } }
		[DefaultValue(false)]
		public new bool TabStop {
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Bindable(false), Browsable(false)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
	[Designer("DevExpress.Utils.Design.XtraScrollableControlDesigner, " + AssemblyInfo.SRAssemblyDesign, typeof(System.ComponentModel.Design.IDesigner)),
	 Description("Groups a collection of controls and provides auto-scrolling behavior."),
	 ToolboxTabName(AssemblyInfo.DXTabNavigation),
	 ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "XtraScrollableControl")
	]
	[DXToolboxItem(DXToolboxItemKind.Free)]
	[DesignerSerializer("DevExpress.Utils.Design.XtraScrollableControlSerializer, " + AssemblyInfo.SRAssemblyDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design"),
	SmartTagSupport(typeof(ControlBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.Auto),
	SmartTagAction(typeof(XtraScrollableControlActions), "DockInParentContainer", "Dock In ParentContainer", SmartTagActionType.CloseAfterExecute),
	SmartTagAction(typeof(XtraScrollableControlActions), "UndockFromParentContainer", "Undock From ParentContainer", SmartTagActionType.CloseAfterExecute),
	SmartTagFilter(typeof(XtraScrollableControlFilter))
	]
	public class XtraScrollableControl : ScrollableControl, ISupportLookAndFeel, IDXControl, IMouseWheelSupport, IGestureClient, IXtraResizableControl {
		HScrollBarViewInfoWithHandler hScrollBar;
		VScrollBarViewInfoWithHandler vScrollBar;
		SizeGripViewInfoWithHandler sizeGrip;
		int vScrollPos, hScrollPos;
		HorizontalScroll hScrollProperties;
		VerticalScroll vScrollProperties;
		protected bool autoScrollCore;
		Size scrollMargin;
		Size requestedScrollMargin;
		Size userAutoScrollMinSize;
		int scrollBarSmallChange;
		bool alwaysScrollActiveControlIntoView;
		bool fireScrollEventOnMouseWheel;
		Rectangle vscrollRect = Rectangle.Empty;
		Rectangle hscrollRect = Rectangle.Empty;
		Rectangle displayRect = Rectangle.Empty;
		AppearanceDefault defaultAppearance;
		ObjectPainter painter;
		ControlHelper helper;
		ArrayList nonClientViewInfos = new ArrayList();
		protected NonClientAreaManager nonClientAreaManager;
		internal bool IsCaptured = false;
		bool allowTouchScroll = false, invertTouchScroll = false;
		public XtraScrollableControl()
			: base() {
			this.helper = new ControlHelper(this, false);
			this.CreateElements();
			this.nonClientAreaManager = new NonClientAreaManager(this, nonClientViewInfos);
			this.InitElements();
			base.AutoScroll = false;
			this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
			this.autoScrollCore = true;
			this.scrollBarSmallChange = 5;
			this.alwaysScrollActiveControlIntoView = true;
			this.fireScrollEventOnMouseWheel = false;
			this.scrollMargin = Size.Empty;
			this.requestedScrollMargin = Size.Empty;
			this.userAutoScrollMinSize = Size.Empty;
			this.defaultAppearance = null;
			this.SubscribeScrollEvents();
			EnableIXtraResizeableControlInterfaceProxy = true;
		}
		protected virtual void CreateElements() {
			hScrollBar = CreateHScrollBar();
			vScrollBar = CreateVScrollBar();
			sizeGrip = CreateSizeGrip();
			hScrollProperties = new HorizontalScroll(hScrollBar, this);
			vScrollProperties = new VerticalScroll(vScrollBar, this);
			nonClientViewInfos.AddRange(new object[] { hScrollBar, vScrollBar, sizeGrip });
		}
		protected virtual void InitElements() {
			sizeGrip.DrawEmpty = true;
			sizeGrip.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			hScrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			vScrollBar.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected virtual HScrollBarViewInfoWithHandler CreateHScrollBar() {
			return new HScrollBarViewInfoWithHandler(this);
		}
		protected virtual VScrollBarViewInfoWithHandler CreateVScrollBar() {
			return new VScrollBarViewInfoWithHandler(this);
		}
		protected virtual SizeGripViewInfoWithHandler CreateSizeGrip() {
			return new SizeGripViewInfoWithHandler(this);
		}
		#region Public Properties
		[Browsable(false)]
		public new HorizontalScroll HorizontalScroll { get { return hScrollProperties; } }
		[Browsable(false)]
		public new VerticalScroll VerticalScroll { get { return vScrollProperties; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlAutoScroll"),
#endif
		RefreshProperties(System.ComponentModel.RefreshProperties.All), DefaultValue(true), SmartTagProperty("Auto Scroll", "", 1, SmartTagActionType.RefreshBoundsAfterExecute)]
		public override bool AutoScroll {
			get { return autoScrollCore; }
			set { autoScrollCore = value; PerformLayout(); }
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("XtraScrollableControlAutoScrollMargin")]
#endif
		public new Size AutoScrollMargin {
			get { return requestedScrollMargin; }
			set {
				if((value.Width < 0) || (value.Height < 0)) {
					throw new ArgumentException("InvalidArgument");
				}
				SetAutoScrollMargin(value.Width, value.Height);
			}
		}
		bool ShouldSerializeAutoScrollMargin() { return !AutoScrollMargin.Equals(new Size(0, 0)); }
#if !SL
	[DevExpressUtilsLocalizedDescription("XtraScrollableControlAutoScrollMinSize")]
#endif
		public new Size AutoScrollMinSize {
			get { return userAutoScrollMinSize; }
			set {
				if(value == userAutoScrollMinSize) return;
				userAutoScrollMinSize = value;
				AutoScroll = true;
				PerformLayout();
			}
		}
		bool ShouldSerializeAutoScrollMinSize() { return !AutoScrollMinSize.Equals(new Size(0, 0)); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Point AutoScrollPosition {
			get {
				Rectangle rect = GetDisplayRectInternal();
				return new Point(rect.X, rect.Y);
			}
			set {
				if(Created) {
					SetDisplayRectLocation(-value.X, -value.Y);
					SyncScrollbars();
				}
			}
		}
#if !SL
	[DevExpressUtilsLocalizedDescription("XtraScrollableControlDisplayRectangle")]
#endif
		public override Rectangle DisplayRectangle {
			get {
				Rectangle rect = ClientRectangle;
				if(!displayRect.IsEmpty) {
					rect.X = displayRect.X;
					rect.Y = displayRect.Y;
					if(HScrollVisible) {
						rect.Width = displayRect.Width;
					}
					if(VScrollVisible) {
						rect.Height = displayRect.Height;
					}
				}
				if(DockPadding != null) {
					rect.X += DockPadding.Left;
					rect.Y += DockPadding.Top;
					rect.Width -= DockPadding.Left + DockPadding.Right;
					rect.Height -= DockPadding.Top + DockPadding.Bottom;
				}
				return rect;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlLookAndFeel"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return Helper.LookAndFeel; } }
		protected virtual bool ShouldSerializeLookAndFeel() {
			if(LookAndFeel.UseDefaultLookAndFeel) return false;
			return true;
		}
		protected bool CanAccessDefaultAppearance {
			get {
				Control current = this;
				while(current != null) {
					if(!current.IsHandleCreated) return false;
					current = current.Parent;
				}
				return true;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Appearance { get { return Helper.Appearance; } }
		protected virtual AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null) {
					if(!CanAccessDefaultAppearance) return CreateSimpleDefaultAppearance();
					defaultAppearance = CreateDefaultAppearance();
				}
				return defaultAppearance;
			}
		}
		AppearanceDefault CreateSimpleDefaultAppearance() {
			if((LookAndFeel != null) && (LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)) {
				return new AppearanceDefault(CommonSkins.GetSkin(LookAndFeel).GetSystemColor(SystemColors.Control));
			}
			return new AppearanceDefault(SystemColors.Control);
		}
		protected virtual bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		void ResetAppearance() { Appearance.Reset(); }
		protected void OnAppearanceChanged(object sender) {
			if(IsHandleCreated) {
				OnBackColorChanged(EventArgs.Empty);
				Invalidate();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlBackColor"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return GetColor(Appearance.BackColor, DefaultAppearance.BackColor); }
			set { Appearance.BackColor = value; }
		}
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlForeColor"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override Color ForeColor {
			get { return LookAndFeelHelper.CheckTransparentForeColor(LookAndFeel, GetColor(Appearance.ForeColor, DefaultAppearance.ForeColor), this); }
			set { Appearance.ForeColor = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlScrollBarSmallChange"),
#endif
 Category("Behavior"), DefaultValue(5), SmartTagProperty("Small Change", "")]
		public virtual int ScrollBarSmallChange {
			get { return scrollBarSmallChange; }
			set {
				if(value < 1) return;
				scrollBarSmallChange = value;
				SyncScrollbars();
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlAlwaysScrollActiveControlIntoView"),
#endif
 Category("Behavior"), DefaultValue(true), SmartTagProperty("Completely display the active control", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public virtual bool AlwaysScrollActiveControlIntoView {
			get { return alwaysScrollActiveControlIntoView; }
			set { alwaysScrollActiveControlIntoView = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlFireScrollEventOnMouseWheel"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public bool FireScrollEventOnMouseWheel {
			get { return fireScrollEventOnMouseWheel; }
			set { fireScrollEventOnMouseWheel = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlAllowTouchScroll"),
#endif
 Category("Behavior"), DefaultValue(false), SmartTagProperty("Touch Scroll", "")]
		public virtual bool AllowTouchScroll {
			get { return allowTouchScroll; }
			set { allowTouchScroll = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("XtraScrollableControlInvertTouchScroll"),
#endif
 Category("Behavior"), DefaultValue(false)]
		public bool InvertTouchScroll {
			get { return invertTouchScroll; }
			set { invertTouchScroll = value; }
		}
		#endregion
		Control IDXControl.Control { get { return this; } }
		XtraScrollableControlAccessible lCAccesible;
		protected internal virtual BaseAccessible DXAccessible {
			get {
				if(lCAccesible == null) lCAccesible = CreateAccessibleInstance();
				return lCAccesible;
			}
		}
		Color GetColor(Color color, Color defColor) {
			if(color == Color.Empty) {
				return defColor;
			}
			return color;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), SmartTagProperty("Text", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		public new void ScrollControlIntoView(Control activeControl) {
			Rectangle clientRect = ClientRectangle;
			if((AutoScroll && (HScrollVisible || VScrollVisible)) && (((activeControl != null) && (clientRect.Width > 0)) && (clientRect.Height > 0))) {
				int x = displayRect.X;
				int y = displayRect.Y;
				int width = scrollMargin.Width;
				int height = scrollMargin.Height;
				Rectangle activeControlBounds = activeControl.Bounds;
				if(activeControl.Parent != this) {
					activeControlBounds = RectangleToClient(activeControl.Parent.RectangleToScreen(activeControlBounds));
				}
				if(activeControlBounds.X < width) {
					x = (displayRect.X + width) - activeControlBounds.X;
				} else if(((activeControlBounds.X + activeControlBounds.Width) + width) > clientRect.Width) {
					x = clientRect.Width - (((activeControlBounds.X + activeControlBounds.Width) + width) - displayRect.X);
					if(((activeControlBounds.X + x) - displayRect.X) < width) {
						x = (displayRect.X + width) - activeControlBounds.X;
					}
				}
				if(activeControlBounds.Y < height) {
					y = (displayRect.Y + height) - activeControlBounds.Y;
				} else if(((activeControlBounds.Y + activeControlBounds.Height) + height) > clientRect.Height) {
					y = clientRect.Height - (((activeControlBounds.Y + activeControlBounds.Height) + height) - displayRect.Y);
					if(((activeControlBounds.Y + y) - displayRect.Y) < height) {
						y = (displayRect.Y + height) - activeControlBounds.Y;
					}
				}
				SetDisplayRectLocation(x, y);
				SyncScrollbars();
			}
		}
		public new void SetAutoScrollMargin(int x, int y) {
			if(x < 0) x = 0;
			if(y < 0) y = 0;
			if((x != requestedScrollMargin.Width) || (y != requestedScrollMargin.Height)) {
				requestedScrollMargin = new Size(x, y);
				if(AutoScroll && (HScrollVisible || VScrollVisible)) {
					PerformLayout();
				}
			}
		}
		protected ControlHelper Helper { get { return helper; } }
		protected internal virtual ObjectPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected virtual ObjectPainter CreatePainter() {
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				return new XtraScrollableControl.ControlSkinPainter(LookAndFeel);
			return new XtraScrollableControl.ControlPainter();
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault app = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if((LookAndFeel != null) && (LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)) {
				GetSkin().Apply(app);
				LookAndFeelHelper.CheckColors(LookAndFeel, app, this);
			}
			return app;
		}
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected internal virtual XtraScrollableControlAccessible CreateAccessibleInstance() {
			return new XtraScrollableControlAccessible(this);
		}
		protected void PerformNCAreaLayout() {
			if(AutoScroll) nonClientAreaManager.PerformNcAreaLayout();
		}
		protected override void AdjustFormScrollbars(bool displayScrollbars) {
			bool needPerformLayout = false;
			Rectangle rect = GetDisplayRectInternal();
			InitScrollsRects();
			if(!displayScrollbars && (HScrollVisible || VScrollVisible)) {
				needPerformLayout = SetVisibleScrollbars(false, false);
			}
			if(!displayScrollbars) {
				rect.Width = ClientRectangle.Width;
				rect.Height = ClientRectangle.Height;
			} else {
				needPerformLayout |= ApplyScrollbarChanges(rect);
			}
			if(needPerformLayout) {
				PerformLayout();
			}
			PerformNCAreaLayout();
		}
		protected void InitScrollsRects() {
			if(!IsHandleCreated) return;
			hscrollRect = vscrollRect = Rectangle.Empty;
			hScrollBar.Bounds = vScrollBar.Bounds = sizeGrip.Bounds = Rectangle.Empty;
		}
		protected void SetScrollsRects() {
			if(!VScrollVisible && !HScrollVisible) { }
			if(VScrollVisible && !HScrollVisible) {
				SetVScrollRect();
			}
			if(!VScrollVisible && HScrollVisible) {
				SetHScrollRect();
			}
			if(VScrollVisible && HScrollVisible) {
				SetVScrollRect();
				SetHScrollRect();
			}
			SetSizeGripRect();
		}
		void SetSizeGripRect() {
			if(VScrollVisible && HScrollVisible && !IsOverlapVScrollBar)
				sizeGrip.Bounds = new Rectangle(vscrollRect.X, hscrollRect.Y, DefaultVScrollBarWidth, DefaultHScrollBarHeight);
			else
				sizeGrip.Bounds = Rectangle.Empty;
		}
		void SetVScrollRect() {
			vscrollRect.X = Bounds.Width - DefaultVScrollBarWidth;
			vscrollRect.Y = 0;
			vscrollRect.Width = DefaultVScrollBarWidth;
			if(HScrollVisible)
				vscrollRect.Height = Bounds.Height - DefaultHScrollBarHeight;
			else
				vscrollRect.Height = Bounds.Height;
		}
		void SetHScrollRect() {
			hscrollRect.X = 0;
			hscrollRect.Y = Bounds.Height - DefaultHScrollBarHeight;
			if(VScrollVisible && !IsOverlapVScrollBar)
				hscrollRect.Width = Bounds.Width - DefaultVScrollBarWidth;
			else
				hscrollRect.Width = Bounds.Width;
			hscrollRect.Height = DefaultHScrollBarHeight;
		}
		protected virtual void SyncScrollbars() {
			if(IsHandleCreated) {
				SetScrollsRects();
				if(HScrollVisible) {
					hScrollBar.Bounds = hscrollRect;
					HorizontalScroll.SetMinimumInternal(0);
					HorizontalScroll.SetMaximumInternal(displayRect.Width - 1);
					HorizontalScroll.SetSmallChangeInternal(ScrollBarSmallChange);
					HorizontalScroll.SetLargeChangeInternal(ClientRectangle.Width);
					hScrollBar.Value = -displayRect.X;
					hScrollBar.Enabled = Enabled && HorizontalScroll.Enabled;
				} else {
					ResetScrollBarProperties(hScrollBar);
				}
				if(VScrollVisible) {
					vScrollBar.Bounds = vscrollRect;
					VerticalScroll.SetMinimumInternal(0);
					VerticalScroll.SetMaximumInternal(displayRect.Height - 1);
					VerticalScroll.SetSmallChangeInternal(ScrollBarSmallChange);
					VerticalScroll.SetLargeChangeInternal(ClientRectangle.Height);
					vScrollBar.Value = -displayRect.Y;
					vScrollBar.Enabled = Enabled && VerticalScroll.Enabled;
				} else {
					ResetScrollBarProperties(vScrollBar);
				}
			}
		}
		void ResetScrollBarProperties(ScrollBarViewInfoWithHandlerBase scrollBar) {
			scrollBar.SetScrollBarValueInternal(0);
		}
		internal IXtraScrollableControlDesigner GetDesigner() {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			IXtraScrollableControlDesigner designer = host.GetDesigner(this) as IXtraScrollableControlDesigner;
			if(designer == null) return null;
			return designer;
		}
		internal void BeforeSerialize() {
			if(!DesignMode || Disposing || !IsHandleCreated) return;
			IXtraScrollableControlDesigner designer = GetDesigner();
			if(designer == null) return;
			SetDisplayRectLocation(0, 0);
			SyncScrollbars();
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			OnScrollAction(ScrollNotifyAction.MouseMove);
		}
		internal void OnScrollAction(ScrollNotifyAction action) {
			if(HScrollBar == null) return;
			HScrollBar.OnAction(action);
			VScrollBar.OnAction(action);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			OnScrollAction(ScrollNotifyAction.MouseMove);
			if(Capture && CapturedScrollBar != null) CapturedScrollBar.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(IsCaptured) {
				ResetCapture();
				if(CapturedScrollBar != null) {
					CapturedScrollBar.OnMouseUp(e);
					CapturedScrollBar.OnLostCapture();
					return;
				}
			}
			base.OnMouseUp(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			ResetCapture();
			base.OnMouseDown(e);
		}
		void ResetCapture() {
			if(!IsCaptured) return;
			IsCaptured = false;
			Capture = false;
		}
		protected override void OnPaint(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				StyleObjectInfoArgs args = new StyleObjectInfoArgs();
				args.SetAppearance(new AppearanceObject(Appearance, DefaultAppearance));
				args.Bounds = ClientRectangle;
				ObjectPainter.DrawObject(cache, Painter, args);
			}
			base.RaisePaintEvent(this, e);
		}
		Control FindFocusedChild(Control control) {
			if(control == null) return null;
			if(control.HasChildren) {
				for(int n = 0; n < control.Controls.Count; n++) {
					Control fc = FindFocusedChild(control.Controls[n]);
					if(fc != null) return fc;
				}
			}
			return control.Focused ? control : null;
		}
		void ScrollActiveControlIntoView() {
			Control focusedControl = FindFocusedChild(this);
			if(AlwaysScrollActiveControlIntoView && CanScrollControlIntoView(focusedControl)) ScrollControlIntoView(focusedControl);
		}
		protected virtual bool CanScrollControlIntoView(Control control) { return true; }
		protected override void OnGotFocus(EventArgs e) {
			Control focusedControl = FindFocusedChild(this);
			if(!AlwaysScrollActiveControlIntoView && focusedControl == null)
				SelectNextControl(focusedControl, true, true, false, true);
			else
				ScrollActiveControlIntoView();
			base.OnGotFocus(e);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			PerformLayout();
		}
		bool isFirstTimeHandleCreating = true; 
		protected override void OnHandleCreated(EventArgs e) {
			if(isFirstTimeHandleCreating) {
				OnDockChanged(EventArgs.Empty);
				base.OnHandleCreated(e);
				if(!base.DesignMode) PerformLayout();
				isFirstTimeHandleCreating = false;
			} else base.OnHandleCreated(e);
		}
		protected override void OnDockChanged(EventArgs e) {
			switch(Dock) {
				case DockStyle.Bottom:
				case DockStyle.Right:
				case DockStyle.Fill:
					if(IsContainerForm()) SizeGripVisible = true;
					break;
				default:
					SizeGripVisible = false;
					break;
			}
			base.OnDockChanged(e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposed) {
				UnSubscribeScrollEvents();
				UnSubscribeControlEvents();
				helper.Dispose();
				hScrollBar.Dispose();
				vScrollBar.Dispose();
				sizeGrip.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m)) return;
			if(!nonClientAreaManager.ProcessMessage(ref m)) {
				base.WndProc(ref m);
			}
			CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected void SubscribeScrollEvents() {
			hScrollBar.Scroll += new ScrollEventHandler(OnHScroll);
			vScrollBar.Scroll += new ScrollEventHandler(OnVScroll);
			hScrollBar.ValueChanged += new EventHandler(hScrollBar_ValueChanged);
			vScrollBar.ValueChanged += new EventHandler(vScrollBar_ValueChanged);
		}
		protected void UnSubscribeScrollEvents() {
			hScrollBar.Scroll -= new ScrollEventHandler(OnHScroll);
			vScrollBar.Scroll -= new ScrollEventHandler(OnVScroll);
			hScrollBar.ValueChanged -= new EventHandler(hScrollBar_ValueChanged);
			vScrollBar.ValueChanged -= new EventHandler(vScrollBar_ValueChanged);
		}
		void vScrollBar_ValueChanged(object sender, EventArgs e) {
			vScrollPos = vScrollBar.Value;
			UpdateByScrollPos(false);
		}
		void hScrollBar_ValueChanged(object sender, EventArgs e) {
			hScrollPos = hScrollBar.Value;
			UpdateByScrollPos(true);
		}
		protected void UpdateByScrollPos(bool isUpdatingHScroll) {
			if(isUpdatingHScroll) SetDisplayRectLocation(-hScrollPos, displayRect.Y);
			else SetDisplayRectLocation(displayRect.X, -vScrollPos);
		}
		protected void UpdateScrollPositions() {
			hScrollPos = -displayRect.X;
			vScrollPos = -displayRect.Y;
		}
		protected new void SetDisplayRectLocation(int x, int y) {
			int amountX = 0;
			int amountY = 0;
			int minWidth = Math.Min(ClientRectangle.Width - displayRect.Width, 0);
			int minHeight = Math.Min(ClientRectangle.Height - displayRect.Height, 0);
			if(x > 0) x = 0;
			if(y > 0) y = 0;
			if(x < minWidth) x = minWidth;
			if(y < minHeight) y = minHeight;
			if(displayRect.X != x) amountX = x - displayRect.X;
			if(displayRect.Y != y) amountY = y - displayRect.Y;
			displayRect.X = x;
			displayRect.Y = y;
			if((amountX != 0) || ((amountY != 0) && IsHandleCreated)) {
				NativeMethods.RECT rect1 = new NativeMethods.RECT(ClientRectangle);
				NativeMethods.RECT rect2 = new NativeMethods.RECT(ClientRectangle);
				NativeMethods.ScrollWindowEx(Handle, amountX, amountY, IntPtr.Zero, ref rect1, IntPtr.Zero, ref rect2, 7);
				ForceUpdate();
			}
			MethodInfo mi = typeof(System.Windows.Forms.Control).GetMethod("UpdateBounds", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Any, new Type[] { }, new ParameterModifier[] { });
			for(int i = 0; i < Controls.Count; i++) {
				Control control = Controls[i];
				if((control != null && mi != null) && control.IsHandleCreated) {
					mi.Invoke(control, new object[] { });
				}
			}
		}
		protected virtual void ForceUpdate() {
			Update();
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(DevExpress.XtraEditors.XtraForm.ProcessSmartMouseWheel(this, ev))
				return;
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			Point prevDispalyRectLocation = displayRect.Location;
			if(VScrollVisible) {
				e.Handled = true;
				SetDisplayRectLocation(displayRect.X, -CalcYOffset(e.Delta));
				SyncScrollbars();
				if(FireScrollEventOnMouseWheel) {
					OnScroll(this, new XtraScrollEventArgs(ScrollEventType.ThumbPosition, vScrollPos, vScrollBar.Value, ScrollOrientation.VerticalScroll));
					vScrollPos = vScrollBar.Value;
				}
			} else if(HScrollVisible) {
				e.Handled = true;
				SetDisplayRectLocation(-CalcXOffset(e.Delta), displayRect.Y);
				SyncScrollbars();
				if(FireScrollEventOnMouseWheel) {
					OnScroll(this, new XtraScrollEventArgs(ScrollEventType.ThumbPosition, hScrollPos, hScrollBar.Value, ScrollOrientation.HorizontalScroll));
					hScrollPos = hScrollBar.Value;
				}
			}
			if(!displayRect.Location.Equals(prevDispalyRectLocation)) Invalidate();
			base.OnMouseWheel(e);
		}
		int CalcXOffset(int delta) {
			int x = -displayRect.X;
			int width = -(ClientRectangle.Width - displayRect.Width);
			x = Math.Max(x - delta, 0);
			x = Math.Min(x, width);
			return x;
		}
		int CalcYOffset(int delta) {
			int y = -displayRect.Y;
			int height = -(ClientRectangle.Height - displayRect.Height);
			y = Math.Max(y - delta, 0);
			y = Math.Min(y, height);
			return y;
		}
		protected void SetOffset(int deltaX, int deltaY) {
			SetDisplayRectLocation(-CalcXOffset(deltaX), -CalcYOffset(deltaY));
			SyncScrollbars();
			Invalidate();
		}
		protected internal NonClientAreaManager NonClientManager { get { return nonClientAreaManager; } }
		protected internal virtual bool IsOverlapVScrollBar { get { return vScrollBar.IsOverlapScrollBar; } }
		protected internal virtual bool IsOverlapHScrollBar { get { return hScrollBar.IsOverlapScrollBar; } }
		protected internal bool HScrollVisible { get { return hScrollBar.ActualVisible; } set { hScrollBar.SetVisibility(value); } }
		protected internal bool VScrollVisible { get { return vScrollBar.ActualVisible; } set { vScrollBar.SetVisibility(value); } }
		protected internal HScrollBarViewInfoWithHandler HScrollBar { get { return hScrollBar; } }
		protected internal VScrollBarViewInfoWithHandler VScrollBar { get { return vScrollBar; } }
		protected internal SizeGripViewInfoWithHandler SizeGrip { get { return sizeGrip; } }
		protected internal bool SizeGripVisible {
			get { return !sizeGrip.DrawEmpty; }
			set {
				sizeGrip.DrawEmpty = !value;
				Invalidate();
			}
		}
		protected internal void RepaintNcElement(IScrollView element) { nonClientAreaManager.ForceRepaintNcElement(element); }
		ScrollBarViewInfoWithHandlerBase capturedScrollBar = null;
		protected internal ScrollBarViewInfoWithHandlerBase CapturedScrollBar {
			get { return capturedScrollBar; }
			set {
				if(value == capturedScrollBar) return;
				capturedScrollBar = value;
			}
		}
		protected internal int DefaultHScrollBarHeight {
			get {
				if(HScrollBar.TouchMode) return ScrollBarBase.GetHorizontalScrollBarHeight(ScrollUIMode.Touch);
				return ScrollBarBase.GetHorizontalScrollBarHeight(ScrollUIMode.Desktop);
			}
		}
		protected internal int DefaultVScrollBarWidth {
			get {
				if(VScrollBar.TouchMode) return ScrollBarBase.GetVerticalScrollBarWidth(ScrollUIMode.Touch);
				return ScrollBarBase.GetVerticalScrollBarWidth(ScrollUIMode.Desktop);
			}
		}
		SkinElement GetSkin() { return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinGroupPanelNoBorder]; }
		Rectangle GetDisplayRectInternal() {
			if(displayRect.IsEmpty) { displayRect = ClientRectangle; }
			return displayRect;
		}
		void IDXControl.OnAppearanceChanged(object sender) { OnAppearanceChanged(sender); }
		void IDXControl.OnLookAndFeelStyleChanged(object sender) {
			defaultAppearance = null;
			painter = null;
			OnAppearanceChanged(sender);
		}
		void CalcScrollMargin() {
			if(Controls.Count == 0) return;
			scrollMargin = requestedScrollMargin;
			if(DockPadding != null) {
				scrollMargin.Height += DockPadding.Bottom;
				scrollMargin.Width += DockPadding.Right;
			}
			for(int i = 0; i < Controls.Count; i++) {
				Control control = Controls[i];
				if(control == null || !control.Visible) continue;
				switch(control.Dock) {
					case DockStyle.Bottom:
						scrollMargin.Height += control.Size.Height;
						break;
					case DockStyle.Right:
						scrollMargin.Width += control.Size.Width;
						break;
				}
			}
		}
		void UpdateScrollsVisibility(ref bool hScrollVisible, ref bool vScrollVisible, Control control) {
			if(control == null) return;
			switch(control.Dock) {
				case DockStyle.Top:
					hScrollVisible = false;
					break;
				case DockStyle.Bottom:
				case DockStyle.Right:
				case DockStyle.Fill:
					hScrollVisible = false;
					vScrollVisible = false;
					break;
				case DockStyle.Left:
					vScrollVisible = false;
					break;
				default: {
						AnchorStyles styles = control.Anchor;
						if((styles & AnchorStyles.Right) == AnchorStyles.Right) {
							hScrollVisible = false;
						}
						if((styles & AnchorStyles.Left) != AnchorStyles.Left) {
							hScrollVisible = false;
						}
						if((styles & AnchorStyles.Bottom) == AnchorStyles.Bottom) {
							vScrollVisible = false;
						}
						if((styles & AnchorStyles.Top) != AnchorStyles.Top) {
							vScrollVisible = false;
						}
						break;
					}
			}
		}
		void UpdateDisplayRectangleBounds(Rectangle display, ref bool hScrollVisible, ref bool vScrollVisible, ref int width, ref int height) {
			if(Controls.Count == 0) return;
			for(int i = 0; i < Controls.Count; i++) {
				bool hScrollVisibleDefault = true;
				bool vScrollVisibleDefault = true;
				Control control = Controls[i];
				if(control == null || !control.Visible) continue;
				UpdateScrollsVisibility(ref hScrollVisibleDefault, ref vScrollVisibleDefault, control);
				if(hScrollVisibleDefault || vScrollVisibleDefault) {
					int w = ((-display.X + control.Bounds.X) + control.Bounds.Width) + scrollMargin.Width;
					int h = ((-display.Y + control.Bounds.Y) + control.Bounds.Height) + scrollMargin.Height;
					if((w > width) && hScrollVisibleDefault) {
						hScrollVisible = true;
						width = w;
					}
					if((h > height) && vScrollVisibleDefault) {
						vScrollVisible = true;
						height = h;
					}
				}
			}
		}
		void SubscribeChildControlEvents(Control control) {
			control.MouseMove += OnChildControlMouseMove;
			control.GotFocus += new EventHandler(OnChildControlGotFocus);
			control.ControlAdded += new ControlEventHandler(OnChildControlAdded);
			control.ControlRemoved += new ControlEventHandler(OnChildControlRemoved);
			if(control.HasChildren) foreach(Control child in control.Controls) SubscribeChildControlEvents(child);
		}
		void OnChildControlMouseMove(object sender, MouseEventArgs e) {
			OnScrollAction(ScrollNotifyAction.MouseMove);
		}
		void UnSubscribeChildControlEvents(Control control) {
			control.MouseMove -= OnChildControlMouseMove;
			control.GotFocus -= new EventHandler(OnChildControlGotFocus);
			control.ControlAdded -= new ControlEventHandler(OnChildControlAdded);
			control.ControlRemoved -= new ControlEventHandler(OnChildControlRemoved);
			if(control.HasChildren) foreach(Control child in control.Controls) UnSubscribeChildControlEvents(child);
		}
		void OnChildControlAdded(object sender, ControlEventArgs e) {
			SubscribeChildControlEvents(e.Control);
		}
		void OnChildControlRemoved(object sender, ControlEventArgs e) {
			UnSubscribeChildControlEvents(e.Control);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			SubscribeChildControlEvents(e.Control);
			base.OnControlAdded(e);
			IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
			if(ixChild != null) foreach(EventHandler eh in new List<EventHandler>(subscriptions)) {
					SubscribeIXChild(ixChild, eh, false);
					eh(this, null);
				}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			UnSubscribeChildControlEvents(e.Control);
			base.OnControlRemoved(e);
			IXtraResizableControl ixChild = e.Control as IXtraResizableControl;
			if(subscriptions.Count > 0 && ixChild != null && Controls.Count == 0) foreach(EventHandler eh in new List<EventHandler>(subscriptions)) {
					UnSubscribeIXChild(ixChild, eh, false);
					eh(this, null);
				}
		}
		void UnSubscribeControlEvents() {
			foreach(Control control in Controls) {
				UnSubscribeChildControlEvents(control);
			}
		}
		protected void OnChildControlGotFocus(object sender, EventArgs e) {
			if(IsHandleCreated) BeginInvoke(new MethodInvoker(ScrollActiveControlIntoView));
		}
		protected virtual bool ApplyScrollbarChanges(Rectangle display) {
			bool changed = false;
			bool hscroll = false;
			bool vscroll = false;
			Rectangle clientRect = ClientRectangle;
			Rectangle rect = clientRect;
			if(HScrollVisible) clientRect.Height += DefaultHScrollBarHeight;
			else rect.Height -= DefaultHScrollBarHeight;
			if(VScrollVisible) clientRect.Width += DefaultVScrollBarWidth;
			else rect.Width -= DefaultVScrollBarWidth;
			int width = rect.Width;
			int height = rect.Height;
			CalcScrollMargin();
			if(!userAutoScrollMinSize.IsEmpty) {
				width = userAutoScrollMinSize.Width + scrollMargin.Width;
				height = userAutoScrollMinSize.Height + scrollMargin.Height;
				hscroll = true;
				vscroll = true;
			}
			UpdateDisplayRectangleBounds(display, ref hscroll, ref vscroll, ref width, ref height);
			if(width <= clientRect.Width) hscroll = false;
			if(height <= clientRect.Height) vscroll = false;
			if(hscroll) clientRect.Height -= DefaultHScrollBarHeight;
			if(vscroll) clientRect.Width -= DefaultVScrollBarWidth;
			if(hscroll && (height > clientRect.Height)) vscroll = true;
			if(vscroll && (width > clientRect.Width)) hscroll = true;
			if(!hscroll) width = clientRect.Width;
			if(!vscroll) height = clientRect.Height;
			hscroll &= HorizontalScroll.visible;
			vscroll &= VerticalScroll.visible;
			changed = SetVisibleScrollbars(hscroll, vscroll) || changed;
			if(HScrollVisible || VScrollVisible) {
				changed = SetDisplayRectangleSize(width, height) || changed;
			} else {
				SetDisplayRectangleSize(width, height);
			}
			SyncScrollbars();
			return changed;
		}
		protected virtual bool SetDisplayRectangleSize(int width, int height) {
			bool displayRectangleChanged = false;
			if((displayRect.Width != width) || (displayRect.Height != height)) {
				displayRect.Width = width;
				displayRect.Height = height;
				displayRectangleChanged = true;
			}
			int widthValue = ClientRectangle.Width - width;
			int heigthValue = ClientRectangle.Height - height;
			if(widthValue > 0) widthValue = 0;
			if(heigthValue > 0) heigthValue = 0;
			int x = displayRect.X;
			int y = displayRect.Y;
			if(!HScrollVisible) x = 0;
			if(!VScrollVisible) y = 0;
			if(x < widthValue) x = widthValue;
			if(y < heigthValue) y = heigthValue;
			SetDisplayRectLocation(x, y);
			return displayRectangleChanged;
		}
		bool SetVisibleScrollbars(bool horiz, bool vert) {
			bool visible = false;
			if(((!horiz && HScrollVisible) || (horiz && !HScrollVisible)) || ((!vert && VScrollVisible) || (vert && !VScrollVisible))) {
				visible = true;
			}
			if(visible) {
				int x = displayRect.X;
				int y = displayRect.Y;
				if(!horiz) x = 0;
				if(!vert) y = 0;
				SetDisplayRectLocation(x, y);
				HScrollVisible = horiz;
				VScrollVisible = vert;
				UpdateStyles();
			}
			return visible;
		}
		bool IsContainerForm() {
			if(!IsHandleCreated) return false;
			Form f = FindForm();
			return Parent == f && f != null && !f.IsMdiChild;
		}
		protected void OnHScroll(object sender, ScrollEventArgs e) {
			XtraScrollEventArgs args = new XtraScrollEventArgs(e.Type, hScrollPos, e.NewValue, ScrollOrientation.HorizontalScroll);
			int maximum = -(ClientRectangle.Width - this.displayRect.Width);
			vScrollPos = CalcScrollPosition(-displayRect.X, maximum, e.Type, HorizontalScroll);
			UpdateByScrollPos(true);
			OnScroll(sender, args);
		}
		public new event XtraScrollEventHandler Scroll;
		protected virtual void OnScroll(object sender, XtraScrollEventArgs e) {
			if(Scroll != null) {
				Scroll(this, e);
			}
		}
		protected virtual int CalcScrollPosition(int value, int maximum, ScrollEventType scrollType, XtraScrollProperties scrollProperties) {
			switch(scrollType) {
				case ScrollEventType.LargeIncrement:
					if(value >= (maximum - scrollProperties.LargeChange))
						value = maximum;
					else
						value += scrollProperties.LargeChange;
					break;
				case ScrollEventType.LargeDecrement:
					if(value <= scrollProperties.LargeChange)
						value = 0;
					else
						value -= scrollProperties.LargeChange;
					break;
				case ScrollEventType.SmallIncrement:
					if(value >= (maximum - scrollProperties.SmallChange))
						value = maximum;
					else
						value += scrollProperties.SmallChange;
					break;
				case ScrollEventType.SmallDecrement:
					if(value <= 0)
						value = 0;
					else
						value -= scrollProperties.SmallChange;
					break;
			}
			return value;
		}
		protected void OnVScroll(object sender, ScrollEventArgs e) {
			XtraScrollEventArgs args = new XtraScrollEventArgs(e.Type, vScrollPos, e.NewValue, ScrollOrientation.VerticalScroll);
			int maximum = -(ClientRectangle.Height - this.displayRect.Height);
			vScrollPos = CalcScrollPosition(-displayRect.Y, maximum, e.Type, VerticalScroll);
			UpdateByScrollPos(false);
			OnScroll(sender, args);
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		class ControlPainter : StyleObjectPainter {
			public ControlPainter() { }
			public override void DrawObject(ObjectInfoArgs e) {
				GetStyle(e).DrawBackground(e.Cache, e.Bounds);
			}
		}
		class ControlSkinPainter : SkinCustomPainter {
			public ControlSkinPainter(ISkinProvider provider) : base(provider) { }
			protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
				return new SkinElementInfo(CommonSkins.GetSkin(base.Provider)[CommonSkins.SkinGroupPanelNoBorder], e.Bounds);
			}
		}
		public interface IXtraScrollableControlDesigner {
			void Update();
		}
		#region IGestureClient Members
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null)
					gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		bool CanTouchScroll { 
			get {
				IntPtr handle = NativeMethods.GetFocus();
				Control control = Control.FromHandle(handle);
				if(control != null) {
					IPopupControl popupControl = control as IPopupControl;
					if(popupControl == null)
						popupControl = control.Parent as IPopupControl;
					if(popupControl != null && popupControl.PopupWindow != null && popupControl.PopupWindow.Visible)
						return false;
				}
				return AutoScroll && AllowTouchScroll; 
			} 
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(!CanTouchScroll) return GestureAllowArgs.None;
			int allow = GestureHelper.GC_PAN_ALL;
			if(!HScrollBar.Visible || !HScrollBar.Enabled) allow &= ~GestureHelper.GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY;
			if(!VScrollBar.Visible || !VScrollBar.Enabled) allow &= ~GestureHelper.GC_PAN_WITH_SINGLE_FINGER_VERTICALLY;
			return new GestureAllowArgs[] { new GestureAllowArgs() { GID = GID.PAN, AllowID = allow } };
		}
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		void IGestureClient.OnBegin(GestureArgs info) { }
		void IGestureClient.OnEnd(GestureArgs info) { }
		Point over = Point.Empty;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				over = Point.Empty;
				return;
			}
			if(!CanTouchScroll || delta.IsEmpty) return;
			if(delta.Y != 0 && VScrollBar.Visible && VScrollBar.Enabled) {
				int oldValue = VScrollBar.Value;
				SetScrollDelta(InvertTouchScroll ? -delta.Y : delta.Y, VScrollBar);
				over.Y = (oldValue == VScrollBar.Value ? over.Y + delta.Y : 0);
			}
			if(delta.X != 0 && VScrollBar.Visible && VScrollBar.Enabled) {
				int oldValue = HScrollBar.Value;
				SetScrollDelta(InvertTouchScroll ? -delta.X : delta.X, HScrollBar);
				over.X = (oldValue == HScrollBar.Value ? over.X + delta.X : 0);
			}
			overPan = over;
		}
		void SetScrollDelta(int delta, ScrollBarViewInfoWithHandlerBase scroll) {
			scroll.Value = scroll.GetScrollBarValue(scroll.Value + delta);
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		#endregion
		#region IXtraResizableControl
		[DefaultValue(true), Browsable(false)]
		public bool EnableIXtraResizeableControlInterfaceProxy { get; set; }
		protected virtual IXtraResizableControl GetInnerIXtraResizableControl() {
			if(Disposing || Controls == null || Controls.Count != 1 || !EnableIXtraResizeableControlInterfaceProxy || IsDockFill()) return null;
			IXtraResizableControl ixChild = Controls[0] as IXtraResizableControl;
			return ixChild;
		}
		bool IsDockFill() {
			return Controls.Count == 1 && Controls[0].Dock != DockStyle.Fill;
		}
		Size IXtraResizableControl.MinSize {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? new Size(1, 1) : CalculateMinMaxSize(true, ixChild.MinSize);
			}
		}
		Size IXtraResizableControl.MaxSize {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? new Size(0, 0) : CalculateMinMaxSize(false, ixChild.MaxSize);
			}
		}
		Size CalculateMinMaxSize(bool isMin, Size childSize) {
			Size corrected = SizeFromClientSize(childSize);
			Size resilt = new Size(childSize.Width > 0 ? corrected.Width : 0, childSize.Height > 0 ? corrected.Height : 0);
			return resilt;
		}
		void SubscribeIXChild(IXtraResizableControl ixChild, EventHandler eh, bool addToCollection) {
			if(ixChild != null) ixChild.Changed += eh;
			if(addToCollection) subscriptions.Add(eh);
		}
		void UnSubscribeIXChild(IXtraResizableControl ixChild, EventHandler eh, bool removeFromCollection) {
			if(ixChild != null) ixChild.Changed -= eh;
			if(removeFromCollection) subscriptions.Remove(eh);
		}
		List<EventHandler> subscriptions = new List<EventHandler>();
		event EventHandler IXtraResizableControl.Changed {
			add {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				SubscribeIXChild(ixChild, value, true);
			}
			remove {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				UnSubscribeIXChild(ixChild, value, true);
			}
		}
		bool IXtraResizableControl.IsCaptionVisible {
			get {
				IXtraResizableControl ixChild = GetInnerIXtraResizableControl();
				return ixChild == null ? false : ixChild.IsCaptionVisible;
			}
		}
		#endregion
	}
	#region Accessibility
	public class XtraScrollableControlAccessible : ContainerBaseAccessible {
		XtraScrollableControl control;
		public XtraScrollableControlAccessible(XtraScrollableControl owner)
			: base(owner) {
			control = owner;
		}
		public override Control GetOwnerControl() { return control; }
		protected override string GetDescription() {
			return AccLocalizer.Active.GetLocalizedString(AccStringId.ScrollableControlDescription);
		}
		protected override string GetName() {
			return control.Name;
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override ChildrenInfo GetChildrenInfo() {
			ChildrenInfo info = base.GetChildrenInfo();
			info["Scroll"] = 2;
			return info;
		}
		protected override string GetDefaultAction() {
			return AccLocalizer.Active.GetLocalizedString(AccStringId.ScrollableControlDefaultAction);
		}
		protected override void OnChildrenCountChanged() {
			AddChild(new ScrollableControlScrollBarAccessible(control.HScrollBar, control));
			AddChild(new ScrollableControlScrollBarAccessible(control.VScrollBar, control));
			base.OnChildrenCountChanged();
		}
	}
	public class ScrollableControlScrollBarAccessible : ScrollBarAccessible {
		ScrollBarViewInfoWithHandlerBase scroll;
		XtraScrollableControl owner;
		public ScrollableControlScrollBarAccessible(ScrollBarViewInfoWithHandlerBase scroll, XtraScrollableControl owner)
			: base(scroll) {
			this.scroll = scroll;
			this.owner = owner;
		}
		public override Rectangle ScreenBounds {
			get {
				Rectangle controlRect = scroll.Bounds;
				controlRect.Location = owner.PointToScreen(controlRect.Location);
				return controlRect;
			}
		}
		protected override IScrollBar Scroll { get { return scroll; } }
		public override Control GetOwnerControl() { return owner; }
		public override bool IsVisible { get { return !scroll.Bounds.IsEmpty; } }
		protected override void OnChildrenCountChanged() {
			AddChild(new ScrollableControlScrollChildButton(Scroll, GetIncName(), GetIncDesc(), new GetBoundsDelegate(GetIncButtonBounds), new MethodInvoker(DoInc)));
			AddChild(new ScrollableControlScrollChildButton(Scroll, GetIncAreaName(), GetIncAreaDesc(), new GetBoundsDelegate(GetIncAreaBounds), new MethodInvoker(DoAreaInc)));
			AddChild(new ScrollableControlScrollBarIndicator(Scroll, GetIndicatorName(), GetIndicatorDesc(), new GetBoundsDelegate(GetIndicatorBounds)));
			AddChild(new ScrollableControlScrollChildButton(Scroll, GetDecAreaName(), GetDecAreaDesc(), new GetBoundsDelegate(GetDecAreaBounds), new MethodInvoker(DoAreaDec)));
			AddChild(new ScrollableControlScrollChildButton(Scroll, GetDecName(), GetDecDesc(), new GetBoundsDelegate(GetDecButtonBounds), new MethodInvoker(DoDec)));
		}
		protected class ScrollableControlScrollChildButton : ScrollChildButton {
			IScrollBar scroll;
			public ScrollableControlScrollChildButton(IScrollBar scroll, string name, string description, GetBoundsDelegate getBounds, MethodInvoker doAction)
				: base(scroll, name, description, getBounds, doAction) {
				this.scroll = scroll;
			}
			public override bool IsVisible {
				get {
					if (((ScrollBarViewInfoWithHandlerBase)scroll).Bounds.IsEmpty) return false;
					return true;
				}
			}
		}
		protected class ScrollableControlScrollBarIndicator : ScrollableControlScrollChildButton {
			public ScrollableControlScrollBarIndicator(IScrollBar scroll, string name, string description, GetBoundsDelegate getBounds)
				: base(scroll, name, description, getBounds, null) {
			}
		}
	}
	#endregion
	#region XtraScrollProperties
	public abstract class XtraScrollProperties {
		bool enabled;
		internal bool visible = true;
		int largeChange;
		int smallChange;
		ScrollBarViewInfoWithHandlerBase scrollBar;
		bool largeChangeSetExternally = false;
		bool smallChangeSetExternally = false;
		bool maximumSetExternally = false;
		bool minimumSetExternally = false;
		protected XtraScrollableControl parent;
		protected XtraScrollProperties(ScrollBarViewInfoWithHandlerBase scrollBar, XtraScrollableControl parent) {
			this.parent = parent;
			this.scrollBar = scrollBar;
			this.enabled = true;
			this.visible = true;
		}
		protected abstract ScrollBarType ScrollBarType { get; }
		public bool Enabled {
			get { return enabled; }
			set {
				if (Enabled == value) return;
				enabled = value;
				parent.PerformLayout();
			}
		}
		public bool Visible {
			get { return GetActualVisibility(); }
			set {
				if (Visible == value) return;
				visible = value;
				parent.PerformLayout();
			}
		}
		public int Value {
			get { return scrollBar.Value; }
			set {
				if (Value == value) return;
				scrollBar.Value = value;
				parent.PerformLayout();
			}
		}
		public int LargeChange {
			get { return GetActualLargeChange(); }
			set {
				if (LargeChange == value) return;
				if (value < 0) {
					largeChangeSetExternally = false;
					return;
				}
				largeChange = value;
				largeChangeSetExternally = true;
				parent.PerformLayout();
			}
		}
		public int SmallChange {
			get { return GetActualSmallChange(); }
			set {
				if (SmallChange == value) return;
				if (value < 0) {
					smallChangeSetExternally = false;
					return;
				}
				smallChange = value;
				smallChangeSetExternally = true;
				parent.PerformLayout();
			}
		}
		public int Maximum {
			get { return scrollBar.Maximum; }
			set {
				if (Maximum == value) return;
				if (value < 0) {
					maximumSetExternally = false;
					return;
				}
				scrollBar.Maximum = value;
				maximumSetExternally = true;
				parent.PerformLayout();
			}
		}
		public int Minimum {
			get { return scrollBar.Minimum; }
			set {
				if (Minimum == value) return;
				if (value < 0) {
					minimumSetExternally = false;
					return;
				}
				scrollBar.Minimum = value;
				minimumSetExternally = true;
				parent.PerformLayout();
			}
		}
		protected abstract bool GetActualVisibility();
		protected int GetActualLargeChange() {
			if (largeChangeSetExternally) return Math.Min(largeChange, (Maximum - Minimum) + 1); ;
			return scrollBar.LargeChange;
		}
		protected int GetActualSmallChange() {
			if (smallChangeSetExternally) return Math.Min(smallChange, LargeChange);
			return scrollBar.SmallChange;
		}
		protected internal void SetLargeChangeInternal(int value) {
			if (largeChangeSetExternally) return;
			scrollBar.LargeChange = value;
		}
		protected internal void SetSmallChangeInternal(int value) {
			if (smallChangeSetExternally) return;
			scrollBar.SmallChange = value;
		}
		protected internal void SetMaximumInternal(int value) {
			if (maximumSetExternally) return;
			scrollBar.Maximum = value;
		}
		protected virtual internal void SetMinimumInternal(int value) {
			if (minimumSetExternally) return;
			scrollBar.Minimum = value;
		}
	}
	public class HorizontalScroll : XtraScrollProperties {
		public HorizontalScroll(HScrollBarViewInfoWithHandlerBase scrollBar, XtraScrollableControl parent) : base(scrollBar, parent) { }
		protected override ScrollBarType ScrollBarType { get { return ScrollBarType.Horizontal; } }
		protected override bool GetActualVisibility() {
			return parent.HScrollVisible;
		}
	}
	public class VerticalScroll : XtraScrollProperties {
		public VerticalScroll(VScrollBarViewInfoWithHandlerBase scrollBar, XtraScrollableControl parent) : base(scrollBar, parent) { }
		protected override ScrollBarType ScrollBarType { get { return ScrollBarType.Vertical; } }
		protected override bool GetActualVisibility() {
			return parent.VScrollVisible;
		}
	}
	#endregion
	public delegate void XtraScrollEventHandler(object sender, XtraScrollEventArgs e);
	[ComVisible(true)]
	public class XtraScrollEventArgs : EventArgs {
		int newValue;
		int oldValue;
		ScrollOrientation scrollOrientation;
		readonly ScrollEventType type;
		public XtraScrollEventArgs(ScrollEventType type, int newValue) {
			this.oldValue = -1;
			this.type = type;
			this.newValue = newValue;
		}
		public XtraScrollEventArgs(ScrollEventType type, int oldValue, int newValue) {
			this.oldValue = -1;
			this.type = type;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		public XtraScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll) {
			this.oldValue = -1;
			this.type = type;
			this.newValue = newValue;
			this.scrollOrientation = scroll;
		}
		public XtraScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll) {
			this.oldValue = -1;
			this.type = type;
			this.newValue = newValue;
			this.scrollOrientation = scroll;
			this.oldValue = oldValue;
		}
		public int NewValue { get { return newValue; } set { newValue = value; } }
		public int OldValue { get { return oldValue; } }
		public ScrollOrientation ScrollOrientation { get { return scrollOrientation; } }
		public ScrollEventType Type { get { return type; } }
	}
	public enum ScrollOrientation {
		HorizontalScroll,
		VerticalScroll
	}
}
