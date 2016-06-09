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
using DevExpress.Utils.Controls;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Dragging.Tabbed;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	class DocumentManagerUIViewUIInteractionListener : UIInteractionServiceListener {
		public DocumentManagerUIView View {
			get { return ServiceProvider as DocumentManagerUIView; }
		}
		public override bool OnActiveItemChanging(ILayoutElement element) {
			return ReactionHelper.React(element, CanActivateDocument);
		}
		public override bool OnActiveItemChanged(ILayoutElement element) {
			return ReactionHelper.React(element, ActivateDocument);
		}
		public override bool OnMenuAction(LayoutElementHitInfo clickInfo) {
			return ReactionHelper.React(clickInfo.Element, ShowDocumentContextMenu, ShowViewContextMenu, clickInfo.HitPoint);
		}
		public override bool OnMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			IDocumentGroupInfo info = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(info != null) {
				bool handled = View.Adapter.UIInteractionService.SuspendTabMouseActivation ||
					View.Adapter.UIInteractionService.ValidationCancelled;
				info.ProcessEvent(EventType.MouseDown, GetArgs(buttons, hitInfo.HitPoint, handled));
			}
			return false;
		}
		public override bool OnMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			IDocumentGroupInfo info = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(info != null)
				info.ProcessEvent(EventType.MouseUp, GetArgs(buttons, hitInfo.HitPoint));
			return false;
		}
		public override void OnMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			IDocumentGroupInfo info = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(info != null)
				info.ProcessEvent(EventType.MouseMove, GetArgs(buttons, hitInfo.HitPoint));
		}
		public override void OnMouseLeave(LayoutElementHitInfo hitInfo) {
			IDocumentGroupInfo info = InfoHelper.GetDocumentGroupInfo(hitInfo.Element);
			if(info != null)
				info.ProcessEvent(EventType.MouseLeave, GetArgs(hitInfo.HitPoint));
		}
		protected override bool FloatElementOnDoubleClick(ILayoutElement element) {
			IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
			bool result = false;
			if(info != null) {
				BaseView owner = info.Owner;
				BaseDocument document = info.BaseDocument;
				result = info.BaseDocument.CanFloat(owner) && base.FloatElementOnDoubleClick(element);
				if(result) {
					if(owner != null && document != null)
						owner.OnEndFloating(document, EndFloatingReason.DoubleClick);
				}
			}
			return result;
		}
		protected virtual bool CanActivateDocument(BaseDocument document) {
			return document.CanActivate();
		}
		protected virtual bool ActivateDocument(BaseDocument document) {
			return View.Manager.View.Controller.Activate(document);
		}
		protected virtual bool ShowViewContextMenu(Point point) {
			return View.Manager.View.Controller.ShowContextMenu(point);
		}
		protected virtual bool ShowDocumentContextMenu(BaseDocument document, Point point) {
			return View.Manager.View.Controller.ShowContextMenu(document, point);
		}
		FloatingHelper helper;
		protected override IUIView GetFloatingView(ILayoutElement element) {
			helper = new FloatingHelper(View, element);
			return helper.GetFloatingView();
		}
		protected override void InitFloatingView(IUIView floatingView, Rectangle itemScreenRect, Rectangle itemContainerScreenRect) {
			itemContainerScreenRect.Offset(View.Manager.GetOffsetNC());
			helper.InitFloatingView(floatingView, itemContainerScreenRect);
		}
		static class ReactionHelper {
			public delegate bool Reaction<T>(T parameter);
			public delegate bool Reaction<T1, T2>(T1 parameter1, T2 parameter2);
			public static bool React(ILayoutElement element, Reaction<BaseDocument> reaction) {
				IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
				if(info != null)
					return reaction(info.BaseDocument);
				return false;
			}
			public static bool React<T>(ILayoutElement element, Reaction<BaseDocument, T> reaction, Reaction<T> commonReaction, T parameter) {
				IBaseDocumentInfo info = InfoHelper.GetBaseDocumentInfo(element);
				if(info != null)
					return reaction(info.BaseDocument, parameter);
				return commonReaction(parameter);
			}
		}
	}
#if DEBUGTEST
	class DocumentManagerUIViewUIInteractionListenerForResizeAssistent : DocumentManagerUIViewUIInteractionListener, IUIServiceListener {
		public DocumentManagerUIViewUIInteractionListenerForResizeAssistent() { }
		public override void OnMouseMove(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			base.OnMouseMove(buttons, hitInfo);
			IDocumentLayoutElement element = hitInfo.Element as IDocumentLayoutElement;
			IResizeAssistentInfo elementInfo = element.GetElementInfo() as IResizeAssistentInfo;
			elementInfo.Show(View.Manager.Adorner);
		}
		public override void OnMouseLeave(LayoutElementHitInfo hitInfo) {
			base.OnMouseLeave(hitInfo);
			IDocumentLayoutElement element = hitInfo.Element as IDocumentLayoutElement;
			IResizeAssistentInfo elementInfo = element.GetElementInfo() as IResizeAssistentInfo;
			elementInfo.Hide(View.Manager.Adorner);
		}
		public override bool OnMouseDown(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			IDocumentLayoutElement element = hitInfo.Element as IDocumentLayoutElement;
			IResizeAssistentInfo elementInfo = element.GetElementInfo() as IResizeAssistentInfo;
			elementInfo.ChangeContainersResizeZoneType();
			return base.OnMouseDown(buttons, hitInfo);
		}
		public override bool OnMouseUp(MouseButtons buttons, LayoutElementHitInfo hitInfo) {
			return base.OnMouseUp(buttons, hitInfo);
		}
		object IUIServiceListener.Key {
			get { return typeof(ResizeAssistentElement); }
		}
	}
#endif
}
