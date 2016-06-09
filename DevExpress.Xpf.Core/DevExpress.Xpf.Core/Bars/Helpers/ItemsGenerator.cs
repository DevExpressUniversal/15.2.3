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

using DevExpress.Entity.Model;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Bars.Helpers {
	public static class PopulateItemsHelper {
		public static T FindOrCreateNew<T>(this IList<T> list, Func<T, bool> predicate, Func<T> createDelegate) where T : class {
			T result = list.FirstOrDefault(predicate);
			if(result == null) {
				result = createDelegate();
				list.Add(result);
			}
			return result;
		}
		public static void GenerateItems(DependencyPropertyChangedEventArgs e, Func<ICommandGroupsGenerator> genCreateCallback) {
			if(e.NewValue != null)
				ViewModelMetadataSource.GenerateMetadata(ViewModelMetadataSource.GetProperties(e.NewValue), genCreateCallback(), ViewModelMetadataOptions.ForRuntime());
		}
	}
	class BarsGenerator : ICommandGroupsGenerator {
		readonly BarManager barManager;
		public BarsGenerator(BarManager barManager) {
			this.barManager = barManager;
		}
		ICommandSubGroupsGenerator ICommandGroupsGenerator.CreateGroup(string groupName) {
			Bar bar = barManager.Bars.FindOrCreateNew(x => x.Caption == groupName, () => new Bar() { Caption = groupName });
			return new BarsGroupsGenerator(bar, barManager.AllowGlyphTheming ? ImageType.GrayScaled : ImageType.Colored);
		}
	}
	class BarsGroupsGenerator : ICommandSubGroupsGenerator {
		readonly Bar bar;
		readonly ImageType imageType;
		readonly BarItemsGenerator gen;
		public BarsGroupsGenerator(Bar bar, ImageType imageType) {
			this.bar = bar;
			this.imageType = imageType;
			this.gen = new BarItemsGenerator(bar, imageType);
		}
		ICommandsGenerator ICommandSubGroupsGenerator.CreateSubGroup(string groupName) {
			if(bar.ItemLinks.Any())
				bar.ItemLinks.Add(new BarItemLinkSeparator());
			return gen;
		}
	}
	public abstract class BarItemsGeneratorBase : ICommandsGenerator {
		class BarItemAttributeApplier : ICommandAttributesApplier {
			static ImageSource GetImageSource(string imageName) {
				return (ImageSource)new ImageSourceConverter().ConvertFrom(imageName);
			}
			readonly BarItem item;
			public BarItemAttributeApplier(BarItem item) {
				this.item = item;
			}
			void ICommandAttributesApplier.SetCaption(string caption) {
				item.Content = caption;
			}
			void ICommandAttributesApplier.SetHint(string hint) {
				item.Hint = hint;
			}
			void ICommandAttributesApplier.SetImageUri(string imageName) {
				item.Glyph = GetImageSource(imageName);
			}
			void ICommandAttributesApplier.SetLargeImageUri(string imageName) {
				item.LargeGlyph = GetImageSource(imageName);
			}
			void ICommandAttributesApplier.SetParameter(string parameterPropertyName) {
				item.SetBinding(BarButtonItem.CommandParameterProperty, new Binding(parameterPropertyName));
			}
			void ICommandAttributesApplier.SetPropertyName(string propertyName) {
				item.SetBinding(BarButtonItem.CommandProperty, new Binding(propertyName));
			}
		}
		readonly ImageType imageType;
		public BarItemsGeneratorBase(ImageType imageType) {
			this.imageType = imageType;
		}
		void ICommandsGenerator.GenerateCommand(IEdmPropertyInfo property) {
			BarItem item = new BarButtonItem();
			PopulateItem(item);
			property.ApplyCommandAttributes(new BarItemAttributeApplier(item), imageType);
		}
		protected abstract void PopulateItem(BarItem item);
	}
	public class BarItemsGenerator : BarItemsGeneratorBase {
		readonly ILinksHolder bar;
		public BarItemsGenerator(ILinksHolder bar, ImageType imageType) 
			: base(imageType) {
			this.bar = bar;
		}
		protected override void PopulateItem(BarItem item) {
			bar.Items.Add(item);
		}
	}
}
