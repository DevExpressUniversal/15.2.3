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

namespace DevExpress.Design.Filtering {
	using System;
	public interface IFilteringModelMetadata {
		PlatformCodeName Platform { get; }
		string ModelTypeProperty { get; }
		string CustomAttributesProperty { get; }
	}
	static class FilteringModelMetadataLoader {
		public static IFilteringModelMetadata Load(Type type) {
			AssertionException.IsNotNull(type);
			return LoadMetadataCore(type) ?? FilteringModelMetadata.Empty;
		}
		static IFilteringModelMetadata LoadMetadataCore(Type type) {
			return Metadata.AttributeHelper.GetAttributeValue<IFilteringModelMetadata, Utils.Design.Filtering.FilteringModelMetadataAttribute>(
				type, (attribute) => new FilteringModelMetadata(GetPlatform(type), attribute));
		}
		static PlatformCodeName GetPlatform(Type type) {
			if(typeof(System.ComponentModel.Component).IsAssignableFrom(type))
				return PlatformCodeName.Win;
			if(typeof(System.Windows.FrameworkElement).IsAssignableFrom(type))
				return PlatformCodeName.Wpf;
			return PlatformCodeName.Unknown;
		}
		class FilteringModelMetadata : IFilteringModelMetadata {
			public static IFilteringModelMetadata Empty = new EmptyFilteringModelMetadata();
			public FilteringModelMetadata(PlatformCodeName platform, Utils.Design.Filtering.FilteringModelMetadataAttribute attribute) {
				if(!string.IsNullOrEmpty(attribute.Platform))
					Platform = GetPlatform(attribute.Platform);
				else
					Platform = platform;
				ModelTypeProperty = "ModelType";
				CustomAttributesProperty = "CustomAttributes";
				if(!string.IsNullOrEmpty(attribute.ModelTypeProperty))
					ModelTypeProperty = attribute.ModelTypeProperty;
				if(!string.IsNullOrEmpty(attribute.CustomAttributesProperty))
					CustomAttributesProperty = attribute.CustomAttributesProperty;
			}
			public PlatformCodeName Platform {
				get;
				private set;
			}
			public string ModelTypeProperty {
				get;
				private set;
			}
			public string CustomAttributesProperty {
				get;
				private set;
			}
			static PlatformCodeName GetPlatform(string platformString) {
				string[] entries = platformString.Split(
					new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
				for(int i = 0; i < entries.Length; i++) {
					PlatformCodeName result;
					if(Enum.TryParse<PlatformCodeName>(entries[i], true, out result))
						return result;
				}
				return PlatformCodeName.Unknown;
			}
			#region Empty
			class EmptyFilteringModelMetadata : IFilteringModelMetadata {
				public PlatformCodeName Platform {
					get { return PlatformCodeName.Unknown; }
				}
				public string ModelTypeProperty {
					get { return null; }
				}
				public string CustomAttributesProperty {
					get { return null; }
				}
			}
			#endregion Empty
		}
	}
}
