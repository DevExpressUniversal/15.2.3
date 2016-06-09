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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.Utils.MVVM.Services {
	public abstract class FileDialogService {
		System.Windows.Forms.FileDialog fileDialog;
		public FileDialogService(System.Windows.Forms.FileDialog fileDialog) {
			this.fileDialog = fileDialog;
		}
		public object File {
			get {
				if(fileDialog == null) return null;
				return new System.IO.FileInfo(fileDialog.FileName);
			}
		}
		public string Filter {
			get {
				if(fileDialog == null) return null;
				return fileDialog.Filter;
			}
			set {
				if(fileDialog != null)
					fileDialog.Filter = value;
			}
		}
		public int FilterIndex {
			get {
				if(fileDialog == null) return -1;
				return fileDialog.FilterIndex;
			}
			set {
				if(fileDialog != null)
					fileDialog.FilterIndex = value;
			}
		}
		public string Title {
			get {
				if(fileDialog == null) return null;
				return fileDialog.Title;
			}
			set {
				if(fileDialog != null)
					fileDialog.Title = value;
			}
		}
		protected bool ShowDialog(Action<System.ComponentModel.CancelEventArgs> fileOK, string directoryName) {
			if(fileDialog == null) return false;
			fileDialog.InitialDirectory = directoryName;
			fileDialog.ShowDialog();
			return true;
		}
	}
	public class SaveFileDialogService : FileDialogService {
		FileDialog fileDialog;
		protected SaveFileDialogService(FileDialog fileDialog)
			: base(fileDialog) {
			this.fileDialog = fileDialog;
		}
		public string DefaultExt { get; set; }
		public string DefaultFileName { get; set; }
		public bool ShowDialog(Action<System.ComponentModel.CancelEventArgs> fileOK, string directoryName, string fileName) {
			this.fileDialog.FileName = fileName;
			return base.ShowDialog(fileOK, directoryName);
		}
		public new object File {
			get { return base.File; }
		}
		public new string Filter {
			get { return base.Filter; }
			set { base.Filter = value; }
		}
		public new int FilterIndex {
			get { return base.FilterIndex; }
			set { base.FilterIndex = value; }
		}
		public new string Title {
			get { return base.Title; }
			set { base.Title = value; }
		}
		public SaveFileDialogService Create() {
			return new SaveFileDialogService(new SaveFileDialog());
		}
	}
	public class OpenFileDialogService : FileDialogService {
		FileDialog fileDialog;
		protected OpenFileDialogService(FileDialog fileDialog)
			: base(fileDialog) {
			this.fileDialog = fileDialog;
		}
		public IEnumerable<object> Files {
			get { return fileDialog.FileNames; }
		}
		public new bool ShowDialog(Action<System.ComponentModel.CancelEventArgs> fileOK, string directoryName) {
			return base.ShowDialog(fileOK, directoryName);
		}
		public new object File {
			get { return base.File; }
		}
		public new string Filter {
			get { return base.Filter; }
			set { base.Filter = value; }
		}
		public new int FilterIndex {
			get { return base.FilterIndex; }
			set { base.FilterIndex = value; }
		}
		public new string Title {
			get { return base.Title; }
			set { base.Title = value; }
		}
		public OpenFileDialogService Create() {
			return new OpenFileDialogService(new OpenFileDialog());
		}
	}
}
