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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Mask;
using DevExpress.Skins;
using System.Globalization;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.Repository {
	public class RepositoryItemCalcEdit : RepositoryItemPopupBase {
		bool showCloseButton;
		int precision;
		public RepositoryItemCalcEdit() {
			showCloseButton = false;
			precision = 6;
		}
		protected override MaskProperties CreateMaskProperties() {
			return new NumericMaskProperties();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public new RepositoryItemCalcEdit Properties { get { return this; } }
		private static object valueChanged = new object();
		[Browsable(false)]
		public new CalcEdit OwnerEdit { get { return base.OwnerEdit as CalcEdit; } }
		[Browsable(false)]
		public override string EditorTypeName { get { return "CalcEdit"; } }
		[Browsable(false)]
		public override HorzAlignment DefaultAlignment { get { return HorzAlignment.Far; } }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCalcEditShowCloseButton"),
#endif
 DefaultValue(false), SmartTagProperty("Show Close Button", "", 0, SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				showCloseButton = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Size PopupFormSize {
			get { return Size.Empty; }
			set { }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCalcEditPrecision"),
#endif
 DefaultValue(6), SmartTagProperty("Precision", "")]
		public int Precision {
			get { return precision; }
			set {
				if(value < 0 || value > 28) return;
				precision = value;
				OnPropertiesChanged();
			}
		}
		public override void Assign(RepositoryItem item) {
			RepositoryItemCalcEdit source = item as RepositoryItemCalcEdit;
			BeginUpdate();
			try {
				base.Assign(item);
				if(source == null) return;
				this.precision = source.Precision;
				this.showCloseButton = source.ShowCloseButton;
			} finally {
				EndUpdate();
			}
			Events.AddHandler(valueChanged, source.Events[valueChanged]);
		}
		protected internal override bool AllowInputOnOpenPopup { get { return false; } }
		public override string GetDisplayText(FormatInfo format, object editValue) {
			if(!(editValue is decimal)) return base.GetDisplayText(format, editValue);
			else return base.GetDisplayText(format, CheckMaxFractionLength((decimal)editValue));
		}
		protected internal virtual decimal CheckMaxFractionLength(decimal num) {
			return decimal.Round(num, Precision);
		}
		protected override bool NeededKeysPopupContains(Keys key) {
			if(key == (Keys.Enter | Keys.Control))
				return true;
			if(key == Keys.Enter)
				return true;
			return base.NeededKeysPopupContains(key);
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCalcEditEditMask"),
#endif
		DXCategory(CategoryName.Format),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RefreshProperties(RefreshProperties.All)
		]
		public string EditMask {
			get { return Mask.EditMask; }
			set { Mask.EditMask = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCalcEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ValueChanged {
			add { this.Events.AddHandler(valueChanged, value); }
			remove { this.Events.RemoveHandler(valueChanged, value); }
		}
		protected override void RaiseEditValueChangedCore(EventArgs e) {
			base.RaiseEditValueChangedCore(e);
			RaiseValueChanged(e);
		}
		protected internal virtual void RaiseValueChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[valueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
	}
}
namespace DevExpress.XtraEditors {
	[DefaultBindingPropertyEx("Value"), Designer("DevExpress.XtraEditors.Design.CalcEditDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
	 Description("Allows an end-user to edit numeric values and perform custom calculation via a dropdown calculator."),
	 ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxBitmap(typeof(ToolboxIconsRootNS), "CalcEdit")
	]
	public class CalcEdit : PopupBaseEdit {
		public CalcEdit() { }
		[Browsable(false)]
		public override string EditorTypeName { get { return "CalcEdit"; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CalcEditProperties"),
#endif
 DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), SmartTagSearchNestedProperties]
		public new RepositoryItemCalcEdit Properties { get { return base.Properties as RepositoryItemCalcEdit; } }
		[Bindable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CalcEditValue"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(ControlConstants.NonObjectBindable), SmartTagProperty("Value", "")]
		public virtual decimal Value {
			get {
				FlushPendingEditActions();
				return ConvertToDecimal(EditValue);
			}
			set {
				EditValue = ConvertFromDecimal(value);
			}
		}
		[Browsable(false)]
		public override object EditValue {
			get { return base.EditValue; }
			set { base.EditValue = value; }
		}
		protected override bool IsNeedHideCursorOnPopup { get { return true; } }
		public override bool DoValidate(PopupCloseMode closeMode) {
			DoClosePopup(closeMode);
			return base.DoValidate(closeMode);
		}
		protected internal override bool AllowPopupTabOut { get { return false; } }
		protected override object ExtractParsedValue(ConvertEditValueEventArgs e) {
			if(e.Handled)
				return e.Value;
			if(Properties.IsNullInputAllowed && Properties.IsNullValue(e.Value))
				return null;
			return ConvertToDecimal(e.Value);
		}
		object ConvertFromDecimal(decimal val) {
			return val;
		}
		decimal ConvertToDecimal(object val) {
			if(val == null)
				return Decimal.Zero;
			if(Properties.Mask.MaskType == MaskType.Numeric && val is string) {
				try {
					return Decimal.Parse((string)val, System.Globalization.CultureInfo.InvariantCulture);
				} catch { }
			}
			try {
				return Convert.ToDecimal(val);
			} catch { }
			return Decimal.Zero;
		}
		protected override PopupBaseForm CreatePopupForm() {
			return new PopupCalcEditForm(this);
		}
		protected internal new PopupCalcEditForm PopupForm { get { return base.PopupForm as PopupCalcEditForm; } }
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			base.OnPopupClosed(closeMode);
			this.UpdateMaskBox();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CalcEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler ValueChanged {
			add { Properties.ValueChanged += value; }
			remove { Properties.ValueChanged -= value; }
		}
	}
}
namespace DevExpress.XtraEditors.ViewInfo {
	public class CalcEditViewInfo : PopupBaseEditViewInfo {
		public CalcEditViewInfo(RepositoryItem item)
			: base(item) {
		}
		public override object EditValue {
			get { return fEditValue; }
			set {
				fEditValue = value;
				OnEditValueChanged();
			}
		}
	}
}
namespace DevExpress.XtraEditors.Popup {
	public class PopupCalcEditFormViewInfo : CustomBlobPopupFormViewInfo {
		public PopupCalcEditFormViewInfo(PopupBaseForm form)
			: base(form) {
			ShowSizeBar = Form.Properties.ShowCloseButton;
		}
		public virtual int CalcBtnIndent { get { return 3; } }
		public virtual int MemBtnIndent { get { return 6; } }
		public new PopupCalcEditForm Form { get { return base.Form as PopupCalcEditForm; } }
	}
	public class PopupCalcEditFormPainter : PopupBaseSizeableFormPainter {
		protected override void DrawContent(PopupFormGraphicsInfoArgs info) {
			base.DrawContent(info);
			PopupCalcEditFormViewInfo vi = info.ViewInfo as PopupCalcEditFormViewInfo;
			if(!vi.ShowSizeBar) return;
			Rectangle underLineRect = new Rectangle(vi.ContentRect.Left + vi.CalcBtnIndent,
				vi.IsTopSizeBar ? vi.SizeBarRect.Bottom - 1 : vi.SizeBarRect.Top, vi.ContentRect.Width - 2 * vi.CalcBtnIndent, 1);
			info.Cache.Graphics.FillRectangle(SystemBrushes.ControlDark, underLineRect);
		}
	}
	public enum CalcStatus { Ok, OkEnteringDigits, Error };
	public class PopupCalcEditForm : CustomBlobPopupForm {
		const int topButtonsCount = 3;
		const int buttonsColCount = 6;
		const int buttonsRowCount = 4;
		protected const int ButtonCount = 28;
		protected CalcStatus fStatus;
		protected CalcButtonType fKeyOperator, fKeyPushedButton;
		protected string fDisplayText;
		private decimal memory, result, displayValue;
		private bool isModified;
		protected static readonly string calculatorDisplayFormat;
		static PopupCalcEditForm() {
			string work = new string('#', 29) + "0." + new string('#', 30);
			calculatorDisplayFormat = work + ";-" + work;
		}
		protected CultureInfo Culture {
			get {
				CultureInfo culture = Properties.Mask.Culture;
				if(culture == null)
					culture = CultureInfo.CurrentCulture;
				return culture;
			}
		}
		protected string DecimalSeparator {
			get {
				return Culture.NumberFormat.NumberDecimalSeparator;
			}
		}
		public PopupCalcEditForm(PopupBaseEdit ownerEdit)
			: base(ownerEdit) {
			this.fShowOkButton = false;
			this.AllowSizing = false;
			this.fCloseButtonStyle = (Properties.ShowCloseButton ? BlobCloseButtonStyle.Glyph : BlobCloseButtonStyle.None);
			InitCalculatorValues();
			CreateCalcButtons();
		}
		protected override Size MinFormSize { get { return Size.Empty; } }
		protected virtual void InitCalculatorValues() {
			this.memory = decimal.Zero;
			this.result = this.displayValue = OwnerEdit.Value;
			this.fKeyOperator = CalcButtonType.Equal;
			this.fKeyPushedButton = CalcButtonType.None;
			this.fStatus = CalcStatus.Ok;
			this.isModified = false;
			if(GetButton(MemButtonIndex) != null) OnMemoryChanged();
			SetDisplayText(displayValue, true);
		}
		protected virtual void CreateCalcButtons() {
			string[] szCaptions = new string[]{ Localizer.Active.GetLocalizedString(StringId.CalcButtonMC), 
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonMR), 
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonMS), 
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonMx),
												  "7", "4", "1", "0", "8", "5", "2", "+/-", "9", "6", "3", 
												  DecimalSeparator,
												  "/", "*", "-", "+",
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonSqrt),
												  "%", "1/x", "=",
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonBack), 
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonCE), 
												  Localizer.Active.GetLocalizedString(StringId.CalcButtonC)};
			Color[] clrColors = new Color[]{Color.Red, Color.Red, Color.Red, Color.Red,
											   Color.Blue, Color.Blue, Color.Blue, Color.Blue, Color.Blue, Color.Blue,
											   Color.Blue, Color.Blue, Color.Blue, Color.Blue, Color.Blue, Color.Blue,
											   Color.Red, Color.Red, Color.Red, Color.Red,
											   Color.DarkRed, Color.DarkBlue, Color.DarkBlue, Color.Red,
											   Color.DarkRed, Color.DarkRed, Color.DarkRed};
			CalcButtonType[] types = new CalcButtonType[] {CalcButtonType.MC, CalcButtonType.MR, CalcButtonType.MS, CalcButtonType.MAdd,
															  CalcButtonType.Seven, CalcButtonType.Four, CalcButtonType.One, CalcButtonType.Zero, CalcButtonType.Eight,
															  CalcButtonType.Five, CalcButtonType.Two, CalcButtonType.Sign, CalcButtonType.Nine, CalcButtonType.Six, CalcButtonType.Three,
															  CalcButtonType.Decimal, CalcButtonType.Div, CalcButtonType.Mul, CalcButtonType.Sub, CalcButtonType.Add,
															  CalcButtonType.Sqrt, CalcButtonType.Percent, CalcButtonType.Fract, CalcButtonType.Equal,
															  CalcButtonType.Back, CalcButtonType.Cancel, CalcButtonType.Clear};
			Control[] buttons = new CalculatorButton[ButtonCount];
			for(int i = (int)CalcButtonType.MC; i < (int)CalcButtonType.None; i++) {
				CalculatorButton button = CreateCalcButton();
				SetButtonParameters(button, szCaptions[i], CalcSkinButtonColor(clrColors[i]));
				button.ButtonType = types[i];
				button.Click += new EventHandler(CalcButton_Click);
				buttons[i] = button;
			}
			CalculatorButton memButton = CreateCalcButton();
			memButton.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			memButton.LookAndFeel.ParentLookAndFeel = Properties.LookAndFeel;
			memButton.Pushed = true;
			buttons[MemButtonIndex] = memButton;
			Controls.AddRange(buttons);
		}
		void SetButtonParameters(CalculatorButton button, string caption, Color color) {
			button.Text = caption;
			button.LookAndFeel.ParentLookAndFeel = Properties.LookAndFeel;
			button.Appearance.ForeColor = color;
			button.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			button.Appearance.Font = Properties.AppearanceDropDown.Font;
		}
		Color CalcSkinButtonColor(Color color) {
			if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return color;
			Skin skin = EditorsSkins.GetSkin(LookAndFeel);
			return GetColor(color, skin, GetSkinColorName(color));
		}
		string GetSkinColorName(Color color) {
			if(color.Name.IndexOf("Blue") > -1)
				return EditorsSkins.SkinCalcEditDigitTextColor;
			return EditorsSkins.SkinCalcEditOperationTextColor;
		}
		Color GetColor(Color color, Skin skin, string objectName) {
			Color ret = skin.Colors.GetColor(objectName);
			if(ret.IsEmpty) return color;
			return ret;
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new PopupCalcEditFormViewInfo(this);
		}
		protected override PopupBaseFormPainter CreatePainter() {
			return new PopupCalcEditFormPainter();
		}
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			Size szButton = BestButtonSize;
			int iBackLen = ((buttonsColCount - 1) * szButton.Width + (topButtonsCount - 1) * ViewInfo.CalcBtnIndent) / topButtonsCount;
			Rectangle rcPos = Rectangle.Empty;
			int x, y = szButton.Height + 2 * ViewInfo.CalcBtnIndent + ViewInfo.ContentRect.Top;
			for(int i = 0; i < buttonsColCount; i++) {
				int index = ViewInfo.IsRightToLeft ? buttonsColCount - i : i;
				x = (index != 0 ? (index * (szButton.Width + ViewInfo.CalcBtnIndent) + ViewInfo.MemBtnIndent) : ViewInfo.CalcBtnIndent) + ViewInfo.ContentRect.Left;
				if(ViewInfo.IsRightToLeft) x -= szButton.Width + (i == 0 ? ViewInfo.CalcBtnIndent : ViewInfo.MemBtnIndent);
				for(int j = 0; j < buttonsRowCount; j++) {
					rcPos = new Rectangle(x, y + j * (szButton.Height + ViewInfo.CalcBtnIndent), szButton.Width, szButton.Height);
					GetButton(i * buttonsRowCount + j).Bounds = rcPos;
				}
			}
			x = ViewInfo.IsRightToLeft ? 2 * ViewInfo.CalcBtnIndent : szButton.Width + ViewInfo.CalcBtnIndent + ViewInfo.MemBtnIndent + ViewInfo.ContentRect.Left;
			for(int i = 0; i < topButtonsCount; i++) {
				int index = ViewInfo.IsRightToLeft ? topButtonsCount - i - 1 : i;
				rcPos = new Rectangle(x + index * (iBackLen + ViewInfo.CalcBtnIndent), ViewInfo.CalcBtnIndent + ViewInfo.ContentRect.Top, iBackLen, szButton.Height);
				GetButton((int)CalcButtonType.Back + i).Bounds = rcPos;
			}
			GetButton(MemButtonIndex).Bounds = new Rectangle(ViewInfo.IsRightToLeft ? ViewInfo.ContentRect.Right - szButton.Width - ViewInfo.CalcBtnIndent : ViewInfo.CalcBtnIndent + ViewInfo.ContentRect.Left, 
				ViewInfo.CalcBtnIndent + ViewInfo.ContentRect.Top, szButton.Width, szButton.Height);
		}
		protected override Size CalcFormSizeCore() {
			Size szButton = BestButtonSize;
			return CalcFormSize(new Size(6 * (szButton.Width + ViewInfo.CalcBtnIndent) + ViewInfo.MemBtnIndent, 5 * szButton.Height + 6 * ViewInfo.CalcBtnIndent));
		}
		public override void ShowPopupForm() {
			base.ShowPopupForm();
			InitCalculatorValues();
		}
		void ValidateAndClosePopup(KeyEventArgs e) {
			e.Handled = true;
			result = displayValue;
			OwnerEdit.ClosePopup();
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			CalculatorButton pushedButton = GetButtonByKeyArgs(e);
			e.Handled = pushedButton != null;
			if(!e.Handled) return;
			if(Properties.ReadOnly && (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)) {
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			if(e.Control && e.KeyCode == Keys.Enter) {
				ValidateAndClosePopup(e);
				return;
			}
			if(e.KeyCode == Keys.Escape) {
				if((this.OwnerEdit != null && this.OwnerEdit.InplaceType != InplaceType.Standalone)) {
					ValidateAndClosePopup(e);
					return;
				}
			}
			if(Status == CalcStatus.Error && pushedButton.ButtonType != CalcButtonType.Clear) return;
			if(fKeyPushedButton != CalcButtonType.None) {
				if(pushedButton.ButtonType == fKeyPushedButton) return;
				GetButton(fKeyPushedButton).Pushed = false;
			}
			pushedButton.Pushed = true;
			fKeyPushedButton = pushedButton.ButtonType;
			CalcButton_Click(GetButton(fKeyPushedButton), EventArgs.Empty);
		}
		public override void ProcessKeyUp(KeyEventArgs e) {
			CalculatorButton pushedButton = GetButtonByKeyArgs(e);
			if(fKeyPushedButton != CalcButtonType.None) GetButton(fKeyPushedButton).Pushed = false;
			fKeyPushedButton = CalcButtonType.None;
		}
		bool IsFrenchKeyboard { get { return DevExpress.XtraEditors.Senders.FrenchKeyboardDetector.IsFrenchKeyboard; } }
		bool IsSpanishKeyboard { get { return DevExpress.XtraEditors.Senders.SpanishKeyboardDetector.IsSpanishKeyboard; } }
		protected virtual CalculatorButton GetButtonBySpanishKeyArgs(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.LButton | Keys.RButton | Keys.Back | Keys.ShiftKey | Keys.Space | Keys.F17:
					if(e.Shift) return GetButton(CalcButtonType.Mul); 
					return GetButton(CalcButtonType.Add);
				case Keys.D0:
					if(e.Shift)
						return GetButton(CalcButtonType.Equal);
					return GetButton(CalcButtonType.Zero);
				case Keys.D7:
					if(e.Shift)
						return GetButton(CalcButtonType.Div);
					return GetButton(CalcButtonType.Seven);
				case Keys.D8:
					return GetButton(CalcButtonType.Eight);
			}
			return null;
		}
		protected virtual CalculatorButton GetButtonByFrenchKeyArgs(KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.D0:
				case Keys.NumPad0: return GetButton(CalcButtonType.Zero);
				case Keys.D1:
				case Keys.NumPad1: return GetButton(CalcButtonType.One);
				case Keys.D2:
				case Keys.NumPad2: return GetButton(CalcButtonType.Two);
				case Keys.D3:
				case Keys.NumPad3: return GetButton(CalcButtonType.Three);
				case Keys.D4:
				case Keys.NumPad4: return GetButton(CalcButtonType.Four);
				case Keys.D5:
				case Keys.NumPad5: return GetButton(CalcButtonType.Five);
				case Keys.D6:
					if(e.Shift)
						return GetButton(CalcButtonType.Six);
					return GetButton(CalcButtonType.Sub);
				case Keys.NumPad6: return GetButton(CalcButtonType.Six);
				case Keys.D7:
				case Keys.NumPad7: return GetButton(CalcButtonType.Seven);
				case Keys.D8:
				case Keys.NumPad8: return GetButton(CalcButtonType.Eight);
				case Keys.D9:
				case Keys.NumPad9: return GetButton(CalcButtonType.Nine);
				case Keys.Multiply: return GetButton(CalcButtonType.Mul);
				case Keys.Divide:
				case Keys.OemQuestion:
					return GetButton(CalcButtonType.Div);
				case Keys.OemMinus:
				case Keys.Subtract: return GetButton(CalcButtonType.Sub);
				case Keys.Oemplus:
					if(e.Shift)
						return GetButton(CalcButtonType.Add);
					return GetButton(CalcButtonType.Equal);
				case Keys.Add: return GetButton(CalcButtonType.Add);
				case Keys.Back: return GetButton(CalcButtonType.Back);
				case Keys.Space:
				case Keys.Return: return GetButton(CalcButtonType.Equal);
				case Keys.Escape: return GetButton(CalcButtonType.Clear);
				case Keys.Decimal:
				case Keys.OemPeriod:
				case Keys.Oemcomma: return GetButton(CalcButtonType.Decimal);
				case Keys.Oemtilde:
					if(e.Shift) return GetButton(CalcButtonType.Percent);
					break;
				case Keys.Oem5:
					if(!e.Shift) return GetButton(CalcButtonType.Mul);
					break;
			}
			return null;
		}
		protected virtual CalculatorButton GetButtonByKeyArgs(KeyEventArgs e) {
			if(IsFrenchKeyboard) return GetButtonByFrenchKeyArgs(e);
			if(IsSpanishKeyboard) {
				CalculatorButton btn = GetButtonBySpanishKeyArgs(e);
				if(btn != null) return btn;
			}
			switch(e.KeyCode) {
				case Keys.D0:
				case Keys.NumPad0: return GetButton(CalcButtonType.Zero);
				case Keys.D1:
				case Keys.NumPad1: return GetButton(CalcButtonType.One);
				case Keys.D2:
				case Keys.NumPad2: return GetButton(CalcButtonType.Two);
				case Keys.D3:
				case Keys.NumPad3: return GetButton(CalcButtonType.Three);
				case Keys.D4:
				case Keys.NumPad4: return GetButton(CalcButtonType.Four);
				case Keys.D5:
					if(e.Shift)
						return GetButton(CalcButtonType.Percent);
					return GetButton(CalcButtonType.Five);
				case Keys.NumPad5: return GetButton(CalcButtonType.Five);
				case Keys.D6:
					if(e.Shift)
						return GetButton(CalcButtonType.Div);
					return GetButton(CalcButtonType.Six);
				case Keys.NumPad6: return GetButton(CalcButtonType.Six);
				case Keys.D7:
				case Keys.NumPad7: return GetButton(CalcButtonType.Seven);
				case Keys.D8:
					if(e.Shift)
						return GetButton(CalcButtonType.Mul);
					return GetButton(CalcButtonType.Eight);
				case Keys.NumPad8: return GetButton(CalcButtonType.Eight);
				case Keys.D9:
				case Keys.NumPad9: return GetButton(CalcButtonType.Nine);
				case Keys.Multiply: return GetButton(CalcButtonType.Mul);
				case Keys.OemQuestion:
				case Keys.Divide: return GetButton(CalcButtonType.Div);
				case Keys.OemMinus:
				case Keys.Subtract: return GetButton(CalcButtonType.Sub);
				case Keys.Oemplus:
					if(e.Shift)
						return GetButton(CalcButtonType.Add);
					return GetButton(CalcButtonType.Equal);
				case Keys.Add: return GetButton(CalcButtonType.Add);
				case Keys.Back: return GetButton(CalcButtonType.Back);
				case Keys.Space:
				case Keys.Return: return GetButton(CalcButtonType.Equal);
				case Keys.Escape: return GetButton(CalcButtonType.Clear);
				case Keys.Decimal:
				case Keys.OemPeriod:
				case Keys.Oemcomma: return GetButton(CalcButtonType.Decimal);
			}
			return null;
		}
		protected CalculatorButton GetButton(CalcButtonType buttonType) {
			return GetButton((int)buttonType);
		}
		protected CalculatorButton GetButton(int buttonIndex) {
			const int parentFormButtonCount = 2;
			if(Controls.Count - 1 < parentFormButtonCount + buttonIndex) return null;
			return Controls[parentFormButtonCount + buttonIndex] as CalculatorButton;
		}
		private void CalcButton_Click(object sender, EventArgs e) {
			if(Properties.ReadOnly) return;
			CalculatorButton btn = sender as CalculatorButton;
			try { ProcessButtonClick(btn); } catch { Error(); }
			CheckModified(btn.ButtonType);
			if(OwnerEdit != null && OwnerEdit.Properties.TextEditStyle == TextEditStyles.DisableTextEditor) OwnerEdit.Invalidate(); 
		}
		protected virtual void ProcessButtonClick(CalculatorButton btn) {
			if(Status == CalcStatus.Error && btn.ButtonType != CalcButtonType.Clear) return;
			if(IsNumber(btn.ButtonType)) {
				EnterDigit(btn.Text);
				return;
			}
			switch(btn.ButtonType) {
				case CalcButtonType.Decimal:
					EnterDecimal();
					break;
				case CalcButtonType.Percent:
					if(Status == CalcStatus.OkEnteringDigits) {
						fStatus = CalcStatus.Ok;
						displayValue *= result / 100.0m;
						SetDisplayText(displayValue, true);
					}
					break;
				case CalcButtonType.Equal:
					switch(fKeyOperator) {
						case CalcButtonType.Add: result += displayValue; break;
						case CalcButtonType.Sub: result -= displayValue; break;
						case CalcButtonType.Mul: result *= displayValue; break;
						case CalcButtonType.Div: if(displayValue == decimal.Zero) { Error(); return; } else result /= displayValue; break;
						case CalcButtonType.Equal: result = displayValue; break;
					}
					fKeyOperator = CalcButtonType.Equal;
					fStatus = CalcStatus.Ok;
					SetDisplayText(result, true);
					break;
				case CalcButtonType.Add:
				case CalcButtonType.Sub:
				case CalcButtonType.Mul:
				case CalcButtonType.Div:
					if(Status != CalcStatus.Error) {
						fStatus = CalcStatus.Ok;
						switch(fKeyOperator) {
							case CalcButtonType.Add:
								SetDisplayText(result + displayValue, true);
								break;
							case CalcButtonType.Sub:
								SetDisplayText(result - displayValue, true);
								break;
							case CalcButtonType.Mul:
								SetDisplayText(result * displayValue, true);
								break;
							case CalcButtonType.Div:
								if(displayValue == decimal.Zero)
									Error();
								else
									SetDisplayText(result / displayValue, true);
								break;
						}
						result = displayValue;
					}
					fKeyOperator = btn.ButtonType;
					break;
				case CalcButtonType.Sqrt:
					if(displayValue < decimal.Zero) Error();
					else {
						fStatus = CalcStatus.Ok;
						SetDisplayText(Sqrt(displayValue), true);
						if(fKeyOperator == CalcButtonType.Equal)
							result = displayValue;
					}
					break;
				case CalcButtonType.Fract:
					if(displayValue == decimal.Zero)
						Error();
					else {
						fStatus = CalcStatus.Ok;
						SetDisplayText(1 / displayValue, true);
						if(fKeyOperator == CalcButtonType.Equal)
							result = displayValue;
					}
					break;
				case CalcButtonType.Sign:
					SetDisplayText(-displayValue, true);
					if(fKeyOperator == CalcButtonType.Equal)
						result = displayValue;
					fStatus = CalcStatus.Ok;
					break;
				case CalcButtonType.Back:
					ProcessBackKey();
					break;
				case CalcButtonType.Cancel:
					fStatus = CalcStatus.Ok;
					SetDisplayText(decimal.Zero, true);
					break;
				case CalcButtonType.Clear:
					fKeyOperator = CalcButtonType.Equal;
					fStatus = CalcStatus.Ok;
					result = decimal.Zero;
					SetDisplayText(result, true);
					break;
				case CalcButtonType.MC:
					memory = decimal.Zero;
					OnMemoryChanged();
					fStatus = CalcStatus.Ok;
					break;
				case CalcButtonType.MR:
					fStatus = CalcStatus.Ok;
					SetDisplayText(memory, true);
					break;
				case CalcButtonType.MS:
					memory = displayValue;
					OnMemoryChanged();
					fStatus = CalcStatus.Ok;
					break;
				case CalcButtonType.MAdd:
					memory += displayValue;
					OnMemoryChanged();
					fStatus = CalcStatus.Ok;
					break;
			}
		}
		protected bool CheckFirst() {
			if(Status == CalcStatus.Ok) {
				fStatus = CalcStatus.OkEnteringDigits;
				displayValue = decimal.Zero;
				return true;
			}
			return false;
		}
		private void CheckModified(CalcButtonType buttonType) {
			if(!isModified) isModified = IsModifier(buttonType);
		}
		protected bool IsNumber(CalcButtonType bt) {
			switch(bt) {
				case CalcButtonType.Zero:
				case CalcButtonType.One:
				case CalcButtonType.Two:
				case CalcButtonType.Three:
				case CalcButtonType.Four:
				case CalcButtonType.Five:
				case CalcButtonType.Six:
				case CalcButtonType.Seven:
				case CalcButtonType.Eight:
				case CalcButtonType.Nine:
					return true;
				default:
					return false;
			}
		}
		private bool IsModifier(CalcButtonType buttonType) {
			if(IsNumber(buttonType)) return true;
			return (buttonType == CalcButtonType.Fract || buttonType == CalcButtonType.Back ||
				buttonType == CalcButtonType.Sqrt || buttonType == CalcButtonType.Clear || buttonType == CalcButtonType.Sign);
		}
		protected void OnMemoryChanged() {
			GetButton(MemButtonIndex).Text = (memory != decimal.Zero ? "M" : string.Empty);
		}
		protected void Error() {
			fStatus = CalcStatus.Error;
			fKeyOperator = CalcButtonType.Equal;
			UpdateOwnerDisplay(DisplayText);
		}
		protected virtual void SetDisplayText(decimal val, bool convert) {
			val = Properties.CheckMaxFractionLength(val);
			displayValue = val;
			if(convert) {
				string work = val.ToString(calculatorDisplayFormat, Properties.Mask.Culture);
				fDisplayText = work;
			}
			UpdateOwnerDisplay(DisplayText);
		}
		protected virtual void UpdateOwnerDisplay(string overrideText) {
			OwnerEdit.MaskBox.OverrideDisplayText(overrideText);
			OwnerEdit.SelectAll();
		}
		protected decimal toDecimal(string val) {
			if(val == null || val.Length == 0)
				return Decimal.Zero;
			decimal rv;
			if(decimal.TryParse(val, NumberStyles.Number, Culture, out rv))
				return rv;
			val = ExtractDecimalNumber(val);
			if(val.IndexOf(DecimalSeparator) == val.Length - DecimalSeparator.Length)
				val = val.Remove(val.Length - DecimalSeparator.Length, DecimalSeparator.Length);
			return Convert.ToDecimal(val);
		}
		protected internal string ExtractDecimalNumber(string val) {
			string num = string.Empty;
			bool rightSymb = false;
			int sepCount = 0;
			for(int i = 0; i < val.Length; i++) {
				rightSymb = false;
				char ch = val[i];
				rightSymb = char.IsDigit(ch);
				if(num.Length == 0 && ch == '-') rightSymb = true;
				if(sepCount == 0 && val.IndexOf(DecimalSeparator, i, DecimalSeparator.Length) == i) {
					sepCount++;
					i += Math.Max(0, DecimalSeparator.Length - 1);
					num += DecimalSeparator;
				}
				if(rightSymb) num += ch;
			}
			return num;
		}
		private void EnterDecimal() {
			if(CheckFirst()) fDisplayText = "0";
			if(DisplayText.IndexOf(DecimalSeparator) == -1) {
				fDisplayText += DecimalSeparator;
				SetDisplayText(toDecimal(DisplayText), false);
			}
		}
		string CheckMaxFractionLength(string dispText) {
			int separatorIndex = dispText.IndexOf(DecimalSeparator);
			if(separatorIndex == -1) return dispText;
			if(dispText.Length - separatorIndex > Properties.Precision)
				return dispText.Remove(separatorIndex + Properties.Precision + 1, dispText.Length - (separatorIndex + Properties.Precision + 1));
			return dispText;
		}
		private void EnterDigit(string digit) {
			string dispText = CheckFirst() ? string.Empty : DisplayText;
			fDisplayText = CheckMaxFractionLength(dispText + digit);
			SetDisplayText(toDecimal(DisplayText), false);
		}
		private decimal Sqrt(decimal value) {
			return Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(value)));
		}
		private void ProcessBackKey() {
			CheckFirst();
			decimal dispValue = decimal.Zero;
			if(DisplayText.Length > 1 && !(DisplayText.Length == 2 && DisplayText[0] == '-')) {
				fDisplayText = DisplayText.Remove(DisplayText.Length - 1, 1);
				dispValue = toDecimal(DisplayText);
			} else {
				fDisplayText = "0";
				fStatus = CalcStatus.Ok;
			}
			SetDisplayText(dispValue, false);
			if(fKeyOperator == CalcButtonType.Equal)
				result = displayValue;
		}
		protected int MemButtonIndex { get { return ButtonCount - 1; } }
		protected virtual CalculatorButton CreateCalcButton() { return new CalculatorButton(); }
		[Browsable(false)]
		public new CalcEdit OwnerEdit { get { return base.OwnerEdit as CalcEdit; } }
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemCalcEdit Properties { get { return OwnerEdit.Properties; } }
		protected new PopupCalcEditFormViewInfo ViewInfo { get { return base.ViewInfo as PopupCalcEditFormViewInfo; } }
		protected virtual Size BestButtonSize {
			get {
				Graphics g = OwnerEdit.ViewInfo.GInfo.AddGraphics(null);
				CalculatorButton tempButton = CreateCalcButton();
				SetButtonParameters(tempButton, "Wg", Color.Empty);
				try {
					CalculatorButton btn = GetButton(CalcButtonType.Sqrt);
					btn.ViewInfo.UpdatePaintAppearance();
					tempButton.ViewInfo.UpdatePaintAppearance();
					return GetMaxSize(btn.CalcBestFit(g), tempButton.CalcBestFit(g));
				} finally {
					OwnerEdit.ViewInfo.GInfo.ReleaseGraphics();
				}
			}
		}
		Size GetMaxSize(Size size1, Size size2) {
			return size1.Width >= size2.Width ? size1 : size2;
		}
		[DXCategory(CategoryName.Appearance)]
		public CalcStatus Status { get { return fStatus; } }
		[DXCategory(CategoryName.Appearance)]
		public virtual string DisplayText { get { return (Status == CalcStatus.Error ? Localizer.Active.GetLocalizedString(StringId.CalcError) : fDisplayText); } }
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue {
			get {
				if(Status == CalcStatus.Error) return null;
				if(!isModified) return OwnerEdit.EditValue;
				if(fKeyOperator == CalcButtonType.Equal) return displayValue;
				return result;
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public decimal DisplayValue { get { return displayValue; } }
	}
}
