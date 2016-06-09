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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.FontProperties;
using DevExpress.Xpf.Reports.UserDesigner.Layout;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtension;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.Xpf.Reports.UserDesigner.ReportExplorer;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public abstract class XRControlModelBase : XRComponentModelBase, IReportExplorerItem {
		XRControlInvalidater xrObjectInvalidater;
		protected XRControlModelBase(XRControlModelFactory.ISource<XRControl> source, ImageSource icon)
			: base(source, icon) {
			this.layout = Report.LayoutProvider.GetLayout(XRObject);
			Layout.PropertyChanged += (s, e) => OnLayoutPropertyChanged(e);
			this.fontProperties = Report.FontPropertiesProvider.GetFontProperties(XRObject);
			FontProperties.PropertyChanged += (s, e) => Report.DiagramItem.Diagram.InvalidateRenderLayer();
			XRControlInvalidater.GetInvalidater(this.XRObject, out xrObjectInvalidater);
			xrObjectInvalidater.DataBindingsInvalidated += (s, e) => OnDataBindingInvalidated();
			AnchorHorizontal = CreateXRPropertyModel(x => ((XRControlDesignSite)x).RuntimeAnchorHorizontal, () => ((XRControlDesignSite)DesignSite).RuntimeAnchorHorizontal, value => ((XRControlDesignSite)DesignSite).RuntimeAnchorHorizontal = value, DesignSite, PropertyDescriptorHelper.GetPropertyDescriptor(XRObject, x => x.AnchorHorizontal));
			AnchorVertical = CreateXRPropertyModel(x => ((XRControlDesignSite)x).RuntimeAnchorVertical, () => ((XRControlDesignSite)DesignSite).RuntimeAnchorVertical, value => ((XRControlDesignSite)DesignSite).RuntimeAnchorVertical = value, DesignSite, PropertyDescriptorHelper.GetPropertyDescriptor(XRObject, x => x.AnchorVertical));
		}
		protected void OnLayoutPropertyChanged(PropertyChangedEventArgs e) {
			RaiseLayoutPropertiesChanged(e);
		}
		protected virtual void RaiseLayoutPropertiesChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Layout.Param3))
				RaisePropertyChanged(() => Width);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param4))
				RaisePropertyChanged(() => Height);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param1))
				RaisePropertyChanged(() => Left);
			else if(e.PropertyName == GetPropertyName(() => Layout.Param2))
				RaisePropertyChanged(() => Top);
		}
		protected override void OnThisPropertyChanged(PropertyChangedEventArgs e) {
			if(e.PropertyName == GetPropertyName(() => Width))
				RaisePropertyChanged(() => Size);
			else if(e.PropertyName == GetPropertyName(() => Height))
				RaisePropertiesChanged(() => Size, () => Bottom);
			else if(e.PropertyName == GetPropertyName(() => Left))
				RaisePropertyChanged(() => Position);
			else if(e.PropertyName == GetPropertyName(() => Top))
				RaisePropertiesChanged(() => Position, () => Bottom, () => TopAbsolute);
			else if(e.PropertyName == GetPropertyName(() => Bottom))
				RaisePropertyChanged(() => BottomAbsolute);
		}
		public virtual double Width {
			get { return Layout.Param3; }
			set { Layout.Param3 = value; }
		}
		public virtual double Height {
			get { return Layout.Param4; }
			set { Layout.Param4 = value; }
		}
		public virtual double Left {
			get { return Layout.Param1; }
			set { Layout.Param1 = value; }
		}
		public virtual double Top {
			get { return Layout.Param2; }
			set { Layout.Param2 = value; }
		}
		public virtual double Bottom {
			get { return Top + Height; }
			set { Height = value - Top; }
		}
		public Size Size {
			get { return new Size(Width, Height); }
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}
		public virtual Point Position {
			get { return new Point(Left, Top); }
			set {
				Left = value.X;
				Top = value.Y;
			}
		}
		public double TopAbsolute { get { return (Parent as XRControlModelBase).Return(x => x.TopAbsolute, () => 0.0) + Top; } }
		public double BottomAbsolute { get { return (Parent as XRControlModelBase).Return(x => x.TopAbsolute, () => 0.0) + Bottom; } }
		void OnDataBindingInvalidated() {
			XRDiagramItemBase.SetShowBindingIcon(DiagramItem, HasDataBindings);
			RaisePropertyChanged(() => HasDataBindings);
		}
		public new XRControl XRObject { get { return (XRControl)base.XRObject; } }
		protected new XRControlDesignSite DesignSite { get { return (XRControlDesignSite)XRObject.Site; } }
		static readonly Size defaultSize = XRDefaults<XRLabel>.Size;
		public static Size DefaultSize { get { return defaultSize; } }
		readonly BaseReportElementLayout layout;
		protected BaseReportElementLayout Layout { get { return layout; } }
		readonly BaseFontProperties fontProperties;
		public BaseFontProperties FontProperties { get { return fontProperties; } }
		public XRPropertyModel<HorizontalAnchorStyles> AnchorHorizontal { get; private set; }
		public XRPropertyModel<VerticalAnchorStyles> AnchorVertical { get; private set; }
		string IReportExplorerItem.TypeString { get { return XRObject.GetDisplayName(); } }
		IXRControlBoundsTracker<XRControlModelBase> parentTopAbsoluteTracker;
		protected override void OnParentChanged() {
			base.OnParentChanged();
			if(parentTopAbsoluteTracker != null)
				parentTopAbsoluteTracker.Dispose();
			parentTopAbsoluteTracker = (Parent as XRControlModelBase).With(x => XRControlModelBoundsTracker.NewTopAbsoluteTracker(x, () => RaisePropertiesChanged(() => BottomAbsolute, () => TopAbsolute)));
			xrObjectInvalidater.InvalidateParent();
			RaisePropertiesChanged(() => TopAbsolute, () => BottomAbsolute);
		}
		public override bool HasDataBindings { get { return XRObject.DataBindings.Any(); } }
		protected override void InitializeXRObjectIfNeeded(object xrObject, ModelFactoryData factoryData) {
			base.InitializeXRObjectIfNeeded(xrObject, factoryData);
			var xrControl = (XRControl)xrObject;
			if(xrControl.Parent == null) {
				var reportModel = (XtraReportModel)factoryData.FactoryOwner;
				if(factoryData.SourceIsNew)
					InitializeNewXRControl(xrControl, reportModel);
			}
		}
		protected virtual void InitializeNewXRControl(XRControl xrControl, XtraReportModel model) { }
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			BindDiagramItemToXRObject(() => XRDiagramItemBase.SetShowBorders(DiagramItem, XRObject.Borders == BorderSide.None && !(XRObject is XRCrossBandLine)), () => XRObject.Borders);
			BindDiagramItem(XRDiagramItemBase.ShowBindingIconProperty, () => HasDataBindings, BindingMode.OneWay);
			BindDiagramItem(DiagramItem.WidthProperty, () => Width);
			BindDiagramItem(DiagramItem.HeightProperty, () => Height);
			BindDiagramItem(DiagramItem.PositionProperty, () => Position);
			BindDiagramItem(DiagramItem.FontFamilyProperty, () => FontProperties.FontFamily, FontProperties);
			BindDiagramItem(DiagramItem.FontSizeProperty, () => FontProperties.FontSize, FontProperties, BindingMode.TwoWay, new FontSizeInPointsToFontSizeInPixelsConverter());
			BindDiagramItem(DiagramItem.FontStyleProperty, () => FontProperties.FontStyle, FontProperties);
			BindDiagramItem(DiagramItem.FontWeightProperty, () => FontProperties.FontWeight, FontProperties);
			BindDiagramItem(DiagramItem.ForegroundProperty, () => FontProperties.Foreground, FontProperties);
			BindAnchorsProperty();
		}
		protected virtual void BindAnchorsProperty() {
			BindDiagramItem(DiagramItem.AnchorsProperty, BindingMode.OneWay, new AnchorsConverter(), DiagramItemBindingSource.Create(() => AnchorHorizontal.Value, AnchorHorizontal), DiagramItemBindingSource.Create(() => AnchorVertical.Value, AnchorVertical));
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return base.GetEditableProperties()
				.InjectProperties(this, x => x.FontProperties)
				.InjectPropertyModel(this, x => x.AnchorHorizontal)
				.InjectPropertyModel(this, x => x.AnchorVertical)
			;
		}
		#region AnchorsConverter
		class AnchorsConverter : IMultiValueConverter {
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
				var horizontalAnchor = (HorizontalAnchorStyles)values[0];
				var verticalAnchor = (VerticalAnchorStyles)values[1];
				var sides = Sides.None;
				switch(horizontalAnchor) {
					case HorizontalAnchorStyles.Left:
						sides |= Sides.Left;
						break;
					case HorizontalAnchorStyles.Right:
						sides |= Sides.Right;
						break;
					case HorizontalAnchorStyles.Both:
						sides |= Sides.Left | Sides.Right;
						break;
				}
				switch(verticalAnchor) {
					case VerticalAnchorStyles.Top:
						sides |= Sides.Top;
						break;
					case VerticalAnchorStyles.Bottom:
						sides |= Sides.Bottom;
						break;
					case VerticalAnchorStyles.Both:
						sides |= Sides.Top | Sides.Bottom;
						break;
				}
				return sides;
			}
			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
				throw new NotSupportedException();
			}
		}
		#endregion
	}
}
