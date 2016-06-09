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
	public enum ScatterPointLabelContentType {
		Argument = 0,
		Weight = 1,
		Values = 2,
		ArgumentAndWeight = 3,
		ArgumentAndValues = 4
	}
	public class ScatterPointLabelOptions : PointLabelOptionsBase {
		const ScatterPointLabelContentType DefaultContent = ScatterPointLabelContentType.Argument;
		const string xmlContent = "Content";
		readonly ScatterChartDashboardItem dashboardItem;
		ScatterPointLabelContentType content;
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContent)
		]
		public ScatterPointLabelContentType Content {
			get { return content; }
			set {
				if(value != content) {
					content = value;
					OnChanged();
				}
			}
		}
		internal ScatterPointLabelOptions(ScatterChartDashboardItem dashboardItem) {
			this.dashboardItem = dashboardItem;
			content = DefaultContent;
		}
		internal override void OnChanged() {
			if(dashboardItem != null)
				dashboardItem.OnChanged(ChangeReason.View);
		}
		internal override PointLabelViewModel CreateViewModel() {
			PointLabelViewModel viewModel = base.CreateViewModel();
			viewModel.ScatterContent = Content;
			return viewModel;
		}
		protected override void AssignInternal(PointLabelOptionsBase pointLabel) {
			base.AssignInternal(pointLabel);
			ScatterPointLabelOptions options = (ScatterPointLabelOptions)pointLabel;
			content = options.Content;
		}
		protected override void SaveToXmlInternal(XElement element) {
			base.SaveToXmlInternal(element);
			if(Content != DefaultContent)
				element.Add(new XAttribute(xmlContent, Content));
		}
		protected override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			string argument = XmlHelper.GetAttributeValue(element, xmlContent);
			if(!String.IsNullOrEmpty(argument))
				Content = XmlHelper.EnumFromString<ScatterPointLabelContentType>(argument);
		}
		internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || Content != DefaultContent;
		}
	}
}
