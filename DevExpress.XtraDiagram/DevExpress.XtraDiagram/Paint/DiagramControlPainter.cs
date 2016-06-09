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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraDiagram.Paint {
	public class DiagramControlPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
		}
		protected override void DrawAdornments(ControlGraphicsInfoArgs info) {
			base.DrawAdornments(info);
			DrawBackground(info);
		}
		protected virtual void DrawBackground(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.BackgroundDrawArgs.Cache = info.Cache;
			viewInfo.BackgroundDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.BackgroundPainter.DrawObject(viewInfo.BackgroundDrawArgs);
			}
			finally {
				viewInfo.BackgroundDrawArgs.Cache = null;
				viewInfo.BackgroundDrawArgs.Clear();
			}
		}
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
			base.DrawBorder(info);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			if(viewInfo.AllowDrawRulers) {
				DrawRulers(info);
			}
			DrawPage(info);
			DrawContentElement(info, args => {
				DrawTextEditSurface(info);
			});
			DrawSnapLines(info);
			DrawSelectionPreviewAdorners(info);
			DrawSelectionPartAdorners(info);
			if(viewInfo.AllowDrawRulers) {
				DrawRulerShadowAdornments(info);
			}
		}
		protected virtual void DrawRulers(ControlGraphicsInfoArgs info) {
			DrawHRulerBackground(info);
			DrawVRulerBackground(info);
			DrawHRuler(info);
			DrawVRuler(info);
		}
		protected virtual void DrawRulerShadowAdornments(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawRulerShadows(info);
		}
		protected virtual void DrawHRulerBackground(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.HRulerDrawArgs.Cache = info.Cache;
			viewInfo.HRulerDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.HRulerBackgroundPainter.DrawObject(viewInfo.HRulerDrawArgs);
			}
			finally {
				viewInfo.HRulerDrawArgs.Cache = null;
				viewInfo.HRulerDrawArgs.Clear();
			}
		}
		protected virtual void DrawVRulerBackground(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.VRulerDrawArgs.Cache = info.Cache;
			viewInfo.VRulerDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.VRulerBackgroundPainter.DrawObject(viewInfo.VRulerDrawArgs);
			}
			finally {
				viewInfo.VRulerDrawArgs.Cache = null;
				viewInfo.VRulerDrawArgs.Clear();
			}
		}
		protected virtual void DrawHRuler(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.HRulerDrawArgs.Cache = info.Cache;
			viewInfo.HRulerDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.HRulerPainter.DrawObject(viewInfo.HRulerDrawArgs);
			}
			finally {
				viewInfo.HRulerDrawArgs.Cache = null;
				viewInfo.HRulerDrawArgs.Clear();
			}
		}
		protected virtual void DrawVRuler(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.VRulerDrawArgs.Cache = info.Cache;
			viewInfo.VRulerDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.VRulerPainter.DrawObject(viewInfo.VRulerDrawArgs);
			}
			finally {
				viewInfo.VRulerDrawArgs.Cache = null;
				viewInfo.VRulerDrawArgs.Clear();
			}
		}
		protected virtual void DrawPage(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			DrawContentElement(info, arg => {
				DrawPageBackground(info);
			});
			DrawScaledElement(info, arg => {
				DrawItems(info);
			});
			DrawContentElement(info, arg => {
				if(!viewInfo.IsTextEditMode) {
					DrawSelectionAdorners(info);
					DrawShapeParameterAdorners(info);
				}
				DrawConnectorGlueAdorners(info);
				DrawConnectorPointDragPreviewAdorner(info);
			});
			DrawScaledElement(info, arg => {
				DrawDragPreviewAdorners(info);
			});
		}
		protected virtual void DrawTextEditSurface(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawTextEditSurface(info);
		}
		protected virtual void DrawSnapLines(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawSnapLines(info);
		}
		protected virtual void DrawSelectionPreviewAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawSelectionPreview(info);
		}
		protected virtual void DrawSelectionPartAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawSelectionParts(info);
		}
		protected void DrawScaledElement(ControlGraphicsInfoArgs info, Action<ControlGraphicsInfoArgs> draw) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			GraphicsScaleState scaleState = info.Cache.SaveScaling(viewInfo.ContentRect);
			info.Graphics.Transform = viewInfo.GetLogicToDisplayTransformMatrix();
			try {
				draw(info);
			}
			finally {
				info.Cache.RestoreScaling(scaleState);
			}
		}
		protected void DrawContentElement(ControlGraphicsInfoArgs info, Action<ControlGraphicsInfoArgs> draw) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			GraphicsClipState clipInfo = info.Cache.ClipInfo.SaveAndSetClip(viewInfo.ContentRect);
			try {
				draw(info);
			}
			finally {
				info.Cache.ClipInfo.RestoreClipRelease(clipInfo);
			}
		}
		protected virtual void DrawPageBackground(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.PageDrawArgs.Cache = info.Cache;
			viewInfo.PageDrawArgs.Initialize(viewInfo);
			try {
				viewInfo.PagePainter.DrawObject(viewInfo.PageDrawArgs);
			}
			finally {
				viewInfo.PageDrawArgs.Cache = null;
				viewInfo.PageDrawArgs.Clear();
			}
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			foreach(DiagramItemInfo itemInfo in viewInfo.VisibleItems) {
				DrawItem(info, itemInfo);
			}
		}
		protected virtual void DrawItem(ControlGraphicsInfoArgs info, DiagramItemInfo itemInfo) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			itemInfo.PaintController.DrawItem(info, itemInfo);
		}
		protected virtual void DrawSelectionAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawSelection(info);
		}
		protected virtual void DrawDragPreviewAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawDragPreview(info);
		}
		protected virtual void DrawConnectorPointDragPreviewAdorner(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawConnectorPointDragPreview(info);
		}
		protected virtual void DrawShapeParameterAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawShapeParameters(info);
		}
		protected virtual void DrawConnectorGlueAdorners(ControlGraphicsInfoArgs info) {
			DiagramControlViewInfo viewInfo = (DiagramControlViewInfo)info.ViewInfo;
			viewInfo.AdornerController.DrawConnectorGlue(info);
		}
	}
}
