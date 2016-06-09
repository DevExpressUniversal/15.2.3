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
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates.Bars {
	public partial class DetailFormV2 : XtraForm, IActionControlsSite, IContextMenuHolder, IWindowTemplate, IBarManagerHolder, ISupportViewChanged, ISupportUpdate, IViewSiteTemplate, ISupportStoreSettings, IViewHolder {
		private static readonly object viewChanged = new object();
		private static readonly object settingsReloaded = new object();
		private StatusMessagesHelper statusMessagesHelper;
		protected virtual void RaiseViewChanged(View view) {
			EventHandler<TemplateViewChangedEventArgs> handler = (EventHandler<TemplateViewChangedEventArgs>)Events[viewChanged];
			if(handler != null) {
				handler(this, new TemplateViewChangedEventArgs(view));
			}
		}
		protected virtual void RaiseSettingsReloaded() {
			EventHandler handler = (EventHandler)Events[settingsReloaded];
			if(handler != null) {
				handler(this, EventArgs.Empty);
			}
		}
		public DetailFormV2() {
			InitializeComponent();
			barManager.ForceLinkCreate();
			statusMessagesHelper = new StatusMessagesHelper(barContainerStatusMessages);
		}
		#region IActionControlsSite Members
		IEnumerable<IActionControl> IActionControlsSite.ActionControls {
			get { return barManager.ActionControls; }
		}
		IEnumerable<IActionControlContainer> IActionControlsSite.ActionContainers {
			get { return barManager.ActionContainers; }
		}
		IActionControlContainer IActionControlsSite.DefaultContainer {
			get { return actionContainerView; }
		}
		#endregion
		#region IFrameTemplate Members
		void IFrameTemplate.SetView(View view) {
			viewSiteManager.SetView(view);
			RaiseViewChanged(view);
		}
		ICollection<IActionContainer> IFrameTemplate.GetContainers() {
			{ return new IActionContainer[0]; }
		}
		IActionContainer IFrameTemplate.DefaultContainer {
			get { return null; }
		}
		#endregion
		#region IWindowTemplate Members
		void IWindowTemplate.SetCaption(string caption) {
			Text = caption;
		}
		void IWindowTemplate.SetStatus(ICollection<string> statusMessages) {
			statusMessagesHelper.SetMessages(statusMessages);
		}
		bool IWindowTemplate.IsSizeable {
			get { return FormBorderStyle == FormBorderStyle.Sizable; }
			set { FormBorderStyle = (value ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog); }
		}
		#endregion
		#region IBarManagerHolder Members
		BarManager IBarManagerHolder.BarManager {
			get { return barManager; }
		}
		event EventHandler IBarManagerHolder.BarManagerChanged {
			add { }
			remove { }
		}
		#endregion
		#region IContextMenuHolder
		PopupMenu IContextMenuHolder.ContextMenu {
			get { return contextMenu; }
		}
		#endregion
		#region ISupportViewChanged Members
		event EventHandler<TemplateViewChangedEventArgs> ISupportViewChanged.ViewChanged {
			add { Events.AddHandler(viewChanged, value); }
			remove { Events.RemoveHandler(viewChanged, value); }
		}
		#endregion
		#region ISupportUpdate Members
		void ISupportUpdate.BeginUpdate() {
			barManager.BeginUpdate();
		}
		void ISupportUpdate.EndUpdate() {
			barManager.EndUpdate();
		}
		#endregion
		#region IViewSiteTemplate Members
		object IViewSiteTemplate.ViewSiteControl {
			get { return viewSiteManager.ViewSiteControl; }
		}
		#endregion
		#region ISupportStoreSettings Members
		void ISupportStoreSettings.SetSettings(IModelTemplate settings) {
			IModelTemplateWin templateModel = (IModelTemplateWin)settings;
			TemplatesHelper templatesHelper = new TemplatesHelper(templateModel);
			IModelFormState formState;
			if(viewSiteManager.View != null) {
				formState = templatesHelper.GetFormStateNode(viewSiteManager.View.Id);
			}
			else {
				formState = templatesHelper.GetFormStateNode();
			}
			formStateModelSynchronizer.Model = formState;
		}
		void ISupportStoreSettings.ReloadSettings() {
			modelSynchronizationManager.ApplyModel();
			RaiseSettingsReloaded();
		}
		void ISupportStoreSettings.SaveSettings() {
			SuspendLayout();
			try {
				modelSynchronizationManager.SynchronizeModel();
			}
			finally {
				ResumeLayout();
			}
		}
		event EventHandler ISupportStoreSettings.SettingsReloaded {
			add { Events.AddHandler(settingsReloaded, value); }
			remove { Events.RemoveHandler(settingsReloaded, value); }
		}
		#endregion
		#region IViewHolder Members
		View IViewHolder.View {
			get { return viewSiteManager.View; }
		}
		#endregion
	}
}
