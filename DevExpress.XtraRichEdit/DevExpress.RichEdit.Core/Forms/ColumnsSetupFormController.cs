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
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	#region ColumnsInfoUI
	public class ColumnsInfoUI : ICloneable<ColumnsInfoUI>, ISupportsCopyFrom<ColumnsInfoUI> {
		#region Fields
		public const int MinColumnWidth = 720; 
		public const int MinSpacingWidth = 0;  
		SectionPropertiesApplyType applyType;
		SectionPropertiesApplyType availableApplyType;
		int pageWidth;
		int? columnCount;
		bool? equalColumnWidth;
		readonly List<ColumnInfoUI> columns = new List<ColumnInfoUI>();
		#endregion
		public ColumnsInfoUI() {
			this.AvailableApplyType = SectionPropertiesApplyType.CurrentSection | SectionPropertiesApplyType.SelectedSections | SectionPropertiesApplyType.WholeDocument;
		}
		#region Properties
		public SectionPropertiesApplyType AvailableApplyType { get { return availableApplyType; } set { availableApplyType = value; } }
		public SectionPropertiesApplyType ApplyType { get { return applyType; } set { applyType = value; } }
		public int PageWidth { get { return pageWidth; } set { pageWidth = value; } }
		public int? ColumnCount { get { return columnCount; } set { columnCount = value; } }
		public bool? EqualColumnWidth { get { return equalColumnWidth; } set { equalColumnWidth = value; } }
		public int MaxColumnCount { get { return PageWidth / (MinColumnWidth + MinSpacingWidth); } }  
		public List<ColumnInfoUI> Columns { get { return columns; } }
		#endregion
		public bool HasColumnsNull() {
			if (this.ColumnCount > Columns.Count)   
				return true;
			for (int i = 0; i < ColumnCount; i++) {
				if (!Columns[i].Width.HasValue)
					return true;
				if (!Columns[i].Spacing.HasValue)
					return true;
			}
			return false;
		}
		protected internal virtual bool HasColumnsInfoUINull() {
			if (!ColumnCount.HasValue)
				return true;
			if (!EqualColumnWidth.HasValue)
				return true;
			return HasColumnsNull();
		}
		public void ChangeColumnCount(int count) {
			if (count <= 0)
				return;
			count = Math.Min(count, MaxColumnCount);
			int previousCount = Columns.Count;
			bool hasColumnInfoUINull = HasColumnsInfoUINull();
			for (int i = Columns.Count; i < count; i++)
				Columns.Add(new ColumnInfoUI(i + 1));
			for (int i = Columns.Count - 1; i >= count; i--)
				Columns.RemoveAt(i);
			this.ColumnCount = count;
			if (hasColumnInfoUINull) {
				CalculateEqualColumnsOnChangeCount();
				return;
			}
			if (!equalColumnWidth.Value && previousCount > 0)
				CalculateNotEqualColumnsOnChangeCount(previousCount);
			else
				CalculateEqualColumnsOnChangeCount();
		}
		protected internal virtual void CalculateEqualColumnsOnChangeCount() {	  
			if (Columns.Count <= 0)
				return;
			int spacingValue;
			if (Columns[0].Spacing.HasValue)
				spacingValue = Columns[0].Spacing.Value;
			else
				spacingValue = MinColumnWidth;
			CalculateUniformColumnsByColumnSpacing(spacingValue);
		}
		protected internal virtual void CalculateNotEqualColumnsOnChangeCount(int previousCount) {	   
			if (Columns.Count <= 0)
				return;
			if (Columns.Count == 1)
				Columns[0].Width = pageWidth;
			int calculateCount = Math.Min(previousCount, Columns.Count);
			for (int i = 0; i < calculateCount; i++)
				Columns[i].Width = Math.Max(MinColumnWidth, Columns[i].Width.Value * previousCount / Columns.Count);
			for (int i = 0; i < calculateCount - 1; i++)
				Columns[i].Spacing = Math.Max(MinSpacingWidth, Columns[i].Spacing.Value * (previousCount - 1) / (Columns.Count - 1));
			if (calculateCount > 0)
				for (int i = calculateCount; i < Columns.Count; i++)
					Columns[i].Width = Columns[calculateCount - 1].Width.Value;
			if (calculateCount > 1)
				for (int i = calculateCount - 1; i < Columns.Count - 1; i++)
					Columns[i].Spacing = Columns[calculateCount - 2].Spacing.Value;
			DisableTheLastSpacing();
			CorrectColumns();
		}
		protected internal virtual void CorrectColumns() {	  
			if (!columnCount.HasValue || columnCount <= 0)
				return;
			int difference = -CalculateAvailableSpace();
			ColumnsDistributionCalculator calculatorWidth = new ColumnsDistributionWidthPriorityCalculator(Columns);
			ColumnsDistributionCalculator calculatorSpacing = new ColumnsDistributionSpacingPriorityCalculator(Columns);
			int sumWidth = calculatorWidth.CalculateTotal(0, Columns.Count - 1);
			int sumSpacing = calculatorSpacing.CalculateTotal(0, Columns.Count - 1);
			double partWidth = (double)sumWidth / (sumWidth + sumSpacing);
			int differenceWidth = (int)(difference * partWidth);
			int differenceSpacing = difference - differenceWidth;
			calculatorWidth.DistributeSpace(0, Columns.Count - 1, differenceWidth);
			calculatorSpacing.DistributeSpace(0, Columns.Count - 2, differenceSpacing);
		}
		protected internal virtual void DisableTheLastSpacing() {		   
			Columns[Columns.Count - 1].Spacing = 0; 
		}
		public void RecalculateColumnsByWidthAfterIndex(int index) {
			if (HasColumnsInfoUINull())
				return;
			if (this.EqualColumnWidth.Value)
				this.CalculateUniformColumnsByColumnWidth();
			else
				this.ChangeColumnsNotEqualByWidthAfterIndex(index);
		}
		public void RecalculateColumnsBySpacingAfterIndex(int index) {
			if (HasColumnsInfoUINull())
				return;
			if (this.EqualColumnWidth.Value)
				this.CalculateUniformColumnsByColumnSpacing();
			else
				this.ChangeColumnsNotEqualBySpacingAfterIndex(index);
		}
		protected internal virtual void CalculateUniformColumnsCore(int columnWidth, int columnSpacing, int restWidth, int restSpacing) {
			ColumnsDistributionCalculator calculatorWidth = new ColumnsDistributionWidthPriorityCalculator(Columns);
			ColumnsDistributionCalculator calculatorSpacing = new ColumnsDistributionSpacingPriorityCalculator(Columns);
			calculatorWidth.SetAllValues(columnWidth, restWidth);
			calculatorSpacing.SetAllValues(columnSpacing, restSpacing);
			DisableTheLastSpacing();
		}
		public void CalculateUniformColumnsByColumnWidth() {
			int columnWidth = (Columns[0].Width.HasValue) ? Columns[0].Width.Value : MinColumnWidth;
			CalculateUniformColumnsByColumnWidth(columnWidth);
		}
		public void CalculateUniformColumnsByColumnWidth(int columnWidth) {
			if (!columnCount.HasValue || columnCount <= 0)
				return;
			if (columnCount <= 1)
				columnWidth = pageWidth;
			if (columnWidth * columnCount > pageWidth)
				columnWidth = pageWidth / columnCount.Value;
			columnWidth = Math.Max(columnWidth, MinColumnWidth);
			int dividend = PageWidth - columnWidth * columnCount.Value;
			int divider = Math.Max(1, columnCount.Value - 1);
			int restSpacing = dividend % divider;
			int columnSpacing = dividend / divider;
			CalculateUniformColumnsCore(columnWidth, columnSpacing, 0, restSpacing);
		}
		public void CalculateUniformColumnsByColumnSpacing() {
			if (HasColumnsInfoUINull())
				return;
			int columnSpacing = (Columns[0].Spacing.HasValue) ? Columns[0].Spacing.Value : MinSpacingWidth;
			CalculateUniformColumnsByColumnSpacing(columnSpacing);
		}
		public void CalculateUniformColumnsByColumnSpacing(int columnSpacing) {
			if (!columnCount.HasValue || columnCount <= 0)
				return;
			columnSpacing = Math.Max(columnSpacing, MinSpacingWidth);
			Debug.Assert(Columns.Count == columnCount.Value);
			if (columnSpacing * (columnCount - 1) > pageWidth - MinColumnWidth * columnCount)
				columnSpacing = (pageWidth - MinColumnWidth * columnCount.Value) / (columnCount.Value - 1);
			if (columnCount <= 1)
				columnSpacing = 0;
			int dividend = PageWidth - columnSpacing * (columnCount.Value - 1);
			int restWidth = dividend % columnCount.Value;
			int columnWidth = dividend / columnCount.Value;
			CalculateUniformColumnsCore(columnWidth, columnSpacing, restWidth, 0);
		}
		protected internal virtual int CalculateAvailableSpace() {
			int usedSpace = 0;
			for (int i = 0; i < columnCount; i++)
				usedSpace += ((Columns[i].Width.HasValue) ? Columns[i].Width.Value : 0) + ((Columns[i].Spacing.HasValue) ? Columns[i].Spacing.Value : 0);
			return pageWidth - usedSpace;
		}
		public void ChangeColumnsNotEqualByWidthAfterIndex(int index) {
			if (!columnCount.HasValue || columnCount <= 0 || index >= columnCount)
				return;
			ColumnsDistributionCalculator calculatorWidth = new ColumnsDistributionWidthPriorityCalculator(Columns);
			ColumnsDistributionCalculator calculatorSpacing = new ColumnsDistributionSpacingPriorityCalculator(Columns);
			calculatorWidth.CorrectValue(index);
			int difference = -CalculateAvailableSpace();
			difference = calculatorWidth.DistributeSpace(index + 1, columnCount.Value - 1, difference);
			difference = calculatorWidth.DistributeSpace(0, index - 1, difference);
			difference = calculatorSpacing.DistributeSpace(0, columnCount.Value - 2, difference);
			Columns[index].Width -= difference;
			DisableTheLastSpacing();
		}
		public void ChangeColumnsNotEqualBySpacingAfterIndex(int index) {
			if (!columnCount.HasValue || columnCount <= 0 || index >= columnCount)
				return;
			ColumnsDistributionCalculator calculatorWidth = new ColumnsDistributionWidthPriorityCalculator(Columns);
			ColumnsDistributionCalculator calculatorSpacing = new ColumnsDistributionSpacingPriorityCalculator(Columns);
			calculatorSpacing.CorrectValue(index);
			int difference = -CalculateAvailableSpace();
			difference = calculatorWidth.DistributeSpace(index + 1, columnCount.Value - 1, difference);
			difference = calculatorWidth.DistributeSpace(0, index, difference);
			difference = calculatorSpacing.DistributeSpace(0, index - 1, difference);
			difference = calculatorSpacing.DistributeSpace(index + 1, columnCount.Value - 2, difference);
			Columns[index].Spacing -= difference;
			DisableTheLastSpacing();
		}
		public ColumnsInfoUI Clone() {
			ColumnsInfoUI result = new ColumnsInfoUI();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ColumnsInfoUI info) {
			this.ApplyType = info.ApplyType;
			this.AvailableApplyType = info.AvailableApplyType;
			this.PageWidth = info.PageWidth;
			this.EqualColumnWidth = info.EqualColumnWidth;
			ChangeColumnCount(info.Columns.Count);
			for (int i = 0; i < Columns.Count; i++) {
				Columns[i].Width = info.Columns[i].Width;
				Columns[i].Spacing = info.Columns[i].Spacing;
			}
		}
	}
	#endregion
	#region ColumnInfoUI
	public class ColumnInfoUI {
		int number;
		int? width;
		int? spacing;
		public ColumnInfoUI(int number) {
			this.number = number;
		}
		public int Number { get { return number; } }
		public int? Width { get { return width; } set { width = value; } }
		public int? Spacing { get { return spacing; } set { spacing = value; } }
	}
	#endregion
	public abstract class ColumnsInfoPreset {
		protected internal virtual string ImageName { get { return String.Empty; } }
		public virtual Image Image { get { return !string.IsNullOrEmpty(ImageName) ? LoadImage() : null; } }
		public virtual Stream ImageStream { get { return !string.IsNullOrEmpty(ImageName) ? GetType().GetAssembly().GetManifestResourceStream(ImageResourceUri) : null; } }
		protected internal string ImageResourceUri { get { return "DevExpress.XtraRichEdit.Images." + ImageName + ".png"; } }
		protected internal virtual int Spacing { get { return 1800; } }
		protected internal virtual Image LoadImage() {
			return CommandResourceImageLoader.CreateBitmapFromResources(ImageResourceUri, GetType().GetAssembly());
		}
		public abstract bool MatchTo(ColumnsInfoUI columnsInfo);
		public abstract void ApplyTo(ColumnsInfoUI columnsInfo);
	}
