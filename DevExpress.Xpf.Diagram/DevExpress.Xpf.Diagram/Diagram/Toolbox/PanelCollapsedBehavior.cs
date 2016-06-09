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
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
namespace DevExpress.Xpf.Diagram {
	public class PanelCollapsedBehavior : Behavior<LayoutPanel> {
		double defaultCompactWidth = 75;
		double normalWidthMultiplier = 1.2;
		bool sizeChangeEnabled = true;
		bool buttonChangeEnabled = true;
		double savedWidth = 0;
		bool isResizing = false;
		double normalWidth = 0;
		public static readonly DependencyProperty IsCompactProperty =
			DependencyProperty.Register("IsCompact", typeof(bool), typeof(PanelCollapsedBehavior), new PropertyMetadata(false));
		public static readonly DependencyProperty ButtonCheckedProperty =
			DependencyProperty.Register("ButtonChecked", typeof(bool), typeof(PanelCollapsedBehavior), new PropertyMetadata(false, (d, e) => ((PanelCollapsedBehavior)d).OnButtonCheckedChanged(e)));
		public static readonly DependencyProperty LayoutErrorProperty =
			DependencyProperty.Register("LayoutError", typeof(double), typeof(PanelCollapsedBehavior), new PropertyMetadata(0.0, (d, e) => ((PanelCollapsedBehavior)d).OnLayoutErrorChanged(e)));
		public bool IsCompact {
			get { return (bool)GetValue(IsCompactProperty); }
			set { SetValue(IsCompactProperty, value); }
		}
		public bool ButtonChecked {
			get { return (bool)GetValue(ButtonCheckedProperty); }
			set { SetValue(ButtonCheckedProperty, value); }
		}
		public double LayoutError {
			get { return (double)GetValue(LayoutErrorProperty); }
			set { SetValue(LayoutErrorProperty, value); }
		}
		public double InitialCompactWidth { get; set; }
		protected override void OnAttached() {
			base.OnAttached();
			if (InitialCompactWidth == 0)
				InitialCompactWidth = defaultCompactWidth;
			SetDefaultNormalWidth();
			AssociatedObject.SizeChanged += LayoutPanel_SizeChanged;
			AssociatedObject.Loaded += AssociatedObject_Loaded;
		}
		void AssociatedObject_Loaded(object sender, RoutedEventArgs e) {
			DockLayoutManager manager = this.AssociatedObject.GetDockLayoutManager();
			manager.DockOperationStarting -= DockOperationStarting;
			manager.DockOperationCompleted -= DockOperationCompleted;
			manager.DockOperationStarting += DockOperationStarting;
			manager.DockOperationCompleted += DockOperationCompleted;
			CoercePanelWidth();
			savedWidth = AssociatedObject.ActualWidth;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.SizeChanged -= LayoutPanel_SizeChanged;
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
			DockLayoutManager manager = this.AssociatedObject.GetDockLayoutManager();
			manager.DockOperationStarting -= DockOperationStarting;
			manager.DockOperationCompleted -= DockOperationCompleted;
		}
		void LayoutPanel_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (sizeChangeEnabled) {
				if (!IsCompact && isResizing && e.NewSize.Width < InitialCompactWidth) {
					IsCompact = true;
					buttonChangeEnabled = false;
					ButtonChecked = true;
					SetDefaultNormalWidth();
				}
				if (IsCompact && isResizing && e.NewSize.Width > normalWidth) {
					IsCompact = false;
					buttonChangeEnabled = false;
					ButtonChecked = false;
				}
			} else
				sizeChangeEnabled = true;
		}
		void OnButtonCheckedChanged(DependencyPropertyChangedEventArgs e) {
			if (buttonChangeEnabled) {
				if ((bool)e.NewValue) {
					if (!this.IsInDesignMode()) {
						savedWidth = AssociatedObject.ActualWidth;
						AssociatedObject.ItemWidth = new GridLength(InitialCompactWidth);
					}
					sizeChangeEnabled = false;
					IsCompact = true;
					SetDefaultNormalWidth();
				} else {
					sizeChangeEnabled = false;
					if (savedWidth < defaultCompactWidth * normalWidthMultiplier)
						savedWidth = 3 * defaultCompactWidth;
					if (!this.IsInDesignMode())
						AssociatedObject.ItemWidth = new GridLength(savedWidth);
					IsCompact = false;
				}
			} else
				buttonChangeEnabled = true;
		}
		void OnLayoutErrorChanged(DependencyPropertyChangedEventArgs e) {
			CoercePanelWidth();
		}
		void DockOperationStarting(object sender, DockOperationStartingEventArgs e) {
			if (e.DockOperation != DockOperation.Resize || e.Item != AssociatedObject) return;
			isResizing = true;
			Debug.WriteLine(isResizing);
		}
		void DockOperationCompleted(object sender, DockOperationCompletedEventArgs e) {
			if (e.DockOperation != DockOperation.Resize || e.Item != AssociatedObject) return;
			isResizing = false;
			if (IsCompact)
				normalWidth = normalWidth < InitialCompactWidth * 2 ? AssociatedObject.ActualWidth * normalWidthMultiplier : InitialCompactWidth * 2;
		}
		void CoercePanelWidth() {
			if (this.IsInDesignMode())
				return;
			double newWidth = AssociatedObject.ActualWidth + LayoutError;
			AssociatedObject.ItemWidth = new GridLength(newWidth);
			if (IsCompact && normalWidth < newWidth * normalWidthMultiplier && normalWidth < InitialCompactWidth * 2)
				normalWidth = newWidth * normalWidthMultiplier;
		}
		void SetDefaultNormalWidth() {
			normalWidth = InitialCompactWidth * normalWidthMultiplier;
		}
	}
}
