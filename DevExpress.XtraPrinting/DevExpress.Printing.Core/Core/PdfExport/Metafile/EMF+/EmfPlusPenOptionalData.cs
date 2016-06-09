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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class EmfPlusPenOptionalData {
		public EmfPlusPenOptionalData(MetaReader reader, PenDataFlags penDataFlags, Pen pen) {
			if(penDataFlags.HasFlag(PenDataFlags.PenDataTransform)) {
				pen.Transform = reader.ReadMatrix();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataStartCap)) {
				pen.StartCap = (LineCap)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataEndCap)) {
				pen.EndCap = (LineCap)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataJoin)) {
				pen.LineJoin = (LineJoin)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataMiterLimit)) {
				pen.MiterLimit = reader.ReadSingle();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataLineStyle)) {
				pen.DashStyle = (DashStyle)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataDashedLineCap)) {
				pen.DashCap = (DashCap)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataDashedLineOffset)) {
				pen.DashOffset = reader.ReadSingle();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataDashedLine)) {
				pen.DashPattern = new EmfPlusDashedLineData(reader).DashPattern;
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataNonCenter)) {
				pen.Alignment = (PenAlignment)reader.ReadInt32();
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataCompoundLine)) {
				pen.CompoundArray = new EmfPlusCompoundLineData(reader).CompoundLineData;
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataCustomStartCap)) {
				EmfPlusCustomCapData capData = new EmfPlusCustomCapData(reader);
				if(capData.ArrowData != null) {
					pen.CustomStartCap = CreateCustomArrowCap(capData.ArrowData);
				} else
					pen.StartCap = LineCap.Flat;
			}
			if(penDataFlags.HasFlag(PenDataFlags.PenDataCustomEndCap)) {
				EmfPlusCustomCapData capData = new EmfPlusCustomCapData(reader);
				if(capData.ArrowData != null) {
					pen.CustomEndCap = CreateCustomArrowCap(capData.ArrowData);
				} else
					pen.EndCap = LineCap.Flat;
			}
		}
		AdjustableArrowCap CreateCustomArrowCap(EmfPlusCustomLineCapArrowData capData) {
			AdjustableArrowCap cap = new AdjustableArrowCap(capData.Width, capData.Height);
			cap.WidthScale = capData.WidthScale;
			cap.Filled = capData.FillState;
			return cap;
		}
	}
}
