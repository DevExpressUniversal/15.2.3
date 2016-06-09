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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	[Flags]
	public enum TargetDimensions { 
		Arguments = 1, 
		Series = 2, 
		Points = Arguments | Series
	};
	public class ChartInteractivityOptions : DashboardItemInteractivityOptions {
		const string xmlTargetDimensions = "TargetDimensions";
		const TargetDimensions DefaultTargetDimensions = TargetDimensions.Arguments;
		TargetDimensions targetDimensions = DefaultTargetDimensions;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartInteractivityOptionsTargetDimensions"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultTargetDimensions)
		]
		public TargetDimensions TargetDimensions {
			get { return targetDimensions; }
			set {
				if (value != targetDimensions) {
					targetDimensions = value;
					OnChanged(ChangeReason.Interactivity);
				}
			}
		}
		internal bool ActualIsDrillDownEnabled {  get { return IsDrillDownEnabled && targetDimensions != TargetDimensions.Points; } }
		protected override bool ShouldSerializeToXml { get { return base.ShouldSerializeToXml || TargetDimensions != DefaultTargetDimensions; } }
		protected internal ChartInteractivityOptions(bool defaultIgnoreMasterFilters)
			: base(defaultIgnoreMasterFilters) { 
		}
		internal override void CopyTo(FilterableDashboardItemInteractivityOptions destination) {
			base.CopyTo(destination);
			ChartInteractivityOptions chartInteractivityOptions = destination as ChartInteractivityOptions;
			if (chartInteractivityOptions != null)
				chartInteractivityOptions.TargetDimensions = TargetDimensions;
		}
		protected override void SaveContentToXml(XElement element) {
			base.SaveContentToXml(element);
			if(TargetDimensions != DefaultTargetDimensions)
				element.Add(new XAttribute(xmlTargetDimensions, targetDimensions));
		}
		protected override void LoadContentFromXml(XElement element) {
			base.LoadContentFromXml(element);
			string targetDimensionsAttr = XmlHelper.GetAttributeValue(element, xmlTargetDimensions);
			if (!String.IsNullOrEmpty(targetDimensionsAttr))
				targetDimensions = XmlHelper.FromString<TargetDimensions>(targetDimensionsAttr);
		}
	}
}
