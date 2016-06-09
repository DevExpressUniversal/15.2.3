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
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Entity.Model;
using DevExpress.Xpf.Core.Design.Wizards.Mvvm.DataModel;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class ViewModelEntityDefinitionsViewModel : MvvmConstructorPageViewModelBase {		
		string viewModelName;
		EntityRepositoryInfo selectedEntity;
		IEnumerable<EntityRepositoryInfo> items;
		IEnumerable<IEdmPropertyInfo> tableKeys;
		public ViewModelEntityDefinitionsViewModel(IViewModelBase parentViewModel, MvvmConstructorContext context)
			: base(parentViewModel, context) {
			IsKeyCorrectionNeeded = true;
		}
		IViewModelLayerService ViewModelLayerService { get { return ServiceContainer.Resolve<IViewModelLayerService>(); } }
		void BuildItems() {
			if(Context.SelectedDataModel == null)
				return;
			bool canBeReadOnly = Context.SelectedViewModelType == ViewModelType.EntityRepository;
			Items = Context.SelectedDataModel.EntityRepositories.Where(ri => !ri.IsReadOnly || canBeReadOnly).ToArray();
		}
		void InitSelectedEntityInfo() {
			if(Items == null)
				return;
			if(Context.SelectedEntity == null)
				this.SelectedEntity = Items.FirstOrDefault();
			else
				this.SelectedEntity = Items.FirstOrDefault(et => et.EntitySet == Context.SelectedEntity);
		}
		void InitViewModelName() {
			ViewModelName = this.Context.SelectedViewModelName;
		}
		void UpdateViewModelName() {
			ViewModelName = GetViewModelName();
		}
		string GetViewModelName() {
			return ViewModelLayerService.GetViewModelName(GetViewModelFolderName(), this.Context.SelectedViewModelType);
		}
		string GetViewModelFolderName() {
			return SelectedEntity == null ? string.Empty : SelectedEntity.EntitySet.ElementType.Type.Name;
		}
		protected override void OnEnter(MvvmConstructorContext context) {
			InitViewModelName();
			BuildItems();
			InitSelectedEntityInfo();
		}
		protected override void OnLeave(MvvmConstructorContext context) {
			try {
				Context.SelectedEntity = this.SelectedEntity.EntitySet;
				Context.SelectedViewModelName = ViewModelName;
				if(this.GetParentViewModel<IMvvmConstructorViewModel>().StepDirection == StepByStepDirection.Next) {
					ITemplatesCodeGen codeGen = this.ServiceContainer.Resolve<ITemplatesCodeGen>();
					var templateContext = new TemplateGenerationContext(context);
					this.Context.SelectedViewModel = codeGen.GenerateViewModel(templateContext, this.Context.SelectedViewModelType, this.Context.SelectedViewModelName, GetViewModelFolderName(), this.Context.SelectedDataModel, this.Context.SelectedEntity, true);
				}
			} catch(Exception ex) {
				Log.SendException(ex, false);
			}
		}
		protected override bool CalcIsCompletedCore() {
			bool isViewModelNameValid = !string.IsNullOrEmpty(ViewModelName);
			if(Context.SelectedViewModelType == ViewModelType.Simple)
				return isViewModelNameValid;
			return isViewModelNameValid && SelectedEntity != null && SelectedDataModel != null;
		}
		public IEnumerable<EntityRepositoryInfo> Items {
			get { return items; }
			set { SetProperty<IEnumerable<EntityRepositoryInfo>>(ref items, value, "Items"); }
		}
		public IDataModel SelectedDataModel {
			get {
				return Context.SelectedDataModel;
			}
		}
		public string ViewModelName {
			get {
				return viewModelName;
			}
			set {
				if(SetProperty<string>(ref viewModelName, value, "ViewModelName"))
					UpdateIsCompleted();
			}
		}
		public EntityRepositoryInfo SelectedEntity {
			get {
				return selectedEntity;
			}
			set {
				if(SetProperty<EntityRepositoryInfo>(ref selectedEntity, value, "SelectedEntity")) {
					this.RaisePropertyChanged("ShowKeyCorrectionOptions");
					UpdateIsCompleted();
					UpdateViewModelName();
					IsEntitySelected = SelectedEntity != null;
					UpdateTableKeys();
				}
			}
		}
		public string DescriptionText {
			get {
				switch(Context.SelectedViewModelType) {
					case ViewModelType.Entity:
						return SR_Mvvm.ViewModelWizard_EntityViewModelDescriptionText;
					case ViewModelType.EntityRepository:
						return SR_Mvvm.ViewModelWizard_EntityRepositoryViewModelDescriptionText;
				}
				return string.Empty;
			}
		}
		bool isEntitySelected;
		public bool IsEntitySelected {
			get {
				return isEntitySelected;
			}
			set {
				SetProperty<bool>(ref isEntitySelected, value, "IsEntitySelected");
			}
		}
		public override string StepDescription {
			get { return SR_Mvvm.ViewModelProperties; }
		}
		bool HasValidType(IEdmPropertyInfo info) {
			if(info == null || SelectedEntity == null || info.PropertyType == null)
				return false;
			return SelectedEntity.PrimaryKeyType.FullName == info.GetUnderlyingClrType().FullName;
		}
		void UpdateTableKeys() {
			if(!ShowKeyCorrectionOptions)
				return;
			List<IEdmPropertyInfo> list = new List<IEdmPropertyInfo>();
			if(SelectedEntity != null) {
				foreach(IEdmPropertyInfo keyMember in SelectedEntity.EntitySet.ElementType.KeyMembers) {
					IEdmPropertyInfo result = keyMember as IEdmPropertyInfo;
					if(HasValidType(result))
						list.Add(result);
				}
				foreach(IEdmPropertyInfo property in SelectedEntity.EntitySet.ElementType.GetProperties())
					if(!SelectedEntity.EntitySet.ElementType.KeyMembers.Any(km => km.Name == property.Name) && HasValidType(property))
						list.Add(property);				
			}
			TableKeys = list;
		}
		public IEnumerable<IEdmPropertyInfo> TableKeys {
			get {
				return tableKeys;
			}
			set {
				SetProperty<IEnumerable<IEdmPropertyInfo>>(ref tableKeys, value, "TableKeys");
			}
		}
		public bool IsKeyCorrectionNeeded { get; set; }
		public bool ShowKeyCorrectionOptions {
			get {
				return IsKeyCorrectionNeeded && SelectedEntity != null && !SelectedEntity.IsReadOnly;
			}
		}
	}
}
