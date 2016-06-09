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

using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Model;
using System;
namespace DevExpress.XtraSpreadsheet.Internal {
	public partial class InnerSpreadsheetControl {
		#region Fields
		InnerPivotTableFieldsPanel innerPivotTableFieldsPanel;
		#endregion
		internal IPivotTableFieldsPanel CreatePivotTableFieldsPanel() {
			return Owner.CreatePivotTableFieldsPanel();
		}
		internal void ResetPivotTableFieldsPanelVisibility() {
			DocumentModel.InnerApplyChanges(DocumentModelChangeActions.ResetPivotTableFieldsPanelVisibility);
		}
		internal void UpdatePivotTableFieldsPanelVisibility() {
			if (endInitializeIsCalled) {
				ShowHidePivotTablePanelCommand command = new ShowHidePivotTablePanelCommand(Owner);
				command.Execute();
			}
		}
		internal void ShowPivotTableFieldsPanel(FieldListPanelPivotTableViewModel viewModel) {
			innerPivotTableFieldsPanel.ShowPanel(viewModel);
		}
		internal void HidePivotTableFieldsPanel() {
			innerPivotTableFieldsPanel.HidePanel();
		}
	}
}
