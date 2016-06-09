#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors.Registrator;
using System.Drawing;
using DevExpress.XtraEditors.ViewInfo;
using System.IO;
using System.Drawing.Imaging;
using DevExpress.ExpressApp.Chart;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Chart.Win {
	[DevExpress.ExpressApp.Editors.PropertyEditor(typeof(ISparklineProvider), true)]
	public class SparklinePropertyEditor : ChartPropertyEditor, IInplaceEditSupport {
		public SparklinePropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
		static SparklinePropertyEditor() {
			SparklineRepositoryItem.Register();
		}
		#region IInplaceEditSupport Members
		public RepositoryItem CreateRepositoryItem() {
			return new SparklineRepositoryItem();
		}
		#endregion
	}
	public class SparklinePainter : BaseEditPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			ISparklineProvider sparklineProvider = ((DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo)(info.ViewInfo)).EditValue as ISparklineProvider;
			SparklineRepositoryItem sparklineRepositoryItem = ((DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo)(info.ViewInfo)).Item as SparklineRepositoryItem;
			if(sparklineProvider != null && sparklineRepositoryItem != null) {
				info.Graphics.DrawImage(sparklineRepositoryItem.GetSparklineImage(sparklineProvider, info.Bounds.Width, info.Bounds.Height), info.Bounds);
			}
		}
	}
	public class SparklineRepositoryItem : RepositoryItem {
		internal const string EditorName = "ChartControl";
		private ChartControl chartControl;
		private Dictionary<int, Image> sparklineImagesCache = new Dictionary<int, Image>();
		private void EnsureSparklineChartControl() {
			if(chartControl == null) {
				chartControl = new ChartControl();
				chartControl.BeginInit();
				SparklineHelper.Setup(chartControl);
				chartControl.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
				chartControl.EndInit();
			}
		}
		protected internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(ChartControl),
					typeof(SparklineRepositoryItem), typeof(BaseEditViewInfo),
					new SparklinePainter(), false, EditImageIndexes.ImageEdit));
			}
		}
		static SparklineRepositoryItem() {
			Register();
		}
		public Image GetSparklineImage(ISparklineProvider dataSourceProvider, int width, int height) {
			Image result = null;
			int dataSourceProviderHashCode = dataSourceProvider.GetHashCode();
			if(sparklineImagesCache.TryGetValue(dataSourceProviderHashCode, out result)
				&& (Math.Abs((float)result.Size.Width - (float)width) / width > 0.2 || Math.Abs((float)result.Size.Height - (float)height) / height > 0.2)) {
				sparklineImagesCache.Remove(dataSourceProviderHashCode);
				result = null;
			}
			if(result == null) {
				EnsureSparklineChartControl();
				chartControl.Size = new Size(width, height);
				#region Series
				chartControl.DataSource = dataSourceProvider.DataSource;
				if(dataSourceProvider.SuppressedSeries != null) {
					foreach(string suppressedSeries in dataSourceProvider.SuppressedSeries) {
						if(chartControl.Series[suppressedSeries] != null) {
							chartControl.Series[suppressedSeries].Visible = false;
						}
					}
				}
				#endregion
				MemoryStream ms = new MemoryStream();
				chartControl.ExportToImage(ms, ImageFormat.Bmp);
				result = Image.FromStream(ms);
				sparklineImagesCache.Add(dataSourceProviderHashCode, result);
			}
			return result;
		}
		public override IVisualBrick GetBrick(PrintCellHelperInfo info) {
			IImageBrick brick = base.CreateImageBrick(info, BrickStyle.CreateDefault());
			brick.Sides = BorderSide.None;
			ISparklineProvider sparklineProvider = info.EditValue as ISparklineProvider;
			brick.Image = GetSparklineImage(sparklineProvider, info.Rectangle.Width, info.Rectangle.Height);
			return brick;
		}
		public override string EditorTypeName { get { return EditorName; } }
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				sparklineImagesCache.Clear();
			}
		}
	}
}
