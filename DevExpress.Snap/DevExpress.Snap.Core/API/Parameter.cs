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
using DevExpress.XtraReports.Parameters;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Office.Utils;
using DevExpress.Snap.Localization;
namespace DevExpress.Snap.Core.API {
	[
#if !SILVERLIGHT
TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI),
#endif
]
	public class Parameter : IParameter {
		Type type;
		object value;
		string name;
		object DefaultValue {
			get {
				return ParameterHelper.GetDefaultValue(Type);
			}
		}
		#region IParameter Members
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("ParameterName"),
#endif
		DefaultValue("")]
		[Category("Design")]
		public string Name {
			get { return name; }
			set {
				ValidateName(value);
				name = value; 
			}
		}
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("ParameterType"),
#endif
#if !SILVERLIGHT
 TypeConverter("DevExpress.XtraReports.Design.ParameterTypeConverter," + AssemblyInfo.SRAssemblyUtilsUI),
#endif
 RefreshProperties(RefreshProperties.All),
	   Category("Data"),
	   ]
		public Type Type {
			get {
				return type ?? typeof(String);
			}
			set {
				type = value;
				if (ParameterHelper.ShouldConvertValue(this.Value, this.Type))
					this.value = ParameterHelper.ConvertFrom(this.Value, this.Type, this.DefaultValue);
			}
		}
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("ParameterValue"),
#endif
		Category("Data"),
		]
		public object Value {
			get { return value ?? DefaultValue; }
			set { this.value = value; }
		}
		#endregion
		public Parameter Clone() {
			Parameter clone = new Parameter();
			clone.name = this.Name;
			clone.type = this.Type;
			clone.value = this.Value;
			return clone;
		}
		void ValidateName(string name) {
			if (string.IsNullOrEmpty(name))
				Exceptions.ThrowInvalidOperationException(SnapLocalizer.GetString(SnapStringId.ParametersErrorNoName));
			string invalidCharactersErrorMsg = SnapLocalizer.GetString(SnapStringId.ParametersErrorInvalidCharacters) + name;
			foreach(char ch in name) 
					if(!DevExpress.Data.Filtering.Helpers.CriteriaLexer.CanContinueColumn(ch))
						Exceptions.ThrowInvalidOperationException(invalidCharactersErrorMsg);
		}
	}
}
