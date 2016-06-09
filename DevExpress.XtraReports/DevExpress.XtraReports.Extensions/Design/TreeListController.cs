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

using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using DevExpress.XtraTreeList;
namespace DevExpress.XtraReports.Design.Tools {
	using System.ComponentModel.Design;
	using DevExpress.XtraReports.UI;
	using System.Windows.Forms;
	public interface ISupportController {
		TreeListController ActiveController { get; set; }
		TreeListController CreateController(IServiceProvider serviceProvider);
	}
	public abstract class TreeListController : IDisposable, IServiceProvider {
		protected static bool IsControlAlive(TreeList control) {
			return control != null && !control.IsDisposed;
		}
		bool batchSync;
		protected TreeList treeList;
		protected IServiceProvider serviceProvider;
		protected IDesignerHost designerHost;
		protected ISelectionService selectionService;
		protected IComponentChangeService changeService;
		protected TreeListController(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			this.selectionService = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
			this.changeService = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			SubscribeEvents();
		}
		protected virtual void SubscribeEvents() {
			this.changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
			this.changeService.ComponentAdded += new ComponentEventHandler(OnComponentAddRemove);
			this.changeService.ComponentRemoved += new ComponentEventHandler(OnComponentAddRemove);
			this.designerHost.LoadComplete += new EventHandler(OnLoadComplete);
			this.designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnDesignerTransactionClosed);
			this.selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
		}
		protected virtual void UnsubscribeEvents() {
			this.changeService.ComponentAdded -= new ComponentEventHandler(OnComponentAddRemove);
			this.changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentAddRemove);
			this.changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
			this.designerHost.LoadComplete -= new EventHandler(OnLoadComplete);
			this.designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnDesignerTransactionClosed);
			this.selectionService.SelectionChanged -= new EventHandler(OnSelectionChanged);
		}
		protected XtraReport RootReport {
			get { return (XtraReport)designerHost.RootComponent; }
		}
		public virtual void Dispose() {
			if(treeList != null && ((ISupportController)treeList).ActiveController == this) {
				UnsubscribeTreeListEvents(treeList);
				((ISupportController)treeList).ActiveController = null;
				treeList = null;
			}
			UnsubscribeEvents();
		}
		void OnComponentAddRemove(object sender, ComponentEventArgs e) {
			CheckUpdate(e.Component);
		}
		void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(e.Component is XtraReport && e.Member != null && e.Member.Name == "ScriptsSource")
				return;
			CheckUpdate(e.Component);
		}
		protected void CheckUpdate(object component) {
			if(designerHost.Loading || !UpdateNeeded(component)) return;
			ReportDesigner designer = RootReport != null ? designerHost.GetDesigner(RootReport) as ReportDesigner : null;
			if(designer == null || !designer.IsActive) return;
			if(DesignMethods.IsDesignerInTransaction(designerHost))
				batchSync = true;
			else
				UpdateTreeList();
		}
		protected virtual bool UpdateNeeded(object component) {
			return true;
		}
		void OnDesignerTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			if(batchSync) UpdateTreeList();
			batchSync = false;
		}
		protected virtual void OnSelectionChanged(object sender, EventArgs e) {
		}
		void OnLoadComplete(object sender, EventArgs e) {
			UpdateTreeList();
		}
		public virtual void CaptureTreeList(Control control) {
			this.treeList = control as TreeList;
			if(this.treeList is ISupportController)
				CaptureTreeListCore((ISupportController)this.treeList, this.treeList);
		}
		public virtual void UpdateTreeList() {
		}
		public virtual void ClearTreeList() {
			if(!IsControlAlive(treeList))
				return;
			treeList.Nodes.Clear();
		}
		protected void CaptureTreeListCore(ISupportController supporter, TreeList treeList) {
			if(supporter.ActiveController == this)
				return;
			if(supporter.ActiveController != null)
				supporter.ActiveController.UnsubscribeTreeListEvents(treeList);
			supporter.ActiveController = this;
			SubscribeTreeListEvents(treeList);
		}
		public virtual void SubscribeTreeListEvents(TreeList treeList) {
		}
		public virtual void UnsubscribeTreeListEvents(TreeList treeList) {
		}
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			return this.serviceProvider.GetService(serviceType);
		}
		#endregion
	}
}
