﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EvernoteSDK;
using FirstFloor.ModernUI.Windows.Controls;
using MimeTypes;

namespace EvernoteApiSample
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : ModernWindow
  {
    private const string EVERNOTE_HOST = "www.evernote.com";
    private const string EVERNOTE_SANDBOX_HOST = "sandbox.evernote.com";

    public MainWindow()
    {
      InitializeComponent();
      InitializeEvernote();
    }

    private void InitializeEvernote()
    {
      ENSession.SetSharedSessionConsumerKey(Properties.Settings.Default.ConsumerKey, Properties.Settings.Default.ConsumerSecret, EVERNOTE_SANDBOX_HOST);
      if (ENSession.SharedSession.IsAuthenticated == false)
      {
        ENSession.SharedSession.AuthenticateToEvernote();
      }
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      FileList list = this.DataContext as FileList;
      string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
      if (files != null)
      {
        foreach (var m in
                 from s in files
                 where s.LastIndexOf(".docx") != -1 || s.LastIndexOf(".xlsx") != -1 || s.LastIndexOf(".pdf") != -1
                 select s)
        {
          list.FileNames.Add(m);
        }
        if (listBox.Items.Count > 0)
        {
          uploadButton.IsEnabled = true;
          clearButton.IsEnabled = true;
        }
      }
    }

    private void Window_PreviewDragOver(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
      {
        e.Effects = DragDropEffects.Copy;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }
      e.Handled = true;
    }

    /// <summary>
    /// Evernoteにファイルをアップロードします
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void uploadButton_Click(object sender, RoutedEventArgs e)
    {
      FileList list = this.DataContext as FileList;

      try
      {
        foreach (var f in list.FileNames)
        {
          ENNote resourceNote = new ENNote();
          resourceNote.Title = "My test note with a resource";
          resourceNote.Content = ENNoteContent.NoteContentWithString("Hello, resource!");
          byte[] file = File.ReadAllBytes(f);
          FileInfo fInfo = new FileInfo(f);
          ENResource resource = new ENResource(file, MimeTypeMap.GetMimeType(fInfo.Extension), fInfo.Name);
          resourceNote.Resources.Add(resource);
          ENNoteRef resourceRef = ENSession.SharedSession.UploadNote(resourceNote, null);
        }
      }
      catch (Exception ex)
      {
        ModernDialog.ShowMessage("アップロード中にエラーが発生しました。", "お知らせ", MessageBoxButton.OK);
        return;
      }
      ModernDialog.ShowMessage("アップロードが完了しました。", "お知らせ", MessageBoxButton.OK);
    }

    private void clearButton_Click(object sender, RoutedEventArgs e)
    {
      FileList list = this.DataContext as FileList;
      list.clear();
    }
  }

  public class FileList
  {
    public FileList()
    {
      FileNames = new ObservableCollection<string>();
    }
    public ObservableCollection<string> FileNames
    {
      get;
      private set;
    }
    public void clear()
    {
      this.FileNames.Clear();
    }
  }
}
