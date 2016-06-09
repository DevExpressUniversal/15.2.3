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

using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Design;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.XtraGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public enum GridViewColumnFixedStyle { None, Left };
	public enum ColumnCommandButtonType { Edit, New, Delete, Select, Update, Cancel, ClearFilter, SelectCheckbox, 
		ApplyFilter, ApplySearchPanelFilter, ClearSearchPanelFilter, ShowAdaptiveDetail, HideAdaptiveDetail }
	public enum GridViewCustomButtonVisibility { FilterRow, AllDataRows, BrowsableRow, EditableRow, Invisible }
	public enum GridViewSelectAllCheckBoxMode { None, Page, AllPages };
	public enum ASPxColumnCaptionLocation { Default, Near, Top, None };
	public enum AutoFilterCondition { Default, BeginsWith, EndsWith, Contains, DoesNotContain, Equals, Less, LessOrEqual, Greater, GreaterOrEqual, NotEqual, Like }
	public enum ColumnFilterMode { Value, DisplayText }
	public abstract class GridViewColumn : WebColumnBase, IWebGridColumn, IDesignTimePropertiesOwner {
		GridViewHeaderStyle headerStyle;
		GridViewCellStyle cellStyle;
		GridViewFooterStyle footerCellStyle;
		GridViewGroupFooterStyle groupFooterCellStyle;
		GridViewExportAppearance exportCellStyle;
		ITemplate headerTemplate;
		ITemplate headerCaptionTemplate;
		ITemplate footerTemplate;
		ITemplate groupFooterTemplate;
		ITemplate filterTemplate;
		public GridViewColumn() 
			: base() {			
			this.headerStyle = new GridViewHeaderStyle();
			this.cellStyle = new GridViewCellStyle();
			this.footerCellStyle = new GridViewFooterStyle();
			this.groupFooterCellStyle = new GridViewGroupFooterStyle();
			this.exportCellStyle = new GridViewExportAppearance();
		}
		[Browsable(false)]
		public virtual ASPxGridView Grid {
			get {
				GridViewColumnCollection gridColl = Collection as GridViewColumnCollection;
				if(gridColl == null) return null;
				return gridColl.Grid;
			}
		}
		protected GridViewColumnHelper ColumnHelper {
			get { return Grid != null ? Grid.ColumnHelper : null; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnHeaderStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual GridViewHeaderStyle HeaderStyle { get { return headerStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual GridViewCellStyle CellStyle { get { return cellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnFooterCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual GridViewFooterStyle FooterCellStyle { get { return footerCellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnGroupFooterCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual GridViewGroupFooterStyle GroupFooterCellStyle { get { return groupFooterCellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnExportCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual GridViewExportAppearance ExportCellStyle { get { return exportCellStyle; } }
		[Category("Templates"), Browsable(false),
		DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		TemplateContainer(typeof(GridViewHeaderTemplateContainer))]
		public virtual ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {
				if(HeaderTemplate == value) return;
				headerTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false),
		DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(GridViewHeaderTemplateContainer))]
		public virtual ITemplate HeaderCaptionTemplate {
			get { return headerCaptionTemplate; }
			set {
				if(HeaderCaptionTemplate == value) return;
				headerCaptionTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false),
		DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(GridViewFilterCellTemplateContainer))]
		public virtual ITemplate FilterTemplate {
			get { return filterTemplate; }
			set {
				if(filterTemplate == value)
					return;
				filterTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false),
		DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(GridViewFooterCellTemplateContainer))]
		public virtual ITemplate FooterTemplate {
			get { return footerTemplate; }
			set {
				if(FooterTemplate == value) return;
				footerTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false),
		DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(GridViewGroupFooterCellTemplateContainer))]
		public virtual ITemplate GroupFooterTemplate {
			get { return groupFooterTemplate; }
			set {
				if(GroupFooterTemplate == value) return;
				groupFooterTemplate = value;
				TemplatesChanged();
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewColumnShowInCustomizationForm")]
#endif
		public new bool ShowInCustomizationForm {
			get { return base.ShowInCustomizationForm; }
			set { base.ShowInCustomizationForm = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnFixedStyle"),
#endif
 Category("Behavior"), DefaultValue(GridViewColumnFixedStyle.None), NotifyParentProperty(true)]
		public GridViewColumnFixedStyle FixedStyle {
			get { return (GridViewColumnFixedStyle)GetIntProperty("FixedStyle", (int)GridViewColumnFixedStyle.None); }
			set {
				if(value == FixedStyle) return;
				SetIntProperty("FixedStyle", (int)GridViewColumnFixedStyle.None, (int)value);
				if(Grid != null)
					(Grid as IWebColumnsOwner).OnColumnCollectionChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnExportWidth"),
#endif
		Category("Appearance"), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int ExportWidth {
			get { return GetIntProperty("ExportWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ExportWidth");
				SetIntProperty("ExportWidth", 0, value); 
			}
		}
		[
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int AdaptivePriority {
			get {
				return GetIntProperty("AdaptivePriority", 0);
			}
			set {
				CommonUtils.CheckNegativeValue((double)value, "AdaptivePriority");
				SetIntProperty("AdaptivePriority", 0, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewColumnMinWidth"),
#endif
		Category("Appearance"), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int MinWidth {
			get { return GetIntProperty("MinWidth", 0); }
			set { SetIntProperty("MinWidth", 0, Math.Max(0, value)); }
		}
		public override bool IsClickable() { return GetAllowDragDrop() || GetAllowSort(); }
		protected internal virtual int GetColumnMinWidth() {
			if(Grid == null) return MinWidth;
			return MinWidth > 0 ? MinWidth : Grid.Settings.ColumnMinWidth;
		}
		protected internal virtual ColumnSortOrder GetSortOrder() { return ColumnSortOrder.None; }		
		protected internal virtual bool GetAllowAutoFilter() { return false; }
		protected internal virtual bool GetAllowDragDrop() { return true; }
		protected internal virtual bool GetAllowSort() { return false; }
		protected internal virtual bool GetAllowGroup() { return GetAllowSort(); }
		protected internal virtual bool GetHasFilterButton() { return false; }
		protected internal virtual bool GetIsFiltered() { return false; }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			GridViewColumn col = (GridViewColumn)source;
			if(col == null) return;
			this.FixedStyle = col.FixedStyle;
			this.HeaderStyle.Assign(col.HeaderStyle);
			this.CellStyle.Assign(col.CellStyle);
			this.FooterCellStyle.Assign(col.FooterCellStyle);
			this.GroupFooterCellStyle.Assign(col.GroupFooterCellStyle);
			this.ExportCellStyle.Assign(col.ExportCellStyle);
			this.HeaderCaptionTemplate = col.HeaderCaptionTemplate;
			this.HeaderTemplate = col.HeaderTemplate;
			this.FilterTemplate = col.FilterTemplate;
			this.FooterTemplate = col.FooterTemplate;
			this.GroupFooterTemplate = col.GroupFooterTemplate;
			this.ExportWidth = col.ExportWidth;
			this.AdaptivePriority = col.AdaptivePriority;
			this.MinWidth = col.MinWidth;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { HeaderStyle, CellStyle, FooterCellStyle, GroupFooterCellStyle, ExportCellStyle };
		}
		protected internal virtual bool IsEquals(string id_field_caption) {
			return id_field_caption == Name;
		}
		[Browsable(false)]
		public GridViewBandColumn ParentBand {
			get {
				if(Collection != null)
					return Collection.Owner as GridViewBandColumn;
				return null;
			}
		}
		protected override string GetDesignTimeCaption() {
			return ToString();
		}
		protected override int GetDesignTimeVisibleIndex() {
			return VisibleIndex;
		}
		protected override void SetDesignTimeVisibleIndex(int index) {
			VisibleIndex = index;
		}
		protected override bool GetDesignTimeVisible() {
			return Visible;
		}
		protected override void SetDesignTimeVisible(bool visible) {
			Visible = visible;
		}
		object IDesignTimePropertiesOwner.Owner { get { return Grid; } }
		string IWebGridColumn.Name { get { return Name; } set { Name = value; } }
		string IWebGridColumn.Caption { get { return Caption; } set { Caption = value; } }
		int IWebGridColumn.VisibleIndex { get { return VisibleIndex; } set { VisibleIndex = value; } }
		bool IWebGridColumn.Visible { get { return Visible; } set { Visible = value; } }
		GridExportAppearanceBase IWebGridColumn.ExportCellStyle { get { return ExportCellStyle; } }
	}
	public class GridViewCommandColumnCustomButton : GridCustomCommandButton {
		public GridViewCommandColumnCustomButton()
			: base() {
		}
		protected GridViewCommandColumnCustomButtonCollection ButtonCollection { get { return Collection as GridViewCommandColumnCustomButtonCollection; } }
		protected internal GridViewCommandColumn Column { get { return ButtonCollection != null ? ButtonCollection.Column : null; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnCustomButtonVisibility"),
#endif
 Category("Appearance"), DefaultValue(GridViewCustomButtonVisibility.BrowsableRow), NotifyParentProperty(true)]
		public GridViewCustomButtonVisibility Visibility {
			get { return (GridViewCustomButtonVisibility)GetEnumProperty("Visibility", GridViewCustomButtonVisibility.BrowsableRow); }
			set {
				if(value == Visibility) return;
				SetEnumProperty("Visibility", GridViewCustomButtonVisibility.BrowsableRow, value);
				LayoutChanged();
			}
		}
		protected internal bool IsVisible(GridViewTableCommandCellType cellType, bool isEditingRow) {
			if(Visibility == GridViewCustomButtonVisibility.Invisible) return false;
			if(Visibility == GridViewCustomButtonVisibility.FilterRow) return cellType == GridViewTableCommandCellType.Filter;
			if(cellType == GridViewTableCommandCellType.Filter) return false;
			if(Visibility == GridViewCustomButtonVisibility.AllDataRows) return cellType == GridViewTableCommandCellType.Data;
			if(isEditingRow) return Visibility == GridViewCustomButtonVisibility.EditableRow;
			return Visibility == GridViewCustomButtonVisibility.BrowsableRow;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var button = source as GridViewCommandColumnCustomButton;
			if(button == null) return;
			Visibility = button.Visibility;
		}
	}
	public class GridViewCommandColumnCustomButtonCollection : Collection<GridViewCommandColumnCustomButton> {
		public GridViewCommandColumnCustomButtonCollection(GridViewCommandColumn column) {
			Column = column;
		}
#if !SL
	[DevExpressWebLocalizedDescription("GridViewCommandColumnCustomButtonCollectionColumn")]
#endif
		public GridViewCommandColumn Column { get; private set; }
		public override string ToString() { return string.Empty; }
		[Browsable(false)]
		public GridViewCommandColumnCustomButton this[string IDorText] {
			get {
				foreach(GridViewCommandColumnCustomButton button in this) {
					if(button.ID == IDorText) return button;
				}
				foreach(GridViewCommandColumnCustomButton button in this) {
					if(button.Text == IDorText) return button;
				}
				return null;
			}
		}
		protected override void OnChanged() {
			base.OnChanged();
			if(Column != null) {
				Column.OnColumnChanged();
			}
		}
	}
	public class GridViewCommandColumn : GridViewColumn {
#pragma warning disable 612
		GridViewCommandColumnButton editButton, newButton, deleteButton, selectButton, clearFilterButton, cancelButton, updateButton;
#pragma warning restore 612
		public GridViewCommandColumn() : this(string.Empty) { }
		public GridViewCommandColumn(string caption) {
			Caption = caption;
			CustomButtons = new GridViewCommandColumnCustomButtonCollection(this);
#pragma warning disable 612
			this.editButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.Edit);
			this.newButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.New);
			this.deleteButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.Delete);
			this.selectButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.Select);
			this.cancelButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.Cancel);
			this.updateButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.Update);
			this.clearFilterButton = new GridViewCommandColumnButton(this, ColumnCommandButtonType.ClearFilter);
#pragma warning restore 612
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnCustomButtons"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string)null), AutoFormatDisable, NotifyParentProperty(true), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public GridViewCommandColumnCustomButtonCollection CustomButtons { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowNewButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowNewButton {
			get { return GetBoolProperty("ShowNewButton", false); }
			set {
				if(ShowNewButton == value) return;
				SetBoolProperty("ShowNewButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowEditButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowEditButton {
			get { return GetBoolProperty("ShowEditButton", false); }
			set {
				if(ShowEditButton == value) return;
				SetBoolProperty("ShowEditButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowUpdateButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowUpdateButton {
			get { return GetBoolProperty("ShowUpdateButton", false); }
			set {
				if(ShowUpdateButton == value) return;
				SetBoolProperty("ShowUpdateButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowCancelButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowCancelButton {
			get { return GetBoolProperty("ShowCancelButton", false); }
			set {
				if(ShowCancelButton == value) return;
				SetBoolProperty("ShowCancelButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowDeleteButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowDeleteButton {
			get { return GetBoolProperty("ShowDeleteButton", false); }
			set {
				if(ShowDeleteButton == value) return;
				SetBoolProperty("ShowDeleteButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowSelectButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowSelectButton {
			get { return GetBoolProperty("ShowSelectButton", false); }
			set {
				if(ShowSelectButton == value) return;
				SetBoolProperty("ShowSelectButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowApplyFilterButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowApplyFilterButton {
			get { return GetBoolProperty("ShowApplyFilterButton", false); }
			set {
				if(ShowApplyFilterButton == value) return;
				SetBoolProperty("ShowApplyFilterButton", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowClearFilterButton"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowClearFilterButton {
			get { return GetBoolProperty("ShowClearFilterButton", false); }
			set {
				if(ShowClearFilterButton == value) return;
				SetBoolProperty("ShowClearFilterButton", false, value);
				OnColumnChanged();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { CustomButtons });
		}
		protected internal override bool GetAllowDragDrop() {
			if(AllowDragDrop == DefaultBoolean.Default && Grid != null)
				return Grid.SettingsBehavior.AllowDragDrop;
			return AllowDragDrop != DefaultBoolean.False;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnButtonType"),
#endif
 Category("Buttons"), DefaultValue(GridCommandButtonRenderMode.Default), NotifyParentProperty(true)]
		public GridCommandButtonRenderMode ButtonType {
			get { return (GridCommandButtonRenderMode)GetEnumProperty("ButtonType", GridCommandButtonRenderMode.Default); }
			set {
				if(value == ButtonType) return;
				SetEnumProperty("ButtonType", GridCommandButtonRenderMode.Default, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowNewButtonInHeader"),
#endif
 Category("Buttons"), NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable]
		public bool ShowNewButtonInHeader {
			get { return GetBoolProperty("ShowNewButtonInHeader", false); }
			set {
				if(value == ShowNewButtonInHeader) return;
				SetBoolProperty("ShowNewButtonInHeader", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnShowSelectCheckbox"),
#endif
 Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowSelectCheckbox {
			get { return GetBoolProperty("ShowSelectCheckbox", false); }
			set {
				if(ShowSelectCheckbox == value) return;
				SetBoolProperty("ShowSelectCheckbox", false, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnSelectAllCheckboxMode"),
#endif
 Category("Buttons"), DefaultValue(GridViewSelectAllCheckBoxMode.None), NotifyParentProperty(true)]
		public GridViewSelectAllCheckBoxMode SelectAllCheckboxMode {
			get { return (GridViewSelectAllCheckBoxMode)GetEnumProperty("SelectAllCheckboxMode", GridViewSelectAllCheckBoxMode.None); }
			set {
				if(SelectAllCheckboxMode == value) return;
				SetEnumProperty("SelectAllCheckboxMode", GridViewSelectAllCheckBoxMode.None, value);
				OnShowAllButtonModeChanged();
			}
		}
		protected void OnShowAllButtonModeChanged() {
			OnColumnChanged();
			if(Grid != null && SelectAllCheckboxMode != GridViewSelectAllCheckBoxMode.None)
				Grid.DataProxy.RequireDataBound();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnAllowDragDrop"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowDragDrop {
			get { return (DefaultBoolean)GetIntProperty("AllowDragDrop", (int)DefaultBoolean.Default); }
			set {
				if(AllowDragDrop == value) return;
				SetIntProperty("AllowDragDrop", (int)DefaultBoolean.Default, (int)value);
				OnColumnChanged();
			}
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption)) return Caption;
			return "#";
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			GridViewCommandColumn src = source as GridViewCommandColumn;
			if(src == null) return;
			ShowNewButtonInHeader = src.ShowNewButtonInHeader;
			ShowSelectCheckbox = src.ShowSelectCheckbox;
			ButtonType = src.ButtonType;
			AllowDragDrop = src.AllowDragDrop;
			ShowNewButton = src.ShowNewButton;
			ShowEditButton = src.ShowEditButton;
			ShowUpdateButton = src.ShowUpdateButton;
			ShowCancelButton = src.ShowCancelButton;
			ShowDeleteButton = src.ShowDeleteButton;
			ShowSelectButton = src.ShowSelectButton;
			ShowApplyFilterButton = src.ShowApplyFilterButton;
			ShowClearFilterButton = src.ShowClearFilterButton;
			CustomButtons.Assign(src.CustomButtons);
			SelectAllCheckboxMode = src.SelectAllCheckboxMode;
		}
		protected internal bool CanShowCommandButton(GridCommandButtonType buttonType) {
			switch(buttonType) {
				case GridCommandButtonType.New:
					return ShowNewButton;
				case GridCommandButtonType.Edit:
					return ShowEditButton;
				case GridCommandButtonType.Update:
					return ShowUpdateButton;
				case GridCommandButtonType.Cancel:
					return ShowCancelButton;
				case GridCommandButtonType.Delete:
					return ShowDeleteButton;
				case GridCommandButtonType.Select:
					return ShowSelectButton;
				case GridCommandButtonType.ApplyFilter:
					return ShowApplyFilterButton;
				case GridCommandButtonType.ClearFilter:
					return ShowClearFilterButton;
			}
			return false;
		}
		[Obsolete("Use the ShowEditButton and SettingsCommandButton.EditButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnEditButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton EditButton { get { return editButton; } }
		[Obsolete("Use the ShowNewButton and SettingsCommandButton.NewButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnNewButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton NewButton { get { return newButton; } }
		[Obsolete("Use the ShowDeleteButton and SettingsCommandButton.DeleteButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnDeleteButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton DeleteButton { get { return deleteButton; } }
		[Obsolete("Use the ShowSelectButton and SettingsCommandButton.SelectButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnSelectButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton SelectButton { get { return selectButton; } }
		[Obsolete("Use the ShowCancelButton and SettingsCommandButton.CancelButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnCancelButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton CancelButton { get { return cancelButton; } }
		[Obsolete("Use the ShowUpdateButton and SettingsCommandButton.UpdateButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnUpdateButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton UpdateButton { get { return updateButton; } }
		[Obsolete("Use the ShowClearFilterButton and SettingsCommandButton.ClearFilterButton properties instead. You can update your project automatically by using a tool provided in the http://www.devexpress.com/kbid=T246446 article.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnClearFilterButton"),
#endif
 Category("Buttons"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All), TypeConverter(typeof(ExpandableObjectConverter)), NotifyParentProperty(true)]
		public GridViewCommandColumnButton ClearFilterButton { get { return clearFilterButton; } }
	}
	[Obsolete]
	public class GridViewCommandColumnButton : StateManager {
		public GridViewCommandColumnButton(GridViewCommandColumn column, ColumnCommandButtonType buttonType) { }
		[Browsable(false)]
		public ColumnCommandButtonType ButtonType { get { return ColumnCommandButtonType.New; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnButtonVisible"),
#endif
 Category("Appearance"), DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool Visible { get { return false; } set { } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnButtonText"),
#endif
 Category("Appearance"), DefaultValue(""), NotifyParentProperty(true)]
		public string Text { get { return string.Empty; } set { } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewCommandColumnButtonImage"),
#endif
 Category("Appearance"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageProperties Image { get { return null; } }
	}
	public class GridViewDataColumnBuilder : ControlBuilder, IEditorPropertiesContainer {
		string editPropertiesType = string.Empty;
		public string EditPropertiesType { get { return editPropertiesType; } }
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			this.editPropertiesType = (string)attribs["PropertiesEditType"];
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
		#region IEditorPropertiesContainer Members
		Type IEditorPropertiesContainer.GetEditorType() {
			return EditRegistrationInfo.GetEditType(EditPropertiesType);
		}
		#endregion
	}
	[ControlBuilder(typeof(GridViewDataColumnBuilder))]
	public class GridViewDataColumn : GridViewColumn, IWebColumnInfo, IDataSourceViewSchemaAccessor, IFilterColumn, IWebGridDataColumn, IWebGridDataColumnAdapterOwner {
		GridViewEditCellStyle editCellStyle;
		GridViewFilterCellStyle filterCellStyle;
		GridColumnEditFormSettings editFormSettings;
		GridViewEditFormCaptionStyle editFormCaptionStyle;
		ITemplate dataItemTemplate, editItemTemplate, groupRowTemplate;
		public GridViewDataColumn() : this(string.Empty, string.Empty) { }
		public GridViewDataColumn(string fieldName) : this(fieldName, string.Empty) { }
		public GridViewDataColumn(string fieldName, string caption) {
			ColumnAdapter = CreateColumnAdapter();
			FieldName = fieldName;
			Caption = caption;
			this.editCellStyle = new GridViewEditCellStyle();
			this.filterCellStyle = new GridViewFilterCellStyle();
			this.editFormCaptionStyle = new GridViewEditFormCaptionStyle();
			this.editFormSettings = new GridColumnEditFormSettings(this);
		}
		protected internal GridDataColumnAdapter ColumnAdapter { get; private set; }
		[Category("Behavior"), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual EditPropertiesBase PropertiesEdit { get { return ColumnAdapter.PropertiesEdit; } set { ColumnAdapter.PropertiesEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettings"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridViewDataColumnSettings Settings { get { return (GridViewDataColumnSettings)ColumnAdapter.Settings; } }
		[ Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridViewDataColumnHeaderFilterSettings SettingsHeaderFilter { get { return (GridViewDataColumnHeaderFilterSettings)ColumnAdapter.SettingsHeaderFilter; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnEditFormSettings"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridColumnEditFormSettings EditFormSettings { get { return editFormSettings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnEditCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridViewEditCellStyle EditCellStyle { get { return editCellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnFilterCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridViewFilterCellStyle FilterCellStyle { get { return filterCellStyle; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnEditFormCaptionStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public GridViewEditFormCaptionStyle EditFormCaptionStyle { get { return editFormCaptionStyle; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public string FilterExpression { get { return ColumnAdapter.FilterExpression; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnFieldName"),
#endif
 Category("Data"), DefaultValue(""), Localizable(false), RefreshProperties(RefreshProperties.Repaint), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), NotifyParentProperty(true)]
		public virtual string FieldName { get { return ColumnAdapter.FieldName; } set { ColumnAdapter.FieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnUnboundType"),
#endif
 Category("Data"), DefaultValue(UnboundColumnType.Bound), NotifyParentProperty(true)]
		public virtual UnboundColumnType UnboundType { get { return ColumnAdapter.UnboundType; } set { ColumnAdapter.UnboundType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnUnboundExpression"),
#endif
 Category("Data"), DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.GridViewUnboundExpressionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string UnboundExpression { get { return ColumnAdapter.UnboundExpression; } set { ColumnAdapter.UnboundExpression = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnGroupIndex"),
#endif
 Category("Data"), DefaultValue(-1), NotifyParentProperty(true)]
		public int GroupIndex { get { return ColumnAdapter.GroupIndex; } set { ColumnAdapter.GroupIndex = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSortIndex"),
#endif
 Category("Data"), DefaultValue(-1), NotifyParentProperty(true)]
		public int SortIndex { get { return ColumnAdapter.SortIndex; } set { ColumnAdapter.SortIndex = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSortOrder"),
#endif
 Category("Data"), DefaultValue(ColumnSortOrder.None), NotifyParentProperty(true)]
		public virtual ColumnSortOrder SortOrder { get { return ColumnAdapter.SortOrder; } set { ColumnAdapter.SortOrder = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnReadOnly"),
#endif
 Category("Behavior"), DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool ReadOnly { get { return ColumnAdapter.ReadOnly; } set { ColumnAdapter.ReadOnly = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowInFilterControl { get { return ColumnAdapter.ShowInFilterControl; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string EditFormCaption { get { return !string.IsNullOrEmpty(EditFormSettings.Caption) ? EditFormSettings.Caption : ToString(); } }
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(GridViewDataItemTemplateContainer)), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataItemTemplate {
			get { return dataItemTemplate; }
			set {
				if(DataItemTemplate == value) return;
				dataItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(GridViewEditItemTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EditItemTemplate {
			get { return editItemTemplate; }
			set {
				if(EditItemTemplate == value) return;
				editItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(GridViewGroupRowTemplateContainer)), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate GroupRowTemplate {
			get { return groupRowTemplate; }
			set {
				if(GroupRowTemplate == value) return;
				groupRowTemplate = value;
				TemplatesChanged();
			}
		}
		public void AutoFilterBy(string value) { ColumnAdapter.AutoFilterBy(value); }
		public void SortAscending() { ColumnAdapter.SortAscending(); }
		public void SortDescending() { ColumnAdapter.SortDescending(); }
		public void UnSort() { ColumnAdapter.UnSort(); }
		public void GroupBy() { ColumnAdapter.GroupBy(); }
		public void UnGroup() { ColumnAdapter.UnGroup(); }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var src = source as GridViewDataColumn;
			if(src == null) return;
			ColumnAdapter.Assign(src.ColumnAdapter);
			EditCellStyle.Assign(src.EditCellStyle);
			FilterCellStyle.Assign(src.FilterCellStyle);
			EditFormCaptionStyle.Assign(src.EditFormCaptionStyle);
			EditFormSettings.Assign(src.EditFormSettings);
			DataItemTemplate = src.DataItemTemplate;
			EditItemTemplate = src.EditItemTemplate;
			GroupRowTemplate = src.GroupRowTemplate;
		}
		protected virtual GridDataColumnAdapter CreateColumnAdapter() {
			return new GridDataColumnAdapter(this);
		}
		protected virtual EditPropertiesBase CreateEditProperties() {
			return IsDesignMode() ? new TextBoxProperties() : null;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.MergedBaseAndNewStateManagedObjects(base.GetStateManagedObjects(), EditCellStyle, FilterCellStyle, EditFormCaptionStyle, EditFormSettings, ColumnAdapter);
		}
		protected override string GetDesignTimeFieldName() { return FieldName; }
		protected override PropertiesBase GetDesignTimeItemEditProperties() { return PropertiesEdit; }
		protected internal virtual Type GetDataType() { return ColumnAdapter.DataType; }
		protected internal ColumnFilterMode GetFilterMode() { return ColumnAdapter.FilterMode; }
		protected internal EditPropertiesBase ExportPropertiesEdit { get { return ColumnAdapter.ExportPropertiesEdit; } }
		protected internal bool IsMultiSelectHeaderFilter { get { return ColumnAdapter.IsMultiSelectHeaderFilter; } }
		public override string ToString() { return ColumnAdapter.GetDisplayName(); }
		public override bool IsClickable() { return base.IsClickable() || GroupIndex > -1; }
		protected internal override ColumnSortOrder GetSortOrder() { return SortOrder; }
		protected internal override bool GetAllowDragDrop() { return ColumnAdapter.AllowDragDrop; }
		protected internal override bool GetAllowEllipsisInText() {
			if(Settings.AllowEllipsisInText == DefaultBoolean.Default && Grid != null)
				return Grid.SettingsBehavior.AllowEllipsisInText;
			return Settings.AllowEllipsisInText == DefaultBoolean.True;
		}
		protected internal override bool GetAllowAutoFilter() { return ColumnAdapter.AllowAutoFilter; }
		protected internal override bool GetHasFilterButton() { return ColumnAdapter.HasFilterButton; }
		protected internal override bool GetIsFiltered() { return ColumnAdapter.IsFiltered; }
		protected internal override bool GetAllowSort() { return ColumnAdapter.AllowSort; }
		protected internal override bool GetAllowGroup() { return ColumnAdapter.AllowGroup; }
		protected internal override bool IsEquals(string id_field_caption) {
			if(id_field_caption == Name) return true;
			if(id_field_caption == FieldName) return true;
			return id_field_caption == Caption;
		}
		#region IFilterColumn Members
		string IFilterablePropertyInfo.PropertyName { get { return (ColumnAdapter as IFilterColumn).PropertyName; } }
		string IFilterablePropertyInfo.DisplayName { get { return (ColumnAdapter as IFilterColumn).DisplayName; } }
		Type IFilterablePropertyInfo.PropertyType { get { return (ColumnAdapter as IFilterColumn).PropertyType; } }
		int IFilterColumn.Index { get { return (ColumnAdapter as IFilterColumn).Index; } }
		FilterColumnClauseClass IFilterColumn.ClauseClass { get { return (ColumnAdapter as IFilterColumn).ClauseClass; } }
		EditPropertiesBase IFilterColumn.PropertiesEdit { get { return (ColumnAdapter as IFilterColumn).PropertiesEdit; } }
		#endregion
		#region IWebColumnInfo Members
		ColumnSortOrder IWebColumnInfo.SortOrder { get { return SortOrder; } }
		string IWebColumnInfo.FieldName { get { return FieldName; } }
		UnboundColumnType IWebColumnInfo.UnboundType { get { return UnboundType; } }
		string IWebColumnInfo.UnboundExpression { get { return UnboundExpression; } }
		bool IWebColumnInfo.ReadOnly { get { return ReadOnly; } }
		ColumnGroupInterval IWebColumnInfo.GroupInterval { get { return Settings.GroupInterval; } }
		EditPropertiesBase IWebColumnInfo.CreateEditProperties() { return CreateEditProperties(); }
		#endregion
		#region IDataSourceViewSchemaAccessor Members
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema { get { return Grid is IDataSourceViewSchemaAccessor ? (Grid as IDataSourceViewSchemaAccessor).DataSourceViewSchema : null; } set { } }
		#endregion
		#region IWebGridDataColumn Members
		string IWebGridDataColumn.FieldName { get { return FieldName; } set { FieldName = value; } }
		UnboundColumnType IWebGridDataColumn.UnboundType { get { return UnboundType; } set { UnboundType = value; } }
		string IWebGridDataColumn.UnboundExpression { get { return UnboundExpression; } set { UnboundExpression = value; } }
		int IWebGridDataColumn.SortIndex { get { return SortIndex; } set { SortIndex = value; } }
		ColumnSortOrder IWebGridDataColumn.SortOrder { get { return SortOrder; } set { SortOrder = value; } }
		bool IWebGridDataColumn.ReadOnly { get { return ReadOnly; } set { ReadOnly = value; } }
		EditPropertiesBase IWebGridDataColumn.PropertiesEdit { get { return PropertiesEdit; } set { PropertiesEdit = value; } }
		GridDataColumnAdapter IWebGridDataColumn.Adapter { get { return ColumnAdapter; } }
		#endregion
		#region IWebGridDataColumnAdapterOwner Members
		WebColumnBase IWebGridDataColumnAdapterOwner.Column { get { return this; } }
		bool IWebGridDataColumnAdapterOwner.HasGrouping { get { return true; } }
		bool IWebGridDataColumnAdapterOwner.HasAutoFilter { get { return true; } }
		Func<GridDataColumnAdapter, GridDataColumnSettings> IWebGridDataColumnAdapterOwner.CreateSettings { get { return a => new GridViewDataColumnSettings(a); } }
		Func<GridDataColumnAdapter, GridDataColumnHeaderFilterSettings> IWebGridDataColumnAdapterOwner.CreateSettingsHeaderFilter { get { return a => new GridViewDataColumnHeaderFilterSettings(a); } }
		#endregion
	}
	public class GridViewDataColumnSettings : GridDataColumnSettings {
		protected internal GridViewDataColumnSettings(GridDataColumnAdapter columnAdapter) 
			: base(columnAdapter) { 
		}
		[Obsolete]
		public GridViewDataColumnSettings(GridViewDataColumn column) : this() { }
		public GridViewDataColumnSettings() : base() { }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowDragDrop { get { return base.AllowDragDrop; } set { base.AllowDragDrop = value; } }
		[DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowEllipsisInText { get { return base.AllowEllipsisInText; } set { base.AllowEllipsisInText = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowSort { get { return base.AllowSort; } set { base.AllowSort = value; } }
		[ DefaultValue(ColumnSortMode.Default), NotifyParentProperty(true)]
		public new ColumnSortMode SortMode { get { return base.SortMode; } set { base.SortMode = value; } }
		[ DefaultValue(ColumnFilterMode.Value), NotifyParentProperty(true)]
		public new ColumnFilterMode FilterMode { get { return base.FilterMode; } set { base.FilterMode = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowFilterBySearchPanel { get { return base.AllowFilterBySearchPanel; } set { base.AllowFilterBySearchPanel = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowHeaderFilter { get { return base.AllowHeaderFilter; } set { base.AllowHeaderFilter = value; } }
		[ DefaultValue(HeaderFilterMode.List), NotifyParentProperty(true),
		 Obsolete("Use the SettingsHeaderFilter.Mode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderFilterMode HeaderFilterMode { get { return base.HeaderFilterMode; } set { base.HeaderFilterMode = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean ShowInFilterControl { get { return base.ShowInFilterControl; } set { base.ShowInFilterControl = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsAllowGroup"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowGroup {
			get { return GetDefaultBooleanProperty("AllowGroup", DefaultBoolean.Default); }
			set {
				if(value == AllowGroup) return;
				SetDefaultBooleanProperty("AllowGroup", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsGroupInterval"),
#endif
 DefaultValue(ColumnGroupInterval.Default), NotifyParentProperty(true)]
		public ColumnGroupInterval GroupInterval {
			get { return (ColumnGroupInterval)GetEnumProperty("GroupInterval", ColumnGroupInterval.Default); }
			set {
				if(value == GroupInterval) return;
				SetEnumProperty("GroupInterval", ColumnGroupInterval.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsAllowAutoFilter"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowAutoFilter {
			get { return GetDefaultBooleanProperty("AllowAutoFilter", DefaultBoolean.Default); }
			set {
				if(value == AllowAutoFilter) return;
				SetDefaultBooleanProperty("AllowAutoFilter", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsAutoFilterCondition"),
#endif
 DefaultValue(AutoFilterCondition.Default), NotifyParentProperty(true)]
		public AutoFilterCondition AutoFilterCondition {
			get { return (AutoFilterCondition)GetEnumProperty("AutoFilterCondition", AutoFilterCondition.Default); }
			set {
				if(value == AutoFilterCondition) return;
				SetEnumProperty("AutoFilterCondition", AutoFilterCondition.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsAllowAutoFilterTextInputTimer"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowAutoFilterTextInputTimer {
			get { return GetDefaultBooleanProperty("AllowAutoFilterTextInputTimer", DefaultBoolean.Default); }
			set {
				if(value == AllowAutoFilterTextInputTimer) return;
				SetDefaultBooleanProperty("AllowAutoFilterTextInputTimer", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsShowFilterRowMenu"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowFilterRowMenu {
			get { return GetDefaultBooleanProperty("ShowFilterRowMenu", DefaultBoolean.Default); }
			set {
				if(ShowFilterRowMenu == value) return;
				SetDefaultBooleanProperty("ShowFilterRowMenu", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSettingsShowFilterRowMenuLikeItem"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean ShowFilterRowMenuLikeItem {
			get { return GetDefaultBooleanProperty("ShowFilterRowMenuLikeItem", DefaultBoolean.Default); }
			set {
				if(value == ShowFilterRowMenuLikeItem)
					return;
				SetDefaultBooleanProperty("ShowFilterRowMenuLikeItem", DefaultBoolean.Default, value);
				OnChanged();
			}
		}
		public override void Assign(GridDataColumnSettings source) {
			base.Assign(source);
			var src = source as GridViewDataColumnSettings;
			if(src == null) return;
			AllowGroup = src.AllowGroup;
			GroupInterval = src.GroupInterval;
			AllowAutoFilter = src.AllowAutoFilter;
			AutoFilterCondition = src.AutoFilterCondition;
			AllowAutoFilterTextInputTimer = src.AllowAutoFilterTextInputTimer;
			ShowFilterRowMenu = src.ShowFilterRowMenu;
			ShowFilterRowMenuLikeItem = src.ShowFilterRowMenuLikeItem;
		}
	}
	public class GridViewDataColumnHeaderFilterSettings : GridDataColumnHeaderFilterSettings {
		internal GridViewDataColumnHeaderFilterSettings(GridDataColumnAdapter columnAdapter)
			: base(columnAdapter) {
		}
		public GridViewDataColumnHeaderFilterSettings() {
		}
		[ DefaultValue(GridHeaderFilterMode.Default), NotifyParentProperty(true)]
		public new GridHeaderFilterMode Mode { get { return base.Mode; } set { base.Mode = value; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewColumnDateRangeCalendarSettings DateRangeCalendarSettings { get { return (GridViewColumnDateRangeCalendarSettings)base.DateRangeCalendarSettings; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewColumnDateRangePickerSettings DateRangePickerSettings { get { return (GridViewColumnDateRangePickerSettings)base.DateRangePickerSettings; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new GridViewColumnDateRangePeriodsSettings DateRangePeriodsSettings { get { return (GridViewColumnDateRangePeriodsSettings)base.DateRangePeriodsSettings; } }
		protected override GridColumnDateRangeCalendarSettings CreateCalendarSettings() {
			return new GridViewColumnDateRangeCalendarSettings();
		}
		protected override GridColumnDateRangePickerSettings CreateDateRangePickerSettings() {
			return new GridViewColumnDateRangePickerSettings();
		}
		protected override GridColumnDateRangePeriodsSettings CreateDateRangePeriodsSettings() {
			return new GridViewColumnDateRangePeriodsSettings();
		}
	}
	public class GridViewColumnDateRangeCalendarSettings : GridColumnDateRangeCalendarSettings {
		[ DefaultValue(StringResources.Calendar_Clear), NotifyParentProperty(true)]
		public new string ClearButtonText { get { return base.ClearButtonText; } set { base.ClearButtonText = value; } }
		[ DefaultValue(StringResources.Calendar_Today), NotifyParentProperty(true)]
		public new string TodayButtonText { get { return base.TodayButtonText; } set { base.TodayButtonText = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowClearButton { get { return base.ShowClearButton; } set { base.ShowClearButton = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowTodayButton { get { return base.ShowTodayButton; } set { base.ShowTodayButton = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowHeader { get { return base.ShowHeader; } set { base.ShowHeader = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowDayHeaders { get { return base.ShowDayHeaders; } set { base.ShowDayHeaders = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowWeekNumbers { get { return base.ShowWeekNumbers; } set { base.ShowWeekNumbers = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightWeekends { get { return base.HighlightWeekends; } set { base.HighlightWeekends = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightToday { get { return base.HighlightToday; } set { base.HighlightToday = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableMonthNavigation { get { return base.EnableMonthNavigation; } set { base.EnableMonthNavigation = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableYearNavigation { get { return base.EnableYearNavigation; } set { base.EnableYearNavigation = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableMultiSelect { get { return base.EnableMultiSelect; } set { base.EnableMultiSelect = value; } }
		[ DefaultValue(FirstDayOfWeek.Default), NotifyParentProperty(true)]
		public new FirstDayOfWeek FirstDayOfWeek { get { return base.FirstDayOfWeek; } set { base.FirstDayOfWeek = value; } }
		[ DefaultValue(typeof(DateTime), ""), NotifyParentProperty(true)]
		public new DateTime MinDate { get { return base.MinDate; } set { base.MinDate = value; } }
		[ DefaultValue(typeof(DateTime), ""), NotifyParentProperty(true)]
		public new DateTime MaxDate { get { return base.MaxDate; } set { base.MaxDate = value; } }
	}
	public class GridViewColumnDateRangePickerSettings : GridColumnDateRangePickerSettings {
		[ DefaultValue(DefaultDisplayFormatString), NotifyParentProperty(true)]
		public new string DisplayFormatString { get { return base.DisplayFormatString; } set { base.DisplayFormatString = value; } }
		[ DefaultValue(0), NotifyParentProperty(true)]
		public new int MinDayCount { get { return base.MinDayCount; } set { base.MinDayCount = value; } }
		[ DefaultValue(0), NotifyParentProperty(true)]
		public new int MaxDayCount { get { return base.MaxDayCount; } set { base.MaxDayCount = value; } }
	}
	public class GridViewColumnDateRangePeriodsSettings : GridColumnDateRangePeriodsSettings {
		[ DefaultValue(2), NotifyParentProperty(true)]
		public new int RepeatColumns { get { return base.RepeatColumns; } set { base.RepeatColumns = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowDaysSection { get { return base.ShowDaysSection; } set { base.ShowDaysSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowWeeksSection { get { return base.ShowWeeksSection; } set { base.ShowWeeksSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowMonthsSection { get { return base.ShowMonthsSection; } set { base.ShowMonthsSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowYearsSection { get { return base.ShowYearsSection; } set { base.ShowYearsSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowPastPeriods { get { return base.ShowPastPeriods; } set { base.ShowPastPeriods = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowFuturePeriods { get { return base.ShowFuturePeriods; } set { base.ShowFuturePeriods = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowPresentPeriods { get { return base.ShowPresentPeriods; } set { base.ShowPresentPeriods = value; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GridColumnEditFormSettings : StateManager {
		GridViewDataColumn column;
		public GridColumnEditFormSettings(GridViewDataColumn column) {
			this.column = column;
		}
		protected GridViewDataColumn Column { get { return column; } }
		protected void OnChanged() { Column.OnColumnChanged(); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsColumnSpan"),
#endif
		DefaultValue(0), NotifyParentProperty(true)]
		public int ColumnSpan {
			get { return (int)GetIntProperty("ColumnSpan", 0); }
			set {
				value = Math.Max(0, value);
				if(ColumnSpan == value) return;
				SetIntProperty("ColumnSpan", 0, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsRowSpan"),
#endif
		DefaultValue(0), NotifyParentProperty(true)]
		public int RowSpan {
			get { return (int)GetIntProperty("RowSpan", 0); }
			set {
				value = Math.Max(0, value);
				if(RowSpan == value) return;
				SetIntProperty("RowSpan", 0, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsVisible"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean Visible {
			get { return (DefaultBoolean)GetIntProperty("Visible", (int)DefaultBoolean.Default); }
			set {
				if(Visible == value) return;
				SetIntProperty("Visible", (int)DefaultBoolean.Default, (int)value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsVisibleIndex"),
#endif
		DefaultValue(-1), NotifyParentProperty(true)]
		public int VisibleIndex {
			get { return GetIntProperty("VisibleIndex", -1); }
			set {
				value = Math.Max(-1, value);
				if(VisibleIndex == value) return;
				SetIntProperty("VisibleIndex", -1, value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsCaptionLocation"),
#endif
		DefaultValue(ASPxColumnCaptionLocation.Default), NotifyParentProperty(true)]
		public ASPxColumnCaptionLocation CaptionLocation {
			get { return (ASPxColumnCaptionLocation)GetIntProperty("CaptionLocation", (int)ASPxColumnCaptionLocation.Default); }
			set {
				if(CaptionLocation == value) return;
				SetIntProperty("CaptionLocation", (int)ASPxColumnCaptionLocation.Default, (int)value);
				OnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GridColumnEditFormSettingsCaption"),
#endif
		DefaultValue(""), NotifyParentProperty(true)]
		public string Caption {
			get { return GetStringProperty("Caption", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				SetStringProperty("Caption", string.Empty, value);
				OnChanged();
			}
		}
		public virtual void Assign(GridColumnEditFormSettings source) {
			this.CaptionLocation = source.CaptionLocation;
			this.Caption = source.Caption;
			this.ColumnSpan = source.ColumnSpan;
			this.RowSpan = source.RowSpan;
			this.Visible = source.Visible;
			this.VisibleIndex = source.VisibleIndex;
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class GridViewColumnCollection : GridColumnCollection {
		public GridViewColumnCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public new GridViewColumn this[int index] { get { return base[index] as GridViewColumn; } }
		public new GridViewColumn this[string ID_FieldName_Caption] { get { return base[ID_FieldName_Caption] as GridViewColumn; } }
		[Browsable(false)]
		public new ASPxGridView Grid { get { return base.Grid as ASPxGridView; } }
		public void Add(GridViewColumn column) {
			base.Add(column);
		}
		public void AddRange(params GridViewColumn[] columns) {
			base.AddRange(columns);
		}
		public void Insert(int index, GridViewColumn column) {
			base.Insert(index, column);
		}
		public void Remove(GridViewColumn column) {
			base.Remove(column);
		}
		public int IndexOf(GridViewColumn column) {
			return base.IndexOf(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Add(IWebGridColumn column) {
			base.Add(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void AddRange(params IWebGridColumn[] columns) {
			base.AddRange(columns);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Insert(int index, IWebGridColumn column) {
			base.Insert(index, column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Remove(IWebGridColumn column) {
			base.Remove(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int IndexOf(IWebGridColumn column) {
			return base.IndexOf(column);
		}
		protected override Type GetKnownType() {
			return typeof(GridViewColumn);
		}
	}
	public class GridViewBandColumn : GridViewColumn, IWebColumnsOwner {
		GridViewColumnCollection columns;
		IWebColumnsOwner columnsOwnerImpl;
		public GridViewBandColumn(string caption) 
			: this() {
			Caption = caption;
		}
		public GridViewBandColumn() {
			this.columns = CreateColumnCollection();
			this.columnsOwnerImpl = new WebColumnsOwnerDefaultImplementation(this, Columns);
		}
		protected IWebColumnsOwner ColumnsOwnerImpl { get { return columnsOwnerImpl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBandColumnColumns"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public GridViewColumnCollection Columns { get { return columns; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewBandColumnAllowDragDrop"),
#endif
 DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public DefaultBoolean AllowDragDrop {
			get { return (DefaultBoolean)GetIntProperty("AllowDragDrop", (int)DefaultBoolean.Default); }
			set {
				if(AllowDragDrop == value) return;
				SetIntProperty("AllowDragDrop", (int)DefaultBoolean.Default, (int)value);
				OnColumnChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int AdaptivePriority { get { return base.AdaptivePriority; } set { base.AdaptivePriority = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override GridViewCellStyle CellStyle { get { return base.CellStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override GridViewGroupFooterStyle GroupFooterCellStyle { get { return base.GroupFooterCellStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override GridViewFooterStyle FooterCellStyle { get { return base.FooterCellStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ITemplate FooterTemplate { get { return base.FooterTemplate; } set { base.FooterTemplate = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ITemplate FilterTemplate { get { return base.FilterTemplate; } set { base.FilterTemplate = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ITemplate GroupFooterTemplate { get { return base.GroupFooterTemplate; } set { base.GroupFooterTemplate = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Unit Width { get { return base.Width; } set { base.Width = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MinWidth { get { return base.MinWidth; } set { base.MinWidth = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ExportWidth { get { return base.ExportWidth; } set { base.ExportWidth = value; } }
		protected override IStateManager[] GetStateManagedObjects() {
			List<IStateManager> list = new List<IStateManager>(base.GetStateManagedObjects());
			list.Add(Columns);
			return list.ToArray();
		}
		protected internal override bool GetAllowDragDrop() {
			if(AllowDragDrop == DefaultBoolean.Default && Grid != null)
				return Grid.SettingsBehavior.AllowDragDrop;
			return AllowDragDrop != DefaultBoolean.False;
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			if(IsDesignMode())
			return "Band";
			return string.Empty;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			GridViewBandColumn band = source as GridViewBandColumn;
			if(band != null) {
				Columns.Assign(band.Columns);
				AllowDragDrop = band.AllowDragDrop;
			}
		}
		protected virtual GridViewColumnCollection CreateColumnCollection() {
			return new GridViewColumnCollection(this);
		}
		WebColumnCollectionBase IWebColumnsOwner.Columns { get { return ColumnsOwnerImpl.Columns; } }
		void IWebColumnsOwner.EnsureVisibleIndices() { ColumnsOwnerImpl.EnsureVisibleIndices(); }
		List<WebColumnBase> IWebColumnsOwner.GetVisibleColumns() { return ColumnsOwnerImpl.GetVisibleColumns(); }
		void IWebColumnsOwner.OnColumnChanged(WebColumnBase column) { 
			ColumnsOwnerImpl.OnColumnChanged(column);
			if(Grid != null)
				Grid.ColumnHelper.Invalidate();
		}
		void IWebColumnsOwner.OnColumnCollectionChanged() { 
			ColumnsOwnerImpl.OnColumnCollectionChanged();
			if(Grid != null)
				Grid.ColumnHelper.Invalidate();
		}
		void IWebColumnsOwner.ResetVisibleColumns() { ColumnsOwnerImpl.ResetVisibleColumns(); }
		void IWebColumnsOwner.ResetVisibleIndices() { ColumnsOwnerImpl.ResetVisibleIndices(); }
		void IWebColumnsOwner.SetColumnVisible(WebColumnBase column, bool value) { ColumnsOwnerImpl.SetColumnVisible(column, value); }
		void IWebColumnsOwner.SetColumnVisibleIndex(WebColumnBase column, int value) { ColumnsOwnerImpl.SetColumnVisibleIndex(column, value); }
		protected override IList GetDesignTimeItems() {
			return (IList)Columns;			
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Columns" });
		}
	}
	#region EditColumns
	[ControlBuilder(typeof(ControlBuilder))]
	public abstract class GridViewEditDataColumn : GridViewDataColumn {
		public GridViewEditDataColumn() : base() { }
		public GridViewEditDataColumn(string fieldName) : base(fieldName) { }
		public GridViewEditDataColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Localizable(false), PersistenceMode(PersistenceMode.Attribute)]
		public override EditPropertiesBase PropertiesEdit { get { return base.PropertiesEdit; } set { } }
		protected override EditPropertiesBase CreateEditProperties() { return null; }
		public static GridViewEditDataColumn CreateColumn(Type dataType) {
			if(dataType == null) return new GridViewDataTextColumn();
			dataType = ReflectionUtils.StripNullableType(dataType);
			if(dataType.Equals(typeof(DateTime))) return new GridViewDataDateColumn();
			if(dataType.Equals(typeof(bool))) return new GridViewDataCheckColumn();
			return new GridViewDataTextColumn();
		}
	}
	public class GridViewDataTextColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new TextBoxProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTextColumnPropertiesTextEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TextBoxProperties PropertiesTextEdit { get { return (TextBoxProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTextEdit" }); }
	}
	public class GridViewDataButtonEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ButtonEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataButtonEditColumnPropertiesButtonEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ButtonEditProperties PropertiesButtonEdit { get { return (ButtonEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesButtonEdit" }); }
	}
	public class GridViewDataMemoColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new MemoProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataMemoColumnPropertiesMemoEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public MemoProperties PropertiesMemoEdit { get { return (MemoProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesMemoEdit" }); }
	}
	public class GridViewDataHyperLinkColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new HyperLinkProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataHyperLinkColumnPropertiesHyperLinkEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HyperLinkProperties PropertiesHyperLinkEdit { get { return (HyperLinkProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesHyperLinkEdit" }); }
	}
	public class GridViewDataCheckColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new CheckBoxProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataCheckColumnPropertiesCheckEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckBoxProperties PropertiesCheckEdit { get { return (CheckBoxProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesCheckEdit" }); }
	}
	public class GridViewDataDateColumn : GridViewEditDataColumn, IDateEditIDResolver {
		protected override EditPropertiesBase CreateEditProperties() { return new DateEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataDateColumnPropertiesDateEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DateEditProperties PropertiesDateEdit { get { return (DateEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesDateEdit" }); }
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) { return (ColumnAdapter as IDateEditIDResolver).GetDateEditIdByDataItemName(dataItemName); }
		string[] IDateEditIDResolver.GetPossibleDataItemNames() { return (ColumnAdapter as IDateEditIDResolver).GetPossibleDataItemNames(); }
	}
	public class GridViewDataSpinEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new SpinEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataSpinEditColumnPropertiesSpinEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpinEditProperties PropertiesSpinEdit { get { return (SpinEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesSpinEdit" }); }
	}
	public class GridViewDataComboBoxColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ComboBoxProperties(this); } 
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataComboBoxColumnPropertiesComboBox"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ComboBoxProperties PropertiesComboBox { get { return (ComboBoxProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesComboBox" }); }
	}
	public class GridViewDataTokenBoxColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new TokenBoxProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTokenBoxColumnPropertiesTokenBox"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TokenBoxProperties PropertiesTokenBox { get { return (TokenBoxProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTokenBox" }); }
	}
	public class GridViewDataDropDownEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new DropDownEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataDropDownEditColumnPropertiesDropDownEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DropDownEditProperties PropertiesDropDownEdit { get { return (DropDownEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesDropDownEdit" }); }
	}
	public class GridViewDataImageColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ImageEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataImageColumnPropertiesImage"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageEditProperties PropertiesImage { get { return (ImageEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesImage" }); }
	}
	public class GridViewDataBinaryImageColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new BinaryImageEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataBinaryImageColumnPropertiesBinaryImage"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public BinaryImageEditProperties PropertiesBinaryImage { get { return (BinaryImageEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesBinaryImage" });
		}
	}
	public class GridViewDataProgressBarColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ProgressBarProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataProgressBarColumnPropertiesProgressBar"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ProgressBarProperties PropertiesProgressBar { get { return (ProgressBarProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesProgressBar" }); }
	}
	public class GridViewDataColorEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new ColorEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColorEditColumnPropertiesColorEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ColorEditProperties PropertiesColorEdit { get { return (ColorEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesColorEdit" }); }
	}
	public class GridViewDataTimeEditColumn : GridViewEditDataColumn {
		protected override EditPropertiesBase CreateEditProperties() { return new TimeEditProperties(this); }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTimeEditColumnPropertiesTimeEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TimeEditProperties PropertiesTimeEdit { get { return (TimeEditProperties)PropertiesEdit; } }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTimeEdit" }); }
	}
	#endregion
}
