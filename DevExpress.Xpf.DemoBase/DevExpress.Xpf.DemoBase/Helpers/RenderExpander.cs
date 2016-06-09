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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase.Helpers {
	[ContentProperty("Child")]
	class RenderExpander : LayoutPanel {
		#region Dependency Properties
		public static readonly DependencyProperty ExpandFactorProperty;
		public static readonly DependencyProperty ChildProperty;
		public static readonly DependencyProperty ExpandSideProperty;
		static RenderExpander() {
			Type ownerType = typeof(RenderExpander);
			ExpandFactorProperty = DependencyProperty.Register("ExpandFactor", typeof(double), ownerType, new PropertyMetadata(1.0, RaiseExpandFactorChanged));
			ChildProperty = DependencyProperty.Register("Child", typeof(UIElement), ownerType, new PropertyMetadata(null, RaiseChildChanged));
			ExpandSideProperty = DependencyProperty.Register("ExpandSide", typeof(Side), ownerType, new PropertyMetadata(Side.Right, RaiseExpandSideChanged));
		}
		double expandFactorValue = 1.0;
		UIElement childValue = null;
		Side expandSideValue = Side.Right;
		static void RaiseExpandFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RenderExpander)d).expandFactorValue = (double)e.NewValue;
			((RenderExpander)d).RaiseExpandFactorChanged(e);
		}
		static void RaiseChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RenderExpander)d).childValue = (UIElement)e.NewValue;
			((RenderExpander)d).RaiseChildChanged(e);
		}
		static void RaiseExpandSideChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RenderExpander)d).expandSideValue = (Side)e.NewValue;
		}
		#endregion
		RectClass childRect = new RectClass();
		public double ExpandFactor { get { return expandFactorValue; } set { SetValue(ExpandFactorProperty, value); } }
		public UIElement Child { get { return childValue; } set { SetValue(ChildProperty, value); } }
		public Side ExpandSide { get { return expandSideValue; } set { SetValue(ExpandSideProperty, value); } }
		protected override Size MeasureOverride(Size availableSize) {
			childRect.Size = availableSize;
			childRect.Location = new Point();
			MeasureChild(childRect, Child, true, true);
			return childRect.Size;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			childRect.Size = finalSize;
			ArrangeChild(childRect, Child);
			Clip = new RectangleGeometry() { Rect = new Rect(childRect.Location, childRect.Size) };
			UpdateTranslation();
			return finalSize;
		}
		void UpdateTranslation() {
			double factor = (1.0 - ExpandFactor);
			Point translate;
			switch(ExpandSide) {
				case Side.Left: translate = new Point(-factor * childRect.Width, 0.0); break;
				case Side.Top: translate = new Point(0.0, -factor * childRect.Height); break;
				case Side.Bottom: translate = new Point(0.0, factor * childRect.Height); break;
				default: translate = new Point(factor * childRect.Width, 0.0); break;
			}
			SetTranslate(Child, translate.X, translate.Y);
		}
		Storyboard GetExpandStoryboard() {
			Storyboard sb = new Storyboard();
			DoubleAnimation a = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500)) };
			Storyboard.SetTargetProperty(a, new PropertyPath("ExpandFactor"));
			sb.Children.Add(a);
			return sb;
		}
		Storyboard GetCollapseStoryboard() {
			Storyboard sb = new Storyboard();
			DoubleAnimation a = new DoubleAnimation() { From = 1.0, To = 0.0, Duration = new Duration(new TimeSpan(0, 0, 0, 0, 500)) };
			Storyboard.SetTargetProperty(a, new PropertyPath("ExpandFactor"));
			sb.Children.Add(a);
			return sb;
		}
		void RaiseExpandFactorChanged(DependencyPropertyChangedEventArgs e) {
			UpdateTranslation();
			if(ExpandFactor < 0.001 && (double)e.OldValue >= 0.001)
				UpdateIsEnabled(false);
			else if(ExpandFactor > 0.001 && (double)e.OldValue < 0.001)
				UpdateIsEnabled(true);
		}
		void UpdateIsEnabled(bool value) {
			ContentControl control = Child as ContentControl;
			if(control == null) return;
			control.IsEnabled = value;
		}
		void RaiseChildChanged(DependencyPropertyChangedEventArgs e) { UpdateChild(e.OldValue, e.NewValue, 1); }
	}
}
