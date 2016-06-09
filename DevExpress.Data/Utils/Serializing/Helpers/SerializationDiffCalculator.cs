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
namespace DevExpress.Utils.Serializing.Helpers {
	public class SerializationDiffCalculator {
		public static XtraPropertyInfo[] CalculateDiff(XtraPropertyInfo[] prevSnapShot, XtraPropertyInfo[] currentSnapShot) {
			XtraPropertyCollection prev = new XtraPropertyCollection();
			prev.AddRange(prevSnapShot);
			XtraPropertyCollection current = new XtraPropertyCollection();
			current.AddRange(currentSnapShot);
			XtraPropertyInfoCollection result = CalculateDiffCore(prev, current);
			return result.ToArray();
		}
		public static XtraPropertyInfoCollection CalculateDiffCore(IXtraPropertyCollection prevSnapShot, IXtraPropertyCollection currentSnapShot) {
			XtraPropertyInfoCollection result = new XtraPropertyInfoCollection();
			int count = currentSnapShot.Count;
			if (prevSnapShot.Count == count) {
				for (int i = 0; i < count; i++)
					result.AddRange(CalculatePropertyDiff(prevSnapShot[i], currentSnapShot[i]));
			} else {
				for (int i = 0; i < count; i++)
					result.Add(currentSnapShot[i]);
			}
			return result;
		}
		protected internal static XtraPropertyInfoCollection CalculatePropertyDiff(XtraPropertyInfo prev, XtraPropertyInfo current) {
			System.Diagnostics.Debug.Assert(prev.Name == current.Name);
			if(prev.ChildProperties != null) {
				System.Diagnostics.Debug.Assert(current.ChildProperties != null);
				if(prev.Value == null) { 
					System.Diagnostics.Debug.Assert(current.Value == null);
					XtraPropertyInfoCollection result = CalculateDiffCore(prev.ChildProperties, current.ChildProperties);
					if(result.Count > 0) {
						XtraPropertyInfo propertyInfo = new XtraPropertyInfo(current.Name, current.PropertyType, current.Value, current.IsKey);
						System.Diagnostics.Debug.Assert(propertyInfo.ChildProperties != null);
						propertyInfo.ChildProperties.AddRange(result.ToArray());
						result.Clear();
						result.Add(propertyInfo);
						return result;
					} else
						return result;
				} else { 
					System.Diagnostics.Debug.Assert(prev.Value.GetType() == typeof(int));
					System.Diagnostics.Debug.Assert(current.Value.GetType() == typeof(int));
					if(!Object.Equals(prev.Value, current.Value)) {
						XtraPropertyInfoCollection result = new XtraPropertyInfoCollection();
						result.Add(current);
						return result;
					} else {
						XtraPropertyInfoCollection result = CalculateDiffCore(prev.ChildProperties, current.ChildProperties);
						if(result.Count > 0) {
							result = new XtraPropertyInfoCollection();
							result.Add(current);
							return result;
						} else
							return new XtraPropertyInfoCollection();
					}
				}
			} else { 
				if(!Object.Equals(prev.Value, current.Value)) {
					XtraPropertyInfoCollection result = new XtraPropertyInfoCollection();
					result.Add(current);
					return result;
				} else
					return new XtraPropertyInfoCollection();
			}
		}
	}
}
