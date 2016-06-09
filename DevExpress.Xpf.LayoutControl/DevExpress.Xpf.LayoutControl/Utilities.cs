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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
#if !SILVERLIGHT
using System.Windows.Threading;
#endif
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	public class ElementBoundsAnimation {
		#region Dependency Properties
		protected static readonly DependencyProperty ClipRectWidthProperty =
			DependencyProperty.RegisterAttached("ClipRectWidth", typeof(double), typeof(ElementBoundsAnimation),
				new PropertyMetadata(OnClipRectSizeChanged));
		protected static readonly DependencyProperty ClipRectHeightProperty =
			DependencyProperty.RegisterAttached("ClipRectHeight", typeof(double), typeof(ElementBoundsAnimation),
				new PropertyMetadata(OnClipRectSizeChanged));
		private static void OnClipRectSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			if (o.HasDefaultValue(e.Property))
				return;
			var element = o as UIElement;
			if (element == null)
				return;
			var clip = element.Clip as RectangleGeometry;
			if (clip == null)
				return;
			Rect clipRect = clip.Rect;
			if (e.Property == ClipRectWidthProperty)
				clipRect.Width = (double)e.NewValue;
			else
				clipRect.Height = (double)e.NewValue;
			clip.Rect = clipRect;
		}
		#endregion Dependency Properties
		public ElementBoundsAnimation(FrameworkElements elements) {
			Elements = elements;
		}
		public bool Begin(TimeSpan duration, IEasingFunction easingFunction = null, Action onCompleted = null) {
			if (IsActive)
				return false;
			Storyboard = new Storyboard();
			foreach (FrameworkElement element in OldElementBounds.Keys) {
				Rect oldBounds = OldElementBounds[element];
				Rect newBounds = NewElementBounds[element];
				AnimateElementPosition(Storyboard, element, oldBounds.Location(), newBounds.Location());
				AnimateElementSize(Storyboard, element, oldBounds.Size(), newBounds.Size());
			}
			if (Storyboard.Children.Count == 0) {
				Storyboard = null;
				if (onCompleted != null)
					onCompleted();
				return false;
			}
			foreach (DoubleAnimation animation in Storyboard.Children) {
				animation.Duration = duration;
				animation.EasingFunction = easingFunction;
			}
			StoryboardCompleted = delegate {
				Storyboard.Stop();
				foreach (FrameworkElement element in OldElementBounds.Keys) {
					if (element.IsPropertyAssigned(UIElement.RenderTransformProperty))
						element.ClearValue(UIElement.RenderTransformProperty);
					if (element.IsPropertyAssigned(UIElement.ClipProperty))
						element.ClearValue(UIElement.ClipProperty);
				}
				StoryboardCompleted = null;
				Storyboard = null;
				if (onCompleted != null)
					onCompleted();
			};
			Storyboard.Completed += delegate {
				if (StoryboardCompleted != null)
					StoryboardCompleted();
			};
#if SILVERLIGHT
			Storyboard.Begin();
#else
			Storyboard.Begin();
#endif
			return true;
		}
		public void Stop() {
			if (!IsActive)
				return;
			Storyboard.SkipToFill();
			StoryboardCompleted();
		}
		public void StoreNewElementBounds(FrameworkElement relativeTo = null) {
			NewElementBounds = GetElementBounds(relativeTo);
		}
		public void StoreOldElementBounds(FrameworkElement relativeTo = null) {
			OldElementBounds = GetElementBounds(relativeTo);
		}
		public bool IsActive { get { return Storyboard != null; } }
		protected void AnimateElementPosition(Storyboard storyboard, FrameworkElement element, Point oldPosition, Point newPosition) {
			if (newPosition == oldPosition)
				return;
			element.RenderTransform = new TranslateTransform();
			storyboard.Children.Add(CreateDoubleAnimation(element, "(RenderTransform).X", oldPosition.X - newPosition.X, 0));
			storyboard.Children.Add(CreateDoubleAnimation(element, "(RenderTransform).Y", oldPosition.Y - newPosition.Y, 0));
		}
		protected void AnimateElementSize(Storyboard storyboard, FrameworkElement element, Size oldSize, Size newSize) {
			if (newSize == oldSize)
				return;
			if (newSize.Width < oldSize.Width || newSize.Height < oldSize.Height)
				return;
			element.Clip = new RectangleGeometry { Rect = oldSize.ToRect() };
			storyboard.Children.Add(CreateDoubleAnimation(element, ClipRectWidthProperty, oldSize.Width, newSize.Width));
			storyboard.Children.Add(CreateDoubleAnimation(element, ClipRectHeightProperty, oldSize.Height, newSize.Height));
		}
		protected DoubleAnimation CreateDoubleAnimation(FrameworkElement element, object property, double from, double to) {
			var result = new DoubleAnimation();
			Storyboard.SetTarget(result, element);
			Storyboard.SetTargetProperty(result, property is string ? new PropertyPath((string)property) : new PropertyPath(property));
			result.From = from;
			result.To = to;
			return result;
		}
		protected ElementBounds GetElementBounds(FrameworkElement relativeTo = null) {
			var result = new ElementBounds();
			foreach (FrameworkElement element in Elements)
				result.Add(element, relativeTo == null ? element.GetBounds() : element.GetBounds(relativeTo));
			return result;
		}
		protected FrameworkElements Elements { get; private set; }
		protected ElementBounds NewElementBounds { get; private set; }
		protected ElementBounds OldElementBounds { get; private set; }
		protected Storyboard Storyboard { get; private set; }
		protected Action StoryboardCompleted { get; private set; }
		protected class ElementBounds : Dictionary<FrameworkElement, Rect> { }
	}
	public class Resources {
		private static LocalizationRes _Strings = new LocalizationRes();
		public LocalizationRes Strings { get { return _Strings; } }
	}
}
