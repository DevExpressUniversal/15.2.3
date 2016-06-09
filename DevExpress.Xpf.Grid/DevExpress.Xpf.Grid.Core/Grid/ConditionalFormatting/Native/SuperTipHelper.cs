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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
namespace DevExpress.Xpf.Grid.Native {
	public static class SuperTipHelper {
		public static object GetSuperTipHeader(DependencyObject obj) {
			return (object)obj.GetValue(SuperTipHeaderProperty);
		}
		public static void SetSuperTipHeader(DependencyObject obj, object value) {
			obj.SetValue(SuperTipHeaderProperty, value);
		}
		public static readonly DependencyProperty SuperTipHeaderProperty =
			DependencyProperty.RegisterAttached("SuperTipHeader", typeof(object), typeof(SuperTipHelper), new PropertyMetadata(null, OnSuperTipHeaderChanged));
		public static object GetSuperTipContent(DependencyObject obj) {
			return (object)obj.GetValue(SuperTipContentProperty);
		}
		public static void SetSuperTipContent(DependencyObject obj, object value) {
			obj.SetValue(SuperTipContentProperty, value);
		}
		public static readonly DependencyProperty SuperTipContentProperty =
			DependencyProperty.RegisterAttached("SuperTipContent", typeof(object), typeof(SuperTipHelper), new PropertyMetadata(null, OnSuperTipContentChanged));
		static void OnSuperTipContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var tip = GetSuperTip(d);
			if(tip.Items.LastOrDefault() != null && tip.Items.LastOrDefault().GetType() == typeof(SuperTipItem))
				tip.Items.Remove(tip.Items.LastOrDefault());
			if(e.NewValue != null)
				tip.Items.Add(new SuperTipItem() { Content = e.NewValue });
		}
		static void OnSuperTipHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			var tip = GetSuperTip(d);
			if(tip.Items.FirstOrDefault() != null && tip.Items.FirstOrDefault().GetType() == typeof(SuperTipHeaderItem))
				tip.Items.Remove(tip.Items.First());
			if(e.NewValue != null)
				tip.Items.Insert(0, new SuperTipHeaderItem() { Content = e.NewValue });
		}
		static SuperTip GetSuperTip(DependencyObject d) {
			GalleryItem item = (GalleryItem)d;
			if(item.SuperTip == null)
				item.SuperTip = new SuperTip();
			return item.SuperTip;
		}
	}
}
