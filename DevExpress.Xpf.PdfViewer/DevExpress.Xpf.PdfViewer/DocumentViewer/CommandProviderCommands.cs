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
using System.Windows.Input;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Collections.ObjectModel;
using DevExpress.Xpf.DocumentViewer;
using CommandBase = DevExpress.Xpf.DocumentViewer.CommandBase;
using DevExpress.Pdf;
namespace DevExpress.Xpf.PdfViewer.Internal {
	public class PdfSetCursorModeItem : CommandBase {
		CursorModeType commandValue;
		bool isChecked;
		int groupIndex;
		public int GroupIndex {
			get { return groupIndex; }
			set { SetProperty(ref groupIndex, value, () => GroupIndex); }
		}
		public CursorModeType CommandValue {
			get { return commandValue; }
			set { SetProperty(ref commandValue, value, () => CommandValue); }
		}
		public bool IsChecked {
			get { return isChecked; }
			set { SetProperty(ref isChecked, value, () => IsChecked); }
		}
	}
	public class PdfOpenDocumentSplitItem : CommandBase {
		ObservableCollection<RecentFileViewModel> recentFiles;
		public ObservableCollection<RecentFileViewModel> RecentFiles {
			get { return recentFiles; }
			set { SetProperty(ref recentFiles, value, () => RecentFiles); }
		}
	}
	public class PdfSplitItem : CommandBase {
		ObservableCollection<ICommand> commands;
		public ObservableCollection<ICommand> Commands {
			get { return commands; }
			set { SetProperty(ref commands, value, () => Commands); }
		}
	}
	public class CommandSetPageLayout : CommandToggleButton {
		bool isSeparator;
		PdfPageLayout pageLayout;
		KeyGesture keyGesture;
		public bool IsSeparator {
			get { return isSeparator; }
			set { SetProperty(ref isSeparator, value, () => IsSeparator); }
		}
		public PdfPageLayout PageLayout {
			get { return pageLayout; }
			set { SetProperty(ref pageLayout, value, () => PageLayout); }
		}
		public KeyGesture KeyGesture {
			get { return keyGesture; }
			set { SetProperty(ref keyGesture, value, () => KeyGesture); }
		}
	}
	public class CommandShowCoverPage : CommandToggleButton { }
}
