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
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Localization;
namespace DevExpress.Snap.Core.Native {
	class ChartBoxPainter : IDisposable {
		#region Constants
		readonly static Font font = new Font(SystemFonts.DefaultFont.FontFamily, 10);
		readonly static Font boldFont = new Font(font, FontStyle.Bold);
		readonly static Brush fontBrush = Brushes.White;
		readonly static Brush brush = new SolidBrush(Color.FromArgb((int)(256 * 0.9), Color.FromArgb(0x86, 0x86, 0x86)));
		#endregion
		public Font Font { get { return font; } }
		public Font BoldFont { get { return boldFont; } }
		public Brush Brush { get { return brush; } }
		public Brush FontBrush { get { return fontBrush; } }
		public void DrawInfoPanel(Graphics graphics, ChartBoxInfoPanelViewInfo viewInfo) {
			DrawBackground(graphics, viewInfo.Bounds);
			graphics.TranslateTransform(viewInfo.Bounds.X, viewInfo.Bounds.Y);
			foreach (var section in viewInfo.Sections) {
				DrawText(graphics, section.Header, BoldFont);
				foreach (var item in section.Items)
					DrawText(graphics, item, Font);
			}
		}
		void DrawBackground(Graphics graphics, Rectangle bounds) {
			graphics.FillRectangle(Brush, bounds);
		}
		void DrawText(Graphics graphics, ChartBoxInfoPanelStringViewInfo stringViewInfo, Font font) {
			graphics.DrawString(stringViewInfo.Text, font, FontBrush, stringViewInfo.Location.X, stringViewInfo.Location.Y, StringFormat.GenericTypographic);
		}
		Rectangle ToRectangle(RectangleF rectF) {
			return new Rectangle(new Point((int)rectF.X, (int)rectF.Y), rectF.Size.ToSize());
		}
		public void Dispose() {
		}
	}
	public class ChartBoxHotZonePainter : BoxHotZonePainter, IChartDropZoneVisitor {
		public ChartBoxHotZonePainter(Painter painter) : base(painter) { }
		public static readonly string UIString_DropValues = SnapLocalizer.GetString(SnapStringId.HotZonePainter_DropValues);
		public static readonly string UIString_DropArguments = SnapLocalizer.GetString(SnapStringId.HotZonePainter_DropArguments);
		protected override string GetLongestString() {
			using(Font font = new Font(SystemFonts.DefaultFont.FontFamily, DefaultFontSize)) {
				float uiStringDropValuesWidth = MeasureString(UIString_DropValues, font).Width;
				float uiStringDropArgumentsWidth = MeasureString(UIString_DropArguments, font).Width;
				float uiStringSecondLineWidth = MeasureString(UIString_SecondLine, font).Width;
				if(uiStringSecondLineWidth > Math.Max(uiStringDropArgumentsWidth, uiStringDropValuesWidth))
					return UIString_SecondLine;
				return uiStringDropArgumentsWidth > uiStringDropValuesWidth ? UIString_DropArguments : UIString_DropValues;
			}
		}
		void IChartDropZoneVisitor.Visit(DropValuesHotZone hotZone) {
			DrawHotZone(hotZone, ChartBoxHotZonePainter.UIString_DropValues);
		}
		void IChartDropZoneVisitor.Visit(DropArgumentsHotZone hotZone) {
			DrawHotZone(hotZone, ChartBoxHotZonePainter.UIString_DropArguments);
		}
	}
}
