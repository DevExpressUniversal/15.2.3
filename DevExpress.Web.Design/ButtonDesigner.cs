#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using System.Collections;
using System.Resources;
using System;
using DevExpress.Utils.About;
namespace DevExpress.Web.Design {
	public class ASPxButtonDesigner : ASPxWebControlDesigner {
		private ASPxButton button = null;
		public ASPxButton Button {
			get { return button; }
		}
		public bool AutoPostBack {
			get { return Button.AutoPostBack; }
			set {
				Button.AutoPostBack = value;
				PropertyChanged("AutoPostBack");
			}
		}
		public string Text {
			get { return Button.Text; }
			set {
				Button.Text = value;
				PropertyChanged("Text");
			}
		}
		public override void ShowAbout() {
			ButtonAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override void Initialize(IComponent component) {
			this.button = (ASPxButton)component;
			base.Initialize(component);
			EnsureReferences("DevExpress.Web" + AssemblyInfo.VSuffix);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new ButtonDesignerActionList(this);
		}
		protected override string GetDesignTimeHtmlInternal() {
			string html = "";
			if(string.IsNullOrEmpty(Button.Text) && Button.Image.IsEmpty) {
				Button.Text = "[" + Button.ID + "]";
				html = base.GetDesignTimeHtmlInternal();
				Button.Text = "";
			} else {
				html = base.GetDesignTimeHtmlInternal();
			}
			return html;
		}
		protected override string GetEmptyDesignTimeHtmlInternal() {
			return "[" + Button.ID + "]";
		}
	}
	public class ButtonAboutDialogHelper : AboutDialogHelperBase {
		public static void ShowAbout(IServiceProvider provider) {
			ShowAboutForm(provider, typeof(ASPxButton), new ProductKind[] { ProductKind.FreeOffer, ProductKind.DXperienceASP });
		}
		public static void ShowTrialAbout(IServiceProvider provider) {
			if (ShouldShowTrialAbout(typeof(ASPxButton)))
				ShowAbout(provider);
		}
	} 
	public class ButtonDesignerActionList : ASPxWebControlDesignerActionList {
		private ASPxButtonDesigner buttonDesigner = null;
		public ButtonDesignerActionList(ASPxButtonDesigner buttonDesigner)
			: base(buttonDesigner) {
			this.buttonDesigner = buttonDesigner;
		}
		public bool AutoPostBack {
			get { return ButtonDesigner.AutoPostBack; }
			set { ButtonDesigner.AutoPostBack = value; }
		}
		public string Text {
			get { return ButtonDesigner.Text; }
			set { ButtonDesigner.Text = value; }
		}
		protected ASPxButtonDesigner ButtonDesigner {
			get { return buttonDesigner; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection collection = base.GetSortedActionItems();
			collection.Add(new DesignerActionPropertyItem("AutoPostBack", "AutoPostBack"));
			collection.Add(new DesignerActionPropertyItem("Text", "Text"));
			return collection;
		}
	}
}
