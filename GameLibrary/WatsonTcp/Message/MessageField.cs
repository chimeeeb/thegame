using System;

namespace WatsonTcp.Message
{
    public class MessageField
    {
        public int BitNumber { get; set; }
        public string Name { get; set; }
        public FieldType Type { get; set; }
        public int Length { get; set; }

        public MessageField()
        {
            throw new NotImplementedException();
        }

        public MessageField(int bitNumber, string name, FieldType fieldType, int length)
        {
            if (bitNumber < 0) throw new ArgumentException("Invalid bit number.");
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (length < 0) throw new ArgumentException("Invalid length.");

            BitNumber = bitNumber;
            Name = name;
            Type = fieldType;
            Length = length;
        }
    } 
}
