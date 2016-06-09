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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.TestScripts;
using System.Threading;
using DevExpress.ExpressApp.Actions;
using System.ComponentModel;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using System.Web.UI.Design;
using System.Drawing;
using DevExpress.Persistent.Base;
using DevExpress.Web;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Templates.ActionContainers;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebActionContainer : SimpleActionContainer, IActionContainer, ITestableContainer{
		private ActionContainerHolder owner;
		private void OnActionRegistered(ActionBase action) {
			if(ActionRegistered != null) {
				ActionRegistered(this, new ActionEventArgs(action));
			}
		}
		private void owner_MenuItemsCreated(object sender, EventArgs e) {
			OnTestableControlsCreated();
		}
		protected void OnTestableControlsCreated() {
			if(TestableControlsCreated != null) {
				TestableControlsCreated(this, EventArgs.Empty);
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ActionContainerHolder Owner {
			get { return owner; }
			set {
				if(owner != value) {
					if(owner != null) {
						owner.MenuItemsCreated -= new EventHandler(owner_MenuItemsCreated);
					}
					owner = value;					
					if(owner != null) {
						owner.MenuItemsCreated += new EventHandler(owner_MenuItemsCreated);
					}
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public bool HasActiveActions {
			get {
				foreach(ActionBase action in Actions) {
					if(action.Active) {
						return true;
					}
				}
				return false;
			}
		}
		[PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultItemImageName { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultItemImageUrl { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultItemCaption { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public override string ContainerId {
			get { return base.ContainerId; }
			set { base.ContainerId = value; }
		}
		[PersistenceMode(PersistenceMode.Attribute)]
		public string DefaultActionID { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public bool IsDropDown { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public bool AutoChangeDefaultAction { get; set; }
		[PersistenceMode(PersistenceMode.Attribute)]
		public string DropDownMenuItemCssClass { get; set; }
		public override void Register(ActionBase action) {
			base.Register(action);
			OnActionRegistered(action);
		}
		public override void Dispose() {
			if(owner != null) {
				owner.MenuItemsCreated -= new EventHandler(owner_MenuItemsCreated);
			}
			base.Dispose();
		}
		#region ITestableContainer Members
		public ITestable[] GetTestableControls() {
			return Owner.GetTestableControls(this);
		}
		public event EventHandler TestableControlsCreated;
		#endregion
		public event EventHandler<ActionEventArgs> ActionRegistered;
		#region Obsolete 15.1
		private ActionItemPaintStyle paintStyle = ActionItemPaintStyle.Default;
		[Obsolete("Use the ActionBase.PaintStyle or the IModelAction.PaintStyle property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ActionItemPaintStyle PaintStyle {
			get { return paintStyle; }
			set { paintStyle = value; }
		}
		#endregion
	}
	public class ActionEventArgs : EventArgs {
		private ActionBase action;
		public ActionEventArgs(ActionBase action) {
			this.action = action;
		}
		public ActionBase Action {
			get {
				return action;
			}
		}
	}
}
