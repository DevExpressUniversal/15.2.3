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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
namespace DevExpress.Utils.Design {
	public class NullableTypeConverter : NullableConverter {
		public NullableTypeConverter(Type type)
			: base(type) {
			properties = base.UnderlyingType.GetProperties();
			defaultValue = Activator.CreateInstance(base.UnderlyingType);
			defaultValueString = defaultValue.ToString();
			isSimpleProperty = properties.Length == 0;
			isStandardValuesSupport = CanGetStandartValues(isSimpleProperty);
			InitializeStandardValueCache();
			FillStandartValuesCache(isStandardValuesSupport);
		}
		PropertyInfo[] properties;
		string nullString = "Null";
		string defaultPopupString = "Default";
		string defaultValueString;
		object defaultValue;
		bool isStandardValuesSupport = true;
		bool getDefaultValuesFromCtor = false;
		bool isSimpleProperty = false;
		Dictionary<string, object> standardValuesCache;
		System.Collections.ICollection standardValuesCollection = null;
		void InitializeStandardValueCache() {
			standardValuesCache = new Dictionary<string, object>();
			standardValuesCache.Add(nullString, null);
		}
		void FillStandartValuesCache(bool isStandardValuesSupport) {
			if(!isStandardValuesSupport) return;
			if(standardValuesCollection == null) {
				standardValuesCache.Add(defaultPopupString, defaultValue);
				return;
			}
			foreach(var sv in standardValuesCollection) {
				if(sv == null)
					continue;
				standardValuesCache.Add(sv.ToString(), sv);
			}
		}
		bool CanGetStandartValues(bool isSimpleProperty) {
			if(isSimpleProperty) {
				getDefaultValuesFromCtor = true;
				standardValuesCollection = GetStandardValues();
				getDefaultValuesFromCtor = false;
				return !(standardValuesCollection == null);
			}
			return true;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			if(getDefaultValuesFromCtor)
				return true;
			return isStandardValuesSupport;
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(getDefaultValuesFromCtor)
				return base.GetStandardValues(context);
			return new System.ComponentModel.TypeConverter.StandardValuesCollection(standardValuesCache.Values.ToList());
		}
		bool flagFrom = false, flagTo = false;
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			Subscribe(context);
			if(flagTo)
				flagFrom = true;
			string stringValue = value as string;
			if(stringValue != null) {
				if(stringValue.ToUpper().Equals(nullString.ToUpper())) {
					return null;
				}
				if(isStandardValuesSupport) {
					object reternedValue;
					standardValuesCache.TryGetValue(stringValue, out reternedValue);
					if(reternedValue != null) {
						return reternedValue;
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			bool isPopUpFlag = false;
			if(!flagFrom && !flagTo)
				flagTo = true;
			if(flagFrom && flagTo) {
				isPopUpFlag = true;
				flagFrom = false;
			}
			var refreshGridEntryMethodInfo = context != null ? context.GetType().GetMethod("Refresh") : null;
			if(destinationType == typeof(string)) {
				if(value == null) {
					InvokeMethod(refreshGridEntryMethodInfo, context, null);
					return nullString;
				}
				string stringValue = value.ToString();
				if(stringValue.Equals(defaultValueString)) {
					if(!isSimpleProperty) {
						if(isPopUpFlag) {
							InvokeMethod(refreshGridEntryMethodInfo, context, null);
							return defaultPopupString;
						}
					}
				}
			}
			InvokeMethod(refreshGridEntryMethodInfo, context, null);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		void InvokeMethod(MethodInfo methodInfo, object obj, object[] parameters) {
			if(methodInfo == null) return;
			methodInfo.Invoke(obj, parameters);
		}
		int propertyGridSubscribeLocker = 0;
		void Subscribe(ITypeDescriptorContext context) {
			try {
				if(context == null || IsPopUpVisible(context)) return;
			}
			catch { return; }
			var ownerGridProperty = context.GetType().GetProperty("OwnerGrid");
			if(ownerGridProperty == null) return;
			PropertyGrid propertyGrid = ownerGridProperty.GetValue(context, null) as PropertyGrid;
			Subscribe(propertyGrid);
		}
		bool IsPopUpVisible(ITypeDescriptorContext context) {
			var propertyGridView_Property = context.GetType().GetProperty("GridEntryHost", BindingFlags.Instance | BindingFlags.NonPublic);
			var propertyGridView = propertyGridView_Property.GetValue(context, null);
			var dropDownListHolder_Field = propertyGridView.GetType().GetField("dropDownHolder", BindingFlags.NonPublic | BindingFlags.Instance);
			object holder = dropDownListHolder_Field.GetValue(propertyGridView);
			bool result = holder == null ? false : (bool)holder.GetType().GetProperty("Visible").GetValue(holder, null);			
			return result;
		}
		void PropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			PropertyGrid propertyGrid = sender as PropertyGrid;
			if(propertyGrid == null) return;
			UnSubscribe(propertyGrid);
			propertyGrid.Refresh();
		}
		void Subscribe(PropertyGrid propertyGrid) {
			if(propertyGrid == null || propertyGridSubscribeLocker != 0) return;
			propertyGrid.PropertyValueChanged += PropertyGrid_PropertyValueChanged;
			propertyGridSubscribeLocker++;
		}
		void UnSubscribe(PropertyGrid propertyGrid) {
			if(propertyGrid == null || propertyGridSubscribeLocker == 0) return;
			propertyGrid.PropertyValueChanged -= PropertyGrid_PropertyValueChanged;
			propertyGridSubscribeLocker--;
		}
	}
}
