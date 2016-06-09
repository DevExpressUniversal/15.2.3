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
using System.ComponentModel;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
namespace DevExpress.XtraCharts {
	[ListBindable(false)]
	public abstract class ChartCollectionBase : CollectionBase, IDisposable, IOwnedElement {
		#region inner classes
		public class BatchUpdateHelper {
			ChartCollectionBase owner;
			List<ChartUpdateInfoBase> lockUpdateInfos = new List<ChartUpdateInfoBase>();
			public BatchUpdateHelper(ChartCollectionBase owner) {
				this.owner = owner;
			}
			bool IsCollectionBatchUpdateInfo(ChartUpdateInfoBase updateInfo) {
				return (updateInfo is SeriesPointCollectionBatchUpdateInfo) || (updateInfo is SeriesCollectionBatchUpdateInfo);
			}
			bool CanCreateBatchUpdate(CollectionUpdateInfo collectionUpdateInfo) { 
				return ((collectionUpdateInfo != null) && ((collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem) || (collectionUpdateInfo.Operation == ChartCollectionOperation.RemoveItem)));
			}
			T GetItem<T>(CollectionUpdateInfo<T> collectionUpdateInfo) {
				return (collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem) ? collectionUpdateInfo.NewItem : collectionUpdateInfo.OldItem;
			}
			int GetIndex<T>(CollectionUpdateInfo<T> collectionUpdateInfo) {
				return collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem ? collectionUpdateInfo.NewIndex : collectionUpdateInfo.OldIndex;
			}
			bool IsIndexValid(CollectionUpdateInfo collectionUpdateInfo, int lastIndex) {
				if(collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem)
					return (lastIndex + 1) == collectionUpdateInfo.NewIndex;
				return lastIndex == collectionUpdateInfo.OldIndex;
			}
			public List<ChartUpdateInfoBase> CreateUpdateInfos() {
				List<ChartUpdateInfoBase> updateInfos = new List<ChartUpdateInfoBase>();
				for(int i = 0; i < lockUpdateInfos.Count; i++) {
					ChartUpdateInfoBase updateInfo = lockUpdateInfos[i];
					if(IsCollectionBatchUpdateInfo(updateInfo)) {
						updateInfos.Add(updateInfo);
						continue;
					}
					var collectionUpdateInfo = lockUpdateInfos[i] as CollectionUpdateInfo;
					if(!CanCreateBatchUpdate(collectionUpdateInfo)) {
						updateInfos.Add(lockUpdateInfos[i]);
						continue;
					}
					var batchUpdate = owner.GetUpdateInfoSequence(i);
					if(batchUpdate == null || batchUpdate.Count == 1) {
						updateInfos.Add(lockUpdateInfos[i]);
						continue;
					}
					if(collectionUpdateInfo.Operation == ChartCollectionOperation.InsertItem)
						updateInfos.Add(owner.CreateBatchUpdateInfo(ChartCollectionOperation.InsertItem, null, -1, batchUpdate, collectionUpdateInfo.NewIndex));
					else
						updateInfos.Add(owner.CreateBatchUpdateInfo(ChartCollectionOperation.RemoveItem, batchUpdate, collectionUpdateInfo.OldIndex, null, -1));
					i += (batchUpdate.Count - 1);
				}
				return updateInfos;
			}
			public ICollection GetUpdateInfoSequence<T>(int index) {
				ICollection<T> updateInfoSequence = null;
				var collectionUpdateInfo = lockUpdateInfos[index++] as CollectionUpdateInfo<T>;
				var operation = collectionUpdateInfo.Operation;
				if(collectionUpdateInfo != null) {
					updateInfoSequence = new System.Collections.ObjectModel.Collection<T>();
					int lastIndex;
					lastIndex = GetIndex<T>(collectionUpdateInfo);
					updateInfoSequence.Add(GetItem<T>(collectionUpdateInfo));
					while(index < lockUpdateInfos.Count) {
						collectionUpdateInfo = lockUpdateInfos[index++] as CollectionUpdateInfo<T>;
						if((collectionUpdateInfo == null) || (collectionUpdateInfo.Operation != operation))
							break;
						if(!IsIndexValid(collectionUpdateInfo, lastIndex))
							break;
						updateInfoSequence.Add(GetItem<T>(collectionUpdateInfo));
						lastIndex = GetIndex(collectionUpdateInfo);
					}
				}
				return updateInfoSequence as ICollection;
			}
			public void Add(ChartUpdateInfoBase changeInfo) {
				lockUpdateInfos.Add(changeInfo);
			}
			public void Clear() {
				lockUpdateInfos.Clear();
			}
		}
		#endregion
		readonly List<int> ids = new List<int>();
		int lastID = 0;
		ChartElement owner; 
		int updateModeCounter = 0;
		bool isDisposed = false;
		BatchUpdateHelper batchUpdateHelper;
		protected NotificationCenter NotificationCenter {
			get {
				return (Owner != null) ? Owner.NotificationCenter : null;
			}
		}
		protected BatchUpdateHelper UpdateHelper { get { return batchUpdateHelper; } }
		protected internal ChartElement Owner { get { return owner; } set { owner = value; } }
		protected virtual bool UpdateLocked { get { return updateModeCounter > 0; } }
		protected ChartCollectionBase() : base() {
			batchUpdateHelper = new BatchUpdateHelper(this);
		}
		protected ChartCollectionBase(ChartElement owner) : base() {
			this.owner = owner;
			batchUpdateHelper = new BatchUpdateHelper(this);
		}
		~ChartCollectionBase() {
			Dispose(false);
		}
		#region IDisposable implementation
		public void Dispose() {
			if (!isDisposed) {
				Dispose(true);
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}
		protected virtual void Dispose(bool disposing) {
			DisposeItems();
		}
		#endregion
		#region IOwnedElement implementation
		IOwnedElement IOwnedElement.Owner { get { return Owner; } }
		IChartContainer IOwnedElement.ChartContainer { get { return Owner.ChartContainer; } }
		#endregion
		#region ID Generator
		int GenerateID() {
			int id = lastID;
			for (; ; ) {
				int indexID = ids.BinarySearch(id);
				if (indexID >= 0) {
					id++;
					continue;
				}
				lastID = id;
				ids.Insert(~indexID, id);
				return id;
			}
		}
		void InitializeID(ISupportID item) {
			if (item != null) {
				if (item.ID == -1)
					item.ID = GenerateID();
				else {
					int indexID = ids.BinarySearch(item.ID);
					if (indexID < 0)
						ids.Insert(~indexID, item.ID);
					if (item.ID > lastID)
						lastID = item.ID;
				}
			}
		}
		void ClearID(ISupportID item) {
			if (item != null && item.ID >= 0 && ids.BinarySearch(item.ID) >= 0) {
				ids.Remove(item.ID);
				if (item.ID < lastID)
					lastID = item.ID;
			}
		}
		void ClearIDs() {
			lastID = 0;
			ids.Clear();
		}
		#endregion
		void DisposeItemsBeforeRemove() {
			foreach (ChartElement item in this)
				DisposeItemBeforeRemove(item);
		}
		void DisposeItems() {
			foreach (ChartElement item in this)
				DisposeItem(item);
		}
		void DecreaseUpdateCounter() {
			updateModeCounter = Math.Max(0, --updateModeCounter);
		}
		protected virtual void DisposeItem(ChartElement item) {
			item.Dispose();
		}
		protected virtual void DisposeItemBeforeRemove(ChartElement item) {
			item.Dispose();
			item.Owner = null;
		}
		protected virtual ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific);
		}
		protected virtual ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific);
		}
		protected virtual ICollection GetUpdateInfoSequence(int index) { return null; }
		protected virtual void OnBeginUpdate() { }
		protected virtual void OnEndUpdate() { }
		protected T GetOwner<T>() where T : IOwnedElement {
			IOwnedElement owner = Owner;
			while (owner != null) {
				if (owner is T)
					return (T)owner;
				owner = Owner.Owner;
			}
			return default(T);
		}
		protected virtual void ChangeOwnerForItem(ChartElement item) {
			item.Owner = owner;
			InitializeID(item as ISupportID);
		}
		protected override void OnClear() {
			SendControlChanging();
			DisposeItemsBeforeRemove();
			base.OnClear();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			ClearIDs();
			RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.Reset, null, -1, null, -1));
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			ChangeOwnerForItem((ChartElement)value);
			RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.InsertItem, null, -1, value, index));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			ClearID(value as ISupportID);
			RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.RemoveItem, value, index, null, -1));
			DisposeItemBeforeRemove((ChartElement)value);
		}
		protected internal virtual void SendControlChanging() {
			if (!UpdateLocked && NotificationCenter != null)
				NotificationCenter.Send(new ChartElement.ElementWillChangeNotification(this));
		}
		protected internal void RaiseControlChanged() {
			RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific));
		}
		protected internal virtual void RaiseControlChanged(ChartUpdateInfoBase changeInfo) {
			if (!UpdateLocked)
				ProcessChanged(changeInfo);
			else
				batchUpdateHelper.Add(changeInfo);
		}
		protected internal virtual void ProcessChanged(ChartUpdateInfoBase changeInfo) {
			if (owner != null)
				owner.ChildCollectionChanged(this, changeInfo);
		}
		protected internal int AddWithoutChanged(ChartElement item) {
			int index = InnerList.Add(item);
			ChangeOwnerForItem(item);
			return index;
		}
		protected int Add(ChartElement item) {
			SendControlChanging();
			return List.Add(item);
		}
		protected void AddRangeWithoutChanged(ICollection coll) {
			InnerList.AddRange(coll);
			foreach (ChartElement item in coll)
				ChangeOwnerForItem(item);
		}
		protected virtual void OnRangeAddComplete(int index) { }
		protected void AddRange(ICollection coll) {
			int index = Count;
			SendControlChanging();
			AddRangeWithoutChanged(coll);
			RaiseControlChanged(CreateBatchUpdateInfo(ChartCollectionOperation.InsertItem, null, -1, coll, index));
		}
		protected void Insert(int index, ChartElement item) {
			SendControlChanging();
			List.Insert(index, item);
		}
		public int IndexOf(ChartElement item) {
			for (int i = 0; i < InnerList.Count; i++)
				if (object.ReferenceEquals(InnerList[i], item))
					return i;
			return -1;
		}
		protected void RemoveWithoutChanged(ChartElement item) {
			int index = IndexOf(item);
			if (index >= 0)
				InnerList.RemoveAt(index);
		}
		protected void Swap(ChartElement item1, ChartElement item2) {
			if (item1 == null)
				throw new ArgumentNullException("item1");
			if (item2 == null)
				throw new ArgumentNullException("item2");
			int index1 = InnerList.IndexOf(item1);
			int index2 = InnerList.IndexOf(item2);
			if (index1 == -1)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "item1");
			if (index2 == -1)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "item2");
			Swap(index1, index2);
		}
		protected virtual void CheckSwapAbility(ChartElement element1, ChartElement element2) {
		}
		protected ChartElement GetElementByID(int id) {
			foreach (ChartElement item in this)
				if (item is ISupportID && ((ISupportID)item).ID == id)
					return item;
			return null;
		}
		public void Remove(ChartElement item) {
			int index = IndexOf(item);
			if (index >= 0) {
				SendControlChanging();
				List.RemoveAt(index);
			}
		}
		public bool Contains(ChartElement item) {
			return IndexOf(item) >= 0;
		}
		public void Swap(int index1, int index2) {
			if (index1 < 0 || index1 >= Count)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "index1");
			if (index2 < 0 || index2 >= Count)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgItemNotInCollection), "index2");
			if (index1 != index2) {
				ChartElement item1 = (ChartElement)List[index1];
				ChartElement item2 = (ChartElement)List[index2];
				CheckSwapAbility(item1, item2);
				SendControlChanging();
				InnerList.RemoveAt(index1);
				InnerList.Insert(index1, item2);
				InnerList.RemoveAt(index2);
				InnerList.Insert(index2, item1);
				RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.SwapItem, item1, index1, item2, index2));
			}
		}
		internal void BeginUpdate(bool forceChanging, bool bindingProcessing) {
			if (forceChanging) {
				SendControlChanging();
				if ((updateModeCounter == 0) && !bindingProcessing)
					OnBeginUpdate();
			}
			updateModeCounter++;
		}
		protected internal virtual void EndUpdate(bool bindingProcessing) {
			DecreaseUpdateCounter();
			if (!UpdateLocked) {
				List<ChartUpdateInfoBase> updateInfos = batchUpdateHelper.CreateUpdateInfos();
				foreach (var updateInfo in updateInfos)
					RaiseControlChanged(updateInfo);
				batchUpdateHelper.Clear();
				if (!bindingProcessing)
					OnEndUpdate();
			}
		}
		public void BeginUpdate() {
			BeginUpdate(true, false);
		}
		public void EndUpdate() {
			EndUpdate(false);
		}
		public void CancelUpdate() {
			DecreaseUpdateCounter();
			batchUpdateHelper.Clear();
		}
		public ChartElement GetElementByIndex(int index) {
			return (ChartElement)List[index];
		}
		public ChartElement[] ToArray() {
			return (ChartElement[])InnerList.ToArray(typeof(ChartElement));
		}
		public virtual void Assign(ChartCollectionBase collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");
			DisposeItems();
			ClearIDs();
			InnerList.Clear();
			RaiseControlChanged(CreateUpdateInfo(ChartCollectionOperation.Reset, null, -1, null, -1));
			foreach (ChartElement obj in collection) {
				ChartElement item = (ChartElement)obj.Clone();
				InnerList.Add(item);
				ChangeOwnerForItem(item);
			}
			RaiseControlChanged(CreateBatchUpdateInfo(ChartCollectionOperation.InsertItem, null, -1, List, 0));
		}
	}
	public abstract class ChartElementNamedCollection : ChartCollectionBase {
		protected abstract string NamePrefix { get; }
		protected ChartElementNamedCollection() : base() {
		}
		protected ChartElementNamedCollection(ChartElement owner) : base(owner) {
		}
		string[] GetNames() {
			string[] names = new string[Count];
			for (int i = 0; i < Count; i++)
				names[i] = ((ChartElementNamed)List[i]).Name;
			return names;
		}
		public new ChartElementNamed[] ToArray() {
			return (ChartElementNamed[])InnerList.ToArray(typeof(ChartElementNamed));
		}
		public string GenerateName() {
			return GenerateName(NamePrefix);
		}
		public string GenerateName(string namePrefix) {
			return NameGenerator.UniqueName(namePrefix, GetNames());
		}
		public ChartElementNamed GetElementByName(string name) {
			foreach (ChartElementNamed obj in this)
				if (obj.Name == name)
					return obj;
			return null;
		}
	}
}
