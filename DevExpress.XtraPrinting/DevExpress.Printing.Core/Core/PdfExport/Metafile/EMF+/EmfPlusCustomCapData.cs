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
	public class EmfPlusCustomCapData {
		public CustomLineCapDataType Type { get; set; }
		public EmfPlusCustomLineCapArrowData ArrowData { get; set; }
		public EmfPlusCustomLineCapData CustomData { get; set; }
		public EmfPlusCustomCapData(MetaReader reader) {
			uint customCapSize = reader.ReadUInt32();
			var pos = reader.BaseStream.Position;
			new EmfPlusGraphicsVersion(reader);
			Type = (CustomLineCapDataType)reader.ReadInt32();
			if(Type == CustomLineCapDataType.CustomLineCapDataTypeAdjustableArrow)
				ArrowData = new EmfPlusCustomLineCapArrowData(reader);
			else if(Type == CustomLineCapDataType.CustomLineCapDataTypeDefault)
				CustomData = new EmfPlusCustomLineCapData(reader);
			System.Diagnostics.Debug.Assert((customCapSize - (reader.BaseStream.Position - pos)) == 0);
		}
	}
	public enum CustomLineCapDataType {
		CustomLineCapDataTypeDefault = 0x00000000,
		CustomLineCapDataTypeAdjustableArrow = 0x00000001
	}
	public class EmfPlusCustomLineCapArrowData {
		public float Width { get; set; }
		public float Height { get; set; }
		public float MiddleInset { get; set; }
		public bool FillState { get; set; }
		public LineCap LineStartCap { get; set; }
		public LineCap LineEndCap { get; set; }
		public LineJoin LineJoin { get; set; }
		public float LineMiterLimit { get; set; }
		public float WidthScale { get; set; }
		public PointF FillHotSpot { get; set; }
		public PointF LineHotSpot { get; set; }
		public EmfPlusCustomLineCapArrowData(MetaReader reader) {
			Width = reader.ReadSingle();
			Height = reader.ReadSingle();
			MiddleInset = reader.ReadSingle();
			FillState = reader.ReadInt32() != 0;
			LineStartCap = (LineCap)reader.ReadInt32();
			LineEndCap = (LineCap)reader.ReadInt32();
			LineJoin = (LineJoin)reader.ReadInt32();
			LineMiterLimit = reader.ReadSingle();
			WidthScale = reader.ReadSingle();
			FillHotSpot = reader.ReadPointF();
			LineHotSpot = reader.ReadPointF();
		}
	}
	public class EmfPlusCustomLineCapData {
		LineCap BaseCap { get; set; }
		float BaseInset { get; set; }
		LineCap StrokeStartCap { get; set; }
		LineCap StrokeEndCap { get; set; }
		LineJoin StrokeJoin { get; set; }
		float StrokeMiterLimit { get; set; }
		float WidthScale { get; set; }
		PointF FillHotSpot { get; set; }
		PointF StrokeHotSpot { get; set; }
		EmfPlusPath FillPath { get; set; }
		EmfPlusPath LinePath { get; set; }
		public EmfPlusCustomLineCapData(MetaReader reader) {
			CustomLineCapDataFlags flags = (CustomLineCapDataFlags)reader.ReadInt32();
			BaseCap = (LineCap)reader.ReadInt32();
			BaseInset = reader.ReadSingle();
			StrokeStartCap = (LineCap)reader.ReadInt32();
			StrokeEndCap = (LineCap)reader.ReadInt32();
			StrokeJoin = (LineJoin)reader.ReadInt32();
			StrokeMiterLimit = reader.ReadSingle();
			WidthScale = reader.ReadSingle();
			PointF FillHotSpot = reader.ReadPointF();
			PointF LineHotSpot = reader.ReadPointF();
			if(flags.HasFlag(CustomLineCapDataFlags.FillPath)) {
				reader.ReadInt32(); 
				FillPath = new EmfPlusPath(reader);
			}
			if(flags.HasFlag(CustomLineCapDataFlags.LinePath)) {
				reader.ReadInt32(); 
				LinePath = new EmfPlusPath(reader);
			}
		}
	}
	[Flags]
	public enum CustomLineCapDataFlags {
		FillPath = 1,
		LinePath = 2
	}
}
