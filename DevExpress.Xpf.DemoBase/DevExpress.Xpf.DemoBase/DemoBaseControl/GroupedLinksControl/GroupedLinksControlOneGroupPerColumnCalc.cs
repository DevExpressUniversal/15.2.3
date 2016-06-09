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
using System.Linq;
using System.Windows;
namespace DevExpress.Xpf.DemoBase {
	class GroupedLinksControlOneGroupPerColumnCalc : GroupedLinksControlCalc {
		public GroupedLinksControlOneGroupPerColumnCalc(IGroupedLinksControl control, double hColumnSpacing) 
			: base(control, hColumnSpacing) { }
		protected override List<double> PutGroups(Size constraint, Action<UIElement, double, double, double, double> put) {
			double x = 0, y = 0;
			var asDesired = AsDesired(put, () => x, () => y);
			double computedColumnSpacing = 0;
			if(Control.FilteredUiGroups.Any()) {
				computedColumnSpacing = Control.FilteredUiGroups
					.Select(g => g.header).Cast<UIElement>()
					.Concat(Control.UiGroups.SelectMany(g => g.links.Select(l => l.control)))
					.Max(e => e.DesiredSize.Width) + hColumnSpacing;
			}
			foreach(var group in Control.FilteredUiGroups) {
				var newLinks = group.links.Where(l => l.link.IsNew).ToList();
				var updatedLinks = group.links.Where(l => l.link.IsUpdated).ToList();
				var featuredLinks = group.links.Where(l => l.link.IsHighlighted).Except(newLinks).Except(updatedLinks).ToList();
				var otherLinks = group.links.Except(newLinks).Except(updatedLinks).Except(featuredLinks).ToList();
				var subGroups = new[] {
					new { Header = group.newHeader, Links = newLinks },
					new { Header = group.updatedHeader, Links = updatedLinks },
					new { Header = group.featuredHeader, Links = featuredLinks },
					new { Header = group.othersHeader, Links = otherLinks }
				};
				asDesired(group.header);
				y += group.header.DesiredSize.Height;
				bool drawShowAllDemos = false;
				foreach(var subGroup in subGroups.Where(sg => sg.Links.Any())) {
					if(y + subGroup.Header.DesiredSize.Height +
					   group.showAllDemos.DesiredSize.Height > constraint.Height) {
						drawShowAllDemos = true;
						break;
					}
					asDesired(subGroup.Header);
					y += subGroup.Header.DesiredSize.Height;
					foreach(var link in subGroup.Links) {
						if(y + link.control.DesiredSize.Height + group.showAllDemos.DesiredSize.Height > constraint.Height) {
							drawShowAllDemos = true;
							break;
						}
						asDesired(link.control);
						y += link.control.DesiredSize.Height;
					}
				}
				if(drawShowAllDemos) {
					asDesired(group.showAllDemos);
				}
				y = 0d;
				x += computedColumnSpacing;
			}
			return Enumerable.Range(0, Control.FilteredUiGroups.Count).Select(_ => computedColumnSpacing).ToList();
		}
	}
}
