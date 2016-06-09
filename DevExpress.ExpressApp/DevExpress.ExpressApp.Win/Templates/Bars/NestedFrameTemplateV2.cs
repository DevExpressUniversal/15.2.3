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
using System.Windows.Forms;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
namespace DevExpress.ExpressApp.Win.Templates.Bars {
	[ToolboxItem(false)]
	public partial class NestedFrameTemplateV2 : XtraUserControl, IActionControlsSite, IFrameTemplate, ISupportActionsToolbarVisibility, IViewSiteTemplate, ISupportUpdate, IBarManagerHolder, ISupportViewChanged, IContextMenuHolder, IBasePrintableProvider, IViewHolder {
		private static readonly object viewChanged = new object();
		private XtraResizableControlProxy resizableControlProxy;
		protected virtual void RaiseViewChanged(View view) {
			EventHandler<TemplateViewChangedEventArgs> handler = (EventHandler<TemplateViewChangedEventArgs>)Events[viewChanged];
			if(handler != null) {
				handler(this, new TemplateViewChangedEventArgs(view));
			}
		}
		protected override IXtraResizableControl GetInnerIXtraResizableControl() {
			return resizableControlProxy;
		}
		public NestedFrameTemplateV2() {
			InitializeComponent();
			resizableControlProxy = new XtraResizableControlProxy(viewSitePanel, barManager.DockControls);
			barManager.ProcessShortcutsWhenInvisible = false;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		public Bar ToolBar {
			get { return standardToolBar; }
		}
		#region IActionControlsSite Members
		IEnumerable<IActionControl> IActionControlsSite.ActionControls {
			get { return barManager.ActionControls; }
		}
		IEnumerable<IActionControlContainer> IActionControlsSite.ActionContainers {
			get { return barManager.ActionContainers; }
		}
		IActionControlContainer IActionControlsSite.DefaultContainer {
			get { return actionContainerDefault; }
		}
		#endregion
		#region IFrameTemplate Members
		void IFrameTemplate.SetView(View view) {
			viewSiteManager.SetView(view);
			if(view != null) {
				Tag = EasyTestTagHelper.FormatTestContainer(view.Caption);
			}
			RaiseViewChanged(view);
		}
		ICollection<IActionContainer> IFrameTemplate.GetContainers() {
			 { return new IActionContainer[0]; }
		}
		IActionContainer IFrameTemplate.DefaultContainer {
			get { return null; }
		}
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
		BarManager IBarManagerHolder.BarManager {
			get { return barManager; }
		}
		event EventHandler IBarManagerHolder.BarManagerChanged {
			add { }
			remove { }
		}
		#endregion
		#region IViewSiteTemplate Members
		object IViewSiteTemplate.ViewSiteControl {
			get { return viewSiteManager.ViewSiteControl; }
		}
		#endregion
		#region ISupportViewChanged Members
		event EventHandler<TemplateViewChangedEventArgs> ISupportViewChanged.ViewChanged {
			add { Events.AddHandler(viewChanged, value); }
			remove { Events.RemoveHandler(viewChanged, value); }
		}
		#endregion
		#region ISupportActionsToolbarVisibility Members
		void ISupportActionsToolbarVisibility.SetVisible(bool isVisible) {
			foreach(Bar bar in barManager.Bars) {
				bar.Visible = isVisible;
			}
		}
		#endregion
		#region IContextMenuHolder Members
		PopupMenu IContextMenuHolder.ContextMenu {
			get { return contextMenu; }
		}
		#endregion
		#region IBasePrintableProvider Members
		object IBasePrintableProvider.GetIPrintableImplementer() {
			View view = viewSiteManager.View;
			return view != null ? view.Control : null;
		}
		#endregion
		#region IViewHolder Members
		View IViewHolder.View {
			get { return viewSiteManager.View; }
		}
		#endregion
	}
}
