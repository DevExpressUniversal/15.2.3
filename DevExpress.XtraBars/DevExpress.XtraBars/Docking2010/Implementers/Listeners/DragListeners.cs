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
using System.Windows.Forms;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	public class DocumentManagerUIViewRegularDragListener : RegularListener {
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		DockingHelper helper;
		public override void OnEnter(Point point, ILayoutElement element) {
			helper = new DockingHelper(View);
			helper.UpdateDockingAdorner(point, element);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			helper.UpdateDockingAdorner(point, element);
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
			return (info != null) && (info.BaseDocument != null);
		}
		bool doSnap = false;
		public override bool CanDrop(Point point, ILayoutElement element) {
			if((helper != null) && !helper.CanDrop(point, element)) {
				Customization.AdornerElementInfo info = View.Manager.SnapAdorner.GetSnapAdornerInfo();
				if(info != null) {
					Customization.SnapAdornerInfoArgs infoArgs = info.InfoArgs as Customization.SnapAdornerInfoArgs;
					doSnap = SnapHelper.CanSnap(point, infoArgs);
					return doSnap;
				}
				return false;
			}
			return true;
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(doSnap) {
				Customization.AdornerElementInfo info = View.Manager.SnapAdorner.GetSnapAdornerInfo();
				Customization.SnapAdornerInfoArgs infoArgs = info.InfoArgs as Customization.SnapAdornerInfoArgs;
				BaseFloatFormUIView view = View.Adapter.GetView(element) as BaseFloatFormUIView;
				if(infoArgs != null && view != null) {
					SnapHelper.Snap(point, element, infoArgs, view);
					View.Manager.SnapAdorner.Reset(info);
					doSnap = false;
				}
			}
			else {
				BaseFloatFormUIView floatView = View.Adapter.GetView(element) as BaseFloatFormUIView;
				if(floatView != null && DragOperationContext.Current != null)
					floatView.EndFloating(EndFloatingReason.Docking);
				if(helper != null) helper.Drop(point, element);
			}
			if(helper != null) helper.ResetDockingAdorner();
		}
		public override void OnLeave() {
			doSnap = false;
			if(helper != null) helper.ResetDockingAdorner();
		}
		public override void OnCancel() {
			BaseFloatFormUIView floatView = View.Adapter.GetView(View.Adapter.DragService.DragItem) as BaseFloatFormUIView;
			if(floatView != null && DragOperationContext.Current != null)
				floatView.EndFloating(EndFloatingReason.Reposition);
			doSnap = false;
			if(helper != null) helper.ResetDockingAdorner();
		}
	}
	public class DocumentManagerUIViewReorderingListener : ReorderingListener {
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		IDocumentGroupInfo groupInfo;
		public override void OnEnter(Point point, ILayoutElement element) {
			IBaseDocumentInfo documentInfo = InfoHelper.GetBaseDocumentInfo(element);
			LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, point);
			groupInfo = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(groupInfo != null)
				UpdateReordering(point, documentInfo.BaseDocument);
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			IBaseDocumentInfo documentInfo = InfoHelper.GetBaseDocumentInfo(element);
			LayoutElementHitInfo hitInfo = View.Adapter.CalcHitInfo(View, point);
			if(hitInfo.Element == null) return;
			groupInfo = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(groupInfo != null)
				UpdateReordering(point, documentInfo.BaseDocument);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			IBaseDocumentInfo documentInfo = InfoHelper.GetBaseDocumentInfo(element);
			if(CheckGroupInfo(element))
				UpdateReordering(point, documentInfo.BaseDocument);
		}
		bool CheckGroupInfo(ILayoutElement element) {
			if(groupInfo != null) {
				if(groupInfo.IsDisposing)
					groupInfo = InfoHelper.GetDocumentGroupInfo(element);
			}
			return (groupInfo != null) && !groupInfo.IsDisposing;
		}
		void UpdateReordering(Point point, BaseDocument document) {
			bool canUpdate = true;
			if(document.IsDockPanel) {
				Docking.DockPanel panel = document.GetDockPanel();
				canUpdate = (panel.DockManager == View.Manager.DockManager);
			}
			if(canUpdate)
				groupInfo.UpdateReordering(View.Manager.Adorner, point, document);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			IBaseDocumentInfo documentInfo = InfoHelper.GetBaseDocumentInfo(element);
			if(CheckGroupInfo(element)) {
				BaseFloatFormUIView floatView = View.Adapter.GetView(element) as BaseFloatFormUIView;
				if(floatView != null && DragOperationContext.Current != null)
					floatView.EndFloating(EndFloatingReason.Docking);
				groupInfo.Reorder(point, documentInfo.BaseDocument);
				groupInfo.ResetReordering(View.Manager.Adorner);
			}
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return InfoHelper.GetBaseDocumentInfo(element) != null;
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return groupInfo != null && groupInfo.CanReorder(point);
		}
		public override void OnLeave() {
			if(groupInfo != null)
				groupInfo.ResetReordering(View.Manager.Adorner);
		}
		public override void OnCancel() {
			if(groupInfo != null)
				groupInfo.ResetReordering(View.Manager.Adorner);
		}
	}
	public class DocumentManagerUIViewResizingListener : ResizingListener {
		SplitHelper helper;
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
			info.BeginSplit(View.Manager.Adorner);
			helper = new SplitHelper(View, point, info);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			int change = helper.CalcChange(point);
			ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
			info.UpdateSplit(View.Manager.Adorner, change);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
			info.ResetSplit(View.Manager.Adorner);
			info.MoveSplitter(helper.CalcSplitChange(point));
		}
		public override void OnCancel() {
			ISplitterInfo info = InfoHelper.GetSplitterInfo(View.Adapter.DragService.DragItem);
			info.ResetSplit(View.Manager.Adorner);
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			return helper.CanSplit(point);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			ISplitterInfo info = InfoHelper.GetSplitterInfo(element);
			if(info != null && info.Owner != null)
				return info.Owner.RaiseEndSizing(helper.CalcChange(point), info);
			return true; 
		}
	}
	public class DocumentManagerUIViewFloatingDragListener : FloatingListener {
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		FloatingHelper helper;
		protected override void InitFloating(ILayoutElement element) {
			helper = new FloatingHelper(View, element);
			using(helper.LockNestedDocumentManagerUpdate())
				base.InitFloating(element);
		}
		protected override IUIView GetFloatingView(ILayoutElement element) {
			return helper.GetFloatingView();
		}
		protected override void InitFloatingView(IUIView floatingView, Rectangle itemScreenRect, Rectangle itemContainerScreenRect) {
			itemContainerScreenRect.Offset(View.Manager.GetOffsetNC());
			helper.InitFloatingView(floatingView, itemContainerScreenRect);
		}
	}
	public class DocumentManagerUIViewDockingListener : DockingListener {
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		LocationHelper helper;
		public override void OnEnter(Point point, ILayoutElement element) {
			helper = new LocationHelper(View, element);
			Point screenPoint = View.ClientToScreen(point);
			View.BeginDocking(helper.CalcLocation(screenPoint), element);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			Point screenPoint = View.ClientToScreen(point);
			View.Docking(helper.CalcLocation(screenPoint));
		}
		public override void OnCancel() {
			View.EndDocking();
		}
	}
	public class BaseFloatFormUIViewNonClientDragListener : NonClientDraggingListener {
		ISnapHelperOwnerView View {
			get { return ServiceProvider as ISnapHelperOwnerView; }
		}
		SnapHelper helper;
		public override void OnEnter(Point point, ILayoutElement element) {
			helper = new SnapHelper(View);
		}
		public override void OnDragging(Point screenPoint, ILayoutElement element) {
			helper.UpdateSnapping(screenPoint, element);
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return helper.CanSnap(point, element);
		}
		public override void OnDrop(Point screenPoint, ILayoutElement element) {
			helper.Snap(screenPoint, element);
			helper = null;
		}
		public override void OnCancel() {
			helper.ResetSnapping();
			helper = null;
		}
		public override void OnLeave() {
			if(helper != null) {
				helper.ResetSnapping();
				helper = null;
			}
		}
		public override bool CanDrag(Point point, ILayoutElement element) {
			IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
			return (info != null) && (info.BaseDocument != null);
		}
	}
	public class BaseFloatFormUIViewFloatingMovingListener : FloatingMovingListener {
		ISnapHelperOwnerView View {
			get { return ServiceProvider as ISnapHelperOwnerView; }
		}
		LocationHelper helper;
		public override void OnBegin(Point point, ILayoutElement element) {
			helper = new LocationHelper(View, element);
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(View.IsFloatLocationLocked) return;
			Point screenPoint = View.ClientToScreen(point);
			SnapHelper.TryRestoreBounds(element, screenPoint);
			View.SetFloatLocation(helper.CalcLocation(screenPoint));
		}
		public override void OnCancel() {
			if(!View.InExternalDragging && !View.IsDisposing) {
				DragOperationContext.Current.RegisterAction(delegate {
					View.EndFloating(EndFloatingReason.Reposition);
				});
			}
		}
	}
	public class FloatPanelUIViewFloatingMovingListener : BaseFloatFormUIViewFloatingMovingListener {
		FloatPanelUIView View {
			get { return ServiceProvider as FloatPanelUIView; }
		}
		public override void OnBegin(Point point, ILayoutElement element) {
			base.OnBegin(point, element);
			if(!View.InExternalDragging)
				View.StartPanelDocking(View.ClientToScreen(point));
		}
		public override void OnDragging(Point point, ILayoutElement element) {
			if(!View.InExternalDragging)
				View.PanelDocking(View.ClientToScreen(point));
			base.OnDragging(point, element);
		}
		public override void OnDrop(Point point, ILayoutElement element) {
			if(!View.InExternalDragging)
				View.EndPanelDocking(View.ClientToScreen(point));
			base.OnDrop(point, element);
		}
		public override void OnCancel() {
			if(!View.InExternalDragging)
				View.CancelPanelDocking();
			base.OnCancel();
		}
		public override bool CanDrop(Point point, ILayoutElement element) {
			return true;
		}
	}
	public class DocumentHostWindowUIViewRegularDragListener : RegularListener {
		ISnapHelperOwnerView View {
			get { return ServiceProvider as ISnapHelperOwnerView; }
		}
		public override void OnCancel() {
			BaseFloatFormUIView floatView = View.Adapter.GetView(View.Adapter.DragService.DragItem) as BaseFloatFormUIView;
			if(floatView != null && DragOperationContext.Current != null)
				floatView.EndFloating(EndFloatingReason.Reposition);
		}
	}
	static class InfoHelper {
		public static IBaseDocumentInfo GetBaseDocumentInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as IBaseDocumentInfo;
		}
		public static IBaseSplitterInfo GetBaseSplitterInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as IBaseSplitterInfo;
		}
		public static IDocumentInfo GetDocumentInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as IDocumentInfo;
		}
		public static ISplitterInfo GetSplitterInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as ISplitterInfo;
		}
		public static IFloatPanelInfo GetFloatPanelInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as IFloatPanelInfo;
		}
		public static IDockingAdornerInfo GetDockingAdornerInfo(ILayoutElement element) {
			IBaseElementInfo info = ((IDocumentLayoutElement)element).GetElementInfo();
			IDockingAdornerInfo adornerInfo = info as IDockingAdornerInfo;
			if(adornerInfo != null)
				return adornerInfo;
			Views.Tabbed.IDocumentInfo docInfoTabbed = info as Views.Tabbed.IDocumentInfo;
			if(docInfoTabbed != null)
				return docInfoTabbed.GroupInfo;
			return null;
		}
		public static IDocumentGroupInfo GetDocumentGroupInfo(ILayoutElement element) {
			IBaseElementInfo info = ((IDocumentLayoutElement)element).GetElementInfo();
			IDocumentGroupInfo groupInfo = info as IDocumentGroupInfo;
			if(groupInfo != null)
				return groupInfo;
			IDocumentInfo docInfo = info as IDocumentInfo;
			if(docInfo != null)
				return docInfo.GroupInfo;
			return null;
		}
		public static IInteractiveElementInfo GetInteractiveElementInfo(ILayoutElement element) {
			return ((IDocumentLayoutElement)element).GetElementInfo() as IInteractiveElementInfo;
		}
	}
	namespace WidgetUI {
		using DevExpress.XtraBars.Docking2010.Views.Widget;
		public class DocumentManagerUIViewReorderingListener : ReorderingListener {
			WidgetView viewCore;
			IStackGroupInfo groupInfoCore;
			IStackGroupInfo targetGroupInfoCore;
			public StackGroup Group { get { return groupInfoCore != null ? groupInfoCore.Group : null; } }
			public StackGroup TargetGroup { get { return targetGroupInfoCore != null ? targetGroupInfoCore.Group : null; } }
			public DocumentManagerUIViewReorderingListener(WidgetView view)
				: base() {
				viewCore = view;
			}
			public WidgetView View {
				get { return viewCore; }
			}
			public override void OnBegin(Point point, ILayoutElement element) {
				groupInfoCore = InfoHelper.GetStackGroupInfo(point, View);
			}
			public override void OnDragging(Point point, ILayoutElement element) {
				targetGroupInfoCore = InfoHelper.GetStackGroupInfo(point, View);
				if(targetGroupInfoCore != null) View.WidgetViewInfo.UpdateDragging(targetGroupInfoCore);
			}
			public override void OnDrop(Point point, ILayoutElement element) {
				View.WidgetViewInfo.Reorder(Group, TargetGroup);
			}
			public override bool CanDrag(Point point, ILayoutElement element) {
				return Group != null && Group.Properties.CanDrag;
			}
			public override bool CanDrop(Point point, ILayoutElement element) {
				return TargetGroup != null && TargetGroup.Properties.CanDrag && View.RaiseEndStackGroupDragging(Group, TargetGroup);
			}
			public override void OnLeave() {
				View.WidgetViewInfo.ResetDragging();
			}
			public override void OnCancel() {
				View.WidgetViewInfo.ResetDragging();
			}
		}
		static class InfoHelper {
			public static IStackGroupInfo GetStackGroupInfo(Point point, WidgetView view) {
				foreach(StackGroup group in view.StackGroups) {
					if(group.Info.Bounds.Contains(point)) {
						return group.Info;
					}
				}
				return null;
			}
		}
	}
}
