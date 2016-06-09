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
using System.ComponentModel;
using DevExpress.Design.DataAccess;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	internal interface IMvvmConstructorPageViewModel : IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>, IViewModelBase, INotifyPropertyChanged {
		string StepDescription { get; }
		bool WithUndo { get; }
	}
	enum StepByStepDirection {
		None,
		Back,
		Next
	}
	internal interface IMvvmConstructorViewModel : IStepByStepConfiguratorViewModel<IMvvmConstructorPageViewModel, MvvmConstructorContext>, IDXDesignWindowContentViewModel, IViewModelBase {
		IEnumerable<IStepsLineItem> StepsLineItems { get; set; }
		StepByStepDirection StepDirection { get; }
	}
	internal class MvvmConstructorViewModel : StepByStepConfiguratorViewModel<IMvvmConstructorPageViewModel, MvvmConstructorContext>, IMvvmConstructorViewModel, IStepByStepConfiguratorViewModel<IMvvmConstructorPageViewModel, MvvmConstructorContext>, IDXDesignWindowContentViewModel, IViewModelBase, INotifyPropertyChanged {		
		IEnumerable<IStepsLineItem> stepsLineItems;
		IAppLayerConstructor constructor;
		StepByStepDirection direction;
		IMvvmConstructorPageViewModel[] pages;
		public MvvmConstructorViewModel(IViewModelBase parentViewModel, MvvmConstructorContext context)
			: base(parentViewModel, (context != null) ? context : new MvvmConstructorContext()) {
			this.direction = StepByStepDirection.None;
		}
		protected override IServiceContainer CreateServiceContainer() {
			return new ServiceCache(base.GetParentServiceContainer());
		}
		protected override void EnterPage(IMvvmConstructorPageViewModel page) {
			if((page != null) && page.WithUndo) {
				if(this.direction == StepByStepDirection.Back)
					this.UndoManager.Undo();
				this.UndoManager.OpenMultyUndoUnit();
			}
			base.EnterPage(page);
		}
		protected override IEnumerable<IMvvmConstructorPageViewModel> GetPages() {
			if(this.pages == null) {
				IEnumerable<IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>> appLayerConstructorGetPages = this.AppLayerConstructor.GetPages(this);
				this.pages = appLayerConstructorGetPages.Cast<IMvvmConstructorPageViewModel>().ToArray<IMvvmConstructorPageViewModel>();
			}
			return this.pages;
		}
		protected override void InitServices() {
			base.InitServices();
			base.ServiceContainer.Register<IDataAccessTechnologyNewItemService, DefaultDataAccessTechnologyNewItemFactory>();
		}
		protected override void LeavePage(IMvvmConstructorPageViewModel page) {
			base.LeavePage(page);
			if((page != null) && page.WithUndo) {
				this.UndoManager.CloseActiveMultyUndoUnit();
				if(this.direction == StepByStepDirection.Back) {
					this.UndoManager.Undo();
				}
			}
		}
		protected override void OnFinished(IMvvmConstructorPageViewModel selectedPage) {
			this.direction = StepByStepDirection.Next;
			try {
				base.SelectedPage = null;
			}
			finally {
				this.direction = StepByStepDirection.None;
			}
			try {
				IWizardTaskManager taskManager = base.ServiceContainer.Resolve<IWizardTaskManager>();
				if (!taskManager.IsStarted)
					taskManager.Start();
			}
			catch(Exception ex) {
				Log.SendException(ex);
			}
			finally {
				IDXDesignWindowViewModel windowModel = GetParentViewModel<IDXDesignWindowViewModel>();
				if(windowModel != null && windowModel.Window != null && !windowModel.Window.DialogResult.HasValue)
					base.OnFinished(selectedPage);
			}
		}
		protected override void OnSelectedIndexChanged(int oldValue, int newValue) {
			this.direction = StepByStepDirection.None;
			if(oldValue < newValue) {
				this.direction = StepByStepDirection.Next;
			}
			else if(oldValue > newValue) {
				this.direction = StepByStepDirection.Back;
			}
			try {
				base.OnSelectedIndexChanged(oldValue, newValue);
			}
			finally {
				this.direction = StepByStepDirection.None;
			}
		}
		protected override void UpdateIsAvailableProperties(int selectedIndex) {
			base.UpdateIsAvailableProperties(selectedIndex);
			ApplyAvailabilityMask();
		}
		void ApplyAvailabilityMask() {
			if(StepsLineItems == null)
				return;
			foreach(IStepsLineItem stepsLineItem in StepsLineItems) {
				stepsLineItem.IsActive = this.SelectedPage != null && stepsLineItem.Name == this.SelectedPage.StepDescription;
				stepsLineItem.IsEnabled = this.Pages.Any(p => p.StepDescription == stepsLineItem.Name);
			}
		}
		protected IAppLayerConstructor AppLayerConstructor {
			get {
				if(this.constructor == null) {
					if(base.Context.PlatformType == PlatformType.WinForms) {
						this.constructor = new WinConstructor(base.Context, base.ServiceContainer);
					} else
						if(this.constructor == null) {
							switch(base.Context.TaskType) {
								case TaskType.DataLayer:
									this.constructor = new DataLayerConstructor(base.Context, base.ServiceContainer);
									break;
								case TaskType.ViewModelLayer:
									this.constructor = new ViewModelConstructor(base.Context, base.ServiceContainer);
									break;
								case TaskType.ViewLayer:
									this.constructor = new ViewConstructor(base.Context, base.ServiceContainer);
									break;
								case TaskType.TabbedMDI:
									this.constructor = new TabbedMDIConstrucor(base.Context, base.ServiceContainer);
									break;
							}
						}
				}
				return this.constructor;
			}
		}
		public StepByStepDirection StepDirection {
			get {
				return this.direction;
			}
		}
		private IUndoManager UndoManager {
			get {
				return base.ServiceContainer.Resolve<IUndoManager>();
			}
		}
		public IEnumerable<IStepsLineItem> StepsLineItems {
			get {
				return stepsLineItems;
			}
			set {
				if(SetProperty<IEnumerable<IStepsLineItem>>(ref stepsLineItems, value, "StepsLineItems"))
					ApplyAvailabilityMask();
			}
		}
	}	
}
