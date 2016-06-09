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
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Drawing;
using System.Collections;
namespace DevExpress.XtraPrinting.Native.LayoutAdjustment {
	public class BrickLayoutDataBase : ILayoutData {
		protected VisualBrick fBrick;
		RectangleF initialRect;
		protected float dpi;
		VerticalAnchorStyles ILayoutData.AnchorVertical { get { return fBrick.BrickOwner.AnchorVertical;  } }
		public virtual bool NeedAdjust { get { return fBrick.NeedAdjust; } }
		public virtual List<ILayoutData> ChildrenData { get { return null; } }
		protected int ChildrenDataCount { get { return ChildrenData != null ? ChildrenData.Count : 0; } }
		public float Top { get { return GetBrickBounds().Top; } }
		public float Bottom { get { return GetBrickBounds().Bottom; } }
		RectangleF ILayoutData.InitialRect { get { return initialRect; } }
		public BrickLayoutDataBase(VisualBrick brick, float dpi) {
			this.dpi = dpi;
			this.fBrick = brick;
			initialRect = GetBrickBounds();
		}
		protected RectangleF GetBrickBounds() {
			return VisualBrickHelper.GetBrickBounds(fBrick, dpi);
		}
		public virtual void UpdateViewBounds() {
			fBrick.BrickOwner.UpdateBrickBounds(fBrick);
		}
		public virtual void SetBoundsY(float y) {
			VisualBrickHelper.SetBrickBoundsY(fBrick, y, dpi);
		}
		void ILayoutData.Anchor(float delta, float dpi) {
			if (delta == 0)
				return;
			VisualBrick brick = this.fBrick;
			if (((ILayoutData)this).AnchorVertical == VerticalAnchorStyles.Both) {
				float height = this.Bottom - this.Top + delta;
				VisualBrickHelper.SetBrickBoundsHeight(this.fBrick, height, dpi);
			} else if (((ILayoutData)this).AnchorVertical == VerticalAnchorStyles.Bottom) {
				RectangleF bounds = VisualBrickHelper.GetBrickBounds(this.fBrick, dpi);
				bounds.Offset(0, delta);
				VisualBrickHelper.SetBrickBounds(this.fBrick, bounds, dpi);
			}
			float selfDelta = Math.Max((Bottom - ((ILayoutData)this).InitialRect.Bottom) - (Top - ((ILayoutData)this).InitialRect.Top), 0);
			AnchorChildren(selfDelta, dpi);
		}
		protected virtual void AnchorChildren(float delta, float dpi) {
			if(ChildrenDataCount > 0)
				foreach(ILayoutData item in this.ChildrenData)
					item.Anchor(delta, dpi);
		}
	}
	public class VisualBrickLayoutData : BrickLayoutDataBase {
		#region static
		static float GetInitialHeight(ILayoutData layoutData) {
			return layoutData.InitialRect.Height;
		}
		static bool CanShrinkBothAnchored(ICollection layoutDatas) {
			foreach(ILayoutData layoutData in layoutDatas)
				if(layoutData.AnchorVertical != VerticalAnchorStyles.Both)
					return true;
			return false;
		}
		#endregion
		List<ILayoutData> childrenData;
		public VisualBrick Brick { 
			get { return fBrick; } 
		}
		public override bool NeedAdjust {
			get {
				if(Brick.SafeGetAttachedValue<bool>(BrickAttachedProperties.SummaryInProgress, false))
					return false;
				if(base.NeedAdjust)
					return true;
				foreach(ILayoutData data in childrenData)
					if(data.NeedAdjust)
						return true;
				return false;
			}
		}
		public override List<ILayoutData> ChildrenData { get { return childrenData; } }
		public VisualBrickLayoutData(VisualBrick brick, float dpi) : base(brick, dpi) {
			childrenData = new List<ILayoutData>();
			foreach(VisualBrick item in brick.Bricks)
				AddToChildrenData(item, childrenData, dpi);
		}
		public override void UpdateViewBounds() {
			if(fBrick.BrickOwner.NeedCalcContainerHeight) {
				VisualBrickHelper.SetBrickBoundsHeight(fBrick, CalculateContainerHeight((fBrick is PanelBrick || fBrick is DevExpress.XtraPrinting.NativeBricks.SeparableBrick) && fBrick.CanShrink, fBrick.CanGrow), dpi);
			} else
				base.UpdateViewBounds();
		}
		protected virtual void AddToChildrenData(VisualBrick item, List<ILayoutData> childrenData, float dpi) {
			childrenData.Add(item.CreateLayoutData(dpi));
		}
		float CalculateContainerHeight(bool isShrinkablePanel, bool panelCanGrow) {
			float childrenDataBottom = 0;
			float childrenDataInitialBottom = 0;
			foreach(ILayoutData data in childrenData)
				if(data.AnchorVertical == VerticalAnchorStyles.None) {
					childrenDataBottom = Math.Max(childrenDataBottom, data.Bottom);
					childrenDataInitialBottom = Math.Max(childrenDataInitialBottom, data.InitialRect.Bottom);
				}
			float height = childrenDataBottom + Math.Max(GetInitialHeight(this) - childrenDataInitialBottom, 0);
			return isShrinkablePanel ? panelCanGrow ? childrenDataBottom : Math.Min(childrenDataBottom, GetBrickBounds().Height) : Math.Max(height, GetBrickBounds().Height);
		}
	}
	public class TableCellLayoutData : VisualBrickLayoutData {
		public TableCellLayoutData(VisualBrick brick, float dpi) : base(brick, dpi) { }
		public override bool NeedAdjust { get { return this.fBrick.NeedAdjust; } }
	}
	public class TableBrickLayoutData : VisualBrickLayoutData {
		public TableBrickLayoutData(VisualBrick brick, float dpi) : base(brick, dpi) { }
		protected override void AnchorChildren(float delta, float dpi) {
			if(delta == 0 || ChildrenDataCount == 0)
				return;
			float heightDelta = delta / ChildrenDataCount;
			for(int i = ChildrenDataCount - 1; i > -1; i--) {
				ILayoutData item = this.ChildrenData[i];
				if ((((ILayoutData)this).AnchorVertical & VerticalAnchorStyles.Bottom) != 0) {
					VisualBrick rowBrick = ((VisualBrickLayoutData)item).Brick;
					RectangleF bounds = VisualBrickHelper.GetBrickBounds(rowBrick, dpi);
					bounds.Height = item.Bottom - item.Top + heightDelta;
					bounds.Offset(0, heightDelta * (ChildrenDataCount - 1 - i));
					VisualBrickHelper.SetBrickBounds(rowBrick, bounds, dpi);
				}
				item.Anchor(heightDelta, dpi);
			}
		}
		protected override void AddToChildrenData(VisualBrick item, List<ILayoutData> childrenData, float dpi) {
			childrenData.Add(new TableRowLayoutData(item, dpi));
		}
	}
	public class TableRowLayoutData : VisualBrickLayoutData {
		public TableRowLayoutData(VisualBrick brick, float dpi) : base(brick, dpi) { }
		protected override void AddToChildrenData(VisualBrick item, List<ILayoutData> childrenData, float dpi) {
			childrenData.Add(new TableCellLayoutData(item, dpi));
		}
	}
	public class LayoutDataContainer : ILayoutData {
		List<ILayoutData> childrenData;
		float initialBottom;
		float bottomSpan;
		float bottom;
		public LayoutDataContainer(List<ILayoutData> childrenData, float bottomSpan) {
			this.childrenData = childrenData;
			this.bottomSpan = bottomSpan;
			UpdateViewBounds();
			initialBottom = bottom;
		}
		public LayoutDataContainer(List<ILayoutData> childrenData)
			: this(childrenData, 0) {
		}
		RectangleF ILayoutData.InitialRect { get { return RectangleF.FromLTRB(0, Top, 0, initialBottom); } }
		float CalculateContainerHeight() {
			float childrenDataInitialBottom = 0;
			foreach (ILayoutData data in childrenData)
				childrenDataInitialBottom = Math.Max(childrenDataInitialBottom, data.Bottom);
			return childrenDataInitialBottom;
		}
		float CalcBottom() {
			return CalculateContainerHeight() + bottomSpan;
		}
		#region ILayoutData Members
		public VerticalAnchorStyles AnchorVertical {
			get { return VerticalAnchorStyles.None; }
		}
		public bool NeedAdjust {
			get { return true; }
		}
		public float Top {
			get { return 0; }
		}
		public float Bottom {
			get { return bottom; }
		}
		public List<ILayoutData> ChildrenData {
			get { return childrenData; }
		}
		public void SetBoundsY(float y) { }
		public void UpdateViewBounds() {
			bottom = CalcBottom();
		}
		public void Anchor(float delta, float dpi) {
			if(ChildrenData != null && ChildrenData.Count > 0)
				foreach(ILayoutData item in ChildrenData) {
					float newDelta = delta;
					if(this.Bottom == this.initialBottom && item.Bottom != item.InitialRect.Bottom) 
						newDelta = item.Bottom - item.InitialRect.Bottom;
					item.Anchor(newDelta, dpi);
				}
		}
		#endregion
	}
}
