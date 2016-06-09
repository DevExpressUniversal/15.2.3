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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils {
	public enum XPThemeSupport {
		Disabled, Enabled, Default
	}
	public enum Locations { Default, Left, Right, Top, Bottom };
	public enum GroupElementLocation { Default, BeforeText, AfterText};
}
namespace DevExpress.XtraEditors.Controls {
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IAutoHeightControl {
		bool SupportsAutoHeight { get; }
		int CalcHeight(GraphicsCache cache);
		event EventHandler HeightChanged;
	}
	public enum BorderStyles {
		NoBorder = 0,
		Simple = 1,
		Flat = 2,
		HotFlat = 3,
		UltraFlat = 4,
		Style3D = 5,
		Office2003 = 6,
		Default = 7
	};
	public enum ButtonPredefines {
		Glyph = 0,
		Down = -1,
		Up = -2,
		Left = -3,
		Right = -4,
		Ellipsis = 1,
		Delete = 2,
		OK = 3,
		Plus = 4,
		Minus = 5,
		Redo = 6,
		Undo = 7,
		Combo = -5,
		SpinUp = -6,
		SpinDown = -7,
		SpinLeft = -8,
		SpinRight = -9,
		Close = -10,
		DropDown = 8,
		Search = 9,
		Clear = 10,
	}
	public enum ButtonStates {
		None = 0,
		Hottrack = 1,
		Push = 2,
		DeepPush = 3,
		Disabled = 4
	};
	[Flags]
	public enum BorderSide {
		None = 0,
		Left = 1,
		Top = 2,
		Right = 4,
		Bottom = 8,
		All = 15
	}
	public enum PopupMenuStyle {
		Default,
		Classic,
		RadialMenu
	}
}
namespace DevExpress.Utils.Drawing {
	public class ButtonHelper {
		static ObjectPainter[] painters;
		static ObjectPainter[] Painters {
			get { 
				if(painters == null) {
					painters = new ObjectPainter[Enum.GetValues(typeof(BorderStyles)).Length];
					painters[(int)BorderStyles.NoBorder] = new EmptyButtonObjectPainter();
					painters[(int)BorderStyles.Simple] = new SimpleButtonObjectPainter();
					painters[(int)BorderStyles.Flat] = new FlatButtonObjectPainter();
					painters[(int)BorderStyles.HotFlat] = new HotFlatButtonObjectPainter();
					painters[(int)BorderStyles.UltraFlat] = new UltraFlatButtonObjectPainter();
					painters[(int)BorderStyles.Style3D] = new Style3DButtonObjectPainter();
					painters[(int)BorderStyles.Office2003] = new Office2003ButtonObjectPainter();
					painters[(int)BorderStyles.Default] = null;
				}
				return painters;
			}
		}
		public static ObjectPainter GetPainter(BorderStyles style) {
			return GetPainter(style, null);
		}
		public static ObjectPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel) {
			return GetPainter(style, lookAndFeel, null);
		}
		public static ObjectPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel, ISkinProvider provider) {
			if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
			if(provider == null) provider = lookAndFeel;
			if(style == BorderStyles.Default) {
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					return new SkinButtonObjectPainter(provider);
				}
				return lookAndFeel.Painter.Button;
			}
			return Painters[(int)style];
		}
	}
	public class SizeGripHelper {
		public static SizeGripObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			return GetPainter(lookAndFeel, null);
		}
		public static SizeGripObjectPainter GetPainter(UserLookAndFeel lookAndFeel, ISkinProvider provider) {
			if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
			if(provider == null) provider = lookAndFeel;
			UserLookAndFeel lf = lookAndFeel.ActiveLookAndFeel;
			if(lf.ActiveStyle == ActiveLookAndFeelStyle.Skin) return new SkinSizeGripObjectPainter(provider);
			return lf.Painter.SizeGrip;
		}
	}
	public class BorderHelper {
		static BorderPainter painterNoBorder;
		static BorderPainter painterSimple;
		static BorderPainter painterFlat;
		static BorderPainter painterHotFlat;
		static BorderPainter painterUltraFlat;
		static BorderPainter painterStyle3D;
		static BorderPainter painterOffice2003;
		delegate BorderPainter CreatePainterDelegate();
		static BorderPainter GetPainterCore(BorderStyles style){
			switch(style) {
			case BorderStyles.Default: return null;
			case BorderStyles.NoBorder: return painterNoBorder ?? CreatePainter(ref painterNoBorder, delegate { return new EmptyBorderPainter(); });
			case BorderStyles.Simple: return painterSimple ?? CreatePainter(ref painterSimple, delegate { return new SimpleBorderPainter(); });
			case BorderStyles.Flat: return painterFlat ?? CreatePainter(ref painterFlat, delegate { return new TextFlatBorderPainter(); });
			case BorderStyles.HotFlat: return painterHotFlat ?? CreatePainter(ref painterHotFlat, delegate { return new HotFlatBorderPainter(); });
			case BorderStyles.UltraFlat: return painterUltraFlat ?? CreatePainter(ref painterUltraFlat, delegate { return new UltraFlatBorderPainter(); });
			case BorderStyles.Style3D: return painterStyle3D ?? CreatePainter(ref painterStyle3D, delegate { return new Border3DSunkenPainter(); });
			case BorderStyles.Office2003: return painterOffice2003 ?? CreatePainter(ref painterOffice2003, delegate { return new Office2003BorderPainter(); });
			}
			throw new ArgumentException();
		}
		static BorderPainter CreatePainter(ref BorderPainter painterNoBorder, CreatePainterDelegate p) {
			painterNoBorder = p();
			return painterNoBorder;
		}
		public static BorderPainter GetPainter(BorderStyles style) {
			return GetPainter(style, null);
		}
		public static BorderPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel) {
			return GetPainter(style, lookAndFeel, null);
		}
		public static BorderPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel, BorderPainter skinBorder) {
			if(style == BorderStyles.Default) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					if(skinBorder != null) return skinBorder;
					return new SkinTextBorderPainter(lookAndFeel);
				}
				return lookAndFeel.Painter.Border;
			}
			return GetPainterCore(style);
		}
		public static BorderPainter GetGridPainter(BorderStyles style, UserLookAndFeel lookAndFeel) {
			BorderPainter res;
			if(style == BorderStyles.Default) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					return new SkinGridBorderPainter(lookAndFeel);
				}
				res = lookAndFeel.Painter.Border;
			} else 
				res = GetPainterCore(style);
			if(res is UltraFlatBorderPainter) res = GetPainterCore(BorderStyles.Simple);
			return res;
		}
		public static BorderPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel, bool isPrinting, BorderPainter skinBorder) {
			if(!isPrinting) return GetPainter(style, lookAndFeel, skinBorder);
			if(style == BorderStyles.Default) {
				if(lookAndFeel == null) lookAndFeel = UserLookAndFeel.Default;
				if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP) return GetPainterCore(BorderStyles.Simple);
				return lookAndFeel.Painter.Border;
			}
			return GetPainterCore(style);
		}
		public static BorderPainter GetPainter(BorderStyles style, UserLookAndFeel lookAndFeel, bool isPrinting) {
			return GetPainter(style, lookAndFeel, isPrinting, null);
		}
	}
}
namespace DevExpress.Utils.Win {
	public interface IPopupControl {
		void ClosePopup();
		Control PopupWindow { get; }
		bool AllowMouseClick(Control control, Point mousePosition);
		bool SuppressOutsideMouseClick { get; }
	}
	public interface IPopupControlEx : IPopupControl {
		void CheckClosePopup(Control activatedControl);
		void WindowHidden(Control control);
	}
	public interface IPopupServiceControl {
		bool SetVisibleCore(Control control, bool newVisible);
		bool SetSimpleVisibleCore(Control control, IntPtr parentForm, bool newVisible);
		bool WndProc(ref System.Windows.Forms.Message m);
		bool IsDummy {
			get;}
		void UpdateTopMost(IntPtr handle);
		void PopupShowing(IPopupControl popup);
		void PopupClosed(IPopupControl popup);
		void EmulateFormFocus(IntPtr formHandle);
	}
	public class NativeControlPaintArgs {
		private Rectangle bounds;
		private Brush backBrush;
		private Graphics graphics;
		private ButtonPredefines kind;
		private bool enabled;
		private ButtonStates state;
		private bool kindDrawn;
		BorderSide borderSides;
		public Rectangle Bounds {
			get { return bounds; }
			set { bounds = value; }
		}
		public Graphics Graphics {
			get { return graphics; }
		}
		public ButtonStates State {
			get { return state; }
			set { state = value; }
		}
		public ButtonPredefines Kind {
			get { return kind; }
			set {
				kind = value;
			}
		}
		public bool Enabled {
			get { return enabled; }
		}
		public bool KindDrawn {
			get { return kindDrawn; }
			set { kindDrawn = value; }
		}
		public Brush BackBrush {
			get { return backBrush; }
		}
		public BorderSide BorderSides {
			get { return borderSides; }
			set { borderSides = value; }
		}
		public NativeControlPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled) {
			this.graphics = graphics;
			this.backBrush = backBrush;
			this.bounds = bounds;
			this.kind = kind;
			this.enabled = enabled;
			this.state = state;
			this.kindDrawn = false;
			if(!Enabled) {
				this.state = ButtonStates.Disabled;
			}
			this.borderSides = BorderSide.All;
		}
	}
	public class NativeControlAdvPaintArgs : NativeControlPaintArgs {
		string themeName;
		int part, partState;
		bool fillBackground;
		public NativeControlAdvPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled) : this(graphics, bounds, kind, state, backBrush, enabled, "") {
		}
		public NativeControlAdvPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled, string themeName) : this(graphics, bounds, kind, state, backBrush, enabled, themeName, -1, -1, false) {
		}
		public NativeControlAdvPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled, string themeName, int part, int partState, bool fillBackground) : base(graphics, bounds, kind, state, backBrush, enabled) {
			this.fillBackground = fillBackground;
			this.themeName = themeName;
			this.part = part;
			this.partState = partState;
		}
		public bool FillBackground { get { return fillBackground; } }
		public string ThemeName { get { return themeName; } }
		public int Part { 
			get { return part; } 
			set { part = value; }
		}
		public int PartState { 
			get { return partState; } 
			set { partState = value; }
		}
	}
	public class NativeCheckControlPaintArgs : NativeControlPaintArgs {
		CheckState checkState;
		public CheckState CheckState {
			get { return checkState; } 
			set {
				checkState = value;
			}
		}
		public NativeCheckControlPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled, CheckState check) : base(graphics, bounds, kind, state, backBrush, enabled) {
			this.checkState = check;
		}
		public NativeCheckControlPaintArgs(Graphics graphics, Rectangle bounds, ButtonPredefines kind, ButtonStates state, Brush backBrush, bool enabled) : this(graphics, bounds, kind, state, backBrush, enabled, CheckState.Unchecked) {
		}
	}
	public class NativeControlPaint {
		static NativeControlPaint() {}
		public static bool IsPossibleNativePaint() { 
			return Painter.ThemesEnabled;
		}
		public static Rectangle DrawControlEdges(NativeControlPaintArgs e) { 
			Painter.DrawBorder(e);
			return e.Bounds; 
		}
		public static Size GetControlEdgesSize(NativeControlPaintArgs e) { 
			return Painter.CalcBorderSize(e);
		}
		public static Rectangle DrawControl(NativeControlPaintArgs e) { 
			e.KindDrawn = true;
			Painter.DrawButton(e);
			return e.Bounds; 
		}
		public static Rectangle DrawHeader(NativeControlPaintArgs e) { 
			Painter.DrawHeader(e);
			return e.Bounds;
		}
		public static Rectangle DrawOpenCloseButton(NativeControlPaintArgs e, bool open) { 
			Painter.DrawOpenCloseButton(e, open);
			return e.Bounds;
		}
	}
}
namespace DevExpress.Utils.Design {
	public static class WindowsFormsDesignTimeSettings {
		static Font defaultDesignTimeFont;
		[Browsable(false)]
		public static Font DefaultDesignTimeFont {
			get {
				if(defaultDesignTimeFont == null) defaultDesignTimeFont = AppearanceObject.CreateDefaultFont();
				return defaultDesignTimeFont;
			}
		}
		[Browsable(false)]
		public static float DesignTimeTouchScaleFactor {
			get {
				return 1f;
			}
		}
		public static string DesignTimeSkinName { get { return "DevExpress Design"; } }
		public static void ApplyDesignSettings(BaseAppearanceCollection collection) {
			ApplyDesignTimeFont(collection);
		}
		public static void ApplyDesignTimeFont(object fontObject) {
			IAppearanceDefaultFontProvider fontProvider = fontObject as IAppearanceDefaultFontProvider;
			if(fontProvider == null) return;
			if(DefaultDesignTimeFont.Equals(AppearanceObject.DefaultFont))
				fontProvider.DefaultFont = null;
			else
				fontProvider.DefaultFont = DefaultDesignTimeFont;
		}
		public static void ApplyDesignSettings(AppearanceObject appearance) {
		}
		public static void ApplyDesignSettings(XtraForm form) {
			ApplyDesignTimeFont(form.LookAndFeel);
			form.LookAndFeel.SetStyle(LookAndFeelStyle.Skin, false, false, DesignTimeSkinName);
		}
		public static void ApplyDesignSettings(XtraUserControl form) {
			ApplyDesignTimeFont(form.LookAndFeel);
		}
	}
}
namespace DevExpress.XtraEditors {
	public enum WindowsFormsFontBehavior { Default, UseControlFont, UseTahoma, UseWindowsFont, ForceWindowsFont, ForceTahoma }
	public static class WindowsFormsSettings {
		static WindowsFormsSettings() {
			AllowDpiScale = true;
		}
		static WindowsFormsFontBehavior? fontBehavior;
		static DefaultBoolean allowPixelScrolling = DefaultBoolean.Default;
		static DragScrollThumbBeyondControlMode dragScrollThumbBeyondControlMode = DragScrollThumbBeyondControlMode.Default;
		static int dragScrollThumbBeyondControlThreshold = 50;
		static DefaultBoolean allowAutoScale = DefaultBoolean.Default;
		static DefaultBoolean allowHoverAnimation = DefaultBoolean.Default;
		static DefaultBoolean rightToLeft = DefaultBoolean.Default, rightToLeftLayout = DefaultBoolean.Default;
		static PopupMenuStyle popupMenuStyle = PopupMenuStyle.Default;
		static TouchUIMode touchUIMode = TouchUIMode.Default;
		static float touchScaleFactor = 2.0f;
		public static DefaultBoolean AllowRibbonFormGlass {
			get;
			set;
		}
		public static DefaultBoolean AllowHoverAnimation {
			get { return allowHoverAnimation; }
			set { allowHoverAnimation = value; }
		}
		public static bool AllowDpiScale { get; set; }
		public static bool GetAllowHoverAnimation(ISkinProvider provider) { 
			if(AllowHoverAnimation != DefaultBoolean.Default)
				return AllowHoverAnimation == DefaultBoolean.True;
			Skin skin = CommonSkins.GetSkin(provider);
			return !skin.Properties.ContainsProperty(CommonSkins.OptAllowHoverAnimation) || skin.Properties.GetBoolean(CommonSkins.OptAllowHoverAnimation);
		}
		public static WindowsFormsFontBehavior FontBehavior {
			get {
				if(fontBehavior == null) return WindowsFormsFontBehavior.Default;
				return fontBehavior.Value;
			}
			set {
				if(FontBehavior == value) return;
				fontBehavior = value;
				FontBehaviorHelper.Update();
			}
		}
		public static void EnableFormSkins() { SkinManager.EnableFormSkins(); }
		public static bool AllowFormSkins { get { return SkinManager.AllowFormSkins; } }
		public static bool AllowWindowGhosting { get { return SkinManager.AllowWindowGhosting; } set { SkinManager.AllowWindowGhosting = value; } }
		public static void EnableFormSkinsIfXP() { SkinManager.EnableFormSkinsIfNotVista();  }
		public static void SetDPIAware() { SkinManager.SetDPIAware();  }
		public static void DisableFormSkins() { SkinManager.DisableFormSkins(); }
		public static void EnableMdiFormSkins() { SkinManager.EnableMdiFormSkins(); }
		public static void DisableMdiFormSkins() { SkinManager.DisableMdiFormSkins();  }
		public static bool AllowArrowDragIndicators { get { return SkinManager.AllowArrowDragIndicators; } set { SkinManager.AllowArrowDragIndicators = value; } }
		public static DefaultBoolean AllowPixelScrolling {
			get { return allowPixelScrolling; }
			set { allowPixelScrolling = value; }
		}
		public static DragScrollThumbBeyondControlMode DragScrollThumbBeyondControlMode {
			get { return dragScrollThumbBeyondControlMode; }
			set { dragScrollThumbBeyondControlMode = value; }
		}
		public static int DragScrollThumbBeyondControlThreshold {
			get { return dragScrollThumbBeyondControlThreshold; }
			set {
				if(value < 0) value = 0;
				dragScrollThumbBeyondControlThreshold = value;
			}
		}
		public static bool IsAllowPixelScrolling {
			get {
				if(AllowPixelScrolling == DefaultBoolean.True) return true;
				return false;
			}
		}
		public static DefaultBoolean RightToLeft {
			get { return rightToLeft; }
			set {
				if(RightToLeft == value) return;
				rightToLeft = value;
				UserLookAndFeel.Default.OnStyleChanged();
			}
		}
		public static DefaultBoolean RightToLeftLayout {
			get { return rightToLeftLayout; }
			set {
				if(RightToLeftLayout == value) return;
				rightToLeftLayout = value;
				UserLookAndFeel.Default.OnStyleChanged();
			}
		}
		public static ScrollUIMode ScrollUIMode { get { return ScrollBarBase.UIMode; } set { ScrollBarBase.UIMode = value; } }
		static DefaultBoolean allowOverpanWindow = DefaultBoolean.Default;
		internal static bool GetAllowOverpanApplicationWindowCore() {
			if(ScrollBarBase.GetUIMode(ScrollUIMode) == XtraEditors.ScrollUIMode.Touch) return false;
			return AllowOverpanApplicationWindow != DefaultBoolean.False;
		}
		public static DefaultBoolean AllowOverpanApplicationWindow {
			get { return allowOverpanWindow; }
			set { allowOverpanWindow = value; }
		}
		public static UserLookAndFeel DefaultLookAndFeel { get { return UserLookAndFeel.Default; } }
		public static DevExpress.Utils.Paint.DXDashStyle FocusRectStyle { get { return XPaint.FocusRectStyle; } set { XPaint.FocusRectStyle = value; } }
		public static void ForceTextRenderPaint() { XPaint.ForceTextRenderPaint(); }
		public static void ForceGDIPlusPaint() { XPaint.ForceGDIPlusPaint(); }
		public static void ForceAPIPaint() { XPaint.ForceAPIPaint(); }
		public static bool DefaultAllowHtmlDraw { get { return XPaint.DefaultAllowHtmlDraw; } set { XPaint.DefaultAllowHtmlDraw = value; } }
		public static Font DefaultFont { get { return AppearanceObject.DefaultFont; } set { AppearanceObject.DefaultFont = value; } }
		public static Font DefaultMenuFont { get { return AppearanceObject.DefaultMenuFont; } set { AppearanceObject.DefaultMenuFont = value; } }
		public static Font DefaultPrintFont { get { return AppearanceObjectPrint.DefaultPrintFont; } set { AppearanceObjectPrint.DefaultPrintFont = value; } }
		public static bool SmartMouseWheelProcessing { get { return DevExpress.XtraEditors.Drawing.MouseWheelHelper.SmartMouseWheelProcessing; } set { DevExpress.XtraEditors.Drawing.MouseWheelHelper.SmartMouseWheelProcessing = value; } }
		static bool showTouchScrollBarOnMouseMove = true;
		public static bool ShowTouchScrollBarOnMouseMove { get { return showTouchScrollBarOnMouseMove; } set { showTouchScrollBarOnMouseMove = value; } }
		public static DefaultBoolean AllowAutoScale {
			get { return allowAutoScale; }
			set { allowAutoScale = value; }
		}
		public static bool GetAllowAutoScale() { return AllowAutoScale != DefaultBoolean.False; }
		public static void ApplyDemoSettings() {
			AllowPixelScrolling = DefaultBoolean.True;
			ScrollUIMode = XtraEditors.ScrollUIMode.Touch;
		}
		public static PopupMenuStyle PopupMenuStyle {
			get { return popupMenuStyle; }
			set { popupMenuStyle = value; }
		}
		public static bool GetIsRightToLeft(Control control) {
			return GetRightToLeft(control) == System.Windows.Forms.RightToLeft.Yes;
		}
		public static bool GetIsRightToLeft(RightToLeft rightToLeft) {
			return GetRightToLeft(rightToLeft) == System.Windows.Forms.RightToLeft.Yes;
		}
		public static RightToLeft GetRightToLeft(Control control) {
			return GetRightToLeft(control == null ? System.Windows.Forms.RightToLeft.No : control.RightToLeft);
		}
		public static bool GetIsRightToLeftLayout(Control control) {
			return GetIsRightToLeftLayout(control == null ? false : ExtractRightToLeftLayout(control));
		}
		static bool ExtractRightToLeftLayout(Control control) {
			Form form = control as Form;
			if(form != null) return form.RightToLeftLayout;
			return false;
		}
		public static RightToLeft GetRightToLeft(RightToLeft rightToLeft) {
			if(RightToLeft == DefaultBoolean.False) return System.Windows.Forms.RightToLeft.No;
			if(RightToLeft == DefaultBoolean.True) return System.Windows.Forms.RightToLeft.Yes;
			return rightToLeft;
		}
		public static bool GetIsRightToLeftLayout(bool rightToLeft) {
			if(RightToLeftLayout == DefaultBoolean.False) return false;
			if(RightToLeftLayout == DefaultBoolean.True) return true;
			return rightToLeft;
		}
		public static void LoadApplicationSettings() {
			((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.Design.UserLookAndFeelDefault.Default).LoadSettings(null);
}
		[DefaultValue(TouchUIMode.Default)]
		public static TouchUIMode TouchUIMode {
			get { return touchUIMode; }
			set {
				if(TouchUIMode == value)
					return;
				touchUIMode = value;
				DefaultLookAndFeel.OnStyleChanged();
			}
		}
		[DefaultValue(2.0f)]
		public static float TouchScaleFactor {
			get { return touchScaleFactor; }
			set {
				if(TouchScaleFactor == value)
					return;
				touchScaleFactor = value;
				DefaultLookAndFeel.OnStyleChanged();
			}
		}
	}
}
namespace DevExpress.XtraEditors.Helpers {
	public class RightToLeftHelper {
		public static void UpdateRightToLeftDockStyle(Control parent) {
			foreach(Control c in parent.Controls) {
				if(c.Dock == DockStyle.Left) c.Dock = DockStyle.Right;
				else if(c.Dock == DockStyle.Right) c.Dock = DockStyle.Left;
				UpdateRightToLeftDockStyle(c);
			}
		}
	}
}
