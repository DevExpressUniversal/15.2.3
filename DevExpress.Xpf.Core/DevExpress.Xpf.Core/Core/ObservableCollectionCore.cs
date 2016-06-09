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
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core {
	public interface ISupportGetCachedIndex<T> {
		int GetCachedIndex(T item);
	}
	public class ObservableCollectionCore<T> : ObservableCollection<T>, ILockable, ISupportGetCachedIndex<T> {
		Lazy<Dictionary<T, int>> indexCacheValue;
		int lockCount;
		Locker collectionChangedLocker = new Locker();
		LazyValue<List<T>> postponedRemoveListValue = new LazyValue<List<T>>(newList);
	static Func<List<T>> newList = () => new List<T>();
	static Func<Dictionary<T, int>> newDict = () => new Dictionary<T, int>();
		public ObservableCollectionCore() {
			indexCacheValue = new Lazy<Dictionary<T, int>>(() => new Dictionary<T, int>());
		}
		public virtual void BeginUpdate() {
			lockCount++;
		}
		protected internal void CancelUpdate() {
			lockCount--;
		}
		public bool IsLockUpdate { get { return lockCount != 0; } }
		public void Assign(IList<T> source) {
			if(!NeedsRebuild(source))
				return;
			BeginUpdate();
			Clear();
			foreach(T item in source) {
				Add(item);
			}
			EndUpdate();
		}
		public void RemoveSafe(T item) {
			if(IndexOf(item) < 0)
				return;
			collectionChangedLocker.DoIfNotLocked(() => Remove(item), () => postponedRemoveListValue.Value.Add(item));
		}
		public bool NeedsRebuild(IList<T> source) {
			return !ListHelper.AreEqual(this, source);
		}
		public virtual void EndUpdate() {
			CancelUpdate();
			if(!IsLockUpdate) OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if(indexCacheValue.IsValueCreated)
				indexCacheValue.Value.Clear();
			if(IsLockUpdate) return;
			collectionChangedLocker.DoLockedAction(() => base.OnCollectionChanged(e));
			if(postponedRemoveListValue.Loaded) {
				postponedRemoveListValue.Value.ForEach(item => Remove(item));
			}
		}
		public int GetCachedIndex(T item) {
			int index;
			if(!indexCacheValue.Value.TryGetValue(item, out index)) {
				indexCacheValue.Value[item] = index = IndexOf(item);
			}
			return index;
		}
	}
}
