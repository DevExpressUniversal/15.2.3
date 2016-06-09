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
using System.Windows.Controls;
using DevExpress.Xpf.Layout.Core;
using System.Windows.Data;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_ListBox", Type = typeof(ItemsControl))]
	public class HiddenItemsPanel : psvControl, IUIElement {
		#region static
		static HiddenItemsPanel() {
			var dProp = new DependencyPropertyRegistrator<HiddenItemsPanel>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		public HiddenItemsPanel() {
			IsVisibleChanged += OnIsVisibleChanged;
		}
		public DockLayoutManager Manager {
			get { return DockLayoutManager.GetDockLayoutManager(this); }
		}
		protected ItemsControl ListBox { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ListBox = LayoutItemsHelper.GetTemplateChild<ItemsControl>(this);
			CompositeCollection cc = new CompositeCollection();
			cc.Add(new CollectionContainer() { Collection = Manager.LayoutController.FixedItems });
			cc.Add(new CollectionContainer() { Collection = Manager.LayoutController.HiddenItems });
			ListBox.ItemsSource = cc;
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(Manager == null) return;
			IUIElement viewUIElement = ((IUIElement)this).GetRootUIScope();
			IView view = Manager.GetView(viewUIElement);
			if(view != null) view.Invalidate();
		}
	}
}
