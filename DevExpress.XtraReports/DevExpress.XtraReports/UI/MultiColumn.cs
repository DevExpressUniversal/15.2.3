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

using System;
using System.ComponentModel;
using DevExpress.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn"),
	]
	public class MultiColumn : IXRSerializable {
		const int DefaultColumnCount = 1;
		const float DefaultColumnWidth = 0.0f;
		const float DefaultColumnSpacing = 0.0f;
		const ColumnLayout DefaultLayout = ColumnLayout.DownThenAcross;
		const MultiColumnMode DefaultMode = MultiColumnMode.None;
		private DetailBand detailBand;
		private int columnCount = DefaultColumnCount;
		private float columnWidth;
		private float columnSpacing;
		private ColumnLayout layout = ColumnLayout.DownThenAcross;
		private MultiColumnMode mode = MultiColumnMode.None;
		private bool ReportIsLoading { get { return detailBand != null && detailBand.ReportIsLoading; } }
		internal bool IsMultiColumn {
			get {
				return
					(mode == MultiColumnMode.UseColumnCount && columnCount > 1) ||
					(mode == MultiColumnMode.UseColumnWidth && columnWidth > 0);
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("MultiColumnColumnCount"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn.ColumnCount"),
		DefaultValue(DefaultColumnCount),
		RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(value <= 0)
					return;
				columnCount = value;
				if(!ReportIsLoading)
					mode = MultiColumnMode.UseColumnCount;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("MultiColumnColumnWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn.ColumnWidth"),
		DefaultValue(DefaultColumnWidth),
		RefreshProperties(RefreshProperties.All),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public float ColumnWidth {
			get { return columnWidth; }
			set {
				if(value < 0)
					return;
				columnWidth = value;
				if(!ReportIsLoading)
					mode = MultiColumnMode.UseColumnWidth;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("MultiColumnColumnSpacing"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn.ColumnSpacing"),
		DefaultValue(DefaultColumnSpacing),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public float ColumnSpacing {
			get { return columnSpacing; }
			set {
				if(value < 0)
					return;
				columnSpacing = value;
			}
		}
		[Obsolete("This property is now obsolete. Use the Layout property instead.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ColumnDirection Direction {
			get {
				return layout == ColumnLayout.AcrossThenDown ? ColumnDirection.AcrossThenDown : ColumnDirection.DownThenAcross;
			}
			set {
				switch(value) {
					case ColumnDirection.AcrossThenDown:
						layout = ColumnLayout.AcrossThenDown;
						break;
					case ColumnDirection.DownThenAcross:
						layout = ColumnLayout.DownThenAcross;
						break;
					default:
						throw new ArgumentException();
				}
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("MultiColumnLayout"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn.Layout"),
		DefaultValue(DefaultLayout),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public ColumnLayout Layout {
			get {
				return layout;
			}
			set {
				layout = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("MultiColumnMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.MultiColumn.Mode"),
		DefaultValue(DefaultMode),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public MultiColumnMode Mode { get { return mode; } set { mode = value; } }
		internal MultiColumn(DetailBand detailBand) {
			this.detailBand = detailBand;
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeInteger("ColumnCount", columnCount);
			serializer.SerializeEnum("Layout", layout);
			serializer.SerializeSingle("ColumnWidth", columnWidth);
			serializer.SerializeSingle("ColumnSpacing", columnSpacing);
			serializer.SerializeEnum("Mode", mode);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			columnCount = serializer.DeserializeInteger("ColumnCount", 1);
			layout = (ColumnLayout)serializer.DeserializeEnum("Direction", typeof(ColumnLayout), ColumnLayout.DownThenAcross);
			columnWidth = serializer.DeserializeSingle("ColumnWidth", 0);
			columnSpacing = serializer.DeserializeSingle("ColumnSpacing", 0);
			mode = (MultiColumnMode)serializer.DeserializeEnum("Mode", typeof(MultiColumnMode), MultiColumnMode.None);
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		#endregion
		internal void SetColumnWidth(float columnWidth) {
			this.columnWidth = columnWidth;
		}
		internal void SetColumnSpacing(float columnSpacing) {
			this.columnSpacing = columnSpacing;
		}
		internal int GetColumnCount(float usefulWidth) {
			switch(mode) {
				case MultiColumnMode.UseColumnCount:
					return columnCount;
				case MultiColumnMode.UseColumnWidth:
					if(columnWidth > 0) {
						float docColumnSpacing = GetColumnSpacingInDpi(GraphicsDpi.Document);
						float floatingCount = ((usefulWidth + docColumnSpacing) / (GetColumnWidthInDpi(GraphicsDpi.Document) + docColumnSpacing));
						int roundedCount = (int)Math.Round(floatingCount);
						int count = FloatsComparer.Default.FirstEqualsSecond(floatingCount, roundedCount) ? roundedCount : (int)floatingCount;
						if(count > 0)
							return count;
					}
					return 1;
				default:
					return 1;
			}
		}
		internal float GetColumnWidth(float usefulWidth, float dpi) {
			float result = 0;
			float columnSpacingInDpi = GetColumnSpacingInDpi(dpi);
			switch(mode) {
				case MultiColumnMode.UseColumnCount:
					result = (usefulWidth + columnSpacingInDpi) / columnCount;
					break;
				case MultiColumnMode.UseColumnWidth:
					result = GetColumnWidthInDpi(dpi) + columnSpacingInDpi;
					break;
				default:
					return 0;
			}
			return Math.Min(result, usefulWidth);
		}
		internal float GetUsefulColumnWidth(float usefulWidth, float dpi) {
			float result = GetColumnWidth(usefulWidth, dpi);
			result = Math.Max(0, result - GetColumnSpacingInDpi(dpi));
			return result;
		}
		float GetColumnWidthInDpi(float dpi) {
			return XRConvert.Convert(columnWidth, detailBand.Dpi, dpi); 
		}
		internal float GetColumnSpacingInDpi(float dpi) {
			return XRConvert.Convert(columnSpacing, detailBand.Dpi, dpi); 
		}
		internal bool ShouldSerialize() {
			return columnCount != DefaultColumnCount || columnWidth != DefaultColumnWidth || 
				columnSpacing != DefaultColumnSpacing || layout != DefaultLayout || mode != DefaultMode;
		}
	}
}
