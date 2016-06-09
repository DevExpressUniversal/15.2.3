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

using System.ComponentModel;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public static class DialogUtils {
		public const string DirectorySeparator = "\\";
		public const string DefaultSpreadsheetFileType = ".xlsx";
		public const string DefaultSpreadsheetFileName = "spreadsheet1";
		public const string TemporaryFileNamePrefix = "dxupload_";
		public const string LabelEndMark = ":";
		public static string GetRootFolder(string folderPath) {
			string phPathWorkingDirectory = UrlUtils.ResolvePhysicalPath(folderPath);
			DirectoryInfo dirInfo = new DirectoryInfo(phPathWorkingDirectory);
			if(dirInfo != null)
				return dirInfo.Name + DirectorySeparator;
			return DirectorySeparator;
		}
		public static string GetControlClientInstanceName(SpreadsheetDialogBase dialog, string controlName) {
			return dialog.ClientID + controlName;
		}
		public static ItemImagePropertiesBase GetChartImage(ASPxSpreadsheet owner, SpreadsheetCommandId commandID, bool isSmallImage) {
			if(isSmallImage)
				return SpreadsheetRibbonHelper.GetSpreadsheetSmallImageProperty(owner, commandID);
			return SpreadsheetRibbonHelper.GetSpreadsheetLargeImageProperty(owner, commandID);
		}
		public static string GetChartText(SpreadsheetCommandId commandID) {
			return SpreadsheetRibbonHelper.GetCommandText(commandID);
		}
		public static HtmlTable CreateTable() {
			return CreateTable("");
		}
		public static HtmlTable CreateTable(string cssClass) {
			HtmlTable table = new HtmlTable();
			table.Attributes.Add("class", cssClass);
			table.Attributes.Add("cellpadding", "0");
			table.Attributes.Add("cellspacing", "0");
			return table;
		}
		public static HtmlTableCell CreateCaptionCell() {
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogCaptionCellCssClass);
			return tableCell;
		}
		public static HtmlTableCell CreateInputCell() {
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogInputCellCssClass);
			return tableCell;
		}
		public static HtmlTableCell CreateSeparatorCell() {
			HtmlTableCell tableCell = new HtmlTableCell();
			tableCell.Attributes.Add("class", SpreadsheetStyles.DialogSeparatorCellCssClass);
			return tableCell;
		}
		public static ASPxTextBox CreateTextBoxWithErrorFrame(string validationGroup) {
			ASPxTextBox txb = new ASPxTextBox();
			txb.Width = Unit.Percentage(100);
			txb.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			txb.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			txb.ValidationSettings.SetFocusOnError = true;
			txb.ValidationSettings.ValidateOnLeave = false;
			txb.ValidationSettings.ValidationGroup = validationGroup;
			txb.ValidationSettings.RequiredField.IsRequired = true;
			txb.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			return txb;
		}
		public static ASPxTextBox CreateTextBoxWithErrorFrame(string controlId, string clientInstanceName, string validationGroup) {
			ASPxTextBox txb = CreateTextBoxWithErrorFrame(validationGroup);
			txb.ID = controlId;
			txb.ClientInstanceName = clientInstanceName;
			return txb;
		}
		public static ASPxSpinEdit CreateSpinEditWithErrorFrame(string validationGroup) {
			ASPxSpinEdit se = new ASPxSpinEdit();
			se.Width = Unit.Percentage(100);
			se.NumberType = SpinEditNumberType.Integer;
			se.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.Text;
			se.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
			se.ValidationSettings.SetFocusOnError = true;
			se.ValidationSettings.ValidateOnLeave = false;
			se.ValidationSettings.ValidationGroup = validationGroup;
			se.ValidationSettings.RequiredField.IsRequired = true;
			se.ValidationSettings.ErrorFrameStyle.Font.Size = FontUnit.Parse("10px");
			return se;
		}
		public static ASPxSpinEdit CreateSpinEditWithErrorFrame(string controlId, string validationGroup) {
			ASPxSpinEdit se = CreateSpinEditWithErrorFrame(validationGroup);
			se.ID = controlId;
			return se;
		}
		public static ASPxSpinEdit CreateSpinEditWithErrorFrame(string controlId, string validationGroup, int minValue, int maxValue) {
			ASPxSpinEdit se = CreateSpinEditWithErrorFrame(controlId, validationGroup);
			se.MinValue = minValue;
			se.MaxValue = maxValue;
			return se;
		}
		public static ASPxHiddenField CreateHiddenField(string controlId) {
			ASPxHiddenField hf = new ASPxHiddenField();
			hf.ID = controlId;
			hf.SyncWithServer = false;
			return hf;
		}
	}
	#region ChangeChartType TabControl
	[ToolboxItem(false)]
	public class STTabPageBase : TabPage {
		private const string DefaultButtonGroup = "dxChartsBtn";
		private const string ButtonClickFunctionTemplate = "function(s, e) {{ ASPx.SpreadsheetDialog.SetChartType('{0}'); }}";
		protected ASPxSpreadsheet OwnerControl { get; private set; }
		protected virtual SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.None; }
		}
		protected virtual SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] { };
		}
		public STTabPageBase(ASPxSpreadsheet spreadsheet) {
			OwnerControl = spreadsheet;
			Text = DialogUtils.GetChartText(CommandId);
			this.TabImage.CopyFrom(DialogUtils.GetChartImage(OwnerControl, CommandId, true));
		}
		public override ContentControlCollection ContentCollection {
			get {
				ContentControlCollection contentCollection = new ContentControlCollection(this.OwnerControl);
				ContentControl content = new ContentControl();
				content.Controls.Add(GetTabPageContent());
				contentCollection.Add(content);
				return contentCollection;
			}
		}
		protected virtual Control GetTabPageContent() {
			Control innerDiv = RenderUtils.CreateDiv();
			SpreadsheetCommandId[] commands = GetChildChartCommands();
			foreach(SpreadsheetCommandId command in commands)
				innerDiv.Controls.Add(CreatedChartButton(command));
			return innerDiv;
		}
		protected virtual ASPxButton CreatedChartButton(SpreadsheetCommandId command) {
			ASPxButton chartButton = new ASPxButton();
			chartButton.GroupName = DefaultButtonGroup;
			chartButton.AutoPostBack = false;
			chartButton.Image.CopyFrom(DialogUtils.GetChartImage(OwnerControl, command, false));
			chartButton.CssClass = SpreadsheetStyles.DialogChartButton;
			chartButton.ClientSideEvents.Click = string.Format(ButtonClickFunctionTemplate, command.ToString());
			chartButton.ToolTip = DialogUtils.GetChartText(command);
			return chartButton;
		}
	}
	[ToolboxItem(false)]
	public class STColumnTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartColumnCommandGroup; }
		}
		public STColumnTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] { SpreadsheetCommandId.InsertChartColumnClustered2D, SpreadsheetCommandId.InsertChartColumnStacked2D,
		SpreadsheetCommandId.InsertChartColumnPercentStacked2D, SpreadsheetCommandId.InsertChartColumnClustered3D, SpreadsheetCommandId.InsertChartColumnStacked3D,
		SpreadsheetCommandId.InsertChartColumnPercentStacked3D, SpreadsheetCommandId.InsertChartColumn3D,
		SpreadsheetCommandId.InsertChartCylinderClustered, SpreadsheetCommandId.InsertChartCylinderStacked,
		SpreadsheetCommandId.InsertChartCylinderPercentStacked, SpreadsheetCommandId.InsertChartConeClustered,
		SpreadsheetCommandId.InsertChartConeStacked, SpreadsheetCommandId.InsertChartConePercentStacked,
		SpreadsheetCommandId.InsertChartCone,SpreadsheetCommandId.InsertChartPyramidClustered,SpreadsheetCommandId.InsertChartPyramidStacked,
		SpreadsheetCommandId.InsertChartPyramidPercentStacked, SpreadsheetCommandId.InsertChartPyramid};
		}
	}
	[ToolboxItem(false)]
	public class STLineTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartLineCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartLine, SpreadsheetCommandId.InsertChartStackedLine,
		SpreadsheetCommandId.InsertChartPercentStackedLine, SpreadsheetCommandId.InsertChartLineWithMarkers,
		SpreadsheetCommandId.InsertChartStackedLineWithMarkers, SpreadsheetCommandId.InsertChartPercentStackedLineWithMarkers,
		SpreadsheetCommandId.InsertChartLine3D};
		}
		public STLineTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	[ToolboxItem(false)]
	public class STPieTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartPieCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartPie2D, SpreadsheetCommandId.InsertChartPieExploded2D,
		SpreadsheetCommandId.InsertChartPie3D, SpreadsheetCommandId.InsertChartPieExploded3D,
		SpreadsheetCommandId.InsertChartDoughnut2D, SpreadsheetCommandId.InsertChartDoughnutExploded2D};
		}
		public STPieTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	[ToolboxItem(false)]
	public class STBarTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartBarCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartBarClustered2D, SpreadsheetCommandId.InsertChartBarStacked2D,
		SpreadsheetCommandId.InsertChartBarPercentStacked2D, SpreadsheetCommandId.InsertChartBarClustered3D,
		SpreadsheetCommandId.InsertChartBarStacked3D, SpreadsheetCommandId.InsertChartBarPercentStacked3D,
		SpreadsheetCommandId.InsertChartHorizontalCylinderClustered, SpreadsheetCommandId.InsertChartHorizontalCylinderStacked,
		SpreadsheetCommandId.InsertChartHorizontalCylinderPercentStacked, SpreadsheetCommandId.InsertChartHorizontalConeClustered,
		SpreadsheetCommandId.InsertChartHorizontalConeStacked, SpreadsheetCommandId.InsertChartHorizontalConePercentStacked,
		SpreadsheetCommandId.InsertChartHorizontalPyramidClustered, SpreadsheetCommandId.InsertChartHorizontalPyramidStacked,
		SpreadsheetCommandId.InsertChartHorizontalPyramidPercentStacked};
		}
		public STBarTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	[ToolboxItem(false)]
	public class STAreaTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartAreaCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartArea, SpreadsheetCommandId.InsertChartStackedArea,
		SpreadsheetCommandId.InsertChartPercentStackedArea, SpreadsheetCommandId.InsertChartArea3D, 
		SpreadsheetCommandId.InsertChartStackedArea3D, SpreadsheetCommandId.InsertChartPercentStackedArea3D};
		}
		public STAreaTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	[ToolboxItem(false)]
	public class STScatterTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartScatterCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartScatterMarkers, SpreadsheetCommandId.InsertChartScatterSmoothLinesAndMarkers,
		SpreadsheetCommandId.InsertChartScatterSmoothLines, SpreadsheetCommandId.InsertChartScatterLinesAndMarkers,
		SpreadsheetCommandId.InsertChartScatterLines, SpreadsheetCommandId.InsertChartBubble,
		SpreadsheetCommandId.InsertChartBubble3D};
		}
		public STScatterTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	[ToolboxItem(false)]
	public class STOtherTabPage : STTabPageBase {
		protected override SpreadsheetCommandId CommandId {
			get { return SpreadsheetCommandId.InsertChartOtherCommandGroup; }
		}
		protected override SpreadsheetCommandId[] GetChildChartCommands() {
			return new SpreadsheetCommandId[] {SpreadsheetCommandId.InsertChartStockHighLowClose, SpreadsheetCommandId.InsertChartStockOpenHighLowClose,
		SpreadsheetCommandId.InsertChartRadar, SpreadsheetCommandId.InsertChartRadarWithMarkers, SpreadsheetCommandId.InsertChartRadarFilled};
		}
		public STOtherTabPage(ASPxSpreadsheet spreadsheet) : base(spreadsheet) { }
	}
	#endregion
	#region ModifyTableStyle TabControl
	[ToolboxItem(false)]
	public abstract class TableStyleTabPageBase : TabPage {
		private const string DefaultButtonGroup = "dxTablesBtn";
		private const string ButtonClickFunctionTemplate = "function(s, e) {{ ASPx.SpreadsheetDialog.SetTableStyle('{0}'); }}";
		private const string NamePrefix = "TableStyle";
		protected ASPxSpreadsheet OwnerControl { get; private set; }
		protected SpreadsheetTableStylesHelper TableStylesHelper { get; private set; }
		protected abstract int PresetsCount { get; }
		protected abstract string GroupPrefix { get; }
		protected abstract XtraSpreadsheetStringId CategoryNameId { get; }
		protected abstract XtraSpreadsheetStringId NamePartId { get; }
		public TableStyleTabPageBase(ASPxSpreadsheet spreadsheet) {
			OwnerControl = spreadsheet;
			TableStylesHelper = new SpreadsheetTableStylesHelper(OwnerControl);
			Text = XtraSpreadsheetLocalizer.GetString(CategoryNameId);
		}
		public override ContentControlCollection ContentCollection {
			get {
				ContentControlCollection contentCollection = new ContentControlCollection(this.OwnerControl);
				ContentControl content = new ContentControl();
				content.Controls.Add(GetTabPageContent());
				contentCollection.Add(content);
				return contentCollection;
			}
		}
		protected virtual Control GetTabPageContent() {
			Control innerDiv = RenderUtils.CreateDiv("dxssModifyChartStyleContenPanel");
			PopulatePresets(innerDiv);
			return innerDiv;
		}
		protected virtual void PopulatePresets(Control container) {
			for(int i = 0; i < PresetsCount; i++) {
				container.Controls.Add(CreatePresetButton(i));
			}
		}
		protected virtual ASPxButton CreatePresetButton(int index) {
			string styleName = NamePrefix + GroupPrefix + (index + 1).ToString();
			string tooltip = GetPresetTooltipText(index);			
			return CreatePresetButtonCore(styleName, tooltip);
		}
		protected virtual ASPxButton CreatePresetButtonCore(string styleName, string tooltip) {
			ASPxButton presetButton = new ASPxButton();
			presetButton.GroupName = DefaultButtonGroup;
			presetButton.AutoPostBack = false;
			presetButton.Image.Url = TableStylesHelper.GetPresetImageURL(styleName);
			presetButton.CssClass = SpreadsheetStyles.TableStyleButton;
			presetButton.ClientSideEvents.Click = string.Format(ButtonClickFunctionTemplate, styleName);
			presetButton.ToolTip = tooltip;
			return presetButton;
		}
		protected string GetPresetTooltipText(int index) {
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_PrefixTableStyleNamePart) + " " +
				XtraSpreadsheetLocalizer.GetString(NamePartId) + " " + (index + 1).ToString();
		}
	}
	[ToolboxItem(false)]
	public class LightTableStyleTabPage : TableStyleTabPageBase {
		protected override int PresetsCount {
			get {
				return 21;
			}
		}
		protected override string GroupPrefix {
			get {
				return "Light";
			}
		}
		protected override XtraSpreadsheetStringId CategoryNameId {
			get {
				return XtraSpreadsheetStringId.Caption_LightTableStyleCategory;
			}
		}
		protected override XtraSpreadsheetStringId NamePartId {
			get {
				return XtraSpreadsheetStringId.Caption_LightTableStyleNamePart;
			}
		}
		public LightTableStyleTabPage(ASPxSpreadsheet spreadsheet) 
			: base(spreadsheet) { 
		}
		protected override void PopulatePresets(Control container) {
			string styleNameNone = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_TableStyleNameIsNone);
			container.Controls.Add(CreatePresetButtonCore(styleNameNone, "None"));
			base.PopulatePresets(container);
		}
	}
	[ToolboxItem(false)]
	public class MediumTableStyleTabPage : TableStyleTabPageBase {
		protected override int PresetsCount {
			get {
				return 28;
			}
		}
		protected override string GroupPrefix {
			get {
				return "Medium";
			}
		}
		protected override XtraSpreadsheetStringId CategoryNameId {
			get {
				return XtraSpreadsheetStringId.Caption_MediumTableStyleCategory;
			}
		}
		protected override XtraSpreadsheetStringId NamePartId {
			get {
				return XtraSpreadsheetStringId.Caption_MediumTableStyleNamePart;
			}
		}
		public MediumTableStyleTabPage(ASPxSpreadsheet spreadsheet) 
			: base(spreadsheet) { 
		}
	}
	[ToolboxItem(false)]
	public class DarkTableStyleTabPage : TableStyleTabPageBase {
		protected override int PresetsCount {
			get {
				return 11;
			}
		}
		protected override string GroupPrefix {
			get {
				return "Dark";
			}
		}
		protected override XtraSpreadsheetStringId CategoryNameId {
			get {
				return XtraSpreadsheetStringId.Caption_DarkTableStyleCategory;
			}
		}
		protected override XtraSpreadsheetStringId NamePartId {
			get {
				return XtraSpreadsheetStringId.Caption_DarkTableStyleNamePart;
			}
		}
		public DarkTableStyleTabPage(ASPxSpreadsheet spreadsheet) 
			: base(spreadsheet) { 
		}
	}
	#endregion
}
