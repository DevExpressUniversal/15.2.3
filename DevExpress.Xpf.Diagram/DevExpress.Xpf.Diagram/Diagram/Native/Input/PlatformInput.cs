﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Diagram.Core;
using IInputElement = DevExpress.Diagram.Core.IInputElement;
namespace DevExpress.Xpf.Diagram {
	public abstract class MouseArgsBase : IMouseArgs {
		readonly LayersHost surface;
		public MouseArgsBase(LayersHost surface) {
			this.surface = surface;
		}
		Point IMouseArgs.Position { get { return surface.GetPosition(GetPosition); } }
		ModifierKeys IMouseArgs.Modifiers { get { return ModifierKeysHelper.GetKeyboardModifiers(); } }
		void IMouseArgs.Capture() {
			surface.CaptureMouseInput();
		}
		void IMouseArgs.Release() {
			surface.ReleaseMouseInput();
		}
		protected abstract Point GetPosition(FrameworkElement relativeTo);
	}
	public class PlatformMouseArgs : MouseArgsBase {
		public PlatformMouseArgs(LayersHost surface) 
			: base(surface) {
		}
		protected override Point GetPosition(FrameworkElement relativeTo) {
			return Mouse.GetPosition(relativeTo);
		}
	}
	public class DragMouseArgs : MouseArgsBase {
		readonly DragEventArgs args;
		public DragMouseArgs(LayersHost surface, DragEventArgs args)
			: base(surface) {
			this.args = args;
		}
		protected override Point GetPosition(FrameworkElement relativeTo) {
			return args.GetPosition(relativeTo);
		}
	}
	public class PlatformMouseButtonArgs : PlatformMouseArgs, IMouseButtonArgs {
		readonly MouseButtonEventArgs args;
		public PlatformMouseButtonArgs(MouseButtonEventArgs args, LayersHost surface)
			: base(surface) {
			this.args = args;
		}
		MouseButton IMouseButtonArgs.ChangedButton { get { return args.ChangedButton; } }
		int IMouseButtonArgs.ClickCount { get { return args.ClickCount; } }
	}
	public static class RoutedEventArgsExtensions {
		public static void HandleEvent(this RoutedEventArgs e, Func<bool> handler) {
			e.Handled = handler();
		}
	}
}
