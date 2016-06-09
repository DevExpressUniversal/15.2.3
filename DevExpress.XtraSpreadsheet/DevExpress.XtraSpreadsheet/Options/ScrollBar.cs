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
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetScrollbarVisibility
	public enum SpreadsheetScrollbarVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region SpreadsheetScrollbarOptions
	public abstract class SpreadsheetScrollbarOptions : SpreadsheetNotificationOptions {
		#region Fields
		const SpreadsheetScrollbarVisibility defaultVisibility = SpreadsheetScrollbarVisibility.Auto;
		internal const string PropertyName_Visibility = "Visibility";
		SpreadsheetScrollbarVisibility visibility;
		#endregion
		#region Properties
		#region Visibility
		[
#if !SL
	DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetScrollbarOptionsVisibility"),
#endif
 XtraSerializableProperty(), NotifyParentProperty(true)]
		public SpreadsheetScrollbarVisibility Visibility {
			get { return visibility; }
			set {
				if (visibility == value)
					return;
				SpreadsheetScrollbarVisibility oldValue = this.visibility;
				visibility = value;
				OnChanged(PropertyName_Visibility, oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeVisibility() { return Visibility != defaultVisibility; }
		protected internal virtual void ResetVisibility() {
			Visibility = defaultVisibility;
		}
		#endregion
		#endregion
		protected override void ResetCore() {
			ResetVisibility();
		}
		protected internal void CopyFrom(SpreadsheetScrollbarOptions value) {
			this.visibility = value.Visibility;
		}
	}
	#endregion
}
