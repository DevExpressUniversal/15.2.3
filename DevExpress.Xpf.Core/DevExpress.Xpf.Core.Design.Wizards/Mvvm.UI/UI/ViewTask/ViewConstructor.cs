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
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class ViewConstructor : ViewModelConstructor {
		public ViewConstructor(MvvmConstructorContext context, IServiceContainer serviceContainer)
			: base(context, serviceContainer) {
		}
		IViewModelLayerService ViewModelLayerService { get { return this.ServiceContainer.Resolve<IViewModelLayerService>(); } }
		ConstructorEntry[] availableEntries;
		public override IEnumerable<ConstructorEntry> GetAvailableEntries() {
			if(availableEntries != null)
				return availableEntries;
			List<ConstructorEntry> result = new List<ConstructorEntry>();
			if(GetAvailableViewModels(true).Any())
				result.Add(ConstructorEntry.ViewModel);
			foreach(ConstructorEntry entry in base.GetAvailableEntries())
				result.Add(entry);
			availableEntries = result.ToArray();
			return availableEntries;
		}
		IEnumerable<IViewModelInfo> GetAvailableViewModels(bool showLocalOnly) {
			foreach(IViewModelInfo item in ViewModelLayerService.GetViewModelTypes()) {
				if(showLocalOnly && !item.IsSolutionType)
					continue;
				if(this.Context.SelectedViewType == ViewType.Entity && item.ViewModelType == ViewModelType.Entity)
					yield return item;
				else if(this.Context.SelectedViewType == ViewType.Repository && item.ViewModelType == ViewModelType.EntityRepository)
					yield return item;
			}
		}
		public override IEnumerable<IHasName> GetEntryItems(ConstructorEntry entry, bool showLocalOnly) {
			if(entry == ConstructorEntry.ViewModel)
				return GetAvailableViewModels(showLocalOnly);
			return base.GetEntryItems(entry, showLocalOnly);
		}
		IMvvmConstructorPageViewModel[] ownPages;
		IEnumerable<IMvvmConstructorPageViewModel> GetOwnPages(IViewModelBase parentViewModel) {
			if(ownPages != null)
				return ownPages;
			ownPages = new IMvvmConstructorPageViewModel[] { new ViewModelSelectorPageModel(parentViewModel, Context) };
			return ownPages;
		}
		public override IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry) {
			if(entry == ConstructorEntry.ViewModel)
				foreach(IMvvmConstructorPageViewModel page in GetOwnPages(parentViewModel))
					yield return page;
			else {
				foreach(IMvvmConstructorPageViewModel page in base.GetPages(parentViewModel, entry))
					yield return page;
				foreach(IMvvmConstructorPageViewModel page in GetOwnPages(parentViewModel))
					yield return page;
			}
		}
		public override void EntryItemSelected(ConstructorEntry entry, IHasName selectedItem) {
			if(entry == ConstructorEntry.ViewModel)
				this.Context.SelectedViewModel = selectedItem as IViewModelInfo;
			else
				base.EntryItemSelected(entry, selectedItem);
		}
	}
}
