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
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNameNavigationAndLayout)]
	public class ScrollBox : LayoutControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty LeftProperty =
			DependencyProperty.RegisterAttached("Left", typeof(double), typeof(ScrollBox), new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty TopProperty =
			DependencyProperty.RegisterAttached("Top", typeof(double), typeof(ScrollBox), new PropertyMetadata(OnAttachedPropertyChanged));
		public static double GetLeft(UIElement element) {
			return (double)element.GetValue(LeftProperty);
		}
		public static void SetLeft(UIElement element, double value) {
			element.SetValue(LeftProperty, value);
		}
		public static double GetTop(UIElement element) {
			return (double)element.GetValue(TopProperty);
		}
		public static void SetTop(UIElement element, double value) {
			element.SetValue(TopProperty, value);
		}
		#endregion Dependency Properties
		#region Layout
		protected override LayoutProviderBase CreateLayoutProvider() {
			return new ScrollBoxLayoutProvider(this);
		}
		#endregion Layout
		public ScrollBox() {
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if(property == LeftProperty || property == TopProperty)
				Changed();
		}
	}
	public class ScrollBoxLayoutProvider : LayoutProviderBase {
		public ScrollBoxLayoutProvider(ILayoutControlBase control)
			: base(control) {
		}
		public override void UpdateChildBounds(FrameworkElement child, ref Rect bounds) {
			base.UpdateChildBounds(child, ref bounds);
			RectHelper.IncLeft(ref bounds, -ScrollBox.GetLeft(child));
			RectHelper.IncTop(ref bounds, -ScrollBox.GetTop(child));
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			maxSize = SizeHelper.Infinite;
			var result = new Size(0, 0);
			foreach(var item in items) {
				item.Measure(maxSize);
				var itemSize = item.DesiredSize;
				itemSize.Width += ScrollBox.GetLeft(item);
				itemSize.Height += ScrollBox.GetTop(item);
				SizeHelper.UpdateMaxSize(ref result, UIElementExtensions.GetRoundedSize(itemSize));
			}
			return result;
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			foreach(var item in items) {
				var itemBounds = RectHelper.New(item.DesiredSize);
				itemBounds.X = bounds.Left + ScrollBox.GetLeft(item);
				itemBounds.Y = bounds.Top + ScrollBox.GetTop(item);
				item.Arrange(itemBounds);
			}
			return base.OnArrange(items, bounds, viewPortBounds);
		}
	}
}
