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
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class ProgressBarBrickExporter : VisualBrickExporter {
		ProgressBarBrick ProgressBarBrick { get { return Brick as ProgressBarBrick; } }
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
			GdiHashtable gdi = gr.PrintingSystem.Gdi;
			RectangleF drawRect = rect;
			gr.FillRectangle(gdi.GetBrush(Style.BackColor), drawRect);
			gr.FillRectangle(
				gdi.GetBrush(Color.FromArgb(Style.BackColor.ToArgb() ^ 0x222222)),
				new RectangleF(drawRect.X, drawRect.Y, drawRect.Width * ProgressBarBrick.ValidPosition / 100f, drawRect.Height));
			if(BorderSide.None != Style.Sides) {
				DrawBorders(gr, rect);
			}
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			if(clientRect.IsEmpty)
				return;
			Brush brush = gr.PrintingSystem.Gdi.GetBrush(ProgressBarBrick.ForeColor);
			using(BrickStringFormat stringFormat = new BrickStringFormat(Style.StringFormat, StringAlignment.Center, StringAlignment.Center)) {
				DrawText(gr, clientRect, stringFormat.Value, brush);
			}
		}
		void DrawText(IGraphics gr, RectangleF clientRectangle, StringFormat sf, Brush brush) {
			string textToDraw = String.Format("{0}%", ProgressBarBrick.Position);
			if(Style.IsJustified) {
				JustifiedStringPainter.DrawString(textToDraw, gr, Style.Font, brush, clientRectangle, sf, true);
			} else {
				gr.DrawString(textToDraw, Style.Font, brush, clientRectangle, sf);
			}
		}
		protected internal override BrickViewData[] GetXlsData(ExportContext xlsExportContext, RectangleF rect) {
			return xlsExportContext.CreateBrickViewDataArray(ProgressBarBrick.Style, rect, TableCell);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			exportProvider.SetCellText(GetTextValueCore(exportProvider.XlsExportContext.XlsExportOptions.TextExportMode), null);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			return GetExportDataCore(exportContext, rect);
		}
		object GetTextValueCore(TextExportMode textExportMode) {
			object result = ProgressBarBrick.TextValue != null ? ProgressBarBrick.TextValue : ProgressBarBrick.Position;
			return textExportMode == TextExportMode.Value ? result : (object)result.ToString();
		}
		BrickViewData[] GetExportDataCore(ExportContext exportContext, RectangleF rect) {
			RectangleF leftRect = new RectangleF(rect.X, rect.Y, rect.Width * ProgressBarBrick.ValidPosition / 100, rect.Height);
			RectangleF rightRect = new RectangleF(rect.X + leftRect.Width, rect.Y, rect.Width - leftRect.Width, rect.Height);
			BrickStyle leftStyle = new BrickStyle(Style);
			leftStyle.BackColor = ProgressBarBrick.ForeColor;
			leftStyle.Sides &= ~BorderSide.Right;
			BrickStyle rightStyle = new BrickStyle(Style);
			rightStyle.Sides &= ~BorderSide.Left;
			return new BrickViewData[] { 
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(leftStyle), leftRect, TableCell), 
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(rightStyle), rightRect, TableCell) 
			};
		}
	}
}
