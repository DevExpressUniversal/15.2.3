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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Animations {
	public abstract class DiagramAnimationBase {
		Action defaultHandler;
		DiagramControl diagram;
		SplineAnimationHelper splineHelper;
		public DiagramAnimationBase(DiagramControl diagram, Action defaultHandler) {
			this.diagram = diagram;
			this.defaultHandler = defaultHandler;
			this.splineHelper = CreateSplineHelper();
			Initialize(this.splineHelper);
		}
		protected SplineAnimationHelper CreateSplineHelper() {
			return new SplineAnimationHelper();
		}
		protected virtual void Initialize(SplineAnimationHelper splineHelper) {
			splineHelper.Init(0, 1.5, 5);
		}
		public void DoDefaultAction() {
			this.defaultHandler();
		}
		public virtual void Do() {
			RunAnimation();
		}
		protected virtual int AnimationLenght { get { return 25; } }
		protected virtual void RunAnimation() {
			XtraAnimator.Current.AddObject(this.diagram, GetId(), 50000, AnimationLenght, CalcFrame);
		}
		protected abstract object GetId();
		protected void CalcFrame(BaseAnimationInfo obj) {
			float value = ((float)obj.CurrentFrame) / obj.FrameCount;
			CalcFrameCore(value, obj);
			if(obj.IsFinalFrame) {
				CalcFinalFrame(obj);
				OnFinished();
				DoDefaultAction();
			}
			this.diagram.Invalidate();
		}
		public  virtual bool AllowMultiple { get { return true; } }
		protected virtual void CalcFinalFrame(BaseAnimationInfo obj) {
		}
		protected DiagramControl Diagram { get { return diagram; } }
		protected SplineAnimationHelper SplineHelper { get { return splineHelper; } }
		public event EventHandler Finished;
		protected void OnFinished() {
			if(Finished != null) Finished(this, EventArgs.Empty);
		}
		protected abstract void CalcFrameCore(float value, BaseAnimationInfo obj);
	}
	public abstract class DiagramOpacityAnimationBase : DiagramAnimationBase {
		double opacityFactor;
		public DiagramOpacityAnimationBase(DiagramControl diagram, Action defaultHandler) : base(diagram, defaultHandler) {
			this.opacityFactor = 1;
		}
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) {
			this.opacityFactor = DiagramColorUtils.CoerceAlpha(255 * (1 - SplineHelper.CalcSpline(value))) / 255.0;
		}
		public virtual Color GetColor(Color color) {
			int alpha = (int)(color.A * this.opacityFactor);
			return Color.FromArgb(alpha, color);
		}
		protected double OpacityFactor { get { return opacityFactor; } }
	}
	public abstract class DiagramBoundsAnimationBase : DiagramAnimationBase {
		Size gripSize;
		Rectangle startBounds;
		Rectangle endBounds;
		Rectangle bounds;
		public DiagramBoundsAnimationBase(DiagramControl diagram, Action defaultHandler, Rectangle startBounds, Rectangle endBounds, Size gripSize) : base(diagram, defaultHandler) {
			this.bounds = Rectangle.Empty;
			this.startBounds = startBounds;
			this.endBounds = endBounds;
			this.gripSize = gripSize;
		}
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) {
			this.bounds = CalcBounds(this.startBounds, this.endBounds, value);
		}
		protected override int AnimationLenght { get { return 20; } }
		protected Rectangle CalcBounds(Rectangle start, Rectangle end, float value) {
			float left = start.X + (end.X - start.X) * value;
			float top = start.Y + (end.Y - start.Y) * value;
			float right = start.Right - (start.Right - end.Right) * value;
			float bottom = start.Bottom - (start.Bottom - end.Bottom) * value;
			return RectangleUtils.FromLTRB(left, top, right, bottom);
		}
		protected Size GripSize { get { return gripSize; } }
		public Rectangle Bounds { get { return bounds; } }
	}
	public class DiagramItemFadeOutAnimation : DiagramOpacityAnimationBase {
		object itemFadeOutId = new object();
		ReadOnlyCollection<DiagramItem> items;
		public DiagramItemFadeOutAnimation(DiagramControl diagram, Action defaultHandler) : base(diagram, defaultHandler) {
			this.items = GetItems();
		}
		protected ReadOnlyCollection<DiagramItem> GetItems() {
			var list = Diagram.Controller.SelectedItems.Select((IDiagramItem item) => (DiagramItem)item).ToList();
			return new ReadOnlyCollection<DiagramItem>(list);
		}
		protected bool IsItemAffected(DiagramItem item) {
			return this.items.Contains(item);
		}
		public Color GetColor(DiagramItem item, Color color) {
			if(!IsItemAffected(item)) return color;
			return GetColor(color);
		}
		protected override object GetId() { return itemFadeOutId; }
	}
	public class DiagramSelectionPreviewFadeOutAnimation : DiagramOpacityAnimationBase {
		object previewFadeOutId = new object();
		public DiagramSelectionPreviewFadeOutAnimation(DiagramControl diagram, Action defaultHandler)
			: base(diagram, defaultHandler) {
		}
		protected override int AnimationLenght { get { return 20; } }
		protected override object GetId() { return previewFadeOutId; }
	}
	public class DiagramItemSelectionBoundsAnimation : DiagramBoundsAnimationBase {
		object boundsAnimation = new object();
		RotationGrip rotationGrip;
		DiagramItemSelection selection;
		public DiagramItemSelectionBoundsAnimation(DiagramControl diagram, Action defaultHandler, Rectangle startBounds, Rectangle endBounds, Size gripSize, RotationGrip rotationGrip) : base(diagram, defaultHandler, startBounds, endBounds, gripSize) {
			this.selection = new DiagramItemSelection();
			this.rotationGrip = rotationGrip;
		}
		protected override object GetId() { return boundsAnimation; }
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) {
			base.CalcFrameCore(value, obj);
			this.selection = DiagramSelectionUtils.CalcSelection(Bounds, GripSize, RotationGrip);
		}
		public RotationGrip RotationGrip { get { return rotationGrip; } }
		public DiagramItemSelection Selection { get { return selection; } }
	}
	public class DiagramConnectorSelectionBoundsAnimation : DiagramBoundsAnimationBase {
		object boundsAnimation = new object();
		Rectangle[] markers = new Rectangle[0];
		public DiagramConnectorSelectionBoundsAnimation(DiagramControl diagram, Action defaultHandler, Rectangle startBounds, Rectangle endBounds, Size gripSize)
			: base(diagram, defaultHandler, startBounds, endBounds, gripSize) {
		}
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) {
			base.CalcFrameCore(value, obj);
			this.markers = CalcMarkers();
		}
		protected override object GetId() { return boundsAnimation; }
		protected virtual Rectangle[] CalcMarkers() {
			Rectangle[] markers = new Rectangle[3];
			markers[0] = GripSize.CreateRect(Bounds.GetTopLeftPt());
			markers[1] = GripSize.CreateRect(Bounds.GetLeftPt());
			markers[2] = GripSize.CreateRect(Bounds.GetRightPt());
			return markers;
		}
		public Rectangle[] Markers { get { return markers; } }
	}
	public abstract class DiagramSizeSnapLineAnimationBase : DiagramOpacityAnimationBase {
		int startLenght;
		int endLenght;
		int lenghtValue;
		public DiagramSizeSnapLineAnimationBase(DiagramControl diagram, Action defaultHandler, int lenght) : base(diagram, defaultHandler) {
			this.startLenght = this.lenghtValue = GetStartLenght(lenght, LenghtFactor);
			this.endLenght = GetEndLenght(lenght, LenghtFactor);
		}
		protected virtual double LenghtFactor { get { return 0.9; } }
		public override bool AllowMultiple { get { return false; } }
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) {
			base.CalcFrameCore(value, obj);
			this.lenghtValue = CalcLenghtValue(OpacityFactor, startLenght, endLenght);
		}
		public int LenghtValue { get { return lenghtValue; } }
		protected bool IsGrowing { get { return endLenght > startLenght; } }
		protected virtual int CalcLenghtValue(double factor, int startLenght, int endLenght) {
			int dx = (int)Math.Abs((endLenght - startLenght) * (1.0 - factor));
			if(IsGrowing) return startLenght + dx;
			return startLenght - dx;
		}
		protected abstract int GetEndLenght(int lenght, double lenghtFactor);
		protected abstract int GetStartLenght(int lenght, double lenghtFactor);
	}
	public abstract class DiagramSizeSnapLineFadeInAnimationBase : DiagramSizeSnapLineAnimationBase {
		object fadeInAnimation = new object();
		public DiagramSizeSnapLineFadeInAnimationBase(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
		public override Color GetColor(Color color) {
			int alpha = (int)(color.A * (1.0 - OpacityFactor));
			return Color.FromArgb(alpha, color);
		}
		protected override int GetStartLenght(int lenght, double lenghtFactor) {
			return lenght - (int)(lenght * (1 - lenghtFactor));
		}
		protected override int GetEndLenght(int lenght, double lenghtFactor) {
			return lenght;
		}
		protected override object GetId() { return fadeInAnimation; }
	}
	public class DiagramHSizeSnapLineFadeInAnimation : DiagramSizeSnapLineFadeInAnimationBase {
		public DiagramHSizeSnapLineFadeInAnimation(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
	}
	public class DiagramVSizeSnapLineFadeInAnimation : DiagramSizeSnapLineFadeInAnimationBase {
		public DiagramVSizeSnapLineFadeInAnimation(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
	}
	public abstract class DiagramSizeSnapLineFadeOutAnimationBase : DiagramSizeSnapLineAnimationBase {
		object fadeOutAnimation = new object();
		public DiagramSizeSnapLineFadeOutAnimationBase(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
		protected override double LenghtFactor { get { return 0.8; } }
		protected override int GetStartLenght(int lenght, double lenghtFactor) {
			return lenght;
		}
		protected override int GetEndLenght(int lenght, double lenghtFactor) {
			return lenght - (int)(lenght * (1 - lenghtFactor));
		}
		protected override object GetId() { return fadeOutAnimation; }
	}
	public class DiagramHSizeSnapLineFadeOutAnimation : DiagramSizeSnapLineFadeOutAnimationBase {
		public DiagramHSizeSnapLineFadeOutAnimation(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
	}
	public class DiagramVSizeSnapLineFadeOutAnimation : DiagramSizeSnapLineFadeOutAnimationBase {
		public DiagramVSizeSnapLineFadeOutAnimation(DiagramControl diagram, Action defaultHandler, int lenght)
			: base(diagram, defaultHandler, lenght) {
		}
	}
	public class NoneDiagramAnimation : DiagramAnimationBase {
		public NoneDiagramAnimation(DiagramControl diagram, Action defaultHandler)
			: base(diagram, defaultHandler) {
		}
		public override void Do() {
			OnFinished();
			DoDefaultAction();
		}
		protected override object GetId() { return null; }
		protected override void CalcFrameCore(float value, BaseAnimationInfo obj) { }
	}
}
