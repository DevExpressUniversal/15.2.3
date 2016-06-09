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

using System.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace WindowsUI {
		class TileContainerInfoElementHitInfo : LayoutElementHitInfo {
			public TileContainerInfoElementHitInfo(Point hitPoint, TileContainerInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class SplitGroupInfoElementHitInfo : LayoutElementHitInfo {
			public SplitGroupInfoElementHitInfo(Point hitPoint, SplitGroupInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
		}
		class SlideGroupInfoElementHitInfo : LayoutElementHitInfo {
			public SlideGroupInfoElementHitInfo(Point hitPoint, SlideGroupInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InScrollBounds {
				get { return InBounds; }
			}
		}
		class PageGroupInfoElementHitInfo : LayoutElementHitInfo {
			public PageGroupInfoElementHitInfo(Point hitPoint, PageGroupInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class TabbedGroupInfoElementHitInfo : LayoutElementHitInfo {
			public TabbedGroupInfoElementHitInfo(Point hitPoint, TabbedGroupInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class PageInfoElementHitInfo : LayoutElementHitInfo {
			public PageInfoElementHitInfo(Point hitPoint, PageInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds && !InContent; }
			}
		}
		class FlyoutInfoElementHitInfo : LayoutElementHitInfo {
			public FlyoutInfoElementHitInfo(Point hitPoint, FlyoutInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds && !InContent; }
			}
		}
		class DetailContainerInfoElementHitInfo : LayoutElementHitInfo {
			public DetailContainerInfoElementHitInfo(Point hitPoint, DetailContainerInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds && !InContent; }
			}
		}
		class OverviewContainerInfoElementHitInfo : LayoutElementHitInfo {
			public OverviewContainerInfoElementHitInfo(Point hitPoint, OverviewContainerInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class ContentContainerHeaderInfoElementHitInfo : LayoutElementHitInfo {
			public ContentContainerHeaderInfoElementHitInfo(Point hitPoint, ContentContainerHeaderInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return false; }
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class SplitterInfoElementHitInfo : LayoutElementHitInfo {
			public SplitterInfoElementHitInfo(Point hitPoint, SplitterInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InResizeBounds {
				get { return InBounds; }
			}
			protected override LayoutElementHitTest[] GetValidHotTests() {
				return new LayoutElementHitTest[] { LayoutElementHitTest.Bounds };
			}
			protected override LayoutElementHitTest[] GetValidPressedTests() {
				return new LayoutElementHitTest[] { LayoutElementHitTest.Bounds };
			}
		}
		class DocumentInfoElementHitInfo : LayoutElementHitInfo {
			public DocumentInfoElementHitInfo(Point hitPoint, DocumentInfoElement element)
				: base(hitPoint, element) {
			}
		}
		class TileInfoElementHitInfo : LayoutElementHitInfo {
			public TileInfoElementHitInfo(Point hitPoint, TileInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InDragBounds {
				get { return InBounds; }
			}
		}
		class ContentContainerActionsBarInfoElementHitInfo : LayoutElementHitInfo {
			public ContentContainerActionsBarInfoElementHitInfo(Point hitPoint, ContentContainerActionsBarInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
		}
		class ScrollBarInfoElementHitInfo : LayoutElementHitInfo {
			public ScrollBarInfoElementHitInfo(Point hitPoint, ScrollBarInfoElement element)
				: base(hitPoint, element) {
			}
			public override bool InHandlerBounds {
				get { return InBounds; }
			}
			public override bool InScrollBounds {
				get { return InBounds; }
			}
		}
	}
}
