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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Internal;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Flyout;
using System.Windows.Media;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using System;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.Xpf.DocumentViewer;
namespace DevExpress.Xpf.Printing {
	public class PreviewPageControl : PageControl {
		public static readonly DependencyProperty PageProperty;
		public PageViewModel Page {
			get { return (PageViewModel)GetValue(PageProperty); }
			set { SetValue(PageProperty, value); }
		}
		#region ctor
		static PreviewPageControl() {
			PageProperty = DependencyPropertyManager.Register("Page", typeof(PageViewModel), typeof(PreviewPageControl), new FrameworkPropertyMetadata(null));
		}
		public PreviewPageControl() {
			DefaultStyleKey = typeof(PreviewPageControl);
			Focusable = false;
		}
		#endregion
		protected override DependencyObject GetContainerForItemOverride() {
			return new PreviewPageControlItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PreviewPageControlItem;
		}
	}
	public class PreviewPageControlItem : PageControlItem {
		public PreviewPageControlItem() {
			DefaultStyleKey = typeof(PreviewPageControlItem);
		}
		protected override void OnIsCoverPageChanged(bool newValue) {
			UpdateMargin(DataContext != null ? ((IPage)DataContext).Margin : new Thickness(0));
		}
		protected override void OnPositionChanged(PageControlItemPosition newValue) {
			UpdateMargin(DataContext != null ? ((IPage)DataContext).Margin : new Thickness(0));
		}
		protected override void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			IPage page = e.NewValue as IPage;
			UpdateMargin(page != null ? page.Margin : new Thickness(0));
		}
		void UpdateMargin(Thickness margin) {
				Margin = margin;
		}
	}
}
