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

using DevExpress.Xpf.Charts;
using System.Collections.Generic;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartConstantLineModel : WpfChartFontModel {
		public override IEnumerable<ChartModelElement> Children {
			get { return null; }
		}
		public ConstantLine ConstantLine {
			get { return (ConstantLine)ChartElement; }
		}
		public string TitleText {
			get {
				if (ConstantLine.Title != null) {
					if (ConstantLine.Title.Content is string || ConstantLine.Title.Content == null)
						return (string)ConstantLine.Title.Content;
					return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ComplexTitleContent);
				}
				else
					return string.Empty;
			}
			set {
				if (ConstantLine.Title != null && (string)ConstantLine.Title.Content != value) {
					ConstantLine.Title.Content = value;
					OnPropertyChanged("TitleText");
				}
			}
		}
		public string LegendText {
			get { return ConstantLine.LegendText; } 
			set{
				if (ConstantLine.LegendText != value) {
					ConstantLine.LegendText = value;
					OnPropertyChanged("LegendText");
				}
			} 
		}
		public object Value {
			get { return ConstantLine.Value; }
			set {
				if (ConstantLine.Value != value) {
					ConstantLine.Value = value;
					OnPropertyChanged("Value");
				}
			}
		}
		public bool ShowTitleBelowLine {
			get {
				if (ConstantLine.Title != null)
					return ConstantLine.Title.ShowBelowLine;
				else
					return (bool)ConstantLineTitle.ShowBelowLineProperty.GetMetadata(typeof(ConstantLineTitle)).DefaultValue;
			}
			set {
				if (ConstantLine.Title != null && ConstantLine.Title.ShowBelowLine != value) {
					ConstantLine.Title.ShowBelowLine = value;
					OnPropertyChanged("ShowTitleBelowLine");
				}
			}
		}
		public bool TitleVisible {
			get {
				if (ConstantLine.Title != null && ConstantLine.Title.Visible.HasValue)
					return ConstantLine.Title.Visible.Value;
				else
					return (bool)ConstantLineTitle.VisibleProperty.GetMetadata(typeof(ConstantLineTitle)).DefaultValue;
			}
			set {
				if (ConstantLine.Title != null && ConstantLine.Title.Visible != value) {
					ConstantLine.Title.Visible = value;
					OnPropertyChanged("TitleVisible");
				}
			}
		}
		public int Thickness {
			get {
				if (ConstantLine.LineStyle != null)
					return ConstantLine.LineStyle.Thickness;
				else
					return (int)LineStyle.ThicknessProperty.GetMetadata(typeof(LineStyle)).DefaultValue;
			}
			set {
				if (ConstantLine.LineStyle != null && ConstantLine.LineStyle.Thickness != value) {
					ConstantLine.LineStyle.Thickness = value;
					OnPropertyChanged("Thickness");
				}
			}
		}
		public SolidColorBrush LineBrush {
			get { return ConstantLine.Brush as SolidColorBrush; }
			set {
				if (ConstantLine.Brush != value) {
					ConstantLine.Brush = value;
					OnPropertyChanged("LineBrush");
				}
			}
		}
		public SolidColorBrush TitleForeground {
			get {
				if (ConstantLine.Title!=null)
					return ConstantLine.Title.Foreground as SolidColorBrush; 
				else
					return (SolidColorBrush)ConstantLineTitle.ForegroundProperty.GetMetadata(typeof(ConstantLineTitle)).DefaultValue;
			}
			set {
				if (ConstantLine.Title.Foreground != value) {
					ConstantLine.Title.Foreground = value;
					OnPropertyChanged("TitleForeground");
				}
			}
		}
		public ConstantLineTitleAlignment TitleAlignment {
			get { return ConstantLine.Title.Alignment; }
			set {
				if (ConstantLine.Title.Alignment != value) {
					ConstantLine.Title.Alignment = value;
					OnPropertyChanged("TitleAlignment");
				}
			}
		}
		public WpfChartConstantLineModel(ChartModelElement parent, ConstantLine constantLine) : base(parent, constantLine) {
			PropertyGridModel = new WpfChartConstantLinePropertyGridModel((WpfChartModel)GetParent<WpfChartModel>(), this);
		}
		public int GetSelfIndex() {
			return ((WpfChartConstantLinesCollectionModel)Parent).IndexOf(this);
		}
	}
}
