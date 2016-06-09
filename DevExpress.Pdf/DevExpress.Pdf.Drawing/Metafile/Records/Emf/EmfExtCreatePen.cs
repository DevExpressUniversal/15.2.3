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
using System.IO;
namespace DevExpress.Pdf.Drawing {
	public class EmfExtCreatePenRecord : EmfRecord {
		readonly int objectIndex;
		readonly EmfLogPenEx pen;
		readonly EmfDeviceIndependentBitmap bitmap;
		public EmfDeviceIndependentBitmap Bitmap { get { return bitmap; } }
		public EmfExtCreatePenRecord(byte[] content) : base(content) {
			BinaryReader contentStream = ContentStream;
			objectIndex = contentStream.ReadInt32();
			int headerOffset = contentStream.ReadInt32() - 8;
			int headerSize = contentStream.ReadInt32();
			int bitmapContentOffset = contentStream.ReadInt32() - 8;
			int bitmapContentSize = contentStream.ReadInt32();
			byte[] header = null;
			byte[] bitmapContent = null;
			if (headerSize != 0) {
				header = new byte[headerSize];
				Array.Copy(content, headerOffset, header, 0, headerSize);
			}
			if (bitmapContentSize != 0) {
				bitmapContent = new byte[bitmapContentSize];
				Array.Copy(content, bitmapContentOffset, bitmapContent, 0, bitmapContentSize);
			}
			pen = new EmfLogPenEx(contentStream);
			if (headerSize != 0 && bitmapContentSize != 0)
				bitmap = new EmfDeviceIndependentBitmap(header, bitmapContent);
		}
		public override void Execute(EmfMetafileGraphics context) {
			context.AddObject(objectIndex, pen);
		}
	}
}
