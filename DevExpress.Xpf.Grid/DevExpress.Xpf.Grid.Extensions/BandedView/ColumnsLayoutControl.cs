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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils;
using StdGrid = System.Windows.Controls.Grid;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.HitTest;
using System;
namespace DevExpress.Xpf.Grid {
	public class ColumnsLayoutControl : HeaderItemsControl {
		public static readonly DependencyProperty ViewProperty =
			DependencyProperty.Register("View", typeof(DataViewBase), typeof(ColumnsLayoutControl), null);
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public BandedViewBehavior BandBehavior { get { return BandedViewBehavior.GetBandBehaviour((TableView)View); } }
		public ResizableGrid LayoutPanel { get { return (ResizableGrid)Panel; } }
		public ColumnsLayoutControl() {
			Loaded += OnLoaded;
		}
		public void Resize(ColumnBase column, double value) {
			BandGridColumnHeader columnHeader = null;
			foreach(BandGridColumnHeader ch in LayoutPanel.Children) {
				if((ColumnBase)ch.DataContext == column)
					columnHeader = ch;
			}
			double diff = value - columnHeader.ActualWidth;
			LayoutPanel.Resize(column, diff);
		}
		protected override FrameworkElement CreateChild(object item) {
			BandGridColumnHeader child = new BandGridColumnHeader() { IsTabStop = false, CanSyncWidth = false, DataContext = null };
			GridViewHitInfoBase.SetHitTestAcceptor(child, new ColumnHeaderTableViewHitTestAcceptor());
			ColumnBase column = ((GridColumnData)item).Column;
			BandBehavior.UpdateColumnHeaderTemplate(column);
			BandedViewBehavior.SetColumnsLayoutControl(column, this);
			BandedViewBehavior.UpdateColumnPosition(BandBehavior, column);
			PrepareChild(child, column);
			return child;
		}
		protected override void ValidateVisualTree() {
			base.ValidateVisualTree();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			ClearPanel();
			PreparePanel();
		}
		void PreparePanel() {
			if(View == null || LayoutPanel == null) return;
			if(LayoutPanel.ColumnDefinitions.Count != 0 || LayoutPanel.RowDefinitions.Count != 0) return;
			LayoutPanel.ClearPanel();
		}
		void ClearPanel() {
			if(View == null || LayoutPanel == null) return;
			InvalidateMeasure();
			LayoutPanel.Initialize(this);
			LayoutPanel.PreparePanel();
		}
		void PrepareChild(BandGridColumnHeader child, ColumnBase column) {
			int columnCorrectingCoef = BandedViewBehavior.GetIsLeftColumn(column) ? 0 : 1;
			int columnSpanCorrectingCoef = BandedViewBehavior.GetIsLeftColumn(column) ? 1 : 0;
			StdGrid.SetRow(child, BandedViewBehavior.GetRow(column));
			StdGrid.SetColumn(child, BandedViewBehavior.GetColumn(column) + columnCorrectingCoef);
			StdGrid.SetRowSpan(child, BandedViewBehavior.GetRowSpan(column));
			StdGrid.SetColumnSpan(child, BandedViewBehavior.GetColumnSpan(column) + columnSpanCorrectingCoef);
		}
	}
	public class BandGridColumnHeader : GridColumnHeader {
		ColumnBase GridColumn { get { return DataContext as ColumnBase; } }
		TableView View { get { return (TableView)GridColumn.View; } }
		BandedViewBehavior BandBehavior { get { return BandedViewBehavior.GetBandBehaviour(View); } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(GridColumn == null) return;
			Margin = BandBehavior.GetColumnHeaderMargin(GridColumn);
			if(BandedViewBehavior.GetIsBand(GridColumn)) BarManager.SetDXContextMenu(this, null);
			else BarManager.SetDXContextMenu(this, View.DataControlMenu);
		}
	}
	public class BandColumnHeaderPanelDropTargetFactory : ColumnHeaderDropTargetFactory {
		protected override IDropTarget CreateDropTargetCore(Panel panel) {
			return new BandColumnHeaderPanelDropTarget();
		}
	}
	[Obsolete("Instead use the BandColumnHeaderPanelDropTarget class.")]
	public class BandColumnHeaderPanelDropTargetFactoryExtension : BandColumnHeaderPanelDropTargetFactory { }
	public class BandColumnHeaderPanelDropTarget : RemoveColumnDropTarget {
		GridColumnHeader GridColumnHeader;
		GridColumn Column;
		ColumnsLayoutControl ColumnsLayoutControl;
		UIElement SourceElement;
		TableView View;
		void Init(UIElement source) {
			if(source is GridColumnHeader) {
				GridColumnHeader = ((GridColumnHeader)source);
				ISupportDragDrop iSupportDragDrop = GridColumnHeader;
				SourceElement = iSupportDragDrop.SourceElement;
				Column = (GridColumn)GridColumnHeader.GetGridColumn(GridColumnHeader);
				ColumnsLayoutControl = BandedViewBehavior.GetColumnsLayoutControl(Column);
				View = (TableView)Column.View;
			}
		}
		public override void OnDragLeave() {
			if(SourceElement is BandGridColumnHeader) return;
			base.OnDragLeave();
		}
		public override void OnDragOver(UIElement source, Point pt) {
			Init(source);
			if(SourceElement is BandGridColumnHeader) return;
			base.OnDragOver(source, pt);
		}
		public override void Drop(UIElement source, Point pt) {
			Init(source);
			base.Drop(source, pt);
			Column.Visible = true;
		}
		protected override bool IsPositionInDropZone(UIElement source, Point pt) {
			GetDataView(source);
			if(SourceElement is BandGridColumnHeader) return false;
			return true;
		}
	}
}
