#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class CustomShapefileData {
		const string xmlShapeData = "ShapeData";
		const string xmlAttributeData = "AttributeData";
		byte[] shapeData;
		byte[] attributeData;
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public byte[] ShapeData {
			get { return shapeData; }
			set {
				if(shapeData != value) {
					shapeData = value;
					OnChanged();
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public byte[] AttributeData {
			get { return attributeData; }
			set {
				if(attributeData != value) {
					attributeData = value;
					OnChanged();
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string ShapeDataSerializable {
			get { return ShapeData != null ? Convert.ToBase64String(ShapeData) : null; }
			set { ShapeData = value != null ? Convert.FromBase64String(value) : null; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		public string AttributeDataSerializable {
			get { return AttributeData != null ? Convert.ToBase64String(AttributeData) : null; }
			set { AttributeData = value != null ? Convert.FromBase64String(value) : null; }
		}
		internal MapDashboardItem DashboardItem { get; set; }
		public CustomShapefileData() {
		}
		public CustomShapefileData(byte[] shapeData, byte[] attributeData) {
			this.shapeData = shapeData;
			this.attributeData = attributeData;
		}
		internal void SaveToXml(XElement element) {
			if(ShapeData != null)
				element.Add(new XAttribute(xmlShapeData, ShapeDataSerializable));
			if(AttributeData != null)
				element.Add(new XAttribute(xmlAttributeData, AttributeDataSerializable));
		}
		internal void LoadFromXml(XElement element) {
			string shapeDataAttr = XmlHelper.GetAttributeValue(element, xmlShapeData);
			if(!string.IsNullOrEmpty(shapeDataAttr))
				ShapeDataSerializable = shapeDataAttr;
			string attributeDataAttr = XmlHelper.GetAttributeValue(element, xmlAttributeData);
			if(!string.IsNullOrEmpty(attributeDataAttr))
				AttributeDataSerializable = attributeDataAttr;
		}
		void OnChanged() {
			if(DashboardItem != null && !DashboardItem.Loading) {
				DashboardItem.ResetMapItems();
				DashboardItem.OnChanged(ChangeReason.MapFile);
			}
		}
		internal CustomShapefileData Clone() {
			CustomShapefileData clone = new CustomShapefileData(shapeData, attributeData);
			return clone;
		}
	}
}
