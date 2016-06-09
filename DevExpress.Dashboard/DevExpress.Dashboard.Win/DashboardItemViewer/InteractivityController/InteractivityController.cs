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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public class InteractivityController {
		public static bool TupleListEquals(List<AxisPointTuple> list1, List<AxisPointTuple> list2) {
			if(list1 != null && list2 != null && list1.Count == list2.Count) {
				HashSet<AxisPointTuple> hashSet = new HashSet<AxisPointTuple>(list1);
				foreach(AxisPointTuple tuple in list2) {
					if(!hashSet.Contains(tuple))
						return false;
				}
				return true;
			}
			return false;
		}
		static int Contains(IList<AxisPointTuple> tupleList, AxisPointTuple tuple) {
			for(int i = 0; i < tupleList.Count; i++) {
				if(tupleList[i].Equals(tuple))
					return i;
			}
			return -1;
		}
		static bool IsInteractivityValue(AxisPointTuple tuple) {
			foreach(AxisPoint axisPoint in tuple.AxisPoints) {
				foreach(object dimensionValue in axisPoint.GetAllDimensionValues()) {
					if(DashboardSpecialValues.IsErrorValue(dimensionValue) ||
						DashboardSpecialValues.IsOthersValue(dimensionValue))
					return false;
				}
			}
			return true;
		}
		bool actionsLocked;
		readonly List<AxisPointTuple> highlight = new List<AxisPointTuple>();
		readonly List<AxisPointTuple> selection = new List<AxisPointTuple>();
		DashboardSelectionMode selectionMode;
		bool highlightEnabled;
		IInteractivityControllerClient client;
		ActionModel actionModel;
		List<AxisPointTuple> prevSelection = new List<AxisPointTuple>();
		protected bool ActionsLocked { get { return actionsLocked; } }
		protected IInteractivityControllerClient Client { get { return client; } }
		protected List<AxisPointTuple> Selection { get { return selection; } }
		protected bool HighlightEnabled { get { return highlightEnabled; } }
		protected DashboardSelectionMode SelectionMode { get { return selectionMode; } }
		public bool ShouldProcessInteractivity { get { return SelectionMode != DashboardSelectionMode.None || highlightEnabled; } }
		public bool CanSetMultipleMasterFilter { get { return actionModel.AffectedItems.ContainsKey(ActionType.SetMultipleValuesMasterFilter); } }
		public bool CanSetSingleMasterFilter { get { return CanSetMasterFilter && !CanSetMultipleMasterFilter; } }
		public bool CanSetMasterFilter { get { return actionModel.AffectedItems.ContainsKey(ActionType.SetMasterFilter); } }
		public bool CanClearMasterFilter { get { return actionModel.AffectedItems.ContainsKey(ActionType.ClearMasterFilter); } }
		public bool CanDrillDown { get { return actionModel.AffectedItems.ContainsKey(ActionType.DrillDown); } }
		public bool CanDrillUp { get { return actionModel.AffectedItems.ContainsKey(ActionType.DrillUp); } }
		public InteractivityController(IInteractivityControllerClient client) {
			this.client = client;
		}
		bool CustomVisualInteractivityEnabled { get { return !CanSetMasterFilter && !CanDrillDown; } }
		public void Update(List<AxisPointTuple> defaultSelection, ActionModel actionModel) {
			if(actionModel != null)
				this.actionModel = actionModel;
			if(defaultSelection != null) {
				selectionMode = GetSelectionMode();
				highlightEnabled = GetHighlightEnable();
				List<AxisPointTuple> newSelection = defaultSelection;
				if(CustomVisualInteractivityEnabled) {
					InteractivityOptions options = new InteractivityOptions() {
						DefaultSelection = defaultSelection,
						SelectionMode = selectionMode,
						HighlightEnable = highlightEnabled
					};
					options = client.RequestInteractivityOptions(options);
					if(options != null) {
						selectionMode = options.SelectionMode;
						highlightEnabled = options.HighlightEnable;
						newSelection = options.DefaultSelection;
					}
				}
				selection.Clear();
				highlight.Clear();
				if(newSelection != null) {
					SetSelection(newSelection);
				}
				ApplySelection();
			}
		}
		public void SetMasterFilter(List<AxisPointTuple> values) {
			List<AxisPointTuple> filteredValues = new List<AxisPointTuple>();
			foreach(AxisPointTuple tuple in values)
				if(IsInteractivityValue(tuple) && !tuple.IsEmpty)
					filteredValues.Add(tuple);
			if(selectionMode != DashboardSelectionMode.None) {
				SetSelection(filteredValues);
				ApplySelection();
				PerformMasterFilter();
			}
		}
		public void ClearMasterFilter() {
			if(actionModel.ClearMasterFilterButtonState == DashboardCommon.ViewModel.ButtonState.Enabled) {
				ClearSelection();
				PerformClearMasterFilter();
			}
		}
		public void DrillDown(AxisPointTuple tuple) {
			if(IsInteractivityValue(tuple))
				PerformDrillDown(tuple, CanSetMasterFilter);
		}
		public void DrillUp() {
			if(CanDrillUp)
				client.PerformDrillUpOperation(CanClearMasterFilter);
		}
		public void ClearSelection() {
			selection.Clear();
			ApplySelection();
		}
		public virtual void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey) {
				if(selectionMode == DashboardSelectionMode.Multiple)
					actionsLocked = true;
				e.Handled = true;
			}
		}
		public virtual void ProcessKeyUp(KeyEventArgs e) {
			if(e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey) {
				UnlockAction();
				e.Handled = true;
			}
		}
		public void ProcessMouseMove(AxisPointTuple tuple) {
			if(highlightEnabled) {
				highlight.Clear();
				if(tuple != null && !tuple.IsEmpty && IsInteractivityValue(tuple)) {
					highlight.Add(tuple);
				}
				RaiseHighlightChanged();
			}
		}
		public void ProcessMouseLeave() {
			highlight.Clear();
			RaiseHighlightChanged();
		}
		public void ProcessMouseClick(MouseButtons mouseButtons, AxisPointTuple axisPointTuple) {
			if(mouseButtons == MouseButtons.Left)
				ProcessMouseClickAction(axisPointTuple);
		}
		public void ProcessMouseDoubleClick(AxisPointTuple tuple) {
			if(CanSetMasterFilter && CanDrillDown)
				PerformDrillDown(tuple, true);
		}
		public virtual void ProcessLostFocus() {
			UnlockAction();
		}
		public IList<DashboardItemCaptionButtonInfoCreator> GetPopupMenuCreators() {
			IList<DashboardItemCaptionButtonInfoCreator> creators = new List<DashboardItemCaptionButtonInfoCreator>();
			if(actionModel.DrillUpButtonState != DashboardCommon.ViewModel.ButtonState.Hidden)
				creators.Insert(0, new DrillUpPopupMenuCreator() { Enabled = actionModel.DrillUpButtonState == DashboardCommon.ViewModel.ButtonState.Enabled });
			if(CustomVisualInteractivityEnabled && selectionMode == DashboardSelectionMode.Multiple) {
				creators.Insert(0, new ClearSelectionPopupMenuCreator() { Enabled = selection.Count > 0 });
			}
			else if(actionModel.ClearMasterFilterButtonState != DashboardCommon.ViewModel.ButtonState.Hidden) {
				bool clearMasterFilterButtonEnable = actionModel.ClearMasterFilterButtonState == DashboardCommon.ViewModel.ButtonState.Enabled;
				creators.Insert(0, new ClearMasterFilterPopupMenuCreator() { Enabled = clearMasterFilterButtonEnable });
			}
			return creators;
		}
		protected virtual void ProcessMouseClickAction(AxisPointTuple tuple) {
			if(tuple != null && !tuple.IsEmpty && IsInteractivityValue(tuple)) {
				if(!CanSetMasterFilter && CanDrillDown) {
					PerformDrillDown(tuple, false);
				}
				else {
					bool selectionChanged = CombineSelection(tuple);
					if(selectionChanged) {
						ApplySelection();
						if(!actionsLocked) {
							PerformMasterFilter();
						}
					}
				}
			}
		}
		protected void ApplySelection() {
			client.SetSelection(selection);
			RaiseSelectionChanged();
		}
		protected void RaiseSelectionChanged() {
			if(!TupleListEquals(prevSelection, selection)) {
				prevSelection = new List<AxisPointTuple>(selection);
				client.RaiseSelectionChanged(selection);
			}
		}
		protected void SetSelection(List<AxisPointTuple> newSelection) {
			selection.Clear();
			selection.AddRange(newSelection);
			if(selectionMode == DashboardSelectionMode.Single && selection.Count > 1)
				selection.RemoveRange(1, selection.Count - 1);
		}
		protected void PerformMasterFilter() {
			if(CanSetMasterFilter) {
				if(selection.Count > 0)
					client.PerformMasterFilterOperation(selection);
				else 
					PerformClearMasterFilter();
			}
		}
		protected virtual void PerformClearMasterFilter() {
			client.PerformClearMasterFilterOperation(false);
		}
		public bool CombineSelection(AxisPointTuple value) {
			if(selectionMode != DashboardSelectionMode.None) {
				int index = Contains(selection, value);
				bool contains = index > -1;
				if(selectionMode == DashboardSelectionMode.Multiple && actionsLocked) {
					if(contains)
						selection.RemoveAt(index);
					else
						selection.Add(value);
					return true;
				}
				else if(!(contains && selection.Count == 1)) {
					selection.Clear();
					selection.Add(value);
					return true;
				}
			}
			return false;
		}
		protected virtual void UnlockAction() {
			if(actionsLocked) {
				actionsLocked = false;
				if(selectionMode == DashboardSelectionMode.Multiple)
					PerformMasterFilter();
			}
		}
		bool GetHighlightEnable() {
			return !CustomVisualInteractivityEnabled;
		}
		DashboardSelectionMode GetSelectionMode() {
			if(CanSetSingleMasterFilter)
				return DashboardSelectionMode.Single;
			if(CanSetMultipleMasterFilter)
				return DashboardSelectionMode.Multiple;
			return DashboardSelectionMode.None;
		}
		void RaiseHighlightChanged() {
			if(highlightEnabled)
				client.SetHighlight(highlight);
		}
		void PerformDrillDown(AxisPointTuple tuple, bool trySetMasterFilter) {
			if(CanDrillDown)
				client.PerformDrillDownOperation(tuple, trySetMasterFilter);
		}
	}
}
