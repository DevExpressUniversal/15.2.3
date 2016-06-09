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
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Services;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelProtectedRangeCollection
	public class ModelProtectedRangeCollection : SimpleCollection<ModelProtectedRange> {
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			foreach (ModelProtectedRange currentProtectedRange in this)
				currentProtectedRange.OnRangeInserting(notificationContext);
		}
		public void OnRemoving(RemoveRangeNotificationContext notificationContext) {
			if (notificationContext.Mode == RemoveCellMode.Default)
				return;
			for (int i = Count - 1; i >= 0; i--) {
				ModelProtectedRange currentRange = this[i];
				if (notificationContext.Range.Includes(currentRange.CellRange))
					this.RemoveAt(i);
				else currentRange.OnRangeRemoving(notificationContext);
			}
		}
		public List<ModelProtectedRange> FindAll(Predicate<ModelProtectedRange> match) {
			return this.InnerList.FindAll(match);
		}
		public int FindIndex(Predicate<ModelProtectedRange> match) {
			return InnerList.FindIndex(match);
		}
		public ModelProtectedRange LookupProtectedRange(CellPosition position) {
			return InnerList.Find((item) => item.CellRange.ContainsCell(position.Column, position.Row));
		}
		public List<ModelProtectedRange> LookupProtectedRanges(CellPosition position) {
			return this.FindAll((item) => item.CellRange.ContainsCell(position.Column, position.Row));
		}
		public List<ModelProtectedRange> LookupProtectedRangesContainingEntireRange(CellRange range) {
			return this.FindAll((item) =>  item.CellRange.Includes(range) );
		}
		public int LookupProtectedRangeIndex(string name) {
			return InnerList.FindIndex((item) => item.Name == name);
		}
		public void Reset() {
			InnerList.ForEach(item => item.Reset());
		}
		public void RemoveProtectedRangeTODO(ModelProtectedRange item) {
			this.InnerList.Remove(item);
		}
	}
	#endregion
	#region ModelProtectedRange
	public class ModelProtectedRange : ISupportsCopyFrom<ModelProtectedRange> {
		#region Fields
		string name;
		CellRangeBase cellRange;
		string securityDescriptor;
		bool? isAccessGranted;
		ProtectionCredentials credentials;
		#endregion
		public ModelProtectedRange(string name, CellRangeBase cellRange) {
			this.name = name;
			this.cellRange = cellRange;
			this.credentials = ProtectionCredentials.NoProtection;
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public CellRangeBase CellRange { get { return cellRange; } set { cellRange = value; } }
		public ProtectionCredentials Credentials { get { return credentials; } set { credentials = value; } }
		public string SecurityDescriptor {
			get { return securityDescriptor; }
			set {
				if (securityDescriptor == value)
					return;
				securityDescriptor = value;
				Reset();
			}
		}
		#endregion
		public bool IsAccessGranted {
			get {
				 if (credentials.IsEmpty)
					isAccessGranted = true;
				if (!isAccessGranted.HasValue || !isAccessGranted.Value)
					isAccessGranted = IsAccessGrantedCore();
				return isAccessGranted.Value;
			}
			set {
				isAccessGranted = value;
			}
		}
		bool IsAccessGrantedCore() {
			DocumentModel documentModel = cellRange.Worksheet.Workbook as DocumentModel;
			if (documentModel == null)
				return false;
			IRangeSecurityService service = documentModel.GetService<IRangeSecurityService>();
			if (service == null)
				return false;
			try {
				return service.CheckAccess(securityDescriptor);
			}
			catch {
				return false;
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext notificationContext) {
			ModifyingUnionRangeProcessor processor = ModifyingUnionRangeProcessor.GetProcessor(notificationContext);
			if (processor != null)
				this.cellRange = processor.ProcessRange(this.cellRange);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext notificationContext) {
			ModifyingUnionRangeProcessor processor = ModifyingUnionRangeProcessor.GetProcessor(notificationContext);
			if (processor != null)
				this.cellRange = processor.ProcessRange(this.cellRange);
		}
		public void CopyFrom(ModelProtectedRange source) {
			Name = source.name;
			this.credentials = source.credentials.Clone();
			SecurityDescriptor = source.securityDescriptor;
		}
		public override bool Equals(object obj) {
			ModelProtectedRange other = obj as ModelProtectedRange;
			if (other == null)
				return false;
			return string.Equals(Name, other.Name) && CellRange.Equals(other.CellRange) && Credentials.Equals(other.Credentials);
		}
		public override int GetHashCode() {
			if (string.IsNullOrEmpty(Name))
				return 0;
			return Name.GetHashCode();
		}
		public void Reset() {
			isAccessGranted = null;
		}
		public static bool IsValidName(string value) {
			if (string.IsNullOrEmpty(value))
				return true;
			if (!char.IsLetter(value[0]) && value[0] != '_')
				return false;
			for (int i = 1; i < value.Length; i++) {
				char curChar = value[i];
				if (!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '.' && curChar != ' ')
					return false;
			}
			return true;
		}
	}
	#endregion
}
