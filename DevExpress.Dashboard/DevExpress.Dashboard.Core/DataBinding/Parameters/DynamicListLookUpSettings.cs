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
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class DynamicListLookUpSettings : ParameterLookUpSettings {
		internal static void CheckTypes(DataFieldType fieldType, Type parameterType) {
			if (fieldType == DataFieldType.DateTime && parameterType != typeof(DateTime) ||
				parameterType == typeof(DateTime) && fieldType != DataFieldType.DateTime)
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.IncorrectParameterType));
		}
		internal const string xmlName = "DynamicListLookUpSettings";
		const string xmlDataSourceName = "DataSourceName", xmlValueMember = "ValueMember", xmlDisplayMember = "DisplayMember", xmlDataMember = "DataMember";
		string dataSourceName;
		string valueMember;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DynamicListLookUpSettingsDataSource"),
#endif
		DevExpress.DataAccess.Native.LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(TypeNames.LookUpSettingsDataSourceListConverter),
		RefreshProperties(RefreshProperties.Repaint),
		DefaultValue(null),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DynamicListLookUpSettings.DataSource")
		]
		public IDashboardDataSource DataSource { get; set; }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DynamicListLookUpSettingsDataMember"),
#endif
		DevExpress.DataAccess.Native.LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		Localizable(false),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.LookUpSettingsDataMemberListConverter),
		DefaultValue(null),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DynamicListLookUpSettings.DataMember")
		]
		public string DataMember { get; set; }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string DataSourceName { get { return DataSource != null ? DataSource.ComponentName : null; } set { dataSourceName = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DynamicListLookUpSettingsValueMember"),
#endif
		DevExpress.DataAccess.Native.LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		Editor(TypeNames.LookUpSettingsDataMemberSelectorEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false),
		DataFieldsBrowser(DataFieldsBrowserDisplayMode.Dimensions),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DynamicListLookUpSettings.ValueMember")
		]
		public string ValueMember { get { return valueMember; }
			set {
				if(valueMember != value) {
					valueMember = value;
					CheckValueMember(value);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DynamicListLookUpSettingsDisplayMember"),
#endif
		DevExpress.DataAccess.Native.LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
		Editor(TypeNames.LookUpSettingsDataMemberSelectorEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false),
		DataFieldsBrowser(DataFieldsBrowserDisplayMode.None),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.DynamicListLookUpSettings.DisplayMember")
		]
		public string DisplayMember { get; set; }		
		protected internal override bool SettingsEquals(object settings) {
			var lookupsettings = settings as DynamicListLookUpSettings;
			if(lookupsettings == null)
				return false;
			if(ValueMember != lookupsettings.ValueMember)
				return false;
			if(DisplayMember != lookupsettings.DisplayMember)
				return false;
			if(DataSource == null && lookupsettings.DataSource == null)
				return true;
			if(DataSource == null || lookupsettings.DataSource == null)
				return false;
			if(DataMember != lookupsettings.DataMember)
				return false;			
			return DataSource.ComponentName == lookupsettings.DataSource.ComponentName;
		}
		void CheckValueMember(string name) {
			if(DataSource != null && Parameter != null) {
				DataFieldType fieldType = DataSource.GetFieldType(name, DataMember);
				if(fieldType == DataFieldType.DateTime)
					Parameter.Type = typeof(DateTime);
				Type parameterType = Parameter.Type;
				CheckTypes(fieldType, parameterType);
			}
		}
		internal void CheckType(Type parameterType) {
			if (DataSource != null && Parameter != null) {
				DataFieldType fieldType = DataSource.GetFieldType(ValueMember, DataMember);				
				CheckTypes(fieldType, parameterType);
			}
		}
		protected internal override void DataBind(IEnumerable<IDashboardDataSource> dataSources) {
			base.DataBind(dataSources);
			DataSource = dataSources.FirstOrDefault(dataSource => dataSource.ComponentName == dataSourceName);
		}
		protected override void Assign(ParameterLookUpSettings lookupsettings) {
			base.Assign(lookupsettings);
			DynamicListLookUpSettings source = lookupsettings as DynamicListLookUpSettings;
			if(source == null)
				return;
			DataMember = source.DataMember;
			DataSource = source.DataSource;
			ValueMember = source.ValueMember;
			DisplayMember = source.DisplayMember;
		}
		protected override ParameterLookUpSettings CreateInstance() {
			return new DynamicListLookUpSettings();
		}
		protected internal override XElement SaveToXml() {
			XElement element = new XElement(xmlName);
			if(DataSource != null)
				element.Add(new XAttribute(xmlDataSourceName, DataSource.ComponentName));
			if(!string.IsNullOrEmpty(DataMember))
				element.Add(new XAttribute(xmlDataMember, DataMember));
			if(!string.IsNullOrEmpty(ValueMember))
				element.Add(new XAttribute(xmlValueMember, ValueMember));
			if(!string.IsNullOrEmpty(DisplayMember))
				element.Add(new XAttribute(xmlDisplayMember, DisplayMember));
			return element;
		}
		protected internal override void LoadFromXml(XElement element) {
			dataSourceName = element.GetAttributeValue(xmlDataSourceName);
			DataMember = element.GetAttributeValue(xmlDataMember);
			XAttribute valueMemberAttribute = element.Attribute(xmlValueMember);
			if(valueMemberAttribute != null)
				ValueMember = valueMemberAttribute.Value;
			XAttribute displayMemberAttribute = element.Attribute(xmlDisplayMember);
			if(displayMemberAttribute != null)
				DisplayMember = displayMemberAttribute.Value;
		}
	}
}
