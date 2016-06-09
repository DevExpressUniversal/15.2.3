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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using System.Collections.Generic;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design {
	public class FieldListController : TreeListController {
		public static ComponentNodeBase SearchNodeByComponent(IComponent component, TreeListNodes nodes) {
			foreach(ComponentNodeBase node in nodes) {
				if(node.Component != null && ReferenceEquals(node.Component, component))
					return node;
				ComponentNodeBase searchNode = SearchNodeByComponent(component, node.Nodes);
				if(searchNode != null)
					return searchNode;
			}
			return null;
		}
		int messageProcessing;
		bool InMessageProcessing { get { return messageProcessing > 0; } }
		public FieldListController(IServiceProvider serviceProvider)
			: base(serviceProvider) {
		}
		FieldListTreeView FieldListTreeView {
			get { return (FieldListTreeView)this.treeList; }
		}
		public override void SubscribeTreeListEvents(TreeList treeList) {
			if(treeList != null) {
				treeList.AfterFocusNode += new NodeEventHandler(tv_AfterFocusNode);
				treeList.KeyDown += new KeyEventHandler(tv_KeyDown);
				treeList.MouseUp += new MouseEventHandler(tv_MouseUp);
			}
		}
		public override void UnsubscribeTreeListEvents(TreeList treeList) {
			if(treeList != null) {
				treeList.AfterFocusNode -= new NodeEventHandler(tv_AfterFocusNode);
				treeList.KeyDown -= new KeyEventHandler(tv_KeyDown);
				treeList.MouseUp -= new MouseEventHandler(tv_MouseUp);
			}
		}
		void tv_MouseUp(object sender, MouseEventArgs e) {
			DataMemberListNodeBase node = FieldListTreeView.GetNodeAt(e.X, e.Y) as DataMemberListNodeBase;
			if(node == null || designerHost.IsDebugging())
				return;
			if(e.Button.IsLeft()) {
				ISelectionService selectionSvc = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
				if(!ReferenceEquals(node.Component, selectionService.PrimarySelection))
					SelectComponent(node.Component);
			} else if(e.Button.IsRight()) {
				FieldListTreeView.SelectNode(node);
				FieldListTreeView.ShowContextMenu(node, new Point(e.X, e.Y));
			}
		}
		void tv_AfterFocusNode(object sender, NodeEventArgs e) {
			if(!InMessageProcessing && (e.Node is DataMemberListNodeBase))
				SelectComponent(((DataMemberListNodeBase)e.Node).Component);
		}
		void SelectComponent(IComponent component) {
			if(component != null) {
				messageProcessing++;
				ISelectionService selectionSvc = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
				selectionSvc.SetSelectedComponents(new object[] { component });
				messageProcessing--;
			}
		}
		private void tv_KeyDown(object sender, KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				DeleteSelectedComponent();
		}
		private void DeleteSelectedComponent() {
			DataMemberListNodeBase selectedNode = FieldListTreeView.SelectedNode as DataMemberListNodeBase;
			if(selectedNode == null || selectedNode.Component == null)
				return;
			int selectedID = selectedNode.Id;
			if(selectedNode.NextNode == null)
				selectedID = selectedNode.PrevNode == null ? selectedNode.ParentNode.Id : selectedNode.PrevNode.Id;
			messageProcessing++;
			IMenuCommandServiceEx menuCommandServiceEx = (IMenuCommandServiceEx)designerHost.GetService(typeof(IMenuCommandService));
			if(selectedNode.Component is CalculatedField)
				menuCommandServiceEx.GlobalInvoke(FieldListCommands.DeleteCalculatedField, new object[] { selectedNode });
			else if(selectedNode.Component is Parameter)
				menuCommandServiceEx.GlobalInvoke(FieldListCommands.DeleteParameter, new object[] { selectedNode });
			messageProcessing--;
			FieldListTreeView.SelectNode(selectedID);
		}
		DataPair GetSelectedDataPair() {
			ISelectionService selectionService = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
			if(selectionService == null) return null;
			XRControl control = selectionService.PrimarySelection as XRControl;
			if(control == null) return null;
			if(!(control is Band)) {
				DataPair dataPair = GetControlDataPair(control);
				if(dataPair != null) return dataPair;
			}
			XtraReportBase report = control is XtraReportBase ? (XtraReportBase)control : control.Report;
			return GetReportDataPair(report);
		}
		DataPair GetReportDataPair(XtraReportBase report) {
			if(report == null)
				return null;
			return new DataPair(GetDataSource(report), report.DataMember);
		}
		object GetDataSource(XtraReportBase report) {
			return report != null ? ReportHelper.GetEffectiveDataSource(report) : null;
		}
		DataPair GetControlDataPair(XRControl control) {
			if(control == null || control.DataBindings.Count == 0)
				return null;
			XRControlDesigner designer = designerHost.GetDesigner(control) as XRControlDesigner;
			if(designer == null)
				return null;
			XRBinding binding = control.DataBindings[designer.GetBindablePropName()];
			if(binding == null) binding = control.DataBindings[0];
			object dataSource = binding.DataSource;
			if(dataSource == null) dataSource = GetDataSource(control.Report);
			return binding != null ? new DataPair(dataSource, binding.DataMember) : null;
		}
		protected override void OnSelectionChanged(object sender, EventArgs e) {
			if(DesignMethods.IsDesignerInTransaction(designerHost) || InMessageProcessing || FieldListTreeView.DraggNode)
				return;
			messageProcessing++;
			DataPair dataPair = GetSelectedDataPair();
			if(dataPair != null)
				UpdateSelection(dataPair.Source, dataPair.Member);
			else {
				ExpandParentNode(selectionService.PrimarySelection as IComponent);
				DataMemberListNodeBase node = (DataMemberListNodeBase)SearchNodeByComponent(selectionService.PrimarySelection as IComponent, FieldListTreeView.Nodes);
				if(node != null) {
					FieldListTreeView.SelectNode(node);
					ExpandParentNode(FieldListTreeView.SelectedNode);
				} else
					UpdateSelection(null, "");
			}
			messageProcessing--;
		}
		void UpdateSelection(object dataSource, string dataMember) {
			TreeListNode node = GetDataMemberListNode(dataSource, dataMember);
			if(node != null)
				FieldListTreeView.SelectNode(node);
			else if(FieldListTreeView.Nodes.Count > 0)
				FieldListTreeView.SelectFirstNode();
		}
		DataMemberListNode GetDataMemberListNode(object dataSource, string dataMember) {
			if(dataSource != null && dataMember != null) {
				INode sourceNode = FieldListTreeView.PickManager.FindSourceNode(FieldListTreeView.Nodes, dataSource);
				if(sourceNode != null)
					return (DataMemberListNode)FieldListTreeView.PickManager.FindDataMemberNode(sourceNode.ChildNodes, dataMember);
			}
			return null;
		}
		void ExpandParentNode(IComponent component) {
			DataMemberListNodeBase parentNode = null;
			DataMemberListNodeBase node = (DataMemberListNodeBase)SearchNodeByComponent(component, FieldListTreeView.Nodes);
			if(node != null) {
				ExpandParentNode(node);
			} else if(component is CalculatedField)
				parentNode = GetDataMemberListNode(((CalculatedField)component).GetEffectiveDataSource(), ((CalculatedField)component).DataMember);
			if(parentNode != null)
				parentNode.Expand();
		}
		static void ExpandParentNode(XtraListNode node) {
			if(node != null && node.ParentNode != null)
				((XtraListNode)node.ParentNode).Expand();
		}
		protected override bool UpdateNeeded(object component) {
			return component is CalculatedField || component is Parameter || BindingHelper.IsListSource(component) || designerHost.RootComponent.Equals(component);
		}
		public override void UpdateTreeList() {
			UpdateDataSource(this.designerHost);
		}
		public override void ClearTreeList() {
			UpdateDataSource(null);
		}
		void UpdateDataSource(IDesignerHost host) {
			if(FieldListTreeView != null) {
				messageProcessing++;
				DataMemberListNodeBase node = FieldListTreeView.SelectedNode as DataMemberListNodeBase;
				FieldListTreeView.UpdateDataSource(host);
				if(node != null)
					UpdateSelection(node.DataSource, node.DataMember);
				FieldListTreeView.Refresh();
				messageProcessing--;
			}
		}
	}
	public class FieldListTreeView : DataSourceNativeTreeList, IToolTipControlClient, ISupportController {
		#region static
		static string GetNodeToolTipText(DataMemberListNodeBase node) {
			if(node == null) return string.Empty;
			string dataMember = (ContextMenuHelper.IdentifyNode(node) != NodeType.ParameterTableNode && !string.IsNullOrEmpty(node.DataMember)) ?
				String.Format(ReportLocalizer.GetString(ReportStringId.UD_TTip_DataMemberDescription), node.DataMember.Replace(".", "." + hairSpace)) : string.Empty;
			string description = ReportLocalizer.GetString(ReportStringId.UD_TTip_ItemDescription);
			if(node.Nodes.Count > 0)
				description = ReportLocalizer.GetString(ReportStringId.UD_TTip_TableDescription);
			return description + dataMember;
		}
		#endregion
		const string hairSpace = "\u200A";
		private bool showNodeToolTips;
		bool dragged;
		int toolTipDelay = 1500;
		TreeListController activeController;
		public TreeListController ActiveController {
			get { return activeController; }
			set { activeController = value; }
		}
		internal bool DraggNode { get { return dragged; } }
		public bool ShowNodeToolTips {
			get { return showNodeToolTips; }
			set { showNodeToolTips = value; }
		}
		public FieldListTreeView(IServiceProvider serviceProvider)
			: base(new TreeListPickManager(new DataContextOptions(true, true))) {
			this.serviceProvider = serviceProvider;
			Size = new System.Drawing.Size(Math.Max(Width, 10), Math.Max(Height, 10));
			this.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.OptionsSelection.MultiSelect = true;
			this.OptionsBehavior.SmartMouseHover = false;
			this.OptionsView.AutoWidth = false;
			this.BestFitVisibleOnly = true;
			ToolTipController.DefaultController.AddClientControl(this);
			this.ShowNodeToolTips = true;
			AfterCollapse += OnCollapseExpand;
			AfterExpand += OnCollapseExpand;
		}
		void OnCollapseExpand(object sender, NodeEventArgs e) {
			BestFitColumns(false);
		}
		public TreeListController CreateController(IServiceProvider serviceProvider) {
			return new FieldListController(serviceProvider);
		}
		protected override string NoneNodeText {
			get { return PreviewLocalizer.GetString(PreviewStringId.NoneString); }
		}
		public new void SelectNode(TreeListNode node) {
			if(this.OptionsSelection.MultiSelect)
				Selection.Set(node);
			base.SelectNode(node);
		}
		public void SelectNode(int id) {
			SelectNode(FindNodeByID(id));
		}
		public void SelectFirstNode() {
			SelectNode(Nodes.FirstNode);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Stop();
				ToolTipController.DefaultController.RemoveClientControl(this);
				Nodes.Clear();
				if(activeController != null) {
					activeController.UnsubscribeTreeListEvents(this);
					activeController = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			dragged = true;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			dragged = false;
		}
		protected override void OnItemDrag(object sender, ItemDragEventArgs e) {
			IDataInfoContainer dataContainer = e.Item as IDataInfoContainer;
			if(dataContainer != null) {
				List<DataInfo> dataInfoItems = new List<DataInfo>();
				foreach(TreeListNode node in Selection) {
					IDataInfoContainer dataItemContainer = node as IDataInfoContainer;
					if(dataItemContainer == null)
						continue;
					DataInfo[] dataInfos = dataItemContainer.GetData();
					foreach(DataInfo dataInfo in dataInfos)
						dataInfoItems.AddRange(GetNestedData(dataInfo));
				}
				if(dataInfoItems.Count > 0)
					((Control)serviceProvider.GetService(typeof(IBandViewInfoService))).DoDragDrop(dataInfoItems.ToArray(), DragDropEffects.Copy);
				dragged = false;
			}
		}
		IList<DataInfo> GetNestedData(DataInfo sourceInfo) {
			DataInfo[] dataInfos = PickManager.GetData(sourceInfo.Source, sourceInfo.Member, item => { return !item.IsListType && !item.IsComplex; });
			return dataInfos != null && dataInfos.Length > 0 ? dataInfos : new DataInfo[] { sourceInfo };
		}
		protected override MenuItemDescriptionCollection CreateMenuItems(DataMemberListNodeBase node) {
			MenuItemDescriptionCollection items = base.CreateMenuItems(node);
			if(serviceProvider != null) {
				IMenuCreationService serv = serviceProvider.GetService(typeof(IMenuCreationService)) as IMenuCreationService;
				if(serv != null)
					serv.ProcessMenuItems(MenuKind.FieldList, items);
			}
			return items;
		}
		protected override XtraContextMenuBase CreateContextMenu() {
			return new XtraContextMenu();
		}
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(System.Drawing.Point point) {
			DataMemberListNodeBase node = GetNodeAt(point) as DataMemberListNodeBase;
			if(node != null && !this.ContextMenuVisible) {
				ToolTipControlInfo info = ToolTipService.GetListToolTipInfo(this, point, node.Bounds, GetNodeToolTipText(node), node);
				if(info != null) {
					info.ImmediateToolTip = false;
					info.Interval = toolTipDelay;
				}
				return info;
			}
			return null;
		}
		bool DevExpress.Utils.IToolTipControlClient.ShowToolTips {
			get { return false; }
		}
		public void UpdateDataSource(IServiceProvider serviceProvider) {
			IDataSourceCollectionProvider dataSourceCollectionProvider = serviceProvider != null ? (IDataSourceCollectionProvider)serviceProvider.GetService(typeof(IDataSourceCollectionProvider)) : null;
			UpdateDataSource(serviceProvider, dataSourceCollectionProvider != null ? dataSourceCollectionProvider.GetDataSourceCollection(null) : null);
			BestFitColumns(false);
		}
	}
}
