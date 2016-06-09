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

namespace DevExpress.XtraRichEdit.Forms {
	partial class MultiLevelNumberingListForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiLevelNumberingListForm));
			this.lBoxLevel = new DevExpress.XtraEditors.ListBoxControl();
			this.lbLevel = new DevExpress.XtraEditors.LabelControl();
			this.lblFollowNumberWith = new DevExpress.XtraEditors.LabelControl();
			this.edtFollowNumber = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblDisplayFormat = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDisplayFormat.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtNumberingAlignment.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtAligned.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lBoxLevel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFollowNumber.Properties)).BeginInit();
			this.SuspendLayout();
			this.edtNumberFormat.AccessibleDescription = null;
			this.edtNumberFormat.AccessibleName = null;
			resources.ApplyResources(this.edtNumberFormat, "edtNumberFormat");
			this.edtNumberFormat.Options.Behavior.Drag = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtNumberFormat.Options.Behavior.Drop = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtNumberFormat.Options.Behavior.ShowPopupMenu = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
			this.edtNumberFormat.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.edtNumberFormat.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.edtNumberFormat.Options.VerticalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
			this.edtNumberFormat.Options.VerticalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
			this.edtNumberFormat.Views.SimpleView.Padding = new System.Windows.Forms.Padding(3, 1, 0, 0);
			this.lblNumberFormat.AccessibleDescription = null;
			this.lblNumberFormat.AccessibleName = null;
			resources.ApplyResources(this.lblNumberFormat, "lblNumberFormat");
			this.lblNumberFormat.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblNumberStyle.AccessibleDescription = null;
			this.lblNumberStyle.AccessibleName = null;
			resources.ApplyResources(this.lblNumberStyle, "lblNumberStyle");
			resources.ApplyResources(this.edtStart, "edtStart");
			this.edtStart.BackgroundImage = null;
			this.edtStart.CausesValidation = false;
			this.edtStart.EditValue = null;
			this.edtStart.Properties.AccessibleDescription = null;
			this.edtStart.Properties.AccessibleName = null;
			this.edtStart.Properties.AutoHeight = ((bool)(resources.GetObject("edtStart.Properties.AutoHeight")));
			this.edtStart.Properties.EditFormat.FormatString = "0{0}";
			this.edtStart.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
			this.edtStart.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("edtStart.Properties.Mask.AutoComplete")));
			this.edtStart.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("edtStart.Properties.Mask.BeepOnError")));
			this.edtStart.Properties.Mask.EditMask = resources.GetString("edtStart.Properties.Mask.EditMask");
			this.edtStart.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("edtStart.Properties.Mask.IgnoreMaskBlank")));
			this.edtStart.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("edtStart.Properties.Mask.MaskType")));
			this.edtStart.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("edtStart.Properties.Mask.PlaceHolder")));
			this.edtStart.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("edtStart.Properties.Mask.SaveLiteral")));
			this.edtStart.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("edtStart.Properties.Mask.ShowPlaceHolders")));
			this.edtStart.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("edtStart.Properties.Mask.UseMaskAsDisplayFormat")));
			this.edtStart.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtStart.Properties.NullValuePromptShowForEmptyValue")));
			this.lblStartAt.AccessibleDescription = null;
			this.lblStartAt.AccessibleName = null;
			resources.ApplyResources(this.lblStartAt, "lblStartAt");
			this.lblNumberPosition.AccessibleDescription = null;
			this.lblNumberPosition.AccessibleName = null;
			resources.ApplyResources(this.lblNumberPosition, "lblNumberPosition");
			resources.ApplyResources(this.edtDisplayFormat, "edtDisplayFormat");
			this.edtDisplayFormat.BackgroundImage = null;
			this.edtDisplayFormat.Properties.AccessibleDescription = null;
			this.edtDisplayFormat.Properties.AccessibleName = null;
			this.edtDisplayFormat.Properties.AutoHeight = ((bool)(resources.GetObject("edtDisplayFormat.Properties.AutoHeight")));
			this.edtDisplayFormat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtDisplayFormat.Properties.Buttons"))))});
			this.edtDisplayFormat.Properties.GlyphAlignment = ((DevExpress.Utils.HorzAlignment)(resources.GetObject("edtDisplayFormat.Properties.GlyphAlignment")));
			this.edtDisplayFormat.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtDisplayFormat.Properties.NullValuePromptShowForEmptyValue")));
			resources.ApplyResources(this.edtNumberingAlignment, "edtNumberingAlignment");
			this.edtNumberingAlignment.BackgroundImage = null;
			this.edtNumberingAlignment.Properties.AccessibleDescription = null;
			this.edtNumberingAlignment.Properties.AccessibleName = null;
			this.edtNumberingAlignment.Properties.AutoHeight = ((bool)(resources.GetObject("edtNumberingAlignment.Properties.AutoHeight")));
			this.edtNumberingAlignment.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtNumberingAlignment.Properties.Buttons"))))});
			this.edtNumberingAlignment.Properties.GlyphAlignment = ((DevExpress.Utils.HorzAlignment)(resources.GetObject("edtNumberingAlignment.Properties.GlyphAlignment")));
			this.edtNumberingAlignment.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtNumberingAlignment.Properties.NullValuePromptShowForEmptyValue")));
			this.lblTextPosition.AccessibleDescription = null;
			this.lblTextPosition.AccessibleName = null;
			resources.ApplyResources(this.lblTextPosition, "lblTextPosition");
			this.btnOk.AccessibleDescription = null;
			this.btnOk.AccessibleName = null;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.BackgroundImage = null;
			this.btnCancel.AccessibleDescription = null;
			this.btnCancel.AccessibleName = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.BackgroundImage = null;
			this.btnFont.AccessibleDescription = null;
			this.btnFont.AccessibleName = null;
			resources.ApplyResources(this.btnFont, "btnFont");
			this.btnFont.BackgroundImage = null;
			resources.ApplyResources(this.edtAligned, "edtAligned");
			this.edtAligned.BackgroundImage = null;
			this.edtAligned.Properties.AccessibleDescription = null;
			this.edtAligned.Properties.AccessibleName = null;
			this.edtAligned.Properties.AutoHeight = ((bool)(resources.GetObject("edtAligned.Properties.AutoHeight")));
			this.edtAligned.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtAligned.Properties.NullValuePromptShowForEmptyValue")));
			resources.ApplyResources(this.edtIndent, "edtIndent");
			this.edtIndent.BackgroundImage = null;
			this.edtIndent.Properties.AccessibleDescription = null;
			this.edtIndent.Properties.AccessibleName = null;
			this.edtIndent.Properties.AutoHeight = ((bool)(resources.GetObject("edtIndent.Properties.AutoHeight")));
			this.edtIndent.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtIndent.Properties.NullValuePromptShowForEmptyValue")));
			this.lblAlignedAt.AccessibleDescription = null;
			this.lblAlignedAt.AccessibleName = null;
			resources.ApplyResources(this.lblAlignedAt, "lblAlignedAt");
			this.lblIndentAt.AccessibleDescription = null;
			this.lblIndentAt.AccessibleName = null;
			resources.ApplyResources(this.lblIndentAt, "lblIndentAt");
			this.lBoxLevel.AccessibleDescription = null;
			this.lBoxLevel.AccessibleName = null;
			resources.ApplyResources(this.lBoxLevel, "lBoxLevel");
			this.lBoxLevel.BackgroundImage = null;
			this.lBoxLevel.Items.AddRange(new object[] {
			resources.GetString("lBoxLevel.Items"),
			resources.GetString("lBoxLevel.Items1"),
			resources.GetString("lBoxLevel.Items2"),
			resources.GetString("lBoxLevel.Items3"),
			resources.GetString("lBoxLevel.Items4"),
			resources.GetString("lBoxLevel.Items5"),
			resources.GetString("lBoxLevel.Items6"),
			resources.GetString("lBoxLevel.Items7"),
			resources.GetString("lBoxLevel.Items8")});
			this.lBoxLevel.Name = "lBoxLevel";
			this.lbLevel.AccessibleDescription = null;
			this.lbLevel.AccessibleName = null;
			resources.ApplyResources(this.lbLevel, "lbLevel");
			this.lbLevel.Name = "lbLevel";
			this.lblFollowNumberWith.AccessibleDescription = null;
			this.lblFollowNumberWith.AccessibleName = null;
			resources.ApplyResources(this.lblFollowNumberWith, "lblFollowNumberWith");
			this.lblFollowNumberWith.Name = "lblFollowNumberWith";
			resources.ApplyResources(this.edtFollowNumber, "edtFollowNumber");
			this.edtFollowNumber.BackgroundImage = null;
			this.edtFollowNumber.EditValue = null;
			this.edtFollowNumber.Name = "edtFollowNumber";
			this.edtFollowNumber.Properties.AccessibleDescription = null;
			this.edtFollowNumber.Properties.AccessibleName = null;
			this.edtFollowNumber.Properties.AutoHeight = ((bool)(resources.GetObject("edtFollowNumber.Properties.AutoHeight")));
			this.edtFollowNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtFollowNumber.Properties.Buttons"))))});
			this.edtFollowNumber.Properties.Items.AddRange(new object[] {
			resources.GetString("edtFollowNumber.Properties.Items"),
			resources.GetString("edtFollowNumber.Properties.Items1"),
			resources.GetString("edtFollowNumber.Properties.Items2")});
			this.edtFollowNumber.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("edtFollowNumber.Properties.NullValuePromptShowForEmptyValue")));
			this.edtFollowNumber.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblDisplayFormat.AccessibleDescription = null;
			this.lblDisplayFormat.AccessibleName = null;
			this.lblDisplayFormat.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			resources.ApplyResources(this.lblDisplayFormat, "lblDisplayFormat");
			this.lblDisplayFormat.Name = "lblDisplayFormat";
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lBoxLevel);
			this.Controls.Add(this.lbLevel);
			this.Controls.Add(this.edtFollowNumber);
			this.Controls.Add(this.lblDisplayFormat);
			this.Controls.Add(this.lblFollowNumberWith);
			this.Icon = null;
			this.Name = "MultiLevelNumberingListForm";
			this.Controls.SetChildIndex(this.lblFollowNumberWith, 0);
			this.Controls.SetChildIndex(this.lblIndentAt, 0);
			this.Controls.SetChildIndex(this.lblAlignedAt, 0);
			this.Controls.SetChildIndex(this.lblDisplayFormat, 0);
			this.Controls.SetChildIndex(this.edtFollowNumber, 0);
			this.Controls.SetChildIndex(this.lblNumberStyle, 0);
			this.Controls.SetChildIndex(this.lbLevel, 0);
			this.Controls.SetChildIndex(this.lBoxLevel, 0);
			this.Controls.SetChildIndex(this.edtNumberFormat, 0);
			this.Controls.SetChildIndex(this.lblNumberFormat, 0);
			this.Controls.SetChildIndex(this.edtStart, 0);
			this.Controls.SetChildIndex(this.lblStartAt, 0);
			this.Controls.SetChildIndex(this.lblNumberPosition, 0);
			this.Controls.SetChildIndex(this.edtDisplayFormat, 0);
			this.Controls.SetChildIndex(this.edtNumberingAlignment, 0);
			this.Controls.SetChildIndex(this.lblTextPosition, 0);
			this.Controls.SetChildIndex(this.btnFont, 0);
			this.Controls.SetChildIndex(this.edtAligned, 0);
			this.Controls.SetChildIndex(this.edtIndent, 0);
			this.Controls.SetChildIndex(this.btnOk, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtDisplayFormat.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtNumberingAlignment.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtAligned.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lBoxLevel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtFollowNumber.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.LabelControl lbLevel;
		protected DevExpress.XtraEditors.ListBoxControl lBoxLevel;
		protected DevExpress.XtraEditors.LabelControl lblFollowNumberWith;
		protected DevExpress.XtraEditors.ComboBoxEdit edtFollowNumber;
		protected DevExpress.XtraEditors.LabelControl lblDisplayFormat;
	}
}
