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
namespace DevExpress.XtraCharts.Native {
	public abstract class EdgeGeometry {
		public static readonly int MaxCrosswiseStep = Math.Max(RaggedEdgeGeometry.CrosswiseStep, WavedEdgeGeometry.CrosswiseStep);
		protected abstract GraphicsPath CreateGraphicsPath(int startX, int startY, int length, bool vertical);
		public GraphicsPath CreateLeftSideGraphicsPath(Rectangle bounds) {
			return CreateGraphicsPath(bounds.Left, bounds.Top, bounds.Height - 1, true);
		}
		public GraphicsPath CreateRightSideGraphicsPath(Rectangle bounds) {
			return CreateGraphicsPath(bounds.Right - 1, bounds.Top, bounds.Height - 1, true);
		}
		public GraphicsPath CreateTopSideGraphicsPath(Rectangle bounds) {
			return CreateGraphicsPath(bounds.Left, bounds.Top, bounds.Width - 1, false);
		}
		public GraphicsPath CreateBottomSideGraphicsPath(Rectangle bounds) {
			return CreateGraphicsPath(bounds.Left, bounds.Bottom - 1, bounds.Width - 1, false);
		}
	}
	public class RaggedEdgeGeometry : EdgeGeometry {
		const int lengthwiseStep = 10;
		public const int CrosswiseStep = 2;
		List<int> vertices = new List<int>();
		Random random = new Random(0);
		public List<int> Vertices { get { return vertices; } }
		protected override GraphicsPath CreateGraphicsPath(int startX, int startY, int length, bool vertical) {
			GraphicsPath path = new GraphicsPath();
			int x1 = startX;
			int y1 = startY;
			if(length > lengthwiseStep) {
				Update(length);
				int increment = lengthwiseStep;
				foreach(int vertex in vertices) {
					int x2 = startX + (vertical ? vertex : increment);
					int y2 = startY + (vertical ? increment : vertex);
					path.AddLine(x1, y1, x2, y2);
					x1 = x2;
					y1 = y2;
					increment += lengthwiseStep;
					if(increment >= length)
						break;
				}
			}
			path.AddLine(x1, y1, startX + (vertical ? 0 : length), startY + (vertical ? length : 0));
			return path;
		}
		public void Update(int length) {
			int currentLength = lengthwiseStep * vertices.Count;
			int delta = length - currentLength;
			while(delta >= lengthwiseStep) {
				int vertex = (int)Math.Round(random.NextDouble() * CrosswiseStep);
				if(random.NextDouble() > 0.5)
					vertex = -vertex;
				vertices.Add(vertex);
				delta -= lengthwiseStep;
			}
		}
	}
	public class WavedEdgeGeometry : EdgeGeometry {
		const int stepCountByWave = 8;
		const int minStepLength = 8;
		const int minWaveLength = minStepLength * stepCountByWave;
		const double angleStep = Math.PI / stepCountByWave;
		public const int CrosswiseStep = 4;
		protected override GraphicsPath CreateGraphicsPath(int startX, int startY, int length, bool vertical) {
			GraphicsPath path = new GraphicsPath();
			int x1 = startX;
			int y1 = startY;
			int minWaveCount = length / minWaveLength;
			if(minWaveCount > 0) {
				double waveLength = (double)length / minWaveCount;
				double stepLength = waveLength / stepCountByWave;
				double increment = stepLength;
				double angle = angleStep;
				while(increment <= length) {
					int vertex = (int)Math.Round(Math.Sin(angle) * CrosswiseStep);
					int x2, y2;
					if(vertical) {
						x2 = startX + vertex;
						y2 = (int)Math.Round(startY + increment);
					}
					else {
						x2 = (int)Math.Round(startX + increment);
						y2 = startY + vertex;
					}
					path.AddLine(x1, y1, x2, y2);
					x1 = x2;
					y1 = y2;
					increment += stepLength;
					angle += angleStep;
				}
			}
			path.AddLine(x1, y1, startX + (vertical ? 0 : length), startY + (vertical ? length : 0));
			return path;
		}
	}
}
