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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit {
	#region FieldsHighlightMode
	[ComVisible(true)]
	public enum FieldsHighlightMode {
		Auto,
		Never,
		Always
	}
	#endregion
	public enum UpdateLockedFields {
		Always,
		Never,
		DocVariableOnly
	}
	#region FieldOptions
	[ComVisible(true)]
	public class FieldOptions : RichEditNotificationOptions {
		#region Fields
		const FieldsHighlightMode defaultHighlightMode = FieldsHighlightMode.Auto;
		static readonly Color defaultHighlightColor = DXColor.Silver;
		const UpdateLockedFields defaultUpdateLockedFields = UpdateLockedFields.Never;
		FieldsHighlightMode highlightMode;
		Color highlightColor;
		bool useCurrentCultureDateTimeFormat;
		bool updateDocVariablesBeforePrint = true;
		bool updateDocVariablesBeforeCopy = true;
		bool updateFieldsOnPaste = true;
		bool throwExceptionOnInvalidFormatSwitch = true;
		bool clearUnhandledDocVariableField = true;
		UpdateLockedFields updateLockedFields = defaultUpdateLockedFields;
		bool updateFieldsInTextBoxes;
		#endregion
		#region Properties
		#region HighlightMode
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FieldOptionsHighlightMode"),
#endif
DefaultValue(defaultHighlightMode)]
		public FieldsHighlightMode HighlightMode {
			get { return highlightMode; }
			set {
				if (highlightMode == value)
					return;
				FieldsHighlightMode oldHighlightMode = highlightMode;
				highlightMode = value;
				OnChanged("HighlightMode", oldHighlightMode, value);
			}
		}
		#endregion
		#region HighlightColor
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsHighlightColor")]
#endif
public Color HighlightColor {
			get { return highlightColor; }
			set {
				if (highlightColor == value)
					return;
				Color oldHighlightColor = highlightColor;
				highlightColor = value;
				OnChanged("HighlightColor", oldHighlightColor, value);
			}
		}
		protected internal virtual bool ShouldSerializeHighlightColor() {
			return HighlightColor != defaultHighlightColor;
		}
		protected internal virtual void ResetHighlightColor() {
			HighlightColor = defaultHighlightColor;
		}
		#endregion
		#region UseCurrentCultureForDateTimeFormatting
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsUseCurrentCultureDateTimeFormat")]
#endif
		[DefaultValue(false)]
		public bool UseCurrentCultureDateTimeFormat {
			get { return useCurrentCultureDateTimeFormat; }
			set {
				if (useCurrentCultureDateTimeFormat == value)
					return;
				useCurrentCultureDateTimeFormat = value;
				OnChanged("UseCurrentCultureDateTimeFormat", !value, value);
			}
		}
		#endregion
		#region UpdateLockedFields
		[DefaultValue(defaultUpdateLockedFields)]
		public UpdateLockedFields UpdateLockedFields {
			get { return updateLockedFields; }
			set {
				if (updateLockedFields == value)
					return;
				UpdateLockedFields oldValue = updateLockedFields;
				updateLockedFields = value;
				OnChanged("UpdateLockedFields", oldValue, value);
			}
		}
		#endregion        
		#region ClearUnhandledDocVariableField
		[DefaultValue(true)]
		public bool ClearUnhandledDocVariableField {
			get { return clearUnhandledDocVariableField; }
			set {
				if (clearUnhandledDocVariableField == value)
					return;
				clearUnhandledDocVariableField = value;
				OnChanged("ClearUnhandledDocVariableField", !value, value);
			}
		}
		#endregion
		#region UpdateDocVariablesBeforePrint
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsUpdateDocVariablesBeforePrint")]
#endif
		[DefaultValue(true)]
		public bool UpdateDocVariablesBeforePrint {
			get { return updateDocVariablesBeforePrint; }
			set {
				if (updateDocVariablesBeforePrint == value)
					return;
				updateDocVariablesBeforePrint = value;
				OnChanged("UpdateDocVariablesBeforePrint", !value, value);
			}
		}
		#endregion
		#region UpdateDocVariablesBeforeCopy
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsUpdateDocVariablesBeforeCopy")]
#endif
		[DefaultValue(true)]
		public bool UpdateDocVariablesBeforeCopy {
			get { return updateDocVariablesBeforeCopy; }
			set {
				if (updateDocVariablesBeforeCopy == value)
					return;
				updateDocVariablesBeforeCopy = value;
				OnChanged("UpdateDocVariablesBeforeCopy", !value, value);
			}
		}
		#endregion
		#region EnableToggleFieldCodesOnPaste
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsUpdateFieldsOnPaste")]
#endif
		[DefaultValue(true)]
		public bool UpdateFieldsOnPaste {
			get { return updateFieldsOnPaste; }
			set {
				if (updateFieldsOnPaste == value)
					return;
				updateFieldsOnPaste = value;
				OnChanged("UpdateFieldsOnPaste", !value, value);
			}
		}
		#endregion
		#region ThrowExceptionOnInvalidFormatSwitch
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("FieldOptionsThrowExceptionOnInvalidFormatSwitch")]
#endif
		[DefaultValue(true)]
		public bool ThrowExceptionOnInvalidFormatSwitch {
			get { return throwExceptionOnInvalidFormatSwitch; }
			set {
				if (throwExceptionOnInvalidFormatSwitch == value)
					return;
				throwExceptionOnInvalidFormatSwitch = value;
				OnChanged("ThrowExceptionOnInvalidFormatSwitch", !value, value);
			}
		}
		#endregion
		public bool UpdateFieldsInTextBoxes {
			get { return updateFieldsInTextBoxes; }
			set {
				if (updateFieldsInTextBoxes == value)
					return;
				updateFieldsInTextBoxes = value;
				OnChanged("UpdateFieldsInTextBoxes", !value, value);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			this.HighlightMode = defaultHighlightMode;
			this.HighlightColor = defaultHighlightColor;
			this.UpdateDocVariablesBeforeCopy = true;
			this.UpdateDocVariablesBeforePrint = true;
			this.UseCurrentCultureDateTimeFormat = false;
			this.UpdateFieldsOnPaste = true;
			this.UpdateLockedFields = defaultUpdateLockedFields;		 
			this.ThrowExceptionOnInvalidFormatSwitch = true;
			this.ClearUnhandledDocVariableField = true;
			this.UpdateFieldsInTextBoxes = false;
		}
		protected internal virtual void CopyFrom(FieldOptions options) {
			this.HighlightColor = options.highlightColor;
			this.HighlightMode = options.highlightMode;
			this.UseCurrentCultureDateTimeFormat = options.useCurrentCultureDateTimeFormat;
			this.UpdateDocVariablesBeforeCopy = options.updateDocVariablesBeforeCopy;
			this.UpdateDocVariablesBeforePrint = options.updateDocVariablesBeforePrint;
			this.UpdateFieldsOnPaste = options.updateFieldsOnPaste;
			this.UpdateLockedFields = options.updateLockedFields;
			this.ThrowExceptionOnInvalidFormatSwitch = options.throwExceptionOnInvalidFormatSwitch;
			this.ClearUnhandledDocVariableField = options.clearUnhandledDocVariableField;
			this.UpdateFieldsInTextBoxes = options.updateFieldsInTextBoxes;
		}
	}
	#endregion
}
