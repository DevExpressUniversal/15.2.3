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

namespace DevExpress.Design.Filtering {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;
	using Utils.Filtering.Internal;
	class EndUserFilteringMetricProperty : LocalizableEndUserFilteringObject<MetricPropertyCodeName>, IFilteringModelMetricProperty {
		public EndUserFilteringMetricProperty(MetricPropertyCodeName codeName, IEndUserFilteringMetric metric)
			: base(codeName) {
			this.Metric = metric;
			if(metric.AttributesTypeDefinition == typeof(IRangeMetricAttributes<>))
				EditorType = new EndUserFilteringMetricEditorType(MetricEditorTypeCodeName.Range, metric.Attributes);
			if(metric.AttributesTypeDefinition == typeof(ILookupMetricAttributes<>))
				EditorType = new EndUserFilteringMetricEditorType(MetricEditorTypeCodeName.Lookup, metric.Attributes);
			if(metric.AttributesTypeDefinition == typeof(IChoiceMetricAttributes<>))
				EditorType = new EndUserFilteringMetricEditorType(MetricEditorTypeCodeName.BooleanChoice, metric.Attributes);
			if(metric.AttributesTypeDefinition == typeof(IEnumChoiceMetricAttributes<>))
				EditorType = new EndUserFilteringMetricEditorType(MetricEditorTypeCodeName.EnumChoice, metric.Attributes);
		}
		public virtual bool ShowName {
			get {
				switch(((IFilteringModelMetricProperty)this).GetCodeName()) {
					case MetricPropertyCodeName.EditorType:
						return false;
				}
				return true;
			}
		}
		public IEndUserFilteringMetric Metric {
			get;
			private set;
		}
		public IFilteringModelMetricEditorType EditorType {
			get;
			private set;
		}
		protected override int GetHash(MetricPropertyCodeName codeName) {
			return (int)codeName;
		}
		public virtual bool IsEditable {
			get { return false; }
		}
	}
	abstract class EditableEndUserFilteringMetricProperty<T> : EndUserFilteringMetricProperty {
		IList<Attribute> attributes;
		public EditableEndUserFilteringMetricProperty(IList<Attribute> attributes, MetricPropertyCodeName codeName, IEndUserFilteringMetric metric)
			: base(codeName, metric) {
			this.attributes = attributes;
			this.valueCore = GetInitialValue();
			defaultValues = GetDefaultValues();
			defaultValue = GetDefaultValue();
		}
		public override bool IsEditable {
			get { return true; }
		}
		public override bool ShowName {
			get { return true; }
		}
		T[] defaultValues;
		T defaultValue;
		T valueCore;
		public T Value {
			get { return valueCore; }
			set {
				T newValue = value;
				if(IsDefaultValue(value)) {
					newValue = defaultValue;
				}
				if(!CanChangeValue(newValue)) return;
				valueCore = newValue;
				OnValueChanged();
			}
		}
		protected virtual void OnValueChanged() { }
		protected abstract T GetInitialValue();
		protected TAttribute GetAttribute<TAttribute>(Func<TAttribute> create) where TAttribute : Attribute {
			TAttribute attribute = attributes.OfType<TAttribute>().FirstOrDefault();
			if(attribute == null) {
				attribute = create();
				attributes.Add(attribute);
			}
			return attribute;
		}
		protected virtual bool CanChangeValue(T newValue) {
			if(valueCore == null) return !(newValue == null);
			return !valueCore.Equals(newValue);
		}
		protected virtual bool IsDefaultValue(T newValue) {
			if(defaultValues == null) return newValue == null;
			else return defaultValues.Contains(newValue);
		}
		protected abstract T[] GetDefaultValues();
		protected abstract T GetDefaultValue();
	}
	class EditableEndUserFilteringMetricCaptionProperty : EditableEndUserFilteringMetricProperty<string> {
		public EditableEndUserFilteringMetricCaptionProperty(IList<Attribute> customAttributes, IEndUserFilteringMetric metric)
			: base(customAttributes, MetricPropertyCodeName.Caption, metric) {
		}
		protected override string GetInitialValue() {
			return Metric.Caption;
		}
		protected override void OnValueChanged() {
			GetAttribute<DisplayAttribute>(() => new DisplayAttribute()).Name = Value;
		}
		protected override string[] GetDefaultValues() {
			return new string[] { String.Empty, null };
		}
		protected override string GetDefaultValue() {
			return null;
		}
	}
	class EditableEndUserFilteringMetricDescriptionProperty : EditableEndUserFilteringMetricProperty<string> {
		public EditableEndUserFilteringMetricDescriptionProperty(IList<Attribute> customAttributes, IEndUserFilteringMetric metric)
			: base(customAttributes, MetricPropertyCodeName.Description, metric) {
		}
		protected override string GetInitialValue() {
			return Metric.Description;
		}
		protected override void OnValueChanged() {
			GetAttribute<DisplayAttribute>(() => new DisplayAttribute()).Description = Value;
		}
		protected override string[] GetDefaultValues() {
			return new string[] { String.Empty, null };
		}
		protected override string GetDefaultValue() {
			return null;
		}
	}
	class EditableEndUserFilteringMetricEditorTypeProperty : EditableEndUserFilteringMetricProperty<IFilteringModelMetricEditorType> {
		public EditableEndUserFilteringMetricEditorTypeProperty(IList<Attribute> customAttributes, IEndUserFilteringMetric metric)
			: base(customAttributes, MetricPropertyCodeName.EditorType, metric) {
		}
		protected override IFilteringModelMetricEditorType GetInitialValue() {
			return this.EditorType;
		}
		protected override IFilteringModelMetricEditorType[] GetDefaultValues() {
			return null;
		}
		protected override IFilteringModelMetricEditorType GetDefaultValue() {
			return null;
		}
	}
}
