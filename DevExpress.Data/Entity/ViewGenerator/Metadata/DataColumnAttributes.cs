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
using System.Linq;
using System.Security;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Entity.Model {
	[SecuritySafeCritical]
	public class DataColumnAttributes {
		readonly AttributeCollection attributes;
		readonly Lazy<TypeConverter> typeConverterValue;
		protected readonly Func<TypeConverter> getTypeConverterCallback;
		readonly Lazy<DisplayAttribute> displayAttributeValue;
		readonly Lazy<DisplayFormatAttribute> displayFormatAttributeValue;
		readonly Lazy<DataType> dataTypeValue;
		readonly Lazy<int> maxLengthValue;
		readonly Lazy<int> maxLength2Value;
		readonly Lazy<bool> requiredValue;
		readonly Lazy<bool?> isReadOnlyValue;
		readonly Lazy<bool?> allowEditValue;
		public DataType DataTypeValue { get { return dataTypeValue.Value; } }
		public bool? AllowEdit { get { return allowEditValue.Value; } }
		public bool? IsReadOnly { get { return isReadOnlyValue.Value; } }
#if !SL
		readonly Lazy<bool> allowScaffoldingValue;
		public bool AllowScaffolding { get { return allowScaffoldingValue.Value; } }
#else
		public bool AllowScaffolding { get { return true; } }
#endif
		public int MaxLengthValue { get { return maxLengthValue.Value; } }
		public int MaxLength2Value { get { return maxLength2Value.Value; } }
		public bool RequiredValue { get { return requiredValue.Value; } }
		public DisplayAttribute DisplayAttributeValue { get { return displayAttributeValue.Value; } }
		public TypeConverter TypeConverter { get { return typeConverterValue.Value; } }
		public bool? AutoGenerateField { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetAutoGenerateField()); } }
		public string Description { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetDescription()); } }
		public string GroupName { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetGroupName()); } }
		public int? Order { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetOrder()); } }
		public string Name { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetName()); } }
		public string ShortName { get { return ReadAttributeProperty(displayAttributeValue, x => x.GetShortName()); } }
		public bool ApplyFormatInEditMode { get { return ReadAttributeProperty(displayFormatAttributeValue, x => x.ApplyFormatInEditMode); } }
		public bool ConvertEmptyStringToNull { get { return ReadAttributeProperty(displayFormatAttributeValue, x => x.ConvertEmptyStringToNull, true); } }
		public string DataFormatString { get { return ReadAttributeProperty(displayFormatAttributeValue, x => x.DataFormatString); } }
		public string NullDisplayText { get { return ReadAttributeProperty(displayFormatAttributeValue, x => x.NullDisplayText); } }
		readonly static Type maxLengthAttributeType;
#if DEBUGTEST
		public static Type TestMaxLengthAttributeType;
#endif
		internal static Type MaxLengthAttributeType {
			get {
#if DEBUGTEST
				if(TestMaxLengthAttributeType != null)
					return TestMaxLengthAttributeType;
#endif
				return maxLengthAttributeType;
			}
		}
		static DataColumnAttributes() {
			maxLengthAttributeType = typeof(ValidationAttribute).Assembly.GetType(typeof(ValidationAttribute).Namespace + ".MaxLengthAttribute", false);
		}
		public DataColumnAttributes(AttributeCollection attributes, Func<TypeConverter> getTypeConverterCallback = null) {
			this.attributes = attributes;
			this.getTypeConverterCallback = getTypeConverterCallback;
			this.typeConverterValue = new Lazy<TypeConverter>(getTypeConverterCallback ?? (() => null));
#if !SL
			allowScaffoldingValue = ReadAttributeProperty<ScaffoldColumnAttribute, bool>(x => x.Scaffold, true);
#endif
			isReadOnlyValue = ReadAttributeProperty<ReadOnlyAttribute, bool?>(x => x.IsReadOnly);
			allowEditValue = ReadAttributeProperty<EditableAttribute, bool?>(x => x.AllowEdit);
			maxLengthValue = ReadAttributeProperty<Attribute, int>(MaxLengthAttributeType, x => (int)TypeDescriptor.GetProperties(x)["Length"].GetValue(x));
			maxLength2Value = ReadAttributeProperty<StringLengthAttribute, int>(x => x.MaximumLength);
			displayFormatAttributeValue = ReadAttributeProperty<DisplayFormatAttribute, DisplayFormatAttribute>(x => x);
			requiredValue = ReadAttributeProperty<RequiredAttribute, bool>(x => true);
			displayAttributeValue = ReadAttributeProperty<DisplayAttribute, DisplayAttribute>(x => x);
			dataTypeValue = ReadAttributeProperty<DataTypeAttribute, DataType>(x => x.DataType);
		}
		public DataColumnAttributes AddAttributes(IEnumerable<Attribute> newAttributes) {
			return new DataColumnAttributes(CombineAttributes(attributes, newAttributes), getTypeConverterCallback);
		}
		static AttributeCollection CombineAttributes(AttributeCollection collection, IEnumerable<Attribute> newAttributes) {
			return new AttributeCollection(collection.Cast<Attribute>().Concat(newAttributes).ToArray());
		}
		public TValue ReadAttributeProperty<TAttribute, TValue>(Lazy<TAttribute> lazyAttributeValue, Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue)) where TAttribute : Attribute {
			if(lazyAttributeValue.Value != null)
				return reader(lazyAttributeValue.Value);
			return defaultValue;
		}
		public Lazy<TValue> ReadAttributeProperty<TAttribute, TValue>(Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue)) where TAttribute : Attribute {
			return ReadAttributeProperty<TAttribute, TValue>(typeof(TAttribute), reader, defaultValue);
		}
		public Lazy<TValue> ReadAttributeProperty<TAttribute, TValue>(Type attributeType, Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue)) where TAttribute : Attribute {
			return new Lazy<TValue>(() =>
			{
				return DevExpress.Data.Utils.AnnotationAttributes.Reader.Read<TAttribute, TValue>(attributeType, attributes, reader, defaultValue);
			});
		}
		public TValue[] GetAttributeValues<TAttribute, TValue>(Func<TAttribute, TValue> reader) where TAttribute : Attribute {
			return DevExpress.Data.Utils.AnnotationAttributes.Reader.Read<TAttribute, TValue>(attributes, reader);
		}
		public static TValue GetAttributeValue<TAttribute, TValue>(Type type, Func<TAttribute, TValue> reader, TValue defaultValue = default(TValue)) where TAttribute : Attribute {
			return DevExpress.Data.Utils.AnnotationAttributes.Reader.Read<TAttribute, TValue>(type, reader, defaultValue);
		}
	}
}
