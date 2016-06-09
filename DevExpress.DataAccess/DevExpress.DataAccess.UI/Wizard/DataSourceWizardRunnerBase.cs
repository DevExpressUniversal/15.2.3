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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Utils.IoC;
namespace DevExpress.DataAccess.UI.Wizard {
	public abstract class DataSourceWizardRunnerBase<TModel, TClient> : IDisposable
		where TModel : IDataComponentModel, new() 
		where TClient : IDataSourceWizardClientBase
	{
		Wizard<TModel> wizard;
		IWizardRunnerContext context;
		IWizardView wizardView;
		bool isDisposed;
		protected DataSourceWizardRunnerBase(UserLookAndFeel lookAndFeel, IWin32Window owner)
			: this(new DefaultWizardRunnerContext(lookAndFeel, owner)) { }
		protected DataSourceWizardRunnerBase(IWizardRunnerContext context) {
			this.context = context;
		}
		public TModel WizardModel {   
			get {
				if(wizard != null) return wizard.GetResultModel();
				return default(TModel);
			}
		}
		protected abstract Type StartPage { get; }
		protected abstract string WizardTitle { get; }
		protected virtual Size WizardSize { get { return Size.Empty; } }
		public bool Run(TClient client, TModel model, Action<IWizardCustomization<TModel>> callback) {
			if(isDisposed)
				throw new ObjectDisposedException(this.ToString());
			var pageFactory = CreatePageFactory(client);
			pageFactory.Container.RegisterInstance<IWizardRunnerContext>(this.context);
			WizardCustomization<TModel> customiztion = new WizardCustomization<TModel>(pageFactory.Container, model, context) { 
				StartPage = this.StartPage,
				WizardTitle = this.WizardTitle,
				WizardSize = this.WizardSize
			};
			callback(customiztion);
			wizardView = context.CreateWizardView(customiztion.WizardTitle, customiztion.WizardSize);
			wizard = new Wizard<TModel>(wizardView, model, pageFactory);
			wizard.SetStartPage(customiztion.StartPage);
			return context.Run(wizard); 
		}
		public bool Run(TClient client, TModel model) {
			return Run(client, model, _ => { });
		}
		public bool Run(TClient client) {
			TModel model = new TModel();
			return Run(client, model);
		}		
		protected abstract WizardPageFactoryBase<TModel, TClient> CreatePageFactory(TClient client);
		public void Dispose() {
			isDisposed = true;
			if (wizardView is IDisposable) {
				((IDisposable)wizardView).Dispose();
			}
		}
	}
	public interface IWizardCustomization<TModel> where TModel : IWizardModel {
		Type StartPage { get; set; }
		string WizardTitle { get; set; }
		Size WizardSize { get; set; }
		TModel Model { get; }
		void RegisterPage<TPageType, TPageInstance>()
			where TPageInstance : TPageType
			where TPageType : IWizardPage<TModel>;
		void RegisterPageView<TViewType, TViewInstance>()
			where TViewInstance : WizardViewBase, TViewType;
		bool OpenConnection(DataConnectionBase dataConnection);
		object Resolve(Type serviceType);
	}
	class WizardCustomization<TModel> : IWizardCustomization<TModel> where TModel : IWizardModel {
		IntegrityContainer container;
		IWizardRunnerContext context;
		public Type StartPage { get; set; } 
		public string WizardTitle { get; set; }
		public Size WizardSize { get; set; }
		public TModel Model { get; private set; }
		public WizardCustomization(IntegrityContainer container, TModel model, IWizardRunnerContext context) {
			this.container = container;
			Model = model;
			this.context = context;
		}
		public void RegisterPage<TPageType, TPageInstance>() 
			where TPageInstance : TPageType
			where TPageType : IWizardPage<TModel> {
			container.RegisterType<TPageType, TPageInstance>();
		}
		public void RegisterPageView<TViewType, TViewInstance>() where TViewInstance : WizardViewBase, TViewType {
			container.RegisterType<TViewType, TViewInstance>();
		}
		public bool OpenConnection(DataConnectionBase dataConnection) {
			IExceptionHandler exceptionHandler = context.CreateExceptionHandler(ExceptionHandlerKind.Default);
			IWaitFormActivator waitFormActivator = context.WaitFormActivator;
			return ConnectionHelper.OpenConnection(dataConnection, exceptionHandler, waitFormActivator, null);
		}
		public object Resolve(Type serviceType) {
			return container.Resolve(serviceType);
		}
	}
}
