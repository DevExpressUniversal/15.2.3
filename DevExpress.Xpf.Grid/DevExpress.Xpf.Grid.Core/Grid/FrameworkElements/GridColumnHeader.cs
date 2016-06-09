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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using Thumb = DevExpress.Xpf.Core.DXThumb;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	[DXToolboxBrowsable(false)]
	public class ColumnHeaderFilterControl : ContentControl {
		public static readonly DependencyProperty ColumnFilterPopupProperty;
		static ColumnHeaderFilterControl() {
			ColumnFilterPopupProperty = DependencyPropertyManager.RegisterAttached("ColumnFilterPopup", typeof(PopupBaseEdit), typeof(ColumnHeaderFilterControl), new PropertyMetadata(null, OnColumnFilterPopupChanged));
		}
		static void OnColumnFilterPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ColumnHeaderFilterControl)d).OnColumnFilterPopupChanged();
		}
		public PopupBaseEdit ColumnFilterPopup {
			get { return (PopupBaseEdit)GetValue(ColumnFilterPopupProperty); }
			set { this.SetValue(ColumnFilterPopupProperty, value); }
		}
		public ColumnHeaderFilterControl() {
			Loaded +=new RoutedEventHandler(ColumnHeaderFilterControl_Loaded);
			Unloaded+=new RoutedEventHandler(ColumnHeaderFilterControl_Unloaded);
		}
		void ColumnHeaderFilterControl_Loaded(object sender, RoutedEventArgs e) {
			Content = ColumnFilterPopup;
		}
		void ColumnHeaderFilterControl_Unloaded(object sender, RoutedEventArgs e) {
			Content = null;
		}
		void OnColumnFilterPopupChanged() {
#if SL
			ColumnFilterPopup.RemoveFromVisualTree();
#endif
			Content = ColumnFilterPopup;
		}
	}
	public class ColumnHeaderSortIndicatorControl : Control { }
}
