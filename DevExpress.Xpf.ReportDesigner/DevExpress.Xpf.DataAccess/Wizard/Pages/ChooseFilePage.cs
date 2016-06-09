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
using System.IO;
using System.Linq;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.DataAccess.Native;
using FileInfo = DevExpress.DataAccess.Wizard.Presenters.FileInfo;
using DevExpress.DataAccess.UI.Localization;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class ChooseFilePage : DataSourceWizardPage, IChooseFilePageView {
		public static ChooseFilePage Create(DataSourceWizardModelBase model) {
			return ViewModelSource.Create(() => new ChooseFilePage(model));
		}
		protected ChooseFilePage(DataSourceWizardModelBase model) : base(model) { }
		[RaiseChanged]
		public virtual string FileName { get; set; }
		public bool ShowPasswordForm(string caption, string fileName, out FileInfo fileInfo) {
			var passwordRequestViewModel = new PasswordRequestViewModel() {
				FileName = fileName,
				JustificationText = caption
			};
			bool dialogResult = false;
			model.Parameters.DoWithPasswordDialogService(dialog => {
				var commands = UICommand.GenerateFromMessageButton(MessageButton.OK, new DefaultMessageButtonLocalizer());
				var okCommand = commands.First();
				okCommand.Command = new DelegateCommand<CancelEventArgs>(e => {
					e.Cancel = passwordRequestViewModel.IsError;
					passwordRequestViewModel.IsError = false;
				});
				dialogResult = dialog.ShowDialog(commands, DataAccessUILocalizer.GetString(DataAccessUIStringId.PasswordRequest), passwordRequestViewModel) == okCommand;
			});
			fileInfo = new FileInfo() {
				FileName = passwordRequestViewModel.FileName,
				Password = passwordRequestViewModel.Password,
				SavePassword = passwordRequestViewModel.SavePassword
			};
			return dialogResult;
		}
		public void SelectFile() {
			model.Parameters.DoWithOpenFileDialogService(dialog => {
				dialog.Filter = FileNameFilterStrings.ExcelCsv;
				if(dialog.ShowDialog())
					FileName = Path.Combine(dialog.File.DirectoryName, dialog.File.Name);
			});
		}
	}
}
