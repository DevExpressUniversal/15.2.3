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
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListViewCellNavigation : GridViewCellNavigation {
		public TreeListViewCellNavigation(DataViewBase view)
			: base(view) {
		}
		protected TreeListView TreeListView { get { return View as TreeListView; } }
		protected internal override void ProcessKey(KeyEventArgs e) {
			switch(e.Key) {
				case Key.Multiply:
					e.Handled = OnMultiply();
					break;
				case Key.Space:
					e.Handled = OnSpace();
					break;
				default:
					break;
			}
			base.ProcessKey(e);
		}
		public override void OnRight(bool isCtrlPressed) {
			bool autoExpand = TreeListView.ShouldExpandCollapseNodesOnNavigation && View.ViewBehavior.NavigationStrategyBase.IsEndNavigationIndex(View);
			if((isCtrlPressed || autoExpand) && !View.IsEditing) {
				OnPlus(true);
				if(autoExpand)
					View.MoveNextCell(false);
			}
			else if(UseAdvHorzNavigation)
				DoBandedViewHorzNavigation(true);
			else
				View.MoveNextCell(false);
		}
		public override void OnLeft(bool isCtrlPressed) {
			bool autoCollapse = TreeListView.ShouldExpandCollapseNodesOnNavigation && View.ViewBehavior.NavigationStrategyBase.IsBeginNavigationIndex(View);
			if((isCtrlPressed || autoCollapse) && !View.IsEditing) {
				OnMinus(true);
				if(autoCollapse)
					View.MovePrevCell(false);
			}
			else if(UseAdvHorzNavigation)
				DoBandedViewHorzNavigation(false);
			else
				View.MovePrevCell(false);
		}
		public override bool OnMinus(bool isCtrlPressed) {
			if(View.IsEditing || View.IsKeyboardFocusInSearchPanel())
				return false;
			return base.OnMinus(isCtrlPressed);
		}
		public override bool OnPlus(bool isCtrlPressed) {
			if(View.IsEditing || View.IsKeyboardFocusInSearchPanel())
				return false;
			return base.OnPlus(isCtrlPressed);
		}
		public virtual bool OnMultiply() {
			if(View.IsInvalidFocusedRowHandle || View.IsExpanded(View.FocusedRowHandle) || View.IsEditing || View.IsKeyboardFocusInSearchPanel())
				return false;
			return TreeListView.ExpandNodeAndAllChildren();
		}
		public virtual bool OnSpace() {
			if(View.IsInvalidFocusedRowHandle || View.IsEditing || !TreeListView.ShowCheckboxes  || !TreeListView.GetNodeByRowHandle(View.FocusedRowHandle).IsCheckBoxEnabled || View.IsKeyboardFocusInSearchPanel())
				return false;
			return TreeListView.ChangeNodeCheckState(View.FocusedRowHandle);
		}
		protected internal override void ProcessMouse(DependencyObject originalSource) {
			if(TreeListView.FindRowMarginControl(originalSource) != null && !(originalSource is System.Windows.Controls.Image)) return;
			base.ProcessMouse(originalSource);
		}
	}
}
