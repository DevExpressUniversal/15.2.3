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
	public class OpenFileDialogService : FileDialogServiceBase, IOpenFileDialogService {
		protected interface IOpenFileDialog : IFileDialog {
			bool Multiselect { get; set; }
#if !SILVERLIGHT
			bool ReadOnlyChecked { get; set; }
			bool ShowReadOnly { get; set; }
#else
			string InitialDirectory { get; set; }
#endif
		}
		protected class OpenFileDialogAdapter : FileDialogAdapter<OpenFileDialog>, IOpenFileDialog {
			public OpenFileDialogAdapter(OpenFileDialog fileDialog) : base(fileDialog) { }
			bool IOpenFileDialog.Multiselect {
				get { return fileDialog.Multiselect; }
				set { fileDialog.Multiselect = value; }
			}
#if !SILVERLIGHT
			bool IOpenFileDialog.ReadOnlyChecked {
				get { return fileDialog.ReadOnlyChecked; }
				set { fileDialog.ReadOnlyChecked = value; }
			}
			bool IOpenFileDialog.ShowReadOnly {
				get { return fileDialog.ShowReadOnly; }
				set { fileDialog.ShowReadOnly = value; }
			}
#else
			string IOpenFileDialog.InitialDirectory {
				get { return fileDialog.InitialDirectory; }
				set { fileDialog.InitialDirectory = value; }
			}
#endif
		}
		public static readonly DependencyProperty MultiselectProperty =
			DependencyProperty.Register("Multiselect", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
#if !SILVERLIGHT
		public static readonly DependencyProperty ReadOnlyCheckedProperty =
			DependencyProperty.Register("ReadOnlyChecked", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowReadOnlyProperty =
			DependencyProperty.Register("ShowReadOnly", typeof(bool), typeof(OpenFileDialogService), new PropertyMetadata(false));
#else
		public static readonly DependencyProperty InitialDirectoryProperty =
			DependencyProperty.Register("InitialDirectory", typeof(string), typeof(OpenFileDialogService), new PropertyMetadata(string.Empty));
#endif
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(string), typeof(OpenFileDialogService), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty FilterIndexProperty =
			DependencyProperty.Register("FilterIndex", typeof(int), typeof(OpenFileDialogService), new PropertyMetadata(1));
		public bool Multiselect {
			get { return (bool)GetValue(MultiselectProperty); }
			set { SetValue(MultiselectProperty, value); }
		}
#if !SILVERLIGHT
		public bool ReadOnlyChecked {
			get { return (bool)GetValue(ReadOnlyCheckedProperty); }
			set { SetValue(ReadOnlyCheckedProperty, value); }
		}
		public bool ShowReadOnly {
			get { return (bool)GetValue(ShowReadOnlyProperty); }
			set { SetValue(ShowReadOnlyProperty, value); }
		}
#else
		public string InitialDirectory {
			get { return (string)GetValue(InitialDirectoryProperty); }
			set { SetValue(InitialDirectoryProperty, value); }
		}
#endif
		public string Filter {
			get { return (string)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}
		public int FilterIndex {
			get { return (int)GetValue(FilterIndexProperty); }
			set { SetValue(FilterIndexProperty, value); }
		}
		IOpenFileDialog OpenFileDialog { get { return (IOpenFileDialog)GetFileDialog(); } }
		public OpenFileDialogService() {
#if !SILVERLIGHT
			CheckFileExists = true;
#endif
		}
		protected override object CreateFileDialog() {
			return new OpenFileDialog();
		}
		protected override IFileDialog CreateFileDialogAdapter() {
			return new OpenFileDialogAdapter((OpenFileDialog)CreateFileDialog());
		}
		protected override void InitFileDialog() {
			OpenFileDialog.Multiselect = Multiselect;
#if !SILVERLIGHT
			OpenFileDialog.ReadOnlyChecked = ReadOnlyChecked;
			OpenFileDialog.ShowReadOnly = ShowReadOnly;
#else
			OpenFileDialog.InitialDirectory = InitialDirectory;
#endif
			OpenFileDialog.Filter = Filter;
			OpenFileDialog.FilterIndex = FilterIndex;
		}
		protected override List<FileInfoWrapper> GetFileInfos() {
#if !SILVERLIGHT
			List<FileInfoWrapper> res = new List<FileInfoWrapper>();
			foreach(string fileName in OpenFileDialog.FileNames)
				res.Add(FileInfoWrapper.Create(fileName));
			return res;
#else
			List<FileInfoWrapper> res = new List<FileInfoWrapper>();
			foreach(FileInfo fileInfo in OpenFileDialog.Files)
				res.Add(new FileInfoWrapper(fileInfo));
			return res;
#endif
		}
		IFileInfo IOpenFileDialogService.File { get { return GetFiles().FirstOrDefault(); } }
		IEnumerable<IFileInfo> IOpenFileDialogService.Files { get { return GetFiles(); } }
		bool IOpenFileDialogService.ShowDialog(Action<CancelEventArgs> fileOK, string directoryName) {
			if(directoryName != null)
				InitialDirectory = directoryName;
			var res = Show(fileOK);
			FilterIndex = OpenFileDialog.FilterIndex;
			return res;
		}
	}
}
