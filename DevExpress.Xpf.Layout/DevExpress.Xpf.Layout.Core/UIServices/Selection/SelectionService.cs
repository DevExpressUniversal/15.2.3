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

namespace DevExpress.Xpf.Layout.Core.Selection {
	class SelectionService : UIService, ISelectionService {
		SelectionInfo<ILayoutElement> selectionCore;
		protected override void OnCreate() {
			selectionCore = CreateSelectionInfo();
			base.OnCreate();
		}
		protected override void OnDispose() {
			Ref.Dispose(ref selectionCore);
			base.OnDispose();
		}
		protected override System.Windows.Input.Key[] GetKeys() {
			return new System.Windows.Input.Key[] { System.Windows.Input.Key.Escape };
		}
		protected virtual SelectionInfo<ILayoutElement> CreateSelectionInfo() {
			return new SelectionInfo<ILayoutElement>(
					OnSelectionChanging, OnSelectionChanged, OnRequestSelectionRange
				);
		}
		public bool GetSelectedState(ILayoutElement element) {
			return Selection.GetSelectedState(element);
		}
		public void Select(IView view, ILayoutElement element, SelectionMode mode) {
			BeginEvent(view);
			ISelectionServiceListener listener = view.GetUIServiceListener<ISelectionServiceListener>();
			if(listener != null) {
				SelectCore(view, element, mode);
			}
			EndEvent();
		}
		public void ClearSelection(IView view) {
			BeginEvent(view);
			Selection.Select(null);
			EndEvent();
		}
		public bool ExtendSelectionToParent(IView view) {
			if(view == null || Selection.LastSelectedElement == null) return false;
			BeginEvent(view);
			bool result = ExtendSelectionToParentCore(view);
			EndEvent();
			return result;
		}
		bool ExtendSelectionToParentCore(IView view) {
			ILayoutElement parent = GetParent(view, Selection.LastSelectedElement);
			return (parent != null) && SelectCore(view, parent, SelectionMode.SingleItem);
		}
		public SelectionInfo<ILayoutElement> Selection {
			get { return selectionCore; }
		}
		ILayoutElement GetParent(IView view, ILayoutElement element) {
			if(element == null) return null;
			if(!element.IsDisposing) return element.Parent;
			ILayoutElement realElement = view.GetElement(SelectionHelper.GetElementKey(element));
			return (realElement != null) ? realElement.Parent : element;
		}
		protected override bool ProcessMouseOverride(IView view, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) {
			IViewAdapter adapter = view.Adapter;
			if(adapter != null) {
				LayoutElementHitInfo hitInfo = adapter.CalcHitInfo(view, ea.Point);
				ILayoutElement elementToSelect = hitInfo.Element;
				if(CanProcessSelection(eventType, ea, elementToSelect)) {
					ProcessSelection(view, elementToSelect);
					return true;
				}
			}
			return false;
		}
		protected override bool ProcessKeyOverride(IView view, Platform.KeyEventType evetType, System.Windows.Input.Key key) {
			if(view == null || Selection.LastSelectedElement == null) return false;
			return ExtendSelectionToParentCore(view);
		}
		protected bool CanProcessSelection(Platform.MouseEventType eventType, Platform.MouseEventArgs ea, ILayoutElement element) {
			if(element == null) return false;
			if(eventType != Platform.MouseEventType.MouseDown || ea.Buttons == Platform.MouseButtons.None) return false;
			if(ea.Buttons == Platform.MouseButtons.Right && element != null)
				return !Selection.GetSelectedState(element);
			return ea.Buttons == Platform.MouseButtons.Left;
		}
		protected void ProcessSelection(IView view, ILayoutElement element) {
			ISelectionServiceListener listener = view.GetUIServiceListener<ISelectionServiceListener>();
			if(listener != null) {
				SelectCore(view, element, listener.CheckMode(element));
			}
			else Selection.Select(null);
		}
		protected bool SelectCore(IView view, ILayoutElement target, SelectionMode mode) {
			ILayoutElementBehavior behavior = view.GetElementBehavior(target);
			if((behavior != null) && behavior.CanSelect()) {
				Selection.Mode = mode;
				Selection.Select(target);
				return true;
			}
			return false;
		}
		bool OnSelectionChanging(ILayoutElement element, bool selected) {
			IView view = GetView(element);
			if(view != null) {
				ISelectionServiceListener listener = view.GetUIServiceListener<ISelectionServiceListener>();
				if(listener != null)
					return listener.OnSelectionChanging(element, selected);
			}
			return true;
		}
		void OnSelectionChanged(ILayoutElement element, bool selected) {
			IView view = GetView(element);
			if(view != null) {
				ISelectionServiceListener listener = view.GetUIServiceListener<ISelectionServiceListener>();
				if(listener != null) 
					listener.OnSelectionChanged(element, selected);
			}
		}
		IView GetView(ILayoutElement element) {
			if(element is ISelectionKey) {
				object viewKey = SelectionHelper.GetViewKey(element);
				return (Sender != null) ? Sender.Adapter.GetView(viewKey) : null;
			}
			return (Sender != null) ? Sender.Adapter.GetView(element) : null;
		}
		ILayoutElement[] OnRequestSelectionRange(ILayoutElement first, ILayoutElement last) {
			IView view = GetView(first);
			if(view != null) {
				ISelectionServiceListener listener = view.GetUIServiceListener<ISelectionServiceListener>();
				if(listener != null)
					return listener.OnRequestSelectionRange(first, last);
			}
			return new ILayoutElement[0];
		}
	}
}
