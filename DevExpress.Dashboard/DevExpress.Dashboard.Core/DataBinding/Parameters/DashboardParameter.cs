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
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Drawing.Design;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Browsing;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.PivotGrid.OLAP;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using System.Collections;
using System.Globalization;
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon {
	public class DashboardParameter : Parameter, INameContainer, ISupportPrefix {
		private ParameterLookUpSettings lookUpSettings;
		const string xmlDescription = "Description";
		const string xmlVisible = "Visible";
		const string xmlAllowMultiselect = "AllowMultiselect";
		const bool DefaultVisible = true;
		const bool DefaultAllowMultiselect = false;
		bool visible = DefaultVisible;
		bool allowMultiselect = DefaultAllowMultiselect;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardParameterVisible"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridBehaviorCategoryName),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(DefaultVisible),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DashboardParameter.Visible")
		]
		public bool Visible { get { return visible; } set { visible = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardParameterDescription"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DashboardParameter.Description"),
		DefaultValue(null),
		Localizable(true)
		]
		public string Description { get; set; }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardParameterAllowMultiselect"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridBehaviorCategoryName),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(DefaultAllowMultiselect),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DashboardParameter.AllowMultiselect")
		]
		public bool AllowMultiselect { get { return allowMultiselect; } set { allowMultiselect = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardParameterLookUpSettings"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		TypeConverter(TypeNames.ParameterLookUpSettingsConverter),
		Editor(TypeNames.ParameterLookUpSettingsEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DashboardParameter.LookUpSettings")
		]
		public ParameterLookUpSettings LookUpSettings {
			get { return lookUpSettings; }
			set {
				if(lookUpSettings != value) {
					if(lookUpSettings != null)
						lookUpSettings.Parameter = null;
					lookUpSettings = value;
					if(lookUpSettings != null)
						lookUpSettings.Parameter = this;
				}
			}
		}
		internal string DisplayText { get { return !string.IsNullOrEmpty(Description) ? Description : Name; } }
		public DashboardParameter() {
		}
		public DashboardParameter(string name, Type type, object value)
			: base(name, type, value) {
		}
		public DashboardParameter(string name, Type type, object value, string description, bool visible, ParameterLookUpSettings lookupsettings)
			: base(name, type, value) {
			Description = description;
			Visible = visible;
			LookUpSettings = lookupsettings;
		}
		internal void DataBind(IEnumerable<IDashboardDataSource> dataSources) {
			if(LookUpSettings != null)
				LookUpSettings.DataBind(dataSources);
		}
		internal DashboardParameterViewModel CreateViewModel(IActualParametersProvider provider) {
			DashboardParameterViewModel parameterModel = new DashboardParameterViewModel();
			parameterModel.Name = Name;
			parameterModel.Description = Description;
			parameterModel.AllowMultiselect = AllowMultiselect;
			parameterModel.Type = GetParameterType();
			parameterModel.ContainsDisplayMember = false;
			parameterModel.AllowNull = AllowNull;
			if(LookUpSettings != null) {
				CreateModelValuesFromLookupSettings(parameterModel, provider);
			}
			if(AllowMultiselect && Value != null && parameterModel.Values != null && parameterModel.Values.Count > 0)
				switch(parameterModel.Type) {
					case ParameterType.Bool:
						parameterModel.DefaultValue = new bool[] { (bool)Value }; break;
					case ParameterType.DateTime:
						parameterModel.DefaultValue = new DateTime[] { (DateTime)Value }; break;
					case ParameterType.Float:
						parameterModel.DefaultValue = new double[] { Convert.ToDouble(Value) }; break;
					case ParameterType.Guid:
						parameterModel.DefaultValue = new Guid[] { (Guid)Value }; break;
					case ParameterType.Int:
						parameterModel.DefaultValue = new Int64[] { Convert.ToInt64(Value) }; break;
					default:
						if(string.IsNullOrWhiteSpace((string)Value))
							parameterModel.DefaultValue = new string[]{};
						else
							parameterModel.DefaultValue = new string[] { (string)Value };
						break;
				}
			else
				switch(parameterModel.Type) {
					case ParameterType.Float:
						parameterModel.DefaultValue = Convert.ToDouble(Value); break;
					case ParameterType.Int:
						parameterModel.DefaultValue = Convert.ToInt64(Value); break;
					default:
						parameterModel.DefaultValue = Value; break;
				}
			parameterModel.Visible = Visible;
			return parameterModel;
		}
		ParameterType GetParameterType() {
			if((Type == typeof(Int32)) || (Type == typeof(Int16)) || (Type == typeof(Int64)))
				return ParameterType.Int;
			else if((Type == typeof(float)) || (Type == typeof(double)) || (Type == typeof(decimal)))
				return ParameterType.Float;
			else if(Type == typeof(bool))
				return ParameterType.Bool;
			else if(Type == typeof(DateTime))
				return ParameterType.DateTime;
			else if(Type == typeof(Guid))
				return ParameterType.Guid;
			else
				return ParameterType.String;
		}
		Type GetParameterModelType() {
			if(Type == typeof(Int16) || Type == typeof(Int32))
				return typeof(Int64);
			else if(Type == typeof(float) || Type == typeof(decimal))
				return typeof(double);
			else
				return Type;
		}
		object ConvertValue(object value) {
			try {
				return ConvertValue(value, value.GetType(), GetParameterModelType());
			}
			catch {
				return null;
			}
		}
		object ConvertValue(object value, Type fromType, Type toType) {
			if(fromType == toType)
				return value;
			TypeConverter converter = TypeDescriptor.GetConverter(toType);
			try {
				if(converter.CanConvertFrom(fromType))
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
			}
			catch { }
			converter = TypeDescriptor.GetConverter(fromType);
			if(converter.CanConvertTo(toType))
				return converter.ConvertTo(null, CultureInfo.InvariantCulture, value, toType);
			return Convert.ChangeType(value, toType, CultureInfo.InvariantCulture);
		}
		bool NullValues(ParameterValueViewModel i) {
			return i.Value == null;
		}
		void CreateModelValuesFromLookupSettings(DashboardParameterViewModel parameterModel, IActualParametersProvider provider) {
			List<ParameterValueViewModel> modelValues = null;
			var staticLookup = LookUpSettings as StaticListLookUpSettings;
			modelValues = new List<ParameterValueViewModel>();
			if(staticLookup != null) {
				foreach(string value in staticLookup.Values)
					modelValues.Add(new ParameterValueViewModel() {
						Value = value
					});
			} else {
				var dynamicLookup = LookUpSettings as DynamicListLookUpSettings;
				if(dynamicLookup != null && dynamicLookup.DataSource != null && !string.IsNullOrEmpty(dynamicLookup.ValueMember) && dynamicLookup.DataSource.ContainsField(dynamicLookup.ValueMember, dynamicLookup.DataMember)) {
					modelValues = dynamicLookup.DataSource.GetParameterValues(dynamicLookup.ValueMember, dynamicLookup.DisplayMember, dynamicLookup.DataMember, provider);
					parameterModel.ContainsDisplayMember = dynamicLookup.DataSource.ContainsParametersDisplayMember(dynamicLookup.ValueMember, dynamicLookup.DisplayMember, dynamicLookup.DataMember);
				}
			}
			foreach(ParameterValueViewModel i in modelValues) {
				i.Value = ConvertValue(i.Value);
			}
			modelValues.RemoveAll(NullValues);
			parameterModel.Values = modelValues;
		}
		protected override void Assign(Parameter parameter) {
			base.Assign(parameter);
			DashboardParameter source = parameter as DashboardParameter;
			if(source == null)
				return;
			AllowNull = source.AllowNull;
			Description = source.Description;
			AllowMultiselect = source.AllowMultiselect;
			Visible = source.Visible;
			if(source.LookUpSettings != null)
				LookUpSettings = source.LookUpSettings.Clone();
			else
				LookUpSettings = null;
		}
		protected override Parameter CreateInstance() {
			return new DashboardParameter();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(!string.IsNullOrEmpty(Description))
				element.Add(new XAttribute(xmlDescription, Description));
			if(visible != DefaultVisible)
				element.Add(new XAttribute(xmlVisible, visible));
			if(LookUpSettings != null)
				element.Add(LookUpSettings.SaveToXml());
			if(AllowMultiselect != DefaultAllowMultiselect)
				element.Add(new XAttribute(xmlAllowMultiselect, allowMultiselect));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XAttribute descriptionAttribute = element.Attribute(xmlDescription);
			if(descriptionAttribute != null)
				Description = descriptionAttribute.Value;
			XAttribute visibleAttribute = element.Attribute(xmlVisible);
			if(visibleAttribute != null)
				visible = XmlHelper.FromString<bool>(visibleAttribute.Value);
			XAttribute allowMultiselectAttribute = element.Attribute(xmlAllowMultiselect);
			if(allowMultiselectAttribute != null)
				allowMultiselect = XmlHelper.FromString<bool>(allowMultiselectAttribute.Value);
			XElement settings = null;
			settings = element.Element(StaticListLookUpSettings.xmlName);
			if(settings != null)
				LookUpSettings = new StaticListLookUpSettings();
			else {
				settings = element.Element(DynamicListLookUpSettings.xmlName);
				if(settings != null)
					LookUpSettings = new DynamicListLookUpSettings();
			}
			if(settings != null)
				LookUpSettings.LoadFromXml(settings);
		}
		protected override void CheckType(Type type) {
			base.CheckType(type);
			DynamicListLookUpSettings dynamicLookup = lookUpSettings as DynamicListLookUpSettings;
			if(dynamicLookup != null) {
				dynamicLookup.CheckType(type);
			}
		}
		#region INameContainer Members
		 event EventHandler<NameChangingEventArgs> INameContainer.NameChanging { add { NameChanging += value; } remove { NameChanging -= value; } }
		#endregion
		#region ISupportPrefix Members
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.NewParameterNamePrefix); } }
		#endregion
	}
}
