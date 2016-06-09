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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors {
	#region Obsolete 15.2
	[Obsolete("Use StaticTextViewItem instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class StaticTextDetailItem : StaticTextViewItem {
		public StaticTextDetailItem(IModelStaticText model, Type objectType)
			: base(objectType, model) {
		}
	}
	#endregion
	public class StaticTextViewItem : StaticText, ITestable {
		private void ApplyAlignmentFromModel() {
			StaticAlignHelper.ApplyAlignment(Control, Model.HorizontalAlign, Model.VerticalAlign);
		}
		private void Control_PreRender(object sender, EventArgs e) {
			ApplyAlignmentFromModel();
		}
		private void UnsubscribeFromEvents() {
			if(Control != null) {
				Control.PreRender -= new EventHandler(Control_PreRender);
				Control.Unload -= Control_Unload;
				if(WebWindow.CurrentRequestPage != null) {
					Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
					((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
				}
			}
		}
		protected override object CreateControlCore() {
			Label result = new Label();
			result.CssClass = "StaticText";
			result.Text = !String.IsNullOrEmpty(Text) ? Text.Replace("\r\n", "<br/>") : String.Empty;
			result.Attributes[EasyTestTagHelper.TestField] = System.Web.HttpUtility.HtmlEncode(Model.Id);
			return result;
		}
		protected override void OnTextChanged(string text) {
			base.OnTextChanged(text);
			if(Control != null) {
				Control.Text = text.Replace("\r\n", "<br/>");
			}
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			Control.PreRender += new EventHandler(Control_PreRender);
			if(WebWindow.CurrentRequestPage != null) {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
				((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
			Control.Unload += new EventHandler(Control_Unload);
		}
		void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			ApplyAlignmentFromModel();
		}
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized(sender as Control);
			if(WebWindow.CurrentRequestPage != null) {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), WebWindow.CurrentRequestPage.GetType(), "Page");
				((ICallbackManagerHolder)WebWindow.CurrentRequestPage).CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			UnsubscribeFromEvents();
			base.BreakLinksToControl(unwireEventsOnly);
		}
		protected override void Dispose(bool disposing) {
			UnsubscribeFromEvents();
			base.Dispose(disposing);
		}
		public StaticTextViewItem(Type objectType, IModelStaticText model) : base(model, objectType) { }
		public new Label Control {
			get { return (Label)base.Control; }
		}
		#region IJScriptTestControl Members
		public string TestCaption {
			get { return Model.Id; }
		}
		public string ClientId {
			get { return this.Control != null ? ((WebControl)this.Control).ClientID : null; }
		}
		public IJScriptTestControl TestControl {
			get { return new JSLabelTestControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Field;
			}
		}
		#endregion
		protected override System.Drawing.FontStyle FontStyle {
			get {
				if(Control != null) {
					return RenderHelper.GetFontStyle(Control);
				}
				return System.Drawing.FontStyle.Regular;
			}
			set {
				if(Control != null) {
					RenderHelper.SetFontStyle(Control, value);
				}
			}
		}
		protected override System.Drawing.Color FontColor {
			get {
				if(Control != null) {
					return Control.ForeColor;
				}
				return System.Drawing.Color.Empty;
			}
			set {
				if(Control != null) {
					Control.ForeColor = value;
				}
			}
		}
		protected override System.Drawing.Color BackColor {
			get {
				if(Control != null) {
					return Control.BackColor;
				}
				return System.Drawing.Color.Empty;
			}
			set {
				if(Control != null) {
					Control.BackColor = value;
				}
			}
		}
		protected override void ResetBackColor() {
			BackColor = new System.Drawing.Color();
		}
		protected override void ResetFontColor() {
			Label label = (Label)Control;
			if(label != null) {
				label.ForeColor = new System.Drawing.Color();
			}
		}
		protected override void ResetFontStyle() {
			if(Control != null) {
				Control.Font.Bold = false;
				Control.Font.Italic = false;
				Control.Font.Underline = false;
				Control.Font.Strikeout = false;
			}
		}
	}
	#region Obsolete 15.2
	[Obsolete("Use StaticImageViewItem instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class StaticImageDetailItem : StaticImageViewItem {
		public StaticImageDetailItem(IModelStaticImage model, Type objectType)
			: base(objectType, model) {
		}
	}
	#endregion
	public class StaticImageViewItem : StaticImage {
		private XafCallbackManager CallbackManager {
			get {
				ICallbackManagerHolder holder = WebWindow.CurrentRequestPage as ICallbackManagerHolder;
				if(holder != null) {
					return holder.CallbackManager;
				}
				return null;
			}
		}
		private void ApplyAlignment() {
			Image imageControl = Control as Image;
			if(imageControl != null) {
				StaticVerticalAlign verticalAlign = Model.VerticalAlign;
				if(verticalAlign != StaticVerticalAlign.NotSet) {
					imageControl.Style.Add("vertical-align", verticalAlign.ToString());
				}
				StaticAlignHelper.ApplyAlignment(imageControl, Model.HorizontalAlign, Model.VerticalAlign);
			}
		}
		private void imageControl_PreRender(object sender, EventArgs e) {
			ApplyAlignment();
		}
		private void imageControl_Unload(object sender, EventArgs e) {
			((WebControl)sender).Unload -= new EventHandler(imageControl_Unload);
			if(CallbackManager != null) {
				CallbackManager.PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
		}
		private void CallbackManager_PreRenderInternal(object sender, EventArgs e) {
			((XafCallbackManager)sender).PreRenderInternal -= new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			ApplyAlignment();
		}
		private void SetImage(Image imageControl, string imageName) {
			ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			imageControl.ImageUrl = imageInfo.ImageUrl;
			imageControl.Width = imageInfo.Width;
			imageControl.Height = imageInfo.Height;
			imageControl.Visible = !imageInfo.IsEmpty;
		}
		protected override object CreateControlCore() {
			Image imageControl = new Image();
			imageControl.CssClass = "StaticImage";
			SetImage(imageControl, ImageName);
			imageControl.PreRender += new EventHandler(imageControl_PreRender);
			imageControl.Unload += new EventHandler(imageControl_Unload);
			return imageControl;
		}
		protected override void OnControlCreated() {
			base.OnControlCreated();
			if(CallbackManager != null) {
				CallbackManager.PreRenderInternal += new EventHandler<EventArgs>(CallbackManager_PreRenderInternal);
			}
		}
		protected override void OnImageChanged(string imageName) {
			base.OnImageChanged(imageName);
			Image imageControl = Control as Image;
			if(imageControl != null) {
				SetImage(imageControl, imageName);
			}
		}
		public StaticImageViewItem(Type objectType, IModelStaticImage model) : base(model, objectType) { }
	}
	[FixedWidthViewItem]
	public class WebActionContainerViewItem : ActionContainerViewItem {
		private List<ActionBase> actionsToRegister;
		private WebActionContainer Container {
			get { return Control != null && Control.ActionContainers.Count != 0 ? (WebActionContainer)Control.ActionContainers[0] : null; }
		}
		private void RegisterDelayed(ActionBase action) {
			if(!actionsToRegister.Contains(action)) {
				actionsToRegister.Add(action);
			}
		}
		private void RecreateContainer() {
			Control.SetActionContainers(new List<WebActionContainer>() { CreateActionContrainer(GetCategories()) });
		}
		private WebActionContainer CreateActionContrainer(string containerId) {
			WebActionContainer webActionContainer = new WebActionContainer();
			webActionContainer.ContainerId = containerId;
			webActionContainer.IsDropDown = false;
			return webActionContainer;
		}
		private void EnshurePostponedActions() {
			foreach(ActionBase action in actionsToRegister) {
				Register(action);
			}
			actionsToRegister.Clear();
		}
		private string GetCategories() {
			return Model.ActionContainer != null ? Model.ActionContainer.Id : Model.Id;
		}
		private void Container_ActionRegistered(object sender, ActionEventArgs e) {
			UpdateEmptyViewItemVisibility();
		}
		public WebActionContainerViewItem(IModelActionContainerViewItem model, Type objectType)
			: base(model, objectType) {
			actionsToRegister = new List<ActionBase>();
		}
		protected override object CreateControlCore() {
			ActionContainerHolder result = new ActionContainerHolder();
			result.ID = "ACH";
			result.ContainerStyle = ActionContainerStyle.Buttons;
			result.Orientation = Model.Orientation;
			WebActionContainer container = CreateActionContrainer(GetCategories());
			result.ActionContainers.Add(container);
			return result;
		}
		protected override void OnControlCreated() {
			EnshurePostponedActions();
			if(Container != null) {
				Container.ActionRegistered += new EventHandler<ActionEventArgs>(Container_ActionRegistered);
			}
			base.OnControlCreated();
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			if(Container != null) {
				Container.ActionRegistered -= new EventHandler<ActionEventArgs>(Container_ActionRegistered);
			}
			base.BreakLinksToControl(unwireEventsOnly);
		}
		public override void Register(ActionBase action) {
			if(Container != null) {
				Container.Register(action);
			}
			else {
				RegisterDelayed(action);
			}
			base.Register(action);
		}
		public override void BeginUpdate() {
			if(Container != null) {
				Container.BeginUpdate();
			}
		}
		public override void EndUpdate() {
			if(Container != null) {
				Container.EndUpdate();
			}
		}
		public override void Clear() {
			base.Clear();
			if(Container != null) {
				Container.Clear();
				RecreateContainer();
			}
			actionsToRegister.Clear();
			UpdateEmptyViewItemVisibility();
		}
		public override void ClearActions() {
			if(Container != null) {
				Container.Clear();
			}
			actionsToRegister.Clear();
		}
		public override string ContainerId {
			get { return GetCategories(); }
			set { Model.Id = value; }
		}
		public override ReadOnlyCollection<ActionBase> Actions {
			get { return Container != null ? Container.Actions : new ReadOnlyCollection<ActionBase>(actionsToRegister); }
		}
		public new ActionContainerHolder Control {
			get { return (ActionContainerHolder)base.Control; }
		}
	}
	public class StaticAlignHelper {
		public static void ApplyAlignment(WebControl control, StaticHorizontalAlign hAlign, StaticVerticalAlign vAlign) {
			if(vAlign != StaticVerticalAlign.NotSet) {
				Control parent = control.Parent;
				while(parent != null && !(parent is TableCell)) {
					parent = parent.Parent;
				}
				if(parent != null && parent.Controls.Count == 1) {
					((TableCell)parent).Style.Add("vertical-align", vAlign.ToString());
				}
			}
			Panel container = control.Parent as Panel;
			if(container != null && hAlign != StaticHorizontalAlign.NotSet) {
				container.Style.Add("text-align", hAlign.ToString());
			}
		}
	}
}
