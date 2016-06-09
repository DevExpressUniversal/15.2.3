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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraEditors.Internal {
	public enum StackPanelAutoSizeMode {
		None, Default, AutoSizeByOrientation
	}
	[ToolboxItem(false)]
	public class StackPanelControl : ContainerControl, IXtraResizableControl, ISupportInitialize {
		StackPanelViewInfo viewInfoCore;
		HorzAlignment contentAlignmentCore;
		Orientation contentOrientationCore;
		StackPanelAutoSizeMode autoSizeModeCore;
		bool allowFixedSideCore;
		int itemIndentCore;
		int initializingCount;
		public StackPanelControl()
			: base() {
			allowFixedSideCore = true;
			viewInfoCore = CreateViewInfo();
		}
		protected bool IsInitializing { get { return initializingCount > 0; } }
		[DefaultValue(StackPanelAutoSizeMode.None)]
		public StackPanelAutoSizeMode AutoSizeMode {
			get { return autoSizeModeCore; }
			set { SetValueCore(ref autoSizeModeCore, value, LayoutChanged); }
		}
		[DefaultValue(HorzAlignment.Default)]
		public HorzAlignment ContentAlignment {
			get { return contentAlignmentCore; }
			set { SetValueCore(ref contentAlignmentCore, value, LayoutChanged); }
		}
		[DefaultValue(Orientation.Horizontal)]
		public Orientation ContentOrientation {
			get { return contentOrientationCore; }
			set { SetValueCore(ref contentOrientationCore, value, LayoutChanged); }
		}
		[DefaultValue(true)]
		public bool AllowFixedSide {
			get { return allowFixedSideCore; }
			set { SetValueCore(ref allowFixedSideCore, value, LayoutChanged); }
		}
		[DefaultValue(0)]
		public int ItemIndent {
			get { return itemIndentCore; }
			set { SetValueCore(ref itemIndentCore, value, LayoutChanged); }
		}
		protected internal StackPanelViewInfo ViewInfo { get { return viewInfoCore; } }
		public Size GetMinSize() {
			Rectangle clientRect = new Rectangle(Point.Empty, ViewInfo.CalcContentSize());
			return ViewInfo.GetBoundsByClientRectangle(clientRect).Size;
		}
		protected virtual StackPanelViewInfo CreateViewInfo() {
			return new StackPanelViewInfo(this);
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			LayoutChanged();
		}
		protected override Control.ControlCollection CreateControlsInstance() {
			return new StackPanelControlCollection(this);
		}
		internal bool isControlAdded;
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			isControlAdded = DesignMode;
			if(e.Control is IXtraResizableControl) {
				ViewInfo.RegisterChild(e.Control);
				LayoutChanged();
			}
		}
		protected override void OnControlRemoved(ControlEventArgs e) {
			base.OnControlRemoved(e);
			if(e.Control is IXtraResizableControl) {
				ViewInfo.UnregisterChild(e.Control);
				LayoutChanged();
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(!ViewInfo.IsReady)
				ViewInfo.Calc();
		}
		public virtual void LayoutChanged() {
			ViewInfo.SetDirty();
			Invalidate();
			if(IsHandleCreated)
				BeginInvoke(new Action(() => { Update(); }));
		}
		protected bool SetValueCore<T>(ref T property, T value, Action action = null) {
			if(property.Equals(value))
				return false;
			property = value;
			if(action != null)
				action();
			return true;
		}
		#region ISupportInitialize Members
		public void BeginInit() {
			initializingCount++;
		}
		public void EndInit() {
			initializingCount--;
			if(!IsInitializing) {
				isControlAdded = false;
				ViewInfo.SetDirty();
				Invalidate();
			}
		}
		#endregion
		#region IXtraResizableControl Members
		event EventHandler IXtraResizableControl.Changed {
			add { }
			remove { }
		}
		bool IXtraResizableControl.IsCaptionVisible {
			get { return false; }
		}
		Size IXtraResizableControl.MaxSize {
			get { return GetMinSize(); }
		}
		Size IXtraResizableControl.MinSize {
			get { return GetMinSize(); }
		}
		#endregion
	}
	public class StackPanelControlCollection : Control.ControlCollection {
		public StackPanelControlCollection(StackPanelControl owner) : base(owner) { }
		public new StackPanelControl Owner { get { return base.Owner as StackPanelControl; } }
		public override void SetChildIndex(Control child, int newIndex) {
			newIndex = CorrectChildIndex(child, newIndex);
			base.SetChildIndex(child, newIndex);
			Owner.LayoutChanged();
		}
		int CorrectChildIndex(Control child, int newIndex) {
			StackPanelItemInfo childInfo = Owner.ViewInfo.Infos[child];
			if(Owner.isControlAdded && childInfo != null) {
				Owner.isControlAdded = false;
				return childInfo.Index;
			}
			else {
				Owner.ViewInfo.Infos.SetChildIndex(child, newIndex);
			}
			return newIndex;
		}
	}
	public class StackPanelItemInfoCollection : List<StackPanelItemInfo> {
		public StackPanelItemInfoCollection() { }
		public StackPanelItemInfo this[Control ownerControl] {
			get {
				if(this.Exists(item => item.Owner == ownerControl))
					return this.Single(item => item.Owner == ownerControl);
				return null;
			}
		}
		public void SetChildIndex(Control child, int newIndex) {
			StackPanelItemInfo childInfo = this[child];
			if(childInfo != null && childInfo.Index != newIndex) {
				if(Remove(childInfo)) {
					if(newIndex > Count - 1 || newIndex < 0)
						Add(childInfo);
					else
						Insert(newIndex, childInfo);
				}
			}
		}
	}
	public class StackPanelViewInfo : ObjectInfoArgs {
		StackPanelControl ownerCore;
		StackPanelItemInfoCollection infosCore;
		public StackPanelViewInfo(StackPanelControl owner) {
			ownerCore = owner;
			infosCore = new StackPanelItemInfoCollection();
		}
		public virtual StackPanelControl Owner { get { return ownerCore; } }
		public StackPanelItemInfoCollection Infos { get { return infosCore; } }
		public StackPanelAutoSizeMode AutoSizeMode { get { return Owner.AutoSizeMode; } }
		public bool IsHorizontal { get { return Owner.ContentOrientation == Orientation.Horizontal; } }
		public HorzAlignment ContentAlignment { get { return Owner.ContentAlignment; } }
		public Padding Padding { get { return Owner.Padding; } }
		public int ItemIndent { get { return Owner.ItemIndent; } }
		public bool AllowFixedSide { get { return Owner.AllowFixedSide; } }
		public Rectangle ClientRectangle { get; set; }
		public Rectangle ContentRectangle { get; set; }
		public bool IsReady { get; private set; }
		public void Calc(int beginIndex = 0) {
			if(IsReady) return;
			AssignSize();
			CalcBounds();
			CalcClientRectangle();
			CalcContentRectangle();
			CalcItems(beginIndex);
			IsReady = true;
		}
		protected virtual StackPanelItemInfo CreateInfo(IXtraResizableControl owner) {
			return new StackPanelItemInfo(owner, this);
		}
		public Rectangle GetBoundsByClientRectangle(Rectangle rect) {
			rect.Offset(-Padding.Left, -Padding.Top);
			rect.Size = new Size(rect.Width + Padding.Horizontal, rect.Height + Padding.Vertical);
			return rect;
		}
		public Rectangle GetObjectClientRectangle() {
			Rectangle rect = Bounds;
			rect.Offset(Padding.Left, Padding.Top);
			rect.Size = new Size(rect.Width - Padding.Horizontal, rect.Height - Padding.Vertical);
			return rect;
		}
		protected virtual void AssignSize() {
			if(Infos.Count == 0) return;
			Size size = GetBoundsByClientRectangle(new Rectangle(Point.Empty, CalcContentSize())).Size;
			Owner.Size = CorrectSizeByDockStyle(size);
		}
		protected virtual Size CorrectSizeByDockStyle(Size size) {
			DockStyle dock = Owner.Dock;
			if(dock == DockStyle.Left || dock == DockStyle.Right)
				size.Height = Owner.Height;
			if(dock == DockStyle.Top || dock == DockStyle.Bottom)
				size.Width = Owner.Width;
			if(dock == DockStyle.Fill)
				size = Owner.Size;
			return size;
		}
		protected virtual void CalcBounds() {
			Bounds = Owner.ClientRectangle;
		}
		protected virtual void CalcClientRectangle() {
			ClientRectangle = GetObjectClientRectangle();
		}
		protected internal virtual void CalcContentRectangle() {
			Point contentLocation = ClientRectangle.Location;
			Size contentSize = CalcContentSize();
			if(ContentAlignment == HorzAlignment.Far)
				contentLocation = IsHorizontal ? new Point(ClientRectangle.Right - contentSize.Width, ClientRectangle.Top) : new Point(ClientRectangle.Left, ClientRectangle.Bottom - contentSize.Height);
			if(ContentAlignment == HorzAlignment.Center) {
				int offset = (int)((IsHorizontal ? ClientRectangle.Width - contentSize.Width : ClientRectangle.Height - contentSize.Height) / 2.0 + 0.5);
				contentLocation = IsHorizontal ? new Point(ClientRectangle.X + offset, ClientRectangle.Y) : new Point(ClientRectangle.X, ClientRectangle.Y + offset);
			}
			ContentRectangle = new Rectangle(contentLocation, contentSize);
		}
		public Size CalcContentSize() {
			Size contentSize = Size.Empty;
			for(int i = 0; i < Infos.Count; i++) {
				StackPanelItemInfo info = Infos[i];
				Size size = info.GetActualSize();
				if(IsHorizontal) {
					contentSize.Width += size.Width + (i != Infos.Count - 1 ? ItemIndent : 0);
					contentSize.Height = Math.Max(size.Height, contentSize.Height);
				}
				else {
					contentSize.Height += size.Height + (i != Infos.Count - 1 ? ItemIndent : 0);
					contentSize.Width = Math.Max(size.Width, contentSize.Width);
				}
			}
			return contentSize;
		}
		protected internal virtual void CalcItems(int beginIndex = 0) {
			for(int i = beginIndex; i < Infos.Count; i++)
				Infos[i].Calc(GetItemLocation(Infos[i], i, beginIndex));
		}
		protected virtual Point GetItemLocation(StackPanelItemInfo info, int index, int beginIndex) {
			if(beginIndex == index && beginIndex > 0)
				return info.Location;
			else if(index == 0)
				return ContentRectangle.Location;
			Rectangle prevItemBounds = new Rectangle(Infos[index - 1].Location, Infos[index - 1].GetActualSize());
			int offset = (IsHorizontal ? prevItemBounds.X + prevItemBounds.Width : prevItemBounds.Y + prevItemBounds.Height) + ItemIndent;
			return IsHorizontal ? new Point(offset, prevItemBounds.Y) : new Point(prevItemBounds.X, offset);
		}
		public virtual void RegisterChild(Control control) {
			IXtraResizableControl child = control as IXtraResizableControl;
			if(child == null) return;
			StackPanelItemInfo info = CreateInfo(child);
			Infos.Add(info);
			SetAutoSizeInLayoutControl(control, true);
		}
		public virtual void UnregisterChild(Control control) {
			StackPanelItemInfo info = Infos[control];
			if(info != null) {
				IXtraResizableControl child = info.Owner as IXtraResizableControl;
				Infos.Remove(info);
				SetAutoSizeInLayoutControl(control, false);
				info.Dispose();
			}
		}
		void SetAutoSizeInLayoutControl(Control control, bool value) {
			if(control is LabelControl)
				(control as LabelControl).SetAutoSizeInLayoutControl(value);
			else if(control is BaseControl)
				(control as BaseControl).AutoSizeInLayoutControl = value;
		}
		public void SetDirty() {
			IsReady = false;
		}
	}
	public class StackPanelItemInfo : IDisposable {
		StackPanelViewInfo viewInfoCore;
		Control ownerCore;
		Rectangle boundsCore;
		public StackPanelItemInfo(IXtraResizableControl owner, StackPanelViewInfo viewInfo) {
			ownerCore = owner as Control;
			viewInfoCore = viewInfo;
			SetOwnerSizes();
			SubscribeOnOwnerEvents();
		}
		public void Dispose() {
			UnsubscribeOnOwnerEvents();
			ownerCore = null;
			viewInfoCore = null;
		}
		public int Index { get { return ViewInfo.Infos.IndexOf(this); } }
		protected internal Control Owner { get { return ownerCore; } }
		protected StackPanelViewInfo ViewInfo { get { return viewInfoCore; } }
		protected StackPanelAutoSizeMode AutoSizeMode { get { return ViewInfo.AutoSizeMode; } }
		protected bool IsHorizontal { get { return ViewInfo.IsHorizontal; } }
		protected Size DefinedMinSize { get { return Owner.MinimumSize; } }
		protected Size DefinedMaxSize { get { return Owner.MaximumSize; } }
		protected Size MinSize { get { return (Owner as IXtraResizableControl).MinSize; } }
		protected Rectangle Bounds { get { return boundsCore; } set { boundsCore = value; } }
		public Point Location { get { return boundsCore.Location; } set { boundsCore.Location = value; } }
		public Size GetActualSize() {
			if(AutoSizeMode == StackPanelAutoSizeMode.Default)
				return MinSize;
			if(AutoSizeMode == StackPanelAutoSizeMode.AutoSizeByOrientation) {
				Size actualSize = new Size(Math.Max(DefinedMinSize.Width, MinSize.Width), Math.Max(DefinedMinSize.Height, MinSize.Height));
				if(!DefinedMaxSize.IsEmpty)
					actualSize = new Size(Math.Min(actualSize.Width, DefinedMaxSize.Width), Math.Min(actualSize.Height, DefinedMaxSize.Height));
				return IsHorizontal ? new Size(actualSize.Width, Bounds.Height) : new Size(Bounds.Width, actualSize.Height);
			}
			return new Size(Math.Max(MinSize.Width, Bounds.Width), Math.Max(MinSize.Height, Bounds.Height));
		}
		protected internal virtual void OnOwnerBoundsChanged(object sender, EventArgs e) {
			SetOwnerSizes();
			if(!ViewInfo.IsReady || Index == -1) return;
			ViewInfo.SetDirty();
			ViewInfo.Calc(Index);
		}
		protected internal virtual void SetOwnerSizes() {
			boundsCore.Size = Owner.Size;
		}
		public void Calc(Point location) {
			Location = location;
			Owner.Bounds = new Rectangle(location, CalcLayoutSize());
		}
		protected virtual Size CalcLayoutSize() {
			Size size = GetActualSize();
			if(ViewInfo.AllowFixedSide)
				return IsHorizontal ? new Size(size.Width, ViewInfo.ContentRectangle.Height) : new Size(ViewInfo.ContentRectangle.Width, size.Height);
			return size;
		}
		protected virtual void SubscribeOnOwnerEvents() {
			(Owner as IXtraResizableControl).Changed += OnOwnerBoundsChanged;
			Owner.LocationChanged += OnOwnerBoundsChanged;
			Owner.SizeChanged += OnOwnerBoundsChanged;
		}
		protected virtual void UnsubscribeOnOwnerEvents() {
			(Owner as IXtraResizableControl).Changed -= OnOwnerBoundsChanged;
			Owner.LocationChanged -= OnOwnerBoundsChanged;
			Owner.SizeChanged -= OnOwnerBoundsChanged;
		}
	}
}
