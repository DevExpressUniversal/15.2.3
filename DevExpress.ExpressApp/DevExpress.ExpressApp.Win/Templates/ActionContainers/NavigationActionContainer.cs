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
using System.Text;
using DevExpress.ExpressApp.Templates;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Templates.ActionContainers;
using System.Drawing;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Core;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	[ToolboxItem(false)] 
	[ToolboxBitmap(typeof(WinApplication), "Resources.ActionContainers.Toolbox_ActionContainer_NavigationBar.bmp")]
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafActionContainers)]
	public class NavigationActionContainer : Panel, IActionContainer, ISupportNavigationActionContainerTesting, IModelSynchronizable {
		private INavigationControl control;
		private List<ActionBase> actions;
		private string containerId;
		private SingleChoiceAction singleChoiceAction;
		private IModelTemplateNavBarCustomization model;
		private INavigationControl CreateNavigationControl(NavigationStyle navigationStyle) {
			switch(navigationStyle) {
				case NavigationStyle.TreeList: return new TreeListNavigationControl();
				case NavigationStyle.NavBar: return new NavBarNavigationControl();
				default: return null;
			}
		}
		protected virtual void OnNavigationControlCreated() {
			if(NavigationControlCreated != null) {
				NavigationControlCreated(this, EventArgs.Empty);
			}
		}
		public void Register(DevExpress.ExpressApp.Actions.ActionBase action, NavigationStyle navigationStyle) {
			IDisposable disposable = control as IDisposable;
			if(disposable != null) {
				disposable.Dispose();
			}
			this.Controls.Clear();
			control = CreateNavigationControl(navigationStyle);
			OnNavigationControlCreated();
			control.SetNavigationActionItems(((ChoiceActionBase)action).Items, (SingleChoiceAction)action);
			this.Controls.Add((Control)control);
			singleChoiceAction = action as SingleChoiceAction;
		}
		public NavigationActionContainer() {
			containerId = NavigationHelper.DefaultContainerId;
			Dock = DockStyle.Fill;
			actions = new List<ActionBase>();
		}
		public event EventHandler<EventArgs> NavigationControlCreated;
		#region IActionContainer Members
		[DefaultValue(NavigationHelper.DefaultContainerId), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return containerId; }
			set { containerId = value; }
		}
		public void Register(DevExpress.ExpressApp.Actions.ActionBase action) {
			NavigationStyle navigationStyle = NavigationStyle.NavBar;
			if(action.Application != null) {
				navigationStyle = NavigationHelper.GetControlStyle(action.Application.Model);
			}
			Register(action, navigationStyle);
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ActionBase> Actions {
			get {
				actions.Clear();
				if(singleChoiceAction != null) {
					actions.Add(singleChoiceAction);
				}
				return actions.AsReadOnly();
			}
		}
		#endregion
		#region ISupportNavigationActionContainerTesting Members
		bool ISupportNavigationActionContainerTesting.IsItemControlVisible(ChoiceActionItem item) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemVisible(item);
		}
		int ISupportNavigationActionContainerTesting.GetGroupCount() {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetGroupCount();
		}
		string ISupportNavigationActionContainerTesting.GetGroupControlCaption(ChoiceActionItem groupItem) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetItemCaption(groupItem);
		}
		int ISupportNavigationActionContainerTesting.GetGroupChildControlCount(ChoiceActionItem groupItem) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetSubItemsCount(groupItem);
		}
		string ISupportNavigationActionContainerTesting.GetChildControlCaption(ChoiceActionItem item) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetItemCaption(item);
		}
		bool ISupportNavigationActionContainerTesting.GetChildControlEnabled(ChoiceActionItem item) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemEnabled(item);
		}
		bool ISupportNavigationActionContainerTesting.GetChildControlVisible(ChoiceActionItem item) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsItemVisible(item);
		}
		bool ISupportNavigationActionContainerTesting.IsGroupExpanded(ChoiceActionItem item) {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.IsGroupExpanded(item);
		}
		string ISupportNavigationActionContainerTesting.GetSelectedItemCaption() {
			return ((ISupportNavigationActionContainerTesting)this).NavigationControl.GetSelectedItemCaption();
		}
		public Control NavigationControl {
			get { return (Control)control; }
		}
		INavigationControlTestable ISupportNavigationActionContainerTesting.NavigationControl {
			get { return (INavigationControlTestable)control; }
		}
		#endregion
		#region ISupportUpdate Members
		private INavigationControl tmpControl;
		public void BeginUpdate() {
			tmpControl = control;
			if(control != null) {
				((ISupportUpdate)control).BeginUpdate();
			}
		}
		public void EndUpdate() {
			if(control != null && tmpControl == control) {
				((ISupportUpdate)control).EndUpdate();
			}
			tmpControl = null;
		}
		#endregion
		public IModelTemplateNavBarCustomization Model {
			get {
				return model;
			}
			set {
				model = value;
			}
		}
		#region IModelSynchronizable Members
		public void ApplyModel() {
			if(model != null) {
				new XtraNavBarModelSynchronizer(this, model).ApplyModel();
			}
		}
		public void SynchronizeModel() {
			if(model != null) {
				new XtraNavBarModelSynchronizer(this, model).SynchronizeModel();
			}
		}
		#endregion
	}
}
