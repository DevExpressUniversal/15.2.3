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
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ConfigureEFStoredProceduresPage<TModel> : WizardPageBase<IConfigureEFStoredProceduresPageView, TModel>
		where TModel : IEFDataSourceModel {
		readonly IWizardRunnerContext context;
		DBSchema schema;
		List<StoredProcedureViewInfo> procedures;
		IParameterService parameterService;
		public ConfigureEFStoredProceduresPage(IConfigureEFStoredProceduresPageView view, IWizardRunnerContext context, IParameterService parameterService)
			: base(view) {
			this.context = context;
			this.parameterService = parameterService;
		}
		public override bool FinishEnabled { get { return true; } }
		public override bool MoveNextEnabled { get { return false; } }
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Loading); } }
		DBSchema Schema { get { return schema ?? (schema = Model.DataConnection.GetDBSchema()); } }
		protected EFContextWrapper GetContextWrapper() {
			EFStoredProcedureInfo storedProcedure = new EFStoredProcedureInfo(Model.DataMember);
			StoredProcParametersHelper.SyncParams(new EFStoredProcInfo(storedProcedure.Name, storedProcedure.Parameters), Schema);
			return new EFContextWrapper(Model.DataConnection, storedProcedure.Name, storedProcedure.Parameters);
		}
		public override void Begin() {
			procedures = StoredProcParametersHelper.SyncProcedures(Model.DataConnection.GetDBSchema(), Model.StoredProceduresInfo);
			View.Initialize(procedures.Where(sp => sp.Checked), GetPreviewData);
			View.SetAddEnabled(procedures.Any(sp => !sp.Checked));
			View.AddClick += View_AddClick;
			View.RemoveClick += View_RemoveClick;
		}
		public override void Commit() {
			View.AddClick -= View_AddClick;
			View.RemoveClick -= View_RemoveClick;
			Model.StoredProceduresInfo = procedures.Where(sp => sp.Checked).Select(info => info.StoredProcedure).ToArray();
		}
		public override Type GetNextPageType() {
			if(procedures.Count(sp => sp.Checked) == 1) {
				bool tablesExists = Schema.Tables.Any() || Schema.Views.Any();
				if(!tablesExists) {
					Model.DataMember = procedures.First(sp => sp.Checked).StoredProcedure.Name;
					Model.DataSchema = GetDataSchema();
					return null;
				}
			}
			return typeof(ChooseEFDataMemberPage<TModel>);
		}
		protected object GetDataSchema() {
			EFContextWrapper wrapper = GetContextWrapper();
			DataContextBase dataContext = new DataContextBase();
			PropertyDescriptorCollection properties = dataContext[wrapper, Model.DataMember].GetItemProperties();
			ResultTable resultTable = new ResultTable(Model.DataMember);
			foreach(PropertyDescriptor property in properties)
				resultTable.AddColumn(property.Name, property.PropertyType);
			return resultTable;
		}
		protected IEnumerable<IParameter> GetEvaluatedParameters() {
			return ParametersEvaluator.EvaluateParameters(GetSourceParameters(),
				GetParameters().Select(DataSourceParameterBase.FromIParameter));
		}
		protected IEnumerable<IParameter> GetSourceParameters() {
			return parameterService != null ? parameterService.Parameters : new List<IParameter>();
		}
		protected IEnumerable<IParameter> GetParameters() { return View.SelectedItem.StoredProcedure.Parameters; }
		void View_AddClick(object sender, EventArgs e) {
			List<StoredProcedureViewInfo> available = procedures.Where(sp => !sp.Checked).ToList();
			if(available.Count == 0) {
				View.SetAddEnabled(false);
				return;
			}
			IEnumerable<StoredProcedureViewInfo> choosen = View.ChooseProceduresToAdd(available);
			IList<StoredProcedureViewInfo> infos = choosen as IList<StoredProcedureViewInfo> ?? choosen.ToList();
			foreach(StoredProcedureViewInfo info in infos) {
				info.Checked = true;
			}
			View.AddToList(infos);
			if(infos.Count == available.Count)
				View.SetAddEnabled(false);
		}
		void View_RemoveClick(object sender, EventArgs e) {
			StoredProcedureViewInfo item = View.SelectedItem;
			if(item == null)
				return;
			item.Checked = false;
			View.RemoveFromList(item);
			View.SetAddEnabled(true);
		}
		public object GetPreviewData() {
			return EFPreviewHelper.GetPreviewData(Model.DataConnection, View.SelectedItem.StoredProcedure,
				GetEvaluatedParameters(), ExceptionHandler, WaitFormActivator);
		}
	}
}
