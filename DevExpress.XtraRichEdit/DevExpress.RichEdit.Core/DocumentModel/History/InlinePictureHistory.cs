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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Model.History {
	#region InlinePictureRunInsertedHistoryItem
	public class InlinePictureRunInsertedHistoryItem : TextRunInsertedBaseHistoryItem {
		#region Fields
		OfficeImage image;
		int notificationId;
		#endregion
		public InlinePictureRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public OfficeImage Image { get { return image; } set { image = value; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (image != null) {
#if !SL
#endif
						image = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected virtual InlinePictureRun CreateInlinePictureRun(Paragraph paragraph, OfficeImage image) {
			return new InlinePictureRun(paragraph, image);
		}
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			InlinePictureRun newRun = CreateInlinePictureRun(runs[RunIndex].Paragraph, this.image);
			newRun.StartIndex = StartIndex;
			runs.Insert(RunIndex, newRun);
			DocumentModel.ResetMerging();
			if(notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
			AfterInsertRun(DocumentModel, newRun, RunIndex);
			runs[RunIndex].AfterRunInserted();
		}
		protected virtual void AfterInsertRun(DocumentModel documentModel, TextRunBase run, RunIndex runIndex) {
			LastInsertedInlinePictureRunInfo lastInsertedRunInfo = PieceTable.LastInsertedInlinePictureRunInfo;
			lastInsertedRunInfo.Run = (InlinePictureRun)run;
			lastInsertedRunInfo.HistoryItem = this;
			lastInsertedRunInfo.RunIndex = runIndex;
		}
	}
	#endregion
}
