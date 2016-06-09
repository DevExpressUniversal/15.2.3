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
using DevExpress.Entity.ProjectModel;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class ViewModelConstructor : DataLayerConstructor {
		public ViewModelConstructor(MvvmConstructorContext context, IServiceContainer serviceContainer)
			: base(context, serviceContainer) {			
		}
		protected IDataAccessLayerService DataModelService { get { return this.ServiceContainer.Resolve<IDataAccessLayerService>(); } }
		ConstructorEntry[] availableEntries;
		public override IEnumerable<ConstructorEntry> GetAvailableEntries() {
			if(availableEntries != null)
				return availableEntries;
			List<ConstructorEntry> result = new List<ConstructorEntry>();
			if(DataModelService.GetAvailableDataModels().Any())
				result.Add(ConstructorEntry.DataModel);
			foreach(ConstructorEntry entry in base.GetAvailableEntries())
				result.Add(entry);
			availableEntries = result.ToArray();
			return availableEntries;
		}
		public override IEnumerable<IHasName> GetEntryItems(ConstructorEntry entry, bool showLocalOnly) {
			if(entry == ConstructorEntry.DataModel)
				foreach(IDataModel model in DataModelService.GetAvailableDataModels())
					if(showLocalOnly && !model.IsInSolution)
						continue;
					else
						yield return model;
			foreach(var item in base.GetEntryItems(entry, showLocalOnly))
				yield return item;
		}
		protected IEnumerable<IMvvmConstructorPageViewModel> GetOwnPages(IViewModelBase parentViewModel, bool isKeyCorrectionNeeded) {
		return new IMvvmConstructorPageViewModel[] { new ViewModelEntityDefinitionsViewModel(parentViewModel, this.Context) { IsKeyCorrectionNeeded = isKeyCorrectionNeeded } };			
		}
		public override IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry) {
			if(entry == ConstructorEntry.DataModel)
				foreach(IMvvmConstructorPageViewModel page in GetOwnPages(parentViewModel, true))
					yield return page;
			else {
				foreach(IMvvmConstructorPageViewModel page in base.GetPages(parentViewModel, entry))
					yield return page;
				foreach(IMvvmConstructorPageViewModel page in GetOwnPages(parentViewModel, false))
					yield return page;
			}
		}		
		public override void EntryItemSelected(ConstructorEntry entry, IHasName selectedItem) {
			if(entry == ConstructorEntry.DataModel)
				this.Context.SelectedDataModel = selectedItem as IDataModel;
			else
				base.EntryItemSelected(entry, selectedItem);
		}		
	}
}
