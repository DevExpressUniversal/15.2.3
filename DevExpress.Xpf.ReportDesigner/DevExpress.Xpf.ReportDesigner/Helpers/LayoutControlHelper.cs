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

using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class LayoutControlHelper {
		public static readonly DependencyProperty WidthSourceProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty OnSizeChangedProperty;
		static LayoutControlHelper() {
			DependencyPropertyRegistrator<LayoutControlHelper>.New()
				.RegisterAttached((FrameworkElement d) => GetWidthSource(d), out WidthSourceProperty, null, OnWidthSourceChanged)
				.RegisterAttached((FrameworkElement d) => GetOnSizeChanged(d), out OnSizeChangedProperty, null)
			;
		}
		public static void SetWidthSource(FrameworkElement element, FrameworkElement value) {
			element.SetValue(WidthSourceProperty, value);
		}
		public static FrameworkElement GetWidthSource(FrameworkElement element) {
			return (FrameworkElement)element.GetValue(WidthSourceProperty);
		}
		static void SetOnSizeChanged(FrameworkElement element, SizeChangedEventHandler value) {
			element.SetValue(OnSizeChangedProperty, value);
		}
		static SizeChangedEventHandler GetOnSizeChanged(FrameworkElement element) {
			return (SizeChangedEventHandler)element.GetValue(OnSizeChangedProperty);
		}
		static void OnWidthSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var target = (FrameworkElement)d;
			var oldValue = (FrameworkElement)e.OldValue;
			var newValue = (FrameworkElement)e.NewValue;
			if(oldValue != null) {
				SizeChangedEventHandler onSizeChanged = GetOnSizeChanged(target);
				oldValue.SizeChanged -= onSizeChanged;
			}
			if(newValue == null) {
				SetOnSizeChanged(target, null);
			} else {
				SizeChangedEventHandler onSizeChanged = (s, args) => {
					target.Width = newValue.ActualWidth;
				};
				SetOnSizeChanged(target, onSizeChanged);
				newValue.SizeChanged += onSizeChanged;
				onSizeChanged(null, null);
			}
		}
	}
}
