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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Colors;
namespace DevExpress.XtraEditors.ColorPick.Picker {
	[ToolboxItem(false)]
	public partial class HSVModelUserControl : ColorModelUserControlBase, IColorPickerTab {
		public HSVModelUserControl() {
			Appearance.Reset();
			InitializeComponent();
			RestoreSortOptions();
			ErrorProvider = this.dxErrorProvider;
			colorGridArea = SelectGridAreaRect;
			gradientArea = SelectGradientAreaRect;
			opacityArea = SelectOpacityAreaRect;
		}
		Rectangle colorGridArea, gradientArea, opacityArea;
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SubscribeCore();
		}
		protected virtual void SubscribeCore() { }
		protected virtual void UnsubscribeCore() {
			UnsubscribeOnContextualEvents();
		}
		protected void SetDefaultRadioGroupSettings() {
			this.hsbGroup.SelectedIndex = 0;
		}
		#region Base Overrides
		public override Panel OpacityPanel {
			get { return this.pnlOpacity; }
		}
		public override Panel GradientPanel {
			get { return this.pnlGradient; }
		}
		public override Panel ColorGridPanel {
			get { return this.pnlColorGrid; }
		}
		public override TextEdit OpacityTextEdit {
			get { return this.txtOpacity; }
		}
		public override LabelControl OpacityLabel {
			get { return this.lblOpacity; }
		}
		public override BaseControl SampleLabel {
			get { return this.lblOpacity; }
		}
		public override bool ShouldChangeColorGrid() {
			return (txtHue.Focused || txtSaturation.Focused || txtBright.Focused || txtOpacity.Focused);
		}
		public override void ChangeColorGridValue(int delta) {
			TextEdit textEdit = null;
			const double min = 0;
			double max = 255;
			if(txtOpacity.Focused) textEdit = txtOpacity;
			else if(txtHue.Focused) {
				textEdit = txtHue;
				max = 360;
			}
			else if(txtSaturation.Focused) {
				textEdit = txtSaturation;
				max = 100;
			}
			else if(txtBright.Focused) {
				textEdit = txtBright;
				max = 100;
			}
			if(textEdit == null) return;
			double value;
			if(double.TryParse(textEdit.Text, out value)) {
				value += delta;
				if(value > max) {
					if(txtHue.Focused) value -= max;
					else value = max;
				}
				if(value < min) {
					if(txtHue.Focused) value += max;
					else value = min;
				}
				textEdit.Text = value.ToString();
			}
		}
		public override void CalculateColorWheel() {
			HueSatBright hueSatBright = new HueSatBright();
			float width = CalculationWidth, height = CalculationHeight;
			ClearSelectedHsb();
			switch(selectedColorEditOption) {
				case ColorEditOption.Hue:
					hueSatBright.Hue = (float)GradientValue;
					hueSatBright.Saturation = GridXCore / width;
					hueSatBright.Brightness = (height - GridYCore) / height;
					hueSatBright.Alpha = OpacityCore / 255.0;
					SelectedHsb = hueSatBright;
					break;
				case ColorEditOption.Saturation:
					hueSatBright.Saturation = (float)GradientValue;
					hueSatBright.Hue = GridXCore / width;
					hueSatBright.Brightness = (height - GridYCore) / height;
					hueSatBright.Alpha = OpacityCore / 255.0;
					SelectedHsb = hueSatBright;
					break;
				case ColorEditOption.Brightness:
					hueSatBright.Brightness = (float)GradientValue;
					hueSatBright.Hue = GridXCore / width;
					hueSatBright.Saturation = (height - GridYCore) / height;
					hueSatBright.Alpha = OpacityCore / 255.0;
					SelectedHsb = hueSatBright;
					break;
			}
			ColorWheelSelected(hueSatBright);
		}
		public override void UpdateColorGridTextBoxes(HueSatBright hueSatBright) {
			if(ChangingColorGridValuesCore == true)
				return;
			ChangingColorGridValuesCore = true;
			try {
				int hue = (int)Math.Round(360.0 * hueSatBright.Hue);
				hue = hue % 360;
				SetHsbValue(txtHue, hue.ToString());
				double saturation = 100 * hueSatBright.Saturation;
				SetHsbValue(txtSaturation, saturation.ToString("0.0"));
				double brightness = 100 * hueSatBright.Brightness;
				SetHsbValue(txtBright, brightness.ToString("0.0"));
				Color color = hueSatBright.AsRGB;
				SetArgbValue(txtOpacity, color.A);
				if(!ChangingHexTextBox) txtHexadecimal.Text = GetHexadecimalValue(color);
			}
			finally {
				ChangingColorGridValuesCore = false;
			}
		}
		public override ColorModelFormPainterBase CreateFormPainter() {
			return ColorModelFormPainterBase.Create(ColorModel.HSB, this);
		}
		public override SuperToolTipManagerBase CreateSuperTipManager() {
			return SuperToolTipManagerBase.Create(ColorModel.HSB);
		}
		#endregion
		#region Methods
		void RefreshColorGridLabelsAndCursors() {
			HueSatBright selectedHsb = GetSelectedHsb();
			float width = CalculationWidth;
			float height = CalculationHeight;
			switch(selectedColorEditOption) {
				case ColorEditOption.Hue:
					selectedHsb = UpdateColorGridHueBasedCore(selectedHsb, width, height);
					break;
				case ColorEditOption.Saturation:
					selectedHsb = UpdateColorGridSaturationBasedCore(selectedHsb, width, height);
					break;
				case ColorEditOption.Brightness:
					selectedHsb = UpdateColorGridBrightnessBasedCore(selectedHsb, width, height);
					break;
			}
			pnlGradient.Invalidate();
			pnlColorGrid.Invalidate();
		}
		HueSatBright UpdateColorGridBrightnessBasedCore(HueSatBright selectedHsb, float width, float height) {
			GridXLabel = ColorModelSettings.HueAxisName;
			GridYLabel = ColorModelSettings.SaturationAxisName;
			GradientLabel = ColorModelSettings.BrightnessAxisName;
			GridXCore = (int)Math.Round(selectedHsb.Hue * width);
			GridYCore = (int)Math.Round((1.0 - selectedHsb.Saturation) * height);
			GradientValue = selectedHsb.Brightness;
			return selectedHsb;
		}
		HueSatBright UpdateColorGridSaturationBasedCore(HueSatBright selectedHsb, float width, float height) {
			GridXLabel = ColorModelSettings.HueAxisName;
			GridYLabel = ColorModelSettings.BrightnessAxisName;
			GradientLabel = ColorModelSettings.SaturationAxisName;
			GridXCore = (int)Math.Round(selectedHsb.Hue * width);
			GridYCore = (int)Math.Round((1.0 - selectedHsb.Brightness) * height);
			GradientValue = selectedHsb.Saturation;
			return selectedHsb;
		}
		HueSatBright UpdateColorGridHueBasedCore(HueSatBright selectedHsb, float width, float height) {
			GridXLabel = ColorModelSettings.SaturationAxisName;
			GridYLabel = ColorModelSettings.BrightnessAxisName;
			GradientLabel = ColorModelSettings.HueAxisName;
			GridXCore = (int)Math.Round(selectedHsb.Saturation * width);
			GridYCore = (int)Math.Round((1 - selectedHsb.Brightness) * height);
			GradientValue = selectedHsb.Hue;
			return selectedHsb;
		}
		void RestoreSortOptions() {
			switch(SelectedColorEditOption) {
				case ColorEditOption.Hue:
					this.hsbGroup.SelectedIndex = 0;
					break;
				case ColorEditOption.Saturation:
					this.hsbGroup.SelectedIndex = 1;
					break;
				case ColorEditOption.Brightness:
					this.hsbGroup.SelectedIndex = 2;
					break;
			}
		}
		void HSBTextBoxesChanged() {
			if(ChangingColorGridValuesCore || !ForceValidation())
				return;
			SetOpacity(GetInt(txtOpacity, 0, 255));
			double hue = GetDouble(txtHue, 0, 359.99999);
			double saturation = GetDouble(txtSaturation, 0, 100);
			double bright = GetDouble(txtBright, 0, 100);
			SelectedHsb = new HueSatBright(hue / 360, saturation / 100.0, bright / 100.0) { Alpha = OpacityCore / 255.0 };
			SelectedColor = SelectedHsb.AsRGB;
		}
		#endregion
		#region Public Interface
		Color selectedColor;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color SelectedColor {
			get { return selectedColor; }
			set {
				ChangingColorGridValuesCore = true;
				try {
					selectedColor = value;
					SetColorOpacity(selectedColor.A);
					pnlOpacity.Invalidate();
					btnMakeWebSafe.Enabled = !(IsWebSafe(selectedColor));
					if(!PickingWheelColor)
						RefreshColorGridLabelsAndCursors();
					FrmColorPicker.SamplePanel.Refresh();
				}
				finally {
					ChangingColorGridValuesCore = false;
				}
				if(!PickingWheelColor) {
					UpdateColorGridTextBoxes(new HueSatBright(value));
				}
			}
		}
		ColorEditOption selectedColorEditOption = ColorEditOption.Hue;
		public override ColorEditOption SelectedColorEditOption {
			get { return selectedColorEditOption; }
			set {
				selectedColorEditOption = value;
				RefreshColorGridLabelsAndCursors();
			}
		}
		public void SetParentForm(FrmColorPicker form) {
			FrmColorPicker = form;
		}
		public virtual void ApplyProperties() {
			if(FrmColorPicker.Properties == null)
				return;
			ColorDialogOptions options = FrmColorPicker.Properties.ColorDialogOptions;
			ApplyShowMakeWebSafeButtonOption(options);
			ApplyAllowTransparencyOption(options);
		}
		protected virtual void ApplyShowMakeWebSafeButtonOption(ColorDialogOptions options) {
			this.btnMakeWebSafe.Visible = options.ShowMakeWebSafeButton;
		}
		protected virtual void ApplyAllowTransparencyOption(ColorDialogOptions options) {
			this.pnlOpacity.Visible = options.AllowTransparency;
			this.lblOpacityStatic.Visible = options.AllowTransparency;
			this.txtOpacity.Visible = options.AllowTransparency;
			this.lblOpacity.Visible = options.AllowTransparency;
		}
		public virtual void SubscribeOnContextualEvents() {
			if(FrmColorPicker != null)
				FrmColorPicker.SamplePanelPaint += OnSamplePanelPaint;
		}
		public virtual void UnsubscribeOnContextualEvents() {
			if(FrmColorPicker != null)
				FrmColorPicker.SamplePanelPaint -= OnSamplePanelPaint;
		}
		#endregion
		#region Handlers
		void OnBrightTextChanged(object sender, EventArgs e) {
			HSBTextBoxesChanged();
		}
		void OnSaturationTextChanged(object sender, EventArgs e) {
			HSBTextBoxesChanged();
		}
		void OnHueTextChanged(object sender, EventArgs e) {
			HSBTextBoxesChanged();
		}
		void OpacityPanelTextChanged(object sender, EventArgs e) {
			HSBTextBoxesChanged();
		}
		void OnMakeWebSafeBtnClick(object sender, EventArgs e) {
			SelectedColor = MakeWebSafe(SelectedColor);
			HSBTextBoxesChanged();
		}
		void OnOpacityPanelPaint(object sender, PaintEventArgs e) {
			if(FormPainter == null)
				return;
			FormPainter.DrawOpacityPanel(e);
		}
		void OnOpacityPanelMouseDown(object sender, MouseEventArgs e) {
			if(!opacityArea.Contains(e.Location)) return;
			pnlOpacity.Focus();
			rightButtonPressedOpacityPanel = true;
			SetColorOpacityFromMouse(e.X);
		}
		bool rightButtonPressedOpacityPanel = false;
		void OnOpacityPanelMouseMove(object sender, MouseEventArgs e) {
			UpdateCursor(OpacityPanel, opacityArea, Cursors.VSplit, e);
			if(rightButtonPressedOpacityPanel)
				SuperTipManager.RefreshOpacityInfo(this, this.opacityToolTip, e, rightButtonPressedOpacityPanel);
		}
		void OnOpacityPanelMouseUp(object sender, MouseEventArgs e) {
			rightButtonPressedOpacityPanel = false;
		}
		void OnColorGridEnter(object sender, EventArgs e) {
			pnlColorGrid.Invalidate();
			EnableCursor(timer);
		}
		void OnColorGridLeave(object sender, EventArgs e) {
			pnlColorGrid.Invalidate();
			DisableCursor(timer);
		}
		void OnOpacityPanelLeave(object sender, EventArgs e) {
			pnlOpacity.Invalidate();
			DisableCursor(timer);
		}
		void OnOpacityPanelEnter(object sender, EventArgs e) {
			pnlOpacity.Invalidate();
			EnableCursor(timer);
		}
		void OnGradientPanelEnter(object sender, EventArgs e) {
			pnlGradient.Invalidate();
			EnableCursor(timer);
		}
		void OnGradientPanelLeave(object sender, EventArgs e) {
			pnlGradient.Invalidate();
			DisableCursor(timer);
		}
		bool rightButtonPressedGradientPanel = false;
		void OnGradientPanelMouseDown(object sender, MouseEventArgs e) {
			if(!gradientArea.Contains(e.Location)) return;
			pnlGradient.Focus();
			rightButtonPressedGradientPanel = true;
			SetGradientPositionFromMouse(e.Y);
		}
		void OnGradientPanelMouseMove(object sender, MouseEventArgs e) {
			UpdateCursor(GradientPanel, gradientArea, Cursors.HSplit, e);
			if(rightButtonPressedGradientPanel)
				SetGradientPositionFromMouse(e.Y);
		}
		void OnGradientPanelMouseUp(object sender, MouseEventArgs e) {
			rightButtonPressedGradientPanel = false;
		}
		bool rightButtonPressedColorGridPanel = false;
		void OnColorGridMouseDown(object sender, MouseEventArgs e) {
			if(!colorGridArea.Contains(e.Location)) return;
			pnlColorGrid.Focus();
			rightButtonPressedColorGridPanel = true;
			SetGridPositionFromMouse(e.X, e.Y);
		}
		void OnColorGridMouseMove(object sender, MouseEventArgs e) {
			UpdateCursor(ColorGridPanel, colorGridArea, ColorGridCursor, e);
			if(rightButtonPressedColorGridPanel)
				SetGridPositionFromMouse(e.X, e.Y);
		}
		void OnColorGridMouseUp(object sender, MouseEventArgs e) {
			rightButtonPressedColorGridPanel = false;
		}
		void OnSamplePanelPaint(object sender, PaintEventArgs e) {
			DrawColorSwatch(e.Graphics, 0, 0, FrmColorPicker.SamplePanel.Width - 1, FrmColorPicker.SamplePanel.Height - 1, selectedColor);
		}
		void OnHexadecimalTextChanged(object sender, EventArgs e) {
			OnHexadecimalTextChanged(txtHexadecimal);
		}
		void OnTimerTick(object sender, EventArgs e) {
			DrawCursor(timer);
		}
		void OnColorGridPaint(object sender, PaintEventArgs e) {
			if(FormPainter == null)
				return;
			FormPainter.DrawColorGrid(e);
		}
		void OnGradientPanelPaint(object sender, PaintEventArgs e) {
			if(FormPainter == null)
				return;
			FormPainter.DrawGradientPanel(e);
		}
		void OnHSBSelectedIndexChanged(object sender, EventArgs e) {
			int index = this.hsbGroup.SelectedIndex;
			if(index == -1) return;
			ColorEditOption option = ColorEditOption.Hue;
			switch(index) {
				case 0: option = ColorEditOption.Hue; break;
				case 1: option = ColorEditOption.Saturation; break;
				case 2: option = ColorEditOption.Brightness; break;
			}
			SelectedColorEditOption = option;
		}
		void OnHueValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsHueValid, txtHue, txtHue.Text, e, ColorModelSettings.HueValidationMsg);
		}
		void OnSaturationValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsSatBrightValid, txtSaturation, txtSaturation.Text, e, ColorModelSettings.SatValidationMsg);
		}
		void OnBrightValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsSatBrightValid, txtBright, txtBright.Text, e, ColorModelSettings.BrightValidationMsg);
		}
		void OnOpacityValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsOpacityValid, txtOpacity, txtOpacity.Text, e, ColorModelSettings.OpacityValidationMsg);
		}
		void OnHexadecimalValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsRgbHexValid, txtHexadecimal, txtHexadecimal.Text, e, ColorModelSettings.ColorHexValidationMsg);
		}
		void OnColorGridDoubleClick(object sender, EventArgs e) {
			FrmColorPicker.ForceClosing();
		}
		#endregion
	}
}
