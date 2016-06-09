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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.WindowsUI.Base {
	internal static class ControlExtensions {
		public static void StartListen(this FrameworkElement control, DependencyProperty property, string target, BindingMode mode = BindingMode.OneWay) {
			control.SetBinding(property, new Binding(target) { Source = control, Mode = mode });
		}
		public static void Forward(this FrameworkElement control, FrameworkElement target,
			DependencyProperty targetProperty, string property, BindingMode mode = BindingMode.OneWay, IValueConverter converter = null) {
			target.SetBinding(targetProperty, new Binding(property) { Source = control, Mode = mode, Converter = converter });
		}
#if SILVERLIGHT
		public static bool GoToState(this Control control, string stateName, bool useTransitions = false) {
			return VisualStateManager.GoToState(control, stateName, useTransitions);
		}
		public static T FindResource<T>(this FrameworkElement control, string name) where T:class {
			return control.Resources[name] as T;
		}
#else
		public static bool GoToState(this FrameworkElement control, string stateName, bool useTransitions = false) {
			return VisualStateManager.GoToState(control, stateName, useTransitions);
		}
		public static T FindResource<T>(this FrameworkElement control, string name) where T:class {
			return control.FindResource(name) as T;
		}
#endif
	}
#if !SILVERLIGHT
	static class TransformExtensions {
		public static Point TransformToDeviceDPI(this Visual visual, Point pt) {
			Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Point(pt.X / m.M11, pt.Y / m.M22);
		}
		public static Size TransformToDeviceDPI(this Visual visual, Size size) {
			Matrix m = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Size(size.Width / m.M11, size.Height / m.M22);
		}
	}
#endif
}
