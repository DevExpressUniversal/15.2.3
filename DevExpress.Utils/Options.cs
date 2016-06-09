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
using System.Collections;
namespace DevExpress.Utils {
	public class SetOptions {
		#region Option functions
		public static ArrayList GetOptionNames(object obj) { 
			ArrayList arr = new ArrayList();
			PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(obj);
			foreach(PropertyDescriptor pd in pds) {
				if(pd.PropertyType.Equals(typeof(bool)) && pd.IsBrowsable) {
					arr.Add(pd.Name);
				}
			}
			arr.Sort();
			return arr;
		}
		public static bool OptionValueByString(string s, object obj) { 
			PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(obj);
			foreach(PropertyDescriptor pd in pds) {
				if(pd.PropertyType.Equals(typeof(bool))) {
					if(pd.Name == s) return (bool)pd.GetValue(obj);
				}
			}
			return false;
		}
		public static void SetOptionValueByString(string s, object obj, bool ch) { 
			PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(obj);
			foreach(PropertyDescriptor pd in pds) {
				if(pd.PropertyType.Equals(typeof(bool))) {
					if(pd.Name == s) {
						pd.SetValue(obj, ch);
						return;
					}
				}
			}
		}
		#endregion
	}
}
