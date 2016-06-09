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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates {
	public class XtraFormTemplateBase : DevExpress.XtraBars.Ribbon.RibbonForm, IWindowTemplate, ISupportStoreSettings, ISupportUpdate, IBarManagerHolder, IViewSiteTemplate, ISupportViewChanged, IClassicToRibbonTransformerHolder, IDynamicContainersTemplate, IViewHolder {
		#region Designer
		private void InitializeComponent() {
			actionsContainersManager = new ActionContainersManager();
			actionsContainersManager.Template = this;
			modelSynchronizationManager = new ModelSynchronizationManager();
			viewSiteManager = new ViewSiteManager();
		}
		#endregion
		private TemplatesHelper templatesHelper;
		private IModelTemplateWin modelTemplate;
		private ClassicToRibbonTransformer ribbonTransformer;
		private RibbonFormStyle formStyle = RibbonFormStyle.Standard;
		private ICollection<BarItem> statusMessageItems;
		protected ActionContainersManager actionsContainersManager;
		protected ModelSynchronizationManager modelSynchronizationManager;
		protected ViewSiteManager viewSiteManager;
		protected XafBarManager barManager;
		private void SetRibbonCaption(string caption) {
			if(Ribbon != null) {
				Ribbon.ApplicationCaption = " ";
				Ribbon.ApplicationDocumentCaption = caption;
			}
			else {
				if(ribbonTransformer != null) {
					ribbonTransformer.Transformed += new EventHandler<EventArgs>(ribbonTransformer_Transformed);
				}
			}
		}
		private void ribbonTransformer_Transformed(object sender, EventArgs e) {
			((ClassicToRibbonTransformer)sender).Transformed -= new EventHandler<EventArgs>(ribbonTransformer_Transformed);
			SetRibbonCaption(Text);
		}
		protected void CheckTransformToRibbon(bool mergeRibbon) {
			if(FormStyle == RibbonFormStyle.Ribbon && ribbonTransformer != null) {
				try {
					SuspendLayout();
					ribbonTransformer.Transform(mergeRibbon);
					OnBarMangerChanged();
				}
				finally {
					ResumeLayout();
				}
			}
		}
		protected virtual void OnBarMangerChanged() {
			if(BarManagerChanged != null) {
				BarManagerChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void AddStatusItems(ICollection<string> statusMessages, BarItemLinkCollection targetItemLinks) {
			foreach(BarItem barItem in statusMessageItems) {
				barItem.Dispose();
			}
			statusMessageItems.Clear();
			int i = 0;
			foreach(string message in statusMessages) {
				BarStaticItem item = new BarStaticItem();
				statusMessageItems.Add(item); 
				item.Name = "StatusBarItem" + i++; 
				item.Visibility = BarItemVisibility.OnlyInRuntime;
				item.AutoSize = BarStaticItemSize.Content;
				item.Caption = message;
				item.MergeType = BarMenuMerge.Replace;
				item.Tag = EasyTestTagHelper.FormatTestField(message.Split(':')[0]);
				targetItemLinks.Add(item);
			}
		}
		protected void EnsureSkinPainter() {
			if(FormStyle == RibbonFormStyle.Ribbon && FormPainter == null) {
				CheckUpdateSkinPainter();
			}
		}
		protected void EnsureStatusBar() { 
			ISupportClassicToRibbonTransform transformable = this as ISupportClassicToRibbonTransform;
			bool hasStatusBar = false;
			if(transformable != null) {
				foreach(Bar bar in transformable.BarManager.Bars) {
					if(bar.IsStatusBar) {
						hasStatusBar = true;
					}
				}
			}
			if(hasStatusBar && StatusBar == null) {
				RibbonStatusBar statusBar = new RibbonStatusBar();
				statusBar.Visible = Visible;
				Controls.Add(statusBar);
			}
		}
		protected override void WndProc(ref Message msg) {
			if(msg.Msg == 0x10 || msg.Msg == 0x11 || msg.Msg == 0x16) {
				try {
					base.WndProc(ref msg);
				}
				catch(Exception exception) {
					HandleExceptionEventArgs handleExceptionEventArgs = new HandleExceptionEventArgs(exception);
					if(CustomHandleExceptionOnClosing != null) {
						CustomHandleExceptionOnClosing(this, handleExceptionEventArgs);
					}
					if(!handleExceptionEventArgs.Handled) {
						throw;
					}
					if(msg.Msg != 0x11) {
						try {
							Dispose();
						}
						catch(Exception) { }
					}
				}
			}
			else {
				base.WndProc(ref msg);
			}
		}
		public override RibbonStatusBar StatusBar {
			get { return base.StatusBar; }
			set {
				if(base.StatusBar != value && base.StatusBar != null && value != null) { 
					base.StatusBar.Visible = value.Visible;
				}
				base.StatusBar = value;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(modelSynchronizationManager != null) {
						modelSynchronizationManager.Dispose();
						modelSynchronizationManager = null;
					}
					if(actionsContainersManager != null) {
						actionsContainersManager.Dispose();
						actionsContainersManager = null;
					}
					if(barManager != null) {
						barManager.Dispose();
						barManager = null;
					}
					if(viewSiteManager != null) {
						viewSiteManager.Dispose();
						viewSiteManager = null;
					}
					if(ribbonTransformer != null) {
						ribbonTransformer.Dispose();
						ribbonTransformer = null;
					}
					statusMessageItems.Clear();
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override FormShowMode ShowMode {
			get { return FormShowMode.AfterInitialization; }
		}
		protected override RibbonFormStyle RibbonFormStyle {
			get { return formStyle; }
		}
		public XtraFormTemplateBase() {
			if(this is ISupportClassicToRibbonTransform) {
				ribbonTransformer = new ClassicToRibbonTransformer(this);
			}
			statusMessageItems = new List<BarItem>(); 
			InitializeComponent();
		}
		[Browsable(false)]
		public IModelTemplateWin ModelTemplate {
			get { return modelTemplate; }
		}
		public BarManager BarManager {
			get {
				if(Ribbon != null) {
					return Ribbon.Manager;
				}
				return barManager;
			}
			set {
				barManager = (XafBarManager)value;
				if(barManager != null) {
					EnsureStatusBar(); 
				}
			}
		}
		[Browsable(false)]
		public Object ViewSiteControl {
			get { return viewSiteManager.ViewSiteControl; }
		}
		[Browsable(false)]
		public ActionContainersManager ActionContainersManager {
			get { return actionsContainersManager; }
		}
		[Browsable(false)]
		public View View {
			get { return viewSiteManager.View; }
		}
		[Browsable(false)]
		public ClassicToRibbonTransformer RibbonTransformer {
			get { return ribbonTransformer; }
			set { ribbonTransformer = value; }
		}
		#region ISupportUpdate Members
		void ISupportUpdate.BeginUpdate() {
			if(barManager != null) {
				barManager.BeginUpdate();
				foreach(BarItem item in barManager.Items) {
					MainMenuItem menuItem = item as MainMenuItem;
					if(menuItem != null) {
						menuItem.BeginUpdate();
					}
				}
			}
		}
		void ISupportUpdate.EndUpdate() {
			if(barManager != null) {
				foreach(BarItem item in barManager.Items) {
					MainMenuItem menuItem = item as MainMenuItem;
					if(menuItem != null) {
						menuItem.EndUpdate();
					}
				}
				barManager.EndUpdate();
			}
		}
		#endregion
		#region IFrameTemplate Members
		public virtual void SetView(View view) {
			viewSiteManager.SetView(view);
			if(view != null) {
				Name = Guid.NewGuid().ToString();
				SetFormIcon(view);
			}
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(view));
			}
		}
		protected virtual void SetFormIcon(View view) {
			if(view != null && view.Model != null) {
				NativeMethods.SetFormIcon(this,
					ImageLoader.Instance.GetImageInfo(ViewImageNameHelper.GetImageName(view)).Image,
					ImageLoader.Instance.GetLargeImageInfo(ViewImageNameHelper.GetImageName(view)).Image);
			}
		}
		public virtual void SetSettings(IModelTemplate modelTemplate) {
			this.modelTemplate = (IModelTemplateWin)modelTemplate;
			templatesHelper = new TemplatesHelper(this.modelTemplate);
			if(modelTemplate != null) {
				if(barManager != null) {
					barManager.Model = templatesHelper.GetBarsCustomizationNode();
				}
			}
		}
		private void OnSettingsReloaded() {
			if(SettingsReloaded != null) {
				SettingsReloaded(this, EventArgs.Empty);
			}
		}
		protected virtual IModelFormState GetFormStateNode() {
			return templatesHelper.GetFormStateNode();
		}
		protected virtual void ReloadSettingsCore() {
			modelSynchronizationManager.ApplyModel();
		}
		public void ReloadSettings() {
			ReloadSettingsCore();
			OnSettingsReloaded();
		}
		public event EventHandler SettingsReloaded;
		public virtual void SaveSettings() {
			SuspendLayout();
			try {
				modelSynchronizationManager.SynchronizeModel();
			}
			finally {
				ResumeLayout();
			}
		}
		public ICollection<IActionContainer> GetContainers() {
			return actionsContainersManager.GetContainers();
		}
		public TemplatesHelper TemplatesHelper {
			get { return templatesHelper; }
		}
		[Browsable(false)]
		public IActionContainer DefaultContainer {
			get { return actionsContainersManager.DefaultContainer; }
		}
		#endregion
		#region IWindowTemplate Members
		void IWindowTemplate.SetStatus(ICollection<string> statusMessages) {
			if(Ribbon != null && Ribbon.StatusBar != null) {
				AddStatusItems(statusMessages, Ribbon.StatusBar.ItemLinks);
			}
			else {
				if(barManager != null) {
					Bar statusBar = barManager.StatusBar;
					if(statusBar != null) {
						AddStatusItems(statusMessages, statusBar.ItemLinks);
					}
				}
			}
		}
		void IWindowTemplate.SetCaption(string caption) {
			Text = caption;
			if(FormStyle == RibbonFormStyle.Ribbon ) {
				SetRibbonCaption(caption);
			}
		}
		#endregion
		public bool IsSizeable {
			get { return FormBorderStyle == FormBorderStyle.Sizable; }
			set { FormBorderStyle = (value ? FormBorderStyle.Sizable : FormBorderStyle.FixedDialog); }
		}
		internal event EventHandler<HandleExceptionEventArgs> CustomHandleExceptionOnClosing;
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		public event EventHandler BarManagerChanged;
		#region ISupportClassicToRibbonTransform Members
		[Browsable(false)]
		public RibbonFormStyle FormStyle {
			get { return formStyle; }
			set {
				formStyle = value;
				EnsureSkinPainter();
			}
		}
		[Browsable(false)]
		public IModelOptionsRibbon RibbonOptions {
			get { return modelTemplate != null ? ((IModelOptionsWin)modelTemplate.Application.Options).RibbonOptions : null; }
		}
		#endregion
		#region IDynamicContainersTemplate Members
		protected virtual void OnActionContainersChanged(ActionContainersChangedEventArgs e) {
			if(ActionContainersChanged != null) {
				ActionContainersChanged(this, e);
			}
		}
		public void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			actionsContainersManager.ActionContainerComponents.AddRange(actionContainers);
			OnActionContainersChanged(new ActionContainersChangedEventArgs(actionContainers, ActionContainersChangedType.Added));
		}
		public void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			foreach(IActionContainer actionContainer in actionContainers) {
				actionsContainersManager.ActionContainerComponents.Remove(actionContainer);
			}
			OnActionContainersChanged(new ActionContainersChangedEventArgs(actionContainers, ActionContainersChangedType.Removed));
		}
		public event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;
		#endregion
	}
}
