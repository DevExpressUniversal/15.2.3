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

using System.Windows;
using System.Windows.Input;
namespace DevExpress.Xpf.DocumentViewer {
	public class DefaultBarManagerItemNames {
		public const string Open = "bOpen";
		public const string Close = "bClose";
		public const string PreviousPage = "bPreviousPage";
		public const string NextPage = "bNextPage";
		public const string Pagination = "bPagination";
		public const string ZoomIn = "bZoomIn";
		public const string ZoomOut = "bZoomOut";
		public const string Zoom = "bZoom";
		public const string ClockwiseRotate = "bClockwiseRotate";
		public const string CounterClockwiseRotate = "bCounterClockwiseRotate";
		public const string PreviousView = "bPreviousView";
		public const string NextView = "bNextView";
		public const string Search = "bSearch";
		public const string SearchSettings = "bSearchSettings";
		public const string SearchNext = "bSearchNext";
		public const string SearchPrevious = "bSearchPrev";
		public const string SearchClose = "bSearchClose";
		public const string SearchCheckWholeWord = "bCheckWholeWord";
		public const string SearchCheckCaseSensitive = "bCheckCaseSensitive";
		public const string Bar = "bMain";
		public const string DefaultPageCategory = "rcMain";
		public const string MainRibbonPage = "rpMain";
		public const string FileRibbonGroup = "rgFile";
		public const string NavigationRibbonGroup = "rgNavigation";
		public const string ZoomRibbonGroup = "rgZoom";
		public const string RotateRibbonGroup = "rgRotate";
		public const string DocumentMapShowOptions = "bDocumentMapOptions";
		public const string DocumentMapExpandCurrentNodeButton = "bDocumentMapExpandCurrentNodeButton";
		public const string DocumentMapExpandCurrentNode = "bDocumentMapExpandCurrentNode";
		public const string DocumentMapExpandTopLevelNode = "bDocumentMapExpandTopLevelNode";
		public const string DocumentMapCollapseTopLevelNode = "bDocumentMapCollapseTopLevelNode";
		public const string DocumentMapGoToBookmark = "bDocumentMapGoToBookmark";
		public const string DocumentMapHideAfterUseNode = "bDocumentMapHideAfterUseNode";
	}
	public class OpenDocumentBarItem : DocumentViewerBarButtonItem {
		static OpenDocumentBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(OpenDocumentBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.O, ModifierKeys.Control)));
		}
	}
	public class CloseDocumentBarItem : DocumentViewerBarButtonItem {
		static CloseDocumentBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(CloseDocumentBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.W, ModifierKeys.Control)));
		}
	}
	public class PreviousPageBarItem : DocumentViewerBarButtonItem { }
	public class NextPageBarItem : DocumentViewerBarButtonItem { }
	public class PaginationBarItem : DocumentViewerBarStaticItem { }
	public class ZoomInBarItem : DocumentViewerBarButtonItem {
		static ZoomInBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(ZoomInBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.OemPlus, ModifierKeys.Control)));
		}
	}
	public class ZoomOutBarItem : DocumentViewerBarButtonItem {
		static ZoomOutBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(ZoomOutBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.OemMinus, ModifierKeys.Control)));
		}
	}
	public class ZoomBarItem : DocumentViewerBarSubItem { }
	public class ShowOptionsBarItem : DocumentViewerBarSubItem { }
	public class ClockwiseRotateBarItem : DocumentViewerBarButtonItem {
		static ClockwiseRotateBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(ClockwiseRotateBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.OemPlus, ModifierKeys.Control | ModifierKeys.Shift)));
		}
	}
	public class CounterClockwiseRotateBarItem : DocumentViewerBarButtonItem {
		static CounterClockwiseRotateBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(CounterClockwiseRotateBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.OemMinus, ModifierKeys.Control | ModifierKeys.Shift)));
		}
	}
	public class PreviousViewBarItem : DocumentViewerBarButtonItem {
		static PreviousViewBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(PreviousViewBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.Left, ModifierKeys.Alt)));
		}
	}
	public class NextViewBarItem : DocumentViewerBarButtonItem {
		static NextViewBarItem() {
			KeyGestureProperty.OverrideMetadata(typeof(NextViewBarItem), new FrameworkPropertyMetadata(new KeyGesture(Key.Right, ModifierKeys.Alt)));
		}
	}
}
