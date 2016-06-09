#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Kpi {
	public class ViewModelInitializedEventArgs : EventArgs {
		private IModelView modelView;
		public ViewModelInitializedEventArgs(IModelView modelView) {
			this.modelView = modelView;
		}
		public IModelView ModelView {
			get { return modelView; }
		}
	}
	public class ShowKpiDrilldownEventArgs : HandledEventArgs {
		public ShowKpiDrilldownEventArgs(IKpiInstance kpiInstance)
			: base(false) {
			KpiInstance = kpiInstance;
		}
		public IKpiInstance KpiInstance { get; private set; }
	}
	public class DrilldownController : ViewController<ListView> {
		private bool IsNotLookup {
			get { return (Frame is NestedFrame) && (((NestedFrame)Frame).ViewItem is DevExpress.ExpressApp.Editors.ListPropertyEditor); }
		}
		private void OnDrilldownViewInitialized(IModelListView listView) {
			if(DrilldownViewInitialized != null) {
				DrilldownViewInitialized(this, new ViewModelInitializedEventArgs(listView));
			}
		}
		private void processController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e) {
			OnShowKpiDrilldown((IKpiInstance)View.CurrentObject);
			e.Handled = true;
		}
		protected internal static IModelListView GetDrilldownListViewModel(XafApplication application, IKpiDefinition kpiDefinition) {
			IModelListView modelListView = application.FindModelView(kpiDefinition.DrillDownListViewId) as IModelListView;
			if(modelListView == null) {
				modelListView = application.Model.Views.AddNode<IModelListView>(kpiDefinition.DrillDownListViewId);
				modelListView.ModelClass = application.Model.BOModel.GetClass(kpiDefinition.TargetObjectType);
				modelListView.DataAccessMode = CollectionSourceDataAccessMode.Client;
				DevExpress.ExpressApp.Model.NodeGenerators.ModelListViewColumnsNodesGenerator.ForceGenerateListViewColumns(modelListView);
			}
			return modelListView;
		}
		protected virtual void OnShowKpiDrilldown(IKpiInstance currentKpiInstance) {
			IObjectSpace os = Application.GetObjectSpaceToShowDetailViewFrom(this.Frame, currentKpiInstance.GetType());
			IKpiInstance kpiInstance = os.GetObject<IKpiInstance>(currentKpiInstance);
			ShowKpiDrilldownEventArgs showKpiDrilldownEventArgs = new ShowKpiDrilldownEventArgs(kpiInstance);
			if(ShowKpiDrilldown != null) {
				ShowKpiDrilldown(this, showKpiDrilldownEventArgs);
			}
			if(!showKpiDrilldownEventArgs.Handled) {
				IModelListView modelListView = GetDrilldownListViewModel(Application, kpiInstance.GetKpiDefinition());
				OnDrilldownViewInitialized(modelListView);
				ListView listView = Application.CreateListView(modelListView, kpiInstance.GetKpiDefinition().GetDrilldownCollectionSource(os), true);
				ShowViewParameters parameters = new ShowViewParameters(listView);
				parameters.TargetWindow = TargetWindow.NewWindow;
				parameters.Context = TemplateContext.View;
				DialogController dialogController = Application.CreateController<DialogController>();
				dialogController.AcceptAction.Caption = CaptionHelper.GetLocalizedText("DialogButtons", "Close", "Close");
				dialogController.AcceptAction.ToolTip = dialogController.AcceptAction.Caption;
				dialogController.AcceptAction.ActionMeaning |= ActionMeaning.Cancel;
				dialogController.CancelAction.Active.SetItemValue("Showing KPI", false);
				dialogController.CloseOnCurrentObjectProcessing = false;
				dialogController.SaveOnAccept = false;
				parameters.Controllers.Add(dialogController);
				Application.ShowViewStrategy.ShowView(parameters, new ShowViewSource(Frame, null));
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ListViewProcessCurrentObjectController processController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processController != null && IsNotLookup) {
				processController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processController_CustomProcessSelectedItem);
			}
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ListViewProcessCurrentObjectController processController = Frame.GetController<ListViewProcessCurrentObjectController>();
			if(processController != null && IsNotLookup) {
				processController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processController_CustomProcessSelectedItem);
			}
		}
		public DrilldownController() {
			this.TargetViewNesting = Nesting.Nested;
			this.TargetObjectType = typeof(IKpiInstance);
		}
		public event EventHandler<ShowKpiDrilldownEventArgs> ShowKpiDrilldown;
		public event EventHandler<ViewModelInitializedEventArgs> DrilldownViewInitialized;
	}
}
