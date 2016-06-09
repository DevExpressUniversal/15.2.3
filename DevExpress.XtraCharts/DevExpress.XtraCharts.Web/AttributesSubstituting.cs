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
using System.Web.UI;
using DevExpress.XtraCharts.Web;
namespace DevExpress.XtraCharts.Native {
	public class ChartElementTypeDescriptor : CustomTypeDescriptor {
		public ChartElementTypeDescriptor(ICustomTypeDescriptor typeDescriptor) : base(typeDescriptor) {
		}
		bool HasNestedTagAttribute(System.ComponentModel.AttributeCollection attributes) {
			foreach (Attribute attr in attributes)
				if (attr.GetType().Equals(typeof(NestedTagPropertyAttribute)))
					return true;
			return false;
		}
		public override PropertyDescriptorCollection GetProperties() {
			PropertyDescriptorCollection properties = base.GetProperties();
			PropertyDescriptor[] descriptors = new PropertyDescriptor[properties.Count];
			for (int i = 0; i < properties.Count; i++) {
				PropertyDescriptor prop = properties[i];
				descriptors[i] = HasNestedTagAttribute(prop.Attributes) ?
					TypeDescriptor.CreateProperty(prop.ComponentType, prop, new PersistenceModeAttribute(PersistenceMode.InnerProperty)) : prop;
			}
			return new PropertyDescriptorCollection(descriptors);
		}
	}
	public class ChartElementDescriptionProvider : TypeDescriptionProvider {
		public ChartElementDescriptionProvider(TypeDescriptionProvider descriptionProvider) : base(descriptionProvider) {
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) {
			return new ChartElementTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
		}
	}
	public static class AttributesSubstituter {
		static bool completed;
		public static void Perform() {
			lock (typeof(AttributesSubstituter))
				if (!completed) {
					TypeDescriptor.AddProvider(new ChartElementDescriptionProvider(TypeDescriptor.GetProvider(typeof(WebChartControl))), typeof(WebChartControl));
					TypeDescriptor.AddProvider(new ChartElementDescriptionProvider(TypeDescriptor.GetProvider(typeof(PaletteWrapper))), typeof(PaletteWrapper)); 
					TypeDescriptor.AddProvider(new ChartElementDescriptionProvider(TypeDescriptor.GetProvider(typeof(ChartElement))), typeof(ChartElement)); 
					completed = true;
				}
		}
	}
}
