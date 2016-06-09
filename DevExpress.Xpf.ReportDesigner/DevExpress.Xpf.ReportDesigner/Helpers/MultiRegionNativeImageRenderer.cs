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
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Editors;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public interface INativeImageRegionRenderer {
		void Render(System.Drawing.Graphics graphics, Rect invalidateRect, Rect viewPort, double zoomFactor);
	}
	public interface INativeImageRegion {
		INativeImageRegionRenderer Renderer { get; }
		Rect LogicBounds { get; }
		bool Visible { get; }
	}
	public class MultiRegionNativeImageRenderer : INativeImageRenderer {
		readonly List<INativeImageRegion> regions = new List<INativeImageRegion>();
		Point offset;
		double zoomFactor;
		Matrix logicToDisplayTransform;
		INativeImageRendererCallback callback;
		public void AddRegion(INativeImageRegion region) {
			regions.Add(region);
			Invalidate();
		}
		public void RemoveRegion(INativeImageRegion region) {
			regions.Remove(region);
			Invalidate();
		}
		public void Update(Point offset, double zoomFactor) {
			this.offset = offset;
			this.zoomFactor = zoomFactor;
			logicToDisplayTransform = CreateLogicToDisplayTransform(offset, zoomFactor);
			Invalidate();
		}
		void Invalidate() {
			if(this.callback != null)
				this.callback.Invalidate();
		}
		void INativeImageRenderer.RegisterCallback(INativeImageRendererCallback callback) {
			this.callback = callback;
		}
		void INativeImageRenderer.ReleaseCallback() {
			this.callback = null;
		}
		void INativeImageRenderer.Render(System.Drawing.Graphics graphics, Rect invalidateRect, Size renderSize) {
			foreach(var regionInfo in regions) {
				var regionDisplayBounds = Rect.Transform(regionInfo.LogicBounds, logicToDisplayTransform);
				var regionInvalidateRect = invalidateRect;
				regionInvalidateRect.Intersect(regionDisplayBounds);
				if(!regionInfo.Visible || regionInvalidateRect.Width < 1.0 || regionInvalidateRect.Height < 1.0) continue;
				regionInfo.Renderer.Render(graphics, regionInvalidateRect, new Rect(MathHelper.InvertPoint(regionDisplayBounds.Location), renderSize), zoomFactor);
			}
		}
		static Matrix CreateLogicToDisplayOffsetTransform(Point offset) {
			var matrix = Matrix.Identity;
			matrix.Translate(-offset.X, -offset.Y);
			return matrix;
		}
		static Matrix CreateLogicToDisplayTransform(Point offset, double zoomFactor) {
			var scaleMatrix = Matrix.Identity;
			scaleMatrix.Scale(zoomFactor, zoomFactor);
			return Matrix.Multiply(scaleMatrix, CreateLogicToDisplayOffsetTransform(offset));
		}
	}
}
