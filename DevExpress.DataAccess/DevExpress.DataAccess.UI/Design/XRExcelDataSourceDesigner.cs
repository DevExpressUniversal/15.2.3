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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.UI.Excel;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.Excel;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Design {
	public class XRExcelDataSourceDesigner : ComponentDesigner {
		public static readonly PropertyDescriptor sourceOptionsPropertyDescriptor = TypeDescriptor.GetProperties(typeof(ExcelDataSource))["SourceOptions"];
		protected static readonly PropertyDescriptor fileNamePropertyDescriptor = TypeDescriptor.GetProperties(typeof(ExcelDataSource))["FileName"];
		protected static readonly PropertyDescriptor schemaPropertyDescriptor = TypeDescriptor.GetProperties(typeof(ExcelDataSource))["Schema"];
		readonly DesignerVerbCollection verbs = new DesignerVerbCollection();
		protected IComponentChangeService componentChangeService;
		protected IDesignerHost designerHost;
		protected ExcelDataSource DataSource {
			get { return (ExcelDataSource)Component; }
		}
		public override DesignerVerbCollection Verbs { get { return verbs; } }
		#region Overrides of ComponentDesigner
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			this.componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if(designerHost.GetService(typeof(IExcelSchemaProvider)) == null)
				designerHost.AddService(typeof(IExcelSchemaProvider), new ExcelSchemaProvider());
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost);
			TaskScheduler ui = TaskScheduler.FromCurrentSynchronizationContext();
			var excelParametersService = DataSource.GetService(typeof(IExcelOptionsCustomizationService)) as ExcelOptionsCustomizationServiceUI
				?? new ExcelOptionsCustomizationServiceUI(owner, ui, lookAndFeel);
			var serviceContainer = (IServiceContainer)DataSource;
			serviceContainer.RemoveService(typeof(IExcelOptionsCustomizationService));
			serviceContainer.AddService(typeof(IExcelOptionsCustomizationService), excelParametersService);
			verbs.Add(new DesignerVerb("Edit...", OnEdit));
			verbs.Add(new DesignerVerb("Update Schema", OnUpdateSchema));
		}
		#endregion
		protected internal virtual UserLookAndFeel GetLookAndFeel(IServiceProvider serviceProvider) {
			var lookAndFeelService = serviceProvider.GetService<ILookAndFeelService>();
			var lookAndFeel = new UserLookAndFeel(new object()) {
				UseDefaultLookAndFeel = false,
				SkinName = lookAndFeelService.LookAndFeel.ActiveSkinName
			};
			return lookAndFeel;
		}
		void OnEdit(object sender, EventArgs eventArgs) {
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IExcelSchemaProvider excelSchemaProvider = designerHost.GetService<IExcelSchemaProvider>();
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Edit ExcelDataSource")) {
				componentChangeService.OnComponentChanging(Component, fileNamePropertyDescriptor);
				componentChangeService.OnComponentChanging(Component, sourceOptionsPropertyDescriptor);
				componentChangeService.OnComponentChanging(Component, schemaPropertyDescriptor);
				if(!DataSource.EditDataSource(lookAndFeel, owner, excelSchemaProvider)) {
					transaction.Cancel();
					return;
				}
				componentChangeService.OnComponentChanged(Component, fileNamePropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(Component, sourceOptionsPropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(Component, schemaPropertyDescriptor, null, null);
				transaction.Commit();
			}
		}
		void OnUpdateSchema(object sender, EventArgs e) {
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IExcelSchemaProvider excelSchemaProvider = (IExcelSchemaProvider)designerHost.GetService(typeof(IExcelSchemaProvider));
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Update schema")) {
				componentChangeService.OnComponentChanging(Component, schemaPropertyDescriptor);
				if(!DataSource.UpdateSchema(owner, lookAndFeel, excelSchemaProvider)) {
					transaction.Cancel();
					return;
				}
				componentChangeService.OnComponentChanged(Component, schemaPropertyDescriptor, null, null);
				transaction.Commit();
			}
		}
	}
}
