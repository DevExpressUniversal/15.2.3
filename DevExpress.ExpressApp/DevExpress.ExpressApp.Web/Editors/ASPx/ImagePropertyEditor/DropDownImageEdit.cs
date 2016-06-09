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
using DevExpress.ExpressApp.Web.Localization;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[ToolboxItem(false)]
	public class DropDownImageEdit : ASPxCallbackPanel, IPictureHolder {
		private ASPxButton showHideButton;
		private IPictureEditShowingStrategy showingStrategy;
		private ImageEdit imageEdit;
		private void imageEdit_Changed(object sender, ImageChangedEventArgs e) {
			OnImageChanged(e);
		}
		private void OnImageChanged(ImageChangedEventArgs args) {
			if(ImageChanged != null) {
				ImageChanged(this, args);
			}
		}
		private void DropDownImageEdit_Load(object sender, EventArgs e) {
			this.ClientSideEvents.EndCallback = String.Format("function(s,e) {{ {0} {1} }}", showingStrategy.GetShowScript(), GetSetButtonTextScript(HideImageText));
			showHideButton.ClientSideEvents.Click = String.Format(@"
				function(s, e) {{
					if({0}) {{
						{1} 
						{2}
					}} else {{ 
						{3}.PerformCallback();
					}} 
				}}", showingStrategy.GetIsVisibleScript(), showingStrategy.GetHideScript(), GetSetButtonTextScript(ShowImageText), ClientID);
			showingStrategy.RegisterOnShownScript(String.Format("function(s,e) {{ {0} }}", GetSetButtonTextScript(HideImageText)));
			showingStrategy.RegisterOnClosingScript(String.Format("function(s,e) {{ {0} }}", GetSetButtonTextScript(ShowImageText)));
		}
		private string GetSetButtonTextScript(string text) {
			return String.Format("{0}.SetText('{1}');", showHideButton.ClientID, text);
		}
		private string ShowImageText {
			get {
				return ASPxImagePropertyEditorLocalizer.Active.GetLocalizedString("ShowImage");
			}
		}
		private string HideImageText {
			get {
				return ASPxImagePropertyEditorLocalizer.Active.GetLocalizedString("HideImage");
			}
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			this.Load += new EventHandler(DropDownImageEdit_Load);
			ASPxWebControl showControl = showingStrategy.CreateControl(imageEdit);
			RenderHelper.SetupASPxWebControl(showControl);
			Controls.Add(showControl);
			showHideButton = RenderHelper.CreateASPxButton();
			showHideButton.Text = ShowImageText;
			showHideButton.ID = "SHB";
			showHideButton.ClientEnabled = true;
			Controls.Add(showHideButton);
			ChildControlsCreated = true;
		}
		public DropDownImageEdit(bool readOnly, bool showInPopup, ImagePropertyEditorStorageMode storageMode, bool postDataImmediatelly) {
			if(showInPopup) {
				showingStrategy = new ShowInPopupStrategy();
			}
			else {
				showingStrategy = new ShowInPanelStrategy();
			}
			imageEdit = new ImageEdit(readOnly, storageMode, postDataImmediatelly);
			imageEdit.ID = "IE";
			imageEdit.ImageChanged += new EventHandler<ImageChangedEventArgs>(imageEdit_Changed);
			SettingsLoadingPanel.Enabled = false;
		}
		public void SetControlImageUrl(string imageId, bool visibleOnRender) {
			imageEdit.SetControlImageUrl(imageId, visibleOnRender);
		}
		public int CustomImageHeight {
			get {
				return imageEdit.CustomImageHeight;
			}
			set {
				imageEdit.CustomImageHeight = value;
			}
		}
		public int CustomImageWidth {
			get {
				return imageEdit.CustomImageWidth;
			}
			set {
				imageEdit.CustomImageWidth = value;
			}
		}
		public System.Drawing.Image Image {
			get { return imageEdit.Image; }
			set { imageEdit.Image = value; }
		}
#if DebugTest
		public IPictureEditShowingStrategy ShowingStrategy {
			get { return showingStrategy; }
		}
#endif
		public event EventHandler<ImageChangedEventArgs> ImageChanged;
		public override void Dispose() {
			if(showHideButton != null) {
				showHideButton.Dispose();
				showHideButton = null;
			}
			if(imageEdit != null) {
				imageEdit.ImageChanged -= new EventHandler<ImageChangedEventArgs>(imageEdit_Changed);
				imageEdit.Dispose();
				imageEdit = null;
			}
			if(showingStrategy != null) {
				showingStrategy.Dispose();
				showingStrategy = null;
			}
			base.Dispose();
		}
	}
}
