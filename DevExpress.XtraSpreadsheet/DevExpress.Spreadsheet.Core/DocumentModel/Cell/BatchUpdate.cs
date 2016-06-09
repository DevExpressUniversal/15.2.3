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
using DevExpress.Office;
using DevExpress.Office.Localization;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Cell : IBatchUpdateable, IBatchUpdateHandler
	partial class Cell {
		#region Properties
		public bool IsUpdateLocked { get { return BatchUpdateHelper != null && BatchUpdateHelper.IsUpdateLocked; } }
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return this.BatchUpdateHelper; } }
		CellBatchUpdateHelper BatchUpdateHelper { get { return FormatIndex < 0 ? Workbook.GetCellTransaction((short)-FormatIndex) : null; } }
		#endregion
		void RegisterTransaction() {
			short originalFormatIndex = FormatIndex;
			this.FormatIndex = (short)-Workbook.RegisterCellTransaction(this);
			BatchUpdateHelper.OriginalFormatIndex = originalFormatIndex;
		}
		void UnregisterTransaction(CellBatchUpdateHelper helper) {
			if (FormatIndex < 0)
				FormatIndex = helper.OriginalFormatIndex;
			Workbook.UnregisterCellTransaction(helper.TransactionId);
		}
		public virtual DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			if (BatchUpdateHelper == null)
				RegisterTransaction();
			BatchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			BatchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			if (!(BatchUpdateHelper is CellBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			BatchUpdateHelper.CancelUpdate();
		}
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			OnFirstBeginUpdateCore();
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			OnLastEndUpdateCore();
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
			OnLastCancelUpdateCore();
		}
		protected internal virtual void OnFirstBeginUpdateCore() {
			BatchUpdateHelper.DeferredNotifications = Workbook.Cache.CellFormatCache[BatchUpdateHelper.OriginalFormatIndex].Clone();
			BatchUpdateHelper.DeferredNotifications.BeginUpdate(); 
		}
		protected internal virtual void OnLastCancelUpdateCore() {
			if (!(BatchUpdateHelper is CellBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			UnregisterTransaction(BatchUpdateHelper);
		}
		protected internal virtual void OnLastEndUpdateCore() {
			if (!(BatchUpdateHelper is CellBatchUpdateHelper))
				Exceptions.ThrowInvalidOperationException(OfficeStringId.Msg_InvalidEndUpdate);
			BatchUpdateHelper.DeferredNotifications.EndUpdate();
			CellBatchUpdateHelper helper = BatchUpdateHelper;
			if (!ReplaceInfo(BatchUpdateHelper.DeferredNotifications, GetBatchUpdateChangeActions())) {
				if (BatchUpdateHelper.FakeAssignDetected)
					NotifyFakeAssign();
			}
			UnregisterTransaction(helper);
		}
		#endregion
	}
	#endregion
	#region CellBatchUpdateHelper
	public class CellBatchUpdateHelper : BatchUpdateHelper<FormatBase> {
		readonly short transactionId;
		short originalFormatIndex;
		HistoryTransaction transaction;
		public CellBatchUpdateHelper(ICell cell, short transactionId)
			: base(cell) {
			this.transactionId = transactionId;
		}
		public short TransactionId { get { return transactionId; } }
		public short OriginalFormatIndex { get { return originalFormatIndex; } set { originalFormatIndex = value; } }
		internal HistoryTransaction Transaction { get { return transaction; } set { transaction = value; } }
	}
	#endregion
}
