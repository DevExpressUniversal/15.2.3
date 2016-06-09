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
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class SqlQueryParametersEditor : UITypeEditor {
		SqlQuery editQuery;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.editQuery = (SqlQuery) context.Instance;
			bool previewRowLimit = this.editQuery is TableQuery;
			bool fixedParameters = this.editQuery is StoredProcQuery;
			IDesignerHost designerHost = context.GetService<IDesignerHost>();
			XRSqlDataSourceDesigner designer = (XRSqlDataSourceDesigner) designerHost.GetDesigner(this.editQuery.Owner.DataSource);
			IParameterService parameterService = provider.GetService<IParameterService>();
			var dataConnectionParametersService = this.editQuery.DataSource.GetService<IDataConnectionParametersService>();
			IDBSchemaProvider dbSchemaProvider = context.GetService<IDBSchemaProvider>();
			IRepositoryItemsProvider repositoryItemsProvider = context.GetService<IRepositoryItemsProvider>() ?? DefaultRepositoryItemsProvider.Instance;
			ICustomQueryValidator customQueryValidator = context.GetService<ICustomQueryValidator>() ?? new CustomQueryValidator();
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			using(UserLookAndFeel lookAndFeel = designer.GetLookAndFeel(context))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Edit parameters"))
			using(ParametersGridFormBase form = new SqlQueryParametersForm(this.editQuery, previewRowLimit, fixedParameters, lookAndFeel, designerHost, parameterService, dbSchemaProvider, dataConnectionParametersService, repositoryItemsProvider, customQueryValidator) {
				StartPosition = FormStartPosition.CenterParent
			}) {
				context.OnComponentChanging();
				if(form.ShowDialog(owner) != DialogResult.OK) {
					transaction.Cancel();
					return value;
				}
				value = form.GetParameters().Select(QueryParameter.FromIParameter).ToList();
				this.editQuery.Parameters.Clear();
				this.editQuery.Parameters.AddRange((List<QueryParameter>) value);
				context.OnComponentChanged();
				transaction.Commit();
			}
			return value;
		}
	}
}
