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
using DevExpress.XtraWaitForm;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.DataAccess.UI.Native {
	public partial class WaitFormWithCancel : WaitForm {
		ISupportCancel canceler;
		public WaitFormWithCancel() {
			InitializeComponent();
			LocalizeComponent();
		}
		void cancelButton_Click(object sender, EventArgs e) {
			if(canceler != null) {
				canceler.Cancel();
				this.Close();
			}
		}
		public override void SetCaption(string caption) {
			base.SetCaption(caption);
			progressPanel1.Caption = caption;
			AdjustSize();
		}
		public override void SetDescription(string description) {
			base.SetDescription(description);
			progressPanel1.Description = description;
			AdjustSize();
		}
		public override void ProcessCommand(Enum cmd, object arg) {
			base.ProcessCommand(cmd, arg);
			WaitFormCommand command = (WaitFormCommand)cmd;
			switch(command) {
				case WaitFormCommand.SetWaitFormObject:
					canceler = (ISupportCancel)arg;
					cancelButton.Enabled = canceler != null;
					break;
				case WaitFormCommand.EnableCancelButton:
					cancelButton.Enabled = (bool)arg;
					break;
				case WaitFormCommand.EnableDescription:
					progressPanel1.ShowDescription = (bool)arg;
					break;
				case WaitFormCommand.ShowCancelButton:
					cancelButton.Visible = (bool)arg;
					break;
				case WaitFormCommand.SetPosition:
					Rectangle parentBounds = (Rectangle)arg;
					Location = new Point(parentBounds.Left + (parentBounds.Width - Width)/2,
						parentBounds.Top + (parentBounds.Height - Height)/2);
					break;
			}
		}
		void LocalizeComponent() {
			string loading = DataAccessUILocalizer.GetString(DataAccessUIStringId.WaitFormWithCancel_Loading);
			progressPanel1.Caption = loading;
			progressPanel1.Description = loading;
			cancelButton.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
		}
		void AdjustSize() {
			int minWidth = progressPanel1.Width + progressPanel1.Padding.Horizontal + cancelButton.Width +
						   cancelButton.Padding.Horizontal;
			if(ClientSize.Width < minWidth) {
				Location = new Point(Location.X - (minWidth - ClientSize.Width)/2, Location.Y);
				ClientSize = new Size(minWidth, ClientSize.Height);
			}
		}
	}
	public enum WaitFormCommand {
		SetWaitFormObject,
		EnableDescription,
		ShowCancelButton,
		SetPosition,
		EnableCancelButton
	}
}
