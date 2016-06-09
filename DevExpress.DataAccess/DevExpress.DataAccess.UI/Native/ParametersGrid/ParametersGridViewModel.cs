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
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Design;
namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	public class ParametersGridViewModel : IParametersGridViewModel {
		ParametersGridModel model;
		readonly IServiceProvider propertyGridServices;
		readonly IParameterService parameterService;
		readonly PrefixNameGenerator parameterNameGenerator;
		readonly Predicate<string> containsName;
		readonly bool isFixedParameters;
		readonly bool previewDataRowLimit;
		readonly Func<object> previewFunc;
		public ParametersGridViewModel(ParametersGridModel model, IParameterService parameterService, IServiceProvider propertyGridServices, Func<object> previewFunc, bool isFixedParameters, bool previewDataRowLimit) {
			this.previewFunc = previewFunc;
			this.isFixedParameters = isFixedParameters;
			this.previewDataRowLimit = previewDataRowLimit;
			this.parameterService = parameterService;
			this.propertyGridServices = propertyGridServices;
			this.model = model;
			Records = new BindingList<GridRecord>(model.Parameters.Select(p => GridRecord.FromIParameter(p, parameterService)).ToList()) {
				AllowNew = true
			};
			Records.ListChanged += Records_ListChanged;
			this.containsName = s => Records.Any(p => string.Equals(s, p.Name, StringComparison.Ordinal));
			this.parameterNameGenerator = new PrefixNameGenerator(DataAccessUILocalizer.GetString(DataAccessUIStringId.DefaultNameParameter), 1);
		}
		public BindingList<GridRecord> Records { get; set; }
		public object[] ExternalParameterExpressions {
			get {
				return (parameterService != null
					? parameterService.Parameters
						.Select(p => new Expression(string.Format("[Parameters.{0}]", p.Name), p.Type))
						.Cast<object>()
						.ToList()
					: new List<object>()).ToArray();
			}
		}
		public object[] Types {
			get { return ParameterEditorService.GetParameterTypes("").Values.Cast<object>().ToArray(); }
		}
		public bool IsFixedParameters {
			get { return isFixedParameters; }
		}
		public bool CanRemoveParameters {
			get { return !isFixedParameters && Records.Count > 0; }
		}
		public bool IsPreviewAvailable {
			get { return previewFunc != null; }
		}
		public bool PreviewDataRowLimit { get { return previewDataRowLimit; } }
		public event EventHandler CanRemoveParametersChanged;
		public event EventHandler ExternalParameterAdded;
		public object LoadPreviewData() {
			return previewFunc();
		}
		public string GetDisplayName(GridRecord record) {
			return record.IsExpression
				? record.Expression == null
					? "[No Type]"
					: record.Expression.ResultType == null
						? "[No Type]" 
						: record.Expression.ResultType.IsGenericType
							? TypeNamesHelper.ShortName(record.Expression.ResultType) :
							ParameterEditorService.GetDisplayNameByParameterType(string.Empty, record.Expression.ResultType)
				: record.Type == null
					? "[No Type]"
					: record.Type.IsGenericType
						? TypeNamesHelper.ShortName(record.Type) :
						ParameterEditorService.GetDisplayNameByParameterType(string.Empty, record.Type);
		}
		public void OnCreateParameter() {
			var generatedName = parameterNameGenerator.GenerateName(containsName);
			var parameter = new GridRecord(generatedName, parameterService);
			Records.Add(parameter);
		}
		public void OnRemoveParameter(GridRecord parameter) {
			if(CanRemoveParameters)
				Records.Remove(parameter);
		}
		public void OnCreateExternalParameter(Func<IPropertyGridView> createViewFunc, GridRecord record) {
			if(parameterService == null || !parameterService.CanCreateParameters) {
				return;
			}
			IParameter parameter = parameterService.CreateParameter(record.Expression.ResultType);
			PropertyGridModel pgModel;
			if(LaunchPropertyGridParameterEdit(createViewFunc, parameter, out pgModel)) {
				parameterService.AddParameter(parameter);
				BindToExternalParameterCore(record, pgModel.Parameter);
				OnExternalParameterAdded();
			}
		}
		public void OnBindToExternalParameter(GridRecord record, string externalParameterName) {
			var externalParameter = parameterService.Parameters.FirstOrDefault(p => p.Name == externalParameterName);
			if(externalParameter == null) {
				return;
			}
			BindToExternalParameterCore(record, externalParameter);
		}
		public void OnExpressionEdit(Func<DataSourceParameterBase, IExpressionView> createViewFunc, GridRecord record) {
			Expression expression = record.Expression;
			if(expression == null)
				return;
			var tempParameter = new QueryParameter(string.Empty, typeof(object), expression);
			ExpressionModel eModel;
			if(LaunchExpressionEdit(createViewFunc, tempParameter, out eModel)) {
				record.Value = new Expression(eModel.ExpressionString, expression.ResultType);
			}
		}
		void BindToExternalParameterCore(GridRecord record, IParameter externalParameter) {
			var externalQueryParameter = externalParameter.ToExternalQueryParameter();
			record.ApplyParameter(externalQueryParameter, !isFixedParameters);
		}
		bool LaunchExpressionEdit(Func<DataSourceParameterBase, IExpressionView> createViewFunc, DataSourceParameterBase parameter, out ExpressionModel eModel) {
			eModel = new ExpressionModel();
			IExpressionView view = createViewFunc(parameter);
			var presenter = new ExpressionPresenter(eModel, view);
			return presenter.Do();
		}
		bool LaunchPropertyGridParameterEdit(Func<IPropertyGridView> createViewFunc, IParameter parameter, out PropertyGridModel pgModel) {
			pgModel = new PropertyGridModel(parameter);
			IPropertyGridView view = createViewFunc();
			var presenter = new PropertyGridPresenter(pgModel, view);
			presenter.InitView();
			return presenter.Do();
		}
		protected virtual void OnCanRemoveParametersChanged() {
			if(CanRemoveParametersChanged != null)
				CanRemoveParametersChanged(this, EventArgs.Empty);
		}
		protected virtual void OnExternalParameterAdded() {
			if(ExternalParameterAdded != null) {
				ExternalParameterAdded(this, EventArgs.Empty);
			}
		}
		#region Event handlers
		void Records_ListChanged(object sender, ListChangedEventArgs e) {
			OnCanRemoveParametersChanged();
		}
		#endregion
	}
}