#region UniformColumnsInfoPreset (abstract class)
	public abstract class UniformColumnsInfoPreset : ColumnsInfoPreset {
		public abstract int ColumnCount { get; }
		public override bool MatchTo(ColumnsInfoUI columnsInfo) {
			if (!columnsInfo.EqualColumnWidth.HasValue)
				return false;
			if (!columnsInfo.EqualColumnWidth.Value)
				return false;
			if (!columnsInfo.ColumnCount.HasValue)
				return false;
			return columnsInfo.ColumnCount.Value == ColumnCount;
		}
		public override void ApplyTo(ColumnsInfoUI columnsInfo) {
			columnsInfo.EqualColumnWidth = true;
			if (columnsInfo.Columns.Count > 0)
				columnsInfo.Columns[0].Spacing = Spacing;
			columnsInfo.ChangeColumnCount(ColumnCount);
		}
	}
#endregion
#region SingleColumnsInfoPreset
	public class SingleColumnsInfoPreset : UniformColumnsInfoPreset {
		public override int ColumnCount { get { return 1; } }
		protected internal override string ImageName { get { return "OneColumn"; } }
	}
#endregion
#region TwoUniformColumnsInfoPreset
	public class TwoUniformColumnsInfoPreset : UniformColumnsInfoPreset {
		public override int ColumnCount { get { return 2; } }
		protected internal override string ImageName { get { return "TwoColumns"; } }
	}
