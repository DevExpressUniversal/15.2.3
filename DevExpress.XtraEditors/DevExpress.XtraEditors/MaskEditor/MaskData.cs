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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Data.Mask;
namespace DevExpress.XtraEditors.Mask {
	using DevExpress.XtraEditors.Controls;
	using DevExpress.Utils.Controls;
	public enum MaskType { None, DateTime, DateTimeAdvancingCaret, Numeric, RegEx, Regular, Simple, Custom }
	public enum AutoCompleteType { Default, None, Strong, Optimistic }
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MaskProperties {
		#region Fields & Properties
		protected bool fSaveLiteral = true;
		protected bool fShowPlaceHolders = true;
		protected char fPlaceHolder = '_';
		protected string fEditMask = string.Empty;
		protected MaskType fMaskType = MaskType.None;
		protected bool fIgnoreMaskBlank = true;
		protected bool fUseMaskAsDisplayFormat = false;
		protected bool fBeepOnError = false;
		protected AutoCompleteType fAutoComplete = AutoCompleteType.Default;
		protected CultureInfo fCulture = null;
		int lockEvents = 0;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesSaveLiteral"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue(true),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual bool SaveLiteral {
			get { return fSaveLiteral; }
			set {
				if(fSaveLiteral != value) {
					DoBeforeChange("SaveLiteral", value);
					fSaveLiteral = value;
					RaiseAfterChange();
				}
			}
		}
		[
		CategoryAttribute("Mask"),
		DefaultValue("_"),
		Localizable(true),
		Obsolete("Use PlaceHolder property instead"),
		RefreshProperties(RefreshProperties.All),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public virtual string Blank {
			get { return ShowPlaceHolders ? PlaceHolder.ToString(CultureInfo.InvariantCulture) : " "; }
			set {
				if(value == null || value.Length == 0) {
					ShowPlaceHolders = false;
				} else {
					ShowPlaceHolders = true;
					PlaceHolder = value[0];
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesAutoComplete"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue(AutoCompleteType.Default),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual AutoCompleteType AutoComplete {
			get { return fAutoComplete; }
			set {
				if(AutoComplete != value) {
					DoBeforeChange("AutoComplete", value);
					fAutoComplete = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesShowPlaceHolders"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue(true),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual bool ShowPlaceHolders {
			get { return fShowPlaceHolders; }
			set {
				if(ShowPlaceHolders != value) {
					DoBeforeChange("ShowPlaceHolders", value);
					fShowPlaceHolders = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesPlaceHolder"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue('_'),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual char PlaceHolder {
			get { return fPlaceHolder; }
			set {
				if(value == '\0')
					value = '_';
				if(fPlaceHolder != value) {
					DoBeforeChange("PlaceHolder", value);
					fPlaceHolder = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesMaskType"),
#endif
		DefaultValue(MaskType.None),
		CategoryAttribute("Mask"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual MaskType MaskType {
			get { return fMaskType; }
			set {
				if(fMaskType != value) {
					DoBeforeChange("MaskType", value);
					fMaskType = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesEditMask"),
#endif
 CategoryAttribute("Mask"),
		DefaultValue(""),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public virtual string EditMask {
			get { return fEditMask; }
			set {
				if(EditMask != value) {
					DoBeforeChange("EditMask", value);
					fEditMask = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesIgnoreMaskBlank"),
#endif
		DefaultValue(true),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public virtual bool IgnoreMaskBlank {
			get { return fIgnoreMaskBlank; }
			set {
				if(IgnoreMaskBlank != value) {
					DoBeforeChange("IgnoreMaskBlank", value);
					fIgnoreMaskBlank = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesUseMaskAsDisplayFormat"),
#endif
		DefaultValue(false),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public virtual bool UseMaskAsDisplayFormat {
			get { return fUseMaskAsDisplayFormat; }
			set {
				if(UseMaskAsDisplayFormat != value) {
					DoBeforeChange("UseMaskAsDisplayFormat", value);
					fUseMaskAsDisplayFormat = value;
					RaiseAfterChange();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskPropertiesBeepOnError"),
#endif
		DefaultValue(false),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public virtual bool BeepOnError {
			get { return fBeepOnError; }
			set {
				if(BeepOnError != value) {
					DoBeforeChange("BeepOnError", value);
					fBeepOnError = value;
					RaiseAfterChange();
				}
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CultureInfo Culture {
			get { return fCulture; }
			set {
				if(!object.ReferenceEquals(value, Culture)) {
					DoBeforeChange("Culture", value);
					fCulture = value;
					RaiseAfterChange();
				}
			}
		}
		#endregion
		#region Events
		public event EventHandler AfterChange;
		public event ChangeEventHandler BeforeChange;
		#endregion
		public MaskProperties() {
		}
		public MaskProperties(MaskProperties data) {
			Assign(data);
		}
		public void BeginUpdate() {
			this.lockEvents++;
		}
		public void EndUpdate() {
			if(--this.lockEvents == 0) {
				RaiseAfterChange();
			}
		}
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		public void Assign(MaskData data) {
			BeginUpdate();
			try {
				this.fIgnoreMaskBlank = data.IgnoreMaskBlank;
				this.fBeepOnError = data.BeepOnError;
				this.fSaveLiteral = data.SaveLiteral;
				this.fMaskType = data.MaskType;
				this.fEditMask = data.EditMask;
				if(data.Blank == null || data.Blank.Length == 0 || data.Blank == " ") {
					this.fShowPlaceHolders = false;
				} else {
					this.fShowPlaceHolders = true;
					this.fPlaceHolder = data.Blank[0];
				}
			} finally {
				EndUpdate();
			}
		}
		public void Assign(MaskProperties data) {
			this.fSaveLiteral = data.SaveLiteral;
			this.fShowPlaceHolders = data.ShowPlaceHolders;
			this.fPlaceHolder = data.PlaceHolder;
			this.fEditMask = data.EditMask;
			this.fMaskType = data.MaskType;
			this.fIgnoreMaskBlank = data.IgnoreMaskBlank;
			this.fUseMaskAsDisplayFormat = data.UseMaskAsDisplayFormat;
			this.fBeepOnError = data.BeepOnError;
			this.fAutoComplete = data.AutoComplete;
			this.fCulture = data.fCulture;
			this.fAllowNullInput = data.AllowNullInput;
			RaiseAfterChange();
		}
		protected void RaiseAfterChange() {
			if(this.lockEvents != 0) return;
			if(AfterChange != null) AfterChange(this, EventArgs.Empty);
		}
		protected void DoBeforeChange(string name, object value) {
			if(BeforeChange != null) {
				ChangeEventArgs args = new ChangeEventArgs(name, value);
				BeforeChange(this, args);
			}
		}
		public override bool Equals(object obj) {
			MaskProperties another = obj as MaskProperties;
			if(another == null)
				return false;
			if(this.MaskType != another.MaskType)
				return false;
			if(this.EditMask != another.EditMask)
				return false;
			if(this.AutoComplete != another.AutoComplete)
				return false;
			if(this.BeepOnError != another.BeepOnError)
				return false;
			if(this.ShowPlaceHolders != another.ShowPlaceHolders)
				return false;
			if(this.PlaceHolder != another.PlaceHolder)
				return false;
			if(this.IgnoreMaskBlank != another.IgnoreMaskBlank)
				return false;
			if(this.AllowNullInput != another.AllowNullInput)
				return false;
			if(this.UseMaskAsDisplayFormat != another.UseMaskAsDisplayFormat)
				return false;
			if(this.SaveLiteral != another.SaveLiteral)
				return false;
			if(!object.ReferenceEquals(this.Culture, another.Culture))
				return false;
			return true;
		}
		public override int GetHashCode() {
			return EditMask == null ? 0 : EditMask.GetHashCode();
		}
		public virtual MaskManager CreateDefaultMaskManager() {
			return CreateDefaultMaskManager(this);
		}
		static MaskManager CreateDefaultMaskManager(MaskProperties mask) {
			CultureInfo managerCultureInfo = mask.Culture;
			if(managerCultureInfo == null)
				managerCultureInfo = CultureInfo.CurrentCulture;
			string editMask = mask.EditMask;
			if(editMask == null)
				editMask = string.Empty;
			switch(mask.MaskType) {
				case MaskType.Numeric:
					return new NumericMaskManager(editMask, managerCultureInfo, mask.AllowNullInput);
				case MaskType.RegEx:
					if(mask.IgnoreMaskBlank && editMask.Length > 0)
						editMask = "(" + editMask + ")?";
					return new RegExpMaskManager(editMask, false, mask.AutoComplete != AutoCompleteType.None, mask.AutoComplete == AutoCompleteType.Optimistic, mask.ShowPlaceHolders, mask.PlaceHolder, managerCultureInfo);
				case MaskType.DateTime:
					return new DateTimeMaskManager(editMask, false, managerCultureInfo, mask.AllowNullInput);
				case MaskType.DateTimeAdvancingCaret:
					return new DateTimeMaskManager(editMask, true, managerCultureInfo, mask.AllowNullInput);
				case MaskType.Regular:
					return new LegacyMaskManager(LegacyMaskInfo.GetRegularMaskInfo(editMask, managerCultureInfo), mask.PlaceHolder, mask.SaveLiteral, mask.IgnoreMaskBlank);
				case MaskType.Simple:
					return new LegacyMaskManager(LegacyMaskInfo.GetSimpleMaskInfo(editMask, managerCultureInfo), mask.PlaceHolder, mask.SaveLiteral, mask.IgnoreMaskBlank);
				default:
					return null;
			}
		}
		internal bool AllowNullInput {
			get {
				if(RepositoryItem != null)
					return RepositoryItem.IsNullInputAllowed;
				if(fAllowNullInput.HasValue)
					return fAllowNullInput.Value;
				return IgnoreMaskBlank;
			}
			set {
				fAllowNullInput = value;
			}
		}
		protected internal DevExpress.XtraEditors.Repository.RepositoryItemTextEdit RepositoryItem = null;   
		bool? fAllowNullInput;
	}
	public class NumericMaskProperties : MaskProperties {
		public NumericMaskProperties()
			: base() {
			this.fMaskType = MaskType.Numeric;
		}
		[
		DefaultValue(MaskType.Numeric),
		CategoryAttribute("Mask"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public override MaskType MaskType {
			get { return base.MaskType; }
			set { base.MaskType = value; }
		}
	}
	public class DateTimeMaskProperties : MaskProperties {
		public DateTimeMaskProperties()
			: base() {
			this.fMaskType = MaskType.DateTime;
		}
		[
		DefaultValue(MaskType.DateTime),
		CategoryAttribute("Mask"),
		Localizable(true),
		RefreshProperties(RefreshProperties.All),
		]
		public override MaskType MaskType {
			get { return base.MaskType; }
			set { base.MaskType = value; }
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	using DevExpress.XtraEditors.Mask;
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MaskData {
		#region Fields & Properties
		private bool saveLiteral = true;
		private string blank = "_";
		private string editMask = string.Empty;
		private MaskType maskType = MaskType.None;
		private bool ignore = true;
		private bool beepOnError = false;
		private bool enableEvent = true;
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataSaveLiteral"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue(true),
		Localizable(true),
		]
		public bool SaveLiteral {
			get { return saveLiteral; }
			set {
				if(saveLiteral != value) {
					saveLiteral = value;
					DoAfterChange("SaveLiteral", value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataBlank"),
#endif
		CategoryAttribute("Mask"),
		DefaultValue("_"),
		Localizable(true),
		]
		public string Blank {
			get { return (blank.Length > 0) ? blank.Substring(0, 1) : "_"; }
			set {
				string newValue = (value.Length == 0) ? " " : value;
				if(blank != newValue) {
					blank = newValue;
					DoAfterChange("Blank", blank);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataMaskType"),
#endif
		DefaultValue(MaskType.None),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public MaskType MaskType {
			get { return maskType; }
			set {
				if(maskType != value) {
					maskType = value;
					DoAfterChange("MaskType", value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataEditMask"),
#endif
		Editor("DevExpress.XtraEditors.Design.MaskEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)),
		CategoryAttribute("Mask"),
		DefaultValue(""),
		Localizable(true),
		]
		public string EditMask {
			get { return editMask; }
			set {
				if(editMask.Equals(value) == false) {
					editMask = value;
					DoAfterChange("EditMask", value);
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataIgnoreMaskBlank"),
#endif
		DefaultValue(true),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public bool IgnoreMaskBlank {
			get { return ignore; }
			set { ignore = value; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("MaskDataBeepOnError"),
#endif
		DefaultValue(false),
		CategoryAttribute("Mask"),
		Localizable(true),
		]
		public bool BeepOnError {
			get { return beepOnError; }
			set { beepOnError = value; }
		}
		#endregion
		#region Events
		public event ChangeEventHandler AfterChange;
		#endregion
		public MaskData() {
		}
		public MaskData(MaskData data) {
			Assign(data);
		}
		public void Assign(MaskData data) {
			if(this.editMask != data.editMask)
				enableEvent = false;
			this.IgnoreMaskBlank = data.ignore;
			this.BeepOnError = data.beepOnError;
			this.SaveLiteral = data.saveLiteral;
			this.Blank = data.blank;
			this.MaskType = data.maskType; 
			enableEvent = true;
			this.EditMask = data.editMask;
		}
		public bool Compare(MaskData sample) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor descriptor in properties) {
				object sampleValue = descriptor.GetValue(sample);
				object value = descriptor.GetValue(this);
				if(sampleValue.Equals(value) == false) return false;
			}
			return true;
		}
		internal void DoAfterChange(string name, object value) {
			if(enableEvent && AfterChange != null) {
				ChangeEventArgs args = new ChangeEventArgs(name, value);
				AfterChange(this, args);
			}
		}
	}
}
