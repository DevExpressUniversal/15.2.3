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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	[ComVisible(true)]
	public class RichEditMailMergeOptions : RichEditNotificationOptions {
		object dataSource;
		string dataMember = String.Empty;
		bool viewMergedData;
		int activeRecord;
		bool keepLastParagraph;
		MailMergeCustomSeparators customSeparators = new MailMergeCustomSeparators();
		public RichEditMailMergeOptions() {		   
		}
		#region DataSource
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditMailMergeOptionsDataSource"),
#endif
DefaultValue(null)]
#if !SL && !DXPORTABLE
		[AttributeProvider(typeof(IListSource))]
#endif
		[NotifyParentProperty(true)]
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if (dataSource == value)
					return;
				object oldDataSource = dataSource;
				dataSource = value;
				OnChanged("DataSource", oldDataSource, value);
			}
		}
		#endregion
		#region DataMember
#if !SL && !DXPORTABLE
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditMailMergeOptionsDataMember"),
#endif
Editor(ControlConstants.DataMemberEditor, typeof(System.Drawing.Design.UITypeEditor))]
#endif
		[DefaultValue(""),
		NotifyParentProperty(true)]
		public virtual string DataMember {
			get { return dataMember; }
			set {
				if (value == null)
					value = String.Empty;
				if (value == dataMember)
					return;
				string oldDataMember = dataMember;
				dataMember = value;
				OnChanged("DataMember", oldDataMember, value);
			}
		}
		#endregion
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditMailMergeOptionsViewMergedData"),
#endif
DefaultValue(false)]
		public bool ViewMergedData {
			get { return viewMergedData; }
			set {
				if (value == viewMergedData)
					return;
				viewMergedData = value;
				OnChanged("ViewMergedData", !value, value);
			}
		}		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public int ActiveRecord {
			get { return activeRecord; }
			set {
				if (activeRecord == value)
					return;
				int oldActiveRecord = activeRecord;
				activeRecord = value;
				OnChanged("ActiveRecord", oldActiveRecord, activeRecord);
			}
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichEditMailMergeOptionsKeepLastParagraph")]
#endif
		[DefaultValue(false)]
		public bool KeepLastParagraph {
			get { return keepLastParagraph; }
			set { keepLastParagraph = value; }
		}
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RichEditMailMergeOptionsCustomSeparators"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MailMergeCustomSeparators CustomSeparators { get { return customSeparators; } }
		protected internal override void ResetCore() {
			this.DataSource = null;
			this.DataMember = String.Empty;
			this.ViewMergedData = false;
			this.KeepLastParagraph = false;
			this.ActiveRecord = 0;
			this.CustomSeparators.Clear();
		}
	}
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	[ComVisible(true)]
	public class MailMergeCustomSeparators {
		string maskGroupSeparator;
		string maskDecimalSeparator;
		string fieldResultGroupSeparator;
		string fieldResultDecimalSeparator;
		public MailMergeCustomSeparators() { }
		public MailMergeCustomSeparators(string maskGroupSeparator, string maskDecimalSeparator, string fieldResultGroupSeparator, string fieldResultDecimalSeparator) {
			this.maskGroupSeparator = maskGroupSeparator;
			this.maskDecimalSeparator = maskDecimalSeparator;
			this.fieldResultGroupSeparator = fieldResultGroupSeparator;
			this.fieldResultDecimalSeparator = fieldResultDecimalSeparator;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeCustomSeparatorsFieldResultDecimalSeparator")]
#endif
		public string FieldResultDecimalSeparator {
			get { return fieldResultDecimalSeparator; }
			set { fieldResultDecimalSeparator = value; }
		}
		void ResetFieldResultDecimalSeparator() {
			FieldResultDecimalSeparator = String.Empty;
		}
		bool ShouldSerializeFieldResultDecimalSeparator() {
			return !String.IsNullOrEmpty(FieldResultDecimalSeparator);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeCustomSeparatorsFieldResultGroupSeparator")]
#endif
		public string FieldResultGroupSeparator {
			get { return fieldResultGroupSeparator; }
			set { fieldResultGroupSeparator = value; }
		}
		void ResetFieldResultGroupSeparator() {
			FieldResultGroupSeparator = String.Empty;
		}
		bool ShouldSerializeFieldResultGroupSeparator() {
			return !String.IsNullOrEmpty(FieldResultGroupSeparator);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeCustomSeparatorsMaskDecimalSeparator")]
#endif
		public string MaskDecimalSeparator {
			get { return maskDecimalSeparator; }
			set { maskDecimalSeparator = value; }
		}
		void ResetMaskDecimalSeparator() {
			MaskDecimalSeparator = String.Empty;
		}
		bool ShouldSerializeMaskDecimalSeparator() {
			return !String.IsNullOrEmpty(MaskDecimalSeparator);
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("MailMergeCustomSeparatorsMaskGroupSeparator")]
#endif
		public string MaskGroupSeparator {
			get { return maskGroupSeparator; }
			set { maskGroupSeparator = value; }
		}
		void ResetMaskGroupSeparator() {
			MaskGroupSeparator = String.Empty;
		}
		bool ShouldSerializeMaskGroupSeparator() {
			return !String.IsNullOrEmpty(MaskGroupSeparator);
		}
		public void Assign(MailMergeCustomSeparators other) {
			this.MaskGroupSeparator = other.MaskGroupSeparator;
			this.FieldResultGroupSeparator = other.FieldResultGroupSeparator;
			this.MaskDecimalSeparator = other.MaskDecimalSeparator;
			this.FieldResultDecimalSeparator = other.FieldResultDecimalSeparator;
		}
		public void Clear() {
			this.MaskGroupSeparator = string.Empty;
			this.FieldResultGroupSeparator = string.Empty;
			this.MaskDecimalSeparator = string.Empty;
			this.FieldResultDecimalSeparator = string.Empty;
		}
		public bool IsEmpty() {
			return String.IsNullOrEmpty(this.MaskGroupSeparator) && String.IsNullOrEmpty(this.MaskDecimalSeparator) && String.IsNullOrEmpty(this.FieldResultGroupSeparator) && String.IsNullOrEmpty(this.FieldResultDecimalSeparator);
		}
	}
}
