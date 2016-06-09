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
	abstract class AppLayerConstructorBase : IAppLayerConstructor {
		MvvmConstructorContext context;
		IServiceContainer serviceContainer;
		public AppLayerConstructorBase(MvvmConstructorContext context, IServiceContainer serviceContainer) {
			this.context = context;
			this.serviceContainer = serviceContainer;
		}	   
		public IEnumerable<IStepByStepConfiguratorPageViewModel<MvvmConstructorContext>> GetPages(IViewModelBase parentViewModel) {
			IMvvmConstructorPageViewModel startPage = GetStartPage(parentViewModel);
			AssertionException.IsNotNull(startPage);
			return new IMvvmConstructorPageViewModel[] { startPage };
		}
		public abstract IEnumerable<IHasName> GetEntryItems(ConstructorEntry entry, bool showLocalOnly);
		public abstract bool IsVisibleWithoutItems(ConstructorEntry entry);
		public virtual bool IsUnsupported(IHasName entryItem) { return false; }
		public virtual IMvvmConstructorPageViewModel GetStartPage(IViewModelBase parentViewModel) {
			return new ConstructorStartPageViewModel(parentViewModel, Context, this);
		}
		public abstract IEnumerable<ConstructorEntry> GetAvailableEntries();
		public abstract IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry);
		public MvvmConstructorContext Context {
			get {
				return this.context;
			}
		}
		public IServiceContainer ServiceContainer {
			get {
				return serviceContainer;
			}		   
		}
		public virtual bool CanCreateNewItemsOf(ConstructorEntry entry) {
			return false;
		}
		public virtual IItemCreator GetEntryItemCreator(ConstructorEntry entry) {
			return null;
		}
		public virtual void EntryItemSelected(ConstructorEntry entry, IHasName selectedItem) {			
		}
	}
}
