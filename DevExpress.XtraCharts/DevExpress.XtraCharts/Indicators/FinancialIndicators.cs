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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FinancialIndicators : ChartElement, IXtraSupportDeserializeCollectionItem {
		readonly TrendLineCollection trendLines;
		readonly FibonacciIndicatorCollection fibonacciIndicators;
		internal XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)Owner; } }
		[
		Obsolete("This property is now obsolete. Use the Indicators property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public TrendLineCollection TrendLines { get { return trendLines; } }
		[
		Obsolete("This property is now obsolete. Use the Indicators property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public FibonacciIndicatorCollection FibonacciIndicators { get { return fibonacciIndicators; } }
		internal FinancialIndicators(XYDiagram2DSeriesViewBase view) : base(view) {
			trendLines = new TrendLineCollection(this);
			fibonacciIndicators = new FibonacciIndicatorCollection(this);
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeTrendLines() {
			return false;
		}
		bool ShouldSerializeFibonacciIndicators() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return false;
		}
		#endregion        
		#region XtraSerializing
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			switch (propertyName) {
				case "TrendLines":
					return new TrendLine();
				case "FibonacciIndicators":
					return new FibonacciIndicator();
				default:
					return null;
			}
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			switch (propertyName) {
				case "TrendLines":
					View.Indicators.Add((TrendLine)e.Item.Value);
					break;
				case "FibonacciIndicators":
					View.Indicators.Add((FibonacciIndicator)e.Item.Value);
					break;
			}
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new FinancialIndicators(null);
		}
	}
	public abstract class FinancialIndicatorCollection : ChartElementNamedCollection, IEnumerable, ICollection, IList {
		class FinancialIIndicatorEnumerator : IEnumerator {
			readonly IList collection;
			readonly Type financialIndicatorType;
			int position;
			object current;
			public object Current { get { return current; } }
			public FinancialIIndicatorEnumerator(IList collection, Type financialIndicatorType) {
				this.collection = collection;
				this.financialIndicatorType = financialIndicatorType;
				Reset();
			}
			public void Reset() {
				position = -1;
				current = null;
			}
			public bool MoveNext() {
				int count = collection.Count;
				while (++position < count) {
					current = collection[position];
					if (financialIndicatorType.IsAssignableFrom(current.GetType()))
						return true;
				}
				current = null;
				return false;
			}
		}
		IndicatorCollection actualCollection;
		protected IndicatorCollection ActualCollection { get { return actualCollection; } }
		protected abstract Type IndicatorType { get; }
		protected FinancialIndicatorCollection(FinancialIndicators financialIndicators) : base(financialIndicators) {
			actualCollection = financialIndicators.View.Indicators;
		}
		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator() {
			return new FinancialIIndicatorEnumerator(actualCollection, IndicatorType);
		}
		#endregion
		#region ICollection implementation
		int ICollection.Count { 
			get { 
				Type indicatorType = IndicatorType;
				int count = 0;
				foreach (Indicator indicator in actualCollection)
					if (indicatorType.IsAssignableFrom(indicator.GetType()))
						count++;
				return count; 
			} 
		}
		bool ICollection.IsSynchronized { get { return ((ICollection)actualCollection).IsSynchronized; } }
		object ICollection.SyncRoot { get { return ((ICollection)actualCollection).SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) { ((ICollection)actualCollection).CopyTo(array, index); }
		#endregion
		#region IList implementation
		object IList.this[int index] { 
			get { return actualCollection[index]; }
			set {}
		}
		bool IList.IsReadOnly { get { return ((IList)actualCollection).IsReadOnly; } }
		bool IList.IsFixedSize { get { return ((IList)actualCollection).IsFixedSize; } }
		int IList.Add(object value) { 
			return ((IList)actualCollection).Add(value); 
		} 
		void IList.Insert(int index, object value) {
			((IList)actualCollection).Insert(index, value); 
		}
		void IList.Remove(object value) {
			((IList)actualCollection).Remove(value);
		}
		void IList.RemoveAt(int index) {
			((IList)actualCollection).RemoveAt(index);
		}
		bool IList.Contains(object value) {
			return ((IList)actualCollection).Contains(value);
		}
		int IList.IndexOf(object value) {
			return ((IList)actualCollection).IndexOf(value);
		}
		#endregion
		public new abstract void Clear();
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class TrendLineCollection : FinancialIndicatorCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.IndTrendLine); } }
		protected override Type IndicatorType { get { return typeof(TrendLine); } }
		public TrendLine this[int index] { get { return ActualCollection[index] as TrendLine; } }
		internal TrendLineCollection(FinancialIndicators financialIndicators) : base(financialIndicators) { 
		}
		public int Add(TrendLine trendLine) {
			return ActualCollection.Add(trendLine);
		}
		public void AddRange(TrendLine[] coll) {
			ActualCollection.AddRange(coll);
		}
		public void Remove(TrendLine trendLine) {
			ActualCollection.Remove(trendLine);
		}
		public bool Contains(TrendLine trendLine) {
			return ActualCollection.Contains(trendLine);
		}		
		public TrendLine GetTrendLineByName(string name) {
			return ActualCollection.GetElementByName(name) as TrendLine;
		}
		public new TrendLine[] ToArray() {
			List<TrendLine> trendLines = new List<TrendLine>();
			foreach (Indicator indicator in ActualCollection) {
				TrendLine trendLine = indicator as TrendLine;
				if (trendLine != null)
					trendLines.Add(trendLine);
			}
			return trendLines.ToArray();
		}
		public override void Clear() {
			for (int i = 0; i < ActualCollection.Count; ) {
				TrendLine trendLine = ActualCollection[i] as TrendLine;
				if (trendLine == null)
					i++;
				else
					ActualCollection.RemoveAt(i);
			}
		}
	}   
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FibonacciIndicatorCollection : FinancialIndicatorCollection {
		protected override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.IndFibonacciIndicator); } }
		protected override Type IndicatorType { get { return typeof(FibonacciIndicator); } }
		public FibonacciIndicator this[int index] { get { return ActualCollection[index] as FibonacciIndicator; } }
		internal FibonacciIndicatorCollection(FinancialIndicators financialIndicators) : base(financialIndicators) {
		}
		public int Add(FibonacciIndicator fibonacciIndicator) {
			return ActualCollection.Add(fibonacciIndicator);
		}
		public void AddRange(FibonacciIndicator[] coll) {
			ActualCollection.AddRange(coll);
		}
		public void Remove(FibonacciIndicator fibonacciIndicator) {
			ActualCollection.Remove(fibonacciIndicator);
		}
		public bool Contains(FibonacciIndicator fibonacciIndicator) {
			return ActualCollection.Contains(fibonacciIndicator);
		}
		public FibonacciIndicator GetFibonacciIndicatorByName(string name) {
			return ActualCollection.GetElementByName(name) as FibonacciIndicator;
		}
		public new FibonacciIndicator[] ToArray() {
			List<FibonacciIndicator> fibonacciIndicators = new List<FibonacciIndicator>();
			foreach (Indicator indicator in ActualCollection) {
				FibonacciIndicator fibonacciIndicator = indicator as FibonacciIndicator;
				if (fibonacciIndicator != null)
					fibonacciIndicators.Add(fibonacciIndicator);
			}
			return fibonacciIndicators.ToArray();
		}
		public override void Clear() {
			for (int i = 0; i < ActualCollection.Count; ) {
				FibonacciIndicator fibonacciIndicator = ActualCollection[i] as FibonacciIndicator;
				if (fibonacciIndicator == null)
					i++;
				else
					ActualCollection.RemoveAt(i);
			}
		}
	}
}
