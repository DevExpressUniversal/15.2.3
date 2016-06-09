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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardSparklineInfo : IDashboardSparklineInfo, IDisposable {
		readonly ISparklineSettings settings;
		readonly ISparklineData data;
		readonly Rectangle bounds;
		readonly ISkinProvider skinProvider;
		IGraphicsCache cache;
		Graphics graphics;
		ISparklineSettings IDashboardSparklineInfo.Settings { get { return settings; } }
		SparklineViewBase IDashboardSparklineInfo.View { get { return settings.View; ; } }
		ISparklineData IDashboardSparklineInfo.Data { get { return data; } }
		Rectangle IDashboardSparklineInfo.Bounds { get { return bounds; } }
		Graphics IDashboardSparklineInfo.Graphics { get { return graphics; } }
		IGraphicsCache IDashboardSparklineInfo.Cache { get { return cache; } }
		ISkinProvider IDashboardSparklineInfo.SkinProvider { get { return skinProvider; } }
		public DashboardSparklineInfo(SparklineOptionsViewModel viewModel, IList<double> value, Rectangle bounds, GraphicsCache cache, ISkinProvider skinProvider) {
			settings = new SparklineSettingsWrapper(viewModel);
			data = new SparklineDataWrapper(value);
			this.bounds = bounds;
			this.cache = cache;
			graphics = cache.Graphics;
			this.skinProvider = skinProvider ?? new DashboardSparklineDefaultSkinProviderWrapper();
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(graphics != null) {
					graphics.Dispose();
					graphics = null;
				}
				if(cache != null) {
					cache.Dispose();
					cache = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	public class SparklineDataWrapper : ISparklineData {
		readonly IList<double> value;
		IList<double> ISparklineData.Values { get { return value; } }
		public SparklineDataWrapper(IList<double> value) {
			this.value = new List<double>(value);
		}
	}
	public class SparklineSettingsWrapper : ISparklineSettings {
		readonly Padding padding;
		readonly SparklineViewBase view;
		readonly SparklineRange valueRange;
		Padding ISparklineSettings.Padding { get { return padding; } }
		public SparklineViewBase View { get { return view; } }
		SparklineRange ISparklineSettings.ValueRange { get { return valueRange; } }
		public SparklineSettingsWrapper(SparklineOptionsViewModel viewModel) {
			padding = new Padding(1);
			valueRange = new SparklineRange();
			switch(viewModel.ViewType) {
				case DashboardCommon.SparklineViewType.Area:
					view = new AreaSparklineView();
					break;
				case DashboardCommon.SparklineViewType.Bar:
					view = new BarSparklineView();
					break;
				case DashboardCommon.SparklineViewType.WinLoss:
					view = new WinLossSparklineView();
					break;
				default:
					view = new LineSparklineView();
					break;
			}
			view.HighlightMinPoint = view.HighlightMaxPoint = viewModel.HighlightMinMaxPoints;
			view.HighlightStartPoint = view.HighlightEndPoint = viewModel.HighlightStartEndPoints;
		}
	}
	public class DashboardSparklineDefaultSkinProviderWrapper : ISkinProvider {
		const string skinName = @"DevExpress Style";
		string ISkinProvider.SkinName { get { return skinName; } }
	}
}
