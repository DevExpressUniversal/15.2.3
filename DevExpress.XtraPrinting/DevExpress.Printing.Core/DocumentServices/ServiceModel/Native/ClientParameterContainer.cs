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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.DocumentServices.ServiceModel.Native {
	public class ClientParameterContainer : IParameterContainer {
		readonly ReadOnlyCollection<ClientParameter> parameters;
		internal event EventHandler ParameterValueChanged;
		internal IEnumerable<Parameter> OriginalParameters {
			get { return parameters.Select(x => x.OriginalParameter); }
		}
		internal IEnumerable<ClientParameter> ClientParameters {
			get { return parameters; }
		}
		internal ClientParameterContainer(IList<ClientParameter> parameters) {
			this.parameters = new ReadOnlyCollection<ClientParameter>(parameters);
			ArrayHelper.ForEach(parameters, p => p.ValueChanged += (o, e) => RaiseParameterValueChanged());
		}
		public ClientParameterContainer()
			: this(new ClientParameter[0]) {
		}
		public ClientParameterContainer(ReportParameterContainer container)
			: this(ToClientParameters(container)) {
		}
		void RaiseParameterValueChanged() {
			if(ParameterValueChanged != null)
				ParameterValueChanged(this, EventArgs.Empty);
		}
		#region IParameterCollection
		public int Count {
			get { return parameters.Count; }
		}
		public IClientParameter this[string path] {
			get { return parameters.FirstOrDefault(cp => cp.Path == path); }
		}
		public IEnumerator<IClientParameter> GetEnumerator() {
			return parameters.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
		static IList<ClientParameter> ToClientParameters(ReportParameterContainer container) {
			var result = new List<ClientParameter>();
			foreach(ReportParameter reportParameter in container.Parameters) {
				StaticListLookUpSettings lookUpSettings = null;
				if(reportParameter.LookUpValues != null) {
					lookUpSettings = new StaticListLookUpSettings();
					foreach(var lookUpValue in reportParameter.LookUpValues) {
						lookUpSettings.LookUpValues.Add(lookUpValue);
					}
				}
				Type type = ParameterValueTypeHelper.GetValueType(reportParameter.Value, reportParameter.MultiValue);
				var xrParameter = new Parameter {
					Description = reportParameter.Description,
					Name = reportParameter.Name,
					Value = reportParameter.Value,
					Visible = reportParameter.Visible,
					Type = type,
					MultiValue = reportParameter.MultiValue,
					LookUpSettings = lookUpSettings
				};
				result.Add(new ClientParameter(xrParameter, reportParameter.Path) {
					IsFilteredLookUpSettings = reportParameter.IsFilteredLookUpSettings
				});
			}
			return result;
		}
	}
}
