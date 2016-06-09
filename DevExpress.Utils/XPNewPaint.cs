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
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Paint;
namespace DevExpress.Utils.WXPaint {
	public class WXPPainterArgs {
		Rectangle bounds;
		string themeName;
		int partId;
		int stateId;
		IntPtr themeHandle;
		public WXPPainterArgs(string themeName, int partId, int stateId) {
			this.themeName = themeName;
			this.partId = partId;
			this.stateId = stateId;
			this.themeHandle = IntPtr.Zero;
			this.bounds = Rectangle.Empty;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public string ThemeName { get { return themeName; } set { themeName = value; } }
		public int StateId { get { return stateId; } set { stateId = value; } }
		public int PartId { get { return partId; } set { partId = value; } }
		public IntPtr ThemeHandle { get { return themeHandle; } set { themeHandle = value; } }
	}
	[SuppressUnmanagedCodeSecurity()]
	public class WXPPainter {
		Hashtable themes = new Hashtable();
		Hashtable brushes = new Hashtable();
		Hashtable bounds = new Hashtable();
		Hashtable sizes = new Hashtable();
		int themesEnabled = -1;
		public event EventHandler ThemeChanged;
		[ThreadStatic]
		static WXPPainter defaultPainter;
		static WXPPainter() {
		}
		public WXPPainter() {
			CheckThemesEnabled();
			Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(OnUserPreferenceChanged);
		}
		protected virtual void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e) {
			OnThemeChanged();
		}
		public static WXPPainter Default { 
			get {
				if(defaultPainter == null) defaultPainter = CreateDefaultPainter();
				return defaultPainter;
			}
		}
		static WXPPainter CreateDefaultPainter() {
			if(IsOnVista) return new VistaWXPainter();
			return new WXPPainter();
		}
		public static bool IsOnVista {
			get { return Environment.OSVersion.Version.Major == 6; }
		}
		public bool IsVista {
			get {
				return IsOnVista;
			}
		}
		protected virtual void CheckThemesEnabled() {
			try {
				if(System.Environment.OSVersion.Version.Major < 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor == 0)) {
					themesEnabled = 0;
					return;
				}
				if(!NativeVista.IsThemeActive()) {
					themesEnabled = 0;
					return;
				}
			}
			catch {
				themesEnabled = 0;
				return;
			}
			CreateBrushes();
			if(IsApplicationThemed && (NativeVista.GetThemeAppProperties() & 0x2) == 2) themesEnabled = 1;
			else
				themesEnabled = 0;
		}
		public virtual bool IsApplicationThemed { get { return NativeVista.IsAppThemed();} }
		public virtual bool ThemesEnabled { get { return themesEnabled == 1; } }
		protected virtual IntPtr GetTheme(string name) {
			if(themesEnabled != 1) return IntPtr.Zero;
			if(!themes.ContainsKey(name)) 
				themes[name] = NativeVista.OpenThemeData(IntPtr.Zero, name);
			return (IntPtr)themes[name];
		}
		public virtual void DisableWindowTheme(Control control) {
			if(!control.IsHandleCreated || !ThemesEnabled) return;
			NativeVista.SetWindowTheme(control.Handle, "", "");
		}
		public virtual void ResetWindowTheme(Control control) {
			if(!control.IsHandleCreated || !ThemesEnabled) return;
			NativeVista.SetWindowTheme(control.Handle, null, null);
		}
		public virtual void OnThemeChanged() {
			themes.Clear();
			CheckThemesEnabled();
			CreateBrushes();
			this.bounds.Clear();
			this.sizes.Clear();
			if(ThemeChanged != null) ThemeChanged(this, EventArgs.Empty);
		}
		void CreateBrushes() {
			foreach(Brush brush in brushes.Values) { 
				brush.Dispose();
			}
			brushes.Clear();
			if(!ThemesEnabled) return;
		}
		protected virtual void UpdateThemeHandle(WXPPainterArgs pArgs) {
			if(pArgs.ThemeHandle == IntPtr.Zero) 
				pArgs.ThemeHandle = GetTheme(pArgs.ThemeName);
		}
		public virtual Brush GetThemeBrush(WXPPainterArgs pArgs, int propertyId) {
			UpdateThemeHandle(pArgs);
			string hash = GetHash(pArgs, propertyId);
			Brush brush = this.brushes[hash] as Brush;
			if(brush == null) {
				Color clr = GetThemeColor(pArgs, propertyId);
				brush = new SolidBrush(clr);
				this.brushes[hash] = brush;
			}
			return brush;
		}
		public virtual Color GetThemeColor(WXPPainterArgs pArgs, int propertyId) {
			UpdateThemeHandle(pArgs);
			int color = 0;
			NativeVista.GetThemeColor(pArgs.ThemeHandle, pArgs.PartId, pArgs.StateId, propertyId, out color);
			return APIXPaint.GDIColorToColor(color);
		}
		public virtual Region GetThemeBackgroundRegion(WXPPainterArgs pArgs) {
			UpdateThemeHandle(pArgs);
			Region res = null;
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			IntPtr region = IntPtr.Zero;
			NativeVista.GetThemeBackgroundRegion(pArgs.ThemeHandle, IntPtr.Zero, pArgs.PartId, pArgs.StateId, ref rr, ref region);
			if(region == IntPtr.Zero) return res;
			IntPtr destRgn = NativeMethods.CreateRectRgn(0,0,0,0),
				reg1 = NativeMethods.CreateRectRgn(rr.Left, rr.Top, rr.Right, rr.Bottom);
			NativeMethods.CombineRgn(destRgn, region, reg1, 3);
			if(destRgn != IntPtr.Zero) {
				res = Region.FromHrgn(destRgn);
			}
			NativeMethods.DeleteObject(region);
			NativeMethods.DeleteObject(destRgn);
			NativeMethods.DeleteObject(reg1);
			return res;
		}
		public virtual Rectangle GetThemeContentRect(WXPPainterArgs pArgs) {
			UpdateThemeHandle(pArgs);
			NativeMethods.RECT source = new NativeMethods.RECT(pArgs.Bounds), res = new NativeMethods.RECT();
			NativeVista.GetThemeBackgroundContentRect(pArgs.ThemeHandle, IntPtr.Zero, pArgs.PartId, pArgs.StateId, ref source, ref res);
			return res.ToRectangle();
		}
		public Rectangle GetThemeBounds(WXPPainterArgs pArgs) { return GetThemeBounds(pArgs, pArgs.Bounds); }
		public virtual Rectangle GetThemeBounds(WXPPainterArgs pArgs, Rectangle rect) {
			UpdateThemeHandle(pArgs);
			string hash = GetHash(pArgs, rect);
			object val = bounds[hash];
			if(val != null) return (Rectangle)val;
			NativeMethods.RECT source, res = new NativeMethods.RECT();
			source = new NativeMethods.RECT(rect);
			if(rect.Height < 12) source.Bottom += (12 - rect.Height);
			IntPtr retCode = NativeVista.GetThemeBackgroundExtent(pArgs.ThemeHandle, IntPtr.Zero, pArgs.PartId, pArgs.StateId, ref source, ref res);
			Rectangle r = res.ToRectangle();
			bounds[hash] = r;
			return r;
		}
		public virtual Size GetThemeSize(WXPPainterArgs pArgs, bool bestSize) {
			UpdateThemeHandle(pArgs);
			NativeMethods.SIZE size = new NativeMethods.SIZE();
			string hash = GetHash(pArgs.ThemeHandle, pArgs.PartId, pArgs.StateId, Rectangle.Empty);
			object res = sizes[hash];
			if(res != null) return (Size)res;
			NativeVista.GetThemePartSize(pArgs.ThemeHandle, IntPtr.Zero, pArgs.PartId, pArgs.StateId, IntPtr.Zero, bestSize ? 1 : 0, ref size);
			Size newSize = size.ToSize();
			sizes[hash] = newSize;
			if(sizes.Count > 200) sizes.Clear();
			return newSize;
		}
		public virtual void DrawThemeParentBackground(Control parent, Graphics g, Rectangle bounds) {
			if(parent == null || !parent.IsHandleCreated) return;
			NativeMethods.RECT rr = new NativeMethods.RECT(bounds);
			IntPtr hdc = g.GetHdc();
			try {
				NativeVista.DrawThemeParentBackground(parent.Handle, hdc, ref rr);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		public virtual void DrawTheme(WXPPainterArgs pArgs, Graphics g, Brush fillBrush) {
			UpdateThemeHandle(pArgs);
			Rectangle resRect = Rectangle.Empty;
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			if(fillBrush != null) {
				Region reg = GetThemeBackgroundRegion(pArgs);
				if(reg != null) { 
					g.FillRegion(fillBrush, reg);
					reg.Dispose();
				}
			}
			IntPtr hdc = g.GetHdc();
			try {
				NativeVista.DrawThemeBackground(pArgs.ThemeHandle, hdc, pArgs.PartId, pArgs.StateId, ref rr, ref rr);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		const int BDR_RAISEDOUTER = 0x0001;
		const int BDR_SUNKENOUTER = 0x0002;
		const int BF_ADJUST	   = 0x2000;
		const int BF_LEFT		 = 0x0001;
		const int BF_TOP		  = 0x0002;
		const int BF_RIGHT		= 0x0004;
		const int BF_BOTTOM	   = 0x0008;
		const int BF_RECT  = (BF_LEFT | BF_TOP | BF_RIGHT | BF_BOTTOM);
		public virtual void DrawThemeEdge(WXPPainterArgs pArgs, Graphics g, Brush fillBrush) {
			UpdateThemeHandle(pArgs);
			Rectangle resRect = Rectangle.Empty;
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			IntPtr hdc = g.GetHdc();
			try {
				NativeVista.DrawThemeEdge(pArgs.ThemeHandle, hdc, pArgs.PartId, pArgs.StateId, ref rr, BDR_RAISEDOUTER | BDR_SUNKENOUTER, BF_RECT | BF_ADJUST, ref rr);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		public virtual Rectangle GetThemeMargins(WXPPainterArgs pArgs, int margin) {
			UpdateThemeHandle(pArgs);
			Rectangle resRect = Rectangle.Empty;
			NativeVista.XPMARGINS margins = new NativeVista.XPMARGINS();
			NativeVista.GetThemeMargins(pArgs.ThemeHandle, IntPtr.Zero, pArgs.PartId, pArgs.StateId, margin, IntPtr.Zero, ref margins);
			return margins.ToRectangle();
		}
		protected string GetHash(WXPPainterArgs pArgs, int propertyId) {
			return GetHash(pArgs.ThemeHandle, pArgs.PartId, pArgs.StateId, new Rectangle(propertyId, propertyId, propertyId, propertyId));
		}
		protected string GetHash(WXPPainterArgs pArgs, Rectangle bounds) {
			return GetHash(pArgs.ThemeHandle, pArgs.PartId, pArgs.StateId, bounds);
		}
		protected string GetHash(WXPPainterArgs pArgs) {
			return GetHash(pArgs.ThemeHandle, pArgs.PartId, pArgs.StateId, pArgs.Bounds);
		}
		protected string GetHash(IntPtr theme, int part, int state, Rectangle bounds) {
			return string.Format("{0},{1},{2},{3}", theme.ToString(), part, state, bounds.ToString());
		}
		public const int XP_TMT_SIZINGMARGINS = 3601, XP_TMT_CONTENTMARGINS = 3602, XP_TMT_CAPTIONMARGINS = 3603,
			XP_TMT_BORDERCOLOR = 3801, XP_TMT_FILLCOLOR = 3802,
			XP_TMT_TEXTCOLOR = 3803, XP_TMT_EDGELIGHTCOLOR = 3804, XP_TMT_EDGEHIGHLIGHTCOLOR = 3805, XP_TMT_EDGESHADOWCOLOR = 3806;
		int useWindowsXPThemeColors = -1;
		public int UseWindowsXPThemeColors {
			get { return useWindowsXPThemeColors; }
			set {
				if(value < 0) value = -1;
				if(value > 2) value = 2;
				if(UseWindowsXPThemeColors == value) return;
				useWindowsXPThemeColors = value;
				OnThemeChanged();
				DevExpress.Utils.Drawing.Office2003Colors.Default.Init();
				LookAndFeel.UserLookAndFeel.Default.OnStyleChanged();
			}
		}
		public virtual XPThemeType GetXPThemeType() {
			const int SZSIZE = 256;
			if(UseWindowsXPThemeColors != -1) return (XPThemeType)(UseWindowsXPThemeColors + 1);
			if(!ThemesEnabled) return XPThemeType.Unknown;
			string themeFileName, themeColor, themeSize;
			themeFileName = new string(' ', SZSIZE * 2);
			themeColor = new string(' ', SZSIZE * 2);
			themeSize = new string(' ', SZSIZE * 2);
			if(NativeVista.GetCurrentThemeName(themeFileName, SZSIZE, themeColor, SZSIZE, themeSize, SZSIZE) != IntPtr.Zero) return XPThemeType.Unknown;
			themeFileName = Trim(themeFileName).ToUpper(CultureInfo.InvariantCulture);
			themeColor = Trim(themeColor).ToUpper(CultureInfo.InvariantCulture);
			if(themeFileName.EndsWith("LUNA.MSSTYLES") || IsVista) {
				if(IsVista) return XPThemeType.Metallic;
				if(themeColor == "NORMALCOLOR") return XPThemeType.NormalColor;
				if(themeColor == "HOMESTEAD") return XPThemeType.Homestead;
				if(themeColor == "METALLIC") return XPThemeType.Metallic;
				return XPThemeType.NormalColor;
			}
			return XPThemeType.Unknown;
		}
		string Trim(string s) {
			s = s.Trim();
			int len = s.Length;
			if(len == 0) return string.Empty;
			if(s[len - 1] == '\0') s = s.Substring(0, len - 1);
			return s.Trim();
		}
	}
	public class VistaWXPainter : WXPPainter {
		int scrollWidth = -1;
		public override Size GetThemeSize(WXPPainterArgs pArgs, bool bestSize) {
			if(scrollWidth == -1) scrollWidth = SystemInformation.VerticalScrollBarWidth;
			switch(pArgs.ThemeName) {
				case "PROGRESS":
				case "progress": return Size.Empty;
				case "combobox":
				case "button": 
					if(pArgs.PartId == XPConstants.BP_RADIOBUTTON || pArgs.PartId == XPConstants.BP_CHECKBOX)
						return base.GetThemeSize(pArgs, bestSize);
					return new Size(scrollWidth, scrollWidth);
				case "spin": return new Size(scrollWidth, scrollWidth / 2);
				case "scrollbar": return base.GetThemeSize(pArgs, bestSize);
			}
			return base.GetThemeSize(pArgs, bestSize);
		}
		public override Rectangle GetThemeMargins(WXPPainterArgs pArgs, int margin) {
			return base.GetThemeMargins(pArgs, margin);
		}
		public override Rectangle GetThemeContentRect(WXPPainterArgs pArgs) {
			if(pArgs.ThemeName == "header") {
				return Rectangle.Inflate(pArgs.Bounds, -2, -2);
			}
			return base.GetThemeContentRect(pArgs);
		}
		public override Rectangle GetThemeBounds(WXPPainterArgs pArgs, Rectangle rect) {
			if(pArgs.ThemeName == "header") {
				return Rectangle.Inflate(rect, 2, 2);
			}
			return base.GetThemeBounds(pArgs, rect);
		}
	}
	public enum XPThemeType { Unknown, NormalColor, Homestead, Metallic };
	public class XPConstants {
		public const int 
			BP_PUSHBUTTON = 1,
			BP_RADIOBUTTON = 2,
			BP_CHECKBOX = 3,
			BP_GROUPBOX = 4,
			BP_USERBUTTON = 5;
		public const int 
			PBS_NORMAL = 1,
			PBS_HOT = 2,
			PBS_PRESSED = 3,
			PBS_DISABLED = 4,
			PBS_DEFAULTED = 5,
			RBS_UNCHECKEDNORMAL = 1,
			RBS_UNCHECKEDHOT = 2,
			RBS_UNCHECKEDPRESSED = 3,
			RBS_UNCHECKEDDISABLED = 4,
			RBS_CHECKEDNORMAL = 5,
			RBS_CHECKEDHOT = 6,
			RBS_CHECKEDPRESSED = 7,
			RBS_CHECKEDDISABLED = 8,
			CBS_UNCHECKEDNORMAL = 1,
			CBS_UNCHECKEDHOT = 2,
			CBS_UNCHECKEDPRESSED = 3,
			CBS_UNCHECKEDDISABLED = 4,
			CBS_CHECKEDNORMAL = 5,
			CBS_CHECKEDHOT = 6,
			CBS_CHECKEDPRESSED = 7,
			CBS_CHECKEDDISABLED = 8,
			CBS_MIXEDNORMAL = 9,
			CBS_MIXEDHOT = 10,
			CBS_MIXEDPRESSED = 11,
			CBS_MIXEDDISABLED = 12,
			GBS_NORMAL = 1,
			GBS_DISABLED = 2,
			CP_DROPDOWNBUTTON = 1,
			CBXS_NORMAL = 1,
			CBXS_HOT = 2,
			CBXS_PRESSED = 3,
			CBXS_DISABLED = 4,
			SBP_ARROWBTN = 1,
			SBP_THUMBBTNHORZ = 2,
			SBP_THUMBBTNVERT = 3,
			SBP_LOWERTRACKHORZ = 4,
			SBP_UPPERTRACKHORZ = 5,
			SBP_LOWERTRACKVERT = 6,
			SBP_UPPERTRACKVERT = 7,
			SBP_GRIPPERHORZ = 8,
			SBP_GRIPPERVERT = 9,
			SBP_SIZEBOX = 10,
			ABS_UPNORMAL = 1,
			ABS_UPHOT = 2,
			ABS_UPPRESSED = 3,
			ABS_UPDISABLED = 4,
			ABS_DOWNNORMAL = 5,
			ABS_DOWNHOT = 6,
			ABS_DOWNPRESSED = 7,
			ABS_DOWNDISABLED = 8,
			ABS_LEFTNORMAL = 9,
			ABS_LEFTHOT = 10,
			ABS_LEFTPRESSED = 11,
			ABS_LEFTDISABLED = 12,
			ABS_RIGHTNORMAL = 13,
			ABS_RIGHTHOT = 14,
			ABS_RIGHTPRESSED = 15,
			ABS_RIGHTDISABLED = 16,
			SCRBS_NORMAL = 1,
			SCRBS_HOT = 2,
			SCRBS_PRESSED = 3,
			SCRBS_DISABLED = 4,
			SZB_RIGHTALIGN = 1,
			SZB_LEFTALIGN = 2,
			SPNP_UP = 1,
			SPNP_DOWN = 2,
			SPNP_UPHORZ = 3,
			SPNP_DOWNHORZ = 4,
			UPS_NORMAL = 1,
			UPS_HOT = 2,
			UPS_PRESSED = 3,
			UPS_DISABLED = 4,
			DNS_NORMAL = 1,
			DNS_HOT = 2,
			DNS_PRESSED = 3,
			DNS_DISABLED = 4,
			UPHZS_NORMAL = 1,
			UPHZS_HOT = 2,
			UPHZS_PRESSED = 3,
			UPHZS_DISABLED = 4,
			DNHZS_NORMAL = 1,
			DNHZS_HOT = 2,
			DNHZS_PRESSED = 3,
			DNHZS_DISABLED = 4,
			TMT_COLOR = 204,
			TMT_BORDERCOLOR = 3801,
			TMT_FILLCOLOR = 3802,
			TMT_TEXTCOLOR = 3803,
			TMT_GRAYTEXT = 1618,
			TMT_EDGELIGHTCOLOR = 3804,
			TMT_EDGEHIGHLIGHTCOLOR = 3805,
			TMT_EDGESHADOWCOLOR = 3806,
			TMT_EDGEDKSHADOWCOLOR = 3807,
			TMT_EDGEFILLCOLOR = 3808,
			TMT_TRANSPARENTCOLOR = 3809,
			TMT_GRADIENTCOLOR1 = 3810,
			TMT_GRADIENTCOLOR2 = 3811,
			TMT_GRADIENTCOLOR3 = 3812,
			TMT_GRADIENTCOLOR4 = 3813,
			TMT_GRADIENTCOLOR5 = 3814,
			TMT_SHADOWCOLOR = 3815,
			TMT_GLOWCOLOR = 3816,
			TMT_TEXTBORDERCOLOR = 3817,
			TMT_TEXTSHADOWCOLOR = 3818,
			TMT_GLYPHTEXTCOLOR = 3819,
			TMT_GLYPHTRANSPARENTCOLOR = 3820,
			TMT_FILLCOLORHINT = 3821,
			TMT_BORDERCOLORHINT = 3822,
			TMT_ACCENTCOLORHINT = 3823,
			LVP_LISTITEM = 1,
			LVP_LISTGROUP = 2,
			LVP_LISTDETAIL = 3,
			LVP_LISTSORTEDDETAIL = 4,
			LVP_EMPTYTEXT = 5,
			LIS_NORMAL = 1,
			LIS_HOT = 2,
			LIS_SELECTED = 3,
			LIS_DISABLED = 4,
			LIS_SELECTEDNOTFOCUS = 5,
			HP_HEADERITEM = 1,
			HP_HEADERITEMLEFT = 2,
			HP_HEADERITEMRIGHT = 3,
			HP_HEADERSORTARROW = 4,
			HIS_NORMAL = 1,
			HIS_HOT = 2,
			HIS_PRESSED = 3,
			HILS_NORMAL = 1,
			HILS_HOT = 2,
			HILS_PRESSED = 3,
			HIRS_NORMAL = 1,
			HIRS_HOT = 2,
			HIRS_PRESSED = 3,
			HSAS_SORTEDUP = 1,
			HSAS_SORTEDDOWN = 2,
			TKP_TRACK = 1,
			TKP_TRACKVERT = 2,
			TKP_THUMB = 3,
			TKP_THUMBBOTTOM = 4,
			TKP_THUMBTOP = 5,
			TKP_THUMBVERT = 6,
			TKP_THUMBLEFT = 7,
			TKP_THUMBRIGHT = 8,
			TKP_TICS = 9,
			TKP_TICSVERT = 10,
			PP_BAR = 1,
			PP_TRANSPARENTBAR = 11,
			PP_TRANSPARENTBARVERT = 12,
			PP_BARVERT = 2,
			PP_CHUNK = 3,
			PP_CHUNKVERT = 4,
			PP_FILL = 5,
			PP_FILLVERT = 6,
			TTP_STANDARD = 1,
			TTP_BALLOON = 3,
			TTSS_NORMAL = 1,
			TTBS_NORMAL = 1,
			TABP_TABITEM = 1,
			TABP_TABITEMLEFTEDGE = 2,
			TABP_TABITEMRIGHTEDGE = 3,
			TABP_TABITEMBOTHEDGE = 4,
			TABP_TOPTABITEM = 5,
			TABP_TOPTABITEMLEFTEDGE = 6,
			TABP_TOPTABITEMRIGHTEDGE = 7,
			TABP_TOPTABITEMBOTHEDGE = 8,
			TABP_PANE = 9,
			TABP_BODY = 10,
			TIS_NORMAL = 1,
			TIS_HOT = 2,
			TIS_SELECTED = 3,
			TIS_DISABLED = 4,
			TIS_FOCUSED = 5,
			TILES_NORMAL = 1,
			TILES_HOT = 2,
			TILES_SELECTED = 3,
			TILES_DISABLED = 4,
			TILES_FOCUSED = 5,
			TIRES_NORMAL = 1,
			TIRES_HOT = 2,
			TIRES_SELECTED = 3,
			TIRES_DISABLED = 4,
			TIRES_FOCUSED = 5,
			TIBES_NORMAL = 1,
			TIBES_HOT = 2,
			TIBES_SELECTED = 3,
			TIBES_DISABLED = 4,
			TIBES_FOCUSED = 5,
			TTIS_NORMAL = 1,
			TTIS_HOT = 2,
			TTIS_SELECTED = 3,
			TTIS_DISABLED = 4,
			TTIS_FOCUSED = 5,
			TTILES_NORMAL = 1,
			TTILES_HOT = 2,
			TTILES_SELECTED = 3,
			TTILES_DISABLED = 4,
			TTILES_FOCUSED = 5,
			TTIRES_NORMAL = 1,
			TTIRES_HOT = 2,
			TTIRES_SELECTED = 3,
			TTIRES_DISABLED = 4,
			TTIRES_FOCUSED = 5,
			TTIBES_NORMAL = 1,
			TTIBES_HOT = 2,
			TTIBES_SELECTED = 3,
			TTIBES_DISABLED = 4,
			TTIBES_FOCUSED = 5,
			RP_GRIPPER = 1,
			RP_GRIPPERVERT = 2,
			RP_BAND = 3,
			RP_CHEVRON = 4,
			RP_CHEVRONVERT = 5,
			CHEVS_NORMAL = 1,
			CHEVS_HOT = 2,
			CHEVS_PRESSED = 3,
			SP_PANE = 1,
			SP_GRIPPERPANE = 2,
			SP_GRIPPER = 3,
			WP_CAPTION = 1,
			WP_SMALLCAPTION = 2,
			WP_MINCAPTION = 3,
			WP_SMALLMINCAPTION = 4,
			WP_MAXCAPTION = 5,
			WP_SMALLMAXCAPTION = 6,
			WP_FRAMELEFT = 7,
			WP_FRAMERIGHT = 8,
			WP_FRAMEBOTTOM = 9,
			WP_SMALLFRAMELEFT = 10,
			WP_SMALLFRAMERIGHT = 11,
			WP_SMALLFRAMEBOTTOM = 12,
			WP_SYSBUTTON = 13,
			WP_MDISYSBUTTON = 14,
			WP_MINBUTTON = 15,
			WP_MDIMINBUTTON = 16,
			WP_MAXBUTTON = 17,
			WP_CLOSEBUTTON = 18,
			WP_SMALLCLOSEBUTTON = 19,
			WP_MDICLOSEBUTTON = 20,
			WP_RESTOREBUTTON = 21,
			WP_MDIRESTOREBUTTON = 22,
			WP_HELPBUTTON = 23,
			WP_MDIHELPBUTTON = 24,
			WP_HORZSCROLL = 25,
			WP_HORZTHUMB = 26,
			WP_VERTSCROLL = 27,
			WP_VERTTHUMB = 28,
			WP_DIALOG = 29,
			WP_CAPTIONSIZINGTEMPLATE = 30,
			WP_SMALLCAPTIONSIZINGTEMPLATE = 31,
			WP_FRAMELEFTSIZINGTEMPLATE = 32,
			WP_SMALLFRAMELEFTSIZINGTEMPLATE = 33,
			WP_FRAMERIGHTSIZINGTEMPLATE = 34,
			WP_SMALLFRAMERIGHTSIZINGTEMPLATE = 35,
			WP_FRAMEBOTTOMSIZINGTEMPLATE = 36,
			WP_SMALLFRAMEBOTTOMSIZINGTEMPLATE = 37,
			FS_ACTIVE = 1,
			FS_INACTIVE = 2,
			CS_ACTIVE = 1,
			CS_INACTIVE = 2,
			CS_DISABLED = 3,
			MXCS_ACTIVE = 1,
			MXCS_INACTIVE = 2,
			MXCS_DISABLED = 3,
			MNCS_ACTIVE = 1,
			MNCS_INACTIVE = 2,
			MNCS_DISABLED = 3,
			HSS_NORMAL = 1,
			HSS_HOT = 2,
			HSS_PUSHED = 3,
			HSS_DISABLED = 4,
			HTS_NORMAL = 1,
			HTS_HOT = 2,
			HTS_PUSHED = 3,
			HTS_DISABLED = 4,
			VSS_NORMAL = 1,
			VSS_HOT = 2,
			VSS_PUSHED = 3,
			VSS_DISABLED = 4,
			VTS_NORMAL = 1,
			VTS_HOT = 2,
			VTS_PUSHED = 3,
			VTS_DISABLED = 4,
			SBS_NORMAL = 1,
			SBS_HOT = 2,
			SBS_PUSHED = 3,
			SBS_DISABLED = 4,
			MINBS_NORMAL = 1,
			MINBS_HOT = 2,
			MINBS_PUSHED = 3,
			MINBS_DISABLED = 4,
			MAXBS_NORMAL = 1,
			MAXBS_HOT = 2,
			MAXBS_PUSHED = 3,
			MAXBS_DISABLED = 4,
			RBS_NORMAL = 1,
			RBS_HOT = 2,
			RBS_PUSHED = 3,
			RBS_DISABLED = 4,
			HBS_NORMAL = 1,
			HBS_HOT = 2,
			HBS_PUSHED = 3,
			HBS_DISABLED = 4,
			CBS_NORMAL = 1,
			CBS_HOT = 2,
			CBS_PUSHED = 3,
			CBS_DISABLED = 4,
			TVP_TREEITEM = 1,
			TVP_GLYPH = 2,
			TVP_BRANCH = 3,
			TREIS_NORMAL = 1,
			TREIS_HOT = 2,
			TREIS_SELECTED = 3,
			TREIS_DISABLED = 4,
			TREIS_SELECTEDNOTFOCUS = 5,
			GLPS_CLOSED = 1,
			GLPS_OPENED = 2,
			EP_EDITBORDER_NOSCROLL = 6,
			EP_BACKGROUNDWITHBORDER = 5,
			EPSH_HOT = 2,
			EPSH_FOCUSED = 3;
	}
}
