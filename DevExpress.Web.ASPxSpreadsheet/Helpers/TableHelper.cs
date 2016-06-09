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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Export.Xl;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class SpreadsheetTableStylesHelper {
		protected const int TablePresetStyleWidth = 86;
		protected const int TablePresetStyleHeight = 66;
		protected DocumentModel DocumentModel { get; private set; }
		protected ASPxSpreadsheet Spreadsheet { get; private set; }
		public SpreadsheetTableStylesHelper(ASPxSpreadsheet spreadsheetControl) {
			Spreadsheet = spreadsheetControl;
			if(Spreadsheet.GetCurrentWorkSessions() != null)
				DocumentModel = spreadsheetControl.GetCurrentWorkSessions().DocumentModel;
		}
		public string GetPresetImageURL(string styleName) {
			var img = GetPresetImage(styleName);
			return GetImageURL(img);
		}
		string GetImageURL(OfficeImage image) {
			return BinaryStorage.GetImageUrl(Spreadsheet, image.GetImageBytes(OfficeImageFormat.Png), BinaryStorageMode.Cache);
		}
		OfficeImage GetPresetImage(string styleName) {
			ITableStyleViewInfo helper =  new TableStyleViewInfo(DocumentModel, styleName);
			Bitmap bmp = new Bitmap(TablePresetStyleWidth, TablePresetStyleHeight);
			Graphics g = Graphics.FromImage(bmp);
			TableStyleViewInfoPainter.Draw(helper, g, new Rectangle(0, 0, TablePresetStyleWidth, TablePresetStyleHeight));
			return OfficeImage.CreateImage(bmp);
		}
	}
	public class TableStyleViewInfoPainter {
		#region Static Members
		protected internal static void Draw(ITableStyleViewInfo helper, Graphics g, Rectangle rectangle) {
			TableStyleViewInfoPainter painter = new TableStyleViewInfoPainter();
			painter.Helper = helper;
			painter.Graphics = g;
			painter.Draw(rectangle);
		}
		#endregion
		const int delta = 3;
		#region Properties
		int RowCount { get { return Helper.RowCount; } }
		int ColumnCount { get { return Helper.ColumnCount; } }
		public ITableStyleViewInfo Helper { get; set; }
		public Graphics Graphics { get; set; }
		#endregion
		public void Draw(Rectangle tableRectangle) {
			int partWidth = tableRectangle.Width / ColumnCount;
			int partHeight = tableRectangle.Height / RowCount;
			for(int column = 0; column < ColumnCount; column++)
				for(int row = 0; row < RowCount; row++) {
					Rectangle rectanglePart = new Rectangle(tableRectangle.X + column * partWidth, tableRectangle.Y + row * partHeight, partWidth, partHeight);
					CellPosition absolutePosition = new CellPosition(column, row);
					DrawBackgroundPart(rectanglePart, absolutePosition);
					DrawLines(rectanglePart, absolutePosition);
				}
		}
		void DrawBackgroundPart(Rectangle rectanglePart, CellPosition absolutePosition) {
			IActualFillInfo fill = Helper.GetActualFillInfo(absolutePosition);
			if(fill.FillType == ModelFillType.Pattern)
				DrawBackgroundPartFromPatternFill(rectanglePart, fill);
			else
				DrawBackgroundPartFromGradientFill(rectanglePart, fill.GradientFill);
		}
		void DrawBackgroundPartFromPatternFill(Rectangle fillBounds, IActualFillInfo fill) {
			Color backColor = Cell.GetBackgroundColor(fill);
			DrawBackgroundFromColor(fillBounds, backColor);
		}
		void DrawBackgroundPartFromGradientFill(Rectangle fillBounds, IActualGradientFillInfo gradientFill) {
			CellBackgroundDisplayFormat displayFormat = new CellBackgroundDisplayFormat();
			displayFormat.GradientFill = gradientFill;
			displayFormat.Bounds = fillBounds;
			DrawBackgroundFromBrush(displayFormat);
		}
		void DrawBackgroundFromColor(Rectangle fillBounds, Color backColor) {
			if(DXColor.IsTransparentOrEmpty(backColor))
				backColor = Color.White;
			Graphics.FillRectangle(new SolidBrush(backColor), fillBounds);
		}
		void DrawBackgroundFromBrush(CellBackgroundDisplayFormat displayFormat) {
			Rectangle fillBounds = displayFormat.Bounds;
			Brush brush = displayFormat.CreateGradientBrush();
			if(brush != Brushes.Transparent)
				using(brush)
					Graphics.FillRectangle(brush, fillBounds);
		}
		void DrawLines(Rectangle borderBounds, CellPosition absolutePosition) {
			DrawLeftLine(borderBounds, absolutePosition);
			DrawRightLine(borderBounds, absolutePosition);
			DrawTopLine(borderBounds, absolutePosition);
			DrawBottomLine(borderBounds, absolutePosition);
			DrawEmbeddedLine(borderBounds, absolutePosition);
		}
		void DrawLeftLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Helper.GetLeftBorderLineStyle(absolutePosition);
			if(style != XlBorderLineStyle.None) {
				Color color = Helper.GetLeftBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Y, borderBounds.X, borderBounds.Bottom);
			}
		}
		void DrawRightLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Helper.GetRightBorderLineStyle(absolutePosition);
			if(style != XlBorderLineStyle.None) {
				Color color = Helper.GetRightBorderColor(absolutePosition);
				DrawLine(color, borderBounds.Right, borderBounds.Y, borderBounds.Right, borderBounds.Bottom);
			}
		}
		void DrawTopLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Helper.GetTopBorderLineStyle(absolutePosition);
			if(style != XlBorderLineStyle.None) {
				Color color = Helper.GetTopBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Y, borderBounds.Right, borderBounds.Y);
			}
		}
		void DrawBottomLine(Rectangle borderBounds, CellPosition absolutePosition) {
			XlBorderLineStyle style = Helper.GetBottomBorderLineStyle(absolutePosition);
			if(style != XlBorderLineStyle.None) {
				Color color = Helper.GetBottomBorderColor(absolutePosition);
				DrawLine(color, borderBounds.X, borderBounds.Bottom, borderBounds.Right, borderBounds.Bottom);
			}
		}
		void DrawEmbeddedLine(Rectangle borderBounds, CellPosition absolutePosition) {
			Color color = Helper.GetTextColor(absolutePosition);
			color = DXColor.IsTransparentOrEmpty(color) ? DXColor.Black : color;
			int y = borderBounds.Y + borderBounds.Height / 2;
			DrawLine(color, borderBounds.X + delta, y, borderBounds.Right - delta, y);
		}
		void DrawLine(Color color, int beginX, int beginY, int endX, int endY) {
			using(Pen pen = new Pen(color, 1))
				Graphics.DrawLine(pen, beginX, beginY, endX, endY);
		}
	}
}
