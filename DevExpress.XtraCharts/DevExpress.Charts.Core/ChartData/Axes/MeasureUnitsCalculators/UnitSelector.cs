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
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public class UnitContainer<TMeasureUnit, TGridAlignment> {
		internal const double UnitChangeFactor = 0.2;
		public class ThresholdItem {
			readonly double threshold;
			public double Threshold { get { return threshold; } }
			public ThresholdItem(double threshold) {
				this.threshold = threshold;
			}
		}
		public class UnitItem<T> : ThresholdItem {
			readonly T unit;
			public T Unit { get { return unit; } }
			public UnitItem(double threshold, T unit) : base(threshold) {
				this.unit = unit;
			}
		}
		public class MeasureItem : UnitItem<TMeasureUnit> {
			public MeasureItem(double threshold, TMeasureUnit unit) : base(threshold, unit) {
			}
		}
		public class AlignmentItem : UnitItem<TGridAlignment> {
			public AlignmentItem(double threshold, TGridAlignment unit) : base(threshold, unit) {
			}
		}
		readonly List<MeasureItem> measureItems;
		readonly List<AlignmentItem> alignmentItems;
		public int MeasureUnitCount {
			get {
				return measureItems.Count;
			}
		}
		public int AlignmentUnitCount {
			get {
				return alignmentItems.Count;
			}
		}
		public UnitContainer() {
			this.measureItems = new List<MeasureItem>();
			this.alignmentItems = new List<AlignmentItem>();
		}
		T SelectUnit<T>(IList<T> list, double value, double previousValue) where T : ThresholdItem {
			double offsetFactor = 1;
			if (!double.IsNaN(previousValue))
				offsetFactor = value > previousValue ? 1.1 : 0.9;
			for (int i = 0; i < list.Count - 1; i++) {
				if (value <= list[i].Threshold)
					return list[i];
				if (value < list[i + 1].Threshold) {
					double factor = (value - list[i].Threshold) / (list[i + 1].Threshold - list[i].Threshold);
					return factor >= UnitChangeFactor * offsetFactor ? list[i + 1] : list[i];
				}
			}
			return list[list.Count - 1];
		}
		T SelectUnit<T>(double value, double previousValue) where T : ThresholdItem {
			if (typeof(T) == typeof(MeasureItem))
				return SelectUnit<T>((IList<T>)measureItems, value, previousValue);
			else if (typeof(T) == typeof(AlignmentItem))
				return SelectUnit<T>((IList<T>)alignmentItems, value, previousValue);
			return null;
		}
		T GetUnitAt<T>(int index) where T : ThresholdItem {
			if (typeof(T) == typeof(MeasureItem)) {
				if (index < measureItems.Count)
					return (T)(object)measureItems[index];
			}
			else if (typeof(T) == typeof(AlignmentItem)) {
				if (index < alignmentItems.Count)
					return (T)(object)alignmentItems[index];
			}
			return null;
		}
		public MeasureItem SelectMeasureUnit(double value, double previousValue) {
			return SelectUnit<MeasureItem>(value, previousValue);
		}
		public AlignmentItem SelectAlignmentUnit(double value, double previousValue) {
			return SelectUnit<AlignmentItem>(value, previousValue);
		}
		public MeasureItem GetMeasureUnitAt(int index) {
			return GetUnitAt<MeasureItem>(index);
		}
		public AlignmentItem GetAlignmentUnitAt(int index) {
			return GetUnitAt<AlignmentItem>(index);
		}
		public void UpdateActiveUnits(IEnumerable<MeasureItem> measureItems, IEnumerable<AlignmentItem> alignmentItems) {
			this.measureItems.Clear();
			if (measureItems != null)
				this.measureItems.AddRange(measureItems);
			this.alignmentItems.Clear();
			if (alignmentItems != null)
				this.alignmentItems.AddRange(alignmentItems);
		}
	}
	public abstract class UnitSelector<TMeasureUnit, TGridAlignment> {
		protected class UnitContainer : UnitContainer<TMeasureUnit, TGridAlignment> {
		}
		public struct GridAlignment {
			readonly TGridAlignment unit;
			readonly double spacing;
			public TGridAlignment Unit { get { return unit; } }
			public double Spacing { get { return spacing; } }
			public GridAlignment(TGridAlignment unit, double spacing) {
				this.unit = unit;
				this.spacing = spacing;
			}
		}
		readonly UnitContainer activeUnitContainer;
		protected UnitContainer ActiveUnitContainer { get { return activeUnitContainer; } }
		public UnitSelector() {
			activeUnitContainer = new UnitContainer();
		}
		internal IList<TMeasureUnit> SelectActiveMeasureUnits() {
			List<TMeasureUnit> units = new List<TMeasureUnit>();
			for (int i = 0; i < activeUnitContainer.MeasureUnitCount; i++)
				units.Add(activeUnitContainer.GetMeasureUnitAt(i).Unit);
			return units;
		}
		public TMeasureUnit SelectMeasureUnit(double value, double previousValue) {
			return activeUnitContainer.SelectMeasureUnit(value, previousValue).Unit;
		}
		public virtual GridAlignment SelectAlignment(double value, double previousValue) {
			var alignment = activeUnitContainer.SelectAlignmentUnit(value, previousValue);
			double spacing = Math.Max(1.0, Math.Ceiling(value / alignment.Threshold));
			return new GridAlignment(alignment.Unit, spacing);
		}
		public abstract void UpdateActiveUnits(double min, double max);
	}
}
