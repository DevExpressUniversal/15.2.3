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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using FlyoutControl = DevExpress.Xpf.Editors.Flyout.FlyoutControl;
namespace DevExpress.Xpf.Navigation.Internal {
	interface IItemsSourceProvider {
		DependencyProperty ItemsSourceProperty { get; }
		DependencyProperty ItemTemplateProperty { get; }
		DependencyProperty ItemTemplateSelectorProperty { get; }
		FrameworkElement Control { get; }
	}
	public class TileNavPaneContentControl : veContentControl, IFlyoutProvider {
		internal TileNavPaneContentControl() {
			DefaultStyleKey = typeof(TileNavPaneContentControl);
		}
		FlyoutControl PartFlyoutControl;
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartFlyoutControl = GetTemplateChild("PART_FlyoutControl") as FlyoutControl;
		}
		#region IFlyoutProvider Members
		FlyoutControl IFlyoutProvider.FlyoutControl {
			get { return PartFlyoutControl; }
		}
		Editors.Flyout.FlyoutPlacement IFlyoutProvider.Placement {
			get { return Editors.Flyout.FlyoutPlacement.Bottom; }
		}
		IFlyoutEventListener IFlyoutProvider.FlyoutEventListener {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		#endregion
	}
	public class TileNavPaneItemsControl : veItemsControl, IItemsSourceProvider {
		internal TileNavPaneItemsControl() {
		}
		#region IItemsSourceProvider Members
		DependencyProperty IItemsSourceProvider.ItemsSourceProperty {
			get { return ItemsSourceProperty; }
		}
		DependencyProperty IItemsSourceProvider.ItemTemplateProperty {
			get { return ItemTemplateProperty; }
		}
		DependencyProperty IItemsSourceProvider.ItemTemplateSelectorProperty {
			get { return ItemTemplateSelectorProperty; }
		}
		FrameworkElement IItemsSourceProvider.Control {
			get { return this; }
		}
		#endregion
	}
	public class TileNavPaneBar : TileBar {
		public TileNavPaneBar() {
			DefaultStyleKey = typeof(TileNavPaneBar);
			ClearValue(FlyoutManager.FlyoutManagerProperty); 
		}
	}
	public class TileNavPaneBarItemsPanel : TileBarItemsPanel {
		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			return new UIElementCollection(this, null); 
		}
		protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
			e.Handled = true;
			base.OnMouseDown(e);
		}
	}
}
