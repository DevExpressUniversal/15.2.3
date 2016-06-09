#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess.UI.Native;
using DevExpress.XtraEditors.Filtering;
using System;
using DevExpress.Utils.UI;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public partial class DashboardFilterForm : FilterForm {
		public static DashboardFilterForm CreateInstance(DashboardFilterFormContext filterContext, IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			IParameterCreator parameterCreator = serviceProvider.RequestServiceStrictly<IParameterCreator>();
			IDashboardOwnerService ownerService = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			IDashboardGuiContextService guiContext = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			return new DashboardFilterForm(parameterCreator, filterContext, ownerService.Dashboard.Parameters, guiContext.LookAndFeel, serviceProvider);
		}
		readonly DashboardFilterFormContext filterContext;
		readonly DashboardParameterCollection parameters;
		IServiceProvider serviceProvider;
		ParameterChangesCollection parametersChanged;
		public DashboardFilterForm() {
			InitializeComponent();
		}
		public DashboardFilterForm(IParameterCreator parameterCreator, DashboardFilterFormContext filterContext, DashboardParameterCollection parameters, UserLookAndFeel lookAndFeel, IServiceProvider serviceProvider)
			: base(parameterCreator, filterContext, parameters.ToList<IParameter>(), lookAndFeel) {
			this.filterContext = filterContext;
			this.parameters = parameters;
			this.parametersChanged = new ParameterChangesCollection(parameters);
			this.serviceProvider = serviceProvider;
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				serviceProvider = null;
			base.Dispose(disposing);
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			FilterCriteriaEditor.BeforeShowValueEditor += OnBeforeShowValueEditor;
		}
		protected override void UnsubscribeEvents() {
			base.UnsubscribeEvents();
			FilterCriteriaEditor.BeforeShowValueEditor -= OnBeforeShowValueEditor;
		}
		protected override void ApplyChanges() {
			if(FilterCriteriaEditor.IsFilterCriteriaChanged) {
				parametersChanged.ApplyChanges(FilterCriteriaEditor.Parameters.Cast<DashboardParameter>());
				string criteriaString = CriteriaOperator.ToString(FilterCriteriaEditor.FilterCriteria);
				IHistoryItem historyItem = filterContext.CreateIHistoryItem(criteriaString, parameters, parametersChanged);
				IDashboardDesignerHistoryService historyService = serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>();
				historyService.RedoAndAdd(historyItem);
				parametersChanged.ResetChanges();
				UpdateApplyButtonEnabledState(false);
			}
		}
		void OnBeforeShowValueEditor(object sender, ShowValueEditorEventArgs e) {
			DataItemFilterDateEdit edit = e.Editor as DataItemFilterDateEdit;
			if(edit != null) {
				DimensionFilterColumn column = e.CurrentNode.GetPropertyForEditing() as DimensionFilterColumn;
				if(column != null)
					edit.Properties.GroupInterval = column.Dimension.DateTimeGroupInterval;
			}
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DashboardFilterForm));
			this.SuspendLayout();
			resources.ApplyResources(this, "$this");
			this.Name = "DashboardFilterForm";
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
		}	 
	}
}
