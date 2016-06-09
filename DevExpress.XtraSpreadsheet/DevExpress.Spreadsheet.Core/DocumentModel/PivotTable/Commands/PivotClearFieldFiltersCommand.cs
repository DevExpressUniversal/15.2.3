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
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotFilterClearType
	public enum PivotFilterClearType {
		Label,
		Value,
		All,
		AllExceptManual,
	}
	#endregion
	#region PivotClearFieldFiltersCommand
	public class PivotClearFieldFiltersCommand : PivotTableTransactedCommand {
		#region Fields
		readonly List<int> filterIndexesToClear;
		readonly PivotFilterClearType clearType;
		readonly int fieldIndex;
		#endregion
		public PivotClearFieldFiltersCommand(PivotTable pivotTable, int fieldIndex, PivotFilterClearType clearType, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.filterIndexesToClear = new List<int>();
			this.fieldIndex = fieldIndex;
			this.clearType = clearType;
			PopulateList();
		}
		PivotFilterCollection Filters { get { return PivotTable.Filters; } }
		bool ShouldUnHideItems { get { return clearType == PivotFilterClearType.All; } }
		protected internal override bool Validate() {
			return filterIndexesToClear.Count > 0 || (ShouldUnHideItems && PivotTable.FieldHasHiddenItems(fieldIndex));
		}
		void PopulateList() {
			for (int i = 0; i < Filters.Count; i++) {
				PivotFilter filter = Filters[i];
				if (filter.FieldIndex == fieldIndex && FilterTypeIsCorrect(filter))
					filterIndexesToClear.Add(i);
			}
		}
		protected internal override void ExecuteCore() {
			for (int i = filterIndexesToClear.Count - 1; i >= 0; i--) {
				PivotFilter filter = Filters[filterIndexesToClear[i]];
				if (filter.IsMeasureFilter)
					PivotTable.Fields[filter.FieldIndex].MeasureFilter = false;
				Filters.Remove(filter);
			}
			if (ShouldUnHideItems)
				PivotTable.Fields[fieldIndex].UnHideItems(ErrorHandler);
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		bool FilterTypeIsCorrect(PivotFilter filter) {
			if (clearType == PivotFilterClearType.Label)
				return filter.IsLabelFilter;
			else if (clearType == PivotFilterClearType.Value)
				return filter.IsMeasureFilter;
			return true;
		}
	}
	#endregion
}
