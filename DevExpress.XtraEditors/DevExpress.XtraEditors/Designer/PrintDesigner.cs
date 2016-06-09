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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraTab;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public abstract class ControlPrintingBase : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		bool autoApply;
		GroupControl previewPanel;
		XtraTabControl tabControl;
		SplitContainerControl splitContanerControl;
		ControlPrintingOptionItemCollection optionItems;
		Control sampleControl;
		ImageCollection imageCollection;
		public ControlPrintingBase() {
			this.optionItems = new ControlPrintingOptionItemCollection();
			this.sampleControl = null;
			this.imageCollection = CreateImageCollection();
			CreateOptionItems();
		}
		bool isFirstLoad = true;
		public override void DoInitFrame() {
			base.DoInitFrame();
			if(!this.isFirstLoad) return;
			this.isFirstLoad = false;
			this.sampleControl = CreateSampleControl();
			InitializeControls();
			CreateOptionControls();
		}
		protected Control SampleControl { get { return sampleControl; } }
		protected ImageCollection ImageCollection { get { return imageCollection; } }
		public ControlPrintingOptionItemCollection OptionItems { get { return optionItems; } }
		public int SplitterPosition { get { return SplitContanerControl.SplitterPosition; } set { SplitContanerControl.SplitterPosition = value; } }
		public abstract Size UserControlSize { get; }
		public bool AutoApply { get { return autoApply; } set { autoApply = value; } }
		public void ApplyOptions() {
			ApplyOptions(AutoApply);
		}
		public void ApplyOptions(bool setOptions) {
			if(!setOptions) return;
			ApplyOptionsCore();
		}
		protected virtual void ApplyOptionsCore() { }
		protected virtual int TabControlWidth { get { return 150; } }
		protected SplitContainerControl SplitContanerControl { get { return splitContanerControl; } }
		protected GroupControl PreviewPanel { get { return previewPanel; } }
		protected XtraTabControl TabControl { get { return tabControl; } }
		protected virtual ImageCollection CreateImageCollection() { return null; }
		protected virtual void CreateOptionItems() { }
		protected virtual Control CreateSampleControl() { return null; }
		protected virtual Size ItemImageSize { get { return ImageCollection != null ? ImageCollection.ImageSize : new Size(16, 16); } }
		protected virtual void InitializeControls() {
			this.pnlMain.DockPadding.All = 2;
			splitContanerControl = new SplitContainerControl();
			tabControl = new XtraTabControl();
			previewPanel = new GroupControl();
			SplitContanerControl.Parent = this.pnlMain;
			SplitContanerControl.Dock = DockStyle.Fill;
			SplitContanerControl.SplitterPosition = TabControlWidth;
			SplitContanerControl.Panel1.Controls.Add(TabControl);
			SplitContanerControl.Panel2.Controls.Add(PreviewPanel);
			TabControl.Dock = DockStyle.Fill;
			PreviewPanel.Dock = DockStyle.Fill;
			PreviewPanel.Text = Localizer.Active.GetLocalizedString(StringId.PreviewPanelText);
			if(SampleControl != null) {
				SampleControl.Parent = PreviewPanel;
				SampleControl.Dock = DockStyle.Fill;
			}
		}
		const int CategoryLeftOffset = 2;
		const int ItemTopOffset = 3;
		const int ItemLeftOffset = 4;
		protected virtual void CreateOptionControls() {
			int top = ItemTopOffset;
			for(int i = 0; i < OptionItems.Count; i++) {
				OptionItems[i].EditableControl = EditingObject;
				OptionItems[i].SampleControl = SampleControl;
				if(IsNextTab(i)) {
					TabControl.TabPages.Add(OptionItems[i].TabName);
					TabControl.SelectedTabPageIndex = TabControl.TabPages.Count - 1; 
					top = ItemTopOffset;
				}
				if(IsNextCategory(i)) {
					top += CreateCategoryControl(OptionItems[i].CategoryName, top) + ItemTopOffset;
				}
				int itemHeight = CreateItemControl(OptionItems[i], top);
				CreateImageControl(OptionItems[i], top, itemHeight);
				top += itemHeight + ItemTopOffset;
			}
			if(TabControl.TabPages.Count > 0)
				TabControl.SelectedTabPageIndex = 0;
		}
		bool IsNextTab(int index) {
			return index == 0 || OptionItems[index - 1].TabName != OptionItems[index].TabName;
		}
		bool IsNextCategory(int index) {
			return IsNextTab(index) || OptionItems[index - 1].CategoryName != OptionItems[index].CategoryName;
		}
		int CreateCategoryControl(string categoryName, int top) {
			Label label = new Label();
			label.Parent = ControlItemParent;
			label.Left = CategoryLeftOffset;
			label.Width = TabControlWidth - label.Left * CategoryLeftOffset;
			label.Top = top;
			label.BackColor = SystemColors.ControlDarkDark;
			label.ForeColor = SystemColors.HighlightText;
			label.Text = categoryName;
			label.Height = label.Font.Height + ItemLeftOffset;
			return label.Height;
		}
		void CreateImageControl(ControlPrintingOptionItem item, int top, int height) {
			PivotPrintImageControl imageControl = new PivotPrintImageControl(ImageCollection, item);
			imageControl.Parent = ControlItemParent;
			imageControl.Left = ItemLeftOffset;
			imageControl.Size = ItemImageSize;
			imageControl.Top = top + (height - imageControl.Height) / 2;
		}
		int CreateItemControl(ControlPrintingOptionItem item, int top) {
			object itemValue = item.Value;
			if(itemValue == null) return top;
			if(itemValue.GetType() == typeof(bool))
				return CreateBooleanItem(item, top);
			if(itemValue.GetType() == typeof(DefaultBoolean))
				return CreateDefaultBooleanItem(item, top);
			return 0;
		}
		int CreateBooleanItem(ControlPrintingOptionItem item, int top) {
			CheckEdit checkEdit = new CheckEdit();
			checkEdit.Tag = item;
			checkEdit.Parent = ControlItemParent;
			checkEdit.Text = item.Text;
			checkEdit.Left = ItemImageSize.Width + ItemLeftOffset * 2;
			checkEdit.Width = TabControlWidth - checkEdit.Left - ItemLeftOffset;
			checkEdit.Checked = (bool)item.Value;
			checkEdit.CheckStateChanged += new EventHandler(OnValueChanged);
			checkEdit.Top = top;
			return checkEdit.CalcMinHeight();
		}
		int CreateDefaultBooleanItem(ControlPrintingOptionItem item, int top) {
			ComboBoxEdit comboBox = new ComboBoxEdit();
			comboBox.Tag = item;
			comboBox.Parent = ControlItemParent;
			comboBox.Left = ItemImageSize.Width + ItemLeftOffset * 2;
			comboBox.Top = top;
			comboBox.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			comboBox.Properties.Items.Add(Localizer.Active.GetLocalizedString(StringId.DefaultBooleanTrue));
			comboBox.Properties.Items.Add(Localizer.Active.GetLocalizedString(StringId.DefaultBooleanFalse));
			comboBox.Properties.Items.Add(Localizer.Active.GetLocalizedString(StringId.DefaultBooleanDefault));
			comboBox.SelectedIndex = (int)item.Value;
			comboBox.Width = GetDefaultBooleanItemWidth(comboBox);
			comboBox.SelectedIndexChanged += new EventHandler(OnValueChanged);
			int height = comboBox.CalcMinHeight();
			Label label = new Label();
			label.Parent = ControlItemParent;
			label.Text = item.Text;
			label.Left = comboBox.Right + ItemLeftOffset / 2;
			label.AutoSize = true;
			label.Top = comboBox.Top + (height - label.Height) / 2;
			return height;
		}
		protected virtual int GetDefaultBooleanItemWidth(ComboBoxEdit comboBox) {
			return 60;
		}
		Control ControlItemParent { get { return TabControl.TabPages[TabControl.TabPages.Count - 1]; } }
		protected virtual void OnValueChanged() {
			if(SampleControl != null)
				SampleControl.Invalidate();
		}
		void OnValueChanged(object sender, EventArgs e) {
			ControlPrintingOptionItem item = null;
			CheckEdit checkEdit = sender as CheckEdit;
			if(checkEdit != null) {
				item = checkEdit.Tag as ControlPrintingOptionItem;
				if(item != null)
					item.Value = checkEdit.Checked;
			}
			ComboBoxEdit comboBox = sender as ComboBoxEdit;
			if(comboBox != null) {
				item = comboBox.Tag as ControlPrintingOptionItem;
				if(item != null)
					item.Value = (DefaultBoolean)comboBox.SelectedIndex;
			}
			OnValueChanged();
		}
	}
	public class ControlPrintingOptionItem {
		string propertyPath;
		string origionalPropertyPath;
		string text;
		string tabName;
		string categoryName;
		int imageIndex;
		object editableControl;
		object lastPathObject;
		object sampleControl;
		object lastSampleObject;
		public event EventHandler ValueChanged;
		public ControlPrintingOptionItem(string propertyPath, string text, string tabName, string categoryName, int imageIndex) : this(propertyPath, string.Empty, text, tabName, categoryName, imageIndex) {
		}
		public ControlPrintingOptionItem(string propertyPath, string origionalPropertyPath, string text, string tabName, string categoryName, int imageIndex) {
			this.propertyPath = propertyPath;
			this.origionalPropertyPath = origionalPropertyPath;
			this.text = text;
			this.tabName = tabName;
			this.categoryName = categoryName;
			this.imageIndex = imageIndex;
			this.editableControl = null;
			this.lastPathObject = null;
			this.sampleControl = null;
			this.lastSampleObject = null;
		}
		public string PropertyPath { get { return propertyPath; } }
		public string OriginalPropertyPath { get { return origionalPropertyPath; } }
		public string Text { get { return text; } }
		public string TabName { get { return tabName; } }
		public string CategoryName { get { return categoryName; } }
		public int ImageIndex { get { return imageIndex; } }
		public object EditableControl { get { return editableControl; } set { editableControl = value; } }
		public object SampleControl { get { return sampleControl; } set { sampleControl = value; } }
		public object Value {
			get {
				PropertyDescriptor propDescriptor = GetEditPropertyDescriptor();
				if(propDescriptor != null && this.lastPathObject != null)
					return propDescriptor.GetValue(this.lastPathObject);
				else return null;
			}
			set {
				if(Value == value)
					return;
				SetValue(GetEditPropertyDescriptor(), this.lastPathObject, value);
				SetValue(GetSamplePropertyDescriptor(), this.lastSampleObject, value);
				if(ValueChanged != null)
					ValueChanged(this, new EventArgs());
			}
		}
		static void SetValue(PropertyDescriptor propDescriptor, object obj, object value) {
			if(propDescriptor != null && obj != null)
				propDescriptor.SetValue(obj, value);
		}
		public bool IsChecked {
			get {
				object value = Value;
				if(value == null) return false;
				if(value.GetType() == typeof(bool)) return (bool)value;
				if(value.GetType() == typeof(DefaultBoolean)) {
					DefaultBoolean dBoolean = (DefaultBoolean)value;
					if(dBoolean == DefaultBoolean.Default) {
						if(OriginalPropertyPath != string.Empty) {
							value = GetValue(OriginalPropertyPath);
							if(value.GetType() == typeof(bool))
								return (bool)value;
						}
						return true;
					}
					return dBoolean == DefaultBoolean.True ? true : false;
				}
				return false;
			}
		}
		PropertyDescriptor GetEditPropertyDescriptor() {
			if(EditableControl == null || PropertyPath.Length == 0)
				return null;
			return GetPropertyDescriptor(EditableControl, PropertyPath, out this.lastPathObject);
		}
		PropertyDescriptor GetSamplePropertyDescriptor() {
			if(EditableControl == null || PropertyPath.Length == 0)
				return null;
			return GetPropertyDescriptor(SampleControl, PropertyPath, out this.lastSampleObject);
		}
		static PropertyDescriptor GetPropertyDescriptor(object source, string path, out object childObject) {
			PropertyDescriptor propertyDescriptor = null;
			childObject = source;
			string[] names = path.Split('.');
			for(int i = 0; i < names.Length; i++) {
				propertyDescriptor = GetPropertyDescriptor(childObject, names[i]);
				if(propertyDescriptor == null) {
					childObject = null;
					break;
				}
				if(i < names.Length - 1) {
					childObject = propertyDescriptor.GetValue(childObject);
					if(childObject == null) return propertyDescriptor;
				}
			}
			return propertyDescriptor;
		}
		object GetValue(string propertyFullPath) {
			if(EditableControl == null || propertyFullPath == string.Empty) return null;
			PropertyDescriptor propertyDescriptor = null;
			string[] names = propertyFullPath.Split(new char[] {'.'});
			object value = EditableControl;
			for(int i = 0; i < names.Length; i ++) {
				propertyDescriptor = GetPropertyDescriptor(value, names[i]);
				if(propertyDescriptor == null) 
					return null;
				value = propertyDescriptor.GetValue(value);
				if(value == null) return null;
			}
			return value;
		}
		static PropertyDescriptor GetPropertyDescriptor(object obj, string propName) {
			return TypeDescriptor.GetProperties(obj).Find(propName, true);
		}
	}
	[ListBindable(false)]
	public class ControlPrintingOptionItemCollection : CollectionBase {
		public ControlPrintingOptionItemCollection() {
		}
		public ControlPrintingOptionItem this[int index] { get { return InnerList[index] as ControlPrintingOptionItem; } }
		public void Add(string propertyPath, string text, string tabName, string categoryName, int imageIndex) {
			Add(new ControlPrintingOptionItem(propertyPath, text, tabName, categoryName, imageIndex));
		}
		public void Add(string propertyPath, string origionalPath, string text, string tabName, string categoryName, int imageIndex) {
			Add(new ControlPrintingOptionItem(propertyPath, origionalPath, text, tabName, categoryName, imageIndex));
		}
		public void Add(ControlPrintingOptionItem item) {
			List.Add(item);
		}
		public int IndexOf(string propertyPath) {
			for(int i = 0; i < List.Count; i++) {
				if(this[i].PropertyPath == propertyPath) return i;
			}
			return -1;
		}
		public void Remove(string propertyPath) {
			int index = IndexOf(propertyPath);
			if(index >= 0) RemoveAt(index);
		}
	}
	[ToolboxItem(false)]
	public class PivotPrintImageControl : Control {
		ImageCollection imageCollection;
		ControlPrintingOptionItem optionItem;
		public PivotPrintImageControl(ImageCollection imageCollection, ControlPrintingOptionItem optionItem) {
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.imageCollection = imageCollection;
			this.optionItem = optionItem;
			if(OptionItem != null)
				OptionItem.ValueChanged += new EventHandler(OnValueChanged);
		}
		public ImageCollection ImageCollection { get { return imageCollection; } }
		public ControlPrintingOptionItem OptionItem { get { return optionItem; } }
		protected override void OnPaint(PaintEventArgs e) {
			if(ImageCollection == null || OptionItem == null || 
				OptionItem.ImageIndex < 0 || OptionItem.ImageIndex >= ImageCollection.Images.Count) return;
			Image image = ImageCollection.Images[OptionItem.ImageIndex];
			if(OptionItem.IsChecked)
				e.Graphics.DrawImage(image, 0, 0);
			else ControlPaint.DrawImageDisabled(e.Graphics, image, 0, 0, SystemColors.Control);
		}
		void OnValueChanged(object sender, EventArgs e) {
			Invalidate();
		}
	}
}
