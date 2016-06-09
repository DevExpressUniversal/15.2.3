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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.NavBar {
	public class NavBarStackPanel : StackPanel {
		const double DesignTimeDefaultSecondarySize = 100d;
		SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		protected double CalcSecondarySize() {
			double secondary = 0;
			foreach(FrameworkElement element in Children)
				secondary = Math.Max(secondary, SizeHelper.GetSecondarySize(element.DesiredSize));
			return secondary;
		}
		protected double CalcDesiredSize() {
			double define = 0;
			foreach(FrameworkElement element in Children)
				define += SizeHelper.GetDefineSize(element.DesiredSize);
			return define;
		}
	}
	public class NavPaneItemsControlPanel : NavBarPositionPanel {
		#region static
		public static readonly DependencyProperty VisibleElementCountProperty;
		static NavPaneItemsControlPanel() {
			VisibleElementCountProperty = NavPaneItemsControl.VisibleElementCountProperty.AddOwner((typeof(NavPaneItemsControlPanel)), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnVisibleElementCountChanged)));
		}
		protected static void OnVisibleElementCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavPaneItemsControlPanel)d).OnVisibleElementCountChanged(e);
		}
		#endregion
		public int VisibleElementCount {
			get { return (int)GetValue(VisibleElementCountProperty); }
			set { SetValue(VisibleElementCountProperty, value); }
		}
		protected NavPaneItemsControl Owner { get; private set; }
		protected override Size MeasureChildrenOverride(Size availableSize) {
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(Orientation);
			double current = 0;
			int elementsCount = 0;
			foreach(FrameworkElement element in Children) {
				NavBarGroup group = element.DataContext as NavBarGroup;
				element.Measure(sizeHelper.CreateSize(double.PositiveInfinity, sizeHelper.GetSecondarySize(availableSize)));
				if (current + sizeHelper.GetDefineSize(element.DesiredSize) < sizeHelper.GetDefineSize(availableSize)) {
					current += sizeHelper.GetDefineSize(element.DesiredSize);
					elementsCount++;
					if(group!=null) group.IsVisibleInOverflowPanel = true;
				} else if (group != null) group.IsVisibleInOverflowPanel = false;
			}
			VisibleElementCount = elementsCount;
			return sizeHelper.CreateSize(current, CalcSecondarySize());
		}
		protected virtual void OnVisibleElementCountChanged(DependencyPropertyChangedEventArgs e) {
			if(Owner == null) {
				Owner = LayoutHelper.FindParentObject<NavPaneItemsControl>(this);
			}
			if(Owner != null)
				Owner.VisibleElementCount = VisibleElementCount;
		}
	}
}
