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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Collections;
using System.Drawing;
using System.Data;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Handler;
using DevExpress.XtraTreeList.Painter;
using System.Collections.Generic;
using DevExpress.XtraEditors.Customization;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using System.Linq;
namespace DevExpress.XtraTreeList.Columns {
	[ToolboxItem(false), DesignTimeVisible(false), Designer("DevExpress.XtraTreeList.Design.TreeListBandDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class TreeListBand : Component, IHeaderObject, IAppearanceOwner, IXtraSerializableLayoutEx {
		protected const int LayoutIdAppearance = 1;
		public const int DefaultBandWidth = 75, DefaultBandMinWidth = 20, DefaultMinimumMinBandWidth = 16;
		string name, caption, customizationCaption;
		bool visible;
		int minWidth, width;
		TreeListBandCollection ownedCollection;
		AppearanceObject appearanceHeader;
		FixedStyle fixedStyle;
		TreeListOptionsBand optionsBand;
		int rowCount;
		public TreeListBand() {
			this.name = this.caption = this.customizationCaption = string.Empty;
			this.visible = true;
			this.width = this.VisibleWidth = DefaultBandWidth;
			this.minWidth = DefaultBandMinWidth;
			this.fixedStyle = FixedStyle.None;
			this.rowCount = 1;
			this.appearanceHeader = new AppearanceObject(this, false);
			this.appearanceHeader.Changed += OnAppearanceChanged;
			this.optionsBand = CreateOptionsBand();
			this.optionsBand.Changed += OnOptinsBandChanged;
			Bands = CreateBandCollection();
			Bands.CollectionChanged += OnBandCollectionChanged;
			Columns = CreateBandColumnCollection();
			Columns.CollectionChanged += OnBandColumnCollectionChanged;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(true, true, true)]
		public TreeListBandCollection Bands { get; private set; }
		internal void XtraClearBands(XtraItemEventArgs e) { Bands.ClearBandItems(e); }
		internal object XtraCreateBandsItem(XtraItemEventArgs e) { return Bands.CreateBandItem(e); }
		internal object XtraFindBandsItem(XtraItemEventArgs e) { return Bands.FindBandItem(e); }
		internal void XtraSetIndexBandsItem(XtraSetItemIndexEventArgs e) { Bands.SetBandItemIndex(e); }
		[Browsable(false)]
		public bool HasChildren { get { return Bands.Count > 0; } }
		[Browsable(false)]
		public bool HasVisibleChildren { get { return Bands.VisibleCount > 0; } }
		[Browsable(false)]
		public int Index {
			get {
				if(OwnedCollection == null) return -1;
				return OwnedCollection.IndexOf(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.NameCollection, true, true, true)]
		public TreeListBandColumnCollection Columns { get; private set; }
		[Browsable(false)]
		public TreeListBand ParentBand { get; private set; }
		[Browsable(false)]
		public virtual TreeListBand RootBand {
			get {
				TreeListBand band = this;
				while(band.ParentBand != null)
					band = band.ParentBand;
				return band;
			}
		}
		[Browsable(false)]
		public TreeList TreeList { get { return OwnedCollection != null ? OwnedCollection.TreeList : null; } }
		bool ShouldSerializeAppearanceHeader() { return AppearanceHeader.ShouldSerialize(); }
		void ResetAppearanceHeader() { AppearanceHeader.Reset(); }
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public AppearanceObject AppearanceHeader {
			get { return appearanceHeader; }
		}
		bool ShouldSerializeOptionsBand() { return OptionsBand.ShouldSerializeCore(this); }
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsBand OptionsBand { get { return optionsBand; } }
		[DXCategory(CategoryName.Appearance), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				OnChanged();
			}
		}
		public string GetTextCaption() {
			if(TreeList == null) return Caption;
			return TreeList.GetNonFormattedCaption(Caption);
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string CustomizationCaption {
			get { return customizationCaption; }
			set {
				if(value == null) value = string.Empty;
				if(CustomizationCaption == value) return;
				customizationCaption = value;
				OnChanged();
			}
		}
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string Name {
			get {
				if(Site != null)
					name = Site.Name;
				return name;
			}
			set {
				if(value == null)
					value = string.Empty;
				name = value;
				if(Site != null)
					Site.Name = name;
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(true), XtraSerializableProperty(), Localizable(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value) return;
				visible = value;
				OnChanged();
			}
		}
		[Browsable(false)]
		public bool ActualVisible {
			get {
				if(!Visible) return false;
				TreeListBand parentBand = ParentBand;
				while(parentBand != null) {
					if(!parentBand.Visible) return false;
					parentBand = parentBand.ParentBand;
				}
				return true;
			}
		}
		[DefaultValue(DefaultBandMinWidth), XtraSerializableProperty(), Localizable(true)]
		public int MinWidth {
			get {
				if(TreeList == null || TreeList.IsLoading) return minWidth;
				return Math.Max(minWidth, TreeList.GetBandIndent(this) + DefaultMinimumMinBandWidth);
			}
			set {
				if(value < DefaultMinimumMinBandWidth)
					value = DefaultMinimumMinBandWidth;
				if(MinWidth == value) return;
				minWidth = value;
				if(Width < MinWidth)
					Width = MinWidth;
				else
					OnChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(FixedStyle.None), XtraSerializableProperty(), Localizable(true)]
		public virtual FixedStyle Fixed {
			get { return fixedStyle; }
			set {
				if(Fixed == value) return;
				fixedStyle = value;
				if(!IsLoading && TreeList != null)
					TreeList.OnBandFixedChanged(this);
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(DefaultBandWidth), XtraSerializableProperty(), Localizable(true)]
		public int Width {
			get { return width; }
			set {
				if(value < MinWidth)
					value = MinWidth;
				if(Width == value) return;
				width = value;
				SetVisibleWidth(width);
				if(!IsLoading)
					TreeList.OnBandWidthChanged(this);
				OnChanged();
			}
		}
		[Browsable(false)]
		public int VisibleWidth { get; private set; }
		[Browsable(false)]
		public bool IsLoading { get { return TreeList == null || TreeList.IsLoading; } }
		[Browsable(false)]
		public virtual int Level {
			get {
				TreeListBand band = this;
				int level = 0;
				while(band.ParentBand != null) { level++; band = band.ParentBand; }
				return level;
			}
		}
		protected virtual TreeListOptionsBand CreateOptionsBand() { return new TreeListOptionsBand(); }
		protected internal TreeListBandCollection OwnedCollection {
			get { return ownedCollection; }
			set {
				if(OwnedCollection == value) return;
				ownedCollection = value;
				OnOwnedCollectionChanged();
			}
		}
		protected internal string GetCustomizationCaption() {
			if(CustomizationCaption == string.Empty) return Caption;
			return CustomizationCaption;
		}
		protected internal void SetVisibleWidth(int value) {
			VisibleWidth = value;
		}
		protected virtual void OnOwnedCollectionChanged() {
			ParentBand = OwnedCollection != null ? OwnedCollection.OwnerBand : null;
			Bands.TreeList = TreeList;
			UpdateChildren(Bands, (band) => { band.Bands.TreeList = TreeList; });
		}
		protected void UpdateChildren(TreeListBandCollection bands, Action<TreeListBand> action) {
			foreach(TreeListBand band in bands) {
				action(band);
				UpdateChildren(band.Bands, action);
			}
		}
		protected virtual TreeListBandCollection CreateBandCollection() {
			return new TreeListBandCollection(this);
		}
		protected virtual TreeListBandColumnCollection CreateBandColumnCollection() {
			return new TreeListBandColumnCollection(this);
		}
		protected virtual void OnOptinsBandChanged(object sender, BaseOptionChangedEventArgs e) {
			OnChanged();
		}
		protected virtual void OnChanged() {
			if(IsLoading) return;
			if(TreeList != null)
				TreeList.OnBandChanged(this);
		}
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		protected virtual void OnBandColumnCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(TreeList != null)
				TreeList.OnBandColumnCollectionChanged(sender, e);
		}
		protected virtual void OnBandCollectionChanged(object sender, CollectionChangeEventArgs e) {
			TreeListBand band = e.Element as TreeListBand;
			if(e.Action == CollectionChangeAction.Add)
				MoveColumns(band);
			if(TreeList != null)
				TreeList.OnBandCollectionChanged(sender, e);
		}
		protected virtual void MoveColumns(TreeListBand band) {
			if(Columns.Count == 0) return;
			TreeListColumn[] columns = new TreeListColumn[Columns.Count];
			((IList)Columns).CopyTo(columns, 0);
			foreach(TreeListColumn col in columns)
				band.Columns.Add(col);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Columns.Clear();
				Bands.Clear();
				if(OwnedCollection != null)
					OwnedCollection.Remove(this);
				this.optionsBand.Changed -= OnOptinsBandChanged;
				this.appearanceHeader.Changed -= OnAppearanceChanged;
			}
			base.Dispose(disposing);
		}
		bool IHeaderObject.FixedWidth { get { return OptionsBand.FixedWidth; } }
		IList IHeaderObject.Children { get { return Bands; } }
		IList IHeaderObject.Columns { get { return Columns; } }
		IHeaderObject IHeaderObject.Parent { get { return null; } }
		void IHeaderObject.SetWidth(int width, bool onlyVisibleWidth) {
			SetVisibleWidth(width);
			if(onlyVisibleWidth) return;
			this.width = width;
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(1), XtraSerializableProperty(), Localizable(true)]
		public int RowCount {
			get { return rowCount; }
			set {
				if(value < 1) value = 1;
				if(value > 10) value = 10;
				if(RowCount == value) return;
				rowCount = value;
				OnChanged();
			}
		}
		protected internal virtual bool AutoFill { get { return true; } }
		public override string ToString() {
			return Caption;
		}
		#region Assign
		protected internal void Assign(TreeListBand band) {
			this.customizationCaption = band.CustomizationCaption;
			this.caption = band.Caption;
			this.fixedStyle = band.Fixed;
			this.minWidth = band.MinWidth;
			try {
				this.Name = band.Name;
			}
			catch { }
			this.visible = band.Visible;
			this.width = band.Width;
			this.rowCount = band.RowCount;
			this.appearanceHeader.AssignInternal(band.AppearanceHeader);
			this.OptionsBand.Assign(band.OptionsBand);
			((IHeaderObjectCollection<TreeListBand>)Bands).Synchronize(band.Bands);
			((IHeaderObjectCollection<TreeListColumn>)Columns).Synchronize(band.Columns);
		}
		#endregion
		#region Serialization
		internal void XtraClearColumns(XtraItemEventArgs e) {
			OptionsLayoutTreeList opt = e.Options as OptionsLayoutTreeList;
			bool addNewColumns = (opt != null && opt.AddNewColumns);
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				if(!addNewColumns) Columns.Clear();
				return;
			}
			List<object> list = new List<object>();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo property in e.Item.ChildProperties) {
				object col = XtraFindColumnsItem(new XtraItemEventArgs(this, Columns, property));
				if(col != null) list.Add(col);
			}
			for(int i = Columns.Count - 1; i >= 0; i--) {
				TreeListColumn column = Columns[i];
				if(!list.Contains(column)) {
					if(addNewColumns) continue;
					Columns.RemoveAt(i);
				}
				else
					Columns.RemoveAt(i);
			}
		}
		internal object XtraCreateColumnsItem(XtraItemEventArgs e) { return null; }
		internal object XtraFindColumnsItem(XtraItemEventArgs e) {
			if(e.Item.Value == null) return null;
			string name = e.Item.Value.ToString();
			if(name == string.Empty) return null;
			return TreeList.Columns.ColumnByName(name);
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			OptionsLayoutTreeList opt = options as OptionsLayoutTreeList;
			if(opt == null) return true;
			if(id == LayoutIdAppearance) return opt.StoreAppearance;
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			OptionsLayoutTreeList opt = options as OptionsLayoutTreeList;
			if(opt == null || opt.StoreAppearance)
				AppearanceHeader.Reset();
		}
		#endregion
	}
	[ToolboxItem(false), ListBindable(false)]
	public class TreeListBandCollection : CollectionBase, IHeaderObjectCollection<TreeListBand> {
		public TreeListBandCollection(TreeListBand ownerBand) : this(ownerBand, null) { }
		public TreeListBandCollection(TreeListBand ownerBand, TreeList treeList) {
			OwnerBand = ownerBand;
			TreeList = treeList;
		}
		[Browsable(false)]
		public TreeListBand OwnerBand { get; private set; }
		public TreeList TreeList { get; internal set; }
		public TreeListBand this[int index] { get { return (TreeListBand)List[index]; } }
		public TreeListBand this[string name] {
			get {
				foreach(TreeListBand band in List) {
					if(band.Name == name) return band;
				}
				return null;
			}
		}
		public int VisibleCount { get { return this.Count((b) => { return b.Visible; }); } }
		protected internal TreeListBand LastVisibleBand {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					TreeListBand band = this[n];
					if(band.Visible) return band;
				}
				return null;
			}
		}
		protected internal TreeListBand FirstVisibleBand {
			get {
				for(int n = 0; n < Count; n++) {
					TreeListBand band = this[n];
					if(band.Visible) return band;
				}
				return null;
			}
		}
		protected internal TreeListBand GetVisibleBand(int index) {
			int res = 0;
			for(int n = 0; n < Count; n++) {
				TreeListBand band = this[n];
				if(band.Visible) {
					if(res == index) return band;
					res++;
				}
			}
			return null;
		}
		public bool Contains(TreeListBand band, bool recursive = false) {
			if(List.Contains(band)) return true;
			if(recursive) {
				foreach(TreeListBand currentBand in List)
					if(currentBand.Bands.Contains(band, recursive)) return true;
			}
			return false;
		}
		public int IndexOf(TreeListBand band) { return List.IndexOf(band); }
		[Browsable(false)]
		public TreeListBand CreateBand() { return new TreeListBand(); }
		public TreeListBand Add() { return AddBand(""); }
		public TreeListBand AddBand(string caption) {
			TreeListBand band = CreateBand();
			band.Caption = caption;
			List.Add(band);
			return band;
		}
		public TreeListBand Add(TreeListBand band) {
			if(!CanAddBand(band))
				return null;
			List.Add(band);
			return band;
		}
		public void AddRange(TreeListBand[] bands) {
			foreach(TreeListBand band in bands)
				List.Add(band);
		}
		public void Insert(int position, TreeListBand band) {
			if(!CanAddBand(band))
				return;
			List.Insert(position, band);
		}
		public virtual void SetBandIndex(int newIndex, TreeListBand band) {
			if(!Contains(band)) return;
			int prevIndex = IndexOf(band);
			newIndex = Math.Min(Count - 1, Math.Max(newIndex, 0));
			if(newIndex == prevIndex) return;
			InnerList.Remove(band);
			if(newIndex == Count)
				List.Add(band);
			else
				List.Insert(newIndex, band);
		}
		public void Remove(TreeListBand band) {
			if(List.Contains(band))
				List.Remove(band);
		}
		protected internal virtual void ReplaceParent(TreeListBand band) {
			if(!CanAddBand(band)) return;
			TreeListBand[] bands = new TreeListBand[Count];
			InnerList.CopyTo(bands, 0);
			InnerList.Clear();
			List.Add(band);
			band.Bands.AddRange(bands);
		}
		protected internal virtual bool CanAddBand(TreeListBand band) {
			return band != null && band != OwnerBand && !Contains(band) && !band.Bands.Contains(OwnerBand, true);
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			TreeListBand band = value as TreeListBand;
			if(band.OwnedCollection != null && band.OwnedCollection != this)
				band.OwnedCollection.RemoveInternal(band);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			TreeListBand band = value as TreeListBand;
			if(band != null)
				band.OwnedCollection = this;
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, band));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			TreeListBand band = value as TreeListBand;
			if(band != null)
				band.OwnedCollection = null;
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, band));
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--)
				List.RemoveAt(n);
		}
		protected void RemoveInternal(TreeListBand band) {
			InnerList.Remove(band);
		}
		IEnumerator<TreeListBand> IEnumerable<TreeListBand>.GetEnumerator() {
			foreach(TreeListBand band in List)
				yield return band;
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
		internal void Set(IList<TreeListBand> list) {
			InnerList.Clear();
			for(int i = 0; i < list.Count; i++)
				InnerList.Add(list[i]);
		}
		internal CollectionChangeEventHandler CollectionChanged;
		#region IHeaderObjectCollection<TreeListBand> Members
		void IHeaderObjectCollection<TreeListBand>.Synchronize(IEnumerable<TreeListBand> sourceCollection) {
			TreeListBand[] bands = new TreeListBand[InnerList.Count];
			InnerList.CopyTo(bands);
			InnerList.Clear();
			foreach(TreeListBand band in sourceCollection) {
				TreeListBand targetBand = null;
				if(band.Name != "") {
					targetBand = FindBand(band.Name, bands);
				}
				if(targetBand == null)
					targetBand = Add();
				else
					InnerList.Add(targetBand);
				targetBand.OwnedCollection = this;
				targetBand.Assign(band);
			}
		}
		TreeListBand FindBand(string name, ICollection<TreeListBand> collection) {
			foreach(TreeListBand band in collection)
				if(band.Name == name) return band;
			return null;
		}
		#endregion
		#region serialization
		internal void ClearBandItems(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null || e.Item.ChildProperties.Count == 0) {
				Clear();
				return;
			}
			OptionsLayoutTreeList opt = e.Options as OptionsLayoutTreeList;
			List<object> items = new List<object>();
			foreach(DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo property in e.Item.ChildProperties) {
				object item = FindBandItem(new XtraItemEventArgs(this, this, property));
				if(item != null) items.Add(item);
			}
			for(int i = Count - 1; i >= 0; i--) {
				TreeListBand band = this[i];
				if(opt != null && opt.AddNewColumns) continue;
				if(!items.Contains(band))
					band.Dispose();
			}
		}
		internal object CreateBandItem(XtraItemEventArgs e) {
			OptionsLayoutTreeList options = e.Options as OptionsLayoutTreeList;
			if(options != null)
				if(options.RemoveOldColumns) return null;
			return Add();
		}
		internal object FindBandItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo property = e.Item.ChildProperties["Name"];
			if(property != null && property.Value != null) name = property.Value.ToString();
			if(name == null || name == string.Empty) return null;
			return this[name];
		}
		internal void SetBandItemIndex(XtraSetItemIndexEventArgs e) {
			if(e.Item == null) return;
			int index = this.List.IndexOf(e.Item.Value);
			if(index == -1 || index == e.NewIndex) return;
			SetBandIndex(e.NewIndex, e.Item.Value as TreeListBand);
		}
		#endregion
	}
	[ListBindable(false)]
	public class TreeListBandColumnCollection : CollectionBase, IHeaderObjectCollection<TreeListColumn> {
		public TreeListBandColumnCollection(TreeListBand band) {
			Band = band;
		}
		public TreeListBand Band { get; private set; }
		public TreeListColumn this[int index] { get { return List[index] as TreeListColumn; } }
		public TreeListColumn this[string name] {
			get {
				for(int n = Count - 1; n >= 0; n--) {
					TreeListColumn column = this[n];
					if(column.Name == name) return column;
				}
				return null;
			}
		}
		public int VisibleCount { get { return this.Count(c => { return c.Visible; }); } }
		public bool Contains(TreeListColumn column) { return List.Contains(column); }
		public int IndexOf(TreeListColumn column) { return List.IndexOf(column); }
		public void Add(TreeListColumn column) {
			if(!CanAddColumn(column)) return;
			List.Add(column);
		}
		public void Insert(int index, TreeListColumn column) {
			if(!CanAddColumn(column)) return;
			List.Insert(index, column);
		}
		public void SetColumnIndex(int newIndex, TreeListColumn column) {
			if(!Contains(column)) return;
			int prevIndex = IndexOf(column);
			if(newIndex < 0) newIndex = -1;
			if(newIndex > Count) newIndex = Count;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) {
				List.Remove(column);
			}
			else {
				if(prevIndex == -1) {
					List.Insert(newIndex, column);
				}
				else {
					InnerList.Remove(column);
					if(newIndex > prevIndex) newIndex--;
					List.Insert(newIndex, column);
				}
			}
			if(column.TreeList != null)
				column.TreeList.RaiseColumnPositionChanged(column);
		}
		public void Remove(TreeListColumn column) {
			if(List.Contains(column))
				List.Remove(column);
		}
		protected virtual bool CanAddColumn(TreeListColumn column) {
			if(column == null || Contains(column)) return false;
			if(Band.Bands.Count > 0) return false;
			return true;
		}
		protected override void OnInsert(int index, object value) {
			base.OnInsert(index, value);
			TreeListColumn column = value as TreeListColumn;
			if(column != null && column.ParentBand != null)
				column.ParentBand.Columns.Remove(column);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			TreeListColumn column = value as TreeListColumn;
			if(column != null) {
				column.ParentBand = Band;
				if(Band.TreeList != null)
					Band.TreeList.UpdateBandColumnsOrder(Band);
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, column));
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			TreeListColumn column = value as TreeListColumn;
			if(column != null) {
				if(Band.TreeList != null && !IsClearing)
					Band.TreeList.UpdateBandColumnsOrder(Band);
				column.ParentBand = null;
			}
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, column));
		}
		protected internal bool IsClearing { get; private set; }
		protected override void OnClear() {
			IsClearing = true;
			try {
				for(int n = Count - 1; n >= 0; n--)
					List.RemoveAt(n);
			}
			finally {
				IsClearing = false;
				if(Band.TreeList != null)
					Band.TreeList.UpdateBandColumnsOrder(Band);
			}
		}
		protected internal void Sort(IComparer comparer) {
			InnerList.Sort(comparer);
		}
		protected internal void Set(TreeListBandRowCollection rows) {
			InnerList.Clear();
			foreach(TreeListBandRow row in rows)
				InnerList.AddRange(row.Columns);
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(CollectionChanged != null)
				CollectionChanged(this, e);
		}
		IEnumerator<TreeListColumn> IEnumerable<TreeListColumn>.GetEnumerator() {
			foreach(TreeListColumn column in List)
				yield return column;
		}
		internal CollectionChangeEventHandler CollectionChanged;
		#region IHeaderObjectCollection<TreeListColumn> Members
		void IHeaderObjectCollection<TreeListColumn>.Synchronize(IEnumerable<TreeListColumn> sourceCollection) {
			InnerList.Clear();
			List<TreeListColumn> columns = new List<TreeListColumn>(sourceCollection);
			for(int n = 0; n < columns.Count; n++) {
				TreeListColumn column = columns[n];
				TreeListColumn currentColumn = Band.TreeList.Columns[column.AbsoluteIndex];
				currentColumn.ParentBand = Band;
				InnerList.Add(currentColumn);
			}
		}
		#endregion
	}
	public class TreeListBandRow : IBandRow {
		public TreeListBandRow() {
			Columns = new VisibleColumnsList();
		}
		public int TotalRowCount {
			get {
				int rowCount = 0;
				foreach(TreeListColumn column in Columns)
					rowCount = Math.Max(rowCount, column.RowCount);
				return rowCount;
			}
		}
		public VisibleColumnsList Columns { get; private set; }
		ICollection IBandRow.Columns { get { return Columns; } }
	}
	[ListBindable(false)]
	public class TreeListBandRowCollection : CollectionBase {
		public TreeListBandRow this[int index] { get { return (TreeListBandRow)List[index]; } }
		public int Add(TreeListBandRow row) {
			int index = List.IndexOf(row);
			if(index > -1) return index;
			return List.Add(row);
		}
		public void Insert(int index, TreeListBandRow row) {
			List.Insert(index, row);
		}
		public int IndexOf(TreeListBandRow row) { return List.IndexOf(row); }
		public TreeListBandRow FindRow(TreeListColumn column) {
			foreach(TreeListBandRow row in this) {
				if(row.Columns.Contains(column)) return row;
			}
			return null;
		}
	}
	[ToolboxItem(false)]
	public class TreeListBandCustomizationForm : TreeListCustomizationForm {
		TreeListCustomizationListBox columnsListBox;
		TreeListBandCustomizationListBox bandsListBox;
		XtraTab.XtraTabControl tabControl;
		XtraTab.XtraTabPage columnsPage, bandsPage;
		bool isUpdateListBox;
		DevExpress.XtraEditors.SearchControl searchBox = null;
		public TreeListBandCustomizationForm(TreeList treeList) : base(treeList, treeList.Handler) { }
		public TreeListCustomizationListBox ColumnsListBox { get { return columnsListBox; } }
		public TreeListBandCustomizationListBox BandsListBox { get { return bandsListBox; } }
		public bool IsColumnsListBoxActive { get { return ActiveListBox == ColumnsListBox; } }
		public bool IsBandsListBoxActive { get { return ActiveListBox == BandsListBox; } }
		protected DevExpress.XtraTab.XtraTabControl TabControl { get { return tabControl; } }
		protected XtraTab.XtraTabPage ColumnsPage { get { return columnsPage; } }
		protected XtraTab.XtraTabPage BandsPage { get { return bandsPage; } }
		protected virtual DevExpress.XtraTab.XtraTabControl CreateTabControl() {
			return new XtraTab.XtraTabControl();
		}
		protected virtual TreeListCustomizationListBox CreateColumnsListBox() {
			return new TreeListCustomizationListBox(this) { BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder };
		}
		protected virtual TreeListBandCustomizationListBox CreateBandsListBox() {
			return new TreeListBandCustomizationListBox(this) { BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder };
		}
		protected override void CreateListBox() {
			this.columnsListBox = CreateColumnsListBox();
			this.ColumnsListBox.Populate();
			this.bandsListBox = CreateBandsListBox();
			this.BandsListBox.Populate();
			SetActiveListBox(ColumnsListBox);
			this.tabControl = CreateTabControl();
			InitTabControl(TabControl);
			this.columnsPage = tabControl.TabPages.Add(TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.CustomizationColumns));
			if(TreeList.CanShowBandsInCustomizationForm) {
				this.bandsPage = tabControl.TabPages.Add(TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.CustomizationBands));
				this.bandsPage.Controls.Add(BandsListBox);
			}
			this.columnsPage.Controls.Add(ColumnsListBox);
			Controls.Add(TabControl);
			this.Padding = new Padding(6);
			this.searchBox = AddSearchControl(ColumnsListBox, this);
		}
		protected virtual void InitTabControl(XtraTab.XtraTabControl tabControl) {
			tabControl.LookAndFeel.Assign(TreeList.ElementsLookAndFeel);
			tabControl.Dock = DockStyle.Fill;
			tabControl.SelectedPageChanged += (sender, e) => OnSelectedTabPageChanged(e.Page);
		}
		protected virtual void OnSelectedTabPageChanged(XtraTab.XtraTabPage newPage) {
			if(newPage == BandsPage) {
				SetActiveListBox(BandsListBox);
				UpdateSearchBox(BandsListBox, TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.SearchForBand));
			}
			else {
				SetActiveListBox(ColumnsListBox);
				UpdateSearchBox(ColumnsListBox, DevExpress.XtraEditors.Controls.Localizer.Active.GetLocalizedString(DevExpress.XtraEditors.Controls.StringId.SearchForColumn));
			}
		}
		protected void UpdateSearchBox(TreeListCustomizationListBox customizationListBox, string nullValuePrompt) {
			if(searchBox == null || customizationListBox == null) return;
			string searchText = searchBox.Text;
			searchBox.Properties.NullValuePrompt = nullValuePrompt;
			searchBox.Client = customizationListBox;
			searchBox.SetFilter(searchText);
		}
		public void ShowBandsPage() {
			if(BandsPage == null) return;
			TabControl.SelectedTabPage = BandsPage;
		}
		public void ShowColumnsPage() {
			if(ColumnsPage == null) return;
			TabControl.SelectedTabPage = ColumnsPage;
		}
		public override void CheckAndUpdate() {
			bool requireUpdate = false;
			if(ColumnsListBox.ItemHeight != ColumnsListBox.GetItemHeight()) {
				ColumnsListBox.ItemHeight = ColumnsListBox.GetItemHeight();
				requireUpdate = true;
			}
			if(BandsListBox.ItemHeight != BandsListBox.GetItemHeight()) {
				BandsListBox.ItemHeight = BandsListBox.GetItemHeight();
				requireUpdate = true;
			}
			if(requireUpdate)
				UpdateSize();
			UpdateListBox();
			Refresh();
		}
		public override void UpdateListBox() {
			if(isUpdateListBox) return;
			isUpdateListBox = true;
			try {
				LookAndFeel.Assign(ControlOwnerLookAndFeel);
				ColumnsListBox.LookAndFeel.Assign(ControlOwnerLookAndFeel);
				BandsListBox.LookAndFeel.Assign(ControlOwnerLookAndFeel);
				TabControl.LookAndFeel.Assign(ControlOwnerLookAndFeel);
				ColumnsListBox.Populate();
				BandsListBox.Populate();
			}
			finally {
				isUpdateListBox = false;
			}
		}
		protected override void SetDefaultFormSize() {
			this.ClientSize = TabControl.CalcSizeByPageClient(new Size(200, Math.Max(ColumnsListBox.GetItemHeight() * 7 + 4, BandsListBox.GetItemHeight() * 7 + 4)));
		}
	}
	#region class TreeList Customization
	[ToolboxItem(false)]
	public class TreeListBandCustomizationListBox : TreeListCustomizationListBox {
		public const int DefaultLevelIndent = 8;
		public TreeListBandCustomizationListBox(TreeListCustomizationForm form)
			: base(form) {
		}
		public override int GetItemHeight() { return TreeList.ViewInfo.BandPanelRowHeight; }
		protected TreeListBand PressedBand {
			get { return CustomizationForm.PressedItem as TreeListBand; }
			set { CustomizationForm.PressedItem = value; }
		}
		public override void Populate() {
			Items.BeginUpdate();
			try {
				Items.Clear();
				foreach(TreeListBand band in TreeList.Bands.OrderBy(b => TreeList.GetNonFormattedCaption(b.GetCustomizationCaption()))) {
					if(!CanAddBand(band)) continue;
					Items.Add(band);
					if(band.HasChildren) AddBandChildren(band);
				}
			}
			finally {
				Items.EndUpdate();
			}
		}
		protected void AddBandChildren(TreeListBand band) {
			if(!band.HasChildren) return;
			foreach(TreeListBand child in band.Bands.OrderBy(b => TreeList.GetNonFormattedCaption(b.GetCustomizationCaption()))) {
				if(!CanAddBand(child)) continue;
				Items.Add(child);
				AddBandChildren(child);
			}
		}
		protected virtual bool CanAddBand(TreeListBand band) {
			if(!band.Visible)
				return band.OptionsBand.ShowInCustomizationForm;
			if(!band.HasChildren) return false;
			bool result = false;
			foreach(TreeListBand child in band.Bands)
				result |= CanAddBand(child);
			return result;
		}
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			TreeListBand band = GetItemValue(index) as TreeListBand;
			int indent = band.Level * DefaultLevelIndent;
			Rectangle itemBounds = bounds;
			itemBounds.Width -= indent;
			if(!IsRightToLeft) itemBounds.X += indent;
			itemBounds.Width++;
			DrawBand(cache, band, itemBounds);
			if(indent > 0) {
				if(IsRightToLeft) bounds.X += bounds.Width - indent;
				bounds.Width = indent;
				DrawIndent(cache, bounds);
			}
		}
		protected virtual void DrawIndent(GraphicsCache cache, Rectangle bounds) {
			using(Brush brush = new SolidBrush(BackColor))
				cache.FillRectangle(brush, bounds);
		}
		public BandInfo GetBandByPoint(Point screenPoint) {
			Point point = PointToClient(screenPoint);
			int index = IndexFromPoint(point);
			if(index < 0) return null;
			TreeListBand band = Items[index] as TreeListBand;
			return GetBandInfo(band, new Rectangle(0, 0, ClientSize.Width, ItemHeight));
		}
		protected void DrawBand(GraphicsCache cache, TreeListBand band, Rectangle bounds) {
			TreePainter.DrawDragBand(cache.Graphics, GetBandInfo(band, bounds));
		}
		protected virtual BandInfo GetBandInfo(TreeListBand band, Rectangle bounds) {
			BandInfo bi = new BandInfo(band) { CustomizationForm = true };
			bi.Bounds = bounds;
			bi.SetAppearance(TreeList.ViewInfo.PaintAppearance.BandPanel);
			TreeList.ViewInfo.ElementPainters.BandPainter.CalcObjectBounds(bi);
			bi.Pressed = (band == PressedBand);
			return bi;
		}
		protected override string GetHintCaptionForEmptyList() {
			return TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.CustomizationFormBandHint);
		}
		protected override void DoShowItem(object item) {
			base.DoShowItem(item);
			TreeListBand band = item as TreeListBand;
			if(band == null) return;
			band.Visible = true;
		}
		protected override bool CanPressItem(object item) {
			TreeListBand band = item as TreeListBand;
			if(band == null || band.Visible) return false;
			return base.CanPressItem(item);
		}
	}
	#endregion
}
