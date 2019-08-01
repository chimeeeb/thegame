using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameLibrary.GUI.Controls
{
    public class SettingsInputFactory
    {
        private readonly Size _maximumLabelSize;
        private readonly Size _inputSize;
        private readonly Size _buttonSize;
        private readonly int _paddingLeft;
        private readonly int _gap;
        private readonly Font _labelFont;

        public int CurrentHeight { get; private set; }

        public SettingsInputFactory(
            Size inputSize, Size maximumLabelSize, Size buttonSize, Font labelFont,
            int paddingLeft, int paddingTop, int gapBetween)
        {
            _maximumLabelSize = maximumLabelSize;
            _inputSize = inputSize;
            _buttonSize = buttonSize;
            _labelFont = labelFont;
            _paddingLeft = paddingLeft;
            _gap = gapBetween + inputSize.Height;
            CurrentHeight = paddingTop;
        }

        public void NextLine()
        {
            CurrentHeight += _gap;
        }

        public Label GenerateLabel(string text)
        {
            return new Label
            {
                Location = new Point(_paddingLeft, CurrentHeight),
                MaximumSize = _maximumLabelSize,
                AutoSize = true,
                Font = _labelFont,
                Text = text
            };
        }

        public T GenerateInput<T, U>(U minValue, U maxValue) where T : NumericBox<U>, new() where U : IComparable<U>, IConvertible
        {
            return new T
            {
                Location = new Point(_maximumLabelSize.Width + _paddingLeft, CurrentHeight),
                Size = _inputSize,
                MinValue = minValue,
                MaxValue = maxValue
            };
        }

        public TextBox GenerateInput()
        {
            return new TextBox
            {
                Location = new Point(_maximumLabelSize.Width + _paddingLeft, CurrentHeight),
                Size = _inputSize
            };
        }

        public ComboBox GenerateComboBox()
        {
            return new ComboBox
            {
                Location = new Point(_maximumLabelSize.Width + _paddingLeft, CurrentHeight),
                Size = _inputSize
            };
        }

        public CheckBox GenerateCheckBox()
        {
            return new CheckBox
            {
                Location = new Point(_maximumLabelSize.Width + _paddingLeft, CurrentHeight)
            };
        }

        public IEnumerable<Button> GenerateButtons(params string[] names)
        {
            int i = 0;
            foreach (string name in names)
                yield return new Button
                {
                    Location = new Point(
                        ((2 * i++ + 1) * ((_maximumLabelSize.Width + _inputSize.Width) / names.Length) +
                         2 * _paddingLeft - _buttonSize.Width) / 2,
                        CurrentHeight),
                    Size = _buttonSize,
                    Text = name
                };
        }
    }
}