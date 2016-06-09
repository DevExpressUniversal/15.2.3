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
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Brick = {Brick}}")]
#endif
	[BrickExporter(typeof(BrickContainerBaseExporter))]
	public class BrickContainerBase : Brick {
		Brick baseBrick;
		internal override IList InnerBrickList {
			get { return new BrickBase[] { baseBrick }; }
		}
		internal override PointF InnerBrickListOffset { get { return new PointF(-Brick.X, -Brick.Y); } }
		protected internal override RectangleF InitialRect {
			get {
				PointF pt = this.baseBrick.InitialRect.Location;
				return new RectangleF(AdjustLocation(pt), this.Size);
			}
		}
		protected RectangleF InitialRectCore {
			get {
				return base.InitialRect;
			}
		}
		public override BrickCollectionBase Bricks { get { return baseBrick.Bricks; } }
		public override bool SeparableHorz { get { return baseBrick.SeparableHorz; } }
		public override bool SeparableVert { get { return baseBrick.SeparableVert; } }
		[XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, 0, XtraSerializationFlags.Cached)]
		public Brick Brick { get { return baseBrick; } set { baseBrick = value; } }
		public override string BrickType { get { return BrickTypes.ContainerBase; } }
		public BrickContainerBase() {
		}
		public BrickContainerBase(Brick baseBrick)
			: base() {
			if(baseBrick == null)
				throw new ArgumentNullException("baseBrick");
			this.baseBrick = baseBrick;
		}
		internal override Brick GetRealBrick() {
			return baseBrick.GetRealBrick();
		}
		internal protected virtual PointF AdjustLocation(PointF pt) {
			return pt;
		}
		public override float ValidatePageBottom(RectangleF pageBounds, bool enforceSplitNonSeparable, RectangleF rect, IPrintingSystemContext context) {
			return baseBrick.ValidatePageBottom(pageBounds, enforceSplitNonSeparable, GetBrickRect(rect.Location), context);
		}
		public override float ValidatePageRight(float pageRight, RectangleF rect) {
			return baseBrick.ValidatePageRight(pageRight, GetBrickRect(rect.Location));
		}
		internal RectangleF GetBrickRect(PointF location) {
			return new RectangleF(AdjustLocation(location), baseBrick.Size);
		}
		protected override object CreateContentPropertyValue(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.Brick)
				return BrickFactory.CreateBrick(e);
			return base.CreateContentPropertyValue(e);
		}
		public override IEnumerator GetEnumerator() {
			return new object[] { baseBrick }.GetEnumerator();
		}
	}
	[
	BrickExporter(typeof(BrickContainerExporter))
	]
	public class BrickContainer : BrickContainerBase {
		float brickOffsetX = 0;
		float brickOffsetY = 0;
		internal override PointF InnerBrickListOffset { get { return new PointF(brickOffsetX - Brick.X, brickOffsetY - Brick.Y); } }
		[
		XtraSerializableProperty,
		DefaultValue(0f),
		]
		public float BrickOffsetX { get { return brickOffsetX; } set { brickOffsetX = value; } }
		[
		XtraSerializableProperty,
		DefaultValue(0f),
		]
		public float BrickOffsetY { get { return brickOffsetY; } set { brickOffsetY = value; } }
		public override string BrickType { get { return BrickTypes.Container; } }
		public BrickContainer() {
		}
		public BrickContainer(Brick baseBrick)
			: base(baseBrick) {
		}
		internal override RectangleF DocumentBandRect {
			get {
				PointF pt = this.Brick.InitialRect.Location;
				pt.X -= brickOffsetX;
				pt.Y -= brickOffsetY;
				return new RectangleF(pt, this.Size);
			}
		}
		internal protected override PointF AdjustLocation(PointF pt) {
			return new PointF(pt.X + brickOffsetX, pt.Y + brickOffsetY);
		}
	}
	public class BrickWrapper : BrickContainerBase {
		internal override PointF InnerBrickListOffset { get { return PointF.Empty; } }
		public BrickWrapper(Brick baseBrick)
			: base(baseBrick) {
		}
		internal override RectangleF DocumentBandRect {
			get {
				return InitialRect;
			}
		}
		protected internal override RectangleF InitialRect {
			get {
				return InitialRectCore;
			}
		}
	}
}
