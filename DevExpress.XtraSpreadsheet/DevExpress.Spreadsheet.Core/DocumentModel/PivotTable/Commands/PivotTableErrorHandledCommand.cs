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

namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotTableErrorHandledCommand
	public abstract class PivotTableErrorHandledCommand : ErrorHandledWorksheetCommand {
		readonly PivotTable pivotTable;
		protected PivotTableErrorHandledCommand(PivotTable pivotTable, IErrorHandler errorHandler)
			: base(pivotTable.Worksheet, errorHandler) {
			this.pivotTable = pivotTable;
		}
		protected PivotTable PivotTable { get { return pivotTable; } }
	}
	#endregion
	#region PivotTableTransactionCommand
	public abstract class PivotTableTransactionCommand : PivotTableErrorHandledCommand {
		#region Fields
		readonly IPivotTableTransaction transaction;
		#endregion
		protected PivotTableTransactionCommand(IPivotTableTransaction transaction)
			: base(transaction.PivotTable, transaction.ErrorHandler) {
				this.transaction = transaction;
		}
		#region Properties
		public IPivotTableTransaction Transaction { get { return transaction; } }
		#endregion
	}
	#endregion
	#region PivotCacheErrorHandledCommand
	public abstract class PivotCacheErrorHandledCommand : ErrorHandledWorksheetCommand {
		readonly PivotCache pivotCache;
		protected PivotCacheErrorHandledCommand(IErrorHandler errorHandler, PivotCache pivotCache)
			: base(pivotCache.DocumentModel.ActiveSheet, errorHandler) {
			this.pivotCache = pivotCache;
		}
		protected PivotCache PivotCache { get { return pivotCache; } }
	}
	#endregion
	#region PivotCacheCollectionErrorHandledCommand
	public abstract class PivotCacheCollectionErrorHandledCommand : ErrorHandledWorksheetCommand {
		readonly PivotCacheCollection pivotCaches;
		protected PivotCacheCollectionErrorHandledCommand(PivotCacheCollection pivotCaches, IErrorHandler errorHandler)
			: base(pivotCaches.DocumentModelPart, errorHandler) {
			this.pivotCaches = pivotCaches;
		}
		protected PivotCacheCollection PivotCaches { get { return pivotCaches; } }
	}
	#endregion
	#region PivotTableTransactedCommand
	public abstract class PivotTableTransactedCommand : PivotTableErrorHandledCommand {
		protected PivotTableTransactedCommand(PivotTable pivotTable, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
		}
		protected internal override void BeginExecute() {
			PivotTable.BeginTransaction(ErrorHandler);
		}
		protected internal override void EndExecute() {
			PivotTable.EndTransaction();
		}
	}
	#endregion
}
