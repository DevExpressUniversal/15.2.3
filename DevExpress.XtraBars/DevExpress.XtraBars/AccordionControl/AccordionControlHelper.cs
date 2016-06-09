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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Internal;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing.Helpers;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
using DevExpress.Utils.Paint;
using DevExpress.Utils.FormShadow;
namespace DevExpress.XtraBars.Navigation {
	public class AccordionControlHelper {
		public AccordionControlHelper(AccordionControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
		}
		AccordionControlViewInfo viewInfo;
		protected AccordionControlViewInfo ViewInfo { get { return viewInfo; } }
		protected internal void ShowVisibleControls() {
			for(int i = 0; i < ViewInfo.ElementsInfo.Count; i++) {
				ShowVisibleGroupControls(ViewInfo.ElementsInfo[i]);
			}
		}
		protected internal void HideControls() {
			for(int i = 0; i < ViewInfo.ElementsInfo.Count; i++) {
				HideGroupControls(ViewInfo.ElementsInfo[i]);
			}
		}
		protected void ShowVisibleGroupControls(AccordionGroupViewInfo groupInfo) {
			ShowVisibleElementControls(groupInfo);
			if(groupInfo.ElementsInfo == null || !groupInfo.Expanded || groupInfo.Element.Style == ElementStyle.Item) return;
			foreach(AccordionElementBaseViewInfo itemInfo in groupInfo.ElementsInfo) {
				if(itemInfo is AccordionGroupViewInfo) {
					ShowVisibleGroupControls(itemInfo as AccordionGroupViewInfo);
				}
				else ShowVisibleElementControls(itemInfo);
			}
		}
		protected void ShowVisibleElementControls(AccordionElementBaseViewInfo elementInfo) {
			if(elementInfo.Element.ContentContainer != null) {
				elementInfo.CheckContentContainerBounds();
				elementInfo.Element.CheckContentContainerVisible();
			}
			if(elementInfo.Element.HeaderControl != null && elementInfo.Element.HeaderVisible) {
				elementInfo.CheckHeaderControlBounds();
				elementInfo.Element.CheckHeaderControlVisible();
			}
		}
		protected void HideGroupControls(AccordionGroupViewInfo groupInfo) {
			if(groupInfo.Element.HeaderVisible && groupInfo.Element.HeaderControlImage == null) groupInfo.CreateImageFromHeaderControl();
			if(groupInfo.Element.HeaderControl != null) groupInfo.Element.HeaderControl.Visible = false;
			if(groupInfo.ElementsInfo == null || groupInfo.Element.Style == ElementStyle.Item) {
				if(groupInfo.Expanded || groupInfo.IsInAnimation) groupInfo.Element.CreateImageFromContentContainer();
				if(groupInfo.Element.ContentContainer != null) groupInfo.Element.ContentContainer.Visible = false;
				return;
			}
			foreach(AccordionElementBaseViewInfo itemInfo in groupInfo.ElementsInfo) {
				if(itemInfo is AccordionGroupViewInfo) HideGroupControls(itemInfo as AccordionGroupViewInfo);
				else HideItemControls(itemInfo as AccordionItemViewInfo);
			}
		}
		protected internal void CreateImageFormHeaderControl(AccordionControlElement element) {
			if(element.HeaderVisible && element.HeaderControl != null) element.HeaderControlImage = AccordionControlPainter.GetControlImage(element.HeaderControl);
			if(element.Style == ElementStyle.Item || element.Elements == null) return;
			foreach(AccordionControlElement childElement in element.Elements) {
				CreateImageFormHeaderControl(childElement);
			}
		}
		protected internal void ClearHeaderControlImages() {
			foreach(AccordionControlElement element in ViewInfo.AccordionControl.Elements) {
				ClearHeaderControlImage(element);
			}
		}
		protected void ClearHeaderControlImage(AccordionControlElement element) {
			if(element.HeaderVisible) element.HeaderControlImage = null;
			if(element.Style == ElementStyle.Item || element.Elements == null) return;
			foreach(AccordionControlElement childElement in element.Elements) {
				ClearHeaderControlImage(childElement);
			}
		}
		protected void HideItemControls(AccordionItemViewInfo itemInfo) {
			if(itemInfo.Expanded || itemInfo.IsInAnimation) itemInfo.Element.CreateImageFromContentContainer();
			if(itemInfo.Element.HeaderVisible && itemInfo.Element.HeaderControlImage == null) itemInfo.CreateImageFromHeaderControl();
			if(itemInfo.Element.ContentContainer != null) itemInfo.Element.ContentContainer.Visible = false;
			if(itemInfo.Element.HeaderControl != null) itemInfo.Element.HeaderControl.Visible = false;
		}
		protected internal AccordionElementBaseViewInfo GetNextItemInfo(AccordionGroupViewInfo group) {
			int index;
			if(group.ParentGroup == null) {
				index = ViewInfo.ElementsInfo.IndexOf(group);
				for(int i = index + 1; i < ViewInfo.ElementsInfo.Count; i++) {
					if(!ViewInfo.ElementsInfo[i].Element.GetVisible()) continue;
					return ViewInfo.ElementsInfo[i];
				}
				return null;
			}
			index = group.ParentGroup.ElementsInfo.IndexOf(group);
			for(int i = index + 1; i < group.ParentGroup.ElementsInfo.Count; i++) {
				if(!group.ParentGroup.ElementsInfo[i].Element.GetVisible()) continue;
				return group.ParentGroup.ElementsInfo[i];
			}
			return GetNextItemInfo(group.ParentGroup);
		}
		protected AccordionElementBaseViewInfo GetExpandedItem(AccordionGroupViewInfo groupInfo, AccordionElementBaseViewInfo expandingItem) {
			foreach(AccordionElementBaseViewInfo itemInfo in groupInfo.ElementsInfo) {
				if(itemInfo == expandingItem) continue;
				if(itemInfo is AccordionGroupViewInfo) {
					AccordionGroupViewInfo childGroupInfo = itemInfo as AccordionGroupViewInfo;
					AccordionElementBaseViewInfo childExpandedItem = GetExpandedItem(childGroupInfo, expandingItem);
					if(childExpandedItem != null) return childExpandedItem;
					continue;
				}
				if(itemInfo.Expanded && itemInfo.Element.HeaderVisible && itemInfo.Element.ContentContainer != null)
					return itemInfo as AccordionItemViewInfo;
			}
			return null;
		}
		protected internal AccordionElementBaseViewInfo GetExpandedItem(AccordionElementBaseViewInfo expandingItem) {
			foreach(AccordionGroupViewInfo groupInfo in ViewInfo.ElementsInfo) {
				if(groupInfo.Element.Style == ElementStyle.Item) {
					if(groupInfo != expandingItem && groupInfo.Expanded)
						return groupInfo;
					continue;
				}
				AccordionElementBaseViewInfo childExpandedItem = GetExpandedItem(groupInfo, expandingItem);
				if(childExpandedItem != null) return childExpandedItem;
			}
			return null;
		}
		public AccordionElementBaseViewInfo GetNonParentExpandedElement(AccordionControlElement expandingElement, List<AccordionGroupViewInfo> elements) {
			for(int i = 0; i < elements.Count; i++) {
				AccordionElementBaseViewInfo elemInfo = elements[i];
				if(elemInfo.Element == expandingElement || !elemInfo.Expanded) continue;
				if(!IsParent(elemInfo.Element, expandingElement))
					return elemInfo;
				AccordionGroupViewInfo groupInfo = elemInfo as AccordionGroupViewInfo;
				if(groupInfo != null && groupInfo.ElementsInfo != null) {
					AccordionElementBaseViewInfo expandedElem = GetNonParentExpandedElement(expandingElement, groupInfo.ElementsInfo);
					if(expandedElem != null)
						return expandedElem;
				}
			}
			return null;
		}
		public AccordionElementBaseViewInfo GetNonParentExpandedElement(AccordionControlElement expandingElement, List<AccordionElementBaseViewInfo> elements) {
			for(int i = 0; i < elements.Count; i++) {
				AccordionElementBaseViewInfo elemInfo = elements[i];
				if(elemInfo.Element == expandingElement || !elemInfo.Expanded) continue;
				if(!IsParent(elemInfo.Element, expandingElement))
					return elemInfo;
				AccordionGroupViewInfo groupInfo = elemInfo as AccordionGroupViewInfo;
				if(groupInfo != null && groupInfo.ElementsInfo != null) {
					AccordionElementBaseViewInfo expandedElem = GetNonParentExpandedElement(expandingElement, groupInfo.ElementsInfo);
					if(expandedElem != null)
						return expandedElem;
				}
			}
			return null;
		}
		static bool IsParent(AccordionControlElement parent, AccordionControlElement element) {
			if(element.OwnerElement == null) return false;
			if(element.OwnerElement == parent) return true;
			return IsParent(parent, element.OwnerElement);
		}
		public Rectangle CheckBounds(Rectangle rect) {
			if(ViewInfo.AccordionControl.IsRightToLeft)
				return new Rectangle(ViewInfo.AccordionControl.Width - rect.X - rect.Width, rect.Y, rect.Width, rect.Height);
			return rect;
		}
		public void CheckControlsVisibility(AccordionControlElementCollection elements) {
			foreach(AccordionControlElement element in elements) {
				CheckElementControlsVisibility(element);
			}
		}
		internal void CheckElementControlsVisibility(AccordionControlElement element) {
			element.CheckContentContainerVisible();
			element.CheckHeaderControlVisible();
			foreach(AccordionControlElement childElement in element.Elements) {
				if(childElement.Style == ElementStyle.Group) {
					CheckElementControlsVisibility(childElement);
				}
				childElement.CheckContentContainerVisible();
				childElement.CheckHeaderControlVisible();
			}
		}
		public static void ForEachElement(Action<AccordionControlElement> handler, AccordionControlElementCollection elements, bool onlyVisible) {
			for(int i = 0; i < elements.Count; i++) {
				if(ShouldProcessChildElements(elements[i], onlyVisible))
					ForEachElement(handler, elements[i].Elements, onlyVisible);
				handler(elements[i]);
			}
		}
		public static List<AccordionControlElement> GetElements(AccordionControlElementCollection elements, bool onlyVisible) {
			List<AccordionControlElement> list = new List<AccordionControlElement>();
			for(int i = 0; i < elements.Count; i++) {
				list.Add(elements[i]);
				if(ShouldProcessChildElements(elements[i], onlyVisible))
					list.AddRange(GetElements(elements[i].Elements, onlyVisible));
			}
			return list;
		}
		static bool ShouldProcessChildElements(AccordionControlElement elem, bool onlyVisible) {
			if(elem.Elements == null) return false;
			if(!onlyVisible) return true;
			return elem.Style == ElementStyle.Group && elem.Expanded && elem.GetVisible();
		}
		public static void ForEachElementInfo(Action<AccordionElementBaseViewInfo> handler, List<AccordionGroupViewInfo> elements) {
			for(int i = 0; i < elements.Count; i++) {
				if(elements[i].ElementsInfo != null) ForEachElementInfo(handler, elements[i].ElementsInfo);
				handler(elements[i]);
			}
		}
		static void ForEachElementInfo(Action<AccordionElementBaseViewInfo> handler, List<AccordionElementBaseViewInfo> elements) {
			for(int i = 0; i < elements.Count; i++) {
				AccordionGroupViewInfo groupInfo = elements[i] as AccordionGroupViewInfo;
				if(groupInfo != null && groupInfo.ElementsInfo != null) ForEachElementInfo(handler, groupInfo.ElementsInfo);
				handler(elements[i]);
			}
		}
		public static void MakeElementVisible(AccordionControlElement element, AccordionControl accordion) {
			if(element.Bounds.Bottom > accordion.ControlInfo.ContentRect.Bottom)
				accordion.ControlInfo.ContentTopIndent += accordion.ControlInfo.ContentRect.Bottom - element.Bounds.Bottom;
			if(element.Bounds.Y < accordion.ControlInfo.ContentRect.Y)
				accordion.ControlInfo.ContentTopIndent += accordion.ControlInfo.ContentRect.Y - element.Bounds.Y;
			accordion.Refresh();
		}
		public static Size CalcExpandCollapseButtonSize(SkinElement elem) {
			if(elem == null || elem.Image == null) return Size.Empty;
			Size buttonSize = GetButtonSize(elem);
			System.Windows.Forms.Padding padding = elem.ContentMargins.ToPadding();
			return new Size(buttonSize.Width + padding.Horizontal, buttonSize.Height + padding.Vertical);
		}
		static Size GetButtonSize(SkinElement elem) {
			if(elem.Glyph != null && elem.Image.Stretch == SkinImageStretch.Stretch)
				return elem.Glyph.GetImageBounds(0).Size;
			return elem.Image.GetImageBounds(0).Size;
		}
		public static Image Load(string name) {
			Image res = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars." + name, typeof(NavigationPaneViewInfo).Assembly);
			Bitmap bmp = res as Bitmap;
			if(bmp != null) bmp.MakeTransparent(Color.Magenta);
			return res;
		}
		public static bool IsLetterOrNumberKey(KeyPressEventArgs e) {
			return char.IsLetter(e.KeyChar) || char.IsNumber(e.KeyChar);
		}
	}
	public class AccordionDropTargetArgs {
		bool canDrop;
		public AccordionDropTargetArgs() {
			this.canDrop = true;
		}
		public AccordionElementBaseViewInfo ElementInfo { get; set; }
		public AccordionElementBaseViewInfo PrevElementInfo { get; set; }
		public object TargetOwner { get; set; }
		internal bool CanDrop { get { return canDrop; } set { canDrop = value; } }
		public bool CanInsertElement() {
			if(ElementInfo == null) return false;
			if(PrevElementInfo == null) {
				if(TargetOwner is AccordionControl)
					return true;
				if(TargetOwner is AccordionControlElement) {
					AccordionControlElement ownerElement = (AccordionControlElement)TargetOwner;
					return CanInsertElementCore(ownerElement);
				}
			}
			return CanInsertElement(PrevElementInfo);
		}
		public bool CanInsertElement(AccordionElementBaseViewInfo targetInfo) {
			if(ElementInfo == null || targetInfo == null) return false;
			AccordionControlElement ownerElement = targetInfo.Element.OwnerElement;
			return CanInsertElementCore(ownerElement);
		}
		bool CanInsertElementCore(AccordionControlElement ownerElement) {
			while(ownerElement != null) {
				if(ownerElement == ElementInfo.Element) return false;
				ownerElement = ownerElement.OwnerElement;
			}
			return true;
		}
	}
	public class KeyboardNavigationHelper {
		public KeyboardNavigationHelper(AccordionControl accordionControl) {
			AccordionControl = accordionControl;
		}
		AccordionControl AccordionControl { get; set; }
		public AccordionControlElement SelectedElement { get; set; }
		public bool ProcessKey(KeyEventArgs e) {
			if(AccordionControl.Elements.Count == 0 || AccordionControl.ControlInfo.IsInAnimation) return false;
			if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
				if(SelectedElement == null) {
					CheckSelectedElement();
					if(SelectedElement == null)
						SelectedElement = AccordionControl.Elements[0];
				}
				else if(e.KeyCode == Keys.Up) SelectPrevElement();
				else if(e.KeyCode == Keys.Down) SelectNextElement();
				else if(e.KeyCode == Keys.Left && !AccordionControl.IsRightToLeft) SelectParentElement();
				else SelectChildElement();
				AccordionControlHelper.MakeElementVisible(SelectedElement, AccordionControl);
				return true;
			}
			if(e.KeyCode == Keys.Home) {
				SelectedElement = AccordionControl.Elements[0];
				AccordionControlHelper.MakeElementVisible(SelectedElement, AccordionControl);
				return true;
			}
			if(e.KeyCode == Keys.End) {
				SelectedElement = AccordionControl.Elements[AccordionControl.Elements.Count - 1];
				AccordionControlHelper.MakeElementVisible(SelectedElement, AccordionControl);
				return true;
			}
			if(e.KeyCode == Keys.Tab) {
				CheckSelectedElement();
				if(SelectedElement != null && SelectedElement.Style == ElementStyle.Item && SelectedElement.ContentContainer != null) {
					if(SelectedElement.ContentContainer.Controls.Count == 0 || !SelectedElement.Expanded) return false;
					SelectedElement.ContentContainer.Controls[0].Focus();
					return true;
				}
			}
			AccordionControlElement elem = GetSelectedElement();
			if(elem != null && ShouldChangeExpandedState(e.KeyCode, elem)) {
				CheckSelectedElement();
				if(elem.CanExpandElement) elem.InvertExpanded();
				AccordionControl.SetSelectedElement(elem);
				CheckSelectedElement();
				AccordionControl.Invalidate();
				return true;
			}
			return false;
		}
		protected bool ShouldChangeExpandedState(Keys keyCode, AccordionControlElement elem) {
			if(keyCode == Keys.Add && !elem.Expanded) return true;
			if(keyCode == Keys.Subtract && elem.Expanded) return true;
			return keyCode == Keys.Space || keyCode == Keys.Enter;
		}
		protected AccordionControlElement GetSelectedElement() {
			if(SelectedElement != null) return SelectedElement;
			return AccordionControl.SelectedElement;
		}
		protected void CheckSelectedElement() {
			SelectedElement = GetSelectedElement();
		}
		protected void SelectParentElement() {
			if(SelectedElement.OwnerElement != null)
				SelectedElement = SelectedElement.OwnerElement;
			else SelectedElement.InvertExpanded();
		}
		protected void SelectChildElement() {
			if(SelectedElement.Style == ElementStyle.Group && SelectedElement.Elements.Count > 0) {
				if(!SelectedElement.Expanded) SelectedElement.InvertExpanded();
				for(int i = 0; i < SelectedElement.Elements.Count; i++) {
					if(IsElementSelectable(SelectedElement.Elements[i])) {
						SelectedElement = SelectedElement.Elements[i];
						break;
					}
				}
			}
		}
		protected bool IsElementSelectable(AccordionControlElement elem) {
			return elem.GetVisible();
		}
		protected void SelectNextElement() {
			AccordionControlElementCollection col = SelectedElement.OwnerElements;
			AccordionControlElement element = SelectedElement;
			if(element.Expanded && element.Style == ElementStyle.Group && element.Elements.Count > 0) {
				for(int i = 0; i < element.Elements.Count; i++) {
					if(IsElementSelectable(element.Elements[i])) {
						SelectedElement = element.Elements[i];
						return;
					}
				}
			}
			while(true) {
				int nextElementIndex = col.IndexOf(element) + 1;
				for(int i = nextElementIndex; i < col.Count; i++) {
					if(IsElementSelectable(col[i])) {
						SelectedElement = col[i];
						return;
					}
				}
				element = element.OwnerElement;
				if(element == null) return;
				col = element.OwnerElements;
			}
		}
		protected void SelectPrevElement() {
			int elementIndex = SelectedElement.OwnerElements.IndexOf(SelectedElement);
			if(elementIndex != 0) {
				AccordionControlElement prevElement = null;
				for(int i = elementIndex - 1; i >= 0; i--) {
					if(IsElementSelectable(SelectedElement.OwnerElements[i])) {
						prevElement = SelectedElement.OwnerElements[i];
						break;
					}
				}
				if(prevElement != null) {
					while(prevElement.Expanded && prevElement.Style == ElementStyle.Group) {
						for(int i = prevElement.Elements.Count - 1; i >= 0; i--) {
							if(IsElementSelectable(prevElement.Elements[i])) {
								prevElement = prevElement.Elements[i];
								break;
							}
							if(i == 0) {
								SelectedElement = prevElement;
								return;
							}
						}
					}
					SelectedElement = prevElement;
					return;
				}
			}
			if(SelectedElement.OwnerElement != null)
				SelectedElement = SelectedElement.OwnerElement;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class AccordionControlAppearances : BaseAppearanceCollection {
		public AccordionControlAppearances() : this(null) { }
		public AccordionControlAppearances(AccordionControl accordion)
			: base() {
				this.accordion = accordion;
				this.item = CreateElementAppearance();
				this.group = CreateElementAppearance();
				this.itemWithContainer = CreateElementAppearance();
		}
		AccordionControl accordion;
		AppearanceObject accordionControl, hint;
		AccordionControlElementAppearances item, group, itemWithContainer;
		protected override void CreateAppearances() {
			this.accordionControl = CreateAppearance("AccordionControl");
			this.hint = CreateAppearance("Hint");
			Hint.Changed += OnChanged;
			AccordionControl.Changed += OnChanged;
		}
		protected AccordionControlElementAppearances CreateElementAppearance() {
		   return new AccordionControlElementAppearances(this.accordion);
		}
		void ResetAccordionControl() { AccordionControl.Reset(); }
		bool ShouldSerializeAccordionControl() { return AccordionControl.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AccordionControl { get { return accordionControl; } }
		void ResetHint() { Hint.Reset(); }
		bool ShouldSerializeHint() { return Hint.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hint { get { return hint; } }
		void ResetItem() { Item.Reset(); }
		bool ShouldSerializeItem() { return Item.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AccordionControlElementAppearances Item { get { return item; } }
		void ResetGroup() { Group.Reset(); }
		bool ShouldSerializeGroup() { return Group.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AccordionControlElementAppearances Group { get { return group; } }
		void ResetItemWithContainer() { ItemWithContainer.Reset(); }
		bool ShouldSerializeItemWithContainer() { return ItemWithContainer.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public AccordionControlElementAppearances ItemWithContainer { get { return itemWithContainer; } }
		public override bool ShouldSerialize() {
			return base.ShouldSerialize() || Item.ShouldSerialize() || Group.ShouldSerialize() || ItemWithContainer.ShouldSerialize();
		}
		public override void Reset() {
			base.Reset();
			Item.Reset();
			Group.Reset();
			ItemWithContainer.Reset();
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			if(Item != null) Item.Dispose();
			if(Group != null) Group.Dispose();
			if(ItemWithContainer != null) ItemWithContainer.Dispose();
			DisposeAppearanceObject(Hint);
			DisposeAppearanceObject(AccordionControl);
			base.Dispose();
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null) return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(this.accordion != null) this.accordion.OnAppearanceChanged();
		}
		protected override void DeserializeCore(XtraPropertyInfo info) {
			if(info.Name == "Item") {
				DeserializeObject(Item, info);
				return;
			}
			if(info.Name == "ItemWithContainer") {
				DeserializeObject(ItemWithContainer, info);
				return;
			}
			if(info.Name == "Group") {
				DeserializeObject(Group, info);
				return;
			}
			base.DeserializeCore(info);
		}
		protected void DeserializeObject(object obj, XtraPropertyInfo info) {
			DeserializeHelper helper = new DeserializeHelper();
			helper.DeserializeObject(obj, info.ChildProperties, OptionsLayoutBase.FullLayout);
		}
		public AccordionControlAppearances Clone() {
			AccordionControlAppearances app = new AccordionControlAppearances();
			app.hint = (AppearanceObject)Hint.Clone();
			app.accordionControl = (AppearanceObject)AccordionControl.Clone();
			app.item = Item.Clone();
			app.itemWithContainer = ItemWithContainer.Clone();
			app.group = Group.Clone();
			return app;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class AccordionControlElementAppearances : BaseAppearanceCollection {
		public AccordionControlElementAppearances() : base() { }
		public AccordionControlElementAppearances(AccordionControl accordion)
			: base() {
				this.accordionControl = accordion;
		}
		public AccordionControlElementAppearances(AccordionControlElement element)
			: base() {
			this.element = element;
		}
		AccordionControl accordionControl;
		AccordionControlElement element;
		AccordionControl AccordionControl {
			get {
				if(this.element != null)
					return this.element.AccordionControl;
				return this.accordionControl;
			}
		}
		AppearanceObject normal, hovered, pressed, disabled;
		protected override void CreateAppearances() {
			this.normal = CreateAppearance("Normal");
			this.hovered = CreateAppearance("Hovered");
			this.pressed = CreateAppearance("Pressed");
			this.disabled = CreateAppearance("Disabled");
			Normal.Changed += OnChanged;
			Hovered.Changed += OnChanged;
			Pressed.Changed += OnChanged;
			Disabled.Changed += OnChanged;
		}
		protected void OnChanged(object sender, EventArgs e) {
			if(AccordionControl != null) AccordionControl.OnAppearanceChanged();
		}
		void ResetNormal() { Normal.Reset(); }
		bool ShouldSerializeNormal() { return Normal.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Normal { get { return normal; } }
		void ResetHovered() { Hovered.Reset(); }
		bool ShouldSerializeHovered() { return Hovered.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hovered { get { return hovered; } }
		[Obsolete("Use Hovered instead of Hover"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AppearanceObject Hover { get { return Hovered; } }
		void ResetPressed() { Pressed.Reset(); }
		bool ShouldSerializePressed() { return Pressed.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Pressed { get { return pressed; } }
		void ResetDisabled() { Disabled.Reset(); }
		bool ShouldSerializeDisabled() { return Disabled.ShouldSerialize(); }
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Disabled { get { return disabled; } }
		public override string ToString() {
			return string.Empty;
		}
		public override void Dispose() {
			DisposeAppearanceObject(Normal);
			DisposeAppearanceObject(Pressed);
			DisposeAppearanceObject(Hovered);
			DisposeAppearanceObject(Disabled);
			base.Dispose();
		}
		public AccordionControlElementAppearances Clone() {
			AccordionControlElementAppearances app = new AccordionControlElementAppearances();
			app.disabled = (AppearanceObject)Disabled.Clone();
			app.hovered = (AppearanceObject)Hovered.Clone();
			app.normal = (AppearanceObject)Normal.Clone();
			app.pressed = (AppearanceObject)Pressed.Clone();
			return app;
		}
		void DisposeAppearanceObject(AppearanceObject app) {
			if(app == null) return;
			app.Changed -= OnChanged;
			app.Dispose();
		}
	}
	public class AccordionControlHintInfo : IComparable {
		bool show;
		string text;
		object hintObject;
		AppearanceObject appearance;
		static AccordionControlHintInfo empty = null;
		public static AccordionControlHintInfo Empty {
			get {
				if(empty == null) empty = new AccordionControlHintInfo(null);
				return empty;
			}
		}
		public AccordionControlHintInfo(AppearanceObject appearance) {
			this.show = false;
			this.text = string.Empty;
			this.hintObject = null;
			this.appearance = appearance;
		}
		protected internal void SetHint(object hintObject, string text) {
			this.hintObject = hintObject;
			this.text = text;
			this.show = true;
		}
		public AppearanceObject Appearance { get { return appearance; } set { appearance = value; } }
		public bool Show { get { return show; } set { show = value; } }
		public string Text { get { return text; } set { text = value; } }
		public object HintObject { get { return hintObject; } set { hintObject = value; } }
		int IComparable.CompareTo(object obj) {
			AccordionControlHintInfo hi = obj as AccordionControlHintInfo;
			if(hi == null) return -1;
			return (Show == hi.Show && Text == hi.Text && HintObject == hi.HintObject) ? 0 : -1;
		}
		public override bool Equals(object obj) {
			return ((IComparable)this).CompareTo(obj) == 0;
		}
		public override int GetHashCode() { return base.GetHashCode(); }
	}
	class AccordionControlCriteriaProvider : SearchControlCriteriaProviderBase {
		protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
			return DxFtsContainsHelper.Create(new string[] { "Caption" }, result, args.FilterCondition);
		}
		protected override Data.Helpers.FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args) {
			return new FindSearchParser().Parse(args.SearchText);
		}
		protected override void DisposeCore() {
		}
	}
	public interface IFilterContent {
		Rectangle Bounds { get; }
		bool Visible { get; set; }
		event EventHandler FilterValueChanged;
		object FilterValue { get; }
		Point Location { get; set; }
		UserLookAndFeel LookAndFeel { get; }
		Control Control { get; }
		int Width { get; set; }
		void UpdateLayout();
		void UpdateAppearance();
	}
	public class AccordionContextItemCollectionOptions : SimpleContextItemCollectionOptions {
		public AccordionContextItemCollectionOptions(IContextItemCollectionOptionsOwner owner)
			: base(owner) {
		}
		public override string ToString() {
			return string.Empty;
		}
	}
	public class AccordionContextButton : SimpleContextButton { }
	public class AccordionTrackBarContextButton : SimpleTrackBarContextButton { }
	public class AccordionCheckContextButton : SimpleCheckContextButton { }
	public class AccordionRatingContextButton : SimpleRatingContextButton { }
	public enum AccordionControlState { Minimized, Normal }
	public enum MinimizeButtonMode { Default, Inverted }
	public class OptionsMinimizing : BaseOptions {
		AccordionControl owner;
		bool allowMinimizing;
		int minimizedWidth, normalWidth;
		AccordionControlState state;
		MinimizeButtonMode minimizeButtonMode;
		public OptionsMinimizing(AccordionControl owner) {
			this.owner = owner;
			this.allowMinimizing = false;
			this.minimizedWidth = -1;
			this.normalWidth = -1;
			this.state = AccordionControlState.Normal;
			this.minimizeButtonMode = MinimizeButtonMode.Default;
		}
		[ Category(CategoryName.Appearance), DefaultValue(false)]
		public bool AllowMinimizing {
			get { return allowMinimizing; }
			set {
				if(AllowMinimizing == value)
					return;
				allowMinimizing = value;
				OnPropertiesChanged();
			}
		}
		void OnPropertiesChanged() {
			this.owner.ForceLayoutChanged();
		}
		[ Category(CategoryName.Appearance), DefaultValue(AccordionControlState.Normal)]
		public AccordionControlState State {
			get { return state; }
			set {
				if(State == value)
					return;
				state = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(-1)]
		public int MinimizedWidth {
			get { return minimizedWidth; }
			set {
				if(MinimizedWidth == value)
					return;
				minimizedWidth = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(-1)]
		public int NormalWidth {
			get { return normalWidth; }
			set {
				if(NormalWidth == value)
					return;
				normalWidth = value;
				OnPropertiesChanged();
			}
		}
		[ DefaultValue(MinimizeButtonMode.Default)]
		public MinimizeButtonMode MinimizeButtonMode {
			get { return minimizeButtonMode; }
			set {
				if(MinimizeButtonMode == value)
					return;
				minimizeButtonMode = value;
				OnPropertiesChanged();
			}
		}
	}
	public class AccordionDefaultAppearances {
		AccordionControlViewInfo viewInfo;
		AppearanceDefault itemNormal, itemHovered, itemPressed, itemDisabled;
		AppearanceDefault itemWithContainerNormal, itemWithContainerHovered, itemWithContainerPressed, itemWithContainerDisabled;
		AppearanceDefault groupNormal, groupHovered, groupPressed, groupDisabled;
		AppearanceDefault rootElementNormal, rootElementHovered, rootElementPressed, rootElementDisabled;
		public AccordionDefaultAppearances(AccordionControlViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			Update();
		}
		public AccordionControlViewInfo ViewInfo { get { return viewInfo; } }
		public virtual void Update() {
			this.itemNormal = CreateAppearance(ElementStyle.Item, false, ObjectState.Normal, false);
			this.itemPressed = CreateAppearance(ElementStyle.Item, false, ObjectState.Pressed, false);
			this.itemHovered = CreateAppearance(ElementStyle.Item, false, ObjectState.Hot, false);
			this.itemDisabled = CreateAppearance(ElementStyle.Item, false, ObjectState.Disabled, false);
			this.itemWithContainerNormal = CreateAppearance(ElementStyle.Item, false, ObjectState.Normal, true);
			this.itemWithContainerPressed = CreateAppearance(ElementStyle.Item, false, ObjectState.Pressed, true);
			this.itemWithContainerHovered = CreateAppearance(ElementStyle.Item, false, ObjectState.Hot, true);
			this.itemWithContainerDisabled = CreateAppearance(ElementStyle.Item, false, ObjectState.Disabled, true);
			this.rootElementNormal = CreateAppearance(ElementStyle.Group, true, ObjectState.Normal, false);
			this.rootElementPressed = CreateAppearance(ElementStyle.Group, true, ObjectState.Pressed, false);
			this.rootElementHovered = CreateAppearance(ElementStyle.Group, true, ObjectState.Hot, false);
			this.rootElementDisabled = CreateAppearance(ElementStyle.Group, true, ObjectState.Disabled, false);
			this.groupNormal = CreateAppearance(ElementStyle.Group, false, ObjectState.Normal, false);
			this.groupPressed = CreateAppearance(ElementStyle.Group, false, ObjectState.Pressed, false);
			this.groupHovered = CreateAppearance(ElementStyle.Group, false, ObjectState.Hot, false);
			this.groupDisabled = CreateAppearance(ElementStyle.Group, false, ObjectState.Disabled, false);
		}
		public AppearanceDefault ItemNormal { get { return itemNormal; } }
		public AppearanceDefault ItemHovered { get { return itemHovered; } }
		public AppearanceDefault ItemPressed { get { return itemPressed; } }
		public AppearanceDefault ItemDisabled { get { return itemDisabled; } }
		public AppearanceDefault ItemWithContainerNormal { get { return itemWithContainerNormal; } }
		public AppearanceDefault ItemWithContainerHovered { get { return itemWithContainerHovered; } }
		public AppearanceDefault ItemWithContainerPressed { get { return itemWithContainerPressed; } }
		public AppearanceDefault ItemWithContainerDisabled { get { return itemWithContainerDisabled; } }
		public AppearanceDefault GroupNormal { get { return groupNormal; } }
		public AppearanceDefault GroupHovered { get { return groupHovered; } }
		public AppearanceDefault GroupPressed { get { return groupPressed; } }
		public AppearanceDefault GroupDisabled { get { return groupDisabled; } }
		public AppearanceDefault RootElementNormal { get { return rootElementNormal; } }
		public AppearanceDefault RootElementHovered { get { return rootElementHovered; } }
		public AppearanceDefault RootElementPressed { get { return rootElementPressed; } }
		public AppearanceDefault RootElementDisabled { get { return rootElementDisabled; } }
		public AppearanceDefault GetAppearance(AccordionControlElement element, ObjectState state) {
			if(element.OwnerElement == null) {
				if(state == ObjectState.Hot) return RootElementHovered;
				if(state == ObjectState.Pressed) return RootElementPressed;
				if(state == ObjectState.Disabled) return RootElementDisabled;
				return RootElementNormal;
			}
			if(element.Style == ElementStyle.Group) {
				if(state == ObjectState.Hot) return GroupHovered;
				if(state == ObjectState.Pressed) return GroupPressed;
				if(state == ObjectState.Disabled) return GroupDisabled;
				return GroupNormal;
			}
			if(element.HasContentContainer) {
				if(state == ObjectState.Hot) return ItemWithContainerHovered;
				if(state == ObjectState.Pressed) return ItemWithContainerPressed;
				if(state == ObjectState.Disabled) return ItemWithContainerDisabled;
				return ItemWithContainerNormal;
			}
			if(state == ObjectState.Hot) return ItemHovered;
			if(state == ObjectState.Pressed) return ItemPressed;
			if(state == ObjectState.Disabled) return ItemDisabled;
			return ItemNormal;
		}
		private AppearanceDefault CreateAppearance(ElementStyle style, bool isRootElement, ObjectState state, bool hasContentContainer) {
			AppearanceDefault def = new AppearanceDefault() { ForeColor = GetDefaultForeColor(style, isRootElement, state) };
			CheckDefaultAppearanceBackColor(def);
			SkinElement elem = ViewInfo.GetHeaderSkinElement(style, isRootElement);
			if(elem != null) {
				def.Font = hasContentContainer ? elem.GetFont(def.Font, FontStyle.Bold) : elem.GetFont(def.Font);
			}
			return def;
		}
		protected void CheckDefaultAppearanceBackColor(AppearanceDefault def) {
			if(ViewInfo.UseFlatStyle) def.BackColor = FlatPaintHelper.HeaderBackColor;
			if(ViewInfo.UseOffice2003Style) {
				def.BackColor = Office2003PaintHelper.HeaderBackColor;
				def.BackColor2 = Office2003PaintHelper.HeaderBackColor2;
			}
		}
		protected virtual Color GetDefaultForeColor(ElementStyle style, bool isRootElement, ObjectState state) {
			DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel = ViewInfo.LookAndFeel.ActiveLookAndFeel;
			if(state == ObjectState.Disabled)
				return CommonSkins.GetSkin(lookAndFeel).GetSystemColor(SystemColors.GrayText);
			if(ViewInfo.UseFlatStyle) return FlatPaintHelper.HeaderForeColor;
			if(ViewInfo.UseOffice2003Style) return Office2003PaintHelper.HeaderForeColor;
			if(ViewInfo.IsMinimized) {
				SkinElement elem = ViewInfo.GetMinimizedSkinElement();
				if(elem != null) return GetForeColorCore(elem, state);
			}
			if(isRootElement) {
				SkinElement elem = AccordionControlSkins.GetSkin(lookAndFeel)[AccordionControlSkins.SkinRootGroup];
				if(elem != null) return GetForeColorCore(elem, state);
				return NavBarSkins.GetSkin(lookAndFeel)[NavBarSkins.SkinGroupHeader].Color.GetForeColor();
			}
			if(style == ElementStyle.Group) {
				SkinElement elem = AccordionControlSkins.GetSkin(lookAndFeel)[AccordionControlSkins.SkinGroup];
				if(elem != null) return GetForeColorCore(elem, state);
			}
			else {
				SkinElement elem = AccordionControlSkins.GetSkin(lookAndFeel)[AccordionControlSkins.SkinItem];
				if(elem != null) return GetForeColorCore(elem, state);
			}
			if(state == ObjectState.Pressed)
				return NavPaneSkins.GetSkin(lookAndFeel)[NavPaneSkins.SkinItemSelected].Color.GetForeColor();
			return NavPaneSkins.GetSkin(lookAndFeel)[NavPaneSkins.SkinItem].Color.GetForeColor();
		}
		protected Color GetForeColorCore(SkinElement elem, ObjectState state) {
			Color defColor = elem.Color.GetForeColor();
			if(state == ObjectState.Hot) return elem.Properties.GetColor("ForeColorHot", defColor);
			if(state == ObjectState.Pressed) return elem.Properties.GetColor("ForeColorPressed", defColor);
			return defColor;
		}
	}
	[ToolboxItem(false)]
	public class AccordionExpandButtonWindow : DXLayeredWindowEx, ISupportXtraAnimationEx {
		Control parent;
		Bitmap newImage, prevImage, currentImage;
		public AccordionExpandButtonWindow(Control parent)
			: base() {
			this.parent = parent;
			this.newImage = this.prevImage = this.currentImage = null;
			this.showingTimer = CreateShowingTimer();
			this.showingLocation = Point.Empty;
			IsInAnimation = false;
		}
		public Bitmap NewImage {
			get { return newImage; }
			set {
				if(NewImage == value)
					return;
				PrevImage = CurrentImage;
				newImage = value;
				if(IsVisible)
					RunBitmapChangingAnimation();
			}
		}
		public Bitmap PrevImage { get { return prevImage; } set { prevImage = value; } }
		public Bitmap CurrentImage { get { return currentImage; } set { currentImage = value; } }
		protected override void DrawCore(GraphicsCache cache) {
			cache.Graphics.Clear(Color.Transparent);
			if(CurrentImage != null) cache.Graphics.DrawImage(CurrentImage, Point.Empty);
		}
		protected override void OnDisposing() {
			if(IsInAnimation) XtraAnimator.Current.Animations.Remove(this);
			this.prevImage = null;
			this.currentImage = null;
			this.newImage = null;
			this.showingTimer.Dispose();
			this.showingTimer = null;
			base.OnDisposing();
		}
		Point showingLocation;
		public void Show(Point p, Bitmap bitmap) {
			NewImage = bitmap;
			if(bitmap == null) {
				return;
			}
			else {
				Size = bitmap.Size;
			}
			Create(ParentHandle);
			if(IsVisible) ForceShow();
			else RunShowingTimer(p);
		}
		void RunShowingTimer(Point p) {
			this.showingLocation = p;
			this.showingTimer.Enabled = true;
		}
		protected IntPtr ParentHandle {
			get {
				if(parent != null && parent.IsHandleCreated)
					return parent.Handle;
				return IntPtr.Zero;
			}
		}
		Timer showingTimer;
		Timer CreateShowingTimer() {
			Timer timer = new Timer();
			timer.Interval = 200;
			timer.Tick += OnShowingTimerTick;
			return timer;
		}
		void OnShowingTimerTick(object sender, EventArgs e) {
			((Timer)sender).Enabled = false;
			ForceShow();
		}
		void ForceShow() {
			if(NewImage != null) {
				Show(showingLocation);
				Update();
				RunBitmapChangingAnimation();
			}
			else ForceDispose();
		}
		bool ISupportXtraAnimation.CanAnimate { get { return true; } }
		Control ISupportXtraAnimation.OwnerControl { get { return null; } }
		void ISupportXtraAnimationEx.OnEndAnimation(BaseAnimationInfo info) {
			IsInAnimation = false;
			if(NewImage == null) {
				ForceDispose();
			}
		}
		void ForceDispose() {
			AccordionControl accordion = this.parent as AccordionControl;
			if(accordion != null) accordion.ControlInfo.DisposeExpandButton();
		}
		void ISupportXtraAnimationEx.OnFrameStep(BaseAnimationInfo info) {
			if(PrevImage == null && NewImage == null) return;
			Size imgSize = NewImage == null ? PrevImage.Size : NewImage.Size;
			float value = (float)((DoubleSplineAnimationInfo)info).Value;
			CurrentImage = new Bitmap(imgSize.Width, imgSize.Height);
			using(Graphics g = Graphics.FromImage(CurrentImage)) {
				if(PrevImage != null)
					g.DrawImage(PrevImage, new Rectangle(Point.Empty, imgSize), 0, 0, imgSize.Width, imgSize.Height, GraphicsUnit.Pixel, XPaint.CreateImageAttributesWithOpacity(1 - value));
				if(NewImage != null)
					g.DrawImage(NewImage, new Rectangle(Point.Empty, imgSize), 0, 0, imgSize.Width, imgSize.Height, GraphicsUnit.Pixel, XPaint.CreateImageAttributesWithOpacity(value));
			}
			Invalidate();
		}
		void RunBitmapChangingAnimation() {
			if(IsInAnimation) XtraAnimator.Current.Animations.Remove(this);
			XtraAnimator.Current.AddAnimation(new DoubleSplineAnimationInfo(this, this, 0, 1, 500));
			IsInAnimation = true;
		}
		bool IsInAnimation { get; set; }
	}
	[ToolboxItem(false)]
	public class AccordionControlForm : TopFormBase {
		XtraFormShadow shadow;
		public AccordionControlForm(AccordionControl ownerControl, AccordionControlElementCollection elements, bool isRootLevel)
			: base() {
			SetStyle(ControlStyles.AllPaintingInWmPaint, false);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.accordionControl = CreateAccordionControl(elements, isRootLevel);
			this.accordionControl.ElementClick += OnElementClick;
			CloneOwnerProrerties(ownerControl);
			Controls.Add(accordionControl);
			CalcContent();
			this.ownerElement = elements.Element;
			this.ownerControl = ownerControl;
			this.shadow = new XtraFormShadow();
			this.shadow.Form = this;
			Opacity = 0;
			this.accordionControl.Paint += OnAccordionControlPaint;
		}
		bool accordionControlShown = false;
		protected void OnAccordionControlPaint(object sender, PaintEventArgs e) {
			if(!this.accordionControlShown) {
				this.accordionControlShown = true;
				this.shadow.Opacity = 255;
				Opacity = 1;
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			this.shadow.Opacity = 0;
		}
		void OnElementClick(object sender, ElementClickEventArgs e) {
			AccordionControlElement elem = e.Element.TagInternal as AccordionControlElement;
			if(elem == null || elem.AccordionControl == null)
				return;
			elem.RaiseElementClick();
		}
		protected virtual void CloneOwnerProrerties(AccordionControl owner) {
			this.accordionControl.Width = owner.ControlInfo.GetExpandedWidth();
			this.accordionControl.AnimationType = owner.AnimationType;
			this.accordionControl.ScrollBarMode = owner.ScrollBarMode;
			this.accordionControl.ShowGroupExpandButtons = owner.ShowGroupExpandButtons;
			this.accordionControl.LookAndFeel.ParentLookAndFeel = owner.LookAndFeel;
			this.accordionControl.ItemHeight = owner.ItemHeight;
			this.accordionControl.GroupHeight = owner.GroupHeight;
			this.accordionControl.ExpandElementMode = owner.ExpandElementMode;
			this.accordionControl.ExpandGroupOnHeaderClick = owner.ExpandGroupOnHeaderClick;
			this.accordionControl.DistanceBetweenRootGroups = owner.DistanceBetweenRootGroups;
			this.accordionControl.AllowGlyphSkinning = owner.AllowGlyphSkinning;
			this.accordionControl.AllowHtmlText = owner.AllowHtmlText;
			this.accordionControl.SetAppearance(owner.Appearance);
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			if(this.shadow != null)
				this.shadow.Opacity = 0;
			Opacity = 0;
			Dispose();
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_DESTROY) {
				Destroying();
			}
			base.WndProc(ref m);
		}
		protected void Destroying() {
			if(OwnerControl == null || OwnerControl.Disposing || !OwnerControl.Focused || !OwnerControl.IsHandleCreated)
				return;
			if(ControlUtils.MouseButtons == MouseButtons.Left) {
				ProcessOwnerMouseDown();
			}
		}
		protected void ProcessOwnerMouseDown() {
			Point pos = OwnerControl.PointToClient(Cursor.Position);
			AccordionElementBaseViewInfo info = OwnerControl.CalcHitInfo(pos).ItemInfo;
			if(info != null && object.Equals(OwnerElement, info.Element))
				return;
			IntPtr lParam = (IntPtr)((pos.Y << 16) | pos.X);
			NativeMethods.SendMessage(OwnerControl.Handle, MSG.WM_LBUTTONDOWN, IntPtr.Zero, lParam);
		}
		AccordionControl accordionControl, ownerControl;
		AccordionControlElementBase ownerElement;
		public AccordionControl AccordionControl { get { return accordionControl; } }
		public AccordionControl OwnerControl { get { return ownerControl; } }
		public AccordionControlElementBase OwnerElement { get { return ownerElement; } }
		public virtual AccordionControl CreateAccordionControl(AccordionControlElementCollection elements, bool isRootLevel) {
			AccordionControl newAccordion = new AccordionControl();
			if(!isRootLevel) {
				newAccordion.Elements.Add(new AccordionControlElement() { HeaderVisible = false });
			}
			AccordionControlElementCollection col = isRootLevel ? newAccordion.Elements : newAccordion.Elements[0].Elements;
			AccordionControlElementCollection newCol = elements.Clone();
			while(newCol.Count > 0) {
				col.Add(newCol[0]);
			}
			return newAccordion;
		}
		public void Show(Point p) {
			Location = p;
			Show();
			AccordionControl.Focus();
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			DrawFormBorder(e);
		}
		public virtual void CalcContent() {
			SkinPaddingEdges margins = GetContentMargins();
			AccordionControl.Location = new Point(margins.Left, margins.Top);
			Size = new Size(AccordionControl.Width + margins.Width, AccordionControl.Height + margins.Height);
		}
		public virtual SkinElement GetBorderSkinElement() {
			return NavPaneSkins.GetSkin(AccordionControl.LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinNavPaneFormBorder];
		}
		protected virtual SkinPaddingEdges GetContentMargins() {
			SkinElement elem = GetBorderSkinElement();
			SkinPaddingEdges margins = (SkinPaddingEdges)elem.ContentMargins.Clone();
			margins.Left++; 
			margins.Right++;
			return margins;
		}
		protected void DrawFormBorder(PaintEventArgs e) {
			using(GraphicsCache cache = new GraphicsCache(e)) {
				cache.FillRectangle(AccordionControl.ControlInfo.GetBackColor(), ClientRectangle);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, new SkinElementInfo(GetBorderSkinElement(), ClientRectangle));
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.accordionControl != null) {
					this.accordionControl.ElementClick -= OnElementClick;
					this.accordionControl.Dispose();
					this.accordionControl = null;
				}
				if(this.ownerControl != null) {
					this.ownerControl.ControlInfo.ResetPressedInfo();
					this.ownerControl.ControlInfo.ResetHoverInfo();
					this.ownerControl.Focus();
				}
				if(this.shadow != null) {
					this.shadow.Dispose();
					this.shadow = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
