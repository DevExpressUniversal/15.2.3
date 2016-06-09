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

using DevExpress.Map.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
namespace DevExpress.Xpf.Map {
	public class ShapefileDataAdapter : CoordinateSystemDataAdapterBase {
		public static readonly DependencyProperty FileUriProperty = DependencyPropertyManager.Register("FileUri",
			typeof(Uri), typeof(ShapefileDataAdapter), new PropertyMetadata(null, UpdateData));
		public static readonly DependencyProperty DefaultEncodingNameProperty = DependencyPropertyManager.Register("DefaultEncodingName",
			typeof(string), typeof(ShapefileDataAdapter), new PropertyMetadata(null, UpdateData));
		[Category(Categories.Data)]
		public Uri FileUri {
			get { return (Uri)GetValue(FileUriProperty); }
			set { SetValue(FileUriProperty, value); }
		}
		[Category(Categories.Data)]
		public string DefaultEncodingName {
			get { return (string)GetValue(DefaultEncodingNameProperty); }
			set { SetValue(DefaultEncodingNameProperty, value); }
		}
		static void UpdateData(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ShapefileDataAdapter dataAdapter = d as ShapefileDataAdapter;
			if (dataAdapter != null && dataAdapter.Layer != null)
				dataAdapter.LoadDataInternal();
		}
		public event ShapesLoadedEventHandler ShapesLoaded;
#if DEBUGTEST
		internal static string Trace { get; set; }
#endif
		CoordBounds boundingRect = CoordBounds.Empty;
		readonly IFileDataAdapter fileAdapter;
		internal CoordBounds BoundingRect { get { return boundingRect; } }
		protected XpfShapeFileLoader ItemsLoader { get { return (XpfShapeFileLoader)fileAdapter.ItemsLoader; } }
		protected internal override MapVectorItemCollection ItemsCollection { get { return fileAdapter.ItemsCollection; } }
		protected override bool CanLoadData { get { return base.CanLoadData && FileUri != null; } }
		public ShapefileDataAdapter() {
			fileAdapter = new FileDataAdapter(new XpfShapeFileLoader { CoordinateSystem = SourceCoordinateSystem.CoordSystemCore });
			fileAdapter.ItemsLoaded += ItemsLoaded;
			fileAdapter.BoundsCalculated += BoundsCalculated;
		}
		static SourceCoordinateSystem WrapCoreCoordinateSystem(CoordSystemCore coordSystem) {
			CartesianCoordSystemCore cartesianCS = coordSystem as CartesianCoordSystemCore;
			if(cartesianCS != null)
				return new CartesianSourceCoordinateSystem(cartesianCS);
			return null;
		}
		internal static string GetFileContent(Uri uri) {
			ResourceLoaderBehaviorBase resourceLoaderBehavior = WpfResourceLoaderBehaviorBase.GetResourceLoaderBehavior(uri);
			if (resourceLoaderBehavior != null)
				return LoadFromResource(uri, resourceLoaderBehavior);
			else if (uri.ToString().ToLower().Contains(@"file://"))
				return LoadFromFile(uri);
			return string.Empty;
		}
		static string LoadFromResource(Uri uri, ResourceLoaderBehaviorBase resourceLoaderBehavior) {
			using(Stream stream = resourceLoaderBehavior.OpenResource(uri)) {
				if(stream == null)
					return string.Empty;
				using(StreamReader streamReader = new StreamReader(stream)) {
					return streamReader.ReadToEnd();
				}
			}
		}
		static string LoadFromFile(Uri uri) {
			try {
				if (!File.Exists(uri.LocalPath))
					return string.Empty;
				using (StreamReader stream = new StreamReader(uri.LocalPath)) {
					return stream.ReadToEnd();
				}
			}
			catch { return string.Empty; }
		}
		public static SourceCoordinateSystem LoadPrjFile(Uri prjFileUri) {
#if DEBUGTEST
			Trace = prjFileUri != null ? prjFileUri.ToString() : "null";
#endif
			string content = GetFileContent(prjFileUri);
			if(!string.IsNullOrEmpty(content)) {
				ProjectionFileParser parser = new ProjectionFileParser(GeoPointFactory.Instance, CartesianPointFactory.Instance);
				CoordSystemCore coordSystem = parser.Parse(content);
				return WrapCoreCoordinateSystem(coordSystem);
			}
			return null;
		}
		Uri GetPrjUri(Uri uri) {
			string text = uri != null ? uri.ToString().ToLower() : string.Empty;
			string ext = Path.GetExtension(text);
			return new Uri(string.IsNullOrEmpty(ext) ? text + ".prj" : text.Replace(ext, ".prj"), UriKind.RelativeOrAbsolute);
		}
		void CopyItems(IList<MapItem> source, MapVectorItemCollection target) {
			target.Clear();
			foreach (MapItem item in source)
				target.Insert(0, item);
		}
		Encoding ObtainDefaultEncoding() {
			return DefaultEncodingName != null ? Encoding.GetEncoding(DefaultEncodingName) : DXEncoding.Default;
		}
		protected void LoadItems() {
			try {
				PrepareInnerLoader();
				ItemsLoader.Load(FileUri, DesignerProperties.GetIsInDesignMode(this));
			} catch (InconsistentDbfException) {
				throw new Exception(DXMapStrings.MsgDisagreeDbfFIle);
			} catch (IncorrectUriException) {
				throw new Exception(DXMapStrings.MsgIncorrectShpFIleUri);
			}
		}
		protected void PrepareInnerLoader() {
			SourceCoordinateSystem actualCoordinateSystem = GetActualCoordinateSystem();
			ItemsLoader.CoordinateSystem = actualCoordinateSystem.CoordSystemCore;
			ItemsLoader.DefaultEncoding = ObtainDefaultEncoding();
		}
		protected void ItemsLoaded(object sender, ItemsLoadedEventArgs<MapItem> e) {
			if (ShapesLoaded != null)
				ShapesLoaded(this, new ShapesLoadedEventArgs(ItemsLoader.Items));
		}
		protected void BoundsCalculated(object sender, BoundsCalculatedEventArgs e) {
			boundingRect = e.Bounds;
			if (Layer != null)
				Layer.UpdateBoundingRect();
		}
		protected override MapDependencyObject CreateObject() {
			return new ShapefileDataAdapter();
		}
		protected internal override SourceCoordinateSystem GetActualCoordinateSystem() {
			if (!SourceCoordinateSystem.IsDefault)
				return SourceCoordinateSystem;
			if (FileUri == null)
				return DefaultSourceCoordinateSystem;
			SourceCoordinateSystem sourceCS = LoadPrjFile(GetPrjUri(FileUri));
			return sourceCS ?? DefaultSourceCoordinateSystem;
		}
		protected internal override void LoadDataInternal(bool publicCall) {
			if (publicCall)
				PrepareInnerLoader();
			base.LoadDataInternal(publicCall);
		}
		protected override void LoadDataCore() {
			if (FileUri != null)
				LoadItems();
		}
		public void LoadFromStream(Stream shpStream, Stream dbfStream) {
			if(CommonUtils.IsEmptyStream(shpStream))
				return;
			PrepareInnerLoader();
			ItemsLoader.Load(shpStream, dbfStream);
			if (Layer != null)
				Layer.OnDataLoaded();
		}
		public override object GetItemSourceObject(MapItem item) {
			return item;
		}
		#region Obsolete members
		[
		Obsolete("The parameterless LoadPrjFile method is obsolete now. Use the static LoadPrjFile with the Uri argument method instead", true),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public SourceCoordinateSystem LoadPrjFile() {
			Uri prjUri = GetPrjUri(FileUri);
			return LoadPrjFile(prjUri);
		}
		#endregion
	}
}
