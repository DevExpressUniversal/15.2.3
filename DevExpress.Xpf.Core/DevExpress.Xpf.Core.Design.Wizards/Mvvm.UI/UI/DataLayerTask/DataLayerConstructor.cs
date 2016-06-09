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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Design.UI;
using DevExpress.Design.DataAccess;
using System.Threading;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Entity.ProjectModel;
using DevExpress.Entity.Model;
using DevExpress.Utils;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class DataLayerConstructor : AppLayerConstructorBase {
		public DataLayerConstructor(MvvmConstructorContext context, IServiceContainer serviceContainer)
			: base(context, serviceContainer) {
		}
		protected IEntityFrameworkModel EntityFrameworkModel { get { return this.ServiceContainer.Resolve<IEntityFrameworkModel>(); } }
		public override IEnumerable<IHasName> GetEntryItems(ConstructorEntry entry, bool showLocalOnly) {
			if(entry == ConstructorEntry.WCFDataService)
				return GetEntryItems(showLocalOnly).Where(c => c.ContainerType == DbContainerType.WCF);
			if(entry == ConstructorEntry.EntityFrameworkModel)
				return GetEntryItems(showLocalOnly).Where(c => c.ContainerType == DbContainerType.EntityFramework);
			return new IHasName[0];
		}
		public override bool IsVisibleWithoutItems(ConstructorEntry entry) {
			return entry != ConstructorEntry.WCFDataService;
		}
		IEnumerable<IContainerInfo> GetEntryItems(bool showLocalOnly) {
			return EntityFrameworkModel.GetContainersInfo().Where(ci => !showLocalOnly || ci.IsSolutionType);
		}
		public override bool IsUnsupported(IHasName entryItem) {
			IContainerInfo containerInfo = entryItem as IContainerInfo;
			return containerInfo != null && !IsAppropriateContainer(containerInfo);
		}
		static bool IsAppropriateContainer(IContainerInfo containerInfo) {
			return containerInfo.ContainerType == DbContainerType.WCF || EntityFrameworkModelBase.IsAtLeastEF6(containerInfo);
		}
		public override IEnumerable<ConstructorEntry> GetAvailableEntries() {
			yield return ConstructorEntry.WCFDataService;
			yield return ConstructorEntry.EntityFrameworkModel;
		}
		IMvvmConstructorPageViewModel[] pages;
		public override IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry) {			
			if(pages != null)
				return pages;
			pages = new IMvvmConstructorPageViewModel[]{new TablesSelectorViewModel(parentViewModel, this.Context) };
			return pages;
		}
		public override bool CanCreateNewItemsOf(ConstructorEntry entry) {
			return entry == ConstructorEntry.EntityFrameworkModel;
		}
		public override IItemCreator GetEntryItemCreator(ConstructorEntry entry) {
			if(entry == ConstructorEntry.EntityFrameworkModel)
				return new EntityFrameworkItemCreatorTask(this.ServiceContainer, SR_Mvvm.GetWizardTitle(this.Context));
			return null;
		}
		public override void EntryItemSelected(ConstructorEntry entry, IHasName selectedItem) {
			this.Context.DbContextCandidate = selectedItem as IContainerInfo;
		}
		#region EntityFrameworkItemCreator
		public class EntityFrameworkItemCreatorTask : SimpleWizardTask, IItemCreator {
			readonly IServiceContainer serviceContainer;
			string wizardTitle;
			public EntityFrameworkItemCreatorTask(IServiceContainer serviceContainer, string wizardTitle) {
				this.serviceContainer = serviceContainer;
				this.wizardTitle = wizardTitle;
				this.Action = () => {
					SynchronizationContext.Current.Post(delegate(object state){
						try {
							DevExpress.Design.DataAccess.DefaultDataAccessTechnologyNewItemFactory.EntityFrameworkItemCreator creator =
								new DevExpress.Design.DataAccess.DefaultDataAccessTechnologyNewItemFactory.EntityFrameworkItemCreator();
							creator.Create();							
						}
						catch(Exception ex) {
							Log.SendException(ex);
						}						
					}, null);
				};
			}
			public void Create() {
				IWizardTaskManager taskManager = serviceContainer.Resolve<IWizardTaskManager>();
				if(!taskManager.Tasks.Contains(this))
					taskManager.Add(this);
			}
			public string Message {
				get {					
					return string.Format(SR_Mvvm.DataAccessLayerWizard_RebuildMessage, wizardTitle); 
				}
			}
			public bool IsCloseDialogNeeded {
				get { return true; }
			}			
		}
		#endregion
	}
}
