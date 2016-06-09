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
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using DevExpress.Xpf.Data;
using System.ComponentModel;
using DevExpress.Xpf.Grid.Native;
namespace DevExpress.Xpf.Grid {
	public class CardData : RowData {
		static readonly DependencyPropertyKey CardHeaderDataPropertyKey;
		public static readonly DependencyProperty CardHeaderDataProperty;
		static readonly DependencyPropertyKey IsExpandedPropertyKey;
		public static readonly DependencyProperty IsExpandedProperty;
		static CardData() {
			CardHeaderDataPropertyKey = DependencyProperty.RegisterReadOnly("CardHeaderData", typeof(CardHeaderData), typeof(CardData), null);
			CardHeaderDataProperty = CardHeaderDataPropertyKey.DependencyProperty;
			IsExpandedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsExpanded", typeof(bool), typeof(CardData), new FrameworkPropertyMetadata(true, OnIsExpandedChanged));
			IsExpandedProperty = IsExpandedPropertyKey.DependencyProperty;
		}
		static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CardData cardData = d as CardData;
			if(cardData != null)
				cardData.RaiseContentChanged();
		}
		internal static void SetIsExpanded(DependencyObject d, bool value) {
			d.SetValue(IsExpandedPropertyKey, value);
		}
		public static bool GetIsExpanded(DependencyObject d) {
			return (bool)d.GetValue(IsExpandedProperty);
		}
		public CardData(DataTreeBuilder treeBuilder)
			: base(treeBuilder) {
			CardHeaderData = new CardHeaderData();
			BindingOperations.SetBinding(CardHeaderData, CardHeaderData.DataInternalProperty, new Binding(RowData.DataContextPropertyName) { Source = this });
			CardHeaderData.Binding = ((CardView)View).CardHeaderBinding;
			CardHeaderData.RowData = this;
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardDataCardHeaderData")]
#endif
		public CardHeaderData CardHeaderData {
			get { return (CardHeaderData)GetValue(CardHeaderDataProperty); }
			private set { SetValue(CardHeaderDataPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardDataIsExpanded")]
#endif
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			internal set { SetValue(IsExpandedPropertyKey, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedLeftCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedRightCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedNoneCellData(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		CardView CardView { get { return (CardView)View; } }
		internal void ChangeExpaned() {
			CardView.ChangeCardExpanded(RowHandle.Value);
		}
		internal void UpdateIsExpanded() {
			IsExpanded = GetCardExpanded();
		}
		bool GetCardExpanded() {
			return GetIsExpanded(RowState);
		}
		internal override void AssignFrom(RowsContainer parentRowsContainer, NodeContainer parentNodeContainer, RowNode rowNode, bool forceUpdate) {
			base.AssignFrom(parentRowsContainer, parentNodeContainer, rowNode, forceUpdate);
			UpdateIsExpanded();
		}
		protected override FrameworkElement CreateRowElement() {
			return new GridCard();
		}
	}
}
