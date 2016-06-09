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
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.UI.ObjectBinding;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.UI.Native.ObjectBinding {
	public class CtorParametersEditor : UITypeEditor {
		#region Overrides of UITypeEditor
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) { return UITypeEditorEditStyle.Modal; }
		#endregion
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			ObjectDataSource ods = context.Instance as ObjectDataSource;
			if(ods == null)
				return base.EditValue(context, provider, value);
			UserLookAndFeel lookAndFeel = context.GetService<ILookAndFeelService>().LookAndFeel;
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IParameterService parameterService = context.GetService<IParameterService>();
			IDesignerHost designerHost = context.GetService<IDesignerHost>();
			ISolutionTypesProvider solutionTypesProvider = designerHost.GetService<ISolutionTypesProvider>();
			IComponentChangeService componentChangeService = context.GetService<IComponentChangeService>();
			IRepositoryItemsProvider repositoryItemsProvider = context.GetService<IRepositoryItemsProvider>() ?? DefaultRepositoryItemsProvider.Instance;
			DesignerTransaction transaction = designerHost.CreateTransaction("Edit CtorParameters");
			componentChangeService.OnComponentChanging(ods, null);
			DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(lookAndFeel, owner);
			if(ods.EditConstructor(solutionTypesProvider, wizardRunnerContext, parameterService, repositoryItemsProvider)) {
				componentChangeService.OnComponentChanged(ods, null, null, null);
				transaction.Commit();
			}
			else
				transaction.Cancel();
			return ods.Constructor;
		}
	}
}
