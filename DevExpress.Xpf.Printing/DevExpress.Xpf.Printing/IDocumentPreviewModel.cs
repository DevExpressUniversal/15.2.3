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
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing {
	public interface IDocumentPreviewModel : IPreviewModel {
		int CurrentPageIndex { get; set; }
		int CurrentPageNumber { get; set; }
		bool ProgressVisibility { get; }
		int ProgressMaximum { get; }
		int ProgressValue { get; }
		bool ProgressMarqueeVisibility { get; }
		DocumentMapTreeViewNode DocumentMapRootNode { get; }
		DocumentMapTreeViewNode DocumentMapSelectedNode { get; set; }
		bool IsDocumentMapVisible { get; set; }
		bool IsParametersPanelVisible { get; set; }
		ParametersModel ParametersModel { get; }
		bool IsScaleVisible { get; }
		bool IsSearchVisible { get; }
		bool IsEmptyDocument { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		BrickInfo FoundBrickInfo { get; set; }
		ICommand PrintCommand { get; }
		ICommand FirstPageCommand { get; }
		ICommand PreviousPageCommand { get; }
		ICommand NextPageCommand { get; }
		ICommand LastPageCommand { get; }
		ICommand ExportCommand { get; }
#if SL
		ICommand ExportToWindowCommand { get; }
#endif
		ICommand WatermarkCommand { get; }
		ICommand StopCommand { get; }
		ICommand ToggleDocumentMapCommand { get; }
		ICommand ToggleParametersPanelCommand { get; }
		ICommand ToggleSearchPanelCommand { get; }
		ICommand PageSetupCommand { get; }
		ICommand ScaleCommand { get; }
#if !SILVERLIGHT
		ICommand PrintDirectCommand { get; }
		ICommand SendCommand { get; }
		ICommand OpenCommand { get; }
		ICommand SaveCommand { get; }
#endif
#if SILVERLIGHT
		ICommand RefreshCommand { get; }
		void Clear();
		void CreateDocument();
#endif
		event EventHandler<PreviewClickEventArgs> PreviewClick;
		event EventHandler<PreviewClickEventArgs> PreviewMouseMove;
#if !SILVERLIGHT
		event EventHandler<PreviewClickEventArgs> PreviewDoubleClick;
#endif
	}
}
