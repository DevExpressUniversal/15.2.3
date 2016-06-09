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
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFormatAlignmentTopCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentTop;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "VerticalAlign";
			}
		}
	}
	public class SRFormatAlignmentMiddleCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentMiddle;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "VerticalAlign";
			}
		}
	}
	public class SRFormatAlignmentBottomCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentBottom;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "VerticalAlign";
			}
		}
	}
	public class SRFormatAlignmentLeftCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentLeft;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "HorizontalAlign";
			}
		}
	}
	public class SRFormatAlignmentCenterCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentCenter;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "HorizontalAlign";
			}
		}
	}
	public class SRFormatAlignmentRightCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAlignmentRight;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "HorizontalAlign";
			}
		}
	}
	public class SRFormatDecreaseIndentCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatDecreaseIndent;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatIncreaseIndentCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatIncreaseIndent;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatWrapTextCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatWrapText;
			}
		}		
		protected override bool DefaultShowText {
			get {
				return true;
			}
		}
	}
	public class SREditingMergeCellsGroupCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingMergeCellsCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SREditingMergeAndCenterCellsCommand());
			Items.Add(new SREditingMergeCellsAcrossCommand());
			Items.Add(new SREditingMergeCellsCommand());
			Items.Add(new SREditingUnmergeCellsCommand());
		}
	}
	public class SREditingMergeAndCenterCellsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingMergeAndCenterCells;
			}
		}
	}
	public class SREditingMergeCellsAcrossCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingMergeCellsAcross;
			}
		}
	}
	public class SREditingMergeCellsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingMergeCells;
			}
		}
	}
	public class SREditingUnmergeCellsCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.EditingUnmergeCells;
			}
		}
	}   
}
