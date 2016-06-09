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

extern alias Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Guard = Platform::DevExpress.Utils.Guard;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public class ThePropertyChangedEventArgs<T> : EventArgs {
		public ThePropertyChangedEventArgs(DependencyPropertyChangedEventArgs e) : this((T)e.OldValue, (T)e.NewValue) { }
		public ThePropertyChangedEventArgs(T oldValue, T newValue) {
			OldValue = oldValue;
			NewValue = newValue;
		}
		public T OldValue { get; private set; }
		public T NewValue { get; private set; }
	}
	public static class ImageSourceUriHelper {
		#region Dependency Properties
		public static readonly DependencyProperty UriSourceProperty =
			DependencyPropertyManager.RegisterAttached("UriSource", typeof(Uri), typeof(ImageSourceUriHelper), new PropertyMetadata(null,
				OnUriSourceChanged));
		static readonly DependencyProperty UriSourceDataProperty =
			DependencyProperty.RegisterAttached("UriSourceData", typeof(Func<Uri, Thread>), typeof(ImageSourceUriHelper), new PropertyMetadata(null));
		#endregion
		public static Uri GetUriSource(Image image) { return (Uri)image.GetValue(UriSourceProperty); }
		public static void SetUriSource(Image image, Uri source) { image.SetValue(UriSourceProperty, source); }
		static void OnUriSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Image image = (Image)d;
			Func<Uri, Thread> setSource = (Func<Uri, Thread>)d.GetValue(UriSourceDataProperty);
			if(setSource == null) {
				setSource = AsyncHelper.Create<Uri>((u, c) => SetSource(image, u, c));
				d.SetValue(UriSourceDataProperty, setSource);
			}
			setSource((Uri)e.NewValue);
		}
		static IEnumerable<ManualResetEvent> SetSource(Image image, Uri uri, CancellationToken cancellationToken) {
			return new ManualResetEvent[] { AsyncHelper.DoWithDispatcher(image.Dispatcher, () => image.Source = ImageSourceHelper.GetImageSource(uri)) };
		}
	}
	internal class DoubleClickHelper {
		public event MouseButtonEventHandler DoubleClick;
		public bool IsClicked { get; private set; }
		System.Windows.Threading.DispatcherTimer timer;
		public DoubleClickHelper(FrameworkElement owner) {
			this.IsClicked = false;
			owner.MouseLeftButtonUp += OnOwnerMouseLeftButtonUp;
			timer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(System.Windows.Forms.SystemInformation.DoubleClickTime) };
			timer.Tick += (o, e) => Reset();
		}
		object olock = new object();
		void OnOwnerMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			lock(olock) {
				if(IsClicked) {
					IsClicked = false;
					timer.Stop();
					OnDoubleClick(sender, e);
				} else {
					IsClicked = true;
					timer.Start();
				}
			}
		}
		void Reset() {
			timer.Stop();
			lock(olock) {
				IsClicked = false;
			}
		}
		private void OnDoubleClick(object sender, MouseButtonEventArgs e) {
			if(DoubleClick != null) {
				DoubleClick(sender, e);
			}
		}
	}
	public class HorizontalGridPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			if(Children.Count == 0) return new Size();
			Size childSize = new Size(availableSize.Width / Children.Count, availableSize.Height);
			double height = 0.0;
			double width = 0.0;
			foreach(UIElement child in Children) {
				child.Measure(childSize);
				height = Math.Max(height, child.DesiredSize.Height);
				width += child.DesiredSize.Width;
			}
			return new Size(width, height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(Children.Count == 0) return new Size();
			Size childSize = new Size(finalSize.Width / Children.Count, finalSize.Height);
			double x = 0.0;
			foreach(UIElement child in Children) {
				child.Arrange(new Rect(new Point(x, 0.0), childSize));
				x += childSize.Width;
			}
			return finalSize;
		}
	}
}
