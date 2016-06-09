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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DevExpress.Xpf.Editors;
using System.Windows.Shapes;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Office.Utils;
using DevExpress.Office.Design.Internal;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class FloatingObjectLayoutFormControl : UserControl, IDialogContent {
		#region Fields
		readonly FloatingObjectLayoutOptionsFormController controller;
		readonly List<ColumnsPresetControl> presetControls = new List<ColumnsPresetControl>();
		#endregion
		#region Properties
		public FloatingObjectLayoutOptionsFormController Controller { get { return controller; } }
		#endregion
		#region (Un)SubscribeEvents
		protected internal virtual void SubscribeControlsEvents() {
			chkHorizontalPositionAlignment.Checked += OnHorizontalPositionAlignmentRadioButtonChecked;
			chkHorizontalAbsolutePosition.Checked += OnHorizontalAbsolutePositionRadioButtonChecked;
			chkVerticalPositionAlignment.Checked += OnVerticalPositionAlignmentRadioButtonChecked;
			chkVerticalAbsolutePosition.Checked += OnVerticalAbsolutePositionRadioButtonChecked;
			edtVerticalAbsolutePosition.EditValueChanged += OnEdtVerticalAbsolutePositionEditValueChanged;
			edtHorizontalAbsolutePosition.EditValueChanged += OnEdtHorizontalAbsolutePositionEditValueChanged;
			cbVerticalAbsolutePositionBelow.SelectedIndexChanged += OnCbVerticalAbsolutePositionBelowSelectedIndexChanged;
			cbVerticalPositionType.SelectedIndexChanged += OnCbVerticalPositionTypeSelectedIndexChanged;
			cbVerticalAlignment.SelectedIndexChanged += OnCbVerticalAlignmentSelectedIndexChanged;
			cbHorizontalAbsolutePositionRightOf.SelectedIndexChanged += OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged;
			cbHorizontalPositionType.SelectedIndexChanged += OnCbHorizontalPositionTypeSelectedIndexChanged;
			cbHorizontalAlignment.SelectedIndexChanged += OnCbHorizontalAlignmentSelectedIndexChanged;
			edtTop.EditValueChanged += OnEdtTopEditValueChanged;
			edtBottom.EditValueChanged += OnEdtBottomEditValueChanged;
			edtLeft.EditValueChanged += OnEdtLeftEditValueChanged;
			edtRight.EditValueChanged += OnEdtRightEditValueChanged;
			chkBoth.Checked += OnBothSidesWrapTextRadioButtonChecked;
			chkLeft.Checked += OnLeftOnlyWrapTextRadioButtonChecked;
			chkRight.Checked += OnRightOnlyWrapTextRadioButtonChecked;
			chkLargest.Checked += OnLargestOnlyWrapTextRadioButtonChecked;
			chkLockAspectRatio.Checked += OnChkLockAspectRatioChecked;
			chkLockAspectRatio.Unchecked += OnChkLockAspectRatioUnchecked;
			spnRotation.EditValueChanged += OnSpnRotationEditValueChanged;
			edtHeightAbsolute.Validate += edtHeightWidthAbsoluteValidate;
			edtWidthAbsolute.Validate += edtHeightWidthAbsoluteValidate;
			edtHeightAbsolute.ValueChanged += OnSpnHeightAbsoluteValueChanged;
			edtWidthAbsolute.ValueChanged += OnSpnWidthAbsoluteValueChanged;
			btnReset.Click += OnBtnResetClick;
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged += ColumnsPresetChecked;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			chkHorizontalPositionAlignment.Checked -= OnHorizontalPositionAlignmentRadioButtonChecked;
			chkHorizontalAbsolutePosition.Checked -= OnHorizontalAbsolutePositionRadioButtonChecked;
			chkVerticalPositionAlignment.Checked -= OnVerticalPositionAlignmentRadioButtonChecked;
			chkVerticalAbsolutePosition.Checked -= OnVerticalAbsolutePositionRadioButtonChecked;
			edtVerticalAbsolutePosition.EditValueChanged -= OnEdtVerticalAbsolutePositionEditValueChanged;
			edtHorizontalAbsolutePosition.EditValueChanged -= OnEdtHorizontalAbsolutePositionEditValueChanged;
			cbVerticalAbsolutePositionBelow.SelectedIndexChanged -= OnCbVerticalAbsolutePositionBelowSelectedIndexChanged;
			cbVerticalPositionType.SelectedIndexChanged -= OnCbVerticalPositionTypeSelectedIndexChanged;
			cbVerticalAlignment.SelectedIndexChanged -= OnCbVerticalAlignmentSelectedIndexChanged;
			cbHorizontalAbsolutePositionRightOf.SelectedIndexChanged -= OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged;
			cbHorizontalPositionType.SelectedIndexChanged -= OnCbHorizontalPositionTypeSelectedIndexChanged;
			cbHorizontalAlignment.SelectedIndexChanged -= OnCbHorizontalAlignmentSelectedIndexChanged;
			edtTop.EditValueChanged -= OnEdtTopEditValueChanged;
			edtBottom.EditValueChanged -= OnEdtBottomEditValueChanged;
			edtLeft.EditValueChanged -= OnEdtLeftEditValueChanged;
			edtRight.EditValueChanged -= OnEdtRightEditValueChanged;
			chkBoth.Checked -= OnBothSidesWrapTextRadioButtonChecked;
			chkLeft.Checked -= OnLeftOnlyWrapTextRadioButtonChecked;
			chkRight.Checked -= OnRightOnlyWrapTextRadioButtonChecked;
			chkLargest.Checked -= OnLargestOnlyWrapTextRadioButtonChecked;
			chkLockAspectRatio.Checked -= OnChkLockAspectRatioChecked;
			chkLockAspectRatio.Unchecked -= OnChkLockAspectRatioUnchecked;
			spnRotation.EditValueChanged -= OnSpnRotationEditValueChanged;
			edtHeightAbsolute.Validate -= edtHeightWidthAbsoluteValidate;
			edtWidthAbsolute.Validate -= edtHeightWidthAbsoluteValidate;
			edtHeightAbsolute.ValueChanged -= OnSpnHeightAbsoluteValueChanged;
			edtWidthAbsolute.ValueChanged -= OnSpnWidthAbsoluteValueChanged;
			btnReset.Click -= OnBtnResetClick;
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged -= ColumnsPresetChecked;
		}
		#endregion (Un)SubscribeEvents
		#region InitForm
		private FloatingObjectLayoutFormControl() {
			InitializeComponent();
		}
		public FloatingObjectLayoutFormControl(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			this.tabControl.SelectedIndex = (int)controllerParameters.InitialTabPage;
			InitializeForm();
			this.tabControl.SelectedContainer = tabWrapping; 
			UpdateForm();
		}
		protected internal virtual FloatingObjectLayoutOptionsFormController CreateController(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters) {
			return new FloatingObjectLayoutOptionsFormController(controllerParameters);
		}
		void InitializeForm() {
			InitializeFormPresetControls();
			InitializeFormHorizontalAlignmentComboBox();
			InitializeFormHorizontalPositionTypeComboBox(cbHorizontalPositionType.Items);
			InitializeFormHorizontalPositionTypeComboBox(cbHorizontalAbsolutePositionRightOf.Items);
			InitializeFormVerticalAlignmentComboBox();
			InitializeFormVerticalPositionTypeComboBox(cbVerticalPositionType.Items, false);
			InitializeFormVerticalPositionTypeComboBox(cbVerticalAbsolutePositionBelow.Items, true);
		}
		protected internal virtual void InitializeFormPresetControls() {
			InitializeFormAddPresetControl(columnsPresetControlSquare, new FloatingObjectSquareTextWrapTypePreset());
			InitializeFormAddPresetControl(columnsPresetControlTight, new FloatingObjectTightTextWrapTypePreset());
			InitializeFormAddPresetControl(columnsPresetControlThought, new FloatingObjectThroughTextWrapTypePreset());
			InitializeFormAddPresetControl(columnsPresetControlTopAndBottom, new FloatingObjectTopAndBottomTextWrapTypePreset());
			InitializeFormAddPresetControl(columnsPresetControlBehindText, new FloatingObjectBehindTextWrapTypePreset());
			InitializeFormAddPresetControl(columnsPresetControlInFrontOfText, new FloatingObjectInFrontOfTextWrapTypePreset());
		}
		protected internal virtual void InitializeFormAddPresetControl(ColumnsPresetControl presetControl, TextWrapTypeInfoPreset preset) {
			presetControl.Tag = preset;
#if SL
			presetControl.ImageSource = preset.Image.Source;
#else
			BitmapImage imageSource = new BitmapImage();
			imageSource.BeginInit();
			imageSource.StreamSource = preset.ImageStream;
			imageSource.EndInit();
			imageSource.Freeze();
			presetControl.ImageSource = imageSource;
#endif
			presetControls.Add(presetControl);
		}
		void InitializeFormHorizontalAlignmentComboBox() {
			ListItemCollection items = cbHorizontalAlignment.Items;
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
		void InitializeFormAddItemInHorizontalPositionAlignmentComboBox(ListItemCollection items, FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment) {
			FloatingObjectHorizontalPositionAlignmentEditItem editItem = new FloatingObjectHorizontalPositionAlignmentEditItem();
			editItem.DisplayText = XtraRichEditLocalizer.GetString(controller.HorizontalPositionAlignmentTable[horizontalPositionAlignment]);
			editItem.HorizontalPositionAlignment = horizontalPositionAlignment;
			items.Add(editItem);
		}
		void InitializeFormHorizontalPositionTypeComboBox(ListItemCollection items) {
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
		void InitializeFormAddItemToHorizontalPositionTypeComboBox(ListItemCollection items, FloatingObjectHorizontalPositionType horizontalPositionType) {
			FloatingObjectHorizontalPositionTypeEditItem editItem = new FloatingObjectHorizontalPositionTypeEditItem();
			editItem.DisplayText = XtraRichEditLocalizer.GetString(controller.HorizontalPositionTypeTable[horizontalPositionType]);
			editItem.HorizontalPositionType = horizontalPositionType;
			items.Add(editItem);
		}
		void InitializeFormVerticalAlignmentComboBox() {
			ListItemCollection items = cbVerticalAlignment.Items;
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
		void InitializeFormAddItemInVerticalPositionAlignmentComboBox(ListItemCollection items, FloatingObjectVerticalPositionAlignment verticalPositionAlignment) {
			FloatingObjectVerticalPositionAlignmentEditItem editItem = new FloatingObjectVerticalPositionAlignmentEditItem();
			editItem.DisplayText = XtraRichEditLocalizer.GetString(controller.VerticalPositionAlignmentTable[verticalPositionAlignment]);
			editItem.VerticalPositionAlignment = verticalPositionAlignment;
			items.Add(editItem);
		}
		void InitializeFormVerticalPositionTypeComboBox(ListItemCollection items, bool useVerticalPositionTypeParagraph) {
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
		void InitializeFormAddItemToVerticalPositionTypeComboBox(ListItemCollection items, FloatingObjectVerticalPositionType verticalPositionType) {
			FloatingObjectVerticalPositionTypeEditItem editItem = new FloatingObjectVerticalPositionTypeEditItem();
			editItem.DisplayText = XtraRichEditLocalizer.GetString(controller.VerticalPositionTypeTable[verticalPositionType]);
			editItem.VerticalPositionType = verticalPositionType;
			items.Add(editItem);
		}
		#endregion InitForm
		#region Update
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			} finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			UpdateTabPagePosition();
			UpdateTabPageTextWrapping();
			UpdateTabPageSize();
		}
		void UpdateTabPagePosition() {
			if (controller.TextWrapType == FloatingObjectTextWrapType.Inline) {
				tabPosition.IsEnabled = false;
			}
			else {
				tabPosition.IsEnabled = true;
				UpdateTabPagePositionHorizontalControls();
				UpdateTabPagePositionVerticalControls();
				chkLockAnchor.IsChecked = controller.Locked;
			}
		}
		void UpdateTabPagePositionHorizontalControls() {
			UpdateTabPagePositionSelectedIndex(cbHorizontalAlignment, controller.HorizontalPositionAlignment, FloatingObjectHorizontalPositionAlignment.Left);
			UpdateTabPagePositionSelectedIndex(cbHorizontalPositionType, controller.HorizontalPositionType, controller.HorizontalPositionType);
			UpdateTabPagePositionSelectedIndex(cbHorizontalAbsolutePositionRightOf, controller.HorizontalPositionType, controller.HorizontalPositionType);
			bool isAbsolutePosition = controller.IsHorizontalAbsolutePosition;
			chkHorizontalPositionAlignment.IsChecked = !isAbsolutePosition;
			chkHorizontalAbsolutePosition.IsChecked = isAbsolutePosition;
			EnableHorizontalPositionControls(!isAbsolutePosition, isAbsolutePosition);
			if (controller.OffsetX.HasValue && (controller.HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None))
				InitializeRichTextIndentEdit(edtHorizontalAbsolutePosition, true, controller.OffsetX.Value);
		}
		void UpdateTabPagePositionVerticalControls() {
			UpdateTabPagePositionSelectedIndex(cbVerticalAlignment, controller.VerticalPositionAlignment, FloatingObjectVerticalPositionAlignment.Top);
			UpdateTabPagePositionSelectedIndex(cbVerticalPositionType, controller.VerticalPositionType, FloatingObjectVerticalPositionType.Page);
			UpdateTabPagePositionSelectedIndex(cbVerticalAbsolutePositionBelow, controller.VerticalPositionType, controller.VerticalPositionType);
			bool isAbsolutePosition = controller.IsVerticalAbsolutePosition;
			chkVerticalPositionAlignment.IsChecked = !isAbsolutePosition;
			chkVerticalAbsolutePosition.IsChecked = isAbsolutePosition;
			EnableVerticalPositionControls(!isAbsolutePosition, isAbsolutePosition);
			if (controller.OffsetY.HasValue && (controller.VerticalPositionAlignment == FloatingObjectVerticalPositionAlignment.None))
				InitializeRichTextIndentEdit(edtVerticalAbsolutePosition, true, controller.OffsetY.Value);
		}
		void UpdateTabPagePositionSelectedIndex(ComboBoxEdit comboBoxEdit, object value, object defaultValue) {
			ListItemCollection items = comboBoxEdit.Items;
			UpdateTabPagePositionSelectedIndex(comboBoxEdit, value, items);
			if (comboBoxEdit.SelectedIndex == -1)
				UpdateTabPagePositionSelectedIndex(comboBoxEdit, defaultValue, items);
		}
		void UpdateTabPagePositionSelectedIndex(ComboBoxEdit comboBoxEdit, object value, ListItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				IEditItem item = (IEditItem)items[i];
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
			UpdateTabPageTextWrappingRadioGroupWrapTextChecked();
			UpdateTabPageTextWrappingDistanceControls();
		}
		void UpdateTabPageTextWrappingRadioGroupWrapTextChecked() {
			if (controller.TextWrapSide.HasValue)
				switch (controller.TextWrapSide.Value) {
					case FloatingObjectTextWrapSide.Both:
						chkBoth.IsChecked = true;
						break;
					case FloatingObjectTextWrapSide.Left:
						chkLeft.IsChecked = true;
						break;
					case FloatingObjectTextWrapSide.Right:
						chkRight.IsChecked = true;
						break;
					case FloatingObjectTextWrapSide.Largest:
						chkLargest.IsChecked = true;
						break;
				}
		}
		void UpdateTabPageTextWrappingDistanceControls() {
			if (controller.TopDistance.HasValue)
				InitializeRichTextIndentEdit(edtTop, false, controller.TopDistance.Value);
			if (controller.BottomDistance.HasValue)
				InitializeRichTextIndentEdit(edtBottom, false, controller.BottomDistance.Value);
			if (controller.LeftDistance.HasValue)
				InitializeRichTextIndentEdit(edtLeft, false, controller.LeftDistance.Value);
			if (controller.RightDistance.HasValue)
				InitializeRichTextIndentEdit(edtRight, false, controller.RightDistance.Value);
		}
		void UpdateTabPageSize() {
			chkLockAspectRatio.IsChecked = controller.LockAspectRatio;
			InitializeRichTextIndentEdit(edtHeightAbsolute, false, controller.ActualHeight);
			InitializeRichTextIndentEdit(edtWidthAbsolute, false, controller.ActualWidth);
			lblOriginalHeight.Text = controller.OriginalHeightAsString();
			lblOriginalWidth.Text = controller.OriginalWidthAsString();
			if (controller.TextWrapType == FloatingObjectTextWrapType.Inline)
				spnRotation.Value = 0;
			else
				spnRotation.Value = (controller.Rotation.HasValue ? controller.DocumentModel.UnitConverter.ModelUnitsToDegree(controller.Rotation.Value) : 0);
		}
		#endregion Update
		#region otherFuncs
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				Controller.ApplyChanges();
			}
		}
		void InitializeRichTextIndentEdit(RichTextIndentEdit tiEdit, bool allowNegativeValues, int value) {
			FloatingObjectRichTextIndentEditProperties properties = controller.FloatingObjectRichTextIndentEditProperties;
			tiEdit.MinValue = properties.GetMinValue(allowNegativeValues);
			tiEdit.MaxValue = properties.MaxValue;
			tiEdit.DefaultUnitType = properties.DefaultUnitType;
			tiEdit.ValueUnitConverter = properties.ValueUnitConverter;
			tiEdit.Value = value;
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
		void EnableHorizontalPositionControls(bool enableAlignmentRow, bool enableAbsolutePositionRow) {
			cbHorizontalAlignment.IsEnabled = enableAlignmentRow;
			cbHorizontalPositionType.IsEnabled = enableAlignmentRow;
			EnableLabel(lblHorizontalPositionType, enableAlignmentRow);
			edtHorizontalAbsolutePosition.IsEnabled = enableAbsolutePositionRow;
			cbHorizontalAbsolutePositionRightOf.IsEnabled = enableAbsolutePositionRow;
			EnableLabel(lblHorizontalAbsolutePosition, enableAbsolutePositionRow);
		}
		void EnableLabel(TextBlock textBlock, bool isEnabled) {
#if !SL
			textBlock.IsEnabled = isEnabled;
#endif
		}
		void EnableVerticalPositionControls(bool enableAlignmentRow, bool enableAbsolutePositionRow) {
			cbVerticalAlignment.IsEnabled = enableAlignmentRow;
			cbVerticalPositionType.IsEnabled = enableAlignmentRow;
			EnableLabel(lblVerticalPositionType, enableAlignmentRow);
			edtVerticalAbsolutePosition.IsEnabled = enableAbsolutePositionRow;
			cbVerticalAbsolutePositionBelow.IsEnabled = enableAbsolutePositionRow;
			EnableLabel(lblVerticalAbsolutePosition, enableAbsolutePositionRow);
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
		FloatingObjectLocation GetFloatingObjectHorizontalLocation() {
			if (!controller.HorizontalPositionAlignment.HasValue || !controller.HorizontalPositionType.HasValue)
				return null;
			FloatingObjectLocation location = new FloatingObjectLocation();
			location.HorizontalPositionAlignment = controller.HorizontalPositionAlignment.Value;
			location.HorizontalPositionType = controller.HorizontalPositionType.Value;
			location.ActualWidth = controller.ActualWidth;
			location.OffsetX = (int)edtHorizontalAbsolutePosition.Value;
			return location;
		}
		FloatingObjectLocation GetFloatingObjectVerticalLocation() {
			if (!controller.VerticalPositionAlignment.HasValue || !controller.VerticalPositionType.HasValue)
				return null;
			FloatingObjectLocation location = new FloatingObjectLocation();
			location.VerticalPositionAlignment = controller.VerticalPositionAlignment.Value;
			location.VerticalPositionType = controller.VerticalPositionType.Value;
			location.ActualWidth = controller.ActualWidth;
			location.OffsetX = (int)edtVerticalAbsolutePosition.Value;
			return location;
		}
		void WrapTextAndDistanceControlsEnabled(bool isHorizontalControlsEnabled, bool isVerticalControlsEnabled) {
			edtLeft.IsEnabled = isHorizontalControlsEnabled;
			edtRight.IsEnabled = isHorizontalControlsEnabled;
			chkBoth.IsEnabled = isHorizontalControlsEnabled;
			chkLeft.IsEnabled = isHorizontalControlsEnabled;
			chkRight.IsEnabled = isHorizontalControlsEnabled;
			chkLargest.IsEnabled = isHorizontalControlsEnabled;
			edtTop.IsEnabled = isVerticalControlsEnabled;
			edtBottom.IsEnabled = isVerticalControlsEnabled;
		}
		#endregion otherFuncs
		#region Events
		void OnHorizontalPositionAlignmentRadioButtonChecked(object sender, EventArgs e) {
			controller.IsHorizontalAbsolutePosition = false;
			EnableHorizontalPositionControls(true, false);
			OnCbHorizontalAlignmentSelectedIndexChanged(this, EventArgs.Empty);
			OnCbHorizontalPositionTypeSelectedIndexChanged(this, EventArgs.Empty);
		}
		void OnHorizontalAbsolutePositionRadioButtonChecked(object sender, EventArgs e) {
			controller.IsHorizontalAbsolutePosition = true;
			EnableHorizontalPositionControls(false, true);
			OnEdtHorizontalAbsolutePositionEditValueChanged(this, EventArgs.Empty);
			OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged(this, EventArgs.Empty);
		}
		void OnCbHorizontalAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			controller.HorizontalPositionAlignment = ((FloatingObjectHorizontalPositionAlignmentEditItem)cbHorizontalAlignment.SelectedItem).HorizontalPositionAlignment;
		}
		void OnCbHorizontalPositionTypeSelectedIndexChanged(object sender, EventArgs e) {
			controller.HorizontalPositionType = ((FloatingObjectHorizontalPositionTypeEditItem)cbHorizontalPositionType.SelectedItem).HorizontalPositionType;
		}
		void OnCbHorizontalAbsolutePositionRightOfSelectedIndexChanged(object sender, EventArgs e) {
			int offsetX = 0;
			FloatingObjectHorizontalPositionType newHorizontalPositionType = ((FloatingObjectHorizontalPositionTypeEditItem)cbHorizontalAbsolutePositionRightOf.SelectedItem).HorizontalPositionType;
			if ((newHorizontalPositionType != FloatingObjectHorizontalPositionType.Character) && (controller.HorizontalPositionType != FloatingObjectHorizontalPositionType.Character))
				offsetX = CalculateOffsetX(newHorizontalPositionType);
			controller.HorizontalPositionType = newHorizontalPositionType;
			if (offsetX != Int32.MinValue) {
				edtHorizontalAbsolutePosition.Value = offsetX;
				OnEdtHorizontalAbsolutePositionEditValueChanged(this, EventArgs.Empty);
			}
			else
				edtHorizontalAbsolutePosition.Value = -1;
		}
		void OnEdtHorizontalAbsolutePositionEditValueChanged(object sender, EventArgs e) {
			controller.HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.None;
			controller.OffsetX = (int)edtHorizontalAbsolutePosition.Value;
		}
		void OnVerticalPositionAlignmentRadioButtonChecked(object sender, EventArgs e) {
			controller.IsVerticalAbsolutePosition = false;
			EnableVerticalPositionControls(true, false);
			OnCbVerticalAlignmentSelectedIndexChanged(this, EventArgs.Empty);
			OnCbVerticalPositionTypeSelectedIndexChanged(this, EventArgs.Empty);
		}
		void OnVerticalAbsolutePositionRadioButtonChecked(object sender, EventArgs e) {
			controller.IsVerticalAbsolutePosition = true;
			EnableVerticalPositionControls(false, true);
			OnEdtVerticalAbsolutePositionEditValueChanged(this, EventArgs.Empty);
			OnCbVerticalAbsolutePositionBelowSelectedIndexChanged(this, EventArgs.Empty);
		}
		void OnCbVerticalAlignmentSelectedIndexChanged(object sender, EventArgs e) {
			controller.VerticalPositionAlignment = ((FloatingObjectVerticalPositionAlignmentEditItem)cbVerticalAlignment.SelectedItem).VerticalPositionAlignment;
		}
		void OnCbVerticalPositionTypeSelectedIndexChanged(object sender, EventArgs e) {
			controller.VerticalPositionType = ((FloatingObjectVerticalPositionTypeEditItem)cbVerticalPositionType.SelectedItem).VerticalPositionType;
		}
		void OnCbVerticalAbsolutePositionBelowSelectedIndexChanged(object sender, EventArgs e) {
			int offsetY = 0;
			FloatingObjectVerticalPositionType newVerticalPositionType = ((FloatingObjectVerticalPositionTypeEditItem)cbVerticalAbsolutePositionBelow.SelectedItem).VerticalPositionType;
			if ((newVerticalPositionType != FloatingObjectVerticalPositionType.Line) && (controller.VerticalPositionType != FloatingObjectVerticalPositionType.Line))
				offsetY = CalculateOffsetY(newVerticalPositionType);
			controller.VerticalPositionType = newVerticalPositionType;
			if (offsetY != Int32.MinValue) {
				edtVerticalAbsolutePosition.Value = offsetY;
				OnEdtVerticalAbsolutePositionEditValueChanged(this, EventArgs.Empty);
			}
			else
				edtVerticalAbsolutePosition.Value = -1;
		}
		void OnEdtVerticalAbsolutePositionEditValueChanged(object sender, EventArgs e) {
			controller.VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.None;
			controller.OffsetY = (int)edtVerticalAbsolutePosition.Value;
		}
		void cheLock_CheckedChanged(object sender, EventArgs e) {
			controller.Locked = chkLockAnchor.IsChecked;
		}
		protected internal virtual void ColumnsPresetChecked(object sender, EventArgs e) {
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
		void OnEdtTopEditValueChanged(object sender, EventArgs e) {
			controller.TopDistance = (int)edtTop.Value;
		}
		void OnEdtBottomEditValueChanged(object sender, EventArgs e) {
			controller.BottomDistance = (int)edtBottom.Value;
		}
		void OnEdtLeftEditValueChanged(object sender, EventArgs e) {
			controller.LeftDistance = (int)edtLeft.Value;
		}
		void OnEdtRightEditValueChanged(object sender, EventArgs e) {
			controller.RightDistance = (int)edtRight.Value;
		}
		void OnLargestOnlyWrapTextRadioButtonChecked(object sender, RoutedEventArgs e) {
			Controller.TextWrapSide = FloatingObjectTextWrapSide.Largest;
		}
		void OnRightOnlyWrapTextRadioButtonChecked(object sender, RoutedEventArgs e) {
			Controller.TextWrapSide = FloatingObjectTextWrapSide.Right;
		}
		void OnLeftOnlyWrapTextRadioButtonChecked(object sender, RoutedEventArgs e) {
			Controller.TextWrapSide = FloatingObjectTextWrapSide.Left;
		}
		void OnBothSidesWrapTextRadioButtonChecked(object sender, RoutedEventArgs e) {
			Controller.TextWrapSide = FloatingObjectTextWrapSide.Both;
		}
		void OnSpnHeightAbsoluteValueChanged(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			bool aspectRatioChecked = chkLockAspectRatio.IsChecked.HasValue ? chkLockAspectRatio.IsChecked.Value : false;
			controller.RecalculateSizeDependingOnHeight(aspectRatioChecked, edtHeightAbsolute.Value ?? 0);
			if (controller.ActualWidth > 0)
				edtWidthAbsolute.Value = controller.ActualWidth;
			SubscribeControlsEvents();
		}
		void OnSpnWidthAbsoluteValueChanged(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			bool aspectRatioChecked = chkLockAspectRatio.IsChecked.HasValue ? chkLockAspectRatio.IsChecked.Value : false;
			controller.RecalculateSizeDependingOnWidth(aspectRatioChecked, edtWidthAbsolute.Value ?? 0);
			if (controller.ActualHeight > 0)
				edtHeightAbsolute.Value = controller.ActualHeight;
			SubscribeControlsEvents();
		}
		void OnBtnResetClick(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			controller.ResetActualHeight();
			edtHeightAbsolute.Value = controller.ActualHeight;
			controller.ResetActualWidth();
			edtWidthAbsolute.Value = controller.ActualWidth;
			controller.ResetRotation();
			spnRotation.Value = controller.DocumentModel.UnitConverter.ModelUnitsToDegree(controller.Rotation.HasValue ? controller.Rotation.Value : 0);
			SubscribeControlsEvents();
		}
		void edtHeightWidthAbsoluteValidate(object sender, ValidationEventArgs e) {
			e.IsValid = controller.StringToTwips(e.Value as String) > 0;
		}
		void OnChkLockAspectRatioChecked(object sender, EventArgs e) {
			controller.LockAspectRatio = chkLockAspectRatio.IsChecked ?? false;
			if (controller.LockAspectRatio) {
				OnSpnHeightAbsoluteValueChanged(null, EventArgs.Empty);
				OnSpnWidthAbsoluteValueChanged(null, EventArgs.Empty);
			}
		}
		void OnChkLockAspectRatioUnchecked(object sender, EventArgs e) {
			controller.LockAspectRatio = chkLockAspectRatio.IsChecked ?? false;
			if (controller.LockAspectRatio) {
				OnSpnHeightAbsoluteValueChanged(null, EventArgs.Empty);
				OnSpnWidthAbsoluteValueChanged(null, EventArgs.Empty);
			}
		}
		void OnSpnRotationEditValueChanged(object sender, EventArgs e) {
			controller.Rotation = controller.DocumentModel.UnitConverter.DegreeToModelUnits((int)spnRotation.Value);
			UnsubscribeControlsEvents();
			try {
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
		#endregion Events
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			this.ApplyChanges();
		}
		void IDialogContent.OnOk() {
			this.ApplyChanges();
		}
		#endregion
	}
	public interface IEditItem {
		object Value { get; }
		string DisplayText { get; set; }
	}
	public class FloatingObjectVerticalPositionTypeEditItem : IEditItem {
		FloatingObjectVerticalPositionType verticalPositionType;
		string displayText;
		public FloatingObjectVerticalPositionType VerticalPositionType { get { return verticalPositionType; } set { verticalPositionType = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return verticalPositionType; } }
	}
	public class FloatingObjectVerticalPositionAlignmentEditItem : IEditItem {
		FloatingObjectVerticalPositionAlignment verticalPositionAlignment;
		string displayText;
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get { return verticalPositionAlignment; } set { verticalPositionAlignment = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return verticalPositionAlignment; } }
	}
	public class FloatingObjectHorizontalPositionTypeEditItem : IEditItem {
		FloatingObjectHorizontalPositionType horizontalPositionType;
		string displayText;
		public FloatingObjectHorizontalPositionType HorizontalPositionType { get { return horizontalPositionType; } set { horizontalPositionType = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return horizontalPositionType; } }
	}
	public class FloatingObjectHorizontalPositionAlignmentEditItem : IEditItem {
		FloatingObjectHorizontalPositionAlignment horizontalPositionAlignment;
		string displayText;
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get { return horizontalPositionAlignment; } set { horizontalPositionAlignment = value; } }
		public string DisplayText { get { return displayText; } set { displayText = value; } }
		public object Value { get { return horizontalPositionAlignment; } }
	}
}
