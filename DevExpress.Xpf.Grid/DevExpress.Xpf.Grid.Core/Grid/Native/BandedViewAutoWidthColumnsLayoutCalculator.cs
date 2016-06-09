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
using System.Windows;
using System.Collections;
namespace DevExpress.Xpf.Grid.Native {
	public class BandedViewAutoWidthColumnsLayoutCalculator : BandedViewColumnsLayoutCalculator {
		public BandedViewAutoWidthColumnsLayoutCalculator(GridViewInfo viewInfo) : base(viewInfo) { }
		protected override void CalcActualLayoutCore(double arrangeWidth, LayoutAssigner layoutAssigner, bool showIndicator, bool needRoundingLastColumn, bool ignoreDetailButtons) {
			StretchColumnsToWidth(BandsLayout.VisibleBands, arrangeWidth, layoutAssigner, needRoundingLastColumn);
		}
		protected override void OnBandResize(BandBase band, double delta) {
			SetDefaultColumnSize(BandsLayout.GetBands(band, true));
			IList rightBands = BandsLayout.GetBands(band, false);
			if(delta > 0)
				IncreaseBandsWidth(rightBands, delta);
			else
				DecreaseBandsWidth(rightBands, delta);
		}
		protected override double CorrectColumnDelta(ColumnBase column, double delta, FixedStyle fixedStyle) {
			delta = base.CorrectColumnDelta(column, delta, fixedStyle);
			if(delta < 0 && !HasSizeableRightColumns(column)) return 0;
			return Math.Min(GetColumnHeaderWidth(column) + delta, GetColumnMaxWidth(column)) - GetColumnHeaderWidth(column);
		}
		protected override double CorrectBandDelta(BandBase band, double delta) {
			delta = base.CorrectBandDelta(band, delta);
			return Math.Min(band.ActualHeaderWidth + delta, GetBandMaxWidth(band)) - band.ActualHeaderWidth;
		}
		bool HasSizeableRightColumns(ColumnBase column) {
			for (int i = column.BandRow.Columns.IndexOf(column) + 1; i < column.BandRow.Columns.Count; i++)
				if(!column.BandRow.Columns[i].FixedWidth)
					return true;
			foreach(BandBase band in BandsLayout.GetBands(column.ParentBand, false))
				foreach(BaseColumn col in GetBandColumns(band, true))
					if (!col.FixedWidth)
						return true;
			return false;
		}
		double GetBandMaxWidth(BandBase band) {
			double width = GetBandsMinWidth(band, false);
			foreach(BandBase subBand in BandsLayout.GetBands(band, true))
				width += subBand.ActualHeaderWidth;
			return GetArrangeWidth(ViewInfo.ColumnsLayoutSize, LayoutAssigner.Default, TableViewBehavior.TableView.ShowIndicator, false) - width;
		}
		double GetColumnMaxWidth(ColumnBase column) {
			double width = GetBandsMinWidth(column.ParentBand, false);
			for(int i = column.BandRow.Columns.IndexOf(column) + 1; i < column.BandRow.Columns.Count; i++)
				width += AutoWidthHelper.GetColumnFixedWidth(column.BandRow.Columns[i], ViewInfo);
			foreach(BandBase subBand in BandsLayout.GetBands(column.ParentBand, true))
				width += subBand.ActualHeaderWidth;
			for(int i = column.BandRow.Columns.IndexOf(column) - 1; i >= 0; i--)
				width += GetColumnHeaderWidth(column.BandRow.Columns[i]);
			return GetArrangeWidth(ViewInfo.ColumnsLayoutSize, LayoutAssigner.Default, TableViewBehavior.TableView.ShowIndicator, false) - width;
		}
		double GetBandsMinWidth(BandBase band, bool isLeft) {
			double minWidth = 0;
			foreach(BandBase subBand in BandsLayout.GetBands(band, isLeft))
				minWidth += GetBandMinWidth(subBand);
			return minWidth;
		}
	}
}
