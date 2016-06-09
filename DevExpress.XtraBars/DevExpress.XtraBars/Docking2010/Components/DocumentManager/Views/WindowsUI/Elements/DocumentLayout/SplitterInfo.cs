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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISplitterInfo : IBaseSplitterInfo, IUIElement {
		void SetGroupInfo(ISplitGroupInfo info);
		int CalcMinSize(Graphics g);
		ObjectState State { get; set; }
		int SplitIndex { get; set; }
		void MoveSplitter(int change);
		void BeginSplit(Adorner adorner);
		void UpdateSplit(Adorner adorner, int change);
		void ResetSplit(Adorner adorner);
	}
	class SplitterInfo : BaseElementInfo, ISplitterInfo {
		ObjectPainter Painter;
		SplitterInfoArgs Info;
		int splitIndexCore = -1;
		public SplitterInfo(WindowsUIView view)
			: base(view) {
			Init(view);
		}
		public override System.Type GetUIElementKey() {
			return typeof(ISplitterInfo);
		}
		protected override void UpdateStyleCore() {
			Init(Owner as WindowsUIView);
		}
		protected override void ResetStyleCore() {
			Reset();
		}
		void Init(WindowsUIView view) {
			Painter = SplitterHelper.GetPainter(view.ElementsLookAndFeel);
			Info = new SplitterInfoArgs(false);
		}
		void Reset() {
			Painter = null;
			Info = null;
		}
		public ISplitGroupInfo GroupInfo {
			get { return groupInfoCore; }
		}
		ObjectState stateCore;
		public ObjectState State {
			get { return stateCore; }
			set {
				if(stateCore == value) return;
				stateCore = value;
				OnStateChanged(value);
			}
		}
		protected virtual void OnStateChanged(ObjectState value) {
			CheckCursor(value);
			Owner.Invalidate(Bounds);
		}
		protected void CheckCursor(ObjectState value) {
			Cursor cursor = null;
			if((value & (ObjectState.Hot | ObjectState.Pressed)) != 0)
				cursor = IsHorizontal ? Cursors.SizeWE : Cursors.SizeNS;
			Owner.SetCursor(cursor);
		}
		public bool IsHorizontal {
			get { return GroupInfo != null && GroupInfo.Group.IsHorizontal; }
		}
		public int SplitIndex {
			get { return splitIndexCore; }
			set { splitIndexCore = value; }
		}
		int splitLength1, splitLength2;
		public int SplitLength1 {
			get { return splitLength1; }
			set { splitLength1 = value; }
		}
		public int SplitLength2 {
			get { return splitLength2; }
			set { splitLength2 = value; }
		}
		public int SplitConstraint1 {
			get {
				if(SplitIndex != -1) {
					Document document1 = GroupInfo.Group.Items[SplitIndex];
					if(document1 != null)
						return IsHorizontal ? document1.Form.MinimumSize.Width :
							document1.Form.MinimumSize.Height;
				}
				return IsHorizontal ? 50 : 75;
			}
		}
		public int SplitConstraint2 {
			get {
				if(SplitIndex != -1) {
					Document document2 = GroupInfo.Group.Items[SplitIndex + 1];
					if(document2 != null)
						return IsHorizontal ? document2.Form.MinimumSize.Width :
							document2.Form.MinimumSize.Height;
				}
				return IsHorizontal ? 50 : 75;
			}
		}
		public void MoveSplitter(int change) {
			if(change != 0 && SplitIndex != -1) {
				using(BatchUpdate.Enter(Owner.Manager)) {
					GroupInfo.Group[SplitIndex] = SplitLength1 + change;
					GroupInfo.Group[SplitIndex + 1] = SplitLength2 - change;
					Owner.Manager.InvokePatchActiveChildren();
				}
			}
		}
		public int CalcMinSize(Graphics g) {
			Info.IsVertical = IsHorizontal;
			Size minSize = ObjectPainter.CalcObjectMinBounds(g, Painter, Info).Size;
			return Info.IsVertical ? minSize.Width : minSize.Height;
		}
		protected void CalcObjectMinBounds(Graphics g) {
			Info.Graphics = g;
			Rectangle r = Painter.CalcObjectMinBounds(Info);
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			Info.IsVertical = IsHorizontal;
			Info.Bounds = bounds;
		}
		protected override void DrawCore(GraphicsCache cache) {
			Info.Cache = cache;
			Info.State = State;
			Painter.DrawObject(Info); 
			Info.Cache = null;
		}
		AdornerElementInfo AdornerInfo;
		public void BeginSplit(Adorner adorner) {
			SplitAdornerInfoArgs splitArgs = new SplitAdornerInfoArgs();
			splitArgs.Bounds = Bounds;
			AdornerInfo = new AdornerElementInfo(new SplitAdornerPainter(), splitArgs);
			adorner.Show(AdornerInfo);
		}
		public void UpdateSplit(Adorner adorner, int change) {
			Point origin = Bounds.Location;
			origin.Offset(IsHorizontal ? new Point(change, 0) : new Point(0, change));
			SplitAdornerInfoArgs splitArgs = AdornerInfo.InfoArgs as SplitAdornerInfoArgs;
			splitArgs.Bounds = new Rectangle(origin, Bounds.Size);
			adorner.Invalidate();
		}
		public void ResetSplit(Adorner adorner) {
			State = ObjectState.Normal;
			adorner.Reset(AdornerInfo);
			AdornerInfo = null;
		}
		#region ISplitterInfo Members
		ISplitGroupInfo groupInfoCore;
		void ISplitterInfo.SetGroupInfo(ISplitGroupInfo info) {
			if(groupInfoCore == info) return;
			LayoutHelper.Unregister(this);
			groupInfoCore = info;
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren IUIElement.Children { get { return null; } }
		#endregion IUIElement
		public BaseDocument[] GetDocuments() {
			Document document1 = GroupInfo.Group.Items[SplitIndex];
			Document document2 = GroupInfo.Group.Items[SplitIndex + 1];
			return new BaseDocument[] { document1, document2 };
		}		
	}
}
