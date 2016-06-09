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
using System.Web;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Templates {
	public interface ICurrentThemeManager {
		string GetCurrentTheme();
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface ITemplateContentHolder {
		TemplateContent TemplateContent { get; }
	}
	public interface IViewDependentControlsHolder {
		void Register(IViewDependentControl control);
	}
	public class CustomizeTemplateContentEventArgs : EventArgs {
		public CustomizeTemplateContentEventArgs(TemplateContent templateContent) {
			TemplateContent = templateContent;
		}
		public TemplateContent TemplateContent { get; private set; }
	}
	public class RequestHelper {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsCallback {
			get { return CallbackControlId != null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string CallbackControlId {
			get { return HttpContext.Current.Request.Form["__CALLBACKID"]; }
		}
	}
	public abstract class BaseXafPage : Page, IFrameTemplate, IWindowTemplate, IViewHolder, ISupportViewChanged, IDynamicContainersTemplate, IViewSiteTemplate, ICallbackManagerHolder, ITemplateContentHolder, IXafUpdatePanelsProvider, IViewDependentControlsHolder, IXafPopupWindowControlContainer, ISupportUpdate, IXafSecurityActionContainerHolder {
		public static ICurrentThemeManager CurrentThemeManager;
		private bool isSizeable;
		private ContextActionsMenu contextMenu;
		private ActionContainerCollection actionContainers;
		private View view;
		private XafCallbackManager callbackManager;
		private List<XafUpdatePanel> updatePanels;
		private List<IViewDependentControl> viewDependenControls;
		protected TemplateContent templateContent;
		private void SetLogoImage(TemplateContent templateContent) {
			string logoImageName = LogoImageName;
			if(logoImageName != "ExpressAppLogo") { 
				IHeaderImageControlContainer headerImageControlContainer = templateContent as IHeaderImageControlContainer;
				if(headerImageControlContainer != null) {
					headerImageControlContainer.HeaderImageControl.DefaultThemeImageLocation = "";
					headerImageControlContainer.HeaderImageControl.ImageName = logoImageName;
				}
			}
		}
		private void OnActionContainersChanged(ActionContainersChangedEventArgs args) {
			if(ActionContainersChanged != null) {
				ActionContainersChanged(this, args);
			}
		}
		protected virtual ContextActionsMenu CreateContextActionsMenu() {
			return new ContextActionsMenu(this, "ListView");
		}
		protected virtual XafCallbackManager CreateCallbackManager() {
			return new XafCallbackManager();
		}
		protected virtual void OnViewChanged(View view) {
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(view));
			}
		}
		protected override void InitializeCulture() {
			base.InitializeCulture();
			WebApplication.Instance.InitializeCulture();
		}
		protected override void OnPreInit(EventArgs e) {
			if(WebApplication.Instance != null && WebApplication.Instance.RequestManager != null) { 
				WebApplication.CurrentRequestTemplateType = WebApplication.Instance.RequestManager.GetTemplateType();
			}
			WebWindow.SetCurrentRequestPage(this);
			if(CurrentThemeManager != null) {
				string themeName = CurrentThemeManager.GetCurrentTheme();
				if(!string.IsNullOrEmpty(themeName)) {
					CurrentTheme = themeName;
				}
			}
			InitContent();
			base.OnPreInit(e);
		}
		protected virtual void InitContent() {
			TemplateType type = WebApplication.CurrentRequestTemplateType;
			if(WebApplication.Instance != null && !WebApplication.Instance.IsLoggedOn) {
				type = TemplateType.Logon;
			}
			templateContent = TemplateContentFactory.Instance.CreateTemplateContent(this, Settings, type);
			templateContent.ID = WebApplication.CurrentRequestTemplateType.ToString();
			InnerContentPlaceHolder.Controls.Add(templateContent);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			SetLogoImage(templateContent);
			if(CustomizeTemplateContent != null) {
				CustomizeTemplateContent(this, new CustomizeTemplateContentEventArgs(templateContent));
			}
			CreateControls();
			Form.Controls.Add(new StartupNavigationItemCallbackHandler());
			CallbackManager.CreateControls(this);
		}
		protected virtual void CreateControls() {
			if(WebApplication.Instance != null) {
				WebApplication.Instance.CreateControls(this);
			}
		}
		protected override void OnLoadComplete(EventArgs e) {
			base.OnLoadComplete(e);
			if(IsCallback) {
				string callbackControlID = RequestHelper.CallbackControlId;
				ICallbackEventHandler callbackControl = FindControl(callbackControlID) as ICallbackEventHandler; 
				if(callbackControl == null) {
					HttpContext.Current.Response.End();
				}
			}
		}
		protected virtual string LogoImageName {
			get { return WebApplication.Instance.Model.Application.Logo; }
		}
		public BaseXafPage() {
			actionContainers = new ActionContainerCollection();
			updatePanels = new List<XafUpdatePanel>();
			viewDependenControls = new List<IViewDependentControl>();
			contextMenu = CreateContextActionsMenu();
			RegisterActionContainers(contextMenu.Containers);
		}
		public static string CurrentTheme {
			get {
				if(ChooseThemeController.UseASPThemesMechanizm) {
					if(WebWindow.CurrentRequestPage != null) {
						return WebWindow.CurrentRequestPage.Theme;
					}
					else {
						return string.Empty;
					}
				}
				else {
					return string.IsNullOrEmpty(ASPxWebControl.GlobalTheme) ? ConfigurationSettings.Theme : ASPxWebControl.GlobalTheme;
				}
			}
			set {
				if(ChooseThemeController.UseASPThemesMechanizm && WebWindow.CurrentRequestPage != null) {
					WebWindow.CurrentRequestPage.Theme = value;
				}
				else {
					ASPxWebControl.GlobalTheme = value;
				}
			}
		}
		[Browsable(false)]
		public virtual string GetScrollableControlID() {
			return "";
		}
		public override void Dispose() {
			if(callbackManager != null) {
				callbackManager.Dispose();
				callbackManager = null;
			}
			contextMenu.Dispose();
			if(actionContainers != null) {
				foreach(IActionContainer container in actionContainers) {
					WebActionContainer webActionContainer = container as WebActionContainer;
					if(webActionContainer != null && webActionContainer.Owner != null) {
						webActionContainer.Owner.Dispose();
					}
				}
				actionContainers.Clear();
				actionContainers = null;
			}
			if(updatePanels != null) {
				updatePanels.Clear();
				updatePanels = null;
			}
			base.Dispose();
		}
		public virtual Control InnerContentPlaceHolder {
			get { return null; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TemplateContentSettings Settings {
			get {
				if(WebApplication.Instance != null) {
					return WebApplication.Instance.Settings;
				}
				throw new InvalidOperationException("The WebApplication.Instance is not initialized yet");
			}
		}
		public event EventHandler<CustomizeTemplateContentEventArgs> CustomizeTemplateContent;
		#region IFrameTemplate Members
		public ICollection<IActionContainer> GetContainers() {
			return actionContainers;
		}
		public void SetView(View view) {
			this.view = view;
			if(view != null) {
				contextMenu.CreateControls(view);
			}
			foreach(IViewDependentControl control in viewDependenControls) {
				control.SetView(view);
			}
			OnViewChanged(view);
		}
		public IActionContainer DefaultContainer {
			get {
				IActionContainer result = null;
				if(TemplateContent != null) {
					result = TemplateContent.DefaultContainer;
					if(result != null) {
						RegisterActionContainers(new IActionContainer[] { result });
					}
				}
				return result;
			}
		}
		#endregion
		#region IWindowTemplate Members
		public void SetCaption(string caption) {
			Title = caption;
			CallbackManager.SetPageCaption(caption);
		}
		public virtual void SetStatus(ICollection<string> statusMessages) { 
		}
		public bool IsSizeable {
			get { return isSizeable; }
			set { isSizeable = value; }
		}
		#endregion
		#region IViewHolder Members
		public View View {
			get { return view; }
		}
		#endregion
		#region ISupportViewChanged Members
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		#endregion
		#region IDynamicContainersTemplate Members
		public void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			IList<IActionContainer> addedContainers = this.actionContainers.TryAdd(actionContainers);
			if(addedContainers.Count > 0) {
				OnActionContainersChanged(new ActionContainersChangedEventArgs(addedContainers, ActionContainersChangedType.Added));
			}
		}
		public void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			IList<IActionContainer> removedContainers = new List<IActionContainer>();
			foreach(IActionContainer actionContainer in actionContainers) {
				if(this.actionContainers.Contains(actionContainer)) {
					this.actionContainers.Remove(actionContainer);
					removedContainers.Add(actionContainer);
				}
			}
			if(removedContainers.Count > 0) {
				OnActionContainersChanged(new ActionContainersChangedEventArgs(removedContainers, ActionContainersChangedType.Removed));
			}
		}
		public event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;
		#endregion
		#region ICallbackManagerHolder Members
		public XafCallbackManager CallbackManager {
			get {
				if(callbackManager == null) {
					callbackManager = CreateCallbackManager();
				}
				return callbackManager;
			}
		}
		#endregion
		#region ITemplateContentHolder Members
		public TemplateContent TemplateContent {
			get { return templateContent; }
		}
		#endregion
		#region IViewSiteTemplate Members
		public object ViewSiteControl {
			get {
				if(TemplateContent != null) {
					return TemplateContent.ViewSiteControl;
				}
				return null;
			}
		}
		#endregion
		#region IXafUpdatePanelsProvider Members
		public List<XafUpdatePanel> GetUpdatePanels() {
			return updatePanels;
		}
		public void RegisterPanel(XafUpdatePanel xafUpdatePanel) {
			if(!updatePanels.Contains(xafUpdatePanel)) {
				updatePanels.Add(xafUpdatePanel);
			}
		}
		public void UnregisterPanel(XafUpdatePanel xafUpdatePanel) {
			if(updatePanels != null) {
				updatePanels.Remove(xafUpdatePanel);
			}
		}
		#endregion
		#region IViewDependentControlsHolder Members
		public void Register(IViewDependentControl control) {
			viewDependenControls.Add(control);
		}
		#endregion
		#region IXafPopupWindowControlContainer Members
		public XafPopupWindowControl XafPopupWindowControl {
			get {
				IXafPopupWindowControlContainer popupWindowControlContainer = TemplateContent as IXafPopupWindowControlContainer;
				return popupWindowControlContainer != null ? popupWindowControlContainer.XafPopupWindowControl : null;
			}
		}
		#endregion
		public void BeginUpdate() {
			if(TemplateContent != null) {
				TemplateContent.BeginUpdate();
			}
		}
		public void EndUpdate() {
			if(TemplateContent != null) {
				TemplateContent.EndUpdate();
			}
		}
		#region IXafSecurityActionContainer Members
		public ActionContainerHolder SecurityActionContainerHolder {
			get {
				IXafSecurityActionContainerHolder actionContainerHolder = TemplateContent as IXafSecurityActionContainerHolder;
				return actionContainerHolder != null ? actionContainerHolder.SecurityActionContainerHolder : null;
			}
		}
		#endregion
#if DebugTest
		public void DebugTest_OnActionContainersChanged(ActionContainersChangedEventArgs args) {
			OnActionContainersChanged(args);
		}
#endif
	}
}
