#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	using DevExpress.DashboardCommon.Native;
	using DevExpress.Data;
	using DevExpress.Data.Filtering;
	using AxisPoints = ICollection<AxisPoint>;
	using StyleIndexesList = List<int>;
	class DashboardFormatConditionValueProvider : IFormatConditionValueProvider, IEvaluatorDataAccess {
		MultiDimensionalData multiData;
		AxisPoint axis1Point;
		AxisPoint axis2Point;
		string measureId;
		public DashboardFormatConditionValueProvider() {
		}
		public void PrepareMeasureValueInfo(MultiDimensionalData multiData, AxisPoint axis1Point, AxisPoint axis2Point, string measureId) {
			this.multiData = multiData;
			this.axis1Point = axis1Point;
			this.axis2Point = axis2Point;
			this.measureId = measureId;
		}
		public void PrepareDimensionValueInfo(MultiDimensionalData multiData, AxisPoint axisPoint) {
			Guard.ArgumentNotNull(axisPoint, ToString() + ": axisPoint");
			this.multiData = multiData;
			this.axis1Point = axisPoint;
			this.axis2Point = null;
			this.measureId = null;
		}
		object GetDataItemValue(string id, bool isMeasure) {
			if(string.IsNullOrEmpty(id)) return null;
			if(isMeasure) {
				return multiData.GetValue(axis1Point, axis2Point, id).Value;
			} else {
				DimensionValue dimensionValue = (axis1Point != null) ? axis1Point.GetDimensionValue(id) : null;
				if(dimensionValue == null) {
					dimensionValue = axis2Point.GetDimensionValue(id);
				}
				return (dimensionValue != null) ? dimensionValue.Value : null;
			}
		}
		#region IFormatConditionValueProvider Members
		object IFormatConditionValueProvider.GetValue(IFormatCondition c) {
			if(string.IsNullOrEmpty(this.measureId)) {
				return axis1Point.Value;
			} else
				return GetDataItemValue(this.measureId, true);
		}
		object IFormatConditionValueProvider.GetExpressionObject(IFormatCondition c) {
			return this;
		}
		#endregion
		#region IEvaluatorDataAccess Members
		object IEvaluatorDataAccess.GetValue(PropertyDescriptor descriptor, object theObject) {
			DataItemPropertyDescriptor dataItemDescriptor = descriptor as DataItemPropertyDescriptor;
			if(dataItemDescriptor == null) return null;
			return GetDataItemValue(dataItemDescriptor.UniqueId, dataItemDescriptor.IsMeasure);
		}
		#endregion
	}
	class DashboardFormatConditionManager {
		static AxisPoints EmptyAxisPointCollection = new AxisPoint[] { };
		readonly MultiDimensionalData multiData;
		readonly IEnumerable<IParameter> parameters;
		public DashboardFormatConditionManager(MultiDimensionalData multiData, IEnumerable<IParameter> parameters) {
			this.multiData = multiData;
			this.parameters = parameters;
		}
		public void Calculate(IFormatRuleCollection formatRules) {
			foreach(DashboardItemFormatRule rule in formatRules) {
				if(!rule.Checked)
					continue;
				AddZeroPosition(rule);
				IEvaluatorRequired iEvaluatorRequired = rule.Condition as IEvaluatorRequired;
				PrepareRule(iEvaluatorRequired, rule);
				IFormatRuleLevel level = rule.LevelCore;
				DataItem itemCalculateBy = level.Item;
				string idCalculateBy = itemCalculateBy != null ? itemCalculateBy.ActualId : null;
				if(itemCalculateBy is Dimension && iEvaluatorRequired == null)
					AddConditionStylesByDimension(formatRules, rule, GetAxisPoints(idCalculateBy));
				else {
					Dimension axis1Item = level.Axis1Item;
					Dimension axis2Item = level.Axis2Item;
					bool hasAxis1Item = axis1Item != null;
					bool hasAxis2Item = axis2Item != null;
					bool ignoreLevel = !level.Enabled;
					AxisPoints axis1Points;
					AxisPoints axis2Points;
					if(ignoreLevel) {
						axis1Points = GetAllAxisPoints(1);
						axis2Points = GetAllAxisPoints(0);
					}
					else {
						axis1Points = hasAxis1Item ? GetAxisPoints(axis1Item.ActualId) : EmptyAxisPointCollection;
						axis2Points = hasAxis2Item ? GetAxisPoints(axis2Item.ActualId) : EmptyAxisPointCollection;
					}
					if(ignoreLevel || hasAxis1Item && hasAxis2Item) {
						foreach(AxisPoint axis1Point in axis1Points)
							AddConditionStylesByMeasure(formatRules, rule, axis1Point, axis2Points, idCalculateBy);
					}
					else if(hasAxis1Item)
						AddConditionStylesByMeasure(formatRules, rule, axis1Points, null, idCalculateBy);
					else if(hasAxis2Item)
						AddConditionStylesByMeasure(formatRules, rule, null, axis2Points, idCalculateBy);
					else
						AddConditionStyleByMeasureCore(formatRules, rule, null, null, idCalculateBy);
				}
			}
		}
		void AddZeroPosition(DashboardItemFormatRule rule) {
			string zeroPositionId = FormatConditionMeasureDescriptorIdManager.GetZeroPositionMeasureDescriptorId(rule.LevelCore);
			MeasureDescriptor zeroPositionDescriptor = multiData.GetZeroPositionMeasureDescriptorByID(zeroPositionId);
			decimal? value = rule.CalcZeroPosition();
			if(value.HasValue)
				multiData.AddValue(null, null, zeroPositionDescriptor, value.Value);
		}
		void PrepareRule(IEvaluatorRequired iEvaluatorRequired, DashboardItemFormatRule rule) {
			if(iEvaluatorRequired != null && rule.Context != null) {
				try {
					PropertyDescriptorCollection properties = DataItemPropertyDescriptor.GenerateCollection(rule.Context.DataItemRepositoryProvider);
					CriteriaOperator criteriaOperator = CriteriaOperator.TryParse(iEvaluatorRequired.Expression);
					ParametersToValuesCriteriaPatcher patcher = new ParametersToValuesCriteriaPatcher(parameters != null ? parameters : new IParameter[0]);
					ExpressionEvaluator evaluator = new ExpressionEvaluator(properties, patcher.Process(criteriaOperator)) {
						DataAccess = rule.ValueProvider
					};
					iEvaluatorRequired.Initialize(evaluator);
				}
				catch {
				}
			}
		}
		AxisPoints GetAxisPoints(string dimensionId) {
			return multiData.GetAxisPointsByDimensionId(dimensionId) ?? EmptyAxisPointCollection;
		}
		AxisPoints GetAllAxisPoints(int index) {
			IList<string> axes = multiData.GetAxisNames();
			return index < axes.Count ? multiData.GetAxisRoot(axes[index]).GetAllAxisPoints() : EmptyAxisPointCollection;
		}
		void AddConditionStylesByDimension(IFormatRuleCollection formatRules, DashboardItemFormatRule rule, AxisPoints axisPoints) {
			foreach(AxisPoint axisPoint in axisPoints) {
				rule.ValueProvider.PrepareDimensionValueInfo(multiData, axisPoint);
				bool hasStyle = AddConditionStyleValue(formatRules, rule, axisPoint, null);
				if(hasStyle && rule.StopIfTrue)
					return;
			}
		}
		void AddConditionStylesByMeasure(IFormatRuleCollection formatRules, DashboardItemFormatRule rule, AxisPoints axis1points, AxisPoint axis2point, string measureIdBy) {
			foreach(AxisPoint axis1point in axis1points) {
				bool hasStyle = AddConditionStyleByMeasureCore(formatRules, rule, axis1point, axis2point, measureIdBy);
				if(hasStyle && rule.StopIfTrue)
					return;
			}
		}
		void AddConditionStylesByMeasure(IFormatRuleCollection formatRules, DashboardItemFormatRule rule, AxisPoint axis1point, AxisPoints axis2points, string measureIdBy) {
			foreach(AxisPoint axis2point in axis2points) {
				bool hasStyle = AddConditionStyleByMeasureCore(formatRules, rule, axis1point, axis2point, measureIdBy);
				if(hasStyle && rule.StopIfTrue)
					return;
			}
		}
		bool AddConditionStyleByMeasureCore(IFormatRuleCollection formatRules, DashboardItemFormatRule rule, AxisPoint axis1point, AxisPoint axis2point, string measureIdBy) {
			rule.ValueProvider.PrepareMeasureValueInfo(multiData, axis1point, axis2point, measureIdBy);
			return AddConditionStyleValue(formatRules, rule, axis1point, axis2point);
		}
		bool AddConditionStyleValue(IFormatRuleCollection formatRules, DashboardItemFormatRule rule, AxisPoint axis1point, AxisPoint axis2point) {
			IStyleSettings styleSetting = rule.CalcStyleSetting();
			int styleSettingIndex = formatRules.IndexOfStyleSettings(styleSetting);
			bool hasStyle = styleSettingIndex >= 0;
			if(hasStyle) {
				DataItem itemApplyTo = rule.LevelCore.ItemApplyTo;
				MeasureDescriptor cfDescriptor = multiData.GetFormatConditionMeasureDescriptorByID(FormatConditionMeasureDescriptorIdManager.GetFormatConditionMeasureDescriptorId(rule.Name));
				MeasureDescriptor normalizedValueDescriptor = multiData.GetNormalizedValueMeasureDescriptorByID(FormatConditionMeasureDescriptorIdManager.GetNormalizedValueMeasureDescriptorId(rule.LevelCore));
				AxisPoint axisPoint1ApplyTo = axis1point;
				AxisPoint axisPoint2ApplyTo = axis2point;
				IFormatRuleIntersectionLevel intersectionLevel = rule.LevelCore as IFormatRuleIntersectionLevel;
				if(intersectionLevel != null) {
					if(itemApplyTo is Dimension && (intersectionLevel.Item != itemApplyTo || rule.Condition is IEvaluatorRequired)) {
						axisPoint1ApplyTo = FindAxisPointAtPointHierarchyById(axis1point, itemApplyTo.ActualId);
						axisPoint2ApplyTo = axisPoint1ApplyTo == null ? FindAxisPointAtPointHierarchyById(axis2point, itemApplyTo.ActualId) : null;
					}
				}
				AddMeasure(axisPoint1ApplyTo, axisPoint2ApplyTo, cfDescriptor, styleSettingIndex);
				AddMeasure(axisPoint1ApplyTo, axisPoint2ApplyTo, normalizedValueDescriptor, rule.CalcNormalizedValue());
			}
			return hasStyle;
		}
		AxisPoint FindAxisPointAtPointHierarchyById(AxisPoint point, string dimensionId) {
			AxisPoint currentAxisPoint = point;
			if(point != null && point.Dimension != null && dimensionId == point.Dimension.ID)
				return point;
			while(currentAxisPoint != null && currentAxisPoint.Parent != null && currentAxisPoint.Parent.Dimension != null) {
				currentAxisPoint = currentAxisPoint.Parent;
				if(dimensionId == currentAxisPoint.Dimension.ID)
					return currentAxisPoint;
			}
			return null;
		}
		void AddMeasure(AxisPoint axisPoint1ApplyTo, AxisPoint axisPoint2ApplyTo, MeasureDescriptor descriptor, int measure) {
			MeasureValue value = multiData.GetValue(axisPoint1ApplyTo, axisPoint2ApplyTo, descriptor);
			StyleIndexesList measureValues = value.Value != null ? (StyleIndexesList)value.Value : new StyleIndexesList();
			measureValues.Add(measure);
			multiData.AddValue(axisPoint1ApplyTo, axisPoint2ApplyTo, descriptor, measureValues);
		}
		void AddMeasure(AxisPoint axisPoint1ApplyTo, AxisPoint axisPoint2ApplyTo, MeasureDescriptor descriptor, decimal? measure) {
			if(measure.HasValue)
				multiData.AddValue(axisPoint1ApplyTo, axisPoint2ApplyTo, descriptor, measure.Value);
		}
	}
}
