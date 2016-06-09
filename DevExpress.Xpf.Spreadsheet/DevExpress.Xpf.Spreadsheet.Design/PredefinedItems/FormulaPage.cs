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

extern alias Platform;
using System;
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Spreadsheet.UI;
namespace DevExpress.Xpf.Spreadsheet.Design {
	public static partial class BarInfos {
		#region FunctionLibrary
		public static BarInfo FunctionLibrary { get { return functionLibrary; } }
		static readonly BarInfo functionLibrary = CreateFunctionLibrary();
		static BarInfo CreateFunctionLibrary() {
			BarSubItemInfo formulasAutoSumSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FunctionsInsertSum", "FunctionsInsertAverage", "FunctionsInsertCountNumbers", "FunctionsInsertMax", "FunctionsInsertMin" },
				   new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button, BarItemInfos.Button }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Formulas",
				"Function Library",
				new BarInfoItems(
					new string[] { "FunctionsAutoSumCommandGroup", "FunctionsFinancialCommandGroup", "FunctionsLogicalCommandGroup", "FunctionsTextCommandGroup", "FunctionsDateAndTimeCommandGroup", "FunctionsLookupAndReferenceCommandGroup", "FunctionsMathAndTrigonometryCommandGroup", "FunctionsStatisticalCommandGroup", "FunctionsEngineeringCommandGroup", "FunctionsCubeCommandGroup", "FunctionsInformationCommandGroup", "FunctionsCompatibilityCommandGroup" },
					new BarItemInfo[] { formulasAutoSumSubItem, new InsertFunctionsBarSubItemInfo(typeof(InsertFinancialFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertLogicalFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertTextFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertDateAndTimeFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertLookupAndReferenceFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertMathAndTrigonometryFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertStatisticalFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertEngineeringFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertCubeFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertInformationFunctionsBarSubItem)), new InsertFunctionsBarSubItemInfo(typeof(InsertCompatibilityFunctionsBarSubItem)) }
				),
				String.Empty,
				String.Empty,
				"Caption_PageFormulas",
				"Caption_GroupFunctionLibrary"
			);
		}
		#endregion
		public static BarInfo FormulaDefinedNames { get { return formulaDefinedNames; } }
		static readonly BarInfo formulaDefinedNames = CreateFormulaDefinedNames();
		static BarInfo CreateFormulaDefinedNames() {
			return new BarInfo(
				String.Empty,
				"Formulas",
				"Defined Names",
				new BarInfoItems(
					new string[] { "FormulasShowNameManager", "FormulasDefineNameCommand", "FormulasInsertDefinedNameCommandGroup", "FormulasCreateDefinedNamesFromSelection" },
					new BarItemInfo[] { BarItemInfos.Button, BarItemInfos.Button, new InsertDefinedNameBarSubItemInfo(typeof(InsertDefinedNamesBarSubItem)), BarItemInfos.Button }
				),
				String.Empty,
				String.Empty,
				"Caption_PageFormulas",
				"Caption_GroupFormulaDefinedNames"
			);
		}
		public static BarInfo FormulaAuditing { get { return formulaAuditing; } }
		static readonly BarInfo formulaAuditing = CreateFormulaAuditing();
		static BarInfo CreateFormulaAuditing() {
			return new BarInfo(
				String.Empty,
				"Formulas",
				"Formula Auditing",
				new BarInfoItems(
					new string[] { "ViewShowFormulas" },
					new BarItemInfo[] { BarItemInfos.Check }
				),
				String.Empty,
				String.Empty,
				"Caption_PageFormulas",
				"Caption_GroupFormulaAuditing"
			);
		}
		public static BarInfo FormulaCalculation { get { return formulaCalculation; } }
		static readonly BarInfo formulaCalculation = CreateFormulaCalculation();
		static BarInfo CreateFormulaCalculation() {
			BarSubItemInfo formulaCalculationModeSubItem = new BarSubItemInfo(
			   new BarInfoItems(
				   new string[] { "FormulasCalculationModeAutomatic", "FormulasCalculationModeManual" },
				   new BarItemInfo[] { BarItemInfos.Check, BarItemInfos.Check }
			   )
			);
			return new BarInfo(
				String.Empty,
				"Formulas",
				"Calculation",
				new BarInfoItems(
					new string[] { "FormulasCalculationOptionsCommandGroup", "FormulasCalculateNow", "FormulasCalculateSheet" },
					new BarItemInfo[] { formulaCalculationModeSubItem, BarItemInfos.ButtonSmallWithTextRibbonStyle, BarItemInfos.ButtonSmallWithTextRibbonStyle }
				),
				String.Empty,
				String.Empty,
				"Caption_PageFormulas",
				"Caption_GroupFormulaCalculation"
			);
		}
	}
}
