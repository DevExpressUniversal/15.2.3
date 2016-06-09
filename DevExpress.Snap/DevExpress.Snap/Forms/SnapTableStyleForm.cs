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
using System.Linq;
using System.Text;
using DevExpress.Snap.Core.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Forms {
	public class SnapTableStyleForm : TableStyleForm {
		SnapTableStyleForm() { }
		public SnapTableStyleForm(FormControllerParameters controllerParameters)
			: base(controllerParameters) {
			cbApplyTo.Enabled = false;
		}
		protected internal TableCellStyle IntermediateTableCellStyle { get { return ((TableCellStyleFormController)Controller).IntermediateTableCellStyle; } }
		private TableCellStyle SourceCellStyle { get { return ((TableCellStyleFormController)Controller).TableSourceStyle; } }
		protected override TableStyleFormControllerBase CreateController(FormControllerParameters controllerParameters) {
			return new TableCellStyleFormController(previewRichEditControl, controllerParameters);
		}
		protected override void ChangeConditionalType() {
		}
		protected override void SetParentStyle() {
			IntermediateTableCellStyle.Parent = (cbParent.SelectedIndex != 0) ? (TableCellStyle)cbParent.SelectedItem : null;
		}
		protected override void FillCurrentStyleComboCore(ComboBoxEdit comboBoxEdit) {
			FillCurrentStyleCombo<TableCellStyle>(comboBoxEdit, SourceCellStyle.DocumentModel.TableCellStyles);
		}
		protected override void FillParentStyleCombo() {
			ComboBoxItemCollection collection = cbParent.Properties.Items;
			TableCellStyleCollection styles = SourceCellStyle.DocumentModel.TableCellStyles;
			collection.BeginUpdate();
			try {
				int count = styles.Count;
				for (int i = 0; i < count; i++)
					if (SourceCellStyle.IsParentValid(styles[i]))
						collection.Add(styles[i]);
			}
			finally {
				collection.EndUpdate();
			}
		}
		protected override void InitializeConditionalStyleType() {
		}
		protected override void SetNewStyleToController(string styleName) {
			((TableCellStyleFormControllerParameters)Controller.Parameters).TableSourceStyle = Controller.Model.TableCellStyles.GetStyleByName(styleName);
		}
		protected override void UpdateFormCore() {
			TableCellStyle parent = SourceCellStyle.Parent;
			cbCurrentStyle.SelectedItem = SourceCellStyle;
			if (parent == null)
				cbParent.SelectedIndex = 0;
			else
				cbParent.SelectedItem = parent;
		}
		protected override void OnApplyToSelectedIndexChangedCore() {
		}
		protected override void OnTableCellsBorderItemPressCore() {
		}
	}
}
