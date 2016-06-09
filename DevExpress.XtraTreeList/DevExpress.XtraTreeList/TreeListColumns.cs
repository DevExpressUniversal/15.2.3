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

namespace DevExpress.XtraTreeList.Columns {
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
	#region class TreeListColumn
	public enum FixedStyle { None, Left, Right };
	[DesignTimeVisible(false), ToolboxItem(false),
	Designer("DevExpress.XtraTreeList.Design.TreeListColumnDesigner, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class TreeListColumn : Component, IAppearanceOwner, DevExpress.Data.IDataColumnInfo, IHeaderObject, IXtraSerializableLayoutEx {
		protected const int LayoutIdAppearance = 1;
		string fieldName, name, caption, customizationCaption, styleName, toolTip;
		ColumnFormatInfo format;
		TreeListOptionsColumn optionsColumn;
		TreeListOptionsColumnFilter optionsFilter;
		ShowButtonModeEnum showButtonMode;
		bool allNodesSummary;
		bool allowIncrementalSearch;
		bool visible;
		object tag;
		TreeListBand parentBand;
		internal int visibleWidth;
		int minWidth;
		internal int absoluteIndex, columnHandle,
			width, 
			visibleIndex;
		int prevVisibleIndex = -1;
		internal SortOrder sortOrder;
		internal int sortIndex;
		int imageIndex;
		StringAlignment imageAlignment;
		UnboundColumnType unboundType;
		FixedStyle _fixed;
		TreeListColumnFilterInfo filterInfo;
		ColumnFilterMode filterMode;
		ColumnSortMode sortMode;
		TreeListFilterInfoCollection mruFilters;
		Size filterPopupSize = Size.Empty;
		string unboundExpression;
		protected SummaryItemType fRowFooterSummary;
		protected SummaryItemType fSummaryFooter;
		protected string fRowFooterSummaryStrFormat;
		protected string fSummaryFooterStrFormat;
		internal DevExpress.XtraEditors.Repository.RepositoryItem columnEdit;
		internal TreeListColumnCollection columns;
		const int defaultColumnWidth = 75, defaultMinColumnWidth = 20, minimumMinColumnWidth = 16, maximumMinColumnWidth = 1000;
		AppearanceObjectEx appearanceCell;
		AppearanceObject appearanceHeader;
		bool showUnboundExpressionMenu;
		int rowIndex;
		int rowCount;
		public TreeListColumn() {
			this.fieldName = this.name = this.caption = this.styleName = this.customizationCaption = this.toolTip = string.Empty;
			this.format = new ColumnFormatInfo(this);
			this.format.Changed += new EventHandler(OnFormatChanged);
			this.optionsColumn = CreateOptionsColumn();
			this.optionsColumn.Changed += new BaseOptionChangedEventHandler(OnOptionsColumnChanged);
			this.filterInfo = new TreeListColumnFilterInfo();
			this.filterMode = ColumnFilterMode.Value;
			this.mruFilters = new TreeListFilterInfoCollection();
			this.optionsFilter = CreateOptionsFilter();
			this.optionsFilter.Changed += new BaseOptionChangedEventHandler(OnOptionsColumnChanged);
			this.imageAlignment = StringAlignment.Near;
			this.showButtonMode = ShowButtonModeEnum.Default;
			this.visibleIndex = this.absoluteIndex = this.columnHandle = this.imageIndex = this.sortIndex = -1;
			this.visible = false;
			this.visibleWidth = this.width = defaultColumnWidth;
			this.rowIndex = 0;
			this.rowCount = 1;
			this.minWidth = defaultMinColumnWidth;
			this.sortOrder = SortOrder.None;
			this.fRowFooterSummary = SummaryItemType.None;
			this.fSummaryFooter = SummaryItemType.None;
			this.allNodesSummary = false;
			this.fRowFooterSummaryStrFormat = "{0}";
			this.fSummaryFooterStrFormat = "{0}";
			this.unboundType = UnboundColumnType.Bound;
			this.columnEdit = null;
			this.columns = null;
			this.tag = null;
			this.allowIncrementalSearch = true;
			this.showUnboundExpressionMenu = false;
			this._fixed = FixedStyle.None;
			this.unboundExpression = string.Empty;
			this.appearanceCell = new AppearanceObjectEx(this);
			this.appearanceCell.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceHeader = new AppearanceObject(this, false);
			this.appearanceHeader.Changed += new EventHandler(this.OnAppearanceChanged);
		}
		protected virtual TreeListOptionsColumn CreateOptionsColumn() {
			return new TreeListOptionsColumn();
		}
		protected virtual TreeListOptionsColumnFilter CreateOptionsFilter() { 
			return new TreeListOptionsColumnFilter(); 
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				appearanceCell.Changed -= new EventHandler(OnAppearanceChanged);
				appearanceHeader.Changed -= new EventHandler(OnAppearanceChanged);
				if(ColumnEdit != null)
					ColumnEdit.Disconnect(this);
				if(columns != null)
					columns.Remove(this);
				this.optionsColumn.Changed -= new BaseOptionChangedEventHandler(OnOptionsColumnChanged);
				this.filterInfo.Clear();
			}
			base.Dispose(disposing);
		}
		bool IsLoading { get { return TreeList == null || TreeList.IsLoading || TreeList.IsDeserializing; } }
		bool IAppearanceOwner.IsLoading { get { return IsLoading; } }
		void OnAppearanceChanged(object sender, EventArgs e) {
			Changed();
		}
		#region public
		[Browsable(false)]
		public Type ColumnType {
			get { return ColumnInfo != null ? ColumnInfo.Type : typeof(object); }
		}
		DevExpress.Data.Utils.AnnotationAttributes columnAnnotationAttributes;
		[Browsable(false)]
		public virtual DevExpress.Data.Utils.AnnotationAttributes ColumnAnnotationAttributes {
			get {
				if(columnAnnotationAttributes == null) {
					if(ColumnInfo != null && ColumnInfo.Descriptor != null)
						columnAnnotationAttributes = new DevExpress.Data.Utils.AnnotationAttributes(ColumnInfo.Descriptor);
				}
				return columnAnnotationAttributes;
			}
		}
		protected virtual internal void SetColumnAnnotationAttributes(DevExpress.Data.Utils.AnnotationAttributes annotationAttributes) {
			this.columnAnnotationAttributes = annotationAttributes;
		}
		protected virtual string GetCaptionFromColumnAnnotation() {
			return DevExpress.Data.Utils.AnnotationAttributes.GetColumnCaption(ColumnAnnotationAttributes);
		}
		protected virtual string GetDescriptionFromColumnAnnotation() {
			return DevExpress.Data.Utils.AnnotationAttributes.GetColumnDescription(ColumnAnnotationAttributes);
		}
		bool ShouldSerializeOptionsColumn() { return OptionsColumn.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnOptionsColumn"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsColumn OptionsColumn { get { return optionsColumn; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnOptionsFilter"),
#endif
 Category("Options"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public TreeListOptionsColumnFilter OptionsFilter { get { return optionsFilter; } }
		protected DataColumnInfo ColumnInfo { get { return TreeList != null ? TreeList.GetDataColumnInfo(FieldName) : null; } }
		protected virtual void OnOptionsColumnChanged(object sender, BaseOptionChangedEventArgs e) {
			if(TreeList != null)
				TreeList.ColumnOptionsChanged(this, e);
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnCaption"),
#endif
 Category("Appearance"), DefaultValue(""), Localizable(true), XtraSerializableProperty()]
		public virtual string Caption { 
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption != value) {
					caption = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnToolTip"),
#endif
 Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string ToolTip {
			get { return toolTip; }
			set {
				if(ToolTip == value) return;
				toolTip = value;
				Changed();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnCustomizationCaption"),
#endif
 Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string CustomizationCaption {
			get { return customizationCaption; }
			set {
				if(value == null) value = string.Empty;
				if(CustomizationCaption != value) {
					customizationCaption = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnFieldName"),
#endif
 Category("Data"), DefaultValue(""), XtraSerializableProperty(),
		TypeConverter("DevExpress.XtraTreeList.Design.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyTreeListDesign)]
		public string FieldName {
			get { 
				if(fieldName == string.Empty && TreeList != null && TreeList.IsUnboundMode) 
					return Caption;
				return fieldName;
			}
			set {
				if(value == null) value = string.Empty;
				if(fieldName != value) {
					fieldName = value;
					Changed();
				}
				if(TreeList != null)
					TreeList.SetColumnHandle(this);
			}
		}
		[Browsable(false), DefaultValue(""), XtraSerializableProperty()]
		public virtual string Name {
			get { 
				if(this.Site != null) 
					name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				if(this.Site != null) this.Site.Name = value;
				name = value;
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnAllowIncrementalSearch"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool AllowIncrementalSearch {
			get { return allowIncrementalSearch; }
			set {
				if(AllowIncrementalSearch == value) return;
				bool prevValue = AllowIncrementalSearch;
				allowIncrementalSearch = value;
			}
		}
		[Category("Behavior"),  DefaultValue(FixedStyle.None), XtraSerializableProperty(), Localizable(true)]
		public virtual FixedStyle Fixed {
			get { return _fixed; }
			set {
				if(Fixed == value) return;
				_fixed = value;
				if(TreeList != null) {
					TreeList.SetColumnFixedStyle(this, value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public virtual string ColumnEditName {
			get { return (ColumnEdit == null ? string.Empty : ColumnEdit.Name); }
			set {
				if(value == null) value = string.Empty;
				if(value == ColumnEditName || TreeList == null) return;
				DevExpress.XtraEditors.Repository.RepositoryItem item = TreeList.ContainerHelper.FindRepositoryItem(value);
				if(item != null) ColumnEdit = item;
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnColumnEdit"),
#endif
 Category("Data"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null),
		Editor("DevExpress.XtraTreeList.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyTreeListDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public RepositoryItem ColumnEdit {
			get { return columnEdit; }
			set {
				if(columnEdit != value) {
					cachedActualColumnSortMode = null;
					RepositoryItem old = columnEdit;
					columnEdit = value;
					if(TreeList == null) return;
					if(old != null) old.Disconnect(this);
					if(ColumnEdit != null)
						ColumnEdit.Connect(this);
					Changed();
				}
			}
		}
		[Browsable(false)]
		public RepositoryItem RealColumnEdit {
			get {
				var ce = ColumnEdit == null ? null : (ColumnEdit.IsDisposed ? null : ColumnEdit);
				if(TreeList == null || ce != null) return ce;
				return TreeList.GetColumnDefaultRepositoryItem(this);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnShowButtonMode"),
#endif
 DefaultValue(ShowButtonModeEnum.Default), XtraSerializableProperty(), Category("Appearance")]
		public ShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(ShowButtonMode == value) return;
				showButtonMode = value;
				Changed();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnVisibleIndex"),
#endif
 Category("Appearance"), DefaultValue(-1), Localizable(true), XtraSerializableProperty()]
		public int VisibleIndex {
			get {
				return !Visible ? -1 : visibleIndex;
			}
			set {
				if(IsLoading || (TreeList != null && TreeList.HasBands)) {
					visibleIndex = value;
					this.visible = this.visibleIndex >= 0;
					if(!IsLoading) {
						TreeList.NormalizeVisibleColumnIndices();
						Changed();
					}
					return;
				}
				TreeList.SetColumnVisibleIndex(this, value, VisibleIndex);
				Changed();
			}
		}
		[ Category("Appearance"), DefaultValue(false), XtraSerializableProperty(), Localizable(true)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(IsLoading) {
					visible = value;
					visibleIndex = visibleIndex < 0 ? 100000 : visibleIndex;
					return;
				}
				if(Visible == value) return;
				int prevIndex = VisibleIndex;
				this.visible = value;
				if(visibleIndex > -1) prevVisibleIndex = visibleIndex;
				if(value)
					this.visibleIndex = prevVisibleIndex > -1 ? prevVisibleIndex : TreeList.VisibleColumns.Count;
				else
					this.visibleIndex = -1;
				TreeList.SetColumnVisibleIndex(this, this.visibleIndex, prevIndex);
				Changed();
			}
		}
		internal void SetVisibleCore(bool newVisible) {
			this.visible = newVisible;
		}
		internal void SetVisibleCore(bool newVisible, int newVisibleIndex) {
			this.visible = newVisible;
			if(this.visibleIndex > -1) this.prevVisibleIndex = this.visibleIndex;
			this.visibleIndex = newVisibleIndex;
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnImageIndex"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), Localizable(true),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.ImageList("Images"), Category("Appearance")]
		public int ImageIndex { 
			get { return imageIndex; }
			set {
				if(ImageIndex != value) {
					imageIndex = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnImageAlignment"),
#endif
 Category("Appearance"), DefaultValue(StringAlignment.Near), XtraSerializableProperty(), Localizable(true)]
		public StringAlignment ImageAlignment { 
			get { return imageAlignment; }
			set {
				if(ImageAlignment != value) {
					imageAlignment = value;
					Changed();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images { get { return (TreeList == null ? null : TreeList.ColumnsImageList); } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnWidth"),
#endif
 Category("Appearance"), DefaultValue(defaultColumnWidth), Localizable(true), XtraSerializableProperty()]
		public int Width { 
			get { return width; }
			set { 
				if(TreeList == null) {
					width = value;
					return;
				}
				if(value < MinWidth) 
					value = MinWidth;
				if(value == Width) return;
				width = value; 
				SetVisibleWidth(visibleWidth); 
				if(TreeList.IsIniting) return;
				Changed();
				TreeList.InternalColumnWidthChanged(this);
			}
		}
		internal void SetVisibleWidth(int value) {
			this.visibleWidth = value;
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string StyleName {
			get { return styleName; }
			set { styleName = value; }
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnSortOrder"),
#endif
 Category("Data"), DefaultValue(SortOrder.None), Localizable(true), XtraSerializableProperty()]
		public SortOrder SortOrder { 
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				if(!(TreeList == null || TreeList.IsIniting)) {
					TreeList.RecalcSortColumns(this, false);
					TreeList.FireChanged();
				}
			}
		}
		[Browsable(false), DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty]
		public int SortIndex {
			get { return sortIndex; }
			set {
				if(value < 0) value = -1;
				if(SortIndex == value) return;
				if(TreeList == null || TreeList.IsIniting) {
					sortIndex = value;
					return;
				}
				TreeList.SetSortedColumnIndex(this, value);
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnSortMode"),
#endif
 DefaultValue(ColumnSortMode.Default), XtraSerializableProperty()]
		public ColumnSortMode SortMode {
			get { return sortMode; }
			set {
				if(SortMode == value) return;
				sortMode = value;
				cachedActualColumnSortMode = null;
				if(TreeList != null)
					TreeList.OnSortModeChanged();
			}
		}
		[Category("Data"), 
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnFilterMode"),
#endif
 DefaultValue(ColumnFilterMode.Value), XtraSerializableProperty()]
		public ColumnFilterMode FilterMode {
			get { return filterMode; }
			set {
				if(FilterMode == value) return;
				filterMode = value;
				if(TreeList != null)
					TreeList.OnFilterModeChanged();
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnShowUnboundExpressionMenu"),
#endif
 DefaultValue(false), Category("Data"), XtraSerializableProperty()]
		public bool ShowUnboundExpressionMenu {
			get { return showUnboundExpressionMenu; }
			set {
				if(ShowUnboundExpressionMenu == value) return;
				showUnboundExpressionMenu = value;
				Changed();
			}
		}
		ColumnSortMode? cachedActualColumnSortMode = null;
		protected internal ColumnSortMode GetActualSortMode() {
			if(cachedActualColumnSortMode != null)
				return cachedActualColumnSortMode.Value;
			ColumnSortMode sort = SortMode;
			if(sort == ColumnSortMode.Default) {
				RepositoryItem item = GetActualColumnEdit();
				sort = (item != null && item.RequireDisplayTextSorting) ? ColumnSortMode.DisplayText : ColumnSortMode.Value;
			}
			cachedActualColumnSortMode = sort;
			return sort;
		}
		protected internal RepositoryItem GetActualColumnEdit() {
			if(TreeList == null || ColumnEdit != null) return ColumnEdit;
			return TreeList.GetColumnDefaultRepositoryItem(this);
		} 
		bool ShouldSerializeAppearanceCell() { return AppearanceCell.ShouldSerialize(); }
		bool ShouldSerializeAppearanceHeader() { return AppearanceHeader.ShouldSerialize(); }
		void ResetAppearanceCell() { AppearanceCell.Reset(); }
		void ResetAppearanceHeader() { AppearanceHeader.Reset(); }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnAppearanceCell"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public AppearanceObjectEx AppearanceCell { get { return appearanceCell; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnAppearanceHeader"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)]
		public AppearanceObject AppearanceHeader { get { return this.appearanceHeader;  } }
		[Browsable(false)]
		public TreeList TreeList { 
			get {
				if(columns == null) return null;
				return columns.TreeList;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AbsoluteIndex { 
			get { 
				if(absoluteIndex == -1 && TreeList != null)
					absoluteIndex = TreeList.GetColumnAbsoluteIndex(this);
				return absoluteIndex; 
			}
			set { 
				absoluteIndex = value;
				columns.SetItemIndex(this, value); 
			}
		}
		[Browsable(false)]
		public int ColumnHandle { get { return columnHandle; } }
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnFormat"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public FormatInfo Format { get { return format; } }
		[Browsable(false)]
		public bool ReadOnly {
			get {
				bool IsDataColumnReadOnly = true;
				if(TreeList != null && TreeList.OptionsBehavior.ReadOnly)
					return true;
				if (ColumnInfo != null) IsDataColumnReadOnly = ColumnInfo.ReadOnly;
				return (OptionsColumn.ReadOnly || IsDataColumnReadOnly);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnMinWidth"),
#endif
 Category("Appearance"), DefaultValue(defaultMinColumnWidth), Localizable(true), XtraSerializableProperty()]
		public virtual int MinWidth {
			get {
				if(TreeList == null || TreeList.IsLoading) return this.minWidth;
				int val = TreeList.MinWidth == 0 ? this.minWidth : TreeList.MinWidth;
				return Math.Max(val, TreeList.GetColumnIndent(this) + minimumMinColumnWidth);
			}
			set {
				if(value < minimumMinColumnWidth) value = minimumMinColumnWidth;
				if(value > maximumMinColumnWidth) value = maximumMinColumnWidth;
				if(this.minWidth == value) return;
				this.minWidth = value;
				if(Width < MinWidth) Width = MinWidth;
				else Changed();
			}
		}
		[Browsable(false)]
		public int VisibleWidth { 
			get { return Math.Max(MinWidth, visibleWidth); } 
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnRowFooterSummary"),
#endif
 Category("Data"), DefaultValue(SummaryItemType.None), Localizable(true), XtraSerializableProperty()]
		public SummaryItemType RowFooterSummary {
			get { return fRowFooterSummary; }
			set {
				if(RowFooterSummary != value) {
					fRowFooterSummary = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnSummaryFooter"),
#endif
 Category("Data"), DefaultValue(SummaryItemType.None), Localizable(true), XtraSerializableProperty()]
		public SummaryItemType SummaryFooter {
			get { return fSummaryFooter; }
			set {
				if(SummaryFooter != value) {
					fSummaryFooter = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnAllNodesSummary"),
#endif
 Category("Data"), DefaultValue(false), Localizable(true), XtraSerializableProperty()]
		public bool AllNodesSummary {
			get { return allNodesSummary; }
			set {
				if(AllNodesSummary != value) {
					allNodesSummary = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnRowFooterSummaryStrFormat"),
#endif
 Category("Appearance"), DefaultValue("{0}"), Localizable(true), XtraSerializableProperty()]
		public string RowFooterSummaryStrFormat {
			get { return fRowFooterSummaryStrFormat; }
			set {
				if(RowFooterSummaryStrFormat != value) {
					fRowFooterSummaryStrFormat = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnSummaryFooterStrFormat"),
#endif
 Category("Appearance"), DefaultValue("{0}"), Localizable(true), XtraSerializableProperty()]
		public string SummaryFooterStrFormat {
			get { return fSummaryFooterStrFormat; }
			set {
				if(SummaryFooterStrFormat != value) {
					fSummaryFooterStrFormat = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnTag"),
#endif
 Category("Data"), DefaultValue(null), XtraSerializableProperty(),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public object Tag {
			get { return tag; }
			set {
				if(Tag != value) {
					tag = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnUnboundType"),
#endif
 Category("Data"), DefaultValue(UnboundColumnType.Bound), XtraSerializableProperty()]
		public UnboundColumnType UnboundType {
			get { return unboundType; }
			set {
				if(UnboundType == value) return;
				unboundType = value;
				if(TreeList != null)
					TreeList.OnColumnUnboundChanged(this);
			}
		}
		[
#if !SL
	DevExpressXtraTreeListLocalizedDescription("TreeListColumnUnboundExpression"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(""), XtraSerializableProperty(),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor))]
		public string UnboundExpression {
			get { return unboundExpression; }
			set {
				if(value == null) value = string.Empty;
				if(UnboundExpression == value) return;
				unboundExpression = value;
				if(TreeList != null) 
					TreeList.OnColumnUnboundExpressionChanged(this);
				Changed();
			}
		}
		public virtual void BestFit() {
			if(TreeList == null || VisibleIndex == -1) return;
			int cx = TreeList.CalcColumnBestWidth(this) - VisibleWidth;
			TreeList.ResizeColumnOnBestFit(VisibleIndex, cx);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual TreeListBand ParentBand {
			get { return parentBand; }
			internal set {
				if(ParentBand == value) return;
				this.parentBand = value;
			}
		}
		[Browsable(false), DefaultValue(0), XtraSerializableProperty()]
		public virtual int RowIndex {
			get { return rowIndex; }
			set {
				if(value < 0) value = 0;
				rowIndex = value;
				if(TreeList != null)
					TreeList.OnColumnRowIndexChanged(this);
				Changed();
			}
		}
		protected internal void SetRowIndex(int value) {
			this.rowIndex = value;
		}
		[Browsable(false)]
		public virtual int ColumnIndex {
			get {
				if(TreeList == null) return -1;
				return TreeList.GetBandColumnIndex(this);
			}
		}
		#endregion public
		#region internal
		protected void Changed() {
			if(IsLoading) return;
			if(TreeList != null) 
				TreeList.InternalColumnChanged(this);
		}
		internal void AssignTo(TreeListColumn column) {
			column.ParentBand = null;
			column.absoluteIndex = this.AbsoluteIndex;
			column.visibleIndex = this.VisibleIndex;
			column.visible = this.Visible;
			column.width = this.Width;
			column.toolTip = this.ToolTip;
			column.showButtonMode = this.ShowButtonMode;
			column.visibleWidth = this.VisibleWidth;
			column.caption = this.Caption;
			column.OptionsColumn.Assign(this.OptionsColumn);
			column.OptionsFilter.Assign(this.OptionsFilter);
			column.fieldName = this.FieldName;
			column.format.Assign(this.Format);
			column.columnEdit = this.ColumnEdit;
			column.styleName = this.StyleName;
			column.sortOrder = this.SortOrder;
			column.sortIndex = this.SortIndex;
			column.fRowFooterSummary = this.RowFooterSummary;
			column.fSummaryFooter = this.SummaryFooter;
			column.fRowFooterSummaryStrFormat = this.RowFooterSummaryStrFormat;
			column.fSummaryFooterStrFormat = this.SummaryFooterStrFormat;
			column.allNodesSummary = this.AllNodesSummary;
			column.columnHandle = this.ColumnHandle;
			column.tag = this.Tag;
			column.unboundType = this.UnboundType;
			column.unboundExpression = this.UnboundExpression;
			column.allowIncrementalSearch = this.allowIncrementalSearch;
			column.appearanceCell.AssignInternal(this.AppearanceCell);
			column.appearanceHeader.AssignInternal(this.AppearanceHeader);
			column.filterMode = this.FilterMode;
			column.sortMode = this.SortMode;
			column.showUnboundExpressionMenu = this.ShowUnboundExpressionMenu;
			column.rowIndex = this.RowIndex;
			cachedActualColumnSortMode = null;
		}
 		protected internal void FireChanged() {
			DevExpress.XtraTreeList.Helpers.DesignHelper.FireChanged(this, IsLoading, DesignMode, GetService(typeof(IComponentChangeService)) as IComponentChangeService);
		}
		internal HorzAlignment DefaultCellTextAlignment {
			get {
				if(TreeList == null) return HorzAlignment.Near;
				return TreeList.ContainerHelper.GetDefaultValueAlignment(ColumnEdit, ColumnType);
			}
		}
		internal void SetColumnHandle(int handle) {
			if(columnHandle != handle && TreeList != null) {
				if(TreeList.IsValidColumnHandle(handle)) {
					columnHandle = handle;
				}
			}
		}
		public virtual string GetCaption() {
			string caption = !string.IsNullOrEmpty(Caption) ?
				Caption : GetCaptionFromColumnAnnotation();
			if(!string.IsNullOrEmpty(caption))
				return caption;
			if(ColumnInfo != null && !string.IsNullOrEmpty(ColumnInfo.Caption) && ColumnInfo.Caption != FieldName)
				return ColumnInfo.Caption;
			return MasterDetailHelper.SplitPascalCaseString(FieldName);
		}
		public virtual string GetDescription() {
			return GetDescriptionFromColumnAnnotation();
		}
		public virtual string GetTextCaption() {
			if(TreeList == null) return GetCaption();
			return TreeList.GetNonFormattedCaption(GetCaption());
		}
		protected internal string GetCustomizationCaption() {
			if(CustomizationCaption == string.Empty) return GetCaption();
			return CustomizationCaption;
		}
		internal bool IsIListColumn {
			get {
				DataColumnInfo info = TreeList.GetDataColumnInfo(fieldName);
				if(info == null) return false;
				return info.Fixed;
			}
		}
		#endregion internal
		#region protected
		protected virtual void OnFormatChanged(object sender, EventArgs e) {
			Changed();
		}
		#endregion protected
		protected internal class ColumnFormatInfo : FormatInfo {
			TreeListColumn column;
			public ColumnFormatInfo(TreeListColumn column) {
				this.column = column;
			}
			public void ApplyColumnType(Type columnType) {
				FormatType = GetFormatTypeByTypeCode(Type.GetTypeCode(columnType));
			}
			private FormatType GetFormatTypeByTypeCode(TypeCode tc) {
				if(tc == TypeCode.DateTime) 
					return FormatType.DateTime;
				if(tc == TypeCode.SByte || tc == TypeCode.Int16 ||
					tc == TypeCode.Int32 || tc == TypeCode.Int64 ||
					tc == TypeCode.UInt16 || tc == TypeCode.UInt32 || 
					tc == TypeCode.UInt64 || tc == TypeCode.Decimal || 
					tc == TypeCode.Double || tc == TypeCode.Single)
					return FormatType.Numeric; 
				return FormatType.None;
			}
			protected override bool IsLoading { 
				get {
					if(column.TreeList == null || column.TreeList.IsLoading) return true;
					return base.IsLoading;
				} 
			}
		}
		#region IDataColumnInfo Members
		string DevExpress.Data.IDataColumnInfo.Caption { get { return GetTextCaption(); } }
		List<DevExpress.Data.IDataColumnInfo> DevExpress.Data.IDataColumnInfo.Columns {
			get {
				List<DevExpress.Data.IDataColumnInfo> res = new List<DevExpress.Data.IDataColumnInfo>();
				if(TreeList == null) return res;
				foreach(TreeListColumn col in TreeList.Columns) {
					if(col == this) continue;
					res.Add(col);
				}
				return res;
			}
		}
		DevExpress.Data.DataControllerBase DevExpress.Data.IDataColumnInfo.Controller { get { return null; } }
		string DevExpress.Data.IDataColumnInfo.FieldName { get { return FieldName; } }
		Type DevExpress.Data.IDataColumnInfo.FieldType { get { return ColumnType; } }
		string DevExpress.Data.IDataColumnInfo.Name { get { return Name; } }
		string DevExpress.Data.IDataColumnInfo.UnboundExpression { get { return UnboundExpression; } }
		#endregion
		#region Serialization
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
			if(opt != null && opt.StoreAppearance) {
				AppearanceCell.Reset();
				AppearanceHeader.Reset();
			}
		}
		#endregion
		#region Filtering
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TreeListColumnFilterInfo FilterInfo { get { return filterInfo; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListFilterInfoCollection MRUFilters { get { return mruFilters; } }
		internal Size FilterPopupSize { get { return filterPopupSize; } set { filterPopupSize = value; } }
		protected internal virtual FilterPopupMode GetFilterPopupMode() {
			FilterPopupMode mode = OptionsFilter.FilterPopupMode;
			if(mode == FilterPopupMode.Default && (ColumnType.Equals(typeof(DateTime)) || ColumnType.Equals(typeof(DateTime?))))
				mode = FilterPopupMode.Date;
			if(mode == FilterPopupMode.Default) 
				mode = FilterPopupMode.List;
			return mode;
		}
		protected internal bool IsCheckedListFilterPopupMode { get { return GetFilterPopupMode() == FilterPopupMode.CheckedList; } }
		protected internal bool IsDateFilterPopupMode { get { return GetFilterPopupMode() == FilterPopupMode.Date; } }
		protected internal bool IsListFilterPopupMode { get { return GetFilterPopupMode() == FilterPopupMode.List; } }
		#endregion
		[DXCategory(CategoryName.Appearance), DefaultValue(1), XtraSerializableProperty(), Localizable(true)]
		public int RowCount {
			get { return rowCount; }
			set {
				if(value < 1) value = 1;
				if(value > 10) value = 10;
				if(RowCount == value) return;
				rowCount = value;
				Changed();
			}
		}
		protected internal virtual bool AutoFill { get { return true; } }
		bool IHeaderObject.FixedWidth { get { return OptionsColumn.FixedWidth; } }
		IList IHeaderObject.Children { get { return null; } }
		IList IHeaderObject.Columns { get { return null; } }
		IHeaderObject IHeaderObject.Parent { get { return ParentBand; } }
		void IHeaderObject.SetWidth(int width, bool onlyVisibleWidth) {
			SetVisibleWidth(width);
			if(onlyVisibleWidth) return;
			this.width = width;
		}
		public override string ToString() {
			return GetCaption();
		}
	}
	#endregion class TreeListColumn
	#region class TreeListColumnCollection 
	[ToolboxItem(false), ListBindable(false)]
	public class TreeListColumnCollection : CollectionBase, IHeaderObjectCollection<TreeListColumn> {
		TreeList treeList;
		int updateCounter;
		internal bool isClearing;
		public event CollectionChangeEventHandler CollectionChanged;
		public TreeListColumnCollection(TreeList owner) {
			this.treeList = owner;
			this.updateCounter = 0;
		}
		#region public
		[Browsable(false)]
		public TreeList TreeList { get { return treeList; } }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListColumnCollectionItem")]
#endif
		public TreeListColumn this[string fieldName] { 
			get {
				for(int n = 0; n < Count; n++) {
					TreeListColumn col = (TreeListColumn)List[n];
					if(col.FieldName == fieldName) return col;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListColumnCollectionItem")]
#endif
		public TreeListColumn this[int index] { 
			get { 
				if(index < 0 || index > List.Count - 1) return null;
				return (TreeListColumn)List[index];  }  
		}
		public void AssignTo(TreeListColumnCollection columns) {
			columns.BeginUpdate();
			try {
				columns.Clear();
				foreach(TreeListColumn col in this) {
					TreeListColumn newCol = columns.Add();
					col.AssignTo(newCol);
				}
			}
			finally {
				columns.EndUpdate();
			}
		}
#if !DXWhidbey
		[Obsolete("Use the AddField method instead")]
		public TreeListColumn Add(string fieldName) {
			return AddField(fieldName);
		}
#endif
		public TreeListColumn AddField(string fieldName) {
			TreeListColumn column = CreateColumn();
			column.FieldName = fieldName;
			List.Add(column);
			return column;
		}
		public TreeListColumn AddVisible(string fieldName) {
			TreeListColumn column = AddField(fieldName);
			column.Visible = true;
			return column;
		}
		public int Add(TreeListColumn column) {
			if(List.Contains(column)) return List.IndexOf(column);
			return List.Add(column);
		}
		public TreeListColumn Add() {
			return AddField(string.Empty);
		}
		protected internal virtual TreeListColumn Add(DataColumnInfo colInfo) {
			if(colInfo == null) return Add();
			TreeListColumn column = CreateColumn();
			column.FieldName = colInfo.ColumnName;
			List.Add(column);
			return column;
		}
		public TreeListColumn Insert(int index) {
			if(index < 0) index = 0;
			if(index >= Count) return Add();
			TreeListColumn column = CreateColumn();
			ClearAbsoluteIndexes();
			List.Insert(index, column);
			return column;
		}
		public virtual void Remove(TreeListColumn column) {
			List.Remove(column);
		}
		public int IndexOf(TreeListColumn column) { return List.IndexOf(column); }
		public virtual void AddRange(TreeListColumn[] columns) {
			BeginUpdate();
			try {
				for (IEnumerator e = columns.GetEnumerator(); e.MoveNext();) {
					TreeListColumn col = (TreeListColumn)e.Current;
					List.Add(e.Current);
				}
			}
			finally {
				EndUpdate();
			}
		}
		public virtual TreeListColumn ColumnByName(string columnName) {
			TreeListColumn result = null;
			for(int i = 0; i < Count; i++) {
				result = this[i];
				if(result.Name == columnName) return result;
			}
			return null;
		}
		public virtual TreeListColumn ColumnByFieldName(string fieldName) {
			TreeListColumn result = null;
			for(int i = 0; i < Count; i++) {
				result = this[i];
				if(result.FieldName == fieldName) return result;
			}
			return null;
		}
		#endregion public
		#region protected
		protected virtual TreeListColumn CreateColumn() { return new TreeListColumn(); }
		protected virtual void BeginUpdate() { this.updateCounter++; }
		protected virtual void EndUpdate() {
			if(--this.updateCounter == 0)
				Changed(CollectionChangeAction.Refresh, null);
		}
		protected override void OnInsertComplete(int index, object value) {
			TreeListColumn source  = value as TreeListColumn;
			SetOwner(source, this);
			NotifyColumnChanged(source);
			Changed(CollectionChangeAction.Add, source);
		}
		protected override void OnClear() {
			this.isClearing = true;
			if(TreeList != null) 
				TreeList.BeginUpdate();
			try {
				for(int i = List.Count - 1; i >= 0; i--)
					this[i].Dispose();
			}
			finally {
				this.isClearing = false;
				if(TreeList != null) {
					if(TreeList.Data != null)
						TreeList.Data.OnClearColumns();
					TreeList.EndUpdate();
				}
			}
		}
		protected override void OnRemoveComplete(int index, object value) {
			TreeListColumn source  = value as TreeListColumn;
			SetOwner(source, null);
			ClearAbsoluteIndexes();
			NotifyColumnRemoving(source);
			NotifyColumnChanged(null);
			Changed(CollectionChangeAction.Remove, source);
		}
		void NotifyColumnRemoving(TreeListColumn column) {
			if(TreeList != null)
				TreeList.InternalColumnRemoving(column);
		}
		void NotifyColumnChanged(TreeListColumn column) {
			if(TreeList != null)
				TreeList.InternalColumnChanged(column);
		}
		void SetOwner(TreeListColumn column, TreeListColumnCollection owner) {
			column.columns = owner;
		}
		protected internal virtual void SetItemIndex(TreeListColumn column, int value) {
			if(value < 0) value = 0;
			if(value > Count) value = Count;
			int prevIndex = IndexOf(column);
			if(prevIndex < 0 || prevIndex == value) return;
			InnerList.RemoveAt(prevIndex);
			if(value > Count) value = Count;
			InnerList.Insert(value, column);
			ClearAbsoluteIndexes();
			Changed(CollectionChangeAction.Refresh, null);
		}
		protected virtual void ClearAbsoluteIndexes() {
			foreach(TreeListColumn col in this) {
				col.absoluteIndex = -1;
			}
		}
		protected internal virtual void Changed(CollectionChangeAction action, TreeListColumn column) {
			if(IsLockUpdate) return;
			if(column == null) {
				ClearAbsoluteIndexes();
			}
			if(CollectionChanged != null)
				CollectionChanged(this, new CollectionChangeEventArgs(action, column));
		}
		protected internal void FireChanged() {
			for(int i = 0; i < Count; i++)
				this[i].FireChanged();
		}
		protected bool IsLockUpdate { get { return (updateCounter != 0); } }
		#endregion protected
		IEnumerator<TreeListColumn> IEnumerable<TreeListColumn>.GetEnumerator() {
			foreach(TreeListColumn column in List)
				yield return column;
		}
		#region IHeaderObjectCollection<TreeListColumn> Members
		void IHeaderObjectCollection<TreeListColumn>.Synchronize(IEnumerable<TreeListColumn> sourceCollection) {
			TreeListColumn[] columns = new TreeListColumn[InnerList.Count];
			InnerList.CopyTo(columns);
			InnerList.Clear();
			foreach(TreeListColumn column in sourceCollection) {
				TreeListColumn targetColumn = null;
				if(column.Name != "") 
					targetColumn = FindColumn(column.Name, columns);
				if(targetColumn == null) 
					targetColumn = Add();
				else 
					InnerList.Add(targetColumn);
				column.AssignTo(targetColumn);
			}
			if(TreeList != null)
				TreeList.RefreshColumnHandles();
		}
		TreeListColumn FindColumn(string name, ICollection<TreeListColumn> collection) {
			foreach(TreeListColumn column in collection) 
				if(column.Name == name) return column;
			return null;
		}
		#endregion
	}
	#endregion class TreeListColumns
	#region class TreeList Customization
	[ToolboxItem(false)]
	public class TreeListCustomizationForm : CustomizationFormBase {
		protected internal static Rectangle CheckCustomizationFormBounds(Rectangle value) {
			if (value == Rectangle.Empty) return value;
			value.Width = Math.Max(MinFormSize.Width, value.Width);
			value.Height = Math.Max(MinFormSize.Height, value.Height);
			value.Location = DevExpress.Utils.ControlUtils.CalcLocation(value.Location, value.Location, value.Size);
			return value;
		}
		TreeList treeList;
		TreeListHandler handler;
		public TreeListCustomizationForm(TreeList treeList, TreeListHandler handler) {
			this.treeList = treeList;
			this.handler = handler;
		}
		protected override string FormCaption {
			get { return TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.ColumnCustomizationText); }
		}
		public override Control ControlOwner { get { return TreeList; } }
		protected override CustomizationListBoxBase CreateCustomizationListBox() {
			return new TreeListCustomizationListBox(this); 
		}
		protected override Rectangle CustomizationFormBounds {
			get { return TreeList.CustomizationFormBounds; }
		}
		protected override DevExpress.LookAndFeel.UserLookAndFeel ControlOwnerLookAndFeel {
			get { return TreeList.ElementsLookAndFeel; }
		}
		internal ColumnInfo GetColumnInfoByPoint(Point pt) {
			pt = ActiveListBox.PointToClient(PointToScreen(pt));
			return (ActiveListBox as TreeListCustomizationListBox).GetColumnInfoByPoint(pt);
		}
		protected override bool AllowSearchBox { get { return TreeList.OptionsCustomization.CustomizationFormSearchBoxVisible; } }
		public TreeList TreeList { get { return treeList; } }
		internal TreeListHandler Handler { get { return handler; } }
	}
	[ToolboxItem(false)]
	public class TreeListCustomizationListBox : CustomCustomizationListBoxBase, IToolTipControlClient {
		public TreeListCustomizationListBox(TreeListCustomizationForm form) : base(form) {
			TreeList.GetToolTipController().AddClientControl(this);
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				TreeList.GetToolTipController().RemoveClientControl(this);
			base.Dispose(disposing);
		}
		public new TreeListCustomizationForm CustomizationForm { get { return base.CustomizationForm as TreeListCustomizationForm; } }
		protected TreeList TreeList { get { return CustomizationForm.TreeList; } }
		protected TreeListHandler TreeHandler { get { return CustomizationForm.Handler; } }
		protected TreeListPainter TreePainter { get { return TreeList.Painter; } }
		protected override void OnMouseDown(MouseEventArgs e) {
			TreeList.CloseEditor();
			if(!e.Button.HasLeft() && CustomizationForm.PressedItem != null) {
				TreeHandler.OnMouseDown(GetTreeListMouseArgs(e));
				return;
			}
			if(e.Button.HasLeft())
				TreeList.Capture = true;
			base.OnMouseDown(e);
			if(CustomizationForm.PressedItem != null) 
				TreeHandler.OnMouseDown(GetTreeListMouseArgs(e));
		}
		protected TreeListColumn PressedColumn { 
			get { return CustomizationForm.PressedItem as TreeListColumn; }
			set { CustomizationForm.PressedItem = value; }
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			TreeHandler.OnMouseMove(GetTreeListMouseArgs(e));
			base.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			TreeHandler.OnMouseUp(GetTreeListMouseArgs(e));
			base.OnMouseUp(e);
			CustomizationForm.PressedItem = null;
			TreeList.Capture = false;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyData == Keys.Escape) {
				TreeHandler.OnKeyDown(e);
				TreeList.CloseEditor();
				TreeList.Capture = false;
			}
		}
		public override int GetItemHeight() { return TreeList.ViewInfo.GetColumnPanelHeight(); ; }
		protected override void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) {
			ColumnInfo ci = GetColumnInfo(index);
			TreePainter.DrawColumn(cache, ci);
		}
		protected override IComparer CreateComparer() {
			return new ColumnComparer();
		}
		protected override void AddItems(ArrayList list) {
			foreach (TreeListColumn column in TreeList.Columns) {
				if((column.VisibleIndex < 0 || (TreeList.ActualShowBands && column.ParentBand == null)) && column.OptionsColumn.ShowInCustomizationForm ) {
					list.Add(column);
				}
			}
		}
		public ColumnInfo GetColumnInfoByPoint(Point pt) {
			int index = IndexFromPoint(pt);
			if (index == -1) return null;
			return GetColumnInfo(index);
		}
		ColumnInfo GetColumnInfo(int index) {
			TreeListColumn column = GetItemValue(index) as TreeListColumn;
			ColumnInfo ci = TreeList.ViewInfo.CreateColumnInfo(column);
			TreeList.ViewInfo.CalcColumnInfo(ci, ViewInfo.GetItemRectangle(index), true);
			ci.Pressed = (column == PressedColumn);
			return ci;
		}
		protected MouseEventArgs GetTreeListMouseArgs(MouseEventArgs e) {
			Point pt = PointToView(new Point(e.X, e.Y));
			return new MouseEventArgs(e.Button, e.Clicks, pt.X, pt.Y, e.Delta);
		}
		class ColumnComparer : IComparer {
			int IComparer.Compare(object x1, object y1) {
				TreeListColumn x = x1 as TreeListColumn;
				TreeListColumn y = y1 as TreeListColumn;
				int res = Comparer.Default.Compare(x.GetCustomizationCaption(), y.GetCustomizationCaption());
				if (res == 0) res = Comparer.Default.Compare(x.AbsoluteIndex, y.AbsoluteIndex);
				return res;
			}
		}
		bool IToolTipControlClient.ShowToolTips { get { return ((IToolTipControlClient)TreeList).ShowToolTips; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) { return ((IToolTipControlClient)TreeList).GetObjectInfo(PointToView(point)); }
		protected override string GetHintCaptionForEmptyList() {
			return TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.CustomizationFormColumnHint);
		}
		protected override AppearanceObject GetHintForEmptyListAppearance() {
			return TreeList.ViewInfo.PaintAppearance.CustomizationFormHint;
		}
		protected override void DoShowItem(object item) {
			base.DoShowItem(item);
			TreeListColumn col = item as TreeListColumn;
			if(col == null) return;
			col.Visible = true;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
	}
	#endregion
}
