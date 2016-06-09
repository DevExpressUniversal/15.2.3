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
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DevExpress.Diagram.Core.Serialization;
using DevExpress.Internal;
using System.Windows.Controls;
namespace DevExpress.Diagram.Core {
	struct AdjustmentInfo {
		public readonly AdjustmentBase[] Adjustments;
		public readonly Item_Owner_Bounds Parent;
		public readonly Item_Owner_Bounds[] Children;
		public AdjustmentInfo(AdjustmentBase[] adjustnents, Item_Owner_Bounds parent, Item_Owner_Bounds[] children) {
			Adjustments = adjustnents;
			Parent = parent;
			Children = children;
		}
	}
	abstract class AdjustmentBase {
		protected readonly IDiagramItem item;
		public AdjustmentBase(IDiagramItem item) {
			this.item = item;
		}
		public abstract void Apply(Transaction transaction);
	}
	class ConnectorAdjustment : AdjustmentBase {
		readonly Point? beginPoint, endPoint;
		readonly ConnectorPointsCollection points;
		readonly IDiagramItem owner;
		readonly int? index;
		public ConnectorAdjustment(IDiagramConnector connector, Point? beginPoint, Point? endPoint, ConnectorPointsCollection points, IDiagramItem owner, int? index) : base(connector) {
			this.beginPoint = beginPoint;
			this.endPoint = endPoint;
			this.owner = owner;
			this.index = index;
			this.points = points;
		}
		public override void Apply(Transaction transaction) {
			var connector = (IDiagramConnector)item;
			transaction.SetItemProperty(connector, points, DiagramConnectorActions.PointsProperty);
			Action<ConnectorPointType, Point?> setPoint = (pointType, point) => {
				if(point != null) {
					transaction.SetItemProperty(connector, point.Value, DiagramConnectorItemExtensions.GetPointProperty(pointType));
					transaction.SetItemProperty(connector, null, DiagramConnectorItemExtensions.GetItemProperty(pointType));
				}
			};
			setPoint(ConnectorPointType.Begin, beginPoint);
			setPoint(ConnectorPointType.End, endPoint);
		}
	}
	class BoundsAjustment : AdjustmentBase {
		public IDiagramItem Item { get { return item; } }
		public readonly Rect DiagramBounds;
		public BoundsAjustment(IDiagramItem item, Rect diagramBounds)
			: base(item) {
			this.DiagramBounds = diagramBounds;
		}
		public override void Apply(Transaction transaction) {
			transaction.SetItemBounds(item, item.Owner().GetItemRelativeRect(DiagramBounds).RemoveBias(item.ActualBounds()));
		}
		public override string ToString() {
			return string.Format("{0}, {1}, DiagramBounds: {2}", GetType().Name, item, DiagramBounds);
		}
	}
	class BoundsOwnerAngleAdjustment : BoundsAjustment {
		static readonly PropertyDescriptor AngleProperty = ExpressionHelper.GetProperty((IDiagramItem x) => x.Angle);
		readonly IDiagramItem owner;
		readonly int? index;
		readonly double angle;
		public BoundsOwnerAngleAdjustment(IDiagramItem item, Rect diagramBounds, IDiagramItem owner, int? index, double angle) 
			: base(item, diagramBounds) {
			this.owner = owner;
			this.index = index;
			this.angle = angle;
		}
		public override void Apply(Transaction transaction) {
			base.Apply(transaction);
			if(item.Angle != angle)
				transaction.SetItemProperty(item, angle, AngleProperty);
		}
		public override string ToString() {
			var result = base.ToString();
			if(owner != item.Owner())
				result += ", Owner: " + owner;
			if(item.Angle != angle)
				result += ", Angle: " + angle;
			return result;
		}
	}
	static class MoveHelper {
		internal static bool DoMoveItems(this IDiagramControl diagram, Transaction transaction, Item_Owner_Bounds[] moveInfos, bool useAnchors, IEnumerable<Direction> directions = null, bool cutOutOfBounds = false) {
			directions = directions ?? new[] { Direction.Right, Direction.Down, Direction.Left, Direction.Up };
			moveInfos = directions.Aggregate(moveInfos, (result, direction) => result = CoerceMoveInfo(result, direction, cutOutOfBounds));
			if(!moveInfos.Any(x => x.DiagramBounds != x.Item.ActualDiagramBounds() || x.Angle != x.Item.Angle || x.Owner != x.Item.Owner()))
				return false;
			var movingItems = new HashSet<IDiagramItem>(moveInfos.Select(x => x.Item));
			var itemsAdjustments = (moveInfos.Any() && moveInfos.First().Item.Owner() is IDiagramList)
				? ListMoveHelper.GetListAdjustments(moveInfos.Single(), directions, cutOutOfBounds).ToArray()
				: moveInfos.Select(x => {
					var adjustment = x.Item.Controller.CreateLayoutAdjustment(x, movingItems.Contains);
					return new AdjustmentInfo(new[] { adjustment }, x, new[] { x });
				}).ToArray();
			var parentAdjustments = GetParentAdjustments(itemsAdjustments.Select(x => x.Parent).ToArray(), directions, cutOutOfBounds);
			var unmovableItems = itemsAdjustments.SelectMany(x => x.Children).Select(x => x.Item).ToArray();
			var childrenAdjustments = new Dictionary<IDiagramItem, AdjustmentBase>();
			foreach(var info in itemsAdjustments.SelectMany(x => x.Children)) {
				CollectChildrenAdjustmentsCore(unmovableItems, useAnchors, childrenAdjustments, info.Item, info.DiagramBounds);
			}
			var allAdjustments = parentAdjustments
				.Concat(itemsAdjustments.SelectMany(x => x.Adjustments))
				.Concat(childrenAdjustments.Values);
			foreach(var item in moveInfos.Where(x => x.Item.Owner() != null)) {
				if(item.Owner != item.Item.Owner())
					transaction.ChangeItemOwner(item.Owner, item.Item, item.Index);
			}
			transaction.AddItems(diagram, moveInfos.Where(x => x.Item.Owner() == null).Select(x => x.ToAddInfo()));
			foreach(var adjustment in allAdjustments) {
				adjustment.Apply(transaction);
			}
			diagram.Controller.Connectors.UpdateRoute(moveInfos, transaction.SetItemProperty);
			return true;
		}
		static AdjustmentBase[] GetParentAdjustments(Item_Owner_Bounds[] moveInfos, IEnumerable<Direction> directions = null, bool cutOutOfBounds = false) {
			var parentAdjustments = new Dictionary<IDiagramItem, Rect>();
			foreach(var moveInfo in moveInfos) {
				foreach(var parent in moveInfo.Owner.GetParentsIncludingSelf()) {
					Rect adjustedBounds = GetAdjustedParentBounds(directions, moveInfo, parent);
					Rect previouslyAdjustedBounds;
					if(parentAdjustments.TryGetValue(parent, out previouslyAdjustedBounds)) {
						parentAdjustments[parent] = MathHelper.GetContainingRect(new[] { adjustedBounds, previouslyAdjustedBounds });
					} else {
						parentAdjustments[parent] = adjustedBounds;
					}
				}
			}
			var unmovableItems = moveInfos.Select(x => x.Item)
				.Concat(parentAdjustments.Keys)
				.Distinct()
				.ToArray();
			var childrenAdjustments = new Dictionary<IDiagramItem, AdjustmentBase>();
			foreach(var info in parentAdjustments.OrderBy(x => x.Key.GetLevel())) {
				CollectChildrenAdjustmentsCore(unmovableItems, true, childrenAdjustments, info.Key, info.Value);
			}
			return parentAdjustments.OrderBy(x => x.Key.GetLevel()).Select(x => new BoundsAjustment(x.Key, x.Value))
				.Concat(childrenAdjustments.Values)
				.ToArray();
		}
		static void CollectChildrenAdjustmentsCore(IDiagramItem[] unmovableItems, bool useAnchors, Dictionary<IDiagramItem, AdjustmentBase> childrenAdjustments, IDiagramItem item, Rect newDiagramBounds) {
			var currentActualDiagramBounds = item.ActualDiagramBounds();
			var topLeftOffset = MathHelper.GetOffset(currentActualDiagramBounds.TopLeft, newDiagramBounds.TopLeft);
			var bottomRightOffset = MathHelper.GetOffset(currentActualDiagramBounds.BottomRight, newDiagramBounds.BottomRight);
			var newActualDiagramBounds = currentActualDiagramBounds.InflateRect(topLeftOffset, bottomRightOffset);
			if(newActualDiagramBounds == currentActualDiagramBounds)
				return;
			foreach(var child in item.NestedItems.Where(x => !unmovableItems.Contains(x))) {
				var currentChildBounds = child.ActualBounds();
				var newChildBounds = (useAnchors ? child.Anchors : Sides.All).AnchorBounds(currentChildBounds, topLeftOffset, bottomRightOffset);
				var newChildDiagramBounds = newChildBounds.OffsetRect(newActualDiagramBounds.Location);
				childrenAdjustments.Add(child, new BoundsAjustment(child, newChildDiagramBounds));
				CollectChildrenAdjustmentsCore(unmovableItems, useAnchors, childrenAdjustments, child, newChildDiagramBounds);
			}
		}
		static readonly PropertyDescriptor AngleProperty = ExpressionHelper.GetProperty((IDiagramItem x) => x.Angle);
		static Rect GetAdjustedParentBounds(IEnumerable<Direction> directions, Item_Owner_Bounds item, IDiagramItem parent) {
			var parentClientDiagramBounds = parent.RotatedClientDiagramBounds();
			return directions.Aggregate(parentClientDiagramBounds.Rect, (result, direction) => {
				var adjustBehavior = parent.Controller.GetAdjustBoundaryBehavior(direction);
				if(adjustBehavior == AdjustBoundaryBehavior.AutoAdjust)
					return AdjustParentBounds(result, new Rect_Angle(item.DiagramBounds, item.Angle - parentClientDiagramBounds.Angle), direction);
				return result;
			}, clientBounds => clientBounds.InflateRect(parent.Padding));
		}
		static Rect AdjustParentBounds(Rect parentBounds, Rect_Angle childBounds, Direction direction) {
			if(MathHelper.IsNotaRect(childBounds.Rect))
				return parentBounds;
			var containingRect = MathHelper.GetContainingRect(new[] { childBounds.RotatedRect, parentBounds });
			return direction.SetSide(parentBounds, direction.GetSide(containingRect));
		}
		static Item_Owner_Bounds[] CoerceMoveInfo(Item_Owner_Bounds[] moveInfo, Direction direction, bool cutOutOfBounds) {
			return moveInfo.Select(item => {
				var adjustingRect = item.DiagramBoundsInfo;
				if(MathHelper.IsNotaRect(adjustingRect.Rect))
					return item;
				var child = item.Item;
				foreach(IDiagramItem parent in item.Owner.GetParentsIncludingSelf()) {
					var maxChildBounds = parent.Controller.GetMaxChildBounds(child, direction);
					var intersection = MathHelper.Intersection(maxChildBounds, adjustingRect.RotatedRect);
					var delta = direction.GetSide(intersection) - direction.GetSide(adjustingRect.RotatedRect);
					var newRect = cutOutOfBounds
							? direction.SetSide(adjustingRect.Rect, direction.GetSide(adjustingRect.Rect) + delta)
							: direction.GetOrientation().OffsetRect(adjustingRect.Rect, delta);
					adjustingRect = new Rect_Angle(newRect, adjustingRect.Angle);
					child = parent;
				}
				return Item_Owner_Bounds.FromDiagramBounds(item.Item, item.Owner, adjustingRect, item.Index);
			}).ToArray();
		}
	}
	static class ListMoveHelper {
		class ListAdjustment : AdjustmentBase {
			public IDiagramItem Item { get { return item; } }
			public readonly double[] Weights;
			public ListAdjustment(IDiagramList item, double[] weights) : base(item) {
				this.Weights = weights;
			}
			public override void Apply(Transaction transaction) {
				var property = ExpressionHelper.GetProperty((IDiagramItem x) => x.Weight);
				transaction.SetMultipleItemsPropertyValues(item.NestedItems, property, Weights.Cast<object>(), x => x.GetFinder(), x => x);
			}
			public override string ToString() {
				return base.ToString() + string.Concat(Weights.Select(x => " " + x));
			}
		}
		class ItemProxy {
			public static ItemProxy[] CreateProxies(IDiagramItem item) {
				return item.NestedItems
					.Select(x =>
						new ItemProxy(x) {
							DiagramBounds = x.ActualDiagramBounds(),
							Weight = x.Weight
						}
					)
					.ToArray();
			}
			public readonly IDiagramItem Item;
			public ItemProxy(IDiagramItem item) {
				this.Item = item;
				NestedProxies = EmptyArray<ItemProxy>.Instance;
			}
			public Rect DiagramBounds { get; set; }
			public double Weight { get; set; }
			public ItemProxy[] NestedProxies { get; set; }
			public void Apply(Rect diagramBounds, double[] weights) {
				DiagramBounds = diagramBounds;
				var list = Item as IDiagramList;
				if(list == null) {
					return;
				}
				var orientation = list.Orientation;
				var unitSize = orientation.GetSize(diagramBounds.Size) / weights.Sum();
				var breadth = orientation.Rotate().GetSize(diagramBounds.Size);
				var offset = 0d;
				NestedProxies.ForEach(weights, (x, weight) => {
					var itemSize = unitSize * weight;
					x.Apply(
						new Rect(diagramBounds.Location.OffsetPoint(orientation.MakePoint(offset, 0)), orientation.MakeSize(itemSize, breadth)),
						x.NestedProxies.Select(nested => nested.Weight).ToArray()
						);
					offset += itemSize;
				});
			}
		}
		public static AdjustmentInfo[] GetListAdjustments(Item_Owner_Bounds moveInfo, IEnumerable<Direction> directions, bool cutOutOfBounds) {
			if(directions == null)
				throw new NotImplementedException();
			var parentAdjustments = GetListParentsAdjustments(moveInfo, directions, cutOutOfBounds).ToArray();
			var adjustments = GetListAdjustmentsCore(moveInfo, directions, cutOutOfBounds, parentAdjustments.First().DiagramBounds.Size)
				.ToArray();
			var rootParentAdjustment = parentAdjustments.Last();
			var childrenMoveInfo = GetChildrenWeightEquvalentMoveInfos(parentAdjustments, adjustments);
			return new[] { new AdjustmentInfo(
				rootParentAdjustment.Yield().Concat<AdjustmentBase>(adjustments).ToArray(),
				Item_Owner_Bounds.FromDiagramBounds(rootParentAdjustment.Item, rootParentAdjustment.Item.Owner(), rootParentAdjustment.DiagramBounds),
				childrenMoveInfo
			)};
		}
		static Item_Owner_Bounds[] GetChildrenWeightEquvalentMoveInfos(BoundsAjustment[] parentAdjustments, ListAdjustment[] adjustments) {
			var rootParentAdjustment = parentAdjustments.Last();
			var proxy = new ItemProxy(rootParentAdjustment.Item) {
				NestedProxies = ItemProxy.CreateProxies(rootParentAdjustment.Item),
				DiagramBounds = rootParentAdjustment.DiagramBounds,
			};
			proxy.NestedProxies.ForEach((x, i) => {
				if(rootParentAdjustment.Item.NestedItems[i] is IDiagramList) {
					x.NestedProxies = ItemProxy.CreateProxies(rootParentAdjustment.Item.NestedItems[i]);
				}
			});
			var itemToProxyMap = proxy.Yield()
				.Flatten(x => x.NestedProxies)
				.ToDictionary(x => x.Item, x => x);
			foreach(var adjustment in parentAdjustments) {
				var itemProxy = itemToProxyMap[adjustment.Item];
				itemProxy.Apply(adjustment.DiagramBounds, itemProxy.NestedProxies.Select(x => x.Weight).ToArray());
			}
			foreach(var adjustment in adjustments) {
				var itemProxy = itemToProxyMap[adjustment.Item];
				itemProxy.Apply(itemProxy.DiagramBounds, adjustment.Weights);
			}
			var childrenMoveInfo = proxy.Yield()
				.Flatten(x => x.NestedProxies)
				.Where(x => !(x.Item is IDiagramList))
				.Select(x => Item_Owner_Bounds.FromDiagramBounds(x.Item, x.Item.Owner(), x.DiagramBounds))
				.ToArray();
			return childrenMoveInfo;
		}
		static IEnumerable<BoundsAjustment> GetListParentsAdjustments(Item_Owner_Bounds moveInfo, IEnumerable<Direction> directions, bool cutOutOfBounds) {
			var firstParentAdjustedBounds = GetListAdjustedParentBounds(directions, moveInfo.Item, moveInfo.DiagramBounds, moveInfo.Owner);
			yield return new BoundsAjustment(moveInfo.Owner, firstParentAdjustedBounds);
			if(moveInfo.Owner.Owner() is IDiagramList) {
				var secondParentAdjustedBounds = GetListAdjustedParentBounds(directions, moveInfo.Item.Owner(), firstParentAdjustedBounds, moveInfo.Owner.Owner());
				yield return new BoundsAjustment(moveInfo.Owner.Owner(), secondParentAdjustedBounds);
			}
		}
		static IEnumerable<ListAdjustment> GetListAdjustmentsCore(Item_Owner_Bounds moveInfo, IEnumerable<Direction> directions, bool cutOutOfBounds, Size newListSize) {
			var child = moveInfo.Item;
			var newChildBounds = moveInfo.Bounds;
			yield return GetListAdjustmentCore(child, newListSize, newChildBounds);
			var list = (IDiagramList)child.Owner();
			var ownerList = list.Owner() as IDiagramList;
			if(ownerList == null)
				yield break;
			var index = child.GetIndexInOwnerCollection();
			foreach(var direction in directions) {
				Func<IDiagramItem, Rect, ListAdjustment> getListAdjustment = (listChild, newListChildBounds) => {
					var newBounds = listChild.ActualBounds();
					newBounds = direction.SetSide(newBounds, direction.GetSide(newListChildBounds));
					return GetListAdjustmentCore(listChild, newListSize, newBounds);
				};
				if(direction.GetOrientation() == list.Orientation) {
					if((index == 0 && direction.IsNear()) || (index == list.NestedItems.Count - 1 && direction.IsFar())) {
						var neighboursAdjustments = ownerList.NestedItems
							.Where(neighbourList => neighbourList != list)
							.Select(neighbourList => {
								return getListAdjustment(neighbourList.NestedItems[index], newChildBounds);
							});
						foreach(var neighbourAdjustment in neighboursAdjustments) {
							yield return neighbourAdjustment;
						}
					}
				} else {
					yield return getListAdjustment(list, newChildBounds.OffsetRect(list.Position));
				}
			}
		}
		static Rect GetListAdjustedParentBounds(IEnumerable<Direction> directions, IDiagramItem item, Rect diagramBounds, IDiagramItem parent) {
			var parentClientDiagramBounds = parent.RotatedClientDiagramBounds();
			return directions.Aggregate(parentClientDiagramBounds.Rect, (result, direction) => {
				var list = (IDiagramList)parent;
				var adjustBounds = direction.GetOrientation() != list.Orientation ||
				   (item == list.NestedItems.First() && direction.IsNear()) ||
				   (item == list.NestedItems.Last() && direction.IsFar());
				if(adjustBounds) {
					var originalChildBounds = item.RotatedDiagramBounds();
					var diff = direction.GetSide(diagramBounds) - direction.GetSide(originalChildBounds.RotatedRect);
					return direction.SetSide(result, direction.GetSide(result) + diff);
				}
				return result;
			}, clientBounds => clientBounds.InflateRect(parent.Padding));
		}
		static ListAdjustment GetListAdjustmentCore(IDiagramItem child, Size newListSize, Rect newChildBounds) {
			var list = (IDiagramList)child.Owner();
			var topLeftOffset = MathHelper.GetOffset(child.ActualBounds().TopLeft, newChildBounds.TopLeft);
			var bottomRightOffset = MathHelper.GetOffset(child.ActualBounds().BottomRight, newChildBounds.BottomRight);
			var nearOffset = list.Orientation.GetPoint(topLeftOffset);
			var farOffset = list.Orientation.GetPoint(bottomRightOffset);
			var currentSizes = list.NestedItems.Select(x => list.Orientation.GetSize(x.ActualSize)).ToArray();
			var index = child.GetIndexInOwnerCollection();
			if(index == 0) {
				currentSizes[index] -= nearOffset;
			}
			if(index > 0) {
				currentSizes[index] -= nearOffset;
				currentSizes[index - 1] += nearOffset;
			}
			if(index < list.NestedItems.Count - 1) {
				currentSizes[index] += farOffset;
				currentSizes[index + 1] -= farOffset;
			}
			if(index == list.NestedItems.Count - 1) {
				currentSizes[index] += farOffset;
			}
			var weights = currentSizes.Select(x => x / list.Orientation.GetSize(newListSize)).ToArray();
			return new ListAdjustment(list, weights);
		}
	}
}
