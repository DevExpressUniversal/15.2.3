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
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Web.UI;
using DevExpress.Web.ASPxSpreadsheet;
using DevExpress.Web.ASPxSpreadsheet.Internal;
[assembly: AllowPartiallyTrustedCallers]
[assembly: AssemblyTitle("DevExpress.Web.ASPxSpreadsheet")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Developer Express Inc.")]
[assembly: AssemblyProduct("DevExpress.Web.ASPxSpreadsheet")]
[assembly: AssemblyCopyright("Copyright (c) 2000-2015 Developer Express Inc.")]
[assembly: AssemblyTrademark("ASPxSpreadsheet")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo(AssemblyInfo.SRAssemblyWebDesign + ", PublicKey=0024000004800000940000000602000000240000525341310004000001000100dfcd8cadc2dd24a7cd4ce95c4a9c1b8e7cb1dc2d665120556b4b0ec35495fddb2bd6eed0ca1e56480276295a225ba2a9746f3d3e1a04547ccf5b26acc3f96eb2a13ac467512497aa79208e32f242fd0618014d53c95a36e5de0e891873841fa8f559566e38e968426488b4aa4d0f0b59e59f38dcf3fbccf25d990ab19c27ddc2")]
[assembly: ComVisible(false)]
[assembly: Guid("1d57a102-b8c2-4f2a-9f1e-b45148f05adc")]
[assembly: AssemblyVersion(AssemblyInfo.Version)]
[assembly: SatelliteContractVersion(AssemblyInfo.SatelliteContractVersion)]
[assembly: AssemblyFileVersion(AssemblyInfo.FileVersion)]
#pragma warning disable 1699
[assembly: AssemblyKeyFile(@"..\..\..\Devexpress.Key\StrongKey.snk")]
#pragma warning restore 1699
[assembly: AssemblyKeyName("")]
[assembly: TagPrefix("DevExpress.Web.ASPxSpreadsheet", "dx")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetTileHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetScrollHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetGridResizingHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetTileMatrixScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetDialogsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetCommandsScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetDynamicSelectionScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetSelectionScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetEditingScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetKeyboardManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetRibbonManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFileManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFolderManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetTabControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFormulaParserScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetSelectionHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetStateControllerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetUploadControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFileManagerUploadControlScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetInputControllerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetPopupMenuHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetPaneManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetPaneViewScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFormulaIntelliSenseManagerScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctionsListBoxScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetValidationHelperScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetRenderProviderScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ar_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_bg_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ca_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_cs_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_da_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_de_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_deCH_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_el_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_en_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_es_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_et_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_fa_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_fi_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_fr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_he_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_hi_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_hr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_hu_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_hy_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_id_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_is_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_it_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ja_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_kk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ko_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_lt_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_lv_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_mk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ms_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_nl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_no_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_pl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_pt_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ptBR_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ro_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ru_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_sk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_sl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_srCyrlCS_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_srLatnBA_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_srLatnCS_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_sv_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_ta_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_th_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_tr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_uk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_vi_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_zhCN_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_zhHans_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_zhHant_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetFunctions_zhTW_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetConfigScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ar_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_bg_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ca_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_cs_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_da_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_de_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_deCH_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_el_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_en_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_es_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_fa_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_fi_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_fr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_he_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_hr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_hu_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_hy_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_is_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_it_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ja_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ko_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_lt_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_lv_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_mk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_nl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_no_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_pl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_pt_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ptBR_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ro_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ru_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_sk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_sl_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_srCyrlCS_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_srLatinBa_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_srLatinCS_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_sv_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_ta_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_tr_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_uk_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_vi_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_zhCN_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_zhHans_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_zhHant_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetLocalization_zhTW_ScriptResourceName, "text/javascript")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetSystemCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetIconSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetGrayScaleIconSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetGrayScaleWithWhiteHottrackIconSpriteCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetDefaultCssResourceName, "text/css", PerformSubstitution = true)]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetIconImages.IconSpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetIconImages.GrayScaleIconSpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetIconImages.GrayScaleWithWhiteHottrackIconSpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetImages.SpriteImageName + ".png", "image/png")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetImages.LoadingPanelImageName + ".gif", "image/gif")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetImages.TouchResizeImageName + ".png", "image/png")]
[assembly: WebResource(ASPxSpreadsheet.SpreadsheetImageResourcePath + SpreadsheetImages.CellsGridImageName + ".png", "image/png")]
[assembly: SpreadsheetWorkSessionRegistration()]
