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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
#if SL
using DXFrameworkContentElement = DevExpress.Xpf.Core.DXFrameworkElement;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class BandedViewContentSelector : Control {
		public static readonly DependencyProperty TableViewContentProperty;
		public static readonly DependencyProperty BandedViewContentProperty;
		public static readonly DependencyProperty BandsLayoutProperty;
		public static readonly DependencyProperty OwnerElementProperty;
		static BandedViewContentSelector() {
			Type ownerType = typeof(BandedViewContentSelector);
			TableViewContentProperty = DependencyProperty.Register("TableViewContent", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((BandedViewContentSelector)d).UpdateTemplate()));
			BandedViewContentProperty = DependencyProperty.Register("BandedViewContent", typeof(ControlTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((BandedViewContentSelector)d).UpdateTemplate()));
			BandsLayoutProperty = DependencyProperty.Register("BandsLayout", typeof(BandsLayoutBase), ownerType, new PropertyMetadata(null, (d, e) => ((BandedViewContentSelector)d).UpdateTemplate()));
			OwnerElementProperty = DependencyProperty.Register("OwnerElement", typeof(FrameworkElement), ownerType, new PropertyMetadata(null));
		}
		public ControlTemplate TableViewContent {
			get { return (ControlTemplate)GetValue(TableViewContentProperty); }
			set { SetValue(TableViewContentProperty, value); }
		}
		public ControlTemplate BandedViewContent {
			get { return (ControlTemplate)GetValue(BandedViewContentProperty); }
			set { SetValue(BandedViewContentProperty, value); }
		}
		public BandsLayoutBase BandsLayout {
			get { return (BandsLayoutBase)GetValue(BandsLayoutProperty); }
			set { SetValue(BandsLayoutProperty, value); }
		}
		public FrameworkElement OwnerElement {
			get { return (FrameworkElement)GetValue(OwnerElementProperty); }
			set { SetValue(OwnerElementProperty, value); }
		}
		void UpdateTemplate() {
			Template = BandsLayout == null ? TableViewContent : BandedViewContent;
		}
	}
}
