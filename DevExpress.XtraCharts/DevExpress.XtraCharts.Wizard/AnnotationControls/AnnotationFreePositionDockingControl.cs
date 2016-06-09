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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationFreePositionDockingControl : ChartUserControl {
		struct DockCornerItem {
			DockCorner dockCorner;
			string text;
			public DockCorner DockCorner { get { return dockCorner; } }
			public DockCornerItem(DockCorner dockCorner) {
				this.dockCorner = dockCorner;
				switch (dockCorner) {
					case DockCorner.LeftBottom:
						text = ChartLocalizer.GetString(ChartStringId.WizDockCornerLeftBottom);
						break;
					case DockCorner.LeftTop:
						text = ChartLocalizer.GetString(ChartStringId.WizDockCornerLeftTop);
						break;
					case DockCorner.RightBottom:
						text = ChartLocalizer.GetString(ChartStringId.WizDockCornerRightBottom);
						break;
					case DockCorner.RightTop:
						text = ChartLocalizer.GetString(ChartStringId.WizDockCornerRightTop);
						break;
					default:
						ChartDebug.Fail("Unknown dock corner");
						goto case DockCorner.LeftTop;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is DockCornerItem) && dockCorner == ((DockCornerItem)obj).dockCorner;
			}
			public override int GetHashCode() {
				return dockCorner.GetHashCode();
			}
		}
		FreePosition position;
		public AnnotationFreePositionDockingControl() {
			InitializeComponent();
		}
		public void Initialize(FreePosition position) {
			this.position = position;
			cbTarget.Properties.Items.Clear();
			IDockTarget[] dockTargets = AnnotationHelper.GetDockTargets(CommonUtils.GetXYDiagram2D(position));
			foreach (IDockTarget dockTarget in dockTargets)
				cbTarget.Properties.Items.Add(AnnotationHelper.GetDockTargetName(dockTarget));
			cbTarget.SelectedItem = AnnotationHelper.GetDockTargetName(position.DockTarget as IDockTarget);
			cbCorner.Properties.Items.Clear();
			foreach (DockCorner dockCorner in Enum.GetValues(typeof(DockCorner)))
				cbCorner.Properties.Items.Add(new DockCornerItem(dockCorner));
			cbCorner.SelectedItem = new DockCornerItem(position.DockCorner);
		}
		void cbTarget_SelectedIndexChanged(object sender, EventArgs e) {
			position.DockTarget = AnnotationHelper.GetDockTarget((string)cbTarget.SelectedItem, CommonUtils.GetXYDiagram2D(position));
		}
		void cbCorner_SelectedIndexChanged(object sender, EventArgs e) {
			position.DockCorner = ((DockCornerItem)cbCorner.SelectedItem).DockCorner;
		}
	}
}
