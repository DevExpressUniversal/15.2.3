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
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class QueryEditor : UITypeEditor {
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			SqlQueryCollection queryOwner = context.Instance as SqlQueryCollection;
			if(queryOwner == null)
				return base.EditValue(context, provider, value);
			SqlQuery query = (SqlQuery) value;
			int index = queryOwner.IndexOf(query);
			var dataSource = queryOwner.DataSource;
			IDesignerHost designerHost = context.GetService<IDesignerHost>();
			XRSqlDataSourceDesigner designer = (XRSqlDataSourceDesigner) designerHost.GetDesigner(queryOwner.DataSource);
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			var sqlWizardOptionsProvider = context.GetService<ISqlWizardOptionsProvider>();
			SqlWizardOptions options = sqlWizardOptionsProvider != null ? sqlWizardOptionsProvider.SqlWizardOptions : SqlWizardOptions.EnableCustomSql;
			ICustomQueryValidator customQueryValidator = context.GetService<ICustomQueryValidator>();
			var parameterService = context.GetService<IParameterService>();
			IDBSchemaProvider dbSchemaProvider = context.GetService<IDBSchemaProvider>();
			IComponentChangeService changeService = context.GetService<IComponentChangeService>();
			using(UserLookAndFeel lookAndFeel = designer.GetLookAndFeel(context))
			using(DesignerTransaction transaction = designerHost.CreateTransaction(string.Format("Edit Queries[{0}]", index))) {
				changeService.OnComponentChanging(dataSource, null);
				if (!query.EditQuery(new EditQueryContext { LookAndFeel = lookAndFeel, Owner = owner, DBSchemaProvider = dbSchemaProvider, ParameterService = parameterService, Options = options, QueryValidator = customQueryValidator })) {
					transaction.Cancel();
					return value;
				}
				changeService.OnComponentChanged(dataSource, null, null, null);
				transaction.Commit();
				return queryOwner[index];
			}
		}
   }
}
