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

using DevExpress.Office.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ApplyPivotTableSortCommand : PivotTableTransactedCommand {
		readonly PivotTableSortTypeField sortType;
		readonly int fieldIndex;
		public ApplyPivotTableSortCommand(PivotTable pivotTable, int fieldIndex, PivotTableSortTypeField sortType, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.sortType = sortType;
			this.fieldIndex = fieldIndex;
		}
		public ApplyPivotTableSortCommand(PivotTable pivotTable, bool descending, int fieldIndex, IErrorHandler errorHandler)
			: this(pivotTable, fieldIndex, descending ? PivotTableSortTypeField.Descending : PivotTableSortTypeField.Ascending, errorHandler) {
		}
		protected internal override bool Validate() {
			if (fieldIndex < 0)
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotCanNotDetermineField)))
					return false;
			return base.Validate();
		}
		protected internal override void ExecuteCore() {
			PivotField pivotField = PivotTable.Fields[fieldIndex];
			pivotField.SortType = sortType;
		}
	}
}
