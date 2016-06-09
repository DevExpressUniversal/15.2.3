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
using System.Linq;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Xpf.Scheduler {
	public class SchedulerColorSchema : SchedulerColorSchemaBase {
		public SchedulerColorSchema()
			: base() {
		}
		public SchedulerColorSchema(Color baseColor)
			: base(baseColor.ToArgb()) {
		}
		[ RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Color BaseColor {
			get { return ColorExtension.FromArgb(BaseColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (BaseColorValue == argbValue)
					return;
				SetBaseColor(argbValue, new ColorSchemaTransformDefault());
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color Cell {
			get { return ColorExtension.FromArgb(CellColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellColorValue == argbValue)
					return;
				CellColorValue = argbValue;
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color CellBorder {
			get { return ColorExtension.FromArgb(CellBorderColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellBorderColorValue == argbValue)
					return;
				CellBorderColorValue = argbValue;
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color CellBorderDark {
			get { return ColorExtension.FromArgb(CellBorderDarkColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellBorderDarkColorValue == argbValue)
					return;
				CellBorderDarkColorValue = argbValue;
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color CellLight {
			get { return ColorExtension.FromArgb(CellLightColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellLightColorValue == argbValue)
					return;
				CellLightColorValue = argbValue;
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color CellLightBorder {
			get { return ColorExtension.FromArgb(CellLightBorderColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellLightBorderColorValue == argbValue)
					return;
				CellLightBorderColorValue = argbValue;
			}
		}
		[ AutoFormatEnable, NotifyParentProperty(true)]
		public Color CellLightBorderDark {
			get { return ColorExtension.FromArgb(CellLightBorderDarkColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellLightBorderDarkColorValue == argbValue)
					return;
				CellLightBorderDarkColorValue = argbValue;
			}
		}
		protected override SchedulerColorSchemaBase CreateSchemaInstance() {
			return new SchedulerColorSchema();
		}
		protected internal void SetBaseColor(Color color, ColorSchemaTransformBase transform) {
			base.SetBaseColor(color.ToArgb(), transform);
		}
		public override string ToString() {
			return "ColorSchema";
		}
	}
	public class SchedulerColorSchemaCollection : SchedulerColorSchemaCollectionBase<SchedulerColorSchema> {
		protected override void InitSchemas() {
			CreateDefaultSchemas();
		}
		public SchedulerColorSchema GetSchema(Color color, int index) {
			return base.GetSchema(color.ToArgb(), index);
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value) {
			return base.Add(value);
		}
	}
	public static class ColorExtension {
		public static Color Empty {
			get { return Color.FromArgb(0, 0, 0, 0); }
		}
		public static int ToArgb(this Color color) {
			return BitConverter.ToInt32(new byte[] { color.B, color.G, color.R, color.A }, 0);
		}
		public static Color FromArgb(int argb) {
			byte[] bytes = BitConverter.GetBytes(argb);
			return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
		}
	}
}