#endregion
#region ThreeUniformColumnsInfoPreset
	public class ThreeUniformColumnsInfoPreset : UniformColumnsInfoPreset {
		public override int ColumnCount { get { return 3; } }
		protected internal override string ImageName { get { return "ThreeColumns"; } }
	}
#endregion
#region TwoNonUniformColumnsInfoPreset (abstract class)
	public abstract class TwoNonUniformColumnsInfoPreset : ColumnsInfoPreset {
		protected internal abstract float FirstColumnRelativeWidth { get; }
		public override bool MatchTo(ColumnsInfoUI columnsInfo) {
			if (!columnsInfo.EqualColumnWidth.HasValue)
				return false;
			if (columnsInfo.EqualColumnWidth.Value)
				return false;
			if (columnsInfo.ColumnCount != 2)
				return false;
			if (columnsInfo.Columns.Count != 2)
				return false;
			if (!columnsInfo.Columns[0].Width.HasValue)
				return false;
			if (!columnsInfo.Columns[0].Spacing.HasValue)
				return false;
			if (!columnsInfo.Columns[1].Width.HasValue)
				return false;
			if (!columnsInfo.Columns[1].Spacing.HasValue)
				return false;
			int totalWidth = columnsInfo.PageWidth - Spacing;
			if (columnsInfo.Columns[0].Width != (int)Math.Round(totalWidth * FirstColumnRelativeWidth))
				return false;
			if (columnsInfo.Columns[0].Spacing != Spacing)
				return false;
			if (columnsInfo.Columns[1].Width != (int)Math.Round((double)totalWidth - columnsInfo.Columns[0].Width.Value))
				return false;
			return columnsInfo.Columns[1].Spacing == 0;
		}
		public override void ApplyTo(ColumnsInfoUI columnsInfo) {
			columnsInfo.EqualColumnWidth = false;
			columnsInfo.ChangeColumnCount(2);
			int totalWidth = columnsInfo.PageWidth - Spacing;
			columnsInfo.Columns[0].Width = (int)Math.Round(totalWidth * FirstColumnRelativeWidth);
			columnsInfo.Columns[0].Spacing = Spacing;
			columnsInfo.Columns[1].Width = (int)Math.Round((double)totalWidth - columnsInfo.Columns[0].Width.Value);
			columnsInfo.Columns[1].Spacing = 0;
		}
	}
