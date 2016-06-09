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

namespace DevExpress.Utils.Filtering.Internal {
	public class FilterRangeEditorSettings {
		public FilterRangeEditorSettings(RangeUIEditorType editorType) {
			NumericEditorType = editorType;
		}
		public FilterRangeEditorSettings(DateTimeRangeUIEditorType editorType) {
			DateTimeEditorType = editorType;
		}
		public RangeUIEditorType? NumericEditorType {
			get;
			private set;
		}
		public DateTimeRangeUIEditorType? DateTimeEditorType {
			get;
			private set;
		}
	}
	public abstract class FilterEditorSettingsBase<TEditorType> where TEditorType : struct {
		public FilterEditorSettingsBase(TEditorType editorType) {
			EditorType = editorType;
		}
		public TEditorType EditorType {
			get;
			private set;
		}
	}
	public class FilterLookupEditorSettings : FilterEditorSettingsBase<LookupUIEditorType> {
		public FilterLookupEditorSettings(LookupUIEditorType editorType, bool useFalgs)
			: base(editorType) {
			UseFlags = useFalgs;
		}
		public bool UseFlags {
			get;
			private set;
		}
	}
	public class FilterBooleanEditorSettings : FilterEditorSettingsBase<BooleanUIEditorType> {
		public FilterBooleanEditorSettings(BooleanUIEditorType editorType)
			: base(editorType) {
		}
	}
	public class FilterEnumEditorSettings : FilterLookupEditorSettings {
		public FilterEnumEditorSettings(LookupUIEditorType editorType, bool useFalgs)
			: base(editorType, useFalgs) {
		}
	}
	public class FilterEditorAttribute : FilterAttribute {
		public FilterEditorAttribute(RangeUIEditorType editorType) {
			RangeEditorSettings = new FilterRangeEditorSettings(editorType);
		}
		public FilterEditorAttribute(DateTimeRangeUIEditorType editorType) {
			RangeEditorSettings = new FilterRangeEditorSettings(editorType);
		}
		public FilterEditorAttribute(LookupUIEditorType editorType, bool useFlags, bool isEnum) {
			if(!isEnum)
				LookupEditorSettings = new FilterLookupEditorSettings(editorType, useFlags);
			else
				EnumEditorSettings = new FilterEnumEditorSettings(editorType, useFlags);
		}
		public FilterEditorAttribute(BooleanUIEditorType editorType) {
			BooleanEditorSettings = new FilterBooleanEditorSettings(editorType);
		}
		public FilterRangeEditorSettings RangeEditorSettings {
			get;
			private set;
		}
		public FilterLookupEditorSettings LookupEditorSettings {
			get;
			private set;
		}
		public FilterBooleanEditorSettings BooleanEditorSettings {
			get;
			private set;
		}
		public FilterEnumEditorSettings EnumEditorSettings {
			get;
			private set;
		}
	}
}
