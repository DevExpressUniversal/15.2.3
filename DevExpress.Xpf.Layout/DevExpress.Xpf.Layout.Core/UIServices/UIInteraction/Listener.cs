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
namespace DevExpress.Xpf.Layout.Core.UIInteraction {
	public class UIInteractionServiceListener : IUIInteractionServiceListener {
		public IUIServiceProvider ServiceProvider { get; set; }
		public object Key { get { return KeyOverride; } }
		protected virtual object KeyOverride {
			get { return typeof(IUIInteractionServiceListener); }
		}
		public virtual bool OnActiveItemChanging(ILayoutElement element) { return true; }
		public virtual bool OnActiveItemChanged(ILayoutElement element) { return true; }
		public virtual void OnActivate() { }
		public virtual void OnDeactivate() { }
		public virtual bool OnClickPreviewAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnClickAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnDoubleClickAction(LayoutElementHitInfo clickInfo) {
			return ToggleStateOnDoubleClick(clickInfo.Element);
		}
		public virtual bool OnMenuAction(LayoutElementHitInfo clickInfo) { return false; }
		public virtual bool OnMiddleButtonClickAction(LayoutElementHitInfo clickInfo) { return false; }
		protected bool ToggleStateOnDoubleClick(ILayoutElement element) {
			if(element == null) return false;
			bool isFloatingElement = IsFloatingElement(element);
			if((isFloatingElement && CanMaximizeOrRestore(element)) || IsMDIDocument(element)) {
				return IsMaximized(element) ? RestoreElementOnDoubleClick(element) : MaximizeElementOnDoubleClick(element);
			}
			else {
				if(IsControlItemElement(element))
					return DoControlItemDoubleClick(element);
			}
			Platform.BaseView baseView = ServiceProvider as Platform.BaseView;
			if(baseView != null) {
				bool fSuspend = isFloatingElement ? baseView.CanSuspendDocking(element) :
					baseView.CanSuspendFloating(element);
				if(fSuspend) return false;
			}
			return isFloatingElement ? DockElementOnDoubleClick(element) : CanFloatElementOnDoubleClick(element) && FloatElementOnDoubleClick(element);
		}
		protected virtual bool MaximizeElementOnDoubleClick(ILayoutElement element) { return false; }
		protected virtual bool RestoreElementOnDoubleClick(ILayoutElement element) { return false; }
		protected virtual bool DockElementOnDoubleClick(ILayoutElement element) {
			return false;
		}
		protected virtual bool DoControlItemDoubleClick(ILayoutElement element) {
			return false;
		}
		protected virtual bool FloatElementOnDoubleClick(ILayoutElement element) {
			Rect itemScreenRect = ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element);
			Rect itemContainerScreenRect = (element.Container == null) ? Rect.Empty :
				ElementHelper.GetScreenRect((ILayoutElementHost)ServiceProvider, element.Container);
			IView floatingView = GetFloatingView(element);
			if(floatingView != null) {
				floatingView.EnsureLayoutRoot();
				InitFloatingView(floatingView, itemScreenRect, itemContainerScreenRect);
			}
			return floatingView != null;
		}
		protected virtual bool CanMaximizeOrRestore(ILayoutElement element) { return false; }
		protected virtual bool IsMaximized(ILayoutElement element) { return false; }
		protected virtual bool IsFloatingElement(ILayoutElement element) { return false; }
		protected virtual bool IsControlItemElement(ILayoutElement element) { return false; }
		protected virtual bool IsMDIDocument(ILayoutElement element) { return false; }
		protected virtual IView GetFloatingView(ILayoutElement element) { return null; }
		protected virtual void InitFloatingView(IView floatingView, Rect itemScreenRect, Rect itemContainerScreenRect) { }
		protected virtual bool CanFloatElementOnDoubleClick(ILayoutElement element) { return true; }
	}
}
