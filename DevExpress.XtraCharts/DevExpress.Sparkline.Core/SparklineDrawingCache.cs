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

using DevExpress.Compatibility.System.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
namespace DevExpress.Sparkline.Core {
	public class SparklineDrawingCache : IDisposable { 
		readonly Dictionary<Color, SolidBrush> brushes = new Dictionary<Color, SolidBrush>();
		readonly Dictionary<Color, Pen> pens = new Dictionary<Color, Pen>();
		bool disposed;
		Color ToUnknownColor(Color color) {
			if (color.IsKnownColor)
				return Color.FromArgb(color.A, color.R, color.G, color.B);
			else
				return color;
		}
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing) {
					foreach (SolidBrush brush in brushes.Values)
						brush.Dispose();
					brushes.Clear();
					foreach (Pen pen in pens.Values)
						pen.Dispose();
					pens.Clear();
				}
				disposed = true;
			}
		}
		public SolidBrush GetSolidBrush(Color color) {
			color = ToUnknownColor(color);
			if (!brushes.ContainsKey(color))
				brushes.Add(color, new SolidBrush(color));
			return brushes[color];
		}
		public Pen GetPen(Color color) {
			color = ToUnknownColor(color);
			if (!pens.ContainsKey(color))
				pens.Add(color, new Pen(color));
			return pens[color];
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	public class SparklinePaintersCache {
		readonly Dictionary<SparklineViewType, BaseSparklinePainter> painters = new Dictionary<SparklineViewType, BaseSparklinePainter>();
		public BaseSparklinePainter GetPainter(SparklineViewBase view) {
			if (!painters.ContainsKey(view.Type))
				painters.Add(view.Type, view.CreatePainter());
			return painters[view.Type];
		}
	}
}
