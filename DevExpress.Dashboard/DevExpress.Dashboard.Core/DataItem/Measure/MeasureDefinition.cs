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
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.MeasureDefinitionCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class MeasureDefinition : DataItemDefinition {
		const string xmlSummaryType = "SummaryType";
		internal const SummaryType DefaultSummaryType = SummaryType.Sum;
		public SummaryType SummaryType { get; private set; }
		public MeasureDefinition(string dataMember, SummaryType summaryType)
			: base(dataMember) {
			SummaryType = summaryType;
		}
		public MeasureDefinition(string dataMember)
			: this(dataMember, DefaultSummaryType) {
		}
		internal MeasureDefinition()
			: this(null) {
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			MeasureDefinition definition = obj as MeasureDefinition;
			return definition != null && definition.SummaryType == SummaryType;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ SummaryType.GetHashCode();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if (SummaryType != DefaultSummaryType)
				element.Add(new XAttribute(xmlSummaryType, SummaryType));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string attr = element.GetAttributeValue(xmlSummaryType);
			if (!string.IsNullOrEmpty(attr))
				SummaryType = XmlHelper.EnumFromString<SummaryType>(attr);
		}
	}
}
