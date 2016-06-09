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
using System.Linq;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraPrinting.Native {
	public class GroupingManager {
		public IDictionary<int, IList<DocumentBandInfo>> PageBands { get; private set; }
		Dictionary<object, IList<GroupingInfo>> pageGroups = new Dictionary<object, IList<GroupingInfo>>();
		HashSet<object> groupKeys = new HashSet<object>();
		public ISet<object> GroupKeys {
			get { return groupKeys; }
		}
		public GroupingManager() {
			PageBands = new Dictionary<int, IList<DocumentBandInfo>>(); ;
		}
		public void AfterBandPrinted(int pageID, DocumentBand band) {
			IList<DocumentBandInfo> bands;
			if(!PageBands.TryGetValue(pageID, out bands)) {
				bands = new List<DocumentBandInfo>();
				PageBands[pageID] = bands;
			}
			bands.Add(new DocumentBandInfo(band.GetPath(), band.GroupKey, band.IsGroupItem));
		}
		public bool TryBuildPageGroups(int pageID, int pageIndex) {
			IList<DocumentBandInfo> bands;
			if(GroupKeys.Count == 0 || !PageBands.TryGetValue(pageID, out bands))
				return false;
			IList<DocumentBandInfo> filterBands = FilterBands(bands, item => item.IsGroupItem || GroupKeys.Contains(item.GroupKey));
			IEnumerable<DocumentBandInfo> sortBands = filterBands.OrderBy<DocumentBandInfo, int[]>(item => item.Path, new ListComparer<int>());
			BuildPageGroups(pageIndex, sortBands);
			return true;
		}
		static IList<DocumentBandInfo> FilterBands(IList<DocumentBandInfo> bands, Predicate<DocumentBandInfo> predicate) {
			List<DocumentBandInfo> result = new List<DocumentBandInfo>(bands.Count);
			foreach(DocumentBandInfo item in bands)
				if(predicate(item))
					result.Add(item);
			return result;
		}
		void BuildPageGroups(int pageIndex, IEnumerable<DocumentBandInfo> bands) {
			foreach(DocumentBandInfo band in bands) {
				int[] path = band.Path;
				if(band.GroupKey != null) {
					IList<GroupingInfo> groups;
					if(!pageGroups.TryGetValue(band.GroupKey, out groups)) {
						groups = new List<GroupingInfo>();
						pageGroups[band.GroupKey] = groups;
					}
					if(groups.Count == 0 || !path.SequenceEqual<int>(groups[groups.Count - 1].Path))
						groups.Add(new GroupingInfo() { Path = path, BeginPageIndex = pageIndex, EndPageIndex = pageIndex });
				} else if(band.IsGroupItem) {
					foreach(IList<GroupingInfo> groups in pageGroups.Values) {
						GroupingInfo group = groups[groups.Count - 1];
						if(path.Length > group.Path.Length)
							group.EndPageIndex = pageIndex;
					}
				}
			}
		}
		public void UpdatePageGroups(int pageIndex) {
			foreach(IList<GroupingInfo> groups in pageGroups.Values) {
				for(int i = groups.Count - 1; i >= 0; i--) {
					GroupingInfo group = groups[i];
					if(pageIndex >= group.BeginPageIndex && pageIndex <= group.EndPageIndex)
						group.EndPageIndex++;
					else
						break;
				}
			}
		}	
		public GroupingInfo GetGroupingInfo(object groupingObject, int pageNumber) {
			IList<GroupingInfo> groups;
			if(!pageGroups.TryGetValue(groupingObject, out groups))
				return null;
			for(int i = groups.Count - 1; i >= 0; i--) {
				GroupingInfo group = groups[i];
				if(pageNumber >= group.BeginPageIndex && pageNumber <= group.EndPageIndex)
					return group;
			}
			return null;
		}
		public void Clear() {
			PageBands.Clear();
			pageGroups.Clear();
		}
	}
	public class DocumentBandInfo {
		public bool IsGroupItem { get; private set; }
		public int[] Path { get; private set; }
		public object GroupKey { get; private set; }
		public DocumentBandInfo(int[] path, object groupKey, bool isGroupItem) {
			Path = path;
			GroupKey = groupKey;
			IsGroupItem = isGroupItem;
		}
	}
	public class GroupingInfo {
		public int[] Path { get; set; }
		public int BeginPageIndex { get; set; }
		public int EndPageIndex { get; set; }
		public GroupingInfo() {
			BeginPageIndex = -1;
			EndPageIndex = -1;
		}
	}
}
