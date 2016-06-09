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
using System.Windows.Documents;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public class PlacementAdorner : BaseSurfacedAdorner {
		public PlacementAdorner(UIElement container)
			: base(container) {
		}
		protected override BaseAdornerSurface CreateAdornerSurface() {
			return new AdornerSurface(this);
		}
		protected internal AdornerSurface PlacementSurface {
			get { return Surface as AdornerSurface; }
		}
		public void Register(UIElement placementElement) {
			PlacementSurface.AddPlacementElement(placementElement);
		}
		public void Unregister(UIElement placementElement) {
			PlacementSurface.RemovePlacementElement(placementElement);
		}
		public bool IsRegistered(UIElement placementElement) {
			return PlacementSurface.IsPlacementElementAdded(placementElement);
		}
		public void SetBoundsInContainer(UIElement placementElement, Rect bounds) {
			SetBoundsInContainerCore(placementElement, bounds);
		}
		protected virtual void SetBoundsInContainerCore(UIElement placementElement, Rect bounds) {
			placementElement.Measure(bounds.Size);
			PlacementSurface.SetBounds(placementElement, bounds);
		}
		public Rect GetBoundsInContainer(UIElement placementElement) {
			return PlacementSurface.GetBounds(placementElement);
		}
		public bool GetVisible(UIElement placementElement) {
			return PlacementSurface.GetVisible(placementElement);
		}
		public void SetVisible(UIElement placementElement, bool visible) {
			PlacementSurface.SetVisible(placementElement, visible);
		}
		public void BringToFront(UIElement placementElement) {
			PlacementSurface.BringToFront(placementElement);
		}
		#region Internal classes
		public class AdornerSurface : BaseAdornerSurface {
			readonly bool EnableValidation;
			Dictionary<UIElement, PlacementItemInfo> placementInfosCore;
			public AdornerSurface(PlacementAdorner adorner)
				: this(adorner, true) {
			}
			public AdornerSurface(PlacementAdorner adorner, bool enableValidation)
				: base(adorner) {
				this.placementInfosCore = new Dictionary<UIElement, PlacementItemInfo>();
				this.EnableValidation = enableValidation;
			}
			protected override Size ArrangeOverride(Size finalSize) {
				foreach(KeyValuePair<UIElement, PlacementItemInfo> pair in PlacementInfos) {
					pair.Key.Arrange(pair.Value.Bounds);
				}
				return finalSize;
			}
			protected internal PlacementAdorner Adorner {
				get { return BaseAdorner as PlacementAdorner; }
			}
			protected internal Dictionary<UIElement, PlacementItemInfo> PlacementInfos {
				get { return placementInfosCore; }
			}
			void ValidateElementIsNotAdded(UIElement placementElement) {
				if(IsPlacementElementAdded(placementElement))
					throw new ArgumentException("Element has already been added");
			}
			void ValidateElementIsAdded(UIElement placementElement) {
				if(!IsPlacementElementAdded(placementElement))
					throw new ArgumentException("Element has not been found");
			}
			PlacementItemInfo GetValidatedElement(UIElement placementElement) {
				if(EnableValidation) ValidateElementIsAdded(placementElement);
				return PlacementInfos[placementElement];
			}
			public void AddPlacementElement(UIElement placementElement) {
				if(EnableValidation) ValidateElementIsNotAdded(placementElement);
				PlacementInfos.Add(placementElement, new PlacementItemInfo(this, placementElement));
				Children.Add(placementElement);
			}
			public void RemovePlacementElement(UIElement placementElement) {
				if(EnableValidation) ValidateElementIsAdded(placementElement);
				PlacementInfos.Remove(placementElement);
				Children.Remove(placementElement);
			}
			public bool IsPlacementElementAdded(UIElement placementElement) {
				return PlacementInfos.ContainsKey(placementElement);
			}
			public void SetBounds(UIElement placementElement, Rect bounds) {
				GetValidatedElement(placementElement).Bounds = bounds;
				Adorner.Update();
			}
			public Rect GetBounds(UIElement placementElement) {
				return GetValidatedElement(placementElement).Bounds;
			}
			public bool GetVisible(UIElement placementElement) {
				return GetValidatedElement(placementElement).Visible;
			}
			public void SetVisible(UIElement placementElement, bool visible) {
				GetValidatedElement(placementElement).Visible = visible;
				Adorner.Update();
			}
			protected virtual void BringToFrontCore(UIElement placementElement) {
				int count = 0;
				foreach(KeyValuePair<UIElement, PlacementItemInfo> pair in PlacementInfos) {
					if(pair.Key == placementElement) continue;
					SetZIndex(pair.Key, count++);
				}
				SetZIndex(placementElement, count++);
			}
			public void BringToFront(UIElement placementElement) {
				if(EnableValidation) ValidateElementIsAdded(placementElement);
				BringToFrontCore(placementElement);
				Adorner.Update();
			}
		}
		public class PlacementItemInfo {
			AdornerSurface surfaceCore;
			UIElement placementItemCore;
			Rect boundsCore;
			bool visibleCore = false;
			public PlacementItemInfo(AdornerSurface surface, UIElement placementItem) {
				this.surfaceCore = surface;
				this.placementItemCore = placementItem;
			}
			public AdornerSurface Surface {
				get { return surfaceCore; }
			}
			public UIElement PlacementItem {
				get { return placementItemCore; }
			}
			public Rect Bounds {
				get { return boundsCore; }
				set {
					if(boundsCore == value) return;
					this.boundsCore = value;
					UpdateBounds();
				}
			}
			public bool Visible {
				get { return visibleCore; }
				set {
					this.visibleCore = value;
					CheckVisible();
				}
			}
			protected void UpdateBounds() {
			}
			protected void CheckVisible() {
				Visibility needed = Visible ? Visibility.Visible : Visibility.Collapsed;
				if(PlacementItem.Visibility != needed) PlacementItem.Visibility = needed;
			}
		}
		#endregion Internal classes
	}
}
