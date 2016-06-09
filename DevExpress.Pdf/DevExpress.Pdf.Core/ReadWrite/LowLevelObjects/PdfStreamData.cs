#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using System.IO;
namespace DevExpress.Pdf.Native {
	public class PdfStreamData : PdfData {
		readonly Stream stream;
		readonly long offset;
		byte[] tail = new byte[10];
		int currentTail = -1;
		long streamPosition;
		long savedPosition;
		public override void SavePosition() {
			savedPosition = streamPosition;
		}
		public override void RestorePosition() {
			if (stream.Position != savedPosition) {
				streamPosition = savedPosition;
				stream.Position = streamPosition;
			}
		}
		public override byte[] Read(int position, int length) {
			stream.Position = offset + position;
			byte[] res = new byte[length];
			stream.Read(res, 0, length);
			streamPosition = offset + position + length;
			return res;
		}
		public override byte[] ToArray() {
			return Read(0, (int)DataLength);
		}
		public PdfStreamData(Stream stream, long offset, long length)
			: base(length) {
			this.stream = stream;
			this.offset = offset;
			streamPosition = offset;
		}
		public override int Get(int index) {
			long newPos = offset + index;
			if (streamPosition != newPos) {
				int distance = (int)(streamPosition - newPos);
				if (distance >= 0 && distance < 10) {
					return tail[(10 + currentTail - distance + 1) % 10];
				}
				stream.Position = newPos;
				streamPosition = newPos;
			}
			int res = stream.ReadByte();
			streamPosition++;
			tail[++currentTail] = (byte)res;
			if (currentTail == 9)
				currentTail = -1;
			return res;
		}
	}
}
