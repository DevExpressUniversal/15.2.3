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
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using System.Collections;
namespace DevExpress.Xpf.Grid {
	public class GridTableViewLayoutCalculatorFactory {
		public virtual ColumnsLayoutCalculator CreateCalculator(GridViewInfo viewInfo, bool autoWidth) {
			if(viewInfo.GridView.DataControl.BandsLayoutCore != null)
				if(autoWidth)
					return new BandedViewAutoWidthColumnsLayoutCalculator(viewInfo);
				else
					return new BandedViewColumnsLayoutCalculator(viewInfo);
			return autoWidth ? new AutoWidthColumnsLayoutCalculator(viewInfo) : new ColumnsLayoutCalculator(viewInfo);
		}
	}
	public class GridViewInfo {
		public const double DefaultColumnWidth = 120;
		public const double DefaultHeaderHeight = 20;
		public const double DefaultRowHeight = 18;
		public const double FixedNoneMinWidth = 1.0d;
		DataViewBase gridView;
		Size columnsLayoutSize = default(Size);
		double verticalScrollBarWidth;
		public GridViewInfo(DataViewBase gridView) {
			this.gridView = gridView;
			LayoutCalculatorFactory = new GridTableViewLayoutCalculatorFactory();
		}
		public Size ColumnsLayoutSize {
			get { return columnsLayoutSize; }
			set {
				if(columnsLayoutSize == value)
					return;
				columnsLayoutSize = value;
				GridView.RebuildVisibleColumns();
			}
		}
		public double VerticalScrollBarWidth {
			get { return verticalScrollBarWidth; }
			set {
				if(verticalScrollBarWidth == value)
					return;
				verticalScrollBarWidth = value;
				GridView.RebuildVisibleColumns();
			}
		}
		public DataViewBase GridView { get { return gridView; } }
		public ITableView TableView { get { return (ITableView)GridView; } }
		public DataControlBase Grid { get { return GridView.DataControl; } }
		public virtual IList<ColumnBase> VisibleColumns { get { return GridView.VisibleColumnsCore; } }
		public virtual int GroupCount { get { return Grid != null ? Grid.ActualGroupCountCore : 0; } }
		public virtual double TotalGroupAreaIndent { get { return TableView.LeftGroupAreaIndent * GroupCount; } }
		public virtual double RightGroupAreaIndent { get { return GroupCount != 0 ? TableView.RightGroupAreaIndent : 0; } }
		public GridTableViewLayoutCalculatorFactory LayoutCalculatorFactory { get; set; }
		public virtual BandsLayoutBase BandsLayout { get { return GridView.DataControl.BandsLayoutCore; } }
		public double GetHeaderIndentsWidth(BaseColumn column) {
			return gridView.ViewBehavior.IsFirstColumn(column) ? FirstColumnIndent : 0;
		}
		protected internal ColumnsLayoutCalculator CreateColumnsLayoutCalculator() {
			return CreateColumnsLayoutCalculator(TableView.AutoWidth);
		}
		protected internal virtual ColumnsLayoutCalculator CreateColumnsLayoutCalculator(bool autoWidth) {
			return LayoutCalculatorFactory.CreateCalculator(this, autoWidth);
		}
		public void CalcColumnsLayout() {
			CreateColumnsLayoutCalculator().CalcActualLayout(ColumnsLayoutSize);
		}
		public double GetColumnHeaderWidth(BaseColumn column) {
			return GetColumnDataWidth(column) + GetHeaderIndentsWidth(column);
		}
		public double GetColumnDataWidth(BaseColumn column) {
			return GetColumnWidthCore(column, c => Math.Max(c.ActualWidth, c.MinWidth));
		}
		public double GetDesiredColumnWidth(BaseColumn column) {
			return GetColumnWidthCore(column, c => GetColumnHeaderWidth(c));
		}
		protected double GetColumnWidthCore(BaseColumn column, Func<BaseColumn, double> method) {
			if(column == null) return DefaultColumnWidth;
			double width = method(column);
			return double.IsNaN(width) ? Math.Max(column.MinWidth, DefaultColumnWidth) : width;
		}
		public double GetDesiredColumnsWidth(IEnumerable columns) {
			double total = 0;
			foreach(BaseColumn column in columns)
				total += GetColumnWidthCore(column, c => GetColumnHeaderWidth(c));
			return total;
		}
		public double GetColumnFixedWidthCore(BaseColumn column) {
			return column.MinWidth + GetHeaderIndentsWidth(column);
		}
		internal double FirstColumnIndent { get { return (TableView.ShowIndicator ? 0 : TableView.LeftDataAreaIndent) + NewItemRowIndent; } }
		internal virtual double NewItemRowIndent { get { return TableView.ActualShowDetailButtons ? 0 : TotalGroupAreaIndent; } }
	}
	public class GridPrintViewInfo : GridViewInfo {
		BandsLayoutBase bandsLayoutCore;
		public override BandsLayoutBase BandsLayout { get { return bandsLayoutCore; } }
		readonly List<ColumnBase> visibleColumns;
		public override IList<ColumnBase> VisibleColumns { get { return visibleColumns; } }
		internal override double NewItemRowIndent { get { return TotalGroupAreaIndent; } }
		public GridPrintViewInfo(DataViewBase view, BandsLayoutBase bandsLayout) : base(view) {
			visibleColumns = view.PrintableColumns.ToList();
			bandsLayoutCore = bandsLayout;
		}
	}
}
