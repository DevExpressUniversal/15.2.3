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
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Browser;
#else
using Microsoft.Win32;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Printing {
	public class DialogService : IDialogService {
		FrameworkElement view;
		public DialogService(FrameworkElement view) {
			if(view == null)
				throw new ArgumentNullException("view");
			this.view = view;
		}
		public void ShowError(string text, string caption) {
#if DEBUGTEST
			if(!System.Diagnostics.Debugger.IsAttached)
				throw new Exception(text);
#endif
#if SILVERLIGHT
			MessageBox.Show(text, caption, MessageBoxButton.OK);
#else
			DXMessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
#endif
		}
		public void ShowInformation(string text, string caption) {
#if SILVERLIGHT
			MessageBox.Show(text, caption, MessageBoxButton.OK);
#else
			DXMessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
#endif
		}
		public Stream ShowSaveFileDialog(string caption, string filter, int filterIndex, string initialDirectory, string fileName) {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = filter;
			dialog.FilterIndex = filterIndex;
#if SL
			try {
				dialog.DefaultFileName = fileName;
			} catch(ArgumentException) {
			}
#else
			dialog.FileName = fileName;
			dialog.Title = caption;
			dialog.InitialDirectory = initialDirectory;
#endif
			if(dialog.ShowDialog() != true) {
				return null;
			}
			return dialog.OpenFile();
		}
#if SILVERLIGHT
		public void ShowHtmlPage(Uri uri) {
			HtmlPage.Window.Navigate(uri, "_blank", "toolbar=0,menubar=0,resizable=1,scrollbars=1");
		}
#else
		public Window GetParentWindow() {
			return LayoutHelper.FindParentObject<Window>(view);
		}
#endif
		public Stream ShowOpenFileDialog(string caption, string filter) {
			string fileName = "";
			return ShowOpenFileDialog(caption, filter, out fileName);
		}
		public Stream ShowOpenFileDialog(string caption, string filter, out string fileName) {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = filter;
#if !SL
			dialog.Title = caption;
#endif
			if(dialog.ShowDialog() != true) {
				fileName = null;
				return null;
			}
#if SL
			fileName = dialog.File.Name;
			return dialog.File.OpenRead();
#else
			fileName = dialog.FileName;
			return dialog.OpenFile();
#endif
		}
	}
}
