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
using System.Text;
using System.Xml;
using System.IO;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using System.Reflection;
using System.Drawing;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Collections;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.XAML {
	public class TransformInfo {
		private float rotateAngleCore;
		public float RotateAngle {
			get { return rotateAngleCore; }
			set { rotateAngleCore = value; }
		}
		private float skewAngleXCore;
		public float SkewAngleX {
			get { return skewAngleXCore; }
			set { skewAngleXCore = value; }
		}
		private float skewAngleYCore;
		public float SkewAngleY {
			get { return skewAngleYCore; }
			set { skewAngleYCore = value; }
		}
		private float offsetXCore;
		public float OffsetX {
			get { return offsetXCore; }
			set { offsetXCore = value; }
		}
		private float offsetYCore;
		public float OffsetY {
			get { return offsetYCore; }
			set { offsetYCore = value; }
		}
		private float scaleXCore = 1;
		public float ScaleX {
			get { return scaleXCore; }
			set { scaleXCore = value; }
		}
		private float scaleYCore = 1;
		public float ScaleY {
			get { return scaleYCore; }
			set { scaleYCore = value; }
		}
		private float centerXCore;
		public float CenterX {
			get { return centerXCore; }
			set { centerXCore = value; }
		}
		private float centerYCore;
		public float CenterY {
			get { return centerYCore; }
			set { centerYCore = value; }
		}
		private float rcenterXCore;
		public float RotateCenterX {
			get { return rcenterXCore; }
			set { rcenterXCore = value; }
		}
		private float rcenterYCore;
		public float RotateCenterY {
			get { return rcenterYCore; }
			set { rcenterYCore = value; }
		}
		private int skewOrderCore = -1;
		public int SkewOrder {
			get { return skewOrderCore; }
			set { skewOrderCore = value; }
		}
		private int rotateOrderCore = -1;
		public int RotateOrder {
			get { return rotateOrderCore; }
			set { rotateOrderCore = value; }
		}
		private int scaleOrderCore = -1;
		public int ScaleOrder {
			get { return scaleOrderCore; }
			set { scaleOrderCore = value; }
		}
		private int translateOrderCore = -1;
		public int TranslateOrder {
			get { return translateOrderCore; }
			set { translateOrderCore = value; }
		}
	}
	public class XamlLoader : BaseShapeLoader {
		XmlDocument xamlDoc;
		public XamlLoader() : base() { }
		protected override void OnCreate() { }
		protected override void OnDispose() {
			xamlDoc = null;
		}
		public XmlDocument Document {
			get { return xamlDoc; }
		}
		public override ComplexShape LoadFromStream(Stream stream) {
			if (!CreateXMLDocumentTryLoad(stream)) throw new XAMLLoadExceptions(XAMLLoadExceptions.DocumentOpenError);
			return ParseDocument();
		}
		protected virtual ComplexShape ParseDocument() {
			XmlNode canvasNode = GetChildNodeByName(Document, "Canvas", true);
			return ParseCanvas(canvasNode, null);
		}
		protected ComplexShape ParseCanvas(XmlNode canvas, ComplexShape result) {
			if (canvas == null) throw new XAMLLoadExceptions(XAMLLoadExceptions.CanvasIsNull);
			if (result == null) result = new ComplexShape();
			result.BeginUpdate();
			ParseBaseShape(canvas, result);
			foreach (XmlNode node in canvas.ChildNodes) {
				BaseShape shape = (node.Name == "Canvas" || node.Name == "Viewbox") ? ParseCanvas(node, null) : ParseNode(node);
				result.Add(shape);
			}
			result.EndUpdate();
			return result;
		}
		protected void ParseBaseShape(XmlNode node, BaseShape shape) {
			shape.Name = ParseStringAttribute(node, "Name");
			float left = ParseFloatAttribute(node, "Canvas.Left");
			float top = ParseFloatAttribute(node, "Canvas.Top");
			float width = ParseFloatAttribute(node, "Width");
			float height = ParseFloatAttribute(node, "Height");
			shape.Bounds = new RectangleF(left, top, width, height);
			ParseShapeAppearanceOptions(node, shape);
		}
		protected virtual void ParseRenderTransformOrigin(XmlNode node, out float originX, out float originY) {
			originX = 0;
			originY = 0;
			string attrName = "RenderTransformOrigin";
			if (node.Attributes != null && node.Attributes[attrName] != null && node.Attributes[attrName].Value != "Auto") {
				string valueString = node.Attributes[attrName].Value;
				int commaindex = valueString.IndexOf(",");
				string oX = valueString.Substring(0, commaindex);
				string oY = valueString.Substring(commaindex + 1, valueString.Length - commaindex - 1);
				originX = float.Parse(oX, CultureInfo.InvariantCulture);
				originY = float.Parse(oY, CultureInfo.InvariantCulture);
			}
		}
		protected Matrix CreateIdentityMatrix() {
			return new Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0f, 0f);
		}
		float GradToRad(float input) {
			return (float)(input * Math.PI / 180.0f);
		}
		protected Matrix ParseShapeTransformation(XmlNode node, BaseShape shape, TransformInfo ti) {
			float originX = 0, originY = 0;
			ParseRenderTransformOrigin(node, out originX, out originY);
			if (ti != null) {
				if (originX == 0) originX = ti.RotateCenterX;
				if (originY == 0) originY = ti.RotateCenterY;
			}
			Matrix transform = CreateIdentityMatrix();
			if (ti != null) {
				Matrix translation = new Matrix(1.0f, 0.0f, 0.0f, 1.0f,
					ti.OffsetX + shape.Bounds.Left,
					ti.OffsetY + shape.Bounds.Top);
				Matrix skewTranslation = new Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0f, 0f);
				skewTranslation.Shear(GradToRad(ti.SkewAngleX), GradToRad(ti.SkewAngleY));
				Matrix rotation = CreateIdentityMatrix();
				rotation.RotateAt(ti.RotateAngle, new PointF(originX * shape.Bounds.Width, originY * shape.Bounds.Height));
				Matrix scaling = new Matrix(ti.ScaleX, 0.0f, 0.0f, ti.ScaleY, 0.0f, 0.0f);
				for (int i = 0; i < 10; i++) {
					if (!scaling.IsIdentity && i == ti.ScaleOrder) transform.Multiply(scaling, MatrixOrder.Append);
					if (!skewTranslation.IsIdentity && i == ti.SkewOrder) transform.Multiply(skewTranslation, MatrixOrder.Append);
					if (!rotation.IsIdentity && i == ti.RotateOrder) transform.Multiply(rotation, MatrixOrder.Append);
					if (!translation.IsIdentity && i == ti.TranslateOrder) transform.Multiply(translation, MatrixOrder.Append);
				}
				scaling.Dispose();
				rotation.Dispose();
				skewTranslation.Dispose();
				translation.Dispose();
			} else {
				if (!shape.Bounds.IsEmpty) {
					Matrix translation = new Matrix(1.0f, 0.0f, 0.0f, 1.0f, shape.Bounds.Left, shape.Bounds.Top);
					transform.Multiply(translation, MatrixOrder.Append);
					translation.Dispose();
				}
			}
			return transform;
		}
		protected void ProcessGradientBrushTransform(XmlNode node, BaseShape shape, BaseGradientBrushObject gradientBrush) {
			TransformInfo ti = ProcessTransform(node, "RelativeTransform");
			if (ti != null) gradientBrush.Transform = ParseShapeTransformation(node, shape, ti);
		}
		protected virtual TransformInfo ProcessTransform(XmlNode node, string transformName) {
			TransformInfo ti = null;
			XmlNode transformNode = GetChildNodeByName(node, transformName, true);
			if (transformNode != null) {
				transformNode = GetChildNodeByName(transformNode, "TransformGroup", true);
				if (transformNode != null) {
					ti = ParseTransformGroup(transformNode);
				}
			}
			return ti;
		}
		protected int GetNodeOrder(XmlNode tempNode) {
			XmlNode pNode = tempNode.ParentNode;
			for (int i = 0; i < pNode.ChildNodes.Count; i++) {
				if (pNode.ChildNodes.Item(i) == tempNode) return i;
			}
			return -1;
		}
		protected virtual TransformInfo ParseTransformGroup(XmlNode node) {
			TransformInfo ti = new TransformInfo();
			XmlNode tempNode = GetChildNodeByName(node, "SkewTransform", true);
			if (tempNode != null) ParseSkewTransform(tempNode, ti, GetNodeOrder(tempNode));
			tempNode = GetChildNodeByName(node, "RotateTransform", true);
			if (tempNode != null) ParseRotateTransform(tempNode, ti, GetNodeOrder(tempNode));
			tempNode = GetChildNodeByName(node, "ScaleTransform", true);
			if (tempNode != null) ParseScaleTransform(tempNode, ti, GetNodeOrder(tempNode));
			tempNode = GetChildNodeByName(node, "TranslateTransform", true);
			if (tempNode != null) ParseTranslateTransform(tempNode, ti, GetNodeOrder(tempNode));
			return ti;
		}
		protected virtual void ParseCenter(XmlNode node, out float cx, out float cy) {
			cx = ParseFloatAttribute(node, "CenterX");
			cy = ParseFloatAttribute(node, "CenterY");
		}
		protected virtual void ParseSkewTransform(XmlNode node, TransformInfo ti, int order) {
			float angleX = ParseFloatAttribute(node, "AngleX");
			float angleY = ParseFloatAttribute(node, "AngleY");
			float cx, cy;
			ParseCenter(node, out cx, out cy);
			ti.SkewAngleX = angleX;
			ti.SkewAngleY = angleY;
			ti.SkewOrder = order;
		}
		protected virtual void ParseRotateTransform(XmlNode node, TransformInfo ti, int order) {
			float angle = ParseFloatAttribute(node, "Angle");
			float cx, cy;
			ParseCenter(node, out cx, out cy);
			ti.RotateAngle = angle;
			ti.RotateOrder = order;
			ti.RotateCenterX = cx;
			ti.RotateCenterY = cy;
		}
		protected virtual void ParseScaleTransform(XmlNode node, TransformInfo ti, int order) {
			float scaleX = ParseFloatAttribute(node, "ScaleX");
			float scaleY = ParseFloatAttribute(node, "ScaleY");
			ti.ScaleX = scaleX;
			ti.ScaleY = scaleY;
			ti.ScaleOrder = order;
		}
		protected virtual void ParseTranslateTransform(XmlNode node, TransformInfo ti, int order) {
			float x = ParseFloatAttribute(node, "X");
			float y = ParseFloatAttribute(node, "Y");
			ti.OffsetX = x;
			ti.OffsetY = y;
			ti.TranslateOrder = order;
		}
		protected BrushObject ParseSolidBrushInfo(XmlNode node, string attrName) {
			Color color = ParseColorAttribute(node, attrName);
			return color.IsEmpty ? BrushObject.Empty : new SolidBrushObject(color);
		}
		protected BrushObject ParseSolidBrushInfo(XmlNode node, string attrName1, string attrName2) {
			BrushObject result = ParseSolidBrushInfo(node, attrName1);
			return result.IsEmpty ? ParseSolidBrushInfo(node, attrName2) : result;
		}
		protected Color ParseColorAttribute(XmlNode node, string attrName) {
			Color result = Color.Empty;
			if (node.Attributes != null && node.Attributes[attrName] != null)
				result = ARGBColorTranslator.FromHtml(node.Attributes[attrName].Value);
			return result;
		}
		protected void ParseShapeAppearanceSolidOptions(XmlNode node, BaseShape shape) {
			shape.Appearance.ContentBrush = ParseSolidBrushInfo(node, "Background", "Fill");
			shape.Appearance.BorderBrush = ParseSolidBrushInfo(node, "Stroke");
		}
		protected void ParseShapeAppearanceGradientOptions(XmlNode node, BaseShape shape) {
			if (shape.Appearance.ContentBrush.IsEmpty) ProcessGradientBrush(node, shape);
		}
		protected void ProcessGradientBrush(XmlNode node, BaseShape shape) {
			XmlNode fillNode = GetChildNodeByName(node, "Fill", true);
			if (fillNode == null) return;
			BaseGradientBrushObject gradientBrush = null;
			XmlNode gradientBrushNode = null;
			gradientBrushNode = GetChildNodeByName(fillNode, "LinearGradientBrush", true);
			if (gradientBrushNode != null) {
				gradientBrush = ParseLinearGradientBrushObject(gradientBrushNode);
			} else {
				gradientBrushNode = GetChildNodeByName(fillNode, "RadialGradientBrush", true);
				gradientBrush = ParseEllipticalGradientBrushObject(gradientBrushNode);
			}
			if (gradientBrush != null) {
				XmlNode gradientStopsNode = GetChildNodeByName(gradientBrushNode, "GradientStops", true);
				XmlNode gsParent = (gradientStopsNode == null) ? gradientBrushNode : gradientStopsNode;
				foreach (XmlNode xmlNode in gsParent.ChildNodes) {
					GradientStop gs = ParseGradientStop(xmlNode);
					if (gs != null) gradientBrush.GradientStops.Add(gs);
				}
			}
			ProcessGradientBrushTransform(gradientBrushNode, shape, gradientBrush);
			shape.Appearance.ContentBrush = gradientBrush;
		}
		protected LinearGradientBrushObject ParseLinearGradientBrushObject(XmlNode node) {
			LinearGradientBrushObject brush = new LinearGradientBrushObject();
			if (node.Attributes["StartPoint"] != null) brush.StartPoint = ParsePointF(node.Attributes["StartPoint"].Value);
			if(node.Attributes["EndPoint"] != null) brush.EndPoint = ParsePointF(node.Attributes["EndPoint"].Value);
			return brush;
		}
		protected EllipticalGradientBrushObject ParseEllipticalGradientBrushObject(XmlNode node) {
			EllipticalGradientBrushObject brush = new EllipticalGradientBrushObject();
			if (node.Attributes["Center"] != null) brush.Center = ParsePointF(node.Attributes["Center"].Value);
			brush.RadiusX = ParseFloatAttribute(node, "RadiusX");
			brush.RadiusY = ParseFloatAttribute(node, "RadiusY");
			return brush;
		}
		protected GradientStop ParseGradientStop(XmlNode node) {
			GradientStop result = null;
			if (node.Name.EndsWith("GradientStop")) {
				Color color = ParseColorAttribute(node, "Color");
				float offset = ParseFloatAttribute(node, "Offset");
				result = new GradientStop(color, offset);
			}
			return result;
		}
		protected PointF2D ParsePointF(string value) {
			char ch = CultureInfo.InvariantCulture.TextInfo.ListSeparator[0x0];
			string[] strArray = value.Split(new char[] { ch });
			float[] numArray = new float[strArray.Length];
			for (int i = 0; i < numArray.Length; i++) {
				numArray[i] = float.Parse(strArray[i], CultureInfo.InvariantCulture);
			}
			if (numArray.Length != 2) {
				throw new XAMLLoadExceptions(XAMLLoadExceptions.TextParseError); ;
			}
			return new PointF2D(numArray[0], numArray[1]);
		}
		protected XmlNode GetChildNodeByName(XmlNode node, string name, bool endsWith) {
			XmlNode result = null;
			foreach (XmlNode tNode in node.ChildNodes) {
				if (endsWith && tNode.Name.EndsWith(name)) {
					result = tNode;
					break;
				}
			}
			return result;
		}
		protected void ParseShapeAppearanceOptions(XmlNode node, BaseShape shape) {
			ParseShapeAppearanceSolidOptions(node, shape);
			ParseShapeAppearanceGradientOptions(node, shape);
			shape.Appearance.BorderWidth = ParseFloatAttribute(node, "StrokeThickness");
		}
		protected float ParseFloatAttribute(XmlNode node, string attrName) {
			float result = 0f;
			if (node.Attributes != null && node.Attributes[attrName] != null && node.Attributes[attrName].Value != "Auto")
				result = float.Parse(node.Attributes[attrName].Value, CultureInfo.InvariantCulture);
			return result;
		}
		protected string ParseStringAttribute(XmlNode node, string attrName) {
			string result = string.Empty;
			if (node.Attributes != null && node.Attributes[attrName] != null)
				result = node.Attributes[attrName].InnerText;
			return result;
		}
		protected BaseShape ParseNode(XmlNode node) {
			BaseShape result = CreateShapeByName(node);
			if (result != null) {
				result.BeginUpdate();
				ParseBaseShape(node, result);
				ParseShapeByType(node, result);
				TransformInfo ti = ProcessTransform(node, "RenderTransform");
				result.Transform = ParseShapeTransformation(node, result, ti);
				result.EndUpdate();
			}
			return result;
		}
		protected BaseShape CreateShapeByName(XmlNode shapeNode) {
			switch (shapeNode.Name) {
				case "Path": return new PathShape();
				case "Rectangle": return new BoxShape();
				case "Ellipse": return new EllipseShape();
				default: return new ComplexShape();
			}
		}
		protected void NormalizePoints(BaseShape shape, ShapePoint[] points) {
			shape.ForceShapeChanged();
			RectangleF rect = ShapeHelper.GetShapeBounds(shape);
			if (rect == Rectangle.Empty) throw new Exception("ShapeHelper.GetShapeBounds returned empty rect WTF!");
			for (int i = 0; i < points.Length; i++) {
				ShapePoint point = points[i];
				points[i].SetPoint(new PointF(point.Point.X - rect.X, point.Point.Y - rect.Y));
			}
		}
		protected void ParseShapeByType(XmlNode shapeNode, BaseShape shape) {
			ShapePoint[] points;
			switch (shapeNode.Name) {
				case "Path":
					points = PathDataParser.GetPathPoints(shapeNode);
					PathShape pathShape = (PathShape)shape;
					pathShape.Points = points;
					if (pathShape.Points.Length == 0) throw new XAMLLoadExceptions(XAMLLoadExceptions.PathParseError);
					NormalizePoints(pathShape, points);
					pathShape.Points = points;
					break;
				case "Rectangle":
				case "Ellipse":
					BoxShape boxShape = (BoxShape)shape;
					boxShape.Box = shape.Bounds;
					boxShape.ForceShapeChanged();
					RectangleF rect = ShapeHelper.GetShapeBounds(boxShape);
					boxShape.Box = new RectangleF2D(boxShape.Bounds.Location - new SizeF(rect.X, rect.Y), boxShape.Bounds.Size);
					break;
			}
		}
		protected bool CreateXMLDocumentTryLoad(Stream stream) {
			this.xamlDoc = new XmlDocument();
			try { Document.Load(stream); } catch { return false; }
			return true;
		}
	}
	public class PathDataParser {
		public static ShapePoint[] GetPathPoints(XmlNode pathNode) {
			if (pathNode.Attributes != null && pathNode.Attributes["Data"] != null) {
				List<ShapePoint> list = new List<ShapePoint>();
				string dataStr = pathNode.Attributes["Data"].Value.ToLowerInvariant();
				int current = dataStr.IndexOf("m");
				bool needStartPath = true;
				while (dataStr.Length > current) {
					ShapePoint point = ParseShapePoint(dataStr, ref current);
					if (point != null) {
						if (needStartPath) {
							point.PointType = PathPointType.Start;
							needStartPath = false;
						}
						list.Add(point);
					} else {
						list[list.Count - 1].PointType |= PathPointType.CloseSubpath;
						needStartPath = true;
					}
				}
				return list.ToArray();
			}
			return new ShapePoint[0];
		}
		static int SkipWhitespaces(string str, int startIndex) {
			int current = startIndex;
			while (str.Length > current && (str[current] == ' ' || str[current] == ',' || str[current] == '\r' || str[current] == '\n' || str[current] == '\t')) current++;
			if (str.Length > current && str[current] == '&')
				while (str.Length > current && (str[current] != ';')) current++;
			return current;
		}
		static bool IsNumberChar(char c) {
			return char.IsDigit(c) || c == '.' || c == '-' || c == '+' || c == 'e';
		}
		static string GetDigits(string str, int startIndex) {
			int current = startIndex;
			while (str.Length > current && IsNumberChar(str[current])) current++;
			return str.Substring(startIndex, current - startIndex);
		}
		static ShapePoint ParseShapePoint(string str, ref int index) {
			float x = 0, y = 0;
			char letter = ParseCoordinate(str, ref x, ref index);
			if (letter == 'z') {
				index++;
				return null;
			}
			ParseCoordinate(str, ref y, ref index);
			return new ShapePoint(new PointF(x, y), PathPointTypeConverter.Convert(letter));
		}
		static char ParseCoordinate(string str, ref float x, ref int currentIndex) {
			char letter = ' ';
			currentIndex = SkipWhitespaces(str, currentIndex);
			if (currentIndex < str.Length && !IsNumberChar(str[currentIndex])) {
				letter = str[currentIndex];
				if (str.Length > currentIndex) currentIndex++;
			}
			currentIndex = SkipWhitespaces(str, currentIndex);
			if (currentIndex < str.Length && IsNumberChar(str[currentIndex])) {
				string number = GetDigits(str, currentIndex);
				currentIndex += number.Length;
				x = float.Parse(number, CultureInfo.InvariantCulture);
			}
			return letter;
		}
	}
}
