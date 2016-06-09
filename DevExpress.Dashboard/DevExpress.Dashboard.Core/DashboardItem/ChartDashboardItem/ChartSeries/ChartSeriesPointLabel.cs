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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum PointLabelPosition {
		Outside = 0,
		Inside = 1
	}
	public enum PointLabelContentType {
		Value = 0,
		Argument = 1,
		SeriesName = 2,
		ArgumentAndValue = 3
	}
	public class PointLabelOptions : PointLabelOptionsBase {
		internal static bool IsPositionEnabled(ChartSeries series) {
			SimpleSeries simpleSeries = series as SimpleSeries;
			if(simpleSeries != null)
				return simpleSeries.SeriesType == SimpleSeriesType.Bar;
			return false;
		}
		const bool DefaultShowForZeroValues = false;
		const PointLabelContentType DefaultContent = PointLabelContentType.Value;
		const string xmlShowForZeroValues = "ShowForZeroValues";
		const string xmlContent = "Content";
		bool showForZeroValues;
		PointLabelContentType content;
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowForZeroValues)
		]
		public bool ShowForZeroValues {
			get { return showForZeroValues; }
			set {
				if(value != showForZeroValues) {
					showForZeroValues = value;
					OnChanged();
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContent)
		]
		public PointLabelContentType Content {
			get { return content; }
			set {
				if(value != content) {					
					content = value;
					OnChanged();
				}
			}
		}
		internal event EventHandler<ChangedEventArgs> Changed;
		public PointLabelOptions() {
			showForZeroValues = DefaultShowForZeroValues;
			content = DefaultContent;
		}
		internal override void OnChanged() {
			OnChanged(new ChangedEventArgs(ChangeReason.View, this, null));
		}
		internal void OnChanged(ChangedEventArgs e) {
			if(Changed != null)
				Changed(this, e);
		}
		internal override PointLabelViewModel CreateViewModel() {
			PointLabelViewModel viewModel = base.CreateViewModel();
			viewModel.ShowForZeroValues = ShowForZeroValues;
			viewModel.Content = Content;
			return viewModel;
		}
		protected override void AssignInternal(PointLabelOptionsBase pointLabel) {
			base.AssignInternal(pointLabel);
			PointLabelOptions options = (PointLabelOptions)pointLabel;
			showForZeroValues = options.ShowForZeroValues;
			content = options.Content;
		}
		protected override void SaveToXmlInternal(XElement element) {
			base.SaveToXmlInternal(element);
			ChartSeries.SaveBoolPropertyToXml(element, xmlShowForZeroValues, ShowForZeroValues, DefaultShowForZeroValues);
			if(Content != DefaultContent)
				element.Add(new XAttribute(xmlContent, Content));
		}
		protected override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			ChartSeries.LoadBoolPropertyFromXml(element, xmlShowForZeroValues, ref showForZeroValues);
			string argument = XmlHelper.GetAttributeValue(element, xmlContent);
			if(!String.IsNullOrEmpty(argument))
				Content = XmlHelper.EnumFromString<PointLabelContentType>(argument);
		}
		internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
			showForZeroValues != DefaultShowForZeroValues ||
			Content != DefaultContent;
		}
	}
}
