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
	class GroupedLinksControlWrappedCalc : GroupedLinksControlCalc {
		internal const double vColumnSpacing = 19d;
		int maxGroupsPerColumn;
		public GroupedLinksControlWrappedCalc(IGroupedLinksControl control, int maxItemsPerColumn, double hColumnSpacing)
			: base(control, hColumnSpacing) {
			this.maxGroupsPerColumn = maxItemsPerColumn;
		}
		protected override List<double> PutGroups(Size constraint, Action<UIElement, double, double, double, double> put) {
			double x = 0, y = 0;
			var asDesired = AsDesired(put, () => x, () => y);
			double columnWidth = 0;
			int columns = 1;
			int groupCount = 0;
			foreach(var group in Control.FilteredUiGroups) {
				bool notEnoughHeight = y + group.header.DesiredSize.Height + group.links.First().control.DesiredSize.Height * 3 > constraint.Height;
				if(groupCount > 0 && (notEnoughHeight || (groupCount >= maxGroupsPerColumn && maxGroupsPerColumn > 0))) {
					groupCount = 0;
					columns++;
					y = 0d;
					x += columnWidth + hColumnSpacing;
					columnWidth = 0;
				}
				columnWidth = Math.Max(columnWidth, group.header.DesiredSize.Width);
				asDesired(group.header);
				y += group.header.DesiredSize.Height;
				int linkIndex = 0;
				foreach(var link in group.links) {
					if(group.links.Count - linkIndex > 1 && y + link.control.DesiredSize.Height * 2 > constraint.Height) {
						columns++;
						y = 0d;
						x += columnWidth + hColumnSpacing;
						columnWidth = 0;
					}
					columnWidth = Math.Max(columnWidth, link.control.DesiredSize.Width);
					asDesired(link.control);
					y += link.control.DesiredSize.Height;
					linkIndex++;
				}
				if(y > 0.01d) { 
					y += vColumnSpacing;
				}
				groupCount++;
			}
			return Enumerable.Range(0, columns).Select(_ => columnWidth).ToList();
		}
	}
}
