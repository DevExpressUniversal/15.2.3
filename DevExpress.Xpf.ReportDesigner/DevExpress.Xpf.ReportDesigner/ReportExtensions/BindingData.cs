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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions {
	public class BindingData {
		public BindingData(object source, string member) {
			Guard.ArgumentNotNull(member, "member");
			this.source = source;
			this.member = member;
		}
		readonly object source;
		public object Source { get { return source; } }
		readonly string member;
		public string Member { get { return member; } }
		public XRBinding GetXRBinding(string propertyName, string formatString) {
			var parameter = source as Parameter;
			if(parameter != null)
				return new XRBinding(parameter, propertyName, formatString);
			return new XRBinding(propertyName, source, member, formatString);
		}
		public override string ToString() {
			return Member;
		}
		#region Equality
		public override int GetHashCode() {
			return (source ?? 0).GetHashCode() ^ member.GetHashCode();
		}
		public static bool operator ==(BindingData a, BindingData b) {
			bool aIsNull = ReferenceEquals(a, null);
			bool bIsNull = ReferenceEquals(b, null);
			if(aIsNull && bIsNull) return true;
			if(aIsNull || bIsNull) return false;
			return Equals(a.Source, b.Source) && string.Equals(a.Member, b.Member, StringComparison.Ordinal);
		}
		public static bool operator !=(BindingData a, BindingData b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			return this == obj as BindingData;
		}
		#endregion
	}
}
