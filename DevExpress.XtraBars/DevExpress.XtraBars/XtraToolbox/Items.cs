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

using DevExpress.Utils.Design;
using DevExpress.Utils.Editors;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraToolbox {
	[
	DesignTimeVisible(false), ToolboxItem(false),
	SmartTagSupport(typeof(ToolboxElementDesignTimeBoundsProvider), SmartTagSupportAttribute.SmartTagCreationMode.UseComponentDesigner),
	Designer("DevExpress.XtraToolbox.Design.ToolboxElementDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(IDesigner))
	]
	public abstract class ToolboxElementBase : Component {
		public ToolboxElementBase() : this(string.Empty) {
			this.caption = DefaultCaption;
		}
		public ToolboxElementBase(string caption) {
			this.caption = caption;
			this.image = null;
			this.tag = null;
			this.name = string.Empty;
			this.appearance = new ToolboxElementAppearance(this);
		}
		string name;
		[Browsable(false), XtraSerializableProperty]
		public string Name {
			get {
				if(Site != null) return Site.Name;
				return name;
			}
			set {
				if(value == null) return;
				name = value;
			}
		}
		string caption;
		[DXCategory(CategoryName.Appearance), Localizable(true)]
		public virtual string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				RaiseElementChanged();
			}
		}
		Image image;
		[Category(CategoryName.Appearance), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				OnImageChanged();
				RaiseElementChanged();
			}
		}
		protected virtual void OnImageChanged() { }
		void ResetAppearance() {
			Appearance.Reset();
		}
		bool ShouldSerializeAppearance() {
			return Appearance.ShouldSerialize();
		}
		ToolboxElementAppearance appearance;
		[Category(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ToolboxElementAppearance Appearance {
			get { return appearance; }
		}
		internal void SetAppearance(ToolboxElementAppearance appearance) {
			this.appearance = appearance;
		}
		object tag;
		[DefaultValue(null), Editor(typeof(UIObjectEditor), typeof(UITypeEditor)), TypeConverter(typeof(ObjectEditorTypeConverter))]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		IToolboxControl owner;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IToolboxControl Owner {
			get { return owner; }
		}
		protected internal void SetOwner(IToolboxControl owner) {
			this.owner = owner;
		}
		protected internal void ResetOwner() {
			SetOwner(null);
		}
		protected virtual void RaiseElementChanged() {
			if(Owner == null) return;
			Owner.Refresh();
		}
		protected internal abstract string DefaultCaption { get; }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				image = null;
				tag = null;
			}
			base.Dispose(disposing);
		}
	}
	public class ToolboxItem : ToolboxElementBase, IToolboxItem {
		static readonly object itemChanged = new object();
		public ToolboxItem() : this(string.Empty) {
			Caption = DefaultCaption;
		}
		public ToolboxItem(string caption) : base(caption) {
			this.beginGroup = false;
			this.visible = true;
		}
		string beginGroupCaption;
		[DXCategory(CategoryName.Appearance)]
		public virtual string BeginGroupCaption {
			get { return beginGroupCaption; }
			set {
				if(BeginGroupCaption == value)
					return;
				beginGroupCaption = value;
				RaiseElementChanged();
			}
		}
		bool beginGroup;
		[DXCategory(CategoryName.Appearance)]
		public virtual bool BeginGroup {
			get { return beginGroup; }
			set {
				if(BeginGroup == value)
					return;
				beginGroup = value;
				RaiseElementChanged();
			}
		}
		bool visible;
		[DefaultValue(true)]
		public bool Visible {
			get { return visible; }
			set {
				if(Visible == value)
					return;
				visible = value;
				RaiseElementChanged();
			}
		}
		[DXCategory(CategoryName.Events)]
		public event ToolboxItemChangedEventHandler ItemChanged {
			add { Events.AddHandler(itemChanged, value); }
			remove { Events.RemoveHandler(itemChanged, value); }
		}
		protected virtual void OnItemChanged(ToolboxItemChangedEventArgs e) {
			ToolboxItemChangedEventHandler handler = (ToolboxItemChangedEventHandler)Events[itemChanged];
			if(handler != null) handler(this, e);
		}
		protected override void OnImageChanged() {
			ToolboxControl toolbox = Owner as ToolboxControl;
			if(toolbox == null) return;
			toolbox.ViewInfo.ImageCache.RemoveItemImage(this);
		}
		protected override void RaiseElementChanged() {
			base.RaiseElementChanged();
			OnItemChanged(new ToolboxItemChangedEventArgs());
		}
		protected internal override string DefaultCaption { get { return "ToolboxItem"; } }
		ToolboxGroup ownerGroup;
		public ToolboxGroup OwnerGroup {
			get { return ownerGroup; }
		}
		protected internal void SetOwnerGroup(ToolboxGroup group) {
			this.ownerGroup = group;
		}
		protected internal void ResetOwnerGroup() {
			this.ownerGroup = null;
		}
	}
	public class ToolboxGroup : ToolboxElementBase, IToolboxGroup {
		static readonly object groupChanged = new object();
		ToolboxItemCollection items;
		public ToolboxGroup() : this(string.Empty) {
			Caption = DefaultCaption;
		}
		public ToolboxGroup(string caption) : base(caption) {
			this.items = CreateItemCollection();
			this.items.ListChanged += OnItemCollectionChanged;
		}
		void OnItemCollectionChanged(object sender, ListChangedEventArgs e) {
			ToolboxControl toolbox = Owner as ToolboxControl;
			if(toolbox != null && toolbox.SelectedGroup == this) {
				Owner.LayoutChanged();
			}
		}
		protected virtual ToolboxItemCollection CreateItemCollection() {
			return new ToolboxItemCollection(this);
		}
		[DXCategory(CategoryName.Data), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("DevExpress.Utils.Design.DXCollectionEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), InheritableCollection]
		public ToolboxItemCollection Items {
			get { return items; }
		}
		public event ToolboxGroupChangedEventHandler GroupChanged {
			add { Events.AddHandler(groupChanged, value); }
			remove { Events.RemoveHandler(groupChanged, value); }
		}
		protected virtual void OnGroupChanged(ToolboxGroupChangedEventArgs e) {
			ToolboxGroupChangedEventHandler handler = (ToolboxGroupChangedEventHandler)Events[groupChanged];
			if(handler != null) handler(this, e);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.items != null) {
					this.items.ListChanged -= OnItemCollectionChanged;
					while(items.Count > 0) {
						items[0].Dispose();
						items.RemoveAt(0);
					}
					this.items = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void RaiseElementChanged() {
			base.RaiseElementChanged();
			OnGroupChanged(new ToolboxGroupChangedEventArgs());
		}
		protected internal override string DefaultCaption {
			get { return "ToolboxGroup"; }
		}
		protected internal virtual void SetItemsOwner(IToolboxControl owner) {
			foreach(ToolboxItem item in Items) {
				item.SetOwner(owner);
			}
		}
		protected internal virtual void ResetItemsOwner() {
			foreach(ToolboxItem item in Items) {
				item.ResetOwner();
			}
		}
	}
	public abstract class ToolboxButtonBase : ToolboxElementBase {
		public ToolboxButtonBase(ToolboxControl toolbox) {
			this.toolbox = toolbox;
			this.info = CreateViewInfo();
		}
		ToolboxControl toolbox;
		protected internal ToolboxControl Toolbox {
			get { return toolbox; }
		}
		ToolboxElementInfoBase info;
		protected internal ToolboxElementInfoBase ViewInfo {
			get { return info; }
		}
		protected abstract ToolboxElementInfoBase CreateViewInfo();
	}
	public class ToolboxMenuButton : ToolboxButtonBase {
		public ToolboxMenuButton(ToolboxControl toolbox) : base(toolbox) { }
		protected override ToolboxElementInfoBase CreateViewInfo() {
			return new ToolboxMenuButtonViewInfo(this, Toolbox.ViewInfo);
		}
		public override string Caption {
			get { return Toolbox.OptionsView.MenuButtonCaption; }
			set { Toolbox.OptionsView.MenuButtonCaption = value; }
		}
		public override Image Image {
			get { return Toolbox.OptionsView.MenuButtonImage; }
			set { Toolbox.OptionsView.MenuButtonImage = value; }
		}
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
	}
	public class ToolboxExpandButton : ToolboxButtonBase {
		public ToolboxExpandButton(ToolboxControl toolbox) : base(toolbox) { }
		protected override ToolboxElementInfoBase CreateViewInfo() {
			return new ToolboxExpandButtonViewInfo(this, Toolbox.ViewInfo);
		}
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
	}
	public class ToolboxMoreItemsButton : ToolboxButtonBase {
		public ToolboxMoreItemsButton(ToolboxControl toolbox) : base(toolbox) { }
		protected override ToolboxElementInfoBase CreateViewInfo() {
			return new ToolboxMoreItemsButtonViewInfo(this, Toolbox.ViewInfo);
		}
		public override Image Image {
			get { return Toolbox.OptionsView.MoreItemsButtonImage; }
			set { Toolbox.OptionsView.MoreItemsButtonImage = value; }
		}
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
	}
	public class ToolboxScrollButtonUp : ToolboxButtonBase {
		public ToolboxScrollButtonUp(ToolboxControl toolbox) : base(toolbox) { }
		protected override ToolboxElementInfoBase CreateViewInfo() {
			return new ToolboxScrollButtonUpViewInfo(this, Toolbox.ViewInfo);
		}
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
	}
	public class ToolboxScrollButtonDown : ToolboxButtonBase {
		public ToolboxScrollButtonDown(ToolboxControl toolbox) : base(toolbox) { }
		protected override ToolboxElementInfoBase CreateViewInfo() {
			return new ToolboxScrollButtonDownViewInfo(this, Toolbox.ViewInfo);
		}
		protected internal override string DefaultCaption {
			get { return string.Empty; }
		}
	}
}
