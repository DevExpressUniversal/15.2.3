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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public class Frame : IDisposable {
		private object tag;
		private TemplateContext context;
		private bool isDisposed;
		private IFrameTemplate template;
		private View view;
		private XafApplication application;
		private LightDictionary<Type, Controller> controllers;
		protected Dictionary<Type, Controller> descendantControllers;
		protected internal Boolean isViewControllersActivation;
		private void template_Disposed(object sender, EventArgs e) {
			if(!isDisposed) {
				SetTemplate(null);
			}
		}
		private void LockControllers() {
			foreach(ISupportUpdate controller in controllers) {
				controller.BeginUpdate();
			}
		}
		private void UnlockControllers() {
			foreach(ISupportUpdate controller in controllers) {
				controller.EndUpdate();
			}
		}
		private void LockUpdates() {
			LockControllers();
			ISupportUpdate supportUpdate = template as ISupportUpdate;
			if(supportUpdate != null) {
				supportUpdate.BeginUpdate();
			}
		}
		private void UnlockUpdates() {
			UnlockControllers();
			ISupportUpdate supportUpdate = template as ISupportUpdate;
			if(supportUpdate != null) {
				supportUpdate.EndUpdate();
			}
		}
		private void ShowViewAction_ProcessCreatedView(object sender, ActionBaseEventArgs e) {
			ProcessActionCreatedView(e);
		}
		private void SubscribeToTemplate(IFrameTemplate template) {
			IComponent component = template as IComponent;
			if(component != null) {
				component.Disposed += new EventHandler(template_Disposed);
			}
			IDynamicContainersTemplate dynamicContainersTemplate = template as IDynamicContainersTemplate;
			if(dynamicContainersTemplate != null) {
				dynamicContainersTemplate.ActionContainersChanged += new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
			}
		}
		private void UnsubscribeFromTemplate(IFrameTemplate template) {
			IComponent component = template as IComponent;
			if(component != null) {
				component.Disposed -= new EventHandler(template_Disposed);
			}
			IDynamicContainersTemplate dynamicContainersTemplate = template as IDynamicContainersTemplate;
			if(dynamicContainersTemplate != null) {
				dynamicContainersTemplate.ActionContainersChanged -= new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
			}
		}
		private void dynamicContainersTemplate_ActionContainersChanged(object sender, ActionContainersChangedEventArgs e) {
			if(e.ChangedType == ActionContainersChangedType.Added) {
				RaiseProcessActionContainerEvents(e.ActionContainers);
			}
		}
		private void Actions_ActionAdded(object sender, DevExpress.ExpressApp.Core.ActionManipulationEventArgs e) {
			e.Action.ProcessCreatedView -= new EventHandler<ActionBaseEventArgs>(ShowViewAction_ProcessCreatedView);
			e.Action.ProcessCreatedView += new EventHandler<ActionBaseEventArgs>(ShowViewAction_ProcessCreatedView);
		}
		private void CurrentAspectChanged(object sender, EventArgs e) {
			foreach(Controller controller in controllers) {
				foreach(ActionBase action in controller.Actions) {
					action.NotifyAboutCurrentAspectChanged();
				}
			}
		}
		private void RaiseProcessActionContainerEvents(IEnumerable<IActionContainer> actionContainers) {
			if(actionContainers != null) {
				foreach(IActionContainer actionContainer in actionContainers) {
					OnProcessActionContainer(actionContainer);
				}
			}
		}
		protected void CheckIsDisposed() {
			if(isDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
		protected void DeactivateViewControllers() {
			foreach(Controller controller in Controllers) {
				ViewController viewController = controller as ViewController;
				if(viewController != null) {
					viewController.SetView(null);
				}
			}
		}
		protected void SaveViewModel() {
			CheckIsDisposed();
			if(view != null) {
				CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
				OnViewModelSaving(cancelEventArgs);
				if(!cancelEventArgs.Cancel) {
					view.SaveModel();
				}
			}
		}
		protected internal void SaveTemplateModel() {
			CheckIsDisposed();
			if(template is ISupportStoreSettings) {
				CancelEventArgs cancelEventArgs = new CancelEventArgs(false);
				OnTemplateModelSaving(cancelEventArgs);
				if(!cancelEventArgs.Cancel) {
					((ISupportStoreSettings)template).SaveSettings();
				}
			}
		}
		protected void ActivateViewControllers() {
			isViewControllersActivation = true;
			try {
				foreach(Controller controller in Controllers) {
					if(controller is ViewController) {
						((ViewController)controller).SetView(view);
					}
				}
			}
			finally {
				isViewControllersActivation = false;
			}
			OnViewControllersActivated();
			foreach(Controller controller in Controllers) {
				if((controller is ViewController) && controller.Active) {
					((ViewController)controller).OnViewControllersActivated();
				}
			}
		}
		protected internal void ClearTemplate() {
			SetTemplate(null);
		}
		protected virtual void OnProcessActionContainer(IActionContainer actionContainer) {
			if(ProcessActionContainer != null) {
				ProcessActionContainer(this, new ProcessActionContainerEventArgs(actionContainer));
			}
		}
		protected virtual void ProcessActionCreatedView(ActionBaseEventArgs e) {
			if(e.ShowViewParameters.CreatedView != null) {
				Application.ShowViewStrategy.ShowView(e.ShowViewParameters, new ShowViewSource(this, e.Action));
			}
		}
		protected virtual void OnViewChanging(View view, Frame sourceFrame, ViewChangingEventArgs args) {
			if(ViewChanging != null) {
				ViewChanging(this, args);
			}
			if((Application != null) && (view != null)) {
				Application.OnViewShowing(this, view, sourceFrame);
			}
		}
		protected virtual void OnViewChanged(Frame sourceFrame) {
			if(ViewChanged != null) {
				ViewChanged(this, new ViewChangedEventArgs(sourceFrame));
			}
			if((Application != null) && (view != null)) {
				Application.OnViewShown(this, sourceFrame);
			}
		}
		protected virtual void OnViewControllersActivated() {
			if(ViewControllersActivated != null) {
				ViewControllersActivated(this, EventArgs.Empty);
			}
		}
		protected virtual void OnTemplateModelSaving(CancelEventArgs cancelEventArgs) {
			if(TemplateModelSaving != null) {
				TemplateModelSaving(this, cancelEventArgs);
			}
		}
		protected virtual void OnViewModelSaving(CancelEventArgs cancelEventArgs) {
			if(ViewModelSaving != null) {
				ViewModelSaving(this, cancelEventArgs);
			}
		}
		protected virtual void OnTemplateViewChanged() {
			if(TemplateViewChanged != null) {
				TemplateViewChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnTemplateChanging() {
			if(TemplateChanging != null) {
				TemplateChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void OnTemplateChanged() {
			if(TemplateChanged != null) {
				TemplateChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void DisposeView(View view) {
			if(view == null) {
				throw new ArgumentNullException("view");
			}
			view.Dispose();
		}
		protected virtual void OnControllerRegistered(Controller controller) { }
		protected virtual void DisposeCore() { }
		protected virtual void OnDisposed() {
			if(Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
		}
		protected virtual void OnDisposing() {
			if(Disposing != null) {
				Disposing(this, EventArgs.Empty);
			}
		}
		protected virtual void SetTemplateView(View view) {
			if(template == null) {
				throw new InvalidOperationException("template is null");
			}
			template.SetView(view);
			OnTemplateViewChanged();
		}
		protected bool HasTemplate {
			get { return (template != null); }
		}
		protected bool IsDisposed {
			get { return isDisposed; }
		}
		public Frame(XafApplication application, TemplateContext context, params Controller[] controllers)
			: this(application, context, (ICollection<Controller>)controllers) {
		}
		public Frame(XafApplication application, TemplateContext context, ICollection<Controller> controllers) {
			Initialize(application, context, controllers);
		}
		private void Initialize(XafApplication application, TemplateContext context, ICollection<Controller> controllers) {
			this.application = application;
			this.context = context;
			this.controllers = new LightDictionary<Type, Controller>();
			descendantControllers = new Dictionary<Type, Controller>();
			if(controllers != null) {
				foreach(Controller controller in controllers) {
					this.controllers.Add(controller.GetType(), controller);
				}
				Tracing.Tracer.LockFlush();
				try {
					foreach(Controller controller in controllers) {
						RegisterController(controller);
					}
				}
				finally {
					Tracing.Tracer.ResumeFlush();
				}
			}
			if((application != null)) {
				application.CurrentAspectProvider.CurrentAspectChanged += new EventHandler(CurrentAspectChanged);
			}
		}
		public void RegisterController(Controller controller) {
			CheckIsDisposed();
			if(controller.Frame == null) {
				if(!this.controllers.ContainsKey(controller.GetType())) {
					this.controllers.Add(controller.GetType(), controller);
				}
				controller.SetFrame(this);
			}
			else if(controller.Frame != this) {
				throw new ArgumentException(string.Format(
					"The '{0}' controller already placed in the '{1}' frame", controller, controller.Frame));
			}
			controller.Actions.ActionAdded += new EventHandler<DevExpress.ExpressApp.Core.ActionManipulationEventArgs>(Actions_ActionAdded);
			foreach(ActionBase action in controller.Actions) {
				action.ProcessCreatedView -= new EventHandler<ActionBaseEventArgs>(ShowViewAction_ProcessCreatedView);
				action.ProcessCreatedView += new EventHandler<ActionBaseEventArgs>(ShowViewAction_ProcessCreatedView);
			}
			if((View != null) && (controller is ViewController) && !controller.Active) {
				((ViewController)controller).SetView(view);
			}
			if(!(controller is ViewController) && !(controller is WindowController)) {
				controller.Active.SetItemValue("Frame", true);
			}
			OnControllerRegistered(controller);
		}
		public void Dispose() {
			if(isDisposed) {
				return;
			}
			isDisposed = true;
			SafeExecutor executor = new SafeExecutor(this);
			executor.Execute(delegate() {
				OnDisposing();
				DisposeCore();
			});
			if(controllers != null) {
				foreach(Controller controller in controllers) {
					if(controller.Actions != null) {
						controller.Actions.ActionAdded -= new EventHandler<DevExpress.ExpressApp.Core.ActionManipulationEventArgs>(Actions_ActionAdded);
						foreach(ActionBase action in controller.Actions) {
							action.ProcessCreatedView -= new EventHandler<ActionBaseEventArgs>(ShowViewAction_ProcessCreatedView);
						}
					}
					executor.Execute(delegate() {
						controller.Active.SetItemValue("Frame", false);
					});
					executor.Dispose(controller);
				}
				controllers.Clear();
			}
			if(descendantControllers != null) {
				descendantControllers.Clear();
			}
			if(view != null) {
				executor.Dispose(view, view.Id);
				view = null;
			}
			if(template != null) {
				UnsubscribeFromTemplate(template);
				if(template is IDisposable) {
					executor.Dispose((IDisposable)template);
				}
				template = null;
			}
			if(application != null) {
				if((application != null && application.CurrentAspectProvider != null)) {
					application.CurrentAspectProvider.CurrentAspectChanged -= new EventHandler(CurrentAspectChanged);
				}
				application = null;
			}
			executor.Execute(delegate() {
				OnDisposed();
			});
			ViewChanging = null;
			ViewChanged = null;
			TemplateChanging = null;
			TemplateChanged = null;
			TemplateModelSaving = null;
			ViewModelSaving = null;
			Disposing = null;
			Disposed = null;
			executor.ThrowExceptionIfAny();
		}
		public void SaveModel() {
			SaveViewModel();
			SaveTemplateModel();
		}
		[Browsable(false)]
		public void CreateTemplate() {
			if(!HasTemplate) {
				if(application != null) {
					SetTemplate(application.CreateTemplate(context));
				}
			}
		}
		public void SetTemplate(IFrameTemplate val) {
			CheckIsDisposed();
			if(template != val) {
				Tracing.Tracer.LogVerboseText("Frame.set_Template: old value - {0}, new value - {1}", (template == null) ? "null" : template.GetType().FullName, (val == null) ? "null" : val.GetType().FullName);
				OnTemplateChanging();
				UnsubscribeFromTemplate(template);
				template = val;
				OnTemplateChanged();
				if(template != null) {
					RaiseProcessActionContainerEvents(template.GetContainers());
					SubscribeToTemplate(template);
					if(view != null) {
						SetTemplateView(view);
					}
				}
			}
		}
		public bool SetView(View view) {
			return SetView(view, true, null);
		}
		public bool SetView(View view, Frame sourceFrame) {
			return SetView(view, true, sourceFrame);
		}
		public bool SetView(View view, bool updateControllers, Frame sourceFrame) {
			CheckIsDisposed();
			LockUpdates();
			try {
				View oldView = this.view;
				if(oldView != null && !oldView.CanClose()) {
					return false;
				}
				try {
					ViewChangingEventArgs args = new ViewChangingEventArgs(view, sourceFrame, true);
					OnViewChanging(view, sourceFrame, args);
					if(oldView != null) {
						if(args.DisposeOldView) {
							oldView.Close(false);
						}
						oldView.SaveModel();
					}
					DeactivateViewControllers();
					if((oldView != null) && args.DisposeOldView) {
						try {
							DisposeView(oldView);
						}
						catch {
							this.view = null;
							throw;
						}
					}
					this.view = view;
					if(updateControllers && (view != null)) {
						ActivateViewControllers();
					}
					if(template != null) {
						SetTemplateView(view);
					}
					OnViewChanged(sourceFrame);
					if(template != null) {
						RaiseProcessActionContainerEvents(template.GetContainers());
					}
				}
				catch(Exception e) {
					throw new InvalidOperationException(string.Format(
						"Exception occurs while assigning the '{0}' view to {1}:\r\n{2}",
						(view != null) ? view.ToString() : "null", (this != null) ? this.GetType().Name : "null", e.Message), e);
				}
			}
			finally {
				UnlockUpdates();
			}
			return true;
		}
		public ControllerType GetController<ControllerType>() where ControllerType : Controller {
			Controller result = controllers[typeof(ControllerType)];
			if(result == null) {
				descendantControllers.TryGetValue(typeof(ControllerType), out result);
				if(result == null) {
					foreach(Controller controller in controllers) {
						if(typeof(ControllerType).IsAssignableFrom(controller.GetType())) {
							descendantControllers[typeof(ControllerType)] = controller;
							result = controller;
							break;
						}
					}
				}
			}
			return (ControllerType)result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IList<Controller> GetControllers(Type type) {
			List<Controller> result = new List<Controller>();
			foreach(Controller controller in controllers) {
				if(type.IsAssignableFrom(controller.GetType())) {
					result.Add(controller);
				}
			}
			return result;
		}
		public void SetControllersActive(string reason, bool isActive) {
			CheckIsDisposed();
			LockUpdates();
			try {
				foreach(Controller controller in controllers.GetValues()) {
					controller.Active.SetItemValue(reason, isActive);
				}
			}
			finally {
				UnlockUpdates();
			}
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FrameContext")]
#endif
		public TemplateContext Context {
			get { return context; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FrameApplication")]
#endif
		public XafApplication Application {
			get { return application; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FrameView")]
#endif
		public View View {
			get { return view; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FrameTemplate")]
#endif
		public IFrameTemplate Template {
			get { return template; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FrameControllers")]
#endif
		public LightDictionary<Type, Controller> Controllers {
			get { return controllers; }
		}
		[BindableAttribute(true)]
		[TypeConverterAttribute(typeof(StringConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		public Boolean IsViewControllersActivation {
			get { return isViewControllersActivation; }
		}
		public event EventHandler<ProcessActionContainerEventArgs> ProcessActionContainer;
		public event EventHandler<ViewChangingEventArgs> ViewChanging;
		public event EventHandler<ViewChangedEventArgs> ViewChanged;
		public event EventHandler ViewControllersActivated;
		public event EventHandler TemplateChanging;
		public event EventHandler TemplateChanged;
		public event EventHandler TemplateViewChanged;
		public event EventHandler<CancelEventArgs> TemplateModelSaving;
		public event EventHandler<CancelEventArgs> ViewModelSaving;
		public event EventHandler Disposing;
		public event EventHandler Disposed;
	}
}
