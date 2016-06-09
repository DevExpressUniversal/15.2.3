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
using ICellBase = DevExpress.XtraSpreadsheet.Model.ICellBase;
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	#region PrepareTargetParsedExpressionVisitorDiffWorkbooks
	public class PrepareTargetParsedExpressionVisitorDiffWorkbooks : PrepareTargetParsedExpressionVisitor {
		public PrepareTargetParsedExpressionVisitorDiffWorkbooks(ICellTable target, ICopyEverythingArguments icopyeverythingArguments)
			: base(icopyeverythingArguments.TargetDataContext, target, icopyeverythingArguments) {
		}
		WorkbookDataContext TargetDataContext { get { return Args.TargetDataContext; } }
		IModelWorkbook WorkbookSource { get { return Args.SourceDataContext.CurrentWorkbook; } }
		IModelWorkbook WorkbookTarget { get { return Args.TargetDataContext.CurrentWorkbook; } }
		SheetDefToOuterAreasReplaceMode Option { get { return Args.SheetDefinitionToOuterAreasReplaceMode; } }
		External.ExternalLinkCollection TargetExternalLinks { get { return Args.TargetExternalLinks; } }
		public int GetTargetSheetDefinition(SheetDefinition sourceSheetDefinition, bool cloneSheetDefinition) {
			int targetSheetDefinitionIndex = Int32.MinValue;
			if (!Args.SourceSheetDefinitionToTargetCache.TryGetValue(sourceSheetDefinition, out targetSheetDefinitionIndex)) {
				if (cloneSheetDefinition)
					targetSheetDefinitionIndex = CopySheetDefinitionToTarget(sourceSheetDefinition);
				else
					targetSheetDefinitionIndex = ConvertSheetDefinitionToExternalTargetSheetDefinition(sourceSheetDefinition);
				Args.SourceSheetDefinitionToTargetCache.Add(sourceSheetDefinition, targetSheetDefinitionIndex);
			}
			return targetSheetDefinitionIndex;
		}
		public override void Visit(ParsedThingErr3d thing) {
			if (!IsValidFormula)
				return;
			base.Visit(thing);
			WasParsedThingError = true; 
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			bool cloneSheetDefinition = !sourceSheetDefinition.IsExternalReference;
			int newSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, cloneSheetDefinition);
			thing.SheetDefinitionIndex = newSheetDefinitionIndex;
			ReplaceCurrentExpression(thing);
		}
		public override void Visit(ParsedThingRef3d thing) {
			if (!IsValidFormula)
				return;
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			bool ifRefersToInsideSource = !sourceSheetDefinition.IsExternalReference && IsThingRefRefersToInsideSourceRange(thing.Position, sourceSheetDefinition);
			bool outerSourceRangeAreas = !ifRefersToInsideSource;
			if (outerSourceRangeAreas ) { 
				if (Option == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
					IsValidFormula = false;
					return;
				}
				if (!sourceSheetDefinition.IsExternalReference && Option == SheetDefToOuterAreasReplaceMode.ThingToValue) { 
					IWorksheet sourceStartSheetDefinition = sourceSheetDefinition.IsExternalReference
						? SourceWorksheet 
						: sourceSheetDefinition.GetSheetStart(WorkbookSource.DataContext);
					IParsedThing newThing = ConvertThingRef3dToValueOrErrorNotAvailable(thing, sourceStartSheetDefinition);
					ReplaceCurrentExpression(newThing);
					return;
				}
			}
			int targetSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, ifRefersToInsideSource);
			CellPosition shifted = thing.Position.GetShifted(Offset, TargetWorksheet);
			CopyValueToExternalWorkbook(sourceSheetDefinition, shifted, targetSheetDefinitionIndex);
			thing.Position = shifted;
			thing.SheetDefinitionIndex = targetSheetDefinitionIndex;
			if (shifted.IsValid && (!ShouldReplacePtg3dWithError || !outerSourceRangeAreas))
				ReplaceCurrentExpression(thing);
			else
				ReplaceWithErrorEquivalent(thing);
		}
		void ReplaceWithErrorEquivalent(ParsedThingRefBase thing) {
			ParsedThingBase err = thing.GetRefErrorEquivalent();
			ReplaceCurrentExpression(err);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!IsValidFormula)
				return;
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			bool ifRefersToInsideSource = !sourceSheetDefinition.IsExternalReference && IsThing3dRelRefersToSourceWorksheet(thing.Location, sourceSheetDefinition);
			bool outerSourceRangeAreas = !ifRefersToInsideSource;
			if (outerSourceRangeAreas) {
				if (Option == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
					IsValidFormula = false;
					return;
				}
			}
			int targetSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, ifRefersToInsideSource);
			bool sharedFormulaProcessing = !TargetDataContext.DefinedNameProcessing;  
			if (sharedFormulaProcessing) {
				CellPosition shifted = thing.Location.ToCellPosition(TargetDataContext); 
				CopyValueToExternalWorkbook(sourceSheetDefinition, shifted, targetSheetDefinitionIndex);
			}
			thing.SheetDefinitionIndex = targetSheetDefinitionIndex;
			if (ShouldReplacePtg3dWithError && outerSourceRangeAreas)
				ReplaceWithErrorEquivalent(thing);
			else
				ReplaceCurrentExpression(thing);
		}
		bool IsThing3dRelRefersToSourceWorksheet(CellOffset cellOffset, SheetDefinition sourceSheetDefinition) {
			WorkbookDataContext sourceContext = this.WorkbookSource.DataContext;
			bool sharedFormulaProcessing = sourceContext.SharedFormulaProcessing;
			if (sharedFormulaProcessing) {
				return !sourceSheetDefinition.IsExternalReference && String.Equals(sourceSheetDefinition.SheetNameStart, SourceWorksheet.Name, StringComparison.OrdinalIgnoreCase);
			}
			bool definedNameFormulaProcessing = sourceContext.DefinedNameProcessing;
			if (definedNameFormulaProcessing) {
				bool refersToSourceWorksheet = !sourceSheetDefinition.IsExternalReference && !Args.IsSheetWillBeExternalSheet(sourceSheetDefinition.SheetNameStart);
				if(refersToSourceWorksheet){
				}
				return refersToSourceWorksheet;
			}
			return false;
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (!IsValidFormula)
				return;
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			CellRange sourcethingRange = thing.CellRange.Clone(SourceWorksheet) as CellRange;
			bool ifRefersToInsideSource = !sourceSheetDefinition.IsExternalReference && IsThingArea3dRefersToInsideSourceRange(sourcethingRange, sourceSheetDefinition);
			if ( !ifRefersToInsideSource) {
				if (Option == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
					IsValidFormula = false;
					return;
				}
			}
			int targetSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, ifRefersToInsideSource);
			var sheet = TargetWorksheet;
			CellRange shifted = thing.CellRange.GetShifted(Offset, sheet);
			CopyRangeValuesToExternalWorksheet(sourceSheetDefinition, shifted, targetSheetDefinitionIndex);
			thing.CellRange = shifted;
			thing.SheetDefinitionIndex = targetSheetDefinitionIndex;
			if (shifted != null && (!ShouldReplacePtg3dWithError || ifRefersToInsideSource))
				ReplaceCurrentExpression(thing);
			else
				ReplaceWithErrorEquivalent(thing);
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			base.Visit(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!IsValidFormula)
				return;
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			bool ifRefersToInsideSource = !sourceSheetDefinition.IsExternalReference && IsThing3dRelRefersToSourceWorksheet(thing.First, sourceSheetDefinition);
			bool outerSourceRangeAreas = !ifRefersToInsideSource;
			if (outerSourceRangeAreas) {
				if (Option == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
					IsValidFormula = false;
					return;
				}
			}
			int targetSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, ifRefersToInsideSource);
			thing.SheetDefinitionIndex = targetSheetDefinitionIndex;
			if (ShouldReplacePtg3dWithError && outerSourceRangeAreas)
				ReplaceWithErrorEquivalent(thing);
			else
				ReplaceCurrentExpression(thing);
		}
		public override void Visit(ParsedThingNameX thing) {
			if (!IsValidFormula)
				return;
			SheetDefinition sourceSheetDefinition = WorkbookSource.SheetDefinitions[thing.SheetDefinitionIndex];
			if (Option == SheetDefToOuterAreasReplaceMode.EntireFormulaToValue) {
				IsValidFormula = false;
				return;
			}
			string sheetNameStart = sourceSheetDefinition.SheetNameStart;
			bool isSheetDefinitionExternal = sourceSheetDefinition.IsExternalReference;
			bool isWorkbookGlobalNotExternalDefinedName = !isSheetDefinitionExternal
				&& (String.IsNullOrEmpty(sheetNameStart) || WorkbookSource.DataContext.GetDefinedName(thing.DefinedName, sourceSheetDefinition).ScopedSheetId < 0);
			bool definedNameIsLocalInSourceSheet = !isSheetDefinitionExternal && !isWorkbookGlobalNotExternalDefinedName && String.Equals(sheetNameStart, this.SourceWorksheet.Name, StringComparison.OrdinalIgnoreCase);
			bool definedNameLocalInOtherSheetsToCopy = !isSheetDefinitionExternal && !isWorkbookGlobalNotExternalDefinedName && ! this.Args.IsSheetWillBeExternalSheet(sheetNameStart);
			bool definedNameLocal = definedNameIsLocalInSourceSheet || definedNameLocalInOtherSheetsToCopy || isWorkbookGlobalNotExternalDefinedName;
			bool cloneSheetDefinition = false;
			if (Args.IsCopyWorksheetOperation && definedNameLocal) {
				cloneSheetDefinition = true;
				if (isWorkbookGlobalNotExternalDefinedName) {
					sourceSheetDefinition = sourceSheetDefinition.Clone();
					sourceSheetDefinition.SheetNameStart = Args.SourceRange.Worksheet.Name;
					sourceSheetDefinition.SheetNameEnd = String.Empty;
				}
			}
			int targetSheetDefinitionIndex = GetTargetSheetDefinition(sourceSheetDefinition, cloneSheetDefinition);
			thing.SheetDefinitionIndex = targetSheetDefinitionIndex;
			base.Visit(thing);
			ReplaceCurrentExpression(thing);
		}
		IParsedThing ConvertThingRef3dToValueOrErrorNotAvailable(ParsedThingRef3d thing, IWorksheet sourcesheetDefinitionStart) {
			IParsedThing newThing = new ParsedThingError(VariantValue.ErrorValueNotAvailable.ErrorValue);
			WorkbookSource.DataContext.PushCurrentWorksheet(sourcesheetDefinitionStart);
			WorkbookSource.DataContext.PushCurrentCell(TargetDataContext.CurrentColumnIndex, TargetDataContext.CurrentRowIndex);
			try {
				VariantValue calculatedValue = thing.EvaluateToValue(WorkbookSource.DataContext);
				if (calculatedValue.IsNumeric) {
					newThing = new ParsedThingNumeric(calculatedValue.NumericValue);
				}
				else if (calculatedValue.IsText) {
					string value = calculatedValue.GetTextValue(WorkbookSource.DataContext.StringTable);
					newThing = new ParsedThingStringValue(value);
				}
			}
			finally {
				WorkbookSource.DataContext.PopCurrentCell();
				WorkbookSource.DataContext.PopCurrentWorksheet();
			}
			return newThing;
		}
		public override void Visit(ParsedThingError thing) {
			base.Visit(thing);
			WasParsedThingError = true;
		}
		protected internal override IWorksheet ShouldProcessSheetDefinition(int sheetDefinitionIndex) {
			throw new InvalidOperationException();
		}
		protected internal override IWorksheet ShouldProcessSheetDefinition(SheetDefinition sheetDefinition) {
			throw new InvalidOperationException();
		}
		public int ConvertSheetDefinitionToExternalTargetSheetDefinition(SheetDefinition sourceSheetDefinition) {
			string sheetNameStart = sourceSheetDefinition.SheetNameStart;
			string externalDocumentFilePath = Args.SourceWorkbookFilePath;
			List<string> sourceWorksheetNames = new List<string>();
			Action<IWorksheet> gatherWorksheetNames = sheet => sourceWorksheetNames.Add(sheet.Name);
			if (sourceSheetDefinition.IsExternalReference) {
				var sourceExternalWorkbook = WorkbookSource.GetExternalWorkbookByIndex(sourceSheetDefinition.ExternalReferenceIndex);
				externalDocumentFilePath = sourceExternalWorkbook.FilePath;
				sourceExternalWorkbook.Sheets.ForEach(gatherWorksheetNames);
			}
			else {
				WorkbookSource.GetSheets().ForEach(gatherWorksheetNames);
			}
			SheetDefinition targetExternalSheetDefinition = new SheetDefinition();
			targetExternalSheetDefinition.SheetNameStart = sheetNameStart;
			targetExternalSheetDefinition.SheetNameEnd = sourceSheetDefinition.SheetNameEnd;
			targetExternalSheetDefinition.ExternalReferenceIndex = GetTargetExternalReferenceIndex(externalDocumentFilePath, TargetExternalLinks, sourceSheetDefinition, sourceWorksheetNames);
			int targetSheetDefinitionIndex = TargetDataContext.RegisterSheetDefinition(targetExternalSheetDefinition);
			return targetSheetDefinitionIndex;
		}
		int CopySheetDefinitionToTarget(SheetDefinition sourceSheetDefinition) {
			SheetDefinition target = sourceSheetDefinition.Clone();
			int result = Int32.MinValue;
			string start = target.SheetNameStart;
			string end = target.SheetNameEnd;
			bool isNormalSheetDefinition = !String.IsNullOrEmpty(start) && !target.IsExternalReference;
			bool isStartSheetNamePointsToSourceWorksheet = !String.IsNullOrEmpty(start) && String.Equals(start, SourceWorksheet.Name, StringComparison.OrdinalIgnoreCase);
			if (isStartSheetNamePointsToSourceWorksheet) {
				target.SheetNameStart = TargetWorksheet.Name;
			}
			else if (isNormalSheetDefinition) {
				if (WorkbookTarget.GetSheetByName(start) == null)
					throw new InvalidOperationException(String.Format("Add {0} empty sheet to Target workbook before copy this", start));
				if (!string.IsNullOrEmpty(end) && WorkbookTarget.GetSheetByName(end) == null)
					throw new InvalidOperationException(String.Format("Add {0} empty sheet to Target workbook before copy this", end));
			}
			else if (String.IsNullOrEmpty(start) || !target.ValidReference) {
			}
			else 
				throw new InvalidOperationException("CopySheetDefinitionToTarget");
			result = WorkbookTarget.DataContext.RegisterSheetDefinition(target);
			return result;
		}
		int GetTargetExternalReferenceIndex(string _externalWbFilePath, External.ExternalLinkCollection targetExternalLinkCollection, SheetDefinition sourcesheetDefinition, List<string> sourceWorksheetNames) {
			string sourceSheetNameStart = sourcesheetDefinition.SheetNameStart;
			string sourceSheetNameEnd = sourcesheetDefinition.SheetNameEnd;
			int index = targetExternalLinkCollection.IndexOf(_externalWbFilePath);
			if (index >= 0)
				return index + 1;
			bool sourceWorkbookHasFilePath = !String.IsNullOrEmpty(_externalWbFilePath);
			string externalLinkName = sourceWorkbookHasFilePath ? _externalWbFilePath : sourceSheetNameStart;
			DocumentModel targetModel = WorkbookTarget as DocumentModel;
			External.ExternalLink externalLink = CreateExternalLinkAndAdd(targetModel, externalLinkName);
			CreateExternalWorksheets(externalLink, sourceWorksheetNames, true);
			if (sourcesheetDefinition.IsExternalReference) {
				External.ExternalWorkbook sourceExternalWorkbook = WorkbookSource.GetExternalWorkbookByIndex(sourcesheetDefinition.ExternalReferenceIndex);
				bool sheetStartMissing = !String.IsNullOrEmpty(sourcesheetDefinition.SheetNameStart) && sourceExternalWorkbook.Sheets.GetSheetIndexByName(sourcesheetDefinition.SheetNameStart) == int.MinValue;
				bool sheetEndMissing = !String.IsNullOrEmpty(sourcesheetDefinition.SheetNameEnd) && sourceExternalWorkbook.Sheets.GetSheetIndexByName(sourcesheetDefinition.SheetNameEnd) == int.MinValue;
				List<string> sheetNamesNotRefreshable = new List<string>();
				if (sheetStartMissing)
					sheetNamesNotRefreshable.Add(sourceSheetNameStart);
				if (sheetEndMissing)
					sheetNamesNotRefreshable.Add(sourceSheetNameEnd);
				CreateExternalWorksheets(externalLink, sheetNamesNotRefreshable, false);
			}
			return TargetExternalLinks.Count; 
		}
		External.ExternalLink CreateExternalLinkAndAdd(DocumentModel documentModel, string filePath) {
			External.ExternalLink result = new External.ExternalLink(documentModel);
			result.Workbook.FilePath = filePath;
			documentModel.ExternalLinks.Add(result); 
			return result;
		}
		void CreateExternalWorksheets(External.ExternalLink externalLink, List<string> names, bool refreshable) {
			foreach (string name in names) {
				if (!string.IsNullOrEmpty(name)) {
					var targetExternalSheet = new External.ExternalWorksheet(externalLink.Workbook, name);
					targetExternalSheet.RefreshFailed = !refreshable;
					externalLink.Workbook.Sheets.Add(targetExternalSheet);
				}
			}
		}
		public void CopyValueToExternalWorkbook(SheetDefinition sourceSheetDefinition, CellPosition shifted, int targetSheetDefinitionIndex) {
			IWorksheet source = null;
			if (sourceSheetDefinition.IsExternalReference) {
				var sourceExternalWorkbook = WorkbookSource.GetExternalWorkbookByIndex(sourceSheetDefinition.ExternalReferenceIndex);
				source = sourceExternalWorkbook.Sheets[sourceSheetDefinition.SheetNameStart];
			}
			SheetDefinition targetSheetDefinition = WorkbookTarget.SheetDefinitions[targetSheetDefinitionIndex];
			if (!targetSheetDefinition.IsExternalReference)  
				return;
			if (!sourceSheetDefinition.IsExternalReference)
				source = WorkbookSource.GetSheetByName(sourceSheetDefinition.SheetNameStart);
			External.ExternalWorkbook targetExternalWorkbook = WorkbookTarget.GetExternalWorkbookByIndex(targetSheetDefinition.ExternalReferenceIndex);
			if (source == null)
				return;
			string sheetNameStart = source.Name;
			if (!shifted.IsValid)
				return;
			External.ExternalWorksheet externalSheet = targetExternalWorkbook.Sheets[sheetNameStart];
			CopyValueToExternalWorksheet(source, shifted, externalSheet);
		}
		void CopyValueToExternalWorksheet(IWorksheet referencedSheet, CellPosition position, External.ExternalWorksheet externalSheet) {
			if (!position.IsValid)
				return;
			ICellBase sourceCellToBeReferencedByRef3dThing = (referencedSheet as ICellTable).TryGetCell(position.Column, position.Row);
			ICellBase targetExternalCell = (externalSheet as ICellTable).GetCell(position.Column, position.Row);
			if (sourceCellToBeReferencedByRef3dThing != null) {
				VariantValue sourceValueShouldBeTextOrNumber = sourceCellToBeReferencedByRef3dThing.Value;
				if (sourceValueShouldBeTextOrNumber.IsSharedString) {
					Worksheet worksheet = referencedSheet as Worksheet;
					if (worksheet != null)
						sourceValueShouldBeTextOrNumber = sourceValueShouldBeTextOrNumber.GetTextValue(worksheet.DataContext.StringTable);
					else
						sourceValueShouldBeTextOrNumber = sourceValueShouldBeTextOrNumber.InlineTextValue;
				}
				if (sourceValueShouldBeTextOrNumber.IsNumeric || sourceValueShouldBeTextOrNumber.IsText)
					targetExternalCell.Value = sourceValueShouldBeTextOrNumber;
			}
		}
		public void CopyRangeValuesToExternalWorksheet(SheetDefinition sourceSheetDefinition, CellRangeBase shifted, int targetSheetDefinitionIndex) {
			IWorksheet source = null;
			if (sourceSheetDefinition.IsExternalReference) {
				var sourceExternalWorkbook = WorkbookSource.GetExternalWorkbookByIndex(sourceSheetDefinition.ExternalReferenceIndex);
				source = sourceExternalWorkbook.Sheets[sourceSheetDefinition.SheetNameStart];
			}
			SheetDefinition targetSheetDefinition = WorkbookTarget.SheetDefinitions[targetSheetDefinitionIndex];
			if (!targetSheetDefinition.IsExternalReference)  
				return;
			if (!sourceSheetDefinition.IsExternalReference)
				source = WorkbookSource.GetSheetByName(sourceSheetDefinition.SheetNameStart);
			External.ExternalWorkbook targetExternalWorkbook = WorkbookTarget.GetExternalWorkbookByIndex(targetSheetDefinition.ExternalReferenceIndex);
			if (source == null)
				return;
			if (shifted == null)
				return; 
			string sheetNameStart = source.Name;
			CellRangeBase thingRef3dRange = shifted.Clone(source);
			External.ExternalWorksheet externalSheet = targetExternalWorkbook.Sheets[sheetNameStart];
			foreach (ICellBase referencedExistingCell in thingRef3dRange.GetExistingCellsEnumerable())
				CopyValueToExternalWorksheet(source, referencedExistingCell.Position, externalSheet);
		}
	}
	#endregion
}
