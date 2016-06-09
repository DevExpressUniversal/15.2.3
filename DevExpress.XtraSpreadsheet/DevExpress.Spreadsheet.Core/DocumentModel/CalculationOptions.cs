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
using DevExpress.Office;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelCalculationMode
	public enum ModelCalculationMode { 
		AutomaticExceptTables = 0,
		Manual = 1,
		Automatic = 2
	}
	#endregion
	#region CalculationOptionsInfo
	public class CalculationOptionsInfo : ICloneable<CalculationOptionsInfo>, ISupportsCopyFrom<CalculationOptionsInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskCalculationMode = 0x00000003;		
		const uint MaskRecalculateBeforeSaving = 0x00000004;
		const uint MaskUpdateRemoteReferences = 0x00000010; 
		const uint MaskDateSystem = 0x00000040;			 
		const uint MaskSaveExternalLinkValues = 0x00000080; 
		const uint MaskAcceptLabelsInFormulas = 0x00000100; 
		const uint MaskFullCalculationOnLoad = 0x00000200;  
		uint packedValues;
		int maximumIterations;
		double iterativeCalculationDelta; 
		#endregion
		#region Properties
		#region ModelCalculationMode
		public ModelCalculationMode CalculationMode {
			get { return (ModelCalculationMode)(packedValues & MaskCalculationMode); }
			set {
				packedValues &= ~MaskCalculationMode;
				packedValues |= (uint)value & MaskCalculationMode;
			}
		}
		#endregion
		#region DateSystem
		public DateSystem DateSystem {
			get { return (DateSystem)((packedValues & MaskDateSystem) >> 6); }
			set {
				packedValues &= ~MaskDateSystem;
				packedValues |= ((uint)value << 6) & MaskDateSystem;
			}
		}
		#endregion
		public bool RecalculateBeforeSaving { get { return GetBooleanVal(MaskRecalculateBeforeSaving); } set { SetBooleanVal(MaskRecalculateBeforeSaving, value); } }
		public bool UpdateRemoteReferences { get { return GetBooleanVal(MaskUpdateRemoteReferences); } set { SetBooleanVal(MaskUpdateRemoteReferences, value); } }
		public bool SaveExternalLinkValues { get { return GetBooleanVal(MaskSaveExternalLinkValues); } set { SetBooleanVal(MaskSaveExternalLinkValues, value); } }
		public bool AcceptLabelsInFormulas { get { return GetBooleanVal(MaskAcceptLabelsInFormulas); } set { SetBooleanVal(MaskAcceptLabelsInFormulas, value); } }
		public bool FullCalculationOnLoad { get { return GetBooleanVal(MaskFullCalculationOnLoad); } set { SetBooleanVal(MaskFullCalculationOnLoad, value); } }
		public int MaximumIterations { get { return maximumIterations; } set { maximumIterations = value; } }
		public double IterativeCalculationDelta { get { return iterativeCalculationDelta; } set { iterativeCalculationDelta = value; } }
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ICloneable<CalculationOptionsInfo> Members
		public CalculationOptionsInfo Clone() {
			CalculationOptionsInfo clone = new CalculationOptionsInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<CalculationOptionsInfo> Members
		public void CopyFrom(CalculationOptionsInfo value) {
			this.packedValues = value.packedValues;
			this.maximumIterations = value.maximumIterations;
			this.iterativeCalculationDelta = value.iterativeCalculationDelta;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			CalculationOptionsInfo info = obj as CalculationOptionsInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues &&
				this.maximumIterations == info.maximumIterations &&
				this.iterativeCalculationDelta == info.iterativeCalculationDelta;
		}
		public override int GetHashCode() {
			return (int)(packedValues ^ maximumIterations ^ iterativeCalculationDelta.GetHashCode());
		}
	}
	#endregion
	#region CalculationOptionsInfoCache
	public class CalculationOptionsInfoCache : UniqueItemsCache<CalculationOptionsInfo> {
		public CalculationOptionsInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override CalculationOptionsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			CalculationOptionsInfo info = new CalculationOptionsInfo();
			info.CalculationMode = ModelCalculationMode.Automatic;
			info.IterativeCalculationDelta = 0.001;
			info.MaximumIterations = 100;
			info.UpdateRemoteReferences = true;
			info.SaveExternalLinkValues = true;
			info.RecalculateBeforeSaving = true;
			return info;
		}
	}
	#endregion
	#region CalculationOptions
	public class CalculationOptions : SpreadsheetUndoableIndexBasedObject<CalculationOptionsInfo> {
		#region Fields
		const bool defaultIiterationsEnabled = false;
		const bool defaultPrecisionAsDisplayed = false;
		bool iterationsEnabled;
		bool precisionAsDisplayed;
		#endregion
		public CalculationOptions(DocumentModel workbook)
			: base(workbook) {
			this.iterationsEnabled = defaultIiterationsEnabled;
			this.precisionAsDisplayed = defaultPrecisionAsDisplayed;
		}
		#region Properties
		#region ModelCalculationMode
		public ModelCalculationMode CalculationMode {
			get { return Info.CalculationMode; }
			set {
				if (CalculationMode == value)
					return;
				SetPropertyValue(SetCalculationModeCore, value);
			}
		}
		DocumentModelChangeActions SetCalculationModeCore(CalculationOptionsInfo info, ModelCalculationMode value) {
			info.CalculationMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RecalculateBeforeSaving
		public bool RecalculateBeforeSaving {
			get { return Info.RecalculateBeforeSaving; }
			set {
				if (RecalculateBeforeSaving == value)
					return;
				SetPropertyValue(SetRecalculateBeforeSavingCore, value);
			}
		}
		DocumentModelChangeActions SetRecalculateBeforeSavingCore(CalculationOptionsInfo info, bool value) {
			info.RecalculateBeforeSaving = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IterationsEnabled
		public bool IterationsEnabled {
			get { return iterationsEnabled; }
			set {
				if (iterationsEnabled == value)
					return;
				DocumentHistory history = DocumentModel.History;
				CalculationIterationsEnabledHistoryItem historyItem = new CalculationIterationsEnabledHistoryItem(this, iterationsEnabled, value);
				history.Add(historyItem);
				historyItem.Execute();
			}
		}
		internal void SetIterationsEnabledCore(bool value) {
			this.iterationsEnabled = value;
		}
		#endregion
		#region MaxIterationCount
		public int MaximumIterations {
			get { return Info.MaximumIterations; }
			set {
				if (MaximumIterations == value)
					return;
				SetPropertyValue(SetMaximumIterationsCore, value);
			}
		}
		DocumentModelChangeActions SetMaximumIterationsCore(CalculationOptionsInfo info, int value) {
			info.MaximumIterations = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IterativeCalculationDelta
		public double IterativeCalculationDelta {
			get { return Info.IterativeCalculationDelta; }
			set {
				if (IterativeCalculationDelta == value)
					return;
				SetPropertyValue(SetIterativeCalculationDeltaCore, value);
			}
		}
		DocumentModelChangeActions SetIterativeCalculationDeltaCore(CalculationOptionsInfo info, double value) {
			info.IterativeCalculationDelta = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UpdateRemoteReferences
		public bool UpdateRemoteReferences {
			get { return Info.UpdateRemoteReferences; }
			set {
				if (UpdateRemoteReferences == value)
					return;
				SetPropertyValue(SetUpdateRemoteReferencesCore, value);
			}
		}
		DocumentModelChangeActions SetUpdateRemoteReferencesCore(CalculationOptionsInfo info, bool value) {
			info.UpdateRemoteReferences = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrecisionAsDisplayed
		public bool PrecisionAsDisplayed {
			get { return precisionAsDisplayed; }
			set {
				if (precisionAsDisplayed == value)
					return;
				CalculationPrecisionAsDisplayedHistoryItem historyItem = new CalculationPrecisionAsDisplayedHistoryItem(DocumentModel, precisionAsDisplayed, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		internal void SetPrecisionAsDisplayedCore(bool value) {
			this.precisionAsDisplayed = value;
		}
		#endregion
		#region DateSystem
		public DateSystem DateSystem {
			get { return Info.DateSystem; }
			set {
				if (DateSystem == value)
					return;
				SetPropertyValue(SetDateSystemCore, value);
			}
		}
		DocumentModelChangeActions SetDateSystemCore(CalculationOptionsInfo info, DateSystem value) {
			info.DateSystem = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SaveExternalLinkValues
		public bool SaveExternalLinkValues {
			get { return Info.SaveExternalLinkValues; }
			set {
				if (SaveExternalLinkValues == value)
					return;
				SetPropertyValue(SetSaveExternalLinkValuesCore, value);
			}
		}
		DocumentModelChangeActions SetSaveExternalLinkValuesCore(CalculationOptionsInfo info, bool value) {
			info.SaveExternalLinkValues = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AcceptLabelsInFormulas
		public bool AcceptLabelsInFormulas {
			get { return Info.AcceptLabelsInFormulas; }
			set {
				if (AcceptLabelsInFormulas == value)
					return;
				SetPropertyValue(SetAcceptLabelsInFormulasCore, value);
			}
		}
		DocumentModelChangeActions SetAcceptLabelsInFormulasCore(CalculationOptionsInfo info, bool value) {
			info.AcceptLabelsInFormulas = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FullCalculationOnLoad
		public bool FullCalculationOnLoad {
			get { return Info.FullCalculationOnLoad; }
			set {
				if (FullCalculationOnLoad == value)
					return;
				SetPropertyValue(SetFullCalculationOnLoadCore, value);
			}
		}
		DocumentModelChangeActions SetFullCalculationOnLoadCore(CalculationOptionsInfo info, bool value) {
			info.FullCalculationOnLoad = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<CalculationOptionsInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.CalculationOptionsInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		public bool IsDefault() {
			return Index == 0 && iterationsEnabled == defaultIiterationsEnabled && precisionAsDisplayed == defaultPrecisionAsDisplayed;
		}
		public void Reset() {
			this.SetIndexInitial(0);
			this.iterationsEnabled = defaultIiterationsEnabled;
			this.precisionAsDisplayed = defaultPrecisionAsDisplayed;
		}
	}
	#endregion
}
