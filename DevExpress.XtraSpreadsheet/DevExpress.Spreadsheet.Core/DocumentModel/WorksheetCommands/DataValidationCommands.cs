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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataValidationCommandBase
	public abstract class DataValidationCommandBase : ErrorHandledWorksheetCommand {
		#region Fields
		readonly CellRangeBase targetRange;
		#endregion
		protected DataValidationCommandBase(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, CellRangeBase targetRange)
			: base(documentModelPart, errorHandler) {
			this.targetRange = targetRange;
		}
		#region Properties
		public CellRangeBase TargetRange { get { return targetRange; } }
		protected DataValidationCollection Collection { get { return Worksheet.DataValidations; } }
		#endregion
		protected internal override bool Validate() {
			if (Worksheet.PivotTables.ContainsItemsInRange(targetRange, true))
				if (!HandleError(new ModelErrorInfo(ModelErrorType.PivotTableCanNotApplyDataValidation)))
					return false;
			if (!object.ReferenceEquals(Worksheet, targetRange.Worksheet))
				return HandleError(new ModelErrorInfo(ModelErrorType.ErrorUseRangeFromAnotherWorksheet));
			return true;
		}
		protected void ExcludeRange(CellRangeBase range, DataValidation sourceItem) {
			int count = Collection.Count;
			for (int i = count - 1; i >= 0; i--) {
				DataValidation item = Collection[i];
				if (!object.ReferenceEquals(item, sourceItem) && item.CellRange.Intersects(range)) {
					CellRangeBase newRange = item.CellRange.ExcludeRange(range);
					if (newRange != null)
						item.CellRange = newRange;
					else
						Collection.RemoveAt(i);
				}
			}
		}
	}
	#endregion
	#region DataValidationClearCommand
	public class DataValidationClearCommand : DataValidationCommandBase {
		public DataValidationClearCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, CellRangeBase targetRange)
			: base(documentModelPart, errorHandler, targetRange) {
		}
		protected internal override void ExecuteCore() {
			ExcludeRange(TargetRange, null);
			ClearInvalidDataCircles();
		}
		void ClearInvalidDataCircles() {
			if (Worksheet.ShowInvalidDataCircles) {
				foreach (CellKey key in TargetRange.GetAllCellKeysEnumerable())
					Worksheet.InvalidDataCircles.Remove(key);
				DocumentModel.ApplyChanges(DocumentModelChangeActions.Redraw);
			}
		}
	}
	#endregion
	#region DataValidationModifyRangeCommand
	public class DataValidationModifyRangeCommand : DataValidationCommandBase {
		#region Fields
		readonly DataValidation dataValidation;
		#endregion
		public DataValidationModifyRangeCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, DataValidation dataValidation, CellRangeBase targetRange)
			: base(documentModelPart, errorHandler, targetRange) {
			this.dataValidation = dataValidation;
		}
		protected internal override void ExecuteCore() {
			ExcludeRange(TargetRange, dataValidation);
			dataValidation.CellRange = TargetRange;
		}
	}
	#endregion
	#region DataValidationAddCommand
	public class DataValidationAddCommand : DataValidationCommandBase {
		#region Fields
		readonly DataValidationType validationType;
		readonly DataValidationOperator validationOperator;
		#endregion
		public DataValidationAddCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, CellRangeBase targetRange, DataValidationType validationType, DataValidationOperator validationOperator)
			: base(documentModelPart, errorHandler, targetRange) {
			this.validationType = validationType;
			this.validationOperator = validationOperator;
		}
		protected internal override void ExecuteCore() {
			ExcludeRange(TargetRange, null);
			DataValidation newItem = new DataValidation(TargetRange, Worksheet);
			newItem.BeginUpdate();
			try {
				newItem.Type = validationType;
				newItem.ValidationOperator = validationOperator;
				if (validationType == DataValidationType.List)
					newItem.SuppressDropDown = false;
			}
			finally {
				newItem.EndUpdate();
			}
			Collection.Add(newItem);
		}
	}
	#endregion
	#region DataValidationUIAddCommand
	public class DataValidationUIAddCommand : DataValidationCommandBase {
		readonly DataValidation newItem;
		public DataValidationUIAddCommand(IDocumentModelPart documentModelPart, IErrorHandler errorHandler, DataValidation newItem)
			: base(documentModelPart, errorHandler, newItem.CellRange) {
			this.newItem = newItem;
		}
		protected internal override void ExecuteCore() {
			ExcludeRange(TargetRange, null);
			Collection.AddOrMergeWithExisting(newItem);
		}
	}
	#endregion
}
