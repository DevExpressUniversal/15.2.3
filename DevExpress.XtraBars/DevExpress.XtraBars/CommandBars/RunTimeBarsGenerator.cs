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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Commands.Design {
	#region RunTimeBarsGenerator (abstract class)
	public abstract class RunTimeBarsGenerator<TControl, TCommandId>
		where TControl : class, ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		readonly IComponent component;
		protected RunTimeBarsGenerator(IComponent component) {
			Guard.ArgumentNotNull(component, "component");
			this.component = component;
		}
		protected IComponent Component { get { return component; } }
		protected virtual object CreateTransaction(string name) {
			return null;
		}
		protected virtual void CommitTransaction(object transaction) {
		}
		protected virtual BarGenerationStrategy CreateGenerationStrategy() {
			return new RunTimeBarGenerationStrategy();
		}
		public virtual void AddNewBars(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			Component barContainer = GetBarContainer();
			if (barContainer == null)
				return;
			barName = String.Format("Add {0} Bar", barName);
			object transaction = CreateTransaction(barName);
			try {
				AddNewBarsCore(barContainer, creators, barName, insertMode);
			}
			finally {
				CommitTransaction(transaction);
			}
		}
		public virtual void AddNewBarsCore(ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			AddNewBarsCore(GetBarContainer(), creators, barName, insertMode);
		}
		void AddNewBarsCore(Component barContainer, ControlCommandBarCreator[] creators, string barName, BarInsertMode insertMode) {
			if (barContainer == null)
				return;
			BarGenerationStrategy generationStrategy = CreateGenerationStrategy();
			ControlCommandBarControllerBase<TControl, TCommandId> barController = EnsureBarController();
			BarGenerationManagerFactory<TControl, TCommandId> factory = CreateBarGenerationManagerFactory();
			for (int i = 0; i < creators.Length; i++) {
				BarGenerationManagerBase<TControl, TCommandId> barGenerationManager = factory.CreateBarGenerationManager(creators[i], barContainer, barController);
				barGenerationManager.InsertMode = insertMode;
				barGenerationManager.CreateBar(generationStrategy);
			}
			UpdateController(barController);
			PerformtInit(barContainer as ISupportInitialize);
		}
		protected virtual void UpdateController(ControlCommandBarControllerBase<TControl, TCommandId> barController) {
			PerformtInit(barController as ISupportInitialize);
		}
		protected virtual void PerformtInit(ISupportInitialize initializer) {
			if (initializer == null)
				return;
			initializer.BeginInit();
			initializer.EndInit();
		}
		public virtual void ClearExistingItems(ControlCommandBarCreator fakeBarCreator) {
			Component barContainer = GetBarContainer();
			if (barContainer == null)
				return;
			BarGenerationStrategy generationStrategy = CreateGenerationStrategy();
			object transaction = CreateTransaction("Delete all bar items");
			try {
				ControlCommandBarControllerBase<TControl, TCommandId> barController = EnsureBarController();
				BarGenerationManagerFactory<TControl, TCommandId> factory = CreateBarGenerationManagerFactory();
				BarGenerationManagerBase<TControl, TCommandId> barGenerationManager = factory.CreateBarGenerationManager(fakeBarCreator, barContainer, barController);
				barGenerationManager.ClearExistingItems(generationStrategy);
			}
			finally {
				CommitTransaction(transaction);
			}
		}
		public virtual bool HasRibbonControl() {
			IComponent designedComponent = Component;
			if (designedComponent == null)
				return false;
			List<RibbonControl> ribbonControlCollection = ComponentFinder.FindComponentsOfType<RibbonControl>(designedComponent.Site);
			return ribbonControlCollection.Count > 0;
		}
		public virtual Component GetBarContainer() {
			IComponent designedComponent = Component;
			if (designedComponent == null)
				return null;
			List<BarManager> barManagerCollection = ComponentFinder.FindComponentsOfType<BarManager>(designedComponent.Site);
			List<RibbonControl> ribbonControlCollection = ComponentFinder.FindComponentsOfType<RibbonControl>(designedComponent.Site);
			List<Component> supportedBarContainerCollection = new List<Component>();
			supportedBarContainerCollection.AddRange(barManagerCollection.ToArray());
			supportedBarContainerCollection.AddRange(ribbonControlCollection.ToArray());
			return ChooseBarContainer(supportedBarContainerCollection);
		}
		protected internal virtual Component ChooseBarContainer(List<Component> supportedBarContainerCollection) {
			if (supportedBarContainerCollection.Count < 1)
				return null;
			return supportedBarContainerCollection[0];
		}
		public virtual ControlCommandBarControllerBase<TControl, TCommandId> EnsureBarController() {
			ControlCommandBarControllerBase<TControl, TCommandId> controller = GetBarController();
			if (controller != null)
				return controller;
			controller = CreateBarController();
			controller.Control = Component as TControl;
			AddToContainer(controller);
			return controller;
		}
		protected ControlCommandBarControllerBase<TControl, TCommandId> GetBarController() {
			List<ControlCommandBarControllerBase<TControl, TCommandId>> controllers = ComponentFinder.FindComponentsOfType<ControlCommandBarControllerBase<TControl, TCommandId>>(Component.Site);
			return controllers.Find(IsControllerMatchToDesigningComponent);
		}
		protected bool IsControllerMatchToDesigningComponent(ControlCommandBarControllerBase<TControl, TCommandId> controller) {
			TControl control = controller.Control;
			if (control == null)
				return false;
			return controller.Control.Equals(Component);
		}
		protected virtual void AddToContainer(IComponent component) {
			ISite site = component.Site;
			if (site == null)
				return;
			IContainer container = site.Container;
			if (container == null)
				return;
			container.Add(component);
		}
		public void BeginAddNewBars() {
			BarManager barManager = GetBarManager();
			if (barManager != null)
				barManager.BeginUpdate();
		}
		public void EndAddNewBars() {
			BarManager barManager = GetBarManager();
			if (barManager != null)
				barManager.EndUpdate();
		}
		BarManager GetBarManager() {
			Component barContainer = GetBarContainer();
			if (barContainer == null)
				return null;
			BarManager barManager = barContainer as BarManager;
			if (barManager == null) {
				RibbonControl ribbon = barContainer as RibbonControl;
				if (ribbon != null)
					return ribbon.Manager;
			}
			return barManager;
		}
		public static bool IsExistBarContainer(IComponent component) {
			IComponent designedComponent = component;
			if (designedComponent == null)
				return false;
			List<BarManager> barManagerCollection = ComponentFinder.FindComponentsOfType<BarManager>(designedComponent.Site);
			List<RibbonControl> ribbonControlCollection = ComponentFinder.FindComponentsOfType<RibbonControl>(designedComponent.Site);
			return barManagerCollection.Count > 0 || ribbonControlCollection.Count > 0;
		}
		protected abstract BarGenerationManagerFactory<TControl, TCommandId> CreateBarGenerationManagerFactory();
		protected abstract ControlCommandBarControllerBase<TControl, TCommandId> CreateBarController();
	}
	#endregion
}
