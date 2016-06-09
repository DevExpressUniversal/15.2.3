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
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	using DevExpress.XtraBars.Docking2010.Views.Widget;
	class WidgetViewElement : BaseViewElement {
		public WidgetViewElement(WidgetView view)
			: base(view) {
		}
		protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
			return new WidgetViewElementHitInfo(pt, this);
		}
	}
	namespace Widget {
		class DocumentInfoElement : DocumentLayoutContainer {
			IDocumentInfo infoCore;
			Point offsetNC;
			public DocumentInfoElement(IDocumentInfo info) {
				infoCore = info;
				offsetNC = Info.Document.Manager.GetOffsetNC();
			}
			public IDocumentInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new LayoutElementHitInfo(pt, this);
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				if(Info == null || Info.Document == null || Info.Document.ContainerControl == null) return;
				FloatDocumentFormInfo documentContainerInfo = Info.Document.ContainerControl.Info;
				if(documentContainerInfo != null) {
					Rectangle header = Offset(documentContainerInfo.caption, offsetNC);
					header = Offset(header, Info.Bounds.Location);
					Rectangle controlBox = Offset(documentContainerInfo.controlBox, offsetNC);
					controlBox = Offset(controlBox, Info.Bounds.Location);
					hitInfo.CheckAndSetHitTest(header, hitInfo.HitPoint, LayoutElementHitTest.Header);
					hitInfo.CheckAndSetHitTest(controlBox, hitInfo.HitPoint, LayoutElementHitTest.ControlBox);
				}
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return Info is IBaseElementInfo ? Info as IBaseElementInfo : base.GetElementInfoCore();
			}
		}
		class StackGroupInfoElement : DocumentLayoutContainer {
			IStackGroupInfo infoCore;
			Point offsetNC;
			public StackGroupInfoElement(IStackGroupInfo info) {
				infoCore = info;
				offsetNC = Info.Owner.Manager.GetOffsetNC();
			}
			public IStackGroupInfo Info {
				get { return infoCore; }
			}
			protected override void EnsureBoundsCore() {
				Bounds = Offset(Info.Bounds, offsetNC);
			}
			public override ILayoutElementBehavior GetBehavior() {
				return new StackGroupInfoElementBehavior(this);
			}
			protected override LayoutElementHitInfo CreateHitInfo(Point pt) {
				return new StackGroupInfoElementHitInfo(pt, this);
			}
			protected override IBaseElementInfo GetElementInfoCore() {
				return infoCore;
			}
			protected override void CalcHitInfoCore(LayoutElementHitInfo hitInfo) {
				base.CalcHitInfoCore(hitInfo);
				Rectangle caption = Offset(infoCore.CaptionBounds, offsetNC);
				hitInfo.CheckAndSetHitTest(Bounds, hitInfo.HitPoint, LayoutElementHitTest.Bounds);
				hitInfo.CheckAndSetHitTest(caption, hitInfo.HitPoint, LayoutElementHitTest.Header);
			}
		}
	}
}
