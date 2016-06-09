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
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Service.Native {
	public static class ReportServiceHelper {
		public static float ToDpi(this ReportUnit reportUnit) {
			return reportUnit.Equals(ReportUnit.HundredthsOfAnInch)
				? GraphicsDpi.HundredthsOfAnInch
				: GraphicsDpi.TenthsOfAMillimeter;
		}
		public static void AssignParameters(XtraReport report, IEnumerable<ReportParameter> clientParameters) {
			if(clientParameters == null) {
				return;
			}
			Dictionary<string, Parameter> parameterPaths = new NestedParameterPathCollector()
				.EnumerateParameters(report)
				.ToDictionary(x => x.Path, x => x.Parameter);
			foreach(var clientParameter in clientParameters) {
				Parameter parameter;
				if(!parameterPaths.TryGetValue(clientParameter.Path, out parameter)) {
					throw new ParameterValidationException(string.Format("Parameter '{0}' not found.", clientParameter.Path));
				}
				ValidateParameter(parameter.Type, parameter.MultiValue, clientParameter);
				parameter.Value = clientParameter.Value;
			}
		}
		public static string MakeUniqueName(Type type, XtraReport rootReport, params XRControl[] controls) {
			var container = new Container();
			try {
				var components = new Component[controls.Length + 1];
				controls.CopyTo(components, 1);
				components[0] = rootReport;
				var enumerator = new NestedComponentEnumerator(components);
				while(enumerator.MoveNext()) {
					if(string.IsNullOrEmpty(enumerator.Current.Name)) {
						container.Add(enumerator.Current);
					} else {
						container.Add(enumerator.Current, enumerator.Current.Name);
					}
				}
				return new XRNameCreationService(null).CreateName(container, type);
			} finally {
				container.RemoveAll();
				container.Dispose();
			}
		}
		static void ValidateParameter(Type parameterType, bool multiValue, ReportParameter clientParameter) {
			var value = clientParameter.Value;
			var path = clientParameter.Path;
			if(value != null && !parameterType.IsAssignableFrom(ParameterValueTypeHelper.GetValueType(value, multiValue))) {
				throw new ParameterValidationException(string.Format("Cannot set value of unrelated type for a parameter '{0}'.", path));
			}
		}
	}
}
