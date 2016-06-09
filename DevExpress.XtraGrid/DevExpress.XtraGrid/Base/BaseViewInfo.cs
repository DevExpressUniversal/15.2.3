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
using System.ComponentModel;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Collections;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Tab;
using DevExpress.Utils.Drawing.Animation;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using System.Drawing.Imaging;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data.Details;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.Skins;
namespace DevExpress.XtraGrid.Views.Base.ViewInfo {
	[ListBindable(false)]
	public class IndentInfoCollection : CollectionBase {
		public int Add(IndentInfo info) {
			return List.Add(info);
		}
		public IndentInfo this[int index] { get { return List[index] as IndentInfo; } }
		public virtual void OffsetContent(int x, int y) {
			foreach(IndentInfo info in this) info.OffsetContent(x, y);
		}
	}
	public class CellId { 
		GridColumn column;
		object row;
		public CellId(object row, GridColumn column) {
			this.column = column;
			this.row = row;
		}
		public GridColumn Column { get { return column; } }
		public object Row { get { return row; } }
		public override bool Equals(object obj)
		{
			CellId id = obj as CellId;
			if (id == null) return base.Equals(obj);
			return Column == id.Column && Row == id.Row;
		}
		public override int GetHashCode()
		{
			int h1 = Column == null? 0: Column.GetHashCode();
			int h2 = Row == null? 0: Row.GetHashCode();
			return h1 ^ h2;
		}
	}
	public class IndentInfo {
		Rectangle bounds;
		public object IndentOwner;
		public AppearanceObject Appearance;
		public bool IsGroupStyle;
		public IndentInfo(object indentOwner, Rectangle bounds, AppearanceObject appearance, bool isGroupStyle) {
			this.IndentOwner = indentOwner;
			this.bounds = bounds;
			this.Appearance = appearance;
			this.IsGroupStyle = isGroupStyle;
		}
		public Rectangle Bounds { get { return bounds; } }
		public IndentInfo(object indentOwner, Rectangle bounds, AppearanceObject appearance) : this(indentOwner, bounds, appearance, false) { }
		public virtual void OffsetContent(int x, int y) {
			this.bounds.Offset(x, y);
		}
	}
	public abstract class BaseViewInfo : ISupportXtraAnimation {
		BaseSelectionInfo _selectionInfo;
		protected int fInRealHeightCalc;
		bool isReady, isDataDirty;
		BaseView view;
		GraphicsInfo gInfo;
		BorderPainter _borderPainter;
		BaseAppearanceCollection paintAppearance;
		bool paintAppearanceDirty = true;
		protected internal Rectangle DragFrameRect = Rectangle.Empty;
		public BaseViewInfo(BaseView view) {
			this.isDataDirty = false;
			this.fInRealHeightCalc = 0;
			this.gInfo = new GraphicsInfo();
			this.view = view;
			this.isReady = false;
			this._selectionInfo = CreateSelectionInfo();
			this.paintAppearance = CreatePaintAppearances();
			UpdatePainters();
		}
		static Point emptyPoint = new Point(-10000, -10000);
		public static Point EmptyPoint { get { return emptyPoint; } }
		protected bool IsPrinting { get { return View.IsPrintingOnly || View.IsPrinting; } }
		public abstract Rectangle Bounds { get; }
		public abstract Rectangle ClientBounds { get; }
		protected virtual Size CalcObjectSize(ObjectPainter painter, ObjectInfoArgs info) {
			Size res = Size.Empty;
			GInfo.AddGraphics(null);
			try {
				res = ObjectPainter.CalcObjectMinBounds(GInfo.Graphics, painter, info).Size;
			} finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
#region Animation
		Control ISupportXtraAnimation.OwnerControl { get { return View.GridControl; } }
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		bool paintAnimatedItems = true;
		ArrayList removeItems;
		public bool PaintAnimatedItems { get { return paintAnimatedItems; } set { paintAnimatedItems = value; } }
		protected virtual void StartAnimation(object animId, BaseEditViewInfo info) {
			AddAnimatedItem(animId, info);
		}
		protected virtual void StopAnimation(object animId, BaseEditViewInfo info) {
			XtraAnimator.RemoveObject(this, animId);
			IAnimatedItem item = info as IAnimatedItem;
			if(item != null) item.OnStop();
		}
		protected abstract BaseEditViewInfo HasItem(CellId id);
		protected virtual bool ShouldAddItem(BaseEditViewInfo vi, CellId id) {
			IAnimatedItem item = vi as IAnimatedItem;
			EditorAnimationInfo info = XtraAnimator.Current.Get(this, id) as EditorAnimationInfo;
			if(item == null || item.FramesCount < 2)  { 
				if(info == null) return false;
				XtraAnimator.Current.Animations.Remove(info);
				return false;
			}
			if(info != null) {
				if(info.ViewInfo == vi) return false;
				info.ViewInfo = vi;
				item.OnStart();
				return false;
			}
			return true;
		}
		protected virtual EditorAnimationInfo GetAnimation(CellId id) {
			if(id == null) return null;
			return XtraAnimator.Current.Get(this, id) as EditorAnimationInfo;
		}
		protected ArrayList RemoveItems { 
			get {
				if(removeItems == null) removeItems = new ArrayList();
				return removeItems; 
			} 
		}
		protected virtual void UpdateAnimatedItem(EditorAnimationInfo editInfo, BaseEditViewInfo newInfo) {
			editInfo.ViewInfo = newInfo;
			if(ShouldStopAnimation(newInfo as IAnimatedItem))
				((IAnimatedItem)newInfo).OnStop();
			else
				((IAnimatedItem)newInfo).OnStart();
		}
		protected internal virtual void ClearAnimatedItems() {
			XtraAnimator.RemoveObject(this);
		}
		protected virtual void RemoveInvisibleItems() {
			EditorAnimationInfo editInfo;
			foreach(BaseAnimationInfo anim in XtraAnimator.Current.Animations) {
				editInfo = anim as EditorAnimationInfo;
				if(editInfo == null) continue;
				BaseEditViewInfo vi = editInfo.ViewInfo as BaseEditViewInfo;
				if(vi == null || vi.EditValue == null) continue;
				BaseEditViewInfo foundInfo = HasItem(editInfo.AnimationId as CellId);
				if(foundInfo == null) RemoveItems.Add(anim.AnimationId);
				else UpdateAnimatedItem(editInfo, foundInfo);
			}
			while(RemoveItems.Count > 0) {
				IAnimatedItem item = RemoveItems[0] as IAnimatedItem;
				if(item != null) item.OnStop();
				XtraAnimator.RemoveObject(this, RemoveItems[0]);
				RemoveItems.RemoveAt(0);
			}
		}
		protected virtual void AddAnimatedItem(object animId, BaseEditViewInfo info) {
			IAnimatedItem item = info as IAnimatedItem;
			XtraAnimator.Current.AddEditorAnimation(animId, info, this, item, new CustomAnimationInvoker(OnAnimation));
			if(ShouldStopAnimation(item)) 
				item.OnStop();
			else 
				item.OnStart();
		}
		protected internal virtual void UpdateAnimatedItems() {
			RemoveInvisibleItems();
			AddAnimatedItems();
		}
		protected abstract void AddAnimatedItems();
		protected virtual void InvalidateAnimatedBounds(IAnimatedItem animItem) {
			View.InvalidateRect(animItem.AnimationBounds);
		}
		protected virtual bool ShouldStopAnimation(IAnimatedItem item) { return false; }
		protected virtual void OnAnimation(BaseAnimationInfo animInfo) {
			EditorAnimationInfo info = animInfo as EditorAnimationInfo;
			if(info == null || View.GridControl == null) return;
			IAnimatedItem animItem = info.ViewInfo as IAnimatedItem;
			if(animItem == null) return;
			if(ShouldStopAnimation(animItem)) {
				StopAnimation(animInfo.AnimationId, info.ViewInfo as BaseEditViewInfo);
				info.CurrentFrame = 0;
				animItem.UpdateAnimation(info);
				InvalidateAnimatedBounds(animItem);
				return;
			}
			if(!info.IsFinalFrame) {
				if(animItem.Owner == null) {
					animItem.UpdateAnimation(info);
					InvalidateAnimatedBounds(animItem);
				}	
			}
			else {
				StopAnimation(animInfo.AnimationId, info.ViewInfo as BaseEditViewInfo);
				StartAnimation(animInfo.AnimationId, info.ViewInfo as BaseEditViewInfo);
			}
		}
#endregion
		public virtual void SetPaintAppearanceDirty() { this.paintAppearanceDirty = true; }
		public BaseAppearanceCollection PaintAppearance { 
			get { 
				if(this.paintAppearanceDirty) UpdatePaintAppearance();
				return paintAppearance; 
			} 
		}
		public virtual bool IsRightToLeft { get { return View.IsRightToLeft; } }
		protected virtual void UpdatePaintAppearance() {
			if(View == null || View.PaintStyle == null) return;
			this.paintAppearance.Combine(View.Appearance, View.PaintStyle.GetAppearanceDefaultInfo(View), true);
			this.paintAppearance.UpdateRightToLeft(IsRightToLeft);
			this.paintAppearanceDirty = false;
			UpdateAlphaBlending();
			UpdatePaintAppearanceDefaults();
		}
		protected virtual void UpdatePaintAppearanceDefaults() { }
		protected virtual void UpdateAlphaBlending() {
			if(GridControl == null || GridControl.IsDesignMode || GridControl.OldAlphaBlending == null || !GridControl.OldAlphaBlending.Enabled) return;
			foreach(DictionaryEntry entry in GridControl.OldAlphaBlending.AlphaStyles) {
				AppearanceObject app = PaintAppearance.GetAppearance(entry.Key.ToString());
				if(app == null || app.BackColor.IsEmpty) continue;
				int level = (int)entry.Value;
				if(level >= 255) continue;
				app.BackColor = Color.FromArgb(level, app.BackColor);
				if(app.BackColor2 != Color.Empty) app.BackColor2 = Color.FromArgb(level, app.BackColor2);
			}
		}
		protected abstract BaseAppearanceCollection CreatePaintAppearances();
		protected virtual void UpdatePainters() {
			this._borderPainter = null;
			BorderStyles bs = View.BorderStyle;
			if(bs == BorderStyles.Default && View.WorkAsLookup) bs = BorderStyles.NoBorder;
			bs = CheckBorderStyle(bs);
			if(View.PaintStyle != null) this._borderPainter = View.PaintStyle.GetBorderPainter(View, bs);
			if(this._borderPainter == null) {
				this._borderPainter = BorderHelper.GetPainter(bs, View.PaintStyle == null ? null : View.ElementsLookAndFeel);
				if(this._borderPainter is UltraFlatBorderPainter) this._borderPainter = new SimpleBorderPainter();
			}
		}
		BorderStyles CheckBorderStyle(BorderStyles bs) {
			if(bs != BorderStyles.Default || !View.IsSkinned) return bs;
			if(GridControl == null) return bs;
			if(GridControl.Parent != null && GridControl.Parent.Dock != DockStyle.None) {
				if(GridControl.Parent is GroupControl) return BorderStyles.NoBorder;
				if(GridControl.Parent is XtraTabPage) return BorderStyles.NoBorder;
			}
			return bs;
		}
		public virtual BorderPainter BorderPainter { get { return _borderPainter; } }
		protected abstract BaseSelectionInfo CreateSelectionInfo();
		public BaseSelectionInfo SelectionInfo { get { return _selectionInfo; } }
		protected void StartRealHeightCalculate() {
			LockSelectionInfo();
			fInRealHeightCalc ++;
		}
		protected void EndRealHeightCalculate() {
			UnlockSelectionInfo();
			fInRealHeightCalc --;
		}
		protected internal virtual Rectangle GetTargetDragRect(int baseHeight) { return Rectangle.Empty; }
		protected internal bool IsRealHeightCalculate { get { return fInRealHeightCalc != 0; } }
		protected virtual Rectangle CalcTabClientRect(Rectangle bounds) {
			if(!ShowTabControl || TabControl == null) return bounds;
			Rectangle r = TabControl.CalcPageClient(bounds);
			return r;
		}
		protected virtual bool CheckMasterTabHitTest(BaseHitInfo hi, Point pt) {
			GridDetailInfo info = CalcTabControlHeaderHitInfo(pt);
			if(info != null) {
				hi.MasterTabRelationIndex = info.RelationIndex;
				return true;
			}
			return false;
		}
		protected virtual GridDetailInfo CalcTabControlHeaderHitInfo(Point pt) {
			if(!ShowTabControl || TabControl == null) return null;
			BaseTabHitInfo hitInfo = TabControl.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.HitTest == XtraTabHitTest.PageHeader) {
				return (hitInfo.Page as ViewTabPage).DetailInfo;
			}
			return null;
		}
		protected virtual void UpdateTabControl() {
			if(!ShowTabControl) {
				TabControl.Bounds = Rectangle.Empty;
				return;
			} 
		}
		protected virtual Rectangle CalcBorderRect(Rectangle bounds) {
			BorderObjectInfoArgs info = CreateBorderInfo(bounds);
			return BorderPainter.GetObjectClientRectangle(info);
		}
		protected internal virtual BorderObjectInfoArgs CreateBorderInfo(Rectangle bounds) {
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(GInfo.Cache, bounds, null);
			info.State = View.Handler.MouseHere ? ObjectState.Hot : ObjectState.Normal;
			return info;
		}
		public virtual bool AllowTabControl { get { return true; } }
		public virtual bool ShowTabControl { get { return View.ParentView != null && View.ParentView.ViewInfo.AllowTabControl && TabControl != null && TabControl.Pages.Count > 0;
													   ; } }
		public virtual ViewTab TabControl { get { return View.TabControl; } }
		public virtual GraphicsInfo GInfo { get { return gInfo; } }
		public virtual BaseView View { get { return view; } }
		public virtual GridControl GridControl { get { return View == null ? null : View.GridControl; } }
		public Rectangle CheckBounds(Rectangle bounds, Rectangle content) {
			if(bounds.X < content.Left) {
				bounds.Width -= (content.Left - bounds.X);
				bounds.X = content.Left;
			}
			if(bounds.Y < content.Top) {
				bounds.Height -= (content.Top - bounds.Y);
				bounds.Y = content.Top;
			}
			if(bounds.Right > content.Right) {
				bounds.Width = content.Right - bounds.X;
			}
			if(bounds.Bottom > content.Bottom) {
				bounds.Height = content.Bottom - bounds.Y;
			}
			return bounds;
		}
		public virtual int CalcRealViewHeight(Rectangle viewRect) {
			return 0;
		}
		public virtual void PrepareCalcRealViewHeight(Rectangle viewRect, BaseViewInfo oldViewInfo) {
		}
		public virtual bool IsDataDirty { 
			get { return isDataDirty; } 
			set {
				isDataDirty = value;
			}
		}
		public void SetDataDirty() { 
			this.isDataDirty = true; 
		} 
		public virtual bool IsReady { 
			get { return isReady && !IsNull; } 
			set { isReady = value; }
		}
		public virtual bool IsNull { get { return false; } }
		protected virtual void CalcViewInfo() {
			IsReady = true;
			IsDataDirty = false;
			UpdatePainters();
		}
		int lockSelectionInfo = 0;
		protected void LockSelectionInfo() { lockSelectionInfo ++; }
		protected void UnlockSelectionInfo() { lockSelectionInfo --; }
		protected bool IsSelectionInfoLocked { get { return lockSelectionInfo != 0; } }
		public virtual void Clear() {
			IsReady = false;
			if(!IsSelectionInfoLocked)
				SelectionInfo.Clear();
		}
		protected virtual void FillRepositoryHashtable(RepositoryItemCollection collection, Hashtable hash) {
			if(collection == null) return;
			foreach(RepositoryItem rpItem in collection) {
				if(rpItem.IsDisposed) continue;
				if(hash.Contains(rpItem)) continue;
				hash.Add(rpItem, new EditorInfo(FillRepositoryCreateViewInfo(rpItem), 1));
			}
		}
		protected BaseEditViewInfo FillRepositoryCreateViewInfo(RepositoryItem item) {
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = item.CreateViewInfo();
				UpdateEditViewInfo(viewInfo);
			return viewInfo;
		}
		protected virtual BorderStyles EditorDefaultBorderStyle { get { return BorderStyles.NoBorder; } }
		protected virtual void UpdateEditViewInfo(DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo vi) {
			vi.DefaultBorderStyle = EditorDefaultBorderStyle;
			vi.AllowDrawFocusRect = false;
			vi.FillBackground = vi.Item.AllowFocusedAppearance ? false : true;
			vi.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Minimum;
			vi.InplaceType = InplaceType.Grid;
			if(vi.LookAndFeel != View.ElementsLookAndFeel && vi.LookAndFeel.UseDefaultLookAndFeel) {
				vi.LookAndFeel = View.ElementsLookAndFeel;
			}
		}
		public virtual ArrayList CalcUniqueEditors() {
			Hashtable hash = new Hashtable();
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(string));
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(bool));
			GridControl.EditorHelper.DefaultRepository.GetRepositoryItem(typeof(DateTime));
			FillRepositoryHashtable(GridControl.EditorHelper.DefaultRepository.Items, hash);
			FillRepositoryHashtable(GridControl.EditorHelper.InternalRepository.Items, hash);
			if(GridControl.ExternalRepository != null)
				FillRepositoryHashtable(GridControl.ExternalRepository.Items, hash);
			return new ArrayList(hash.Values);
		}
		public virtual Size ScaleSize(Size size) {
			return RectangleHelper.ScaleSize(size, GridControl == null ? new SizeF(1f, 1f) : GridControl.ScaleFactor);
		}
		public virtual int ScaleVertical(int height) {
			return RectangleHelper.ScaleVertical(height, GridControl == null ? 1f : GridControl.ScaleFactor.Height);
		}
		public virtual int ScaleHorizontal(int width) {
			return RectangleHelper.ScaleHorizontal(width, GridControl == null ? 1f : GridControl.ScaleFactor.Width);
		}
		public virtual int DeScaleVertical(int height) {
			return RectangleHelper.DeScaleVertical(height, GridControl == null ? 1f : GridControl.ScaleFactor.Height);
		}
		public virtual int DeScaleHorizontal(int width) {
			return RectangleHelper.DeScaleHorizontal(width, GridControl == null ? 1f : GridControl.ScaleFactor.Width);
		}
		protected class EditorInfo
		{
			public BaseEditViewInfo EditViewInfo;
			public int RowCount;
			public EditorInfo(BaseEditViewInfo editViewInfo) : this(editViewInfo, 1) { }
			public EditorInfo(BaseEditViewInfo editViewInfo, int rowCount) {
				this.EditViewInfo = editViewInfo;
				this.RowCount = rowCount;
			}
		}
		protected virtual int CellVertIndent { get { return 2; } }
		protected virtual int MinAllowedEditorHeight { get { return 0; } }
		public virtual int CalcMinEditorHeight(AppearanceObject[] styles) {
			int maxH = 0;
			Graphics g = GInfo.AddGraphics(null);
			try {
				ArrayList infos = CalcUniqueEditors();
				foreach(EditorInfo editInfo in infos) {
					for(int n = 0; n < styles.Length; n++) {
						editInfo.EditViewInfo.PaintAppearance = styles[n];
						int minHeight = editInfo.EditViewInfo.CalcMinHeight(g);
						minHeight = Math.Max(MinAllowedEditorHeight, minHeight);
						if(editInfo.RowCount > 1) minHeight = minHeight / editInfo.RowCount + minHeight % editInfo.RowCount;
						maxH = Math.Max(minHeight + CellVertIndent , maxH);
					}
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return Math.Max(maxH, 17);
		}
		public virtual int CalcMaxHeight(AppearanceObject[] appearances) {
			Graphics g = GInfo.AddGraphics(null);
			int res = 0;
			try {
				for(int n = 0; n < appearances.Length; n++) {
					res = Math.Max(res, appearances[n].CalcDefaultTextSize(g).Height);
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
	}
	public class BaseHitInfo {
		Point hitPoint;
		int masterTabRelationIndex;
		public BaseHitInfo() {
			this.hitPoint = BaseViewInfo.EmptyPoint;
		}
		public virtual void Clear() {
			this.hitPoint = BaseViewInfo.EmptyPoint;
			this.MasterTabRelationIndex = -1;
		}
		public int MasterTabRelationIndex { get { return masterTabRelationIndex; } set { masterTabRelationIndex = value; } }
		public virtual Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public bool IsValid { get { return HitPoint.X != BaseViewInfo.EmptyPoint.X; } }
		protected internal virtual int HitTestInt { get { return 0; } }
		public BaseView View { get; set; }
	}
}
