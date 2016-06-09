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
using System.IO;
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.Data.Export;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsChartFrameType
	public enum XlsChartFrameType {
		Frame = 0x0000,
		FrameWithShadow = 0x0004
	}
	#endregion
	#region IXlsChartFrameContainer
	public interface IXlsChartFrameContainer {
		void Add(XlsChartFrame frame);
	}
	#endregion
	#region XlsChartFrame
	public class XlsChartFrame : IXlsChartBuilder, IXlsChartLineFormat, IXlsChartAreaFormat, IXlsChartGraphicFormat, IXlsChartShapeFormatContainer {
		#region Fields
		XlsChartLineFormat lineFormat = new XlsChartLineFormat();
		XlsChartAreaFormat areaFormat = new XlsChartAreaFormat();
		XlsChartGraphicFormat graphicFormat = new XlsChartGraphicFormat();
		XlsChartShapeFormatCollection shapeFormats = new XlsChartShapeFormatCollection();
		#endregion
		#region Properties
		public IXlsChartFrameContainer Container { get; set; }
		public XlsChartLineFormat LineFormat { get { return lineFormat; } }
		public XlsChartAreaFormat AreaFormat { get { return areaFormat; } }
		public XlsChartGraphicFormat GraphicFormat { get { return graphicFormat; } }
		public XlsChartFrameType FrameType { get; set; }
		public bool AutoSize { get; set; }
		public bool AutoPosition { get; set; }
		public XlsChartShapeFormatCollection ShapeFormats { get { return shapeFormats; } }
		#endregion
		#region IXlsChartBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
			if (Container != null)
				Container.Add(this);
		}
		#endregion
		#region IXlsChartLineFormat Members
		bool IXlsChartLineFormat.Apply {
			get { return lineFormat.Apply; }
			set { lineFormat.Apply = value; }
		}
		bool IXlsChartLineFormat.Auto {
			get { return lineFormat.Auto; }
			set { lineFormat.Auto = value; }
		}
		bool IXlsChartLineFormat.AxisVisible {
			get { return lineFormat.AxisVisible; }
			set { lineFormat.AxisVisible = value; }
		}
		XlsChartLineStyle IXlsChartLineFormat.LineStyle {
			get { return lineFormat.LineStyle; }
			set { lineFormat.LineStyle = value; }
		}
		XlsChartLineThickness IXlsChartLineFormat.Thickness {
			get { return lineFormat.Thickness; }
			set { lineFormat.Thickness = value; }
		}
		bool IXlsChartLineFormat.AutoColor {
			get { return lineFormat.AutoColor; }
			set { lineFormat.AutoColor = value; }
		}
		Color IXlsChartLineFormat.LineColor {
			get { return lineFormat.LineColor; }
			set { lineFormat.LineColor = value; }
		}
		int IXlsChartLineFormat.LineColorIndex {
			get { return lineFormat.LineColorIndex; }
			set { lineFormat.LineColorIndex = value; }
		}
		#endregion
		#region IXlsChartAreaFormat Members
		bool IXlsChartAreaFormat.Apply {
			get { return areaFormat.Apply; }
			set { areaFormat.Apply = value; }
		}
		Color IXlsChartAreaFormat.ForegroundColor {
			get { return areaFormat.ForegroundColor; }
			set { areaFormat.ForegroundColor = value; }
		}
		Color IXlsChartAreaFormat.BackgroundColor {
			get { return areaFormat.BackgroundColor; }
			set { areaFormat.BackgroundColor = value; }
		}
		XlsChartFillType IXlsChartAreaFormat.FillType {
			get { return areaFormat.FillType; }
			set { areaFormat.FillType = value; }
		}
		bool IXlsChartAreaFormat.AutoColor {
			get { return areaFormat.AutoColor; }
			set { areaFormat.AutoColor = value; }
		}
		bool IXlsChartAreaFormat.InvertIfNegative {
			get { return areaFormat.InvertIfNegative; }
			set { areaFormat.InvertIfNegative = value; }
		}
		#endregion
		#region IXlsChartGraphicFormat Members
		bool IXlsChartGraphicFormat.Apply {
			get { return graphicFormat.Apply; }
			set { graphicFormat.Apply = value; }
		}
		OfficeArtProperties IXlsChartGraphicFormat.ArtProperties {
			get { return graphicFormat.ArtProperties; }
			set { graphicFormat.ArtProperties = value; }
		}
		OfficeArtTertiaryProperties IXlsChartGraphicFormat.ArtTertiaryProperties {
			get { return graphicFormat.ArtTertiaryProperties; }
			set { graphicFormat.ArtTertiaryProperties = value; }
		}
		#endregion
		#region IXlsChartShapePropertiesContainer Members
		void IXlsChartShapeFormatContainer.Add(XlsChartShapeFormat properties) {
			if(IsValidSpCheckSum(properties.CheckSum))
				this.shapeFormats.Add(properties);
		}
		#endregion
		public void SetupShapeProperties(ShapeProperties shapeProperties) {
			if (this.shapeFormats.Count > 0)
				this.shapeFormats[0].SetupShapeProperties(shapeProperties);
			else {
				LineFormat.SetupShapeProperties(shapeProperties);
				if(GraphicFormat.Apply && AreaFormat.Apply && !AreaFormat.AutoColor)
					GraphicFormat.SetupShapeProperties(shapeProperties);
				else
					AreaFormat.SetupShapeProperties(shapeProperties);
			}
		}
		bool IsValidSpCheckSum(int checkSum) {
			if (GraphicFormat.Apply)
				return true; 
			MsoCrc32Compute crc32 = new MsoCrc32Compute();
			LineFormat.CalcSpCheckSum(crc32);
			if (GraphicFormat.Apply && AreaFormat.Apply && !AreaFormat.AutoColor)
				GraphicFormat.CalcSpCheckSum(crc32);
			else
				AreaFormat.CalcSpCheckSum(crc32);
			return crc32.CrcValue == checkSum;
		}
	}
	#endregion
}
