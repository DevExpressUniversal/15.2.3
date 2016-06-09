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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ChartImage : ChartElement, ICustomTypeDescriptor {
		class ChartImagePropertyDescriptorCollection : PropertyDescriptorCollection {
			public ChartImagePropertyDescriptorCollection(ChartImage image, ICollection descriptors) : base(new PropertyDescriptor[0]) {
				foreach (PropertyDescriptor descriptor in descriptors) {
					PropertyDescriptor updatedDescriptor;
					switch (descriptor.DisplayName) {
						case "Image":
							updatedDescriptor = new CustomPropertyDescriptor(descriptor, image.ImageVisible);
							break;
						case "ImageUrl":
							updatedDescriptor = new CustomPropertyDescriptor(descriptor, image.ImageUrlVisible);
							break;
						default:
							updatedDescriptor = descriptor;
							break;
					}
					Add(updatedDescriptor);
				}
			}
		}
		Image image;
		string imageUrl = String.Empty;
		bool shouldDisposeImage;
		protected bool ImageVisible { get { return ChartContainer != null && ChartContainer.ControlType != ChartContainerType.WebControl; } }
		protected bool ImageUrlVisible { get { return ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartImageImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartImage.Image"),
		Localizable(true),
		XtraSerializableProperty
		]
		public Image Image {
			get {
				if (image != null)
					return image;
				if (ChartContainer == null)
					return null;
				image = ContainerAdapter.LoadBitmap(imageUrl);
				shouldDisposeImage = true;
				return image;
			}
			set {
				if (!Object.ReferenceEquals(image, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					image = value;
					imageUrl = String.Empty;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.ClearTextureCache));
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartImageImageUrl"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartImage.ImageUrl"),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraCharts.Design.ImageUrlEditor," + AssemblyInfo.SRAssemblyChartsExtensions, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string ImageUrl {
			get { return imageUrl; }
			set {
				if (!Loading && ImageVisible)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectUseImageUrlProperty));
				if (value != imageUrl) {
					SendNotification(new ElementWillChangeNotification(this));
					if (shouldDisposeImage) {
						DisposeImage();
						shouldDisposeImage = false;
					}
					else
						image = null;
					imageUrl = value;
					RaiseControlChanged();
				}
			}
		}
		internal ChartImage(ChartElement owner) 
			: base(owner) { }
		#region ICustomTypeDescriptor implementation
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return new ChartImagePropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new ChartImagePropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion 
		#region ShouldSerialize & Reset
		bool ShouldSerializeImage() {
			return image != null && !ImageUrlVisible;
		}
		void ResetImage() {
			Image = null;
		}
		bool ShouldSerializeImageUrl() {
			return !String.IsNullOrEmpty(imageUrl) && !ImageVisible;
		}
		void ResetImageUrl() {
			ImageUrl = "";
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeImageUrl() || ShouldSerializeImage();
		}
		#endregion 
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Image":
					return ShouldSerializeImage();
				case "ImageUrl":
					return ShouldSerializeImageUrl();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void DisposeImage() {
			if (image != null) {
				image.Dispose();
				image = null;
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed)
				DisposeImage();
			base.Dispose(disposing);
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartImage(null);
		}
		public void Clear() {
			this.Image = null;
			this.imageUrl = String.Empty;
		} 
		public override string ToString() {
			return "(Image)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ChartImage chartImage = obj as ChartImage;
			if (chartImage != null) {
				image = chartImage.image == null ? null : (Image)chartImage.image.Clone();
				imageUrl = chartImage.imageUrl;
			}
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class BackgroundImage : ChartImage {
		const bool DefaultStretch = false;
		bool stretch = DefaultStretch;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("BackgroundImageStretch"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.BackgroundImage.Stretch"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool Stretch {
			get { return stretch; }
			set {
				if (value != stretch) {
					SendNotification(new ElementWillChangeNotification(this));
					stretch = value;
					RaiseControlChanged();
				}
			}
		}
		internal BackgroundImage(ChartElement owner) : base(owner) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeStretch() {
			return stretch != DefaultStretch;
		}
		void ResetStretch() {
			Stretch = DefaultStretch;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeStretch();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "Stretch" ? ShouldSerializeStretch() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect, bool reverseY) {
			Image image = Image;
			if (image == null) 
				return null;
			return stretch ? (GraphicsCommand)new StretchedImageRectangleGraphicsCommand(rect, image, false, reverseY) : 
							 (GraphicsCommand)new TiledImageRectangleGraphicsCommand(rect, image, false, reverseY);
		}
		internal GraphicsCommand CreateGraphicsCommand(ZPlaneRectangle rect) {
			return CreateGraphicsCommand(rect, false);
		}
		internal bool Render(IRenderer renderer, Rectangle bounds) {
			Image image = Image;
			if (image == null)
				return false;
			renderer.DrawImage(image, bounds, Stretch ? ChartImageSizeMode.Stretch : ChartImageSizeMode.Tile);
			return true;
		}
		internal void Render(IRenderer renderer, IPolygon polygon) {
			using (GraphicsPath path = polygon.GetPath()) {
				using (Region region = new Region(path)) {
					renderer.SetClipping(region);
					Render(renderer, MathUtils.StrongRound(polygon.Rect));
					renderer.RestoreClipping();
				}
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new BackgroundImage(null);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			BackgroundImage backImage = obj as BackgroundImage;
			if (backImage != null)
				stretch = backImage.stretch;
		}
	}
}
