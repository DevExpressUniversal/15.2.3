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
	#region PivotClearFiltersCommand
	public class PivotClearFiltersCommand : PivotTableTransactedCommand {
		readonly bool shouldUnHideItems;
		public PivotClearFiltersCommand(PivotTable pivotTable, bool shouldUnHideItems, IErrorHandler errorHandler)
			: base(pivotTable, errorHandler) {
			this.shouldUnHideItems = shouldUnHideItems;
		}
		protected internal override bool Validate() {
			return PivotTable.Filters.Count > 0 || (shouldUnHideItems && PivotTable.HasHiddenItems());
		}
		protected internal override void ExecuteCore() {
			PivotTable.Filters.Clear();
			ClearFieldsProperties();
			PivotTable.CalculationInfo.InvalidateCalculatedCache();
		}
		void ClearFieldsProperties() {
			foreach (PivotField field in PivotTable.Fields) {
				field.MeasureFilter = false;
				if (shouldUnHideItems)
					field.UnHideItems(ErrorHandler);
			}
		}
	}
	#endregion
	#region TurnOffPivotMultipleFiltersCommand
	public class TurnOffPivotMultipleFiltersCommand : PivotClearFiltersCommand {
		public TurnOffPivotMultipleFiltersCommand(PivotTable pivotTable, IErrorHandler errorHandler) 
			: base(pivotTable, true, errorHandler) { }
		bool TurnedOn { get { return PivotTable.MultipleFieldFilters; } }
		protected internal override bool Validate() {
			return TurnedOn;
		}
		protected internal override void ExecuteCore() {
			if (TurnedOn && HasDifferentFilters())
				base.ExecuteCore();
			PivotTable.MultipleFieldFilters = false;
		}
		bool HasDifferentFilters() {
			if (PivotTable.HasHiddenItems() && PivotTable.Filters.Count > 0)
				return true;
			List<int> fieldIndexes = new List<int>();
			foreach (PivotFilter filter in PivotTable.Filters) {
				int fieldIndex = filter.FieldIndex;
				if (fieldIndexes.Contains(fieldIndex))
					return true;
				fieldIndexes.Add(fieldIndex);
			}
			return false;
		}
	}
	#endregion
}
