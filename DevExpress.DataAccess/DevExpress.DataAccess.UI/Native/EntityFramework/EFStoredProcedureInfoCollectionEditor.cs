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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
using DevExpress.XtraWaitForm;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	internal class EFStoredProcedureInfoCollectionEditor : UITypeEditor {
		class WizardRunner : DataSourceWizardRunnerBase<EFDataSourceModel, EFDataSourceWizardClientUI> {
			public WizardRunner(UserLookAndFeel lookAndFeel, IWin32Window owner) : base(lookAndFeel, owner) { }
			public WizardRunner(IWizardRunnerContext context) : base(context) { }
			#region Overrides of DataSourceWizardRunnerBase<TModel,TClient>
			protected override Type StartPage {
				get { return typeof(ConfigureEFStoredProceduresPage<EFDataSourceModel>); }
			}
			protected override string WizardTitle {
				get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.EFStoredProcsEditorTitle); }
			}
			protected override WizardPageFactoryBase<EFDataSourceModel, EFDataSourceWizardClientUI> CreatePageFactory(EFDataSourceWizardClientUI client) {
				return new EFWizardPageFactory<EFDataSourceModel, EFDataSourceWizardClientUI>(client);
			}
			#endregion
		}
		public static readonly PropertyDescriptor StoredProceduresDescriptor = TypeDescriptor.GetProperties(typeof(EFDataSource))["StoredProcedures"];
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			EFStoredProcedureInfoCollection collection = value as EFStoredProcedureInfoCollection;
			if(collection == null)
				return base.EditValue(context, provider, value);
			EFDataSource dataSource = collection.DataSource;
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			UserLookAndFeel lookAndFeel = context.GetService<ILookAndFeelService>().LookAndFeel;
			IParameterService parameterService = context.GetService<IParameterService>();
			IRepositoryItemsProvider repositoryItemsProvider = context.GetService<IRepositoryItemsProvider>() ?? DefaultRepositoryItemsProvider.Instance;
			IWaitFormActivator waitFormActivator = new WaitFormActivatorDesignTime(owner, typeof(DemoWaitForm), lookAndFeel.ActiveSkinName);
			IExceptionHandler exceptionHandler = new ConnectionExceptionHandler(owner, lookAndFeel);
			if(!ConnectionHelper.OpenConnection(dataSource.Connection, exceptionHandler, waitFormActivator))
				return value;
			IServiceContainer serviceProvider = new ServiceContainer();
			if(parameterService != null)
				serviceProvider.AddService(typeof(IParameterService), parameterService);
			serviceProvider.AddService(typeof(UserLookAndFeel), lookAndFeel);
			IDesignerHost host = context.GetService<IDesignerHost>();
			IComponentChangeService componentChangeService = context.GetService<IComponentChangeService>();
			if(StoredProcParametersHelper.SyncProcedures(dataSource.Connection.GetDBSchema(), dataSource.StoredProcedures).Count == 0) {
				XtraMessageBox.Show(lookAndFeel, owner,
					DataAccessUILocalizer.GetString(DataAccessUIStringId.EFEditorsNoStoredProcs),
					DataAccessUILocalizer.GetString(DataAccessUIStringId.EFStoredProcsEditorTitle),
					MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return value;
			}
			using(DesignerTransaction transaction = host.CreateTransaction("Manage Stored Procedures")) {
				var client = new EFDataSourceWizardClientUI(parameterService, null, null, null) {
					RepositoryItemsProvider = repositoryItemsProvider
				};
				var model = new EFDataSourceModel(dataSource.Connection) {
					StoredProceduresInfo = dataSource.StoredProcedures.ToArray()
				};
				var runner = new WizardRunner(lookAndFeel, owner);
				if(runner.Run(client, model)) {
					componentChangeService.OnComponentChanging(dataSource, StoredProceduresDescriptor);
					collection.Clear();
					collection.AddRange(runner.WizardModel.StoredProceduresInfo);
					value = collection;
					componentChangeService.OnComponentChanged(dataSource, StoredProceduresDescriptor, null, null);
					transaction.Commit();
					ISelectionService selectionService = context.GetService<ISelectionService>();
					selectionService.SetSelectedComponents(new object[] { host.RootComponent });
					selectionService.SetSelectedComponents(new object[] { context.Instance });
				}
				else
					transaction.Cancel();
			}
			return base.EditValue(context, provider, value);
		}
	}
}
