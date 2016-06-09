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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	static class FilterEditorAttributeBuilder {
		readonly static ConstructorInfo numericRangeAttributeCtor;
		readonly static ConstructorInfo dateTimeRangeAttributeCtor;
		readonly static ConstructorInfo lookupAttributeCtor;
		readonly static ConstructorInfo booleanChoiceAttributeCtor;
		static FilterEditorAttributeBuilder() {
			var attributeType = typeof(Internal.FilterEditorAttribute);
			numericRangeAttributeCtor = attributeType.GetConstructor(new Type[] { typeof(RangeUIEditorType) });
			dateTimeRangeAttributeCtor = attributeType.GetConstructor(new Type[] { typeof(DateTimeRangeUIEditorType) });
			lookupAttributeCtor = attributeType.GetConstructor(new Type[] { typeof(LookupUIEditorType), typeof(bool), typeof(bool) });
			booleanChoiceAttributeCtor = attributeType.GetConstructor(new Type[] { typeof(BooleanUIEditorType) });
		}
		internal static CustomAttributeBuilder Build(IEndUserFilteringMetric metric) {
			if(metric != null) {
				if(metric.AttributesTypeDefinition == typeof(IRangeMetricAttributes<>)) {
					IRangeMetricAttributes rangeMetric = (IRangeMetricAttributes)metric.Attributes;
					if(rangeMetric.IsNumericRange)
						return new CustomAttributeBuilder(numericRangeAttributeCtor, new object[] { rangeMetric.NumericRangeUIEditorType });
					else
						return new CustomAttributeBuilder(dateTimeRangeAttributeCtor, new object[] { rangeMetric.DateTimeRangeUIEditorType });
				}
				if(metric.AttributesTypeDefinition == typeof(ILookupMetricAttributes<>)) {
					ILookupMetricAttributes lookupMetric = (ILookupMetricAttributes)metric.Attributes;
					return new CustomAttributeBuilder(lookupAttributeCtor, new object[] { lookupMetric.EditorType, lookupMetric.UseFlags, false });
				}
				if(metric.AttributesTypeDefinition == typeof(IChoiceMetricAttributes<>)) {
					IBooleanChoiceMetricAttributes booleanChoiceMetric = (IBooleanChoiceMetricAttributes)metric.Attributes;
					return new CustomAttributeBuilder(booleanChoiceAttributeCtor, new object[] { booleanChoiceMetric.EditorType });
				}
				if(metric.AttributesTypeDefinition == typeof(IEnumChoiceMetricAttributes<>)) {
					IEnumChoiceMetricAttributes enumChoiceMetric = (IEnumChoiceMetricAttributes)metric.Attributes;
					return new CustomAttributeBuilder(lookupAttributeCtor, new object[] { enumChoiceMetric.EditorType, enumChoiceMetric.UseFlags, true });
				}
				throw new NotSupportedException(metric.AttributesTypeDefinition.ToString());
			}
			return null;
		}
	}
}
