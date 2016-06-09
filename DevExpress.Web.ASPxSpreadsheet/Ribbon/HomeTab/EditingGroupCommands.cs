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
	public class SRFormatAutoSumCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingAutoSumCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatFunctionsInsertSumCommand());
			Items.Add(new SRFormatFunctionsInsertAverageCommand());
			Items.Add(new SRFormatFunctionsInsertCountNumbersCommand());
			Items.Add(new SRFormatFunctionsInsertMaxCommand());
			Items.Add(new SRFormatFunctionsInsertMinCommand());
		}
	}
	public class SRFormatFunctionsInsertSumCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertSum;
			}
		}
	}
	public class SRFormatFunctionsInsertAverageCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertAverage;
			}
		}
	}
	public class SRFormatFunctionsInsertCountNumbersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertCountNumbers;
			}
		}
	}
	public class SRFormatFunctionsInsertMaxCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertMax;
			}
		}
	}
	public class SRFormatFunctionsInsertMinCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInsertMin;
			}
		}
	}
	public class SRFormatFillCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFillCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatEditingFillDownCommand());
			Items.Add(new SRFormatEditingFillRightCommand());
			Items.Add(new SRFormatEditingFillUpCommand());
			Items.Add(new SRFormatEditingFillLeftCommand());
		}
	}
	public class SRFormatEditingFillDownCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFillDown;
			}
		}
	}
	public class SRFormatEditingFillRightCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFillRight;
			}
		}
	}
	public class SRFormatEditingFillUpCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFillUp;
			}
		}
	}
	public class SRFormatEditingFillLeftCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFillLeft;
			}
		}
	}
	public class SRFormatClearCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatClearAllCommand());
			Items.Add(new SRFormatClearFormatsCommand());
			Items.Add(new SRFormatClearContentsCommand());
			Items.Add(new SRFormatClearCommentsCommand());
			Items.Add(new SRFormatClearHyperlinksCommand());
			Items.Add(new SRFormatRemoveHyperlinksCommand());
		}
	}
	public class SRFormatClearAllCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearAll;
			}
		}
	}
	public class SRFormatClearFormatsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearFormats;
			}
		}
	}
	public class SRFormatClearContentsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearContents;
			}
		}
	}
	public class SRFormatClearCommentsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearComments;
			}
		}
	}
	public class SRFormatClearHyperlinksCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatClearHyperlinks;
			}
		}
	}
	public class SRFormatRemoveHyperlinksCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatRemoveHyperlinks;
			}
		}
	}
	public class SREditingSortAndFilterCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingSortAndFilterCommandGroup;
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
			Items.Add(new SREditingSortAscendingCommand());
			Items.Add(new SREditingSortDescendingCommand());
		}
	}
	public class SREditingSortAscendingCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.DataSortAscending;
			}
		}
	}
	public class SREditingSortDescendingCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.DataSortDescending;
			}
		}
	}
	public class SREditingFindAndSelectCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingFind;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FindAll;
			}
		}
	}
}
