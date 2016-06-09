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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class Gauge : KpiElement {
		const string xmlMinimum = "Minimum";
		const string xmlMaximum = "Maximum";
		const string deltaMainValueName = "DeltaMainValue";
		const string deltaIndicatorTypeName = "DeltaIndicatorType";
		const string deltaIsGoodName = "DeltaIsGood";
		double? minimum;
		double? maximum;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GaugeMinimum"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null)
		]
		public double? Minimum {
			get { return minimum; }
			set {
				if (minimum != value) {
					minimum = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GaugeMaximum"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null)
		]
		public double? Maximum {
			get { return maximum; }
			set {
				if (maximum != value) {
					maximum = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		public Gauge()
			: this(null) {
		}
		public Gauge(Measure actualValue)
			: this(actualValue, null) {
		}
		public Gauge(Measure actualValue, Measure targetValue)
			: base(actualValue, targetValue) {
		}
		protected override DataItemContainer CreateInstance() {
			return new Gauge();
		}
		protected internal override void Assign(DataItemContainer dataItemContainer) {
			base.Assign(dataItemContainer);
			Gauge gauge = dataItemContainer as Gauge;
			if (gauge != null) {
				minimum = gauge.minimum;
				maximum = gauge.maximum;
			}
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(minimum.HasValue)
				element.Add(new XAttribute(xmlMinimum, minimum));
			if(maximum.HasValue)
				element.Add(new XAttribute(xmlMaximum, maximum));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string minimumAttr = XmlHelper.GetAttributeValue(element, xmlMinimum);
			if(!string.IsNullOrEmpty(minimumAttr))
				minimum = (double?)Double.Parse(minimumAttr, CultureInfo.InvariantCulture);
			string maximumAttr = XmlHelper.GetAttributeValue(element, xmlMaximum);
			if(!string.IsNullOrEmpty(maximumAttr))
				maximum = (double?)Double.Parse(maximumAttr, CultureInfo.InvariantCulture);
		}
	}
}
