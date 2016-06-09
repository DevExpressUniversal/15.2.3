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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual bool ShouldExportSheetProperties() {
			return ShouldExportSheetPropertiesTabColor(ActiveSheet) ||
				ShouldExportSheetPropertiesPageSetupContent(ActiveSheet.PrintSetup) ||
				ShouldExportOutlineSheetPropertiesContent(ActiveSheet.Properties.GroupAndOutlineProperties) ||
				ActiveSheet.Properties.TransitionOptions.TransitionFormulaEntry ||
				ActiveSheet.Properties.TransitionOptions.TransitionFormulaEvaluation ||
				!string.IsNullOrEmpty(ActiveSheet.Properties.CodeName);
		}
		protected internal virtual bool ShouldExportSheetPropertiesTabColor(Worksheet sheet) {
			return sheet.Properties.TabColorIndex != 0;
		}
		protected internal virtual bool ShouldExportSheetPropertiesPageSetupContent(PrintSetup printSetup) {
			PrintSetupInfo defaultInfo = Workbook.Cache.PrintSetupInfoCache.DefaultItem;
			return !(defaultInfo.AutoPageBreaks == printSetup.AutoPageBreaks &&
				defaultInfo.FitToPage == printSetup.FitToPage);
		}
		protected internal virtual bool ShouldExportOutlineSheetPropertiesContent(GroupAndOutlineProperties outlineProperties) {
			GroupAndOutlinePropertiesInfo defaultInfo = Workbook.Cache.GroupAndOutlinePropertiesInfoCache.DefaultItem;
			return !(defaultInfo.ApplyStyles == outlineProperties.ApplyStyles &&
				defaultInfo.ShowColumnSumsRight == outlineProperties.ShowColumnSumsRight &&
				defaultInfo.ShowRowSumsBelow == outlineProperties.ShowRowSumsBelow);
		}
		protected internal virtual void GenerateSheetProperties() {
			if (!ShouldExportSheetProperties())
				return;
			WriteShStartElement("sheetPr");
			try {
				if(!string.IsNullOrEmpty(ActiveSheet.Properties.CodeName))
					WriteStringValue("codeName", ActiveSheet.Properties.CodeName);
				TransitionOptions options = ActiveSheet.Properties.TransitionOptions;
				if (options.TransitionFormulaEvaluation)
					WriteBoolValue("transitionEvaluation", options.TransitionFormulaEvaluation);
				if (options.TransitionFormulaEntry)
					WriteBoolValue("transitionEntry", options.TransitionFormulaEntry);
				GenerateSheetPropertiesSheetTabColor(ActiveSheet.Properties.TabColorIndex);
				GenerateOutlineSheetPropertiesContent(ActiveSheet.Properties.GroupAndOutlineProperties);
				GenerateSheetPropertiesPageSetupContent(ActiveSheet.PrintSetup);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetPropertiesSheetTabColor(int tabColorIndex) {
			WriteColor(Workbook.Cache.ColorModelInfoCache[tabColorIndex], "tabColor");
		}
		protected internal virtual void GenerateSheetPropertiesPageSetupContent(PrintSetup printSetup) {
			if (!ShouldExportSheetPropertiesPageSetupContent(printSetup))
				return;
			PrintSetupInfo defaultInfo = Workbook.Cache.PrintSetupInfoCache.DefaultItem;
			WriteShStartElement("pageSetUpPr");
			try {
				if (defaultInfo.AutoPageBreaks != printSetup.AutoPageBreaks)
					WriteBoolValue("autoPageBreaks", printSetup.AutoPageBreaks);
				if (defaultInfo.FitToPage != printSetup.FitToPage)
					WriteBoolValue("fitToPage", printSetup.FitToPage);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateOutlineSheetPropertiesContent(GroupAndOutlineProperties outlineProperties) {
			if (!ShouldExportOutlineSheetPropertiesContent(outlineProperties))
				return;
			GroupAndOutlinePropertiesInfo defaultInfo = Workbook.Cache.GroupAndOutlinePropertiesInfoCache.DefaultItem;
			WriteShStartElement("outlinePr");
			try {
				if (defaultInfo.ShowColumnSumsRight !=  outlineProperties.ShowColumnSumsRight)
					WriteBoolValue("summaryRight", outlineProperties.ShowColumnSumsRight);
				if (defaultInfo.ShowRowSumsBelow != outlineProperties.ShowRowSumsBelow)
					WriteBoolValue("summaryBelow", outlineProperties.ShowRowSumsBelow);
				if (defaultInfo.ApplyStyles != outlineProperties.ApplyStyles)
					WriteBoolValue("applyStyles", outlineProperties.ApplyStyles);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}
