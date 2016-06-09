﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.Printing {
	public interface IPreviewModel : INotifyPropertyChanged {
		int PageCount { get; }
		FrameworkElement PageContent { get; }
		double PageViewWidth { get; }
		double PageViewHeight { get; }
		double Zoom { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		void SetZoom(double value);
		ZoomItemBase ZoomMode { get; set; }
		string ZoomDisplayFormat { get; set; }
		string ZoomDisplayText { get; }
		IEnumerable<ZoomItemBase> ZoomModes { get; }
		bool IsCreating { get; }
		bool IsLoading { get; }
		bool IsIncorrectPageContent { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		IDialogService DialogService { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		ICursorService CursorService { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool UseSimpleScrolling { get; set; }
		InputController InputController { get; }
		ICommand ZoomOutCommand { get; }
		ICommand ZoomInCommand { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		void HandlePreviewMouseLeftButtonDown(MouseButtonEventArgs e, FrameworkElement source);
		[EditorBrowsable(EditorBrowsableState.Never)]
		void HandlePreviewMouseLeftButtonUp(MouseButtonEventArgs e, FrameworkElement source);
		[EditorBrowsable(EditorBrowsableState.Never)]
		void HandlePreviewMouseMove(MouseEventArgs e, FrameworkElement source);
#if !SILVERLIGHT
		[EditorBrowsable(EditorBrowsableState.Never)]
		void HandlePreviewDoubleClick(MouseEventArgs e, FrameworkElement source);
#endif
	}
}
