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
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusFillPolygonRecord : EmfPlusFillBase {
		readonly PointF[] points;
		public EmfPlusFillPolygonRecord(short flags, byte[] content)
			: base(flags, content) {
			EmfPlusReader contentStream = ContentStream;
			bool isCompressed = (Flags & compressedFlagMask) != 0;
			bool p = (Flags & 0x800) != 0;
			int count = contentStream.ReadInt32();
			points = new PointF[count];
			if (p) {
				points[0] = new PointF(contentStream.ReadEmfPlusInt(), contentStream.ReadEmfPlusInt());
				for (int i = 1; i < count; i++)
					points[i] = new PointF(points[i - 1].X + contentStream.ReadEmfPlusInt(),
						points[i - 1].Y + contentStream.ReadEmfPlusInt());
			}
			else
				for (int i = 0; i < count; i++)
					points[i] = contentStream.ReadPointF(isCompressed);
		}
		protected override void Draw(EmfMetafileGraphics context) {
			base.Draw(context);
			context.FillPolygon(points);
		}
	}
}
