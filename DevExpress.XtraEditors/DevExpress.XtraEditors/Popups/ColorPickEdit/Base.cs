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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public abstract class InnerColorPickControlBase : BaseStyleControl {
		static object selectedColorChanged = new object();
		ColorItem selectedColorItem;
		public InnerColorPickControlBase() {
			this.selectedColorItem = null;
		}
		public Color SelectedColor {
			get { return SelectedColorItem != null ? SelectedColorItem.Color : Color.Empty; }
			set {
				if(value == Color.Empty) {
					SelectedColorItem = null;
				}
				else {
					SelectedColorItem = GetColorItemByColor(value);
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColorItem SelectedColorItem {
			get { return selectedColorItem; }
			set {
				if(SelectedColorItem == value)
					return;
				ColorItem prev = SelectedColorItem;
				selectedColorItem = value;
				OnSelectedColorItemChanged(prev, SelectedColorItem);
			}
		}
		protected virtual void OnSelectedColorItemChanged(ColorItem prevItem, ColorItem nextItem) {
			OnSelectedColorChanged(new InnerColorPickControlSelectedColorChangedEventArgs(prevItem, nextItem));
			OnPropertiesChanged();
		}
		public virtual void SetColor(Color color, bool raiseEvent) {
			if(raiseEvent) {
				SelectedColor = color;
			}
			else {
				this.selectedColorItem = GetColorItemByColor(color);
			}
		}
		protected internal virtual void OnSelectedColorChanged(InnerColorPickControlSelectedColorChangedEventArgs e) {
			InnerColorPickControlSelectedColorChangedEventHandler handler = (InnerColorPickControlSelectedColorChangedEventHandler)Events[selectedColorChanged];
			if(handler != null) handler(this, e);
		}
		protected virtual bool ShouldSerializeSelectedColor() { return SelectedColor != Color.Empty; }
		protected virtual void ResetSelectedColor() { SelectedColor = Color.Empty; }
		public event InnerColorPickControlSelectedColorChangedEventHandler SelectedColorChanged {
			add { Events.AddHandler(selectedColorChanged, value); }
			remove { Events.RemoveHandler(selectedColorChanged, value); }
		}
		public void DoMouseWheel(MouseEventArgs e) {
			OnMouseWheel(e);
		}
		public void DoKeyDown(KeyEventArgs e) {
			OnKeyDown(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(!this.fDisposing && Visible) LayoutChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Selectable {
			get { return GetStyle(ControlStyles.Selectable); }
			set { SetStyle(ControlStyles.Selectable, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UserMouse {
			get { return GetStyle(ControlStyles.UserMouse); }
			set { SetStyle(ControlStyles.UserMouse, value); }
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			LayoutChanged();
		}
		public abstract bool ContainsColor(Color color);
		public abstract ColorItem GetColorItemByColor(Color value);
	}
	[TypeConverter("DevExpress.XtraEditors.Design.ColorItemTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesignFull)]
	public class ColorItem {
		Color color;
		public ColorItem() : this(Color.Empty) { }
		public ColorItem(Color color) {
			this.color = color;
		}
		protected internal virtual bool IsAutoColor { get { return false; } }
		public override string ToString() {
			return string.Format("{0}: ({1})", GetType().Name, Color.ToString());
		}
		public Color Color { get { return color; } set { color = value; } }
	}
	public class AutoColorItem : ColorItem {
		public AutoColorItem(Color color)
			: base(color) {
		}
		protected internal override bool IsAutoColor { get { return true; } }
	}
	[ListBindable(false), TypeConverter(typeof(UniversalCollectionTypeConverter)), Editor("DevExpress.XtraEditors.Design.ColorItemCollectionEditor," + AssemblyInfo.SRAssemblyEditorsDesignFull, typeof(UITypeEditor))]
	public class ColorItemCollection : CollectionBase {
		int lockUpdate;
		public ColorItemCollection() {
			this.lockUpdate = 0;
		}
		public virtual void Add(ColorItem item) {
			List.Add(item);
		}
		public virtual void AddRange(IEnumerable items) {
			BeginUpdate();
			try {
				foreach(ColorItem item in items) Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public virtual void AddColorRange(IEnumerable items, bool clearOld = true) {
			BeginUpdate();
			try {
				if(clearOld) Clear();
				foreach(Color color in items) Add(new ColorItem(color));
			}
			finally {
				EndUpdate();
			}
		}
		public virtual ColorItem GetItem(Color color, bool safeCheck) {
			return GetItemByColor(color, safeCheck);
		}
		protected virtual ColorItem GetItemByColor(Color color, bool safeCheck) {
			foreach(ColorItem colorItem in List) {
				if(IsColorsEquals(colorItem.Color, color, safeCheck)) return colorItem;
			}
			return null;
		}
		protected bool IsColorsEquals(Color xColor, Color yColor, bool safeCheck) {
			if(safeCheck) {
				if(!xColor.Name.Equals(yColor.Name)) return false;
			}
			return (xColor.A == yColor.A) && (xColor.R == yColor.R) && (xColor.G == yColor.G) && (xColor.B == yColor.B);
		}
		public virtual ColorItem this[int index] {
			get { return List[index] as ColorItem; }
			set {
				if(value == null) return;
				List[index] = value;
			}
		}
		public virtual bool Remove(ColorItem item) {
			if(!Contains(item)) return false;
			List.Remove(item);
			return true;
		}
		public virtual void Insert(int position, ColorItem item) {
			if(Contains(item)) return;
			List.Insert(position, item);
		}
		public virtual bool ContainsColor(Color color, bool safeCheck) {
			return GetItemByColor(color, safeCheck) != null;
		}
		public virtual bool Contains(ColorItem item) {
			return List.Contains(item);
		}
		protected override void OnClear() {
			for(int n = Count - 1; n >= 0; n--) RemoveAt(n);
		}
		public virtual int IndexOf(ColorItem item) {
			return List.IndexOf(item);
		}
		protected override void OnInsert(int position, object value) {
			if(!(value is ColorItem)) return;
			base.OnInsert(position, value);
		}
		protected override void OnInsertComplete(int position, object value) {
			base.OnInsertComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, position));
		}
		protected override void OnRemoveComplete(int position, object value) {
			base.OnRemoveComplete(position, value);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, position));
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			RaiseListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				RaiseListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		#region Changed Event
		protected virtual void RaiseListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(ListChanged != null) ListChanged(this, e);
		}
		public event ListChangedEventHandler ListChanged;
		#endregion
	}
	public delegate void InnerColorPickControlSelectedColorChangingEventHandler(object sender, InnerColorPickControlSelectedColorChangingEventArgs e);
	public class InnerColorPickControlSelectedColorChangingEventArgs : EventArgs {
		bool cancel;
		ColorItem oldColorItem;
		ColorItem newColorItem;
		public InnerColorPickControlSelectedColorChangingEventArgs(ColorItem oldColorItem, ColorItem newColorItem) {
			this.cancel = false;
			this.oldColorItem = oldColorItem;
			this.newColorItem = newColorItem;
		}
		public ColorItem NewColorItem { get { return newColorItem; } }
		public ColorItem OldColorItem { get { return oldColorItem; } }
		public bool Cancel { get { return cancel; } set { cancel = value; } }
	}
	public delegate void InnerColorPickControlSelectedColorChangedEventHandler(object sender, InnerColorPickControlSelectedColorChangedEventArgs e);
	public class InnerColorPickControlSelectedColorChangedEventArgs : EventArgs {
		ColorItem oldColorItem;
		ColorItem newColorItem;
		public InnerColorPickControlSelectedColorChangedEventArgs(ColorItem oldColorItem, ColorItem newColorItem) {
			this.oldColorItem = oldColorItem;
			this.newColorItem = newColorItem;
		}
		public ColorItem NewColorItem { get { return newColorItem; } }
		public ColorItem OldColorItem { get { return oldColorItem; } }
		public Color NewColor { get { return NewColorItem != null ? NewColorItem.Color : Color.Empty; } }
		public Color OldColor { get { return OldColorItem != null ? OldColorItem.Color : Color.Empty; } }
	}
	public abstract class ScrollControllerBase : IDisposable {
		BaseStyleControl owner;
		VScrollBar vScroll;
		bool vScrollVisible;
		bool isRightToLeft;
		Rectangle clientRect, vscrollRect;
		public ScrollControllerBase(BaseStyleControl owner) {
			this.owner = owner;
			this.clientRect = this.vscrollRect = Rectangle.Empty;
			this.vScroll = CreateVScroll();
			this.VScroll.Visible = false;
			this.VScroll.SmallChange = 1;
			this.isRightToLeft = false;
			this.VScroll.LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		protected virtual VScrollBar CreateVScroll() { return new VScrollBar(); }
		public virtual void AddControls(Control container) {
			if(container == null) return;
			container.Controls.Add(VScroll);
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(VScroll);
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		public int VScrollPosition { get { return VScroll.Value; } }
		public VScrollBar VScroll { get { return vScroll; } }
		public BaseStyleControl Owner { get { return owner; } }
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public Rectangle VScrollRect { get { return vscrollRect; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisiblity();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		public bool IsRightToLeft {
			get {
				return isRightToLeft;
			}
			set {
				if(isRightToLeft == value) return;
				isRightToLeft = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			this.vscrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(VScrollVisible) {
				if(!IsRightToLeft) {
					r.Location = new Point(ClientRect.Right - VScrollWidth, ClientRect.Y);
				}
				else {
					r.Location = new Point(ClientRect.X, ClientRect.Y);
				}
				r.Size = new Size(VScrollWidth, ClientRect.Height);
				vscrollRect = r;
			}
		}
		public void UpdateVisiblity() {
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			VScroll.Bounds = VScrollRect;
		}
		int lockLayout = 0;
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisiblity();
				if(ClientRect.IsEmpty) VScroll.SetVisibility(false);
			}
			finally {
				lockLayout--;
			}
		}
		public event EventHandler VScrollValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(VScroll != null) VScroll.Dispose();
			}
		}
	}
	public class ColorItemUtils {
		public static ToolTipControlInfo GetTooltipInfo(ColorItemInfo item, ColorTooltipFormat format) {
			ToolTipControlInfo tooltipInfo = new ToolTipControlInfo();
			if(KnownColorsUtil.IsKnownColor(item.Color)) {
				tooltipInfo.Title = KnownColorsUtil.GetColorName(item.Color);
			}
			tooltipInfo.Text = item.GetHint(format);
			tooltipInfo.Object = item;
			tooltipInfo.ToolTipLocation = ToolTipLocation.BottomRight;
			tooltipInfo.Interval = 500;
			tooltipInfo.SuperTip = GetSuperTip(item, format);
			return tooltipInfo;
		}
		static SuperToolTip GetSuperTip(ColorItemInfo item, ColorTooltipFormat format) {
			SuperToolTip superTip = new SuperToolTip();
			if(KnownColorsUtil.IsKnownColor(item.Color)) {
				superTip.Items.Add(new ToolTipTitleItem() { Text = KnownColorsUtil.GetColorName(item.Color) });
			}
			superTip.Items.Add(new ToolTipItem() { Text = item.GetHint(format) });
			return superTip;
		}
	}
	public class KnownColorsUtil {
		static KnownColorsUtil instance;
		static KnownColorsUtil() {
			instance = new KnownColorsUtil();
		}
		Dictionary<Color, string> dict;
		protected KnownColorsUtil() {
			this.dict = DoLoad();
		}
		protected virtual Dictionary<Color, string> DoLoad() {
			Dictionary<Color, string> res = new Dictionary<Color, string>(new ColorEqualityComparer());
			foreach(PropertyInfo colorProp in GetColorProperties()) {
				Color color = (Color)colorProp.GetValue(null, null);
				if(res.ContainsKey(color)) continue;
				res.Add(color, colorProp.Name);
			}
			return res;
		}
		protected IEnumerable<PropertyInfo> GetColorProperties() {
			Type colorType = typeof(Color);
			return colorType.GetProperties(BindingFlags.Public | BindingFlags.Static).Where(pr => pr.PropertyType == typeof(Color));
		}
		public static bool IsKnownColor(Color color) {
			return instance.dict.ContainsKey(color);
		}
		public static string GetColorName(Color color) {
			return instance.dict[color] as string;
		}
		public class ColorEqualityComparer : EqualityComparer<Color> {
			public override bool Equals(Color left, Color right) {
				return left.ToArgb() == right.ToArgb();
			}
			public override int GetHashCode(Color obj) {
				return obj.ToArgb();
			}
		}
	}
}
