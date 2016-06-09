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

using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFormatInsertCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertCellsCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatInsertSheetRowsCommand());
			Items.Add(new SRFormatInsertSheetColumnsCommand());
			Items.Add(new SRFormatInsertSheetCommand());
		}
	}
	public class SRFormatInsertSheetRowsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertSheetRows;
			}
		}
	}
	public class SRFormatInsertSheetColumnsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertSheetColumns;
			}
		}
	}
	public class SRFormatInsertSheetCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.InsertSheet;
			}
		}
	}
	public class SRFormatRemoveCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.RemoveCellsCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatRemoveSheetRowsCommand());
			Items.Add(new SRFormatRemoveSheetColumnsCommand());
			Items.Add(new SRFormatRemoveSheetCommand());
		}
	}
	public class SRFormatRemoveSheetRowsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.RemoveSheetRows;
			}
		}
	}
	public class SRFormatRemoveSheetColumnsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.RemoveSheetColumns;
			}
		}
	}
	public class SRFormatRemoveSheetCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.RemoveSheet;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.RemoveSheet;
			}
		}
	}
	public class SRFormatFormatCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatRowHeightCommand());
			Items.Add(new SRFormatAutoFitRowHeightCommand());
			Items.Add(new SRFormatColumnWidthCommand());
			Items.Add(new SRFormatAutoFitColumnWidthCommand());
			Items.Add(new SRFormatDefaultColumnWidthCommand());
			Items.Add(new SRFormatHideAndUnhideCommand());
			Items.Add(new SRFormatRenameSheetCommand());
			Items.Add(new SRFormatMoverOrCopySheetCommand());
		}
	}
	public class SRFormatRowHeightCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatRowHeight;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatRowHeight;
			}
		}
	}
	public class SRFormatAutoFitRowHeightCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAutoFitRowHeight;
			}
		}
	}
	public class SRFormatColumnWidthCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatColumnWidth;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatColumnWidth;
			}
		}
	}
	public class SRFormatAutoFitColumnWidthCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAutoFitColumnWidth;
			}
		}
	}
	public class SRFormatDefaultColumnWidthCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatDefaultColumnWidth;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatDefaultColumnWidth;
			}
		}
	}
	public class SRFormatHideAndUnhideCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.HideAndUnhideCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatHideRowsCommand());
			Items.Add(new SRFormatHideColumnsCommand());
			Items.Add(new SRFormatHideSheetCommand());
			Items.Add(new SRFormatUnhideRowsCommand());
			Items.Add(new SRFormatUnhideColumnsCommand());
			Items.Add(new SRFormatUnhideSheetCommand());
		}
	}
	public class SRFormatHideRowsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.HideRows;
			}
		}
	}
	public class SRFormatHideColumnsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.HideColumns;
			}
		}
	}
	public class SRFormatHideSheetCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.HideSheet;
			}
		}
	}
	public class SRFormatUnhideRowsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.UnhideRows;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.UnhideRows;
			}
		}
	}
	public class SRFormatUnhideColumnsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.UnhideColumns;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.UnhideColumns;
			}
		}
	}
	public class SRFormatUnhideSheetCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.UnhideSheet;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.UnhideSheet;
			}
		}
	}
	public class SRFormatRenameSheetCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.RenameSheet;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.RenameSheet;
			}
		}
	}
	public class SRFormatMoverOrCopySheetCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.MoveOrCopySheetWebCommand;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.MoveOrCopySheet;
			}
		}
	}
}
