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
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface IShowDialog {
		DialogResult ShowDialog();
	}
	public interface IImportQuestion : IShowDialog {
		void SetValues(string defaultLanguageValue, string translatedValue);
		bool IsYesForAll { get; }
	}
	[ToolboxItem(false)]
	public class ImportQuestionForm : XtraForm, IImportQuestion {
		public const string Title = "Import";
		public const string Question = "Do you want to import property?";
		private ToolTip toolTip;
		private GroupBox groupBox;
		private SimpleButton yesButton;
		private SimpleButton noButton;
		private SimpleButton cancelButton;
		private SimpleButton yesForAllButton;
		private Label defaultLanguageValueCaptionLabel;
		private Label translatedValueCaptionLabel;
		private Label defaultLanguageValueLabel;
		private Label translatedValueLabel;
		private Label questionLabel;
		private bool isYesForAll;
		public ImportQuestionForm() {
			Text = Title;
			Width = 400;
			Height = 190;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			InitializeControls();
			this.CenterToParent();
			Image LocalizationImage = DevExpress.ExpressApp.Utils.ImageLoader.Instance.GetImageInfo("BO_Localization").Image;
			if(LocalizationImage != null) {
				Icon = Icon.FromHandle(new Bitmap(LocalizationImage).GetHicon());
			}
		}
		public bool IsYesForAll {
			get { return isYesForAll; }
		}
		private void InitializeControls() {
			toolTip = new ToolTip();
			toolTip.AutoPopDelay = 0;
			toolTip.ReshowDelay = 0;
			toolTip.InitialDelay = 500;
			groupBox = new GroupBox();
			groupBox.Left = 10;
			groupBox.Width = ClientSize.Width - 20;
			groupBox.Height = 110;
			groupBox.Text = "Apply property:";
			questionLabel = new System.Windows.Forms.Label();
			questionLabel.Width = 250;
			questionLabel.Text = Question;
			questionLabel.Top = 100;
			questionLabel.Left = (this.ClientSize.Width - questionLabel.Width) / 2;
			questionLabel.Visible = false;
			int buttonTop = 120;
			defaultLanguageValueCaptionLabel = new System.Windows.Forms.Label();
			defaultLanguageValueCaptionLabel.Text = "Default Language Value: ";
			defaultLanguageValueCaptionLabel.Font = new System.Drawing.Font(defaultLanguageValueCaptionLabel.Font, FontStyle.Bold);
			defaultLanguageValueCaptionLabel.Top = 20;
			defaultLanguageValueCaptionLabel.Left = 10;
			defaultLanguageValueCaptionLabel.Width = 200;
			translatedValueCaptionLabel = new System.Windows.Forms.Label();
			translatedValueCaptionLabel.Text = "Translated Value: ";
			translatedValueCaptionLabel.Font = new System.Drawing.Font(defaultLanguageValueCaptionLabel.Font, FontStyle.Bold);
			translatedValueCaptionLabel.Top = 60;
			translatedValueCaptionLabel.Left = 10;
			translatedValueCaptionLabel.Width = 200;
			defaultLanguageValueLabel = new System.Windows.Forms.Label();
			defaultLanguageValueLabel.Top = 40;
			defaultLanguageValueLabel.Left = 30;
			defaultLanguageValueLabel.Height = 15;
			defaultLanguageValueLabel.AutoEllipsis = true;
			defaultLanguageValueLabel.Width = groupBox.ClientSize.Width - 40;
			translatedValueLabel = new System.Windows.Forms.Label();
			translatedValueLabel.Top = 80;
			translatedValueLabel.Left = 30;
			translatedValueLabel.Height = 15;
			translatedValueLabel.AutoEllipsis = true;
			translatedValueLabel.Width = groupBox.ClientSize.Width - 40;
			yesButton = new SimpleButton();
			yesButton.Top = buttonTop;
			yesButton.Text = "&Yes";
			yesButton.Click += new EventHandler(yesButton_Click);
			yesForAllButton = new SimpleButton();
			yesForAllButton.Top = buttonTop;
			yesForAllButton.Text = "Yes for &All";
			yesForAllButton.Click += new EventHandler(yesForAllButton_Click);
			noButton = new SimpleButton();
			noButton.Top = buttonTop;
			noButton.Text = "&No";
			noButton.Click += new EventHandler(noButton_Click);
			cancelButton = new SimpleButton();
			cancelButton.Top = buttonTop;
			cancelButton.Text = "&Cancel";
			int buttonSpaceSize = (this.ClientSize.Width - yesButton.Width - noButton.Width - cancelButton.Width - yesForAllButton.Width) / 5;
			yesButton.Left = buttonSpaceSize;
			yesForAllButton.Left = 2 * buttonSpaceSize + yesButton.Width;
			noButton.Left = 3 * buttonSpaceSize + yesButton.Width + yesForAllButton.Width;
			cancelButton.Left = 4 * buttonSpaceSize + yesButton.Width + yesForAllButton.Width + noButton.Width;
			SetClientSizeCore(ClientSize.Width, yesButton.Bottom + 10);
			this.Controls.Add(questionLabel);
			this.Controls.Add(groupBox);
			this.Controls.Add(yesButton);
			this.Controls.Add(yesForAllButton);
			this.Controls.Add(noButton);
			this.Controls.Add(cancelButton);
			groupBox.Controls.Add(defaultLanguageValueCaptionLabel);
			groupBox.Controls.Add(translatedValueCaptionLabel);
			groupBox.Controls.Add(defaultLanguageValueLabel);
			groupBox.Controls.Add(translatedValueLabel);
			this.AcceptButton = yesButton;
			this.CancelButton = cancelButton;
		}
		private void yesForAllButton_Click(object sender, EventArgs e) {
			isYesForAll = true;
			DialogResult = DialogResult.Yes;
		}
		private void noButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.No;
		}
		private void yesButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Yes;
		}
		#region IImportQuestuion Members
		public void SetValues(string defaultLanguageValue, string translatedValue) {
			defaultLanguageValueLabel.Text = defaultLanguageValue;
			translatedValueLabel.Text = translatedValue;
			toolTip.SetToolTip(defaultLanguageValueLabel, defaultLanguageValue);
			toolTip.SetToolTip(translatedValueLabel, translatedValue);
			this.CenterToParent();
		}
		DialogResult IShowDialog.ShowDialog() {
			if(isYesForAll) {
				return DialogResult.Yes;
			}
			return ShowDialog();
		}
		#endregion
	}
}
