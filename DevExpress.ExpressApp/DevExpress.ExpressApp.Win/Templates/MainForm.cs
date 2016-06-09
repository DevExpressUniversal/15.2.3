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
using System.IO;
using System.Text;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
namespace DevExpress.ExpressApp.Win.Templates {
	public partial class MainForm : MainFormTemplateBase, IDockManagerHolder, ISupportClassicToRibbonTransform, INavigationPanelHolder {
		public override void SetSettings(IModelTemplate modelTemplate) {
			base.SetSettings(modelTemplate);
			navigation.Model = TemplatesHelper.GetNavBarCustomizationNode();
			formStateModelSynchronizerComponent.Model = GetFormStateNode();
			modelSynchronizationManager.ModelSynchronizableComponents.Add(navigation);
		}
		protected virtual void InitializeImages() {
			barMdiChildrenListItem.Glyph = ImageLoader.Instance.GetImageInfo("Action_WindowList").Image;
			barMdiChildrenListItem.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_WindowList").Image;
			barSubItemPanels.Glyph = ImageLoader.Instance.GetImageInfo("Action_Navigation").Image;
			barSubItemPanels.LargeGlyph = ImageLoader.Instance.GetLargeImageInfo("Action_Navigation").Image;
		}
		public MainForm() {
			InitializeComponent();
			InitializeImages();
			UpdateMdiModeDependentProperties();
			documentManager.BarAndDockingController = mainBarAndDockingController;
			documentManager.MenuManager = mainBarManager;
			BarManager.ForceLinkCreate();
		}
		public Bar ClassicStatusBar {
			get { return _statusBar; }
		}
		public DockPanel DockPanelNavigation {
			get { return dockPanelNavigation; }
		}
		public DockManager DockManager {
			get { return mainDockManager; }
		}
		protected override void UpdateMdiModeDependentProperties() {
			base.UpdateMdiModeDependentProperties();
			bool isMdi = UIType == UIType.StandardMDI || UIType == UIType.TabbedMDI;
			viewSitePanel.Visible = !isMdi;
			if(isMdi) {
				barSubItemWindow.Visibility = BarItemVisibility.Always;
				barMdiChildrenListItem.Visibility = BarItemVisibility.Always;
			}
			else {
				barSubItemWindow.Visibility = BarItemVisibility.Never;
				barMdiChildrenListItem.Visibility = BarItemVisibility.Never;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(ModelTemplate != null && !string.IsNullOrEmpty(ModelTemplate.DockManagerSettings)) {
				MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(ModelTemplate.DockManagerSettings));
				DockManager.SerializationOptions.RestoreDockPanelsText = false;
				DockManager.RestoreLayoutFromStream(stream);
			}
		}
		protected override void OnClosing(CancelEventArgs e) {
			if(ModelTemplate != null) {
				MemoryStream stream = new MemoryStream();
				DockManager.SaveLayoutToStream(stream);
				ModelTemplate.DockManagerSettings = Encoding.UTF8.GetString(stream.ToArray());
			}
			base.OnClosing(e);
		}
		private void mainBarManager_Disposed(object sender, System.EventArgs e) {
			if(this.mainBarManager != null) {
				this.mainBarManager.Disposed -= new System.EventHandler(mainBarManager_Disposed);
			}
			modelSynchronizationManager.ModelSynchronizableComponents.Remove(barManager);
			this.barManager = null;
			this.mainBarManager = null;
			this._mainMenuBar = null;
			this._statusBar = null;
			this.standardToolBar = null;
			this.barDockControlBottom = null;
			this.barDockControlLeft = null;
			this.barDockControlRight = null;
			this.barDockControlTop = null;
		}
	}
}
