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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraSpellChecker.Strategies;
namespace DevExpress.XtraSpellChecker.Forms {
	public class SpellingFormBase : BaseSpellCheckForm {
		private System.ComponentModel.Container components = null;
		private StringCollection suggestions = new StringCollection();
		public SpellingFormBase() : base() {
			Initialize();
		}
		public SpellingFormBase(SpellChecker spellChecker) : base(spellChecker) {
			Initialize();
		}
		void Initialize() {
			InitializeComponent(); 
		}
		public override void Init() {
			base.Init();
			this.spellingFormResult = SpellCheckOperation.Cancel;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private static readonly object spellingFormResultChanged = new object();
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpellingFormBase));
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "CustomSpellCheckForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomSpellCheckForm_FormClosing);
			this.ResumeLayout(false);
		}
		#endregion
		SpellCheckOperation spellingFormResult;
		string suggestion = String.Empty;
		public virtual StringCollection Suggestions {
			get {
				return suggestions;
			}
		}
		public virtual void ShowSpellingFormDialog(IWin32Window owner) {
			Init();
			if (!Visible)
				DevExpress.Utils.Internal.FormTouchUIAdapter.ShowDialog(this, owner);
		}
		public virtual void ShowSpellingForm(IWin32Window owner) {
			Init();
			if(!Visible)
				DevExpress.Utils.Internal.FormTouchUIAdapter.Show(this, owner);
		}
		public SpellCheckOperation SpellingFormResult {
			get { return spellingFormResult; }
			set {
				this.spellingFormResult = value;
				string fSuggestion = spellingFormResult == SpellCheckOperation.Cancel ? string.Empty : Suggestion;
				RaiseSpellCheckFormResultChanged(new SpellingFormResultChangedEventArgs(spellingFormResult, fSuggestion));
			}
		}
		public event SpellingFormResultChangedEventHandler SpellCheckFormResultChanged {
			add { Events.AddHandler(spellingFormResultChanged, value); }
			remove { Events.RemoveHandler(spellingFormResultChanged, value); }
		}
		protected virtual void RaiseSpellCheckFormResultChanged(SpellingFormResultChangedEventArgs e) {
			SpellingFormResultChangedEventHandler handler = (SpellingFormResultChangedEventHandler)this.Events[spellingFormResultChanged];
			if (handler != null) handler(this, e);
		}
		public virtual string Suggestion {
			get {
				if (!String.IsNullOrEmpty(suggestion))
					return suggestion;
				return CalcSuggestion();
			}
			set { suggestion = value; }
		}
		protected virtual string CalcSuggestion() {
			return String.Empty;
		}
		protected override bool ShouldSaveFormInformation() {
			return true;
		}
		private void CustomSpellCheckForm_FormClosing(object sender, FormClosingEventArgs e) {
			if(SpellChecker != null)
				if(SpellingFormResult != SpellCheckOperation.Cancel)
					SpellingFormResult = SpellCheckOperation.Cancel;
		}
	}
}
