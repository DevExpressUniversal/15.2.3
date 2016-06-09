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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraBars.Docking2010.Views.Widget;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public interface ISplitterInfo : IBaseSplitterInfo, IDockingAdornerInfo, IUIElement {
		int CalcMinSize(Graphics g);
		ObjectState State { get; set; }
		int SplitIndex { get; set; }
		void MoveSplitter(int change);
		void BeginSplit(Adorner adorner);
		void UpdateSplit(Adorner adorner, int change);
		void ResetSplit(Adorner adorner);
	}
#if DEBUGTEST
	public interface IResizeAssistentInfo : IBaseElementInfo, IUIElement {
		void Show(Adorner adorner);
		void ChangeContainersResizeZoneType();
		void Hide(Adorner adorner);
	}
	public class ResizeAssistentAdornerInfoArgs : AdornerElementInfoArgs {
		public bool IsHorizontal { get; set; }
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			Rectangle rect = Bounds;
			rect.Inflate(new Size(20, 20));
			return new Rectangle[] { rect };
		}
		protected override int CalcCore() {
			return 0;
		}
	}
	public class ResizeAssistentAdornerPainter : AdornerPainter {
		public override void DrawObject(ObjectInfoArgs e) {
		}
	}
	class ResizeAssistentInfo : BaseElementInfo, IResizeAssistentInfo {
		DockingContainer[] splittableContainersCore;
		FourAdjacentContainersInfo quadCrossContainerInfoCore;
		public ResizeAssistentInfo(TabbedView view)
			: base(view) {
		}
		protected override void OnDispose() {
			quadCrossContainerInfoCore = null;
			base.OnDispose();
		}
		public FourAdjacentContainersInfo FourAdjacentContainersInfo {
			get { return quadCrossContainerInfoCore; }
			internal set {
				quadCrossContainerInfoCore = value;
				Bounds = quadCrossContainerInfoCore.CrossRect;
			}
		}
		public DockingContainer[] SplittableContainers { get { return splittableContainersCore; } internal set { splittableContainersCore = value; } }
		public Orientation Orientation { get; set; }
		public TabbedViewInfo ViewInfo { get { return (Owner as TabbedView).ViewInfo as TabbedViewInfo; } }
		protected AdornerElementInfo AdornerInfo { get; set; }
		public virtual void Show(Adorner adorner) {
			if(FourAdjacentContainersInfo == null) return;
			if(AdornerInfo == null || !adorner.Elements.Contains(AdornerInfo)) {
				Cursor.Current = Cursors.Default;
				ResizeAssistentAdornerInfoArgs infoArgs = new ResizeAssistentAdornerInfoArgs();
				AdornerInfo = new AdornerElementInfo(new ResizeAssistentAdornerPainter(), infoArgs);
			}
			AdornerInfo.InfoArgs.Bounds = Bounds;
			(AdornerInfo.InfoArgs as ResizeAssistentAdornerInfoArgs).IsHorizontal = Orientation == Orientation.Vertical;
			adorner.Invalidate();
			adorner.Show(AdornerInfo);
		}
		public virtual void Hide(Adorner adorner) {
			adorner.Hide();
			adorner.Reset(AdornerInfo);
			AdornerInfo = null;
			adorner.Invalidate();
		}
		Orientation ToggleOrientation(Orientation orientation) {
			return orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
		}
		public virtual void ChangeContainersResizeZoneType() {
			bool canChangeResizeType = true;
			var parentsWithChildren = new Dictionary<DockingContainer, List<DockingContainer>>();
			foreach(var splittableContainer in SplittableContainers) {
				var adjacentContainersPair = new List<DockingContainer>();
				foreach(var adjacentContainer in FourAdjacentContainersInfo.Containers) {
					DockingContainer parent = adjacentContainer.Parent;
					while(parent != null) {
						if(parent == splittableContainer)
							adjacentContainersPair.Add(adjacentContainer);
						parent = parent.Parent;
					}
				}
				parentsWithChildren.Add(splittableContainer, adjacentContainersPair);
			}
			int[] splitIndexes = new int[SplittableContainers.Length];
			for(int i = 0; i < SplittableContainers.Length; i++) {
				DockingContainer splittableContainer = SplittableContainers[i];
				List<DockingContainer> containers = parentsWithChildren[splittableContainer];
				if(containers.Count < 2) {
					canChangeResizeType = false;
					break;
				}
				Stack<DockingContainer> pathToAdjacentContainer1 = GetPath(containers[0], splittableContainer);
				Stack<DockingContainer> pathToAdjacentContainer2 = GetPath(containers[1], splittableContainer);
				DockingContainer[] generalPathNodes = pathToAdjacentContainer1.Intersect(pathToAdjacentContainer2).ToArray();
				while(generalPathNodes.Length != 0) {
					if(!TryChangeChildrenContainersResizeZoneType(generalPathNodes[0], generalPathNodes[1], containers)) {
						canChangeResizeType = false;
						break;
					}
					pathToAdjacentContainer1 = GetPath(containers[0], splittableContainer);
					pathToAdjacentContainer2 = GetPath(containers[1], splittableContainer);
					generalPathNodes = pathToAdjacentContainer1.Intersect(pathToAdjacentContainer2).ToArray();
				}
				if(canChangeResizeType) {
					int index1 = splittableContainer.Nodes.IndexOf(pathToAdjacentContainer1.Count == 0 ? containers[0] : pathToAdjacentContainer1.Peek());
					int index2 = splittableContainer.Nodes.IndexOf(pathToAdjacentContainer2.Count == 0 ? containers[1] : pathToAdjacentContainer2.Peek());
					splitIndexes[i] = Math.Min(index1, index2);
				}
			}
			if(canChangeResizeType)
				ChangeContainersResizeZoneOrientationCore(SplittableContainers, splitIndexes);
			Owner.Manager.LayoutChanged();
		}
		bool TryChangeChildrenContainersResizeZoneType(DockingContainer currentParent, DockingContainer nextParent, IList<DockingContainer> adjacentContainers) {
			const int splittableNodesCount = 2;
			DockingContainer[] splittableNodes = new DockingContainer[splittableNodesCount];
			bool splitIndexesIsValid = false;
			int[] splitIndexes = new int[splittableNodesCount];
			foreach(var neightborContainer in DockingContainerHelper.GetNeightbors(nextParent)) {
				int neightborContainerIndex = currentParent.Nodes.IndexOf(neightborContainer);
				int splittableContainerIndex = currentParent.Nodes.IndexOf(nextParent);
				if(neightborContainerIndex < splittableContainerIndex)
					splittableNodes = new[] { neightborContainer, nextParent };
				else
					splittableNodes = new[] { nextParent, neightborContainer };
				splitIndexes = GetSplitIndexes(splittableNodes, adjacentContainers);
				if(ValidateSplitIndexes(splitIndexes, splittableNodes)) {
					splitIndexesIsValid = true;
					break;
				}
			}
			if(!splitIndexesIsValid) return false;
			int leftSideIndex = currentParent.Nodes.IndexOf(splittableNodes.First());
			ChangeContainersResizeZoneOrientationCore(splittableNodes, splitIndexes);
			for(int i = leftSideIndex; i > 0; i--) {
				splittableNodes = new[] { currentParent.Nodes[i - 1], currentParent.Nodes[i] };
				splitIndexes = GetSplitIndexes(splittableNodes, adjacentContainers);
				if(!ValidateSplitIndexes(splitIndexes, splittableNodes)) return false;
				ChangeContainersResizeZoneOrientationCore(splittableNodes, splitIndexes);
			}
			while(currentParent.Nodes.Count > 1) {
				splittableNodes = new[] { currentParent.Nodes[0], currentParent.Nodes[1] };
				splitIndexes = GetSplitIndexes(splittableNodes, adjacentContainers);
				if(!ValidateSplitIndexes(splitIndexes, splittableNodes)) return false;
				ChangeContainersResizeZoneOrientationCore(splittableNodes, splitIndexes);
			}
			return true;
		}
		int[] GetSplitIndexes(IList<DockingContainer> splittableContainers, IList<DockingContainer> adjacentContainers) {
			DockingContainer adjacentContainersHierarchyParent = null;
			DockingContainer splittableContainer = null;
			foreach(var container in splittableContainers) {
				if(container.Contains(adjacentContainers))
					adjacentContainersHierarchyParent = container;
				else
					splittableContainer = container;
			}
			int comparableNodeIndex =
				adjacentContainersHierarchyParent.Nodes
					.Where(node => node.Contains(adjacentContainers, false) || adjacentContainers.Contains(node))
					.Select(node => adjacentContainersHierarchyParent.Nodes.IndexOf(node))
					.OrderBy(i => i)
					.First();
			if(comparableNodeIndex == adjacentContainersHierarchyParent.Nodes.Count - 1) comparableNodeIndex--;
			DockingContainer comparableNode = adjacentContainersHierarchyParent.Nodes[comparableNodeIndex];
			DockingContainer nodeWithSameLocation = GetNodeWithSameLocation(splittableContainer, comparableNode);
			int splittableContainerSplitIndex = nodeWithSameLocation != null ? splittableContainer.Nodes.IndexOf(nodeWithSameLocation) : -1;
			int[] result = new int[splittableContainers.Count];
			result[splittableContainers.IndexOf(adjacentContainersHierarchyParent)] = comparableNodeIndex;
			result[splittableContainers.IndexOf(splittableContainer)] = splittableContainerSplitIndex;
			return result;
		}
		bool ValidateSplitIndexes(int[] splitIndexes, DockingContainer[] splittableContainers) {
			for(int i = 0; i < splittableContainers.Length; i++) {
				int verifiableIndex = splitIndexes[i];
				if(!splittableContainers.IsValidIndex(verifiableIndex))
					return false;
			}
			return true;
		}
		DockingContainer GetNodeWithSameLocation(DockingContainer nodesParent, DockingContainer container) {
			foreach(DockingContainer node in nodesParent.Nodes) {
				if(nodesParent.Orientation == Orientation.Horizontal && CheckSizesEquality(node.LayoutRect.Right, container.LayoutRect.Right)) return node;
				if(nodesParent.Orientation == Orientation.Vertical && CheckSizesEquality(node.LayoutRect.Bottom, container.LayoutRect.Bottom)) return node;
			}
			return null;
		}
		bool CheckSizesEquality(int size1, int size2) {
			int delta = Math.Abs(size1 - size2);
			return delta <= 3;
		}
		Stack<DockingContainer> GetPath(DockingContainer container, DockingContainer startContainer) {
			var parentsStack = new Stack<DockingContainer>();
			DockingContainer parent = container.Parent;
			while(parent != null && parent != startContainer) {
				parentsStack.Push(parent);
				parent = parent.Parent;
			}
			return parentsStack;
		}
		protected class MoveOperationArgs {
			public DockingContainer DestContainer { get; private set; }
			public IList<DockingContainer> MovableNodes { get; private set; }
			public DockingContainer SplittableContainer { get; private set; }
			public Size SplittableContainerSizeInPixels { get; private set; }
			public Size SplittableContainersSizeInPixels { get; private set; }
			public Size MovableNodesTotalSizeInPixels { get; private set; }
			public double SplittableContainersSectionCount { get; private set; }
			public static MoveOperationArgs Create(DockingContainer destContainer, IList<DockingContainer> splittableContainers, DockingContainer splittableContainer, int[] splitRange) {
				DockingContainerLengthHelper lengthHelper = new DockingContainerLengthHelper();
				MoveOperationArgs args = new MoveOperationArgs();
				args.SplittableContainer = splittableContainer;
				args.DestContainer = destContainer;
				args.MovableNodes = new List<DockingContainer>();
				for(int i = 0; i < args.SplittableContainer.Nodes.Count; i++) {
					if(i >= splitRange[0] && i <= splitRange[1])
						args.MovableNodes.Add(args.SplittableContainer.Nodes[i]);
				}
				args.SplittableContainerSizeInPixels = lengthHelper.GetPixels(args.SplittableContainer);
				args.MovableNodesTotalSizeInPixels = lengthHelper.GetPixels(args.SplittableContainer, splitRange[0], splitRange[1]);
				foreach(var container in splittableContainers)
					args.SplittableContainersSectionCount += CalcElementsCount(container);
				args.SplittableContainersSizeInPixels = lengthHelper.GetPixels((IList<IRelativeLengthElement>)splittableContainers, args.SplittableContainer.Parent.Orientation);
				return args;
			}
			static int CalcElementsCount(DockingContainer container) {
				int result = 0;
				if(container.Nodes[0].Element != null)
					result++;
				foreach(var node in container.Nodes[0].Nodes)
					result++;
				return result;
			}
		}
		DockingContainerLengthHelper lengthHelper = new DockingContainerLengthHelper();
		protected virtual void ChangeContainersResizeZoneOrientationCore(IList<DockingContainer> splittableContainers, IList<int> splitIndexes) {
			DockingContainer parentNode = splittableContainers[0].Parent;
			IList<DockingContainer> destContainers = CreateContainers(2, parentNode.Orientation);
			IList<MoveOperationArgs> moveOperationsArgs = CreateArgsForMoveOperations(splittableContainers, splitIndexes, destContainers);
			foreach(MoveOperationArgs operationArgs in moveOperationsArgs)
				DoMoveOperation(operationArgs);
			foreach(var container in destContainers)
				container.LayoutRect = new Rectangle(container.LayoutRect.Location, lengthHelper.GetPixels(container));
			if(DockingContainerHelper.ShouldReplaceContainerOnChildren(splittableContainers, destContainers))
				DockingContainerHelper.ReplaceContainerOnChildren(parentNode, destContainers);
			else if(DockingContainerHelper.ContainerContainsOtherNodesExceptOf(splittableContainers, parentNode))
				DockingContainerHelper.ReplaceChildrenOnContainer(splittableContainers, destContainers);
			else {
				parentNode.Orientation = ToggleOrientation(parentNode.Orientation);
				parentNode.Nodes.AddRange(destContainers);
				foreach(var container in splittableContainers)
					container.Parent = null;
			}
			foreach(var splittableContainer in splittableContainers)
				splittableContainer.Dispose();
			ViewInfo.CalcContainers();
		}
		protected virtual IList<MoveOperationArgs> CreateArgsForMoveOperations(IList<DockingContainer> splittableContainers, IList<int> splitIndexes, IList<DockingContainer> destContainers) {
			List<MoveOperationArgs> moveOperationsArgs = new List<MoveOperationArgs>();
			for(int i = 0; i < splittableContainers.Count; i++) {
				DockingContainer splittableContainer = splittableContainers[i];
				int splitIndex = splitIndexes[i];
				for(int j = 0; j < destContainers.Count; j++) {
					int[] splitRange = j == 0 ? new[] { 0, splitIndex } : new[] { splitIndex + 1, splittableContainer.Nodes.Count - 1 };
					MoveOperationArgs args = MoveOperationArgs.Create(destContainers[j], splittableContainers, splittableContainer, splitRange);
					moveOperationsArgs.Add(args);
				}
			}
			return moveOperationsArgs;
		}
		protected virtual void DoMoveOperation(MoveOperationArgs args) {
			args.DestContainer.Length.UnitValue = lengthHelper.GetStars(ToggleOrientation(args.DestContainer.Orientation), args.MovableNodesTotalSizeInPixels, args.SplittableContainerSizeInPixels, 2.0);
			if(args.MovableNodes.Count == 1) {
				DockingContainer node = args.MovableNodes[0];
				if(node.Nodes.Count > 0 && node.Orientation == args.DestContainer.Orientation) {
					while(node.Nodes.Count != 0) {
						DockingContainer extractableNode = node.Nodes[0];
						extractableNode.Length.UnitValue = lengthHelper.GetStars(extractableNode, args.SplittableContainersSizeInPixels, args.SplittableContainersSectionCount);
						extractableNode.Parent = args.DestContainer;
					}
				}
				else {
					node.Parent = args.DestContainer;
					node.Length.UnitValue = lengthHelper.GetStars(node, args.SplittableContainersSizeInPixels, args.SplittableContainersSectionCount);
				}
			}
			else {
				DockingContainer innerContainer = CreateContainerInstance(ToggleOrientation(args.DestContainer.Orientation));
				innerContainer.Length.UnitValue = lengthHelper.GetStars(args.DestContainer.Orientation, args.MovableNodesTotalSizeInPixels, args.SplittableContainersSizeInPixels, args.SplittableContainersSectionCount);
				foreach(var node in args.MovableNodes) {
					node.Parent = innerContainer;
					node.Length.UnitValue = lengthHelper.GetStars(node, args.MovableNodesTotalSizeInPixels, args.MovableNodes.Count);
				}
				innerContainer.Parent = args.DestContainer;
				innerContainer.LayoutRect = new Rectangle(Point.Empty, lengthHelper.GetPixels(innerContainer));
			}
		}
		DockingContainer[] CreateContainers(int count, Orientation orientation) {
			DockingContainer[] containers = new DockingContainer[count];
			for(int i = 0; i < containers.Length; i++)
				containers[i] = CreateContainerInstance(orientation);
			return containers;
		}
		protected virtual DockingContainer CreateContainerInstance(Orientation orientation, Length length = null) {
			DockingContainer container = new DockingContainer();
			container.Orientation = orientation;
			container.TabbedViewInfo = ViewInfo;
			if(length != null) {
				container.Length.UnitValue = length.UnitValue;
				container.Length.UnitType = length.UnitType;
			}
			return container;
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) { }
		protected override bool CalcIsVisible() {
			return !Bounds.IsEmpty;
		}
		public override System.Type GetUIElementKey() {
			return typeof(IResizeAssistentInfo);
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
	}
	class FourAdjacentContainersInfo {
		public DockingContainer[] Containers { get; set; }
		public Rectangle CrossRect { get { return CalcCrossRectangle(); } }
		public Rectangle CalcCrossRectangle() {
			Rectangle result = Rectangle.Empty;
			if(!Containers.All(i => i != null)) return result;
			List<DockingContainer> containers = new List<DockingContainer>();
			result.Location = new Point(Containers[0].LayoutRect.Right, Containers[0].LayoutRect.Bottom);
			result.Size = new Size(Containers[1].LayoutRect.Left - Containers[0].LayoutRect.Right, Containers[2].LayoutRect.Top - Containers[0].LayoutRect.Bottom);
			return result;
		}
		enum EdgeType {
			Left, Top, Right, Bottom
		}
		struct CoordinateInfo<T> {
			public T CoordinateOwner { get; set; }
			public EdgeType ContainingEdge { get; set; }
		}
		public static IList<FourAdjacentContainersInfo> FindFourAdjacentContainers(DockingContainer rootContainer) {
			IDictionary<int, List<CoordinateInfo<DockingContainer>>> xCoordinatesWithOwners, yCoordinatesWithOwners;
			IDictionary<DockingContainer, Rectangle> containerWithRectPairs = GetContainerAndLayoutRectPairs(rootContainer);
			CreateCoordinatesDictionaries(containerWithRectPairs, out xCoordinatesWithOwners, out yCoordinatesWithOwners);
			List<FourAdjacentContainersInfo> fourAdjacentContainersCollection = new List<FourAdjacentContainersInfo>();
			foreach(var y in yCoordinatesWithOwners.Keys) {
				foreach(var x in xCoordinatesWithOwners.Keys) {
					FourAdjacentContainersInfo info = new FourAdjacentContainersInfo();
					IEnumerable<DockingContainer> xCoordinateOwners = xCoordinatesWithOwners[x].Select(ownerInfo => ownerInfo.CoordinateOwner);
					IEnumerable<DockingContainer> yCoordinateOwners = yCoordinatesWithOwners[y].Select(ownerInfo => ownerInfo.CoordinateOwner);
					IEnumerable<DockingContainer> intersectResultCollection = xCoordinateOwners.Intersect(yCoordinateOwners);
					if(intersectResultCollection.Count() == 4) {
						var intersectCoordinateInfos = new List<CoordinateInfo<DockingContainer>>();
						intersectCoordinateInfos.AddRange(xCoordinatesWithOwners[x]);
						intersectCoordinateInfos.AddRange(yCoordinatesWithOwners[y]);
						info.Containers = ArrangeFourAdjacentContainers(intersectResultCollection, intersectCoordinateInfos.Where(i => intersectResultCollection.Contains(i.CoordinateOwner)));
						fourAdjacentContainersCollection.Add(info);
					}
				}
			}
			return fourAdjacentContainersCollection;
		}
		static DockingContainer[] ArrangeFourAdjacentContainers(IEnumerable<DockingContainer> adjacentContainers, IEnumerable<CoordinateInfo<DockingContainer>> intersectCoordinatesInfos) {
			DockingContainer[] result = new DockingContainer[4];
			foreach(var container in adjacentContainers) {
				EdgeType[] pointType = intersectCoordinatesInfos.Where(ci => ci.CoordinateOwner == container).Select(ci => ci.ContainingEdge).ToArray();
				if(pointType.Contains(EdgeType.Right) && pointType.Contains(EdgeType.Bottom))
					result[0] = container;
				if(pointType.Contains(EdgeType.Left) && pointType.Contains(EdgeType.Bottom))
					result[1] = container;
				if(pointType.Contains(EdgeType.Right) && pointType.Contains(EdgeType.Top))
					result[2] = container;
				if(pointType.Contains(EdgeType.Left) && pointType.Contains(EdgeType.Top))
					result[3] = container;
			}
			return result.ToArray();
		}
		static IDictionary<DockingContainer, Rectangle> GetContainerAndLayoutRectPairs(DockingContainer rootContainer) {
			Rectangle bounds = rootContainer.TabbedViewInfo.Bounds;
			IDictionary<DockingContainer, Rectangle> containerAndLayoutRectPairs = CalcContainers(bounds, rootContainer);
			IDictionary<DockingContainer, Rectangle> result = new Dictionary<DockingContainer, Rectangle>();
			foreach(var pair in containerAndLayoutRectPairs) {
				DockingContainer container = pair.Key;
				if(container.Element != null && container.Nodes.Count == 0)
					result.Add(pair);
			}
			return result;
		}
		static IDictionary<DockingContainer, Rectangle> CalcContainers(Rectangle bounds, DockingContainer node, IDictionary<DockingContainer, Rectangle> pairs = null) {
			if(pairs == null)
				pairs = new Dictionary<DockingContainer, Rectangle>();
			CalcContainerNodes(bounds, node, node.Orientation == Orientation.Horizontal, pairs);
			foreach(var item in node.Nodes)
				CalcContainers(pairs[item], item, pairs);
			return pairs;
		}
		static void CalcContainerNodes(Rectangle bounds, DockingContainer node, bool isHorizontal, IDictionary<DockingContainer, Rectangle> nodesRects) {
			int length = isHorizontal ? bounds.Width : bounds.Height ;
			LayoutGroupLengthHelper.CalcActualGroupLength(length, node.Nodes, false);
			Point offset = bounds.Location;
			foreach(var item in node.Nodes) {
				Size layoutSize = isHorizontal ? new Size(item.ActualLength, bounds.Height) : new Size(bounds.Width, item.ActualLength);
				Rectangle layoutRect = new Rectangle(offset, layoutSize);
				nodesRects.Add(item, layoutRect);
				if(isHorizontal)
					offset.X += item.ActualLength;
				else
					offset.Y += item.ActualLength;
			}
		}
		static void CreateCoordinatesDictionaries(IDictionary<DockingContainer, Rectangle> containersWithRects, out IDictionary<int, List<CoordinateInfo<DockingContainer>>> xCoordinatesDict, out IDictionary<int, List<CoordinateInfo<DockingContainer>>> yCoordinatesDict) {
			xCoordinatesDict = new SortedDictionary<int, List<CoordinateInfo<DockingContainer>>>();
			yCoordinatesDict = new SortedDictionary<int, List<CoordinateInfo<DockingContainer>>>();
			foreach(var container in containersWithRects.Keys)
				AddCoordinatesInDictionaries(containersWithRects[container], container, xCoordinatesDict, yCoordinatesDict);
			Normalize(xCoordinatesDict);
			Normalize(yCoordinatesDict);
		}
		static void AddCoordinatesInDictionaries<T>(Rectangle rect, T rectOwner, IDictionary<int, List<CoordinateInfo<T>>> xCoordinatesDict, IDictionary<int, List<CoordinateInfo<T>>> yCoordinatesDict) {
			var coordinateLocationTypes = (EdgeType[])Enum.GetValues(typeof(EdgeType));
			foreach(EdgeType coordinateLocationType in coordinateLocationTypes) {
				int coordinate = GetCoordinateByType(rect, coordinateLocationType);
				var coordinateOwner = new CoordinateInfo<T>() { CoordinateOwner = rectOwner, ContainingEdge = coordinateLocationType };
				var coordinatesDict = coordinateLocationType == EdgeType.Left || coordinateLocationType == EdgeType.Right ? xCoordinatesDict : yCoordinatesDict;
				AddCoordinateInDictionary(coordinate, coordinateOwner, coordinatesDict);
			}
		}
		static int GetCoordinateByType(Rectangle rect, EdgeType coordinateType) {
			switch(coordinateType) {
				case EdgeType.Top: return rect.Top;
				case EdgeType.Right: return rect.Right;
				case EdgeType.Bottom: return rect.Bottom;
				default: return rect.Left;
			}
		}
		static void AddCoordinateInDictionary<T>(int coordinate, T owner, IDictionary<int, List<T>> coordinatesHeap) {
			if(!coordinatesHeap.ContainsKey(coordinate)) {
				var ownersList = new List<T>();
				ownersList.Add(owner);
				coordinatesHeap.Add(coordinate, ownersList);
			}
			else {
				List<T> coordinateOwners = coordinatesHeap[coordinate];
				coordinateOwners.Add(owner);
			}
		}
		static void Normalize<T>(IDictionary<int, List<T>> coordinateWithOwnerPairList) {
			for(int i = coordinateWithOwnerPairList.Keys.Count - 1; i > 0; i--) {
				int coordinate = coordinateWithOwnerPairList.Keys.ToArray()[i];
				int neighborCoordinate = coordinateWithOwnerPairList.Keys.ToArray()[i - 1];
				if(coordinate - neighborCoordinate <= 5) {
					List<T> coordinateOwners = coordinateWithOwnerPairList[coordinate];
					coordinateWithOwnerPairList.Remove(coordinate);
					coordinateWithOwnerPairList[neighborCoordinate].AddRange(coordinateOwners);
				}
			}
		}
	}
	class StickySplitterInfo : SplitterInfo {
		bool useSticky;
		int stickyDelta;
		public StickySplitterInfo(TabbedView view): base(view) { }
		public virtual int DefaultStickyZoneTickness { get { return 30; } }
		protected virtual bool CheckSticky(int change, DockingContainer nodeParent) {
			if(change == 0 || nodeParent.Parent == null || nodeParent.Parent.Nodes.Count < 2) return false;
			int currentPositionCoordinate = IsHorizontal ? Bounds.X + change : Bounds.Y + change;
			int startPositionCoordinate = IsHorizontal ? Bounds.X : Bounds.Y;
			Size[] stickyRanges = GetStickyRanges(nodeParent);
			Size hitRange;
			if(DoHitToStickyRanges(currentPositionCoordinate, stickyRanges, out hitRange)) {
				stickyDelta = (hitRange.Width + DefaultStickyZoneTickness / 2) - startPositionCoordinate;
				return true;
			}
			return false;
		}
		bool DoHitToStickyRanges(int coordinate, Size[] stickyRanges, out Size resultRange) {
			resultRange = Size.Empty;
			foreach(Size stickyRange in stickyRanges) {
				if(coordinate >= stickyRange.Width && coordinate <= stickyRange.Height) {
					resultRange = stickyRange;
					return true;
				}
			}
			return false;
		}
		Size[] GetStickyRanges(DockingContainer container) {
			List<Size> ranges = new List<Size>();
			Size excludableRange = CalcStickyRange(Node1.LayoutRect);
			List<DockingContainer> prevNodes = new List<DockingContainer>();
			do {
				CalcStickyRanges(container, ranges, excludableRange, prevNodes);
				prevNodes.Add(container);
				container = container.Parent;
			} 
			while(container != null);
			return ranges.ToArray();
		}
		void CalcStickyRanges(DockingContainer container, List<Size> ranges, Size excludableRange, List<DockingContainer> prevNodes) {
			foreach(var node in container.Nodes) {
				if(prevNodes.Contains(node)) continue;
				if(node.Element == null)
					CalcStickyRanges(node, ranges, excludableRange, prevNodes);
				else {
					Size range = CalcStickyRange(node.LayoutRect);
					if(!ranges.Contains(range) && range != excludableRange)
						ranges.Add(range);
				}
			}
		}
		Size CalcStickyRange(Rectangle rect) {
			if(IsHorizontal)
				return new Size(rect.Right - DefaultStickyZoneTickness / 2, rect.Right + DefaultStickyZoneTickness / 2);
			else
				return new Size(rect.Bottom - DefaultStickyZoneTickness / 2, rect.Bottom + DefaultStickyZoneTickness / 2);
		}
		public override void UpdateSplit(Adorner adorner, int change) {
			useSticky = CheckSticky(change, Node1.Parent);
			change = useSticky ? stickyDelta : change;
			base.UpdateSplit(adorner, change);
		}
		protected override void MoveSplitterCore(int change) {
			change = useSticky ? stickyDelta : change;
			base.MoveSplitterCore(change);
		}
	}
#endif
	class SplitterInfo : BaseElementInfo, ISplitterInfo {
		ObjectPainter Painter;
		SplitterInfoArgs Info;
		int splitIndexCore = -1;
		public DockingContainer Node1 { get; set; }
		public DockingContainer Node2 { get; set; }
		public SplitterInfo(TabbedView view)
			: base(view) {
			Init(view);
		}
		public override System.Type GetUIElementKey() {
			return typeof(ISplitterInfo);
		}
		protected override void UpdateStyleCore() {
			Init(Owner as TabbedView);
		}
		protected override void ResetStyleCore() {
			Reset();
		}
		void Init(TabbedView view) {
			Painter = SplitterHelper.GetPainter(view.ElementsLookAndFeel);
			Info = new SplitterInfoArgs(!view.IsHorizontal);
		}
		void Reset() {
			Painter = null;
			Info = null;
		}
		ObjectState stateCore;
		public ObjectState State {
			get { return stateCore; }
			set {
				if(stateCore == value) return;
				stateCore = value;
				OnStateChanged(value);
			}
		}
		protected virtual void OnStateChanged(ObjectState value) {
			CheckCursor(value);
			if(Owner != null)
				Owner.Invalidate();
		}
		protected void CheckCursor(ObjectState value) {
			Cursor cursor = null;
			if((value & (ObjectState.Hot | ObjectState.Pressed)) != 0)
				cursor = IsHorizontal ? Cursors.SizeWE : Cursors.SizeNS;
			if(Owner != null)
				Owner.SetCursor(cursor);
		}
		public bool IsHorizontal { get; set; }
		public int SplitIndex {
			get { return splitIndexCore; }
			set { splitIndexCore = value; }
		}
		int splitLength1, splitLength2;
		public int SplitLength1 {
			get { return Node1.ActualLength; }
			set { splitLength1 = value; }
		}
		public int SplitLength2 {
			get { return Node2.ActualLength; }
			set { splitLength2 = value; }
		}
		public int SplitConstraint1 {
			get { return IsHorizontal ? Node1.GetMinSize().Width : Node1.GetMinSize().Height; }
		}
		public int SplitConstraint2 {
			get { return IsHorizontal ? Node2.GetMinSize().Width : Node2.GetMinSize().Height; }
		}
		public BaseDocument[] GetDocuments() {
			return null;
		}
		public void MoveSplitter(int change) {
			if(change == 0 || SplitIndex == -1) return;
			MoveSplitterCore(change);
		}
		protected virtual void MoveSplitterCore(int change) {
			using(BatchUpdate.Enter(Owner.Manager)) {
				int allWidth = Node1.ActualLength + Node2.ActualLength;
				double allStar = Node1.Length.UnitValue + Node2.Length.UnitValue;
				double delta = (change * allStar) / allWidth;
				Node1.Length.UnitValue += delta;
				Node2.Length.UnitValue -= delta;
				Owner.SetLayoutModified();
				Owner.Manager.InvokePatchActiveChildren();
			}
		}
		public int CalcMinSize(Graphics g) {
			Info.IsVertical = IsHorizontal;
			Size minSize = ObjectPainter.CalcObjectMinBounds(g, Painter, Info).Size;
			return Info.IsVertical ? minSize.Width : minSize.Height;
		}
		protected void CalcObjectMinBounds(Graphics g) {
			Info.Graphics = g;
			Rectangle r = Painter.CalcObjectMinBounds(Info);
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			Info.IsVertical = IsHorizontal;
			Info.Bounds = bounds;
		}
		protected override void DrawCore(GraphicsCache cache) {
			Info.Cache = cache;
			Info.State = State;
			Info.Cache = null;
		}
		protected AdornerElementInfo AdornerInfo { get; set; }
		public virtual void BeginSplit(Adorner adorner) {
			SplitAdornerInfoArgs splitArgs = new SplitAdornerInfoArgs();
			splitArgs.Bounds = Bounds;
			AdornerInfo = new AdornerElementInfo(new SplitAdornerPainter(), splitArgs);
			adorner.Invalidate();
			adorner.Show(AdornerInfo);
		}
		public virtual void UpdateSplit(Adorner adorner, int change) {
			Point origin = Bounds.Location;
			origin.Offset(IsHorizontal ? new Point(change, 0) : new Point(0, change));
			SplitAdornerInfoArgs splitArgs = AdornerInfo.InfoArgs as SplitAdornerInfoArgs;
			splitArgs.Bounds = new Rectangle(origin, Bounds.Size);
			adorner.Invalidate();
		}
		public virtual void ResetSplit(Adorner adorner) {
			State = ObjectState.Normal;
			adorner.Reset(AdornerInfo);
			AdornerInfo = null;
		}
		#region IDockingAdornerInfo
		AdornerElementInfo DockingAdornerInfo;
		public void UpdateDocking(Adorner adorner, Point point, BaseDocument dragItem) {
			var bounds = Owner.Manager.Bounds;
			DockingAdornerInfoArgs args = DockingAdornerInfoArgs.EnsureInfoArgs(ref DockingAdornerInfo, adorner, Owner, dragItem, bounds);
			args.Adorner = Owner.Manager.GetDockingRect();
			args.Bounds = args.Content = args.Container = bounds;
			args.Header = Rectangle.Empty;
			args.MousePosition = point;
			args.DragItem = dragItem;
			if(args.Calc())
				adorner.Invalidate();
		}
		public void ResetDocking(Adorner adorner) {
			adorner.Reset(DockingAdornerInfo);
			DockingAdornerInfo = null;
		}
		public bool CanDock(Point point) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				return args.IsOverDockHint(point, out hint);
			}
			return false;
		}
		public void Dock(Point point, BaseDocument document) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				if(args.IsOverDockHint(point, out hint)) {
					Docking.FloatForm fForm = document.Form as Docking.FloatForm;
					switch(hint) {
						case DockHint.SideLeft:
						case DockHint.SideTop:
						case DockHint.SideRight:
						case DockHint.SideBottom:
							new DockHelper(Owner).DockSide(document, fForm, hint);
							break;
					}
				}
			}
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
	}
}