#endregion
#region LeftNarrowColumnsInfoPreset
	public class LeftNarrowColumnsInfoPreset : TwoNonUniformColumnsInfoPreset {
		protected internal override float FirstColumnRelativeWidth { get { return 0.292f; } }
		protected internal override string ImageName { get { return "LeftColumns"; } }
	}
#endregion
#region RightNarrowColumnsInfoPreset
	public class RightNarrowColumnsInfoPreset : TwoNonUniformColumnsInfoPreset {
		protected internal override float FirstColumnRelativeWidth { get { return 0.708f; } }
		protected internal override string ImageName { get { return "RightColumns"; } }
	}
#endregion
#region ColumnsDistributionCalculator (abstract class)
	public abstract class ColumnsDistributionCalculator {
		readonly List<ColumnInfoUI> columns;
		protected ColumnsDistributionCalculator(List<ColumnInfoUI> columns) {
			Guard.ArgumentNotNull(columns, "columns");
			this.columns = columns;
		}
		public List<ColumnInfoUI> Columns { get { return columns; } }
		public abstract int MinValue { get; }
		protected internal virtual int CalculateTotal(int from, int to) {
			int result = 0;
			for (int i = from; i <= to; i++)
				result += GetValue(Columns[i]);
			return result;
		}
		protected internal virtual bool HasEnoughSpaceForDistribution(int from, int to, int space) {
			int total = CalculateTotal(from, to);
			return space < total - MinValue * (to - from + 1);
		}
		protected internal virtual int SetMinValues(int from, int to, int space) {
			for (int i = from; i <= to; i++) {
				space -= GetValue(Columns[i]) - MinValue;
				SetValue(Columns[i], MinValue);
			}
			return space;
		}
		protected internal virtual void CorrectValue(int index) { 
			if (index >= Columns.Count)
				return;
			if (GetValue(Columns[index]) < MinValue)
				SetValue(Columns[index], MinValue);
		}
		protected internal virtual int DistributeRemainder(int from, int to, int remainder) {  
			int correction = (remainder > 0) ? 1 : -1;
			while (remainder != 0) {
				for (int i = from; i <= to && (remainder != 0); i++) {
					int newValue = GetValue(Columns[i]) - correction;
					if (newValue > MinValue) {
						SetValue(Columns[i], newValue);
						remainder -= correction;
					}
				}
			}
			return 0;
		}
		protected internal virtual int DistributeSpaceCore(int from, int to, int space) {  
			int remainder = space % (to - from + 1);
			int difference =space / (to - from + 1);
			for (int i = from; i <= to; i++) {
				int newValue = GetValue(Columns[i]) - difference;
				if (newValue >= MinValue)
					SetValue(Columns[i], newValue);
				else {
					SetValue(Columns[i], MinValue);
					remainder += (MinValue - newValue);
				}
			}
			DistributeRemainder(from, to, remainder);
			return 0;
		}
		protected internal virtual int DistributeSpace(int from, int to, int space) {
			if (from > to)
				return space;
			if (HasEnoughSpaceForDistribution(from, to, space))
				return DistributeSpaceCore(from, to, space);
			else
				return SetMinValues(from, to, space);
		}
		protected internal virtual void SetAllValues(int value, int rest) {
			int count = Columns.Count;
			for (int i = 0; i < count; i++)
				SetValue(Columns[i], value);
			DistributeSpace(0, count - 1, -rest);
		}
		protected abstract int GetValue(ColumnInfoUI column);
		protected abstract void SetValue(ColumnInfoUI column, int value);
	}
