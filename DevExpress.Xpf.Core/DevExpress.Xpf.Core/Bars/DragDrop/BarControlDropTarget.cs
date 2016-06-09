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
using System.Linq;
using System.Text;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Bars.Customization;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class DropIndicator : ContentControl {
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateState();
		}
		protected virtual void UpdateState() {
			Orientation orientation = BarDragProvider.GetDropIndicatorOrientation(this);
			VisualStateManager.GoToState(this, orientation.ToString(), false);
		}
	}
	public class EmptyDropTargetFactory : IDropTargetFactory {
		#region IDropTargetFactory Members
		public IDropTarget CreateDropTarget(UIElement dropTargetElement) {
			return EmptyDropTarget.Instance;
		}
		#endregion
	}
	public class BarControlDropTarget : IDropTarget {
		public bool AlwaysAdd { get; set; }
		LinksControl linksControl;
		public BarControlDropTarget(LinksControl linksControl) {
			this.linksControl = linksControl;
		}
		public LinksControl LinksControl { get { return linksControl; } }
		protected virtual BarItemLinkControlBase GetItemLinkControl(Point pt) {
			bool insertAfter = false;
			return GetItemLinkControl(pt, ref insertAfter);
		}
		protected virtual bool IsAfter(Point p, BarItemLinkControl linkControl, Rect linkControlBounds) {			
			return p.X > linkControlBounds.Width / 2;
		}
		protected BarItemLinkControlBase GetItemLinkControl(Point pt, ref bool insertAfter) {
			if(AlwaysAdd) {
				if(LinksControl.Items.Count == 0)
					return null;
				BarItemLinkControl linkControl = LinksControl.GetLinkControl(linksControl.Items.Count - 1) as BarItemLinkControl;
				if(linkControl == null)
					return linkControl;
				insertAfter = true;
				return linkControl;
			}
			insertAfter = false;
			for (int i = 0; i < LinksControl.Items.Count; i++) {
				BarItemLinkControl linkControl = LinksControl.GetLinkControl(i) as BarItemLinkControl;
				if(linkControl == null)
					continue;
				Point linkControlLeftTopCorner = linkControl.TranslatePointWithoutTransform(new Point(0,0));
				Point linkControlLeftRightBottomCorner = linkControl.TranslatePointWithoutTransform(new Point(linkControl.ActualWidth, linkControl.ActualHeight));
				Rect linkControlBounds = new Rect(linkControlLeftTopCorner, linkControlLeftRightBottomCorner);
				Point p = LinksControl.ItemsPresenter.TranslatePoint(pt, linkControl);
				if(linkControlBounds.Contains(p)) {
					insertAfter = IsAfter(p, linkControl, linkControlBounds);
					return linkControl;
				}
			}
			return null;
		}
		protected virtual BarItemLinkControlBase GetDropItemLinkControl(Point pt) {
			for(int i = 0; i < LinksControl.Items.Count; i++) {
				BarItemLinkControlBase linkControl = LinksControl.GetLinkControl(i);
				Point localPoint = LinksControl.ItemsPresenter.TranslatePoint(pt, linkControl);
				if(localPoint.X < linkControl.ActualWidth / 2 && localPoint.Y >= 0 && localPoint.Y < linkControl.ActualHeight)
					return linkControl;
			}
			return null;
		}
		[ThreadStatic]
		Tuple<BarControlDropTarget, PositionedCompatibilityAdorner> currentDropAdorner;
		PositionedCompatibilityAdorner DropAdorner { get { return currentDropAdorner.If(x=>x.Item1==this).With(x=>x.Item2); } }
		protected void SetDropAdorner(PositionedCompatibilityAdorner adorner) {
			var cAdorner = currentDropAdorner.With(x => x.Item2);
			var cOwner = currentDropAdorner.With(x => x.Item1);
			if (cAdorner == adorner)
				return;
			if (currentDropAdorner != null) {
				cAdorner.Destroy();
				currentDropAdorner = null;
			}
			if (adorner == null)
				return;
			currentDropAdorner = new Tuple<BarControlDropTarget, PositionedCompatibilityAdorner>(this, adorner);
		}
		CompatibilityAdornerContainer AdornerContainer { get { return LinksControl.PanelAdornerContainer; } } 
		protected DropIndicator DropIndicator { get; private set; }
		protected virtual void UpdateDropIndicatorParams(ContentControl dropIndicator) {
			dropIndicator.Height = LinksControl.ItemsPresenter.ActualHeight;
		}
		protected virtual void CreateDropIndicatorAdorner() {
			DropIndicator = new DropIndicator();
			DropIndicator.Style = LinksControl.DropIndicatorStyle;
			UpdateDropIndicatorParams(DropIndicator);
			SetDropAdorner(new PositionedCompatibilityAdorner(DropIndicator));
			if(AdornerContainer == null)
				throw new ArgumentNullException("BarAdornerContainer was not found in template of LinksControl by name 'PART_Adorner'. LinksControlType = " + LinksControl.GetType().FullName + ".");
			AdornerContainer.Initialize(DropAdorner);
		}
		protected virtual void DestroyDropAdorner() {
			if (DropAdorner != null)
				SetDropAdorner(null);
		}
		protected void UpdateAdornerPosition(Point pt) {
			bool insertArter = false;
			BarItemLinkControl selLink = GetItemLinkControl(pt, ref insertArter) as BarItemLinkControl;
			if(selLink == null) return;
			Point adornerLocation = GetAdornerLocation(selLink, insertArter);
			DropAdorner.UpdateLocation(adornerLocation);
		}
		protected virtual Point GetAdornerLocation(BarItemLinkControl linkControl, bool insertArter) {
			GeneralTransform transform = linkControl.TransformToVisual(LinksControl.ItemsPresenter);
			Point selLinkLeftTopCorner = linkControl.TranslatePointWithoutTransform(new Point(0, 0));
			Point selLinkRightTopCorner = linkControl.TranslatePointWithoutTransform(new Point(linkControl.ActualWidth, 0));
			return insertArter ? transform.Transform(selLinkRightTopCorner) : transform.Transform(selLinkLeftTopCorner);
		}
		protected virtual void InsertObjectFrom(UIElement source, BarItemLinkControlBase selLink, bool insertAfter) {
			InsertObjectFrom(source, selLink.With(x => x.LinksControl), selLink, insertAfter);
		}
		protected virtual void InsertObjectFrom(UIElement source, LinksControl target, BarItemLinkControlBase selLink, bool insertAfter) {
			var strategy = BarNameScope.GetService<ICustomizationService>(target).With(x => x.CustomizationHelper).With(x => x.Strategy);
			if (strategy == null)
				return;
			IBarItem sourceItem = (source as BarItemList).With(x => x.DragItem.DataContext as BarItemInfo).With(x => x.Item);
			BarItemLinkControl linkControl = BarDragDropElementHelper.Current.With(x => x.Owner as BarItemLinkControl);
			sourceItem = sourceItem ?? linkControl.With(x => x.Link);
			strategy.DragItem(sourceItem, target.With(x=>x.LinksHolder), selLink.With(x => x.LinkBase as BarItemLink), linkControl.Return(BarDragProvider.GetDragTypeCore, () => DragType.Copy), insertAfter);
		}
		protected virtual int GetLinkControlIndex(BarItemLinkControlBase selLink) {
			for(int i = 0; i < LinksControl.Items.Count; i++) {
				if(((BarItemLinkInfo)LinksControl.Items[i]).LinkControl == selLink) return i;
			}
			return -1;
		}
		protected virtual BarItemLinkBase CloneLink(BarItemLink link) {
			BarStaticItem staticItem = link.Item as BarStaticItem;
			BarItemLinkBase res = null;
			if(staticItem != null && staticItem.IsPrivate && staticItem.DataContext is BarItem) {
				res = (staticItem.DataContext as BarItem).CreateLink();
			}
			else {
				res = link.Item.CreateLink();
				res.Assign(link);
			}
			return res;
		}
		protected virtual void InsertLink(BarItemLink link, BarItemLinkControlBase selLink, int insertIndex) {
			if(insertIndex == -1) {
				LinksControl.ItemLinks.Add(CloneLink(link));
			}
			else {
				if(BarItem.IsContainsItself(link.Item as ILinksHolder, selLink.LinkBase.Links.Holder))
					return;
				selLink.LinkBase.Links.Insert(insertIndex, CloneLink(link));
		}
		}
		protected virtual void InsertBarItemFrom(BarItem barItem, BarItemLinkControlBase selLink, bool insertAfter) {
			if(selLink == null) {
				LinksControl.ItemLinks.Add(CreateLink(barItem));
			} else {
				BarItemLinkCollection itemLinks = GetInsertItemLinks(selLink);
				if(BarItem.IsContainsItself(barItem as ILinksHolder, itemLinks.Holder))
					return;
				BarItemLinkBase link = GetLinkByControl(selLink);
				int insertIndex = GetInsertIndex(itemLinks, link, insertAfter);
				if(insertIndex == -1) {
					itemLinks.Add(CreateLink(barItem));
				}
				itemLinks.Insert(insertIndex, CreateLink(barItem));
			}
		}
		BarItemLinkBase CreateLink(BarItem item) {			
			BarItemLinkBase link = item.CreateLink();
			link.CreatedByCustomizationDialog = true;
			return link;
		}
		protected virtual BarItemLinkBase GetLinkByControl(BarItemLinkControlBase link) {
			return link.LinkBase;
		}
		protected virtual BarItemLinkCollection GetInsertItemLinks(BarItemLinkControlBase selLink) {
			if(selLink.LinkBase.Links.Holder is BarListItem) {
				return selLink.LinksControl.ItemLinks;
			}
			return selLink.LinkBase.Links;
		}
		protected virtual int GetInsertIndex(BarItemLinkCollection itemLinks, BarItemLinkBase link, bool insertAfter) {
			int insertIndex = itemLinks.IndexOf(link);
			if(insertAfter) insertIndex++;
			return Math.Max(0, insertIndex);
		}
		#region IDropTarget Members
		void IDropTarget.Drop(UIElement source, Point pt) {
			if(!CheckNameScope(source))
				return;
			bool insertAfter = false;
			BarItemLinkControlBase selLink = GetItemLinkControl(pt, ref insertAfter);
			InsertObjectFrom(source, LinksControl, selLink, insertAfter);
			DestroyDropAdorner();
		}
		void IDropTarget.OnDragLeave() {
			DestroyDropAdorner();
		}
		protected bool CheckNameScope(UIElement source) {			
			var ch = BarNameScope.GetService<ICustomizationService>(source).With(x => x.CustomizationHelper);
			return ch == null ? false : ch == BarNameScope.GetService<ICustomizationService>(source).With(x => x.CustomizationHelper);
		}
		protected UIElement GetDragSource(UIElement source) {
			if (!(source is BarItemList))
				source = BarDragDropElementHelper.Current.With(x => x.Owner as UIElement);
			return source;
		}
		void IDropTarget.OnDragOver(UIElement source, Point pt) {
			UIElement dragSource = GetDragSource(source);
			if (!CheckNameScope(source) || dragSource == null || this.LinksControl.With(x=>x.LinksHolder).If(x=>x.IsMergedState).ReturnSuccess()) {
				BarDragProvider.SetDragTypeCore(source, DragType.Remove);
				return;
			}
			if(DropAdorner == null)
				CreateDropIndicatorAdorner();
			UpdateAdornerPosition(pt);
			bool canCopyItem = !(dragSource is BarItemList);
			DragType dragType = KeyboardHelper.IsControlPressed && canCopyItem ? DragType.Copy : DragType.Move;
			BarDragProvider.SetDragTypeCore(dragSource, dragType);
			CheckOpenSubItem(pt, dragSource);
		}
		#endregion
		protected virtual void CheckOpenSubItem(Point pt, UIElement source) {
			var ilc = GetItemLinkControl(pt);
			var popup = (ilc as IPopupOwner).With(x => x.Popup).With(x => x.Popup);
			if (popup != null) {
				((IPopupOwner)ilc).ShowPopup();
				if (popup.IsOpen)
					return;
			}
			if (ilc.With(BarManagerHelper.GetPopup) == null)
				PopupMenuManager.CloseAllPopups();			
		}
		protected virtual BarItem GetDragItem(UIElement source) {
			BarItemList list = source as BarItemList;
			if(list != null)
				return ((BarItemInfo)list.DragItem.DataContext).Item;
			BarItemLinkInfo linkInfo = BarDragDropElementHelper.Current.With(x => x.Owner as BarItemLinkControl).With(x=>x.LinkInfo);
			return linkInfo.Link.Item;
		}
	}
}
