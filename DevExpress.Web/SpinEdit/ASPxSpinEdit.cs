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
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum SpinEditNumberType {Integer, Float};
	public enum SpinEditNumberFormat { Custom, Currency, Percent, Number };
	public class SpinEditProperties : SpinEditPropertiesBase {
		public SpinEditProperties()
			: base() {
		}
		public SpinEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool AllowNull {
			get { return GetBoolProperty("AllowNull", true); }
			set { SetBoolProperty("AllowNull", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesDisplayFormatString"),
#endif
		NotifyParentProperty(true), DefaultValue("d"), AutoFormatDisable]
		public override string DisplayFormatString {
			get {
				if(NumberFormat == SpinEditNumberFormat.Custom)
					return base.DisplayFormatString;
				else
					return GetFormatStringByFormat(NumberFormat);
			}
			set {
				if(!String.IsNullOrEmpty(value) && DisplayFormatString != value)
					NumberFormat = SpinEditNumberFormat.Custom;
				base.DisplayFormatString = value;
			}
		}
		protected override string DefaultDisplayFormatString {
			get { return "d"; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesNumberFormat"),
#endif
		NotifyParentProperty(true), DefaultValue(SpinEditNumberFormat.Number), AutoFormatDisable]
		public SpinEditNumberFormat NumberFormat {
			get { return (SpinEditNumberFormat)GetEnumProperty("NumberFormatType", SpinEditNumberFormat.Number); }
			set { SetEnumProperty("NumberFormatType", SpinEditNumberFormat.Number, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesDecimalPlaces"),
#endif
		NotifyParentProperty(true), DefaultValue(0), AutoFormatDisable]
		public int DecimalPlaces {
			get { return GetIntProperty("DecimalPlaces", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "DecimalPlaces");
				SetIntProperty("DecimalPlaces", 0, value);
				if(SpinEdit != null)
					SpinEdit.Value = SpinEdit.Value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesIncrement"),
#endif
		DefaultValue(typeof(Decimal), "1"), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Decimal Increment {
			get { return GetDecimalProperty("Increment", 1); }
			set { SetDecimalProperty("Increment", 1, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesLargeIncrement"),
#endif
		DefaultValue(typeof(Decimal), "10"), NotifyParentProperty(true), AutoFormatDisable]
		public Decimal LargeIncrement {
			get { return GetDecimalProperty("LargeIncrement", 10); }
			set { SetDecimalProperty("LargeIncrement", 10, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesMaxValue"),
#endif
		DefaultValue(typeof(Decimal), "0"), NotifyParentProperty(true), AutoFormatDisable]
		public Decimal MaxValue {
			get { return GetDecimalProperty("MaxValue", 0); }
			set { SetDecimalProperty("MaxValue", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesMinValue"),
#endif
		DefaultValue(typeof(Decimal), "0"), NotifyParentProperty(true), AutoFormatDisable]
		public Decimal MinValue {
			get { return GetDecimalProperty("MinValue", 0); }
			set { SetDecimalProperty("MinValue", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesNumberType"),
#endif
		DefaultValue(SpinEditNumberType.Float), NotifyParentProperty(true), AutoFormatDisable]
		public SpinEditNumberType NumberType {
			get { return (SpinEditNumberType)GetEnumProperty("NumberType", SpinEditNumberType.Float); }
			set { SetEnumProperty("NumberType", SpinEditNumberType.Float, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesShowOutOfRangeWarning"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool ShowOutOfRangeWarning {
			get { return GetBoolProperty("ShowOutOfRangeWarning", true); }
			set { SetBoolProperty("ShowOutOfRangeWarning", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesSpinButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatEnable]
		public SpinButtons SpinButtons {
			get { return SpinButtonsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesLargeIncrementButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle LargeIncrementButtonStyle {
			get { return Styles.SpinEditLargeIncrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesLargeDecrementButtonStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle LargeDecrementButtonStyle {
			get { return Styles.SpinEditLargeDecrementButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SpinEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable,
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public new SpinEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as SpinEditClientSideEvents; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			SpinEditProperties src = source as SpinEditProperties;
			if (src != null) {
				LargeIncrement = src.LargeIncrement;
				MaxValue = src.MaxValue;
				MinValue = src.MinValue;
				AllowNull = src.AllowNull;
				DecimalPlaces = src.DecimalPlaces;
				NumberType = src.NumberType;
				Increment = src.Increment;
				NumberFormat = src.NumberFormat;
				ShowOutOfRangeWarning = src.ShowOutOfRangeWarning;
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxSpinEdit();
		}
		protected internal ASPxSpinEdit SpinEdit {
			get { return Owner as ASPxSpinEdit; }
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new SpinEditClientSideEvents(this);
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if (args.DisplayText == null && SpinEditUtils.IsReal(args.EditValue)) {
				Decimal number = SpinEditUtils.GetDecimalByValue(args.EditValue, DecimalPlaces);			   
				if (number == Decimal.MinValue)
					args.EditValue = null;
				else
					args.EditValue = number;
			}
			return base.GetDisplayTextCore(args, encode);
		}
		protected string GetFormatStringByFormat(SpinEditNumberFormat formatType) {
			string ret = "";
			switch (formatType) {
				case SpinEditNumberFormat.Number:
					ret = "g";
					break;
				case SpinEditNumberFormat.Currency:
					ret = "c";
					break;
				case SpinEditNumberFormat.Percent:
					ret = "{0}%";
					break;
			}
			return ret;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxSpinEdit runat=\"server\" Number=\"0\" />"),
	DefaultProperty("Number"), DefaultEvent("NumberChanged"), ControlValueProperty("Number"),
	ValidationProperty("Number"), DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxSpinEdit.bmp")]
	public class ASPxSpinEdit : ASPxSpinEditBase {
		private const string ClientNumberDecimalSeparatorKey = "ClientNumberDecimalSeparatorKey";
		private static readonly object NumberChangedEvent = new object();
		internal bool lockClientValueChanged = false;
		public ASPxSpinEdit()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditIncrement"),
#endif
		DefaultValue(typeof(Decimal), "1"), AutoFormatDisable]
		public Decimal Increment {
			get { return Properties.Increment; }
			set { Properties.Increment = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditLargeIncrement"),
#endif
		DefaultValue(typeof(Decimal), "10"), AutoFormatDisable]
		public Decimal LargeIncrement {
			get { return Properties.LargeIncrement; }
			set { Properties.LargeIncrement = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditMaxValue"),
#endif
		DefaultValue(typeof(Decimal), "0"), AutoFormatDisable]
		public Decimal MaxValue {
			get { return Properties.MaxValue; }
			set {
				Properties.MaxValue = value;
				if(DesignMode && !IsLoading())
					CheckRestrictions();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditMinValue"),
#endif
		DefaultValue(typeof(Decimal), "0"), AutoFormatDisable]
		public Decimal MinValue {
			get { return Properties.MinValue; }
			set { 
				Properties.MinValue = value;
				if(DesignMode && !IsLoading()) 
					CheckRestrictions();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditShowOutOfRangeWarning"),
#endif
		DefaultValue(true), AutoFormatDisable]
		public bool ShowOutOfRangeWarning {
			get { return Properties.ShowOutOfRangeWarning; }
			set { Properties.ShowOutOfRangeWarning = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditAllowNull"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool AllowNull {
			get { return Properties.AllowNull; }
			set { Properties.AllowNull = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditDecimalPlaces"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int DecimalPlaces {
			get { return Properties.DecimalPlaces; }
			set { Properties.DecimalPlaces = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditNumberType"),
#endif
		DefaultValue(SpinEditNumberType.Float), AutoFormatDisable]
		public SpinEditNumberType NumberType {
			get { return Properties.NumberType; }
			set { Properties.NumberType = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditSpinButtons"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public SpinButtons SpinButtons {
			get { return Properties.SpinButtons; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditLargeIncrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle LargeIncrementButtonStyle {
			get { return Properties.LargeIncrementButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditLargeDecrementButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpinButtonStyle LargeDecrementButtonStyle {
			get { return Properties.LargeDecrementButtonStyle; }
		}
		bool ShouldSerializeNumber() {
			return Number != Decimal.MinValue;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditNumber"),
#endif
		Localizable(true), DefaultValue(typeof(Decimal), "0"),
		TypeConverter(typeof(DecimalNumberConverter)),
		Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public Decimal Number {
			get { return (Value is Decimal) ? (Decimal)Value : Decimal.MinValue; }
			set {
				if(value != Decimal.MinValue)
					Value = value;
				else
					Value = null;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(false), AutoFormatDisable, Localizable(false)]
		public override string Text {
			get {
				if (Value is Decimal) {
					if (Number == Decimal.MinValue)
						return "";
					return Number.ToString(CultureInfo.CurrentCulture);
				}
				return base.Text;
			}
			set {
				Value = SpinEditUtils.GetDecimalByValue(value, DecimalPlaces);
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxSpinEditValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set {
				Decimal number;
				if(!CommonUtils.IsNullValue(value) &&
					SpinEditUtils.TryParseToDecimal(value, DecimalPlaces, out number))
					base.Value = number;
				else
					base.Value = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new SpinEditClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new SpinEditProperties Properties {
			get { return base.Properties as SpinEditProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxSpinEditNumberChanged"),
#endif
		Category("Action")]
		public event EventHandler NumberChanged
		{
			add { Events.AddHandler(NumberChangedEvent, value); }
			remove { Events.RemoveHandler(NumberChangedEvent, value); }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new SpinEditProperties(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientSpinEdit";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!AllowNull)
				stb.Append(localVarName + ".allowNull = false;\n");
			if (IsValueNull())
				stb.Append(localVarName + ".number = null;\n");
			else
				stb.Append(localVarName + ".number = " + HtmlConvertor.ToScript(Number) + ";\n");
			if(Increment != 1)
				stb.Append(localVarName + ".inc = " + HtmlConvertor.ToScript(Increment) + ";\n");
			if (LargeIncrement != 10)
				stb.Append(localVarName + ".largeInc = " + HtmlConvertor.ToScript(LargeIncrement) + ";\n");
			if (MinValue != 0)
				stb.Append(localVarName + ".minValue = " + HtmlConvertor.ToScript(MinValue) + ";\n");
			if (MaxValue != 0)
				stb.Append(localVarName + ".maxValue = " + HtmlConvertor.ToScript(MaxValue) + ";\n");
			if (MaxLength != 0)
				stb.Append(localVarName + ".maxLength = " + HtmlConvertor.ToScript(MaxLength) + ";\n");
			if (DecimalPlaces != 0) {
				stb.Append(localVarName + ".decimalPlaces = " + HtmlConvertor.ToScript(DecimalPlaces) + ";\n");
			}
			if (NumberType == SpinEditNumberType.Integer)
				stb.Append(localVarName + ".numberType = \"i\";\n");
			if(this.lockClientValueChanged)
				stb.Append(localVarName + ".lockValueChanged = true;\n");
		}
		protected override bool IsOutOfRangeWarningSupported() {
			return true;
		}
		protected override bool IsOutOfRangeWarningMode() {
			return ShowOutOfRangeWarning;
		}
		protected override string GetInvalidValueRangeWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidSpinEditRange);
		}
		protected override string GetInvalidMinValueWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidSpinEditMinValue);
		}
		protected override string GetInvalidMaxValueWarningMessage() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.InvalidSpinEditMaxValue);
		}
		private void RegisterClientNumberDecimalSeparator() {
			RegisterScriptBlock(ClientNumberDecimalSeparatorKey, RenderUtils.GetScriptHtml(string.Format("ASPx.NumberDecimalSeparator = {0};\n",
				HtmlConvertor.ToScript(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))));
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterClientNumberDecimalSeparator();
		}
		protected void OnNumberChanged(EventArgs e) {
			EventHandler handler = Events[NumberChangedEvent] as EventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnNumberChanged(EventArgs.Empty);
		}
		protected override object GetPostBackValue(string controlName, System.Collections.Specialized.NameValueCollection postCollection) {
			object value = base.GetPostBackValue(controlName, postCollection);
			if (value == null || object.Equals(value, ""))
				return null;
			return SpinEditUtils.GetDecimalByValue(value, DecimalPlaces);
		}
		protected override bool IsPostBackValueSecure(object value) {
			bool result = base.IsPostBackValueSecure(value);
			if (value != null) {
				decimal decimalValue = (decimal)value;
				if (UseRestrictions())
					result = result && decimalValue >= MinValue && decimalValue <= MaxValue;
			}
			return result;
		}
		protected internal override void ValidateProperties() {
			CheckRestrictions();
		}
		protected void CheckRestrictions() {
			if(!UseRestrictions()) return;
			if(MaxValue < MinValue)
				throw new ArgumentException(string.Format(StringResources.ASPxEdit_InvalidRange, "MaxValue", "MinValue"));
		}
		protected internal bool IsValueNull() {
			return Number == Decimal.MinValue;
		}
		protected override EditButtonStyle GetSpinButtonStyle(SpinButtonExtended button) {
			EditButtonStyle style = base.GetSpinButtonStyle(button);
			switch(button.ButtonKind) {
				case SpinButtonKind.LargeIncrement:
					style.CopyFrom(GetLargeIncrementButtonStyle());
					break;
				case SpinButtonKind.LargeDecrement:
					style.CopyFrom(GetLargeDecrementButtonStyle());
					break;
			}
			return style;
		}
		protected EditButtonStyle GetLargeIncrementButtonStyle() {
			EditButtonStyle ret = new EditButtonStyle();
			ret.CopyFrom(RenderStyles.GetDefaultSpinLargeIncrementButtonStyle());
			ret.CopyFrom(RenderStyles.SpinEditLargeIncrementButton);
			return ret;
		}
		protected EditButtonStyle GetLargeDecrementButtonStyle() {
			EditButtonStyle ret = new EditButtonStyle();
			ret.CopyFrom(RenderStyles.GetDefaultSpinLargeDecrementButtonStyle());
			ret.CopyFrom(RenderStyles.SpinEditLargeDecrementButton);
			return ret;
		}
		protected internal bool UseRestrictions() {
			return Properties.MaxValue != 0 || Properties.MinValue != 0;
		}
		protected internal Decimal GetMaxValue() {
			if (!UseRestrictions())
				return NumberType == SpinEditNumberType.Integer ? Int32.MaxValue : Decimal.MaxValue;
			return MaxValue;
		}
		protected internal Decimal GetMinValue() {
			if (!UseRestrictions())
				return NumberType == SpinEditNumberType.Integer ? Int32.MinValue : Decimal.MinValue;
			return MinValue;
		}		
	}
}
namespace DevExpress.Web.Internal {
	public class DecimalNumberConverter : DecimalConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (SpinEditUtils.IsNumeric(value)) {
				Decimal number = SpinEditUtils.GetDecimalByValue(value);
				if (number == Decimal.MinValue)
					return "";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string) {
				string s = ((string)value).Trim();
				if (s.Length == 0)
					return Decimal.MinValue;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class SpinEditUtils {
		public static Decimal GetDecimalByValue(object value) {
			return GetDecimalByValue(value, 0);
		}
		public static Decimal GetDecimalByValue(object value, int decimalPlaces) {
			if (value is Decimal)
				return (Decimal)value;
			else if (CommonUtils.IsNullValue(value))
				return Decimal.MinValue;
			else {
				Decimal number = Decimal.MinValue;
				if(!TryParseToDecimal(value, decimalPlaces, out number))
					return Decimal.MinValue;
				return number;
			}
		}
		public static bool TryParseToDecimal(object value, out Decimal number) {
			return TryParseToDecimal(value, 0, out number);
		}
		public static bool TryParseToDecimal(object value, int decimalPlaces, out Decimal number) {
			bool ret = false;
			number = 0;
			try {
				number = Convert.ToDecimal(value);
				if(decimalPlaces > 0)
					number = Math.Round(number, decimalPlaces);
				ret = true;
			} catch (Exception e) {
				if(!(e is FormatException || e is OverflowException)) throw;
			}
			return ret;
		}
		public static bool IsNumeric(object value) {
			return IsReal(value) || IsIntegral(value);
		}
		public static bool IsReal(object value) {
			return value is decimal || value is float || value is double;
		}
		public static bool IsIntegral(object value) {
			return value is int || value is short || value is ushort 
				|| value is uint || value is long || value is ulong 
				|| value is char || value is sbyte || value is byte;
		}
	}
}
