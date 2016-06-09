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
namespace DevExpress.DashboardCommon {
	public enum WeightedLegendType { Linear, Nested }
	public class WeightedLegend : MapLegendBase {
		const string xmlName = "WeightedLegend";
		const string xmlLegendType = "WeightedLegendType";
		const WeightedLegendType DefaultType = WeightedLegendType.Linear;
		WeightedLegendType type = DefaultType;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("WeightedLegendType"),
#endif
		DefaultValue(DefaultType)
		]
		public WeightedLegendType Type {
			get { return type; }
			set {
				if(value != type) {
					type = value;
					OnChanged();
				}
			}
		}
		protected override string XmlElementName { get { return xmlName; } }
		internal WeightedLegend(IChangeService changeService)
			: base(changeService) {
		}
		protected override void SaveToXmlInternal(XElement element) {
			if(type != DefaultType)
				element.Add(new XAttribute(xmlLegendType, type));
		}
		protected override void LoadFromXmlInternal(XElement element) {
			string attribute = XmlHelper.GetAttributeValue(element, xmlLegendType);
			if(!String.IsNullOrEmpty(attribute))
				type = XmlHelper.EnumFromString<WeightedLegendType>(attribute);
		}
		internal void CopyFrom(WeightedLegend source) {
			if (source == null)
				return;
			base.CopyFrom(source);
			Type = source.Type;
		}
	}
}
