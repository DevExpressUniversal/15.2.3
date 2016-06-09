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
	public class TreeListViewRowNavigation : GridViewRowNavigation {
		public TreeListViewRowNavigation(DataViewBase view)
			: base(view) {
		}
		protected TreeListView TreeListView { get { return View as TreeListView; } }
		protected internal override void ProcessKey(KeyEventArgs e) {
			bool handled = false;
			if(e.Key == Key.Multiply)
				handled = OnMultiply();
			else if(e.Key == Key.Space)
				handled = OnSpace();
			if(handled) e.Handled = true;
			else base.ProcessKey(e);
		}
		public override void OnRight(bool isCtrlPressed) {
			bool expandCollapseOnNavigation = TreeListView.ShouldExpandCollapseNodesOnNavigation;
			if(isCtrlPressed || expandCollapseOnNavigation) {
				if(!OnPlus(true) && expandCollapseOnNavigation && TreeListView.FocusedNode != null && TreeListView.FocusedNode.IsTogglable)
					OnDown();
			}
		}
		public override void OnLeft(bool isCtrlPressed) {
			bool expandCollapseOnNavigation = TreeListView.ShouldExpandCollapseNodesOnNavigation;
			if(isCtrlPressed || expandCollapseOnNavigation) {
				if(!OnMinus(true) && expandCollapseOnNavigation && TreeListView.FocusedNode != null && TreeListView.FocusedNode.ParentNode != null)
					TreeListView.FocusedNode = TreeListView.FocusedNode.ParentNode;
			}
		}
		public virtual bool OnMultiply() {
			if(View.IsInvalidFocusedRowHandle || View.IsExpanded(View.FocusedRowHandle) || View.IsKeyboardFocusInSearchPanel())
				return false;
			return TreeListView.ExpandNodeAndAllChildren();
		}
		public virtual bool OnSpace() {
			if(View.IsInvalidFocusedRowHandle || !TreeListView.ShowCheckboxes || !TreeListView.GetNodeByRowHandle(View.FocusedRowHandle).IsCheckBoxEnabled || View.IsKeyboardFocusInSearchPanel())
				return false;
			return TreeListView.ChangeNodeCheckState(View.FocusedRowHandle);
		}
		protected internal override void ProcessMouse(DependencyObject originalSource) {
			if(TreeListView.FindRowMarginControl(originalSource) != null && !(originalSource is System.Windows.Controls.Image)) return;
			base.ProcessMouse(originalSource);
		}
		public override void OnTab(bool isShiftPressed) {
			if(isShiftPressed)
				OnUp(true);
			else
				OnDown();
		}
	}
}
