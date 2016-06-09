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
using System.Windows;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
namespace DevExpress.Xpf.Docking.Design {
	public class BaseLayoutItemPlacementAdapter : PlacementAdapter {
		IPlacementHelper placementHelper;
		Rect initialRect;
		public BaseLayoutItemPlacementAdapter() {
			initialRect = new Rect();
		}
		public override void BeginPlacement(ModelItem item) {
			if(item == null) throw new ArgumentNullException("item");
			placementHelper = PlacementHelperFactory.CreatePlacementHelper(item);
			initialRect = item.View.RenderSizeBounds;
		}
		public override bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
			return placementHelper != null ? placementHelper.CanSetPosition(intent, position) : intent == PlacementIntent.Size;
		}
		public override RelativeValueCollection GetPlacement(ModelItem item, params RelativePosition[] positions) {
			return new RelativeValueCollection();
		}
		public override Rect GetPlacementBoundary(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) {
			return new Rect();
		}
		public override Rect GetPlacementBoundary(ModelItem item) {
			return new Rect();
		}
		public override void SetPlacements(ModelItem item, PlacementIntent intent, RelativeValueCollection placement) {
			SetPlacementsCore(item, intent, placement);
		}
		public override void SetPlacements(ModelItem item, PlacementIntent intent, params RelativeValue[] positions) {
			SetPlacementsCore(item, intent, positions);
		}
		void SetPlacementsCore(ModelItem item, PlacementIntent intent, IEnumerable<RelativeValue> positions) {
			Thickness r = new Thickness();
			foreach(RelativeValue value in positions) {
				if(double.IsNaN(value.Value)) continue;
				RelativePosition pos = value.Position;
				if(pos == RelativePositions.RightSide) {
					double diff = -initialRect.Width + value.Value;
					r.Right += diff;
				}
				if(pos == RelativePositions.LeftSide) {
					double diff = -initialRect.X + value.Value;
					r.Left += diff;
				}
				if(pos == RelativePositions.TopSide) {
					double diff = -initialRect.Y + value.Value;
					r.Top += diff;
				}
				if(pos == RelativePositions.BottomSide) {
					double diff = -initialRect.Height + value.Value;
					r.Bottom += diff;
				}
			}
			placementHelper.SetPlacement(r);
		}
	}
	interface IPlacementHelper {
		void SetPlacement(Thickness diff);
		bool CanSetPosition(PlacementIntent intent, RelativePosition position);
	}
	static class PlacementHelperFactory {
		static bool IsMdiPanel(ModelItem item) {
			return item.Parent.Is<DocumentGroup>() && object.Equals(item.Parent.Properties["MDIStyle"].ComputedValue, MDIStyle.MDI);
		}
		public static IPlacementHelper CreatePlacementHelper(ModelItem item) {
			if(item.Is<DocumentPanel>() && IsMdiPanel(item)) {
				return new DocumentPanelPlacementHelper(item);
			}
			if(item.Parent.Is<LayoutGroup>())
				return new LayoutGroupChildPlacementHelper(item);
			return new EmptyPlacementHelper();
		}
		class EmptyPlacementHelper : IPlacementHelper {
			#region IPlacementHelper Members
			public void SetPlacement(Thickness diff) { }
			public bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
				return false;
			}
			#endregion
		}
		class DocumentPanelPlacementHelper : IPlacementHelper {
			Rect InitialRect;
			ModelItem Item;
			public DocumentPanelPlacementHelper(ModelItem item) {
				Item = item;
				Size mdiSize = (Size)Item.Properties["MDISize"].ComputedValue;
				if(double.IsNaN(mdiSize.Width) || double.IsNaN(mdiSize.Height)) {
					mdiSize = item.View.RenderSizeBounds.Size;
				}
				InitialRect = new Rect((Point)Item.Properties["MDILocation"].ComputedValue, mdiSize);
			}
			#region IPlacementHelper Members
			public void SetPlacement(Thickness diff) {
				Point location = new Point(Math.Ceiling(InitialRect.X + diff.Left), Math.Ceiling(InitialRect.Y + diff.Top));
				Size size = new Size(Math.Ceiling(InitialRect.Width + diff.Right - diff.Left), Math.Ceiling(InitialRect.Height + diff.Bottom - diff.Top));
				Rect result = new Rect(location, size);
				Item.Properties["MDILocation"].SetValue(result.Location);
				Item.Properties["MDISize"].SetValue(result.Size);
			}
			public bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
				return intent == PlacementIntent.Size;
			}
			#endregion
		}
		class LayoutGroupChildPlacementHelper : IPlacementHelper {
			ModelItem Item;
			public LayoutGroupChildPlacementHelper(ModelItem item) {
				Item = item;
			}
			void ApplyChange(ModelItem prev, double change, bool isHorizontal) {
				var bounds1 = Item.View.RenderSizeBounds;
				var bounds2 = prev.View.RenderSizeBounds;
				string property = isHorizontal ? "ItemWidth" : "ItemHeight";
				double pxLenght1 = isHorizontal ? bounds1.Width : bounds1.Height;
				double pxLenght2 = isHorizontal ? bounds2.Width : bounds2.Height;
				var length1 = (GridLength)Item.Properties[property].ComputedValue;
				var length2 = (GridLength)prev.Properties[property].ComputedValue;
				double x1 = (pxLenght1 + change) * (length1.Value + length2.Value) / (pxLenght2 + pxLenght1);
				double x2 = length1.Value + length2.Value - x1;
				Item.Properties[property].SetValue(new GridLength(Math.Round(x1, 2), GridUnitType.Star));
				prev.Properties[property].SetValue(new GridLength(Math.Round(x2, 2), GridUnitType.Star));
			}
			#region IPlacementHelper Members
			public void SetPlacement(Thickness diff) {
				ModelItem parent = Item.Parent;
				int index = parent.ItemsProperty().IndexOf(Item);
				ModelItem prev = parent.ItemsProperty().IsValidIndex(index - 1) ? parent.ItemsProperty()[index - 1] : null;
				ModelItem next = parent.ItemsProperty().IsValidIndex(index + 1) ? parent.ItemsProperty()[index + 1] : null;
				if(diff.Right != 0) {
					ApplyChange(next, diff.Right, true);
				}
				if(diff.Left != 0) {
					ApplyChange(prev, -diff.Left, true);
				}
				if(diff.Top != 0) {
					ApplyChange(prev, -diff.Top, false);
				}
				if(diff.Bottom != 0) {
					ApplyChange(next, diff.Bottom, false);
				}
			}
			public bool CanSetPosition(PlacementIntent intent, RelativePosition position) {
				return intent == PlacementIntent.Size;
			}
			#endregion
		}
	}
}
