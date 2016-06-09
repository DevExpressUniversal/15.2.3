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

namespace DevExpress.Utils.WXPaint {
	using System;
	using System.Runtime.InteropServices;
	using System.Drawing;
	using System.Collections;
	using System.Windows.Forms;
	using DevExpress.XtraEditors.Controls;
	using DevExpress.Utils.Paint;
	using DevExpress.Utils.Win;
	using DevExpress.Utils.Drawing.Helpers;
	using System.Security;
	[SuppressUnmanagedCodeSecurity()]
	public class Painter {
		const int btnTextColor = 1618;
		static Hashtable themes = new Hashtable();
		static Hashtable brushes = new Hashtable();
		static Hashtable bounds = new Hashtable();
		static Hashtable sizes = new Hashtable();
		static int themesEnabled = -1;
		static Painter() {
			CheckThemesEnabled();
		}
		public Painter() {
		}
		[System.Security.SecuritySafeCritical]
		protected static void CheckThemesEnabled() {
			try {
				if(System.Environment.OSVersion.Version.Major < 5 || (System.Environment.OSVersion.Version.Major == 5 && System.Environment.OSVersion.Version.Minor == 0)) {
					themesEnabled = 0;
					return;
				}
				NativeVista.IsThemeActive();
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
		public static bool IsApplicationThemed {
			get { return NativeVista.IsAppThemed(); }
		}
		public static bool ThemesEnabled {
			get { return themesEnabled == 1; }
		}
		public static IntPtr GetTheme(string name) {
			if(themesEnabled != 1) return IntPtr.Zero;
			if(!themes.ContainsKey(name))
				themes[name] = NativeVista.OpenThemeData(IntPtr.Zero, name);
			return (IntPtr)themes[name];
		}
		public static void ThemeChanged() {
			ClearThemes();
			bounds.Clear();
			sizes.Clear();
			CheckThemesEnabled();
			CreateBrushes();
		}
		public static void ClearThemes() {
			themes.Clear();
		}
		public static Brush ButtonText {
			get { return brushes[btnTextColor] as Brush; }
		}
		static void CreateBrushes() {
			lock(brushes) {
				foreach(Brush brush in brushes.Values) {
					brush.Dispose();
				}
				brushes.Clear();
			}
			if(!ThemesEnabled) return;
			NativeControlPaintArgs pArgs = new NativeControlPaintArgs(null, Rectangle.Empty, ButtonPredefines.OK, ButtonStates.None, Brushes.Red, true);
			brushes[btnTextColor] = new SolidBrush(GetThemeColor(pArgs, btnTextColor));
		}
		public static Color GetThemeColor(NativeControlPaintArgs pArgs, int propertyId) { return GetThemeColorSafe(pArgs, propertyId); }
		[SecuritySafeCritical]
		static Color GetThemeColorSafe(NativeControlPaintArgs pArgs, int propertyId) {
			string themeName = "";
			int part = 0, state = 0;
			pArgs.KindDrawn = CalcThemePartState(pArgs, ref themeName, ref part, ref state);
			IntPtr theme = GetTheme(themeName);
			int color = 0;
			NativeVista.GetThemeColor(theme, part, state, propertyId, out color);
			return APIXPaint.GDIColorToColor(color);
		}
		static ButtonPredefines[] pred = new ButtonPredefines[] {
																	ButtonPredefines.Combo, ButtonPredefines.Up, ButtonPredefines.Down, ButtonPredefines.Left,
																	ButtonPredefines.Right, ButtonPredefines.SpinUp, ButtonPredefines.SpinDown,
																	ButtonPredefines.SpinRight, ButtonPredefines.SpinLeft
																};
		static string[] themeNames = new string[] {
													  "combobox", "scrollbar", "scrollbar","scrollbar", "scrollbar",
													  "spin","spin","spin","spin"
												  };
		static int[] themePart = new int[] {
											   1, 1, 1, 1, 1,
											   1, 2, 3, 4
										   };
		static int[] themeStateOff = new int[] {
												   0, 0, 4, 8, 12,
												   0, 0, 0, 0, 0
											   };
		static int CalcCheckState(NativeControlPaintArgs pArgs) {
			int state = buttonStates[(int)pArgs.State];
			NativeCheckControlPaintArgs nc = pArgs as NativeCheckControlPaintArgs;
			if(nc == null) return state;
			if(nc.CheckState == CheckState.Checked) state += 4;
			if(nc.CheckState == CheckState.Indeterminate) state += 8;
			return state;
		}
		static bool CalcThemePartState(NativeControlPaintArgs pArgs, ref string theme, ref int part, ref int state) {
			NativeControlAdvPaintArgs pAdv = pArgs as NativeControlAdvPaintArgs;
			int index = -1;
			if(pAdv != null && pAdv.ThemeName != string.Empty) {
				theme = pAdv.ThemeName;
				part = state = 0;
				index = Array.IndexOf(themeNames, pAdv.ThemeName);
				if(pAdv.Part != -1) part = pAdv.Part;
				if(pAdv.PartState != -1) state = pAdv.PartState;
				else state = buttonStates[(int)pArgs.State];
				if(index == -1) return true;
			}
			if(index == -1) index = Array.IndexOf(pred, pArgs.Kind);
			if(index == -1) {
				theme = "button";
				part = 1;
				state = buttonStates[(int)pArgs.State];
				return false;
			}
			theme = themeNames[index];
			part = themePart[index];
			state = themeStateOff[index] + buttonStates[(int)pArgs.State];
			return true;
		}
		static int[] buttonStates = new int[] {1, 2, 3, 3, 4};
		protected static Region GetThemeBackgroundRegion(IntPtr hdc, NativeControlPaintArgs pArgs, IntPtr theme, int part, int state) {
			Region res = null;
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			IntPtr region = IntPtr.Zero;
			NativeVista.GetThemeBackgroundRegion(theme, IntPtr.Zero, part, state, ref rr, ref region);
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
		protected static void FillThemeBackgroundRegion(IntPtr hdc, NativeControlPaintArgs pArgs, IntPtr theme, int part, int state) {
			SolidBrush brush = pArgs.BackBrush as SolidBrush;
			int color = Paint.APIXPaint.ColorToGDIColor((brush == null ? Color.White : Color.Red ));
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			IntPtr region = IntPtr.Zero;
			NativeVista.GetThemeBackgroundRegion(theme, IntPtr.Zero, part, state, ref rr, ref region);
			if(region == IntPtr.Zero) return;
			IntPtr destRgn = NativeMethods.CreateRectRgn(0,0,0,0),
				reg1 = NativeMethods.CreateRectRgn(rr.Left, rr.Top, rr.Right, rr.Bottom);
			NativeMethods.CombineRgn(destRgn, region, reg1, 3);
			if(destRgn != IntPtr.Zero) {
				IntPtr hBrush = NativeMethods.CreateSolidBrush(color);
				NativeMethods.FillRgn(hdc, destRgn, hBrush);
				NativeMethods.DeleteObject(hBrush);
			}
			NativeMethods.DeleteObject(region);
			NativeMethods.DeleteObject(destRgn);
			NativeMethods.DeleteObject(reg1);
		}
		protected static Rectangle GetThemeContentRect(NativeControlPaintArgs pArgs, IntPtr theme, IntPtr hdc, int part, int state) {
			NativeMethods.RECT source = new NativeMethods.RECT(pArgs.Bounds), res = new NativeMethods.RECT();
			NativeVista.GetThemeBackgroundContentRect(theme, hdc, part, state, ref source, ref res);
			return res.ToRectangle();
		}
		static string GetHash(IntPtr theme, int part, int state, Rectangle bounds) {
			return string.Format("{0},{1},{2},{3}", theme.ToString(), part, state, bounds.ToString());
		}
		protected static Size GetThemeSize(NativeControlPaintArgs pArgs, IntPtr theme, IntPtr hdc, int part, int state) {
			NativeMethods.SIZE size = new NativeMethods.SIZE();
			string hash = GetHash(theme, part, state, Rectangle.Empty);
			object res = sizes[hash];
			if(res != null) return (Size)res;
			NativeVista.GetThemePartSize(theme, hdc, part, state, IntPtr.Zero, 1, ref size);
			Size newSize = size.ToSize();
			sizes[hash] = newSize;
			return newSize;
		}
		protected static Rectangle GetThemeBounds(NativeControlPaintArgs pArgs, IntPtr theme, IntPtr hdc, int part, int state) {
			string hash = GetHash(theme, part, state, pArgs.Bounds);
			object val = bounds[hash];
			if(val != null) return (Rectangle)val;
			NativeMethods.RECT source, res = new NativeMethods.RECT();
			source = new NativeMethods.RECT(pArgs.Bounds);
			if(pArgs.Bounds.Height < 12) source.Bottom += (12 - pArgs.Bounds.Height);
			IntPtr retCode = NativeVista.GetThemeBackgroundExtent(theme, hdc, part, state, ref source, ref res);
			Rectangle r = res.ToRectangle();
			bounds[hash] = r;
			return r;
		}
		public static void DrawCloseButton(NativeControlPaintArgs pArgs) {
			IntPtr theme = GetTheme("window");
			int part = 18;
			int state = buttonStates[(int)pArgs.State];
			pArgs.Bounds = DrawTheme(pArgs, theme, part, state, true);
			return;
		}
		public static void DrawHeader(NativeControlPaintArgs pArgs) {
			IntPtr theme = GetTheme("header");
			int part = 1;
			int state;
			switch(pArgs.State) {
				case ButtonStates.Hottrack: state = 2;
					break;
				case ButtonStates.Push: state = 3;
					break;
				default:
					state = 1;
					break;
			}
			pArgs.Bounds = DrawTheme(pArgs, theme, part, state, true);
			return;
		}
		public static void DrawOpenCloseButton(NativeControlPaintArgs pArgs, bool open) {
			IntPtr theme = GetTheme("treeview");
			int part = 2;
			int state = open ? 2 : 1;
			pArgs.Bounds = DrawTheme(pArgs, theme, part, state, true);
			return;
		}
		protected static Rectangle DrawTheme(NativeControlPaintArgs pArgs, IntPtr theme, int part, int state, bool fillBackground) {
			Rectangle resRect = Rectangle.Empty;
			NativeMethods.RECT rr = new NativeMethods.RECT(pArgs.Bounds);
			Graphics g = pArgs.Graphics;
			if(fillBackground) {
				Region reg = GetThemeBackgroundRegion(IntPtr.Zero, pArgs, theme, part, state);
				if(reg != null) { 
					g.FillRegion(pArgs.BackBrush, reg);
					reg.Dispose();
				}
			}
			IntPtr hdc = g.GetHdc();
			try {
				NativeVista.DrawThemeBackground(theme, hdc, part, state, ref rr, ref rr);
				resRect = GetThemeContentRect(pArgs, theme, hdc, part, state);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
			return resRect;
		}
		protected static Rectangle DrawCore(NativeControlPaintArgs pArgs, bool calcBounds, bool calcSize, bool fillbackGround) {
			try {
				string themeName = "";
				int part = 0, state = 0;
				pArgs.KindDrawn = CalcThemePartState(pArgs, ref themeName, ref part, ref state);
				IntPtr theme = GetTheme(themeName);
				if(calcSize) {
					return new Rectangle(Point.Empty, GetThemeSize(pArgs, theme, IntPtr.Zero, part, state));
				}
				if(calcBounds) 
					return GetThemeBounds(pArgs, theme, IntPtr.Zero, part, state);
				pArgs.Bounds = DrawTheme(pArgs, theme, part, state, fillbackGround);
			}
			catch {
			}
			return Rectangle.Empty;
		}
		public static Rectangle CalcBounds(NativeControlPaintArgs pArgs) {
			return CalcButtonBounds(pArgs);
		}
		public static Rectangle CalcContentRect(NativeControlPaintArgs pArgs) {
			string themeName = "";
			int part = 0, state = 0;
			pArgs.KindDrawn = CalcThemePartState(pArgs, ref themeName, ref part, ref state);
			IntPtr theme = GetTheme(themeName);
			return GetThemeContentRect(pArgs, theme, IntPtr.Zero, part, state);
		}
		public static Rectangle CalcButtonBounds(NativeControlPaintArgs pArgs) {
			return DrawCore(pArgs, true, false, false);
		}
		public static Size CalcButtonMinSize(NativeControlPaintArgs pArgs) {
			return DrawCore(pArgs, true, true, false).Size;
		}
		public enum XP_THEME_SIZE { TS_MIN, TS_TRUE, TS_DRAW }
		public static Size CalcSize(NativeControlPaintArgs pArgs, XP_THEME_SIZE eSize) {
			NativeMethods.SIZE size = new NativeMethods.SIZE();
			string themeName = "";
			int part = 0, state = 0;
			CalcThemePartState(pArgs, ref themeName, ref part, ref state);
			IntPtr theme = GetTheme(themeName);
			NativeVista.GetThemePartSize(theme, IntPtr.Zero, part, state, IntPtr.Zero, (int)eSize, ref size);
			return size.ToSize();
		}
		public static Size CalcBorderSize(NativeControlPaintArgs pArgs) {
			string themeName = "";
			int part = 0, state = 0;
			CalcThemePartState(pArgs, ref themeName, ref part, ref state);
			return GetThemeSize(pArgs, GetTheme(themeName), IntPtr.Zero, 1200 , 1);
		}
		const int BDR_SUNKENOUTER = 0x0002;
		const int BDR_RAISEDINNER = 0x0004;
		const int BF_ADJUST	   = 0x2000;
		const int BF_LEFT		 = 0x0001;
		const int BF_TOP		  = 0x0002;
		const int BF_RIGHT		= 0x0004;
		const int BF_BOTTOM	   = 0x0008;
		public static void DrawBorder(NativeControlPaintArgs pArgs) {
			NativeMethods.RECT rect = new NativeMethods.RECT(pArgs.Bounds), outRect;
			outRect = rect;
			IntPtr hdc = pArgs.Graphics.GetHdc();
			try {
				string themeName = "";
				int part = 0, state = 0;
				int sides = 0;
				if((pArgs.BorderSides & BorderSide.Left) != 0) sides |= BF_LEFT;
				if((pArgs.BorderSides & BorderSide.Right) != 0) sides |= BF_RIGHT;
				if((pArgs.BorderSides & BorderSide.Top) != 0) sides |= BF_TOP;
				if((pArgs.BorderSides & BorderSide.Bottom) != 0) sides |= BF_BOTTOM;
				CalcThemePartState(pArgs, ref themeName, ref part, ref state);
				themeName = "edit";
				part = state = 0;
				NativeVista.DrawThemeEdge(GetTheme(themeName), hdc, part, state, ref rect, BDR_SUNKENOUTER | BDR_RAISEDINNER, sides | BF_ADJUST, ref outRect);
				pArgs.Bounds = outRect.ToRectangle();
			}
			finally {
				pArgs.Graphics.ReleaseHdc(hdc);
			}
		}
		public static void DrawButton(NativeControlPaintArgs pArgs) {
			Draw(pArgs);
		}
		public static void Draw(NativeControlPaintArgs pArgs) {
			NativeControlAdvPaintArgs pAdv = pArgs as NativeControlAdvPaintArgs;
			DrawCore(pArgs, false, false, pAdv == null ? true : pAdv.FillBackground);
		}
		public const int XP_TMT_SIZINGMARGINS = 3601, XP_TMT_CONTENTMARGINS = 3602, XP_TMT_CAPTIONMARGINS = 3603,
			XP_TMT_BORDERCOLOR = 3801, XP_TMT_FILLCOLOR = 3802,
			XP_TMT_TEXTCOLOR = 3803, XP_TMT_EDGELIGHTCOLOR = 3804, XP_TMT_EDGEHIGHLIGHTCOLOR = 3805, XP_TMT_EDGESHADOWCOLOR = 3806;
		public static Rectangle CalcThemeMargins(NativeControlAdvPaintArgs pArgs, int xt_tmt_margin) {
			NativeVista.XPMARGINS margins = new NativeVista.XPMARGINS();
			string themeName = "";
			int part = 0, state = 0;
			CalcThemePartState(pArgs, ref themeName, ref part, ref state);
			IntPtr res = NativeVista.GetThemeMargins(GetTheme(themeName), IntPtr.Zero, part, state, xt_tmt_margin, IntPtr.Zero, ref margins);
			return margins.ToRectangle();
		}
	}
}
