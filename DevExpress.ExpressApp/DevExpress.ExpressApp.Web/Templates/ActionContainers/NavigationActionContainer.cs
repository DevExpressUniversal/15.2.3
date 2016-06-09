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
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Templates.ActionContainers;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Web.UI;
using System.Drawing;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public interface IWebNavigationControl : INavigationControl {
		Control Control { get; }
		Control TestControl { get; }
	}
	[ToolboxItem(false)] 
	public class NavigationActionContainer : ASPxPanel, IActionContainer, ISupportNavigationActionContainerTesting, ITestableEx, ISupportCallbackStartupScriptRegistering, ISupportAdditionalParametersTestControl {
		private IWebNavigationControl control;
		private string containerId;
		private SingleChoiceAction singleChoiceAction;
		private void SubscribeToCallbackStartupScriptRegistering() {
			ISupportCallbackStartupScriptRegistering startupScriptRegistering = control as ISupportCallbackStartupScriptRegistering;
			if(startupScriptRegistering != null) {
				startupScriptRegistering.RegisterCallbackStartupScript += new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegistering_RegisterCallbackStartupScript);
			}
		}
		private void UnsubscribeFromCallbackStartupScriptRegistering() {
			ISupportCallbackStartupScriptRegistering startupScriptRegistering = control as ISupportCallbackStartupScriptRegistering;
			if(startupScriptRegistering != null) {
				startupScriptRegistering.RegisterCallbackStartupScript -= new EventHandler<RegisterCallbackStartupScriptEventArgs>(startupScriptRegistering_RegisterCallbackStartupScript);
			}
		}
		private void startupScriptRegistering_RegisterCallbackStartupScript(object sender, RegisterCallbackStartupScriptEventArgs e) {
			OnRegisterCallbackStartupScript(e);
		}
		public static bool ShowImages { get; set; }
		protected virtual IWebNavigationControl CreateNavigationControl(NavigationStyle controlStyle) {
			switch(controlStyle) {
				case NavigationStyle.TreeList: return new TreeViewNavigationControl(!WebApplicationStyleManager.IsNewStyle || ShowImages);
				case NavigationStyle.NavBar: {
					NavBarNavigationControl control = new NavBarNavigationControl(!WebApplicationStyleManager.IsNewStyle || ShowImages);
						((DevExpress.Web.ASPxNavBar)control.Control).Width = this.Width;
						return control;
					}
				default: return null;
			}
		}
		protected virtual void OnRegisterCallbackStartupScript(RegisterCallbackStartupScriptEventArgs e) {
			if(RegisterCallbackStartupScript != null) {
				RegisterCallbackStartupScript(this, e);
			}
		}
		public NavigationActionContainer() {
			containerId = NavigationHelper.DefaultContainerId;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			WebActionContainerHelper.TryRegisterActionContainer(this, new IActionContainer[] { this });
		}
		public override void Dispose() {
			UnsubscribeFromCallbackStartupScriptRegistering();
			if(control != null) {
				control.Control.Unload -= new EventHandler(Control_Unload);
				if(control is IDisposable) {
					((IDisposable)control).Dispose();
				}
			}
			RegisterCallbackStartupScript = null;
			base.Dispose();
		}
		public void Register(DevExpress.ExpressApp.Actions.ActionBase action, NavigationStyle navigationStyle) {
			UnsubscribeFromCallbackStartupScriptRegistering();
			IDisposable disposable = control as IDisposable;
			if(disposable != null) {
				disposable.Dispose();
			}
			this.Controls.Clear();
			if(navigationStyle == NavigationStyle.NavBar) {
				this.CssClass += " NavBarLiteAC";
			}
			control = CreateNavigationControl(navigationStyle);
			control.Control.Unload += new EventHandler(Control_Unload); 
			SubscribeToCallbackStartupScriptRegistering();
			control.SetNavigationActionItems(((ChoiceActionBase)action).Items, (SingleChoiceAction)action);
			this.Controls.Add(control.Control);
			singleChoiceAction = action as SingleChoiceAction;
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
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
			get { return new ReadOnlyCollection<ActionBase>(new ActionBase[] { singleChoiceAction }); }
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
			get { return (Control)control.Control; }
		}
		INavigationControlTestable ISupportNavigationActionContainerTesting.NavigationControl {
			get { return (INavigationControlTestable)control; }
		}
		#endregion
		#region ISupportUpdate Members
		private ISupportUpdate tmpControl;
		public void BeginUpdate() {
			tmpControl = control as ISupportUpdate;
			if(tmpControl != null) {
				tmpControl.BeginUpdate();
			}
		}
		public void EndUpdate() {
			if(tmpControl != null) {
				tmpControl.EndUpdate();
			}
			tmpControl = null;
		}
		#endregion
		#region ITestable Members
		public string TestCaption {
			get { return singleChoiceAction != null ? singleChoiceAction.Caption : ""; }
		}
		public IJScriptTestControl TestControl {
			get {
				return null;
			}
		}
		public string ClientId {
			get { return ((WebControl)control.TestControl).ClientID; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Action;
			}
		}
		#endregion
		#region ISupportCallbackStartupScriptRegistering Members
		public event EventHandler<RegisterCallbackStartupScriptEventArgs> RegisterCallbackStartupScript;
		#endregion
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return GetType(); }
		}
		#endregion
		#region ISupportAdditionalParametersTestControl Members
		public ICollection<string> GetAdditionalParameters(object control) {
			ISupportAdditionalParametersTestControl additionalParametersTestControl = this.control as ISupportAdditionalParametersTestControl;
			if(additionalParametersTestControl != null) {
				return additionalParametersTestControl.GetAdditionalParameters(additionalParametersTestControl);
	}
			return new string[0];
}
		#endregion
#if DebugTest
		public IWebNavigationControl DebugTest_CreateNavigationControl(NavigationStyle controlStyle) {
			return CreateNavigationControl(controlStyle);
		}
#endif
	}
}
