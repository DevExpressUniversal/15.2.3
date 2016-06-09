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
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.Utils;
using DevExpress.Utils.Design;
#if DXWhidbey
using System.Windows.Forms.Design.Behavior;
using DevExpress.Serialization.CodeDom;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Internal;
using System.Collections.Generic;
using System.Linq;
#endif
namespace DevExpress.XtraEditors.Design {
	public class BaseRepositoryItemDesigner : BaseComponentDesigner, IInheritanceService {
		public virtual RepositoryItem Item { get { return Component as RepositoryItem; } }
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		BaseEdit GetOwnerEdit() { 
			RepositoryItem item = Item;
			while(item != null) {
				if(item.OwnerEdit != null)
					return item.OwnerEdit;
				item = item.OwnerItem;
			}
			return null;
		}
		protected override object GetService(Type type) {
			if((GetOwnerEdit() != null || Item.OwnerItem != null) && type.Equals(typeof(IInheritanceService))) {
				return this;
			}
			return base.GetService(type);
		}
		void IInheritanceService.AddInheritedComponents(IComponent component, IContainer container) { }
		InheritanceAttribute IInheritanceService.GetInheritanceAttribute(IComponent component) {
			BaseEdit ownerEdit = GetOwnerEdit();
			ISite site = null;
			if(ownerEdit != null)
				site = ownerEdit.Site;
			else if(Item.OwnerItem != null)
				site = Item.OwnerItem.Site;
			if(site == null) return InheritanceAttribute.Default;
			IInheritanceService srv = site.GetService(typeof(IInheritanceService)) as IInheritanceService;
			if(srv == null) return InheritanceAttribute.Default;
			return srv.GetInheritanceAttribute(ownerEdit != null ? (IComponent)ownerEdit : (IComponent)Item.OwnerItem);
		}
	}
	public class SparklineRepositoryItemDesigner : BaseRepositoryItemDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ProjectHelper.AddReference(component, AssemblyInfo.SRAssemblySparklineCore);
		}
	}
	public class SparklineEditDesigner : BaseEditDesigner {
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ProjectHelper.AddReference(component, AssemblyInfo.SRAssemblySparklineCore);
		}
	}
	public class EditDesignerVerb : DesignerVerb {
		public EditDesignerVerb(string text, EventHandler handler) : base(text, handler) {
		}
		public override bool Checked { 
			get { return BaseEditViewInfo.ShowFieldBindings; } 
			set {
				BaseEditViewInfo.ShowFieldBindings = value;
				base.Checked = value;
			}
		}
	}
	public class DropDownButtonDesigner : SimpleButtonDesigner {		
		public new virtual DropDownButton Button { get { return Control as DropDownButton; } }
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(Button == null || Button.Container == null) return;
			if(Button.MenuManager == null)
				Button.MenuManager = ControlDesignerHelper.GetBarManager(Button.Container);
		}
	}
	public class BaseEditDesigner : BaseControlDesigner {
		IComponentChangeService changeService = null;
		DesignerVerbCollection verbs;
		protected DesignerVerb showBindings;
		internal protected static ArrayList Designers = new ArrayList();
		public BaseEditDesigner() {
			Designers.Add(this);
			this.verbs = new DesignerVerbCollection();
			showBindings = new EditDesignerVerb(Properties.Resources.ShowBindingsCaption, new EventHandler(OnVerb_ShowBindings));
			showBindings.Checked = false;
			this.verbs.Add(new DesignerVerb(Properties.Resources.AboutCaption, new EventHandler(OnVerb_About)));
#if !DXWhidbey //doesnt necessary to show bindings under new environment
			this.verbs.Add(showBindings);
#endif
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
		protected virtual void OnVerb_About(object sender, EventArgs e) {
			BaseEdit.About();
		}
		protected override DesignerActionListCollection CreateActionLists() {
			if(Editor == null)
				return new DesignerActionListCollection();
			return base.CreateActionLists();
		}
		protected virtual void OnVerb_ShowBindings(object sender, EventArgs e) {
			showBindings.Checked = BaseEditViewInfo.ShowFieldBindings = !BaseEditViewInfo.ShowFieldBindings;
			foreach(BaseEditDesigner des in Designers) {
				if(des.Editor == null || !des.Editor.IsHandleCreated) return;
				des.showBindings.Checked = BaseEditViewInfo.ShowFieldBindings;
				des.Editor.Refresh();
			}
		}
		public BaseEditViewInfo ViewInfo { get { return Editor.GetViewInfo() as BaseEditViewInfo; } }
		public virtual BaseEdit Editor { get { return Control as BaseEdit; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(component.Site != null) {
				changeService = component.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				changeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
			}
		}
		public override SelectionRules SelectionRules { 
			get {
				SelectionRules res = base.SelectionRules;
				if(Editor == null) return res;
				if(Editor.Properties.AutoHeight) {
					res &= ~(SelectionRules.BottomSizeable | SelectionRules.TopSizeable);
				}
				return res;
			}
		}
		public override DesignerVerbCollection Verbs { get { return verbs; } }
		bool initializNewInvoked = false;
		public override void InitializeNewComponent(IDictionary defaultValues) {
			if(this.initializNewInvoked) return;
			this.initializNewInvoked = true;
			base.InitializeNewComponent(defaultValues);
			OnInitializeNew(defaultValues);
			if(Editor == null || Editor.Container == null) return;
			if(Editor.MenuManager == null)
				Editor.MenuManager = ControlDesignerHelper.GetBarManager(Editor.Container);
		}
		protected virtual void OnInitializeNew(IDictionary defaultValues) {
			ResetEditValue();
		}
		protected override void OnInitilizeDataWindowDrop(IComponent component) {
			OnInitializeNew(null);
			if(Editor != null) {
				Editor.Top -= 3;
			}
		}
		protected virtual void ResetEditValue() {
			if(Editor != null) {
				TypeDescriptor.GetProperties(Editor.GetType())["EditValue"].ResetValue(Editor);
			}
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			DXPropertyDescriptor.ConvertDescriptors(properties, null);
		}
		protected override void Dispose(bool disposing) {
			Designers.Remove(this);
			if(disposing) {
				if(changeService != null)
					changeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
				changeService = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void ResetReferenceName() {
			IReferenceService refServ = GetService(typeof(IReferenceService)) as IReferenceService;
			if(refServ == null) return;
			FieldInfo fi = refServ.GetType().GetField("referenceList", BindingFlags.Instance | BindingFlags.GetField| BindingFlags.NonPublic);
			if(fi == null) return;
			ArrayList refList = fi.GetValue(refServ) as ArrayList;
			if(refList != null) {
				foreach(object obj in refList) {
					PropertyInfo pi = obj.GetType().GetProperty("SitedComponent", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);
					if(pi == null) continue;
					object val = pi.GetValue(obj, null);
					if(val != Editor) continue;
					MethodInfo me = obj.GetType().GetMethod("ResetName", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
					if(me != null) me.Invoke(obj, null);
				}
			}
		}
		protected void OnComponentRename(object sender, ComponentRenameEventArgs e) {
			if(e.Component == Editor) {
				ResetReferenceName(); 
			}
		}
		protected override void OnPaintAdornments(PaintEventArgs e) {
			base.OnPaintAdornments(e);
			if(Control.DataBindings.Count == 0) return;
			BaseEditViewInfo vi = ViewInfo;
			if(vi == null || !BaseEditViewInfo.ShowFieldBindings) return;
			string text = vi.GetDataBindingText();
			if(text == "") return;
			Rectangle bounds = vi.ClientRect;
			using(Brush backBrush = new SolidBrush(Color.FromArgb(140, DevExpress.Utils.ColorUtils.FlatBarItemHighLightBackColor)),
					  foreBrush = new SolidBrush(SystemColors.ControlText)) {
				e.Graphics.FillRectangle(backBrush, bounds);
				bounds.Inflate(-1, 0);
				e.Graphics.DrawString(text, Editor.Properties.Appearance.Font, foreBrush, bounds, Editor.Properties.Appearance.GetStringFormat());
			}
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new BaseEditActionList(this));
			base.RegisterActionLists(list);
		}
		protected override DXAboutActionList GetAboutAction() {
			return null;
		}
		protected virtual bool UpdateSnapLine(ArrayList lines, SnapLineType type, int newOffset) {
			if(lines == null || lines.Count == 0) return false;
			foreach(SnapLine line in lines) {
				if(line.SnapLineType == type) {
					line.AdjustOffset(newOffset - line.Offset);
					return true;
				}
			}
			return false;	
		}
		public override IList SnapLines {
			get {
				ArrayList res = new ArrayList();
				if(Editor == null)
					return res;
				res.AddRange(base.SnapLines);
				BaseEditViewInfo vi = this.Editor.GetViewInfo() as BaseEditViewInfo;
				Rectangle tb = vi.GetTextBounds();
				if(!tb.IsEmpty) {
					res.Add(new SnapLine(SnapLineType.Baseline, (tb.Top - vi.Bounds.Top) + vi.GetTextAscentHeight() + 2, SnapLinePriority.Medium));
					UpdateSnapLine(res, SnapLineType.Top, tb.Top - vi.Bounds.Top);
				}
				return res;
			}
		}
	}
	public class BaseEditAboutActionList : DesignerActionList {
		public BaseEditAboutActionList(IComponent component) : base(component) { }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "About", Properties.Resources.AboutCaption, "Information"));
			return res;
		}
		public virtual void About() { BaseEdit.About(); }
	}
	public class BaseEditActionList : DesignerActionList {		
		public BaseEditActionList(BaseEditDesigner designer) : base(designer.Component) { }
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionPropertyItem("EditorType", Properties.Resources.ChangeEditorTypeCaption, "Editor Type"));
			if(Editor.DataBindings.Count > 0) 
				res.Add(new DesignerActionPropertyItem("ShowBindings", Properties.Resources.ShowBindingsCaption));
			return res;
		}
		[Editor(typeof(EditorTypeEditor), typeof(UITypeEditor))]
		public string EditorType {
			get { return Editor.EditorTypeName; }
			set {
				if(EditorType == value) return;
				ChangeType(value);
			}
		}
		protected virtual void ChangeType(string editorTypeName) {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return;
			DesignerTransaction transaction = host.CreateTransaction("ChangeEditorType");
			string name = null;
			BaseEdit edit = null;
			ISelectionService selService = GetService(typeof(ISelectionService)) as ISelectionService;
			try {
				EditorClassInfo info = Registrator.EditorRegistrationInfo.Default.Editors[editorTypeName];
				if(info == null) throw new ArgumentException(Properties.Resources.EditorNotFoundCaption);
				edit = info.CreateEditor();
				host.Container.Add(edit);
				IDesigner designer = host.GetDesigner(edit);
				IComponentInitializer initializer = designer as IComponentInitializer;
				if(initializer != null) initializer.InitializeNewComponent(null);
				Hashtable properties = new Hashtable(), events = new Hashtable();
				CopyProperties(Editor, typeof(Control), properties);
				CopyEvents(Editor, events);
				bool? autoHeight = null;
				if(!Editor.Properties.AutoHeight) {
					if(Editor is MemoEdit || Editor is PictureEdit) 
						autoHeight = true;
					else
						autoHeight = false;
				}
				edit.Properties.Assign(Editor.Properties);
				edit.EditValue = Editor.EditValue;
				ArrayList biCache = new ArrayList(Editor.DataBindings);
				foreach(Binding bi in biCache) {
					try {
						Editor.DataBindings.Remove(bi);
						edit.DataBindings.Add(bi);
					}
					catch { }
				}
				CopyBaseEditProperties(Editor, edit);
				CopyButtons(Editor, edit);
				Rectangle bounds = Editor.Bounds;
				Control parent = Editor.Parent;
				name = Editor.Name;
				host.DestroyComponent(Editor);
				if(edit.Parent == null) edit.Parent = parent;
				edit.Bounds = bounds;
				if(edit.Site != null) edit.Site.Name = name;
				PasteProperties(edit, properties);
				PasteEvents(edit.Site, edit, events);
				if(autoHeight != null) edit.Properties.AutoHeight = autoHeight.Value;
			}
			catch {
				transaction.Cancel();
				throw;
			}
			transaction.Commit();
			if(selService != null && edit != null) selService.SetSelectedComponents(new object[] { edit });
		}
		void CopyBaseEditProperties(BaseEdit source, BaseEdit dest) {
			dest.TabStop = source.TabStop;
			dest.MenuManager = source.MenuManager;
			dest.ToolTipController = source.ToolTipController;
			dest.ToolTip = source.ToolTip;
			dest.ToolTipIconType = source.ToolTipIconType;
			dest.ToolTipTitle = source.ToolTipTitle;
			if(source.SuperTip != null)
				dest.SuperTip = source.SuperTip.Clone() as SuperToolTip;
		}
		void CopyButtons(BaseEdit source, BaseEdit dest) {
			ButtonEdit destButtonEdit = dest as ButtonEdit;
			ButtonEdit sourceButtonEdit = source as ButtonEdit;
			if(destButtonEdit == null || sourceButtonEdit == null) return;
			CopyPopupButtons(source, dest);
			CopySpinButtons(source, dest);
		}
		void CopySpinButtons(BaseEdit source, BaseEdit dest) {
			BaseSpinEdit destSpinBaseEdit = dest as BaseSpinEdit;
			BaseSpinEdit sourceSpinBaseEdit = source as BaseSpinEdit;
			if((destSpinBaseEdit != null && sourceSpinBaseEdit != null) || (destSpinBaseEdit == null && sourceSpinBaseEdit == null)) return;
			if(destSpinBaseEdit != null) {
				destSpinBaseEdit.Properties.Buttons.Clear();
				destSpinBaseEdit.Properties.CreateDefaultButton();
			}
		}
		void CopyPopupButtons(BaseEdit source, BaseEdit dest) {
			PopupBaseEdit destPopupBaseEdit = dest as PopupBaseEdit;
			PopupBaseEdit sourcePopupBaseEdit = source as PopupBaseEdit;
			if((destPopupBaseEdit != null && sourcePopupBaseEdit != null) || (destPopupBaseEdit == null && sourcePopupBaseEdit == null)) return;
			ButtonEdit destButtonEdit = dest as ButtonEdit;
			if(destButtonEdit != null) {
				destButtonEdit.Properties.Buttons.Clear();
				destButtonEdit.Properties.CreateDefaultButton();
			}
		}
		protected void CopyProperties(object source, Type propertiesSource, Hashtable dictionary) {
			CopyProperties(source, TypeDescriptor.GetProperties(propertiesSource), dictionary);
		}
		bool AllowCopyProperty(PropertyDescriptor property) {
			if(property.Name == "TabStop") return false;
			if(property.IsReadOnly) return false;
			if(!property.IsBrowsable) return false;
			if(property.SerializationVisibility != DesignerSerializationVisibility.Visible) return false;
			return true;
		}
		void CopyProperties(object source, PropertyDescriptorCollection properties, Hashtable dictionary) {
			foreach(PropertyDescriptor property in properties) {
				if(!AllowCopyProperty(property)) continue;
				try {
					if(!property.ShouldSerializeValue(source)) continue;
					dictionary[property] = property.GetValue(source);
				}
				catch { }
			}
		}
		void CopyEvents(object source, Hashtable dictionary) {
			IEventBindingService eventService = GetService(typeof(IEventBindingService)) as IEventBindingService;
			if(eventService == null) return;
			foreach(EventDescriptor ev in TypeDescriptor.GetEvents(source)) {
				PropertyDescriptor pd = eventService.GetEventProperty(ev);
				object val = null;
				if(pd != null) val = pd.GetValue(source);
				if(val == null) continue;
				dictionary[ev] = val;
			}
		}
		void PasteProperties(object destination, Hashtable dictionary) {
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(destination);
			foreach(DictionaryEntry entry in dictionary) {
				PropertyDescriptor source = entry.Key as PropertyDescriptor;
				PropertyDescriptor dest = coll[source.Name];
				if(dest == null || dest.SerializationVisibility != DesignerSerializationVisibility.Visible) continue;
				if(dest.IsReadOnly || !source.PropertyType.Equals(dest.PropertyType)) continue;
				dest.SetValue(destination, entry.Value);
			}
		}
		void PasteEvents(ISite site, object destination, Hashtable dictionary) {
			if(site == null) return;
			IEventBindingService eventService = site.GetService(typeof(IEventBindingService)) as IEventBindingService;
			if(eventService == null) return;
			EventDescriptorCollection coll = TypeDescriptor.GetEvents(destination);
			foreach(DictionaryEntry entry in dictionary) {
				EventDescriptor sourceEvent = entry.Key as EventDescriptor;
				EventDescriptor dest = coll[sourceEvent.Name];
				if(dest == null || !dest.IsBrowsable || !dest.EventType.Equals(sourceEvent.EventType)) continue;
				PropertyDescriptor destProperty = eventService.GetEventProperty(dest);
				if(destProperty == null) continue;
				destProperty.SetValue(destination, entry.Value);
			}
			dictionary.Clear();
		}
		protected BaseEdit Editor { get { return Component as BaseEdit; } }
		public bool ShowBindings {
			get {
				return BaseEditViewInfo.ShowFieldBindings;
			}
			set {
				BaseEditViewInfo.ShowFieldBindings = !ShowBindings;
				foreach(BaseEditDesigner des in BaseEditDesigner.Designers) {
					if(des.Editor == null || !des.Editor.IsHandleCreated) return;
					des.Editor.Refresh();
				}
			}
		}
	}
	public class TextEditDesigner : BaseEditDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
		}
		public new TextEdit Editor { get { return base.Editor as TextEdit; } }
		public new TextEditViewInfo ViewInfo { get { return base.ViewInfo as TextEditViewInfo; } }
		public override IList SnapLines {
			get {
				ArrayList res = new ArrayList();
				res.AddRange(base.SnapLines);
				if(Editor != null && ViewInfo != null) {
					Rectangle tb = ViewInfo.GetTextBounds();
					if(!tb.IsEmpty) {
						UpdateSnapLine(res, SnapLineType.Baseline, tb.Y + ViewInfo.TextBaselineOffset);
					}
				}
				return res;
			}
		}		
	}
	public class PopupBaseEditDesigner : ButtonEditDesigner { }
	public class DateEditDesigner : PopupBaseEditDesigner {
		object ShouldSerializeEditValue() { return true; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		object EditValue {
			get { return Editor.EditValue; }
			set { Editor.EditValue = value; }
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			PropertyDescriptor pd = properties["EditValue"] as PropertyDescriptor;
			if(pd != null) properties["EditValue"] = TypeDescriptor.CreateProperty(typeof(DateEditDesigner), "EditValue", typeof(object), new Attribute[] {
				new BindableAttribute(true),
				new LocalizableAttribute(true), 
				new CategoryAttribute("Data"), 
				new RefreshPropertiesAttribute(RefreshProperties.All),
				new EditorAttribute(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
				new TypeConverterAttribute(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
			});
		}
	}
	public class TimeEditDesigner : BaseSpinEditDesigner {
		object EditValue {
			get { return Editor.EditValue; }
			set { Editor.EditValue = value; }
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			PropertyDescriptor pd = properties["EditValue"] as PropertyDescriptor;
			if(pd != null) properties["EditValue"] = TypeDescriptor.CreateProperty(typeof(TimeEditDesigner), "EditValue", typeof(object), new Attribute[] {
				new BindableAttribute(true),
				new LocalizableAttribute(true), 
				new CategoryAttribute("Data"), 
				new RefreshPropertiesAttribute(RefreshProperties.All),
				new EditorAttribute(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
				new TypeConverterAttribute(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
			});
		}
	}
	public class TimeSpanEditDesigner : TimeEditDesigner { }
	public class BaseSpinEditDesigner : ButtonEditDesigner {
		protected override void ResetEditValue() { }
	}
	public class CalcEditDesigner : ButtonEditDesigner {
		protected override void ResetEditValue() { }
	}
	public class HyperLinkEditDesigner : ButtonEditDesigner {
	}
	public class SimpleButtonDesigner : BaseButtonDesigner {
		public new SimpleButton Button { get { return base.Button as SimpleButton; } }
		protected override bool CanUseComponentSmartTags { get { return true; } }
		public override IList SnapLines {
			get {
				ArrayList res = new ArrayList();
				res.AddRange(base.SnapLines);
				if(Button != null) {
					SimpleButtonViewInfo vi = Button.GetButtonViewInfo();
					if(vi != null) {
						Rectangle tb = vi.TextBounds;
						if(tb != Rectangle.Empty) {
							res.Add(new SnapLine(SnapLineType.Baseline, tb.Bottom, SnapLinePriority.Medium));
							UpdateSnapLine(res, SnapLineType.Baseline, tb.Bottom);
						}
					}
				}
				return res;
			}
		}
	}
	public class BaseButtonDesigner : BaseControlDesigner {
		public virtual BaseButton Button { get { return Control as BaseButton; } }
		protected virtual bool UpdateSnapLine(ArrayList lines, SnapLineType type, int newOffset) {
			if(lines == null || lines.Count == 0) return false;
			foreach(SnapLine line in lines) {
				if(line.SnapLineType == type) {
					line.AdjustOffset(newOffset - line.Offset);
					return true;
				}
			}
			return false;
		}
	}
	public class SearchControlDesigner : ButtonEditDesigner {
		public new SearchControl Editor { get { return base.Editor as SearchControl; } }
		protected override void CreateDefaultButton() { }
		protected override void RemoveDefaultButtons() { }
	}
	public class ButtonEditDesigner : TextEditDesigner {
		public new ButtonEdit Editor { get { return base.Editor as ButtonEdit; } }
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			if(Editor == null) return;
#if DXWhidbey
			if(!allowRemoveDefaultButtons) return;
#endif
			RemoveDefaultButtons();
		}
		protected virtual void RemoveDefaultButtons() {
			IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
			Editor.Properties.BeginUpdate();
			try {
				for(int n = Editor.Properties.Buttons.Count - 1; n >= 0; n--) {
					EditorButton button = Editor.Properties.Buttons[n];
					if(!IsDefaultButton(button)) continue;
					Editor.Properties.Buttons.RemoveAt(n);
				}
			}
			finally {
				Editor.Properties.CancelUpdate();
				if(Editor.IsHandleCreated) Editor.Refresh();
			}
		}
#if DXWhidbey
		bool allowRemoveDefaultButtons = true;
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			CreateDefaultButton();
		}
		protected virtual void CreateDefaultButton() {
			this.allowRemoveDefaultButtons = false;
			if(Editor != null && Editor.Properties.Buttons.Count == 0)
				Editor.Properties.CreateDefaultButton();
		}
#else
		protected override void OnInitializeNew(IDictionary defaultValues) {
			base.OnInitializeNew(defaultValues);
			if(Editor != null) Editor.Properties.CreateDefaultButton();
		}
#endif
		protected virtual bool IsDefaultButton(EditorButton button) {
			PropertyInfo pi = button.GetType().GetProperty("IsDefaultButton", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
			if(pi == null || ((bool)pi.GetValue(button, null)) == false) return false;
			return true;
		}
	}   
	public class BaseCheckEditDesigner : BaseEditDesigner {
		public override IList SnapLines {
			get {
				ArrayList snapList = base.SnapLines as ArrayList;
				if(Editor != null) {
					BaseCheckEditViewInfo viewInfo = this.Editor.GetViewInfo() as BaseCheckEditViewInfo;
					if (snapList != null && viewInfo != null) {
						if (viewInfo.Item.GlyphAlignment == HorzAlignment.Far) {
							snapList.Add(new SnapLine(SnapLineType.Left, 0, SnapLinePriority.Medium));
							snapList.Add(new SnapLine(SnapLineType.Left, viewInfo.GetTextBounds().Left, SnapLinePriority.Medium));
						}
						UpdateSnapLine(snapList, SnapLineType.Left, viewInfo.CheckInfo.GlyphRect.Left);
						UpdateSnapLine(snapList, SnapLineType.Baseline, viewInfo.CheckInfo.CaptionRect.Top + viewInfo.GetTextAscentHeight() + 1);
						return snapList;
					}
				}
				return base.SnapLines;
			}
		}
		public BaseCheckEdit CheckEdit { get { return Control as BaseCheckEdit; } }
		public override SelectionRules SelectionRules {
			get {
				SelectionRules rules = base.SelectionRules;
				if(CheckEdit == null) return rules;
				if(CheckEdit.Properties.AutoWidth) rules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
				if(CheckEdit.Properties.AutoHeight) rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable);
				return rules;
			}
		}
	}
	public class BaseCheckActionList : DesignerActionList {
		BaseCheckEditDesigner designer;
		public BaseCheckActionList(BaseCheckEditDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();			
			return res;
		}
		protected BaseCheckEdit Editor {
			get { return designer.Editor as BaseCheckEdit; }
		}
		protected BaseCheckEditDesigner Designer { get { return designer; } }
		protected void FireChanged() {
			EditorContextHelper.FireChanged(designer, Editor);
		}
	}
	public class ToggleSwitchDesigner : BaseCheckEditDesigner { }
	public class CheckEditDesigner : BaseCheckEditDesigner { }
	public class ComboBoxEditDesigner : ButtonEditDesigner { }
	public class FontEditDesigner : ButtonEditDesigner { }
	public class CheckedComboBoxEditDesigner : ButtonEditDesigner {		
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new CheckedComboBoxEditActionList(this));
			base.RegisterActionLists(list);
		}
		protected class CheckedComboBoxEditActionList : ListControlActionList {
			CheckedComboBoxEditDesigner designer;
			bool boundMode;
			public CheckedComboBoxEditActionList(CheckedComboBoxEditDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
				this.boundMode = (DataSource != null);
			}
			public override DesignerActionItemCollection GetSortedActionItems() {
				if(Designer.Editor == null)
					return new DesignerActionItemCollection();
				DesignerActionItem firstItem = new DesignerActionPropertyItem("BoundMode", Properties.Resources.UseDataBoundItemsCaption, DataBindingCategoryName);
				if(boundMode || (DataSource != null)) {
					boundMode = true;
					DesignerActionItemCollection result = base.GetSortedActionItems();
					result.Insert(0, firstItem);
					return result;
				}
				DesignerActionItemCollection unboundsResult = new DesignerActionItemCollection();
				unboundsResult.Add(firstItem);						   
				return unboundsResult;
			}
			protected override void FireChanged() {
				EditorContextHelper.FireChanged(Designer, EditorProperties); 
			}
			protected override void CreateDataSource() {
				IServiceProvider serviceProvider = GetService(typeof(IDesignerHost)) as IServiceProvider;
				DevExpress.Design.DataAccess.UI.DataSourceWizard.Run(serviceProvider, EditorProperties);
			}
			protected override object DataSourceCore { get { return EditorProperties.DataSource; } set { EditorProperties.DataSource = value; } }
			protected override string DisplayMemberCore { get { return EditorProperties.DisplayMember; } set { EditorProperties.DisplayMember = value; } }
			protected override string ValueMemberCore { get { return EditorProperties.ValueMember; } set { EditorProperties.ValueMember = value; } }
			protected override Control ListControl { get { return Designer.Editor; } }
			protected RepositoryItemCheckedComboBoxEdit EditorProperties { get { return (Designer.Editor.Properties) as RepositoryItemCheckedComboBoxEdit; } }
			protected CheckedComboBoxEditDesigner Designer { get { return designer; } }
			public bool BoundMode {
				get { return boundMode; }
				set {
					if(!value) DataSource = null;
					if(DataSource == null) this.boundMode = value;
					OnRefreshContent();
				}
			}
			protected override void OnRefreshContent() {
				if(!Designer.CanUseComponentSmartTags) base.OnRefreshContent();
				else BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(this.Component);
			}
		}
	}
	public class PopupContainerControlDesigner : GroupDesigner {
		protected override bool CanUseComponentSmartTags { get { return true; } }	   
	}
	public class ImageComboBoxEditDesigner : ButtonEditDesigner {
	}
	public class BreadCrumbDesigner : ButtonEditDesigner {
		DesignerActionListCollection list = null;
		public BreadCrumbDesigner() {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(BreadCrumb.Properties.Nodes.IsEmpty) {
				BreadCrumbNode root = BreadCrumb.AddDefaultRootNode();
				if(root != null) BreadCrumb.Properties.SelectedNode = root;
			}
		}
		protected override DesignerActionListCollection CreateActionLists() {
			if(list == null) {
				list = base.CreateActionLists();
				list.Add(new BreadCrumbDesignerActionList(Component));
			}
			return list;
		}
		public BreadCrumbEdit BreadCrumb { get { return Control as BreadCrumbEdit; } }
	}
	public class BreadCrumbDesignerActionList : DesignerActionList, IImageCollectionHelper {
		public BreadCrumbDesignerActionList(IComponent component)
			: base(component) {
		}
		[TypeConverter(typeof(BreadCrumbSelectedNodeTypeConverter))]
		public BreadCrumbNode SelectedNode {
			get { return BreadCrumb.Properties.SelectedNode; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "SelectedNode", value); }
		}
		[DefaultValue(null), TypeConverter(typeof(ImageCollectionImagesConverter))]
		public object Images {
			get { return BreadCrumb.Properties.Images; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "Images", value); }
		}
		[DefaultValue(-1), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public virtual int RootImageIndex {
			get { return BreadCrumb.Properties.RootImageIndex; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "RootImageIndex", value); }
		}
		[DefaultValue(-1), Editor(typeof(ImageIndexesEditor), typeof(UITypeEditor)), ImageList("Images")]
		public virtual int ImageIndex {
			get { return BreadCrumb.Properties.ImageIndex; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "ImageIndex", value); }
		}
		[Editor(typeof(DXImageEditor), typeof(UITypeEditor))]
		public Image RootGlyph {
			get { return BreadCrumb.Properties.RootGlyph; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "RootGlyph", value); }
		}
		public int NodeDropDownRowCount {
			get { return BreadCrumb.Properties.NodeDropDownRowCount; }
			set { EditorContextHelper.SetPropertyValue(BreadCrumb.Site, BreadCrumb.Properties, "NodeDropDownRowCount", value); }
		}
		#region IImageCollectionHelper
		Control IImageCollectionHelper.OwnerControl { get { return BreadCrumb; } }
		#endregion
		protected internal BreadCrumbEdit BreadCrumb { get { return Component as BreadCrumbEdit; } }
	}
	public partial class BredCrumbNodeCollectionEditor : CollectionEditor {
		public BredCrumbNodeCollectionEditor()
			: base(typeof(BreadCrumbNodeCollection)) {
		}
		protected override CollectionEditor.CollectionForm CreateCollectionForm() {
			return new BredCrumbNodeCollectionForm(this);
		}
		public static RepositoryItemBreadCrumbEdit GetProperties(object contextInstance) {
			if(contextInstance is BreadCrumbEdit) {
				return ((BreadCrumbEdit)contextInstance).Properties;
			}
			if(contextInstance is RepositoryItemBreadCrumbEdit) {
				return (RepositoryItemBreadCrumbEdit)contextInstance;
			}
			return (RepositoryItemBreadCrumbEdit)((IDXObjectWrapper)contextInstance).SourceObject;
		}
	}
	public class BreadCrumbHistoryEditor : CollectionEditor {
		public BreadCrumbHistoryEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type itemType) {
			RepositoryItemBreadCrumbEdit properties = BredCrumbNodeCollectionEditor.GetProperties(Context.Instance);
			if(itemType == typeof(BreadCrumbHistoryItem)) {
				BreadCrumbHistoryItem item = new BreadCrumbHistoryItem();
				properties.History.Add(item);
				return item;
			}
			return base.CreateInstance(itemType);
		}
	}
	public class ColorItemCollectionEditor : CollectionEditor {
		public ColorItemCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type itemType) {
			if(itemType == typeof(ColorItem)) {
				return new ColorItem();
			}
			return base.CreateInstance(itemType);
		}
		public InnerColorPickControl OwnerControl { get { return Context.Instance as InnerColorPickControl; } }
	}
	public class BreadCrumbNodeImageIndexesEditor : ImageIndexesEditor {
		protected override object GetImageListInfo(ITypeDescriptorContext context) {
			object imageList = null;
			BreadCrumbNode node = DXObjectWrapper.GetInstance(context) as BreadCrumbNode;
			if(node != null && node.Properties != null) {
				imageList = node.Properties.Images;
			}
			return imageList;
		}
		protected override ObjectPickerControl CreateObjectPickerControl(ITypeDescriptorContext context, object value) {
			return new EditorForm(this, GetImageListInfo(context), value);
		}
		protected class EditorForm : ImageIndexesEditorForm {
			public EditorForm(BreadCrumbNodeImageIndexesEditor editor, object imageList, object editValue)
				: base(editor, imageList, editValue) {
			}
			protected override string GetItemText(int imageIndex) {
				if(BreadCrumbNode.IsNoneImageIndex(imageIndex)) return BreadCrumbNode.NoneItemText;
				if(BreadCrumbNode.IsDefaultImageIndex(imageIndex)) return BreadCrumbNode.DefaultItemText;
				return base.GetItemText(imageIndex);
			}
		}
	}
	public class BreadCrumbNodeImageIndexesTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && value != null) {
				if(BreadCrumbNode.IsDefaultImageIndex((int)value))
					return BreadCrumbNode.DefaultItemText;
				if(BreadCrumbNode.IsNoneImageIndex((int)value))
					return BreadCrumbNode.NoneItemText;
				return value.ToString();
			}
			return value.ToString();
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string valueCore = value as string;
			if(valueCore != null) {
				if(string.Equals(valueCore, BreadCrumbNode.NoneItemText, StringComparison.Ordinal))
					return BreadCrumbNode.NoneImageIndex;
				if(string.Equals(valueCore, BreadCrumbNode.DefaultItemText, StringComparison.Ordinal))
					return BreadCrumbNode.DefaultImageIndex;
				return int.Parse(valueCore);
			}
			return value;
		}
	}
	public class FormatRuleContainValuesCollectionEditor : CollectionEditor {
		public FormatRuleContainValuesCollectionEditor(Type type)
			: base(type) {
		}
		internal class ValueItem {
			object _value;
			[Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
			[TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
			[DefaultValue(null)]
			public object Value { get { return _value; } set { _value = value; } }
			public override string ToString() {
				if(Value == null) return "(null)";
				string res = Value.ToString();
				if(Value is string) return res;
				return string.Format("{0} ({1})", res, Value.GetType().Name);
			}
		}
		protected override object CreateInstance(Type itemType) {
			return new ValueItem();
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			List<ValueItem> values = new List<ValueItem>();
			if(value is ICollection) {
				foreach(var v in (ICollection)value) values.Add(new ValueItem() { Value = v });
			}
			ICollection res = base.EditValue(context, provider, values) as ICollection;
			if(value == null && (res == null || res.Count == 0)) return value;
			ArrayList final = new ArrayList();
			foreach(ValueItem vi in res) final.Add(vi.Value);
			return final;
		}
	}
	public class TokenEditDesigner : BaseEditDesigner {
		public TokenEditDesigner() {
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			Edit.InitializeNewComponent();
		}
		public TokenEdit Edit { get { return Control as TokenEdit; } }
	}
	public class TokenEditTokenCollectionEditor : CollectionEditor {
		public TokenEditTokenCollectionEditor(Type type)
			: base(type) {
		}
		protected override object CreateInstance(Type itemType) {
			RepositoryItemTokenEdit properties = GetProperties();
			if(itemType == typeof(TokenEditToken)) {
				TokenEditToken item = CreateNewToken();
				properties.Tokens.Add(item);
				return item;
			}
			return base.CreateInstance(itemType);
		}
		protected TokenEditToken CreateNewToken() {
			object value = null;
			for(int i = 1; value == null || IsTokenExists(value); i++) {
				value = CreateTokenValue(i);
			}
			return new TokenEditToken(value);
		}
		protected object CreateTokenValue(int i) {
			return string.Format("Value{0}", i.ToString());
		}
		protected virtual RepositoryItemTokenEdit GetProperties() {
			RepositoryItemTokenEdit properties = Context.Instance as RepositoryItemTokenEdit;
			if(properties == null) {
				properties = ((TokenEdit)Context.Instance).Properties;
			}
			return properties;
		}
		protected virtual bool IsTokenExists(object value) {
			RepositoryItemTokenEdit properties = GetProperties();
			foreach(TokenEditToken tok in properties.Tokens) {
				if(tok.Value != null && tok.Value.Equals(value)) return true;
			}
			return false;
		}
	}
	public class RadioGroupDesigner : BaseEditDesigner {
		public RadioGroup RadioGroup { get { return base.Editor as RadioGroup; } }
		public override IList SnapLines {
			get {
				ArrayList res = new ArrayList();
				res.AddRange(base.SnapLines);
				if(RadioGroup != null) {
					Rectangle tb = RadioGroup.GetFirstItemTextBounds();
					if(!tb.IsEmpty) {
						res.Add(new SnapLine(SnapLineType.Baseline, tb.Bottom, SnapLinePriority.Medium));
						UpdateSnapLine(res, SnapLineType.Baseline, tb.Bottom);
					}
				}
				return res;
			}
		}   
	}
	public class PopupContainerEditDesigner : ButtonEditDesigner { }
	public class BlobEditDesigner : ButtonEditDesigner {
	}
	public class MemoExEditDesigner : BlobEditDesigner {		
	}
	public class ImageEditDesigner : BlobEditDesigner { }
	public class PictureEditDesigner : BaseEditDesigner {
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			SetCameraMenuVisible();
		}
		void SetCameraMenuVisible() {
			var picEdit = Editor as PictureEdit;
			if(picEdit != null) {
				picEdit.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
			}
		}
	}
	public class MemoEditDesigner : BaseEditDesigner { }
	public class LookUpEditBaseDataBindingActionList : ListControlActionList {
		ButtonEditDesigner designer;
		public LookUpEditBaseDataBindingActionList(ButtonEditDesigner designer)
			: base(designer.Component) {
			this.designer = designer;
		}
		protected override void FireChanged() { EditorContextHelper.FireChanged(Designer, ListControl); }
		protected override void CreateDataSource() {
			IServiceProvider serviceProvider = GetService(typeof(IDesignerHost)) as IServiceProvider;
			DevExpress.Design.DataAccess.UI.DataSourceWizard.Run(serviceProvider, Properties);
		}
		protected override object DataSourceCore { get { return Properties.DataSource; } set { Properties.DataSource = value; } }
		protected override string DisplayMemberCore { get { return Properties.DisplayMember; } set { Properties.DisplayMember = value; } }
		protected override string ValueMemberCore { get { return Properties.ValueMember; } set { Properties.ValueMember = value; } }
		protected override Control ListControl { get { return Designer.Editor; } }
		protected RepositoryItemLookUpEditBase Properties { get { return (Designer.Editor.Properties)as RepositoryItemLookUpEditBase; } }
		protected ButtonEditDesigner Designer { get { return designer; } }
		protected override void OnRefreshContent() {
			if(!designer.IsUseComponentSmartTags) base.OnRefreshContent();
			else BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(this.Component);
		}
	}
	public class LookUpEditDesigner : ButtonEditDesigner {	
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			list.Add(new LookUpEditBaseDataBindingActionList(this));			
			base.RegisterActionLists(list);
		}
		public RepositoryItemLookUpEdit Properties { get { return (Editor == null ? null : Editor.Properties); } }
		public new LookUpEdit Editor { get { return base.Editor as LookUpEdit; } }
	}
	public class EditFieldEditor : ObjectSelectorEditor {
		public EditFieldEditor() : base(true) {
		}
		protected virtual RepositoryItemCollection[] GetRepository(ITypeDescriptorContext context) {
			return null;
		}
		protected virtual RepositoryItemCollection GetMainRepository(ITypeDescriptorContext context) {
			RepositoryItemCollection[] rps = GetRepository(context);
			if(rps == null || rps.Length == 0) return null;
			return rps[0];
		}
		protected static ImageList selectorImageList = null;
		protected internal static void FillImageList(EditorClassInfoCollection items) {
			if(selectorImageList != null) return;
			selectorImageList = new ImageList();
			selectorImageList.TransparentColor = System.Drawing.Color.Magenta;
			foreach(EditorClassInfo cItem in items) {
				Image image = cItem.Image;
				if(image == null) image = new Bitmap(16, 16);
				if(selectorImageList.Images.Count == 0) {
					selectorImageList.Images.Add(new System.Drawing.Bitmap(image.Size.Width, image.Size.Height));
					selectorImageList.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Design.Images.Existing.bmp", typeof(EditFieldEditor).Assembly));
					selectorImageList.Images.Add(DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraEditors.Design.Images.New.bmp", typeof(EditFieldEditor).Assembly));
				}
				selectorImageList.Images.Add(image);
			}
		}
		void SetImageIndex(SelectorNode node, int index) {
			node.SelectedImageIndex = node.ImageIndex = index;
		}
		protected virtual int GetImageIndex(EditorClassInfoCollection items, object obj) {
			EditorClassInfo cItem = obj as EditorClassInfo;
			if(cItem == null) {
				RepositoryItem rItem = obj as RepositoryItem;
				if(rItem != null) {
					cItem = items[rItem.EditorTypeName];
				}
			}
			if(cItem != null) return items.IndexOf(cItem) + 3;
			return 0; 
		}
		protected override void FillTreeWithData(ObjectSelectorEditor.Selector selector, ITypeDescriptorContext context, IServiceProvider provider) {
			EditorRegistrationInfo.Default.RegisterUserItems((provider == null ? null : provider.GetService(typeof(IDesignerHost))) as IDesignerHost);
			FillImageList(EditorRegistrationInfo.Default.Editors);
			RepositoryItemCollection[] rps = GetRepository(context);
			selector.Clear();
			selector.ImageList = selectorImageList;
			SelectorNode node = null;
			if(rps != null && rps.Length > 0) {
				node = selector.AddNode(Properties.Resources.ExistingCaption, this, null);
				SetImageIndex(node, 1);
				foreach(RepositoryItemCollection rp in rps) {
					foreach(RepositoryItem item in rp) {
						SetImageIndex(selector.AddNode(item.Name, item, node), GetImageIndex(EditorRegistrationInfo.Default.Editors, item));
					}
				}
			}
			node = selector.AddNode(Properties.Resources.NewCaption, this, null);
			SetImageIndex(node, 2);
			foreach(EditorClassInfo cItem in EditorRegistrationInfo.Default.Editors.Cast<EditorClassInfo>().OrderBy(q=>q.Name)) {
				if(!cItem.DesignTimeVisible || cItem.AllowInplaceEditing != ShowInContainerDesigner.Anywhere) continue;
				SelectorNode n = selector.AddNode(cItem.Name, cItem, node);
				n.SelectedImageIndex = n.ImageIndex = GetImageIndex(EditorRegistrationInfo.Default.Editors, cItem);
			}
			SetImageIndex(selector.AddNode(Properties.Resources.NoneCaption, null, null), -1);
			selector.Height = selector.ItemHeight * 20;
			selector.Width = 300;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object val) {
			if(context == null) return null;
			RepositoryItemCollection main = GetMainRepository(context);
			if(main == null) return null;
			object oldValue = val;
			object newValue = base.EditValue(context, provider, val);
			if(newValue is EditorClassInfo) {
				EditorClassInfo cItem = newValue as EditorClassInfo;
				newValue = cItem.CreateRepositoryItem();
				main.Add(newValue as RepositoryItem);
			}
			if(newValue == this) return oldValue;
			return newValue;
		}
	}
	public class ImageComboBoxItemCollectionEditor : CollectionEditor {
		public ImageComboBoxItemCollectionEditor(Type type) : base(type) { }
		protected override object CreateInstance(Type itemType) {
			ImageComboBoxItem item = Activator.CreateInstance(itemType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null) as ImageComboBoxItem;
			if(item == null || Collection == null) return item;
			ImageComboBoxItem lastItem = GetLastItem();
			if(lastItem == null) {
				item.Value = 0;
				return item;
			}
			item.Value = IncrementValue(lastItem.Value);
			return item;
		}
		ListBox formListBox = null;
		protected ImageComboBoxItem GetLastItem() {
			if(this.formListBox == null) {
				if(Collection == null || Collection.Count == 0) return null;
				return Collection[Collection.Count - 1];
			}
			if(this.formListBox.Items.Count == 0) return null;
			object item = this.formListBox.Items[this.formListBox.Items.Count - 1];
			if(item == null) return null;
			PropertyInfo pi = item.GetType().GetProperty("Value");
			if(pi != null) return pi.GetValue(item, null) as ImageComboBoxItem;
			return null;
		}
		protected override CollectionForm CreateCollectionForm() {
			CollectionForm frm = base.CreateCollectionForm();
			FieldInfo fi = frm.GetType().GetField("listbox", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic);
			if(fi != null) this.formListBox = fi.GetValue(frm) as ListBox;
			return frm;
		}
		protected virtual object IncrementValue(object val) {
			if(val == null) return null;
			Type type = val.GetType();
			try {
				if(type.Equals(typeof(string))) return null;
				Decimal newVal = Convert.ToDecimal(val);
				newVal += 1;
				val = Convert.ChangeType(newVal, type);
			}
			catch {
				return null;
			}
			return val;
		}
		protected ImageComboBoxItemCollection Collection {
			get {
				if(Context == null || Context.PropertyDescriptor == null) return null;
				object instance = Context.Instance;
				ImageComboBoxEdit edit = instance as ImageComboBoxEdit;
				if(edit != null) instance = edit.Properties;
				return Context.PropertyDescriptor.GetValue(instance) as ImageComboBoxItemCollection;
			}
		}
	}
	public class RadioGroupSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			CodeDomSerializer serializer = manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer)) as CodeDomSerializer;
			return serializer != null ? serializer.Deserialize(manager, codeObject) : null;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer serializer = manager.GetSerializer(typeof(Component), typeof(CodeDomSerializer)) as CodeDomSerializer;
			if(serializer == null) return null;
			object statements = serializer.Serialize(manager, value) as CodeStatementCollection;
			RadioGroupLocalizable group = (RadioGroupLocalizable)value;
			RadioGroupItemCollection items = group.Properties.Items;
			if(NeedLocalize(manager) && items.Count > 0) {
				for(int i = 0; i < items.Count; i++)
					SerializeResource(manager, group.GetItemName(i), items[i].Description);
			}
			return statements;
		}
		bool NeedLocalize(IDesignerSerializationManager manager) {
			IDesignerHost host = (IDesignerHost)manager.GetService(typeof(IDesignerHost));
			if(host != null) {
				PropertyDescriptor property = TypeDescriptor.GetProperties(host.RootComponent)["Localizable"];
				if(property != null) return (bool)property.GetValue(host.RootComponent);
			}
			return false;
		}
	}
