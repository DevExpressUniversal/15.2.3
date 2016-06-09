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
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[LookupEditCustomBindingProperties("LookUpEdit")]
	public class RepositoryItemLookUpEdit : RepositoryItemLookUpEditBase, IDataInfo {
		bool showLines, showHeader, caseSensitiveSearch, 
			hotTrackItems, throwExceptionOnInvalidLookUpEditValueType, useCtrlScroll, useDropDownRowsAsMaxCount = false;
		int dropDownRows, dropDownItemHeight, autoSearchColumnIndex, sortColumnIndex;
		int lockFormatParseCounter, bestFitRowCount;
		SearchMode searchMode;
		HeaderClickMode headerClickMode;
		LookUpColumnInfoCollection columns;
		Hashtable displayValues;
		LookUpListDataAdapter dataAdapter;
		HighlightStyle highlightedItemStyle;
		AppearanceObject appearanceDropDownHeader;
		public RepositoryItemLookUpEdit() {
			this.bestFitRowCount = 0;
			this.appearanceDropDownHeader = CreateAppearance("DropDownHeader");
			this.showLines = this.showHeader = this.hotTrackItems = 
				this.useCtrlScroll = true;
			this.caseSensitiveSearch = this.throwExceptionOnInvalidLookUpEditValueType = false;
			this.dropDownRows = 7;
			this.dropDownItemHeight = 0;
			this.autoSearchColumnIndex = this.sortColumnIndex = 0;
			this.lockFormatParseCounter = 0;
			this.highlightedItemStyle = HighlightStyle.Default;
			this.searchMode = SearchMode.AutoFilter;
			this.headerClickMode = HeaderClickMode.Sorting;
			this.columns = CreateColumns();
			this.dataAdapter = CreateDataAdapter();
			this.dataAdapter.DataSourceChanged += new EventHandler(OnDataSourceChanged);
			this.dataAdapter.AdapterListChanged += new ListChangedEventHandler(OnListChanged);
			this.columns.CollectionChanged += new CollectionChangeEventHandler(Columns_CollectionChanged);
			this.displayValues = new Hashtable();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DataAdapter.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
				DataAdapter.AdapterListChanged -= new ListChangedEventHandler(OnListChanged);
				DataAdapter.Dispose();
				Columns.CollectionChanged -= new CollectionChangeEventHandler(Columns_CollectionChanged);
			}
			base.Dispose(disposing);
		}
		protected override void OnFormat_Changed(object sender, EventArgs e) {
			base.OnFormat_Changed(sender, e);
			this.displayValues.Clear();
		}
		protected override void DestroyAppearances() {
			base.DestroyAppearances();
			DestroyAppearance(AppearanceDropDownHeader);
		}
		void ResetAppearanceDropDownHeader() { AppearanceDropDownHeader.Reset(); }
		bool ShouldSerializeAppearanceDropDownHeader() { return AppearanceDropDownHeader.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditAppearanceDropDownHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDropDownHeader {
			get { return appearanceDropDownHeader; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditHighlightedItemStyle"),
#endif
 DefaultValue(HighlightStyle.Default), DXCategory(CategoryName.Appearance)]
		public virtual HighlightStyle HighlightedItemStyle {
			get { return highlightedItemStyle; }
			set {
				if(HighlightedItemStyle == value) return;
				highlightedItemStyle = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnBestFitModeChanged() {
			ClearPopupWidth();
			base.OnBestFitModeChanged();
		}
		internal void BeginLockFormatParseUpdate() { lockFormatParseCounter++; }
		internal void CancelLockFormatParseUpdate() { lockFormatParseCounter--; }
		protected internal override bool LockFormatParse {
			get { return base.LockFormatParse; }
			set {
				if(lockFormatParseCounter == 0)
					base.LockFormatParse = value;
			}
		}
		protected override void OnPropertiesChanged() {
			if(OwnerEdit != null && OwnerEdit.visualLayoutUpdate > 0) {
				return;
			}
			base.OnPropertiesChanged();
		}
		protected virtual LookUpColumnInfoCollection CreateColumns() {
			return new LookUpColumnInfoCollection(this);
		}
		protected virtual LookUpListDataAdapter CreateDataAdapter() {
			return new LookUpListDataAdapter(this);
		}
		public virtual void ForceInitialize() {
			ActivateDataSource(ActivationMode.Immediate);
		}
		protected internal enum ActivationMode { Immediate, Normal, BindingContext }
		protected internal virtual void ActivateDataSource(ActivationMode mode) {
			if(mode != ActivationMode.Immediate && (IsLoading || (OwnerEdit != null && !OwnerEdit.IsHandleCreated))) return;
			BindingContext context = OwnerEdit == null ? null : OwnerEdit.BindingContext;
			if(!MasterDetailHelper.IsDataSourceReady(context, DataSource, string.Empty)) return;
			bool lockPropertiesChanged = (mode == ActivationMode.BindingContext && IsDesignMode);
			if(lockPropertiesChanged) BeginUpdate();
			try {
				DataAdapter.SetDataSource(MasterDetailHelper.GetDataSource(context, DataSource, string.Empty), DisplayMember, ValueMember);
			}
			finally {
				if(lockPropertiesChanged) CancelUpdate();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemLookUpEdit Properties { get { return this; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteLookUpData)]
		public RepositoryItemLookUpEdit LookUpData { get { return this; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteLookUpKeyField)]
		public string KeyField { get { return ValueMember;} set { ValueMember = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteLookUpDisplayField)]
		public string DisplayField { get { return DisplayMember;} set { DisplayMember = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteLookUpKeyValue)]
		public object KeyValue { get { return (OwnerEdit == null ? null : OwnerEdit.EditValue); } set { if(OwnerEdit != null) OwnerEdit.EditValue = value; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "LookUpEdit"; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteLookUpNullString)]
		public string NullString { get { return NullText; } set { NullText = value; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditShowLines"),
#endif
 DefaultValue(true)]
		public bool ShowLines {
			get { return showLines; }
			set {
				if(ShowLines == value) return;
				showLines = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditShowHeader"),
#endif
 DefaultValue(true)]
		public virtual bool ShowHeader {
			get { return showHeader; }
			set {
				if(ShowHeader == value) return;
				showHeader = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditCaseSensitiveSearch"),
#endif
 DefaultValue(false)]
		public bool CaseSensitiveSearch {
			get { return caseSensitiveSearch; }
			set {
				if(CaseSensitiveSearch == value) return;
				caseSensitiveSearch = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeUseCtrlScroll() {
			if(OwnerEdit == null) return !UseCtrlScroll;
			return (UseCtrlScroll != ((LookUpEdit)OwnerEdit).DefaultUseCtrlScroll); 
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditUseCtrlScroll")
#else
	Description("")
#endif
]
		public virtual bool UseCtrlScroll {
			get { return useCtrlScroll; }
			set {
				if(UseCtrlScroll == value) return;
				useCtrlScroll = value;
				OnPropertiesChanged();
			}
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(UseCtrlScroll) {
				if(key == (Keys.Up | Keys.Control))
					return true;
				if(key == (Keys.Down | Keys.Control))
					return true;
			} else {
				if(key == Keys.Up)
					return true;
				if(key == Keys.Down)
					return true;
			}
			return base.NeededKeysContains(key);
		}
		protected internal virtual int GetDropDownRows() {
			if(UseDropDownRowsAsMaxCount && DataAdapter != null) {
				if(DataAdapter.ListSourceRowCount > 0 && DataAdapter.ListSourceRowCount < DropDownRows) return DataAdapter.ListSourceRowCount;
			}
			return DropDownRows;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditDropDownRows"),
#endif
 DefaultValue(7), SmartTagProperty("DropDown Rows", "")]
		public int DropDownRows {
			get { return dropDownRows; }
			set {
				if(value < 1) value = 1;
				if(DropDownRows == value) return;
				dropDownRows = value;
				ClearDropDownRows();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditUseDropDownRowsAsMaxCount"),
#endif
 DefaultValue(false)]
		public bool UseDropDownRowsAsMaxCount {
			get { return useDropDownRowsAsMaxCount; }
			set {
				if(UseDropDownRowsAsMaxCount == value) return;
				useDropDownRowsAsMaxCount = value;
				CheckDropDownRows();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditDropDownItemHeight"),
#endif
 DefaultValue(0)]
		public int DropDownItemHeight {
			get { return dropDownItemHeight; }
			set {
				if(value < 1) value = 0;
				if(DropDownItemHeight == value) return;
				dropDownItemHeight = value;
				OnPropertiesChanged();
			}
		}
		protected override void OnPopupFormSizeChanged() {
			ClearPopupWidth();
			base.OnPopupFormSizeChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get { return base.PopupFormSize; }
			set { base.PopupFormSize = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditPopupWidth"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(0)]
		public virtual int PopupWidth {
			get { return PopupFormSize.Width; }
			set { PopupFormSize = new Size(value, 0); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditSearchMode"),
#endif
 DefaultValue(SearchMode.AutoFilter), SmartTagProperty("Search Mode", "")]
		public SearchMode SearchMode {
			get { return searchMode; }
			set {
				if(SearchMode == value) return;
				searchMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditHeaderClickMode"),
#endif
 DefaultValue(HeaderClickMode.Sorting)]
		public HeaderClickMode HeaderClickMode {
			get { return headerClickMode; }
			set {
				if(HeaderClickMode == value) return;
				headerClickMode = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditAutoSearchColumnIndex"),
#endif
 DefaultValue(0)]
		public int AutoSearchColumnIndex {
			get { return autoSearchColumnIndex; }
			set {
				if(AutoSearchColumnIndex == value) return;
				autoSearchColumnIndex = value;
				ClearAutoSearchColumnIndex();
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditSortColumnIndex"),
#endif
 DefaultValue(0)]
		public int SortColumnIndex {
			get { return sortColumnIndex; }
			set {
				if(SortColumnIndex == value) return;
				sortColumnIndex = value;
				OnSortColumnIndexChanged();
			}
		}
		protected virtual void OnSortColumnIndexChanged() {
			ClearSortColumnIndex();
			OnPropertiesChanged();
			if(IsLoading) return;
			DataAdapter.SortBySortColumn();
		}
		protected override void OnDataSourceChanged() {
			ActivateDataSource(ActivationMode.Normal);
			base.OnDataSourceChanged();
		}
		protected override void OnValueMemberChanged(string oldValue, string newValue) {
			if(!IsLoading && OwnerEdit != null) {
				if(oldValue != string.Empty) {
					int index = GetDataSourceRowIndex(oldValue, OwnerEdit.EditValue);
					OwnerEdit.EditValue = DataAdapter.GetKeyValue(index);
				}
			}
			ActivateDataSource(ActivationMode.Normal);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoleteHotTrackItems)]
		public bool HotTrackRows {
			get { return HotTrackItems; }
			set { HotTrackItems = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditBestFitRowCount"),
#endif
 DefaultValue(0)]
		public int BestFitRowCount {
			get { return bestFitRowCount; }
			set {
				if(value < 0) value = 0;
				if(BestFitRowCount == value) return;
				bestFitRowCount = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditHotTrackItems"),
#endif
 DefaultValue(true)]
		public bool HotTrackItems {
			get { return hotTrackItems; }
			set {
				if(HotTrackItems == value) return;
				hotTrackItems = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditThrowExceptionOnInvalidLookUpEditValueType"),
#endif
 DefaultValue(false)]
		public virtual bool ThrowExceptionOnInvalidLookUpEditValueType {
			get { return throwExceptionOnInvalidLookUpEditValueType; }
			set { 
				if(ThrowExceptionOnInvalidLookUpEditValueType == value) return;
				throwExceptionOnInvalidLookUpEditValueType = value; 
				OnPropertiesChanged();
			}
		}
		[Localizable(true), DXCategory(CategoryName.Data), MergableProperty(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LookUpColumnInfoCollection Columns {
			get { return columns; }
		}
		[Browsable(false)]
		public new LookUpEdit OwnerEdit { get { return base.OwnerEdit as LookUpEdit; } }
		public override void Assign(RepositoryItem item) {
			RepositoryItemLookUpEdit source = item as RepositoryItemLookUpEdit;
			BeginUpdate(); 
			try {
				base.Assign(item);
				if(source == null) return;
				this.bestFitRowCount = source.BestFitRowCount;
				this.caseSensitiveSearch = source.CaseSensitiveSearch;
				this.dropDownItemHeight = source.DropDownItemHeight;
				this.dropDownRows = source.DropDownRows;
				this.useDropDownRowsAsMaxCount = source.UseDropDownRowsAsMaxCount;
				this.autoSearchColumnIndex = source.AutoSearchColumnIndex;
				this.sortColumnIndex = source.SortColumnIndex;
				this.hotTrackItems = source.HotTrackItems;
				this.throwExceptionOnInvalidLookUpEditValueType = source.ThrowExceptionOnInvalidLookUpEditValueType;
				this.highlightedItemStyle = source.HighlightedItemStyle;
				this.searchMode = source.SearchMode;
				this.headerClickMode = source.HeaderClickMode;
				this.showHeader = source.ShowHeader;
				this.showLines = source.ShowLines;
				this.useCtrlScroll = source.UseCtrlScroll;
				this.AppearanceDropDownHeader.Assign(source.AppearanceDropDownHeader);
				AssignColumns(source.Columns);
				this.DataAdapter.FilterPrefix = string.Empty;
				ActivateDataSource(ActivationMode.Normal);
			}
			finally {
				EndUpdate();
			}
			Events.AddHandler(getNotInListValue, source.Events[getNotInListValue]);
			Events.AddHandler(listChanged, source.Events[listChanged]);
		}
		protected virtual void AssignColumns(LookUpColumnInfoCollection source) {
			this.Columns.Assign(source);
			for(int i = 0; i < Columns.Count; i++) {
				if(!PropertyStore.Contains(Columns[i].FieldName)) continue;
				LookUpColumnPopupSaveInfo colInfo = (LookUpColumnPopupSaveInfo)PropertyStore[Columns[i].FieldName];
				Columns[i].Init(colInfo);
			}
		}
		protected override void OnContainerLoaded() {
			base.OnContainerLoaded();
			ActivateDataSource(ActivationMode.Immediate);
		}
		protected internal LookUpListDataAdapter DataAdapter { get { return dataAdapter; } }
		protected virtual void OnDataSourceChanged(object sender, EventArgs e) {
			ResetDisplayValues();
			if(DesignMode) {
				LayoutChanged();
				return;
			}
			OnPropertiesChanged();
		}
		protected internal void ResetDisplayValues() {
			this.displayValues.Clear();
		}
		protected virtual void OnListChanged(object sender, ListChangedEventArgs e) {
			displayValues.Clear();
			if(OwnerEdit != null) {
				if(OwnerEdit.InvokeRequired)
					OwnerEdit.BeginInvoke(new ListChangedEventHandler(this.OnListChanged), new object[] {sender, e});
				else
					OwnerEdit.OnListChanged(e);
			}
			RaiseListChanged(e);
		}
		protected virtual void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(populatingColumns) {
				ResetDisplayValues();
				return;
			}
			if(!IsLoading)
				DataAdapter.ColumnCollectionChanged(e);
			if(e.Action == CollectionChangeAction.Refresh) {
				if(e.Element != null) {
					LookUpColumnInfo col = e.Element as LookUpColumnInfo;
					if(PropertyStore.Contains(col.FieldName)) {
						LookUpColumnPopupSaveInfo colInfo = (LookUpColumnPopupSaveInfo)PropertyStore[col.FieldName];
						colInfo.Width = col.Width;
						colInfo.SortOrder = col.SortOrder;
					}
					return;
				}
				if(Columns.Count == 0) {
					ArrayList list = new ArrayList(PropertyStore.Keys);
					foreach(object key in list) {
						if(key is LookUpColumnInfo)
							PropertyStore.Remove(((LookUpColumnInfo)key).FieldName);
					}
				}
			}
			if(e.Action == CollectionChangeAction.Remove && e.Element != null) {
				LookUpColumnInfo col = e.Element as LookUpColumnInfo;
				if(PropertyStore.Contains(col.FieldName)) PropertyStore.Remove(col.FieldName);
			}
			ResetDisplayValues();
			OnPropertiesChanged();
		}
		bool populatingColumns = false;
		public virtual void PopulateColumns() {
			BeginUpdate();
			try {
				this.populatingColumns = true;
				Columns.Clear();
				for(int i = 0; i < DataAdapter.Columns.Count; i ++) {
					if(!DataAdapter.Columns[i].Browsable) continue;
					LookUpColumnInfo ci = Columns.CreateColumn();
					ci.FieldName = DataAdapter.Columns[i].Name;
					ci.Caption = DataAdapter.Columns[i].Caption;
					ci.Width = 20;
					Type columnType = DataAdapter.Columns[i].Type;
					if(EnumDisplayTextHelper.IsEnum(columnType))
						EnumDisplayTextHelper.ResetDisplayTextsCache(columnType);
					ci.SetFormatInfo(columnType);
					Columns.Add(ci);
				}
				BestFit();
			}
			finally {
				this.populatingColumns = false;
				EndUpdate();
			}
		}
		public virtual int BestFit() {
			if(Columns.Count == 0) return 0;
			int total = 0;
			GraphicsInfo ginfo = new GraphicsInfo();
			Graphics g = ginfo.AddGraphics(null);
			try {
				for(int n = 0; n < Columns.Count; n++) {
					LookUpColumnInfo column = Columns[n];
					if(!column.Visible) continue;
					column.Width = MeasureColumn(g, column);
					if(PropertyStore.Contains(column.FieldName)) {
						LookUpColumnPopupSaveInfo colInfo = (LookUpColumnPopupSaveInfo)PropertyStore[column.FieldName];
						colInfo.Width = column.Width;
					}
					total += column.Width;
				}
			} finally {
				ginfo.ReleaseGraphics();
			}
			total += SystemInformation.VerticalScrollBarWidth;
			return total;
		}
		public virtual int MeasureColumn(Graphics g, LookUpColumnInfo column) {
			int width = 0;
			int rowCount = DataAdapter.ItemCount;
			if(BestFitRowCount > 0) rowCount = Math.Min(rowCount, BestFitRowCount);
			for(int i = 0; i < rowCount; i++) {
				string s = DataAdapter.GetCellString(column, i);
				width = Math.Max(width, Appearance.CalcTextSize(g, s, 0).ToSize().Width + 8);
			}
			if(ShowHeader) {
				width = Math.Max(width, Appearance.CalcTextSize(g, column.Caption, 0).ToSize().Width + 10);
				if(Columns.VisibleIndexOf(column) == AutoSearchColumnIndex) width += 11 + 2;
			}
			return width;
		}
		protected override bool NeededKeysPopupContains(Keys key) {
			switch(key) {
				case Keys.Left:
				case Keys.Right:
				case Keys.Up:
				case Keys.Down:
				case Keys.PageUp:
				case Keys.PageDown:
				case Keys.Enter:
				case Keys.Enter | Keys.Control:
				case Keys.Enter | Keys.Shift:
				case Keys.Delete:
					return true;
			}
			return base.NeededKeysPopupContains(key);
		}
		protected internal override bool AllowInputOnOpenPopup { get { return SearchMode == SearchMode.AutoFilter; } }
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(OwnerEdit != null && !OwnerEdit.IsDisplayTextValid) {
				return GetValueDisplayText(format, OwnerEdit.AutoSearchText);
			}
			string displayText = GetLookUpDisplayText(format, editValue);
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(editValue, displayText);
			if(format != EditFormat) RaiseCustomDisplayText(e);
			return GetNormalizedText(e.DisplayText);
		}
		string GetLookUpDisplayText(FormatInfo format, object editValue) {
			if(DataSource == null && !IsNullValue(editValue)) return GetNullEditText();
			if(IsNullValue(editValue)) return GetNullText(format);
			string dispText = (string)displayValues[editValue];
			if(dispText == null) {
				object dispValue = GetDisplayValueByKeyValue(editValue);
				if(editValue != null && dispValue == null)
					dispText = string.Empty;
				dispText = GetValueDisplayText(format, dispValue);
				if(!DataAdapter.IsFiltered)
					displayValues[editValue] = dispText;
			}
			return dispText;
		}
		internal void RemoveDisplayValue(object editValue) {
			if(IsNullValue(editValue)) return;
			displayValues.Remove(editValue);
		}
		protected internal string GetValueDisplayText(FormatInfo format, object displayValue) {
			if(IsNullValue(displayValue)) return string.Empty;
			return GetNormalizedText(format.GetDisplayText(displayValue));
		}
		public virtual object GetDisplayValueByKeyValue(object keyValue) {
			int index = DataAdapter.FindValueIndex(ValueMember, keyValue);
			return DataAdapter.GetValueAtIndex(DisplayMember, index);
		}
		public virtual object GetKeyValueByDisplayValue(object displayValue) {
			int index = DataAdapter.FindValueIndex(DisplayMember, displayValue);
			return DataAdapter.GetKeyValue(index);
		}
		public virtual object GetKeyValueByDisplayText(string displayText) {
			if(OwnerEdit != null) OwnerEdit.LockIsDisplayTextValid();
			try {
				for(int i = 0; i < DataAdapter.ItemCount; i++) {
					object result = DataAdapter.GetKeyValue(i);
					string dispText = GetDisplayText(result);
					if(dispText == displayText)
						return result;
				}
			}
			finally {
				if(OwnerEdit != null) OwnerEdit.UnlockIsDisplayTextValid();
			}
			return null;
		}
		protected override ExportMode GetExportMode() {
			if (ExportMode == ExportMode.Default) return ExportMode.DisplayText;
			return ExportMode;
		}
		public virtual object GetDataSourceRowByKeyValue(object keyValue) {
			int index = DataAdapter.FindValueIndex(ValueMember, keyValue);
			return DataAdapter.GetDataSourceRowAtIndex(index);
		}
		public virtual object GetDataSourceRowByDisplayValue(object displayValue) {
			int index = DataAdapter.FindValueIndex(DisplayMember, displayValue);
			return DataAdapter.GetDataSourceRowAtIndex(index);
		}
		public virtual object GetDataSourceRowByMultipleKeyValues(object keyValue, object[] otherValues, string[] otherFields) {
			if(otherValues == null || otherValues.Length == 0 || otherFields == null ||
				otherFields.Length == 0) return GetDataSourceRowByKeyValue(keyValue);
			return DataAdapter.GetDataSourceRowAtIndex(GetDataSourceRowIndexByMultipleKeyValues(keyValue, otherValues, otherFields));
		}
		public virtual object GetDisplayValueByMultipleKeyValues(object keyValue, object[] otherValues, string[] otherFields) {
			if(otherValues == null || otherValues.Length == 0 || otherFields == null ||
				otherFields.Length == 0) return GetDisplayValueByKeyValue(keyValue);
			return DataAdapter.GetValueAtIndex(DisplayMember, GetDataSourceRowIndexByMultipleKeyValues(keyValue, otherValues, otherFields));
		}
		public virtual object GetDataSourceValue(LookUpColumnInfo column, int rowIndex) {
			if(column == null) return null;
			return GetDataSourceValue(column.FieldName, rowIndex);
		}
		public virtual object GetDataSourceValue(string fieldName, int rowIndex) {
			return DataAdapter.GetValueAtIndex(fieldName, rowIndex);
		}
		public virtual int GetDataSourceRowIndex(LookUpColumnInfo column, object value) {
			if(column == null) return -1;
			return GetDataSourceRowIndex(column.FieldName, value);
		}
		public int GetListSourceIndex(int index) {
			return DataAdapter.GetListSourceRowIndex(index);
		}
		public virtual int GetDataSourceRowIndex(string fieldName, object value) {
			return DataAdapter.FindValueIndex(fieldName, value);
		}
		private int GetDataSourceRowIndexByMultipleKeyValues(object keyValue, object[] otherValues, string[] otherFields) {
			if(otherValues.Length != otherFields.Length) throw new IndexOutOfRangeException(Localizer.Active.GetLocalizedString(StringId.NotValidArrayLength));
			int index = 0;
			object Null = DataAdapter.Null;
			while(index < DataAdapter.ItemCount) {
				index = DataAdapter.FindValueIndex(ValueMember, keyValue, index);
				if(index == -1) return -1;
				bool found = true;
				for(int i = 0; i < otherValues.Length; i++) {
					object val = DataAdapter.GetValueAtIndex(otherFields[i], index);
					if(val == Null) {
						if(otherValues[i] == Null) continue;
					}
					else if(val.Equals(otherValues[i])) continue;
					else { 
						found = false;
						break;
					}
				}
				if(found) return index;
				else index++;
			}
			return -1;
		}
		private void ClearPopupWidth() {
			if(PropertyStore.Contains(LookUpPropertyNames.PopupWidth)) PropertyStore.Remove(LookUpPropertyNames.PopupWidth);
			if(PropertyStore.Contains(LookUpPropertyNames.PopupBestWidth)) PropertyStore.Remove(LookUpPropertyNames.PopupBestWidth);
		}
		internal void ClearDropDownRows() {
			if(PropertyStore.Contains(LookUpPropertyNames.DropDownRows)) PropertyStore.Remove(LookUpPropertyNames.DropDownRows);
		}
		internal bool CheckDropDownRows() {
			object rowsCount = PropertyStore[LookUpPropertyNames.DropDownRows];
			if(rowsCount != null) {
				if((int)rowsCount != GetDropDownRows()) {
					ClearDropDownRows();
					ResetPopupFormSize();
					return false;
				}
			}
			return true;
		}
		private void ClearAutoSearchColumnIndex() {
			if(PropertyStore.Contains(LookUpPropertyNames.AutoSearchColumnIndex)) PropertyStore.Remove(LookUpPropertyNames.AutoSearchColumnIndex);
		}
		private void ClearSortColumnIndex() {
			if(PropertyStore.Contains(LookUpPropertyNames.SortColumnIndex)) PropertyStore.Remove(LookUpPropertyNames.SortColumnIndex);
		}
		protected override bool IsNeededKeyCore(Keys keyData) {
			if(keyData == Keys.Return && TextEditStyle == TextEditStyles.Standard) {
				if(OwnerEdit != null && OwnerEdit.IsPopupOpen) return true;
				if(OwnerEdit != null) {
					if(IsProcessNewValueExists && OwnerEdit.IsModified && !OwnerEdit.IsDisplayTextValid) return true;
				}
				return false;
			}
			return base.IsNeededKeyCore(keyData);
		}
		[Browsable(false)]
		public override bool RequireDisplayTextSorting { get { return true; } }
		static readonly object getNotInListValue = new object();
		static readonly object listChanged = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditGetNotInListValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event GetNotInListValueEventHandler GetNotInListValue {
			add {
				displayValues.Clear();
				this.Events.AddHandler(getNotInListValue, value); 
				OnPropertiesChanged();
			}
			remove {
				displayValues.Clear();
				this.Events.RemoveHandler(getNotInListValue, value); 
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookUpEditListChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListChangedEventHandler ListChanged {
			add { this.Events.AddHandler(listChanged, value); }
			remove { this.Events.RemoveHandler(listChanged, value); }
		}
		protected internal virtual void RaiseGetNotInListValue(GetNotInListValueEventArgs e) {
			GetNotInListValueEventHandler handler = (GetNotInListValueEventHandler)Events[getNotInListValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		internal bool CanRaiseGetNotInListValue { get { return (GetNotInListValueEventHandler)Events[getNotInListValue] != null; } }
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			ListChangedEventHandler handler = (ListChangedEventHandler)Events[listChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	public enum BestFitMode { None, BestFit, BestFitResizePopup }
	public enum SearchMode { OnlyInPopup, AutoComplete, AutoFilter }
	public enum HeaderClickMode { Sorting, AutoSearch }
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
	public class LookUpColumnInfo {
		protected FormatInfo fFormat;
		private int width;
		string fieldName, caption;		
		HorzAlignment alignment;
		bool visible;
		ColumnSortOrder sortOrder;
		LookUpColumnInfoCollection owner;
		static int generatorIndex = 0;
		DefaultBoolean allowSort = DefaultBoolean.Default;
		public LookUpColumnInfo() : this(string.Format("{0}{1}", Localizer.Active.GetLocalizedString(StringId.LookUpColumnDefaultName), ++generatorIndex)) {}
		public LookUpColumnInfo(string fieldName) : this(fieldName, 20) {}
		public LookUpColumnInfo(string fieldName, int width) : this(fieldName, fieldName, width) {}
		public LookUpColumnInfo(string fieldName, int width, string caption) : this(fieldName, caption, width) {}
		public LookUpColumnInfo(string fieldName, string caption) : this(fieldName, caption, 20) { }
		public LookUpColumnInfo(string fieldName, string caption, int width) : this(fieldName, caption, width, FormatType.None, string.Empty, true, HorzAlignment.Default) {}
		public LookUpColumnInfo(string fieldName, string caption, int width, FormatType formatType, string formatString, bool visible, HorzAlignment alignment) :
			this(fieldName, caption, width, formatType, formatString, visible, alignment, ColumnSortOrder.None) {}
		public LookUpColumnInfo(string fieldName, string caption, int width, FormatType formatType, string formatString, bool visible, HorzAlignment alignment, ColumnSortOrder sortOrder)
			:
			this(fieldName, caption, width, formatType, formatString, visible, alignment, sortOrder, DefaultBoolean.Default) { }
		public LookUpColumnInfo(string fieldName, string caption, int width, FormatType formatType, string formatString, bool visible, HorzAlignment alignment, ColumnSortOrder sortOrder, DefaultBoolean allowSort) {
			fFormat = new FormatInfo();
			fFormat.FormatType = formatType;
			fFormat.FormatString = formatString;
			this.allowSort = allowSort;
			this.fieldName = fieldName;
			this.width = width;
			this.caption = caption;
			this.visible = visible;
			this.alignment = alignment;
			this.sortOrder = sortOrder;
			this.owner = null;
		}
		public override string ToString() {
			return string.Format("{0} (\"{1}\")", Caption, FieldName);
		}
		internal void Init(LookUpColumnPopupSaveInfo colInfo) {
			if(colInfo == null) return;
			this.width = colInfo.Width;
			this.sortOrder = colInfo.SortOrder;
		}
		public virtual void Assign(LookUpColumnInfo source) {
			this.fFormat.Assign(source.fFormat);
			this.width = source.Width;
			this.fieldName = source.FieldName;		
			this.caption = source.Caption;
			this.alignment = source.Alignment;
			this.visible = source.Visible;
			this.allowSort = source.AllowSort;
			this.sortOrder = source.SortOrder;
			Changed();
		}
		protected internal virtual void SetFormatInfo(Type aType) {
			FormatType = FormatType.None;
			FormatString = string.Empty;
			Alignment = HorzAlignment.Near;
			TypeCode tc = Type.GetTypeCode(aType);
			if(tc == TypeCode.DateTime) {
				FormatType = FormatType.DateTime;
				FormatString = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
				return;
			}
			if(tc == TypeCode.SByte || tc == TypeCode.Decimal ||
				tc == TypeCode.Double || tc == TypeCode.Int16 ||
				tc == TypeCode.Int32 || tc == TypeCode.Int64 ||
				tc == TypeCode.Single || tc == TypeCode.UInt16 ||
				tc == TypeCode.UInt32 || tc == TypeCode.UInt64) {
				FormatType = FormatType.Numeric;
				Alignment = HorzAlignment.Far;
				FormatString = string.Empty;
			}
		}
		[DefaultValue(DefaultBoolean.Default), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoAllowSort")
#else
	Description("")
#endif
]
		public DefaultBoolean AllowSort {
			get { return allowSort; }
			set {
				if(AllowSort == value) return;
				allowSort = value;
				Changed();
			}
		}
		[DefaultValue(20), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoWidth")
#else
	Description("")
#endif
]
		public int Width { 
			get { return width; } 
			set { 
				value = Math.Max(MinColumnWidth, value);
				if(value == Width) return;
				width = value;
				Changed();
			} 
		}
		[DefaultValue(FormatType.None), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoFormatType")
#else
	Description("")
#endif
]
		public FormatType FormatType { 
			get { return fFormat.FormatType; } 
			set {
				if(FormatType == value) return;
				fFormat.FormatType = value;
				Changed();
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoFormatString"),
#endif
 Localizable(true)]
		public string FormatString { 
			get { return fFormat.FormatString; } 
			set {
				if(FormatString == value) return;
				fFormat.FormatString = value;
				Changed();
			}
		}
		[DefaultValue(HorzAlignment.Default), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoAlignment")
#else
	Description("")
#endif
]
		public HorzAlignment Alignment { 
			get { return alignment; } 
			set {
				if(Alignment == value) return;
				alignment = value;
				Changed();
			}
		}
		[DefaultValue(true), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoVisible")
#else
	Description("")
#endif
]
		public bool Visible { 
			get { return visible; } 
			set {
				if(Visible == value) return;
				visible = value;
				Changed();
			}
		}
		[DefaultValue(ColumnSortOrder.None), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoSortOrder")
#else
	Description("")
#endif
]
		public ColumnSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				Changed();
			}
		}
		internal static int MinColumnWidth { get { return 5; } }
		[DefaultValue(""), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoCaption"),
#endif
 Localizable(true)]
		public string Caption { 
			get { return caption; } 
			set { 
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				caption = value;
				Changed();
			}
		}
		[DefaultValue(""), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoFieldName"),
#endif
 TypeConverter("DevExpress.XtraEditors.Design.LookUpColumnDataMemberTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
		public string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value) return;
				fieldName = value;
				Changed();
			}
		}
		protected virtual void Changed() {
			if(Owner != null)
				Owner.OnItemChanged(this);
		}
		protected internal LookUpColumnInfoCollection Owner { get { return owner; } set { owner = value; } }
		public object GetOwner() { return Owner.GetOwnerControl(); }
	}
	[ListBindable(false), Editor("DevExpress.XtraEditors.Design.LookUpColumnInfoCollectionEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, "System.Drawing.Design.UITypeEditor, System.Drawing")]
	public class LookUpColumnInfoCollection : CollectionBase {
		object owner;
		int lockUpdate;
		public event CollectionChangeEventHandler CollectionChanged;
		public LookUpColumnInfoCollection() : this(null) {}
		public LookUpColumnInfoCollection(object owner) {
			this.owner = owner;
			this.lockUpdate = 0;
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoCollectionItem")]
#endif
		public LookUpColumnInfo this[int index] { get { return List[index] as LookUpColumnInfo; } }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoCollectionItem")]
#endif
		public LookUpColumnInfo this[string fieldName] { 
			get { 
				for(int i = 0; i < Count; i++)
					if(this[i].FieldName == fieldName) return this[i];
				return null;
			} 
		}
		protected virtual void BeginUpdate() { lockUpdate++; }
		protected virtual void EndUpdate() {
			if(--lockUpdate == 0) 
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		public virtual void Assign(LookUpColumnInfoCollection columns) {
			BeginUpdate();
			try {
				Clear();
				if(columns == null) return;
				for(int n = 0; n < columns.Count; n++) {
					LookUpColumnInfo column = columns[n];
					LookUpColumnInfo newColumn = CreateColumn();
					newColumn.Assign(column);
					Add(newColumn);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void AddRange(LookUpColumnInfo[] columns) {
			BeginUpdate();
			try {
				foreach(LookUpColumnInfo column in columns) {
					Add(column);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual int Add(LookUpColumnInfo column) {
			return List.Add(column);
		}
		public int IndexOf(LookUpColumnInfo column) {
			return List.IndexOf(column);
		}
		public LookUpColumnInfo GetVisibleColumn(int index) {
			int counter = -1;
			foreach(LookUpColumnInfo item in this) {
				if(item.Visible) counter++;
				if(counter == index) return item;
			}
			return null;
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("LookUpColumnInfoCollectionVisibleCount")]
#endif
		public int VisibleCount { 
			get {
				int counter = 0;
				foreach(LookUpColumnInfo item in this) {
					if(item.Visible) counter++;
				}
				return counter;
			}
		}
		public int VisibleIndexOf(LookUpColumnInfo column) {
			if(column == null || !column.Visible) return -1;
			int counter = -1;
			foreach(LookUpColumnInfo item in this) {
				if(item.Visible) counter++;
				if(item == column) return counter;
			}
			return -1;
		}
		public void Remove(LookUpColumnInfo column) {
			List.Remove(column);
		}
		public virtual LookUpColumnInfo CreateColumn() {
			return new LookUpColumnInfo();
		}
		protected override void OnInsertComplete(int index, object item) {
			SetOwner(item, true);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int index, object item) {
			SetOwner(item, false);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			for(int i = 0; i < Count; i++)
				SetOwner(InnerList[i], false);
		}
		protected override void OnClearComplete() {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		void SetOwner(object item, bool me) {
			LookUpColumnInfo column = (LookUpColumnInfo)item;
			column.Owner = (me ? this : null);
		}
		protected internal virtual void OnItemChanged(LookUpColumnInfo column) {
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, column));
		}
		protected virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(lockUpdate != 0) return;
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		internal object GetOwnerControl() { return owner; }
	}
	public delegate void GetNotInListValueEventHandler(object sender, GetNotInListValueEventArgs e);
	public delegate void ProcessNewValueEventHandler(object sender, ProcessNewValueEventArgs e);
	public class GetNotInListValueEventArgs : EventArgs {
		string fieldName;
		int recordIndex;
		object fValue;
		public GetNotInListValueEventArgs(string fieldName, int recordIndex) {
			this.fieldName = fieldName;
			this.recordIndex = recordIndex;
			this.fValue = null;
		}
		public string FieldName { get { return fieldName; } }
		public int RecordIndex { get { return recordIndex; } }
		public object Value { get { return fValue; } set { fValue = value; } }
	}
	public class AddNewValueEventArgs : CancelEventArgs {
		object newValue;
		public AddNewValueEventArgs() {
			this.newValue = null;
			Cancel = false;
		}
		public object NewValue { get { return newValue; } set { newValue = value; } }
	}
	public delegate void AddNewValueEventHandler(object sender, AddNewValueEventArgs e);
	public class ProcessNewValueEventArgs : EventArgs {
		object displayValue;
		bool handled;
		public ProcessNewValueEventArgs(object displayValue) {
			this.displayValue = displayValue;
			this.handled = false;
		}
		public object DisplayValue { get { return displayValue; } set { displayValue = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
	}
	public class InvalidLookUpEditValueTypeException : Exception {
		Type editValueType, valueMemberType;
		public InvalidLookUpEditValueTypeException(Type editValueType, Type valueMemberType) :
			base(Localizer.Active.GetLocalizedString(StringId.LookUpInvalidEditValueType)) {
			this.editValueType = editValueType;
			this.valueMemberType = valueMemberType;
		}
		public Type EditValueType { get { return editValueType; } }
		public Type ValueMemberType { get { return valueMemberType; } }
	}
	public class KeyPressHelper {
		string text, prevText;
		int selectionStart, selectionLength, maxLength, prevSelectionLength;
		char _char;
		public const char backSpaceChar = '\b';
		public KeyPressHelper(string text, int maxLength) : this(text, text.Length, 0, maxLength) {}
		public KeyPressHelper(string text, int selectionStart, int selectionLength, int maxLength) {
			this.text = (text == null ? string.Empty : text).Normalize();
			this.selectionStart = selectionStart;
			this.selectionLength = selectionLength;
			this.maxLength = maxLength;
			this._char = text.Length > 0 ? text[text.Length - 1] : '\0';
			this.prevText = this.text;
			this.prevSelectionLength = this.selectionLength;
		}
		public char CharValue { get { return _char; } }
		public void ProcessChar(char charCode) {
			this._char = charCode;
			string res = this.text;
			if(charCode == backSpaceChar) {
				if(SelectionLength == 0) {
					if(selectionStart == 0) return;
					selectionStart = Math.Max(0, SelectionStart - 1);
				}
				res = text.Remove(SelectionStart, Math.Min(text.Length - SelectionStart, Math.Max(1, SelectionLength)));
			}
			else if(maxLength <= 0 || text.Length - SelectionLength < maxLength) {
				res = text.Substring(0, SelectionStart) + charCode.ToString();
				int count = Math.Max(0, text.Length - (SelectionStart + SelectionLength));
				res += text.Substring(SelectionStart + SelectionLength, count);
				selectionStart++;
			}
			this.text = res;
		}
		public string Text { get { return text; } }
		public string PrevText { get { return prevText; } }
		public int PrevSelectionLength { get { return prevSelectionLength; } }
		public int SelectionStart { get { return selectionStart;} } 
		public int SelectionLength { get { return selectionLength; } }
		public int GetCorrectedAutoSearchSelectionStart(string currentText, char pressedChar) {
			if(pressedChar != backSpaceChar) return SelectionStart;
			if(PrevSelectionLength > 0)
				return Math.Max(0, SelectionStart - 1);
			return SelectionStart;
		}
	}
	public class LookUpColumnPopupSaveInfo {
		int width;
		ColumnSortOrder sortOrder;
		internal LookUpColumnPopupSaveInfo(int width, ColumnSortOrder sortOrder) {
			this.width = width;
			this.sortOrder = sortOrder;
		}
		public int Width { get { return width; } set { width = value; } }
		public ColumnSortOrder SortOrder { get { return sortOrder; } set { sortOrder = value; } }
	}
}
namespace DevExpress.XtraEditors.ListControls {
	public interface ILookUpDataFilter {
		int RowCount { get ; }
		string FilterPrefix { get; set; }
		int GetRecordIndex(int rowIndex);
		event EventHandler FilteredListChanged;
	}
	public class LookUpListDataAdapter : ListDataAdapter, ILookUpDataFilter {
		RepositoryItemLookUpEdit item;
		string filterPrefix;
		public LookUpListDataAdapter(RepositoryItemLookUpEdit item) {
			this.item = item;
			this.filterPrefix = string.Empty;
		}
		protected override bool SupportSortedProperty { get { return false; } }
		int ILookUpDataFilter.RowCount { get { return this.VisibleListSourceRowCount; } }
		public object GetKeyValue(int rowIndex) {
			return GetValueAtIndex(Item.ValueMember, rowIndex);
		}
		public string FilterPrefix { 
			get { return filterPrefix; } 
			set {
				if(value == null) value = string.Empty;
				if(FilterPrefix == value) return;
				filterPrefix = value;
				Refilter();
			} 
		}
		public bool CanSortColumn(LookUpColumnInfo column) {
			if(column == null || column.AllowSort == DefaultBoolean.False) return false;
			DataColumnInfo info = Columns[column.FieldName];
			if(info != null && CanSortColumn(info)) {
				if(column.AllowSort == DefaultBoolean.True) return true;
				return info.AllowSort;
			}
			return false;
		}
		internal void SortBySortColumn() {
			LookUpColumnInfo column = null;
			if(Item.SortColumnIndex > -1 && Item.SortColumnIndex < Item.Columns.Count)
				column = Item.Columns[Item.SortColumnIndex];
			SortByColumn(column);
		}
		public void SortByColumn(LookUpColumnInfo column) {
			if(!CanSortColumn(column) || column.SortOrder == ColumnSortOrder.None)
				SortInfo.Clear();
			else
				SortInfo.ClearAndAddRange(new DataColumnSortInfo[] { new DataColumnSortInfo(Columns[column.FieldName], column.SortOrder) });
		}
		protected override void OnRequestSynchronization() {
			SortBySortColumn();
		}
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			SortBySortColumn();
		}
		public virtual string FilterField { 
			get {
				string result = Item.DisplayMember;
				if(result == string.Empty) result = Item.ValueMember;
				if(result == string.Empty && Columns.Count == 1) result = Columns[0].Name;
				return result; 
			} 
		}
		void Refilter() {
			if(Item.OwnerEdit != null) Item.OwnerEdit.IncVisualLayoutUpdate();
			try {
				this.FilterExpression = CreateFilterExpression();
				if(FilteredListChanged != null)
					FilteredListChanged(this, EventArgs.Empty);
			}
			finally {
				if(Item.OwnerEdit != null) Item.OwnerEdit.DecVisualLayoutUpdate();
			}
		}
		protected virtual string CreateFilterExpression() {
			if(string.IsNullOrEmpty(FilterPrefix)) return string.Empty;
			return CriteriaOperator.ToString(new FunctionOperator(FunctionOperatorType.StartsWith, new OperandProperty(FilterField), FilterPrefix));
		}
		int ILookUpDataFilter.GetRecordIndex(int rowIndex) { return rowIndex; }
		public event EventHandler FilteredListChanged;
		public virtual string GetCellString(LookUpColumnInfo column, int recordIndex) {
			object obj = GetValueAtIndex(column.FieldName, recordIndex);
			if(Item.IsNullValue(obj)) return string.Empty;
			IFormattable ifl = obj as IFormattable;
			if(ifl != null) {
				switch(column.FormatType) {
					case FormatType.Numeric:
						if(string.IsNullOrEmpty(column.FormatString)) {
							if(EnumDisplayTextHelper.IsEnum(obj.GetType()))
								return EnumDisplayTextHelper.GetCachedDisplayText(obj);
						}
						return ifl.ToString(column.FormatString, null);
					case FormatType.DateTime:
					case FormatType.Custom:
						return ifl.ToString(column.FormatString, null);
					case FormatType.None:
						if(EnumDisplayTextHelper.IsEnum(obj.GetType()))
							return EnumDisplayTextHelper.GetCachedDisplayText(obj);
						break;
				}
			}
			return obj.ToString();
		}
		protected override object GetUnboundData(string propName, int index, object value) {
			GetNotInListValueEventArgs e = new  GetNotInListValueEventArgs(propName, index);
			Item.RaiseGetNotInListValue(e);
			return e.Value;
		}
		DataColumnInfoCollection GetBoundColumns() {
			DataColumnInfoCollection result = new DataColumnInfoCollection();
			DataColumnInfo[] columns = new MasterDetailHelper().GetDataColumnInfo(null, Item.DataSource, string.Empty);
			if(columns != null) {
				for(int i = 0; i < columns.Length; i++)
					result.Add(columns[i]);
			}
			return result;
		}
		protected override UnboundColumnInfoCollection GetNotInListColumns() {
			DataColumnInfoCollection collection = GetBoundColumns(); 
			UnboundColumnInfoCollection result = base.GetNotInListColumns();
			for(int i = 0; i < Item.Columns.Count; i++) {
				LookUpColumnInfo col = Item.Columns[i];
				AddNotInListColumn(collection, result, col.FieldName);
			}
			AddMemberColumn(collection, result, ValueMember);
			AddMemberColumn(collection, result, DisplayMember);
			return result;
		}
		void AddNotInListColumn(DataColumnInfoCollection boundColumns, UnboundColumnInfoCollection unboundColumns, string fieldName) {
			if(boundColumns.GetColumnIndex(fieldName) != -1) return;
			unboundColumns.Add(new UnboundColumnInfo(fieldName, UnboundColumnType.Object, true));
		}
		void AddMemberColumn(DataColumnInfoCollection boundColumns, UnboundColumnInfoCollection unboundColumns, string fieldName) {
			if(fieldName == string.Empty) return;
			if(Item.Columns[fieldName] != null) return;
			AddNotInListColumn(boundColumns, unboundColumns, fieldName);
		}
		internal void ColumnCollectionChanged(CollectionChangeEventArgs e) {
			if(!(e.Action == CollectionChangeAction.Refresh && e.Element != null)) { 
				SortInfo.Clear();
				PopulateColumns();
			}
			SortBySortColumn();
		}
		protected override bool UnboundDataExists { get { return Item.CanRaiseGetNotInListValue; } }
		public RepositoryItemLookUpEdit Item { get { return item; } }
		public override bool AlwaysUsePrimitiveDataSource {
			get {
				ListDataControllerHelper helper = Helper as ListDataControllerHelper;
				if(helper == null) return false;
				Type indexerType = helper.GetIndexerPropertyType();
				if(indexerType == null) return base.AlwaysUsePrimitiveDataSource;
				return indexerType.IsPrimitive; 
			}
		}
	}
}
namespace DevExpress.XtraEditors {
	[DefaultBindingProperty("EditValue"), Designer("DevExpress.XtraEditors.Design.LookUpEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Displays a drop-down list of lookup values."),
	ToolboxTabName(AssemblyInfo.DXTabCommon), SmartTagAction(typeof(LookUpEditActions), "Columns", "Columns", SmartTagActionType.CloseAfterExecute), SmartTagAction(typeof(LookUpEditActions), "PopulateColumns", "Populate Columns"), ToolboxBitmap(typeof(ToolboxIconsRootNS), "LookUpEdit")
	]
	public class LookUpEdit : LookUpEditBase {
		bool isDisplayTextValid;
		public LookUpEdit() {
			this.isDisplayTextValid = true;
		}
		public override void Reset() {
			this.isDisplayTextValid = true;
			base.Reset();	
		}
		protected override void InitializeDefaultProperties() {
			base.InitializeDefaultProperties();
			Properties.UseCtrlScroll = DefaultUseCtrlScroll;
		}
		protected override void AcceptPopupValue(object val) {
			this.isDisplayTextValid = true;
			base.AcceptPopupValue(val);
			ResetPopupFilter();
			UpdateMaskBoxDisplayText();
		}
		void ResetPopupFilter() {
			if(PopupForm != null) PopupForm.Filter.FilterPrefix = string.Empty;
		}
		protected virtual bool NeedUpdateDisplayText(PopupCloseMode closeMode) {
			return closeMode != PopupCloseMode.Immediate;
		}
		protected override void  UpdateEditValueOnClose(PopupCloseMode closeMode, bool acceptValue, object newValue, object oldValue) {
			if(acceptValue && CompareEditValue(newValue, EditValue, true) && !NeedUpdateDisplayText(closeMode))
				return;
 			base.UpdateEditValueOnClose(closeMode, acceptValue, newValue, oldValue);
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			base.OnPopupClosed(closeMode);
			ResetPopupFilter();
		}
		protected override void OnEditValueChanging(ChangingEventArgs e) {
			if(!Properties.IsNullValue(e.NewValue) && Properties.DataAdapter.Columns[Properties.ValueMember] != null) {
				Type fieldType = Properties.DataAdapter.Columns[Properties.ValueMember].Type;
				Type evType = e.NewValue.GetType();
				if(!fieldType.IsAssignableFrom(evType)) {
					if(Properties.ThrowExceptionOnInvalidLookUpEditValueType)
						throw new InvalidLookUpEditValueTypeException(evType, fieldType);
					else e.Cancel = true;
					if(IsEditorActive || IsAcceptingEditValue) IsModified = true;
				}
			}
			LockIsDisplayTextValid();
			try {
				base.OnEditValueChanging(e);
			}
			finally {
				UnlockIsDisplayTextValid();
			}
		}
		protected internal virtual void OnListChanged(ListChangedEventArgs e) {
			if(Properties.UseDropDownRowsAsMaxCount) Properties.CheckDropDownRows();
			ViewInfo.RefreshDisplayText = true;
			LockIsDisplayTextValid();
			try {
				Refresh();
			}
			finally { UnlockIsDisplayTextValid(); }
		}
		protected override bool CanShowPopup { get { return base.CanShowPopup && Properties.DataAdapter.ItemCount > 0; } }
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupLookUpEditForm(this);
		}
		protected internal virtual void PopupFormResultValueEntered() {
			ViewInfo.RefreshDisplayText = true;
			CheckAutoSearchText(true);
		}
		protected internal void ProcessPopupAutoSearchValue() {
			if(PopupForm != null) {
				if(CheckInputNewValue(false)) {
					UpdateEditValueFromMaskBoxText();
					PopupForm.SetResultValue(EditValue);
					this.isDisplayTextValid = true;
				}
				PopupForm.EnterValue();
			}
		}
		protected internal override void ProcessPopupTabKey(KeyEventArgs e) {
			ProcessPopupAutoSearchValue();
			if(IsPopupOpen) return;
			base.ProcessDialogKey(e.KeyData);
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			this.isDisplayTextValid = true;
		}
		protected override void OnMaskBox_ValueChanged(object sender, EventArgs e) { 
			if(MaskBox.IsImeInProgress && !MaskBox.IsImeResult) return;
			ProcessText(new KeyPressHelper(MaskBox.MaskBoxText, MaskBox.MaskBoxSelectionStart, MaskBox.MaskBoxSelectionLength, -1), true, '\0', false);
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			string prevText = IsMaskBoxAvailable ? MaskBox.MaskBoxText : string.Empty;
			base.OnEditorKeyDown(e);
			if(e.Handled) return;
			if(!IsPopupOpen) {
				if(e.KeyCode == Keys.Return) {
					if(IsMaskBoxAvailable) ProcessNewValueCore(false, prevText);
				}
				else if(CheckScrollEditValue(e.KeyData))
					e.Handled = true;
			}
		}
		protected virtual bool CheckScrollEditValue(Keys keyData) {
			int delta = 0;
			if(Properties.UseCtrlScroll) {
				if(keyData == (Keys.Up | Keys.Control)) delta = -1;
				if(keyData == (Keys.Down | Keys.Control)) delta = 1;
			} else {
				if(keyData == Keys.Up) delta = -1;
				if(keyData == Keys.Down) delta = 1;
			}
			ScrollEditValue(delta);
			return (delta != 0);
		}
		protected virtual void ScrollEditValue(int delta) {
			if(Properties.DataAdapter.ItemCount < 1 || Properties.ReadOnly || delta == 0) return;
			SetRowIndex(ItemIndex + delta);
		}
		void SetRowIndex(int value) {
			value = Math.Min(Properties.DataAdapter.ItemCount - 1, Math.Max(value, 0));
			EditValue = Properties.GetDataSourceValue(Properties.ValueMember, value);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int ItemIndex {
			get { return Properties.GetDataSourceRowIndex(Properties.ValueMember, EditValue); }
			set {
				int itemIndex = ItemIndex;
				if(itemIndex == value) return;
				ScrollEditValue(value - ItemIndex);
			}
		}
		protected internal override bool FireSpinRequest(DXMouseEventArgs e, bool isUp) {
			if(e != null && IsPopupOpen) {
				OnMouseWheel(e);
				return true;
			}
			return base.FireSpinRequest(e, isUp);
		}
		protected override bool CanProcessAutoSearchText { 
			get { 
				return (!IsPopupOpen || Properties.SearchMode == SearchMode.AutoFilter) && !Properties.ReadOnly; 
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled || !Properties.AllowMouseWheel) return;
				ee.Handled = true;
				if(IsPopupOpen) {
					PopupForm.TopIndex += (ee.Delta > 0 ? -SystemInformation.MouseWheelScrollLines : SystemInformation.MouseWheelScrollLines);
				} else {
					Keys scrollKey = (ee.Delta > 0 ? Keys.Up : Keys.Down);
					if((Control.ModifierKeys & Keys.Control) != 0)
						scrollKey |= Keys.Control;
					CheckScrollEditValue(scrollKey);
				}
			} finally {
				ee.Sync();
			}
		}
		protected override void ProcessAutoSearchNavKey(KeyEventArgs e) {
			string prev = AutoSearchText;
			base.ProcessAutoSearchNavKey(e);
			if(prev != AutoSearchText && PopupForm != null) PopupForm.Filter.FilterPrefix = GetAutoSearchTextFilter();
		}
		protected override void ParseEditorValue() {
			AutoSearchText = string.Empty;
			isDisplayTextValid = true;
			if(IsMaskBoxAvailable) {
				if(CheckInputNewValue(false)) {  
					Properties.DataAdapter.FilterPrefix = string.Empty;
					UpdateMaskBoxDisplayText();
				}
				UpdateEditValueFromMaskBoxText();
				EditValue = EditValue; 
			}
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			base.OnBindingContextChanged(e);
			Properties.ActivateDataSource(RepositoryItemLookUpEdit.ActivationMode.BindingContext);
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			Properties.ActivateDataSource(RepositoryItemLookUpEdit.ActivationMode.Normal);
		}
		private void UpdateEditValueFromMaskBoxText() {
			if(Properties.GetDisplayText(Properties.ActiveFormat, EditValue) == MaskBox.MaskBoxText) return;
			int index = FindItem(MaskBox.MaskBoxText, false, 0);
			if(index != -1)
				EditValue = Properties.DataAdapter.GetKeyValue(index);
		}
		protected override void DoShowPopup() {
			if(Properties.Columns.Count == 0) Properties.PopulateColumns();
			base.DoShowPopup();
		}
		private bool IsFirstOpeningPopupChar { 
			get { return (!IsPopupOpen && Properties.SearchMode != SearchMode.AutoFilter && IsImmediatePopup); }
		}
		protected override void DoImmediatePopup(int itemIndex, char pressedKey) { 
			if(IsFirstOpeningPopupChar)
				ShowPopup();
		}
		protected override void ProcessFindItem(KeyPressHelper helper, char pressedKey) {
			ProcessText(helper, true, pressedKey);
		}
		protected void ProcessText(KeyPressHelper helper, bool canImmediatePopup, char pressedKey) {
			ProcessText(helper, canImmediatePopup, pressedKey, Properties.SearchMode != SearchMode.OnlyInPopup);
		}
		protected virtual void ProcessText(KeyPressHelper helper, bool canImmediatePopup, char pressedKey, bool partialSearch) {
			bool prevOpen = IsPopupOpen;
			if(IsNeedShowPopup(canImmediatePopup))
				ShowPopup();
			int itemIndex = FindItem(helper.Text, partialSearch, 0);
			isDisplayTextValid = itemIndex != -1;
			int? forcedSelStart = null;
			int? forcedSelLength = null;
			if(IsDisplayTextValid) {
				EditValue = Properties.DataAdapter.GetKeyValue(itemIndex);
				AutoSearchText = helper.Text;
				if(IsMaskBoxAvailable) {
					MaskBoxTextModified();
					forcedSelStart = helper.GetCorrectedAutoSearchSelectionStart(Text, pressedKey);
					forcedSelLength = Text.Length-forcedSelStart;
					SelectionStart = forcedSelStart.Value;
					SelectionLength = forcedSelLength.Value;
				}
			}
			else {
				if(IsMaskBoxAvailable) {
					AutoSearchText = helper.Text;
					MaskBoxTextModified();
					forcedSelStart = helper.SelectionStart;
					forcedSelLength = 0;
					SelectionStart = forcedSelStart.Value;
					SelectionLength = forcedSelLength.Value;
				}
				else {
					if(helper.Text.Length > 1) {
						AutoSearchText = helper.Text.Substring(0, helper.Text.Length - 1);
					}
					else 
						AutoSearchText = string.Empty;
					isDisplayTextValid = true;
				}
			}
			if(IsNeedShowPopup(canImmediatePopup)) {
				if(PopupForm != null) {
					if(Properties.SearchMode == SearchMode.AutoFilter) {
					if(IsDisplayTextValid) {
						PopupForm.Filter.FilterPrefix = GetAutoSearchTextFilter();
						PopupForm.SetResultValue(EditValue);
					}
					else
						PopupForm.Filter.FilterPrefix = (IsMaskBoxAvailable ? MaskBox.MaskBoxText : GetAutoSearchTextFilter());
					}
					else {
						AutoSearchText = string.Empty;
					}
					if(AutoSearchText == string.Empty) PopupForm.SelectedIndex = Properties.DataAdapter.FindValueIndex(Properties.ValueMember, EditValue);
					if(!prevOpen && Properties.SearchMode == SearchMode.OnlyInPopup) {
						PopupForm.ProcessKeyPress(new KeyPressEventArgs(helper.CharValue));
					}
				}
			}
			LayoutChanged();
			if(Text == helper.Text) {
				if(forcedSelStart.HasValue)
					SelectionStart = forcedSelStart.Value;
				if(forcedSelLength.HasValue)
					SelectionLength = forcedSelLength.Value;
			}
		}
		private bool IsNeedShowPopup(bool canImmediatePopup) {
			if(!IsEditorActive)
				return false;
			return (canImmediatePopup && Properties.ImmediatePopup) || Properties.SearchMode == SearchMode.AutoFilter;
		}
		protected virtual int FindItem(string text, bool partialSearch, int startIndex) {
			if(text == null) return -1;
			if(text.Length == 0 && partialSearch) return -1;
			if(!Properties.CaseSensitiveSearch) text = text.ToLower();
			if(startIndex < 0) startIndex = 0;
			for(int i = startIndex; i < Properties.DataAdapter.ItemCount; i++) {
				string itemText = Properties.GetValueDisplayText(Properties.ActiveFormat, Properties.DataAdapter.GetValueAtIndex(Properties.DisplayMember, i));
				if(!Properties.CaseSensitiveSearch) itemText = itemText.ToLower();
				if(partialSearch) {
					if(text == itemText.Substring(0, Math.Min(itemText.Length, text.Length))) return i;
				} else {
					if(text == itemText) return i;
				}
			}
			return -1;
		}
		private void MaskBoxTextModified() {
			UpdateMaskBoxDisplayText();
			IsModified = true;
		}
		protected internal new PopupLookUpEditForm PopupForm { get { return base.PopupForm as PopupLookUpEditForm; } }
		protected override bool IsNeedHideCursorOnPopup { get { return (Properties.SearchMode != SearchMode.AutoFilter); } }
		protected internal virtual bool DefaultUseCtrlScroll { get { return false; } }
		public virtual object GetColumnValue(LookUpColumnInfo column) {
			if(column == null) return null;
			return GetColumnValue(column.FieldName);
		}
		public virtual object GetColumnValue(string fieldName) {
			if(fieldName == Properties.ValueMember) return EditValue;
			return Properties.GetDataSourceValue(fieldName, Properties.GetDataSourceRowIndex(Properties.ValueMember, EditValue));
		}
		int lockIsDisplayTextValidCounter = 0;
		internal void LockIsDisplayTextValid() { this.lockIsDisplayTextValidCounter++; }
		internal void UnlockIsDisplayTextValid(){ this.lockIsDisplayTextValidCounter--; }
		[Browsable(false)]
		public bool IsDisplayTextValid { 
			get {
				if(lockIsDisplayTextValidCounter != 0) return true;
				return isDisplayTextValid; 
			} 
		}
		protected internal override void SetEmptyEditValue(object emptyEditValue) {
			this.isDisplayTextValid = true;
			base.SetEmptyEditValue(emptyEditValue);
		}
		[Browsable(false)]
		public override string EditorTypeName { get { return "LookUpEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemLookUpEdit Properties { get { return base.Properties as RepositoryItemLookUpEdit; } }
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { 
			get { return base.Text; }
			set {
				if(Properties.ReadOnly) return;
				if(value == null) value = string.Empty;
				ProcessText(new KeyPressHelper(value, value.Length, 0, Properties.MaxLength), true, '\0');
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpEditGetNotInListValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event GetNotInListValueEventHandler GetNotInListValue {
			add { this.Properties.GetNotInListValue += value; }
			remove { this.Properties.GetNotInListValue -= value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("LookUpEditListChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event ListChangedEventHandler ListChanged {
			add { Properties.ListChanged += value; }
			remove { Properties.ListChanged -= value; }
		}
		protected override bool CheckInputNewValue(bool partial) {
			if(!IsMaskBoxAvailable) return false;
			return ProcessNewValueCore(partial, MaskBox.MaskBoxText);
		}
		protected virtual bool ProcessNewValueCore(bool partital, string text) {
			if(FindItem(text, partital, 0) != -1) return false;
			object dispVal = text;
			DataColumnInfo info = Properties.DataAdapter.Columns[Properties.DisplayMember];
			if(info != null) {
				try { dispVal = Convert.ChangeType(dispVal, info.Type); }
				catch { }
			}
			ProcessNewValueEventArgs e = new ProcessNewValueEventArgs(dispVal);
			Properties.RaiseProcessNewValue(e);
			if(e.Handled) {
				Properties.DataAdapter.FilterPrefix = string.Empty;
				object newEditValue = Properties.GetKeyValueByDisplayValue(e.DisplayValue);
				Properties.RemoveDisplayValue(newEditValue);
				EditValue = newEditValue;
			}
			return e.Handled;
		}
		public override object GetSelectedDataRow() {
			return Properties.GetDataSourceRowByKeyValue(EditValue);
		}
		protected internal override void Refresh(bool resetCache) {
			if(resetCache) Properties.ResetDisplayValues();
			base.Refresh(resetCache);
		}
	}
	public enum PopupFilterMode { Default, Contains, StartsWith }
}
