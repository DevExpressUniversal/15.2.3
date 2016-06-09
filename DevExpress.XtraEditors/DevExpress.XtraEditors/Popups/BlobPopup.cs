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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Mask;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.Popup {
	public enum BlobCloseButtonStyle { None, Caption, Glyph }
	public class CustomBlobPopupForm : PopupBaseSizeableForm {
		SimpleButton okButton, closeButton;
		protected BlobCloseButtonStyle fCloseButtonStyle;
		protected bool fShowOkButton;
		Control separatorLine = null;
		public CustomBlobPopupForm (PopupBaseEdit ownerEdit) : base(ownerEdit) {
			CreateButtons();
			this.okButton.TabStop = this.closeButton.TabStop = false;
			this.okButton.FocusOnMouseDown = this.closeButton.FocusOnMouseDown = false;
			CloseButton.Click += new EventHandler(OnButton_Click);
			OkButton.Click += new EventHandler(OnButton_Click);
			UpdateButtons();
			Controls.Add(OkButton);
			Controls.Add(CloseButton);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(OkButton != null) {
					OkButton.Dispose();
				}
				if(CloseButton != null) {
					CloseButton.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override Control SeparatorControl { get { return separatorLine; } }
		protected void CreateSeparatorLine() {
			separatorLine = new PopupSeparatorLine(OwnerEdit.LookAndFeel);
			this.Controls.Add(separatorLine);
		}
		protected internal override int UpdateWidthWhenRightGrip(int width) {
			if(SizingBounds.X + width < OwnerEdit.PointToScreen(new Point(2, 0)).X) {
				return OwnerEdit.PointToScreen(new Point(2, 0)).X - SizingBounds.X;
			}
			return base.UpdateWidthWhenRightGrip(width);
		}
		protected virtual bool IsPopupWidthStored { get { return OwnerEdit.Properties.PropertyStore["BlobSize"] != null; } }
		protected override Size CalcFormSizeCore() {
			if(ViewInfo != null) ViewInfo.UpdatePaintAppearance();
			object blobSize = OwnerEdit.Properties.PropertyStore["BlobSize"];
			if(blobSize == null) blobSize = DefaultBlobFormSize;
			return CalcFormSize((Size)blobSize);
		}
		protected virtual Size DefaultEmptySize { get { return new Size(200, 150); } }
		protected virtual Size DefaultBlobFormSize { 
			get { 
				Size res = OwnerEdit.Properties.GetDesiredPopupFormSize(false);
				if(res.Width == 0) res.Width = DefaultEmptySize.Width;
				if(res.Height == 0) res.Height = DefaultEmptySize.Height;
				return res;
			} 
		}
		public override void HidePopupForm() {
			if(FormResized)
				SetPropertyStore(ViewInfo.ContentRect.Size);
			base.HidePopupForm();
		}
		protected virtual void SetPropertyStore(Size size) {
			OwnerEdit.Properties.PropertyStore["BlobSize"] = size;
		}
		protected virtual void SetupButtons() {
			this.fShowOkButton = false;
			this.fCloseButtonStyle = BlobCloseButtonStyle.Glyph;
		}
		protected virtual void CreateButtons() {
			SetupButtons();
			this.okButton = new SimpleButton();
			if(CloseButtonStyle == BlobCloseButtonStyle.Glyph)
				this.closeButton = new CloseButton();
			else
				this.closeButton = new SimpleButton();
			this.CloseButton.AllowFocus = false;
			UpdateEnableButtons();
		}
		internal void UpdateEnableButtons() {
			UpdateEnableButtonsCore(!Properties.ReadOnly);
		}
		protected virtual void UpdateEnableButtonsCore(bool value){
			this.okButton.Enabled = value;
		}
		public override void ShowPopupForm() {
			UpdateEnableButtons();
			base.ShowPopupForm();
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() {
			return new CustomBlobPopupFormViewInfo(this);
		}
		protected new CustomBlobPopupFormViewInfo ViewInfo { get { return base.ViewInfo as CustomBlobPopupFormViewInfo; } }
		protected void OnButton_Click(object sender, EventArgs e) {
			if(sender == CloseButton) OnCloseButtonClick();
			if(sender == OkButton) OnOkButtonClick();
		}
		protected virtual void UpdateButtons() {
			CloseButton.ButtonStyle = OkButton.ButtonStyle = OwnerEdit.Properties.ButtonsStyle;
			CloseButton.LookAndFeel.ParentLookAndFeel = OkButton.LookAndFeel.ParentLookAndFeel = OwnerEdit.LookAndFeel;
			if(CloseButton is CloseButton) {
				if(CloseButton.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003) {
					CloseButton.LookAndFeel.ParentLookAndFeel = null;
					CloseButton.LookAndFeel.UseDefaultLookAndFeel = false;
					CloseButton.LookAndFeel.UseWindowsXPTheme = true;
					CloseButton.LookAndFeel.Style = LookAndFeelStyle.Office2003;
				}
			}
			OkButton.Text = Localizer.Active.GetLocalizedString(StringId.OK);
			CloseButton.Text = Localizer.Active.GetLocalizedString(StringId.Cancel);
			UpdateButtonPainters(OkButton);
			UpdateButtonPainters(CloseButton);
		}
		protected internal void UpdateButtonPainters(SimpleButton button) {
			button.ViewInfo.UpdatePaintersCore();
		}
		[DXCategory(CategoryName.Behavior)]
		public BlobCloseButtonStyle CloseButtonStyle { get { return fCloseButtonStyle; } set { fCloseButtonStyle = value; } }
		[DXCategory(CategoryName.Behavior)]
		public bool ShowCloseButton { get { return CloseButtonStyle != BlobCloseButtonStyle.None; } }
		[DXCategory(CategoryName.Behavior)]
		public bool ShowOkButton { get { return fShowOkButton; } }
		[DXCategory(CategoryName.Behavior)]
		public SimpleButton OkButton { get { return okButton; } }
		[DXCategory(CategoryName.Behavior)]
		public SimpleButton CloseButton { get { return closeButton; } }
		protected override Size DefaultMinFormSize { get { return new Size(200, 150); } }
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			OkButton.Bounds = ViewInfo.OkButtonRect;
			CloseButton.Bounds = ViewInfo.CloseButtonRect;
			CloseButton.Visible = ShowCloseButton;
			OkButton.Visible = ShowOkButton;
		}
		protected virtual void OnCloseButtonClick() {
			OwnerEdit.CancelPopup();
		}
		protected virtual void OnOkButtonClick() {
			OwnerEdit.ClosePopup();
		}
	}
	public class CustomBlobPopupFormViewInfo : PopupBaseSizeableFormViewInfo {
		protected Rectangle fCloseButtonRect, fOkButtonRect;
		Size _buttonSize, _closeButtonSize;
		AppearanceObject paintAppearanceContent;
		public CustomBlobPopupFormViewInfo(PopupBaseForm form) : base(form) {
			this.paintAppearanceContent = new AppearanceObject();
		}
		public AppearanceObject PaintAppearanceContent { get { return paintAppearanceContent; } }
		protected override void Clear() {
			base.Clear();
			this.fCloseButtonRect = this.fOkButtonRect = Rectangle.Empty;
			this._closeButtonSize = this._buttonSize = Size.Empty;
		}
		public new CustomBlobPopupForm Form { get { return base.Form as CustomBlobPopupForm ; } }
		public Rectangle CloseButtonRect { get { return fCloseButtonRect; } }
		public Rectangle OkButtonRect { get { return fOkButtonRect; } }
		protected virtual Size ButtonSize { get { return _buttonSize; } }
		protected virtual Size CloseButtonSize { get { return _closeButtonSize; } }
		protected virtual void CalcButtonSize() {
			if(ButtonSize != Size.Empty) return;
			if(!Form.ShowCloseButton && !Form.ShowOkButton) {
				this._buttonSize = Size.Empty;
			} else {
				GInfo.AddGraphics(null);
				try {
					this._closeButtonSize = this._buttonSize = Size.Empty;
					if(Form.ShowCloseButton) {
						this._closeButtonSize = Form.CloseButton.CalcBestFit(GInfo.Graphics);
					}
					if(Form.ShowOkButton) {
						_buttonSize = Form.OkButton.CalcBestFit(GInfo.Graphics);
						_buttonSize.Width += 32; 
						if(Form.CloseButtonStyle == BlobCloseButtonStyle.Caption) {
							_buttonSize.Width = Math.Max(_buttonSize.Width, _closeButtonSize.Width);
							_buttonSize.Height = Math.Max(_buttonSize.Height, _closeButtonSize.Height);
							_closeButtonSize = _buttonSize;
						}
					}
					_buttonSize.Width = Math.Max(50, _buttonSize.Width);
				}
				finally {
					GInfo.ReleaseGraphics();
				}
			}
			return;
		}
		public override Size CalcSizeByContentSize(Size contentSize) {
			Size res = base.CalcSizeByContentSize(contentSize);
			int buttonPanelWidth = (res.Width - contentSize.Width);
			if(ShowSizeBar) buttonPanelWidth += OkButtonRect.Width + CloseButtonRect.Width + buttonIndent * 2;
			if(ShowSizeGrip) buttonPanelWidth += SizeGripRect.Width;
			res.Width = Math.Max(buttonPanelWidth, res.Width);
			return res;
		}
		protected virtual void CheckFont(AppearanceDefault appearance) {
			LookAndFeelHelper.CheckFont(Form.LookAndFeel, appearance);
		}
		protected virtual AppearanceDefault DefaultAppearanceContent { 
			get { 
				var res = new AppearanceDefault(GetSystemColor(SystemColors.WindowText), GetSystemColor(SystemColors.Window), HorzAlignment.Default, VertAlignment.Center);
				CheckFont(res);
				return res;
			} 
		}
		public override void UpdatePaintAppearance() { 
			PaintAppearance.Assign(DefaultAppearance);
			AppearanceHelper.Combine(PaintAppearanceContent, new AppearanceObject[] { Appearance, StyleController == null ? null : StyleController.AppearanceDropDown}, DefaultAppearanceContent);
		}
		protected override int CalcSizeBarHeight(Size gripSize) {
			int height = base.CalcSizeBarHeight(gripSize);
			if(Form.ShowOkButton) height += SizeBarIndent; 
			return height;
		}
		const int buttonIndent = 5;
		protected virtual int SizeBarIndent { get { return 8; } }
		protected virtual int SizeBarSideIndent { get { return 5; } }
		protected override int CalcSizeBarContentHeight() {
			CalcButtonSize();
			int height = Math.Max(CloseButtonSize.Height, ButtonSize.Height);
			return Math.Max(base.CalcSizeBarContentHeight(), height) ;
		}
		protected virtual int GetButtonXIndent() { return 0; }
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			if(SizeBarRect.IsEmpty) return;
			CalcButtonSize();
			Size size = ButtonSize;
			if(size.IsEmpty) return;
			int y = SizeBarRect.Y + (SizeBarRect.Height - size.Height + 1) / 2, x;
			x = IsRightToLeft ? SizeBarRect.Left + SizeBarSideIndent : SizeBarRect.Right - SizeBarSideIndent;
			if(!SizeGripRect.IsEmpty && !IsLeftSizeGrip && !IsRightToLeft) x = SizeGripRect.X;
			if(IsRightToLeft && !SizeGripRect.IsEmpty && IsLeftSizeGrip) x = SizeGripRect.Right;
			Rectangle r = new Rectangle(IsRightToLeft ? x : x - size.Width, y, size.Width, size.Height);
			if(Form.CloseButtonStyle != BlobCloseButtonStyle.Glyph) {
				if(Form.ShowOkButton && Form.ShowCloseButton) {
					this.fOkButtonRect.Size = this.fCloseButtonRect.Size = size;
					this.fOkButtonRect.Y = this.fCloseButtonRect.Y = y;
					int xIndent = GetButtonXIndent();
					this.fOkButtonRect.X = (x - size.Width * 2) - buttonIndent + xIndent;
					this.fCloseButtonRect.X = (x - size.Width) + xIndent;
					if(IsRightToLeft) {
						this.fOkButtonRect.X = x + size.Width + buttonIndent;
						this.fCloseButtonRect.X = x;
					}
				} else {
					if(Form.ShowOkButton) this.fOkButtonRect = r;
					if(Form.ShowCloseButton) this.fOkButtonRect = r;
				}
			} else {
				if(Form.ShowCloseButton) {
					y = SizeBarRect.Y + (SizeBarRect.Height - CloseButtonSize.Height) / 2;
					bool isRightClose = false;
					if(IsLeftSizeGrip && !SizeGripRect.IsEmpty) {
						x = SizeBarRect.Right - SizeBarSideIndent - CloseButtonSize.Width;
						isRightClose = true;
					} else {
						x = SizeBarRect.X + SizeBarSideIndent;
					}
					fCloseButtonRect = new Rectangle(x, y, CloseButtonSize.Width, CloseButtonSize.Height);
					if(Form.ShowOkButton && isRightClose) {
						r.X = SizeGripRect.Right + SizeBarSideIndent;
					}
				}
				if(Form.ShowOkButton) this.fOkButtonRect = r;
			}
		}
	}
	public class PopupSeparatorLine : LabelControl {
		public PopupSeparatorLine(UserLookAndFeel lookAndFeel) {
			Color borderColor = SystemColors.ControlDark;
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
				borderColor = new SkinElementInfo(CommonSkins.GetSkin(lookAndFeel)[CommonSkins.SkinTextBorder]).Element.Border.Bottom;
			Initialize(borderColor);
		}
		public Color Color { get { return BackColor; } set { BackColor = value; } }
		void Initialize(Color color) {
			AutoSizeMode = LabelAutoSizeMode.None;
			BorderStyle = BorderStyles.NoBorder;
			BackColor = color;
			Height = 1;
			LookAndFeel.Style = LookAndFeelStyle.Flat;
		}
	}
}
