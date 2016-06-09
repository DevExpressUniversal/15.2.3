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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Forms;
using DevExpress.XtraRichEdit.Localization;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SymbolForm.edtFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SymbolForm.btnOK")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SymbolForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SymbolForm.lblFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.SymbolForm.symbolListBox")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class SymbolForm : XtraForm {
		#region Fields
		readonly InsertSymbolViewModel viewModel;
		#endregion
		SymbolForm() {
			InitializeComponent();
		}
		public SymbolForm(InsertSymbolViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			if (viewModel.ModelessBehavior)
				btnOK.Text = XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SymbolFormInsertButton);
			UpdateForm();
		}
		#region Properties
		public InsertSymbolViewModel ViewModel { get { return viewModel; } }
		#endregion
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			edtFont.Text = ViewModel.FontName;
			symbolListBox.FontName = ViewModel.FontName;
			symbolListBox.SetSelectedChar(ViewModel.UnicodeChar);
		}
		protected internal virtual void SubscribeControlsEvents() {
			this.edtFont.EditValueChanged += OnFontEditValueChanged;
			this.symbolListBox.SelectedIndexSymbolListBoxChanged += OnSymbolListBoxSelectedIndexChanged;
			this.symbolListBox.MouseDoubleClick += symbolListBoxMouseDoubleClick;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			this.edtFont.EditValueChanged -= OnFontEditValueChanged;
			this.symbolListBox.SelectedIndexSymbolListBoxChanged -= OnSymbolListBoxSelectedIndexChanged;
			this.symbolListBox.MouseDoubleClick -= symbolListBoxMouseDoubleClick;
		}
		void symbolListBoxMouseDoubleClick(object sender, MouseEventArgs e) {
			if (symbolListBox.isMouseClickInItems)
				ViewModel.ApplyChanges();
		}
		void OnFontEditValueChanged(object sender, EventArgs e) {
			string fontName = edtFont.Text;
			symbolListBox.FontName = fontName;
			ViewModel.FontName = fontName;
			ViewModel.UnicodeChar = symbolListBox.GetSelectedChar();
		}
		void OnSymbolListBoxSelectedIndexChanged(object sender, EventArgs e) {
			ViewModel.UnicodeChar = symbolListBox.GetSelectedChar();
		}
		void OnOkClick(object sender, EventArgs e) {
			ApplyChanges();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		protected internal virtual void ApplyChanges() {
			ViewModel.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
	}
}
