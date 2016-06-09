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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Markup;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Collections;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Mvvm.UI.Interactivity;
namespace DevExpress.Xpf.Bars {
	public abstract class BarManagerControllerBase : FrameworkElement, IActionContainer {
		#region static
		static BarManagerControllerBase() {
		}
		public BarManagerControllerBase() { }
		#endregion
		public DependencyObject Owner { get; set; }
		public abstract void Execute();		
		DependencyObject IActionContainer.AssociatedObject {
			get { return Owner; }
			set { Owner = value; }
		}
		void IControllerAction.Execute(DependencyObject context) {
			Execute();
		}
		IActionContainer IControllerAction.Container { get; set; }
	}
	public class BarManagerController : BarManagerControllerBase {
		BarManagerActionContainer container;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerControllerActionContainer")]
#endif
		public BarManagerActionContainer ActionContainer {
			get {
				if (container == null) {
					container = CreateActionContainer();
					AddLogicalChild(container);
					SetUpActionContainer(container);
				}
				return container;
			}
		}
		protected virtual void SetUpActionContainer(BarManagerActionContainer container) {
			container.DataContext = DataContext;
			if (container.Controller != this)
				container.Controller = this;
		}
		public override void Execute() {
			if (ActionContainer == null)
				throw new ArgumentNullException("ActionContainer");
			ActionContainer.Execute();
		}
		protected virtual BarManagerActionContainer CreateActionContainer() { return new BarManagerActionContainer(this); }
		protected override IEnumerator LogicalChildren {
			get {
				return new SingleLogicalChildEnumerator(ActionContainer);
			}
		}
	}
	public class TemplatedBarManagerController : BarManagerController {
		#region static
		public static readonly DependencyProperty TemplateProperty;
		static TemplatedBarManagerController() {
			TemplateProperty = DependencyPropertyManager.Register("Template", typeof(DataTemplate), typeof(TemplatedBarManagerController), new FrameworkPropertyMetadata());
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("TemplatedBarManagerControllerTemplate")]
#endif
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		protected override BarManagerActionContainer CreateActionContainer() {
			if (Template == null)
				throw new ArgumentNullException("TemplateProperty");
			return (BarManagerActionContainer)Template.LoadContent();
		}
	}
	[ContentProperty("Actions")]
	public class BarManagerActionContainer : FrameworkElement, ILogicalChildrenContainer {
		#region static
		static BarManagerActionContainer() { }
		public BarManagerActionContainer() : this(null) { }
		public BarManagerActionContainer(BarManagerController barManagerController) {
			this.barManagerController = barManagerController;
		}
		#endregion
		BarManagerActionCollection actions;
		private BarManagerController barManagerController;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerActionContainerController")]
#endif
		public BarManagerController Controller { get { return barManagerController; } set { barManagerController = value; OnControllerChanged(); } }
		protected virtual void OnControllerChanged() {
			foreach (var action in Actions)
				action.Container = Controller;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerActionContainerActions")]
#endif
		public BarManagerActionCollection Actions {
			get {
				if (actions == null)
					actions = CreateActions();
				return actions;
			}
		}
		protected virtual BarManagerActionCollection CreateActions() {
			return new BarManagerActionCollection(this);
		}
		public virtual void Execute() {
			Actions.Execute();
		}
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			if (child is DependencyObject && LogicalTreeHelper.GetParent((DependencyObject)child) != null)
				return;
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			RemoveLogicalChild(child);
		}
		protected override IEnumerator LogicalChildren {
			get {
				return Actions.GetEnumerator();
			}
		}
	}
	static class ActionContainerHelper {
		public static IActionContainer GetRootContainer(this IControllerAction action) {
			if (action == null)
				return null;
			if (action.Container == null)
				return action as IActionContainer;
			return GetRootContainer(action.Container);
		}
		public static BarManager GetBarManager(this IActionContainer container) {
			if (container == null)
				return null;
			container = container.GetRootContainer();
			return (container as DependencyObject).With(BarManager.GetBarManager) ?? container.AssociatedObject.With(BarManager.GetBarManager);
		}
	}	
	public interface IActionContainer : IControllerAction {
		DependencyObject AssociatedObject { get; set; }
	}
	public interface IControllerAction {
		void Execute(DependencyObject context = null);
		IActionContainer Container { get; set; }		
	}
	public interface IBarManagerControllerAction : IControllerAction {		
		object GetObject();
	}
	public abstract class BarManagerControllerActionBase : DXFrameworkContentElement, IBarManagerControllerAction {
		public BarManagerControllerActionBase() { }		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerControllerActionBaseManager")]
#endif
		public virtual BarManager Manager { get { return Container.GetBarManager(); } }
		public IActionContainer Container { get; set; }
		#region IBarManagerControllerAction Members
		void IControllerAction.Execute(DependencyObject context) {
			using (CollectionAction.InitializeContext(this, context))
				ExecuteCore(context);
		}
		object IBarManagerControllerAction.GetObject() {
			return GetObjectCore();
		}
		#endregion
		protected virtual void ExecuteCore(DependencyObject context) { }
		public abstract object GetObjectCore();
		public virtual bool IsEqual(BarManagerControllerActionBase action) {
			if (action == null || action.GetType() != GetType()) return false;
			if (action.Manager != Manager) return false;
			return true;
		}
	}
	public enum ItemLinksHolderType { MainMenu, StatusBar, Other }
	public class CreateStandardLayoutAction : BarManagerControllerActionBase {
		#region static
		public static readonly DependencyProperty AllowTopDockContainerProperty;
		public static readonly DependencyProperty AllowLeftDockContainerProperty;
		public static readonly DependencyProperty AllowRightDockContainerProperty;
		public static readonly DependencyProperty AllowBottomDockContainerProperty;
		static CreateStandardLayoutAction() {
			AllowTopDockContainerProperty = DependencyPropertyManager.Register("AllowTopDockContainer", typeof(bool), typeof(CreateStandardLayoutAction), new FrameworkPropertyMetadata(true));
			AllowLeftDockContainerProperty = DependencyPropertyManager.Register("AllowLeftDockContainer", typeof(bool), typeof(CreateStandardLayoutAction), new FrameworkPropertyMetadata(true));
			AllowRightDockContainerProperty = DependencyPropertyManager.Register("AllowRightDockContainer", typeof(bool), typeof(CreateStandardLayoutAction), new FrameworkPropertyMetadata(true));
			AllowBottomDockContainerProperty = DependencyPropertyManager.Register("AllowBottomDockContainer", typeof(bool), typeof(CreateStandardLayoutAction), new FrameworkPropertyMetadata(true));
		}
		#endregion
		public CreateStandardLayoutAction() :this(null){ }
		public CreateStandardLayoutAction(BarManager manager) { internalManager = manager; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CreateStandardLayoutActionAllowTopDockContainer")]
#endif
		public bool AllowTopDockContainer {
			get { return (bool)GetValue(AllowTopDockContainerProperty); }
			set { SetValue(AllowTopDockContainerProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CreateStandardLayoutActionAllowBottomDockContainer")]
#endif
		public bool AllowBottomDockContainer {
			get { return (bool)GetValue(AllowBottomDockContainerProperty); }
			set { SetValue(AllowBottomDockContainerProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CreateStandardLayoutActionAllowLeftDockContainer")]
#endif
		public bool AllowLeftDockContainer {
			get { return (bool)GetValue(AllowLeftDockContainerProperty); }
			set { SetValue(AllowLeftDockContainerProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CreateStandardLayoutActionAllowRightDockContainer")]
#endif
		public bool AllowRightDockContainer {
			get { return (bool)GetValue(AllowRightDockContainerProperty); }
			set { SetValue(AllowRightDockContainerProperty, value); }
		}
		BarManager internalManager;
		internal void SetManager(BarManager manager) {
			internalManager = manager;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("CreateStandardLayoutActionManager")]
#endif
		public override BarManager Manager {
			get {
				if (internalManager != null)
					return internalManager;
				return CollectionAction.GetContext(this) as BarManager ?? CollectionAction.GetContext(this).With(BarManager.GetBarManager) ?? base.Manager;
			}
		}
		protected override void ExecuteCore(DependencyObject context) {
			base.ExecuteCore(context);
			if (!Manager.CreateStandardLayout && Manager.StandardContainers.Count > 0)
				return;
			if (AllowTopDockContainer) {
				BarContainerControl topControl = new BarContainerControl() { ContainerType = BarContainerType.Top };
				DockPanel.SetDock(topControl, Dock.Top);
				Manager.AddStandardContainer(topControl);
			}
			if (AllowBottomDockContainer) {
				BarContainerControl bottomControl = new BarContainerControl() { ContainerType = BarContainerType.Bottom };
				DockPanel.SetDock(bottomControl, Dock.Bottom);
				Manager.AddStandardContainer(bottomControl);
			}
			if (AllowLeftDockContainer) {
				BarContainerControl leftControl = new BarContainerControl() { ContainerType = BarContainerType.Left };
				leftControl.Orientation = Orientation.Vertical;
				DockPanel.SetDock(leftControl, Dock.Left);
				Manager.AddStandardContainer(leftControl);
			}
			if (AllowRightDockContainer) {
				BarContainerControl rightControl = new BarContainerControl() { ContainerType = BarContainerType.Right };
				rightControl.Orientation = Orientation.Vertical;
				DockPanel.SetDock(rightControl, Dock.Right);
				Manager.AddStandardContainer(rightControl);
			}
			Manager.InvalidateMeasure();
		}
		public override object GetObjectCore() {
			return null;
		}
		public override bool IsEqual(BarManagerControllerActionBase action) {
			CreateStandardLayoutAction act = action as CreateStandardLayoutAction;
			bool res = base.IsEqual(action);
			if (!res) return false;
			return act.AllowTopDockContainer == AllowTopDockContainer && act.AllowBottomDockContainer == AllowBottomDockContainer &&
				act.AllowLeftDockContainer == AllowLeftDockContainer && act.AllowRightDockContainer == AllowRightDockContainer;
		}
	}
	public class BarManagerActionCollection : ObservableCollection<IBarManagerControllerAction> {
		public BarManagerActionCollection(BarManagerActionContainer container) {
			Container = container;
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarManagerActionCollectionContainer")]
#endif
		public BarManagerActionContainer Container { get; private set; }
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.OldItems != null) {
				foreach (IControllerAction action in e.OldItems) {
					action.Do(((ILogicalChildrenContainer)Container).RemoveLogicalChild);
					action.Container = null;
				}
			}
			if (e.NewItems == null) return;
			foreach (IControllerAction action in e.NewItems) {
				action.Do(((ILogicalChildrenContainer)Container).AddLogicalChild);
				action.Container = Container.Controller;
			}
		}
		public virtual void Execute() {
			foreach (IBarManagerControllerAction action in this) {
				if (action == null)
					continue;
				action.Execute(null);
				(action as DependencyObject).Do(x => CollectionActionHelper.SetActionExecuted(x, true));
			}
		}
		protected override void ClearItems() {
			while (Items.Count > 0)
				RemoveAt(0);
		}
	}
	public class BarManagerLinksHolderController : BarManagerController {
		ILinksHolder holderCore;
		public BarManagerLinksHolderController()
			: base() {
		}
		public BarManagerLinksHolderController(ILinksHolder holder)
			: base() {
			Holder = holder;
		}		
		public ILinksHolder Holder {
			get { return holderCore; }
			set {
				if (Holder == value) return;
				if (Holder != null)
					Uninitialize(Holder);
				Owner = value as DependencyObject;
				holderCore = value;
				if (Holder != null)
					Initialize(Holder);
			}
		}
		public override void Execute() {
			OnBeforeExecute();
			base.Execute();
			OnAfterExecute();
		}
		protected override void SetUpActionContainer(BarManagerActionContainer container) { }
		protected virtual void OnBeforeExecute() {
			BarManagerHelper.SetLinksHolder(ActionContainer, Holder);
			foreach (var action in ActionContainer.Actions) {
				var dO = action as DependencyObject;
				if (dO == null)
					continue;
				BarManagerHelper.SetLinksHolder(dO, Holder);
			}
			var fe = Holder as FrameworkElement;
			object dC = null;
			var fce = Holder as FrameworkContentElement;
			dC = fce.With(x => x.DataContext);
			ActionContainer.DataContext = fe.With(x => x.DataContext) ?? dC;
		}
		protected virtual void OnAfterExecute() {
			BarManagerHelper.SetLinksHolder(ActionContainer, null);
			foreach (var action in ActionContainer.Actions) {
				var dO = action as DependencyObject;
				if (dO == null)
					continue;
				BarManagerHelper.SetLinksHolder(dO, null);
			}
		}
		protected virtual void Uninitialize(ILinksHolder holder) { }
		protected virtual void Initialize(ILinksHolder holder) { }
	}
	public class BarManagerMenuController : BarManagerLinksHolderController {
		public BarManagerMenuController() : base() {
		}
		public BarManagerMenuController(PopupMenu menu) : base(menu) {
		}		
		public PopupMenu Menu {
			get { return Holder as PopupMenu; }
			set { Holder = value; }
		}
	}
}
