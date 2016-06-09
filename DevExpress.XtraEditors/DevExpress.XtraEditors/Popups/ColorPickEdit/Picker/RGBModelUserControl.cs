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
	public partial class RGBModelUserControl : ColorModelUserControlBase, IColorPickerTab {
		public RGBModelUserControl() {
			Appearance.Reset();
			InitializeComponent();
			ErrorProvider = this.dxErrorProvider;
			colorGridArea = SelectGridAreaRect;
			gradientArea = SelectGradientAreaRect;
			opacityArea = SelectOpacityAreaRect;
		}
		Rectangle opacityArea, colorGridArea, gradientArea;
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SubscribeCore();
			SetPageDefaultSettings();
		}
		protected virtual void SubscribeCore() { }
		protected virtual void UnsubscribeCore() {
			UnsubscribeOnContextualEvents();
		}
		protected void SetPageDefaultSettings() {
			GridXLabel = ColorModelSettings.HueAxisName;
			GridYLabel = ColorModelSettings.SaturationAxisName;
			GradientLabel = ColorModelSettings.LuminanceAxisName;
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
			return (txtRed.Focused || txtGreen.Focused || txtBlue.Focused || txtOpacity.Focused);
		}
		public override void ChangeColorGridValue(int delta) {
			TextEdit textEdit = null;
			const double min = 0;
			double max = 255;
			if(txtRed.Focused) textEdit = txtRed;
			else if(txtGreen.Focused) textEdit = txtGreen;
			else if(txtBlue.Focused) textEdit = txtBlue;
			else if(txtOpacity.Focused) textEdit = txtOpacity;
			if(textEdit == null) return;
			double value;
			if(double.TryParse(textEdit.Text, out value)) {
				value += delta;
				if(value > max)
					value = max;
				if(value < min)
					value = min;
				textEdit.Text = value.ToString();
			}
		}
		public override void CalculateColorWheel() {
			HueSatLight hueSatLight = new HueSatLight();
			ClearSelectedHsb();
			float width = CalculationWidth, height = CalculationHeight;
			hueSatLight.Lightness = (float)GradientValue;
			hueSatLight.Hue = GridXCore / width;
			hueSatLight.Saturation = (height - GridYCore) / height;
			HueSatBright hsb = FromHueSatLight(PrepareHueSatLight(hueSatLight), OpacityCore / 255.0);
			SelectedHsb = hsb;
			ColorWheelSelected(hsb);
		}
		protected HueSatBright FromHueSatLight(HueSatLight hsl, double alpha) {
			Color color = DevExpress.Utils.Colors.ColorConverter.FromHueSatLight(hsl);
			return new HueSatBright(color) { Alpha = alpha };
		}
		public override void UpdateColorGridTextBoxes(HueSatBright hueSatBright) {
			if(ChangingColorGridValuesCore)
				return;
			ChangingColorGridValuesCore = true;
			try {
				Color color = hueSatBright.AsRGB;
				SetArgbValue(txtRed, color.R);
				SetArgbValue(txtGreen, color.G);
				SetArgbValue(txtBlue, color.B);
				SetArgbValue(txtOpacity, color.A);
				if(!ChangingHexTextBox) txtHexadecimal.Text = GetHexadecimalValue(color);
			}
			finally {
				ChangingColorGridValuesCore = false;
			}
		}
		public override ColorModelFormPainterBase CreateFormPainter() {
			return ColorModelFormPainterBase.Create(ColorModel.RGB, this);
		}
		public override SuperToolTipManagerBase CreateSuperTipManager() {
			return SuperToolTipManagerBase.Create(ColorModel.RGB);
		}
		#endregion
		#region Methods
		void RefreshColorGridLabelsAndCursors() {
			float width = CalculationWidth;
			float height = CalculationHeight;
			HueSatLight hueSatLight = DevExpress.Utils.Colors.ColorConverter.FromColor(GetSelectedHsb().AsRGB);
			GridXCore = (int)Math.Round(hueSatLight.Hue * width);
			GridYCore = (int)Math.Round(height - hueSatLight.Saturation * height);
			GradientValue = hueSatLight.Lightness;
			pnlGradient.Invalidate();
			pnlColorGrid.Invalidate();
		}
		void RGBTextBoxesChanged() {
			if(ChangingColorGridValuesCore)
				return;
			int red = GetIntSpinEdit(txtRed, 0, 255);
			int green = GetIntSpinEdit(txtGreen, 0, 255);
			int blue = GetIntSpinEdit(txtBlue, 0, 255);
			SetOpacity(GetIntSpinEdit(txtOpacity, 0, 255));
			ClearSelectedHsb();
			SelectedColor = Color.FromArgb(OpacityCore, red, green, blue);
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
		public virtual void SetParentForm(FrmColorPicker form) {
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
		void OnRedTextChanged(object sender, EventArgs e) {
			RGBTextBoxesChanged();
		}
		void OnGreenTextChanged(object sender, EventArgs e) {
			RGBTextBoxesChanged();
		}
		void OnBlueTextChanged(object sender, EventArgs e) {
			RGBTextBoxesChanged();
		}
		void OnMakeWebSafeBtnClick(object sender, EventArgs e) {
			SelectedColor = MakeWebSafe(SelectedColor);
			RGBTextBoxesChanged();
		}
		void OnOpacityPanelPaint(object sender, PaintEventArgs e) {
			if(FormPainter == null)
				return;
			FormPainter.DrawOpacityPanel(e);
		}
		bool rightButtonPressedOpacityPanel = false; 
		void OnOpacityPanelMouseDown(object sender, MouseEventArgs e) {
			if(!opacityArea.Contains(e.Location)) return;
			pnlOpacity.Focus();
			rightButtonPressedOpacityPanel = true;
			SetColorOpacityFromMouse(e.X);
		}
		void OnOpacityPanelMouseMove(object sender, MouseEventArgs e) {
			UpdateCursor(OpacityPanel, opacityArea, Cursors.VSplit, e);
			SuperTipManager.RefreshOpacityInfo(this, opacityToolTip, e, rightButtonPressedOpacityPanel);
		}
		void OnOpacityPanelMouseUp(object sender, MouseEventArgs e) {
			rightButtonPressedOpacityPanel = false;
		}
		void OnOpacityPanelTextChanged(object sender, EventArgs e) {
			RGBTextBoxesChanged();
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
		void OnGradientPanelMouseDown(object sender, MouseEventArgs e) {
			if(!gradientArea.Contains(e.Location)) return;
			pnlGradient.Focus();
			rightButtonPressedGradientPanel = true;
			SetGradientPositionFromMouse(e.Y);
		}
		bool rightButtonPressedGradientPanel = false;
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
		void OnHexadecimalValidating(object sender, CancelEventArgs e) {
			ColorPickerValidationHelper.ValidateCore(ErrorProvider, ColorPickerValidationHelper.IsRgbHexValid, txtHexadecimal, txtHexadecimal.Text, e, ColorModelSettings.ColorHexValidationMsg);
		}
		void OnColorGridDoubleClick(object sender, EventArgs e) {
			FrmColorPicker.ForceClosing();
		}
		#endregion
	}
}
