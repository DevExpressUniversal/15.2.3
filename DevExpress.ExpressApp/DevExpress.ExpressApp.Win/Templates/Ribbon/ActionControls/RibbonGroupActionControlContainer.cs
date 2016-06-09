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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.Utils;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls {
	[DesignTimeVisible(false), DXToolboxItem(false)]
	[DebuggerDisplay("{ActionCategory,nq}")]
	public class RibbonGroupActionControlContainer : Component, IActionControlContainer, ISupportInitialize {
		private const bool DefaultHasEditorsClearButton = true;
		private string actionCategory;
		private bool hasEditorsClearButton = DefaultHasEditorsClearButton;
		private IDictionary<string, IActionControl> actionControlByActionId = new Dictionary<string, IActionControl>();
		private RibbonPageGroup ribbonGroup;
		private BarItemsFactory barItemsFactory;
		private int initializing;
		private bool isInitialized;
		protected void CheckCanSet(string propertyName) {
			if(!IsInitializing && !DesignMode) {
				string message = string.Format("Cannot assign the '{0}' value because property initialization must be done between 'BeginInit' and 'EndInit' calls.", propertyName);
				throw new InvalidOperationException(message);
			}
		}
		protected virtual void OnEndInit() {
			if(ActionCategory == null) {
				throw new InvalidOperationException("Cannot initialize the Action Container because its 'ActionCategory' property is null.");
			}
			if(RibbonGroup == null) {
				string message = string.Format("Cannot initialize the '{0}' Action Container because its 'RibbonGroup' property is null.", ActionCategory);
				throw new InvalidOperationException(message);
			}
			if(!DesignMode) {
				if(RibbonGroup.Ribbon == null) {
					string message = string.Format("Cannot initialize the '{0}' Action Container because its 'RibbonGroup.Ribbon' property is null.", ActionCategory);
					throw new InvalidOperationException(message);
				}
				barItemsFactory = new BarItemsFactory(RibbonGroup.Ribbon.Manager);
			}
		}
		public RibbonGroupActionControlContainer() { }
		public RibbonGroupActionControlContainer(string actionCategory, RibbonPageGroup container) {
			BeginInit();
			ActionCategory = actionCategory;
			RibbonGroup = container;
			EndInit();
		}
		public void BeginInit() {
			if(IsInitialized) {
				string message = String.Format("The 'BeginInit' method call is invalid for the '{0}' Action Container because it is already initialized.", ActionCategory);
				throw new InvalidOperationException(message);
			}
			initializing++;
		}
		public void EndInit() {
			initializing--;
			if(!IsInitializing) {
				OnEndInit();
				isInitialized = true;
			}
		}
		public IEnumerable<IActionControl> GetActionControls() {
			return actionControlByActionId.Values;
		}
		public IActionControl FindActionControl(string actionId) {
			return actionControlByActionId.ContainsKey(actionId) ? actionControlByActionId[actionId] : null;
		}
		public void AddActionControl(IActionControl actionControl) {
			Guard.ArgumentNotNull(actionControl, "actionControl");
			Guard.ArgumentIsNotNullOrEmpty(actionControl.ActionId, "actionControl.ActionId");
			actionControlByActionId.Add(actionControl.ActionId, actionControl);
		}
		public ISimpleActionControl AddSimpleActionControl(string actionId) {
			BarButtonItem barItem = barItemsFactory.CreateButtonItem();
			RibbonGroup.ItemLinks.Add(barItem);
			ISimpleActionControl actionControl = new BarButtonItemSimpleActionControl(actionId, barItem);
			AddActionControl(actionControl);
			return actionControl;
		}
		public ISingleChoiceActionControl AddSingleChoiceActionControl(string actionId, bool isHierarchical, SingleChoiceActionItemType itemType) {
			if(!isHierarchical && itemType == SingleChoiceActionItemType.ItemIsMode && !IsMenuMode) {
				return AddBarComboBoxItemSingleChoiceActionControl(actionId);
			}
			return AddBarButtonItemSingleChoiceActionControl(actionId, itemType);
		}
		public ISingleChoiceActionControl AddBarButtonItemSingleChoiceActionControl(string actionId, SingleChoiceActionItemType itemType) {
			BarButtonItem barItem = barItemsFactory.CreateButtonItem();
			RibbonGroup.ItemLinks.Add(barItem);
			BarButtonItemSingleChoiceActionControl actionControl = new BarButtonItemSingleChoiceActionControl();
			actionControl.BeginInit();
			actionControl.ActionId = actionId;
			actionControl.BarItem = barItem;
			actionControl.ItemType = itemType;
			actionControl.EndInit();
			AddActionControl(actionControl);
			return actionControl;
		}
		public ISingleChoiceActionControl AddBarComboBoxItemSingleChoiceActionControl(string actionId) {
			BarEditItem barItem = barItemsFactory.CreateImageComboBoxEditItem();
			RibbonGroup.ItemLinks.Add(barItem);
			ISingleChoiceActionControl actionControl = new BarComboBoxItemSingleChoiceActionControl(actionId, barItem);
			AddActionControl(actionControl);
			return actionControl;
		}
		public ISingleChoiceActionControl AddChooseSkinActionControl(string actionId) {
			RibbonGalleryBarItem barItem = new RibbonGalleryBarItem(RibbonGroup.Ribbon.Manager);
			RibbonGroup.ItemLinks.Add(barItem);
			ISingleChoiceActionControl actionControl = new RibbonChooseSkinActionControl(actionId, barItem);
			AddActionControl(actionControl);
			return actionControl;
		}
		public IParametrizedActionControl AddParametrizedActionControl(string actionId, Type valueType) {
			BarEditItem editItem = barItemsFactory.CreateEditItem(valueType, HasEditorsClearButton);
			RibbonGroup.ItemLinks.Add(editItem);
			BarEditItemParametrizedActionControl actionControl = new BarEditItemParametrizedActionControl(actionId, editItem);
			AddActionControl(actionControl);
			return actionControl;
		}
		protected bool IsInitializing {
			get { return initializing != 0; }
		}
		protected bool IsInitialized {
			get { return isInitialized; }
		}
		[Description("Specifies the Action Container's identifier.")]
		public string ActionCategory {
			get { return actionCategory; }
			set {
				if(ActionCategory != value) {
					CheckCanSet("ActionCategory");
					actionCategory = value;
				}
			}
		}
		[Browsable(false)]
		public RibbonPageGroup RibbonGroup {
			get { return ribbonGroup; }
			set {
				if(RibbonGroup != value) {
					CheckCanSet("RibbonGroup");
					ribbonGroup = value;
				}
			}
		}
		[Description("Specifies if the BarButtonItem or BarEditItem control with RepositoryItemImageComboBox will be created for Actions of the SingleChoiceAction type under certain conditions.")]
		[DefaultValue(false)]
		public bool IsMenuMode { get; set; }
		[Browsable(false)]
		[Description("Specifies if the clear button will be added to the container's editable controls.")]
		[DefaultValue(DefaultHasEditorsClearButton)]
		public bool HasEditorsClearButton {
			get { return hasEditorsClearButton; }
			set { hasEditorsClearButton = value; }
		}
	}
}
