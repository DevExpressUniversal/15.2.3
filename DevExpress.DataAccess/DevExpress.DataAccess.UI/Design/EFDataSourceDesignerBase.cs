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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.DataAccess.UI.Design {
	public abstract class EFDataSourceDesignerBase : ComponentDesigner {
		readonly DesignerVerbCollection verbs = new DesignerVerbCollection();
		protected IDesignerHost designerHost;
		protected ILookAndFeelService lookAndFeelService;
		protected IComponentChangeService componentChangeService;
		protected IConnectionStorageService connectionStorageService;
		protected ISolutionTypesProvider solutionTypesProvider;
		protected IConnectionStringsProvider connectionStringsProvider;
		protected IParameterService parameterService;
		protected IWin32Window owner;
		public override DesignerVerbCollection Verbs { get { return verbs; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			CreateServices();
			AddServicesToProviders();
			CreateEditVerb();
		}
		void CreateServices() {
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			lookAndFeelService = designerHost.GetService<ILookAndFeelService>();
			if(lookAndFeelService == null) {
				lookAndFeelService = new LookAndFeelService();
			}
			connectionStorageService = CreateConnectionStorageService();
			solutionTypesProvider = CreateSolutionTypesProvider();
			connectionStringsProvider = CreateConnectionStringsProvider();
			parameterService = (IParameterService)GetService(typeof(IParameterService));
			IUIService uiService = (IUIService)GetService(typeof(IUIService));
			owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
		}
		void AddServicesToProviders() {
			AddServicesToDesignerHost();
			AddServicesToDataSource();
		}
		void CreateEditVerb() {
			verbs.Clear();
			verbs.Add(new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.EFDataSourceDesignerVerbEdit), EditVerbHandler));
		}
		protected abstract void EditVerbHandler(object sender, EventArgs a);
		protected abstract ISolutionTypesProvider CreateSolutionTypesProvider();
		protected abstract IConnectionStringsProvider CreateConnectionStringsProvider();
		protected abstract IConnectionStorageService CreateConnectionStorageService();
		protected virtual void AddServicesToDesignerHost() { }
		protected abstract void AddServicesToDataSource();
	}
}
