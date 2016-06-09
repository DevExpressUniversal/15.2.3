#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.ExpressApp.Win.Core {
	public partial class ExceptionDialogForm : XtraForm {
		const int ButtonHeight = 22;
		const string labelTextAttributeName = "ExceptionDialogLabel";
		private string labelText;
		public static int Spacing = 12;
		public static int MemoHeight = 70;
		public static int MemoWidth = 400;
		private string messageText;
		XtraEditors.MemoEdit messageMemo;
		Rectangle iconRectangle;
		Rectangle messageRectangle;
		Icon icon;
		SimpleButton[] buttons;
		public UserLookAndFeel GetLookAndFeel() {
			XtraForm form = Owner as XtraForm;
			if(form != null)
				return form.LookAndFeel;
			return null;
		}
		private ExceptionDialogForm() {
			InitializeComponent();
			labelText = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.TheFollowingErrorOccurred);
			icon = SystemIcons.Error;
		}
		public static void ShowMessage(string caption, string exceptionMessage) {
			ExceptionDialogForm form = new ExceptionDialogForm();
			form.Tag = EasyTestTagHelper.FormatTestField("FormCaption");
			form.ShowMessageBoxDialog(caption, exceptionMessage);
		}
		protected virtual void ShowMessageBoxDialog(string caption, string exceptionMessage) {
			this.Text = caption;
			this.messageText = exceptionMessage;
			if(GetLookAndFeel() != null)
				LookAndFeel.Assign(GetLookAndFeel());
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = false;
			MaximizeBox = false;
			IWin32Window owner = Owner;
			if(owner == null) {
				Form activeForm = Form.ActiveForm;
				if(activeForm != null && !activeForm.InvokeRequired) {
					owner = activeForm;
				}
			}
			if(owner != null) {
				Control ownerControl = owner as Control;
				if(ownerControl != null) {
					if(!ownerControl.Visible) {
						owner = null;
					}
					else {
						Form ownerForm = ownerControl.FindForm();
						if(ownerForm != null) {
							if((!ownerForm.Visible)
								|| ownerForm.WindowState == FormWindowState.Minimized
								|| ownerForm.Right <= 0
								|| ownerForm.Bottom <= 0) {
								owner = null;
							}
						}
					}
				}
			}
			if(owner == null) {
				ShowInTaskbar = true;
				StartPosition = FormStartPosition.CenterScreen;
			}
			else {
				ShowInTaskbar = false;
				StartPosition = FormStartPosition.CenterParent;
			}
			CreateButtons();
			CalcIconBounds();
			CalcMessageBounds(); 
			CreateMemo();
			CalcFinalSizes(); 
			Form frm = owner as Form;
			if(frm != null && frm.TopMost)
				this.TopMost = true; 
			ShowDialog(owner);
		}
		void CalcMessageBounds() {
			int messageTop, messageLeft, messageWidth, messageHeight;
			messageTop = Spacing;
			messageLeft = (icon == null) ? Spacing : (this.iconRectangle.Left + this.iconRectangle.Width + Spacing);
			int maxFormWidth = this.MaximumSize.Width;
			if(maxFormWidth <= 0)
				maxFormWidth = SystemInformation.WorkingArea.Width;
			int maxTextWidth = maxFormWidth - Spacing - messageLeft;
			if(maxTextWidth < 10)
				maxTextWidth = 10;
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			SizeF textSize = GetPaintAppearance().CalcTextSize(ginfo.Graphics, labelText, maxTextWidth);
			ginfo.ReleaseGraphics();
			messageWidth = (int)Math.Ceiling(textSize.Width);
			int maxFormHeight = this.MaximumSize.Height;
			if(maxFormHeight <= 0)
				maxFormHeight = SystemInformation.WorkingArea.Height;
			int maxTextHeight = maxFormHeight - Spacing - ButtonHeight - Spacing - messageTop - SystemInformation.CaptionHeight;
			if(maxTextHeight < 10)
				maxTextHeight = 10;
			messageHeight = (int)Math.Ceiling(textSize.Height);
			if(messageHeight > maxTextHeight)
				messageHeight = maxTextHeight;
			messageWidth = messageWidth < MemoWidth ? MemoWidth : messageWidth;
			this.messageRectangle = new Rectangle(messageLeft, messageTop, messageWidth, messageHeight);
		}
		void CalcIconBounds() {
			this.iconRectangle = new Rectangle(Spacing, Spacing, icon.Width, icon.Height);
		}
		void CreateButtons() {
			buttons = new SimpleButton[1];
			Int64 i = 0;
				SimpleButton currentButton = new SimpleButton();
				currentButton.LookAndFeel.Assign(LookAndFeel);
				buttons[i] = currentButton;
				currentButton.DialogResult = DialogResult.OK;
				currentButton.Text = Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxOkButtonText);
				this.Controls.Add(currentButton);
			if(buttons.Length == 1)
				this.CancelButton = buttons[0];
			buttons[0].Select();
		}
		void CreateMemo() {
			this.messageMemo = new MemoEdit();
			messageMemo.Location = new Point(this.messageRectangle.Left, this.messageRectangle.Bottom + Spacing);
			messageMemo.Size = new Size(MemoWidth, MemoHeight);
			messageMemo.Text = messageText;
			messageMemo.Properties.ReadOnly = true;
			this.Controls.Add(this.messageMemo);
		}
		void CalcFinalSizes() {
			int buttonsTotalWidth = 0;
			foreach(SimpleButton button in buttons) {
				if(buttonsTotalWidth != 0)
					buttonsTotalWidth += Spacing;
				buttonsTotalWidth += button.Width;
			}
			int buttonsTop = this.messageRectangle.Bottom + 2 * Spacing;
			int wantedFormWidth = this.MinimumSize.Width;
			if(wantedFormWidth == 0)
				wantedFormWidth = SystemInformation.MinimumWindowSize.Width;
			int messageRectangleRight = this.messageRectangle.Right;
			if(messageRectangleRight < MemoWidth) {
				messageRectangleRight = MemoWidth;
			}
			if(wantedFormWidth < messageRectangleRight + Spacing)
				wantedFormWidth = messageRectangleRight + Spacing;
			if(wantedFormWidth < buttonsTotalWidth + Spacing + Spacing)
				wantedFormWidth = buttonsTotalWidth + Spacing + Spacing;
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				using(StringFormat fmt = TextOptions.DefaultStringFormat.Clone() as StringFormat) {
					fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
					using(Font captionFont = ControlUtils.GetCaptionFont()) {
						int maxCaptionForcedWidth = SystemInformation.WorkingArea.Width / 3 * 2;
						int captionTextWidth = 1 + 4 + (int)ginfo.Cache.CalcTextSize(this.Text, captionFont, fmt, 0).Width;
						int captionTextWidthWithButtonsAndSpacing = captionTextWidth + SystemInformation.CaptionButtonSize.Width * 5 / 4;
						int captionWidth = Math.Min(maxCaptionForcedWidth, captionTextWidthWithButtonsAndSpacing);
						if(wantedFormWidth < captionWidth)
							wantedFormWidth = captionWidth;
					}
				}
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			int formElementsWidth = 2 * SystemInformation.FixedFrameBorderSize.Width;
			int formElementsHeight = 2 * SystemInformation.FixedFrameBorderSize.Height;
			DevExpress.Skins.Skin currentSkin = DevExpress.Skins.FormSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default);
			if(currentSkin != null) {
				formElementsWidth = currentSkin[DevExpress.Skins.FormSkins.SkinFormFrameLeft].Size.MinSize.Width +
										currentSkin[DevExpress.Skins.FormSkins.SkinFormFrameRight].Size.MinSize.Width;
				formElementsHeight = currentSkin[DevExpress.Skins.FormSkins.SkinFormCaption].Size.MinSize.Height +
										currentSkin[DevExpress.Skins.FormSkins.SkinFormFrameBottom].Size.MinSize.Height;
			}
			this.Width = wantedFormWidth + formElementsWidth;
			this.Height = MemoHeight + buttonsTop + buttons[0].Height + Spacing + formElementsHeight + SystemInformation.CaptionHeight;
			int nextButtonPos = (this.Width - buttonsTotalWidth) / 2;
			for(int i = 0; i < buttons.Length; ++i) {
				buttons[i].Location = new Point(nextButtonPos, buttonsTop + MemoHeight);
				nextButtonPos += buttons[i].Width + Spacing;
			}
			if(icon == null) {
				this.messageRectangle.Offset((wantedFormWidth - (messageRectangleRight + Spacing)) / 2, 0);
			}
			if(icon != null && this.messageRectangle.Height < this.iconRectangle.Height) {
				this.messageRectangle.Offset(0, (this.iconRectangle.Height - this.messageRectangle.Height) / 2);
			}
		}
		public string MessageText {
			get {
				return messageText;
			}
		}
		AppearanceObject GetPaintAppearance() {
			AppearanceObject paintAppearance = new AppearanceObject(Appearance, DefaultAppearance);
			paintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			paintAppearance.TextOptions.VAlignment = VertAlignment.Top;
			paintAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			return paintAppearance;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache gcache = new GraphicsCache(e)) {
				gcache.Graphics.DrawIcon(icon, this.iconRectangle);
				GetPaintAppearance().DrawString(gcache, labelText, this.messageRectangle);
			}
		}
		#region Obsolete 15.1
		[Obsolete("Use the MemoHeight member instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static int MemoHeigth {
			get { return MemoHeight; }
			set { MemoHeight = value; }
		}
		#endregion
	}
}
