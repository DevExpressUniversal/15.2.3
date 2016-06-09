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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Utils;
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Printing {
	public class RowContent : DependencyObject {
		public static readonly DependencyProperty IsEvenProperty = DependencyPropertyManager.Register("IsEven", typeof(bool), typeof(RowContent), new PropertyMetadata(false));
		public static readonly DependencyProperty UsablePageWidthProperty = DependencyPropertyManager.Register("UsablePageWidth", typeof(double), typeof(RowContent), new PropertyMetadata(0d));
		public static readonly DependencyProperty UsablePageHeightProperty = DependencyPropertyManager.Register("UsablePageHeight", typeof(double), typeof(RowContent), new UIPropertyMetadata(0d));
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(RowContent), new PropertyMetadata((object)null));
		public bool IsEven {
			get { return (bool)GetValue(IsEvenProperty); }
			set { SetValue(IsEvenProperty, value); }
		}
		public double UsablePageWidth {
			get { return (double)GetValue(UsablePageWidthProperty); }
			set { SetValue(UsablePageWidthProperty, value); }
		}
		public double UsablePageHeight {
			get { return (double)GetValue(UsablePageHeightProperty); }
			set { SetValue(UsablePageHeightProperty, value); }
		}
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
}
