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

using System.Collections.Generic;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region HyperlinkChangeDisplayTextHistoryItem
	public class HyperlinkChangeDisplayTextHistoryItem : SpreadsheetStringHistoryItem {
		readonly ModelHyperlink hyperlink;
		public HyperlinkChangeDisplayTextHistoryItem(ModelHyperlink hyperlink, string oldDisplayText, string newDisplayText)
			: base(hyperlink.Workbook.ActiveSheet, oldDisplayText, newDisplayText) {
			this.hyperlink = hyperlink;
		}
		protected override void UndoCore() {
			hyperlink.SetDisplayTextCore(OldValue);
		}
		protected override void RedoCore() {
			hyperlink.SetDisplayTextCore(NewValue);
		}
	}
	#endregion
	#region HyperlinkChangeTooltipTextHistoryItem
	public class HyperlinkChangeTooltipTextHistoryItem : SpreadsheetStringHistoryItem {
		readonly ModelHyperlink hyperlink;
		public HyperlinkChangeTooltipTextHistoryItem(ModelHyperlink hyperlink, string oldTooltipText, string newTooltipText)
			: base(hyperlink.Workbook.ActiveSheet, oldTooltipText, newTooltipText) {
			this.hyperlink = hyperlink;
		}
		protected override void UndoCore() {
			hyperlink.SetTooltipTextCore(OldValue);
		}
		protected override void RedoCore() {
			hyperlink.SetTooltipTextCore(NewValue);
		}
	}
	#endregion
	#region HyperlinkChangeReferenceHistoryItem
	public class HyperlinkChangeReferenceHistoryItem : SpreadsheetHistoryItem {
		readonly ModelHyperlink hyperlink;
		readonly CellRange oldRange;
		readonly CellRange newRange;
		public HyperlinkChangeReferenceHistoryItem(ModelHyperlink hyperlink, CellRange oldRange, CellRange newRange)
			: base(hyperlink.Workbook.ActiveSheet) {
			this.hyperlink = hyperlink;
			this.oldRange = oldRange;
			this.newRange = newRange;
		}
		protected override void UndoCore() {
			hyperlink.SetReferenceCore(OldValue);
		}
		protected override void RedoCore() {
			hyperlink.SetReferenceCore(NewValue);
		}
		#region Properties
		public CellRange OldValue { get { return oldRange; } }
		public CellRange NewValue { get { return newRange; } }
		#endregion
	}
	#endregion
	#region HyperlinkChangeIsExternalHistoryItem
	public class HyperlinkChangeIsExternalHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly ModelHyperlink hyperlink;
		public HyperlinkChangeIsExternalHistoryItem(ModelHyperlink hyperlink, bool oldIsExternal, bool newIsExternal)
			: base(hyperlink.Workbook.ActiveSheet, oldIsExternal, newIsExternal) {
			this.hyperlink = hyperlink;
		}
		protected override void UndoCore() {
			hyperlink.SetIsExternalCore(OldValue);
		}
		protected override void RedoCore() {
			hyperlink.SetIsExternalCore(NewValue);
		}
	}
	#endregion
}
