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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region RichEditDocumentHistory
	public class RichEditDocumentHistory : DocumentHistory {
		RichEditCompositeHistoryItem initialSyntaxHighlightTransaction;
		public RichEditDocumentHistory(DocumentModel documentModel)
			: base(documentModel) {
		}
		new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public override HistoryItem BeginSyntaxHighlight() {
			if (TransactionLevel > 0) {
				RichEditCompositeHistoryItem transaction = Transaction as RichEditCompositeHistoryItem;
				if (transaction != null && transaction.SyntaxHighlightTransaction != null)
					Exceptions.ThrowInternalException();
				SyntaxHighlightHistoryItem syntaxHighlightTransaction = Transaction as SyntaxHighlightHistoryItem;
				if (syntaxHighlightTransaction == null)
					SetTransaction(new SyntaxHighlightHistoryItem(DocumentModel, transaction, TransactionLevel));
			}
			else {
				HistoryItem current = Current;
				if (current == null) {
					if (initialSyntaxHighlightTransaction == null)
						SetTransaction(new SyntaxHighlightHistoryItem(DocumentModel, null, TransactionLevel));
					else
						SetTransaction(initialSyntaxHighlightTransaction);
				}
				else {
					RichEditCompositeHistoryItem compositeHistoryItem = current as RichEditCompositeHistoryItem;
					if (compositeHistoryItem == null) {
						compositeHistoryItem = (RichEditCompositeHistoryItem)CreateCompositeHistoryItem();
						compositeHistoryItem.AddItem(Current);
						Items[CurrentIndex] = compositeHistoryItem;
					}
					if (compositeHistoryItem.SyntaxHighlightTransaction == null)
						compositeHistoryItem.SyntaxHighlightTransaction = new SyntaxHighlightHistoryItem(DocumentModel, compositeHistoryItem, TransactionLevel);
					SetTransaction(compositeHistoryItem.SyntaxHighlightTransaction);
				}
			}
			SetTransactionLevel(TransactionLevel + 1);
			return Transaction;
		}
		public override void EndSyntaxHighlight() {
			SetTransactionLevel(TransactionLevel - 1);
			SyntaxHighlightHistoryItem syntaxHighlightTransaction = Transaction as SyntaxHighlightHistoryItem;
			if (syntaxHighlightTransaction == null)
				Exceptions.ThrowInternalException();
			if (syntaxHighlightTransaction.StartTransactionLevel != TransactionLevel)
				return;
			if (syntaxHighlightTransaction.Parent == null) {
				if (syntaxHighlightTransaction.Items.Count > 0)
					initialSyntaxHighlightTransaction = syntaxHighlightTransaction;
				SetTransaction(null);
			}
			else {
				if (syntaxHighlightTransaction.Items.Count > 0)
					syntaxHighlightTransaction.Parent.SyntaxHighlightTransaction = syntaxHighlightTransaction;
				if (TransactionLevel != 0) {
					SetTransaction(syntaxHighlightTransaction.Parent);
					if (Transaction.Count > 0) {
						Transaction.Items.AddRange(syntaxHighlightTransaction.Items);
						RichEditCompositeHistoryItem transaction = Transaction as RichEditCompositeHistoryItem;
						if (transaction != null)
							transaction.SyntaxHighlightTransaction = null;
					}
				}
			}
		}
		protected override void OnEndUndoCore() {
			if (!DocumentModel.ValidateActivePieceTable())
				DocumentModel.SetActivePieceTable(DocumentModel.MainPieceTable, null);
		}
		protected override HistoryItem CommitAsSingleItemCore(HistoryItem singleItem) {
			RichEditCompositeHistoryItem transaction = Transaction as RichEditCompositeHistoryItem;
			if (transaction != null) {
				if (transaction.SyntaxHighlightTransaction != null)
					return null;
			}
			if (singleItem is TextRunAppendTextHistoryItem) {
				Transaction.Items.RemoveAt(0);
				SetModifiedTextAppended(false);
				return Transaction;
			}
			return base.CommitAsSingleItemCore(singleItem);
		}
		public override void Add(HistoryItem item) {
			if (TransactionLevel != 0) {
				RichEditCompositeHistoryItem transaction = Transaction as RichEditCompositeHistoryItem;
				if (transaction != null) {
					Debug.Assert(transaction.SyntaxHighlightTransaction == null);
				}
			}
			base.Add(item);
		}
		protected override CompositeHistoryItem CreateCompositeHistoryItem() {
			return new RichEditCompositeHistoryItem(DocumentModel);
		}
		protected override void UndoCore() {
			DocumentModel.DeferredChanges.SuppressSyntaxHighlight = true;
			base.UndoCore();
		}
		protected override void RedoCore() {
			DocumentModel.DeferredChanges.SuppressSyntaxHighlight = true;
			base.RedoCore();
		}
		public virtual void AddRangeTextAppendedHistoryItem(TextRunAppendTextHistoryItem item) {
			if (TransactionLevel != 0)
				Transaction.AddItem(item);
			else
				SetModifiedTextAppended(true);
		}
	}
	#endregion
}
