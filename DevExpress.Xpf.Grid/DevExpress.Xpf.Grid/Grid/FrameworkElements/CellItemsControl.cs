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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using System.Windows.Input;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class CellItemsControl : CellItemsControlBase {
		protected override FrameworkElement CreateChildCore(GridCellData cellData) {
			return new GridCellContentPresenter(cellData);
		}
		protected override void ValidateElementCore(FrameworkElement element, GridCellData cellData) {
			GridCellContentPresenter cellPresenter = (GridCellContentPresenter)element;
			if(cellPresenter.ShouldSyncProperties)
				cellPresenter.SyncProperties(cellData);
		}
		TableViewBehavior TableViewBehavior { get { return View != null ? View.ViewBehavior as TableViewBehavior : null; } }
		protected override void OnCurrentViewChanged() {
			base.OnCurrentViewChanged();
#if !SL
			if(TableViewBehavior != null)
				TableViewBehavior.OnCellItemsControlLoaded();
#endif
		}
	}
	public class RowItemsControl : CachedItemsControl {
	}
	public class BandedCachedItemsControl : CachedItemsControl, INotifyCurrentViewChanged, ILayoutNotificationHelperOwner {
		LayoutNotificationHelper layoutNotificationHelper;
		public BandedCachedItemsControl() {
			layoutNotificationHelper = new LayoutNotificationHelper(this);
		}
		DataViewBase View { get { return (DataViewBase)GridControl.GetCurrentView(this); } }
		protected override Size MeasureOverride(Size constraint) {
			layoutNotificationHelper.Subscribe();
			return base.MeasureOverride(constraint);
		}
		#region INotifyCurrentViewChanged Members
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			InvalidateMeasure();
		}
		#endregion
		DependencyObject ILayoutNotificationHelperOwner.NotificationManager {
			get { return View != null ? View.DataControl : null; ; }
		}
	}
	public class BandedViewAdditionalRowControl : BandedCachedItemsControl {
	}
	public class BandedViewTotalSummaryControl : BandedCachedItemsControl {
	}
	public class BandedViewGroupSummaryItemsControl : BandedCachedItemsControl {
	}
	public class BandedViewHeaderItemsControl : HeaderItemsControl, ILayoutNotificationHelperOwner {
		LayoutNotificationHelper layoutNotificationHelper;
		public BandedViewHeaderItemsControl() {
			layoutNotificationHelper = new LayoutNotificationHelper(this);
		}
		protected override Size MeasureOverride(Size constraint) {
			layoutNotificationHelper.Subscribe();
			return base.MeasureOverride(constraint);
		}
		protected override bool CanSyncWidth { get { return false; } }
		DataViewBase View { get { return DataControlBase.GetCurrentView(this); } }
		DependencyObject ILayoutNotificationHelperOwner.NotificationManager {
			get { return View != null ? View.DataControl : null; }
		}
	}
	public class HeaderItemsControl : CachedItemsControl {
		protected virtual bool CanSyncWidth { get { return true; } }
		protected override FrameworkElement CreateChild(object item) {
			GridColumnHeader header = new GridColumnHeader() { IsTabStop = false, CanSyncWidth = CanSyncWidth, CanSyncColumnPosition = true, DataContext = null };
			GridViewHitInfoBase.SetHitTestAcceptor(header, new ColumnHeaderTableViewHitTestAcceptor());
			return header;
		}
		protected override void ValidateElement(FrameworkElement element, object item) {
			base.ValidateElement(element, item);
			GridColumnData data = item as GridColumnData;
			if(data == null || data.Column == null || data.Column.View == null)
				return;
			BarManager.SetDXContextMenu(element, data.View.DataControlMenu);
			GridPopupMenu.SetGridMenuType(element, GridMenuType.Column);
			GridColumn.SetVisibleIndex(element, data.Column.VisibleIndex);
			GridColumn.SetHeaderPresenterType(element, HeaderPresenterType.Headers);
			GridColumnHeader.SetGridColumn(element, data.Column);
			element.DataContext = data.Column;
			ColumnBase column = data.Column;
			((Button)element).Command = column.View.GetColumnCommand(column);
		}
	}
	public class TotalSummaryItemsControl : CachedItemsControl {
	}
	public class ColumnItemsControl : CachedItemsControl {
		protected override FrameworkElement CreateChild(object item) {
			object data = null;
			GridColumnBase column = item as GridColumnBase;
			if(column != null && column.View != null)
				data = new GridColumnData(column.View.HeadersData) { Column = column };
			BandBase band = item as BandBase;
			if(band != null)
				data = new BandData() { Column = band };
			if(data != null)
				return new ContentPresenter() { ContentTemplate = ItemTemplate, Content = data };
			return base.CreateChild(item);
		}
	}
	public class GroupPanelColumnItemsControl : ColumnItemsControl {
		public GroupPanelColumnItemsControl() {
			this.SetDefaultStyleKey(typeof(GroupPanelColumnItemsControl));
		}
	}
}
