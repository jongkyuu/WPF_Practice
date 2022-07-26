using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.Grid;

using System.Windows.Controls;

namespace TreeView_DevExpress
{
    /// <summary>
    /// DynamicNodeLoading.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DynamicNodeLoading : UserControl
    {
        FileSystemDataProviderBase Helper { get; set; }


        public DynamicNodeLoading()
        {
            InitializeComponent();

            Helper = new FileSystemHelper();
            InitDrives();
        }

        public void InitDrives()
        {
            treeList.BeginDataUpdate();
            try
            {
                string[] root = Helper.GetLogicalDrives();

                foreach (string s in root)
                {
                    TreeListNode node = new TreeListNode() { Content = new FileSystemItem(s, "Drive", "<Drive>", s), Image = FileSystemImages.DiskImage };
                    view.Nodes.Add(node);
                    node.IsExpandButtonVisible = DefaultBoolean.True;
                    node.Tag = false;
                }
            }
            catch { }
            treeList.EndDataUpdate();
        }


        private void treeListView_NodeExpanding(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            TreeListNode node = e.Node;
            if (NodeIsFolder(node))
                node.Image = FileSystemImages.OpenedFolderImage;
            if (node.Tag == null || (bool)node.Tag == false)
            {
                InitFolder(node);
                node.Tag = true;
            }
        }

        bool NodeIsFolder(TreeListNode node)
        {
            return (node.Content as FileSystemItem).ItemType == "Folder";
        }

        private void InitFolder(TreeListNode treeListNode)
        {
            treeList.BeginDataUpdate();
            InitFolders(treeListNode);
            InitFiles(treeListNode);
            treeList.EndDataUpdate();
        }

        private void InitFiles(TreeListNode treeListNode)
        {
            FileSystemItem item = treeListNode.Content as FileSystemItem;
            if (item == null) return;
            TreeListNode node;
            try
            {
                string[] root = Helper.GetFiles(item.FullName);
                foreach (string s in root)
                {
                    node = new TreeListNode() { Content = new FileSystemItem(Helper.GetFileName(s), "File", Helper.GetFileSize(s).ToString(), s), Image = FileSystemImages.FileImage };
                    node.IsExpandButtonVisible = DefaultBoolean.False;
                    treeListNode.Nodes.Add(node);
                }
            }
            catch { }
        }

        private void InitFolders(TreeListNode treeListNode)
        {
            FileSystemItem item = treeListNode.Content as FileSystemItem;
            if (item == null) return;

            try
            {
                string[] root = Helper.GetDirectories(item.FullName);
                foreach (string s in root)
                {
                    try
                    {
                        TreeListNode node = new TreeListNode() { Content = new FileSystemItem(Helper.GetDirectoryName(s), "Folder", "<Folder>", s), Image = FileSystemImages.ClosedFolderImage };
                        treeListNode.Nodes.Add(node);
                        node.IsExpandButtonVisible = HasFiles(s) ? DefaultBoolean.True : DefaultBoolean.False;
                    }
                    catch { }
                }
            }
            catch
            {
                treeListNode.IsExpandButtonVisible = DefaultBoolean.False;
            }
        }

        private bool HasFiles(string path)
        {
            string[] root = Helper.GetFiles(path);
            if (root.Length > 0) return true;
            root = Helper.GetDirectories(path);
            if (root.Length > 0) return true;
            return false;
        }

        private void view_NodeCollapsing(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            TreeListNode node = e.Node;
            if (NodeIsFolder(node))
                node.Image = FileSystemImages.ClosedFolderImage;
        }
    }



    public abstract class FileSystemDataProviderBase
    {
        public abstract string[] GetLogicalDrives();
        public abstract string[] GetDirectories(string path);
        public abstract string[] GetFiles(string path);
        public abstract string GetDirectoryName(string path);
        public abstract string GetFileName(string path);
        public abstract string GetFileSize(string path);
        internal string GetFileSize(long size)
        {
            if (size >= 1024)
                return string.Format("{0:### ### ###} KB", size / 1024);
            return string.Format("{0} Bytes", size);
        }
    }

    public class FileSystemHelper : FileSystemDataProviderBase
    {

        public override string[] GetLogicalDrives()
        {
            //return Directory.GetLogicalDrives();
            return new string[] { @"D:\JK\Work\RecipeCreator\RecipeCreator_Job\22th_BatchRunMergeCSV" };
        }

        public override string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public override string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public override string GetDirectoryName(string path)
        {
            return new DirectoryInfo(path).Name;
        }

        public override string GetFileName(string path)
        {
            return new FileInfo(path).Name;
        }

        public override string GetFileSize(string path)
        {
            long size = new FileInfo(path).Length;
            return GetFileSize(size);
        }
    }



    public class FileSystemItem
    {
        public FileSystemItem(string name, string type, string size, string fullName)
        {
            this.Name = name;
            this.ItemType = type;
            this.Size = size;
            this.FullName = fullName;
        }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string Size { get; set; }
        public string FullName { get; set; }
    }
    public class FileSystemImages
    {
        static ImageSource fileImage;
        public static ImageSource FileImage
        {
            get
            {
                if (fileImage == null)
                {
                    var uri = DXImageHelper.GetImageUri(@"Images/Arrange/WithTextWrapping_BottomCenter_16x16.png");
                    fileImage = new BitmapImage(uri);
                }

                return fileImage;
            }
        }
        static ImageSource diskImage;
        public static ImageSource DiskImage
        {
            get
            {
                if (diskImage == null)
                {
                    var uri = DXImageHelper.GetImageUri(@"Images/Actions/Group_32x32.png");
                    diskImage = new BitmapImage(uri);
                }
                    //diskImage = ImageHelper.GetImage("Local_Disk");
                return diskImage;
            }
        }
        static ImageSource closedFolderImage;
        public static ImageSource ClosedFolderImage
        {
            get
            {
                if (closedFolderImage == null)
                {
                    var uri = DXImageHelper.GetImageUri(@"Images/Actions/Open_16x16.png");
                    closedFolderImage = new BitmapImage(uri);
                }

                //closedFolderImage = ImageHelper.GetImage("Images/Actions/Open_16x16.png");
                return closedFolderImage;
            }
        }
        static ImageSource openedFolderImage;
        public static ImageSource OpenedFolderImage
        {
            get
            {
                if (openedFolderImage == null)
                {
                    var uri = DXImageHelper.GetImageUri(@"Images/Actions/Open2_16x16.png");
                    openedFolderImage = new BitmapImage(uri);
                }
                    //openedFolderImage = ImageHelper.GetImage("Images/Actions/Open2_16x16.png");
                return openedFolderImage;
            }
        }
    }
}
