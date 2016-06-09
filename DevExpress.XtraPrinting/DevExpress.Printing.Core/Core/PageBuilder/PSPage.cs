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
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export.Web;
namespace DevExpress.XtraPrinting.Native {
	public class PSPage : Page {
		BrickList bricks = new BrickList();
		SizeF bricksSize = SizeF.Empty;
		bool isAdditional;
		bool locked;
		internal bool IsEmpty { get { return InnerBricks.Count == 0; } }
		internal SizeF BricksSize { get { return bricksSize; } }
		public BrickList Bricks { get { return bricks; } }
		public SizeF ClippedPageSize {
			get {
				return new SizeF(PageSize.Width - MarginsF.Left - MarginsF.Right, GetClippedPageHeight());
			}
		}
		public bool Additional {
			get { return isAdditional; }
		}
		public RectangleF TopMarginBounds {
			get {
				return GetIntersectedBrickBounds(InnerBricks, PageData.PageHeaderRect);
			}
		}
		public RectangleF BottomMarginBounds {
			get {
				return GetIntersectedBrickBounds(InnerBricks, PageData.PageFooterRect);
			}
		}
		public PSPage(ReadonlyPageData pageData, bool isAdditional) : this(pageData) {
			this.isAdditional = isAdditional;
		}
		public PSPage(ReadonlyPageData pageData)
			: base(pageData) {
		}
		public PSPage() {
		}
		void AddInnerBrick(BrickBase brick, RectangleF rect) {
			if(brick != null && rect.Height > 0 && rect.Width > 0) {
				brick.Rect = rect;
				InnerBricks.Insert(0, brick);
			}
		}
		void ModifyBricksSize(BrickBase brick) {
			bricksSize.Width = Math.Max(bricksSize.Width, brick.Rect.Right);
			bricksSize.Height = Math.Max(bricksSize.Height, brick.Rect.Bottom);
		}
		public void AddContent(IEnumerable<Brick> bricks) {
			if(!locked) {
				foreach(Brick brick in bricks) {
					ModifyBricksSize(brick);
					Bricks.Add(brick);
				}
			}
		}
		public void ClearContent() {
			if(!locked)
				Bricks.Clear();
		}
		public void LockContent() {
			locked = true;
		}
		internal int InsertAfter(Brick brick, Brick previous) {
			ModifyBricksSize(brick);
			return Bricks.InsertAfter(brick, previous);
		}
		internal void RemoveOuterBricks() {
			for (int i = 0; i < Bricks.Count;)
				if (Rect.IntersectsWith(Bricks[i].Rect))
					i++;
				else
					Bricks.RemoveAt(i);
		}
		public virtual void AfterCreate(RectangleF usefulPageArea, IServiceProvider servProvider) {
			SizeF size = Rect.Size;
			float error = usefulPageArea.Bottom - UsefulPageRectF.Bottom;
			if (error > 0)
				size.Height -= error;
			PointF offset = new PointF(-Rect.X, -Rect.Y);
			IList<Brick> innerBricks = Bricks;
			CompositeBrick contentBrick = CreateWrapperBrick(innerBricks ?? new List<Brick>(), offset);
			AddInnerBrick(contentBrick, new RectangleF(usefulPageArea.Location, size));
			if(this.NoClip)
				contentBrick.NoClip = true;
		}
		static RectangleF GetIntersectedBrickBounds(IList bricks, RectangleF bounds) {
			foreach(BrickBase brick in bricks)
				if(bounds.IntersectsWith(brick.Rect))
					return brick.Rect;
			return Rectangle.Empty;
		}
		public BrickBase AddBricks(ICollection bricks, RectangleF rect) {
			System.Diagnostics.Debug.Assert(rect.Top <= rect.Bottom && rect.Left <= rect.Right);
			if(bricks == null || bricks.Count == 0)
				return null;
			List<Brick> innerBricks = new List<Brick>(bricks.OfType<Brick>());
			BrickBase brick = CreateWrapperBrick(innerBricks, PointF.Empty);
			AddInnerBrick(brick, rect);
			return brick;
		}
		internal float GetTopMarginOffset() {
			if(TopMarginBounds.IsEmpty)
				return 0;
			float minBrickTop = MarginsF.Top;
			foreach(CompositeBrick brick in InnerBrickList)
				if(brick.Location.Y == 0)
					foreach(BrickBase innerBrick in brick.InnerBricks)
						minBrickTop = Math.Min(innerBrick.Y, minBrickTop);
			return MarginsF.Top - minBrickTop;
		}
		float GetBottomMarginOffset() {
			if(BottomMarginBounds.IsEmpty)
				return 0;
			float maxBrickBottom = 0;
			float startBottomMarginY = PageSize.Height - MarginsF.Bottom;
			foreach(CompositeBrick brick in InnerBrickList)
				if(brick.Y == startBottomMarginY)
					foreach(BrickBase innerBrick in brick.InnerBricks)
						maxBrickBottom = Math.Max(innerBrick.Y + innerBrick.Height, maxBrickBottom);
			return maxBrickBottom;
		}
		internal float GetClippedPageHeight() {
			return PageSize.Height - MarginsF.Top - MarginsF.Bottom + GetTopMarginOffset() + GetBottomMarginOffset();
		}
	}
}
