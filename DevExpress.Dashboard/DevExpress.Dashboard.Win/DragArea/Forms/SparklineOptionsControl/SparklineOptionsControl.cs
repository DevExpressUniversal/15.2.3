#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.DashboardCommon;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class SparklineOptionsControl : DashboardUserControl {
		SparklineOptions options = new SparklineOptions();
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SparklineOptions SparklineOptions { get { return options; } }
		public event EventHandler OptionsChanged;
		public SparklineOptionsControl() {
			InitializeComponent();
			foreach(SparklineViewType mode in Enum.GetValues(typeof(SparklineViewType)))
				cbSparklineIndication.Properties.Items.Add(new SparklineIndicationModeItem(mode));
			cbSparklineIndication.SelectedIndex = 0;
		}
		void RaiseOptionsChanged() {
			if(OptionsChanged != null)
				OptionsChanged(this, EventArgs.Empty);
		}
		void ShowMinMaxValuesCheckedChanged(object sender, EventArgs e) {
			options.HighlightMinMaxPoints = ceShowMinMaxPoints.Checked;
			RaiseOptionsChanged();
		}
		void ShowStartEndValuesCheckedChanged(object sender, EventArgs e) {
			options.HighlightStartEndPoints = ceShowStartEndPoints.Checked;
			RaiseOptionsChanged();
		}
		void SparklineIndicationChanged(object sender, EventArgs e) {
			options.ViewType = ((SparklineIndicationModeItem)cbSparklineIndication.SelectedItem).Mode;
			RaiseOptionsChanged();
		}
		public void PrepareOptions(SparklineOptions options) {
			this.options = options.Clone();
			cbSparklineIndication.SelectedItem = new SparklineIndicationModeItem(options.ViewType);
			ceShowMinMaxPoints.Checked = options.HighlightMinMaxPoints;
			ceShowStartEndPoints.Checked = options.HighlightStartEndPoints;
		}
	}
}
