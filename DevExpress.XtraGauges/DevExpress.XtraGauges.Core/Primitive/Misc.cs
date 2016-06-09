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
using System.Drawing.Drawing2D;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraGauges.Core.Primitive {
	public class CustomDrawElementEventArgs : EventArgs {
		IRenderingContext contextCore;
		IViewInfo infoCore;
		BaseShapeCollection shapesCore;
		bool handledCore;
		BaseShapePainter painterCore;
		public CustomDrawElementEventArgs(IRenderingContext context, BaseShapePainter painter, IViewInfo info, BaseShapeCollection shapes) {
			this.contextCore = context;
			this.painterCore = painter;
			this.infoCore = info;
			this.shapesCore = shapes;
		}
		public BaseShapePainter Painter {
			get{return painterCore;}
		}
		public BaseShapeCollection Shapes {
			get { return shapesCore; }
		}
		public IRenderingContext Context {
			get { return contextCore; }
		}
		public IViewInfo Info {
			get { return infoCore; }
		}
		public bool Handled {
			get { return handledCore; }
			set { handledCore = value; }
		}
		public void Draw() {
			using(PaintInfo paintInfo = new PaintInfo(Context)) {
				Painter.Draw(paintInfo, Shapes);
			}
		}
	}
	public delegate void CustomDrawElementEventHandler(object sender, CustomDrawElementEventArgs e);
	public delegate void TransformChangedHandler();
	public class CacheKeys {
		public static readonly object TransformationMatrix = new object();
		public static readonly object RelativeBoundBox = new object();
		public static readonly object RenderedImage = new object();
		public static string ShapePathCacheName(BaseShape complexShape, BaseShape shape) {
			return complexShape.Name + "[CSP]" + shape.Name;
		}
	}
	public enum PrimitiveState { Normal, Selected, Pressed, HotTracked ,Disabled }
	public abstract class BaseHitInfo :BaseObject{
		static Point emptyPoint = new Point(int.MinValue, int.MinValue);
		Point hitPointCore;
		protected BaseHitInfo(Point point) {
			this.hitPointCore = point;
		}
		protected override void OnCreate() {
			this.hitPointCore = EmptyPoint;
		}
		public virtual void Clear() {
			this.hitPointCore = EmptyPoint;
		}
		public static Point EmptyPoint {
			get { return emptyPoint; }
		}
		public virtual Point HitPoint { 
			get { return hitPointCore; } 
		}
		public bool IsValid { 
			get { return HitPoint != EmptyPoint; } 
		}
		public abstract bool IsEmpty { get;}
	}
	public class BasePrimitiveHitInfo : BaseHitInfo {
		IRenderableElement elementCore;
		public static BasePrimitiveHitInfo Empty;
		static BasePrimitiveHitInfo() {
			Empty = new EmptyPrimitiveHitInfo();
		}
		public BasePrimitiveHitInfo(IRenderableElement element, Point point)
			: base(point) {
			this.elementCore = element;
		}
		protected override void OnDispose() { }
		public IRenderableElement Element {
			get { return elementCore; }
		}
		public override bool IsEmpty {
			get { return false; }
		}
		class EmptyPrimitiveHitInfo : BasePrimitiveHitInfo {
			public EmptyPrimitiveHitInfo() : base(null,EmptyPoint) { }
			public override bool IsEmpty {
				get { return true; }
			}
		}
	}
}
