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
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Diagram {
	public class ScrollingLengthBehavior : Behavior<ScrollViewer> {
		public static readonly DependencyProperty ScrollingLengthProperty =
						DependencyProperty.Register("ScrollingLength", typeof(double), typeof(ScrollingLengthBehavior), new PropertyMetadata(0.0));
		static readonly DependencyProperty InternalActualWidthProperty =
						DependencyProperty.Register("InternalActualWidth", typeof(double), typeof(ScrollingLengthBehavior),
							new PropertyMetadata(0.0, (d, e) => ((ScrollingLengthBehavior)d).OnInternalActualWidthChanged(e)));
		static readonly DependencyProperty InternalActualContentWidthProperty =
						DependencyProperty.Register("InternalActualContentWidth", typeof(double), typeof(ScrollingLengthBehavior),
							new PropertyMetadata(0.0, (d, e) => ((ScrollingLengthBehavior)d).OnInternalActualContentWidthChanged(e)));
		public double ScrollingLength {
			get { return (double)GetValue(ScrollingLengthProperty); }
			set { SetValue(ScrollingLengthProperty, value); }
		}
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.Loaded += AssociatedObject_Loaded;
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			BindingOperations.SetBinding(this, InternalActualWidthProperty, new Binding("ActualWidth") { Source = AssociatedObject, Mode = BindingMode.OneWay });
			BindingOperations.SetBinding(this, InternalActualContentWidthProperty, new Binding("ActualWidth") { Source = (ShapeToolboxPreview)(AssociatedObject.Content), Mode = BindingMode.OneWay });
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			BindingOperations.ClearBinding(this, InternalActualWidthProperty);
			BindingOperations.ClearBinding(this, InternalActualContentWidthProperty);
		}
		void OnInternalActualWidthChanged(DependencyPropertyChangedEventArgs e) {
			SetValue(ScrollingLengthProperty, (double)GetValue(InternalActualContentWidthProperty) - (double)(e.NewValue));
		}
		void OnInternalActualContentWidthChanged(DependencyPropertyChangedEventArgs e) {
			SetValue(ScrollingLengthProperty, (double)(e.NewValue) - (double)GetValue(InternalActualWidthProperty));
		}
	}
}
