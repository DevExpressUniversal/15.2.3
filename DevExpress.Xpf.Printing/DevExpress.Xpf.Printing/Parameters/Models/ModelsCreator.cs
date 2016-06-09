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
using DevExpress.Data.Browsing;
using DevExpress.Utils;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.Xpf.Printing.Parameters.Models.Native {
	public class ModelsCreator {
		#region remote mode
		public static List<ParameterModel> CreateParameterModels(IEnumerable<IClientParameter> parameters) {
			var result = new List<ParameterModel>();
			if(parameters == null || parameters.Count() == 0)
				return result;
			parameters.ForEach(x => {
				result.Add(CreateParameterModel(x));
			});
			return result;
		}
		static ParameterModel CreateParameterModel(IClientParameter parameter) {
			var clientParameter = ((ClientParameter)parameter);
			var lookUpValues = clientParameter.OriginalParameter.LookUpSettings is StaticListLookUpSettings
				? ((StaticListLookUpSettings)clientParameter.OriginalParameter.LookUpSettings).LookUpValues
				: null;
			var parameterModel = ParameterModel.CreateParameterModel(clientParameter.OriginalParameter, lookUpValues);
			parameterModel.Path = clientParameter.Path;
			parameterModel.IsFilteredLookUpSettings = clientParameter.IsFilteredLookUpSettings;
			return parameterModel;
		}
		#endregion
		#region local mode
		public static List<ParameterModel> CreateParameterModels(IEnumerable<Parameter> parameters, DataContext dataContext) {
			var result = new List<ParameterModel>();
			if(parameters == null || parameters.Count() == 0)
				return result;
			var valueProvider = new ParameterValueProvider(result);
			parameters.ForEach(x => result.Add(CreateParameterModel(x, dataContext, valueProvider)));
			return result;
		}
		static ParameterModel CreateParameterModel(Parameter parameter, DataContext dataContext, ParameterValueProvider valueProvider) {
			var lookUpSettings = GetLookUpSettings(parameter);
			var lookUpValues = LookUpHelper.GetLookUpValues(lookUpSettings, dataContext, valueProvider);
			var model = ParameterModel.CreateParameterModel(parameter, lookUpValues);
			if(lookUpSettings != null)
				model.IsFilteredLookUpSettings = !string.IsNullOrEmpty(lookUpSettings.FilterString);
			return model;
		}
		static LookUpSettings GetLookUpSettings(Parameter parameter) {
			if(parameter.LookUpSettings!=null)
				return parameter.LookUpSettings;
			if(parameter.Type.IsEnum) {
				var staticList = new StaticListLookUpSettings();
				var enumValues = Enum.GetValues(parameter.Type);
				var typeConverter = TypeDescriptor.GetConverter(parameter.Type);
				foreach(var enumValue in enumValues) {
					var description = EnumExtensions.GetEnumItemDisplayText(enumValue);
					staticList.LookUpValues.Add(new LookUpValue(enumValue, description));
				}
				return staticList;
			}
			return null;
		}
		#endregion
	}
}
