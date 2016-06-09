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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Charts {
	public abstract class ChartDependencyObjectCollection<T> : ObservableCollection<T>, ISupportInitialize, IOwnedElement {
		static void SetOwnerForChildren(IList children, IChartElement owner) {
			foreach (object child in children) {
				IChartElement element = child as IChartElement;
				if (element != null)
					element.Owner = owner;
			}
		}
		readonly Locker changedLocker = new Locker();
		IChartElement owner;
		protected virtual bool Loading { get { return false; } }
		protected virtual ChartElementChange Change { get { return ChartElementChange.None; } }
		protected IChartElement Owner { get { return owner; } }
		protected virtual bool BatchUpdateSupported { get { return false; } }
		protected virtual bool ShouldUpdateLogicalTree { get { return true; } }
		protected Locker ChangedLocker { get { return changedLocker; } }
		protected bool IsLocked { get { return changedLocker.IsLocked || Loading; } }
		#region IOwnedElement implementation
		IChartElement IOwnedElement.Owner {
			get { return owner; }
			set {
				RemoveChildren(this);
				owner = value;
				AddChildren(this);
			}
		}
		#endregion
		protected void Update() {
			if (owner == null)
				return;
			ChartElementHelper.Update(owner, Change | ChartElementChange.CollectionModified);
		}
		protected ChartCollectionOperation GetOperation(NotifyCollectionChangedAction action) {
			switch (action) {
				case NotifyCollectionChangedAction.Add:
					return ChartCollectionOperation.InsertItem;
				case NotifyCollectionChangedAction.Move:
					return ChartCollectionOperation.MoveItem;
				case NotifyCollectionChangedAction.Remove:
					return ChartCollectionOperation.RemoveItem;
				case NotifyCollectionChangedAction.Replace:
					return ChartCollectionOperation.RemoveItem;
				case NotifyCollectionChangedAction.Reset:
					return ChartCollectionOperation.Reset;
				default:
					throw new NotImplementedException();
			}
		}
		protected ICollection<D> GetCollection<D>(IList list) {
			if (list == null)
				return null;
			ICollection<D> collection = new Collection<D>();
			foreach (object obj in list)
				collection.Add((D)obj);
			return collection;
		}
		protected void AddChildren(IList children) {
			SetOwnerForChildren(children, owner);
			if (Owner != null && ShouldUpdateLogicalTree)
				foreach (object child in children)
					Owner.AddChild(child);
		}
		protected void RemoveChildren(IList children) {
			SetOwnerForChildren(children, null);
			if (Owner != null && ShouldUpdateLogicalTree)
				foreach (object child in children)
					Owner.RemoveChild(child);
		}
		protected internal virtual void EndInit(bool shouldUpdate) {
			changedLocker.Unlock();
			if (shouldUpdate && !IsLocked)
				Update();
		}
		public void AddRange(IEnumerable<T> items) {
			foreach (T item in items)
				Add(item);
		}
		public void BeginInit() {
			changedLocker.Lock();
		}
		public void EndInit() {
			EndInit(true);
		}
	}
	public abstract class ChartElementCollection<T> : ChartDependencyObjectCollection<T> {
		#region inner classes
		public class BatchUpdateHelper {
			ChartElementCollection<T> owner;
			List<ChartUpdateInfoBase> lockUpdateInfos = new List<ChartUpdateInfoBase>();
			public BatchUpdateHelper(ChartElementCollection<T> owner) {
				this.owner = owner;
			}
			bool IsCollectionBatchUpdateInfo(ChartUpdateInfoBase updateInfo) {
				return (updateInfo is SeriesPointCollectionBatchUpdateInfo) || (updateInfo is SeriesCollectionBatchUpdateInfo);
			}
			bool CanCreateBatchUpdate(CollectionUpdateInfo collectionUpdateInfo) {
				return ((collectionUpdateInfo != null) && ((collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem) || (collectionUpdateInfo.Operation == ChartCollectionOperation.RemoveItem)));
			}
			object GetItem(CollectionUpdateInfo collectionUpdateInfo) {
				return (collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem) ? collectionUpdateInfo.NewItem : collectionUpdateInfo.OldItem;
			}
			int GetIndex(CollectionUpdateInfo collectionUpdateInfo) {
				return collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem ? collectionUpdateInfo.NewIndex : collectionUpdateInfo.OldIndex;
			}
			bool IsIndexValid(CollectionUpdateInfo collectionUpdateInfo, int lastIndex) {
				if (collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem)
					return (lastIndex + 1) == collectionUpdateInfo.NewIndex;
				return lastIndex == collectionUpdateInfo.OldIndex;
			}
			public List<ChartUpdateInfoBase> CreateUpdateInfos() {
				List<ChartUpdateInfoBase> updateInfos = new List<ChartUpdateInfoBase>();
				for (int i = 0; i < lockUpdateInfos.Count; i++) {
					ChartUpdateInfoBase updateInfo = lockUpdateInfos[i];
					if (IsCollectionBatchUpdateInfo(updateInfo)) {
						updateInfos.Add(updateInfo);
						continue;
					}
					var collectionUpdateInfo = lockUpdateInfos[i] as CollectionUpdateInfo;
					if (!CanCreateBatchUpdate(collectionUpdateInfo)) {
						updateInfos.Add(lockUpdateInfos[i]);
						continue;
					}
					if (!owner.BatchUpdateSupported) {
						updateInfos.Add(lockUpdateInfos[i]);
						continue;
					}
					IList batchUpdate = GetUpdateInfoSequence(i);
					if (batchUpdate == null || batchUpdate.Count == 1) {
						updateInfos.Add(lockUpdateInfos[i]);
						continue;
					}
					if (collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem)
						updateInfos.Add(owner.CreateUpdateInfo(NotifyCollectionChangedAction.Add, batchUpdate, null, collectionUpdateInfo.NewIndex, -1));
					else
						updateInfos.Add(owner.CreateUpdateInfo(NotifyCollectionChangedAction.Remove, null, batchUpdate, -1, collectionUpdateInfo.OldIndex));
					i += (batchUpdate.Count - 1);
				}
				return updateInfos;
			}
			IList GetUpdateInfoSequence(int index) {
				IList updateInfoSequence = null;
				var collectionUpdateInfo = lockUpdateInfos[index++] as CollectionUpdateInfo;
				var operation = collectionUpdateInfo.Operation;
				if (collectionUpdateInfo != null) {
					updateInfoSequence = new List<object>();
					int lastIndex;
					lastIndex = GetIndex(collectionUpdateInfo);
					updateInfoSequence.Add(GetItem(collectionUpdateInfo));
					while (index < lockUpdateInfos.Count) {
						collectionUpdateInfo = lockUpdateInfos[index++] as CollectionUpdateInfo;
						if ((collectionUpdateInfo == null) || (collectionUpdateInfo.Operation != operation))
							break;
						if (!IsIndexValid(collectionUpdateInfo, lastIndex))
							break;
						updateInfoSequence.Add(GetItem(collectionUpdateInfo));
						lastIndex = GetIndex(collectionUpdateInfo);
					}
				}
				return updateInfoSequence as IList;
			}
			public void Add(ChartUpdateInfoBase changeInfo) {
				lockUpdateInfos.Add(changeInfo);
			}
			public void Clear() {
				lockUpdateInfos.Clear();
			}
		}
		#endregion
		readonly BatchUpdateHelper updateHelper;
		public ChartElementCollection() {
			updateHelper = new BatchUpdateHelper(this);
		}
		protected virtual ChartUpdateInfoBase CreateUpdateInfo(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newStartingIndex, int oldStartingIndex) {
			return new EmptyUpdateInfo(Owner);
		}
		protected virtual void PerformCollectionChanged(NotifyCollectionChangedEventArgs e) { }
		protected override void ClearItems() {
			RemoveChildren(this);
			base.ClearItems();
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null && (e.Action == NotifyCollectionChangedAction.Reset || e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace))
				RemoveChildren(e.OldItems);
			if (e.NewItems != null && (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace))
				AddChildren(e.NewItems);
			PerformCollectionChanged(e);
			if (!IsLocked)
				ChartElementHelper.Update(Owner, CreateUpdateInfo(e.Action, e.NewItems, e.OldItems, e.NewStartingIndex, e.OldStartingIndex), Change | ChartElementChange.CollectionModified);				
			else {
				updateHelper.Add(CreateUpdateInfo(e.Action, e.NewItems, e.OldItems, e.NewStartingIndex, e.OldStartingIndex));
			}
			base.OnCollectionChanged(e);
		}
		internal void ClearUpdatesCache() {
			updateHelper.Clear();
		}
		internal void ForceEndInit() {
			List<ChartUpdateInfoBase> updateInfos = updateHelper.CreateUpdateInfos();
			foreach (var updateInfo in updateInfos)
				ChartElementHelper.Update(Owner, updateInfo, Change | ChartElementChange.CollectionModified);
			ClearUpdatesCache();
		}
		protected internal override void EndInit(bool shouldUpdate) {
			ChangedLocker.Unlock();
			if (!IsLocked)
				ForceEndInit();
		}
		internal List<J> Convert<J>(Func<T, J> converter) {
			List<J> list = new List<J>();
			foreach (T obj in this) {
				J converted = converter(obj);
				if (converted != null)
					list.Add(converted);
			}
			return list;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class PointItemsDictionary : HybridDictionary {
		public List<SeriesPointItem> this[ISeriesPoint key] { get { return base[key] as List<SeriesPointItem>; } }
	}
}
