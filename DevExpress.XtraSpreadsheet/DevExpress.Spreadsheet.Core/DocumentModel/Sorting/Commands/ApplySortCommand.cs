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
namespace DevExpress.XtraSpreadsheet.Model {
	#region ApplySortCommand
	public class ApplySortCommand : ErrorHandledWorksheetCommand {
		#region Fields
		SortState sortState;
		#endregion
		public ApplySortCommand(SortState sortState, IErrorHandler errorHandler)
			: base(sortState.DocumentModelPart, errorHandler) {
			this.sortState = sortState;
		}
		#region Properties
		CellRange SortRange { get { return sortState.SortRange; } }
		SortConditionCollection Conditions { get { return sortState.SortConditions; } }
		#endregion
		protected internal override void ExecuteCore() {
			RangeSortEngine engine = new RangeSortEngine(Worksheet);
			List<ModelSortField> sortFields = new List<ModelSortField>();
			int count = Conditions.Count;
			for (int i = 0; i < count; i++)
				sortFields.Add(CreateSortField(Conditions[i]));
			engine.Sort(SortRange, sortFields);
		}
		ModelSortField CreateSortField(SortCondition condition) {
			ModelSortField result = new ModelSortField();
			result.Comparer = CreateComparer(condition);
			result.ColumnOffset = condition.SortReference.TopLeft.Column - SortRange.TopLeft.Column;
			return result;
		}
		IComparer<VariantValue> CreateComparer(SortCondition condition) {
			SharedStringTable sharedStringTable = DocumentModel.SharedStringTable;
			if (condition.Descending)
				return new DescendingSortVariantValueComparer(sharedStringTable);
			return new SortVariantValueComparer(sharedStringTable);
		}
		protected internal override bool Validate() {
			return SortRange != null && Conditions.Count != 0;
		}
	}
	#endregion
}
