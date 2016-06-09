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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Threading;
using System.Globalization;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Model {
	public class VmlShapeType {
		#region Constants
		public const string DefaultID = "_x0000_t202";
		#endregion
		#region Fields
		string id;
		int coordsizeX;
		int coordsizeY;
		float spt; 
		string path; 
		VmlLineStrokeSettings stroke; 
		VmlShapePath shapePath;
		VmlSingleFormulasCollection formulas; 
		VmlShapeProtections shapeProtections; 
		#endregion
		public VmlShapeType() {
			id = DefaultID;
			coordsizeX = 21600;
			coordsizeY = 21600;
			spt = 202;
			path = "m,l,21600r21600,l21600,xe";
			stroke = new VmlLineStrokeSettings();
			stroke.Joinstyle = VmlStrokeJoinStyle.Miter;
			shapePath = new VmlShapePath();
			shapePath.Gradientshapeok = true;
			shapePath.Connecttype = VmlConnectType.Rect;
		}
		#region Properties
		public string Id { get { return id; } set { id = value; } }
		public int CoordsizeX { get { return coordsizeX; } set { coordsizeX = value; } }
		public int CoordsizeY { get { return coordsizeY; } set { coordsizeY = value; } }
		public float Spt { get { return spt; } set { spt = value; } }
		public string Path { get { return path; } set { path = value; } }
		public VmlLineStrokeSettings Stroke { get { return stroke; } set { stroke = value; } }
		public VmlShapePath ShapePath { get { return shapePath; } set { shapePath = value; } }
		public VmlSingleFormulasCollection Formulas { get { return formulas; } set { formulas = value; } }
		public VmlShapeProtections ShapeProtections { get { return shapeProtections; } set { shapeProtections = value; } }
		#endregion
	}
	public class VmlShapeProtections {
		#region Fields
		bool? adjusthandles;
		bool? aspectratio;
		bool? cropping;
		VmlExtensionHandlingBehavior ext = VmlExtensionHandlingBehavior.View;
		bool? grouping;
		bool? position;
		bool? rotation;
		bool? selection;
		bool? shapetype;
		bool? text;
		bool? ungrouping;
		bool? verticies;
		#endregion
		#region Properties
		public bool? Adjusthandles { get { return adjusthandles; } set { adjusthandles = value; } }
		public bool? Aspectratio { get { return aspectratio; } set { aspectratio = value; } }
		public bool? Cropping { get { return cropping; } set { cropping = value; } }
		public VmlExtensionHandlingBehavior Ext { get { return ext; } set { ext = value; } }
		public bool? Grouping { get { return grouping; } set { grouping = value; } }
		public bool? Position { get { return position; } set { position = value; } }
		public bool? Rotation { get { return rotation; } set { rotation = value; } }
		public bool? Selection { get { return selection; } set { selection = value; } }
		public bool? Shapetype { get { return shapetype; } set { shapetype = value; } }
		public bool? Text { get { return text; } set { text = value; } }
		public bool? Ungrouping { get { return ungrouping; } set { ungrouping = value; } }
		public bool? Verticies { get { return verticies; } set { verticies = value; } }
		#endregion
	}
	public class VmlSingleFormulasCollection : SimpleCollection<VmlSingleFormula> {
	}
	public class VmlSingleFormula {
		string equation;
		public string Equation { get { return equation; } set { equation = value; } }
	}
	public class VmlLineStrokeSettings {
		VmlStrokeJoinStyle joinstyle; 
		string title; 
		VmlFillType filltype; 
		public VmlLineStrokeSettings() {
			joinstyle = VmlStrokeJoinStyle.Round;
			filltype = VmlFillType.Solid;
		}
		#region Properties
		public VmlStrokeJoinStyle Joinstyle { get { return joinstyle; } set { joinstyle = value; } }
		public VmlFillType Filltype { get { return filltype; } set { filltype = value; } }
		public string Title { get { return title; } set { title = value; } }
		#endregion
		public void CopyFrom(VmlLineStrokeSettings source) {
			Joinstyle = source.Joinstyle;
			Title = source.Title;
			Filltype = source.Filltype;
		}
	}
	#region VmlStrokeFillType
	public enum VmlFillType {
		Solid,
		Gradient,
		GradientRadial,
		Tile,
		Pattern,
		Frame
	}
	#endregion
	#region VmlStrokeJoinStyle
	public enum VmlStrokeJoinStyle {
		Round,
		Bevel,
		Miter
	}
	#endregion
	#region ShapePath
	public class VmlShapePath {
		VmlConnectType connecttype;
		bool gradientshapeok;
		public VmlShapePath() {
			connecttype = VmlConnectType.None;
			gradientshapeok = false;
		}
		#region Properties
		public VmlConnectType Connecttype { get { return connecttype; } set { connecttype = value; } }
		public bool Gradientshapeok { get { return gradientshapeok; } set { gradientshapeok = value; } }
		#endregion
		public void CopyFrom(VmlShapePath source) {
			Connecttype = source.Connecttype;
			Gradientshapeok = source.Gradientshapeok;
		}
	}
	#endregion
	#region VmlConnectType
	public enum VmlConnectType {
		None,
		Rect,
		Segments,
		Custom
	}
	#endregion
}
