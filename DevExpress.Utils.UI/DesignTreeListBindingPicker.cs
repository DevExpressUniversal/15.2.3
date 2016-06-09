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
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Localization;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design {
	public class DesignTreeListBindingPicker : DataSourceNativeTreeList {
		#region static
		static XtraListNode GetFirstChild(XtraListNode node) {
			return (node != null && node.Nodes.Count > 0) ? node.Nodes[0] as XtraListNode : null;
		}
		static DataMemberListNode GetDataMemberNode(XtraListNode node) {
			XtraListNode memberNode = node;
			while(memberNode != null && !(memberNode is DataMemberListNode)) {
				memberNode = GetFirstChild(memberNode);
			}
			return memberNode as DataMemberListNode;
		}
		static string GetDataMember(Collection<Pair<object, string>> dataSourceDataMemberPairs, DesignBinding binding) {
			if (binding.IsNull)
				return string.Empty;
			foreach(Pair<object, string> pair in dataSourceDataMemberPairs) {
				if(pair.First == binding.DataSource)
					return string.IsNullOrEmpty(pair.Second) || binding.DataMember.StartsWith(pair.Second) ? binding.DataMember : string.Format("{0}.{1}", pair.Second, binding.DataMember);
			}
			return string.Empty;
		}
		#endregion
		protected DesignBinding fSelectedBinding;
		IWindowsFormsEditorService edSvc;
		TreeListNode expandedCollapsedNode;
		bool addNoneNode = true;
		bool firstTimeEditorOpen = true;
		[DefaultValue(true)]
		public bool AddNoneNode { get { return addNoneNode; } set { addNoneNode = value; } } 
		public DesignBinding SelectedBinding {
			get { return fSelectedBinding; }
		}
		public DesignTreeListBindingPicker()
			: this(new TreeListPickManager()) {
		}
		public DesignTreeListBindingPicker(TreeListPickManager pickManager)
			: base(pickManager) {
			this.BorderStyle = BorderStyles.NoBorder;
			this.Width = 200;
		}
		protected override string NoneNodeText {
			get { return PreviewLocalizer.GetString(PreviewStringId.NoneString); }
		}
		void UpdateSelectedDesignBinding(XtraListNode node) {
			fSelectedBinding = GetDesignBinding(GetDataMemberNode(node));
		}
		protected DesignBinding GetDesignBinding(DataMemberListNode dataMemberNode) {
			return dataMemberNode != null && dataMemberNode.DataSource != null && dataMemberNode.DataMember != null ? CreateDesignBinding(dataMemberNode.DataSource, dataMemberNode.DataMember) :
				CreateDesignBinding(null, null);
		}
		protected virtual DesignBinding CreateDesignBinding(object dataSource, string dataMember) {
			return new DesignBinding(dataSource, dataMember);
		}
		protected override bool IsInputKey(Keys key) {
			return key == Keys.Enter ? true :
				base.IsInputKey(key);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			if(this.SelectedNode != expandedCollapsedNode)
				CloseDropDown();
			expandedCollapsedNode = null;
		}
		protected override void OnNodeExpand(object sender, DevExpress.XtraTreeList.BeforeExpandEventArgs e) {
			base.OnNodeExpand(sender, e);
			expandedCollapsedNode = e.Node;
			this.SelectNode(e.Node);
		}
		protected override void OnNodeCollapse(object sender, DevExpress.XtraTreeList.BeforeCollapseEventArgs e) {
			base.OnNodeCollapse(sender, e);
			expandedCollapsedNode = e.Node;
			this.SelectNode(e.Node);
		}
		void CloseDropDown() {
			if(CanCloseDropDown(SelectedNode)) {
				UpdateSelectedDesignBinding(SelectedNode);
				CloseDropDownCore();
			}
		}
		protected void CloseDropDownCore() {
			if(edSvc != null)
				edSvc.CloseDropDown();
		}
		protected virtual bool CanCloseDropDown(XtraListNode node) {
			return node != null && node.Nodes.Count == 0;
		}
		public void End() {
			Stop();
			fSelectedBinding = null;
		}
		public void Start(Collection<Pair<object, string>> dataSourceDataMemberPairs, IServiceProvider serviceProvider, DesignBinding selectedBinding) {
			edSvc = serviceProvider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			this.fSelectedBinding = selectedBinding;
			this.Nodes.Clear();
			this.PickManager.ServiceProvider = serviceProvider;
			this.PickManager.FillContent((XtraListNodes)Nodes, dataSourceDataMemberPairs, AddNoneNode);
			ForceCreateHandle();
			base.Start();
			SelectBindingNode(GetDataMember(dataSourceDataMemberPairs, selectedBinding));
			DesignLookAndFeelHelper.SetParentLookAndFeel(this, serviceProvider);
			if(dataSourceDataMemberPairs.Count > 0) {
				Rectangle bounds = GetUnionBounds((XtraListNodes)Nodes, Rectangle.Empty);
				if(firstTimeEditorOpen) {
					firstTimeEditorOpen = false;
					bounds.Width += 2 * SystemInformation.VerticalScrollBarWidth;
				}
				this.Size = new Size(Math.Max(Width, bounds.Width), Math.Max(Height, Math.Min(200, bounds.Height)));
			}
		}
		public void Start(IList dataSources, IServiceProvider serviceProvider, DesignBinding selectedBinding) {
			Collection<Pair<object, string>> dataSourcesPairs = new Collection<Pair<object, string>>();
			foreach(object dataSource in dataSources)
				dataSourcesPairs.Add(new Pair<object, string>(dataSource, string.Empty));
			Start(dataSourcesPairs, serviceProvider, selectedBinding);
		}
		void SelectBindingNode(string dataMember) {
			XtraListNode node = FindSelectedNode(dataMember);
			if(node == null)
				node = (XtraListNode)PickManager.FindNoneNode((IList)Nodes);
			SelectNode(node);
			UpdateSelectedDesignBinding(node);
		}
		DataMemberListNode FindSelectedNode(string dataMember) {
			foreach(XtraListNode item in Nodes) {
				DataMemberListNode node = FindSelectedNode((XtraListNodes)item.Nodes, dataMember);
				if(node != null) return node;
			}
			return null;
		}
		private DataMemberListNode FindSelectedNode(XtraListNodes nodes, string dataMember) {
			return this.fSelectedBinding == null || this.fSelectedBinding.IsNull ? null :
				(DataMemberListNode)PickManager.FindDataMemberNode(nodes, dataMember);
		}
		private Rectangle GetUnionBounds(XtraListNodes nodes, Rectangle bounds) {
			foreach(XtraListNode node in nodes) {
				bounds = Rectangle.Union(bounds, node.Bounds);
				if(node.Expanded)
					bounds = GetUnionBounds((XtraListNodes)node.Nodes, bounds);
			}
			return bounds;
		}
		void ForceCreateHandle() {
			IntPtr ignoredHandle = new IntPtr();
			ignoredHandle = Handle;
		}
	}
}
