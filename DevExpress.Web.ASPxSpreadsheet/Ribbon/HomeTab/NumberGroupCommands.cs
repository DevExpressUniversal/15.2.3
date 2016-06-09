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
	public class SRFormatNumberAccountingCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "style";
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatNumberAccountingUSCommand());
			Items.Add(new SRFormatNumberAccountingUKCommand());
			Items.Add(new SRFormatNumberAccountingEuroCommand());
			Items.Add(new SRFormatNumberAccountingPRCCommand());
			Items.Add(new SRFormatNumberAccountingSwissCommand());
		}
	}
	public class SRFormatNumberAccountingUSCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingUS;
			}
		}
	}
	public class SRFormatNumberAccountingUKCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingUK;
			}
		}
	}
	public class SRFormatNumberAccountingEuroCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingEuro;
			}
		}
	}
	public class SRFormatNumberAccountingPRCCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingPRC;
			}
		}
	}
	public class SRFormatNumberAccountingSwissCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccountingSwiss;
			}
		}
	}
	public class SRFormatNumberPercentCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberPercent;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "style";
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatNumberCommaStyleCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberAccounting;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "style";
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatNumberIncreaseDecimalCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberIncreaseDecimal;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatNumberDecreaseDecimalCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNumberDecreaseDecimal;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
}
