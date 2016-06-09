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

extern alias Platform;
using DevExpress.Design.UI;
using System;
using System.Windows.Input;
using System.Windows.Media;
using Guard = Platform::DevExpress.Utils.Guard;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class NodeSelectorItemViewModel : MvvmControlTreeNodeViewModel<INodeSelectorItem, NodeSelectorItemViewModel, NodeSelectorItemSelectorViewModel> {
		readonly NodeSelectorMainViewModelBase main;
		ICommand selectAndDoneCommand;
		public NodeSelectorItemViewModel(NodeSelectorItemViewModel parent, NodeSelectorItemSelectorViewModel selector, NodeSelectorMainViewModelBase main)
			: base(parent, selector, MvvmControlFilterStrategy.DoNotFilterVisibleTreeNodeChildren) {
			Guard.ArgumentNotNull(main, "main");
			this.main = main;
		}
		public string Name { get { return TreeNode.Name; } }
		public ImageSource Icon { get { return TreeNode.Icon; } }
		public void SelectAndDone() {
			if(!CanBeSelected) {
				IsExpanded = !IsExpanded;
				return;
			}
			this.main.ItemSelector.SelectedTreeNode = TreeNode;
			this.main.Done();
		}
		public ICommand SelectAndDoneCommand {
			get {
				if(selectAndDoneCommand == null)
					selectAndDoneCommand = new WpfDelegateCommand(SelectAndDone);
				return selectAndDoneCommand;
			}
		}
		protected override void OnTreeNodeChanged() {
			base.OnTreeNodeChanged();
			CanBeSelected = TreeNode.CanBeSelected;
		}
		protected override int CompareTreeNodes(INodeSelectorItem t1, INodeSelectorItem t2) {
			return string.Compare(t1.Name, t2.Name, StringComparison.Ordinal);
		}
		protected override bool GetIsVisible(string filter) {
			if(TreeNode.Parent == null) return false;
			string treeNodeName = TreeNode.Name;
			return treeNodeName == null || treeNodeName.ToLowerInvariant().Contains(filter);
		}
	}
}
