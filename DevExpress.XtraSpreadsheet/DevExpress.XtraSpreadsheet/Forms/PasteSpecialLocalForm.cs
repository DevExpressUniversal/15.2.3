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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSpreadsheet.Commands;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Forms {
	[DXToolboxItem(false)]
	public partial class PasteSpecialLocalForm : XtraForm {
		readonly PasteSpecialLocalFormController controller;
		PasteSpecialLocalForm() {
			InitializeComponent();
		}
		public PasteSpecialLocalForm(PasteSpecialLocalFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeForm();
			SubscribeControlEvents();
			UpdateForm();
		}
		protected internal PasteSpecialLocalFormController Controller { get { return controller; } }
		void InitializeForm() {
			IList<string> items = controller.GetPasteSpecialItems();
			foreach(var item in items) {
				RadioGroupItem rgItem = new RadioGroupItem(null, item);
				rg_paste.Properties.Items.Add(rgItem);
			}
		}
		public virtual PasteSpecialLocalFormController CreateController(PasteSpecialLocalFormControllerParameters controllerParameters) {
			return new PasteSpecialLocalFormController(controllerParameters);
		}
		protected void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			rg_paste.SelectedIndex = controller.CurrentPasteSpecialItemIndex;
			cbSkipBlanks.Checked = controller.SkipBlanks;
		}
		protected internal virtual void SubscribeControlEvents() {
			rg_paste.SelectedIndexChanged += OnSelectedIndexChanged;
			cbSkipBlanks.CheckedChanged += OnSkipBlanksCheckedChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			rg_paste.SelectedIndexChanged -= OnSelectedIndexChanged;
			cbSkipBlanks.CheckedChanged -= OnSkipBlanksCheckedChanged;
		}
		protected internal virtual void OnSkipBlanksCheckedChanged(object sender, EventArgs e) {
			controller.SkipBlanks = cbSkipBlanks.Checked;
		}
		protected internal virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
			controller.CurrentPasteSpecialItemIndex = rg_paste.SelectedIndex;
		}
		private void btnOk_Click(object sender, EventArgs e) {
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		private void rg_paste_Properties_DoubleClick(object sender, EventArgs e) {
			MouseEventArgs mouseEventArgs = e as MouseEventArgs;
			if (mouseEventArgs == null)
				return;
			RadioGroup group = sender as RadioGroup;
			for(int i =0; i < group.Properties.Items.Count; i++) {
				if (group.GetItemRectangle(i).Contains(mouseEventArgs.X, mouseEventArgs.Y)) {
					OnItemDoubleClick(group.Properties.Items[i]);
					break;
				}
			}
		}
		private void OnItemDoubleClick(RadioGroupItem item) {
			btnOk_Click(this, new EventArgs());
		}
	}
}
