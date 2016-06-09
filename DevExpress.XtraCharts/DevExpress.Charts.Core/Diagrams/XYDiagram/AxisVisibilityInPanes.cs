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
namespace DevExpress.Charts.Native {
	public class AxisVisibilityInPanes {
		readonly IAxisData axis;
		readonly Dictionary<IPane, bool> visibility = new Dictionary<IPane, bool>();
		protected IAxisData Axis { get { return axis; } }
		public Dictionary<IPane, bool> Visibility { get { return visibility; } }
		public AxisVisibilityInPanes(IAxisData axis) {
			this.axis = axis;
		}
		public virtual bool IsPaneVisible(IPane pane) {
			bool visible;
			return visibility.TryGetValue(pane, out visible) && visible;
		}
		public virtual bool GetVisibilityInPane(IPane pane) {
			return visibility.ContainsKey(pane) && visibility[pane];
		}
		public virtual bool SetVisibilityInPane(IPane pane, bool visible) {
			if (!visibility.ContainsKey(pane))
				return false;
			Changing();
			visibility[pane] = visible;
			Changed();
			return true;
		}
		public virtual void UpdateVisibilityInPanes(IList<IPane> newPanes) {
			if (newPanes != null) {
				foreach (IPane pane in new List<IPane>(visibility.Keys))
					if (!newPanes.Contains(pane))
						visibility.Remove(pane);
				foreach (IPane pane in newPanes)
					if (!visibility.ContainsKey(pane))
						visibility.Add(pane, true);
			}
		}
		protected virtual void Changing() {
		}
		protected virtual void Changed() {
		}
		protected void Reset(bool visible) {
			foreach (IPane pane in new List<IPane>(visibility.Keys))
				SetVisibilityInPane(pane, visible);
		}
	}
}
