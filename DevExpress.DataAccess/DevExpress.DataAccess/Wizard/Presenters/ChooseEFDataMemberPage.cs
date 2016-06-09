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
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public abstract class ChooseEFDataMemberPageBase<TView, TModel> : WizardPageBase<TView, TModel>
		where TView : IChooseEFDataMemberPageViewBase
		where TModel : IEFDataSourceModel {
		readonly IWizardRunnerContext context;
		DBSchema schema;
		protected ChooseEFDataMemberPageBase(TView view, IWizardRunnerContext context)
			: base(view) {
			this.context = context;
		}
		protected EFDataConnection Connection { get { return Model.DataConnection; } }
		protected List<DBTable> GetTables {
			get {
				return new List<DBTable>(Schema.Tables.Union(schema.Views).OrderBy(t => t.Name));
			}
		}
		protected List<DBStoredProcedure> GetProcedures {
			get {
				return new List<DBStoredProcedure>(Schema.StoredProcedures.OrderBy(p => p.Name));
			}
		}
		protected IEnumerable<string> StoredProcNames {
			get {
				return Model.StoredProceduresInfo != null ? Model.StoredProceduresInfo.Select(p => p.Name) : new string[0];
			} 
		}
		protected abstract string DataMember {
			get;
		}
		protected DBSchema Schema {
			get { if(schema == null)
				schema = Connection.GetDBSchema();
			return schema;
			}
		}
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Connection); } }
		public override void Begin() {
			InitializeView();
			View.DataMemberChanged += View_DataMemberChanged;
		}
		public override bool Validate(out string errorMessage) {
			try {
				Model.DataSchema = GetDataSchema();
				Model.DataMember = View.DataMember;
			}
			catch(Exception ex) {
				ExceptionHandler.HandleException(ex);
				errorMessage = ex.Message;
				return false;
			}
			errorMessage = string.Empty;
			return true;
		}
		public override void Commit() { }
		protected abstract EFContextWrapper GetContextWrapper();
		protected abstract void InitializeView();
		object GetDataSchema() {
			EFContextWrapper wrapper = GetContextWrapper();
			DataContextBase dataContext = new DataContextBase();
			PropertyDescriptorCollection properties = dataContext[wrapper, View.DataMember].GetItemProperties();
			ResultTable resultTable = new ResultTable(Model.DataMember);
			foreach(PropertyDescriptor property in properties)
				resultTable.AddColumn(property.Name, property.PropertyType);
			return resultTable;
		}
		void View_DataMemberChanged(object sender, EventArgs e) {
			RaiseChanged();
		}
	}
	public class ChooseEFDataMemberPage<TModel> : ChooseEFDataMemberPageBase<IChooseEFDataMemberPageView, TModel>
		where TModel : IEFDataSourceModel {
		protected override string DataMember {
			get {
				return View.DataMember;
			}
		}
		public ChooseEFDataMemberPage(IChooseEFDataMemberPageView view, IWizardRunnerContext context)
			: base(view, context) {
		}
		public override Type GetNextPageType() {
			return typeof(ConfigureEFStoredProceduresPage<TModel>);
		}
		protected override EFContextWrapper GetContextWrapper() {
			if(!View.StoredProcChosen)
				return new EFContextWrapper(Model.DataConnection);
			EFStoredProcedureInfo storedProcedure = new EFStoredProcedureInfo(View.DataMember);
			StoredProcParametersHelper.SyncParams(new EFStoredProcInfo(storedProcedure.Name, storedProcedure.Parameters), Schema);
			return new EFContextWrapper(Model.DataConnection, storedProcedure.Name, storedProcedure.Parameters);
		}
		protected override void InitializeView() {
			View.Initialize(null, GetProcedures, DataMember);
		}
	}
}
