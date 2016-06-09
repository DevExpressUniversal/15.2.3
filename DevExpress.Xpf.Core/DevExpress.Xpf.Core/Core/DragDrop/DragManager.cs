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
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Input;
#if SILVERLIGHT
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Core {
	public static class DragManager {
		public static readonly DependencyProperty IsStartDragPlaceProperty;
		public static readonly DependencyProperty DropTargetFactoryProperty;
		public static readonly DependencyProperty IsDraggingProperty;
		public static readonly DependencyProperty AllowMouseMoveSelectionFuncProperty;
		static DragManager() {
			IsStartDragPlaceProperty = DependencyPropertyManager.RegisterAttached("IsStartDragPlace", typeof(bool), typeof(DragManager), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
			DropTargetFactoryProperty = DependencyPropertyManager.RegisterAttached("DropTargetFactory", typeof(IDropTargetFactory), typeof(DragManager), new FrameworkPropertyMetadata(null));
			IsDraggingProperty = DependencyPropertyManager.RegisterAttached("IsDragging", typeof(bool), typeof(DragManager), new FrameworkPropertyMetadata(false));
			AllowMouseMoveSelectionFuncProperty = DependencyPropertyManager.RegisterAttached("AllowMouseMoveSelectionFunc", typeof(Func<MouseEventArgs, bool>), typeof(DragManager), new FrameworkPropertyMetadata(null));
		}
		public static void SetIsStartDragPlace(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsStartDragPlaceProperty, value);
		}
		public static bool GetIsStartDragPlace(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsStartDragPlaceProperty);
		}
		public static void SetDropTargetFactory(DependencyObject element, IDropTargetFactory value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DropTargetFactoryProperty, value);
		}
		public static IDropTargetFactory GetDropTargetFactory(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (IDropTargetFactory)element.GetValue(DropTargetFactoryProperty);
		}
		public static void SetIsDragging(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsDraggingProperty, value);
		}
		public static bool GetIsDragging(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsDraggingProperty);
		}
		public static void SetAllowMouseMoveSelectionFunc(DependencyObject element, Func<MouseEventArgs, bool> value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(AllowMouseMoveSelectionFuncProperty, value);
		}
		public static void RemoveAllowMouseMoveSelectionFunc(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.ClearValue(AllowMouseMoveSelectionFuncProperty);
		}
		public static Func<MouseEventArgs, bool> GetAllowMouseMoveSelectionFunc(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Func<MouseEventArgs, bool>)element.GetValue(AllowMouseMoveSelectionFuncProperty);
		}
	}
}
