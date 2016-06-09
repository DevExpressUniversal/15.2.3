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
namespace DevExpress.XtraRichEdit.Forms {
	#region InsertDeleteTableCellsFormControllerParameters (abstract class)
	public abstract class InsertDeleteTableCellsFormControllerParameters : FormControllerParameters {
		readonly TableCellsParameters cellsParameters;
		internal InsertDeleteTableCellsFormControllerParameters(IRichEditControl control, TableCellsParameters cellsParameters)
			: base(control) {
			Guard.ArgumentNotNull(cellsParameters, "cellsParameters");
			this.cellsParameters = cellsParameters;
		}
		internal TableCellsParameters CellsParameters { get { return cellsParameters; } }
	}
	#endregion
	#region InsertTableCellsFormControllerParameters
	public class InsertTableCellsFormControllerParameters : InsertDeleteTableCellsFormControllerParameters {
		internal InsertTableCellsFormControllerParameters(IRichEditControl control, TableCellsParameters cellsParameters)
			: base(control, cellsParameters) {
		}
	}
	#endregion
	#region DeleteTableCellsFormControllerParameters
	public class DeleteTableCellsFormControllerParameters : InsertDeleteTableCellsFormControllerParameters {
		internal DeleteTableCellsFormControllerParameters(IRichEditControl control, TableCellsParameters cellsParameters)
			: base(control, cellsParameters) {
		}
	}
	#endregion
	#region InsertDeleteTableCellsFormController (abstract class)
	public abstract class InsertDeleteTableCellsFormController : FormController {
		#region Fields
		TableCellsParameters sourceParameters;
		TableCellOperation cellOperations;
		#endregion
		protected InsertDeleteTableCellsFormController(InsertDeleteTableCellsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceParameters = controllerParameters.CellsParameters;
			InitializeController();
		}
		#region Properties
		public TableCellsParameters SourceParameters { get { return sourceParameters; } }
		public TableCellOperation CellOperation { get { return cellOperations; } set { cellOperations = value; } }
		#endregion
		protected internal virtual void InitializeController() {
			CellOperation = sourceParameters.CellOperation;
		}
		public override void ApplyChanges() {
			SourceParameters.CellOperation = CellOperation;
		}
	}
	#endregion
	#region InsertTableCellsFormController
	public class InsertTableCellsFormController : InsertDeleteTableCellsFormController {
		public InsertTableCellsFormController(InsertTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
		}
	}
	#endregion
	#region DeleteTableCellsFormController
	public class DeleteTableCellsFormController : InsertDeleteTableCellsFormController {
		public DeleteTableCellsFormController(DeleteTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
		}
	}
	#endregion
}
