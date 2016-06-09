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
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region ExportFormula
		FormulaBase PrepareFormula(ICell cell) {
			return ReplaceCustomFunctions(cell);
		}
		protected internal virtual void ExportFormula(ICell cell, FormulaBase formula) {
			FormulaType formulaType = formula.Type;
			bool calculateAlways = formula.CalculateAlways || formula.IsVolatile();
			if (formulaType == FormulaType.ArrayPart && !calculateAlways)
				return;
			WriteShStartElement("f");
			try {
				if (calculateAlways)
					WriteBoolValue("ca", true);
				ExportFormulaCore(cell, formula, formulaType);
			}
			finally {
				WriteShEndElement();
			}
			return;
		}
		void ExportFormulaCore(ICell cell, FormulaBase formula, FormulaType formulaType) {
			switch (formulaType) {
				case FormulaType.Array:
					ExportArrayFormula(cell, formula as ArrayFormula);
					break;
				case FormulaType.Shared:
					ExportSharedFormulaRef(cell, formula as SharedFormulaRef);
					break;
				case FormulaType.Normal:
					ExportFormulaBody(cell, formula);
					break;
			}
		}
		FormulaBase ReplaceCustomFunctions(ICell cell) {
			if (Workbook.DocumentExportOptions.CustomFunctionExportMode == CustomFunctionExportMode.CalculatedValue && cell.FormulaType != FormulaType.ArrayPart)
				return cell.GetFormulaWithoutCustomFunctions(true);
			return cell.Formula;
		}
		#region Array formula
		void ExportArrayFormula(ICell cell, ArrayFormula arrayFormula) {
			ExportFormulaArray(arrayFormula);
			ExportFormulaBody(cell, arrayFormula);
		}
		protected internal virtual void ExportFormulaArray(ArrayFormula arrayFormula) {
			WriteShStringValue("t", "array");
			WriteShStringValue("ref", arrayFormula.Range.ToString());
		}
		#endregion
		#region Shared formula
		void ExportSharedFormulaRef(ICell cell, SharedFormulaRef sharedFormulaRef) {
			int index;
			if (ExportedSharedFormulas.TryGetValue(sharedFormulaRef.HostSharedFormula, out index))
				ExportNonFirstCell(sharedFormulaRef, index);
			else
				ExportFirstCell(cell, sharedFormulaRef);
		}
		void ExportNonFirstCell(SharedFormulaRef sharedFormulaRef, int index) {
			ExportFormulaShared(index);
		}
		void ExportFirstCell(ICell cell, SharedFormulaRef sharedFormulaRef) {
			int index = ExportedSharedFormulas.Count;
			ExportedSharedFormulas.Add(sharedFormulaRef.HostSharedFormula, index);
			SharedFormula formula = sharedFormulaRef.HostSharedFormula;
			ExportFormulaShared(formula, index);
			ExportFormulaBody(cell, formula);
		}
		protected internal void ExportFormulaShared(int sharedFormulaIndex) {
			WriteStringValue("t", "shared");
			WriteStringValue("si", sharedFormulaIndex.ToString());
		}
		protected internal void ExportFormulaShared(SharedFormula sharedFormula, int sharedFormulaIndex) {
			WriteStringValue("t", "shared");
			WriteStringValue("ref", sharedFormula.Range.ToString());
			WriteStringValue("si", sharedFormulaIndex.ToString());
		}
		#endregion
		protected internal virtual void ExportFormulaBody(ICell cell, IFormula formula) {
			string formulaText = formula.GetBody(cell);
			Debug.Assert(formulaText[0] == '=');
			WriteShString(EncodeXmlCharsXML1_0(formulaText.Remove(0, 1)));
		}
		#endregion
	}
}
