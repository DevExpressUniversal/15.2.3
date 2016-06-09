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
using System.Linq;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Web;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using XRParameter = DevExpress.XtraReports.Parameters.Parameter;
namespace DevExpress.XtraReports.Web.Native.ParametersPanel {
	public class RemoteParametersProvider : IParametersProvider {
		readonly ReportParameter[] reportParameters;
		readonly ParametersEditorCreatorBase<ASPxEditBase> editorCreator;
		public RemoteParametersProvider(ReportParameter[] reportParameters, ParametersEditorCreatorBase<ASPxEditBase> editorCreator) {
			this.reportParameters = reportParameters;
			this.editorCreator = editorCreator;
		}
		#region IReportParametersProvider Members
		public ASPxParameterInfo[] GetParameters(Func<ParameterPath, ASPxEditBase, ASPxParameterInfo> map) {
			var result = reportParameters
				.Select(ConvertToParameterPath)
				.Select(x => map(x, CreateEditor(x)))
				.ToArray();
			for(int i = 0; i < result.Length; i++) {
				reportParameters[i].Value = result[i].Value;
			}
			return result;
		}
		#endregion
		ASPxEditBase CreateEditor(ParameterPath parameterPath) {
			return editorCreator.CreateEditorByParameter(parameterPath.Parameter);
		}
		static ParameterPath ConvertToParameterPath(ReportParameter parameter) {
			var result = new XRParameter {
				Name = parameter.Name,
				Description = parameter.Description,
				MultiValue = parameter.MultiValue,
				Value = parameter.Value,
				Type = ParameterValueTypeHelper.GetValueType(parameter.Value, parameter.MultiValue),
				Visible = parameter.Visible
			};
			if(parameter.LookUpValues != null) {
				var lookupValues = new StaticListLookUpSettings();
				lookupValues.LookUpValues.AddRange(parameter.LookUpValues);
				if(parameter.IsFilteredLookUpSettings) {
					lookupValues.FilterString = bool.TrueString;
				}
				result.LookUpSettings = lookupValues;
			}
			return new ParameterPath(result, parameter.Path);
		}
	}
}
