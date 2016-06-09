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
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.DemoBase {
	abstract class GroupedLinksControlCalc {
		internal readonly double hColumnSpacing = 10d;
		protected readonly IGroupedLinksControl Control;
		protected GroupedLinksControlCalc(IGroupedLinksControl control, double hColumnSpacing) {
			this.Control = control;
			this.hColumnSpacing = hColumnSpacing;
		}
		protected abstract List<double> PutGroups(Size constraint, Action<UIElement, double, double, double, double> put);
		protected static Action<UIElement> AsDesired(Action<UIElement, double, double, double, double> put, Func<double> x, Func<double> y) {
			return e => put(e, x(), y(), e.DesiredSize.Width, e.DesiredSize.Height);
		}
		List<ContentControl> GetAllControls(List<UIGroup> groups) {
			return groups.SelectMany(g => new[] {
				g.featuredHeader, g.header, g.newHeader, g.updatedHeader, g.othersHeader, g.showAllDemos
			}.Concat(g.links.Select(l => l.control))).ToList();
		}
		public Size Measure(Size constraint, Action<List<double>> setColumns, VerticalAlignment verticalAlignment) {
			Debug.Assert(!double.IsNaN(constraint.Height));
			Debug.Assert(!double.IsNaN(constraint.Width));
			var inf = new Size(double.PositiveInfinity, double.PositiveInfinity);
			var filtered = Control.FilteredUiGroups;
			Control.SetVisibleGroupsCount(filtered.Count);
			GetAllControls(filtered).ForEach(c => c.Measure(inf));
			var rects = new List<Rect>();
			Action<UIElement, double, double, double, double> put = (e, x, y, w, h) => rects.Add(new Rect(x, y, w, h));
			setColumns(PutGroups(constraint, put));
			double width = rects.Any() ? rects.Max(r => r.X + r.Width) : 0;
			double height = rects.Any() ? rects.Max(r => r.Y + r.Height) : 0;
			width = Math.Max(width, Control.MeasureContraintWidth);
			return new Size(width, verticalAlignment == VerticalAlignment.Stretch ? constraint.Height : height);
		}
		public Size Arrange(Size arrangeBounds) {
			var visible = new HashSet<UIElement>();
			Action<UIElement, double, double, double, double> put = (e, x, y, w, h) => {
				e.Arrange(new Rect(x, y, w, h));
				visible.Add(e);
			};
			PutGroups(arrangeBounds, put);
			var hidden = new HashSet<UIElement>(GetAllControls(Control.UiGroups));
			hidden.ExceptWith(visible);
			foreach(var elem in hidden) {
				elem.Arrange(new Rect());
			}
			return arrangeBounds;
		}
	}
}
