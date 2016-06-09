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

using System;
using System.Collections.Generic;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartAxisLabelModel : ChartModelElement {
		public override IEnumerable<ChartModelElement> Children { get { return null; } }
		public AxisLabel Label { get { return (AxisLabel)ChartElement; } }
		public bool Visible {
			get {
				if (Label != null)
					return Label.Visible;
				else
					return true;
			}
			set {
				if (Label != null && Label.Visible != value) {
					Label.Visible = value;
					OnPropertyChanged("Visible");
				}
			}
		}
		public bool Staggered {
			get {
				if (Label != null)
					return Label.Staggered;
				else
					return false;
			}
			set {
				if (Label != null && Label.Staggered != value) {
					Label.Staggered = value;
					OnPropertyChanged("Staggered");
				}
			}
		}
		public int Angle {
			get { return Label != null ? Label.Angle : 0; }
			set {
				if (Label != null && Label.Angle != value) {
					Label.Angle = value;
					OnPropertyChanged("Angle");
				}
			}
		}
		public WpfChartAxisLabelModel(ChartModelElement parent, AxisLabel axisLabel) : base(parent, axisLabel) {
		}
	}
	public class WpfChartAxisCustomLabelModel : ChartModelElement {
		public override IEnumerable<ChartModelElement> Children {
			get {
				return null;
			}
		}
		public CustomAxisLabel CustomAxisLabel {
			get {
				return (CustomAxisLabel)ChartElement;
			}
		}
		public string DisplayName {
			get {
				if (CustomAxisLabel.Content is string)
					return (string)CustomAxisLabel.Content;
				return ChartDesignerLocalizer.GetString(ChartDesignerStringIDs.ComplexTitleContent);
			}
		}
		public WpfChartAxisCustomLabelModel(ChartModelElement parent, CustomAxisLabel axisLabel) : base(parent, axisLabel) {
		}
	}
}
