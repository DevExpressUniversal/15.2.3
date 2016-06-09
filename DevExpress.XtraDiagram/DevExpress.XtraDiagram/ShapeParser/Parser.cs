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
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram.Parser {
	public abstract class DiagramPathViewParserBase : ShapeParser, IDisposable {
		DiagramItemParseStrategy parseStrategy;
		public DiagramPathViewParserBase(DiagramItemParseStrategy parseStrategy) : base(parseStrategy) {
			this.parseStrategy = parseStrategy;
		}
		public DiagramItemView Parse(IXtraPathView view, Rectangle bounds, DiagramAppearanceObject appearance) {
			DiagramItemView itemView = new DiagramItemView();
			ParseStrategy.BeginParse(newPath => itemView.AddPath(newPath));
			try {
				Parse(view.Shape);
			}
			finally {
				ParseStrategy.EndParse();
			}
			if(!string.IsNullOrEmpty(view.Text)) {
				itemView.AddPath(CreateDiagramTextPath(view, bounds, appearance.CreateTextPathOptions()));
			}
			TransformView(itemView, bounds, view.Angle);
			return itemView;
		}
		protected void TransformView(DiagramItemView itemView, Rectangle bounds, double angle) {
			itemView.ForEach(path => {
				if(AllowTransformPath(path)) TranslateTransformPath(path, bounds, angle);
			});
			if(MathUtils.IsNotEquals(angle, 0)) {
				itemView.ForEach(path => {
					if(AllowTransformPath(path)) RotateTransformPath(path, bounds, angle);
				});
			}
		}
		protected virtual bool AllowTransformPath(DiagramGraphicsPath path) {
			return true;
		}
		protected void TranslateTransformPath(DiagramGraphicsPath path, Rectangle bounds, double angle) {
			path.TranslateTransform(bounds);
		}
		protected void RotateTransformPath(DiagramGraphicsPath path, Rectangle bounds, double angle) {
			path.RotateTransform(-angle, bounds);
		}
		protected virtual DiagramGraphicsPath CreateDiagramTextPath(IXtraPathView view, Rectangle bounds, TextPathOptions options) {
			return CreateDiagramPath(CreateTextPath(view, options));
		}
		protected abstract XtraGraphicsPath CreateTextPath(IXtraPathView view, TextPathOptions options);
		protected abstract DiagramGraphicsPath CreateDiagramPath(XtraGraphicsPath path);
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			this.parseStrategy = null;
		}
		#endregion
		protected DiagramItemParseStrategy ParseStrategy { get { return parseStrategy; } }
	}
	public abstract class DiagramPathViewParser : DiagramPathViewParserBase {
		DiagramXtraPathCache pathCache;
		public DiagramPathViewParser(DiagramItemParseStrategy parseStrategy) : base(parseStrategy) {
			this.pathCache = new DiagramXtraPathCache();
		}
		protected override DiagramGraphicsPath CreateDiagramTextPath(IXtraPathView view, Rectangle bounds, TextPathOptions options) {
			bool cacheHit;
			XtraGraphicsPath path = this.pathCache.GetPath(view, options, CreateTextPath, out cacheHit);
			path.OffsetTransform(GetTextPosition(view, bounds));
			if(!cacheHit) {
				if(MathUtils.IsNotEquals(view.Angle, 0)) path.RotateTransform(-view.Angle, bounds);
			}
			return CreateDiagramPath(path);
		}
		protected virtual Point GetTextPosition(IXtraPathView view, Rectangle bounds) {
			return PointUtils.ApplyOffset(bounds.Location, view.TextBounds.Location);
		}
		protected override bool AllowTransformPath(DiagramGraphicsPath path) {
			if(path.ContainsText) return false;
			return base.AllowTransformPath(path);
		}
		#region IDisposable
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.pathCache != null) this.pathCache.Dispose();
			}
			this.pathCache = null;
			base.Dispose(disposing);
		}
		#endregion
	}
	public class DiagramSimpleShapeParser : DiagramPathViewParserBase {
		public DiagramSimpleShapeParser(DiagramItemParseStrategy parseStrategy)
			: base(parseStrategy) {
		}
		protected override XtraGraphicsPath CreateTextPath(IXtraPathView view, TextPathOptions options) {
			return GraphicsPathUtils.CreateShapeTextPath(view, options);
		}
		protected override DiagramGraphicsPath CreateDiagramPath(XtraGraphicsPath path) {
			return new DiagramShapeTextGraphicsPath(path);
		}
	}
	public class DiagramShapeParser : DiagramPathViewParser {
		public DiagramShapeParser(DiagramItemParseStrategy parseStrategy)
			: base(parseStrategy) {
		}
		protected override XtraGraphicsPath CreateTextPath(IXtraPathView view, TextPathOptions options) {
			return GraphicsPathUtils.CreateShapeTextPath(view, options);
		}
		protected override DiagramGraphicsPath CreateDiagramPath(XtraGraphicsPath path) {
			return new DiagramShapeTextGraphicsPath(path);
		}
	}
	public class DiagramSimpleConnectorParser : DiagramPathViewParserBase {
		public DiagramSimpleConnectorParser(DiagramItemParseStrategy parseStrategy)
			: base(parseStrategy) {
		}
		protected override XtraGraphicsPath CreateTextPath(IXtraPathView view, TextPathOptions options) {
			return GraphicsPathUtils.CreateConnectorTextPath(view, options);
		}
		protected override DiagramGraphicsPath CreateDiagramPath(XtraGraphicsPath path) {
			return new DiagramConnectorTextGraphicsPath(path);
		}
	}
	public class DiagramConnectorParser : DiagramPathViewParser {
		public DiagramConnectorParser(DiagramItemParseStrategy parseStrategy)
			: base(parseStrategy) {
		}
		protected override XtraGraphicsPath CreateTextPath(IXtraPathView view, TextPathOptions options) {
			return GraphicsPathUtils.CreateConnectorTextPath(view, options);
		}
		protected override DiagramGraphicsPath CreateDiagramPath(XtraGraphicsPath path) {
			return new DiagramConnectorTextGraphicsPath(path);
		}
	}
}
