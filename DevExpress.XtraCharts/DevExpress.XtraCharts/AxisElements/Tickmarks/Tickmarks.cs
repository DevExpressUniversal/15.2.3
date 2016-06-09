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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public abstract class Tickmarks : TickmarksBase, ICustomTypeDescriptor {
		#region inner classes
		class TickmarksPropertyDescriptorCollection: PropertyDescriptorCollection {
			public TickmarksPropertyDescriptorCollection(Tickmarks tickmarks, ICollection descriptors) : base(new PropertyDescriptor[] {}) {
				foreach (PropertyDescriptor pd in descriptors)
					if (pd.DisplayName == "MinorCount")
						Add(new CustomPropertyDescriptor(pd, false));
					else
						Add(pd);
			}
		}
		#endregion
		protected new Axis2D Axis { get { return (Axis2D)base.Axis; } }
		internal int MaxLength { get { return Math.Max(MinorVisible ? MinorLength : 0, Length); } }
		protected Tickmarks(Axis2D axis) : base(axis) {
		}
		#region ICustomTypeDescriptor implementation
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
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
			return new TickmarksPropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new TickmarksPropertyDescriptorCollection(this, TypeDescriptor.GetProperties(this, true));
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
		protected internal abstract void ApplySignToHalfs(ref int halfFirst, ref int halfSecond);
		protected void InverseSignOfHalfsIfRotated(ref int halfFirst, ref int halfSecond) {
			if(((XYDiagram2D)Axis.Diagram).ActualRotated) {
				halfFirst = -halfFirst;
				halfSecond = -halfSecond;
			}
		}
		public override string ToString() {
			return "(Tickmarks)";
		}
	}
	public sealed class TickmarksX : Tickmarks {
		internal TickmarksX() : base(null) { }
		internal TickmarksX(AxisXBase axis) : base(axis) { }
		internal TickmarksX(SwiftPlotDiagramAxisXBase axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new TickmarksX();
		}
		protected internal override void ApplySignToHalfs(ref int halfFirst, ref int halfSecond) {
			if(Axis.ActualReverse)
				halfSecond = -halfSecond;				
			else				
				halfFirst = -halfFirst;				
			InverseSignOfHalfsIfRotated(ref halfFirst, ref halfSecond);
		}
	}
	public sealed class TickmarksY : Tickmarks {
		internal TickmarksY() : base(null) { }
		internal TickmarksY(AxisYBase axis) : base(axis) { }
		internal TickmarksY(SwiftPlotDiagramAxisYBase axis) : base(axis) { }
		protected override ChartElement CreateObjectForClone() {
			return new TickmarksY();
		}
		protected internal override void ApplySignToHalfs(ref int halfFirst, ref int halfSecond) {
			if(Axis.ActualReverse)
				halfFirst = -halfFirst;
			else
				halfSecond = -halfSecond;
			InverseSignOfHalfsIfRotated(ref halfFirst, ref halfSecond);
		}
	}
}
