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

extern alias Platform;
using System;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Windows;
using DevExpress.Charts.Designer.Native;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Core;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Charts.Designer {
	public sealed class ChartDesigner {
		ChartControl userChart;
		IModelItem chartModelItem;
		ChartControl designerChart;
		ChartDesignerWindow designerWindow;
		IDesignTimeProvider designTimeProvider;
		internal ChartControl UserChart { get { return userChart; } }
		internal ChartControl DesignerChart { get { return designerChart; } }
		internal bool IsDesignTime { get { return chartModelItem != null; } }
		internal IModelItem ChartModelItem { get { return chartModelItem; } }
		internal IDesignTimeProvider DesignTimeProvider { get { return designTimeProvider; } }
		internal ChartDesigner(ChartControl chartControl, IModelItem chartModelItem, IDesignTimeProvider designTimeProvider) : this(chartControl) {
			this.chartModelItem = chartModelItem;
			this.designTimeProvider = designTimeProvider;
		}
		public ChartDesigner(ChartControl chartControl) {
			this.userChart = chartControl;
			AssignChart();
			designerChart.AutoLayout = false;
		}
		void AssignDataSource() {
			designerChart.DataSource = userChart.DataSource;
			if (userChart.Diagram != null && userChart.Diagram.Series.Count > 0)
				for (int i = 0; i < userChart.Diagram.Series.Count; i++)
					designerChart.Diagram.Series[i].DataSource = userChart.Diagram.Series[i].DataSource;
			if (designerChart.Diagram != null && designerChart.Diagram.SeriesTemplate == null && !String.IsNullOrEmpty(designerChart.Diagram.SeriesDataMember))
				designerChart.Diagram.SeriesDataMember = String.Empty;
		}
		void AssignChart() {
			using (MemoryStream chartStream = new MemoryStream()) {
				userChart.SaveToStream(chartStream);
				chartStream.Position = 0;
				designerChart = new ChartControl();
				designerChart.LoadFromStream(chartStream);
				AssignDataSource();
			}
		}
		[SecurityCritical]
		internal void DesignTimeShow() {
			this.designerWindow = new ChartDesignerWindow(this, designTime: true, forceTheme: null);
			designTimeProvider.ShowDialog(designerWindow);
			this.designerWindow.SaveLayoutToRegistry();
		}
		public void Show(Window owner, Theme theme = null) {
			this.designerWindow = new ChartDesignerWindow(this, designTime: false, forceTheme: theme);
			this.designerWindow.Owner = owner;
			this.designerWindow.ShowDialog();
			this.designerWindow.SaveLayoutToRegistry();
		}
		public void Close() {
			this.designerWindow.Close();
		}
	}
}
namespace DevExpress.Charts.Designer.Native {
	public static class DesignerInDesignTimeHelper {
		public static ChartDesigner CreateDesignerForDesignTime(ChartControl chartControl, IModelItem chartModelItem, IDesignTimeProvider designTimeProvider) {
			return new ChartDesigner(chartControl, chartModelItem, designTimeProvider);
		}
		[SecurityCritical]
		public static void RunDesignerInDesignTime(ChartDesigner designer) {
			designer.DesignTimeShow();
		}
	}
	public interface IDesignTimeProvider {
		bool? ShowDialog(Window window);
		IModelItem CreateBindingItem(IModelItem item, string elementName);
	}
}
