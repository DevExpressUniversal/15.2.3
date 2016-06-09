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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.DemoBase.Helpers {
	[TemplatePart(Name = "ContentPresenter", Type = typeof(ContentPresenter))]
	class PopupControl : ContentControl {
		#region Dependency Properties
		public static readonly DependencyProperty IsOpenProperty;
		public static readonly DependencyProperty TopOffsetProperty;
		public static readonly DependencyProperty BottomOffsetProperty;
		public static readonly DependencyProperty LeftOffsetProperty;
		public static readonly DependencyProperty RightOffsetProperty;
		static PopupControl() {
			Type ownerType = typeof(PopupControl);
			IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), ownerType, new PropertyMetadata(false,
				(d, e) => ((PopupControl)d).RaiseIsOpenChanged(e)));
			TopOffsetProperty = DependencyProperty.Register("TopOffset", typeof(double), ownerType, new PropertyMetadata(0.0));
			BottomOffsetProperty = DependencyProperty.Register("BottomOffset", typeof(double), ownerType, new PropertyMetadata(0.0));
			LeftOffsetProperty = DependencyProperty.Register("LeftOffset", typeof(double), ownerType, new PropertyMetadata(0.0));
			RightOffsetProperty = DependencyProperty.Register("RightOffset", typeof(double), ownerType, new PropertyMetadata(0.0));
		}
		#endregion
		bool popupEnabled;
		Popup popup;
		Border border;
		Grid contentContainer;
		public PopupControl() {
			this.SetDefaultStyleKey(typeof(PopupControl));
		}
		public Popup Popup {
			get { return popup; }
			set {
				if(this.popupEnabled) return;
				this.popup = value;
				EnablePopup();
			}
		}
		public bool IsOpen { get { return (bool)GetValue(IsOpenProperty); } set { SetValue(IsOpenProperty, value); } }
		public double TopOffset { get { return (double)GetValue(TopOffsetProperty); } set { SetValue(TopOffsetProperty, value); } }
		public double BottomOffset { get { return (double)GetValue(BottomOffsetProperty); } set { SetValue(BottomOffsetProperty, value); } }
		public double LeftOffset { get { return (double)GetValue(LeftOffsetProperty); } set { SetValue(LeftOffsetProperty, value); } }
		public double RightOffset { get { return (double)GetValue(RightOffsetProperty); } set { SetValue(RightOffsetProperty, value); } }
		void RaiseIsOpenChanged(DependencyPropertyChangedEventArgs e) { }
		void EnablePopup() {
			if(this.popupEnabled || Popup == null || this.contentContainer == null || this.border == null) return;
			Popup.AllowsTransparency = true;
			this.popupEnabled = true;
			this.contentContainer.SizeChanged += OnPresenterSizeChanged;
			this.border.Child = null;
			Popup.Child = this.contentContainer;
			this.border.Child = Popup;
			Popup.PlacementTarget = this.border;
			Popup.Placement = PlacementMode.Center;
			Popup.SetBinding(Popup.IsOpenProperty, new Binding("IsOpen") { Source = this, Mode = BindingMode.TwoWay });
			BindingOperations.ClearBinding(this.border, Border.VisibilityProperty);
		}
		void OnPresenterSizeChanged(object sender, SizeChangedEventArgs e) {
			if(Popup != null) {
				double width = this.contentContainer.ActualWidth;
				double height = this.contentContainer.ActualHeight;
				double h = HorizontalAlignment == HorizontalAlignment.Left ? 1.0 : HorizontalAlignment == HorizontalAlignment.Right ? -1.0 : 0.0;
				double v = VerticalAlignment == VerticalAlignment.Top ? 1.0 : VerticalAlignment == VerticalAlignment.Bottom ? -1.0 : 0.0;
				Popup.HorizontalOffset = h * Math.Floor(width / 2.0);
				Popup.VerticalOffset = v * Math.Floor(height / 2.0);
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.border = (Border)GetTemplateChild("Border");
			this.contentContainer = (Grid)GetTemplateChild("ContentContainer");
			EnablePopup();
		}
	}
	class DemoSplashScreenControl : Control {
		PopupControl popupControl;
		public static readonly DependencyProperty IsSplashScreenVisibleProperty;
		static DemoSplashScreenControl() {
			Type ownerType = typeof(DemoSplashScreenControl);
			IsSplashScreenVisibleProperty = DependencyProperty.Register("IsSplashScreenVisible", typeof(bool), ownerType, new PropertyMetadata(false, null));
		}
		public DemoSplashScreenControl() {
			DefaultStyleKey = typeof(DemoSplashScreenControl);
		}
		public bool IsSplashScreenVisible { get { return (bool)GetValue(IsSplashScreenVisibleProperty); } set { SetValue(IsSplashScreenVisibleProperty, value); } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.popupControl = (PopupControl)GetTemplateChild("PopupControl");
			if(!System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted) {
				Popup popup = new Popup() { StaysOpen = true };
				this.popupControl.Popup = popup;
			}
		}
	}
}
