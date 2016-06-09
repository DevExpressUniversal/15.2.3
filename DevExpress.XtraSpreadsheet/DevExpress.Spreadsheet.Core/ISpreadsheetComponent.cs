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
using System.IO;
using System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.Office;
using System.ComponentModel;
namespace DevExpress.Spreadsheet {
	#region ISpreadsheetComponent
	public interface ISpreadsheetComponent : IBatchUpdateable, IServiceContainer, ISupportsContentChanged {
		DocumentOptions Options { get; }
		DocumentUnit Unit { get; set; }
		bool Modified { get; set; }
		IClipboardManager Clipboard { get; }
		void CreateNewDocument();
		bool LoadDocument(string fileName);
		bool LoadDocument(string fileName, DocumentFormat format);
		bool LoadDocument(Stream stream, DocumentFormat format);
		bool LoadDocument(byte[] buffer, DocumentFormat format);
		void SaveDocument(string fileName);
		void SaveDocument(string fileName, DocumentFormat format);
		void SaveDocument(Stream stream, DocumentFormat format);
		byte[] SaveDocument(DocumentFormat format);
		event ActiveSheetChangingEventHandler ActiveSheetChanging;
		event ActiveSheetChangedEventHandler ActiveSheetChanged;
		event SheetRenamingEventHandler SheetRenaming;
		event SheetRenamedEventHandler SheetRenamed;
		event SheetInsertedEventHandler SheetInserted;
		event SheetRemovedEventHandler SheetRemoved;
		event RowsInsertedEventHandler RowsInserted;
		event RowsRemovedEventHandler RowsRemoved;
		event ColumnsInsertedEventHandler ColumnsInserted;
		event ColumnsRemovedEventHandler ColumnsRemoved;
		event EventHandler SelectionChanged;
		event PanesFrozenEventHandler PanesFrozen;
		event PanesUnfrozenEventHandler PanesUnfrozen;
		event ScrollPositionChangedEventHandler ScrollPositionChanged;
		event EventHandler ModifiedChanged;
		event EventHandler UnitChanging;
		event EventHandler UnitChanged;
		event EventHandler DocumentLoaded;
		event EventHandler EmptyDocumentCreated;
		event BeforeImportEventHandler BeforeImport;
		event BeforeExportEventHandler BeforeExport;
		event EventHandler InitializeDocument;
		event InvalidFormatExceptionEventHandler InvalidFormatException;
		event BeforePrintSheetEventHandler BeforePrintSheet;
		event RangeCopyingEventHandler RangeCopying;
		event ShapesCopyingEventHandler ShapesCopying;
		event RangeCopiedEventHandler RangeCopied;
		event CopiedRangePastingEventHandler CopiedRangePasting;
		event CopiedRangePastedEventHandler CopiedRangePasted;
		event ClipboardDataPastingEventHandler ClipboardDataPasting;
		event ClipboardDataObtainedEventHandler ClipboardDataObtained;
		event EventHandler ClipboardDataPasted;
	}
	#endregion
}
