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
using System.Drawing.Drawing2D;
namespace DevExpress.Pdf.Drawing {
	public class EmfMetafileHeaderRecord : EmfRecord {
		readonly EmfRectL bounds;
		readonly EmfRectL frame;
		readonly int size;
		readonly int recordsCount;
		readonly short handlesCount;
		readonly int palEntries;
		readonly EmfSizeL device;
		readonly EmfSizeL millimeters;
		public EmfRectL Bounds { get { return bounds; } }
		public EmfRectL Frame { get { return frame; } }
		public int Size { get { return size; } }
		public int RecordsCount { get { return recordsCount; } }
		public short HandlesCount { get { return handlesCount; } }
		public int PalEntries { get { return palEntries; } }
		public EmfSizeL Device { get { return device; } }
		public EmfSizeL Millimeters { get { return millimeters; } }
		public EmfMetafileHeaderRecord(byte[] content)
			: base(content) {
			BinaryReader contentStream = ContentStream;
			bounds = new EmfRectL(contentStream);
			frame = new EmfRectL(contentStream);
			contentStream.ReadInt32();
			contentStream.ReadInt32();
			size = contentStream.ReadInt32();
			recordsCount = contentStream.ReadInt32();
			handlesCount = contentStream.ReadInt16();
			contentStream.ReadBytes(10);
			palEntries = contentStream.ReadInt32();
			device = new EmfSizeL(contentStream);
			millimeters = new EmfSizeL(contentStream);
		}
		public override void Execute(EmfMetafileGraphics context) {
			context.SaveGraphicsState();
			double dpiX = (bounds.Right - bounds.Left) / (frame.Right - (double)frame.Left);
			double dpiY = (bounds.Bottom - bounds.Top) / (frame.Bottom - (double)frame.Top);
			context.TranslateTransform( -frame.Left * dpiX, -frame.Top * dpiY);
		}
	}
}
