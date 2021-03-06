using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ADLGUI
{
    public partial class MainWindow : Window
    {
        private bool _parsed;
        ParserState state;
        public bool Parsed {
            get => _parsed;
            set
            {
                if(_parsed != value)
                {
                    _parsed = value;
                    Label_Parse.Content = _parsed ? "Parsed" : "Not parsed";
                    Label_Parse.Foreground = _parsed ? Brushes.Green : Brushes.Red;
                }
            }
        }
        public bool Verbose { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            state = new ParserState(null);
            Output.Target = TextBox_Output;

            TextBox_Story.Text =
                @"Fluent availablechash
Fluent isinside
Fluent haveproduct
Fluent havenewproduct
Action EXCHANGE duration 4
Action GOIN duration 2
Action SELL duration 2
Action GOOUT duration 4
Action BUY duration 2
GOIN causes [isinside] if [~isinside]";

            TextBox_Scenario.Text = @"
Let's asssume the following scenario Sc= (OBS, ACS) such that:
        OBS={{~isinside ^ ~availablecash ^ ~haveproduct, 0}}
        ACS= {{(GOIN, 1), (SELL 3), (BUY, 5), (GOOUT,9)}";
                    
        }

        private void TextBox_Story_TextChanged(object sender, TextChangedEventArgs e)
        {
            Parsed = false;
        }

        private void TextBox_Story_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

  

        private void Button_StoryParse_Click(object sender, RoutedEventArgs e)
        {
            Output.PrintSeparator();
            Output.Print("Attempting a tokenize test on the current story text...");
            try
            {
                List<Token> list = Tokenizer.Tokenize(TextBox_Story.Text);
                Output.Print(TextBox_Story.Text.Length.ToString());
                if (Verbose)
                {
                    Output.Print("Tokenize procedure finished without explicit failure.");
                    Output.Print("Tokens created:");
                    for (int i = 0; i < list.Count; i++)
                    {
                        Output.Print($"{i + 1} - {list[i].Type.ToString()}: {list[i].Name}");
                    }
                    Output.PrintNLine();
                    Output.Print("Now attempting to parse the received token list...");
                    state = Parser.Parse(list);
                    Output.Print($"Created fluents: {state.FluentList()}");
                    Output.Print($"Created actions: {state.ActionList()}");
                    Output.Print($"Created expressions: {state.ExpressionList()}");
                }
                else
                {
                    state = Parser.Parse(list);
                }
                Output.Print("Done.");
                Parsed = true;
            }
            catch(Exception ex)
            {
                Output.Print($"an error occurred\n{ex.ToString()}");
                if (ex is TokenException)
                {
                    TokenException tEx = (TokenException)ex;
                    TextBox_Story.Focus();
                    TextBox_Story.Select(
                        TextBox_Story.GetCharacterIndexFromLineIndex(tEx.LineNumber - 1)
                        + tEx.ColumnNumber - 1, tEx.TokenLength);
                    TextBox_Story.ScrollToLine(tEx.LineNumber - 1);
                }
            }
        }

        private void Button_StoryClear_Click(object sender, RoutedEventArgs e)
        {
            TextBox_Story.Text = "";
            Parsed = false;
            state = new ParserState(null);
        }

        private void Button_StoryLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog() { Filter = "MAR Language|*.mar" };
            if (saveFileDialog.ShowDialog() == true)
                TextBox_Story.Text = File.ReadAllText(saveFileDialog.FileName);
        }

        private void Button_StorySave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter="MAR Language|*.mar" };
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, TextBox_Story.Text);
        }

        private void TextBox_Query_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Button_QueryExecute_Click(object sender, RoutedEventArgs e)
        {
            //Output.Clear();
            //if (Parsed == false)
            //{
            //    Output.Print("First parse the story.");
            //    return;
            //}
            //Output.PrintSeparator();
            //Output.Print("Attempting a tokenize the query...");
            //Output.PrintSeparator();
            //try
            //{
            //    List<Token> list = Tokenizer.Tokenize(TextBox_Query.Text);
            //    if (Verbose)
            //    {
            //        Output.PrintSeparator();
            //        Output.Print("Tokenize procedure finished without explicit failure.");
            //        Output.Print("Tokens created:");
            //        for (int i = 0; i < list.Count; i++)
            //        {
            //            Output.Print($"{i + 1} - {list[i].Type.ToString()}: {list[i].Name}");
            //        }
            //        Output.PrintNLine();
            //        Output.Print("Now attempting to parse the received token list...");
            //        Output.PrintSeparator();
            //    }
            //    Output.PrintSeparator();

            //    Query q = Parser.ParseQuery(list, state);
            //    state.Q = q;
            //    Output.Clear();
            //    Output.PrintSeparator();
            //    Output.Print($"Query: {TextBox_Query.Text}");
            //    Output.PrintSeparator();
            //    Output.Print("Wait...");
                
            //    var result = q.Solve(state.Story);
            //    Output.Clear();
            //    Output.PrintSeparator();
            //    Output.Print($"Query: {TextBox_Query.Text}");
            //    Output.PrintSeparator();
            //    if (result)
            //    {
            //        Output.Print("Answer: True.");
            //    }
            //    else
            //    {
            //        Output.Print("Answer: False.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Output.Print($"an error occurred\n{ex.ToString()}");
            //    if (ex is TokenException)
            //    {
            //        TokenException tEx = (TokenException)ex;
            //        TextBox_Query.Focus();
            //        TextBox_Query.Select(
            //            TextBox_Query.GetCharacterIndexFromLineIndex(tEx.LineNumber - 1)
            //            + tEx.ColumnNumber - 1, tEx.TokenLength);
            //        TextBox_Query.ScrollToLine(tEx.LineNumber - 1);
            //    }
            //}
        }

        private void TextBox_Output_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_Output.ScrollToEnd();
        }
    }
}
