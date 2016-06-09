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
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
#if !SL
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Interop;
#else
using DevExpress.XpfToSLUtils;
#endif
#if SL
using DependencyPropertyChangedEventArgs = System.Windows.SLDependencyPropertyChangedEventArgs;
using Popup = DevExpress.Xpf.Core.SLPopup;
using PropertyChangedCallback = System.Windows.SLPropertyChangedCallback;
#endif
#if !SL
namespace DevExpress.Xpf.Core {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class ThemesPopup : Popup { }
	public partial class PopupBase : Popup {
		#region static
		public static readonly DependencyProperty PopupContentProperty;
		static PopupBase() {
			PopupContentProperty = DependencyPropertyManager.Register("PopupContent", typeof(object), typeof(PopupBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnPopupContentPropertyChanged)));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupBase), new FrameworkPropertyMetadata(typeof(PopupBase)));
#endif
		}
		protected static void OnPopupContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PopupBase)d).OnPopupContentChanged(e);
		}
		#endregion
		public PopupBase() {
		}
		protected virtual void Initialize() {
			PopupBorderControl control = CreateBorderControl();
#if !SILVERLIGHT
			if(control != null && BrowserInteropHelper.IsBrowserHosted)
				control.Margin = new Thickness();
#endif
			control.Popup = this;
			Child = control;
		}
		protected virtual PopupBorderControl CreateBorderControl() {
			return new PopupBorderControl();
		}
		public object PopupContent {
			get { return GetValue(PopupContentProperty); }
			set { SetValue(PopupContentProperty, value); }
		}
		protected void OnPopupContentChanged(DependencyPropertyChangedEventArgs e) {
			if(Child == null)
				Initialize();
			PopupContentChangedInternal((UIElement)e.OldValue);
		}
		protected virtual void PopupContentChangedInternal(UIElement oldValue) {
			PopupBorderControl control = Child as PopupBorderControl;
			if(control != null)
				control.Content = PopupContent;
		}
	}
	public class PopupBorderControl : ContentControl {
		#region static
		public static readonly DependencyProperty ContentWidthProperty =
			DependencyProperty.Register("ContentWidth", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.WidthProperty.DefaultMetadata.DefaultValue));
		public static readonly DependencyProperty ContentMinWidthProperty =
			DependencyProperty.Register("ContentMinWidth", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.MinWidthProperty.DefaultMetadata.DefaultValue));
		public static readonly DependencyProperty ContentMaxWidthProperty =
			DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.MaxWidthProperty.DefaultMetadata.DefaultValue));
		public static readonly DependencyProperty ContentHeightProperty =
			DependencyProperty.Register("ContentHeight", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.HeightProperty.DefaultMetadata.DefaultValue));
		public static readonly DependencyProperty ContentMinHeightProperty =
			DependencyProperty.Register("ContentMinHeight", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.MinHeightProperty.DefaultMetadata.DefaultValue));
		public static readonly DependencyProperty ContentMaxHeightProperty =
			DependencyProperty.Register("ContentMaxHeight", typeof(double), typeof(PopupBorderControl), new UIPropertyMetadata(FrameworkElement.MaxHeightProperty.DefaultMetadata.DefaultValue));
		public double ContentWidth {
			get { return (double)GetValue(ContentWidthProperty); }
			set { SetValue(ContentWidthProperty, value); }
		}
		public double ContentMinWidth {
			get { return (double)GetValue(ContentMinWidthProperty); }
			set { SetValue(ContentMinWidthProperty, value); }
		}
		public double ContentMaxWidth {
			get { return (double)GetValue(ContentMaxWidthProperty); }
			set { SetValue(ContentMaxWidthProperty, value); }
		}
		public double ContentHeight {
			get { return (double)GetValue(ContentHeightProperty); }
			set { SetValue(ContentHeightProperty, value); }
		}
		public double ContentMinHeight {
			get { return (double)GetValue(ContentMinHeightProperty); }
			set { SetValue(ContentMinHeightProperty, value); }
		}
		public double ContentMaxHeight {
			get { return (double)GetValue(ContentMaxHeightProperty); }
			set { SetValue(ContentMaxHeightProperty, value); }
		}
		public static readonly DependencyProperty PopupProperty;
		static PopupBorderControl() {
			PopupProperty = DependencyPropertyManager.Register("Popup", typeof(Popup), typeof(PopupBorderControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
#if !SL
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupBorderControl), new FrameworkPropertyMetadata(typeof(PopupBorderControl)));
#endif
		}
		#endregion
#if SL
		public PopupBorderControl() {
			DefaultStyleKey = typeof(PopupBorderControl);
		}
#endif
		public PopupBorderControl() {
			Loaded += new RoutedEventHandler(OnLoaded);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState();
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
			VisualStateManager.GoToState(this, BrowserInteropHelper.IsBrowserHosted ? "BrowserHosted" : "Standalone", false);
		}
		public Popup Popup {
			get { return (Popup)GetValue(PopupProperty); }
			set { SetValue(PopupProperty, value); }
		}
	}
}
