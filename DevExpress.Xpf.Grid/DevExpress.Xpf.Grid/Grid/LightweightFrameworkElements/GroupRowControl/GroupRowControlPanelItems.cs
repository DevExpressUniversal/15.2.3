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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid.GroupRowLayout {
	public interface IGroupPanelItemOwner {
		void Connect(Column column);
		void Disconnect(Column column);
	}
	public interface IGroupPanelItem {
		IGroupPanelItemOwner Parent { get; set; }
		void OnItemOwnerReplaced(IGroupPanelItemOwner oldValue);
		int Index { get; set; }
	}
	public class GroupPanelVisualItemOwner : IGroupPanelItemOwner {
		readonly Panel panelCore;
		public Panel Panel { get { return panelCore; } }
		UIElementCollection Children { get { return Panel.Children; } }
		public GroupPanelVisualItemOwner(Panel panel) {
			this.panelCore = panel;
		}
		#region IGroupStructureItemOwner Members
		void IGroupPanelItemOwner.Connect(Column column) {
			Children.Add(column.Element);
		}
		void IGroupPanelItemOwner.Disconnect(Column column) {
			Children.Remove(column.Element);
		}
		#endregion
	}
	public abstract class GroupPanelItemContainer<T> : IGroupPanelItem, IGroupPanelItemOwner, IEnumerable<T> where T : IGroupPanelItem {
		SortedList<int, T> children = new SortedList<int, T>();
		public void Add(T child, int index) {
			child.Index = index;
			children.Add(child.Index, child);
			child.Parent = this;
		}
		public void Remove(int index) {
			T child = Get(index);
			if(children == null)
				return;
			child.Index = 0;
			children.Remove(index);
			child.Parent = null;
		}
		public T Get(int index) {
			T child = default(T);
			children.TryGetValue(index, out child);
			return child;
		}
		public int Count { get { return children.Count; } }
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		#endregion
		IEnumerator<T> GetEnumeratorCore() {
			return children.Values.GetEnumerator();
		}
		#region IGroupStructureItem
		IGroupPanelItemOwner ownerCore;
		public IGroupPanelItemOwner Parent {
			get {
				return ownerCore;
			}
			set {
				if(ownerCore != value) {
					IGroupPanelItemOwner oldValue = ownerCore;
					ownerCore = value;
					OnItemOwnerReplaced(oldValue);
				}
			}
		}
		public void OnItemOwnerReplaced(IGroupPanelItemOwner oldValue) {
			foreach(IGroupPanelItem item in this)
				item.OnItemOwnerReplaced(oldValue);
		}
		public int Index { get; set; }
		#endregion
		#region IGroupStructureItemOwner
		void IGroupPanelItemOwner.Connect(Column column) {
			if(Parent != null)
				Parent.Connect(column);
		}
		void IGroupPanelItemOwner.Disconnect(Column column) {
			if(Parent != null)
				Parent.Disconnect(column);
		}
		#endregion
	}
	public class GroupContainer : GroupPanelItemContainer<Group> {
		public Column Get(IndexDefinition index) {
			return GetLayer(index).Return(l => l.Get(index.Column), () => null);
		}
		public void Add(Column column, IndexDefinition index) {
			GetLayer(index).Do(l => l.Add(column, index.Column));
		}
		public void Remove(IndexDefinition index) {
			GetLayer(index).Do(l => l.Remove(index.Column));
		}
		Layer GetLayer(IndexDefinition index) {
			return Get(index.Group).Return(g => g.Get(index.Layer), () => null);
		}
	}
	public class Group : GroupPanelItemContainer<Layer> { }
	public class Layer : GroupPanelItemContainer<Column> {
		public void Add(UIElement element, int index) {
			Add(new Column(element), index);
		}
	}
	public class Column : IGroupPanelItem {
		readonly UIElement elementCore;
		public UIElement Element { get { return elementCore; } }
		public Column(UIElement element) {
			this.elementCore = element;
		}
		IGroupPanelItemOwner ownerCore;
		public IGroupPanelItemOwner Parent {
			get {
				return ownerCore;
			}
			set {
				if(ownerCore != value) {
					IGroupPanelItemOwner oldValue = ownerCore;
					ownerCore = value;
					OnItemOwnerReplaced(oldValue);
				}
			}
		}
		public void OnItemOwnerReplaced(IGroupPanelItemOwner oldValue) {
			if(oldValue != null)
				oldValue.Disconnect(this);
			if(Parent != null)
				Parent.Connect(this);
		}
		public int Index { get; set; }
	}
	public struct IndexDefinition {
		readonly int group;
		readonly int layer;
		readonly int column;
		public int Group { get { return group; } }
		public int Layer { get { return layer; } }
		public int Column { get { return column; } }
		public IndexDefinition(int group, int layer, int column) {
			this.group = group;
			this.layer = layer;
			this.column = column;
		}
	}
}
