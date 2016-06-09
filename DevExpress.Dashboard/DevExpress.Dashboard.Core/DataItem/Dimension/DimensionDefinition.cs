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
using System.Collections.Generic;
using DevExpress.DashboardCommon.Localization;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DimensionDefinitionCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class DimensionDefinition : DataItemDefinition {
		const string xmlDateTimeGroupInterval = "DateTimeGroupInterval";
		const string xmlTextGroupInterval = "TextGroupInterval";
		internal const string MeasureNamesDataMember = "EFF5CC0D-0EDC-49B1-8A4A-7731869B439B";
		internal const TextGroupInterval DefaultTextGroupInterval = TextGroupInterval.None;
		internal const DateTimeGroupInterval DefaultDateTimeGroupInterval = DateTimeGroupInterval.Year;
		internal static readonly DimensionDefinition MeasureNamesDefinition = new DimensionDefinition(MeasureNamesDataMember);
		public TextGroupInterval TextGroupInterval { get; private set; }
		public DateTimeGroupInterval DateTimeGroupInterval { get; private set; }
		public DimensionDefinition(string dataMember, DateTimeGroupInterval dateTimeGroupInterval)
			: this(dataMember, dateTimeGroupInterval, DefaultTextGroupInterval) {
		}
		public DimensionDefinition(string dataMember, TextGroupInterval textGroupInterval)
			: this(dataMember, DefaultDateTimeGroupInterval, textGroupInterval) {
		}
		public DimensionDefinition(string dataMember)
			: this(dataMember, DefaultDateTimeGroupInterval, DefaultTextGroupInterval) {
		}
		internal DimensionDefinition(string dataMember, DateTimeGroupInterval dateTimeGroupInterval, TextGroupInterval textGroupInterval)
			: base(dataMember) {
			DateTimeGroupInterval = dateTimeGroupInterval;
			TextGroupInterval = textGroupInterval;
		}
		internal DimensionDefinition()
			: this(null) {
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj))
				return false;
			DimensionDefinition definition = obj as DimensionDefinition;
			return definition != null && definition.TextGroupInterval == TextGroupInterval && definition.DateTimeGroupInterval == DateTimeGroupInterval;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ TextGroupInterval.GetHashCode() ^ DateTimeGroupInterval.GetHashCode();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if (DateTimeGroupInterval != DefaultDateTimeGroupInterval)
				element.Add(new XAttribute(xmlDateTimeGroupInterval, DateTimeGroupInterval));
			if (TextGroupInterval != DefaultTextGroupInterval)
				element.Add(new XAttribute(xmlTextGroupInterval, TextGroupInterval));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string attr = element.GetAttributeValue(xmlDateTimeGroupInterval);
			if (!string.IsNullOrEmpty(attr))
				DateTimeGroupInterval = XmlHelper.EnumFromString<DateTimeGroupInterval>(attr);
			attr = element.GetAttributeValue(xmlTextGroupInterval);
			if (!string.IsNullOrEmpty(attr))
				TextGroupInterval = XmlHelper.EnumFromString<TextGroupInterval>(attr);
		}
	}
}
