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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Utils;
using StdColumnDefinition = System.Windows.Controls.ColumnDefinition;
using StdGrid = System.Windows.Controls.Grid;
namespace DevExpress.Xpf.Grid {
	public class ResizableGrid : StdGrid {
#if SILVERLIGHT
		public bool IsLoaded { get; private set; }
#endif
		public BandedViewBehavior BandBehavior;
		public TableView View;
		public ColumnsLayoutControl Owner;
		double CorrectingLeftAreaWidth {
			get { return CorrectingColumn.Width.Value; }
		}
		StdColumnDefinition CorrectingColumn;
		public ResizableGrid() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public void Initialize(ColumnsLayoutControl owner) {
			Owner = owner;
			BandBehavior = Owner.BandBehavior;
			View = (TableView)Owner.View;
		}
		public void PreparePanel() {
			if(ColumnDefinitions.Count != 0 || RowDefinitions.Count != 0) return;
			CreateCorrectingColumn();
			foreach(ColumnDefinition columnDefinition in BandBehavior.ColumnDefinitions)
				ColumnDefinitions.Add(columnDefinition.CreateGridColumnDefinition());
			foreach(RowDefinition rowDefinition in BandBehavior.RowDefinitions)
				RowDefinitions.Add(rowDefinition.CreateGridRowDefinition());
			LayoutUpdated += OnLayoutUpdated;
		}
		public void UpdateColumns() {
			PrepareColumns(new Rows(this));
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			if(!IsLoaded) return;
			LayoutUpdated -= OnLayoutUpdated;
			Rows rows = new Rows(this);
			PrepareColumns(rows);
			CorrectColumnsWidth(rows);
		}
		public void ClearPanel() {
			ColumnDefinitions.Clear();
			RowDefinitions.Clear();
		}
		void CreateCorrectingColumn() {
			CorrectingColumn = new StdColumnDefinition();
			Binding b = new Binding("GroupedColumns.Count");
			b.Source = View;
			b.Converter = new GridLengthValueConverter();
			b.ConverterParameter = ((TableView)View).LeftGroupAreaIndent;
			BindingOperations.SetBinding(CorrectingColumn, System.Windows.Controls.ColumnDefinition.WidthProperty, b);
			ColumnDefinitions.Insert(0, CorrectingColumn);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT
			IsLoaded = true;
#endif
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT
			IsLoaded = false;
#endif
		}
		void PrepareColumns(Rows rows) {
			int counter = 0;
			for(int i = 0; i < rows.Count; i++) {
				Row row = rows[i];
				for(int j = 0; j < row.Count; j++) {
					RowCell rowCell = row[j];
					Canvas.SetZIndex(rowCell.GridColumnControl, counter);
					rowCell.GridColumn.VisibleIndex = counter;
					counter++;
				}
			}
		}
		void CorrectColumnsWidth(Rows rows) {
			for(int i = 0; i < rows.Count; i++) {
				Row row = rows[i];
				for(int j = 0; j < row.Count; j++) {
					RowCell rowCell = row[j];
					rowCell.ColumnWidth = new GridLength(rowCell.ActualWidth / row.RowWidth, GridUnitType.Star);
				}
			}
		}
		public void Resize(ColumnBase gridColumn, double diff) {
			Rows rows = new Rows(this);
			CorrectColumnsWidth(rows);
			if(diff == 0d || double.IsNaN(diff)) {
				return;
			}
			int columnIndex = BandedViewBehavior.GetColumn(gridColumn);
			int rowIndex = BandedViewBehavior.GetRow(gridColumn);
			Row row = rows[rowIndex];
			row.SelectColumn(columnIndex);
			if(diff > 0) {
				DecreaseColumnsWidth(row, row.SelectedIndex + 1, ref diff);
				IncreaseSelectedColumnWidth(row, diff);
			}
			if(diff < 0) {
				diff *= -1;
				if(Math.Abs(row.SelectedCell.ActualWidth - row.SelectedCell.ColumnMinWidth) <= 0.5)
					return;
				DecreaseSelectedColumnWidth(row, ref diff);
				IncreaseColumnsWidth(row, row.SelectedIndex + 1, diff);
			}
			row.Foreach(row.SelectedIndex, (rowCell) => {
				double res = rowCell.NewWidth / row.RowWidth;
				rowCell.ColumnWidth = new GridLength(res, GridUnitType.Star);
			});
		}
		void IncreaseSelectedColumnWidth(Row row, double diff) {
			row.SelectedCell.NewWidth = row.SelectedCell.ActualWidth + diff;
		}
		void DecreaseSelectedColumnWidth(Row row, ref double diff) {
			if(row.SelectedCell.ActualWidth - diff < row.SelectedCell.ColumnMinWidth) {
				diff = row.SelectedCell.ActualWidth - row.SelectedCell.ColumnMinWidth;
				row.SelectedCell.NewWidth = row.SelectedCell.ColumnMinWidth;
				return;
			}
			row.SelectedCell.NewWidth = row.SelectedCell.ActualWidth - diff;
		}
		void IncreaseColumnsWidth(Row row, int startIndex, double diff) {
			double one = 0d; row.Foreach(startIndex, (rowCell) => { one += rowCell.ColumnWidth.Value; });
			row.Foreach(startIndex, (rowCell) => {
				double coef = rowCell.ColumnWidth.Value / one;
				double cDiff = diff * coef;
				rowCell.NewWidth = rowCell.ActualWidth + cDiff;
			});
		}
		void DecreaseColumnsWidth(Row row, int startIndex, ref double value) {
			double allWidth = 0d;
			double allMinWidth = 0d;
			row.Foreach(startIndex, (rowCell) => {
				allWidth += rowCell.ActualWidth;
				allMinWidth += rowCell.ColumnMinWidth;
			});
			if(allWidth - value < allMinWidth) value = allWidth - allMinWidth;
			row.Foreach(startIndex, (rowCell) => { 
				rowCell.NewWidth = rowCell.ActualWidth; 
			});
			if(value == 0d) return;
			double one = 0d; row.Foreach(startIndex, (rowCell) => { 
				if(rowCell.NewWidth > rowCell.ColumnMinWidth) one += rowCell.ColumnWidth.Value; 
			});
			if(one == 0d) return;
			double one2 = 0d;
			double diff = value;
			double diff2 = 0d;
			int i = 0;
			while(true) {
				i++;
				row.Foreach(startIndex, (rowCell) => {
					double coef = rowCell.ColumnWidth.Value / one;
					double cDiff = diff * coef;
					rowCell.NewWidth = rowCell.NewWidth - cDiff;
					if(rowCell.NewWidth <= rowCell.ColumnMinWidth) {
						rowCell.NewWidth = rowCell.ColumnMinWidth;
						diff2 += rowCell.ColumnMinWidth - rowCell.NewWidth;
						one2 += rowCell.ColumnWidth.Value;
					}
					return;
				});
				if(diff2 == 0d || one2 == 0d) break;
				diff = diff2;
				one -= one2;
				diff2 = 0d;
				one2 = 0d;
			}
		}
		#region Helpers
		class HelperBase {
			protected readonly ResizableGrid Owner;
			public HelperBase(ResizableGrid owner) {
				Owner = owner;
			}
		}
		class RowCell : HelperBase {
			public readonly int Row;
			public readonly int RowSpan;
			public readonly int Column;
			public readonly int ColumnSpan;
			public readonly ColumnBase GridColumn;
			public readonly ColumnDefinitions ColumnDefinitions;
			public readonly ContentControl GridColumnControl;
			public double ActualWidth { 
				get {
					double res = GridColumnControl.ActualWidth;
					if(Column == 0) res -= Owner.CorrectingLeftAreaWidth;
					return res;
				} 
			}
			public GridLength ColumnWidth {
				get { return GetColumnWidth(); }
				set { SetColumnWidth(value); }
			}
			public double ColumnMinWidth {
				get { return GetColumnMinWidth(); }
			}
			public double NewWidth { get; set; }
			public bool NewWidthIsMinWidth { get { return NewWidth == ColumnMinWidth; } }
			public RowCell(ResizableGrid owner, ColumnBase gridColumn, ContentControl contentControl)
				: base(owner) {
				GridColumnControl = contentControl;
				GridColumn = gridColumn;
				Row = BandedViewBehavior.GetRow(GridColumn);
				RowSpan = BandedViewBehavior.GetRowSpan(GridColumn);
				Column = BandedViewBehavior.GetColumn(GridColumn);
				ColumnSpan = BandedViewBehavior.GetColumnSpan(GridColumn);
				ColumnDefinitions = new ColumnDefinitions();
				for(int i = Column; i < Column + ColumnSpan; i++)
					ColumnDefinitions.Add(Owner.BandBehavior.ColumnDefinitions[i]);
			}
			double GetColumnMinWidth() {
				double res = 0d;
				foreach(ColumnDefinition column in ColumnDefinitions) res += column.MinWidth;
				return Math.Max(res, 0d);
			}
			GridLength GetColumnWidth() {
				double res = 0d;
				foreach(ColumnDefinition column in ColumnDefinitions)
					res += column.Width.Value;
				return new GridLength(res, GridUnitType.Star);
			}
			void SetColumnWidth(GridLength value) {
				if(NewWidthIsMinWidth) {
					double w = value.Value / ColumnDefinitions.Count;
					foreach(ColumnDefinition column in ColumnDefinitions)
						column.Width = new GridLength(w, GridUnitType.Star);
					return;
				}
				double oldWidth = ColumnWidth.Value;
				double newWidth = value.Value;
				foreach(ColumnDefinition column in ColumnDefinitions) {
					double oldColumnWidth = column.Width.Value;
					double newColumnWidth = newWidth * oldColumnWidth / oldWidth;
					if(double.IsNaN(newColumnWidth)) newColumnWidth = 0d;
					column.Width = new GridLength(newColumnWidth, GridUnitType.Star);
				}
			}
		}
		class Row : HelperBase {
			public RowCell this[int index] { get { return RowCellList[index]; } }
			public int Count { get { return RowCellList.Count; } }
			public double RowWidth {
				get {
					double res = 0d;
					for(int i = 0; i < Count; i++) res += this[i].ActualWidth;
					return res - Owner.CorrectingLeftAreaWidth;
				}
			}
			public RowCell SelectedCell {
				get { return this[SelectedIndex]; }
			}
			public int SelectedIndex { get; private set; }
			public Row(ResizableGrid owner, int rowIndex)
				: base(owner) {
				RowCellList = new List<RowCell>();
				foreach(ContentControl item in Owner.Children) {
					ColumnBase gridColumn = item.DataContext as ColumnBase;
					int row = BandedViewBehavior.GetRow(gridColumn);
					int rowSpan = BandedViewBehavior.GetRowSpan(gridColumn);
					if(row <= rowIndex && row + rowSpan > rowIndex)
						RowCellList.Add(new RowCell(Owner, gridColumn, item));
					Comparison<RowCell> sortingMethos = (rowCell1, rowCell2) => {
						if(rowCell1.Column == rowCell2.Column) return 0;
						return rowCell1.Column < rowCell2.Column ? -1 : 1;
					};
					RowCellList.Sort(sortingMethos);
				}
			}
			public void SelectColumn(int columnIndex) {
				for(int i = 0; i < Count; i++) {
					if(this[i].Column == columnIndex) {
						SelectedIndex = i;
						return;
					}
				}
			}
			public void Foreach(int startIndex, Action<RowCell> action) {
				for(int i = startIndex; i < Count; i++) action(this[i]);
			}
			public void Foreach(int startIndex, int count, Action<RowCell> action) {
				for(int i = startIndex; i < startIndex + count; i++) action(this[i]);
			}
			readonly List<RowCell> RowCellList;
		}
		class Rows : HelperBase {
			public Row this[int index] { get { return RowList[index]; } }
			public int Count { get { return RowList.Count; } }
			public Rows(ResizableGrid owner)
				: base(owner) {
				RowList = new List<Row>();
				for(int i = 0; i < Owner.BandBehavior.RowDefinitions.Count; i++)
					RowList.Add(new Row(Owner, i));
			}
			void UpdateAllowResizingPropertyForEachRowCell() {
			}
			List<Row> RowList;
		}
		#endregion
	}
}
