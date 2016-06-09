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

using DevExpress.Mvvm.UI.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
namespace DevExpress.Mvvm.UI {
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	[TargetType(typeof(UserControl)), TargetType(typeof(Window))]
	public class FolderBrowserDialogService : ServiceBase, IFolderBrowserDialogService {
		public static readonly DependencyProperty DescriptionProperty =
			DependencyProperty.Register("Description", typeof(string), typeof(FolderBrowserDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty RootFolderProperty =
			DependencyProperty.Register("RootFolder", typeof(Environment.SpecialFolder), typeof(FolderBrowserDialogService), new PropertyMetadata(Environment.SpecialFolder.Desktop));
		public static readonly DependencyProperty ShowNewFolderButtonProperty =
			DependencyProperty.Register("ShowNewFolderButton", typeof(bool), typeof(FolderBrowserDialogService), new PropertyMetadata(true));
		public static readonly DependencyProperty StartPathProperty =
			DependencyProperty.Register("StartPath", typeof(string), typeof(FolderBrowserDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty RestorePreviouslySelectedDirectoryProperty =
			DependencyProperty.Register("RestorePreviouslySelectedDirectory", typeof(bool), typeof(FolderBrowserDialogService), new PropertyMetadata(true));
		public static readonly DependencyProperty HelpRequestCommandProperty =
			DependencyProperty.Register("HelpRequestCommand", typeof(ICommand), typeof(FolderBrowserDialogService), new PropertyMetadata(null));
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public Environment.SpecialFolder RootFolder {
			get { return (Environment.SpecialFolder)GetValue(RootFolderProperty); }
			set { SetValue(RootFolderProperty, value); }
		}
		public bool ShowNewFolderButton {
			get { return (bool)GetValue(ShowNewFolderButtonProperty); }
			set { SetValue(ShowNewFolderButtonProperty, value); }
		}
		public string StartPath {
			get { return (string)GetValue(StartPathProperty); }
			set { SetValue(StartPathProperty, value); }
		}
		public bool RestorePreviouslySelectedDirectory {
			get { return (bool)GetValue(RestorePreviouslySelectedDirectoryProperty); }
			set { SetValue(RestorePreviouslySelectedDirectoryProperty, value); }
		}
		public ICommand HelpRequestCommand {
			get { return (ICommand)GetValue(HelpRequestCommandProperty); }
			set { SetValue(HelpRequestCommandProperty, value); }
		}
		public event EventHandler HelpRequest {
			add { Dialog.HelpRequest += value; }
			remove { Dialog.HelpRequest -= value; }
		}
		FolderBrowserDialog Dialog;
		public FolderBrowserDialogService() {
			Dialog = new FolderBrowserDialog();
			HelpRequest += (d, e) => {
				if(HelpRequestCommand != null && HelpRequestCommand.CanExecute(e))
					HelpRequestCommand.Execute(e);
			};
		}
		string resultPath = string.Empty;
		string IFolderBrowserDialogService.ResultPath {
			get { return resultPath; }
		}
		bool IFolderBrowserDialogService.ShowDialog() {
			Dialog.Description = Description;
			Dialog.RootFolder = RootFolder;
			Dialog.ShowNewFolderButton = ShowNewFolderButton;
			if(RestorePreviouslySelectedDirectory && !string.IsNullOrEmpty(resultPath))
				Dialog.SelectedPath = resultPath;
			else
				Dialog.SelectedPath = StartPath;
			var res = Dialog.ShowDialog();
			resultPath = Dialog.SelectedPath;
			if(res == DialogResult.OK)
				return true;
			if(res == DialogResult.Cancel)
				return false;
			throw new InvalidOperationException();
		}
	}
}
