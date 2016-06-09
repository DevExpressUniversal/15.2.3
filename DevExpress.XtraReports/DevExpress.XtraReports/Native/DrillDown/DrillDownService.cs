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
using System.Text;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.DrillDown {
	public interface IDrillDownService : IDrillDownServiceBase {
		bool BandExpanded(Band band);
		bool TryGetGroupExpanded(DrillDownKey key, out bool value);
		void SetGroupExpanded(DrillDownKey key, bool value);
		DrillDownKey CreateKey(string name, int level, int[] indices);
		void Clear();
		void SetDrillDownControlPresent();
	}
	public class DrillDownService : IDrillDownService {
		bool isDrillDowning = false;
		bool hasDrillDownControls;
		readonly Dictionary<DrillDownKey, bool> groupExpanded = new Dictionary<DrillDownKey, bool>();
		readonly KeyCreator keyCreator = new KeyCreator();
		public Dictionary<DrillDownKey, bool> GroupExpanded {
			get { return groupExpanded; }
		}
		public DrillDownService() {
		}
		IDictionary<DrillDownKey, bool> IDrillDownServiceBase.Keys {
			get { return groupExpanded; }
		}
		bool IDrillDownServiceBase.IsDrillDowning {
			get { return isDrillDowning; }
			set { isDrillDowning = value; }
		}
		void IDrillDownServiceBase.Reset() {
			if(!isDrillDowning) {
				groupExpanded.Clear();
			} else
				throw new InvalidOperationException();
		}
		void IDrillDownService.Clear() {
			keyCreator.Clear();
		}
		DrillDownKey IDrillDownService.CreateKey(string name, int level, int[] indices) {
			return keyCreator.CreateKey(name, level, indices);
		}
		bool IDrillDownService.BandExpanded(Band band) {
			if(!hasDrillDownControls)
				return true;
			foreach(Band item in band.Report.OrderedBands) {
				if(ReferenceEquals(item, band))
					return band.DrillDownExpandedInternal;
				if(!ReferenceEquals(item.Report, band.Report) || item.DrillDownExpandedInternal)
					continue;
				if(band is GroupFooterBand && band.LevelInternal > item.LevelInternal)
					continue;
				return false;
			}
			return true;
		}
		bool IDrillDownService.TryGetGroupExpanded(DrillDownKey key, out bool value) {
			return TryGetGroupExpandedCore(key, out value);
		}
		bool TryGetGroupExpandedCore(DrillDownKey key, out bool value) {
			return groupExpanded.TryGetValue(key, out value);
		}
		void IDrillDownService.SetGroupExpanded(DrillDownKey key, bool value) {
			groupExpanded[key] = value;
		}
		void IDrillDownService.SetDrillDownControlPresent() {
			hasDrillDownControls = true;
		}
	}
	class KeyCreator {
		class LevelKey : IComparable<LevelKey> {
			public int LevelGroup { get; set; }
			public int Level { get; set; }
			public override int GetHashCode() {
				return LevelGroup ^ Level;
			}
			public override bool Equals(object obj) {
				LevelKey other = obj as LevelKey;
				return other != null && LevelGroup == other.LevelGroup && Level == other.Level;
			}
			int IComparable<LevelKey>.CompareTo(LevelKey other) {
				int result = Comparer<int>.Default.Compare(LevelGroup, other.LevelGroup);
				return result != 0 ? result : Comparer<int>.Default.Compare(Level, other.Level);
			}
		}
		List<int> relativeIndices = new List<int>();
		List<int> storedIndices = new List<int>();
		SortedList<LevelKey, int> levelIndices = new SortedList<LevelKey, int>();
		public DrillDownKey CreateKey(string name, int level, params int[] indices) {
			int[] result = CreateRelativeIndices(name, level, indices);
			return new DrillDownKey(name, result);
		}
		int[] CreateRelativeIndices(string name, int level, params int[] indices) {
			List<int> result = new List<int>(indices.Length);
			for(int i = 0; i < indices.Length - 1; i++) {
				if(relativeIndices.Count == i) {
					relativeIndices.Add(0);
					storedIndices.Add(indices[i]);
				} else if(indices[i] == 0) {
					relativeIndices[i] = 0;
					storedIndices[i] = 0;
				} else if(indices[i] > storedIndices[i]) {
					relativeIndices[i] = relativeIndices[i] + 1;
					storedIndices[i] = indices[i];
				}
				result.Add(relativeIndices[i]);
			}
			LevelKey levelKey = new LevelKey() { LevelGroup = indices.Length, Level = level };
			if(!levelIndices.ContainsKey(levelKey))
				levelIndices[levelKey] = -1;
			for(int i = levelIndices.Count - 1; i >= 0; i--) {
				LevelKey key = levelIndices.Keys[i];
				if(key.LevelGroup != levelKey.LevelGroup)
					continue;
				if(key.Level < level)
					levelIndices[key] = -1;
				else if(key.Level == level) {
					int value = indices[indices.Length - 1] == 0 ? 0 : levelIndices.Values[i] + 1;
					levelIndices[key] = value;
					result.Add(value);
				} else
					result.Add(levelIndices.Values[i]);
			}
			return result.ToArray();
		}
		public void Clear() {
			relativeIndices.Clear();
			storedIndices.Clear();
			levelIndices.Clear();
		}
	}
}
