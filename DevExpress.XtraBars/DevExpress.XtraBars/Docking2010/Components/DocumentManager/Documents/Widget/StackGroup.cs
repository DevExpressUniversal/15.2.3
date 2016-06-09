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
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IStackGroupProperties : DevExpress.Utils.Base.IBaseProperties {
		int Capacity { get; set; }
		bool AllowDragging { get; set; }
	}
	public interface IStackGroupDefaultProperties : DevExpress.Utils.Base.IBaseDefaultProperties {
		int? Capacity { get; set; }
		DefaultBoolean AllowDragging { get; set; }
		[Browsable(false)]
		int ActualCapacity { get; }
		[Browsable(false)]
		bool CanDrag { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class BaseRelativeLengthElement : BaseComponent {
		bool visibleCore;
		public event EventHandler VisibilityChanged;
		public event EventHandler LengthChanged;
		public BaseRelativeLengthElement(IContainer container)
			: base(container) {
			lengthCore = new Length(1, LengthUnitType.Star);
			lengthCore.PropertyChanged += OnPropertiesChanged;
			visibleCore = true;
		}
		int actualLengthCore;
		Length lengthCore;
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content),
		DXDescription ("DevExpress.XtraBars.Docking2010.Views.Widget.BaseRelativeLengthElement,Length")]
		public Length Length {
			get { return lengthCore; }
			set {
				lengthCore.PropertyChanged -= OnPropertiesChanged;
				SetValue(ref lengthCore, value);
				if(lengthCore != null)
					lengthCore.PropertyChanged += OnPropertiesChanged;
			}
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			LayoutChanged();
			RaiseLengthChanged();
		}
		void RaiseLengthChanged() {
			if(LengthChanged != null) LengthChanged(this, EventArgs.Empty);
		}
		internal virtual bool SetAcutalLength(int value) {
			if(actualLengthCore != value) {
				actualLengthCore = value;
				return true;
			}
			return false;
		}
		[Browsable(false)]
		public int ActualLength {
			get { return actualLengthCore; }
		}
		protected internal bool SetGroupVisibility(bool value) {
			if(visibleCore == value) return false;
			visibleCore = value;
			if(VisibilityChanged != null)
				VisibilityChanged(this, EventArgs.Empty);
			return true;
		}
		[Browsable(false)]
		public bool IsVisible {
			get { return visibleCore; }
		}
	}
	public enum LayoutMode { StackLayout, TableLayout, FlowLayout }
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class StackGroup : BaseRelativeLengthElement, IDocumentGroup {
		IStackGroupDefaultProperties propertiesCore;
		DocumentManager managerCore;
		DocumentCollection itemsCore;
		IStackGroupInfo infoCore;
		WidgetView parentCore;
		string captionCore;
		AppearanceObject appearanceCaptionCore;
		public StackGroup()
			: base(null) {
			InitProperties(null);
		}
		public StackGroup(IContainer container)
			: base(container) {
			InitProperties(null);
		}
		public StackGroup(IStackGroupProperties groupProperties)
			: base(null) {
			InitProperties(groupProperties);
		}
		[Browsable(false)]
		public bool IsHorizontal {
			get { return Parent.Orientation == System.Windows.Forms.Orientation.Horizontal; }
		}
		[Category("Appearance"), XtraSerializableProperty,
#if !SL
	DevExpressXtraBarsLocalizedDescription("StackGroupCaption"),
#endif
 DefaultValue(""), Localizable(true)]
		public string Caption {
			get { return captionCore; }
			set { SetValue(ref captionCore, value, UpdateLayout); }
		}
		[Category("Appearance"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("StackGroupAppearanceCaption"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		bool ShouldSerializeAppearanceCaption() {
			return ((AppearanceCaption != null) && AppearanceCaption.ShouldSerialize());
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		[Category("Data"), 
#if !SL
	DevExpressXtraBarsLocalizedDescription("StackGroupItems"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DocumentCollection Items {
			get { return itemsCore; }
		}
		protected Document GetDocument(int index) {
			return (index >= 0 && index < Items.Count) ? Items[index] : null;
		}
		WidgetViewController Controller {
			get { return Manager.View.Controller as WidgetViewController; }
		}
		[Browsable(false)]
		public DocumentManager Manager {
			get { return managerCore; }
		}
		[Browsable(false)]
		public WidgetView Parent {
			get { return parentCore; }
		}
		protected internal void SetManager(DocumentManager manager) {
			managerCore = manager;
		}
		protected internal void SetParent(WidgetView parent) {
			parentCore = parent;
		}
		protected override void OnLayoutChanged() {
			if(Parent != null)
				Parent.LayoutChanged();
		}
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXtraBarsLocalizedDescription("StackGroupProperties")
#else
	Description("")
#endif
]
		public IStackGroupDefaultProperties Properties {
			get { return propertiesCore; }
		}
		protected override void OnCreate() {
			itemsCore = CreateDocumentCollection();
			Items.CollectionChanged += OnItemsCollectionChanged;
			appearanceCaptionCore = new AppearanceObject();
			AppearanceCaption.Changed += OnAppearanceCaptionChanged;
			base.OnCreate();
		}
		protected override void OnDispose() {
			if(Items != null)
				Items.CollectionChanged -= OnItemsCollectionChanged;
			if(Properties != null)
				Properties.Changed -= OnPropertiesChanged;
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			Ref.Dispose(ref appearanceCaptionCore);
			base.OnDispose();
		}
		void OnItemsCollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<Document> ea) {
			if(ea.ChangedType == CollectionChangedType.ElementAdded) {
				if(Info != null) Info.Register(ea.Element);
				ea.Element.SetParent(this);
			}
			if(ea.ChangedType == CollectionChangedType.ElementDisposed) {
				if(Info != null) Info.Unregister(ea.Element);
				ea.Element.SetParent(null);
			}
			if(ea.ChangedType == CollectionChangedType.ElementRemoved) {
				ea.Element.SetParent(null);
				if(Info != null) {
					Info.Unregister(ea.Element);
				}
			}
			UpdateLayout();
		}
		void UpdateLayout() {
			SetLayoutModifired();
			LayoutChanged();
		}
		void SetLayoutModifired() {
			if(Parent != null)
				Parent.SetLayoutModified();
		}
		protected virtual DocumentCollection CreateDocumentCollection() {
			return new DocumentCollection(this);
		}
		int lockUpdateWithAnimation = 0;
		internal bool AnimationUpdateLocked { get { return lockUpdateWithAnimation > 0; } }
		protected internal void SetInfo(IStackGroupInfo info) {
			if(Info != null) {
				lockUpdateWithAnimation++;
				foreach(Document document in Items)
					Info.Unregister(document);
				lockUpdateWithAnimation--;
			}
			infoCore = info;
			if(Info != null)
				foreach(Document document in Items)
					Info.Register(document);
		}
		void InitProperties(IStackGroupProperties parentProperties) {
			captionCore = string.Empty;
			propertiesCore = CreateDefaultProperties(parentProperties);
			Properties.Changed += OnPropertiesChanged;
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected virtual IStackGroupDefaultProperties CreateDefaultProperties(IStackGroupProperties parentProperties) {
			return new StackGroupDefaultProperties(parentProperties);
		}
		void OnPropertiesChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected internal IStackGroupInfo Info {
			get { return infoCore; }
		}
		[Browsable(false)]
		public bool IsFilledUp {
			get {
				int capacity = Properties.ActualCapacity;
				return capacity > 0 && Items.Count >= capacity;
			}
		}
		protected virtual internal void OnInsert(int index) { }
		protected internal void Add(BaseDocument document, int index) {
			if(Controller != null)
				Controller.Dock(document as Document, this, index);
		}
		protected internal void Remove(BaseDocument document) {
			if(Controller != null)
				Controller.RemoveDocument(document);
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		IBaseElementInfo IDocumentGroup.Info {
			get { return Info; }
		}
		BaseMutableList<Document> IDocumentGroup.Items {
			get { return Items; }
		}
	}
	[ListBindable(false), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter)),
	RefreshProperties(RefreshProperties.All)]
	public class StackGroupCollection : BaseMutableListEx<StackGroup> {
		public bool Insert(int index, StackGroup group) {
			return InsertCore(index, group);
		}
		public override string ToString() {
			if(Count == 0) return "None";
			return string.Format("Count {0}", Count);
		}
		public void AddStackGroup(double unitValue = 1, LengthUnitType unitType = LengthUnitType.Star) {
			Add(new StackGroup() { Length = new Length(unitValue, unitType) });
		}
	}
	public class StackGroupProperties : BaseProperties, IStackGroupProperties {
		public StackGroupProperties() {
			SetDefaultValueCore("Capacity", 0);
			SetDefaultValueCore("AllowDragging", true);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new StackGroupProperties();
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int Capacity {
			get { return GetValueCore<int>("Capacity"); }
			set { SetValueCore("Capacity", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowDragging {
			get { return GetValueCore<bool>("AllowDragging"); }
			set { SetValueCore("AllowDragging", value); }
		}
	}
	public class StackGroupDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IStackGroupDefaultProperties {
		public StackGroupDefaultProperties(IStackGroupProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowDragging", DefaultBoolean.Default);
			SetConverter("AllowDragging", GetDefaultBooleanConverter(true));
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new StackGroupDefaultProperties(ParentProperties as IStackGroupProperties);
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? Capacity {
			get { return GetValueCore<int?>("Capacity"); }
			set { SetValueCore("Capacity", value); }
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public DefaultBoolean AllowDragging {
			get { return GetValueCore<DefaultBoolean>("AllowDragging"); }
			set { SetValueCore("AllowDragging", value); }
		}
		[Browsable(false)]
		public int ActualCapacity {
			get { return GetActualValueFromNullable<int>("Capacity"); }
		}
		[Browsable(false)]
		public bool CanDrag {
			get { return GetActualValue<DefaultBoolean, bool>("AllowDragging"); }
		}
	}
}
