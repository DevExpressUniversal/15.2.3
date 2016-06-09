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

using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl {
		#region Fields
		InnerDataValidationInplaceEditor innerDataValidationInplaceEditor;
		#endregion
		#region Properties
		public InnerDataValidationInplaceEditor InnerDataValidationInplaceEditor { get { return innerDataValidationInplaceEditor; } }
		public bool IsDataValidationInplaceEditorActive { get { return innerDataValidationInplaceEditor != null ? innerDataValidationInplaceEditor.IsActive : false; } }
		#endregion
		internal IDataValidationInplaceEditor CreateDataValidationInplaceEditor() {
			return Owner.CreateDataValidationInplaceEditor();
		}
		internal void ActivateDataValidationInplaceEditor(Rectangle bounds, DataValidationInplaceValueStorage allowedValuesStorage) {
			InnerDataValidationInplaceEditor.Activate(bounds, allowedValuesStorage);
		}
		internal void DeactivateDataValidationInplaceEditor() {
			DataValidationCloseEditorCommand command = new DataValidationCloseEditorCommand(this.Owner);
			command.Execute();
		}
	}
}
