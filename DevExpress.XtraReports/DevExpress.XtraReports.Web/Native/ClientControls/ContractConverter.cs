#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting.WebClientUIControl.DataContracts;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Web.ClientControls.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.DataContracts;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class ContractConverter {
		public static DataSourceInfo[] ConvertToContracts(IEnumerable<DataSourceJson> dataSources) {
			int i = 1;
			return dataSources
				.Select(x => new DataSourceInfo {
					Id = (i++).ToString(),
					Name = x.Name,
					IsSqlDataSource = x.IsSqlDataSource
				}).ToArray();
		}
		public static string[] GetJsonData(IEnumerable<DataSourceJson> dataSources) {
			return dataSources
				.Select(x => x.JsonData)
				.ToArray();
		}
		public static MenuAction[] ConvertToContracts(IEnumerable<ClientControlsMenuItemModel> menuItems) {
			return menuItems
				.Select(ConvertToContract)
				.ToArray();
		}
		public static string[] GetMenuItemsJSClickActions(IEnumerable<ClientControlsMenuItemModel> menuItems) {
			return menuItems
				.Select(x => x.JSClickAction)
				.ToArray();
		}
		public static TypedReportParameter GetTypedReportParameter(ParameterPath parameterPath, DataContext dataContext) {
			var parameter = parameterPath.Parameter;
			LookUpValueCollection lookUpValues = parameter.LookUpSettings == null
				? null
				: (LookUpHelper.GetLookUpValues(parameter.LookUpSettings, dataContext) ?? new LookUpValueCollection());
			if(lookUpValues != null) {
				foreach(LookUpValue lookUpValue in lookUpValues) {
					lookUpValue.Value = ToJsParameterValue(lookUpValue.Value);
					lookUpValue.Description = lookUpValue.RealDescription;
				}
			}
			return new TypedReportParameter {
				Description = parameter.Description,
				Path = parameterPath.Path,
				Name = parameter.Name,
				MultiValue = parameter.MultiValue,
				Value = parameter.MultiValue ? ToJsParameterMultiValue(parameter.Value as IEnumerable, parameter.Type) : ToJsParameterValue(parameter.Value),
				TypeString = parameter.Type.FullName,
				Visible = parameter.Visible,
				IsFilteredLookUpSettings = parameter.HasCascadeLookUpSettings(),
				LookUpValues = lookUpValues
			};
		}
		public static object ToJsParameterValue(object value) {
			return value is DateTime ? SerializeDateTime((DateTime)value) : value;
		}
		public static WizardDataConnection[] ConvertDataConnectionsToContracts(Dictionary<string, string> connectionStrings) {
			var result = new WizardDataConnection[connectionStrings.Count];
			var i = 0;
			foreach(var pair in connectionStrings) {
				result[i] = new WizardDataConnection {
					Name = pair.Key,
					Description = pair.Value
				};
				i++;
			}
			return result;
		}
		static Array ToJsParameterMultiValue(IEnumerable values, Type type) {
			if(values == null) {
				return null;
			}
			if(type == typeof(DateTime)) {
				type = typeof(string);
			}
			var result = new ArrayList();
			foreach(var value in values) {
				result.Add(ToJsParameterValue(value));
			}
			return result.ToArray(type);
		}
		static string SerializeDateTime(DateTime date) {
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(DateTime));
			return converter == null || !converter.CanConvertTo(typeof(string)) ? "" : (string)converter.ConvertToInvariantString(date);
		}
		static MenuAction ConvertToContract(ClientControlsMenuItemModel menuItem) {
			return new MenuAction {
				Text = menuItem.Text,
				ImageClassName = menuItem.ImageClassName,
				Disabled = menuItem.Disabled,
				Visible = menuItem.Visible,
				HotKey = menuItem.HotKey,
				Container = menuItem.Container.ToString().ToLowerInvariant(),
				HasSeparator = menuItem.HasSeparator
			};
		}
	}
}
