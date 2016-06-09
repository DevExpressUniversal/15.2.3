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

using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Layout.Core;
using DevExpress.Xpf.Layout.Core.Base;
namespace DevExpress.Xpf.Docking.Platform {
	public class LayoutElementFactory : UILayoutElementFactory {
		protected delegate ILayoutElement CreateInstance(
				UIElement uiElement, UIElement view
			);
		IDictionary<System.Type, CreateInstance> initializers;
		IDictionary<System.Type, CreateInstance> customInitializers;
		protected IDictionary<System.Type, CreateInstance> Initializers {
			get { return initializers; }
		}
		protected IDictionary<System.Type, CreateInstance> CustomInitializers {
			get { return customInitializers; }
		}
		public LayoutElementFactory() {
			initializers = new Dictionary<System.Type, CreateInstance>();
			customInitializers = new Dictionary<System.Type, CreateInstance>();
			InitializeFactory();
		}
		protected virtual void InitializeFactory() {
			Initializers[typeof(LayoutPanel)] = (element, view) => new DockPaneElement(element, view);
			Initializers[typeof(DocumentPanel)] = (element, view) => new DocumentElement(element, view);
			Initializers[typeof(LayoutGroup)] = (element, view) => new GroupPaneElement(element, view);
			Initializers[typeof(TabbedGroup)] = (element, view) => new TabbedPaneElement(element, view);
			Initializers[typeof(VisualElements.TabbedPaneItem)] = (element, view) => new TabbedPaneItemElement(element, view);
			Initializers[typeof(DocumentGroup)] = (element, view) => new DocumentPaneElement(element, view);
			Initializers[typeof(VisualElements.DocumentPaneItem)] = (element, view) => new DocumentPaneItemElement(element, view);
			Initializers[typeof(VisualElements.Splitter)] = (element, view) => new SplitterElement(element, view);
			Initializers[typeof(VisualElements.TabbedLayoutGroupItem)] = (element, view) => new TabbedLayoutGroupHeaderElement(element, view);
			Initializers[typeof(LayoutControlItem)] = (element, view) => new ControlItemElement(element, view);
			Initializers[typeof(LayoutSplitter)] = (element, view) => new LayoutSplitterElement(element, view);
			Initializers[typeof(EmptySpaceItem)] = (element, view) => new FixedItemElement(element, view);
			Initializers[typeof(LabelItem)] = (element, view) => new FixedItemElement(element, view);
			Initializers[typeof(SeparatorItem)] = (element, view) => new FixedItemElement(element, view);
			CustomInitializers[typeof(LayoutGroup)] = (element, view) => new TabbedLayoutGroupElement(element, view);
			CustomInitializers[typeof(DocumentPanel)] = (element, view) => new MDIDocumentElement(element, view);
		}
		protected sealed override IEnumerator<IUIElement> GetUIEnumerator(IUIElement rootKey) {
			return new Layout.Core.IUIElementEnumerator(rootKey);
		}
		protected sealed override ILayoutElement CreateElement(IUIElement uiKey) {
			UIElement uiElement = uiKey as UIElement;
			UIElement view = uiKey.GetRootUIScope() as UIElement;
			return Resolve(uiElement, view);
		}
		System.Type GetInitializerType(IDictionary<System.Type, CreateInstance> initializers, System.Type elementType) {
			System.Type type = elementType;
			while(type != null) {
				if(initializers.ContainsKey(type)) break;
				type = type.BaseType;
			}
			return type;
		}
		protected virtual bool TryGetCustomInitializer(UIElement uiElement, out CreateInstance createInstance) {
			System.Type type = uiElement.GetType();
			return CustomInitializers.TryGetValue(GetInitializerType(customInitializers, type), out createInstance);
		}
		ILayoutElement Resolve(UIElement uiElement, UIElement view) {
			AssertionException.IsNotNull(uiElement);
			AssertionException.IsNotNull(view);
			CreateInstance createInstance = null;
			System.Type type = uiElement.GetType();
			if(CanUseCustomInitializer(uiElement)) {
				if(TryGetCustomInitializer(uiElement, out createInstance))
					return createInstance(uiElement, view);
			}
			if(initializers.TryGetValue(GetInitializerType(initializers, type), out createInstance))
				return createInstance(uiElement, view);
			throw new AssertionException(string.Format("Could not resolve element for {0}.", type));
		}
		protected virtual bool CanUseCustomInitializer(UIElement uiElement) {
			LayoutGroup group = uiElement as LayoutGroup;
			if(group != null) return group.ItemType == LayoutItemType.Group && group.IsTabHost;
			DocumentPanel document = uiElement as DocumentPanel;
			if(document != null) return document.IsMDIChild;
			return false;
		}
	}
	public class FloatLayoutElementFactory : LayoutElementFactory {
		protected override void InitializeFactory() {
			base.InitializeFactory();
			Initializers[typeof(VisualElements.FloatingAdornerPresenter)] = (element, view) => new FloatPanePresenterElement(element, view);
			Initializers[typeof(VisualElements.FloatingWindowPresenter)] = (element, view) => new FloatPanePresenterElement(element, view);
			Initializers[typeof(FloatGroup)] = (element, view) => new EmptyLayoutContainer();
		}
		protected override bool CanUseCustomInitializer(UIElement uiElement) {
			DocumentPanel document = uiElement as DocumentPanel;
			return (document != null && document.IsFloatingRootItem) || base.CanUseCustomInitializer(uiElement);
		}
		protected override bool TryGetCustomInitializer(UIElement uiElement, out CreateInstance createInstance) {
			DocumentPanel document = uiElement as DocumentPanel;
			if(document != null && document.IsFloatingRootItem)
				createInstance = (element, view) => new FloatDocumentElement(element, view);
			else base.TryGetCustomInitializer(uiElement, out createInstance);
			return createInstance != null;
		}
	}
	public class AutoHideLayoutElementFactory : LayoutElementFactory {
		protected override void InitializeFactory() {
			base.InitializeFactory();
			Initializers[typeof(VisualElements.AutoHideTray)] = (element, view) => new AutoHideTrayElement(element, view);
			Initializers[typeof(VisualElements.AutoHidePane)] = (element, view) => new AutoHidePaneElement(element, view);
			Initializers[typeof(AutoHideGroup)] = (element, view) => new AutoHideTrayHeadersGroupElement(element, view);
			Initializers[typeof(VisualElements.AutoHidePaneHeaderItem)] = (element, view) => new AutoHidePaneHeaderItemElement(element, view);
		}
	}
}
