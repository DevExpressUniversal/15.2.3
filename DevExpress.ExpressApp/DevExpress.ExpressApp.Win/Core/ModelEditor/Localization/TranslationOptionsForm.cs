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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public interface ITranslationOptions : IShowDialog {
		string SourceLanguageCode { get; }
	}
	public class TranslationOptionsForm : XtraForm, ITranslationOptions {
		private const string automaticLanguageCaption = "(Auto)";
		private const string translateToCaption = "Translate the selected record(s) to '{0}' language?";
		private const string title = "Translate";
		private LabelControl descriptionLabel;
		private LabelControl languageFromLabel;
		private LabelControl languageToLabel;
		private ComboBoxEdit sourceLanguageComboBox;
		private SimpleButton translateButton;
		private SimpleButton cancelButton;
		private string destinationLanguageCode;
		private ITranslatorProvider provider;
		public TranslationOptionsForm(string sourceLanguageCode, string destinationLanguageCode, ITranslatorProvider provider) {
			this.destinationLanguageCode = destinationLanguageCode;
			this.provider = provider;
			Text = title;
			Width = 270;
			FormBorderStyle = FormBorderStyle.FixedSingle;
			MaximizeBox = false;
			MinimizeBox = false;
			InitializeControls();
			FillSourceLanguages(string.IsNullOrEmpty(sourceLanguageCode) ? automaticLanguageCaption : sourceLanguageCode);
			SetDestinationLanguage(destinationLanguageCode);
			this.CenterToParent();
			Image LocalizationImage = ImageLoader.Instance.GetImageInfo("BO_Localization").Image;
			if (LocalizationImage != null) {
				Icon = Icon.FromHandle(new Bitmap(LocalizationImage).GetHicon());
			}
		}
		public SimpleButton TranslateButton {
			get { return translateButton; }
		}
		public ComboBoxEdit SourceLanguageComboBox {
			get { return sourceLanguageComboBox; }
		}
		public string SourceLanguageCode {
			get {
				string result = (string)sourceLanguageComboBox.SelectedItem;
				return result == automaticLanguageCaption ? string.Empty : result;
			}
		}
		private void InitializeControls() {
			int borderSize = 10;
			languageToLabel = new LabelControl();
			languageToLabel.Top = borderSize;
			languageToLabel.Left = borderSize;
			SetDestinationLanguage(destinationLanguageCode);
			languageToLabel.Size = languageToLabel.CalcBestSize();
			languageFromLabel = new LabelControl();
			languageFromLabel.Top = languageToLabel.Bottom + borderSize + 2;
			languageFromLabel.Left = borderSize;
			languageFromLabel.Text = "Source language:";
			languageFromLabel.Size = languageFromLabel.CalcBestSize();
			sourceLanguageComboBox = new DevExpress.XtraEditors.ComboBoxEdit();
			sourceLanguageComboBox.Top = languageToLabel.Bottom + borderSize;
			sourceLanguageComboBox.Left = languageFromLabel.Right + borderSize * 2;
			sourceLanguageComboBox.Width = ClientSize.Width - sourceLanguageComboBox.Left - borderSize;
			sourceLanguageComboBox.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			descriptionLabel = new LabelControl();
			descriptionLabel.AllowHtmlString = true;
			descriptionLabel.Top = sourceLanguageComboBox.Bottom + borderSize;
			descriptionLabel.Left = borderSize;
			descriptionLabel.Text = provider.Description;
			descriptionLabel.Size = descriptionLabel.CalcBestSize();
			cancelButton = new SimpleButton();
			cancelButton.Top = descriptionLabel.Bottom + borderSize;
			cancelButton.Text = "Cancel";
			cancelButton.Left = ClientSize.Width - borderSize - cancelButton.Width;
			translateButton = new SimpleButton();
			translateButton.Top = descriptionLabel.Bottom + borderSize;
			translateButton.Left = ClientSize.Width - borderSize * 2 - translateButton.Width - cancelButton.Width;
			translateButton.Text = "Translate";
			translateButton.Click += new EventHandler(translateButton_Click);
			this.SetClientSizeCore(ClientSize.Width, translateButton.Bottom + borderSize);
			this.Controls.Add(languageFromLabel);
			this.Controls.Add(languageToLabel);
			this.Controls.Add(sourceLanguageComboBox);
			this.Controls.Add(translateButton);
			this.Controls.Add(cancelButton);
			this.Controls.Add(descriptionLabel);
			this.AcceptButton = translateButton;
			this.CancelButton = cancelButton;
		}
		private void translateButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.OK;
		}
		private List<string> SourceLanguages {
			get {
				List<string> result = new List<string>();
				try {
					result.AddRange(provider.GetLanguages());
				}
				catch (Exception e) {
					Messaging.GetMessaging(null).Show(e.Message, "Translator error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				result.Sort();
				result.Insert(0, automaticLanguageCaption);
				return result;
			}
		}
		private void FillSourceLanguages(string sourceLanguageCode) {
			sourceLanguageComboBox.Properties.Items.Clear();
			sourceLanguageComboBox.Properties.Items.AddRange(SourceLanguages);
			sourceLanguageComboBox.SelectedIndex = 0;
			foreach (string item in sourceLanguageComboBox.Properties.Items) {
				if (item == sourceLanguageCode) {
					sourceLanguageComboBox.SelectedItem = item;
				}
			}
		}
		private void SetDestinationLanguage(string destinationLanguageCode) {
			languageToLabel.Text = string.Format(translateToCaption, destinationLanguageCode);
		}
	}
}
