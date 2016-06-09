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
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Localization;
namespace DevExpress.XtraBars {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter)), Serializable]
	public class BarManagerCategory : MarshalByRefObject, ICloneable, ISerializable {
		Guid guid;
		BarManagerCategoryCollection collection = null;
		string name;
		bool visible;
		static BarManagerCategory defaultCategory, totalCategory;
		static BarManagerCategory() {
			defaultCategory = new BarManagerCategory(BarLocalizer.Active.GetLocalizedString(BarString.BarUnassignedItems), new Guid("{F96A77CC-41A0-4295-8F5A-5C2410A21EF7}"));
			totalCategory = new BarManagerCategory(BarLocalizer.Active.GetLocalizedString(BarString.BarAllItems), new Guid("{6FFDDB2B-9015-4d97-A4C1-91613E0EF537}"));
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCategoryDefaultCategory")]
#endif
		public static BarManagerCategory DefaultCategory { get { return defaultCategory; } }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerCategoryTotalCategory"),
#endif
 EditorBrowsable(EditorBrowsableState.Never)]
		public static BarManagerCategory TotalCategory { get { return totalCategory; } }
		public BarManagerCategory()
			: this("category", Guid.NewGuid()) {
		}
		public BarManagerCategory(string name) : this(name, Guid.NewGuid()) { }
		public BarManagerCategory(string name, Guid guid) {
			this.guid = guid;
			this.name = name;
			this.visible = true;
		}
		public BarManagerCategory(string name, Guid guid, bool visible) {
			this.name = name;
			this.guid = guid;
			this.visible = visible;
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerCategoryVisible"),
#endif
 Localizable(true), DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnVisibleChanged();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BarManagerCategoryName"),
#endif
 Localizable(true)]
		public string Name {
			get { return name; }
			set {
				if(value == null) value = "";
				if(Name == value) return;
				if(Collection == null) {
					this.name = value;
					return;
				}
				OnNameChanging(Name, value);
			}
		}
		[Browsable(false)]
		public Guid Guid {
			get { return guid; }
			set {
				throw new ArgumentException("'Guid' property can't be changed at runtime");
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Index { get { return Collection == null ? -1 : Collection.IndexOf(this); } }
		public override string ToString() {
			return Name;
		}
		protected BarManagerCategoryCollection Collection { get { return collection; } }
		protected virtual void OnVisibleChanged() {
			if(Collection != null) Collection.OnCategoryVisibleChanged(this);
		}
		protected virtual void OnNameChanging(string oldName, string newName) {
			if(Collection != null) Collection.OnCategoryNameChanging(this, oldName, newName);
		}
		protected internal void SetCollection(BarManagerCategoryCollection collection) {
			this.collection = collection;
		}
		protected internal void SetName(string name) {
			this.name = name;
		}
		protected BarManager Manager {
			get { return Collection == null ? null : Collection.Manager; }
		}
		public int GetItemCount() {
			if(Manager == null) return 0;
			int count = 0;
			for(int n = 0; n < Manager.Items.Count; n++) {
				if(Manager.Items[n].CategoryGuid == this.Guid) count++;
			}
			return count;
		}
		public BarItem GetItem(int index) {
			if(Manager == null) return null;
			int count = 0;
			for(int n = 0; n < Manager.Items.Count; n++) {
				BarItem item = Manager.Items[n];
				if(item.CategoryGuid == this.Guid) {
					if(count++ == index) return item;
				}
			}
			return null;
		}
		object ICloneable.Clone() {
			BarManagerCategory category = new BarManagerCategory(Name, Guid, Visible);
			return category;
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			si.AddValue("Name", Name);
			si.AddValue("Guid", Guid);
			si.AddValue("Visible", Visible);
		}
		internal BarManagerCategory(SerializationInfo si, StreamingContext context) {
			foreach(SerializationEntry entry in si) {
				switch(entry.Name) {
					case "Name":
						this.name = (string)entry.Value;
						break;
					case "Visible":
						this.visible = (bool)entry.Value;
						break;
					case "Guid":
						this.guid = (Guid)entry.Value;
						break;
				}
			}
		}
	}
	[ListBindable(false)]
	public class BarManagerCategoryCollection : CollectionBase {
		bool shouldUpdateItemsAfterLoad = false; 
		BarManager manager;
		Hashtable guids;
		Hashtable names;
		public event CollectionChangeEventHandler CollectionChange;
		public BarManagerCategoryCollection(BarManager manager) {
			this.manager = manager;
			this.guids = new Hashtable();
			this.names = new Hashtable();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BarManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCategoryCollectionItem")]
#endif
		public BarManagerCategory this[int index] { get { return List[index] as BarManagerCategory; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCategoryCollectionItem")]
#endif
		public BarManagerCategory this[string name] { get { return Names[name] as BarManagerCategory; } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("BarManagerCategoryCollectionItem")]
#endif
		public BarManagerCategory this[Guid guid] {
			get {
				if(guid.Equals(BarManagerCategory.DefaultCategory.Guid)) return BarManagerCategory.DefaultCategory;
				return Guids[guid] as BarManagerCategory;
			}
		}
		public int IndexOf(string categoryName) { return IndexOf(this[categoryName]); }
		public int IndexOf(BarManagerCategory category) { return List.IndexOf(category); }
		public virtual BarManagerCategory Add(string name) {
			if(Manager.IsLoading) this.shouldUpdateItemsAfterLoad = true;
			BarManagerCategory cat = new BarManagerCategory(name);
			Add(cat);
			return cat;
		}
		internal BarManagerCategory Replace(BarManagerCategory category) {
			int index = IndexOf(category);
			if(index >= 0) {
				category = ((ICloneable)category).Clone() as BarManagerCategory;
				InnerList[index] = category;
				OnInsertComplete(index, category);
			}
			return category;
		}
		public virtual void AddRange(BarManagerCategory[] categories) {
			foreach(BarManagerCategory category in categories) Add(category);
		}
		public virtual int Add(BarManagerCategory category) {
			return List.Add(category);
		}
		public virtual void Insert(int position, BarManagerCategory category) {
			List.Insert(position, category);
		}
		public virtual void Remove(BarManagerCategory category) {
			List.Remove(category);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		protected Hashtable Names { get { return names; } }
		protected Hashtable Guids { get { return guids; } }
		protected override void OnInsert(int position, object item) {
			BarManagerCategory category = item as BarManagerCategory;
			if(category == null) throw new ArgumentException("Category can't be null");
			if(Names[category.Name] != null) throw new ArgumentException("Category 'Name' should be unique");
			if(Guids[category.Guid] != null) throw new ArgumentException("Category 'Guid' should be unique");
		}
		protected override void OnInsertComplete(int position, object item) {
			BarManagerCategory category = item as BarManagerCategory;
			category.SetCollection(this);
			Names[category.Name] = category;
			Guids[category.Guid] = category;
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, category));
		}
		protected override void OnRemoveComplete(int position, object item) {
			BarManagerCategory category = item as BarManagerCategory;
			category.SetCollection(null);
			Names.Remove(category.Name);
			Guids.Remove(category.Guid);
			RemoveCategoryItems(category);
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, category));
		}
		protected virtual void RemoveCategoryItems(BarManagerCategory category) {
			if(Manager == null) return;
			foreach(BarItem item in Manager.Items) {
				if(item.CategoryGuid == category.Guid)
					item.CategoryGuid = BarManagerCategory.DefaultCategory.Guid;
			}
		}
		protected internal virtual void OnCategoryNameChanging(BarManagerCategory category, string oldName, string newName) {
			if(Names.ContainsKey(newName)) throw new ArgumentException("Category 'Name' should be unique");
			Names.Remove(category.Name);
			category.SetName(newName);
			Names[newName] = category;
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, category));
		}
		protected internal virtual void OnCategoryVisibleChanged(BarManagerCategory category) {
			OnChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, category));
		}
		protected virtual void OnChanged(CollectionChangeEventArgs e) {
			if(CollectionChange != null) CollectionChange(this, e);
		}
		protected internal virtual void OnLoaded() {
			if(!this.shouldUpdateItemsAfterLoad) return;
			for(int n = Manager.Items.Count - 1; n >= 0; n--) {
				BarItem item = Manager.Items[n];
				int index = item.categoryIndex;
				BarManagerCategory cat = (index == -1 || index >= Manager.Categories.Count ? BarManagerCategory.DefaultCategory : Manager.Categories[index]);
				item.SetCategory(cat.Guid);
			}
		}
	}
}
