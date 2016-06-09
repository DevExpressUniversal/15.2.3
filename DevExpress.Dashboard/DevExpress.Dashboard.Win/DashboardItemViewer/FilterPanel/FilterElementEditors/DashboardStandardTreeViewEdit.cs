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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardStandardTreeViewEdit : DashboardTreeViewEdit {
		public DashboardStandardTreeViewEdit() {
			OptionsView.ShowColumns = false;
			OptionsView.ShowVertLines = false;
			OptionsView.ShowHorzLines = false;
			OptionsView.ShowIndicator = false;
			OptionsBehavior.Editable = false;
			OptionsView.ShowBandsMode = Utils.DefaultBoolean.False;
			OptionsSelection.EnableAppearanceFocusedCell = false;
			FocusedNodeChanged += DashboardStandardTreeViewEdit_FocusedNodeChanged;
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
				base.OnBindingContextChanged(e); 
			});
		}
		void DashboardStandardTreeViewEdit_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			if(!UpdateLocker.IsLocked)
				RaiseElementSelectionChanged(e);
		}
		IEnumerable<TreeListNode> GetLastLevelNodes(TreeListNode node) {
			return node.HasChildren ? node.Nodes.SelectMany(child => GetLastLevelNodes(child)) : new List<TreeListNode> { node };
		}
		protected override  IEnumerable<int> GetSelectionInternal() {
			return GetLastLevelNodes(FocusedNode).Select(node => (int)node.GetValue(TreeViewUniqueIDPropertyDescriptor.Member));
		}
	}
}
