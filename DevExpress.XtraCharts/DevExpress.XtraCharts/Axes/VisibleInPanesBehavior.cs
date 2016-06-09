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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public class VisibleInPanesBehavior : AxisVisibilityInPanes {
		readonly Locker locker = new Locker();
		IList<int> deserializedPaneIds;
		Axis2D Axis2D { get { return (Axis2D)base.Axis; } }
		IList<int> VisibleInPanesIds {
			get {
				if (deserializedPaneIds != null)
					return deserializedPaneIds;
				List<int> ids = new List<int>(Visibility.Count);
				foreach (KeyValuePair<IPane, bool> item in Visibility)
					if (item.Value)
						ids.Add(((XYDiagramPaneBase)item.Key).ActualPaneID);
				return ids;
			}
		}
		public string VisibleInPanesSerializable {
			get {
				IList<int> ids = VisibleInPanesIds;
				int count = ids.Count;
				string[] strings = new string[count];
				for (int i = 0; i < count; i++)
					strings[i] = ids[i].ToString();
				return SerializingUtils.StringArrayToString(strings);
			}
			set {
				string[] array = SerializingUtils.StringToStringArray(value);
				List<int> ids = new List<int>(array.Length);
				foreach (string str in array)
					ids.Add(Convert.ToInt32(str));
				deserializedPaneIds = ids;
			}
		}
		public VisibleInPanesBehavior(Axis2D axis) : base(axis) {
		}
		public void Assign(VisibleInPanesBehavior behavior) {
			deserializedPaneIds = behavior.VisibleInPanesIds;
		}
		public override bool GetVisibilityInPane(IPane pane) {
			if (pane == null)
				throw new ArgumentNullException("pane");
			return base.GetVisibilityInPane(pane);
		}
		public override bool SetVisibilityInPane(IPane pane, bool visible) {
			if (pane == null)
				throw new ArgumentNullException("pane");
			if (!base.SetVisibilityInPane(pane, visible) && Axis2D.Diagram != null)
				throw new InvalidPaneException(ChartLocalizer.GetString(ChartStringId.MsgInvalidPane));
			return true;
		}
		public override void UpdateVisibilityInPanes(IList<IPane> newPanes) {
			base.UpdateVisibilityInPanes(newPanes);
			if (deserializedPaneIds != null) {
				XYDiagram2D diagram = Axis2D.XYDiagram2D;
				if (diagram != null) {
					locker.Lock();
					try {
						Reset(false);
						foreach (int paneID in deserializedPaneIds) {
							XYDiagramPaneBase pane = diagram.GetPaneByID(paneID);
							if (pane != null)
								if (!base.SetVisibilityInPane(pane, true))
									Visibility.Add(pane, true);
						}
						deserializedPaneIds = null;
					}
					finally {
						locker.Unlock();
					}
				}
			}
		}
		protected override void Changing() {
			base.Changing();
			if (!locker.IsLocked)
				Axis2D.SendNotification(new ChartElement.ElementWillChangeNotification(this));
		}
		protected override void Changed() {
			base.Changed();
			if (!locker.IsLocked)
				Axis2D.RaiseControlChanged();
		}
	}
}
