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
namespace DevExpress.XtraExport {
	public class XdrOpenXmlElement : OpenXmlElement {
		public XdrOpenXmlElement(XmlDocument document)
			: base(document) {
			Prefix = "xdr";
		}
		public override string NamespaceURI { get { return XlsxHelper.XdrNs; } }
	}
	public class ExtNode : XdrOpenXmlElement {
		int cx;
		int cy;
		public ExtNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "ext"; } }
		public int Cx {
			get { return cx; }
			set { cx = value; }
		}
		public int Cy {
			get { return cy; }
			set { cy = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "cx", cx.ToString());
			XlsxHelper.AppendAttribute(this, "cy", cy.ToString());
		}
	}
	public abstract class ConnectNodeBase : XdrOpenXmlElement {
		int col;
		int row;
		int colOff;
		int rowOff;
		public ConnectNodeBase(XmlDocument document)
			: base(document) {
		}
		public int Col {
			get { return col; }
			set { col = value; }
		}
		public int Row {
			get { return row; }
			set { row = value; }
		}
		public int ColOff {
			get { return colOff; }
			set { colOff = value; }
		}
		public int RowOff {
			get { return rowOff; }
			set { rowOff = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendXdrNode(OwnerDocument, this, "col", col.ToString());
			XlsxHelper.AppendXdrNode(OwnerDocument, this, "colOff", colOff.ToString());
			XlsxHelper.AppendXdrNode(OwnerDocument, this, "row", row.ToString());
			XlsxHelper.AppendXdrNode(OwnerDocument, this, "rowOff", rowOff.ToString());
		}
	}
	public class FromNode : ConnectNodeBase {
		public FromNode(XmlDocument document) 
			: base(document) {
		}
		public override string LocalName { get { return "from"; } }
	}
	public class ToNode : ConnectNodeBase {
		public ToNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "to"; } }
	}
	public abstract class NvPrNodeBase : XdrOpenXmlElement {
		int id;
		string hyperLinkId;
		public NvPrNodeBase(XmlDocument document)
			: base(document) {
		}
		public int Id {
			get { return id; }
			set { id = value; }
		}
		public string HyperLinkId {
			get { return hyperLinkId; }
			set { hyperLinkId = value;}
		}
		public abstract string ObjectName { get; }
		public abstract string CanvasLocalName { get; }
		protected override void CollectDataBeforeWrite() {
			XmlNode cNvPrNode = XlsxHelper.AppendXdrNode(OwnerDocument, this, "cNvPr", string.Empty);
			XlsxHelper.AppendAttribute(cNvPrNode, "id", id.ToString());
			XlsxHelper.AppendAttribute(cNvPrNode, "name", ObjectName + " " + id.ToString());
			if(!string.IsNullOrEmpty(hyperLinkId)) {
				XmlNode hyperLink = XlsxHelper.AppendANode(OwnerDocument, cNvPrNode, "hlinkClick", string.Empty);
				XlsxHelper.AppendAttribute(hyperLink, "id", hyperLinkId, "r", XlsxHelper.RelationshipsNs);
			}
			XlsxHelper.AppendXdrNode(OwnerDocument, this, CanvasLocalName, string.Empty);
		}
	}
	public class NvCxnSpPrNode : NvPrNodeBase {
		public NvCxnSpPrNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "nvCxnSpPr"; } }
		public override string CanvasLocalName { get { return "cNvCxnSpPr"; } }
		public override string ObjectName { get { return "LineShape"; } }
	}  
	public class NvPicPrNode : NvPrNodeBase {
		public NvPicPrNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "nvPicPr"; } }
		public override string CanvasLocalName { get { return "cNvPicPr"; } }
		public override string ObjectName { get { return "Picture"; } }
	}
	public abstract class SpPrNodeBase : XdrOpenXmlElement {
		int cx, cy, offX, offY;
		XmlNode xfrmExtNode, xfrmOffNode;
		protected XmlNode xfrmNode;
		public SpPrNodeBase(XmlDocument document)
			: base(document) {
			xfrmNode = XlsxHelper.AppendANode(document, this, "xfrm", string.Empty);
			xfrmOffNode = XlsxHelper.AppendANode(document, xfrmNode, "off", string.Empty);
			xfrmExtNode = XlsxHelper.AppendANode(document, xfrmNode, "ext", string.Empty);
			XmlNode prstGeomNode = XlsxHelper.AppendANode(document, this, "prstGeom", string.Empty);
			XlsxHelper.AppendAttribute(prstGeomNode, "prst", PrstValue);
		}
		public override string LocalName {
			get { return "spPr"; }
		}
		public int Cx {
			get { return cx; }
			set { cx = value; }
		}
		public int Cy {
			get { return cy; }
			set { cy = value; }
		}
		public int OffX {
			get { return offX; }
			set { offX = value; }
		}
		public int OffY {
			get { return offX; }
			set { offY = value; }
		}
		public abstract string PrstValue { get; }
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(xfrmOffNode, "x", offX.ToString());
			XlsxHelper.AppendAttribute(xfrmOffNode, "y", offY.ToString());
			XlsxHelper.AppendAttribute(xfrmExtNode, "cx", cx.ToString());
			XlsxHelper.AppendAttribute(xfrmExtNode, "cy", cy.ToString());	  
		}
	}
	public class SpPrPicNode : SpPrNodeBase {
		public SpPrPicNode(XmlDocument document)
			: base(document) {
		}
		public override string PrstValue { get { return "rect"; } }
	}
	public class SpPrShapeNode : SpPrNodeBase {
		bool isVerticalFlip;
		public bool IsVerticalFlip {
			get { return isVerticalFlip; }
			set { isVerticalFlip = value; }
		}
		public SpPrShapeNode(XmlDocument document)
			: base(document) {
		}
		public override string PrstValue { get { return "line"; } }
		protected override void CollectDataBeforeWrite() {
			base.CollectDataBeforeWrite();
			if(isVerticalFlip)
				XlsxHelper.AppendAttribute(xfrmNode, "flipH",  "1");
		}
	}
	public class SrcRectNode : OpenXmlElement {
		decimal l, r, t, b;
		public SrcRectNode(XmlDocument document)
			: base(document) {
			Prefix = "a";
		}
		public override string NamespaceURI { get { return XlsxHelper.ANs; } }
		public override string LocalName {
			get { return "srcRect"; }
		}
		public decimal L {
			get { return l; }
			set { l = value; }
		}
		public decimal R {
			get { return r; }
			set { r = value; }
		}
		public decimal T {
			get { return t; }
			set { t = value; }
		}
		public decimal B {
			get { return b; }
			set { b = value; }
		}
		public bool IsZero { get { return L == 0 && R == 0 && T == 0 && B == 0; } }
		string GetString(decimal value) {
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture) + "%";
		}
		protected override void CollectDataBeforeWrite() {
			if (l != 0)
				XlsxHelper.AppendAttribute(this, "l", GetString(l));
			if (r != 0)
				XlsxHelper.AppendAttribute(this, "r", GetString(r));
			if (t != 0)
				XlsxHelper.AppendAttribute(this, "t", GetString(t));
			if (b != 0)
				XlsxHelper.AppendAttribute(this, "b", GetString(b));
		}
	}
	public class LnNode : OpenXmlElement {
		string color, style;
		int width;
		public LnNode(XmlDocument document)
			: base(document) {
			Prefix = "a";
		}
		public override string NamespaceURI { get { return XlsxHelper.ANs; } }
		public override string LocalName {
			get { return "ln"; }
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public string Color {
			get { return color; }
			set { color = value; }
		}
		public string Style {
			get { return style; }
			set { style = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "w", width.ToString());
			XmlNode solidFillNode = XlsxHelper.AppendANode(OwnerDocument, this, "solidFill", string.Empty);
			XmlNode srgbClr = XlsxHelper.AppendANode(OwnerDocument, solidFillNode, "srgbClr", string.Empty);
			XlsxHelper.AppendAttribute(srgbClr, "val", color);
			XmlNode prstDashNode = XlsxHelper.AppendANode(OwnerDocument, this, "prstDash", string.Empty);
			XlsxHelper.AppendAttribute(prstDashNode, "val", style);
		}
	}
	public class PicNode : XdrOpenXmlElement {
		NvPicPrNode nvPicPrNode;
		BlipFillNode blipFillNode;
		SpPrPicNode spPrNode;
		public PicNode(XmlDocument document)
			: base(document) {
			nvPicPrNode = new NvPicPrNode(document);
			AppendChild(nvPicPrNode);
			blipFillNode = new BlipFillNode(document);
			AppendChild(blipFillNode);
			spPrNode = new SpPrPicNode(document);
			AppendChild(spPrNode);
		}
		public NvPicPrNode NvPicPrNode {
			get { return nvPicPrNode; }
			set { nvPicPrNode = value; }
		}
		public BlipFillNode BlipFillNode {
			get { return blipFillNode; }
			set { blipFillNode = value; }
		}
		public SpPrPicNode SpPrNode {
			get { return spPrNode; }
			set { spPrNode = value; }
		}
		public override string LocalName { get { return "pic"; } }
	}
	public class LineShapeNode : XdrOpenXmlElement {
		NvCxnSpPrNode nvCxnSpPrNode;
		SpPrShapeNode spPrNode;
		LnNode lnNode;
		public LineShapeNode(XmlDocument document)
			: base(document) {
			nvCxnSpPrNode = new NvCxnSpPrNode(document);
			AppendChild(nvCxnSpPrNode);
			spPrNode = new SpPrShapeNode(document);
			AppendChild(spPrNode);
			lnNode = new LnNode(document);
			spPrNode.AppendChild(lnNode);
		}
		public NvCxnSpPrNode NvCxnSpPrNode {
			get { return nvCxnSpPrNode; }
			set { nvCxnSpPrNode = value; }
		}
		public SpPrShapeNode SpPrNode {
			get { return spPrNode; }
			set { spPrNode = value; }
		}
		public LnNode LnNode {
			get { return lnNode; }
			set { lnNode = value; }
		}
		public override string LocalName { get { return "cxnSp"; } }  
	}
	public class BlipFillNode : XdrOpenXmlElement {
		string id;
		SrcRectNode srcRectNode;
		public BlipFillNode(XmlDocument document)
			: base(document) {
			srcRectNode = new SrcRectNode(document);
		}
		public override string LocalName {
			get { return "blipFill"; }
		}
		public string Id {
			get { return id; }
			set { id = value; }
		}
		public SrcRectNode SrcRectNode { get { return srcRectNode; } }
		protected override void CollectDataBeforeWrite() {
			XmlNode blipNode = XlsxHelper.AppendANode(OwnerDocument, this, "blip", string.Empty);
			XlsxHelper.AppendAttribute(blipNode, "xmlns:r", XlsxHelper.RelationshipsNs);
			XlsxHelper.AppendAttribute(blipNode, "embed", id.ToString(), "r", XlsxHelper.RelationshipsNs);
			if(srcRectNode.IsZero)
				XlsxHelper.AppendANode(OwnerDocument, this, "srcRect", string.Empty);
			else
				AppendChild(srcRectNode);
			XmlNode stretchNode = XlsxHelper.AppendANode(OwnerDocument, this, "stretch", string.Empty);
			XlsxHelper.AppendANode(OwnerDocument, stretchNode, "fillRect", string.Empty);
		}
	}
	public class OneCellAnchorNode : XdrOpenXmlElement {
		FromNode fromNode;
		ExtNode extNode;
		PicNode picNode;
		public OneCellAnchorNode(XmlDocument document)
			: base(document) {
			fromNode = new FromNode(document);
			AppendChild(fromNode);
			extNode = new ExtNode(document);
			AppendChild(extNode);
			picNode = new PicNode(document);
			AppendChild(picNode);
			XlsxHelper.AppendXdrNode(document, this, "clientData", string.Empty);
		}
		public override string LocalName { get { return "oneCellAnchor"; } }
		public FromNode FromNode {
			get { return fromNode; }
			set { fromNode = value; }
		}
		public ExtNode ExtNode {
			get { return extNode; }
			set { extNode = value; }
		}
		public PicNode PicNode {
			get { return picNode; }
			set { picNode = value; }
		}
	}
	abstract public class TwoCellAnchorNodeBase : XdrOpenXmlElement {
		FromNode fromNode;
		ToNode toNode;
		protected XdrOpenXmlElement childNode;
		public TwoCellAnchorNodeBase(XmlDocument document)
			: base(document) {
			fromNode = new FromNode(document);
			AppendChild(fromNode);
			toNode = new ToNode(document);
			AppendChild(toNode);
			childNode = CreateChildNode(document);
			AppendChild(childNode);
			XlsxHelper.AppendXdrNode(document, this, "clientData", string.Empty);
		}
		public abstract XdrOpenXmlElement CreateChildNode(XmlDocument document);
		public override string LocalName { get { return "twoCellAnchor"; } }
		public FromNode FromNode {
			get { return fromNode; }
			set { fromNode = value; }
		}
		public ToNode ToNode {
			get { return toNode; }
			set { toNode = value; }
		}
	}
	public class TwoCellAnchorPictureNode : TwoCellAnchorNodeBase {
		public PicNode PicNode {
			get { return (PicNode)childNode; }
			set { childNode = value; }
		}
		public TwoCellAnchorPictureNode(XmlDocument document) 
			 :  base(document) {
		}
		public override XdrOpenXmlElement CreateChildNode(XmlDocument document) {
			return new PicNode(document);
		}  
	}
	public class TwoCellAnchorLineShapeNode : TwoCellAnchorNodeBase {
		public LineShapeNode ShapeNode {
			get { return (LineShapeNode)childNode; }
			set { childNode = value; }
		}
		public TwoCellAnchorLineShapeNode(XmlDocument document)
			: base(document) {
		}
		public override XdrOpenXmlElement CreateChildNode(XmlDocument document) {
			return new LineShapeNode(document);
		}
	}
	public class WsDrNode : XdrOpenXmlElement {
		public WsDrNode(XmlDocument document)
			: base(document) {
			XlsxHelper.AppendAttribute(this, "xmlns:xdr", XlsxHelper.XdrNs);
			XlsxHelper.AppendAttribute(this, "xmlns:a", XlsxHelper.ANs);
		}
		public override string LocalName { get { return "wsDr"; } }
		public OneCellAnchorNode AppendOneCellAnchorNode() {
			OneCellAnchorNode node = new OneCellAnchorNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
		public TwoCellAnchorPictureNode AppendTwoCellAnchorNode() {
			TwoCellAnchorPictureNode node = new TwoCellAnchorPictureNode(OwnerDocument);
			XlsxHelper.AppendAttribute(node, "editAs", "oneCell");
			AppendChild(node);
			return node;
		}
		public TwoCellAnchorLineShapeNode AppendTwoCellAnchorLineShapeNode() {
			TwoCellAnchorLineShapeNode node = new TwoCellAnchorLineShapeNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
	}
	public class DrawingDocument: OpenXmlDocument {
		WsDrNode wsDrNode;
		public DrawingDocument() {
			wsDrNode = new WsDrNode(this);
			AppendChild(wsDrNode);
		}
		public WsDrNode WsDrNode {
			get { return wsDrNode; }
			set { wsDrNode = value; }
		}
	}
}
