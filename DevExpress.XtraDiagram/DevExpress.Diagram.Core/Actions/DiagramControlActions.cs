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
namespace DevExpress.Diagram.Core {
	public class Item_Owner_Bounds {
		public static Item_Owner_Bounds FromLocalBounds(IDiagramItem item, IDiagramItem owner, Rect bounds, double? angle = null, int? index = null) {
			return new Item_Owner_Bounds(item, owner, new Rect_Angle(owner.GetDiagramRect(bounds), angle ?? item.Angle), index);
		}
		public static Item_Owner_Bounds FromDiagramBounds(IDiagramItem item, IDiagramItem owner, Rect diagramBounds, int? index = null) {
			return new Item_Owner_Bounds(item, owner, new Rect_Angle(diagramBounds, item.Angle), index);
		}
		public static Item_Owner_Bounds FromDiagramBounds(IDiagramItem item, IDiagramItem owner, Rect_Angle diagramBounds, int? index = null) {
			return new Item_Owner_Bounds(item, owner, diagramBounds, index);
		}
		public readonly IDiagramItem Item;
		public readonly IDiagramItem Owner;
		public double Angle { get { return DiagramBoundsInfo.Angle; } }
		public Rect Bounds { get { return Owner.GetItemRelativeRect(DiagramBounds); } }
		public Rect DiagramBounds { get { return DiagramBoundsInfo.Rect; } }
		public readonly Rect_Angle DiagramBoundsInfo;
		public readonly int? Index;
		Item_Owner_Bounds(IDiagramItem item, IDiagramItem owner, Rect_Angle diagramBounds, int? index) {
			this.Item = item;
			this.Owner = owner;
			this.DiagramBoundsInfo = diagramBounds;
			this.Index = index;
		}
		public AddItemInfo ToAddInfo() {
			return new AddItemInfo(Item, Owner, Index);
		}
	}
	public class Rect_Angle {
		public readonly Rect Rect;
		public readonly double Angle;
		public Rect RotatedRect { get { return Rect.Rotate(Angle); } }
		public Rect_Angle(Rect rect, double angle) {
			this.Rect = rect;
			this.Angle = angle;
		}
	}
	class Point_Angle {
		public readonly Point Point;
		public readonly double Angle;
		public Point_Angle(Point point, double angle) {
			this.Point = point;
			this.Angle = angle;
		}
	}
	public enum AdjustBoundaryBehavior {
		None,
		AutoAdjust,
		DisableOutOfBounds,
	}
	public static class DiagramControlActions {
		#region Public
		public static void DeleteSelectedItems(this IDiagramControl diagram) {
			if(diagram.CanDeleteSelectedItems()) {
				diagram.ExecuteWithSelectionRestore(
					transaction => {
						var itemsToDelete = diagram.SelectedItems().GetParentsOnly().ToArray();
						foreach(var item in itemsToDelete) {
							if(item.IsInDiagram())
								item.GetSelfAndChildren().ForEach(x => x.Controller.BeforeDelete(transaction));
							if(item.IsInDiagram())
								transaction.RemoveItem(item);
						}
					},
					restoreSelectionItems: new IDiagramItem[0]
				);
			}
		}
		public static bool CanDeleteSelectedItems(this IDiagramControl diagram) {
			return diagram.TrueForAllSelectedItemParents(x => x.CanDeleteCore());
		}
		public static bool CanCopySelectedItems(this IDiagramControl diagram) {
			return diagram.TrueForAllSelectedItemParents(x => x.CanCopyCore());
		}
		public static void MoveSelection(this IDiagramControl diagram, Point from, Point to) {
			var moveInfo = diagram.GetMoveItemsInfo(diagram.GetSelectedMovableItems(), from, to);
			if(!moveInfo.Any())
				return;
			diagram.MoveItemsCore(moveInfo);
		}
		internal static void MoveItemsCore(this IDiagramControl diagram, Item_Owner_Bounds[] moveInfo) {
			diagram.ExecuteWithSelectionRestore(transaction => {
				diagram.DoMoveItems(transaction, moveInfo, useAnchors: false);
			}, allowMerge: false);
		}
		public static void CopySelection(this IDiagramControl diagram, Point from, Point to) {
			var items = diagram.GetSelectedCopyableItems();
			var cloneInfo = diagram.SerializeItems(items.ToList(), StoreRelationsMode.RelativeToSerializingItems);
			var clonedItems = diagram.DeserializeItems(cloneInfo, StoreRelationsMode.RelativeToSerializingItems);
			var map = items
				.Zip(clonedItems, (item, cloned) => new { item, cloned })
				.ToDictionary(x => x.item, x => x.cloned);
			var moveInfo = diagram.GetMoveItemsInfo(items, from, to, item => map[item]);
			if(!moveInfo.Any())
				return;
			diagram.ExecuteAddItemsAction(moveInfo);
		}
		public static void DrawItem(this IDiagramControl diagram, Point from, Point to, IDiagramItem item) {
			var rectFromPoints = MathHelper.RectFromPoints(from, to);
			var resultRect = diagram.GetSnapInfo(Enumerable.Empty<IDiagramItem>(), from).SnapRect(rectFromPoints);
			DrawItemAtPoint(diagram, item, from, resultRect);
		}
#if DEBUGTEST
		public static void DrawItemForTests(this IDiagramControl diagram, Point from, Point to, ItemTool tool) {
			diagram.DrawItem(from, to, tool.CreateItem(diagram));
		}
#endif
		public static void DrawItemAtPoint(this IDiagramControl diagram, IDiagramItem item, Point hitPoint, Rect rect) {
			diagram.Controller.DoCoercedInsertTargetAction(diagram.RootItem().FindContainerItemAtPoint(hitPoint), item.Yield(), owner => {
				diagram.DrawItem(item, owner, rect);
			});
		}
		public static void DrawItem(this IDiagramControl diagram, ItemTool tool) {
			var item = tool.CreateItem(diagram);
			diagram.Controller.DoDefaultInsertTargetAction(item.Yield(), owner => {
				var rect = owner.ActualDiagramBounds().GetCenter().GetCenteredRect(tool.DefaultItemSize);
				var snapInfo = diagram.GetSnapInfo(snapScopeItem: owner, allowSnapToItems: false);
				var snapResult = snapInfo.SnapRectLocation(rect);
				diagram.DrawItem(item, owner, snapResult.Result);
			});
		}
		public static void DrawItem(this IDiagramControl diagram, IDiagramItem item, IDiagramItem owner, Rect rect, int? index = null) {
			item.Angle = -owner.RotatedDiagramBounds().Angle;
			diagram.ExecuteAddItemsAction(new[] { Item_Owner_Bounds.FromDiagramBounds(item, owner, rect, index) }, selectChildren: false);
		}
		public static void CopySelectedItems(this IDiagramControl diagram) {
			if(diagram.CanCopySelectedItems()) {
				var items = GetMoveableItems(diagram, diagram.SelectedItems());
				if(items.Any())
					DiagramClipboard.Save(diagram, items.ToList());
				else
					DiagramClipboard.Clear();
			}
		}
		public static void Paste(this IDiagramControl diagram) {
			IList<IDiagramItem> items = DiagramClipboard.GetObjects(diagram);
			if(items != null) {
				diagram.Controller.DoDefaultInsertTargetAction(items, container => {
					var offset = items.Select(x => x.ActualBounds()).GetContainingRect().GetCenterRectOffset(container.Size);
					var moveInfo = items.Select(x => Item_Owner_Bounds.FromLocalBounds(x, container, x.ActualBounds().OffsetRect(offset))).ToArray();
					diagram.ExecuteAddItemsAction(moveInfo);
				});
			}
		}
		public static void CutSelectedItems(this IDiagramControl diagram) {
			if(diagram.CanCopySelectedItems() && diagram.CanDeleteSelectedItems()) {
				diagram.CopySelectedItems();
				diagram.DeleteSelectedItems();
			}
		}
		public static void SelectAll(this IDiagramControl diagram) {
			diagram.Selection().SelectItems(diagram.RootItem().GetChildren(), true);
		}
		#endregion
		#region Internal
		public static IEnumerable<IDiagramItem> GetSelectedMovableItems(this IDiagramControl diagram) {
			return diagram.SelectedItems().Where(x => x.CanMove);
		}
		public static IEnumerable<IDiagramItem> GetSelectedCopyableItems(this IDiagramControl diagram) {
			return diagram.SelectedItems().Where(x => x.Controller.CanCopyCore());
		}
		public static bool CanMoveItemsTo(this IDiagramControl diagram, IEnumerable<IDiagramItem> items, Point to) {
			return diagram.GetMoveItemsInfo(items, default(Point), to).Any();
		}
		static bool TrueForAllSelectedItemParents(this IDiagramControl diagram, Predicate<DiagramItemController> predicate) {
			return diagram.SelectedItems().Any() && diagram.SelectedItems().GetParentsOnly().All(x => predicate(x.Controller));
		}
		static void ExecuteAddItemsAction(this IDiagramControl diagram, Item_Owner_Bounds[] moveInfo, bool selectChildren = true) {
			diagram.ExecuteWithSelectionRestore(
				transaction => {
					diagram.DoMoveItems(transaction, moveInfo, default(bool));
				},
				restoreSelectionItems: (selectChildren ?
					moveInfo.SelectMany(x => x.Item.GetSelfAndChildren()) :
					moveInfo.Select(x => x.Item)).ToArray()
			);
		}
		#endregion
		#region private
		static Item_Owner_Bounds[] GetMoveItemsInfo(this IDiagramControl diagram, IEnumerable<IDiagramItem> items, Point from, Point to, Func<IDiagramItem, IDiagramItem> getActualItem = null) {
			getActualItem = getActualItem ?? (item => item);
			var owner = diagram.RootItem().FindContainerItemAtPoint(to);
			var result = new List<Item_Owner_Bounds>();
			foreach(IDiagramItem item in GetMoveableItems(diagram, items)) {
				var actualItem = getActualItem(item);
				bool canAdd = diagram.Controller.DoCoercedInsertTargetAction(item == owner ? (IDiagramContainer)owner.Owner() : owner, actualItem.Yield(), actualOwner => {
					Point offset = to.OffsetPoint(from.InvertPoint());
					Rect oldBounds = item.ActualDiagramBounds();
					Point newCenter = oldBounds.GetCenter().OffsetPoint(offset);
					Point newDiagramPosition = newCenter.GetCenteredRect(oldBounds.Size).Location;
					result.Add(Item_Owner_Bounds.FromDiagramBounds(actualItem, actualOwner, new Rect(newDiagramPosition, actualItem.Size)));
				});
				if(!canAdd)
					return Enumerable.Empty<Item_Owner_Bounds>().ToArray();
			}
			return result.ToArray();
		}
		static IDiagramItem[] GetMoveableItems(IDiagramControl diagram, IEnumerable<IDiagramItem> items) {
			return items.GetParentsOnly().Where(x => x != diagram.RootItem()).ToArray();
		}
		#endregion
	}
	public static class LayoutActions {
		public static void MoveSelectionNoSnap(this IDiagramControl diagram, Direction direction) {
			diagram.MoveSelectionCore(direction, () => SnapInfo.Empty);
		}
		public static void MoveSelection(this IDiagramControl diagram, Direction direction) {
			diagram.MoveSelectionCore(direction, () => diagram.GetSnapInfo(snapScopeItem: diagram.PrimarySelection(), allowSnapToItems: false));
		}
		static void MoveSelectionCore(this IDiagramControl diagram, Direction direction, Func<SnapInfo> getSnapInfo) {
			var primarySelection = diagram.PrimarySelection();
			if(primarySelection == null)
				return;
			var snapInfo = getSnapInfo();
			var bounds = primarySelection.ActualDiagramBounds();
			var snapOffset = GetSnapOffset(snapInfo, default(Point), direction, bounds);
			if(!direction.IsRightDirection(snapOffset))
				snapOffset = GetSnapOffset(snapInfo, snapInfo.GetOffset(direction), direction, bounds);
			var items = diagram.GetSelectedMovableItems().GetParentsOnly().ToArray();
			var moveInfo = items.Select(x => Item_Owner_Bounds.FromDiagramBounds(x, x.Owner(), x.ActualDiagramBounds().OffsetRect(snapOffset))).ToArray();
			diagram.ExecuteWithSelectionRestore(transaction => {
				if(diagram.DoMoveItems(transaction, moveInfo, useAnchors: false, directions: direction.Yield()))
					transaction.AddMergeToken(direction);
			}, allowMerge: true);
			diagram.BringSelectionIntoView();
		}
		static Point GetSnapOffset(SnapInfo snapInfo, Point offset, Direction direction, Rect bounds) {
			var snapOffset = snapInfo.GetRectSnapOffset(snapInfo.GetOffset(direction), bounds).Offset;
			return direction.GetOrientation().OffsetPoint(offset, snapOffset);
		}
	}
	public static class SizeInfo {
		public static SizeInfo<IDiagramItem> Create(IDiagramItem item, ResizeMode mode) {
			return new SizeInfo<IDiagramItem>(item, item.RotatedDiagramBounds(), GetMinResizingSize(item, mode));
		}
		public static SizeInfo<IAdorner> Create(IAdorner adorner, IDiagramItem item, ResizeMode mode) {
			return new SizeInfo<IAdorner>(adorner, adorner.RotatedBounds(), item.Return(x => GetMinResizingSize(x, mode), () => default(Size)));
		}
		static Size GetMinResizingSize(IDiagramItem item, ResizeMode mode) {
			return item.Controller.GetMinResizingSize(directions: mode.GetDirections());
		}
		public static SizeInfo<IDiagramItem>[] GetSizeInfo(IEnumerable<IDiagramItem> items, ResizeMode mode) {
			return items.Select(x => SizeInfo.Create(x, mode)).ToArray();
		}
	}
	public static class DiagramControlResizeActions {
		public static IEnumerable<IDiagramItem> GetResizableSelectedItems(this IDiagramControl diagram) {
			return diagram.SelectedItems().Where(x => x.CanResize);
		}
		public static AxisLine[] ResizeSelection(this IDiagramControl diagram, Point startPosition, Point endPosition, ResizeMode mode, Transaction transaction, IEnumerable<SizeInfo<IDiagramItem>> items, SnapInfo snapInfo, double rotationAngle) {
			var resizedItemInfoSnapResult = Resize(items, mode, startPosition, endPosition, snapInfo, rotationAngle);
			var moveInfo = resizedItemInfoSnapResult.Result.Select(x => Item_Owner_Bounds.FromDiagramBounds(x.Item, x.Item.Owner(), x.Rect)).ToArray();
			transaction.ExecuteWithSelectionRestore(diagram, nestedTransaction => {
				var directions = MathHelper.AreEqual(rotationAngle, 0) ? mode.GetDirections() : null;
				diagram.DoMoveItems(nestedTransaction, moveInfo, useAnchors: true, directions: directions, cutOutOfBounds: true);
			});
			return resizedItemInfoSnapResult.SnapLines;
		}
		public static AxisLine[] ResizeAdorners(this IDiagramControl diagram, Point startPosition, Point endPosition, ResizeMode mode, IEnumerable<SizeInfo<IAdorner>> items, SnapInfo snapInfo, double rotationAngle) {
			var resizedItemInfoSnapResult = Resize(items, mode, startPosition, endPosition, snapInfo, rotationAngle);
			resizedItemInfoSnapResult.Result.ForEach(x => x.Item.Bounds = x.Rect.Rect);
			return resizedItemInfoSnapResult.SnapLines;
		}
		static SnapResult<IEnumerable<SizeInfo<T>>> Resize<T>(IEnumerable<SizeInfo<T>> items, ResizeMode mode, Point startPosition, Point endPosition, SnapInfo snapInfo, double rotationAngle) {
			var snapResult = snapInfo.SnapRectResizing(items, mode, startPosition, endPosition, rotationAngle);
			var newBounds = ResizeHelper.ResizeRects(items, mode, snapResult.Offset, rotationAngle);
			return snapResult.WithResult(items.Zip(newBounds, (x, rect) => new SizeInfo<T>(x.Item, new Rect_Angle(rect, x.Rect.Angle), x.MinSize)));
		}
	}
	public static class DiagramControlEditActions {
		public static void StartEditPrimarySelection(this IDiagramControl diagram) {
			if(!diagram.CanEditPrimarySelection())
				return;
			var item = diagram.PrimarySelection();
			IAdorner adorner = null;
			adorner = diagram.AdornerFactory().CreateInplaceEditor(item, () => {
				adorner.Destroy();
				diagram.Controller.FocusSurface();
			});
			adorner.Bounds = item.Controller.GetInplaceEditAdornerBounds();
		}
		public static bool CanEditPrimarySelection(this IDiagramControl diagram) {
			return diagram.PrimarySelection().Return(x => x.Controller.GetFontTraits().Return(y => y.AllowEdit, () => false), () => false);
		}
	}
	public static class DiagramControlChangeParameterActions {
		static readonly PropertyDescriptor ParametersProperty = ExpressionHelper.GetProperty((IDiagramShape x) => x.Parameters);
		internal static void ChangeParameter(this IDiagramControl diagram, IDiagramShape shape, ParameterDescription parameter, Point position, SnapInfo snapInfo, Transaction nestedTransaction) {
			Point snappedPosition = snapInfo.SnapPoint(position);
			Point rotatedPosition = shape.Position.Rotate(shape.Angle, shape.GetDiagramRotationCenter());
			Point localPoint = MathHelper.GetOffset(rotatedPosition, snappedPosition).Rotate(shape.Angle);
			double parameterValue = parameter.GetValue(shape.Size, shape.GetParameters().ToArray(), localPoint);
			if(parameterValue != shape.GetParameterValue(parameter))
				shape.SetParameterValue(nestedTransaction, parameter, parameterValue);
		}
		internal static void SetParameterValue(this IDiagramShape shape, Transaction transaction, ParameterDescription parameter, double value) {
			DoubleCollection newParams = shape.GetParameters();
			newParams[shape.GetParameterIndex(parameter)] = value;
			transaction.SetItemProperty(shape, newParams, ParametersProperty);
			shape.UpdateRouteForAttachedConnectors(transaction);
		}
	}
	class Item_Angle {
		public readonly IDiagramItem Item;
		public readonly double Angle;
		public Item_Angle(IDiagramItem item, double angle) {
			this.Item = item;
			this.Angle = angle;
		}
	}
	public static class DiagramControlRotationActions {
		const double SnapAngle = 15;
		internal static void RotateItems(this IDiagramControl diagram, Transaction transaction, double angleDelta, Item_Angle[] rotateableItems, Point rotationCenter) {
			var moveInfoList = new List<Item_Owner_Bounds>();
			foreach(var item in rotateableItems) {
				double newAngle = item.Angle + angleDelta;
				moveInfoList.Add(GetItemInfo(rotationCenter, item.Item, item.Item.Angle, newAngle));
			}
			var moveInfo = moveInfoList.ToArray();
			diagram.DoMoveItems(transaction, moveInfo, true);
		}
		static Item_Owner_Bounds GetItemInfo(Point rotationCenter, IDiagramItem item, double oldAngle, double newAngle) {
			Rect_Angle oldRect = new Rect_Angle(item.ActualDiagramBounds(), oldAngle);
			Rect_Angle newRect = oldRect.Rotate(newAngle - oldAngle, rotationCenter);
			return Item_Owner_Bounds.FromDiagramBounds(item, item.Owner(), newRect);
		}
		internal static double SnapRotationAngle(this IDiagramControl diagram, double angle, bool snap) {
			return snap ? MathHelper.SnapAngle(angle, SnapAngle) : angle;
		}
	}
}
