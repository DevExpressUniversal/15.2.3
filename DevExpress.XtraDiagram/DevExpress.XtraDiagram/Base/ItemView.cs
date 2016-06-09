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
using System.Drawing.Drawing2D;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraDiagram.Base {
	public class DiagramItemView : IDisposable {
		DiagramGraphicsPathCollection graphicsPaths;
		public DiagramItemView() {
			this.graphicsPaths = new DiagramGraphicsPathCollection();
		}
		public void Draw(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			if(drawArgs.DrawShadow) {
				DrawShadow(cache, drawArgs);
			}
			ForEach(path => path.Draw(cache, drawArgs));
		}
		public bool HitTest(Point pt) {
			return this.graphicsPaths.Exists(path => path.IsVisible(pt));
		}
		public bool OutlineHitTest(Point pt, Pen outlinePen) {
			return this.graphicsPaths.Exists(path => path.IsOutlineVisible(pt, outlinePen));
		}
		public void ForEach(Action<DiagramGraphicsPath> handler) {
			graphicsPaths.ForEach(handler);
		}
		public void ForEachShaded(Action<DiagramGraphicsPath> handler) {
			ForEach(path => {
				if(path.Filled && !path.ContainsText) handler(path);
			});
		}
		public void AddPath(DiagramGraphicsPath path) {
			this.graphicsPaths.Add(path);
		}
		protected virtual void DrawShadow(GraphicsCache cache, DiagramItemDrawArgs drawArgs) {
			if(drawArgs.ItemBounds.Size == Size.Empty) return;
			SmoothingMode smoothingMode = cache.Graphics.SmoothingMode;
			cache.Graphics.SmoothingMode = SmoothingMode.None;
			try {
				Image shadowImage = drawArgs.PaintCache.GetShapeShadowImage(drawArgs, DrawShapeShadowImage);
				cache.Graphics.DrawImageUnscaled(shadowImage, drawArgs.ItemBounds.Location);
			}
			finally {
				cache.Graphics.SmoothingMode = smoothingMode;
			}
		}
		protected virtual void DrawShapeShadowImage(DiagramItemDrawArgs drawArgs, GraphicsCache cache) {
			ForEachShaded(path => path.DrawShapeShadow(drawArgs, cache));
		}
		internal int GetGraphicsPathCount() { return this.graphicsPaths != null ? this.graphicsPaths.Count : 0; }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(this.graphicsPaths != null) {
					this.graphicsPaths.ForEach(path => path.Dispose());
					this.graphicsPaths.Clear();
				}
			}
			this.graphicsPaths = null;
		}
		#endregion
	}
}
