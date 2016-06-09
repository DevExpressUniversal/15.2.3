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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class PivotFieldRenameCommand : PivotTableTransactedCommand {
		readonly PivotField field;
		readonly string newName;
		public PivotFieldRenameCommand(PivotField field, string newName, IErrorHandler errorHandler)
			: base(field.PivotTable, errorHandler) {
			this.field = field;
			this.newName = newName;
		}
		bool ShouldSetName { get; set; }
		protected internal override bool Validate() {
			if (string.IsNullOrEmpty(newName))
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldNameIsInvalid));
			PivotTable table = field.PivotTable;
			PivotFieldCollection fields = table.Fields;
			for (int i = 0; i < fields.Count; i++) {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(table.GetFieldCaption(i), newName) == 0) {
					if (!Object.ReferenceEquals(fields[i], field))
						return HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldNameAlreadyExists));
					return true;
				}
			}
			ShouldSetName = true;
			return true;
		}
		protected internal override void ExecuteCore() {
			if (ShouldSetName)
				field.Name = newName;
		}
	}
	public class PivotDataFieldRenameCommand : PivotTableTransactedCommand {
		readonly PivotDataField dataField;
		readonly string newName;
		public PivotDataFieldRenameCommand(PivotDataField dataField, string newName, IErrorHandler errorHandler)
			: base(dataField.PivotTable, errorHandler) {
			this.dataField = dataField;
			this.newName = newName;
		}
		bool ShouldSetName { get; set; }
		protected internal override bool Validate() {
			if (string.IsNullOrEmpty(newName))
				return HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldNameIsInvalid));
			PivotFieldCollection fields = PivotTable.Fields;
			for (int i = 0; i < fields.Count; i++)
				if (StringExtensions.CompareInvariantCultureIgnoreCase(PivotTable.GetFieldCaption(i), newName) == 0)
					return HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldNameAlreadyExists));
			PivotDataFieldCollection dataFields = PivotTable.DataFields;
			for (int i = 0; i < dataFields.Count; i++) {
				PivotDataField currentDataField = dataFields[i];
				if (StringExtensions.CompareInvariantCultureIgnoreCase(currentDataField.Name, newName) == 0) {
					if (!Object.ReferenceEquals(currentDataField, dataField))
						return HandleError(new ModelErrorInfo(ModelErrorType.PivotFieldNameAlreadyExists));
					return true;
				}
			}
			ShouldSetName = true;
			return true;
		}
		protected internal override void ExecuteCore() {
			if (ShouldSetName)
				dataField.Name = newName;
		}
	}
}
