#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.SystemModule {
	[XafDisplayName("Copy Model Differences")]
	[DomainComponent]
	public class ModelDifferenceCopyParameters : IDisposable {
		private IObjectSpace objectSpace;
		private Type modelDifferenceType;
		private ModelDifferenceCopyBehavior copyBehavior;
		private IList sourceModelDifferences;
		private IModelDifference sourceModelDifference;
		private IList targetModelDifferences;
		private void ApplyCriteriaToTargetModelDifferences() {
			BinaryOperator criteria = new BinaryOperator(
				objectSpace.TypesInfo.FindTypeInfo(modelDifferenceType).KeyMember.Name,
				objectSpace.GetKeyValue(sourceModelDifference),
				BinaryOperatorType.NotEqual);
			objectSpace.ApplyCriteria(targetModelDifferences, criteria);
		}
		public ModelDifferenceCopyParameters(IObjectSpace objectSpace, Type modelDifferenceType)
			: base() {
			this.objectSpace = objectSpace;
			this.modelDifferenceType = modelDifferenceType;
			copyBehavior = ModelDifferenceCopyBehavior.Overwrite;
			sourceModelDifferences = objectSpace.GetObjects(modelDifferenceType);
			targetModelDifferences = objectSpace.GetObjects(modelDifferenceType);
		}
		[Browsable(false)]
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public ModelDifferenceCopyBehavior CopyBehavior {
			get { return copyBehavior; }
			set { copyBehavior = value; }
		}
		[Browsable(false)]
		[ElementTypeProperty("ModelDifferenceType")]
		public IList SourceModelDifferences {
			get { return sourceModelDifferences; }
		}
		[ImmediatePostData]
		[DataSourceProperty("SourceModelDifferences")]
		[DataTypeProperty("ModelDifferenceType")]
		[EditorAlias(EditorAliases.LookupPropertyEditor)]
		public IModelDifference SourceModelDifference {
			get { return sourceModelDifference; }
			set {
				sourceModelDifference = value;
				ApplyCriteriaToTargetModelDifferences();
			}
		}
		[ElementTypeProperty("ModelDifferenceType")]
		[EditorAlias(EditorAliases.ListPropertyEditor)]
		public IList TargetModelDifferences {
			get { return targetModelDifferences; }
		}
		[Browsable(false)]
		public Type ModelDifferenceType {
			get { return modelDifferenceType; }
		}
		public void Dispose() {
			if(sourceModelDifferences is IDisposable) {
				((IDisposable)sourceModelDifferences).Dispose();
			}
			sourceModelDifferences = null;
			sourceModelDifference = null;
			if(targetModelDifferences is IDisposable) {
				((IDisposable)targetModelDifferences).Dispose();
			}
			targetModelDifferences = null;
			objectSpace = null;
		}
	}
	public class ModelDifferenceViewController : ViewController {
		public static String ExportedModelDifferencesFolderName = "ExportedModelDifferences";
		private SimpleAction createModelDifferencesAction;
		private PopupWindowShowAction copyModelDifferenceAction;
		private SimpleAction exportModelDifferencesAction;
		private SimpleAction resetModelDifferencesAction;
		private SimpleAction importSharedModelDifferenceAction;
		private String exportedModelDifferencesPath;
		private void ExportModelDifferencesAction_CustomGetTotalTooltip(Object sender, CustomGetTotalTooltipEventArgs e) {
			e.Tooltip = String.Format(e.Tooltip, exportedModelDifferencesPath);
		}
		private void ImportSharedModelDifferenceAction_CustomGetTotalTooltip(Object sender, CustomGetTotalTooltipEventArgs e) {
			String fileName = Application.GetDiffDefaultName(PathHelper.GetApplicationFolder()) + ModelStoreBase.ModelFileExtension;
			e.Tooltip = String.Format(e.Tooltip, fileName);
		}
		private void ImportSharedModelDifferenceAction_CustomGetFormattedConfirmationMessage(Object sender, CustomGetFormattedConfirmationMessageEventArgs e) {
			String fileName = Application.GetDiffDefaultName(PathHelper.GetApplicationFolder()) + ModelStoreBase.ModelFileExtension;
			e.ConfirmationMessage = String.Format(e.ConfirmationMessage, fileName);
		}
		private void CreateModelDifferencesAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			Application.SaveModelChanges();
			CreateModelDifferences();
			if(View is ListView) {
				((ListView)View).CollectionSource.Reload();
				View.Refresh();
			}
		}
		private void CopyModelDifferenceAction_CustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs e) {
			Application.SaveModelChanges();
			ModelDifferenceCopyParameters modelDifferenceCopyParameters = new ModelDifferenceCopyParameters(ObjectSpace, ModelDifferenceType);
			modelDifferenceCopyParameters.SourceModelDifference = (IModelDifference)View.CurrentObject;
			DetailView detailView = Application.CreateDetailView(ObjectSpace, modelDifferenceCopyParameters, false);
			ListPropertyEditor listPropertyEditor = (ListPropertyEditor)detailView.FindItem("TargetModelDifferences");
			((IFrameContainer)listPropertyEditor).InitializeFrame();
			listPropertyEditor.ListView.AllowNew[""] = false;
			listPropertyEditor.ListView.AllowEdit[""] = false;
			listPropertyEditor.ListView.AllowDelete[""] = false;
			e.View = detailView;
			e.DialogController.SaveOnAccept = false;
		}
		private void CopyModelDifferenceAction_Execute(Object sender, PopupWindowShowActionExecuteEventArgs e) {
			ModelDifferenceCopyParameters copyParameters = (ModelDifferenceCopyParameters)e.PopupWindowViewCurrentObject;
			IList targetModelDifferences = ((ListPropertyEditor)((DetailView)e.PopupWindowView).FindItem("TargetModelDifferences")).ListView.SelectedObjects;
			CopyModelDifference(copyParameters.CopyBehavior, copyParameters.SourceModelDifference, targetModelDifferences);
			copyParameters.Dispose();
			if(View is ListView) {
				((ListView)View).CollectionSource.Reload();
			}
		}
		private void ExportModelDifferencesAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			Application.SaveModelChanges();
			ExportModelDifferences(View.SelectedObjects);
		}
		private void ImportSharedModelDifferencesAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			ImportSharedModelDifference();
		}
		private void ResetModelDifferencesAction_Execute(Object sender, SimpleActionExecuteEventArgs e) {
			ResetModelDifferences(View.SelectedObjects);
		}
		private void ObjectSpace_Committing(Object sender, CancelEventArgs e) {
			foreach(Object obj in ((IObjectSpace)sender).ModifiedObjects) {
				if(obj is IModelDifference) {
					((IModelDifference)obj).Version = ((IModelDifference)obj).Version + 1;
				}
				if((obj is IModelDifferenceAspect) && (((IModelDifferenceAspect)obj).Owner != null)) {
					((IModelDifferenceAspect)obj).Owner.Version = ((IModelDifferenceAspect)obj).Owner.Version + 1;
				}
			}
		}
		protected virtual Type ModelDifferenceType {
			get { return ((ObjectView)View).ObjectTypeInfo.Type; }
		}
		protected virtual void UpdateActionState() {
			createModelDifferencesAction.Active["UserTypeInfo"] = (ModelDifferenceDbStore.UserTypeInfo != null);
			copyModelDifferenceAction.Active["UserTypeInfo"] = (ModelDifferenceDbStore.UserTypeInfo != null);
			exportModelDifferencesAction.Active["UserTypeInfo"] = (ModelDifferenceDbStore.UserTypeInfo != null);
			resetModelDifferencesAction.Active["UserTypeInfo"] = (ModelDifferenceDbStore.UserTypeInfo != null);
			exportModelDifferencesAction.Active["UserTypeInfo"] = (ModelDifferenceDbStore.UserTypeInfo != null);
		}
		protected virtual IList<String> GetContextIds(IObjectSpace objectSpace) {
			HashSet<String> contextIds = new HashSet<String>();
			IList modelDifferences = objectSpace.GetObjects(ModelDifferenceType);
			foreach(IModelDifference modelDifference in modelDifferences) {
				contextIds.Add(modelDifference.ContextId);
			}
			if(contextIds.Count == 0) {
				contextIds.Add("");
			}
			return contextIds.ToList();
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.Committing += ObjectSpace_Committing;
			UpdateActionState();
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			ObjectSpace.Committing -= ObjectSpace_Committing;
		}
		protected virtual void CreateModelDifferences() {
			using(IObjectSpace objectSpace = Application.CreateObjectSpace(ModelDifferenceType)) {
				foreach(String contextId in GetContextIds(objectSpace)) {
					foreach(Object user in objectSpace.GetObjects(ModelDifferenceDbStore.UserTypeInfo.Type)) {
						String userId = ModelDifferenceDbStore.UserIdTypeConverter.ConvertToInvariantString(objectSpace.GetKeyValue(user));
						IModelDifference modelDifference = ModelDifferenceDbStore.FindModelDifference(objectSpace, ModelDifferenceType, userId, contextId);
						if(modelDifference == null) {
							modelDifference = (IModelDifference)objectSpace.CreateObject(ModelDifferenceType);
							modelDifference.UserId = userId;
							modelDifference.ContextId = contextId;
						}
						for(Int32 i = 0; i < ((ModelApplicationBase)Application.Model).AspectCount; i++) {
							String aspectName = ((ModelApplicationBase)Application.Model).GetAspect(i);
							IModelDifferenceAspect modelDifferenceAspect = ModelDifferenceDbStore.FindModelDifferenceAspect(modelDifference, aspectName);
							if(modelDifferenceAspect == null) {
								modelDifferenceAspect = (IModelDifferenceAspect)objectSpace.CreateObject(
									ModelDifferenceDbStore.GetModelDifferenceAspectType(Application.TypesInfo, ModelDifferenceType));
								modelDifferenceAspect.Name = aspectName;
								modelDifferenceAspect.Xml = ModelDifferenceDbStore.EmptyXafml;
								modelDifferenceAspect.Owner = modelDifference;
							}
						}
					}
				}
				objectSpace.CommitChanges();
			}
		}
		protected virtual void CopyModelDifference(ModelDifferenceCopyBehavior copyBehavior, IModelDifference sourceModelDifference, IList targetModelDifferences) {
			using(IObjectSpace objectSpace = Application.CreateObjectSpace(ModelDifferenceType)) {
				foreach(IModelDifference targetModelDifference_Original in targetModelDifferences) {
					IModelDifference targetModelDifference = objectSpace.GetObject(targetModelDifference_Original);
					targetModelDifference.Version = targetModelDifference.Version + 1;
					foreach(IModelDifferenceAspect sourceModelDifferenceAspect in sourceModelDifference.Aspects) {
						IModelDifferenceAspect targetModelDifferenceAspect =
							ModelDifferenceDbStore.FindModelDifferenceAspect(targetModelDifference, sourceModelDifferenceAspect.Name);
						if(targetModelDifferenceAspect == null) {
							targetModelDifferenceAspect = (IModelDifferenceAspect)objectSpace.CreateObject(
								ModelDifferenceDbStore.GetModelDifferenceAspectType(Application.TypesInfo, ModelDifferenceType));
							targetModelDifferenceAspect.Name = sourceModelDifferenceAspect.Name;
							targetModelDifferenceAspect.Owner = targetModelDifference;
						}
						if((copyBehavior == ModelDifferenceCopyBehavior.Merge) && !String.IsNullOrWhiteSpace(targetModelDifferenceAspect.Xml)) {
							ModelXmlCombiner combiner = new ModelXmlCombiner();
							targetModelDifferenceAspect.Xml = combiner.CombineXmlStrings(targetModelDifferenceAspect.Xml, sourceModelDifferenceAspect.Xml, Application.Model);
						}
						else {
							targetModelDifferenceAspect.Xml = sourceModelDifferenceAspect.Xml;
						}
					}
				}
				objectSpace.CommitChanges();
			}
		}
		protected virtual void ExportModelDifferences(IList modelDifferences) {
			foreach(IModelDifference modelDifference in modelDifferences) {
				String path = exportedModelDifferencesPath;
				String fileNameTemplate = Application.GetDiffDefaultName(path);
				if(!String.IsNullOrWhiteSpace(modelDifference.UserId)) {
					path = Path.Combine(path, modelDifference.UserName);
					fileNameTemplate = ModelStoreBase.UserDiffDefaultName;
				}
				if(!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
				foreach(IModelDifferenceAspect aspect in modelDifference.Aspects) {
					String fileName = 
						fileNameTemplate 
						+ (String.IsNullOrEmpty(aspect.Name) ? "" : "_" + aspect.Name)
						+ ModelStoreBase.ModelFileExtension;
					File.WriteAllText(Path.Combine(path, fileName), aspect.Xml);
				}
			}
		}
		protected virtual void ImportSharedModelDifference() {
			using(IObjectSpace objectSpace = Application.CreateObjectSpace(ModelDifferenceType)) {
				IModelDifference modelDifference =
					(IModelDifference)objectSpace.FindObject(ModelDifferenceType, new BinaryOperator(ModelDifferenceDbStore.UserIdPropertyName, ""));
				if(modelDifference == null) {
					modelDifference = (IModelDifference)objectSpace.CreateObject(ModelDifferenceType);
					modelDifference.UserId = "";
				}
				else {
					modelDifference.Version = modelDifference.Version + 1;
				}
				ModelDifferenceDbStore.ImportSharedModelDifference(Application, objectSpace, modelDifference);
				objectSpace.CommitChanges();
			}
		}
		protected virtual void ResetModelDifferences(IList modelDifferences) {
			using(IObjectSpace objectSpace = Application.CreateObjectSpace(ModelDifferenceType)) {
				foreach(IModelDifference modelDifference_Original in modelDifferences) {
					IModelDifference modelDifference = objectSpace.GetObject(modelDifference_Original);
					modelDifference.Version = modelDifference.Version + 1;
					foreach(IModelDifferenceAspect modelDifferenceAspect in modelDifference.Aspects) {
						modelDifferenceAspect.Xml = ModelDifferenceDbStore.EmptyXafml;
					}
				}
				objectSpace.CommitChanges();
			}
		}
		public ModelDifferenceViewController()
			: base() {
			TypeOfView = typeof(ObjectView);
			TargetViewNesting = Nesting.Any;
			TargetObjectType = typeof(IModelDifference);
			exportedModelDifferencesPath = Path.Combine(PathHelper.GetApplicationFolder(), ExportedModelDifferencesFolderName);
			createModelDifferencesAction = new SimpleAction(this, "CreateModelDifferences", PredefinedCategory.Tools);
			createModelDifferencesAction.SelectionDependencyType = SelectionDependencyType.Independent;
			createModelDifferencesAction.ImageName = "Action_ModelDifferences_Create";
			createModelDifferencesAction.ToolTip = "Initializes settings (model differences) for all users.";
			createModelDifferencesAction.Execute += CreateModelDifferencesAction_Execute;
			copyModelDifferenceAction = new PopupWindowShowAction(this, "CopyModelDifference", PredefinedCategory.Tools);
			copyModelDifferenceAction.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
			copyModelDifferenceAction.ImageName = "Action_ModelDifferences_Copy";
			copyModelDifferenceAction.ToolTip = "Copies settings (model differences) from one user to another. Select source settings (model differences), click this button and choose targets in the invoked popup. Changes are activated after a user restarts the application.";
			copyModelDifferenceAction.CustomizePopupWindowParams += CopyModelDifferenceAction_CustomizePopupWindowParams;
			copyModelDifferenceAction.Execute += CopyModelDifferenceAction_Execute;
			exportModelDifferencesAction = new SimpleAction(this, "ExportModelDifferences", PredefinedCategory.Tools);
			exportModelDifferencesAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			exportModelDifferencesAction.ImageName = "Action_ModelDifferences_Export";
			exportModelDifferencesAction.ToolTip = "Exports selected settings (model differences) to the {0} folder.";
			exportModelDifferencesAction.CustomGetTotalTooltip += ExportModelDifferencesAction_CustomGetTotalTooltip;
			exportModelDifferencesAction.Execute += ExportModelDifferencesAction_Execute;
			importSharedModelDifferenceAction = new SimpleAction(this, "ImportSharedModelDifference", PredefinedCategory.Tools);
			importSharedModelDifferenceAction.SelectionDependencyType = SelectionDependencyType.Independent;
			importSharedModelDifferenceAction.ImageName = "Action_ModelDifferences_Import";
			importSharedModelDifferenceAction.ToolTip = "Imports shared settings (model differences) from the {0} file located in the application folder.";
			importSharedModelDifferenceAction.ConfirmationMessage = "You are about to overwrite shared settings (model differences) with the {0} file content. This cannot be undone. Do you want to proceed?";
			importSharedModelDifferenceAction.CustomGetTotalTooltip += ImportSharedModelDifferenceAction_CustomGetTotalTooltip;
			importSharedModelDifferenceAction.CustomGetFormattedConfirmationMessage += ImportSharedModelDifferenceAction_CustomGetFormattedConfirmationMessage;
			importSharedModelDifferenceAction.Execute += ImportSharedModelDifferencesAction_Execute;
			resetModelDifferencesAction = new SimpleAction(this, "ResetModelDifferences", PredefinedCategory.Tools);
			resetModelDifferencesAction.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
			resetModelDifferencesAction.ImageName = "Action_ModelDifferences_Reset";
			resetModelDifferencesAction.ToolTip = "Resets selected settings (model differences). Changes are applied after a user restarts the application.";
			resetModelDifferencesAction.ConfirmationMessage = "You are about to reset the selected settings (model differences). Do you want to proceed?";
			resetModelDifferencesAction.Execute += ResetModelDifferencesAction_Execute;
		}
		public SimpleAction CreateModelDifferencesAction {
			get { return createModelDifferencesAction; }
		}
		public PopupWindowShowAction CopyModelDifferenceAction {
			get { return copyModelDifferenceAction; }
		}
		public SimpleAction ExportModelDifferencesAction {
			get { return exportModelDifferencesAction; }
		}
		public SimpleAction ImportSharedModelDifferenceAction {
			get { return importSharedModelDifferenceAction; }
		}
		public SimpleAction ResetModelDifferencesAction {
			get { return resetModelDifferencesAction; }
		}
		public String ExportedModelDifferencesPath {
			get { return exportedModelDifferencesPath; }
			set {
				exportedModelDifferencesPath = value;
				exportModelDifferencesAction.RaiseChanged(ActionChangedType.ToolTip);
			}
		}
	}
}
