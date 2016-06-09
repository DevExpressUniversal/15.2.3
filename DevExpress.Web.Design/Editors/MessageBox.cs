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
namespace DevExpress.Web.Design.Forms {
	using DevExpress.Web.Internal;
	public enum MessageBoxButtonsEx {
		OK, OKCancel, YesNoCancel, YesNo, YesNoYesToAllNoToAll,
		YesNoYesToAllNoToAllCancel, YesYesToAllNoNoDontAskMe
	}
	public enum DialogResultEx { None, OK, Cancel, Yes, No, YesToAll, NoToAll, NoDontAskMe }
	public partial class MessageBoxEx {
		private const MessageBoxButtonsEx DefaultButtons = MessageBoxButtonsEx.OK;
		public MessageBoxEx() {
		}
		public static DialogResultEx Show(IWin32Window owner, string text, string caption, MessageBoxButtonsEx buttons) {
			bool dummy = false;
			return Show(owner, text, caption, MessageBoxButtonsToDialogResults(buttons), false, false, ref dummy);
		}
		public static DialogResultEx Show(IWin32Window owner, string text, string caption, MessageBoxButtonsEx buttons,
			ref bool dontAskMeAgain) {
			return Show(owner, text, caption, MessageBoxButtonsToDialogResults(buttons), true, false, ref dontAskMeAgain);
		}
		public static DialogResultEx Show(IWin32Window owner, string text, string caption, MessageBoxButtonsEx buttons,
			bool buttonAutoSize, ref bool dontAskMeAgain) {
			return Show(owner, text, caption, MessageBoxButtonsToDialogResults(buttons), true, buttonAutoSize, ref dontAskMeAgain);
		}
		static DialogResultEx[] MessageBoxButtonsToDialogResults(MessageBoxButtonsEx buttons) {
			if (!Enum.IsDefined(typeof(MessageBoxButtonsEx), buttons))
				throw new InvalidEnumArgumentException("buttons", (int)buttons, typeof(DialogResultEx));
			switch (buttons) {
				case MessageBoxButtonsEx.OK:
					return new DialogResultEx[] { DialogResultEx.OK };
				case MessageBoxButtonsEx.OKCancel:
					return new DialogResultEx[] { DialogResultEx.OK, DialogResultEx.Cancel };
				case MessageBoxButtonsEx.YesNo:
					return new DialogResultEx[] { DialogResultEx.Yes, DialogResultEx.No };
				case MessageBoxButtonsEx.YesNoCancel:
					return new DialogResultEx[] { DialogResultEx.Yes, DialogResultEx.No, DialogResultEx.Cancel };
				case MessageBoxButtonsEx.YesNoYesToAllNoToAll:
					return new DialogResultEx[] { DialogResultEx.Yes, DialogResultEx.YesToAll, DialogResultEx.No, 
						DialogResultEx.NoToAll };
				case MessageBoxButtonsEx.YesNoYesToAllNoToAllCancel:
					return new DialogResultEx[] { DialogResultEx.Yes, DialogResultEx.YesToAll, DialogResultEx.No, 
						DialogResultEx.NoToAll, DialogResultEx.Cancel };
				case MessageBoxButtonsEx.YesYesToAllNoNoDontAskMe:
					return new DialogResultEx[] { DialogResultEx.Yes, DialogResultEx.YesToAll, DialogResultEx.No, 
						DialogResultEx.NoDontAskMe };
				default:
					throw new ArgumentException("buttons");
			}
		}
		protected static DialogResultEx Show(IWin32Window owner, string text, string caption,
			DialogResultEx[] commands, bool showDontAskMeAgain, bool buttonAutoSize, ref bool dontAskMeAgain) {
			using (MessageBoxForm form = new MessageBoxForm(owner, text, caption, commands, showDontAskMeAgain, buttonAutoSize, dontAskMeAgain)) {
				DialogResultEx result = form.ShowMessageBox(out dontAskMeAgain);
				return result;
			}
		}
	}
	public class MessageBoxForm : Form {
		private const int Spacing = 12;
		private const int ButtonSpacing = 6;
		private const int ButtonTopSpacing = 18;
		IWin32Window fOwner;
		private string fText;
		private string fCaption;
		private DialogResultEx[] fCommands;
		private bool fShowDontAskMeAgain;
		private bool fButtonAutoSize;
		private bool fDontAskMeAgain;
		private DialogResultEx fDialogResult = DialogResultEx.Cancel;
		private Label fLblMessage;
		private Button[] fButtons;
		private Rectangle fMessageRectangle;
		private CheckBox fCbDontAskMeAgain;
		public MessageBoxForm(IWin32Window owner, string text, string caption, DialogResultEx[] commands,
			bool showDontAskMeAgain, bool buttonAutoSize, bool dontAskMeAgain) {
			fOwner = owner;
			fText = text;
			fCaption = caption;
			fCommands = commands;
			fShowDontAskMeAgain = IsContaintDontAskMeAgain(commands) ? false : showDontAskMeAgain;
			fDontAskMeAgain = dontAskMeAgain;
			fButtonAutoSize = buttonAutoSize;
		}
		protected virtual DialogResult ConvertDialogResult(DialogResultEx result) {
			switch (result) {
				case DialogResultEx.Cancel:
					return DialogResult.Cancel;
				case DialogResultEx.No:
					return DialogResult.No;
				case DialogResultEx.OK:
					return DialogResult.OK;
				case DialogResultEx.Yes:
					return DialogResult.Yes;
				case DialogResultEx.YesToAll:
				case DialogResultEx.NoToAll:
					return DialogResult.OK;
				case DialogResultEx.NoDontAskMe:
					return DialogResult.No;
				default:
					return DialogResult.None;
			}
		}
		protected virtual string GetButtonText(DialogResultEx target) {
			switch (target) {
				case DialogResultEx.Cancel:
					return StringResources.MessageBox_DialogResultCancel;
				case DialogResultEx.No:
					return StringResources.MessageBox_DialogResultNo;
				case DialogResultEx.OK:
					return StringResources.MessageBox_DialogResultOK;
				case DialogResultEx.Yes:
					return StringResources.MessageBox_DialogResultYes;
				case DialogResultEx.YesToAll:
					return StringResources.MessageBox_DialogResultYesToAll;
				case DialogResultEx.NoToAll:
					return StringResources.MessageBox_DialogResultNoToAll;
				case DialogResultEx.NoDontAskMe:
					return StringResources.MessageBox_DialogResultNoDontAskMe;
				default:
					return '&' + target.ToString();
			}
		}
		protected internal IWin32Window GetFormOwner() {
			IWin32Window owner = fOwner;
			if (owner == null) {
				Form activeForm = Form.ActiveForm;
				if (activeForm != null && !activeForm.InvokeRequired)
					owner = activeForm;
			}
			if (owner != null) {
				Control ownerControl = owner as Control;
				if (ownerControl != null) {
					if (!ownerControl.Visible)
						owner = null;
					else {
						Form ownerForm = ownerControl.FindForm();
						if (ownerForm != null) {
							if ((!ownerForm.Visible)
								|| ownerForm.WindowState == FormWindowState.Minimized
								|| ownerForm.Right <= 0
								|| ownerForm.Bottom <= 0)
								owner = null;
						}
					}
				}
			}
			return owner;
		}
		protected internal DialogResultEx ShowMessageBox(out bool dontAskMeAgain) {
			FormBorderStyle = FormBorderStyle.FixedDialog;
			Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
				System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			KeyPreview = true;
			MinimizeBox = false;
			MaximizeBox = false;
			ShowInTaskbar = false;
			StartPosition = FormStartPosition.CenterParent;
			Text = fCaption;
			CreateButtons();
			CalcMessageBounds();
			CalcFinalSizes();
			DisableCloseButtonIfNeeded();
			IWin32Window owner = GetFormOwner();
			Form ownerForm = owner as Form;
			if (ownerForm != null && ownerForm.TopMost)
				TopMost = true; 
			ShowDialog(owner);
			if (fCbDontAskMeAgain != null)
				dontAskMeAgain = fCbDontAskMeAgain.Checked;
			else if (fDialogResult == DialogResultEx.NoDontAskMe)
				dontAskMeAgain = true;
			else
				dontAskMeAgain = false;
			return fDialogResult;
		}
		protected virtual void OnButtonClick(object sender, EventArgs e) {
			Button button = sender as Button;
			if (button != null) {
				DialogResultEx result = (DialogResultEx)button.Tag;
				fDialogResult = result;
			}
		}
		protected override void OnClosing(CancelEventArgs e) {
			if (CancelButton == null && DialogResult == DialogResult.Cancel)
				e.Cancel = true;
			base.OnClosing(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			if (e.KeyChar == '\x03') {
				Clipboard.SetDataObject(fCaption + Environment.NewLine +
					"---------------------------" +
					Environment.NewLine + fText, true);
				e.Handled = true;
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			DisableCloseButtonIfNeeded();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if (Visible && !ContainsFocus)
				Activate();
		}
		private void CalcMessageBounds() {
			int messageTop, messageLeft, messageWidth, messageHeight;
			messageTop = Spacing;
			messageLeft = Spacing;
			int maxFormWidth = MaximumSize.Width;
			if (maxFormWidth <= 0)
				maxFormWidth = SystemInformation.WorkingArea.Width;
			int maxTextWidth = maxFormWidth - Spacing - messageLeft;
			if (maxTextWidth < 10)
				maxTextWidth = 10;
			fLblMessage = new Label();
			Controls.Add(fLblMessage);
			fLblMessage.AutoSize = true;
			fLblMessage.MaximumSize = new Size(maxTextWidth / 3 * 2, fLblMessage.MaximumSize.Height);
			fLblMessage.Text = fText;
			messageWidth = fLblMessage.Size.Width;
			int maxFormHeight = MaximumSize.Height;
			if (maxFormHeight <= 0)
				maxFormHeight = SystemInformation.WorkingArea.Height;
			int maxTextHeight = maxFormHeight - Spacing - fButtons[0].Height - Spacing - messageTop -
				SystemInformation.CaptionHeight;
			if (maxTextHeight < 10)
				maxTextHeight = 10;
			messageHeight = fLblMessage.Size.Height;
			if (messageHeight > maxTextHeight)
				messageHeight = maxTextHeight;
			fMessageRectangle = new Rectangle(messageLeft, messageTop, messageWidth, messageHeight);
		}
		private void CreateButtons() {
			if (fCommands == null || fCommands.Length <= 0)
				throw new ArgumentException("At least one button must be specified", "buttons");
			fButtons = new Button[fCommands.Length];
			for (int i = 0; i < fCommands.Length; ++i) {
				Button currentButton = new Button();
				if (fButtonAutoSize)
					currentButton.AutoSize = true;
				DesignUtils.CheckLargeFontSize(currentButton);
				fButtons[i] = currentButton;
				currentButton.DialogResult = ConvertDialogResult(fCommands[i]);
				if (fCommands[i] == DialogResultEx.None)
					throw new ArgumentException("The 'DialogResult.None' button cannot be specified", "buttons");
				if (currentButton.DialogResult == DialogResult.Cancel)
					CancelButton = currentButton;
				currentButton.Tag = (object)fCommands[i];
				currentButton.Text = GetButtonText(fCommands[i]);
				currentButton.Click += new EventHandler(OnButtonClick);
				Controls.Add(currentButton);
			}
			if (fButtons.Length == 1)
				CancelButton = fButtons[0];
			if (fShowDontAskMeAgain) {
				fCbDontAskMeAgain = new CheckBox();
				Controls.Add(fCbDontAskMeAgain);
				fCbDontAskMeAgain.Text = StringResources.MessageBox_DontAskMeAgain;
				fCbDontAskMeAgain.AutoSize = true;
				fCbDontAskMeAgain.CheckAlign = ContentAlignment.BottomLeft;
				fCbDontAskMeAgain.TextAlign = ContentAlignment.MiddleLeft;
				fCbDontAskMeAgain.Checked = fDontAskMeAgain;
			}
		}
		private void CalcFinalSizes() {
			int buttonsTotalWidth = 0;
			foreach (Button button in fButtons) {
				if (buttonsTotalWidth != 0)
					buttonsTotalWidth += ButtonSpacing;
				buttonsTotalWidth += button.Width;
			}
			int buttonsTop = fMessageRectangle.Bottom + Spacing;
			int wantedFormWidth = MinimumSize.Width;
			if (wantedFormWidth == 0)
				wantedFormWidth = SystemInformation.MinimumWindowSize.Width;
			if (wantedFormWidth < fMessageRectangle.Right + Spacing)
				wantedFormWidth = fMessageRectangle.Right + Spacing;
			if (wantedFormWidth < buttonsTotalWidth + Spacing + Spacing)
				wantedFormWidth = buttonsTotalWidth + Spacing + Spacing;
			Label captionLabel = new Label();
			Controls.Add(captionLabel);
			captionLabel.AutoSize = true;
			captionLabel.Font = new Font(captionLabel.Font.FontFamily, captionLabel.Font.Size, FontStyle.Bold);
			captionLabel.Text = Text;
			int maxCaptionForcedWidth = SystemInformation.WorkingArea.Width / 3 * 2;
			int captionTextWidth = 1 + 4 + captionLabel.Size.Width;
			int captionTextWidthWithButtonsAndSpacing = captionTextWidth +
				SystemInformation.CaptionButtonSize.Width * 5 / 4;
			int captionWidth = Math.Min(maxCaptionForcedWidth, captionTextWidthWithButtonsAndSpacing);
			if (wantedFormWidth < captionWidth)
				wantedFormWidth = captionWidth;
			Controls.Remove(captionLabel);
			if (fCbDontAskMeAgain != null) {
				int checkBoxWidth = fCbDontAskMeAgain.Width + 2 * Spacing;
				if (wantedFormWidth < checkBoxWidth)
					wantedFormWidth = checkBoxWidth;
			}
			Width = wantedFormWidth + 2 * SystemInformation.FixedFrameBorderSize.Width;
			Height = buttonsTop + fButtons[0].Height + ButtonTopSpacing +
				2 * SystemInformation.FixedFrameBorderSize.Height + SystemInformation.CaptionHeight;
			if (fCbDontAskMeAgain != null)
				Height += fCbDontAskMeAgain.Height + Spacing;
			if (fCbDontAskMeAgain != null)
				fCbDontAskMeAgain.Location = new Point(Spacing, buttonsTop);
			int nextButtonPos = (wantedFormWidth - buttonsTotalWidth) / 2;
			if (fCbDontAskMeAgain != null)
				buttonsTop += fCbDontAskMeAgain.Height + Spacing;
			for (int i = 0; i < fButtons.Length; ++i) {
				fButtons[i].Location = new Point(nextButtonPos, buttonsTop - Spacing + ButtonTopSpacing);
				nextButtonPos += fButtons[i].Width + ButtonSpacing;
			}
			fMessageRectangle.Offset(-3, 0);
			fLblMessage.Location = new Point(fMessageRectangle.X, fMessageRectangle.Y);
		}
		private bool IsContaintDontAskMeAgain(DialogResultEx[] commands) {
			return new List<DialogResultEx>(commands).Contains(DialogResultEx.NoDontAskMe);
		}
		#region API
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern  uint EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
		const uint SC_CLOSE = 0xF060;
		const uint MF_GRAYED = 0x0001;
		void DisableCloseButtonIfNeeded() {
			if (CancelButton != null)
				return;
			IntPtr hMenu = GetSystemMenu(Handle, false);
			EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
		}
		#endregion
	}
}
