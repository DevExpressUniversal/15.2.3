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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class LanguageForm : XtraForm {
		LanguageFormController controller;
		public LanguageForm() {
			InitializeComponent();
		}
		public void Initialize(LanguageFormController controller) {
			this.controller = controller;
			foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
				this.languageListBox.Items.Add(new LanguageItem(ci));
			}
			if (controller.Language.Latin != null) {
				for (int i = 0; i < this.languageListBox.ItemCount; i++) {
					if (this.languageListBox.Items[i].ToString() == controller.Language.Latin.DisplayName)
						this.languageListBox.SetSelected(i, true);
				}
			}
			else {
				this.languageListBox.SelectedIndex = -1;
			}
			if (controller.NoProof.HasValue)
				this.checkGrammar.Checked = controller.NoProof.Value;
			else {
				this.checkGrammar.CheckState = CheckState.Indeterminate;
			}
		}
		private void languageListBox_Click(object sender, EventArgs e) {
			controller.Language = new LangInfo(((LanguageItem)this.languageListBox.SelectedItem).Culture, null, null);
		}
		private void languageListBox_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter) 
				ApplyChange();
		}
		private void languageListBox_KeyPress(object sender, KeyPressEventArgs e) {
		}
		private void languageListBox_KeyUp(object sender, KeyEventArgs e) {
		}
		private void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
		private void btnOK_Click(object sender, EventArgs e) {
			ApplyChange();
		}
		private void btnSetDefault_CheckedChanged(object sender, EventArgs e) {
		}
		private void checkGrammar_CheckedChanged(object sender, EventArgs e) {
			if (checkGrammar.CheckState == CheckState.Indeterminate)
				controller.NoProof = null;
			else
				controller.NoProof = checkGrammar.Checked; 
		}
		private void checkGrammar_CheckStateChanged(object sender, EventArgs e) {
		}
		void ApplyChange() {
			if (this.languageListBox.SelectedItem != null)
				controller.Language = new LangInfo(((LanguageItem)this.languageListBox.SelectedItem).Culture, null, null);
			controller.ApplyChanges();
			Close();
		}
	}
	public class LanguageItem {
		CultureInfo ci;
		public LanguageItem(CultureInfo ci) {
			this.ci = ci;
		}
		public CultureInfo Culture { 
			get {
				return ci;
			}
		}
		public override string ToString() {
			return ci.DisplayName;
		}
	}
}
