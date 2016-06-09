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
	internal partial class ChartAxisControl : AxisControlBase {
		public override XtraTabControl TabControl { get { return tbcTabPages; } } 
		public ChartAxisControl() {
			InitializeComponent();
			InitializeTags();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			Axis2D axis2D = Axis as Axis2D;
			if (axis2D != null) {
				axisAppearanceControl.Initialize(axis2D);
				generalTabsControl.Initialize(axis2D, Chart, ChangeAxisNameMethod);
				axesElementsControl.Initialize(lookAndFeel, axis2D, Chart);
				labelsGeneralControl.Initialize(lookAndFeel, axis2D, Chart);
				stripsControl.Initialize(axis2D);
				constantLinesControl.Initialize(lookAndFeel, axis2D);
				Axis axis = axis2D as Axis;
				if (axis == null)
					tbcTabPages.TabPages.Remove(tbScaleBreaks);
				else
					scaleBreaksControl.Initialize(axis, Chart);
				tbScaleOptions.PageVisible = axisScaleOptionsControl.Initialize(generalTabsControl, axis2D, Chart);
			}
		}
		protected override void InitializeTags() {
			tbAppearance.Tag = AxisPageTab.Appearance;
			tbConstantLines.Tag = AxisPageTab.ConstantLines;
			tbElements.Tag = AxisPageTab.Elements;
			tbGeneral.Tag = AxisPageTab.General;
			tbLabels.Tag = AxisPageTab.Labels;
			tbStrips.Tag = AxisPageTab.Strips;
			tbScaleOptions.Tag = AxisPageTab.ScaleOptions;
			if (Axis is Axis)
				tbScaleBreaks.Tag = AxisPageTab.ScaleBreaks;
		}
	}
}
