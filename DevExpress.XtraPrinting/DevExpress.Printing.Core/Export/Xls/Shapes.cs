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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	using DevExpress.XtraExport.OfficeArt;
	using System.Drawing;
	using DevExpress.Compatibility.System.Drawing;
	partial class XlsDataAwareExporter {
		int CountShapes(IXlShapeContainer container) {
			int result = 0;
			if(container != null) {
				foreach(XlShape shape in container.Shapes) {
					if(shape.GeometryPreset == XlGeometryPreset.Line)
						result++;
				}
			}
			return result;
		}
		bool ShouldExportShapes() {
			foreach(XlsTableBasedDocumentSheet sheet in sheets)
				if(ShouldExportShapes(sheet as IXlShapeContainer))
					return true;
			return false;
		}
		bool ShouldExportShapes(IXlShapeContainer container) {
			if(container != null) {
				foreach(XlShape shape in container.Shapes) {
					if(shape.GeometryPreset == XlGeometryPreset.Line)
						return true;
				}
			}
			return false;
		}
		OfficeArtShapeContainer CreateShape(XlShape shape) {
			OfficeArtShapeContainer result = new OfficeArtShapeContainer();
			int flags = 0x0B00;
			if(shape.Frame.FlipVertical)
				flags |= 0x0080;
			if(shape.Frame.FlipHorizontal)
				flags |= 0x0040;
			result.Items.Add(new OfficeArtShapeRecord(0x0014, GetTopmostShapeId(currentSheet.SheetId) + shape.Id, flags));
			result.Items.Add(CreateShapeProperties(shape));
			result.Items.Add(new OfficeArtClientAnchorSheet(shape));
			result.Items.Add(new OfficeArtShapeClientData(shape));
			return result;
		}
		OfficeArtProperties CreateShapeProperties(XlShape shape) {
			OfficeArtProperties result = new OfficeArtProperties();
			result.Properties.Add(new OfficeArtIntProperty(0x017f, 0x00010000)); 
			result.Properties.Add(new OfficeArtIntProperty(0x01bf, 0x00100000)); 
			Color color = shape.Outline.Color.ConvertToRgb(currentDocument.Theme);
			int colorValue = (int)color.R | (int)color.G << 8 | (int)color.B << 16;
			result.Properties.Add(new OfficeArtIntProperty(0x01c0, colorValue)); 
			result.Properties.Add(new OfficeArtIntProperty(0x01cb, (int)(shape.Outline.Width * 12700))); 
			result.Properties.Add(new OfficeArtIntProperty(0x01ce, (int)shape.Outline.Dashing)); 
			result.Properties.Add(new OfficeArtIntProperty(0x01ff, 0x00180018)); 
			result.Properties.Add(new OfficeArtIntProperty(0x0303, 0x00000000)); 
			result.Properties.Add(new OfficeArtStringProperty(0xc380, shape.Name)); 
			result.Properties.Add(new OfficeArtIntProperty(0x03bf, 0x00200000)); 
			return result;
		}
	}
}
namespace DevExpress.XtraExport.OfficeArt {
	using DevExpress.XtraExport.Xls;
	using DevExpress.XtraExport.Implementation;
	#region OfficeArtShapeClientData
	class OfficeArtShapeClientData : OfficeArtClientData {
		XlShape shape;
		public OfficeArtShapeClientData(XlShape shape)
			: base() {
			this.shape = shape;
		}
		#region Properties
		public XlShape Shape { get { return shape; } }
		#endregion
		protected override void WriteObjData(BinaryWriter writer) {
			writer.Write((ushort)0x15);
			writer.Write((ushort)0x12);
			writer.Write((ushort)0x01); 
			writer.Write((ushort)(shape.Id)); 
			writer.Write((ushort)0x6011); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
		}
	}
	#endregion
}
