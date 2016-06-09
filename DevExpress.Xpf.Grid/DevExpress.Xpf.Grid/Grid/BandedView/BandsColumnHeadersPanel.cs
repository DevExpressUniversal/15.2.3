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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using System.ComponentModel;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class BandsNoneDropPanel : BandsColumnHeadersPanel {
		protected internal override ColumnBase GetColumn(FrameworkElement element) {
			return ((GridColumnData)((ContentPresenter)element).Content).ColumnCore;
		}
	}
	public class BandsGroupSummaryAlignByColumnsPanel : BandsNoneDropPanel {
		public static readonly DependencyProperty FixedProperty = DependencyProperty.Register("Fixed", typeof(FixedStyle), typeof(BandsGroupSummaryAlignByColumnsPanel), new FrameworkPropertyMetadata(FixedStyle.None, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty LeftMarginProperty = DependencyProperty.Register("LeftMargin", typeof(double), typeof(BandsGroupSummaryAlignByColumnsPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public FixedStyle Fixed {
			get { return (FixedStyle)GetValue(FixedProperty); }
			set { SetValue(FixedProperty, value); }
		}
		public double LeftMargin {
			get { return (double)GetValue(LeftMarginProperty); }
			set { SetValue(LeftMarginProperty, value); }
		}
		double leftIndent;
		TableView View { get { return ((GroupRowData)DataContext).View as TableView; } }
		protected override void ArrangeChild(UIElement child, double x, double y, double width, double height) {
			bool hasOverlap = HasOverlap(x);
			base.ArrangeChild(child, CalcX(x, hasOverlap), y, CalcWidth(width, x, hasOverlap), height);
		}
		protected override void MeasureChild(UIElement child, double availableWidth, double availableHeight, double x) {
			base.MeasureChild(child, CalcWidth(availableWidth, x, HasOverlap(x)), availableHeight, x);
		}
		bool HasOverlap(double x) {
			return x < leftIndent;
		}
		double CalcX(double x, bool hasOverlap) {
			return hasOverlap ? leftIndent : x;
		}
		double CalcWidth(double width, double x, bool hasOverlap) {
			return hasOverlap ? Math.Max(0d, width - leftIndent + x) : width;
		}
		protected override Size MeasureOverride(Size availableSize) {
			UpdateLeftIndent();
			return base.MeasureOverride(availableSize);
		}
		void UpdateLeftIndent() {
			double offset = 0d;
			if(Fixed == FixedStyle.None || Fixed == FixedStyle.Right) {
				if(View.FixedLeftVisibleColumns != null && View.FixedLeftVisibleColumns.Count > 0)
					offset += View.FixedLeftContentWidth + View.TotalGroupAreaIndent + View.FixedLineWidth;
				if(Fixed == FixedStyle.None)
					offset += View.ScrollingHeaderVirtualizationMargin.Left;
				else 
					offset += View.FixedNoneContentWidth + View.FixedLineWidth;
			}
			leftIndent = Math.Max(0, -LeftMargin - offset);
		}
	}
	public class BandsCellsPanel : BandsColumnHeadersPanel {
		public static readonly DependencyProperty FixedNoneContentWidthProperty;
		static BandsCellsPanel() {
			Type ownerType = typeof(BandsCellsPanel);
			FixedNoneContentWidthProperty = DependencyPropertyManager.Register("FixedNoneContentWidth", typeof(double), ownerType, new PropertyMetadata(double.NaN, (d, e) => ((BandsCellsPanel)d).OnFixedNoneContentWidthChanged()));
		}
		public double FixedNoneContentWidth {
			get { return (double)GetValue(FixedNoneContentWidthProperty); }
			set { SetValue(FixedNoneContentWidthProperty, value); }
		}
		protected internal override ColumnBase GetColumn(FrameworkElement element) {
			return ((IGridCellEditorOwner)element).AssociatedColumn;
		}
		internal new RowData RowData { get { return DataContext as RowData; } }
		protected internal override double GetColumnWidth(ColumnBase column) {
			return Math.Max(0, column.ActualDataWidth + RowData.GetRowIndent(column));
		}
		protected internal override double GetBandWidth(BandBase band) {
			if(!band.HasLeftSibling)
				return Math.Max(0, band.ActualHeaderWidth - ((ITableView)RowData.View).TableViewBehavior.ViewInfo.FirstColumnIndent + RowData.GetRowIndent(true));
			return band.ActualHeaderWidth;
		}
		void OnFixedNoneContentWidthChanged() {
			InvalidateMeasure();
		}
	}
	public class BandsColumnHeadersPanel : Panel {
		public static readonly DependencyProperty BandsProperty;
		static BandsColumnHeadersPanel() {
			Type ownerType = typeof(BandsColumnHeadersPanel);
			BandsProperty = DependencyPropertyManager.Register("Bands", typeof(IList<BandBase>), ownerType, new PropertyMetadata(null, (d, e) => ((BandsColumnHeadersPanel)d).InvalidateMeasure()));
		}
		public IList<BandBase> Bands {
			get { return (IList<BandBase>)GetValue(BandsProperty); }
			set { SetValue(BandsProperty, value); }
		}
		BandLayoutStrategyBase GetLayoutStrategy(BandBase band) {
			if(band.ColumnDefinitions.Count > 0 || band.RowDefinitions.Count > 0) {
				return GridBandLayoutStrategy.Instance;
			}
			return StackBandLayoutStrategy.Instance;
		}
		protected override Size MeasureOverride(Size availableSize) {
			double width = 0;
			double height = 0;
			double x = 0;
			BandsLayoutBase.ForeachVisibleBand(Bands, (band) => {
				Size size = GetLayoutStrategy(band).MeasureElements(band, availableSize, x, this);
				if(band.VisibleBands.Count == 0)
					x += GetBandWidth(band);
				width += size.Width;
				height = Math.Max(height, size.Height);
			});
			foreach(UIElement element in Children) {
				if(OrderPanelBase.GetVisibleIndex(element) < 0) {
					element.Visibility = Visibility.Collapsed;
					element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				}
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double width = 0;
			BandsLayoutBase.ForeachVisibleBand(Bands, (band) => {
				width = GetLayoutStrategy(band).ArrangeElements(band, finalSize, width, this);
			});
			return finalSize;
		}
		protected internal virtual ColumnBase GetColumn(FrameworkElement element) {
			return element.DataContext as ColumnBase;
		}
		protected internal virtual double GetColumnWidth(ColumnBase column) {
			return column.ActualHeaderWidth;
		}
		protected internal virtual double GetBandWidth(BandBase band) {
			return band.ActualHeaderWidth;
		}
		protected virtual void ArrangeChild(UIElement child, double x, double y, double width, double height) {
			child.Arrange(new Rect(x, y, width, height));
		}
		protected virtual void MeasureChild(UIElement child, double availableWidth, double availableHeight, double x) {
			child.Measure(new Size(availableWidth, availableHeight));
		}
		internal ColumnsRowDataBase RowData { get { return DataContext as ColumnsRowDataBase; } }
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
	#region inner classes
		abstract class BandLayoutStrategyBase {
			public abstract Size MeasureElements(BandBase band, Size availableSize, double width, BandsColumnHeadersPanel panel);
			public abstract double ArrangeElements(BandBase band, Size finalSize, double width, BandsColumnHeadersPanel panel);
		}
		class StackBandLayoutStrategy : BandLayoutStrategyBase {
			private StackBandLayoutStrategy() { }
			public static readonly BandLayoutStrategyBase Instance = new StackBandLayoutStrategy();
			public override Size MeasureElements(BandBase band, Size availableSize, double width, BandsColumnHeadersPanel panel) {
				double totalHeight = 0;
				double maxWidth = 0;
				Dictionary<ColumnBase, FrameworkElement> bandElements = GetBandElements(band, panel);
				if(band.ActualRows.Count != 0) {
					foreach(BandRow row in band.ActualRows) {
						double maxHeight = 0;
						double rowWidth = 0;
						double x = width;
						foreach(ColumnBase column in row.Columns) {
							if(bandElements.ContainsKey(column)) {
								FrameworkElement child = bandElements[column];
								child.Visibility = Visibility.Visible;
								panel.MeasureChild(child, panel.GetColumnWidth(column), Math.Max(0, availableSize.Height - totalHeight), x);
								maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
								rowWidth += panel.GetColumnWidth(column);
							}
							x += panel.GetColumnWidth(column);
						}
						totalHeight += maxHeight;
						maxWidth = Math.Max(maxWidth, rowWidth);
					}
				} else {
					maxWidth = panel.GetBandWidth(band);
				}
				return new Size(band.VisibleBands.Count == 0 ? maxWidth : 0, totalHeight);
			}
			public override double ArrangeElements(BandBase band, Size finalSize, double width, BandsColumnHeadersPanel panel) {
				Dictionary<ColumnBase, FrameworkElement> bandElements = GetBandElements(band, panel);
				double height = 0;
				double maxWidth = 0;
				if(band.ActualRows.Count != 0) {
					for(int i = 0; i < band.ActualRows.Count; i++) {
						double currentWidth = width;
						double maxHeight = 0;
						if(i == band.ActualRows.Count - 1) {
							maxHeight = finalSize.Height - height;
						}
						else {
							foreach(ColumnBase column in band.ActualRows[i].Columns) {
								if(bandElements.ContainsKey(column)) {
									maxHeight = Math.Max(bandElements[column].DesiredSize.Height, maxHeight);
								}
							}
						}
						for(int j = 0; j < band.ActualRows[i].Columns.Count; j++) {
							ColumnBase column = band.ActualRows[i].Columns[j];
							if(bandElements.ContainsKey(column)) {
								FrameworkElement child = bandElements[column];
								panel.ArrangeChild(child, currentWidth, height, panel.GetColumnWidth(column), maxHeight);
							}
							currentWidth += panel.GetColumnWidth(column);
						}
						height += maxHeight;
						maxWidth = Math.Max(maxWidth, currentWidth);
					}
				} else {
					maxWidth = panel.GetBandWidth(band) + width;
				}
				return band.VisibleBands.Count == 0 ? maxWidth : width;
			}
			Dictionary<ColumnBase, FrameworkElement> GetBandElements(BandBase band, BandsColumnHeadersPanel panel) {
				Dictionary<ColumnBase, FrameworkElement> bandElements = new Dictionary<ColumnBase, FrameworkElement>();
				foreach(FrameworkElement element in panel.Children) {
					ColumnBase column = panel.GetColumn(element);
					if(column != null)
						bandElements[column] = element;
				}
				return bandElements;
			}
		}
		class GridBandLayoutStrategy : BandLayoutStrategyBase {
			private GridBandLayoutStrategy() { }
			public static readonly BandLayoutStrategyBase Instance = new GridBandLayoutStrategy();
			public override Size MeasureElements(BandBase band, Size availableSize, double width, BandsColumnHeadersPanel panel) {
				double rowsHeight = 0;
				double rowsWidth = 0;
				Dictionary<GridColumn, UIElement> children = GetBandElements(band, panel);
				foreach(GridColumn column in children.Keys) {
					UIElement child = children[column];
					panel.MeasureChild(child, double.PositiveInfinity, double.PositiveInfinity, rowsWidth + width);
					rowsHeight = Math.Max(rowsHeight, child.DesiredSize.Height);
					rowsWidth += child.DesiredSize.Width;
				}
				if(band.RowDefinitions.Count != 0) {
					rowsHeight = 0;
					foreach(BandRowDefinition rowDefinition in band.RowDefinitions)
						rowsHeight += rowDefinition.Height.Value;
				}
				if(band.ColumnDefinitions.Count != 0) {
					rowsWidth = 0;
					foreach(BandColumnDefinition columnDefinition in band.ColumnDefinitions)
						rowsWidth += columnDefinition.Width.Value;
				}
				return new Size(rowsWidth, rowsHeight);
			}
			public override double ArrangeElements(BandBase band, Size finalSize, double width, BandsColumnHeadersPanel panel) {
				Dictionary<GridColumn, UIElement> children = GetBandElements(band, panel);
				foreach(GridColumn column in children.Keys) {
					UIElement child = children[column];
					double offsetY = 0;
					for(int i = 0; i < BandBase.GetGridRow(column); i++) {
						offsetY += band.RowDefinitions[i].Height.Value;
					}
					double offsetX = 0;
					for(int i = 0; i < BandBase.GetGridColumn(column); i++) {
						offsetX += band.ColumnDefinitions[i].Width.Value;
					}
					panel.ArrangeChild(child, width + offsetX, offsetY, GetColumnWidth(BandBase.GetGridColumn(column), band), GetRowHeight(BandBase.GetGridRow(column), band, finalSize.Height));
				}
				double bandWidth = 0;
				foreach(BandColumnDefinition columnDefinition in band.ColumnDefinitions)
					bandWidth += columnDefinition.Width.Value;
				return width + bandWidth;
			}
			double GetColumnWidth(int i, BandBase band) {
				if(i < 0 || band.ColumnDefinitions.Count < i + 1) return GridViewInfo.DefaultColumnWidth;
				return band.ColumnDefinitions[i].Width.Value;
			}
			double GetRowHeight(int i, BandBase band, double maxHeight) {
				if(i < 0 || band.RowDefinitions.Count < i + 1) return maxHeight;
				return band.RowDefinitions[i].Height.Value;
			}
			Dictionary<GridColumn, UIElement> GetBandElements(BandBase band, BandsColumnHeadersPanel panel) {
				Dictionary<GridColumn, UIElement> elements = new Dictionary<GridColumn, UIElement>();
				if(band.VisibleBands.Count == 0) {
					foreach(FrameworkElement element in panel.Children) {
						GridColumn column = element.DataContext as GridColumn;
						if(band.ColumnsCore.Contains(column))
							elements[column] = element;
					}
				}
				return elements;
			}
		}
	#endregion
	}
}
