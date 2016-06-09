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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Xpf.NavBar {
	public class NavPanePopupWindowContent : ContentControl, IScrollMode {
		public NavPanePopupWindowContent() {
			DefaultStyleKey = typeof(NavPanePopupWindowContent);
			Loaded += OnLoaded;
		}		
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			if(this.DataContext as NavBarGroup == null)
				return;
			NavBarGroup group = ((NavBarGroup)this.DataContext);
			NavBarControl navBar = (group).NavBar;
			if(navBar == null) return;
			NavigationPaneView view = navBar.View as NavigationPaneView;
			if(view == null) return;			
			if(double.IsNaN(Width)) {
				return;
			}
			if(Width > view.MaxPopupWidth) Width = view.MaxPopupWidth;
		}
		ScrollControl IScrollMode.ScrollControl {
			get { return (ScrollControl)GetTemplateChild("scrollControl"); }
		}
		bool sizeValid = false;
		protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo) {
			if (SystemParameters.MenuDropAlignment) {
				var popup = Parent as NavPanePopup;
				if (popup == null)
					return;
				if (sizeInfo.PreviousSize.Width != 0)
					sizeValid = true;
				popup.UpdateHorizontalOffset();
				if (sizeValid)
					popup.HorizontalOffset += sizeInfo.NewSize.Width - sizeInfo.PreviousSize.Width;
			}
		}		
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBinding(ScrollingSettings.ScrollModeProperty, new Binding("ActualScrollMode"));
			UpdateScrollModeStates();
		}
		void UpdateScrollModeStates() {
			ScrollingSettings.UpdateScrollModeStates(this);
		}
	}
	public class NavPanePopupWindowFrame : ContentControl {
		public NavPanePopupWindowFrame() {
			DefaultStyleKey = typeof(NavPanePopupWindowFrame);
		}
	}
}
