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

using System;
using System.Collections.Generic;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraBars.Docking2010.Views.NativeMdi;
using DevExpress.XtraBars.Docking2010.Views.NoDocuments;
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Dragging {
	abstract class BaseMDIElementFactory : UILayoutElementFactory {
		protected delegate ILayoutElement CreateInstance(IUIElement element, IUIElement view);
		IDictionary<System.Type, CreateInstance> initializers;
		protected IDictionary<System.Type, CreateInstance> Initializers {
			get { return initializers; }
		}
		public BaseMDIElementFactory() {
			initializers = new Dictionary<System.Type, CreateInstance>();
			InitializeFactory();
		}
		protected abstract void InitializeFactory();
		protected sealed override IEnumerator<IUIElement> GetUIEnumerator(IUIElement rootKey) {
			return new IUIElementEnumerator(rootKey);
		}
		protected sealed override ILayoutElement CreateElement(IUIElement uiKey) {
			return Resolve(uiKey, LayoutHelper.GetRootUIScope(uiKey));
		}
		ILayoutElement Resolve(IUIElement element, IUIElement view) {
			AssertionException.IsNotNull(element);
			AssertionException.IsNotNull(view);
			CreateInstance createInstance = null;
			System.Type key = GetKey(element);
			if(initializers.TryGetValue(key, out createInstance))
				return createInstance(element, view);
			throw new AssertionException(string.Format("Could not resolve element for {0}.", key));
		}
		protected virtual System.Type GetKey(IUIElement element) {
			if(element is IUIElementInfo)
				return ((IUIElementInfo)element).GetUIElementKey();
			return element.GetType();
		}
	}
	class DocumentManagerElementFactory : BaseMDIElementFactory {
		protected override void InitializeFactory() {
			Initializers[typeof(DocumentManager)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new DocumentManagerElement(element as DocumentManager); });
			Initializers[typeof(TabbedView)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new TabbedViewElement(element as TabbedView); });
			Initializers[typeof(NativeMdiView)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new NativeMdiViewElement(element as NativeMdiView); });
			Initializers[typeof(NoDocumentsView)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new NoDocumentsViewElement(element as NoDocumentsView); });
			Initializers[typeof(WidgetView)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WidgetViewElement(element as WidgetView); });
			Initializers[typeof(Views.Tabbed.IDocumentGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Tabbed.DocumentGroupInfoElement(element as Views.Tabbed.IDocumentGroupInfo); });
			Initializers[typeof(Views.Tabbed.IDocumentInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Tabbed.DocumentInfoElement(element as Views.Tabbed.IDocumentInfo); });
			Initializers[typeof(Views.Tabbed.ISplitterInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Tabbed.SplitterInfoElement(element as Views.Tabbed.ISplitterInfo); });
#if DEBUGTEST
			Initializers[typeof(Views.Tabbed.IResizeAssistentInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Tabbed.ResizeAssistentElement(element as Views.Tabbed.IResizeAssistentInfo); });
#endif
			Initializers[typeof(WindowsUIView)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUIViewElement(element as WindowsUIView); });
			Initializers[typeof(Views.WindowsUI.ITileContainerInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.TileContainerInfoElement(element as Views.WindowsUI.ITileContainerInfo); });
			Initializers[typeof(Views.WindowsUI.ITileInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.TileInfoElement(element as Views.WindowsUI.ITileInfo); });
			Initializers[typeof(Views.WindowsUI.IDocumentInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.DocumentInfoElement(element as Views.WindowsUI.IDocumentInfo); });
			Initializers[typeof(Views.WindowsUI.IPageInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.PageInfoElement(element as Views.WindowsUI.IPageInfo); });
			Initializers[typeof(Views.WindowsUI.IFlyoutInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.FlyoutInfoElement(element as Views.WindowsUI.IFlyoutInfo); });
			Initializers[typeof(Views.WindowsUI.ISplitGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.SplitGroupInfoElement(element as Views.WindowsUI.ISplitGroupInfo); });
			Initializers[typeof(Views.WindowsUI.ISlideGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.SlideGroupInfoElement(element as Views.WindowsUI.ISlideGroupInfo); });
			Initializers[typeof(Views.WindowsUI.ISlideGroupScrollBarInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.ScrollBarInfoElement(element as ISlideGroupScrollBarInfo); });
			Initializers[typeof(Views.WindowsUI.ITileContainerScrollBarInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.ScrollBarInfoElement(element as ITileContainerScrollBarInfo); });
			Initializers[typeof(Views.WindowsUI.IPageGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.PageGroupInfoElement(element as Views.WindowsUI.IPageGroupInfo); });
			Initializers[typeof(Views.WindowsUI.ITabbedGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.TabbedGroupInfoElement(element as Views.WindowsUI.ITabbedGroupInfo); });
			Initializers[typeof(Views.WindowsUI.IDetailContainerInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.DetailContainerInfoElement(element as Views.WindowsUI.IDetailContainerInfo); });
			Initializers[typeof(Views.WindowsUI.IOverviewContainerInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.OverviewContainerInfoElement(element as Views.WindowsUI.IOverviewContainerInfo); });
			Initializers[typeof(Views.WindowsUI.IContentContainerHeaderInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.ContentContainerHeaderInfoElement(element as Views.WindowsUI.IContentContainerHeaderInfo); });
			Initializers[typeof(Views.WindowsUI.ISplitterInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.SplitterInfoElement(element as Views.WindowsUI.ISplitterInfo); });
			Initializers[typeof(Views.WindowsUI.IContextActionsBarInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.ContentContainerActionsBarInfoElement(element as Views.WindowsUI.IContextActionsBarInfo); });
			Initializers[typeof(Views.WindowsUI.INavigationActionsBarInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new WindowsUI.ContentContainerActionsBarInfoElement(element as Views.WindowsUI.INavigationActionsBarInfo); });
			Initializers[typeof(Views.Widget.IDocumentInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Widget.DocumentInfoElement(element as Views.Widget.IDocumentInfo); });
			Initializers[typeof(Views.Widget.IStackGroupInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new Widget.StackGroupInfoElement(element as Views.Widget.IStackGroupInfo); });
		}
		protected override Type GetKey(IUIElement element) {
			if(element is DocumentManager)
				return typeof(DocumentManager);
			if(element is BaseView)
				return ((BaseView)element).GetUIElementKey();
			return base.GetKey(element);
		}
	}
	class FloatFormElementFactory : BaseMDIElementFactory {
		protected override void InitializeFactory() {
			Initializers[typeof(IFloatDocumentInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new FloatDocumentInfoElement(element as IFloatDocumentInfo); });
		}
	}
	class FloatPanelElementFactory : BaseMDIElementFactory {
		protected override void InitializeFactory() {
			Initializers[typeof(IFloatPanelInfo)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new FloatPanelInfoElement(element as IFloatPanelInfo); });
		}
	}
	class DocumentsHostWindowElementFactory : DocumentManagerElementFactory {
		protected override void InitializeFactory() {
			Initializers[typeof(IDocumentsHostWindowRoot)] = new CreateInstance(
				delegate(IUIElement element, IUIElement view) { return new DocumentsHostWindowElement(element as IDocumentsHostWindowRoot); });
			base.InitializeFactory();
		}
	}
}
