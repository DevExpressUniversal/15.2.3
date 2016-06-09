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

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class Card : KpiElement {
		const string xmlShowSparkline = "ShowStartEndValues";
		const bool DefaultShowSparkline = true;
		readonly SparklineOptions sparklineOptions;
		bool showSparkline = DefaultShowSparkline;
		internal Measure SparklineValue { get { return ActualValue ?? TargetValue; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CardSparklineOptions"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SparklineOptions SparklineOptions { get { return sparklineOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CardShowSparkline"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultShowSparkline)
		]
		public bool ShowSparkline { 
			get { return showSparkline; } 
			set {
				showSparkline = value;
				OnChanged(ChangeReason.ClientData, this, showSparkline);
			} 
		}
		public Card()
			: this(null) {
		}
		public Card(Measure actualValue)
			: this(actualValue, null) {
		}
		public Card(Measure actualValue, Measure targetValue)
			: base(actualValue, targetValue) {
			sparklineOptions = new SparklineOptions(this);
		}
		protected override DataItemContainer CreateInstance() {
			return new Card();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(showSparkline != DefaultShowSparkline)
				element.Add(new XAttribute(xmlShowSparkline, showSparkline));
			sparklineOptions.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string showSparklineString = XmlHelper.GetAttributeValue(element, xmlShowSparkline);
			if(!string.IsNullOrEmpty(showSparklineString))
				showSparkline = XmlHelper.FromString<bool>(showSparklineString);
			sparklineOptions.LoadFromXml(element);
		}
		protected internal override void Assign(DataItemContainer dataItemContainer) {
			base.Assign(dataItemContainer);
			Card card = dataItemContainer as Card;
			if(card != null) {
				showSparkline = card.ShowSparkline;
				sparklineOptions.Assign(card.SparklineOptions);
			}
		}
	}
}
