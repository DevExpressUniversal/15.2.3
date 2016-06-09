#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.Xpo;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.ExpressApp.ReportsV2 {
	public static class ReportParametersDataSourceInitializer {
		public static void SetupMultiValueParametersDataSources(XtraReport report) {
			SetupParametersDataSources(report, true);
		}
		public static void SetupParametersDataSources(XtraReport report) {
			SetupParametersDataSources(report, false);
		}
		private static void SetupParametersDataSources(XtraReport report, bool multiValueOnly) {
			List<Parameter> persistentParameters = new List<Parameter>();
			foreach(var parameter in report.Parameters) {
				if(parameter.LookUpSettings != null || (multiValueOnly && !parameter.MultiValue)) {
					continue;
				}
				if(parameter.Type.IsEnum) {
					parameter.LookUpSettings = CreateEnumLookupSettings(parameter.Type);
				}
				else {
					var typeInfo = XafTypesInfo.Instance.FindTypeInfo(parameter.Type);
					if(typeInfo != null && typeInfo.IsPersistent && typeInfo.KeyMember != null) {
						DataSourceBase dataSource = CreateParameterViewDataSource(typeInfo);
						report.ComponentStorage.Add(dataSource);
						parameter.LookUpSettings = CreateDataSourceLookupSettings(typeInfo, dataSource);
						if(parameter.Value != null) {
							parameter.Value = typeInfo.KeyMember.GetValue(parameter.Value);
						}
						if(parameter.MultiValue) {
							parameter.Type = typeInfo.KeyMember.MemberType;
						}
						else {
							parameter.Type = GetNullableType(typeInfo.KeyMember.MemberType);
						}
						persistentParameters.Add(parameter);
					}
				}
			}
			if(persistentParameters.Count > 0) {
				CriteriaOperator criteriaOperator = CriteriaOperator.Parse(report.FilterString);
				report.FilterString = PersistentParameterVisitor.Process(criteriaOperator, persistentParameters);
			}
		}
		private static StaticListLookUpSettings CreateEnumLookupSettings(Type parameterType) {
			StaticListLookUpSettings settings = new StaticListLookUpSettings();
			EnumDescriptor enumDescriptor = new EnumDescriptor(parameterType);
			foreach(var value in enumDescriptor.Values) {
				settings.LookUpValues.Add(new LookUpValue(value, enumDescriptor.GetCaption(value)));
			}
			return settings;
		}
		private static DynamicListLookUpSettings CreateDataSourceLookupSettings(ITypeInfo typeInfo, DataSourceBase dataSource) {
			DynamicListLookUpSettings settings = new DynamicListLookUpSettings();
			settings.DataSource = dataSource;
			settings.ValueMember = typeInfo.KeyMember.Name;
			settings.DisplayMember = typeInfo.DefaultMember.Name;
			return settings;
		}
		private static ViewDataSource CreateParameterViewDataSource(ITypeInfo typeInfo) {
			ViewDataSource dataSource = new ViewDataSource();
			dataSource.ObjectTypeName = typeInfo.FullName;
			dataSource.Sorting.Add(new SortProperty(typeInfo.DefaultMember.Name, Xpo.DB.SortingDirection.Ascending));
			dataSource.Properties.Add(new ViewProperty(typeInfo.KeyMember.Name, typeInfo.KeyMember.Name));
			dataSource.Properties.Add(new ViewProperty(typeInfo.DefaultMember.Name, typeInfo.DefaultMember.Name));
			return dataSource;
		}
		private static Type GetNullableType(Type type) {
			if(type.IsValueType)
				return typeof(Nullable<>).MakeGenericType(type);
			else
				return type;
		}
	}
}
