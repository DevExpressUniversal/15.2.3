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
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public enum SparklineViewType { Line, Area, Bar, WinLoss }
	public class SparklineOptions {
		const string xmlSparklineOptions = "SparklineOptions";
		const string xmlViewType = "ViewType";
		const string xmlHighlightMinMaxPoints = "HighlightMinMaxPoints";
		const string xmlHighlightStartEndPoints = "HighlightStartEndPoints";
		const SparklineViewType DefaultViewType = SparklineViewType.Line;
		const bool DefaultHighlightMinMaxPoints = true;
		const bool DefaultHighlightStartEndPoints = true;
		readonly IChangeService changeService;
		SparklineViewType viewType = DefaultViewType;
		bool highlightMinMaxPoints = DefaultHighlightMinMaxPoints;
		bool highlightStartEndPoints = DefaultHighlightStartEndPoints;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SparklineOptionsViewType"),
#endif
		DefaultValue(DefaultViewType)
		]
		public SparklineViewType ViewType {
			get { return viewType; }
			set {
				if(viewType != value) {
					viewType = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SparklineOptionsHighlightMinMaxPoints"),
#endif
		DefaultValue(DefaultHighlightMinMaxPoints)
		]
		public bool HighlightMinMaxPoints {
			get { return highlightMinMaxPoints; }
			set {
				if(highlightMinMaxPoints != value) {
					highlightMinMaxPoints = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SparklineOptionsHighlightStartEndPoints"),
#endif
		DefaultValue(DefaultHighlightStartEndPoints)
		]
		public bool HighlightStartEndPoints {
			get { return highlightStartEndPoints; }
			set {
				if(highlightStartEndPoints != value) {
					highlightStartEndPoints = value;
					OnChanged();
				}
			}
		}
		internal SparklineOptions()
			: this(null) {
		}
		internal SparklineOptions(IChangeService changeService) {
			this.changeService = changeService;
		}
		internal void SaveToXml(XElement parentElement) {
			if(ShouldSerialize()) {
				XElement element = new XElement(xmlSparklineOptions);
				if(viewType != DefaultViewType)
					element.Add(new XAttribute(xmlViewType, viewType));
				if(highlightMinMaxPoints != DefaultHighlightMinMaxPoints)
					element.Add(new XAttribute(xmlHighlightMinMaxPoints, highlightMinMaxPoints));
				if(highlightStartEndPoints != DefaultHighlightStartEndPoints)
					element.Add(new XAttribute(xmlHighlightStartEndPoints, highlightStartEndPoints));
				parentElement.Add(element);
			}
		}
		internal void LoadFromXml(XElement parentElement) {
			XElement element = parentElement.Element(xmlSparklineOptions);
			if(element != null) {
				string viewTypeAttr = element.GetAttributeValue(xmlViewType);
				if(!String.IsNullOrEmpty(viewTypeAttr))
					viewType = XmlHelper.EnumFromString<SparklineViewType>(viewTypeAttr);
				string highlightMinMaxPointsAttr = element.GetAttributeValue(xmlHighlightMinMaxPoints);
				if(!String.IsNullOrEmpty(highlightMinMaxPointsAttr))
					highlightMinMaxPoints = XmlHelper.FromString<bool>(highlightMinMaxPointsAttr);
				string highlightStartEndPointsAttr = element.GetAttributeValue(xmlHighlightStartEndPoints);
				if(!String.IsNullOrEmpty(highlightStartEndPointsAttr))
					highlightStartEndPoints = XmlHelper.FromString<bool>(highlightStartEndPointsAttr);
			}
		}
		internal SparklineOptions Clone() {
			SparklineOptions options = new SparklineOptions();
			options.Assign(this);
			return options;
		}
		internal void Assign(SparklineOptions options) {
			try {
				viewType = options.ViewType;
				highlightMinMaxPoints = options.HighlightMinMaxPoints;
				highlightStartEndPoints = options.HighlightStartEndPoints;
			}
			finally {
				OnChanged();
			}
		}
		void OnChanged() {
			if(changeService != null)
				changeService.OnChanged(new ChangedEventArgs(ChangeReason.View, this, null));
		}
		bool ShouldSerialize() {
			return viewType != DefaultViewType || highlightMinMaxPoints != DefaultHighlightMinMaxPoints || highlightStartEndPoints != DefaultHighlightStartEndPoints;
		}
	}
}
