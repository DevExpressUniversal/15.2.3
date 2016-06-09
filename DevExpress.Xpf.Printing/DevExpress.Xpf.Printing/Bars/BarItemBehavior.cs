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
using DevExpress.Xpf.Bars;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using System.Windows.Threading;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing {
	public static class BarItemBehavior {
		public static readonly DependencyProperty SendZoomToModelOnClickProperty =
			DependencyPropertyManager.RegisterAttached("SendZoomToModelOnClick", typeof(PreviewModelBase), typeof(BarItemBehavior), new PropertyMetadata(null, OnSendZoomToModelOnClickChanged));
		public static PreviewModelBase GetSendZoomToModelOnClick(DependencyObject obj) {
			return (PreviewModelBase)obj.GetValue(SendZoomToModelOnClickProperty);
		}
		public static void SetSendZoomToModelOnClick(DependencyObject obj, PreviewModelBase value) {
			obj.SetValue(SendZoomToModelOnClickProperty, value);
		}
		static void OnSendZoomToModelOnClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarButtonItem barButtonItem = d as BarButtonItem;
			if (barButtonItem == null)
				throw new NotSupportedException();
			PreviewModelBase oldModel = e.OldValue as PreviewModelBase;
			if (oldModel != null) {
				barButtonItem.ItemClick -= barButtonItem_ItemClick;
			}
			PreviewModelBase newModel = e.NewValue as PreviewModelBase;
			if (newModel != null) {
				barButtonItem.ItemClick += barButtonItem_ItemClick;
			}
		}
		static void barButtonItem_ItemClick(object sender, ItemClickEventArgs e) {
			BarButtonItem barButtonItem = (BarButtonItem)e.Item;
			PreviewModelBase model = BarItemBehavior.GetSendZoomToModelOnClick(barButtonItem);
			if (model == null)
				throw new InvalidProgramException();
			model.ZoomMode = (ZoomItemBase)barButtonItem.CommandParameter;
		}
	}
}
