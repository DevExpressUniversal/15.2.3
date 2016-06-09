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
	public class SRViewFreezePanesGroupCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewFreezePanesCommandGroup;
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
			Items.Add(new SRViewUnfreezePanesCommand());
			Items.Add(new SRViewFreezePanesCommand());
			Items.Add(new SRViewFreezeTopRowCommand());
			Items.Add(new SRViewFreezeFirstColumnCommand());
		}
	}
	public class SRViewUnfreezePanesCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewUnfreezePanes;
			}
		}
	}
	public class SRViewFreezePanesCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FreezePanesWebCommand;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewFreezePanes;
			}
		}
	}
	public class SRViewFreezeTopRowCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FreezeRowWebCommand;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewFreezeTopRow;
			}
		}
	}
	public class SRViewFreezeFirstColumnCommand : SRDropDownCommandBase {
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FreezeColumnWebCommand;
			}
		}
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.ViewFreezeFirstColumn;
			}
		}
	}
}
