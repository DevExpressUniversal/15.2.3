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
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Model.History {
	#region InsertRangePermissionHistoryItem
	public class InsertRangePermissionHistoryItem : InsertDocumentIntervalHistoryItem {
		public InsertRangePermissionHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void RedoCore() {
			DocumentLogPosition start = Algorithms.Min(Position, PieceTable.DocumentEndLogPosition);
			DocumentLogPosition end = Algorithms.Min(Position + Length, PieceTable.DocumentEndLogPosition);
			RunInfo runInfo = PieceTable.ApplyDocumentPermissionCore(start, end, IndexToInsert);
			RunIndex startRunIndex = runInfo != null ? runInfo.NormalizedStart.RunIndex : RunIndex.DontCare;
			RunIndex endRunIndex = runInfo != null ? runInfo.NormalizedEnd.RunIndex : RunIndex.DontCare;
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
		}
		protected override void UndoCore() {
			DocumentLogPosition start = Algorithms.Min(Position, PieceTable.DocumentEndLogPosition);
			DocumentLogPosition end = Algorithms.Min(Position + Length, PieceTable.DocumentEndLogPosition);
			RunInfo runInfo = PieceTable.RemoveDocumentPermissionCore(start, end, IndexToInsert);
			if (runInfo == null)
				return;
			RunIndex startRunIndex = runInfo != null ? runInfo.NormalizedStart.RunIndex : RunIndex.DontCare;
			RunIndex endRunIndex = runInfo != null ? runInfo.NormalizedEnd.RunIndex : RunIndex.DontCare;
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
		}
	}
	#endregion
	#region RemoveRangePermissionHistoryItem
	public class RemoveRangePermissionHistoryItem : InsertDocumentIntervalHistoryItem {
		public RemoveRangePermissionHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override void RedoCore() {
			DocumentLogPosition start = Algorithms.Min(Position, PieceTable.DocumentEndLogPosition);
			DocumentLogPosition end = Algorithms.Min(Position + Length, PieceTable.DocumentEndLogPosition);
			RunInfo runInfo = PieceTable.RemoveDocumentPermissionCore(start, end, IndexToInsert);
			RunIndex startRunIndex = runInfo != null ? runInfo.NormalizedStart.RunIndex : RunIndex.DontCare;
			RunIndex endRunIndex = runInfo != null ? runInfo.NormalizedEnd.RunIndex : RunIndex.DontCare;
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
		}
		protected override void UndoCore() {
			DocumentLogPosition start = Algorithms.Min(Position, PieceTable.DocumentEndLogPosition);
			DocumentLogPosition end = Algorithms.Min(Position + Length, PieceTable.DocumentEndLogPosition);
			RunInfo runInfo = PieceTable.ApplyDocumentPermissionCore(start, end, IndexToInsert);
			RunIndex startRunIndex = runInfo != null ? runInfo.NormalizedStart.RunIndex : RunIndex.DontCare;
			RunIndex endRunIndex = runInfo != null ? runInfo.NormalizedEnd.RunIndex : RunIndex.DontCare;
			PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw, startRunIndex, endRunIndex);
		}
	}
	#endregion
	#region DeleteRangePermissionHistoryItem
	public class DeleteRangePermissionHistoryItem : RichEditHistoryItem {
		#region Fields
		RangePermission deletedRangePermission;
		#endregion
		public DeleteRangePermissionHistoryItem(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		public RangePermission DeletedRangePermission { get { return deletedRangePermission; } set { deletedRangePermission = value; } }
		#endregion
		protected override void RedoCore() {
			RangePermissionCollection rangePermissions = PieceTable.RangePermissions;
			int count = rangePermissions.Count;
			for (int i = 0; i < count; i++) {
				RangePermission permission = rangePermissions[i];
				if (permission.Properties.Index == deletedRangePermission.Properties.Index && permission.NormalizedStart == deletedRangePermission.NormalizedStart && permission.NormalizedEnd == deletedRangePermission.NormalizedEnd) {
					rangePermissions.RemoveAt(i);
					break;
				}
			}
			DocumentModelChangeActions changeActions = DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw;
			PieceTable.ApplyChangesCore(changeActions, deletedRangePermission.Interval.NormalizedStart.RunIndex, deletedRangePermission.Interval.NormalizedEnd.RunIndex);
		}
		protected override void UndoCore() {
			PieceTable.RangePermissions.Add(this.deletedRangePermission);
			DocumentModelChangeActions changeActions = DocumentModelChangeActions.RaiseContentChanged | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.Redraw;
			PieceTable.ApplyChangesCore(changeActions, deletedRangePermission.Interval.NormalizedStart.RunIndex, deletedRangePermission.Interval.NormalizedEnd.RunIndex);
		}
	}
	#endregion
}
