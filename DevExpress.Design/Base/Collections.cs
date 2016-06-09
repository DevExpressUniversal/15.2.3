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
using System.Collections.Specialized;
using System.Windows.Data;
namespace DevExpress.Design.ComponentModel {
	public class PropertySortDescription {
		public PropertySortDescription(string propertyName, System.ComponentModel.ListSortDirection direction) {
			PropertyName = propertyName;
			Direction = direction;
		}
		public string PropertyName { get; private set; }
		public System.ComponentModel.ListSortDirection Direction { get; private set; }
		public string DisplayName {
			get { return this.ToDisplayName(); }
		}
		public void InvertDirection() {
			Direction = (Direction == System.ComponentModel.ListSortDirection.Ascending) ?
				System.ComponentModel.ListSortDirection.Descending : System.ComponentModel.ListSortDirection.Ascending;
		}
	}
	public class PropertySortDescriptionCollection : System.Collections.ObjectModel.Collection<PropertySortDescription>, INotifyCollectionChanged {
		protected override void ClearItems() {
			base.ClearItems();
			OnCollectionChanged(NotifyCollectionChangedAction.Reset);
		}
		protected override void InsertItem(int index, PropertySortDescription item) {
			base.InsertItem(index, item);
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		protected override void RemoveItem(int index) {
			PropertySortDescription item = base[index];
			base.RemoveItem(index);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}
		protected override void SetItem(int index, PropertySortDescription item) {
			PropertySortDescription description = base[index];
			base.SetItem(index, item);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, description, index);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		NotifyCollectionChangedEventHandler CollectionChanged;
		void OnCollectionChanged(NotifyCollectionChangedAction action) {
			if(CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
		}
		void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
			if(CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
		}
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(CollectionChanged, value); }
			remove { CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(CollectionChanged, value); }
		}
		public bool Contains(string propertyName) {
			return Find(propertyName) != null;
		}
		public void Remove(string propertyName) {
			var item = Find(propertyName);
			if(item != null)
				Items.Remove(item);
		}
		protected PropertySortDescription Find(string propertyName) {
			foreach(PropertySortDescription pd in Items) {
				if(pd.PropertyName == propertyName) return pd;
			}
			return null;
		}
		public void InvertDirection(PropertySortDescription item) {
			if(!Items.Contains(item)) return;
			int index = Items.IndexOf(item);
			item.InvertDirection();
			OnCollectionChanged(NotifyCollectionChangedAction.Reset);
		}
	}
	public class PropertyGroupDescriptionCollection : System.Collections.ObjectModel.Collection<PropertyGroupDescription>, INotifyCollectionChanged {
		protected override void ClearItems() {
			base.ClearItems();
			OnCollectionChanged(NotifyCollectionChangedAction.Reset);
		}
		protected override void InsertItem(int index, PropertyGroupDescription item) {
			base.InsertItem(index, item);
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		protected override void RemoveItem(int index) {
			PropertyGroupDescription item = base[index];
			base.RemoveItem(index);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
		}
		protected override void SetItem(int index, PropertyGroupDescription item) {
			PropertyGroupDescription description = base[index];
			base.SetItem(index, item);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, description, index);
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		NotifyCollectionChangedEventHandler CollectionChanged;
		void OnCollectionChanged(NotifyCollectionChangedAction action) {
			if(CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
		}
		void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) {
			if(CollectionChanged != null)
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
		}
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(CollectionChanged, value); }
			remove { CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(CollectionChanged, value); }
		}
		public bool Contains(string propertyName) {
			return Find(propertyName) != null;
		}
		public void Add(string propertyName) {
			var item = new PropertyGroupDescription(propertyName);
			int index = Items.Count;
			Items.Add(item);
			OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}
		public void Remove(string propertyName) {
			var item = Find(propertyName);
			if(item != null) {
				int index = Items.IndexOf(item);
				if(Items.Remove(item)) {
					OnCollectionChanged(NotifyCollectionChangedAction.Remove, item, index);
				}
			}
		}
		protected PropertyGroupDescription Find(string propertyName) {
			foreach(PropertyGroupDescription pd in Items) {
				if(pd.PropertyName == propertyName) return pd;
			}
			return null;
		}
	}
	static class SortStringExtension {
		public static string ToSortString(this PropertySortDescription description) {
			return description.PropertyName + " " + ToSortString(description.Direction);
		}
		public static string ToDisplayName(this PropertySortDescription description) {
			return description.PropertyName + " (" + ToSortString(description.Direction) + ")";
		}
		public static string ToSortString(this System.ComponentModel.ListSortDirection direction) {
			return direction == System.ComponentModel.ListSortDirection.Ascending ? "ASC" : "DESC";
		}
		public static string ToSortString(this IEnumerable<PropertySortDescription> sortDescriptions) {
			var sb = new System.Text.StringBuilder();
			foreach(PropertySortDescription description in sortDescriptions) {
				if(sb.Length > 0)
					sb.Append(",");
				sb.Append(ToSortString(description));
			}
			return sb.ToString();
		}
	}
}
