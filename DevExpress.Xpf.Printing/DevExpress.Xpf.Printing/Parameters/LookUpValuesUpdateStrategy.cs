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
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Printing.Parameters.Models;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraPrinting.Native;
using System.Threading.Tasks;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.Client;
using EventArgs = DevExpress.Data.Utils.ServiceModel.ScalarOperationCompletedEventArgs<DevExpress.DocumentServices.ServiceModel.DataContracts.ParameterLookUpValues[]>;
using DevExpress.Xpf.Printing;
namespace DevExpress.Xpf.Printing.Parameters {
	abstract class LookUpValuesUpdateStrategy {
		readonly IDialogService dialogService;
		readonly IEnumerable<ParameterModel> parameterModels;
		readonly ParameterValueProvider valueProvider;
		public event EventHandler LookUpValuesUpdated;
		protected IDialogService DialogService { get { return dialogService; } }
		protected IEnumerable<ParameterModel> ParameterModels { get { return parameterModels; } }
		protected ParameterValueProvider ParameterValuesProvider { get { return valueProvider; } }
		public LookUpValuesUpdateStrategy(IEnumerable<ParameterModel> parameterModels, IDialogService dialogService) {
			this.parameterModels = parameterModels;
			this.valueProvider = new ParameterValueProvider(parameterModels);
			this.dialogService = dialogService;
		}
		public abstract void Update(ParameterModel changedModel);
		protected void RaiseLookUpValuesUpdated() {
			if(LookUpValuesUpdated != null)
				LookUpValuesUpdated(this, EventArgs.Empty);
		}
	}
	class LocalLookUpValuesUpdateStrategy : LookUpValuesUpdateStrategy {
		readonly DataContext dataContext;
		public LocalLookUpValuesUpdateStrategy(IEnumerable<ParameterModel> parameterModels, DataContext dataContext, IDialogService dialogService)
			: base(parameterModels, dialogService) {
			this.dataContext = dataContext;
		}
		public override void Update(ParameterModel changedModel) {
			Task.Factory.StartNew(() => {
				var dependentParameters = CascadingParametersService.GetDependentParameters(ParameterModels.Select(x => x.Parameter), changedModel.Parameter);
				ParameterModels
					.Where(model => dependentParameters.Contains(model.Parameter))
					.ForEach(model => model.UpdateLookUpValues(LookUpHelper.GetLookUpValues(model.Parameter.LookUpSettings, dataContext, ParameterValuesProvider)));
			}).ContinueWith((task) => {
				if(task.Exception != null && DialogService != null) {
					DialogService.ShowError(task.Exception.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
				}
				RaiseLookUpValuesUpdated();
			});
		}
	}
	class RemoteLookUpValuesUpdateStrategy : LookUpValuesUpdateStrategy {
		readonly InstanceIdentity reportIdentity;
		readonly Func<IReportServiceClient> getClient;
		public RemoteLookUpValuesUpdateStrategy(IEnumerable<ParameterModel> parameterModels, InstanceIdentity reportIdentity, Func<IReportServiceClient> getClient, IDialogService dialogService)
			: base(parameterModels, dialogService) {
			this.reportIdentity = reportIdentity;
			this.getClient = getClient;
		}
		public override void Update(ParameterModel changedModel) {
			var client = getClient();
			var parameterStubs = ParameterModels.Select(model => model.GetReportParameterStub()).ToArray();
			var requiredParameterPaths =
				ParameterModels.Reverse()
				.TakeWhile(x => x != changedModel)
				.Where(x => x.IsFilteredLookUpSettings)
				.Select(x => x.Path);
			client.GetLookUpValuesCompleted += client_GetLookUpValuesCompleted;
			client.GetLookUpValuesAsync(reportIdentity, parameterStubs, requiredParameterPaths.ToArray(), null);
		}
		void client_GetLookUpValuesCompleted(object sender, EventArgs e) {
			if(e.Error != null && DialogService != null) {
				DialogService.ShowError(e.Error.Message, PrintingLocalizer.GetString(PrintingStringId.Error));
			} else {
				foreach(var lookUps in e.Result) {
					var parameterModel = ParameterModels.FirstOrDefault(x => x.Path == lookUps.Path);
					if(parameterModel == null)
						continue;
					parameterModel.UpdateLookUpValues(lookUps.LookUpValues);
				}
			}
			RaiseLookUpValuesUpdated();
		}
	}
}
