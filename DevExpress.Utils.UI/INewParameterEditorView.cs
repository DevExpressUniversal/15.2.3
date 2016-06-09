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
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Parameters;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public interface INewParameterEditorView {
		string Name { get; set; }
		string Description { get; set; }
		Type Type { get; set; }
		object DefaultValue { get; set; }
		bool ShowAtParametersPanel { get; set; }
		bool StandardValuesSupported { get; set; }
		bool MultiValue { get; set; }
		LookUpSettingsTab LookUpSettingsActiveTab { get; set; }
		object DataSource { get; set; }
		object DataAdapter { get; set; }
		string ValueMember { get; set; }
		string DataMember { get; set; }
		string DisplayMember { get; set; }
		string FilterString { get; set; }
		IList LookUpValues { get; set; }
		event EventHandler Submit;
		event EventHandler ShowAtParametersPanelChanged;
		event EventHandler StandardValuesSupportedChanged;
		event EventHandler MultiValueChanged;
		event EventHandler<ValidationEventArgs> ValidateName;
		event EventHandler TypeChanged;
		event EventHandler DataSourceChanged;
		event EventHandler ValueMemberChanged;
		event EventHandler DisplayMemberChanged;
		event EventHandler LookUpValuesChanged;
		event EventHandler FilterStringChanged;
		event EventHandler ActiveTabChanged;
		void EnableLookUpSettings(bool enable, bool enableStandardValuesSupported);		
		void EnableSubmit(bool enable);
		void PopulateTypes(Dictionary<Type, string> availableTypes);
		void SetEditType(Type type, bool multiValue);
	}
	public enum LookUpSettingsTab {
		StaticList,
		DynamicList
	}
	public class ValidationEventArgs : EventArgs {
		public bool IsValid { get; set; }
		public ValidationEventArgs() {
			IsValid = true;
		}
	}
}
