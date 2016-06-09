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
using DevExpress.Utils.Controls;
using DevExpress.Office;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetNotificationOptions (abstract class)
	public abstract class SpreadsheetNotificationOptions : BaseOptions {
		protected SpreadsheetNotificationOptions() {
			Reset();
		}
		#region Events
		public event BaseOptionChangedEventHandler Changed { add { ChangedCore += value; } remove { ChangedCore -= value; } }
		#endregion
		public override void Reset() {
			BeginUpdate();
			try {
				ResetCore();
			}
			finally {
				EndUpdate();
			}
		}
		protected internal virtual void OnChanged<T>(string name, T oldValue, T newValue) {
			OnChanged(new BaseOptionChangedEventArgs(name, oldValue, newValue));
		}
		protected internal abstract void ResetCore();
		protected internal bool IsAllowed(DocumentCapability option) {
			return option == DocumentCapability.Default || option == DocumentCapability.Enabled;
		}
	}
	#endregion
	#region SpreadsheetProtectionOptions
	[ComVisible(true)]
	public class SpreadsheetProtectionOptions : SpreadsheetNotificationOptions, ISupportsCopyFrom<SpreadsheetProtectionOptions> {
		#region Fields
		const int minSpinCount = 10;
		const int maxSpinCount = 10000000; 
		const int defaultSpinCount = 100000;
		bool useStrongPasswordVerifier = true;
		int spinCount = defaultSpinCount;
		#endregion
		public SpreadsheetProtectionOptions() {
			Reset();
		}
		#region Properties
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionOptionsUseStrongPasswordVerifier"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool UseStrongPasswordVerifier {
			get { return useStrongPasswordVerifier; }
			set {
				if (useStrongPasswordVerifier == value)
					return;
				bool oldValue = this.useStrongPasswordVerifier;
				this.useStrongPasswordVerifier = value;
				OnChanged("UseStrongPasswordVerifier", oldValue, value);
			}
		}
		#region SpinCount
		[
#if !SL
	DevExpressSpreadsheetCoreLocalizedDescription("SpreadsheetProtectionOptionsSpinCount"),
#endif
 DefaultValue(defaultSpinCount), NotifyParentProperty(true)]
		public int SpinCount {
			get { return spinCount; }
			set {
				int newValue = Math.Max(minSpinCount, value);
				newValue = Math.Min(newValue, maxSpinCount);
				if (this.spinCount == newValue)
					return;
				float oldValue = this.spinCount;
				this.spinCount = newValue;
				OnChanged("SpinCount", oldValue, newValue);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			this.useStrongPasswordVerifier = true;
			this.spinCount = defaultSpinCount;
		}
		#region ISupportsCopyFrom<SpreadsheetProtectionOptions> Members
		public void CopyFrom(SpreadsheetProtectionOptions value) {
			this.useStrongPasswordVerifier = value.useStrongPasswordVerifier;
			this.spinCount = value.spinCount;
		}
		#endregion
	}
	#endregion
}
