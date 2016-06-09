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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	using DevExpress.Utils;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Localization;
	using DevExpress.Web.DropDownEdit;
	using DevExpress.Web.Design;
	public enum ColorOnError { Undo, Null }
	public class ColorEditProperties : DropDownEditPropertiesBase {
		ColorNestedControlProperties colorNestedControlProperties;
		public ColorEditProperties()
			: base() {
		}
		public ColorEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal ColorNestedControlProperties ColorNestedControlProperties {
			get {
				if(colorNestedControlProperties == null)
					colorNestedControlProperties = new ColorNestedControlProperties();
				return colorNestedControlProperties;
			}
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Browsable(false),
		MergableProperty(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public ColorEditItemCollection Items {
			get { return ColorNestedControlProperties.Items; }
		}
		protected internal ColorEditItemCollection CustomColorTableItems {
			get { return ColorNestedControlProperties.CustomColorTableItems; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesEnableCustomColors"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableCustomColors {
			get { return ColorNestedControlProperties.EnableCustomColors; }
			set { ColorNestedControlProperties.EnableCustomColors = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesEnableAutomaticColorItem"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public bool EnableAutomaticColorItem {
			get { return ColorNestedControlProperties.EnableAutomaticColorItem; }
			set { ColorNestedControlProperties.EnableAutomaticColorItem = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesAutomaticColor"),
#endif
		DefaultValue(typeof(Color), "Black"), NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter)), AutoFormatDisable]
		public Color AutomaticColor {
			get { return ColorNestedControlProperties.AutomaticColor; }
			set { ColorNestedControlProperties.AutomaticColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesAutomaticColorItemCaption"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_AutomaticColor)]
		public string AutomaticColorItemCaption {
			get { return ColorNestedControlProperties.AutomaticColorItemCaption; }
			set { ColorNestedControlProperties.AutomaticColorItemCaption = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesAutomaticColorItemValue"),
#endif
		DefaultValue(ASPxColorEdit.DefaultAutomaticColorItemValue), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public string AutomaticColorItemValue {
			get { return ColorNestedControlProperties.AutomaticColorItemValue; }
			set { ColorNestedControlProperties.AutomaticColorItemValue = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColumnCount"),
#endif
		DefaultValue(ColorTable.DefaultColumnCount), NotifyParentProperty(true), AutoFormatDisable]
		public int ColumnCount {
			get { return ColorNestedControlProperties.ColumnCount; }
			set { ColorNestedControlProperties.ColumnCount = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool AllowNull {
			get { return GetBoolProperty("AllowNull", true); }
			set { SetBoolProperty("AllowNull", true, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties ButtonEditEllipsisImage {
			get { return Images.ButtonEditEllipsis; }
		}
		[NotifyParentProperty(true), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength {
			get { return base.MaxLength; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorIndicatorWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit ColorIndicatorWidth {
			get { return GetUnitProperty("ColorIndicatorWidth", Unit.Empty); }
			set { SetUnitProperty("ColorIndicatorWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorIndicatorHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit ColorIndicatorHeight {
			get { return GetUnitProperty("ColorIndicatorHeight", Unit.Empty); }
			set { SetUnitProperty("ColorIndicatorHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesDisplayColorIndicatorWidth"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit DisplayColorIndicatorWidth {
			get { return GetUnitProperty("DisplayColorIndicatorWidth", Unit.Empty); }
			set { SetUnitProperty("DisplayColorIndicatorWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesDisplayColorIndicatorHeight"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit DisplayColorIndicatorHeight {
			get { return GetUnitProperty("DisplayColorIndicatorHeight", Unit.Empty); }
			set { SetUnitProperty("DisplayColorIndicatorHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesDisplayColorIndicatorSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatEnable]
		public Unit DisplayColorIndicatorSpacing {
			get { return GetUnitProperty("DisplayColorIndicatorSpacing", Unit.Empty); }
			set { SetUnitProperty("DisplayColorIndicatorSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorOnError"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(ColorOnError.Undo), AutoFormatDisable]
		public ColorOnError ColorOnError {
			get { return (ColorOnError)GetEnumProperty("ColorOnError", ColorOnError.Undo); }
			set { SetEnumProperty("ColorOnError", ColorOnError.Undo, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new ColorEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as ColorEditClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorTableStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorTableStyle ColorTableStyle {
			get { return Styles.ColorTable; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorPickerStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorPickerStyle ColorPickerStyle {
			get { return Styles.ColorPicker; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorTableCellStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorTableCellStyle ColorTableCellStyle {
			get { return Styles.ColorTableCell; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesColorIndicatorStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorIndicatorStyle ColorIndicatorStyle {
			get { return Styles.ColorIndicator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesDisplayColorIndicatorStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorIndicatorStyle DisplayColorIndicatorStyle {
			get { return Styles.DisplayColorIndicator; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set {  }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool DisplayFormatInEditMode {
			get { return base.DisplayFormatInEditMode; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesCustomColorButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_CustomColor)]
		public string CustomColorButtonText {
			get { return ColorNestedControlProperties.CustomColorButtonText; }
			set { ColorNestedControlProperties.CustomColorButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesOkButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_OK)]
		public string OkButtonText {
			get { return ColorNestedControlProperties.OkButtonText; }
			set { ColorNestedControlProperties.OkButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditPropertiesCancelButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_Cancel)]
		public string CancelButtonText {
			get { return ColorNestedControlProperties.CancelButtonText; }
			set { ColorNestedControlProperties.CancelButtonText = value; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxColorEdit();
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			ParentSkinOwner = args.SkinOwner;
			ColorEditDisplayControl displayControl = new ColorEditDisplayControl(this);
			string text = GetDisplayText(args);
			displayControl.Color = !CommonUtils.IsNullValue(text) ? text : "";
			return displayControl;
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(!CommonUtils.IsNullValue(args.EditValue) && (args.EditValue is Color))
				args.EditValue = ColorUtils.ToHexColor((Color)args.EditValue);
			return base.GetDisplayTextCore(args, encode);
		}
		protected internal AppearanceStyleBase GetDisplayColorIndicatorStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultColorIndicatorStyle());
			style.CopyFrom(DisplayColorIndicatorStyle);
			style.Cursor = "default";
			return style;
		}
		protected internal Unit GetDisplayColorIndicatorSpacing() {
			return DisplayColorIndicatorSpacing.IsEmpty ? Styles.GetDefaultDisplayColorIndicatorSpacing() : DisplayColorIndicatorSpacing;
		}
		protected internal Unit GetColorIndicatorHeight(Unit indicatorHeight, AppearanceStyleBase indicatorStyle) {
			Unit height = indicatorHeight;
			if(!height.IsEmpty && height.Type != UnitType.Percentage) {
				double dHeight = ConvertToPixels(height);
				dHeight -= ConvertToPixels(indicatorStyle.GetBorderWidthTop()) +
						 ConvertToPixels(indicatorStyle.GetBorderWidthBottom());
				height = Unit.Pixel((int)dHeight);
			}
			return height;
		}
		protected internal Unit GetColorIndicatorWidth(Unit indicatorWidth, AppearanceStyleBase indicatorStyle) {
			Unit width = indicatorWidth;
			if(!width.IsEmpty && width.Type != UnitType.Percentage) {
				double dWidth = ConvertToPixels(width);
				dWidth -= ConvertToPixels(indicatorStyle.GetBorderWidthLeft()) +
						 ConvertToPixels(indicatorStyle.GetBorderWidthRight());
				width = Unit.Pixel((int)dWidth);
			}
			return width;
		}
		protected internal double ConvertToPixels(Unit value) {
			UnitType heghtType = value.Type;
			double heightValue = value.Value;
			UnitUtils.ConvertToPixels(ref heghtType, ref heightValue);
			return heightValue;
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new ColorEditClientSideEvents(this);
		}
		protected internal override bool IsClearButtonVisibleAuto() {
			return AllowNull && base.IsClearButtonVisibleAuto();
		}
		public override void Assign(PropertiesBase source) {
			ColorEditProperties src = source as ColorEditProperties;
			if(src != null) {
				AllowNull = src.AllowNull;
				ColorOnError = src.ColorOnError;
				ColorIndicatorWidth = src.ColorIndicatorWidth;
				ColorIndicatorHeight = src.ColorIndicatorHeight;
				DisplayColorIndicatorWidth = src.DisplayColorIndicatorWidth;
				DisplayColorIndicatorHeight = src.DisplayColorIndicatorHeight;
				DisplayColorIndicatorSpacing = src.DisplayColorIndicatorSpacing;
				ColorNestedControlProperties.Assign(src.ColorNestedControlProperties);
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ColorNestedControlProperties });
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	Designer("DevExpress.Web.Design.ASPxColorEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxColorEdit.bmp")
	]
	public class ASPxColorEdit : ASPxDropDownEditBase, IControlDesigner {
		protected internal const string ColorEditScriptResourceName = EditScriptsResourcePath + "ColorEdit.js";
		protected internal static string ColorIndicatorIdPostfix = "CI";
		protected internal const string DefaultAutomaticColorItemValue = "Automatic";
		private static readonly object ColorChangedEvent = new object();
		public ASPxColorEdit()
			: base() {
		}
		protected ASPxColorEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string DisplayFormatString {
			get { return Properties.DisplayFormatString; }
			set { Properties.DisplayFormatString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditEnableCustomColors"),
#endif
		DefaultValue(false), AutoFormatDisable]
		public bool EnableCustomColors {
			get { return Properties.EnableCustomColors; }
			set {
				Properties.EnableCustomColors = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditEnableAutomaticColorItem"),
#endif
		DefaultValue(false), AutoFormatDisable]
		public bool EnableAutomaticColorItem {
			get { return Properties.EnableAutomaticColorItem; }
			set {
				Properties.EnableAutomaticColorItem = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditAutomaticColorItemCaption"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_AutomaticColor)]
		public string AutomaticColorItemCaption {
			get { return Properties.AutomaticColorItemCaption; }
			set { Properties.AutomaticColorItemCaption = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditAutomaticColor"),
#endif
		DefaultValue(typeof(Color), "Black"), TypeConverter(typeof(WebColorConverter)),
		NotifyParentProperty(true), AutoFormatDisable]
		public Color AutomaticColor {
			get { return Properties.AutomaticColor; }
			set { Properties.AutomaticColor = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditAutomaticColorItemValue"),
#endif
		DefaultValue(DefaultAutomaticColorItemValue), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public string AutomaticColorItemValue {
			get { return Properties.AutomaticColorItemValue; }
			set { Properties.AutomaticColorItemValue = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColumnCount"),
#endif
		DefaultValue(ColorTable.DefaultColumnCount), AutoFormatDisable]
		public int ColumnCount {
			get { return Properties.ColumnCount; }
			set { Properties.ColumnCount = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return Properties.AllowNull; }
			set { Properties.AllowNull = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorOnError"),
#endif
		Category("Behavior"), DefaultValue(ColorOnError.Undo), AutoFormatDisable]
		public ColorOnError ColorOnError {
			get { return Properties.ColorOnError; }
			set { Properties.ColorOnError = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies {
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID {
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event ButtonEditClickEventHandler ButtonClick {
			add {  }
			remove {  }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new event EventHandler TextChanged {
			add { }
			remove { }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ButtonImageProperties ButtonEditEllipsisImage {
			get { return Properties.ButtonEditEllipsisImage; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int MaxLength {
			get { return Properties.MaxLength; }
			set {  }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColor"),
#endif
		DefaultValue(typeof(Color), ""), TypeConverter(typeof(WebColorConverter)),
		Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public Color Color {
			get {
				if(IsAutomaticColorSelected)
					return AutomaticColor;
				else
					if(Value is Color)
						return (Color)Value;
					else
						return Color.Empty;
			}
			set {
				if(value == Color.Empty)
					Value = null;
				else
					Value = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsAutomaticColorSelected {
			get { return Value != null && AutomaticColorItemValue != null && Value.Equals(AutomaticColorItemValue); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(true, BindingDirection.TwoWay), AutoFormatDisable, Localizable(false)]
		public override string Text {
			get {
				if(Value is Color) {
					if(Color == Color.Empty)
						return "";
					return ColorUtils.ToHexColor(Color);
				}
				return base.Text; 
			}
			set { Value = ColorUtils.ValueToColor(value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxColorEditValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set { 
				Color color;
				if (value is Color)
					base.Value = value;
				else if (value != null && ColorUtils.TryParseColor(value.ToString(), out color))
					this.Value = color;
				else
					base.Value = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ColorEditItemCollection Items {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorIndicatorWidth"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ColorIndicatorWidth {
			get { return Properties.ColorIndicatorWidth; }
			set { Properties.ColorIndicatorWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorIndicatorHeight"),
#endif
		Category("Layout"), AutoFormatEnable, DefaultValue(typeof(Unit), "")]
		public Unit ColorIndicatorHeight {
			get { return Properties.ColorIndicatorHeight; }
			set { Properties.ColorIndicatorHeight = value; }
		}
		protected ColorEditItemCollection CustomColorTableItems {
			get { return Properties.CustomColorTableItems; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditCustomColorButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_CustomColor)]
		public string CustomColorButtonText {
			get { return Properties.CustomColorButtonText; }
			set { Properties.CustomColorButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditOkButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_OK)]
		public string OkButtonText {
			get { return Properties.OkButtonText; }
			set { Properties.OkButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditCancelButtonText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable, Localizable(true), DefaultValue(StringResources.ColorEdit_Cancel)]
		public string CancelButtonText {
			get { return Properties.CancelButtonText; }
			set { Properties.CancelButtonText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorTableStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColorTableStyle ColorTableStyle {
			get { return Properties.ColorTableStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorPickerStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColorPickerStyle ColorPickerStyle {
			get { return Properties.ColorPickerStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorTableCellStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColorTableCellStyle ColorTableCellStyle {
			get { return Properties.ColorTableCellStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorIndicatorStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ColorIndicatorStyle ColorIndicatorStyle {
			get { return Properties.ColorIndicatorStyle; }
		}
		protected AppearanceStyleBase DefaultColorIndicatorStyle {
			get { return Properties.Styles.GetDefaultColorIndicatorStyle(); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new ColorEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxColorEditColorChanged"),
#endif
		Category("Action")]
		public event EventHandler ColorChanged
		{
			add { Events.AddHandler(ColorChangedEvent, value); }
			remove { Events.RemoveHandler(ColorChangedEvent, value); }
		}
		protected internal new ColorEditProperties Properties {
			get { return base.Properties as ColorEditProperties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new ColorEditProperties(this);
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new ColorEditControl(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientColorEdit";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!AllowNull)
				stb.Append(localVarName + ".allowNull = false;\n");
			if(ColorOnError != ColorOnError.Undo)
				stb.Append(localVarName + ".ColorOnError = " + HtmlConvertor.ToScript(GetColorOnErrorCode()) + ";\n");
			if(!Color.IsEmpty)
				stb.AppendFormat("{0}.color = '{1}';\n", localVarName, ColorUtils.ToHexColor(Color));
			if(IsAutomaticColorSelected)
				stb.Append(localVarName + ".isAutomaticColorSelected = true;\n");
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxColorEdit), ColorEditScriptResourceName);
		}
		protected internal AppearanceStyleBase GetColorTableStyle() {
			return RenderStyles.ColorTable;
		}
		protected internal ColorTableCellStyle GetColorTableCellStyle() {
			return RenderStyles.ColorTableCell;
		}
		protected internal AppearanceStyleBase GetColorPickerStyle() {
			return RenderStyles.ColorPicker;
		}
		protected internal AppearanceStyleBase GetColorIndicatorStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(DefaultColorIndicatorStyle);
			style.CopyFrom(RenderStyles.ColorIndicator);
			return style;
		}
		protected void OnColorChanged(EventArgs e) {
			EventHandler handler = Events[ColorChangedEvent] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnColorChanged(EventArgs.Empty);
		}
		protected internal string GetColorIndicatorID() {
			return ColorIndicatorIdPostfix;
		}
		protected internal Unit GetColorIndicatorHeight() {
			return Properties.GetColorIndicatorHeight(ColorIndicatorHeight, GetColorIndicatorStyle());
		}
		protected internal Unit GetColorIndicatorWidth() {
			return Properties.GetColorIndicatorWidth(ColorIndicatorWidth, GetColorIndicatorStyle());
		}
		protected internal string GetHexColor() {
			return ColorUtils.ToHexColor(Color);
		}
		protected internal override bool IsNeedItemImageCell {
			get { return true; }
		}
		protected internal int GetColumnCountForRender() {
			if(Items.IsEmpty)
				return ColumnCount <= ColorTable.DefaultColorTableItems.Length ? ColumnCount : ColorTable.DefaultColorTableItems.Length;
			return ColumnCount <= Items.Count ? ColumnCount : Items.Count;
		}
		protected override string GetClientObjectStateInputID() {
			return UniqueID + "$State";
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(ClientObjectState != null)
				LoadClientState(GetClientObjectStateValue<string>("colors"));
			return base.LoadPostData(postCollection);
		}
		protected internal override void LoadClientState(string state) {
			Properties.ColorNestedControlProperties.DeserializeColorsToCustomColorTableItems(state);
		}
		protected internal override string SaveClientState() {
			return Properties.ColorNestedControlProperties.GetSerializedCustomColorTableItems();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal string GetColorOnErrorCode() {
			switch (ColorOnError) {
				case ColorOnError.Null:
					return "n";
				default:
					return "u";
			}
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.ColorEditCommonFormDesigner"; } }
	}
}
