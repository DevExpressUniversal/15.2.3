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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using DevExpress.Web.Internal;
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using System.ComponentModel.DataAnnotations;
	public class MaskAttribute : ValidationAttribute, IClientValidatable {
		public MaskAttribute(string mask) {
			MaskSettings = new MaskSettings(null);
			MaskSettings.ErrorText = string.Empty;
			MaskSettings.Mask = mask;
			MaskSettings.PromptChar = MaskSettings.DefaultPromptChar;
			MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.All;
		}
		public string Mask { get { return MaskSettings.Mask; } set { MaskSettings.Mask = value; } }
		public char PromptChar { get { return MaskSettings.PromptChar; } set { MaskSettings.PromptChar = value; } }
		public MaskIncludeLiteralsMode IncludeLiterals { get { return MaskSettings.IncludeLiterals; } set { MaskSettings.IncludeLiterals = value; } }
		protected internal MaskSettings MaskSettings { get; private set; }
		public override bool IsValid(object value) {
			if(value is string)
				return MaskValidator.IsValueValid(value as string, Mask, IncludeLiterals);
			return true;
		}
		IEnumerable<ModelClientValidationRule> IClientValidatable.GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
			yield return new ModelClientValidationRule {
				ValidationType = "dxmask",
				ErrorMessage = FormatErrorMessage(metadata.DisplayName)
			};
		}
	}
	public class DateRangeAttribute: ValidationAttribute, IClientValidatable {
		class MVCxDateEditRangeSettings: DateEditRangeSettings {
			public MVCxDateEditRangeSettings()
				: base(null) {
			}
			protected internal new string GetErrorText(){
				return base.GetErrorText();
			}
		}
		public DateRangeAttribute() {
			DateEditRangeSettings = new MVCxDateEditRangeSettings();
		}
		public string StartDateEditFieldName { 
			get { return DateEditRangeSettings.StartDateEditID; } 
			set { DateEditRangeSettings.StartDateEditID = value; } 
		}
		public int MinDayCount {
			get { return DateEditRangeSettings.MinDayCount; }
			set { DateEditRangeSettings.MinDayCount = value; }
		}
		public int MaxDayCount {
			get { return DateEditRangeSettings.MaxDayCount; }
			set { DateEditRangeSettings.MaxDayCount = value; }
		}
		public string MinErrorText {
			get { return DateEditRangeSettings.MinErrorText; }
			set { DateEditRangeSettings.MinErrorText = value; }
		}
		public string RangeErrorText {
			get { return DateEditRangeSettings.RangeErrorText; }
			set { DateEditRangeSettings.RangeErrorText = value; }
		}
		protected internal DateEditRangeSettings DateEditRangeSettings { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new string ErrorMessage {
			get { return base.ErrorMessage; }
			set { base.ErrorMessage = value; }
		}
		protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
			DateTime start, end;
			RefreshErrorMesssage();
			if(TryGetStartDate(validationContext, out start) && TryGetDateTime(value, out end) && !DateRangeValidator.IsValueValid(start, end, DateEditRangeSettings))
				return new ValidationResult(ErrorMessage);
			return ValidationResult.Success;
		}
		bool TryGetStartDate(ValidationContext validationContext, out DateTime start) {
			start = DateTime.MinValue;
			PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(StartDateEditFieldName);
			if(propertyInfo == null)
				return false;
			return TryGetDateTime(propertyInfo.GetValue(validationContext.ObjectInstance, null), out start);
		}
		bool TryGetDateTime(object value, out DateTime result) {
			result = DateTime.MinValue;
			if(value == null)
				return false;
			string dateString = (value ?? string.Empty).ToString();
			return DateTime.TryParse(dateString, out result);
		}
		void RefreshErrorMesssage() {
			var settings = DateEditRangeSettings as MVCxDateEditRangeSettings;
			if(settings != null)
				ErrorMessage = settings.GetErrorText();
		}
		#region IClientValidatable Members
		public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) {
			RefreshErrorMesssage();
			yield return new ModelClientValidationRule {
				ValidationType = "dxdaterange",
				ErrorMessage = FormatErrorMessage(metadata.DisplayName)
			};
		}
		#endregion
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public static class AttributeMapper {
		const string MaxLengthAttributeTypeFullName = "System.ComponentModel.DataAnnotations.MaxLengthAttribute";
		const string LengthPropertyName = "Length";
		public static void PrepareEditorProperties(EditPropertiesBase editProperties, ModelMetadata metadata) {
			if(metadata == null) return;
			PrepareEditorProperties(metadata.PropertyName, metadata.ContainerType, editProperties);
		}
		public static void PrepareEditorProperties(string fieldName, Type fieldType, EditPropertiesBase editProperties) {
			if(string.IsNullOrEmpty(fieldName)) return;
			PrepareEditorProperties(fieldType.GetProperty(fieldName), editProperties);
		}
		public static string GetRangeAttributeErrorMessage(ModelMetadata metadata) {
			RangeAttribute rangeAttribute = GetAttribute<RangeAttribute>(metadata);
			return rangeAttribute != null ? rangeAttribute.ErrorMessage : null;
		}
		public static T GetAttribute<T>(ModelMetadata metadata) where T : Attribute {
			if(metadata == null) return null;
			string propertyName = metadata.PropertyName;
			Type containerType = metadata.ContainerType;
			if(containerType == null || string.IsNullOrEmpty(propertyName)) return null;
			PropertyInfo propertyInfo = containerType.GetProperty(propertyName);
			return GetMetadataTypeAttribute<T>(propertyInfo);
		}
		static void PrepareEditorProperties(PropertyInfo propertyInfo, EditPropertiesBase editProperties) {
			if(propertyInfo == null)
				return;
			try {
				SetMaskSettings(GetMaskSettingsByProperties(editProperties), propertyInfo);
				SetDateRangeSettings(GetDateRangeSettingsByProperties(editProperties), propertyInfo);
				SetMinMaxValues(GetMinMaxValuesSetter(editProperties), propertyInfo);
				SetMaxLength(GetMaxLengthSetter(editProperties), propertyInfo);
			} finally { }
		}
		static void SetMinMaxValues(Action<RangeAttribute> action, PropertyInfo propertyInfo) {
			if(action != null) {
				var dateRangeAttribute = GetMetadataTypeAttribute<RangeAttribute>(propertyInfo);
				if(dateRangeAttribute != null)
					action(dateRangeAttribute);
			}
		}
		static void SetMaxLength(Action<int> action, PropertyInfo propertyInfo) {
			if(action != null) {
				var maxLengthAttribute = GetMetadataTypeAttribute(propertyInfo, MaxLengthAttributeTypeFullName);
				if(maxLengthAttribute == null) return;
				Expression propertyExpression = Expression.Property(Expression.Constant(maxLengthAttribute), LengthPropertyName);
				int lengthValue = Expression.Lambda<Func<Int32>>(propertyExpression).Compile()();
				action(lengthValue);
			}
		}
		static void SetMaskSettings(MaskSettings maskSettings, PropertyInfo propertyInfo) {
			if(maskSettings == null)
				return;
			var maskAttribute = GetMetadataTypeAttribute<MaskAttribute>(propertyInfo);
			if(maskAttribute != null)
				maskSettings.Assign(maskAttribute.MaskSettings);
		}
		static void SetDateRangeSettings(DateEditRangeSettings settings, PropertyInfo propertyInfo) {
			if(settings == null)
				return;
			var dateRangeAttribute = GetMetadataTypeAttribute<DateRangeAttribute>(propertyInfo);
			if(dateRangeAttribute != null)
				settings.Assign(dateRangeAttribute.DateEditRangeSettings);
		}
		static PropertyDescriptorCollection GetMetadataProperties(PropertyInfo propertyInfo) {
			var provider = new AssociatedMetadataTypeTypeDescriptionProvider(propertyInfo.DeclaringType);
			var typeDescriptor = provider.GetTypeDescriptor(propertyInfo.DeclaringType);
			return typeDescriptor.GetProperties();
		}
		static T GetMetadataTypeAttribute<T>(PropertyInfo propertyInfo) where T : class {
			foreach(PropertyDescriptor property in GetMetadataProperties(propertyInfo)) {
				if(property.Name == propertyInfo.Name)
					return property.Attributes.OfType<T>().FirstOrDefault();
			}
			return null;
		}
		static ValidationAttribute GetMetadataTypeAttribute(PropertyInfo propertyInfo, string fullAttributeTypeName) {
			foreach(PropertyDescriptor property in GetMetadataProperties(propertyInfo)) {
				if(property.Name == propertyInfo.Name)
					return property.Attributes.OfType<ValidationAttribute>().FirstOrDefault(attr => attr.GetType().FullName == fullAttributeTypeName);
			}
			return null;
		}
		static Action<int> GetMaxLengthSetter(EditPropertiesBase editProperties) {
			if(editProperties is TextBoxPropertiesBase)
				return (maxLength) => { ((TextBoxPropertiesBase)editProperties).MaxLength = maxLength; };
			if(editProperties is MemoProperties)
				return (maxLength) => { ((MemoProperties)editProperties).MaxLength = maxLength; };
			return null;
		}
		static Action<RangeAttribute> GetMinMaxValuesSetter(EditPropertiesBase editProperties) {
			if(editProperties is SpinEditProperties)
				return (range) => {
					if(range.OperandType == typeof(Int32)) {
						SpinEditProperties properties = (SpinEditProperties)editProperties;
						properties.MaxValue = (int)range.Maximum;
						properties.MinValue = (int)range.Minimum;
					}
				};
			if(editProperties is CalendarProperties) {
				return (range) => {
					if(range.OperandType == typeof(DateTime)) {
						CalendarProperties properties = (CalendarProperties)editProperties;
						TryToSetDateTime(dt => properties.MaxDate = dt, range.Maximum);
						TryToSetDateTime(dt => properties.MinDate = dt, range.Minimum);
					}
				};
			}
			if(editProperties is DateEditProperties) {
				return (range) => {
					if(range.OperandType == typeof(DateTime)) {
						DateEditProperties properties = (DateEditProperties)editProperties;
						TryToSetDateTime(dt => properties.MaxDate = dt, range.Maximum);
						TryToSetDateTime(dt => properties.MinDate = dt, range.Minimum);
					}
				};
			}
			return null;
		}
		static MaskSettings GetMaskSettingsByProperties(EditPropertiesBase editProperties) {
			if(editProperties is TextBoxProperties)
				return ((TextBoxProperties)editProperties).MaskSettings;
			if(editProperties is ButtonEditProperties)
				return ((ButtonEditProperties)editProperties).MaskSettings;
			return null;
		}
		static void TryToSetDateTime(Action<DateTime> dateTimeSetter, object dateTime) {
			DateTime tmp;
			if(DateTime.TryParse(dateTime as string, out tmp))
				dateTimeSetter(tmp);
		}
		static DateEditRangeSettings GetDateRangeSettingsByProperties(EditPropertiesBase editProperties){
			if(editProperties is DateEditProperties)
				return ((DateEditProperties)editProperties).DateRangeSettings;
			return null;
		}
	}
}
