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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Design {
	public partial class ChartSeriesOptionsForm : DashboardForm {
		ChartSeries chartSeries;
		public ChartSeries ChartSeries {
			get { return chartSeries; }
			set { 
				chartSeries = value; 
				btnOK.Enabled = value != null;
			}
		}
		public ChartSeriesOptionsForm() {
			InitializeComponent();
			seriesGallery.GalleryControl.Gallery.ItemClick += OnGalleryItemClick;
			seriesGallery.GalleryControl.Gallery.ItemDoubleClick += OnGalleryItemDoubleClick;
			seriesGallery.GalleryControl.Controller = barAndDockingController1;
		}
		public void InitializeGallery(IEnumerable<SeriesViewGroup> groups) {
			seriesGallery.Initialize(new ChartNewSeriesCommandFactory(this), groups);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			Size initialSize = ClientSize;
			int additionalHeight = initialSize.Height + seriesGallery.Top - seriesGallery.Bottom;
			int additionalWidth = initialSize.Width + seriesGallery.Left - seriesGallery.Right;
			int buttonRight = btnCancel.Right;
			int minimalWidth = buttonRight - btnOK.Left + (initialSize.Width - buttonRight) * 2 + 20;
			Size size = seriesGallery.CalcBestSize();
			ClientSize = new Size(Math.Max(minimalWidth, size.Width + additionalWidth), size.Height + additionalHeight);
			MinimumSize = new Size(minimalWidth, 0);
		}
		bool ExecuteCommand(GalleryItem item) {
			if(item != null) {
				ChartSeriesOptionsCommandBase command = item.Tag as ChartSeriesOptionsCommandBase;
				if(command != null) {
					command.Execute();
					return true;
				}
			}
			return false;
		}
		void OnGalleryItemClick(object sender, GalleryItemClickEventArgs e) {
			ExecuteCommand(e.Item);
		}
		void OnGalleryItemDoubleClick(object sender, GalleryItemClickEventArgs e) {
			if(ExecuteCommand(e.Item)) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		protected override void DisposeInternal(bool disposing) {
			if(disposing && seriesGallery != null && seriesGallery.GalleryControl != null && seriesGallery.GalleryControl.Gallery != null)
				seriesGallery.GalleryControl.Gallery.ItemDoubleClick -= OnGalleryItemDoubleClick;
			base.DisposeInternal(disposing);
		}
		internal void InitLookAndFeel(UserLookAndFeel userLookAndFeel) {
			LookAndFeel.ParentLookAndFeel = userLookAndFeel;
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = userLookAndFeel;
		}
	}
	class NewSeriesCommand : ChartSeriesOptionsCommandBase {
		private ChartSeriesOptionsForm context;
		private ChartSeriesConverter converter;
		public override bool Selected {
			get { return false; }
		}
		public NewSeriesCommand(ChartSeriesOptionsForm context, ChartSeriesConverter converter) {
			this.context = context;
			this.converter = converter;
		}
		public override void Execute() {
			context.ChartSeries = converter.CreateSeries();
		}
	}
}
