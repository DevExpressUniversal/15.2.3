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
using System.Windows;
using System.Windows.Interop;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.MetaData;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
using DevExpress.Design.Mvvm.Wizards.UI;
using DevExpress.Design.UI;
using DevExpress.Design.Mvvm;
using DevExpress.Utils.Design;
using DevExpress.Design.Mvvm.Wizards;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.ViewModelData;
using DevExpress.Utils.About;
using DevExpress.Design.Mvvm.Wizards.Diagnostics;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public abstract class MvvmWizardBase : WizardTaskManager {
		public MvvmWizardBase()
			: base(null) {
		}
		protected virtual void InitServices(IWizardRunContext wizardRunContext) {
			if(ServiceContainer == null)
				return;
			ServiceContainer.AddService(typeof(ITemplatesPlatform), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new DTETemplatesPlatform(container)));
			ServiceContainer.AddService(typeof(IUndoManager), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new DevExpress.Design.Mvvm.Wizards.UI.BaseUndoManager()));
			ServiceContainer.AddService(typeof(IWizardTaskManager), this);
			ServiceContainer.AddService(typeof(ISolutionTypesProvider), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new SolutionTypesProvider()));
			ServiceContainer.AddService(typeof(IEntityFrameworkModel), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new DevExpress.Xpf.Core.Design.Wizards.Mvvm.EntityFramework.EntityFrameworkModel(container)));
			ServiceContainer.AddService(typeof(IDataAccessLayerService), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new DataAccesLayerService(container)));
			ServiceContainer.AddService(typeof(ITemplatesCodeGen), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new TemplatesCodeGen(container)));
			ServiceContainer.AddService(typeof(IViewModelLayerService), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => new ViewModelLayerService(container)));
			ServiceContainer.AddService(typeof(IWizardRunContext), new System.ComponentModel.Design.ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type serviceType) => {
				if(wizardRunContext != null)
					return wizardRunContext;
				return new DefaultWizardRunContext();
			}));
			DefaultLogServicesImpl logServices = new DefaultLogServicesImpl();
			ServiceContainer.AddService(typeof(ILogServices), logServices);
		}
		protected abstract TaskType TaskType { get; }
		void SetOwner(Window window) {
			if(window == null)
				return;
			IntPtr ptr = GetMainWindowHandle();
			if(ptr == IntPtr.Zero)
				return;
			HwndSource hwndSource = HwndSource.FromHwnd(ptr);
			Window wnd = hwndSource == null ? null : hwndSource.RootVisual as Window;
			if(wnd != null)
				window.Owner = wnd;
		}
		IntPtr GetMainWindowHandle() {
			IntPtr result = IntPtr.Zero;
			EnvDTE.DTE dte = DTEHelper.GetCurrentDTE();
			if(dte == null)
				return result;
			EnvDTE.Window window = dte.MainWindow;
			if(window == null)
				return result;
			return new IntPtr(window.HWnd);
		}
		protected override Window GetUIWindow() {
			Window result = base.GetUIWindow();
			SetOwner(result);
			return result;
		}
		void UAlgoLogStarted() {
			string message = string.Format("MvvmWizardStarted:TaskType:{0};ViewModelType:{1};ViewType:{2}", this.TaskType, Context.SelectedViewModelType, Context.SelectedViewType);			
		}
		void UAlgoLogFinished() {
			string message = this.MainWindow != null && this.MainWindow.DialogResult.GetValueOrDefault() ? "MvvmWizardFinished" : "MvvmWizardClosed";
		}
		void UAlgoLogClosedWithException(Exception ex) {
			string message = string.Format("MvvmWizardClosedWithException:Message: {0}", ex.Message);
		}
		public void Run(IWizardRunContext wizardRunContext) {
			Context.TaskType = this.TaskType;
			try {				
				UAlgoLogStarted();
				InitServices(wizardRunContext);
				Run();
				UAlgoLogFinished();
			}
			catch(Exception ex) {
				UAlgoLogClosedWithException(ex);
				Log.SendException(ex);
			}
			finally {
				MetaDataServices.Reset();				
			}
		}	   
	}
}
