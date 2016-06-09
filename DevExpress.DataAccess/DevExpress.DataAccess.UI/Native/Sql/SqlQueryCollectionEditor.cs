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
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class SqlQueryCollectionEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			SqlQueryCollection sqlQueryCollection = (SqlQueryCollection) value;
			if(sqlQueryCollection == null)
				return null;
			SqlDataSource dataSource = sqlQueryCollection.DataSource;
			IDesignerHost host = context.GetService<IDesignerHost>();
			XRSqlDataSourceDesigner designer =  (XRSqlDataSourceDesigner) host.GetDesigner(dataSource);
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IParameterService parameterService = context.GetService<IParameterService>();
			IDBSchemaProvider dbSchemaProvider = context.GetService<IDBSchemaProvider>();
			IComponentChangeService componentChangeService = context.GetService<IComponentChangeService>();
			var sqlWizardOptionsProvider = context.GetService<ISqlWizardOptionsProvider>();
			SqlWizardOptions options = sqlWizardOptionsProvider != null ? sqlWizardOptionsProvider.SqlWizardOptions : SqlWizardOptions.EnableCustomSql;
			ICustomQueryValidator customQueryValidator = context.GetService<ICustomQueryValidator>();
			using(UserLookAndFeel lookAndFeel = designer.GetLookAndFeel(context))
			using(DesignerTransaction transaction = host.CreateTransaction("Manage Queries")) {
				componentChangeService.OnComponentChanging(dataSource, XRSqlDataSourceDesigner.SqlQueryCollectionPropertyDescriptor);
				componentChangeService.OnComponentChanging(dataSource, XRSqlDataSourceDesigner.ResultSchemaPropertyDescriptor);
				if(!dataSource.ManageQueries(new EditQueryContext { LookAndFeel = lookAndFeel, Owner = owner, DBSchemaProvider = dbSchemaProvider, ParameterService = parameterService, Options = options, QueryValidator = customQueryValidator})) {
					transaction.Cancel();
					return value;
				}
				componentChangeService.OnComponentChanged(dataSource, XRSqlDataSourceDesigner.SqlQueryCollectionPropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(dataSource, XRSqlDataSourceDesigner.ResultSchemaPropertyDescriptor, null, null);
				transaction.Commit();
				ISelectionService selectionService = context.GetService<ISelectionService>();
				selectionService.SetSelectedComponents(new object[] { host.RootComponent });
				selectionService.SetSelectedComponents(new object[] { context.Instance });
				return dataSource.Queries;
			}
		}
	}
}
