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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region SurfaceChartView
		internal void GenerateSurfaceChartView(SurfaceChartView view) {
			WriteStartElement("surfaceChart", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleBoolAttributeTag("wireframe", view.Wireframe);
				GenerateChartViewSeries(view);
				GenerateBandFormats(view.BandFormats);
				GenerateAxisGroupRef(view);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region Surface3DChartView
		internal void GenerateSurface3DChartView(Surface3DChartView view) {
			WriteStartElement("surface3DChart", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleBoolAttributeTag("wireframe", view.Wireframe);
				GenerateChartViewSeries(view);
				GenerateBandFormats(view.BandFormats);
				GenerateAxisGroupRef(view);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region BandFormats
		protected internal void GenerateBandFormats(BandFormatCollection bandFormats) {
			WriteStartElement("bandFmts", DrawingMLChartNamespace);
			try {
				foreach (BandFormat format in bandFormats)
					GenerateBandFormat(format);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateBandFormat(BandFormat format) {
			WriteStartElement("bandFmt", DrawingMLChartNamespace);
			try {
				GenerateChartSimpleIntAttributeTag("idx", format.BandId);
				GenerateChartShapeProperties(format.ShapeProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
