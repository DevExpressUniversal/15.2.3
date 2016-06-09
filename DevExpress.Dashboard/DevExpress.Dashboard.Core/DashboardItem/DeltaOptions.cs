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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public enum DeltaValueType { ActualValue, AbsoluteVariation, PercentVariation, PercentOfTarget }
	public enum DeltaIndicationMode { GreaterIsGood, LessIsGood, WarningIfGreater, WarningIfLess, NoIndication }
	public enum DeltaIndicationThresholdType { Absolute, Percent }
	public class DeltaOptions {
		const string xmlDeltaOptions = "DeltaOptions";
		const string xmlValueType = "ValueType";
		const string xmlResultIndicationMode = "ResultIndicationMode";
		const string xmlResultIndicationThresholdType = "ResultIndicationThresholdType";
		const string xmlResultIndicationThreshold = "ResultIndicationThreshold";
		const DeltaValueType DefaultValueType = DeltaValueType.AbsoluteVariation;
		const DeltaIndicationMode DefaultResultIndicationMode = DeltaIndicationMode.GreaterIsGood;
		const DeltaIndicationThresholdType DefaultResultIndicationThresholdType = DeltaIndicationThresholdType.Percent;
		const decimal DefaultResultIndicationThreshold = 0m;
		const string DefaultResultIndicationThresholdString = "0";
		readonly IChangeService changeService;
		DeltaValueType valueType = DefaultValueType;
		DeltaIndicationMode resultIndicationMode = DefaultResultIndicationMode;		
		DeltaIndicationThresholdType resultIndicationThresholdType = DefaultResultIndicationThresholdType;
		decimal resultIndicationThreshold = DefaultResultIndicationThreshold;		
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaOptionsValueType"),
#endif
		DefaultValue(DefaultValueType)
		]
		public DeltaValueType ValueType {
			get { return valueType; }
			set {
				if (valueType != value) {
					valueType = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaOptionsResultIndicationMode"),
#endif
		DefaultValue(DefaultResultIndicationMode)
		]
		public DeltaIndicationMode ResultIndicationMode {
			get { return resultIndicationMode; }
			set {
				if (value != resultIndicationMode) {
					resultIndicationMode = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaOptionsResultIndicationThresholdType"),
#endif
		DefaultValue(DefaultResultIndicationThresholdType)
		]
		public DeltaIndicationThresholdType ResultIndicationThresholdType {
			get { return resultIndicationThresholdType; }
			set {
				if (value != resultIndicationThresholdType) {
					resultIndicationThresholdType = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DeltaOptionsResultIndicationThreshold"),
#endif
		DefaultValue(typeof(decimal), DefaultResultIndicationThresholdString)
		]
		public decimal ResultIndicationThreshold {
			get { return resultIndicationThreshold; }
			set {
				if (value != resultIndicationThreshold) {
					resultIndicationThreshold = value;
					OnChanged();
				}
			}
		}
		internal DeltaValueType SubValue1DisplayType {
			get { return valueType == DeltaValueType.ActualValue ? DeltaValueType.AbsoluteVariation : DeltaValueType.ActualValue; }
		}
		internal DeltaValueType SubValue2DisplayType {
			get { return (valueType == DeltaValueType.ActualValue || valueType == DeltaValueType.AbsoluteVariation) ? DeltaValueType.PercentVariation : DeltaValueType.AbsoluteVariation; }
		}
		internal bool Changed {
			get {
				return valueType != DefaultValueType || resultIndicationMode != DefaultResultIndicationMode ||
					   resultIndicationThresholdType != DefaultResultIndicationThresholdType || resultIndicationThreshold != DefaultResultIndicationThreshold;
			}
		}
		internal DeltaOptions()
			: this(null) {
		}
		internal DeltaOptions(IChangeService changeService) {
			this.changeService = changeService;
		}
		internal DeltaOptions Clone() {
			DeltaOptions clone = new DeltaOptions();
			clone.Assign(this);
			return clone;
		}
		internal void Assign(DeltaOptions deltaOptions) {
			try {
				valueType = deltaOptions.ValueType;
				resultIndicationMode = deltaOptions.resultIndicationMode;
				resultIndicationThresholdType = deltaOptions.resultIndicationThresholdType;
				resultIndicationThreshold = deltaOptions.resultIndicationThreshold;
			}
			finally {
				OnChanged();
			}			
		}
		internal bool Compare(DeltaOptions deltaOptions) {
			return deltaOptions != null && deltaOptions.valueType == valueType && deltaOptions.resultIndicationMode == resultIndicationMode &&
				deltaOptions.resultIndicationThresholdType == resultIndicationThresholdType && deltaOptions.resultIndicationThreshold == resultIndicationThreshold;
		}
		internal void SaveToXml(XElement parentElement) {
			if(ShouldSerialize()) {
				XElement element = new XElement(xmlDeltaOptions);
				if(valueType != DefaultValueType)
					element.Add(new XAttribute(xmlValueType, valueType));
				if(resultIndicationMode != DefaultResultIndicationMode)
					element.Add(new XAttribute(xmlResultIndicationMode, resultIndicationMode));
				if(resultIndicationThresholdType != DefaultResultIndicationThresholdType)
					element.Add(new XAttribute(xmlResultIndicationThresholdType, resultIndicationThresholdType));
				if(resultIndicationThreshold != DefaultResultIndicationThreshold)
					element.Add(new XAttribute(xmlResultIndicationThreshold, resultIndicationThreshold));
				parentElement.Add(element);
			}
		}
		internal void LoadFromXml(XElement parentElement) {
			XElement element = parentElement.Element(xmlDeltaOptions);
			if (element != null) {
				string valueTypeAttr = XmlHelper.GetAttributeValue(element, xmlValueType);
				if (!String.IsNullOrEmpty(valueTypeAttr))
					valueType = XmlHelper.EnumFromString<DeltaValueType>(valueTypeAttr);
				string resultIndicationModeAttr = XmlHelper.GetAttributeValue(element, xmlResultIndicationMode);
				if (!String.IsNullOrEmpty(resultIndicationModeAttr))
					resultIndicationMode = XmlHelper.EnumFromString<DeltaIndicationMode>(resultIndicationModeAttr);
				string resultIndicationThresholdTypeAttr = XmlHelper.GetAttributeValue(element, xmlResultIndicationThresholdType);
				if (!String.IsNullOrEmpty(resultIndicationThresholdTypeAttr))
					resultIndicationThresholdType = XmlHelper.EnumFromString<DeltaIndicationThresholdType>(resultIndicationThresholdTypeAttr);
				string resultIndicationThresholdAttr = XmlHelper.GetAttributeValue(element, xmlResultIndicationThreshold);
				if (!String.IsNullOrEmpty(resultIndicationThresholdAttr))
					resultIndicationThreshold = XmlHelper.FromString<decimal>(resultIndicationThresholdAttr);
			}
		}
		void OnChanged() {
			if(changeService != null)
				changeService.OnChanged(new ChangedEventArgs(ChangeReason.ClientData, this, null));
		}
		bool ShouldSerialize() {
			return valueType != DefaultValueType || resultIndicationMode != DefaultResultIndicationMode || resultIndicationThresholdType != DefaultResultIndicationThresholdType ||
				resultIndicationThreshold != DefaultResultIndicationThreshold;
		}
	}
}
