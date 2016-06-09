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
using System.Drawing;
using System.Linq;
using System.Windows;
using DevExpress.Utils;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Xpf.Printing.BrickCollection {
	public class BrickCollector {
		#region Fields & Properties
		readonly PrintingSystemBase printingSystem;
		readonly Dictionary<BrickStyleKey, BrickStyle> brickStyles = new Dictionary<BrickStyleKey, BrickStyle>();
		readonly Dictionary<TargetType, IBrickCreator> brickCreators = new Dictionary<TargetType, IBrickCreator>();
		readonly Dictionary<IVisualBrick, IOnPageUpdater> onPageUpdaters = new Dictionary<IVisualBrick, IOnPageUpdater>();
		readonly Dictionary<string, BookmarkInfo> bookmarkInfos = new Dictionary<string, BookmarkInfo>();
		IVisualTreeWalker visualTreeWalker; 
		internal IVisualTreeWalker VisualTreeWalker {
			get { return visualTreeWalker; }
			set {
				Guard.ArgumentNotNull(value, "value");
				visualTreeWalker = value;
			}
		}
		internal Dictionary<IVisualBrick, IOnPageUpdater> BrickUpdaters {
			get { return onPageUpdaters; }
		}
		#endregion
		#region Constructors
		public BrickCollector(PrintingSystemBase printingSystem) {
			Guard.ArgumentNotNull(printingSystem, "printingSystem");
			this.printingSystem = printingSystem;
			brickCreators.Add(TargetType.None, new NoneBrickCreator());
			brickCreators.Add(TargetType.Text, new TextBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.Panel, new PanelBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.Image, new ImageBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.Boolean, new CheckBoxBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.PageNumber, new PageInfoBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.ProgressBar, new ProgressBarBrickCreator(printingSystem, brickStyles, onPageUpdaters));
			brickCreators.Add(TargetType.TrackBar, new TrackBarBrickCreator(printingSystem, brickStyles, onPageUpdaters));
		}
		#endregion
		#region Methods
		public BrickCollectionBase ToBricks(FrameworkElement container, out float containerBrickHeight) {
			Guard.ArgumentNotNull(container, "container");
			var rootBrickContainer = new PanelBrick { Style = new BrickStyle() };
			var rootBrickContainerBounds = GraphicsUnitConverter2.PixelToDoc(new RectangleF(0f, 0f, (float)container.ActualWidth, (float)container.ActualHeight));
			rootBrickContainer.Initialize(printingSystem, rootBrickContainerBounds);
			CollectBricks(container, container, rootBrickContainer);
			containerBrickHeight = rootBrickContainer.Height;
			return rootBrickContainer.Bricks;
		}
		public void Clear() {
			brickStyles.Clear();
			onPageUpdaters.Clear();
			bookmarkInfos.Clear();
		}
#if DEBUGTEST
		internal BrickCollectionBase Test_ToBricks(FrameworkElement container) {
			float ignore;
			return ToBricks(container, out ignore);
		}
#endif
		void CollectBricks(DependencyObject currentItem, DependencyObject parent, PanelBrick brickContainer) {
			VisualBrick brick = null;
			UIElement currentItemUIElement = currentItem as UIElement;
			UIElement parentUIElement = parent as UIElement;
			if(currentItemUIElement != null && parentUIElement != null) {
				var targetType = GetEffectiveTargetType(currentItem);
				var brickCreator = GetBrickCreator(targetType);
				brick = brickCreator.Create(currentItemUIElement, parentUIElement);
				if(brick != null)
					brickContainer.Bricks.Add(brick);
			}
			FrameworkElement currentItemFrameworkElement = currentItem as FrameworkElement;
			if(currentItemFrameworkElement != null) {
				string bookmark = ExportSettings.GetBookmark(currentItemFrameworkElement);
				if(!string.IsNullOrEmpty(bookmark))
					brick.BookmarkInfo = GetBookmarkInfo(currentItemFrameworkElement, bookmark);
			}
			if(currentItemUIElement != null && currentItemUIElement.Visibility == Visibility.Collapsed)
				return;
			int childrenCount = VisualTreeWalker.GetChildrenCount(currentItem);
			for(int childIndex = 0; childIndex < childrenCount; childIndex++) {
				DependencyObject child = VisualTreeWalker.GetChild(currentItem, childIndex);
				PanelBrick nextBrickContainer = brick as PanelBrick;
				if(nextBrickContainer != null)
					CollectBricks(child, currentItem, nextBrickContainer);
				else if(brick == null)
					CollectBricks(child, parent, brickContainer);
			}
		}
		internal static TargetType GetEffectiveTargetType(DependencyObject item) {
			return GetAttachedTargetType(item) ?? GetInheritedTargetType(item);
		}
		internal static TargetType? GetAttachedTargetType(DependencyObject item) {
			var valueSource = DependencyPropertyHelper.GetValueSource(item, ExportSettings.TargetTypeProperty);
			if(valueSource.BaseValueSource > BaseValueSource.Default || ExportSettings.GetPropertiesHintMask(item).HasFlag(ExportSettingsProperties.TargetType))
				return ExportSettings.GetTargetType(item);
			return null;
		}
		internal static TargetType GetInheritedTargetType(DependencyObject item) {
			if(item is IPageNumberExportSettings)
				return TargetType.PageNumber;
			if(item is IProgressBarExportSettings)
				return TargetType.ProgressBar;
			if(item is ITrackBarExportSettings)
				return TargetType.TrackBar;
			if(item is ITextExportSettings)
				return TargetType.Text;
			if(item is IImageExportSettings)
				return TargetType.Image;
			if(item is IBooleanExportSettings)
				return TargetType.Boolean;
			return TargetType.None;
		}
		internal IBrickCreator GetBrickCreator(TargetType targetType) {
			return brickCreators[targetType];
		}
		BookmarkInfo GetBookmarkInfo(FrameworkElement item, string bookmark) {
			BookmarkInfo parentBookmarkInfo = GetParentBookmarkInfo(item);
			var bookmarkInfo = new BookmarkInfo(NullBrickOwner.Instance, bookmark, parentBookmarkInfo);
			if(item.Name != null)
				bookmarkInfos[item.Name] = bookmarkInfo;
			return bookmarkInfo;
		}
		BookmarkInfo GetParentBookmarkInfo(DependencyObject item) {
			string parentBookmarkName = ExportSettings.GetBookmarkParentName(item);
			BookmarkInfo parentBookmarkInfo = null;
			if(parentBookmarkName != null && !bookmarkInfos.TryGetValue(parentBookmarkName, out parentBookmarkInfo))
				throw new InvalidOperationException(string.Format("No parent with the {0} name was found for the bookmark.", parentBookmarkName));
			return parentBookmarkInfo;
		}
		#endregion
	}
}
