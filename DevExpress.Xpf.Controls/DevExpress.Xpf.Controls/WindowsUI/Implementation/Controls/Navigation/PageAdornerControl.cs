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
using DevExpress.Xpf.WindowsUI.Base;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.WindowsUI {
#if !SILVERLIGHT
#endif
	[DXToolboxBrowsable]
	public class PageAdornerControl : veHeaderedContentControl {
		#region static
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty ShowBackButtonProperty;
		public static readonly DependencyProperty BackCommandParameterProperty;
		static PageAdornerControl() {
			var dProp = new DependencyPropertyRegistrator<PageAdornerControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("BackCommand", ref BackCommandProperty, (ICommand)null);
			dProp.Register("ShowBackButton", ref ShowBackButtonProperty, true);
			dProp.Register("BackCommandParameter", ref BackCommandParameterProperty, (object)null);
		}
		#endregion
		public PageAdornerControl() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(PageAdornerControl);
#endif
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public bool ShowBackButton {
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}
		public object BackCommandParameter {
			get { return GetValue(BackCommandParameterProperty); }
			set { SetValue(BackCommandParameterProperty, value); }
		}
	}
}
