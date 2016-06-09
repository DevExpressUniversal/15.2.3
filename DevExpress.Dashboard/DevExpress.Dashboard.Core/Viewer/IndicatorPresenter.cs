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

using DevExpress.XtraGauges.Core.Drawing;
using System.Drawing;
using DevExpress.DashboardCommon.Data;
using System;
using System.Collections.Generic;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Win.Gauges.State;
using DevExpress.XtraGauges.Base;
namespace DevExpress.DashboardCommon.Viewer {
	public enum ResultType { 
		Warning, 
		Good, 
		Bad
	}
	public class IndicatorPresenter {
		internal class CustomShader : BaseColorShader {
			readonly Color goodResultColor;
			readonly Color badResultColor;
			readonly Color warningResultColor;
			ResultType resultType = ResultType.Warning;
			public ResultType ResultType { get { return resultType; } set { resultType = value; } }
			public CustomShader(Color goodResultColor, Color badResultColor, Color warningResultColor) {
				this.goodResultColor = goodResultColor;
				this.badResultColor = badResultColor;
				this.warningResultColor = warningResultColor;
			}
			protected override void Assign(string shaderData) {
			}
			protected override string GetShaderDataTag() {
				return string.Empty;
			}
			protected override string GetShaderTypeTag() {
				return "Custom";
			}
			protected override void ProcessCore(ref Color sourceColor) {
				switch(ResultType) {
					case ResultType.Good:
						sourceColor = goodResultColor;
						break;
					case ResultType.Bad:
						sourceColor = badResultColor;
						break;
					case ResultType.Warning:
						sourceColor = warningResultColor;
						break;
				}
			}
			protected override DevExpress.XtraGauges.Core.Base.BaseObject CloneCore() {
				return new CustomShader(goodResultColor, badResultColor, warningResultColor) { ResultType = resultType };
			}
		}
		readonly static Dictionary<StateIndicatorShapeType, IndicatorType> indicatorTypes = new Dictionary<StateIndicatorShapeType, IndicatorType>();
		static IndicatorPresenter() {
			indicatorTypes.Add(DashboardIndicators.TrendUp, IndicatorType.UpArrow);
			indicatorTypes.Add(DashboardIndicators.TrendDown, IndicatorType.DownArrow);
			indicatorTypes.Add(DashboardIndicators.Warning, IndicatorType.Warning);
		}
		readonly GaugeContainer gaugeContainer = new GaugeContainer();
		readonly ImageCache imageCache = new ImageCache();
#if DEBUGTEST
		internal ImageCache ImageCache { get { return imageCache; } }
#endif
		public IndicatorPresenter(Color goodResultColor, Color badResultColor, Color warningResultColor) {
			StateIndicatorComponent stateIndicatorComponent = new StateIndicatorComponent("stateIndicatorComponent");
			stateIndicatorComponent.Shader = new CustomShader(goodResultColor, badResultColor, warningResultColor);
			stateIndicatorComponent.Size = new SizeF(200F, 200F);
			foreach(KeyValuePair<StateIndicatorShapeType, IndicatorType> pair in indicatorTypes) {
				stateIndicatorComponent.States.Add(new IndicatorState(pair.Key));
			}
			stateIndicatorComponent.StateIndex = 0;
			StateIndicatorGauge stateIndicatorGauge = new StateIndicatorGauge();
			stateIndicatorGauge.Indicators.AddRange(new StateIndicatorComponent[] {
				stateIndicatorComponent
			});
			gaugeContainer.Size = new Size(200, 200);
			gaugeContainer.Gauges.AddRange(new IGauge[] {
				stateIndicatorGauge
			});
		}
		void CreateCache(Size size, ResultType resultType) {
			StateIndicatorGauge stateIndicatorGauge = (StateIndicatorGauge)gaugeContainer.Gauges[0];
			StateIndicatorComponent stateIndicatorComponent = stateIndicatorGauge.Indicators[0];
			for(int i = 0; i < stateIndicatorComponent.States.Count; i++) {
				IIndicatorState indicatorState = stateIndicatorComponent.States[i];
				IndicatorType indicatorType = indicatorTypes[indicatorState.ShapeType];
				((CustomShader)stateIndicatorComponent.Shader).ResultType = resultType;
				stateIndicatorComponent.StateIndex = i;
				imageCache[indicatorType, resultType] = gaugeContainer.GetImage(size.Width, size.Height);
			}
		}
		public Image GetImage(Size size, IndicatorType type, bool isGood) {
			ResultType resultType = isGood ? ResultType.Good : ResultType.Bad;
			if(type == IndicatorType.Warning) {
				size.Width = size.Height = Math.Min(size.Width, size.Height);
				resultType = ResultType.Warning;
			}
			if(imageCache[type, resultType] == null || imageCache[type, resultType].Size != size)
				CreateCache(size, resultType);
			return imageCache[type, resultType];
		}
	}
	internal class ImageCache {
		readonly Image[,] imageCache;
		public Image this[IndicatorType type, ResultType resultType] {
			get {
				int index = (int)type;
				if(index < 0 || index >= imageCache.GetLength(0)) return null;
				return imageCache[index, resultType == ResultType.Good ? 1 : 0];
			}
			set {
				int index = (int)type;
				if(index < 0 || index >= imageCache.GetLength(0)) return;
				imageCache[index, resultType == ResultType.Good ? 1 : 0] = value;
			}
		}
		public ImageCache() {
			int count = Enum.GetValues(typeof(IndicatorType)).Length;
			imageCache = new Image[count, 2];
			for(int i = 0; i < count; i++)
				imageCache[i, 0] = imageCache[i, 1] = null;
		}
	}
}
