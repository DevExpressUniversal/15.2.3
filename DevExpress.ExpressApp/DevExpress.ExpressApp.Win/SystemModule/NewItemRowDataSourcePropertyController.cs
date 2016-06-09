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
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class NewItemRowDataSourcePropertyController : ViewController {
		private WinColumnsListEditor gridListEditor;
		private RepositoryItemLookupEdit lookupEdit;
		private RepositoryItemPopupCriteriaEdit popupCriteriaEdit;
		public NewItemRowDataSourcePropertyController() {
			TypeOfView = typeof(ListView);
			TargetViewNesting = Nesting.Any;
		}
		private void ReleaseGridListEditor() {
			if(gridListEditor != null) {
				if(gridListEditor.ColumnView != null) {
					gridListEditor.ColumnView.ShowingEditor -= new CancelEventHandler(GridView_ShowingEditor);
					gridListEditor.ColumnView.HiddenEditor -= new EventHandler(GridView_HiddenEditor);
				}
				gridListEditor = null;
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			ListView listView = (ListView)View;
			ReleaseGridListEditor();
			gridListEditor = listView.Editor as WinColumnsListEditor;
			if(gridListEditor != null) {
				gridListEditor.ColumnView.ShowingEditor += new CancelEventHandler(GridView_ShowingEditor);
				gridListEditor.ColumnView.HiddenEditor += new EventHandler(GridView_HiddenEditor);
			}
		}
		private void GridView_HiddenEditor(object sender, EventArgs e) {
			if(lookupEdit != null) {
				lookupEdit.QueryPopUp -= new CancelEventHandler(lookupEdit_QueryPopUp);
				lookupEdit = null;
			}
			if(popupCriteriaEdit != null) {
				popupCriteriaEdit.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(popupCriteriaEdit_ButtonClick);
				popupCriteriaEdit = null;
			}
		}
		private void GridView_ShowingEditor(object sender, CancelEventArgs e) {
			ColumnView columnView = (ColumnView)sender;
			lookupEdit = columnView.FocusedColumn.ColumnEdit as RepositoryItemLookupEdit;
			if(lookupEdit != null) {
				lookupEdit.QueryPopUp += new CancelEventHandler(lookupEdit_QueryPopUp);
			}
			popupCriteriaEdit = columnView.FocusedColumn.ColumnEdit as RepositoryItemPopupCriteriaEdit;
			if(popupCriteriaEdit != null) {
				popupCriteriaEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(popupCriteriaEdit_ButtonClick);
			}
		}
		private void popupCriteriaEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			EnsureCurrentObject();
		}
		private void lookupEdit_QueryPopUp(object sender, CancelEventArgs e) {
			EnsureCurrentObject();
		}
		private void EnsureCurrentObject() {
			if(((ListView)View).CurrentObject == null) {
				ColumnView gridView = ((WinColumnsListEditor)((ListView)View).Editor).ColumnView;
				gridView.ActiveEditor.IsModified = true;
				((IGridInplaceEdit)gridView.ActiveEditor).GridEditingObject = ((ListView)View).CurrentObject;
				gridView.RefreshEditor(true);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			ReleaseGridListEditor();
		}
	}
}
