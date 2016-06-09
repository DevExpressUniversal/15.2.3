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

using System.IO;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public abstract class EmfRecord : PdfDisposableObject  {
		public static EmfRecord Create(EmfRecordType recordType, byte[] content) {
			switch (recordType) {
				case EmfRecordType.EMR_HEADER:
					return new EmfMetafileHeaderRecord(content);
				case EmfRecordType.EMR_RESTOREDC:
					return new EmfRestoreDcRecord(content);
				case EmfRecordType.EMR_SAVEDC:
					return new EmfSaveDcRecord(content);
				case EmfRecordType.EMR_SETICMMODE:
					return new EmfSetIcmModeRecord(content);
				case EmfRecordType.EMR_SETMITERLIMIT:
					return new EmfSetMiterLimitRecord(content);
				case EmfRecordType.EMR_MODIFYWORLDTRANSFORM:
					return new EmfModifyWorldTransformRecord(content);
				case EmfRecordType.EMR_EXTCREATEPEN:
					return new EmfExtCreatePenRecord(content);
				case EmfRecordType.EMR_POLYGON16:
					return new EmfPolygon16Record(content);
				case EmfRecordType.EMR_DELETEOBJECT:
					return new EmfDeletetObjectRecord(content);
				case EmfRecordType.EMR_SELECTOBJECT:
					return new EmfSelectObjectRecord(content);
				case EmfRecordType.EMR_EOF:
					return new EmfEofRecord(content);
				case EmfRecordType.EMR_POLYLINE16:
					return new EmfPolyLine16Record(content);
				case EmfRecordType.EMR_POLYBEZIER16:
					return new EmfPolyBezier16Record(content);
				case EmfRecordType.EMR_COMMENT:
					return new EmfCommentRecord(content);
				default:
					return null;
			}
		}
		readonly EmfPlusReader contentStream;
		protected EmfPlusReader ContentStream { get { return contentStream; } }
		protected EmfRecord(byte[] content) {
			contentStream = new EmfPlusReader(new MemoryStream(content));
		}
		public abstract void Execute(EmfMetafileGraphics context);
		protected override void Dispose(bool disposing) {
			if (disposing)
				contentStream.Dispose();
		}
	}
}
