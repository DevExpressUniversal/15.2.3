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
using System.Reflection;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class EmfPlusRegion {
		public Region Region { get; set; }
		public EmfPlusRegion(MetaReader reader) {
			new EmfPlusGraphicsVersion(reader);
			Region = new Region();
			uint regionNodeCount = reader.ReadUInt32();
				Region = new EmfPlusRegionNode(reader).Region;
		}
	}
	public interface IRegionNodeVisitor<T> {
		T Visit(RegionRectNode node);
		T Visit(RegionPathNode node);
		T Visit(RegionEmptyNode node);
		T Visit(RegionInfiniteNode node);
		T Visit(RegionChildNodesNode node);
	}
	public abstract class RegionNode {
		public abstract RegionNodeDataType Type { get; }
		public static RegionNode Read(MetaReader reader) {
			RegionNodeDataType type = (RegionNodeDataType)reader.ReadUInt32();
			switch(type) {
				case RegionNodeDataType.RegionNodeDataTypeRect:
					return new RegionRectNode(reader);
				case RegionNodeDataType.RegionNodeDataTypePath:
					return new RegionPathNode(reader);
				case RegionNodeDataType.RegionNodeDataTypeEmpty:
					return new RegionEmptyNode();
				case RegionNodeDataType.RegionNodeDataTypeInfinite:
					return new RegionInfiniteNode();
				default:
					return new RegionChildNodesNode(reader, type);
			}
		}
		public abstract T Accept<T>(IRegionNodeVisitor<T> visitor);
	}
	public class RegionRectNode : RegionNode {
		public RectangleF Rect { get; private set; }
		public override RegionNodeDataType Type {
			get { return RegionNodeDataType.RegionNodeDataTypeRect; }
		}
		public RegionRectNode(MetaReader reader) {
			this.Rect = reader.ReadRectF();
		}
		public override T Accept<T>(IRegionNodeVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
	public class RegionPathNode : RegionNode {
		public GraphicsPath Path { get; private set; }
		public override RegionNodeDataType Type {
			get { return RegionNodeDataType.RegionNodeDataTypePath; }
		}
		public RegionPathNode(MetaReader reader) {
			int regionNodePathLength = reader.ReadInt32();
			MetaReader rd = new MetaReader(new MemoryStream(reader.ReadBytes(regionNodePathLength)));
			Path = new EmfPlusPath(rd).Path;
		}
		public override T Accept<T>(IRegionNodeVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
	public class RegionEmptyNode : RegionNode {
		public override RegionNodeDataType Type {
			get { return RegionNodeDataType.RegionNodeDataTypeEmpty; }
		}
		public override T Accept<T>(IRegionNodeVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
	public class RegionInfiniteNode : RegionNode {
		public override RegionNodeDataType Type {
			get { return RegionNodeDataType.RegionNodeDataTypeInfinite; }
		}
		public override T Accept<T>(IRegionNodeVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
	public class RegionChildNodesNode : RegionNode {
		RegionNodeDataType type;
		public RegionNode Left { get; private set; }
		public RegionNode Right { get; private set; }
		public override RegionNodeDataType Type {
			get { return type; }
		}
		public RegionChildNodesNode(MetaReader reader, RegionNodeDataType type) {
			this.type = type;
			this.Left = RegionNode.Read(reader);
			this.Right = RegionNode.Read(reader);
		}
		public override T Accept<T>(IRegionNodeVisitor<T> visitor) {
			return visitor.Visit(this);
		}
	}
	public class UnionAll : IRegionNodeVisitor<GraphicsPath> {
		public GraphicsPath Process(RegionNode node) {
			return node.Accept(this);
		}
		public GraphicsPath Visit(RegionRectNode node) {
			GraphicsPath path = new GraphicsPath();
			path.AddRectangle(node.Rect);
			return path;
		}
		public GraphicsPath Visit(RegionPathNode node) {
			return node.Path;
		}
		public GraphicsPath Visit(RegionEmptyNode node) {
			return null;
		}
		public GraphicsPath Visit(RegionInfiniteNode node) {
			return null;
		}
		public GraphicsPath Visit(RegionChildNodesNode node) {
			GraphicsPath path = new GraphicsPath();
			AddPath(path, Process(node.Left));
			AddPath(path, Process(node.Right));
			return path;
		}
		static void AddPath(GraphicsPath path, GraphicsPath subPath) {
			if(subPath != null && subPath.PointCount > 0)
				path.AddPath(subPath, false);
		}
	}
}
