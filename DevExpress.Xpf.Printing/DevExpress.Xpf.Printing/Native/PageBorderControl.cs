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

using System.Windows.Controls;
using System.Windows;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class PageBorderControl : ContentControl {
		public static readonly DependencyProperty IsPageShadowVisibleProperty = DependencyPropertyManager.Register(
			"IsPageShadowVisible",
			typeof(bool),
			typeof(PageBorderControl),
			new PropertyMetadata(true));
		public bool IsPageShadowVisible {
			get { return (bool)GetValue(IsPageShadowVisibleProperty); }
			set { SetValue(IsPageShadowVisibleProperty, value); }
		}
#if SL
		public PageBorderControl() {
			DefaultStyleKey = typeof(PageBorderControl);
		}
#else
		static PageBorderControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(PageBorderControl), new FrameworkPropertyMetadata(typeof(PageBorderControl)));
		}
		public PageBorderControl() {
			Focusable = false;
		}
#endif
	}
}
