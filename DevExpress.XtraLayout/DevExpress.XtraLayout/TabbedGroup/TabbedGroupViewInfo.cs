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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Tab;
using DevExpress.XtraTab;
namespace DevExpress.XtraLayout.ViewInfo {
	public abstract class BaseTabbedViewInfo : BaseLayoutItemViewInfo {
		protected BaseTabbedViewInfo(TabbedGroup owner) : base(owner) { }
		public new TabbedGroup Owner {
			get { return base.Owner as TabbedGroup; }
		}
	}
	public class TabbedGroupViewInfo : BaseTabbedViewInfo {
		Locations textLocation;
		public Locations TextLocation {
			get { return textLocation; }
			set { textLocation = value; }
		}
		public TabbedGroupViewInfo(TabbedGroup owner, Locations textLocation)
			: base(owner) {
			this.textLocation = textLocation;
			var tabInfo = GetTabObjectInfo();
			tabInfo.Tab.SelectedPageChanged += Owner.OnSelectedTabChanged;
			tabInfo.Tab.CloseButtonClick += Owner.OnCloseButtonClick;
		}
		protected internal override void Destroy() {
			base.Destroy();
			var tabInfo = GetTabObjectInfo();
			tabInfo.Tab.SelectedPageChanged -= Owner.OnSelectedTabChanged;
			tabInfo.Tab.CloseButtonClick -= Owner.OnCloseButtonClick;
			tabInfo.Tab.Dispose();
		}
		public override object Clone() {
			TabbedGroupViewInfo cloneInfo = (TabbedGroupViewInfo)base.Clone();
			cloneInfo.textLocation = this.textLocation;
			return cloneInfo;
		}
		protected override ObjectInfoArgs CreateBorderInfo() {
			return new TabObjectInfo(Owner);
		}
		protected TabObjectInfo GetTabObjectInfo() {
			return base.BorderInfo as TabObjectInfo;
		}
		public new TabObjectInfo BorderInfo {
			get {
				CalculateViewInfoIfNeeded();
				return GetTabObjectInfo();
			}
		}
		protected override void UpdateBorderInfo(ObjectInfoArgs borderInfo) {
			TabObjectInfo vi = borderInfo as TabObjectInfo;
			vi.Tab.ViewInfo.BeginUpdate();
			vi.Tab.SetHeaderLocationAndOrientation(textLocation, Owner.HeaderOrientation);
			vi.Bounds = CalculateObjectBounds(BoundsRelativeToControl);
			vi.Tab.Bounds = CalculateObjectBounds(BoundsRelativeToControl);
			vi.Tab.Populate();
			vi.Tab.LayoutChanged();
			vi.Tab.ViewInfo.EndUpdate();
			vi.Tab.ViewInfo.CheckFirstPageIndex();
		}
		public Rectangle GetScreenTabFocusRect(int index) {
			CalculateViewInfoIfNeeded();
			if(Owner.TabPages.Count < index || Owner.TabPages.Count == 0) return Rectangle.Empty;
			Rectangle rect = BorderInfo.Tab.ViewInfo.HeaderInfo.AllPages[index].Text;
			rect.Inflate(1, 1);
			switch(Owner.TextLocation) {
				case Locations.Default:
				case Locations.Top:
				case Locations.Bottom:
					rect.X--; break;
				case Locations.Left:
				case Locations.Right:
					rect.Y++;
					break;
			}
			if(this.BorderInfo != null && this.BorderInfo.Tab != null && ((IXtraTab)this.BorderInfo.Tab).HeaderOrientation == DevExpress.XtraTab.TabOrientation.Vertical) {
				if(Owner.TextLocation == Locations.Left || Owner.TextLocation == Locations.Top || Owner.TextLocation == Locations.Default) {
					rect.X++;
					rect.Y++;
				}
				if(Owner.TextLocation == Locations.Right || Owner.TextLocation == Locations.Bottom) {
					rect.X--;
					rect.Y--;
				}
			}
			Rectangle buttonsBounds = BorderInfo.Tab.ViewInfo.HeaderInfo.ButtonsBounds;
			if(rect.IntersectsWith(buttonsBounds)) {
				if(rect.Left > buttonsBounds.Left) 
					rect = Rectangle.Empty;
				else
					rect.Width = buttonsBounds.Left - rect.Left;
			}
			return rect;
		}
		public bool IsLastRow(int index) {
			CalculateViewInfoIfNeeded();
			XtraTab.ViewInfo.BaseTabHeaderViewInfo headerInfo = BorderInfo.Tab.ViewInfo.HeaderInfo;
			index -= BorderInfo.Tab.ViewInfo.FirstVisiblePageIndex;
			if(index < 0 || headerInfo.VisiblePages.Count <= index)
				return false;
			return (headerInfo.Rows.Count - 1) == headerInfo.Rows.IndexOf(headerInfo.VisiblePages[index].Row);
		}
		public Rectangle GetScreenTabCaptionRect(int index) {
			CalculateViewInfoIfNeeded();
			XtraTab.ViewInfo.BaseTabHeaderViewInfo headerInfo = BorderInfo.Tab.ViewInfo.HeaderInfo;
			index -= BorderInfo.Tab.ViewInfo.FirstVisiblePageIndex;
			if(index < 0 || headerInfo.VisiblePages.Count <= index)
				return Rectangle.Empty;
			return headerInfo.VisiblePages[index].Bounds;
		}
		protected internal override void UpdateBorder() {
			UpdateBorderInfo(BorderInfo);
			PaintStyle.GetPainter(this.Owner).GetBorderPainter(this).CalcObjectBounds(BorderInfo);
			ShouldUpdateBorder = false;
		}
		protected override Rectangle GetPainterRect() {
			Rectangle source = measureRect;
			return BorderInfo.Tab.CalcPageClient(source);
		}
		protected override void CalculatePaddings() {
			if(ShouldUpdateBorder) UpdateBorder();
			bool fMultiLine = BorderInfo.Tab.ViewInfo.IsMultiLine;
			if(cashedPaddingRect.IsEmpty || fMultiLine) {
				if(fMultiLine) {
					switch(TextLocation) {
						case Locations.Top:
						case Locations.Bottom:
						case Locations.Default:
							measureRect = new Rectangle(0, 0, Owner.Bounds.Width - Spaces.Width, 1000);
							break;
						case Locations.Left:
						case Locations.Right:
							measureRect = new Rectangle(0, 0, 1000, Owner.Bounds.Height - Spaces.Height);
							break;
					}
				}
				cashedPaddingRect = BorderInfo.Tab.CalcPageClient(measureRect);
			}
			Padding = new DevExpress.XtraLayout.Utils.Padding(Owner.Padding.Left + Spaces.Left + cashedPaddingRect.Left,
				Owner.Padding.Right + Spaces.Right + measureRect.Right - cashedPaddingRect.Right,
				Owner.Padding.Top + Spaces.Top + cashedPaddingRect.Top,
				Owner.Padding.Bottom + Spaces.Bottom + measureRect.Bottom - cashedPaddingRect.Bottom);
		}
		protected override Size AddLabel(Size size) {
			return size;
		}
		protected override void CalculateRegions() {
			Point leftTop = new Point(Padding.Left, Padding.Top);
			Size innerSize = new Size(Owner.Bounds.Size.Width - Padding.Left - Padding.Right,
				Owner.Bounds.Size.Height - Padding.Top - Padding.Bottom);
			ClientArea = new Rectangle(leftTop, innerSize);
		}
		public Rectangle RealClientArea {
			get {
				Rectangle clientArea = BoundsRelativeToControl;
				clientArea.X += Padding.Left;
				clientArea.Y += Padding.Top;
				clientArea.Y += TabsCaptionArea.Height;
				clientArea.Width -= Padding.Width;
				clientArea.Height -= Padding.Height;
				return clientArea;
			}
		}
		public Rectangle TabsCaptionArea {
			get {
				CalculateViewInfoIfNeeded();
				Rectangle rect = BoundsRelativeToControl;
				int temp;
				DevExpress.XtraLayout.Utils.Padding realPadding = Padding;
				switch(Owner.TextLocation) {
					case Locations.Left:
						rect.Width = realPadding.Left - Owner.Padding.Left;
						break;
					case Locations.Right:
						temp = realPadding.Right - Owner.Padding.Right;
						rect.X = rect.Right - temp;
						rect.Width = temp;
						break;
					case Locations.Top:
					case Locations.Default:
						rect.Height = realPadding.Top - Owner.Padding.Top;
						break;
					case Locations.Bottom:
						temp = realPadding.Bottom - Owner.Padding.Bottom;
						rect.Y = rect.Bottom - temp;
						rect.Height = temp;
						break;
				}
				return rect;
			}
		}
	}
}
