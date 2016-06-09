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
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.DataAccess.Editors;
using DevExpress.Xpf.DataAccess.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension {
	public class XRParameterService : IParameterService {
		readonly XRControlModelFactory factory;
		readonly IList<XRComponentModelBase> components;
		public XRParameterService(IList<XRComponentModelBase> components, XRControlModelFactory factory) {
			this.factory = factory;
			this.components = components;
		}
		bool IParameterService.CanCreateParameters { get { return true; } }
		void IParameterService.AddParameter(IParameter parameter) {
			if(parameter == null) return;
			var reportParameter = (ParameterWithoutLookUpSettings)parameter;
			var realParameter = new Parameter() {
				Name = reportParameter.Name,
				Description = reportParameter.Description,
				Type = reportParameter.Type,
				Value = reportParameter.Value,
				Visible = reportParameter.Visible,
				MultiValue = reportParameter.MultiValue,
			};
			components.Add(factory.GetModel(realParameter, true));
		}
		string IParameterService.AddParameterString { get { return "New Report Parameter..."; } }
		IParameter IParameterService.CreateParameter(Type type) {
			return new Parameter() { Type = type, Name = ParametersHelper.GetNewName(((IParameterService)this).Parameters, "parameter") };
		}
		string IParameterService.CreateParameterString { get { return "Report ParameterService"; } }
		IEnumerable<IParameter> IParameterService.Parameters {
			get {
				var parameters = new List<Parameter>();
				foreach(var component in components) {
					var parameter = component.XRObject as Parameter;
					if(parameter != null)
						parameters.Add(parameter);
				}
				return parameters;
			}
		}
	}
}
