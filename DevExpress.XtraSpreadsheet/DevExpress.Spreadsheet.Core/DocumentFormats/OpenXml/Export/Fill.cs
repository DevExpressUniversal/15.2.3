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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region GradientFillTypeTable
		internal static Dictionary<ModelGradientFillType, string> GradientFillTypeTable = CreateGradientFillTypeTable();
		static Dictionary<ModelGradientFillType, string> CreateGradientFillTypeTable() {
			Dictionary<ModelGradientFillType, string> result = new Dictionary<ModelGradientFillType, string>();
			result.Add(ModelGradientFillType.Linear, "linear");
			result.Add(ModelGradientFillType.Path, "path");
			return result;
		}
		#endregion
		protected internal virtual void GenerateFillsContent() {
			exportStyleSheet.FillInfoHelper.WriteFills(this);
		}
		protected internal void WritePatternFill(int fillIndex) {
			FillInfo info = Workbook.Cache.FillInfoCache[fillIndex];
			WriteShStartElement("patternFill");
			try {
				WriteStringValue("patternType", patternTypeTable[info.PatternType]);
				WriteColor(Workbook.Cache.ColorModelInfoCache[info.ForeColorIndex], "fgColor");
				WriteColor(Workbook.Cache.ColorModelInfoCache[info.BackColorIndex], "bgColor");
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void WriteGradientFill(int gradientFillIndex, GradientStopInfoCollection stops) {
			GradientFillInfo info = Workbook.Cache.GradientFillInfoCache[gradientFillIndex];
			WriteShStartElement("gradientFill");
			try {
				WriteEnumValue("type", info.Type, GradientFillTypeTable, GradientFillInfo.DefaultType);
				WriteDoubleValue("degree", info.Degree, GradientFillInfo.DefaultDegree);
				WriteFloatValue("left", info.Left, GradientFillInfo.DefaultConvergenceValue);
				WriteFloatValue("right", info.Right, GradientFillInfo.DefaultConvergenceValue);
				WriteFloatValue("top", info.Top, GradientFillInfo.DefaultConvergenceValue);
				WriteFloatValue("bottom", info.Bottom, GradientFillInfo.DefaultConvergenceValue);
				stops.ForEach(WriteGradientStop);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WriteGradientStop(int index) {
			WriteShStartElement("stop");
			try {
				GradientStopInfo info = Workbook.Cache.GradientStopInfoCache[index];
				WriteDoubleValue("position", info.Position);
				WriteColor(info.GetColorModelInfo(Workbook), "color");
			} finally {
				WriteEndElement();
			}
		}
	}
	#region ExportFillInfoHelper
	public class ExportFillInfoHelper {
		#region ExportFillInfo
		struct ExportFillInfo {
			#region Static Members
			public static ExportFillInfo Create(int fillIndex) {
				ExportFillInfo result = new ExportFillInfo();
				result.fillIndex = fillIndex;
				result.isPatternFill = true;
				return result;
			}
			public static ExportFillInfo Create(int fillIndex, GradientStopInfoCollection stops) {
				ExportFillInfo result = new ExportFillInfo();
				result.fillIndex = fillIndex;
				result.isPatternFill = false;
				result.stops = stops;
				return result;
			}
			#endregion
			#region Fields
			int fillIndex;
			bool isPatternFill;
			GradientStopInfoCollection stops;
			#endregion
			#region Properties
			public int FillIndex { get { return fillIndex; } }
			public bool IsPatternFill { get { return isPatternFill; } }
			public GradientStopInfoCollection Stops { get { return stops; } }
			#endregion
			public override int GetHashCode() {
				int result = fillIndex ^ isPatternFill.GetHashCode();
				return stops == null ? result : result ^ stops.GetHashCode();
			}
		}
		#endregion
		readonly Dictionary<ExportFillInfo, int> fillInfoTable = new Dictionary<ExportFillInfo, int>();
		protected internal int FillCount { get { return fillInfoTable.Count; } }
		protected internal void RegisterPatternFillIndex(int index) {
			RegisterFillIndexCore(ExportFillInfo.Create(index));
		}
		void RegisterGradientFillIndex(int index, GradientStopInfoCollection stops) {
			RegisterFillIndexCore(ExportFillInfo.Create(index, stops));
		}
		void RegisterFillIndexCore(ExportFillInfo info) {
			if (!fillInfoTable.ContainsKey(info))
				fillInfoTable.Add(info, fillInfoTable.Count);
		}
		protected internal void RegisterFill(FormatBase fillOwner) {
			if (fillOwner.Fill.FillType == ModelFillType.Pattern)
				RegisterPatternFillIndex(fillOwner.FillIndex);
			else
				RegisterGradientFillIndex(fillOwner.GradientFillInfoIndex, fillOwner.GradientStopInfoCollection);
		}
		public void WriteFills(OpenXmlExporter exporter) {
			exporter.WriteShStartElement("fills");
			try {
				exporter.WriteIntValue("count", fillInfoTable.Count);
				foreach (ExportFillInfo info in fillInfoTable.Keys)
					WriteFill(info, exporter);
			}
			finally {
				exporter.WriteShEndElement();
			}
		}
		void WriteFill(ExportFillInfo info, OpenXmlExporter exporter) {
			exporter.WriteShStartElement("fill");
			try {
				int fillIndex = info.FillIndex;
				if (info.IsPatternFill)
					exporter.WritePatternFill(fillIndex);
				else
					exporter.WriteGradientFill(fillIndex, info.Stops);
			}
			finally {
				exporter.WriteShEndElement();
			}
		}
		protected internal int GetFillId(FormatBase fillOwner) {
			if (fillOwner.Fill.FillType == ModelFillType.Pattern)
				return GetPatternId(fillOwner.FillIndex);
			return GetGradientId(fillOwner.GradientFillInfoIndex, fillOwner.GradientStopInfoCollection);
		}
		protected internal int GetPatternId(int fillIndex) {
			return GetIdCore(ExportFillInfo.Create(fillIndex));
		}
		protected internal int GetGradientId(int fillIndex, GradientStopInfoCollection stops) {
			return GetIdCore(ExportFillInfo.Create(fillIndex, stops));
		}
		int GetIdCore(ExportFillInfo info) {
			Debug.Assert(fillInfoTable.ContainsKey(info));
			return fillInfoTable[info];
		}
	}
	#endregion
 }
