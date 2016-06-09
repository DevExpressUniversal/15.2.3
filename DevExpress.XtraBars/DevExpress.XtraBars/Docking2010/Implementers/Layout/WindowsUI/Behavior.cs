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

using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	namespace WindowsUI {
		class TileContainerInfoElementBehavior : DockLayoutElementBehavior {
			TileContainerInfoElement elementCore;
			protected TileContainerInfoElement Element {
				get { return elementCore; }
			}
			public TileContainerInfoElementBehavior(TileContainerInfoElement element) {
				elementCore = element;
			}
		}
		class SplitGroupInfoElementBehavior : DockLayoutElementBehavior {
			SplitGroupInfoElement elementCore;
			protected SplitGroupInfoElement Element {
				get { return elementCore; }
			}
			public SplitGroupInfoElementBehavior(SplitGroupInfoElement element) {
				elementCore = element;
			}
		}
		class SlideGroupInfoElementBehavior : DockLayoutElementBehavior {
			SlideGroupInfoElement elementCore;
			protected SlideGroupInfoElement Element {
				get { return elementCore; }
			}
			public SlideGroupInfoElementBehavior(SlideGroupInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return true; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.Scrolling:
						return true;
				}
				return false;
			}
		}
		class PageGroupInfoElementBehavior : DockLayoutElementBehavior {
			PageGroupInfoElement elementCore;
			protected PageGroupInfoElement Element {
				get { return elementCore; }
			}
			public PageGroupInfoElementBehavior(PageGroupInfoElement element) {
				elementCore = element;
			}
		}
		class TabbedGroupInfoElementBehavior : DockLayoutElementBehavior {
			TabbedGroupInfoElement elementCore;
			protected TabbedGroupInfoElement Element {
				get { return elementCore; }
			}
			public TabbedGroupInfoElementBehavior(TabbedGroupInfoElement element) {
				elementCore = element;
			}
		}
		class PageInfoElementBehavior : DockLayoutElementBehavior {
			PageInfoElement elementCore;
			protected PageInfoElement Element {
				get { return elementCore; }
			}
			public PageInfoElementBehavior(PageInfoElement element) {
				elementCore = element;
			}
		}
		class FlyoutInfoElementBehavior : DockLayoutElementBehavior {
			FlyoutInfoElement elementCore;
			protected FlyoutInfoElement Element {
				get { return elementCore; }
			}
			public FlyoutInfoElementBehavior(FlyoutInfoElement element) {
				elementCore = element;
			}
		}
		class DetailContainerInfoElementBehavior : DockLayoutElementBehavior {
			DetailContainerInfoElement elementCore;
			protected DetailContainerInfoElement Element {
				get { return elementCore; }
			}
			public DetailContainerInfoElementBehavior(DetailContainerInfoElement element) {
				elementCore = element;
			}
		}
		class OverviewContainerInfoElementBehavior : DockLayoutElementBehavior {
			OverviewContainerInfoElement elementCore;
			protected OverviewContainerInfoElement Element {
				get { return elementCore; }
			}
			public OverviewContainerInfoElementBehavior(OverviewContainerInfoElement element) {
				elementCore = element;
			}
		}
		class SplitterInfoElementBehavior : DockLayoutElementBehavior {
			SplitterInfoElement elementCore;
			protected SplitterInfoElement Element {
				get { return elementCore; }
			}
			public SplitterInfoElementBehavior(SplitterInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return true; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.Resizing:
						return true;
				}
				return false;
			}
		}
		class ContentContainerHeaderInfoElementBehavior : DockLayoutElementBehavior {
			ContentContainerHeaderInfoElement elementCore;
			protected ContentContainerHeaderInfoElement Element {
				get { return elementCore; }
			}
			public ContentContainerHeaderInfoElementBehavior(ContentContainerHeaderInfoElement element) {
				elementCore = element;
			}
		}
		class DocumentInfoElementBehavior : DockLayoutElementBehavior {
			DocumentInfoElement elementCore;
			protected DocumentInfoElement Element {
				get { return elementCore; }
			}
			public DocumentInfoElementBehavior(DocumentInfoElement element) {
				elementCore = element;
			}
		}
		class TileInfoElementBehavior : DockLayoutElementBehavior {
			TileInfoElement elementCore;
			protected TileInfoElement Element {
				get { return elementCore; }
			}
			public TileInfoElementBehavior(TileInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return true; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.Docking:
						return true;
				}
				return false;
			}
		}
		class ContentContainerActionsBarInfoElementBehavior : DockLayoutElementBehavior {
			ContentContainerActionsBarInfoElement elementCore;
			protected ContentContainerActionsBarInfoElement Element {
				get { return elementCore; }
			}
			public ContentContainerActionsBarInfoElementBehavior(ContentContainerActionsBarInfoElement element) {
				elementCore = element;
			}
		}
		class ScrollBarInfoElementBehavior : DockLayoutElementBehavior {
			ScrollBarInfoElement elementCore;
			protected ScrollBarInfoElement Element {
				get { return elementCore; }
			}
			public ScrollBarInfoElementBehavior(ScrollBarInfoElement element) {
				elementCore = element;
			}
			public override bool AllowDragging {
				get { return true; }
			}
			public override bool CanDrag(OperationType operation) {
				switch(operation) {
					case OperationType.Scrolling:
						return true;
				}
				return false;
			}
		}
	}
}
