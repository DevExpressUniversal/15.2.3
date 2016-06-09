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

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(ImageAnnotationTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class ImageAnnotation : Annotation {
		const ChartImageSizeMode DefaultSizeMode = ChartImageSizeMode.AutoSize;
		readonly ChartImage image;
		Image defaultImage;
		ChartImageSizeMode sizeMode = DefaultSizeMode;
		internal Image ActualImage {
			get {
				if (Image.Image != null)
					return Image.Image;
				if (defaultImage == null)
					defaultImage = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraCharts.Images.ImageAnnotationDefaultPicture.png", Assembly.GetExecutingAssembly());
				return defaultImage;
			}
		}
		protected internal override bool ActualAutoSize { 
			get { return sizeMode == ChartImageSizeMode.AutoSize; } 
			set {
				if (value == true)
					sizeMode = ChartImageSizeMode.AutoSize;
				else if (sizeMode == ChartImageSizeMode.AutoSize)
					sizeMode = ChartImageSizeMode.Stretch;
				} 
		}
		protected internal override string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.ImageAnnotationPrefix); } }
		protected override AnnotationAppearance Appearance {
			get {
				IChartAppearance rootAppearance = CommonUtils.GetActualAppearance(this);
				return rootAppearance != null ? rootAppearance.ImageAnnotationAppearance : null;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ImageAnnotationSizeMode"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ImageAnnotation.SizeMode"),
		Category(Categories.Behavior),
		TypeConverter(typeof(EnumTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public ChartImageSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if (value != sizeMode) {
					SendNotification(new ElementWillChangeNotification(this));
					sizeMode = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ImageAnnotationImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ImageAnnotation.Image"),
		Category(Categories.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		NestedTagProperty
		]
		public ChartImage Image { get { return image; } }
		public ImageAnnotation() : this(string.Empty) {
		}
		public ImageAnnotation(string name) : base(name) {
			this.image = new ChartImage(this);
		}
		public ImageAnnotation(string name, string imageUrl) : this(name) {
			this.image.ImageUrl = imageUrl;
		}
		public ImageAnnotation(string name, Image image) : this(name) {
			this.image.Image = image;			
		}		
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "SizeMode")
				return ShouldSerializeSizeMode();
			if (propertyName == "Image")
				return ShouldSerializeImage();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeSizeMode() {
			return sizeMode != DefaultSizeMode;
		}
		void ResetSizeMode() {
			SizeMode = DefaultSizeMode;
		}
		bool ShouldSerializeImage() {
			return image.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeSizeMode() || ShouldSerializeImage();
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if (disposing && !IsDisposed && defaultImage != null) {
				defaultImage.Dispose();
				defaultImage = null;
			}
			base.Dispose(disposing);
		}
		protected override Size CalculateInnerSize() {
			return ActualImage == null ? Size.Empty : ActualImage.Size;
		}
		protected override ChartElement CreateObjectForClone() {
			return new ImageAnnotation();
		}
		protected internal override AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, AnnotationLayout anchorPointLayout) {
			UpdateSize();
			SaveLayout(shapeLayout.Position, anchorPointLayout.Position);
			if (!Visible || ChartContainer == null || ChartContainer.Chart == null)
				return null;
			int indexInRepository = ChartContainer.Chart.AnnotationRepository.IndexOf(this);
			return new ImageAnnotationViewData(this, Shape, anchorPointLayout.RefinedPoint, anchorPointLayout.Position, indexInRepository, shapeLayout.Position);
		}
		protected internal override AnnotationViewData CalculateViewData(TextMeasurer textMeasurer, AnnotationLayout shapeLayout, AnnotationLayout anchorPointLayout, Rectangle allowedBoundsForAnnotationPlacing) {
			UpdateSize();
			SaveLayout(shapeLayout.Position, anchorPointLayout.Position);
			if (!Visible || ChartContainer == null || ChartContainer.Chart == null)
				return null;
			int indexInRepository = ChartContainer.Chart.AnnotationRepository.IndexOf(this);
			return new ImageAnnotationViewData(this, Shape, anchorPointLayout.RefinedPoint, anchorPointLayout.Position, indexInRepository, shapeLayout.Position, allowedBoundsForAnnotationPlacing);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ImageAnnotation annotation = obj as ImageAnnotation;
			if (annotation == null)
				return;
			sizeMode = annotation.sizeMode;
			image.Assign(annotation.image);
		}
	}
}
