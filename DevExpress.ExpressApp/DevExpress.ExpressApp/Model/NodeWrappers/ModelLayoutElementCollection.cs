#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Model.NodeWrappers {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelBandLayoutItemCollection : ModelCollection<IModelBandedLayoutItem> {
		public ModelBandLayoutItemCollection(ModelLayoutElementCollection collection)
			: base(collection.GetItems<IModelBandedLayoutItem>()) {
		}
		public ModelBandLayoutItemCollection(IEnumerable<IModelBandedLayoutItem> collection)
			: base(collection) {
		}
		public ModelBandLayoutItemCollection(IModelBandsLayout bandsLayout, IModelColumns columns)
			: base(GetBandedItems(bandsLayout, columns)) {
		}
		public ModelBandLayoutItemCollection(IModelNode node)
			: base(GetBandedItems(node)) {
		}
		private static IEnumerable<IModelBandedLayoutItem> GetBandedItems(IModelBandsLayout bandsLayout, IModelColumns columns) {
			List<IModelBandedLayoutItem> result = new List<IModelBandedLayoutItem>();
			if(columns != null) {
				foreach(IModelBandedColumn column in columns) {
					result.Add(column);
				}
			}
			if(bandsLayout != null) {
				result.AddRange(bandsLayout);
			}
			return result;
		}
		private static IEnumerable<IModelBandedLayoutItem> GetBandedItems(IModelNode node) {
			IModelListView listViewModel = GetListViewModel(node);
			if(listViewModel != null) {
				return GetBandedItems(listViewModel.BandsLayout, listViewModel.Columns);
			}
			return new List<IModelBandedLayoutItem>();
		}
		private static IModelListView GetListViewModel(IModelNode node) {
			IModelListView listViewModel = null;
			if(node != null) {
				if(node is IModelListView) {
					listViewModel = (IModelListView)node;
				}
				else {
					if(node is IModelBandsLayout) {
						listViewModel = (IModelListView)node.Parent;
					}
					else {
						if(node is IModelColumns) {
							listViewModel = (IModelListView)node.Parent;
						}
						else {
							if(node is IModelBandedLayoutItem && node.Parent != null) {
								listViewModel = (IModelListView)node.Parent.Parent;
							}
							else {
								if(node is IModelColumn && node.Parent != null) {
									listViewModel = (IModelListView)node.Parent.Parent;
								}
							}
						}
					}
				}
			}
			return listViewModel;
		}
		protected virtual ModelBandLayoutItemCollection CreateInstance(IEnumerable<IModelBandedLayoutItem> collection) {
			return new ModelBandLayoutItemCollection(collection);
		}
		public IList<IModelBandedLayoutItem> GetAllVisibleItems() {
			List<IModelBandedLayoutItem> visibleItems = new List<IModelBandedLayoutItem>();
			foreach(IModelBandedLayoutItem item in this) {
				if(!item.Index.HasValue || item.Index > -1) {
					visibleItems.Add(item);
				}
			}
			return visibleItems;
		}
		public IList<IModelBandedLayoutItem> GetVisibleChildren(IModelNode model) {
			List<IModelBandedLayoutItem> visibleChildren = new List<IModelBandedLayoutItem>();
			foreach(IModelBandedLayoutItem item in GetChildren(model)) {
				if(!item.Index.HasValue || item.Index > -1) {
					visibleChildren.Add(item);
				}
			}
			return visibleChildren;
		}
		protected virtual IModelBandedLayoutItem GetParentItem(IModelBandedLayoutItem item) {
			return item.OwnerBand;
		}
		protected virtual bool IsChild(IModelBandedLayoutItem item, IModelLayoutElement parent) {
			bool result = false;
			IModelLayoutElement parentItem = GetParentItem(item);
			if(parent == null || parent is IModelBandsLayout) {
				result = parentItem == null;
			}
			else {
				result = parentItem != null ? parentItem.Id == parent.Id : false;
			}
			return result;
		}
		public ModelBandLayoutItemCollection GetChildren(IModelNode parent) {
			List<IModelBandedLayoutItem> innerList = new List<IModelBandedLayoutItem>();
			IModelLayoutElement parentBand = parent as IModelLayoutElement;
			foreach(IModelBandedLayoutItem item in this) {
				if(IsChild(item, parentBand)) {
					innerList.Add(item);
				}
			}
			return CreateInstance(innerList);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelLayoutElementCollection : ModelCollection<IModelLayoutElement> {
		public ModelLayoutElementCollection(IEnumerable<IModelLayoutElement> collection)
			: base(collection) {
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelCollection<T> : IEnumerable<T> where T : IModelLayoutElement {
		List<T> innerList = null;
		Dictionary<string, T> innerDictionary = null;
		public ModelCollection(IEnumerable<T> collection) {
			innerList = new List<T>(collection);
			Sort(new ModelLayoutElementNodesComparer<T>(false));
			innerDictionary = new Dictionary<string, T>(innerList.Count);
			foreach(T item in innerList) {
				innerDictionary[item.Id] = item;
			}
		}
		public void Sort(IComparer<T> comparer) {
			innerList.Sort(comparer);
		}
		public int Count {
			get {
				return innerList.Count;
			}
		}
		public int IndexOf(string itemId) {
			return innerList.IndexOf(this[itemId]);
		}
		public void Remove(string id, bool removeFromModel) {
			T model = this[id];
			if(model != null) {
				if(removeFromModel) {
					model.Remove();
				}
				innerList.Remove(model);
				innerDictionary.Remove(id);
			}
		}
		public IEnumerator<T> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		public T this[int index] {
			get {
				return innerList[index];
			}
		}
		public T this[string id] {
			get {
				T result;
				innerDictionary.TryGetValue(id, out result);
				return result;
			}
		}
		public IList<D> GetItems<D>() where D : T {
			List<D> result = new List<D>();
			foreach(T item in innerList) {
				if(item is D) {
					result.Add((D)item);
				}
			}
			return result;
		}
	}
}
