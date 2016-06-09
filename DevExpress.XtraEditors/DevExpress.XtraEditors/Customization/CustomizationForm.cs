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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraEditors.Customization {
	[ToolboxItem(false)]
	public class CustomizationListBoxBase : ListBoxControl {
		const int EmptyHintIndent = 5;
		Point downPoint = Point.Empty;
		public CustomizationListBoxBase() {
		}
		object pressedItem;
		protected internal virtual object PressedItem {
			get { return pressedItem; }
			set {
				if(pressedItem == value) return;
				InvalidateObject(pressedItem);				
				pressedItem = value;
				InvalidateObject(pressedItem);
			}
		}
		protected virtual void AddItems(ArrayList list) { }
		public virtual int GetItemHeight() { return 12; }
		protected virtual bool CanPressItem(object item) { return true; }
		protected virtual void DrawItemObject(GraphicsCache cache, int index, Rectangle bounds, DrawItemState itemState) { }
		protected virtual Point PointToView(Point p) { return p; }
		protected override void OnLoaded() {
			base.OnLoaded();
			ItemHeight = GetItemHeight();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(ViewInfo != null) ViewInfo.FocusRect = Rectangle.Empty;
			base.OnPaint(e);
			DrawHintForEmptyList(e);
		}
		protected virtual void DrawHintForEmptyList(PaintEventArgs e) {
			if(ItemCount != 0 || string.IsNullOrEmpty(GetHintCaptionForEmptyList())) return;
			AppearanceObject app = GetHintForEmptyListAppearance();
			if(app != null) {
				using(FrozenAppearance appearance = new FrozenAppearance(app)) {
					if(appearance.ForeColor == Color.Transparent)
						appearance.ForeColor = ViewInfo.PaintAppearance.ForeColor;
					using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
						appearance.DrawString(cache, GetHintCaptionForEmptyList(),
							new Rectangle(EmptyHintIndent, EmptyHintIndent, this.Width - EmptyHintIndent * 2, this.Height - EmptyHintIndent * 2),
							appearance.GetStringFormat(new TextOptions(HorzAlignment.Near, VertAlignment.Top, WordWrap.Wrap, Trimming.Default)));
					}
				}
			}
		}
		protected virtual string GetHintCaptionForEmptyList() {
			return "";
		}
		protected virtual AppearanceObject GetHintForEmptyListAppearance() {
			return null;
		}
		protected internal override void RaiseDrawItem(ListBoxDrawItemEventArgs e) {
			e.Handled = true;
			if(e.Index < 0 || e.Index >= Items.Count) return;
			DrawItemObject(e.Cache, e.Index, e.Bounds, e.State);
		}
		protected object ItemByPoint(Point pt) {
			int i = IndexFromPoint(pt);
			if(i >= 0 && i < Items.Count) return GetItemValue(i);
			return null;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Capture = false;
			base.OnMouseDown(e);
			downPoint = new Point(e.X, e.Y);
			object item = ItemByPoint(downPoint);
			if(item == null || !CanPressItem(item)) return;
			PressedItem = item;
			if((e.Button & MouseButtons.Left) != 0) {
				Capture = true;
			}
			if((e.Button & MouseButtons.Right) != 0) {
				ShowItemMenu(item);
			}
		}
		protected virtual void ShowItemMenu(object item) { }
		protected override void OnMouseUp(MouseEventArgs e) {
			if(!IsDragging) {
				int index = IndexFromPoint(new Point(e.X, e.Y));
				if(index > 0) 
					Handler.ControlState.FocusedIndex = index;
			}
			base.OnMouseUp(e);
			Capture = false;
			PressedItem = null;
			if(IsDragging) {
				Point pp = PointToView(new Point(e.X, e.Y));
				MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, pp.X, pp.Y, e.Delta);
				EndDrag(me);
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			base.OnMouseDoubleClick(e);
			downPoint = new Point(e.X, e.Y);
			object item = ItemByPoint(downPoint);
			if(item == null || !CanPressItem(item)) return;
			DoShowItem(item);
		}
		protected virtual void DoShowItem(object item) { }
		protected virtual bool IsDragging { get { return false; } }
		protected virtual void DoDragDrop(object dragItem, Point p) { }
		protected virtual void EndDrag(MouseEventArgs e) { }
		protected virtual void CancelDrag() { }
		protected virtual void DoDragging(MouseEventArgs e) { }
		protected override void OnMouseMove(MouseEventArgs e) {
			Point p = new Point(e.X, e.Y);
			Point pp = PointToView(p);
			MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks,
				pp.X, pp.Y, e.Delta);
			if(IsDragging) {
				if(me.Button != MouseButtons.Left)
					CancelDrag();
				else
					DoDragging(me);
				return;
			}
			Size dragSize = System.Windows.Forms.SystemInformation.DragSize;
			if(PressedItem != null) {
				if((e.Button & MouseButtons.Left) != 0) {
					if(Math.Abs(p.X - downPoint.X) > dragSize.Width ||
						Math.Abs(p.Y - downPoint.Y) > dragSize.Height) {
						DoDragDrop(PressedItem, pp);
						if(IsDragging)
							Capture = true;
						else
							PressedItem = null;
						return;
					}
				}
			}
			base.OnMouseMove(e);
		}
		public void InvalidateObject(object obj) {
			if(obj == null) return;
			Invalidate();
		}
		public virtual void Populate() {
			BeginUpdate();
			try {
				int topIndex = this.TopIndex;
				Items.Clear();
				ArrayList list = new ArrayList();
				AddItems(list);
				list.Sort(CreateComparer());
				foreach(object item in list)
					Items.Add(item);
				if(topIndex > -1)
					TopIndex = topIndex;
			} finally {
				EndUpdate();
			}
		}
		protected virtual IComparer CreateComparer() {
			return new DefaultComparer();
		}
		class DefaultComparer : IComparer {
			int IComparer.Compare(object a, object b) {
				return Comparer.Default.Compare(a.ToString(), b.ToString());
			}
		}
	}
	[ToolboxItem(false)]
	public class CustomCustomizationListBoxBase : CustomizationListBoxBase {
		CustomizationFormBase form;
		public CustomCustomizationListBoxBase(CustomizationFormBase form) {
			this.form = form;
			this.Dock = DockStyle.Fill;
		}
		public CustomizationFormBase CustomizationForm {
			get { return form; }
		}
		protected override Point PointToView(Point p) {
			return CustomizationForm.ControlOwner.PointToClient(PointToScreen(p));
		}
	}
	[ToolboxItem(false)]
	public abstract class CustomizationFormBase : XtraForm {
		public const int DefaultLocation = -10000;
		public static Point DefaultPoint = new Point(DefaultLocation, DefaultLocation);
		CustomizationListBoxBase activeListBox;
		bool isUpdateListBox = false;
		public CustomizationFormBase() {
			Visible = false;
			Text = FormCaption;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			MinimizeBox = false;
			MinimumSize = MinFormSize;
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
		}
		protected static Size MinFormSize { get { return new Size(200, 100); } }
		protected override void Dispose(bool disposing) {
			StopCustomization();
			base.Dispose(disposing);
		}
		protected virtual void InitCustomizationForm() {
			CreateListBox();
			UpdateListBox();
			UpdateSize();
		}
		protected virtual void StopCustomization() { }
		public abstract Control ControlOwner { get; }
		protected abstract CustomizationListBoxBase CreateCustomizationListBox();
		protected abstract string FormCaption { get; }
		protected virtual void OnFormClosed() {
			StopCustomization();
		}
		protected abstract Rectangle CustomizationFormBounds { get; }
		protected virtual UserLookAndFeel ControlOwnerLookAndFeel { get { return UserLookAndFeel.Default; } }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
		}
		protected override void OnClosed(EventArgs e) {
			OnFormClosed();
			base.OnClosed(e);
		}
		protected virtual bool AllowSearchBox { get { return false; } }
		protected virtual void CreateListBox() {
			SetActiveListBox(CreateCustomizationListBox());
			ActiveListBox.Populate();
			ActiveListBox.Dock = DockStyle.Fill;
			Controls.Add(ActiveListBox);
			AddSearchControl(ActiveListBox, this);
		}
		protected SearchControl AddSearchControl(ISearchControlClient client, Control parent) {
			if(AllowSearchBox) {
				SearchControl searchBox = CreateSearchControl();
				searchBox.Properties.Client = client;
				searchBox.Dock = DockStyle.Top;
				PanelControl separator = new PanelControl();
				separator.BorderStyle = BorderStyles.NoBorder;
				separator.Height = 6;
				separator.Dock = DockStyle.Top;
				parent.Controls.Add(separator);
				parent.Controls.Add(searchBox);
				parent.Padding = new Padding(6);
				searchBox.Properties.NullValuePrompt = Localizer.Active.GetLocalizedString(StringId.SearchForColumn);
				searchBox.Properties.FindDelay = 100;
				return searchBox;
			}
			return null;
		}
		protected virtual SearchControl CreateSearchControl() {
			return new SearchControl();
		}
		public void RefreshItems() {
			ActiveListBox.Populate();
		}
		public virtual CustomizationListBoxBase ActiveListBox { get { return activeListBox; } }
		protected void SetActiveListBox(CustomizationListBoxBase value) {
			this.activeListBox = value;
		}
		public virtual void CheckAndUpdate() {
			if(ActiveListBox.ItemHeight != ActiveListBox.GetItemHeight()) {
				ActiveListBox.ItemHeight = ActiveListBox.GetItemHeight();
				UpdateSize();
			}
			UpdateListBox();
			Refresh();
		}
		public void UpdateSize() {
			if(CustomizationFormBounds.Size.IsEmpty)
				SetDefaultFormSize();
			else
				Size = CustomizationFormBounds.Size;
		}
		protected virtual void SetDefaultFormSize() {
			this.ClientSize = new Size(200, ActiveListBox.GetItemHeight() * 7 + 4);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
		}
		protected virtual Point CalcLocation(Point location) {
			if(location.X == DefaultLocation) {
				if(CustomizationFormBounds.Location.IsEmpty && ControlOwner.Parent != null) {
					location = ControlOwner.Parent.PointToScreen(new Point(ControlOwner.Parent.ClientRectangle.Right,
						ControlOwner.Parent.ClientRectangle.Bottom));
					location.Offset(-this.Size.Width, -this.Size.Height);
				} else
					location = CustomizationFormBounds.Location;
				location = DevExpress.Utils.ControlUtils.CalcFormLocation(location, Size);
			}
			return location;
		}
		public void ShowCustomization(Control parentControl, Point location) {
			if(parentControl != null)
				ShowCustomization(parentControl);
			else ShowCustomization(location);
		}
		public virtual void ShowCustomization(Point location) {
			Form controlOwnerForm = ControlOwner.FindForm();
			if(controlOwnerForm != null) {
				if(controlOwnerForm.MdiParent != null) {
					if(Owner != null && Owner != controlOwnerForm.MdiParent)
						Owner.RemoveOwnedForm(this);
					controlOwnerForm.MdiParent.AddOwnedForm(this);
				}
				else controlOwnerForm.AddOwnedForm(this);
			}
			ShowCustomizationCore(location, true);
		}
		public virtual void ShowCustomization(Control parentControl) {
			if(parentControl == null) {
				ShowCustomization(CustomizationFormBounds.Location);
				return;
			}
			FormBorderStyle = FormBorderStyle.None;
			TopLevel = false;
			Parent = parentControl;
			Dock = DockStyle.Fill;
			ShowCustomizationCore(Point.Empty, false);
		}
		protected virtual void ShowCustomizationCore(Point location, bool calcLocation, bool allowChangeFocus) {
			InitCustomizationForm();
			if(calcLocation) {
				Location = CalcLocation(location);
			}
			this.Visible = true;
			if(allowChangeFocus) ControlOwner.Focus();
		}
		protected virtual void ShowCustomizationCore(Point location, bool calcLocation) {
			ShowCustomizationCore(location, calcLocation, true);
		}
		public virtual void UpdateListBox() {
			if(isUpdateListBox) return;
			isUpdateListBox = true;
			try {
				LookAndFeel.Assign(ControlOwnerLookAndFeel);
				ActiveListBox.LookAndFeel.Assign(ControlOwnerLookAndFeel);
				ActiveListBox.Populate();
			}
			finally {
				isUpdateListBox = false;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object PressedItem {
			get { return ActiveListBox.PressedItem; }
			set {
				if(PressedItem != value) 
					ActiveListBox.PressedItem = value;
			}
		}
	}
}
