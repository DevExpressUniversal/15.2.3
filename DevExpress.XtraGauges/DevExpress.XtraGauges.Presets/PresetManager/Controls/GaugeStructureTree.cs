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
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Design;
using DevExpress.XtraGauges.Base;
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.DragDrop;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraGauges.Presets.PresetManager {
	[System.ComponentModel.ToolboxItem(false)]
	public class GaugeStructureTree : DXTreeView {
		DragDropHelper dragDropHelperCore;
		GaugeTypeNode circularGaugesNodeCore;
		GaugeTypeNode linearGaugesNodeCore;
		GaugeTypeNode digitalGaugesNodeCore;
		GaugeTypeNode stateIndicatorGaugesNodeCore;
		ScaleComponentFilter pointerFilter;
		ScaleComponentFilter layerFilter;
		ScaleComponentFilter otherFilter;
		delegate bool ScaleComponentFilter(IScaleComponent scaleComponent);
		public GaugeStructureTree() {
			AllowCustomMouseMoveProcessing = false;
			this.AllowDrag = false;
			this.AllowDrop = false;
			this.dragDropHelperCore = new DragDropHelper(this);
			LoadIcons();
			CreateFilters();
			CreateGaugeTypeNodes();
		}
		protected void LoadIcons() {
			ImageList = new ImageList();
			Image nodeImages = ResourceImageHelper.CreateImageFromResources(
					"DevExpress.XtraGauges.Presets.Resources.Images.gauge-designer-icons2.png",
					typeof(PresetCustomizeForm).Assembly
				);
			ImageList.ColorDepth = ColorDepth.Depth32Bit;
			ImageList.ImageSize = new Size(16, 16);
			ImageList.Images.AddStrip(nodeImages);
		}
		protected void CreateFilters() {
			this.pointerFilter = delegate(IScaleComponent sc) {
				return (sc is IMarker) || (sc is IRangeBar) || (sc is IScaleLevel) || (sc is INeedle);
			};
			this.layerFilter = delegate(IScaleComponent sc) {
				return (sc is IScaleBackgroundLayer) || (sc is IScaleEffectLayer) || (sc is ISpindleCap);
			};
			this.otherFilter = delegate(IScaleComponent sc) {
				return !(pointerFilter(sc) || layerFilter(sc));
			};
		}
		protected void CreateGaugeTypeNodes() {
			circularGaugesNodeCore = new GaugeTypeNode("Circular Gauges", NodeImageKeys.CircularGaugesRoot);
			linearGaugesNodeCore = new GaugeTypeNode("Linear Gauges", NodeImageKeys.LinearGaugesRoot);
			digitalGaugesNodeCore = new GaugeTypeNode("Digital Gauges", NodeImageKeys.DigitalGaugesRoot);
			stateIndicatorGaugesNodeCore = new GaugeTypeNode("State Indicator Gauges", NodeImageKeys.StateIndicatorGaugesRoot);
		}
		protected DragDropHelper DragDropHelper {
			get { return dragDropHelperCore; }
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			DragDropHelper.OnMouseDown(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			DragDropHelper.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			DragDropHelper.OnMouseUp(e);
		}
		int lockUpdateTreeCounter = 0;
		protected bool IsUpdateTreeViewLocked {
			get { return lockUpdateTreeCounter > 0; }
		}
		public void LockUpdateTreeView() {
			lockUpdateTreeCounter++;
		}
		public void UnlockUpdateTreeView() {
			--lockUpdateTreeCounter;
		}
		public void UpdateDesigner() {
			PresetCustomizeForm designerForm = FindForm() as PresetCustomizeForm;
			if(designerForm != null) designerForm.UpdateDesigner();
		}
		public void UpdateTreeView(IGaugeContainer container) {
			if(IsUpdateTreeViewLocked) return;
			LockUpdateTreeView();
			ClearStructure();
			foreach(IGauge gauge in container.Gauges) {
				GaugeNode gaugeNode = BuildGaugeNode(gauge);
				switch(gauge.GetType().Name) {
					case "CircularGauge":
						RootNodeAddGaugeNode(CircularGaugesNode, gaugeNode, NodeImageKeys.CircularGauge);
						break;
					case "LinearGauge":
						RootNodeAddGaugeNode(LinearGaugesNode, gaugeNode, NodeImageKeys.LinearGauge);
						break;
					case "DigitalGauge":
						RootNodeAddGaugeNode(DigitalGaugesNode, gaugeNode, NodeImageKeys.DigitalGauge);
						break;
					case "StateIndicatorGauge":
						RootNodeAddGaugeNode(StateIndicatorGaugesNode, gaugeNode, NodeImageKeys.StateIndicatorGauge);
						break;
				}
			}
			Nodes.AddRange(new TreeNode[] { CircularGaugesNode, LinearGaugesNode, DigitalGaugesNode, StateIndicatorGaugesNode });
			UnlockUpdateTreeView();
		}
		public void UpdateSelection(ICustomizationFrameClient client) {
			if(IsUpdateTreeViewLocked) return;
			LockUpdateTreeView();
			BaseGaugeStructureNode selectedNode;
			selectedNode = GetSelectedStructureNode(CircularGaugesNode, client);
			if(selectedNode == null) selectedNode = GetSelectedStructureNode(LinearGaugesNode, client);
			if(selectedNode == null) selectedNode = GetSelectedStructureNode(DigitalGaugesNode, client);
			if(selectedNode == null) selectedNode = GetSelectedStructureNode(StateIndicatorGaugesNode, client);
			if(selectedNode != null)
				this.SelectNode(selectedNode);
			else ClearSelection();
			UnlockUpdateTreeView();
		}
		protected BaseGaugeStructureNode GetSelectedStructureNode(BaseGaugeStructureNode rootNode, ICustomizationFrameClient client) {
			if(rootNode == null) return null;
			if(rootNode.CustomizationFrameClient == client) return rootNode;
			if(rootNode.Nodes.Count > 0) {
				foreach(TreeNode node in rootNode.Nodes) {
					BaseGaugeStructureNode result = GetSelectedStructureNode(node as BaseGaugeStructureNode, client);
					if(result != null) return result;
				}
			}
			return null;
		}
		protected void RootNodeAddGaugeNode(GaugeTypeNode rootNode, GaugeNode gaugeNode, int index) {
			if(string.IsNullOrEmpty(gaugeNode.Text)) {
				gaugeNode.Text = "Gauge" + rootNode.Nodes.Count.ToString();
			}
			gaugeNode.ImageIndex = gaugeNode.SelectedImageIndex = index;
			rootNode.Nodes.Add(gaugeNode);
		}
		protected void ClearStructure() {
			Nodes.Clear();
			CircularGaugesNode.Nodes.Clear();
			LinearGaugesNode.Nodes.Clear();
			DigitalGaugesNode.Nodes.Clear();
			StateIndicatorGaugesNode.Nodes.Clear();
		}
		protected GaugeNode BuildGaugeNode(IGauge gauge) {
			GaugeNode gaugeNode = new GaugeNode(gauge);
			ISerizalizeableElement sElement = gauge as ISerizalizeableElement;
			if(sElement != null) {
				List<ISerizalizeableElement> elementsToClassify = sElement.GetChildren();
				List<IScale> scales = TrimElements<IScale>(elementsToClassify);
				List<IScaleComponent> scaleComponents = TrimElements<IScaleComponent>(elementsToClassify);
				List<BaseGaugeStructureNode> scaleNodes = CreateElementNodes(scales, gauge);
				AddGroupNode(gaugeNode, "Scales", scaleNodes);
				for(int i = 0; i < scaleNodes.Count; i++) {
					IScale scale = ((GaugeElementNode)scaleNodes[i]).Element as IScale;
					List<IScaleComponent> pointers = FilterScaleDependentComponents(scale, scaleComponents, pointerFilter);
					List<IScaleComponent> layers = FilterScaleDependentComponents(scale, scaleComponents, layerFilter);
					List<IScaleComponent> otherElements = FilterScaleDependentComponents(scale, scaleComponents, otherFilter);
					AddGroupNode(scaleNodes[i], "Pointers", CreateElementNodes(pointers, gauge));
					AddGroupNode(scaleNodes[i], "Layers", CreateElementNodes(layers, gauge));
					AddGroupNode(scaleNodes[i], "Other Elements", CreateElementNodes(otherElements, gauge));
				}
				List<ILabel> labels = TrimElements<ILabel>(elementsToClassify);
				List<BaseGaugeStructureNode> labelNodes = CreateElementNodes(labels, gauge);
				AddGroupNode(gaugeNode, "Labels", labelNodes);
				List<IImageIndicator> imageIndicators = TrimElements<IImageIndicator>(elementsToClassify);
				List<BaseGaugeStructureNode> imageIndicatorNodes = CreateElementNodes(imageIndicators, gauge);
				AddGroupNode(gaugeNode, "Images", imageIndicatorNodes);
				List<IStateIndicator> indicators = TrimElements<IStateIndicator>(elementsToClassify);
				List<BaseGaugeStructureNode> indicatorNodes = CreateElementNodes(indicators, gauge);
				AddGroupNode(gaugeNode, "Indicators", indicatorNodes);
				if(elementsToClassify.Count > 0) {
					List<BaseGaugeStructureNode> dLayerNodes = new List<BaseGaugeStructureNode>();
					AppendElementNodes(dLayerNodes, TrimElements<IDigitalBackgroundLayer>(elementsToClassify), gauge);
					AppendElementNodes(dLayerNodes, TrimElements<IDigitalEffectLayer>(elementsToClassify), gauge);
					AddGroupNode(gaugeNode, "Layers", dLayerNodes);
				}
			}
			return gaugeNode;
		}
		List<IScaleComponent> FilterScaleDependentComponents(IScale scale, List<IScaleComponent> scaleComponents, ScaleComponentFilter filter) {
			List<IScaleComponent> scaleNodeChildren = new List<IScaleComponent>();
			for(int i = 0; i < scaleComponents.Count; i++) {
				if(scaleComponents[i].Scale == scale && filter(scaleComponents[i])) scaleNodeChildren.Add(scaleComponents[i]);
			}
			return scaleNodeChildren;
		}
		protected void AddGroupNode(BaseGaugeStructureNode node, string groupName, List<BaseGaugeStructureNode> elementNodes) {
			if(elementNodes.Count > 0) {
				GaugeElementGroupNode groupNode = new GaugeElementGroupNode(groupName);
				groupNode.Nodes.AddRange(elementNodes.ToArray());
				node.Nodes.Add(groupNode);
			}
		}
		protected void AppendElementNodes<T>(List<BaseGaugeStructureNode> list, List<T> elements, IGauge gauge) where T : class {
			foreach(T e in elements) {
				list.Add(new GaugeElementNode(e as IElement<IRenderableElement>, gauge));
			}
		}
		protected List<BaseGaugeStructureNode> CreateElementNodes<T>(List<T> elements, IGauge gauge) where T : class {
			List<BaseGaugeStructureNode> list = new List<BaseGaugeStructureNode>();
			foreach(T e in elements) {
				list.Add(new GaugeElementNode(e as IElement<IRenderableElement>, gauge));
			}
			return list;
		}
		protected List<T> TrimElements<T>(List<ISerizalizeableElement> children) where T : class {
			List<T> filtered = new List<T>();
			List<ISerizalizeableElement> elements = new List<ISerizalizeableElement>(children);
			foreach(ISerizalizeableElement e in elements) {
				if(e is T) {
					filtered.Add(e as T);
					children.Remove(e);
				}
			}
			return filtered;
		}
		protected GaugeTypeNode CircularGaugesNode {
			get { return circularGaugesNodeCore; }
		}
		protected GaugeTypeNode LinearGaugesNode {
			get { return linearGaugesNodeCore; }
		}
		protected GaugeTypeNode DigitalGaugesNode {
			get { return digitalGaugesNodeCore; }
		}
		protected GaugeTypeNode StateIndicatorGaugesNode {
			get { return stateIndicatorGaugesNodeCore; }
		}
	}
	public abstract class BaseGaugeStructureNode : TreeNode {
		protected BaseGaugeStructureNode(string s)
			: base(s) {
			ImageIndex = SelectedImageIndex = NodeImageKeys.Default;
		}
		public abstract bool AllowRemove { get; }
		public abstract bool AllowAdd { get; }
		public abstract bool AllowRename { get; }
		public abstract bool AllowDuplicate { get; }
		public abstract ICustomizationFrameClient CustomizationFrameClient { get; }
		public abstract void OnAdd();
		public abstract void OnRemove();
		public abstract void OnRename();
		public abstract void OnDuplicate();
		protected GaugeStructureTree Structure {
			get { return TreeView as GaugeStructureTree; }
		}
		public virtual bool AllowDrag { get { return false; } }
		public virtual bool CanDrop(BaseGaugeStructureNode dragNode) { return false; }
		public virtual bool ProcessDrop(BaseGaugeStructureNode dragNode) { return false; }
	}
	public class GaugeTypeNode : BaseGaugeStructureNode, ICustomizationFrameClient {
		public GaugeTypeNode(string s, int imageIndex)
			: base(s) {
			ImageIndex = SelectedImageIndex = imageIndex;
		}
		public sealed override bool AllowAdd { get { return true; } }
		public sealed override bool AllowRemove { get { return false; } }
		public sealed override bool AllowRename { get { return false; } }
		public sealed override bool AllowDuplicate { get { return false; } }
		public sealed override ICustomizationFrameClient CustomizationFrameClient { get { return this; } }
		public override void OnAdd() {
			if(Structure == null) return;
			PresetCustomizeForm designerForm = Structure.FindForm() as PresetCustomizeForm;
			if(designerForm == null) return;
			if(AddGauge(designerForm.DesignGaugeContainer) != null)
				designerForm.UpdateDesigner();
		}
		protected IGauge AddGauge(IGaugeContainer container) {
			IGauge result = null;
			if(container != null) {
				if(Text.Contains("Circular")) result = container.AddGauge(GaugeType.Circular);
				if(Text.Contains("Linear")) result = container.AddGauge(GaugeType.Linear);
				if(Text.Contains("Digital")) result = container.AddGauge(GaugeType.Digital);
				if(Text.Contains("State")) result = container.AddGauge(GaugeType.StateIndicator);
			}
			return result;
		}
		public void DuplicateGauge(IGauge sourceGauge) {
			if(Structure == null) return;
			Dictionary<IScale, IScale> restoreScale = new Dictionary<IScale,IScale>();
			PresetCustomizeForm designerForm = Structure.FindForm() as PresetCustomizeForm;
			if(designerForm == null) return;
			designerForm.BeginDesignerUpdate();
			try {
				IGauge cloneGauge = AddGauge(designerForm.DesignGaugeContainer);
				List<ISerizalizeableElement> children = ((ISerizalizeableElement)sourceGauge).GetChildren();
				List<BaseElement<IRenderableElement>> duplicateList = new List<BaseElement<IRenderableElement>>();;
				foreach(ISerizalizeableElement item in children) {
					BaseElement<IRenderableElement> source = item as BaseElement<IRenderableElement>;
					BaseElement<IRenderableElement> duplicate = cloneGauge.DuplicateElement(source);
					if(duplicate != null) {
						cloneGauge.AddGaugeElement(duplicate);
						duplicateList.Add(duplicate);
					}
					if(duplicate is IScale)
						restoreScale.Add(source as IScale, duplicate as IScale);
				}
				foreach(BaseElement<IRenderableElement>  item in duplicateList) {
					RestoreRelation(item, restoreScale);
				}
			}
			finally {
				designerForm.EndDesignerUpdate();
			}
		}
		void RestoreRelation(BaseElement<IRenderableElement> duplicate, IDictionary<IScale, IScale> scales) {
			IScaleStateIndicator indicatorComponent = duplicate as IScaleStateIndicator;
			if(indicatorComponent != null) {
				indicatorComponent.IndicatorScale = scales[indicatorComponent.Scale];
				return;
			}
			IArcScaleComponent arcScaleComponent = duplicate as IArcScaleComponent;
			if(arcScaleComponent != null) {
				arcScaleComponent.ArcScale = scales[arcScaleComponent.Scale] as IArcScale;
				return;
			}
			ILinearScaleComponent linearScaleComponent = duplicate as ILinearScaleComponent;
			if(linearScaleComponent != null) {
				linearScaleComponent.LinearScale = scales[linearScaleComponent.Scale] as ILinearScale;
				return;
			}
		}
		public override void OnRemove() { }
		public override void OnRename() { }
		public override void OnDuplicate() { }
		#region ICustomizationFrameClient Members
		Rectangle ICustomizationFrameClient.Bounds {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		event EventHandler ICustomizationFrameClient.Changed { add { } remove { } }
		CustomizationFrameBase[] ICustomizationFrameClient.CreateCustomizeFrames() {
			throw new NotImplementedException();
		}
		void ICustomizationFrameClient.ResetAutoLayout() {
			throw new NotImplementedException();
		}
		#endregion
		#region ISupportCustomizeAction Members
		public CustomizeActionInfo[] GetActions() {
			return new CustomizeActionInfo[] { new GaugeTypeNodeCustomizeAction("OnAdd", "Add Gauge", "Add Gauge", this) };
		}
		class GaugeTypeNodeCustomizeAction : CustomizeActionInfo {
			GaugeTypeNode node;
			static Image AddImage;
			static GaugeTypeNodeCustomizeAction() {
				ImageCollection btnImages = ImageHelper.CreateImageCollectionFromResources(
					"DevExpress.XtraGauges.Presets.Resources.Images.gauge-designer-icons1.png",
					typeof(PresetCustomizeForm).Assembly, new Size(16, 16));
				AddImage = btnImages.Images[0];
			}
			public GaugeTypeNodeCustomizeAction(string methodName, string description, string descriptionShort, GaugeTypeNode node)
				: base(methodName, description, descriptionShort, AddImage) {
				this.node = node;
			}
			public void OnAdd() {
				this.node.OnAdd();
			}
		}
		#endregion
	}
	public class GaugeNode : BaseGaugeStructureNode {
		IGauge gaugeCore;
		public GaugeNode(IGauge gauge)
			: base(gauge.Name) {
			gaugeCore = gauge;
		}
		public IGauge Gauge {
			get { return gaugeCore; }
		}
		public sealed override bool AllowAdd { get { return false; } }
		public sealed override bool AllowRemove { get { return true; } }
		public sealed override bool AllowRename { get { return true; } }
		public sealed override bool AllowDuplicate { get { return true; } }
		public sealed override ICustomizationFrameClient CustomizationFrameClient {
			get { return Gauge.Model as ICustomizationFrameClient; }
		}
		public override void OnAdd() { }
		public override void OnRemove() {
			Gauge.Dispose();
			if(Structure == null) return;
			Structure.UpdateDesigner();
		}
		public override void OnRename() {
			Gauge.Name = RenameHelper.Rename(this);
		}
		public override void OnDuplicate() {
			((GaugeTypeNode)this.Parent).DuplicateGauge(Gauge);
		}
	}
	public class GaugeElementNode : BaseGaugeStructureNode {
		IGauge gaugeCore;
		IElement<IRenderableElement> elementCore;
		public GaugeElementNode(IElement<IRenderableElement> e, IGauge gauge)
			: base(e.Name) {
			elementCore = e;
			gaugeCore = gauge;
			ImageIndex = SelectedImageIndex = NodeImageKeys.GetElementsImageIndex(Element);
		}
		public sealed override bool AllowAdd { get { return false; } }
		public sealed override bool AllowRemove { get { return true; } }
		public sealed override bool AllowRename { get { return true; } }
		public sealed override bool AllowDuplicate { get { return true; } }
		public IGauge Gauge {
			get { return gaugeCore; }
		}
		public IElement<IRenderableElement> Element {
			get { return elementCore; }
		}
		public sealed override ICustomizationFrameClient CustomizationFrameClient {
			get { return elementCore as ICustomizationFrameClient; }
		}
		public override void OnAdd() { }
		public override void OnRemove() {
			Gauge.RemoveGaugeElement(Element as BaseElement<IRenderableElement>);
		}
		public override void OnRename() {
			IComposite<IRenderableElement> parent = Element.Parent;
			parent.Remove(Element);
			Element.Name = RenameHelper.Rename(this);
			parent.Add(Element);
		}
		public override void OnDuplicate() {
			BaseElement<IRenderableElement> duplicate = Gauge.DuplicateElement(Element as BaseElement<IRenderableElement>);
			if(duplicate != null) Gauge.AddGaugeElement(duplicate);
		}
		public override bool AllowDrag {
			get { return Element is IScaleComponent; }
		}
		public override bool CanDrop(BaseGaugeStructureNode dragNode) {
			BaseGaugeModel dragNodeModel = BaseGaugeModel.Find(dragNode.CustomizationFrameClient);
			BaseGaugeModel dropNodeModel = BaseGaugeModel.Find(Element);
			return (Element is IScale) && (dragNodeModel == dropNodeModel);
		}
		public override bool ProcessDrop(BaseGaugeStructureNode dragNode) {
			GaugeElementNode elNode = dragNode as GaugeElementNode;
			if(elNode != null) {
				IScaleComponent scaleComponent = elNode.Element as IScaleComponent;
				IScale scale = Element as IScale;
				if(scaleComponent != null && scale != null && scaleComponent.Scale != scale) {
					IArcScale aScale = Element as IArcScale;
					if(aScale != null && scaleComponent is IArcScaleComponent) {
						((IArcScaleComponent)scaleComponent).ArcScale = aScale;
						return true;
					}
					ILinearScale lScale = Element as ILinearScale;
					if(lScale != null && scaleComponent is ILinearScaleComponent) {
						((ILinearScaleComponent)scaleComponent).LinearScale = lScale;
						return true;
					}
					if(scaleComponent is IScaleStateIndicator) {
						((IScaleStateIndicator)scaleComponent).IndicatorScale = scale;
						return true;
					}
				}
			}
			return false;
		}
	}
	public class GaugeElementGroupNode : BaseGaugeStructureNode {
		public GaugeElementGroupNode(string s)
			: base(s) {
			ImageIndex = SelectedImageIndex = NodeImageKeys.Group;
		}
		public sealed override bool AllowAdd { get { return false; } }
		public sealed override bool AllowRemove { get { return false; } }
		public sealed override bool AllowRename { get { return false; } }
		public sealed override bool AllowDuplicate { get { return false; } }
		public sealed override ICustomizationFrameClient CustomizationFrameClient {
			get {
				GaugeNode parentNode = FindParentNode<GaugeNode>(this);
				return (parentNode != null) ? parentNode.CustomizationFrameClient : null;
			}
		}
		static T FindParentNode<T>(BaseGaugeStructureNode node) where T : BaseGaugeStructureNode {
			while(node != null) {
				if(node is T)
					return (T)node;
				node = node.Parent as BaseGaugeStructureNode;
			}
			return null;
		}
		public override void OnAdd() { }
		public override void OnRemove() { }
		public override void OnRename() { }
		public override void OnDuplicate() { }
	}
	public static class NodeImageKeys {
		public static int Default = 8;
		public static int CircularGaugesRoot = 0;
		public static int LinearGaugesRoot = 1;
		public static int DigitalGaugesRoot = 2;
		public static int StateIndicatorGaugesRoot = 3;
		public static int CircularGauge = 4;
		public static int LinearGauge = 5;
		public static int DigitalGauge = 6;
		public static int StateIndicatorGauge = 7;
		public static int Group = 8;
		public static int GetElementsImageIndex(IElement<IRenderableElement> element) {
			string key = element.GetType().Name;
			int value;
			return Elements.TryGetValue(key, out value) ? value : Default;
		}
		static Dictionary<string, int> Elements;
		static NodeImageKeys() {
			Elements = new Dictionary<string, int>();
			Elements.Add("ArcScaleComponent", 9);
			Elements.Add("ArcScaleBackgroundLayerComponent", 10);
			Elements.Add("ArcScaleNeedleComponent", 11);
			Elements.Add("ArcScaleRangeBarComponent", 12);
			Elements.Add("ArcScaleMarkerComponent", 13);
			Elements.Add("ArcScaleSpindleCapComponent", 14);
			Elements.Add("ArcScaleEffectLayerComponent", 15);
			Elements.Add("ArcScaleStateIndicatorComponent", 16);
			Elements.Add("StateImageIndicatorComponent", 24);
			Elements.Add("LabelComponent", 17);
			Elements.Add("ImageIndicatorComponent", 25);
			Elements.Add("LinearScaleLevelComponent", 18);
			Elements.Add("LinearScaleComponent", 19);
			Elements.Add("LinearScaleBackgroundLayerComponent", 20);
			Elements.Add("LinearScaleRangeBarComponent", 21);
			Elements.Add("LinearScaleMarkerComponent", 22);
			Elements.Add("LinearScaleEffectLayerComponent", 23);
			Elements.Add("LinearScaleStateIndicatorComponent", 16);
			Elements.Add("DigitalBackgroundLayerComponent", 20);
			Elements.Add("DigitalEffectLayerComponent", 23);
			Elements.Add("StateIndicatorComponent", 16);
		}
	}
	public static class RenameHelper {
		public static string Rename(BaseGaugeStructureNode node) {
			if(node == null) return node.Text;
			Point screenPoint = node.TreeView.PointToScreen(node.Bounds.Location);
			Rectangle editorRect = new Rectangle(
					screenPoint.X, screenPoint.Y,
					node.TreeView.ClientRectangle.Width - node.Bounds.Left, Math.Max(node.Bounds.Height, 20)
				);
			using(ModalTextBox textBox = new ModalTextBox()) {
				textBox.Bounds = editorRect;
				textBox.EditText = node.Text;
				if(textBox.ShowDialog() != DialogResult.Cancel) {
					string newName = textBox.EditText.Trim();
					if(newName != node.Text) {
						IGaugeContainer container = GetIGaugeContainer(node);
						if(container != null) {
							List<string> names = GetNames(container);
							if(names.Contains(newName)) {
								newName = newName.TrimEnd(new char[] { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
								newName = UniqueNameHelper.GetUniqueName(newName, names, 0);
							}
							node.Text = newName;
						}
					}
				}
			}
			return node.Text;
		}
		static List<string> GetNames(IGaugeContainer container) {
			List<string> names = new List<string>();
			foreach(IGauge g in container.Gauges) {
				names.AddRange(g.GetNames());
			}
			return names;
		}
		static IGaugeContainer GetIGaugeContainer(BaseGaugeStructureNode node) {
			IGaugeContainer container = null;
			GaugeStructureTree tree = node.TreeView as GaugeStructureTree;
			if(tree != null) {
				PresetCustomizeForm designerForm = tree.FindForm() as PresetCustomizeForm;
				if(designerForm != null) {
					container = designerForm.DesignGaugeContainer;
				}
			}
			return container;
		}
	}
	public class DragDropHelper {
		GaugeStructureTree treeViewCore;
		public DragDropHelper(GaugeStructureTree tree) {
			this.treeViewCore = tree;
			this.dragCursorCore = new DragCursor();
		}
		enum DragDropState { Regular, WaitBeginDrag, Dragging }
		readonly Cursor DropCursor = Cursors.PanWest;
		readonly Cursor NoDropCursor = Cursors.No;
		readonly Point InvalidPoint = new Point(-10000, -10000);
		readonly int BeginDraggingDistance = 4;
		DragCursor dragCursorCore;
		DragDropState State = DragDropState.Regular;
		Point StartPoint;
		BaseGaugeStructureNode DragNode;
		protected GaugeStructureTree Structure {
			get { return treeViewCore; }
		}
		public void OnMouseMove(MouseEventArgs ea) {
			if((ea.Button & MouseButtons.Left) != 0) {
				Point currentPoint = ea.Location;
				switch(State) {
					case DragDropState.Dragging:
						DoDragging(currentPoint);
						break;
					case DragDropState.WaitBeginDrag:
						if(CanStartDragging(currentPoint)) {
							StartDragging(currentPoint);
						}
						break;
				}
			}
		}
		public void OnMouseDown(MouseEventArgs ea) {
			if(ea.Button == MouseButtons.Left && State == DragDropState.Regular) {
				Point currentPoint = ea.Location;
				if(CanWaitBeginDrag(currentPoint, out DragNode)) {
					StartPoint = currentPoint;
					State = DragDropState.WaitBeginDrag;
					SetTreeViewCapture();
				}
			}
			else CancelDragging();
		}
		bool CanWaitBeginDrag(Point p, out BaseGaugeStructureNode node) {
			node = Structure.GetNodeAt(p) as BaseGaugeStructureNode;
			return node != null && node.AllowDrag;
		}
		public void OnMouseUp(MouseEventArgs ea) {
			if(ea.Button == MouseButtons.Left && State == DragDropState.Dragging) {
				EndDragging(ea.Location);
			}
			else CancelDragging();
		}
		bool CanStartDragging(Point p) {
			return Math.Max(Math.Abs(StartPoint.X - p.X), Math.Abs(StartPoint.Y - p.Y)) >= BeginDraggingDistance;
		}
		bool CanDrop(BaseGaugeStructureNode dragNode, Point p, out BaseGaugeStructureNode node) {
			node = Structure.GetNodeAt(p) as BaseGaugeStructureNode;
			return node != null && node.CanDrop(dragNode);
		}
		protected void StartDragging(Point p) {
			State = DragDropState.Dragging;
			DragCursorOn();
		}
		protected void DoDragging(Point p) {
			DragCursorCheckPosition(p);
		}
		protected virtual void EndDragging(Point p) {
			BaseGaugeStructureNode node;
			if(CanDrop(DragNode, p, out node)) {
				if(node.ProcessDrop(DragNode)) Structure.UpdateDesigner();
			}
			CancelDragging();
		}
		void CancelDragging() {
			ResetTreeViewCapture();
			StartPoint = InvalidPoint;
			State = DragDropState.Regular;
			DragNode = null;
			DragCursorOff();
		}
		private void SetTreeViewCapture() {
			if(!Structure.Capture) {
				Structure.Capture = true;
				Structure.MouseCaptureChanged += StructureMouseCaptureChanged;
			}
		}
		private void ResetTreeViewCapture() {
			if(Structure.Capture) {
				Structure.MouseCaptureChanged -= StructureMouseCaptureChanged;
				Structure.Capture = false;
			}
		}
		void StructureMouseCaptureChanged(object sender, EventArgs e) {
			if(!Structure.Capture) Structure.Capture = true;
		}
		private DragCursor DragCursorForm {
			get { return dragCursorCore; }
		}
		private void DragCursorOff() {
			if(Structure.Cursor != Cursors.Default) {
				Structure.Cursor = Cursors.Default;
			}
			if(DragCursorForm.Visible) {
				DragCursorForm.Visible = false;
				DragCursorForm.DragNode = null;
			}
		}
		private void DragCursorOn() {
			if(!DragCursorForm.Visible) {
				DragCursorForm.DragNode = DragNode;
				DragCursorForm.Visible = true;
			}
		}
		private void DragCursorCheckPosition(Point p) {
			BaseGaugeStructureNode node;
			Cursor newCursor = CanDrop(DragNode, p, out node) ? DropCursor : NoDropCursor;
			if(Structure.Cursor != newCursor) Structure.Cursor = newCursor;
			if(DragCursorForm.Visible) {
				DragCursorForm.Location = Structure.PointToScreen(p) + new Size(0, -DragCursorForm.Height / 2);
			}
		}
		class DragCursor : BaseDragHelperForm {
			BaseGaugeStructureNode dragNodeCore;
			Font itemFont;
			StringFormat itemTextFormat;
			public DragCursor()
				: base(0, 0) {
				itemFont = new Font("Tahoma", 8.25f);
				itemTextFormat = new StringFormat();
				itemTextFormat.Alignment = StringAlignment.Near;
				itemTextFormat.LineAlignment = StringAlignment.Center;
				itemTextFormat.Trimming = StringTrimming.EllipsisCharacter;
				itemTextFormat.FormatFlags = StringFormatFlags.NoWrap;
				Size = new Size(154, 24);
				Opacity = 0.75f;
			}
			public BaseGaugeStructureNode DragNode {
				get { return dragNodeCore; }
				set { dragNodeCore = value; }
			}
			protected override void OnPaintBackground(PaintEventArgs e) {
			}
			protected override void OnPaint(PaintEventArgs e) {
				GraphicsCache cache = new GraphicsCache(e);
				cache.Graphics.FillRectangle(Brushes.White, 0, 0, Size.Width, Size.Height);
				if(DragNode != null) {
					Image img = DragNode.TreeView.ImageList.Images[DragNode.ImageIndex];
					cache.Graphics.DrawImageUnscaled(img, 4, 4);
					cache.Graphics.DrawString(DragNode.Text, itemFont, Brushes.Black, new RectangleF(24, 0, 130, 24), itemTextFormat);
				}
			}
		}
	}
}
