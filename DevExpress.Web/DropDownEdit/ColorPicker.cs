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
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	[ToolboxItem(false)
	]
	public class ColorPicker : ASPxWebControl { 
		public enum ParameterName {
			Red = 0,
			Green = 1,
			Blue = 2
		}
		protected internal const string ColorPickerImagesResourcePath = "DevExpress.Web.Images.";
		public const string ColorAreaImageName = "cpColorArea";
		public const string ColorPointerImageName = "cpColorPointer";
		public const string HueAreaImageName = "cpHueArea";
		public const string HuePointerImageName = "cpHuePointer";
		public const string ClearCellImageName = "cpClearCell";
		protected internal const string ColorPickerScriptResourceName = ASPxColorEdit.EditScriptsResourcePath + "ColorPicker.js";
		const string ColorAreaID = "CA";
		const string HueAreaID = "HA";
		const string SavedColorDivID = "SC";
		const string CurrentColorID = "CC";
		const string WebColorInputId = "WC";
		readonly string[] InputsId = { "RI", "GI", "BI" };
		readonly string[] ColorParameterText = { "R", "G", "B" };
		protected Dictionary<ParameterName, ColorParameter> parameters;
		const int ParametersTableRowCount = 3;
		const int ParametersTableColumnCount = 1;
		protected Table MainTable { get; private set; }
		protected WebControl ColorAreaDiv { get; private set; }
		protected WebControl HueAreaDiv { get; private set; }
		protected WebControl HueAreaImageDiv { get; private set; }
		protected WebControl ColorPointerDiv { get; private set; }
		protected WebControl HuePointerDiv { get; private set; }
		protected Table ParametersTable { get; private set; }
		protected WebControl CurrentAndSavedDiv { get; private set; }
		protected WebControl CurrentColorDiv { get; private set; }
		protected WebControl SavedColorDiv { get; private set; }
		protected TableCell ParametersCell { get; private set; }
		protected WebControl WebColorInput { get; private set; }
		protected ColorPicker(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		public ColorPicker()
			: this(null) { }
		protected override void ClearControlFields() {
			this.parameters = null;
			MainTable = null;
			ColorAreaDiv = null;
			HueAreaDiv = null;
			HueAreaImageDiv = null;
			ColorPointerDiv = null;
			HuePointerDiv = null;
			ParametersTable = null;
			CurrentAndSavedDiv = null;
			CurrentColorDiv = null;
			SavedColorDiv = null;
			WebColorInput = null;
		}
		protected override void CreateControlHierarchy() {
			CreateCurrentAndSavedDiv();
			CreateSavedColorDiv();
			CreateCurrentColorDiv();
			CreateColorPointerDiv();
			CreateHuePointerDiv();
			CreateColorAreaDiv();
			CreateHueAreaImageDiv();
			CreateHueAreaDiv();
			CreateParametrs();
			CreateWebColorInput();
			CreateParametersTable();
			CreateMainTable();
		}
		private void CreateCurrentAndSavedDiv() {
			CurrentAndSavedDiv = RenderUtils.CreateDiv();
		}
		protected void PrepareCurrentAndSavedDiv() {
			GetCurrentAndSavedDivStyle().AssignToControl(CurrentAndSavedDiv);
		}
		protected void CreateSavedColorDiv() {
			SavedColorDiv = RenderUtils.CreateDiv();
			SavedColorDiv.ID = SavedColorDivID;
		}
		protected void PrepareSavedColorDiv() {
			GetSavedColorDivStyle().AssignToControl(SavedColorDiv);
		}
		protected void CreateCurrentColorDiv() {
			CurrentColorDiv = RenderUtils.CreateDiv();
			CurrentColorDiv.ID = CurrentColorID;
		}
		protected void PrepareCurrentColor() {
			GetCurrentColorDivStyle().AssignToControl(CurrentColorDiv);
		}
		protected void CreateColorPointerDiv() {
			ColorPointerDiv = RenderUtils.CreateDiv();
		}
		protected void PrepareColorPointerDiv() {
			GetColorPointerDivStyle().AssignToControl(ColorPointerDiv);
		}
		protected void CreateHuePointerDiv() {
			HuePointerDiv = RenderUtils.CreateDiv();
		}
		protected void PrepareHuePointerDiv() {
			GetHuePointerDivStyle().AssignToControl(HuePointerDiv);
		}
		protected override void PrepareControlHierarchy() {
			PrepareColorPointerDiv();
			PrepareHuePointerDiv();
			PrepareColorAreaDiv();
			PrepareHueAreaImageDiv();
			PrepareHueAreaDiv();
			PrepareMainTable();
			PrepareCurrentAndSavedDiv();
			PrepareSavedColorDiv();
			PrepareCurrentColor();
			PrepareWebColorInput();
		}
		protected void CreateColorAreaDiv() {
			ColorAreaDiv = RenderUtils.CreateDiv();
			ColorAreaDiv.ID = ColorAreaID;
			Controls.Add(ColorAreaDiv);
			ColorAreaDiv.Controls.Add(ColorPointerDiv);
		}
		protected void PrepareColorAreaDiv() {
			GetColorAreaDivStyle().AssignToControl(ColorAreaDiv);
		}
		protected void CreateHueAreaImageDiv() {
			HueAreaImageDiv = RenderUtils.CreateDiv();
		}
		protected void PrepareHueAreaImageDiv() {
			GetHueAreaImageDivStyle().AssignToControl(HueAreaImageDiv);
		}
		protected void CreateHueAreaDiv() {
			HueAreaDiv = RenderUtils.CreateDiv();
			Controls.Add(HueAreaDiv);
			HueAreaDiv.ID = HueAreaID;
			HueAreaDiv.Controls.Add(HuePointerDiv);
			HueAreaDiv.Controls.Add(HueAreaImageDiv);
		}
		protected void PrepareHueAreaDiv() {
			GetHueAreaDivStyle().AssignToControl(HueAreaDiv);
		}
		protected void CreateMainTable() {
			MainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow mainRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(mainRow);
			mainRow.Cells.Add(CreateMainTableCell(ColorAreaDiv));
			mainRow.Cells.Add(CreateMainTableCell(HueAreaDiv));
			ParametersCell = CreateMainTableCell(ParametersTable);
			CurrentAndSavedDiv.Controls.Add(CurrentColorDiv);
			CurrentAndSavedDiv.Controls.Add(SavedColorDiv);
			ParametersCell.Controls.Add(CurrentAndSavedDiv);
			ParametersCell.Controls.Add(WebColorInput);
			mainRow.Cells.Add(ParametersCell);
		}
		protected TableCell CreateMainTableCell(WebControl control) {
			TableCell cell = RenderUtils.CreateTableCell();
			cell.Controls.Add(control);
			return cell;
		}
		protected void PrepareMainTable() {
			GetMainTableStyle().AssignToControl(MainTable);
			RenderUtils.AssignAttributes(this, MainTable);
			GetParametersCellStyle().AssignToControl(ParametersCell);
		}
		protected void CreateParametrs() {
			parameters = new Dictionary<ParameterName, ColorParameter>();
			foreach(ParameterName parameter in Enum.GetValues(typeof(ParameterName))) {
				ColorParameter colorParameter = new ColorParameter(ColorParameterText[(int)parameter], InputsId[(int)parameter]);
				colorParameter.ID = ColorParameterText[(int)parameter];
				colorParameter.ParentSkinOwner = this;
				parameters.Add(parameter, colorParameter);
			}
		}
		protected void CreateWebColorInput() {
			WebColorInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			WebColorInput.ID = WebColorInputId;
		}
		protected void PrepareWebColorInput() {
			WebColorInput.Attributes.Add("maxlength", "7");
			GetWebColorInputStyle().AssignToControl(WebColorInput);
		}
		protected void CreateParametersTable() {
			ParametersTable = RenderUtils.CreateTable(true);
			Controls.Add(ParametersTable);
			for(int i = 0; i < ParametersTableRowCount; i++) {
				TableRow row = RenderUtils.CreateTableRow();
				ParametersTable.Rows.Add(row);
				for(int j = 0; j < ParametersTableColumnCount; j++) {
					TableCell cell = RenderUtils.CreateTableCell();
					cell.Controls.Add(parameters[(ParameterName)(i * ParametersTableColumnCount + j)]);
					row.Cells.Add(cell);
				}
			}
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.ColorPicker";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ColorTable), ColorPickerScriptResourceName);
		}
		protected internal ColorPickerStyles Styles {
			get { return (ColorPickerStyles)StylesInternal; }
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new ColorPickerStyles(this);
		}
		protected internal virtual AppearanceStyle GetMainTableStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(ControlStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetColorAreaDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultColorAreaDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetHueAreaDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultHueAreaDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetHueAreaImageDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultHueAreaImageDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetCurrentAndSavedDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultCurrentAndSavedDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetParametersCellStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultParametersCellStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetWebColorInputStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultWebColorInputStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetSavedColorDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultSavedColorDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetCurrentColorDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultCurrentColorDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetColorPointerDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultColorPointerDivStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetHuePointerDivStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultHuePointerDivStyle());
			return style;
		}
	}
	public class ColorPickerStyles : StylesBase {
		protected internal const string CssClassPrefix = "dxcp";
		protected internal const string 
			ColorAreaCssClass = CssClassPrefix + "ColorAreaSys",
			HueAreaCssClass = CssClassPrefix + "HueAreaSys",
			HueAreaImageCssClass = CssClassPrefix + "HueAreaImageSys",
			ColorPointerCssClass = CssClassPrefix + "ColorPointerSys",
			HuePointerCssClass = CssClassPrefix + "HuePointerSys",
			ParametersCellCssClass = CssClassPrefix + "ParametersCellSys";
		public ColorPickerStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected internal override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		protected internal AppearanceStyleBase GetDefaultColorAreaDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = ColorAreaCssClass;
			style.CopyFrom(CreateStyleByName("ColorArea"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultHueAreaDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = HueAreaCssClass;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultHueAreaImageDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = HueAreaImageCssClass;
			style.CopyFrom(CreateStyleByName("HueAreaImage"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultColorPointerDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = ColorPointerCssClass;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultHuePointerDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = HuePointerCssClass;
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCurrentAndSavedDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("CurrentAndSaved"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultCurrentColorDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("CurrentColor"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultSavedColorDivStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("SavedColor"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultParametersCellStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CssClass = ParametersCellCssClass;
			style.CopyFrom(CreateStyleByName("ParametersCell"));
			return style;
		}
		protected internal AppearanceStyleBase GetDefaultWebColorInputStyle() {
			AppearanceStyleBase style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("WebColorInput"));
			return style;
		}
	}
	[ToolboxItem(false)
	]
	public class ColorParameter : ASPxWebControl {
		public string Text { get; set; }
		public string InputId { get; set; }
		public string Value { get; set; }
		public ColorParameter(string text, string inputId) {
			Text = text;
			InputId = inputId;
		}
		protected override void CreateControlHierarchy() {
			Controls.Add(new ColorParameterControl(this));
		}
		public override bool EnableViewState {
			get { return false; }
			set { }
		}
		protected internal ColorParameterStyles Styles {
			get { return (ColorParameterStyles)StylesInternal; }
		}
		protected override StylesBase CreateStyles() {
			return new ColorParameterStyles(this);
		}
		protected internal AppearanceStyle GetMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultMainDivStyle());
			return style;
		}
	}
	public class ColorParameterControl : ASPxInternalWebControl {
		ColorParameter colorParameter;
		public WebControl MainDiv { get; set; }
		public WebControl Label { get; set; }
		public WebControl Input { get; set; }
		public ColorParameter ColorParameter {
			get { return colorParameter; }
		}
		public ColorParameterControl(ColorParameter colorParameter)
			: base() {
			this.colorParameter = colorParameter;
		}
		protected override void ClearControlFields() {
			MainDiv = null;
			Label = null;
			Input = null;
		}
		protected override void CreateControlHierarchy() {
			CreateLabel();
			CreateInput();
			CreateMainDiv();
		}
		protected override void PrepareControlHierarchy() {
			PrepareInput();
			PrepareLabel();
			colorParameter.GetMainDivStyle().AssignToControl(MainDiv);
		}
		protected void CreateMainDiv() {
			MainDiv = RenderUtils.CreateDiv();
			Controls.Add(MainDiv);
			MainDiv.Controls.Add(Label);
			MainDiv.Controls.Add(Input);
		}
		protected void CreateLabel() {
			Label = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
			LiteralControl literalControl = RenderUtils.CreateLiteralControl(ColorParameter.Text);
			Label.Controls.Add(literalControl);
		}
		protected void PrepareLabel() {
			RenderUtils.SetStringAttribute(Label, "for", Input.ClientID);
		}
		protected void CreateInput() {
			Input = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			Input.ID = ColorParameter.InputId;
		}
		protected void PrepareInput() {
			Input.Attributes.Add("maxlength", "3");
		}
	}
	public class ColorParameterStyles : StylesBase {
		public ColorParameterStyles(ColorParameter colorParameter)
			: base(colorParameter) {
		}
		protected internal const string CssClassPrefix = "dxcp";
		protected internal override string GetCssClassNamePrefix() {
			return CssClassPrefix;
		}
		protected internal AppearanceStyle GetDefaultMainDivStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName("ColorParameterMainDiv"));
			return style;
		}
	}
}
