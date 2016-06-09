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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class ScrollToVisibleAreaBehavior : Behavior<FrameworkElement> {
		ScrollViewer scrollViewer;
		protected override void OnAttached() {
			base.OnAttached();
			if(AssociatedObject.IsLoaded) {
				Initialize();
			} else {
				AssociatedObject.Loaded += Button_Loaded;
			}
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			if(scrollViewer != null) {
				scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
			}
		}
		void Button_Loaded(object sender, RoutedEventArgs e) {
			AssociatedObject.Loaded -= Button_Loaded;
			Initialize();
		}
		void Initialize() {
			scrollViewer = LayoutTreeHelper.GetVisualParents(AssociatedObject).OfType<ScrollViewer>().FirstOrDefault();
			if(scrollViewer != null) {
				scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
			}
		}
		void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
			double initialHorizontalOffset = GetInitialHorizontalOffset(AssociatedObject, scrollViewer);
			var offset = e.HorizontalOffset > initialHorizontalOffset
				? e.HorizontalOffset - initialHorizontalOffset
				: 0;
			ApplyHorizontalTranslateTransform(AssociatedObject, offset);
		}
		static double GetInitialHorizontalOffset(UIElement element, ScrollViewer scrollViewer) {
			double startOffset = scrollViewer.HorizontalOffset;
			var translateTransform = element.RenderTransform as TranslateTransform;
			if(translateTransform != null) {
				startOffset -= translateTransform.X;
			}
			double result = element.TranslatePoint(new Point(startOffset, 0), scrollViewer).X;
			return result;
		}
		static void ApplyHorizontalTranslateTransform(UIElement element, double x) {
			var transform = element.RenderTransform as TranslateTransform;
			if(transform == null) {
				transform = new TranslateTransform();
				element.RenderTransform = transform;
			}
			transform.X = x;
		}
	}
}
