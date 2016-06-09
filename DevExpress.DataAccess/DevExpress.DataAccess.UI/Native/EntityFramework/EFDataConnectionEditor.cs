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
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	public class EFDataConnectionEditor : UITypeEditor {
		static readonly PropertyDescriptor connectionParametersPropertyDescriptor = TypeDescriptor.GetProperties(typeof(EFDataSource))["ConnectionParameters"];
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			EFDataSource dataSource = context.Instance as EFDataSource;
			if(dataSource == null)
				return base.EditValue(context, provider, value);
			EFDataConnection dataConnection = value as EFDataConnection;
			IDesignerHost designerHost = context.GetService<IDesignerHost>();
			ISolutionTypesProvider solutionTypesProvider = context.GetService<ISolutionTypesProvider>();
			IConnectionStringsProvider connectionStringsProvider = context.GetService<IConnectionStringsProvider>();
			IConnectionStorageService connectionStorage = context.GetService<IConnectionStorageService>();
			UserLookAndFeel lookAndFeel = context.GetService<ILookAndFeelService>().LookAndFeel;
			IUIService uiService = context.GetService<IUIService>();
			IParameterService parameterService = context.GetService<IParameterService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IComponentChangeService changeService = designerHost.GetService<IComponentChangeService>();
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Edit Connection")) {
				changeService.OnComponentChanging(dataSource, connectionParametersPropertyDescriptor);
				DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, owner);
				if(!dataSource.EditConnection(wizardRunnerContext, solutionTypesProvider, connectionStringsProvider, connectionStorage, parameterService)) {
					transaction.Cancel();
				}
				changeService.OnComponentChanged(dataSource, connectionParametersPropertyDescriptor, null, null);
				transaction.Commit();
			}
			return dataSource.Connection;
		}
	}
}
