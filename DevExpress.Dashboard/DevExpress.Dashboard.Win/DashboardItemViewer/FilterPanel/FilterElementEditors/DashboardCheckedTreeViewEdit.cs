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

using DevExpress.DashboardCommon.Viewer;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardCheckedTreeViewEdit : DashboardTreeViewEdit {
		Hashtable nodeStateCache;
		public DashboardCheckedTreeViewEdit() {
			OptionsView.ShowColumns = false;
			OptionsView.ShowVertLines = false;
			OptionsView.ShowHorzLines = false;
			OptionsView.ShowIndicator = false;
			OptionsView.ShowBandsMode = Utils.DefaultBoolean.False;
			OptionsView.ShowCheckBoxes = true;
			OptionsBehavior.Editable = false;
			OptionsBehavior.AllowRecursiveNodeChecking = true;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			Columns.Add(new TreeListColumn() {
				FieldName = TreeViewNodeDisplayTextPropertyDescriptor.Member,
				VisibleIndex = 0
			});
			ParentFieldName = TreeViewParentIDPropertyDescriptor.Member;
			KeyFieldName = TreeViewUniqueIDPropertyDescriptor.Member;
			BorderStyle = BorderStyles.NoBorder;
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			PerformAction(() => {
				nodeStateCache = SaveNodesData();
				base.OnBindingContextChanged(e);
				RestoreNodesData(nodeStateCache);
			});
		}
		protected override void RaiseAfterCheckNode(TreeListNode node) {
			base.RaiseAfterCheckNode(node);
			if(!IsShowAllNode(node)) {
				SetShowAllNodeState();
				RaiseCheckedChanged();
			}
		}
		int GetNodeUniqueID(TreeListNode node) {
			return Convert.ToInt32(node.GetValue(TreeViewUniqueIDPropertyDescriptor.Member));
		}
		void RaiseCheckedChanged() {
			if(!UpdateLocker.IsLocked)
				RaiseElementSelectionChanged(EventArgs.Empty);
		}
		protected override IEnumerable<int> GetSelectionInternal() {
			TreeListGetSelectedNodeOperation operation = new TreeListGetSelectedNodeOperation(GetNodeUniqueID);
			NodesIterator.DoOperation(operation);
			return operation.Selection;
		}
		protected override void SetSelectionInternal(IEnumerable<int> selection) {
			HashSet<object> hashSet = new HashSet<object>(selection.Cast<object>());
			PerformAction(() => {
				NodesIterator.DoOperation((node) => {
					if(hashSet.Contains(node.GetValue(TreeViewUniqueIDPropertyDescriptor.Member)))
						SetNodeCheckState(node, CheckState.Checked, true);
				});
				SetShowAllNodeState();
			});
		}
		#region ShowAll client emulation
		void SetShowAllNodeState() {
			TreeListNode showAllNode = FindNodeByKeyID(-1);
			IList<TreeListNode> checkedNodes = GetAllCheckedNodes();
			if(checkedNodes.Contains(showAllNode))
				checkedNodes.Remove(showAllNode);
			if(AllNodesCount - 1 == checkedNodes.Count)
				showAllNode.CheckState = CheckState.Checked;
			else if(checkedNodes.Count == 0)
				showAllNode.CheckState = CheckState.Unchecked;
			else
				showAllNode.CheckState = CheckState.Indeterminate;
		}
		protected override CheckNodeEventArgs RaiseBeforeCheckNode(TreeListNode node, CheckState prevState, CheckState state) {
			if(IsShowAllNode(node)) {
				PerformAction(() => {
					if(state == CheckState.Checked)
						CheckAll();
					else
						UncheckAll();
				});
				RaiseCheckedChanged();
			}
			return base.RaiseBeforeCheckNode(node, prevState, state);
		}
		bool IsShowAllNode(TreeListNode node) {
			return GetNodeUniqueID(node) == -1;
		}
		#endregion
	}
	public class TreeListGetSelectedNodeOperation : TreeListOperation {
		IList<int> selection;
		Func<TreeListNode, int> getNodeUniqueIdDelegate;
		bool canContinueIteration = true;
		public override bool CanContinueIteration(TreeListNode node) {
			return canContinueIteration && base.CanContinueIteration(node);
		}
		public override bool NeedsVisitChildren(TreeListNode node) {
			return !node.Checked;
		}
		public TreeListGetSelectedNodeOperation(Func<TreeListNode, int> getNodeUniqueIdDelegate) {
			selection = new List<int>();
			this.getNodeUniqueIdDelegate = getNodeUniqueIdDelegate;
		}
		public override void Execute(TreeListNode node) {
			if(node.Checked) {
				int nodeID = getNodeUniqueIdDelegate(node);
				if(nodeID == -1) {
					canContinueIteration = false;
					selection.Clear();
				}
				selection.Add(nodeID);
			}
		}
		public IEnumerable<int> Selection { get { return selection; } }
	}
}
