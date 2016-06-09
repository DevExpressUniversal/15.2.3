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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.ComponentModel;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.btnFont")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.edtAligned")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.edtIndent")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.lblAlignedAt")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListFormsBase.lblIndentAt")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class NumberingListFormsBase : XtraForm {
		readonly RichEditControl control;
		readonly IFormOwner formOwner;
		BulletedListFormController controller;
		public NumberingListFormsBase(ListLevelCollection<ListLevel> levels, int levelIndex, RichEditControl control, IFormOwner formOwner) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(levels, "levels");
			Guard.ArgumentNotNull(formOwner, "formOwner");
			Guard.ArgumentNonNegative(levelIndex, "levelIndex");
			this.control = control;
			this.formOwner = formOwner;
			this.controller = CreateController(levels);
			this.Controller.EditedLevelIndex = levelIndex;
			InitializeComponent();
			InitializeForm();
		}
		public NumberingListFormsBase() { 
			InitializeComponent();
		}
		#region Properties
		protected internal RichEditControl Control { get { return control; } }
		protected internal BulletedListFormController Controller { get { return controller; } }
		protected internal IFormOwner FormOwner { get { return formOwner; } }
		#endregion
		void InitializeForm() {
			this.edtAligned.ValueUnitConverter = Control.DocumentModel.UnitConverter;
			this.edtIndent.ValueUnitConverter = Control.DocumentModel.UnitConverter;
			this.edtAligned.Properties.DefaultUnitType = Control.InnerControl.UIUnit;
			this.edtIndent.Properties.DefaultUnitType = Control.InnerControl.UIUnit;
		}
		protected virtual BulletedListFormController CreateController(ListLevelCollection<ListLevel> sourceLevels) {
			return new BulletedListFormController(sourceLevels);
		}
		protected virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected virtual void UpdateFormCore() {
			edtAligned.Value = CalculateFirstLineIndent();
			edtIndent.Value = Controller.LeftIndent;
		}
		protected internal virtual int CalculateFirstLineIndent() {
			ParagraphProperties paragraphProperties = Controller.EditedLevel.ParagraphProperties;
			if(paragraphProperties.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				return Controller.LeftIndent - Controller.FirstLineIndent;
			return Controller.FirstLineIndent + Controller.LeftIndent;
		}
		void OnOkClick(object sender, EventArgs e) {
			ApplyChanges();
		}
		public virtual void ApplyChanges() {
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		void OnFontClick(object sender, EventArgs e) {
			ShowFontForm(control, Controller.CharacterProperties);
		}
		protected internal virtual void SubscribeControlsEvents() {
			edtAligned.ValueChanged += OnAlignedValueChanged;
			edtIndent.ValueChanged += OnIndentValueChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			edtAligned.ValueChanged -= OnAlignedValueChanged;
			edtIndent.ValueChanged -= OnIndentValueChanged;
		}
		void OnAlignedValueChanged(object sender, EventArgs e) {
			AssignControllerIndentValues(edtAligned.Value, edtIndent.Value);
		}
		protected internal virtual int UpdateFirstLineIndent(int newValue) {
			return Math.Abs(newValue - Controller.LeftIndent);
		}
		void OnIndentValueChanged(object sender, EventArgs e) {
			AssignControllerIndentValues(edtAligned.Value, edtIndent.Value);
		}
		protected void AssignControllerIndentValues(int? align, int? indent) {
			if(!align.HasValue || !indent.HasValue)
				return;
			int alignValue = align.Value;
			int indentValue = indent.Value;
			Controller.LeftIndent = indentValue;
			int probableFirstLineIndent = indentValue - alignValue;
			if(probableFirstLineIndent > 0) {
				Controller.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				Controller.FirstLineIndent = probableFirstLineIndent;
			}
			else if(probableFirstLineIndent < 0) {
				Controller.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				Controller.FirstLineIndent = -probableFirstLineIndent;
			}
			else {
				Controller.FirstLineIndentType = ParagraphFirstLineIndent.None;
				Controller.FirstLineIndent = 0;
			}
		}
		public static void ShowFontForm(RichEditControl control, CharacterProperties sourceProperties) {
			MergedCharacterProperties editedProperties = new MergedCharacterProperties(sourceProperties.Info.Info, sourceProperties.Info.Options);
			control.ShowFontForm(editedProperties, ApplyCharacterProperties, sourceProperties);
		}
		static void ApplyCharacterProperties(MergedCharacterProperties properties, object data) {
			FontPropertiesModifier modified = new FontPropertiesModifier(properties);
			modified.ApplyCharacterProperties((ICharacterProperties)data);
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			if (FormOwner != null)
				FormOwner.Hide();
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if (FormOwner != null)
				if (DialogResult == DialogResult.Cancel)
					FormOwner.Show();
				else
					FormOwner.Close();
		}
	}
}
