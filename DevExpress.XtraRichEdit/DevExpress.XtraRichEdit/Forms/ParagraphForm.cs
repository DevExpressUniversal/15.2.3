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
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraEditors.Controls;
using DevExpress.Office;
using System.Drawing;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.xtraTabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.indentsAndSpacing")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.labelControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.labelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.labelControl3")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.paragraphSpacingControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.btnTabs")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.lblAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.edtAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.paragraphIndentationControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.edtOutlineLevel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.lblOutlineLevel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.lineAndPageBreaks")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.labelControl4")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.chkPageBreakBefore")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.chkKeepLinesTogether")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ParagraphForm.chkContextualSpacing")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	#region ParagraphForm
	[DXToolboxItem(false)]
	public partial class ParagraphForm : DevExpress.XtraEditors.XtraForm, IFormOwner {
		readonly ParagraphFormController controller;
		readonly IRichEditControl control;
		ParagraphForm() {
			InitializeComponent();
		}
		public ParagraphForm(ParagraphFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		protected internal ParagraphFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		protected internal virtual ParagraphFormController CreateController(ParagraphFormControllerParameters controllerParameters) {
			return new ParagraphFormController(controllerParameters);
		}
		protected override void OnShown(EventArgs e) {
			UpdateBtnTabsEnabled();
		}
		void UpdateBtnTabsEnabled() {
			if (Control != null)
				this.btnTabs.Enabled = Control.InnerControl.DocumentModel.DocumentCapabilities.ParagraphTabsAllowed;
		}
		private void InitializeForm() {
			this.btnTabs.Visible = Controller.CanEditTabs;
			this.paragraphIndentationControl.Properties.UnitType = control.InnerControl.UIUnit;
			this.paragraphIndentationControl.Properties.ValueUnitConverter = UnitConverter;
			this.paragraphSpacingControl.Properties.UnitType = DocumentUnit.Point;
			this.paragraphSpacingControl.Properties.MaxSpacing = ParagraphFormDefaults.MaxSpacingByDefault;
			this.paragraphSpacingControl.Properties.ValueUnitConverter = UnitConverter;
			IList<string> outlineLevelItems = Controller.OutlineLevelItems;
			ComboBoxItemCollection items = edtOutlineLevel.Properties.Items;
			int count = outlineLevelItems.Count;
			for (int i = 0; i < count; i++)
				items.Add(outlineLevelItems[i]);
		}
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
			UpdateParagraphIndentationControl();
			UpdateParagraphSpacingControl();
			edtAlignment.Alignment = Controller.Alignment;
			UpdateEdtOutlineLevelControl();
			UpdateChkKeepLinesTogetherControl();
			UpdateChkPageBreakBeforeControl();
			UpdateChkContextualSpacingControl();
		}
		void UpdateChkContextualSpacingControl() {
			if (Controller.ContextualSpacing == null) {
				chkContextualSpacing.Properties.AllowGrayed = true;
				chkContextualSpacing.CheckState = CheckState.Indeterminate;
			}
			else
				chkContextualSpacing.Checked = Controller.ContextualSpacing.Value;
		}
		void UpdateChkPageBreakBeforeControl() {
			if (Controller.PageBreakBefore == null) {
				chkPageBreakBefore.Properties.AllowGrayed = true;
				chkPageBreakBefore.CheckState = CheckState.Indeterminate;
			}
			else
				chkPageBreakBefore.Checked = Controller.PageBreakBefore.Value;
		}
		void UpdateChkKeepLinesTogetherControl() {
			if (Controller.KeepLinesTogether == null) {
				chkKeepLinesTogether.Properties.AllowGrayed = true;
				chkKeepLinesTogether.CheckState = CheckState.Indeterminate;
			}
			else
				chkKeepLinesTogether.Checked = Controller.KeepLinesTogether.Value;
		}
		void UpdateEdtOutlineLevelControl() {
			if (Controller.OutlineLevel.HasValue) {
				int level = Controller.OutlineLevel.Value;
				if (level < 0 || level > 9)
					level = 0;
				edtOutlineLevel.SelectedIndex = level;
			}
			else
				edtOutlineLevel.SelectedIndex = -1;
		}
		void UpdateParagraphSpacingControl() {
			this.paragraphSpacingControl.BeginUpdate();
			try {
				ParagraphSpacingProperties properties = this.paragraphSpacingControl.Properties;
				properties.SpacingAfter = controller.SpacingAfter;
				properties.SpacingBefore = controller.SpacingBefore;
				properties.LineSpacing = controller.LineSpacing;
				properties.LineSpacingType = controller.LineSpacingType;
			}
			finally {
				this.paragraphSpacingControl.EndUpdate();
			}
		}
		void UpdateParagraphIndentationControl() {
			this.paragraphIndentationControl.BeginUpdate();
			try {
				ParagraphIndentationProperties properties = this.paragraphIndentationControl.Properties;
				properties.LeftIndent = Controller.LeftIndent;
				if (Controller.FirstLineIndentType == ParagraphFirstLineIndent.Hanging && Controller.FirstLineIndent.HasValue)
					properties.LeftIndent -= Controller.FirstLineIndent;
				properties.RightIndent = Controller.RightIndent;
				properties.FirstLineIndent = Controller.FirstLineIndent;
				properties.FirstLineIndentType = Controller.FirstLineIndentType;
			}
			finally {
				this.paragraphIndentationControl.EndUpdate();
			}
		}
		#region SubscribeControlsEvents
		protected internal virtual void SubscribeControlsEvents() {
			edtAlignment.SelectedIndexChanged += OnAlignmentSelectedIndexChanged;
			edtOutlineLevel.SelectedIndexChanged += OnOutlineLevelSelectedIndexChanged;
			paragraphIndentationControl.ParagraphIndentControlChanged += OnParagraphIndentationControlChanged;
			paragraphSpacingControl.ParagraphSpacingControlChanged += OnParagraphSpacingControlChanged;		   
		}
		#endregion
		#region UnsubscribeControlsEvents
		protected internal virtual void UnsubscribeControlsEvents() {
			edtAlignment.SelectedIndexChanged -= OnAlignmentSelectedIndexChanged;
			edtOutlineLevel.SelectedIndexChanged -= OnOutlineLevelSelectedIndexChanged;
			paragraphIndentationControl.ParagraphIndentControlChanged -= OnParagraphIndentationControlChanged;
			paragraphSpacingControl.ParagraphSpacingControlChanged -= OnParagraphSpacingControlChanged;
		}
		#endregion
		#region OnTabsClick
		protected internal virtual void OnTabsClick(object sender, EventArgs e) {
			ShowTabsFormCommand showTabsFormCommand = new ShowTabsFormCommand(Control, this);
			showTabsFormCommand.Execute();
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		#endregion
		#region OnOkClick
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			if (!chkKeepLinesTogether.Properties.AllowGrayed || chkKeepLinesTogether.CheckState != CheckState.Indeterminate)
				Controller.KeepLinesTogether = chkKeepLinesTogether.Checked;
			if (!chkPageBreakBefore.Properties.AllowGrayed || chkPageBreakBefore.CheckState != CheckState.Indeterminate)
				Controller.PageBreakBefore = chkPageBreakBefore.Checked;
			if (!chkContextualSpacing.Properties.AllowGrayed || chkContextualSpacing.CheckState != CheckState.Indeterminate)
				Controller.ContextualSpacing = chkContextualSpacing.Checked;
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
		#endregion
		#region OnAlignmentSelectedIndexChanged
		protected internal virtual void OnAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			Controller.Alignment = edtAlignment.Alignment;
		}
		#endregion
		#region OnOutlineLevelSelectedIndexChanged
		protected internal virtual void OnOutlineLevelSelectedIndexChanged(object sender, EventArgs e) {
			if (edtOutlineLevel.SelectedIndex >= 0)
				Controller.OutlineLevel = edtOutlineLevel.SelectedIndex;
		}
		#endregion
		#region OnParagraphIndentationControlParagraphIndentControlChanged
		protected internal virtual void OnParagraphIndentationControlChanged(object sender, EventArgs e) {
			Controller.FirstLineIndentType = paragraphIndentationControl.Properties.FirstLineIndentType;
			Controller.FirstLineIndent = paragraphIndentationControl.Properties.FirstLineIndent;
			Controller.LeftIndent = this.paragraphIndentationControl.Properties.LeftIndent;
			if (paragraphIndentationControl.Properties.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				Controller.LeftIndent += Controller.FirstLineIndent;
			Controller.RightIndent = this.paragraphIndentationControl.Properties.RightIndent;
		}
		#endregion
		#region OnParagraphSpacingControlParagraphSpacingControlChanged
		protected internal virtual void OnParagraphSpacingControlChanged(object sender, EventArgs e) {
			ParagraphSpacingProperties properties = paragraphSpacingControl.Properties;
			controller.SpacingAfter = properties.SpacingAfter;
			controller.SpacingBefore = properties.SpacingBefore;
			controller.LineSpacing = properties.LineSpacing;
			controller.LineSpacingType = properties.LineSpacingType;
		}
		#endregion
		#region IFormOwner Members
		void IFormOwner.Hide() {
			this.Hide();
		}
		#endregion
	}
	#endregion
	[ToolboxItem(false)]
	public class RichEditTabPage : DevExpress.XtraTab.XtraTabPage {
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			Region oldClip = null;
			Region newClip = null;
			if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes) {
				oldClip = e.Graphics.Clip;
				RectangleF clipBounds = e.Graphics.ClipBounds;
				clipBounds.Inflate(1, 0);
				newClip = new Region(clipBounds);
				e.Graphics.Clip = newClip;
			}
			base.OnPaint(e);
			if (oldClip != null) {
				e.Graphics.Clip = oldClip;
			}
			if (newClip != null)
				newClip.Dispose();
		}
	}
	[ToolboxItem(false)]
	public class RichEditPanel : DevExpress.XtraEditors.PanelControl {
		protected override CreateParams CreateParams {
			get {
				return DevExpress.XtraRichEdit.Native.RightToLeftHelper.PatchCreateParams(base.CreateParams, this);
			}
		}
	}
}
