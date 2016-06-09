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
using System.Windows;
using System.Windows.Data;
using StdColumnDefinition = System.Windows.Controls.ColumnDefinition;
using StdGrid = System.Windows.Controls.Grid;
using System.Windows.Controls;
namespace DevExpress.Xpf.Grid {
	public class CellLayoutControl : CellItemsControl {
		public BandedViewBehavior BandBehavior { get { return BandedViewBehavior.GetBandBehaviour((TableView)View); } }
		StdGrid LayoutPanel { get { return (StdGrid)Panel; } }
		public CellLayoutControl() {
			Loaded += OnLoaded;
		}
		protected override FrameworkElement CreateChildCore(GridCellData cellData) {
			ColumnBase gridColumn = cellData.Column;
			AutoWidthCellContentPresenter presenter = new AutoWidthCellContentPresenter();
			int row = BandedViewBehavior.GetRow(gridColumn);
			int column = BandedViewBehavior.GetColumn(gridColumn) + 1;
			int rowSpan = BandedViewBehavior.GetRowSpan(gridColumn);
			int columnSpan = BandedViewBehavior.GetColumnSpan(gridColumn);
			StdGrid.SetRow(presenter, row);
			StdGrid.SetColumn(presenter, column);
			StdGrid.SetRowSpan(presenter, rowSpan);
			StdGrid.SetColumnSpan(presenter, columnSpan);
			if(BandedViewBehavior.GetIsBand(gridColumn)) presenter.Visibility = Visibility.Collapsed;
			else presenter.Visibility = Visibility.Visible;
			return presenter;
		}
		protected override void ValidateVisualTree() {
			PreparePanel();
			UpdateCells();
			base.ValidateVisualTree();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			ClearPanel();
		}
		void ClearPanel() {
			if(View == null || LayoutPanel == null) return;
			InvalidateMeasure();
			LayoutPanel.ColumnDefinitions.Clear();
			LayoutPanel.RowDefinitions.Clear();
		}
		void PreparePanel() {
			if(View == null || LayoutPanel == null) return;
			if(LayoutPanel.ColumnDefinitions.Count != 0 || LayoutPanel.RowDefinitions.Count != 0) return;
			CreateCorrectingColumn();
			foreach(ColumnDefinition columnDefinition in BandBehavior.ColumnDefinitions)
				LayoutPanel.ColumnDefinitions.Add(columnDefinition.CreateGridColumnDefinition());
			foreach(RowDefinition rowDefinition in BandBehavior.RowDefinitions)
				LayoutPanel.RowDefinitions.Add(rowDefinition.CreateGridRowDefinition());
		}
		void CreateCorrectingColumn() {
			StdColumnDefinition res = new StdColumnDefinition();
			Binding b = new Binding("Level");
			b.Source = DataContext;
			b.Converter = new GridLengthValueConverter();
			b.ConverterParameter = ((TableView)View).LeftGroupAreaIndent;
			BindingOperations.SetBinding(res, StdColumnDefinition.WidthProperty, b);
			LayoutPanel.ColumnDefinitions.Insert(0, res);
			Binding b1 = new Binding("Level");
			b1.Source = DataContext;
			b1.Converter = new ValueConverter();
			b1.ConverterParameter = ((TableView)View).LeftGroupAreaIndent;
			BindingOperations.SetBinding(this, CellLayoutControl.MarginProperty, b1);
		}
		void UpdateCells() {
			if(View == null || LayoutPanel == null) return;
			foreach(AutoWidthCellContentPresenter presenter in LayoutPanel.Children) {
				if(BandedViewBehavior.GetIsBottomColumn(presenter.Column)) {
					presenter.SetBorderThickness(new Thickness(0, 0, 1, 0));
				} else {
					presenter.SetBorderThickness(new Thickness(0, 0, 1, 1));
					presenter.Padding = new Thickness(0, 0, 1, 1);
				}
			}
		}
		class ValueConverter : IValueConverter {
			public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				double res = (int)value * (double)parameter;
				return new Thickness() { Left = -res };
			}
			public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
				throw new NotImplementedException();
			}
		}
	}
	public class AutoWidthCellContentPresenter : GridCellContentPresenter {
		public void SetBorderThickness(Thickness value) {
			Border border = (Border)GetTemplateChild("ContentBorder");
			if(border == null) return;
			border.BorderThickness = value;
		}
		public void SetBorderPadding(Thickness value) {
			Border border = (Border)GetTemplateChild("ContentBorder");
			if(border == null) return;
			border.Padding = value;
		}
		protected override void SyncWidth(GridCellData cellData) { }
		public AutoWidthCellContentPresenter() { }
	}
}
