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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Export;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class ZipCodeBrickExporter : VisualBrickExporter {
		#region static
		static Hashtable digitsTable = new Hashtable();
		static ZipCodeBrickExporter() {
			digitsTable['0'] = new int[] { 0, 1, 2, 3, 4, 5 };
			digitsTable['1'] = new int[] { 6, 3, 4 };
			digitsTable['2'] = new int[] { 5, 4, 8, 2 };
			digitsTable['3'] = new int[] { 5, 6, 7, 8 };
			digitsTable['4'] = new int[] { 0, 7, 4, 3 };
			digitsTable['5'] = new int[] { 5, 0, 7, 3, 2 };
			digitsTable['6'] = new int[] { 6, 1, 2, 3, 7 };
			digitsTable['7'] = new int[] { 5, 6, 1 };
			digitsTable['8'] = new int[] { 0, 1, 2, 3, 4, 5, 7 };
			digitsTable['9'] = new int[] { 8, 4, 5, 0, 7 };
			digitsTable['-'] = new int[] { 7 };
		}
		static void DrawSegment(IGraphics gr, Pen pen, RectangleF bounds, int index) {
			float y = (bounds.Top + bounds.Bottom) / 2.0f;
			switch (index) {
				case 0:
					gr.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, y);
					break;
				case 1:
					gr.DrawLine(pen, bounds.Left, y, bounds.Left, bounds.Bottom);
					break;
				case 2:
					gr.DrawLine(pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
					break;
				case 3:
					gr.DrawLine(pen, bounds.Right, bounds.Bottom, bounds.Right, y);
					break;
				case 4:
					gr.DrawLine(pen, bounds.Right, y, bounds.Right, bounds.Top);
					break;
				case 5:
					gr.DrawLine(pen, bounds.Right, bounds.Top, bounds.Left, bounds.Top);
					break;
				case 6:
					gr.DrawLine(pen, bounds.Right, bounds.Top, bounds.Left, y);
					break;
				case 7:
					gr.DrawLine(pen, bounds.Right, y, bounds.Left, y);
					break;
				case 8:
					gr.DrawLine(pen, bounds.Right, y, bounds.Left, bounds.Bottom);
					break;
			}
		}
		static void DrawDash(IGraphics gr, Pen pen, float x1, float y1, float x2, float y2) {
			gr.DrawLine(pen, x1, y1, x2, y2);
		}
		static float CalcDigitWidth(float height) {
			return height / 2.0f;
		}
		static float CalcSpaceWidth(float height) {
			return height / 12.0f;
		}
		static void DrawPlaceHolder(IGraphics gr, Pen pen, RectangleF bounds) {
			for (int i = 0; i < 9; i++)
				DrawSegment(gr, pen, bounds, i);
		}
		static void DrawChar(IGraphics gr, Pen pen, RectangleF bounds, char ch) {
			int[] array = digitsTable[ch] as int[];
			if (array == null)
				return;
			int count = array.Length;
			for (int i = 0; i < count; i++)
				DrawSegment(gr, pen, bounds, array[i]);
		}
		#endregion
		ZipCodeBrick ZipCodeBrick { get { return Brick as ZipCodeBrick; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			float penWidth = GraphicsUnitConverter.Convert(ZipCodeBrick.SegmentWidth, GraphicsUnit.Pixel, GraphicsUnit.Document);
			clientRect.Inflate(-penWidth / 2, -penWidth / 2);
			Brush brush = null;
			float width = GraphicsUnitConverter.Convert(1, GraphicsUnit.Pixel, GraphicsUnit.Document);
			Pen pen = new Pen(ZipCodeBrick.ForeColor, width);
			try {
				pen.DashStyle = DashStyle.Dot;
				pen.LineJoin = LineJoin.Round;
				using (pen) {
					Brush boldBrush = null;
					Pen boldPen = new Pen(ZipCodeBrick.ForeColor, penWidth);
					try {
						boldPen.DashStyle = DashStyle.Solid;
						boldPen.StartCap = LineCap.Triangle;
						boldPen.EndCap = LineCap.Triangle;
						boldPen.Alignment = PenAlignment.Left;
						using (boldPen) {
							float digitWidth = CalcDigitWidth(clientRect.Height);
							float spaceWidth = CalcSpaceWidth(clientRect.Height);
							RectangleF digitBounds = new RectangleF(clientRect.Left, clientRect.Top, digitWidth, clientRect.Height);
							int count = ZipCodeBrick.Text.Length;
							for (int i = 0; i < count; i++) {
								DrawPlaceHolder(gr, pen, digitBounds);
								DrawChar(gr, boldPen, digitBounds, ZipCodeBrick.Text[i]);
								digitBounds.Offset(spaceWidth + digitWidth + penWidth, 0);
							}
						}
					}
					finally {
						if (boldBrush != null) {
							boldBrush.Dispose();
							boldBrush = null;
						}
					}
				}
			}
			finally {
				if (brush != null) {
					brush.Dispose();
					brush = null;
				}
			}
		}
		protected override object[] GetSpecificKeyPart() {
			return new object[] { ZipCodeBrick.SegmentWidth, ZipCodeBrick.Text };
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			if(exportProvider.XlsExportContext.RawDataMode)
				exportProvider.SetCellText(ZipCodeBrick.Text, null);
			else
				FillTableCell(exportProvider);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider, ImageSizeMode.StretchImage, ImageAlignment.Default, exportProvider.CurrentData.OriginalBounds);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellWithImage(exportProvider);
		}
	}
}
