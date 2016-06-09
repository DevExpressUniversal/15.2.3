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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Data;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.DataAccess.Design;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Design.Entity;
using DevExpress.Design.TypePickEditor;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards3;
using DevExpress.XtraReports.Wizards3.Views;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.XtraReports.Design {
	public class DataSourceEditorVS : TreeViewTypePickEditor {
		#region inner classes
		protected class DataSourcePickerTreeViewVS : TypePickerTreeViewVS {
			public DataSourcePickerTreeViewVS(Type type)
				: base(type, null, null) {
			}
			protected override void FillNodes(TreeListNodes nodes, IDesignerHost designerHost) {
				DataSourceProviderService dspSvc = (DataSourceProviderService)designerHost.GetService(typeof(DataSourceProviderService));
				if(dspSvc == null) return;
				DataSourceGroupCollection dataSources = dspSvc.GetDataSources();
				if(dataSources == null) return;
				foreach(DataSourceGroup dataSourceGroup in dataSources) {
					if(dataSourceGroup.IsDefault) {
						CreateProjectGroup(nodes, designerHost, dataSourceGroup);
					} else {
						TreeListNode groupNode = new XtraListNode(dataSourceGroup.Name, nodes) { StateImageIndex = 9 };
						CreateProjectGroup(groupNode.Nodes, designerHost, dataSourceGroup);
						if(groupNode.Nodes.Count > 0)
							((IList)nodes).Add(groupNode);
					}
				}
			}
			void CreateProjectGroup(TreeListNodes nodes, IDesignerHost designerHost, DataSourceGroup dataSourceGroup) {
				foreach(DataSourceDescriptor dataSourceDescriptor in dataSourceGroup.DataSources) {
					try {
						PickerNode pickerNode = CreateBindingSourcePickerNode(designerHost.GetType(dataSourceDescriptor.TypeName));
						((IList)nodes).Add(pickerNode);
					} catch {
					}
				}
			}
			protected virtual PickerNode CreateBindingSourcePickerNode(Type type) {
				return new BindingSourceTypePickerNode(type.Name, type, this.Nodes) { StateImageIndex = 8 };
			}
		}
#endregion //inner classes
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return ProjectHelper.IsWebProject(provider) ?
				new TypePickerTreeViewVS(typeof(System.Data.DataSet), null, null) :
				new DataSourcePickerTreeViewVS(typeof(System.Data.DataSet));
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider provider) {
			return new DataSourcePickerPanelVS((TypePickerTreeViewVS)picker);
		}
	}
	class DataSourcePickerPanelVS : DataSourcePickerPanel {
		protected override bool CanAddNewDataSource(object instance) {
			return !(instance is CalculatedField) && !(instance is FormattingRule);
		}
		protected override void OnHyperLinkClick(object sender, LinkLabelLinkClickedEventArgs e) {
			var designerHost = Provider.GetService<IDesignerHost>();
			var lookAndFeelService = (ILookAndFeelService)designerHost.GetService(typeof(ILookAndFeelService));
			var uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			bool isWebProject = ProjectHelper.IsWebProject(designerHost);
			IParameterService parameterService = (IParameterService)designerHost.GetService(typeof(IParameterService));
			IPopupOwner popupOwner = Provider.GetService<IWindowsFormsEditorService>() as IPopupOwner;
			if(popupOwner != null) popupOwner.DisableClosing();
			var solutionTypesProvider = new PureSolutionTypesProvider();
			var connectionStringsProvider = new VSConnectionStringsService(designerHost);
			var connectionStorageService = new VSConnectionStorageService(designerHost);
			var client = new DataSourceWizardClientUI(connectionStorageService, parameterService, solutionTypesProvider, connectionStringsProvider, null) {
				DataSourceTypes = DataSourceTypes.All, 
				CustomQueryValidator = new VSCustomQueryValidator(), 
				Options = SqlWizardOptions.EnableCustomSql, 
				PropertyGridServices = designerHost
			};
			bool anyConnections = client.DataConnections.Any();
			var runner = new DataSourceWizardRunner<XtraReportModel>(lookAndFeelService.LookAndFeel, owner);
			try {
				if(runner.Run(client, new XtraReportModel(), customization => {
					customization.StartPage = !isWebProject ? typeof(ChooseDataSourceTypePage<XtraReportModel>) :
						anyConnections ? typeof(ChooseConnectionPage<XtraReportModel>) :
						typeof(ConnectionPropertiesPage<XtraReportModel>);
					customization.RegisterPageView<IChooseObjectConstructorPageView, ChooseObjectConstructorPageViewEx>();
				})) {
					XtraReportModel wizardModel = runner.WizardModel;
					DataComponentCreator.SaveConnectionIfShould(wizardModel, connectionStorageService);
					IComponent component = (IComponent)new DataComponentCreator().CreateDataComponent(wizardModel);
					DesignerTransaction designerTransaction = designerHost.CreateTransaction("Create report Data Source");
					designerHost.Container.Add(component);
					designerTransaction.Commit();
					if(popupOwner != null) popupOwner.EnableClosing();
					treeView.CloseDropDown(new InstancePickerNode(string.Empty, component, treeView.Nodes));
				}
			} finally {
				if(popupOwner != null) popupOwner.EnableClosing();
			}
		}
		public DataSourcePickerPanelVS(TypePickerTreeView treeView)
			: base(treeView, "Add Report Data Source...") {
		}
	}
	public class DataAdapterEditorVS : TreeViewTypePickEditor {
		protected override TypePickerTreeView CreateTypePicker(IServiceProvider provider) {
			return new TypePickerTreeViewVS(typeof(Component), new DataObjectAttribute(true), "true");
		}
		protected override TypePickerPanel CreatePickerPanel(TypePickerTreeView picker, IServiceProvider serviceProvider) {
			return new TypePickerPanel(picker);
		}
	}
	class PureSolutionTypesProvider : DTESolutionTypesProvider {
		protected override IEnumerable<Type> GetTypes(EnvDTE.Project project) {
			IEnumerable<Type> types = GetTypesCore(project, true);
			return FilterTypes(types, project, true);
		}
	}
}
