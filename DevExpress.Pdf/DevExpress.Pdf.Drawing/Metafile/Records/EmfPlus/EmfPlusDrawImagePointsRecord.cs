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
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusDrawImagePointsRecord : EmfPlusRecord {
		readonly PointF[] points;
		readonly RectangleF srcRect;
		public EmfPlusDrawImagePointsRecord(short flags, byte[] content)
			: base(flags, content) {
			EmfPlusReader contentStream = ContentStream;
			contentStream.ReadInt32();
			contentStream.ReadInt32();
			srcRect = contentStream.ReadRectF(false);
			int count = contentStream.ReadInt32();
			points = new PointF[count];
			bool r = (Flags & 0x0800) != 0;
			bool isCompressed = (Flags & compressedFlagMask) != 0;
			if (r) {
				points[0] = new PointF(contentStream.ReadEmfPlusInt(), contentStream.ReadEmfPlusInt());
				for (int i = 1; i < count; i++)
					points[i] = new PointF(contentStream.ReadEmfPlusInt() - points[i - 1].X, contentStream.ReadEmfPlusInt() - points[i - 1].Y);
			}
			else
				for (int i = 0; i < count; i++)
					points[i] = ContentStream.ReadPointF(isCompressed);
		}
		public override void Execute(EmfMetafileGraphics context) {
			PointF point = new PointF(points[2].X + (points[1].X - points[0].X), points[2].Y);
			PointF[] minMaxPoints = new PointF[2];
			minMaxPoints[0] = point;
			minMaxPoints[1] = point;
			for (int i = 0; i < points.Length; i++) {
				minMaxPoints[0].X = Math.Min(points[i].X, minMaxPoints[0].X);
				minMaxPoints[0].Y = Math.Min(points[i].Y, minMaxPoints[0].Y);
				minMaxPoints[1].X = Math.Max(points[i].X, minMaxPoints[1].X);
				minMaxPoints[1].Y = Math.Max(points[i].Y, minMaxPoints[1].Y);
			}
			context.DrawImage(Flags & emfPlusObjectIdMask, new RectangleF(minMaxPoints[0],
				new SizeF(minMaxPoints[1].X - minMaxPoints[0].X, minMaxPoints[1].Y - minMaxPoints[0].Y)), srcRect);
		}
	}
}
