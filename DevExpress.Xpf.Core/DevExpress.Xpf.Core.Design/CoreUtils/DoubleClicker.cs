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
using System.Windows.Input;
using System.Windows.Controls.Primitives;
#if !SLDESIGN
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
#endif
#if SL && !SLDESIGN
using Thumb = DevExpress.Xpf.Core.DragAndDrop.Thumb;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using Decorator = System.Windows.Controls.ContentControl;
#endif
#if SLDESIGN
namespace DevExpress.Xpf.Core.Design.CoreUtils {
#else
namespace DevExpress.Xpf.Core {
#endif
#if !SLDESIGN
	public class DoubleClicker : ContentControl {
		public static readonly DependencyProperty CommandParameterProperty = DependencyPropertyManager.Register("CommandParameter", typeof(object), typeof(DoubleClicker), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty CommandProperty = DependencyPropertyManager.Register("Command", typeof(ICommand), typeof(DoubleClicker), new FrameworkPropertyMetadata(null));
#if !SILVERLIGHT
		public static readonly DependencyProperty IsDoubleClickAreaProperty = DependencyProperty.RegisterAttached("IsDoubleClickArea", typeof(bool), typeof(DoubleClicker), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
		public static void SetIsDoubleClickArea(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsDoubleClickAreaProperty, value);
		}
		public static bool GetIsDoubleClickArea(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsDoubleClickAreaProperty);
		}
		protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
			if(Command != null && GetIsDoubleClickArea((DependencyObject)e.OriginalSource))
				Command.Execute(CommandParameter);
		}
#endif
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
	}
#endif
	public class MouseEventsEndPoint : Decorator {
#if !SILVERLIGHT || SLDESIGN
		protected override void OnMouseDown(MouseButtonEventArgs e) {
			base.OnMouseDown(e);
			e.Handled = true;
		}
		protected override void OnMouseUp(MouseButtonEventArgs e) {
			base.OnMouseUp(e);
			e.Handled = true;
		}
				protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			e.Handled = true;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			e.Handled = true;
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			e.Handled = true;
		}
#endif
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			e.Handled = true;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = true;
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);
			e.Handled = true;
		}
		protected override void OnMouseRightButtonUp(MouseButtonEventArgs e) {
			base.OnMouseRightButtonUp(e);
			e.Handled = true;
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e) {
			base.OnMouseWheel(e);
			e.Handled = true;
		}
	}
}
