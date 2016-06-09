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
#if !SL
using System.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Model.History {
	#region InlineCustomObjectRunInsertedHistoryItem
	public class InlineCustomObjectRunInsertedHistoryItem : TextRunInsertedBaseHistoryItem {
		#region Fields
		IInlineCustomObject customObject;
		float scaleX;
		float scaleY;
		int notificationId;
		#endregion
		public InlineCustomObjectRunInsertedHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public IInlineCustomObject CustomObject { get { return customObject; } set { customObject = value; } }
		public float ScaleX { get { return scaleX; } set { scaleX = value; } }
		public float ScaleY { get { return scaleY; } set { scaleY = value; } }
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (customObject != null) {
#if !SL
						IDisposable disposable = customObject as IDisposable;
						if (disposable != null)
							disposable.Dispose();
#endif
						customObject = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void UndoCore() {
			PieceTable.Runs[RunIndex].BeforeRunRemoved();
			PieceTable.Runs.RemoveAt(RunIndex);
			DocumentModel.ResetMerging();
			DocumentModelStructureChangedNotifier.NotifyRunRemoved(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
		}
		protected override void RedoCore() {
			TextRunCollection runs = PieceTable.Runs;
			InlineCustomObjectRun newRange = new InlineCustomObjectRun(runs[RunIndex].Paragraph, this.customObject);
			newRange.StartIndex = StartIndex;
			newRange.ScaleX = ScaleX;
			newRange.ScaleY = ScaleY;
			runs.Insert(RunIndex, newRange);
			DocumentModel.ResetMerging();
			if (notificationId == NotificationIdGenerator.EmptyId)
				notificationId = DocumentModel.History.GetNotificationId();
			DocumentModelStructureChangedNotifier.NotifyRunInserted(PieceTable, PieceTable, ParagraphIndex, RunIndex, 1, notificationId);
			runs[RunIndex].AfterRunInserted();
		}
	}
	#endregion
}
