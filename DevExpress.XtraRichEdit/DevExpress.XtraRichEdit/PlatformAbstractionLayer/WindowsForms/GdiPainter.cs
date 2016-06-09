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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Drawing;
using DevExpress.Office.PInvoke;
namespace DevExpress.XtraRichEdit.Drawing {
	#region RichEditGdiPainter
	public class RichEditGdiPainter : GdiPainter, DevExpress.XtraRichEdit.API.Layout.ISupportCustomPainting {
		readonly GdiPlusBoxMeasurer measurer;
		public RichEditGdiPainter(GraphicsCache cache, GdiPlusBoxMeasurer measurer)
			: base(cache, measurer.DocumentModel.LayoutUnitConverter) {
			Guard.ArgumentNotNull(measurer, "measurer");
			this.measurer = measurer;
		}
		#region Properties
		public GdiPlusBoxMeasurer Measurer { get { return measurer; } }
		#endregion
		[System.Security.SecuritySafeCritical]
		protected override IntPtr GetMeasureHdc(IntPtr hdc) {
			return Measurer.Graphics.GetHdc();
		}
		protected override void ReleaseMeasureHdc(IntPtr measureHdc) {
			Measurer.Graphics.ReleaseHdc(measureHdc);
		}
		protected override void DrawImageCore(Image img, RectangleF bounds, XtraPrinting.ImageSizeMode sizing) {
			if (sizing != XtraPrinting.ImageSizeMode.Tile)
				base.DrawImageCore(img, bounds, sizing);
			else {
				using (TextureBrush brush = new TextureBrush(img, System.Drawing.Drawing2D.WrapMode.Tile)) {
					Graphics.FillRectangle(brush, bounds);
				}
			}
		}
		protected internal override void DefaultDrawStringImplementation(IntPtr hdc, string text, FontInfo fontInfo, Rectangle bounds, StringFormat stringFormat) {
			GdiTextViewInfo textInfo = (GdiTextViewInfo)Measurer.TextViewInfoCache.TryGetTextViewInfo(text, fontInfo);
			if (textInfo != null)
				DrawStringCore(hdc, text, bounds, textInfo);
			else
				DrawNonCachedString(hdc, fontInfo, text, bounds, stringFormat);
		}
		protected internal virtual void DrawStringCore(IntPtr hdc, string text, Rectangle bounds, GdiTextViewInfo textInfo) {
			Win32.RECT clipRect = Win32.RECT.FromRectangle(bounds);
			Win32.ExtTextOut(
				hdc,
				bounds.X,
				bounds.Y,
				Win32.EtoFlags.ETO_GLYPH_INDEX,
				ref clipRect,
				textInfo.Glyphs,
				textInfo.GlyphCount,
				textInfo.CharacterWidths
				);
		}
		public override void DrawEllipse(Pen pen, Rectangle bounds) {
			Graphics.DrawEllipse(pen, bounds);
		}
		#region ISupportCustomPainting Members
		void API.Layout.ISupportCustomPainting.BeginCustomPaint() {
		}
		void API.Layout.ISupportCustomPainting.EndCustomPaint() {
		}
		#endregion
	}
	#endregion
}
