#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor.InternalDesigner {
	internal class GridViewModificationObserver<T> : ColumnViewModificationObserver<T> where T : GridView {
		public GridViewModificationObserver(T view)
			: base(view) {
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			view.ColumnWidthChanged += view_ColumnWidthChanged;
			view.GroupSummary.CollectionChanged += GroupSummary_CollectionChanged;
			view.GridMenuItemClick += GridListEditorDesigner_GridMenuItemClick;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			view.ColumnWidthChanged -= view_ColumnWidthChanged;
			view.GroupSummary.CollectionChanged -= GroupSummary_CollectionChanged;
			view.GridMenuItemClick -= GridListEditorDesigner_GridMenuItemClick;
		}
		private void view_ColumnWidthChanged(object sender, ColumnEventArgs e) {
			OnViewChanged();
		}
		private void GroupSummary_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnViewChanged();
		}
		private void GridListEditorDesigner_GridMenuItemClick(object sender, GridMenuItemClickEventArgs e) {
			if((e.DXMenuItem.Tag is GridStringId)) {
				GridStringId id = (GridStringId)e.DXMenuItem.Tag;
				switch(id) {
					case GridStringId.MenuColumnBestFitAllColumns:
						view.Layout += view_Layout;
						break;
					case GridStringId.MenuFooterCount:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuFooterMax:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuFooterMin:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuFooterSum:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuFooterAverage:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuFooterNone:
						ObserveColumnSummaryCollectionChanged(e.Column);
						break;
					case GridStringId.MenuColumnClearSorting:
						view.Layout += view_Layout;
						break;
					default:
						break;
				}
			}
		}
		private void ObserveColumnSummaryCollectionChanged(GridColumn column) {
			if(column != null) {
				column.Summary.CollectionChanged -= Summary_CollectionChanged;
				column.Summary.CollectionChanged += Summary_CollectionChanged;
			}
		}
		private void Summary_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			OnViewChanged();
		}
		private void view_Layout(object sender, EventArgs e) {
			((ColumnView)sender).Layout -= view_Layout;
			OnViewChanged();
		}
	}
}
