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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Windows.Input;
namespace DevExpress.Xpf.DocumentViewer {
	public class CommandButton : CommandBase { }
	public class CommandToggleButton : CommandBase {
		int groupIndex;
		bool isChecked;
		public bool IsChecked {
			get { return isChecked; }
			set { SetProperty(ref isChecked, value, () => IsChecked); }
		}
		public int GroupIndex {
			get { return groupIndex; }
			set { SetProperty(ref groupIndex, value, () => GroupIndex); }
		}
	}
	public class CommandCheckItems : CommandBase {
		IEnumerable<CommandToggleButton> items;
		public IEnumerable<CommandToggleButton> Items {
			get { return items; }
			set { SetProperty(ref items, value, () => Items); }
		}
		public void UpdateCheckState(Func<CommandToggleButton, bool> checkItemFunc) {
			if (Items == null)
				return;
			foreach (var item in Items)
				item.IsChecked = checkItemFunc(item);
		}
	}
	public class CommandSetZoomFactorAndModeItem : CommandToggleButton {
		bool isSeparator;
		double zoomFactor;
		ZoomMode zoomMode;
		KeyGesture keyGesture;
		public bool IsSeparator {
			get { return isSeparator; }
			set { SetProperty(ref isSeparator, value, () => IsSeparator); }
		}
		public double ZoomFactor {
			get { return zoomFactor; }
			set { SetProperty(ref zoomFactor, value, () => ZoomFactor); }
		}
		public ZoomMode ZoomMode {
			get { return zoomMode; }
			set { SetProperty(ref zoomMode, value, () => ZoomMode); }
		}
		public KeyGesture KeyGesture {
			get { return keyGesture; }
			set { SetProperty(ref keyGesture, value, () => KeyGesture); }
		}
	}
	public class CommandPagination : CommandBase {
		int currentPageNumber;
		int pageCount;
		public int CurrentPageNumber {
			get { return currentPageNumber; }
			set { SetProperty(ref currentPageNumber, value, () => CurrentPageNumber); }
		}
		public int PageCount {
			get { return pageCount; }
			set { SetProperty(ref pageCount, value, () => PageCount); }
		}
	}
}
