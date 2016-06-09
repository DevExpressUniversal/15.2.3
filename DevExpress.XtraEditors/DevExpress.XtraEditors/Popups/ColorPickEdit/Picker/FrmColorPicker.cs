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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using DevExpress.Utils.Colors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.ColorPick.Picker {
	public partial class FrmColorPicker : XtraForm {
		public FrmColorPicker() : this(null) { }
		public FrmColorPicker(RepositoryItemColorPickEdit properties) {
			UpdateDialogSettings(CreateGraphics(), "WwW", Appearance.Font);
			this.Properties = properties;
			InitializeComponent();
			SetChildTabsParent();
		}
		protected virtual void UpdateDialogSettings(Graphics graphics, string str, Font font) {
			int differenceSize = CalcDifferenceSize(graphics, str, font);
			DialogSettings.INT_Left = 25 + differenceSize;
			DialogSettings.INT_Bottom = 20 + differenceSize;
		}
		protected virtual int CalcDifferenceSize(Graphics graphics, string str, Font font) {
			int oldHeight = 14;
			int newHeight = graphics.MeasureString(str, font).ToSize().Height;
			return Math.Max(0, Math.Abs(newHeight - oldHeight));
		}
		protected internal RepositoryItemColorPickEdit Properties {
			get;
			private set;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			InitDialogBase();
			ApplyTabProperties();
		}
		protected virtual void InitDialogBase() {
			IColorPickerTab tab = FromTabPage(TabControl.SelectedTabPage);
			tab.SubscribeOnContextualEvents();
			ApplyDialogOptions();
		}
		protected virtual void ApplyDialogOptions() {
			if(Properties == null)
				return;
			ColorDialogOptions options = Properties.ColorDialogOptions;
			if(options.FormIcon != null) {
				Icon = options.FormIcon;
				ShowIcon = true;
			}
			else {
				ShowIcon = false;
			}
		}
		protected virtual void SetChildTabsParent() {
			ForEachTab(page => page.SetParentForm(this));
		}
		protected virtual void ApplyTabPropertiesCore() {
			if(Properties == null)
				return;
			ColorDialogOptions options = Properties.ColorDialogOptions;
			ApplyShowTabsOption(options);
			ApplyShowPreviewOption(options);
		}
		protected virtual void ApplyShowTabsOption(ColorDialogOptions options) {
			switch(options.ShowTabs) {
				case ShowTabs.All:
					SetTabPagesVisibilityState(true);
					break;
				case ShowTabs.HSBModel:
					SetTabPagesVisibilityState(false);
					this.tabHSB.PageVisible = true;
					break;
				case ShowTabs.RGBModel:
					SetTabPagesVisibilityState(false);
					this.tabRGB.PageVisible = true;
					break;
			}
		}
		protected virtual void ApplyShowPreviewOption(ColorDialogOptions options) {
			SamplePanel.Visible = options.ShowPreview;
			SampleLabel.Visible = options.ShowPreview;
		}
		protected virtual void ApplyTabProperties() {
			ApplyTabPropertiesCore();
			ForEachTab(page => page.ApplyProperties());
		}
		protected virtual void ForEachTab(FrmColorPickerCallback callback) {
			foreach(XtraTabPage page in TabControl.TabPages) {
				IColorPickerTab pickerTab = FromTabPage(page);
				callback(pickerTab);
			}
		}
		protected virtual void SetTabPagesVisibilityState(bool state) {
			foreach(XtraTabPage page in TabControl.TabPages) {
				page.PageVisible = state;
			}
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}
		protected IColorPickerTab FromTabPage(XtraTabPage page) {
			if(page == null)
				return null;
			return page.Controls[0] as IColorPickerTab;
		}
		#region Public Interface
		public void ForceClosing() {
			DialogResult = DialogResult.OK;
		}
		public Color SelectedColor {
			get { return SelectedTab.SelectedColor; }
			set { ForEachTab(page => page.SelectedColor = value); }
		}
		public event PaintEventHandler SamplePanelPaint {
			add { SamplePanel.Paint += value; }
			remove { SamplePanel.Paint -= value; }
		}
		public Panel SamplePanel { get { return this.pnlSample; } }
		public LabelControl SampleLabel { get { return this.lblSample; } }
		#endregion
		protected internal XtraTabControl TabControl { get { return this.tbctColors; } }
		protected internal IColorPickerTab SelectedTab { get { return TabControl.SelectedTabPage.Controls[0] as IColorPickerTab; } }
		#region Delegates
		protected delegate void FrmColorPickerCallback(IColorPickerTab tab);
		#endregion
		#region Handlers
		void OnTabControlSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			IColorPickerTab prev = FromTabPage(e.PrevPage);
			if(prev != null)
				prev.UnsubscribeOnContextualEvents();
			IColorPickerTab page = FromTabPage(e.Page);
			if(page != null) 
				page.SubscribeOnContextualEvents();
		}
		void OnToolTipControllerBeforeShow(object sender, ToolTipControllerShowEventArgs e) {
			IColorPickerTab tab = SelectedTab;
			tab.SuperTipManager.RefreshSampleInfo(e, tab as ColorModelUserControlAbstractBase);
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			e.Cancel = false;
			base.OnFormClosing(e);
		}
		#endregion
	}
	public interface IColorPickerTab {
		Color SelectedColor { get; set; }
		void SetParentForm(FrmColorPicker form);
		void ApplyProperties();
		void SubscribeOnContextualEvents();
		void UnsubscribeOnContextualEvents();
		SuperToolTipManagerBase SuperTipManager { get; }
	}
	class DialogSettings {
		public static readonly int INT_Right = 5;
		public static int INT_Left = 25;
		public static readonly int INT_Top = 5;
		public static int INT_Bottom = 20;
		public static readonly int OpacityPanelLeftOffset = 3;
		public static readonly int OpacityPanelWidthExtension = 6;
		public static readonly int StdValueOffset = 1;
		public static readonly int BigValueOffset = 10;
		public static readonly int OpacityPanelContentOffset = 7;
		public static readonly int LabelToContentDistance = 3;
	}
	class ColorModelSettings {
		public static readonly double HueMaxMinOffset = 0.002;
		public static readonly double RgbColorGridBrightnessValue = 0.75;
		public static string HueAxisName { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickHueAxisName); } }
		public static string SaturationAxisName { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickSaturationAxisName);  } }
		public static string LuminanceAxisName { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickLuminanceAxisName); } }
		public static string BrightnessAxisName { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickBrightnessAxisName); } }
		public static string OpacityAxisName { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickOpacityAxisName); } }
		public static string RedValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickRedValidationMsg); } }
		public static string GreenValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickGreenValidationMsg); } }
		public static string BlueValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickBlueValidationMsg); } }
		public static string OpacityValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickOpacityValidationMsg); } }
		public static string ColorHexValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickColorHexValidationMsg); } }
		public static string HueValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickHueValidationMsg); } }
		public static string SatValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickSaturationValidationMsg); } }
		public static string BrightValidationMsg { get { return Localizer.Active.GetLocalizedString(StringId.ColorPickBrightValidationMsg); } }
	}
	public abstract class ColorModelUserControlAbstractBase : XtraUserControl {
		public ColorModelUserControlAbstractBase() {
			this.OpacityCore = 255;
			this.SelectedHsb = new HueSatBright(0, 0, 0, 0);
			this.ColorGridCursor = new Cursor(GetType().Assembly.GetManifestResourceStream("DevExpress.XtraEditors.Popups.ColorPickEdit.Picker.ColorGrid.cur"));
			this.FormPainter = CreateFormPainter();
			this.FrmColorPicker = null;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ColorGridPanel != null) ColorGridPanel.Cursor = ColorGridCursor;
			this.SuperTipManager = CreateSuperTipManager();
		}
		#region Common Properties
		public int GridXCore { get; set; }
		public int GridYCore { get; set; }
		public string GridXLabel { get; set; }
		public string GridYLabel { get; set; }
		public string GradientLabel { get; set; }
		public int OpacityCore { get; set; }
		public double GradientValue { get; set; }
		protected bool ChangingHexTextBox { get; set; }
		public bool CursorOn { get; set; }
		public int LastOpacityHover { get; set; }
		protected Cursor ColorGridCursor { get; set; }
		public HueSatBright SelectedHsb { get; set; }
		protected bool ChangingColorGridValuesCore { get; set; }
		protected bool PickingWheelColor { get; set; }
		protected ColorModelFormPainterBase FormPainter { get; set; }
		protected DXErrorProvider.DXErrorProvider ErrorProvider { get; set; }
		public virtual SuperToolTipManagerBase SuperTipManager { get; set; }
		protected internal FrmColorPicker FrmColorPicker { get; set; }
		#endregion
		#region Process Cmd Keys
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(ShouldChangeColorGrid())
				return ChangeColorGridCore(keyData);
			else if(IsDefaultCmdKeyProcessing())
				return ProcessDefaultCmdKey(keyData);
			return false;
		}
		bool ChangeColorGridCore(Keys keyData) {
			if(keyData == Keys.Up) {
				ChangeColorGridValue(1);
				return true;
			}
			else if(keyData == Keys.Down) {
				ChangeColorGridValue(-1);
				return true;
			}
			return false;
		}
		bool IsDefaultCmdKeyProcessing() {
			return true;
		}
		bool ProcessDefaultCmdKey(Keys keyData) {
			if(OpacityPanel.Focused)
				return ProcessOpacityPanelKeyCmd(keyData);
			else if(GradientPanel.Focused)
				return ProcessGradientPanelKeyCmd(keyData);
			else if(ColorGridPanel.Focused)
				return ProcessColorGridPanelKeyCmd(keyData);
			return false;
		}
		bool ProcessOpacityPanelKeyCmd(Keys keyData) {
			switch(keyData) {
				case Keys.Left:
					SetOpacity(OpacityCore - DialogSettings.StdValueOffset);
					return true;
				case Keys.Right:
					SetOpacity(OpacityCore + DialogSettings.StdValueOffset);
					return true;
				case Keys.Left | Keys.Control:
					SetOpacity(OpacityCore - DialogSettings.BigValueOffset);
					return true;
				case Keys.Right | Keys.Control:
					SetOpacity(OpacityCore + DialogSettings.BigValueOffset);
					return true;
				case Keys.Home:
					SetOpacity(0);
					return true;
				case Keys.End:
					SetOpacity(255);
					return true;
			}
			return false;
		}
		bool ProcessGradientPanelKeyCmd(Keys keyData) {
			switch(keyData) {
				case Keys.Up:
					SetGradientPosition((int)Math.Round(GradientValue * 255 + DialogSettings.StdValueOffset));
					return true;
				case Keys.Down:
					SetGradientPosition((int)Math.Round(GradientValue * 255 - DialogSettings.StdValueOffset));
					return true;
				case Keys.Up | Keys.Control:
					SetGradientPosition((int)Math.Round(GradientValue * 255 + DialogSettings.BigValueOffset));
					return true;
				case Keys.Down | Keys.Control:
					SetGradientPosition((int)Math.Round(GradientValue * 255 - DialogSettings.BigValueOffset));
					return true;
				case Keys.Home:
					SetGradientPosition(255);
					return true;
				case Keys.End:
					SetGradientPosition(0);
					return true;
			}
			return false;
		}
		bool ProcessColorGridPanelKeyCmd(Keys keyData) {
			switch(keyData) {
				case Keys.Up:
					SetGridPosition(GridXCore, GridYCore - DialogSettings.StdValueOffset);
					return true;
				case Keys.Up | Keys.Control:
					SetGridPosition(GridXCore, GridYCore - DialogSettings.BigValueOffset);
					return true;
				case Keys.Down:
					SetGridPosition(GridXCore, GridYCore + DialogSettings.StdValueOffset);
					return true;
				case Keys.Down | Keys.Control:
					SetGridPosition(GridXCore, GridYCore + DialogSettings.BigValueOffset);
					return true;
				case Keys.Left:
					SetGridPosition(GridXCore - DialogSettings.StdValueOffset, GridYCore);
					return true;
				case Keys.Left | Keys.Control:
					SetGridPosition(GridXCore - DialogSettings.BigValueOffset, GridYCore);
					return true;
				case Keys.Right | Keys.Control:
					SetGridPosition(GridXCore + DialogSettings.BigValueOffset, GridYCore);
					return true;
				case Keys.Right:
					SetGridPosition(GridXCore + DialogSettings.StdValueOffset, GridYCore);
					return true;
				case Keys.Home:
					SetGridPosition(0, 0);
					return true;
				case Keys.End:
					SetGridPosition(255, 255);
					return true;
			}
			return false;
		}
		#endregion
		#region Abstract Members
		public abstract void ChangeColorGridValue(int delta);
		public abstract bool ShouldChangeColorGrid();
		public abstract void CalculateColorWheel();
		public abstract void UpdateColorGridTextBoxes(HueSatBright hueSatBright);
		public abstract Panel OpacityPanel { get; }
		public abstract Panel GradientPanel { get; }
		public abstract Panel ColorGridPanel { get; }
		public abstract TextEdit OpacityTextEdit { get; }
		public abstract LabelControl OpacityLabel { get; }
		public abstract ColorModelFormPainterBase CreateFormPainter();
		public abstract SuperToolTipManagerBase CreateSuperTipManager();
		public abstract Color SelectedColor { get; set; }
		public abstract ColorEditOption SelectedColorEditOption { get; set; }
		public abstract BaseControl SampleLabel { get; }
		public ISkinProvider SkinProvider { get { return SampleLabel.LookAndFeel; } }
		#endregion
		#region Methods
		protected void SetOpacity(int x) {
			SetColorOpacity(x);
			SelectedColor = Color.FromArgb(OpacityCore, SelectedColor);
			OpacityPanel.Invalidate();
		}
		protected void SetColorOpacity(int x) {
			OpacityCore = x;
			if(OpacityCore > 255)
				OpacityCore = 255;
			if(OpacityCore < 0)
				OpacityCore = 0;
			if(!OpacityTextEdit.Focused)
				OpacityTextEdit.Text = OpacityCore.ToString();
			int percent = (int)Math.Ceiling(100 * OpacityCore / 255.0);
			OpacityLabel.Text = percent.ToString() + "%";
		}
		protected void SetGradientPosition(int value) {
			value = InBounds(value, 0, 255);
			double normalizedValue = (double)value / 255;
			if(GradientValue != normalizedValue) {
				GradientValue = normalizedValue;
				PickingWheelColor = true;
				try {
					CalculateColorWheel();
				}
				finally {
					PickingWheelColor = false;
				}
				ColorGridPanel.Invalidate();
				GradientPanel.Invalidate();
			}
		}
		protected void SetGridPosition(int x, int y) {
			int xPos = InBounds(x, 0, CalculationWidth);
			int yPos = InBounds(y, 0, CalculationHeight);
			if(GridXCore != xPos || GridYCore != yPos) {
				GridYCore = yPos;
				GridXCore = xPos;
				CalculateColorWheel();
				ColorGridPanel.Invalidate();
				GradientPanel.Invalidate();
			}
		}
		protected HueSatBright GetSelectedHsb() {
			if(SelectedHsb.Alpha != 0) return SelectedHsb;
			return new HueSatBright(SelectedColor);
		}
		protected void ClearSelectedHsb() {
			SelectedHsb = new HueSatBright(0, 0, 0, 0);
		}
		public Rectangle SelectOpacityAreaRect {
			get {
				return new Rectangle(DialogSettings.INT_Left, DialogSettings.INT_Top, OpacityPanel.Width - DialogSettings.INT_Right - DialogSettings.INT_Left, OpacityPanel.Height - DialogSettings.INT_Bottom - DialogSettings.INT_Top);
			}
		}
		public Rectangle SelectGradientAreaRect {
			get {
				return new Rectangle(DialogSettings.INT_Left, DialogSettings.INT_Top, GradientPanel.Width - DialogSettings.INT_Right - DialogSettings.INT_Left, GradientPanel.Height - DialogSettings.INT_Bottom - DialogSettings.INT_Top);
			}
		}
		public Rectangle SelectGridAreaRect {
			get {
				return new Rectangle(DialogSettings.INT_Left, DialogSettings.INT_Top, ColorGridPanel.Width - DialogSettings.INT_Right - DialogSettings.INT_Left, ColorGridPanel.Height - DialogSettings.INT_Bottom - DialogSettings.INT_Top);
			}
		}
		protected virtual void UpdateCursor(Control control, Rectangle rect, Cursor cursor, MouseEventArgs e) {
			control.Cursor = rect.Contains(e.Location) ? cursor : Cursors.Default;
		}
		public void SetColorOpacityFromMouse(int x) {
			SetOpacity((int)Math.Round(Scale(x - DialogSettings.INT_Left + 1, 0, SelectOpacityAreaRect.Width, 255)));
		}
		protected void EnableCursor(Timer timer) {
			timer.Enabled = true;
			CursorOn = true;
			DrawCursor(timer);
		}
		protected void DrawCursor(Timer timer) {
			if(GradientPanel.Focused)
				InvalidateGradientCursor();
			else if(OpacityPanel.Focused)
				InvalidateOpacityCursor();
			else if(ColorGridPanel.Focused)
				InvalidateColorGridCursor();
			CursorOn = !CursorOn;
			if(CursorOn) timer.Interval = 600;
			else timer.Interval = 350;
		}
		protected void DisableCursor(Timer timer) {
			timer.Enabled = false;
			CursorOn = false;
			DrawCursor(timer);
		}
		protected void InvalidateColorGridCursor() {
			ColorGridPanel.Invalidate();
		}
		protected void InvalidateOpacityCursor() {
			OpacityPanel.Invalidate();
		}
		protected void InvalidateGradientCursor() {
			GradientPanel.Invalidate();
		}
		public void InitColorGridValues(out int steps, out int panelHeight) {
			steps = ColorGridPanel.Width - DialogSettings.INT_Left - DialogSettings.INT_Right;
			panelHeight = ColorGridPanel.Height - DialogSettings.INT_Top - DialogSettings.INT_Bottom;
		}
		protected void SetGradientPositionFromMouse(int y) {
			int pos = InBounds(y, 0, SelectGradientAreaRect.Height);
			pos = (int)((double)(pos) / ((double)(SelectGradientAreaRect.Height) / (double)255));
			SetGradientPosition(255 - pos);
		}
		protected void SetGridPositionFromMouse(int mouseX, int mouseY) {
			int y = mouseY - DialogSettings.INT_Top;
			int x = mouseX - DialogSettings.INT_Left;
			SetGridPosition(x, y);
		}
		protected void ColorWheelSelected(HueSatBright hueSatBright) {
			UpdateColorGridTextBoxes(hueSatBright);
			PickingWheelColor = true;
			try {
				SelectedColor = hueSatBright.AsRGB;
			}
			finally {
				PickingWheelColor = false;
			}
		}
		protected void OnHexadecimalTextChanged(TextEdit hexEdit) {
			if(ChangingColorGridValuesCore || !ForceValidation())
				return;
			string hexText = hexEdit.Text.Trim();
			if(!hexText.StartsWith("#"))
				hexText = "#" + hexText;
			Color newColor;
			if(!DevExpress.Utils.Colors.ColorConverter.GetColorValueHex(hexText, out newColor))
				return;
			ChangingHexTextBox = true;
			try {
				SetColorOpacity(newColor.A);
				ClearSelectedHsb();
				SelectedColor = newColor;
			}
			finally {
				ChangingHexTextBox = false;
			}
		}
		protected bool ForceValidation() {
			return ValidateChildren();
		}
		public int CalculationWidth {
			get {
				if(ColorGridPanel == null) return 0;
				return ColorGridPanel.Width - DialogSettings.INT_Left - DialogSettings.INT_Right;
			}
		}
		public int CalculationHeight {
			get {
				if(ColorGridPanel == null) return 0;
				return ColorGridPanel.Height - DialogSettings.INT_Top - DialogSettings.INT_Bottom;
			}
		}
		public int XComponent { get { return (int)Math.Round(Scale(GridXCore, 0, CalculationWidth, 255)); } }
		public int YComponent { get { return (int)Math.Round(Scale(CalculationHeight - GridYCore, 0, CalculationHeight, 255)); } }
		#endregion
		#region Static
		protected static void SetHsbValue(TextEdit textEdit, string newValue) {
			if(textEdit.Focused)
				return;
			textEdit.Text = newValue;
		}
		protected static int InBounds(int pos, int lower, int upper) {
			return Math.Min(Math.Max(pos, lower), upper);
		}
		protected static void SetArgbValue(TextEdit textEdit, byte value) {
			if(textEdit.Focused)
				return;
			textEdit.Text = value.ToString();
		}
		protected static string GetHexadecimalValue(Color color) {
			if(color.A != 255)
				return String.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
			return String.Format("{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
		}
		protected static int GetInt(TextEdit textEdit, int min, int max) {
			int number;
			int.TryParse(textEdit.Text, out number);
			if(number < min) number = min;
			if(number > max) number = max;
			return number;
		}
		protected static int GetIntSpinEdit(SpinEdit spinEdit, int min, int max) {
			int number = (int)spinEdit.Value;
			if(number < min) number = min;
			if(number > max) number = max;
			return number;
		}
		protected static double GetDouble(TextEdit textEdit, double min, double max) {
			double number;
			double.TryParse(textEdit.Text, out number);
			if(number < min) number = min;
			if(number > max) number = max;
			return number;
		}
		protected static int MakeWebSafe(int value) {
			int distance = value % 0x33;
			if(distance <= 0x19)
				return value - distance;
			return value + 0x33 - distance;
		}
		protected static Color MakeWebSafe(Color selectedColor) {
			return Color.FromArgb(255, MakeWebSafe(selectedColor.R), MakeWebSafe(selectedColor.G), MakeWebSafe(selectedColor.B));
		}
		protected static bool IsWebSafe(byte value) {
			return value == 0 || value == 0x33 || value == 0x66 || value == 0x99 || value == 0xCC || value == 0xFF;
		}
		protected static bool IsWebSafe(Color color) {
			return IsWebSafe(color.R) && IsWebSafe(color.G) && IsWebSafe(color.B) && color.A == 255;
		}
		public static void DrawColorSwatch(Graphics graphics, int x, int y, int width, int height, Color color) {
			if(color == Color.Empty || color == Color.Transparent || color.Name == "Empty" && color.A == 0) {
				Color forColor = Color.Red;
				var hatchStyle = HatchStyle.BackwardDiagonal;
				if(color == Color.Transparent) {
					forColor = Color.SkyBlue;
					hatchStyle = HatchStyle.SolidDiamond;
				}
				using(HatchBrush newSolidBrush = new HatchBrush(hatchStyle, forColor, Color.White))
					graphics.FillRectangle(newSolidBrush, x, y, width, height);
			}
			else {
				if(color.A < 255)
					using(HatchBrush newSolidBrush = new HatchBrush(HatchStyle.SolidDiamond, Color.SkyBlue, Color.White))
						graphics.FillRectangle(newSolidBrush, x, y, width, height);
				using(SolidBrush newSolidBrush = new SolidBrush(color))
					graphics.FillRectangle(newSolidBrush, x, y, width, height);
			}
			Pen outlinePen = Pens.DarkGray;
			if(color == Color.DarkGray)
				outlinePen = Pens.DimGray;
			graphics.DrawRectangle(outlinePen, x, y, width, height);
		}
		protected static float FInBounds(float pos, float lower, float upper) {
			return Math.Min(Math.Max(pos, lower), upper);
		}
		protected static float Scale(float value, float lower, float upper, float factor) {
			value = FInBounds(value, lower, upper);
			return value / (upper - lower) * factor;
		}
		public static Color GetLabelColors(bool hasFocus, ISkinProvider sp, BaseControl baseControl) {
			SkinElement element = CommonSkins.GetSkin(sp)[CommonSkins.SkinLabel];
			if(!hasFocus)
				return LookAndFeelHelper.GetSystemColorEx(sp, SystemColors.GrayText);
			Color color = element.GetAppearanceDefault().ForeColor;
			return color == Color.Transparent ? LookAndFeelHelper.GetTransparentForeColor(sp, baseControl) : color;
		}
		public static HueSatLight PrepareHueSatLight(HueSatLight hueSatLight) {
			if(hueSatLight.Hue == 1)
				hueSatLight.Hue -= ColorModelSettings.HueMaxMinOffset;
			if(hueSatLight.Hue == 0)
				hueSatLight.Hue += ColorModelSettings.HueMaxMinOffset;
			return hueSatLight;
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class ColorModelUserControlBase : ColorModelUserControlAbstractBase {
		public ColorModelUserControlBase() {
		}
		public override void ChangeColorGridValue(int delta) { }
		public override bool ShouldChangeColorGrid() {
			return true;
		}
		public override void CalculateColorWheel() { }
		public override void UpdateColorGridTextBoxes(HueSatBright hueSatBright) { }
		public override Panel OpacityPanel {
			get { return null; }
		}
		public override Panel GradientPanel {
			get { return null; }
		}
		public override Panel ColorGridPanel {
			get { return null; }
		}
		public override TextEdit OpacityTextEdit {
			get { return null; }
		}
		public override LabelControl OpacityLabel {
			get { return null; }
		}
		public override ColorModelFormPainterBase CreateFormPainter() {
			return null;
		}
		public override SuperToolTipManagerBase CreateSuperTipManager() {
			return null;
		}
		public override Color SelectedColor {
			get { return Color.Empty; }
			set { }
		}
		public override ColorEditOption SelectedColorEditOption {
			get { return ColorEditOption.Hue; }
			set { }
		}
		public override BaseControl SampleLabel {
			get { return null; }
		}
	}
	public enum ColorModel { RGB, HSB }
	public abstract class ColorModelFormPainterBase {
		public ColorModelFormPainterBase(ColorModelUserControlAbstractBase control) {
			this.Control = control;
		}
		protected ColorModelUserControlAbstractBase Control { get; private set; }
		public static ColorModelFormPainterBase Create(ColorModel model, ColorModelUserControlAbstractBase control) {
			if(model == ColorModel.RGB)
				return new RGBColorModelFormPainter(control);
			return new HSBColorModelFormPainter(control);
		}
		public abstract void DrawColorGrid(PaintEventArgs e);
		public abstract void DrawGradientPanel(PaintEventArgs e);
		#region Common
		public void DrawGridLabels(Graphics graphics) {
			int bottom = Control.ColorGridPanel.ClientRectangle.Height - DialogSettings.INT_Bottom;
			DrawVerticalLabel(graphics, bottom - 1, Control.GridYLabel, Control.ColorGridPanel.Focused);
			DrawHorizontalLabel(graphics, Control.GridXLabel, DialogSettings.INT_Left, bottom + DialogSettings.LabelToContentDistance + 1, Control.ColorGridPanel.ClientRectangle.Width - 3, Control.ColorGridPanel.Focused);
		}
		public void DrawHorizontalLabel(Graphics graphics, string text, int x, int y, int width, bool hasFocus) {
			if(!ShouldDrawArrows)
				return;
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Color labelColor = ColorModelUserControlAbstractBase.GetLabelColors(hasFocus, Control.SkinProvider, Control.SampleLabel);
			using(GraphicsCache cache = new GraphicsCache(graphics)) {
				using(Brush fontBrush = new SolidBrush(labelColor)) {
					Rectangle rect = new Rectangle();
					rect.Location = new Point(x + 3, y);
					rect.Size = graphics.MeasureString(text, Control.Font).ToSize();
					cache.DrawString(text, Control.Font, fontBrush, rect, StringFormat.GenericDefault);
				}
				SizeF sizeF = graphics.MeasureString(text, Control.Font);
				int yArrow = (int)Math.Round(y + sizeF.Height / 2);
				int textXEnd = (int)Math.Round(x + sizeF.Width);
				Point start = new Point(textXEnd + 4, yArrow);
				Point end = new Point(width - 6, yArrow);
				using(Pen linePen = new Pen(labelColor)) {
					graphics.DrawLine(linePen, start, end);
				}
				Point[] points = new Point[3];
				points[0] = end;
				points[1] = new Point(end.X - 9, end.Y - 4);
				points[2] = new Point(end.X - 9, end.Y + 4);
				using(Brush arrowBrush = new SolidBrush(labelColor)) {
					graphics.FillPolygon(arrowBrush, points);
				}
			}
		}
		public void DrawVerticalLabel(Graphics graphics, float textYStart, string label, bool hasFocus) {
			DrawVerticalLabel(graphics, textYStart, label, hasFocus, 7, 4);
		}
		public void DrawVerticalLabel(Graphics graphics, float textYStart, string label, bool hasFocus, int offset, bool isXOffset) {
			if(isXOffset)
				DrawVerticalLabel(graphics, textYStart, label, hasFocus, offset, 4);
			else
				DrawVerticalLabel(graphics, textYStart, label, hasFocus, 7, offset);
		}
		public void DrawVerticalLabel(Graphics graphics, float textYStart, string label, bool hasFocus, int xOffset, int yOffset) {
			if(!ShouldDrawArrows)
				return;
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			using(GraphicsCache cache = new GraphicsCache(graphics)) {
				SizeF sizeF = graphics.MeasureString(label, Control.Font);
				Rectangle clientRect = FromRectangleF(new RectangleF(xOffset, Control.SelectGradientAreaRect.Height - sizeF.Width, sizeF.Height, sizeF.Width));
				Color labelColor = ColorModelUserControlAbstractBase.GetLabelColors(hasFocus, Control.SkinProvider, Control.SampleLabel);
				using(SolidBrush labelBrush = new SolidBrush(labelColor)) {
					cache.DrawVString(label, Control.Font, labelBrush, clientRect, StringFormat.GenericDefault, 270);
				}
				int height = Control.SelectGradientAreaRect.Height;
				Point linePtBottom = new Point((int)(xOffset + sizeF.Height / 2), (int)(Control.SelectGradientAreaRect.Height - sizeF.Width));
				Point linePtTop = new Point((int)(xOffset + sizeF.Height / 2), Control.SelectGradientAreaRect.Y);
				using(Pen linePen = new Pen(labelColor)) {
					graphics.DrawLine(linePen, linePtBottom, linePtTop);
				}
				Point[] points = new Point[3];
				points[0] = linePtTop;
				points[1] = new Point(linePtTop.X - 4, linePtTop.Y + 9);
				points[2] = new Point(linePtTop.X + 4, linePtTop.Y + 9);
				using(Brush arrowBrush = new SolidBrush(labelColor)) {
					graphics.FillPolygon(arrowBrush, points);
				}
			}
		}
		protected virtual bool ShouldDrawArrows {
			get {
				if(Properties == null)
					return true;
				ShowArrows showArrows = Properties.ColorDialogOptions.ShowArrows;
				return showArrows == ShowArrows.Default || showArrows == ShowArrows.True;
			}
		}
		protected RepositoryItemColorPickEdit Properties {
			get {
				if(Control.FrmColorPicker == null)
					return null;
				return Control.FrmColorPicker.Properties;
			}
		}
		static Rectangle FromRectangleF(RectangleF rectF) {
			int width = (int)rectF.Width + 1;
			int height = (int)rectF.Height + 1;
			return new Rectangle((int)rectF.X, (int)rectF.Y, width, height);
		}
		public void ClearBackground(Graphics graphics, Panel panel) {
			using(SolidBrush bgBrush = new SolidBrush(panel.BackColor)) {
				graphics.FillRectangle(bgBrush, panel.ClientRectangle);
			}
		}
		public void PaintGridCursor(Graphics graphics) {
			int circleRadius = 5;
			int circleDiameter = circleRadius * 2;
			Rectangle rect = new Rectangle(DialogSettings.INT_Left + Control.GridXCore - circleRadius - 1, DialogSettings.INT_Top + Control.GridYCore - circleRadius, circleDiameter, circleDiameter);
			float innerPenWidth = 1f;
			float outerPenWidth = 1f;
			if(Control.ColorGridPanel.Focused) {
				innerPenWidth = 2f;
				outerPenWidth = 1.8f;
			}
			Color outerBorderColor;
			Color innerBorderColor;
			if(Control.CursorOn || !Control.ColorGridPanel.Focused) {
				outerBorderColor = Color.Black;
				innerBorderColor = Color.White;
			}
			else {
				outerBorderColor = Color.FromArgb(158, Color.Black);
				innerBorderColor = Color.FromArgb(192, Color.White);
			}
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			using(Pen outerBorderPen = new Pen(outerBorderColor, outerPenWidth))
				graphics.DrawEllipse(outerBorderPen, rect);
			rect.Inflate(-1, -1);
			using(Pen innerBorderPen = new Pen(innerBorderColor, 2 * innerPenWidth / 3))
				graphics.DrawEllipse(innerBorderPen, rect);
		}
		public void DrawGradientLine(Graphics graphics, int top, int panelHeight, int i, Color start, Color end) {
			using(Brush linearGradientBrush = new LinearGradientBrush(new Rectangle(DialogSettings.INT_Left, top, 1, panelHeight), start, end, 90.0f))
			using(Pen gradientPen = new Pen(linearGradientBrush))
				graphics.DrawLine(gradientPen, new Point(DialogSettings.INT_Left + i, top), new Point(DialogSettings.INT_Left + i, top + panelHeight));
		}
		public void DrawControlBorder(Graphics graphics, Rectangle clientRect) {
			int left = DialogSettings.INT_Left - 1;
			int top = DialogSettings.INT_Top - 1;
			Rectangle rect = new Rectangle(left, top, clientRect.Right - left - DialogSettings.INT_Right, clientRect.Bottom - top - 1);
			graphics.DrawRectangle(SystemPens.ControlDark, rect);
		}
		public void DrawGradientLabel(Graphics graphics, Rectangle clientRect) {
			DrawVerticalLabel(graphics, Control.GradientPanel.ClientRectangle.Height - 14, Control.GradientLabel, Control.GradientPanel.Focused, DialogSettings.INT_Bottom, false);
		}
		public void DrawGradientCursor(Graphics graphics) {
			int yPos = (int)Math.Round((1.0 - Control.GradientValue) * Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top) - 1;
			int overhang = 2;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			int left = DialogSettings.INT_Left - overhang;
			float penWidth = 1f;
			if(Control.GradientPanel.Focused)
				penWidth = 1.5f;
			Color borderColor;
			if(Control.CursorOn && Control.GradientPanel.Focused)
				borderColor = Color.Black;
			else borderColor = Color.FromArgb(158, Color.Black);
			using(Pen borderPen = new Pen(borderColor, penWidth))
			using(Brush fillBrush = new SolidBrush(Color.White)) {
				Point[] leftArrow = { new Point(left + 7, yPos), new Point(left, yPos - 5), new Point(left, yPos + 5) };
				graphics.FillPolygon(fillBrush, leftArrow);
				graphics.DrawPolygon(borderPen, leftArrow);
				int right = left + Control.GradientPanel.Width - DialogSettings.INT_Left - 4 + overhang;
				Point[] rightArrow = { new Point(right - 7, yPos), new Point(right, yPos - 5), new Point(right, yPos + 5) };
				graphics.FillPolygon(fillBrush, rightArrow);
				graphics.DrawPolygon(borderPen, rightArrow);
			}
		}
		public void DrawOpacityPanel(PaintEventArgs e) {
			Rectangle clientRect = Control.OpacityPanel.ClientRectangle;
			int offset = Control.SelectOpacityAreaRect.X;
			Rectangle gridRect = Control.SelectOpacityAreaRect;
			gridRect.Inflate(-1, 0);
			gridRect.Width -= 1;
			ClearBackground(e.Graphics, Control.OpacityPanel);
			DrawOpacityPanelLabel(e.Graphics, clientRect);
			DrawOpacityPanelBackground(e.Graphics, clientRect, gridRect);
			DrawOpacityPanelBorder(e.Graphics, gridRect);
			DrawOpacityPanelForeground(e.Graphics, gridRect);
			DrawOpacityPanelGrip(e.Graphics, gridRect);
		}
		void DrawOpacityPanelLabel(Graphics graphics, Rectangle clientRect) {
			string text = ColorModelSettings.OpacityAxisName;
			DrawHorizontalLabel(graphics, text, Control.SelectOpacityAreaRect.X, clientRect.Height - DialogSettings.INT_Bottom + DialogSettings.OpacityPanelContentOffset, clientRect.Width - 4, Control.OpacityPanel.Focused);
		}
		void DrawOpacityPanelBackground(Graphics graphics, Rectangle clientRect, Rectangle gridRect) {
			using(HatchBrush newSolidBrush = new HatchBrush(HatchStyle.SolidDiamond, Color.SkyBlue, Color.White)) {
				graphics.FillRectangle(newSolidBrush, gridRect);
			}
		}
		void DrawOpacityPanelBorder(Graphics graphics, Rectangle gridRect) {
			Rectangle newGridRect = new Rectangle(gridRect.Location, gridRect.Size);
			newGridRect.Inflate(2, 2);
			graphics.DrawRectangle(SystemPens.ControlDark, newGridRect);
		}
		void DrawOpacityPanelForeground(Graphics graphics, Rectangle gridRect) {
			int red = Control.SelectedColor.R, green = Control.SelectedColor.G, blue = Control.SelectedColor.B, width = gridRect.Width;
			for(int i = 0; i < width; i++) {
				int alpha = (int)Math.Round(i * 255.0 / width);
				Color color = Color.FromArgb(alpha, red, green, blue);
				using(Pen opacityPen = new Pen(color)) {
					int xPos = gridRect.X + i + 1;
					graphics.DrawLine(opacityPen, xPos, gridRect.Top, xPos, gridRect.Bottom);
				}
			}
		}
		void DrawOpacityPanelGrip(Graphics graphics, Rectangle gridRect) {
			Color borderColor;
			float penWidth = Control.OpacityPanel.Focused ? 1.5f : 1f;
			if(Control.OpacityPanel.Focused && Control.CursorOn) borderColor = Color.Black;
			else borderColor = Color.FromArgb(192, Color.Black);
			using(Pen borderPen = new Pen(borderColor, penWidth)) {
				int topFlatEdge = gridRect.Y - 2;
				int xPos = gridRect.X - 1 + (int)Math.Round(Control.OpacityCore / 255.0 * (gridRect.Width + 2));
				int arrowLength = 6, arrowWidth = 4;
				Point[] topArrow = { new Point(xPos, topFlatEdge + arrowLength), new Point(xPos - arrowWidth, topFlatEdge), new Point(xPos + arrowWidth, topFlatEdge) };
				graphics.FillPolygon(Brushes.White, topArrow);
				graphics.DrawPolygon(borderPen, topArrow);
				int bottomFlatEdge = gridRect.Y + gridRect.Height + 2;
				Point[] bottomArrow = { new Point(xPos, bottomFlatEdge - arrowLength), new Point(xPos - arrowWidth, bottomFlatEdge), new Point(xPos + arrowWidth, bottomFlatEdge) };
				graphics.FillPolygon(Brushes.White, bottomArrow);
				graphics.DrawPolygon(borderPen, bottomArrow);
			}
		}
		#endregion
	}
	public class RGBColorModelFormPainter : ColorModelFormPainterBase {
		public RGBColorModelFormPainter(ColorModelUserControlAbstractBase control) : base(control) { }
		public override void DrawColorGrid(PaintEventArgs e) {
			Rectangle clientRect = Control.ColorGridPanel.ClientRectangle;
			ClearBackground(e.Graphics, Control.ColorGridPanel);
			DrawColorGridCore(e.Graphics);
			clientRect.Height -= DialogSettings.INT_Bottom - 1;
			DrawControlBorder(e.Graphics, clientRect);
			PaintGridCursor(e.Graphics);
			DrawGridLabels(e.Graphics);
		}
		public override void DrawGradientPanel(PaintEventArgs e) {
			Rectangle clientRect = Control.GradientPanel.ClientRectangle;
			clientRect.Height = Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top + 1;
			Graphics graphics = e.Graphics;
			ClearBackground(graphics, Control.GradientPanel);
			DrawLightnessGradient(graphics, clientRect);
			DrawControlBorder(graphics, clientRect);
			DrawGradientCursor(graphics);
			DrawGradientLabel(graphics, clientRect);
		}
		void DrawLightnessGradient(Graphics graphics, Rectangle clientRect) {
			HueSatLight hsl = new HueSatLight();
			hsl.Hue = (float)Control.GridXCore / Control.CalculationWidth;
			hsl.Saturation = (Control.CalculationHeight - Control.GridYCore) / 255.0;
			ColorModelUserControlAbstractBase.PrepareHueSatLight(hsl);
			int top = DialogSettings.INT_Top;
			int steps = clientRect.Height;
			int panelWidth = Control.GradientPanel.Width - DialogSettings.INT_Right;
			for(int i = top; i < steps; i++) {
				hsl.Lightness = (float)(steps - i) / steps;
				using(Pen linePen = new Pen(DevExpress.Utils.Colors.ColorConverter.FromHueSatLight(hsl))) {
					graphics.DrawLine(linePen, new Point(DialogSettings.INT_Left, i), new Point(panelWidth, i));
				}
			}
		}
		void DrawColorGridCore(Graphics graphics) {
			int steps, panelHeight;
			Control.InitColorGridValues(out steps, out panelHeight);
			HueSatBright hsb = new HueSatBright();
			hsb.Brightness = ColorModelSettings.RgbColorGridBrightnessValue;
			for(int i = 0; i < steps; i++) {
				hsb.Hue = (double)i / steps;
				hsb.Saturation = 1.0;
				Color start = hsb.AsRGB;
				hsb.Saturation = 0;
				Color end = hsb.AsRGB;
				DrawGradientLine(graphics, DialogSettings.INT_Top, panelHeight, i, start, end);
			}
		}
	}
	public class HSBColorModelFormPainter : ColorModelFormPainterBase {
		public HSBColorModelFormPainter(ColorModelUserControlAbstractBase control) : base(control) { }
		public override void DrawColorGrid(PaintEventArgs e) {
			double value = Control.GradientValue;
			Rectangle clientRect = Control.ColorGridPanel.ClientRectangle;
			ClearBackground(e.Graphics, Control.ColorGridPanel);
			switch(Control.SelectedColorEditOption) {
				case ColorEditOption.Hue:
					DrawSaturationLightnessGrid(e.Graphics, value);
					break;
				case ColorEditOption.Saturation:
					DrawHueLightnessGrid(e.Graphics, value);
					break;
				case ColorEditOption.Brightness:
					DrawHueSaturationGrid(e.Graphics, value);
					break;
			}
			clientRect.Height -= DialogSettings.INT_Bottom - 1;
			DrawControlBorder(e.Graphics, clientRect);
			PaintGridCursor(e.Graphics);
			DrawGridLabels(e.Graphics);
		}
		public override void DrawGradientPanel(PaintEventArgs e) {
			Rectangle clientRect = Control.GradientPanel.ClientRectangle;
			clientRect.Height = Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top + 1;
			Graphics graphics = e.Graphics;
			ClearBackground(graphics, Control.GradientPanel);
			switch(Control.SelectedColorEditOption) {
				case ColorEditOption.Hue:
					DrawHueGradient(graphics);
					break;
				case ColorEditOption.Saturation:
					DrawSaturationGradient(graphics);
					break;
				case ColorEditOption.Brightness:
					DrawBrightnessGradient(graphics);
					break;
			}
			DrawControlBorder(graphics, clientRect);
			DrawGradientCursor(graphics);
			DrawGradientLabel(graphics, clientRect);
		}
		void DrawSaturationLightnessGrid(Graphics graphics, double startingHue) {
			int steps, panelHeight;
			Control.InitColorGridValues(out steps, out panelHeight);
			HueSatBright hsb = new HueSatBright();
			hsb.Hue = (float)startingHue;
			for(int i = 0; i < steps; i++) {
				hsb.Saturation = (float)i / steps;
				hsb.Brightness = 1.0f;
				Color start = hsb.AsRGB;
				hsb.Brightness = 0;
				Color end = hsb.AsRGB;
				DrawGradientLine(graphics, DialogSettings.INT_Top, panelHeight, i, start, end);
			}
		}
		void DrawHueLightnessGrid(Graphics graphics, double startingSaturation) {
			int steps, panelHeight;
			Control.InitColorGridValues(out steps, out panelHeight);
			HueSatBright hsb = new HueSatBright();
			hsb.Saturation = (float)startingSaturation;
			for(int i = 0; i < steps; i++) {
				hsb.Hue = (float)i / steps;
				hsb.Brightness = 1.0f;
				Color start = hsb.AsRGB;
				hsb.Brightness = 0.0f;
				Color end = hsb.AsRGB;
				DrawGradientLine(graphics, DialogSettings.INT_Top, panelHeight, i, start, end);
			}
		}
		void DrawHueSaturationGrid(Graphics graphics, double startingLightness) {
			int steps, panelHeight;
			Control.InitColorGridValues(out steps, out panelHeight);
			HueSatBright hsb = new HueSatBright();
			hsb.Brightness = startingLightness;
			for(int i = 0; i < steps; i++) {
				hsb.Hue = (double)i / steps;
				hsb.Saturation = 1.0;
				Color start = hsb.AsRGB;
				hsb.Saturation = 0;
				Color end = hsb.AsRGB;
				DrawGradientLine(graphics, DialogSettings.INT_Top, panelHeight, i, start, end);
			}
		}
		void DrawHueGradient(Graphics graphics) {
			HueSatLight hsl = new HueSatLight();
			hsl.Lightness = 0.5;
			hsl.Saturation = 1.0;
			int steps = Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top + 1;
			int right = Control.GradientPanel.Width - DialogSettings.INT_Right;
			int upperBound = steps;
			for(int i = DialogSettings.INT_Top; i < upperBound; i++) {
				hsl.Hue = (double)(steps - i) / steps;
				using(Pen linePen = new Pen(ColorWrapper.ConvertTo<Color>(hsl.AsRGB)))
					graphics.DrawLine(linePen, new Point(DialogSettings.INT_Left, i), new Point(right, i));
			}
		}
		void DrawBrightnessGradient(Graphics graphics) {
			HueSatBright hsb = new HueSatBright();
			hsb.Hue = (double)Control.GridXCore / Control.CalculationWidth;
			int steps = Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top + 1;
			int panelWidth = Control.GradientPanel.Width - DialogSettings.INT_Right;
			hsb.Saturation = (Control.CalculationHeight - Control.GridYCore) / 255.0;
			for(int i = DialogSettings.INT_Top; i < steps; i++) {
				hsb.Brightness = (float)(steps - i) / steps;
				using(Pen linePen = new Pen(hsb.AsRGB)) {
					graphics.DrawLine(linePen, new Point(DialogSettings.INT_Left, i), new Point(panelWidth, i));
				}
			}
		}
		void DrawSaturationGradient(Graphics graphics) {
			HueSatBright hsb = new HueSatBright();
			hsb.Hue = (float)Control.GridXCore / Control.CalculationWidth;
			int steps = Control.SelectGradientAreaRect.Height + DialogSettings.INT_Top + 1;
			int panelWidth = Control.GradientPanel.Width - DialogSettings.INT_Right;
			double brightness = (Control.SelectGradientAreaRect.Height - Control.GridYCore) / 255.0;
			hsb.Brightness = brightness * 0.7 + 0.3;
			for(int i = DialogSettings.INT_Top; i < steps; i++) {
				hsb.Saturation = (float)(steps - i) / steps;
				using(Pen linePen = new Pen(hsb.AsRGB)) {
					graphics.DrawLine(linePen, new Point(DialogSettings.INT_Left, i), new Point(panelWidth, i));
				}
			}
		}
	}
	class ColorPickerValidationHelper {
		public static bool IsRgbValid(string text) {
			return IsIntValueInRange(text, 0, 255);
		}
		public static bool IsOpacityValid(string text) {
			return IsRgbValid(text);
		}
		public static bool IsRgbHexValid(string text) {
			if(string.IsNullOrEmpty(text))
				return false;
			if(text.Length > 8)
				return false;
			uint res;
			return uint.TryParse(text, NumberStyles.HexNumber, null, out res);
		}
		public static bool IsHueValid(string text) {
			return IsFloatValueInRange(text, 0, 359);
		}
		public static bool IsSatBrightValid(string text) {
			return IsFloatValueInRange(text, 0, 100);
		}
		static bool IsIntValueInRange(string text, int left, int right) {
			if(string.IsNullOrEmpty(text))
				return false;
			int res;
			return int.TryParse(text, out res) && res >= left && res <= right;
		}
		static bool IsFloatValueInRange(string text, int left, int right) {
			if(string.IsNullOrEmpty(text))
				return false;
			float res;
			return float.TryParse(text, out res) && res >= left && res <= right;
		}
		public static void ValidateCore(DXErrorProvider.DXErrorProvider dxp, Validator callback, Control control, string text, CancelEventArgs e, string errorMsg) {
			if(callback(text)) {
				dxp.SetError(control, string.Empty);
				e.Cancel = false;
			}
			else {
				dxp.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
				dxp.SetError(control, errorMsg);
				e.Cancel = true;
			}
		}
		public delegate bool Validator(string text);
	}
	public abstract class SuperToolTipManagerBase {
		public static SuperToolTipManagerBase Create(ColorModel model) {
			if(model == ColorModel.RGB)
				return new RGBSuperTiptManager();
			return new HSBSuperTipManager();
		}
		public void RefreshOpacityInfo(ColorModelUserControlAbstractBase control, ToolTip toolip, MouseEventArgs e, bool capture) {
			Rectangle gridRect = control.SelectOpacityAreaRect;
			if(capture)
				control.SetColorOpacityFromMouse(e.X);
			if(control.LastOpacityHover != e.X) {
				control.LastOpacityHover = e.X;
				if(!gridRect.Contains(e.Location)) {
					toolip.RemoveAll();
					return;
				}
				int percent = (int)Math.Ceiling(100.0 * (e.X - DialogSettings.INT_Left) / gridRect.Width);
				if(percent > 100)
					percent = 100;
				if(percent < 0)
					percent = 0;
				toolip.SetToolTip(control.OpacityPanel, percent.ToString() + "%");
			}
		}
		public virtual void RefreshSampleInfo(ToolTipControllerShowEventArgs e, ColorModelUserControlAbstractBase control) {
			Color color = control.SelectedColor;
			ToolTipTitleItem titleItem = GetToolTipTitleItem(e);
			titleItem.Text = FormatTitleCore(titleItem, color);
			ToolTipItem contentItem = GetToolTipContentItem(e);
			contentItem.Text = FormatContentCore(contentItem, control);
		}
		protected ToolTipTitleItem GetToolTipTitleItem(ToolTipControllerShowEventArgs e) {
			return e.SuperTip.Items[0] as ToolTipTitleItem;
		}
		protected ToolTipItem GetToolTipContentItem(ToolTipControllerShowEventArgs e) {
			return e.SuperTip.Items[1] as ToolTipItem;
		}
		protected virtual string FormatTitleCore(ToolTipTitleItem titleItem, Color color) {
			return string.Format("#{0}{1}{2}{3}", color.A.ToString("X02"), color.R.ToString("X02"), color.G.ToString("X02"), color.B.ToString("X02"));
		}
		public abstract string FormatContentCore(ToolTipItem contentItem, ColorModelUserControlAbstractBase control);
	}
	class RGBSuperTiptManager : SuperToolTipManagerBase {
		public override string FormatContentCore(ToolTipItem contentItem, ColorModelUserControlAbstractBase control) {
			Color color = control.SelectedColor;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GetRedComponentInfo(color));
			sb.AppendLine(GetGreenComponentInfo(color));
			sb.AppendLine(GetBlueComponentInfo(color));
			sb.AppendLine(GetOpacityComponentInfo(color));
			return sb.ToString();
		}
		string GetRedComponentInfo(Color color) {
			return string.Format("Red:       {0}", color.R.ToString());
		}
		string GetGreenComponentInfo(Color color) {
			return string.Format("Green:    {0}", color.G.ToString());
		}
		string GetBlueComponentInfo(Color color) {
			return string.Format("Blue:       {0}", color.B.ToString());
		}
		string GetOpacityComponentInfo(Color color) {
			return string.Format("Opacity: {0}", color.A.ToString());
		}
	}
	class HSBSuperTipManager : SuperToolTipManagerBase {
		public override string FormatContentCore(ToolTipItem contentItem, ColorModelUserControlAbstractBase control) {
			Color color = control.SelectedColor;
			HueSatBright hsb = control.SelectedHsb;
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(GetHueComponentInfo(hsb));
			sb.AppendLine(GetSatComponentInfo(hsb));
			sb.AppendLine(GetBrightComponentInfo(hsb));
			sb.AppendLine(GetOpacityComponentInfo(color));
			return sb.ToString();
		}
		string GetHueComponentInfo(HueSatBright hsb) {
			return string.Format("Hue:       {0}", (hsb.Hue * 360).ToString("0.0"));
		}
		string GetSatComponentInfo(HueSatBright hsb) {
			return string.Format("Sat:        {0}", (hsb.Saturation * 100).ToString("0.0"));
		}
		string GetBrightComponentInfo(HueSatBright hsb) {
			return string.Format("Bright:    {0}", (hsb.Brightness * 100).ToString("0.0"));
		}
		string GetOpacityComponentInfo(Color color) {
			return string.Format("Opacity: {0}", color.A.ToString());
		}
	}
	[ToolboxItem(false)]
	public class SelectablePanel : Panel {
		public SelectablePanel() {
			SetStyle(ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ContainerControl, false);
		}
	}
}
