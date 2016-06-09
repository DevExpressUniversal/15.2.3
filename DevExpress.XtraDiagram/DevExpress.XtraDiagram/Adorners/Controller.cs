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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraDiagram.Adorners {
	public class DiagramAdornerController : IDisposable {
		DiagramControl diagram;
		Dictionary<Type, List<DiagramAdornerBase>> adorners;
		DiagramSelectionTracker selectionTracker;
		public DiagramAdornerController(DiagramControl diagram) {
			this.diagram = diagram;
			this.adorners = new Dictionary<Type, List<DiagramAdornerBase>>();
			this.selectionTracker = new DiagramSelectionTracker(diagram);
			this.selectionTracker.Changed += OnSelectionChanged;
			this.diagram.AnimationFinished += OnDiagramAnimationFinished;
			this.diagram.SelectionChanged += OnDiagramSelectionChanged;
		}
		public T Create<T>(Func<T> adornerFactory) where T : DiagramAdornerBase {
			T adorner = adornerFactory();
			SetTranslators(adorner);
			SubscribeEvents(adorner);
			IList list = adorners.GetOrAdd(adorner.GetType(), () => new List<DiagramAdornerBase>());
			list.Add(adorner);
			OnAdornerCreated(adorner);
			return adorner;
		}
		protected virtual void SetTranslators<T>(T adorner) where T : DiagramAdornerBase {
			SetPlatformRectTranslator(adorner);
			SetPlatformPointTranslator(adorner);
			SetLogicalPointTranslator(adorner);
		}
		public void ForEach(Action<DiagramAdornerBase> action) {
			foreach(var pair in adorners) {
				adorners[pair.Key].ForEach(action);
			}
		}
		public void ForEachAdorner(Type key, Action<DiagramAdornerBase> action) {
			IList<DiagramAdornerBase> list = adorners.GetValueOrDefault(key);
			if(list != null) {
				Array.ForEach(list.ToArray(), adorner => action(adorner));
			}
		}
		public void DrawAdorners(Type key, ControlGraphicsInfoArgs info) {
			ForEachAdorner(key, adorner => DrawAdorner(info, adorner));
		}
		public bool IsEditMode {
			get { return ContainsAdorner(typeof(DiagramInplaceEditorAdorner)); }
		}
		public void DrawSelection(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramSelectionAdorner), info);
			DrawAdorners(typeof(DiagramConnectorSelectionAdorner), info);
		}
		public void DrawDragPreview(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramItemDragPreviewAdorner), info);
			DrawAdorners(typeof(DiagramConnectorDragPreviewAdorner), info);
		}
		public void DrawConnectorPointDragPreview(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramConnectorPointDragPreviewAdorner), info);
		}
		public void DrawRulerShadows(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramHRulerShadowAdorner), info);
			DrawAdorners(typeof(DiagramVRulerShadowAdorner), info);
		}
		public void DrawTextEditSurface(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramInplaceEditorAdorner), info);
		}
		public void DrawSnapLines(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramHBoundsSnapLineAdorner), info);
			DrawAdorners(typeof(DiagramVBoundsSnapLineAdorner), info);
			DrawAdorners(typeof(DiagramHSizeSnapLineAdorner), info);
			DrawAdorners(typeof(DiagramVSizeSnapLineAdorner), info);
		}
		public void DrawSelectionPreview(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramSelectionPreviewAdorner), info);
		}
		public void DrawSelectionParts(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramSelectionPartAdorner), info);
			DrawAdorners(typeof(DiagramConnectorSelectionPartAdorner), info);
		}
		public void DrawShapeParameters(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramShapeParametersAdorner), info);
		}
		public void DrawConnectorGlue(ControlGraphicsInfoArgs info) {
			DrawAdorners(typeof(DiagramGlueToItemAdorner), info);
			DrawAdorners(typeof(DiagramGlueToPointAdorner), info);
			DrawAdorners(typeof(DiagramConnectionPointsAdorner), info);
		}
		protected void DrawAdorner(ControlGraphicsInfoArgs info, DiagramAdornerBase adorner) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			DiagramAdornerObjectInfoArgsBase drawArgs = adorner.GetDrawArgs();
			GraphicsRotationState rotationState = null;
			if(adorner.IsRotationSupports) {
				rotationState = info.Cache.SaveRotation(adorner.RotationOrigin, adorner.Angle);
			}
			drawArgs.Cache = info.Cache;
			drawArgs.Initialize(viewInfo, viewInfo.DefaultAppearances, adorner);
			try {
				adorner.GetPainter().DrawObject(drawArgs);
			}
			finally {
				drawArgs.Cache = null;
				drawArgs.Clear();
				if(rotationState != null) info.Cache.RestoreRotation(rotationState);
			}
		}
		public IEnumerable<DiagramAdornerBase> GetAdorners(Type key) {
			return adorners.GetValueOrDefault(key);
		}
		public int BucketCount { get { return adorners.Count; } }
		public bool InSelectionSizeGrip(Point pt) {
			return SelectAdorner<DiagramSelectionAdorner>(adorner => adorner.InSizeGrip(pt) && adorner.CanResize) != null;
		}
		public bool InRotationGrip(Point pt) {
			return SelectAdorner<DiagramSelectionAdorner>(adorner => adorner.InRotationGrip(pt) && adorner.CanRotate) != null;
		}
		public bool InShapeParameter(Point pt) {
			return GetShapeParameterAdorner(pt) != null;
		}
		public bool InEditSurface(Point pt) {
			return GetEditSurfaceAdorner(pt) != null;
		}
		public bool InConnectorBeginPoint(Point pt) {
			DiagramConnectorSelectionAdorner adorner = GetConnectorSelectionAdorner(pt);
			return adorner != null && adorner.Rects.InBeginPoint(pt);
		}
		public bool InConnectorEndPoint(Point pt) {
			DiagramConnectorSelectionAdorner adorner = GetConnectorSelectionAdorner(pt);
			return adorner != null && adorner.Rects.InEndPoint(pt);
		}
		public bool InConnectorIntermediatePoint(Point pt) {
			DiagramConnectorSelectionAdorner adorner = GetConnectorSelectionAdorner(pt);
			return adorner != null && adorner.Rects.InIntermediatePoint(pt);
		}
		public void EnsureEditSurfaceDestroyed() {
			ForEachAdorner(typeof(DiagramInplaceEditorAdorner), adorner => ((DiagramInplaceEditorAdorner)adorner).OnDestroy());
		}
		public DiagramConnectorSelectionAdorner GetConnectorSelectionAdorner(Point pt) {
			return SelectAdorner<DiagramConnectorSelectionAdorner>(adorner => adorner.Rects.Contains(pt));
		}
		public DiagramShapeParametersAdorner GetShapeParameterAdorner(Point pt) {
			return SelectAdorner<DiagramShapeParametersAdorner>(adorner => adorner.InParameter(pt));
		}
		public DiagramInplaceEditorAdorner GetEditSurfaceAdorner(Point pt) {
			return SelectAdorner<DiagramInplaceEditorAdorner>(adorner => adorner.LogicalBounds.Contains(pt));
		}
		protected TResult SelectAdorner<TResult>(Func<TResult, bool> findCondition) where TResult : DiagramAdornerBase {
			var list = adorners.GetValueOrDefault(typeof(TResult));
			if(list == null) return null;
			foreach(DiagramAdornerBase adorner in list) {
				if(findCondition((TResult)adorner)) return (TResult)adorner;
			}
			return null;
		}
		public DiagramSizeGripItem GetSizeGripItem(Point pt) {
			return GetSelectionAdorner().Select(adorner => adorner.GetSizeGripItem(pt));
		}
		public DiagramShapeParameterItem GetShapeParameterItem(Point pt) {
			return GetShapeParameterAdorner(pt).Select(adorner => adorner.GetShapeParameterItem(pt));
		}
		public DiagramEditSurfaceItem GetEditSurfaceItem(Point pt) {
			return GetEditSurfaceAdorner(pt).Select(adorner => new DiagramEditSurfaceItem(adorner.Item));
		}
		public DiagramConnectorPointItem GetConnectorBeginPointItem(Point pt) {
			return GetConnectorSelectionAdorner(pt).Select(adorner => adorner.GetConnectorBeginPointItem());
		}
		public DiagramConnectorPointItem GetConnectorEndPointItem(Point pt) {
			return GetConnectorSelectionAdorner(pt).Select(adorner => adorner.GetConnectorEndPointItem());
		}
		public DiagramConnectorPointItem GetConnectorIntermediatePointItem(Point pt) {
			return GetConnectorSelectionAdorner(pt).Select(adorner => adorner.GetConnectorIntermediatePointItem(pt));
		}
		public DiagramSelectionAdorner GetSelectionAdorner() {
			return GetAdorners(typeof(DiagramSelectionAdorner)).Select(adorners => adorners.FirstOrDefault() as DiagramSelectionAdorner);
		}
		public DiagramSelectionPreviewAdorner GetSelectionPreviewAdorner() {
			return GetAdorners(typeof(DiagramSelectionPreviewAdorner)).Select(adorners => adorners.FirstOrDefault() as DiagramSelectionPreviewAdorner);
		}
		public DiagramInplaceEditorAdorner GetInplaceEditorAdorner() {
			return GetAdorners(typeof(DiagramInplaceEditorAdorner)).Select(adorners => adorners.FirstOrDefault() as DiagramInplaceEditorAdorner);
		}
		public DiagramShapeParametersAdorner GetShapeParameterAdorner() {
			return GetAdorners(typeof(DiagramShapeParametersAdorner)).Select(adorners => adorners.FirstOrDefault() as DiagramShapeParametersAdorner);
		}
		public Rectangle GetInplaceEditorAdornerDisplayBounds() {
			DiagramAdornerBase adorner = GetInplaceEditorAdorner();
			return adorner != null ? adorner.DisplayBounds : Rectangle.Empty;
		}
		public Rectangle GetSelectionPreviewAdornerDisplayBounds() {
			DiagramAdornerBase adorner = GetSelectionPreviewAdorner();
			return adorner != null ? adorner.DisplayBounds : Rectangle.Empty;
		}
		public bool HasSelection {
			get { return ContainsAdorner(typeof(DiagramSelectionAdorner)); }
		}
		public bool HasSelectionPreview {
			get { return ContainsAdorner(typeof(DiagramSelectionPreviewAdorner)); }
		}
		public bool ContainsAdorner(Type key) {
			return adorners.ContainsKey(key);
		}
		protected void OnSelectionChanged(object sender, DiagramAdornerControllerAdornerChangedEventArgs e) {
			Rectangle startBounds = e.Prev.AdjustDisplayBounds(e.Next);
			Rectangle endBounds = e.Next.DisplayBounds;
			Diagram.AnimationController.UpdateAdornerBounds(e.Next, () => { }, startBounds, endBounds);
		}
		protected virtual void OnDiagramAnimationFinished(object sender, EventArgs e) {
			DestroyAnimatedAdorners();
		}
		protected virtual void OnDiagramSelectionChanged(object sender, DiagramSelectionChangedEventArgs e) {
			if(!Diagram.ContainsSelection()) SelectionTracker.Reset();
		}
		protected void DestroyAnimatedAdorners() {
			List<DiagramAdornerBase> list = new List<DiagramAdornerBase>();
			ForEach(adorner => {
				if(adorner.IsDestroyed) list.Add(adorner);
			});
			if(list.Count == 0) return;
			for(int i = list.Count - 1; i >= 0; i--) RemoveAdorner(list[i]);
		}
		protected internal DiagramSelectionTracker SelectionTracker { get { return selectionTracker; } }
		#region Events
		public event DiagramStartInplaceEditingEventHandler StartInplaceEditing;
		protected virtual void OnStartInplaceEditing(DiagramStartInplaceEditingEventArgs e) {
			if(StartInplaceEditing != null) StartInplaceEditing(this, e);
		}
		public event DiagramFinishInplaceEditingEventHandler FinishInplaceEditing;
		protected void OnFinishInplaceEditing(DiagramFinishInplaceEditingEventArgs e) {
			if(FinishInplaceEditing != null) FinishInplaceEditing(this, e);
		}
		public event DiagramInplaceEditRectChangedEventHandler InplaceEditRectChanged;
		protected virtual void OnInplaceEditRectChanged(DiagramInplaceEditRectChangedEventArgs e) {
			if(InplaceEditRectChanged != null) InplaceEditRectChanged(this, e);
		}
		#endregion
		protected virtual void SetPlatformRectTranslator<T>(T adorner) where T : DiagramAdornerBase {
			adorner.SetPlatformRectTranslator(Diagram.DiagramViewInfo.PlatformToDiagramRect);
		}
		protected virtual void SetPlatformPointTranslator<T>(T adorner) where T : DiagramAdornerBase {
			adorner.SetPlatformPointTranslator(Diagram.DiagramViewInfo.PlatformToDiagramPoint);
		}
		protected virtual void SetLogicalPointTranslator<T>(T adorner) where T : DiagramAdornerBase {
			adorner.SetLogicalPointTranslator(Diagram.DiagramViewInfo.LogicalToDiagramPoint);
		}
		protected virtual void SubscribeEvents<T>(T adorner) where T : DiagramAdornerBase {
			adorner.Initialized += OnDiagramAdornerInitialized;
			adorner.Changed += OnDiagramAdornerChanged;
			adorner.Destroyed += RequestDestroyDiagramAdorner;
			adorner.BoundsChanged += OnDiagramAdornerBoundsChanged;
		}
		protected virtual void UnsubscribeEvents<T>(T adorner) where T : DiagramAdornerBase {
			adorner.Initialized -= OnDiagramAdornerInitialized;
			adorner.Changed -= OnDiagramAdornerChanged;
			adorner.Destroyed -= RequestDestroyDiagramAdorner;
			adorner.BoundsChanged -= OnDiagramAdornerBoundsChanged;
		}
		protected virtual void OnAdornerCreated(DiagramAdornerBase adorner) {
			if(adorner.AffectsOnInplaceEditing) {
				DiagramItemAdornerBase itemAdorner = (DiagramItemAdornerBase)adorner;
				OnStartInplaceEditing(new DiagramStartInplaceEditingEventArgs(itemAdorner.Item, itemAdorner.DisplayBounds));
			}
		}
		protected virtual void OnDiagramAdornerInitialized(object sender, DiagramAdornerInitializedEventArgs e) {
			SelectionTracker.Track(e.Adorner);
			Diagram.AnimationController.CreateAdorner(e.Adorner, () => { });
		}
		protected virtual void OnAdornerRemoved(DiagramAdornerBase adorner) {
			if(adorner.AffectsOnInplaceEditing) {
				DiagramItemAdornerBase itemAdorner = (DiagramItemAdornerBase)adorner;
				OnFinishInplaceEditing(new DiagramFinishInplaceEditingEventArgs(itemAdorner.Item, itemAdorner.DisplayBounds));
			}
		}
		protected virtual void OnDiagramAdornerBoundsChanged(object sender, DiagramAdornerBoundsChangedEventArgs e) {
			DiagramAdornerBase adorner = e.Adorner;
			if(adorner.AffectsOnInplaceEditing) {
				OnInplaceEditRectChanged(new DiagramInplaceEditRectChangedEventArgs(((DiagramItemAdornerBase)adorner).Item, e.Bounds));
			}
		}
		protected virtual void OnDiagramAdornerChanged(object sender, DiagramAdornerChangedEventArgs e) {
			if(Diagram.AnimationController.InAnimation) return;
			NotifyDiagram(e.Adorner, e.ChangeKind);
		}
		protected virtual void RequestDestroyDiagramAdorner(object sender, DiagramAdornerDestroyedEventArgs e) {
			DiagramAdornerBase adorner = e.Adorner;
			Diagram.AnimationController.DestroyAdorner(adorner, () => DestroyDiagramAdorner(adorner));
		}
		protected virtual void DestroyDiagramAdorner(DiagramAdornerBase adorner) {
			RemoveAdorner(adorner);
			OnAdornerRemoved(adorner);
			if(!Diagram.AnimationController.InAnimation) NotifyDiagram(adorner, DiagramAdornedChangeKind.Destroy);
		}
		protected void RemoveAdorner(DiagramAdornerBase adorner) {
			UnsubscribeEvents(adorner);
			Type key = adorner.GetType();
			IList col = adorners.GetValueOrDefault(key);
			if(col != null) {
				col.Remove(adorner);
				if(col.Count == 0) adorners.Remove(key);
			}
		}
		protected virtual void NotifyDiagram(DiagramAdornerBase source, DiagramAdornedChangeKind changeKind) {
			if(source.AffectsDiagramView)
				Diagram.DoLayout();
			else {
				Diagram.Invalidate();
			}
		}
		#region Dispose
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				ForEach(adorner => UnsubscribeEvents(adorner));
				this.adorners.Clear();
			}
			this.adorners = null;
			this.diagram = null;
		}
		#endregion
		public DiagramControl Diagram { get { return diagram; } }
	}
	public abstract class DiagramItemEditingBaseEventArgs : EventArgs {
		readonly DiagramItem item;
		readonly Rectangle displayBounds;
		public DiagramItemEditingBaseEventArgs(DiagramItem item, Rectangle displayBounds) {
			this.item = item;
			this.displayBounds = displayBounds;
		}
		public DiagramItem Item { get { return item; } }
		public Rectangle DisplayBounds { get { return displayBounds; } }
	}
	public class DiagramStartInplaceEditingEventArgs : DiagramItemEditingBaseEventArgs {
		public DiagramStartInplaceEditingEventArgs(DiagramItem item, Rectangle displayBounds)
			: base(item, displayBounds) {
		}
	}
	public delegate void DiagramStartInplaceEditingEventHandler(object sender, DiagramStartInplaceEditingEventArgs e);
	public class DiagramFinishInplaceEditingEventArgs : DiagramItemEditingBaseEventArgs {
		public DiagramFinishInplaceEditingEventArgs(DiagramItem item, Rectangle displayBounds)
			: base(item, displayBounds) {
		}
	}
	public delegate void DiagramFinishInplaceEditingEventHandler(object sender, DiagramFinishInplaceEditingEventArgs e);
	public class DiagramInplaceEditRectChangedEventArgs : EventArgs {
		readonly DiagramItem item;
		readonly Rectangle displayBounds;
		public DiagramInplaceEditRectChangedEventArgs(DiagramItem item, Rectangle bounds) {
			this.item = item;
			this.displayBounds = bounds;
		}
		public DiagramItem Item { get { return item; } }
		public Rectangle DisplayBounds { get { return displayBounds; } }
	}
	public delegate void DiagramInplaceEditRectChangedEventHandler(object sender, DiagramInplaceEditRectChangedEventArgs e);
	public class DiagramAdornerControllerAdornerChangedEventArgs {
		readonly DiagramAdornerBase prev;
		readonly DiagramAdornerBase next;
		public DiagramAdornerControllerAdornerChangedEventArgs(DiagramAdornerBase prev, DiagramAdornerBase next) {
			this.prev = prev;
			this.next = next;
		}
		public DiagramAdornerBase Prev { get { return prev; } }
		public DiagramAdornerBase Next { get { return next; } }
	}
	public delegate void DiagramAdornerControllerAdornerChangedEventHandler(object sender, DiagramAdornerControllerAdornerChangedEventArgs e);
}
