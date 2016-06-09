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
namespace DevExpress.Utils {
	#region IBatchUpdateable interface
	public interface IBatchUpdateable {
		void BeginUpdate();
		void EndUpdate();
		void CancelUpdate();
		bool IsUpdateLocked { get; }
		BatchUpdateHelper BatchUpdateHelper { get; }
	}
	#endregion
	#region IBatchUpdateHandler
	public interface IBatchUpdateHandler {
		void OnFirstBeginUpdate();
		void OnBeginUpdate();
		void OnEndUpdate();
		void OnLastEndUpdate();
		void OnCancelUpdate();
		void OnLastCancelUpdate();
	}
	#endregion
	#region BatchUpdateHelper
	public class BatchUpdateHelper {
		#region Fields
		IBatchUpdateHandler batchUpdateHandler;
		int suspendUpdateCount;
		bool overlappedTransaction;
		#endregion
		public BatchUpdateHelper(IBatchUpdateHandler batchUpdateHandler) {
			if (batchUpdateHandler == null)
				throw new ArgumentException("batchUpdateHandler", "batchUpdateHandler");
			this.batchUpdateHandler = batchUpdateHandler;
		}
		#region Properties
		public bool IsUpdateLocked { get { return suspendUpdateCount > 0; } }
		public int SuspendUpdateCount { get { return suspendUpdateCount; } }
		public IBatchUpdateHandler BatchUpdateHandler { get { return batchUpdateHandler; } set { batchUpdateHandler = value; } }
		public bool OverlappedTransaction { get { return overlappedTransaction; } }
		#endregion
		public void BeginUpdate() {
			if (overlappedTransaction)
				return;
			if (!IsUpdateLocked) {
				overlappedTransaction = true;
				try {
					batchUpdateHandler.OnFirstBeginUpdate();
				}
				finally {
					overlappedTransaction = false;
				}
			}
			batchUpdateHandler.OnBeginUpdate();
			this.suspendUpdateCount++;
		}
		public void EndUpdate() {
			if (overlappedTransaction)
				return;
			if (IsUpdateLocked) {
				batchUpdateHandler.OnEndUpdate();
				this.suspendUpdateCount--;
				if (!IsUpdateLocked) {
					overlappedTransaction = true;
					try {
						batchUpdateHandler.OnLastEndUpdate();
					}
					finally {
						overlappedTransaction = false;
					}
				}
			}
		}
		public void CancelUpdate() {
			if (overlappedTransaction)
				return;
			if (IsUpdateLocked) {
				batchUpdateHandler.OnCancelUpdate();
				this.suspendUpdateCount--;
				if (!IsUpdateLocked) {
					overlappedTransaction = true;
					try {
						batchUpdateHandler.OnLastCancelUpdate();
					}
					finally {
						overlappedTransaction = false;
					}
				}
			}
		}
	}
	#endregion
}
