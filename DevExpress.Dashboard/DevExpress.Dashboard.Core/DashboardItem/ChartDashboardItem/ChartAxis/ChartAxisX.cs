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
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public class ChartAxisX : ChartAxis {
		const int DefaultVisiblePointsCount = 10;
		const bool DefaultEnableZooming = false;
		const bool DefaultLimitVisiblePoints = false;
		const string xmlVisiblePointsCount = "VisiblePointsCount";
		const string xmlEnableZooming = "EnableZooming";
		const string xmlLimitVisiblePoints = "LimitVisiblePoints";
		bool enableZooming = DefaultEnableZooming;
		bool limitVisiblePoints = DefaultLimitVisiblePoints;
		int visiblePointsCount = DefaultVisiblePointsCount;
		protected override bool DefaultTitleVisible { get { return false; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisXEnableZooming"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultEnableZooming)
		]
		public bool EnableZooming {
			get { return enableZooming; }
			set {
				if(enableZooming != value) {
					enableZooming = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisXLimitVisiblePoints"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultLimitVisiblePoints)
		]
		public bool LimitVisiblePoints {
			get { return limitVisiblePoints; }
			set {
				if(limitVisiblePoints != value) {
					limitVisiblePoints = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisXVisiblePointsCount"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultVisiblePointsCount)
		]
		public int VisiblePointsCount {
			get { return visiblePointsCount; }
			set {
				if(visiblePointsCount != value) {
					visiblePointsCount = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		public ChartAxisX(ChartDashboardItem container)
			: base(container) {
		}
		internal ChartAxisX(IChartAxisContainer container)
			: base(container) {
		}
		protected override string GetTitle() {
			return Container != null ? Container.GetTitle(false) : string.Empty;
		}
		internal ChartAxisViewModel CreateViewModel(ChartDashboardItemViewModel model) {
			ChartAxisViewModel viewModel = new ChartAxisViewModel();
			InitializeViewModel(viewModel);
			bool isContinuousArgumentScale = model.Argument.Type != ChartArgumentType.String;
			bool isArgumentReverseRequired = isContinuousArgumentScale && Container.isReverseRequiredForContinuousScale;
			viewModel.Reverse = (viewModel.Reverse ^ isArgumentReverseRequired);
			viewModel.EnableZooming = EnableZooming;
			viewModel.LimitVisiblePoints = LimitVisiblePoints && VisiblePointsCount > 0 && !isContinuousArgumentScale;
			viewModel.VisiblePointsCount = VisiblePointsCount;
			return viewModel;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(EnableZooming != DefaultEnableZooming)
				element.Add(new XAttribute(xmlEnableZooming, EnableZooming));
			if(LimitVisiblePoints != DefaultLimitVisiblePoints)
				element.Add(new XAttribute(xmlLimitVisiblePoints, LimitVisiblePoints));
			if(VisiblePointsCount != DefaultVisiblePointsCount)
				element.Add(new XAttribute(xmlVisiblePointsCount, VisiblePointsCount));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string enableZoomingAttr = element.GetAttributeValue(xmlEnableZooming);
			if(!String.IsNullOrEmpty(enableZoomingAttr))
				enableZooming = XmlHelper.FromString<bool>(enableZoomingAttr);
			string maxVisibleCountEnabledAttr = element.GetAttributeValue(xmlLimitVisiblePoints);
			if(!String.IsNullOrEmpty(maxVisibleCountEnabledAttr))
				limitVisiblePoints = XmlHelper.FromString<bool>(maxVisibleCountEnabledAttr);
			string maxVisibleCountAttr = element.GetAttributeValue(xmlVisiblePointsCount);
			if(!String.IsNullOrEmpty(maxVisibleCountAttr))
				visiblePointsCount = XmlHelper.FromString<int>(maxVisibleCountAttr);
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize()
				|| EnableZooming != DefaultEnableZooming
				|| LimitVisiblePoints != DefaultLimitVisiblePoints
				|| VisiblePointsCount != DefaultVisiblePointsCount;
		} 
	}
}
