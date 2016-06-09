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
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Xpf.DemoBase {
	public class GroupedLinksControlPanel : Panel, IGroupedLinksControl {
		public VerticalAlignment LocalVerticalAlignment {
			get { return (VerticalAlignment)GetValue(LocalVerticalAlignmentProperty); }
			set { SetValue(LocalVerticalAlignmentProperty, value); }
		}
		public static readonly DependencyProperty LocalVerticalAlignmentProperty =
			DependencyProperty.Register("LocalVerticalAlignment", typeof(VerticalAlignment), typeof(GroupedLinksControlPanel),
				new FrameworkPropertyMetadata(VerticalAlignment.Stretch, FrameworkPropertyMetadataOptions.AffectsMeasure));
		List<UIGroup> uiGroups;
		GroupedLinksControl control;
		public GroupedLinksControlPanel(List<UIGroup> uiGroups, GroupedLinksControl control) {
			this.uiGroups = uiGroups;
			this.control = control;
			SetBinding(LocalVerticalAlignmentProperty, new Binding("VerticalAlignment") { Source = control, Mode = BindingMode.TwoWay });
			this.calc = new Lazy<GroupedLinksControlCalc>(() => {
				if (control.LayoutType == GroupedLinksControlLayoutType.OneGroupPerColumn)
					return new GroupedLinksControlOneGroupPerColumnCalc(this, control.HColumnSpacing);
				return new GroupedLinksControlWrappedCalc(this, control.MaxGroupsPerColumn, control.HColumnSpacing);
			});
		}
		Lazy<GroupedLinksControlCalc> calc;
		GroupedLinksControlCalc Calc { get { return calc.Value; } }
		bool Contains(string text, string str) {
			return CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, str, CompareOptions.IgnoreCase) != -1 ||
				   CultureInfo.InvariantCulture.CompareInfo.IndexOf(text.Replace(" ", ""), str.Replace(" ", ""), CompareOptions.IgnoreCase) != -1;
		}
		protected override Size MeasureOverride(Size constraint) {
			return Calc.Measure(constraint, c => control.Columns = c, LocalVerticalAlignment);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			return Calc.Arrange(arrangeBounds);
		}
		public Rect VisibleArea { get; set; }
		public List<UIGroup> UiGroups {
			get { return uiGroups; }
		}
		public List<UIGroup> FilteredUiGroups {
			get {
				return uiGroups.Select(g => new UIGroup {
					featuredHeader = g.featuredHeader,
					header = g.header,
					newHeader = g.newHeader,
					showAllDemos = g.showAllDemos,
					updatedHeader = g.updatedHeader,
					othersHeader = g.othersHeader,
					links = g.links.Where(l => Contains(l.link.Title + g.group.Header, control.FilterString)).ToList()
				}).Where(g => g.links.Any()).ToList();
			}
		}
		public double MeasureContraintWidth { get { return control.MeasureConstraint.Width; } }
		public void SetVisibleGroupsCount(int count) {
			control.VisibleGroupsCount = count;
		}
	}
}
