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
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model {
	#region SelectionBasedOperation (abstract class)
	public abstract class SelectionBasedOperation {
		readonly PieceTable pieceTable;
		protected SelectionBasedOperation(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		protected internal abstract bool ShouldProcessRunParent(RunInfo info);
		protected internal abstract bool ShouldProcessContentInSameParent(RunInfo info);
		protected internal abstract void ProcessRunParent(RunInfo info, bool documentLastParagraphSelected);
		protected internal abstract void ProcessContentInsideParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected);
		protected internal virtual bool ProcessContentSameParent(RunInfo info, bool allowMergeWithNextParagraph, bool documentLastParagraphSelected) {
			if (ShouldProcessRunParent(info)) {
				ProcessRunParent(info, documentLastParagraphSelected);
				return true;
			}
			else {
				ProcessContentInsideParent(info, allowMergeWithNextParagraph, documentLastParagraphSelected);
				return false;
			}
		}
		protected internal virtual void ProcessContentCrossParent(RunInfo info, bool documentLastParagraphSelected) {
			int sectionCount = ProcessHead(info, documentLastParagraphSelected);
			if (ProcessMiddle(info, sectionCount, documentLastParagraphSelected))
				ProcessTail(info, documentLastParagraphSelected);
		}
		protected internal abstract int ProcessHead(RunInfo info, bool documentLastParagraphSelected); 
		protected internal abstract bool ProcessMiddle(RunInfo info, int paragraphCount, bool documentLastParagraphSelected); 
		protected internal abstract int ProcessTail(RunInfo info, bool documentLastParagraphSelected); 
		public virtual bool Execute(DocumentLogPosition startLogPosition, int length, bool documentLastParagraphSelected) {
			if (length <= 0)
				return false;
			DocumentModel.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
					bool isSelectionChanged = DocumentModel.Selection.IsSelectionChanged;
					try {
						BeforeExecute();
						RunInfo info = PieceTable.ObtainAffectedRunInfo(startLogPosition, length);
						bool result = ExecuteCore(info, documentLastParagraphSelected);
						AfterExecute();
						return result;
					}
					finally {
						if (!isSelectionChanged)
							DocumentModel.Selection.IsSelectionChanged = false;
					}
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public virtual bool ExecuteCore(RunInfo info, bool documentLastParagraphSelected) {
			if (ShouldProcessContentInSameParent(info)) {
				return ProcessContentSameParent(info, true, documentLastParagraphSelected);
			}
			else {
				ProcessContentCrossParent(info, documentLastParagraphSelected);
				return true;
			}
		}
		protected internal abstract void BeforeExecute();
		protected internal abstract void AfterExecute();
	}
	#endregion
}
