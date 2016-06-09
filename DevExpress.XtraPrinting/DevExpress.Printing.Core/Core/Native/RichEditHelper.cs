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

using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System;
using System.Runtime.InteropServices;
namespace DevExpress.XtraPrinting.Native {
	public static class RichEditHelper {
		public static void DrawRtf(Graphics gr, RichTextBox richTextBox, RectangleF bounds) {
			int charFrom = 0;
			int result = 0;
			int height = 0;
			RectangleF formatFangeBounds = bounds;
			while(charFrom < richTextBox.TextLength) {
				height = FormatRangeInternal(true, gr, richTextBox, formatFangeBounds, charFrom, -1, out result);
				if(charFrom > 0 && height > formatFangeBounds.Height)
					break;
				FormatRangeInternal(false, gr, richTextBox, formatFangeBounds, charFrom, -1, out charFrom);
				formatFangeBounds.Y += height;
				formatFangeBounds.Height -= height;
				if(formatFangeBounds.Y >= bounds.Bottom)
					break;
			}
		}
		public static Image GetRtfImage(RichTextBox richTextBox, float dpi, RectangleF bounds) {
			bounds = GraphicsUnitConverter.Convert(bounds, dpi, DevExpress.XtraPrinting.GraphicsDpi.Pixel);
			if(bounds.Width > 3 && bounds.Height > 3) {
				bounds.X = 0;
				bounds.Y = 0;
				Metafile metaFile = XtraPrinting.Native.MetafileCreator.CreateInstance(bounds, MetafileFrameUnit.Pixel);
				using(Graphics g = Graphics.FromImage(metaFile))
					DrawRtf(g, richTextBox, bounds);
				return metaFile;
			}
			return null;
		}
		public static int MeasureRtfInPixels(string rtfText, RectangleF bounds, int minHeight) {
			RichTextBox richTextBox = new RichTextBox();
			richTextBox.Rtf = rtfText;
			if(richTextBox.TextLength == 0) {
				return minHeight;
			} else {
				bounds.X = 0;
				bounds.Y = 0;
				int height = 0;
				int charFrom = 0, charTo = 0;
				int fullHeight = 0;
				Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
				try {
					do {
						int lastHeight = 0;
						while(charTo < richTextBox.TextLength) {
							height = FormatRangeInternal(true, graphics, richTextBox, bounds, charFrom, -1, out charTo);
							if(height == lastHeight) {
								charFrom = charTo;
								break;
							}
							lastHeight = height;
							bounds.Height = height * 2;
						}
						fullHeight += height;
					} while(charTo < richTextBox.TextLength);
					return fullHeight;
				} finally {
					graphics.Dispose();
					richTextBox.Dispose();
				}
			}
		}
		public static RectangleF CorrectRtfLineBounds(float dpi, RichTextBox richTextBox, RectangleF bounds, int charFrom, out int charEnd) {
			PointF location = bounds.Location;
			bounds.X = 0;
			bounds.Y = 0;
			bounds = GraphicsUnitConverter.Convert(bounds, dpi, DevExpress.XtraPrinting.GraphicsDpi.Pixel);
			Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
			try {
				float height = FormatRangeInternal(true, graphics, richTextBox, bounds, charFrom, -1, out charEnd);
				if(Math.Abs(height - bounds.Height) > GraphicsUnitConverter.Convert(1, DevExpress.XtraPrinting.GraphicsDpi.Pixel, dpi))
					bounds.Height = height;
			} finally {
				graphics.Dispose();
			}
			bounds = GraphicsUnitConverter.Convert(bounds, DevExpress.XtraPrinting.GraphicsDpi.Pixel, dpi);
			bounds.Location = location;
			return bounds;
		}
		[System.Security.SecuritySafeCritical]
		private static int FormatRangeInternal(bool measure, Graphics graphics, RichTextBox richTextBox, RectangleF bounds, int charFrom, int charTo, out int charEnd) {
			int result = 0;
			float DpiY = graphics.DpiY;
			PointF[] points = { bounds.Location, 
				new PointF(bounds.Location.X + bounds.Size.Width, bounds.Location.Y + bounds.Size.Height) };
			graphics.Transform.TransformPoints(points);
			points[0].X = Convert.ToInt32(points[0].X);
			points[0].Y = Convert.ToInt32(points[0].Y);
			points[1].X = Convert.ToInt32(points[1].X);
			points[1].Y = Convert.ToInt32(points[1].Y);
			Rectangle r = new Rectangle((int)points[0].X, (int)points[0].Y,
				(int)points[1].X - (int)points[0].X - 1, (int)points[1].Y - (int)points[0].Y - 1);
			Win32.STRUCT_CHARRANGE cr;
			cr.cpMin = charFrom;
			cr.cpMax = (charTo >= 0 && charTo > charFrom && charTo <= richTextBox.TextLength) ? charTo : richTextBox.TextLength;
			Win32.STRUCT_RECT rc;
			rc.top = (int)(r.Top / graphics.DpiY * 1440);
			rc.left = (int)(r.Left / graphics.DpiX * 1440);
			rc.right = (int)(r.Right / graphics.DpiX * 1440);
			rc.bottom = (int)(r.Bottom / graphics.DpiY * 1440);
			Win32.STRUCT_RECT rcPage;
			rcPage.top = 0;
			rcPage.left = 0;
			rcPage.right = rc.right;
			rcPage.bottom = rc.bottom;
			using(Graphics wndGraphics = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = graphics.GetHdc();
				IntPtr wndDc = wndGraphics.GetHdc();
				try {
					Win32.STRUCT_FORMATRANGE fr;
					fr.chrg = cr;
					fr.hdc = hdc;
					fr.hdcTarget = wndDc;
					fr.rc = rc;
					fr.rcPage = rcPage;
					IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
					Marshal.StructureToPtr(fr, lParam, false);
					Int32 wParam = measure ? 0 : 1;
					charEnd = Win32.SendMessage(richTextBox.Handle, Win32.EM_FORMATRANGE, wParam, lParam);
					if(measure) {
						fr = (Win32.STRUCT_FORMATRANGE)Marshal.PtrToStructure(lParam, typeof(Win32.STRUCT_FORMATRANGE));
						result = Convert.ToInt32(fr.rc.bottom * DpiY / 1440) + 1;
					}
					Marshal.FreeCoTaskMem(lParam);
				} finally {
					graphics.ReleaseHdc(hdc);
					wndGraphics.ReleaseHdc(wndDc);
				}
			}
			return result;
		}
	}
}
