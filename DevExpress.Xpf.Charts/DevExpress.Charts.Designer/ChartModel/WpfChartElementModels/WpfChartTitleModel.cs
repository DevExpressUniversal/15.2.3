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

using System.Collections.Generic;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartTitleModelBase : WpfChartFontModel {
		TitleBase Title { get { return (TitleBase)ChartElement; } }
		public string DisplayName {
			get {
				if ((Title != null) && (Title.Content != null)) {
					if (Title.Content is string)
						return Title.Content as string;
					return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ComplexTitleContent);
				}
				else
					return string.Empty;
			}
			set {
				if (!(Title.Content is string) || (string)Title.Content != value) {
					Title.Content = value;
					OnPropertyChanged("DisplayName");
				}
			}
		}
		public bool Visible {
			get {
				if (Title != null)
					return Title.Visible.HasValue ? Title.Visible.Value : true;
				else
					return false;
			}
			set {
				if (Title.Visible != value) {
					Title.Visible = value;
					OnPropertyChanged("Visible");
				}
			}
		}
		public WpfChartTitleModelBase(ChartModelElement parent, TitleBase title) : base(parent, title) { }
	}
	public class WpfChartTitleModel : WpfChartTitleModelBase {
		public Title Title { get { return (Title)ChartElement; } }
		public override IEnumerable<ChartModelElement> Children { get { return null; } }
		public WpfChartTitleModel(ChartModelElement parent, Title title) : base(parent, title) {
				PropertyGridModel = new WpfChartTitlePropertyGridModel(this);
		}
		public int GetSelfIndex() {
			if (Parent is WpfChartTitleCollectionModel)
					return ((WpfChartTitleCollectionModel)Parent).IndexOf(this);
			return -1;
		}
	}
	public class WpfChartAxisTitleModel : WpfChartTitleModelBase {
		public AxisTitle Title { get { return (AxisTitle)ChartElement; } }
		public TitleAlignment Alignment {
			get { return Title != null ? Title.Alignment : TitleAlignment.Near; }
			set {
				if (Title.Alignment != value) {
					Title.Alignment = value;
					OnPropertyChanged("Alignment");
				}
			}
		}
		public override IEnumerable<ChartModelElement> Children { get { return null; } }
		public WpfChartAxisTitleModel(ChartModelElement parent, AxisTitle title)
			: base(parent, title) {
		}
	}
}
