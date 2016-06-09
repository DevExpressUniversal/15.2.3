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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.ComponentModel.DataAnnotations;
	abstract class EndUserFilteringElement : StatelessObject, IEndUserFilteringElement {
		readonly string pathCore;
		public EndUserFilteringElement(Func<IServiceProvider> getServiceProvider, string path)
			: base(getServiceProvider) {
			pathCore = path;
		}
		public string Path {
			get { return pathCore; }
		}
		protected sealed override string GetId() {
			return pathCore;
		}
		public string Caption {
			get { return GetMetadata<string>(x => x.GetCaption); }
		}
		public string Description {
			get { return GetMetadata<string>(x => x.GetDescription); }
		}
		public string Layout {
			get { return GetMetadata<string>(x => x.GetLayout); }
		}
		public int Order {
			get { return GetMetadata<int>(x => x.GetOrder); }
		}
		public string DataFormatString {
			get { return GetMetadata<string>(x => x.GetDataFormatString); }
		}
		public DataType? DataType {
			get { return GetMetadata<DataType?>(x => x.GetDataType); }
		}
		public Type EnumDataType {
			get { return GetMetadata<Type>(x => x.GetEnumDataType); }
		}
		public bool IsVisible {
			get { return GetMetadata<bool>(x => x.GetIsVisible) && GetBehavior<bool>(x => x.GetIsVisible); }
		}
		public bool IsEnabled {
			get { return GetBehavior<bool>(x => x.GetIsEnabled); }
		}
	}
}
