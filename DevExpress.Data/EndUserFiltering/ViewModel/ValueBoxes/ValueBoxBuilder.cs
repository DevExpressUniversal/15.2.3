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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;
	public interface IViewModelBuilderResolver {
		IViewModelBuilder CreateViewModelBuilder();
		IViewModelBuilder CreateValueViewModelBuilder(IEndUserFilteringMetric metric);
	}
	sealed class DefaultViewModelBuilderResolver : IViewModelBuilderResolver {
		internal static DefaultViewModelBuilderResolver Instance = new DefaultViewModelBuilderResolver();
		DefaultViewModelBuilderResolver() { }
		readonly static IDictionary<Type, Func<IEndUserFilteringMetric, IViewModelBuilder>> initializers =
			new Dictionary<Type, Func<IEndUserFilteringMetric, IViewModelBuilder>>() {
				{ typeof(IRangeMetricAttributes<>), (metric) => new RangeValueViewModelBuilder(metric) },
				{ typeof(ILookupMetricAttributes<>), (metric) => new LookupValueViewModelBuilder(metric) },
				{ typeof(IChoiceMetricAttributes<>), (metric) => new BooleanChoiceValueViewModelBuilder(metric) },
				{ typeof(IEnumChoiceMetricAttributes<>), (metric) => new EnumValueViewModelBuilder(metric) },
		};
		IViewModelBuilder IViewModelBuilderResolver.CreateViewModelBuilder() {
			return DefaultViewModelBuilder.Instance;
		}
		IViewModelBuilder IViewModelBuilderResolver.CreateValueViewModelBuilder(IEndUserFilteringMetric metric) {
			Func<IEndUserFilteringMetric, IViewModelBuilder> initializer;
			if(initializers.TryGetValue(metric.AttributesTypeDefinition, out initializer))
				return initializer(metric);
			return DefaultViewModelBuilder.Instance;
		}
		#region Builders
		class DefaultViewModelBuilder : IViewModelBuilder {
			internal readonly static IViewModelBuilder Instance = new DefaultViewModelBuilder();
			DefaultViewModelBuilder() { }
			string IViewModelBuilder.TypeNameModifier {
				get { return null; }
			}
			bool IViewModelBuilder.ForceBindableProperty(PropertyInfo property) {
				return false;
			}
			void IViewModelBuilder.BuildBindablePropertyAttributes(PropertyInfo property, PropertyBuilder builder) { }
		}
		class ValueViewModelBuilderBase<TMetricAttributes> : IViewModelBuilder 
			where TMetricAttributes : IMetricAttributes {
			readonly IEndUserFilteringMetric metric;
			public ValueViewModelBuilderBase(IEndUserFilteringMetric metric) {
				this.metric = metric;
			}
			protected IEndUserFilteringMetric Metric {
				get { return metric; }
			}
			protected TMetricAttributes MetricAttributes {
				get { return (TMetricAttributes)metric.Attributes; }
			}
			string IViewModelBuilder.TypeNameModifier {
				get { return metric.Path; }
			}
			void IViewModelBuilder.BuildBindablePropertyAttributes(PropertyInfo property, PropertyBuilder propertyBuilder) {
				if(CanProcessProperty(property))
					BuildBindablePropertyAttributesCore(property, propertyBuilder);
			}
			bool IViewModelBuilder.ForceBindableProperty(PropertyInfo property) {
				return ForceBindableProperty(property);
			}
			protected virtual bool CanProcessProperty(PropertyInfo property) {
				return System.Array.IndexOf(GetPropertiesToProcess(), property.Name) != -1;
			}
			protected virtual string[] GetPropertiesToProcess() {
				return new string[] { };
			}
			protected virtual void BuildBindablePropertyAttributesCore(PropertyInfo property, PropertyBuilder propertyBuilder) {
				DisplayFormatAttributeBuilder.Build(metric)
					.@Do(a => propertyBuilder.SetCustomAttribute(a));
				DataTypeAttributeBuilder.Build(metric)
					.@Do(a => propertyBuilder.SetCustomAttribute(a));
				BuildBindablePropertyFilterAttributes(propertyBuilder);
			}
			protected void BuildBindablePropertyFilterAttributes(PropertyBuilder propertyBuilder) {
				FilterPropertyAttributeBuilder.Build(metric)
					.@Do(a => propertyBuilder.SetCustomAttribute(a));
				FilterEditorAttributeBuilder.Build(metric)
					.@Do(a => propertyBuilder.SetCustomAttribute(a));
			}
			protected virtual bool ForceBindableProperty(PropertyInfo property) {
				return false;
			}
		}
		class RangeValueViewModelBuilder : ValueViewModelBuilderBase<IRangeMetricAttributes> {
			public RangeValueViewModelBuilder(IEndUserFilteringMetric metric)
				: base(metric) {
			}
			readonly static string[] PropertiesToProcess = new string[] { 
				"FromValue", "ToValue" 
			};
			protected override string[] GetPropertiesToProcess() {
				return PropertiesToProcess;
			}
			protected override void BuildBindablePropertyAttributesCore(PropertyInfo property, PropertyBuilder propertyBuilder) {
				if(property.Name == "FromValue") {
					DisplayAttributeBuilder.Build(MetricAttributes.FromName)
						.@Do(a => propertyBuilder.SetCustomAttribute(a));
				}
				if(property.Name == "ToValue") {
					DisplayAttributeBuilder.Build(MetricAttributes.ToName)
						.@Do(a => propertyBuilder.SetCustomAttribute(a));
				}
				if(TypeHelper.IsNullable(Metric.Type)) {
					RequiredAttributeBuilder.Build(Metric)
						.@Do(a => propertyBuilder.SetCustomAttribute(a));
				}
				base.BuildBindablePropertyAttributesCore(property, propertyBuilder);
			}
		}
		class LookupValueViewModelBuilder : ValueViewModelBuilderBase<ILookupMetricAttributes> {
			public LookupValueViewModelBuilder(IEndUserFilteringMetric metric)
				: base(metric) {
			}
			readonly static string ValuesProperty = "Values";
			protected override bool ForceBindableProperty(PropertyInfo property) {
				return property.Name == ValuesProperty;
			}
			protected override bool CanProcessProperty(PropertyInfo property) {
				return property.Name == ValuesProperty;
			}
			protected override void BuildBindablePropertyAttributesCore(PropertyInfo property, PropertyBuilder propertyBuilder) {
				BuildBindablePropertyFilterAttributes(propertyBuilder);
			}
		}
		class BooleanChoiceValueViewModelBuilder : ValueViewModelBuilderBase<IBooleanChoiceMetricAttributes> {
			public BooleanChoiceValueViewModelBuilder(IEndUserFilteringMetric metric)
				: base(metric) {
			}
			readonly static string ValueProperty = "Value";
			protected override bool ForceBindableProperty(PropertyInfo property) {
				return property.Name == ValueProperty;
			}
			protected override bool CanProcessProperty(PropertyInfo property) {
				return property.Name == ValueProperty;
			}
			protected override void BuildBindablePropertyAttributesCore(PropertyInfo property, PropertyBuilder propertyBuilder) {
				DisplayAttributeBuilder.Build(Metric)
					.@Do(a => propertyBuilder.SetCustomAttribute(a));
				base.BuildBindablePropertyAttributesCore(property, propertyBuilder);
			}
		}
		class EnumValueViewModelBuilder : ValueViewModelBuilderBase<IEnumChoiceMetricAttributes> {
			public EnumValueViewModelBuilder(IEndUserFilteringMetric metric)
				: base(metric) {
			}
			readonly static string[] ForceBindableProperties = new string[] { 
				"Value", "Values" 
			};
			protected override bool ForceBindableProperty(PropertyInfo property) {
				return Array.IndexOf(ForceBindableProperties, property.Name) != -1;
			}
			protected override string[] GetPropertiesToProcess() {
				return ForceBindableProperties;
			}
			protected override void BuildBindablePropertyAttributesCore(PropertyInfo property, PropertyBuilder propertyBuilder) {
				if(property.Name == "Value") {
					DisplayAttributeBuilder.Build(Metric)
						.@Do(a => propertyBuilder.SetCustomAttribute(a));
					EnumDataTypeAttributeBuilder.Build(MetricAttributes.EnumType)
						.@Do(a => propertyBuilder.SetCustomAttribute(a));
					base.BuildBindablePropertyAttributesCore(property, propertyBuilder);
				}
				if(property.Name == "Values")
					BuildBindablePropertyFilterAttributes(propertyBuilder);
			}
		}
		#endregion Builders
	}
}
