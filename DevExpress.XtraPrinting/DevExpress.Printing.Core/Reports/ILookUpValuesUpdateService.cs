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
using System.Threading.Tasks;
using DevExpress.Data.Browsing;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Native;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Parameters.Native {
	public interface ILookUpValuesProvider {
		Task<IEnumerable<ParameterLookUpValuesContainer>> GetLookUpValues(Parameter changedParameter, IParameterEditorValueProvider editorValueProvider);
	}
	public class LookUpValuesProvider : ILookUpValuesProvider {
		readonly IEnumerable<Parameter> parameters;
		readonly DataContext dataContext;
		public LookUpValuesProvider(IEnumerable<Parameter> parameters, DataContext dataContext) {
			this.parameters = parameters;
			this.dataContext = dataContext;
		}
		public Task<IEnumerable<ParameterLookUpValuesContainer>> GetLookUpValues(Parameter changedParameter, IParameterEditorValueProvider editorValueProvider) {
			return Task.Factory.StartNew<IEnumerable<ParameterLookUpValuesContainer>>(() => {
				List<ParameterLookUpValuesContainer> result = new List<ParameterLookUpValuesContainer>();
				var firstDependentParameter = parameters.FirstOrDefault(x => CascadingParametersService.IsDependentParameter(changedParameter.Name, x));
				if(firstDependentParameter != null) {
					var dependentParameters = parameters
						.SkipWhile(x => x != firstDependentParameter)
						.Where(x => x.HasCascadeLookUpSettings())
						.Select(x => x);
					foreach(var x in dependentParameters) {
						var values = LookUpHelper.GetLookUpValues(x.LookUpSettings, dataContext, editorValueProvider);
						if(values == null)
							break;
						result.Add(new ParameterLookUpValuesContainer(x, values));
					}
				}
				return result.ToArray();
			});
		}
	}
	public class RemoteLookUpValuesProvider : ILookUpValuesProvider {
		readonly Func<IReportServiceClient> getActualClient;
		readonly ClientParameterContainer parametersContainer;
		readonly InstanceIdentity reportIdentity;
		EventHandler<ScalarOperationCompletedEventArgs<ParameterLookUpValues[]>> getLookUpsCompleted;
		TaskCompletionSource<IEnumerable<ParameterLookUpValuesContainer>> tcs;
		public RemoteLookUpValuesProvider(ClientParameterContainer parametersContainer, InstanceIdentity reportIdentity, Func<IReportServiceClient> getActualClient) {
			this.parametersContainer = parametersContainer;
			this.getActualClient = getActualClient;
			this.reportIdentity = reportIdentity;
		}
		public Task<IEnumerable<ParameterLookUpValuesContainer>> GetLookUpValues(Parameter changedParameter, IParameterEditorValueProvider editorValueProvider) {
			List<ParameterLookUpValuesContainer> lookUpValues = new List<ParameterLookUpValuesContainer>();
			tcs = new TaskCompletionSource<IEnumerable<ParameterLookUpValuesContainer>>();
			if(!parametersContainer.Any(x => ((ClientParameter)x).IsFilteredLookUpSettings)) {
				tcs.SetResult(new ParameterLookUpValuesContainer[] { });
				return tcs.Task;
			}
			var client = getActualClient();
			if(client == null) {
				tcs.SetException(new InvalidOperationException("IReportServiceClient instance does not initialized."));
				return tcs.Task;
			}
			var reportParameters = parametersContainer.ToParameterStubs();
			reportParameters.ForEach(x => x.Value = GetActualEditorValue(x, editorValueProvider));
			var changedParameterIndex = parametersContainer;
			string[] requiredParameterPaths = parametersContainer
				.Cast<ClientParameter>()
				.Reverse()
				.TakeWhile(x => x.OriginalParameter != changedParameter)
				.Where(x => x.IsFilteredLookUpSettings)
				.Select(x => x.Path)
				.ToArray();
			getLookUpsCompleted = (s, e) => OnGetLookUpValuesCompleted(s, e);
			client.GetLookUpValuesCompleted += getLookUpsCompleted;
			client.GetLookUpValuesAsync(reportIdentity, reportParameters, requiredParameterPaths, null);
			return tcs.Task;
		}
		void OnGetLookUpValuesCompleted(object sender, ScalarOperationCompletedEventArgs<ParameterLookUpValues[]> e) {
			var client = sender as IReportServiceClient;
			client.GetLookUpValuesCompleted -= getLookUpsCompleted;
			if(e.Error != null)
				tcs.SetException(e.Error);
			else {
				var lookUpValuesContainer = e.Result.Select(x => new ParameterLookUpValuesContainer(((ClientParameter)parametersContainer[x.Path]).OriginalParameter, x.LookUpValues));
				tcs.SetResult(lookUpValuesContainer);
			}
		}
		object GetActualEditorValue(ReportParameter parameterStub, IParameterEditorValueProvider editorValueProvider) {
			var parameter = parametersContainer.FirstOrDefault(x => ((ClientParameter)x).Path == parameterStub.Path) as ClientParameter;
			return parameter != null ? editorValueProvider.GetValue(parameter.OriginalParameter) : parameterStub.Value;
		}
	}
}
