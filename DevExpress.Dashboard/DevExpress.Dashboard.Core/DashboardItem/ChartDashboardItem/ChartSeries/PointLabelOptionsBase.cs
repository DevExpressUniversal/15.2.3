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
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum PointLabelOrientation {
		Default = 0,
		RotateRight = 1,
		RotateLeft = 2
	}
	public enum PointLabelOverlappingMode {
		Hide = 0,
		None = 1,
		Reposition = 2
	}
	public abstract class PointLabelOptionsBase {
		const bool DefaultShowPointLabels = false;
		const PointLabelOrientation DefaultOrientation = PointLabelOrientation.Default;
		const PointLabelOverlappingMode DefaultOverlappingMode = PointLabelOverlappingMode.Hide;
		const PointLabelPosition DefaultPosition = PointLabelPosition.Outside;
		const string xmlPointLabel = "PointLabelOptions";
		const string xmlShowPointLabels = "Visible";
		const string xmlOrientation = "Orientation";
		const string xmlOverlappingMode = "OverlappingMode";
		const string xmlPosition = "Position";
		bool showPointLabels;
		PointLabelOrientation orientation;
		PointLabelOverlappingMode overlappingMode;
		PointLabelPosition position;
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowPointLabels)
		]
		public bool ShowPointLabels {
			get { return showPointLabels; }
			set {
				if(value != showPointLabels) {
					showPointLabels = value;
					OnChanged();
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultOrientation)
		]
		public PointLabelOrientation Orientation {
			get { return orientation; }
			set {
				if(value != orientation) {
					orientation = value;
					OnChanged();
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultOverlappingMode)
		]
		public PointLabelOverlappingMode OverlappingMode {
			get { return overlappingMode; }
			set {
				if(value != overlappingMode) {
					overlappingMode = value;
					OnChanged();
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultPosition)
		]
		public PointLabelPosition Position {
			get { return position; }
			set {
				if(value != position) {
					position = value;
					OnChanged();
				}
			}
		}
		protected PointLabelOptionsBase() {
			showPointLabels = DefaultShowPointLabels;
			orientation = DefaultOrientation;
			overlappingMode = DefaultOverlappingMode;
			position = DefaultPosition;
		}
		internal abstract void OnChanged();
		internal virtual PointLabelViewModel CreateViewModel() {
			return new PointLabelViewModel() {
				ShowPointLabels = showPointLabels,
				Orientation = orientation,
				OverlappingMode = overlappingMode,
				Position = Position
			};
		}
		public void Assign(PointLabelOptionsBase pointLabel) {
			AssignInternal(pointLabel);
		}
		protected virtual void AssignInternal(PointLabelOptionsBase pointLabel) {
			showPointLabels = pointLabel.ShowPointLabels;
			orientation = pointLabel.Orientation;
			overlappingMode = pointLabel.OverlappingMode;
			position = pointLabel.Position;
		}
		internal XElement SaveToXml() {
			XElement element = new XElement(xmlPointLabel);
			SaveToXmlInternal(element);
			return element;
		}
		protected virtual void SaveToXmlInternal(XElement element) {
			ChartSeries.SaveBoolPropertyToXml(element, xmlShowPointLabels, showPointLabels, DefaultShowPointLabels);
			if(orientation != DefaultOrientation)
				element.Add(new XAttribute(xmlOrientation, orientation));
			if(overlappingMode != DefaultOverlappingMode)
				element.Add(new XAttribute(xmlOverlappingMode, overlappingMode));
			if(position != DefaultPosition)
				element.Add(new XAttribute(xmlPosition, Position));
		}
		internal void LoadFromXml(XElement seriesElement) {
			XElement element = seriesElement.Element(xmlPointLabel);
			if(element != null)
				LoadFromXmlInternal(element);
		}
		protected virtual void LoadFromXmlInternal(XElement element) {
			ChartSeries.LoadBoolPropertyFromXml(element, xmlShowPointLabels, ref showPointLabels);
			string argument = XmlHelper.GetAttributeValue(element, xmlOrientation);
			if(!String.IsNullOrEmpty(argument))
				orientation = XmlHelper.EnumFromString<PointLabelOrientation>(argument);
			argument = XmlHelper.GetAttributeValue(element, xmlOverlappingMode);
			if(!String.IsNullOrEmpty(argument))
				overlappingMode = XmlHelper.EnumFromString<PointLabelOverlappingMode>(argument);
			argument = XmlHelper.GetAttributeValue(element, xmlPosition);
			if(!String.IsNullOrEmpty(argument))
				position = XmlHelper.EnumFromString<PointLabelPosition>(argument);
		}
		internal virtual bool ShouldSerialize() {
			return showPointLabels != DefaultShowPointLabels ||
			orientation != DefaultOrientation ||
			position != DefaultPosition ||
			overlappingMode != DefaultOverlappingMode;
		}
	}
}
