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

namespace DevExpress.Pdf.Drawing {
	public abstract class EmfPlusRecord : EmfRecord {
		protected const short emfPlusObjectIdMask = 0xFF;
		protected const short compressedFlagMask = 0x4000;
		protected const short combineModeMask = 0xF00;
		public static EmfPlusRecord Create(EmfPlusRecordType type, short flags, byte[] content) {
			switch (type) {
				case EmfPlusRecordType.EmfPlusHeader:
					return new EmfPlusHeaderRecord(flags, content);
				case EmfPlusRecordType.EmfPlusEndOfFile:
					return new EmfPlusEofRecord(flags, content);
				case EmfPlusRecordType.EmfPlusObject:
					return EmfPlusObjectRecordBase.Create(flags, content);
				case EmfPlusRecordType.EmfPlusDrawRects:
					return new EmfPlusDrawRectsRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawBeziers:
					return new EmfPlusDrawBeziersRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawArc:
					return new EmfPlusDrawArcRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawEllipse:
					return new EmfPlusDrawEllipseRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawLines:
					return new EmfPlusDrawLinesRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawPie:
					return new EmfPlusDrawPieRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawPath:
					return new EmfPlusDrawPathRecord(flags, content);
				case EmfPlusRecordType.EmfPlusFillPolygon:
					return new EmfPlusFillPolygonRecord(flags, content);
				case EmfPlusRecordType.EmfPlusFillRects:
					return new EmfPlusFillRectsRecord(flags, content);
				case EmfPlusRecordType.EmfPlusMultiplyWorldTransform:
					return new EmfPlusMultiplyWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusScaleWorldTransform:
					return new EmfPlusScaleWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSetWorldTransform:
					return new EmfPlusSetWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusTranslateWorldTransform:
					return new EmfPlusTranslateWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusResetWorldTransform:
					return new EmfPlusResetWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusRotateWorldTransform:
					return new EmfPlusRotateWorldTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSave:
					return new EmfPlusSaveRecord(flags, content);
				case EmfPlusRecordType.EmfPlusRestore:
					return new EmfPlusRestoreRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSetClipRegion:
					return new EmfPlusSetClipRegionRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSetClipRect:
					return new EmfPlusSetClipRectRecord(flags, content);
				case EmfPlusRecordType.EmfPlusFillEllipse:
					return new EmfPlusFillEllipseRecord(flags, content);
				case EmfPlusRecordType.EmfPlusFillPath:
					return new EmfPlusFillPathRecord(flags, content);
				case EmfPlusRecordType.EmfPlusFillPie:
					return new EmfPlusFillPieRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawImage:
					return new EmfPlusDrawImageRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawImagePoints:
					return new EmfPlusDrawImagePointsRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawString:
					return new EmfPlusDrawStringRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSetClipPath:
					return new EmfPlusSetClipPathRecord(flags, content);
				case EmfPlusRecordType.EmfPlusClear:
					return new EmfPlusClearRecord(flags, content);
				case EmfPlusRecordType.EmfPlusSetPageTransform:
					return new EmfPlusSetPageTransformRecord(flags, content);
				case EmfPlusRecordType.EmfPlusResetClip:
					return new EmfPlusResetClipRecord(flags, content);
				case EmfPlusRecordType.EmfPlusDrawDriverString:
					return new EmfPlusDrawDriverStringRecord(flags, content);
				default:
					return null;
			}
		}
		readonly short flags;
		protected short Flags { get { return flags; } }
		protected EmfPlusRecord(short flags, byte[] content)
			: base(content) {
			this.flags = flags;
		}
	}
}
