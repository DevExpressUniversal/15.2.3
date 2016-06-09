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
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface IReplaceQuestion : IShowDialog {
		bool ShowDialogAgain { get; }
		void SetValues(int itemsCount, string defaultLanguageValue);
	}
	public class ReplaceQuestionForm : XtraForm, IReplaceQuestion {
		public const string Title = "Multiple Values Translation";
		public const string Question = "There are {0} records with this default language value:\r\n<b>\"{1}\"</b>\r\nApply the current translation to all of them?";
		public const string UseAlwaysText = "Don't show this dialog again.";
		private PictureBox informationPictureBox;
		private LabelControl questionLabel;
		private SimpleButton yesButton;
		private SimpleButton noButton;
		private DevExpress.XtraEditors.CheckEdit showDialogAgainCheckButton;
		public ReplaceQuestionForm() {
			Text = Title;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			Image LocalizationImage = ImageLoader.Instance.GetImageInfo("BO_Localization").Image;
			if (LocalizationImage != null) {
				Icon = Icon.FromHandle(new Bitmap(LocalizationImage).GetHicon());
			}
		}
		private void InitializeControls(int itemsCount, string defaultLanguageValue) {
			informationPictureBox = new PictureBox();
			informationPictureBox.Top = 10;
			informationPictureBox.Left = 10;
			informationPictureBox.Width = 32;
			informationPictureBox.Height = 32;
			informationPictureBox.Image = ImageLoader.Instance.GetImageInfo("Action_AboutInfo_32x32").Image;
			if (informationPictureBox.Image != null) {
				informationPictureBox.Width = informationPictureBox.Image.Width;
				informationPictureBox.Height = informationPictureBox.Image.Height;
			}
			informationPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
			questionLabel = new LabelControl();
			questionLabel.AllowHtmlString = true;
			questionLabel.Top = 10;
			questionLabel.Height = informationPictureBox.Height;
			questionLabel.Left = informationPictureBox.Right + 10;
			string defaultLanguageValueDisplayText = (defaultLanguageValue != null && defaultLanguageValue.Length > 40) ? defaultLanguageValue.Substring(0, 40) + "..." : defaultLanguageValue;
			questionLabel.Text = string.Format(Question, itemsCount, defaultLanguageValueDisplayText);
			questionLabel.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			questionLabel.Size = questionLabel.CalcBestSize();
			informationPictureBox.Height = questionLabel.Height;
			showDialogAgainCheckButton = new CheckEdit();
			showDialogAgainCheckButton.Text = UseAlwaysText;
			showDialogAgainCheckButton.Left = 10;
			showDialogAgainCheckButton.Top = questionLabel.Bottom + 10;
			showDialogAgainCheckButton.Checked = false;
			showDialogAgainCheckButton.Size = showDialogAgainCheckButton.CalcBestSize();
			int buttonTop = showDialogAgainCheckButton.Bottom + 10;
			yesButton = new SimpleButton();
			yesButton.Top = buttonTop;
			yesButton.Text = "&Yes";
			yesButton.Click += new EventHandler(yesButton_Click);
			noButton = new SimpleButton();
			noButton.Top = buttonTop;
			noButton.Text = "&No";
			SetClientSizeCore(questionLabel.Right + 10, yesButton.Bottom + 10); 
			yesButton.Left = (ClientSize.Width - 10 - yesButton.Width - noButton.Width) / 2;
			noButton.Left = yesButton.Right + 10;
			this.Controls.Add(informationPictureBox);
			this.Controls.Add(questionLabel);
			this.Controls.Add(yesButton);
			this.Controls.Add(noButton);
			this.Controls.Add(showDialogAgainCheckButton);
			this.AcceptButton = yesButton;
			this.CancelButton = noButton;
		}
		private void yesButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Yes;
		}
		#region IReplaceQuestion Members
		public bool ShowDialogAgain {
			get { return showDialogAgainCheckButton.Checked; }
		}
		public void SetValues(int itemsCount, string defaultLanguageValue) {
			InitializeControls(itemsCount, defaultLanguageValue);
			this.CenterToParent();
		}
		#endregion
	}
}
