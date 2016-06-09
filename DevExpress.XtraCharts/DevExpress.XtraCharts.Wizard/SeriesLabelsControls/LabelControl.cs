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

using System.Collections;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class LabelControl : LabelControlBase {
		ILabelBehaviorProvider LabelBehaviorProvider { get { return Label; } }
		public override XtraTabControl TabControl { get { return tbcPagesControl; } }
		public LabelControl() {
			InitializeComponent();
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			generalControl.Initialize(lookAndFeel, Label, Series, Chart, UpdateControls);
			if (LabelBehaviorProvider.ConnectorSupported)
				labelConnectorControl.Initialize(Label);
			else
				tbcPagesControl.TabPages.Remove(tbConnector);
			backgroundControl.Initialize(Label, null);
			borderControl.Initialize(Label.Border);
			if (LabelBehaviorProvider.ShadowSupported)
				shadowControl.Initialize(Label.Shadow);
			else
				tbcPagesControl.TabPages.Remove(tbShadow);
			pointOptionsControl.Initialize(Series);
			UpdateControls();
		}
		protected override void InitializeTags() {
			tbGeneral.Tag = ParseEnum("General");
			if (LabelBehaviorProvider.ConnectorSupported)
				tbConnector.Tag = ParseEnum("Line");
			tbAppearance.Tag = ParseEnum("Appearance");
			tbBorder.Tag = ParseEnum("Border");
			tbPointOptions.Tag = ParseEnum("PointOptions");
			if (LabelBehaviorProvider.ShadowSupported)
				tbShadow.Tag = ParseEnum("Shadow");
		}
		void UpdateControls() {
			labelConnectorControl.Enabled = Series.ActualLabelsVisibility && LabelBehaviorProvider.ConnectorEnabled;
			backgroundControl.Enabled = Series.ActualLabelsVisibility;
			borderControl.Enabled = Series.ActualLabelsVisibility;
			shadowControl.Enabled = Series.ActualLabelsVisibility;
		}
	}
}
