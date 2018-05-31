using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CAD
{
    public partial class Form1 : Form
    {
        private LibraryManager libraryManager;
        private Canvas CanvasMain;

        public Form1()
        {
            InitializeComponent();                          //初始化
            libraryManager = new LibraryManager();            //元件库管理
        }

#region 元件库panel的缩放，关闭操作
        private bool IsContain_Panel2 = false;
        private bool IsDown = false;
        private Point startPoint;

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if(IsContain_Panel2)
            {
                if (e.Location.X >= this.panel2.Width - 3)
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
                if(IsDown)
                {
                    this.panel2.Size = new Size(e.X, this.panel2.Height);
                }
            }
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            this.IsContain_Panel2 = false;
            this.Cursor = Cursors.Default;
        }

        private void panel2_MouseEnter(object sender, EventArgs e)
        {
            this.IsContain_Panel2 = true;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if(this.Cursor == Cursors.SizeWE)
            {
                this.IsDown = true;
            }
            startPoint = Cursor.Position;
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            this.IsDown = false;
        }

        private bool IsClick = false;
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            foreach(ToolStripButton btn in toolStrip2.Items)
            {
                btn.Checked = false;
            }
            IsClick = !IsClick;
            if(IsClick)
            {
                this.Listview1_ItemLoad();
                this.panel2.Visible = true;
                this.toolStripButton2.BackColor = Color.LightGray;
            }
            else
            {
                this.panel2.Visible = false;
                this.toolStripButton2.BackColor = Color.White;
                ElementTool.elementToolState = ElementToolStyle.Select;
            }
        }
        #endregion

        int index = 0;
        private void Listview1_ItemLoad()
        {
            this.listView1.Items.Clear();
            this.imageList1.Images.Clear();
            index = 0;
            listView1.LargeImageList = imageList1;
            foreach (FileInfo info in CanvasMain.CanvasHelper.RefreshList())
            {
                ListViewItem item = new ListViewItem();
                item.Text = info.Name.Split('.')[0];
                imageList1.Images.Add(libraryManager.GetIcon(info.Name));
                item.ImageIndex = index;
                item.Tag = info.Name;
                listView1.Items.Add(item);
                index++;
            }
        }

        /*
         * 主窗体加载事件
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            InitialCanvas();
        }
        /*
         * 初始化画布
         */
        private void InitialCanvas()
        {
            CanvasMain = new Canvas(new Size(this.panelContent.Width,this.panelContent.Height));
            CanvasMain.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(CanvasMain);
        }

#region 按钮的集合
        //新建文件
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //打开文件
        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        //新建文件--快捷按钮
        private void toolStripButton3_Click(object sender, EventArgs e)
        {

        }
        //选择按钮
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ElementTool.elementToolState = ElementToolStyle.Select;
        }
        //链路按钮
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ElementTool.elementToolState = ElementToolStyle.Link;
        }
        //撤销
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            CanvasMain.commandManager.UndoCommand();
        }
        //恢复
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CanvasMain.commandManager.RedoCommand();
        }

        #endregion
        /*
         * 元件库拖拽开始
         */
        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ElementTool.elementToolState = ElementToolStyle.Component;
            ListViewItem listitem = (ListViewItem)e.Item;
            CanvasMain.CanvasHelper.ReadxmlCreateComponent(listitem.Tag.ToString());
            this.listView1.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        /*
         * 元件库列表点击
         */
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            ElementTool.elementToolState = ElementToolStyle.Component;
            ListViewHitTestInfo info = listView1.HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                CanvasMain.CanvasHelper.ReadxmlCreateComponent(info.Item.Tag.ToString());
            }
        }
        /*
         * 内容区域拖拽进入
         */
        private void panelContent_DragEnter(object sender, DragEventArgs e)
        {
            CanvasMain.DragEnter(e);
        }
        /*
         * 内容区域拖拽完成
         */
        private Point screenPoint;
        private void panelContent_DragDrop(object sender, DragEventArgs e)
        {
            screenPoint = this.CanvasMain.PointToScreen(new Point(0,0));
            CanvasMain.DragDrop(e, screenPoint);
        }

        /*
         * 按下Delete键删除操作
         */
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                CanvasMain.RemoveElement();
            }
        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            foreach(ToolStripButton button in toolStrip2.Items)
            {
                button.Checked = false;
            }
            ToolStripButton toolScriptButton = (ToolStripButton)e.ClickedItem;
            toolScriptButton.Checked = true;
        }

        private void 添加元件到库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        private void 刷新toolStripMenuItem_Click(object sender, EventArgs e)
        {
            Listview1_ItemLoad();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            CanvasMain.MultipleIncrease();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            CanvasMain.MutipleDecrease();
        }
    }
}