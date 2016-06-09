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
using System.ComponentModel;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
namespace DevExpress.ExpressApp.Web.Controls {
	public enum DetailViewCaptionMode {
		ViewCaption,
		ObjectCaption,
		ViewAndObjectCaption
	}
	[ToolboxItem(false)]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ViewCaptionLabelControl : WebControl {
		Label firstPart = new Label();
		Label secondPart = new Label();
		public ViewCaptionLabelControl() {
			firstPart.CssClass = "XafVCap-First";
			secondPart.CssClass = "XafVCap-Second";
			Controls.Add(firstPart);
			Controls.Add(secondPart);
		}
		public Label FirstPart {
			get { return firstPart; }
		}
		public Label SecondPart {
			get { return secondPart; }
		}
	}
	[ToolboxItem(false)] 
	[Designer("DevExpress.ExpressApp.Web.Design.ViewCaptionControlDesigner, DevExpress.ExpressApp.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	public class ViewCaptionControl : SimpleViewDependentControl<ViewCaptionLabelControl> {
		private bool showObjectCaptionFirst;
		private string captionSeparator = " - ";
		private View view;
		private DetailViewCaptionMode detailViewCaptionMode = DetailViewCaptionMode.ViewAndObjectCaption;
		private WebWindow window;
		private void UpdateControlTextOld(string objectCaption, string viewCaption) {
			SplitString result = new SplitString();
			result.Separator = CaptionSeparator;
			if(ShowObjectCaptionFirst) {
				result.FirstPart = objectCaption;
				result.SecondPart = viewCaption;
			}
			else {
				result.FirstPart = viewCaption;
				result.SecondPart = objectCaption;
			}
			Control.SecondPart.Text = System.Web.HttpUtility.HtmlEncode(result.Text);
		}
		private void UpdateControlText(string objectCaption, string viewCaption) {
			string firstPart = ShowObjectCaptionFirst ? objectCaption : viewCaption;
			string secondPart = ShowObjectCaptionFirst ? viewCaption : objectCaption;
			if(string.IsNullOrEmpty(secondPart)) {
				Control.FirstPart.Text = string.Empty;
				Control.SecondPart.Text = System.Web.HttpUtility.HtmlEncode(firstPart);
			}
			else {
				Control.FirstPart.Text = System.Web.HttpUtility.HtmlEncode(firstPart);
				Control.SecondPart.Text = System.Web.HttpUtility.HtmlEncode(secondPart);
			}
		}
		private void UpdateControlText(View view) {
			if(view != null) {
				string objectCaption = (view is DetailView && DetailViewCaptionMode != DetailViewCaptionMode.ViewCaption) ? view.GetCurrentObjectCaption() : "";
				string viewCaption = (view is DetailView && DetailViewCaptionMode == DetailViewCaptionMode.ObjectCaption) ? "" : view.Caption;
				view = null;
				if(WebApplicationStyleManager.IsNewStyle) {
					UpdateControlText(objectCaption, viewCaption);
				}
				else {
					UpdateControlTextOld(objectCaption, viewCaption);
				}
			}
		}
		protected override void SetupControl() {
			base.SetupControl();
			Control.ID = "VSL";
		}
		public override void SetView(View view) {
			this.view = view;
			UpdateControlText(view);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(WebWindow.CurrentRequestWindow != null) {
				window = WebWindow.CurrentRequestWindow;
				window.PagePreRender += new EventHandler(Window_PreRender);
			}
			if(TestScriptsManager.EasyTestEnabled && Control != null) {
				Control.Attributes[EasyTestTagHelper.TestField] = "FormCaption";
				Control.Attributes[EasyTestTagHelper.TestControlClassName] = JSLabelTestControl.ClassName;
			}
		}
		protected override void OnUnload(EventArgs e) {
			if(window != null) {
				window.PagePreRender -= new EventHandler(Window_PreRender);
				window = null;
			}
			view = null;
			base.OnUnload(e);
		}
		public ViewCaptionControl()
			: base() {
		}
		private void Window_PreRender(object sender, EventArgs e) {
			UpdateControlText(view);
		}
		public DetailViewCaptionMode DetailViewCaptionMode {
			get { return detailViewCaptionMode; }
			set { detailViewCaptionMode = value; }
		}
		public bool ShowObjectCaptionFirst {
			get { return showObjectCaptionFirst; }
			set { showObjectCaptionFirst = value; }
		}
		public string CaptionSeparator {
			get { return captionSeparator; }
			set { captionSeparator = value; }
		}
#if DebugTest
		public void SetViewForTest(View view) {
			SetView(view);
		}
#endif
	}
}
