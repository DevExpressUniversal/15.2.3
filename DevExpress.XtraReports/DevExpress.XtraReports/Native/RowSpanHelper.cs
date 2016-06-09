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

using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Native {
	public static class RowSpanHelper {
		public static void MergeBricks(VisualBrick brick, Dictionary<XRTableCell, object> mergedCells) {
			BrickCollectionBase rows = ((TableBrick)brick).Bricks;
			for(int i = 0; i < rows.Count; i++) {
				BrickCollectionBase currentRowBricks = rows[i].Bricks as BrickCollectionBase;
				for(int j = 0; j < currentRowBricks.Count; j++) {
					VisualBrick currentBrick = currentRowBricks[j] as VisualBrick;
					XRTableCell cell = currentBrick.BrickOwner as XRTableCell;
					if(mergedCells.ContainsKey(cell)) {
						if(cell.RowSpan > 1) {
							currentBrick.Height = GraphicsUnitConverter.Convert(CalculateMergedCellHeight(cell, mergedCells), cell.Dpi, GraphicsDpi.Document);
						} else {
							currentBrick.IsVisible = false;
						}
					}
				}
			}
		}
		public static void MergeCells(XRTableCell cell, Dictionary<XRTableCell, object> mergedCells) {
			if(cell.RowSpan > 1 && !cell.HasChildren && cell.ProcessDuplicatesMode == ProcessDuplicatesMode.Leave) {
				MultiKey key = new MultiKey(Guid.NewGuid());
				AppendMergedCellsDictionary(cell, key, mergedCells);
				mergedCells.Add(cell, key);
			}
		}
		static void AppendMergedCellsDictionary(XRTableCell mainCell, object key, Dictionary<XRTableCell, object> mergedCells) {
			XRTable table = mainCell.Row.Table;
			for(int i = mainCell.Row.Index + 1; i < Math.Min(mainCell.Row.Index + mainCell.RowSpan, table.Rows.Count); i++) {
				XRTableRow row = table.Rows[i];
				for(int j = 0; j < row.Cells.Count; j++) {
					XRTableCell cell = row.Cells[j];
					if(RowSpanHelper.CellIsOverlapped(mainCell.LeftF, mainCell.RightF, cell.LeftF, cell.RightF)) return;
					if(FloatsComparer.Default.FirstEqualsSecond(mainCell.LeftF, cell.LeftF) && FloatsComparer.Default.FirstEqualsSecond(mainCell.RightF, cell.RightF)) {
						if(cell.RowSpan > 1 || cell.HasChildren) return;
						cell.Visible = mainCell.Visible;
						cell.CanShrink = mainCell.CanShrink;
						cell.CanGrow = mainCell.CanGrow;
						mergedCells.Add(cell, key);
					}
				}
			}
		}
		static bool CellIsOverlapped(float mainCellL, float mainCellR, float cellL, float cellR) {
			return ((FloatsComparer.Default.FirstGreaterSecondLessThird(cellR, mainCellL, mainCellR)) ||
			  (FloatsComparer.Default.FirstGreaterSecondLessThird(mainCellR, cellL, cellR)) ||
			  (FloatsComparer.Default.FirstGreaterSecondLessThird(mainCellL, cellL, cellR))) ? true : false;
		}
		public static float CalculateMergedCellHeight(XRTableCell cell, Dictionary<XRTableCell, object> mergedCells) {
			object key;
			float height = 0;
			mergedCells.TryGetValue(cell, out key);
			IEnumerable selection = mergedCells.Where(x => x.Value == key).ToList();
			foreach(KeyValuePair<XRTableCell, object> pair in selection) {
				height += pair.Key.HeightF;
			}
			return height;
		}
		public static RectangleF GetCellBounds(XRTableCell cell, Dictionary<XRTableCell, object> mergedCells, Graphics gr) {
			RectangleF result = cell.GetClipppedBandBounds(gr.PageUnit);
			if(cell.RowSpan > 1) {
				result.Height = GraphicsUnitConverter.Convert(CalculateMergedCellHeight(cell, mergedCells), cell.Dpi, GraphicsDpi.Document);
			}
			return result;
		}
		public static BorderSide GetCellBorders(XRTableCell cell, Dictionary<XRTableCell, object> mergedCells) {
			if(!(mergedCells.ContainsKey(cell))) return cell.VisibleContourBorders;
			if(cell.RowSpan < 2) return new BorderSide();
			object key;
			mergedCells.TryGetValue(cell, out key);
			var cells = mergedCells.Where(x => x.Value == key).ToList();
			cells.Sort((x, y) => { return Comparer<double>.Default.Compare(x.Key.Row.BoundsF.Top, y.Key.Row.BoundsF.Top); });
			XRTableCell lastCell  = cells.Last().Key;
			return lastCell.VisibleContourBorders;
		}
	}
}
