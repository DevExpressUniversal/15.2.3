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
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class RangeStopCollection : NotificationCollection<double> {
	}
	public abstract class MapScale {
		internal IChangeService ChangeService { get; set; }
		protected void OnChanged() {
			if(ChangeService != null)
				ChangeService.OnChanged(new ChangedEventArgs(ChangeReason.View, this, null));
		}
		internal abstract void SaveToXml(XElement parentElement);
		internal abstract void LoadFromXml(XElement parentElement);
	}
	public class UniformScale : MapScale {
		const string xmlLevelsCount = "LevelsCount";
		internal const int DefaultLevelsCount = 10;
		int levelsCount = DefaultLevelsCount;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("UniformScaleLevelsCount"),
#endif
		DefaultValue(DefaultLevelsCount)
		]
		public int LevelsCount {
			get { return levelsCount; }
			set {
				if (levelsCount != value) {
					levelsCount = value;
					OnChanged();
				}
			}
		}
		internal override void SaveToXml(XElement element) {
			if(levelsCount != DefaultLevelsCount)
				element.Add(new XAttribute(xmlLevelsCount, levelsCount));
		}
		internal override void LoadFromXml(XElement element) {
			string levelsCountAttr = XmlHelper.GetAttributeValue(element, xmlLevelsCount);
			if(!String.IsNullOrEmpty(levelsCountAttr))
				levelsCount = XmlHelper.FromString<int>(levelsCountAttr);
		}
	}
	public class CustomScale : MapScale {
		const string xmlRangeStop = "RangeStop";
		const string xmlPercentScale = "PercentScale";
		const bool DefaulPercentScale = true;
		bool isPercent = DefaulPercentScale;
		readonly RangeStopCollection rangeStops = new RangeStopCollection();
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CustomScaleRangeStops"),
#endif
		Editor(TypeNames.DefaultCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public RangeStopCollection RangeStops { get { return rangeStops; } }
		[
		DefaultValue(DefaulPercentScale)
		]
		public bool IsPercent {
			get { return isPercent; }
			set {
				if (isPercent != value) {
					isPercent = value;
					OnChanged();
				}
			}
		}
		public CustomScale() {
			rangeStops.CollectionChanged += (sender, e) => OnChanged();
		}
		internal override void SaveToXml(XElement element) {
			if (isPercent != DefaulPercentScale)
				element.Add(new XAttribute(xmlPercentScale, isPercent));
			foreach (double val in rangeStops)
				element.Add(new XElement(xmlRangeStop, val));
		}
		internal override void LoadFromXml(XElement element) {
			string percentScaleAttr = XmlHelper.GetAttributeValue(element, xmlPercentScale);
			if(!String.IsNullOrEmpty(percentScaleAttr))
				isPercent = XmlHelper.FromString<bool>(percentScaleAttr);
			foreach(XElement rangeStopElement in element.Elements(xmlRangeStop))
				if(rangeStopElement.Value != null)
					rangeStops.Add(XmlHelper.FromString<double>(rangeStopElement.Value));
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class MapScaleEqualityComparer : IEqualityComparer<MapScale> {
		public bool Equals(MapScale scale1, MapScale scale2) {
			UniformScale uniform1 = scale1 as UniformScale;
			UniformScale uniform2 = scale2 as UniformScale;
			CustomScale custom1 = scale1 as CustomScale;
			CustomScale custom2 = scale2 as CustomScale;
			if ((scale1 == null && scale2 == null))
				return true;
			if ((uniform1 != null && uniform2 == null) ||
				(custom1 != null && custom2 == null) ||
				(uniform1 == null && uniform2 != null) ||
				(custom1 == null && custom2 != null))
				return false;
			if (custom1 != null) {
				if (custom1.RangeStops.Count == custom2.RangeStops.Count) {
					for (int i = 0; i < custom1.RangeStops.Count; i++) {
						if (!custom1.RangeStops[i].Equals(custom2.RangeStops[i]))
							return false;
					}
					return true;
				}
			}
			if (uniform1 != null) {
				return uniform1.LevelsCount.Equals(uniform2.LevelsCount);
			}
			return false;
		}
		public int GetHashCode(MapScale scale) {
			UniformScale uniform = scale as UniformScale;
			CustomScale custom = scale as CustomScale;
			if (uniform != null)
				return uniform.LevelsCount.GetHashCode();
			else
				return custom.RangeStops.Count.GetHashCode()|custom.IsPercent.GetHashCode();;
		}
	}
}
