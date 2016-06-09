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
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public class NavPanePopupSizeGrip : Thumb {
		FrameworkElement parent, root;
		public NavPanePopupSizeGrip() {
			this.SetDefaultStyleKey(typeof(NavPanePopupSizeGrip));
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		public override void OnApplyTemplate() {
			parent = (FrameworkElement)this.GetTemplatedParent();
			root = GetRootElement() as FrameworkElement;
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			DragDelta += OnDragDelta;
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			DragDelta -= OnDragDelta;
		}
		double GetMaxWidth() {
			var group = this.DataContext as NavBarGroup;
			if (group == null)
				return parent.MaxWidth;
			NavBarControl navBar = (group).NavBar;
			if(navBar==null) return parent.MaxWidth;
			NavigationPaneView view = navBar.View as NavigationPaneView;
			if(view==null) return parent.MaxWidth;
			return view.MaxPopupWidth;
		}
		UIElement GetRootElement() {
			return DevExpress.Xpf.Core.Native.LayoutHelper.GetRoot(this);
		}
		void OnDragDelta(object sender, DragDeltaEventArgs e) {
			UpdateParentWidth(e.HorizontalChange);
		}
		void UpdateParentWidth(double horizontalChange) {
			double desiredWidth = parent.ActualWidth + horizontalChange;
			double maxWidth = GetMaxWidth();
			if (desiredWidth <= parent.MinWidth)
				parent.Width = parent.MinWidth;
			else if (maxWidth <= desiredWidth)
				parent.Width = maxWidth;
			else
				parent.Width = desiredWidth;
		}		
	}
}
