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
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using MS.Internal.PresentationFramework;
using System.ComponentModel;
namespace DevExpress.Xpf.Core {
	public class TransferControl : ContentControl {
		public static readonly DependencyProperty CurrentContentProperty;
		public static readonly DependencyProperty PreviousContentProperty;
		public static readonly DependencyProperty ControlTemplateProperty;
		public static readonly DependencyProperty PreviousControlTemplateProperty;
		public static readonly DependencyProperty SpeedRatioProperty;
		public static readonly RoutedEvent ContentChangedEvent;
		static TransferControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TransferControl), new FrameworkPropertyMetadata(typeof(TransferControl)));
			CurrentContentProperty = DependencyProperty.Register("CurrentContent", typeof(object), typeof(TransferControl), new FrameworkPropertyMetadata(null));
			PreviousContentProperty = DependencyProperty.Register("PreviousContent", typeof(object), typeof(TransferControl), new FrameworkPropertyMetadata(null));
			ControlTemplateProperty = DependencyProperty.Register("ControlTemplate", typeof(ControlTemplate), typeof(TransferControl), new FrameworkPropertyMetadata(null));
			PreviousControlTemplateProperty = DependencyProperty.Register("PreviousControlTemplate", typeof(ControlTemplate), typeof(TransferControl), new FrameworkPropertyMetadata(null));
			SpeedRatioProperty = DependencyProperty.Register("SpeedRatio", typeof(double), typeof(TransferControl), new FrameworkPropertyMetadata(1d));
			ContentChangedEvent = EventManager.RegisterRoutedEvent("ContentChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TransferControl));
			DataObjectBase.NeedsResetEventProperty.OverrideMetadata(typeof(TransferControl), new PropertyMetadata(true));
		}
		DispatcherTimer timer = new DispatcherTimer();
		public TransferControl() {
			timer.Tick += new EventHandler(RenewAnimation);
			timer.IsEnabled = false;
			AddHandler(DataObjectBase.ResetEvent, new RoutedEventHandler(OnReset));
		}
		protected bool AnimationInProgress { get { return timer.IsEnabled; } }
		public object PreviousContent {
			get { return GetValue(PreviousContentProperty); }
			set { SetValue(PreviousContentProperty, value); }
		}
		public object CurrentContent {
			get { return GetValue(CurrentContentProperty); }
			set { SetValue(CurrentContentProperty, value); }
		}
		public ControlTemplate ControlTemplate {
			get { return (ControlTemplate)GetValue(ControlTemplateProperty); }
			set { SetValue(ControlTemplateProperty, value); }
		}
		public ControlTemplate PreviousControlTemplate {
			get { return (ControlTemplate)GetValue(PreviousControlTemplateProperty); }
			set { SetValue(PreviousControlTemplateProperty, value); }
		}
		public double SpeedRatio {
			get { return (double)GetValue(SpeedRatioProperty); }
			set { SetValue(SpeedRatioProperty, value); }
		}
		protected virtual bool SkipLongAnimations { get { return true; } }
		protected FrameworkElement ContentControl { get; set; }
		protected FrameworkElement PreviousContentControl { get; set; }
		protected object PendingContent { get; set; }
		public event RoutedEventHandler ContentChanged {
			add { this.AddHandler(ContentChangedEvent, value); }
			remove { this.RemoveHandler(ContentChangedEvent, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ContentControl = (FrameworkElement)GetTemplateChild("PART_ContentPresenter");
			PreviousContentControl = (FrameworkElement)GetTemplateChild("PART_PreviousContentPresenter");
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			OnContentChangedCore(oldContent, newContent);
		}
		protected virtual void OnContentChangedCore(object oldContent, object newContent) {
			if(AnimationInProgress) {
				PendingContent = newContent;
			} else {
				CurrentContent = newContent;
				PreviousContent = oldContent;
				TryRaiseContentChanged(newContent);
			}
		}
		protected virtual void TryRaiseContentChanged(object newContent) {
			if(PreviousContent != null && CurrentContent != null)
				RaiseContentChanged();
		}
		protected virtual void StartTimer() {
			if(!SkipLongAnimations) return;
			timer.Interval = TimeSpan.FromMilliseconds(1000d / SpeedRatio);
			timer.IsEnabled = true;
		}
		protected virtual void RaiseContentChanged() {
			if(ContentControl != null)
				ContentControl.RaiseEvent(new RoutedEventArgs(ContentChangedEvent, this));
			if(PreviousContentControl != null)
				PreviousContentControl.RaiseEvent(new RoutedEventArgs(ContentChangedEvent, this));
			StartTimer();
		}
		void RenewAnimation(object sender, EventArgs e) {
			RenewAnimationCore();
		}
		protected virtual void EndTimer() {
			timer.IsEnabled = false;
		}
		protected void RenewAnimationCore() {
			EndTimer();
			if(PendingContent != null) {
				if(CurrentContent != PendingContent) {
					OnContentChangedCore(CurrentContent, PendingContent);
				}
				PendingContent = null;
			}
			ResetPreviousContent();
		}
		protected virtual void ResetPreviousContent() {
			PreviousContent = null;
		}
		bool raisingResetEvent;
		void OnReset(object sender, RoutedEventArgs e) {
			if(raisingResetEvent)
				return;
			raisingResetEvent = true;
			try {
					RenewAnimationCore();
					timer.IsEnabled = false;
					RaiseEvent(new RoutedEventArgs() { RoutedEvent = DataObjectBase.ResetEvent });
			} finally {
				raisingResetEvent = false;
			}
		}
		protected internal virtual void OnPrevContentChanged(TransferContentControl control) {
		}
		protected internal virtual void OnCurrentContentChanged(TransferContentControl control) {
		}
	}
	public class TransferContentControl : ContentControl {
		public static readonly DependencyProperty IsPreviousContentProperty;
		static TransferContentControl() {
			EventManager.RegisterClassHandler(typeof(TransferContentControl), TransferControl.ContentChangedEvent, new RoutedEventHandler(OnContentChanged));
			TransferControl.SpeedRatioProperty.AddOwner(typeof(TransferContentControl), new FrameworkPropertyMetadata(1d));
			IsPreviousContentProperty = DependencyProperty.Register("IsPreviousContent", typeof(bool), typeof(TransferContentControl), new FrameworkPropertyMetadata(false));
		}
		static void OnContentChanged(object sender, RoutedEventArgs e) {
			((TransferContentControl)sender).OnContentChanged();
		}
		public bool IsPreviousContent {
			get { return (bool)GetValue(IsPreviousContentProperty); }
			set { SetValue(IsPreviousContentProperty, value); }
		}		
		ContentPresenter contentPresenter;
		public ContentPresenter ContentPresenter { get { return contentPresenter; } }
		public double SpeedRatio {
			get { return (double)GetValue(TransferControl.SpeedRatioProperty); }
			set { SetValue(TransferControl.SpeedRatioProperty, value); }
		}
		TransferControl transferControl;
		protected TransferControl TransferControl {
			get { return transferControl; }
			set { transferControl = value; }
		}
		protected TransferControl FindTransferControl() {
			DependencyObject node = this;
			while(node != null && !(node is TransferControl))
				node = VisualTreeHelper.GetParent(node);
			return node as TransferControl;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			contentPresenter = (ContentPresenter)GetTemplateChild("PART_ContentPresenter");
		}
		void OnContentChanged() {
			if(TransferControl == null)
				transferControl = FindTransferControl();
			if(TransferControl != null) {
				if(IsPreviousContent) 
					TransferControl.OnPrevContentChanged(this);
				else 
					TransferControl.OnCurrentContentChanged(this);
			}
			if(contentPresenter != null)
				contentPresenter.RaiseEvent(new RoutedEventArgs(TransferControl.ContentChangedEvent, this));
		}
	}
	public class NegativeConverter : MarkupExtension, IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return -System.Convert.ToDouble(value);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class SpinStyleCameraPositionConverter : MarkupExtension, IMultiValueConverter {
		#region IMultiValueConverter Members
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
				return new Point3D(0, 0, 0);
			double width = (double)values[0];
			double fieldOfView = (double)values[1];
			double zPosition = width / (2 * Math.Tan((fieldOfView * Math.PI) / 360));
			return new Point3D(0, 0, zPosition);
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
}
