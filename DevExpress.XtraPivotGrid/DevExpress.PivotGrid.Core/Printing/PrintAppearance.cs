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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Utils.Design;
namespace DevExpress.PivotGrid.Printing {
	public class PrintTextOptions {
		static StringAlignment toStringAlignment(VertAlignment textVAlignment) {
			if(textVAlignment == VertAlignment.Center)
				return StringAlignment.Center;
			else if(textVAlignment == VertAlignment.Top)
				return StringAlignment.Near;
			else if(textVAlignment == VertAlignment.Bottom)
				return StringAlignment.Far;
			else
				return StringAlignment.Center;
		}
		static StringAlignment toStringAlignment(HorzAlignment textHAlignment) {
			if(textHAlignment == HorzAlignment.Center)
				return StringAlignment.Center;
			else if(textHAlignment == HorzAlignment.Far)
				return StringAlignment.Far;
			return StringAlignment.Near;
		}
		HorzAlignment hAlignment = HorzAlignment.Default;
		VertAlignment vAlignment = VertAlignment.Default;
		StringFormat format;
		StringTrimming trimming;
		WordWrap wordWrap = WordWrap.NoWrap;
		public PrintTextOptions() {
			Reset();
		}
		internal void Assign(PrintTextOptions value) {
			HAlignment = value.HAlignment;
			VAlignment = value.VAlignment;
			Trimming = value.trimming;
			WordWrap = value.WordWrap;
		}
		public void Reset() {
			HAlignment = HorzAlignment.Default;
			VAlignment = VertAlignment.Default;
			Trimming = StringTrimming.None;
			WordWrap = WordWrap.NoWrap;
		}
		[NotifyParentProperty(true), XtraSerializableProperty()]
		public WordWrap WordWrap {
			get {
				return wordWrap;
			}
			set {
				if(wordWrap != value) {
					wordWrap = value;
					ClearStringFormat();
				}
			}
		}
		[NotifyParentProperty(true), XtraSerializableProperty()]
		public StringTrimming Trimming {
			get {
				return trimming;
			}
			set {
				if(trimming != value) {
					trimming = value;
					ClearStringFormat();
				}
			}
		}
		internal bool ShouldSerialize() { 
			return ShouldSerializeHAlignment() ||
				ShouldSerializeVAlignment() ||
				WordWrap == WordWrap.Wrap;
		}
		internal StringFormat StringFormat {
			get {
				if(format == null) {
					format = new StringFormat(StringFormat.GenericTypographic);
					format.FormatFlags = WordWrap == WordWrap.Wrap ? 0 : StringFormatFlags.NoWrap;
					format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
					format.Alignment = toStringAlignment(HAlignment);
					format.LineAlignment = toStringAlignment(VAlignment);
					format.Trimming = Trimming;
				}
				return format;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintTextOptionsHAlignment"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty(), AutoFormatEnable() 
		]
		public virtual HorzAlignment HAlignment {
			get { return hAlignment; }
			set { 
				if(hAlignment != value) {
					hAlignment = value;
					ClearStringFormat();
				} 
			}
		}
		bool ShouldSerializeHAlignment() { return hAlignment != HorzAlignment.Default; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintTextOptionsHAlignment"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty(), AutoFormatEnable()
		]
		public virtual VertAlignment VAlignment {
			get { return vAlignment; }
			set {
				if(vAlignment != value) {
					vAlignment = value;
					ClearStringFormat();
				}
			}
		}
		internal bool ShouldSerializeVAlignment() { return vAlignment != VertAlignment.Default; }
		protected void ClearStringFormat() {
			if(format != null) {
				format.Dispose();
				format = null;
			}
		}
		internal void Merge(PrintTextOptions textOptions) {
			if(HAlignment == HorzAlignment.Default)
				HAlignment = textOptions.HAlignment;
			if(VAlignment == VertAlignment.Default)
				VAlignment = textOptions.VAlignment;
			if(WordWrap == WordWrap.NoWrap)
				WordWrap = textOptions.WordWrap;
			if(Trimming == StringTrimming.None)
				Trimming = textOptions.Trimming;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PrintAppearanceObject : IPivotPrintAppearance, IPivotPrintAppearanceOptions {
		PrintTextOptions textOptions;
		public PrintAppearanceObject(){
			textOptions = new PrintTextOptions();
			ResetBorderColor();
		}
		public override string ToString() { return string.Empty; }
		#region Properties
		Color backColor;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectBackColor")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearanceObject.BackColor")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		void ResetBackColor() { BackColor = Color.Empty; }
	 	internal bool ShouldSerializeBackColor() { return BackColor != Color.Empty; }
		[Browsable(false)]
		public bool IsSetBackColor {
			get { return ShouldSerializeBackColor(); }
		}
		Color borderColor;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectBorderColor")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearanceObject.BorderColor")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		void ResetBorderColor() { BorderColor = Color.Empty; }
		internal bool ShouldSerializeBorderColor() { return BorderColor != Color.Empty; }
		[Browsable(false)]
		public bool IsSetBorderColor {
			get { return ShouldSerializeBorderColor(); }
		}
		static Font defaultFont = null;
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectDefaultFont"),
#endif
 NotifyParentProperty(true)]
		public static Font DefaultFont {
			get {
				if(defaultFont == null) defaultFont = CreateDefaultFont();
				return defaultFont;
			}
			set {
				if(value == null) value = CreateDefaultFont();
				defaultFont = value;
			}
		}
		static Font CreateDefaultFont() {
			return new Font(new FontFamily("Tahoma"), SystemFonts.DefaultFont.Size);
		}
		protected virtual Font InnerDefaultFont { get { return DefaultFont; } }
		Font font;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectFont")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearanceObject.Font")]
		[TypeConverter(typeof(FontTypeConverter))]
		[NotifyParentProperty(true), XtraSerializableProperty]
		[RefreshProperties(RefreshProperties.All)]
		public Font Font {
			get { return font ?? InnerDefaultFont; }
			set {
				if(Font == value) return;
				font = value;
			}
		}
		void ResetFont() { Font = null; }
		bool ShouldSerializeFont() { return font != null && !font.Equals(InnerDefaultFont); }
		[Browsable(false)]
		public bool IsSetFont {
			get { return ShouldSerializeFont(); }
		}
		Color foreColor;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectForeColor")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearanceObject.ForeColor")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		public Color ForeColor { get { return foreColor; } set { foreColor = value; } }
		void ResetForeColor() { ForeColor = Color.Empty; }
		internal bool ShouldSerializeForeColor() { return ForeColor != Color.Empty; }
		[Browsable(false)]
		public bool IsSetForeColor {
			get { return ShouldSerializeForeColor(); }
		}
		static float defaultBorderWidth = 1.0f;
		float borderWidth = defaultBorderWidth;
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceObjectBorderWidth")]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearanceObject.BorderWidth")]
		[NotifyParentProperty(true), XtraSerializableProperty]
		[RefreshProperties(RefreshProperties.All)]
		public float BorderWidth {
			get { return borderWidth; }
			set {
				if(BorderWidth == value) return;
				borderWidth = value;
			}
		}
		void ResetBorderWidth() { BorderWidth = defaultBorderWidth; }
		bool ShouldSerializeBorderWidth() { return borderWidth != defaultBorderWidth; }
		[Browsable(false)]
		public bool IsSetBorderWidth {
			get { return ShouldSerializeBorderWidth(); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("This property is retained only for backward compatibility.")]
		public Color BackColor2 { get { return Color.Empty; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("This property is retained only for backward compatibility.")]
		public LinearGradientMode GradientMode { get { return LinearGradientMode.Horizontal; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Obsolete("This property is retained only for backward compatibility.")]
		public Image Image { get { return null; } set { } }
		#endregion
		#region Serialization
		public bool ShouldSerialize() {
			return ShouldSerializeBackColor() || ShouldSerializeBorderColor() || ShouldSerializeFont() || ShouldSerializeForeColor() || textOptions.ShouldSerialize();
		}
		public virtual void Reset() {
			BackColor = Color.Empty;
			BorderColor = Color.Empty;
			Font = null;
			ForeColor = Color.Empty;
			textOptions.Reset();
		}
		#endregion
		#region IPivotPrintAppearance
		Color IPivotPrintAppearance.BorderColor {
			get { return BorderColor; }
		}
		Color IPivotPrintAppearance.ForeColor {
			get { return ForeColor; }
		}
		Color IPivotPrintAppearance.BackColor {
			get { return BackColor; }
		}
		Font IPivotPrintAppearance.Font {
			get { return Font; }
		}
		float IPivotPrintAppearance.BorderWidth {
			get { return BorderWidth; }
		}
		XtraPrinting.BrickBorderStyle IPivotPrintAppearance.BorderStyle {
			get { return  XtraPrinting.BrickBorderStyle.Center; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)
		]
		public virtual PrintTextOptions TextOptions {
			get { return textOptions; }
		}
		[NotifyParentProperty(true), DefaultValue(HorzAlignment.Default)]
		public virtual HorzAlignment TextHorizontalAlignment {
			get { return textOptions.HAlignment; }
			set {
				textOptions.HAlignment = value;
			}
		}
		[NotifyParentProperty(true), DefaultValue(VertAlignment.Default)]
		public virtual VertAlignment TextVerticalAlignment {
			get { return textOptions.VAlignment; }
			set {
				textOptions.VAlignment = value;
			}
		}
		[NotifyParentProperty(true), DefaultValue(false)]
		public virtual bool WordWrap {
			get { return textOptions.WordWrap == DevExpress.Utils.WordWrap.Wrap; }
			set { textOptions.WordWrap = value ? DevExpress.Utils.WordWrap.Wrap : DevExpress.Utils.WordWrap.NoWrap;  }
		}
		[NotifyParentProperty(true), DefaultValue(StringTrimming.None)]
		public virtual StringTrimming Trimming {
			get { return textOptions.Trimming; }
			set { textOptions.Trimming = value; }
		}
		StringFormat IPivotPrintAppearance.StringFormat {
			get {
				return textOptions.StringFormat;
			}
		}
		HorzAlignment IPivotPrintAppearance.TextHorizontalAlignment {
			get { return textOptions.HAlignment; }
			set { textOptions.HAlignment = value; }
		}
		VertAlignment IPivotPrintAppearance.TextVerticalAlignment {
			get { return textOptions.VAlignment; }
			set { textOptions.VAlignment = value; }
		}
		IPivotPrintAppearanceOptions IPivotPrintAppearance.Options {
			get { return this; }
		}
		object ICloneable.Clone() {
			PrintAppearanceObject appearance = CreateNewInstance();
			Assign(appearance);
			return appearance;
		}
		protected virtual PrintAppearanceObject CreateNewInstance() {
			return new PrintAppearanceObject();
		}
		#endregion
		#region IPivotPrintAppearanceOptions
		bool IPivotPrintAppearanceOptions.UseTextOptions {
			get { return textOptions.ShouldSerialize(); }
		}
		bool IPivotPrintAppearanceOptions.UseBorderColor {
			get { return ShouldSerializeBorderColor(); }
		}
		bool IPivotPrintAppearanceOptions.UseForeColor {
			get { return ShouldSerializeForeColor(); }
		}
		bool IPivotPrintAppearanceOptions.UseBackColor {
			get { return ShouldSerializeBackColor(); }
		}
		bool IPivotPrintAppearanceOptions.UseFont {
			get { return this.ShouldSerializeFont(); }
		}
		bool IPivotPrintAppearanceOptions.UseBorderWidth {
			get { return this.ShouldSerializeBorderWidth(); }
		}
		bool IPivotPrintAppearanceOptions.UseBorderStyle {
			get { return false; }
		}
		#endregion
		protected internal void Assign(PrintAppearanceObject appearance) {
			backColor = appearance.backColor;
			borderColor = appearance.borderColor;
			font = appearance.font;
			foreColor = appearance.foreColor;
			textOptions.Assign(appearance.textOptions);
		}
		protected internal void Merge(PrintAppearanceObject appearance) {
			if(appearance.ShouldSerializeBackColor() && !ShouldSerializeBackColor())
				backColor = appearance.backColor;
			if(appearance.ShouldSerializeBorderColor() && !ShouldSerializeBorderColor())
				borderColor = appearance.borderColor;
			if(appearance.ShouldSerializeFont() && !ShouldSerializeFont())
				font = appearance.font;
			if(appearance.ShouldSerializeForeColor() && !ShouldSerializeForeColor())
				foreColor = appearance.foreColor;
			textOptions.Merge(appearance.TextOptions);
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PrintAppearance {
		PrintAppearanceObject Get(ref PrintAppearanceObject obj) {
			if(obj == null) obj = CreateAppearanceObject();
			return obj;
		}
		void Set(ref PrintAppearanceObject cell, PrintAppearanceObject value) {
			if(value == null) cell = CreateAppearanceObject();
			else cell = value;
		}
		protected virtual PrintAppearanceObject CreateAppearanceObject() {
			return new PrintAppearanceObject();
		}
		public override string ToString() { return string.Empty; }
		PrintAppearanceObject cell;
		bool ShouldSerializeCell() { return Cell.ShouldSerialize(); }
		void ResetCell() { Cell.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceCell"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.Cell")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject Cell { get { return Get(ref cell); } set { Set(ref cell, value); } }		
		PrintAppearanceObject fieldHeader;
		bool ShouldSerializeFieldHeader() { return FieldHeader.ShouldSerialize(); }
		void ResetFieldHeader() { FieldHeader.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceFieldHeader"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.FieldHeader")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject FieldHeader { get { return Get(ref fieldHeader); } set { Set(ref fieldHeader, value); } }
		PrintAppearanceObject totalCell;
		bool ShouldSerializeTotalCell() { return TotalCell.ShouldSerialize(); }
		void ResetTotalCell() { TotalCell.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceTotalCell"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.TotalCell")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject TotalCell { get { return Get(ref totalCell); } set { Set(ref totalCell, value); } }
		PrintAppearanceObject grandTotalCell;
		bool ShouldSerializeGrandTotalCell() { return GrandTotalCell.ShouldSerialize(); }
		void ResetGrandTotalCell() { GrandTotalCell.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceGrandTotalCell"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.GrandTotalCell")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject GrandTotalCell { get { return Get(ref grandTotalCell); } set { Set(ref grandTotalCell, value); } }
		PrintAppearanceObject customTotalCell;
		bool ShouldSerializeCustomTotalCell() { return CustomTotalCell.ShouldSerialize(); }
		void ResetCustomTotalCell() { CustomTotalCell.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceCustomTotalCell"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.CustomTotalCell")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject CustomTotalCell { get { return Get(ref customTotalCell); } set { Set(ref customTotalCell, value); } }
		PrintAppearanceObject fieldValue;
		bool ShouldSerializeFieldValue() { return FieldValue.ShouldSerialize(); }
		void ResetFieldValue() { FieldValue.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceFieldValue"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.FieldValue")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject FieldValue { get { return Get(ref fieldValue); } set { Set(ref fieldValue, value); } }
		PrintAppearanceObject fieldValueTotal;
		bool ShouldSerializeFieldValueTotal() { return FieldValueTotal.ShouldSerialize(); }
		void ResetFieldValueTotal() { FieldValueTotal.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceFieldValueTotal"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.FieldValueTotal")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject FieldValueTotal { get { return Get(ref fieldValueTotal); } set { Set(ref fieldValueTotal, value); } }
		PrintAppearanceObject fieldValueGrandTotal;
		bool ShouldSerializeFieldValueGrandTotal() { return FieldValueGrandTotal.ShouldSerialize(); }
		void ResetFieldValueGrandTotal() { FieldValueGrandTotal.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceFieldValueGrandTotal"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.FieldValueGrandTotal")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject FieldValueGrandTotal { get { return Get(ref fieldValueGrandTotal); } set { Set(ref fieldValueGrandTotal, value); } }
		PrintAppearanceObject lines;
		bool ShouldSerializeLines() { return Lines.ShouldSerialize(); }
		void ResetLines() { Lines.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PrintAppearanceLines"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.PivotGrid.Printing.PrintAppearance.Lines")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content), AutoFormatEnable()]
		public PrintAppearanceObject Lines { get { return Get(ref lines); } set { Set(ref lines, value); } }
		public void Reset() {
			MethodInfo[] methods = typeof(PrintAppearance).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0; i < methods.Length; i++) {
				if(!methods[i].Name.StartsWith("Reset") || methods[i].Name == "Reset") continue;
				methods[i].Invoke(this, null);
			}
		}
		public bool ShouldSerialize() {
			MethodInfo[] methods = typeof(PrintAppearance).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
			for(int i = 0; i < methods.Length; i++) {
				if(!methods[i].Name.StartsWith("ShouldSerialize") || methods[i].Name == "ShouldSerialize") continue;
				if((bool)methods[i].Invoke(this, null)) 
					return true;
			}
			return false;
		}
		internal PrintAppearanceObject GetActualFieldValueAppearance(PivotGridValueType valueType, IPrintAppearanceOwner field) {
			PrintAppearanceObject appearance;
			if(field == null) {
			switch(valueType) {
					case PivotGridValueType.Value:
						appearance = Combine(FieldValue);
						break;
				case PivotGridValueType.CustomTotal:
				case PivotGridValueType.Total:
						appearance = Combine(FieldValueTotal, FieldValue);
					break;
				case PivotGridValueType.GrandTotal:
						appearance = Combine(FieldValueGrandTotal, FieldValueTotal, FieldValue);
					break;
					default:
						throw new ArgumentOutOfRangeException("valueType");
				}
			} else {
				switch(valueType) {
				case PivotGridValueType.Value:
						appearance = Combine(field.FieldValue, FieldValue);
						break;
					case PivotGridValueType.CustomTotal:
					case PivotGridValueType.Total:
						appearance = Combine(field.FieldValueTotal, FieldValueTotal, field.FieldValue, FieldValue);
						break;
					case PivotGridValueType.GrandTotal:
						appearance = Combine(field.FieldValueGrandTotal, FieldValueGrandTotal, field.FieldValueTotal, FieldValueTotal, field.FieldValue, FieldValue);
					break;
				default:
					throw new ArgumentOutOfRangeException("valueType");
			}
			}
			SetDefaultFieldValueAppearance(appearance);
			return appearance;
		}
		internal PrintAppearanceObject GetActualFieldAppearance(IPrintAppearanceOwner field) {
			PrintAppearanceObject appearance = field == null ? Combine(FieldHeader) : Combine(field.Field, FieldHeader);
			SetDefaultFieldAppearance(appearance);
			return appearance;
		}
		internal PrintAppearanceObject GetActualCellAppearance(PivotGridCellItem cellItem) {
			PrintAppearanceObject appearance;
			IPrintAppearanceOwner field = cellItem.DataField as IPrintAppearanceOwner;
			if(cellItem.IsGrandTotalAppearance)
				appearance = GetGrandTotalCellAppearance(field);
			else
				if(cellItem.IsTotalAppearance)
					appearance = GetTotalCellAppearance(field);
				else
					appearance = GetActualCellAppearance(field);
			if(cellItem.IsCustomTotalAppearance) {
				if(field != null)
					appearance.Merge(field.CustomTotalCell);
				appearance.Merge(CustomTotalCell);
				if(!cell.ShouldSerializeBackColor())
					appearance.BackColor = Color.LightGray;
			}
			return appearance;
		}
		internal PrintAppearanceObject GetTotalCellAppearance(IPrintAppearanceOwner field) {
			PrintAppearanceObject appearance;
			if(field == null) 
				appearance = Combine(TotalCell, Cell);
			else 
				appearance = Combine(field.TotalCell, TotalCell, field.Cell, Cell);
			SetDefaultCellAppearance(appearance);
			return appearance;
		}
		internal PrintAppearanceObject GetGrandTotalCellAppearance(IPrintAppearanceOwner field) {
			PrintAppearanceObject appearance;
			if(field == null)
				appearance = Combine(GrandTotalCell, TotalCell, Cell);
			else
				appearance = Combine(field.GrandTotalCell, GrandTotalCell, field.TotalCell, TotalCell, field.Cell, Cell);
			SetDefaultCellAppearance(appearance);
			return appearance;
		}
		internal PrintAppearanceObject GetActualCellAppearance(IPrintAppearanceOwner field) {
			PrintAppearanceObject appearance;
			if(field == null)
				appearance = Combine(Cell);
			else
				appearance = Combine(field.Cell, Cell);
			SetDefaultCellAppearance(appearance);
			return appearance;
		}
		PrintAppearanceObject Combine(params PrintAppearanceObject[] appearances) {
			PrintAppearanceObject appearance = CreateAppearanceObject();
			if(appearances.Length > 0)
				appearance.Assign(appearances[0]);
			for(int i = 1; i < appearances.Length; i++)
				appearance.Merge(appearances[i]);
			return appearance;
		}
		void SetDefaultCellAppearance(PrintAppearanceObject appearance) {
			if(!appearance.ShouldSerializeForeColor())
				appearance.ForeColor = Color.FromArgb(255, 0, 0, 0);
			if(!appearance.ShouldSerializeBackColor())
				appearance.BackColor = Color.FromArgb(255, 255, 255, 255);
			SetDefaultBorderColor(appearance);
		}
		void SetDefaultFieldValueAppearance(PrintAppearanceObject appearance) {
			if(!appearance.ShouldSerializeBackColor())
				appearance.BackColor = SystemColors.Control;
			if(!appearance.ShouldSerializeForeColor())
				appearance.ForeColor = SystemColors.ControlText;
			SetDefaultBorderColor(appearance);
		}
		void SetDefaultFieldAppearance(PrintAppearanceObject appearance) {
			if(!appearance.ShouldSerializeForeColor())
				appearance.ForeColor = SystemColors.ControlText;
			if(!appearance.ShouldSerializeBackColor())
				appearance.BackColor = SystemColors.Control;
			SetDefaultBorderColor(appearance);
		}
		void SetDefaultBorderColor(PrintAppearanceObject appearance) {
			if(!appearance.ShouldSerializeBorderColor())
				appearance.BorderColor = Lines.ShouldSerializeForeColor() ? Lines.ForeColor : SystemColors.ControlDark;
		}
	}
	public interface IPrintAppearanceOwner {
		PrintAppearanceObject Cell { get; }
		PrintAppearanceObject TotalCell { get; }
		PrintAppearanceObject CustomTotalCell { get; }
		PrintAppearanceObject GrandTotalCell { get; }
		PrintAppearanceObject FieldValue { get; }
		PrintAppearanceObject FieldValueTotal { get; }
		PrintAppearanceObject FieldValueGrandTotal { get; }
		PrintAppearanceObject Field { get; }
	}
}
