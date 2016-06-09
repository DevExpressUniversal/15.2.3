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

using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Design {
	public static class DesignSR {
		public const string NoneString = "(none)";
		public const string ShapefileFilter = "Shapefiles (*.shp) |*.shp";
		public const string KmlFileFilter = "KML files (*.kml) |*.kml";
		public const string SvgFileFilter = "SVG files (*.svg) |*.svg";
		public const string CardinalPointDegree = "{CP}{D:6}°";
		public const string CardinalPointDegreeMinute = "{CP}{D}°{M:2}'";
		public const string CardinalPointDegreeMinuteSecond = "{CP}{D}°{M}'{S:4}''";
		public const string DegreeMinuteCardinalPoint = "{D}°{M:2}'{CP}";
		public const string DegreeMinuteSecondCardinalPoint = "{D}°{M}'{S:4}''{CP}";
		public const string DegreeCardinalPoint = "{D:1}°{CP}";
		public const string ValueMeasureUnit = "{F}{MU}";
		public const string PrecisionValueMeasureUnit = "{F:1}{MU}";
		public const string CustomMeasureUnit = "Custom...";
		public const string CustomEllipsoid = "Custom...";
		public const string MetersInUnitTextBoxIsEmpty = "The 'Meters in Unit' textbox is empty.";
		public const string SemimajorAxisTextBoxIsEmpty = "The 'Semimajor axis' textbox is empty.";
		public const string InverceFlatteringTextBoxIsEmpty = "The 'Inverce Flattering' textbox is empty";
		public const string OpenStreetInvalidTileUriTemplate = "http://{0}.tile.INSERT_SERVER_NAME.com/{1}/{2}/{3}.png";
	}
	public static class DesignHelper {
		public static List<Type> IgnoredTypes = new List<Type>() { };
		public static Uri SelectFileUri(Uri source, string fileFilter) {
			using (OpenFileDialog openDialog = new OpenFileDialog()) {
				openDialog.CheckFileExists = true;
				openDialog.Filter =  fileFilter;
				return openDialog.ShowDialog() == DialogResult.OK ? new Uri(openDialog.FileName) : source;
			}
		}
		public static InnerMap FindInnerMapByMapItem(MapItem item) {
			IMapDataAdapter data = ((IOwnedElement)item).Owner as IMapDataAdapter;
			if (data == null)
				return null;
			MapItemsLayerBase layer = data.GetLayer();
			if (layer == null)
				return null;
			return ((IOwnedElement)layer).Owner as InnerMap;
		}
	}
}
