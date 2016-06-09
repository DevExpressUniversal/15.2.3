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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.Utils.Drawing.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Microsoft.Win32;
using System.Linq;
namespace DevExpress.Utils.Helpers {
	public static class FileSystemHelper {
		static readonly string ShellFoldersKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders";
		static readonly string DownloadsDirKeyName = "{374DE290-123F-4565-9164-39C4925E467B}";
		public static string GetDownloadsDir() {
			RegistryKey key = Registry.CurrentUser.OpenSubKey(ShellFoldersKey);
			if(key == null) return null;
			return (string)key.GetValue(DownloadsDirKeyName);
		}
		public static bool IsDirExists(string dir) {
			return Directory.Exists(dir);
		}
		public static string[] GetSubFolders(string rootDir) {
			string[] subDirs = GetSubDirs(rootDir);
			if(subDirs == null)
				return new string[0];
			if(subDirs.Length <= MaxEntitiesCount)
				return subDirs;
			string[] res = new string[MaxEntitiesCount];
			Array.Copy(subDirs, res, res.Length);
			return res;
		}
		public static string GetDirName(string path) {
			if(string.IsNullOrEmpty(path)) return string.Empty;
			return new DirectoryInfo(path).Name;
		}
		public static string[] GetSubDirs(string dir) {
			FileSystemEntryCollection col = new FileSystemEntryCollection();
			InitDirectories(dir, col, IconSizeType.Small, Size.Empty, false);
			return (from obj in col select obj.Name).ToArray<string>();
		}
		public static void Run(string path) {
			try {
				Process.Start(path);
			}
			catch { }
		}
		public static IEnumerable<DriveInfo> GetFixedDrives() {
			foreach(DriveInfo driveInfo in DriveInfo.GetDrives()) {
				if(driveInfo.DriveType != DriveType.Fixed) continue;
				yield return driveInfo;
			}
		}
		public static void ShellExecuteFileInfo(string path, ShellExecuteInfoFileType type) {
			ShellExecuteInfo.ShowShellExecuteFileInfo(path, type);
		}
		public static readonly int MaxEntitiesCount = 80;
		public static FileSystemEntryCollection GetFileSystemEntries(string path, IconSizeType sizeType, Size itemSize) {
			FileSystemEntryCollection col = new FileSystemEntryCollection();
			InitDirectories(path, col, sizeType, itemSize);
			InitFiles(path, col, sizeType, itemSize);
			return col;
		}
		public static void InitFiles(string path, FileSystemEntryCollection col, IconSizeType sizeType, Size itemSize) {
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			FileInfo[] files = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
			for(int i = 0; i < files.Length && i < MaxEntitiesCount; i++) {
				FileInfo fileInfo = files[i];
				if(!MatchFilter(fileInfo.Attributes)) continue;
				col.Add(new FileEntry(fileInfo.Name, fileInfo.FullName, GetImage(fileInfo.FullName, sizeType, itemSize)));
			}
		}
		public static void InitDirectories(string path, FileSystemEntryCollection col, IconSizeType sizeType, Size itemSize) {
			InitDirectories(path, col, sizeType, itemSize, true);
		}
		public static void InitDirectories(string path, FileSystemEntryCollection col, IconSizeType sizeType, Size itemSize, bool getIcons) {
			DirectoryInfo dirInfo = new DirectoryInfo(path);
			DirectoryInfo[] dirs = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
			for(int i = 0; i < dirs.Length && i < MaxEntitiesCount; i++) {
				DirectoryInfo subDirInfo = dirs[i];
				if(!CheckAccess(subDirInfo) || !MatchFilter(subDirInfo.Attributes)) continue;
				col.Add(new DirectoryEntry(subDirInfo.Name, subDirInfo.FullName, getIcons ? GetImage(subDirInfo.FullName, sizeType, itemSize) : null));
			}
		}
		public static bool MatchFilter(FileAttributes attributes) {
			return (attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0;
		}
		public static bool CheckAccess(DirectoryInfo info) {
			bool isOk = false;
			try {
				var secInfo = info.GetAccessControl();
				isOk = true;
			}
			catch { }
			return isOk;
		}
		public static Image GetImage(string path, IconSizeType sizeType, Size itemSize) {
			return FileSystemImageCache.Cache.GetImage(path, sizeType, itemSize);
		}
	}
	public class FileSystemEntryCollection : Collection<FileSystemEntry> {
		bool showExtensions;
		public FileSystemEntryCollection() {
			this.showExtensions = false;
		}
		public bool ShowExtensions {
			get { return showExtensions; }
			set {
				if(ShowExtensions == value)
					return;
				showExtensions = value;
				OnShowExtensionsChanged();
			}
		}
		protected virtual void OnShowExtensionsChanged() { }
		protected override void InsertItem(int index, FileSystemEntry item) {
			base.InsertItem(index, item);
			item.SetOwner(this);
		}
		protected override void ClearItems() {
			base.ClearItems();
		}
		protected override void RemoveItem(int index) {
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, FileSystemEntry item) {
			base.SetItem(index, item);
		}
	}
	public enum IconSizeType {
		Medium = 0x0,
		Small = 0x1,
		Large = 0x2,
		ExtraLarge = 0x4
	}
	public class FileSytemEntryComparer : IComparer<FileSystemEntry> {
		public int Compare(FileSystemEntry x, FileSystemEntry y) {
			if(x.GetType() == y.GetType()) return 0;
			return x.GetType() == typeof(FileEntry) ? -1 : 1;
		}
	}
	public abstract class FileSystemEntry : INotifyPropertyChanged {
		bool isCheck;
		Image image;
		string name, shortName, path, group;
		FileSystemEntryCollection owner;
		public FileSystemEntry(string name, string path, Image image) {
			this.name = name;
			this.shortName = GetShortName(name);
			this.path = path;
			this.image = image;
			this.isCheck = false;
			this.group = string.Empty;
			this.owner = null;
		}
		protected internal void SetOwner(FileSystemEntryCollection owner) { this.owner = owner; }
		public void SetCheck(bool check) {
			this.isCheck = check;
		}
		public void SetGroup(string group) {
			this.group = group;
		}
		public string Name {
			get { return owner.ShowExtensions ? name : shortName; }
			set {
				if(Name == value)
					return;
				name = value;
				shortName = GetShortName(value);
				OnPropertyChanged("Name");
			}
		}
		public string Group {
			get { return group; }
			set {
				if(Group == value)
					return;
				group = value;
				OnPropertyChanged("Group");
			}
		}
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				OnPropertyChanged("Image");
			}
		}
		public string Path {
			get { return path; }
			set {
				if(Path == value)
					return;
				path = value;
				OnPropertyChanged("Path");
			}
		}
		public bool IsCheck {
			get { return isCheck; }
			set {
				if(IsCheck == value)
					return;
				isCheck = value;
				OnPropertyChanged("IsCheck");
			}
		}
		public void ShowProperties() {
			ShellExecuteInfo.ShowShellExecuteFileInfo(Path, ShellExecuteInfoFileType.Properties);
		}
		public abstract void DoAction(IFileSystemNavigationSupports ownerCore);
		public abstract string GetShortName(string name);
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	public class FileEntry : FileSystemEntry {
		public FileEntry(string text, string description, Image image)
			: base(text, description, image) {
		}
		public override void DoAction(IFileSystemNavigationSupports ownerCore) {
			ShellExecuteInfo.ShowShellExecuteFileInfo(Path, ShellExecuteInfoFileType.Open);
		}
		public override string GetShortName(string name) {
			return System.IO.Path.GetFileNameWithoutExtension(name);
		}
	}
	public class DirectoryEntry : FileSystemEntry {
		public DirectoryEntry(string text, string description, Image image)
			: base(text, description, image) {
		}
		public override void DoAction(IFileSystemNavigationSupports ownerCore) {
			if(string.IsNullOrEmpty(Name)) return;
			string newPath = System.IO.Path.Combine(ownerCore.CurrentPath, Name);
			ownerCore.UpdatePath(newPath);
		}
		public override string GetShortName(string name) {
			return name;
		}
	}
	public interface IFileSystemNavigationSupports {
		void UpdatePath(string path);
		string CurrentPath { get; }
	}
	public class FileSystemImageCache {
		Dictionary<string, Image> hashTable;
		protected FileSystemImageCache() {
			this.hashTable = new Dictionary<string, Image>();
		}
		static FileSystemImageCache cacheCore = null;
		public static FileSystemImageCache Cache {
			get {
				if(cacheCore == null) cacheCore = new FileSystemImageCache();
				return cacheCore;
			}
		}
		public Image GetImage(string path, IconSizeType sizeType, Size itemSize) {
			string key = GetKey(path);
			if(key != null && this.hashTable.ContainsKey(key)) {
				return (Image)hashTable[key];
			}
			Image img = FileSystemImageHelper.GetFileImage(path, sizeType, itemSize);
			if(key != null) this.hashTable[key] = img;
			return img;
		}
		protected virtual string GetKey(string path) {
			string extension = Path.GetExtension(path);
			if(string.IsNullOrEmpty(extension)) return null;
			if(string.Equals(extension, ".lnk", StringComparison.Ordinal)) return null;
			return extension;
		}
		public void ClearCache() {
			hashTable.Clear();
		}
	}
	[System.Security.SecuritySafeCritical]
	static class FileSystemImageHelper {
		#region Constants
		const uint ILD_TRANSPARENT = 0x00000001;
		const uint SHGFI_ICON = 0x000000100;
		const uint SHGFI_DISPLAYNAME = 0x000000200;
		const uint SHGFI_TYPENAME = 0x000000400;
		const uint SHGFI_ATTRIBUTES = 0x000000800;
		const uint SHGFI_ICONLOCATION = 0x000001000;
		const uint SHGFI_EXETYPE = 0x000002000;
		const uint SHGFI_SYSICONINDEX = 0x000004000;
		const uint SHGFI_LINKOVERLAY = 0x000008000;
		const uint SHGFI_SELECTED = 0x000010000;
		const uint SHGFI_ATTR_SPECIFIED = 0x000020000;
		const uint SHGFI_LARGEICON = 0x000000000;
		const uint SHGFI_SMALLICON = 0x000000001;
		const uint SHGFI_OPENICON = 0x000000002;
		const uint SHGFI_SHELLICONSIZE = 0x000000004;
		const uint SHGFI_PIDL = 0x000000008;
		const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
		const uint SHGFI_ADDOVERLAYS = 0x000000020;
		const uint SHGFI_OVERLAYINDEX = 0x000000040;
		#endregion
		public static Icon GetFileIcon(string path, IconSizeType sizeType, Size itemSize) {
			SHFILEINFO shinfo = new SHFILEINFO();
			IntPtr retVal = SHGetFileInfo(path, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), (int)(SHGFI_SYSICONINDEX | SHGFI_ICON));
			int iconIndex = shinfo.iIcon;
			IImageList iImageList = (IImageList)GetSystemImageListHandle(sizeType);
			IntPtr hIcon = IntPtr.Zero;
			if(iImageList != null) {
				iImageList.GetIcon(iconIndex, (int)ILD_TRANSPARENT, ref hIcon);
				Marshal.ReleaseComObject(iImageList);
			}
			Icon icon = null;
			if(hIcon != IntPtr.Zero) {
				icon = Icon.FromHandle(hIcon).Clone() as Icon;
				DestroyIcon(shinfo.hIcon);
			}
			return icon;
		}
		public static Image GetFileImage(string path, IconSizeType sizeType, Size itemSize) {
			return IconToBitmap(GetFileIcon(path, sizeType, itemSize), sizeType, itemSize);
		}
		public static Image IconToBitmap(Icon ico, IconSizeType sizeType, Size itemSize) {
			Image res = null;
			if(ico == null) return new Bitmap(itemSize.Width, itemSize.Height);
			Bitmap src = ico.ToBitmap();
			int sizeReal = CalcRealSize(src);
			int desiredSize = GetDesiredSize(sizeType);
			if(sizeReal > desiredSize) {
				res = ResizeBitmap(src, itemSize);
				src.Dispose();
				return res;
			}
			if(Math.Abs(sizeReal - desiredSize) < DeltaSize) return src;
			res = MakeBorder(itemSize, src, sizeReal);
			src.Dispose();
			return res;
		}
		static int DeltaSize { get { return 50; } }
		static Image MakeBorder(Size itemSize, Bitmap bmpSource, int sizeReal) {
			using(Bitmap bmpSmall = bmpSource.Clone(new Rectangle(0, 0, sizeReal, sizeReal), bmpSource.PixelFormat)) {
				Bitmap result = new Bitmap(itemSize.Width, itemSize.Height);
				using(Graphics g = Graphics.FromImage(result)) {
					g.DrawRectangle(Pens.LightGray, 0, 0, itemSize.Width - 1, itemSize.Height - 1);
					Point pt = new Point(result.Width / 2 - bmpSmall.Width / 2, result.Width / 2 - bmpSmall.Width / 2);
					g.DrawImage(bmpSmall, pt);
				}
				return result;
			}
		}
		static int CalcRealSize(Bitmap bmp) {
			for(int i = bmp.Height - 1; i >= 0; i--) {
				for(int j = 0; j < bmp.Width; j++) {
					if(bmp.GetPixel(i, j).A != 0 || bmp.GetPixel(j, i).A != 0) {
						return i;
					}
				}
			}
			return Math.Max(bmp.Width, bmp.Height);
		}
		public static Bitmap ResizeBitmap(Bitmap bmpSource, Size newSize) {
			Bitmap result = new Bitmap(newSize.Width, newSize.Height);
			using(Graphics g = Graphics.FromImage(result))
				g.DrawImage(bmpSource, 0, 0, newSize.Width, newSize.Height);
			return result;
		}
		static int GetDesiredSize(IconSizeType sizeType) {
			switch(sizeType) {
				case IconSizeType.Medium: return 32;
				case IconSizeType.Small: return 16;
				case IconSizeType.Large: return 48;
				case IconSizeType.ExtraLarge: return 254;
			}
			return 0;
		}
		static IImageList GetSystemImageListHandle(IconSizeType sizeType) {
			IImageList iImageList = null;
			Guid imageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
			int ret = SHGetImageList((int)sizeType, ref imageListGuid, ref iImageList);
			return iImageList;
		}
		[ComImport(), Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		interface IImageList {
			[PreserveSig]
			int Add(IntPtr hbmImage, IntPtr hbmMask, ref int pi);
			[PreserveSig]
			int ReplaceIcon(int i, IntPtr hicon, ref int pi);
			[PreserveSig]
			int SetOverlayImage(int iImage, int iOverlay);
			[PreserveSig]
			int Replace(int i, IntPtr hbmImage, IntPtr hbmMask);
			[PreserveSig]
			int AddMasked(IntPtr hbmImage, int crMask, ref int pi);
			[PreserveSig]
			int Draw(ref IMAGELISTDRAWPARAMS pimldp);
			[PreserveSig]
			int Remove(int i);
			[PreserveSig]
			int GetIcon(int i, int flags, ref IntPtr picon);
			[PreserveSig]
			int GetImageInfo(int i, ref IMAGEINFO pImageInfo);
			[PreserveSig]
			int Copy(int iDst, IImageList punkSrc, int iSrc, int uFlags);
			[PreserveSig]
			int Merge(int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int Clone(ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int GetImageRect(int i, ref NativeMethods.RECT prc);
			[PreserveSig]
			int GetIconSize(ref int cx, ref int cy);
			[PreserveSig]
			int SetIconSize(int cx, int cy);
			[PreserveSig]
			int GetImageCount(ref int pi);
			[PreserveSig]
			int SetImageCount(int uNewCount);
			[PreserveSig]
			int SetBkColor(int clrBk, ref int pclr);
			[PreserveSig]
			int GetBkColor(ref int pclr);
			[PreserveSig]
			int BeginDrag(int iTrack, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int EndDrag();
			[PreserveSig]
			int DragEnter(IntPtr hwndLock, int x, int y);
			[PreserveSig]
			int DragLeave(IntPtr hwndLock);
			[PreserveSig]
			int DragMove(int x, int y);
			[PreserveSig]
			int SetDragCursorImage(ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);
			[PreserveSig]
			int DragShowNolock(int fShow);
			[PreserveSig]
			int GetDragImage(ref NativeMethods.POINT ppt, ref NativeMethods.POINT pptHotspot, ref Guid riid, ref IntPtr ppv);
			[PreserveSig]
			int GetItemFlags(int i, ref int dwFlags);
			[PreserveSig]
			int GetOverlayImage(int iOverlay, ref int piIndex);
		};
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool DestroyIcon(IntPtr hIcon);
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
		[DllImport("shell32.dll", EntryPoint = "#727")]
		static extern int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);
	}
	public enum ShellExecuteInfoFileType {
		Edit,
		Explore,
		Find,
		Open,
		Print,
		Properties
	}
	[System.Security.SecuritySafeCritical]
	static class ShellExecuteInfo {
		const int SW_SHOW = 5;
		const uint SEE_MASK_INVOKEIDLIST = 12;
		internal static bool ShowShellExecuteFileInfo(string Filename, ShellExecuteInfoFileType type) {
			SHELLEXECUTEINF info = new SHELLEXECUTEINF();
			info.cbSize = Marshal.SizeOf(info);
			info.lpVerb = type.ToString();
			info.lpFile = Filename;
			info.nShow = SW_SHOW;
			info.fMask = SEE_MASK_INVOKEIDLIST;
			return ShellExecuteEx(ref info);
		}
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		static extern bool ShellExecuteEx(ref SHELLEXECUTEINF lpExecInfo);
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	struct SHELLEXECUTEINF {
		public int cbSize;
		public uint fMask;
		public IntPtr hwnd;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpVerb;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpFile;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpParameters;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpDirectory;
		public int nShow;
		public IntPtr hInstApp;
		public IntPtr lpIDList;
		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpClass;
		public IntPtr hkeyClass;
		public uint dwHotKey;
		public IntPtr hIcon;
		public IntPtr hProcess;
	}
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	struct SHFILEINFO {
		public IntPtr hIcon;
		public int iIcon;
		public uint dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}
	[StructLayout(LayoutKind.Sequential)]
	struct IMAGELISTDRAWPARAMS {
		public int cbSize;
		public IntPtr himl;
		public int i;
		public IntPtr hdcDst;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public int xBitmap;
		public int yBitmap;
		public int rgbBk;
		public int rgbFg;
		public int fStyle;
		public int dwRop;
		public int fState;
		public int Frame;
		public int crEffect;
	}
	[StructLayout(LayoutKind.Sequential)]
	struct IMAGEINFO {
		public IntPtr hbmImage;
		public IntPtr hbmMask;
		public int Unused1;
		public int Unused2;
		public NativeMethods.RECT rcImage;
	}
}
