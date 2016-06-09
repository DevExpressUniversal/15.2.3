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

using System.Collections.Generic;
using DevExpress.Xpo;
namespace DevExpress.XtraReports.Service.Native.DAL {
	[NonPersistent]
	public abstract class SmartXPObject : ExtendedXPObject, IEnumerableChangedProperties {
		readonly List<string> changedProperties = new List<string>(1);
#if DEBUGTEST
		internal string[] ChangedProperties_DEBUG {
			get { return changedProperties.ToArray(); }
		}
#endif
		protected SmartXPObject(Session session)
			: base(session) {
		}
		public void SmartSave() {
			if(Session.IsNewObject(this) || changedProperties.Count > 0) {
				Save();
			}
		}
		protected override void OnSaved() {
			base.OnSaved();
			changedProperties.Clear();
		}
		protected override void OnChangedProperty(string propertyName) {
			if(!IsLoading && !changedProperties.Contains(propertyName)) {
				changedProperties.Add(propertyName);
			}
		}
		#region IEnumerateChangedProperties Members
		IList<string> IEnumerableChangedProperties.ChangedProperties {
			get { return changedProperties; }
		}
		#endregion
	}
}
