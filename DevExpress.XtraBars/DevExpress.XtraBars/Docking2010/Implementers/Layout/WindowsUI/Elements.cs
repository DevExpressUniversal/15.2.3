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
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
	class WindowsUIViewElement : BaseViewElement {
		public WindowsUIViewElement(WindowsUIView view)
			: base(view) {
		}
	}
	namespace WindowsUI {
		class TileContainerInfoElement : DocumentLayoutContainer {
			ITileContainerInfo infoCore;
			Point offsetNC;
			public TileContainerInfoElement(ITileContainerInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ITileContainerInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new TileContainerInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new TileContainerInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class SplitGroupInfoElement : DocumentLayoutContainer {
			ISplitGroupInfo infoCore;
			Point offsetNC;
			public SplitGroupInfoElement(ISplitGroupInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ISplitGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new SplitGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new SplitGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class SlideGroupInfoElement : DocumentLayoutContainer {
			ISlideGroupInfo infoCore;
			Point offsetNC;
			public SlideGroupInfoElement(ISlideGroupInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ISlideGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new SlideGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new SlideGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class PageGroupInfoElement : DocumentLayoutContainer {
			IPageGroupInfo infoCore;
			Point offsetNC;
			public PageGroupInfoElement(IPageGroupInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IPageGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new PageGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new PageGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class TabbedGroupInfoElement : DocumentLayoutContainer {
			ITabbedGroupInfo infoCore;
			Point offsetNC;
			public TabbedGroupInfoElement(ITabbedGroupInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ITabbedGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new TabbedGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new TabbedGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class PageInfoElement : DocumentLayoutContainer {
			IPageInfo infoCore;
			Point offsetNC;
			public PageInfoElement(IPageInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IPageInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new PageInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new PageInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				IDocumentInfo documentInfo = (Info != null) ? Info.DocumentInfo : null;
				if(documentInfo != null) {
					Rectangle content = Offset(documentInfo.Bounds, offsetNC);
					hitInfo.CheckAndSetHitTest(content, hitInfo.HitPoint, LayoutElementHitTest.Content);
				}
			}
		}
		class FlyoutInfoElement : DocumentLayoutContainer {
			IFlyoutInfo infoCore;
			Point offsetNC;
			public FlyoutInfoElement(IFlyoutInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IFlyoutInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new FlyoutInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new FlyoutInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				IDocumentInfo documentInfo = (Info != null) ? Info.DocumentInfo : null;
				if(documentInfo != null) {
					Rectangle content = Offset(documentInfo.Bounds, offsetNC);
					hitInfo.CheckAndSetHitTest(content, hitInfo.HitPoint, LayoutElementHitTest.Content);
				}
			}
		}
		class DetailContainerInfoElement : DocumentLayoutContainer {
			IDetailContainerInfo infoCore;
			Point offsetNC;
			public DetailContainerInfoElement(IDetailContainerInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IDetailContainerInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new DetailContainerInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new DetailContainerInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				IDocumentInfo documentInfo = (Info != null) ? Info.DocumentInfo : null;
				if(documentInfo != null) {
					Rectangle content = Offset(documentInfo.Bounds, offsetNC);
					hitInfo.CheckAndSetHitTest(content, hitInfo.HitPoint, LayoutElementHitTest.Content);
				}
			}
		}
		class OverviewContainerInfoElement : DocumentLayoutContainer {
			IOverviewContainerInfo infoCore;
			Point offsetNC;
			public OverviewContainerInfoElement(IOverviewContainerInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IOverviewContainerInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new OverviewContainerInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new OverviewContainerInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class SplitterInfoElement : DocumentLayoutElement {
			ISplitterInfo infoCore;
			Point offsetNC;
			public SplitterInfoElement(ISplitterInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ISplitterInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new SplitterInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new SplitterInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override object InitHotState() {
				return InitState(ObjectState.Hot);
			}
			protected override object InitPressedState() {
				return InitState(ObjectState.Pressed);
			}
			protected override void OnStateChanged(object hitResult, State state) {
				Info.State = (ObjectState)((int)state);
			}
			protected object InitState(ObjectState state) {
				return ((Info.State & state) != 0) ? (object)LayoutElementHitTest.Bounds : null;
			}
		}
		class ContentContainerHeaderInfoElement : DocumentLayoutContainer {
			IContentContainerHeaderInfo infoCore;
			Point offsetNC;
			public ContentContainerHeaderInfoElement(IContentContainerHeaderInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IContentContainerHeaderInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new ContentContainerHeaderInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new ContentContainerHeaderInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class DocumentInfoElement : DocumentLayoutElement {
			IDocumentInfo infoCore;
			Point offsetNC;
			public DocumentInfoElement(IDocumentInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IDocumentInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new DocumentInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new DocumentInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class TileInfoElement : DocumentLayoutElement {
			ITileInfo infoCore;
			Point offsetNC;
			public TileInfoElement(ITileInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public ITileInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.ItemInfo.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new TileInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new TileInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class ContentContainerActionsBarInfoElement : DocumentLayoutContainer {
			IContentContainerActionsBarInfo infoCore;
			Point offsetNC;
			public ContentContainerActionsBarInfoElement(IContentContainerActionsBarInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IContentContainerActionsBarInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new ContentContainerActionsBarInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new ContentContainerActionsBarInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
		class ScrollBarInfoElement : DocumentLayoutElement {
			IScrollBarInfo infoCore;
			public ScrollBarInfoElement(IScrollBarInfo info) {
				infoCore = info;
			}
			public IScrollBarInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Info.Bounds;
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new ScrollBarInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new ScrollBarInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
		}
	}
}
