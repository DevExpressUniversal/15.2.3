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

using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Data.XtraReports.Labels {
	public static class LabelWizardHelper {
		public static string GetPaperKindFormattedString(PaperKindData paperKindData, GraphicsUnit targetUnit) {
			float width = GraphicsUnitConverter.Convert(paperKindData.Width, paperKindData.Unit, targetUnit);
			float height = GraphicsUnitConverter.Convert(paperKindData.Height, paperKindData.Unit, targetUnit);
			return string.Format("{0} x {1}", GetFormattedValueInUnits(width, targetUnit, false), GetFormattedValueInUnits(height, targetUnit, true));
		}
		public static string GetFormattedValueInUnits(float value, GraphicsUnit targetUnit) {
			return GetFormattedValueInUnits(value, targetUnit, true);
		}
		static string GetFormattedValueInUnits(float value, GraphicsUnit targetUnit, bool includeUnitText) {
			string format = targetUnit == GraphicsUnit.Millimeter ? "N1" : "N2";
			string formattedValue = value.ToString(format);
			if(!includeUnitText)
				return formattedValue;
			string graphicsUnitText = GraphicsDpi.UnitToString(targetUnit);
			return string.Format("{0} {1}", formattedValue, graphicsUnitText);
		}
		public static int GetLabelsCount(float labelPitch, float labelWidth, float margin, GraphicsUnit labelUnit, float paperDimension, GraphicsUnit paperUnit) {
			float paperDimentionInLabelUnit = GraphicsUnitConverter.Convert(paperDimension, paperUnit, labelUnit);
			return (int)((paperDimentionInLabelUnit - margin + (labelPitch - labelWidth)) / labelPitch);
		}
		public static float GetOtherMarginValue(float labelPitch, float labelWidth, float margin, GraphicsUnit labelUnit, float paperDimension, GraphicsUnit paperUnit) {
			int labelsCount = GetLabelsCount(labelPitch, labelWidth, margin, labelUnit, paperDimension, paperUnit);
			float paperDimentionInLabelUnit = GraphicsUnitConverter.Convert(paperDimension, paperUnit, labelUnit);
			return paperDimentionInLabelUnit - margin - (labelsCount - 1) * labelPitch - labelWidth;
		}
	}
}
