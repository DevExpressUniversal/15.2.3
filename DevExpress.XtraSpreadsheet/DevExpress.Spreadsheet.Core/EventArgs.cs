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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Export;
using DevExpress.Office.Import;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using Cell = DevExpress.Spreadsheet.Cell;
using Worksheet = DevExpress.Spreadsheet.Worksheet;
#if !SL
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Forms;
using System.Drawing;
#else
using DevExpress.Data;
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet {
	#region BeforeImportEventHandler
	[ComVisible(true)]
	public delegate void BeforeImportEventHandler(object sender, SpreadsheetBeforeImportEventArgs e);
	#endregion
	#region SpreadsheetBeforeImportEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetBeforeImportEventArgs : EventArgs {
		#region Fields
		readonly IImporterOptions options;
		readonly DocumentFormat documentFormat;
		#endregion
		public SpreadsheetBeforeImportEventArgs(DocumentFormat documentFormat, IImporterOptions options) {
			if (documentFormat == DocumentFormat.Undefined)
				Exceptions.ThrowArgumentException("documentFormat", documentFormat);
			Guard.ArgumentNotNull(options, "options");
			this.documentFormat = documentFormat;
			this.options = options;
		}
		#region Properties
		public IImporterOptions Options { get { return options; } }
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		#endregion
	}
	#endregion
	#region BeforeExportEventHandler
	[ComVisible(true)]
	public delegate void BeforeExportEventHandler(object sender, SpreadsheetBeforeExportEventArgs e);
	#endregion
	#region SpreadsheetBeforeExportEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetBeforeExportEventArgs : EventArgs {
		#region Fields
		readonly IExporterOptions options;
		readonly DocumentFormat documentFormat;
		#endregion
		public SpreadsheetBeforeExportEventArgs(DocumentFormat documentFormat, IExporterOptions options) {
			if (documentFormat == DocumentFormat.Undefined)
				Exceptions.ThrowArgumentException("documentFormat", documentFormat);
			Guard.ArgumentNotNull(options, "options");
			this.documentFormat = documentFormat;
			this.options = options;
		}
		#region Properties
		public IExporterOptions Options { get { return options; } }
		public DocumentFormat DocumentFormat { get { return documentFormat; } }
		#endregion
	}
	#endregion
	#region InvalidFormatExceptionEventHandler
	[ComVisible(true)]
	public delegate void InvalidFormatExceptionEventHandler(object sender, SpreadsheetInvalidFormatExceptionEventArgs e);
	#endregion
	#region SpreadsheetInvalidFormatExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetInvalidFormatExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		#endregion
		public SpreadsheetInvalidFormatExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		#endregion
	}
	#endregion
	#region ActiveSheetChangeEventArgsBase (abstract class)
	public abstract class ActiveSheetChangeEventArgsBase : EventArgs {
		#region Fields
		readonly string oldActiveSheetName;
		readonly string newActiveSheetName;
		#endregion
		protected ActiveSheetChangeEventArgsBase(string oldActiveSheetName, string newActiveSheetName) {
			Guard.ArgumentIsNotNullOrEmpty(oldActiveSheetName, "oldActiveSheetName");
			Guard.ArgumentIsNotNullOrEmpty(newActiveSheetName, "newActiveSheetName");
			this.oldActiveSheetName = oldActiveSheetName;
			this.newActiveSheetName = newActiveSheetName;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ActiveSheetChangeEventArgsBaseOldActiveSheetName")]
#endif
		public string OldActiveSheetName { get { return oldActiveSheetName; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ActiveSheetChangeEventArgsBaseNewActiveSheetName")]
#endif
		public string NewActiveSheetName { get { return newActiveSheetName; } }
		#endregion
	}
	#endregion
	#region ActiveSheetChangingEventHandler
	[ComVisible(true)]
	public delegate void ActiveSheetChangingEventHandler(object sender, ActiveSheetChangingEventArgs e);
	#endregion
	#region ActiveSheetChangingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class ActiveSheetChangingEventArgs : ActiveSheetChangeEventArgsBase {
		#region Fields
		bool cancel;
		#endregion
		public ActiveSheetChangingEventArgs(string oldActiveSheetName, string newActiveSheetName)
			: base(oldActiveSheetName, newActiveSheetName) {
		}
		#region Properties
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		#endregion
	}
	#endregion
	#region ActiveSheetChangedEventHandler
	[ComVisible(true)]
	public delegate void ActiveSheetChangedEventHandler(object sender, ActiveSheetChangedEventArgs e);
	#endregion
	#region ActiveSheetChangedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class ActiveSheetChangedEventArgs : ActiveSheetChangeEventArgsBase {
		public ActiveSheetChangedEventArgs(string oldActiveSheetName, string newActiveSheetName)
			: base(oldActiveSheetName, newActiveSheetName) {
		}
	}
	#endregion
	#region SheetRenamingEventHandler
	[ComVisible(true)]
	public delegate void SheetRenamingEventHandler(object sender, SheetRenamingEventArgs e);
	#endregion
	#region SheetRenamingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SheetRenamingEventArgs : CancelEventArgs {
		#region Fields
		string oldName;
		string newName;
		#endregion
		public SheetRenamingEventArgs(string oldName, string newName) {
			this.oldName = oldName;
			this.NewName = newName;
		}
		#region Properties
		public string OldName { get { return oldName; } }
		public string NewName { get { return newName; } set { newName = value; } }
		#endregion
	}
	#endregion
	#region SheetRenamedEventHandler
	[ComVisible(true)]
	public delegate void SheetRenamedEventHandler(object sender, SheetRenamedEventArgs e);
	#endregion
	#region SheetRenamedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SheetRenamedEventArgs : EventArgs {
		#region Fields
		string oldName;
		string newName;
		#endregion
		public SheetRenamedEventArgs(string oldName, string newName) {
			this.oldName = oldName;
			this.newName = newName;
		}
		#region Properties
		public string OldName { get { return oldName; } }
		public string NewName { get { return newName; } }
		#endregion
	}
	#endregion
	#region SheetEventArgsBase (abstract class)
	public abstract class SheetEventArgsBase : EventArgs {
		#region Fields
		readonly string sheetName;
		#endregion
		protected SheetEventArgsBase(string sheetName) {
			Guard.ArgumentIsNotNullOrEmpty(sheetName, "sheetName");
			this.sheetName = sheetName;
		}
		#region Properties
		public string SheetName { get { return sheetName; } }
		#endregion
	}
	#endregion
	#region SheetInsertedEventHandler
	[ComVisible(true)]
	public delegate void SheetInsertedEventHandler(object sender, SheetInsertedEventArgs e);
	#endregion
	#region SheetInsertedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SheetInsertedEventArgs : SheetEventArgsBase {
		public SheetInsertedEventArgs(string sheetName)
			: base(sheetName) {
		}
	}
	#endregion
	#region SheetRemovingEventHandler
	[ComVisible(true)]
	public delegate void SheetRemovingEventHandler(object sender, SheetRemovingEventArgs e);
	#endregion
	#region SheetRemovingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SheetRemovingEventArgs : SheetEventArgsBase {
		#region Fields
		bool cancel;
		#endregion
		public SheetRemovingEventArgs(string sheetName)
			: base(sheetName) {
		}
		#region Properties
		public bool Cancel { get { return cancel; } set { cancel = value; } }
		#endregion
	}
	#endregion
	#region SheetRemovedEventHandler
	[ComVisible(true)]
	public delegate void SheetRemovedEventHandler(object sender, SheetRemovedEventArgs e);
	#endregion
	#region SheetRemovedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SheetRemovedEventArgs : SheetEventArgsBase {
		public SheetRemovedEventArgs(string sheetName)
			: base(sheetName) {
		}
	}
	#endregion
	#region ColumnsInserted
	public class ColumnsChangedEventArgs : SheetEventArgsBase {
		#region Fields
		readonly int startIndex;
		readonly int count;
		#endregion
		public ColumnsChangedEventArgs(string sheetName, int startIndex, int count)
			: base(sheetName) {
			this.startIndex = startIndex;
			this.count = count;
		}
		#region Properties
		public int StartIndex { get { return startIndex; } }
		public int Count { get { return count; } }
		#endregion
	}
	#endregion
	#region RowsChanged
	public class RowsChangedEventArgs : ColumnsChangedEventArgs {
		public RowsChangedEventArgs(string sheetName, int startIndex, int count) :
			base(sheetName, startIndex, count) {
		}
	}
	#endregion
	#region RowsRemovedEventHandler
	[ComVisible(true)]
	public delegate void RowsRemovedEventHandler(object sender, RowsChangedEventArgs e);
	#endregion
	#region RowsInsertedEventHandler
	[ComVisible(true)]
	public delegate void RowsInsertedEventHandler(object sender, RowsChangedEventArgs e);
	#endregion
	#region ColumnsRemovedEventHandler
	[ComVisible(true)]
	public delegate void ColumnsRemovedEventHandler(object sender, ColumnsChangedEventArgs e);
	#endregion
	#region ColumnsInsertedEventHandler
	[ComVisible(true)]
	public delegate void ColumnsInsertedEventHandler(object sender, ColumnsChangedEventArgs e);
	#endregion
	#region BeforePrintSheetEventHandler
	public delegate void BeforePrintSheetEventHandler(object sender, BeforePrintSheetEventArgs e);
	#endregion
	#region BeforePrintSheetEventArgs
	public class BeforePrintSheetEventArgs : CancelEventArgs {
		readonly string name;
		readonly int index;
		internal BeforePrintSheetEventArgs(string name, int index) {
			this.name = name;
			this.index = index;
		}
		public string Name { get { return name; } }
		public int Index { get { return index; } }
	}
	#endregion
	#region PanesFrozenEventHandler
	[ComVisible(true)]
	public delegate void PanesFrozenEventHandler(object sender, PanesFrozenEventArgs e);
	#endregion
	#region PanesFrozenEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class PanesFrozenEventArgs : SheetEventArgsBase {
		#region Fields
		readonly int rowOffset;
		readonly int columnOffset;
		readonly DevExpress.XtraSpreadsheet.Model.CellPosition topLeftCell;
		#endregion
		public PanesFrozenEventArgs(string sheetName, int rowOffset, int columnOffset, DevExpress.XtraSpreadsheet.Model.CellPosition topLeftCell)
			: base(sheetName) {
			this.rowOffset = rowOffset;
			this.columnOffset = columnOffset;
			this.topLeftCell = topLeftCell;
		}
		#region Properties
		internal DevExpress.XtraSpreadsheet.Model.CellPosition ModelTopLeftCell { get { return topLeftCell; } } 
		internal IWorkbook Workbook { get; set; }
		public int RowOffset { get { return rowOffset; } }
		public int ColumnOffset { get { return columnOffset; } }
		public Range TopLeft { get { return Workbook.Worksheets[SheetName][topLeftCell.Row, topLeftCell.Column]; } }
		#endregion
	}
	#endregion
	#region PanesUnfrozenEventHandler
	[ComVisible(true)]
	public delegate void PanesUnfrozenEventHandler(object sender, PanesUnfrozenEventArgs e);
	#endregion
	#region PanesUnfrozenEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class PanesUnfrozenEventArgs : SheetEventArgsBase {
		public PanesUnfrozenEventArgs(string sheetName)
			: base(sheetName) {
		}
	}
	#endregion
	#region ScrollPositionChangedEventHandler
	[ComVisible(true)]
	public delegate void ScrollPositionChangedEventHandler(object sender, ScrollPositionChangedEventArgs e);
	#endregion
	#region ScrollPositionChangedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class ScrollPositionChangedEventArgs : SheetEventArgsBase {
		#region Fields
		readonly int columnIndex;
		readonly int rowIndex;
		#endregion
		public ScrollPositionChangedEventArgs(string sheetName, int columnIndex, int rowIndex)
			: base(sheetName) {
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			this.columnIndex = columnIndex;
		}
		#region Properties
		public int ColumnIndex { get { return columnIndex; } }
		public int RowIndex { get { return rowIndex; } }
		#endregion
	}
	#endregion
	public abstract class RangeBasedEventArgs<T> : EventArgs 
		where T : DevExpress.XtraSpreadsheet.Model.CellRangeBase {
		T range;
		bool cellunionAllowed = false;
		DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet worksheet = null;
		internal RangeBasedEventArgs(T range, bool unionAllowed) {
			this.range = range;
			this.cellunionAllowed = unionAllowed;
		}
		protected internal T ModelRange { get { return range; } }
		protected void SetApiRange(DevExpress.Spreadsheet.Range value) {
			var native = value as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeRange;
			if (native == null)
				return;
			DevExpress.XtraSpreadsheet.Model.CellRangeBase newModelRange = native.ModelRange;
			if (!Object.ReferenceEquals(range.Worksheet.Workbook, newModelRange.Worksheet.Workbook)) {
				SpreadsheetExceptions.ThrowInvalidOperationException(DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorkbook);
			}
			DevExpress.XtraSpreadsheet.Model.CellRangeBase sourceRangeBase = newModelRange as DevExpress.XtraSpreadsheet.Model.CellRangeBase;
			if (sourceRangeBase == null)
				SpreadsheetExceptions.ThrowInvalidOperationException(DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetStringId.Msg_ErrorIncorrectReferenceExpression);
			if (!cellunionAllowed) {
				DevExpress.XtraSpreadsheet.Model.CellRange asNormalRange = newModelRange as DevExpress.XtraSpreadsheet.Model.CellRange;
				if (asNormalRange == null)
					SpreadsheetExceptions.ThrowInvalidOperationException(DevExpress.XtraSpreadsheet.Localization.XtraSpreadsheetStringId.Msg_ErrorCommandCannotPerformedWithMultipleSelections);
			}
			range = (T)sourceRangeBase;
		}
		protected DevExpress.Spreadsheet.Range GetApiRange(DevExpress.XtraSpreadsheet.Model.CellRangeBase range) {
			if (worksheet == null)
				Exceptions.ThrowInternalException();
			return new DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeRange(range, worksheet);
		}
		internal void SetRangeApiWorksheet(DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet worksheet) {
			if (worksheet == null)
				throw new ArgumentNullException("worksheet", "worksheet is null.");
			this.worksheet = worksheet;
		}
	}
	public class RangeCopyingEventArgs : RangeBasedEventArgs<DevExpress.XtraSpreadsheet.Model.CellRange> {
		bool isCut;
		bool cancel;
		internal RangeCopyingEventArgs(DevExpress.XtraSpreadsheet.Model.CellRange range, bool isCut)
			: base(range, false) {
			this.cancel = false;
			this.isCut = isCut;
		}
		public bool IsCut { get { return isCut; } set { isCut = value; } }
		public Range Range { get { return GetApiRange(ModelRange); } set { SetApiRange(value); } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public class RangeCopiedEventArgs : EventArgs { 
		bool isCut;
		string cellRange;
		internal RangeCopiedEventArgs(DevExpress.XtraSpreadsheet.Model.CellRange range, bool isCut, XtraSpreadsheet.Model.WorkbookDataContext context) { 
			this.isCut = isCut;
			this.cellRange = range.ToString(context);
		}
		public bool IsCut { get { return isCut; } set { isCut = value; } }
		public string RangeReference { get { return cellRange; } }
	}
	public class ShapesCopyingEventArgs : CancelEventArgs {
		public ShapesCopyingEventArgs() {
		}
	}
	#region CopiedRangePastingEventArgs
	public class CopiedRangePastingEventArgs : RangeBasedEventArgs<DevExpress.XtraSpreadsheet.Model.CellRangeBase> {
		PasteSpecial flags;
		internal CopiedRangePastingEventArgs(DevExpress.XtraSpreadsheet.Model.CellRangeBase targetRange, PasteSpecial flags)
			: base(targetRange, true) {
			this.flags = flags;
		}
		public Range TargetRange { get { return GetApiRange(ModelRange); } }
		public bool IsCut { get; set; }
		public PasteSpecial PasteSpecialFlags { get { return flags; } set { flags = value; } }
		public bool Cancel { get; set; }
	} 
	#endregion
	#region ClipboardDataPastedEventArgs ( not used )
	public class ClipboardDataPastedEventArgs : EventArgs {
		string range;
		internal ClipboardDataPastedEventArgs(DevExpress.XtraSpreadsheet.Model.CellRangeBase range)
			{
			this.range = range.ToString(range.Worksheet.Workbook.DataContext);
		}
		public string RangeReference { get { return range; } }
	} 
	#endregion
	#region CopiedRangePastedEventArgs
	public class CopiedRangePastedEventArgs : EventArgs {
		DevExpress.XtraSpreadsheet.Model.CellRange sourceRange;
		XtraSpreadsheet.Model.CellRangeBase targetRange;
		DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorkbook workbook = null;
		internal CopiedRangePastedEventArgs(XtraSpreadsheet.Model.CellRange sourceRange, XtraSpreadsheet.Model.CellRangeBase targetRange) {
			this.sourceRange = sourceRange;
			this.targetRange = targetRange;
		}
		public Range SourceRange { get { return GetApiRange(sourceRange); } }
		public Range TargetRange { get { return GetApiRange(targetRange); } }
		Range GetApiRange(XtraSpreadsheet.Model.CellRangeBase modelRange) {
			var worksheet = workbook.Worksheets[modelRange.Worksheet.Name] as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet;
			return new DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeRange(modelRange, worksheet);
		}
		internal void SetApiWorkbook(DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorkbook workbook) {
			this.workbook = workbook;
		}
	} 
	#endregion
	#region ClearingSourceRangeOnPasteAfterCutEventArgs ( not used )
	public class ClearingSourceRangeOnPasteAfterCutEventArgs : RangeBasedEventArgs<DevExpress.XtraSpreadsheet.Model.CellRangeBase> {
		internal ClearingSourceRangeOnPasteAfterCutEventArgs(XtraSpreadsheet.Model.CellRangeBase range)
			: base(range, true) {
		}
		public Range Range { get { return GetApiRange(ModelRange); } }
		public bool Cancel { get; set; }
		public bool ShouldResetClipboard { get; set; }
	} 
	#endregion
	#region ClipboardDataObtainedEventArgs
	public class ClipboardDataObtainedEventArgs : CancelEventArgs { 
		readonly XtraSpreadsheet.Model.DocumentModel temporaryModel = null;
		DevExpress.Spreadsheet.Worksheet cachedApiWorksheet = null;
		string rangeReference;
		PasteSpecial flags;
		internal ClipboardDataObtainedEventArgs(
			XtraSpreadsheet.Model.DocumentModel temporaryModelWithContent, string rangeReference, ModelPasteSpecialFlags modelFlags)
			: base(false) {
			temporaryModel = temporaryModelWithContent;
			this.rangeReference = rangeReference;
			this.flags = (PasteSpecial)modelFlags;
		}
		public string Range { get { return rangeReference; } set { rangeReference = value; } }
		public PasteSpecial Flags { get { return flags; } set { flags = value; } }
		public DevExpress.Spreadsheet.Worksheet GetWorksheet() {
			if (cachedApiWorksheet == null) {
				var apiWorkbook = new DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorkbook(temporaryModel);
				cachedApiWorksheet = apiWorkbook.NativeWorksheets[0];
			}
			return cachedApiWorksheet;
		}
	} 
	#endregion
	#region RangeCopyingEventHandler
	[ComVisible(true)]
	public delegate void RangeCopyingEventHandler(object sender, RangeCopyingEventArgs e);
	#endregion
	#region RangeCopiedEventHandler
	[ComVisible(true)]
	public delegate void RangeCopiedEventHandler(object sender, RangeCopiedEventArgs e);
	#endregion
	#region CopiedRangePastingEventHandler
	[ComVisible(true)]
	public delegate void CopiedRangePastingEventHandler(object sender, CopiedRangePastingEventArgs e);
	#endregion
	#region CopiedRangePastedEventHandler
	[ComVisible(true)]
	public delegate void CopiedRangePastedEventHandler(object sender, CopiedRangePastedEventArgs e);
	#endregion
	#region ClipboardDataPastedEventHandler ( not used )
	#endregion
	#region ClearingSourceRangeOnPasteAfterCutEventHandler   (not used)
	[ComVisible(true)]
	public delegate void ClearingSourceRangeOnPasteAfterCutEventHandler(
		object sender, ClearingSourceRangeOnPasteAfterCutEventArgs e);
	#endregion
	#region ClipboardDataPastingEventHandler
	[ComVisible(true)]
	public delegate void ClipboardDataPastingEventHandler(object sender, EventArgs e);
	#endregion
	#region ShapesCopyingEventHandler
	[ComVisible(true)]
	public delegate void ShapesCopyingEventHandler(object sender, ShapesCopyingEventArgs e);
	#endregion
	#region ClipboardDataObtainedEventHandler
	[ComVisible(true)]
	public delegate void ClipboardDataObtainedEventHandler(object sender, ClipboardDataObtainedEventArgs e);
	#endregion
}
namespace DevExpress.XtraSpreadsheet {
	#region UnhandledExceptionEventHandler
	[ComVisible(true)]
	public delegate void UnhandledExceptionEventHandler(object sender, SpreadsheetUnhandledExceptionEventArgs e);
	#endregion
	#region SpreadsheetUnhandledExceptionEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetUnhandledExceptionEventArgs : EventArgs {
		#region Fields
		readonly Exception exception;
		bool handled;
		#endregion
		public SpreadsheetUnhandledExceptionEventArgs(Exception e) {
			Guard.ArgumentNotNull(e, "e");
			this.exception = e;
		}
		#region Properties
		public Exception Exception { get { return exception; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	#region SpreadsheetCellEventArgsBase (abstract class)
	public abstract class SpreadsheetCellEventArgsBase : EventArgs {
		#region Fields
		readonly Model.Worksheet sheet;
		readonly int rowIndex;
		readonly int columnIndex;
		Cell apiCell;
		#endregion
		protected SpreadsheetCellEventArgsBase(Model.Worksheet sheet, int columnIndex, int rowIndex) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sheet = sheet;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseSheetName")]
#endif
		public string SheetName { get { return sheet.Name; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseColumnIndex")]
#endif
		public int ColumnIndex { get { return columnIndex; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseRowIndex")]
#endif
		public int RowIndex { get { return rowIndex; } }
		internal InnerSpreadsheetControl InnerControl { get; set; }
		#region Worksheet
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseWorksheet")]
#endif
		public Worksheet Worksheet {
			get {
				if (InnerControl == null)
					return null;
				return InnerControl.Document.Worksheets[sheet.Name];
			}
		}
		#endregion
		#region Cell
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseCell")]
#endif
		public Cell Cell {
			get {
				if (apiCell != null)
					return apiCell;
				Worksheet apiSheet = Worksheet;
				if (apiSheet == null)
					return null;
				apiCell = apiSheet[rowIndex, columnIndex];
				return apiCell;
			}
		}
		#endregion
		#region Value
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseValue")]
#endif
		public CellValue Value {
			get {
				Cell cell = Cell;
				if (cell == null)
					return CellValue.Empty;
				else
					return cell.Value;
			}
		}
		#endregion
		#region Formula
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseFormula")]
#endif
		public string Formula {
			get {
				Cell cell = Cell;
				if (cell == null)
					return String.Empty;
				else
					return cell.Formula;
			}
		}
		#endregion
		#region FormulaInvariant
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetCellEventArgsBaseFormulaInvariant")]
#endif
		public string FormulaInvariant {
			get {
				Cell cell = Cell;
				if (cell == null)
					return String.Empty;
				else
					return cell.FormulaInvariant;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region CellBeginEditEventHandler
	[ComVisible(true)]
	public delegate void CellBeginEditEventHandler(object sender, SpreadsheetCellCancelEventArgs e);
	#endregion
	#region SpreadsheetCellCancelEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetCellCancelEventArgs : SpreadsheetCellEventArgsBase {
		internal SpreadsheetCellCancelEventArgs(Model.Worksheet sheet, int columnIndex, int rowIndex)
			: base(sheet, columnIndex, rowIndex) {
		}
		#region Properties
		public bool Cancel { get; set; }
		#endregion
	}
	#endregion
	#region CellEndEditEventHandler
	[ComVisible(true)]
	public delegate void CellEndEditEventHandler(object sender, SpreadsheetCellValidatingEventArgs e);
	#endregion
	#region SpreadsheetCellValidatingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetCellValidatingEventArgs : SpreadsheetCellCancelEventArgs {
		#region Fields
		string editorText;
		#endregion
		public SpreadsheetCellValidatingEventArgs(Model.Worksheet sheet, int columnIndex, int rowIndex, string editorText)
			: base(sheet, columnIndex, rowIndex) {
			this.editorText = editorText;
		}
		#region Properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use the EditorText property instead.", true)]
		public string TextValue { get { return editorText; } }
		public string EditorText { get { return editorText; } set { editorText = value; } }
		#endregion
	}
	#endregion
	#region CellCancelEditEventHandler
	[ComVisible(true)]
	public delegate void CellCancelEditEventHandler(object sender, SpreadsheetCellCancelEditEventArgs e);
	#endregion
	#region SpreadsheetCellCancelEditEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetCellCancelEditEventArgs : SpreadsheetCellEventArgsBase {
		internal SpreadsheetCellCancelEditEventArgs(Model.Worksheet sheet, int columnIndex, int rowIndex)
			: base(sheet, columnIndex, rowIndex) {
		}
	}
	#endregion
	#region CellValueChangedEventHandler
	[ComVisible(true)]
	public delegate void CellValueChangedEventHandler(object sender, SpreadsheetCellEventArgs e);
	#endregion
	#region SpreadsheetCellEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class SpreadsheetCellEventArgs : SpreadsheetCellEventArgsBase {
		#region Fields
		readonly Model.CellContentSnapshot cellSnapshot;
		#endregion
		internal SpreadsheetCellEventArgs(Model.CellContentSnapshot cellSnapshot)
			: base(cellSnapshot.Cell.Worksheet, cellSnapshot.Cell.ColumnIndex, cellSnapshot.Cell.RowIndex) {
			Guard.ArgumentNotNull(cellSnapshot, "cellSnapshot");
			this.cellSnapshot = cellSnapshot;
		}
		#region Properties
		#region OldValue
		public CellValue OldValue {
			get {
				if (cellSnapshot == null) 
					return Value;
				else
					return new CellValue(cellSnapshot.Value, cellSnapshot.Cell.Context);
			}
		}
		#endregion
		#region OldFormula
		public string OldFormula {
			get {
				if (cellSnapshot == null) 
					return Formula;
				else
					return cellSnapshot.Formula;
			}
		}
		#endregion
		#region OldFormulaInvariant
		public string OldFormulaInvariant {
			get {
				if (cellSnapshot == null) 
					return FormulaInvariant;
				else
					return cellSnapshot.FormulaInvariant;
			}
		}
		#endregion
		#endregion
	}
	#endregion
	#region HyperlinkClickEventHandler
	[ComVisible(true)]
	public delegate void HyperlinkClickEventHandler(object sender, HyperlinkClickEventArgs e);
	#endregion
	#region HyperlinkClickEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class HyperlinkClickEventArgs : EventArgs {
		bool handled;
		readonly DevExpress.XtraSpreadsheet.Internal.InnerSpreadsheetControl control;
		readonly Keys modifierKeys;
		Range targetRange;
		internal HyperlinkClickEventArgs(DevExpress.XtraSpreadsheet.Internal.InnerSpreadsheetControl control, Keys modifierKeys) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.modifierKeys = modifierKeys;
		}
		public bool Handled { get { return handled; } set { handled = value; } }
		public Keys ModifierKeys { get { return modifierKeys; } }
		public bool Control { get { return (modifierKeys & Keys.Control) == Keys.Control; } }
		public bool Alt { get { return (modifierKeys & Keys.Alt) == Keys.Alt; } }
		public bool Shift { get { return (modifierKeys & Keys.Shift) == Keys.Shift; } }
		public bool IsExternal { get; internal set; }
		public string TargetUri { get; internal set; }
		internal DevExpress.XtraSpreadsheet.Model.CellRangeBase ModelTargetRange { get; set; }
		public Range TargetRange {
			get {
				if (targetRange == null) {
					if (ModelTargetRange == null)
						return null;
					Worksheet sheet = control.Document.Worksheets[ModelTargetRange.Worksheet.Name];
					if (sheet == null)
						return null;
					targetRange = sheet.Range.FromLTRB(ModelTargetRange.TopLeft.Column, ModelTargetRange.TopLeft.Row, ModelTargetRange.BottomRight.Column, ModelTargetRange.BottomRight.Row);
				}
				return targetRange;
			}
		}
	}
	#endregion
#if !SL
	#region ShowFormEventArgs
	public class ShowFormEventArgs : EventArgs {
		#region Fields
		bool handled;
		DialogResult dialogResult = DialogResult.None;
		IWin32Window parent;
		#endregion
		#region Properties
		public DialogResult DialogResult { get { return dialogResult; } set { dialogResult = value; } }
		public IWin32Window Parent { get { return parent; } set { parent = value; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		#endregion
	}
	#endregion
	#region FormShowingEventArgs (abstract class)
	public abstract class FormShowingEventArgs : ShowFormEventArgs {
		protected FormShowingEventArgs(FormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.Parent = controllerParameters.Control;
		}
	}
	#endregion
	#region FormatCellsFormShowingEventHandler
	public delegate void FormatCellsFormShowingEventHandler(object sender, FormatCellsFormShowingEventArgs e);
	#endregion
	#region FormatCellsFormShowingEventArgs
	public class FormatCellsFormShowingEventArgs : FormShowingEventArgs {
		readonly FormatCellsFormControllerParameters controllerParameters;
		public FormatCellsFormShowingEventArgs(FormatCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public FormatCellsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region HyperlinkFormShowingEventHandler
	public delegate void HyperlinkFormShowingEventHandler(object sender, HyperlinkFormShowingEventArgs e);
	#endregion
	#region HyperlinkFormShowingEventArgs
	public class HyperlinkFormShowingEventArgs : FormShowingEventArgs {
		readonly HyperlinkFormControllerParameters controllerParameters;
		public HyperlinkFormShowingEventArgs(HyperlinkFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public HyperlinkFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region PasteSpecialFormShowingEventHandler
	public delegate void PasteSpecialFormShowingEventHandler(object sender, PasteSpecialFormShowingEventArgs e);
	#endregion
	#region PasteSpecialFormShowingEventArgs
	public class PasteSpecialFormShowingEventArgs : FormShowingEventArgs {
		readonly PasteSpecialFormControllerParameters controllerParameters;
		public PasteSpecialFormShowingEventArgs(PasteSpecialFormControllerParameters controllerParameters)
			: base(controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
		}
		public PasteSpecialFormControllerParameters ControllerParameters { get { return controllerParameters; } }
	}
	#endregion
	#region DefinedNameEventArgsBase
	public abstract class DefinedNameEventArgsBase : CancelEventArgs {
		#region Properties
		public string Name { get; private set; }
		public string Scope { get; private set; }
		public int ScopeIndex { get; private set; }
		public string Reference { get; private set; }
		public string Comment { get; private set; }
		#endregion
		protected DefinedNameEventArgsBase(string name, string scope, int scopeIndex, string reference, string comment)
			: base() {
			Name = name;
			Scope = scope;
			ScopeIndex = scopeIndex;
			Reference = reference;
			Comment = comment;
		}
	}
	#endregion
	#region DefinedNameDeletingEventHandler
	public delegate void DefinedNameDeletingEventHandler(object sender, DefinedNameDeletingEventArgs e);
	#endregion
	#region DefinedNameDeletingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class DefinedNameDeletingEventArgs : DefinedNameEventArgsBase {
		internal DefinedNameDeletingEventArgs(string name, string scope, int scopeIndex, string reference, string comment)
			: base(name, scope, scopeIndex, reference, comment) {
		}
	}
	#endregion
	#region DefinedNameEditingEventHandler
	public delegate void DefinedNameEditingEventHandler(object sender, DefinedNameEditingEventArgs e);
	#endregion
	#region DefinedNameEditingEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class DefinedNameEditingEventArgs : DefinedNameEventArgsBase {
		#region Properties
		public string OriginalName { get; private set; }
		#endregion
		internal DefinedNameEditingEventArgs(string name, string originalName, string scope, int scopeIndex, string reference, string comment)
			: base(name, scope, scopeIndex, reference, comment) {
			OriginalName = originalName;
		}
	}
	#endregion
	#region DocumentPropertiesChangedEventHandler
	public delegate void DocumentPropertiesChangedEventHandler(object sender, DocumentPropertiesChangedEventArgs e);
	#endregion
	#region DocumentPropertiesChangedEventArgs
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class DocumentPropertiesChangedEventArgs : EventArgs {
		#region Properties
		public bool BuiltInPropertiesChanged { get; private set; }
		public bool CustomPropertiesChanged { get; private set; }
		#endregion
		internal DocumentPropertiesChangedEventArgs(bool builtInPropertiesChanged, bool customPropertiesChanged)
			: base() {
			BuiltInPropertiesChanged = builtInPropertiesChanged;
			CustomPropertiesChanged = customPropertiesChanged;
		}
	}
	#endregion
#endif
}
namespace DevExpress.XtraSpreadsheet.Model {
	#region DocumentContentChangedEventHandler
	public delegate void DocumentContentChangedEventHandler(object sender, DocumentContentChangedEventArgs e);
	#endregion
	#region DocumentContentChangedEventArgs
	public class DocumentContentChangedEventArgs : EventArgs {
		readonly bool suppressBindingNotifications;
		public DocumentContentChangedEventArgs(bool suppressBindingNotifications) {
			this.suppressBindingNotifications = suppressBindingNotifications;
		}
		public bool SuppressBindingNotifications { get { return suppressBindingNotifications; } }
	}
	#endregion
	#region SheetVisibleStateChangedEventHandler
	public delegate void SheetVisibleStateChangedEventHandler(object sender, SheetVisibleStateChangedEventArgs e);
	#endregion
	#region SheetVisibleStateChangedEventArgs
	public class SheetVisibleStateChangedEventArgs : SheetEventArgsBase {
		#region Fields
		SheetVisibleState oldValue;
		SheetVisibleState newValue;
		#endregion
		public SheetVisibleStateChangedEventArgs(string sheetName, SheetVisibleState oldValue, SheetVisibleState newValue)
			: base(sheetName) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public SheetVisibleState OldValue { get { return oldValue; } }
		public SheetVisibleState NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region SheetTabColorChangedEventHandler
	public delegate void SheetTabColorChangedEventHandler(object sender, SheetTabColorChangedEventArgs e);
	#endregion
	#region SheetTabColorChangedEventArgs
	public class SheetTabColorChangedEventArgs : SheetEventArgsBase {
		#region Fields
		Color oldValue;
		Color newValue;
		#endregion
		public SheetTabColorChangedEventArgs(string sheetName, Color oldValue, Color newValue)
			: base(sheetName) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public Color OldValue { get { return oldValue; } }
		public Color NewValue { get { return newValue; } }
		#endregion
	}
	#endregion
	#region CellRemovingEventHandler
	public delegate void CellRemovingEventHandler(object sender, CellRemovingEventArgs e);
	#endregion
	#region CellRemovingEventArgs
	public class CellRemovingEventArgs : SheetEventArgsBase {
		#region Fields
		CellKey key;
		#endregion
		public CellRemovingEventArgs(string sheetName, CellKey key)
			: base(sheetName) {
			this.key = key;
		}
		#region Properties
		public CellKey Key { get { return key; } }
		#endregion
	}
	#endregion
}
