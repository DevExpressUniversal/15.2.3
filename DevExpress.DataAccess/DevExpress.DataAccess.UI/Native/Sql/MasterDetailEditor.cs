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
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class MasterDetailEditor : UITypeEditor {
		static readonly PropertyDescriptor relationsDescriptor = TypeDescriptor.GetProperties(typeof(SqlDataSource))["Relations"];
		public static PropertyDescriptor RelationsDescriptor {
			get { return relationsDescriptor; }
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			MasterDetailInfoCollection relations = value as MasterDetailInfoCollection;
			if(relations == null)
				return value;
			IUIService uiService = context.GetService<IUIService>();
			IDesignerHost host = context.GetService<IDesignerHost>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			XRSqlDataSourceDesigner designer = (XRSqlDataSourceDesigner) host.GetDesigner(relations.Owner);
			SqlDataSource sqlDataSource = relations.Owner;
			IDBSchemaProvider dbSchemaProvider = context.GetService<IDBSchemaProvider>();
			IComponentChangeService componentChangeService = context.GetService<IComponentChangeService>();
			using(UserLookAndFeel lookAndFeel = designer.GetLookAndFeel(context))
			using(DesignerTransaction transaction = host.CreateTransaction("Manage Relations")) {
				componentChangeService.OnComponentChanging(sqlDataSource, RelationsDescriptor);
				if(!sqlDataSource.ManageRelations(new ManageRelationsContext{ LookAndFeel = lookAndFeel, Owner = owner, DBSchemaProvider = dbSchemaProvider })) {
					transaction.Cancel();
					return value;
				}
				componentChangeService.OnComponentChanged(sqlDataSource, RelationsDescriptor, null, null);
				transaction.Commit();
				return sqlDataSource.Relations;
			}
		}
	}
}
