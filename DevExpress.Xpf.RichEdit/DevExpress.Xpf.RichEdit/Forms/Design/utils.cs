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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using System.Resources;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Design.Internal {
	#region ParagraphPropertiesBase (abstract class)
	public abstract class ParagraphPropertiesBase : IBatchUpdateHandler, IBatchUpdateable {
		#region Fields
		BatchUpdateHelper batchUpdateHelper;
		bool defferedPropertiesChanged;
		DocumentModelUnitConverter valueUnitConverter;
		DocumentUnit unitType;
		#endregion
		protected ParagraphPropertiesBase() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.valueUnitConverter = new DocumentModelUnitTwipsConverter();
			this.unitType = GetDefaultUnitType();
		}
		protected internal virtual DocumentUnit GetDefaultUnitType() {
			return DocumentUnit.Inch;
		}
		#region Properties
		public DocumentModelUnitConverter ValueUnitConverter {
			get { return valueUnitConverter; }
			set {
				if (ValueUnitConverter == value)
					return;
				valueUnitConverter = value;
				OnPropertiesChanged();
			}
		}
		public DocumentUnit UnitType {
			get { return unitType; }
			set {
				if (UnitType == value)
					return;
				unitType = value;
				OnPropertiesChanged();
			}
		}
		#endregion
		#region Events
		public event EventHandler PropertiesChanged;
		#endregion
		protected virtual void OnPropertiesChanged() {
			if (IsUpdateLocked)
				this.defferedPropertiesChanged = true;
			else
				RaisePropertiesChanged();
		}
		protected internal virtual void RaisePropertiesChanged() {
			if (PropertiesChanged != null)
				PropertiesChanged(this, EventArgs.Empty);
		}
		#region IBatchUpdateable Members
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BatchUpdateHelper BatchUpdateHelper { get { return batchUpdateHelper; } }
		public void BeginUpdate() {
			BatchUpdateHelper.BeginUpdate();
		}
		public void CancelUpdate() {
			BatchUpdateHelper.CancelUpdate();
		}
		public void EndUpdate() {
			BatchUpdateHelper.EndUpdate();
		}
		public bool IsUpdateLocked {
			get { return BatchUpdateHelper.IsUpdateLocked; }
		}
		#endregion
		#region IBatchUpdateHandler Members
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			this.defferedPropertiesChanged = false;
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (defferedPropertiesChanged)
				RaisePropertiesChanged();
		}
		#endregion
	}
	#endregion
	#region ParagraphIndentationProperties
	public class ParagraphIndentationProperties : ParagraphPropertiesBase {
		#region Fields
		ParagraphFirstLineIndent? firstLineIndentType;
		int? leftIndent;
		int? rightIndent;
		int? firstLineIndent;
		#endregion
		public ParagraphIndentationProperties() {
		}
		#region Properties
		public ParagraphFirstLineIndent? FirstLineIndentType {
			get {
				return firstLineIndentType;
			}
			set {
				if (FirstLineIndentType == value)
					return;
				firstLineIndentType = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(0)]
		public int? LeftIndent {
			get {
				return leftIndent;
			}
			set {
				if (LeftIndent == value)
					return;
				leftIndent = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(0)]
		public int? RightIndent {
			get {
				return rightIndent;
			}
			set {
				if (RightIndent == value)
					return;
				rightIndent = value;
				OnPropertiesChanged();
			}
		}
		public int? FirstLineIndent {
			get { return firstLineIndent; }
			set {
				if (FirstLineIndent == value)
					return;
				firstLineIndent = value;
				OnPropertiesChanged();
			}
		}
		#endregion
	}
	#endregion
	#region ParagraphSpacingProperties
	public class ParagraphSpacingProperties : ParagraphPropertiesBase {
		#region Fields
		int? spacingAfter;
		int? spacingBefore;
		int maxSpacing;
		ParagraphLineSpacing? lineSpacingType;
		float? lineSpacing;
		#endregion
		public ParagraphSpacingProperties() {
			this.spacingAfter = 0;
			this.spacingBefore = 0;
			this.maxSpacing = ParagraphFormDefaults.MaxIndentByDefault;
			this.lineSpacing = null;
			this.lineSpacingType = ParagraphLineSpacing.Single;
		}
		#region Properties
		[DefaultValue(0)]
		public int? SpacingAfter {
			get { return spacingAfter; }
			set {
				if (spacingAfter == value)
					return;
				spacingAfter = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(0)]
		public int? SpacingBefore {
			get { return spacingBefore; }
			set {
				if (spacingBefore == value)
					return;
				spacingBefore = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ParagraphFormDefaults.MaxSpacingByDefault)]
		public int MaxSpacing {
			get { return maxSpacing; }
			set {
				if (MaxSpacing == value)
					return;
				maxSpacing = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ParagraphLineSpacing.Single)]
		public ParagraphLineSpacing? LineSpacingType {
			get { return lineSpacingType; }
			set {
				if (LineSpacingType == value)
					return;
				lineSpacingType = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(null)]
		public float? LineSpacing {
			get { return lineSpacing; }
			set {
				if (LineSpacing == value)
					return;
				lineSpacing = value;
				OnPropertiesChanged();
			}
		}
		public int MaxLineSpacing { get { return ParagraphFormDefaults.MaxLineSpacingByDefault; } }
		public int MinLineSpacing { get { return ParagraphFormDefaults.MinLineSpacingByDefault; } }
		#endregion
		protected internal override DocumentUnit GetDefaultUnitType() {
			return DocumentUnit.Point;
		}
	}
	#endregion
	#region TabPositionEditControlProperties
	public class TabPositionEditControlProperties : ParagraphPropertiesBase {
		#region Fields
		TabFormattingInfo tabFormattingInfo;
		#endregion
		public TabPositionEditControlProperties() {
			this.tabFormattingInfo = new TabFormattingInfo();
		}
		#region Properties
		public TabFormattingInfo TabFormattingInfo {
			get {
				return tabFormattingInfo;
			}
			set {
				if (TabFormattingInfo == value)
					return;
				tabFormattingInfo.Clear();
				tabFormattingInfo.AddRange(value);
				OnPropertiesChanged();
			}
		}
		#endregion
	}
	#endregion
	#region TablePropertiesBase (abstract class)
	public abstract class TablePropertiesBase : ParagraphPropertiesBase {
		#region Fields
		const int defaultMinValue = -4000;
		const int defaultMaxValue = 4000;
		int minValue;
		int maxValue;
		bool? useDefaultValue;
		#endregion
		protected TablePropertiesBase() {
			this.minValue = defaultMinValue;
			this.maxValue = defaultMaxValue;
			this.useDefaultValue = true;
		}
		#region Properties
		public bool? UseDefaultValue {
			get {
				return useDefaultValue;
			}
			set {
				if (useDefaultValue == value)
					return;
				useDefaultValue = value;
				OnPropertiesChanged();
			}
		}
		public int MinValue {
			get {
				return minValue;
			}
			set {
				if (minValue == value)
					return;
				minValue = value;
				OnPropertiesChanged();
			}
		}
		public int MaxValue {
			get {
				return maxValue;
			}
			set {
				if (maxValue == value)
					return;
				maxValue = value;
				OnPropertiesChanged();
			}
		}
		#endregion
	}
	#endregion
	#region TableSizeProperties
	public class TableSizeProperties : TablePropertiesBase {
		#region Fields
		int? width;
		WidthUnitType? widthType;
		int valueForPercent;
		#endregion
		public TableSizeProperties() {
		}
		#region Properties
		[DefaultValue(0)]
		public int? Width {
			get {
				return width;
			}
			set {
				if (width == value)
					return;
				width = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(WidthUnitType.ModelUnits)]
		public WidthUnitType? WidthType {
			get {
				return widthType;
			}
			set {
				if (widthType == value)
					return;
				widthType = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(0)]
		public int ValueForPercent {
			get {
				return valueForPercent;
			}
			set {
				if (valueForPercent == value)
					return;
				valueForPercent = value;
				OnPropertiesChanged();
			}
		}
		#endregion
	}
	#endregion
	#region TableRowHeightProperties
	public class TableRowHeightProperties : TablePropertiesBase {
		#region Fields
		int? height;
		HeightUnitType? heightType;
		#endregion
		public TableRowHeightProperties() {
		}
		#region Properties
		[DefaultValue(0)]
		public int? Height {
			get {
				return height;
			}
			set {
				if (height == value)
					return;
				height = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(HeightUnitType.Minimum)]
		public HeightUnitType? HeightType {
			get {
				return heightType;
			}
			set {
				if (heightType == value)
					return;
				heightType = value;
				OnPropertiesChanged();
			}
		}
		#endregion
	}
	#endregion
}
