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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BulletedListForm.lblBulletCharacter")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BulletedListForm.btnCharacter")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BulletedListForm.lblBulletPosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BulletedListForm.bulletCharacterControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.BulletedListForm.lblTextPosition")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region BulletedListForm
	public partial class BulletedListForm : NumberingListFormsBase {
		#region Fields
		static SymbolProperties[] SymbolsProperties = {
			new SymbolProperties((char)8226, "Symbol"),
			new SymbolProperties((char)252, "Wingdings"),
			new SymbolProperties((char)1063, "Wingdings"),
			new SymbolProperties((char)118, "Wingdings"),
			new SymbolProperties((char)1031, "Wingdings 2"),
			new SymbolProperties((char)242, "Wingdings 2")
		};
		#endregion
		public BulletedListForm() { 
			InitializeComponent();
		}
		public BulletedListForm(ListLevelCollection<ListLevel> levels, int levelIndex, RichEditControl control, IFormOwner formOwner)
			: base(levels, levelIndex, control, formOwner) {
			InitializeComponent();
			CreateBulletCharacters();
			UpdateForm();
		}
		protected internal void CreateBulletCharacters() {
			string fontName = Controller.CharacterProperties.Info.FontName;
			SymbolProperties activeProperties = new SymbolProperties(Controller.DisplayFormat[0], fontName);
			int symbolIndex = GetActiveSymbolIndex(activeProperties);
			if (symbolIndex == -1) {
				SymbolsProperties[0] = activeProperties;
				symbolIndex = 0;
			}
			bulletCharacterControl.InitializeComponents(SymbolsProperties);
			bulletCharacterControl.SelectedIndex = symbolIndex;
		}
		protected int GetActiveSymbolIndex(SymbolProperties activeProperties) {
			int count = SymbolsProperties.Length;
			for (int i = 0; i < count; i++) {
				if (activeProperties.Equals(SymbolsProperties[i]))
					return i;
			}
			return -1;
		}
		protected internal override void SubscribeControlsEvents() {
			base.SubscribeControlsEvents();
			bulletCharacterControl.SelectedIndexChanged += OnBulletCharacterControlSelectedIndexChanged;
			bulletCharacterControl.MouseDoubleClick += OnMouseDoubleClick;
			btnFont.KeyDown += OnKeyDown;
			btnCharacter.KeyDown += OnKeyDown;
			btnOk.KeyDown += OnKeyDown;
			btnCancel.KeyDown += OnKeyDown;
			bulletCharacterControl.KeyDown += OnKeyDown;
		}
		protected internal override void UnsubscribeControlsEvents() {
			base.UnsubscribeControlsEvents();
			bulletCharacterControl.SelectedIndexChanged -= OnBulletCharacterControlSelectedIndexChanged;
			bulletCharacterControl.MouseDoubleClick -= OnMouseDoubleClick;
			btnFont.KeyDown -= OnKeyDown;
			btnCharacter.KeyDown -= OnKeyDown;
			btnOk.KeyDown -= OnKeyDown;
			btnCancel.KeyDown -= OnKeyDown;
			bulletCharacterControl.KeyDown -= OnKeyDown;
		}
		void OnBulletCharacterControlSelectedIndexChanged(object sender, EventArgs e) {
			SymbolProperties symbolProperties = bulletCharacterControl.GetActiveSymbolProperties();
			Controller.DisplayFormat = symbolProperties.UnicodeChar.ToString();
			Controller.CharacterProperties.FontName = symbolProperties.FontName;
		}
		void OnKeyDown(object sender, KeyEventArgs e) {
			bool isNumber = e.KeyCode > Keys.D0 && e.KeyCode < Keys.D7;
			bool isNumberNum = e.KeyCode > Keys.NumPad0 && e.KeyCode < Keys.NumPad7;
			if (isNumber || isNumberNum) {
				int index = GetNumber(e.KeyCode);
				bulletCharacterControl.SelectedIndex = index - 1;
			}
		}
		int GetNumber(Keys key) {
			if (key == Keys.D1 || key == Keys.NumPad1)
				return 1;
			if (key == Keys.D2 || key == Keys.NumPad2)
				return 2;
			if (key == Keys.D3 || key == Keys.NumPad3)
				return 3;
			if (key == Keys.D4 || key == Keys.NumPad4)
				return 4;
			if (key == Keys.D5 || key == Keys.NumPad5)
				return 5;
			if (key == Keys.D6 || key == Keys.NumPad6)
				return 6;
			return 0;
		}
		void OnMouseDoubleClick(object sender, MouseEventArgs e) {
			ApplyChanges();
		}
		private class ChooseCharacterViewModel : RichEditInsertSymbolViewModel {
			readonly BulletedListForm form;
			readonly SymbolProperties symbolProperties;
			public ChooseCharacterViewModel(IRichEditControl control, BulletedListForm form, SymbolProperties symbolProperties)
				: base(control) {
				this.form = form;
				this.symbolProperties = symbolProperties;
			}
			public SymbolProperties SymbolProperties { get { return symbolProperties; } }
			public override void ApplyChanges() {
				symbolProperties.FontName = this.FontName;
				symbolProperties.UnicodeChar = this.UnicodeChar;
				form.ApplySymbolProperties(this);
			}
		}
		void OnCharacterClick(object sender, EventArgs e) {
			SymbolProperties symbolProperties = bulletCharacterControl.GetActiveSymbolProperties();
			ChooseCharacterViewModel viewModel = new ChooseCharacterViewModel(this.Control, this, symbolProperties);
			Control.ShowSymbolForm(viewModel);
		}
		void ApplySymbolProperties(ChooseCharacterViewModel viewModel) {
			int index = bulletCharacterControl.SelectedIndex;
			SymbolsProperties[index] = viewModel.SymbolProperties;
			Controller.DisplayFormat = viewModel.UnicodeChar.ToString();
			Controller.CharacterProperties.FontName = viewModel.FontName;
			bulletCharacterControl.SetActiveSymbolProperties(viewModel.SymbolProperties);
			bulletCharacterControl.Focus();
		}
	}
	#endregion
}
