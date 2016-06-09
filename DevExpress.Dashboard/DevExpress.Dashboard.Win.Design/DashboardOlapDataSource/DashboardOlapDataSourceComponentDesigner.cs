#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.DataAccess.Design;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using DevExpress.DashboardWin.Native;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
namespace DevExpress.DashboardWin.Design {
	public class DashboardOlapDataSourceComponentDesigner : ComponentDesigner {
		IDesignerHost designerHost;
		IComponentChangeService changeService;
		DashboardOlapDataSource OlapDataSource { get { return (DashboardOlapDataSource)Component; } } 
		public override DesignerVerbCollection Verbs {
			get {
				DesignerVerbCollection verbs = base.Verbs;
				verbs.Add(new DesignerVerb("Configure Connection...", OnConfigureConnectionClick));
				return verbs;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			designerHost = GetService(typeof(IDesignerHost)) as IDesignerHost;
			changeService = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
			if (designerHost.GetService(typeof(ILookAndFeelService)) == null)
				designerHost.AddService(typeof(ILookAndFeelService), new VSLookAndFeelService(designerHost));
			if (designerHost.GetService(typeof(IDashboardGuiContextService)) == null)
				designerHost.AddService(typeof(IDashboardGuiContextService), new DashboardGuiContextServiceDesignTime(designerHost));			
			ReplaceConnectionStringProvider();
			ReplaceConnectionStorageService();
		}
		void ReplaceConnectionStringProvider() {
			IDisposable connectionStringProviderDisposable = OlapDataSource.ConnectionStringsProvider as IDisposable;
			if (connectionStringProviderDisposable != null)
				connectionStringProviderDisposable.Dispose();
			OlapDataSource.ConnectionStringsProvider = new VSConnectionStringsService(designerHost);
		}
		void ReplaceConnectionStorageService() {
			IDisposable connectionStorageServiceDisposable = OlapDataSource.ConnectionStorageService as IDisposable;
			if (connectionStorageServiceDisposable != null)
				connectionStorageServiceDisposable.Dispose();
			OlapDataSource.ConnectionStorageService = new VSConnectionStorageService(designerHost);
		}
		void OnConfigureConnectionClick(object sender, EventArgs e) {
			string transactionName = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemEditDataSource), OlapDataSource.Name);
			DesignerTransaction transaction = designerHost.CreateTransaction(transactionName);
			try {
				changeService.OnComponentChanging(OlapDataSource, null);
				bool chaged = OlapDataSource.ConfigureConnection(designerHost);
				if (chaged) {
					changeService.OnComponentChanged(OlapDataSource, null, null, null);
					transaction.Commit();
				}
				else
					transaction.Cancel();
			}
			catch {
				transaction.Cancel();
			}
		}
	}
}
