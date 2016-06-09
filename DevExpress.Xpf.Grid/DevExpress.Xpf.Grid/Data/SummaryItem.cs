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
using System.ComponentModel;
using DevExpress.Data;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using System.Globalization;
using DevExpress.Data.Summary;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridSummaryItem : SummaryItemBase, IDetailElement<SummaryItemBase>, IGroupFooterSummaryItem {
		public static readonly DependencyProperty ShowInGroupColumnFooterProperty;
		static GridSummaryItem() {
			ShowInGroupColumnFooterProperty = DependencyPropertyManager.Register("ShowInGroupColumnFooter", typeof(string), typeof(GridSummaryItem), new PropertyMetadata("", (d, e) => ((GridSummaryItem)d).OnSummaryChanged(e), (d, baseValue) => baseValue == null ? string.Empty : baseValue));
		}
		[XtraSerializableProperty]
		public string ShowInGroupColumnFooter {
			get { return (string)GetValue(ShowInGroupColumnFooterProperty); }
			set { SetValue(ShowInGroupColumnFooterProperty, value); }
		}
		internal override string ActualShowInColumn { get { return string.IsNullOrEmpty(ShowInGroupColumnFooter) ? base.ActualShowInColumn : ShowInGroupColumnFooter; } }
		protected override string GetFooterDisplayFormatCore(string stringId) {
			return GridControlLocalizer.GetString(stringId);
		}
		#region IDetailElement<ColumnBase> Members
		SummaryItemBase IDetailElement<SummaryItemBase>.CreateNewInstance(params object[] args) {
			return new GridSummaryItem();
		}
		#endregion
	}
}
