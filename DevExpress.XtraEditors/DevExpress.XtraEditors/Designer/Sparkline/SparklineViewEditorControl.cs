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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Localization;
namespace DevExpress.XtraEditors.Design {
	public partial class SparklineViewEditorControl : UserControl {
		bool isInitialized;
		IWindowsFormsEditorService edSvc;
		SparklineViewBase sparklineView;
		SparklineViewBase newSparklineView;
		public SparklineViewBase NewSparklineView { get { return newSparklineView; } }
		public SparklineViewEditorControl(IWindowsFormsEditorService edSvc, SparklineViewBase sparklineView) {
			InitializeComponent();
			this.sparklineView = sparklineView;
			newSparklineView = sparklineView;
			this.edSvc = edSvc;
			InitializeItems();
		}
		void InitializeItems() {
			foreach (SparklineViewType value in Enum.GetValues(typeof(SparklineViewType))) {
				SparklineViewTypeItem item = new SparklineViewTypeItem(value, SparklineLocalizationHelper.GetSparklineViewName(value));
				lbcViews.Items.Add(item);
				if (sparklineView.Type == value)
					lbcViews.SelectedItem = item;
			}
			isInitialized = true;
		}
		void CloseDropDown() {
			if (edSvc != null)
				edSvc.CloseDropDown();
		}
		void lbcViews_SelectedIndexChanged(object sender, EventArgs e) {
			if (isInitialized) {
				SparklineViewTypeItem viewTypeItem = (SparklineViewTypeItem)lbcViews.SelectedItem;
				if (viewTypeItem.ViewType != sparklineView.Type) {
					newSparklineView = SparklineViewBase.CreateView(viewTypeItem.ViewType);
					newSparklineView.Assign(sparklineView);
				}
				else
					newSparklineView = sparklineView;
				CloseDropDown();
			}
		}
		void lbcViews_DoubleClick(object sender, EventArgs e) {
			if (isInitialized)
				CloseDropDown();
		}
	}
	public class SparklineViewTypeItem {
		SparklineViewType viewType;
		string name;
		public SparklineViewType ViewType { get { return viewType; } }
		public SparklineViewTypeItem(SparklineViewType viewType, string name) {
			this.viewType = viewType;
			this.name = name;
		}
		public override string ToString() {
			return name;
		}
	}
}
