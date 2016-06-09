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
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting.Stubs;
using DevExpress.Xpf.Drawing.Imaging;
using DevExpress.Xpf.Security;
using DevExpress.Xpf.Drawing;
#else
#if !DXPORTABLE
using DevExpress.Data.Helpers;
using System.Drawing.Imaging;
using System.Security.Permissions;
#endif
#endif
namespace DevExpress.XtraPrinting.Native {
	public interface IMeasurer {
		SizeF MeasureString(string text, Font font, PointF location, StringFormat stringFormat, GraphicsUnit pageUnit);
		SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit);
		SizeF MeasureString(string text, Font font, float width, StringFormat stringFormat, GraphicsUnit pageUnit);
		SizeF MeasureString(string text, Font font, GraphicsUnit pageUnit);
	}
#if !DXPORTABLE
	public static class GraphicsHelper {
		[ThreadStatic]
		static Bitmap staticBitmap;
		[ThreadStatic]
		static Bitmap staticHiResBitmap;
		static bool? canCreateFromZeroHwnd;
#if !SL
		static bool? canUseGetHdc;
		static bool? canUseFontSizeInPoints;
#endif
		public static bool CanCreateFromZeroHwnd {
			get {
				if(!canCreateFromZeroHwnd.HasValue)
					try {
						using(Graphics g = CreateGraphicsFromZeroHwnd())
							canCreateFromZeroHwnd = true;
					} catch {
						canCreateFromZeroHwnd = false;
					}
				return canCreateFromZeroHwnd.Value;
			}
		}
#if !SL
		public static bool CanUseGetHdc {
			get {
				if(!canUseGetHdc.HasValue) {
					try {
						using (Bitmap bmp = new Bitmap(1, 1)) {
							using (Graphics gr = Graphics.FromImage(bmp)) {
								try {
									gr.GetHdc();
								}
								finally {
									try {
										gr.ReleaseHdc();
									}
									catch {
										canUseGetHdc = false;										
									}
								}
							}
						}
						if(!canUseGetHdc.HasValue)
							canUseGetHdc = true;
					}
					catch {
						canUseGetHdc = false;
					}
				}
				return canUseGetHdc.GetValueOrDefault();
			}
		}
		public static bool CanUseFontSizeInPoints {
			get {
				if (!canUseFontSizeInPoints.HasValue) {
					try {
						using(Font font = new Font("Arial", 12, GraphicsUnit.Document)) {
							float sizeInPoints = font.SizeInPoints;
						}
						canUseFontSizeInPoints = true;
					}
					catch {
						canUseFontSizeInPoints = false;
					}
				}
				return canUseFontSizeInPoints.GetValueOrDefault();
			}
		}
#endif
		public static void ResetCanCreateFromZeroHwnd() {
			canCreateFromZeroHwnd = null;
		}
		public static Graphics CreateGraphicsWithoutAspCheck() {
			return CreateGraphicsCore(false);
		}
		public static Graphics CreateGraphics() {
			return CreateGraphicsCore(true);
		}
		static Graphics CreateGraphicsCore(bool checkAsp) {
			if (CanCreateFromZeroHwnd) {
				Graphics gr = CreateGraphicsFromZeroHwnd();
				if (checkAsp && PSNativeMethods.AspIsRunning && (gr.DpiX != 96.0 || gr.DpiY != 96.0))
					gr.Dispose();
				else
					return gr;
			}
			return CreateGraphicsFromImage();
		}
		static Graphics CreateGraphicsFromImage() {
			if(staticBitmap == null) {
				staticBitmap = new Bitmap(10, 10, PixelFormat.Format32bppArgb);
				staticBitmap.SetResolution(96f, 96f);
			}
			return Graphics.FromImage(staticBitmap);
		}
		public static Graphics CreateGraphicsFromHiResImage() {
			if(staticHiResBitmap == null) {
				staticHiResBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
				staticHiResBitmap.SetResolution(300f, 300f);
			}
			return Graphics.FromImage(staticHiResBitmap);
		}
#if DEBUGTEST
		public static Graphics Test_CreateGraphicsFromImage() {
			return CreateGraphicsFromImage();
		}
#endif
		static Graphics CreateGraphicsFromZeroHwnd() {
			return Graphics.FromHwnd(IntPtr.Zero);
		}
	}
#endif
	#region FontSizeHelper
	public static class FontSizeHelper {
#if !SL && !DXPORTABLE
		public static float GetSizeInPoints(Font font) {
			if (DevExpress.XtraPrinting.Native.GraphicsHelper.CanUseFontSizeInPoints)
				return font.SizeInPoints;
			switch (font.Unit) {
				case GraphicsUnit.Document:
					return DocumentsToPointsF(font.Size);
				case GraphicsUnit.Inch:
					return InchesToPointsF(font.Size);
				case GraphicsUnit.Millimeter:
					return MillimetersToPointsF(font.Size);
				case GraphicsUnit.Point:
					return font.Size;
				default:
					return font.Size;
			}
		}
		static float InchesToPointsF(float val) {
			return val * 72;
		}
		static float DocumentsToPointsF(float val) {
			return val * 6.0f / 25.0f;
		}
		static float MillimetersToPointsF(float val) {
			return 72.0f * val / 25.4f;
		}
#else
		public static float GetSizeInPoints(Font font) {
			return font.SizeInPoints;
		}
#endif
	}
#endregion
}
