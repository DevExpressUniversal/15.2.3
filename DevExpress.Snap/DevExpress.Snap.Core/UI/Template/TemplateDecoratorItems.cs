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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using System.Drawing;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.UI.Templates {	
	public class TemplateDecoratorItem  {
		protected const int HorizontalDistance = 1;
		public TemplateDecoratorItem(TemplateDecoratorItemBoundaryBase startBoundary, TemplateDecoratorItemBoundaryBase endBoundary) {
			StartBoundary = startBoundary;
			EndBoundary = endBoundary;
		}
		protected virtual TemplateDecoratorItemBoundaryBase StartBoundary { get; private set; }
		protected virtual TemplateDecoratorItemBoundaryBase EndBoundary { get; private set; }
		public Point[][] GetGraphicsPathPoints(ILayoutToPhysicalBoundsConverter converter) {
			Rectangle startBounds = StartBoundary.GetPhysicalBoundsAsStartBoundary(converter);
			Rectangle endBounds = EndBoundary.GetPhysicalBoundsAsEndBoundary(converter);
			startBounds.Inflate(HorizontalDistance, 0);
			endBounds.Inflate(HorizontalDistance, 0);
			Point[] singleTextRowPoints = GetGraphicsPathSingleRow(startBounds, endBounds);
			if (singleTextRowPoints != null)
				return new Point[][] { singleTextRowPoints };
			if (StartBoundary is TemplateDecoratorItemTableRowBoundary && EndBoundary is TemplateDecoratorItemColumnBoundary) {
				endBounds = Rectangle.FromLTRB(Math.Min(endBounds.Left, startBounds.Left), endBounds.Top, Math.Max(endBounds.Right, startBounds.Right), endBounds.Bottom);
			}
			if (StartBoundary is TemplateDecoratorItemColumnBoundary && EndBoundary is TemplateDecoratorItemTableRowBoundary) {
				startBounds = Rectangle.FromLTRB(Math.Min(endBounds.Left, startBounds.Left), startBounds.Top, Math.Max(endBounds.Right, startBounds.Right), startBounds.Bottom);
			}
			if (StartBoundary is TemplateDecoratorItemColumnBoundary && EndBoundary is TemplateDecoratorItemColumnBoundary) {
				TemplateDecoratorItemColumnBoundary columnBoundary = StartBoundary as TemplateDecoratorItemColumnBoundary;
				Rectangle columnBounds = converter.GetPixelPhysicalBounds(GetColumnContentBounds(columnBoundary.Column));
				startBounds = new Rectangle(columnBounds.X, columnBounds.Y, columnBounds.Width, 0);
				endBounds = new Rectangle(columnBounds.X, columnBounds.Bottom, columnBounds.Width, 0);
			}
			int leftMiddleTop = startBounds.Bottom;
			int rightMiddleBottom = endBounds.Top;
			Point[] startRectPoints = GetRectanglePoints(startBounds);
			Point[] endRectPoints = GetRectanglePoints(endBounds);
			if (startBounds.Left >= endBounds.Right && rightMiddleBottom - leftMiddleTop < 2) {
				return new Point[][] { startRectPoints, endRectPoints };
			}
			else {
				Point[] points = new Point[] { 
					startRectPoints[0], startRectPoints[1], new Point(startRectPoints[2].X, rightMiddleBottom), new Point(endRectPoints[1].X, rightMiddleBottom),
					endRectPoints[2], endRectPoints[3], new Point(endRectPoints[0].X, leftMiddleTop), new Point(startRectPoints[3].X, leftMiddleTop)
				};
				return new Point[][]{ points };
			}
		}
		Rectangle GetColumnContentBounds(Column column) {
			int minX = Int32.MaxValue;
			int minY = Int32.MaxValue;
			int maxX = Int32.MinValue;
			int maxY = Int32.MinValue;
			RowCollection rows = column.Rows;
			int rowsCount = rows.Count;
			for (int i = 0; i < rowsCount; i++) {
				Rectangle bounds = rows[i].ActualSizeBounds;
				minX = Math.Min(bounds.X, minX);
				minY = Math.Min(bounds.Y, minY);
				maxX = Math.Max(bounds.Right, maxX);
				maxY = Math.Max(bounds.Bottom, maxY);
			}
			TableViewInfoCollection tables = column.InnerTables;
			if (tables != null) {
				int columnLeft = column.Bounds.Left;
				int tablesCount = tables.Count;
				for (int i = 0; i < tablesCount; i++) {
					TableViewInfo viewInfo = tables[i];
					if (viewInfo.Table.NestedLevel > 0)
						continue;					
					DevExpress.XtraRichEdit.Utils.SortedList<int> positions = viewInfo.VerticalBorderPositions.AlignmentedPosition;
					int positionsCount = positions.Count;
					if (positionsCount > 1) {
						minX = Math.Min(positions[0] + columnLeft, minX);
						maxX = Math.Max(positions[positionsCount - 1] + columnLeft, maxX);
					}
					minY = Math.Min(viewInfo.TopAnchor.VerticalPosition, minY);
					maxY = Math.Min(viewInfo.BottomAnchor.VerticalPosition, maxY);
				}
			}
			return Rectangle.FromLTRB(minX, minY, maxX, maxY);
		}
		protected virtual Point[] GetGraphicsPathSingleRow(Rectangle startBounds, Rectangle endBounds) {
			TemplateDecoratorItemParagraphBoundary startTextBoundary = StartBoundary as TemplateDecoratorItemParagraphBoundary;
			if (startTextBoundary == null)
				return null;
			TemplateDecoratorItemParagraphBoundary endTextBoundary = EndBoundary as TemplateDecoratorItemParagraphBoundary;
			if (endTextBoundary == null)
				return null;
			if (!Object.ReferenceEquals(startTextBoundary.Row, endTextBoundary.Row))
				return null;					  
			Rectangle rect = Rectangle.FromLTRB(startBounds.Left, Math.Min(startBounds.Top, endBounds.Top), endBounds.Right, Math.Min(startBounds.Bottom, endBounds.Bottom));			
			return GetRectanglePoints(rect);
		}
		protected Point[] GetRectanglePoints(Rectangle rect) {
			return new Point[] { new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Top), new Point(rect.Right, rect.Bottom), new Point(rect.Left, rect.Bottom) };			
		}
	}   
	public abstract class TemplateDecoratorItemBoundaryBase {		
		protected TemplateDecoratorItemBoundaryBase(Column column) {
			Column = column;
		}
		public Column Column { get; private set; }
		public abstract Rectangle GetPhysicalBoundsAsStartBoundary(ILayoutToPhysicalBoundsConverter converter);
		public abstract Rectangle GetPhysicalBoundsAsEndBoundary(ILayoutToPhysicalBoundsConverter converter);
	}
	public class TemplateDecoratorItemColumnBoundary : TemplateDecoratorItemBoundaryBase {
		public TemplateDecoratorItemColumnBoundary(Column column) : base(column) {			
		}		
		public override Rectangle GetPhysicalBoundsAsStartBoundary(ILayoutToPhysicalBoundsConverter converter) {
			Rectangle result = converter.GetPixelPhysicalBounds(Column.Bounds);
			result.Height = 0;
			return result;
		}
		public override Rectangle GetPhysicalBoundsAsEndBoundary(ILayoutToPhysicalBoundsConverter converter) {
			Rectangle result = converter.GetPixelPhysicalBounds(Column.Bounds);
			result.Y += result.Height;
			result.Height = 0;
			return result;
		}
	}
	public class TemplateDecoratorItemParagraphBoundary : TemplateDecoratorItemBoundaryBase {
		public TemplateDecoratorItemParagraphBoundary(Column column, Row row)
			: base(column) {
			Row = row;
		}
		public Row Row { get; private set; }
		public override Rectangle GetPhysicalBoundsAsStartBoundary(ILayoutToPhysicalBoundsConverter converter) {
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(Row.Boxes.Last.Bounds.Left, Row.Bounds.Top, Column.Bounds.Right, Row.Bounds.Bottom));
		}
		public override Rectangle GetPhysicalBoundsAsEndBoundary(ILayoutToPhysicalBoundsConverter converter) {
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(Column.Bounds.Left , Row.Bounds.Top, Column.Bounds.Right, Row.Bounds.Bottom));
		}
	}
	public class TemplateDecoratorItemTextBoundary : TemplateDecoratorItemParagraphBoundary {
		public TemplateDecoratorItemTextBoundary(Column column, Row row, CharacterBox characterBox)
			: base(column, row) {			
			CharacterBox = characterBox;
		}
		public CharacterBox CharacterBox { get; private set; }
		public override Rectangle GetPhysicalBoundsAsStartBoundary(ILayoutToPhysicalBoundsConverter converter) {
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(CharacterBox.Bounds.Left, Row.Bounds.Top, Column.Bounds.Right, Row.Bounds.Bottom));			
		}
		public override Rectangle GetPhysicalBoundsAsEndBoundary(ILayoutToPhysicalBoundsConverter converter) {
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(Column.Bounds.Left, Row.Bounds.Top, CharacterBox.Bounds.Right, Row.Bounds.Bottom));
		}
	}
	public class TemplateDecoratorItemTableRowBoundary : TemplateDecoratorItemBoundaryBase {
		public TemplateDecoratorItemTableRowBoundary(Column column, TableCellViewInfo cell)
			: base(column) {
			Cell = cell;			
		}
		public TableCellViewInfo Cell { get; private set; }
		public override Rectangle GetPhysicalBoundsAsStartBoundary(ILayoutToPhysicalBoundsConverter converter) {
			TableCellVerticalAnchor anchor = Cell.TableViewInfo.Anchors[Cell.TopAnchorIndex];
			int tableLeft = Cell.TableViewInfo.GetColumnLeft(0);
			int tableRight = Cell.TableViewInfo.GetColumnRight(Cell.TableViewInfo.VerticalBorderPositions.AlignmentedPosition.Count - 2);
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(Math.Min(Column.Bounds.Left, tableLeft), anchor.VerticalPosition, Math.Max(Column.Bounds.Right, tableRight), anchor.VerticalPosition));			
		}
		public override Rectangle GetPhysicalBoundsAsEndBoundary(ILayoutToPhysicalBoundsConverter converter) {
			TableCellVerticalAnchor anchor = Cell.TableViewInfo.Anchors[Cell.BottomAnchorIndex];
			int tableLeft = Cell.TableViewInfo.GetColumnLeft(0);
			int tableRight = Cell.TableViewInfo.GetColumnRight(Cell.TableViewInfo.VerticalBorderPositions.AlignmentedPosition.Count - 2);
			return converter.GetPixelPhysicalBounds(Rectangle.FromLTRB(Math.Min(Column.Bounds.Left, tableLeft), anchor.VerticalPosition, Math.Max(Column.Bounds.Right, tableRight), anchor.VerticalPosition));
		}
	}
}
