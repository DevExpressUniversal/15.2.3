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
	public class EmfPlusPath {
		readonly PointF[] points;
		readonly byte[] types;
		public PointF[] Points { get { return points; } }
		public byte[] Types { get { return types; } }
		public EmfPlusPath(EmfPlusReader reader) {
			reader.ReadInt32();
			int count = reader.ReadInt32();
			points = new PointF[count];
			types = new byte[count];
			short flags = reader.ReadInt16();
			reader.ReadInt16();
			bool c = (flags & 0x4000) != 0;
			bool r = (flags & 0x1000) != 0;
			bool p = (flags & 0x0800) != 0;
			if (p) {
				points[0] = new PointF(reader.ReadEmfPlusInt(), reader.ReadEmfPlusInt());
				for (int i = 0; i < count; i++)
					points[i] = new PointF(points[i - 1].X + reader.ReadEmfPlusInt(), points[i - 1].Y + reader.ReadEmfPlusInt());
			}
			else
				for (int i = 0; i < count; i++)
					points[i] = reader.ReadPointF(c);
			for (int i = 0; i < count; )
				types[i++] = reader.ReadByte();
		}
	}
}
