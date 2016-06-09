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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.LayoutControl.Serialization;
namespace DevExpress.Xpf.LayoutControl {
	public interface ILiveCustomizationAreasProvider {
		void GetLiveCustomizationAreas(IList<Rect> areas, FrameworkElement relativeTo);
	}
	public interface ILayoutModelBase { }
	public interface ILayoutControlBase : IScrollControl, ILayoutModelBase {
		FrameworkElement GetMoveableItem(Point p);
		bool AllowItemMoving { get; }
		LayoutProviderBase LayoutProvider { get; }
		Brush MovingItemPlaceHolderBrush { get; }
	}
#if !SILVERLIGHT
#endif
	public abstract class LayoutControlBase : ScrollControl, ILayoutControlBase, ISerializableItem {
		public const double DefaultItemSpace = 4;
		public const double DefaultPadding = 12;
		#region Dependency Properties
		public static readonly DependencyProperty ItemSpaceProperty =
			DependencyProperty.Register("ItemSpace", typeof(double), typeof(LayoutControlBase),
				new PropertyMetadata(DefaultItemSpace, 
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutControlBase)o;
#if SILVERLIGHT
						if (!o.EnsureDefaultValue(e.Property, control.GetDefaultItemSpace(), true))
#endif
							control.OnItemSpaceChanged((double)e.OldValue, (double)e.NewValue);
					}));
		public static readonly DependencyProperty PaddingProperty =
			DependencyProperty.Register("Padding", typeof(Thickness), typeof(LayoutControlBase),
				new PropertyMetadata(new Thickness(DefaultPadding),
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutControlBase)o;
#if SILVERLIGHT
						if (!o.EnsureDefaultValue(e.Property, control.GetDefaultPadding(), true))
#endif
							control.OnPaddingChanged((Thickness)e.OldValue, (Thickness)e.NewValue);
					}));
		protected static readonly Brush DefaultMovingItemPlaceHolderBrush = new SolidColorBrush(Color.FromArgb(51, 0, 0, 0));
		#endregion Dependency Properties
		static LayoutControlBase() {
			DXSerializer.SerializationProviderProperty.OverrideMetadata(typeof(LayoutControlBase), new UIPropertyMetadata(new LayoutControlSerializationProviderBase()));			
		}
		public LayoutControlBase() {
			LayoutProvider = CreateLayoutProvider();
#if SILVERLIGHT
			this.EnsureDefaultValue(ItemSpaceProperty, GetDefaultItemSpace(), false);
			this.EnsureDefaultValue(PaddingProperty, GetDefaultPadding(), false);
#endif
			SerializableItem.SetTypeName(this, this.GetType().Name);
		}
		public virtual void ReadFromXML(XmlReader xml) {
			if (!xml.IsStartElement(GetType().Name))
				return;
			ReadFromXMLCore(xml);
		}
		public virtual void WriteToXML(XmlWriter xml) {
			xml.WriteStartElement(GetType().Name);
			WriteToXMLCore(xml);
			xml.WriteEndElement();
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlBaseController")]
#endif
		public new LayoutControllerBase Controller { get { return (LayoutControllerBase)base.Controller; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlBaseItemSpace")]
#endif
		[XtraSerializableProperty]
		public double ItemSpace {
			get { return (double)GetValue(ItemSpaceProperty); }
			set { SetValue(ItemSpaceProperty, Math.Max(0, value)); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutControlBasePadding")]
#endif
		[XtraSerializableProperty]
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		#region Children
		protected override Rect GetChildBounds(FrameworkElement child) {
			var result = base.GetChildBounds(child);
			LayoutProvider.UpdateChildBounds(child, ref result);
			return result;
		}
		#endregion Children
		#region Layout
		protected override Size OnMeasure(Size availableSize) {
			foreach(var child in GetLogicalChildren(false))
				if(child.Visibility == Visibility.Collapsed)
					child.Measure(availableSize);
			return LayoutProvider.Measure(GetLogicalChildren(true), availableSize, CreateLayoutProviderParameters());
		}
		protected override Size OnArrange(Rect bounds) {
			var viewPortBounds = bounds;
			if(Controller.IsScrollable())
				RectHelper.Offset(ref bounds, -HorizontalOffset, -VerticalOffset);
			return LayoutProvider.Arrange(GetLogicalChildren(true), bounds, viewPortBounds, CreateLayoutProviderParameters());
		}
		protected override Thickness GetContentPadding() {
			var result = base.GetContentPadding();
			ThicknessHelper.Inc(ref result, Padding);
			return result;
		}
		protected abstract LayoutProviderBase CreateLayoutProvider();
		protected virtual LayoutParametersBase CreateLayoutProviderParameters() {
			return new LayoutParametersBase(ItemSpace);
		}
		protected LayoutProviderBase LayoutProvider { get; private set; }
		#endregion Layout
		#region XML Storage
		protected const string XMLIDName = "ID";
		protected virtual void ReadFromXMLCore(XmlReader xml) {
			ReadCustomizablePropertiesFromXML(xml);
			FrameworkElement lastChild;
			ReadChildrenFromXML(xml, out lastChild);
		}
		protected virtual void WriteToXMLCore(XmlWriter xml) {
			WriteCustomizablePropertiesToXML(xml);
			WriteChildrenToXML(GetLogicalChildren(false), xml);
		}
		protected virtual FrameworkElement ReadChildFromXML(XmlReader xml, IList children, int index) {
			string id = xml[XMLIDName];
			FrameworkElement element = null;
			if (!string.IsNullOrEmpty(id))
				element = FindByXMLID(id);
			return ReadChildFromXMLCore(element, xml, children, index, id);
		}
		protected virtual FrameworkElement ReadChildFromXMLCore(FrameworkElement element, XmlReader xml, IList children, int index, string id) {
			if (xml.Name == "Element" && element != null) {
				AddChildFromXML(children, element, index);
				ReadCustomizablePropertiesFromXML(element, xml);
				return element;
			}
			else
				return null;
		}
		protected virtual void WriteChildToXML(FrameworkElement child, XmlWriter xml) {
			xml.WriteStartElement("Element");
			xml.WriteAttributeString(XMLIDName, GetXMLID(child));
			WriteChildToXMLCore(child, xml);
			xml.WriteEndElement();
		}
		protected virtual void WriteChildToXMLCore(FrameworkElement child, XmlWriter xml) {
			WriteCustomizablePropertiesToXML(child, xml);
		}
		protected virtual void ReadChildrenFromXML(XmlReader xml, out FrameworkElement lastChild) {
			int firstChildIndex;
			MoveLogicalChildrenToEnd(out firstChildIndex);
			ReadChildrenFromXML(Children, xml, firstChildIndex, out lastChild);
		}
		protected void ReadChildrenFromXML(IList children, XmlReader xml, int firstChildIndex, out FrameworkElement lastChild) {
			lastChild = null;
			if (!xml.IsEmptyElement) {
				int index = firstChildIndex;
				while (xml.Read() && xml.NodeType != XmlNodeType.EndElement) {
					FrameworkElement control = ReadChildFromXML(xml, children, index);
					if (control != null) {
						lastChild = control;
						index++;
					}
				}
			}
		}
		protected void WriteChildrenToXML(IList children, XmlWriter xml) {
			foreach (FrameworkElement child in children)
				WriteChildToXML(child, xml);
		}
		protected virtual void ReadCustomizablePropertiesFromXML(XmlReader xml) { }
		protected virtual void ReadCustomizablePropertiesFromXML(FrameworkElement element, XmlReader xml) { }
		protected virtual void WriteCustomizablePropertiesToXML(XmlWriter xml) { }
		protected virtual void WriteCustomizablePropertiesToXML(FrameworkElement element, XmlWriter xml) { }
		protected virtual void AddChildFromXML(IList children, FrameworkElement element, int index) {
			int oldIndex = children.IndexOf(element);
			if (oldIndex == index)
				return;
			element.SetParent(null);
			children.Insert(index, element);
		}
		protected virtual FrameworkElement FindByXMLID(string id) {
			return FindName(id) as FrameworkElement;
		}
		protected virtual string GetXMLID(FrameworkElement element) {
			return element.Name;
		}
		protected void MoveLogicalChildrenToEnd(out int firstChildIndex) {
			firstChildIndex = 0;
			for (int i = 0; i < Children.Count; i++) {
				UIElement child = Children[i];
				if (!IsLogicalChild(child)) {
					if (i != firstChildIndex) {
						Children.RemoveAt(i);
						Children.Insert(firstChildIndex, child);
					}
					firstChildIndex++;
				}
			}
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new LayoutControllerBase(this);
		}
#if SILVERLIGHT
		protected virtual double GetDefaultItemSpace() {
			return DefaultItemSpace;
		}
		protected virtual Thickness GetDefaultPadding() {
			return new Thickness(DefaultPadding);
		}
#endif
		protected virtual void OnItemSpaceChanged(double oldValue, double newValue) {
			Changed();
		}
		protected virtual void OnPaddingChanged(Thickness oldValue, Thickness newValue) {
			Changed();
		}
		#region ILayoutControlBase
		FrameworkElement ILayoutControlBase.GetMoveableItem(Point p) {
			return Controller.GetMoveableItem(p);
		}
		bool ILayoutControlBase.AllowItemMoving { get { return false; } }
		LayoutProviderBase ILayoutControlBase.LayoutProvider { get { return LayoutProvider; } }
		Brush ILayoutControlBase.MovingItemPlaceHolderBrush { get { return null; } }
		#endregion ILayoutControlBase
		#region ISerializableItem Members
		FrameworkElement ISerializableItem.Element {
			get { return this; }
		}
		#endregion
		#region ISerializableCollectionItem Members
		string ISerializableCollectionItem.Name {
			get { return Name; }
			set { Name = value; }
		}
		string ISerializableCollectionItem.TypeName {
			get { return SerializableItem.GetTypeName(this); }
		}
		string ISerializableCollectionItem.ParentName {
			get { return SerializableItem.GetParentName(this); }
			set { SerializableItem.SetParentName(this, value); }
		}
		string ISerializableCollectionItem.ParentCollectionName {
			get { return SerializableItem.GetParentCollectionName(this); }
			set { SerializableItem.SetParentCollectionName(this, value); }
		}
		#endregion
	}
	public class LayoutParametersBase {
		public LayoutParametersBase(double itemSpace) {
			ItemSpace = itemSpace;
		}
		public double ItemSpace { get; set; }
	}
	public abstract class LayoutProviderBase {
		private LayoutParametersBase _Parameters;
		public LayoutProviderBase(ILayoutModelBase model) {
			Model = model;
		}
		public Size Measure(FrameworkElements items, Size maxSize, LayoutParametersBase parameters) {
			Parameters = parameters;
			return OnMeasure(items, maxSize);
		}
		public Size Arrange(FrameworkElements items, Rect bounds, Rect viewPortBounds, LayoutParametersBase parameters) {
			Parameters = parameters;
			return OnArrange(items, bounds, viewPortBounds);
		}
		public virtual void CopyLayoutInfo(FrameworkElement from, FrameworkElement to) {
			to.Width = from.Width;
			to.Height = from.Height;
			to.MinWidth = from.DesiredSize.Width - (from.Margin.Left + from.Margin.Right);
			to.MinHeight = from.DesiredSize.Height - (from.Margin.Top + from.Margin.Bottom);
			to.MaxWidth = from.MaxWidth;
			to.MaxHeight = from.MaxHeight;
			to.Margin = from.Margin;
			to.HorizontalAlignment = from.HorizontalAlignment;
			to.VerticalAlignment = from.VerticalAlignment;
		}
		public virtual void UpdateChildBounds(FrameworkElement child, ref Rect bounds) {
		}
		public virtual void UpdateScrollableAreaBounds(ref Rect bounds) {
		}
		protected virtual Size OnMeasure(FrameworkElements items, Size maxSize) {
			return new Size(0, 0);
		}
		protected virtual Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			return bounds.Size();
		}
		protected virtual void OnParametersChanged() {
		}
		protected ILayoutModelBase Model { get; private set; }
		protected LayoutParametersBase Parameters {
			get { return _Parameters; }
			private set {
				_Parameters = value;
				OnParametersChanged();
			}
		}
	}
	public class LayoutControllerBase : ScrollControlController {
		public LayoutControllerBase(ILayoutControlBase control)
			: base(control) {
		}
		public ILayoutControlBase ILayoutControl { get { return IControl as ILayoutControlBase; } }
		public LayoutProviderBase LayoutProvider { get { return ILayoutControl.LayoutProvider; } }  
		#region Scrolling
		protected override Rect ScrollableAreaBounds {
			get {
				var result = base.ScrollableAreaBounds;
				LayoutProvider.UpdateScrollableAreaBounds(ref result);
				return result;
			}
		}
		#endregion Scrolling
		#region Drag&Drop
		public virtual FrameworkElement GetMoveableItem(Point p) {
			return (FrameworkElement)IPanel.ChildAt(p, true, true, false);
		}
		protected virtual bool CanItemDragAndDrop() {
			return ILayoutControl.AllowItemMoving;
		}
		protected virtual bool CanDragAndDropItem(FrameworkElement item) {
			return true;
		}
		protected virtual DragAndDropController CreateItemDragAndDropControler(Point startDragPoint, FrameworkElement dragControl) {
			return null;
		}
		protected override bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			if (WantsItemDragAndDrop(p, point => GetMoveableItem(point), out controller))
				return true;
			return base.WantsDragAndDrop(p, out controller);
		}
		protected bool WantsItemDragAndDrop(Point p, Func<Point, FrameworkElement> getItem, out DragAndDropController controller) {
			if (CanItemDragAndDrop()) {
				FrameworkElement item = getItem(p);
				if (item != null && CanDragAndDropItem(item)) {
					controller = CreateItemDragAndDropControler(p, item);
					return true;
				}
			}
			controller = null;
			return false;
		}
		#endregion
	}
	public abstract class LayoutItemDragAndDropControllerBase : DragAndDropController {
		public static double DragImageOpacity = 0.7;
		public LayoutItemDragAndDropControllerBase(Controller controller, Point startDragPoint, FrameworkElement dragControl)
			: base(controller, startDragPoint) {
			DragControl = dragControl;
			DragControlParent = (Panel)DragControl.GetParent();
			if (IsDragControlVisible) {
				DragControlOrigin = DragControl.GetPosition(Controller.Control);
				DragControlPlaceHolder = CreateDragControlPlaceHolder();
				InitDragControlPlaceHolder();
			}
			else
				DragControlOrigin = PointHelper.Empty;
			DragControlIndex = DragControlParent != null ? DragControlParent.Children.IndexOf(DragControl) : -1;
			if (!PointHelper.IsEmpty(DragControlOrigin))
				StartDragRelativePoint = new Point(
					(StartDragPoint.X - DragControlOrigin.X) / DragControl.ActualWidth,
					(StartDragPoint.Y - DragControlOrigin.Y) / DragControl.ActualHeight);
		}
		public Point StartDragRelativePoint { get; set; }
		protected virtual FrameworkElement CreateDragControlPlaceHolder() {
			return new Canvas();
		}
		protected virtual void InitDragControlPlaceHolder() {
			if (DragControlPlaceHolder is Panel)
				((Panel)DragControlPlaceHolder).Background = Controller.ILayoutControl.MovingItemPlaceHolderBrush;
			Controller.LayoutProvider.CopyLayoutInfo(DragControl, DragControlPlaceHolder);
		}
		protected override bool AllowAutoScrolling { get { return true; } }
		protected new LayoutControllerBase Controller { get { return (LayoutControllerBase)base.Controller; } }
		protected FrameworkElement DragControl { get; private set; }
		protected int DragControlIndex { get; private set; }
		protected Point DragControlOrigin { get; private set; }
		protected Panel DragControlParent { get; private set; }
		protected FrameworkElement DragControlPlaceHolder { get; private set; }
		protected bool IsDragControlVisible { get { return DragControlParent != null && DragControlParent.GetVisible(); } }
		#region Drag Image
		private object _MaxWidthStoredValue;
		private object _MaxHeightStoredValue;
		private object _OpacityStoredValue;
		protected override FrameworkElement CreateDragImage() {
			return DragControl;
		}
		protected override void InitializeDragImage() {
			_MaxWidthStoredValue = DragImage.StorePropertyValue(FrameworkElement.MaxWidthProperty);
			_MaxHeightStoredValue = DragImage.StorePropertyValue(FrameworkElement.MaxHeightProperty);
			_OpacityStoredValue = DragImage.StorePropertyValue(UIElement.OpacityProperty);
			base.InitializeDragImage();
			if (double.IsNaN(DragImage.Width))
				DragImage.MaxWidth = Math.Min(DragImage.MaxWidth, Controller.IPanel.ContentBounds.Width);
			if (double.IsNaN(DragImage.Height))
				DragImage.MaxHeight = Math.Min(DragImage.MaxHeight, Controller.IPanel.ContentBounds.Height);
			DragImage.Opacity = DragImageOpacity;
			if (IsDragControlVisible)
				DragControlParent.Children.Insert(DragControlIndex, DragControlPlaceHolder);
		}
		protected override void FinalizeDragImage() {
			if (IsDragControlVisible)
				DragControlParent.Children.Remove(DragControlPlaceHolder);
			DragImage.RestorePropertyValue(FrameworkElement.MaxWidthProperty, _MaxWidthStoredValue);
			DragImage.RestorePropertyValue(FrameworkElement.MaxHeightProperty, _MaxHeightStoredValue);
			DragImage.RestorePropertyValue(UIElement.OpacityProperty, _OpacityStoredValue);
			base.FinalizeDragImage();
		}
		protected override Point GetDragImageOffset() {
			Size dragImageSize = DragImageSize;
			return new Point(
				-Math.Floor(StartDragRelativePoint.X * dragImageSize.Width) - DragImage.Margin.Left,
				-Math.Floor(StartDragRelativePoint.Y * dragImageSize.Height) - DragImage.Margin.Top);
		}
		protected Size DragImageSize {
			get {
				Size result = DragImage.GetSize();
				if (result == SizeHelper.Zero) {
					result = DragImage.DesiredSize;
					SizeHelper.Deflate(ref result, DragImage.Margin);
				}
				return result;
			}
		}
		#endregion Drag Image
	}
}
