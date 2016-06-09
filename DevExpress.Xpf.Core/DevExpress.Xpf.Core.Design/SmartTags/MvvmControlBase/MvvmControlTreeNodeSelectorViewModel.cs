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

using DevExpress.Design.UI;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Mvvm;
using System.Collections;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class MvvmControlTreeNodeSelectorViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel> : BindableBase
		where TTreeNodeSelectorViewModel : MvvmControlTreeNodeSelectorViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel>
		where TTreeNodeViewModel : MvvmControlTreeNodeViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel>
		where TTreeNode : class, IMvvmControlTreeNode<TTreeNode> {
		readonly Func<string, Thread> filterAsync;
		TTreeNode rootTreeNode;
		TTreeNodeViewModel rootTreeNodeViewModel;
		TTreeNode selectedTreeNode;
		TTreeNodeViewModel selectedTreeNodeViewModel;
		string filterText;
		WeakEventHandler<EventArgs, EventHandler> selectedTreeNodeChanged;
		public MvvmControlTreeNodeSelectorViewModel() {
			filterAsync = AsyncHelper.Create<string>(FilterAsync);
		}
		public TTreeNode RootTreeNode {
			get { return rootTreeNode; }
			set { SetProperty(ref rootTreeNode, value, () => RootTreeNode, OnRootTreeNodeChanged); }
		}
		public TTreeNodeViewModel RootTreeNodeViewModel {
			get { return rootTreeNodeViewModel; }
			protected set { SetProperty(ref rootTreeNodeViewModel, value, () => RootTreeNodeViewModel, OnRootTreeNodeViewModelChanged); }
		}
		public TTreeNode SelectedTreeNode {
			get { return selectedTreeNode; }
			set { SetProperty(ref selectedTreeNode, value, () => SelectedTreeNode, OnSelectedTreeNodeChanged); }
		}
		public TTreeNodeViewModel SelectedTreeNodeViewModel {
			get { return selectedTreeNodeViewModel; }
			set { SetProperty(ref selectedTreeNodeViewModel, value, () => SelectedTreeNodeViewModel, OnSelectedTreeNodeViewModelChanged); }
		}
		public string FilterText {
			get { return filterText; }
			set { SetProperty(ref filterText, value, () => FilterText, Filter); }
		}
		public Thread FilterThread { get; private set; }
		public event EventHandler SelectedTreeNodeChanged { add { selectedTreeNodeChanged += value; } remove { selectedTreeNodeChanged -= value; } }
		public TTreeNodeViewModel CreateTreeNodeViewModel(MvvmControlTreeNodeViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel> parent, TTreeNode treeNode) {
			TTreeNodeViewModel v = CreateTreeNodeViewModelCore(Guard.ArgumentMatchType<TTreeNodeViewModel>(parent, "parent"));
			v.TreeNode = treeNode;
			return v;
		}
		protected abstract TTreeNodeViewModel CreateTreeNodeViewModelCore(TTreeNodeViewModel parent);
		void OnRootTreeNodeChanged() {
			if(RootTreeNode == null) {
				RootTreeNodeViewModel = null;
				return;
			}
			if(RootTreeNodeViewModel == null) {
				TTreeNodeViewModel rootTreeNodeViewModel = CreateTreeNodeViewModelCore(null);
				rootTreeNodeViewModel.IsExpanded = true;
				rootTreeNodeViewModel.TreeNode = RootTreeNode;
				RootTreeNodeViewModel = rootTreeNodeViewModel;
			} else {
				RootTreeNodeViewModel.TreeNode = RootTreeNode;
			}
			if(!string.IsNullOrEmpty(FilterText))
				Filter();
		}
		void OnRootTreeNodeViewModelChanged() {
			if(RootTreeNodeViewModel != null)
				ExpandTreeViewItem(RootTreeNodeViewModel.Children);
		}
		void ExpandTreeViewItem(ICollection collection) {
			if(collection == null || collection.Count != 1) return;
			foreach(MvvmControlTreeNodeViewModel<TTreeNode, TTreeNodeViewModel, TTreeNodeSelectorViewModel> item in collection) {
				item.IsExpanded = true;
				ExpandTreeViewItem(item.Children);
			}
		}
		void Filter() {
			string filterText = string.Empty;
			if(!string.IsNullOrEmpty(FilterText))
				filterText = FilterText.ToLowerInvariant();
			FilterThread = filterAsync(filterText);
		}
		IEnumerable<ManualResetEvent> FilterAsync(string text, CancellationToken cancellationToken) {
			TTreeNodeViewModel rootTreeNodeViewModel = RootTreeNodeViewModel;
			if(rootTreeNodeViewModel != null)
				rootTreeNodeViewModel.Filter(text, cancellationToken);
			return null;
		}
		protected virtual void OnSelectedTreeNodeChanged() {
			selectedTreeNodeChanged.SafeRaise(this, EventArgs.Empty);
			if(RootTreeNodeViewModel == null) return;
			TTreeNode[] path = GetReversedPathTo(SelectedTreeNode).Reverse().ToArray();
			if(path.Length == 0 || !object.Equals(path[0], RootTreeNodeViewModel.TreeNode)) return;
			TTreeNodeViewModel viewModel = RootTreeNodeViewModel;
			for(int i = 1; i < path.Length; ++i) {
				if(viewModel == null) break;
				viewModel.IsExpanded = true;
				viewModel = viewModel.Children.Where(c => object.Equals(c.TreeNode, path[i])).FirstOrDefault();
			}
			DebugHelper.Assert(viewModel != null && object.Equals(selectedTreeNode, viewModel.TreeNode));
			if(viewModel != null)
				viewModel.IsSelected = true;
		}
		protected virtual void OnSelectedTreeNodeViewModelChanged() {
			SelectedTreeNode = SelectedTreeNodeViewModel == null ? null : SelectedTreeNodeViewModel.TreeNode;
		}
		static IEnumerable<TTreeNode> GetReversedPathTo(TTreeNode item) {
			for(TTreeNode parent = item; parent != null; parent = parent.Parent) {
				yield return parent;
			}
		}
	}
	public interface IMvvmControlTreeNode<TTreeNode> where TTreeNode : IMvvmControlTreeNode<TTreeNode> {
		TTreeNode Parent { get; }
		IEnumerable<TTreeNode> GetChildren();
	}
}
