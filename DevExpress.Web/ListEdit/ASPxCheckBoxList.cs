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
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxData("<{0}:ASPxCheckBoxList runat=\"server\" ValueType=\"System.String\"></{0}:ASPxCheckBoxList>"),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCheckBoxList.bmp")
	]
	public class ASPxCheckBoxList : ASPxCheckListBase, IMultiSelectListEdit {
		private SelectedIndexCollection selectedIndices;
		private SelectedItemCollection selectedItems;
		private SelectedValueCollection selectedValues;
		private bool selectedIndicesClientChanged = false;
		public ASPxCheckBoxList() : base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties CheckedImage {
			get { return Properties.CheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties UncheckedImage {
			get { return Properties.UncheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListCheckBoxFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual  EditorDecorationStyle CheckBoxFocusedStyle {
			get { return Properties.CheckBoxFocusedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListCheckBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxStyle {
			get { return Properties.CheckBoxStyle; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCheckBoxListValue")]
#endif
		public override object Value {
			get {
				ListEditHelper.ValueDemanded(ConvertEmptyStringToNull);
				return base.Value;
			}
			set {
				ListEditHelper.ValueDemanded(ConvertEmptyStringToNull);
				if(!CommonUtils.AreEqual(base.Value, value, false)) { 
					base.Value = value;
					ListEditHelper.OnValueChanged(value);
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListSelectedIndices"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		MergableProperty(false), AutoFormatDisable]
		public SelectedIndexCollection SelectedIndices {
			get {
				if(selectedIndices == null)
					selectedIndices = new SelectedIndexCollection(this);
				return selectedIndices;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxListSelectedItems"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		MergableProperty(false), AutoFormatDisable]
		public SelectedItemCollection SelectedItems {
			get {
				if(selectedItems == null)
					selectedItems = new SelectedItemCollection(this);
				return selectedItems;
			}
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxCheckBoxListSelectedValues")]
#endif
		public SelectedValueCollection SelectedValues {
			get {
				if(selectedValues == null) {
					selectedValues = new SelectedValueCollection(this);
					selectedValues.SetSortMethod(new SelectedValueCollection.SortMethod(SortSelectedValue));
				}
				return selectedValues;
			}
		}
		protected void SortSelectedValue() {
			List<object> sortedValues = new List<object>();
			foreach(ListEditItem item in Items) {
				if(item.Selected)
					sortedValues.Add(item.Value);
			}
			SelectedValues.ClearSelection();
			foreach(object value in sortedValues)
				SelectedValues.AddInternal(value);
		}
		public void SelectAll() {
			ListEditHelper.SetAllSelection(true);
		}
		public void UnselectAll() {
			ListEditHelper.SetAllSelection(false);
		}
		protected internal new CheckBoxListProperties Properties {
			get { return base.Properties as CheckBoxListProperties; }
		}
		protected override ListEditHelper CreateListEditHelper() {
			return new CheckBoxListHelper(new CheckBoxListSelectHelperOwnerProxy(this));
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			return base.LoadPostData(postCollection) || GetIsSelectionChangedByPostData();
		}
		protected override object GetPostBackValue(string key, System.Collections.Specialized.NameValueCollection postCollection) {
			string serializedIndices = GetClientValue(UniqueID, postCollection);
			if(serializedIndices != null){
				ListEditItemsSerializingHelper serializingHelper = new ListEditItemsSerializingHelper(Properties);
				List<int> clientSelectedIndices = serializingHelper.DeserializeMultiSelectIndices(serializedIndices);
				this.selectedIndicesClientChanged = SelectedIndices.Count != clientSelectedIndices.Count;
				List<int> shouldBeUnselected = new List<int>();
				foreach(int index in selectedIndices) {
					if(!clientSelectedIndices.Contains(index)){
						this.selectedIndicesClientChanged = true;
						shouldBeUnselected.Add(index);
					}
				}
				foreach(int index in clientSelectedIndices) {
					if(index < Items.Count && !Items[index].Selected){
						this.selectedIndicesClientChanged = true;
						Items[index].Selected = true;
					}
				}
				foreach (int index in shouldBeUnselected) {
					Items[index].Selected = false;
				}
			}
			return Value;
		}
		protected bool GetIsSelectionChangedByPostData() {
			return this.selectedIndicesClientChanged;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new CheckBoxListProperties(this);
		}		
		protected override string GetClientObjectClassName() {
			return "ASPxClientCheckBoxList";
		}
		protected override List<InternalCheckBoxImageProperties> GetImages() {
			InternalCheckBoxImageProperties checkedImage = new InternalCheckBoxImageProperties();
			checkedImage.MergeWith(Properties.Images.CheckBoxChecked);
			checkedImage.MergeWith(Properties.Images.GetImageProperties(Page, EditorImages.CheckBoxCheckedImageName));
			InternalCheckBoxImageProperties uncheckedImage = new InternalCheckBoxImageProperties();
			uncheckedImage.MergeWith(Properties.Images.CheckBoxUnchecked);
			uncheckedImage.MergeWith(Properties.Images.GetImageProperties(Page, EditorImages.CheckBoxUncheckedImageName));
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] { checkedImage, uncheckedImage });	  
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(ListEditHelper.GetMultiSelectedIndicesArrayScript(localVarName));
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			return RenderStyles.GetDefaultCheckBoxListStyle();
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.CheckBoxList);
			return style;
		}
		protected override AppearanceStyleBase GetInternalCheckBoxFocusedStyle(){
			return GetICBFocusedStyle();
		}
		protected internal override AppearanceStyleBase GetInternalCheckBoxStyle(){
			return GetICBStyle();
		}
		protected AppearanceStyleBase GetICBFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultICBFocusedClass());
			style.CopyFrom(CheckBoxFocusedStyle);
			return style;
		}
		protected AppearanceStyleBase GetICBStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultICBClass());
			style.CopyFrom(CheckBoxStyle);
			return style;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { SelectedValues });
		}
		protected override ASPxCheckBox CreateItemControlCore(int index, ListEditItem item){
			return new CheckBoxListItemControl(this, item, index);			
		}
		protected internal override bool? GetItemSelected(ListEditItem item) {
			return ListEditHelper.GetItemSelected(item, ConvertEmptyStringToNull);
		}
		internal void OnItemSelectionChanged(ListEditItem item, bool selected) {
			ListEditHelper.OnItemSelectionChanged(item, selected);
		}
		protected internal void OnItemDeleting(ListEditItem item) {
			ListEditHelper.OnItemDeleting(item);
		}
		protected internal void OnItemsCleared() {
			ListEditHelper.OnItemsCleared();
		}
	}
}
namespace DevExpress.Web.Internal {
	public class CheckBoxListSelectHelperOwnerProxy : IListEditMultiSelectHelperOwner {
		ASPxCheckBoxList checkBoxList;
		public CheckBoxListSelectHelperOwnerProxy(ASPxCheckBoxList checkBoxList) {
			this.checkBoxList = checkBoxList;
		}
		ASPxCheckBoxList CheckBoxList {
			get { return checkBoxList; }
		}
		object IEditDataHelperOwner.DataSource { get { return CheckBoxList.DataSource; } }
		string IEditDataHelperOwner.DataSourceID { get { return checkBoxList.DataSourceID; } }
		bool IEditDataHelperOwner.DesignMode { get { return CheckBoxList.DesignMode; } }
		ListEditItemCollection IEditDataHelperOwner.Items { get { return CheckBoxList.Items; } }
		object IEditDataHelperOwner.Value {
			get { return CheckBoxList.Value; }
			set { CheckBoxList.Value = value; }
		}
		Type IEditDataHelperOwner.ValueType { get { return CheckBoxList.ValueType; } }
		bool IEditDataHelperOwner.IsLoading() { return CheckBoxList.IsLoading(); }
		public SelectedValueCollection SelectedValues { get { return CheckBoxList.SelectedValues; } }
		public ListEditSelectionMode SelectionMode {
			get { return ListEditSelectionMode.CheckColumn; }
		}
	}
}
