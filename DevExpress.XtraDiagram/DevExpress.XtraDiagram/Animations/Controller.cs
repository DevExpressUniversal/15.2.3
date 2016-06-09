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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Animations {
	public class DiagramAnimationController : IDisposable {
		DiagramControl diagram;
		Dictionary<Type, DiagramAnimationBase> animations;
		public DiagramAnimationController(DiagramControl diagram) {
			this.diagram = diagram;
			this.animations = new Dictionary<Type, DiagramAnimationBase>();
			this.diagram.AnimationFinished += OnDiagramAnimationFinished;
		}
		public void ExecuteCommand(DiagramCommandBase command, Action defaultHandler, Func<bool> canExecute) {
			DiagramAnimationBase diagramAnimation = GetDiagramCommandAnimation(command, defaultHandler, canExecute);
			DoItemIfRequired(diagramAnimation);
		}
		public void CreateAdorner(DiagramAdornerBase adorner, Action defaultHandler) {
			DiagramAnimationBase diagramAnimation = GetCreateAdornerAnimation(adorner, defaultHandler);
			DoItemIfRequired(diagramAnimation);
		}
		public void DestroyAdorner(DiagramAdornerBase adorner, Action defaultHandler) {
			DiagramAnimationBase diagramAnimation = GetDestroyAdornerAnimation(adorner, defaultHandler);
			DoItemIfRequired(diagramAnimation);
		}
		public void UpdateAdornerBounds(DiagramAdornerBase adorner, Action defaultHandler, Rectangle startBounds, Rectangle endBounds) {
			DiagramAnimationBase diagramAnimation = GetAdornerBoundsAnimation(adorner, defaultHandler, startBounds, endBounds);
			DoItemIfRequired(diagramAnimation);
		}
		protected virtual DiagramAnimationBase GetDiagramCommandAnimation(DiagramCommandBase command, Action defaultHandler, Func<bool> canExecute) {
			if(canExecute()) {
				if(command.IsDeleteCommand()) return new DiagramItemFadeOutAnimation(Diagram, defaultHandler);
			}
			return new NoneDiagramAnimation(Diagram, defaultHandler);
		}
		protected virtual DiagramAnimationBase GetCreateAdornerAnimation(DiagramAdornerBase adorner, Action defaultHandler) {
			return new NoneDiagramAnimation(Diagram, defaultHandler);
		}
		protected virtual DiagramAnimationBase GetDestroyAdornerAnimation(DiagramAdornerBase adorner, Action defaultHandler) {
			if(adorner.IsSelectionPreview()) {
				return new DiagramSelectionPreviewFadeOutAnimation(Diagram, defaultHandler);
			}
			return new NoneDiagramAnimation(Diagram, defaultHandler);
		}
		protected virtual DiagramAnimationBase GetAdornerBoundsAnimation(DiagramAdornerBase adorner, Action defaultHandler, Rectangle startBounds, Rectangle endBounds) {
			if(adorner.IsItemSelection()) {
				return new DiagramItemSelectionBoundsAnimation(Diagram, defaultHandler, startBounds, endBounds, GetSizeGripSize(), RotationGrip.Empty);
			}
			if(adorner.IsConnectorSelection()) {
				return new DiagramConnectorSelectionBoundsAnimation(Diagram, defaultHandler, startBounds, endBounds, GetSizeGripSize());
			}
			return new NoneDiagramAnimation(Diagram, defaultHandler);
		}
		protected Size GetSizeGripSize() {
			return Diagram.DiagramViewInfo.SelectionSizeGripSize;
		}
		protected virtual void DoItemIfRequired(DiagramAnimationBase diagramAnimation) {
			if(IsAnimationActive(diagramAnimation) && !diagramAnimation.AllowMultiple) return;
			OnStartAnimation(diagramAnimation);
			diagramAnimation.Finished += (sender, e) => OnFinishAnimation(diagramAnimation);
			try {
				diagramAnimation.Do();
			}
			finally {
			}
		}
		public bool InAnimation { get { return this.animations.Count > 0; } }
		protected virtual void OnDiagramAnimationFinished(object sender, EventArgs e) {
			this.animations.Clear();
		}
		protected virtual void OnStartAnimation(DiagramAnimationBase diagramAnimation) {
			this.animations[diagramAnimation.GetType()] = diagramAnimation;
		}
		protected virtual void OnFinishAnimation(DiagramAnimationBase diagramAnimation) {
			this.animations.Remove(diagramAnimation.GetType());
		}
		public DiagramItemFadeOutAnimation ItemFadeOutAnimation { get { return GetAnimation<DiagramItemFadeOutAnimation>(); } }
		public DiagramSelectionPreviewFadeOutAnimation SelectionPreviewFadeOutAnimation {
			get { return GetAnimation<DiagramSelectionPreviewFadeOutAnimation>(); }
		}
		public DiagramItemSelectionBoundsAnimation ItemSelectionBoundsAnimation {
			get { return GetAnimation<DiagramItemSelectionBoundsAnimation>(); }
		}
		public DiagramConnectorSelectionBoundsAnimation ConnectorSelectionBoundsAnimation {
			get { return GetAnimation<DiagramConnectorSelectionBoundsAnimation>(); }
		}
		public DiagramSizeSnapLineAnimationBase GetHSizeSnapLineAnimation() {
			DiagramSizeSnapLineAnimationBase snapLineAnimation = HSizeSnapLineFadeInAnimation;
			if(snapLineAnimation == null) {
				snapLineAnimation = HSizeSnapLineFadeOutAnimation;
			}
			return snapLineAnimation;
		}
		public DiagramSizeSnapLineAnimationBase GetVSizeSnapLineAnimation() {
			DiagramSizeSnapLineAnimationBase snapLineAnimation = VSizeSnapLineFadeInAnimation;
			if(snapLineAnimation == null) {
				snapLineAnimation = VSizeSnapLineFadeOutAnimation;
			}
			return snapLineAnimation;
		}
		protected DiagramHSizeSnapLineFadeInAnimation HSizeSnapLineFadeInAnimation {
			get { return GetAnimation<DiagramHSizeSnapLineFadeInAnimation>(); }
		}
		protected DiagramHSizeSnapLineFadeOutAnimation HSizeSnapLineFadeOutAnimation {
			get { return GetAnimation<DiagramHSizeSnapLineFadeOutAnimation>(); }
		}
		protected DiagramVSizeSnapLineFadeInAnimation VSizeSnapLineFadeInAnimation {
			get { return GetAnimation<DiagramVSizeSnapLineFadeInAnimation>(); }
		}
		protected DiagramVSizeSnapLineFadeOutAnimation VSizeSnapLineFadeOutAnimation {
			get { return GetAnimation<DiagramVSizeSnapLineFadeOutAnimation>(); }
		}
		public bool IsAnimationActive(DiagramAnimationBase diagramAnimation) {
			return IsAnimationActive(diagramAnimation.GetType());
		}
		public bool IsAnimationActive(Type type) { return this.animations.ContainsKey(type); }
		protected TR GetAnimation<TR>() where TR : DiagramAnimationBase {
			return this.animations.GetValueOrDefault(typeof(TR)) as TR;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.animations != null) this.animations.Clear();
			}
			this.diagram = null;
			this.animations = null;
		}
		#endregion
		public DiagramControl Diagram { get { return diagram; } }
	}
}
