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
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					  "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class IndicatorCollection : ChartElementNamedCollection, IEnumerable<Indicator> {
		protected override string NamePrefix {  get { return String.Empty; }  }
		internal XYDiagram2DSeriesViewBase View { get { return (XYDiagram2DSeriesViewBase)Owner; } }
		public Indicator this[int index] { get { return (Indicator)List[index]; } }
		internal IndicatorCollection(XYDiagram2DSeriesViewBase view) : base(view) {
		}
		#region IEnumerable Implementation
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<Indicator>)this).GetEnumerator();
		}
		#endregion
		#region IEnumerable<Indicator> Implementation
		IEnumerator<Indicator> IEnumerable<Indicator>.GetEnumerator() {
			foreach (Indicator indicator in this)
				yield return indicator;
		}
		#endregion
		IRefinedSeries FindRefinedSeries() {
			return View != null && View.Owner is Series && View.Chart != null ? View.Chart.ViewController.FindRefinedSeries(View.Series) : null;
		}
		protected override void OnInsert(int index, object value) {
			Indicator indicator = value as Indicator;
			if (indicator == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectIndicator));
			if (!View.Loading)
				indicator.Validate(View, FindRefinedSeries());
			base.OnInsert(index, value);
		}
		internal void OnEndLoding() {
			foreach (Indicator indicator in this) {
				SeparatePaneIndicator advancedIndicator = indicator as SeparatePaneIndicator;
				if (advancedIndicator != null)
					advancedIndicator.OnEndLoading();
			}
		}
		public new Indicator[] ToArray() {
			return (Indicator[])InnerList.ToArray(typeof(Indicator));
		}
		public int Add(Indicator indicator) {
			return base.Add(indicator);
		}
		public void AddRange(Indicator[] coll) {
			base.AddRange(coll);
		}
		public void Insert(int index, Indicator indicator) {
			base.Insert(index, indicator);
		}
		public void Remove(Indicator indicator) {
			base.Remove(indicator);
		}
		public void Swap(Indicator indicator1, Indicator indicator2) {
			base.Swap(indicator1, indicator2);
		}
		public int IndexOf(Indicator indicator) {
			return base.IndexOf(indicator);
		}
		public bool Contains(Indicator indicator) {
			return base.Contains(indicator);
		} 
		public override void Assign(ChartCollectionBase collection) {
			XYDiagram2DSeriesViewBase view = View;
			if (view == null)
				base.Assign(collection);
			else {
				if (collection == null)
					throw new ArgumentNullException("collection");
				Clear();
				foreach (Indicator indicator in collection)
					try {
						indicator.Validate(view, FindRefinedSeries());
						ChartElement item = (Indicator)indicator.Clone();
						InnerList.Add(item);
						ChangeOwnerForItem(item);
					}
					catch {
					}
			}
		}
	}
}
