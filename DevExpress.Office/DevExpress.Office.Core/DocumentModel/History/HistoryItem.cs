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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using System.Diagnostics;
namespace DevExpress.Office.History {
	#region HistoryItem (abstract class)
	public abstract class HistoryItem : IDisposable {
		readonly IDocumentModelPart documentModelPart; 
		protected HistoryItem(IDocumentModelPart documentModelPart) {
			Guard.ArgumentNotNull(documentModelPart, "documentModelPart");
			this.documentModelPart = documentModelPart;
		}
		#region Properties
		public IDocumentModelPart DocumentModelPart { get { return documentModelPart; } }
		public IDocumentModel DocumentModel { get { return documentModelPart.DocumentModel; } }
		public virtual bool ChangeModified { get { return true; } }
		#endregion
		public virtual void Execute() {
			RedoCore();
		}
		public void Undo() {
#if DEBUG
			int currentIndexBefore = DocumentModel.History.CurrentIndex;
			int countBefore = DocumentModel.History.Count;
			int transactionCountBefore = 0;
			if (DocumentModel.History.Transaction != null)
				transactionCountBefore = DocumentModel.History.Transaction.Count;
#endif
			UndoCore();
#if DEBUG
			System.Diagnostics.Debug.Assert(countBefore == DocumentModel.History.Count);
			System.Diagnostics.Debug.Assert(currentIndexBefore == DocumentModel.History.CurrentIndex);
			if (DocumentModel.History.Transaction != null)
				System.Diagnostics.Debug.Assert(DocumentModel.History.IsHistoryDisabled || transactionCountBefore == DocumentModel.History.Transaction.Count);
#endif
		}
		public void Redo() {
#if DEBUG
			int currentIndexBefore = DocumentModel.History.CurrentIndex;
			int countBefore = DocumentModel.History.Count;
			int transactionCountBefore = 0;
			if (DocumentModel.History.Transaction != null)
				transactionCountBefore = DocumentModel.History.Transaction.Count;
#endif
			RedoCore();
#if DEBUG
			System.Diagnostics.Debug.Assert(countBefore == DocumentModel.History.Count);
			System.Diagnostics.Debug.Assert(currentIndexBefore == DocumentModel.History.CurrentIndex);
			if (DocumentModel.History.Transaction != null)
				System.Diagnostics.Debug.Assert(DocumentModel.History.IsHistoryDisabled || transactionCountBefore == DocumentModel.History.Transaction.Count);
#endif
		}
		protected abstract void UndoCore();
		protected abstract void RedoCore();
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
}
