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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotChangeDataSourceCommand
	public class PivotChangeDataSourceCommand : PivotCacheCreateCommand {
		readonly PivotTable pivotTable;
		public PivotChangeDataSourceCommand(PivotTable pivotTable, IPivotCacheSource newSource, IErrorHandler errorHandler)
			: base(pivotTable.DocumentModel, errorHandler, newSource) {
			this.pivotTable = pivotTable;
		}
		protected internal override void ExecuteCore() {
			PivotCache oldCache = pivotTable.Cache;
			base.ExecuteCore();
			if (oldCache.Equals(Result))
				return;
			PivotCache newCache = Result as PivotCache;
			HistoryHelper.SetValue(DocumentModel, oldCache, newCache, pivotTable.SetCacheCore);
			pivotTable.BeginTransaction(ErrorHandler);
			pivotTable.Fields.Clear();
			pivotTable.PageFields.Clear();
			pivotTable.ColumnFields.Clear();
			pivotTable.RowFields.Clear();
			pivotTable.DataFields.Clear();
			for (int i = 0; i < newCache.CacheFields.Count; ++i)
				pivotTable.Fields.Add(pivotTable.CreateField());
			if (pivotTable.EndTransaction())
				if (DocumentModel.PivotCaches.RemoveCacheIfIsNotReferenced(oldCache))
					DocumentModel.ApplyChanges(DocumentModelChangeActions.ClearHistory); 
		}
	}
	#endregion
}
