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

extern alias Platform;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using System.Windows.Controls;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
#if SL
using Platform::DevExpress.Xpf.Core.Design;
using Platform::DevExpress.Xpf.Core.Design.SmartTags;
using Platform::DevExpress.Xpf.NavBar;
using Platform::System.Windows;
#else
using System.Windows;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.SmartTags;
#endif
namespace DevExpress.Xpf.NavBar.Design {
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	class NavBarControlSelectionAdornerProvider : SelectionAdornerProviderBase {		
		protected override void UpdateSelection(Selection newSelection) {
			base.UpdateSelection(newSelection);
			UpdateActiveGroup(newSelection.PrimarySelection);
			ShowSelectedNavBarItem(newSelection.PrimarySelection);
		}
		void ShowSelectedNavBarItem(ModelItem primarySelection) {
			ModelItem navBarItem = NavBarDesignTimeHelper.FindInParents<NavBarItem>(primarySelection);
			ModelItem navBarGroup = NavBarDesignTimeHelper.FindInParents<NavBarGroup>(navBarItem);
			if(navBarGroup == null) return;
			NavBarGroup group = (NavBarGroup)navBarGroup.GetCurrentValue();
			NavBarItem item = (NavBarItem)navBarItem.GetCurrentValue();
			group.SelectedItem = item;
		}
		void UpdateActiveGroup(ModelItem primarySelection) {
			if(primarySelection == null)
				return;
			ModelItem navBar = NavBarDesignTimeHelper.FindNavBarControl(primarySelection);
			ModelItem group = NavBarDesignTimeHelper.FindInParents<NavBarGroup>(primarySelection);
			if(navBar != null && group != null) {
				var navBarControl = navBar.GetCurrentValue() as NavBarControl;
				var navBarGroup = group.GetCurrentValue() as NavBarGroup;
				if(navBarControl != null && navBarGroup != null) {
					navBarControl.ActiveGroup = navBarGroup;
				}
			}
		}
		public override SelectionBorder CreateSelectionBorder() {
			return new NavBarSelectionBorder();
		}
	}
	class NavBarSelectionBorder : SelectionBorder {
		NavBarViewProvider ViewProvider {
			get {
				if(viewProvider == null)
					viewProvider = new NavBarViewProvider();
				return viewProvider;
			}
		}
		static NavBarViewProvider viewProvider;
		protected override FrameworkElement GetSelectedElement() {
			ModelItem navBar = NavBarDesignTimeHelper.FindNavBarControl(PrimarySelection);
			if(navBar == null)
				return null;
			return ViewProvider.ProvideView(PrimarySelection);
		}
	}
	class NavBarViewProvider : IViewProvider {
		public FrameworkElement ProvideView(ModelItem item) {
			ModelItem navBar = NavBarDesignTimeHelper.FindNavBarControl(item);
			if(navBar == null) return null;
			if(item.IsItemOfType(typeof(NavBarGroup))) {
				foreach(ViewItem view in NavBarViewItemHelper.GetViewItems<NavBarGroupControl>(navBar.View)) {
					NavBarGroupControl control = (NavBarGroupControl)view.PlatformObject;
					if(control.Group.Equals(item.GetCurrentValue()))
						return control;
				}
			} else if(item.IsItemOfType(typeof(NavBarItem))) {
				foreach(ViewItem view in NavBarViewItemHelper.GetViewItems<NavBarItemControl>(navBar.View)) {
					NavBarItemControl control = (NavBarItemControl)view.PlatformObject;
					if(control.DataContext.Equals(item.GetCurrentValue()))
						return control;
				}
			}
			return null;
		}
	}
}
