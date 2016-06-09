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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Base;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraBars.Docking2010.Views.Widget;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface IRelativeLengthElement {
		DevExpress.XtraBars.Docking2010.Views.Widget.Length Length { get; set; }
		int ActualLength { get; set; }
		Orientation Orientation { get; set; }
		Rectangle LayoutRect { get; set; }
		void CaclLayout(Graphics g, Rectangle bounds);
		bool Visible { get; set; }
	}
	public class DockingContainer : IRelativeLengthElement, IBaseObject {
		public DockingContainer() {
			Nodes = new DockingContainerCollection();
			Nodes.CollectionChanged += NodesCollectionChanged;
			Length = new Length();
			Splitters = new List<DevExpress.XtraBars.Docking2010.Views.Tabbed.ISplitterInfo>();
			Element = null;
		}
		void NodesCollectionChanged(Base.CollectionChangedEventArgs<DockingContainer> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(ea.Element.Parent == null) {
					ea.Element.SetParent(this);
				}
				if(Nodes.Count > 1) {
					RepopulateSplitters();
				}
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				RepopulateSplitters();
			}
		}
		public void RepopulateSplitters() {
			ClearSplitters();
			PopulateSplitters();
		}
		void PopulateSplitters() {
			if(TabbedViewInfo == null) return;
			for(int i = 0; i < Nodes.Count - 1; i++) {
				var splitter = TabbedViewInfo.RegisterSplitter() as SplitterInfo;
				splitter.IsHorizontal = Orientation == Orientation.Horizontal;
				Splitters.Add(splitter);
				splitter.Node1 = Nodes[i];
				splitter.Node2 = Nodes[i + 1];
			}
		}
		void ClearSplitters() {
			foreach(SplitterInfo item in Splitters) {
				item.Node1 = null;
				item.Node2 = null;
				TabbedViewInfo.UnregisterSplitter(item);
			}
			Splitters.Clear();
		}
		internal List<ISplitterInfo> Splitters { get; set; }
		internal TabbedViewInfo TabbedViewInfo { get; set; }
		DockingContainer parentCore;
		[DefaultValue(null)]
		public DockingContainer Parent {
			get { return parentCore; }
			set {
				if(parentCore == value) return;
				if(parentCore != null) {
					parentCore.Nodes.Remove(this);
				}
				parentCore = value;
				if(parentCore != null) {
					parentCore.Nodes.Add(this);
				}
			}
		}
		public void SetParent(DockingContainer parent) {
			parentCore = parent;
		}
		public void Replace(DockingContainer newNode) {
			if(Parent == null) return;
			newNode.Length.UnitType = Length.UnitType;
			newNode.Length.UnitValue = Length.UnitValue;
			newNode.parentCore = Parent;
			int index = Parent.Nodes.IndexOf(this);
			Parent = null;
			newNode.Parent.Nodes.Insert(index, newNode);
		}
		Size defaultMinSize = new Size(10, 30);
		public Size GetMinSize(){
			if(Nodes.Count == 0) 
				return defaultMinSize;
			Size result = Size.Empty;
			foreach(var node in Nodes) {
				Size nodeMinSize = node.GetMinSize();
				if(Orientation == Orientation.Horizontal) {
					result.Width += nodeMinSize.Width;
					result.Height = Math.Max(nodeMinSize.Height, result.Height);
				}
				else {
					result.Width = Math.Max(nodeMinSize.Width, result.Width);
					result.Height += nodeMinSize.Height;
				}
			}
			return result;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DockingContainerCollection Nodes { get; set; }
		IRelativeLengthElement elementCore;
		[DefaultValue(null)]
		public IRelativeLengthElement Element {
			get { return elementCore; }
			set {
				elementCore = value;
				if(elementCore == null) return;
				if(elementCore.Length.UnitValue != 1)
					Length.UnitValue = elementCore.Length.UnitValue;
				elementCore.Length = Length;
			}
		}
		[DefaultValue(Orientation.Horizontal)]
		public Orientation Orientation { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Length Length { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ActualLength { get; set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle LayoutRect { get; set; }
		public void CaclLayout(Graphics g, Rectangle bounds) {
			if(Element != null) {
				var info = (Element as DocumentGroup).Info;
				if(info != null) {
					info.Calc(g, bounds);
					Element.ActualLength = ActualLength;
				}
			}
		}
		public bool IsDisposing {
			get { return false; }
		}
		bool disposed;
		public event EventHandler Disposed;
		public void Dispose() {
			if(!disposed) {
				disposed = true;
				Nodes.CollectionChanged -= NodesCollectionChanged;
				ClearSplitters();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Visible { get; set; }
		public void AddGroup(DocumentGroup newGroup) {
			DockingContainer newGroupNode = new DockingContainer() { Element = newGroup, TabbedViewInfo = this.TabbedViewInfo };
			newGroupNode.Parent = this;
		}
#if DEBUGTEST
		protected internal bool Contains(DockingContainer node) {
			foreach(DockingContainer watchingNode in Nodes) {
				if(watchingNode == node)
					return true;
				if(watchingNode.Contains(node))
					return true;
			}
			return false;
		}
		protected internal bool Contains(IList<DockingContainer> nodes, bool checkAll = true) {
			foreach(var node in nodes) {
				if(!checkAll && Contains(node)) return true;
				if(checkAll && !Contains(node)) return false;
			}
			return checkAll;
		}
#endif
	}
	class DockingContainerLengthHelper {
		public Rectangle GetRectangle(IList<IRelativeLengthElement> elements, Orientation orientation, int startIndex, int endIndex){
			Point location = elements[startIndex].LayoutRect.Location;
			Size totalSizeInPoints = Size.Empty;
			if(orientation == Orientation.Vertical) {
				totalSizeInPoints.Width = elements[0].LayoutRect.Width;
				totalSizeInPoints.Height = startIndex == endIndex ? elements[startIndex].LayoutRect.Height : elements[endIndex].LayoutRect.Bottom - elements[startIndex].LayoutRect.Top;
			}
			else {
				totalSizeInPoints.Width = startIndex == endIndex ? elements[startIndex].LayoutRect.Width : elements[endIndex].LayoutRect.Right - elements[startIndex].LayoutRect.Left;
				totalSizeInPoints.Height = elements[0].LayoutRect.Height;
			}
			return new Rectangle(location, totalSizeInPoints);
		}
		public Size GetPixels(DockingContainer container) {
			return GetPixels(container, 0, container.Nodes.Count - 1);
		}
		public Size GetPixels(DockingContainer container, int startIndex) {
			return GetPixels(container, startIndex, container.Nodes.Count - 1);
		}
		public Size GetPixels(DockingContainer container, int startIndex, int endIndex) {
			return GetPixels(container.Nodes.ToArray(), container.Orientation, startIndex, endIndex);
		}
		public Size GetPixels(IList<IRelativeLengthElement> elements, Orientation orientation) {
			return GetPixels(elements, orientation, 0, elements.Count - 1);
		}
		public Size GetPixels(IList<IRelativeLengthElement> elements, Orientation orientation, int startIndex, int endIndex) {
			return GetRectangle(elements, orientation, startIndex, endIndex).Size;
		}
		public double GetStars(DockingContainer element, double containerElementsCount) {
			return GetStars(element, element.Parent.LayoutRect.Size, containerElementsCount);
		}
		public double GetStars(DockingContainer element, Size containerSizeInPoints, double containerElementsCount) {
			return GetStars(element.Parent.Orientation, element.LayoutRect.Size, containerSizeInPoints, containerElementsCount, element.Length.UnitValue);
		}
		public double GetStars(Orientation orientation, Size elementSizeInPoints, Size containerSizeInPoints, double containerElementsCount, double defaultLength = 1.0) {
			if(orientation == Orientation.Horizontal) {
				if(elementSizeInPoints.Width == 0 || containerSizeInPoints.Width == 0)
					return defaultLength;
				return elementSizeInPoints.Width / (double)containerSizeInPoints.Width * containerElementsCount;
			}
			else {
				if(elementSizeInPoints.Height == 0 || containerSizeInPoints.Height == 0)
					return defaultLength;
				return elementSizeInPoints.Height / (double)containerSizeInPoints.Height * containerElementsCount;
			}
		}
	}
	public class LayoutGroupLengthHelper {
		public static void CalcActualGroupLength(int length, IEnumerable<IRelativeLengthElement> collection, bool allowChangeGroupVisibility = true) {
			int remainingLength = length;
			double sectionCount = 0;
			List<IRelativeLengthElement> groupsWithRelativeLength = new List<IRelativeLengthElement>();
			List<IRelativeLengthElement> groupsWithFixedLength = new List<IRelativeLengthElement>();
			foreach(IRelativeLengthElement group in collection) {
				if(group.Length.UnitType == LengthUnitType.Pixel) {
					group.ActualLength = (int)group.Length.UnitValue;
					remainingLength -= group.ActualLength;
					groupsWithFixedLength.Add(group);
				}
				else {
					groupsWithRelativeLength.Add(group);
					sectionCount += group.Length.UnitValue;
				}
			}
			CalcGroupVisibility(remainingLength, sectionCount, groupsWithRelativeLength, groupsWithFixedLength, allowChangeGroupVisibility);
		}
		static void CalcGroupVisibility(int remainingLength, double sectionCount, List<IRelativeLengthElement> groupsWithRelativeLength, List<IRelativeLengthElement> groupsWithFixedLength, bool allowChangeGroupVisibility) {
			List<IRelativeLengthElement> visibleGroups = new List<IRelativeLengthElement>();
			foreach(IRelativeLengthElement group in groupsWithRelativeLength) {
				if(!allowChangeGroupVisibility) {
					group.Visible = true;
					visibleGroups.Add(group);
					continue;
				}
				int newActualLength = (int)(remainingLength * (group.Length.UnitValue / sectionCount));
				if(newActualLength > 60) {
					group.Visible = true;
					visibleGroups.Add(group);
				}
				else {
					group.Visible = false;
					group.ActualLength = 0;
					sectionCount -= group.Length.UnitValue;
				}
			}
			foreach(IRelativeLengthElement group in visibleGroups) {
				group.ActualLength = (int)(remainingLength * (group.Length.UnitValue / sectionCount));
			}
		}
		static int PositiveCompare(IRelativeLengthElement x, IRelativeLengthElement y) {
			if(x == y) return 0;
			if(x == null || y == null)
				return 0;
			return x.Length.UnitValue.CompareTo(y.Length.UnitValue);
		}
	}
	public class DockingContainerCollection : BaseMutableListEx<DockingContainer> {
		public bool Insert(int index, DockingContainer container) {
			return InsertCore(index, container);
		}
	}
	public static class DockingContainerHelper {
		static DockingContainerLengthHelper lengthHelper = new DockingContainerLengthHelper();
		public static void RemoveDocumentGroup(DocumentGroup group, DockingContainer rootNode) {
			DockingContainer targetNode = GetTargetNode(group, rootNode);
			if(targetNode == null) return;
			var parent = targetNode.Parent;
			targetNode.Parent = null;
			if(parent == null) return;
			if(parent.Nodes.Count == 1) {
				DockingContainer lastNode = parent.Nodes[0];
				parent.Replace(lastNode);
				if(lastNode.Nodes.Count != 0 && lastNode.Orientation == lastNode.Parent.Orientation) {
					ReplaceContainerOnChildren(lastNode, lastNode.Nodes.ToArray());
				}
			}
			if(parent.Nodes.Count == 0) {
				parent.Parent = null;
				parent = null;
			}
		}
		public static void CorrectContainerNodesRelativeLength(DockingContainer container) {
			double sectionCount = 0;
			foreach(var node in container.Nodes) {
				sectionCount += node.Length.UnitValue;
			}
			foreach(var node in container.Nodes) {
				node.Length.UnitValue = (node.Length.UnitValue / sectionCount) * container.Nodes.Count; 
			}
		}
		public static void ReplaceContainerOnChildren(DockingContainer container) {
			ReplaceContainerOnChildren(container, container.Nodes.ToArray());
		}
		public static void ReplaceContainerOnChildren(DockingContainer container, IList<DockingContainer> children) {
			DockingContainer parent = container.Parent;
			int insertIndex = parent.Nodes.IndexOf(container);
			while(container.Nodes.Count != 0) {
				DockingContainer node = container.Nodes[0];
				node.Parent = null;
			}
			container.Parent = null;
			container.Dispose();
			int totalCount = parent.Nodes.Count + children.Count;
			foreach(DockingContainer node in parent.Nodes)
				node.Length.UnitValue = lengthHelper.GetStars(node, totalCount);
			for(int i = 0; i < children.Count; i++) {
				DockingContainer child = children[i];
				parent.Nodes.Insert(insertIndex + i, child);
				child.Length.UnitValue = lengthHelper.GetStars(child, totalCount);
			}
		}
		public static void ReplaceChildrenOnContainer(IList<DockingContainer> replacableChildren, IList<DockingContainer> containerChildren) {
			DockingContainer targetNode = replacableChildren[0].Parent;
			int mainContainerInsertIndex = targetNode.Nodes.IndexOf(replacableChildren[0]);
			Orientation mainContainerOrientation = targetNode.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
			DockingContainer mainContainer = new DockingContainer() { TabbedViewInfo = targetNode.TabbedViewInfo, Orientation = mainContainerOrientation };
			mainContainer.Nodes.AddRange(containerChildren);
			Size containerSize = lengthHelper.GetPixels((IList<IRelativeLengthElement>)replacableChildren, targetNode.Orientation);
			foreach(var replacableChild in replacableChildren) {
				replacableChild.Parent = null;
				replacableChild.Dispose();
			}
			int totalCount = targetNode.Nodes.Count + containerChildren.Count;
			mainContainer.Length.UnitValue = lengthHelper.GetStars(targetNode.Orientation, containerSize, targetNode.LayoutRect.Size, totalCount);
			foreach(var node in targetNode.Nodes)
				node.Length.UnitValue = lengthHelper.GetStars(targetNode.Orientation, node.LayoutRect.Size, targetNode.LayoutRect.Size, totalCount);
			targetNode.Nodes.Insert(mainContainerInsertIndex, mainContainer);
		}
		public static bool ShouldReplaceContainerOnChildren(IList<DockingContainer> containerNodes, IList<DockingContainer> children) {
			DockingContainer container = containerNodes[0].Parent;
			return !ContainerContainsOtherNodesExceptOf(containerNodes, container) && container.Parent != null && container.Parent.Orientation != children[0].Orientation;
		}
		public static bool ContainerContainsOtherNodesExceptOf(IList<DockingContainer> exceptableNodes, DockingContainer container) {
			if(container.Nodes.Count != exceptableNodes.Count) return true;
			foreach(DockingContainer exceptableNode in exceptableNodes) {
				if(!container.Nodes.Contains(exceptableNode))
					return true;
			}
			return false;
		}
		public static void AddToGroup(IRelativeLengthElement targetGroup, IRelativeLengthElement newGroup, DockingContainer rootNode, Orientation orientation, bool after) {
			DockingContainer targetNode = GetTargetNode(targetGroup, rootNode);
			DockingContainer newGroupNode = new DockingContainer() { Element = newGroup, TabbedViewInfo = rootNode.TabbedViewInfo };
			DockingContainer currentNode = GetTargetNode(newGroup, rootNode);
			if(currentNode != null) {
				currentNode.Parent = null;
			}
			if(targetNode != null) {
				if(targetNode.Parent != null && targetNode.Parent.Orientation == orientation) {
					int index = targetNode.Parent.Nodes.IndexOf(targetNode);
					index += after ? 1 : 0;
					newGroupNode.SetParent(targetNode.Parent);
					newGroupNode.Parent.Nodes.Insert(index, newGroupNode);
					return;
				}
				DockingContainer parentNode = new DockingContainer() { TabbedViewInfo = rootNode.TabbedViewInfo };
				parentNode.Orientation = orientation;
				targetNode.Replace(parentNode);
				if(after) {
					targetNode.Parent = parentNode;
					newGroupNode.Parent = parentNode;
				}
				else {
					newGroupNode.Parent = parentNode;
					targetNode.Parent = parentNode;
				}
			}
		}
		public static void AddToGroup(DocumentGroup targetGroup, DocumentGroup newGroup, DockingContainer rootNode, Orientation orientation) {
			AddToGroup(targetGroup, newGroup, rootNode, orientation, true);
		}
		public static DockingContainer[] GetNeightbors(DockingContainer container) {
			DockingContainer parent = container.Parent;
			if(parent == null || parent.Nodes.Count == 1) return new DockingContainer[0];
			int containerIndex = parent.Nodes.IndexOf(container);
			if(containerIndex == 0)
				return new[] { parent.Nodes[containerIndex + 1] };
			if(containerIndex == parent.Nodes.Count - 1)
				return new[] { parent.Nodes[containerIndex - 1] };
			return new[] { parent.Nodes[containerIndex - 1], parent.Nodes[containerIndex + 1] };
		}
		internal static DockingContainer GetTargetNode(IRelativeLengthElement targetGroup, DockingContainer node) {
			if(node.Element == targetGroup)
				return node;
			DockingContainer result = null;
			if(node.Nodes.Count != 0) {
				foreach(var item in node.Nodes) {
					result = GetTargetNode(targetGroup, item);
					if(result != null)
						return result;
				}
			}
			return result;
		}
	}
}
