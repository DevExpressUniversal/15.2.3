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
using System.Collections;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Entity.Model;
using System.Diagnostics;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class WinConstructor :TabbedMDIConstrucor {
		public WinConstructor(MvvmConstructorContext context, IServiceContainer serviceContainer)
			: base(context, serviceContainer) {
		}
		public override IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry) {
			if(entry == ConstructorEntry.DataModel)
				foreach(IMvvmConstructorPageViewModel page in GetOwnPages(parentViewModel, true))
					yield return page;
			else {
				foreach(var basePage in base.GetPages(parentViewModel, entry))
					yield return basePage;
			}
		}	 
	} 
	class TabbedMDIConstrucor : ViewModelConstructor {
		public TabbedMDIConstrucor(MvvmConstructorContext context, IServiceContainer serviceContainer)
			: base(context, serviceContainer) {
		}
		public override IEnumerable<IMvvmConstructorPageViewModel> GetPages(IViewModelBase parentViewModel, ConstructorEntry entry) {
			return new IMvvmConstructorPageViewModel[] {
				new TabbedMDIViewModel(parentViewModel, this.Context, entry)
			};
		}
	}
	class TabbedMDIViewModel : TablesSelectorViewModel {
		ConstructorEntry entry;
		public TabbedMDIViewModel(IViewModelBase parentViewModel, MvvmConstructorContext context, ConstructorEntry entry)
			: base(parentViewModel, context) {
			this.entry = entry;
		}
		protected override void BuildEntities() {
			try {
				if(entry == ConstructorEntry.DataModel) {
					ValidateContainerInfo(this.Context.SelectedDataModel.DbContainer);
					if(this.ModelHasErrors)
						return;
					Entities = BuildEntities(this.Context.SelectedDataModel.Entities);
					BeginItemStatesUpdating();
					try {
						if(this.Context.SelectedTables != null) {
							if(this.Context.SelectedTables.Length > 0)
								foreach(IEntitySetInfo info in this.Context.SelectedTables)
									foreach(EntitySetInfoViewModel entity in Entities)
										if(entity.Info == info) {
											entity.Checked = true;
											break;
										}
						} else
							foreach(EntitySetInfoViewModel entity in Entities)
								entity.Checked = true;
					} finally {
						EndItemStatesUpdating();
					}
					if(Entities.Any())
						SelectedInfo = Entities.First();
				} else
					base.BuildEntities();
				UpdateSelectAllState();
			} catch(Exception ex) {
				Log.SendException(ex);
			}
		}
		protected override void OnLeave(MvvmConstructorContext context) {
			if(Entities == null)
				return;
			this.Context.SelectedTables = Entities.Where(en => en.Checked).Select<EntitySetInfoViewModel, IEntitySetInfo>(arg => arg.Info).ToArray();
			if(!this.CalcIsCompletedCore())
				return;
			if(this.GetParentViewModel<IMvvmConstructorViewModel>().StepDirection == StepByStepDirection.Next) {
				ITemplatesCodeGen codeGen = this.ServiceContainer.Resolve<ITemplatesCodeGen>();
				var templateContext = new TemplateGenerationContext(context);
				if(entry == ConstructorEntry.DataModel) {
					codeGen.GenerateDocumentsView(templateContext, this.Context.SelectedDataModel, this.Context.SelectedViewModelType, true);
				} else {
					templateContext.DbContainer = this.Context.DataSource;
					codeGen.GenerateDocumentsView(templateContext, this.Context.DataSource, this.Context.SelectedViewModelType, true);
				}
			}
		}
		string GetDisclaimerTextWPF() {
			switch(this.Context.SelectedUIType) {
				case UIType.WindowsUI:
					return SR_Mvvm.HybridUIWizard_DisclaimerText;
				case UIType.OutlookInspired:
					return SR_Mvvm.OutlookInspiredWizard_DisclaimerText;
				case UIType.Standard:
					return SR_Mvvm.TabbedMDIWizard_DisclaimerText;
				case UIType.Browser:
					return SR_Mvvm.BrowserUIWizard_DisclaimerText;
				default:
					Debug.Fail("unknown UIType");
					return SR_Mvvm.TabbedMDIWizard_DisclaimerText;
			}
		}
		string GetDisclaimerTextWinForms() {
			switch(this.Context.SelectedUIType) {
				case UIType.WindowsUI:
					return SR_Mvvm.HybridUIWizard_DisclaimerText;
				case UIType.OutlookInspired:
					return SR_Mvvm.OutlookInspiredWizard_DisclaimerText;
				case UIType.Standard:
					return SR_Mvvm.TabbedMDIWizard_DisclaimerText;
				case UIType.Browser:
					return SR_Mvvm.BrowserUIWizard_DisclaimerText;
				default:
					Debug.Fail("unknown UIType");
					return SR_Mvvm.TabbedMDIWizard_DisclaimerText;
			}
		}
		public override string DisclaimerText {
			get {
				switch(Context.PlatformType) {
					case PlatformType.WinForms:
						return GetDisclaimerTextWPF();
					case PlatformType.WPF:
						return GetDisclaimerTextWinForms();
				}
				return string.Empty;
			}
		}
		public override string SelectTablesText {
			get { return SR_Mvvm.TabbedMDIWizard_SelectTablesText; }
		}
		public override string StepDescription {
			get { return SR_Mvvm.TabbedMDIWizard_SelectTablesPageDescription; }
		}
	}
}
