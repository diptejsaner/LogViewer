using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace LogViewer
{
    /// <summary>
    /// Interaction logic for TreeViewWindow.xaml
    /// </summary>
    public partial class TreeViewWindow : Window
    {
        public TreeViewWindow(List<Tuple<string, int>> data)
        {
            InitializeComponent();

            Stack<TreeViewItem> st = new Stack<TreeViewItem>();

            int curr_level = -1;
            TreeViewItem mainNode = new TreeViewItem();
            //data[0].Item1
            mainNode.Header = data[0].Item1;

            treeView1.Items.Add(mainNode);

            st.Push(mainNode);
            for (int i = 0; i < data.Count; i++)
            {
                if (curr_level != data[i].Item2)
                {
                    curr_level = data[i].Item2;
                    TreeViewItem treeNode = new TreeViewItem();
                    treeNode.Header = data[i].Item1;

                    st.Peek().Items.Add(treeNode);
                    st.Push(treeNode);
                }
                else
                {
                    curr_level--;
                    st.Pop();
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            //foreach (TreeViewItem tn in treeView1.Items[0])
            //{
            //    if (tn.Header.ToString().Contains(tvSearchTextBox.Text))
            //    {
            //        treeView1.SelectedNode = tn;
            //        treeView1.SelectedNode.BackColor = Color.Yellow;
            //    }
            //}
        }
    }
}