#endregion
#region ColumnsDistributionWidthPriorityCalculator
	public class ColumnsDistributionWidthPriorityCalculator : ColumnsDistributionCalculator {
		public ColumnsDistributionWidthPriorityCalculator(List<ColumnInfoUI> columns)
			: base(columns) {
		}
		public override int MinValue { get { return 720; } }	
		protected override int GetValue(ColumnInfoUI column) {
			return (column.Width.HasValue) ? column.Width.Value : 0;
		}
		protected override void SetValue(ColumnInfoUI column, int value) {
			column.Width = value;
		}
	}
#endregion
#region ColumnsDistributionSpacingPriorityCalculator
	public class ColumnsDistributionSpacingPriorityCalculator : ColumnsDistributionCalculator {
		public ColumnsDistributionSpacingPriorityCalculator(List<ColumnInfoUI> columns)
			: base(columns) {
		}
		public override int MinValue { get { return 0; } }  
		protected override int GetValue(ColumnInfoUI column) {
			return (column.Spacing.HasValue) ? column.Spacing.Value : 0;
		}
		protected override void SetValue(ColumnInfoUI column, int value) {
			column.Spacing = value;
		}
	}
#endregion
#region ColumnsSetupFormControllerParameters
	public class ColumnsSetupFormControllerParameters : FormControllerParameters {
		readonly ColumnsInfoUI columnsInfo;
		internal ColumnsSetupFormControllerParameters(IRichEditControl control, ColumnsInfoUI columnsInfo)
			: base(control) {
			Guard.ArgumentNotNull(columnsInfo, "columnsInfo");
			this.columnsInfo = columnsInfo;
		}
		internal ColumnsInfoUI ColumnsInfo { get { return columnsInfo; } }
	}
