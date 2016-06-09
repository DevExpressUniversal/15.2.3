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
using DevExpress.DashboardCommon.Localization;
namespace DevExpress.DashboardCommon {
	public enum LogarithmicBase { Base2, Base5, Base10 }
	public class ChartAxisY : ChartAxis {
		static int ConvertToInt(LogarithmicBase type) {
			switch(type) {
				case LogarithmicBase.Base2:
					return 2;
				case LogarithmicBase.Base5:
					return 5;
				case LogarithmicBase.Base10:
					return 10;
				default:
					return 0;
			}
		}
		const string xmlAlwaysShowZeroLevel = "AlwaysShowZeroLevel";
		const string xmlShowGridLines = "ShowGridLines";
		const string xmlLogarithmic = "Logarithmic";
		const string xmlLogarithmicBase = "LogarithmicBase";
		const bool DefaultLogarithmic = false;
		const LogarithmicBase DefaultLogarithmicBase = LogarithmicBase.Base10;
		bool defaultAlwaysShowZeroLevel;
		bool alwaysShowZeroLevel;
		bool showGridLines;
		bool logarithmic = DefaultLogarithmic;
		LogarithmicBase logarithmicBase = DefaultLogarithmicBase;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisYAlwaysShowZeroLevel"),
#endif
		Category(CategoryNames.Data)
		]
		public bool AlwaysShowZeroLevel {
			get { return alwaysShowZeroLevel; }
			set {
				if(value != alwaysShowZeroLevel) {
					alwaysShowZeroLevel = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisYShowGridLines"),
#endif
		Category(CategoryNames.Layout)
		]
		public bool ShowGridLines {
			get { return showGridLines; }
			set {
				if(value != showGridLines) {
					showGridLines = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisYLogarithmic"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultLogarithmic)
		]
		public bool Logarithmic {
			get { return logarithmic; }
			set {
				if(value != logarithmic) {
					logarithmic = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartAxisYLogarithmicBase"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultLogarithmicBase)
		]
		public LogarithmicBase LogarithmicBase {
			get { return logarithmicBase; }
			set {
				if(value != logarithmicBase) {
					logarithmicBase = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		protected override bool DefaultTitleVisible { get { return true; } }
		protected virtual bool DefaultShowGridLines { get { return true; } }
		internal ChartAxisY(IChartAxisContainer container, bool defaultAlwaysShowZeroLevel)
			: base(container) {
			showGridLines = DefaultShowGridLines;
			this.defaultAlwaysShowZeroLevel = defaultAlwaysShowZeroLevel;
			alwaysShowZeroLevel = defaultAlwaysShowZeroLevel;
		}
		internal ChartAxisViewModel CreateViewModel() {
			ChartAxisViewModel viewModel = new ChartAxisViewModel {
				ShowZeroLevel = AlwaysShowZeroLevel,
				ShowGridLines = ShowGridLines,
				Logarithmic = Logarithmic,
				LogarithmicBase = ConvertToInt(logarithmicBase)
			};
			InitializeViewModel(viewModel);
			return viewModel;
		}
		protected override string GetTitle() {
			return Container != null ? Container.GetTitle(false) : string.Empty; 
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(AlwaysShowZeroLevel != defaultAlwaysShowZeroLevel)
				element.Add(new XAttribute(xmlAlwaysShowZeroLevel, alwaysShowZeroLevel));
			if(ShowGridLines != DefaultShowGridLines) 
				element.Add(new XAttribute(xmlShowGridLines, showGridLines));
			if(Logarithmic != DefaultLogarithmic)
				element.Add(new XAttribute(xmlLogarithmic, logarithmic));
			if(LogarithmicBase != DefaultLogarithmicBase)
				element.Add(new XAttribute(xmlLogarithmicBase, logarithmicBase));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string attributeAlwaysShowZeroLevel = element.GetAttributeValue(xmlAlwaysShowZeroLevel);
			if(!String.IsNullOrEmpty(attributeAlwaysShowZeroLevel))
				alwaysShowZeroLevel = XmlHelper.FromString<bool>(attributeAlwaysShowZeroLevel);
			string attributeShowGridLines = element.GetAttributeValue(xmlShowGridLines);
			if(!String.IsNullOrEmpty(attributeShowGridLines))
				showGridLines = XmlHelper.FromString<bool>(attributeShowGridLines);
			string attributeLogarithmic = element.GetAttributeValue(xmlLogarithmic);
			if(!string.IsNullOrEmpty(attributeLogarithmic))
				logarithmic = XmlHelper.FromString<bool>(attributeLogarithmic);
			string attributeLogarithmicBase = element.GetAttributeValue(xmlLogarithmicBase);
			if(!string.IsNullOrEmpty(attributeLogarithmicBase))
				logarithmicBase = XmlHelper.EnumFromString<LogarithmicBase>(attributeLogarithmicBase);
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || AlwaysShowZeroLevel != defaultAlwaysShowZeroLevel || ShowGridLines != DefaultShowGridLines
				|| Logarithmic != DefaultLogarithmic || LogarithmicBase != DefaultLogarithmicBase;
		}
	}
}
