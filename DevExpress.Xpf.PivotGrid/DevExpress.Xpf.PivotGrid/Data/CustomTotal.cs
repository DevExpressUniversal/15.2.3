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
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
#if SL
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DXFrameworkContentElement = System.Windows.FrameworkElement;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class PivotGridInternalCustomTotal : PivotGridCustomTotalBase {
		PivotGridCustomTotal wrapper;
		public PivotGridInternalCustomTotal()
			: base() { }
		public PivotGridCustomTotal Wrapper {
			get { return wrapper; }
			set { wrapper = value; }
		}
	}
	public class PivotGridCustomTotal : DXFrameworkContentElement {
		#region statics
		public static readonly DependencyProperty CellFormatProperty;
		public static readonly DependencyProperty FormatProperty;
		public static readonly DependencyProperty SummaryTypeProperty;
		static PivotGridCustomTotal() {
			Type ownerType = typeof(PivotGridCustomTotal);
			CellFormatProperty = DependencyPropertyManager.Register("CellFormat", typeof(string), ownerType, new PropertyMetadata(null, OnCellFormatPropertyChanged));
			FormatProperty = DependencyPropertyManager.Register("Format", typeof(string), ownerType, new PropertyMetadata(null, OnFormatPropertyChanged));
			SummaryTypeProperty = DependencyPropertyManager.Register("SummaryType", typeof(FieldSummaryType), ownerType, new PropertyMetadata(FieldSummaryType.Sum,
				OnSummaryTypePropertyChanged));
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty TagProxyProperty = DependencyPropertyManager.Register("TagProxy", typeof(object), typeof(PivotGridCustomTotal), new PropertyMetadata(null, OnTagProxyChanged));
		static void OnTagProxyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridCustomTotal)d).OnTagChanged(e.NewValue);
		}
		static void OnCellFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridCustomTotal)d).OnCellFormatChanged((string)e.NewValue);
		}
		static void OnFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridCustomTotal)d).OnFormatChanged((string)e.NewValue);
		}
		static void OnSummaryTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridCustomTotal)d).OnSummaryTypeChanged((FieldSummaryType)e.NewValue);
		}
		#endregion
		PivotGridInternalCustomTotal internalTotal;
		public PivotGridCustomTotal() : this(new PivotGridInternalCustomTotal()) { }
		protected internal PivotGridCustomTotal(PivotGridInternalCustomTotal customTotal) {
			InternalTotal = customTotal;
			SetBinding(PivotGridCustomTotal.TagProxyProperty, new Binding("Tag") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
		}
		protected internal PivotGridInternalCustomTotal InternalTotal {
			get { return internalTotal; }
			private set {
				if(internalTotal != null)
					internalTotal.Wrapper = null;
				internalTotal = value;
				if(internalTotal != null)
					internalTotal.Wrapper = this;
			}
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCustomTotalCellFormat")]
#endif
		public string CellFormat {
			get { return (string)GetValue(CellFormatProperty); }
			set { SetValue(CellFormatProperty, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCustomTotalFormat")]
#endif
		public string Format {
			get { return (string)GetValue(FormatProperty); }
			set { SetValue(FormatProperty, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridCustomTotalSummaryType")]
#endif
		public FieldSummaryType SummaryType {
			get { return (FieldSummaryType)GetValue(SummaryTypeProperty); }
			set { SetValue(SummaryTypeProperty, value); }
		}
		public virtual void CloneTo(PivotGridCustomTotal clone) {
			clone.CellFormat = CellFormat;
			clone.Format = Format;
			clone.SummaryType = SummaryType;
			clone.Tag = Tag;
		}
		public virtual string GetValueText(object value) {
			return InternalTotal.GetValueText(value);
		}
		public virtual bool IsEqual(PivotGridCustomTotal total) {
			return InternalTotal.IsEqual(total.InternalTotal);
		}
		public override string ToString() {
			return InternalTotal.ToString();
		}
		protected virtual void OnCellFormatChanged(string newValue) {
			InternalTotal.CellFormat.FormatString = newValue;
			if(InternalTotal.CellFormat.FormatString != newValue)
				CellFormat = InternalTotal.CellFormat.FormatString;
			InternalTotal.CellFormat.FormatType = string.IsNullOrEmpty(newValue) ? FormatType.None : FormatType.Custom;
		}
		protected virtual void OnFormatChanged(string newValue) {
			InternalTotal.Format.FormatString = newValue;
			if(InternalTotal.Format.FormatString != newValue)
				Format = InternalTotal.Format.FormatString;
			InternalTotal.Format.FormatType = string.IsNullOrEmpty(newValue) ? FormatType.None : FormatType.Custom;
		}
		protected virtual void OnTagChanged(object newValue) {
			InternalTotal.Tag = newValue;
		}
		protected virtual void OnSummaryTypeChanged(FieldSummaryType newValue) {
			InternalTotal.SummaryType = newValue.ToPivotSummaryType();
		}
	}
	public class PivotGridCustomTotalCollection : PivotChildCollection<PivotGridCustomTotal> {
		PivotGridCustomTotalCollectionBase internalCollection;
		public PivotGridCustomTotalCollection(PivotGridField field)
			: base(field) {
			internalCollection = field.InternalField.CustomTotals;
		}
		protected internal PivotGridCustomTotalCollectionBase InternalCollection {
			get { return internalCollection; }
		}
		public PivotGridCustomTotal Add(FieldSummaryType summaryType) {
			PivotGridCustomTotal customTotal = new PivotGridCustomTotal();
			customTotal.SummaryType = summaryType;
			Add(customTotal);
			return customTotal;
		}
		public void Add(PivotGridCustomTotalCollection collection) {
			foreach(PivotGridCustomTotal customTotal in collection) {
				PivotGridCustomTotal customTotal2 = new PivotGridCustomTotal();
				customTotal.CloneTo(customTotal2);
				Add(customTotal2);
			}
		}
		protected override void OnItemAdded(int index, PivotGridCustomTotal item) {
			base.OnItemAdded(index, item);
			((IList)InternalCollection).Insert(index, item.InternalTotal);
		}
		protected override void OnItemRemoved(int index, PivotGridCustomTotal item) {
			base.OnItemRemoved(index, item);
			InternalCollection.Remove(item.InternalTotal);
		}
		protected override void OnItemMoved(int oldIndex, int newIndex, PivotGridCustomTotal item) {
			base.OnItemMoved(oldIndex, newIndex, item);
			InternalCollection.RemoveAt(oldIndex);
			((IList)InternalCollection).Insert(newIndex, item.InternalTotal);
		}
		protected override void OnItemReplaced(int index, PivotGridCustomTotal oldItem, PivotGridCustomTotal newItem) {
			base.OnItemReplaced(index, oldItem, newItem);
			((IList)InternalCollection)[index] = newItem.InternalTotal;
		}
		protected override void OnItemsClearing(IList<PivotGridCustomTotal> oldItems) {
			base.OnItemsClearing(oldItems);
			InternalCollection.Clear();
		}
		public bool Contains(FieldSummaryType summaryType) {
			return InternalCollection.Contains(summaryType.ToPivotSummaryType());
		}
		public void AssignArray(PivotGridCustomTotal[] totals) {
			Clear();
			foreach(PivotGridCustomTotal customTotal in totals) {
				Add(customTotal);
			}
		}
		public PivotGridCustomTotal[] CloneToArray() {
			PivotGridCustomTotal[] customTotals = new PivotGridCustomTotal[Count];
			for(int i = 0; i < Count; i++) {
				customTotals[i] = new PivotGridCustomTotal();
				this[i].CloneTo(customTotals[i]);
			}
			return customTotals;
		}
	}
}
