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
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model.History {
	#region FloatingObjectAnchorRunInsertedHistoryItem
	public class FloatingObjectAnchorRunInsertedHistoryItem : TextRunInsertedBaseHistoryItem {
		FloatingObjectAnchorRun newRun;
		int notificationId;
		public FloatingObjectAnchorRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
		}
		public override void Execute() {
			newRun = new FloatingObjectAnchorRun(PieceTable.Runs[RunIndex].Paragraph);
			newRun.StartIndex = StartIndex;
			base.Execute();
		}
		protected override void RedoCore() {
			PieceTable.Runs.Insert(RunIndex, newRun);
			DocumentModel.ResetMerging();
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
			AfterInsertRun(DocumentModel, newRun, RunIndex);
			PieceTable.Runs[RunIndex].AfterRunInserted();
		}
		void AfterInsertRun(DocumentModel documentModel, TextRunBase run, RunIndex runIndex) {
			LastInsertedFloatingObjectAnchorRunInfo lastInsertedRunInfo = PieceTable.LastInsertedFloatingObjectAnchorRunInfo;
			lastInsertedRunInfo.Run = (FloatingObjectAnchorRun)run;
			lastInsertedRunInfo.HistoryItem = this;
			lastInsertedRunInfo.RunIndex = runIndex;
		}
	}
	#endregion
	#region LastInsertedFloatingObjectAnchorRunInfo
	public class LastInsertedFloatingObjectAnchorRunInfo {
		#region Fields
		PieceTable pieceTable;
		RunIndex runIndex;
		FloatingObjectAnchorRun run;
		FloatingObjectAnchorRunInsertedHistoryItem historyItem;
		#endregion
		public LastInsertedFloatingObjectAnchorRunInfo() {
			Reset(null);
		}
		#region Properties
		#region Run
		public FloatingObjectAnchorRun Run {
			[System.Diagnostics.DebuggerStepThrough]
			get { return run; }
			[System.Diagnostics.DebuggerStepThrough]
			set { run = value; }
		}
		#endregion
		#region RunIndex
		public RunIndex RunIndex {
			[System.Diagnostics.DebuggerStepThrough]
			get { return runIndex; }
			[System.Diagnostics.DebuggerStepThrough]
			set { runIndex = value; }
		}
		#endregion
		#region HistoryItem
		public FloatingObjectAnchorRunInsertedHistoryItem HistoryItem {
			[System.Diagnostics.DebuggerStepThrough]
			get { return historyItem; }
			[System.Diagnostics.DebuggerStepThrough]
			set { historyItem = value; }
		}
		#endregion
		#region PieceTable
		protected internal PieceTable PieceTable {
			[System.Diagnostics.DebuggerStepThrough]
			get { return pieceTable; }
			[System.Diagnostics.DebuggerStepThrough]
			set { pieceTable = value; }
		}
		#endregion
		#endregion
		public void Reset(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.run = null;
			this.runIndex = new RunIndex(-1);
		}
	}
	#endregion
	#region FloatingObjectAnchorRunMovedHistoryItem
	public class FloatingObjectAnchorRunMovedHistoryItem : TextRunInsertedBaseHistoryItem {
		RunIndex newRunIndex;
		ParagraphIndex newParagraphIndex;
		int notificationId1;
		int notificationId2;
		public FloatingObjectAnchorRunMovedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public RunIndex NewRunIndex { get { return newRunIndex; } set { newRunIndex = value; } }
		public ParagraphIndex NewParagraphIndex { get { return newParagraphIndex; } set { newParagraphIndex = value; } }
		protected override void RedoCore() {
			TextRunBase run = PieceTable.Runs[RunIndex];
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			if (notificationId1 == NotificationIdGenerator.EmptyId) {
				notificationId1 = DocumentModel.History.GetNotificationId();
				notificationId2 = DocumentModel.History.GetNotificationId();
			}
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId1);
			RunIndex index = NewRunIndex;
			if (index >= RunIndex)
				index--;
			PieceTable.Runs.Insert(index, run);
			run.Paragraph = PieceTable.Paragraphs[NewParagraphIndex];
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, NewParagraphIndex, index, 1, notificationId2);
		}
		protected override void UndoCore() {
			RunIndex index = NewRunIndex;
			if (index >= RunIndex)
				index--;
			TextRunBase run = PieceTable.Runs[index];
			PieceTable.Runs.RemoveAt(index);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, NewParagraphIndex, index, 1, notificationId2);
			PieceTable.Runs.Insert(RunIndex, run);
			run.Paragraph = PieceTable.Paragraphs[ParagraphIndex];
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId1);
		}
	}
	#endregion
}
