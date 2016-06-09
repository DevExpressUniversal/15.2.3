﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
namespace DevExpress.Office.Drawing {
	#region OfficeSelectionPainter (abstract class)
	public abstract class OfficeSelectionPainter {
		#region Fields
		protected static readonly Color HotZoneGradientColor = Color.FromArgb(255, 202, 234, 237);
		protected static readonly Color HotZoneRotationGradientColor = Color.FromArgb(255, 136, 228, 58);
		protected static readonly Color HotZoneBorderColor = Color.Gray;
		protected static readonly Color ShapeBorderColor = Color.FromArgb(255, 90, 147, 211);
		protected static readonly int ShapeBorderWidth = 1;
		readonly IGraphicsCache cache;
		readonly Stack<Matrix> transformsStack = new Stack<Matrix>();
		#endregion
		protected OfficeSelectionPainter(IGraphicsCache cache) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		#region Properties
		public IGraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return cache.Graphics; } }
		public virtual SmoothingMode FillSmoothingMode { get { return SmoothingMode.HighQuality; } }
		public virtual PixelOffsetMode FillPixelOffsetMode { get { return PixelOffsetMode.HighQuality; } }
		#endregion
		public bool TryPushRotationTransform(Point center, float angleInDegrees) {
			bool needApplyTransform = (angleInDegrees % 360f) != 0;
			if (!needApplyTransform)
				return false;
			PushRotationTransform(center, angleInDegrees);
			return true;
		}
		public void PushRotationTransform(Point center, float angleInDegrees) {
			transformsStack.Push(Graphics.Transform.Clone());
			Matrix transform = Graphics.Transform;
			transform.RotateAt(angleInDegrees, center);
			Graphics.Transform = transform;
		}
		public void PopTransform() {
			Graphics.Transform = transformsStack.Pop();
		}
		protected internal virtual void DrawRectangularHotZone(Rectangle bounds, Color color) {
			FillInOptionalQuality(delegate(Graphics graphics) {
				graphics.FillRectangle(CreateFillBrush(bounds, color), bounds);
				graphics.DrawRectangle(cache.GetPen(HotZoneBorderColor), bounds);
			});
		}
		protected internal virtual void DrawEllipticHotZone(Rectangle bounds, Color color) {
			FillInOptionalQuality(delegate(Graphics graphics) {
				graphics.FillEllipse(CreateFillBrush(bounds, color), bounds);
				graphics.DrawEllipse(cache.GetPen(HotZoneBorderColor), bounds);
			});
		}
		protected internal virtual Brush CreateFillBrush(Rectangle bounds, Color color) {
			LinearGradientBrush brush = new LinearGradientBrush(bounds, Color.White, color, LinearGradientMode.Vertical);
			brush.SetSigmaBellShape(0.5f, 1.0f);
			return brush;
		}
		protected void DrawInHighQuality(Action<Graphics> draw) {
			Graphics graphics = Cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			try {
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				draw(graphics);
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
			}
		}
		protected void FillInOptionalQuality(Action<Graphics> fill) {
			Graphics graphics = Cache.Graphics;
			SmoothingMode oldSmoothingMode = graphics.SmoothingMode;
			PixelOffsetMode oldPixelOffsetMode = graphics.PixelOffsetMode;
			try {
				graphics.SmoothingMode = FillSmoothingMode;
				graphics.PixelOffsetMode = FillPixelOffsetMode;
				fill(graphics);
			}
			finally {
				graphics.SmoothingMode = oldSmoothingMode;
				graphics.PixelOffsetMode = oldPixelOffsetMode;
			}
		}
		protected abstract void FillRectangle(Rectangle bounds);
		protected abstract void DrawRectangle(Rectangle bounds);
		protected abstract void DrawLine(Point from, Point to);
	}
	#endregion
}
