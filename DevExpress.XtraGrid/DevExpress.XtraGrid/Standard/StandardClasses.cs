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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Summary;
using DevExpress.LookAndFeel;
using System.Collections.Generic;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Internal;
namespace DevExpress.XtraGrid.Drawing {
	internal interface IGridSummaryFind {
		GridSummaryItem FindSummaryItem(SummaryItem item);
	}
	public class GridTotalSummaryCollection : List<GridColumnSummaryItem>, IGridSummaryFind {
		GridView view;
		int activeCount = -1;
		bool dirty = true;
		public GridTotalSummaryCollection(GridView view) {
			this.view = view;
		}
		public GridView View { get { return view; } }
		public void Rebuild() {
			this.dirty = false;
			Clear();
			this.activeCount = 0;
			for(int n = 0; n < View.Columns.Count; n++) {
				foreach(GridColumnSummaryItem item in View.Columns[n].Summary) {
					this.Add(item);
					if(item.SummaryType != SummaryItemType.None) activeCount++;
				}
			}
		}
		public void CheckDirty() {
			if(this.dirty) Rebuild();
		}
		public void SetDirty() {
			this.dirty = true;
		}
		public void ResetActiveCount() {
			this.activeCount = -1;
		}
		public int ActiveCount {
			get {
				if(activeCount < 0) UpdateActiveCount();
				return activeCount;
			}
		}
		void UpdateActiveCount() {
			this.activeCount = 0;
			for(int n = 0; n < Count; n++) {
				if(this[n].SummaryType != SummaryItemType.None) activeCount++;
			}
		}
		GridSummaryItem IGridSummaryFind.FindSummaryItem(SummaryItem item) {
			foreach(GridSummaryItem sitem in this) {
				if(sitem.SummaryItem == item) return sitem;
			}
			return null;
		}
	}
}
namespace DevExpress.XtraGrid { 
	#region Summary classes
	public enum SummaryItemField {
		FieldName
	}
	public class ShouldSerializeSummaryItemEventArgs : EventArgs {
		bool shouldSerialize;
		GridSummaryItem item;
		SummaryItemField itemField;
		public ShouldSerializeSummaryItemEventArgs(GridSummaryItem item, SummaryItemField itemField) {
			this.shouldSerialize = true;
			this.item = item;
			this.itemField = itemField;
		}
		public GridSummaryItem Item { get { return item; } }
		public SummaryItemField ItemField { get { return itemField; } }
		public bool ShouldSerialize { get { return shouldSerialize; } set { shouldSerialize = value; } }
	}
	public delegate void ShouldSerializeSummaryItemEventHandler(object sender, ShouldSerializeSummaryItemEventArgs e);
	[ListBindable(false)]
	public class GridGroupSummaryItemCollection : GridSummaryItemCollection {
		BaseView view;
		public GridGroupSummaryItemCollection(BaseView view) {
			this.view = view;
		}
		public override BaseView View { get { return view; } }
		public new GridGroupSummaryItem Add() { return base.Add() as GridGroupSummaryItem; }
		public virtual GridGroupSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			return Add(summaryType, fieldName, null); 
		}
		public virtual GridGroupSummaryItem Add(SummaryItemType summaryType, string fieldName, GridColumn showInColumn) {
			return Add(summaryType, fieldName, showInColumn, "");
		}
		public virtual GridGroupSummaryItem Add(SummaryItemType summaryType, string fieldName, GridColumn showInColumn, string displayFormat) {
			return Add(summaryType, fieldName, showInColumn, displayFormat, null);
		}
		public virtual GridGroupSummaryItem Add(SummaryItemType summaryType, string fieldName, GridColumn showInColumn, string displayFormat, IFormatProvider formatProvider) {
			GridGroupSummaryItem item = CreateItem() as GridGroupSummaryItem;
			item.ShowInGroupColumnFooter = showInColumn;
			item.SummaryType = summaryType;
			item.FieldName = fieldName;
			item.DisplayFormat = displayFormat;
			item.Format = formatProvider;
			Add(item);
			return item;
		}
		protected override GridSummaryItem CreateItem() { return new GridGroupSummaryItem(); }
		protected override void OnInsert(int index, object item) {
			GridGroupSummaryItem gi = item as GridGroupSummaryItem;
			if(gi == null || List.Contains(gi)) throw new ArgumentException("item");
			base.OnInsert(index, item);
		}
		protected internal virtual void Refresh() {
			BeginUpdate();
			try {
				foreach(GridGroupSummaryItem item in this) {
					item.ShowInGroupColumnFooterName = item.ShowInGroupColumnFooterName;
				}
			}
			finally {
				CancelUpdate();
			}
		}
	}
	[ListBindable(false)]
	public class GridColumnSummaryItemCollection : GridSummaryItemCollection {
		GridColumn column;
		public GridColumnSummaryItemCollection(GridColumn column) {
			this.column = column;
		}
		public new GridColumnSummaryItem this[int index] { get { return base[index] as GridColumnSummaryItem; } }
		public new GridColumnSummaryItem this[object tag] { get { return base[tag] as GridColumnSummaryItem; } }
		protected internal GridColumn Column { get { return column; } }
		public override BaseView View { get { return Column == null ? null : Column.View; } }
		public new GridColumnSummaryItem Add() { return base.Add() as GridColumnSummaryItem; }
		public virtual GridColumnSummaryItem Add(SummaryItemType summaryType, string fieldName) {
			return Add(summaryType, fieldName, "");
		}
		public virtual GridColumnSummaryItem Add(SummaryItemType summaryType) {
			return Add(summaryType, "");
		}
		public virtual GridColumnSummaryItem Add(SummaryItemType summaryType, string fieldName, string displayFormat) {
			return Add(summaryType, fieldName, displayFormat, null);
		}
		public virtual GridColumnSummaryItem Add(SummaryItemType summaryType, string fieldName, string displayFormat, IFormatProvider formatProvider) {
			GridColumnSummaryItem item = CreateItem() as GridColumnSummaryItem;
			item.SummaryType = summaryType;
			item.FieldName = fieldName;
			item.DisplayFormat = displayFormat;
			item.Format = formatProvider;
			Add(item);
			return item;
		}
		protected override GridSummaryItem CreateItem() {
			GridColumnSummaryItem res = new GridColumnSummaryItem();
			return res;
		}
		protected override void OnInsert(int index, object item) {
			GridColumnSummaryItem gi = item as GridColumnSummaryItem;
			if(gi == null || List.Contains(gi)) throw new ArgumentException("item");
			base.OnInsert(index, item);
		}
		protected internal GridSummaryItem GetActiveItem(int index) {
			if(index >= ActiveCount) return null;
			int active = 0;
			for(int n = 0; n < Count; n++) {
				if(this[n].SummaryType != SummaryItemType.None && this[n].Exists) {
					if(index == active++) return this[n];
				}
			}
			return null;
		}
		internal GridSummaryItem FindEmptyOrCreate(SummaryItemType summaryType, string fieldName, string displayFormat, IFormatProvider formatProvider) {
			if(Count == 0) return Add();
			for(int n = 0; n < Count; n++) {
				if(this[n].SummaryType == SummaryItemType.None && this[n].FieldName == Column.FieldName) return this[n];
			}
			return Add(summaryType, fieldName, displayFormat, formatProvider);
		}
	}
	public class GridSummaryItemCollection : CollectionBase, IGridSummaryFind, IEnumerable<GridSummaryItem> {
		int lockUpdate, activeCount;
		public event CollectionChangeEventHandler CollectionChanged;
		public GridSummaryItemCollection() {
			this.lockUpdate = 0;
			this.activeCount = -1;
		}
		protected internal void SetItemIndex(GridSummaryItem item, int newIndex) {
			newIndex = Math.Min(Math.Max(0, newIndex), Count);
			int prevIndex = List.IndexOf(item);
			if(prevIndex < 0 || prevIndex == newIndex) return;
			InnerList.RemoveAt(prevIndex);
			if(newIndex > prevIndex) newIndex --;
			InnerList.Insert(newIndex, item);
		}
		public virtual void Assign(GridSummaryItemCollection coll) {
			BeginUpdate();
			try {
				Clear();
				for(int n = 0; n < coll.Count; n++) {
					Add(CreateItem());
				}
				for(int n = 0; n < coll.Count; n++) {
					this[n].Assign(coll[n]);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void BeginUpdate() { 
			lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if( -- lockUpdate == 0) OnItemChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual void CancelUpdate() {
			lockUpdate --;
		}
		public virtual GridSummaryItem Add() { 
			GridSummaryItem item = CreateItem();
			Add(item);
			return item;
		}
		public virtual int Add(GridSummaryItem item) {
			return List.Add(item);
		}
		protected internal virtual void OnItemChanged(CollectionChangeEventArgs e) {
			this.activeCount = -1;
			if(lockUpdate != 0) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected virtual GridSummaryItem CreateItem() { return new GridSummaryItem(); }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridSummaryItemCollectionView")]
#endif
		public virtual BaseView View { get { return null; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridSummaryItemCollectionItem")]
#endif
		public GridSummaryItem this[int index] { get { return List[index] as GridSummaryItem; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridSummaryItemCollectionItem")]
#endif
		public GridSummaryItem this[object tag] { 
			get { 
				foreach(GridSummaryItem item in List) {
					if(Object.Equals(item.Tag, tag)) return item;
				}
				return null;
			}
		}
		public virtual void AddRange(GridSummaryItem[] items) {
			BeginUpdate();
			try {
				foreach(GridSummaryItem item in items) {
					Add(item);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void Remove(GridSummaryItem item) {
			if(List.Contains(item)) List.Remove(item);
		}
		protected override void OnInsertComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			GridSummaryItem sItem = item as GridSummaryItem;
			this.activeCount = -1;
			sItem.SetCollection(this);
			ResetIndexes();
			OnItemChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			base.OnInsertComplete(index, item);
			this.activeCount = -1;
			GridSummaryItem sItem = item as GridSummaryItem;
			sItem.SetCollection(null);
			ResetIndexes();
			OnItemChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			InnerList.Clear();
			OnItemChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridSummaryItemCollectionActiveCount")]
#endif
		public int ActiveCount {
			get {
				if(activeCount == -1) UpdateActiveCount();
				return activeCount;
			}
		}
		protected virtual void ResetIndexes() {
			for(int n = 0; n < Count; n ++ ) {
				this[n].SetIndex(n);
			}
		}
		public virtual int IndexOf(GridSummaryItem item) { return List.IndexOf(item); }
		protected virtual void UpdateActiveCount() {
			activeCount = 0; 
			int cnt = Count;
			for(int n = 0; n < cnt; n++) {
				if(this[n].SummaryType != SummaryItemType.None && this[n].Exists) activeCount ++;
			}
		}
		GridSummaryItem IGridSummaryFind.FindSummaryItem(SummaryItem item) {
			foreach(GridSummaryItem sitem in this) {
				if(sitem.SummaryItem == item) return sitem;
			}
			return null;
		}
		internal object XtraCreateSummaryItem(XtraItemEventArgs e) {
			return Add();
		}
		internal void XtraSetIndexSummaryItem(XtraSetItemIndexEventArgs e) {
			if(e.Item == null) return;
			int index = IndexOf(e.Item.Value as GridSummaryItem);
			if(index == -1 || index == e.NewIndex) return;
			SetItemIndex(e.Item.Value as GridSummaryItem, e.NewIndex);
		}
		internal object XtraFindSummaryItem(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			DevExpress.Utils.Serializing.Helpers.XtraPropertyInfo xp = e.Item.ChildProperties["Tag"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(name == null || name == string.Empty) return null;
			return this[name];
		}
		IEnumerator<GridSummaryItem> IEnumerable<GridSummaryItem>.GetEnumerator() {
			foreach(GridSummaryItem summaryItem in InnerList)
				yield return summaryItem;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridSummaryItem : ISummaryItem {
		object _tag;
		SummaryItem _summaryItem;
		GridSummaryItemCollection collection;
		SummaryItemType _summaryType;
		string _displayFormat, _fieldName;
		int _index;
		IFormatProvider _formatProvider;
		public GridSummaryItem(SummaryItemType summaryType, string fieldName, string displayFormat, object tag) {
			this._summaryItem = null;
			this._tag = tag;
			this.collection = null;
			this._index = -1;
			this._summaryType = summaryType;
			this._displayFormat = displayFormat;
			this._fieldName = fieldName;
			this._formatProvider = null;
		}
		public override string ToString() {
			string res = OptionsHelper.GetObjectText(this);
			return string.IsNullOrEmpty(res) ? "<None>" : res;
		}
		protected internal void Reset() {
			this._summaryItem = null;
			this._tag = null;
			this._index = -1;
			this._summaryType = SummaryItemType.None;
			this._displayFormat = string.Empty;
			this._fieldName = DefaultFieldName;
			this._formatProvider = null;
		}
		protected virtual string DefaultFieldName { get { return string.Empty; } }
		public GridSummaryItem() : this(SummaryItemType.None, "", "") { }
		public GridSummaryItem(SummaryItemType summaryType, string fieldName, string displayFormat) : this(summaryType, fieldName, displayFormat, null) { }
		protected internal virtual void AssignSummaryItem() {
			if(SummaryItem == null) return;
			if(IsEqualsSummaryItem(SummaryItem)) return;
			SummaryItem.Tag = this;
			SummaryItem.SummaryType = SummaryType;
			SummaryItem.FieldName = FieldName;
		}
		internal bool Exists {
			get { return SummaryItem == null ? true : SummaryItem.Exists; }
		}
		internal bool EqualsSummaryItem() {
			return IsEqualsSummaryItem(SummaryItem);
		}
		protected internal virtual bool IsEqualsSummaryItem(SummaryItem item) {
			return this.FieldName == item.FieldName && this.SummaryType == item.SummaryType;
		}
		protected internal virtual SummaryItem SummaryItem { get { return _summaryItem; } set { _summaryItem = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual GridSummaryItemCollection Collection { get { return collection; } }
		protected internal void SetCollection(GridSummaryItemCollection coll) { this.collection = coll; }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridSummaryItemTag"),
#endif
		XtraSerializableProperty(), DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return _tag; }
			set {
				_tag = value;
			}
		}
		[Browsable(false)]
		public virtual int Index { 
			get { 
				if(_index == -1) {
					if(Collection == null) return -1;
					_index = Collection.IndexOf(this);
				}
				return _index; 
			}
		}
		protected internal void SetIndex(int val) { this._index = val; }
		public virtual bool IsEquals(GridSummaryItem item) {
			return (SummaryType == item.SummaryType && this.DisplayFormat == item.DisplayFormat &&
				this.FieldName == item.FieldName);
		}
		public virtual void Assign(GridSummaryItem source) {
			if(IsEquals(source)) return;
			SummaryItemType prevType = SummaryType;
			this._tag = source.Tag;
			this._summaryType = source.SummaryType;
			this._displayFormat = source.DisplayFormat;
			this._fieldName = source.FieldName;
			this._formatProvider = source.Format;
			if(prevType != SummaryType)	OnChanged();
		}
		protected internal virtual bool ShouldSerializeFieldName() {
			return !string.IsNullOrEmpty(FieldName);
		}
		public void SetSummary(SummaryItemType summaryType, string displayFormat) {
			SetSummary(summaryType, displayFormat, null);
		}
		public void SetSummary(SummaryItemType summaryType, string displayFormat, IFormatProvider format) {
			if(SummaryType == summaryType && DisplayFormat == displayFormat && Format == format) return;
			this._summaryType = summaryType;
			this._displayFormat = displayFormat;
			this._formatProvider = format;
			OnChanged();
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridSummaryItemFieldName"),
#endif
 XtraSerializableProperty(1),
		Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public virtual string FieldName {
			get { return _fieldName; }
			set {
				if(FieldName == value) return;
				_fieldName = value;
				if(SummaryType != SummaryItemType.None) OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridSummaryItemDisplayFormat"),
#endif
 DefaultValue(""), XtraSerializableProperty(100), Localizable(true)]
		public string DisplayFormat {
			get { return _displayFormat; }
			set {
				if(DisplayFormat == value) return;
				_displayFormat = value;
				if(SummaryType != SummaryItemType.None) OnChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridSummaryItemSummaryType"),
#endif
 DefaultValue(SummaryItemType.None), XtraSerializableProperty(1)]
		public SummaryItemType SummaryType {
			get { return _summaryType; }
			set {
				if(SummaryType == value) return;
				OnChangingSummaryType(SummaryType, value);
				_summaryType = value;
				OnChanged();
			}
		}
		void OnChangingSummaryType(SummaryItemType summaryType, SummaryItemType value) {
			var currentFormat = GetDefaultDisplayFormat();
			this._summaryType = value;
			if(currentFormat == DisplayFormat)
				_displayFormat = GetDefaultDisplayFormat();
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridSummaryItemFormat"),
#endif
 DefaultValue(null), Browsable(false)]
		public IFormatProvider Format {
			get { return _formatProvider; }
			set {
				if(Format == value) return;
				_formatProvider = value;
				OnChanged();
			}
		}
		[Browsable(false)]
		public virtual object SummaryValue { 
			get { 
				if(SummaryItem == null) return null;
				return SummaryItem.SummaryValue;
			} 
		}
		public virtual string GetFormatDisplayText(string formatString, object value) {
			if(Format == null) return String.Format(formatString, value);
			return String.Format(Format, formatString, value);
		}
		public virtual string GetDisplayText(object value, bool fullForm) {
			try {
				string format = DisplayFormat;
				if(format == "") format = GetDisplayFormatByType(SummaryType, fullForm);
				return GetFormatDisplayText(format, value);
			} catch {
			}
			return "";
		}
		public virtual string GetDisplayFormatByType(SummaryItemType itemType, bool fullForm) {
			string result = "{0}", beforeText = "";
			string dislayFormat = GetDisplayFormatByType(itemType);
			if (!string.IsNullOrEmpty(dislayFormat)) {
				beforeText = string.Format("({0})", dislayFormat); 
			}
			if(fullForm)
				return beforeText;
			return result;
		}
		protected internal string GetDisplayFormatByType(SummaryItemType itemType) {
			switch (itemType) {
				case SummaryItemType.Average:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterAverageFormat);
				case SummaryItemType.Sum:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterSumFormat);
				case SummaryItemType.Count:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterCountGroupFormat);
				case SummaryItemType.Min:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterMinFormat);
				case SummaryItemType.Max:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterMaxFormat);
				case SummaryItemType.Custom:
					return GridLocalizer.Active.GetLocalizedString(GridStringId.MenuFooterCustomFormat);
			}
			return string.Empty;
		}
		protected virtual void OnChanged() {
			if(Collection != null) Collection.OnItemChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}
		public virtual string GetDefaultDisplayFormat() {
			return string.Empty;
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
	public class GridColumnSummaryItem : GridSummaryItem {
		public GridColumnSummaryItem(SummaryItemType summaryType, string fieldName, string displayFormat, object tag) : base(summaryType, fieldName, displayFormat, tag) {
		}
		public GridColumnSummaryItem(SummaryItemType summaryType, string fieldName, string displayFormat) : this(summaryType, fieldName, displayFormat, null) {
		}
		public GridColumnSummaryItem(SummaryItemType summaryType) : this(summaryType, "", "") { }
		public GridColumnSummaryItem() : this(SummaryItemType.None) { }
		protected internal GridColumn Column {
			get {
				GridColumnSummaryItemCollection coll = (GridColumnSummaryItemCollection)Collection;
				if(coll == null) return null;
				return coll.Column;
			}
		}
		protected override string DefaultFieldName { get { return Column == null ? string.Empty : Column.FieldName; } }
		public override string GetDefaultDisplayFormat() {
			if(Column == null) return string.Empty;
			GridView view = Column.View as GridView;
			if(view == null) return string.Empty;
			return view.GetSummaryFormat(Column, SummaryType);
		}
		protected internal override bool ShouldSerializeFieldName() {
			return FieldName != DefaultFieldName;
		}
		public override string FieldName {
			get {
				string res =  base.FieldName;
				if(string.IsNullOrEmpty(res)) return DefaultFieldName;
				return res;
			}
			set {
				base.FieldName = value;
			}
		}
	}
	[TypeConverter("DevExpress.XtraGrid.TypeConverters.GroupSummaryItemTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
	public class GridGroupSummaryItem : GridSummaryItem {
		protected GridColumn fShowInGroupColumnFooter;
		public GridGroupSummaryItem(SummaryItemType summaryType, string fieldName, GridColumn showInColumn, string displayFormat, object tag) : base(summaryType, fieldName, displayFormat, tag) {
			this.fShowInGroupColumnFooter = showInColumn;
		}
		public GridGroupSummaryItem(SummaryItemType summaryType, string fieldName, GridColumn showInColumn, string displayFormat) : this(summaryType, fieldName, showInColumn, displayFormat, null) {
		}
		public GridGroupSummaryItem() : this(SummaryItemType.None, "", null, "") {
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridGroupSummaryItemShowInGroupColumnFooter"),
#endif
 DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.GroupSummaryColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign),
		RefreshProperties(RefreshProperties.All)]
		public virtual GridColumn ShowInGroupColumnFooter {
			get { return fShowInGroupColumnFooter; }
			set {
				if(ShowInGroupColumnFooter == value) return;
				fShowInGroupColumnFooter = value;
				OnChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public virtual string ShowInGroupColumnFooterName {
			get { return ShowInGroupColumnFooter == null ? "" : ShowInGroupColumnFooter.Name; }
			set {
				if(value == null || value == "") {
					ShowInGroupColumnFooter = null;
					return;
				}
				if(Collection == null) return;
				ColumnView view = Collection.View as ColumnView;
				if(view == null) return;
				ShowInGroupColumnFooter = view.Columns.ColumnByName(value);
			}
		}
		public override bool IsEquals(GridSummaryItem item) {
			bool result = base.IsEquals(item);
			if(!result) return false;
			GridGroupSummaryItem gs = item as GridGroupSummaryItem;
			if(gs == null) return false;
			return (ShowInGroupColumnFooter == gs.ShowInGroupColumnFooter);
		}
		public override void Assign(GridSummaryItem source) {
			if(IsEquals(source)) return;
			GridGroupSummaryItem gs = source as GridGroupSummaryItem;
			if(gs != null) {
				GridColumn col = gs.ShowInGroupColumnFooter;
				if(col != null) {
					if(Collection != null && Collection.View != null) {
						ColumnView colView = Collection.View as ColumnView;
						if(col.AbsoluteIndex < colView.Columns.Count)
							col = colView.Columns[col.AbsoluteIndex];
						else 
							col = null;
					} else
						col = null;
				}
				this.fShowInGroupColumnFooter = col;
			}
			base.Assign(source);
		}
		public override string GetDefaultDisplayFormat() {
			if(string.IsNullOrEmpty(this.FieldName)) return string.Empty;
			if(this.Collection== null || this.Collection.View == null) return string.Empty;
			GridView view = this.Collection.View as GridView;
			if(view == null) return string.Empty;
			return view.GetSummaryItemDisplayFormat(this);
		}
	}
	#endregion Summary classes
	#region FormatCondition classes
	[ListBindable(false)]
	public class StyleFormatConditionCollection : FormatConditionCollectionBase {
		BaseView view;
		public StyleFormatConditionCollection(BaseView view) {
			this.view = view;
		}
		public override int CompareValues(object val1, object val2) {
			return View.DataController.ValueComparer.Compare(val1, val2);
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("StyleFormatConditionCollectionView")]
#endif
		public BaseView View { get { return view as BaseView; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("StyleFormatConditionCollectionIsLoading")]
#endif
		public override bool IsLoading { get { return View == null || View.IsLoading; } }
		public StyleFormatCondition GetStyleFormatByValue(GridColumn column, object value, int listSourceRow) {
			int cnt = Count;
			if(cnt == 0) return null;
			for(int n = 0; n < cnt; n++) {
				StyleFormatCondition cond = this[n];
				if(cond.CheckValue(column, value, listSourceRow)) 
					return cond;
			}
			return null;
		}
		protected override object CreateItem() { return new StyleFormatCondition(); }
		protected GridControl GridControl { get { return View != null ? View.GridControl : null; } }
		public void AddRange(StyleFormatCondition[] conditions) { base.AddRange(conditions); }
#if !SL
	[DevExpressXtraGridLocalizedDescription("StyleFormatConditionCollectionItem")]
#endif
		public new StyleFormatCondition this[int index] { get { return base[index] as StyleFormatCondition; } }
		public void Add(StyleFormatCondition condition) { base.Add(condition); }
		internal void ResetEvaluatorsCore() {
			ResetEvaluators();
		}
	}
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class StyleFormatCondition : StyleFormatConditionBase, ISupportLookAndFeel {
		bool applyToRow;
		public StyleFormatCondition(FormatConditionEnum condition) : this(condition, null, null, null) {
			SetLoaded(true);
		}
		public StyleFormatCondition(FormatConditionEnum condition, GridColumn column, object tag, object val1) : this(condition, column, tag, val1, null) { }
		public StyleFormatCondition(FormatConditionEnum condition, GridColumn column, object tag, object val1, object val2) : this(condition, column, tag, val1, val2, false) { }
		public StyleFormatCondition(FormatConditionEnum condition, GridColumn column, object tag, object val1, object val2, bool applyToRow) : this(condition, tag, (AppearanceObject)null, val1, val2, column, applyToRow) { 
			SetLoaded(true);
		}
		public StyleFormatCondition() : this(FormatConditionEnum.None, null, null, null, null) {
			SetLoaded(false);
		}
		public StyleFormatCondition(FormatConditionEnum condition, object tag, AppearanceDefault appearanceDefault, object val1, object val2,
			GridColumn column, bool applyToRow) : base(condition, tag, appearanceDefault, val1, val2, column) {
			this.applyToRow = applyToRow;
		}
		public StyleFormatCondition(FormatConditionEnum condition, object tag, AppearanceObject appearance, object val1, object val2,
			GridColumn column, bool applyToRow) : base(condition, tag, appearance, val1, val2, column) {
			this.applyToRow = applyToRow;
		}
		[Browsable(false)]
		public new StyleFormatConditionCollection Collection {
			get { return base.Collection as StyleFormatConditionCollection; } 
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("StyleFormatConditionColumn"),
#endif
 DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnReferenceConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public new GridColumn Column {
			get { return base.Column as GridColumn; }
			set { base.Column = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public string ColumnName {
			get {
				if(Column != null) return Column.Name;
				return string.Empty;
			}
			set {
				if(Collection == null) return;
				GridColumn column = (Collection.View as ColumnView).Columns.ColumnByName(value);
				Column = column;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("StyleFormatConditionApplyToRow"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ApplyToRow {
			get { return applyToRow; }
			set {
				if(ApplyToRow == value) return;
				applyToRow = value;
				ItemChanged();
			}
		}
		protected override void AssignColumn(object colObject) {
			base.AssignColumn(null);
			GridColumn column = (GridColumn)colObject;
			if(this.Collection.View is ColumnView) {
				ColumnView view = this.Collection.View as ColumnView;
				if(column != null && column.AbsoluteIndex != -1) 
					base.AssignColumn(view.Columns[column.AbsoluteIndex]);
			}
		}
		public override void Assign(StyleFormatConditionBase source) {
			StyleFormatCondition condition = (StyleFormatCondition)source;
			this.applyToRow = condition.ApplyToRow;
			base.Assign(source);
		}
		protected override List<IDataColumnInfo> GetColumns() {
			if(Collection == null || Collection.View == null) return base.GetColumns();
			List<IDataColumnInfo> res = new List<IDataColumnInfo>();
			foreach(GridColumn col in ((ColumnView)Collection.View).Columns) res.Add(new GridColumnIDataColumnInfoWrapper(col, GridColumnIDataColumnInfoWrapperEnum.General));
			return res;
		}
		protected override bool IsFitCore(ExpressionEvaluator evaluator, object val, object row) {
			try {
				object res = evaluator.Evaluate(row);
				if(res is bool) return (bool)res;
				return Convert.ToBoolean(res);
			}
			catch {
				return false;
			}
		}
		protected override DataControllerBase Controller {
			get {
				if(Collection == null || Collection.View == null) return null;
				return Collection.View.DataController;
			}
		}
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return Collection == null || Collection.View == null ? null : Collection.View.ElementsLookAndFeel; }
		}
		#endregion
	}
	#endregion
}
