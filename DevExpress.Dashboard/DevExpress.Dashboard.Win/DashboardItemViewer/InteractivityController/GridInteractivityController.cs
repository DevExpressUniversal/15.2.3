#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.ViewerData;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public class GridInteractivityController : InteractivityController {
		GridDashboardItemViewer gridViewer;
		bool ShiftPressed { get { return Control.ModifierKeys.HasFlag(Keys.Shift); } }
		public List<AxisPointTuple> ActualSelection { get { return Selection; } }
		public GridInteractivityController(GridDashboardItemViewer gridViewer, IInteractivityControllerClient client)
			: base(client) {
			this.gridViewer = gridViewer;
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Back:
					DrillUp();
				break;
				case Keys.Escape:
					ClearMasterFilter();
				break;
				case Keys.Enter:
					AxisPointTuple tuple = gridViewer.GetRowTuple(gridViewer.GetFocusedRowHandle());
					DrillDown(tuple);
				break;
			}
			base.ProcessKeyDown(e);
		}
		public override void ProcessKeyUp(KeyEventArgs e) {
			AxisPointTuple target = gridViewer.GetRowTuple(gridViewer.GetFocusedRowHandle());
			if(target != null) {
				switch(e.KeyCode) {
					case Keys.Up:
					case Keys.Down:
					case Keys.PageUp:
					case Keys.PageDown:
						PerformMasterFilterWithoutSettingSelection(target);
					break;
					case Keys.Space:
					case Keys.Tab:
						if(e.Modifiers == Keys.None)
							PerformMasterFilterWithoutSettingSelection(target);
					break;
				}
			}
			base.ProcessKeyUp(e);
		}
		void PerformMasterFilterWithoutSettingSelection(AxisPointTuple axisPointTuple) {
			if(!ActionsLocked) {
				Selection.Clear();
				if(CombineSelection(axisPointTuple)) {
					RaiseSelectionChanged();
					PerformMasterFilter();
				}
			}
		}
		protected override void ProcessMouseClickAction(AxisPointTuple tuple) {
			if(!ShiftPressed) {
				base.ProcessMouseClickAction(tuple);
			}	 
		}
		protected override void UnlockAction() {
			Selection.Clear();
			Selection.AddRange(gridViewer.GetSelectedTuples());
			ApplySelection();
			base.UnlockAction();
		}
		public void UpdateSelection() {
			ApplySelection();		}
		public void ProcessGridControlSelectionChanged(CollectionChangeAction action) {
			GridDashboardView gridView = gridViewer.GridView;
			if(SelectionMode == DashboardSelectionMode.None)
				gridView.ClearSelection();
			if(SelectionMode == DashboardSelectionMode.Single) {
				int[] selectedRows = gridView.GetSelectedRows();
				int focusedRow = gridView.FocusedRowHandle;
				if(((selectedRows.Length < 1 && action == CollectionChangeAction.Remove) || selectedRows.Length > 1) && focusedRow >= 0) {
					gridViewer.SetGridControlSelection(new List<AxisPointTuple> { gridViewer.GetRowTuple(focusedRow) });
				}
			}
		}
	}
}