#if DXWhidbey
	public class RepositoryItemCodeDomSerializer : DXComponentCodeDomSerializer {
	}
#endif
	public class LabelControlDesigner : BaseControlDesigner {
		protected override bool CanUseComponentSmartTags { get { return true; } }
		LabelControlDesigner() {
#if DXWhidbey
			AutoResizeHandles = true;
#endif
		}
		public override SelectionRules SelectionRules {
			get {
				SelectionRules rules = base.SelectionRules;
				Object obj = Component;
				PropertyDescriptor desc = TypeDescriptor.GetProperties(obj)["AutoSizeMode"];
				LabelControl label = Control as LabelControl;
				if(label == null) return rules;
				switch(label.RealAutoSizeMode) { 
					case LabelAutoSizeMode.Horizontal:
						rules &= ~SelectionRules.AllSizeable;
						break;
					case LabelAutoSizeMode.Vertical:
						rules &= ~(SelectionRules.BottomSizeable | SelectionRules.TopSizeable);
						break;
				}
				return rules;
			}
		}
#if DXWhidbey
		public override IList SnapLines {
			get {
				LabelControl label = Component as LabelControl;
				ArrayList snapList = base.SnapLines as ArrayList;
				if(label == null)
					return snapList;
				snapList.Add(new SnapLine(SnapLineType.Baseline, label.GetTextBaselineY(), SnapLinePriority.Medium));
				return snapList as ArrayList;
			}
		}
#endif
	}
	public class TrackBarActionList : DesignerActionList {
		public TrackBarDesigner designer;
		public TrackBarActionList(IComponent component, TrackBarDesigner trackBarDesigner)
			: base(component) {
			designer = trackBarDesigner;
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			res.Add(new DesignerActionMethodItem(this, "PopulateLabels", "Populate Labels", true));
			return res;
		}
		public void PopulateLabels() {
			using(TrackBarAutoPopulateLabelsForm frm = new TrackBarAutoPopulateLabelsForm(TrackBar)) {
				DialogResult dlgRes = UIService.ShowDialog(frm);
				if(dlgRes != DialogResult.OK)
					return;
				PopulateLabelsCore(frm.Info);
			}
		}
		protected void PopulateLabelsCore(TrackBarAutoPopulatingInfo info) {
			TrackBar.Properties.Labels.Clear();
			for(int i = info.Minimun; i <= info.Maximum; i += info.Step) {
				TrackBarLabel label = new TrackBarLabel() { Value = i, Label = i.ToString() };
				TrackBar.Properties.Labels.Add(label);
			}
		}
		IUIService uiSvc = null;
		protected IUIService UIService {
			get {
				if(uiSvc == null)
					uiSvc = GetService(typeof(IUIService)) as IUIService;
				return uiSvc;
			}
		}
		TrackBarControl TrackBar { get { return designer.Control as TrackBarControl; } }
	}
	public class TrackBarDesigner : BaseEditDesigner {
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			list.Add(new TrackBarActionList(Component, this));
			base.RegisterActionLists(list);
		}
		public override SelectionRules SelectionRules {
			get {
				SelectionRules selRules = base.SelectionRules;
				TrackBarControl trackBar = this.Control as TrackBarControl;
				if((trackBar != null) && trackBar.Properties.AutoSize) {
					if(trackBar.Properties.Orientation == Orientation.Horizontal)
						selRules &= ~(SelectionRules.BottomSizeable | SelectionRules.TopSizeable);
					else
						selRules &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
				}
				return selRules;
			}
		}
	}
	public class ProgressPanelDesigner : BaseControlDesigner {
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class ImageSliderDesigner : BaseControlDesigner {		
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class ImageSliderImagesEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
		int currentImageIndex;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			SliderImageCollection result = (SliderImageCollection)value;
			using(ImageCollection images = CreateImageCollection(result)) {
				ImageCollectionEditor editor = new ImageCollectionEditor(typeof(InnerImagesList));
				IList list = editor.EditValue(context, provider, images.Images.InnerImages) as IList;
				ImageSlider slider = ((ImageSlider)result.Slider);
				this.currentImageIndex = slider.CurrentImageIndex;
				result.Clear();
				foreach(ImageInfo info in list) {				   
					result.Add(info.Image);
				}
				slider.CurrentImageIndex = this.currentImageIndex;			  
			}
			return value;
		}
		protected ImageCollection CreateImageCollection(SliderImageCollection sliderImages) {
			ImageCollection collection = new ImageCollection(false);
			foreach(Image image in sliderImages)
				collection.Images.Add(image);
			return collection;
		}
	}
	public class RangeControlDesigner : BaseControlDesigner {
		IComponentChangeService changeService;
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			AutomaticBindHelper.BindToComponent(this, "Client", typeof(IRangeControlClient));
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (changeService != null) {
				changeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
		}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (changeService != null) {
						changeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		protected virtual void OnComponentRemoved(object sender, ComponentEventArgs e) {
			AutomaticBindHelper.UnbindFromRemovedComponent(this, e.Component, "Client", typeof(IRangeControlClient));
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				DesignerVerbCollection coll = base.DXVerbs;
				DesignerVerb addGroupVerb = new DesignerVerb(Properties.Resources.AddNumericClientCaption, new EventHandler(OnAddNumericClient));
				coll.Add(addGroupVerb);
				return coll;
			}
		}
		public RangeControl RangeControl { get { return (RangeControl)Component; } }
		protected virtual void OnAddNumericClient(object sender, EventArgs e) {
			NumericRangeControlClient client = new NumericRangeControlClient();
			client.RangeControl = RangeControl;
			RangeControl.Client = client;
			if(RangeControl.Container != null)
				RangeControl.Container.Add(client);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class FilterControlDesigner : BaseControlDesignerSimple {
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class NavigatorBaseDesigner : BaseControlDesignerSimple {
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	public class RatingControlDesigner : BaseEditDesigner {
		public override SelectionRules SelectionRules {
			get {
				SelectionRules res = base.SelectionRules;
				if(Editor == null) return res;
				if(((RepositoryItemRatingControl)Editor.Properties).AutoSize) {
					res &= ~(SelectionRules.LeftSizeable | SelectionRules.RightSizeable);
				}
				return res;			   
			}
		}
	}
	public class CameraControlDesigner : BaseControlDesigner {
		DesignerActionListCollection actionLists;
		public override DesignerActionListCollection ActionLists {
			get {
				if(actionLists == null)
					actionLists = CreateActionList();
				return actionLists;
			}
		}
		DesignerActionListCollection CreateActionList() {
			DesignerActionListCollection list = new DesignerActionListCollection();
			list.Add(new CameraControlDesignerActionsList(this));
			DXSmartTagsHelper.CreateDefaultLinks(this, list);
			return list;
		}
		class CameraControlDesignerActionsList : DesignerActionList {
			public CameraControlDesignerActionsList(CameraControlDesigner designer)
				: base(designer.Component) { }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("Dock", "Choose Dock Style"));
				return res;
			}
			public DockStyle Dock {
				get {
					if(Camera == null) return DockStyle.None;
					return Camera.Dock;
				}
				set { EditorContextHelper.SetPropertyValue(Component.Site, Camera, "Dock", value); }
			}
			Camera.CameraControl Camera {
				get { return Component as Camera.CameraControl; }
			}
		}
	}
}	
