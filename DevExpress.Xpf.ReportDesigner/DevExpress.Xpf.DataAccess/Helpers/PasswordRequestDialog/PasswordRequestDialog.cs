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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.DataAccess.Native;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.DataAccess.Native {
	public class PasswordRequestDialog : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty OpenFileDialogServiceTemplateProperty;
		static readonly Action<PasswordRequestDialog, Action<IOpenFileDialogService>> openFileDialogServiceAccessor;
		static PasswordRequestDialog() {
			DependencyPropertyRegistrator<PasswordRequestDialog>.New()
				.Register(d => d.EditValue, out EditValueProperty, null)
				.RegisterServiceTemplateProperty(d => d.OpenFileDialogServiceTemplate, out OpenFileDialogServiceTemplateProperty, out openFileDialogServiceAccessor)
				.OverrideDefaultStyleKey()
			;
		}
		public PasswordRequestDialog() {
			selectFileCommand = DelegateCommandFactory.Create(SelectFile);
		}
		readonly ICommand selectFileCommand;
		public ICommand SelectFileCommand { get { return selectFileCommand; } }
		public PasswordRequestViewModel EditValue {
			get { return (PasswordRequestViewModel)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public void DoWithOpenFileDialogService(Action<IOpenFileDialogService> action) { openFileDialogServiceAccessor(this, action); }
		public DataTemplate OpenFileDialogServiceTemplate {
			get { return (DataTemplate)GetValue(OpenFileDialogServiceTemplateProperty); }
			set { SetValue(OpenFileDialogServiceTemplateProperty, value); }
		}
		public void SelectFile() {
			DoWithOpenFileDialogService(dialog => {
				dialog.Filter = FileNameFilterStrings.ExcelCsv;
				if(dialog.ShowDialog())
					EditValue.FileName = Path.Combine(dialog.File.DirectoryName, dialog.File.Name);
			});
		}
	}
}
