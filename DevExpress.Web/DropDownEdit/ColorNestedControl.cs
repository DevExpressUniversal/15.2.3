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

using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Localization;
using DevExpress.Web.DropDownEdit;
using System.Drawing;
using System.Text;
namespace DevExpress.Web.Internal {
	public class ColorNestedControlProperties : PropertiesBase {
		ColorEditItemCollection colorTableItems;
		ColorEditItemCollection customColorTableItems;
		public ColorNestedControlProperties()
			: this(null) {
		}
		public ColorNestedControlProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		public ColorEditItemCollection Items {
			get {
				if(colorTableItems == null)
					colorTableItems = new ColorEditItemCollection();
				return colorTableItems;
			}
		}
		public bool EnableCustomColors {
			get { return GetBoolProperty("EnableCustomColors", false); }
			set { SetBoolProperty("EnableCustomColors", false, value); }
		}
		public bool EnableAutomaticColorItem {
			get { return GetBoolProperty("EnableAutomaticColorItem", false); }
			set { SetBoolProperty("EnableAutomaticColorItem", false, value); }
		}
		public int ColumnCount {
			get { return GetIntProperty("ColumnCount", ColorTable.DefaultColumnCount); }
			set { SetIntProperty("ColumnCount", ColorTable.DefaultColumnCount, value); }
		}
		public string CustomColorButtonText {
			get { return GetStringProperty("CustomColorButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_CustomColor)); }
			set { SetStringProperty("CustomColorButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_CustomColor), value); }
		}
		public string AutomaticColorItemCaption {
			get { return GetStringProperty("AutomaticColorItemCaption", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_AutomaticColor)); }
			set { SetStringProperty("AutomaticColorItemCaption", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_AutomaticColor), value); }
		}
		public string AutomaticColorItemValue {
			get { return GetStringProperty("AutomaticColorItemValue", ASPxColorEdit.DefaultAutomaticColorItemValue); }
			set { SetStringProperty("AutomaticColorItemValue", ASPxColorEdit.DefaultAutomaticColorItemValue, value); }
		}
		public string OkButtonText {
			get { return GetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_OK)); }
			set { SetStringProperty("OkButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_OK), value); }
		}
		public string CancelButtonText {
			get { return GetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_Cancel)); }
			set { SetStringProperty("CancelButtonText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.ColorEdit_Cancel), value); }
		}
		public Color AutomaticColor {
			get { return GetColorProperty("AutomaticColor", Color.Black); }
			set { SetColorProperty("AutomaticColor", Color.Black, value); }
		}
		public ColorEditItemCollection GetColorTableItems() {
			return Items.IsEmpty ? new ColorEditItemCollection(ColorTable.DefaultColorTableItems)
				: this.colorTableItems;
		}
		public ColorEditItemCollection CustomColorTableItems {
			get {
				if(customColorTableItems == null)
					customColorTableItems = new ColorEditItemCollection();
				return customColorTableItems;
			}
		}
		public void DeserializeColorsToCustomColorTableItems(string state) {
			if(string.IsNullOrEmpty(state)) return;
			string []colors = ColorTable.DeserializeColors(state);
			for (int i = colors.Length - 1 ; i >= 0 ; i--)
				CustomColorTableItems.SetColor(i, colors[i]);
		}
		public string GetSerializedCustomColorTableItems() {
			return CustomColorTable.SerializeColors(CustomColorTableItems);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Items });
		}
		public override void Assign(PropertiesBase source) {
			ColorNestedControlProperties src = source as ColorNestedControlProperties;
			if(src != null) {
				ColumnCount = src.ColumnCount;
				EnableCustomColors = src.EnableCustomColors;
				CustomColorButtonText = src.CustomColorButtonText;
				CancelButtonText = src.CancelButtonText;
				OkButtonText = src.OkButtonText;
				EnableAutomaticColorItem = src.EnableAutomaticColorItem;
				AutomaticColorItemCaption = src.AutomaticColorItemCaption;
				AutomaticColor = src.AutomaticColor;
				AutomaticColorItemValue = src.AutomaticColorItemValue;
				Items.Assign(src.Items);
				CustomColorTableItems.Assign(src.CustomColorTableItems);
			}
			base.Assign(source);
		}
	}
	[ToolboxItem(false)]
	public class ColorNestedControl : ASPxWebControl {
		protected internal const string ColorNestedControlScriptResourceName = WebScriptsResourcePath + "Editors.ColorNestedControl.js",
			ColorNestedControlPostfix = "CNC",
			ColorTablesIdPostfix = "CTS",
			ColorSelectorMainDivIdPostfix = "CS",
			AutomaticColorItemIdPostfix = "ACI",
			AutomaticColorItemSelectionFrameIdPostfix = "ACISF";
		public ColorNestedControl(ColorNestedControlProperties properties)
			: base() {
			Properties = properties;
			ID = ColorNestedControlPostfix;
		}
		protected ColorTablesControl ColorTablesControl { get; private set; }
		protected ColorSelectorControl ColorSelectorControl { get; private set; }
		protected internal ColorNestedControlStyles RenderStyles {
			get { return (ColorNestedControlStyles)RenderStylesInternal; }
		}
		protected internal ColorNestedControlProperties Properties { get; private set; }
		protected internal ColorEditItemCollection CustomColorTableItems {
			get { return Properties.CustomColorTableItems; }
		}
		public bool EnableCustomColors {
			get { return Properties.EnableCustomColors; }
			set {
				Properties.EnableCustomColors = value;
				LayoutChanged();
			}
		}
		public string AutomaticColorItemCaption {
			get { return Properties.AutomaticColorItemCaption; }
			set { Properties.AutomaticColorItemCaption = value; }
		}
		public string AutomaticColorItemValue {
			get { return Properties.AutomaticColorItemValue; }
			set { Properties.AutomaticColorItemValue = value; }
		}
		public Color AutomaticColor {
			get { return Properties.AutomaticColor; }
			set { Properties.AutomaticColor = value; }
		}
		public int ColumnCount {
			get { return Properties.ColumnCount; }
			set { Properties.ColumnCount = value; }
		}
		public string CustomColorButtonText {
			get { return Properties.CustomColorButtonText; }
			set { Properties.CustomColorButtonText = value; }
		}
		public bool EnableAutomaticColorItem {
			get { return Properties.EnableAutomaticColorItem; }
			set {
				Properties.EnableAutomaticColorItem = value;
				LayoutChanged();
			}
		}
		public ColorNestedControlClientSideEvents ClientSideEvents {
			get { return (ColorNestedControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ColorTablesControl = null;
			ColorSelectorControl = null;
		}
		protected override void CreateControlHierarchy() {
			ColorTablesControl = new ColorTablesControl(this);
			Controls.Add(ColorTablesControl);
			if(Properties.EnableCustomColors) {
				ColorSelectorControl = new ColorSelectorControl(this);
				Controls.Add(ColorSelectorControl);
			}
		}
		protected internal int GetColumnCountForRender() {
			if(Properties.Items.IsEmpty)
				return Properties.ColumnCount <= ColorTable.DefaultColorTableItems.Length ? Properties.ColumnCount : ColorTable.DefaultColorTableItems.Length;
			return Properties.ColumnCount <= Properties.Items.Count ? Properties.ColumnCount : Properties.Items.Count;
		}
		protected internal string GetColorTablesID() {
			return ColorTablesIdPostfix;
		}
		protected internal string GetColorSelectorMainDivID() {
			return ColorSelectorMainDivIdPostfix;
		}
		protected internal string GetAutomaticColorItemId() {
			return AutomaticColorItemIdPostfix;
		}
		protected internal string GetAutomaticColorItemSelectionFrameId() {
			return AutomaticColorItemSelectionFrameIdPostfix;
		}
		protected internal ColorTableStyle ColorTableStyle {
			get { return ((ColorNestedControlStyles)StylesInternal).ColorTable; }
		}
		protected internal ColorPickerStyle ColorPickerStyle {
			get { return ((ColorNestedControlStyles)StylesInternal).ColorPicker; }
		}
		protected internal ColorTableCellStyle ColorTableCellStyle {
			get { return ((ColorNestedControlStyles)StylesInternal).ColorTableCell; }
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ColorNestedControlStyles(this);
		}
		protected internal ColorTableCellStyle GetColorTableCellStyle() {
			return RenderStyles.ColorTableCell;
		}
		protected internal AppearanceStyleBase GetColorTableStyle() {
			return RenderStyles.ColorTable;
		}
		protected internal AppearanceStyleBase GetColorPickerStyle() {
			return RenderStyles.ColorPicker;
		}
		protected internal AppearanceStyleBase GetCustomColorButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultCustomColorButtonStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetAutomaticColorItemStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultAutomaticColorItemStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetAutomaticColorItemCellSelectedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultAutomaticColorItemCellSelectedStyle());
			style.CopyFrom(ColorTableCellStyle.SelectedStyle);
			return style;
		}
		protected internal AppearanceStyle GetAutomaticColorItemCellHoverStyle() {
			AppearanceStyle ret = new AppearanceStyle();
			ret.CopyFrom(RenderStyles.GetDefaultAutomaticColorItemCellHoverStyle());
			ret.CopyFrom(ColorTableCellStyle.HoverStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetAutomaticColorItemCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultAutomaticColorItemCellStyle());
			style.CopyFrom(ColorTableCellStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetAutomaticColorItemCellDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultAutomaticColorItemCellDivStyle());
			return style;
		}
		protected internal virtual AppearanceStyleBase GetColorTablesMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultColorTablesMainDivStyle());
			style.CopyFrom(GetColorTableStyle());
			return style;
		}
		protected internal virtual AppearanceStyleBase GetColorSelectorMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultColorSelectorMainDivStyle());
			style.CopyFrom(GetColorPickerStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetButtonsPanelDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultButtonsPanelDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetOkButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultOkButtonStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetCancelButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultCancelButtonStyle());
			return style;
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxEditBase), ASPxEditBase.EditSystemCssResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.ColorNestedControl";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!EnableCustomColors)
				stb.Append(localVarName + ".enableCustomColors = false;\n");
			if(EnableAutomaticColorItem) {
				stb.Append(localVarName + ".enableAutomaticColorItem = true;\n");
				stb.AppendFormat("{0}.automaticColorItemValue = '{1}';\n", localVarName, Properties.AutomaticColorItemValue);
				if(!Properties.AutomaticColor.IsEmpty)
					stb.AppendFormat("{0}.automaticColor = '{1}';\n", localVarName, ColorUtils.ToHexColor(Properties.AutomaticColor));
				AddAutomaticColorSelectedItem(stb);
				AddAutomaticColorHoverItem(stb);
			}
		}
		protected void AddAutomaticColorSelectedItem(StringBuilder stb) {
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			helper.AddStyle(GetAutomaticColorItemCellSelectedStyle(), AutomaticColorItemSelectionFrameIdPostfix, IsEnabled());
			helper.GetCreateSelectedScript(stb);
		}
		protected void AddAutomaticColorHoverItem(StringBuilder stb) {
			StateScriptRenderHelper helper = new StateScriptRenderHelper(Page, ClientID);
			helper.AddStyle(GetAutomaticColorItemCellHoverStyle(), AutomaticColorItemSelectionFrameIdPostfix, IsEnabled());
			helper.GetCreateHoverScript(stb);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected sealed override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ColorNestedControl), ColorNestedControlScriptResourceName);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ColorNestedControlClientSideEvents();
		}
	}
	public class ColorNestedControlClientSideEvents : ClientSideEvents {
		public ColorNestedControlClientSideEvents()
			: base() {
		}
		public string ColorChanged {
			get { return GetEventHandler("ColorChanged"); }
			set { SetEventHandler("ColorChanged", value); }
		}
		public string CustomColorTableUpdated {
			get { return GetEventHandler("CustomColorTableUpdated"); }
			set { SetEventHandler("CustomColorTableUpdated", value); }
		}
		public string ShouldBeClosed {
			get { return GetEventHandler("ShouldBeClosed"); }
			set { SetEventHandler("ShouldBeClosed", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ColorChanged");
			names.Add("CustomColorTableUpdated");
			names.Add("ShouldBeClosed");
		}
	}
	public class ColorNestedControlStyles : StylesBase {
		public const string ColorTableStyleName = "ColorTable";
		public const string ColorPickerStyleName = "ColorPicker";
		public const string ColorTableCellStyleName = "ColorTableCell";
		public const string ColorIndicatorStyleName = "ColorIndicator";
		public const string DisplayColorIndicatorStyleName = "DisplayColorIndicator";
		internal const string ButtonPanelDivCssClass = "dxeButtonsPanelDivSys";
		public ColorNestedControlStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return "dxe";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(ColorTableStyleName, delegate() { return new ColorTableStyle(); }));
			list.Add(new StyleInfo(ColorPickerStyleName, delegate() { return new ColorPickerStyle(); }));
			list.Add(new StyleInfo(ColorTableCellStyleName, delegate() { return new ColorTableCellStyle(); }));
		}
		public virtual ColorTableStyle ColorTable {
			get { return (ColorTableStyle)GetStyle(ColorTableStyleName); }
		}
		public virtual ColorPickerStyle ColorPicker {
			get { return (ColorPickerStyle)GetStyle(ColorPickerStyleName); }
		}
		public virtual ColorTableCellStyle ColorTableCell {
			get { return (ColorTableCellStyle)GetStyle(ColorTableCellStyleName); }
		}
		public virtual ColorIndicatorStyle ColorIndicator {
			get { return (ColorIndicatorStyle)GetStyle(ColorIndicatorStyleName); }
		}
		public virtual ColorIndicatorStyle DisplayColorIndicator {
			get { return (ColorIndicatorStyle)GetStyle(DisplayColorIndicatorStyleName); }
		}
		protected internal AppearanceStyleBase GetDefaultColorIndicatorStyle() {
			ColorIndicatorStyle style = new ColorIndicatorStyle();
			style.CopyFrom(CreateStyleByName("ColorIndicator"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultColorTablesMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorTablesMainDiv"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultColorSelectorMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorSelectorMainDiv"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultButtonsPanelDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = ButtonPanelDivCssClass;
			style.CopyFrom(CreateStyleByName("ButtonsPanelDiv"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCustomColorButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("CustomColorButton"));
			style.CopyFrom(CreateStyleByName("ColorEditButton"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultAutomaticColorItemStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("AutomaticColorItem"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultAutomaticColorItemCellSelectedStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ACICellSelected"));
			return style;
		}
		protected internal AppearanceStyle GetDefaultAutomaticColorItemCellHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(string.Format("{0}CellHover", "AutomaticColorItem")));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultAutomaticColorItemCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ACICell"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultAutomaticColorItemCellDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ACICellDiv"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultOkButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorEditButton"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCancelButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorEditButton"));
			return style;
		}
	}
}
