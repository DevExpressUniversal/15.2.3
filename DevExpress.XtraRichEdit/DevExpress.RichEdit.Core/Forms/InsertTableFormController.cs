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
	#region InsertTableFormControllerParameters
	public class InsertTableFormControllerParameters : FormControllerParameters {
		readonly CreateTableParameters parameters;
		internal InsertTableFormControllerParameters(IRichEditControl control, CreateTableParameters parameters)
			: base(control) {
			Guard.ArgumentNotNull(parameters, "parameters");
			this.parameters = parameters;
		}
		internal CreateTableParameters Parameters { get { return parameters; } }
	}
	#endregion
	#region InsertTableFormController
	public class InsertTableFormController : FormController {
		readonly CreateTableParameters sourceParameters;
		int columnCount;
		int rowCount;
		public InsertTableFormController(InsertTableFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceParameters = controllerParameters.Parameters;
			InitializeController();
		}
		public CreateTableParameters SourceParameters { get { return sourceParameters; } }
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
		public int RowCount { get { return rowCount; } set { rowCount = value; } }
		protected internal virtual void InitializeController() {
			ColumnCount = SourceParameters.ColumnCount;
			RowCount = SourceParameters.RowCount;
		}
		public override void ApplyChanges() {
			SourceParameters.ColumnCount = ColumnCount;
			SourceParameters.RowCount = RowCount;
		}
	}
	#endregion
}
