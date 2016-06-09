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

using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PivotTableRemoveCommand : SpreadsheetModelCommand {
		readonly PivotTable pivotTable;
		public PivotTableRemoveCommand(PivotTable pivotTable)
			: base(pivotTable.Worksheet) {
			this.pivotTable = pivotTable;
			ShouldClearHistory = true;
		}
		public bool ShouldClearHistory { get; set; }
		protected internal override void ExecuteCore() {
			Worksheet.PivotTables.Remove(pivotTable);
			bool cacheRemoved = DocumentModel.PivotCaches.RemoveCacheIfIsNotReferenced(pivotTable.Cache);
			if (cacheRemoved && ShouldClearHistory)
				DocumentModel.ApplyChanges(DocumentModelChangeActions.ClearHistory); 
			DocumentModel.ApplyChanges(DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility);
		}
	}
}
