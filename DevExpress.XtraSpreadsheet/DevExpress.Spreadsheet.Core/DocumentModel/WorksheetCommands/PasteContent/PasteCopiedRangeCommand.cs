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

using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PasteCopiedRange : PasteContentWorksheetCommand {
		SourceTargetRangesForCopy ranges = null;
		bool cutMode = false;
		public PasteCopiedRange(Worksheet worksheet, IErrorHandler errorHandler)
			: base(worksheet, errorHandler) {
			ShouldClearOfficeClipboard = true;
		}
		public override DocumentFormat Format { get { return DocumentFormat.Undefined; } }
		public override bool ShouldGetContentFromStream { get { return false; } }
		public bool ShouldClearOfficeClipboard { get; set; }
		protected internal override bool IsDataAvailable() {
			return DocumentModel.IsCopyCutMode;
		}
		protected internal override void PopulateDocumentModelFromContentStream(DocumentModel result, Stream stream) {
		}
		protected internal override Stream GetContentStream() {
			return null;
		}
		bool RaisePastingEvent(CellRangeBase targetRange, ModelPasteSpecialFlags modelFlags) {
			CopiedRangePastingEventArgs args = new CopiedRangePastingEventArgs(targetRange, (Spreadsheet.PasteSpecial)modelFlags);
			args.IsCut = cutMode;
			if (DocumentModel.RaiseCopiedRangePasting(args))
				return false;
			this.PasteOptions = new ModelPasteSpecialOptions((ModelPasteSpecialFlags)args.PasteSpecialFlags);
			cutMode = args.IsCut;
			return true;
		}
		protected override bool DoValidate() {
			if (!DocumentModel.IsCopyCutMode)
				return false;
			this.cutMode = DocumentModel.CopiedRangeProvider.IsCut;
			var targetRange = DocumentModel.ActiveSheet.Selection.AsRange().Clone() as CellRangeBase;
			this.ranges = new SourceTargetRangesForCopy(DocumentModel.CopiedRangeProvider.Range, targetRange);
			RangeCopyOperation operation = CreateRangeCopyOperation(ranges, PasteOptions.InnerFlags, cutMode);
			if (!CheckTargetRange(targetRange, ranges.SourceRange, true))
				return false;
			IModelErrorInfo error = operation.Checks();
			return HandleError(error);
		}
		protected virtual RangeCopyOperation CreateRangeCopyOperation(SourceTargetRangesForCopy ranges, ModelPasteSpecialFlags flags, bool isCut) {
			RangeCopyOperation operation = (isCut) ? new CutRangeOperation(ranges) : new RangeCopyOperation(ranges, flags);
			return operation;
		}
		protected internal override void ExecuteCore() {
			if (!DocumentModel.IsCopyCutMode)
				return;
			bool old = DocumentModel.SuppressResetingCopiedRange;
			DocumentModel.SuppressResetingCopiedRange = true;
			if (!RaisePastingEvent(ranges.GetUnionOfTargetRanges(), PasteOptions.InnerFlags))
				return;
			RangeCopyOperation operation = CreateRangeCopyOperation(this.ranges, PasteOptions.InnerFlags, cutMode);
			DocumentModel.BeginUpdate();
			try {
				operation.ErrorHandler = this.ErrorHandler;
				operation.PasteSpecialOptions.ShouldSkipEmptyCellsInSourceRange = PasteOptions.ShouldSkipEmptyCellsInSourceRange; 
				operation.SuppressChecks = true; 
				operation.Execute();
				CellRangeBase rangeToSelect = operation.RangesInfo.GetUnionOfTargetRanges();
				SetRangeForSelection(rangeToSelect);
				DocumentModel.RaiseCopiedRangePasted(this.ranges.SourceRange, rangeToSelect);
				if (this.cutMode) {
					CellRangeBase rangeToClear = operation.GetRangeToClearAfterCut();
					Model.Worksheet sourceWorksheet = ranges.SourceWorksheet;
					if (rangeToClear != null) {
						ClearingSourceRangeOnPasteAfterCutEventArgs args = new ClearingSourceRangeOnPasteAfterCutEventArgs(rangeToClear);
						args.ShouldResetClipboard = ShouldClearOfficeClipboard;
						DocumentModel.RaiseClearingSourceRangeOnPasteAfterCut(args);
						ShouldClearOfficeClipboard = args.ShouldResetClipboard;
						if (!args.Cancel) {
							bool removeHyperlinksIntersectedClearedRange = false;
							bool shouldClearAutoFilter = false;
							sourceWorksheet.ClearAll(rangeToClear, ModelPasteSpecialFlags.All, ErrorHandler, shouldClearAutoFilter, removeHyperlinksIntersectedClearedRange);
							sourceWorksheet.ClearCellsNoShift(rangeToClear);
						}
					}
				}
			}
			finally {
				if (!cutMode)
					DocumentModel.ApplyChanges(DocumentModelChangeActions.UpdateTransactedVersionInCopiedRange);
				DocumentModel.SuppressResetingCopiedRange = old;
				if (cutMode && ShouldClearOfficeClipboard)
					DocumentModel.ClearCopiedRange();
				DocumentModel.EndUpdate();
			}
		}
	}
}
