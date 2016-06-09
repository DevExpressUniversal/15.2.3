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
using System.Windows.Input;
using System.Collections.Generic;
using DevExpress.Design.UI;
using DevExpress.Design.Mvvm.EntityFramework;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using System.Windows.Documents;
using DevExpress.Entity.Model;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	class TablesSelectorViewModel : MvvmConstructorPageViewModelBase {
		private EntitySetInfoViewModel selectedInfo;
		IEnumerable<EntitySetInfoViewModel> entities;
		protected bool itemStatesUpdating;
		public TablesSelectorViewModel(IViewModelBase parentViewModel, MvvmConstructorContext context)
			: base(parentViewModel, context) {
			ShowErrorDetails = new WpfDelegateCommand(() => {
				MessageLogWindow logWindow = new MessageLogWindow(this.ServiceContainer);
				if(this.WizardTaskManager.MainWindow != null)
					logWindow.Owner = WizardTaskManager.MainWindow;
				logWindow.ShowDialog();
			});
		}
		IWizardTaskManager WizardTaskManager { get { return ServiceContainer.Resolve<IWizardTaskManager>(); } }
		protected IEntityFrameworkModel EntityFrameworkModel { get { return ServiceContainer.Resolve<IEntityFrameworkModel>(); } }
		protected virtual void BuildEntities() {
			var type = Context.DbContextCandidate.ResolveType();
			if(!type.IsPublic) {
				this.ModelHasErrors = true;
				Log.Send("The type {0} must be public.", true, type.FullName);
				return;
			}
			IDbContainerInfo containerInfo = EntityFrameworkModel.GetContainer(this.Context.DbContextCandidate);
			ValidateContainerInfo(containerInfo);
			if(this.ModelHasErrors)
				return;
			this.Context.DataSource = containerInfo;
			Entities = BuildEntities(containerInfo);
			BeginItemStatesUpdating();
			try {
				if(this.Context.SelectedTables != null && this.Context.SelectedTables.Length > 0) {
					foreach(IEntitySetInfo info in this.Context.SelectedTables)
						foreach(EntitySetInfoViewModel entity in Entities)
							if(entity.Info == info) {
								entity.Checked = true;
								break;
							}
				}
			}
			finally {
				EndItemStatesUpdating();
				UpdateSelectAllState();
			}
			if(Entities.Any())
				SelectedInfo = Entities.First();
		}
		protected void ValidateContainerInfo(IDbContainerInfo containerInfo) {
			if(containerInfo == null)
				this.ModelHasErrors = true;
			else if(!containerInfo.EntitySets.Any()) {
				Log.SendWarning("Entity Framework Model \"" + containerInfo.Name + "\" contains no DbSet.", true);
				this.ModelHasErrors = true;
			}
		}
		EntitySetInfoViewModel[] BuildEntities(IDbContainerInfo dbContex) {
			if(dbContex == null)
				return new EntitySetInfoViewModel[0];
			return BuildEntities(dbContex.EntitySets);
		}
		protected EntitySetInfoViewModel[] BuildEntities(IEnumerable<IEntitySetInfo> entitySets) {
			List<EntitySetInfoViewModel> result = new List<EntitySetInfoViewModel>();
			foreach(IEntitySetInfo item in entitySets)
				result.Add(new EntitySetInfoViewModel(item, this));
			return result.ToArray();
		}
		public IEnumerable<EntitySetInfoViewModel> Entities {
			get { return entities; }
			set { SetProperty(ref entities, value, "Entities"); }
		}
		public EntitySetInfoViewModel SelectedInfo {
			get {
				return selectedInfo;
			}
			set {
				SetProperty(ref selectedInfo, value, "SelectedInfo");
			}
		}
		protected override void OnEnter(MvvmConstructorContext context) {
			BuildEntities();
		}
		protected override void OnLeave(MvvmConstructorContext context) {
			this.modelHasErrors = false;
			if(Entities == null)
				return;
			this.Context.SelectedTables = Entities.Where(en => en.Checked).Select(arg => arg.Info).ToArray();
			if(this.GetParentViewModel<IMvvmConstructorViewModel>().StepDirection == StepByStepDirection.Next) {
				ITemplatesCodeGen codeGen = this.ServiceContainer.Resolve<ITemplatesCodeGen>();
				var templateContext = new TemplateGenerationContext(context);
				this.Context.SelectedDataModel = codeGen.GenerateEntityDataModel(templateContext, this.Context.DataSource, true);
			}
		}
		protected override bool CalcIsCompletedCore() {
			return Entities != null && Entities.Any(en => en.Checked);
		}
		public override string StepDescription {
			get { return SR_Mvvm.SelectingEntitiesPageDescription; }
		}
		public virtual string DisclaimerText {
			get {
				return SR_Mvvm.DataAccessLayerWizard_DisclaimerText;
			}
		}
		public virtual string SelectTablesText {
			get { return SR_Mvvm.DataAccessLayerWizard_SelectTables; }
		}
		bool modelHasErrors;
		public bool ModelHasErrors {
			get { return modelHasErrors; }
			set { SetProperty(ref modelHasErrors, value, "ModelHasErrors"); }
		}
		public ICommand ShowErrorDetails { get; private set; }
		public string EmptyModelText {
			get { return SR_Mvvm.EmptyEntityFrameworkModelText; }
		}
		bool? selectAllState;
		public bool? SelectAllState {
			get { return selectAllState; }
			set {
				SetProperty(ref selectAllState, value, () => SelectAllState, () => OnSelectAllChanged());
			}
		}
		void OnSelectAllChanged() {
			if(SelectAllState == null || !SelectAllState.HasValue || IsItemStatesUpdating)
				return;
			bool state = SelectAllState.Value;
			BeginItemStatesUpdating();
			try {
				foreach(EntitySetInfoViewModel item in Entities)
					item.Checked = state;
			}
			finally {
				EndItemStatesUpdating();
			}
		}
		protected bool IsItemStatesUpdating { get { return itemStatesUpdating; } }
		protected void BeginItemStatesUpdating() {
			itemStatesUpdating = true;
		}
		protected void EndItemStatesUpdating() {
			itemStatesUpdating = false;
		}
		public void UpdateSelectAllState() {
			if(IsItemStatesUpdating || Entities == null)
				return;
			BeginItemStatesUpdating();
			try {
				bool? result = null;
				foreach(EntitySetInfoViewModel item in Entities) {
					if(!result.HasValue) {
						result = item.Checked;
						continue;
					}
					if(item.Checked && result.Value)
						continue;
					if(!item.Checked && !result.Value)
						continue;
					result = null;
					break;
				}
				SelectAllState = result;
			}
			finally {
				EndItemStatesUpdating();
			}
		}
	}
	class EntitySetInfoViewModel : WpfBindableBase {
		readonly TablesSelectorViewModel parentViewModel;
		bool readOnly;
		IEntitySetInfo info;
		bool @checked;
		public EntitySetInfoViewModel(IEntitySetInfo info, TablesSelectorViewModel parentViewModel) {
			this.parentViewModel = parentViewModel;
			this.info = info;
			Checked = true;
			ReadOnly = info.ShouldGenerateReadOnlyView();
		}
		public IEntitySetInfo Info {
			get {
				return info;
			}
			set { SetProperty<IEntitySetInfo>(ref info, value, "Info"); }
		}
		public bool Checked {
			get {
				return @checked;
			}
			set {
				if(SetProperty<bool>(ref @checked, value, "Checked")) {
					RaisePropertyChanged("CanChangeReadOnly");
					parentViewModel.UpdateSelectAllState();
				}
			}
		}
		public bool CanChangeReadOnly {
			get {
				return !(info.IsView || info.ReadOnly) && Checked;
			}
		}
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				if(SetProperty<bool>(ref readOnly, value, "ReadOnly"))
					info.AttachedInfo.UserSelectedIsReadOnly = readOnly;
			}
		}
		public IEnumerable<IEdmPropertyInfo> TableKeys {
			get {
				if(info.ElementType == null)
					yield break;
				foreach(IEdmPropertyInfo keyMember in info.ElementType.KeyMembers) {
					IEdmPropertyInfo result = keyMember as IEdmPropertyInfo;
					if(result != null)
						yield return result;
				}
				foreach(IEdmPropertyInfo property in info.ElementType.GetProperties()) {
					if(!info.ElementType.KeyMembers.Any(km => km.Name == property.Name))
						yield return property;
				}
			}
		}
	}
}
