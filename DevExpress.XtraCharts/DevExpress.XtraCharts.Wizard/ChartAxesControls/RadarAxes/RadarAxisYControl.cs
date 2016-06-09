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

using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class RadarAxisYControl : AxisControlBase {
		public override XtraTabControl TabControl { get { return xtraTabControl; } }
		public RadarAxisYControl() {
			InitializeComponent();
			InitializeTags();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			RadarAxisY axis = Axis as RadarAxisY;
			if (axis != null) {
				generalTabsControl.Initialize(axis, Chart);
				axisAppearanceControl.Initialize(axis);
				axisElementsControl.Initialize(axis);
				radarAxisYLabelControl.Initialize(lookAndFeel, axis, Chart);
				tbScaleOptions.PageVisible = axisScaleOptionsControl.Initialize(generalTabsControl, axis, Chart);
			}
		}
		protected override void InitializeTags() {
			tbAppearance.Tag = AxisPageTab.Appearance;
			tbElements.Tag = AxisPageTab.Elements;
			tbGeneral.Tag = AxisPageTab.General;
			tbLabels.Tag = AxisPageTab.Labels;
			tbScaleOptions.Tag = AxisPageTab.ScaleOptions;
		}
	}
}
