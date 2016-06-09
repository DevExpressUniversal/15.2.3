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
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Native.Data;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraWaitForm;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	public class EFStoredProcedureParametersEditor : UITypeEditor {
		const bool previewRowLimit = false;
		const bool fixedParameters = true;
		static readonly PropertyDescriptor StoredProceduresDescriptor = TypeDescriptor.GetProperties(typeof(EFDataSource))["StoredProcedures"];
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			EFStoredProcedureInfo editStoredProcedure = context.Instance as EFStoredProcedureInfo;
			EFDataSource dataSource = editStoredProcedure.ParentCollection.DataSource;
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IDesignerHost host = context.GetService<IDesignerHost>();
			UserLookAndFeel lookAndFeel = context.GetService<ILookAndFeelService>().LookAndFeel;
			IParameterService parameterService = provider.GetService<IParameterService>();
			IWaitFormActivator waitFormActivator = new WaitFormActivatorDesignTime(owner, typeof(DemoWaitForm), lookAndFeel.ActiveSkinName);
			IComponentChangeService componentChangeService = context.GetService<IComponentChangeService>();
			IRepositoryItemsProvider repositoryItemsProvider = context.GetService<IRepositoryItemsProvider>() ?? DefaultRepositoryItemsProvider.Instance;
			if(!ConnectionHelper.OpenConnection(dataSource.Connection, new ConnectionExceptionHandler(owner, lookAndFeel), waitFormActivator))
				return value;
			using(DesignerTransaction transaction = host.CreateTransaction("Edit Stored Procedure Parameters"))
			using(ParametersGridFormBase form = new EFStoredProcedureParametersForm(editStoredProcedure, previewRowLimit, fixedParameters, lookAndFeel, host, parameterService, repositoryItemsProvider)) {
				componentChangeService.OnComponentChanging(dataSource, StoredProceduresDescriptor);
				if(form.ShowDialog(owner) == DialogResult.OK) {
					value = form.GetParameters().Select(EFParameter.FromIParameter);
					editStoredProcedure.Parameters.Clear();
					editStoredProcedure.Parameters.AddRange((IEnumerable<EFParameter>)value);
					componentChangeService.OnComponentChanged(dataSource, StoredProceduresDescriptor, null, null);
					transaction.Commit();
				} else
					transaction.Cancel();
			}
			return value;
		}
	}
}
