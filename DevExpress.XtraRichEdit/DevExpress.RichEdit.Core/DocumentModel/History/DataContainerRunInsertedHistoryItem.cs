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
using System.Linq;
using System.Text;
using DevExpress.Office.History;
namespace DevExpress.XtraRichEdit.Model.History {
	public class DataContainerRunInsertedHistoryItem : TextRunInsertedBaseHistoryItem {
		int notificationId;
		public DataContainerRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public IDataContainer DataContainer { get; set; }
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			DataContainerRun newRun = new DataContainerRun(runs[RunIndex].Paragraph);
			newRun.DataContainer = DataContainer;
			newRun.StartIndex = StartIndex;
			runs.Insert(RunIndex, newRun);
			DocumentModel.ResetMerging();
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
			runs[RunIndex].AfterRunInserted();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (DataContainer != null) {
#if !SL
						IDisposable disposable = DataContainer as IDisposable;
						if (disposable != null)
							disposable.Dispose();
#endif
						DataContainer = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
}
