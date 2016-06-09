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
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using System.Collections;
using DevExpress.XtraPrinting;
using System.Drawing.Imaging;
using System.Reflection;
namespace DevExpress.XtraReports.UI {
	public enum WinControlDrawMethod_Utils {
		UseWMPaint = 0,
		UseWMPrint = 1,
		UseWMPaintRecursive = 2,
		UseWMPrintRecursive = 3
	}
	public enum WinControlImageType_Utils {
		Metafile = 0,
		Bitmap = 1
	}
}
namespace DevExpress.XtraReports.Native {
	public static class XRControlPaint {
		#region ControlPaint
		const int DCX_WINDOW = 1;
		const int DCX_LOCKWINDOWUPDATE = 0x400;
		const int DCX_CACHE = 2;
		static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern IntPtr GetDCEx(HandleRef hWnd, HandleRef hrgnClip, int flags);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern IntPtr CreatePen(int nStyle, int nWidth, int crColor);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern int SetROP2(HandleRef hDC, int nDrawMode);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern IntPtr SelectObject(HandleRef hDC, HandleRef hObject);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern int SetBkColor(HandleRef hDC, int clr);
		[DllImport("gdi32.dll", EntryPoint = "Rectangle", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern bool DrawRectangle(HandleRef hdc, int left, int top, int right, int bottom);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern bool DeleteObject(HandleRef hObject);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern IntPtr GetStockObject(int nIndex);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern int ReleaseDC(HandleRef hWnd, HandleRef hDC);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern bool PatBlt(HandleRef hdc, int left, int top, int width, int height, int rop);
		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		static extern IntPtr CreateSolidBrush(int crColor);
		public static void DrawReversibleFrame(Rectangle rect, Color color, FrameStyle style, Control control) {
			rect = control.RectangleToClient(rect);
			int nDrawMode;
			Color c;
			if(color.GetBrightness() < 0.5) {
				nDrawMode = 10;
				c = Color.White;
			} else {
				nDrawMode = 7;
				c = Color.Black;
			}
			IntPtr windowHandle = GetDCEx(new HandleRef(null, control.Handle), NullHandleRef, DCX_WINDOW | DCX_CACHE | DCX_LOCKWINDOWUPDATE);
			IntPtr penHandle;
			switch(style) {
				case FrameStyle.Dashed:
					penHandle = CreatePen(2, 1, ColorTranslator.ToWin32(color));
					break;
				default:
					penHandle = CreatePen(0, 2, ColorTranslator.ToWin32(color));
					break;
			}
			int rop = SetROP2(new HandleRef(null, windowHandle), nDrawMode);
			IntPtr stockPtr = SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, GetStockObject(5)));
			IntPtr pentPtr = SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, penHandle));
			SetBkColor(new HandleRef(null, windowHandle), ColorTranslator.ToWin32(c));
			DrawRectangle(new HandleRef(null, windowHandle), rect.X, rect.Y, rect.Right, rect.Bottom);
			SetROP2(new HandleRef(null, windowHandle), rop);
			SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, stockPtr));
			SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, pentPtr));
			if(penHandle != IntPtr.Zero) {
				DeleteObject(new HandleRef(null, penHandle));
			}
			ReleaseDC(NullHandleRef, new HandleRef(null, windowHandle));
		}
		public static void FillReversibleRectangle(Rectangle rectangle, Color backColor, Control control) {
			rectangle = control.RectangleToClient(rectangle);
			int rop = GetColorRop(backColor, 0xa50065, 0x5a0049);
			int nDrawMode = GetColorRop(backColor, 6, 6);
			IntPtr windowHandle = GetDCEx(new HandleRef(null, control.Handle), NullHandleRef, 0x403);
			IntPtr brushHandle = CreateSolidBrush(ColorTranslator.ToWin32(backColor));
			int rop2 = SetROP2(new HandleRef(null, windowHandle), nDrawMode);
			IntPtr brushPtr = SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, brushHandle));
			PatBlt(new HandleRef(null, windowHandle), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, rop);
			SetROP2(new HandleRef(null, windowHandle), rop2);
			SelectObject(new HandleRef(null, windowHandle), new HandleRef(null, brushPtr));
			DeleteObject(new HandleRef(null, brushHandle));
			ReleaseDC(NullHandleRef, new HandleRef(null, windowHandle));
		}
		static int GetColorRop(Color color, int darkROP, int lightROP) {
			if(color.GetBrightness() < 0.5) {
				return darkROP;
			}
			return lightROP;
		}
		#endregion
		private static Brush controlBrush = new SolidBrush(Color.FromArgb(unchecked((int)0xffece9d8)));
		private const int msgWM_PRINT = 0x0317;
		private class DashStyle : IDisposable {
			private Brush brush;
			private float dash;
			private float spell;
			public Brush Brush {
				get { return brush; }
			}
			public float Dash {
				get { return dash; }
			}
			public float Spell {
				get { return spell; }
			}
			public DashStyle(Brush brush, float dash, float spell) {
				this.brush = brush;
				this.dash = dash;
				this.spell = spell;
			}
			public void Dispose() {
				if(brush != null) {
					brush.Dispose();
					brush = null;
				}
			}
		}
		public static void DrawGrid(Graphics gr, Rectangle bounds, SizeF pixelsBetweenDots, Color foreColor) {
			using(Brush brush = new SolidBrush(foreColor)) {
				RectangleF clipBounds = RectangleF.Intersect(gr.ClipBounds, bounds);
				PointF startPoint = GetFirstDrawnPoint(clipBounds, bounds, pixelsBetweenDots);
				for(float x = startPoint.X; x < clipBounds.Right; x += pixelsBetweenDots.Width) {
					for(float y = startPoint.Y; y < clipBounds.Bottom; y += pixelsBetweenDots.Height) {
						gr.FillRectangle(brush, (int)Math.Round((double)x), (int)Math.Round((double)y), 1, 1);
					}
				}
			}
		}
		static PointF GetFirstDrawnPoint(RectangleF clipBounds, Rectangle controlBounds, SizeF pixelsBetweenDots) {
			float firstDrawnRowIndex = (float)Math.Ceiling(Math.Max(clipBounds.Y - controlBounds.Top, 0) / pixelsBetweenDots.Height);
			float firstDrawnColumnIndex = (float)Math.Ceiling(Math.Max(clipBounds.X - controlBounds.Left, 0) / pixelsBetweenDots.Width);
			return new PointF(controlBounds.Left + pixelsBetweenDots.Width * firstDrawnColumnIndex, controlBounds.Top + pixelsBetweenDots.Height * firstDrawnRowIndex);
		}
		static void DrawHorizDashLine(Graphics gr, DashStyle style, float x, float y, float width, float height) {
			RectangleF baseRect = new RectangleF(x, y, style.Dash, height);
			float right = x + width;
			while(baseRect.Left < right) {
				if(baseRect.Right > right) {
					baseRect.Width = right - baseRect.Left;
				}
				gr.FillRectangle(style.Brush, baseRect);
				baseRect.X = baseRect.Right + style.Spell;
			}
		}
		private static void DrawVertDashLine(Graphics gr, DashStyle style, float x, float y, float width, float height) {
			RectangleF baseRect = new RectangleF(x, y, width, style.Dash);
			float bottom = y + height;
			while(baseRect.Top < bottom) {
				if(baseRect.Bottom > bottom) {
					baseRect.Height = bottom - baseRect.Top;
				}
				gr.FillRectangle(style.Brush, baseRect);
				baseRect.Y = baseRect.Bottom + style.Spell;
			}
		}
		public static void DrawBorder(Graphics gr, RectangleF rect, Brush brush, float borderWidth, DevExpress.XtraPrinting.BorderSide sides) {
			if(borderWidth == 0 || sides == DevExpress.XtraPrinting.BorderSide.None)
				return;
			float bWidth = Math.Min(borderWidth, rect.Width);
			RectangleF r = new RectangleF(rect.Location, new SizeF(bWidth, rect.Height));
			if((sides & DevExpress.XtraPrinting.BorderSide.Left) > 0)
				gr.FillRectangle(brush, r);
			if((sides & DevExpress.XtraPrinting.BorderSide.Right) > 0) {
				r.Offset(rect.Width - bWidth, 0);
				gr.FillRectangle(brush, r);
			}
			bWidth = Math.Min(borderWidth, rect.Height);
			r = new RectangleF(rect.Location, new SizeF(rect.Width, bWidth));
			if((sides & DevExpress.XtraPrinting.BorderSide.Top) > 0)
				gr.FillRectangle(brush, r);
			if((sides & DevExpress.XtraPrinting.BorderSide.Bottom) > 0) {
				r.Offset(0, rect.Height - bWidth);
				gr.FillRectangle(brush, r);
			}
		}
		public static Image GetControlImage(Control ctl, WinControlDrawMethod_Utils drawMethod, WinControlImageType_Utils imageType) {
			if(ctl == null || ctl.Size.Width <= 0 || ctl.Size.Height <= 0)
				return null;
			Image image = null;
			image = imageType == WinControlImageType_Utils.Metafile ?
				(Image)MetafileCreator.CreateInstance(new Rectangle(Point.Empty, ctl.Size), MetafileFrameUnit.Pixel) :
				(Image)new Bitmap(ctl.Size.Width, ctl.Size.Height);
			using(Graphics gr = Graphics.FromImage(image))
				DrawWinControl(ctl, gr, drawMethod);
			return image;
		}
		[FlagsAttribute]
		enum msgWM_PRINTOptions {
			PRF_CHECKVISIBLE = 0x00000001,
			PRF_NONCLIENT = 0x00000002,
			PRF_CLIENT = 0x00000004,
			PRF_ERASEBKGND = 0x00000008,
			PRF_CHILDREN = 0x00000010,
			PRF_OWNED = 0x00000020
		};
		static bool EnableControlStyle(Control control, bool enable, int style) {
			bool wasEnabled = (bool)InvokeMethod(control, "GetStyle", new object[] { style });
			if(wasEnabled != enable)
				InvokeMethod(control, "SetStyle", new object[] { style, enable });
			return wasEnabled;
		}
		static bool EnableDoubleBuffer(Control control, bool enable) {
			return EnableControlStyle(control, enable, (int)ControlConstants.DoubleBuffer);
		}
		static bool EnableAllPaintingInWmPaint(Control control, bool enable) {
			return EnableControlStyle(control, enable, 8192);
		}
		private static void DrawWinControlWMPaintRecursive(Control control, IntPtr hdc, Rectangle rect) {
			bool doubleBufferEnabled = EnableDoubleBuffer(control, false);
			bool allPaintingInWmPaint = EnableAllPaintingInWmPaint(control, false);
			try {
				Win32.SIZE size = new Win32.SIZE(0, 0);
				Win32.SetViewportOrgEx(hdc, rect.X, rect.Y, size);
				Message msg = Message.Create(control.Handle, 20, hdc, IntPtr.Zero);
				InvokeMethod(control, "WndProc", new object[] { msg });
				msg = Message.Create(control.Handle, 0xF, hdc, IntPtr.Zero);
				InvokeMethod(control, "WndProc", new object[] { msg });
			} catch { }
			foreach(Control child in GetChildControlsWithCorrectZOrder(control)) {
				if(control.Visible && child.Visible || !control.Visible) {
					Rectangle childRect = child.Bounds;
					childRect.Offset(rect.Location);
					DrawWinControlWMPaintRecursive(child, hdc, childRect);
				}
			}
			EnableAllPaintingInWmPaint(control, allPaintingInWmPaint);
			EnableDoubleBuffer(control, doubleBufferEnabled);
		}
		private static void DrawWinControlWMPaint(Control control, IntPtr hdc) {
			bool doubleBufferEnabled = EnableDoubleBuffer(control, false);
			try {
				Message msg = Message.Create(control.Handle, 0xF, hdc, IntPtr.Zero);
				InvokeMethod(control, "WndProc", new object[] { msg });
			} catch { }
			EnableDoubleBuffer(control, doubleBufferEnabled);
		}
		private static void DrawWinControlWMPrintRecursive(Control control, IntPtr hdc, Rectangle rect) {
			bool doubleBufferEnabled = EnableDoubleBuffer(control, false);
			Win32.SIZE size = new Win32.SIZE(0, 0);
			Win32.SetViewportOrgEx(hdc, rect.X, rect.Y, size);
			msgWM_PRINTOptions drawOptions =
				msgWM_PRINTOptions.PRF_CLIENT |
				msgWM_PRINTOptions.PRF_ERASEBKGND |
				msgWM_PRINTOptions.PRF_NONCLIENT;
			Win32.SendMessage(control.Handle, msgWM_PRINT, (int)hdc, (IntPtr)drawOptions);
			foreach(Control child in GetChildControlsWithCorrectZOrder(control)) {
				if(control.Visible && child.Visible || !control.Visible) {
					Rectangle childRect = child.Bounds;
					childRect.Offset(rect.Location);
					DrawWinControlWMPrintRecursive(child, hdc, childRect);
				}
			}
			EnableDoubleBuffer(control, doubleBufferEnabled);
		}
		static IList GetChildControlsWithCorrectZOrder(Control control) {
			int count = control.Controls.Count;
			List<Control> controls = new List<Control>(count);
			for (int i = count - 1; i >= 0; i--)
				controls.Add(control.Controls[i]);
			return controls;
		}
		private static void DrawWinControlWMPrint(Control control, IntPtr hdc) {
			bool doubleBufferEnabled = EnableDoubleBuffer(control, false);
			msgWM_PRINTOptions drawOptions =
				msgWM_PRINTOptions.PRF_CHILDREN |
				msgWM_PRINTOptions.PRF_CLIENT |
				msgWM_PRINTOptions.PRF_ERASEBKGND |
				msgWM_PRINTOptions.PRF_NONCLIENT |
				msgWM_PRINTOptions.PRF_OWNED;
			Win32.SendMessage(control.Handle, msgWM_PRINT, (int)hdc, (IntPtr)drawOptions);
			EnableDoubleBuffer(control, doubleBufferEnabled);
		}
		private static void DrawWinControl(Control control, Graphics gr, WinControlDrawMethod_Utils drawMethod) {
			if(control == null || control.IsDisposed)
				return;
			PaintEventArgs pevent = new PaintEventArgs(gr, new Rectangle(0, 0, control.Width, control.Height));
			IntPtr hdc = gr.GetHdc();
			try {
				switch(drawMethod) {
					case WinControlDrawMethod_Utils.UseWMPrintRecursive:
						DrawWinControlWMPrintRecursive(control, hdc, pevent.ClipRectangle);
						break;
					case WinControlDrawMethod_Utils.UseWMPrint:
						DrawWinControlWMPrint(control, hdc);
						break;
					case WinControlDrawMethod_Utils.UseWMPaintRecursive:
						DrawWinControlWMPaintRecursive(control, hdc, pevent.ClipRectangle);
						break;
					case WinControlDrawMethod_Utils.UseWMPaint:
					default:
						DrawWinControlWMPaint(control, hdc);
						break;
				}
			} catch { }
			gr.ReleaseHdc(hdc);
		}
		private static object InvokeMethod(Control control, string method, object[] args) {
			MethodInfo mi = control.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
			if(mi != null)
				return mi.Invoke(control, args);
			return null;
		}
		public static void DrawTriangleDown(Graphics gr, Rectangle rect, bool center) {
			rect = CalcTriangleRect(rect, center);
			rect.Height = 1;
			while(rect.Width > 0) {
				gr.FillRectangle(SystemBrushes.ControlText, rect);
				rect.Y += 1;
				rect.X += 1;
				rect.Width -= 2;
			}
		}
		private static Rectangle CalcTriangleRect(Rectangle baseRect, bool center) {
			Rectangle rect = new Rectangle(Point.Empty, new Size(9, 4));
			rect.Offset(0, (baseRect.Height - rect.Height) / 2);
			if(center)
				rect.X = (baseRect.Right - rect.Width) / 2;
			else
				rect.X = baseRect.Right - rect.Width - 8;
			return rect;
		}
	}
}
