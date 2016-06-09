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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.Utils;
using Guard = Platform::DevExpress.Utils.Guard;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class MvvmControlTreeNodeViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel> : BindableBase
		where TTreeNodeSelectorViewModel : MvvmControlTreeNodeSelectorViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel>
		where TTreeNodeViewModel : MvvmControlTreeNodeViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel>
		where TTreeNode : class, IMvvmControlTreeNode<TTreeNode> {
		public const int FilterDepth = 3;
		TTreeNode treeNode;
		bool canBeSelected = true;
		bool isSelected;
		bool isExpanded;
		bool isVisible = true;
		bool hasVisibleChildren = true;
		int distanceToExpandedBranch = int.MaxValue;
		string searchText = string.Empty;
		WeakEventHandler<EventArgs, EventHandler> isExpandedChanged;
		WeakEventHandler<EventArgs, EventHandler> distanceToExpandedBranchChanged;
		MvvmControlFilterStrategy filterStrategy;
		public MvvmControlTreeNodeViewModel(TTreeNodeViewModel parent, TTreeNodeSelectorViewModel selector, MvvmControlFilterStrategy filterStrategy)
			: base() {
			this.filterStrategy = filterStrategy;
			Parent = parent;
			if(Parent != null) {
				Parent.IsExpandedChanged += OnParentIsExpandedChanged;
				Parent.DistanceToExpandedBranchChanged += OnParentDistanceToExpandedBranchChanged;
			}
			Selector = selector;
			UpdateDistanceToExpandedBranch();
		}
		public TTreeNodeViewModel Parent { get; private set; }
		public TTreeNodeSelectorViewModel Selector { get; private set; }
		public TTreeNode TreeNode {
			get { return treeNode; }
			set { SetProperty(ref treeNode, value, () => treeNode, OnTreeNodeChanged); }
		}
		public bool CanBeSelected {
			get { return canBeSelected; }
			set { SetProperty(ref canBeSelected, value, () => CanBeSelected); }
		}
		public bool IsSelected {
			get { return isSelected; }
			set { SetProperty(ref isSelected, value, () => IsSelected, OnIsSelectedChanged); }
		}
		public bool IsExpanded {
			get { return isExpanded; }
			set { SetProperty(ref isExpanded, value, () => IsExpanded, OnIsExpandedChanged); }
		}
		public int DistanceToExpandedBranch {
			get { return distanceToExpandedBranch; }
			private set { SetProperty(ref distanceToExpandedBranch, value, () => DistanceToExpandedBranch, OnDistanceToExpandedBranchChanged); }
		}
		public bool IsVisible {
			get { return isVisible; }
			private set {
				SetProperty(ref isVisible, value, () => IsVisible);
			}
		}
		public bool HasVisibleChildren {
			get { return hasVisibleChildren; }
			private set {
				SetProperty(ref hasVisibleChildren, value, () => HasVisibleChildren);
			}
		}
		public ObservableCollection<TTreeNodeViewModel> ActualChildren { get; private set; }
		public ObservableCollection<TTreeNodeViewModel> Children {
			get {
				if(ActualChildren == null)
					UpdateChildren();
				return ActualChildren;
			}
		}
		public string SearchText {
			get { return searchText; }
			private set { SetProperty(ref searchText, value, () => SearchText); }
		}
		public event EventHandler IsExpandedChanged { add { isExpandedChanged += value; } remove { isExpandedChanged -= value; } }
		public event EventHandler DistanceToExpandedBranchChanged { add { distanceToExpandedBranchChanged += value; } remove { distanceToExpandedBranchChanged -= value; } }
		string prevFilter = null;
		public bool Filter(string filter, CancellationToken cancellationToken) {
			if(cancellationToken.IsCancellationRequested) return true;
			bool isVisible = string.IsNullOrEmpty(filter) ? true : GetIsVisible(filter);
			SearchText = filter;
			if(isVisible && filterStrategy == MvvmControlFilterStrategy.DoNotFilterVisibleTreeNodeChildren)
				filter = string.Empty;
			bool hasVisibleChildren = false;
				foreach(TTreeNodeViewModel child in ActualChildren ?? (IEnumerable<TTreeNodeViewModel>)new TTreeNodeViewModel[] { }) {
					if(cancellationToken.IsCancellationRequested) return true;
					hasVisibleChildren |= child.Filter(filter, cancellationToken);
				}
			if(cancellationToken.IsCancellationRequested) return true;
			HasVisibleChildren = hasVisibleChildren;
			isVisible = isVisible || hasVisibleChildren;
			IsVisible = isVisible;
			prevFilter = filter;
			return isVisible;
		}
		bool IsSelectedAnyChild(TTreeNodeViewModel tTreeNodeViewModel) {
			if(tTreeNodeViewModel.IsSelected) return true;
			return tTreeNodeViewModel.ActualChildren != null && tTreeNodeViewModel.ActualChildren.Any(IsSelectedAnyChild);
		}
		protected virtual void UpdateChildrenCore(ObservableCollection<TTreeNodeViewModel> children) {
			int i = 0;
			foreach(TTreeNode newChildItem in TreeNode.GetChildren()) {
				while(true) {
					if(i >= children.Count) {
						TTreeNodeViewModel v = Selector.CreateTreeNodeViewModel(this, newChildItem);
						children.Add(v);
						++i;
						break;
					}
					int compare = CompareTreeNodes(children[i].TreeNode, newChildItem);
					if(compare < 0) {
						children.RemoveAt(i);
					} else if(compare > 0) {
						TTreeNodeViewModel v = Selector.CreateTreeNodeViewModel(this, newChildItem);
						children.Insert(i, v);
						++i;
						break;
					} else {
						children[i].TreeNode = newChildItem;
						++i;
						break;
					}
				}
			}
			for(int tail = children.Count - i; --tail >= 0; )
				children.RemoveAt(children.Count - 1);
		}
		protected abstract int CompareTreeNodes(TTreeNode t1, TTreeNode t2);
		protected abstract bool GetIsVisible(string filter);
		protected virtual void OnTreeNodeChanged() {
			IsSelected = object.Equals(Selector.SelectedTreeNode, TreeNode);
			ResetOrUpdateChildren();
		}
		void OnParentIsExpandedChanged(object sender, EventArgs e) {
			UpdateDistanceToExpandedBranch();
		}
		void OnParentDistanceToExpandedBranchChanged(object sender, EventArgs e) {
			UpdateDistanceToExpandedBranch();
		}
		protected virtual void OnDistanceToExpandedBranchChanged() {
			distanceToExpandedBranchChanged.SafeRaise(this, EventArgs.Empty);
			ResetOrUpdateChildren();
		}
		void UpdateDistanceToExpandedBranch() {
			DistanceToExpandedBranch = Parent == null ? 0 : Parent.IsExpanded ? Parent.DistanceToExpandedBranch : Parent.DistanceToExpandedBranch + 1;
		}
		void UpdateChildren() {
			if(ActualChildren == null) {
				ObservableCollection<TTreeNodeViewModel> actualChildren = new ObservableCollection<TTreeNodeViewModel>();
				UpdateChildrenCore(actualChildren);
				ActualChildren = actualChildren;
			} else {
				UpdateChildrenCore(ActualChildren);
			}
		}
		void ResetOrUpdateChildren() {
			if(TreeNode == null) return;
			if(DistanceToExpandedBranch >= FilterDepth) {
				ActualChildren = null;
				RaisePropertyChanged(() => Children);
			} else {
				UpdateChildren();
				HasVisibleChildren = ActualChildren.Count != 0;
			}
		}
		protected virtual void OnIsExpandedChanged() {
			isExpandedChanged.SafeRaise(this, EventArgs.Empty);
		}
		protected virtual void OnIsSelectedChanged() {
			if(IsSelected) {
				Selector.SelectedTreeNodeViewModel = (TTreeNodeViewModel)this;
			} else {
				System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => {
					if(Selector.SelectedTreeNodeViewModel == this)
						Selector.SelectedTreeNodeViewModel = null;
				}));
			}
		}
	}
}
