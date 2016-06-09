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
using System.Linq;
using DevExpress.Skins;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class DashboardSparklinePainterCore {
		readonly SparklinePaintersCache paintersCache = new SparklinePaintersCache();
		public void Draw(IDashboardSparklineInfo info) {
			SparklineViewBase view = info.View;
			SparklineAppearanceHelper.SetSparklineAppearanceProvider(view, new DashboardSparklineAppearanceProvider(info.SkinProvider));
			BaseSparklinePainter painter = paintersCache.GetPainter(view);
			painter.Initialize(info.Data, info.Settings, info.Bounds);
			painter.Draw(info.Graphics, info.Cache);
		}
	}
	public interface IDashboardSparklineInfo {
		SparklineViewBase View { get; }
		ISparklineData Data { get; }
		ISparklineSettings Settings { get; }
		Rectangle Bounds { get; }
		Graphics Graphics { get; }
		IGraphicsCache Cache { get; }
		ISkinProvider SkinProvider { get; }
	}
	public class DashboardSparklineAppearanceProvider : ISparklineAppearanceProvider {
		public Color Color { get; private set; }
		public Color EndPointColor { get; private set; }
		public Color MarkerColor { get; private set; }
		public Color MaxPointColor { get; private set; }
		public Color MinPointColor { get; private set; }
		public Color NegativePointColor { get; private set; }
		public Color StartPointColor { get; private set; }
		public DashboardSparklineAppearanceProvider(ISkinProvider skinProvider) {
			if(skinProvider != null) {
				Skin skin = SparklineSkins.GetSkin(skinProvider);
				if(skin == null)
					return;
				Color = skin.Colors.GetColor(SparklineSkins.Color);
				EndPointColor = skin.Colors.GetColor(SparklineSkins.ColorEndPoint);
				MarkerColor = skin.Colors.GetColor(SparklineSkins.ColorMarker);
				MaxPointColor = skin.Colors.GetColor(SparklineSkins.ColorMaxPoint);
				MinPointColor = skin.Colors.GetColor(SparklineSkins.ColorMinPoint);
				NegativePointColor = skin.Colors.GetColor(SparklineSkins.ColorNegativePoint);
				StartPointColor = skin.Colors.GetColor(SparklineSkins.ColorStartPoint);
			}
		}
	}
}
