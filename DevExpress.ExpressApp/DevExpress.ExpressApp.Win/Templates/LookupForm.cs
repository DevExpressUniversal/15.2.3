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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Utils;
namespace DevExpress.ExpressApp.Win.Templates {
	public partial class LookupForm : XtraFormTemplateBase, ILookupPopupFrameTemplate {
		private const int minWidth = 420;
		private const int minHeight = 150;
		private LookupControlTemplate frameTemplate;
		private void ButtonsContainersPanel_Changed(object sender, EventArgs e) {
			frameTemplate.ButtonsContainersPanel.MaximumSize = new Size(0, frameTemplate.ButtonsContainersPanel.Root.MinSize.Height);
		}
		protected override IModelFormState GetFormStateNode() {
			if(View != null) {
				return TemplatesHelper.GetFormStateNode(View.Id);
			}
			else {
				return base.GetFormStateNode();
			}
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			if(frameTemplate.IsSearchEnabled) {
				frameTemplate.FindEditor.Focus();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				frameTemplate.ButtonsContainersPanel.Changed -= new EventHandler(ButtonsContainersPanel_Changed);
			}
			base.Dispose(disposing);
		}
		protected override DevExpress.XtraBars.Ribbon.RibbonFormStyle RibbonFormStyle {
			get {
				return DevExpress.XtraBars.Ribbon.RibbonFormStyle.Standard;
			}
		}
		public override void SetSettings(IModelTemplate modelTemplate) {
			base.SetSettings(modelTemplate);
			formStateModelSynchronizerComponent.Model = GetFormStateNode();
		}
		public LookupForm() {
			InitializeComponent();
			MinimumSize = new Size(minWidth, minHeight);
			StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			NativeMethods.SetExecutingApplicationIcon(this);
			ShowInTaskbar = true;
			KeyPreview = true;
			frameTemplate = new LookupControlTemplate();
			Controls.Add(frameTemplate);
			frameTemplate.Dock = DockStyle.Fill;
			actionsContainersManager.ActionContainerComponents.AddRange(frameTemplate.GetContainers());
			actionsContainersManager.DefaultContainer = frameTemplate.DefaultContainer;
			viewSiteManager.ViewSiteControl = (Control)frameTemplate.ViewSiteControl;
			frameTemplate.ButtonsContainersPanel.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 12, 12, 0);
			((Control)this).Padding = new Padding(0, 0, 0, 12);
			frameTemplate.ButtonsContainersPanel.Changed += new EventHandler(ButtonsContainersPanel_Changed);
			frameTemplate.ButtonsContainersPanel.SendToBack();
		  }
		public override void SetView(DevExpress.ExpressApp.View view) {
			frameTemplate.SetView(view);
			if(view != null) {
				SetFormIcon(view);
			}
			int nonClientWidth = Size.Width - ClientSize.Width + Padding.Size.Width;
			int nonClientHeight = Size.Height - ClientSize.Height + Padding.Size.Height;
			ClientSize = frameTemplate.PreferredSize;
			MinimumSize = new Size(
				Math.Max(frameTemplate.MinimumSize.Width + nonClientWidth, minWidth),
				Math.Max(frameTemplate.MinimumSize.Height + nonClientHeight, minHeight));
		}
		public LookupControlTemplate FrameTemplate { get { return frameTemplate; } }
		#region ILookupPopupFrameTemplate Members
		public void SetStartSearchString(string searchString) {
			frameTemplate.SetStartSearchString(searchString);
		}
		public bool IsSearchEnabled {
			get { return frameTemplate.IsSearchEnabled; }
			set { frameTemplate.IsSearchEnabled = value; }
		}
		public void FocusFindEditor() { }
		#endregion		
	}
}
