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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils {
	public static class MouseButtonsExtentions {
		static MouseButtons GetRealValue(MouseButtons buttons) {
			return buttons & (MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle);
		}
		public static bool HasLeft(this MouseButtons buttons) {
			return (buttons & MouseButtons.Left) != 0;
		}
		public static bool HasRight(this MouseButtons buttons) {
			return (buttons & MouseButtons.Right) != 0;
		}
		public static bool HasMiddle(this MouseButtons buttons) {
			return (buttons & MouseButtons.Middle) != 0;
		}
		public static bool IsLeft(this MouseButtons buttons) {
			return GetRealValue(buttons) == MouseButtons.Left;
		}
		public static bool IsRight(this MouseButtons buttons) {
			return GetRealValue(buttons) == MouseButtons.Right;
		}
		public static bool IsMiddle(this MouseButtons buttons) {
			return GetRealValue(buttons) == MouseButtons.Middle;
		}
		public static bool IsNone(this MouseButtons buttons) {
			return GetRealValue(buttons) == MouseButtons.None;
		}
	}
	public static class ScaleUtils {
		static SizeF scaleFactor = SizeF.Empty;
		public static SizeF ScaleFactor {
			get {
				if(scaleFactor.Width == 0) scaleFactor = GetScaleFactor();
				return scaleFactor;
			}
			set {
				scaleFactor = value;
			}
		}
		public static SizeF GetScaleFactor() {
			Size sz = GetSystemDPI();
			return new SizeF((float)sz.Width / 96f, (float)sz.Height / 96f);
		}
		public static Size GetSystemDPI() {
			IntPtr screenHDC = NativeMethods.GetDC(IntPtr.Zero);
			try {
				int dx = NativeMethods.GetDeviceCaps(screenHDC, 88);
				int dy = NativeMethods.GetDeviceCaps(screenHDC, 90);
				return new Size(dx, dy);
			}
			finally { NativeMethods.ReleaseDC(IntPtr.Zero, screenHDC); }
		}
		public static Size GetScaleSize(Size size) {
			Size ret = Size.Empty;
			DevExpress.Utils.Drawing.GraphicsInfo.Default.AddGraphics(null);
			IntPtr hdc = DevExpress.Utils.Drawing.GraphicsInfo.Default.Graphics.GetHdc();
			try {
				ret = new Size(
					Convert.ToInt32(size.Width * DevExpress.Utils.Text.HdcPixelUtils.GetLogicPixelPerInchX(hdc) / 96.0),
					Convert.ToInt32(size.Height * DevExpress.Utils.Text.HdcPixelUtils.GetLogicPixelPerInchY(hdc) / 96.0));
			}
			finally {
				DevExpress.Utils.Drawing.GraphicsInfo.Default.Graphics.ReleaseHdc(hdc);
				DevExpress.Utils.Drawing.GraphicsInfo.Default.ReleaseGraphics();
			}
			return ret;
		}
		public static bool IsLargeFonts { get { return GetScaleSize(new Size(10, 10)).Width < 12; } }
	}
	public enum EditorShowMode { Default, MouseDown, MouseUp, Click, MouseDownFocused };
	public class ControlUtils {
		static int columnResizeEdgeSize = 4;
		public static int ColumnResizeEdgeSize {
			get { return columnResizeEdgeSize; }
			set { columnResizeEdgeSize = Math.Max(2, value); }
		}
#if DXWhidbey
		public static bool EnableComponentNotifications(bool newValue, IComponent component) {
			if(component == null) return false;
			return EnableComponentNotifications(newValue, component.Site);
		}
		public static bool EnableComponentNotifications(bool newValue, ISite site) {
			if(site == null) return false;
			IDesignerHost host = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return false;
			IDesignerLoaderService srv = host.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
			if(srv == null) return false;
			MethodInfo mi = srv.GetType().GetMethod("EnableComponentNotification", BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null) return (bool)mi.Invoke(srv, new object[] { newValue });
			return false;
		}
#else
		public static bool EnableComponentNotifications(bool newValue, IComponent component) { return false; }
		public static bool EnableComponentNotifications(bool newValue, ISite site) { return false; }
#endif
		public static MouseButtons MouseButtons {
			get {
				MouseButtons ms = MouseButtons.None;
				if(NativeMethods.GetAsyncKeyState(1) != 0) ms |= MouseButtons.Left;
				if(NativeMethods.GetAsyncKeyState(2) != 0) ms |= MouseButtons.Right;
				if(NativeMethods.GetAsyncKeyState(4) != 0) ms |= MouseButtons.Middle;
				return ms;
			}
		}
		public static bool IsKeyPressed(Keys key) {
			return (NativeMethods.GetAsyncKeyState((int)key) & 0x8000) != 0;
		}
		public static Point CalcLocation(Point bottomLocation, Point topLocation, Size popupSize, bool allowXcheck) {
			Point res = CalcLocation(bottomLocation, topLocation, popupSize);
			if(!allowXcheck) res.X = bottomLocation.X;
			return res;
		}
		static bool useVirtualScreenForDropDown = false;
		public static bool UseVirtualScreenForDropDown {
			get { return useVirtualScreenForDropDown; }
			set { useVirtualScreenForDropDown = value; }
		}
		public static Point CalcLocation(Point bottomLocation, Point topLocation, Size popupSize) {
			return CalcLocation(bottomLocation, topLocation, popupSize, Size.Empty, false);
		}
		public static Point CalcLocation(Point bottomLocation, Point topLocation, Size popupSize, Size ownerSize, bool isRightToLeft, Rectangle workingAreaBounds) {
			Rectangle rect = workingAreaBounds;
			Point location = bottomLocation;
			int bottom = bottomLocation.Y + popupSize.Height;
			int top = topLocation.Y - popupSize.Height;
			int maxBottomOutsize = bottom - rect.Bottom;
			int maxTopOutsize = rect.Top - top;
			if(maxBottomOutsize > 0 && maxBottomOutsize > maxTopOutsize) {
				location = topLocation;
				location.Y -= popupSize.Height;
			}
			if(isRightToLeft && ownerSize != Size.Empty) {
				location.X -= (popupSize.Width - ownerSize.Width);
			}
			if(location.X + popupSize.Width > rect.Right) {
				location.X = (rect.Right - popupSize.Width);
			}
			if(location.X < rect.Left) location.X = rect.Left;
			return location;
		}
		public static Point CalcLocation(Point bottomLocation, Point topLocation, Size popupSize, Size ownerSize, bool isRightToLeft) {
			Rectangle rect = UseVirtualScreenForDropDown ? SystemInformation.VirtualScreen : SystemInformation.WorkingArea;
			if(SystemInformation.MonitorCount > 1) {
				rect = SystemInformation.WorkingArea;
				Screen scrBottom = Screen.FromPoint(bottomLocation), scrTop;
				if(bottomLocation == topLocation)
					scrTop = scrBottom;
				else
					scrTop = Screen.FromPoint(topLocation);
				if(scrBottom.Equals(scrTop)) rect = UseVirtualScreenForDropDown ? scrTop.Bounds : scrTop.WorkingArea;
				else {
					rect = UseVirtualScreenForDropDown ? scrBottom.Bounds : scrBottom.WorkingArea;
				}
			}
			return CalcLocation(bottomLocation, topLocation, popupSize, ownerSize, isRightToLeft, rect);
		}
		public static Point CalcFormLocation(Point location, Size formSize) {
			Rectangle rect = Screen.FromPoint(location).WorkingArea;
			if(location.X + formSize.Width > rect.Right) location.X = (rect.Right - formSize.Width);
			if(location.X < rect.Left) location.X = rect.Left;
			if(location.Y + formSize.Height > rect.Bottom) location.Y = (rect.Bottom - formSize.Height);
			if(location.Y < rect.Top) location.Y = rect.Top;
			return location;
		}
		public static Rectangle ConstrainFormBounds(Control owner, Rectangle r) {
			Screen scr = Screen.FromControl(owner);
			if(r.Top < scr.WorkingArea.Top) {
				r.Height += r.Top;
				r.Y = 0;
			}
			else if(r.Bottom > scr.WorkingArea.Bottom) {
				r.Height = scr.WorkingArea.Bottom - r.Top;
			}
			return r;
		}
		public static void SuspendRedraw(Control control) {
			if(control == null || !control.IsHandleCreated) return;
			SetRedraw(control, false);
		}
		public static void ResumeRedraw(Control control) {
			if(control == null || !control.IsHandleCreated) return;
			SetRedraw(control, true);
			NativeMethods.SendMessage(control.Handle, WM_NCPAINT, 0, IntPtr.Zero);
			control.Invalidate(true);
		}
		const int WM_SETREDRAW = 0x000B, WM_NCPAINT = 0x0085;
		static void SetRedraw(Control control, bool value) {
			NativeMethods.SendMessage(control.Handle, WM_SETREDRAW, value ? 1 : 0, IntPtr.Zero);
		}
		static Font GetCaptionFontBackup() {
			int fontHeight = SystemInformation.CaptionHeight;
			if(fontHeight >= 8) {
				fontHeight = fontHeight * 3 / 5;
			}
			Font font = new Font("Arial", fontHeight, FontStyle.Bold, GraphicsUnit.Pixel);
			return font;
		}
		public static Font GetCaptionFont() {
			try {
				NativeMethods.NONCLIENTMETRICS metrics = new NativeMethods.NONCLIENTMETRICS();
				bool getFontResult = NativeMethods.SystemParametersInfo(NativeMethods.SPI_GETNONCLIENTMETRICS, metrics.cbSize, metrics, 0);
				return Font.FromLogFont(metrics.lfCaptionFont);
			}
			catch {
				return GetCaptionFontBackup();
			}
		}
		public static bool IsSymbolFont(Font font) {
			const int SYMBOL_CHARSET = 2;
			return GetFontCharSet(font) == SYMBOL_CHARSET;
		}
		public static byte GetFontCharSet(Font font) {
			NativeMethods.LOGFONT lf = new NativeMethods.LOGFONT();
			IntPtr hfont = font.ToHfont();
			NativeMethods.GetObject(hfont, Marshal.SizeOf(lf), lf);
			NativeMethods.DeleteObject(hfont);
			return lf.lfCharSet;
		}
		public static void EnsureDestroyRegion(Control control) {
			if(control == null) return;
			Region reg = control.Region;
			if(reg != null) {
				control.Region = null;
				reg.Dispose();
			}
		}
	}
	public class ColorUtils {
		public static Color GetRealColor(Color clr) {
			using(Graphics g = DevExpress.Utils.Drawing.GraphicsInfo.CreateTempEmptyGraphics()) {
				Color res = g.GetNearestColor(clr);
				return res;
			}
		}
		public static Color OffsetColor(Color clr, int dR, int dG, int dB) {
			dR = Math.Min(255, clr.R + dR);
			dG = Math.Min(255, clr.G + dG);
			dB = Math.Min(255, clr.B + dB);
			return Color.FromArgb(dR, dG, dB);
		}
		public static Color FlatBarBorderColor {
			get {
				Color res = SystemColors.ControlDark;
				res = Color.FromArgb(GetDarkValue(res.R), GetDarkValue(res.G), GetDarkValue(res.B));
				return GetRealColor(res);
			}
		}
		public static Color FlatBarBackColor {
			get {
				Color res = SystemColors.Control;
				res = Color.FromArgb(GetLightValue(res.R), GetLightValue(res.G), GetLightValue(res.B));
				return GetRealColor(res);
			}
		}
		public static Color FlatBarItemPressedBackColor {
			get {
				return GetRealColor(GetLightColor(14, 44, 40));
			}
		}
		public static Color FlatBarItemHighLightBackColor {
			get {
				return GetRealColor(GetLightColor(-2, 30, 72));
			}
		}
		public static Color FlatBarItemDownedColor {
			get { return GetRealColor(GetLightColor(11, 9, 73)); }
		}
		protected class ColorInfo {
			public int BtnFaceColorPart, HighlightColorPart, WindowColorPart;
		}
		protected static int GetDarkValue(int val) {
			return (val * 8) / 10;
		}
		protected static int GetLightValue(byte val) {
			return val + ((255 - val) * 16) / 100;
		}
		protected static int GetLightIndex(ColorInfo info, byte btnFaceValue, byte highlightValue, byte windowValue) {
			int res = (btnFaceValue * info.BtnFaceColorPart) / 100 +
				(highlightValue * info.HighlightColorPart) / 100 +
				(windowValue * info.WindowColorPart) / 100;
			if(res < 0) res = 0;
			if(res > 255) res = 255;
			return res;
		}
		static ColorInfo colorInfo = new ColorInfo();
		public static Color GetLightColor(int btnFaceColorPart, int highlightColorPart, int windowColorPart) {
			colorInfo.BtnFaceColorPart = btnFaceColorPart;
			colorInfo.HighlightColorPart = highlightColorPart;
			colorInfo.WindowColorPart = windowColorPart;
			Color btnFace = SystemColors.Control,
				highlight = SystemColors.Highlight,
				window = SystemColors.Window;
			Color res;
			if(btnFace == Color.White || btnFace == Color.Black)
				res = highlight;
			else {
				res = Color.FromArgb(
					GetLightIndex(colorInfo, btnFace.R, highlight.R, window.R), 
					GetLightIndex(colorInfo, btnFace.G, highlight.G, window.G), 
					GetLightIndex(colorInfo, btnFace.B, highlight.B, window.B)); 
			}
			return res;
		}
		public static Color FlatTabBackColor {
			get {
				Color clr = SystemColors.Control;
				int r = clr.R, g = clr.G, b = clr.B;
				int max = Math.Max(Math.Max(r, g), b);
				int delta = 0x23;
				int maxDelta = (255 - (max + delta));
				if(maxDelta > 0) maxDelta = 0;
				r += (delta + maxDelta);
				g += (delta + maxDelta);
				b += (delta + maxDelta);
				return Color.FromArgb(r, g, b);
			}
		}
	}
	public static class ScreenShotMaker {
		static int count;
		public static void MakeScreenShot(Control control, string str, string path) {
			if(control == null)
				return;
			Form form = control.FindForm();
			if(form == null)
				return;
			using(Bitmap image = new Bitmap(form.Bounds.Width,
											form.Bounds.Height)) {
				using(Graphics g = Graphics.FromImage(image)) {
					g.CopyFromScreen(form.Bounds.X, form.Bounds.Y, 0, 0, image.Size, CopyPixelOperation.SourceCopy);
					g.DrawString(str, SystemFonts.DefaultFont, Brushes.White, new Point(0, 0));
				}
				image.Save(path + "Image" + string.Format("{0:D3}", count) + ".png");
				count++;
			}
		}
		public static void SaveGraphicsSnapshot(Graphics srcGraphics, string str, string path) {
			Image image = MakeGraphicsSnapshot(srcGraphics);
			image.Save(path + "Image" + string.Format("{0:D3}", count) + ".png");
			count++;
		}
		public static void SaveGraphicsSnapshot(Graphics srcGraphics, string fileName) {
			Image img = MakeGraphicsSnapshot(srcGraphics);
			img.Save(fileName);
		}
		public static Bitmap MakeGraphicsSnapshot(Graphics srcGraphics) {
			Bitmap bmp = new Bitmap((int)srcGraphics.VisibleClipBounds.Width, (int)srcGraphics.VisibleClipBounds.Height);
			using(Graphics g = Graphics.FromImage(bmp)) {
				IntPtr hdcSource = srcGraphics.GetHdc();
				IntPtr hdcDest = g.GetHdc();
				const int SRCCOPY = 0xCC0020;
				NativeMethods.BitBlt(hdcDest, (int)srcGraphics.VisibleClipBounds.X, (int)srcGraphics.VisibleClipBounds.Height, bmp.Width, bmp.Height, hdcSource, 0, 0, SRCCOPY);
				srcGraphics.ReleaseHdc(hdcSource);
				g.ReleaseHdc(hdcDest);
			}
			return bmp;
		}
		public static void MakeScreenShot(Control control, Message msg, string path) {
			MakeScreenShot(control, msg.ToString(), path);
		}
		public static void MakeScreenShot(Control control, Message msg) {
			MakeScreenShot(control, msg, "c:\\Snapshots\\");
		}
	}
#if DXWhidbey
	public class DXMouseEventArgs : HandledMouseEventArgs {
		HandledMouseEventArgs original = null;
		internal bool ishMouseWheel = false;
		protected DXMouseEventArgs(HandledMouseEventArgs e)
			: base(e.Button, e.Clicks, e.X, e.Y, e.Delta, e.Handled) {
			this.original = e;
			Sync();
		}
		public DXMouseEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta, bool handled) : base(buttons, clicks, x, y, delta, handled) { }
		public DXMouseEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta) : this(buttons, clicks, x, y, delta, false) { }
		public static DXMouseEventArgs GetMouseArgs(Control control, EventArgs eventArgs) {
			DXMouseEventArgs ee = eventArgs as DXMouseEventArgs;
			if(ee == null) {
				HandledMouseEventArgs he = eventArgs as HandledMouseEventArgs;
				MouseEventArgs e = eventArgs as MouseEventArgs;
				if(e == null) {
					Point loc = Control.MousePosition;
					if(control != null && control.IsHandleCreated) loc = control.PointToClient(loc);
					e = new MouseEventArgs(Control.MouseButtons, 1, loc.X, loc.Y, 0);
				}
				if(he != null)
					ee = new DXMouseEventArgs(he);
				else
					ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			}
			return ee;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsHMouseWheel { get { return ishMouseWheel; } }
		public new bool Handled {
			get {
				Sync();
				return base.Handled;
			}
			set {
				base.Handled = value;
				Sync();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Sync() { if(original != null) original.Handled = base.Handled; }
		public static DXMouseEventArgs GetMouseArgs(MouseEventArgs eventArgs) {
			return GetMouseArgs(null, eventArgs);
		}
	}
#else
	public class DXMouseEventArgs : MouseEventArgs {
		bool _handled;
		public DXMouseEventArgs(MouseButtons buttons, int clicks, int x, int y, int delta) : base(buttons, clicks, x, y, delta) {
			this._handled = false;
		}
		public static DXMouseEventArgs GetMouseArgs(Control control, EventArgs eventArgs) {
			DXMouseEventArgs ee = eventArgs as DXMouseEventArgs;
			if(ee == null) {
				MouseEventArgs e = eventArgs as MouseEventArgs;
				if(e == null) {
					Point loc = Control.MousePosition;
					if(control != null && control.IsHandleCreated) loc = control.PointToClient(loc);
					e = new MouseEventArgs(Control.MouseButtons, 1, loc.X, loc.Y, 0);
				}
				ee = new DXMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);
			}
			return ee;
		}
		public static DXMouseEventArgs GetMouseArgs(MouseEventArgs eventArgs) {
			return GetMouseArgs(null, eventArgs);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Sync() { }
		public virtual bool Handled { get { return _handled; } set { _handled = value; } }
	}
#endif
	[AttributeUsage(System.AttributeTargets.Property)]
	public class ImageListAttribute : Attribute {
		public ImageListAttribute(object imageList) {
			this.imageList = imageList;
		}
		public ImageListAttribute(string name) {
			this.name = name;
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		protected object imageList;
		string name;
	}
	public class Permissions {
		public static void UnmanagedCode() {
		}
		public static void UnmanagedCodeDemand() {
		}
		public static void Request() {
			UnmanagedCode();
		}
	}
	public interface IFocusablePopupForm {
		bool AllowFocus { get; }
	}
	public class OSVersionHelper {
		public static bool Is64BitOS() {
			return Marshal.SizeOf(typeof(IntPtr)) == 8;
		}
	}
	public interface ILockService {
		bool CanChangeComponents(ICollection components);
		bool CanDeleteComponents(ICollection components);
	}
	class RootNS {
	}
	public static class ImageCollectionUtils {
		public static string[] ImageFileExtensions = { ".bmp", ".jpg", ".gif", ".png" };
		public static List<string> GetImageResourceNames(Assembly asm) {
			return GetImageResourceNames(asm, ImageFileExtensions);
		}
		public static List<string> GetImageResourceNames(Assembly asm, string[] extensions) {
			var images = from resource in asm.GetManifestResourceNames() where IsImageResourceId(resource, extensions) select resource;
			return images.ToList();
		}
		public static Image GetImage(Assembly asm, string resource) {
			Stream stream = asm.GetManifestResourceStream(resource);
			if(stream == null) return null;
			Image img = null;
			try { img = Image.FromStream(stream); }
			catch { }
			return img;
		}
		static bool IsImageResourceId(string resource, string[] extensions) {
			for(int i = 0; i < extensions.Length; i++) {
				if(resource.EndsWith(extensions[i])) return true;
			}
			return false;
		}
		public static void DoMerge(InnerImagesList images, IList<ImageInfo> newCol, Type itemType, bool preserveSelected) {
			IEnumerable<ImageInfo> items = images.FindAll(img => img.GetType() == itemType);
			IEnumerable<ImageInfo> newItems = GetChanges(newCol, items);
			IEnumerable<ImageInfo> deletedItems = GetChanges(items, newCol);
			if(newItems.Count() > 0) images.AddRange(newItems);
			if(!preserveSelected) {
				foreach(ImageInfo obj in deletedItems) images.Remove(obj);
			}
		}
		public static IEnumerable<ImageInfo> GetChanges(IEnumerable<ImageInfo> col1, IEnumerable<ImageInfo> col2) {
			foreach(ImageInfo obj in col1) {
				if(!col2.Contains(obj)) yield return obj;
			}
		}
		public static IList<ImageInfo> FilterByItemType(IList<ImageInfo> col, Type itemType) {
			if(col.Count == 0) return col;
			List<ImageInfo> list = new List<ImageInfo>();
			foreach(ImageInfo obj in col) {
				if(obj.GetType() == itemType) list.Add(obj);
			}
			return list;
		}
	}
	public class StringSplitInfo {
		int startPos, endPos;
		public StringSplitInfo(string text, bool isSplitter, int startPos, int endPos) {
			this.Text = text;
			this.IsSplitter = isSplitter;
			this.startPos = startPos;
			this.endPos = endPos;
		}
		protected internal bool Check(bool isSplitter, string text, int startPos, int endPos) {
			if(!string.Equals(Text, text, StringComparison.Ordinal)) return false;
			return IsSplitter == isSplitter && StartPos == startPos && EndPos == endPos;
		}
		public override string ToString() {
			return string.Format("Text = {0} IsSplitter = {1}, Range = {2} - {3}", Text, IsSplitter.ToString(), StartPos.ToString(), EndPos.ToString());
		}
		public string Text { get; private set; }
		public bool IsSplitter { get; private set; }
		public int StartPos { get { return startPos; } }
		public int EndPos { get { return endPos; } }
	}
	public class StringSplitHelper {
		public static List<StringSplitInfo> Split(string text, StringCollection separators) {
			return Split(text, separators, null, false);
		}
		public static List<StringSplitInfo> Split(string text, IEnumerable<string> separators) {
			return Split(text, separators, null, false);
		}
		public static List<string> SplitSimple(string text, char separator) {
			StringCollection col = new StringCollection();
			col.Add(separator.ToString());
			return SplitSimple(text, col);
		}
		public static List<string> SplitSimple(string text, StringCollection separators) {
			List<string> list = new List<string>();
			List<StringSplitInfo> splitInfo = Split(text, separators);
			for(int i = 0; i < splitInfo.Count; i++) {
				StringSplitInfo item = splitInfo[i];
				if(item.IsSplitter) continue;
				string itemText;
				if(i > 0 && splitInfo[i - 1].IsSplitter) {
					itemText = item.Text.TrimStart();
				}
				else {
					itemText = item.Text;
				}
				list.Add(itemText);
			}
			return list;
		}
		public static List<StringSplitInfo> Split(string text, StringCollection separators, IEnumerable<string> keywords, bool filterSeparators) {
			List<string> arg = new List<string>(separators.Count);
			foreach(string sep in separators) {
				arg.Add(sep);
			}
			return Split(text, arg, keywords, filterSeparators);
		}
		public static List<StringSplitInfo> Split(string text, IEnumerable<string> separators, IEnumerable<string> keywords) {
			return Split(text, separators, keywords, false);
		}
		public static List<StringSplitInfo> Split(string text, IEnumerable<string> separators, IEnumerable<string> keywords, bool filterSeparators) {
			if(text == null || separators == null) return null;
			List<StringSplitInfo> res = new List<StringSplitInfo>();
			SplitCore(text, separators, res, keywords, filterSeparators);
			return res;
		}
		static void SplitCore(string text, IEnumerable<string> separators, List<StringSplitInfo> res, IEnumerable<string> keywords, bool filterSeparators) {
			List<string> sepsCore = GetSeps(separators);
			int i, lastItemPos = 0, newPos;
			StringBuilder buf = new StringBuilder();
			for(i = 0; i < text.Length; i++) {
				if(IsMayBeSeparator(text[i], sepsCore)) {
					if(CheckWholeSep(text, i, sepsCore, out newPos)) {
						if(buf.Length > 0) {
							int tempPos = 0;
							bool isSep = CheckWholeSep(text, lastItemPos, separators, out tempPos);
							AddSplitInfo(res, buf.ToString(), isSep, lastItemPos, i, filterSeparators);
						}
						AddSplitInfo(res, text.Substring(i, newPos - i + 1), true, i, newPos + 1, filterSeparators);
						i = newPos;
						buf.Clear();
						lastItemPos = newPos + 1;
						continue;
					}
				}
				if(IsKeyword(buf, keywords)) {
					AddSplitInfo(res, buf.ToString(), false, lastItemPos, i, filterSeparators);
					buf.Clear();
					lastItemPos = i;
				}
				if(CheckWholeKeyword(text, i, keywords, out newPos)) {
					if(buf.Length > 0) {
						AddSplitInfo(res, buf.ToString(), false, lastItemPos, i, filterSeparators);
					}
					AddSplitInfo(res, text.Substring(i, newPos - i + 1), false, i, newPos + 1, filterSeparators);
					buf.Clear();
					i = newPos;
					lastItemPos = newPos + 1;
					continue;
				}
				if(i < text.Length) buf.Append(text[i]);
			}
			if(buf.Length > 0) {
				bool isSep = CheckWholeSep(text, i - 1, separators, out newPos);
				AddSplitInfo(res, buf.ToString(), isSep, lastItemPos, lastItemPos + buf.Length, filterSeparators);
			}
		}
		static void AddSplitInfo(List<StringSplitInfo> res, string text, bool isSep, int startPos, int endPos, bool filterSeps) {
			if(isSep && filterSeps) return;
			res.Add(new StringSplitInfo(text, isSep, startPos, endPos));
		}
		static bool IsKeyword(StringBuilder builder, IEnumerable<string> keywords) {
			if(keywords == null) return false;
			return keywords.Contains(builder.ToString(), StringComparer.Ordinal);
		}
		static List<string> GetSeps(IEnumerable<string> separators) {
			List<string> res = new List<string>(separators);
			res.Add(Environment.NewLine);
			return res;
		}
		static bool IsMayBeSeparator(char ch, IEnumerable<string> separators) {
			foreach(string sep in separators) {
				if(sep.Length == 0) continue;
				if(sep[0] == ch) return true;
			}
			return false;
		}
		static bool CheckWholeSep(string text, int pos, IEnumerable<string> separators, out int newPos) {
			return CheckFollowToken(text, pos, separators, out newPos);
		}
		static bool CheckWholeKeyword(string text, int pos, IEnumerable<string> keywords, out int newPos) {
			newPos = -1;
			return CheckFollowToken(text, pos, keywords, out newPos);
		}
		static bool CheckFollowToken(string text, int pos, IEnumerable<string> keys, out int newPos) {
			newPos = -1;
			if(keys == null || keys.Count() == 0) return false;
			int maxLenght = keys.Max(sep => sep.Length);
			StringBuilder buf = new StringBuilder();
			for(int i = pos; i < text.Length; i++) {
				buf.Append(text[i]);
				if(buf.Length > maxLenght) break;
				if(keys.Contains(buf.ToString(), StringComparer.Ordinal)) {
					newPos = i;
					return true;
				}
			}
			return false;
		}
	}
}
namespace DevExpress.Utils.Registrator {
	public delegate void CheckTypeHandler(Type type);
	public class RegistratorHelper {
		public RegistratorHelper(CheckTypeHandler handler, Type baseType, IDesignerHost designerHost) {
			if(handler == null) return;
			RegisterUserItems(handler, baseType, designerHost);
		}
		static string[] systemNames = new string[] {"mscorlib", "system", "microsoft", "presentationframework", "presentationcore", "uiatomationtypes", "extensibility",
												"envdte", "office", "vslangproj",
										AssemblyInfo.SRAssemblyUtils.ToLower(), AssemblyInfo.SRAssemblyGrid.ToLower(),
										AssemblyInfo.SRAssemblyEditors.ToLower(), AssemblyInfo.SRAssemblyData.ToLower(), AssemblyInfo.SRAssemblyNavBar.ToLower()};
		protected virtual void RegisterUserItems(CheckTypeHandler handler, Type baseType, IDesignerHost designerHost) {
			try {
				try {
					if(designerHost != null) designerHost.GetType(designerHost.RootComponentClassName);
				}
				catch {
				}
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				if(assemblies == null || assemblies.Length == 0) return;
				foreach(Assembly asm in assemblies) {
					try {
						if(systemNames.FirstOrDefault(q => asm.GetName().Name.ToLowerInvariant().StartsWith(q)) != null) continue;
					Type[] types = asm.GetTypes();
					for(int n = 0; n < types.Length; n++) {
						Type type = types[n];
						if(baseType == null || type.IsSubclassOf(baseType)) {
							handler(type);
						}
					}
				}
					catch {
					}
				}
			}
			catch {
			}
		}
	}
}
namespace DevExpress.XtraBars.Docking {
	public class ProhibitUsingAsDockingContainerAttribute : Attribute {
		public static bool IsDefined(ContainerControl container) {
			if(container == null) return false;
			var attributes = TypeDescriptor.GetAttributes(container);
			return (attributes != null) && attributes[typeof(ProhibitUsingAsDockingContainerAttribute)] != null;
		}
	}
}
