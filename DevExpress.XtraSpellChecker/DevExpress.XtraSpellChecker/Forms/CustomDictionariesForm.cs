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
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.Utils;
using System.Text;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.CustomDictionariesForm.btnOK")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.CustomDictionariesForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.CustomDictionariesForm.mmWords")]
#endregion
namespace DevExpress.XtraSpellChecker.Forms {
	public class CustomDictionariesForm : BaseSpellCheckForm {
		#region Generated Fields
		protected internal DevExpress.XtraEditors.SimpleButton btnOK;
		protected internal DevExpress.XtraEditors.SimpleButton btnCancel;
		protected internal DevExpress.XtraEditors.MemoEdit mmWords;
		#endregion
		System.ComponentModel.Container components = null;
		SpellCheckerCustomDictionary dictionary = null;
		bool isModified = false;
		public CustomDictionariesForm() {
			InitializeComponent();
		}
		public CustomDictionariesForm(SpellChecker spellChecker, SpellCheckerCustomDictionary dictionary)
			: base(spellChecker) {
			InitializeComponent();
			this.Icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraSpellChecker.Images.Dictionary.ico", System.Reflection.Assembly.GetExecutingAssembly());
			this.dictionary = dictionary;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomDictionariesForm));
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.mmWords = new DevExpress.XtraEditors.MemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.mmWords.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.mmWords, "mmWords");
			this.mmWords.Name = "mmWords";
			this.mmWords.TextChanged += new System.EventHandler(this.mmWords_TextChanged);
			this.mmWords.Enter += new System.EventHandler(this.mmWords_Enter);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.mmWords);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Name = "CustomDictionariesForm";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.EditCustomDictForm_Closing);
			this.Load += new System.EventHandler(this.EditCustomDictForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.mmWords.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected SpellCheckerCustomDictionary Dictionary { get { return dictionary; } }
		public virtual bool IsModified { get { return isModified; } set { isModified = value; } }
		protected virtual void LoadWords() {
			StringBuilder text = new StringBuilder();
			if (Dictionary.WordCount > 0) {
				for (int i = 0; i < Dictionary.WordCount; i++)
					text.AppendLine(Dictionary[i]);
			}
			mmWords.Text = text.ToString();
		}
		void EditCustomDictForm_Load(object sender, EventArgs e) {
			OnLoad();
		}
		protected virtual void OnLoad() {
			if (Dictionary != null)
				LoadWords();
		}
		protected virtual bool NeedSave() {
			return IsModified;
		}
		private void mmWords_TextChanged(object sender, EventArgs e) {
			IsModified = true;
		}
		private void EditCustomDictForm_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (DialogResult == DialogResult.OK && NeedSave()) {
				Dictionary.Clear();
				Dictionary.AddWords(mmWords.Lines);
			}
		}
		protected override bool ShouldSaveFormInformation() {
			return true;
		}
		private void mmWords_Enter(object sender, EventArgs e) {
			OnMemoGotFocus();
		}
		protected virtual void OnMemoGotFocus() {
			mmWords.DeselectAll();
		}
	}
}
