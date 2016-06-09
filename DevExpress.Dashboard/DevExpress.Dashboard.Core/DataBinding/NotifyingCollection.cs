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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
namespace DevExpress.DataAccess {
	public class NotifyingCollection<T> : Collection<T>, IUpdateLocker where T : class {
		class ChangeCache {
			readonly Dictionary<T, int> items = new Dictionary<T, int>();
			public void AddItem(T item) {
				if(!items.ContainsKey(item))
					items.Add(item, 1);
				else
					items[item]++;
			}
			public void RemoveItem(T item) {
				if(!items.ContainsKey(item))
					items.Add(item, -1);
				else
					items[item]--;
			}
			public void GetAddedRemovedItems(out List<T> addedItems, out List<T> removedItems) {
				addedItems = new List<T>();
				removedItems = new List<T>();
				foreach(KeyValuePair<T, int> pair in items) {
					if(pair.Value > 0)
						addedItems.Add(pair.Key);
					else if(pair.Value < 0)
						removedItems.Add(pair.Key);
				}
			}
			public void Clear() {
				items.Clear();
			}
		}
		readonly Locker locker = new Locker();
		readonly ChangeCache changeCache = new ChangeCache();
		ListXmlSerializer<T> xmlSerializer;
		protected internal ListXmlSerializer<T> XmlSerializer {
			get { return xmlSerializer; }
			set {
				if(value != xmlSerializer) {
					if(xmlSerializer != null)
						xmlSerializer.List = null;
					xmlSerializer = value;
					if(xmlSerializer != null)
						xmlSerializer.List = this;
				}
			}
		}
		public event EventHandler<NotifyingCollectionChangedEventArgs<T>> CollectionChanged;
		public event EventHandler<NotifyingCollectionBeforeItemAddedEventArgs<T>> BeforeItemAdded;
		public void AddRange(params T[] items) {
			AddRange((IEnumerable<T>)items);
		}
		public void AddRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, "items");
			BeginUpdate();
			try {
				foreach(T item in items)
					Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public void RemoveRange(params T[] items) {
			RemoveRange((IEnumerable<T>)items);
		}
		public void RemoveRange(IEnumerable<T> items) {
			Guard.ArgumentNotNull(items, "items");
			BeginUpdate();
			try {
				foreach(T item in items)
					Remove(item);
			}
			finally {
				EndUpdate();
			}
		}
		public void BeginUpdate() {
			locker.Lock();
		}
		public void EndUpdate() {
			locker.Unlock();
			OnChanged();
		}
		public T[] ToArray() {
			T[] array = new T[Count];
			CopyTo(array, 0);
			return array;
		}
		public bool Contains(Predicate<T> match) {
			return FindFirst(match) != null;
		}
		public T FindFirst(Predicate<T> match) {
			foreach(T item in this)
				if(match(item))
					return item;
			return null;
		}
		public void ForEach(Action<T> action) {
			Guard.ArgumentNotNull(action, "action");
			for(int i = 0; i < this.Count; i++)
				action(this[i]);
		}
		protected internal virtual void SaveToXml(XElement element) {
			if(xmlSerializer != null)
				xmlSerializer.SaveToXml(element);
		}
		protected internal virtual void LoadFromXml(XElement element) {
			if(xmlSerializer != null)
				xmlSerializer.LoadFromXml(element);
		}
		protected override void InsertItem(int index, T item) {
			BeforeAddItem(item);
			base.InsertItem(index, item);
			OnChanged();
		}
		protected override void RemoveItem(int index) {
			BeforeRemoveItem(this[index]);
			base.RemoveItem(index);
			OnChanged();
		}
		protected override void ClearItems() {
			foreach(T item in this)
				BeforeRemoveItem(item);
			bool changed = Count > 0;
			base.ClearItems();
			if(changed)
				OnChanged();
		}
		protected override void SetItem(int index, T item) {
			if(object.ReferenceEquals(this[index], item))
				return;
			BeforeAddItem(item);
			BeforeRemoveItem(this[index]);
			base.SetItem(index, item);
			OnChanged();
		}
		void BeforeAddItem(T item) {
			if(item == null)
				throw new InvalidOperationException(DataAccessLocalizer.GetString(DataAccessStringId.MessageNullItem));
			if(Contains(item))
				throw new InvalidOperationException(String.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageDuplicateItem), item));
			if(BeforeItemAdded != null)
				BeforeItemAdded(this, new NotifyingCollectionBeforeItemAddedEventArgs<T>(item));
			changeCache.AddItem(item);
		}
		void BeforeRemoveItem(T item) {
			changeCache.RemoveItem(item);
		}
		void OnChanged() {
			if(!locker.IsLocked) {
				if(CollectionChanged != null) {
					List<T> addedItems;
					List<T> removedItems;
					changeCache.GetAddedRemovedItems(out addedItems, out removedItems);
					CollectionChanged(this, new NotifyingCollectionChangedEventArgs<T>(addedItems, removedItems));
				}
				changeCache.Clear();
			}
		}
	}
	public class NotifyingCollectionChangedEventArgs<T> : EventArgs {
		public IList<T> AddedItems { get; private set; }
		public IList<T> RemovedItems { get; private set; }
		public NotifyingCollectionChangedEventArgs(IList<T> addedItems, IList<T> removedItems) {
			AddedItems = addedItems;
			RemovedItems = removedItems;
		}
	}
	public class NotifyingCollectionBeforeItemAddedEventArgs<T> : EventArgs {
		public T Item { get; private set; }
		public NotifyingCollectionBeforeItemAddedEventArgs(T item) {
			Item = item;
		}
	}
	[Obsolete("The UniqueItemCollection class in now obsolete. Use the NotifyingCollection class instead.")]
	public class UniqueItemCollection<T> : NotifyingCollection<T> where T : class {
	}
}
