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

using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGauges.Core.Drawing {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class GraphicsProperties {
		TextRenderingHint textRenderingHintCore;
		CompositingQuality compositingQualityCore;
		SmoothingMode smoothingModeCore;
		InterpolationMode interpolationModeCore;
		public GraphicsProperties() {
			textRenderingHintCore = TextRenderingHint.AntiAlias;
			interpolationModeCore = InterpolationMode.HighQualityBilinear;
			smoothingModeCore = SmoothingMode.HighQuality;
			compositingQualityCore = CompositingQuality.HighQuality;
		}
		[DefaultValue(TextRenderingHint.AntiAlias), NotifyParentProperty(true)]
		public TextRenderingHint TextRenderingHint {
			get { return textRenderingHintCore; }
			set {
				if(value == textRenderingHintCore) return;
				textRenderingHintCore = value;
				RaiseGraphicsPropertiesChanged();
			}
		}
		[TypeConverter("DevExpress.XtraGauges.Core.Drawing.InterpolationModePropertyConverter," + AssemblyInfo.SRAssemblyGaugesCore)]
		[DefaultValue(InterpolationMode.HighQualityBilinear), NotifyParentProperty(true)]
		public InterpolationMode InterpolationMode {
			get { return interpolationModeCore; }
			set {
				if(value == interpolationModeCore) return;
				interpolationModeCore = value;
				RaiseGraphicsPropertiesChanged();
			}
		}
		[TypeConverter("DevExpress.XtraGauges.Core.Drawing.SmoothingModePropertyConverter," + AssemblyInfo.SRAssemblyGaugesCore)]
		[DefaultValue(SmoothingMode.HighQuality), NotifyParentProperty(true)]
		public SmoothingMode SmoothingMode {
			get { return smoothingModeCore; }
			set {
				if(value == smoothingModeCore) return;
				smoothingModeCore = value;
				RaiseGraphicsPropertiesChanged();
			}
		}
		[TypeConverter("DevExpress.XtraGauges.Core.Drawing.CompositingQualityPropertyConverter," + AssemblyInfo.SRAssemblyGaugesCore)]
		[DefaultValue(CompositingQuality.HighQuality), NotifyParentProperty(true)]
		public CompositingQuality CompositingQuality {
			get { return compositingQualityCore; }
			set {
				if(value == compositingQualityCore) return;
				compositingQualityCore = value;
				RaiseGraphicsPropertiesChanged();
			}
		}
		public bool ShouldSerialize() {
			return (CompositingQuality != CompositingQuality.HighQuality ||
					SmoothingMode != SmoothingMode.HighQuality ||
					InterpolationMode != InterpolationMode.HighQualityBilinear ||
					TextRenderingHint != TextRenderingHint.AntiAlias);
		}
		public void Reset() {
			CompositingQuality = CompositingQuality.HighQuality;
			SmoothingMode = SmoothingMode.HighQuality;
			InterpolationMode = InterpolationMode.HighQualityBilinear;
			TextRenderingHint = TextRenderingHint.AntiAlias;
		}
		public event EventHandler GraphicsPropertiesChanged;
		void RaiseGraphicsPropertiesChanged() {
			if(GraphicsPropertiesChanged != null)
				GraphicsPropertiesChanged(this, EventArgs.Empty);
		}
		public override string ToString() {
			return DevExpress.Utils.Controls.OptionsHelper.GetObjectText(this, true);
		}
	}
	public class CompositingQualityPropertyConverter : GraphicsPropertiesConverter<CompositingQuality> { }
	public class SmoothingModePropertyConverter : GraphicsPropertiesConverter<SmoothingMode> { }
	public class InterpolationModePropertyConverter : GraphicsPropertiesConverter<InterpolationMode> { }
	public class GraphicsPropertiesConverter<T> : DevExpress.Utils.Design.EnumTypeConverter {
		public GraphicsPropertiesConverter() : base(typeof(T)) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			if(context != null && context.Instance is GraphicsProperties) {
				List<T> standardCollection = new List<T>();
				foreach(T item in Enum.GetValues(typeof(T))) {
					if(item.ToString().Contains("Invalid")) continue;
					standardCollection.Add(item);
				}
				return new StandardValuesCollection(standardCollection);
			}
			return base.GetStandardValues(context);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return value.ToString();
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
