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

using DevExpress.DashboardCommon.Native;
using System.ComponentModel;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon {
	public class DashboardItemColoringOptions {
		const string xmlUseGlobalColors = "UseGlobalColors";
		const string xmlMeasuresColoringMode = "MeasuresColoringMode";
		const ColoringMode DefaultMeasuresColoringMode = ColoringMode.Default;
		const bool DefaultUseGlobalColors = true;
		bool useGlobalColors = DefaultUseGlobalColors;
		ColoringMode measuresColoringMode = DefaultMeasuresColoringMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemColoringOptionsUseGlobalColors"),
#endif
		DefaultValue(DefaultUseGlobalColors)
		]
		public bool UseGlobalColors {
			get {
				return useGlobalColors;
			}
			set {
				if (useGlobalColors != value) {
					useGlobalColors = value;
					OnChanged(ChangeReason.UseGlobalColoring);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemColoringOptionsMeasuresColoringMode"),
#endif
		DefaultValue(DefaultMeasuresColoringMode)
		]
		public ColoringMode MeasuresColoringMode {
			get {
				return measuresColoringMode;
			}
			set {
				if (measuresColoringMode != value) {
					measuresColoringMode = value;
					OnChanged(ChangeReason.Coloring);
				}
			}
		}
		internal IChangeService ChangeService { get; set; }
		internal void SaveToXml(XElement element) {
			if (MeasuresColoringMode != DefaultMeasuresColoringMode)
				element.Add(new XAttribute(xmlMeasuresColoringMode, MeasuresColoringMode));
			if (UseGlobalColors != DefaultUseGlobalColors)
				element.Add(new XAttribute(xmlUseGlobalColors, UseGlobalColors));
		}
		internal void LoadFromXml(XElement element) {
			string measuresColoringModeAttr = XmlHelper.GetAttributeValue(element, xmlMeasuresColoringMode);
			if (!string.IsNullOrEmpty(measuresColoringModeAttr))
				measuresColoringMode = XmlHelper.FromString<ColoringMode>(measuresColoringModeAttr);
			string useGlobalColorsAttr = XmlHelper.GetAttributeValue(element, xmlUseGlobalColors);
			if (!string.IsNullOrEmpty(useGlobalColorsAttr))
				useGlobalColors = XmlHelper.FromString<bool>(useGlobalColorsAttr);
		}
		internal bool ShouldSerialize() {
			return UseGlobalColors != DefaultUseGlobalColors || MeasuresColoringMode != DefaultMeasuresColoringMode;
		}
		void OnChanged(ChangeReason changeReason) {
			if (ChangeService != null)
				ChangeService.OnChanged(new ChangedEventArgs(changeReason, null, null));
		}
	}
}
