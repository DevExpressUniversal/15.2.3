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
using System.Collections;
using System.Drawing;
namespace DevExpress.XtraBars.Docking.Helpers {
	public class DirectionSize {
		Size size;
		bool isHorizontal;
		public DirectionSize(Size size, bool isHorizontal) {
			this.size = size;
			this.isHorizontal = isHorizontal;
		}
		public int Width { 
			get { return IsHorizontal ? size.Width : size.Height; } 
			set {
				if(IsHorizontal)
					size.Width = value;
				else
					size.Height = value;
			}
		}
		public int Height { 
			get { return IsHorizontal ? size.Height: size.Width; } 
			set {
				if(!IsHorizontal)
					size.Width = value;
				else
					size.Height = value;
			}
		}
		public static Size GetSize(int width, int height, DockingStyle dock) {
			if(LayoutRectangle.GetIsHorizontal(dock))
				return new Size(width, height);
			else
				return new Size(height, width);
		}
		public Size Size { get { return size; } }
		public Size DirectSize { get { return new Size(Width, Height); } }
		public bool IsHorizontal { get { return isHorizontal; } }
	}
	public class DirectionRectangle {
		Rectangle bounds;
		bool isHorizontal;
		DockingStyle dock;
		public DirectionRectangle(Rectangle bounds, bool isHorizontal) {
			this.bounds = bounds;
			this.isHorizontal = isHorizontal;
			this.dock = DockingStyle.Float;
		}
		public DirectionRectangle(Rectangle bounds, DockingStyle dock) : this(bounds, LayoutRectangle.GetIsHorizontal(dock)) {
			this.dock = dock;
		}
		public void RemoveSize(int width, bool fromHead) {
			Size size = new Size(width, width);
			if(fromHead) {
				if(Dock == DockingStyle.Bottom || Dock == DockingStyle.Right)
					CutFromHead(size);
				else
					CutFromTail(size);
			}
			else {
				if(Dock == DockingStyle.Bottom || Dock == DockingStyle.Right)
					CutFromTail(size);
				else
					CutFromHead(size);
			}
		}
		public void CutFromHead(Size size) {
			CutFromHead(IsHorizontal ? size.Width : size.Height);
		}
		public void CutFromHead(int size) {
			if(IsHorizontal)
				bounds = LayoutRectangle.RemoveSize(bounds, size, DockingStyle.Left);
			else
				bounds = LayoutRectangle.RemoveSize(bounds, size, DockingStyle.Top);
		}
		public void CutFromTail(Size size) {
			CutFromTail(IsHorizontal ? size.Width : size.Height);
		}
		public void CutFromTail(int size) {
			if(IsHorizontal)
				bounds = LayoutRectangle.RemoveSize(bounds, size, DockingStyle.Right);
			else
				bounds = LayoutRectangle.RemoveSize(bounds, size, DockingStyle.Bottom);
		}
		public Rectangle GetPrevRectangle(Size size, int tailIndent) {
			Point location;
			if(IsHorizontal)
				location = new Point(Bounds.Right - tailIndent - size.Width, Bounds.Top + (Bounds.Height - size.Height) / 2);
			else
				location = new Point(Bounds.Left + (Bounds.Width - size.Width) / 2, Bounds.Bottom - tailIndent - size.Height);
			return new Rectangle(location, size);
		}
		public Rectangle GetNextRectangle(Size size, int headIndent) {
			Point location;
			if(IsHorizontal)
				location = new Point(Bounds.Left + headIndent, Bounds.Top + (Bounds.Height - size.Height) / 2);
			else
				location = new Point(Bounds.Left + (Bounds.Width - size.Width) / 2, Bounds.Top + headIndent);
			return new Rectangle(location, size);
		}
		public void Inflate(int size) {
			if(IsHorizontal)
				bounds.Inflate(size, 0);
			else
				bounds.Inflate(0, size);
		}
		public void SetLocation(Point location) {
			SetLocation(IsHorizontal ? location.X : location.Y);
		}
		public void SetLocation(int location) {
			if(IsHorizontal)
				bounds.X = location;
			else
				bounds.Y = location;
		}
		public void Offset(int dy) {
			if(IsHorizontal) this.bounds.X += dy;
			else this.bounds.Y += dy;
		}
		public void SetSize(Size size) {
			SetSize(IsHorizontal ? size.Width : size.Height);
		}
		public void IncreaseHeight(int size) {
			if(IsHorizontal)
				bounds.Height += size;
			else
				bounds.Width += size;
		}
		public void IncreaseY(int size) {
			if(IsHorizontal)
				bounds.Y += size;
			else
				bounds.X += size;
		}
		public void SetSize(int size) {
			if(IsHorizontal)
				bounds.Width = size;
			else
				bounds.Height = size;
		}
		public Rectangle GetSideRectangle(int size) {
			return GetSideRectangle(size, 0);
		}
		public Rectangle GetSideRectangle(Size size) {
			return GetSideRectangle(IsHorizontal ? size.Width : size.Height);
		}
		public Rectangle GetSideRectangle(int size, int offsetLocation) {
			DirectionSize dSize = new DirectionSize(new Size(size, Height), IsHorizontal);
			if(LayoutRectangle.GetIsHead(Dock))
				return GetNextRectangle(dSize.DirectSize, offsetLocation);
			else
				return GetPrevRectangle(dSize.DirectSize, offsetLocation);
		}
		public int Width { get { return (IsHorizontal ? Bounds.Width : Bounds.Height); } }
		public int Height { get { return (IsHorizontal ? Bounds.Height : Bounds.Width); } }
		public int Left { get { return (IsHorizontal ? Bounds.Left : Bounds.Top); } }
		public int Right { get { return (IsHorizontal ? Bounds.Right : Bounds.Bottom); } }
		public Rectangle Bounds { get { return bounds; } }
		public bool IsHorizontal { get { return isHorizontal; } }
		public DockingStyle Dock { get { return dock; } }
	}
	public class AdjacentInfo {
		LayoutInfo adjacentLayout;
		Size adjacentSize;
		public AdjacentInfo(LayoutInfo adjacentLayout, Size adjacentSize) {
			this.adjacentLayout = adjacentLayout;
			this.adjacentSize = adjacentSize;
		}
		public void SetSize() {
			AdjacentLayout.SetSize(AdjacentSize);
		}
		public LayoutInfo AdjacentLayout { get { return adjacentLayout; } }
		public Size AdjacentSize { get { return adjacentSize; } }
	}
	public class LayoutRectangle {
		Rectangle bounds;
		DockingStyle dock;
		public LayoutRectangle(Rectangle bounds, DockingStyle dock) {
			this.bounds = bounds;
			this.dock = dock;
		}
		public Rectangle RemoveSize(int size) {
			this.bounds = RemoveSize(bounds, size, Dock);
			return Bounds;
		}
		public Rectangle RemoveSize(Size size) {
			return RemoveSize(new DirectionSize(size, IsHorizontal).Width);
		}
		public static Rectangle RemoveSize(Rectangle bounds, int size, DockingStyle dock) {
			switch(dock) {
				case DockingStyle.Left:
					bounds.X += size;
					bounds.Width -= size;
					break;
				case DockingStyle.Right:
					bounds.Width -= size;
					break;
				case DockingStyle.Top:
					bounds.Y += size;
					bounds.Height -= size;
					break;
				case DockingStyle.Bottom:
					bounds.Height -= size;
					break;
			}
			return bounds;
		}
		public static bool GetIsHorizontal(DockingStyle dock) {
			return (dock == DockingStyle.Left || dock == DockingStyle.Right);
		}
		public static bool GetIsVertical(DockingStyle dock) {
			return (dock == DockingStyle.Top || dock == DockingStyle.Bottom);
		}
		public static bool GetIsHead(DockingStyle dock) {
			return (dock == DockingStyle.Left || dock == DockingStyle.Top);
		}
		public static DockingStyle GetHead(bool isHorizontal) {
			return (isHorizontal ? DockingStyle.Left : DockingStyle.Top);
		}
		public static DockingStyle GetTail(bool isHorizontal) {
			return (isHorizontal ? DockingStyle.Right : DockingStyle.Bottom);
		}
		public static DockingStyle GetOppositeDockingStyle(DockingStyle dock) {
			int n = Convert.ToInt32(dock) + 1;
			if(n / 2 == 1) return (DockingStyle)(4 - n);
			return (DockingStyle)(8 - n);
		}
		public bool IsHorizontal { get { return GetIsHorizontal(Dock); } }
		public Rectangle Bounds { get { return bounds; } }
		public DockingStyle Dock { get { return dock; } }
	}
	public class AutoHideInfo : ChangedCollectionBase {
		DockingStyle dock;
		protected internal AutoHideInfo(DockingStyle dock) {
			this.dock = dock;
		}
		public int Add(LayoutInfo info) {
			if(Contains(info)) return List.IndexOf(info);
			return List.Add(info);
		}
		public void Remove(LayoutInfo info) {
			List.Remove(info);
		}
		public bool Contains(LayoutInfo info) {
			return InnerList.Contains(info);
		}
		public int IndexOf(LayoutInfo info) {
			return InnerList.IndexOf(info);
		}
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			for(int i = 0; i < Count; i++)
				this[i].ReplaceSavedParent(oldParent, newParent, replaceSaved);
		}
		protected internal void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			for(int i = 0; i < Count; i++)
				this[i].OnCreateDestroyLayout(info, owner, newInfo);
		}
		public LayoutInfo this[int index] { get { return InnerList[index] as LayoutInfo; } }
		public DockingStyle Dock { get { return dock; } }
	}
	public class AutoHideInfoCollection : CollectionBase {
		public int Add(AutoHideInfo info) {
			return InnerList.Add(info);
		}
		public void Remove(AutoHideInfo info) {
			InnerList.Remove(info);
		}
		public AutoHideInfo this[int index] { get { return InnerList[index] as AutoHideInfo; } }
		public AutoHideInfo this[DockingStyle dock] {
			get {
				for(int index = 0; index < Count; index ++) {
					if(this[index].Dock == dock)
						return this[index];
				}
				return null;
			}
		}
		public bool Contains(AutoHideInfo info) { return InnerList.Contains(info); }
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			for(int i = 0; i < Count; i++)
				this[i].ReplaceSavedParent(oldParent, newParent, replaceSaved);
		}
		protected internal void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			for(int i = 0; i < Count; i++)
				this[i].OnCreateDestroyLayout(info, owner, newInfo);
		}
		protected internal void OnLayoutInfoAutoHideChanged(LayoutInfo info) {
			if(info.AutoHide)
				Add(info);
			else
				Remove(info);
		}
		void Add(LayoutInfo info) {
			DockingStyle dock = info.AutoHideDock;
			if(this[dock] == null)
				Add(new AutoHideInfo(dock));
			this[dock].Add(info);
		}
		void Remove(LayoutInfo info) {
			for(int i = Count - 1; i >= 0; i --) {
				if(!this[i].Contains(info)) continue;
				this[i].Remove(info);
				if(this[i].Count == 0)
					Remove(this[i]);
				break;
			}
		}
		protected internal Rectangle GetClientBounds(Rectangle bounds, int panelSize) {
			foreach(AutoHideInfo info in this) {
				bounds = (new LayoutRectangle(bounds, info.Dock)).RemoveSize(panelSize);
			}
			return bounds;
		}
		protected internal Rectangle GetClientBounds(Rectangle bounds, int panelSize, System.Collections.Generic.Dictionary<DockingStyle, int> panelsSize) {
			foreach(AutoHideInfo info in this) {
				int length = 0;
				if(panelsSize.TryGetValue(info.Dock, out length))
					bounds = (new LayoutRectangle(bounds, info.Dock)).RemoveSize(length);
				else
					bounds = (new LayoutRectangle(bounds, info.Dock)).RemoveSize(panelSize);	
			}
			return bounds;
		}
	}
	public class LayoutInfoCollection : CollectionBase {
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			LayoutInfo info = value as LayoutInfo;
			if(info != null) info.Disposed += info_Disposed;
		}
		protected override void OnRemoveComplete(int index, object value) {
			LayoutInfo info = value as LayoutInfo;
			if(info != null) info.Disposed -= info_Disposed;
			base.OnRemoveComplete(index, value);
		}
		void info_Disposed(object sender, EventArgs e) {
			LayoutInfo info = sender as LayoutInfo;
			info.Disposed -= info_Disposed;
			if(InnerList.Contains(info))
				InnerList.Remove(info);
		}
		protected internal void OnLayoutInfoVisibleChanged(LayoutInfo info) {
			if(info.Visibility == DockVisibility.Hidden) {
				if(!InnerList.Contains(info))
					((IList)this).Add(info);
			}
			else {
				if(InnerList.Contains(info))
					((IList)this).Remove(info);
			}
		}
		protected internal void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			for(int i = 0; i < Count; i++)
				this[i].ReplaceSavedParent(oldParent, newParent, replaceSaved);
		}
		protected internal void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			for(int i = 0; i < Count; i++)
				this[i].OnCreateDestroyLayout(info, owner, newInfo);
		}
		public LayoutInfo this[int index] { get { return InnerList[index] as LayoutInfo; } }
	}
	public class LayoutConsts {
		public const int InvalidIndex = -1;
		public static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);
		public static readonly Size InvalidSize = Size.Empty;
		public static readonly Rectangle InvalidRectangle = Rectangle.Empty;
		public const bool DefaultFloatVertical = false;
		public const bool DefaultCanParentLayout = false;
		public const DockVisibility DefaultVisibility = DockVisibility.Visible;
	}
	public interface ILayoutInfoInitializer {
		void InitializeBeforeAssignContent(LayoutInfo info);
		void InitializeAfterAssignContent(LayoutInfo info);
	}
	public class SavedInfo {
		BaseLayoutInfo savedParent;
		DockingStyle savedDock;
		int savedIndex;
		bool savedTabbed, nativeParent,savedMdiDocument;
		bool canParentLayout, topLevel;
		double savedFactorSize;
		public SavedInfo() {
			Clear();
		}
		public SavedInfo(LayoutInfo info, bool topLevel) {
			Assign(info.SavedInfo);
			SavedParent = info;
			CanParentLayout = true;
			this.topLevel = topLevel;
		}
		public virtual void Assign(SavedInfo savedInfo) {
			SavedParent = savedInfo.SavedParent;
			SavedDock = savedInfo.SavedDock;
			SavedIndex = savedInfo.SavedIndex;
			SavedTabbed = savedInfo.SavedTabbed;
			SavedMdiDocument = savedInfo.SavedMdiDocument;
			NativeParent = savedInfo.NativeParent;
			CanParentLayout = savedInfo.CanParentLayout;
			SavedFactorSize = savedInfo.SavedFactorSize;
			SavedParentFloatVertical = savedInfo.SavedParentFloatVertical;
			this.topLevel = savedInfo.TopLevel;
		}
		public virtual void Clear() {
			this.savedParent = null;
			this.savedDock = DockingStyle.Float;
			this.savedIndex = LayoutConsts.InvalidIndex;
			this.savedTabbed = false;
			this.savedMdiDocument = false;
			this.nativeParent = true;
			this.canParentLayout = LayoutConsts.DefaultCanParentLayout;
			this.topLevel = false;
			this.savedFactorSize = 1;
		}
		bool IsSavedParent(BaseLayoutInfo parent) { return (SavedParent == parent); }
		public virtual void ReplaceSavedParent(LayoutInfo oldParent, BaseLayoutInfo newParent, bool replaceSaved) {
			if(!IsSavedParent(oldParent)) return;
			this.SavedParent = newParent;
			if(replaceSaved) {
				SavedDock = oldParent.Dock;
				SavedIndex = oldParent.Index;
				SavedTabbed = oldParent.Tabbed;
			}
		}
		public virtual void OnCreateDestroyLayout(LayoutInfo info, BaseLayoutInfo owner, LayoutInfo newInfo) {
			bool create = (newInfo != null);
			if(create) {
				if(!info.SavedInfo.NativeParent && info.SavedInfo.SavedParent == SavedParent) {
					CanParentLayout = true;
					return;
				}
				else if(!NativeParent) {
					SavedParent = newInfo;
					return;
				}
			}
			else {
				if(IsSavedParent(info)) {
					ReplaceSavedParent(info, owner, owner.AcceptSettingsOnChildDestroy);
					NativeParent = false;
				}
			}
			if(!IsSavedParent(owner)) return;
			CanParentLayout = !create;
			if(topLevel) CanParentLayout = true;
		}
		public virtual void SaveSettings(LayoutInfo info) {
			SavedParent = info.Parent;
			SavedDock = info.Dock;
			SavedIndex = info.Index;
			SavedTabbed = info.IsTab;
			SavedMdiDocument = info.IsMdiDocument;
			SavedFactorSize = info.SizeFactor;
			if(info.Parent is DockLayout) {
				SavedParentFloatVertical = (info.Parent as DockLayout).FloatVertical;
			}
		}
		public virtual void Restore(LayoutInfo info) {
			if(!Saved) return;
			if(info.Visibility != DockVisibility.Visible) {
				info.OnRestoreVisibility();
			}
			info.CanParentLayout = CanParentLayout;
			bool tabbed = SavedTabbed;
			ILayoutInfoInitializer savedLayoutInfoInitializer = info.Initializer;
			try {
				info.Initializer = new RestoreTabbedLayoutInfoInitializer(tabbed);
				LayoutInfo savedParent = SavedParent as LayoutInfo;
				info.SizeFactor = SavedFactorSize;
				if(SavedMdiDocument || (savedParent != null && savedParent.IsMdiDocument)) {
					if(savedParent == null && SavedParent != null) {
						DockLayoutManager manager = SavedParent as DockLayoutManager;
						if(manager != null && info.Parent == null) {
							manager.AddLayoutInternal(info);
							(info as BaseLayoutInfo).Parent = manager;
							manager.DockManager.UpdateRootPanels();
						}
					}
					info.DockAsMdiDocument();
				}
				else if(savedParent != null && SavedTabbed)
					info.DockAsTab(savedParent, SavedIndex);
				else {
					info.DockTo(SavedParent, SavedDock, SavedIndex);
					DockLayout docklayout = info.Parent as DockLayout;
					if(docklayout != null)
						docklayout.FloatVertical = SavedParentFloatVertical;
				}
			}
			finally {
				info.Initializer = savedLayoutInfoInitializer;
				info.CanParentLayout = LayoutConsts.DefaultCanParentLayout;
			}
			if(info.LayoutParent != null) {
				info.LayoutParent.Tabbed = tabbed;
				if(info.LayoutParent.Tabbed)
					info.LayoutParent.ActiveChild = info;
			}
			Clear();
		}
		public virtual BaseLayoutInfoManager GetManager(LayoutInfo info) {
			BaseLayoutInfoManager result = null;
			if(SavedParent is LayoutInfo)
				result = ((LayoutInfo)SavedParent).InnerManager;
			else if(info.LayoutParent != null && (!info.LayoutParent.IsValid || info.IsParentAutoHide))
				result = info.LayoutParent.InnerManager;
			else if(SavedParent != null)
				result = SavedParent.Root;
			return result;
		}
		public DockingStyle AutoHideDock {
			get {
				if(SavedParent is LayoutInfo) return ((LayoutInfo)SavedParent).AutoHideDock;
				else return SavedDock;
			}
		}
		protected bool TopLevel { get { return topLevel; } }
		protected bool CanParentLayout { get { return canParentLayout; } set { canParentLayout = value; } }
		public BaseLayoutInfo SavedParent { get { return savedParent; } set { savedParent = value; } }
		public bool NativeParent { get { return nativeParent; } set { nativeParent = value; } }
		public int SavedIndex { get { return savedIndex; } set { savedIndex = value; } }
		public DockingStyle SavedDock { get { return savedDock; } set { savedDock = value; } }
		public bool SavedParentFloatVertical { get; set; }
		public bool SavedTabbed { get { return savedTabbed; } set { savedTabbed = value; } }
		public bool SavedMdiDocument { get { return savedMdiDocument; } set { savedMdiDocument = value; } }
		public bool Saved { get { return !(SavedParent == null && SavedIndex == LayoutConsts.InvalidIndex); } }
		public double SavedFactorSize { get { return savedFactorSize; } set { savedFactorSize = value; } }
		internal void UpdateAutoHideDock(DockingStyle dock) {
			if(SavedParent is LayoutInfo) return;
			if(SavedParent == null && dock != DockingStyle.Float && dock != DockingStyle.Fill) {
				SavedDock = dock;
			}
		}
	}
}
