using System;
using System.Globalization;
using System.Windows.Forms;

namespace GameLibrary.GUI.Controls
{
    /// <summary>
    /// Abstract controller for numeric input
    /// </summary>
    public abstract class NumericBox<T> : TextBox where T : IComparable<T>, IConvertible
    {
        /// <summary>
        /// Current minimal value of the input
        /// </summary>
        protected T _minValue;

        /// <summary>
        /// Current maximal value of the input
        /// </summary>
        protected T _maxValue;
        /// <summary>
        /// Current value of the input
        /// </summary>
        protected T _value;

        /// <summary>
        /// Gets or sets minimal value of the input
        /// </summary>
        public T MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                if (value.CompareTo(Value) > 0)
                    Value = value;
            }
        }
        /// <summary>
        /// Gets or sets maximal value of the input
        /// </summary>
        public T MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                if (value.CompareTo(Value) < 0)
                    Value = value;
            }
        }

        /// <summary>
        /// Gets or sets value of the input
        /// </summary>
        public abstract T Value { get; set; }

        /// <summary>
        /// Occurs when the value of the control changes
        /// </summary>
        public event EventHandler ValueChanged;

        protected void OnValueChanged(object sender, EventArgs e)
        {
            if (_value.CompareTo(MinValue) < 0) _value = MinValue;
            if (_value.CompareTo(MaxValue) > 0) _value = MaxValue;

            EventHandler handler = ValueChanged;
            handler?.Invoke(sender, e);
        }
    }

    /// <summary>
    /// A controller for numeric input
    /// </summary>
    public class NumericBoxInt : NumericBox<int>
    {
        /// <summary>
        /// Gets or sets value of the input
        /// </summary>
        public override int Value
        {
            get
            {
                if (!int.TryParse(Text, out _value)) _value = 0;
                return _value;
            }
            set
            {
                _value = value;
                Text = _value.ToString();
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) e.Handled = true;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (long.TryParse(Text, out var res))
            {
                if (Text.Length > 10 || res > MaxValue)
                    Value = MaxValue;
                if (Text.Length > 10 || res < MinValue)
                    Value = MinValue;
            }
        }
    }



    /// <summary>
    /// A controller for numeric input
    /// </summary>
    public class NumericBoxDouble : NumericBox<double>
    {
        /// <summary>
        /// Gets or sets value of the input
        /// </summary>
        public override double Value
        {
            get
            {
                if (!double.TryParse(Text, NumberStyles.Float, CultureInfo.InvariantCulture, out _value)) _value = 0;
                return _value;
            }
            set
            {
                _value = value;
                Text = _value.ToString(CultureInfo.InvariantCulture);
                OnValueChanged(this, EventArgs.Empty);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) e.Handled = true;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (long.TryParse(Text, out var res))
            {
                if (Text.Length > 10 || res > MaxValue)
                    Value = MaxValue;
                if (Text.Length > 10 || res < MinValue)
                    Value = MinValue;
            }
        }
    }
}