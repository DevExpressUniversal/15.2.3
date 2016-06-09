#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Service;
namespace DevExpress.DashboardExport {
	public class ExportScrollController {
		ItemViewerClientState clientState;
		public bool ShowHScroll { get; private set; }
		public bool ShowVScroll { get; private set; }
		public ExportScrollController(ItemViewerClientState clientState) {
			this.clientState = clientState;
		}
		public void CalculateShowScrollbars(bool isCardWithoutCaption, int initialScrollableAreaWidth, bool firstIteration) {
			int scrollableAreaHeight = clientState.VScrollingState != null ? clientState.VScrollingState.ScrollableAreaSize : clientState.ViewerArea.Height;
			bool vScrollingClientStateIsNull = clientState.VScrollingState == null;
			bool hScrollingClientStateIsNull = clientState.HScrollingState == null;
			if(vScrollingClientStateIsNull || hScrollingClientStateIsNull) {
				if(!vScrollingClientStateIsNull)
					ShowVScroll = clientState.VScrollingState.VirtualSize > scrollableAreaHeight;
				if(!hScrollingClientStateIsNull)
					ShowHScroll = clientState.HScrollingState.VirtualSize > initialScrollableAreaWidth;
				return;
			}
			int hScrollSize = isCardWithoutCaption ? clientState.HScrollingState.ScrollBarSize : ExportScrollBar.PrintSize;
			int vScrollSize = isCardWithoutCaption ? clientState.VScrollingState.ScrollBarSize : ExportScrollBar.PrintSize;
			int vContentSize = clientState.VScrollingState.VirtualSize;
			int hContentSize = clientState.HScrollingState.VirtualSize;
			ShowVScroll = vContentSize > scrollableAreaHeight;
			ShowHScroll = ShowVScroll ? hContentSize > initialScrollableAreaWidth - vScrollSize : hContentSize > initialScrollableAreaWidth;
			if(ShowHScroll && !ShowVScroll)
				ShowVScroll = firstIteration ? vContentSize > scrollableAreaHeight - hScrollSize : vContentSize > scrollableAreaHeight;
		}
	}
}
