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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetDocumentServer {
		#region UnitChanging
		EventHandler onUnitChanging;
		public event EventHandler UnitChanging { add { onUnitChanging += value; } remove { onUnitChanging -= value; } }
		protected internal virtual void RaiseUnitChanging() {
			if (onUnitChanging != null)
				onUnitChanging(Owner, EventArgs.Empty);
		}
		#endregion
		#region UnitChanged
		EventHandler onUnitChanged;
		public event EventHandler UnitChanged { add { onUnitChanged += value; } remove { onUnitChanged -= value; } }
		protected internal virtual void RaiseUnitChanged() {
			if (onUnitChanged != null)
				onUnitChanged(this, EventArgs.Empty);
		}
		#endregion
		#region DocumentClosing
		CancelEventHandler onDocumentClosing;
		public event CancelEventHandler DocumentClosing { add { onDocumentClosing += value; } remove { onDocumentClosing -= value; } }
		protected internal virtual bool RaiseDocumentClosing() {
			if (onDocumentClosing != null) {
				CancelEventArgs args = new CancelEventArgs();
				onDocumentClosing(Owner, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region UpdateUI
		EventHandler onUpdateUI;
		public event EventHandler UpdateUI { add { onUpdateUI += value; } remove { onUpdateUI -= value; } }
		protected internal virtual void RaiseUpdateUI() {
			if (onUpdateUI != null)
				onUpdateUI(Owner, EventArgs.Empty);
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region InnerDocumentLoaded
		EventHandler onInnerDocumentLoaded;
		public event EventHandler InnerDocumentLoaded { add { onInnerDocumentLoaded += value; } remove { onInnerDocumentLoaded -= value; } }
		protected internal void RaiseInnerDocumentLoaded() {
			if (onInnerDocumentLoaded != null)
				onInnerDocumentLoaded(Owner, EventArgs.Empty);
		}
		#endregion
		#region DocumentLoaded
		EventHandler onDocumentLoaded;
		public event EventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal virtual void RaiseDocumentLoaded() {
			if (onDocumentLoaded != null)
				onDocumentLoaded(Owner, EventArgs.Empty);
		}
		#endregion
		#region InnerEmptyDocumentCreated
		EventHandler onInnerEmptyDocumentCreated;
		public event EventHandler InnerEmptyDocumentCreated { add { onInnerEmptyDocumentCreated += value; } remove { onInnerEmptyDocumentCreated -= value; } }
		protected internal void RaiseInnerEmptyDocumentCreated() {
			if (onInnerEmptyDocumentCreated != null)
				onInnerEmptyDocumentCreated(Owner, EventArgs.Empty);
		}
		#endregion
		#region EmptyDocumentCreated
		EventHandler onEmptyDocumentCreated;
		public event EventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal virtual void RaiseEmptyDocumentCreated() {
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(Owner, EventArgs.Empty);
		}
		bool ShouldRaiseEmptyDocumentCreated() {
			return onEmptyDocumentCreated != null;
		}
		#endregion
		#region BeforeImport
		BeforeImportEventHandler onBeforeImport;
		public event BeforeImportEventHandler BeforeImport { add { onBeforeImport += value; } remove { onBeforeImport -= value; } }
		protected internal virtual void RaiseBeforeImport(SpreadsheetBeforeImportEventArgs args) {
			if (onBeforeImport != null)
				onBeforeImport(Owner, args);
		}
		#endregion
		#region BeforeExport
		BeforeExportEventHandler onBeforeExport;
		public event BeforeExportEventHandler BeforeExport { add { onBeforeExport += value; } remove { onBeforeExport -= value; } }
		protected internal virtual void RaiseBeforeExport(SpreadsheetBeforeExportEventArgs args) {
			if (onBeforeExport != null)
				onBeforeExport(Owner, args);
		}
		#endregion
		#region InitializeDocument
		EventHandler onInitializeDocument;
		public event EventHandler InitializeDocument { add { onInitializeDocument += value; } remove { onInitializeDocument -= value; } }
		protected internal virtual void RaiseInitializeDocument(EventArgs args) {
			if (onInitializeDocument != null)
				onInitializeDocument(Owner, args);
		}
		#endregion
		#region InvalidFormatException
		InvalidFormatExceptionEventHandler onInvalidFormatException;
		public event InvalidFormatExceptionEventHandler InvalidFormatException { add { onInvalidFormatException += value; } remove { onInvalidFormatException -= value; } }
		protected internal virtual void RaiseInvalidFormatException(Exception e) {
			if (onInvalidFormatException != null) {
				SpreadsheetInvalidFormatExceptionEventArgs args = new SpreadsheetInvalidFormatExceptionEventArgs(e);
				onInvalidFormatException(Owner, args);
			}
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		public event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged(bool suppressBindingNotifications) {
			if (onContentChanged != null)
				onContentChanged(Owner, new DocumentContentChangedEventArgs(suppressBindingNotifications));
		}
		#endregion
		#region SchemaChanged
		EventHandler onSchemaChanged;
		public event EventHandler SchemaChanged { add { onSchemaChanged += value; } remove { onSchemaChanged -= value; } }
		protected internal virtual void RaiseSchemaChanged() {
			if (onSchemaChanged != null)
				onSchemaChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ActiveViewChanged
		EventHandler onActiveViewChanged;
		public event EventHandler ActiveViewChanged { add { onActiveViewChanged += value; } remove { onActiveViewChanged -= value; } }
		protected internal virtual void RaiseActiveViewChanged() {
			if (onActiveViewChanged != null)
				onActiveViewChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region SelectionChanged
		EventHandler onSelectionChanged;
		public event EventHandler SelectionChanged { add { onSelectionChanged += value; } remove { onSelectionChanged -= value; } }
		protected internal virtual void RaiseSelectionChanged() {
			if (onSelectionChanged != null)
				onSelectionChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ActiveSheetChanging
		ActiveSheetChangingEventHandler onActiveSheetChanging;
		public event ActiveSheetChangingEventHandler ActiveSheetChanging { add { onActiveSheetChanging += value; } remove { onActiveSheetChanging -= value; } }
		protected internal virtual void RaiseActiveSheetChanging(ActiveSheetChangingEventArgs args) {
			if (onActiveSheetChanging != null)
				onActiveSheetChanging(Owner, args);
		}
		#endregion
		#region ActiveSheetChanged
		ActiveSheetChangedEventHandler onActiveSheetChanged;
		public event ActiveSheetChangedEventHandler ActiveSheetChanged { add { onActiveSheetChanged += value; } remove { onActiveSheetChanged -= value; } }
		protected internal virtual void RaiseActiveSheetChanged(ActiveSheetChangedEventArgs args) {
			if (onActiveSheetChanged != null)
				onActiveSheetChanged(Owner, args);
		}
		#endregion
		#region SheetRenaming
		SheetRenamingEventHandler onSheetRenaming;
		public event SheetRenamingEventHandler SheetRenaming { add { onSheetRenaming += value; } remove { onSheetRenaming -= value; } }
		protected internal virtual void RaiseSheetRenaming(SheetRenamingEventArgs args) {
			if (onSheetRenaming != null)
				onSheetRenaming(Owner, args);
		}
		#endregion
		#region SheetRenamed
		SheetRenamedEventHandler onSheetRenamed;
		public event SheetRenamedEventHandler SheetRenamed { add { onSheetRenamed += value; } remove { onSheetRenamed -= value; } }
		protected internal virtual void RaiseSheetRenamed(SheetRenamedEventArgs args) {
			if (onSheetRenamed != null)
				onSheetRenamed(Owner, args);
		}
		#endregion
		#region SheetInserted
		SheetInsertedEventHandler onSheetInserted;
		public event SheetInsertedEventHandler SheetInserted { add { onSheetInserted += value; } remove { onSheetInserted -= value; } }
		protected internal virtual void RaiseSheetInserted(SheetInsertedEventArgs args) {
			if (onSheetInserted != null) {
				onSheetInserted(Owner, args);
			}
		}
		#endregion
		#region SheetRemoved
		SheetRemovedEventHandler onSheetRemoved;
		public event SheetRemovedEventHandler SheetRemoved { add { onSheetRemoved += value; } remove { onSheetRemoved -= value; } }
		protected internal virtual void RaiseSheetRemoved(SheetRemovedEventArgs args) {
			if (onSheetRemoved != null) {
				onSheetRemoved(Owner, args);
			}
		}
		#endregion
		#region RowsRemoved
		RowsRemovedEventHandler onRowsRemoved;
		public event RowsRemovedEventHandler RowsRemoved { add { onRowsRemoved += value; } remove { onRowsRemoved -= value; } }
		protected internal virtual void RaiseRowsRemoved(RowsChangedEventArgs args) {
			if (onRowsRemoved != null)
				onRowsRemoved(Owner, args);
		}
		#endregion
		#region RowsInserted
		RowsInsertedEventHandler onRowsInserted;
		public event RowsInsertedEventHandler RowsInserted { add { onRowsInserted += value; } remove { onRowsInserted -= value; } }
		protected internal virtual void RaiseRowsInserted(RowsChangedEventArgs args) {
			if (onRowsInserted != null)
				onRowsInserted(Owner, args);
		}
		#endregion
		#region ColumnsRemoved
		ColumnsRemovedEventHandler onColumnsRemoved;
		public event ColumnsRemovedEventHandler ColumnsRemoved { add { onColumnsRemoved += value; } remove { onColumnsRemoved -= value; } }
		protected internal virtual void RaiseColumnsRemoved(ColumnsChangedEventArgs args) {
			if (onColumnsRemoved != null)
				onColumnsRemoved(Owner, args);
		}
		#endregion
		#region ColumnsInserted
		ColumnsInsertedEventHandler onColumnsInserted;
		public event ColumnsInsertedEventHandler ColumnsInserted { add { onColumnsInserted += value; } remove { onColumnsInserted -= value; } }
		protected internal virtual void RaiseColumnsInserted(ColumnsChangedEventArgs args) {
			if (onColumnsInserted != null)
				onColumnsInserted(Owner, args);
		}
		#endregion
		#region BeforePrintSheet
		BeforePrintSheetEventHandler onBeforePrintSheet;
		public event BeforePrintSheetEventHandler BeforePrintSheet { add { onBeforePrintSheet += value; } remove { onBeforePrintSheet -= value; } }
		protected internal virtual bool RaiseBeforePrintSheet(BeforePrintSheetEventArgs args) {
			if (onBeforePrintSheet != null) {
				onBeforePrintSheet(Owner, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region PanesFrozen
		PanesFrozenEventHandler onPanesFrozen;
		public event PanesFrozenEventHandler PanesFrozen { add { onPanesFrozen += value; } remove { onPanesFrozen -= value; } }
		protected internal void RaisePanesFrozen(PanesFrozenEventArgs args) {
			if (onPanesFrozen != null) {
				args.Workbook = Document;
				onPanesFrozen(this, args);
			}
		}
		#endregion
		#region PanesUnfrozen
		PanesUnfrozenEventHandler onPanesUnfrozen;
		public event PanesUnfrozenEventHandler PanesUnfrozen { add { onPanesUnfrozen += value; } remove { onPanesUnfrozen -= value; } }
		protected internal void RaisePanesUnfrozen(PanesUnfrozenEventArgs args) {
			if (onPanesUnfrozen != null)
				onPanesUnfrozen(this, args);
		}
		#endregion
		#region ScrollPositionChanged
		ScrollPositionChangedEventHandler onScrollPositionChanged;
		public event ScrollPositionChangedEventHandler ScrollPositionChanged { add { onScrollPositionChanged += value; } remove { onScrollPositionChanged -= value; } }
		protected internal void RaiseScrollPositionChanged(ScrollPositionChangedEventArgs args) {
			if (onScrollPositionChanged != null)
				onScrollPositionChanged(this, args);
		}
		#endregion
		RangeCopyingEventHandler onRangeCopying;
		public event RangeCopyingEventHandler RangeCopying { add { onRangeCopying += value; } remove { onRangeCopying -= value; } }
		protected internal void RaiseRangeCopying(RangeCopyingEventArgs args) {
			if (onRangeCopying != null) {
				string sheetName = args.ModelRange.Worksheet.Name;
				var sheet = NativeDocument.NativeWorksheets[sheetName] as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet;
				args.SetRangeApiWorksheet(sheet);
				onRangeCopying(this, args);
			}
		}
		RangeCopiedEventHandler onRangeCopied;
		public event RangeCopiedEventHandler RangeCopied {  add { onRangeCopied += value; } remove { onRangeCopied -= value; } }
		protected internal void RaiseRangeCopied(RangeCopiedEventArgs args) {
			if (onRangeCopied != null)
				onRangeCopied(this, args);
		}
		ShapesCopyingEventHandler onShapesCopying;
		public event ShapesCopyingEventHandler ShapesCopying { add { onShapesCopying += value; } remove { onShapesCopying -= value; } }
		protected internal void RaiseShapesCopying(ShapesCopyingEventArgs args) {
			if (onShapesCopying != null)
				onShapesCopying(this, args);
		}
		CopiedRangePastingEventHandler onCopiedRangePasting;
		public event CopiedRangePastingEventHandler CopiedRangePasting { add { onCopiedRangePasting += value; } remove { onCopiedRangePasting -= value; } }
		protected internal void RaiseCopiedRangePasting(CopiedRangePastingEventArgs args) {
			if (onCopiedRangePasting != null) {
				string sheetName = args.ModelRange.Worksheet.Name;
				var sheet = NativeDocument.NativeWorksheets[sheetName] as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorksheet;
				args.SetRangeApiWorksheet(sheet);
				onCopiedRangePasting(this, args);
			}
		}
		CopiedRangePastedEventHandler onCopiedRangePasted;
		public event CopiedRangePastedEventHandler CopiedRangePasted { add { onCopiedRangePasted += value; } remove { onCopiedRangePasted -= value; } }
		protected internal void RaiseCopiedRangePasted(CopiedRangePastedEventArgs args) {
			if (onCopiedRangePasted != null) {
				args.SetApiWorkbook(Document as DevExpress.XtraSpreadsheet.API.Native.Implementation.NativeWorkbook);
				onCopiedRangePasted(this, args);
			}
		}
		ClipboardDataPastingEventHandler onClipboardDataPasting;
		public event ClipboardDataPastingEventHandler ClipboardDataPasting { add { onClipboardDataPasting += value; } remove { onClipboardDataPasting -= value; } }
		protected internal void RaiseClipboardDataPasting(EventArgs args) {
			if (onClipboardDataPasting != null) {
				onClipboardDataPasting(this, args);
			}
		}
		ClipboardDataObtainedEventHandler onClipboardDataObtained;
		public event ClipboardDataObtainedEventHandler ClipboardDataObtained { add { onClipboardDataObtained += value; } remove { onClipboardDataObtained -= value; } }
		protected internal void RaiseClipboardDataObtained(ClipboardDataObtainedEventArgs args) {
			if (onClipboardDataObtained != null) {
				onClipboardDataObtained(this, args);
			}
		}
		EventHandler onClipboardDataPasted;
		public event EventHandler ClipboardDataPasted { add { onClipboardDataPasted += value; } remove { onClipboardDataPasted -= value; } }
		protected internal void RaiseClipboardDataPasted(EventArgs args) {
			if (onClipboardDataPasted != null) {
				onClipboardDataPasted(this, args);
			}
		}
	}
	public partial class InnerSpreadsheetControl {
		#region UnhandledException
		UnhandledExceptionEventHandler onUnhandledException;
		public event UnhandledExceptionEventHandler UnhandledException { add { onUnhandledException += value; } remove { onUnhandledException -= value; } }
		public bool RaiseUnhandledException(Exception e) {
			try {
				if (onUnhandledException != null) {
					SpreadsheetUnhandledExceptionEventArgs args = new SpreadsheetUnhandledExceptionEventArgs(e);
					onUnhandledException(Owner, args);
					return args.Handled;
				}
				else
					return false;
			}
			catch {
				return false;
			}
		}
		#endregion
		#region ReadOnlyChanged
		EventHandler onReadOnlyChanged;
		public event EventHandler ReadOnlyChanged { add { onReadOnlyChanged += value; } remove { onReadOnlyChanged -= value; } }
		protected internal virtual void RaiseReadOnlyChanged() {
			if (onReadOnlyChanged != null)
				onReadOnlyChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region ZoomChanged
		EventHandler onZoomChanged;
		public event EventHandler ZoomChanged { add { onZoomChanged += value; } remove { onZoomChanged -= value; } }
		protected internal virtual void RaiseZoomChanged() {
			if (onZoomChanged != null)
				onZoomChanged(Owner, EventArgs.Empty);
		}
		#endregion
		#region CellBeginEdit
		CellBeginEditEventHandler onCellBeginEdit;
		public event CellBeginEditEventHandler CellBeginEdit { add { onCellBeginEdit += value; } remove { onCellBeginEdit -= value; } }
		protected internal virtual bool RaiseCellBeginEdit(Model.Worksheet sheet, int columnIndex, int rowIndex) {
			if (onCellBeginEdit != null) {
				SpreadsheetCellCancelEventArgs args = new SpreadsheetCellCancelEventArgs(sheet, columnIndex, rowIndex);
				args.InnerControl = this;
				onCellBeginEdit(Owner, args);
				return !args.Cancel;
			}
			else
				return true;
		}
		#endregion
		#region CellEndEdit
		CellEndEditEventHandler onCellEndEdit;
		public event CellEndEditEventHandler CellEndEdit { add { onCellEndEdit += value; } remove { onCellEndEdit -= value; } }
		protected internal virtual CellEndEditResult RaiseCellEndEdit(Model.Worksheet sheet, int columnIndex, int rowIndex, string value) {
			if (onCellEndEdit != null) {
				SpreadsheetCellValidatingEventArgs args = new SpreadsheetCellValidatingEventArgs(sheet, columnIndex, rowIndex, value);
				args.InnerControl = this;
				onCellEndEdit(Owner, args);
				return new CellEndEditResult(args.Cancel, args.EditorText);
			}
			else
				return new CellEndEditResult(false, value);
		}
		#endregion
		#region CellCancelEdit
		CellCancelEditEventHandler onCellCancelEdit;
		public event CellCancelEditEventHandler CellCancelEdit { add { onCellCancelEdit += value; } remove { onCellCancelEdit -= value; } }
		protected internal virtual void RaiseCellCancelEdit(Model.Worksheet sheet, int columnIndex, int rowIndex) {
			if (onCellCancelEdit != null) {
				SpreadsheetCellCancelEditEventArgs args = new SpreadsheetCellCancelEditEventArgs(sheet, columnIndex, rowIndex);
				args.InnerControl = this;
				onCellCancelEdit(Owner, args);
			}
		}
		#endregion
		#region CellValueChanged
		CellValueChangedEventHandler onCellValueChanged;
		public event CellValueChangedEventHandler CellValueChanged { add { onCellValueChanged += value; } remove { onCellValueChanged -= value; } }
		protected internal override void RaiseCellValueChanged(SpreadsheetCellEventArgs args) {
			if (onCellValueChanged != null) {
				args.InnerControl = this;
				onCellValueChanged(Owner, args);
			}
		}
		#endregion
		#region HyperlinkClick
		HyperlinkClickEventHandler onHyperlinkClick;
		public event HyperlinkClickEventHandler HyperlinkClick { add { onHyperlinkClick += value; } remove { onHyperlinkClick -= value; } }
		protected internal virtual bool RaiseHyperlinkClick(HyperlinkClickEventArgs args) {
			if (onHyperlinkClick != null) {
				onHyperlinkClick(Owner, args);
				return args.Handled;
			}
			return false;
		}
		#endregion
		#region ProtectionWarning
		HandledEventHandler onProtectionWarning;
		public event HandledEventHandler ProtectionWarning { add { onProtectionWarning += value; } remove { onProtectionWarning -= value; } }
		protected internal virtual bool RaiseProtectionWarning() {
			if (onProtectionWarning != null) {
				HandledEventArgs args = new HandledEventArgs();
				onProtectionWarning(Owner, args);
				return args.Handled;
			}
			else
				return false;
		}
		#endregion
		#region DefinedNameEditing
		DefinedNameEditingEventHandler onDefinedNameEditing;
		public event DefinedNameEditingEventHandler DefinedNameEditing { add { onDefinedNameEditing += value; } remove { onDefinedNameEditing -= value; } }
		public virtual bool RaiseDefinedNameEditing(string name, string originalName, string scope, int scopeIndex, string reference, string comment) {
			if (onDefinedNameEditing != null) {
				DefinedNameEditingEventArgs args = new DefinedNameEditingEventArgs(name, originalName, scope, scopeIndex, reference, comment);
				onDefinedNameEditing(Owner, args);
				return args.Cancel;
			}
			else
				return false;
		}
		#endregion
		#region DefinedNameDeleting
		DefinedNameDeletingEventHandler onDefinedNameDeleting;
		public event DefinedNameDeletingEventHandler DefinedNameDeleting { add { onDefinedNameDeleting += value; } remove { onDefinedNameDeleting -= value; } }
		protected internal virtual bool RaiseDefinedNameDeleting(string name, string scope, int scopeIndex, string reference, string comment) {
			if (onDefinedNameDeleting != null) {
				DefinedNameDeletingEventArgs args = new DefinedNameDeletingEventArgs(name, scope, scopeIndex, reference, comment);
				onDefinedNameDeleting(Owner, args);
				return args.Cancel;
			}
			else
				return false;
		}
		#endregion
		#region DefinedNameValidating
		DefinedNameEditingEventHandler onDefinedNameValidating;
		public event DefinedNameEditingEventHandler DefinedNameValidating { add { onDefinedNameValidating += value; } remove { onDefinedNameValidating -= value; } }
		protected internal virtual bool RaiseDefinedNameValidating(string name, string originalName, string scope, int scopeIndex, string reference, string comment) {
			if (onDefinedNameValidating != null) {
				DefinedNameEditingEventArgs args = new DefinedNameEditingEventArgs(name, originalName, scope, scopeIndex, reference, comment);
				onDefinedNameValidating(Owner, args);
				return args.Cancel;
			}
			else
				return false;
		}
		#endregion
		#region DocumentPropertiesChanged
		DocumentPropertiesChangedEventHandler onDocumentPropertiesChanged;
		public event DocumentPropertiesChangedEventHandler DocumentPropertiesChanged { add { onDocumentPropertiesChanged += value; } remove { onDocumentPropertiesChanged -= value; } }
		protected internal override void RaiseDocumentPropertiesChanged(bool builtInPropertiesChanged, bool customPropertiesChanged) {
			if (onDocumentPropertiesChanged != null && (builtInPropertiesChanged || customPropertiesChanged)) {
				DocumentPropertiesChangedEventArgs args = new DocumentPropertiesChangedEventArgs(builtInPropertiesChanged, customPropertiesChanged);
				onDocumentPropertiesChanged(Owner, args);
			}
		}
		#endregion
	}
	public struct CellEndEditResult {
		public bool Cancelled { get; private set; }
		public string Text { get; private set; }
		public CellEndEditResult(bool cancelled, string text)
			: this() {
			Cancelled = cancelled;
			Text = text;
		}
	}
}
