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
	using DevExpress.XtraBars.Docking2010.Views.Tabbed;
	using DevExpress.XtraTab;
	using DevExpress.XtraTab.ViewInfo;
	class TabbedViewElement : BaseViewElement {
		public TabbedViewElement(TabbedView view)
			: base(view) {
		}
	}
	namespace Tabbed {
		class DocumentGroupInfoElement : DocumentLayoutContainer {
			IDocumentGroupInfo infoCore;
			IXtraTab tabCore;
			Point offsetNC;
			public DocumentGroupInfoElement(IDocumentGroupInfo info) {
				infoCore = info;
				tabCore = info.Tab;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IXtraTab Tab {
				get { return tabCore; }
			}
			public IDocumentGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new DocumentGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new DocumentGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				BaseTabControlViewInfo tabInfo = Tab.ViewInfo;
				if(tabInfo != null) {
					Rectangle header = Offset(tabInfo.HeaderInfo.Bounds, offsetNC);
					Rectangle content = Offset(tabInfo.PageClientBounds, offsetNC);
					Rectangle buttons = Offset(tabInfo.HeaderInfo.ButtonsBounds, offsetNC);
					hitInfo.CheckAndSetHitTest(header, hitInfo.HitPoint, LayoutElementHitTest.Header);
					hitInfo.CheckAndSetHitTest(content, hitInfo.HitPoint, LayoutElementHitTest.Content);
					hitInfo.CheckAndSetHitTest(buttons, hitInfo.HitPoint, LayoutElementHitTest.ControlBox);
				}
			}
		}
		class DocumentInfoElement : DocumentLayoutElement {
			IDocumentInfo infoCore;
			IXtraTabPage pageCore;
			Point offsetNC;
			public IDocumentInfo Info {
				get { return infoCore; }
			}
			public bool CanFloat {
				get { return Info.BaseDocument.CanFloat() && !((IDesignTimeSupport)Info.Owner).IsLoaded; }
			}
			public IXtraTabPage TabPage {
				get { return pageCore; }
			}
			public DocumentInfoElement(IDocumentInfo info) {
				infoCore = info;
				pageCore = info.TabPage;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public override bool HitTestingEnabled {
				get { return infoCore != null && Info.BaseDocument != null && Info.BaseDocument.IsVisible; }
			}
			protected override void EnsureBoundsCore() {
				BaseTabPageViewInfo pageInfo = TabPage.TabControl.ViewInfo.HeaderInfo.VisiblePages[TabPage];
				if(pageInfo != null) {
					Bounds = Offset(pageInfo.Bounds, offsetNC);
				}
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
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				BaseTabHeaderViewInfo headerInfo = TabPage.TabControl.ViewInfo.HeaderInfo;
				BaseTabPageViewInfo pageInfo = headerInfo.VisiblePages[TabPage];
				if(pageInfo != null) {
					Rectangle header = Offset(pageInfo.Bounds, offsetNC);
					Rectangle controlBox = Offset(pageInfo.ButtonsPanel.Bounds, offsetNC);
					Rectangle tabButtons = Offset(headerInfo.ButtonsBounds, offsetNC);
					hitInfo.CheckAndSetHitTest(header, hitInfo.HitPoint, LayoutElementHitTest.Header);
					hitInfo.CheckAndSetHitTest(controlBox, hitInfo.HitPoint, LayoutElementHitTest.ControlBox);
					hitInfo.CheckAndSetHitTest(tabButtons, hitInfo.HitPoint, LayoutElementHitTest.ControlBox);
				}
			}
			public bool CanReordering { get { return Info.Document.Properties.CanReorderTab; } }
		}
#if DEBUGTEST
		class ResizeAssistentElement : DocumentLayoutElement {
			IResizeAssistentInfo elementInfoCore;
			Point offsetNC;
			public ResizeAssistentElement(IResizeAssistentInfo info) {
				elementInfoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IResizeAssistentInfo Info { get { return elementInfoCore; } }
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return elementInfoCore;
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new ResizeAssistentElementHitInfo(pt, this);
			}
		}
#endif
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
	}
}
