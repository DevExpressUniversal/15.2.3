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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design.Commands;
using System.Drawing.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraPrinting.Shape.Native;
namespace DevExpress.XtraReports.Design {
	#region XRShapeActionListBase
	public class XRShapeActionListBase : XRComponentDesignerActionList {
		protected XRShape XRShape {
			get { return (XRShape)Component; }
		}
		protected ShapeBase ShapeBase {
			get { return XRShape.Shape; }
		}
		public XRShapeActionListBase(XRControlDesigner designer)
			: base(designer) {
		}
		protected void AddShapePropertyItem(DesignerActionItemCollection actionItems, string name) {
			AddPropertyItem(actionItems, name, name, Component);
		}
	}
	#endregion
	#region XRShapeDesignerActionList1
	public class XRShapeDesignerActionList1 : XRShapeActionListBase {
		public bool Stretch {
			get { return XRShape.Stretch; }
			set { SetPropertyValue("Stretch", value); }
		}
		public XRShapeDesignerActionList1(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddShapePropertyItem(actionItems, "Stretch");
		}
	}
	#endregion
	#region XRShapeDesignerActionList2
	public class XRShapeDesignerActionList2 : XRShapeActionListBase {
		public Color FillColor {
			get { return XRShape.FillColor; }
			set { SetPropertyValue("FillColor", value); }
		}
		public Color ForeColor {
			get { return XRShape.ForeColor; }
			set { SetPropertyValue("ForeColor", value); }
		}
		public int LineWidth {
			get { return XRShape.LineWidth; }
			set { SetPropertyValue("LineWidth", value); }
		}
		public int Angle {
			get { return XRShape.Angle; }
			set { SetPropertyValue("Angle", value); }
		}
		public XRShapeDesignerActionList2(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddShapePropertyItem(actionItems, "ForeColor");
			if(ShapeHelper.SupportsFillColor(ShapeBase))
				AddShapePropertyItem(actionItems, "FillColor");
			AddShapePropertyItem(actionItems, "LineWidth");
			AddShapePropertyItem(actionItems, "Angle");
		}
	}
	#endregion
	#region ShapeBaseActionList
	public class ShapeBaseActionList : XRShapeActionListBase {
		[RefreshProperties(RefreshProperties.All)]
		public ShapeBase Shape {
			get { return ShapeBase; }
			set {
				SetPropertyValue("Shape", value);
				XRSmartTagService smartTagService = (XRSmartTagService)Component.Site.GetService(typeof(XRSmartTagService));
				smartTagService.UpdateForm(designer, true);
			}
		}
		public ShapeBaseActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			AddShapePropertyItem(actionItems, "Shape");
		}
	}
	#endregion
	#region FilletShapeBaseActionList
	public class FilletShapeBaseActionList : ShapeBaseActionList {
		public int Fillet {
			get { return ((FilletShapeBase)ShapeBase).Fillet; }
			set { SetPropertyValue(ShapeBase, "Fillet", value); }
		}
		public FilletShapeBaseActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Fillet", "Fillet", ShapeBase);
		}
	}
	#endregion
	#region ShapeBracketActionList
	public class ShapeBracketActionList : ShapeBaseActionList {
		public int TipLength {
			get { return ((ShapeBracket)ShapeBase).TipLength; }
			set { SetPropertyValue(ShapeBase, "TipLength", value); }
		}
		public ShapeBracketActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "TipLength", "TipLength", ShapeBase);
		}
	}
	#endregion
	#region ShapeBraceActionList
	public class ShapeBraceActionList : ShapeBracketActionList {
		ShapeBrace ShapeBrace {
			get { return (ShapeBrace)ShapeBase; }
		}
		public int Fillet {
			get { return ShapeBrace.Fillet; }
			set { SetPropertyValue(ShapeBase, "Fillet", value); }
		}
		public int TailLength {
			get { return ShapeBrace.TailLength; }
			set { SetPropertyValue(ShapeBase, "TailLength", value); }
		}
		public ShapeBraceActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Fillet", "Fillet", ShapeBase);
			AddPropertyItem(actionItems, "TailLength", "TailLength", ShapeBase);
		}
	}
	#endregion
	#region ShapeTrapeziodActionList
	#endregion
	#region ShapePolygonActionList
	public class ShapePolygonActionList : FilletShapeBaseActionList {
		public int NumberOfSides {
			get { return ((ShapePolygon)ShapeBase).NumberOfSides; }
			set { SetPropertyValue(ShapeBase, "NumberOfSides", value); }
		}
		public ShapePolygonActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "NumberOfSides", "NumberOfSides", ShapeBase);
		}
	}
	#endregion
	#region ShapeStarActionList
	public class ShapeStarActionList : FilletShapeBaseActionList {
		public int StarPointCount {
			get { return ((ShapeStar)ShapeBase).StarPointCount; }
			set { SetPropertyValue(ShapeBase, "StarPointCount", value); }
		}
		public float Concavity {
			get { return ((ShapeStar)ShapeBase).Concavity; }
			set { SetPropertyValue(ShapeBase, "Concavity", value); }
		}
		public ShapeStarActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "StarPointCount", "StarPointCount", ShapeBase);
			AddPropertyItem(actionItems, "Concavity", "Concavity", ShapeBase);
		}
	}
	#endregion
	#region ShapeArrowActionList
	public class ShapeArrowActionList : FilletShapeBaseActionList {
		ShapeArrow ShapeArrow {
			get { return (ShapeArrow)ShapeBase; }
		}
		public int ArrowHeight {
			get { return ShapeArrow.ArrowHeight; }
			set { SetPropertyValue(ShapeBase, "ArrowHeight", value); }
		}
		public int ArrowWidth {
			get { return ShapeArrow.ArrowWidth; }
			set { SetPropertyValue(ShapeBase, "ArrowWidth", value); }
		}
		public ShapeArrowActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "ArrowHeight", "ArrowHeight", ShapeBase);
			AddPropertyItem(actionItems, "ArrowWidth", "ArrowWidth", ShapeBase);
		}
	}
	#endregion
	#region ShapeCrossActionList
	public class ShapeCrossActionList : FilletShapeBaseActionList {
		ShapeCross ShapeCross {
			get { return (ShapeCross)ShapeBase; }
		}
		public int HorizontalLineWidth {
			get { return ShapeCross.HorizontalLineWidth; }
			set { SetPropertyValue(ShapeBase, "HorizontalLineWidth", value); }
		}
		public int VerticalLineWidth {
			get { return ShapeCross.VerticalLineWidth; }
			set { SetPropertyValue(ShapeBase, "VerticalLineWidth", value); }
		}
		public ShapeCrossActionList(XRControlDesigner designer)
			: base(designer) {
		}
		protected override void FillActionItemCollection(DesignerActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "HorizontalLineWidth", "HorizontalLineWidth", ShapeBase);
			AddPropertyItem(actionItems, "VerticalLineWidth", "VerticalLineWidth", ShapeBase);
		}
	}
	#endregion
}
