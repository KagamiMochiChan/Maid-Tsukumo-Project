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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace nse
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class Console : Window
    {

        /// <summary>
        /// インスタンス
        /// </summary>
        public Console()
        {
            core = new Core();

            InitializeComponent();

            Output = new List<string>(new string[]{"Hello World.","Norito Script Executer Console","Version 0.1.0 Code N/N"});

            Input = new List<string>();
            input = string.Empty;
            I = new Paragraph();

            Directory = Environment.CurrentDirectory;

            threads = new List<Task>();

            TextCompositionManager.AddPreviewTextInputHandler(this, OnPreviewTextInput);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(this, OnPreviewTextInputUpdate);

            WriteConsole();

            Console_IO.TextChanged += TextChanged;
        }

        private Core core;

        /// <summary>
        /// 出力のリスト
        /// </summary>
        private List<string> Output;

        /// <summary>
        /// カレントディレクトリ
        /// </summary>
        private string Directory;

        private string Prompt { get => $"{Directory} >"; }

        /// <summary>
        /// 入力
        /// </summary>
        private List<string> Input;


        /// <summary>
        /// input
        /// </summary>
        private string input;

        /// <summary>
        /// プロンプトと入力のパラグラフ
        /// </summary>
        private Paragraph I;

        private List<Task> threads;

        /// <summary>
        /// リッチテキストボックスへの書き込み
        /// </summary>
        private void WriteConsole()
        {
            Console_IO.Document.Blocks.Clear();
            Paragraph o = new Paragraph();
            foreach (string s in Output)
            {
                o.Inlines.Add(s);
                o.Inlines.Add(new LineBreak());
            }
            o.Inlines.Add(new LineBreak());
            Console_IO.Document.Blocks.Add(o);
            I = new Paragraph();
            I.Inlines.Add(Prompt+input);
            Console_IO.Document.Blocks.Add(I);
            WriteDebug();
        }

        /// <summary>
        /// キャレットが編集可能位置にあるかの判定
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        private bool IsInputArea(TextPointer pointer)
            =>
            I.ContentStart.IsInSameDocument(pointer)
            &&
            I.ContentStart.GetOffsetToPosition(pointer) - 1 >= Prompt.Length;

        /// <summary>
        /// デバック情報の書き込み
        /// </summary>
        private void WriteDebug()
        {
            Debug.Text = $@"Console Output : Line = {Output.Count}
Console Output : Count = {string.Join('\n', Output).Length}
Console Prompt : Count = {Prompt.Length}
Console Protection : Line = {Output.Count + 3}
Console Protection : Count = {$"{string.Join('\n', Output)}\n\n{Directory} >".Length}
Console Input : Count = {Input.Count}
Console Input : Count = {((Run)I.Inlines.ElementAt(0)).Text.Length}";
        }

        /// <summary>
        /// IME使用中かのフラグ
        /// </summary>
        private bool isImeOnConv = false;

        /// <summary>
        /// 反応回避用バッファ
        /// </summary>
        private bool EnterKeyBuffer = false;

        /// <summary>
        /// OnPreviewTextInputEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            EnterKeyBuffer = isImeOnConv;
            isImeOnConv = false;
        }

        /// <summary>
        /// OnPreviewTextInputUpdateEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewTextInputUpdate(object sender, TextCompositionEventArgs e) => isImeOnConv = e.TextComposition.CompositionText.Length != 0;

        /// <summary>
        /// KeyUpされたときのイベント
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e">KeyEvent</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            WriteDebug();
            switch (e.Key)
            {
                case Key.Enter:
                    if (isImeOnConv) break;
                    if (EnterKeyBuffer) EnterKeyBuffer = false;
                    else
                    {
                        string s = input;
                        Output.Add(Prompt + s);
                        WriteConsole();
                        Input.Add(s);
                        core.Execute(s);
                        e.Handled = true;
                        Console_IO.CaretPosition = I.ContentEnd;
                    }
                    break;

                case Key.Left:
                case Key.Back:
                    if (I.ContentStart.IsInSameDocument(Console_IO.CaretPosition) && I.ContentStart.GetOffsetToPosition(Console_IO.CaretPosition) - 1 == Prompt.Length)
                        e.Handled = true;
                    break;

                case Key.Down:
                    break;
                case Key.Up:
                    break;

                
                default:
                    if (!IsInputArea(Console_IO.CaretPosition))
                        if (e.Key == Key.Back || e.Key == Key.Delete)
                            e.Handled = true;
                        else
                            Console_IO.CaretPosition = I.ContentEnd;
                    break;
            }
        }

        /// <summary>
        /// 右クリックのイベント
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            var c = Clipboard.GetText();

            if (c.Contains('\n'))
            {
                foreach (var s in (input + c).Split('\n'))
                {
                    Output.Add(Prompt + s);
                    WriteConsole();
                    Input.Add(s);
                    core.Execute(s);
                    Console_IO.CaretPosition = I.ContentEnd;
                }
            }
            else
                input += c;
            e.Handled = true;
        }

        private void TextChanged(object o, TextChangedEventArgs e) => input = new TextRange(I.ContentStart.GetPositionAtOffset(Prompt.Length + 1), I.ContentEnd).Text;
    }
}
