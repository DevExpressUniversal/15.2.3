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
using System.Collections.Generic;
using DevExpress.Office.History;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region DefinedNameChangeNameHistoryItem
	public class DefinedNameChangeNameHistoryItem : HistoryItem {
		#region Fields
		readonly DefinedName definedName;
		string oldName;
		string newName;
		#endregion
		public DefinedNameChangeNameHistoryItem(DefinedName definedName, string oldName, string newName)
			: base(definedName.Workbook) {
			this.definedName = definedName;
			this.oldName = oldName;
			this.newName = newName;
		}
		protected override void UndoCore() {
			definedName.SetNameCore(oldName);
		}
		protected override void RedoCore() {
			definedName.SetNameCore(newName);
		}
	}
	#endregion
	#region DefinedCommentChangeCommentHistoryItem
	public class DefinedCommentChangeCommentHistoryItem : HistoryItem {
		#region Fields
		readonly DefinedName definedName;
		string oldValue;
		string newValue;
		#endregion
		public DefinedCommentChangeCommentHistoryItem(DefinedName definedName, string oldComment, string newComment)
			: base(definedName.Workbook) {
			this.oldValue = oldComment;
			this.newValue = newComment;
			this.definedName = definedName;
		}
		protected override void UndoCore() {
			definedName.SetCommentCore(oldValue);
		}
		protected override void RedoCore() {
			definedName.SetCommentCore(newValue);
		}
	}
	#endregion
	#region DefinedIsHiddenChangeIsHiddenHistoryItem
	public class DefinedIsHiddenChangeIsHiddenHistoryItem : HistoryItem {
		#region Fields
		readonly DefinedName definedName;
		bool oldValue;
		bool newValue;
		#endregion
		public DefinedIsHiddenChangeIsHiddenHistoryItem(DefinedName definedName, bool oldIsHidden, bool newIsHidden)
			: base(definedName.Workbook) {
			this.definedName = definedName;
			this.oldValue = oldIsHidden;
			this.newValue = newIsHidden;
		}
		protected override void UndoCore() {
			definedName.SetIsHiddenCore(oldValue);
		}
		protected override void RedoCore() {
			definedName.SetIsHiddenCore(newValue);
		}
	}
	#endregion
	#region DefinedNameExpressionChangedHistoryItem
	public class DefinedNameExpressionChangedHistoryItem : HistoryItem {
		#region Fields
		readonly DefinedNameBase definedName;
		readonly ParsedExpression oldExpression;
		readonly ParsedExpression newExpression;
		#endregion
		public DefinedNameExpressionChangedHistoryItem(DefinedNameBase definedName, ParsedExpression oldExpression, ParsedExpression newExpression)
			: base(definedName.Workbook.DataContext.Workbook) {
			this.definedName = definedName;
			this.oldExpression = oldExpression;
			this.newExpression = newExpression;
		}
		protected override void UndoCore() {
			definedName.SetExpressionCore(oldExpression);
		}
		protected override void RedoCore() {
			definedName.SetExpressionCore(newExpression);
		}
	}
	#endregion
}
