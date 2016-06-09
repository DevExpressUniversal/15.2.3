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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
namespace DevExpress.XtraDiagram {
	public partial class DiagramShape : DiagramItem, IDiagramShape, IXtraPathView {
		public DiagramShape() {
		}
		public DiagramShape(ShapeDescription shape) {
			SetShape(shape);
		}
		public DiagramShape(ShapeDescription shape, Rectangle bounds)
			: this(shape, bounds.X, bounds.Y, bounds.Width, bounds.Height) {
		}
		public DiagramShape(ShapeDescription shape, int x, int y, int width, int height) : base(x, y, width, height) {
			SetShape(shape);
		}
		public DiagramShape(ShapeDescription shape, Rectangle bounds, string content)
			: this(shape, bounds.X, bounds.Y, bounds.Width, bounds.Height, content) {
		}
		public DiagramShape(ShapeDescription shape, int x, int y, int width, int height, string content) : base(x, y, width, height) {
			this._Content = content;
			SetShape(shape);
		}
		protected sealed override DiagramItemController CreateItemController() {
			return CreateDiagramShapeController();
		}
		protected virtual DiagramShapeController CreateDiagramShapeController() {
			return new DiagramShapeController(this);
		}
		protected void SetShape(ShapeDescription shape) {
			this._Shape = shape;
			Controller.UpdateShape();
		}
		protected override void InvalidateVisual() {
			Update();
		}
		protected virtual void Update() {
			Controller.UpdateShape();
		}
		protected internal override string EditValue {
			get { return Content; }
			set { Content = value; }
		}
		protected virtual void OnShapeChanged(ShapeDescription oldShape) {
			Update();
		}
		protected virtual void UpdateShape() {
			Update();
		}
		protected internal override bool IsRoutable { get { return false; } }
		#region IXtraPathView
		int IXtraPathView.ItemId { get { return ItemId; } }
		ShapeGeometry IXtraPathView.Shape {
			get { return Controller.Shape; }
		}
		string IXtraPathView.Text { get { return Content; } }
		Rectangle IXtraPathView.TextBounds {
			get { return Controller.EditorBounds.ToWinRect();  }
		}
		#endregion
		protected internal new DiagramShapeController Controller { get { return base.Controller as DiagramShapeController; } }
	}
	public enum TextDecoration {
		None, Underline, Strikethrough
	}
	public class DoubleCollection : List<double> {
		public DoubleCollection(IEnumerable<double> items) : base(items) { }
	}
}
