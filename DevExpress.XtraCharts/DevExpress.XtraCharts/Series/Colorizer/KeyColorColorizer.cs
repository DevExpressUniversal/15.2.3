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

using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraCharts {
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					 "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(KeyColorColorizerTypeConverter))
	]
	public class KeyColorColorizer : ChartPaletteColorizerBase, IPatternHolder {
		const string DefaultLegendPattern = "{V}";
		readonly KeyCollection keys;
		readonly Dictionary<object, int> indexList;
		int curentIndex = 0;
		IColorizerKeyProvider keyProvider;
		string legendPattern;
		string ActualLegendPattern { get { return !string.IsNullOrEmpty(legendPattern) ? legendPattern : DefaultLegendPattern; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("KeyColorColorizerKeys"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.KeyColorColorizer.Keys"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Editor("DevExpress.XtraCharts.Design.KeyCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor))
		]
		public KeyCollection Keys { get { return keys; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("KeyColorColorizerKeyProvider"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.KeyColorColorizer.KeyProvider"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DefaultValue(null),
		Browsable(false)
		]
		public IColorizerKeyProvider KeyProvider {
			get { return keyProvider; }
			set {
				if (keyProvider != value) {
					RaisePropertyChanging();
					keyProvider = value;
					RaisePropertyChanged("KeyProvider");
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("KeyColorColorizerLegendItemPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.KeyColorColorizer.LegendItemPattern"),
		Editor("DevExpress.XtraCharts.Design.KeyColorColorizerLegendItemPatternEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty]
		public string LegendItemPattern {
			get { return legendPattern; }
			set {
				if (value != legendPattern) {
					RaisePropertyChanging();
					legendPattern = value;
					RaisePropertyChanged("LegendItemPattern");
				}
			}
		}
		public KeyColorColorizer() {
			indexList = new Dictionary<object, int>();
			keys = new KeyCollection();
			keys.CollectionChanged += KeysChanged;
			keys.CollectionChanging += KeysChanging;
		}
		#region IPatternHolder
		PatternDataProvider IPatternHolder.GetDataProvider(PatternConstants patternConstant) {
			return new KeyColorColorizerLegendItemDataProvider();
		}
		string IPatternHolder.PointPattern { get { return ActualLegendPattern; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeKeys() {
			return Keys.Count > 0;
		}
		bool ShouldSerializeLegendItemPattern() {
			return !String.IsNullOrEmpty(legendPattern);
		}
		void ResetLegendItemPattern() {
			LegendItemPattern = string.Empty;
		}
		internal bool ShouldSerialize() {
			return
						ShouldSerializeLegendItemPattern() ||
						ShouldSerializeKeys();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "LegendItemPattern":
					return ShouldSerializeLegendItemPattern();
				case "Keys":
					return ShouldSerializeKeys();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		void KeysChanging(object sender, CollectionChangingEventArgs<object> e) {
			RaisePropertyChanging();
		}
		void KeysChanged(object sender, CollectionChangedEventArgs<object> e) {
			RaisePropertyChanged("Keys");
		}
		string GetText(object value) {
			PatternParser patternParser = new PatternParser(ActualLegendPattern, this);
			patternParser.SetContext(value.ToString());
			return patternParser.GetText();
		}
		protected override List<LegendItem> GetLegendItems(Palette palette, bool textVisible, Color textColor, Font legendFont, bool markerVisible, Size markerSize) {
			List<LegendItem> items = new List<LegendItem>();
			if (palette.Count == 0 || (keys.Count == 0 && indexList.Keys.Count == 0))
				return items;
			if (keys.Count > 0) {
				for (int i = 0; i < keys.Count; i++)
					items.Add(new LegendItem(GetText(keys[i]), palette[i % palette.Count].Color, textColor, markerSize, markerVisible, textVisible));
			}
			else {
				foreach (object key in indexList.Keys)
					items.Add(new LegendItem(GetText(key), palette[indexList[key] % palette.Count].Color, textColor, markerSize, markerVisible, textVisible));
			}
			return items;
		}
		internal string[] GetAvailablePatternPlaceholders() {
			return new string[1] { ToolTipPatternUtils.ValuePattern };
		}
		public override Color GetPointColor(object argument, object[] values, object colorKey, Palette palette) {
			int index = -1;
			object key = null;
			if (keyProvider != null)
				key = keyProvider.GetKey(colorKey);
			else
				key = colorKey;
			if (keys.Count > 0) {
				index = keys.IndexOf(key);
				if (index < 0)
					return Color.Empty;
				return palette[index].Color;
			}
			if (!indexList.ContainsKey(key)) {
				indexList.Add(key, curentIndex);
				curentIndex++;
			}
			index = indexList[key] % palette.Count;
			return palette[index].Color;
		}
	}
	public class KeyCollection : NotificationCollection<object> { }
}
