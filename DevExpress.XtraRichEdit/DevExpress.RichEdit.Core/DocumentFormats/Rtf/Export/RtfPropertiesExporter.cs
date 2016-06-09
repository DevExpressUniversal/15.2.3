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

using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfPropertiesExporter
	public abstract class RtfPropertiesExporter {
		#region Fields
		readonly IRtfExportHelper rtfExportHelper;
		readonly RtfBuilder rtfBuilder;
		readonly DocumentModel documentModel;
		#endregion
		protected RtfPropertiesExporter(DocumentModel documentModel, IRtfExportHelper rtfExportHelper, RtfBuilder rtfBuilder) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(rtfExportHelper, "rtfExportHelper");
			Guard.ArgumentNotNull(rtfBuilder, "rtfBuilder");
			this.documentModel = documentModel;
			this.rtfExportHelper = rtfExportHelper;
			this.rtfBuilder = rtfBuilder;
		}
		#region Properties
		protected internal IRtfExportHelper RtfExportHelper { get { return rtfExportHelper; } }
		protected internal RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		protected internal DocumentModel DocumentModel { get { return documentModel; } }
		internal DocumentModelUnitConverter UnitConverter { get { return DocumentModel.UnitConverter; } }
		#endregion
		protected void WriteBoolCommand(string command, bool value) {
			if (value)
				RtfBuilder.WriteCommand(command);
			else
				RtfBuilder.WriteCommand(command, 0);
		}
		#region WriteBorderProperties
		internal void WriteBorderProperties(BorderInfo border) {
			BorderInfo defaultBorder = DocumentModel.Cache.BorderInfoCache.DefaultItem;
			WriteBorderStyle(border.Style);
			if(border.Style == BorderLineStyle.Nil)
				return;
			WriteBorderWidth(border.Width, defaultBorder.Width);
			if (border.Offset != defaultBorder.Offset)
				RtfBuilder.WriteCommand(RtfExportSR.BorderSpace, UnitConverter.ModelUnitsToTwips(border.Offset));
			if (border.Color != defaultBorder.Color && border.Color != DXColor.Empty) {
				int colorIndex = RtfExportHelper.GetColorIndex(border.Color);
				RtfBuilder.WriteCommand(RtfExportSR.BorderColor, colorIndex);
			}
			if (border.Frame != defaultBorder.Frame)
				RtfBuilder.WriteCommand(RtfExportSR.BorderFrame);
			if (border.Shadow != defaultBorder.Shadow)
				RtfBuilder.WriteCommand(RtfExportSR.BorderShadow);
		}
		void WriteBorderWidth(int value, int defaultValue) {
			if (value == defaultValue)
				return;
			int val = UnitConverter.ModelUnitsToTwips(value);
			if (val > 255) {
				val = System.Math.Min((val / 2), 255);
				RtfBuilder.WriteCommand(RtfExportSR.BorderWidth, val);
				RtfBuilder.WriteCommand(RtfExportSR.BorderDoubleWidth);
			}
			else
				RtfBuilder.WriteCommand(RtfExportSR.BorderWidth, val);
		}
		void WriteBorderStyle(BorderLineStyle value) {
			if (value != BorderLineStyle.Single) {
				string borderCommand;
				if (RtfContentExporter.BorderLineStyles.TryGetValue(value, out borderCommand))
					RtfBuilder.WriteCommand(borderCommand);
				else {
					int borderArtIndex = RtfArtBorderConverter.GetBorderArtIndex(value);
					if (borderArtIndex > 0)
						RtfBuilder.WriteCommand(RtfExportSR.BorderArtIndex, borderArtIndex);
				}
			}
			else
				RtfBuilder.WriteCommand(RtfExportSR.BorderSingleWidth);
		}
		#endregion
		#region WriteWidthUnit
		protected void WriteWidthUnit(WidthUnitInfo unit, string typeKeyword, string valueKeyword, bool writeValueAnyway) {
			WidthUnitInfo defaultWidthUnit = DocumentModel.Cache.UnitInfoCache.DefaultItem;
			if (unit.Type != defaultWidthUnit.Type) {
				int val = unit.Value;
				switch (unit.Type) {
					case WidthUnitType.ModelUnits:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.ModelUnits);
						val = UnitConverter.ModelUnitsToTwips(val);
						break;
					case WidthUnitType.FiftiethsOfPercent:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.FiftiethsOfPercent);
						break;
					case WidthUnitType.Auto:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.Auto);
						break;
					case WidthUnitType.Nil:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.Nil);
						break;
					default:
						Exceptions.ThrowInternalException();
						break;
				}
				if (unit.Value != defaultWidthUnit.Value || writeValueAnyway)
					RtfBuilder.WriteCommand(valueKeyword, val);
			}
		}
		protected void WriteWidthUnit(WidthUnitInfo unit, string typeKeyword, string valueKeyword) {
			WriteWidthUnit(unit, typeKeyword, valueKeyword, false);
		}
		#endregion
		#region WriteWidthUnitInTwips
		protected internal void WriteWidthUnitInTwips(WidthUnitInfo unit, string typeKeyword, string valueKeyword) {
			WidthUnitInfo defaultWidthUnit = DocumentModel.Cache.UnitInfoCache.DefaultItem;
			if (unit.Type != defaultWidthUnit.Type) {
				int val = unit.Value;
				switch (unit.Type) {
					case WidthUnitType.ModelUnits:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.ModelUnits);
						val = UnitConverter.ModelUnitsToTwips(unit.Value);
						break;
					case WidthUnitType.Nil:
						RtfBuilder.WriteCommand(typeKeyword, (int)WidthUnitType.Nil);
						break;
					default:
						Exceptions.ThrowInternalException();
						break;
				}
				if (unit.Value != defaultWidthUnit.Value)
					RtfBuilder.WriteCommand(valueKeyword, val);
			}
		}
		#endregion
		protected internal virtual bool ShouldExportCellMargin(WidthUnitInfo marginUnit) {
			return marginUnit.Type != WidthUnitType.Nil || marginUnit.Value != 0;
		}
	}
	#endregion
}
