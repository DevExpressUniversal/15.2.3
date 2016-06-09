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
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[ToolboxItem(false)]
	public class ASPxProgressBarBase : ASPxWebControl {
		public const int MaxPosition = 100;
		public const int MinPosition = 0;
		protected internal const string ProgressBarBaseScriptResourceName = WebScriptsResourcePath + "Progress.js";
		private ProgressControl progressControl = null;
		protected ProgressBarSettings settings = null;
		private IndicatorStyle indicatorStyle = null;
		private bool hasOwner;
		public ASPxProgressBarBase(ASPxWebControl owner)
			: this() {
			if(owner == null || owner == this)
				throw new ArgumentException("owner");
			this.hasOwner = true;
		}
		public ASPxProgressBarBase()
			: base() {
			this.indicatorStyle = new IndicatorStyle();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseDisplayFormatString"),
#endif
		AutoFormatDisable, DefaultValue(ProgressBarSettings.DefaultDisplayFormatString)]
		public virtual string DisplayFormatString {
			get { return ProgressBarSettings.DisplayFormatString; }
			set { ProgressBarSettings.DisplayFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseDisplayMode"),
#endif
		AutoFormatDisable, DefaultValue(ProgressBarDisplayMode.Percentage)]
		public virtual ProgressBarDisplayMode DisplayMode {
			get { return ProgressBarSettings.DisplayMode; }
			set { ProgressBarSettings.DisplayMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseShowPosition"),
#endif
		AutoFormatEnable, DefaultValue(true)]
		public virtual bool ShowPosition {
			get { return ProgressBarSettings.ShowPosition; }
			set {
				ProgressBarSettings.ShowPosition = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseMinimum"),
#endif
		AutoFormatDisable, DefaultValue(typeof(Decimal), "0")]
		public Decimal Minimum {
			get { return GetDecimalProperty("Minimum", 0); }
			set { SetDecimalProperty("Minimum", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseMaximum"),
#endif
		AutoFormatDisable, DefaultValue(typeof(Decimal), "100")]
		public Decimal Maximum {
			get { return GetDecimalProperty("Maximum", 100); }
			set { SetDecimalProperty("Maximum", 100, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBasePosition"),
#endif
		AutoFormatDisable, DefaultValue(typeof(Decimal), "0")]
		public Decimal Position {
			get { return GetDecimalProperty("Position", 0); }
			set { SetDecimalProperty("Position", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseCustomDisplayFormat"),
#endif
		AutoFormatDisable]
		public string CustomDisplayFormat
		{
			get { return GetStringProperty("CustomDisplayFormat", string.Empty); }
			set { SetStringProperty("CustomDisplayFormat", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBasePaddings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyleBase)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseIndicatorStyle"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IndicatorStyle IndicatorStyle {
			get { return this.indicatorStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseClientInstanceName"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarBaseClientVisible"),
#endif
		DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		protected internal ProgressBarSettings ProgressBarSettings {
			get {
				if (settings == null)
					settings = CreateProgressBarSettings(this);
				return settings;
			}
		}
		protected override StylesBase CreateStyles() {
			return new ProgressBarStyles(this);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.progressControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.progressControl = new ProgressControl(this);
			Controls.Add(this.progressControl);
		}
		protected internal virtual ProgressBarSettings CreateProgressBarSettings(ASPxProgressBarBase progressBar) {
			return new ProgressBarSettings(progressBar);
		}
		internal string GetMainCellCssClassName() {
			return GetCssClassNamePrefix() + "PBMainCell";
		}
		protected internal IndicatorStyle GetIndicatorStyle() {
			return IndicatorStyle;
		}
		protected internal AppearanceStyleBase GetValueIndicatorStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			MergeDisableStyle(style);
			style.HorizontalAlign = HorizontalAlign.Center;
			style.VerticalAlign = VerticalAlign.Middle;
			return style;
		}
		public override Paddings GetPaddings() {
			return GetControlStyle().Paddings;
		}
		protected internal Unit GetDivIndicatorWidth() {
			if(Width.Type == UnitType.Percentage)
				return Unit.Percentage(GetPercentValue());
			else {
				double width = ConvertToPixels(Width);
				width -= ConvertToPixels(GetPaddings().GetPaddingLeft()) +
						 ConvertToPixels(GetPaddings().GetPaddingRight()) +
						 ConvertToPixels(GetControlStyle().GetBorderWidthLeft()) +
						 ConvertToPixels(GetControlStyle().GetBorderWidthRight()) +
						 ConvertToPixels(GetIndicatorStyle().GetBorderWidthLeft()) +
						 ConvertToPixels(GetIndicatorStyle().GetBorderWidthRight());
				double indicatorWidth = width / 100 * GetPercentValue();
				if(indicatorWidth < 0)
					indicatorWidth = 0;
				return Unit.Pixel((int)indicatorWidth);
			}
		}
		protected internal Unit GetDivIndicatorHeight() {
			double height = GetControlCellHeight().Value;
			height -= ConvertToPixels(GetIndicatorStyle().GetBorderWidthBottom()) +
					  ConvertToPixels(GetIndicatorStyle().GetBorderWidthTop());
			if(height < 0)
				height = 0;
			return Unit.Pixel((int)height);
		}
		protected internal Margins GetValueIndicatorMargins() {
			double height = -GetControlCellHeight().Value;
			return new Margins(Unit.Empty, Unit.Pixel((int)height), Unit.Empty, Unit.Empty);
		}
		protected internal Unit GetControlCellHeight() {
			double height = ConvertToPixels(Height);
			height -= ConvertToPixels(GetPaddings().GetPaddingTop()) +
					  ConvertToPixels(GetPaddings().GetPaddingBottom()) +
					  ConvertToPixels(GetControlStyle().GetBorderWidthBottom()) +
					  ConvertToPixels(GetControlStyle().GetBorderWidthTop());
			if(height < 0)
				height = 0;
			return Unit.Pixel((int)height);
		}
		protected internal double ConvertToPixels(Unit value) {
			UnitType heghtType = value.Type;
			double heightValue = value.Value;
			UnitUtils.ConvertToPixels(ref heghtType, ref heightValue);
			return heightValue;
		}
		protected internal double GetPercentValue() {
			if (Maximum == Minimum)
				return 0;
			double percentValue = (double)(100* (Position - Minimum) / (Maximum - Minimum));
			if (percentValue > MaxPosition)
				percentValue = MaxPosition;
			if (percentValue < MinPosition)
				percentValue = MinPosition;
			return percentValue;
		}
		protected internal string GetIndicatorValueText() {
			if(DisplayMode == ProgressBarDisplayMode.Custom) {
				if(!string.IsNullOrEmpty(DisplayFormatString)) {
					return CustomDisplayFormat
						.Replace("{0}", string.Format(CommonUtils.GetFormatString(DisplayFormatString), Position))
						.Replace("{1}", string.Format(CommonUtils.GetFormatString(DisplayFormatString), Minimum))
						.Replace("{2}", string.Format(CommonUtils.GetFormatString(DisplayFormatString), Maximum));
				}
				else {
					return CustomDisplayFormat
						.Replace("{0}", Position.ToString())
						.Replace("{1}", Minimum.ToString())
						.Replace("{2}", Maximum.ToString());
				}
			}
			string indicatorValue = DisplayMode == ProgressBarDisplayMode.Position ? Position.ToString() : ((int)GetPercentValue()).ToString();
			if(!string.IsNullOrEmpty(DisplayFormatString))
				indicatorValue = string.Format(CommonUtils.GetFormatString(DisplayFormatString), Convert.ToDecimal(indicatorValue));
			if(DisplayMode == ProgressBarDisplayMode.Position)
				return indicatorValue;
			return string.Format(IsRightToLeft() && CultureInfo.CurrentCulture.NumberFormat.PercentPositivePattern == 0
			   ? "{0} %" : "{0}%", indicatorValue);
		}
		protected internal string GetDivIndicatorID() {
			return "DI";
		}
		protected internal string GetValueIndicatorCellID() {
			return "VIC";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientProgressBarBase";
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			if(IsRightToLeft() && CultureInfo.CurrentCulture.NumberFormat.PercentPositivePattern > 0)
				RegisterCultureInfoScript();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterFormatterScript();
			RegisterIncludeScript(typeof(ASPxProgressBarBase), ProgressBarBaseScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendFormat("{0}.minimum = {1};\n", localVarName, HtmlConvertor.ToScript(Minimum));
			stb.AppendFormat("{0}.maximum = {1};\n", localVarName, HtmlConvertor.ToScript(Maximum));
			stb.AppendFormat("{0}.position = {1};\n", localVarName, HtmlConvertor.ToScript(Position));
			stb.AppendFormat("{0}.displayMode = {1};\n", localVarName, (int)DisplayMode);
			if(DisplayMode == ProgressBarDisplayMode.Custom)
				stb.AppendFormat("{0}.customDisplayFormat = {1};\n", localVarName, HtmlConvertor.ToScript(CustomDisplayFormat));
			if(!string.IsNullOrEmpty(DisplayFormatString))
				stb.AppendFormat("{0}.displayFormat={1};\n", localVarName, HtmlConvertor.ToScript(CommonUtils.GetFormatString(DisplayFormatString)));
			if(!this.hasOwner)
				stb.Append(localVarName + ".hasOwner = false;\n");
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyle(GetDisabledStyle(), GetValueIndicatorCellID(), IsEnabled());
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ProgressBarSettings, IndicatorStyle });
		}
	}
}
namespace DevExpress.Web {
	using System.Collections.Specialized;
	using DevExpress.Web.Internal;
	using DevExpress.Web;
	public class ProgressBarProperties : EditPropertiesBase {
		public ProgressBarProperties()
			: this(null) {
		}
		public ProgressBarProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string NullDisplayText {
			get { return base.NullDisplayText; }
			set { base.NullDisplayText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new EditClientSideEventsBase ClientSideEvents {
			get { return (EditClientSideEventsBase)base.ClientSideEvents; }
		}
		[
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		protected internal EditorCaptionSettingsBase CaptionSettings {
			get { return base.CaptionSettingsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesDisplayFormatString"),
#endif
		Category("Settings"), DefaultValue(ProgressBarSettings.DefaultDisplayFormatString)]
		public override string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		protected override string DefaultDisplayFormatString {
			get { return ProgressBarSettings.DefaultDisplayFormatString; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesDisplayMode"),
#endif
		Category("Settings"), DefaultValue(ProgressBarDisplayMode.Percentage), NotifyParentProperty(true), AutoFormatDisable]
		public ProgressBarDisplayMode DisplayMode {
			get { return (ProgressBarDisplayMode)GetEnumProperty("DisplayMode", ProgressBarDisplayMode.Percentage); }
			set {
				if(DisplayMode != value) {
					SetEnumProperty("DisplayMode", ProgressBarDisplayMode.Percentage, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesShowPosition"),
#endif
		Category("Settings"), NotifyParentProperty(true), AutoFormatDisable, DefaultValue(true)]
		public bool ShowPosition {
			get { return GetBoolProperty("ShowPosition", true); }
			set {
				if(ShowPosition != value) {
					SetBoolProperty("ShowPosition", true, value);
					LayoutChanged();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesMaximum"),
#endif
		DefaultValue(typeof(Decimal), "100"), NotifyParentProperty(true), AutoFormatDisable,
		TypeConverter(typeof(DecimalNumberConverter))]
		public Decimal Maximum {
			get { return GetDecimalProperty("Maximum", 100); }
			set { SetDecimalProperty("Maximum", 100, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesMinimum"),
#endif
		DefaultValue(typeof(Decimal), "0"), NotifyParentProperty(true), AutoFormatDisable,
		TypeConverter(typeof(DecimalNumberConverter))]
		public Decimal Minimum {
			get { return GetDecimalProperty("Minimum", 0); }
			set { SetDecimalProperty("Minimum", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesWidth"),
#endif
		DefaultValue(typeof(Unit), ""), Category("Layout"), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesHeight"),
#endif
		DefaultValue(typeof(Unit), ""), Category("Layout"), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesCustomDisplayFormat"),
#endif
		DefaultValue(typeof(string), ""), AutoFormatDisable, NotifyParentProperty(true), Category("Settings")]
		public string CustomDisplayFormat
		{
			get { return GetStringProperty("CustomDisplayFormat", string.Empty); }
			set { SetStringProperty("CustomDisplayFormat", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ProgressBarPropertiesIndicatorStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarIndicatorStyle IndicatorStyle {
			get { return Styles.ProgressBarIndicator; }
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new EditClientSideEventsBase();
		}
		protected override ASPxEditBase CreateEditInstance() {
			ASPxSpinEdit spinEdit = new ASPxSpinEdit();
			spinEdit.NumberType = SpinEditNumberType.Integer;
			spinEdit.MinValue = Minimum;
			spinEdit.MaxValue = Maximum;
			return spinEdit;
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			ASPxProgressBar progressBar = new ASPxProgressBar();
			progressBar.Value = args.EditValue;
			progressBar.Properties.ParentSkinOwner = args.SkinOwner;
			progressBar.Properties.ParentImages = args.Images;
			progressBar.Properties.ParentStyles = args.Styles;
			progressBar.Properties.Assign(this);
			if(progressBar.Width.IsEmpty)
				progressBar.Width = Unit.Percentage(100);
			progressBar.SetOwnerControl(args.SkinOwner as ASPxWebControl);
			return progressBar;
		}
		public override HorizontalAlign GetDisplayControlDefaultAlign() {
			return HorizontalAlign.Left;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ProgressBarProperties src = source as ProgressBarProperties;
				if(src != null) {
					CustomDisplayFormat = src.CustomDisplayFormat;
					Height = src.Height;
					Width = src.Width;
					Maximum = src.Maximum;
					Minimum = src.Minimum;
					DisplayMode = src.DisplayMode;
					ShowPosition = src.ShowPosition;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxProgressBar Width=\"200px\" Position=\"50\" runat=\"server\"></{0}:ASPxProgressBar>"),
	Designer("DevExpress.Web.Design.ASPxProgressBarDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxProgressBar.bmp"),
	DefaultProperty("Position"), ControlValueProperty("Position"),
	DataBindingHandler("DevExpress.Web.Design.ProgressBarDataBindingHandler, " + AssemblyInfo.SRAssemblyWebDesignFull)
]
	public class ASPxProgressBar : ASPxEditBase {
		protected internal const string ProgressBarScriptResourceName = EditScriptsResourcePath + "ProgressBar.js";
		private ProgressBarEditControl progressBarEdit = null;
		public ASPxProgressBar()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarIndicatorStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarIndicatorStyle IndicatorStyle {
			get { return Properties.IndicatorStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarPaddings"),
#endif
		Category("Layout"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarDisplayFormatString"),
#endif
		Category("Settings"), DefaultValue(ProgressBarSettings.DefaultDisplayFormatString), Localizable(true), AutoFormatDisable]
		public virtual string DisplayFormatString {
			get { return Properties.DisplayFormatString; }
			set { Properties.DisplayFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarDisplayMode"),
#endif
		Category("Settings"), DefaultValue(ProgressBarDisplayMode.Percentage), NotifyParentProperty(true), AutoFormatEnable]
		public ProgressBarDisplayMode DisplayMode {
			get { return Properties.DisplayMode; }
			set {
				Properties.DisplayMode = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarShowPosition"),
#endif
		Category("Settings"), DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowPosition {
			get { return Properties.ShowPosition; }
			set {
				Properties.ShowPosition = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public EditClientSideEventsBase ClientSideEvents {
			get { return (EditClientSideEventsBase)ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarCaptionSettings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorCaptionSettingsBase CaptionSettings {
			get { return Properties.CaptionSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarWidth"),
#endif
		DefaultValue(typeof(Unit), "")]
		public override Unit Width {
			get { return Properties.Width; }
			set { Properties.Width = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarHeight"),
#endif
		DefaultValue(typeof(Unit), "")]
		public override Unit Height {
			get { return Properties.Height; }
			set { Properties.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarMinimum"),
#endif
		AutoFormatDisable, DefaultValue(typeof(Decimal), "0"),
		TypeConverter(typeof(DecimalNumberConverter))]
		public Decimal Minimum {
			get { return Properties.Minimum; }
			set {
				Properties.Minimum = value;
				if(DesignMode && !IsLoading()) {
					CheckRestrictions();
					CheckPositionRangeAndCorrect();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarMaximum"),
#endif
		AutoFormatDisable, DefaultValue(typeof(Decimal), "100"),
		TypeConverter(typeof(DecimalNumberConverter))]
		public Decimal Maximum {
			get { return Properties.Maximum; }
			set {
				Properties.Maximum = value;
				if(DesignMode && !IsLoading()) {
					CheckRestrictions();
					CheckPositionRangeAndCorrect();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarCustomDisplayFormat"),
#endif
		AutoFormatDisable, Bindable(true, BindingDirection.OneWay), Localizable(true), Category("Settings")]
		public string CustomDisplayFormat
		{
			get { return Properties.CustomDisplayFormat; }
			set { Properties.CustomDisplayFormat = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxProgressBarPosition"),
#endif
		DefaultValue(typeof(Decimal), "0"), Bindable(true, BindingDirection.OneWay), 
		TypeConverter(typeof(DecimalNumberConverter)), AutoFormatDisable]
		public Decimal Position {
			get { return (Value is Decimal) ? (Decimal)Value : Minimum; }
			set {
				if(DesignMode && !IsLoading()) 
					CommonUtils.CheckValueRange(value, Minimum, Maximum, "Position");
				if(value != 0)
					Value = value;
				else
					Value = null;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxProgressBarValue")]
#endif
		public override object Value {
			get { return base.Value; }
			set {
				Decimal position;
				if(!CommonUtils.IsNullValue(value) && TryParseToDecimal(value, out position))
					base.Value = position;
				else
					base.Value = value;
			}
		}
		protected internal new ProgressBarProperties Properties {
			get { return base.Properties as ProgressBarProperties; }
		}
		protected ProgressBarEditControl ProgressBarEdit {
			get { return progressBarEdit; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.progressBarEdit = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.progressBarEdit = new ProgressBarEditControl(this);
			Controls.Add(ProgressBarEdit);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ProgressBarEdit.ClientInstanceName = GetProgressBarEditClientInstanceName();
			ProgressBarEdit.ControlStyle.Reset();
			RenderUtils.AssignAttributes(this, ProgressBarEdit);
			ProgressBarEdit.ClientVisible = IsClientVisible();
			ProgressBarEdit.Position = Position;
			ProgressBarEdit.Minimum = Minimum;
			ProgressBarEdit.Maximum = Maximum;
			ProgressBarEdit.CustomDisplayFormat = CustomDisplayFormat;
			ProgressBarEdit.ControlStyle.CopyFrom(GetControlStyle());
			ProgressBarEdit.DisabledStyle.Assign(GetDisabledStyle());
			ProgressBarEdit.IndicatorStyle.Assign(GetIndicatorStyle());
			ProgressBarEdit.DisplayFormatString = DisplayFormatString;
			if(!Width.IsEmpty)
				ProgressBarEdit.Width = Width;
			if(!Height.IsEmpty)
				ProgressBarEdit.Height = Height;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new ProgressBarProperties(this);
		}
		protected override bool NeedVerifyRenderingInServerForm() {
			return true;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxProgressBar), ProgressBarScriptResourceName);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected virtual bool IsFormatterScriptRequired() {
			return IsDisplayFormatCapabilityEnabled();
		}
		protected virtual bool IsDisplayFormatCapabilityEnabled() {
			return !string.IsNullOrEmpty(DisplayFormatString);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientProgressBar";
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			return false;
		}
		protected override void PrepareControlStyle(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.GetDefaultProgressBarStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(RenderStyles.ProgressBar);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
		}
		protected ProgressBarIndicatorStyle GetIndicatorStyle() {
			ProgressBarIndicatorStyle style = new ProgressBarIndicatorStyle();
			style.CopyFrom(RenderStyles.GetDefaultProgressBarIndicatorStyle());
			style.CopyFrom(RenderStyles.ProgressBarIndicator);
			return style;
		}
		protected internal override void ValidateProperties() {
			base.ValidateProperties();
			CheckRestrictions();
			CommonUtils.CheckValueRange(Position, Minimum, Maximum, "Position");
		}
		protected internal void CheckPositionRangeAndCorrect() {
			if(Position < Minimum)
				Position = Minimum;
			else if(Position > Maximum)
				Position = Maximum;
			PropertyChanged("Position");
		}
		protected void CheckRestrictions() {
			if(!UseRestrictions()) return;
			if(Maximum < Minimum)
				throw new ArgumentException(string.Format(StringResources.ASPxEdit_InvalidRange, "MaxValue", "MinValue"));
		}
		protected internal bool UseRestrictions() {
			return Properties.Maximum != 0 || Properties.Minimum != 0;
		}
		protected internal Decimal GetMaxValue() {
			if(!UseRestrictions())
				return ASPxProgressBarBase.MaxPosition;
			return Maximum;
		}
		protected internal Decimal GetMinValue() {
			if(!UseRestrictions())
				return ASPxProgressBarBase.MinPosition;
			return Minimum;
		}
		protected string GetProgressBarEditClientInstanceName() {
			return ClientID + "_MC";
		}
		protected bool TryParseToDecimal(object value, out Decimal position) {
			bool ret = false;
			position = 0;
			try {
				position = Convert.ToDecimal(value);
				ret = true;
			}
			catch(Exception e) {
				if(!(e is FormatException)) throw;
			}
			return ret;
		}
	}
}
