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

using System.Collections.Generic;
using System;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.PivotGrid {
	public enum PivotRunningTotalCalculationDirection { RightThenDown, LeftToRight, TopToBottom};
	public enum PivotVariationCalculationDirection { LeftToRight, TopToBottom };
	public enum PivotVariationCalculationType { Absolute, Percent };
	public enum PivotPercentageCalculationBaseLevel { Custom, GrandTotal, Total };
	public enum PivotPercentageCalculationBaseType { Cell, Column, Row };
	public enum PivotRankCalculationScope { Column, Row };
	public enum PivotRankCalculationOrder { LargestToSmallest, SmallestToLargest };
	public interface IPivotCalculationCreator {
		PivotRunningTotalCalculationBase CreateRunningTotalCalculation();
		PivotVariationCalculationBase CreateVariationCalculation();
		PivotPercentageCalculationBase CreatePercentageCalculation();
		PivotRankCalculationBase CreateRankCalculation();
		PivotIndexCalculationBase CreateIndexCalculation();
	}
	public class BaseCalculationCreator : IPivotCalculationCreator {
		#region IPivotCalculationCreator Members
		public PivotRunningTotalCalculationBase CreateRunningTotalCalculation() {
			return new PivotRunningTotalCalculationBase();
		}
		public PivotVariationCalculationBase CreateVariationCalculation() {
			return new PivotVariationCalculationBase();
		}
		public PivotPercentageCalculationBase CreatePercentageCalculation() {
			return new PivotPercentageCalculationBase();
		}
		public PivotRankCalculationBase CreateRankCalculation() {
			return new PivotRankCalculationBase();
		}
		public PivotIndexCalculationBase CreateIndexCalculation() {
			return new PivotIndexCalculationBase();
		}
		#endregion
	}
	public abstract class PivotCalculationBase {
		public const PivotRunningTotalCalculationDirection DefaultRunningTotalDirection = PivotRunningTotalCalculationDirection.LeftToRight;
		public const PivotVariationCalculationDirection DefaultVariationDirection = PivotVariationCalculationDirection.LeftToRight;
		public const PivotVariationCalculationType DefaultVariationType = PivotVariationCalculationType.Absolute;
		public const PivotPercentageCalculationBaseLevel DefaultPercentageBaseLevel = PivotPercentageCalculationBaseLevel.Total;
		public const PivotPercentageCalculationBaseType DefaultPercentageBaseType = PivotPercentageCalculationBaseType.Column;
		public const PivotRankCalculationOrder DefaultRankOrder = PivotRankCalculationOrder.SmallestToLargest;
		public const PivotRankCalculationScope DefaultRankScope = PivotRankCalculationScope.Column;
		public const bool DefaultApplyToTargetCellsOnly = false;
		public const bool DefaultTargetCellsIsColumnGrandTotal = false;
		public const DefaultBoolean DefaultCalculateAcrossGroups = DefaultBoolean.Default;
		internal static IPivotCalculationCreator BaseCreator = new BaseCalculationCreator();
		public event EventHandler Changed;
		protected void RaiseChanged() {
			Changed.SafeRaise(this, EventArgs.Empty);
		}
		internal static PivotCalculationBase CreateCalculationBySummaryType(PivotGridFieldBase field) {
			PivotSummaryDisplayType type = field.SummaryDisplayType;
			IPivotCalculationCreator creator = field.CalculationCreator;
			switch(type) {
				case PivotSummaryDisplayType.Default:
					return null;
				case PivotSummaryDisplayType.AbsoluteVariation:
				case PivotSummaryDisplayType.PercentVariation:
					var variationCalc = creator.CreateVariationCalculation();
					variationCalc.VariationType = (type == PivotSummaryDisplayType.AbsoluteVariation) ? PivotVariationCalculationType.Absolute : PivotVariationCalculationType.Percent;
					variationCalc.Direction = field.Data.OptionsDataField.Area != PivotDataArea.RowArea ?
						PivotVariationCalculationDirection.LeftToRight : PivotVariationCalculationDirection.TopToBottom;
					return variationCalc;
				case PivotSummaryDisplayType.PercentOfColumn:
				case PivotSummaryDisplayType.PercentOfColumnGrandTotal:
				case PivotSummaryDisplayType.PercentOfRow:
				case PivotSummaryDisplayType.PercentOfRowGrandTotal:
				case PivotSummaryDisplayType.PercentOfGrandTotal:
					var percentageCalc = creator.CreatePercentageCalculation();
					if(type != PivotSummaryDisplayType.PercentOfGrandTotal)
						percentageCalc.BaseType = (type == PivotSummaryDisplayType.PercentOfColumn || type == PivotSummaryDisplayType.PercentOfColumnGrandTotal) ?
							PivotPercentageCalculationBaseType.Column : PivotPercentageCalculationBaseType.Row;
					if(type == PivotSummaryDisplayType.PercentOfColumn || type == PivotSummaryDisplayType.PercentOfRow)
						percentageCalc.BaseLevel = PivotPercentageCalculationBaseLevel.Total;
					if(type == PivotSummaryDisplayType.PercentOfColumnGrandTotal || type == PivotSummaryDisplayType.PercentOfRowGrandTotal)
						percentageCalc.BaseLevel = PivotPercentageCalculationBaseLevel.GrandTotal;
					if(type == PivotSummaryDisplayType.PercentOfGrandTotal)
						percentageCalc.BaseLevel = PivotPercentageCalculationBaseLevel.Custom;
					return percentageCalc;
				case PivotSummaryDisplayType.RankInColumnLargestToSmallest:
				case PivotSummaryDisplayType.RankInColumnSmallestToLargest:
				case PivotSummaryDisplayType.RankInRowLargestToSmallest:
				case PivotSummaryDisplayType.RankInRowSmallestToLargest:
					var rankCalc = creator.CreateRankCalculation();
					rankCalc.Scope = (type == PivotSummaryDisplayType.RankInColumnLargestToSmallest || type == PivotSummaryDisplayType.RankInColumnSmallestToLargest) ?
						PivotRankCalculationScope.Column : PivotRankCalculationScope.Row;
					rankCalc.Order = (type == PivotSummaryDisplayType.RankInColumnSmallestToLargest || type == PivotSummaryDisplayType.RankInRowSmallestToLargest) ?
						PivotRankCalculationOrder.SmallestToLargest : PivotRankCalculationOrder.LargestToSmallest;
					return rankCalc;
				case PivotSummaryDisplayType.Index:
					return creator.CreateIndexCalculation();
			}
			return null;
		}
		protected abstract Type GetFieldType();
		#region IPivotFieldCalculationInfo Members
		internal Type FieldType {
			get { return GetFieldType(); }
		}
		#endregion
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return GetType().Name; } }
		protected internal virtual void OnEndDeserializing(PivotGridFieldCollectionBase owner){ }
	}
	public class PivotCalculationTargetCellInfoBase {
		PivotGridFieldBase columnField;
		PivotGridFieldBase rowField;
		string columnFieldUniqueNameInternal;
		string rowFieldUniqueNameInternal;
		bool isColumnGrandTotal = PivotCalculationBase.DefaultTargetCellsIsColumnGrandTotal;
		public PivotCalculationTargetCellInfoBase(PivotGridFieldBase columnField, PivotGridFieldBase rowField, bool isColumnGrandTotal) {
			this.columnField = columnField;
			this.rowField = rowField;
			this.isColumnGrandTotal = isColumnGrandTotal;
		}
		public PivotCalculationTargetCellInfoBase(PivotGridFieldBase field, PivotGridFieldBase crossAreaField)
			: this(field, crossAreaField, false) { }
		public PivotCalculationTargetCellInfoBase(PivotGridFieldBase field)
			: this(field, null, false) { }
		public PivotCalculationTargetCellInfoBase()
			: this(null, null, false) { }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PivotGridFieldBase ColumnField {
			get { return columnField; }
			set {
				if(value != columnField) {
					columnField = value;
					RaiseChanged();
				}
			}
		}
		[
		Browsable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty()
		]
		public string ColumnFieldUniqueName {
			get { return ColumnField != null ? ColumnField.Name : String.Empty; }
			set { columnFieldUniqueNameInternal = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PivotGridFieldBase RowField {
			get { return rowField; }
			set {
				if(value != rowField) {
					rowField = value;
					RaiseChanged();
				}
			}
		}
		[
		Browsable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty()
		]
		public string RowFieldUniqueName {
			get { return RowField != null ? RowField.Name : String.Empty; }
			set { rowFieldUniqueNameInternal = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DefaultValue(PivotCalculationBase.DefaultTargetCellsIsColumnGrandTotal),
		XtraSerializableProperty()
		]
		public bool IsColumnGrandTotal {
			get { return isColumnGrandTotal; }
			set {
				if(value != isColumnGrandTotal) {
					isColumnGrandTotal = value;
					RaiseChanged();
				}
			}
		}
		internal event EventHandler Changed;
		void RaiseChanged() {
			Changed.SafeRaise(this, EventArgs.Empty);
		}
		internal void RestoreFields(PivotGridFieldCollectionBase fields) {
			if(!String.IsNullOrEmpty(rowFieldUniqueNameInternal))
				rowField = fields.GetFieldByName(rowFieldUniqueNameInternal);
			if(!String.IsNullOrEmpty(columnFieldUniqueNameInternal))
				columnField = fields.GetFieldByName(columnFieldUniqueNameInternal);
		}
	}
	public abstract class PivotSpecificCellsCalculationBase : PivotCalculationBase {
		bool applyToTargetCellsOnly;
		readonly IList<PivotCalculationTargetCellInfoBase> targetCells;
		protected PivotSpecificCellsCalculationBase() {
			this.applyToTargetCellsOnly = DefaultApplyToTargetCellsOnly;
			this.targetCells = CreateTargetCells();
		}
		protected virtual IList<PivotCalculationTargetCellInfoBase> CreateTargetCells() {
			var targetCellsCollection = new PivotCalculationTargetCellCollectionBase<PivotCalculationTargetCellInfoBase>();
			targetCellsCollection.Changed += OnCellsChanged;
			return targetCellsCollection;
		}
		[
		DefaultValue(DefaultApplyToTargetCellsOnly), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public bool ApplyToTargetCellsOnly {
			get { return applyToTargetCellsOnly; }
			set {
				if(value != applyToTargetCellsOnly) {
					applyToTargetCellsOnly = value;
					RaiseChanged();
				}
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IList<PivotCalculationTargetCellInfoBase> TargetCellsInternal {
			get { return targetCells; }
		}
		public void AddTargetCells(PivotGridFieldBase field) {
			TargetCellsInternal.Add(new PivotCalculationTargetCellInfoBase(field));
		}
		protected void OnCellsChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void XtraClearTargetCells(XtraItemEventArgs e) {
			TargetCellsInternal.Clear();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeTargetCells() {
			return TargetCellsInternal.Count > 0;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetTargetCells() {
			TargetCellsInternal.Clear();
		}
		protected internal override void OnEndDeserializing(PivotGridFieldCollectionBase owner) {
			foreach(PivotCalculationTargetCellInfoBase cellInfo in TargetCellsInternal) {
				cellInfo.RestoreFields(owner);
			}
		}
	}
	public class PivotCalculationTargetCellCollectionBase<T> : NotifyCollection<T> where T : PivotCalculationTargetCellInfoBase {
		protected override void InsertItem(int index, T item) {
			base.InsertItem(index, item);
			item.Changed += itemChanged;
		}
		protected override void SetItem(int index, T item) {
			base.SetItem(index, item);
			item.Changed += itemChanged;
		}
		protected override void RemoveItem(int index) {
			this[index].Changed -= itemChanged;
			base.RemoveItem(index);
		}
		void itemChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
	public abstract class PivotCrossGroupCalculationBase : PivotSpecificCellsCalculationBase {
		DefaultBoolean calculateAcrossGroups;
		protected PivotCrossGroupCalculationBase() {
			calculateAcrossGroups = DefaultCalculateAcrossGroups;
		}
		[
		DefaultValue(DefaultCalculateAcrossGroups), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public DefaultBoolean CalculateAcrossGroups {
			get { return calculateAcrossGroups; }
			set {
				if(value != calculateAcrossGroups) {
					calculateAcrossGroups = value;
					RaiseChanged();
				}
			}
		}
	}
	public class PivotRunningTotalCalculationBase : PivotCrossGroupCalculationBase {
		PivotRunningTotalCalculationDirection direction;
		public PivotRunningTotalCalculationBase() {
			direction = DefaultRunningTotalDirection;
		}
		[
		DefaultValue(DefaultRunningTotalDirection), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotRunningTotalCalculationDirection Direction {
			get { return direction; }
			set {
				if(value != direction) {
					direction = value;
					RaiseChanged();
				}
			}
		}
		protected override Type GetFieldType() {
			return typeof(decimal);
		}
	}
	public class PivotVariationCalculationBase : PivotCrossGroupCalculationBase {
		PivotVariationCalculationDirection direction;
		PivotVariationCalculationType variationType;
		public PivotVariationCalculationBase() {
			direction = DefaultVariationDirection;
			variationType = DefaultVariationType;
		}
		[
		DefaultValue(DefaultVariationDirection), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotVariationCalculationDirection Direction {
			get { return direction; }
			set {
				if(value != direction) {
					direction = value;
					RaiseChanged();
				}
			}
		}
		[
		DefaultValue(DefaultVariationType), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotVariationCalculationType VariationType {
			get { return variationType; }
			set {
				if(value != variationType) {
					variationType = value;
					RaiseChanged();
				}
			}
		}
		protected override Type GetFieldType() {
			return typeof(decimal);
		}
	}
	public class PivotPercentageCalculationBase : PivotSpecificCellsCalculationBase {
		PivotPercentageCalculationBaseType baseType;
		PivotPercentageCalculationBaseLevel baseLevel;
		object[] columnValues;
		object[] rowValues;
		public PivotPercentageCalculationBase() {
			baseType = DefaultPercentageBaseType;
			baseLevel = DefaultPercentageBaseLevel;
		}
		[
		DefaultValue(DefaultPercentageBaseType), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotPercentageCalculationBaseType BaseType {
			get { return baseType; }
			set {
				if(value != baseType) {
					baseType = value;
					RaiseChanged();
				}
			}
		}
		[
		DefaultValue(DefaultPercentageBaseLevel), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotPercentageCalculationBaseLevel BaseLevel {
			get { return baseLevel; }
			set {
				if(value != baseLevel) {
					baseLevel = value;
					RaiseChanged();
				}
			}
		}
		[
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection)
		]
		public object[] ColumnValues {
			get { return columnValues; }
			set {
				if(value != columnValues) {
					columnValues = value;
					RaiseChanged();
				}
			}
		}
		[
		DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection)
		]
		public object[] RowValues {
			get { return rowValues; }
			set {
				if(value != rowValues) {
					rowValues = value;
					RaiseChanged();
				}
			}
		}
		protected override Type GetFieldType() {
			return typeof(decimal);
		}
	}
	public class PivotRankCalculationBase : PivotSpecificCellsCalculationBase {
		PivotRankCalculationScope scope;
		PivotRankCalculationOrder order;
		public PivotRankCalculationBase() {
			scope = DefaultRankScope;
			order = DefaultRankOrder;
		}
		[
		DefaultValue(DefaultRankScope), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotRankCalculationScope Scope {
			get { return scope; }
			set {
				if(value != scope) {
					scope = value;
					RaiseChanged();
				}
			}
		}
		[
		DefaultValue(DefaultRankOrder), NotifyParentProperty(true),
		XtraSerializableProperty()
		]
		public PivotRankCalculationOrder Order {
			get { return order; }
			set {
				if(value != order) {
					order = value;
					RaiseChanged();
				}
			}
		}
		protected override Type GetFieldType() {
			return typeof(int);
		}
	}
	public class PivotIndexCalculationBase : PivotCalculationBase {
		protected override Type GetFieldType() {
			return typeof(decimal);
		}
	}
	static class XtraSerializingUtils {
		const string typeNamePropertyName = "TypeNameSerializable";
		public static object GetContentPropertyInstance(XtraItemEventArgs e) {
			string typeName = e.Item.ChildProperties[typeNamePropertyName].Value.ToString();
			Type type = ReflectionHelper.GetTypeByTypeName(typeName);
			return Activator.CreateInstance(type);
		}
	}
	public static class ReflectionHelper {
		static List<System.Reflection.Assembly> asms = new List<System.Reflection.Assembly>();
		public static void AddAssembly(System.Reflection.Assembly asm) {
			asms.Add(asm);
		}
		static ReflectionHelper() {
			AddAssembly(typeof(ReflectionHelper).GetAssembly());
		}
		public static Type GetTypeByTypeName(string typeName) {
			foreach(System.Reflection.Assembly asm in asms) {
				Type type = asm.GetType("DevExpress.PivotGrid." + typeName);
				if(type == null)
					type = asm.GetType("DevExpress.XtraPivotGrid." + typeName);
				if(type != null)
					return type;
			}
			return null;
		}
	}
}
