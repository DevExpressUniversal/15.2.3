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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates {
	[ToolboxItem(false)]
	public partial class NestedFrameTemplate : UserControl, IFrameTemplate, ISupportActionsToolbarVisibility, IViewSiteTemplate, ISupportUpdate, IBarManagerHolder, ISupportStoreSettings, ISupportViewChanged, IXtraResizableControl, DevExpress.XtraPrinting.IBasePrintableProvider {
		public const string ListViewStateNodeName = "ListViewState";
		public const string MenuBarsCustomizationNodeName = "XtraBarsCustomization";
		private Size minSize;
		private readonly Size maxSize;
		private TemplatesHelper localizationHelper;
		private void OnSetViewControl() {
			IXtraResizableControl viewResizableControl = View.Control as IXtraResizableControl;
			if(viewResizableControl != null) {
				viewResizableControl.Changed += new EventHandler(viewResizableControl_Changed);
			}
			UpdateMinSize();
		}
		private void viewResizableControl_Changed(object sender, EventArgs e) {
			UpdateMinSize();
		}
		private void barDockControl_SizeChanged(object sender, EventArgs e) {
			if(View != null && View.IsControlCreated) {
				UpdateMinSize();
			}
		}
		private void UpdateMinSize() {
			Size newMinSize = CalculateMinSize();
			if(MinSize != newMinSize) {
				MinSize = newMinSize;
			}
		}
		private Size CalculateMinSize() {
			Size viewControlMinSize;
			IXtraResizableControl viewResizableControl = View.Control as IXtraResizableControl;
			if(viewResizableControl != null) {
				viewControlMinSize = viewResizableControl.MinSize;
			}
			else {
				viewControlMinSize = ((Control)View.Control).MinimumSize;
			}
			Size borderSize = CalculateBorderSize();
			Size newMinSize = new Size(
				barDockControlLeft.Width + viewControlMinSize.Width + barDockControlRight.Width + borderSize.Width,
				barDockControlTop.Height + viewControlMinSize.Height + barDockControlBottom.Height + borderSize.Height);
			return newMinSize;
		}
		private Size CalculateBorderSize() {
			int _width = viewSitePanel.Bounds.Width - viewSitePanel.DisplayRectangle.Width;
			int _height = viewSitePanel.DisplayRectangle.Height < 0 ? _width : viewSitePanel.Bounds.Height - viewSitePanel.DisplayRectangle.Height;
			return new Size(_width, _height);
		}
		protected void RaiseXtraResizableControlChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		private void SetBarManagerModel() {
			if(localizationHelper != null) {
				barManager.Model = localizationHelper.GetBarsCustomizationNode(View != null ? View.Id : String.Empty);
			}
		}
		protected virtual void OnBarMangerChanged() {
			if(BarManagerChanged != null) {
				BarManagerChanged(this, EventArgs.Empty);
			}
		}
		public NestedFrameTemplate() {
			InitializeComponent();
			maxSize = new Size(0, 0);
			barManager.ProcessShortcutsWhenInvisible = false;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		private View View {
			get { return viewSiteManager.View; }
		}
		public Bar ToolBar {
			get { return standardToolBar; }
		}
		public event EventHandler BarManagerChanged;
		#region IFrameTemplate Members
		private void view_ControlsCreated(object sender, EventArgs e) {
			OnSetViewControl();
		}
		private void OnViewChanged() {
			SetBarManagerModel();
			if(View != null) {
				if(View.IsControlCreated) {
					OnSetViewControl();
				}
				Tag = EasyTestTagHelper.FormatTestContainer(View.Caption);
			}
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(View));
			}
		}
		public ICollection<IActionContainer> GetContainers() {
			return actionContainersManager.GetContainers();
		}
		public void SetView(View view) {
			if(View != null) {
				View.ControlsCreated -= new EventHandler(view_ControlsCreated);
				if(View.IsControlCreated && View.Control is IXtraResizableControl) {
					((IXtraResizableControl)View.Control).Changed -= new EventHandler(viewResizableControl_Changed);
				}
			}
			viewSiteManager.SetView(view);
			if(View != null) {
				View.ControlsCreated += new EventHandler(view_ControlsCreated);
			}
			OnViewChanged();
		}
		public IActionContainer DefaultContainer {
			get { return actionContainersManager.DefaultContainer; }
		}
		public virtual void SetStatus(string[] messages) { }
		#endregion
		#region ISupportStoreSettings
		private void OnSettingsReloaded() {
			if(SettingsReloaded != null) {
				SettingsReloaded(this, EventArgs.Empty);
			}
		}
		protected virtual void ReloadSettingsCore() {
			modelSynchronizationManager.ApplyModel();
		}
		public virtual void SetSettings(IModelTemplate modelTemplate) {
			localizationHelper = new TemplatesHelper((IModelTemplateWin)modelTemplate);
			SetBarManagerModel();
		}		
		public void ReloadSettings() {
			ReloadSettingsCore();
			OnSettingsReloaded();
		}
		public virtual void SaveSettings() {
			modelSynchronizationManager.SynchronizeModel();			
		}
		public event EventHandler SettingsReloaded;
		#endregion
		#region ISupportUpdate Members
		void ISupportUpdate.BeginUpdate() {
			barManager.BeginUpdate();
		}
		void ISupportUpdate.EndUpdate() {
			barManager.EndUpdate();
		}
		#endregion
		#region IBarManagerHolder Members
		public BarManager BarManager {
			get { return barManager; }
		}
		#endregion
		#region IXtraResizableControl
		public bool IsCaptionVisible {
			get { return false; }
		}
		public Size MinSize {
			get { return minSize; }
			set {
				minSize = value;
				RaiseXtraResizableControlChanged();
			}
		}
		public Size MaxSize {
			get { return maxSize; }
		}
		public event EventHandler Changed;
		#endregion
		#region IViewSiteTemplate Members
		public object ViewSiteControl {
			get { return viewSitePanel; }
		}
		#endregion
		#region ISupportViewChanged Members
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		#endregion
		#region IActionBarVisibilityManager Members
		public void SetVisible(bool isVisible) {
			foreach(Bar bar in barManager.Bars) {
				bar.Visible = isVisible;
			}
		}		
		#endregion
		public object GetIPrintableImplementer() {
			if(View == null) return null;
			return View.Control;
		}
	}
}
