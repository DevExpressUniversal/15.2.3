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
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
using System.Drawing.Design;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors.ToolboxIcons;
namespace DevExpress.XtraEditors.ViewInfo {
	public class CheckedListBoxViewInfo : BaseListBoxViewInfo {
		CheckObjectPainter checkMarkPainter;
		public CheckedListBoxViewInfo(CheckedListBoxControl listBox)
			: base(listBox) {
			checkMarkPainter = null;
		}
		protected static int CheckMarkIndent { get { return 2; } }
		public int FullMarkWidth { get { return CheckMarkSize.Width + 2 * CheckMarkIndent; } }
		public CheckObjectPainter CheckMarkPainter {
			get {
				if(checkMarkPainter == null) UpdatePainters();
				return checkMarkPainter;
			}
		}
		public Size CheckMarkSize {
			get {
				CheckObjectViewInfo viewInfo = new CheckObjectViewInfo(true, CheckState.Unchecked, OwnerControl.CheckStyle, OwnerControl.PictureChecked, OwnerControl.PictureUnchecked, OwnerControl.PictureGrayed);
				return CheckMarkPainter.CalcObjectMinBounds(viewInfo.InfoArgs).Size;
			}
		}
		protected internal override int CalcItemMinHeight() {
			return Math.Max(base.CalcItemMinHeight(), CheckMarkSize.Height + 2);
		}
		protected override int CalcItemHeight(Graphics g, int itemIndex) {
			return Math.Max(base.CalcItemHeight(g, itemIndex), CheckMarkSize.Height + 2);
		}
		public override int CalcBestColumnWidth() {
			int result = base.CalcBestColumnWidth();
			result += FullMarkWidth;
			return result;
		}
		protected override BaseListBoxViewInfo.ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			CheckedItemInfo itemInfo = new CheckedItemInfo(OwnerControl, OffsetBounds(bounds, FullMarkWidth), item, text, index, index == -1 ? CheckState.Unchecked : OwnerControl.GetItemCheckState(index), index == -1 ? true : OwnerControl.GetItemEnabledCore(index), OwnerControl.CheckStyle, OwnerControl.PictureChecked, OwnerControl.PictureUnchecked, OwnerControl.PictureGrayed);
			itemInfo.Bounds = bounds;
			UpdateCheckMarkBounds(itemInfo.CheckArgs, bounds);
			return itemInfo;
		}
		protected Rectangle OffsetBounds(Rectangle bounds, int xOffset) {
			Rectangle newBounds = new Rectangle(bounds.Left + xOffset, bounds.Top, bounds.Width - xOffset, bounds.Height);
			if(OwnerControl.IsRightToLeft) newBounds.X -= xOffset;
			return newBounds;
		}
		protected void UpdateCheckMarkBounds(CheckObjectInfoArgs checkArgs, Rectangle bounds) {
			UpdateBoundsCore(checkArgs, CheckMarkPainter, bounds, CheckMarkSize, FullMarkWidth, CheckMarkIndent);
			checkArgs.LookAndFeel = OwnerControl.LookAndFeel;
		}
		protected void UpdateBoundsCore(ObjectInfoArgs args, ObjectPainter painter, Rectangle bounds, Size size, int fullWidth, int indent) {
			args.Bounds = new Rectangle(new Point(OwnerControl.IsRightToLeft ? bounds.Right - fullWidth + indent
				: bounds.Left + indent, bounds.Top + (bounds.Height - size.Height) / 2), size);
			painter.CalcObjectBounds(args);
		}
		protected override void UpdatePainters() {
			checkMarkPainter = CheckPainterHelper.GetPainter(OwnerControl.LookAndFeel);
			base.UpdatePainters();
		}
		protected internal bool GetItemEnabled(int index) {
			ItemInfo info = GetItemByIndex(index);
			if(info == null) return true;
			return info.Enabled;
		}
		public override void UpdateItemState(BaseListBoxViewInfo.ItemInfo itemInfo) {
			base.UpdateItemState(itemInfo);
			if((itemInfo.Index > -1 && itemInfo.Index < ItemCount) && OwnerControl.GetItemChecked(itemInfo.Index))
				itemInfo.State |= DrawItemState.Checked;
		}
		public new CheckedItemInfo GetItemByIndex(int index) {
			return (CheckedItemInfo)base.GetItemByIndex(index);
		}
		protected new BaseCheckedListBoxControl OwnerControl { get { return base.OwnerControl as BaseCheckedListBoxControl; } }
		public class CheckedItemInfo : ItemInfo {
			CheckObjectViewInfo checkViewInfo;
			public CheckedItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index, CheckState checkState, bool enabled)
				: this(ownerControl, rect, item, text, index, checkState, enabled, CheckStyles.Standard, null, null, null) {
			}
			public CheckedItemInfo(BaseListBoxControl ownerControl, Rectangle rect, object item, string text, int index, CheckState checkState, bool enabled, CheckStyles checkStyle, Image pictureChecked, Image pictureUnchecked, Image pictureGrayed)
				: base(ownerControl, rect, item, text, index) {
				checkViewInfo = new CheckObjectViewInfo(enabled, checkState, checkStyle, pictureChecked, pictureUnchecked, pictureGrayed);
			}
			public override bool Enabled { get { return checkViewInfo.Enabled; } }
			public CheckObjectInfoArgs CheckArgs { get { return CheckViewInfo.InfoArgs; } }
			protected internal CheckObjectViewInfo CheckViewInfo { get { return checkViewInfo; } }
		}		
	}
	public class StyleObjectViewInfo {
		StyleObjectInfoArgs infoArgs;
		public StyleObjectViewInfo(bool enabled) {
			infoArgs = CreateStyleObjectInfoArgs();
			if(!enabled)
				infoArgs.State |= ObjectState.Disabled;
		}
		public virtual bool HitTest(Point point) {
			if(infoArgs == null) return false;
			return infoArgs.Bounds.Contains(point);
		}
		public virtual bool Enabled { get { return (infoArgs.State & DevExpress.Utils.Drawing.ObjectState.Disabled) == 0; } }
		public StyleObjectInfoArgs InfoArgs { get { return infoArgs; } }
		protected virtual StyleObjectInfoArgs CreateStyleObjectInfoArgs() {
			return new StyleObjectInfoArgs(null);
		}
		public virtual void Paint(ObjectPainter painter, GraphicsCache cache, AppearanceObject appearance) {
			infoArgs.Cache = cache;
			infoArgs.BackAppearance = appearance;
			try {
				painter.DrawObject(infoArgs);
			} finally {
				infoArgs.Cache = null;
			}
		}
	}
	public class CheckObjectViewInfo : StyleObjectViewInfo {
		public CheckObjectViewInfo(bool enabled, CheckState checkState, CheckStyles checkStyle, Image pictureChecked, Image pictureUnchecked, Image pictureGrayed)
			: base(enabled) {
			InfoArgs.CheckState = checkState;
			InfoArgs.CheckStyle = checkStyle;
			InfoArgs.PictureChecked = pictureChecked;
			InfoArgs.PictureUnchecked = pictureUnchecked;
			InfoArgs.PictureGrayed = pictureGrayed;
			InfoArgs.GlyphAlignment = HorzAlignment.Center;
			InfoArgs.DefaultImages = CheckObjectPainter.RequireDefaultImages(InfoArgs) ? BaseCheckedListBoxControl.DefaultCheckImages : null;
		}
		public new CheckObjectInfoArgs InfoArgs { get { return (CheckObjectInfoArgs)base.InfoArgs; } }
		protected override StyleObjectInfoArgs CreateStyleObjectInfoArgs() {
			return new CheckObjectInfoArgs(null);
		}
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class PainterCheckedListBox : BaseListBoxPainter {
		protected override void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			base.DrawItemCore(info, itemInfo, e);
			CheckedListBoxViewInfo.CheckedItemInfo checkedItemInfo = (CheckedListBoxViewInfo.CheckedItemInfo)itemInfo;
			checkedItemInfo.CheckViewInfo.Paint(((CheckedListBoxViewInfo)info.ViewInfo).CheckMarkPainter, info.Cache, e.Appearance);
		}
	}
}
namespace DevExpress.XtraEditors {
	[DevExpress.Utils.Design.SmartTagAction(typeof(BaseCheckedListBoxControlActions), "EditItems", "Edit items", DevExpress.Utils.Design.SmartTagActionType.CloseAfterExecute), DevExpress.Utils.Design.SmartTagFilter(typeof(BaseCheckedListBoxControlFilter))]
	public abstract class BaseCheckedListBoxControl : BaseListBoxControl {
		[ThreadStatic]
		static ArrayList defaultCheckImages = null;
		protected static internal ArrayList DefaultCheckImages {
			get {
				if(defaultCheckImages == null)
					defaultCheckImages = GetDefaultCheckImages();
				return defaultCheckImages;
			}
		}
		static ArrayList GetDefaultCheckImages() {
			ImageCollection imageCollection = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Images.CheckBoxes.gif", typeof(RepositoryItemCheckEdit).Assembly, new Size(18, 18), Color.Magenta);
			if(imageCollection == null) return null;
			ArrayList images = new ArrayList();
			for(int i = 0; i < imageCollection.Images.Count; i++)
				images.Add(imageCollection.Images[i]);
			imageCollection.Dispose();
			return images;
		}
		bool allowGrayed;
		CheckStyles checkStyle;
		Image pictureChecked, pictureUnchecked, pictureGrayed;
		string checkMember;
		internal const string CheckMemberException = "CheckMember should correspond to a NullableBoolean or Boolean field in a data source.";
		#region Events
		readonly object itemChecking = new object();
		readonly object itemCheck = new object();
		readonly object getItemEnabled = new object();
		readonly object checkMemberChanged = new object();
		readonly object convertCheckValue = new object();
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlItemChecking"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event ItemCheckingEventHandler ItemChecking {
			add { Events.AddHandler(itemChecking, value); }
			remove { Events.RemoveHandler(itemChecking, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlItemCheck"),
#endif
 DXCategory(CategoryName.Behavior)]
		public event DevExpress.XtraEditors.Controls.ItemCheckEventHandler ItemCheck {
			add { Events.AddHandler(itemCheck, value); }
			remove { Events.RemoveHandler(itemCheck, value); }
		}
		[ DXCategory(CategoryName.Behavior)]
		public event GetItemEnabledEventHandler GetItemEnabled {
			add { Events.AddHandler(getItemEnabled, value); }
			remove { Events.RemoveHandler(getItemEnabled, value); }
		}
		[ DXCategory(CategoryName.Behavior)]
		public event EventHandler CheckMemberChanged {
			add { Events.AddHandler(checkMemberChanged, value); }
			remove { Events.RemoveHandler(checkMemberChanged, value); }
		}
		[ DXCategory(CategoryName.Behavior)]
		public event ConvertCheckValueEventHandler ConvertCheckValue {
			add { Events.AddHandler(convertCheckValue, value); }
			remove { Events.RemoveHandler(convertCheckValue, value); }
		}  
		#endregion Events
		ConvertCheckValueEventArgs convertCheckValueEventArgs;
		public BaseCheckedListBoxControl()
			: base() {
			this.checkStyle = CheckStyles.Standard;
			this.allowGrayed = false;
			this.checkMember = "";
		}
		#region Properties
		[ DefaultValue(""), DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string CheckMember {
			get { return checkMember; }
			set {
				if(value == null) value = string.Empty;
				if(value == CheckMember) return;
				checkMember = value;
				OnCheckMemberChanged();
			}
		}
		protected bool HasCheckMember { get { return !string.IsNullOrEmpty(CheckMember); } }
		protected ConvertCheckValueEventArgs ConvertCheckValueEventArgs {
			get {
				if(convertCheckValueEventArgs == null)
					convertCheckValueEventArgs = new ConvertCheckValueEventArgs();
				return convertCheckValueEventArgs;
			}
		}
		protected virtual void OnCheckMemberChanged() {
			ResetItemsCheckState();
			RaiseCheckMemberChanged();
		}
		protected void RaiseCheckMemberChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[checkMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected CheckState GetDataSourceCheckValue(int index) {
			return GetDataSourceCheckStateFromCheckValue(index, GetDataSourceValue(index, CheckMember));
		}
		protected virtual CheckState GetDataSourceCheckStateFromCheckValue(int index, object checkValue) { 
			try {
				return CheckedListBoxItem.GetCheckState((bool?)RaiseConvertCheckValue(index, checkValue, false));
			}
			catch(InvalidCastException) {
				throw new InvalidCastException(CheckMemberException);
			}
		}
		protected void SetDataSourceCheckValue(int index, CheckState value) {
			settingDataSourceCheckValue++;
			try {
				object result = CheckedListBoxItem.GetCheckValue(value);
				DataAdapter.SetRowValue(index, CheckMember, RaiseConvertCheckValue(index, result, true));
			}
			finally {
				settingDataSourceCheckValue--;
			}
		}
		protected object RaiseConvertCheckValue(int index, object value, bool isSet) {
			ConvertCheckValueEventArgs.Initialize(index, value, isSet);
			ConvertCheckValueEventHandler handler = (ConvertCheckValueEventHandler)this.Events[convertCheckValue];
			if(handler != null) 
				handler(this, ConvertCheckValueEventArgs);
			return ConvertCheckValueEventArgs.Value;
		}
		int settingDataSourceCheckValue = 0;
		#endregion
		protected override ListBoxControlHandler CreateHandler() {
			return new CheckedListBoxControlHandler(this);
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.CheckedListBoxAccessible(this);
		}
		protected override void OnListChanged(object sender, ListChangedEventArgs e) {
			base.OnListChanged(sender, e);
			if(IsBoundMode) {
				if(settingDataSourceCheckValue > 0) return;
				if(e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted ||
					e.ListChangedType == ListChangedType.ItemMoved || e.ListChangedType == ListChangedType.Reset)
					ResetItemsCheckState();
				if(e.ListChangedType == ListChangedType.ItemChanged && HasCheckMember) {
					CheckedIndices.SetItemCheckState(e.NewIndex, GetDataSourceCheckValue(e.NewIndex));
					LayoutChanged();
				}
			}
			else {
				switch(e.ListChangedType) {
					case ListChangedType.ItemAdded:
					case ListChangedType.ItemDeleted:
					case ListChangedType.Reset:
						ClearCheckedIndices();
						LayoutChanged();
						break;
					case ListChangedType.ItemChanged:
						if(e.OldIndex != e.NewIndex)
							OnSetItemCheckState(new ItemCheckingEventArgs(e.NewIndex, Items[e.NewIndex].CheckState, CheckedIndicesInternal.GetItemCheckState(e.NewIndex)));
						else
							LayoutChanged();
						break;
				}
			}
		}
		public override void Refresh() {
			if(IsBoundMode && HasCheckMember)
				ResetItemsCheckState();
			base.Refresh();
		}
		protected abstract void ClearCheckedIndices();
		protected override void OnDataSourceChanged(object sender, EventArgs e) {
			ResetItemsCheckState();
			base.OnDataSourceChanged(sender, e);
		}
		protected virtual void ResetItemsCheckState() {
			if(IsSearching) return;
			CheckedIndices.Clear();
			if(!IsBoundMode) {
				CheckedIndexCollection indices = CheckedIndices;
				for(int i = 0; i < Items.Count; i++)
					indices.SetItemCheckState(i, Items[i].CheckState);
			}
			else {
				if(HasCheckMember) {
					CheckedIndexCollection indices = CheckedIndices;
					for(int i = 0; i < ItemCount; i++)
						indices.SetItemCheckState(i, GetDataSourceCheckValue(i));
				}
			}
			LayoutChanged();
		}
		protected internal virtual void OnChangeCheck(int index) {
			if(index == -1) return;
			if(!GetItemEnabledCore(index)) return;
			ToggleItem(index);
		}
		public virtual void ToggleItem(int index) {
			if(index < 0 || index > ItemCount - 1) return;
			SetItemCheckState(index, CalcNextCheckState(GetItemCheckState(index)));
		}
		public virtual void ToggleSelectedItems() {
			for(int i = 0; i < SelectedIndices.Count; i++) {
				OnChangeCheck(SelectedIndices[i]);
			}
		}
		public void UnCheckSelectedItems() {
			for(int i = 0; i < SelectedIndices.Count; i++) {
				int itemIndex = SelectedIndices[i];
				if(!GetItemEnabledCore(itemIndex)) continue;
				SetItemChecked(itemIndex, false);
			}
		}
		public void CheckSelectedItems() {
			for(int i = 0; i < SelectedIndices.Count; i++) {
				int itemIndex = SelectedIndices[i];
				if(!GetItemEnabledCore(itemIndex)) continue;
				SetItemChecked(itemIndex, true);
			}
		}
		public bool IsAllSelectedItemsChecked() {
			for(int i = 0; i < SelectedIndices.Count; i++) {
				int itemIndex = SelectedIndices[i];
				if(!GetItemEnabledCore(itemIndex)) continue;
				if(!GetItemChecked(itemIndex))
					return false;
			}
			return true;
		}
		public virtual void InvertCheckState() {
			foreach(CheckedListBoxItem item in Items) {
				item.InvertCheckState();
			}
			LayoutChanged();
		}
		public void UnCheckAll() {
			SetItemsChecked(false);
		}
		public void CheckAll() {
			SetItemsChecked(true);
		}
		protected internal virtual void SetItemsChecked(bool check) {
			CheckState newValue = check ? CheckState.Checked : CheckState.Unchecked;
			ItemCheckingEventArgs checkingArgs = new ItemCheckingEventArgs();
			Controls.ItemCheckEventArgs checkArgs = new Controls.ItemCheckEventArgs();
			int count = ItemCount;
			for(int i = 0; i < ItemCount; i++) {
				CheckState oldValue = GetItemCheckState(i);
				if(oldValue == newValue) continue;
				checkingArgs.Index = i;
				checkingArgs.OldValue = oldValue;
				checkingArgs.NewValue = newValue;
				RaiseItemChecking(checkingArgs);
				if(checkingArgs.Cancel || checkingArgs.NewValue == checkingArgs.OldValue) continue;
				SetItemCheckStateCore(i, checkingArgs.NewValue);
				checkArgs.Index = i;
				checkArgs.State = GetItemCheckState(i);
				RaiseItemCheck(checkArgs);
			}
			LayoutChanged();
		}
		protected virtual void OnSetItemCheckState(ItemCheckingEventArgs e) {
			RaiseItemChecking(e);
			if(e.Cancel || e.NewValue == e.OldValue) return;
			SetItemCheckStateCore(e.Index, e.NewValue);
			LayoutChanged();
			RaiseItemCheck(new DevExpress.XtraEditors.Controls.ItemCheckEventArgs(e.Index, e.NewValue));
		}
		protected virtual void SetItemCheckStateCore(int index, CheckState value) {
			if(IsBoundMode && HasCheckMember) 
				SetDataSourceCheckValue(index, value);
			CheckedIndicesCore.SetItemCheckState(index, value);
			if(!IsBoundMode) 
				Items[index].CheckStateInternal = value;
		}
		protected CheckState CalcNextCheckState(CheckState state) {
			if(state == CheckState.Checked) return CheckState.Unchecked;
			if(state == CheckState.Unchecked) {
				if(AllowGrayed) return CheckState.Indeterminate;
				else return CheckState.Checked;
			}
			if(state == CheckState.Indeterminate) return CheckState.Checked;
			return CheckState.Indeterminate;
		}
		#region Properties
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlCheckOnClick"),
#endif
 DefaultValue(false)]
		public bool CheckOnClick {
			get { return CheckOnClickCore; }
			set { CheckOnClickCore = value; }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlAllowGrayed"),
#endif
 DefaultValue(false)]
		public virtual bool AllowGrayed {
			get { return allowGrayed; }
			set { allowGrayed = value; }
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(CheckStyles.Standard)]
		public CheckStyles CheckStyle {
			get { return checkStyle; }
			set {
				if(CheckStyle == value) return;
				checkStyle = value;
				ClearItemSizes();
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureChecked {
			get { return pictureChecked; }
			set {
				if(PictureChecked == value) return;
				pictureChecked = AdjustPicture(value);
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureUnchecked {
			get { return pictureUnchecked; }
			set {
				if(PictureUnchecked == value) return;
				pictureUnchecked = AdjustPicture(value);
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image PictureGrayed {
			get { return pictureGrayed; }
			set {
				if(PictureGrayed == value) return;
				pictureGrayed = AdjustPicture(value);
				LayoutChanged();
			}
		}
		protected virtual Image AdjustPicture(Image image) {
			return ImageHelper.MakeTransparent(image);
		}
		[Localizable(true), DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseCheckedListBoxControlItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CheckedListBoxItemCollection Items { get { return ItemsCore as CheckedListBoxItemCollection; } }
		[Browsable(false)]
		public CheckedIndexCollection CheckedIndices { get { return CheckedIndicesCore; } }
		[Browsable(false)]
		public CheckedItemCollection CheckedItems { get { return CheckedItemsCore; } }
		public int CheckedItemsCount {
			get {
				int count = 0;
				for(int i = 0; i < ItemCount; i++) {
					if(GetItemCheckState(i) == CheckState.Checked)
						count++;
				}
				return count;
			}
		}
		protected internal new CheckedListBoxViewInfo ViewInfo { get { return base.ViewInfo as CheckedListBoxViewInfo; } }
		protected abstract CheckedIndexCollection CheckedIndicesCore { get; }
		protected abstract CheckedIndexCollection CheckedIndicesInternal { get; }
		protected abstract CheckedItemCollection CheckedItemsCore { get; }
		protected abstract bool CheckOnClickCore { get; set; }
		#endregion Properties
		#region CheckedIndexCollection
		public class CheckedIndexCollection : ICollection, IEnumerable {
			private ArrayList checkIndices, indeterminateIndices;
			internal CheckedIndexCollection() {
				this.checkIndices = new ArrayList();
				this.indeterminateIndices = new ArrayList();
			}
			public int Count { get { return checkIndices.Count; } }
			bool ICollection.IsSynchronized { get { return checkIndices.IsSynchronized; } }
			object ICollection.SyncRoot { get { return checkIndices.SyncRoot; } }
			void ICollection.CopyTo(Array array, int index) { checkIndices.CopyTo(array, index); }
			IEnumerator IEnumerable.GetEnumerator() { return checkIndices.GetEnumerator(); }
			public bool IsReadOnly { get { return true; } }
			public int IndexOf(int index) { return checkIndices.IndexOf(index); }
			public int this[int index] { get { return (int)checkIndices[index]; } }
			internal CheckState GetItemCheckState(int index) {
				if(IndexOf(index) != -1) return CheckState.Checked;
				return ((int)indeterminateIndices.IndexOf(index) == -1 ? CheckState.Unchecked : CheckState.Indeterminate);
			}
			internal void SetItemCheckState(int index, CheckState state) {
				int checkIndex = IndexOf(index);
				int indeterminateIndex = indeterminateIndices.IndexOf(index);
				if(state == CheckState.Checked) {
					if(checkIndex == -1) checkIndices.Add(index);
					if(indeterminateIndex != -1) indeterminateIndices.RemoveAt(indeterminateIndex);
				}
				if(state == CheckState.Indeterminate) {
					if(checkIndex != -1) checkIndices.RemoveAt(checkIndex);
					if(indeterminateIndex == -1) indeterminateIndices.Add(index);
				}
				if(state == CheckState.Unchecked) {
					if(checkIndex != -1) checkIndices.RemoveAt(checkIndex);
					if(indeterminateIndex != -1) indeterminateIndices.RemoveAt(indeterminateIndex);
				}
			}
			internal void Set(ICollection indices) {
				checkIndices.Clear();
				checkIndices.AddRange(indices);
				indeterminateIndices.Clear();
			}
			internal void Set(int index) {
				checkIndices.Clear();
				checkIndices.Add(index);
				indeterminateIndices.Clear();
			}
			internal void Clear() {
				checkIndices.Clear();
				indeterminateIndices.Clear();
			}
			internal void ShiftItemIndices(int fromIndex, int shiftStep) {
				ShiftCollectionIndices(checkIndices, fromIndex, shiftStep);
				ShiftCollectionIndices(indeterminateIndices, fromIndex, shiftStep);
			}
			static void ShiftCollectionIndices(ArrayList indexCollection, int addedDeletedIndex, int direction) {
				for(int i = indexCollection.Count - 1; i >= 0; --i) {
					int oldIndex = (int)indexCollection[i];
					if(oldIndex < addedDeletedIndex) {
					}
					else if(oldIndex == addedDeletedIndex && direction < 0) {
						indexCollection.RemoveAt(i);
					}
					else {
						indexCollection[i] = oldIndex + direction;
					}
				}
			}
		}
		#endregion
		#region CheckedItemCollection
		public class CheckedItemCollection : ICollection, IEnumerable {
			private CheckedListBoxControl checkedListBox;
			internal CheckedItemCollection(CheckedListBoxControl checkedListBox) { this.checkedListBox = checkedListBox; }
			public int Count { get { return CheckedIndices.Count; } }
			bool ICollection.IsSynchronized { get { return ((ICollection)CheckedIndices).IsSynchronized; } }
			object ICollection.SyncRoot { get { return ((ICollection)CheckedIndices).SyncRoot; } }
			void ICollection.CopyTo(Array array, int index) {
				ArrayList itemValues = new ArrayList();
				for(int i = 0; i < Count; i++) itemValues.Add(this[i]);
				itemValues.CopyTo(array, index);
			}
			IEnumerator IEnumerable.GetEnumerator() { return new CheckedItemCollectionIEnumerator(((IEnumerable)CheckedIndices).GetEnumerator(), checkedListBox); }
			public bool IsReadOnly { get { return CheckedIndices.IsReadOnly; } }
			public int IndexOf(object itemValue) {
				for(int i = 0; i < Count; i++) {
					if(this[i] == itemValue) return i;
				}
				return -1;
			}
			public object this[int index] { get { return checkedListBox.GetItemValue(CheckedIndices[index]); } }
			protected CheckedIndexCollection CheckedIndices { get { return checkedListBox.CheckedIndices; } }
			private class CheckedItemCollectionIEnumerator : IEnumerator {
				IEnumerator indicesEnumerator;
				CheckedListBoxControl checkedListBox;
				public CheckedItemCollectionIEnumerator(IEnumerator indicesEnumerator, CheckedListBoxControl checkedListBox) {
					this.indicesEnumerator = indicesEnumerator;
					this.checkedListBox = checkedListBox;
				}
				bool IEnumerator.MoveNext() { return indicesEnumerator.MoveNext(); }
				void IEnumerator.Reset() { indicesEnumerator.Reset(); }
				object IEnumerator.Current { get { return checkedListBox.GetItem((int)indicesEnumerator.Current); } }
			}
			internal CheckedListBoxItem[] ToArray() {
				CheckedListBoxItem[] array = new CheckedListBoxItem[Count];
				for(int i = 0; i < Count; i++) 
					array[i] = checkedListBox.GetItem(CheckedIndices[i]) as CheckedListBoxItem;
				return array;
			}
			internal Array ToArray(Type elementType) {
				ArrayList array = new ArrayList();
				for(int i = 0; i < Count; i++) {
					object value = checkedListBox.GetItemValue(CheckedIndices[i]);
					if(value is ShowAllElement)
						continue;
					if(value == null && elementType.IsValueType)
						continue;
					array.Add(value);
				}
				return array.ToArray(elementType);
			}
		}
		#endregion
		protected internal override bool GetItemEnabledCore(int index) {
			if(!Enabled) return false;
			bool enabled = IsBoundMode ? true : Items[index].Enabled;
			return RaiseGetItemEnabled(index, enabled);
		}
		public virtual CheckState GetItemCheckState(int index) {
			if(index < 0 || index > ItemCount - 1)
				throw new ArgumentOutOfRangeException("index");
			if(IsBoundMode)
				return CheckedIndices.GetItemCheckState(index);
			CheckedListBoxItem item = GetItemCore(index) as CheckedListBoxItem;
			if(item == null)
				throw new ArgumentOutOfRangeException("index");
			return item.CheckState;
		}
		public virtual bool GetItemChecked(int index) {
			return GetItemCheckState(index) == CheckState.Checked;
		}
		public virtual void SetItemCheckState(int index, CheckState value) {
			CheckState currentState = GetItemCheckState(index);
			if(currentState != value)
				OnSetItemCheckState(new ItemCheckingEventArgs(index, value, currentState));
		}
		public virtual void SetItemChecked(int index, bool value) {
			SetItemCheckState(index, value ? CheckState.Checked : CheckState.Unchecked);
		}		
		protected virtual void RaiseItemChecking(ItemCheckingEventArgs e) {
			ItemCheckingEventHandler handler = (ItemCheckingEventHandler)Events[itemChecking];
			if(handler != null) handler(this, e);
		}
		protected virtual void RaiseItemCheck(DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
			DevExpress.XtraEditors.Controls.ItemCheckEventHandler handler = (DevExpress.XtraEditors.Controls.ItemCheckEventHandler)Events[itemCheck];
			if(handler != null) handler(this, e);
		}
		protected virtual bool RaiseGetItemEnabled(int index, bool enabled) {
			GetItemEnabledEventArgs e = new GetItemEnabledEventArgs(index, enabled);
			GetItemEnabledEventHandler handler = (GetItemEnabledEventHandler)Events[getItemEnabled];
			if(handler != null) handler(this, e);
			return e.Enabled;
		}
	}
	public class CheckedListBoxControlHandler : ListBoxControlHandler {
		public CheckedListBoxControlHandler(BaseCheckedListBoxControl control) : base(control) { }
		protected override ListBoxControlState CreateState(HandlerState state) {
			if(state == HandlerState.Unselectable) return new CheckedUnselectableState(this);
			if(state == HandlerState.MultiSimpleSelect) return new CheckedMultiSimpleSelectState(this);
			if(state == HandlerState.ExtendedMultiSimpleSelect) return new CheckedExtendedMultiSelectState(this);
			return new CheckedSingleSelectState(this);
		}
		public class CheckedUnselectableState : UnselectableState {
			CheckedListBoxPressInfo pressInfo;
			public CheckedUnselectableState(CheckedListBoxControlHandler handler) : base(handler) { }
			protected new BaseCheckedListBoxControl ListBox { get { return base.ListBox as BaseCheckedListBoxControl; } }
			public override void Init() {
				base.Init();
				pressInfo = new CheckedListBoxPressInfo(-1, -1);
			}
			public override void MouseDown(MouseEventArgs e) {
				int prevFocusedIndex = FocusedIndex;
				base.MouseDown(e);
				if(pressedItemIndex != -1) pressInfo = new CheckedListBoxPressInfo(pressedItemIndex, prevFocusedIndex);
			}
			public override void MouseUp(MouseEventArgs e) {
				base.MouseUp(e);
				if(pressedItemIndex != -1 && pressInfo.CanChangePressState(FocusedIndex, ListBox.CheckOnClick))
					ListBox.OnChangeCheck(FocusedIndex);
				pressInfo.Reset();
			}
			public override void LostFocus() {
				base.LostFocus();
				pressInfo.Reset();
			}
			protected override void SpaceKeyDown() {
				base.SpaceKeyDown();
				ListBox.OnChangeCheck(FocusedIndex);
			}
		} 
		public class CheckedSingleSelectState : SingleSelectState {
			CheckedListBoxPressInfo pressInfo;
			public CheckedSingleSelectState(CheckedListBoxControlHandler handler) : base(handler) { }
			public override void Init() {
				base.Init();
				pressInfo = new CheckedListBoxPressInfo(-1, -1);
			}
			public override void MouseDown(MouseEventArgs e) {
				int prevSelectedIndex = ListBox.SelectedIndex;
				base.MouseDown(e);
				if(pressedItemIndex != -1) pressInfo = new CheckedListBoxPressInfo(pressedItemIndex, prevSelectedIndex);
			}
			protected new BaseCheckedListBoxControl ListBox { get { return base.ListBox as BaseCheckedListBoxControl; } }
			protected virtual bool CanChangePressState(Point location, int selIndex, bool checkOnClick) {
				return pressInfo.CanChangePressState(selIndex, checkOnClick);
			}
			protected CheckedListBoxViewInfo ViewInfo { get { return (CheckedListBoxViewInfo)Handler.OwnerControl.ViewInfo; } }
			public override void MouseUp(MouseEventArgs e) {
				base.MouseUp(e);
				if(pressedItemIndex != -1 && CanChangePressState(e.Location, ListBox.SelectedIndex, ListBox.CheckOnClick)) {
					ListBox.OnChangeCheck(ListBox.SelectedIndex);
				}
				pressInfo.Reset();
			}
			public override void LostFocus() {
				base.LostFocus();
				pressInfo.Reset();
			}
			protected override void SpaceKeyDown() {
				base.SpaceKeyDown();
				ListBox.OnChangeCheck(ListBox.SelectedIndex);
			}
		}
		public class CheckedMultiSimpleSelectState : MultiSimpleSelectState {
			public CheckedMultiSimpleSelectState(CheckedListBoxControlHandler handler) 
				: base(handler) { 
			}
			protected new BaseCheckedListBoxControl ListBox { get { return (BaseCheckedListBoxControl)base.ListBox; } }
			protected override void UpdatePressedItemIndex(MouseEventArgs e) {
				base.UpdatePressedItemIndex(e);
				if(pressedItemIndex == -1) return;
				DevExpress.XtraEditors.ViewInfo.CheckedListBoxViewInfo.CheckedItemInfo item = ListBox.ViewInfo.GetItemByIndex(pressedItemIndex);
				if(item.CheckViewInfo.HitTest(e.Location)) {
					if(!ListBox.SelectedIndices.Contains(pressedItemIndex))
						ListBox.OnChangeCheck(pressedItemIndex);
					else {
						if(ListBox.GetItemEnabledCore(pressedItemIndex)) {
							if(!ListBox.GetItemChecked(pressedItemIndex))
								ListBox.CheckSelectedItems();
							else
								ListBox.UnCheckSelectedItems();
						}
					}
					pressedItemIndex = -1;
				}
			}
			protected override void EnterKeyDown() {
				if(ListBox.IsAllSelectedItemsChecked())
					ListBox.UnCheckSelectedItems();
				else
					ListBox.CheckSelectedItems();
			}
		}
		public class CheckedExtendedMultiSelectState : ExtendedMultiSelectState {
			public CheckedExtendedMultiSelectState(CheckedListBoxControlHandler handler) 
				: base(handler) { 
			}
			protected new BaseCheckedListBoxControl ListBox { get { return (BaseCheckedListBoxControl)base.ListBox; } }
			protected override void UpdatePressedItemIndex(MouseEventArgs e) {
				base.UpdatePressedItemIndex(e);
				if(pressedItemIndex == -1) return;
				DevExpress.XtraEditors.ViewInfo.CheckedListBoxViewInfo.CheckedItemInfo item = ListBox.ViewInfo.GetItemByIndex(pressedItemIndex);
				if(item.CheckViewInfo.HitTest(e.Location)) {
					if(!ListBox.SelectedIndices.Contains(pressedItemIndex))
						ListBox.OnChangeCheck(pressedItemIndex);
					else {
						if(ListBox.GetItemEnabledCore(pressedItemIndex)) {
							if(!ListBox.GetItemChecked(pressedItemIndex))
								ListBox.CheckSelectedItems();
							else
								ListBox.UnCheckSelectedItems();
						}
					}
					pressedItemIndex = -1;
				}
			}
			public override void MouseDown(MouseEventArgs e) {
				int oldFocusedIndex = FocusedIndex;
				base.MouseDown(e);
				if(pressedItemIndex == -1) return;
				if(oldFocusedIndex == FocusedIndex && !Handler.IsControlPressed && !Handler.IsShiftPressed) {
					ListBox.OnChangeCheck(FocusedIndex);
				}
			}
			protected override void SpaceKeyDown() {
				if(!ListBox.GetItemEnabledCore(FocusedIndex)) return;
				SpaceEnterKeyDown();
			}			
			protected override void EnterKeyDown() {
				SpaceEnterKeyDown();
			}
			protected virtual void SpaceEnterKeyDown() {
				if(ListBox.IsAllSelectedItemsChecked())
					ListBox.UnCheckSelectedItems();
				else
					ListBox.CheckSelectedItems();
			}
			public override void KeyPress(KeyPressEventArgs e) {
				if(e.Handled || !IncrementalSearch && (e.KeyChar == ' ' || e.KeyChar == '\r')) return;
				search.KeyPress(e.KeyChar);				
			}
		}
		#region CheckedListBoxPressInfo
		struct CheckedListBoxPressInfo {
			int pressItemIndex;
			int selectedIndex;
			public int PressedItemIndex { get { return pressItemIndex; } set { pressItemIndex = value; } }
			public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = value; } }
			public CheckedListBoxPressInfo(int pressItemIndex, int selectedIndex) {
				this.pressItemIndex = pressItemIndex;
				this.selectedIndex = selectedIndex;
			}
			public bool CanChangePressState(int index, bool always) {
				if(pressItemIndex == -1) return false;
				if(index == pressItemIndex) {
					if(always) return true;
					else return (index == selectedIndex);
				}
				return false;
			}
			public void Reset() { pressItemIndex = selectedIndex = -1; }
		}
		#endregion
	}
	[DXToolboxItem(DXToolboxItemKind.Free), DefaultProperty("Items"),
	 Description("Displays a check list of items."),
	ToolboxTabName(AssemblyInfo.DXTabCommon)
	]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple", EnableDirectBinding = false)]
	[CheckedListBoxControlCustomBindingProperties]
	[ToolboxBitmap(typeof(ToolboxIconsRootNS), "CheckedListBoxControl")]
	public class CheckedListBoxControl : BaseCheckedListBoxControl {
		private CheckedIndexCollection checkedIndices;
		private CheckedItemCollection checkedItems;
		private bool checkOnClick;
		public CheckedListBoxControl()
			: base() {
			this.checkedIndices = new CheckedIndexCollection();
			this.checkedItems = new CheckedItemCollection(this);
			this.checkOnClick = false;
			this.Items.ListChanged += new ListChangedEventHandler(OnCollectionChanged);
		}
		protected override ListBoxItemCollection CreateItemsCollection() {
			return new CheckedListBoxItemCollection();
		}
		protected virtual void OnCollectionChanged(object sender, ListChangedEventArgs e) {
			if(!IsBoundMode) base.OnListChanged(sender, e);
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() { return new CheckedListBoxViewInfo(this); }
		protected override BaseControlPainter CreatePainter() { return new PainterCheckedListBox(); }
		protected override CheckedIndexCollection CheckedIndicesCore {
			get {
				if(!IsBoundMode && checkedIndices.Count == 0) {
					for(int i = 0; i < Items.Count; i++)
						checkedIndices.SetItemCheckState(i, Items[i].CheckState);
				}
				return checkedIndices;
			}
		}
		protected override CheckedIndexCollection CheckedIndicesInternal { get { return checkedIndices; } }
		protected override CheckedItemCollection CheckedItemsCore { get { return checkedItems; } }
		protected override bool CheckOnClickCore { get { return checkOnClick; } set { checkOnClick = value; } }
		protected override void DoSort() {
			new ListBoxItemSorter(Items, this).DoSort();
		}
		protected override string GetItemTextCore(int index) {
			if(index < 0 || index > ItemCount - 1) return string.Empty;
			string text = base.GetItemTextCore(index);
			if(IsBoundMode) return text;
			object item = GetItemCore(index);
			return (item != null ? item.ToString() : string.Empty);
		}
		protected override object GetItemValueCore(int index) {
			CheckedListBoxItem item = GetItemCore(index) as CheckedListBoxItem;
			if(item == null) return null;
			return item.Value;
		}
		protected override void SetItemValueCore(object itemValue, int index) {
			if(itemValue is CheckedListBoxItem)
				base.SetItemValueCore(itemValue, index);
			else
				Items[index].Value = itemValue;
		}
		protected override void ClearCheckedIndices() {
			checkedIndices.Clear();
		}
		protected class CheckedListBoxControlCustomBindingPropertiesAttribute : ListControlCustomBindingPropertiesAttribute {
			public CheckedListBoxControlCustomBindingPropertiesAttribute()
				: base("CheckedListBoxControl") {
			}
		}
	}
	public class ConvertCheckValueEventArgs : EventArgs {
		public object Value { get; set; }
		public bool IsSet { get; private set; }
		public bool IsGet { get { return !IsSet; } }
		public int Index { get; private set; }
		protected internal void Initialize(int index, object value, bool isSet) {
			Index = index;
			IsSet = isSet;
			Value = value;
		}
	}
	public delegate void ConvertCheckValueEventHandler(object sender, ConvertCheckValueEventArgs e);
}
namespace DevExpress.XtraEditors.Controls {
	[ListBindable(false)]
	public class CheckedListBoxItemCollection : ListBoxItemCollection, IEnumerable<CheckedListBoxItem> {
		public CheckedListBoxItemCollection() : base() { }
		public CheckedListBoxItemCollection(int capacity) : base(capacity) { }
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("CheckedListBoxItemCollectionItem")]
#endif
		public new CheckedListBoxItem this[int index] {
			get { return List[index] as CheckedListBoxItem; }
			set { List[index] = value; }
		}
		protected override ListBoxItem CreateItem(object value, string description) {
			return new CheckedListBoxItem(value, description);
		}
		public List<object> GetCheckedValues() {
			List<object> list = new List<object>();
			foreach(CheckedListBoxItem item in this) {
				if(item.CheckState == CheckState.Checked) list.Add(item.Value);
			}
			return list;
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("CheckedListBoxItemCollectionItem")]
#endif
		public CheckedListBoxItem this[object value] {
			get {
				for(int i = 0; i < this.Count; i++) {
					CheckedListBoxItem item = this[i];
					if(item == null) continue;
					if(object.Equals(item.Value, value)) return item;
				}
				return null;
			}
		}
		public override int IndexOf(object value) {
			if(value is CheckedListBoxItem)
				return base.IndexOf(value);
			return IndexOfValue(value);
		}
		public override bool Contains(object item) {
			if(item is CheckedListBoxItem)
				return base.Contains(item);
			return IndexOfValue(item) > -1;
		}
		public void AddRange(CheckedListBoxItem[] items) { base.AddRange(items); }
		public override int Add(object value) {
			if(value is CheckedListBoxItem) return base.Add(value);
			return Add(value, CheckState.Unchecked);
		}
		public int Add(object value, CheckState state, bool enabled) {
			return Add(value, string.Empty, state, enabled);
		}
		public int Add(object value, CheckState state) {
			return Add(value, state, true);
		}
		public int Add(object value, bool isChecked) {
			return Add(value, (bool?)isChecked);
		}
		public int Add(object value, bool? isChecked) {
			return Add(value, CheckedListBoxItem.GetCheckState(isChecked));
		}
		public int Add(object value, string description) {
			return Add(value, description, CheckState.Unchecked, true);
		}
		public int Add(object value, string description, CheckState checkState, bool enabled) {
			return Add(CreateCheckedListBoxItem(value, description, checkState, enabled));
		}
		protected override void Attach(object item) { ((CheckedListBoxItem)item).ItemChanged += new EventHandler(OnItemChanged); }
		protected override void Detach(object item) { ((CheckedListBoxItem)item).ItemChanged -= new EventHandler(OnItemChanged); }
		protected virtual void OnItemChanged(object sender, EventArgs e) {
			int itemIndex = IndexOf(sender as ListBoxItem);
			ListChangedEventArgs args = new ListChangedEventArgs(ListChangedType.ItemChanged, itemIndex, itemIndex);
			if(e == CheckedListBoxItem.CheckStateChangedArgs)
				args = new ListChangedEventArgs(ListChangedType.ItemChanged, itemIndex, -1);
			OnListChanged(args);
		}
		protected virtual CheckedListBoxItem CreateCheckedListBoxItem(object value, string description, CheckState checkState, bool enabled) {
			return new CheckedListBoxItem(value, description, checkState, enabled);
		}
		internal void UpdateItemsChanged() {
			if(Count > 0) this[0].Changed();
		}
		#region IEnumerable<CheckedListBoxItem> Members
		IEnumerator<CheckedListBoxItem> IEnumerable<CheckedListBoxItem>.GetEnumerator() {
			foreach(CheckedListBoxItem item in InnerList)
				yield return item;
		}
		#endregion
	}
	[TypeConverter("DevExpress.XtraEditors.Design.CheckedListBoxItemTypeConverter, " + AssemblyInfo.SRAssemblyEditorsDesign)]
	public class CheckedListBoxItem : ListBoxItem {
		public static CheckState GetCheckState(bool? isChecked) {
			if(!isChecked.HasValue) return CheckState.Indeterminate;
			return isChecked.Value ? CheckState.Checked : CheckState.Unchecked;
		}
		public static bool? GetCheckValue(CheckState checkState) {
			if(checkState == CheckState.Indeterminate) return null;
			return checkState == CheckState.Checked ? true : false;
		}
		internal readonly static CheckStateChangedEventArgs CheckStateChangedArgs = new CheckStateChangedEventArgs();
		CheckState checkState;
		bool enabled;
		string description;
		public CheckedListBoxItem() : this(null) { }
		public CheckedListBoxItem(object value) : this(value, false) { }
		public CheckedListBoxItem(object value, bool isChecked) : this(value, CheckedListBoxItem.GetCheckState(isChecked)) { }
		public CheckedListBoxItem(object value, string description) : this(value, description, CheckState.Unchecked, true) { }
		public CheckedListBoxItem(object value, string description, object tag) : this(value, description, CheckState.Unchecked, true, tag) { }
		public CheckedListBoxItem(object value, CheckState checkState) : this(value, checkState, true) { }
		public CheckedListBoxItem(object value, string description, CheckState checkState) : this(value, description, checkState, true) { }
		public CheckedListBoxItem(object value, string description, CheckState checkState, object tag) : this(value, description, checkState, true, tag) { }
		public CheckedListBoxItem(object value, CheckState checkState, bool enabled) : this(value, string.Empty, checkState, enabled) {}
		public CheckedListBoxItem(object value, string description, CheckState checkState, bool enabled) : this(value, description, checkState, enabled, null) { }
		public CheckedListBoxItem(object value, string description, CheckState checkState, bool enabled, object tag)
			: base(value, tag) {
			this.checkState = checkState;
			this.enabled = enabled;
			this.description = description;
		}
		[DefaultValue(CheckState.Unchecked), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckedListBoxItemCheckState")
#else
	Description("")
#endif
]
		public virtual CheckState CheckState { get { return CheckStateInternal; } set { SetCheckStateCore(value, true); } }
		public virtual void InvertCheckState() {
			if(CheckStateInternal == CheckState.Indeterminate) return;
			SetCheckStateCore((CheckStateInternal == CheckState.Checked) ? CheckState.Unchecked : CheckState.Checked, true);
		}
		protected internal void SetCheckStateCore(CheckState checkState, bool raiseChanged) {
			if(CheckStateInternal != checkState) {
				CheckStateInternal = checkState;
				if(raiseChanged) Changed(CheckStateChangedArgs);
			}
		}
		internal CheckState CheckStateInternal { get { return checkState; } set { checkState = value; } }
		[DefaultValue(true), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("CheckedListBoxItemEnabled")
#else
	Description("")
#endif
]
		public virtual bool Enabled {
			get { return enabled; }
			set {
				if(Enabled != value) {
					enabled = value;
					Changed();
				}
			}
		}
		[DefaultValue(""), Description("")]
		public virtual string Description {
			get { return description; }
			set {
				if(Description != value) {
					description = value;
					Changed();
				}
			}
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Description)) return Description;
			return base.ToString();
		}
	}
	internal class CheckStateChangedEventArgs : EventArgs { }
	public class GetItemEnabledEventArgs : EventArgs {
		int index;
		bool enabled;
		public GetItemEnabledEventArgs(int index, bool enabled) {
			this.index = index;
			this.enabled = enabled;
		}
		public int Index { get { return index; } }
		public bool Enabled { get { return enabled; } set { enabled = value; } }
	}
	public class ItemCheckingEventArgs : ChangingEventArgs {
		int index;
		public ItemCheckingEventArgs(int index, CheckState newValue, CheckState oldValue)
			: base(oldValue, newValue) {
			this.index = index;
		}
		protected internal ItemCheckingEventArgs()
			: base(CheckState.Checked, CheckState.Checked) {
		}
		public int Index { 
			get { return index; }
			protected internal set { index = value; }
		}
		public new CheckState OldValue { 
			get { return (CheckState)base.OldValue; }
			protected internal set { base.OldValue = value; }
		}
		public new CheckState NewValue {
			get { return (CheckState)base.NewValue; }
			set { base.NewValue = value; }
		}
	}
	public class ItemCheckEventArgs : EventArgs {
		int index;
		CheckState state;
		public ItemCheckEventArgs(int index, CheckState state) {
			this.index = index;
			this.state = state;
		}
		protected internal ItemCheckEventArgs() { }
		public int Index { 
			get { return index; }
			protected internal set { index = value; }
		}
		public CheckState State { 
			get { return state; }
			protected internal set { state = value; }
		}
	}
	public delegate void ItemCheckingEventHandler(object sender, ItemCheckingEventArgs e);
	public delegate void ItemCheckEventHandler(object sender, ItemCheckEventArgs e);
	public delegate void GetItemEnabledEventHandler(object sender, GetItemEnabledEventArgs e);
}
