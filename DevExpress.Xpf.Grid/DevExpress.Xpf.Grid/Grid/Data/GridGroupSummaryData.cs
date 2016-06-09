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
using DevExpress.Xpf.Data;
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using System.Collections.Generic;
using System.Windows.Data;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridGroupSummaryData : GridColumnData {
		static readonly DependencyPropertyKey SummaryItemPropertyKey;
		public static readonly DependencyProperty SummaryItemProperty;
		static readonly DependencyPropertyKey SummaryValuePropertyKey;
		public static readonly DependencyProperty SummaryValueProperty;
		static readonly DependencyPropertyKey TextPropertyKey;
		public static readonly DependencyProperty TextProperty;
		static readonly DependencyPropertyKey IsLastPropertyKey;
		public static readonly DependencyProperty IsLastProperty;
		static readonly DependencyPropertyKey IsFirstPropertyKey;
		public static readonly DependencyProperty IsFirstProperty;
		static GridGroupSummaryData() {
			SummaryItemPropertyKey = DependencyPropertyManager.RegisterReadOnly("SummaryItem", typeof(GridSummaryItem), typeof(GridGroupSummaryData), new PropertyMetadata(null, OnContentChanged));
			SummaryItemProperty = SummaryItemPropertyKey.DependencyProperty;
			SummaryValuePropertyKey = DependencyPropertyManager.RegisterReadOnly("SummaryValue", typeof(object), typeof(GridGroupSummaryData), new PropertyMetadata(null, OnContentChanged));
			SummaryValueProperty = SummaryValuePropertyKey.DependencyProperty;
			TextPropertyKey = DependencyPropertyManager.RegisterReadOnly("Text", typeof(string), typeof(GridGroupSummaryData), new PropertyMetadata(null, (d, e) => ((GridGroupSummaryData)d).OnTextAffectingPropertyChanged()));
			TextProperty = TextPropertyKey.DependencyProperty;
			IsLastPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsLast", typeof(bool), typeof(GridGroupSummaryData), new PropertyMetadata(false, (d, e) => ((GridGroupSummaryData)d).OnTextAffectingPropertyChanged()));
			IsLastProperty = IsLastPropertyKey.DependencyProperty;
			IsFirstPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFirst", typeof(bool), typeof(GridGroupSummaryData), new PropertyMetadata(false, OnContentChanged));
			IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummaryDataSummaryItem")]
#endif
		public GridSummaryItem SummaryItem {
			get { return (GridSummaryItem)GetValue(SummaryItemProperty); }
			internal set { this.SetValue(SummaryItemPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummaryDataSummaryValue")]
#endif
		public object SummaryValue {
			get { return GetValue(SummaryValueProperty); }
			internal set { this.SetValue(SummaryValuePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummaryDataText")]
#endif
		public string Text {
			get { return (string)GetValue(TextProperty); }
			internal set { this.SetValue(TextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummaryDataIsLast")]
#endif
		public bool IsLast {
			get { return (bool)GetValue(IsLastProperty); }
			internal set { this.SetValue(IsLastPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridGroupSummaryDataIsFirst")]
#endif
		public bool IsFirst {
			get { return (bool)GetValue(IsFirstProperty); }
			internal set { this.SetValue(IsFirstPropertyKey, value); }
		}
		public GridGroupSummaryData(ColumnsRowDataBase rowData)
			: base(rowData) {
		}
		IGroupValueClient client = null;
		internal void SetGroupValueClient(IGroupValueClient client) {
			this.client = client;
		}
		void OnTextAffectingPropertyChanged() {
			OnContentChanged();
			if(client != null)
				client.UpdateText();
		}
	}
	public class GridGroupSummaryColumnData : GridColumnData {
		static readonly DependencyPropertyKey HasSummaryPropertyKey;
		public static readonly DependencyProperty HasSummaryProperty;
		static readonly DependencyPropertyKey HasRightSiblingPropertyKey;
		public static readonly DependencyProperty HasRightSiblingProperty;
		static GridGroupSummaryColumnData() {
			HasRightSiblingPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasRightSibling", typeof(bool), typeof(GridGroupSummaryColumnData), new PropertyMetadata(null));
			HasRightSiblingProperty = HasRightSiblingPropertyKey.DependencyProperty;
			HasSummaryPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasSummary", typeof(bool), typeof(GridGroupSummaryColumnData), new PropertyMetadata((d, e) => ((GridGroupSummaryColumnData)d).OnHasSummaryChanged()));
			HasSummaryProperty = HasSummaryPropertyKey.DependencyProperty;
		}
		public bool HasRightSibling {
			get { return (bool)GetValue(HasRightSiblingProperty); }
			internal set { this.SetValue(HasRightSiblingPropertyKey, value); }
		}
		public bool HasSummary {
			get { return (bool)GetValue(HasSummaryProperty); }
			protected internal set { this.SetValue(HasSummaryPropertyKey, value); }
		}
		public GroupRowData GroupRowData { get { return (GroupRowData)base.RowDataBase; } }
		public GridGroupSummaryColumnData(GroupRowData rowData)
			: base(rowData) {
		}
		IGroupRowColumnSummaryClient columnSummaryClient;
		internal void SetColumnSummaryClient(IGroupRowColumnSummaryClient client) {
			columnSummaryClient = client;
		}
		internal void UpdateSummaryClientFocusState() {
			if(columnSummaryClient != null)
				columnSummaryClient.UpdateFocusState();
		}
		internal void UpdateSummaryIsReady() {
			if(columnSummaryClient != null)
				columnSummaryClient.UpdateIsReady();
		}
		void OnHasSummaryChanged() {
			if(columnSummaryClient != null)
				columnSummaryClient.UpdateHasSummary();
		}
		protected override void OnValueChanged(object oldValue) {
			base.OnValueChanged(oldValue);
			if(columnSummaryClient != null)
				columnSummaryClient.UpdateSummaryValue();
		}
	}
}
