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
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Static
		internal static Dictionary<LayoutMode, string> LayoutModeTable = CreateLayoutModeTable();
		internal static Dictionary<LayoutTarget, string> LayoutTargetTable = CreateLayoutTargetTable();
		static Dictionary<LayoutMode, string> CreateLayoutModeTable() {
			Dictionary<LayoutMode, string> result = new Dictionary<LayoutMode, string>();
			result.Add(LayoutMode.Edge, "edge");
			result.Add(LayoutMode.Factor, "factor");
			return result;
		}
		static Dictionary<LayoutTarget, string> CreateLayoutTargetTable() {
			Dictionary<LayoutTarget, string> result = new Dictionary<LayoutTarget, string>();
			result.Add(LayoutTarget.Inner, "inner");
			result.Add(LayoutTarget.Outer, "outer");
			return result;
		}
		#endregion
		#region Chart Layout
		protected internal void GenerateChartLayoutContent(LayoutOptions layout) {
			WriteStartElement("layout", DrawingMLChartNamespace);
			try {
				GenerateChartManualLayoutContent(layout);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartManualLayoutContent(LayoutOptions layout) {
			if (layout.Auto)
				return;
			WriteStartElement("manualLayout", DrawingMLChartNamespace);
			try {
				if (layout.Target != LayoutTarget.Outer)
					GenerateChartSimpleStringAttributeTag("layoutTarget", LayoutTargetTable[layout.Target]);
				if (layout.Left.Mode == LayoutMode.Edge)
					GenerateChartSimpleStringAttributeTag("xMode", LayoutModeTable[layout.Left.Mode]);
				if (layout.Top.Mode == LayoutMode.Edge)
					GenerateChartSimpleStringAttributeTag("yMode", LayoutModeTable[layout.Top.Mode]);
				if (layout.Width.Mode == LayoutMode.Edge)
					GenerateChartSimpleStringAttributeTag("wMode", LayoutModeTable[layout.Width.Mode]);
				if (layout.Height.Mode == LayoutMode.Edge)
					GenerateChartSimpleStringAttributeTag("hMode", LayoutModeTable[layout.Height.Mode]);
				if (layout.Left.Mode != LayoutMode.Auto)
					GenerateChartSimpleDoubleAttributeTag("x", layout.Left.Value);
				if (layout.Top.Mode != LayoutMode.Auto)
					GenerateChartSimpleDoubleAttributeTag("y", layout.Top.Value);
				if (layout.Width.Mode != LayoutMode.Auto)
					GenerateChartSimpleDoubleAttributeTag("w", layout.Width.Value);
				if (layout.Height.Mode != LayoutMode.Auto)
					GenerateChartSimpleDoubleAttributeTag("h", layout.Height.Value);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
		#region Values writers
		void GenerateChartSimpleStringTag(string tag, string value) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				DocumentContentWriter.WriteString(value);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateChartSimpleStringAttributeTag(string tag, string value) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				WriteStringValue("val", value);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartSimpleBoolAttributeTag(string tag, bool value) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				WriteBoolValue("val", value);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateSimpleIntAttributeTag(string tag, int value, string ns) {
			WriteStartElement(tag, ns);
			try {
				WriteIntValue("val", value);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal virtual void GenerateChartSimpleIntAttributeTag(string tag, int value) {
			GenerateSimpleIntAttributeTag(tag, value, DrawingMLChartNamespace);
		}
		protected internal virtual void GenerateChartSimpleDoubleAttributeTag(string tag, double value) {
			WriteStartElement(tag, DrawingMLChartNamespace);
			try {
				WriteStringValue("val", value.ToString(CultureInfo.InvariantCulture));
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
