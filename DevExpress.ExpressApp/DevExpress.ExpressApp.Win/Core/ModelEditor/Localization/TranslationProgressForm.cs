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
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWaitForm;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class TranslationProgressForm : WaitForm {
		private DevExpress.XtraEditors.LabelControl label;
		private DevExpress.XtraEditors.ProgressBarControl progressBar;
		private SimpleButton cancelButton;
		private bool manualClose = false;
		public TranslationProgressForm() {
			Width = 400;
			Height = 140;
			FormBorderStyle = FormBorderStyle.None;
			MaximizeBox = false;
			MinimizeBox = false;
			ShowInTaskbar = false; 
			InitializeControls();
			this.CenterToScreen();
		}
		private void InitializeControls() {
			label = new DevExpress.XtraEditors.LabelControl();
			label.Top = 10;
			label.Text = "Translating the selected records...";
			label.Size = label.CalcBestSize();
			label.Left = (ClientSize.Width - label.Width) / 2;
			progressBar = new DevExpress.XtraEditors.ProgressBarControl();
			progressBar.Top = 30;
			progressBar.Left = 10;
			progressBar.Width = ClientSize.Width - 2 * progressBar.Left;
			progressBar.Height = 30;
			progressBar.Properties.Minimum = 0;
			progressBar.Properties.Maximum = 100;
			cancelButton = new SimpleButton();
			cancelButton.Top = 70;
			cancelButton.Left = (ClientSize.Width - cancelButton.Width) / 2;
			cancelButton.Text = "Cancel";
			this.Controls.Add(label);
			this.Controls.Add(progressBar);
			this.Controls.Add(cancelButton);
			this.CancelButton = cancelButton;
		}
		public override void SetCaption(string caption) {
			base.SetCaption(caption);
			label.Text = caption;
			label.Size = label.CalcBestSize();
			label.Left = (ClientSize.Width - label.Width) / 2;
		}
		public override void ProcessCommand(Enum cmd, object arg) {
			base.ProcessCommand(cmd, arg);
			if(cmd is WaitFormCommand) {
				WaitFormCommand cmdCore = (WaitFormCommand)cmd;
				switch(cmdCore) {
					case WaitFormCommand.SetProgress: {
						decimal percent = (decimal)arg;
						int newPosition = (int)(percent * 100);
						if(newPosition != progressBar.Position) {
							progressBar.Position = newPosition;
							progressBar.Refresh();
						}
						break;
					}
					case WaitFormCommand.SetCaption: {
						SetCaption((string)arg);
						break;
					}
					case WaitFormCommand.ManualClose: {
						manualClose = true;
						break;
					}
				}
			}
		}
		public void SetProgress(decimal percent) {
			if (IsHandleCreated) {
				this.Invoke(new ThreadStart(delegate() {
					int newPosition = (int)(percent * 100);
					if(newPosition != progressBar.Position) {
						progressBar.Position = newPosition;
						progressBar.Refresh();
					}
				}));
			}
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if(!manualClose) {
				SplashScreenManager.CloseForm(true, 0, null, false, true);
			}
		}
		public enum WaitFormCommand {
			SetProgress,
			SetCaption,
			ManualClose
		}
	}
}
