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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export.XLS;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	XRDesigner("DevExpress.XtraReports.Design.XRShapeDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRShapeDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultProperty("Shape"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRShape.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRShape", "Shape"),
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRToolboxSubcategoryAttribute(1, 1),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRShape.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRShape.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRShape : XRControl, IShapeBaseOwner, IXtraSupportCreateContentPropertyValue {
		#region fields & properties
		static string defaultShape = ShapeHelper.GetInvariantName(DevExpress.XtraPrinting.Localization.PreviewStringId.Shapes_Ellipse);
		ShapeBase shape;
		int lineWidth = 1;
		DashStyle lineStyle = DashStyle.Solid;
		int angle;
		Color fillColor = Color.Transparent;
		bool stretch;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRShapeShape"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.Shape"),
		SRCategory(ReportStringId.CatBehavior),
		Browsable(true),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, -1)]
		public ShapeBase Shape {
			get { return shape; }
			set {
				if(value != null) {
					shape = !IsDeserializing ? ShapeFactory.CloneShape(value) : value;
				}
			}
		}
		bool ShouldSerializeShape() {
			return shape.ShapeName != defaultShape;
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRShapeLineWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.LineWidth"),
		SRCategory(ReportStringId.CatAppearance),
		Browsable(true),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(1),
		XtraSerializableProperty]
		public int LineWidth {
			get { return lineWidth; }
			set { lineWidth = ShapeHelper.ValidateRestrictedValue(value, 0, int.MaxValue, "LineWidth"); }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.LineStyle"),
		TypeConverter(typeof(DevExpress.Utils.Design.DashStyleTypeConverter)),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(DashStyle.Solid),
		XtraSerializableProperty]
		public DashStyle LineStyle {
			get { return lineStyle; }
			set { lineStyle = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRShapeAngle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.Angle"),
		SRCategory(ReportStringId.CatBehavior),
		Browsable(true),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(0),
		XtraSerializableProperty]
		public int Angle {
			get { return angle; }
			set { angle = ShapeHelper.ValidateAngle(value); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRShapeStretch"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.Stretch"),
		SRCategory(ReportStringId.CatBehavior),
		Browsable(true),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DefaultValue(false),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		XtraSerializableProperty]
		public bool Stretch {
			get { return stretch; }
			set { stretch = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRShapeFillColor"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRShape.FillColor"),
		SRCategory(ReportStringId.CatAppearance),
		Browsable(true),
		RefreshProperties(RefreshProperties.All),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty]
		public Color FillColor {
			get { return fillColor; }
			set { fillColor = value; }
		}
		#region hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		#endregion
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRShape() {
			shape = ShapeFactory.Create(this, defaultShape);
		}
		protected override XRControlScripts CreateScripts() {
			return new XRShapeScripts(this);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ShapeBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			ShapeBrick shapeBrick = (ShapeBrick)brick;
			shapeBrick.LineWidth = XRConvert.Convert(LineWidth, Dpi, GraphicsDpi.Document);
			shapeBrick.LineStyle = LineStyle;
			shapeBrick.Angle = Angle;
			shapeBrick.Stretch = Stretch;
			shapeBrick.FillColor = FillColor;
			shapeBrick.Shape = ShapeFactory.CloneShape(Shape);
		}
		protected internal override void GetStateFromBrick(VisualBrick brick) {
			base.GetStateFromBrick(brick);
			ShapeBrick shapeBrick = (ShapeBrick)brick;
			LineWidth = (int)XRConvert.Convert(shapeBrick.LineWidth, GraphicsDpi.Document, Dpi);
			LineStyle = shapeBrick.LineStyle;
			Angle = shapeBrick.Angle;
			Stretch = shapeBrick.Stretch;
			FillColor = shapeBrick.FillColor;
			Shape = shapeBrick.Shape;
		}
		bool ShouldSerializeFillColor() {
			return fillColor != Color.Transparent;
		}
		#region IXtraSupportCreateContentPropertyValue Members
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			if(e.Item.Name == "Shape")
				return Activator.CreateInstance(typeof(ShapeBase).Assembly.GetType("DevExpress.XtraPrinting.Shape.Shape" + (string)e.Item.ChildProperties["ShapeName"].Value));
			return null;
		}
		#endregion
	}
}
