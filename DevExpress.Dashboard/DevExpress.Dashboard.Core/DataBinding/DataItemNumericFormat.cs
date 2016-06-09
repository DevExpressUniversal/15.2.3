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

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum DataItemNumericFormatType { Auto, General, Number, Currency, Scientific, Percent }
	public enum DataItemNumericUnit { Auto, Ones, Thousands, Millions, Billions }
	public class DataItemNumericFormat {
		internal const string XmlNumericFormat = "NumericFormat";
		const string xmlFormatType = "FormatType";
		const string xmlPrecision = "Precision";
		const string xmlUnit = "Unit";
		const string xmlIncludeGroupSeparator = "IncludeGroupSeparator";
		const string xmlCurrencyCultureName = "CurrencyCultureName";
		const DataItemNumericFormatType DefaultFormatType = DataItemNumericFormatType.Auto;
		const int DefaultPrecision = 2;
		const DataItemNumericUnit DefaultUnit = DataItemNumericUnit.Auto;
		const bool DefaultIncludeGroupSeparator = false;
		const string DefaultCurrencyCultureName = null;
		readonly DataItem dataItem;
		readonly Locker locker = new Locker();
		DataItemNumericFormatType formatType = DefaultFormatType;
		int precision = DefaultPrecision;
		DataItemNumericUnit unit = DefaultUnit;
		bool includeGroupSeparator = DefaultIncludeGroupSeparator;
		string currencyCultureName = DefaultCurrencyCultureName;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormatFormatType"),
#endif
		DefaultValue(DefaultFormatType)
		]
		public DataItemNumericFormatType FormatType { 
			get { return formatType; }
			set {
				if (value != formatType) {
					formatType = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormatPrecision"),
#endif
		DefaultValue(DefaultPrecision)
		]
		public int Precision {
			get { return precision; }
			set {
				if (value < 0)
					throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectNumericPrecision));
				if (value != precision) {
					precision = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormatUnit"),
#endif
		DefaultValue(DefaultUnit)
		]
		public DataItemNumericUnit Unit {
			get { return unit; }
			set{
				if (value != unit) {
					unit = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormatIncludeGroupSeparator"),
#endif
		DefaultValue(DefaultIncludeGroupSeparator)
		]
		public bool IncludeGroupSeparator {
			get { return includeGroupSeparator; }
			set {
				if (value != includeGroupSeparator) {
					includeGroupSeparator = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataItemNumericFormatCurrencyCultureName"),
#endif
		Editor(TypeNames.CurrencyEditor, typeof(UITypeEditor)),
		DefaultValue(DefaultCurrencyCultureName),
		Localizable(false)
		]
		public string CurrencyCultureName {
			get { return currencyCultureName; }
			set {
				if (value != currencyCultureName) {
					SetCurrencyCultureName(value, !Loading);
					OnChanged();
				}
			}
		}
		bool Loading { get { return dataItem != null && dataItem.Loading; } }
		internal DataItemNumericFormat()
			: this(null) {
		}
		internal DataItemNumericFormat(DataItem dataItem) {
			this.dataItem = dataItem;
		}
		public void BeginUpdate() {
			locker.Lock();
		}
		public void EndUpdate() {
			locker.Unlock();
			OnChanged();
		}
		internal NumericFormatViewModel CreateViewModel(bool forcePlusSign, NumericFormatType defaultFormatType, string defaultCurrencyCultureName) {
			string actualCurrencyCultureName = currencyCultureName ?? defaultCurrencyCultureName;
			if(formatType == DataItemNumericFormatType.Auto)
				return new NumericFormatViewModel(defaultFormatType, 0, DataItemNumericUnit.Auto, false, false, 3, actualCurrencyCultureName);
			return new NumericFormatViewModel((NumericFormatType)formatType, precision, unit, includeGroupSeparator, forcePlusSign, unit == DataItemNumericUnit.Auto ? 3 : 0, 
				actualCurrencyCultureName);		
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(XmlNumericFormat);
			if(FormatType != DefaultFormatType)
				element.Add(new XAttribute(xmlFormatType, formatType));
			if(Precision != DefaultPrecision)
				element.Add(new XAttribute(xmlPrecision, precision));
			if(Unit != DefaultUnit)
				element.Add(new XAttribute(xmlUnit, unit));
			if(IncludeGroupSeparator != DefaultIncludeGroupSeparator)
				element.Add(new XAttribute(xmlIncludeGroupSeparator, includeGroupSeparator));
			if(CurrencyCultureName != DefaultCurrencyCultureName)
				element.Add(new XAttribute(xmlCurrencyCultureName, currencyCultureName));
			return element;
		}
		internal void LoadFromXml(XElement element) {
			string attribute = XmlHelper.GetAttributeValue(element, xmlFormatType);
			if (!String.IsNullOrEmpty(attribute))
				formatType = XmlHelper.FromString<DataItemNumericFormatType>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlPrecision);
			if (!String.IsNullOrEmpty(attribute))
				precision = XmlHelper.FromString<int>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlUnit);
			if (!String.IsNullOrEmpty(attribute))
				unit = XmlHelper.FromString<DataItemNumericUnit>(attribute);
			attribute = XmlHelper.GetAttributeValue(element, xmlIncludeGroupSeparator);
			if (!String.IsNullOrEmpty(attribute))
				includeGroupSeparator = XmlHelper.FromString<bool>(attribute);
			SetCurrencyCultureName(XmlHelper.GetAttributeValue(element, xmlCurrencyCultureName), false);
		}
		internal bool ShouldSerialize() {
			return FormatType != DefaultFormatType || Precision != DefaultPrecision || Unit != DefaultUnit || IncludeGroupSeparator != DefaultIncludeGroupSeparator ||
				CurrencyCultureName != DefaultCurrencyCultureName;
		}
		void OnChanged() {
			if(dataItem != null && !locker.IsLocked)
				dataItem.OnChanged();
		}
		void SetCurrencyCultureName(string currencyCultureName, bool throwException) {
			if(currencyCultureName == null) {
				this.currencyCultureName = null;
			}
			else {
				try {
					CultureInfoExtensions.CreateSpecificCulture(currencyCultureName);
				}
				catch(Exception e) {
					if(throwException) {
						throw new InvalidCultureNameException(currencyCultureName, e);
					}
					this.currencyCultureName = null;
				}
				this.currencyCultureName = currencyCultureName;
			}
		}
		internal void Assign(DataItemNumericFormat source) {
			FormatType = source.FormatType;
			Precision = source.Precision;
			Unit = source.Unit;
			IncludeGroupSeparator = source.IncludeGroupSeparator;
			CurrencyCultureName = source.CurrencyCultureName;
		}
	}
}
