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

using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraPivotGrid.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.XtraPivotGrid.FilterPopup.SummaryFilter {
	public class PivotSummaryFilterPanel : PanelControl {
		internal static string GetEditMask(FormatInfo formatInfo, Type dataType) {
			if(formatInfo == null || string.IsNullOrEmpty(formatInfo.FormatString)
					|| formatInfo.FormatString.Contains("{") || formatInfo.FormatString.Contains("#")) {
				return PivotDisplayValueFormatter.GetEditMask(dataType);
			}
			switch(formatInfo.FormatString[0]) {
				case 'c':
				case 'C':
				case 'd':
				case 'D':
				case 'f':
				case 'F':
				case 'g':
				case 'G':
				case 'n':
				case 'N':
				case 'p':
				case 'P':
					return formatInfo.FormatString;
				default:
					break;
			}
			return string.Empty;
		}
		const int InnerMargin = 5;
		const int TextMargin = 8;
		const int OuterMargin = 12;
		const int BottomMargin = 7;
		const int LegendMargin = 2;
		const int TextEditWidth = 60;
		int comboBoxHeight;
		int labelHeight;
		int checkBoxHeight;
		int textEditHeight;
		readonly ISummaryFilterController filterController;
		SummaryFilterRangeControl rangeControl;
		CheckEdit ceSpecificLevel;
		SummaryFilterComboBoxEdit cbColumn;
		SummaryFilterComboBoxEdit cbRow;
		LabelControl lblRangeFrom;
		LabelControl lblRangeTo;
		TextEdit teEnd;
		TextEdit teStart;
		LabelControl lblRow;
		LabelControl lblColumn;
		LegendLabelControl legendFiltered;
		LegendLabelControl legendUnfiltered;
		PivotSummaryFilterPanel() { }
		public PivotSummaryFilterPanel(ISummaryFilterController filter) {
			this.filterController = filter;
			this.BorderStyle = BorderStyles.NoBorder;
			CreateControls();
			SubscribeEvents();
			AddControls();
			UpdateControlsStyle();
		}
		protected ISummaryFilterController FilterController { get { return filterController; } }
		void CreateControls() {
			this.rangeControl = new SummaryFilterRangeControl(FilterController);
			this.ceSpecificLevel = new CheckEdit();
			this.ceSpecificLevel.Checked = FilterController.Mode == PivotSummaryFilterMode.SpecificLevel;
			this.cbColumn = new SummaryFilterComboBoxEdit(true);
			this.cbRow = new SummaryFilterComboBoxEdit(false);
			this.teStart = new TextEdit();
			this.teStart.EditValue = rangeControl.SelectedRange.Minimum;
			this.lblRangeFrom = new LabelControl();
			this.lblRangeTo = new LabelControl();
			this.teEnd = new TextEdit();
			this.teEnd.EditValue = rangeControl.SelectedRange.Maximum;
			this.lblRow = new LabelControl();
			this.lblColumn = new LabelControl();
			this.legendFiltered = new LegendLabelControl();
			this.legendUnfiltered = new LegendLabelControl();
		}
		void AddControls() {
			Controls.Add(lblRangeFrom);
			Controls.Add(lblRangeTo);
			Controls.Add(teStart);
			Controls.Add(teEnd);
			Controls.Add(rangeControl);
			Controls.Add(ceSpecificLevel);
			Controls.Add(lblRow);
			Controls.Add(lblColumn);
			Controls.Add(cbRow);
			Controls.Add(cbColumn);
			Controls.Add(legendFiltered);
			Controls.Add(legendUnfiltered);
		}
		void UpdateControlsStyle() {
			foreach(Control control in Controls) {
				ISupportLookAndFeel skinedControl = control as ISupportLookAndFeel;
				if(skinedControl != null) {
					skinedControl.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				}
			}
		}
		void DisposeControls() {
			foreach(Control control in Controls) {
				IDisposable disposableControl = control as IDisposable;
				if(disposableControl == null) continue;
				disposableControl.Dispose();
			}
		}
		internal bool AllowMouseClick(Control control) {
			return control == this || Contains(control) ||
				cbRow.ContainsControl(control) || cbColumn.ContainsControl(control);
		}
		#region Contols Initialization
		internal void InitControls() {
			InitHeights();
			InitRangeTextEditsArea();
			InitRangeControl();
			InitRangeHistogramLegend();
			InitCheckEdits();
			InitComboboxLabels();
			InitComboBoxes();
			UpdateControls();
			RightToLeftControls();
		}
		void RightToLeftControls() {
			if(IsRightToLeft) {
				foreach(Control control in Controls) {
					control.Bounds = new Rectangle(
						Right - control.Right,
						 control.Top,
						 control.Width,
						 control.Height
						);
					if(control.Anchor.HasFlag(AnchorStyles.Left) && !control.Anchor.HasFlag(AnchorStyles.Right)) {
						control.Anchor &= ~AnchorStyles.Left;
						control.Anchor |= AnchorStyles.Right;
					} else if(control.Anchor.HasFlag(AnchorStyles.Right) && !control.Anchor.HasFlag(AnchorStyles.Left)) {
						control.Anchor &= ~AnchorStyles.Right;
						control.Anchor |= AnchorStyles.Left;
					}
				}
			}
		}
		void InitHeights() {
			labelHeight = lblRangeFrom.CalcBestSize().Height;
			comboBoxHeight = cbColumn.CalcBestSize().Height;
			checkBoxHeight = ceSpecificLevel.CalcBestSize().Height;
			textEditHeight = teStart.CalcBestSize().Height;
		}
		void InitRangeTextEditsArea() {
			lblRangeFrom.SuspendLayout();
			try {
				lblRangeFrom.Name = "lblRangeFrom";
				lblRangeFrom.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterRangeFrom);
				lblRangeFrom.Width = lblRangeFrom.CalcBestSize().Width;
				lblRangeFrom.Location = new Point(OuterMargin, OuterMargin + (textEditHeight - labelHeight) / 2);
			} finally {
				lblRangeFrom.ResumeLayout();
			}
			teStart.SuspendLayout();
			try {
				teStart.Name = "teStart";
				teStart.Location = new Point(lblRangeFrom.Right + TextMargin, OuterMargin);
				teStart.Width = TextEditWidth;
				teStart.CausesValidation = true;
				teStart.Properties.ValidateOnEnterKey = true;
			} finally {
				teStart.ResumeLayout();
			}
			lblRangeTo.SuspendLayout();
			try {
				lblRangeTo.Name = "lblRangeTo";
				lblRangeTo.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterRangeTo);
				lblRangeFrom.Width = lblRangeTo.CalcBestSize().Width;
				lblRangeTo.Location = new Point(teStart.Right + TextMargin, OuterMargin + (textEditHeight - labelHeight) / 2);
			} finally {
				lblRangeTo.ResumeLayout();
			}
			teEnd.SuspendLayout();
			try {
				teEnd.Name = "teEnd";
				teEnd.Location = new Point(lblRangeTo.Right + TextMargin, OuterMargin);
				teEnd.Width = TextEditWidth;
				teEnd.CausesValidation = true;
				teEnd.Properties.ValidateOnEnterKey = true;
			} finally {
				teEnd.ResumeLayout();
			}
		}
		void InitRangeControl() {
			rangeControl.SuspendLayout();
			try {
				rangeControl.Name = "rangeControl";
				rangeControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				rangeControl.Location = new Point(OuterMargin, textEditHeight + 2 * OuterMargin);
				rangeControl.Size = new Size(Width - 2 * OuterMargin, Height - 5 * OuterMargin - InnerMargin - BottomMargin - textEditHeight - 2 * comboBoxHeight - labelHeight);
			} finally {
				rangeControl.ResumeLayout();
			}
		}
		void InitRangeHistogramLegend() {
			legendUnfiltered.SuspendLayout();
			int legendTop = rangeControl.Bottom + LegendMargin;
			try {
				legendUnfiltered.Name = "legendUnfiltered";
				legendUnfiltered.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				legendUnfiltered.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterLegendHidden);
				legendUnfiltered.LegendColor = rangeControl.UnfilteredColor;
				legendUnfiltered.Location = new Point(Right - OuterMargin - legendUnfiltered.Width, legendTop);
				legendUnfiltered.Width = legendUnfiltered.CalcBestSize().Width;
			} finally {
				legendUnfiltered.ResumeLayout();
			}
			legendFiltered.SuspendLayout();
			try {
				legendFiltered.Name = "legendFiltered";
				legendFiltered.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
				legendFiltered.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterLegendVisible);
				legendFiltered.LegendColor = rangeControl.FilteredColor;
				legendFiltered.Location = new Point(legendUnfiltered.Left - OuterMargin - legendFiltered.Width, legendTop);
				legendFiltered.Width = legendFiltered.CalcBestSize().Width;
			} finally {
				legendFiltered.ResumeLayout();
			}
		}
		void InitCheckEdits() {
			ceSpecificLevel.SuspendLayout();
			try {
				ceSpecificLevel.Name = "ceSpecificLevel";
				ceSpecificLevel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				ceSpecificLevel.Location = new Point(OuterMargin, Height - InnerMargin - OuterMargin - BottomMargin - comboBoxHeight - checkBoxHeight);
				ceSpecificLevel.Properties.Caption = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterApplyToSpecificLevel);
				ceSpecificLevel.Width = ceSpecificLevel.CalcBestSize().Width;
			} finally {
				ceSpecificLevel.ResumeLayout();
			}
		}
		void InitComboboxLabels() {
			lblColumn.SuspendLayout();
			lblRow.SuspendLayout();
			try {
				int labelLeft = ceSpecificLevel.Right + 5 * TextMargin;
				lblColumn.Name = "lblColumn";
				lblColumn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				lblColumn.AutoSize = true;
				lblColumn.Location = new Point(labelLeft, Height - comboBoxHeight - OuterMargin - BottomMargin + (cbColumn.Height - labelHeight) / 2);
				lblColumn.Text = PivotGridLocalizer.Active.GetLocalizedString(PivotGridStringId.SummaryFilterColumnField);
				lblColumn.Width = lblColumn.CalcBestSize().Width;
				lblRow.Name = "lblRow";
				lblRow.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				lblRow.AutoSize = true;
				lblRow.GetTextBaselineY();
				lblRow.Location = new Point(labelLeft, Height - 2 * comboBoxHeight - OuterMargin - BottomMargin - InnerMargin + (cbRow.Height - labelHeight) / 2);
				lblRow.Text = PivotGridLocalizer.GetString(PivotGridStringId.SummaryFilterRowField);
				lblRow.Width = lblRow.CalcBestSize().Width;
			} finally {
				lblColumn.ResumeLayout();
				lblRow.ResumeLayout();
			}
		}
		void InitComboBoxes() {
			cbColumn.SuspendLayout();
			cbRow.SuspendLayout();
			try {
				int comboboxTop = Height - comboBoxHeight - OuterMargin - BottomMargin;
				int comboboxLeft = Math.Max(lblRow.Right, lblColumn.Right) + TextMargin;
				int comboboxwidth = Right - OuterMargin - comboboxLeft;
				cbColumn.Name = "cbColumn";
				cbColumn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				cbColumn.Location = new Point(comboboxLeft, comboboxTop);
				cbColumn.Width = comboboxwidth;
				cbColumn.CausesValidation = true;
				cbColumn.Properties.ValidateOnEnterKey = true;
				cbRow.Name = "cbRow";
				cbRow.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				cbRow.Location = new Point(comboboxLeft, comboboxTop - InnerMargin - comboBoxHeight);
				cbRow.Width = comboboxwidth;
				cbRow.CausesValidation = true;
				cbRow.Properties.ValidateOnEnterKey = true;
			} finally {
				cbColumn.ResumeLayout();
				cbRow.ResumeLayout();
			}
		}
		#endregion
		void UpdateControls() {
			lblRow.Enabled = lblColumn.Enabled = cbRow.Enabled = cbColumn.Enabled = ceSpecificLevel.Checked = FilterController.Mode == PivotSummaryFilterMode.SpecificLevel;
			cbRow.UpdateState(FilterController);
			cbColumn.UpdateState(FilterController);
			UpdateTextEditValue(teStart, FilterController.StartValue);
			UpdateTextEditValue(teEnd, FilterController.EndValue);
			rangeControl.UpdateRange();
			UpdateTextEditFormat(teStart, rangeControl.DataType);
			UpdateTextEditFormat(teEnd, rangeControl.DataType);
		}
		void UpdateTextEditValue(TextEdit textEdit, object value) {
			textEdit.Properties.LockEvents();
			textEdit.EditValue = value;
			textEdit.Properties.UnLockEvents();
		}
		void UpdateTextEditFormat(TextEdit textEdit, Type dataType) {
			textEdit.Properties.LockEvents();
			FormatInfo formatInfo = FilterController.GetFormatInfo(null);
			textEdit.Properties.DisplayFormat.Assign(formatInfo);
			textEdit.Properties.Mask.MaskType = MaskType.Numeric;
			textEdit.Properties.Mask.EditMask = GetEditMask(formatInfo, dataType);
			textEdit.Properties.UnLockEvents();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeControls();
				this.rangeControl = null;
				this.ceSpecificLevel = null;
				this.cbColumn = null;
				this.cbRow = null;
				this.teStart = null;
				this.teEnd = null;
				this.lblRow = null;
				this.lblColumn = null;
				this.legendFiltered = null;
				this.legendUnfiltered = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void SubscribeEvents() {
			FilterController.Updated += OnFilterControllerUpdated;
			teStart.EditValueChanged += OnStartEditValueChanged;
			teEnd.EditValueChanged += OnEndEditValueChanged;
			teStart.Properties.Validating += OnValidatingControl;
			teEnd.Properties.Validating += OnValidatingControl;
			rangeControl.RangeChanged += OnRangeControlRangeChanged;
			ceSpecificLevel.CheckedChanged += OnSpecificLevelCheckedChanged;
			cbRow.SelectedIndexChanged += OnComboboxSelectedIndexChanged;
			cbColumn.SelectedIndexChanged += OnComboboxSelectedIndexChanged;
			cbRow.Properties.Validating += OnValidatingControl;
			cbColumn.Properties.Validating += OnValidatingControl;
		}
		protected virtual void UnsubscribeEvents() {
			FilterController.Updated -= OnFilterControllerUpdated;
			teStart.EditValueChanged -= OnStartEditValueChanged;
			teEnd.EditValueChanged -= OnEndEditValueChanged;
			teStart.Properties.Validating -= OnValidatingControl;
			teEnd.Properties.Validating -= OnValidatingControl;
			rangeControl.RangeChanged -= OnRangeControlRangeChanged;
			ceSpecificLevel.CheckedChanged -= OnSpecificLevelCheckedChanged;
			cbRow.SelectedIndexChanged -= OnComboboxSelectedIndexChanged;
			cbColumn.SelectedIndexChanged -= OnComboboxSelectedIndexChanged;
			cbRow.Properties.Validating -= OnValidatingControl;
			cbColumn.Properties.Validating -= OnValidatingControl;
		}
		void OnFilterControllerUpdated(object sender, EventArgs e) {
			UpdateControls();
		}
		void OnStartEditValueChanged(object sender, EventArgs e) {
			FilterController.StartValue = PivotValueTypeConverter.ConvertTo(teStart.EditValue, rangeControl.DataType);
		}
		void OnEndEditValueChanged(object sender, EventArgs e) {
			FilterController.EndValue = PivotValueTypeConverter.ConvertTo(teEnd.EditValue, rangeControl.DataType);
		}
		void OnValidatingControl(object sender, CancelEventArgs e) {
			e.Cancel = IsControlInvalid(sender);
		}
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			BeginInvoke(new MethodInvoker(() => DoValidateControl()));
		}
		void DoValidateControl() {
			BaseEdit invalidControl = GetInvalidControl();
			if(invalidControl == null) return;
			bool isModified = invalidControl.IsModified;
			invalidControl.IsModified = true;
			try {
				invalidControl.DoValidate();
			} finally {
				invalidControl.IsModified = isModified;
			}
			invalidControl.Focus();
		}
		BaseEdit GetInvalidControl() {
			BaseEdit[] controls = new BaseEdit[] { teStart, teEnd, cbRow, cbColumn };
			foreach(BaseEdit control in controls) {
				if(IsControlInvalid(control))
					return control;
			}
			return null;
		}
		bool IsControlInvalid(object control) {
			return ((control == teStart || control == teEnd) && (FilterController.Validity == PivotSummaryFilterValidity.InvalidRange))
				|| ((control == cbRow || control == cbColumn) && (FilterController.Validity == PivotSummaryFilterValidity.InvalidLevel));
		}
		void OnSpecificLevelCheckedChanged(object sender, EventArgs e) {
			FilterController.BeginUpdate();
			FilterController.Mode = ceSpecificLevel.Checked ? PivotSummaryFilterMode.SpecificLevel : PivotSummaryFilterMode.LastLevel; ;
			FilterController.StartValue = FilterController.EndValue = null;
			FilterController.EndUpdate();
		}
		void OnRangeControlRangeChanged(object sender, RangeControlRangeEventArgs range) {
			FilterController.BeginUpdate();
			FilterController.StartValue = range.Range.Minimum;
			FilterController.EndValue = range.Range.Maximum;
			FilterController.EndUpdate();
		}
		void OnComboboxSelectedIndexChanged(object sender, EventArgs e) {
			FilterController.BeginUpdate();
			FilterController.SetLevelFields(cbRow.SelectedItem as PivotGridFieldBase, cbColumn.SelectedItem as PivotGridFieldBase);
			FilterController.StartValue = FilterController.EndValue = null;
			FilterController.EndUpdate();
		}
		public void ClearFilter() {
			FilterController.Clear();
		}
	}
	class SummaryFilterComboBoxEdit : ComboBoxEdit {
		readonly bool isColumn;
		SummaryFilterComboBoxEdit() { }
		internal SummaryFilterComboBoxEdit(bool isColumn) {
			this.isColumn = isColumn;
			Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		internal void UpdateState(ISummaryFilterController filter) {
			IList<PivotGridFieldBase> fields = filter.GetAreaFields(isColumn);
			PivotGridFieldBase activeField = filter.GetLevelField(isColumn);
			Properties.Items.Clear();
			Properties.Items.Add(PivotGridLocalizer.GetString(PivotGridStringId.GrandTotal));
			for(int i = 0; i < fields.Count; i++)
				Properties.Items.Add(fields[i]);
			Properties.LockEvents();
			try {
				SelectedIndex = activeField == null ? 0 : fields.IndexOf(activeField) + 1;
			} finally {
				Properties.UnLockEvents();
			}
		}
		public bool ContainsControl(Control control) {
			bool res = base.Contains(control);
			if(!res && PopupForm != null)
				return PopupForm.Contains(control);
			return res;
		}
	}
}
