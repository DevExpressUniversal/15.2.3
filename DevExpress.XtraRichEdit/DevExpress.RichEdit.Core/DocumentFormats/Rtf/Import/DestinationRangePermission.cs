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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Export.Rtf;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region RangePermissionDestinationBase (abstract class)
	public abstract class RangePermissionDestinationBase : StringValueDestination {
		protected RangePermissionDestinationBase(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		public override void AfterPopRtfState() {
			string data = Value.Trim();
			if (!String.IsNullOrEmpty(data) && data.Length == 16) {
				ImportRangePermissionInfo rangePermission;
				if (!Importer.RangePermissions.TryGetValue(data, out rangePermission)) {
					rangePermission = new ImportRangePermissionInfo();
					rangePermission.PermissionInfo.UserName = ObtainUserName(data);
					rangePermission.PermissionInfo.Group = ObtainGroupName(data);
					Importer.RangePermissions.Add(data, rangePermission);
				}
				AssignRangePermissionPosition(rangePermission);
			}
		}
		protected internal virtual string ObtainUserName(string data) {
			int value = ObtainUserId(data);
			return Importer.GetUserNameById(value);
		}
		protected internal virtual string ObtainGroupName(string data) {
			int value = ObtainUserId(data);
			string result;
			if (RtfContentExporter.PredefinedUserGroups.TryGetValue(value, out result))
				return result;
			else
				return String.Empty;
		}
		protected internal int ObtainUserId(string data) {
			int valueLow;
			if (!Int32.TryParse(data.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out valueLow))
				return int.MinValue;
			int valueHigh;
			if (!Int32.TryParse(data.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out valueHigh))
				return int.MinValue;
			return (valueHigh << 8) | valueLow;
		}
		protected internal abstract void AssignRangePermissionPosition(ImportRangePermissionInfo rangePermission);
	}
	#endregion
	#region RangePermissionStartDestination
	public class RangePermissionStartDestination : RangePermissionDestinationBase {
		public RangePermissionStartDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new RangePermissionStartDestination(Importer);
		}
		protected internal override void AssignRangePermissionPosition(ImportRangePermissionInfo rangePermission) {
			rangePermission.Start = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region RangePermissionEndDestination
	public class RangePermissionEndDestination : RangePermissionDestinationBase {
		public RangePermissionEndDestination(RtfImporter rtfImporter)
			: base(rtfImporter) {
		}
		protected internal override StringValueDestination CreateEmptyClone() {
			return new RangePermissionEndDestination(Importer);
		}
		protected internal override void AssignRangePermissionPosition(ImportRangePermissionInfo rangePermission) {
			rangePermission.End = Importer.Position.LogPosition;
		}
	}
	#endregion
}
