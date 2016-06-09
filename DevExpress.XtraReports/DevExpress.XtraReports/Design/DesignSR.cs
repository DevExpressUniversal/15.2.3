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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design
{
	public static class DesignSR {
		public const string ProductName = "XtraReports";
		public const string PropertyGridCurrentString = "(Current)";
		public const string InvalidArgument = " is not a valid value for ";
		public const string Trans_Add = "Add {0} object";
		public const string Trans_CreateComponents = "Create component(s)";
		public const string Trans_CreateTool = "Creating components in tool";
		public const string Trans_ReorderBands = "Reorder Bands";
		public const string Trans_ChangeProp = "Change {0}";
		public const string TransFmt_SendToBack = "Send {0} components to back";
		public const string TransFmt_BringToFront = "Bring {0} components to front";
		public const string TransFmt_Format = "Format {0} components ({1})";
		public const string TransFmt_CenterVert = "Vertical center of {0} component(s)";
		public const string TransFmt_CenterHoriz = "Horizotal center of {0} component(s)";
		public const string TransFmt_SizeToGrid = "Size {0} component(s) to grid";
		public const string TransFmt_AlignToGrid = "Align {0} component(s) to grid";
		public const string TransFmt_OneMove = "Move {0}";
		public const string TransFmt_Move = "Move {0} component(s)";
		public const string TransFmt_OneSize = "Size {0}";
		public const string TransFmt_Size = "Size {0} components";
		public const string TransFmt_ChangeHeight = "Change {0}.Height";
		public const string TransFmt_Font = "Set property 'Font'";
		public const string TransFmt_SetProperty = "Set property '{0}'";
		public const string TransFmt_Justify = "Property 'Alignment' changed";
		public const string TransFmt_Summary = "Summary changed";
		public const string TransFmt_Angle = "Change {0}.Angle";
		public const string TransFmt_ShapeChanged = "Shape changed";
		public const string TransFmt_ChangeFormattingRules = "Change {0}." + XRComponentPropertyNames.FormattingRules;
		public const string Trans_Paste = "Paste components";
		public const string Trans_Delete = "Delete components";
		public const string Trans_ConvertToLabels = "Convert To Labels";
		public const string Trans_Delete_Component = "Delete {0}";
		public static string DataGridNewString { get { return ReportLocalizer.GetString(ReportStringId.ScriptEditor_NewString); } }
		public static string DataGridNoneString { get { return PreviewLocalizer.GetString(PreviewStringId.NoneString); } }
		public static string PropertyGridNotSet { get { return ReportLocalizer.GetString(ReportStringId.UD_PropertyGrid_NotSetText); } }
		public static string Verb_EditBands { get { return ReportLocalizer.GetString(ReportStringId.Verb_EditBands); } }
		public static string Verb_EditGroupFields { get { return ReportLocalizer.GetString(ReportStringId.Verb_EditGroupFields); } }
		public static string Verb_Import { get { return ReportLocalizer.GetString(ReportStringId.Verb_Import); } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string Verb_Export { get { return ReportLocalizer.GetString(ReportStringId.Verb_Export); } }
		public static string Verb_Save { get { return ReportLocalizer.GetString(ReportStringId.Verb_Save); } }
		public static string Verb_About { get { return ReportLocalizer.GetString(ReportStringId.Verb_About); } }
		public static string Verb_RTFClear { get { return ReportLocalizer.GetString(ReportStringId.Verb_RTFClear); } }
		public static string Verb_RTFLoad { get { return ReportLocalizer.GetString(ReportStringId.Verb_RTFLoad); } }
		public static string Verb_FormatString { get { return ReportLocalizer.GetString(ReportStringId.Verb_FormatString); } }
		public static string Verb_SummaryWizard { get { return ReportLocalizer.GetString(ReportStringId.Verb_SummaryWizard); } }
		public static string Verb_ReportWizard { get { return ReportLocalizer.GetString(ReportStringId.Verb_ReportWizard); } }
		public static string Verb_Insert { get { return ReportLocalizer.GetString(ReportStringId.Verb_Insert); } }
		public static string Verb_Delete { get { return ReportLocalizer.GetString(ReportStringId.Verb_Delete); } }
		public static string Verb_Bind { get { return ReportLocalizer.GetString(ReportStringId.Verb_Bind); } }
		public static string Verb_EditText { get { return ReportLocalizer.GetString(ReportStringId.Verb_EditText); } }
		public static string Verb_AddFieldToArea { get { return ReportLocalizer.GetString(ReportStringId.Verb_AddFieldToArea); } }
		public static string Verb_RunDesigner { get { return ReportLocalizer.GetString(ReportStringId.Verb_RunDesigner); } }
		public static string Verb_LoadReportTemplate { get { return ReportLocalizer.GetString(ReportStringId.Verb_LoadReportTemplate); } }
		public static string Verb_EditBindings { get { return ReportLocalizer.GetString(ReportStringId.Verb_EditBindings); } }
		public static string PivotGridForm_GroupMain_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_GroupMain_Caption); } }
		public static string PivotGridForm_GroupMain_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_GroupMain_Description); } }
		public static string PivotGridForm_ItemFields_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemFields_Caption); } }
		public static string PivotGridForm_ItemFields_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemFields_Description); } }
		public static string PivotGridForm_ItemLayout_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemLayout_Caption); } }
		public static string PivotGridForm_ItemLayout_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemLayout_Description); } }
		public static string PivotGridForm_GroupPrinting_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_GroupPrinting_Caption); } }
		public static string PivotGridForm_GroupPrinting_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_GroupPrinting_Description); } }
		public static string PivotGridForm_ItemAppearances_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemAppearances_Caption); } }
		public static string PivotGridForm_ItemAppearances_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemAppearances_Description); } }
		public static string PivotGridForm_ItemSettings_Caption { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemSettings_Caption); } }
		public static string PivotGridForm_ItemSettings_Description { get { return ReportLocalizer.GetString(ReportStringId.PivotGridForm_ItemSettings_Description); } }
		public static string PivotGridFrame_Fields_ColumnsText { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Fields_ColumnsText); } }
		public static string PivotGridFrame_Fields_DescriptionText1 { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Fields_DescriptionText1); } }
		public static string PivotGridFrame_Fields_DescriptionText2 { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Fields_DescriptionText2); } }
		public static string PivotGridFrame_Layouts_DescriptionText { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Layouts_DescriptionText); } }
		public static string PivotGridFrame_Layouts_SelectorCaption1 { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Layouts_SelectorCaption1); } }
		public static string PivotGridFrame_Layouts_SelectorCaption2 { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Layouts_SelectorCaption2); } }
		public static string PivotGridFrame_Appearances_DescriptionText { get { return ReportLocalizer.GetString(ReportStringId.PivotGridFrame_Appearances_DescriptionText); } }
		public const string Margins = "Set Property 'Margins'";
		public const string PageSettings = "Set PageSettings";
		public const string DefaultValueString = "(Default)";
		public const string AutoValueString = "(Auto)";
		public const string NoneValueString = DevExpress.Utils.Design.DesignSR.NoneValueString;
	}
}
