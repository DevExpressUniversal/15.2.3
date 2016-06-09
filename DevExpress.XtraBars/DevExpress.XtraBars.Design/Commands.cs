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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Design.Forms;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	public class DesignTimeBarGenerationStrategy : BarGenerationStrategy {
		readonly IDesignerHost designerHost;
		IComponentChangeService changeService;
		public DesignTimeBarGenerationStrategy(IDesignerHost designerHost) {
			Guard.ArgumentNotNull(designerHost, "designerHost");
			this.designerHost = designerHost;
		}
		public IDesignerHost DesignerHost { get { return designerHost; } }
		public IComponentChangeService ChangeService {
			get {
				if (changeService == null)
					changeService = DesignerHost.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				return changeService;
			}
		}
		public override void AddToContainer(IComponent component) {
			DesignerHost.Container.Add(component);
		}
		public override void RemoveFromContainer(IComponent component) {
			DesignerHost.Container.Remove(component);
		}
		public override void OnComponentChanging(object component, MemberDescriptor member) {
			IComponentChangeService changeService = ChangeService;
			if (changeService != null)
				changeService.OnComponentChanging(component, member);
		}
		public override void OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue) {
			IComponentChangeService changeService = ChangeService;
			if (changeService != null)
				changeService.OnComponentChanged(component, member, oldValue, newValue);
		}
		public override IComponent CreateComponent(Type type) {
			return DesignerHost.CreateComponent(type);
		}
	}
	#region CommandBasedBarComponentBaseDesigner (abstract class)
	public abstract class CommandBasedBarComponentBaseDesigner : BaseComponentDesigner {
		#region Fields
		IComponentChangeService changeService;
		#endregion
		protected CommandBasedBarComponentBaseDesigner() {
		}
		#region Properties
		public CommandBasedBarComponentBase CommandBar { get { return Component as CommandBasedBarComponentBase; } }
		public Form ParentForm { get { return ParentComponent as Form; } }
		public IComponentChangeService ChangeService { get { return changeService; } set { changeService = value; } }
		#endregion
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			}
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
		}
		protected virtual void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (changeService != null) {
				changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
				changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
			}
		}
	}
	#endregion
	#region CommandBarComponentBaseDesigner
	public class CommandBarComponentBaseDesigner : CommandBasedBarComponentBaseDesigner {
		public new CommandBarComponentBase CommandBar { get { return base.CommandBar as CommandBarComponentBase; } }
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			CommandBar.BeginInit();
			try {
				CommandBar.BarManager = GetBarManager();
				InitializeNewCommandBar();
			}
			finally {
				CommandBar.EndInit();
			}
		}
		protected internal BarManager GetBarManager() {
			return ComponentFinder.FindComponentOfType<BarManager>(Component.Site);
		}
		protected override void OnComponentAdded(object sender, ComponentEventArgs e) {
			base.OnComponentAdded(sender, e);
			if (CommandBar.BarManager == null) {
				BarManager manager = e.Component as BarManager;
				if (manager != null)
					CommandBar.BarManager = manager;
			}
		}
		protected internal virtual void InitializeNewCommandBar() {
		}
	}
	#endregion
	#region RibbonCommandBarComponentBaseDesigner
	public class RibbonCommandBarComponentBaseDesigner : CommandBasedBarComponentBaseDesigner {
		public new RibbonCommandBarComponentBase CommandBar { get { return base.CommandBar as RibbonCommandBarComponentBase; } }
		public override void InitializeNewComponent(System.Collections.IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			CommandBar.BeginInit();
			try {
				CommandBar.RibbonControl = GetRibbonControl();
				InitializeNewCommandBar();
			}
			finally {
				CommandBar.EndInit();
			}
		}
		protected internal RibbonControl GetRibbonControl() {
			return ComponentFinder.FindComponentOfType<RibbonControl>(Component.Site);
		}
		protected override void OnComponentAdded(object sender, System.ComponentModel.Design.ComponentEventArgs e) {
			base.OnComponentAdded(sender, e);
			if (CommandBar.RibbonControl == null) {
				RibbonControl ribbon = e.Component as RibbonControl;
				if (ribbon != null)
					CommandBar.RibbonControl = ribbon;
			}
		}
		protected internal virtual void InitializeNewCommandBar() {
		}
	}
	#endregion
	#region ControlCommandBarControllerDesigner<TControl, TCommandId> (abstract class)
	public abstract class ControlCommandBarControllerDesigner<TControl, TCommandId> : BaseComponentDesigner
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		IComponentChangeService componentChangeService;
		IComponentChangeService ComponentChangeService {
			get {
				if (componentChangeService == null)
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				return componentChangeService;
			}
		}
		protected ControlCommandBarControllerBase<TControl, TCommandId> BarController { get { return (ControlCommandBarControllerBase<TControl, TCommandId>)Component; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if (ComponentChangeService != null) {
				ComponentChangeService.ComponentAdded += OnComponentAdded;
				ComponentChangeService.ComponentRemoved += OnComponentRemoved;
			}
		}
		protected virtual void OnComponentAdded(object sender, ComponentEventArgs e) {
			TControl addedControl = e.Component as TControl;
			if (addedControl != null) {
				if (BarController != null && BarController.Control == null) {
					bool isAlreadyAssigned = IsControlAlreadyAssigned(addedControl);
					if (!isAlreadyAssigned)
						BarController.Control = addedControl;
				}
			}
		}
		protected internal virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Control", typeof(TControl));
		}
		protected internal bool IsControlAlreadyAssigned(TControl addedControl) {
			List<ControlCommandBarControllerBase<TControl, TCommandId>> barControllerCollection = ComponentFinder.FindComponentsOfType<ControlCommandBarControllerBase<TControl, TCommandId>>(Component.Site);
			int count = barControllerCollection.Count;
			for (int i = 0; i < count; i++) {
				ControlCommandBarControllerBase<TControl, TCommandId> someController = barControllerCollection[i];
				if (someController.Control == addedControl) {
					return true;
				}
			}
			return false;
		}
	}
	#endregion
	#region DesignTimeBarsGenerator<TControl, TCommandId> (abstract class)
	public abstract class DesignTimeBarsGenerator<TControl, TCommandId> : RunTimeBarsGenerator<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		readonly IDesignerHost host;
		protected DesignTimeBarsGenerator(IDesignerHost host, IComponent component)
			: base(component) {
			Guard.ArgumentNotNull(host, "host");
			this.host = host;
		}
		IDesignerHost DesignerHost { get { return host; } }
		public override void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			base.AddNewBars(creators, barName, insertMode);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		public override void ClearExistingItems(ControlCommandBarCreator fakeBarCreator) {
			base.ClearExistingItems(fakeBarCreator);
			EditorContextHelperEx.RefreshSmartPanel(Component);
		}
		protected override BarGenerationStrategy CreateGenerationStrategy() {
			return new DesignTimeBarGenerationStrategy(DesignerHost);
		}
		protected override object CreateTransaction(string name) {
			return DesignerHost.CreateTransaction(name);
		}
		protected override void CommitTransaction(object transaction) {
			DesignerTransaction designerTransaction = transaction as DesignerTransaction;
			if (designerTransaction != null)
				designerTransaction.Commit();
			IDisposable disposable = transaction as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}
		protected override Component ChooseBarContainer(List<Component> supportedBarContainerCollection) {
			if (supportedBarContainerCollection.Count < 1)
				return null;
			else if (supportedBarContainerCollection.Count == 1)
				return supportedBarContainerCollection[0];
			else {
				SelectBarManagerForm form = new SelectBarManagerForm();
				form.BarContainerCollection.AddRange(supportedBarContainerCollection);
				form.ShowDialog();
				return form.SelectedContainer;
			}
		}
		public override ControlCommandBarControllerBase<TControl, TCommandId> EnsureBarController() {
			EnsureReferences(DesignerHost);
			return base.EnsureBarController();
		}
		protected override void AddToContainer(IComponent component) {
			DesignerHost.Container.Add(component);
		}
		protected abstract void EnsureReferences(IDesignerHost designerHost);
	}
	#endregion
}
