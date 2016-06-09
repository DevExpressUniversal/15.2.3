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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using System.Drawing;
using System.Linq;
namespace DevExpress.XtraMap.Native {
	public enum MapAnimationState { None = 0, Zoom = 4, Scroll = 1, ZoomScroll = 5, ZoomOnScrollComplete = 6, ScrollOnZoomComplete = 9, ScrollCompleted = 2, ZoomCompleted = 8 }
	public enum MapAnimationStartMode { Interactive, Runtime }
	public class AnimationController : MapDisposableObject {
		readonly InnerMap map;
		MapPoint cursorPosition = MapPoint.Empty;
		CoordPoint animatedCenterPoint;
		double animatedZoomLevel = InnerMap.DefaultZoomLevel;
		MapAnimation scrollAnimation;
		MapAnimation zoomAnimation;
		MapAnimationState state;
		bool needChangeCenterWhileZooming = true;
		double anchorZoom = InnerMap.DefaultZoomLevel;
		CoordPoint anchorCenter;
		MapAnimationStartMode animationStartMode = MapAnimationStartMode.Runtime;
		MapPoint initialZoomAnchorPoint = MapPoint.Empty;
		object timerLocker = new object();
		protected MapUnitConverter UnitConverter { get { return map.UnitConverter; } }
		protected internal MapPoint CursorPosition { get { return cursorPosition; } }
		protected internal bool NeedChangeCenterWhileZooming { get { return needChangeCenterWhileZooming; } }
		protected internal double AnchorZoom { get { return anchorZoom; } }
		protected internal CoordPoint AnchorCenter { get { return anchorCenter; } }
		protected internal MapPoint InitialZoomAnchorPoint {
			get { return initialZoomAnchorPoint; }
			set {
				if(initialZoomAnchorPoint == value)
					return;
				lock(timerLocker) {
					initialZoomAnchorPoint = value;
					OnInitialZoomAnchorPointChanged();
				}
			}
		}
		protected internal MapAnimation ScrollAnimation { get { return scrollAnimation; } }
		protected internal MapAnimation ZoomAnimation { get { return zoomAnimation; } }
		protected internal MapAnimationStartMode AnimationStartMode { get { return animationStartMode; } set { animationStartMode = value; } }
		protected internal virtual double CurrentScrollProgress { get { return scrollAnimation.Progress; } }
		protected internal virtual double CurrentZoomProgress { get { return zoomAnimation.Progress; } }
		protected internal bool InProgress { get { return ScrollAnimation.InProgress || ZoomAnimation.InProgress; } }
		public MapAnimationState AnimationState { get { return state; } protected set { state = value; } }
		protected internal CoordPoint AnimatedCenterPoint { 
			get { return animatedCenterPoint; } 
		}
		protected internal double AnimatedZoomLevel { get { return animatedZoomLevel; } }
		internal AnimationController(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
			scrollAnimation = new MapAnimation(this.map);
			zoomAnimation = new MapAnimation(this.map);
			this.anchorCenter = map.UnitConverter.PointFactory.CreatePoint(0, 0);
			this.animatedCenterPoint = map.UnitConverter.PointFactory.CreatePoint(0, 0);
		}
		protected override void DisposeOverride() {
			StopAnimation();
			scrollAnimation = null;
			zoomAnimation = null;
		}
		void StopAnimation() {
			this.scrollAnimation.Stop();
			this.zoomAnimation.Stop();
		}
		void UpdateAnchorZoom(double anchor) {
			anchorZoom = anchor;
		}
		void UpdateAnchorCenter(CoordPoint anchor) {
			this.anchorCenter = anchor;
		}
		internal double CalculateZoomLevel(double anchorZoom, double newZoom, double progress) {
			return anchorZoom + (newZoom - anchorZoom) * progress;
		}
		CoordPoint CalculateZoomCenterPoint(CoordPoint oldCenter, double oldZoom, double newZoom) {
			return map.MoveBeforeZooming(oldCenter, cursorPosition, oldZoom, newZoom);
		}
		internal CoordPoint CalculateScrollCenterPoint(CoordPoint anchorCenter, CoordPoint newCenter, double progress) {
			progress = map.OwnedControl.Capture && progress == 0.0 ? ScrollAnimation.FirstFrameProgress : progress;
			double offsetX = (newCenter.GetX() - anchorCenter.GetX()) * (1 - progress);
			double offsetY = (newCenter.GetY() - anchorCenter.GetY()) * (1 - progress);
			return newCenter.Offset(-offsetX, -offsetY);
		}
		internal protected MapAnimationState CalculateAnimationState(object sender, AnimationAction action) {
			bool zoomAnimationJustCompleted = ((sender == zoomAnimation) && (action == AnimationAction.Complete));
			bool scrollAnimationJustCompleted = ((sender == scrollAnimation) && (action == AnimationAction.Complete));
			return (MapAnimationState)(zoomAnimationJustCompleted ? 8 : 0) + (zoomAnimation.InProgress ? 4 : 0) + (scrollAnimationJustCompleted ? 2 : 0) + (scrollAnimation.InProgress ? 1 : 0);
		}
		void CalcParametersOnScroll() {
			animatedCenterPoint = CalculateScrollCenterPoint(anchorCenter, map.CenterPoint, CurrentScrollProgress);
		}
		void CalcParametersOnZoom() {
			double oldZoom = animatedZoomLevel;
			lock(timerLocker) {
				animatedZoomLevel = CalculateZoomLevel(anchorZoom, map.ZoomLevel, CurrentZoomProgress);
				animatedCenterPoint = CalculateZoomCenterPoint(animatedCenterPoint, oldZoom, animatedZoomLevel);
			}
		}
		void CalcParametersOnZoomScroll() {
			if(AnimationStartMode == MapAnimationStartMode.Interactive) 
				OnInteractiveZoomScroll();
			else
				animatedCenterPoint = CalculateScrollCenterPoint(anchorCenter, map.CenterPoint, CurrentScrollProgress);
			animatedZoomLevel = CalculateZoomLevel(anchorZoom, map.ZoomLevel, CurrentZoomProgress);
		}
		void OnInteractiveZoomScroll() {
			if(CurrentScrollProgress < CurrentZoomProgress)
				animatedCenterPoint = CalculateScrollCenterPoint(anchorCenter, map.CenterPoint, CurrentScrollProgress);
			else
				scrollAnimation.Stop();
		}
		void CalcParametersOnScrollOnZoomComplete() {
			InitialZoomAnchorPoint = MapPoint.Empty;
		}
		void CalcParametersOnZoomOnScrollComplete() {
			InitialZoomAnchorPoint = MapPoint.Empty;
			needChangeCenterWhileZooming = true;
			cursorPosition = UnitConverter.CoordPointToScreenPoint(animatedCenterPoint);
		}
		void CalcParametersOnZoomCompleted() {
			UpdateAnchorCenter(animatedCenterPoint);
			if(animationStartMode != MapAnimationStartMode.Interactive)
				animatedCenterPoint = map.CenterPoint;
		}
		void CalcParametersOnScrollCompleted() {
			needChangeCenterWhileZooming = true;
		}
		internal void StartScrollAnimation() {
			scrollAnimation.Start();
			FrameChanged(scrollAnimation, AnimationAction.FrameChanged);
		}
		void OnInitialZoomAnchorPointChanged() {
			this.cursorPosition = CalculateCursorPosition();
		}
		MapPoint CalculateCursorPosition() {
			if(InitialZoomAnchorPoint != MapPoint.Empty)
				return InitialZoomAnchorPoint;
			return new MapPoint(map.ContentRectangle.Width / 2.0, map.ContentRectangle.Height / 2.0);
		}
		protected internal void InitializeZoomAnimation() {
			if(scrollAnimation.InProgress) {
				bool interactive = AnimationStartMode == MapAnimationStartMode.Interactive;
				needChangeCenterWhileZooming = interactive;
				if(interactive)
					scrollAnimation.Stop();
			}
		}
		internal void StartZoomAnimation(double oldZoomLevel) {
			double intermediateZoomLevel = zoomAnimation.InProgress ? CalculateZoomLevel(anchorZoom, oldZoomLevel, CurrentZoomProgress) : AnimatedZoomLevel;
			UpdateAnchorZoom(intermediateZoomLevel);
			zoomAnimation.Start();
		}
		internal void SynchronizeCenterAndZoom() {
			animatedCenterPoint = map.CenterPoint;
			animatedZoomLevel = map.ZoomLevel;
			UpdateAnchorCenter(map.CenterPoint);
			UpdateAnchorZoom(map.ZoomLevel);
			this.cursorPosition = RectUtils.GetCenterPoint(map.ContentRectangle);
		}
		internal void FrameChanged(object sender, AnimationAction action) {
			bool updateScale = false;
			AnimationState = CalculateAnimationState(sender, action);
			switch(AnimationState) {
				case MapAnimationState.None:
					return;
				case MapAnimationState.Scroll:
					CalcParametersOnScroll();
					break;
				case MapAnimationState.Zoom:
					CalcParametersOnZoom();
					updateScale = true;
					break;
				case MapAnimationState.ZoomScroll:
					updateScale = true;
					CalcParametersOnZoomScroll();
					break;
				case MapAnimationState.ZoomOnScrollComplete:
					updateScale = true;
					CalcParametersOnZoomOnScrollComplete();
					break;
				case MapAnimationState.ScrollOnZoomComplete:
					CalcParametersOnScrollOnZoomComplete();
					break;
				case MapAnimationState.ScrollCompleted:
					CalcParametersOnScrollCompleted();
					break;
				case MapAnimationState.ZoomCompleted:
					CalcParametersOnZoomCompleted();
					break;
			}
			if(action == AnimationAction.Complete)
				OnAnimationComplete();
			if(map.RenderController == null)
				return;
			map.UpdateNavigationPanel(new Point((int)(cursorPosition.X), (int)(cursorPosition.Y)), !updateScale);
			map.RenderController.PerformUpdate(UpdateActionType.Render);
		}
		void OnAnimationComplete() {
			map.UpdateViewportRect(true);
		}
		internal void InitializeScrollAnimation() {
			UpdateAnchorCenter(map.CoordinateSystem.CreateNormalizedPoint(animatedCenterPoint));
		}
	}
}
