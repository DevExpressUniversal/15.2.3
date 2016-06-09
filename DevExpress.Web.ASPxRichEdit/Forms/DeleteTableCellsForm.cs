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

using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class DeleteTableCellsForm : RichEditDialogBase {
		protected ASPxRadioButtonList CellOperationList { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			CellOperationList = group.Items.CreateEditor<ASPxRadioButtonList>("RblCellOperation", buffer: Editors, showCaption: false);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			CellOperationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DeleteCells_ShiftCellsLeft), (int)TableCellOperation.ShiftToTheHorizontally);
			CellOperationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DeleteCells_ShiftCellsUp), (int)TableCellOperation.ShiftToTheVertically);
			CellOperationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DeleteCells_DeleteEntireRow), (int)TableCellOperation.RowOperation);
			CellOperationList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DeleteCells_DeleteEntireColumn), (int)TableCellOperation.ColumnOperation);
			CellOperationList.ValueType = typeof(int);
			CellOperationList.SelectedIndex = 0;
			CellOperationList.Border.BorderStyle = BorderStyle.None;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgTableCellsForm";
		}
	}
}
