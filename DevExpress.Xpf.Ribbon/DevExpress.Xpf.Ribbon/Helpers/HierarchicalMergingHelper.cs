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

using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
namespace DevExpress.Xpf.Ribbon {
	interface IHierarchicalMergingSupport<TElement> where TElement : IHierarchicalMergingSupport<TElement> {
		bool AsyncMergingEnabled { get; }
		HierarchicalMergingHelper<TElement> Helper { get; }
		TElement MergedParent { get; set; }
		void InvalidateMeasure();
		void ReMerge();
	}
	class HierarchicalMergingHelper<TElement> : DispatcherObject where TElement : IHierarchicalMergingSupport<TElement> {
		TElement owner;
		ObservableCollection<TElement> actualMergedChildren = new ObservableCollection<TElement>();
		ObservableCollection<TElement> mergedChildren;
		ReadOnlyObservableCollection<TElement> readonlyMergedChildren;
		ReadOnlyObservableCollection<TElement> compositeMergedChildren;
		CompositeCollection compositeMergedChildrenInternal;
		ICollectionView compositeMergedChildrenViewInternal;
		PostponedAction remergeAction;
		TElement mergedParent;
		private TElement actualMergedParent;
		public TElement ActualMergedParent {
			get { return actualMergedParent; }
			set {
				if (Equals(value, actualMergedParent)) return;
				TElement oldValue = actualMergedParent;
				actualMergedParent = value;
				OnActualMergedParentChanged(oldValue);
			}
		}		
		public TElement MergedParent {
			get { return mergedParent; }
			set {
				if(Equals(value, mergedParent)) return;
				TElement oldValue = mergedParent;
				mergedParent = value;
				OnMergedParentChanged(oldValue);
			}
		}
		public ObservableCollection<TElement> ActualMergedChildren { get { return actualMergedChildren; } }
		public ReadOnlyObservableCollection<TElement> CompositeMergedChildren { get { return compositeMergedChildren; } }
		public ReadOnlyObservableCollection<TElement> MergedChildren { get { return readonlyMergedChildren; } }
		ICollectionView CompositeMergedChildrenInternal { get { return compositeMergedChildrenViewInternal; } }
		public HierarchicalMergingHelper(TElement owner) {
			remergeAction = new PostponedAction(TrueResult);
			this.owner = owner;
			mergedChildren = new ObservableCollection<TElement>();
			readonlyMergedChildren = new ReadOnlyObservableCollection<TElement>(mergedChildren);
			compositeMergedChildrenInternal = new CompositeCollection();
			compositeMergedChildrenViewInternal = ((ICollectionViewFactory)compositeMergedChildrenInternal).CreateView();
			var wrappedCollection = new ObservableCollectionConverter<TElement, TElement>() { Selector = new Func<TElement, TElement>(SelectorFunc), Source = CompositeMergedChildrenInternal };
			compositeMergedChildren = new ReadOnlyObservableCollection<TElement>(wrappedCollection);
			((INotifyCollectionChanged)MergedChildren).CollectionChanged += OnMergedChildrenCollectionChanged;
			((INotifyCollectionChanged)CompositeMergedChildren).CollectionChanged += OnCompositeMergedChildrenCollectionChanged;
		}
		TElement SelectorFunc(TElement source) { return source; }
		bool TrueResult() { return true; }
		void OnMergedParentChanged(TElement oldValue) {
			owner.MergedParent = MergedParent;
			ReMergeAsync();
		}
		protected virtual void OnActualMergedParentChanged(TElement oldValue) {
			if (ActualMergedParent != null)
				BarNameScope.GetService<IItemToLinkBinderService>(owner).Lock();
			else
				BarNameScope.GetService<IItemToLinkBinderService>(owner).Unlock();
		}
		public void Merge(TElement child) {
			if(child == null || MergedChildren.Contains(child) || child.MergedParent != null)
				return;
			mergedChildren.Add(child);
		}
		public void UnMerge(TElement child) {
			if(child == null || !MergedChildren.Contains(child))
				return;
			mergedChildren.Remove(child);
		}
		public void ReMergeAsync() {
			remergeAction.PerformPostpone(ReMerge);
			if (owner.AsyncMergingEnabled)
				Dispatcher.BeginInvoke(new Action(ReMergeForce), (DispatcherPriority)7);
			else
				ReMergeForce();
			owner.InvalidateMeasure();
		}
		void ReMerge() {
			if(ActualMergedChildren.All(CompositeMergedChildren.Contains) && CompositeMergedChildren.All(ActualMergedChildren.Contains) && MergedParent == null)
				return;
			owner.ReMerge();
		}
		public void ReMergeForce() {
			var mChildren = ActualMergedChildren.ToArray();
			foreach(var element in MergedChildren) {
				element.Helper.ReMergeForce();
			}
			remergeAction.PerformForce();
			var oldMChildren = mChildren.Except(ActualMergedChildren).ToArray();
			var newMChildren = ActualMergedChildren.Except(mChildren).ToArray();
			foreach(var element in oldMChildren)
				element.Helper.ReMergeForce();
			foreach(var element in newMChildren)
				element.Helper.ReMergeForce();
		}
		protected virtual void OnMergedChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.NewItems != null) {
				foreach(TElement newChild in e.NewItems) {
					newChild.Helper.MergedParent = owner;
					((IMergingSupport)newChild).IsAutomaticallyMerged &= MergingPropertiesHelper.IsAutomaticMergingInProcess(newChild);
				}
			}
			if(e.OldItems != null) {
				foreach(TElement newChild in e.OldItems) {
					newChild.Helper.MergedParent = default(TElement);
					((IMergingSupport)newChild).IsAutomaticallyMerged = true;
				}
			}
			compositeMergedChildrenInternal.Clear();
			foreach(var child in MergedChildren) {
				compositeMergedChildrenInternal.Add(child);
				compositeMergedChildrenInternal.Add(new CollectionContainer() { Collection = child.Helper.CompositeMergedChildren });
			}
			CompositeMergedChildrenInternal.Refresh();
		}
		protected virtual void OnCompositeMergedChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			ReMergeAsync();
		}
	}
}
