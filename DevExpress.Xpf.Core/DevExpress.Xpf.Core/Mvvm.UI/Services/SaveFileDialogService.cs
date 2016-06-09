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

#if !SILVERLIGHT
using System.Windows.Forms;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Mvvm.UI {
	[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
	[TargetType(typeof(System.Windows.Controls.UserControl)), TargetType(typeof(Window))]
	public class SaveFileDialogService : FileDialogServiceBase, ISaveFileDialogService {
		protected interface ISaveFileDialog : IFileDialog {
			bool CreatePrompt { get; set; }
			bool OverwritePrompt { get; set; }
#if SILVERLIGHT
			string DefaultFileName { get; set; }
			Stream OpenFile();
			string SafeFileName();
#endif
		}
		protected class SaveFileDialogAdapter : FileDialogAdapter<SaveFileDialog>, ISaveFileDialog {
			public SaveFileDialogAdapter(SaveFileDialog fileDialog) : base(fileDialog) { }
			bool ISaveFileDialog.CreatePrompt {
				get { return fileDialog.CreatePrompt; }
				set { fileDialog.CreatePrompt = value; }
			}
			bool ISaveFileDialog.OverwritePrompt {
				get { return fileDialog.OverwritePrompt; }
				set { fileDialog.OverwritePrompt = value; }
			}
#if SILVERLIGHT
			bool ISaveFileDialog.DefaultFileName {
				get { return fileDialog.DefaultFileName; }
				set { fileDialog.DefaultFileName = value; }
			}
			Stream ISaveFileDialog.OpenFile() {
				return fileDialog.OpenFile();
			}
			string ISaveFileDialog.SafeFileName() {
				return fileDialog.SafeFileName;
			}
#endif
		}
#if !SILVERLIGHT
		public static readonly DependencyProperty CreatePromptProperty =
			DependencyProperty.Register("CreatePrompt", typeof(bool), typeof(SaveFileDialogService), new PropertyMetadata(false));
		public static readonly DependencyProperty OverwritePromptProperty =
			DependencyProperty.Register("OverwritePrompt", typeof(bool), typeof(SaveFileDialogService), new PropertyMetadata(true));
#endif
		public static readonly DependencyProperty DefaultExtProperty =
			DependencyProperty.Register("DefaultExt", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty DefaultFileNameProperty =
			DependencyProperty.Register("DefaultFileName", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty FilterIndexProperty =
			DependencyProperty.Register("FilterIndex", typeof(int), typeof(SaveFileDialogService), new PropertyMetadata(1));
#if !SILVERLIGHT
		public bool CreatePrompt {
			get { return (bool)GetValue(CreatePromptProperty); }
			set { SetValue(CreatePromptProperty, value); }
		}
		public bool OverwritePrompt {
			get { return (bool)GetValue(OverwritePromptProperty); }
			set { SetValue(OverwritePromptProperty, value); }
		}
#endif
		public string DefaultExt {
			get { return (string)GetValue(DefaultExtProperty); }
			set { SetValue(DefaultExtProperty, value); }
		}
		public string DefaultFileName {
			get { return (string)GetValue(DefaultFileNameProperty); }
			set { SetValue(DefaultFileNameProperty, value); }
		}
		public string Filter {
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}
		public int FilterIndex {
			get { return (int)GetValue(FilterIndexProperty); }
			set { SetValue(FilterIndexProperty, value); }
		}
		ISaveFileDialog SaveFileDialog { get { return (ISaveFileDialog)GetFileDialog(); } }
		public SaveFileDialogService() {
#if !SILVERLIGHT
			CheckFileExists = false;
#endif
		}
		protected override object CreateFileDialog() {
			return new SaveFileDialog();
		}
		protected override IFileDialog CreateFileDialogAdapter() {
			return new SaveFileDialogAdapter((SaveFileDialog)CreateFileDialog());
		}
		protected override void InitFileDialog() {
#if !SILVERLIGHT
			SaveFileDialog.CreatePrompt = CreatePrompt;
			SaveFileDialog.OverwritePrompt = OverwritePrompt;
			SaveFileDialog.FileName = DefaultFileName;
#else
			SaveFileDialog.DefaultFileName = DefaultFileName;
#endif
			SaveFileDialog.DefaultExt = DefaultExt;
			SaveFileDialog.Filter = Filter;
			SaveFileDialog.FilterIndex = FilterIndex;
		}
		protected override List<FileInfoWrapper> GetFileInfos() {
#if !SILVERLIGHT
			List<FileInfoWrapper> res = new List<FileInfoWrapper>();
			foreach(string fileName in SaveFileDialog.FileNames)
				res.Add(FileInfoWrapper.Create(fileName));
			return res;
#else
			return new List<FileInfoWrapper>();
#endif
		}
#if !SILVERLIGHT
		IFileInfo ISaveFileDialogService.File { get { return GetFiles().FirstOrDefault(); } }
#endif
		bool ISaveFileDialogService.ShowDialog(Action<CancelEventArgs> fileOK, string directoryName, string fileName) {
#if !SILVERLIGHT
			if(directoryName != null)
				InitialDirectory = directoryName;
#endif
			if(fileName != null)
				DefaultFileName = fileName;
			var res = Show(fileOK);
			FilterIndex = SaveFileDialog.FilterIndex;
			return res;
		}
#if SILVERLIGHT
		Stream ISaveFileDialogService.OpenFile() {
			return SaveFileDialog.OpenFile();
		}
		string ISaveFileDialogService.SafeFileName() {
			return SaveFileDialog.SafeFileName;
		}
#endif
	}
}
