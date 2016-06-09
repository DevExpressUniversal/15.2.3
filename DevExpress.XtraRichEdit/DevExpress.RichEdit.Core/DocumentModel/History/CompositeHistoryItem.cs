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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.History;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Model.History {
	public class RichEditCompositeHistoryItem : CompositeHistoryItem {
		public RichEditCompositeHistoryItem(DocumentModel documentModel)
			: base(documentModel.MainPieceTable) {
		}
		SyntaxHighlightHistoryItem syntaxHighlightTransaction;
		public SyntaxHighlightHistoryItem SyntaxHighlightTransaction { get { return syntaxHighlightTransaction; } set { syntaxHighlightTransaction = value; } }
		protected override void UndoCore() {
			if (syntaxHighlightTransaction != null)
				syntaxHighlightTransaction.Undo();
			base.UndoCore();
		}
		protected override void RedoCore() {
			base.RedoCore();
			if (syntaxHighlightTransaction != null)
				syntaxHighlightTransaction.Redo();
		}
	}
	#region SyntaxHighlightHistoryItem
	public class SyntaxHighlightHistoryItem : RichEditCompositeHistoryItem {
		readonly RichEditCompositeHistoryItem parent;
		readonly int startTransactionLevel;
		public SyntaxHighlightHistoryItem(DocumentModel documentModel, RichEditCompositeHistoryItem parent, int startTransactionLevel)
			: base(documentModel) {
			this.parent = parent;
			this.startTransactionLevel = startTransactionLevel;
		}
		public RichEditCompositeHistoryItem Parent { get { return parent; } }
		public int StartTransactionLevel { get { return startTransactionLevel; } }
	}
	#endregion
}
