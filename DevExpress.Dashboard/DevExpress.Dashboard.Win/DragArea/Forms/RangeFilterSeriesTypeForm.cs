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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
namespace DevExpress.DashboardWin.Native {
	public partial class RangeFilterSeriesTypeForm : DashboardForm {
		readonly ChartSelectorContext context;
		readonly ChartChangeSeriesOptionsCommandBase cancelCommand;
		public RangeFilterSeriesTypeForm() {
			InitializeComponent();
		}
		public RangeFilterSeriesTypeForm(ChartSelectorContext context, IEnumerable<SeriesViewGroup> seriesViewGroups)
			: this() {
			this.context = context;
			cancelCommand = new CancelChartViewSelectorCommand(context);
			seriesGallery.Initialize(new ChartChangeSeriesOptionCommandFactory(context), seriesViewGroups);
			seriesGallery.GalleryControl.Controller = barAndDockingController1;
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			seriesGallery.SelectedItemChanged += OnSeriesGallerySelectedItemChanged;
		}
		void OnSeriesGallerySelectedItemChanged(object sender, GalleryItemEventArgs e) {
			GalleryItem chechedItems = e.Item;
			if(chechedItems != null) {
				ChartChangeSeriesOptionCommand command = chechedItems.Tag as ChartChangeSeriesOptionCommand;
				if(command != null) {
					command.Execute();
				}
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SetSize();
		}
		void SetSize() {
			Size initialSize = ClientSize;
			int additionalHeight = initialSize.Height + seriesGallery.Top - seriesGallery.Bottom;
			int additionalWidth = initialSize.Width + seriesGallery.Left - seriesGallery.Right;
			int minimalWidth = btnCancel.Right - btnOK.Left + (initialSize.Width - btnCancel.Right) * 2 + 20;
			Size size = seriesGallery.CalcBestSize();
			ClientSize = new Size(Math.Max(minimalWidth, size.Width + additionalWidth), size.Height + additionalHeight);
			MinimumSize = new Size(minimalWidth, 0);
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(DialogResult == DialogResult.Cancel) {
				if(cancelCommand != null)
					cancelCommand.Execute();
			}
			else if(context != null)
				context.ApplyHistoryItem();
		}
	}
}
