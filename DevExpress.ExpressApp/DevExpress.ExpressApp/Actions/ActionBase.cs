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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using DevExpress.ExpressApp.Core.Design;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeWrappers;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Actions {
	public enum TargetObjectsCriteriaMode { TrueAtLeastForOne, TrueForAll }
	public enum SelectionDependencyType { Independent, RequireSingleObject, RequireMultipleObjects }
	[Flags]
	public enum ActionMeaning { Unknown, Accept, Cancel }
	[Serializable]
	public class ActionExecutionException : Exception {
		private ActionBase action;
		protected ActionExecutionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public ActionExecutionException(ActionBase action, Exception innerException)
			: base(innerException.Message, innerException) {
			this.action = action;
		}
		public ActionBase Action {
			get { return action; }
		}
	}
	public class ActionBaseEventArgs : EventArgs {
		private ActionBase action;
		private ShowViewParameters showViewParameters = new ShowViewParameters();
		public ActionBaseEventArgs(ActionBase action) {
			this.action = action;
		}
		public ActionBase Action {
			get { return action; }
		}
		public ShowViewParameters ShowViewParameters {
			get { return showViewParameters; }
		}
	}
	public class CustomGetTotalTooltipEventArgs : EventArgs {
		public CustomGetTotalTooltipEventArgs (String tooltip) {
			Tooltip = tooltip;
		}
		public String Tooltip { get; set; }
	}
	public class CustomGetFormattedConfirmationMessageEventArgs : EventArgs {
		public CustomGetFormattedConfirmationMessageEventArgs(String confirmationMessage) {
			ConfirmationMessage = confirmationMessage;
		}
		public String ConfirmationMessage { get; set; }
	}
	public class HandleExceptionEventArgs : HandledEventArgs {
		private Exception exception;
		public HandleExceptionEventArgs(Exception exception) {
			this.exception = exception;
		}
		public Exception Exception {
			get { return exception; }
		}
	}
	[DefaultEvent("Execute")]
	[DebuggerDisplay("Id = {Id}")]
	[ToolboxItemFilter("DevExpress.ExpressApp.Controller", ToolboxItemFilterType.Require)]
	public abstract class ActionBase : Component, ISupportUpdate {
		private object tag;
		public const string AnyCaption = "Any";
		protected const string CaptionChangedEventName = "CaptionChanged";
		protected const string StateChangedEventName = "StateChanged";
		protected const string EnabledChangedEventName = "EnabledChanged";
		protected const string ActiveChangedEventName = "ActiveChanged";
		protected const string ToolTipChangedEventName = "ToolTipChanged";
		protected const string ImageChangedEventName = "ImageChanged";
		protected const string ConfirmationMessageChangedEventName = "ConfirmationMessageChanged";
		protected const string ShortcutEventName = "ShortcutChanged";
		public const string RequireSingleObjectContext = "ByContext_RequireSingleObject";
		public const string RequireMultipleObjectsContext = "ByContext_RequireMultipleObjects";
		private string id = string.Empty;
		private ActionMeaning actionMeaning = ActionMeaning.Unknown;
		private string diagnosticInfo;
		private int lockCount;
		private bool isDisposed = false;
		private ISelectionContext selectionContext;
		private BoolList enabledList;
		private BoolList activeList;
		private Type typeOfView;
		private ModelActionInternal model = null;
		private string disableReasonsHint = string.Empty;
		private List<ActionChangedType> pendingEvents = new List<ActionChangedType>();
		private XafApplication application;
		private Controller controller;
		private void context_SelectionChanged(object sender, EventArgs e) {
			UpdateState();
		}
		private void activeList_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			if(e.NewValue) {
				OnActivated();
			}
			else {
				OnDeactivated();
			}
			RaiseChanged(ActionChangedType.Active);
		}
		private void enabledList_FalseItemsChanged(object sender, EventArgs e) {
			string oldHint = disableReasonsHint;
			disableReasonsHint = Enabled ? string.Empty : GetReasonsForFalse(Enabled, false);
			if(oldHint != disableReasonsHint) {
				RaiseChanged(ActionChangedType.ToolTip);
			}
		}
		private void enabledList_ResultValueChanged(object sender, BoolValueChangedEventArgs e) {
			RaiseChanged(ActionChangedType.Enabled);
		}
		private string GetReasonCaption(string id, IModelDisableReasons actionReasons) {
			IModelReason reason = null; ;
			if(actionReasons != null) {
				reason = actionReasons[id];
			}
			if(reason == null) {
				if(Model.Application != null && Model.Application.ActionDesign != null) {
					reason = Model.Application.ActionDesign.DisableReasons[id];
				}
			}
			if(reason != null) {
				return reason.Caption;
			}
			return null;
		}
		private void UnSubscribeSelectionContextEvents() {
			if(selectionContext != null) {
				selectionContext.SelectionChanged -= new EventHandler(context_SelectionChanged);
				selectionContext.CurrentObjectChanged -= new EventHandler(context_SelectionChanged);
				selectionContext.SelectionTypeChanged -= new EventHandler(context_SelectionChanged);
			}
		}
		public virtual void AssignInfo(IModelAction newInfo) {
			if (newInfo != null && Model != newInfo) {
				if(newInfo is ModelActionInternal) {
					model = (ModelActionInternal)newInfo;
				}
				else {
					model = new ModelActionInternal(newInfo);
				}
				id = model.Id;
			}
		}
		protected internal void RaiseChanged(ActionChangedType propertyType) {
			if(LockCount > 0) {
				if(!pendingEvents.Contains(propertyType)) {
					pendingEvents.Add(propertyType);
				}
			}
			else {
				OnChanged(new ActionChangedEventArgs(propertyType));
			}
		}
		protected internal virtual void NotifyAboutCurrentAspectChanged() {
			RaiseChanged(ActionChangedType.Caption);
			RaiseChanged(ActionChangedType.ToolTip);
		}
		protected string GetReasonsForFalse(BoolList reasons, bool includeReasonsWithoutUserFriendlyText) {
			List<string> result = new List<string>();
			IModelDisableReasons actionReasons = Model.DisableReasons;
			foreach(string key in reasons.GetKeys()) {
				if(!reasons[key]) {
					string reason = GetReasonCaption(key, actionReasons);
					if(!string.IsNullOrEmpty(reason)) {
						result.Add(reason);
					}
					else {
						if(includeReasonsWithoutUserFriendlyText) {
							result.Add(key);
						}
					}
				}
			}
			return string.Join(Environment.NewLine, result.ToArray());
		}
		protected void LogActionEnd() {
			Tracing.Tracer.LogText("Action '{0}' done", Id);
		}
		protected void LogActionCancel() {
			Tracing.Tracer.LogText("Action '{0}' executing canceled", Id);
		}
		protected bool ExecuteCore(Delegate handler, ActionBaseEventArgs eventArgs) {
			bool result = false;
			Tracing.Tracer.LogSubSeparator("Execute action");
			LogActionInfo();
			if(Enabled && Active) {
				try {
					CancelEventArgs args = new CancelEventArgs(false);
					OnExecuting(args);
					if(!args.Cancel) {
						if(handler != null) {
							RaiseExecute(eventArgs);
						}
						OnExecuted(eventArgs);
						OnProcessCreatedView(eventArgs);
						OnExecuteCompleted(eventArgs);
						result = true;
					}
					else {
						OnExecuteCanceled(eventArgs);
						LogActionCancel();
					}
					LogActionEnd();
				}
				catch(ThreadAbortException) {
				}
				catch(TargetInvocationException e) {
					OnHandleException(e.InnerException);
				}
				catch(Exception e) {
					OnHandleException(e);
				}
			}
			else {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToExecuteDisabledOrInactiveAction, GetReasonsForFalse(Enabled, true), GetReasonsForFalse(Active, true), id));
			}
			return result;
		}
		protected internal abstract void RaiseExecute(ActionBaseEventArgs eventArgs);
		protected internal void UpdateState() {
				Enabled.RemoveItem(RequireSingleObjectContext);
				Enabled.RemoveItem(RequireMultipleObjectsContext);
				Active.RemoveItem(RequireSingleObjectContext);
				Active.RemoveItem(RequireMultipleObjectsContext);
				if(selectionContext != null) {
					SelectionType selectionType = selectionContext.SelectionType;
					bool actionIsActive = (SelectionType.MultipleSelection & selectionType) == SelectionType.MultipleSelection
						|| ((SelectionType.TemporarySelection & selectionType) == SelectionType.TemporarySelection && selectionContext.SelectedObjects.Count == 1);
					switch(SelectionDependencyType) {
						case SelectionDependencyType.RequireSingleObject: {
								if(selectionContext.SelectionType != SelectionType.None) {
									Enabled[RequireSingleObjectContext] = selectionContext.SelectedObjects.Count == 1;
									RaiseChanged(ActionChangedType.ConfirmationMessage);
								}
								Active[RequireSingleObjectContext] = actionIsActive;
								break;
							}
						case SelectionDependencyType.RequireMultipleObjects: {
								if(selectionContext.SelectionType != SelectionType.None) {
									Enabled[RequireMultipleObjectsContext] = selectionContext.SelectedObjects.Count > 0 || ((SelectionType.TemporarySelection & selectionType) == SelectionType.TemporarySelection);
									RaiseChanged(ActionChangedType.ConfirmationMessage);
								}
								Active[RequireMultipleObjectsContext] = actionIsActive;
							}
							break;
					}
				}
		}
		protected virtual void LogActionInfo() {
			string result = Tracing.Tracer.GetMessageByValue("Type", GetType().ToString(), false);
			result += Tracing.Tracer.GetMessageByValue("ID", Id, true);
			result += Tracing.Tracer.GetMessageByValue("Category", Category, true);
			if(controller != null) {
				result += Tracing.Tracer.GetMessageByValue("Controller.Name", controller.Name, true);
			}
			if(SelectionContext != null) {
				result += Tracing.Tracer.GetMessageByValue("Context.Name", SelectionContext.Name, true);
				result += Tracing.Tracer.GetMessageByValue("Context.IsRoot", SelectionContext.IsRoot, true);
				if(SelectionContext.SelectedObjects != null) {
					result += Tracing.Tracer.GetMessageByValue("Context.SelectedObjects.Count", SelectionContext.SelectedObjects.Count, true);
				}
				result += Tracing.Tracer.GetMessageByValue("Context.CurrentObject", SelectionContext.CurrentObject, true);
			}
			Tracing.Tracer.LogText(result);
		}
		protected virtual void ReleaseLockedEvents() {
			foreach(ActionChangedType propertyType in pendingEvents) {
				OnChanged(new ActionChangedEventArgs(propertyType));
			}
			pendingEvents.Clear();
		}
		protected virtual void OnChanged(ActionChangedEventArgs args) {
			if(Changed != null) {
				Changed(this, args);
			}
		}
		protected virtual void OnActivated() { }
		protected virtual void OnDeactivated() { }
		protected virtual void OnExecuted(ActionBaseEventArgs e) {
			if(Executed != null) {
				Executed(this, e);
			}
		}
		protected virtual void OnExecuteCanceled(ActionBaseEventArgs e) {
			if(ExecuteCanceled != null) {
				ExecuteCanceled(this, e);
			}
		}
		protected virtual void OnExecuteCompleted(ActionBaseEventArgs e) {
			if(ExecuteCompleted != null) {
				ExecuteCompleted(this, e);
			}
		}
		protected virtual void OnProcessCreatedView(ActionBaseEventArgs e) {
			if(ProcessCreatedView != null) {
				ProcessCreatedView(this, e);
			}
		}
		protected virtual void OnExecuting(CancelEventArgs e) {
			if(Executing != null) {
				Executing(this, e);
			}
		}
		protected virtual void OnHandleException(Exception e) {
			HandleExceptionEventArgs args = new HandleExceptionEventArgs(e);
			if(HandleException != null) {
				HandleException(this, args);
			}
			if(!args.Handled) {
				throw new ActionExecutionException(this, e);
			}
		}
		protected virtual void OnCustomGetTotalTooltip(CustomGetTotalTooltipEventArgs args) {
			if(CustomGetTotalTooltip != null) {
				CustomGetTotalTooltip(this, args);
			}
		}
		protected virtual void OnCustomGetFormattedConfirmationMessage(CustomGetFormattedConfirmationMessageEventArgs args) {
			if(CustomGetFormattedConfirmationMessage != null) {
				CustomGetFormattedConfirmationMessage(this, args);
			}
		}
		protected override void Dispose(bool disposing) {
			if(isDisposed) {
				return;
			}
			try {
				isDisposed = true;
				if(disposing) {
					if(Disposing != null) {
						Disposing(this, EventArgs.Empty);
					}
				}
				UnSubscribeSelectionContextEvents();
				selectionContext = null;
				Disposing = null;
				Changed = null;
				Executed = null;
				Executing = null;
				ProcessCreatedView = null;
				HandleException = null;
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal bool Available {
			get { return Active && Enabled; }
		}
		protected ActionBase(IContainer container) {
			if(container != null) {
				container.Add(this);
			}
			controller = container as Controller;
		}
		protected ActionBase(Controller owner, String id, PredefinedCategory category)
			: this(owner, id, category.ToString()) {
		}
		protected ActionBase(Controller owner, String id, String category) {
			this.Id = id;
			this.Category = category;
			this.controller = owner;
			if(owner != null) {
				owner.Actions.Add(this);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetId() {
			id = Guid.NewGuid().ToString();
		}
		public void AssignFrom(ActionBase action) {
			AssignInfo(action.Model);
		}
		protected internal void SetController(Controller controller) {
			this.controller = controller;
			if((controller != null) && (controller.Frame != null)) {
				SelectionContext = controller.Frame.View;
			}
		}
		public String GetFormattedConfirmationMessage() {
			String result = (ConfirmationMessage != null) ? ConfirmationMessage : "";
			if(selectionContext != null) {
				switch(SelectionDependencyType) {
					case SelectionDependencyType.RequireSingleObject: {
						if(selectionContext.SelectedObjects.Count == 1 && !string.IsNullOrEmpty(result)
							&& (selectionContext.SelectedObjects[0] != null)) {
							result = String.Format(result, ReflectionHelper.GetObjectDisplayText(selectionContext.SelectedObjects[0]));
						}
						break;
					}
					case SelectionDependencyType.RequireMultipleObjects: {
						result = String.Format(result, selectionContext.SelectedObjects.Count.ToString());
						break;
					}
				}
			}
			CustomGetFormattedConfirmationMessageEventArgs args = new CustomGetFormattedConfirmationMessageEventArgs(result);
			OnCustomGetFormattedConfirmationMessage(args);
			result = args.ConfirmationMessage;
			return result;
		}
		public override String ToString() {
			return string.Format("ID=\"{0}\", Controller=\"{1}\"", Id, Controller);
		}
		public virtual String GetTotalToolTip() {
			return GetTotalToolTip("");
		}
		public virtual String GetTotalToolTip(String itemCaption) {
			String result = ToolTip;
			if(String.IsNullOrEmpty(result) || String.IsNullOrEmpty(itemCaption)) {
				result += itemCaption;
			}
			else {
				result += " " + itemCaption;
			}
			if(String.IsNullOrEmpty(disableReasonsHint) || String.IsNullOrEmpty(result)) {
				result += disableReasonsHint;
			}
			else {
				result += "\r\n" + disableReasonsHint;
			}
			CustomGetTotalTooltipEventArgs args = new CustomGetTotalTooltipEventArgs(result);
			OnCustomGetTotalTooltip(args);
			result = args.Tooltip;
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public Boolean IsExecuting { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int LockCount { get { return lockCount; } }
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public BoolList Enabled {
			get {
				if(enabledList == null) {
					enabledList = new BoolList(true, BoolListOperatorType.And, ReadByNonExistentKeyMode.ThrowException);
					enabledList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(enabledList_ResultValueChanged);
					enabledList.Changed += new EventHandler<EventArgs>(enabledList_FalseItemsChanged);
				}
				return enabledList; 
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public BoolList Active {
			get {
				if(activeList == null) {
					activeList = new BoolList();
					activeList.ResultValueChanged += new EventHandler<BoolValueChangedEventArgs>(activeList_ResultValueChanged);
				}
				return activeList; 
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ISelectionContext SelectionContext {
			get { return selectionContext; }
			set {
				if(selectionContext != value) {
					UnSubscribeSelectionContextEvents();
					selectionContext = value;
					if(selectionContext != null) {
						selectionContext.SelectionChanged += new EventHandler(context_SelectionChanged);
						selectionContext.CurrentObjectChanged += new EventHandler(context_SelectionChanged);
						selectionContext.SelectionTypeChanged += new EventHandler(context_SelectionChanged);
					}
					RaiseChanged(ActionChangedType.SelectionContext);
					UpdateState();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public string TestName {
			get {
				ISelectionContext currentSelectionContext = selectionContext;
				if(currentSelectionContext == null) {
					if(controller != null && controller.Frame != null && controller.Frame.View != null) {
						currentSelectionContext = (ISelectionContext)controller.Frame.View;
					}
				}
				if((currentSelectionContext != null) && (!currentSelectionContext.IsRoot)) {
					return (currentSelectionContext.Name + '.' + Caption);
				}
				return Caption;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XafApplication Application {
			get { return application; }
			set { application = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Controller Controller {
			get { return controller; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#if !SL
	[DevExpressExpressAppLocalizedDescription("ActionBaseModel")]
#endif
		public IModelAction Model {
			get {
				if(model == null) {
					model = new ModelActionInternal(Id);
					model.Category = PredefinedCategory.Unspecified.ToString();
					model.SelectionDependencyType = SelectionDependencyType.Independent;
				}
				return model;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DiagnosticInfo {
			get { return diagnosticInfo; }
			set { diagnosticInfo = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseId"),
#endif
 Category("Design"), DesignTimeTest(DesignTimeTest.DefaulValueIsSpecified, DesignTimeTestMode.Skip)]
		public string Id {
			get {
				if(string.IsNullOrEmpty(id)) {
					id = Guid.NewGuid().ToString();
				}
				return id;
			}
			set {
				if(id != value) {
					id = value;
					if(string.IsNullOrEmpty(Caption) || Caption == CaptionHelper.ConvertCompoundName(Model.Id)) {
						Caption = CaptionHelper.ConvertCompoundName(value);
					}
					Model.Id = value;
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseSelectionDependencyType"),
#endif
 DefaultValue(SelectionDependencyType.Independent), Category("Behavior")]
		public SelectionDependencyType SelectionDependencyType {
			get { return Model.SelectionDependencyType; }
			set { Model.SelectionDependencyType = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseActionMeaning"),
#endif
 DefaultValue(ActionMeaning.Unknown), Category("Behavior")]
		public ActionMeaning ActionMeaning {
			get { return actionMeaning; }
			set { actionMeaning = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTag"),
#endif
 BindableAttribute(true), DefaultValue(null), Category("Data"), TypeConverterAttribute(typeof(StringConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		[Browsable(false)]
		public bool HasImage {
			get { return !string.IsNullOrEmpty(ImageName); }
		}
		[Browsable(false)]
		public event EventHandler Disposing;
		[Browsable(false)] 
		public event EventHandler<ActionBaseEventArgs> Executed;
		[Browsable(false)] 
		public event EventHandler<ActionBaseEventArgs> ExecuteCanceled;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public event EventHandler<ActionBaseEventArgs> ExecuteCompleted;
		[Browsable(false)] 
		public event CancelEventHandler Executing;
		[Browsable(false)]
		public event EventHandler<ActionBaseEventArgs> ProcessCreatedView;
		[Browsable(false)]
		public event EventHandler<HandleExceptionEventArgs> HandleException;
		[Browsable(false)]
		public event EventHandler<ActionChangedEventArgs> Changed;
		public event EventHandler<CustomGetTotalTooltipEventArgs> CustomGetTotalTooltip;
		public event EventHandler<CustomGetFormattedConfirmationMessageEventArgs> CustomGetFormattedConfirmationMessage;
		#region Target Options
		[Browsable(false), DefaultValue(null)]
		public Type TypeOfView {
			get { return typeOfView; }
			set { typeOfView = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetViewType"),
#endif
 DefaultValue(ViewType.Any), Category("Target")]
		public ViewType TargetViewType {
			get {
				if(typeof(ListView).IsAssignableFrom(typeOfView)) {
					return ViewType.ListView;
				}
				if(typeof(DetailView).IsAssignableFrom(typeOfView)) {
					return ViewType.DetailView;
				}
				if(typeof(DashboardView).IsAssignableFrom(typeOfView)) {
					return ViewType.DashboardView;
				}
				return ViewType.Any;
			}
			set {
				Model.TargetViewType = value;
				if(value == ViewType.ListView) {
					typeOfView = typeof(ListView);
				}
				else if(value == ViewType.DetailView) {
					typeOfView = typeof(DetailView);
				}
				else if(value == ViewType.DashboardView) {
					typeOfView = typeof(DashboardView);
				}
				else {
					typeOfView = typeof(View);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetViewNesting"),
#endif
 DefaultValue(Nesting.Any), Category("Target")]
		public Nesting TargetViewNesting {
			get { return Model.TargetViewNesting; }
			set { Model.TargetViewNesting = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetViewId"),
#endif
 DefaultValue(null), Category("Target")]
		public string TargetViewId {
			get { return Model.TargetViewId; }
			set { Model.TargetViewId = value; }
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetObjectsCriteria"),
#endif
 DefaultValue(null), Category("Target")]
		public string TargetObjectsCriteria {
			get { return Model.TargetObjectsCriteria; }
			set {
				Model.TargetObjectsCriteria = value;
				RaiseChanged(ActionChangedType.TargetObjectsCriteria);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetObjectsCriteriaMode"),
#endif
 DefaultValue(TargetObjectsCriteriaMode.TrueAtLeastForOne), Category("Target")]
		public TargetObjectsCriteriaMode TargetObjectsCriteriaMode {
			get { return Model.TargetObjectsCriteriaMode; }
			set {
				Model.TargetObjectsCriteriaMode = value;
				RaiseChanged(ActionChangedType.TargetObjectsCriteriaMode);
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseTargetObjectType"),
#endif
 DefaultValue(null), Category("Target")]
		[TypeConverter(typeof(BusinessClassTypeConverter<object>))]
		public Type TargetObjectType {
			get { return ReflectionHelper.FindType(Model.TargetObjectType); }
			set {
				if(value != null) {
					Model.TargetObjectType = value.FullName;
				}
				else {
					Model.TargetObjectType = "";
				}
				RaiseChanged(ActionChangedType.TargetObjectsCriteria);
			}
		}
		#endregion
		#region UI Options
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseImageName"),
#endif
 DefaultValue(null), Category("Appearance")]
		public string ImageName {
			get { return Model.ImageName; }
			set {
				if(Model.ImageName != value) {
					Model.ImageName = value;
					RaiseChanged(ActionChangedType.Image);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseCaption"),
#endif
 DefaultValue(""), Category("Appearance")]
		public string Caption {
			get { return Model.Caption; }
			set {
				if(Caption != value) {
					Model.Caption = value;
					RaiseChanged(ActionChangedType.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBasePaintStyle"),
#endif
 DefaultValue(DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Default), Category("Appearance")]
		public DevExpress.ExpressApp.Templates.ActionItemPaintStyle PaintStyle {
			get { return Model.PaintStyle; }
			set {
				if(PaintStyle != value) {
					Model.PaintStyle = value;
					RaiseChanged(ActionChangedType.PaintStyle);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseQuickAccess"),
#endif
 DefaultValue(false), Category("Appearance")]
		public bool QuickAccess {
			get { return Model.QuickAccess; }
			set {
				if(QuickAccess != value) {
					IModelAction modelAction = model;
					if(model.ModelNode != null) {
						modelAction = model.ModelNode;
					}
					modelAction.QuickAccess = value;
					RaiseChanged(ActionChangedType.QuickAccess);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseCategory"),
#endif
 TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ActionCategoryConverter)), DefaultValue("Unspecified"), Category("Appearance")]
		public string Category {
			get { return Model.Category; }
			set { Model.Category = value; }
		}
		[Editor(Constants.MultilineStringEditorType, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseConfirmationMessage"),
#endif
Category("Appearance"), DefaultValue("")]
		public string ConfirmationMessage {
			get { return Model.ConfirmationMessage; }
			set {
				if(Model.ConfirmationMessage != value) {
					Model.ConfirmationMessage = value;
					RaiseChanged(ActionChangedType.ConfirmationMessage);
				}
			}
		}
		[Editor(Constants.MultilineStringEditorType, typeof(System.Drawing.Design.UITypeEditor))]
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseToolTip"),
#endif
DefaultValue(""), Category("Appearance")]
		public string ToolTip {
			get { return Model.ToolTip; }
			set {
				if(Model.ToolTip != value) {
					Model.ToolTip = value;
					RaiseChanged(ActionChangedType.ToolTip);
				}
			}
		}
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("ActionBaseShortcut"),
#endif
 DefaultValue(null), Category("Behavior")]
		public string Shortcut {
			get { return Model.Shortcut; }
			set {
				if(Shortcut != value) {
					Model.Shortcut = value;
					RaiseChanged(ActionChangedType.Shortcut);
				}
			}
		}
		#endregion
		#region ISupportUpdate Members
		public void BeginUpdate() {
			lockCount++;
		}
		public void EndUpdate() {
			if(LockCount > 0) {
				lockCount--;
				if(LockCount == 0) {
					ReleaseLockedEvents();
				}
			}
		}
		#endregion
	}
	public enum ActionChangedType {
		Caption,
		Enabled,
		Active,
		ToolTip,
		Image,
		ConfirmationMessage,
		TargetObjectsCriteria, 
		TargetObjectsCriteriaMode,
		SelectionContext,
		PaintStyle,
		Shortcut,
		QuickAccess
	}
	public class ActionChangedEventArgs : EventArgs {
		private ActionChangedType changedPropertyType;
		public ActionChangedEventArgs(ActionChangedType changedPropertyType) {
			this.changedPropertyType = changedPropertyType;
		}
		public ActionChangedType ChangedPropertyType {
			get {
				return changedPropertyType;
			}
		}
	}
}
