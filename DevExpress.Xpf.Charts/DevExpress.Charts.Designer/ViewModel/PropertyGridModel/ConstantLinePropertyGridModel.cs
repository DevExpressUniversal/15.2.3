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

using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Charts;
using System.ComponentModel;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartConstantLinePropertyGridModel : PropertyGridModelBase {
		readonly ConstantLine constantLine;
		readonly SetConstantLinePropertyCommand setPropertyCommand;
		WpfChartLineStylePropertyGridModel lineStyle;
		WpfChartConstantLineTitlePropertyGridModel title;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		[Category(Categories.Behavior)]
		public bool Visible {
			get { return constantLine.Visible; }
			set {
				SetProperty("Visible", value);
			}
		}
		[Category(Categories.Behavior)]
		public object Value {
			get { return constantLine.Value; }
			set {
				SetProperty("Value", value);
			}
		}
		[Category(Categories.Brushes)]
		public Brush Brush {
			get { return constantLine.Brush; }
			set {
				SetProperty("Brush", value);
			}
		}
		[Category(Categories.Presentation)]
		public string LegendText {
			get { return constantLine.LegendText; }
			set {
				SetProperty("LegendText", value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DefaultValue(null),
		Category(Categories.Presentation)
		]
		public WpfChartLineStylePropertyGridModel LineStyle {
			get { return lineStyle; }
			set {
				LineStyle newValue = value != null ? new LineStyle() : null;
				SetProperty("LineStyle", newValue);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DefaultValue(null),
		Category(Categories.Elements)
		]
		public WpfChartConstantLineTitlePropertyGridModel Title {
			get { return title; }
			set {
				ConstantLineTitle newValue = value != null ? new ConstantLineTitle() : null;
				SetProperty("Title", newValue);
			}
		}
		public WpfChartConstantLinePropertyGridModel(WpfChartModel chartModel, WpfChartConstantLineModel constantLineModel) : base(chartModel) {
			this.constantLine = constantLineModel.ConstantLine;
			setPropertyCommand = new SetConstantLinePropertyCommand(chartModel);
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (constantLine.LineStyle != null){
				if (lineStyle != null && constantLine.LineStyle != lineStyle.LineStyle || lineStyle == null)
					lineStyle = new WpfChartLineStylePropertyGridModel(ChartModel, constantLine.LineStyle, setPropertyCommand, "LineStyle.");
			}
			else
				lineStyle = null;
			if (constantLine.Title != null) {
				if (title != null && constantLine.Title != title.Title || title == null)
					title = new WpfChartConstantLineTitlePropertyGridModel(ChartModel, constantLine.Title);
			}
			else
				title = null;
		}
	}
	public class WpfChartConstantLineTitlePropertyGridModel : WpfChartTitleBasePropertyGridModel {
		readonly SetConstantLinePropertyCommand setPropertyCommand;
		internal new ConstantLineTitle Title { get { return base.Title as ConstantLineTitle; } }
		[Category(Categories.Layout)]
		public ConstantLineTitleAlignment Alignment {
			get { return Title.Alignment; }
			set { SetProperty("Alignment", value); }
		}
		[Category(Categories.Layout)]
		public bool ShowBelowLine {
			get { return Title.ShowBelowLine; }
			set { SetProperty("ShowBelowLine", value); }
		}
		protected override ICommand SetObjectPropertyCommand {
			get { return setPropertyCommand; }
		}
		public WpfChartConstantLineTitlePropertyGridModel() : this(null, null) { }
		public WpfChartConstantLineTitlePropertyGridModel(WpfChartModel chartModel, ConstantLineTitle title) : base(chartModel, title, "Title.") {
			setPropertyCommand = new SetConstantLinePropertyCommand(chartModel);
		}
	}
}
