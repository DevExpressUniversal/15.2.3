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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public class ObjectMethodActionsViewController : ViewController {
		private static bool enabled = true;
		[DefaultValue(true)]
		public static bool Enabled { get { return enabled; } set { enabled = value; } }
		private static LightDictionary<MethodInfo, ActionAttribute> methods; 
		private void action_Executing(Object sender, CancelEventArgs e) {
			if(sender is IObjectSpaceLink) {
				((IObjectSpaceLink)sender).ObjectSpace = ObjectSpace;
			}
		}
		private void action_Executed(Object sender, ActionBaseEventArgs e) {
			OnActionExecuted(sender);
		}
		protected void OnActionExecuted(object sender) {
			if(((IMethodAction)sender).Attribute.AutoCommit && NeedCommitChanges()) {
				View.ObjectSpace.CommitChanges();
			}
		}
		protected virtual bool NeedCommitChanges() {
			return View is ListView;
		}
		protected void AddAction(ActionBase action) {
			Actions.Add(action);
			action.Executing += action_Executing;
			action.Executed += action_Executed;
		}
		protected virtual void SetActions(ObjectMethodActionsViewController destController) {
			destController.Actions.Clear();
			foreach(ActionBase action in Actions) {
				destController.AddAction(MethodActionHelper.CreateMethodAction(action));
			}
		}
		protected virtual void CreateActionsByMethods(LightDictionary<MethodInfo, ActionAttribute> methods) {
			Actions.Clear();
			foreach(MethodInfo methodInfo in methods.GetKeys()) {
				AddAction(MethodActionHelper.CreateMethodAction(methodInfo, methods[methodInfo]));
			}
		}
		protected virtual LightDictionary<MethodInfo, ActionAttribute> CollectMethods(IEnumerable<Type> boTypes) {
			methods = new LightDictionary<MethodInfo, ActionAttribute>();
			foreach(Type type in boTypes) {
				foreach(MethodInfo methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
					if(methodInfo.DeclaringType != type)
						continue;
					object[] actionAttributes = methodInfo.GetCustomAttributes(typeof(ActionAttribute), true);
					if(actionAttributes.Length > 0) {
						if(methodInfo.ContainsGenericParameters) {
							throw new InvalidOperationException(
								SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ActionMethodShouldBeNonGeneric,
								methodInfo.Name, type.FullName));
						}
						else if(!IsFromBaseType(methodInfo) && !methods.ContainsKey(methodInfo)) {
							methods.Add(methodInfo, (ActionAttribute)actionAttributes[0]);
						}
					}
				}
			}
			return methods;
		}
		internal void CreateActions(IEnumerable<Type> boTypes) {
			if(Enabled) {
				CreateActionsByMethods(CollectMethods(boTypes));
			}
		}
		internal static void Reset() {
			methods = null;
		}
		private bool IsFromBaseType(MethodInfo methodInfo) {
			if(methodInfo.DeclaringType.BaseType != null) {
				MethodInfo baseMethodInfo = methodInfo.DeclaringType.BaseType.GetMethod(methodInfo.Name);
				if(baseMethodInfo != null) {
					if(baseMethodInfo.GetCustomAttributes(typeof(ActionAttribute), true).Length > 0) {
						return true;
					}
				}
			}
			return false;
		}
		public override Controller Clone(IModelApplication modelApplication) {
			if(methods != null) {
				CreateActionsByMethods(methods);
			}
			ObjectMethodActionsViewController result = (ObjectMethodActionsViewController)TypeHelper.CreateInstance(GetType());
			SetActions(result);
			SetInfo(result, modelApplication);
			return result;
		}
	}
	public class ObjectMethodActionsActionsNodeUpdater : ModelNodesGeneratorUpdater<ModelActionsNodesGenerator> {
		public override void UpdateNode(DevExpress.ExpressApp.Model.Core.ModelNode node) {
			IModelActions modelActions = ((IModelActions)node);
			ObjectMethodActionsViewController controller = null;
			foreach(Controller item in ((IModelSources)node.Application).Controllers) {
				if(item is ObjectMethodActionsViewController) {
					controller = ((ObjectMethodActionsViewController)item);
				}
			}
			if(controller != null) {
				controller.CreateActions(((IModelSources)node.Application).BOModelTypes);
				foreach(ActionBase action in controller.Actions) {
					if (node.GetNode(action.Id) == null) {
						IModelAction modelAction = node.AddNode<IModelAction>(action.Id);
						ModelActionsNodesGenerator.SetAction(modelAction, action);
					}
				}
			}
		}
	}
	public class MethodActionHelper {
		public static string GetMethodActionId(MethodInfo methodInfo) {
			Guard.ArgumentNotNull(methodInfo, "methodInfo");
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if(parameters.Length > 1) {
				throw new ArgumentException(string.Format("The {0} MethodInfo has more then one parameter.", methodInfo), "methodInfo");
			}
			string result;
			if(parameters.Length == 0) {
				result = ModelNodesGeneratorSettings.GetIdPrefix(methodInfo.DeclaringType) + "." + methodInfo.Name;
			}
			else {
				result = ModelNodesGeneratorSettings.GetIdPrefix(methodInfo.DeclaringType) + "." + methodInfo.Name + "." + parameters[0].ParameterType.Name;
			}
			return result;
		}
		public static void InvokeMethod(MethodInfo methodInfo, IObjectSpace objectSpace, IEnumerable forObjects, params Object[] parameters) {
			if(methodInfo.IsStatic) {
				methodInfo.Invoke(null, parameters);
			}
			else {
				foreach(Object obj in forObjects) {
					if(obj is XafDataViewRecord) {
						methodInfo.Invoke(objectSpace.GetObject(obj), parameters);
					}
					else {
						methodInfo.Invoke(obj, parameters);
					}
				}
			}
		}
		public static void SetMethodActionInfo(ActionBase action, MethodInfo methodInfo, ActionAttribute attribute) {
			action.Caption = String.IsNullOrEmpty(attribute.Caption) ? methodInfo.Name : attribute.Caption;
			action.Category = attribute.Category;
			if(!String.IsNullOrEmpty(attribute.ConfirmationMessage)) {
				action.ConfirmationMessage = attribute.ConfirmationMessage;
			}
			if(!String.IsNullOrEmpty(attribute.TargetObjectsCriteria)) {
				action.TargetObjectsCriteria = attribute.TargetObjectsCriteria;
			}
			action.TargetObjectsCriteriaMode = attribute.TargetObjectsCriteriaMode;
			if(!String.IsNullOrEmpty(attribute.ToolTip)) {
				action.ToolTip = attribute.ToolTip;
			}
			if(!String.IsNullOrEmpty(attribute.ImageName)) {
				action.ImageName = attribute.ImageName;
			}
			if(methodInfo.IsStatic) {
				action.SelectionDependencyType = SelectionDependencyType.Independent;
			}
			else {
				if(attribute.SelectionDependencyType == MethodActionSelectionDependencyType.RequireSingleObject) {
					action.SelectionDependencyType = SelectionDependencyType.RequireSingleObject;
				}
				else {
					action.SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects;
				}
			}
			action.TargetObjectType = methodInfo.DeclaringType;
		}
		public static ActionBase CreateMethodAction(MethodInfo methodInfo, ActionAttribute attribute) {
			ActionBase action = null;
			if(methodInfo.GetParameters().Length == 0) {
				action = new MethodSimpleAction(methodInfo, attribute);
			}
			else if(methodInfo.GetParameters().Length == 1) {
				action = new MethodParameterObjectAction(methodInfo, attribute);
			}
			return action;
		}
		public static ActionBase CreateMethodAction(ActionBase action) {
			ActionBase result = null;
			if(action is MethodSimpleAction) {
				result = new MethodSimpleAction((MethodSimpleAction)action);
			}
			else if(action is MethodParameterObjectAction) {
				result = new MethodParameterObjectAction((MethodParameterObjectAction)action);
			}
			return result;
		}
	}
	public interface IMethodAction {
		MethodInfo MethodInfo { get; }
		ActionAttribute Attribute { get; }
	}
	[ToolboxItem(false)]
	public class MethodSimpleAction : SimpleAction, IMethodAction, IObjectSpaceLink {
		private MethodInfo methodInfo;
		private ActionAttribute attribute;
		private IObjectSpace objectSpace;
		private void SetMethodInfo(MethodInfo methodInfo) {
			if(methodInfo.GetParameters().Length > 0) {
				throw new InvalidOperationException(
					SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ActionMethodShouldBeParameterless,
					methodInfo.Name, methodInfo.DeclaringType.FullName));
			}
			this.methodInfo = methodInfo;
		}
		private void SubscribeToEvents(){
			Execute += new SimpleActionExecuteEventHandler(action_Execute);
		}
		private void UnsubscribeFromEvents() {
			Execute -= new SimpleActionExecuteEventHandler(action_Execute);
		}
		protected internal void ExecuteCore(IList selectedObjects) {
			MethodActionHelper.InvokeMethod(MethodInfo, objectSpace, selectedObjects);
		}
		private void action_Execute(object sender, SimpleActionExecuteEventArgs e) {
			ExecuteCore(e.SelectedObjects);
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeFromEvents();
			this.methodInfo = null;
			this.attribute = null;
			base.Dispose(disposing);
		}
		public MethodSimpleAction(MethodSimpleAction sourceAction) {
			SetMethodInfo(sourceAction.MethodInfo);
			this.attribute = sourceAction.Attribute;
			AssignFrom(sourceAction);
			Id = sourceAction.Id;
			SubscribeToEvents();
		}
		public MethodSimpleAction(MethodInfo methodInfo, ActionAttribute attribute) {
			SetMethodInfo(methodInfo);
			this.attribute = attribute;
			Id = MethodActionHelper.GetMethodActionId(methodInfo);
			MethodActionHelper.SetMethodActionInfo(this, methodInfo, attribute);
			SubscribeToEvents();
		}
		public MethodInfo MethodInfo {
			get { return this.methodInfo; }
		}
		public ActionAttribute Attribute {
			get { return this.attribute; }
		}
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
	}
	[ToolboxItem(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class MethodParameterObjectAction : PopupWindowShowAction, IMethodAction, IObjectSpaceLink {
		private MethodInfo methodInfo;
		private ActionAttribute attribute;
		private Type parameterObjectType;
		private IObjectSpace objectSpace;
		private void SetMethodInfo(MethodInfo methodInfo) {
			if(methodInfo.GetParameters().Length != 1) {
				throw new InvalidOperationException(String.Format(
					"Unable to create action for method {0} of class {1}. Action method must have one parameter.",
					methodInfo.Name, methodInfo.DeclaringType.FullName));
			}
			this.methodInfo = methodInfo;
		}
		private void SubscribeToEvents() {
			Execute += new PopupWindowShowActionExecuteEventHandler(action_Execute);
			CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(action_CustomizePopupWindowParams);
		}
		private void UnsubscribeFromEvents() {
			Execute -= new PopupWindowShowActionExecuteEventHandler(action_Execute);
			CustomizePopupWindowParams -= new CustomizePopupWindowParamsEventHandler(action_CustomizePopupWindowParams);
		}
		private Object CreateParameterObject() {
			Object result = null;
			if(ParameterObjectType.GetConstructor(new Type[] { MethodInfo.DeclaringType }) != null) {
				View view = null;
				if(Controller is ViewController) {
					view = ((ViewController)Controller).View;
				}
				else {
					view = Controller.Frame.View;
				}
				Object obj = view.CurrentObject;
				if(obj is XafDataViewRecord) {
					obj = view.ObjectSpace.GetObject(obj);
				}
				result = TypeHelper.CreateInstance(ParameterObjectType, obj);
			}
			else {
				result = TypeHelper.CreateInstance(ParameterObjectType);
			}
			return result;
		}
		private void action_CustomizePopupWindowParams(Object sender, CustomizePopupWindowParamsEventArgs e) {
			IObjectSpace objectSpace = Application.CreateObjectSpace(parameterObjectType);
			DetailView detailView = Application.CreateDetailView(objectSpace, CreateParameterObject());
			detailView.ViewEditMode = ViewEditMode.Edit;
			e.View = detailView;
		}
		private void action_Execute(Object sender, PopupWindowShowActionExecuteEventArgs e) {
			Object parameterObject = e.PopupWindow.View.CurrentObject;
			try {
				MethodActionHelper.InvokeMethod(MethodInfo, objectSpace, e.SelectedObjects, parameterObject);
			}
			catch(TargetInvocationException exception) {
				OnHandleException(exception.InnerException);
			}
			catch(Exception exception) {
				OnHandleException(exception);
			}
		}
		protected override void Dispose(Boolean disposing) {
			UnsubscribeFromEvents();
			this.attribute = null;
			this.methodInfo = null;
			this.parameterObjectType = null;
			base.Dispose(disposing);
		}
		protected Type ParameterObjectType {
			get { return this.parameterObjectType; }
		}
		public MethodParameterObjectAction(MethodParameterObjectAction sourceAction) {
			SetMethodInfo(sourceAction.MethodInfo);
			this.attribute = sourceAction.Attribute;
			this.parameterObjectType = sourceAction.ParameterObjectType;
			AssignFrom(sourceAction);
			Id = sourceAction.Id;
			SubscribeToEvents();
		}
		public MethodParameterObjectAction(MethodInfo methodInfo, ActionAttribute attribute) {
			SetMethodInfo(methodInfo);
			this.attribute = attribute;
			this.parameterObjectType = methodInfo.GetParameters()[0].ParameterType;
			Id = MethodActionHelper.GetMethodActionId(methodInfo);
			MethodActionHelper.SetMethodActionInfo(this, methodInfo, attribute);
			SubscribeToEvents();
		}
		public MethodInfo MethodInfo {
			get { return this.methodInfo; }
		}
		public ActionAttribute Attribute {
			get { return this.attribute; }
		}
		IObjectSpace IObjectSpaceLink.ObjectSpace {
			get { return objectSpace; }
			set { objectSpace = value; }
		}
	}
}
