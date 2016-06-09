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
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public enum FloatingMode {
		Adorner,
		Window,
		Popup
	}
	[ContentProperty("Content")]
	public class FloatingContainerControl : Control {
		#region static
		static Type ownerType = typeof(FloatingContainerControl);
		public static readonly DependencyProperty FloatingModeProperty = DependencyProperty.Register(
				"FloatingMode", typeof(FloatingMode), ownerType,
				new PropertyMetadata(FloatingMode.Adorner, OnFloatingModeChanged)
			);
		public static readonly DependencyProperty AdornerTemplateProperty = DependencyProperty.Register(
				"AdornerTemplate", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnTemplateChanged)
			);
		public static readonly DependencyProperty WindowTemplateProperty = DependencyProperty.Register(
				"WindowTemplate", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnTemplateChanged)
			);
		public static readonly DependencyProperty OwnerProperty = FloatingContainer.OwnerProperty.AddOwner(ownerType);
		public static readonly DependencyProperty FloatLocationProperty = DependencyProperty.Register("FloatLocation", typeof(Point), ownerType);
		public static readonly DependencyProperty FloatSizeProperty = DependencyProperty.Register("FloatSize", typeof(Size), ownerType);
		public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), ownerType);
		public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), ownerType);
		static void OnTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((FloatingContainerControl)sender).UpdateTemplate();
		}
		static void OnFloatingModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
			((FloatingContainerControl)sender).UpdateTemplate();
		}
		static FloatingContainerControl() {
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		#endregion static
		public FrameworkElement Owner {
			get { return (FrameworkElement)GetValue(OwnerProperty); }
			set { SetValue(OwnerProperty, value); }
		}
		public Point FloatLocation {
			get { return (Point)GetValue(FloatLocationProperty); }
			set { SetValue(FloatLocationProperty, value); }
		}
		public Size FloatSize {
			get { return (Size)GetValue(FloatSizeProperty); }
			set { SetValue(FloatSizeProperty, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		public FloatingMode FloatingMode {
			get { return (FloatingMode)GetValue(FloatingModeProperty); }
			set { SetValue(FloatingModeProperty, value); }
		}
		public ControlTemplate AdornerTemplate {
			get { return (ControlTemplate)GetValue(AdornerTemplateProperty); }
			set { SetValue(AdornerTemplateProperty, value); }
		}
		public ControlTemplate WindowTemplate {
			get { return (ControlTemplate)GetValue(WindowTemplateProperty); }
			set { SetValue(WindowTemplateProperty, value); }
		}
		public FloatingMode ActualFloatingMode {
			get {
				if(BrowserInteropHelper.IsBrowserHosted) return FloatingMode.Adorner;
				return FloatingMode;
			}
		}
		protected void UpdateTemplate() {
			if(Owner == null)
				Owner = (FrameworkElement)VisualTreeHelper.GetParent(this);
			switch(ActualFloatingMode) {
				case FloatingMode.Adorner: Template = AdornerTemplate; break;
				case FloatingMode.Window: Template = WindowTemplate; break;
			}
		}
	}
}
