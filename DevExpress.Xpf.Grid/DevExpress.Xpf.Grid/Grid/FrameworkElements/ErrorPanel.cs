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
using DevExpress.Xpf.Grid.HitTest;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class ErrorPanel : Control {
		public ErrorPanel(){
			this.SetDefaultStyleKey(typeof(ErrorPanel));
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}
		FrameworkElement root;
		FrameworkElement Root {
			get {
				return root;
			}
			set {
				if(root == value)
					return;
				if(root != null)
					root.RemoveHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(InternalOnMouseLeftButtonDown));
				root = value;
				if(root != null)
					root.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(InternalOnMouseLeftButtonDown), true);
			}
		}
		public void OnLoaded(object sender, RoutedEventArgs e) {
			Root = GetTopLevelVisual();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			Root = null;
		}
		internal void InternalOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Visibility = Visibility.Collapsed;
		}
		FrameworkElement GetTopLevelVisual() {
			FrameworkElement root = LayoutHelper.GetTopLevelVisual(this) as FrameworkElement;
			if(root is Popup)
				return ((Popup)root).Child as FrameworkElement;
			return root;
		}
	}
}
