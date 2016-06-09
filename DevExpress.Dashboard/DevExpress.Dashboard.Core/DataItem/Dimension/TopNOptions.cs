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
using System.Drawing.Design;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum DimensionTopNMode {
		Top = 0,
		Bottom = 1
	}
	public class DimensionTopNOptions {
		const string xmlTopNEnabled = "TopNEnabled";
		const string xmlTopNCount = "TopNCount";
		const string xmlTopNShowOthers = "TopNShowOthers";
		const string xmlTopNMeasure = "TopNMeasure";
		const string xmlTopNMode = "TopNMode";
		const bool DefaultEnabled = false;
		const bool DefaultShowOthers = false;
		const int DefaultCount = 5;
		const DimensionTopNMode DefaultMode = DimensionTopNMode.Top;
		internal static DimensionSortOrder GetDimensionSortOrder(DimensionTopNMode mode) {
			return mode == DimensionTopNMode.Top ? DimensionSortOrder.Descending : DimensionSortOrder.Ascending;
		}
		readonly Dimension dimension;
		readonly DataItemBox<Measure> measureBox;
		readonly Locker locker = new Locker();		
		bool enabled = DefaultEnabled;		
		bool showOthers = DefaultShowOthers;
		int count = DefaultCount;
		DimensionTopNMode mode = DefaultMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptionsEnabled"),
#endif
		DefaultValue(DefaultEnabled)
		]
		public bool Enabled {
			get { return enabled; }
			set {
				if (value != enabled) {
					enabled = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptionsShowOthers"),
#endif
		DefaultValue(DefaultShowOthers)
		]
		public bool ShowOthers {
			get { return showOthers; }
			set {
				if (value != showOthers) {
					showOthers = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptionsCount"),
#endif
		DefaultValue(DefaultCount)
		]
		public int Count {
			get { return count; }
			set {
				if (value <= 0)
					throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageIncorrectValueTopNCount));
				if (value != count) {
					count = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptionsMode"),
#endif
		DefaultValue(DefaultMode)
		]
		public DimensionTopNMode Mode {
			get { return mode; }
			set {
				if (value != mode) {
					mode = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptionsMeasure"),
#endif
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter("DevExpress.DashboardWin.Design.MeasurePropertyTypeConverter," + AssemblyInfo.SRAssemblyDashboardWinDesign),
		DefaultValue(null)
		]
		public Measure Measure { 
			get { return measureBox.DataItem; } 
			set { measureBox.DataItem = value;} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string MeasureName { get { return measureBox.UniqueName; } set { measureBox.UniqueName = value; } }
		internal int ActualCount { get { return enabled ? count : 0; } }
		internal bool ActualShowOthers { get { return enabled && showOthers; } }
		internal DimensionTopNOptions(Dimension dimension) {
			this.dimension = dimension;
			measureBox = new DataItemBox<Measure>((IDataItemRepositoryProvider)dimension, xmlTopNMeasure);
			measureBox.Changed += (sender, e) => OnChanged();
		}
		public void BeginUpdate() {
			locker.Lock();
		}
		public void EndUpdate() {
			locker.Unlock();
			OnChanged();
		}
		internal void SaveToXml(XElement element) {
			if(ShouldSerialize()) {
				if(Enabled != DefaultEnabled)
					element.Add(new XAttribute(xmlTopNEnabled, enabled));
				if(ShowOthers != DefaultShowOthers)
					element.Add(new XAttribute(xmlTopNShowOthers, showOthers));
				if(Count != DefaultCount)
					element.Add(new XAttribute(xmlTopNCount, count));
				if(Mode != DefaultMode)
					element.Add(new XAttribute(xmlTopNMode, mode));
				measureBox.SaveToXml(element);
			}
		}
		internal void LoadFromXml(XElement element) {
			string enabledAttr = XmlHelper.GetAttributeValue(element, xmlTopNEnabled);
			if (!String.IsNullOrEmpty(enabledAttr))
				enabled = XmlHelper.FromString<bool>(enabledAttr);
			string showOthersAttr = XmlHelper.GetAttributeValue(element, xmlTopNShowOthers);
			if (!String.IsNullOrEmpty(showOthersAttr))
				showOthers = XmlHelper.FromString<bool>(showOthersAttr);
			string countAttr = XmlHelper.GetAttributeValue(element, xmlTopNCount);
			if (!String.IsNullOrEmpty(countAttr))
				count = XmlHelper.FromString<int>(countAttr);
			string modeAttr = XmlHelper.GetAttributeValue(element, xmlTopNMode);
			if (!String.IsNullOrEmpty(modeAttr))
				mode = XmlHelper.EnumFromString<DimensionTopNMode>(modeAttr);
			measureBox.LoadFromXml(element);
		}
		internal void OnEndLoading() {
			measureBox.OnEndLoading();
		}
		void OnChanged() {
			if(dimension != null && !locker.IsLocked)
				dimension.OnChanged();
		}
		bool ShouldSerialize() {
			return Enabled != DefaultEnabled || ShowOthers != DefaultShowOthers || Count != DefaultCount || Mode != DefaultMode || measureBox.ShouldSerialize;
		}
		void AssignCore(DimensionTopNOptions options) {
			Enabled = options.Enabled;
			ShowOthers = options.ShowOthers;
			Count = options.Count;
			Mode = options.Mode;
		}
		internal void Assign(DimensionTopNOptions options) {
			BeginUpdate();
			try {
				AssignCore(options);
				Measure = options.Measure;
			}
			finally {
				EndUpdate();
			}
		}
		internal void WeakAssign(DimensionTopNOptions options) {
			BeginUpdate();
			try {
				AssignCore(options);
			}
			finally {
				EndUpdate();
			}
		}
	}
}
