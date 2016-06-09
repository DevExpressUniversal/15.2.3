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

using DevExpress.Office.Commands.Internal;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Import.Csv;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class PasteContentWorksheetCommand : ErrorHandledWorksheetCommand {
		CellRangeBase rangeToSelect = null;
		PasteSource pasteSource;
		ModelPasteSpecialOptions options;
		protected PasteContentWorksheetCommand(Worksheet worksheet, IErrorHandler errorHandler)
			: base(worksheet, errorHandler) {
			this.pasteSource = new EmptyPasteSource();
			PasteOptions = new ModelPasteSpecialOptions();
		}
		public CellRangeBase RangeToSelect { get { return rangeToSelect; } }
		public abstract bool ShouldGetContentFromStream { get; }
		public virtual string SourceFilePath { get { return String.Empty; } }
		public abstract DocumentFormat Format { get; }
		public PasteSource PasteSource { get { return pasteSource; } set { pasteSource = value; } }
		protected virtual bool IsTextOrCommaSeparatedImporterUsed { get { return false; } }
		public ModelPasteSpecialOptions PasteOptions {
			get { return options; }
			set {
				this.options = new ModelPasteSpecialOptions(GetRestrictedFlags(value.InnerFlags));
				this.options.ShouldSkipEmptyCellsInSourceRange = value.ShouldSkipEmptyCellsInSourceRange;
			}
		}
		public Action<CellRange> ShowUnprotectRangeForm { get; set; }
		protected virtual ModelPasteSpecialFlags GetRestrictedFlags(ModelPasteSpecialFlags flags) {
			return flags;
		}
		protected internal override void ExecuteCore() {
			DocumentModel source = CreateSourceDocumentModel();
			PasteContent(source);
		}
		protected internal override bool Validate() {
			return DoValidate();
		}
		protected virtual bool DoValidate() {
			return true;
		}
		void PasteContent(DocumentModel source) {
			if (source == null)
				return;
			CellRange sourceRange = ObtainSourceRange(source);
			if (sourceRange == null)
				return;
			Worksheet sourceWorksheet = sourceRange.Worksheet as Worksheet;
			if (sourceWorksheet == null)
				return;
			ModelPasteSpecialFlags commandFlags = PasteOptions.InnerFlags;
			ClipboardDataObtainedEventArgs args = new ClipboardDataObtainedEventArgs(source, sourceRange.ToString(), commandFlags);
			DocumentModel.RaiseClipboardDataObtained(args);
			if (args.Cancel)
				return;
			commandFlags = (ModelPasteSpecialFlags) args.Flags;
			sourceRange = CellRange.Create(sourceWorksheet, args.Range);
			CellRange targetRange = Worksheet.Selection.ActiveRange;
			SourceTargetRangesForCopy ranges = new SourceTargetRangesForCopy(sourceRange, targetRange);
			if (!CheckTargetRange(targetRange, sourceRange, true))
				return;
			bool pasteXlsFromClipboard = PasteOptions.ShouldCopyOtherFormats;
			if (!pasteXlsFromClipboard) {
				var targetRangeHasNotTheSameShapeAsSourceRange = ranges.First.TargetRanges.Count != 1;
				if (targetRangeHasNotTheSameShapeAsSourceRange) {
					if (!HandleError(new ClarificationErrorInfo(ModelErrorType.PasteAreaNotSameSizeAsSelectionClarification)))
						return;
					CellRange onlyFirstTargetRange = ranges.First.TargetRanges[0];
					ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(sourceRange, onlyFirstTargetRange);
				}
			}
			ModelPasteSpecialFlags commandFlagsWithOtherFormats = commandFlags | ModelPasteSpecialFlags.OtherFormats;
			RangeCopyOperation operation = new RangeCopyOperation(ranges, commandFlagsWithOtherFormats);
			operation.ErrorHandler = this.ErrorHandler;
			operation.ShouldConvertSourceStringTextValueToFormattedValue = IsTextOrCommaSeparatedImporterUsed;
			operation.PasteSpecialOptions.ShouldSkipEmptyCellsInSourceRange = PasteOptions.ShouldSkipEmptyCellsInSourceRange;
			operation.SheetDefinitionToOuterAreasReplaceMode = SheetDefToOuterAreasReplaceMode.EntireFormulaToValue;
			IModelErrorInfo error = operation.Checks();
			bool executionAllowed = HandleError(error);
			if (executionAllowed) {
				operation.PasteSpecialOptions.InnerFlags = commandFlags; 
				operation.SuppressChecks = true;
				operation.Execute();
				CellRangeBase rangeToSelect = ranges.GetUnionOfTargetRanges();
				SetRangeForSelection(rangeToSelect);
				DocumentModel.RaiseClipboardDataPasted(rangeToSelect);
			}
		}
		protected bool CheckTargetRange(CellRangeBase targetRange, CellRange sourceRange, bool withAskingPass) {
			Worksheet sheet = targetRange.Worksheet as Worksheet;
			if (sheet != null && !sheet.Properties.Protection.SheetLocked)
				return true;
			return targetRange.ForAll(r => CheckTargetRangeCore(r, sourceRange, withAskingPass));
		}
		bool CheckTargetRangeCore(CellRange targetRange, CellRange sourceRange, bool withAskingPassword) {
			Worksheet sheet = targetRange.Worksheet as Worksheet;
			CellRange fullTargetRange = new CellRange(sheet, targetRange.TopLeft, new CellPosition(Math.Max(targetRange.BottomRight.Column, targetRange.TopLeft.Column + sourceRange.Width - 1), Math.Max(targetRange.BottomRight.Row, targetRange.TopLeft.Row + sourceRange.Height - 1)));
			bool allowed = false;
			if (sheet.TryEditRangeContent(fullTargetRange, this.ShowUnprotectRangeForm, null))
				allowed = true;
			else
				allowed = DocumentModel.CheckRangeAccess(fullTargetRange);
			if (!allowed) {
				IModelErrorInfo readonlyError = new ModelErrorInfo(ModelErrorType.CellOrChartIsReadonly);
				HandleError(readonlyError);
				return false;
			}
			return true;
		}
		protected internal virtual CellRange ObtainSourceRange(DocumentModel source) {
			return source.Sheets[0].GetPrintRange();
		}
		protected void SetRangeForSelection(CellRangeBase newRange) {
			rangeToSelect = newRange;
		}
		protected internal virtual DocumentModel CreateSourceDocumentModel() {
			Stream stream = GetContentStream();
			if(stream == null)
				return null;
			DocumentModel result = Worksheet.Workbook.CreateEmptyCopy();
			result.SwitchToEmptyHistory(true);
			PopulateDocumentModelFromContentStream(result, stream);
			result.Properties.CalculationOptions.CalculationMode = ModelCalculationMode.Manual;
			return result;
		}
		protected string GetTextFromMemoryStream(string clipboarddataFormat, Encoding encoding) {
			string text = string.Empty;
			byte[] bytes = GetBytesFromMemoryStream(clipboarddataFormat);
			text = encoding.GetString(bytes, 0, bytes.Length);
			return text;
		}
		protected byte[] GetBytesFromMemoryStream(string clipboarddataFormat) {
			MemoryStream stream = PasteSource.GetData(clipboarddataFormat) as MemoryStream;
			if (stream == null) {
				return new byte[0];
			}
			byte[] bytes = new byte[Math.Min(4096, stream.Length)];
			stream.Read(bytes, 0, bytes.Length);
			return bytes;
		}
		protected internal abstract bool IsDataAvailable();
		protected internal abstract Stream GetContentStream();
		protected internal abstract void PopulateDocumentModelFromContentStream(DocumentModel result, Stream stream);
	}
}
