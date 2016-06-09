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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.NavBar {
	public enum GroupPosition { First, Last, Middle, Single }
	public partial class NavBarPositionPanel : NavBarStackPanel {
		static readonly DependencyPropertyKey GroupPositionPropertyKey;
		public static readonly DependencyProperty GroupPositionProperty;
		static NavBarPositionPanel() {
			GroupPositionPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("GroupPosition", typeof(GroupPosition), typeof(NavBarPositionPanel), new FrameworkPropertyMetadata(GroupPosition.Middle));
			GroupPositionProperty = GroupPositionPropertyKey.DependencyProperty;
		}
		internal static void SetGroupPosition(DependencyObject d, GroupPosition value) {
			if(((FrameworkElement)d).DataContext is DependencyObject)
			((DependencyObject)((FrameworkElement)d).DataContext).SetValue(GroupPositionPropertyKey, value);
			d.SetValue(GroupPositionPropertyKey, value);
		}
		public static GroupPosition GetGroupPosition(DependencyObject d) {
			return (GroupPosition)d.GetValue(GroupPositionProperty);
		}
		Size OnMeasureOverride(Size availableSize) {
			UpdateChildren();
			return MeasureChildrenOverride(availableSize);
		}
		protected virtual Size MeasureChildrenOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		protected sealed override Size MeasureOverride(Size availableSize) {
			return OnMeasureOverride(availableSize);
		}
		void UpdateChildren() {
			List<UIElement> visibleChildren = new List<UIElement>();
			foreach(UIElement element in Children)
				if(UIElementHelper.IsVisible(element))
					visibleChildren.Add(element);
			for(int i = 0; i < visibleChildren.Count; i++) {
				SetGroupPosition(visibleChildren[i], GetGroupPosition(i, visibleChildren.Count));
			}
		}
		GroupPosition GetGroupPosition(int index, int count) {
			if(count == 1)
				return GroupPosition.Single;
			if(index == 0)
				return GroupPosition.First;
			if(index == count - 1)
				return GroupPosition.Last;
			return GroupPosition.Middle;
		}
	}
}
