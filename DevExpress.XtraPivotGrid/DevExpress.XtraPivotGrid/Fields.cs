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
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Repository;
using DevExpress.LookAndFeel;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid.ViewInfo;
using DevExpress.Utils.Design;
using DevExpress.PivotGrid.Export;
using DevExpress.Export.Xl;
namespace DevExpress.XtraPivotGrid {
	[TypeConverter(typeof(DevExpress.Utils.Design.UniversalTypeConverterEx))]
	public class PivotGridCustomTotal : PivotGridCustomTotalBase {
		AppearanceObject appearance;
		public PivotGridCustomTotal()
			: this(PivotSummaryType.Sum) {
		}
		public PivotGridCustomTotal(PivotSummaryType summaryType)
			: base(summaryType) {
			this.appearance = new AppearanceObject();
			this.appearance.Changed += new EventHandler(OnAppearanceChanged);
		}
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridCustomTotalAppearance"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridCustomTotal.Appearance"),
		Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance), Localizable(true)
		]
		public AppearanceObject Appearance { get { return appearance; } }
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		public override void CloneTo(PivotGridCustomTotalBase clone) {
			base.CloneTo(clone);
			if(clone is PivotGridCustomTotal)
				((PivotGridCustomTotal)clone).Appearance.Assign(Appearance);
		}
		public override bool IsEqual(PivotGridCustomTotalBase total) {
			if(!(total is PivotGridCustomTotal)) return false;
			return base.IsEqual(total) && Appearance.IsEqual(((PivotGridCustomTotal)total).Appearance);
		}
	}
	public class PivotGridCustomTotalCollection : PivotGridCustomTotalCollectionBase {
		public PivotGridCustomTotalCollection() { }
		public PivotGridCustomTotalCollection(PivotGridFieldBase field) : base(field) { }
		public PivotGridCustomTotalCollection(PivotGridCustomTotalBase[] totals)
			: base(totals) { }
#if !SL
	[DevExpressXtraPivotGridLocalizedDescription("PivotGridCustomTotalCollectionItem")]
#endif
		public new PivotGridCustomTotal this[int index] { get { return (PivotGridCustomTotal)InnerList[index]; } }
		[Browsable(false)]
		public new PivotGridField Field { get { return (PivotGridField)base.Field; } }
		public new PivotGridCustomTotal Add(PivotSummaryType summaryType) {
			return (PivotGridCustomTotal)base.Add(summaryType);
		}
		public virtual void AddRange(PivotGridCustomTotal[] customSummaries) {
			foreach(PivotGridCustomTotal customTotal in customSummaries) {
				AddCore(customTotal);
			}
		}
		protected override PivotGridCustomTotalBase CreateCustomTotal() {
			return new PivotGridCustomTotal();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridFieldToolTips {
		PivotGridField field;
		string headerText;
		FormatInfo valueFormat;
		string valueText;
		public PivotGridFieldToolTips(PivotGridField field) {
			this.field = field;
			this.headerText = string.Empty;
			this.valueText = string.Empty;
			this.valueFormat = new FormatInfo(Field);
		}
		protected PivotGridField Field { get { return field; } }
		[
		DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.HeaderText")]
		public string HeaderText {
			get { return headerText; }
			set { headerText = value; }
		}
		void ResetValueFormat() { ValueFormat.Reset(); }
		bool XtraShouldSerializeValueFormat() { return ShouldSerializeValueFormat(); }
		bool ShouldSerializeValueFormat() { return !ValueFormat.IsEmpty; }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueFormat")]
		public FormatInfo ValueFormat { get { return valueFormat; } }
		[
		DefaultValue(""), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldToolTips.ValueText")]
		public string ValueText {
			get { return valueText; }
			set { valueText = value; }
		}
		public string GetValueText(object value) {
			if(ValueFormat.IsEmpty) return ValueText;
			return ValueFormat.GetDisplayText(value);
		}
		public bool ShouldSerialize() {
			return HeaderText != string.Empty || ValueText != string.Empty || !ValueFormat.IsEmpty;
		}
		public void Reset() {
			HeaderText = string.Empty;
			ValueText = string.Empty;
			ValueFormat.Reset();
		}
	}
	[
	DesignTimeVisible(false), ToolboxItem(false),
	Designer("DevExpress.XtraPivotGrid.Design.PivotGridFieldDesigner, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(System.ComponentModel.Design.IDesigner))
	]
	public class PivotGridField : PivotGridFieldBase, IPivotGridViewInfoDataOwner, ISupportLookAndFeel {
		PivotGridFieldAppearances appearance;
		int imageIndex;
		Size dropDownFilterListSize;
		PivotGridFieldToolTips toolTips;
		RepositoryItem fieldEdit;
		EventHandler summaryTypeChanged;
		static DevExpress.PivotGrid.IPivotCalculationCreator CalcCreator = new PivotCalculationCreator();
		static PivotGridField() {
			DevExpress.PivotGrid.ReflectionHelper.AddAssembly(System.Reflection.Assembly.GetExecutingAssembly());
		}
		public PivotGridField() : this(string.Empty, PivotArea.FilterArea) { }
		public PivotGridField(PivotGridData data)
			: base(data) {
			Init();
		}
		public PivotGridField(string fieldName, PivotArea area)
			: base(fieldName, area) {
			Init();
		}
		void Init() {
			this.imageIndex = -1;
			this.dropDownFilterListSize = Size.Empty;
			this.toolTips = new PivotGridFieldToolTips(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && appearance != null) {
				appearance.Changed -= new EventHandler(OnAppearanceChanged);
				this.appearance = null;
			}
			base.Dispose(disposing);
		}
		#region events
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldSummaryTypeChanged"),
#endif
 Category("Layout")]
		public event EventHandler SummaryTypeChanged {
			add { summaryTypeChanged += value; }
			remove { summaryTypeChanged -= value; }
		}
		protected void RaiseSummaryTypeChanged() {
			if(summaryTypeChanged != null) summaryTypeChanged(this, new EventArgs());
		}
		#endregion
		protected override DevExpress.PivotGrid.IPivotCalculationCreator CalculationCreator {
			get { return CalcCreator; }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldDataType"),
#endif
 Category("Data"), Browsable(false)]
		public override Type DataType {
			get {
				if(IsDesignTime && base.DataType == typeof(object))
					return null; 
				return base.DataType;
			}
		}
		void ResetCustomTotals() { CustomTotals.Clear(); }
		bool ShouldSerializeCustomTotals() { return CustomTotals.Count > 0; }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldCustomTotals"),
#endif
 Browsable(true),
		Category("Behaviour"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, true, true, 0, XtraSerializationFlags.DefaultValue),
		Editor("DevExpress.XtraPivotGrid.Design.CustomTotalsCollectionEditor, " + AssemblyInfo.SRAssemblyPivotGridDesignFull,
			"System.Drawing.Design.UITypeEditor, System.Drawing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.CustomTotals"),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public new PivotGridCustomTotalCollection CustomTotals {
			get { return (PivotGridCustomTotalCollection)base.CustomTotals; }
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptions"),
#endif
 Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Options")
		]
		public new PivotGridFieldOptionsEx Options { get { return (PivotGridFieldOptionsEx)base.Options; } }
		protected override PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollection(this);
		}
		protected override PivotGridFieldOptions CreateOptions(PivotOptionsChangedEventHandler eventHandler, string name) {
			return new PivotGridFieldOptionsEx(eventHandler, this, "Options");
		}
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		protected PivotGridField DataField { get { return Data != null ? (PivotGridField)Data.DataField : null; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldAppearance"),
#endif
 Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Appearance")]
		public virtual PivotGridFieldAppearances Appearance { 
			get {
				if(appearance == null) {
					this.appearance = CreateAppearance();
					this.appearance.Changed += new EventHandler(OnAppearanceChanged);
				}
				return appearance;
			}
		}
		protected virtual PivotGridFieldAppearances CreateAppearance() {
			return new PivotGridFieldAppearances(this);
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldImageIndex"),
#endif
 DefaultValue(-1), Category("Appearance"),
		Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(System.Drawing.Design.UITypeEditor)),
		DevExpress.Utils.ImageList("HeaderImages"), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ImageIndex")]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				OnImageChanged();
			}
		}
		void ResetToolTips() { ToolTips.Reset(); }
		bool ShouldSerializeToolTips() { return ToolTips.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldToolTips"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ToolTips")]
		public PivotGridFieldToolTips ToolTips { get { return toolTips; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object HeaderImages {
			get { return DataViewInfo == null ? null : DataViewInfo.HeaderImages; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Size DropDownFilterListSize {
			get { return dropDownFilterListSize; }
			set { dropDownFilterListSize = value; }
		}
		[Browsable(false)]
		public PivotGridControl PivotGrid { get { return DataViewInfo != null ? DataViewInfo.PivotGrid : null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetDefaultEditParameters(RepositoryItem edit) {
			if(edit == null)
				return;
			if(!edit.DisplayFormat.IsEquals(CellFormat) && !edit.DisplayFormat.IsEquals(DefaultDecimalFormat) && !edit.DisplayFormat.IsEquals(TotalCellFormat) && !edit.DisplayFormat.IsEquals(GrandTotalCellFormat))
				edit.DisplayFormat.Assign(CellFormat);
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual bool CanEdit {
			get {
				if(IsDesignTime) return false;
				return Options.AllowEdit && Data.OptionsCustomization.AllowEdit;
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public virtual bool CanFilterRadioMode {
			get {
				if(Data != null && Options.IsFilterRadioMode == DefaultBoolean.Default)
					return Data.OptionsFilterPopup.IsRadioMode;
				return Options.IsFilterRadioMode == DefaultBoolean.True;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public virtual string FieldEditName {
			get {
				if(FieldEdit != null) return FieldEdit.Name;
				return "";
			}
			set {
				if(value == FieldEditName) return;
				if(PivotGrid == null) return;
				DevExpress.XtraEditors.Repository.RepositoryItem ri = PivotGrid.EditorHelper.FindRepositoryItem(value);
				if(ri != null) FieldEdit = ri;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null),
		TypeConverter(typeof(DevExpress.XtraPivotGrid.TypeConverters.FieldEditConverter)),
		Editor("DevExpress.XtraPivotGrid.Design.FieldEditEditor, " + AssemblyInfo.SRAssemblyPivotGridDesign, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldFieldEdit"),
#endif
 Category("Editing")]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.FieldEdit")]
		public virtual RepositoryItem FieldEdit {
			get { return fieldEdit; }
			set {
				if(fieldEdit == value)
					return;
				RepositoryItem old = fieldEdit;
				fieldEdit = value;
				if(old != null) old.Disconnect(this);
				if(FieldEdit != null) {
					FieldEdit.Connect(this);
				}
				SetDefaultEditParameters(FieldEdit);
				OnEditOptionsChanged();
			}
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public virtual bool CanShowUnboundExpressionMenu {
			get {
				if(IsDesignTime) return false;
				return IsUnbound && Options.ShowUnboundExpressionMenu;
			}
		}
		[Editor(typeof(DevExpress.XtraEditors.Design.ExpressionEditorBase), typeof(System.Drawing.Design.UITypeEditor))]
		public new string UnboundExpression { get { return base.UnboundExpression; } set { base.UnboundExpression = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanHide {
			get { return Options.AllowHide == DefaultBoolean.Default ? base.CanHide : Options.AllowHide != DefaultBoolean.False; }
		}
		public void BestFit() {
			if(DataViewInfo != null)
				DataViewInfo.BestFit(this);
		}
		void OnImageChanged() {
			if(Visible) DoLayoutChanged();
		}
		void OnEditOptionsChanged() {
			if(!Visible || Area != PivotArea.DataArea) return;
			DoLayoutChanged();
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			if(Visible) DoLayoutChanged();
		}
		protected override void OnSummaryChanged(PivotSummaryType oldSummaryType) {
			base.OnSummaryChanged(oldSummaryType);
			RaiseSummaryTypeChanged();
		}
		protected PivotGridViewInfoData DataViewInfo { get { return (PivotGridViewInfoData)base.Data; } }
		PivotGridViewInfoData IPivotGridViewInfoDataOwner.DataViewInfo {
			get { return DataViewInfo; }
		}
		public override string ToString() {
			string displayText = base.ToString();
			return displayText != string.Empty ? displayText : Name;
		}
		protected override void OnResetSerializationProperties(OptionsLayoutBase options) {
			base.OnResetSerializationProperties(options);
			Appearance.Reset();
		}
		public void ShowFilterPopup() {
			if(Data == null || Data.ViewInfo == null) return;
			PivotHeadersViewInfoBase headersViewInfo = Data.ViewInfo.GetHeader(Area);
			PivotHeaderViewInfo headerViewInfo = (PivotHeaderViewInfo)headersViewInfo[Data.GetFieldItem(this)];
			if(headerViewInfo != null)
				headerViewInfo.ShowFilterPopup();
		}
		#region ISupportLookAndFeel Members
		bool ISupportLookAndFeel.IgnoreChildren { get { return true; } }
		UserLookAndFeel ISupportLookAndFeel.LookAndFeel {
			get { return Data == null ? UserLookAndFeel.Default : Data.ActiveLookAndFeel; }
		}
		#endregion
	}
	public class PivotGridFieldOptionsEx : PivotGridFieldOptions {
		bool readOnly;
		bool allowEdit;
		DefaultBoolean allowHide;
		PivotShowButtonModeEnum showButtonMode;
		bool showExpressionMenu;
		DefaultBoolean isFilterRadioMode;
		public PivotGridFieldOptionsEx(PivotOptionsChangedEventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.readOnly = false;
			this.allowEdit = true;
			this.allowHide = DefaultBoolean.Default;
			this.showButtonMode = PivotShowButtonModeEnum.Default;
			this.showExpressionMenu = false;
			this.isFilterRadioMode = DefaultBoolean.Default;
		}
		[
		XtraSerializableProperty(), DefaultValue(false),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExReadOnly"),
#endif
 Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridField.Options.ReadOnly"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				if(readOnly == value) return;
				readOnly = value;
				OnOptionsChanged(FieldOptions.ReadOnly);
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(true),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExAllowEdit"),
#endif
 Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridField.Options.AllowEdit"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool AllowEdit {
			get { return allowEdit; }
			set {
				if(allowEdit == value) return;
				allowEdit = value;
				OnOptionsChanged(FieldOptions.AllowEdit);
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(PivotShowButtonModeEnum.Default),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExShowButtonMode"),
#endif
 Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile,
			"DevExpress.XtraPivotGrid.PivotGridField.Options.ShowButtonMode")
		]
		public virtual PivotShowButtonModeEnum ShowButtonMode {
			get { return showButtonMode; }
			set {
				if(showButtonMode == value) return;
				showButtonMode = value;
				OnOptionsChanged(FieldOptions.ShowButtonMode);
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(false),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExShowUnboundExpressionMenu"),
#endif
 Category("Editing"),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
			DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.Options.ShowUnboundExpressionMenu"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool ShowUnboundExpressionMenu {
			get {
				return showExpressionMenu;
			}
			set {
				if(showExpressionMenu == value) return;
				showExpressionMenu = value;
				OnOptionsChanged(FieldOptions.ShowUnboundExpressionMenu);
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default), Category("Editing"),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExIsFilterRadioMode"),
#endif
 NotifyParentProperty(true)
		]
		public virtual DefaultBoolean IsFilterRadioMode {
			get { return isFilterRadioMode; }
			set {
				if(value == isFilterRadioMode) return;
				isFilterRadioMode = value;
				OnOptionsChanged(FieldOptions.IsFilterRadioMode);
			}
		}
		[
		XtraSerializableProperty(), DefaultValue(DefaultBoolean.Default),
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldOptionsExAllowHide"),
#endif
 NotifyParentProperty(true),
		]
		public virtual DefaultBoolean AllowHide {
			get { return allowHide; }
			set { 
				allowHide = value; 
				OnOptionsChanged(FieldOptions.AllowFilter);
			}
		}
	}
	public class PivotGridFieldCollection : PivotGridFieldCollectionBase {
		public PivotGridFieldCollection(PivotGridData data) : base(data) { }
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldCollectionItem"),
#endif
 EditorBrowsable(EditorBrowsableState.Always)]
		public new PivotGridField this[int index] { get { return (PivotGridField)base[index]; } }
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridFieldCollectionItem"),
#endif
 EditorBrowsable(EditorBrowsableState.Always)]
		public new PivotGridField this[string fieldName] { get { return (PivotGridField)base[fieldName]; } }
		public new PivotGridField GetFieldByName(string name) {
			return (PivotGridField)base.GetFieldByName(name);
		}
		public new PivotGridField Add() {
			return (PivotGridField)base.Add();
		}
		public new PivotGridField Add(string fieldName, PivotArea area) {
			return (PivotGridField)base.Add(fieldName, area);
		}
		public void Add(PivotGridField field) {
			base.Add(field);
		}
		public virtual void AddRange(PivotGridField[] fields) {
			foreach(PivotGridField field in fields) {
				AddCore(field);
			}
		}
		public void Remove(PivotGridField field) {
			List.Remove(field);
		}
		protected override PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			PivotGridField field = CreateFieldCore(fieldName, area);
			IContainer componentContainer = Data.GetComponentContainer();
			if(componentContainer != null) {
				if(!Data.IsLoading && field.Name == string.Empty && field.FieldName != string.Empty)
					field.SetNameCore(GenerateName(field.FieldName));
				try {
					componentContainer.Add(field, field.Name);
				} catch {
					componentContainer.Add(field);
				}
			}
			return field;
		}
		protected virtual PivotGridField CreateFieldCore(string fieldName, PivotArea area) {
			return new PivotGridField(fieldName, area);
		}
		protected override bool IsNameOccupied(string name) {
			bool res = base.IsNameOccupied(name);
			if(res) 
				return res;
			IContainer container = Data.GetComponentContainer();
			return container != null && container.Components[name] != null;
		}
		protected override void OnInsertComplete(int index, object obj) {
			base.OnInsertComplete(index, obj);
			Data.PopupateCustomizationFormFields();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			DisposeField((PivotGridField)obj);
		}
		protected override void DisposeField(PivotGridFieldBase field) {
			IContainer componentContainer = Data.GetComponentContainer();
			if(componentContainer != null) {
				componentContainer.Remove(field);
			}
			base.DisposeField(field);
			Data.PopupateCustomizationFormFields();
		}
	}
}
