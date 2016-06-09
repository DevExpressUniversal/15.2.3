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
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," +
	 AssemblyInfo.SRAssemblyChartsExtensions, "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")]
	public class SeriesNameTemplate : ChartElement {
		string beginText = String.Empty;
		string endText = String.Empty;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesNameTemplateBeginText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesNameTemplate.BeginText"),
		Localizable(true),
		XtraSerializableProperty]
		public string BeginText {
			get { return beginText; }
			set {
				if (value != beginText) {
					ControlChange(() => beginText = value);
					UpdateDataContainer();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesNameTemplateEndText"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesNameTemplate.EndText"),
		Localizable(true),
		XtraSerializableProperty]
		public string EndText {
			get { return endText; }
			set {
				if (value != endText) {
					ControlChange(() => endText = value);
					UpdateDataContainer();
				}
			}
		}
		public SeriesNameTemplate() : this(null) {
		}
		internal SeriesNameTemplate(DataContainer owner) : base(owner) {
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "BeginText")
				return ShouldSerializeBeginText();
			if (propertyName == "EndText")
				return ShouldSerializeEndText();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBeginText() {
			return BeginText != string.Empty;
		}
		void ResetBeginText() {
			BeginText = string.Empty;
		}
		bool ShouldSerializeEndText() {
			return EndText != string.Empty;
		}
		void ResetEndText() {
			EndText = string.Empty;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBeginText() ||
				ShouldSerializeEndText();
		}
		#endregion
		protected override ChartElement CreateObjectForClone() {
			return new SeriesNameTemplate(null);
		}
		protected void ControlChange(Action change) {
			SendNotification(new ElementWillChangeNotification(this));
			change();
			SendNotification(new ChartElement.ElementChangedNotification(this, new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific)));
		}
		void UpdateDataContainer() {
			DataContainer dataContainer = Owner as DataContainer;
			if (dataContainer != null)
				dataContainer.UpdateBinding(true, false);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesNameTemplate template = obj as SeriesNameTemplate;
			if (template == null)
				return;
			this.beginText = template.beginText;
			this.endText = template.endText;
		}
		public override string ToString() {
			return "(SeriesNameTemplate)";
		}
	}
}
