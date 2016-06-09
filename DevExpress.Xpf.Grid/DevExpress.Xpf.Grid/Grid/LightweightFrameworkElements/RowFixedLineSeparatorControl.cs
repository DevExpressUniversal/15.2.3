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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public class RowFixedLineSeparatorControl : Control {
		readonly Func<TableViewBehavior, IList<ColumnBase>> getFixedColumnsFunc;
		readonly Func<BandsLayoutBase, IList<BandBase>> getFixedBandsFunc;
		public RowFixedLineSeparatorControl(Func<TableViewBehavior, IList<ColumnBase>> getFixedColumnsFunc, Func<BandsLayoutBase, IList<BandBase>> getFixedBandsFunc) {
			this.getFixedColumnsFunc = getFixedColumnsFunc;
			this.getFixedBandsFunc = getFixedBandsFunc;
		}
		internal void UpdateVisibility(DataControlBase dataControl) {
			IList list = (dataControl.BandsLayoutCore != null ? (IList)getFixedBandsFunc(dataControl.BandsLayoutCore) : (IList)getFixedColumnsFunc((TableViewBehavior)dataControl.viewCore.ViewBehavior));
			Visibility = list != null && list.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		public bool ShowVerticalLines {
			get { return (bool)GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowVerticalLinesProperty =
			DependencyProperty.Register("ShowVerticalLines", typeof(bool), typeof(RowFixedLineSeparatorControl), new PropertyMetadata(true));
		static RowFixedLineSeparatorControl() {
			Type ownerType = typeof(RowFixedLineSeparatorControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
	}
}
