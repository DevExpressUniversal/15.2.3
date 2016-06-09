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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data;
namespace DevExpress.ExpressApp.Win.SystemModule {
	#region Obsolete 15.2
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class GridViewKeyboardController : ViewController, IModelExtender {
		private GridColumn currentColumn = null;
		private BarShortcut filterShortcut;
		private BarShortcut groupShortcut;
		private BarShortcut sortShortcut;
		private BarShortcut navigateBackShortcut;
		private BarShortcut navigateForwardShortcut;
		private void GridViewKeyboardController_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e) {
			if(e.Column == currentColumn) e.Info.State = ObjectState.Hot;
		}
		private void Group(GridView gridView) {
			if(gridView.OptionsView.ShowGroupPanel == true) {
				if(currentColumn != null && currentColumn.GroupIndex == -1) {
					currentColumn.Group();
				}
				else {
					if(currentColumn != null) {
						currentColumn.UnGroup();
					}
				}
			}
		}
		private void ChangeSort(GridColumn column) {
			foreach(GridColumn gridColumn in column.View.Columns) {
				if(gridColumn != column) {
					gridColumn.SortOrder = ColumnSortOrder.None;
				}
			}
			if(column.SortOrder == ColumnSortOrder.Ascending) {
				column.SortOrder = ColumnSortOrder.Descending;
			}
			else {
				if(column.SortOrder == ColumnSortOrder.Descending) {
					if(column.GroupIndex == -1) {
						column.SortOrder = ColumnSortOrder.None;
					}
					else {
						column.SortOrder = ColumnSortOrder.Ascending;
					}
				}
				else {
					column.SortOrder = ColumnSortOrder.Ascending;
				}
			}
		}
		private void Sort() {
			if(currentColumn != null) {
				ChangeSort(currentColumn);
			}
		}
		private void Navigate(GridView gridView, bool back) {
			if(currentColumn == null) {
				currentColumn = gridView.FocusedColumn;
			}
			if(currentColumn != null) {
				ArrayList columns = new ArrayList();
				columns.AddRange(gridView.GroupedColumns);
				columns.AddRange(gridView.VisibleColumns);
				int index = currentColumn.GroupIndex > -1 ? currentColumn.GroupIndex : gridView.GroupedColumns.Count + currentColumn.VisibleIndex;
				int nextIndex = index + (back ? -1 : 1);
				if(nextIndex < 0) {
					nextIndex = columns.Count - 1;
				}
				if(nextIndex >= columns.Count) {
					nextIndex = 0;
				}
				currentColumn = (GridColumn)columns[nextIndex];
				gridView.GridControl.Invalidate();
			}
		}
		private void Init() {
			IModelListViewShortcuts listViewShortcuts = (IModelListViewShortcuts)Application.Model.Options;
			this.filterShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.FilterShortcut);
			this.groupShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.GroupShortcut);
			this.sortShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.SortShortcut);
			this.navigateBackShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.NavigateBackShortcut);
			this.navigateForwardShortcut = ShortcutHelper.ParseBarShortcut(listViewShortcuts.NavigateForwardShortcut);
		}
		private GridListEditor GetGridListEditor() {
			ListView listView = (ListView)View;
			GridListEditor gridListEditor = listView.Editor as GridListEditor;
			return gridListEditor;
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			GridListEditor gridListEditor = GetGridListEditor();
			if(gridListEditor != null) {
				gridListEditor.Grid.KeyDown += new KeyEventHandler(GridViewKeyboardController_KeyDown);
				gridListEditor.GridView.CustomDrawColumnHeader += new ColumnHeaderCustomDrawEventHandler(GridViewKeyboardController_CustomDrawColumnHeader);
			}
		}
		private void GridViewKeyboardController_KeyDown(object sender, KeyEventArgs e) {
			GridView gridView = ((GridControl)sender).MainView as GridView;
			#region Remove try-catch when  B32997 is fixed
			try {
				if(filterShortcut != null && e.KeyData == filterShortcut.Key && currentColumn != null) {
					gridView.ShowFilterPopup(currentColumn);
				}
			}
			catch(Exception) { }
			try {
				if(groupShortcut != null && e.KeyData == groupShortcut.Key) {
					Group(gridView);
				}
			}
			catch(Exception) { }
			try {
				if(sortShortcut != null && e.KeyData == sortShortcut.Key) {
					Sort();
				}
			}
			catch(Exception) { }
			if(e.KeyCode == Keys.Tab) {
				currentColumn = null;
			}
			try {
				if(navigateForwardShortcut != null && e.KeyData == navigateForwardShortcut.Key) {
					Navigate(gridView, false);
					e.Handled = true;
				}
			}
			catch(Exception) { }
			try {
				if(navigateBackShortcut != null && e.KeyData == navigateBackShortcut.Key) {
					Navigate(gridView, true);
					e.Handled = true;
				}
			}
			catch(Exception) { }
			#endregion
		}
		protected override void OnActivated() {
			base.OnActivated();
			Init();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			currentColumn = null;
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			GridListEditor gridListEditor = GetGridListEditor();
			if(gridListEditor != null && gridListEditor.Grid != null) {
				gridListEditor.Grid.KeyDown -= new KeyEventHandler(GridViewKeyboardController_KeyDown);
				gridListEditor.GridView.CustomDrawColumnHeader -= new ColumnHeaderCustomDrawEventHandler(GridViewKeyboardController_CustomDrawColumnHeader);
			}
			base.OnDeactivated();
		}
		public GridViewKeyboardController() {
			TargetViewType = ViewType.ListView;
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelOptions, IModelListViewShortcuts>();
		}
		#endregion
	}
	public interface IModelListViewShortcuts {
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShortcutsFilterShortcut"),
#endif
 DefaultValue("Control+Alt+F")]
		string FilterShortcut { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShortcutsSortShortcut"),
#endif
 DefaultValue("Control+Alt+I")]
		string SortShortcut { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShortcutsGroupShortcut"),
#endif
 DefaultValue("Control+Alt+T")]
		string GroupShortcut { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShortcutsNavigateBackShortcut"),
#endif
 DefaultValue("Control+Alt+Left")]
		string NavigateBackShortcut { get; set; }
		[
#if !SL
	DevExpressExpressAppWinLocalizedDescription("IModelListViewShortcutsNavigateForwardShortcut"),
#endif
 DefaultValue("Control+Alt+Right")]
		string NavigateForwardShortcut { get; set; }
	}
	#endregion
}
