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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public class VerticalGrowthStrategy :GrowthStrategy {
		public VerticalGrowthStrategy(Worksheet templateSheet) :base(templateSheet){
		}
		protected override void CopyStaticRangeFormatting(Worksheet targetSheet) {
			CopySheetColumnWidths(TemplateSheet.GetPrintRange(), targetSheet);
		}
		protected override void CopyChangedRangeFormatting(CellRange sourceRange, CellRange targetRange) {
			CopyRowsHeights(sourceRange, targetRange);
		}
		protected override void SetWatchingPosition(CellPosition position, int offset) {
			CurrentWatchingPosition = new CellPosition(position.Column, position.Row + offset);
		}
		protected override CellPosition GetTargetPosition(CellPosition position, CellRange sourceRange, CellRange targetRange) {
			return new CellPosition(position.Column, position.Row + targetRange.TopLeft.Row - sourceRange.TopLeft.Row);
		}
		internal protected override CellRange AppendRange(Worksheet targetSheet, CellRange sourceRange) {
			if (sourceRange == null)
				return null;
			CellPosition topLeft = new CellPosition(sourceRange.TopLeft.Column, End);
			CellPosition bottomRight = new CellPosition(sourceRange.BottomRight.Column, End + sourceRange.Height - 1);
			CellRange targetRange = new CellRange(targetSheet, topLeft, bottomRight);
			CopyRange(sourceRange, targetRange, targetSheet);
			End += sourceRange.Height;
			return targetRange;
		}
		internal protected override int GetPositionCriterion(CellPosition position) {
			return position.Row;
		}
		internal protected override CellRange GetSubrangeByCriteria(CellRange range, int topLeft, int bottomRight) {
			return new CellRange(range.Worksheet, new CellPosition(range.TopLeft.Column, topLeft, range.TopLeft.ColumnType, range.TopLeft.RowType), new CellPosition(range.BottomRight.Column, bottomRight, range.BottomRight.ColumnType, range.BottomRight.RowType));
		}
		protected internal override CellRange ExcludeGroupHeaderFooterRanges(List<GroupInfo> groupInfo, CellRange detailRange) {
			CellRange sourceRange = detailRange;
			foreach (GroupInfo info in groupInfo) {
				if (info.Header != null)
					sourceRange = new CellRange(sourceRange.Worksheet, new CellPosition(sourceRange.TopLeft.Column, info.Header.BottomRight.Row + 1), sourceRange.BottomRight);
				if (info.Footer != null)
					sourceRange = new CellRange(sourceRange.Worksheet, sourceRange.TopLeft, new CellPosition(sourceRange.BottomRight.Column, info.Footer.TopLeft.Row - 1));
			}
			return sourceRange;
		}
	}
}
