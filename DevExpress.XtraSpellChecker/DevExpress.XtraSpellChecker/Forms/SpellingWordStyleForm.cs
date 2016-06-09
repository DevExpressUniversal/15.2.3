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
using System.Reflection;
using DevExpress.XtraSpellChecker.Rules;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Native;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraSpellChecker.Forms.SpellingWordStyleForm.mmNotInDictionary")]
#endregion
namespace DevExpress.XtraSpellChecker.Forms {
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1301:AvoidDuplicateAccelerators")]
	public class SpellingWordStyleForm : SpellingOutlookStyleForm {
		#region Fields
		private static readonly object memoSelectionStartChanged = new object();
		#endregion
		#region Generated Fields
		protected internal DevExpress.XtraSpellChecker.Controls.CustomSpellCheckMemoEdit mmNotInDictionary;
		#endregion
		private System.ComponentModel.Container components = null;
		public SpellingWordStyleForm() {
			Initialize();
		}
		public SpellingWordStyleForm(SpellChecker spellChecker)
			: base(spellChecker) {
			Initialize();
		}
		void Initialize() {
			InitializeComponent();
			InitializeIcon();
		}
		protected override void InitializeIcon() {
			this.Icon = ResourceImageHelper.CreateIconFromResources("DevExpress.XtraSpellChecker.Images.SpellChecker.ico", System.Reflection.Assembly.GetExecutingAssembly());
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellingWordStyleForm));
			this.mmNotInDictionary = new DevExpress.XtraSpellChecker.Controls.CustomSpellCheckMemoEdit();
			((System.ComponentModel.ISupportInitialize)(this.txtNotInDictionary.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtChangeTo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcSuggestions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.mmNotInDictionary.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblNotInDictionary, "lblNotInDictionary");
			resources.ApplyResources(this.lblChangeTo, "lblChangeTo");
			resources.ApplyResources(this.lblSuggestions, "lblSuggestions");
			resources.ApplyResources(this.btnIgnore, "btnIgnore");
			resources.ApplyResources(this.btnChangeAll, "btnChangeAll");
			resources.ApplyResources(this.btnAdd, "btnAdd");
			resources.ApplyResources(this.btnSuggest, "btnSuggest");
			resources.ApplyResources(this.btnIgnoreAll, "btnIgnoreAll");
			resources.ApplyResources(this.btnChange, "btnChange");
			resources.ApplyResources(this.btnOptions, "btnOptions");
			resources.ApplyResources(this.btnUndoLast, "btnUndoLast");
			resources.ApplyResources(this.txtNotInDictionary, "txtNotInDictionary");
			resources.ApplyResources(this.txtChangeTo, "txtChangeTo");
			this.lbcSuggestions.ItemAutoHeight = true;
			resources.ApplyResources(this.lbcSuggestions, "lbcSuggestions");
			resources.ApplyResources(this.btnClose, "btnClose");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			resources.ApplyResources(this.btnDelete, "btnDelete");
			resources.ApplyResources(this.lblRepeatedWord, "lblRepeatedWord");
			this.mmNotInDictionary.AllowSelectAll = false;
			resources.ApplyResources(this.mmNotInDictionary, "mmNotInDictionary");
			this.mmNotInDictionary.Name = "mmNotInDictionary";
			this.mmNotInDictionary.Properties.HideSelection = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mmNotInDictionary);
			this.KeyPreview = true;
			this.Name = "SpellingWordStyleForm";
			this.Load += new System.EventHandler(this.SpellCheckFormEx_Load);
			this.Controls.SetChildIndex(this.lblRepeatedWord, 0);
			this.Controls.SetChildIndex(this.lblNotInDictionary, 0);
			this.Controls.SetChildIndex(this.lblSuggestions, 0);
			this.Controls.SetChildIndex(this.mmNotInDictionary, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.btnClose, 0);
			this.Controls.SetChildIndex(this.btnUndoLast, 0);
			this.Controls.SetChildIndex(this.btnOptions, 0);
			this.Controls.SetChildIndex(this.btnChange, 0);
			this.Controls.SetChildIndex(this.btnIgnoreAll, 0);
			this.Controls.SetChildIndex(this.btnSuggest, 0);
			this.Controls.SetChildIndex(this.btnAdd, 0);
			this.Controls.SetChildIndex(this.btnChangeAll, 0);
			this.Controls.SetChildIndex(this.txtChangeTo, 0);
			this.Controls.SetChildIndex(this.lblChangeTo, 0);
			this.Controls.SetChildIndex(this.lbcSuggestions, 0);
			this.Controls.SetChildIndex(this.txtNotInDictionary, 0);
			this.Controls.SetChildIndex(this.btnIgnore, 0);
			this.Controls.SetChildIndex(this.btnDelete, 0);
			((System.ComponentModel.ISupportInitialize)(this.txtNotInDictionary.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtChangeTo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcSuggestions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.mmNotInDictionary.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
	   private void SpellCheckFormEx_Load(object sender, System.EventArgs e) {
			if (SpellChecker == null || SpellChecker.SearchStrategy == null)
				return;
		   ISpellCheckMSWordTextControlController textController = SpellChecker.SearchStrategy.TextController as ISpellCheckMSWordTextControlController;
		   if (textController != null)
				textController.Select(textController.SelectionStart, textController.SelectionFinish);
	   }
		protected override void SetMinimumFormSize() {
			MinimumSize = new Size(475, 306);
		}
	}
}
