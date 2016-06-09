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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public enum EmfPlusRecordType {
		EmfPlusHeader = 0x4001,
		EmfPlusEndOfFile = 0x4002,
		EmfPlusComment = 0x4003,
		EmfPlusGetDC = 0x4004,
		EmfPlusMultiFormatStart = 0x4005,
		EmfPlusMultiFormatSection = 0x4006,
		EmfPlusMultiFormatEnd = 0x4007,
		EmfPlusObject = 0x4008,
		EmfPlusClear = 0x4009,
		EmfPlusFillRects = 0x400A,
		EmfPlusDrawRects = 0x400B,
		EmfPlusFillPolygon = 0x400C,
		EmfPlusDrawLines = 0x400D,
		EmfPlusFillEllipse = 0x400E,
		EmfPlusDrawEllipse = 0x400F,
		EmfPlusFillPie = 0x4010,
		EmfPlusDrawPie = 0x4011,
		EmfPlusDrawArc = 0x4012,
		EmfPlusFillRegion = 0x4013,
		EmfPlusFillPath = 0x4014,
		EmfPlusDrawPath = 0x4015,
		EmfPlusFillClosedCurve = 0x4016,
		EmfPlusDrawClosedCurve = 0x4017,
		EmfPlusDrawCurve = 0x4018,
		EmfPlusDrawBeziers = 0x4019,
		EmfPlusDrawImage = 0x401A,
		EmfPlusDrawImagePoints = 0x401B,
		EmfPlusDrawString = 0x401C,
		EmfPlusSetRenderingOrigin = 0x401D,
		EmfPlusSetAntiAliasMode = 0x401E,
		EmfPlusSetTextRenderingHint = 0x401F,
		EmfPlusSetTextContrast = 0x4020,
		EmfPlusSetInterpolationMode = 0x4021,
		EmfPlusSetPixelOffsetMode = 0x4022,
		EmfPlusSetCompositingMode = 0x4023,
		EmfPlusSetCompositingQuality = 0x4024,
		EmfPlusSave = 0x4025,
		EmfPlusRestore = 0x4026,
		EmfPlusBeginContainer = 0x4027,
		EmfPlusBeginContainerNoParams = 0x4028,
		EmfPlusEndContainer = 0x4029,
		EmfPlusSetWorldTransform = 0x402A,
		EmfPlusResetWorldTransform = 0x402B,
		EmfPlusMultiplyWorldTransform = 0x402C,
		EmfPlusTranslateWorldTransform = 0x402D,
		EmfPlusScaleWorldTransform = 0x402E,
		EmfPlusRotateWorldTransform = 0x402F,
		EmfPlusSetPageTransform = 0x4030,
		EmfPlusResetClip = 0x4031,
		EmfPlusSetClipRect = 0x4032,
		EmfPlusSetClipPath = 0x4033,
		EmfPlusSetClipRegion = 0x4034,
		EmfPlusOffsetClip = 0x4035,
		EmfPlusDrawDriverString = 0x4036,
		EmfPlusStrokeFillPath = 0x4037,
		EmfPlusSerializableObject = 0x4038,
		EmfPlusSetTSGraphics = 0x4039,
		EmfPlusSetTSClip = 0x403A
	}
}
