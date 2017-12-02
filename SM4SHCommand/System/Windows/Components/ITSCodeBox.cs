using FastColoredTextBoxNS;
using SALT.Moveset.AnimCMD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SALT.Moveset;
using System.IO;

namespace Sm4shCommand
{
    public class SubstringAutocompleteItem : AutocompleteItem
    {
        protected readonly string lowercaseText;
        protected readonly bool ignoreCase;

        public SubstringAutocompleteItem(string text, bool ignoreCase = true)
            : base(text)
        {
            this.ignoreCase = ignoreCase;
            if (ignoreCase)
                lowercaseText = text.ToLower();
        }

        public override CompareResult Compare(string fragmentText)
        {
            if (ignoreCase)
            {
                if (lowercaseText.Contains(fragmentText.ToLower()))
                    return CompareResult.Visible;
            }
            else
            {
                if (Text.Contains(fragmentText))
                    return CompareResult.Visible;
            }

            return CompareResult.Hidden;
        }
    }

    public class ITS_EDITOR : FastColoredTextBox
    {
        TextStyle keywordStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        TextStyle HexStyle = new TextStyle(Brushes.DarkCyan, null, FontStyle.Regular);
        TextStyle DecStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular);
        TextStyle StrStyle = new TextStyle(Brushes.Chocolate, null, FontStyle.Regular);
        TextStyle CommentStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Regular);

        public AutocompleteMenu AutocompleteMenu { get; set; }
        public void SetAutocomplete(string[] autocomplete)
        {
            if (autocomplete != null)
            {
                var menu = new AutocompleteMenu(this) { AppearInterval = 1 };
                menu.Items.MaximumSize = new Size(int.MaxValue, menu.Items.MaximumSize.Height);
                menu.Items.Size = this.Size;
                menu.ShowItemToolTips = true;
                menu.ToolTipDuration = int.MaxValue;
                menu.Items.SetAutocompleteItems(autocomplete);
                this.AutocompleteMenu = menu;
                this.AutoCompleteBrackets = true;
                this.AutoIndent = true;
            }
        }
        public void SetAutocomplete(AutocompleteItem[] autocomplete)
        {
            if (autocomplete != null)
            {
                var menu = new AutocompleteMenu(this) { AppearInterval = 1 };
                menu.Items.MaximumSize = new Size(int.MaxValue, menu.Items.MaximumSize.Height);
                menu.Items.Size = this.Size;
                menu.ShowItemToolTips = true;
                menu.ToolTipDuration = int.MaxValue;
                menu.Items.SetAutocompleteItems(autocomplete);

                this.AutocompleteMenu = menu;
                this.AutoCompleteBrackets = true;
                this.AutoIndent = true;
            }
        }

        public ITS_EDITOR()
        {
            this.TextChanged += NewBox_TextChanged;
            this.AutoCompleteBrackets = true;
            this.AutoIndent = true;
        }
        private void NewBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //clear previous highlighting
            e.ChangedRange.ClearStyle(StyleIndex.All);
            //highlight tags
            e.ChangedRange.SetStyle(CommentStyle, @"(\/\/.*?$|\/\*.*?\*\/)");
            e.ChangedRange.SetStyle(keywordStyle, @"\b(if|else|try|catch|MoveDef|Unlisted)\b");
            e.ChangedRange.SetStyle(HexStyle, @"0x[^\)\s,\r\n]+");
            e.ChangedRange.SetStyle(DecStyle, @"\b(?:[0-9]*\\.)?[0-9]+\b");
            e.ChangedRange.SetStyle(StrStyle, "\"(\\.|[^\"])*\"");
        }
    }
}