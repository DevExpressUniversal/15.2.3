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
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPrinting;
using System.Collections;
namespace DevExpress.XtraEditors.CustomEditor {
	public interface IAnyControlEdit {
		event EventHandler EditValueChanged;
		object EditValue { get; set; }
		Size CalcSize(Graphics g);
		bool SupportsDraw { get; }
		bool AllowBorder { get; }
		void Draw(GraphicsCache cache, AnyControlEditViewInfo viewInfo);
		void SetupAsDrawControl();
		void SetupAsEditControl();
		string GetDisplayText(object EditValue);
		bool IsNeededKey(KeyEventArgs e);
		bool AllowClick(Point point);
		bool AllowBitmapCache { get; }
	}
	[UserRepositoryItem("Register")]
	public class RepositoryItemAnyControl : RepositoryItem {
		IAnyControlEdit control;
		IAnyControlEdit drawControlInstance;
		static RepositoryItemAnyControl() { Register(); }
		internal const string EditorName = "AnyControlEdit";
		public static void Register() {
			EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(AnyControlEdit),
				typeof(RepositoryItemAnyControl), typeof(AnyControlEditViewInfo),
				new AnyControlEditPainter(), true, null));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				IAnyControlEdit control = Control;
				Control = null;
				if(AllowDisposeControl) Dispose(control);
				DestroyCache();
			}
			base.Dispose(disposing);
		}
		protected internal void DestroyCache() {
			if(bitmapCacheInfo == null) return;
			foreach(var entry in bitmapCacheInfo) {
				DestroyCacheElement(entry.Value);
			}
			bitmapCacheInfo.Clear();
		}
		void DestroyCacheElement(object item) {
			IList list = item as IList;
			if(list != null) {
				foreach(BitmapCacheInfo c in list) c.Dispose();
			}
			else {
				((BitmapCacheInfo)item).Dispose();
			}
		}
		public override DevExpress.XtraPrinting.IVisualBrick GetBrick(PrintCellHelperInfo info) {
			ITextBrick brick = CreateTextBrick(info);
			return brick;
		}
		public static string GetBasicDisplayText(object editValue) {
			if(editValue == null || object.ReferenceEquals(editValue, DBNull.Value)) return "";
			return editValue.ToString();
		}
		public override string EditorTypeName { get { return EditorName; } }
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			RepositoryItemAnyControl source = item as RepositoryItemAnyControl;
			if(source == null) return;
			if(OwnerEdit != null) {
				Control = source.CreateControlInstance();
			} else
				Control = source.Control;
		}
		public void RefreshControl() {
			if(control != null) {
				OnControlRelease(control);
				OnControlSubscribe(control);
			}
			OnControlChanged(); 
		}
		public IAnyControlEdit Control {
			get { return control; }
			set {
				if(!(value is Control)) value = null;
				if(control == value) return;
				if(control != null) OnControlRelease(control);
				control = value;
				if(control != null) OnControlSubscribe(control);
				OnControlChanged();
			}
		}
		public override DevExpress.XtraEditors.Controls.BorderStyles BorderStyle {
			get {
				if(Control != null && !Control.AllowBorder) return BorderStyles.NoBorder;
				return base.BorderStyle;
			}
			set {
				base.BorderStyle = value;
			}
		}
		protected internal override bool NeededKeysContains(Keys key) {
			if(base.NeededKeysContains(key)) return true;
			switch(key) {
				case Keys.F2:
				case Keys.A:
				case Keys.Add:
				case Keys.B:
				case Keys.Back:
				case Keys.C:
				case Keys.Clear:
				case Keys.D:
				case Keys.D0:
				case Keys.D1:
				case Keys.D2:
				case Keys.D3:
				case Keys.D4:
				case Keys.D5:
				case Keys.D6:
				case Keys.D7:
				case Keys.D8:
				case Keys.D9:
				case Keys.Decimal:
				case Keys.Delete:
				case Keys.Divide:
				case Keys.E:
				case Keys.End:
				case Keys.F:
				case Keys.F20:
				case Keys.G:
				case Keys.H:
				case Keys.Home:
				case Keys.I:
				case Keys.Insert:
				case Keys.J:
				case Keys.K:
				case Keys.L:
				case Keys.Left:
				case Keys.M:
				case Keys.Multiply:
				case Keys.N:
				case Keys.NumPad0:
				case Keys.NumPad1:
				case Keys.NumPad2:
				case Keys.NumPad3:
				case Keys.NumPad4:
				case Keys.NumPad5:
				case Keys.NumPad6:
				case Keys.NumPad7:
				case Keys.NumPad8:
				case Keys.NumPad9:
				case Keys.Alt:
				case (Keys.RButton | Keys.ShiftKey):
				case Keys.O:
				case Keys.Oem8:
				case Keys.OemBackslash:
				case Keys.OemCloseBrackets:
				case Keys.Oemcomma:
				case Keys.OemMinus:
				case Keys.OemOpenBrackets:
				case Keys.OemPeriod:
				case Keys.OemPipe:
				case Keys.Oemplus:
				case Keys.OemQuestion:
				case Keys.OemQuotes:
				case Keys.OemSemicolon:
				case Keys.Oemtilde:
				case Keys.P:
				case Keys.Q:
				case Keys.R:
				case Keys.Right:
				case Keys.S:
				case Keys.Space:
				case Keys.Subtract:
				case Keys.T:
				case Keys.U:
				case Keys.V:
				case Keys.W:
				case Keys.X:
				case Keys.Y:
				case Keys.Z:
					return true;
			}
			return false;
		}
		protected virtual void OnControlChanged() {
			OnPropertiesChanged();
		}
		protected override void OnPropertiesChanged() {
			DestroyCache();
			base.OnPropertiesChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new AnyControlEdit OwnerEdit { get { return base.OwnerEdit as AnyControlEdit; } }
		protected virtual void OnControlSubscribe(IAnyControlEdit control) {
			if(OwnerEdit != null) OwnerEdit.OnControlSubscribe(control);
		}
		public override BaseEdit CreateEditor() {
			var editor =  base.CreateEditor() as AnyControlEdit;
			if(Control != null) {
				editor.Properties.Control = CreateControlInstance();
				editor.Properties.AllowDisposeControl = true;
			}
			return editor;
		}
		protected virtual void OnControlRelease(IAnyControlEdit control) {
			Dispose(drawControlInstance);
			this.drawControlInstance = null;
			if(OwnerEdit != null) OwnerEdit.OnControlRelease(control);
		}
		protected internal IAnyControlEdit GetDrawControlInstance() {
			if(OwnerEdit != null) return Control;
			EnsureDrawControlInstance();
			return drawControlInstance;
		}
		internal IAnyControlEdit CreateControlInstance() {
			ICloneable cloneable = Control as ICloneable;
			if(cloneable != null) return cloneable.Clone() as IAnyControlEdit;
			if(Control != null) return Activator.CreateInstance(Control.GetType()) as IAnyControlEdit;
			return null;
		}
		protected internal void EnsureDrawControlInstance() {
			if(drawControlInstance == null && Control != null) {
				drawControlInstance = CreateControlInstance();
				drawControlInstance.SetupAsDrawControl();
			}
		}
		void Dispose(IAnyControlEdit control) {
			IDisposable disposable = control as IDisposable;
			if(disposable != null) disposable.Dispose();
		}
		internal bool AllowDisposeControl { get; set; }
		FixedDictionary<string, object> bitmapCacheInfo;
		class BitmapCacheInfo : IDisposable {
			public object EditValue;
			public Bitmap Bitmap;
			public void Dispose() {
				if(Bitmap != null) Bitmap.Dispose();
				Bitmap = null;
			}
		}
		protected internal virtual Bitmap GetCachedBitmap(string key, object editValue) {
			if(bitmapCacheInfo == null) return null;
			object value = null;
			try {
				if(!bitmapCacheInfo.TryGetValue(key, out value)) return null;
				if(value is IList) {
					foreach(BitmapCacheInfo cache in (IList)value) {
						if(object.Equals(cache.EditValue, editValue)) return cache.Bitmap;
					}
				}
				else {
					BitmapCacheInfo cache = (BitmapCacheInfo)value;
					if(object.Equals(cache.EditValue, editValue)) return cache.Bitmap;
				}
			}
			catch { }
			return null;
		}
		protected internal virtual void StoreCachedBitmap(string key, object editValue, Bitmap bitmap) {
			if(bitmapCacheInfo == null) {
				bitmapCacheInfo = new FixedDictionary<string, object>(50);
				bitmapCacheInfo.DictionaryChanged += OnCacheChanged;
			}
			try {
				object value;
				var cache = new BitmapCacheInfo() { Bitmap = bitmap, EditValue = editValue };
				if(!bitmapCacheInfo.TryGetValue(key, out value)) {
					bitmapCacheInfo[key] = cache;
					return;
				}
				IList list = value as IList;
				if(list == null) {
					list = new ArrayList();
					list.Add(value);
					bitmapCacheInfo[key] = list;
				}
				list.Add(cache);
			}
			catch {
			}
		}
		void OnCacheChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action != CollectionChangeAction.Remove) return;
			DestroyCacheElement(e.Element);
		}
	}
	public class AnyControlEdit : BaseEdit {
		static AnyControlEdit() { RepositoryItemAnyControl.Register(); }
		public override string EditorTypeName { get { return RepositoryItemAnyControl.EditorName; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemAnyControl Properties { get { return base.Properties as RepositoryItemAnyControl; } }
		public override bool IsNeededKey(KeyEventArgs e) {
			return base.IsNeededKey(e) || (Properties.Control != null && Properties.Control.IsNeededKey(e));
		}
		public override bool AllowMouseClick(Control control, Point p) {
			p = PointToClient(p);
			if(base.AllowMouseClick(control, p)) return true;
			if(GetChildAtPoint(p) != null) return true;
			if(Properties.Control != null && Properties.Control.AllowClick(p)) return true;
			return false;
		}
		protected override bool ProcessKeyPreview(ref System.Windows.Forms.Message m) {
			if(InplaceType != DevExpress.XtraEditors.Controls.InplaceType.Standalone) {
				return ProcessKeyMessage(ref m);
			}
			return base.ProcessKeyPreview(ref m);
		}
		internal void OnControlSubscribe(IAnyControlEdit control) {
			Control c = control as Control;
			if(c != null) {
				if(!DesignMode) {
					c.Dock = DockStyle.None;
					c.Parent = this;
					c.ParentChanged += OnControl_ParentChanged;
					c.Visible = true;
				}
			}
			control.EditValueChanged += OnControlEditValueChanged;
		}
		internal void OnControlRelease(IAnyControlEdit control) {
			Control c = control as Control;
			if(c != null) {
				c.ParentChanged -= OnControl_ParentChanged;
			}
			if(c != null && Controls.Contains(c)) {
				if(!DesignMode) c.Parent = null;
			}
			control.EditValueChanged -= OnControlEditValueChanged;
		}
		void OnControlEditValueChanged(object sender, EventArgs e) {
			if(Properties.Control != null) EditValue = Properties.Control.EditValue;
		}
		void OnControl_ParentChanged(object sender, EventArgs e) {
			Control c = sender as Control;
			if(c.Parent != this && Properties.Control == c) c.Parent = this;
		}
		protected internal override void LayoutChanged() {
			base.LayoutChanged();
			UpdateControlBounds();
		}
		protected override void OnAfterUpdateViewInfo() {
			base.OnAfterUpdateViewInfo();
			UpdateControlBounds();
		}
		protected void UpdateControlBounds() {
			Control c = Properties.Control as Control;
			if(c != null) {
				if(!ViewInfo.IsReady)
					c.Bounds = ClientRectangle;
				else
					c.Bounds = ViewInfo.ContentRect;
			}
		}
		[Browsable(false)]
		public override bool IsEditorActive {
			get {
				if(!this.Enabled)
					return false;
				IContainerControl container = GetContainerControl();
				if(container == null) return EditorContainsFocus;
				return container.ActiveControl == this || Contains(container.ActiveControl);
			}
		}
		protected internal override Control InnerControl { get { return Properties.Control as Control; } }
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			Control c = Properties.Control as Control;
			if(c != null) {
				if(ContainsFocus && !c.ContainsFocus) c.Focus();
			}
		}
	}
	public class AnyControlEditViewInfo : BaseEditViewInfo {
		public AnyControlEditViewInfo(RepositoryItem item) : base(item) { 
		}
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
		}
		public override bool AllowDrawFocusRect {
			get {
				return false;
			}
			set {
			}
		}
		protected override Size CalcContentSize(Graphics g) {
			Size res = base.CalcContentSize(g);
			if(DrawControlInstance == null) return res;
			Size editor = DrawControlInstance.CalcSize(g);
			res.Height = Math.Max(editor.Height, res.Height);
			res.Width = Math.Max(editor.Width, res.Width);
			return res;
		}
		public IAnyControlEdit DrawControlInstance { 
			get { return Item.GetDrawControlInstance(); } }
		public new RepositoryItemAnyControl Item { get { return base.Item as RepositoryItemAnyControl; } }
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			if(DrawControlInstance != null) {
				DrawControlInstance.EditValue = EditValue;
				SetDisplayText(DrawControlInstance.GetDisplayText(EditValue));
			}
		}
		Bitmap drawBitmap = null;
		public override void Dispose() {
			if(drawBitmap != null) drawBitmap.Dispose();
			drawBitmap = null;
			base.Dispose();
		}
		public Bitmap EnsureBitmap(Size size) {
			if(drawBitmap != null && (drawBitmap.Width < size.Width || drawBitmap.Height < size.Height)) {
				drawBitmap.Dispose();
				drawBitmap = null;
			}
			if(drawBitmap == null) {
				drawBitmap = new Bitmap(size.Width, size.Height);
			}
			return drawBitmap;
		}
		string GetViewInfoKey() {
			return string.Format("{0}|{1}|{2}|{3}", PaintAppearance.BackColor, PaintAppearance.ForeColor, ContentRect.Size, EditValue);
		}
		protected internal virtual Bitmap GetCachedBitmapByViewInfo() {
			var key = GetViewInfoKey();
			return Item.GetCachedBitmap(key, EditValue);
		}
		protected internal virtual void StoreCachedBitmapByViewInfo(Bitmap bitmap) {
			if(Item.Control != null && !Item.Control.AllowBitmapCache) return;
			if(bitmap == drawBitmap) {
				bitmap = drawBitmap.Clone(new Rectangle(Point.Empty, ContentRect.Size), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			}
			Item.StoreCachedBitmap(GetViewInfoKey(), EditValue, bitmap);
		}
	}
	public class AnyControlEditPainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			var viewInfo = info.ViewInfo as AnyControlEditViewInfo;
			info.ViewInfo.PaintAppearance.DrawBackground(info.Cache, info.Bounds);
			var drawControl = viewInfo.DrawControlInstance;
			if(drawControl == null) {
				DrawEmptyContent(info); 
				return;
			}
			if(viewInfo.OwnerEdit != null) return;
			if(drawControl.SupportsDraw) {
				drawControl.Draw(info.Cache, viewInfo);
			}
			else {
				DrawAsBitmap(info.Cache, drawControl, viewInfo);
			}
		}
		void DrawEmptyContent(ControlGraphicsInfoArgs info) {
			info.Cache.FillRectangle(Brushes.Red, info.Bounds);
			info.ViewInfo.PaintAppearance.DrawString(info.Cache, "Empty", info.Bounds);
		}
		void DrawAsBitmap(GraphicsCache cache, IAnyControlEdit drawControl, AnyControlEditViewInfo viewInfo) {
			Control c = drawControl as Control;
			if(c == null) return;
			Bitmap bitmap = viewInfo.GetCachedBitmapByViewInfo();
			if(bitmap == null) {
				drawControl.EditValue = viewInfo.EditValue;
				c.Size = viewInfo.ContentRect.Size;
				c.BackColor = GetNonTransparentColor(viewInfo.PaintAppearance.BackColor);
				c.ForeColor = viewInfo.PaintAppearance.ForeColor;
				bitmap = viewInfo.EnsureBitmap(viewInfo.ContentRect.Size);
				c.DrawToBitmap(bitmap, new Rectangle(Point.Empty, viewInfo.ContentRect.Size));
				viewInfo.StoreCachedBitmapByViewInfo(bitmap);
			}
			cache.Paint.DrawImage(cache.Graphics, bitmap, viewInfo.ContentRect, new Rectangle(Point.Empty, viewInfo.ContentRect.Size), true);
		}
		Color GetNonTransparentColor(Color c) {
			return Color.FromArgb(c.R, c.G, c.B);
		}
	}
	#region FixedDictionary
	internal class FixedDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
		Dictionary<TKey, TValue> dictionary;
		Queue<TKey> queue;
		int size;
		public event CollectionChangeEventHandler DictionaryChanged;
		public FixedDictionary(int maxSize) {
			this.size = maxSize;
			this.dictionary = new Dictionary<TKey, TValue>(maxSize + 1);
			this.queue = new Queue<TKey>(maxSize);
		}
		public virtual void Add(TKey key, TValue value) {
			this.dictionary.Add(key, value);
			if(queue.Count == size) {
				var keyToRemove = queue.Dequeue();
				RaiseChanged(CollectionChangeAction.Remove, dictionary[keyToRemove]);
				dictionary.Remove(keyToRemove);
			}
			queue.Enqueue(key);
		}
		public void Clear() {
			this.dictionary.Clear();
			this.queue.Clear();
		}
		void RaiseChanged(CollectionChangeAction changeAction, TValue value) {
			if(DictionaryChanged == null) return;
			CollectionChangeEventArgs c = new CollectionChangeEventArgs(changeAction, value);
			DictionaryChanged(this, c);
		}
		public virtual bool Remove(TKey key) {
			var value = dictionary[key];
			if(dictionary.Remove(key)) {
				Queue<TKey> newQueue = new Queue<TKey>(size);
				foreach(TKey item in queue)
					if(!dictionary.Comparer.Equals(item, key))
						newQueue.Enqueue(item);
				queue = newQueue;
				RaiseChanged(CollectionChangeAction.Remove, value);
				return true;
			}
			else
				return false;
		}
		public virtual bool ContainsKey(TKey key) {
			return dictionary.ContainsKey(key);
		}
		public virtual bool TryGetValue(TKey key, out TValue value) {
			return dictionary.TryGetValue(key, out value);
		}
		ICollection<TKey> IDictionary<TKey, TValue>.Keys { get { return dictionary.Keys; } }
		ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return dictionary.Values; } }
		public virtual TValue this[TKey key] {
			get {
				return dictionary[key];
			}
			set {
				if(dictionary.ContainsKey(key)) {
					dictionary[key] = value;
				}
				else {
					Add(key, value);
				}
			}
		}
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
			throw new NotImplementedException();
		}
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
			throw new NotImplementedException();
		}
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
			throw new NotImplementedException();
		}
		public virtual int Count { get { return dictionary.Count; } }
		public virtual bool IsReadOnly { get { return false; } }
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
			throw new NotImplementedException();
		}
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return dictionary.GetEnumerator();
		}
	}
	#endregion 
}
