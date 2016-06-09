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
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Windows.Forms.ComponentModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Drawing;
using System.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Design;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Design;
using System.Globalization;
using DevExpress.Utils.Serializing;
using DevExpress.Data.Filtering;
using System.Text;
using System.Collections.Generic;
using DevExpress.LookAndFeel;
using DevExpress.XtraGrid.Internal;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Columns {
	public enum AutoFilterCondition { Default, Like, Equals, Contains }
	public enum FilterPopupMode { Default, List, CheckedList, Date, DateSmart, DateAlt }
	#region ColumnHelpers (ColumnOptions, ColumnFilterType, ColumnFilterInfo)
	public enum ColumnFilterKind { User, Predefined };
	public enum ColumnFilterType { None, Custom, Value, AutoFilter};
	internal class ColumnFilterInfoTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type type) {
			if(type == null || type.Equals(typeof(string))) return false;
			return base.CanConvertFrom(context, type);
		}
	}
	public class ColumnFilterInfoCollection : CollectionBase, IEnumerable<ColumnFilterInfo> {
		public ColumnFilterInfo this[int index] { get { return List[index] as ColumnFilterInfo; } }
		public int Add(ColumnFilterInfo info) {
			if(!CanAdd(info)) return -1;
			return List.Add(info);
		}
		public void Insert(int index, ColumnFilterInfo info) { 
			if(!CanAdd(info)) return;
			List.Insert(index, info);
		}
		protected internal void CheckCount(int maxCount) {
			maxCount = Math.Max(0, maxCount);
			while(Count > maxCount) RemoveAt(Count - 1);
		}
		protected virtual bool CanAdd(ColumnFilterInfo info) {
			return info != null && info.Type != ColumnFilterType.None;
		}
		internal void InsertMRU(ColumnFilterInfo info, int maxCount) {
			if(!CanAdd(info)) return;
			if(info.Kind == ColumnFilterKind.Predefined || info.Type == ColumnFilterType.AutoFilter) return;
			int index = Find(info);
			if(index != -1) RemoveAt(index);
			Insert(0, info.Clone() as ColumnFilterInfo);
			CheckCount(maxCount);
		}
		internal int Find(ColumnFilterInfo info) {
			for(int n = 0; n < Count; n++) {
				if(this[n].Equals(info)) return n;
			}
			return -1;
		}
		internal bool Contains(ColumnFilterInfo info) {
			return List.Contains(info);
		}
		internal void Remove(ColumnFilterInfo info) {
			if(Contains(info)) List.Remove(info);
		}
		IEnumerator<ColumnFilterInfo> IEnumerable<ColumnFilterInfo>.GetEnumerator() {
			foreach(ColumnFilterInfo filterInfo in InnerList)
				yield return filterInfo;
		}
	}
	[Serializable, TypeConverter(typeof(DevExpress.Utils.Design.BinaryTypeConverter))]
	public class ColumnFilterInfo : ISerializable {
		static ColumnFilterInfo empty = null;
#if !SL
	[DevExpressXtraGridLocalizedDescription("ColumnFilterInfoEmpty")]
#endif
		public static ColumnFilterInfo Empty {
			get {
				if(empty == null) empty = new ColumnFilterInfo();
				return empty;
			}
		}
		ColumnFilterType type;
		object _value;
		CriteriaOperator fFilterCriteria;
		string displayText;
		ColumnFilterKind kind = ColumnFilterKind.User;
		public ColumnFilterInfo(ColumnFilterType type, object _value, CriteriaOperator filter, string displayText) {
			this.type = type;
			this._value = _value;
			this.fFilterCriteria = filter;
			this.displayText = displayText;
		}
		public ColumnFilterInfo(CriteriaOperator filter) : this(ColumnFilterType.Custom, null, filter, string.Empty) { }
		public ColumnFilterInfo(ColumnFilterType type, object _value, string filterString, string displayText)
			: this(type, _value, CriteriaOperator.TryParse(filterString), displayText) { }
		public ColumnFilterInfo(ColumnFilterType type, object _value, string filterString) : this(type, _value, filterString, string.Empty) { }
		public ColumnFilterInfo() : this(ColumnFilterType.None, null, (CriteriaOperator)null, string.Empty) { }
		public ColumnFilterInfo(string filterString, object _value) : this(ColumnFilterType.Value, _value, filterString, string.Empty) { }
		public ColumnFilterInfo(ColumnFilterType type, string filterString) : this(type, null, filterString, string.Empty) { }
		public ColumnFilterInfo(string filterString) : this(ColumnFilterType.Custom, null, filterString, string.Empty) { }
		public ColumnFilterInfo(GridColumn column, object _value, string displayText)
			: this(ColumnFilterType.Value, _value, (CriteriaOperator)null, displayText) {
			UpdateValueFilterIfNeeded(column);
		}
		public ColumnFilterInfo(GridColumn column, object _value) : this(column, _value, string.Empty) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(object _value) : this((GridColumn)null, _value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(byte _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(byte[] _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(char _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(DateTime _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(decimal _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(double _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(float _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(Guid _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(int _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(long _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(short _value) : this((object)_value) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(TimeSpan _value) : this((object)_value) { }
		public ColumnFilterInfo(string filterString, string displayText, object val, ColumnFilterType filterType) : this(filterType, val, filterString, displayText) { }
		[Obsolete("Use the ColumnFilterInfo(GridColumn column, object _value, string displayText) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(object _value, string displayText) : this((GridColumn)null, _value, displayText) { }
		public ColumnFilterInfo(string filterString, string displayText) : this(ColumnFilterType.Custom, null, filterString, displayText) { }
		[Obsolete("Use the ColumnFilterInfo(ColumnFilterType type, object _value, string filterString) instead"), EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnFilterInfo(ColumnFilterType type, object _value, string filterString, string displayText, string valueDisplayText) : this(type, _value, filterString, displayText) { }
		public bool Equals(ColumnFilterInfo filter) {
			if(filter.Type != Type || !Equals(filter.FilterCriteria, FilterCriteria) || filter.DisplayText != DisplayText ) return false;
			if(filter.Value == Value || object.Equals(filter.Value, Value)) return true;
			return false;
		}
		[Browsable(false)]
		public ColumnFilterKind Kind { get { return kind; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoFilterString"),
#endif
 XtraSerializableProperty()]
		public string FilterString {
			get {
				return CriteriaOperator.ToString(FilterCriteria);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoFilterCriteria"),
#endif
XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public CriteriaOperator FilterCriteria {
			get { return fFilterCriteria; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoDisplayText"),
#endif
 XtraSerializableProperty()]
		public string DisplayText {
			get { return displayText; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoValueDisplayText"),
#endif
 XtraSerializableProperty()]
		[Obsolete("Use ColumnView.CustomFilterDisplayText instead.")]
		public string ValueDisplayText {
			get { return string.Empty; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoType"),
#endif
 XtraSerializableProperty()]
		public ColumnFilterType Type { get { return type; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("ColumnFilterInfoValue"),
#endif
 XtraSerializableProperty()]
		public object Value { get { return _value; } }
		public virtual object Clone() {
			ColumnFilterInfo res = new ColumnFilterInfo(Type, Value, CriteriaOperator.Clone(FilterCriteria), DisplayText);
			res.kind = this.kind;
			return res;
		}
		[Obsolete("Use ColumnView.CustomFilterDisplayText instead.")]
		public virtual string GetDisplayText() {
			if(DisplayText == "") return FilterString;
			return DisplayText; 
		}
		[Obsolete("Use ColumnView.CustomFilterDisplayText instead.")]
		public string GetValueDisplayText() {
			if(ValueDisplayText == string.Empty) return GetDisplayText();
			return ValueDisplayText;
		}
		protected internal void SetFilterKind(ColumnFilterKind kind) { this.kind = kind; }
		protected internal void SetFilterString(string val) {
			this.fFilterCriteria = CriteriaOperator.TryParse(val);
		}
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			ColumnFilterType type = Type;
			object val = Value;
			if(Type == ColumnFilterType.Value) {
				type = ColumnFilterType.Custom;
				val = null;
			}
			si.AddValue("Type", (int)type);
			if(Type == ColumnFilterType.None) return;
			si.AddValue("Kind", (int)kind);
			si.AddValue("DisplayText", DisplayText);
			si.AddValue("FilterString", FilterString);
			si.AddValue("Value", val);
			si.AssemblyName = this.GetType().Assembly.GetName().Name;
		}
		internal ColumnFilterInfo(SerializationInfo si, StreamingContext context) : this() {
			foreach (SerializationEntry entry in si) {
				switch (entry.Name) {
					case "Type":
						this.type = (ColumnFilterType)entry.Value;
						break;
					case "DisplayText":
						this.displayText = (string)entry.Value;
						break;
					case "FilterString":
						SetFilterString((string)entry.Value);
						break;
					case "Kind":
						this.kind = (ColumnFilterKind)entry.Value;
						break;
					case "Value":
						this._value = entry.Value;
						break;
				}
			}
		}
		protected internal void UpdateValueFilterIfNeeded(GridColumn column) {
			if(column == null)
				return;
			if(column.View == null)
				return;
			if(this.Type != ColumnFilterType.Value)
				return;
			if(!ReferenceEquals(FilterCriteria, null))
				return;
			this.fFilterCriteria = column.View.DataController.CalcColumnFilterCriteriaByValue(column.FilterFieldName, this.Value, true, column.View.IsRoundDateTime(column), null);
		}
	}
	#endregion ColumnHelpers (ColumnOptions, ColumnFilterType, ColumnFilterInfo)
	public class GridColumnConverter : ComponentConverter {
		public GridColumnConverter(Type type) : base(type) { }
	}
	public enum FixedStyle { None, Left, Right };
	[DesignTimeVisible(false), ToolboxItem(false), TypeConverter(typeof(GridColumnConverter)),
	Designer("DevExpress.XtraGrid.Design.GridColumnDesigner, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.ComponentModel.Design.IDesigner))]
	public class GridColumn : Component, IAppearanceOwner, IXtraSerializableLayoutEx, ISupportLookAndFeel, IDataColumnInfoProvider {
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3;
		UnboundColumnType unboundType;
		internal bool grouped = false;
		protected internal const int defaultColumnWidth = 75;
		string unboundExpression;
		HorzAlignment defaultValueAlignment;
		FormatInfo displayFormat, groupFormat;
		FixedStyle _fixed;
		object tag;
		bool widthLocked, visible, showUnboundExpressionMenu, allowSummaryMenu;
		string caption, styleName, name, headerStyleName, toolTip, customizationCaption;
		int minWidth, columnHandle, imageIndex, maxWidth;
		Image image;
		StringAlignment imageAlignment;
		int visibleIndex;
		internal int absoluteIndex,
			sortIndex, visibleWidth, prevVisibleIndex = -1;
		int groupIndex;
		internal int width;
		internal ColumnSortOrder sortOrder;
		internal bool sortedBeforeGrouping;
		internal GridColumnCollection columns;
		string fieldName, fieldNameSortGroup;
		protected DevExpress.XtraEditors.Repository.RepositoryItem fColumnEdit;
		protected string fColumnEditName;
		int filterPopupWidth = 0;
		Size checkedFilterPopupSize = Size.Empty;
		ShowButtonModeEnum showButtonMode;
		AppearanceObjectEx appearanceCell;
		AppearanceObject appearanceHeader;
		ColumnFilterInfoCollection mruFilters;
		ColumnSortMode sortMode;
		ColumnGroupInterval filterPopupInterval;
		ColumnGroupInterval groupInterval;
		OptionsColumn optionsColumn;
		OptionsColumnFilter optionsFilter;
		OptionsColumnEditForm optionsEditForm;
		ColumnFilterMode filterMode;
		GridColumnSummaryItemCollection summary;
		public GridColumn() {
			this.unboundExpression = string.Empty;
			this.unboundType = UnboundColumnType.Bound;
			this.optionsColumn = CreateOptionsColumn();
			this.optionsColumn.Changed += new BaseOptionChangedEventHandler(OnOptionsChanged);
			this.optionsFilter = CreateOptionsFilter();
			this.optionsFilter.Changed += new BaseOptionChangedEventHandler(OnOptionsChanged);
			this.optionsEditForm = CreateOptionsEditForm();
			this.optionsEditForm.Changed += new BaseOptionChangedEventHandler(OnOptionsChanged);
			this.filterPopupInterval = ColumnGroupInterval.Default;
			this.filterMode = ColumnFilterMode.Value;
			this.groupInterval = ColumnGroupInterval.Default;
			this.sortMode = ColumnSortMode.Default;
			this.mruFilters = new ColumnFilterInfoCollection();
			this.appearanceCell = CreateCellAppearance(); 
			this.appearanceHeader = CreateHeaderAppearance();
			this.appearanceCell.Changed += new EventHandler(OnAppearanceChanged);
			this.appearanceHeader.Changed += new EventHandler(OnAppearanceChanged);
			this.defaultValueAlignment = HorzAlignment.Default;
			this.displayFormat = new GridColumnFormatInfo(new GridColumnFormatInfo.AllowParseEventHandler(OnFormat_AllowParse));
			this.displayFormat.Changed += new EventHandler(OnFormat_Changed);
			this.groupFormat = new GridColumnFormatInfo(new GridColumnFormatInfo.AllowParseEventHandler(OnFormat_AllowParse));
			this.groupFormat.Changed += new EventHandler(OnFormat_Changed);
			this.showButtonMode = ShowButtonModeEnum.Default;
			this.widthLocked = false;
			this.imageAlignment = StringAlignment.Near;
			this.imageIndex = -1;
			this.image = null;
			this.maxWidth = 0;
			this.minWidth = 20;
			this.fColumnEditName = this.name = string.Empty;
			this.summary = new GridColumnSummaryItemCollection(this);
			this.summary.CollectionChanged += new CollectionChangeEventHandler(Summary_CollectionChanged);
			this.headerStyleName = this.styleName = string.Empty;
			this.fColumnEdit = null;
			this.sortOrder = ColumnSortOrder.None;
			this.sortedBeforeGrouping = false;
			this.fieldNameSortGroup = this.fieldName = "";
			this.customizationCaption = this.toolTip = this.caption = string.Empty;
			this.visibleIndex = this.absoluteIndex = -1;
			this.visible = false;
			this.columns = null;
			this.columnHandle = -1;
			this.sortIndex = this.groupIndex = -1;
			this.visibleWidth = this.width = defaultColumnWidth;
			this._fixed = FixedStyle.None;
			this.showUnboundExpressionMenu = false;
			this.allowSummaryMenu = true;
		}
		protected virtual AppearanceObject CreateHeaderAppearance() { return new AppearanceObject(this, false); }
		protected virtual AppearanceObjectEx CreateCellAppearance() { return new AppearanceObjectEx(this); }
		void Summary_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(View != null) View.OnColumnSummaryCollectionChanged(this, e);
		}
		#region public
		internal object XtraCreateSummaryItem(XtraItemEventArgs e) { return Summary.XtraCreateSummaryItem(e); }
		internal void XtraSetIndexSummaryItem(XtraSetItemIndexEventArgs e) { Summary.XtraSetIndexSummaryItem(e); }
		internal object XtraFindSummaryItem(XtraItemEventArgs e) { return Summary.XtraFindSummaryItem(e); }
		internal bool ShouldSerializeSummaryItem() {
			if(Summary.Count == 0) return false;
			return SummaryItem.SummaryType != SummaryItemType.None; 
		}
		protected internal bool HasSummary { get { return Summary.Count > 0 && Summary.ActiveCount > 0; } }
		bool XtraShouldSerializeSummaryItem() { return false; }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnSummaryItem"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Content), XtraSerializablePropertyId(LayoutIdData)]
		public GridSummaryItem SummaryItem {
			get {
				if(Summary.Count == 0) {
					Summary.Add();
				}
				return Summary[0];
			}
		}
		internal bool XtraShouldSerializeSummary() { return ShouldSerializeSummary(); }
		internal bool ShouldSerializeSummary() {
			if(Summary.Count == 0 || (Summary.ActiveCount == 0 & Summary.Count == 1)) return false;
			return true;
		}
		[Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(true, true, true, 1000), XtraSerializablePropertyId(LayoutIdData), DXCategory(CategoryName.Data)]
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSummary")]
#endif
		public GridColumnSummaryItemCollection Summary { get { return summary; } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(-1)]
		public virtual string Name {
			get { 
				if(View != null && View.IsDeserializing) return name;
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null) value = string.Empty;
				name = value;
				if(Site != null) Site.Name = name;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnShowButtonMode"),
#endif
 DefaultValue(ShowButtonModeEnum.Default), XtraSerializableProperty(), DXCategory(CategoryName.Appearance)]
		public ShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(ShowButtonMode == value) return;
				showButtonMode = value;
				OnChanged();
			}
		}
		internal bool ShouldSerializeOptionsColumn() { return OptionsColumn.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnOptionsColumn"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsColumn OptionsColumn { get { return optionsColumn; } }
		internal bool ShouldSerializeOptionsFilter() { return OptionsFilter.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnOptionsFilter"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsColumnFilter OptionsFilter { get { return optionsFilter; } }
		internal bool ShouldSerializeOptionsEditForm() { return OptionsEditForm.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnOptionsEditForm"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		public OptionsColumnEditForm OptionsEditForm { get { return optionsEditForm; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnToolTip"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string ToolTip {
			get { return toolTip; }
			set {
				if(ToolTip == value) return;
				toolTip = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(value == null) value = string.Empty;
				if(Caption != value) {
					caption = value;
					cachedTextCaption = null;
					OnChanged();
				}
			}
		}
		public virtual string GetCaption() {
			string caption = Caption;
			if(View == null) 
				return caption;
			if(string.IsNullOrEmpty(caption))
				caption = GetCaptionFromColumnAnnotation();
			if(string.IsNullOrEmpty(caption)) {
				if(ColumnInfo != null)
					caption = ColumnInfo.Caption;
				else
					caption = MasterDetailHelper.SplitPascalCaseString(FieldName);
			}
			return caption;
		}
		public virtual string GetDescription() {
			return GetDescriptionFromColumnAnnotation();
		}
		public override string ToString() {
			return GetTextCaption();
		}
		string cachedTextCaption = null;
		public virtual string GetTextCaption() {
			if(View == null) return Caption;
			if(cachedTextCaption != null) return cachedTextCaption;
			cachedTextCaption = View.GetNonFormattedCaption(GetCaption());
			cachedTextCaption = cachedTextCaption.Replace('\r', ' ').Replace("\n", "");
			return cachedTextCaption;
		}
		protected internal virtual string GetTextCaptionForPrinting() {
			if(View == null) return Caption;
			var gview = View.OptionsPrint as GridOptionsPrint;
			if(gview != null && !gview.AllowMultilineHeaders) return GetTextCaption();
			return View.GetNonFormattedCaption(GetCaption());
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnCustomizationCaption"),
#endif
 DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		public string CustomizationCaption {
			get { return customizationCaption; }
			set {
				if(value == null) value = string.Empty;
				if(CustomizationCaption != value) {
					customizationCaption = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnImageAlignment"),
#endif
 DefaultValue(StringAlignment.Near), XtraSerializableProperty(), Localizable(true)]
		public virtual StringAlignment ImageAlignment {
			get { return imageAlignment; }
			set {
				if(ImageAlignment != value) {
					imageAlignment = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnImageIndex"),
#endif
 DefaultValue(-1),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.ImageList("Images"), XtraSerializableProperty(), Localizable(true)]
		public virtual int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex != value) {
					imageIndex = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnImage"),
#endif
 DefaultValue(null),
		Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual Image Image {
			get { return image; }
			set {
				if(Image != value) {
					image = value;
					OnChanged();
				}
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnFieldName"),
#endif
 DefaultValue(""), XtraSerializableProperty(),
		Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = "";
				if(value == FieldName) return;
				if(View != null)
					View.SetColumnFieldName(this, value);
				else
					SetFieldNameCore(value);
				if(!IsLoading && UnboundType != UnboundColumnType.Bound) View.OnColumnUnboundChanged(this);
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnFieldNameSortGroup"),
#endif
 DefaultValue(""), XtraSerializableProperty(),
		Editor("DevExpress.XtraGrid.Design.GridColumnNameEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.FieldNameTypeConverter, " + AssemblyInfo.SRAssemblyGridDesign)]
		public string FieldNameSortGroup {
			get { return fieldNameSortGroup; }
			set {
				if(value == null) value = "";
				if(value == FieldNameSortGroup) return;
				fieldNameSortGroup = value;
				OnChanged();
			}
		}
		protected internal string GetCustomizationCaption() {
			if(CustomizationCaption == string.Empty) return GetCaption();
			return CustomizationCaption;
		}
		internal void SetFieldNameCore(string newName) {
			if(this.columns != null) {
				columns.ChangeFieldName(this, FieldName, newName);
			}
			this.fieldName = newName;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnFixed"),
#endif
 DefaultValue(FixedStyle.None), XtraSerializableProperty(), Localizable(true)]
		public virtual FixedStyle Fixed {
			get { return _fixed; }
			set {
				if(Fixed == value) return;
				_fixed = value;
				if(View != null) {
					View.SetColumnFixedStyle(this, value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object Images {
			get { return View == null ? null : View.Images; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public virtual string ColumnEditName {
			get { 
				if(ColumnEdit != null) return ColumnEdit.Name;
				return "";
			}
			set {
				if(value == ColumnEditName) return;
				if(View == null || View.GridControl == null) return;
				DevExpress.XtraEditors.Repository.RepositoryItem ri = View.GridControl.EditorHelper.FindRepositoryItem(value);
				if(ri != null) ColumnEdit = ri;
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnColumnEdit"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null),
		TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnEditConverter, " + AssemblyInfo.SRAssemblyGridDesign),
		Editor("DevExpress.XtraGrid.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public DevExpress.XtraEditors.Repository.RepositoryItem ColumnEdit {
			get { return fColumnEdit; }
			set {
				if(ColumnEdit != value) {
					DevExpress.XtraEditors.Repository.RepositoryItem old = fColumnEdit;
					fColumnEdit = value;
					if(old != null) old.Disconnect(this);
					this.defaultValueAlignment = HorzAlignment.Default;
					if(ColumnEdit != null) {
						ColumnEdit.Connect(this);
					}
					if(View == null) return;
					OnChanged();
				}
			}
		}
		internal bool ColumnEditIsSparkLine {
			get {
				return RealColumnEdit != null && RealColumnEdit.GetType().Name.IndexOf("Sparkline") > -1;
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnVisibleIndex"),
#endif
 DefaultValue(-1), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual int VisibleIndex {
			get { return !Visible ? -1 : visibleIndex; }
			set {
				if(IsLoading) {
					visibleIndex = value;
					this.visible = this.visibleIndex >= 0;
					return;
				}
				View.SetColumnVisibleIndex(this, value);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnTag"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public virtual object Tag {
			get { return tag; }
			set { tag = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnVisible"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(false), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual bool Visible {
			get { return visible; }
			set {
				if(IsLoading) {
					visible = value;
					visibleIndex = visibleIndex < 0 ? 100000 : visibleIndex;
					return;
				}
				if(Visible == value) return;
				this.visible = value;
				if(visibleIndex > -1) prevVisibleIndex = visibleIndex;
				if(value)
					this.visibleIndex = prevVisibleIndex > -1 ? prevVisibleIndex : View.VisibleColumns.Count;
				else
					this.visibleIndex = -1;
				View.SetColumnVisibleIndex(this, this.visibleIndex);
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
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnGroupIndex"),
#endif
 DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializablePropertyId(LayoutIdData)]
		public int GroupIndex {
			get { return groupIndex; }
			set {
				if(value < 0) value = -1;
				if(IsLoading) {
					groupIndex = value;
					return;
				}
				this.grouped = GroupIndex > -1;
				View.DoColumnChangeGroupIndex(this, value);
			}
		}
		internal void SetGroupIndexCore(int groupIndex) {
			this.groupIndex = groupIndex;
		}
		internal void SetGroupIndex(int groupIndex) {
			this.grouped = GroupIndex > -1;
			this.groupIndex = groupIndex;
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnSortMode"),
#endif
 DefaultValue(ColumnSortMode.Default), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdData)]
		public ColumnSortMode SortMode {
			get { return sortMode; }
			set {
				if(SortMode == value) return;
				sortMode = value;
				if(View != null)
					View.OnSortGroupPropertyChanged();
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnFilterMode"),
#endif
 DefaultValue(ColumnFilterMode.Value), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdData)]
		public ColumnFilterMode FilterMode {
			get { return filterMode; }
			set {
				if(FilterMode == value) return;
				filterMode = value;
				if(View != null)
					View.OnFilterModeChanged();
			}
		}
		[DXCategory(CategoryName.Data),  DefaultValue(ColumnGroupInterval.Default), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdData)]
		internal ColumnGroupInterval FilterPopupInterval { 
			get { return filterPopupInterval; }
			set {
				if(FilterPopupInterval == value) return;
				filterPopupInterval = value;
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnGroupInterval"),
#endif
 DefaultValue(ColumnGroupInterval.Default), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdData)]
		public ColumnGroupInterval GroupInterval {
			get { return groupInterval; }
			set {
				if(GroupInterval == value) return;
				groupInterval = value;
				if(View != null)
					View.OnSortGroupPropertyChanged();
			}
		}
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnSortOrder"),
#endif
 DefaultValue(ColumnSortOrder.None), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializablePropertyId(LayoutIdData)]
		public ColumnSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(IsLoading) {
					sortOrder = value;
					return;
				}
				View.DoColumnChangeSortOrder(this, value);
			}
		}
		[DefaultValue(-1), Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), 
		XtraSerializablePropertyId(LayoutIdData)]
		public int SortIndex { 
			get { return sortIndex; }
			set {
				if(value < 0) value = -1;
				if(IsLoading) {
					this.sortIndex = value;
					return;
				}
				View.DoColumnChangeSortIndex(this, value);
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnWidth"),
#endif
 DefaultValue(defaultColumnWidth), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout), Localizable(true)]
		public virtual int Width {
			get { return width; }
			set {
				if(IsLoading || View == null) {
					width = value;
					return;
				}
				View.SetColumnWidth(this, value, false);
			}
		}
		public void Resize(int newWidth) {
			ResizeCore(newWidth, false);
		}
		protected internal void ResizeCore(int newWidth, bool force) {
			if(View == null || IsLoading) return;
			if(Width == newWidth && !force) return;
			View.SetColumnWidth(this, newWidth, force);
			View.OnColumnWidthChanged(this);
		}
		bool ShouldSerializeAppearanceCell() { return AppearanceCell.ShouldSerialize(); } 
		void ResetAppearanceCell() { AppearanceCell.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnAppearanceCell"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public AppearanceObjectEx AppearanceCell {
			get { return appearanceCell; }
		}
		bool ShouldSerializeAppearanceHeader() { return AppearanceHeader.ShouldSerialize(); } 
		void ResetAppearanceHeader() { AppearanceHeader.Reset(); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnAppearanceHeader"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public AppearanceObject AppearanceHeader {
			get { return appearanceHeader; }
		}
		internal ColumnView view = null;
		[Browsable(false)]
		public ColumnView View { 
			get {
				if(columns == null) return view;
				return columns.View;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AbsoluteIndex { 
			get { 
				if(absoluteIndex == -1)
					absoluteIndex = View.GetColumnAbsoluteIndex(this);
				return absoluteIndex; 
			}
			set { 
				View.SetColumnAbsoluteIndex(this, value); }
		}
		internal DataColumnInfo ColumnInfoSort {
			get {
				if(View == null || string.IsNullOrEmpty(FieldNameSortGroup)) return ColumnInfo;
				return View.DataController.Columns[FieldNameSortGroup];
			}
		}
		internal DataColumnInfo ColumnInfoFilter {
			get {
				if(View == null || string.IsNullOrEmpty(FilterFieldName)) return ColumnInfo;
				return View.DataController.Columns[FilterFieldName];
			}
		}
		internal DataColumnInfo ColumnInfo {
			get {
				if(View == null || ColumnHandle == -1 || ColumnHandle >= View.DataController.Columns.Count) return null;
				return View.DataController.Columns[columnHandle];
			}
		}
		protected internal virtual bool FilterBySortFieldName { get { return OptionsFilter.FilterBySortField == DefaultBoolean.True; } }
		protected internal string SortFieldName { get { return string.IsNullOrEmpty(FieldNameSortGroup) ? FieldName : FieldNameSortGroup; } }
		protected internal string FilterFieldName { get { return string.IsNullOrEmpty(FieldNameSortGroup) || !FilterBySortFieldName  ? FieldName : FieldNameSortGroup; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ColumnHandle { 
			get {
				if(columnHandle < 0) UpdateHandle();
				return columnHandle;	
			}
			set {
				if(View == null) return;
				if(value < 0 || value >= View.DataController.Columns.Count) value = -1;
				if(columnHandle != value) {
					this.defaultValueAlignment = HorzAlignment.Default;
					this.columnHandle = value;
					this.columnAnnotationAttributes = null;
					OnChanged();
				}
			}
		}
		internal void SetColumnHandle(int value) {
			if(this.columnHandle != value)
				this.defaultValueAlignment = HorzAlignment.Default;
			this.columnHandle = value;
			this.columnAnnotationAttributes = null;
		}
		bool XtraShouldSerializeDisplayFormat() { return ShouldSerializeDisplayFormat(); }
		bool ShouldSerializeDisplayFormat() { return !DisplayFormat.IsEmpty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnDisplayFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public FormatInfo DisplayFormat {
			get { return displayFormat; }
		}
		bool XtraShouldSerializeGroupFormat() { return ShouldSerializeGroupFormat(); }
		bool ShouldSerializeGroupFormat() { return !GroupFormat.IsEmpty; }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnGroupFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public FormatInfo GroupFormat {
			get { return groupFormat; }
		}
		[Browsable(false)]
		public RepositoryItem RealColumnEdit {
			get {
				var ce = ColumnEdit == null ? null : (ColumnEdit.IsDisposed ? null : ColumnEdit);
				if(View == null) return ce;
				if(ce != null) return ce;
				return View.GetColumnDefaultRepositoryItem(this);
			}
		}
		[Browsable(false)]
		public bool ReadOnly { 
			get { 
				DataColumnInfo col = View.DataController.Columns[ColumnHandle];
				return OptionsColumn.ReadOnly || (col != null && col.ReadOnly) || !View.DataController.AllowEdit;
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnMinWidth"),
#endif
 DefaultValue(20), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout)]
		public virtual int MinWidth {
			get { return minWidth; }
			set {
				if(value < 10) value = 10;
				if(value > 1000) value = 1000;
				if(value == MinWidth) return;
				minWidth = value;
				OnChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnMaxWidth"),
#endif
 DefaultValue(0), XtraSerializableProperty()]
		public virtual int MaxWidth {
			get { return maxWidth; }
			set {
				if(value < 0) value = 0;
				if(value == MaxWidth) return;
				maxWidth = value;
				OnChanged();
				if(Width > MaxWidth && MaxWidth > 0) Width = MaxWidth;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColumnFilterInfoCollection MRUFilters { get { return mruFilters; } }
		internal void UpdateFilterInfo(int popupWidth) {
			if(View != null && View.OptionsFilter.AllowColumnMRUFilterList) {
				ColumnFilterInfo filter = FilterInfo;
				MRUFilters.InsertMRU(filter, View.OptionsFilter.MRUColumnFilterListCount);
			}
			this.filterPopupWidth = popupWidth;
		}
		internal int FilterPopupWidth { get { return filterPopupWidth; } }
		internal void UpdateCheckedFilterInfo(Size popupSize) {
			this.checkedFilterPopupSize = popupSize;
		}
		internal Size CheckedFilterPopupSize { get { return checkedFilterPopupSize; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleWidth { get { return visibleWidth; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColumnFilterInfo FilterInfo {
			get { 
				if(View != null) {
					ViewColumnFilterInfo info = View.ActiveFilter[this];
					if(info != null) return info.Filter;
				}
				return ColumnFilterInfo.Empty;
			}
			set {
				if(value == null || View == null) return;
				View.ActiveFilter.Add(this, value);
			}
		}
		[Browsable(false)]
		public string SummaryText {
			get {
				if(Summary.Count == 0 || Summary.ActiveCount == 0) return string.Empty;
				StringBuilder sb = new StringBuilder();
				for(int n = 0; n < Summary.Count; n++) {
					GridColumnSummaryItem item = Summary[n];
					if(item.SummaryType == SummaryItemType.None) continue;
					if(sb.Length > 0) sb.AppendLine();
					sb.Append(item.GetDisplayText(item.SummaryValue, false));
				}
				return sb.ToString();
			}
		}
		[Browsable(false)]
		public virtual bool CanShowInCustomizationForm {
			get {
				if(View ==null) return false;
				return View.CanShowColumnInCustomizationForm(this);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnShowUnboundExpressionMenu"),
#endif
 DefaultValue(false), DXCategory(CategoryName.Data), XtraSerializableProperty()]
		public bool ShowUnboundExpressionMenu {
			get { return showUnboundExpressionMenu; }
			set {
				if(ShowUnboundExpressionMenu == value) return;
				showUnboundExpressionMenu = value;
				OnChanged();
			}
		}
		[Browsable(false), 
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnAllowSummaryMenu"),
#endif
 DefaultValue(true), DXCategory(CategoryName.Data), XtraSerializableProperty()]
		public bool AllowSummaryMenu {
			get { return allowSummaryMenu; }
			set {
				if(AllowSummaryMenu == value) return;
				allowSummaryMenu = value;
				OnChanged();
			}
		}
		public void ClearFilter() {
			FilterInfo = ColumnFilterInfo.Empty;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.optionsColumn.Changed -= new BaseOptionChangedEventHandler(OnOptionsChanged);
				this.optionsFilter.Changed -= new BaseOptionChangedEventHandler(OnOptionsChanged);
				this.appearanceCell.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearanceHeader.Changed -= new EventHandler(OnAppearanceChanged);
				if(DisplayFormat != null) {
					((GridColumnFormatInfo)this.displayFormat).AllowParse -= new GridColumnFormatInfo.AllowParseEventHandler(OnFormat_AllowParse);
					this.displayFormat.Changed -= new EventHandler(OnFormat_Changed);
				}
				if(GroupFormat != null) {
					((GridColumnFormatInfo)this.groupFormat).AllowParse -= new GridColumnFormatInfo.AllowParseEventHandler(OnFormat_AllowParse);
					this.groupFormat.Changed -= new EventHandler(OnFormat_Changed);
				}
				if(ColumnEdit != null) {
					ColumnEdit.Disconnect(this);
					this.fColumnEdit = null;
				}
				GridColumnCollection cols = this.columns;
				this.columns = null;
				if(cols != null) {
					cols.Remove(this);
				}
			}
			base.Dispose(disposing);
		}
		public virtual void BestFit() {
			int bestWidth = GetBestWidth();
			if(bestWidth == 0) return;
			ResizeCore(bestWidth, true);
		}
		public int GetBestWidth() {
			return View == null ? VisibleWidth : View.CalcColumnBestWidth(this);
		}
		#endregion public
		protected virtual OptionsColumnEditForm CreateOptionsEditForm() { return new OptionsColumnEditForm(); }
		protected virtual OptionsColumnFilter CreateOptionsFilter() { return new OptionsColumnFilter(); }
		protected virtual OptionsColumn CreateOptionsColumn() { return new OptionsColumn(); }
		protected internal ColumnSortMode GetSortMode() {
			RepositoryItem item = RealColumnEdit;
			ColumnSortMode sort = SortMode;
			if(sort == ColumnSortMode.Default) {
				sort = item.RequireDisplayTextSorting ? ColumnSortMode.DisplayText : ColumnSortMode.Value;
			}
			return sort;
		}
		protected internal virtual ColumnGroupInterval GetGroupInterval() {
			RepositoryItem item = RealColumnEdit;
			ColumnGroupInterval group = GroupInterval;
			if(group == ColumnGroupInterval.Default) {
				if(item is RepositoryItemDateEdit) {
					group = ColumnGroupInterval.Date;
				}
			}
			if(group == ColumnGroupInterval.Default) {
				group = ColumnGroupInterval.Value;
			}
			return group;
		}
		protected internal ColumnFilterMode GetFilterMode() {
			RepositoryItem item = RealColumnEdit;
			if(item is RepositoryItemCheckEdit) return ColumnFilterMode.Value;
			if(IsLookUpDisplayColumn) return ColumnFilterMode.DisplayText;
			return FilterMode;
		}
		protected internal bool IsLookUpDisplayColumn { 
			get { 
				if(View == null || !View.WorkAsLookup) return false;
				return View.LookUpDisplayColumn == this;
			}
		}
		internal bool AllowFilterModeChanging {
			get {
				if(this.OptionsFilter.AllowFilterModeChanging == DefaultBoolean.Default) return IsLookUpRealColumnEdit;
				return this.OptionsFilter.AllowFilterModeChanging == DefaultBoolean.True;
			}
		}
		protected internal virtual bool IsLookUpRealColumnEdit {
			get { 
				return (this.RealColumnEdit is RepositoryItemLookUpEditBase);
			}
		}
		internal void SelectInDesigner() {
			if(View == null) return;
			if(View.CanDesignerSelectColumn(this) && View.IsDesignMode && View.GridControl != null) 
				View.GridControl.SetComponentsSelected(new object[] {this});
		}
		internal bool GetSelectedInDesigner() {
			if(View != null && View.IsDesignMode && View.GridControl != null) 
				return View.GridControl.GetComponentSelected(this);
			return false;
		}
		#region internal
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		protected internal void SetName(string newName) { this.name = newName; }
		protected internal bool WidthLocked { get { return widthLocked; } set { widthLocked = value; } }
		protected internal virtual void OnChanged() {
			if(columns != null) 
				columns.OnChanged(this);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsLoading { get { return View == null || View.IsLoading || View.IsDeserializing; } }
		protected internal virtual void Assign(GridColumn column) {
			this.showUnboundExpressionMenu = column.ShowUnboundExpressionMenu;
			this.allowSummaryMenu = column.AllowSummaryMenu;
			this.unboundExpression = column.UnboundExpression;
			this.unboundType = column.UnboundType;
			this.tag = column.Tag;
			this.sortMode = column.SortMode;
			this.filterPopupInterval = column.FilterPopupInterval;
			this.filterMode = column.FilterMode;
			this.groupInterval = column.GroupInterval;
			this.showButtonMode = column.ShowButtonMode;
			this.customizationCaption = column.CustomizationCaption;
			this.caption = column.Caption;
			this.fColumnEdit = column.ColumnEdit;
			this.fieldNameSortGroup = column.FieldNameSortGroup;
			this.SetFieldNameCore(column.FieldName);
			this._fixed = column.Fixed;
			this.imageAlignment = column.ImageAlignment;
			this.imageIndex = column.ImageIndex;
			this.image = column.Image;
			this.minWidth = column.MinWidth;
			this.maxWidth = column.MaxWidth;
			this.name = column.Name;
			this.toolTip = column.ToolTip;
			this.appearanceCell.AssignInternal(column.AppearanceCell);
			this.appearanceHeader.AssignInternal(column.AppearanceHeader);
			this.visibleIndex = column.visibleIndex;
			this.visible = column.Visible;
			this.width = column.Width;
			this.absoluteIndex = column.AbsoluteIndex;
			this.visibleWidth = column.VisibleWidth;
			this.columnHandle = column.columnHandle;
						this.GroupFormat.Assign(column.GroupFormat);
			this.DisplayFormat.Assign(column.DisplayFormat);
			this.OptionsColumn.Assign(column.OptionsColumn);
			this.OptionsFilter.Assign(column.OptionsFilter);
			this.OptionsEditForm.Assign(column.OptionsEditForm);
			this.Summary.BeginUpdate();
			this.Summary.Assign(column.Summary);
			this.Summary.EndUpdate();
		}
		protected internal void SetCaption(string newCaption) { caption = newCaption; }
		protected internal HorzAlignment DefaultValueAlignment {
			get {
				if(this.defaultValueAlignment != HorzAlignment.Default || IsLoading) return this.defaultValueAlignment;
				this.defaultValueAlignment = View.GridControl.EditorHelper.GetDefaultValueAlignment(RealColumnEdit, ColumnType);
				return this.defaultValueAlignment;
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnUnboundType"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(UnboundColumnType.Bound), XtraSerializableProperty()]
		public UnboundColumnType UnboundType {
			get { return unboundType; }
			set {
				if(UnboundType == value) return;
				unboundType = value;
				if(!IsLoading && UnboundType != UnboundColumnType.Bound && FieldName == string.Empty)
					SetFieldNameCore(Name);
				if(View != null) View.OnColumnUnboundChanged(this);
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnUnboundExpression"),
#endif
 DXCategory(CategoryName.Data), DefaultValue(""), XtraSerializableProperty(),
		Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor))
		]
		public string UnboundExpression {
			get { return unboundExpression; }
			set {
				if(value == null) value = string.Empty;
				if(UnboundExpression == value) return;
				unboundExpression = value;
				if(View != null) View.OnColumnUnboundExpressionChanged(this);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUnboundExpressionValid { 
			get {
				if(View == null) return true;
				if(string.IsNullOrEmpty(UnboundExpression)) return true;
				Exception e;
				return View.IsExpressionValid(UnboundExpression, out e);
			}
		}
		[Browsable(false)]
		public virtual Type ColumnType {
			get {
				if(Name == GridView.CheckBoxSelectorColumnName) return typeof(bool);
				DataColumnInfo col = View.DataController.Columns[ColumnHandle];
				if(col == null) return typeof(object);
				return col.Type;
			}
		}
		DevExpress.Data.Utils.AnnotationAttributes columnAnnotationAttributes;
		[Browsable(false)]
		public virtual DevExpress.Data.Utils.AnnotationAttributes ColumnAnnotationAttributes {
			get {
				if(Name == GridView.CheckBoxSelectorColumnName)
					return null;
				if(columnAnnotationAttributes == null) {
					DataColumnInfo col = View.DataController.Columns[ColumnHandle];
					if(col != null && col.PropertyDescriptor != null)
						columnAnnotationAttributes = new DevExpress.Data.Utils.AnnotationAttributes(col.PropertyDescriptor);
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
		protected virtual void ApplyFormat() {
			Type type = ColumnType;
			for(int n = 0; n < formatInfo.Length; n++) { 
				if(formatInfo[n].TypeName.IndexOf("+" + type.Name + "+") != -1) {
					if(DisplayFormat.FormatType == DevExpress.Utils.FormatType.None)
						DisplayFormat.FormatType = formatInfo[n].FormatType;
					break;
				}
			}
		}
		#endregion internal
		#region private
		struct ColumnFormatInfo {
			public string TypeName;
			public FormatType FormatType;
			internal ColumnFormatInfo(string name, FormatType typ) {
				TypeName = name;
				FormatType = typ;
			}
		}
		static ColumnFormatInfo[] formatInfo = new ColumnFormatInfo[] {
																		  new ColumnFormatInfo("+DateTime+", FormatType.DateTime),
																		  new ColumnFormatInfo("+Byte+Decimal+Double+Int16+Int32+Int64+SByte+Single+UInt16++UInt32+UInt64+", 
																		  FormatType.Numeric)
																	  };
		internal bool temporaryFixed;
		#endregion private
		#region protected
		protected virtual void OnOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if(View != null) View.OnColumnOptionsChanged(this, e);
			OnChanged();
		}
		protected internal virtual void InternalSetWidth(int newValue) {
			if(OptionsColumn.FixedWidth) return;
			this.width = newValue;
		}
		#endregion protected
		internal bool GetIsNonSortableEditor() {
			return RealColumnEdit != null && RealColumnEdit.IsNonSortableEditor;
		}
		protected internal virtual bool GetAllowSort() {
			DefaultBoolean opt = OptionsColumn.AllowSort;
			if(!IsLoading && opt == DefaultBoolean.Default) {
				if(GetIsNonSortableEditor()) return false;
				if(ColumnInfoSort == null) return DesignMode || View.IsLevelDefault;
				return ColumnInfoSort.AllowSort;
			}
			return opt != DefaultBoolean.False;
		}
		protected internal bool IsListFilterPopup {
			get { return GetFilterPopupMode() == FilterPopupMode.List; }
		}
		protected internal bool IsCheckedListFilterPopup {
			get { return GetFilterPopupMode() == FilterPopupMode.CheckedList; }
		}
		protected internal bool IsDateFilterPopup {
			get {
				return GetFilterPopupMode() == FilterPopupMode.Date || GetFilterPopupMode() == FilterPopupMode.DateSmart || GetFilterPopupMode() == FilterPopupMode.DateAlt;
			}
		}
		protected internal virtual FilterPopupMode GetFilterPopupMode() {
			FilterPopupMode mode = OptionsFilter.FilterPopupMode;
			if(mode == FilterPopupMode.Default 
				&& (ColumnType.Equals(typeof(DateTime)) || ColumnType.Equals(typeof(DateTime?))) 
				&& FilterMode != ColumnFilterMode.DisplayText) mode = FilterPopupMode.DateSmart;
			if(mode == FilterPopupMode.Default) mode = FilterPopupMode.List;
			return mode;
		}
		protected internal virtual bool IsCheckedFilterPopup {
			get {
				return this.OptionsFilter.FilterPopupMode == FilterPopupMode.CheckedList;
			}
		}
		protected internal virtual bool GetAllowGroup() {
			DefaultBoolean opt = OptionsColumn.AllowGroup;
			if(!IsLoading && opt == DefaultBoolean.Default) {
				if(GetIsNonSortableEditor()) return false;
				opt = DefaultBoolean.True;
			}
			return opt != DefaultBoolean.False;
		}
		protected internal virtual bool GetAllowMerge() {
			if(View == null) return false;
			return View.GetColumnAllowMerge(this);
		}
		protected virtual bool OnFormat_AllowParse(object sender) {
			return IsLoading;
		}
		protected virtual void OnFormat_Changed(object sender, EventArgs e) {
			OnChanged();
		}
		internal void UnGroup(int proposedVisibleIndex) {
			if(GroupIndex == -1) return;
			UnGroupCore(proposedVisibleIndex);
		}
		public void UnGroup() {
			if(GroupIndex == -1) return;
			UnGroupCore(-1);
		}
		public void Group() {
			if(GroupIndex != -1) return;
			GroupIndex = (View == null ? 0 : View.SortInfo.GroupCount);
		}
		protected virtual void UnGroupCore(int proposedVisibleIndex) {
			this.visible = true;
			if(View == null || VisibleIndex > -1) {
				GroupIndex = -1;
				return;
			}
			View.BeginUpdate();
			try {
				GroupIndex = -1;
				if(proposedVisibleIndex > -1) VisibleIndex = proposedVisibleIndex;
				else {
				for(int i = 0; i < View.VisibleColumns.Count; i++)
					if(View.VisibleColumns[i].OptionsColumn.AllowMove) {
						VisibleIndex = i;
						return;
					}
				if(VisibleIndex < 0) VisibleIndex = View.VisibleColumns.Count;
			}
			}
			finally {
				View.EndUpdate();
			}
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null) return true;
			if(id == LayoutIdAppearance) return optGrid.Columns.StoreAppearance;
			if(optGrid.Columns.StoreLayout && id == LayoutIdLayout) return true;
			if(optGrid.StoreDataSettings && id == LayoutIdData) return true;
			if(optGrid.Columns.StoreAllOptions) return true;
			return false;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
			this.sortOrder = ColumnSortOrder.None;
			this.sortIndex = this.groupIndex = -1;
			OptionsLayoutGrid optGrid = options as OptionsLayoutGrid;
			if(optGrid == null || optGrid.Columns.StoreAppearance) {
				AppearanceCell.Reset();
				AppearanceHeader.Reset();
			}
			if(optGrid != null && optGrid.Columns.StoreAllOptions) optGrid = null;
			if(optGrid == null || optGrid.Columns.StoreLayout) {
				this.visibleWidth = this.width = defaultColumnWidth;
				this.VisibleIndex = -1;
				this.Visible = false;
				this.MinWidth = 20;
			}
			if(optGrid == null || optGrid.StoreDataSettings) {
				this.Summary.Clear();
				this.groupInterval = ColumnGroupInterval.Default;
				this.sortMode = ColumnSortMode.Default;
				this.filterMode = ColumnFilterMode.Value;
			}
			if(optGrid == null || optGrid.Columns.StoreAllOptions) {
				FieldName = Caption = string.Empty;
				ShowButtonMode = ShowButtonModeEnum.Default;
				this.ImageIndex = -1;
				this.ImageAlignment = StringAlignment.Near;
				this.ColumnEditName = string.Empty;
				this.DisplayFormat.Reset();
				this.Fixed = FixedStyle.None;
				this.unboundType = UnboundColumnType.Bound;
				this.OptionsColumn.Reset();
				this.OptionsFilter.Reset();
			}
		}
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel { get { return View == null ? UserLookAndFeel.Default : View.ElementsLookAndFeel; } }
		#endregion
		internal void ResetDataSummaryItem() {
			foreach(GridColumnSummaryItem item in Summary) {
				item.SummaryItem = null;
			}
		}
		#region IDataColumnInfoProvider Members
		IDataColumnInfo IDataColumnInfoProvider.GetInfo(object arguments) {
			GridColumnIDataColumnInfoWrapperEnum fieldType = GridColumnIDataColumnInfoWrapperEnum.ExpressionEditor;
			if(arguments != null && Enum.IsDefined(typeof(GridColumnIDataColumnInfoWrapperEnum), arguments)) {
				fieldType = (GridColumnIDataColumnInfoWrapperEnum)arguments;
			}
			return new GridColumnIDataColumnInfoWrapper(this, fieldType);
		}
		#endregion
		internal void UpdateHandle() {
			if(View == null || View.DataControllerCore == null) return;
			this.SetColumnHandle(View.DataController.Columns.GetColumnIndex(FieldName));
		}
		internal void SetFixedCore(FixedStyle fixedStyle) {
			this._fixed = fixedStyle;
		}
	}
	public class GroupSummarySortInfo {
		GridSummaryItem summaryItem;
		ColumnSortOrder sortOrder;
		GridColumn groupColumn;
		GroupSummarySortInfoCollection collection;
		public GroupSummarySortInfo(GridSummaryItem summaryItem, GridColumn groupColumn) : this(summaryItem, groupColumn, ColumnSortOrder.Ascending) { }
		public GroupSummarySortInfo(GridSummaryItem summaryItem, GridColumn groupColumn, ColumnSortOrder sortOrder) {
			if(groupColumn == null) throw new ArgumentException("groupColumn");
			this.groupColumn = groupColumn;
			this.summaryItem = summaryItem;
			this.sortOrder = sortOrder;
			this.collection = null;
		}
		internal int GroupLevel { get { return GroupColumn.GroupIndex; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoGroupColumn")]
#endif
		public GridColumn GroupColumn { get { return groupColumn; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoSummaryItem")]
#endif
		public GridSummaryItem SummaryItem { get { return summaryItem; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoSortOrder")]
#endif
		public ColumnSortOrder SortOrder { get { return sortOrder; } }
		internal void Remove() {
			if(Collection != null) Collection.RemoveCore(this);
		}
		GroupSummarySortInfoCollection Collection { get { return collection; } }
		internal void SetCollection(GroupSummarySortInfoCollection collection) {
			this.collection = collection;
		}
	}
	[ListBindable(false)]
	public class GroupSummarySortInfoCollection : CollectionBase, IEnumerable<GroupSummarySortInfo> {
		[NonSerialized]
		GridView view;
		[NonSerialized]
		int lockUpdate = 0;
		public GroupSummarySortInfoCollection(GridView view) {
			this.view = view;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoCollectionView")]
#endif
		public GridView View { get { return view; } }
		public void ClearAndAddRange(GroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos);
			}
			finally {
				EndUpdate();
			}
		}
		public void Remove(GridSummaryItem summary) {
			GroupSummarySortInfo info = this[summary];
			if(info != null) Remove(info);
		}
		public void Remove(GroupSummarySortInfo info) {
			int index = IndexOf(info);
			if(index != -1) RemoveAt(index);
		}
		public int IndexOf(GroupSummarySortInfo info) { return List.IndexOf(info); }
		public void AddRange(GroupSummarySortInfo[] sortInfos) {
			BeginUpdate();
			try {
				foreach(GroupSummarySortInfo info in sortInfos) { List.Add(info); }
			}
			finally {
				EndUpdate();
			}
		}
		public GroupSummarySortInfo Add(GridSummaryItem summary, ColumnSortOrder sortOrder) {
			if(View.SortInfo.GroupCount == 0) return null;
			return Add(summary, sortOrder, View.SortInfo[0].Column);
		}
		public GroupSummarySortInfo Add(GridSummaryItem summary, ColumnSortOrder sortOrder, GridColumn groupColumn) {
			if(summary == null) throw new ArgumentNullException("summary");
			if(groupColumn == null)  throw new ArgumentNullException("groupColumn");
			if(groupColumn.GroupIndex < 0) return null;
			return Add(new GroupSummarySortInfo(summary, groupColumn, sortOrder));
		}
		public GroupSummarySortInfo Add(GroupSummarySortInfo info) {
			if(info == null) throw new ArgumentNullException("info");
			if(!AddCore(info)) return null;
			return info;
		}
		protected virtual bool AddCore(GroupSummarySortInfo info) {
			if(!CanAdd(info, Count)) return false;
			List.Add(info);
			return true;
		}
		protected virtual bool CanAdd(GroupSummarySortInfo sortInfo, int position) {
			if(View == null || sortInfo.SummaryItem == null || sortInfo.SummaryItem.SummaryType == SummaryItemType.None) return false;
			return true;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoCollectionItem")]
#endif
		public GroupSummarySortInfo this[GridSummaryItem summary] {
			get {
				foreach(GroupSummarySortInfo info in this) {
					if(info.SummaryItem == summary) return info;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GroupSummarySortInfoCollectionItem")]
#endif
		public GroupSummarySortInfo this[int index] { 
			get { 
				if(index < Count) return List[index] as GroupSummarySortInfo; 
				return null;
			} 
		}
		public void BeginUpdate() {
			this.lockUpdate ++;
		}
		void CancelUpdate() {
			this.lockUpdate --;
		}
		public void EndUpdate() {
			if(-- this.lockUpdate == 0) 
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		internal void RemoveCore(GroupSummarySortInfo info) {
			BeginUpdate();
			try {
				Remove(info);
			} finally {
				CancelUpdate();
			}
		}
		internal void Synchronize() {
			BeginUpdate();
			try {
				for(int n = Count - 1; n >= 0; n--) {
					GroupSummarySortInfo info = this[n];
					if(View.GroupSummary.IndexOf(info.SummaryItem) == -1 || info.GroupColumn == null || info.GroupColumn.GroupIndex < 0) 
							RemoveCore(info);
				}
			} finally {
				CancelUpdate();
			}
		}
		protected override void OnInsertComplete(int position, object item) {
			((GroupSummarySortInfo)item).SetCollection(this);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int position, object item) {
			((GroupSummarySortInfo)item).SetCollection(null);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			InnerList.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected internal virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(this.lockUpdate != 0 || View == null) return;
			View.OnGroupSummarySortInfoCollectionChanged(e);
		}
		protected internal SummarySortInfo[] GetSummaryItems() {
			SummarySortInfo[] items = new SummarySortInfo[Count];
			int n = 0;
			foreach(GroupSummarySortInfo info in this) {
				if(info.SummaryItem.SummaryType != SummaryItemType.None && info.SummaryItem.SummaryItem != null && info.GroupLevel >= 0)
					items[n++] = new SummarySortInfo(info.SummaryItem.SummaryItem, info.GroupLevel, info.SortOrder);
			}
			return items;
		}
		internal string GetStateInfo() {
			if(Count == 0) return string.Empty;
			StringBuilder sb = new StringBuilder();
			foreach(GroupSummarySortInfo info in this) {
				string state = GetState(info);
				if(string.IsNullOrEmpty(state)) continue;
				sb.AppendFormat("{0}:{1}", state.Length, state);
			}
			return sb.ToString();
		}
		internal void RestoreState(string value) {
			Clear();
			if(string.IsNullOrEmpty(value)) return;
			while(true) {
				int index = value.IndexOf(':');
				if(index < 1) break;
				int len = 0;
				if(!int.TryParse(value.Substring(0, index), out len)) break;
				RestoreStateInfo(value.Substring(index + 1, len));
				value = value.Substring(index + 1 + len);
			}
		}
		string GetState(GroupSummarySortInfo info) {
			if(info.SummaryItem == null || info.GroupColumn == null) return string.Empty;
			string itemIndexTag = string.Format("#{0}", info.SummaryItem.Index);
			string tag = info.SummaryItem.Tag == null || string.IsNullOrEmpty(info.SummaryItem.Tag.ToString()) ? itemIndexTag : info.SummaryItem.Tag.ToString();
			return string.Format("{0},{1},{2}", info.GroupColumn.Name, info.SortOrder, tag);
		}
		void RestoreStateInfo(string state) {
			try {
				string[] split = state.Split(',');
				if(split.Length < 3) return;
				if(split.Length > 3) split[2] = string.Join(",", split, 2, split.Length - 2);
				string columnName = split[0], summaryTag = split[2];
				ColumnSortOrder order = (ColumnSortOrder)Enum.Parse(typeof(ColumnSortOrder), split[1]);
				GridColumn column = View.Columns.ColumnByName(columnName);
				GridSummaryItem summary = GetSummaryItem(summaryTag);
				if(summary == null || column == null) return;
				Add(summary, order, column);
			}
			catch { }
		}
		GridSummaryItem GetSummaryItem(string summaryTag) {
			if(summaryTag[0] != '#') return View.GroupSummary[(object)summaryTag];
			int num;
			if(!int.TryParse(summaryTag.Substring(1), out num)) return null;
			if(num < 0 || num >= View.GroupSummary.Count) return null;
			return View.GroupSummary[num];
		}
		internal void Update(DevExpress.Utils.Drawing.SortedShapeObjectInfoArgs si, GridColumn gridColumn) {
			if(Count == 0) return;
			foreach(GroupSummarySortInfo info in this) {
				if(info.GroupColumn == gridColumn) {
					si.Ascending = info.SortOrder == ColumnSortOrder.Ascending;
					si.UseAlternateShapes = true;
					return;
				}
			}
		}
		IEnumerator<GroupSummarySortInfo> IEnumerable<GroupSummarySortInfo>.GetEnumerator() {
			foreach(GroupSummarySortInfo sortInfo in InnerList)
				yield return sortInfo;
		}
	}
	[ListBindable(false), Serializable]
	public class GridColumnSortInfoCollection : CollectionBase, IEnumerable<GridColumnSortInfo> {
		[NonSerialized]
		ColumnView view;
		[NonSerialized]
		int lockUpdate = 0;
		internal int groupCount = 0;
		[NonSerialized]
		int maxGroupCount = 1000;
		public GridColumnSortInfoCollection(ColumnView view) {
			this.view = view;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoCollectionView")]
#endif
		public ColumnView View { get { return view; } }
		public void ClearAndAddRange(GridColumnSortInfo[] sortInfos) { ClearAndAddRange(sortInfos, 0); }
		public void ClearAndAddRange(GridColumnSortInfo[] sortInfos, int groupCount) {
			BeginUpdate();
			try {
				Clear();
				AddRange(sortInfos, groupCount);
			}
			finally {
				EndUpdate();
			}
		}
		public void Remove(GridColumn column) {
			GridColumnSortInfo columnInfo = this[column];
			if(columnInfo != null) Remove(columnInfo);
		}
		public void Remove(GridColumnSortInfo columnInfo) {
			int index = IndexOf(columnInfo);
			if(index != -1) RemoveAt(index);
		}
		public int IndexOf(GridColumnSortInfo columnInfo) { return List.IndexOf(columnInfo); }
		public void AddRange(GridColumnSortInfo[] sortInfos) { AddRange(sortInfos, groupCount);	}
		public void AddRange(GridColumnSortInfo[] sortInfos, int groupCount) {
			BeginUpdate();
			try {
				GroupCount = groupCount;
				foreach(GridColumnSortInfo columnInfo in sortInfos) { 
					if(CanAdd(columnInfo, Count)) List.Add(columnInfo); 
				}
			}
			finally {
				EndUpdate();
			}
		}
		internal GridColumnSortInfo AddGroup(GridColumn column) {
			return InsertGroup(GroupCount, column);
		}
		internal GridColumnSortInfo InsertGroup(int position, GridColumn column) {
			if(column == null) throw new ArgumentNullException("column");
			GridColumnSortInfo info = this[column];
			if(info != null && info.IsGrouped) return info;
			BeginUpdate();
			try {
				ColumnSortOrder order = info == null ? ColumnSortOrder.Ascending :  info.SortOrder;
				if(info != null) Remove(info);
				info = Insert(position, column, order);
				GroupCount ++;
			} finally {
				EndUpdate();
			}
			return info;
		}
		public GridColumnSortInfo Add(GridColumn column, ColumnSortOrder sortOrder) {
			if(column == null) throw new ArgumentNullException("column");
			return Add(new GridColumnSortInfo(column, sortOrder));
		}
		public GridColumnSortInfo Add(GridColumnSortInfo columnInfo) {
			if(columnInfo == null) throw new ArgumentNullException("columnInfo");
			if(!AddCore(columnInfo)) return null;
			return columnInfo;
		}
		public GridColumnSortInfo Insert(int position, GridColumn column, ColumnSortOrder sortOrder) {
			return Insert(position, new GridColumnSortInfo(column, sortOrder));
		}
		public GridColumnSortInfo Insert(int position, GridColumnSortInfo columnInfo) {
			if(columnInfo == null) throw new ArgumentNullException("columnInfo");
			if(position < 0) position = 0;
			if(position >= Count) position = Count;
			if(!CanAdd(columnInfo, position)) return null;
			List.Insert(position, columnInfo);
			return columnInfo;
		}
		protected virtual bool AddCore(GridColumnSortInfo columnSortInfo) {
			if(!CanAdd(columnSortInfo, Count)) return false;
			List.Add(columnSortInfo);
			return true;
		}
		protected virtual bool CanAdd(GridColumnSortInfo columnSortInfo, int position) {
			if(View == null || columnSortInfo.Column == null || this[columnSortInfo.Column] != null) return false;
			if(!View.CanDataSortColumn(columnSortInfo.Column)) return false;
			if(this.groupCount > 0 && this.groupCount >= position && !View.CanDataGroupColumn(columnSortInfo.Column)) return false;
			return true;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("GridColumnSortInfoCollectionGroupCount"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int GroupCount {
			get {
				if(groupCount <= Count) return groupCount;
				return Count;
			}
			set {
				if(value > MaxGroupCount) value = MaxGroupCount;
				if(value < 0) value = 0;
				int prev = GroupCount;
				groupCount = value;
				if(GroupCount != prev)
					OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
			}
		}
		protected internal int MaxGroupCount {
			get {
				int res = maxGroupCount;
				if(View != null && View.GetMaxGroupCount() > -1) return View.GetMaxGroupCount();
				return maxGroupCount; 
			}
			set { maxGroupCount = value; }
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoCollectionItem")]
#endif
		public GridColumnSortInfo this[GridColumn column] {
			get {
				foreach(GridColumnSortInfo columnInfo in this) {
					if(columnInfo.Column == column) return columnInfo;
				}
				return null;
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoCollectionItem")]
#endif
		public GridColumnSortInfo this[int index] { get { return List[index] as GridColumnSortInfo; } }
		List<GridColumnSortInfo> clone = new List<GridColumnSortInfo>();
		int cloneGroupCount = 0;
		public void BeginUpdate() {
			if(this.lockUpdate ++ == 0) {
				this.cloneGroupCount = GroupCount;
				this.clone = new List<GridColumnSortInfo>();
				foreach(GridColumnSortInfo s in this) {
					clone.Add(new GridColumnSortInfo(s.Column, s.SortOrder));
				}
			}
		}
		internal void CancelUpdate() {
			--this.lockUpdate;
		}
		public void EndUpdate() {
			if(--this.lockUpdate == 0 && CheckChanges()) {
				try {
					OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
				}
				catch {
					if(Count > 0) Clear();
					throw;
				}
			}
		}
		bool CheckChanges() {
			if(GroupCount != cloneGroupCount) return true;
			if(clone.Count != Count) return true;
			for(int n = 0; n < Count; n++) {
				if(clone[n].Column != this[n].Column || clone[n].SortOrder != this[n].SortOrder) return true;
			}
			return false;
		}
		protected internal bool IsLockUpdate { get { return this.lockUpdate != 0; } }
		protected override void OnInsertComplete(int position, object item) {
			GridColumnSortInfo columnInfo = (GridColumnSortInfo)item;
			columnInfo.SetCollection(this);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, item));
		}
		protected override void OnRemoveComplete(int position, object item) {
			GridColumnSortInfo columnInfo = (GridColumnSortInfo)item;
			columnInfo.SetCollection(null);
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, item));
		}
		protected override void OnClear() {
			if(Count == 0) return;
			InnerList.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
		}
		protected internal virtual void OnCollectionChanged(CollectionChangeEventArgs e) {
			if(View == null) return;
			if(lockUpdate != 0) return;
			View.OnColumnSortInfoCollectionChanged(e);
		}
		protected internal virtual void MoveTo(GridColumnSortInfo columnInfo, int newIndex) {
			int prevIndex = IndexOf(columnInfo);
			if(newIndex < 0) newIndex = -1;
			if(newIndex > Count) newIndex = Count;
			if(prevIndex == newIndex) return;
			if(newIndex == -1) {
				Remove(columnInfo);
			} else {
				InnerList.Remove(columnInfo);
				if(newIndex > prevIndex) newIndex --;
				List.Insert(newIndex, columnInfo);
			}
		}
		protected internal DataColumnSortInfo[] DataSortInfo {
			get {
				DataColumnSortInfo[] dataSortInfo = new DataColumnSortInfo[Count];
				for(int n = 0; n < Count; n++) {
					dataSortInfo[n] = this[n].DataSortInfo;
				}
				return dataSortInfo;
			}
		}
		public void ClearSorting() {
			if(GroupCount < Count) {
				BeginUpdate();
				try {
					while(GroupCount < Count) RemoveAt(Count - 1);
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected internal void OnDeserialize(GridColumnCollection columns) {
			for(int n = Count - 1; n >= 0; n--) {
				if(!this[n].RestoreColumn(columns)) RemoveAt(n);
			}
		}
		protected internal void Assign(GridColumnSortInfoCollection sortInfo, GridColumnCollection columns) {
			BeginUpdate();
			try {
				Clear();
				GroupCount = sortInfo.groupCount;;
				foreach(GridColumnSortInfo info in sortInfo) {
					GridColumn col = columns == null ? info.Column : columns.Find(info.Column);
					if(col != null)
						Add(col, info.SortOrder);
				}
			} finally {
				EndUpdate();
			}
		}
		IEnumerator<GridColumnSortInfo> IEnumerable<GridColumnSortInfo>.GetEnumerator() {
			foreach(GridColumnSortInfo sortInfo in InnerList)
				yield return sortInfo;
		}
	}
	[Serializable, TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverter))]
	public class GridColumnSortInfo : ISerializable {
		[NonSerialized]
		GridColumnSortInfoCollection collection = null;
		[NonSerialized]
		GridColumn column;
		ColumnSortOrder sortOrder;
		public GridColumnSortInfo(GridColumn column, ColumnSortOrder sortOrder) {
			this.column = column;
			this.sortOrder = sortOrder;
		}
		protected GridColumnSortInfoCollection Collection { get { return collection; } }
		protected ColumnView View { get { return Collection != null ? Collection.View : null; } }
		protected GroupSummarySortInfo GetSummarySortInfo() {
			if(View == null || View.GroupSummarySortInfoCore == null || !IsGrouped) return null;
			return View.GroupSummarySortInfoCore[Index];
		}
		void CheckRemoveSummarySortInfo() {
			GroupSummarySortInfo info = GetSummarySortInfo();
			if(info != null) info.Remove();
		}
		internal void SetCollection(GridColumnSortInfoCollection collection) {
			this.collection = collection;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoColumn")]
#endif
		public GridColumn Column {
			get { return column; }
			set {
				if(value == null || value == Column) return;
				column = value;
				OnChanged();
			}
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoSortOrder")]
#endif
		public ColumnSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				if(value == ColumnSortOrder.None) {
					if(IsGrouped) return;
					Remove();
					return;
				}
				CheckRemoveSummarySortInfo();
				sortOrder = value;
				OnChanged();
			}
		}
		protected internal ColumnGroupInterval GroupInterval { get { return Column.GetGroupInterval(); } }
		internal int SortIndex {
			get {
				if(Collection == null || IsGrouped) return -1;
				return Index - Collection.GroupCount;
			}
		}
		internal int Index { get { return Collection == null ? -1 : Collection.IndexOf(this); } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnSortInfoIsGrouped")]
#endif
		public bool IsGrouped { get { return Collection != null && Index < Collection.GroupCount; } }
		internal void Remove() {
			if(Collection != null) Collection.Remove(this);
		}
		protected void OnChanged() {
			if(Collection != null) Collection.OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}
		internal DataColumnSortInfo DataSortInfo {
			get {
				if(Collection == null || Column.ColumnInfoSort == null) return null;
				DataColumnSortInfo res = new DataColumnSortInfo(Column.ColumnInfoSort, SortOrder);
				if(Column.ColumnInfoSort != Column.ColumnInfo) {
					res.AuxColumnInfo = Column.ColumnInfo;
				}
				if(IsGrouped) res.GroupInterval = GroupInterval;
				return res;
			}
		}
		internal void UpdateColumn() {
			Column.sortIndex = SortIndex;
			Column.sortOrder = SortOrder;
			if(IsGrouped)
				column.SetGroupIndex(IsGrouped ? Index : -1);
			else
				column.SetGroupIndexCore(-1);
		}
		string columnName = string.Empty;
		[System.Security.SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
			si.AddValue("SortOrder", (int)SortOrder);
			si.AddValue("ColumnName", Column != null ? Column.Name : "");
			si.AssemblyName = this.GetType().Assembly.GetName().Name;
		}
		internal GridColumnSortInfo(SerializationInfo si, StreamingContext context) : this(null, ColumnSortOrder.None) {
			foreach (SerializationEntry entry in si) {
				switch (entry.Name) {
					case "SortOrder":
						this.sortOrder = (ColumnSortOrder)entry.Value;
						break;
					case "ColumnName":
						this.columnName = (string)entry.Value;
						break;
				}
			}
		}
		internal bool RestoreColumn(GridColumnCollection columns) {
			if(this.columnName == null || this.columnName == string.Empty || columns == null) return false;
			GridColumn col = columns.ColumnByName(this.columnName);
			if(col != null) {
				this.columnName = string.Empty;
				this.column = col;
				return true;
			}
			return false;
		}
	}
	[ToolboxItem(false)]
	public class GridColumnVisibleCollection : GridColumnReadOnlyCollection {
		public GridColumnVisibleCollection(ColumnView view) : base(view) { }
		protected internal bool Hide(GridColumn column) {
			int index = IndexOf(column);
			if(index < 0) {
				column.SetVisibleCore(false, -1);
				return false;
			}
			RemoveAtCore(index);
			column.SetVisibleCore(false, -1);
			UpdateIndexes();
			return true;
		}
		protected internal void UpdateIndexes() {
			for(int n = 0; n < Count; n++) {
				this[n].SetVisibleCore(true, n);
			}
		}
		protected internal bool Show(GridColumn column) { return Show(column, Count); }
		protected internal bool Show(GridColumn column, int index) {
			if(index < 0) index = 0;
			if(index >= Count) index = Count;
			int currentIndex = IndexOf(column);
			if(currentIndex == index) {
				column.SetVisibleCore(true, index);
				return false;
			}
			if(currentIndex < 0) {
				InsertCore(index, column);
			}
			else {
				RemoveAtCore(currentIndex);
				if(index > currentIndex) index--;
				InsertCore(index, column);
			}
			column.SetVisibleCore(true, index);
			UpdateIndexes();
			return true;
		}
		bool isDirty = false;
		internal void SetDirty() { SetDirty(true); }
		internal void SetDirty(bool value) {
			this.isDirty = value;
		}
		protected internal bool IsDirty { get { return isDirty; } }
	}
	[ToolboxItem(false)]
	public class GridColumnReadOnlyCollection : ReadOnlyCollectionBase, IEnumerable<GridColumn> {
		ColumnView view;
		public GridColumnReadOnlyCollection(ColumnView view) {
			this.view = view;
		}
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnReadOnlyCollectionView")]
#endif
		public virtual ColumnView View { get { return view; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnReadOnlyCollectionItem")]
#endif
		public GridColumn this[int index] { 
			get { 
				if(index < Count && index > -1) return (GridColumn)InnerList[index]; 
				return null;
			}  
		}
		internal IList ToIList() { return this.InnerList; }
		public virtual int IndexOf(GridColumn column) { return InnerList.IndexOf(column); }
		public virtual bool Contains(GridColumn column) { return InnerList.Contains(column); } 
		protected internal void InsertCore(int index, GridColumn column) {
			if(Contains(column)) return;
			if(index < 0) return;
			if(index > Count) index = Count;
			InnerList.Insert(index, column);
		}
		protected internal void AddRangeCore(ICollection col) {
			InnerList.AddRange(col);
		}
		protected internal int AddCore(GridColumn column) {
			int index = IndexOf(column);
			if(index > -1) return index;
			return InnerList.Add(column);
		}
		protected internal void CheckRemove(GridColumn column) {
			if(InnerList.Contains(column)) InnerList.Remove(column);
		}
		protected internal void RemoveCore(GridColumn column) {
			InnerList.Remove(column);
		}
		protected internal void RemoveAtCore(int index) { InnerList.RemoveAt(index); }
		protected internal void ClearCore() {
			InnerList.Clear();
		}
		protected internal void SortCore(IComparer comparer) {
			InnerList.Sort(comparer);
		}
		protected internal int VisibleIndexOf(GridColumn column) {
			int v = 0;
			for(int n = 0; n < Count; n ++) {
				if(this[n] == column) return v;
				if(this[n].VisibleIndex != -1) v++;
			}
			return -1;
		}
		protected internal virtual bool MoveToVisibleIndex(GridColumn column, int newIndex) {
			if(newIndex < 1) newIndex = 0;
			if(newIndex > Count) newIndex = Count;
			int prevIndex = IndexOf(column), prevVIndex = VisibleIndexOf(column);
			if(prevVIndex >= 0 && prevVIndex < newIndex) newIndex--;
			if(prevVIndex == newIndex) return false;
			if(prevIndex != -1) RemoveCore(column);
			int vi = -1;
			for(int n = 0; n < Count; n++) {
				if(newIndex - 1 == vi) {
					InsertCore(n, column);
					return true;
				}
				if(this[n].VisibleIndex != -1) vi ++;
			}
			AddCore(column);
			return true;
		}
		IEnumerator<GridColumn> IEnumerable<GridColumn>.GetEnumerator() {
			foreach(GridColumn column in InnerList)
				yield return column;
		}
	}
	#region class GridColumnCollection 
	[ToolboxItem(false), 
#if DXWhidbey
#endif
	ListBindable(false)]
	public class GridColumnCollection : CollectionBase, IEnumerable<GridColumn> {
		IDictionary fields = null;
		protected ColumnView fView;
		protected internal CollectionChangeEventHandler CollectionChanged;
		public GridColumnCollection(ColumnView view) {
			this.fView = view;
		}
		[Browsable(false)]
		public ColumnView View { get { return fView; } }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnCollectionItem")]
#endif
		public GridColumn this[string fieldName] { get { return ColumnByFieldName(fieldName); 	} }
#if !SL
	[DevExpressXtraGridLocalizedDescription("GridColumnCollectionItem")]
#endif
		public GridColumn this[int index] { get { return (GridColumn)List[index]; }  }
		protected internal virtual void Assign(GridColumnCollection columns) {
			this.Clear();
			for(int n = 0; n < columns.Count; n++) {
				this.Add();
			}
			for(int n = 0; n < this.Count; n++) {
				this[n].Assign(columns[n]);
			}
		}
		internal GridColumn ColumnByDataColumn(DataColumnInfo info) {
			if(info == null) return null;
			GridColumn column = (GridColumn)info.Tag;
			if(column != null && column.View != null) return column;
			column = ColumnByFieldName(info.Name);
			info.Tag = column;
			return column;
		}
		public virtual GridColumn ColumnByName(string columnName) {
			GridColumn col;
			for(int n = Count - 1; n >= 0; n--) {
				col = this[n];
				if(col.Name == columnName) return col;
			}
			return null;
		}
		public virtual GridColumn ColumnByFieldName(string fieldName) {
			if(fieldName == null || fieldName.Length == 0)
				return null;
			if(fields == null) {
				fields = new System.Collections.Specialized.HybridDictionary(this.Count);
				foreach(GridColumn col in this) {
					if(col.FieldName == null || col.FieldName.Length == 0)
						continue;
					fields[col.FieldName] = col;
				}
			}
			return (GridColumn)fields[fieldName];
		}
		protected void ResetColumnsByFieldsNameCache() {
			fields = null;		
		}
		public virtual void AddRange(GridColumn[] columns) {
			foreach(GridColumn col in columns) {
				Add(col);
			}
		}
		public void Insert(int index, GridColumn column) {
			if(List.Contains(column)) return;
			List.Insert(index, column);
		}
		public virtual GridColumn Insert(int index) {
			if(index < 0) index = 0;
			if(index >= Count) return Add();
			GridColumn column = CreateColumn();
			List.Insert(index, column);
			return column;
		}
		public virtual int Add(GridColumn column) {
			if(List.Contains(column)) return List.IndexOf(column);
			return List.Add(column);
		}
		public GridColumn AddVisible(string fieldName, string caption) {
			GridColumn res = AddVisible(fieldName);
			res.Caption = caption;
			return res;
		}
		public GridColumn AddVisible(string fieldName) {
			GridColumn res = AddField(fieldName);
			res.Visible = true;
			if(View != null) res.VisibleIndex = View.VisibleColumns.Count;
			return res;
		}
		public virtual GridColumn Add() { return AddField(""); }
#if !DXWhidbey
		[Obsolete(ObsoleteText.SRObsoleteColumnAdd)]
		public virtual GridColumn Add(string fieldName) {
			return AddField(fieldName);
		}
#endif
		public virtual GridColumn AddField(string fieldName) {
			GridColumn column = CreateColumn();
			column.FieldName = fieldName;
			List.Add(column);
			return column;
		}
		public virtual void Remove(GridColumn column) {
			if(!List.Contains(column)) return;
			List.Remove(column);
		}
		public virtual bool Contains(GridColumn column) { return List.Contains(column); }
		public virtual int IndexOf(GridColumn column) { return List.IndexOf(column); }
		protected internal virtual GridColumn CreateColumn() { return new GridColumn(); }
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			GridColumn col = obj as GridColumn;
			if(col == null || (col.columns != null) ) return;
			if(col.FieldName != string.Empty)
				ResetColumnsByFieldsNameCache();
			col.columns = this;
			UpdateIndexes();
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, col));
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			GridColumn col = obj as GridColumn;
			if(col.ColumnInfo != null) col.ColumnInfo.Tag = null;
			if(col.FieldName != string.Empty)
				ResetColumnsByFieldsNameCache();
			col.columns = null;
			UpdateIndexes();
			RaiseCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, obj));
		}
		internal void ChangeFieldName(GridColumn column, string oldName, string newName) {
			if(oldName != string.Empty) {
				ResetColumnsByFieldsNameCache();
			}
			if(newName != string.Empty)
				ResetColumnsByFieldsNameCache();
		}
		protected override void OnClear() {
			ClearColumns(false);
		}
		protected internal virtual void DestroyColumns() {
			ClearColumns(true);
		}
		protected internal virtual void ClearColumns(bool dispose) {
			if(Count == 0) return;
			if(View != null) {
				View.BeginDataUpdate();
				View.DataController.TotalSummary.Clear();
			}
			try {
				for(int n = Count - 1; n >= 0; n --) {
					GridColumn col = this[n];
					List.RemoveAt(n);
					if(dispose) col.Dispose();
				}
			}
			finally {
				if(View != null) {
					View.EndDataUpdate();
				}
				ResetColumnsByFieldsNameCache();
			}
		}
		protected virtual void RaiseCollectionChanged(CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				GridSummaryItem item = e.Element as GridSummaryItem;
				if(item != null && item.SummaryType == SummaryItemType.None) return;
			}
			if(CollectionChanged != null) CollectionChanged(this, e);
		}
		protected virtual void UpdateIndexes() {
			for(int n = 0; n < Count; n++) 
				this[n].absoluteIndex = n;
		}
		protected internal virtual void OnChanged(GridColumn column) {
			if(column == null) {
				UpdateIndexes();
			}
			if(View != null)
				View.OnColumnChanged(column);
		}
		protected internal virtual void SetItemIndex(GridColumn column, int value) {
			if(value < 0) value = 0;
			if(value > Count) value = Count;
			int prevIndex = List.IndexOf(column);
			if(prevIndex < 0 || prevIndex == value) return;
			InnerList.RemoveAt(prevIndex);
			if(value > Count) value = Count;
			InnerList.Insert(value, column);
			UpdateIndexes();
			OnChanged(column);
		}
		protected virtual void ResetAbsoluteIndexes() {
			for(int n = 0; n < this.Count;  n++) {
				this[n].absoluteIndex = n;
			}
		}
		protected internal virtual void Synchronize(GridColumnCollection sourceColumns) {
			GridColumn[] cols = new GridColumn[InnerList.Count];
			InnerList.CopyTo(cols);
			InnerList.Clear();
			foreach(GridColumn col in sourceColumns) {
				GridColumn destColumn = null;
				if(col.Name != "") {
					destColumn = FindColumn(col.Name, cols);
				}
				if(destColumn == null) destColumn = Add();
				else InnerList.Add(destColumn);
				destColumn.Assign(col);
			}
			UpdateHandlesCore();
		}
		protected virtual GridColumn FindColumn(string name, ICollection cols) {
			foreach(GridColumn col in cols) {
				if(col.Name == name) return col;
			}
			return null;
		}
		protected internal virtual GridColumn FindLocalColumn(GridColumn col) {
			if(col.columns == this) return col;
			if(col.columns == null) return null;
			if(col.columns.Count == this.Count) {
				GridColumn res = this[col.AbsoluteIndex];
				if(res.Name == col.Name) return res;
			}
			return null;
		}
		protected internal virtual void UpdateHandlesCore() {
			foreach(GridColumn column in this) {
				column.UpdateHandle();
			}
		}
		internal GridColumn Find(GridColumn otherCollectionColumn) {
			if(otherCollectionColumn == null) return null;
			GridColumn res = null;
			if(otherCollectionColumn.AbsoluteIndex < Count)
				res = this[otherCollectionColumn.AbsoluteIndex];
			else
				res = this[otherCollectionColumn.FieldName];
			if(res == null || res.FieldName != otherCollectionColumn.FieldName) res = null;
			return res;
		}
		internal GridColumn ColumnByFilterField(string filterFieldName) {
			for(int n = 0; n < Count; n++) {
				if(this[n].FilterFieldName == filterFieldName) return this[n];
			}
			return null;
		}
		IEnumerator<GridColumn> IEnumerable<GridColumn>.GetEnumerator() {
			foreach(GridColumn column in InnerList)
				yield return column;
		}
	}
	#endregion class GridColumns
	public class GridColumnFormatInfo : FormatInfo {
		public delegate bool AllowParseEventHandler(object sender);
		public GridColumnFormatInfo() {
		}
		protected internal GridColumnFormatInfo(AllowParseEventHandler allowParse) {
			this.AllowParse = allowParse;
		}
		protected internal event AllowParseEventHandler AllowParse;
		protected override bool IsLoading { 
			get { 
				if(base.IsLoading) return true;
				if(AllowParse != null) return AllowParse(this);
				return false;
			}
		}
	}
}
namespace DevExpress.XtraGrid.Internal {
	public enum GridColumnIDataColumnInfoWrapperEnum { General, Sort, Filter, ExpressionEditor }
	public class GridViewPreviewIDataColumnInfoWrapper : IDataColumnInfo {
		bool isServerMode = false;
		GridView view;
		public GridViewPreviewIDataColumnInfoWrapper(GridView view) {
			this.isServerMode = view.IsServerMode;
			this.view = view;
		}
		string IDataColumnInfo.Caption { get { return "Preview"; } }
		List<IDataColumnInfo> IDataColumnInfo.Columns { get { return new List<IDataColumnInfo>(); } }
		DataControllerBase IDataColumnInfo.Controller { get { return view.DataController; } }
		string IDataColumnInfo.FieldName { 
			get {
				if(isServerMode) return view.PreviewFieldName;
				return GridView.PreviewUnboundColumnName; 
			} 
		}
		Type IDataColumnInfo.FieldType { get { return typeof(string); } }
		string IDataColumnInfo.Name { get { return "Preview"; } }
		string IDataColumnInfo.UnboundExpression { get { return string.Empty; } }
	}
	public class GridColumnIDataColumnInfoWrapper : IDataColumnInfo {
		GridColumnIDataColumnInfoWrapperEnum fieldType;
		GridColumn column;
		public GridColumnIDataColumnInfoWrapper(GridColumn column, GridColumnIDataColumnInfoWrapperEnum fieldType) {
			this.fieldType = fieldType;
			this.column = column;
		}
		public static GridColumn GetColumn(ColumnView view, IDataColumnInfo column) {
			GridColumnIDataColumnInfoWrapper res = column as GridColumnIDataColumnInfoWrapper;
			if(res != null) return res.column;
			return view.Columns[column.FieldName];
		}
		public static bool Contains(List<IDataColumnInfo> list, GridColumn column) {
			for(int n = 0; n < list.Count; n++) {
				GridColumnIDataColumnInfoWrapper w = list[n] as GridColumnIDataColumnInfoWrapper;
				if(w != null) {
					if(w.column == column) return true;
					continue;
				}
				if(list[n].FieldName == column.FieldName) return true;
			}
			return false;
		}
		public static bool Contains(List<IDataColumnInfo> list, string fieldName) {
			for(int n = 0; n < list.Count; n++) {
				GridColumnIDataColumnInfoWrapper w = list[n] as GridColumnIDataColumnInfoWrapper;
				if(list[n].FieldName == fieldName) return true;
			}
			return false;
		}
		#region IDataColumnInfo Members
		ColumnView View { get { return column.View; } }
		string IDataColumnInfo.Caption { get { return column.GetTextCaption(); } }
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				List<IDataColumnInfo> res = new List<IDataColumnInfo>();
				if(View == null) return res;
				foreach(GridColumn col in View.Columns) {
					if(col == this.column) continue;
					if(!col.OptionsColumn.ShowInExpressionEditor && fieldType == GridColumnIDataColumnInfoWrapperEnum.ExpressionEditor) continue;
					res.Add(new GridColumnIDataColumnInfoWrapper(col, this.fieldType));
				}
				return res;
			}
		}
		DataControllerBase IDataColumnInfo.Controller { get { return View == null ? null : View.DataController; } }
		string IDataColumnInfo.FieldName {
			get {
				switch(this.fieldType) {
					case GridColumnIDataColumnInfoWrapperEnum.Filter: return column.FilterFieldName;
					case GridColumnIDataColumnInfoWrapperEnum.Sort: return column.FieldNameSortGroup;
				}
				return column.FieldName;
			}
		}
		Type IDataColumnInfo.FieldType {
			get {
				DataColumnInfo info = column.ColumnInfo;
				switch(this.fieldType) {
					case GridColumnIDataColumnInfoWrapperEnum.Filter: info = column.ColumnInfoFilter; break;
					case GridColumnIDataColumnInfoWrapperEnum.Sort: info = column.ColumnInfoSort; break;
				}
				if(info != null) return info.Type;
				return typeof(object);
			}
		}
		string IDataColumnInfo.Name { get { return column.Name; } }
		string IDataColumnInfo.UnboundExpression { get { return column.UnboundExpression; } }
		#endregion
	}
}
