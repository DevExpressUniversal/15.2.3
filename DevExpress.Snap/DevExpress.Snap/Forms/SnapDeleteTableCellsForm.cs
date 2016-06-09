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
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraEditors.Controls;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Forms {
	public class SnapDeleteTableCellsForm : DeleteTableCellsForm {
		SnapDocumentModel documentModel;
		protected SnapDeleteTableCellsForm() {			
		}
		public SnapDeleteTableCellsForm(DeleteTableCellsFormControllerParameters controllerParameters)
			: base(controllerParameters) {
		}
		protected internal SnapDocumentModel DocumentModel { get { return documentModel; } }
		protected override DeleteTableCellsFormController CreateController(DeleteTableCellsFormControllerParameters controllerParameters) {
			this.documentModel = ((SnapControl)controllerParameters.Control).DocumentModel;
			return base.CreateController(controllerParameters);
		}
		protected override void UpdateFormCore() {
			UpdateRadioGroup();
			int cellOperationIndex = (int)Controller.CellOperation;
			if (this.rgCellOperation.Properties.Items[cellOperationIndex].Enabled)
				this.rgCellOperation.SelectedIndex = cellOperationIndex;
			else {
				cellOperationIndex = CalculateFirstAvailableIndex();
				this.rgCellOperation.SelectedIndex = cellOperationIndex;
				Controller.CellOperation = (TableCellOperation)cellOperationIndex;
			}
		}
		void UpdateRadioGroup() {
			RadioGroupItemCollection items = this.rgCellOperation.Properties.Items;
			DocumentModel.BeginUpdate();
			try {
				items[(int)TableCellOperation.ShiftToTheHorizontally].Enabled = CanShiftToTheHorizontally();
				items[(int)TableCellOperation.ShiftToTheVertically].Enabled = CanShiftToTheVertically();
				items[(int)TableCellOperation.RowOperation].Enabled = CanDeleteRow();
				items[(int)TableCellOperation.ColumnOperation].Enabled = CanDeleteColumn();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal virtual bool CanShiftToTheHorizontally() {
			return TableCommandsHelper.CanPerformTableCellOperationWithColumn(DocumentModel);
		}
		protected internal virtual bool CanShiftToTheVertically() {
			return TableCommandsHelper.CanShiftToTheVertically(documentModel);
		}
		protected internal virtual bool CanDeleteColumn() {
			return TableCommandsHelper.CanPerformTableCellOperationWithColumn(DocumentModel);
		}
		protected internal virtual bool CanDeleteRow() {
			return TableCommandsHelper.CanPerformTableCellsOperationWithRow(DocumentModel);
		}
		int CalculateFirstAvailableIndex() {
			RadioGroupItemCollection items = this.rgCellOperation.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				if (items[i].Enabled)
					return i;
			}
			Exceptions.ThrowInternalException();
			return -1;
		}
	}
}
