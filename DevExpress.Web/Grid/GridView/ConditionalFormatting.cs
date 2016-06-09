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
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class GridViewFormatConditionCollection : GridFormatConditionCollection {
		public GridViewFormatConditionCollection()
			: base() {
		}
		protected internal GridViewFormatConditionCollection(IWebControlObject owner, Action formatSummaryChangedHandler)
			: base(owner, formatSummaryChangedHandler) {
		}
		protected override Type[] GetKnownTypes() {
			return new Type[] { 
				typeof(GridViewFormatConditionHighlight),
				typeof(GridViewFormatConditionTopBottom),
				typeof(GridViewFormatConditionColorScale),
				typeof(GridViewFormatConditionIconSet)
			};
		}
	}
	public class GridViewFormatConditionHighlight : GridFormatConditionHighlight {
		[ DefaultValue(""), NotifyParentProperty(true), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public bool ApplyToRow { get { return ApplyToItem; } set { ApplyToItem = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[ DefaultValue(GridConditionRule.Expression), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new GridConditionRule Rule { get { return base.Rule; } set { base.Rule = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Value1 { get { return base.Value1; } set { base.Value1 = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object Value2 { get { return base.Value2; } set { base.Value2 = value; } }
		[ DefaultValue(""), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.GridFormatConditionExpressionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public new string Expression { get { return base.Expression; } set { base.Expression = value; } }
		[ DefaultValue(GridConditionHighlightFormat.LightRedFillWithDarkRedText), NotifyParentProperty(true)]
		public new GridConditionHighlightFormat Format { get { return base.Format; } set { base.Format = value; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle CellStyle { get { return (GridViewCellStyle)ItemCellStyle; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle RowStyle { get { return (GridViewRowStyle)ItemStyle; } }
		protected override AppearanceStyle CreateItemCellStyle() {
			return new GridViewCellStyle();
		}
		protected override AppearanceStyle CreateItemStyle() {
			return new GridViewRowStyle();
		}
	}
	public class GridViewFormatConditionTopBottom : GridFormatConditionTopBottom {
		[ DefaultValue(""), NotifyParentProperty(true), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[ DefaultValue(false), NotifyParentProperty(true)]
		public bool ApplyToRow { get { return ApplyToItem; } set { ApplyToItem = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[ DefaultValue(10), NotifyParentProperty(true)]
		public new decimal Threshold { get { return base.Threshold; } set { base.Threshold = value; } }
		[ DefaultValue(GridTopBottomRule.AboveAverage), NotifyParentProperty(true)]
		public new GridTopBottomRule Rule { get { return base.Rule; } set { base.Rule = value; } }
		[ DefaultValue(GridConditionHighlightFormat.LightRedFillWithDarkRedText), NotifyParentProperty(true)]
		public new GridConditionHighlightFormat Format { get { return base.Format; } set { base.Format = value; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewCellStyle CellStyle { get { return (GridViewCellStyle)ItemCellStyle; } }
		[ PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridViewRowStyle RowStyle { get { return (GridViewRowStyle)ItemStyle; } }
		protected override AppearanceStyle CreateItemCellStyle() {
			return new GridViewCellStyle();
		}
		protected override AppearanceStyle CreateItemStyle() {
			return new GridViewRowStyle();
		}
	}
	public class GridViewFormatConditionColorScale : GridFormatConditionColorScale {
		[ DefaultValue(""), NotifyParentProperty(true), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object MinimumValue { get { return base.MinimumValue; } set { base.MinimumValue = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object MaximumValue { get { return base.MaximumValue; } set { base.MaximumValue = value; } }
		[ NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), AutoFormatEnable]
		public new Color MinimumColor { get { return base.MinimumColor; } set { base.MinimumColor = value; } }
		[ NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), AutoFormatEnable]
		public new Color MiddleColor { get { return base.MiddleColor; } set { base.MiddleColor = value; } }
		[ NotifyParentProperty(true),
		TypeConverter(typeof(WebColorConverter)), DefaultValue(typeof(Color), ""), AutoFormatEnable]
		public new Color MaximumColor { get { return base.MaximumColor; } set { base.MaximumColor = value; } }
		[ DefaultValue(GridConditionColorScaleFormat.GreenYellowRed), NotifyParentProperty(true)]
		public new GridConditionColorScaleFormat Format { get { return base.Format; } set { base.Format = value; } }
	}
	public class GridViewFormatConditionIconSet : GridFormatConditionIconSet {
		[ DefaultValue(""), NotifyParentProperty(true), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool Enabled { get { return base.Enabled; } set { base.Enabled = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object MinimumValue { get { return base.MinimumValue; } set { base.MinimumValue = value; } }
		[ DefaultValue(null), NotifyParentProperty(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object MaximumValue { get { return base.MaximumValue; } set { base.MaximumValue = value; } }
		[ DefaultValue(null), NotifyParentProperty(true)]
		public new GridConditionIconSetFormat Format { get { return base.Format; } set { base.Format = value; } }
	}
}
