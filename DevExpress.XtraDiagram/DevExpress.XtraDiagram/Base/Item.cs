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
using DevExpress.Diagram.Core;
using DevExpress.XtraEditors;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
using DevExpress.XtraDiagram.Registration;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram {
	[DesignTimeVisible(false), ToolboxItem(false)]
	public abstract partial class DiagramItem : Component, IDiagramItem, IXtraSupportDeserializeCollectionItem {
		int width;
		int height;
		int minWidth;
		int minHeight;
		int itemId;
		bool tabStop;
		DiagramAppearanceObject appearance;
		ISelectionLayer diagramSelectionLayer;
		DiagramItemController controller;
		static readonly Size DefaultMinSize = new Size(1, 1);
		static DiagramItem() {
			XtraDiagramRegistrator.RegisterDefaults();
		}
		public DiagramItem() : this(0, 0, 0, 0) {
		}
		public DiagramItem(Rectangle bounds)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height) {
		}
		public DiagramItem(int x, int y, int width, int height) {
			this.itemId = Ids.GetId();
			this._Position.X = x;
			this._Position.Y = y;
			this.width = width;
			this.height = height;
			this.minWidth = DefaultMinSize.Width;
			this.minHeight = DefaultMinSize.Height;
			this.tabStop = true;
			this.appearance = CreateAppearance();
			this.diagramSelectionLayer = CreateDiagramSelectionLayer();
			this.controller = CreateItemController();
		}
		protected virtual DiagramAppearanceObject CreateAppearance() {
			DiagramAppearanceObject res = new DiagramAppearanceObject();
			res.Changed += OnAppearanceChanged;
			return res;
		}
		protected virtual void DestroyAppearance(DiagramAppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= OnAppearanceChanged;
			appearance.Dispose();
		}
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged(true);
		}
		protected virtual ISelectionLayer CreateDiagramSelectionLayer() {
			return DefaultSelectionLayer.Instance;
		}
		protected abstract DiagramItemController CreateItemController();
		[Localizable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int X {
			get { return (int)Position.X; }
			set {
				if(X == value)
					return;
				var oldValue = _Position;
				_Position.X = value;
				OnPositionChanged(oldValue);
			}
		}
		[Localizable(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Y {
			get { return (int)Position.Y; }
			set {
				if(Y == value)
					return;
				var oldValue = _Position;
				_Position.Y = value;
				OnPositionChanged(oldValue);
			}
		}
		[Localizable(true), DXCategory(CategoryName.Layout), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int Width {
			get { return width; }
			set {
				value = CoerceWidth(value);
				if(Width == value)
					return;
				width = value;
				OnPropertiesChanged();
			}
		}
		[Localizable(true), DXCategory(CategoryName.Layout), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int Height {
			get { return height; }
			set {
				value = CoerceHeight(value);
				if(Height == value)
					return;
				height = value;
				OnPropertiesChanged();
			}
		}
		[Localizable(true), DXCategory(CategoryName.Layout)]
		public virtual Size Size {
			get { return new Size(Width, Height); }
			set {
				value = CoerceSize(value);
				if(Size == value)
					return;
				Size oldSize = Size;
				this.width = value.Width;
				this.height = value.Height;
				OnSizeChanged(oldSize, Size);
			}
		}
		protected Size CoerceSize(Size size) {
			return new Size(CoerceWidth(size.Width), CoerceHeight(size.Height));
		}
		protected virtual void OnSizeChanged(Size oldSize, Size newSize) {
			Controller.OnSizeChanged(oldSize.ToPlatformSize());
			OnPropertiesChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle Bounds {
			get { return new Rectangle(X, Y, Width, Height); }
			set {
				if(Bounds == value)
					return;
				this._Position.X = value.X;
				this._Position.Y = value.Y;
				this.width = CoerceWidth(value.Width);
				this.height = CoerceHeight(value.Height);
				OnPropertiesChanged();
			}
		}
		internal static readonly int MaximumWidth = 1000;
		internal static readonly int MaximumHeight = 1000;
		protected int CoerceWidth(int width) {
			return Math.Min(MaximumWidth, width);
		}
		protected int CoerceHeight(int height) {
			return Math.Min(MaximumHeight, height);
		}
		[DXCategory(CategoryName.Layout), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MinWidth {
			get { return minWidth; }
			set {
				value = CoerceMinWidth(value);
				if(MinWidth == value)
					return;
				minWidth = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Layout), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int MinHeight {
			get { return minHeight; }
			set {
				value = CoerceMinHeight(value);
				if(MinHeight == value)
					return;
				minHeight = value;
				OnPropertiesChanged();
			}
		}
		[DXCategory(CategoryName.Layout)]
		public virtual Size MinSize {
			get { return new Size(MinWidth, MinHeight); }
			set {
				value = CoerceMinSize(value);
				if(MinSize == value)
					return;
				this.minWidth = value.Width;
				this.minHeight = value.Height;
				OnPropertiesChanged();
			}
		}
		protected virtual bool ShouldSerializeMinSize() { return MinSize != DefaultMinSize; }
		protected virtual void ResetMinSize() { MinSize = DefaultMinSize; }
		protected int CoerceMinWidth(int value) {
			return Math.Max(1, value);
		}
		protected int CoerceMinHeight(int value) {
			return Math.Max(1, value);
		}
		protected Size CoerceMinSize(Size size) {
			return new Size(CoerceMinWidth(size.Width), CoerceMinHeight(size.Height));
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(true)]
		public virtual bool TabStop {
			get { return tabStop; }
			set {
				if(TabStop == value)
					return;
				tabStop = value;
				OnPropertiesChanged();
			}
		}
		protected virtual void OnPositionChanged(PointFloat oldValue) {
			Controller.OnPositionChanged(oldValue.ToPlatformPoint());
		}
		[DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DiagramAppearanceObject Appearance { get { return appearance; } }
		protected virtual void OnSelectionLayerChanged() {
		}
		IList<IDiagramItem> items = null;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual IList<IDiagramItem> Items {
			get {
				if(items == null) {
					items = CreateItemCollection();
				}
				return items;
			}
		}
		protected virtual IList<IDiagramItem> CreateItemCollection() {
			return new DiagramItemReadOnlyCollection();
		}
		internal int ItemId { get { return itemId; } }
		protected virtual void OnPropertiesChanged() {
			OnPropertiesChanged(false);
		}
		protected virtual void OnPropertiesChanged(bool appearanceChanged) {
			GetRootItem().DoIfNotNull(root => root.LayoutChanged(appearanceChanged));
		}
		protected internal DiagramItemController Controller { get { return controller; } }
		protected virtual IXtraDiagramRoot GetRootItem() {
			IDiagramItem current = this.Owner();
			while(current != null) {
				if(current is IXtraDiagramRoot) return (IXtraDiagramRoot)current;
				current = current.Owner();
			}
			return null;
		}
		[Browsable(false)]
		public bool HasChildren { get { return Items.Count > 0; } }
		[Browsable(false)]
		public IDiagramItem Owner { get { return this.Owner(); } }
		protected internal abstract string EditValue { get; set; }
		protected internal abstract bool IsRoutable { get; }
		protected virtual void OnAngleChanged() {
			Controller.OnAngleChanged();
		}
		internal bool IsDisposing { get { return isDisposing; } }
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return DiagramItemController.CreateCollectionItem(propertyName, e);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			DiagramItemController.SetIndexCollectionItem(propertyName, e);
		}
		#endregion
		bool isDisposing = false;
		#region IDisposable
		protected void DisposeChildren() {
			if(Items.Count == 0) return;
			for(int i = Items.Count - 1; i >= 0; i--) {
				DiagramItem item = (DiagramItem)Items[i];
				item.Dispose();
			}
		}
		protected override void Dispose(bool disposing) {
			this.isDisposing = true;
			if(disposing) {
				DisposeChildren();
				IXtraDiagramRoot rootItem = GetRootItem();
				if(rootItem != null) {
					rootItem.RemoveFromContainer(this);
				}
				IDiagramItem ownerItem = Owner;
				if(ownerItem != null) {
					ownerItem.NestedItems.Remove(this);
				}
				DestroyAppearance(Appearance);
			}
			this.appearance = null;
			base.Dispose(disposing);
		}
		#endregion
	}
}
