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
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Design.Internal;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.tabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.tabPagePosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.tabPageTextWrapping")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.tabPageSize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.rgHorizontal")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.rgVertical")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbVerticalAbsolutePositionBelow")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbVerticalPositionType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbVerticalAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbHorizontalAbsolutePositionRightOf")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbHorizontalPositionType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.cbHorizontalAlignment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.chkLock")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlSquare")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlBehind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlInFrontOf")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlThought")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlTopAndBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.columnsPresetControlTight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.rgTextWrapSide")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.chkLockAspectRatio")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnVerticalAbsolutePosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnHorizontalAbsolutePosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnBottom")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.spnTop")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblHorizontalPositionType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblVerticalPositionType")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblHorizontalAbsolutePosition")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.FloatingObjectLayoutForm.lblVerticalAbsolutePosition")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class FloatingObjectLayoutForm : DevExpress.XtraEditors.XtraForm {
		#region Fields
		readonly FloatingObjectLayoutOptionsFormController controller;
		readonly List<ColumnsPresetControl> presetControls = new List<ColumnsPresetControl>();
		Dictionary<FloatingObjectTextWrapSide, int> textWrapSideTable;
		#endregion Fields
		#region Properties
		public FloatingObjectLayoutOptionsFormController Controller { get { return controller; } }
		#endregion Properties
		#region (Un)SubscribeEvents
		protected internal virtual void SubscribeControlsEvents() {
			btnOk.Click += OnBtnOkClick;
			btnCancel.Click += OnBtnCancelClick;
			rgHorizontal.SelectedIndexChanged += OnRgHorizontalSelectedIndexChanged;
			rgVertical.SelectedIndexChanged += OnRgVerticalSelectedIndexChanged;
			cbVerticalAbsolutePositionBelow.SelectedIndexChanged += OnCbVerticalAbsolutePositionBelowSelectedIndexChanged;
			cbVerticalPositionType.SelectedIndexChanged += OnCbVerticalPositionTypeSelectedIndexChanged;
			cbVerticalAlignment.SelectedIndexChanged += OnCbVerticalAlignmentSelectedIndexChanged;
			cbHorizontalAbsolutePositionRightOf.SelectedIndexChanged += OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged;
			cbHorizontalPositionType.SelectedIndexChanged += OnCbHorizontalPositionTypeSelectedIndexChanged;
			cbHorizontalAlignment.SelectedIndexChanged += OnCbHorizontalAlignmentSelectedIndexChanged;
			spnVerticalAbsolutePosition.ValueChanged += OnSpnVerticalAbsolutePositionValueChanged;
			spnHorizontalAbsolutePosition.ValueChanged += OnSpnHorizontalAbsolutePositionValueChanged;
			chkLock.CheckedChanged += chkLock_CheckedChanged;
			rgTextWrapSide.SelectedIndexChanged += OnRgTextWrapSideSelectedIndexChanged;
			spnTop.ValueChanged += OnSpnTopValueChanged;
			spnBottom.ValueChanged += OnSpnBottomValueChanged;
			spnLeft.ValueChanged += OnSpnLeftValueChanged;
			spnRight.ValueChanged += OnSpnRightValueChanged;
			spnRotation.ValueChanged += OnSpnRotationValueChanged;
			spnHeightAbs.ValueChanged += OnSpnHeightAbsoluteValueChanged;
			spnWidthAbs.ValueChanged += OnSpnWidthAbsoluteValueChanged;
			chkLockAspectRatio.CheckedChanged += OnChkLockAspectRatioCheckedChanged;
			btnReset.Click += OnBtnResetClick;
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged += ColumnsPresetChecked;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			btnOk.Click -= OnBtnOkClick;
			btnCancel.Click -= OnBtnCancelClick;
			rgHorizontal.SelectedIndexChanged -= OnRgHorizontalSelectedIndexChanged;
			rgVertical.SelectedIndexChanged -= OnRgVerticalSelectedIndexChanged;
			cbVerticalAbsolutePositionBelow.SelectedIndexChanged -= OnCbVerticalAbsolutePositionBelowSelectedIndexChanged;
			cbVerticalPositionType.SelectedIndexChanged -= OnCbVerticalPositionTypeSelectedIndexChanged;
			cbVerticalAlignment.SelectedIndexChanged -= OnCbVerticalAlignmentSelectedIndexChanged;
			cbHorizontalAbsolutePositionRightOf.SelectedIndexChanged -= OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged;
			cbHorizontalPositionType.SelectedIndexChanged -= OnCbHorizontalPositionTypeSelectedIndexChanged;
			cbHorizontalAlignment.SelectedIndexChanged -= OnCbHorizontalAlignmentSelectedIndexChanged;
			spnVerticalAbsolutePosition.ValueChanged -= OnSpnVerticalAbsolutePositionValueChanged;
			spnHorizontalAbsolutePosition.ValueChanged -= OnSpnHorizontalAbsolutePositionValueChanged;
			chkLock.CheckedChanged -= chkLock_CheckedChanged;
			rgTextWrapSide.SelectedIndexChanged -= OnRgTextWrapSideSelectedIndexChanged;
			spnTop.ValueChanged -= OnSpnTopValueChanged;
			spnBottom.ValueChanged -= OnSpnBottomValueChanged;
			spnLeft.ValueChanged -= OnSpnLeftValueChanged;
			spnRight.ValueChanged -= OnSpnRightValueChanged;
			spnRotation.ValueChanged -= OnSpnRotationValueChanged;
			spnHeightAbs.ValueChanged -= OnSpnHeightAbsoluteValueChanged;
			spnWidthAbs.ValueChanged -= OnSpnWidthAbsoluteValueChanged;
			chkLockAspectRatio.CheckedChanged -= OnChkLockAspectRatioCheckedChanged;
			btnReset.Click -= OnBtnResetClick;
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged -= ColumnsPresetChecked;
		}
		#endregion (Un)SubscribeEvents
		#region InitForm
		protected FloatingObjectLayoutForm(){
			InitializeComponent();
		}
		public FloatingObjectLayoutForm(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			this.textWrapSideTable = CreateTextWrapSideTable();
			InitializeForm(); 
			this.tabControl.SelectedTabPageIndex = (int)controllerParameters.InitialTabPage;
			UpdateForm();
		}
		protected internal virtual FloatingObjectLayoutOptionsFormController CreateController(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters) {
			return new FloatingObjectLayoutOptionsFormController(controllerParameters);
		}
		Dictionary<FloatingObjectTextWrapSide, int> CreateTextWrapSideTable() {
			Dictionary<FloatingObjectTextWrapSide, int> table = new Dictionary<FloatingObjectTextWrapSide, int>();
			table.Add(FloatingObjectTextWrapSide.Both, 0);
			table.Add(FloatingObjectTextWrapSide.Left, 1);
			table.Add(FloatingObjectTextWrapSide.Right, 2);
			table.Add(FloatingObjectTextWrapSide.Largest, 3);
			return table;
		}
		void InitializeForm() {
			InitializeFormInitPresetControls(); 
			InitializeFormHorizontalAlignmentComboBox();
			InitializeFormFillHorizontalPositionTypeComboBox(cbHorizontalPositionType.Properties.Items);
			InitializeFormFillHorizontalPositionTypeComboBox(cbHorizontalAbsolutePositionRightOf.Properties.Items);
			InitializeFormFillVerticalAlignmentComboBox();
			InitializeFormFillVerticalPositionTypeComboBox(cbVerticalPositionType.Properties.Items, false);
			InitializeFormFillVerticalPositionTypeComboBox(cbVerticalAbsolutePositionBelow.Properties.Items, true);
		}
		protected internal virtual void InitializeFormInitPresetControls() {
			InitializeFormInitPresetControlsAdd(columnsPresetControlSquare, new FloatingObjectSquareTextWrapTypePreset());
			InitializeFormInitPresetControlsAdd(columnsPresetControlTight, new FloatingObjectTightTextWrapTypePreset());
			InitializeFormInitPresetControlsAdd(columnsPresetControlThought, new FloatingObjectThroughTextWrapTypePreset());
			InitializeFormInitPresetControlsAdd(columnsPresetControlTopAndBottom, new FloatingObjectTopAndBottomTextWrapTypePreset());
			InitializeFormInitPresetControlsAdd(columnsPresetControlBehind, new FloatingObjectBehindTextWrapTypePreset());
			InitializeFormInitPresetControlsAdd(columnsPresetControlInFrontOf, new FloatingObjectInFrontOfTextWrapTypePreset());
		}
		protected internal virtual void InitializeFormInitPresetControlsAdd(ColumnsPresetControl presetControl, TextWrapTypeInfoPreset preset) {
			presetControl.Tag = preset;
			presetControls.Add(presetControl);
		}
		void InitializeFormHorizontalAlignmentComboBox() {
			ComboBoxItemCollection items = cbHorizontalAlignment.Properties.Items;
			items.BeginUpdate();
			try {
				InitializeFormAddItemInHorizontalPositionAlignmentComboBox(items, FloatingObjectHorizontalPositionAlignment.Left);
				InitializeFormAddItemInHorizontalPositionAlignmentComboBox(items, FloatingObjectHorizontalPositionAlignment.Center);
				InitializeFormAddItemInHorizontalPositionAlignmentComboBox(items, FloatingObjectHorizontalPositionAlignment.Right);
			}
			finally {
				items.EndUpdate();
			}
		}
		void InitializeFormFillHorizontalPositionTypeComboBox(ComboBoxItemCollection items) {
			items.BeginUpdate();
			try {
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.Margin);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.Page);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.Column);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.Character);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.LeftMargin);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.RightMargin);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.InsideMargin);
				InitializeFormAddItemToHorizontalPositionTypeComboBox(items, FloatingObjectHorizontalPositionType.OutsideMargin);
			}
			finally {
				items.EndUpdate();
			}
		}
		void InitializeFormFillVerticalAlignmentComboBox() {
			ComboBoxItemCollection items = cbVerticalAlignment.Properties.Items;
			items.BeginUpdate();
			try {
				InitializeFormAddItemInVerticalPositionAlignmentComboBox(items, FloatingObjectVerticalPositionAlignment.Top);
				InitializeFormAddItemInVerticalPositionAlignmentComboBox(items, FloatingObjectVerticalPositionAlignment.Center);
				InitializeFormAddItemInVerticalPositionAlignmentComboBox(items, FloatingObjectVerticalPositionAlignment.Bottom);
				InitializeFormAddItemInVerticalPositionAlignmentComboBox(items, FloatingObjectVerticalPositionAlignment.Inside);
				InitializeFormAddItemInVerticalPositionAlignmentComboBox(items, FloatingObjectVerticalPositionAlignment.Outside);
			}
			finally {
				items.EndUpdate();
			}
		}
		void InitializeFormFillVerticalPositionTypeComboBox(ComboBoxItemCollection items, bool useVerticalPositionTypeParagraph) {
			items.BeginUpdate();
			try {
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.Margin);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.Page);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.Line);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.TopMargin);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.BottomMargin);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.InsideMargin);
				InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.OutsideMargin);
				if (useVerticalPositionTypeParagraph)
					InitializeFormAddItemToVerticalPositionTypeComboBox(items, FloatingObjectVerticalPositionType.Paragraph);
			}
			finally {
				items.EndUpdate();
			}
		}
		void InitializeFormAddItemInHorizontalPositionAlignmentComboBox(ComboBoxItemCollection items, FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment) {
			items.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(controller.HorizontalPositionAlignmentTable[horizontalPositionAlignment]), horizontalPositionAlignment, -1));
		}
		void InitializeFormAddItemToHorizontalPositionTypeComboBox(ComboBoxItemCollection items, FloatingObjectHorizontalPositionType horizontalPositionType) {
			items.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(controller.HorizontalPositionTypeTable[horizontalPositionType]), horizontalPositionType, -1));
		}
		void InitializeFormAddItemInVerticalPositionAlignmentComboBox(ComboBoxItemCollection items, FloatingObjectVerticalPositionAlignment verticalPositionAlignment) {
			items.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(controller.VerticalPositionAlignmentTable[verticalPositionAlignment]), verticalPositionAlignment, -1));
		}
		void InitializeFormAddItemToVerticalPositionTypeComboBox(ComboBoxItemCollection items, FloatingObjectVerticalPositionType verticalPositionType) {
			items.Add(new ImageComboBoxItem(XtraRichEditLocalizer.GetString(controller.VerticalPositionTypeTable[verticalPositionType]), verticalPositionType, -1));
		}
		#endregion InitForm
		#region UpdateTabs
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
			tabControl.BeginUpdate();
			try {
				UpdateTabPagePosition();
				UpdateTabPageTextWrapping();
				UpdateTabPageSize();
			}
			finally {
				tabControl.EndUpdate();
			}
		}
		void UpdateTabPagePosition() {
			if (controller.TextWrapType == FloatingObjectTextWrapType.Inline) {
				tabControl.TabPages[0].PageEnabled = false;
			}
			else {
				tabControl.TabPages[0].PageEnabled = true;
				UpdateTabPagePositionHorizontalControls();
				UpdateTabPagePositionVerticalControls();
				chkLock.Checked = (controller.Locked == true);
			}
		}
		void UpdateTabPagePositionVerticalControls() {
			UpdateTabPagePositionUpdateSelectedIndex(cbVerticalAlignment, controller.VerticalPositionAlignment, FloatingObjectVerticalPositionAlignment.Top);
			UpdateTabPagePositionUpdateSelectedIndex(cbVerticalPositionType, controller.VerticalPositionType, FloatingObjectVerticalPositionType.Page);
			UpdateTabPagePositionUpdateSelectedIndex(cbVerticalAbsolutePositionBelow, controller.VerticalPositionType, controller.VerticalPositionType);
			rgVertical.SelectedIndex = controller.VerticalPositionAlignment != FloatingObjectVerticalPositionAlignment.None ? 0 : 1;
			EnableVerticalPositionControls(rgVertical.SelectedIndex == 0, rgVertical.SelectedIndex != 0);
			UpdateRichTextIndentEdit(spnVerticalAbsolutePosition, true, controller.OffsetY ?? 0);
		}
		void UpdateTabPagePositionHorizontalControls() {
			UpdateTabPagePositionUpdateSelectedIndex(cbHorizontalAlignment, controller.HorizontalPositionAlignment, FloatingObjectHorizontalPositionAlignment.Left);
			UpdateTabPagePositionUpdateSelectedIndex(cbHorizontalPositionType, controller.HorizontalPositionType, controller.HorizontalPositionType);
			UpdateTabPagePositionUpdateSelectedIndex(cbHorizontalAbsolutePositionRightOf, controller.HorizontalPositionType, controller.HorizontalPositionType);
			if (controller.HorizontalPositionAlignment.HasValue)
				rgHorizontal.SelectedIndex = controller.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None ? 1 : 0;
			else
				rgHorizontal.Enabled = false;
			EnableHorizontalPositionControls(rgHorizontal.SelectedIndex == 0, rgHorizontal.SelectedIndex != 0);
			UpdateRichTextIndentEdit(spnHorizontalAbsolutePosition, true, controller.OffsetX ?? 0);
		}
		void UpdateTabPagePositionUpdateSelectedIndex(ImageComboBoxEdit comboBoxEdit, object value, object defaultValue) {
			ImageComboBoxItemCollection items = comboBoxEdit.Properties.Items;
			UpdateTabPagePositionUpdateSelectedIndex(comboBoxEdit, value, items);
			if (comboBoxEdit.SelectedIndex == -1)
				UpdateTabPagePositionUpdateSelectedIndex(comboBoxEdit, defaultValue, items);
		}
		void UpdateTabPagePositionUpdateSelectedIndex(ImageComboBoxEdit comboBoxEdit, object value, ImageComboBoxItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				ImageComboBoxItem item = items[i];
				if (Object.Equals(item.Value, value)) {
					comboBoxEdit.SelectedIndex = i;
					break;
				}
			}
		}
		void UpdateTabPageTextWrapping() {
			if (controller.TextWrapType == FloatingObjectTextWrapType.Inline)
				WrapTextAndDistanceControlsEnabled(false, false);
			else
				WrapTextAndDistanceControlsEnabled(true, true);
			PresetControlsChecked();
			if (controller.TextWrapSide.HasValue)
				rgTextWrapSide.SelectedIndex = textWrapSideTable[controller.TextWrapSide.Value];
			UpdateTabPageTextWrappingDistanceControls();
		}
		void UpdateTabPageTextWrappingDistanceControls() {
			UpdateRichTextIndentEdit(spnTop, false, controller.TopDistance ?? 0);
			UpdateRichTextIndentEdit(spnBottom, false, controller.BottomDistance ?? 0);
			UpdateRichTextIndentEdit(spnLeft, false, controller.LeftDistance ?? 0);
			UpdateRichTextIndentEdit(spnRight, false, controller.RightDistance ?? 0);
		}
		void UpdateTabPageSize() {
			chkLockAspectRatio.Checked = controller.LockAspectRatio;
			UpdateRichTextIndentEdit(spnHeightAbs, false, controller.ActualHeight);
			UpdateRichTextIndentEdit(spnWidthAbs, false, controller.ActualWidth);
			lblOriginalSizeHeightValue.Text = controller.OriginalHeightAsString();
			lblOriginalSizeWidthValue.Text = controller.OriginalWidthAsString();
			if (controller.TextWrapType == FloatingObjectTextWrapType.Inline)
				spnRotation.Value = 0;
			else
				spnRotation.Value = (controller.Rotation.HasValue ? controller.DocumentModel.UnitConverter.ModelUnitsToDegree(controller.Rotation.Value) : 0);
		}
		void UpdateRichTextIndentEdit(RichTextIndentEdit tiEdit, bool allowNegativeValues, int value) {
			FloatingObjectRichTextIndentEditProperties properties = controller.FloatingObjectRichTextIndentEditProperties;
			tiEdit.Properties.MinValue = properties.GetMinValue(allowNegativeValues);
			tiEdit.Properties.MaxValue = properties.MaxValue;
			tiEdit.Properties.DefaultUnitType = properties.DefaultUnitType;
			tiEdit.ValueUnitConverter = properties.ValueUnitConverter;
			tiEdit.Value = value;
		}
		#endregion UpdateTabs
		#region otherFuncs
		void EnableHorizontalPositionControls(bool enableAlignmentRow, bool enableAbsolutePositionRow) {
			cbHorizontalAlignment.Enabled = enableAlignmentRow;
			cbHorizontalPositionType.Enabled = enableAlignmentRow;
			lblHorizontalPositionType.Enabled = enableAlignmentRow;
			spnHorizontalAbsolutePosition.Enabled = enableAbsolutePositionRow;
			cbHorizontalAbsolutePositionRightOf.Enabled = enableAbsolutePositionRow;
			lblHorizontalAbsolutePosition.Enabled = enableAbsolutePositionRow;
		}
		void EnableVerticalPositionControls(bool enableAlignmentRow, bool enableAbsolutePositionRow) {
			cbVerticalAlignment.Enabled = enableAlignmentRow;
			cbVerticalPositionType.Enabled = enableAlignmentRow;
			lblVerticalPositionType.Enabled = enableAlignmentRow;
			spnVerticalAbsolutePosition.Enabled = enableAbsolutePositionRow;
			cbVerticalAbsolutePositionBelow.Enabled = enableAbsolutePositionRow;
			lblVerticalAbsolutePosition.Enabled = enableAbsolutePositionRow;
		}
		int GetXCoordinate() {
			if (!controller.OffsetX.HasValue)
				return Int32.MinValue;
			int offset = controller.ToDocumentLayoutUnitConverter.ToLayoutUnits(controller.OffsetX.Value);
			FloatingObjectTargetPlacementInfo placementInfo = controller.PlacementInfo;
			switch (controller.HorizontalPositionType) {
				case FloatingObjectHorizontalPositionType.LeftMargin:
				case FloatingObjectHorizontalPositionType.InsideMargin:
				case FloatingObjectHorizontalPositionType.Page:
					return placementInfo.PageBounds.Left + offset;
				case FloatingObjectHorizontalPositionType.Column:
					return placementInfo.ColumnBounds.Left + offset;
				case FloatingObjectHorizontalPositionType.Margin:
					return placementInfo.PageClientBounds.Left + offset;
				case FloatingObjectHorizontalPositionType.OutsideMargin:
				case FloatingObjectHorizontalPositionType.RightMargin:
					return placementInfo.PageClientBounds.Right + offset;
				case FloatingObjectHorizontalPositionType.Character:
					return placementInfo.OriginX + offset;
				default:
					Exceptions.ThrowInternalException();
					return Int32.MinValue;
			}
		}
		int GetYCoordinate() {
			if (!controller.OffsetY.HasValue)
				return Int32.MinValue;
			int offset = controller.ToDocumentLayoutUnitConverter.ToLayoutUnits(controller.OffsetY.Value);
			FloatingObjectTargetPlacementInfo placementInfo = controller.PlacementInfo;
			switch (controller.VerticalPositionType) {
				case FloatingObjectVerticalPositionType.Paragraph:
				case FloatingObjectVerticalPositionType.Line:
					return placementInfo.OriginY + offset;
				case FloatingObjectVerticalPositionType.Page:
				case FloatingObjectVerticalPositionType.OutsideMargin:
				case FloatingObjectVerticalPositionType.InsideMargin:
				case FloatingObjectVerticalPositionType.TopMargin:
					return placementInfo.PageBounds.Y + offset;
				case FloatingObjectVerticalPositionType.BottomMargin:
					return placementInfo.PageBounds.Bottom + offset;
				case FloatingObjectVerticalPositionType.Margin:
					return placementInfo.ColumnBounds.Y + offset;
				default:
					Exceptions.ThrowInternalException();
					return Int32.MinValue;
			}
		}
		int CalculateOffsetX(FloatingObjectHorizontalPositionType newHorizontalPositionType) {
			FloatingObjectHorizontalPositionCalculator calculator = new FloatingObjectHorizontalPositionCalculator(controller.ToDocumentLayoutUnitConverter);
			FloatingObjectLocation location = GetFloatingObjectHorizontalLocation();
			int x = GetXCoordinate();
			if (Object.ReferenceEquals(location, null) || Object.ReferenceEquals(x, null))
				return Int32.MinValue;
			return calculator.CalculateFloatingObjectOffsetX(newHorizontalPositionType, x, controller.PlacementInfo);
		}
		int CalculateOffsetY(FloatingObjectVerticalPositionType newVerticalPositionType) {
			FloatingObjectVerticalPositionCalculator calculator = new FloatingObjectVerticalPositionCalculator(controller.ToDocumentLayoutUnitConverter);
			FloatingObjectLocation location = GetFloatingObjectVerticalLocation();
			int y = GetYCoordinate();
			if (Object.ReferenceEquals(location, null) || Object.ReferenceEquals(y, null))
				return Int32.MinValue;
			return calculator.CalculateFloatingObjectOffsetY(newVerticalPositionType, y, controller.PlacementInfo);
		}
		FloatingObjectLocation GetFloatingObjectHorizontalLocation() {
			if (!controller.HorizontalPositionAlignment.HasValue || !controller.HorizontalPositionType.HasValue)
				return null;
			FloatingObjectLocation location = new FloatingObjectLocation();
			location.HorizontalPositionAlignment = controller.HorizontalPositionAlignment.Value;
			location.HorizontalPositionType = controller.HorizontalPositionType.Value;
			location.ActualWidth = controller.ActualWidth;
			location.OffsetX = (int)spnHorizontalAbsolutePosition.Value;
			return location;
		}
		FloatingObjectLocation GetFloatingObjectVerticalLocation() {
			if (!controller.VerticalPositionAlignment.HasValue || !controller.VerticalPositionType.HasValue)
				return null;
			FloatingObjectLocation location = new FloatingObjectLocation();
			location.VerticalPositionAlignment = controller.VerticalPositionAlignment.Value;
			location.VerticalPositionType = controller.VerticalPositionType.Value;
			location.ActualWidth = controller.ActualWidth;
			location.OffsetX = (int)spnVerticalAbsolutePosition.Value;
			return location;
		}
		void WrapTextAndDistanceControlsEnabled(bool isHorizontalControlsEnabled, bool isVerticalControlsEnabled) {
			spnLeft.Enabled = isHorizontalControlsEnabled;
			lblLeft.Enabled = isHorizontalControlsEnabled;
			spnRight.Enabled = isHorizontalControlsEnabled;
			lblRight.Enabled = isHorizontalControlsEnabled;
			rgTextWrapSide.Enabled = isHorizontalControlsEnabled;
			spnTop.Enabled = isVerticalControlsEnabled;
			lblTop.Enabled = isVerticalControlsEnabled;
			spnBottom.Enabled = isVerticalControlsEnabled;
			lblBottom.Enabled = isVerticalControlsEnabled;
		}
		void PresetControlsChecked() {
			for (int i = 0; i < presetControls.Count; i++) {
				TextWrapTypeInfoPreset textWrapTypePreset = (TextWrapTypeInfoPreset)presetControls[i].Tag;
				bool isSelected;
				if ((controller.TextWrapType == FloatingObjectTextWrapType.None) && (textWrapTypePreset.TextWrapType == FloatingObjectTextWrapType.None))
					isSelected = (controller.IsBehindDocument == textWrapTypePreset.IsBehindDocument);
				else
					isSelected = (textWrapTypePreset.TextWrapType == controller.TextWrapType);
				presetControls[i].Checked = isSelected;
				presetControls[i].TabStop = isSelected;
				if ((controller.TextWrapType == FloatingObjectTextWrapType.Tight) || (controller.TextWrapType == FloatingObjectTextWrapType.Through))
					WrapTextAndDistanceControlsEnabled(true, false);
				else if (controller.TextWrapType == FloatingObjectTextWrapType.Square)
					WrapTextAndDistanceControlsEnabled(true, true);
				else if (controller.TextWrapType == FloatingObjectTextWrapType.TopAndBottom)
					WrapTextAndDistanceControlsEnabled(false, true);
				else if (controller.TextWrapType == FloatingObjectTextWrapType.None)
					WrapTextAndDistanceControlsEnabled(false, false);
			}
		}
		#endregion otherFuncs
		#region Events
		void OnBtnOkClick(object sender, EventArgs e) {
			controller.ApplyChanges();
			DialogResult = DialogResult.OK;
		}
		void OnBtnCancelClick(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
		}
		void OnRgHorizontalSelectedIndexChanged(object sender, EventArgs e) {
			int selectedIndex = rgHorizontal.SelectedIndex;
			bool isHorizontalAbsolutePosition = (selectedIndex != 0);
			controller.IsHorizontalAbsolutePosition = isHorizontalAbsolutePosition;
			EnableHorizontalPositionControls(!isHorizontalAbsolutePosition, isHorizontalAbsolutePosition);
			if (isHorizontalAbsolutePosition) {
				OnSpnHorizontalAbsolutePositionValueChanged(this, EventArgs.Empty);
				OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged(this, EventArgs.Empty);
			}
			else {
				OnCbHorizontalAlignmentSelectedIndexChanged(this, EventArgs.Empty);
				OnCbHorizontalPositionTypeSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		void OnRgVerticalSelectedIndexChanged(object sender, EventArgs e) {
			int selectedIndex = rgVertical.SelectedIndex;
			bool isVerticalAbsolutePosition = (selectedIndex != 0);
			controller.IsVerticalAbsolutePosition = isVerticalAbsolutePosition;
			EnableVerticalPositionControls(!isVerticalAbsolutePosition, isVerticalAbsolutePosition);
			if (isVerticalAbsolutePosition) {
				OnSpnVerticalAbsolutePositionValueChanged(this, EventArgs.Empty);
				OnCbVerticalAbsolutePositionBelowSelectedIndexChanged(this, EventArgs.Empty);
			}
			else {
				OnCbVerticalAlignmentSelectedIndexChanged(this, EventArgs.Empty);
				OnCbVerticalPositionTypeSelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		void OnCbVerticalAbsolutePositionBelowSelectedIndexChanged(object sender, EventArgs e) {
			int offsetY = 0;
			FloatingObjectVerticalPositionType newVerticalPositionType = (FloatingObjectVerticalPositionType)((ImageComboBoxItem)cbVerticalAbsolutePositionBelow.SelectedItem).Value;
			if ((newVerticalPositionType != FloatingObjectVerticalPositionType.Line) && (controller.VerticalPositionType != FloatingObjectVerticalPositionType.Line))
				offsetY = CalculateOffsetY(newVerticalPositionType);
			controller.VerticalPositionType = newVerticalPositionType;
			if (offsetY != Int32.MinValue) {
				spnVerticalAbsolutePosition.Value = offsetY;
				OnSpnVerticalAbsolutePositionValueChanged(this, EventArgs.Empty);
			}
			else
				spnVerticalAbsolutePosition.Value = -1;
		}
		void OnCbVerticalPositionTypeSelectedIndexChanged(object sender, EventArgs e) {
			controller.VerticalPositionType = (FloatingObjectVerticalPositionType)((ImageComboBoxItem)cbVerticalPositionType.SelectedItem).Value;
		}
		void OnCbVerticalAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			controller.VerticalPositionAlignment = (FloatingObjectVerticalPositionAlignment)((ImageComboBoxItem)cbVerticalAlignment.SelectedItem).Value;
		}
		void OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged(object sender, EventArgs e) {
			int offsetX = 0;
			FloatingObjectHorizontalPositionType newHorizontalPositionType = (FloatingObjectHorizontalPositionType)((ImageComboBoxItem)cbHorizontalAbsolutePositionRightOf.SelectedItem).Value;
			if ((newHorizontalPositionType != FloatingObjectHorizontalPositionType.Character) && (controller.HorizontalPositionType != FloatingObjectHorizontalPositionType.Character))
				offsetX = CalculateOffsetX(newHorizontalPositionType);
			controller.HorizontalPositionType = newHorizontalPositionType;
			if (offsetX != Int32.MinValue) {
				spnHorizontalAbsolutePosition.Value = offsetX;
				OnSpnHorizontalAbsolutePositionValueChanged(this, EventArgs.Empty);
			}
			else
				spnHorizontalAbsolutePosition.Value = -1;
		}
		void OnCbHorizontalPositionTypeSelectedIndexChanged(object sender, EventArgs e) {
			controller.HorizontalPositionType = (FloatingObjectHorizontalPositionType)((ImageComboBoxItem)cbHorizontalPositionType.SelectedItem).Value;
		}
		void OnCbHorizontalAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			controller.HorizontalPositionAlignment = (FloatingObjectHorizontalPositionAlignment)((ImageComboBoxItem)cbHorizontalAlignment.SelectedItem).Value;
		}
		void OnSpnVerticalAbsolutePositionValueChanged(object sender, EventArgs e) {
			controller.VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.None;
			controller.OffsetY = spnVerticalAbsolutePosition.Value;
		}
		void OnSpnHorizontalAbsolutePositionValueChanged(object sender, EventArgs e) {
			controller.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.None;
			controller.OffsetX = spnHorizontalAbsolutePosition.Value;
		}
		void chkLock_CheckedChanged(object sender, EventArgs e) {
			controller.Locked = chkLock.Checked;
		}
		void ColumnsPresetChecked(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			try {
				ColumnsPresetControl control = (ColumnsPresetControl)sender;
				TextWrapTypeInfoPreset preset = (TextWrapTypeInfoPreset)control.Tag;
				if ((controller.TextWrapType == FloatingObjectTextWrapType.Inline) && (preset.TextWrapType != FloatingObjectTextWrapType.Inline)) {
					Controller.ApplyPreset(preset);
					UpdateFormCore();
				}
				else {
					Controller.ApplyPreset(preset);
					PresetControlsChecked();
				}
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		void OnRgTextWrapSideSelectedIndexChanged(object sender, EventArgs e) {
			int selectedIndex = rgTextWrapSide.SelectedIndex;
			foreach (FloatingObjectTextWrapSide key in textWrapSideTable.Keys)
				if (textWrapSideTable[key] == selectedIndex)
					controller.TextWrapSide = key;
		}
		void OnSpnTopValueChanged(object sender, EventArgs e) {
			controller.TopDistance = spnTop.Value;
		}
		void OnSpnBottomValueChanged(object sender, EventArgs e) {
			controller.BottomDistance = spnBottom.Value;
		}
		void OnSpnLeftValueChanged(object sender, EventArgs e) {
			controller.LeftDistance = spnLeft.Value;
		}
		void OnSpnRightValueChanged(object sender, EventArgs e) {
			controller.RightDistance = spnRight.Value;
		}
		void OnChkLockAspectRatioCheckedChanged(object sender, EventArgs e) {
			controller.LockAspectRatio = chkLockAspectRatio.Checked;
			OnSpnHeightAbsoluteValueChanged(null, EventArgs.Empty);
			OnSpnWidthAbsoluteValueChanged(null, EventArgs.Empty);
		}
		void OnBtnResetClick(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			controller.ResetActualHeight();
			spnHeightAbs.Value = controller.ActualHeight;
			controller.ResetActualWidth();
			spnWidthAbs.Value = controller.ActualWidth;
			controller.ResetRotation();
			spnRotation.Value = controller.DocumentModel.UnitConverter.ModelUnitsToDegree(controller.Rotation.HasValue ? controller.Rotation.Value : 0);
			SubscribeControlsEvents();
		}
		void OnSpnHeightAbsoluteValueChanged(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			controller.RecalculateSizeDependingOnHeight(chkLockAspectRatio.Checked, spnHeightAbs.Value ?? 0);
			spnWidthAbs.Value = controller.ActualWidth;
			SubscribeControlsEvents();
		}
		void OnSpnWidthAbsoluteValueChanged(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			controller.RecalculateSizeDependingOnWidth(chkLockAspectRatio.Checked, spnWidthAbs.Value ?? 0);
			spnHeightAbs.Value = controller.ActualHeight;
			SubscribeControlsEvents();
		}
		void OnSpnRotationValueChanged(object sender, EventArgs e) {
			int value;
			if (Int32.TryParse(spnRotation.Text.Replace("\u00B0", ""), out value)) {
				UnsubscribeControlsEvents();
				try {
					controller.Rotation = controller.DocumentModel.UnitConverter.DegreeToModelUnits(value);
					if ((controller.Rotation != 0) && (controller.TextWrapType == FloatingObjectTextWrapType.Inline)) {
						UnsubscribeControlsEvents();
						controller.TextWrapType = FloatingObjectTextWrapType.None;
						controller.IsBehindDocument = false;
						UpdateFormCore();
					}
				}
				finally {
					SubscribeControlsEvents();
				}
			}
		}
		#endregion Events
	}
}
