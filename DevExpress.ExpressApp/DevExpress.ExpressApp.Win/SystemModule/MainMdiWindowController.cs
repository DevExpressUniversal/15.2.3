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
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Ribbon;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public class MainMdiWindowController : WindowController {
		private void Window_TemplateChanging(object sender, EventArgs e) {
			OnTemplateChanging();
		}
		private void Window_TemplateChanged(object sender, EventArgs e) {
			OnTemplateChanged();
		}
		private void MainForm_Load(object sender, EventArgs e) {
			if(Window.Template is XtraFormTemplateBase) return;
			DocumentManager documentManager = sender is IDocumentsHostWindow ? ((IDocumentsHostWindow)sender).DocumentManager : null;
			RibbonControl ribbon = sender is RibbonForm ? ((RibbonForm)sender).Ribbon : null;
			if(documentManager != null && ribbon != null) {
				string defaultPageCaption = CaptionHelper.GetLocalizedText("Ribbon", "HomePage", "Home");
				ribbon.SelectedPage = ribbon.Pages[defaultPageCaption];
				new RibbonDefaultSelectedPageHelper(defaultPageCaption).Attach(documentManager.View, ribbon);
			}
		}
		private IModelTemplateWin GetTemplateModel(Form form) {
			IFrameTemplate template = form is IFrameTemplate ? ((IFrameTemplate)form) : null;
			return (IModelTemplateWin)Application.GetTemplateCustomizationModel(template);
		}
		private DockManager GetDockManager(object form) {
			return form is IDockManagerHolder ? ((IDockManagerHolder)form).DockManager : null;
		}
		private void OnTemplateChanging() {
			if(Window.Template is Form) {
				((Form)Window.Template).Load -= MainForm_Load;
			}
		}
		private void OnTemplateChanged() {
			if(Window.Template is Form) {
				((Form)Window.Template).Load += MainForm_Load;
			}
			if(!(Window.Template is XtraFormTemplateBase)) {
				Form form = Window.Template as Form;
				if(form != null) {
					IModelTemplateWin templateModel = GetTemplateModel(form);
					DockManager dockManager = GetDockManager(form);
					if(templateModel != null && dockManager != null) {
						new DockManagerStateRestorer().Attach(form, templateModel, dockManager);
					}
				}
				RibbonForm ribbonForm = Window.Template as RibbonForm;
				if(ribbonForm != null && ribbonForm.Ribbon != null) {
					new RibbonMergeHelper().Attach(ribbonForm.Ribbon);
				}
				else {
					IBarManagerHolder barManagerHolder = Window.Template as IBarManagerHolder;
					if(barManagerHolder != null) {
						new BarManagerMergeHelper().Attach(barManagerHolder);
					}
				}
				IDocumentsHostWindow documentsHostWindow = Window.Template as IDocumentsHostWindow;
				if(documentsHostWindow != null && documentsHostWindow.DocumentManager != null) {
					new LoadingIndicatorHelper().Attach(documentsHostWindow.DocumentManager);
					new DocumentManagerViewPopupMenuHelper().Attach(documentsHostWindow.DocumentManager);
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.TemplateChanging += Window_TemplateChanging;
			Window.TemplateChanged += Window_TemplateChanged;
			OnTemplateChanged();
		}
		protected override void OnDeactivated() {
			Window.TemplateChanging -= Window_TemplateChanging;
			Window.TemplateChanged -= Window_TemplateChanged;
			if(Window.Template is Form) {
				((Form)Window.Template).Load -= MainForm_Load;
			}
			base.OnDeactivated();
		}
		public MainMdiWindowController() {
			TargetWindowType = WindowType.Main;
		}
	}
}
