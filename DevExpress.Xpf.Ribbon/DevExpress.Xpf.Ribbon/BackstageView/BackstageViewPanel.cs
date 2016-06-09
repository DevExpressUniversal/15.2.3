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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class BackstageViewPanel : Panel {
		#region static
		public static readonly DependencyProperty TabPaneProperty;
		public static readonly DependencyProperty FirstControlPaneProperty;
		public static readonly DependencyProperty SecondControlPaneProperty;
		static BackstageViewPanel() {
			TabPaneProperty = DependencyPropertyManager.Register("TabPane", typeof(UIElement), typeof(BackstageViewPanel),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabPanePropertyChanged)));
			FirstControlPaneProperty = DependencyPropertyManager.Register("FirstControlPane", typeof(UIElement), typeof(BackstageViewPanel),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFirstControlPanePropertyChanged)));
			SecondControlPaneProperty = DependencyPropertyManager.Register("SecondControlPane", typeof(UIElement), typeof(BackstageViewPanel),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSecondControlPanePropertyChanged)));
		}
		protected static void OnTabPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewPanel)d).OnTabPaneChanged(e.OldValue as UIElement);			
		}
		protected static void OnFirstControlPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewPanel)d).OnFirstControlPaneChanged(e.OldValue as UIElement);
		}
		protected static void OnSecondControlPanePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BackstageViewPanel)d).OnSecondControlPaneChanged(e.OldValue as UIElement);
		}
		#endregion        
		#region dep props
		public UIElement TabPane {
			get { return (UIElement)GetValue(TabPaneProperty); }
			set { SetValue(TabPaneProperty, value); }
		}
		public UIElement FirstControlPane {
			get { return (UIElement)GetValue(FirstControlPaneProperty); }
			set { SetValue(FirstControlPaneProperty, value); }
		}
		public UIElement SecondControlPane {
			get { return (UIElement)GetValue(SecondControlPaneProperty); }
			set { SetValue(SecondControlPaneProperty, value); }
		}
		#endregion
		protected virtual void OnTabPaneChanged(UIElement oldValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(TabPane != null)
				Children.Add(TabPane);
		}
		protected virtual void OnFirstControlPaneChanged(UIElement oldValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(FirstControlPane != null)
				Children.Add(FirstControlPane);
		}
		protected virtual void OnSecondControlPaneChanged(UIElement oldValue) {
			if(oldValue != null)
				Children.Remove(oldValue);
			if(SecondControlPane != null)
				Children.Add(SecondControlPane);
		}
		protected override Size MeasureOverride(Size availableSize) {
			Size sz = new Size();
			if(TabPane != null) {
				TabPane.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				sz.Width = TabPane.DesiredSize.Width;
				sz.Height = TabPane.DesiredSize.Height;
			}
			if(FirstControlPane != null && FirstControlPane.Visibility != Visibility.Collapsed) {
				FirstControlPane.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				sz.Width += FirstControlPane.DesiredSize.Width;
				sz.Height = Math.Max(sz.Height, FirstControlPane.DesiredSize.Height);
			}
			if(SecondControlPane != null && SecondControlPane.Visibility != Visibility.Collapsed) {
				SecondControlPane.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				sz.Width += SecondControlPane.DesiredSize.Width;
				sz.Height = Math.Max(sz.Height, SecondControlPane.DesiredSize.Height);
			}
			return sz;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect pos = new Rect();
			double h = 0;
			if(TabPane != null) {
				pos.Width = TabPane.DesiredSize.Width;
				pos.Height = Math.Max(TabPane.DesiredSize.Height, finalSize.Height);
				TabPane.Arrange(pos);
				pos.X += pos.Width;
				h = pos.Height;
			}
			if(FirstControlPane != null && FirstControlPane.Visibility != Visibility.Collapsed) {
				pos.Width = FirstControlPane.DesiredSize.Width;
				pos.Height = Math.Max(FirstControlPane.DesiredSize.Height, finalSize.Height);
				FirstControlPane.Arrange(pos);
				pos.X += pos.Width;
				h = Math.Max(pos.Height, h);
			}
			if(SecondControlPane != null && SecondControlPane.Visibility != Visibility.Collapsed) {
				pos.Width = SecondControlPane.DesiredSize.Width;
				pos.Height = Math.Max(SecondControlPane.DesiredSize.Height, finalSize.Height);
				SecondControlPane.Arrange(pos);
				pos.X += pos.Width;
				h = Math.Max(pos.Height, h);
			}
			return finalSize;
		}
	}	
}
