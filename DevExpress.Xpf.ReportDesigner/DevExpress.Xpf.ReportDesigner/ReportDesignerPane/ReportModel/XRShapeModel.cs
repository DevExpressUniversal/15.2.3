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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraPrinting.Shape;
using DevExpress.XtraPrinting.Shape.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public class XRShapeModel : XRControlModelBase<XRShape, XRShapeDiagramItem> {
		protected internal XRShapeModel(XRControlModelFactory.ISource<XRShape> factory, ImageSource icon)
			: base(factory, icon) {
			var viewModel = new ShapeBaseViewModel(XRObject);
			Shape = CreateXRPropertyModel(() => XRObject.Shape, () => viewModel, shapeBaseViewModel => Tracker.Set(XRObject, x => x.Shape, shapeBaseViewModel.ShapeBase), new ShabeBaseTypeConverter());
		}
		public XRPropertyModel<ShapeBaseViewModel> Shape { get; private set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string ShapeName {
			get { return Shape.Value.Data; }
			set { Shape.Value.Data = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int Fillet {
			get {
				if(XRObject.Shape is FilletShapeBase)
					return ((FilletShapeBase)XRObject.Shape).Fillet;
				else if(XRObject.Shape is ShapeBrace)
					return ((ShapeBrace)XRObject.Shape).Fillet;
				else return 0;
			}
			set {
				if(XRObject.Shape is FilletShapeBase)
					((FilletShapeBase)XRObject.Shape).Fillet = value;
				else if(XRObject.Shape is ShapeBrace)
					((ShapeBrace)XRObject.Shape).Fillet = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ArrowHeight {
			get { return (XRObject.Shape as ShapeArrow).Return(x => x.ArrowHeight, () => 0); }
			set { (XRObject.Shape as ShapeArrow).Do(x => x.ArrowHeight = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int ArrowWidth {
			get { return (XRObject.Shape as ShapeArrow).Return(x => x.ArrowWidth, () => 0); }
			set { (XRObject.Shape as ShapeArrow).Do(x => x.ArrowWidth = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int NumberOfSides {
			get { return (XRObject.Shape as ShapePolygon).Return(x => x.NumberOfSides, () => 0); }
			set { (XRObject.Shape as ShapePolygon).Do(x => x.NumberOfSides = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public float Concavity {
			get { return (XRObject.Shape as ShapeStar).Return(x => x.Concavity, () => 0); }
			set { (XRObject.Shape as ShapeStar).Do(x => x.Concavity = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int StarPointCount {
			get { return (XRObject.Shape as ShapeStar).Return(x => x.StarPointCount, () => 0); }
			set { (XRObject.Shape as ShapeStar).Do(x => x.StarPointCount = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int HorizontalLineWidth {
			get { return (XRObject.Shape as ShapeCross).Return(x => x.HorizontalLineWidth, () => 0); }
			set { (XRObject.Shape as ShapeCross).Do(x => x.HorizontalLineWidth = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int VerticalLineWidth {
			get { return (XRObject.Shape as ShapeCross).Return(x => x.VerticalLineWidth, () => 0); }
			set { (XRObject.Shape as ShapeCross).Do(x => x.VerticalLineWidth = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int TipLength {
			get { return (XRObject.Shape as ShapeBracket).Return(x => x.TipLength, () => 0); }
			set { (XRObject.Shape as ShapeBracket).Do(x => x.TipLength = value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int TailLength {
			get { return (XRObject.Shape as ShapeBrace).Return(x => x.TailLength, () => 0); }
			set { (XRObject.Shape as ShapeBrace).Do(x => x.TailLength = value); }
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			return base.GetEditableProperties()
				.InjectPropertyModel(this, x => x.Shape)
				.InjectProperty(this, x => x.Fillet)
				.InjectProperty(this, x => x.ShapeName)
				.InjectProperty(this, x => x.ArrowHeight)
				.InjectProperty(this, x => x.ArrowWidth)
				.InjectProperty(this, x => x.NumberOfSides)
				.InjectProperty(this, x => x.Concavity)
				.InjectProperty(this, x => x.StarPointCount)
				.InjectProperty(this, x => x.HorizontalLineWidth)
				.InjectProperty(this, x => x.VerticalLineWidth)
				.InjectProperty(this, x => x.TipLength)
				.InjectProperty(this, x => x.TailLength)
			;
		}
	}
	public class ShapeBaseViewModel {
		readonly XRShape shape;
		public XRShape Shape { get { return shape; } }
		public ShapeBaseViewModel(XRShape shape) {
			this.shape = shape;
		}
		string data;
		string shapeName;
		[Display(AutoGenerateField = false)]
		public string Data {
			get { return (data = shape.Shape.ShapeName); }
			set { Tracker.Set(shape, x => x.Shape, ShapeFactory.Create(shape, shapeName = value)); }
		}
		public ShapeBase ShapeBase { get { return shape.Shape; } }
		public override bool Equals(object obj) {
			return this.shapeName == ((ShapeBaseViewModel)obj).shapeName;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class ShabeBaseTypeConverter : ExpandableObjectConverter {
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			var xrShape = ((ShapeBaseViewModel)value).Shape;
			var allProperties = PropertyDescriptorHelper.GetPropertyDescriptors(value, context, null, attributes);
			var shapeBaseProperties = new List<PropertyDescriptor>() { allProperties[ExpressionHelper.GetPropertyName((ShapeBaseViewModel x) => x.Data)] };
			var shapeBaseViewModel = (ShapeBaseViewModel)value;
			if(shapeBaseViewModel.ShapeBase != null)
				shapeBaseProperties.AddRange(ProxyPropertyDescriptor.GetProxyDescriptors(shapeBaseViewModel, x => x.ShapeBase).ToArray());
			return new PropertyDescriptorCollection(shapeBaseProperties.Where(x => {
				var browsableAttribute = (BrowsableAttribute)x.Attributes[typeof(BrowsableAttribute)];
				return browsableAttribute == null || browsableAttribute.Browsable;
			}).ToArray());
		}
	}
}
