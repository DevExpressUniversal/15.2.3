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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Spreadsheet;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.JSONTypes;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Web.Office.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Web.Design;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using System.Collections;
using DevExpress.Web.Office;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.Web.ASPxSpreadsheet {
	public enum SpreadsheetRibbonMode {
		Ribbon,
		ExternalRibbon,
		None,
		Auto,
		OneLineRibbon
	}
	[DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxSpreadsheet"),
	Designer("DevExpress.Web.ASPxSpreadsheet.Design.ASPxSpreadsheetDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), "ASPxSpreadsheet.bmp"),
	ToolboxData(@"<{0}:ASPxSpreadsheet runat=""server"" WorkDirectory = """ + ASPxSpreadsheet.DefaultWorkDirectory + @"""></{0}:ASPxSpreadsheet>"),
	DXWebToolboxItem(true), 
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabData)
]
	public class ASPxSpreadsheet : ASPxWebControl, OfficeWorkSessionControl, IParentSkinOwner, IControlDesigner, IAutoSaveControl {
		protected internal const string
			SpreadsheetScriptsResourcePath = "DevExpress.Web.ASPxSpreadsheet.Scripts.",
			SpreadsheetStyleResourcePath = "DevExpress.Web.ASPxSpreadsheet.Css.",
			SpreadsheetImageResourcePath = "DevExpress.Web.ASPxSpreadsheet.Images.",
			SpreadsheetScriptResourceName = SpreadsheetScriptsResourcePath + "Spreadsheet.js",
			SpreadsheetTileHelperScriptResourceName = SpreadsheetScriptsResourcePath + "TileHelper.js",
			SpreadsheetScrollHelperScriptResourceName = SpreadsheetScriptsResourcePath + "ScrollHelper.js",
			SpreadsheetGridResizingHelperScriptResourceName = SpreadsheetScriptsResourcePath + "GridResizingHelper.js",
			SpreadsheetTileMatrixScriptResourceName = SpreadsheetScriptsResourcePath + "TileMatrix.js",
			SpreadsheetCommandsScriptResourceName = SpreadsheetScriptsResourcePath + "Commands.js",
			SpreadsheetDynamicSelectionScriptResourceName = SpreadsheetScriptsResourcePath + "DynamicSelection.js",
			SpreadsheetSelectionScriptResourceName = SpreadsheetScriptsResourcePath + "Selection.js",
			SpreadsheetEditingScriptResourceName = SpreadsheetScriptsResourcePath + "Editing.js",
			SpreadsheetKeyboardManagerScriptResourceName = SpreadsheetScriptsResourcePath + "KeyboardManager.js",
			SpreadsheetRibbonManagerScriptResourceName = SpreadsheetScriptsResourcePath + "RibbonManager.js",
			SpreadsheetDialogsScriptResourceName = SpreadsheetScriptsResourcePath + "Dialogs.js",
			SpreadsheetSystemCssResourceName = SpreadsheetStyleResourcePath + "System.css",
			SpreadsheetSpriteCssResourceName = SpreadsheetStyleResourcePath + "sprite.css",
			SpreadsheetIconSpriteCssResourceName = SpreadsheetStyleResourcePath + "ISprite.css",
			SpreadsheetGrayScaleIconSpriteCssResourceName = SpreadsheetStyleResourcePath + "GISprite.css",
			SpreadsheetGrayScaleWithWhiteHottrackIconSpriteCssResourceName = SpreadsheetStyleResourcePath + "GWISprite.css",
			SpreadsheetDefaultCssResourceName = SpreadsheetStyleResourcePath + "default.css",
			SpreadsheetFileManagerScriptResourceName = SpreadsheetScriptsResourcePath + "FileManager.js",
			SpreadsheetFolderManagerScriptResourceName = SpreadsheetScriptsResourcePath + "FolderManager.js",
			SpreadsheetTabControlScriptResourceName = SpreadsheetScriptsResourcePath + "TabControl.js",
			SpreadsheetFormulaParserScriptResourceName = SpreadsheetScriptsResourcePath + "FormulaParser.js",
			SpreadsheetSelectionHelperScriptResourceName = SpreadsheetScriptsResourcePath + "SelectionHelper.js",
			SpreadsheetStateControllerScriptResourceName = SpreadsheetScriptsResourcePath + "StateController.js",
			SpreadsheetUploadControlScriptResourceName = SpreadsheetScriptsResourcePath + "UploadControl.js",
			SpreadsheetFileManagerUploadControlScriptResourceName = SpreadsheetScriptsResourcePath + "FileManagerUploadControl.js",
			SpreadsheetInputControllerScriptResourceName = SpreadsheetScriptsResourcePath + "InputController.js",
			SpreadsheetPopupMenuHelperScriptResourceName = SpreadsheetScriptsResourcePath + "PopupMenuHelper.js",
			SpreadsheetPaneManagerScriptResourceName = SpreadsheetScriptsResourcePath + "PaneManager.js",
			SpreadsheetPaneViewScriptResourceName = SpreadsheetScriptsResourcePath + "PaneView.js",
			SpreadsheetFormulaIntelliSenseManagerScriptResourceName = SpreadsheetScriptsResourcePath + "FormulaIntelliSenseManager.js",
			SpreadsheetFunctionsListBoxScriptResourceName = SpreadsheetScriptsResourcePath + "FunctionsListBox.js",
			SpreadsheetValidationHelperScriptResourceName = SpreadsheetScriptsResourcePath + "Validation.js",
			SpreadsheetRenderProviderScriptResourceName = SpreadsheetScriptsResourcePath + "RenderProvider.js",
		#region ClientSideLocalizationResources
			SpreadsheetStringResourcesScriptsResourcePath = SpreadsheetScriptsResourcePath + "StringResources.",
			SpreadsheetFunctions_en_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions.js",
			SpreadsheetFunctions_ar_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ar.js",
			SpreadsheetFunctions_bg_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_bg.js",
			SpreadsheetFunctions_ca_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ca.js",
			SpreadsheetFunctions_cs_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_cs.js",
			SpreadsheetFunctions_da_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_da.js",
			SpreadsheetFunctions_deCH_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_de-CH.js",
			SpreadsheetFunctions_de_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_de.js",
			SpreadsheetFunctions_el_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_el.js",
			SpreadsheetFunctions_es_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_es.js",
			SpreadsheetFunctions_et_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_et.js",
			SpreadsheetFunctions_fa_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_fa.js",
			SpreadsheetFunctions_fi_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_fi.js",
			SpreadsheetFunctions_fr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_fr.js",
			SpreadsheetFunctions_he_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_he.js",
			SpreadsheetFunctions_hi_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_hi.js",
			SpreadsheetFunctions_hr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_hr.js",
			SpreadsheetFunctions_hu_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_hu.js",
			SpreadsheetFunctions_hy_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_hy.js",
			SpreadsheetFunctions_id_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_id.js",
			SpreadsheetFunctions_is_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_is.js",
			SpreadsheetFunctions_it_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_it.js",
			SpreadsheetFunctions_ja_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ja.js",
			SpreadsheetFunctions_kk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_kk.js",
			SpreadsheetFunctions_ko_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ko.js",
			SpreadsheetFunctions_lt_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_lt.js",
			SpreadsheetFunctions_lv_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_lv.js",
			SpreadsheetFunctions_mk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_mk.js",
			SpreadsheetFunctions_ms_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ms.js",
			SpreadsheetFunctions_nl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_nl.js",
			SpreadsheetFunctions_no_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_no.js",
			SpreadsheetFunctions_pl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_pl.js",
			SpreadsheetFunctions_ptBR_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_pt-BR.js",
			SpreadsheetFunctions_pt_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_pt.js",
			SpreadsheetFunctions_ro_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ro.js",
			SpreadsheetFunctions_ru_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ru.js",
			SpreadsheetFunctions_sk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sk.js",
			SpreadsheetFunctions_sl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sl.js",
			SpreadsheetFunctions_srCyrlCS_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sr-Cyrl-CS.js",
			SpreadsheetFunctions_srLatnBA_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sr-Latn-BA.js",
			SpreadsheetFunctions_srLatnCS_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sr-Latn-CS.js",
			SpreadsheetFunctions_sv_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_sv.js",
			SpreadsheetFunctions_ta_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_ta.js",
			SpreadsheetFunctions_th_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_th.js",
			SpreadsheetFunctions_tr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_tr.js",
			SpreadsheetFunctions_uk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_uk.js",
			SpreadsheetFunctions_vi_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_vi.js",
			SpreadsheetFunctions_zhCN_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_zh-CN.js",
			SpreadsheetFunctions_zhHans_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_zh-Hans.js",
			SpreadsheetFunctions_zhHant_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_zh-Hant.js",
			SpreadsheetFunctions_zhTW_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Functions_zh-TW.js",
			SpreadsheetConfigScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Config.js",
			SpreadsheetLocalization_en_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization.js",
			SpreadsheetLocalization_ar_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ar.js",
			SpreadsheetLocalization_bg_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_bg.js",
			SpreadsheetLocalization_ca_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ca.js",
			SpreadsheetLocalization_cs_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_cs.js",
			SpreadsheetLocalization_da_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_da.js",
			SpreadsheetLocalization_de_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_de.js",
			SpreadsheetLocalization_deCH_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_de-CH.js",
			SpreadsheetLocalization_el_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_el.js",
			SpreadsheetLocalization_es_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_es.js",
			SpreadsheetLocalization_fa_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_fa.js",
			SpreadsheetLocalization_fi_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_fi.js",
			SpreadsheetLocalization_fr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_fr.js",
			SpreadsheetLocalization_he_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_he.js",
			SpreadsheetLocalization_hr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_hr.js",
			SpreadsheetLocalization_hu_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_hu.js",
			SpreadsheetLocalization_hy_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_hy.js",
			SpreadsheetLocalization_is_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_is.js",
			SpreadsheetLocalization_it_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_it.js",
			SpreadsheetLocalization_ja_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ja.js",
			SpreadsheetLocalization_ko_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ko.js",
			SpreadsheetLocalization_lt_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_lt.js",
			SpreadsheetLocalization_lv_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_lv.js",
			SpreadsheetLocalization_mk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_mk.js",
			SpreadsheetLocalization_nl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_nl.js",
			SpreadsheetLocalization_no_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_no.js",
			SpreadsheetLocalization_pl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_pl.js",
			SpreadsheetLocalization_pt_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_pt.js",
			SpreadsheetLocalization_ptBR_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_pt-BR.js",
			SpreadsheetLocalization_ro_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ro.js",
			SpreadsheetLocalization_ru_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ru.js",
			SpreadsheetLocalization_sk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sk.js",
			SpreadsheetLocalization_sl_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sl.js",
			SpreadsheetLocalization_srCyrlCS_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sr-Cyrl-CS.js",
			SpreadsheetLocalization_srLatinBa_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sr-Latin-Ba.js",
			SpreadsheetLocalization_srLatinCS_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sr-Latin-CS.js",
			SpreadsheetLocalization_sv_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_sv.js",
			SpreadsheetLocalization_ta_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_ta.js",
			SpreadsheetLocalization_tr_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_tr.js",
			SpreadsheetLocalization_uk_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_uk.js",
			SpreadsheetLocalization_vi_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_vi.js",
			SpreadsheetLocalization_zhCN_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_zh-CN.js",
			SpreadsheetLocalization_zhHans_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_zh-Hans.js",
			SpreadsheetLocalization_zhHant_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_zh-Hant.js",
			SpreadsheetLocalization_zhTW_ScriptResourceName = SpreadsheetStringResourcesScriptsResourcePath + "Localization_zh-TW.js";
		#endregion
		static readonly string FunctionsScriptResourceNamePattern = SpreadsheetStringResourcesScriptsResourcePath + "Functions{0}.js";
		static readonly string LocalizationScriptResourceNamePattern = SpreadsheetStringResourcesScriptsResourcePath + "Localization{0}.js";
		static readonly List<string> AvailableCultures = new List<string>(new string[] {			
			"ar", "bg", "ca", "cs", "da", "de", "de-CH", "el", "es", "fa", "fi", "fr", "he", "hr", "hu", "hy", "is", "it",
			"ja", "ko", "lt", "lv", "mk", "nl", "no", "pl", "pt", "pt-BR", "ro", "ru", "sk", "sl", "sr-Cyrl-CS", "sr-Latin-Ba",
			"sr-Latin-CS", "sv", "ta", "tr", "uk", "vi", "zh-CN", "zh-Hans", "zh-Hant", "zh-TW"
		});
		protected internal const string WorkBookContainerID = "WBC";
		protected internal const string SpreadsheetRibbonContainerID = "SSRC";
		protected internal const string SpreadsheetInputTargetID = "IT";
		protected internal const string SpreadsheetPopupMenuContainerID = "SSPUM";
		protected internal const string SpreadsheetPopupDialogContainerID = "SSDC";
		protected internal const string SpreadsheetAutoFilterPopupMenuContainerID = "SSAFPM";
		protected internal const string SpreadsheetTabContainerID = "SSTC";
		protected internal const string SpreadsheetDocumentUpdatePrefix = "SSDU";
		protected internal const string SpreadsheetFormulaBarID = "SSFB";
		protected internal const string SpreadsheetFormulaBarTextBoxID = "SSFBTB";
		protected internal const string SpreadsheetFormulaBarMenuID = "SSFBM";
		protected internal const string SpreadsheetFunctionsListBoxID = "SSFLB";
		protected internal const string SpreadsheetFormulaBarMenuEnterItemName = "Enter";
		protected internal const string SpreadsheetFormulaBarMenuCancelItemName = "Cancel";
		SpreadsheetRibbonTabCollection ribbonTabs;
		SpreadsheetRibbonContextTabCategoryCollection ribbonContextTabCategories;
		protected internal static string[] DialogNames = new string[] { "inserthyperlinkdialog", "editlinkdialog", "insertpicturedialog", "renamesheetdialog", "openfiledialog", "savefiledialog", 
			"rowheightdialog", "columnwidthdialog", "defaultcolumnwidthdialog", "unhidesheetdialog", "changecharttypedialog", "chartselectdatadialog", "modifychartlayoutdialog", 
			"chartchangetitledialog", "chartchangehorizontalaxistitledialog", "chartchangeverticalaxistitledialog", "modifychartstyledialog", "findandreplacedialog", "datafiltersimpledialog",
			"customdatafilterdialog", "customdatetimefilterdialog", "datafiltertop10dialog", "tableselectdatadialog", "datavalidationdialog", "validationconfirmdialog", "moveorcopysheetdialog", "modifytablestyledialog", 
			"pagesetupdialog" };
		protected internal static string[] UniqueFormNames = new string[] { "InsertLinkForm", "InsertPictureForm", "RenameSheetForm", "OpenFileForm", "SaveFileForm", "RowHeightForm", 
			"ColumnWidthForm", "DefaultColumnWidthForm", "UnhideSheetForm", "ChangeChartTypeForm", "ChartSelectDataForm", "ModifyChartLayoutForm", "ChartChangeTitleForm", 
			"ChartChangeHorizontalAxisTitleForm", "ChartChangeVerticalAxisTitleForm", "ModifyChartStyleForm", "FindAndReplaceForm", "DataFilterSimpleForm", "CustomDataFilterForm", 
			"CustomDateTimeFilterForm", "DataFilterTop10DialogForm", "TableSelectDataDialogForm", "DataValidationDialogForm", "ValidationConfirmDialogForm", "MoveOrCopySheetDialogForm", "ModifyTableStyleDialogForm",
			"PageSetupDialogForm" };
		protected internal static string[] DialogFormNames = new string[] { UniqueFormNames[0], UniqueFormNames[0], UniqueFormNames[1], UniqueFormNames[2], UniqueFormNames[3], UniqueFormNames[4], 
			UniqueFormNames[5], UniqueFormNames[6], UniqueFormNames[7], UniqueFormNames[8], UniqueFormNames[9], UniqueFormNames[10], UniqueFormNames[11], UniqueFormNames[12], UniqueFormNames[13],
			UniqueFormNames[14],UniqueFormNames[15],UniqueFormNames[16], UniqueFormNames[17], UniqueFormNames[18], UniqueFormNames[19], UniqueFormNames[20], UniqueFormNames[21], UniqueFormNames[22],
			UniqueFormNames[23], UniqueFormNames[24], UniqueFormNames[25], UniqueFormNames[26] };
		protected internal const string CustomeDialogNamePrefix = "ss_cd_";
		protected internal const string InternalCallbackPostfix = "%Spread%Sheet";
		protected internal const string UploadControlUrlParametr = "DXSSUC";
		protected internal const string DefaultWorkDirectory = "~/App_Data/WorkDirectory";
		protected internal const string DefaultImageDirectory = "~/";
		SpreadsheetFormsSettings settingsForms;
		SpreadsheetDialogFormSettings settingsDialogForm;
		SpreadsheetDocumentSelectorSettings settingsDocumentSelector;
		SpreadsheetDialogSettings settingsDialogs;
		SpreadsheetTabControlStyles stylesTabControl;
		SpreadsheetRibbonStyles stylesRibbon;
		SpreadsheetMenuStyles stylesPopupMenu;
		SpreadsheetButtonStyles stylesButton;
		SpreadsheetEditorsStyles stylesEditors;
		SpreadsheetFileManagerStyles stylesFileManager;
		SpreadsheetFormulaBarStyles stylesFormulaBar;
		SpreadsheetFormulaAutoCompeteStyles stylesFormulaAutoCompete;
		SpreadsheetFileManagerImages imagesFileManager;
		string internalCallbackArgument = "";
		Dictionary<string, string> dialogFormNamesDictionary = new Dictionary<string, string>();
		Control currentDialogForm;
		WebControl containerControl = null;
		SpreadsheetControl spreadsheetWebConrol = null;
		SpreadsheetControl SpreadsheetWebControl {
			get {
				if(spreadsheetWebConrol == null)
					spreadsheetWebConrol = DesignMode ? new SpreadsheetControlDesignTime(this) : new SpreadsheetControl(this);
				return spreadsheetWebConrol;
			}
		}
		private static readonly object EventCallback = new object();
		private static readonly object EventDocumentCallback = new object();
		private static readonly object DocumentSelectorFolderCreatingEventKey = new object();
		private static readonly object DocumentSelectorItemRenamingEventKey = new object();
		private static readonly object DocumentSelectorItemDeletingEventKey = new object();
		private static readonly object DocumentSelectorItemMovingEventKey = new object();
		private static readonly object DocumentSelectorFileUploadingEventKey = new object();
		private static readonly object DocumentSelectorItemCopyingEventKey = new object();
		private static readonly object DocumentSelectorCloudProviderRequestEventKey = new object();
		private static readonly object EventSaving = new object();
		static ASPxSpreadsheet() {
			ASPxHttpHandlerModule.Subscribe(new DocumentWorkSessionManagerSubscriber());
		}
		public ASPxSpreadsheet()
			: base() {
			FillDialogFormNamesDictionary();
			this.settingsForms = CreateSettingsForms();
			this.settingsDialogForm = CreateSettingsDialogForm();
			this.settingsDocumentSelector = CreateSettingsDocumentSelector();
			this.settingsDialogs = CreateSettingsDialogs();
			this.stylesTabControl = new SpreadsheetTabControlStyles(this);
			this.stylesButton = new SpreadsheetButtonStyles(this);
			this.stylesEditors = new SpreadsheetEditorsStyles(this);
			this.stylesPopupMenu = new SpreadsheetMenuStyles(this);
			this.stylesRibbon = new SpreadsheetRibbonStyles(this);
			this.stylesFileManager = new SpreadsheetFileManagerStyles(this);
			this.stylesFormulaBar = new SpreadsheetFormulaBarStyles(this);
			this.stylesFormulaAutoCompete = new SpreadsheetFormulaAutoCompeteStyles(this);
			this.imagesFileManager = new SpreadsheetFileManagerImages(this);
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetImages Images { get { return (SpreadsheetImages)ImagesInternal; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetRibbonTabs"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), Category("Ribbon")]
		public SpreadsheetRibbonTabCollection RibbonTabs {
			get {
				if(ribbonTabs == null)
					ribbonTabs = new SpreadsheetRibbonTabCollection(this);
				return ribbonTabs;
			}
		}
		[
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), Category("Ribbon")]
		public SpreadsheetRibbonContextTabCategoryCollection RibbonContextTabCategories {
			get {
				if(ribbonContextTabCategories == null)
					ribbonContextTabCategories = new SpreadsheetRibbonContextTabCategoryCollection(this);
				return ribbonContextTabCategories;
			}
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetActiveTabIndex"),
#endif
		Category("Ribbon"), DefaultValue(1), AutoFormatDisable]
		public int ActiveTabIndex {
			get { return GetIntProperty("ActiveTabIndex", 1); }
			set { SetIntProperty("ActiveTabIndex", 1, value); }
		}
		[
		Category("Ribbon"), DefaultValue(SpreadsheetRibbonMode.Auto), AutoFormatDisable, Localizable(false)]
		public SpreadsheetRibbonMode RibbonMode {
			get { return (SpreadsheetRibbonMode)GetEnumProperty("RibbonMode", SpreadsheetRibbonMode.Auto); }
			set {
				SetEnumProperty("RibbonMode", SpreadsheetRibbonMode.Auto, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetAssociatedRibbonID"),
#endif
		Category("Ribbon"), DefaultValue(""), AutoFormatDisable,
		TypeConverter("DevExpress.Web.Design.RibbonControlIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		NotifyParentProperty(true), Localizable(false)]
		public string AssociatedRibbonID {
			get { return GetStringProperty("AssociatedRibbonID", string.Empty); }
			set { SetStringProperty("AssociatedRibbonID", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetTemporaryFolder"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false), 
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsDocumentSelector.EditingSettings.TemporaryFolder property instead.")]
		public string TemporaryFolder {
			get { return GetStringProperty("TemporaryFolder", ""); }
			set { SetStringProperty("TemporaryFolder", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetWorkDirectory"),
#endif
		DefaultValue(ASPxSpreadsheet.DefaultWorkDirectory), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public string WorkDirectory {
			get { return GetStringProperty("WorkDirectory", ASPxSpreadsheet.DefaultWorkDirectory); }
			set { SetStringProperty("WorkDirectory", ASPxSpreadsheet.DefaultWorkDirectory, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetReadOnly"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool ReadOnly {
			get { return GetBoolProperty("ReadOnly", false); }
			set {
				SetBoolProperty("ReadOnly", false, value);
				AssignReadOnlyToInnerControl();
			}
		}
		[Browsable(false), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetStyles Styles {
			get { return StylesInternal as SpreadsheetStyles; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesTabControl"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetTabControlStyles StylesTabControl {
			get { return stylesTabControl; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesRibbon"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetRibbonStyles StylesRibbon {
			get { return stylesRibbon; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesPopupMenu"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetMenuStyles StylesPopupMenu {
			get { return stylesPopupMenu; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesButton"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetButtonStyles StylesButton {
			get { return stylesButton; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetEditorsStyles StylesEditors {
			get { return stylesEditors; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public SpreadsheetClientSideEvents ClientSideEvents {
			get { return (SpreadsheetClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || !Enabled;
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetStylesFileManager"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerStyles StylesFileManager {
			get { return stylesFileManager; }
		}
		[
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFormulaBarStyles StylesFormulaBar {
			get { return stylesFormulaBar; }
		}
		[
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFormulaAutoCompeteStyles StylesFormulaAutoCompete {
			get { return stylesFormulaAutoCompete; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSettingsForms"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public SpreadsheetFormsSettings SettingsForms {
			get { return settingsForms; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSettingsDialogForm"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), Category("Settings"),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		AutoFormatDisable, Obsolete("This property is now obsolete. Use the SettingsDialogs property instead")]
		public SpreadsheetDialogFormSettings SettingsDialogForm { get { return settingsDialogForm; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ASPxSpreadsheetLoadingPanelSettings SettingsLoadingPanel {
			get { return (ASPxSpreadsheetLoadingPanelSettings)base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSettingsDocumentSelector"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetDocumentSelectorSettings SettingsDocumentSelector {
			get { return this.settingsDocumentSelector; }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSettingsDialogs"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), Category("Settings"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public SpreadsheetDialogSettings SettingsDialogs { get { return settingsDialogs; } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetShowConfirmOnLosingChanges"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowConfirmOnLosingChanges {
			get { return GetBoolProperty("ShowConfirmOnLosingChanges", true); }
			set {
				if(value == ShowConfirmOnLosingChanges)
					return;
				SetBoolProperty("ShowConfirmOnLosingChanges", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetConfirmOnLosingChanges"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ConfirmOnLosingChanges {
			get { return GetStringProperty("ConfirmOnLosingChanges", string.Empty); }
			set {
				if(value == ConfirmOnLosingChanges)
					return;
				SetStringProperty("ConfirmOnLosingChanges", string.Empty, value);
			}
		}
		[ DefaultValue(AutoSaveMode.Default), NotifyParentProperty(true)]
		public AutoSaveMode AutoSaveMode {
			get { return (AutoSaveMode)GetEnumProperty("AutoSaveMode", AutoSaveMode.Default); }
			set { 
				bool turnOn = value == Office.Internal.AutoSaveMode.On && AutoSaveMode != value;
				if(turnOn)
					WorkSessionProcessing.Start();
				SetEnumProperty("AutoSaveMode", AutoSaveMode.Default, value); 
			}
		}
		[ DefaultValue(typeof(TimeSpan), AutoSaveDefaultSettings.DefaultAutoSaveTimeoutSecondsString), NotifyParentProperty(true)]
		public TimeSpan AutoSaveTimeout  {
			get { return (TimeSpan)(GetObjectProperty("AutoSaveTimeout", AutoSaveDefaultSettings.DefaultAutoSaveTimeout)); }
			set { SetObjectProperty("AutoSaveTimeout", AutoSaveDefaultSettings.DefaultAutoSaveTimeout, value); }
		}
		[
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowFormulaBar {
			get { return GetBoolProperty("ShowFormulaBar", true); }
			set {
				SetBoolProperty("ShowFormulaBar", true, value);
				LayoutChanged();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {				
				StylesButton,
				StylesEditors,
				StylesFileManager,
				StylesPopupMenu,
				StylesRibbon,
				StylesTabControl,
				StylesFormulaBar,
				StylesFormulaAutoCompete,
				SettingsForms,
				SettingsDocumentSelector,
				SettingsDialogs,
#pragma warning disable 618
				SettingsDialogForm,
#pragma warning restore 618
				SettingsLoadingPanel,
				RibbonTabs,
				RibbonContextTabCategories
			});
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocument"),
#endif
		AutoFormatDisable,  
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IWorkbook Document {
			get {
				var currentWorkSession = GetCurrentWorkSessions();
				LayoutChanged();
				if(currentWorkSession != null) {
					return currentWorkSession.WebSpreadsheetControl.InnerControl.Document;
				}
				return null;
			}
		}
		[
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DocumentId { 
			get { 
				var workSession = GetCurrentWorkSessions();
				return workSession != null ? workSession.DocumentPathOrID : null; 
			}
			set {
				if(value == DocumentId) return;
				WorkSessions.CheckDocumentIsUnique(value);
				var workSession = GetCurrentWorkSessions();
				workSession.DocumentPathOrID = value; 
			}
		}
		Guid workSessionGuid = Guid.Empty;
		protected Guid WorkSessionGuid {
			get { return workSessionGuid; }
		}
		protected Control CurrentDialogForm {
			get { return currentDialogForm; }
			set { currentDialogForm = value; }
		}
		protected internal WebControl ContainerControl {
			get { return containerControl ?? this; }
		}
		protected string CurrentDialogName { get { return GetClientObjectStateValue<string>("currentDialog"); } }
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback {
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentCallback"),
#endif
		Category("Action"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This event is now obsolete. Use the Callback event instead")]
		public event CallbackEventHandlerBase DocumentCallback {
			add { Events.AddHandler(EventDocumentCallback, value); }
			remove { Events.RemoveHandler(EventDocumentCallback, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorFolderCreating"),
#endif
		Category("Action")]
		public event FileManagerFolderCreateEventHandler DocumentSelectorFolderCreating {
			add { Events.AddHandler(DocumentSelectorFolderCreatingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorFolderCreatingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorItemRenaming"),
#endif
		Category("Action")]
		public event FileManagerItemRenameEventHandler DocumentSelectorItemRenaming {
			add { Events.AddHandler(DocumentSelectorItemRenamingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemRenamingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorItemDeleting"),
#endif
		Category("Action")]
		public event FileManagerItemDeleteEventHandler DocumentSelectorItemDeleting {
			add { Events.AddHandler(DocumentSelectorItemDeletingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemDeletingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorItemMoving"),
#endif
		Category("Action")]
		public event FileManagerItemMoveEventHandler DocumentSelectorItemMoving {
			add { Events.AddHandler(DocumentSelectorItemMovingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemMovingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorFileUploading"),
#endif
		Category("Action")]
		public event FileManagerFileUploadEventHandler DocumentSelectorFileUploading {
			add { Events.AddHandler(DocumentSelectorFileUploadingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorFileUploadingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorItemCopying"),
#endif
		Category("Action")]
		public event FileManagerItemCopyEventHandler DocumentSelectorItemCopying {
			add { Events.AddHandler(DocumentSelectorItemCopyingEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorItemCopyingEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetDocumentSelectorCloudProviderRequest"),
#endif
		Category("Action")]
		public event FileManagerCloudProviderRequestEventHandler DocumentSelectorCloudProviderRequest {
			add { Events.AddHandler(DocumentSelectorCloudProviderRequestEventKey, value); }
			remove { Events.RemoveHandler(DocumentSelectorCloudProviderRequestEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetSaving"),
#endif
		Category("Action")]
		public event DocumentSavingEventHandler Saving {
			add { Events.AddHandler(EventSaving, value); }
			remove { Events.RemoveHandler(EventSaving, value); }
		}
		protected Dictionary<string, string> DialogFormNamesDictionary {
			get { return dialogFormNamesDictionary; }
		}
		protected override ImagesBase CreateImages() {
			return new SpreadsheetImages(this);
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("ASPxSpreadsheetImagesFileManager"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SpreadsheetFileManagerImages ImagesFileManager {
			get { return imagesFileManager; }
		}
		protected void OpenWorkSession(Guid sessionGuidId, DocumentContentContainer documentContentContainer, string clientId) {
			ChangeDocumentWorkSession(sessionGuidId, documentContentContainer);
			PrepareInnerControl();
		}
		protected void ChangeDocumentWorkSession(Guid sessionGuidId, DocumentContentContainer documentContentContainer) {
			ChangeDocumentWorkSession(sessionGuidId, documentContentContainer, true);
		}
		protected void ChangeDocumentWorkSession(Guid sessionGuidId, DocumentContentContainer documentContentContainer, bool reasonIsOpening) {
			bool reasonIsSaveAs = !reasonIsOpening;
			Guid sessionGuid = sessionGuidId;
			if(reasonIsOpening)
				sessionGuid = SpreadsheetWorkSessionsUtils.OpenWorkSession(sessionGuidId, documentContentContainer);
			else if(reasonIsSaveAs)
				sessionGuid = SpreadsheetWorkSessionsUtils.SaveAsWorkSession(sessionGuidId, documentContentContainer);
			SetSessionGuid(sessionGuid);		
		}
		public void New() {
			CreateNewWorkSession();
		}
		public void Save() {  
			SpreadsheetWorkSession currentSession = GetCurrentWorkSessions();
			bool calledFromSavingEventHanlder = RaiseSavingCallIsLocked();
			DocumentSavingEventArgs args = calledFromSavingEventHanlder ? null : RaiseSaving(currentSession.DocumentPathOrID, MultiUserConflict.None);
			if(calledFromSavingEventHanlder || !args.Handled) {
				currentSession.SaveInTheSameFile();
			}
			if(args.Handled && Document.Modified) {
				Document.Modified = false;
			}
		}
		public void SaveCopy(string documentPath) { 
			string currentDocPathBackUp = Document.Path;
			Document.SaveDocument(documentPath);
			Document.Options.Save.CurrentFileName = currentDocPathBackUp;
		}
		public void SaveCopy(Stream stream, DocumentFormat format) { 
			Document.SaveDocument(stream, format);
		}
		public byte[] SaveCopy(DocumentFormat format) { 
			return Document.SaveDocument(format);
		}
		public void Open(string pathToDocument) {
			var documentContentContainer = new DocumentContentContainer(pathToDocument);
			OpenCore(documentContentContainer);
		}
		public void Open(string pathToDocument, DocumentFormat format) {
			var documentContentContainer = new DocumentContentContainer(pathToDocument, format.ToString());
			OpenCore(documentContentContainer);
		}
		public void Open(string documentId, DocumentFormat format, Func<Stream> contentAccessor) {
			if(!ConnectToOpenedDocumentById(documentId)) {
				Stream stream = contentAccessor();
				var documentContentContainer = new DocumentContentContainer(stream, format.ToString(), documentId);
				OpenCore(documentContentContainer);
			}
		}
		public void Open(string documentId, DocumentFormat format, Func<byte[]> contentAccessor) {
			if(!ConnectToOpenedDocumentById(documentId)) {
				byte[] array = contentAccessor();
				var documentContentContainer = new DocumentContentContainer(array, format.ToString(), documentId);
				OpenCore(documentContentContainer);
			}
		}
		public void Open(SpreadsheetDocumentInfo documentInfo) {
			SetSessionGuid(documentInfo.WorkSessionGuid);
		}
		protected void OpenCore(DocumentContentContainer documentContentContainer) {
			 if (!IsInternalServiceCallback()) {
				OpenWorkSession(WorkSessionGuid, documentContentContainer, ClientID);
				ResetControlHierarchy();
			}
		}
		protected bool ConnectToOpenedDocumentById(string documentId) {
			SpreadsheetDocumentInfo documentInfo = DocumentManager.FindDocument(documentId) as SpreadsheetDocumentInfo;
			bool documentWithThisDocumentIdAlreadyOpened = documentInfo != null;
			if(documentWithThisDocumentIdAlreadyOpened)
				Open(documentInfo);
			return documentWithThisDocumentIdAlreadyOpened;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This method is now obsolete. Use the DocumentManager.CloseDocument method instead.")]
		public void Close() {
			CloseDocumentInternal();
		}
		protected void CloseDocumentInternal() {
			CloseCurrentSession();
			ResetControlHierarchy();
		}
		public virtual bool IsInternalServiceCallback() {
			if(Page != null && (Page.IsCallback || Page.IsPostBack)) {
				if(Request != null && Request.Params != null) {
					string callbackParams = string.Empty;
					if(Page.IsCallback) {
						callbackParams = Request.Params[RenderUtils.CallbackControlParamParamName];
					} else if(Page.IsPostBack) {
						callbackParams = Request.Params[RenderUtils.PostbackEventArgumentParamName];
					}
					return callbackParams.EndsWith(InternalCallbackPostfix) || IsInternalUploadControlPostback();
				}
			}
			return false;
		}
		protected virtual bool IsInternalUploadControlPostback() {
			if(Page != null && (Page.IsCallback || Page.IsPostBack)) {
				if(Request != null && Request.Params != null) {
					string callbackParams = string.Empty;
					if(Page.IsPostBack) {
						callbackParams = Request.Params[ASPxSpreadsheet.UploadControlUrlParametr];
					}
					return callbackParams != null && callbackParams.Equals(ASPxSpreadsheet.InternalCallbackPostfix);
				}
			}
			return false;
		}
		#region OfficeWorkSessionControl Members
		Guid OfficeWorkSessionControl.GetWorkSessionID() {
			return WorkSessionGuid;
		}
		void OfficeWorkSessionControl.AttachToWorkSession(Guid workSessionID) {
			SetSessionGuid(workSessionID);
		}
		#endregion
		protected void CloseCurrentSession() {
			WorkSessions.CloseWorkSession(WorkSessionGuid);
			SetSessionGuid(Guid.Empty);
		}
		protected void SetSessionGuid(Guid sessionGuid) {
			WorkSessions.OnControlDetachFromWorkSession(this.workSessionGuid);
			this.workSessionGuid = sessionGuid;
			if(WorkSessionGuid != Guid.Empty)
				SyncWorkSessionSettings();
		}
		protected void SyncWorkSessionSettings() {
			SpreadsheetWorkSession session = GetCurrentWorkSessions();
			if(session != null)
				session.SyncSettingWithControl(this);
		}
		protected internal SpreadsheetWorkSession GetCurrentWorkSessions() {
			EnsureWorkSession();
			return (SpreadsheetWorkSession)WorkSessions.Get(WorkSessionGuid, true);
		}
		static object locker = new object();
		protected void CheckWorkDirectoryAccess() {
			lock(locker) {
				FileUtils.CheckOrCreateDirectory(WorkDirectory, this, "WorkDirectory");
			}
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new SpreadsheetClientSideEvents();
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!DesignMode) {
				CheckWorkDirectoryAccess();
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(Page == null)
				return;
			if(Page.IsCallback || Page.IsPostBack)
				LoadDocumentClientState();
		}
		protected void LoadDocumentClientState() {
			LoadWorkSessionIdFromRequest();
			LoadSelectionState();
		}
		protected NameValueCollection GetClientState() {
			if(ClientObjectState == null)
				return new NameValueCollection();
			return HttpUtility.ParseQueryString(GetClientObjectStateValue<string>("state"));
		}
		protected void LoadSelectionState() {
			NameValueCollection controlClientState = GetClientState();
			if(controlClientState != null && controlClientState.Count > 0) {
				SpreadsheetWorkSession session = GetCurrentWorkSessions();
				if(session != null)
					SelectionHelper.LoadClientSelection(session.DocumentModel.ActiveSheet, controlClientState);
			}			
		}
		protected void LoadWorkSessionIdFromRequest() {
			if(ClientObjectState == null) return;
			Guid clientWorkSessionID = DocumentRequestHelper.GetSessionGuid(GetClientObjectStateValue<string>(DocumentRequestHelper.SessionGuidStateKey));
			SetSessionGuid(clientWorkSessionID);
		}
		protected override string GetSkinControlName() {
			return "Spreadsheet";
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxSpreadsheet), SpreadsheetSystemCssResourceName);
		}
		protected override void RegisterDefaultRenderCssFile() {
			base.RegisterDefaultRenderCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxSpreadsheet), SpreadsheetDefaultCssResourceName);
		}
		protected override void RegisterCustomSpriteCssFile(string spriteCssFile) {
			base.RegisterCustomSpriteCssFile(spriteCssFile);
			if(Images.MenuIconSet != MenuIconSetType.NotSet)
				Images.RegisterIconSpriteCssFile(Page);
		}
		protected override void RegisterDefaultSpriteCssFile() {
			base.RegisterDefaultSpriteCssFile();
			Images.RegisterIconSpriteCssFile(Page);
		}
		protected override StylesBase CreateStyles() {
			return new SpreadsheetStyles(this);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDialogUtilsScripts();
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetTileHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetScrollHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetGridResizingHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetTileMatrixScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetCommandsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetDynamicSelectionScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetSelectionScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetEditingScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetKeyboardManagerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetRibbonManagerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetDialogsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetFormulaParserScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetSelectionHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetStateControllerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetInputControllerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetPopupMenuHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetPaneManagerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetPaneViewScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetFormulaIntelliSenseManagerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetValidationHelperScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetRenderProviderScriptResourceName);
			RegisterIncludeScript(typeof(ASPxSpreadsheet), SpreadsheetConfigScriptResourceName);
			RegisterFunctionsLocalizationScript();
			RegisterMenuLocalizationScript();
		}
		#region ClientSideLocalizationResourcesIncluding
		protected void RegisterFunctionsLocalizationScript() {
			CultureInfo culture = GetCurrentWorkSessions().DocumentModel.Culture;
			RegisterLocalizationScript(culture, FunctionsScriptResourceNamePattern, SpreadsheetFunctions_en_ScriptResourceName,
				IsFunctionLocalizationResourceAvailable);
		}
		protected void RegisterMenuLocalizationScript() {
			CultureInfo culture = CultureInfo.CurrentUICulture;
			if(!AvailableCultures.Contains(culture.Name))
				culture = new CultureInfo(culture.Name.Split('-')[0]);
			RegisterLocalizationScript(culture, LocalizationScriptResourceNamePattern, SpreadsheetLocalization_en_ScriptResourceName, null);
		}
		protected void RegisterLocalizationScript(CultureInfo culture, string fileNamePattern,
			string defaultFileName, LocalizationResourceChecker checkResourceAvailability) {
			string fileNamePostfix = GetRequiredScriptFileNamePostfix(culture, checkResourceAvailability);
			try {
				RegisterIncludeScript(typeof(ASPxSpreadsheet), string.Format(fileNamePattern, fileNamePostfix));
			}
			catch {
				RegisterIncludeScript(typeof(ASPxSpreadsheet), defaultFileName);
			}
		}
		protected string GetRequiredScriptFileNamePostfix(CultureInfo culture, LocalizationResourceChecker checkResourceAvailability) {
			string result = GetRequiredScriptFileNamePostfixCore(culture, checkResourceAvailability);
			if(!string.IsNullOrEmpty(result))
				result = "_" + result;
			return result;
		}
		protected string GetRequiredScriptFileNamePostfixCore(CultureInfo culture, LocalizationResourceChecker checkResourceAvailability) {
			if(checkResourceAvailability == null)
				return culture.Name;
			bool isCultureResourceAvailable = checkResourceAvailability(culture);			
			if(culture.IsNeutralCulture)
				return isCultureResourceAvailable ? culture.Name : string.Empty;
			string neutralCultureName = culture.Name.Split('-')[0];
			bool isNeutralCultureResourceAvailable = checkResourceAvailability(new CultureInfo(neutralCultureName));
			if(isNeutralCultureResourceAvailable)
				return isCultureResourceAvailable ? culture.Name : neutralCultureName;
			return string.Empty;
		}
		protected delegate bool LocalizationResourceChecker(CultureInfo culture);
		static bool IsFunctionLocalizationResourceAvailable(CultureInfo culture) {
			return new XtraSpreadsheetFunctionNameResLocalizer().Manager.GetResourceSet(culture, true, false) != null
				|| new XtraSpreadsheetFunctionDescriptionResLocalizer().Manager.GetResourceSet(culture, true, false) != null
				|| new XtraSpreadsheetFunctionArgumentsNamesResLocalizer().Manager.GetResourceSet(culture, true, false) != null
				|| new XtraSpreadsheetFunctionArgumentsDescriptionsResLocalizer().Manager.GetResourceSet(culture, true, false) != null;
		}
		#endregion
		protected override string GetClientObjectClassName() {
			return "ASPxClientSpreadsheet";
		}
		public void CreateDefaultRibbonTabs(bool clearExistingTabs) {
			if(RibbonMode == SpreadsheetRibbonMode.ExternalRibbon) {
				ASPxRibbon ribbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
				if(ribbon != null)
					SpreadsheetRibbonHelper.AddTabCollectionToControl(ribbon, new SpreadsheetDefaultRibbon(this).DefaultRibbonTabs, clearExistingTabs);
			} else{
				if(clearExistingTabs)
					RibbonTabs.Clear();
				RibbonTabs.CreateDefaultRibbonTabs();
			} 
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			string script = string.Format(@"ASPx.SpreadsheetDialog.Titles={{InsertImage:{0},InsertLink:{1},ChangeLink:{2},RenameSheet:{3},OpenFile:{4},SaveFile:{5},RowHeight:{6},ColumnWidth:{7},
                                            DefaultColumnWidth:{8},UnhideSheet:{9},ChangeChartType:{10},ChartSelectData:{11},ModifyChartLayout:{12},ChartChangeTitle:{13},
                                            ChartChangeHorizontalAxisTitle:{14},ChartChangeVerticalAxisTitle:{15},ModifyChartStyle:{16},FindAndReplace:{17},DataFilterSimple:{18},
                                            TableSelectData:{19}, MoveOrCopySheet:{20}, DataValidation:{21}, ModifyTableStyle:{22}, PageSetup:{23}}}",
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertImage)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.InsertLink)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChangeLink)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RenameSheet)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.OpenFile)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.SaveFile)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.RowHeight)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ColumnWidth)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.DefaultColumnWidth)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.UnhideSheet)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChangeChartType)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChartSelectData)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ModifyChartLayout)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChartChangeTitle)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChartChangeHorizontalAxisTitle)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ChartChangeVerticalAxisTitle)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ModifyChartStyle)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FindAndReplace)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.DataFilterSimple)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.TableSelectData)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.MoveOrCopySheet)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.DataValidation)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ModifyTableStyle)),
				HtmlConvertor.ToScript(ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.PageSetup)));
			RegisterScriptBlock("SpreadsheetDialogTitles", RenderUtils.GetScriptHtml(script));
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(WorkSessionGuid != Guid.Empty) {
				stb.AppendFormat("{0}.sessionGuid = '{1}';\n", localVarName, WorkSessionGuid.ToString());
				stb.AppendFormat("{0}.tileSize={1};\n", localVarName, HtmlConvertor.ToJSON(
					new JSONCellPosition(SpreadsheetRenderHelper.TileColCount, SpreadsheetRenderHelper.TileRowCount)));
				stb.AppendFormat("{0}.visibleRangePadding={1};\n", localVarName, HtmlConvertor.ToJSON(
					new JSONCellPosition(SpreadsheetRenderHelper.WindowHorizontalPadding, SpreadsheetRenderHelper.WindowVerticalPadding)));
				if(RibbonMode == SpreadsheetRibbonMode.ExternalRibbon) {
					ASPxRibbon externalRibbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
					stb.AppendFormat("{0}.ribbonClientID='{1}';\n", localVarName, IsMvcRender() ? AssociatedRibbonID : externalRibbon != null ? externalRibbon.ClientID : string.Empty);
				}
				if(ShowConfirmOnLosingChanges)
					stb.AppendFormat("{0}.confirmUpdate={1};\n", localVarName, HtmlConvertor.ToScript(GetConfirmUpdate()));
				stb.AppendFormat("{0}.autoFilterImagesClassNames={1};\n", localVarName, HtmlConvertor.ToJSON(GetAutoFilterImagesClassNames()));
				AppearanceStyle functionArgumentsHintStyle = StylesFormulaAutoCompete.GetFunctionArgumentInfoStyle();
				stb.AppendFormat("{0}.functionArgumentsHintStyle=[{1}, {2}];\n", localVarName,
					HtmlConvertor.ToScript(functionArgumentsHintStyle.CssClass),
					HtmlConvertor.ToScript(functionArgumentsHintStyle.GetStyleAttributes(Page).Value));
				if(!ShowFormulaBar)
					stb.AppendFormat("{0}.showFormulaBar = false;\n", localVarName);
				stb.AppendFormat("{0}.menuIconSet={1};\n", localVarName, HtmlConvertor.ToScript(GetMenuIconSetName()));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetCurrentDocumentPath() {
			SpreadsheetWorkSession session = GetCurrentWorkSessions();
			return session.Document.Path;
		}
		public string GetWorkDirectory() {
			return UrlUtils.NormalizeRelativePath(WorkDirectory);
		}
		public string GetImageDirectory() {
			return UrlUtils.NormalizeRelativePath(DefaultImageDirectory);
		}
		protected string GetConfirmUpdate() {
			if(!string.IsNullOrEmpty(ConfirmOnLosingChanges))
				return ConfirmOnLosingChanges;
			return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.ConfirmOnLosingChanges);
		}
		protected string GetMenuIconSetName() {
			return SpreadsheetIconImages.Categories[Images.MenuIconSet];
		}
		protected internal string GetPopupMenuControlOnItemClickScript() {
			return string.Format("function(s, e) {{ ASPx.SSMenuItemClick('{0}', e.item.name, e.item.items); }}", ClientID);
		}
		protected internal string GetPopupMenuControlOnCloseUpScript() {
			return string.Format("function(s, e) {{ ASPx.SSMenuCloseUp('{0}'); }}", ClientID);
		}
		protected internal string GetFormulaBarMenuOnItemClickScript() {
			return string.Format("function(s, e) {{ ASPx.SSFormulaBarMenuItemClick('{0}', e.item.name); }}", ClientID);
		}
		protected internal Hashtable GetAutoFilterImagesClassNames() {
			Hashtable imagesClassNames = new Hashtable();
			imagesClassNames.Add(AutoFilterImageType.None, string.Empty);
			imagesClassNames.Add(AutoFilterImageType.DropDown, Images.GetImageProperties(Page, "DropDownButton").SpriteProperties.CssClass);
			imagesClassNames.Add(AutoFilterImageType.Filtered, GetAutoFilterImageClassName(AutoFilterImageType.Filtered));
			imagesClassNames.Add(AutoFilterImageType.Ascending, GetAutoFilterImageClassName(AutoFilterImageType.Ascending));
			imagesClassNames.Add(AutoFilterImageType.Descending, GetAutoFilterImageClassName(AutoFilterImageType.Descending));
			imagesClassNames.Add(AutoFilterImageType.FilteredAndAscending, GetAutoFilterImageClassName(AutoFilterImageType.FilteredAndAscending));
			imagesClassNames.Add(AutoFilterImageType.FilteredAndDescending, GetAutoFilterImageClassName(AutoFilterImageType.FilteredAndDescending));
			return imagesClassNames;
		}
		string GetAutoFilterImageClassName(AutoFilterImageType imageType) {
			ImageProperties autoFilterImage = Images.GetImageProperties(Page, "AutoFilter_" + imageType);
			return autoFilterImage.SpriteProperties.CssClass;
		}
		protected internal SpreadsheetFormulaBarButtonImageProperties GetFormulaBarMenuEnterItemImageProperties() {
			SpreadsheetFormulaBarButtonImageProperties image = new SpreadsheetFormulaBarButtonImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, SpreadsheetIconImages.FormulaBarEnterButton));
			image.CopyFrom(Images.FormulaBarEnterButtonImage);
			return image;
		}
		protected internal SpreadsheetFormulaBarButtonImageProperties GetFormulaBarMenuCancelItemImageProperties() {
			SpreadsheetFormulaBarButtonImageProperties image = new SpreadsheetFormulaBarButtonImageProperties();
			image.CopyFrom(Images.GetImageProperties(Page, SpreadsheetIconImages.FormulaBarCancelButton));
			image.CopyFrom(Images.FormulaBarCancelButtonImage);
			return image;
		}
		protected virtual SpreadsheetFormsSettings CreateSettingsForms() {
			return new SpreadsheetFormsSettings(this);
		}
		protected virtual SpreadsheetDialogFormSettings CreateSettingsDialogForm() {
			return new SpreadsheetDialogFormSettings(this);
		}
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new ASPxSpreadsheetLoadingPanelSettings(this);
		}
		protected virtual SpreadsheetDocumentSelectorSettings CreateSettingsDocumentSelector() {
			return new SpreadsheetDocumentSelectorSettings(this);
		}
		protected virtual SpreadsheetDialogSettings CreateSettingsDialogs() {
			return new SpreadsheetDialogSettings(this);
		}
		protected void EnsureWorkSession() {
			if(WorkSessionGuid == Guid.Empty)
				CreateNewWorkSession();
		}
		protected void CreateNewWorkSession() {
			OpenWorkSession(Guid.Empty, DocumentContentContainer.Empty, ClientID);
		}
		protected void PrepareInnerControl() {
			AssignReadOnlyToInnerControl();
		}
		protected void AssignReadOnlyToInnerControl() {
			SpreadsheetWorkSession session = GetCurrentWorkSessions();
			if(session != null) {
				var innerControl = session.WebSpreadsheetControl.InnerControl;
				if(innerControl != null)
					innerControl.IsReadOnly = ReadOnly;
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(SpreadsheetWebControl);
			EnsureWorkSession();
			CreateDialogs();
		}
		protected virtual void CreateDialogs() {
			if(!DesignMode) {
				if(!string.IsNullOrEmpty(CurrentDialogName) && IsInternalServiceCallback()) {
					CurrentDialogForm = CreateDialogFromControl(CurrentDialogName, ContainerControl);
					if(Page != null && Page.IsCallback) {
						CurrentDialogForm.DataBind();
						RenderUtils.LoadPostDataRecursive(CurrentDialogForm, Request.Params, true);
					}
				}
			}
		}
		protected internal Control CreateDialogFromControl(string dialogName, WebControl parent) {
			string dialogFormName = DialogFormNamesDictionary[dialogName];
			string userSpecifiedFormPath = SettingsForms.GetFormPath(dialogFormName);
			Control userControl;
			if(string.IsNullOrEmpty(userSpecifiedFormPath)) {
				userControl = CreateDefaultForm(dialogFormName);
				PrepareUserControl(userControl, parent, dialogName, true);
			} else {
				userControl = CreateUserControl(userSpecifiedFormPath);
				PrepareUserControl(userControl, parent, dialogName, false);
			}
			return userControl;
		}
		protected Control CreateDefaultForm(string formName) {
			switch(formName) {
				case "InsertLinkForm":
					return CreateInsertHyperLinkDialog();
				case "InsertPictureForm":
					return CreateInsertPictureDialog();
				case "RenameSheetForm":
					return CreateRenameSheetDialog();
				case "OpenFileForm":
					return CreateOpenFileDialog();
				case "SaveFileForm":
					return CreateSaveFileDialog();
				case "RowHeightForm":
					return CreateRowHeightDialog();
				case "ColumnWidthForm":
					return CreateColumnWidthDialog();
				case "DefaultColumnWidthForm":
					return CreateDefaultColumnWidthDialog();
				case "UnhideSheetForm":
					return CreateUnhideSheetDialog();
				case "ChangeChartTypeForm":
					return CreateChangeChartTypeDialog();
				case "ChartSelectDataForm":
					return CreateChartSelectDataDialog();
				case "ModifyChartLayoutForm":
					return CreateModifyChartLayoutDialog();
				case "ChartChangeHorizontalAxisTitleForm":
					return CreateChartChangeHorizontalAxisTitleDialog();
				case "ChartChangeTitleForm":
					return CreateChartChangeTitleDialog();
				case "ChartChangeVerticalAxisTitleForm":
					return CreateChartChangeVerticalAxisTitleDialog();
				case "ModifyChartStyleForm":
					return CreateModifyChartStyleDialog();
				case "FindAndReplaceForm":
					return CreateFindAndReplaceDialog();
				case "DataFilterSimpleForm":
					return CreateDataFilterSimpleDialog();
				case "CustomDataFilterForm":
					return CreateCustomDataFilterDialog();
				case "CustomDateTimeFilterForm":
					return CreateCustomDateTimeFilterDialog();
				case "DataFilterTop10DialogForm":
					return CreateDataFilterTop10Dialog();
				case "TableSelectDataDialogForm":
					return CreateTableSelectDataDialog();
				case "DataValidationDialogForm":
					return CreateDataValidationDialogForm();
				case "ValidationConfirmDialogForm":
					return CreateValidationConfirmDialogForm();
				case "MoveOrCopySheetDialogForm":
					return CreateMoveOrCopySheetDialog();
				case "ModifyTableStyleDialogForm":
					return CreateModifyTableStyleDialog();
				case "PageSetupDialogForm":
					return CreatePageSetupDialog();
				default:
					throw new ArgumentException();
			}
		}
		protected internal virtual ASPxUploadControl CreateUploadControl() {
			return new SpreadsheetUploadControl();
		}
		protected internal virtual SpreadsheetFileManager CreateFileManager() {
			return new SpreadsheetFileManager();
		}
		protected virtual Control CreateInsertPictureDialog() {
			return new Internal.Forms.InsertImageDialog();
		}
		protected virtual Control CreateInsertHyperLinkDialog() {
			return new Internal.Forms.InsertHyperlinkDialog();
		}
		protected virtual Control CreateRenameSheetDialog() {
			return new Internal.Forms.SreadsheetRenameSheetDialog();
		}
		protected virtual Control CreateOpenFileDialog() {
			return new Internal.Forms.OpenFileDialog();
		}
		protected virtual Control CreateSaveFileDialog() {
			return new Internal.Forms.SaveAsFileDialog();
		}
		protected virtual Control CreateRowHeightDialog() {
			return new Internal.Forms.SpreadsheetRowHeightDialog();
		}
		protected virtual Control CreateColumnWidthDialog() {
			return new Internal.Forms.SpreadsheetColumnWidthDialog();
		}
		protected virtual Control CreateDefaultColumnWidthDialog() {
			return new Internal.Forms.SpreadsheetDefaultColumnWidthDialog();
		}
		protected virtual Control CreateUnhideSheetDialog() {
			return new Internal.Forms.UnhideSheetDialog();
		}
		protected virtual Control CreateChangeChartTypeDialog() {
			return new Internal.Forms.ChangeChartTypeDialog();
		}
		protected virtual Control CreateChartSelectDataDialog() {
			return new Internal.Forms.ChartSelectDataDialog();
		}
		protected virtual Control CreateModifyChartLayoutDialog() {
			return new Internal.Forms.ModifyChartLayoutDialog();
		}
		protected virtual Control CreateChartChangeHorizontalAxisTitleDialog() {
			return new Internal.Forms.ChartChangeHorizontalAxisTitleDialog();
		}
		protected virtual Control CreateChartChangeTitleDialog() {
			return new Internal.Forms.ChartChangeTitleDialog();
		}
		protected virtual Control CreateChartChangeVerticalAxisTitleDialog() {
			return new Internal.Forms.ChartChangeVerticalAxisTitleDialog();
		}
		protected virtual Control CreateModifyChartStyleDialog() {
			return new Internal.Forms.ModifyChartStyleDialog();
		}
		protected virtual Control CreateFindAndReplaceDialog() {
			return new Internal.Forms.FindAndReplaceDialog();
		}
		protected virtual Control CreateDataFilterSimpleDialog() {
			return new Internal.Forms.DataFilterSimpleDialog();
		}
		protected virtual Control CreateCustomDataFilterDialog() {
			return new Internal.Forms.CustomDataFilterDialog();
		}
		protected virtual Control CreateCustomDateTimeFilterDialog() {
			return new Internal.Forms.CustomDateTimeFilterDialog();
		}
		protected virtual Control CreateDataFilterTop10Dialog() {
			return new Internal.Forms.DataFilterTop10Dialog();
		}
		protected virtual Control CreateTableSelectDataDialog() {
			return new Internal.Forms.TableSelectDataDialog();
		}
		protected virtual Control CreateDataValidationDialogForm() {
			return new Internal.Forms.DataValidationDialog();
		}
		protected virtual Control CreateValidationConfirmDialogForm() {
			return new Internal.Forms.ValidationConfirmDialog();
		}
		protected virtual Control CreateMoveOrCopySheetDialog() {
			return new Internal.Forms.MoveOrCopySheetDialog();
		}
		protected virtual Control CreateModifyTableStyleDialog() {
			return new Internal.Forms.ModifyTableStyleDialog();
		}
		protected virtual Control CreatePageSetupDialog() {
			return new Internal.Forms.PageSetupDialog();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(Page != null && Page.IsCallback && RibbonMode != SpreadsheetRibbonMode.ExternalRibbon)
				RibbonHelper.SyncRibbonControlCollection(RibbonTabs, SpreadsheetWebControl.RibbonControl, postCollection);
			return base.LoadPostData(postCollection);
		}
		protected object GetDialogFormRenderResult(string dialogName) {
			return GetControlRenderResult(GetCurrentDialogControl(dialogName));
		}
		protected string GetControlRenderResult(Control control) {
			string content = "";
			BeginRendering();
			try {
				if(control != null)
					content = RenderUtils.GetRenderResult(control);
			} finally {
				EndRendering();
			}
			return content;
		}
		protected virtual Control GetCurrentDialogControl(string dialogName) {
			return CurrentDialogForm;
		}
		protected virtual void OnCallback(CallbackEventArgsBase e) {
			SpreadsheetCallbackArgumentsReader argumentsReader = new SpreadsheetCallbackArgumentsReader(e.Parameter);
			if(argumentsReader.IsCustomCallback)
				RaiseCustomCallback(argumentsReader.CustomCallbackArg);
			if(argumentsReader.IsDocumentCallback)
				RaiseDocumentCallback(argumentsReader.DocumentCallbackArg);
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected void RaiseCustomCallback(string eventArgument) {
			CallbackEventHandlerBase handler = Events[EventCallback] as CallbackEventHandlerBase;
			if(handler != null) {
				CallbackEventArgsBase e = new CallbackEventArgsBase(eventArgument);
				handler(this, e);
			}
		}
		protected void RaiseDocumentCallback(string eventArgument) {
			CallbackEventHandlerBase handler = Events[EventDocumentCallback] as CallbackEventHandlerBase;
			if(handler != null) {
				CallbackEventArgsBase e = new CallbackEventArgsBase(eventArgument);
				handler(this, e);
			}
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			this.internalCallbackArgument = eventArgument;
			CallbackEventArgsBase args = new CallbackEventArgsBase(eventArgument);
			OnCallback(args);
		}
		protected override object GetCallbackResult() {
			SpreadsheetCallbackArgumentsReader argumentsReader = new SpreadsheetCallbackArgumentsReader(this.internalCallbackArgument);
			if(argumentsReader.IsInternalServiceCallback) {
				SpreadsheetDialogCallbackArgumentsReader dialogCallbackReader = new SpreadsheetDialogCallbackArgumentsReader(this.internalCallbackArgument);
				EnsureChildControls();
				if(dialogCallbackReader.IsLoadDialogFromRenderCallback)
					return GetDialogFormRenderResult(RenderUtils.DialogFormCallbackStatus);
				else
					if(dialogCallbackReader.IsUploadImageCallback)
						return GetInsertImageUploadResult(dialogCallbackReader.ImageUrl);
					else
						if(dialogCallbackReader.IsFileManagerCallback)
							return GetFileManagerCallbackResult(dialogCallbackReader.FileManagerCallbackData, CurrentDialogName);
						else
							if(dialogCallbackReader.IsNewDocumentCallback)
								return DoNewDocumentCallbackCommand();
							else
								if(dialogCallbackReader.IsOpenFileCallback)
									return DoOpenDocumentCallbackCommand(dialogCallbackReader.OpenFilePath);
								else
									if(dialogCallbackReader.IsSaveFileCallback)
										return SaveSpreadsheetDocument(dialogCallbackReader.SaveFilePath);
			} else if (argumentsReader.IsDocumentCallback || argumentsReader.IsCustomCallback)
				return GetCustomeCallbackResult(argumentsReader.CustomCallbackArg);
			return base.GetCallbackResult();
		}
		protected virtual string GetCustomeCallbackResult(string callbackArgs) {
			return ASPxSpreadsheet.SpreadsheetDocumentUpdatePrefix + this.WorkSessionGuid;
		}
		protected string PrepareDocumentPath(string filePath) {
			if(!string.IsNullOrEmpty(filePath))
				filePath = UrlUtils.ResolvePhysicalPath(GetWorkDirectory() + filePath);
			return filePath;
		}
		protected string DoNewDocumentCallbackCommand() {
			var newFileDocumentContentContainer = new DocumentContentContainer(null);
			return DoNewOrOpenCallbackCommand(WebSpreadsheetCommandID.FileNew, SpreadsheetDialogCallbackArgumentsReader.NewDocumentCallbackPrefix, newFileDocumentContentContainer);
		}
		protected string DoOpenDocumentCallbackCommand(string filePath) {
			filePath = PrepareDocumentPath(filePath);
			var openFileDocumentContentContainer = new DocumentContentContainer(filePath);
			return DoNewOrOpenCallbackCommand(WebSpreadsheetCommandID.FileOpen, SpreadsheetDialogCallbackArgumentsReader.OpenFileCallbackPrefix, openFileDocumentContentContainer);
		}
		protected string DoNewOrOpenCallbackCommand(WebSpreadsheetCommandID webCommandID, string callbackResponsePrefix, DocumentContentContainer documentContentContainer) {
			DoSwitchToAnotherDocumentByCallbackCommandCore(webCommandID, documentContentContainer);
			return PerformSpreadsheetCommandAsCallback(webCommandID, callbackResponsePrefix, documentContentContainer, null, true);
		}
		protected string DoSaveAsCallbackCommandCore(WebSpreadsheetCommandID webCommandID, string callbackResponsePrefix, DocumentContentContainer documentContentContainer, string filePathCommandArgName) {
			DoSwitchToAnotherDocumentByCallbackCommandCore(webCommandID, documentContentContainer);
			return PerformSpreadsheetCommandAsCallback(webCommandID, callbackResponsePrefix, documentContentContainer, filePathCommandArgName, false);
		}
		protected void DoSwitchToAnotherDocumentByCallbackCommandCore(WebSpreadsheetCommandID webCommandID, DocumentContentContainer documentContentContainer) {
			bool reasonIsOpening = webCommandID == WebSpreadsheetCommandID.FileNew || 
								   webCommandID == WebSpreadsheetCommandID.FileOpen;
			ChangeDocumentWorkSession(WorkSessionGuid, documentContentContainer, reasonIsOpening);
		}
		private string PerformSpreadsheetCommandAsCallback(WebSpreadsheetCommandID webCommandID, string callbackResponsePrefix, DocumentContentContainer documentContentContainer, string filePathCommandArgName, bool anotherDocumentOpened) {
			SpreadsheetWorkSession currentSession = GetCurrentWorkSessions();
			if(currentSession != null) {
				NameValueCollection controlClientState = GetClientState();
				controlClientState.Add(SpreadsheetCommandHelper.CommandIDParamName, WebSpreadsheetCommands.GetWebCommandName(webCommandID));
				if(!string.IsNullOrEmpty(filePathCommandArgName))
					controlClientState.Add(filePathCommandArgName, documentContentContainer.PathOrID);
				string callbackResult = currentSession.ProcessCallbackCommand(controlClientState, anotherDocumentOpened);
				return callbackResponsePrefix + RenderUtils.CallBackSeparator + callbackResult;
			}
			return string.Empty;
		}
		protected internal string SaveSpreadsheetDocument(string filePath) {
			filePath = PrepareDocumentPath(filePath);
			SpreadsheetWorkSession currentSession = GetCurrentWorkSessions();
			string currentDocumentPath = GetCurrentDocumentPath();
			bool sameFile = PathUtils.AreSameFilePath(currentDocumentPath, filePath);
			bool noNewFileToSave = string.IsNullOrEmpty(filePath);
			var webSpreadsheetCommandID = sameFile || noNewFileToSave ? 
				WebSpreadsheetCommandID.FileSave  :
				WebSpreadsheetCommandID.FileSaveAs;
			if(webSpreadsheetCommandID == WebSpreadsheetCommandID.FileSave) {
				DocumentSavingEventArgs args = RaiseSaving(currentSession.DocumentPathOrID, MultiUserConflict.None);
				if(!args.Handled) {
					currentSession.EnsureDocumentPathBeforeSaving();
					var saveFileDocumentContentContainer = new DocumentContentContainer(currentDocumentPath);
					return PerformSpreadsheetCommandAsCallback(webSpreadsheetCommandID, SpreadsheetDialogCallbackArgumentsReader.SaveFileCallbackPrefix, saveFileDocumentContentContainer, null, false);
				}
			} else if(webSpreadsheetCommandID == WebSpreadsheetCommandID.FileSaveAs) {
				MultiUserConflict multiUserConflict = WorkSessions.DetectMultiUserSavingConflict(WorkSessionGuid, filePath);
				DocumentSavingEventArgs args = RaiseSaving(filePath, multiUserConflict);
				if(!args.Handled) {
					bool cantSaveAs = multiUserConflict == MultiUserConflict.OtherUserDocumentOverride && args.MultiUserConflictResolve == MultiUserConflictResolve.Persist;
					if(cantSaveAs) {
						throw new CantSaveToAlreadyOpenedFileException(); 
					} else {
						var saveAsFileDocumentContentContainer = new DocumentContentContainer(filePath);
						return DoSaveAsCallbackCommandCore(webSpreadsheetCommandID, SpreadsheetDialogCallbackArgumentsReader.SaveFileCallbackPrefix, 
							saveAsFileDocumentContentContainer, FileSaveAsCommand.FilePathParamName);
					}
				}
			}
			return string.Empty;
		}
		private int raiseSavingCallLockCount = 0;
		protected void LockRaiseSavingCall() {
			raiseSavingCallLockCount++;
		}
		protected void UnlockRaiseSavingCall() {
			raiseSavingCallLockCount--;
		}
		protected bool RaiseSavingCallIsLocked() {
			return raiseSavingCallLockCount > 0;
		}
		protected DocumentSavingEventArgs RaiseSaving(string documentID, MultiUserConflict multiUserConflict) {
			LockRaiseSavingCall();
			try {
				var args = new DocumentSavingEventArgs(documentID, multiUserConflict);
				var handler = Events[EventSaving] as DocumentSavingEventHandler;
				if(handler != null)
					handler(this, args);
				return args;
			} finally {
				UnlockRaiseSavingCall();
			}
		}
		protected object GetFileManagerCallbackResult(string callbackArgs, string dialogName) {
			switch(dialogName) {
				case "savefiledialog":
				SpreadsheetFolderManager folderManager = GetDialogFileManager(dialogName) as SpreadsheetFolderManager;
				folderManager.IsSpreadsheetCallback = this.IsCallback;
				return folderManager.GetCallbackResult(callbackArgs);
				default:
				SpreadsheetFileManager fileManager = GetDialogFileManager(dialogName) as SpreadsheetFileManager;
				fileManager.IsSpreadsheetCallback = this.IsCallback;
				return fileManager.GetCallbackResult(callbackArgs);
			}
		}
		protected Control GetDialogFileManager(string dialogName) {
			return FindControlHelper.LookupControlRecursive(
				GetCurrentDialogControl(dialogName),
				"FileManager"
			);
		}
		protected string GetInsertImageUploadResult(string imageUrl) {
			SpreadsheetDownloadHelper helper = new SpreadsheetDownloadHelper(this);
			return helper.GetInsertImageUploadResult(imageUrl);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string AddImage(Stream stream, string fileName) {
			SpreadsheetWorkSession currentSession = GetCurrentWorkSessions();
			if(currentSession != null) {
				NameValueCollection controlClientState = GetClientState();
				if(controlClientState != null && controlClientState.Count > 0) {
					controlClientState.Add(SpreadsheetCommandHelper.CommandIDParamName, WebSpreadsheetCommands.GetWebCommandName(WebSpreadsheetCommandID.InsertPicture));
					controlClientState.Add(InsertPictureWebCommand.PicturePathParamName, fileName);
					SpreadsheetRenderHelper renderHelper = new SpreadsheetRenderHelper(currentSession, controlClientState);
					WebSpreadsheetCommandContext commandContext = new WebSpreadsheetCommandContext(renderHelper, controlClientState);
					InsertPictureWebCommand command = new InsertPictureWebCommand(commandContext, stream);
					return currentSession.ProcessCustomCommand(renderHelper, command);
				}
			}
			return string.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorFolderCreating(FileManagerFolderCreateEventArgs args) {
			FileManagerFolderCreateEventHandler handler = (FileManagerFolderCreateEventHandler)Events[DocumentSelectorFolderCreatingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorItemRenaming(FileManagerItemRenameEventArgs args) {
			FileManagerItemRenameEventHandler handler = (FileManagerItemRenameEventHandler)Events[DocumentSelectorItemRenamingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorItemDeleting(FileManagerItemDeleteEventArgs args) {
			FileManagerItemDeleteEventHandler handler = (FileManagerItemDeleteEventHandler)Events[DocumentSelectorItemDeletingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorItemMoving(FileManagerItemMoveEventArgs args) {
			FileManagerItemMoveEventHandler handler = (FileManagerItemMoveEventHandler)Events[DocumentSelectorItemMovingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorFileUploading(FileManagerFileUploadEventArgs args) {
			FileManagerFileUploadEventHandler handler = (FileManagerFileUploadEventHandler)Events[DocumentSelectorFileUploadingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorItemCopying(FileManagerItemCopyEventArgs args) {
			FileManagerItemCopyEventHandler handler = (FileManagerItemCopyEventHandler)Events[DocumentSelectorItemCopyingEventKey];
			if(handler != null)
				handler(this, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseDocumentSelectorCloudProviderRequest(FileManagerCloudProviderRequestEventArgs args) {
			FileManagerCloudProviderRequestEventHandler handler = (FileManagerCloudProviderRequestEventHandler)Events[DocumentSelectorCloudProviderRequestEventKey];
			if(handler != null)
				handler(this, args);
		}
		protected void FillDialogFormNamesDictionary() {
			for(int i = 0; i < DialogNames.Length; i++)
				DialogFormNamesDictionary.Add(DialogNames[i], DialogFormNames[i]);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxSpreadsheet.Design.SpreadsheetCommonFormDesigner"; } }
	}
	static class PathUtils {
		public static bool AreSameFilePath(string path1, string path2) {
			try {
				if(String.IsNullOrEmpty(path1) || String.IsNullOrEmpty(path2))
					return false;
				return Path.GetFullPath(path1).ToLowerInvariant() == Path.GetFullPath(path2).ToLowerInvariant();
			} catch {
				return false;
			}
		}
	}
}
internal class ToolboxBitmapAccess { }
