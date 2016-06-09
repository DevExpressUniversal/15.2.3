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
using System.Windows.Media;
using System.Windows.Documents;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
#if !SL
using DevExpress.Xpf.Core.HitTest;
using HitTestResult = DevExpress.Xpf.Core.HitTest.HitTestResult;
using HitTestResultBehavior = DevExpress.Xpf.Core.HitTest.HitTestResultBehavior;
using PointHitTestParameters = DevExpress.Xpf.Core.HitTest.PointHitTestParameters;
#endif
namespace DevExpress.Xpf.Core.Native {
	public abstract class FloatingContainerDragElement : IDragElement {
		protected readonly FloatingContainer container;
		protected FloatingContainerDragElement(FloatingContainer container) {
			this.container = container;
#if !SL
			container.UseScreenCoordinates = true;
#endif
		}
		#region IDragElement Members
		public void UpdateLocation(Point newPos) {
			container.FloatLocation = CorrectLocation(newPos);
			UpdateContainer();
		}
		public virtual void Destroy() {
			container.Close();
		}
		#endregion
		protected virtual void UpdateContainer() {
			container.IsOpen = true;
		}
		protected abstract Point CorrectLocation(Point newPos);		
	}
	public abstract class CustomDragElement: FloatingContainerDragElement {
		protected CustomDragElement(FloatingContainer container)
			: base(container) {
			container.ShowActivated = false;
		}
		protected CustomDragElement()
			: this(FloatingContainerFactory.Create(FloatingMode.Window)) {
		}
	}
	public abstract class HeaderDragElementBase : CustomDragElement {
		public const int RemoveColumnDragIconMargin = 50;
		public bool IsTouchEnabled { get; private set; }
		public ContentPresenter DragPreviewElement { get; private set; }
		public FloatingWindowContainer WindowContainer { get { return (FloatingWindowContainer)container; } }
		public FloatingContainer FloatingContainer { get { return container; } }
		double widthCorrectionOffset;
		double heightCorrectionOffset;
		bool allowTransparency;
		protected FrameworkElement HeaderElement { get; private set; }
		protected abstract FrameworkElement HeaderButton { get; }
		protected abstract string DragElementTemplatePropertyName { get; }
		protected abstract void AddGridChild(object child);
		protected abstract void RemoveGridChild(object child);
		protected abstract void SetDragElementSize(FrameworkElement elem, Size size);
		public HeaderDragElementBase(FrameworkElement headerElement, DependencyObject dataContext, Point offset)
			: this(headerElement, dataContext, offset, FloatingContainerFactory.CheckPopupHost(headerElement)) {
		}
		public HeaderDragElementBase(FrameworkElement headerElement, DependencyObject dataContext, Point offset, FloatingMode mode)
			: base(FloatingContainerFactory.Create(mode)) {
			HeaderElement = headerElement; 
#if !SL
			ChangeTouchEnabled(LayoutHelper.GetTopLevelVisual(headerElement).Parent is Popup);
			container.DeactivateOnClose = true;
			container.FlowDirection = HeaderElement.FlowDirection;
			container.UseActiveStateOnly = true;
#endif
			DragPreviewElement = TemplateHelper.CreateBindedContentPresenter(
				HeaderElement, DragElementTemplatePropertyName, dataContext);
			DragPreviewElement.IsHitTestVisible = false;
			DragPreviewElement.VerticalAlignment = VerticalAlignment.Top;
			DragPreviewElement.HorizontalAlignment = HorizontalAlignment.Left;
			AddGridChild(DragPreviewElement);
			double width = Math.Min(200, Math.Max(100, HeaderButton.ActualWidth));
			double height = Math.Min(75, HeaderButton.ActualHeight);
			SetDragElementSize(DragPreviewElement, new Size(width, height));
#if !SL
			allowTransparency = FloatingContainerFactory.CheckPopupHost(headerElement) != FloatingMode.Popup || !BrowserInteropHelper.IsBrowserHosted;			
#else
			allowTransparency = true;
#endif
			SetDragElementAllowTransparency(DragPreviewElement, allowTransparency);
			int transparencyCorrection = allowTransparency ? RemoveColumnDragIconMargin : 0;
			DragPreviewElement.Width = width + transparencyCorrection;
			DragPreviewElement.Height = height + transparencyCorrection;
#if SL
			transparencyCorrection = 0;
#endif
			widthCorrectionOffset = Math.Min(0, (HeaderButton.ActualWidth == width ? 0 : width - offset.X - SystemParameters.MinimumHorizontalDragDistance));
			heightCorrectionOffset = Math.Min(0, (HeaderButton.ActualHeight == height ? 0 : height - offset.Y - SystemParameters.MinimumVerticalDragDistance));
			container.Owner = HeaderButton;
			container.Content = DragPreviewElement;
			container.ShowContentOnly = true;
			container.FloatSize = new Size(DragPreviewElement.Width, DragPreviewElement.Height);			
		}
		protected virtual void SetDragElementAllowTransparency(FrameworkElement elem, bool allowTransparency) { }
		protected override Point CorrectLocation(Point newPos) {
			PointHelper.Offset(ref newPos, -widthCorrectionOffset, -heightCorrectionOffset);
#if !SL
			if(container.Owner.FlowDirection == FlowDirection.RightToLeft) {
				newPos = container.CorrectRightToLeftLocation(newPos);
				if(container is FloatingAdornerContainer) {
					if(!((FloatingAdornerContainer)container).InvertLeftAndRightOffsets)
						return newPos;
				}
				newPos.X = allowTransparency ? newPos.X - RemoveColumnDragIconMargin : 0;
			}
			if(IsTouchEnabled)
				newPos.X += DragPreviewElement.Width;
#endif
			return newPos;
		}
		public override void Destroy() {
			base.Destroy();
			RemoveGridChild(DragPreviewElement);
		}
#if !SL
		void ChangeTouchEnabled(bool isPopupContainer) {
			IsTouchEnabled = isPopupContainer && SystemParameters.MenuDropAlignment;
		}
#endif
	}
	public abstract class HeaderDropTargetBase : IDropTarget {
#if !SL
		PositionedAdornerContainer dragAdorner;
		public PositionedAdornerContainer DragAdorner { get { return dragAdorner; } }
#else
		public FloatingContainer DragAdorner { get; private set; }
#endif
		public Panel Panel { get; private set; }
		protected virtual IList Children { get { return Panel.Children; } }		
		public HeaderDropTargetBase(Panel panel) {
			Panel = panel;
		}
		#region IDropTarget Members
		void IDropTarget.OnDragOver(UIElement source, Point pt) {
			object dragAnchor = null;
			if(CanDropCore(source, pt, out dragAnchor, true)) {
				if(DragAdorner == null)
					CreateDragIndicatorAdorner(source);
				UpdateDragAdornerLocationCore(source, dragAnchor);
			} else {
				DestroyDragAdorner();
			}
		}
		protected virtual bool CanDropCore(UIElement source, Point pt, out object dragAnchor, bool isDrag) {
			int dropIndex = GetDropIndexFromDragSource(source, pt);
			int dragIndex = GetDragIndex(dropIndex);
			dragAnchor = isDrag ? dragIndex : dropIndex;
			return (CanDrop(source, dropIndex) && (int)dragAnchor != -1);
		}
		void IDropTarget.OnDragLeave() {
			DestroyDragAdorner();
		}
		void IDropTarget.Drop(UIElement source, Point pt) {
			object dropAnchor = null;
			if(CanDropCore(source, pt, out dropAnchor, false))
				MoveColumnToCore(source, dropAnchor);
		}
		#endregion
		protected abstract FrameworkElement GridElement { get; }
		protected abstract FrameworkElement DragIndicatorTemplateSource { get; }
		protected abstract string DragIndicatorTemplatePropertyName { get; }
		protected abstract string HeaderButtonTemplateName { get; }
		protected virtual void MoveColumnToCore(UIElement source, object dropAnchor) {
			MoveColumnTo(source, (int)dropAnchor);
		}
		protected abstract void MoveColumnTo(UIElement source, int dropIndex);
		protected abstract int DropIndexCorrection { get; }
		protected abstract HeaderHitTestResult CreateHitTestResult();
		protected abstract bool CanDrop(UIElement source, int dropIndex);
		protected abstract bool IsSameSource(UIElement element);
		protected abstract int GetRelativeVisibleIndex(UIElement element);
		protected abstract int GetVisibleIndex(DependencyObject obj);
		protected abstract Orientation GetDropPlaceOrientation(DependencyObject element);
		protected abstract void SetDropPlaceOrientation(DependencyObject element, Orientation value);		
		protected abstract void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value);
#if !SL
		protected UIElement TopContainer { get { return LayoutHelper.GetTopContainerWithAdornerLayer(AdornableElement); } }
#else
		protected UIElement TopContainer { get { return Application.Current.RootVisual; } }
#endif
		protected virtual int GetDragIndex(int dropIndex) { return dropIndex; }
		protected virtual int ChildrenCount { get { return Children.Count; } }
		protected int GetChildHeaderIndexByPoint(Point pt, out bool isFarCorner) {
			HeaderHitTestResult result = GetColumnHeaderHitTestResult(pt);
			isFarCorner = GetIsFarCorner(result, pt);
			if(result.HeaderElement == null)
				return ChildrenCount;
			if(!IsVisible(result.HeaderElement))
				return -1;
			int visibleIndex = GetVisibleIndex(result.HeaderElement);
			return visibleIndex >= 0 ? visibleIndex : ChildrenCount;
		}
		protected virtual bool GetIsFarCorner(HeaderHitTestResult result, Point point) {
			return result.DropPlace == DropPlace.Next;
		}
		protected bool IsVisible(DependencyObject dObj) {
			UIElement element = dObj as UIElement;
#if !SL
			return element != null && element.IsVisible;
#else
			return element != null;
#endif
		}
		protected HeaderHitTestResult GetColumnHeaderHitTestResult(Point pt) {
			HeaderHitTestResult result = CreateHitTestResult();
#if !SL
			HitTestHelper.HitTest(AdornableElement, null, result.HitTestCallBack, new PointHitTestParameters(pt));
#else
			HitTestHelper.HitTest(AdornableElement, null, result.HitTestCallBack, new PointHitTestParameters(pt, false));
#endif
			return result;
		}
		protected virtual int GetDropIndexFromDragSource(UIElement element, Point pt) {
			bool isFarCorner;
			int dropVisibleIndex = GetChildHeaderIndexByPoint(pt, out isFarCorner);
			if(dropVisibleIndex == -1)
				return -1;
			int oldvisibleIndex = GetRelativeVisibleIndex(element);
			if(oldvisibleIndex == -1)
				return isFarCorner ? dropVisibleIndex + 1 : dropVisibleIndex;
			return GetColumnVisibleIndexFromDragSource(oldvisibleIndex, dropVisibleIndex, isFarCorner, IsSameSource(element));
		}		
		int GetColumnVisibleIndexFromDragSource(int oldVisibleIndex, int newVisibleIndex, bool isFarCorner, bool sameSource) {
			if(oldVisibleIndex == newVisibleIndex && sameSource)
				return -1;
			if(isFarCorner)
				newVisibleIndex++;
			if((oldVisibleIndex == newVisibleIndex || oldVisibleIndex + 1 == newVisibleIndex) && sameSource)
				return -1;
			return newVisibleIndex;
		}
		protected virtual void DestroyDragAdorner() {
			if(DragAdorner == null)
				return;
#if !SL
			AdornerLayer.GetAdornerLayer(TopContainer).Remove(DragAdorner);
			this.DragAdorner.Resources.MergedDictionaries.Clear();
			this.dragAdorner = null;
#else            
			DragAdorner.IsOpen = false;
			DragAdorner = null;
#endif
		}
		protected UIElement DragIndicator {
			get {
#if !SL
				return DragAdorner != null ? DragAdorner.Child : null;
#else
				return DragAdorner != null ? DragAdorner.Content as UIElement : null;
#endif
			} 
		}
		void CreateDragIndicatorAdorner(UIElement sourceElement) {
			ContentPresenter dragIndicator = TemplateHelper.CreateBindedContentPresenter(DragIndicatorTemplateSource, DragIndicatorTemplatePropertyName, GetDragIndicatorDataContext(sourceElement));
			dragIndicator.IsHitTestVisible = false;
			Orientation dropPlaceOrientation = GetDropPlaceOrientation(Panel);
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(dropPlaceOrientation);
			double size = sizeHelper.GetSecondarySize(AdornableElement.RenderSize) / 2;
			if(ChildrenCount > 0)
				size = sizeHelper.GetSecondarySize(GetColumnHeaderButton((FrameworkElement)GetHeaderButtonOwner(0)).RenderSize);
			SetColumnHeaderDragIndicatorSize(dragIndicator, size);
			SetDropPlaceOrientation(dragIndicator, dropPlaceOrientation);
#if !SL     
			AdornerLayer layer = AdornerLayer.GetAdornerLayer(TopContainer);
			this.dragAdorner = new PositionedAdornerContainer(AdornableElement, dragIndicator);
			this.dragAdorner.Resources.Clear();
			this.dragAdorner.Resources.MergedDictionaries.Add(GridElement.Resources);
			layer.Add(DragAdorner);
#else            
			DragAdorner = FloatingContainerFactory.Create(FloatingMode.Adorner);
			DragAdorner.Content = dragIndicator;
			DragAdorner.ShowContentOnly = true;
			DragAdorner.Owner = AdornableElement as FrameworkElement;
			DragAdorner.IsOpen = true;
#endif
		}
		protected virtual object GetDragIndicatorDataContext(UIElement sourceElement) { return null; }
		protected virtual UIElement AdornableElement { get { return Panel; } }
		protected virtual object GetHeaderButtonOwner(int index) {
			return Children[index];
		}
		protected virtual void UpdateDragAdornerLocationCore(UIElement sourceElement, object headerAnchor) {
			UpdateDragAdornerLocation((int)headerAnchor);
		}
		protected virtual void UpdateDragAdornerLocation(int headerIndex) {
			int index = Math.Min(ChildrenCount - 1, headerIndex);
			Rect r = index < 0 ?
				new Rect(2, AdornableElement.RenderSize.Height / 4, 0, 0) :
				LayoutHelper.GetRelativeElementRect(GetColumnHeaderButton((FrameworkElement)GetHeaderButtonOwner(index)) as FrameworkElement, AdornableElement);			
			Point location = r.TopLeft();
			if(headerIndex >= ChildrenCount) {
				SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(GetDropPlaceOrientation(AdornableElement));
				Size size = sizeHelper.CreateSize(sizeHelper.GetDefineSize(r.Size()), 0);
				PointHelper.Offset(ref location, size.Width, size.Height);
			}
#if !SL
			DragAdorner.UpdateLocation(CorrectDragIndicatorLocation(location));
#else
			DragAdorner.FloatLocation = CorrectDragIndicatorLocation(location);
#endif
		}
		protected virtual Point CorrectDragIndicatorLocation(Point point) {
			return point;
		}
		protected FrameworkElement GetColumnHeaderButton(FrameworkElement parentElement) {
			VisualTreeEnumerator en = new VisualTreeEnumerator(parentElement);
			while(en.MoveNext()) {
				FrameworkElement element = en.Current as FrameworkElement;
				if(element != null && element.Name == HeaderButtonTemplateName)
					return element;
			}
			return parentElement;
		}
		protected abstract class HeaderHitTestResult {
			public DependencyObject HeaderElement { get; private set; }
			public DropPlace DropPlace { get; private set; }
			public HeaderHitTestResult() {
				DropPlace = DropPlace.None;
			}
			public HitTestResultBehavior HitTestCallBack(HitTestResult result) {
				UIElement element = result.VisualHit as UIElement;
#if !SL
				if(element != null && !element.IsVisible)
				return HitTestResultBehavior.Continue;
#endif
				if(GetGridColumn(result.VisualHit) != null) {
					HeaderElement = result.VisualHit;
				}
				if(GetDropPlace(result.VisualHit) != DropPlace.None)
					DropPlace = GetDropPlace(result.VisualHit);				
				return HitTestResultBehavior.Continue;
			}
			protected abstract DependencyObject GetGridColumn(DependencyObject visualHit);
			protected abstract DropPlace GetDropPlace(DependencyObject visualHit);		
		}
	}
}
namespace DevExpress.Xpf.Core {
	public enum DropPlace { None, Next, Previous }
}
