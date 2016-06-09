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
using System.Drawing;
using System.Linq;
using System.Reflection;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.Utils.Design {
	internal class ExternalCollectionHelper {
		public ExternalCollectionHelper(IEnumerable collection) {
			InnerCollection = GetCollectionObject(collection);
		}
		protected CollectionWrapper InnerCollection { get; private set; }
		public IEnumerable Collection {
			get { return InnerCollection.ExternalCollection; }
		}
		public bool CanAddNewItem(Type[] newItemType) {
			return InnerCollection.CanAddNewItem(newItemType);
		}
		public void ChangeCollection(InternalCollectionHelper internalHelper) {
			InnerCollection.ChangeCollection(internalHelper);
		}
		public bool Contains(object item) {
			return InnerCollection.Contains(item);
		}
		public bool IsReadOnly {
			get { return InnerCollection.IsReadOnly; }
		}
		CollectionWrapper GetCollectionObject(IEnumerable collection) {
			if(collection != null) {
				if(collection is IList) {
					return new NonGenericIList(collection);
				}
				if(collection.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))) {
					return new GenericICollection(collection);
				}
				return new NonGenericICollection(collection);
			}
			return null;
		}
		#region Collection
		protected abstract class CollectionWrapper {
			public CollectionWrapper(IEnumerable collection) {
				this.ExternalCollection = collection;
			}
			protected Type itemCollectionType;
			IEnumerable collection;
			public IEnumerable ExternalCollection {
				get { return this.collection; }
				private set {
					if(value == null)
						throw new ArgumentException("Empty input collection!");
					collection = value;
				}
			}
			public virtual bool CanAddNewItem(Type[] newItemTypes) {
				if(newItemTypes == null) return false;
				bool isCanAdd = true;
				foreach(var t in newItemTypes) {
					isCanAdd &= itemCollectionType.IsAssignableFrom(t);
				}
				return isCanAdd;
			}
			public abstract void ChangeCollection(InternalCollectionHelper internalHelper);
			public abstract bool Contains(object item);
			public abstract bool IsReadOnly { get; }
		}
		class NonGenericICollection : CollectionWrapper {
			public NonGenericICollection(IEnumerable collection) : base(collection) { }
			public override bool CanAddNewItem(Type[] newItemType) {
				return false;
			}
			public override void ChangeCollection(InternalCollectionHelper internalHelper) {
				return;
			}
			public override bool Contains(object item) {
				foreach(object collectionItem in this.ExternalCollection) {
					if(collectionItem.Equals(item))
						return true;
				}
				return false;
			}
			public override bool IsReadOnly {
				get { return true; }
			}
		}
		class NonGenericIList : CollectionWrapper {
			public NonGenericIList(IEnumerable collection)
				: base(collection) {
				ilistExternalCollection = this.ExternalCollection as IList;
				var properties = ilistExternalCollection.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | BindingFlags.Default);
				foreach(var p in properties) {
					if(p.Name == "Item") {
						itemCollectionType = p.PropertyType;
						break;
					}
				}
			}
			IList ilistExternalCollection;
			public override void ChangeCollection(InternalCollectionHelper internalHelper) {
				ilistExternalCollection.Clear();
				foreach(var item in internalHelper.Collection) {
					ilistExternalCollection.Add(item.Tag);
				}
			}
			public override bool Contains(object item) {
				return ilistExternalCollection.Contains(item);
			}
			public override bool IsReadOnly {
				get { return false; }
			}
		}
		class GenericICollection : CollectionWrapper {
			public GenericICollection(IEnumerable collection)
				: base(collection) {
				itemCollectionType = this.ExternalCollection.GetType().GetGenericArguments()[0];
				genericICollection = this.ExternalCollection.GetType().GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)).FirstOrDefault();
			}
			Type genericICollection;
			protected ICollection<object> GenericCollection { get { return ExternalCollection as ICollection<object>; } }
			public override void ChangeCollection(InternalCollectionHelper internalHelper) {
				ICollection<object> collection = ExternalCollection.Cast<object>() as ICollection<object>;
				if(internalHelper != null) {
					MethodInfo addMethod = genericICollection.GetMethod("Add");
					MethodInfo clearMethod = genericICollection.GetMethod("Clear");
					if(clearMethod != null && addMethod != null) {
						clearMethod.Invoke(this.ExternalCollection, null);
						foreach(var item in internalHelper.Collection) {
							addMethod.Invoke(this.ExternalCollection, new object[] { item.Tag });
						}
					}
				}
			}
			MethodInfo containsMethod = null;
			public override bool Contains(object item) {
				if(containsMethod == null) {
					containsMethod = genericICollection.GetMethod("Contains");
				}
				return containsMethod != null ? (bool)containsMethod.Invoke(this.ExternalCollection, new object[] { item }) : false;
			}
			public override bool IsReadOnly {
				get { return false; }
			}
		}
		#endregion
	}
	internal class InternalCollectionHelper {
		public InternalCollectionHelper(IEnumerable initializeCollection, ICollectionEventsProvider provider, DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
			collection = new List<IDataRepositoryItem>();
			map = new Dictionary<object, IDataRepositoryItem>();
			externalCollectionRemovedItem = new List<IDataRepositoryItem>();
			saveCollection = new List<IDataRepositoryItem>();
			newItems = new List<IDataRepositoryItem>();
			ownerProvider = provider;
			this.itemTypeInfos = itemTypeInfos;
			FillTypeImageMap(itemTypeInfos);
			Refresh(initializeCollection);
		}
		bool isCollectionItemMoved = false;
		List<IDataRepositoryItem> saveCollection;
		List<IDataRepositoryItem> externalCollectionRemovedItem;
		List<IDataRepositoryItem> newItems;
		List<IDataRepositoryItem> collection;
		Dictionary<object, IDataRepositoryItem> map;
		Dictionary<DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo, int> itemTypeInfosMap;
		ICollectionEventsProvider ownerProvider;
		DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos;
		public IEnumerable<IDataRepositoryItem> Collection {
			get { return this.collection; }
		}
		#region Public Methods
		public void MoveItem(IDataRepositoryItem item, int offset) {
			int selectedItemIndex = -1;
			if(item != null) {
				int itemsCount = this.collection.Count;
				if(itemsCount == 1) return;
				selectedItemIndex = this.collection.IndexOf(item);
				int newIndex = selectedItemIndex + offset;
				newIndex = Math.Min(itemsCount - 1, Math.Max(0, newIndex));
				IDataRepositoryItem item2 = this.collection[newIndex];
				if(!RaiseCollectionChanging(CollectionAction.Reorder, item.Tag, item2.Tag)) {
					isCollectionItemMoved = true;
					this.collection.RemoveAt(selectedItemIndex);
					this.collection.Insert(newIndex, item);
					RaiseCollectionChanged(CollectionAction.Reorder, item.Tag, item2.Tag);
				}
			}
		}
		public object OnRemoveItem(object item) {
			if(item == null) return null;
			IDataRepositoryItem removedItem = GetDataRepositoryCore(item);
			if(!this.collection.Contains(item)) return item;
			int removedItemIndex = this.collection.IndexOf(removedItem);
			bool removeResult = false;
			bool flag = IsNewItem(removedItem);
			removeResult = RemoveItem(removedItem, flag);
			if(removeResult) {
				map.Remove(removedItem.Tag);
				int newIndex = removedItemIndex == this.collection.Count ? removedItemIndex - 1 : removedItemIndex;
				var dataRepositoryRemoveItem = this.collection.ElementAtOrDefault(newIndex);
				return dataRepositoryRemoveItem == null ? dataRepositoryRemoveItem : dataRepositoryRemoveItem.Tag;
			}
			return item;
		}
		public void OnAddNewItem(object item) {
			if(item != null && ownerProvider != null) {
				string name = ownerProvider.GetCustomDisplayText(item, "Name");
				string type = item.GetType().Name;
				int itemImageIndex = GetImageIndex(item.GetType());
				IDataRepositoryItem repositoryItem = new DataRepositoryItem(name, type, item, itemImageIndex);
				newItems.Add(repositoryItem);
				map.Add(item, repositoryItem);
				AddItem(repositoryItem);
			}
		}
		public void Reset() {
			this.map.Clear();
			this.externalCollectionRemovedItem.Clear();
			this.collection.Clear();
			saveCollection.Clear();
			this.newItems.Clear();
			this.itemTypeInfosMap.Clear();
		}
		public int Count {
			get {
				return collection.Count;
			}
		}
		public IDataRepositoryItem Find(object item) {
			IDataRepositoryItem returnValue;
			if(item == null) return null;
			map.TryGetValue(item, out returnValue);
			return returnValue;
		}
		public int IndexOf(IDataRepositoryItem item) {
			return collection.IndexOf(item);
		}
		public IDataRepositoryItem this[int index] {
			get {
				return collection[index];
			}
		}
		public List<IDataRepositoryItem> GetRemoveCollection(bool applyChanges) {
			return applyChanges ? externalCollectionRemovedItem : newItems;
		}
		public bool ChangeCollection(bool applyChanges) {
			bool isCollectionChanged = externalCollectionRemovedItem.Count > 0 || newItems.Count > 0;
			var removeCollection = GetRemoveCollection(applyChanges);
			foreach(var item in removeCollection)
				RemoveItem(item, true);
			removeCollection.Clear();
			if(!applyChanges) {
				this.collection.Clear();
				foreach(var savedItem in saveCollection) {
					this.collection.Add(savedItem);
				}
			}
			return applyChanges && (isCollectionItemMoved | isCollectionChanged);
		}
		public void Refresh(IEnumerable collection) {
			this.Reset();
			foreach(object element in collection) {
				OnAddNewItem(element);
			}
			foreach(var item in this.collection)
				saveCollection.Add(item);
			newItems.Clear();
		}
		#endregion
		#region Private Methods
		bool IsNewItem(IDataRepositoryItem removedItem) {
			if(newItems.Contains(removedItem)) {
				newItems.Remove(removedItem);
				return true;
			}
			else {
				externalCollectionRemovedItem.Add(removedItem);
				return false;
			}
		}
		void FillTypeImageMap(DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] itemTypeInfos) {
			itemTypeInfosMap = new Dictionary<DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo, int>();
			if(itemTypeInfos == null) return;
			for(int i = 0; i < itemTypeInfos.Length; i++)
				if(!itemTypeInfosMap.ContainsKey(itemTypeInfos[i]))
					itemTypeInfosMap.Add(itemTypeInfos[i], i);
		}
		int GetImageIndex(Type type) {
			int index = 0;
			if(this.itemTypeInfos != null) {
				var itemTypeInfo = this.itemTypeInfos.FirstOrDefault(ti => ti.Type == type);
				if(itemTypeInfo != null)
					itemTypeInfosMap.TryGetValue(itemTypeInfo, out index);
			}
			return index;
		}
		IDataRepositoryItem GetDataRepositoryCore(object item) {
			IDataRepositoryItem repositoryItem = item as IDataRepositoryItem;
			if(repositoryItem != null) return repositoryItem;
			return this.Find(item);
		}
		void AddItem(IDataRepositoryItem item) {
			if(!RaiseCollectionChanging(CollectionAction.Add, item.Tag, null)) {
				collection.Add(item);
				RaiseCollectionChanged(CollectionAction.Add, item.Tag, null);
			}
		}
		bool RemoveItem(IDataRepositoryItem item, bool isCreatedItem) {
			bool removeResult = false;
			if(!RaiseCollectionChanging(CollectionAction.Remove, item.Tag, null)) {
				removeResult = this.collection.Remove(item);
				RaiseCollectionChanged(CollectionAction.Remove, item.Tag, null, isCreatedItem);
			}
			return removeResult;
		}
		void RaiseCollectionChanged(CollectionAction action, object item, object targetItem, bool isCreatedItem = true) {
			if(ownerProvider != null) {
				CollectionChangedEventArgs changedArgs = new CollectionChangedEventArgs(action, item, targetItem);
				changedArgs.IsCreatedItem = isCreatedItem;
				try { ownerProvider.RaiseCollectionChanged(changedArgs); }
				catch { }
			}
		}
		bool RaiseCollectionChanging(CollectionAction action, object item, object targetItem) {
			CollectionChangingEventArgs changingArgs = new CollectionChangingEventArgs(action, item, targetItem);
			if(ownerProvider != null) {
				try { ownerProvider.RaiseCollectionChanging(changingArgs); }
				catch { }
			}
			return changingArgs.Cancel;
		}
		#endregion
		#region DataRepository
		public interface IDataRepositoryItem {
			string Name { get; }
			string Type { get; }
			object Tag { get; }
			int ImageIndex { get; }
		}
		public class DataRepositoryItem : IDataRepositoryItem {
			public DataRepositoryItem() : this(null) { }
			public DataRepositoryItem(string name) : this(name, null, null) { }
			public DataRepositoryItem(string name, string type) : this(name, type, null) { }
			public DataRepositoryItem(string name, string type, object tag) : this(name, type, tag, 0) { }
			public DataRepositoryItem(string name, string type, object tag, int imageIndex) {
				this.Name = name;
				this.Type = type;
				this.Tag = tag;
				this.ImageIndex = imageIndex;
			}
			public string Name { get; set; }
			public string Type { get; set; }
			public object Tag { get; set; }
			public int ImageIndex { get; set; }
		}
		#endregion
	}
}
