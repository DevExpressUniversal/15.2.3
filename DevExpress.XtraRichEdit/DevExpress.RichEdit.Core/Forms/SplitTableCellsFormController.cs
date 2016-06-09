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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraRichEdit.Forms {
	#region SplitTableCellsFormControllerParameters
	public class SplitTableCellsFormControllerParameters : FormControllerParameters {
		readonly SplitTableCellsParameters parameters;
		internal SplitTableCellsFormControllerParameters(IRichEditControl control, SplitTableCellsParameters parameters)
			: base(control) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.parameters = parameters;
		}
		internal SplitTableCellsParameters Parameters { get { return parameters; } }
	}
	#endregion
	#region SplitTableCellsFormController
	public class SplitTableCellsFormController : FormController {
		#region Fields
		const int maxRowsCount = 15; 
		const int maxColumnsCount = 63; 
		readonly SplitTableCellsParameters sourceParameters;
		int columnsCount;
		int rowsCount;
		bool mergeCellsBeforeSplit;
		List<int> allowedRowsCount;
		#endregion
		public SplitTableCellsFormController(SplitTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceParameters = controllerParameters.Parameters;
			InitializeController();
		}
		#region Properties
		public SplitTableCellsParameters SourceParameters { get { return sourceParameters; } }
		public int ColumnsCount { get { return columnsCount; } set { columnsCount = value; } }
		public int RowsCount { get { return rowsCount; } set { rowsCount = value; } }
		public bool MergeCellsBeforeSplit { get { return mergeCellsBeforeSplit; } set { mergeCellsBeforeSplit = value; } }
		public int RowsCountAfterMerge { get { return SourceParameters.RowCountAfterMerge; } }
		public int SourceRowsCount { get { return SourceParameters.RowsCount; } }
		public List<int> AllowedRowsCount { get { return allowedRowsCount; } }
		internal int MaxRowsCount { get { return maxRowsCount; } }
		internal int MaxColumnsCount { get { return maxColumnsCount; } }
		#endregion
		protected internal virtual void InitializeController() {
			ColumnsCount = sourceParameters.ColumnsCount;
			RowsCount = sourceParameters.RowsCount;
			MergeCellsBeforeSplit = sourceParameters.MergeCellsBeforeSplit;
			allowedRowsCount = CalculateAllowedRowsCount();
		}
		List<int> CalculateAllowedRowsCount() {
			if (RowsCountAfterMerge <= 1)
				return null;
			List<int> result = new List<int>();
			result.Add(1);
			for (int i = 2; i <= RowsCountAfterMerge; i++) {
				if (RowsCountAfterMerge % i == 0)
					result.Add(i);
			}
			return result;
		}
		public override void ApplyChanges() {
			sourceParameters.ColumnsCount = ColumnsCount;
			sourceParameters.RowsCount = RowsCount;
			sourceParameters.MergeCellsBeforeSplit = MergeCellsBeforeSplit;
		}
	}
	#endregion
}
