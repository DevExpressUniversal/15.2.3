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
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess {
	[
#if !SILVERLIGHT
TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI),
#endif
]
	public class Parameter : IParameter, INamedItem {
		static string GetParameterName(string parameterName) {
			if(string.IsNullOrEmpty(parameterName))
				return parameterName;
			StringBuilder builder = new StringBuilder();
			foreach(char ch in parameterName) {
				if(CriteriaLexer.CanContinueColumn(ch))
					builder.Append(ch);
			}
			return builder.ToString();
		}
		internal const string xmlName = "Parameter";
		const bool DefaultAllowNull = false;
		const string xmlNameProperty = "Name";
		const string xmlType = "Type";
		const string xmlValue = "Value";
		const string xmlAllowNull = "AllowNull";
		protected event EventHandler<NameChangingEventArgs> NameChanging;
		Type type;
		object value;
		string name;
		bool allowNull = DefaultAllowNull;
		#region IParameter Members
		[
		LocalizableCategory(DataAccessStringId.PropertyGridBehaviorCategoryName),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(DefaultAllowNull),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DashboardParameter.AllowNull")
		]
		public bool AllowNull { get { return allowNull; } set { allowNull = value; } }		
		object DefaultValue {
			get {
				if(type == typeof(DateTime))
					return new DateTime(2000, 1, 1);
				return ParameterHelper.GetDefaultValue(Type);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ParameterName"),
#endif
		DefaultValue(null),
		LocalizableCategory(DataAccessStringId.PropertyGridDesignCategoryName),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DataAccess.Parameter.Name")
		]
		public string Name {
			get { return name; }
			set {
				string newName = GetParameterName(value);
				if(name == newName)
					return;
				if(newName != value)
					throw new InvalidNameException(string.Format(DataAccessLocalizer.GetString(DataAccessStringId.MessageWrongCharacterParameterName), value));
				if(NameChanging != null)
					NameChanging(this, new NameChangingEventArgs(newName));				
				name = newName;
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ParameterType"),
#endif
#if !SILVERLIGHT
		TypeConverter("DevExpress.XtraReports.Design.ParameterTypeConverter," + AssemblyInfo.SRAssemblyUtilsUI),
#endif
		RefreshProperties(RefreshProperties.All),
		LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		DefaultValue(null),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DataAccess.Parameter.Type")
		]
		public Type Type {
			get {
				return type ?? typeof(String);
			}
			set {
				if(type != value) {
					CheckType(value);
					type = value;
				}
				if(ParameterHelper.ShouldConvertValue(this.Value, this.Type))
					this.value = ParameterHelper.ConvertFrom(this.Value, this.Type, this.DefaultValue);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ParameterValue"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		DefaultValue(null),
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DataAccess.Parameter.Value")
		]
		public virtual object Value {
			get { return AllowNull ? value : value ?? DefaultValue; }
			set { this.value = value; }
		}
		#endregion
		public Parameter()
			: this(string.Empty, null, null) {
		}
		public Parameter(string name, Type type, object value)
			: this(name, type, value, DefaultAllowNull) {
		}
		public Parameter(string name, Type type, object value, bool allowNull) {
			Name = name;
			Type = type;
			Value = value;
			this.allowNull = allowNull;
		}
		public Parameter Clone() {
			Parameter parameter = CreateInstance();
			parameter.Assign(this);
			return parameter;
		}
		protected virtual void CheckType(Type type) {
		}
		protected virtual void Assign(Parameter parameter) {
			Name = parameter.Name;
			Type = parameter.Type;
			Value = parameter.Value;
			AllowNull = parameter.AllowNull;
		}
		protected virtual Parameter CreateInstance() {
			return new Parameter();
		}
		protected internal virtual void SaveToXml(XElement element) {
			if(!string.IsNullOrEmpty(Name))
				element.Add(new XAttribute(xmlNameProperty, Name));
			if(type != typeof(string) && type != null)
				element.Add(new XAttribute(xmlType, Type.AssemblyQualifiedName));
			if(value != null)
				element.Add(new XAttribute(xmlValue, Value));
			if(allowNull != DefaultAllowNull)
				element.Add(new XAttribute(xmlAllowNull, allowNull));
		}
		protected internal virtual void LoadFromXml(XElement element) {
			XAttribute nameAttribute = element.Attribute(xmlNameProperty);
			if(nameAttribute != null)
				Name = GetParameterName(nameAttribute.Value);
			XAttribute typeAttribute = element.Attribute(xmlType);
			if(typeAttribute != null)
				Type = Type.GetType(typeAttribute.Value, false);
			XAttribute valueAttribute = element.Attribute(xmlValue);
			if(valueAttribute != null)
				Value = XmlHelperBase.FromString(this.Type, valueAttribute.Value);
			string allowNullString = XmlHelper.GetAttributeValue(element, xmlAllowNull);
			if(!String.IsNullOrEmpty(allowNullString))
				allowNull = XmlHelper.FromString<bool>(allowNullString);
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class ParametersEqualityComparer : IEqualityComparer<IParameter> {		
		public bool Equals(IParameter param1, IParameter param2) {
			if(param1 == null && param2 == null)
				return true;
			if(param1 == null || param2 == null)
				return false;
			if(param1.Name != param2.Name)
				return false;
			if(!Equals(param1.Type, param2.Type))
				return false;
			if(!Equals(param1.Value, param2.Value))
				return false;
			return true;
		}
		public int GetHashCode(IParameter parameter) {
			return parameter.Name.GetHashCode();
		}
	}
}
