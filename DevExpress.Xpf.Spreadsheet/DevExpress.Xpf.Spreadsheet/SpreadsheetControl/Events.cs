﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet;
#if SL || WPF
namespace DevExpress.Xpf.Spreadsheet {
#else
namespace DevExpress.XtraSpreadsheet {
#endif
	#region SpreadsheetControl
	public partial class SpreadsheetControl {
		#region UpdateUI
		public event EventHandler UpdateUI {
			add { if (InnerControl != null) InnerControl.UpdateUI += value; }
			remove { if (InnerControl != null) InnerControl.UpdateUI -= value; }
		}
		#endregion
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region InvalidFormatException
		public event InvalidFormatExceptionEventHandler InvalidFormatException {
			add { if (InnerControl != null) InnerControl.InvalidFormatException += value; }
			remove { if (InnerControl != null) InnerControl.InvalidFormatException -= value; }
		}
		#endregion
		#region UnhandledException
		public event DevExpress.XtraSpreadsheet.UnhandledExceptionEventHandler UnhandledException {
			add { if (InnerControl != null) InnerControl.UnhandledException += value; }
			remove { if (InnerControl != null) InnerControl.UnhandledException -= value; }
		}
		#endregion
		#region DocumentClosing
		public event CancelEventHandler DocumentClosing {
			add { if (InnerControl != null) InnerControl.DocumentClosing += value; }
			remove { if (InnerControl != null) InnerControl.DocumentClosing -= value; }
		}
		#endregion
		#region DocumentLoaded
		EventHandler onDocumentLoaded;
		public event EventHandler DocumentLoaded { add { onDocumentLoaded += value; } remove { onDocumentLoaded -= value; } }
		protected internal virtual void RaiseDocumentLoaded() {
			if (onDocumentLoaded != null)
				onDocumentLoaded(this, EventArgs.Empty);
		}
		#endregion
		#region EmptyDocumentCreated
		EventHandler onEmptyDocumentCreated;
		public event EventHandler EmptyDocumentCreated { add { onEmptyDocumentCreated += value; } remove { onEmptyDocumentCreated -= value; } }
		protected internal virtual void RaiseEmptyDocumentCreated() {
			if (onEmptyDocumentCreated != null)
				onEmptyDocumentCreated(this, EventArgs.Empty);
		}
		#endregion
		#region ReadOnlyChanged
		EventHandler onReadOnlyChanged;
		public event EventHandler ReadOnlyChanged { add { onReadOnlyChanged += value; } remove { onReadOnlyChanged -= value; } }
		protected internal virtual void RaiseReadOnlyChanged() {
			if (onReadOnlyChanged != null)
				onReadOnlyChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ModifiedChanged
		EventHandler onModifiedChanged;
		public event EventHandler ModifiedChanged { add { onModifiedChanged += value; } remove { onModifiedChanged -= value; } }
		protected internal virtual void RaiseModifiedChanged() {
			if (onModifiedChanged != null)
				onModifiedChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ContentChanged
		EventHandler onContentChanged;
		public event EventHandler ContentChanged { add { onContentChanged += value; } remove { onContentChanged -= value; } }
		protected internal virtual void RaiseContentChanged() {
			if (onContentChanged != null)
				onContentChanged(this, EventArgs.Empty);
		}
		#endregion
		#region ZoomChanged
		public event EventHandler ZoomChanged {
			add { if (InnerControl != null) InnerControl.ZoomChanged += value; }
			remove { if (InnerControl != null) InnerControl.ZoomChanged -= value; }
		}
		#endregion
		#region BeforeImport
		public event BeforeImportEventHandler BeforeImport {
			add { if (InnerControl != null) InnerControl.BeforeImport += value; }
			remove { if (InnerControl != null) InnerControl.BeforeImport -= value; }
		}
		#endregion
		#region BeforeExport
		public event BeforeExportEventHandler BeforeExport {
			add { if (InnerControl != null) InnerControl.BeforeExport += value; }
			remove { if (InnerControl != null) InnerControl.BeforeExport -= value; }
		}
		#endregion
		#region InitializeDocument
		public event EventHandler InitializeDocument {
			add { if (InnerControl != null) InnerControl.InitializeDocument += value; }
			remove { if (InnerControl != null) InnerControl.InitializeDocument -= value; }
		}
		#endregion
		#region UnitChanging
		public event EventHandler UnitChanging {
			add {
				if (InnerControl != null)
					InnerControl.UnitChanging += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.UnitChanging -= value;
			}
		}
		#endregion
		#region UnitChanged
		public event EventHandler UnitChanged {
			add {
				if (InnerControl != null)
					InnerControl.UnitChanged += value;
			}
			remove {
				if (InnerControl != null)
					InnerControl.UnitChanged -= value;
			}
		}
		#endregion
		#region SelectionChanged
		public event EventHandler SelectionChanged {
			add { if (InnerControl != null) InnerControl.SelectionChanged += value; }
			remove { if (InnerControl != null) InnerControl.SelectionChanged -= value; }
		}
		#endregion
		#region ActiveSheetChanging
		public event ActiveSheetChangingEventHandler ActiveSheetChanging {
			add { if (InnerControl != null) InnerControl.ActiveSheetChanging += value; }
			remove { if (InnerControl != null) InnerControl.ActiveSheetChanging -= value; }
		}
		#endregion
		#region ActiveSheetChanged
		public event ActiveSheetChangedEventHandler ActiveSheetChanged {
			add { if (InnerControl != null) InnerControl.ActiveSheetChanged += value; }
			remove { if (InnerControl != null) InnerControl.ActiveSheetChanged -= value; }
		}
		#endregion
		#region CellBeginEdit
		public event CellBeginEditEventHandler CellBeginEdit {
			add { if (InnerControl != null) InnerControl.CellBeginEdit += value; }
			remove { if (InnerControl != null) InnerControl.CellBeginEdit -= value; }
		}
		#endregion
		#region CellEndEdit
		public event CellEndEditEventHandler CellEndEdit {
			add { if (InnerControl != null) InnerControl.CellEndEdit += value; }
			remove { if (InnerControl != null) InnerControl.CellEndEdit -= value; }
		}
		#endregion
		#region CellCancelEdit
		public event CellCancelEditEventHandler CellCancelEdit {
			add { if (InnerControl != null) InnerControl.CellCancelEdit += value; }
			remove { if (InnerControl != null) InnerControl.CellCancelEdit -= value; }
		}
		#endregion
		#region CellValueChanged
		public event CellValueChangedEventHandler CellValueChanged {
			add { if (InnerControl != null) InnerControl.CellValueChanged += value; }
			remove { if (InnerControl != null) InnerControl.CellValueChanged -= value; }
		}
		#endregion
		#region SheetRenaming
		public event SheetRenamingEventHandler SheetRenaming {
			add { if (InnerControl != null) InnerControl.SheetRenaming += value; }
			remove { if (InnerControl != null) InnerControl.SheetRenaming -= value; }
		}
		#endregion
		#region SheetRenamed
		public event SheetRenamedEventHandler SheetRenamed {
			add { if (InnerControl != null) InnerControl.SheetRenamed += value; }
			remove { if (InnerControl != null) InnerControl.SheetRenamed -= value; }
		}
		#endregion
		#region SheetRemoved
		public event SheetRemovedEventHandler SheetRemoved {
			add { if (InnerControl != null) InnerControl.SheetRemoved += value; }
			remove { if (InnerControl != null) InnerControl.SheetRemoved -= value; }
		}
		#endregion
		#region SheetInserted
		public event SheetInsertedEventHandler SheetInserted {
			add { if (InnerControl != null) InnerControl.SheetInserted += value; }
			remove { if (InnerControl != null) InnerControl.SheetInserted -= value; }
		}
		#endregion
		#region RowsRemoved
		public event RowsRemovedEventHandler RowsRemoved {
			add { if (InnerControl != null) InnerControl.RowsRemoved += value; }
			remove { if (InnerControl != null) InnerControl.RowsRemoved -= value; }
		}
		#endregion
		#region RowsInserted
		public event RowsInsertedEventHandler RowsInserted {
			add { if (InnerControl != null) InnerControl.RowsInserted += value; }
			remove { if (InnerControl != null) InnerControl.RowsInserted -= value; }
		}
		#endregion 
		#region ColumnsRemoved
		public event ColumnsRemovedEventHandler ColumnsRemoved {
			add { if (InnerControl != null) InnerControl.ColumnsRemoved += value; }
			remove { if (InnerControl != null) InnerControl.ColumnsRemoved -= value; }
		}
		#endregion
		#region RowsInserted
		public event ColumnsInsertedEventHandler ColumnsInserted {
			add { if (InnerControl != null) InnerControl.ColumnsInserted += value; }
			remove { if (InnerControl != null) InnerControl.ColumnsInserted -= value; }
		}
		#endregion
		#region BeforePrintSheet
		public event BeforePrintSheetEventHandler BeforePrintSheet {
			add { if (InnerControl != null) InnerControl.BeforePrintSheet += value; }
			remove { if (InnerControl != null) InnerControl.BeforePrintSheet -= value; }
		}
		#endregion
		#region HyperlinkClick
		public event HyperlinkClickEventHandler HyperlinkClick {
			add { if (InnerControl != null) InnerControl.HyperlinkClick += value; }
			remove { if (InnerControl != null) InnerControl.HyperlinkClick -= value; }
		}
		#endregion
		#region PanesFrozen
		public event PanesFrozenEventHandler PanesFrozen {
			add { if (InnerControl != null) InnerControl.PanesFrozen += value; }
			remove { if (InnerControl != null) InnerControl.PanesFrozen -= value; }
		}
		#endregion
		#region PanesUnfrozen
		public event PanesUnfrozenEventHandler PanesUnfrozen {
			add { if (InnerControl != null) InnerControl.PanesUnfrozen += value; }
			remove { if (InnerControl != null) InnerControl.PanesUnfrozen -= value; }
		}
		#endregion
		#region ProtectionWarning
		public event HandledEventHandler ProtectionWarning {
			add { if (InnerControl != null) InnerControl.ProtectionWarning += value; }
			remove { if (InnerControl != null) InnerControl.ProtectionWarning -= value; }
		}
		#endregion
		#region DefinedNameEditing
		public event DefinedNameEditingEventHandler DefinedNameEditing {
			add { if (InnerControl != null) InnerControl.DefinedNameEditing += value; }
			remove { if (InnerControl != null) InnerControl.DefinedNameEditing -= value; }
		}
		#endregion
		#region DefinedNameDeleting
		public event DefinedNameDeletingEventHandler DefinedNameDeleting {
			add { if (InnerControl != null) InnerControl.DefinedNameDeleting += value; }
			remove { if (InnerControl != null) InnerControl.DefinedNameDeleting -= value; }
		}
		#endregion
		#region DefinedNameValidating
		public event DefinedNameEditingEventHandler DefinedNameValidating {
			add { if (InnerControl != null) InnerControl.DefinedNameValidating += value; }
			remove { if (InnerControl != null) InnerControl.DefinedNameValidating -= value; }
		}
		#endregion
		#region DocumentPropertiesChanged
		public event DocumentPropertiesChangedEventHandler DocumentPropertiesChanged {
			add { if (InnerControl != null) InnerControl.DocumentPropertiesChanged += value; }
			remove { if (InnerControl != null) InnerControl.DocumentPropertiesChanged -= value; }
		}
		#endregion
		#region ScrollPositionChanged
		public event ScrollPositionChangedEventHandler ScrollPositionChanged {
			add { if (InnerControl != null) InnerControl.ScrollPositionChanged += value; }
			remove { if (InnerControl != null) InnerControl.ScrollPositionChanged -= value; }
		}
		#endregion
		#region RangeCopying
		public event RangeCopyingEventHandler RangeCopying {
			add { if (InnerControl != null) InnerControl.RangeCopying += value; }
			remove { if (InnerControl != null) InnerControl.RangeCopying -= value; }
		} 
		#endregion
		#region RangeCopied
		public event RangeCopiedEventHandler RangeCopied {
			add { if (InnerControl != null) InnerControl.RangeCopied += value; }
			remove { if (InnerControl != null) InnerControl.RangeCopied -= value; }
		} 
		#endregion
		#region ShapesCopying
		public event ShapesCopyingEventHandler ShapesCopying {
			add { if (InnerControl != null) InnerControl.ShapesCopying += value; }
			remove { if (InnerControl != null) InnerControl.ShapesCopying -= value; }
		} 
		#endregion
		#region CopiedRangePasting
		public event CopiedRangePastingEventHandler CopiedRangePasting {
			add { if (InnerControl != null) InnerControl.CopiedRangePasting += value; }
			remove { if (InnerControl != null) InnerControl.CopiedRangePasting -= value; }
		} 
		#endregion
		#region CopiedRangePasted
		public event CopiedRangePastedEventHandler CopiedRangePasted {
			add { if (InnerControl != null) InnerControl.CopiedRangePasted += value; }
			remove { if (InnerControl != null) InnerControl.CopiedRangePasted -= value; }
		} 
		#endregion
		#region ClipboardDataPasting
		public event ClipboardDataPastingEventHandler ClipboardDataPasting {
			add { if (InnerControl != null) InnerControl.ClipboardDataPasting += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataPasting -= value; }
		} 
		#endregion
		#region ClipboardDataObtained
		public event ClipboardDataObtainedEventHandler ClipboardDataObtained {
			add { if (InnerControl != null) InnerControl.ClipboardDataObtained += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataObtained -= value; }
		} 
		#endregion
		#region ClipboardDataPasted
		public event EventHandler ClipboardDataPasted {
			add { if (InnerControl != null) InnerControl.ClipboardDataPasted += value; }
			remove { if (InnerControl != null) InnerControl.ClipboardDataPasted -= value; }
		} 
		#endregion
	}
	#endregion
}
