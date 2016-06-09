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
namespace DevExpress.Xpf.Core.Native {
	public static class DragDropRegionManager {
		public static void RegisterDragDropRegion(object control, string regionName) {
			if(regionName == null) regionName = string.Empty;
			var region = Regions.FirstOrDefault(x => object.Equals(x.RegionName, regionName));
			if(region == null) {
				region = new Region(regionName);
				Regions.Add(region);
			}
			region.Add(control);
		}
		public static void UnregisterDragDropRegion(object control, string regionName) {
			if(regionName == null) regionName = string.Empty;
			var region = Regions.FirstOrDefault(x => object.Equals(x.RegionName, regionName));
			if(region != null)
				region.Remove(control);
		}
		public static IEnumerable<object> GetDragDropControls(string regionName) {
			if(regionName == null) regionName = string.Empty;
			var region = Regions.FirstOrDefault(x => object.Equals(x.RegionName, regionName));
			if(region != null)
				return region.GetControls();
			return new List<object>();
		}
		static readonly List<Region> Regions = new List<Region>();
		class Region {
			public readonly string RegionName;
			public Region(string regionName) {
				RegionName = regionName;
			}
			public void Add(object control) {
				if(control == null) return;
				if(GetReference(control) != null) return;
				ControlReferences.Add(new WeakReference(control));
			}
			public void Remove(object control) {
				if(control == null) return;
				var reference = GetReference(control);
				if(reference == null) return;
				ControlReferences.Remove(reference);
			}
			public IEnumerable<object> GetControls() {
				UpdateReferences();
				return ControlReferences.Select(x => x.Target).ToList();
			}
			readonly List<WeakReference> ControlReferences = new List<WeakReference>();
			void UpdateReferences() {
				List<WeakReference> referencesToDelete = new List<WeakReference>();
				foreach(var reference in ControlReferences)
					if(!reference.IsAlive) referencesToDelete.Add(reference);
				foreach(var reference in referencesToDelete)
					ControlReferences.Remove(reference);
			}
			WeakReference GetReference(object control) {
				UpdateReferences();
				return ControlReferences.FirstOrDefault(x => x.Target == control);
			}
		}
	}
}
