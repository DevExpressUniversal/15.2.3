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
using System.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public abstract class ASPxObjectPropertyEditorBase : ASPxPropertyEditor, IActionSource, IComplexViewItem, IXafCallbackHandler {
#if DebugTest
		public const string ObjectIDKey = "ObjectID";
#else
		private const string ObjectIDKey = "ObjectID";
#endif
		private IList<ActionBase> actions = new List<ActionBase>();
		private SimpleAction navigateToObjectAction;
		private LinkButton viewModeButton;
		private bool isViewModeButtonLoaded = false;
		protected WebApplication application;
		protected IObjectSpace objectSpace;
		private TargetWindow navigateToObjectTargetWindow = TargetWindow.Default;
		private static bool showLink = true;
		public SimpleAction NavigateToObjectAction {
			get { return navigateToObjectAction; }
		}
		private void SetButtonIDAttribute(LinkButton button) {
			button.Attributes["id"] = button.ClientID;
		}
		private void viewModeEditor_PreRender(object sender, EventArgs e) {
			LinkButton button = (LinkButton)sender;
			button.PreRender -= new EventHandler(viewModeEditor_PreRender);
			SetButtonIDAttribute(button);
		}
		private void viewModeButton_Load(object sender, EventArgs e) {
			LinkButton link = (LinkButton)sender;
			link.Load -= new EventHandler(viewModeButton_Load);
			ICallbackManagerHolder holder = link.Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(link.UniqueID, this);
				SetCallbackScript(link, holder);
			}
			isViewModeButtonLoaded = true;
		}
		private void viewModeButton_Unload(object sender, EventArgs e) {
			LinkButton button = (LinkButton)sender;
			button.Unload -= new EventHandler(viewModeButton_Unload);
			SetButtonIDAttribute(button);		
		}
#if DebugTest
		public static void SetCallbackScript(LinkButton linkButton, ICallbackManagerHolder holder) {
#else
		private static void SetCallbackScript(LinkButton linkButton, ICallbackManagerHolder holder) {
#endif
			if(holder != null && linkButton.Attributes[ObjectIDKey] != null) {
				string callbackParams = linkButton.Attributes[ObjectIDKey];
				linkButton.OnClientClick = string.Format(@"{0} window.disableNotifyWindowClose = true;
                                                               event.cancelBubble = true; return false;", holder.CallbackManager.GetScript(linkButton.UniqueID, string.Format("'{0}'", callbackParams)));
				linkButton.Attributes.Remove(ObjectIDKey);
			}
		}
		private void navigateToObjectAction_OnExecute(Object sender, SimpleActionExecuteEventArgs args) {
			NavigateToObject(args.ShowViewParameters);
		}
		protected void DisposeAction(ActionBase action) {
			try {
				action.Dispose();
			}
			catch(Exception e) {
				Tracing.Tracer.LogSubSeparator("Exception occurs on disposing an action");
				Tracing.Tracer.LogError(e);
			}
		}
		protected virtual void NavigateToObject(ShowViewParameters showViewParameters) {
			Object selectedObject = navigateToObjectAction.SelectionContext.SelectedObjects[0];
			IObjectSpace objectSpace = application.CreateObjectSpace((selectedObject != null) ? selectedObject.GetType() : null);
			Object obj = objectSpace.GetObject(selectedObject);
			Tracing.Tracer.LogValue("NavigateToObject", obj);
			if(obj != null) {
				showViewParameters.CreatedView = application.CreateDetailView(objectSpace, obj);
			}
			showViewParameters.TargetWindow = NavigateToObjectTargetWindow;
		}
		protected override WebControl CreateViewModeControlCore() {
			if(CanShowLink()) {
				viewModeButton = new LinkButton();
				viewModeButton.Load += new EventHandler(viewModeButton_Load);
				viewModeButton.PreRender += new EventHandler(viewModeEditor_PreRender);
				viewModeButton.Unload += new EventHandler(viewModeButton_Unload);
				isViewModeButtonLoaded = false;
			}
			else {
				return base.CreateViewModeControlCore();
			}
			return viewModeButton;
		}
		protected virtual bool CanShowLink() {
			return ShowLink && MemberInfo.MemberTypeInfo.IsPersistent;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(navigateToObjectAction != null) {
						navigateToObjectAction.Execute -= new SimpleActionExecuteEventHandler(navigateToObjectAction_OnExecute);
						DisposeAction(navigateToObjectAction);
						navigateToObjectAction = null;
					}
					if(viewModeButton != null) {
						viewModeButton.Dispose();
						viewModeButton = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void ReadViewModeValueCore() {
			if(InplaceViewModeEditor != null) {
				if(CanShowLink()) {
					string propertyDisplayValue = GetPropertyDisplayValue();
					LinkButton control = ((LinkButton)InplaceViewModeEditor);
					if(propertyDisplayValue == string.Empty || propertyDisplayValue == CaptionHelper.DefaultNullValueText) {
						control.CssClass = "emptyLink";
					}
					control.Text = HttpUtility.HtmlEncode(propertyDisplayValue);
					control.Enabled = (GetControlValueCore() != null);
					control.Attributes.Remove(ObjectIDKey);
					if(PropertyValue != null) {
						control.Attributes[ObjectIDKey] = objectSpace.GetKeyValueAsString(PropertyValue);
					}
					if(isViewModeButtonLoaded) {
						SetCallbackScript(control, control.Page as ICallbackManagerHolder);
					}
				}
				else {
					base.ReadViewModeValueCore();
				}
			}
		}
		public ASPxObjectPropertyEditorBase(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			navigateToObjectAction = new SimpleAction(null, PropertyName + "_NavigateToObject_" + Guid.NewGuid(), PropertyName + "_Edit");
			navigateToObjectAction.Execute += new SimpleActionExecuteEventHandler(navigateToObjectAction_OnExecute);
			actions.Add(navigateToObjectAction);
			skipEditModeDataBind = true;
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(viewModeButton != null) {
				viewModeButton.PreRender -= new EventHandler(viewModeEditor_PreRender);
				viewModeButton.Load -= new EventHandler(viewModeButton_Load);
			}
			if(!unwireEventsOnly) {
				viewModeButton = null;
			}
			isViewModeButtonLoaded = false;
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public virtual void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
			this.application = application as WebApplication;
		}
		public IList<ActionBase> Actions {
			get { return actions; }
		}
		public TargetWindow NavigateToObjectTargetWindow {
			get { return navigateToObjectTargetWindow; }
			set { navigateToObjectTargetWindow = value; }
		}
		public static bool ShowLink {
			get { return showLink; }
			set { showLink = value; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
		#region IXafCallbackHandler Members
		void IXafCallbackHandler.ProcessAction(string parameter) {
			object objectKey = objectSpace.GetObjectKey(MemberInfo.MemberType, parameter);
			if(objectKey != null) {
				object obj = objectSpace.GetObjectByKey(MemberInfo.MemberType, objectKey);
				if(obj != null) {
					TemporarySelectionContext context = new TemporarySelectionContext(obj);
					try {
						navigateToObjectAction.SelectionContext = context;
						navigateToObjectAction.DoExecute();
					}
					finally {
						context.Dispose();
					}
				}
			}
		}
		#endregion
	}
}
