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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	[TemplatePart(Name = "PART_ButtonsPanel", Type = typeof(ButtonsPanel))]
	[TemplatePart(Name = "PART_LayoutTreeView", Type = typeof(LayoutTreeView))]
	[TemplatePart(Name = "PART_HiddenItemsPanel", Type = typeof(HiddenItemsPanel))]
	[TemplatePart(Name = "PART_OptionsPanel", Type = typeof(OptionsPanel))]
	public class CustomizationControl : psvControl, IControlHost, IUIElement {
		#region static
		static CustomizationControl() {
			var dProp = new DependencyPropertyRegistrator<CustomizationControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetDockLayoutManager(this); } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		public CustomizationControl() {
		}
		protected override void OnDispose() {
			if(PartLayoutTreeView != null) {
				PartLayoutTreeView.Dispose();
				PartLayoutTreeView = null;
			}
			base.OnDispose();
		}
		protected ButtonsPanel PartButtonsPanel { get; private set; }
		protected LayoutTreeView PartLayoutTreeView { get; private set; }
		protected HiddenItemsPanel PartHiddenItemsPanel { get; private set; }
		protected OptionsPanel PartOptionsPanel { get; private set; }
		protected DXTabItem PartTabItemLayoutTree { get; private set; }
		protected DXTabItem PartTabItemHiddenItems { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PartButtonsPanel = GetTemplateChild("PART_ButtonsPanel") as ButtonsPanel;
			PartLayoutTreeView = GetTemplateChild("PART_LayoutTreeView") as LayoutTreeView;
			PartHiddenItemsPanel = GetTemplateChild("PART_HiddenItemsPanel") as HiddenItemsPanel;
			PartOptionsPanel = GetTemplateChild("PART_OptionsPanel") as OptionsPanel;
			PartTabItemLayoutTree = GetTemplateChild("PART_TabItemLayoutTree") as DXTabItem;
			PartTabItemHiddenItems = GetTemplateChild("PART_TabItemHiddenItems") as DXTabItem;
			if(PartTabItemLayoutTree != null)
				PartTabItemLayoutTree.Header = DockingLocalizer.GetString(DockingStringId.TitleLayoutTreeView);
			if(PartTabItemHiddenItems != null)
				PartTabItemHiddenItems.Header = DockingLocalizer.GetString(DockingStringId.TitleHiddenItemsList);
			Focusable = false;
		}
		public FrameworkElement[] GetChildren() {
			return new FrameworkElement[] { PartButtonsPanel, PartLayoutTreeView, PartHiddenItemsPanel , PartOptionsPanel};
		}
	}
}
