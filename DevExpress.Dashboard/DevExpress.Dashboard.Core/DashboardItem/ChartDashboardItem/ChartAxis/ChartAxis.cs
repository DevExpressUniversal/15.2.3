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
	public abstract class ChartAxis {
		const string xmlReverse = "Reverse";
		const string xmlTitleVisible = "TitleVisible";
		const string xmlVisible = "Visible";
		const bool DefaultReverse = false;
		const bool DefaultVisible = true;
		bool reverse;
		bool titleVisible;
		bool visible = DefaultVisible;
		readonly NameBox titleBox = new NameBox("Title");
		readonly IChartAxisContainer container;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisReverse"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultReverse)
		]
		public bool Reverse {
			get { return reverse; }
			set {
				if(value != reverse) {
					reverse = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisVisible"),
#endif
		DefaultValue(DefaultVisible)
		]
		public bool Visible {
			get { return visible; }
			set {
				if(value != visible) {
					visible = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisTitleVisible")
#else
	Description("")
#endif
		]
		public bool TitleVisible {
			get { return titleVisible; }
			set {
				if(value != titleVisible) {
					titleVisible = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisTitle"),
#endif
		DefaultValue(null),
		Localizable(true)
		]
		public string Title { get { return titleBox.Name; } set { titleBox.Name = value; } }
		internal string DisplayTitle { get { return titleBox.DisplayName; } }
		protected abstract bool DefaultTitleVisible { get; }
		protected IChartAxisContainer Container { get { return container; } }
		protected ChartAxis(IChartAxisContainer container) {
			titleVisible = DefaultTitleVisible;
			titleBox.NameChanged += (s, e) => OnChanged(ChangeReason.View);
			titleBox.RequestDefaultName += (s, e) => e.DefaultName = GetTitle();
			this.container = container;
		}
		protected void OnChanged(ChangeReason reason) {
			if(Container != null)
				Container.OnChanged(reason, this);
		}
		protected abstract string GetTitle();
		protected internal virtual void SaveToXml(XElement element) {
			if(Visible != DefaultVisible)
				element.Add(new XAttribute(xmlVisible, visible));
			if(Reverse != DefaultReverse)
				element.Add(new XAttribute(xmlReverse, reverse));
			if(TitleVisible != DefaultTitleVisible)
				element.Add(new XAttribute(xmlTitleVisible, titleVisible));
			titleBox.SaveToXml(element);
		}
		protected internal virtual void LoadFromXml(XElement element) {
			string visibleAttribute = element.GetAttributeValue(xmlVisible);
			if(!String.IsNullOrEmpty(visibleAttribute))
				visible = XmlHelper.FromString<bool>(visibleAttribute);
			string reverseAttribute = element.GetAttributeValue(xmlReverse);
			if(!String.IsNullOrEmpty(reverseAttribute))
				reverse = XmlHelper.FromString<bool>(reverseAttribute);
			string titleVisibleAttribute = element.GetAttributeValue(xmlTitleVisible);
			if(!String.IsNullOrEmpty(titleVisibleAttribute))
				titleVisible = XmlHelper.FromString<bool>(titleVisibleAttribute);
			titleBox.LoadFromXml(element);
		}
		protected void InitializeViewModel(ChartAxisViewModel viewModel) {
			viewModel.Reverse = reverse;
			viewModel.Title = titleVisible ? DisplayTitle : null;
			viewModel.Visible = visible;
		}
		protected internal virtual bool ShouldSerialize() {
			return Visible != DefaultVisible || titleBox.ShouldSerialize() || TitleVisible != DefaultTitleVisible || Reverse != DefaultReverse; 
		} 
	}
}
