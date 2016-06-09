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
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class PrepareTargetParsedExpressionVisitor : ReferenceThingsRPNVisitor {
		#region fields
		Worksheet sourceWorksheet;
		CellPositionOffset offset;
		ICellTable targetWorksheet;
		bool wasParsedThingError;
		bool shouldRepacePtg3dWithError;
		ICopyEverythingArguments args;
		#endregion
		public PrepareTargetParsedExpressionVisitor(WorkbookDataContext targetContext, ICellTable target, ICopyEverythingArguments args)
			: base(targetContext) {
			this.args = args;
			this.sourceWorksheet = args.SourceRange.Worksheet as Worksheet;
			this.targetWorksheet = target;
			this.wasParsedThingError = false;
			this.shouldRepacePtg3dWithError = false;
		}
		public bool IsDifferentDocumentModels { get { return !Object.ReferenceEquals(sourceWorksheet.Workbook, targetWorksheet.Workbook); } }
		public CellRange SourceRange { get { return args.SourceRange; } }
		public Worksheet SourceWorksheet { get { return sourceWorksheet; } }
		public ICellTable TargetWorksheet { get { return targetWorksheet; } }
		public CellPositionOffset Offset { get { return offset; } set { offset = value; } }
		public bool WasParsedThingError { get { return wasParsedThingError; } set { wasParsedThingError = value; } }
		public bool ShouldReplacePtg3dWithError { get { return shouldRepacePtg3dWithError; } set { shouldRepacePtg3dWithError = value; } }
		protected ICopyEverythingArguments Args { get { return args; } }
		protected override internal IWorksheet ShouldProcessSheetDefinition(int sheetDefinitionIndex) {
			if (IsDifferentDocumentModels)
				throw new InvalidOperationException(); 
			return base.ShouldProcessSheetDefinition(sheetDefinitionIndex);
		}
		protected internal override IWorksheet ShouldProcessSheetDefinition(SheetDefinition sheetDefinition) {
			if (IsDifferentDocumentModels)
				throw new InvalidOperationException(); 
			return base.ShouldProcessSheetDefinition(sheetDefinition);
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition;
			if (args.IsCopyWorksheetOperation && thingWithSheetDefinition != null) {
				SheetDefinition sourceSheetDefinition = DataContext.GetSheetDefinition(thingWithSheetDefinition.SheetDefinitionIndex);
				bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, DataContext, SourceWorksheet);
				if (refersToSourceWorksheet) {
					thingWithSheetDefinition.SheetDefinitionIndex = RenameSheetDefinition(sourceSheetDefinition);
					this.FormulaChanged = true;
				}
			}
			CellPosition newPos = thing.Position.GetShifted(offset, targetWorksheet);
			if (!newPos.IsValid)
				return thing.GetRefErrorEquivalent();
			thing.Position = newPos;
			return thing;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			return base.ProcessRefRel(thing);
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition;
			if (args.IsCopyWorksheetOperation && thingWithSheetDefinition != null) {
				SheetDefinition sourceSheetDefinition = DataContext.GetSheetDefinition(thingWithSheetDefinition.SheetDefinitionIndex);
				bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, DataContext, SourceWorksheet);
				if (refersToSourceWorksheet) {
					thingWithSheetDefinition.SheetDefinitionIndex = RenameSheetDefinition(sourceSheetDefinition);
					this.FormulaChanged = true;
				}
			}
			thing.CellRange = thing.CellRange.GetShifted(offset, targetWorksheet);
			if (thing.CellRange == null)
				return thing.GetRefErrorEquivalent();
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			return thing;
		}
		public override CellRangeBase ProcessCellRange(CellRange range) {
			return range.GetShifted(offset);
		}
		public override void Visit(ParsedThingNameX thing) {
			base.Visit(thing); 
		}
		int RenameSheetDefinition(SheetDefinition sourceSheetDefinition) {
			SheetDefinition modified = sourceSheetDefinition.Clone();
			if (!String.IsNullOrEmpty(sourceSheetDefinition.SheetNameStart))
				modified.SheetNameStart = TargetWorksheet.Name;
			int result = DataContext.RegisterSheetDefinition(modified);
			return result;
		}
		protected bool IsThingRefersToInsideSourceCore(SheetDefinition sourceSheetDefinition) {
			if (sourceSheetDefinition.IsExternalReference)
				throw new InvalidOperationException("process this case before this method call");
			if (Args.IsSheetWillBeExternalSheet(sourceSheetDefinition.SheetNameStart))
				return false;
			return true;
		}
		protected bool IsThingRefRefersToInsideSourceRange(CellPosition nonShiftedSourceCellPosition, SheetDefinition sourceSheetDefinition) {
			bool result = IsThingRefersToInsideSourceCore(sourceSheetDefinition);
			if (!result)
				return false;
			return SourceRange.ContainsCell(nonShiftedSourceCellPosition.Column, nonShiftedSourceCellPosition.Row);
		}
		protected bool IsThingArea3dRefersToInsideSourceRange(CellRange targetShiftedThingRange, SheetDefinition sourceSheetDefinition) {
			bool result = IsThingRefersToInsideSourceCore(sourceSheetDefinition);
			if (!result)
				return false;
			CellRange shiftedInSourceWorksheet = targetShiftedThingRange.Clone(SourceWorksheet) as CellRange;
			return SourceRange.ContainsRange(shiftedInSourceWorksheet);
		}
		#region what to override
		#endregion
	}
}
