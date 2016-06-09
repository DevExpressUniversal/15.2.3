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
#if !SL
using DevExpress.Xpf.Utils;
#else
using StackPanel = DevExpress.Xpf.Core.DXStackPanel;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Core {
	public enum StackPanelElementPosition { First, Middle, Last, Single };
	public class PositionStackPanel : StackPanel {
		public static readonly DependencyProperty PositionProperty;
		public static readonly DependencyProperty IndexProperty;
		static PositionStackPanel() {
			PositionProperty = DependencyPropertyManager.RegisterAttached("Position", typeof(StackPanelElementPosition), typeof(PositionStackPanel), new FrameworkPropertyMetadata(StackPanelElementPosition.Middle));
			IndexProperty = DependencyPropertyManager.RegisterAttached("Index", typeof(int), typeof(PositionStackPanel), new FrameworkPropertyMetadata(-1));
		}
		public static void SetPosition(DependencyObject element, StackPanelElementPosition value) {
			element.SetValue(PositionProperty, value);
		}
		public static StackPanelElementPosition GetPosition(DependencyObject element) {
			return (StackPanelElementPosition)element.GetValue(PositionProperty);
		}
		public static void SetIndex(DependencyObject element, int value) {
			element.SetValue(IndexProperty, value);
		}
		public static int GetIndex(DependencyObject element) {
			return (int)element.GetValue(IndexProperty);
		}
		void UpdateChildrenProperties() {
			if (Children.Count == 0)
				return;
			for (int index = 0; index < Children.Count; index++) {
				Children[index].SetValue(PositionStackPanel.PositionProperty, CalculatePosition(index, Children.Count));
				Children[index].SetValue(PositionStackPanel.IndexProperty, index);
			}
		}
		public static StackPanelElementPosition CalculatePosition(int index, int count) {
			if (count == 1)
				return StackPanelElementPosition.Single;
			if (index == 0)
				return StackPanelElementPosition.First;
			if (index == count - 1)
				return StackPanelElementPosition.Last;
			return StackPanelElementPosition.Middle;
		}
#if SL
		Size previousSize;
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size size = base.ArrangeOverride(arrangeSize);
			if (previousSize != size) {
				UpdateChildrenProperties();
				previousSize = size;
			}
			return size;
		}
#else
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size size = base.ArrangeOverride(arrangeSize);
			UpdateChildrenProperties();
			return size;
		}
#endif
	}
}
