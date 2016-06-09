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

using System.Drawing;
using DevExpress.XtraCharts.GLGraphics;
namespace DevExpress.XtraCharts.Native {
	public class ConeGraphicsCommand : GraphicsCommand {
		float offset;
		float height;
		float startRadius;
		float finishRadius;
		bool startClosed;
		bool finishClosed;
		Color color;
		float holeRadiusFactor;
		public ConeGraphicsCommand(float startHeight, float finishHeight, float startRadius, float finishRadius, Color color, bool startClosed, bool finishClosed, int holeRadiusPercent) {
			offset = startHeight;
			height = finishHeight - startHeight;
			this.startRadius = startRadius;
			this.finishRadius = finishRadius;
			this.color = color;
			this.startClosed = startClosed;
			this.finishClosed = finishClosed;
			this.holeRadiusFactor = holeRadiusPercent / 100.0f;
		}
		protected override void ExecuteInternal(OpenGLGraphics gr) {
			GL.Color4ub(color.R, color.G, color.B, color.A);
			GL.MatrixMode(GL.MODELVIEW);
			GL.PushMatrix();
			try {
				GL.Rotated(90, 1, 0, 0);
				GL.PushMatrix();
				try {
					GL.Translated(0.0, 0.0, offset);
					GLHelper.PartialCone(height, startRadius, finishRadius, 0.0f, 360.0f);
					if (holeRadiusFactor != 0 || holeRadiusFactor != 1)
						GLHelper.PartialCone(height, startRadius * holeRadiusFactor, finishRadius * holeRadiusFactor, 0.0f, 360.0f);
					if (startClosed && holeRadiusFactor != 1)
						GLHelper.PartialDisk(startRadius * holeRadiusFactor, startRadius, 0.0f, 360.0f);
					if (finishClosed && holeRadiusFactor != 1) {
						GL.PushMatrix();
						try {
							GL.Translated(0.0, 0.0, height);
							GLHelper.PartialDisk(finishRadius * holeRadiusFactor, finishRadius, 0.0f, 360.0f);
						}
						finally {
							GL.PopMatrix();
						}
					}
				}
				finally {
					GL.PopMatrix();
				}
			}
			finally {
				GL.PopMatrix();
			}
		}
	}
}
