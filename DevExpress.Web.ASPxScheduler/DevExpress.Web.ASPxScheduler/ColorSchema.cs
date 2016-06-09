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
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Native;
namespace DevExpress.Web.ASPxScheduler {
	public class SchedulerColorSchema : SchedulerColorSchemaBase {
		public SchedulerColorSchema()
			: base() {
		}
		public SchedulerColorSchema(Color baseColor)
			: base(baseColor.ToArgb()) {
		}
		[RefreshProperties(RefreshProperties.All), NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public Color BaseColor {
			get { return Color.FromArgb(BaseColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (BaseColorValue == argbValue)
					return;
				SetBaseColor(argbValue, new ColorSchemaTransformDefault());
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color Cell {
			get { return Color.FromArgb(CellColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellColorValue == argbValue)
					return;
				CellColorValue = argbValue;
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color CellBorder {
			get { return Color.FromArgb(CellBorderColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellBorderColorValue == argbValue)
					return;
				CellBorderColorValue = argbValue;
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color CellBorderDark {
			get { return Color.FromArgb(CellBorderDarkColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellBorderDarkColorValue == argbValue)
					return;
				CellBorderDarkColorValue = argbValue;
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color CellLight {
			get { return Color.FromArgb(CellLightColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellLightColorValue == argbValue)
					return;
				CellLightColorValue = argbValue;
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color CellLightBorder {
			get { return Color.FromArgb(CellLightBorderColorValue); }
			set {
				int argbValue = value.ToArgb();
				if (CellLightBorderColorValue == argbValue)
					return;
				CellLightBorderColorValue = argbValue;
			}
		}
		[AutoFormatEnable(), NotifyParentProperty(true), XtraSerializableProperty()]
		public Color CellLightBorderDark {
			get { return Color.FromArgb(CellLightBorderDarkColorValue); }
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
		public virtual SchedulerColorSchema GetSchema(Color color, int index) {
			return base.GetSchema(color.ToArgb(), index);
		}
		[Browsable(false), DXBrowsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value) {
			return base.Add(value);
		}
		protected override void OnInsertComplete(int index, SchedulerColorSchema value) {
			base.OnInsertComplete(index, value);
		}
	}
}