#endregion
#region ColumnsSetupFormController
	public class ColumnsSetupFormController : FormController {
#region Fields
		readonly IRichEditControl control;
		readonly ColumnsInfoUI columnsInfo;
		readonly ColumnsInfoUI sourceColumnsInfo;
		DocumentModelUnitConverter valueUnitConverter;
#endregion
		public ColumnsSetupFormController(ColumnsSetupFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.sourceColumnsInfo = controllerParameters.ColumnsInfo;
			this.columnsInfo = sourceColumnsInfo.Clone();
			this.control = controllerParameters.Control;
			this.valueUnitConverter = control.InnerControl.DocumentModel.UnitConverter;
		}
#region Properties
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } set { valueUnitConverter = value; } }
		public ColumnsInfoUI SourceColumnsInfo { get { return sourceColumnsInfo; } }
		public ColumnsInfoUI ColumnsInfo { get { return columnsInfo; } }
		public SectionPropertiesApplyType AvailableApplyType { get { return columnsInfo.AvailableApplyType; } }
		public SectionPropertiesApplyType ApplyType { get { return columnsInfo.ApplyType; } set { columnsInfo.ApplyType = value; } }
#endregion
		public override void ApplyChanges() {
			sourceColumnsInfo.CopyFrom(columnsInfo);
		}
		public void ChangeColumnCount(int count) {
			columnsInfo.ChangeColumnCount(count);
		}
		public void SetEqualColumnWidth(bool value) {
			columnsInfo.EqualColumnWidth = value;
			if (value)
				columnsInfo.CalculateUniformColumnsByColumnSpacing();
		}
		public void ApplyPreset(ColumnsInfoPreset preset) {
			preset.ApplyTo(ColumnsInfo);
		}
	}
#endregion
}
