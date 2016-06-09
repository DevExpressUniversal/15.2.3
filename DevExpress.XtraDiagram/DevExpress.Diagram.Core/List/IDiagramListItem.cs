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
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Internal;
using System.Collections.ObjectModel;
using DevExpress.Utils;
using System.Windows.Documents;
using System.ComponentModel;
using System.Windows.Media;
using DevExpress.Diagram.Core.Themes;
using DevExpress.Utils.Serializing;
using System.IO;
using DevExpress.Diagram.Core.TypeConverters;
using System.Windows.Controls;
namespace DevExpress.Diagram.Core {
	public interface IDiagramList : IDiagramItem {
		[XtraSerializableProperty]
		Orientation Orientation { get; set; }
	}
	public class DiagramListController : DiagramContainerControllerBase {
		new IDiagramList Item { get { return (IDiagramList)base.Item; } }
		Orientation Orientation { get { return Item.Orientation; } }
		public DiagramListController(IDiagramList item) : base(item) {
		}
		internal override void ChildChanged(IDiagramItem child, ItemChangedKind kind) {
			base.ChildChanged(child, kind);
			if(kind == ItemChangedKind.Added || kind == ItemChangedKind.Removed || kind == ItemChangedKind.Bounds) {
				UpdateLayout();
			}
		}
		public override void OnActualSizeChanged() {
			base.OnActualSizeChanged();
			UpdateLayout();
		}
		Locker updateLayoutLocker = new Locker();
		void UpdateLayout() {
			updateLayoutLocker.DoLockedActionIfNotLocked(() => {
				var unitSize = Orientation.GetSize(Item.ActualSize) / Item.NestedItems.Sum(x => x.Weight);
				var breadth = Orientation.Rotate().GetSize(Item.ActualSize);
				var offset = 0d;
				Item.NestedItems.ForEach(x => {
					var itemSize = unitSize * x.Weight;
					x.SetBounds(new Rect(Orientation.MakePoint(offset, 0), Orientation.MakeSize(itemSize, breadth)));
					offset += itemSize;
				});
			});
		}
		protected internal override Rect GetMaxChildBounds(IDiagramItem child, Direction direction) {
			var index = child.GetIndexInOwnerCollection();
			if(index < 0)
				throw new InvalidOperationException();
			var maxBounds = base.GetMaxChildBounds(child, direction);
			maxBounds = Orientation.Rotate().SetSide(maxBounds, Side.Near, -MathHelper.VeryLargeValue);
			maxBounds = Orientation.Rotate().SetSide(maxBounds, Side.Far, 2 * MathHelper.VeryLargeValue);
			var nearSideDelta = index == 0
				? -MathHelper.VeryLargeValue
				: CalcNeighboursMargin(Item.NestedItems.Take(index).ToArray(), Orientation, direction);
			maxBounds = Orientation.SetSide(maxBounds, Side.Near, Orientation.GetSide(maxBounds, Side.Near) + nearSideDelta);
			var farSideDelta = index == Item.NestedItems.Count - 1
				? -MathHelper.VeryLargeValue
				: CalcNeighboursMargin(Item.NestedItems.Reverse().Take(Item.NestedItems.Count - 1 - index).ToArray(), Orientation, direction);
			maxBounds = Orientation.SetSide(maxBounds, Side.Far, Orientation.GetSide(maxBounds, Side.Far) - farSideDelta);
			return maxBounds;
		}
		static double CalcNeighboursMargin(IDiagramItem[] items, Orientation orientation, Direction direction) {
			return items.Take(items.Length - 1).Sum(x => orientation.GetSize(x.ActualSize)) 
				+ orientation.GetSize(items.Last().Controller.GetMinResizingSize(direction.Yield()));
		}
		internal override Size GetMinResizingSize(IEnumerable<Direction> directions) {
			var childrenSizes = Item.NestedItems.Select(x => x.Controller.GetMinResizingSize(directions));
			return Orientation.MakeSize(childrenSizes.Sum(x => Orientation.GetSize(x)), childrenSizes.Max(x => Orientation.Rotate().GetSize(x)));
		}
		protected override bool CanHaveNoChildren { get { return false; } }
	}
}
