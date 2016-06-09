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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class TrackBarBrickExporter : VisualBrickExporter {
		protected override void DrawObject(XtraPrinting.IGraphics gr, System.Drawing.RectangleF rect) {
			TrackBarBrick brick = Brick as TrackBarBrick;
			if(brick != null)
				BrickPaint.DrawTrackBar(gr, rect, gr.PrintingSystem.Gdi, brick.Position, brick.Minimum, brick.Maximum);
		}
		TrackBarBrick TrackBarBrick { get { return Brick as TrackBarBrick; } }
		protected internal override BrickViewData[] GetXlsData(ExportContext xlsExportContext, RectangleF rect) {
			return xlsExportContext.CreateBrickViewDataArray(TrackBarBrick.Style, rect, TableCell);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			exportProvider.SetCellText(GetTextValueCore(exportProvider.XlsExportContext.XlsExportOptions.TextExportMode), null);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			return GetExportDataCore(exportContext, rect);
		}
		protected internal override BrickViewData[] GetHtmlData(ExportContext htmlExportContext, RectangleF rect, RectangleF clipRect) {
			return GetHtmlExportDataCore(htmlExportContext, rect);
		}
		object GetTextValueCore(TextExportMode textExportMode) {
			object result = TrackBarBrick.TextValue != null ? TrackBarBrick.TextValue : TrackBarBrick.Position;
			return textExportMode == TextExportMode.Value ? result : (object)result.ToString();
		}
		BrickViewData[] GetHtmlExportDataCore(ExportContext exportContext, RectangleF rect) {
			int pos = TrackBarBrick.Position, min = TrackBarBrick.Minimum, max = TrackBarBrick.Maximum, offset = 4; 
			Size thumb = new Size(10, (int)rect.Height - 2 * offset);
			RectangleF thumbF = new RectangleF(rect.X + (rect.Width - thumb.Width) * (pos - min) / (max - min), rect.Y - thumb.Height / 2 + rect.Height / 2, thumb.Width, thumb.Height);
			BrickStyle thumbStyle = new BrickStyle(Style);
			thumbStyle.BackColor = TrackBarBrick.ForeColor;
			thumbStyle.Sides = BorderSide.None;
			BrickStyle leftTopStyle = new BrickStyle(Style) { Sides = BorderSide.Left | BorderSide.Top };
			BrickStyle leftBottomStyle = new BrickStyle(Style) { Sides = BorderSide.Left | BorderSide.Bottom };
			BrickStyle rightTopStyle = new BrickStyle(Style) { Sides = BorderSide.Right | BorderSide.Top };
			BrickStyle rightBottomStyle = new BrickStyle(Style) { Sides = BorderSide.Right | BorderSide.Bottom };
			BrickStyle topStyle = new BrickStyle(Style) { Sides = BorderSide.Top };
			BrickStyle bottomStyle = new BrickStyle(Style) { Sides = BorderSide.Bottom };
			if(pos == min) {
				topStyle.Sides = BorderSide.Top | BorderSide.Left;
				bottomStyle.Sides = BorderSide.Bottom | BorderSide.Left;
				thumbStyle.Sides = BorderSide.Left;
			}
			if(pos == max) {
				topStyle.Sides = BorderSide.Top | BorderSide.Right;
				bottomStyle.Sides = BorderSide.Bottom | BorderSide.Right;
				thumbStyle.Sides = BorderSide.Right;
			}
			BrickStyle lineLeftStyle = new BrickStyle(Style) { Sides = BorderSide.Left, BackColor = TrackBarBrick.ForeColor };
			BrickStyle lineRightStyle = new BrickStyle(Style) { Sides = BorderSide.Right, BackColor = TrackBarBrick.ForeColor };
			int lineHeight = 2;
			SizeF leftRectSize = new SizeF(thumbF.X - rect.X, rect.Height / 2 - lineHeight / 2);
			RectangleF leftTopRect = new RectangleF(rect.Location, leftRectSize);
			RectangleF leftBottomRect = new RectangleF(new PointF(rect.X, rect.Y + rect.Height / 2 + lineHeight / 2), leftRectSize);
			SizeF rightRectSize = new SizeF(rect.Right - thumbF.Right, rect.Height / 2 - lineHeight / 2);
			RectangleF rightTopRect = new RectangleF(new PointF(thumbF.Right, rect.Y), rightRectSize);
			RectangleF rightBottomRect = new RectangleF(new PointF(thumbF.Right, rect.Y + rect.Height / 2 + lineHeight / 2), rightRectSize);
			SizeF topRectSize = new SizeF(thumbF.Width, offset);
			RectangleF topRect = new RectangleF(new PointF(thumbF.X, rect.Y), topRectSize);
			RectangleF bottomRect = new RectangleF(new PointF(thumbF.X, thumbF.Bottom), topRectSize);
			RectangleF lineLeftRect = new RectangleF(new PointF(rect.X, rect.Y + rect.Height / 2 - lineHeight / 2), new SizeF(leftRectSize.Width, lineHeight));
			RectangleF lineRightRect = new RectangleF(new PointF(thumbF.Right, rect.Y + rect.Height / 2 - lineHeight / 2), new SizeF(rightRectSize.Width, lineHeight));
			return new BrickViewData[] { 
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(thumbStyle), thumbF, TableCell),				
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(lineLeftStyle), lineLeftRect, TableCell),
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(lineRightStyle), lineRightRect, TableCell),			   
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(leftTopStyle), leftTopRect, TableCell),
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(leftBottomStyle), leftBottomRect, TableCell),				
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(rightTopStyle), rightTopRect, TableCell),
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(rightBottomStyle), rightBottomRect, TableCell),				
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(topStyle), topRect, TableCell),
				exportContext.CreateBrickViewData(exportContext.PrintingSystem.Styles.GetStyle(bottomStyle), bottomRect, TableCell)
			};
		}
		BrickViewData[] GetExportDataCore(ExportContext exportContext, RectangleF rect) {
			int pos = TrackBarBrick.Position, min = TrackBarBrick.Minimum, max = TrackBarBrick.Maximum;
			RectangleF leftRect = new RectangleF(rect.X, rect.Y, rect.Width * (pos - min) / (max - min), rect.Height);
			RectangleF rightRect = new RectangleF(rect.X + leftRect.Width, rect.Y, rect.Width - leftRect.Width, rect.Height);
			BrickStyle leftStyle = new BrickStyle(Style);
			leftStyle.BackColor = TrackBarBrick.ForeColor;
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
