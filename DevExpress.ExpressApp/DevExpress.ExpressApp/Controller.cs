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
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	[ToolboxItem(false)]
	[Designer("DevExpress.ExpressApp.Design.ControllerDesigner, DevExpress.ExpressApp.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IRootDesigner))]
	[DesignerCategory("Component")]
	[DefaultEvent("Activated")]
	public abstract class Controller : Component, ISupportUpdate {
		private object tag;
		public const string ControllerActiveKey = "Controller active";
		private ActionList actions;
		private XafApplication application;
		private Frame frame;
		private BoolList activeList;
		private void actions_ActionAdded(object sender, ActionManipulationEventArgs e) {
			e.Action.Active.SetItemValue(ControllerActiveKey, Active);
			e.Action.SetController(this);
		}
		private void activeList_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			if(!Active) {
				OnDeactivated();
				UpdateActionsActivity();
			}
			else {
				UpdateActionsActivity();
				OnActivated();
			}
		}
		private void frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			foreach(ActionBase action in Actions) {
				action.SelectionContext = Active ? frame.View as ISelectionContext : null;
			}
		}
		private void UpdateActionsActivity() {
			foreach(ActionBase action in Actions) {
				UpdateActionActivity(action);
			}
		}
		private IModelActions GetActionsNode(IModelApplication model) { 
			IModelActionDesign actionDesign = model.ActionDesign;
			Guard.ArgumentNotNull(actionDesign, "model.ActionDesign");
			IModelActions modelActions = actionDesign.Actions;
			Guard.ArgumentNotNull(modelActions, "model.ActionDesign.Actions");
			return modelActions;
		}
		protected internal void SetFrame(Frame frame) {
			if(this.frame != null) {
				this.frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(frame_ViewChanged);
			}
			this.frame = frame;
			this.frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(frame_ViewChanged);
			this.application = frame.Application;
			foreach(ActionBase action in Actions) {
				action.Application = frame.Application;
			}
			OnFrameAssigned();
		}
		protected virtual void OnAfterConstruction() {
			if(AfterConstruction != null) {
				AfterConstruction(this, EventArgs.Empty);
			}
		}
		protected virtual void OnFrameAssigned() {
			if(FrameAssigned != null) {
				FrameAssigned(this, EventArgs.Empty);
			}
		}
		protected virtual void OnActivated() {
			if(Activated != null) {
				Activated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnDeactivated() {
			if(Deactivated != null) {
				Deactivated(this, EventArgs.Empty);
			}
		}
		protected virtual void BeginUpdate() {
			foreach(ISupportUpdate action in actions) {
				action.BeginUpdate();
			}
		}
		protected virtual void EndUpdate() {
			foreach(ISupportUpdate action in actions) {
				action.EndUpdate();
			}
		}
		protected virtual void UpdateActionActivity(ActionBase action) {
			action.Active.SetItemValue(ControllerActiveKey, Active);
		}
		protected void RegisterActions(IContainer container) {
			if(container != null) {
				foreach(object item in container.Components) {
					ActionBase action = item as ActionBase;
					if(action != null) {
						actions.Add(action);
					}
				}
			}
		}
		protected void RegisterActions(params ActionBase[] actions) {
			foreach(ActionBase action in actions) {
				this.actions.Add(action);
			}
		}
		protected void SetInfo(Controller destController, IModelApplication modelApplication) {
			if(modelApplication != null) {
				Guard.ArgumentNotNull(destController, "destController"); 
				IModelActions modelActions = GetActionsNode(modelApplication);  
				foreach(ActionBase destAction in destController.Actions) {
					Guard.ArgumentNotNull(destAction, "destAction"); 
					ActionBase thisAction = Actions[destAction.Id];
					if(thisAction != null) {
						IModelAction modelAction = modelActions[thisAction.Id];
						if(modelAction != null) {
							destAction.AssignInfo(modelAction);
						}
					}
				}
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(Active.ResultValue) {
						throw new Exception(string.Format("The active controller {0} is trying to dispose", GetType().FullName));
					}
					SafeExecutor executor = new SafeExecutor(this);
					if(actions != null) {
						actions.ActionAdded -= new EventHandler<ActionManipulationEventArgs>(actions_ActionAdded);
						foreach(ActionBase action in actions) {
							executor.Dispose(action, action.Id);
						}
						actions.Clear();
					}
					frame = null;
					application = null;
					activeList.ResultValueChanged -= new EventHandler<BoolValueChangedEventArgs>(activeList_ResultValueChanged);
					FrameAssigned = null;
					Activated = null;
					Deactivated = null;
					executor.ThrowExceptionIfAny();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected Controller() {
			activeList = new BoolList(false, BoolListOperatorType.And, ReadByNonExistentKeyMode.ThrowException);
			activeList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(activeList_ResultValueChanged);
			actions = new ActionList();
			actions.ActionAdded += new EventHandler<ActionManipulationEventArgs>(actions_ActionAdded);
		}
		public static Controller Create(Type controllerType) {
			Controller result = (Controller)TypeHelper.CreateInstance(controllerType);
			result.OnAfterConstruction();
			return result;
		}
		public virtual Controller Clone(IModelApplication modelApplication) {
			Controller result = Create(GetType());
			SetInfo(result, modelApplication);
			return result;
		}
		public virtual void CustomizeTypesInfo(ITypesInfo typesInfo) { }
		[Browsable(false)]
		public BoolList Active {
			get { return activeList; }
		}
		[Browsable(false)]
		public XafApplication Application {
			get { return application; }
			set { application = value; }
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ActionList Actions {
			get { return actions; }
		}
		[Browsable(false)]
		public Frame Frame {
			get { return frame; }
		}
		[Browsable(false)]
		public string Name {
			get { return GetType().FullName; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ControllerTag"),
#endif
 BindableAttribute(true), Category("Data"), TypeConverterAttribute(typeof(StringConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ControllerAfterConstruction"),
#endif
 Category("Events")]
		public event EventHandler AfterConstruction;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ControllerFrameAssigned"),
#endif
 Category("Events")]
		public event EventHandler FrameAssigned;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ControllerActivated"),
#endif
 Category("Events")]
		public event EventHandler Activated;
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ControllerDeactivated"),
#endif
 Category("Events")]
		public event EventHandler Deactivated;
		#region ISupportUpdate Members
		void ISupportUpdate.BeginUpdate() {
			BeginUpdate();
		}
		void ISupportUpdate.EndUpdate() {
			EndUpdate();
		}
		#endregion
		#region Obsolete 14.2
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		protected string GetControllerActionId(string actionId) {
#pragma warning disable 0618
			return GetControllerActionId(Name, actionId);
#pragma warning restore 0618
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		protected string GetControllerActionId(string controllerName, string actionId) {
			return controllerName + "." + actionId;
		}
		#endregion
	}
	public interface IModelExtender {
		void ExtendModelInterfaces(ModelInterfaceExtenders extenders);
	}
}
