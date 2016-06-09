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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class CustomizeActionControlEventArgs : EventArgs {
		private ActionBase action;
		private BarActionBaseItem actionControl;
		public CustomizeActionControlEventArgs(BarActionBaseItem actionControl, ActionBase action) {
			this.actionControl = actionControl;
			this.action = action;
		}
		public ActionBase Action {
			get { return action; }
			set { action = value; }
		}
		public BarActionBaseItem ActionControl {
			get { return actionControl; }
			set { actionControl = value; }
		}
	}
	public class ApplicationMenuItemComparer : IComparer<ActionContainerBarItem> {
		public int Compare(ActionContainerBarItem x, ActionContainerBarItem y) {
			if(x != null && y != null) {
				return x.ApplicationMenuIndex.CompareTo(y.ApplicationMenuIndex);
			}
			return 0;
		}
	}
	public class XafBarLinkContainerItem : BarLinkContainerExItem, ITargetRibbonElement {
		private string targetPageCaption;
		private string targetPageGroupCaption;
		private string targetPageCategoryCaption;
		private Color targetPageCategoryColor;
		protected override bool ShouldUpdateEditingLink { 
			get { return true; }
		}
		#region ITargetRibbonElement Members
		[DefaultValue(null), Category("Ribbon")]
		[Localizable(true)]
		public string TargetPageCaption {
			get { return targetPageCaption; }
			set { targetPageCaption = value; }
		}
		[DefaultValue(null), Category("Ribbon")]
		[Localizable(true)]
		public string TargetPageCategoryCaption {
			get { return targetPageCategoryCaption; }
			set { targetPageCategoryCaption = value; }
		}
		[DefaultValue(typeof(Color), "LightPink"), Category("Ribbon")]
		public Color TargetPageCategoryColor {
			get { return targetPageCategoryColor; }
			set { targetPageCategoryColor = value; }
		}
		[DefaultValue(""), Category("Ribbon")]
		[Localizable(true)]
		public string TargetPageGroupCaption {
			get { return targetPageGroupCaption; }
			set { targetPageGroupCaption = value; }
		}
		#endregion
	}
	public class ActionContainerBarItem : XafBarLinkContainerItem, IActionContainer {
		private string containerId;
		private List<ActionBase> actions;
		private BarActionItemsFactory barActionItemsFactory;
		private bool applicationMenuItem;
		private int applicationMenuIndex;
		private void action_HandleException(object sender, HandleExceptionEventArgs e) {
			if(!e.Handled) {
				Application.OnThreadException(e.Exception);
				e.Handled = true;
			}
		}
		private void barActionItemsFactory_BarItemChanged(object sender, BarItemChangedEventArgs e) {
			AddItem(e.ActionItem.Control);
		}
		private void barActionItemsFactory_ActionItemChanged(object sender, ActionItemChangedEventArgs e) {
			if(actions.Contains(e.Action)) {
				BarActionBaseItem item = GetItem(e.Action);
				AddItem(item.Control);
			}
		}
		private void UnlinkBarActionItemsFactory() {
			if(barActionItemsFactory != null) {
				barActionItemsFactory.BarItemChanged -= new EventHandler<BarItemChangedEventArgs>(barActionItemsFactory_BarItemChanged);
				barActionItemsFactory.ActionItemChanged -= new EventHandler<ActionItemChangedEventArgs>(barActionItemsFactory_ActionItemChanged);
			}
		}
		private BarActionBaseItem GetItem(ActionBase action) {
			BarActionBaseItem item = GetItemCore(action);
			CustomizeActionControlEventArgs args = new CustomizeActionControlEventArgs(item, action);
			OnCustomizeActionControl(args);
			return args.ActionControl;
		}
		protected virtual BarActionBaseItem GetItemCore(ActionBase action) {
			barActionItemsFactory.Mode = BarActionItemsFactoryMode.Default;
			return barActionItemsFactory.GetBarItem(action);
		}
		protected virtual void OnCustomizeActionControl(CustomizeActionControlEventArgs args) {
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			UnlinkBarActionItemsFactory();
			if(Manager != null) {
				barActionItemsFactory = BarActionItemsFactoryProvider.GetBarActionItemsFactory(Manager);
				barActionItemsFactory.BarItemChanged += new EventHandler<BarItemChangedEventArgs>(barActionItemsFactory_BarItemChanged);
				barActionItemsFactory.ActionItemChanged += new EventHandler<ActionItemChangedEventArgs>(barActionItemsFactory_ActionItemChanged);
			}
		}
		protected override void OnItemLinkCollectionChanged(object sender, CollectionChangeEventArgs e) {
			base.OnItemLinkCollectionChanged(sender, e);
			if(e.Action == CollectionChangeAction.Add) {
				((BarItemLink)e.Element).BeginGroup = true;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(actions != null) {
						foreach(ActionBase action in actions) {
							action.HandleException -= new EventHandler<HandleExceptionEventArgs>(action_HandleException);
						}
						actions.Clear();
						actions = null;
					}
				}
				UnlinkBarActionItemsFactory(); 
				barActionItemsFactory = null; 
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public ActionContainerBarItem() {
			actions = new List<ActionBase>();
		}
		public void Register(ActionBase action) {
			if(!actions.Contains(action)) {
				actions.Add(action);
				action.HandleException += new EventHandler<HandleExceptionEventArgs>(action_HandleException);
				BarActionBaseItem item = GetItem(action);
				AddItem(item.Control);
			}
		}
		public ActionBase FindActionByItem(BarItem barItem) {
			return barActionItemsFactory.FindActionByItem(barItem);
		}
		public override string ToString() {
			return ContainerId;
		}
		[Browsable(false)]
		public BarActionItemsFactory ActionItemsFactory {
			get { return barActionItemsFactory; }
		}
		[DefaultValue(null), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return containerId; }
			set { containerId = value; }
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ActionBase> Actions {
			get { return actions.AsReadOnly(); }
		}
		[DefaultValue(false), Category("Ribbon")]
		public bool ApplicationMenuItem {
			get { return applicationMenuItem; }
			set { applicationMenuItem = value; }
		}
		[DefaultValue(0), Category("Ribbon")]
		public int ApplicationMenuIndex {
			get { return applicationMenuIndex; }
			set { applicationMenuIndex = value; }
		}
	}
	public class ActionContainerMenuBarItem : ActionContainerBarItem {
		protected override BarActionBaseItem GetItemCore(ActionBase action) {
			ActionItemsFactory.Mode = BarActionItemsFactoryMode.Menu;
			return ActionItemsFactory.GetBarItem(action);
		}
	}
}
