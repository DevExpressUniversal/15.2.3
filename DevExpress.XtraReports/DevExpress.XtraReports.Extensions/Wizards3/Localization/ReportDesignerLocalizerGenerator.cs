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
using System;
namespace DevExpress.XtraReports.Wizards3.Localization {
	#region enum ReportBoxDesignerStringId
	public enum ReportBoxDesignerStringId {
		Wizard_WindowTitleDatasource,
		Wizard_WindowTitleReport,
		Wizard_AddGroupingLvl_Description,
		Wizard_ChooseColumns_Description,
		Wizard_ChooseReportType_Description,
		Wizard_ChooseSummaryOptions_Description,
		Wizard_ChooseTableOrView_Description,
		Wizard_LabelInformation_Description,
		Wizard_LabelOptions_Description,
		Wizard_ReportComplete_Description,
		Wizard_ReportLayout_Description,
		Wizard_ReportStyle_UnexpectedBehavior,
		Wizard_ReportStyle_Description,
		Wizard_Layout_AlignLeft1,
		Wizard_Layout_AlignLeft2,
		Wizard_Layout_Columnar,
		Wizard_Layout_Justified,
		Wizard_Layout_Outline1,
		Wizard_Layout_Outline2,
		Wizard_Layout_Stepped,
		Wizard_Layout_Tabular,
		Wizard_ChooseDSType_Description,
		Wizard_LabelOptions_LabelsOnPage,
	}
	#endregion
	#region ReportDesignerLocalizer.AddStrings 
	public partial class ReportDesignerLocalizer {
		void AddStrings() {
			AddString(ReportBoxDesignerStringId.Wizard_WindowTitleDatasource, "Data Source Wizard");
			AddString(ReportBoxDesignerStringId.Wizard_WindowTitleReport, "Report Wizard");
			AddString(ReportBoxDesignerStringId.Wizard_AddGroupingLvl_Description, "Create multiple groups, each with a single field value, or define several fields in the same group.");
			AddString(ReportBoxDesignerStringId.Wizard_ChooseColumns_Description, "Select the columns you want to display within your report.");
			AddString(ReportBoxDesignerStringId.Wizard_ChooseReportType_Description, "Select the report type you wish to create.");
			AddString(ReportBoxDesignerStringId.Wizard_ChooseSummaryOptions_Description, "What summary functions would you like to calculate?");
			AddString(ReportBoxDesignerStringId.Wizard_ChooseTableOrView_Description, "The table or view you choose determines which columns will be available in your report.");
			AddString(ReportBoxDesignerStringId.Wizard_LabelInformation_Description, "Select one of the predefined labels by specifying the Product and its ID");
			AddString(ReportBoxDesignerStringId.Wizard_LabelOptions_Description, "You can adjust the label's parameters here if required.");
			AddString(ReportBoxDesignerStringId.Wizard_ReportComplete_Description, "We have all the information needed to process the report.");
			AddString(ReportBoxDesignerStringId.Wizard_ReportLayout_Description, "The report layout specifies the manner in which selected data fields are arranged on individual pages.");
			AddString(ReportBoxDesignerStringId.Wizard_ReportStyle_UnexpectedBehavior, "Unexpected behavior: none of the style is selected.");
			AddString(ReportBoxDesignerStringId.Wizard_ReportStyle_Description, "The report style specifies the appearance of your report.");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_AlignLeft1, "Align Left 1");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_AlignLeft2, "Align Left 2");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Columnar, "Columnar");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Justified, "Justified");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Outline1, "Outline 1");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Outline2, "Outline 2");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Stepped, "Stepped");
			AddString(ReportBoxDesignerStringId.Wizard_Layout_Tabular, "Tabular");
			AddString(ReportBoxDesignerStringId.Wizard_ChooseDSType_Description, "Select the data source type.");
			AddString(ReportBoxDesignerStringId.Wizard_LabelOptions_LabelsOnPage, "labels on the page");
		}
	}
	 #endregion
}
