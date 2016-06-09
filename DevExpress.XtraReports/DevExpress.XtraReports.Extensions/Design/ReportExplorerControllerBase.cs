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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.Design.Tools;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.Data.Browsing;
namespace System.ComponentModel {
	static class IComponentExtensions {
		public static bool IsVisible(this IComponent comp) {
			if(comp is Control) return false;
			AttributeCollection attributes = TypeDescriptor.GetAttributes(comp);
			return !attributes.Contains(new System.ComponentModel.DesignTimeVisibleAttribute(false));
		}
		public static bool IsVisual(this IComponent comp) {
			return comp is XRControl || comp is Control;
		}
	}
}
namespace DevExpress.XtraReports.Design {
	public abstract class ReportExplorerControllerBase : TreeListController {
		#region classes
		public class ComponentComparer : IComparer {
			public int Compare(object x, object y) {
				return ((XRControl)x).CompareTabOrder((XRControl)y);
			}
		}
		protected internal class ComponentNode : ComponentNodeBase {
			string defaultName;
			bool canRemove;
			protected IComponent fComponent;
			public override IComponent Component {
				get { return fComponent; }
			}
			public bool CanRemove {
				get { return canRemove; }
			}
			public bool IsFormattingNode {
				get { return fComponent is FormattingRule || fComponent is XRControlStyle; }
			}
			public bool IsExternalControlStyleNode {
				get { return fComponent is XRControlStyle && fComponent.Site == null; }
			}
			public ComponentNode(IComponent component, TreeListNodes owner)
				: this(component, owner, true) {
			}
			public ComponentNode(IComponent component, TreeListNodes owner, bool canRemove)
				: this(component, owner, canRemove, "Unknown") {
			}
			public ComponentNode(IComponent component, TreeListNodes owner, bool canRemove, string defaultName)
				: base(owner) {
				this.fComponent = component;
				this.canRemove = canRemove;
				this.defaultName = defaultName;
				UpdateView();
				UpdateImageIndex();
			}
			public void UpdateComponent(IComponent component) {
				this.fComponent = component;
			}
			public virtual void UpdateView() {
				Text = fComponent != null && fComponent.Site != null ? fComponent.Site.Name : defaultName;
			}
			public void UpdateImageIndex() {
				StateImageIndex = SelectImageIndex = GetImageIndex();
			}
			protected virtual int GetImageIndex() {
				return ReportExplorerControllerBase.GetImageIndex(fComponent);
			}
			public virtual string GetToolTip() {
				return "";
			}
		}
		protected class XRControlNode : ComponentNode {
			Func<XRControl, int> getImageIndex;
			protected XRControl Control {
				get { return (XRControl)fComponent; }
			}
			public XRControlNode(XRControl xrControl, TreeListNodes owner, Func<XRControl, int> getImageIndex)
				: base(xrControl, owner) {
					this.getImageIndex = getImageIndex;
					UpdateImageIndex();
			}
			public override string GetToolTip() {
				if(Control == null || Control.IsDisposed)
					return string.Empty;
				StringBuilder toolTipTextBuilder = new StringBuilder();
				foreach(XRBinding binding in Control.DataBindings) {
					if(binding != null) {
						if(toolTipTextBuilder.Length > 0)
							toolTipTextBuilder.Append('\n');
						toolTipTextBuilder.Append(String.Format("{0}: [{1}]", binding.PropertyName, binding.DisplayColumnName));
					}
				}
				return toolTipTextBuilder.ToString();
			}
			protected override int GetImageIndex() {
				return getImageIndex != null ? getImageIndex(Control) : -1;
			}
		}
		protected class SatelliteNode : ComponentNode {
			protected Control Control {
				get { return (Control)fComponent; }
			}
			public SatelliteNode(Control ctl, TreeListNodes owner)
				: base(ctl, owner, false) {
			}
			public override void UpdateView() {
				if(Control != null && Control.Name.Length > 0)
					Text = Control.Site != null ? Control.Site.Name : Control.Name;
				else
					base.UpdateView();
			}
		}
		protected class XRControlStyleNode : ComponentNode {
			public XRControlStyleNode(XRControlStyle style, TreeListNodes owner)
				: base(style, owner, true) {
			}
			protected XRControlStyle Style {
				get { return (XRControlStyle)fComponent; }
			}
			public override void UpdateView() {
				if(Style != null && Style.Site == null)
					Text = Style.Name;
				else
					base.UpdateView();
			}
		}
		#endregion
		#region static
		static string ExternalPrefix = "External";
		static private ImageCollection imageCollection = new ImageCollection();
		protected static Dictionary<string, int> imageIndices = new Dictionary<string, int>();
		static public ImageCollection ImageCollection {
			get { return imageCollection; }
		}
		static ReportExplorerControllerBase() {
			AddImage(XRBitmaps.SubBand, typeof(SubBand).Name);
			AddImage(XRBitmaps.Report, typeof(XtraReport).Name);
			AddImage(XRBitmaps.DetailReport, typeof(DetailReportBand).Name);
			AddImage(XRBitmaps.WinControlContainer, typeof(WinControlContainer).Name);
			AddImage(XRBitmaps.BottomMarginBand, typeof(BottomMarginBand).Name);
			AddImage(XRBitmaps.TopMarginBand, typeof(TopMarginBand).Name);
			AddImage(XRBitmaps.DetailBand, typeof(DetailBand).Name);
			AddImage(XRBitmaps.GroupFooterBand, typeof(GroupFooterBand).Name);
			AddImage(XRBitmaps.GroupHeaderBand, typeof(GroupHeaderBand).Name);
			AddImage(XRBitmaps.PageFooterBand, typeof(PageFooterBand).Name);
			AddImage(XRBitmaps.PageHeaderBand, typeof(PageHeaderBand).Name);
			AddImage(XRBitmaps.ReportFooterBand, typeof(ReportFooterBand).Name);
			AddImage(XRBitmaps.ReportHeaderBand, typeof(ReportHeaderBand).Name);
			AddImage(XRBitmaps.XRControl, typeof(XRControl).Name);
			AddImage(XRBitmaps.XRLabel, typeof(XRLabel).Name);
			AddImage(XRBitmaps.XRCheckBox, typeof(XRCheckBox).Name);
			AddImage(XRBitmaps.XRRichText, typeof(XRRichText).Name);
			AddImage(XRBitmaps.XRPictureBox, typeof(XRPictureBox).Name);
			AddImage(XRBitmaps.XRPanel, typeof(XRPanel).Name);
			AddImage(XRBitmaps.XRTable, typeof(XRTable).Name);
			AddImage(XRBitmaps.XRTableCell, typeof(XRTableCell).Name);
			AddImage(XRBitmaps.XRTableRow, typeof(XRTableRow).Name);
			AddImage(XRBitmaps.XRLine, typeof(XRLine).Name);
			AddImage(XRBitmaps.XRShape, typeof(XRShape).Name);
			AddImage(XRBitmaps.XRBarCode, typeof(XRBarCode).Name);
			AddImage(XRBitmaps.XRZipCode, typeof(XRZipCode).Name);
			AddImage(XRBitmaps.XRChart, typeof(XRChart).Name);
			AddImage(XRBitmaps.XRPivotGrid, typeof(XRPivotGrid).Name);
			AddImage(XRBitmaps.XRPageInfo, typeof(XRPageInfo).Name);
			AddImage(XRBitmaps.XRPageBreak, typeof(XRPageBreak).Name);
			AddImage(XRBitmaps.XRCrossBandLine, typeof(XRCrossBandLine).Name);
			AddImage(XRBitmaps.XRCrossBandBox, typeof(XRCrossBandBox).Name);
			AddImage(XRBitmaps.XRSubReport, typeof(XRSubreport).Name);
			AddImage(XRBitmaps.XRSparkline, typeof(XRSparkline).Name);
			AddImage(XRBitmaps.XRGauge, typeof(XRGauge).Name);
			AddImage(XRBitmaps.ControlStyle, typeof(XRControlStyle).Name);
			AddImage(XRBitmaps.ExternalControlStyle, ExternalPrefix + typeof(XRControlStyle).Name);
			AddImage(XRBitmaps.FormattingRule, typeof(FormattingRule).Name);
			AddImage(XRBitmaps.SqlDataSource, "SqlDataSource");
			AddImage(XRBitmaps.None, "None");
		}
		static protected int AddImage(Image bitmap, string key) {
			imageCollection.AddImage(bitmap);
			imageIndices.Add(key, imageCollection.Images.Count - 1);
			return imageCollection.Images.Count - 1;
		}
		protected static string GetTypeName(IComponent comp) {
			if(comp is XtraReport)
				return typeof(XtraReport).Name;
			else if(comp is XRControlStyle)
				return (comp.Site == null ? ExternalPrefix : string.Empty) + typeof(XRControlStyle).Name;
			else
				return comp != null ? comp.GetType().Name : string.Empty;
		}
		protected static int GetImageIndex(IComponent comp) {
			if(comp == null)
				return -1;
			int result;
			string key = GetTypeName(comp);
			if(!imageIndices.TryGetValue(key, out result)) {
				ToolboxBitmapAttribute attribute = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(comp)[typeof(ToolboxBitmapAttribute)];
				if(attribute != null) {
					Image image = FindImageXRControlSubclass(comp);
					if(image == null) {
						ToolboxBitmapAttribute componentAttribute = (ToolboxBitmapAttribute)TypeDescriptor.GetAttributes(typeof(Component))[typeof(ToolboxBitmapAttribute)];
						if(attribute == componentAttribute)
							return GetImageIndex("XRControl");
						image = attribute.GetImage(comp, false);
					}
					if(image != null)
						return AddImage(image, key);
				}
			}
			return result;
		}
		static Image FindImageXRControlSubclass(IComponent comp) {
			XRControl control = comp as XRControl;
			if(control == null)
				return null;
			Type type = comp.GetType();
			Type baseType = type.BaseType;
			bool hasAttribute = HasAttribute(type);
			while(baseType.Assembly != typeof(XRControl).Assembly) {
				hasAttribute |= HasAttribute(baseType);
				baseType = baseType.BaseType;
			}
			if(hasAttribute)
				return null;
			int index;
			if(imageIndices.TryGetValue(baseType.Name, out index)) {
				return imageCollection.Images[index];
			}
			return null;
		}
		static bool HasAttribute(Type type) {
			return type.GetCustomAttributes(typeof(ToolboxBitmapAttribute), false).Length > 0;
		}
		static int GetImageIndex(string key) {
			int result;
			return imageIndices.TryGetValue(key, out result) ? result : -1;
		}
		protected static bool ContainsNode(TreeListNode node1, TreeListNode node2) {
			if(node2.ParentNode == null)
				return false;
			if(node2.ParentNode.Equals(node1))
				return true;
			return ContainsNode(node1, node2.ParentNode);
		}
		protected ComponentNode CreateNode(IComponent component, TreeListNodes owner) {
			if(component is XRControl) {
				XRControl xrControl = (XRControl)component;
				if(xrControl.RealControl.Site != null)
					return new XRControlNode(xrControl.RealControl, owner, GetImageIndexCore);
			}
			if(component is XRControlStyle) {
				return new XRControlStyleNode(component as XRControlStyle, owner);
			}
			return component != null ? new ComponentNode(component, owner) : null;
		}
		protected virtual int GetImageIndexCore(XRControl control) {
			return control == null || control.IsDisposed ? GetImageIndex("None") : GetImageIndex(control);
		}
		protected void UpdateComponentView(TreeListNodes nodes, IList components) {
			UpdateNodes(nodes, components);
			foreach(XRControl xrControl in components) {
				ArrayList items = xrControl.GetPrintableControls();
				FilterComponents(items);
				items.Sort(GetComparer());
				if(xrControl is Band)
					items.AddRange(((Band)xrControl).SubBands);
				foreach(ComponentNode node in nodes) {
					if(node.Text == xrControl.Name) {
						UpdateComponentView(node.Nodes, items);
						break;
					}
				}
			}
		}
		protected virtual IComparer GetComparer() {
			return new ComponentComparer();
		}
		protected virtual void FilterComponents(ArrayList items) {
			Dictionary<XRControl, XRControl> controls = new Dictionary<XRControl, XRControl>();
			foreach(XRControl item in items)
				controls[item] = item;
			items.Clear();
			items.AddRange(controls.Values);
		}
		protected void UpdateNodes(TreeListNodes nodes, IList components) {
			for(int i = nodes.Count - 1; i >= 0; i--) {
				ComponentNode node = nodes[i] as ComponentNode;
				int nodeComponentIndex = components.IndexOf(node.Component);
				if(!(nodeComponentIndex > -1 && ReferenceEquals(components[nodeComponentIndex], node.Component)) && node.CanRemove)
					node.Remove();
			}
			int index = 0;
			foreach(IComponent component in components) {
				ComponentNode node = GetNodeByComponent(component, nodes);
				if(node == null) {
					node = BuildNode(component, nodes);
					index = node != null ? InsertNode(nodes, node, index) : index;
				} else {
					if(nodes.IndexOf(node) != index)
						node.TreeList.SetNodeIndex(node, index);
					index++;
					node.UpdateView();
					node.UpdateImageIndex();
				}
				if(component is WinControlContainer)
					UpdateNodes(node.Nodes, new IComponent[] { ((WinControlContainer)component).WinControl });
			}
		}
		static int InsertNode(TreeListNodes nodes, ComponentNode node, int index) {
			foreach(ComponentNode reviewNode in nodes)
				if(node.Text == reviewNode.Text)
					return index;
			((IList)nodes).Insert(index, node);
			return index + 1;
		}
		ComponentNode BuildNode(IComponent comp, TreeListNodes owner) {
			ComponentNode node = CreateNode(comp, owner);
			XtraListNode childNode = CreateSatelliteNode(comp as WinControlContainer, owner);
			if(childNode != null)
				((IList)node.Nodes).Add(childNode);
			return node;
		}
		static XtraListNode CreateSatelliteNode(WinControlContainer comp, TreeListNodes owner) {
			return comp != null && comp.WinControl != null ? new SatelliteNode(comp.WinControl, owner) : null;
		}
		protected static ComponentNode GetNodeByComponent(IComponent component, TreeListNodes nodes) {
			return GetNodeByPredicate(nodes, item => { return ReferenceEquals(component, item.Component); });
		}
		protected static ComponentNode GetNodeByPredicate(TreeListNodes nodes, Predicate<ComponentNode> predicate) {
			foreach(ComponentNode node in nodes)
				if(node.Component != null && predicate(node))
					return node;
			return null;
		}
		protected static ComponentNode SearchNodeByComponent(IComponent component, TreeListNodes nodes) {
			return (ComponentNode)FieldListController.SearchNodeByComponent(component, nodes);
		}
		#endregion
		protected XtraTreeView ReportTreeView {
			get { return (XtraTreeView)treeList; }
		}
		protected ReportExplorerControllerBase(IServiceProvider servProvider) : base(servProvider) {
		}
		protected virtual XRControl RootControl {
			get { return RootReport; }
		}
		protected virtual void UpdateView() {
			if(!IsControlAlive(ReportTreeView))
				return;
			ReportTreeView.BeginUnboundLoad();
			try {
				if(ReportTreeView.Nodes.Count == 0) {
					((IList)ReportTreeView.Nodes).Add(CreateNode(RootControl, ReportTreeView.Nodes));
					((XtraListNode)ReportTreeView.Nodes[0]).Expand();
				} else {
					((XtraListNode)ReportTreeView.Nodes[0]).Text = RootControl.Name;
				}
				UpdateComponentView(ReportTreeView.Nodes, new object[] { RootControl });
			} finally {
				ReportTreeView.EndUnboundLoad();
			}
		}
		protected void SetSelectedNode(IComponent component) {
			ComponentNode node = SearchNodeByComponent(component, ReportTreeView.Nodes);
			if(node == null) {
				component = FrameSelectionUIService.GetComponentContainer(this.designerHost, component);
				if(component != null)
					node = SearchNodeByComponent(component, ReportTreeView.Nodes);
			}
			ReportTreeView.SelectNode(node);
			if(ReportTreeView.Nodes.Count > 0 && ReportTreeView.SelectedNode == ReportTreeView.Nodes[0])
				((XtraListNode)ReportTreeView.Nodes[0]).Expand();
		}
		public override void UpdateTreeList() {
			UpdateView();
		}
	}
}
